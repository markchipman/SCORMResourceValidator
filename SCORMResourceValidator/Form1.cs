using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.IO.Compression;
using System.IO;
using System.Windows.Forms;
using System.Net;
using System.Security.Cryptography;
using System.Xml.Schema;
using System.Xml.XPath;

namespace SCORMResourceValidator
{
    public partial class Form1 : Form
    {
        private List<string> fileWhitelist = new List<string>();
        private List<string> logFilelist = new List<string>();
        private int intPIFFilesFoundCount = 0, intManifestFilesFoundCount = 0, intPIFFilesMissingCount = 0, intManifestFilesMissingCount = 0;
        private string strPIFFilename = "", strPIFFilenameNoExt = "", strLogDir = "";
        private Boolean debugMode = false;
        private int numMetadatafiles = 0;
        private int numContentAgMetadatafiles = 0;
        private int numSCOmetadatafiles = 0;
        private int numAssetmetadatafiles = 0;
        private IEnumerable<XElement> ContentAgmetafiles;
        private IEnumerable<XElement> SCOmetafiles;
        private IEnumerable<XElement> Assetmetafiles;
        private List<string> validXMLfiles = new List<string>();
        private List<string> invalidXMLfiles = new List<string>();
        private List<string> metadataXMLfiles = new List<string>();
        private int numvalidmetadatafiles = 0;
        private int numinvalidmetadatafiles = 0;
        private int nummissingmetadatafiles = 0;
        private string metadatafiles_errors = "";
        private List<string> metadatafilesErrors = new List<string>();
        private List<string> metadataFilesMissing = new List<string>();
        private bool ShouldBeQuiz = false;

        //vars for parsing the ADL test suite logs
        private string ADLTestSuiteSummaryfile = "";
        private string ADLTestSuiteDetailsfile = "";
        private string ADLTestSuiteLuanchDetailsfile = "";
        private bool AllADLTestSuitefilesfound = false;
        private bool CPCTS_conformant = true;//Content Package Conformance Test Summary
        private bool CPCTD_conformant = true;//Content Package Conformance Test Details
        private bool SCO_runtime_conformant = true;//Sharable Content Object (SCO) Run-Time Environment Conformance Test
        private bool SCO_conformant = true;//have to have two of these because the previous one comes from the launch detail file  and the other from the summary file
        private bool Metadatafile_conformant = true;//Metadata validate Conformance
        public struct LogValues
        {
            public string item_name;
            public int twritten_Count;
            public string twritten_value;
            public int trecieved_Count;
            public string trecieved_value;
            public string error;
        };
        private List<SCOInfo> SCOList = new List<SCOInfo>();
        private List<LogValues> mainCGIcalls = new List<LogValues>();
        private List<LogValues> extraCGIcalls = new List<LogValues>();
        struct RuntimeErrormsgs
        {
            public string sco;
            public string item;
            public string error;
        }
        List <RuntimeErrormsgs> SCORuntimeErrors = new List<RuntimeErrormsgs>();

        public Form1()
        {
            InitializeComponent();

            //default SCORM 1.2 and 2004 3rd edition package files to ignore
            string[] SCORMSchemas = {
                "adlcp_rootv1p2.xsd",
                "adlcp_v1p3.xsd",
                "adlnav_v1p3.xsd",
                "adlseq_v1p3.xsd",
                "anyElement.xsd",
                "custom.xsd",
                "dataTypes.xsd",
                "elementNames.xsd",
                "elementTypes.xsd",
                "imscp_rootv1p1p2.xsd",
                "imscp_v1p1.xsd",
                "imsmanifest.xml",
                "imsmd_rootv1p2p1.xsd",
                "imsssp_v1p0.xsd",
                "imsss_v1p0.xsd",
                "imsss_v1p0auxresource.xsd",
                "imsss_v1p0control.xsd",
                "imsss_v1p0delivery.xsd",
                "imsss_v1p0limit.xsd",
                "imsss_v1p0objective.xsd",
                "imsss_v1p0random.xsd",
                "imsss_v1p0rollup.xsd",
                "imsss_v1p0seqrule.xsd",
                "imsss_v1p0util.xsd",
                "ims_xml.xsd",
                "lom.xsd",
                "lomCustom.xsd",
                "lomLoose.xsd",
                "lomStrict.xsd",
                "loose.xsd",
                "rootElement.xsd",
                "strict.xsd",
                "vocabTypes.xsd",
                "vocabValues.xsd",
                "xml.xsd",
                "datatypes.dtd",
                "XMLSchema.dtd"
               };

            logFilelist.Add("manifest_files_found.html");
            logFilelist.Add("manifest_files_missing.html");
            logFilelist.Add("packaged_files_found.html");
            logFilelist.Add("packaged_files_missing.html");
            //logFilelist.Add("metadata_file_report.doc");
            logFilelist.Add("ValidateMD.doc");
            logFilelist.Add("PIF_file_validate.txt");


            fileWhitelist.AddRange(SCORMSchemas);
           
            grpPIFFilesFound.Text = "Files found in PIF";
            grpManifestFilesFound.Text = "Resources found in Manifest";
            grpPIFFilesMissing.Text = "Files found in Manifest and not found in PIF";
            grpManifestFilesMissing.Text = "Files found in PIF and not found in Manifest";

            lblStatus.Text = "";
            lblStatus.Visible = true;            
        }

        private void SelectPIF_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            displaySelectedPIFInfo();
        }


        private void btnSelectLogDir_Click(object sender, EventArgs e)
        {
            if (logFolderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                lblLogFileDir.Text = logFolderBrowserDialog.SelectedPath;
            }
            
        }

        private void btnSelectADLLogDir_Click(object sender, EventArgs e)
        {
            if (ADLlogFolderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                lblADLlogFileDir.Text = ADLlogFolderBrowserDialog.SelectedPath;
            }
            
        }


        private void btnValidate_Click(object sender, EventArgs e)
        {
          
            if (readPIFFile())
            {
                turnOnGUI();
                createLogs();
                compareLogs();
                parseLogs();
            }
        }

        /// <summary>
        /// Clears lists and file counts.
        /// </summary>
        private void resettheForm()
        {
            //Lists and counts
            intPIFFilesFoundCount = 0;
            listPIFFilesFound.Items.Clear();
            grpPIFFilesFound.Text = "Files found in PIF";

            intManifestFilesFoundCount = 0;
            listManifestFilesFound.Items.Clear();
            grpManifestFilesFound.Text = "Resources found in Manifest";

            intPIFFilesMissingCount = 0;
            listPIFFilesMissing.Items.Clear();
            grpPIFFilesMissing.Text = "Files found in Manifest and not found in PIF";
            grpPIFFilesMissing.ForeColor = SystemColors.ControlText;
            listPIFFilesMissing.BackColor = SystemColors.ControlLight;

            intManifestFilesMissingCount = 0;
            listManifestFilesMissing.Items.Clear();
            grpManifestFilesMissing.Text = "Files found in PIF and not found in Manifest";
            grpManifestFilesMissing.ForeColor = SystemColors.ControlText;
            listManifestFilesMissing.BackColor = SystemColors.ControlLight;

            lblStatus.Text = "";
            ShouldBeQuiz = false;

            //Metadata counts
            numMetadatafiles = 0;
            numContentAgMetadatafiles = 0;
            numSCOmetadatafiles = 0;
            numAssetmetadatafiles = 0;

            nummissingmetadatafiles = 0;
            numvalidmetadatafiles = 0;
            numinvalidmetadatafiles = 0;

            validXMLfiles.Clear();
            invalidXMLfiles.Clear();
            metadataXMLfiles.Clear();
            metadatafilesErrors.Clear();


            //optional Log file folder chooser
            logFolderBrowserDialog.Reset();
            ADLlogFolderBrowserDialog.Reset();
            lblLogFileDir.Text = "No Directory selected";
            lblADLlogFileDir.Text = "No Directory selected";
            lbllogvalidate.Visible = false;
            lblTSlogvalidate.Visible = false;
            warningLogsfail.Visible = false;
            warningPiffmissing.Visible = false;
            warningMFMising.Visible = false;
            warningtestsuitelogfail.Visible = false;


            //optional Log parser
            CPCTS_conformant = true;
            CPCTD_conformant = true;
            SCO_runtime_conformant = true;
            SCO_conformant = true;
            Metadatafile_conformant = true;
            SCOList.Clear();
            mainCGIcalls.Clear();
            extraCGIcalls.Clear();
            ADLTestSuiteSummaryfile = "";
            ADLTestSuiteDetailsfile = "";
            ADLTestSuiteLuanchDetailsfile = "";

        }

        /// <summary>
        /// Displays the selected PIF information in the form
        /// and disables lists prior to validation.
        /// </summary>
        private void displaySelectedPIFInfo()
        {
            if (!openFileDialog1.FileName.Contains(".zip"))
            {
                displayErrorMsg("Application Error", Path.GetFileName(openFileDialog1.FileName) + " is not a valid PIF (.zip archive). Please select a PIF.");
                return;
            }

            strPIFFilename = Path.GetFileName(openFileDialog1.FileName);
            strPIFFilenameNoExt = Path.GetFileNameWithoutExtension(openFileDialog1.FileName);

            if (Path.GetDirectoryName(openFileDialog1.FileName).Length > 20)
            {
                lblPIFFilename.Text = Path.GetDirectoryName(openFileDialog1.FileName).Substring(0, 19) + "...\\" + strPIFFilename;
                ToolTip tooltip = new ToolTip();
                tooltip.SetToolTip(lblPIFFilename, openFileDialog1.FileName);
            }
            else
            {
                lblPIFFilename.Text = openFileDialog1.FileName;
            }

            lblStatus.Text = "";
            lblStatus.BackColor = SystemColors.Control;
            lblStatus.BorderStyle = BorderStyle.None;

            linkViewLogs.Visible = false;

            listPIFFilesFound.Enabled = false;
            listManifestFilesFound.Enabled = false;
            listPIFFilesMissing.Enabled = false;
            listManifestFilesMissing.Enabled = false;

            listPIFFilesFound.BackColor = SystemColors.ControlLight;
            listManifestFilesFound.BackColor = SystemColors.ControlLight;
            listPIFFilesMissing.BackColor = SystemColors.ControlLight;
            listManifestFilesMissing.BackColor = SystemColors.ControlLight;


            listPIFFilesFound.Items.Clear();
            listManifestFilesFound.Items.Clear();
            listPIFFilesMissing.Items.Clear();
            listManifestFilesMissing.Items.Clear();

            btnValidate.Enabled = true;
            btnSelectLogDir.Enabled = true;
            btnSelectADLLogDir.Enabled = true;

            resettheForm();
        }

        /// <summary>
        /// Reads the selected PIF (.zip) and creates lists of files from the archive and imsmanifest.xml file in the PIF.
        /// requires that a PIF file was selected in the opendialog function
        /// </summary>
        private bool readPIFFile()
        {
            string zipPath = openFileDialog1.FileName;
            List<string> pifFiles = new List<string>();
            List<string> manifestFiles = new List<string>();
            ZipArchive archive;
            XDocument manifest;
            //XDocument tempXml;
            // XmlSchemaSet schemas = new XmlSchemaSet();
            // XmlSchema tempschema;


            try
            {
                archive = ZipFile.OpenRead(zipPath);
            }
            catch(Exception ex)
            {
                showErrorState("Error: " + strPIFFilename + " is not a valid PIF.");
                displayErrorMsg("Application Error", "The application could not read " + strPIFFilename + " because it is not a valid PIF.", ex);
                return false;
            }

            using (archive)
            {
                if (!archive.Entries.Any(p => p.FullName == "imsmanifest.xml"))
                {
                    showErrorState("Error: There is no imsmanifest.xml file in at the root of " + strPIFFilename + ". Please verify that you selected the correct file.");
                    displayErrorMsg("Validation Error", "The selected PIF does not contain an imsmanifest.xml file at the root level.");
                    return false;
                }

                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    
                    
                    String entryFilename = WebUtility.UrlDecode(entry.FullName);
                    entryFilename = entryFilename.Replace("\\", "/");
                    //String entryFilename = entry.FullName;
                    if (!entryFilename.Substring(entryFilename.Length - 1).Equals("/"))
                        { intPIFFilesFoundCount++; }

                    if (!entryFilename.Substring(entryFilename.Length - 1).Equals("/") && !isSCORMSchemaFile(entryFilename))
                    {
                        pifFiles.Add(entryFilename);
                        listPIFFilesFound.Items.Add(entryFilename);
                    }

                    if (entryFilename == "imsmanifest.xml")
                    { 
                        try
                        {
                            manifest = XDocument.Load(entry.Open());
                        }
                        catch (Exception ex)
                        {
                            showErrorState("Error: The imsmanifest.xml file in " + strPIFFilename + " is not a valid XML document.");
                            displayErrorMsg("Application Error", "The application could not read imsmanifest.xml because it is not a valid XML document.", ex);
                            return false;
                        }

                        XNamespace ns = manifest.Root.Attribute("xmlns").Value;// "http://www.imsglobal.org/xsd/imscp_v1p1";
                        XNamespace mdns = "http://www.adlnet.org/xsd/adlcp_v1p3";
                        XNamespace imsss = "http://www.imsglobal.org/xsd/imsss";

                        // This creates the lists and counts of all the files listed in the Manifest

                        var mdFiles = manifest.Root.Descendants(ns + "metadata").Descendants(mdns + "location");
                        var files = manifest.Root.Descendants(ns + "file");
                        var mm = manifest.Root.Descendants(imsss + "minNormalizedMeasure");
                        
                        if (mm.Count() > 0) ShouldBeQuiz = true; 
                        
                        //manifest.Root.Attribute
                       
                        foreach (var file in files)
                        {
                           String filename = WebUtility.UrlDecode(file.Attribute("href").Value);
                            
                            //check the Attributes of resources and see if there is any additional path
                            var attrbts = file.Parent.Attributes();
                            foreach (XAttribute z in attrbts)
                            {
                                if (z.Name.ToString().Contains("base"))
                                {
                                    filename = z.Value.ToString().Trim() + filename;
                                }
                            }
                               
                            if (!isSCORMSchemaFile(filename))
                            {
                                manifestFiles.Add(filename.Normalize());
                                listManifestFilesFound.Items.Add(filename);
                                intManifestFilesFoundCount++;
                            }
                        }

                        foreach (var mdFile in mdFiles)
                        {
                            String mdFilename = WebUtility.UrlDecode(mdFile.Value);

                            if (!manifestFiles.Contains(mdFilename))
                            {
                                manifestFiles.Add(mdFilename);
                                listManifestFilesFound.Items.Add(mdFilename);
                                intManifestFilesFoundCount++;                                
                            }
                        }

                        // This gets the counts of the metadata files
                        // Doing these in the reverse order than they are in the manifest
                        // to catch record the last section any duplicates (that is how
                        // it works in the meta data editor



                        Assetmetafiles = manifest.Root.Descendants(ns + "resources").Descendants(ns + "resource").Where(el => el.Attribute(mdns + "scormType").Value == "asset").Descendants(mdns + "location");
                        foreach (var Assetfile in Assetmetafiles)
                        {


                            if (!metadataXMLfiles.Contains(Assetfile.Value.ToString()))
                            {
                                numMetadatafiles++;
                                metadataXMLfiles.Add(Assetfile.Value.ToString());
                                ValidateMetadatafile(Assetfile.Value.ToString(), archive, "asset");
                                numAssetmetadatafiles++;
                            }


                        }

                        SCOmetafiles = manifest.Root.Descendants(ns + "resources").Descendants(ns + "resource").Where(el => el.Attribute(mdns + "scormType").Value == "sco").Descendants(mdns + "location");
                        foreach (var SCOfile in SCOmetafiles)
                        {
                            
                            
                            if (!metadataXMLfiles.Contains(SCOfile.Value.ToString()))
                            {
                                numMetadatafiles++;
                                metadataXMLfiles.Add(SCOfile.Value.ToString());
                                ValidateMetadatafile(SCOfile.Value.ToString(), archive, "sco");
                                numSCOmetadatafiles++;
                            }

                        }

                        ContentAgmetafiles = manifest.Root.Descendants(ns + "organizations").Descendants(ns + "metadata").Descendants(mdns + "location");
                        foreach (var ContentAgmetafile in ContentAgmetafiles)
                        {


                            if (!metadataXMLfiles.Contains(ContentAgmetafile.Value.ToString()))
                            {
                                numMetadatafiles++;
                                metadataXMLfiles.Add(ContentAgmetafile.Value.ToString());
                                ValidateMetadatafile(ContentAgmetafile.Value.ToString(), archive, "content aggregation");
                                numContentAgMetadatafiles++;
                            }

                        }

                        // Set styles
                        lblStatus.BorderStyle = BorderStyle.FixedSingle;
                        lblStatus.Text = "Success! Saved logs for " + strPIFFilename + ".";
                        lblStatus.BackColor = Color.FromArgb(200, 255, 200);
                        linkViewLogs.Visible = true;
                    } // end imsmanifest if
                    


                } // end foreach for ZipArchive


                // Validate metadata files
                //moved this to above because multi-sco Pifs were miscounting things
              /*  ValidateMetadatafiles(ContentAgmetafiles, archive, "content aggregation");
                ValidateMetadatafiles(SCOmetafiles, archive, "sco");
                ValidateMetadatafiles(Assetmetafiles, archive, "asset"); 
                */
                
                // Compare PIF and manifest lists to generate the lists of missing files
                var pifFilesMissing = manifestFiles.Where(mFile => !pifFiles.Contains(mFile.Trim())); // these are files listed in the manifest but not found in the PIF
                var manifestFilesMissing = pifFiles.Where(pFile => !manifestFiles.Contains(pFile)); // these are non-schema files found in the PIF but are not listed in the manifest

                //var metadataFilesMissing = metadataXMLfiles.Where(mFile => !pifFiles.Contains(mFile)); // metadata files listed in the manifest but not found in the PIF
                //nummissingmetadatafiles = metadataFilesMissing.Count();
              /*  List<string> pifFilesMissing = new List<string>();

                foreach (string mfl in manifestFiles)
                {
                    if (!pifFiles.Contains(mfl))
                    {
                        pifFilesMissing.Add(mfl);
                    }
                }
                */

                foreach (string file in pifFilesMissing)
                {
                    if (!isSCORMSchemaFile(file.ToString()))
                    {
                        listPIFFilesMissing.Items.Add(file.ToString());
                        intPIFFilesMissingCount++;
                    }
                }

                foreach (string file in manifestFilesMissing)
                {
                    if (!isSCORMSchemaFile(file.ToString()))
                    {
                        listManifestFilesMissing.Items.Add(file.ToString());
                        intManifestFilesMissingCount++;
                    }
                }

                
            } // end using
            return true;
        }

        /// <summary>
        /// Enables disabled GUI elements.
        /// </summary>
        private void turnOnGUI()
        {
            btnValidate.Enabled = false;
            btnSelectLogDir.Enabled = false;
            btnSelectADLLogDir.Enabled = false; 

            grpPIFFilesFound.Text += " (" + intPIFFilesFoundCount + " files) - packaged_files_found.html";
            grpManifestFilesFound.Text += " (" + intManifestFilesFoundCount + " files) - manifest_files_found.html";
            grpPIFFilesMissing.Text += " (" + intPIFFilesMissingCount + " files) - packaged_files_missing.html";
            grpManifestFilesMissing.Text += " (" + intManifestFilesMissingCount + " files) - manifest_files_missing.html";
            if (intManifestFilesMissingCount > 0)
            {
            }

            listPIFFilesFound.Enabled = true;
            listManifestFilesFound.Enabled = true;
            listPIFFilesMissing.Enabled = true;
            listManifestFilesMissing.Enabled = true;

            listPIFFilesFound.BackColor = System.Drawing.Color.White;
            listManifestFilesFound.BackColor = System.Drawing.Color.White;
            if (intPIFFilesMissingCount > 0)
            {
                grpPIFFilesMissing.ForeColor = Color.Crimson;
                warningPiffmissing.Visible = true;
                //   listPIFFilesMissing.BackColor = Color.PaleGoldenrod;
            }
             listPIFFilesMissing.BackColor = System.Drawing.Color.White;
            if (intManifestFilesMissingCount > 0)
            {
                grpManifestFilesMissing.ForeColor = Color.Crimson;
                warningMFMising.Visible = true;
                
             //   listManifestFilesMissing.BackColor = Color.PaleGoldenrod;
            }
            listManifestFilesMissing.BackColor = System.Drawing.Color.White;

            lblPIFFilename.Text = "";

           
        }

        /// <summary>
        /// Generates and saves all 5 log files to the the "logs" folder.
        /// </summary>
        private void createLogs()
        {
            string logHTMLTemplate = "";
            strLogDir = @"logs\RV_" + DateTime.Now.ToString("yyMMdd_hhmmss") + "_" + strPIFFilenameNoExt;

            /*
             * Generate two 'found' file logs: packaged_files_found.hmtml and manifest_files_found.html
             */

            // Generate PIF files found log (packaged_files_found.html)            
            logHTMLTemplate = createHTMLTemplate(listPIFFilesFound, "found", "Packaged Files Found", "Files found in the PIF");
            
            System.IO.Directory.CreateDirectory(strLogDir);
            System.IO.File.WriteAllText(strLogDir + @"\packaged_files_found.html", logHTMLTemplate);

            // Generate manifest files found log (manifest_files_found.html)
            logHTMLTemplate = createHTMLTemplate(listManifestFilesFound, "found", "Manifest Files Found", "Files found as resources in the manifest");
            System.IO.Directory.CreateDirectory(strLogDir);
            System.IO.File.WriteAllText(strLogDir + @"\manifest_files_found.html", logHTMLTemplate);

            /*
             * Generate two 'missing' file logs: packaged_files_missing.hmtml and manifest_files_missing.html
             */

            // Generate PIF files missing HTML log (packaged_files_missing.html)
            logHTMLTemplate = createHTMLTemplate(listManifestFilesMissing, "missing", "Packaged Files Missing", "Files found in the PIF but missing as resources in the manifest");
            System.IO.Directory.CreateDirectory(strLogDir);
            System.IO.File.WriteAllText(strLogDir + @"\packaged_files_missing.html", logHTMLTemplate);

            // Generate manifest files missing log (manifest_files_missing.html)
            logHTMLTemplate = createHTMLTemplate(listPIFFilesMissing, "missing", "Manifest Files Missing", "Files found as resources in the manifest but missing from the PIF");
            System.IO.Directory.CreateDirectory(strLogDir);
            System.IO.File.WriteAllText(strLogDir + @"\manifest_files_missing.html", logHTMLTemplate);

            //Generate Metadata logs - take out for now until can recreate the whole Metadata parse
             logHTMLTemplate = createMetadataDocTemplate("METADATA FILE REPORT");
             System.IO.Directory.CreateDirectory(strLogDir);
            System.IO.File.WriteAllText(strLogDir + @"\ValidateMD.doc", logHTMLTemplate);
            //System.IO.File.WriteAllText(strLogDir + @"\metadata_file_report.doc", logHTMLTemplate);

            //Generate checksum
            try
            {
            string sdf = "";
            sdf = GetMD5HashFromFile(openFileDialog1.FileName);
            System.IO.File.WriteAllText(strLogDir + @"\PIF_file_validate.txt", sdf);

            }
            catch
            { }



        }

        /// <summary>
        /// Creates an HTML log file with file counts using the specified ListBox, type, title, and description.
        /// </summary>
        /// <param name="fileList">The target ListBox from which to read items.</param>
        /// <param name="type">Type of page ("found" or "missing") that determines the background color.</param>
        /// <param name="title">The log title.</param>
        /// <param name="desc">Description text that appears under the title.</param>
        /// <returns>An HTML page string for the log file.</returns>
        private string createHTMLTemplate(ListBox fileList, string type, string title, string desc)
        {
            int htmlFileCount = 1;
            String pathPrefix = @"\\pif:\";
            String logTemplateHead = "<!DOCTYPE html><title>" + title + "</title><style>body{font-family:sans-serif;font-size:16px}body.found{background:#c0ffc0}body.missing{background:#ffc0c0}h1{font-size:2em}h2{font-size:1.5em;border-bottom:1px solid #888;padding-bottom:1em}table{background:#fff;margin-bottom:20px;border:1px solid #777;box-shadow:1px 1px 3px #333}tr.head td{font-weight:700;background:#ccc;border-bottom:2px solid #333}td{border:1px solid #333;padding:10px}tr.bold td{border-top:2px solid #333;font-weight:700}td.count{text-align:center}</style>";
            String logHTMLTemplate = logTemplateHead + "<body class=\"" + type + "\"><h1>" + title + "</h1><h2>" + desc + "</h2><h3>Files and resources for " + strPIFFilename +"</h3><p>Select each figure to open the corresponding log file.</p>";

            logHTMLTemplate += "<table cellpadding=0 cellspacing=0><tr class=head><td>File Count</td><td class=\"count\">#</td></tr>";
            logHTMLTemplate += "<tr><td>Files found in manifest </td><td class=\"count\"><a href=\"manifest_files_found.html\" title=\"Manifest Files Found\">" + intManifestFilesFoundCount + "</a></td></tr>";
            logHTMLTemplate += "<tr><td>File missing from manifest</td><td class=\"count\"><a href=\"manifest_files_missing.html\" title=\"Manifest Files Missing\">" + intManifestFilesMissingCount + "</a></td></tr>";
            logHTMLTemplate += "<tr><td>Files missing from PIF</td><td class=\"count\"><a href=\"packaged_files_missing.html\" title=\"Packaged Files Missing\">(" + intPIFFilesMissingCount + ")</a></td></tr>";
            logHTMLTemplate += "<tr class=bold><td>Total files contained in this package (PIF)</td><td class=\"count\"><a href=\"packaged_files_found.html\" title=\"Packaged Files Found\">" + intPIFFilesFoundCount + "</a></td></tr></table>";
            logHTMLTemplate += "<h3>" + title + " count for " + strPIFFilename + "</h3>";

            if (fileList.Items.Count > 0)
            {
                logHTMLTemplate += "<table class=\"files\" cellpadding=0 cellspacing=0><tr class=\"head\"><td class=\"count\">#</td><td>File name and path</td></tr>";

                foreach (string file in fileList.Items)
                {
                    logHTMLTemplate += "<tr><td class=\"count\">" + htmlFileCount++ + "</td><td>" + pathPrefix + file.Replace("/", @"\") + "</td></tr>";
                }

                logHTMLTemplate += "</table>";
            }
            else
            {
                logHTMLTemplate += "No file entries";
            }

            logHTMLTemplate += "</body></html>";

            return logHTMLTemplate;
        }

        /// <summary>
        /// Creates an Simple Word log file with file counts using the specified ListBox, type, title, and description.
        /// </summary>
        /// <param name="fileList">The target ListBox from which to read items.</param>
        /// <param name="type">Type of page ("found" or "missing") that determines the background color.</param>
        /// <param name="title">The log title.</param>
        /// <param name="desc">Description text that appears under the title.</param>
        /// <returns>An HTML page string for the log file.</returns>
        private string createMetadataDocTemplate(string title)
        {
            string logTemplate = title + ": \\" +  openFileDialog1.FileName + "\\imsmanifest.xml \r\n \r\n ===================== \r\n \r\n";
            logTemplate = logTemplate + "Total number of metadata files: " + numMetadatafiles + " \r\n \r\n \r\n";
            logTemplate = logTemplate + "Number of SCO metadata files: " + numSCOmetadatafiles + " \r\n \r\n";
            logTemplate = logTemplate + "Number of CONTENT AGGREGATION metadata files: " + numContentAgMetadatafiles + "  \r\n \r\n";
            logTemplate = logTemplate + "Number of ASSET metadata files: " + numAssetmetadatafiles + " \r\n \r\n \r\n";
            logTemplate = logTemplate + "------------------------------- \r\n \r\n";
            logTemplate = logTemplate + "Number of INVALID metadata files: " + numinvalidmetadatafiles + " \r\n \r\n";
            logTemplate = logTemplate + "Number of VALID metadata files: " + numvalidmetadatafiles + " \r\n \r\n";
            logTemplate = logTemplate + "Number of MISSING metadata files: " + nummissingmetadatafiles + " \r\n \r\n \r\n";
            logTemplate = logTemplate + "INVALID METADATA FILE DETAILS BELOW: \r\n \r\n";
            logTemplate = logTemplate + "------------------------------------ \r\n \r\n";

            string tempfilename = "";
            int filenumber = 1;
            List<string> errmsgs = new List<string>();

            foreach (string x in metadatafilesErrors)
            {
                string[] xm = x.Split('~');
                if (tempfilename == "")
                {
                    tempfilename = xm[0];
                    logTemplate = logTemplate + "\r\n File #" + filenumber + " = " + tempfilename + "\r\n";
                }
                else
                {
                    if (tempfilename != xm[0])
                    {
                        logTemplate = logTemplate + "\r\n Errors found: " + errmsgs.Count() + "\r\n \r\n ==============\r\n";
                        foreach(string errs in errmsgs)
                        {
                            logTemplate = logTemplate + "\r\n " + errs + "\r\n";
                        }
                        logTemplate = logTemplate + "\r\n -------------- \r\n \r\n";
                        filenumber++;
                        tempfilename = xm[0];
                        logTemplate = logTemplate + "\r\n File #" + filenumber + " = " + tempfilename + "\r\n";
                        errmsgs.Clear();
                    
                    }
                }                
                errmsgs.Add(xm[1]);


            }
            if (errmsgs.Count() > 0)
            {
                logTemplate = logTemplate + "\r\n Errors found: " + errmsgs.Count() + "\r\n \r\n ==============\r\n";
                foreach (string errs in errmsgs)
                {
                    logTemplate = logTemplate + "\r\n " + errs + "\r\n";
                }
                
                if (metadataFilesMissing.Count == 0)
                {
                    logTemplate = logTemplate + "\r\n -------------- \r\n \r\n \r\n";
                }
                else
                {
                    logTemplate = logTemplate + "\r\n \r\n ==============\r\n \r\n";
                }
                    
            }

            //do missing metadata files here
            if (metadataFilesMissing.Count > 0)
            {
                logTemplate = logTemplate + "The following METADATA File";
                if (metadataFilesMissing.Count > 1) logTemplate = logTemplate + "s are";
                logTemplate = logTemplate + " listed in the Mainfest but not found in the PIF: \r\n \r\n";
                foreach (string mdm in metadataFilesMissing)
                {
                    logTemplate = logTemplate + mdm + "\r\n";
                }
                logTemplate = logTemplate + "\r\n -------------- \r\n \r\n \r\n";
            }


            return logTemplate;
        }

        /// <summary>
        /// Creates an HTML log file with file counts using the specified ListBox, type, title, and description.
        /// </summary>
        /// <param name="Validationresults">dictionary object that contains the results of the validation of the log files</param>
        /// <returns>An HTML page string for the log file.</returns>
        private string createLogValidateHTMLTemplate(Dictionary<string,string> Validationresults)
        {
            String bodycssclass = "found";
            String logTemplateTable = "<table cellpadding=0 cellspacing=0><tr class=head><td>Log File</td><td class=\"count\" colspan=\"2\">Results</td></tr>";
            foreach (string lfn in logFilelist)
            {
                logTemplateTable += "<tr><td>" + lfn + " </td><td class=\"count\">";
                if (Validationresults.TryGetValue(lfn, out string themessage))
                {
                    logTemplateTable += themessage + "</td><td class=\"diff\">&#10008</td>";
                    bodycssclass = "missing";
                }
                else
                {
                    if (lfn == "PIF_file_validate.txt")
                    {
                        logTemplateTable += "The PIF file was the same as previously test.</td><td class=\"same\">&#10003;</td>";
                    }
                    else
                    {
                        logTemplateTable += "The log file had the correct content.</td><td class=\"same\">&#10003;</td>";
                    }
                    
                }
                logTemplateTable += "</tr>";
            }
            logTemplateTable += "</table>";

            String logTemplateHead = "<!DOCTYPE html><title>Log Comparison Validation</title><style>body{font-family:sans-serif;font-size:16px}body.found{background:#c0ffc0}body.missing{background:#ffc0c0}h1{font-size:2em}h2{font-size:1.5em;border-bottom:1px solid #888;padding-bottom:1em}table{background:#fff;margin-bottom:20px;border:1px solid #777;box-shadow:1px 1px 3px #333}tr.head td{font-weight:700;background:#ccc;border-bottom:2px solid #333}td{border:1px solid #333;padding:10px}tr.bold td{border-top:2px solid #333;font-weight:700}td.count{text-align:center}td.same{text-align:center;font-size: 26px;color:green;font-weight:700}td.diff{text-align:center;font-size: 26px;color:red;font-weight:700}</style>";
            String logHTMLTemplate = logTemplateHead + "<body class=\"" + bodycssclass + "\"><h1>Log Comparison Validation</h1><h2>Results from comparison of previously made log files</h2><h3>Files and resources for " + strPIFFilename + "</h3><p>Select each figure to open the corresponding log file.</p>";
            logHTMLTemplate += logTemplateTable;
            logHTMLTemplate += "</body></html>";

            return logHTMLTemplate;
        }

        /// <summary>
        /// Creates an HTML log file with file counts using the specified ListBox, type, title, and description.
        /// </summary>
        /// <param name="Validationresults">dictionary object that contains the results of the validation of the log files</param>
        /// <returns>An HTML page string for the log file.</returns>
        private string createLogParserHTMLTemplate(List<string> cPSErrors, List<string> cPSLMeatdataErrors, List<string> cPSLSCOErrors, List<string> CPDLWarnErrors, bool CPDLWell, bool CPDLvalid)
        {
            string adlsummaryfilename = ADLTestSuiteSummaryfile;
            string adldetailsfilename = ADLTestSuiteDetailsfile;           
            String logHTMLTemplate = "";

            //Manifest files missing *****************************************************************************************************************************
            logHTMLTemplate = logHTMLTemplate + "<h3> Summary of Resource Validator Manifest Files Missing : manifest_files_missing.html</h3>";
            if (intPIFFilesMissingCount == 0)
            logHTMLTemplate = logHTMLTemplate + "<h2>This log is conformant</h2><br>";
            else
            {
                //  logHTMLTemplate = logHTMLTemplate + "There are " + intManifestFilesMissingCount.ToString() + " missing.";
                logHTMLTemplate = logHTMLTemplate + "<table border='1'><tr><th>Files Listed in Manifest that are not in the package</th></tr>";
                foreach (string file in listPIFFilesMissing.Items)
                {
                    logHTMLTemplate = logHTMLTemplate + "<tr><td>" + file.Replace(" / ", @"\") + "</td></tr>";
                }
                logHTMLTemplate = logHTMLTemplate + "</table>";
            }

            //Package files missing *****************************************************************************************************************************
            logHTMLTemplate = logHTMLTemplate + "<h3>Summary of Resource Validator Packaged Files Missing : packaged_files_missing.html</h3>";
            if (intManifestFilesMissingCount== 0)
                logHTMLTemplate = logHTMLTemplate + "<h3>There are no extra files</h3>";
            else
            {
                logHTMLTemplate = logHTMLTemplate + "<table border='1'><tr><th>Files present in the package, but not listed on the Manifest</th></tr>";
                foreach (string file in listManifestFilesMissing.Items)
                {
                    logHTMLTemplate = logHTMLTemplate + "<tr><td>" + file.Replace(" / ", @"\") + "</td></tr>";
                }
                logHTMLTemplate = logHTMLTemplate + "</table>";
            }
            logHTMLTemplate = logHTMLTemplate + "<p>";
            logHTMLTemplate = logHTMLTemplate + "<em>Total files contained in this package: " + intPIFFilesFoundCount.ToString() +  "</em>";
            logHTMLTemplate = logHTMLTemplate + "</p><br>";

            //ADL testsuite Summary file *****************************************************************************************************************************
            logHTMLTemplate = logHTMLTemplate + "<h3>Summary of Content Package Conformance Test Summary: " + Path.GetFileName(ADLTestSuiteSummaryfile) + "</h3>";
            logHTMLTemplate = logHTMLTemplate + "<ol>";
            
            if (cPSErrors.Count() > 0)
            {
              
              foreach(var i in cPSErrors)
                {
                    logHTMLTemplate = logHTMLTemplate + "<li><font color=\"red\">Manifest Summary: </font>" + i.ToString() + " </li>";
                }
                CPCTS_conformant = false;
            }
            else
            {
            logHTMLTemplate = logHTMLTemplate + "<li>Manifest Summary: There are no errors in the Manifest.</li>";

            }

            if (cPSLMeatdataErrors.Count() > 0)
            {
                
                foreach (var i in cPSErrors)
                {
                    logHTMLTemplate = logHTMLTemplate + "<li><font color=\"red\">Meta-data Testing: </font>" + i + "</li>";
                }
            }
            else
            {
                logHTMLTemplate = logHTMLTemplate + "<li>Meta-data Testing: There are no errors in the Meta-data.</li>";
            }

            if (SCOList.Count > 0)
            {
                if (!SCO_conformant)
                {
                    logHTMLTemplate = logHTMLTemplate + "<li><font color=\"red\">SCO Testing: </font>One or more SCO's contain errors.</li>";
                }
                else
                {
                    logHTMLTemplate = logHTMLTemplate + "<li>SCO Testing: There are no errors in the SCO's.</li>";
                }
               
            }
                            
            logHTMLTemplate = logHTMLTemplate + "</ol>";

            //Lists errors for SCOs found in the ADL testsuite Summary file
            if (!SCO_conformant)
            {
                foreach (SCOInfo s in SCOList)
                {
                    if (s.getSummaryLogErrorsCount() > 0)
                    {
                        logHTMLTemplate += "<h3>" + s.getSCOname() + "</h3>";
                        logHTMLTemplate += "<p>" + s.getLaunchfile() + "</p>";
                        logHTMLTemplate += "<table border=\"1\"><tbody><tr><th>Error Message</th></tr>";
                        logHTMLTemplate += "<tr><td class=\"error\">";
                        foreach(string ers in s.getSummaryLogErrors())
                        {
                            if (s.getSummaryLogErrors().IndexOf(ers) > 0) logHTMLTemplate += ", ";
                            logHTMLTemplate += ers;
                        }
                        logHTMLTemplate += "</td></tr></tbody></table>";
                    }
                }
            }
            logHTMLTemplate += "<br>";
            
            //ADL testsuite Details file *****************************************************************************************************************************

            logHTMLTemplate = logHTMLTemplate + "<h3>Summary of Content Package Conformance Test Details : " + Path.GetFileName(ADLTestSuiteDetailsfile) + "</h3>";
            logHTMLTemplate = logHTMLTemplate + "<ol>";
            
            if (CPDLWell)
            {
            logHTMLTemplate = logHTMLTemplate + "<li>Manifest Instance: The Manifest Instance is well-formed</li>";

            }
            if (CPDLvalid)
            {
                logHTMLTemplate = logHTMLTemplate + "<li>Manifest Instance: The Manifest Instance is valid</li>";
            }
            if (CPDLvalid && CPDLWell)
            {
                logHTMLTemplate = logHTMLTemplate + "<li>Manifest Instance: The manifest instance is minimum conformant</li>";
            }
            else
            {
                logHTMLTemplate = logHTMLTemplate + "<li><font color='red'>Manifest Instance:</font> The manifest instance is NOT minimum conformant</li>";
            }
            logHTMLTemplate = logHTMLTemplate + "</ol><br>";

            if (CPDLWarnErrors.Count() > 0)
            {
                logHTMLTemplate = logHTMLTemplate + "<table border='1'>";
                logHTMLTemplate = logHTMLTemplate + "<tbody><tr><th>General error and warning messages</th></tr>";
                foreach(string r in CPDLWarnErrors)
                {
                    logHTMLTemplate +=  "<tr><td class=\"error\">" + r + "</td></tr>";

                }
                logHTMLTemplate = logHTMLTemplate + "</tbody></table><br>";

            }

            // Summary /SCO log data *****************************************************************************************************************************

            logHTMLTemplate = logHTMLTemplate + "<h3> Summary of Sharable Content Object (SCO) Run-Time Environment Conformance Test : " + Path.GetFileName(ADLTestSuiteSummaryfile) + "</h3>";
            if (SCO_runtime_conformant) 
                logHTMLTemplate = logHTMLTemplate + "<h2>The Runtime Log is conformant</h2>";
            else
            {
                logHTMLTemplate = logHTMLTemplate + "<table border='1'><tbody>";
                logHTMLTemplate = logHTMLTemplate + "<tr><th class='conform'>SCO Identifier</th><th class='errors'>Error</th></tr>";


                if (ADLTestSuiteLuanchDetailsfile == "")
                {
                    
                    logHTMLTemplate = logHTMLTemplate + "<tr><td class=\"conform\">NO_SCO</td>" +
                        "<td><ul><li class=\"error\">Log Contains No SCOs!<ul><li class=\"normal\">This log file contains no SCOs. Make sure that this is a valid Runtime log</li>" +
                        "<li><a href=\"" + ADLTestSuiteSummaryfile.Substring(ADLTestSuiteSummaryfile.LastIndexOf("\\") + 1) + "\">Detail File</a></li></ul></li></ul>" +
                        "</td></tr>";
                }
                else
                {
                    string templht = "";
                    int errorcount = 0;
                    foreach (var sco in SCOList)
                    {
                        templht = templht + "<tr><td class=\"conform\">" + sco.getSCOname() + "</td><td><ul>";
                    
                        foreach (var c in sco.getmainCGIcalls())
                        {
                            if(c.error != "")
                            {
                                    templht = templht + "<li class=\"error\">" + c.item_name;
                                    templht = templht + "<ul><li class=\"normal\">" + c.error + "</li></ul></li>";
                                    errorcount++;
                            }
                        }
                        foreach (var c in sco.getextraCGIcalls())
                        {
                            if (c.error != "")
                            {
                                templht = templht + "<li class=\"error\">" + c.item_name;
                                templht = templht + "<ul><li class=\"normal\">" + c.error + "</li></ul></li>";
                                errorcount++;
                            }
                        }
                        templht = templht + "</ul></td></tr>";
                    }
                    if (errorcount != 0) logHTMLTemplate += templht;
                    templht = "";
                    
                   
                }
              
                logHTMLTemplate = logHTMLTemplate + "<tr></tr>";
                //end loop thru errors
                logHTMLTemplate = logHTMLTemplate + "</tbody></table>";
            }

            logHTMLTemplate = logHTMLTemplate + "<br>";

            //Metadata editor *****************************************************************************************************************************
            logHTMLTemplate = logHTMLTemplate + "<h3>Summary of Metadata Editor Batch Validation : ValidateMD.doc</h3>";
            logHTMLTemplate = logHTMLTemplate + "<p>Manifest File: imsmanifest.xml</p>";
            logHTMLTemplate = logHTMLTemplate + "<table border='1'>";
            logHTMLTemplate = logHTMLTemplate + "<tr><td>Total Number of Metadata Files:</td><td>" + numMetadatafiles + "</td></tr>";
            logHTMLTemplate = logHTMLTemplate + "<tr><td>Number of SCO Metadata Files:</td><td>" + numSCOmetadatafiles + "</td></tr>";
            logHTMLTemplate = logHTMLTemplate + "<tr><td>Number of Content Aggregation Metadata Files:</td><td>" + numContentAgMetadatafiles + "</td></tr>";
            logHTMLTemplate = logHTMLTemplate + "<tr><td>Number of Asset Metadata Files:</td><td>" + numAssetmetadatafiles + "</td></tr>";
            logHTMLTemplate = logHTMLTemplate + "<tr><td>Number of Invalid Files:</td><td>" + numinvalidmetadatafiles + "</td></tr>";
            logHTMLTemplate = logHTMLTemplate + "<tr><td>Number of Valid Files:</td><td>" + numvalidmetadatafiles + "</td></tr>";
            logHTMLTemplate = logHTMLTemplate + "<tr><td>Number of Missing Metadata Files:</td><td>" + nummissingmetadatafiles + "</td></tr>";
            logHTMLTemplate = logHTMLTemplate + "</table><br>";

            //Back to ADL testsuite Summary/ Launch Detail and Runtime log **********************************************************************************

            if (SCOList.Count() > 0)
            {
                logHTMLTemplate = logHTMLTemplate + "<h3> Details of Sharable Content Object (SCO) Run-Time Environment Conformance Test  : " + ADLTestSuiteSummaryfile.Substring(ADLTestSuiteSummaryfile.LastIndexOf("\\") + 1) + "</h3><br>";

                foreach(var sco in SCOList)
                {
                    /*
                    XElement sld;
                    sld = XElement.Load(sco.getLaunchfile());
                    String thefile = "";
                    var lfile = from te in sld.Elements("message") where te.Value.Contains("Attempting to Launch SCO:") == true select te;
                    foreach(var t in lfile)
                    {
                        thefile = t.Value;
                    }     */
                String thefile = sco.getLaunchfile();
                    logHTMLTemplate = logHTMLTemplate + "<h3>" + sco.getSCOname() + "</h3>";
                    //logHTMLTemplate = logHTMLTemplate + "<p>Launch File: " + thefile.Replace("Attempting to Launch SCO:","") + " </p>";
                    logHTMLTemplate = logHTMLTemplate + "<p>Launch File: " + thefile.Replace("Attempting to Launch SCO:", "") + " </p>";
                    logHTMLTemplate = logHTMLTemplate + "<table border='1'>";
                    logHTMLTemplate = logHTMLTemplate + "<tr><th class=conform>Conformant</th><th class=apicalls>API Call</th><th class=details>Detail</th><th class=errors>Errors</th><th class=warnings>Warnings</th></tr>";

            

                    foreach (var t in sco.getmainCGIcalls())
                    {
                        logHTMLTemplate = logHTMLTemplate + "<tr>";
                        logHTMLTemplate = logHTMLTemplate + "<td class=confromance>";
                        if (t.twritten_Count > 0) { logHTMLTemplate = logHTMLTemplate + "yes"; }
                        else { logHTMLTemplate = logHTMLTemplate + "<font color=red>NO</font>"; }//CPCTS_conformant = false;
                        logHTMLTemplate = logHTMLTemplate + "</td><td class=apicall>" + t.item_name + "</td>";
                        logHTMLTemplate = logHTMLTemplate + "<td class=details>";
                        logHTMLTemplate = logHTMLTemplate + "<ul>";
                        logHTMLTemplate = logHTMLTemplate + "<li class=normal>Times invoked: " + t.twritten_Count + "</li>";
                        logHTMLTemplate = logHTMLTemplate + "</ul>";
                        logHTMLTemplate = logHTMLTemplate + "</td>";
                        logHTMLTemplate = logHTMLTemplate + "<td><br>";
                        if (t.error != "") logHTMLTemplate = logHTMLTemplate + t.error;
                        logHTMLTemplate = logHTMLTemplate + "</td>";
                        logHTMLTemplate = logHTMLTemplate + "<td class=warnings><br></td>";
                        logHTMLTemplate = logHTMLTemplate + "</tr>";
                    }

                    foreach(var et in sco.getextraCGIcalls())
                    {
                        logHTMLTemplate = logHTMLTemplate + "<tr>";
                        if (et.error != "") logHTMLTemplate = logHTMLTemplate + "<td class=conform><font color=red>NO</font></td>";
                        else logHTMLTemplate = logHTMLTemplate + "<td class=conform>yes</td>";
                        logHTMLTemplate = logHTMLTemplate + "<td class=apicall>" + et.item_name + "</td><td class=details>";
                        logHTMLTemplate = logHTMLTemplate + "<ul>";
                        logHTMLTemplate = logHTMLTemplate + "<li class=normal>Times Written: " + et.twritten_Count + "</li>";
                        logHTMLTemplate = logHTMLTemplate + "<ul>";
                        logHTMLTemplate = logHTMLTemplate + "<li>Last Value Written: " + et.twritten_value + "</li>";
                        logHTMLTemplate = logHTMLTemplate + "</ul>";
                        logHTMLTemplate = logHTMLTemplate + "<li>Times Retrieved: " + et.trecieved_Count + "</li>";
                        logHTMLTemplate = logHTMLTemplate + "<ul>";
                        logHTMLTemplate = logHTMLTemplate + "<li>Last Value Retrieved: " + et.trecieved_value + "</li>";
                        logHTMLTemplate = logHTMLTemplate + "</ul>";
                        logHTMLTemplate = logHTMLTemplate + "</ul>";
                        logHTMLTemplate = logHTMLTemplate + "</td>";
                        logHTMLTemplate = logHTMLTemplate + "<td><br>";
                        if (et.error != "") logHTMLTemplate = logHTMLTemplate + et.error;
                        logHTMLTemplate = logHTMLTemplate + "</td>";
                        logHTMLTemplate = logHTMLTemplate + "<td class=warnings><ul><br></ul></td>";
                        logHTMLTemplate = logHTMLTemplate + "</tr>";
                    }
           

                    logHTMLTemplate = logHTMLTemplate + "</table>";
                }

            }

            //ParsedOutput Summary header
            String logTemplateHead = "<html xmlns=http://www.w3.org/TR/REC-html40><head><META http-equiv=Content-Type content=text/html; charset=UTF-8><title>Army Log Parser Output</title><style type=text/css>th{background-color=rgb(220,220,220);}table{width=92%;border=1;}td.conform{width=5%;}td.apicall{width=15%;}td.details{width=30%;}td.errors{width=30%;}th.conform{width=5%;}th.apicall{width=15%;}th.details{width=30%;}th.errors{width=30%;}th.warnings(width=20%;}li.error{color=red}li.normal{color=black}td.error{color=red}</style></head>";
            String logHTMLsummaryTemplate = logTemplateHead + "<body><h1>Parsing Logs:</h1>";
            logHTMLsummaryTemplate = logHTMLsummaryTemplate + "<ul>";
            logHTMLsummaryTemplate = logHTMLsummaryTemplate + "<li>Resource Validator Manifest Files Missing [ <a href=>manifest_files_missing.html</a>] : ";
            if (intPIFFilesMissingCount == 0) logHTMLsummaryTemplate = logHTMLsummaryTemplate + "<font color=green>CONFORMANT</font></li>";
            else logHTMLsummaryTemplate = logHTMLsummaryTemplate + "<font color=red>NON-CONFORMANT</font></li>";
            logHTMLsummaryTemplate = logHTMLsummaryTemplate + "<li>Resource Validator Packaged Files Missing [ <a href=>packaged_files_missing.html</a>] : ";
            if (intManifestFilesMissingCount == 0) logHTMLsummaryTemplate = logHTMLsummaryTemplate + "<font color=green>CONFORMANT</font></li>";
            else logHTMLsummaryTemplate = logHTMLsummaryTemplate + "<font color=red>NON-CONFORMANT</font></li>";
            logHTMLsummaryTemplate = logHTMLsummaryTemplate + "<li>Content Package Conformance Test Summary [ <a href='file://" + ADLTestSuiteSummaryfile + "'>" + ADLTestSuiteSummaryfile.Substring(ADLTestSuiteSummaryfile.LastIndexOf("\\") + 1) + "</a>] : ";
            if (CPCTS_conformant) logHTMLsummaryTemplate = logHTMLsummaryTemplate + "<font color=green>CONFORMANT</font></li>";
            else logHTMLsummaryTemplate = logHTMLsummaryTemplate + "<font color=red>NON-CONFORMANT</font></li>";
            logHTMLsummaryTemplate = logHTMLsummaryTemplate + "<li>Content Package Conformance Test Details [ <a href='file://" + ADLTestSuiteDetailsfile + "'>" + ADLTestSuiteDetailsfile.Substring(ADLTestSuiteDetailsfile.LastIndexOf("\\") + 1) + "</a>] : ";
            if (CPCTD_conformant) logHTMLsummaryTemplate = logHTMLsummaryTemplate + "<font color=green>CONFORMANT</font></li>";
            else logHTMLsummaryTemplate = logHTMLsummaryTemplate + "<font color=red>NON-CONFORMANT</font></li>";
            logHTMLsummaryTemplate = logHTMLsummaryTemplate + "<li>Sharable Content Object (SCO) Run-Time Environment Conformance Test [ <a href='file://" + ADLTestSuiteSummaryfile + "'>" + ADLTestSuiteSummaryfile.Substring(ADLTestSuiteSummaryfile.LastIndexOf("\\")+1) + "</a>] : ";
            if (SCO_runtime_conformant) logHTMLsummaryTemplate = logHTMLsummaryTemplate + "<font color=green>CONFORMANT</font></li>";
            else logHTMLsummaryTemplate = logHTMLsummaryTemplate + "<font color=red>NON-CONFORMANT</font></li>";
            logHTMLsummaryTemplate = logHTMLsummaryTemplate + "<li>Metadata Editor Batch Validation [ <a href=''>ValidateMD.doc</a>] : ";
            if (Metadatafile_conformant) logHTMLsummaryTemplate = logHTMLsummaryTemplate + "<font color=green>CONFORMANT</font></li>";
            else logHTMLsummaryTemplate = logHTMLsummaryTemplate + "<font color=red>NON-CONFORMANT</font></li>";
            logHTMLsummaryTemplate = logHTMLsummaryTemplate + "</ul>";

            logHTMLTemplate = logHTMLsummaryTemplate + logHTMLTemplate;


            logHTMLTemplate += "</body></html>";

            //return the html
            return logHTMLTemplate;
        }

        /// <summary>
        /// Displays a specified error message in the GUI and disables the Validate button.
        /// </summary>
        /// <param name="statusMsg">The error message to display.</param>
        private void showErrorState(String statusMsg)
        {
            lblStatus.Text = statusMsg;
            lblStatus.BackColor = Color.FromArgb(255, 200, 200);
            lblStatus.BorderStyle = BorderStyle.FixedSingle;
            lblPIFFilename.Text = "No PIF selected.";
            btnValidate.Enabled = false;
        }

        /// <summary>
        /// Displays an error message box with the specified title and text.
        /// </summary>
        /// <param name="title">The title of the error message box.</param>
        /// <param name="desc">The text for the error message box.</param>
        private void displayErrorMsg(String title, String desc)
        {
            System.Windows.Forms.MessageBox.Show(desc, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Displays an error message with the specified title, text, and optional exception's information.
        /// </summary>
        /// <param name="title">The title of the error message box.</param>
        /// <param name="desc">The body text for the error message box.</param>
        /// <param name="ex">The exception object.</param>
        private void displayErrorMsg(String title, String desc, Exception ex)
        {
            String exMsg = "";

            if (debugMode)
            {
                exMsg = "\n\nDEBUG INFO\n**********\n" + ex.Message + "\n" + ex.StackTrace;
            }

            System.Windows.Forms.MessageBox.Show(desc + exMsg, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Checks to see if a specified filename is on the SCORM schema whitelist.
        /// </summary>
        /// <param name="filename">The file name to check.</param>
        /// <returns></returns>
        public bool isSCORMSchemaFile(String filename)
        {
            foreach (string item in fileWhitelist)
            {
                if (filename.Contains(item))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Displays a MessageBox containing info about the application.
        /// </summary>
        private void displayAboutBox()
        {
            System.Windows.Forms.MessageBox.Show("SCORM Resource Validator v2.2\n© 2018 JANUS Research Group", "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Displays a MessageBox containing help info.
        /// </summary>
        private void displayHelpBox()
        {
            System.Windows.Forms.MessageBox.Show("How to use the Resource Validator:\n1. Browse and select a SCORM 2004 PIF. \n2. (Optional, skip this step if not comparing log files.) Click the \"Browse\" button at the bottom and select the directory where previously created logs are to compare to the new logs that will be created. \n3. Click the \"Validate\" button.  \n4. Retrieve the log files from the 'logs' folder or click \"View Log Files\" to open folder that contain the logs.", "Help", MessageBoxButtons.OK, MessageBoxIcon.Question);
        }

        private void logFolderBrowserDialog_HelpRequest(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Click function for About info
        /// </summary>
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            displayAboutBox();
        }

        /// <summary>
        /// Click function for HOW to menu item
        /// </summary>
        private void howToUseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            displayHelpBox();
        }

        /// <summary>
        /// Click Function that opens log folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void linkViewLogs_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(strLogDir);
        }
        
        /// <summary>
        /// Compares two files Byte by Byte.
        /// </summary>
        /// <param name="file1">The path and file name of one file to compare</param>
        /// <param name="file2">The path and file name of the second file to be compared to</param>
        private bool FileComparebyByte(string file1, string file2)
        {
            int file1byte;
            int file2byte;
            FileStream fs1;
            FileStream fs2;

            //determine if the same file was refereenced, may remove
            if (file1 == file2) return true;

            //Open the two files
            fs1 = new FileStream(file1, FileMode.Open, FileAccess.Read);
            fs2 = new FileStream(file2, FileMode.Open, FileAccess.Read);

            //Check the sizes
            if (fs1.Length != fs2.Length)
            {
                //close files
                fs1.Close();
                fs2.Close();

                return false;
            }

            do
            {
                file1byte = fs1.ReadByte();
                file2byte = fs2.ReadByte();
            }
            while ((file1byte == fs1.ReadByte() && (file1byte != -1)));

            //close files
            fs1.Close();
            fs2.Close();

            return ((file1byte - file2byte == 0));

        }

        /// <summary>
        /// Compares the content of two files as a String. This stripes out all the "White space"
        /// of the text and simple compares the contant as a single line of text.
        /// </summary>
        /// <param name="filepath1">The path and file name of one file to compare</param>
        /// <param name="filepath2">The path and file name of the second file to be compared to</param>
        private bool FileComparebyString(string filepath1, string filepath2)
        {
            string text1 = "";
            string text2 = "";

            try { 
                using (StreamReader streamReader = new StreamReader(filepath1))
                {
                     text1 = streamReader.ReadToEnd();
                }
            }
            catch { text1 = "not found"; }
            try
            {
                using (StreamReader streamReader = new StreamReader(filepath2))
                {
                    text2 = streamReader.ReadToEnd();
                }
            }
            catch { text2 = "not found"; }


            if (RemoveWhitespace(text1) == RemoveWhitespace(text2))
            {
                return true;
            }

            return false;

        }

        /// <summary>
        /// Removes all the "White space" from a string
        /// White space includes spaces, carriage returns and tab, etc...
        /// </summary>
        /// <param name="input">a string of text to have white space removed</param>
        public string RemoveWhitespace(string input)
        {
            return new string(input.ToCharArray().Where(c => !Char.IsWhiteSpace(c)).ToArray());
        }

        /// <summary>
        /// Compares the contents of the log files that are created by this 
        /// application to a set of log files found in the directory 
        /// selected by the user. It will only try to do a compare and 
        /// create a log file if the directory is specified, otherwise it 
        /// does nothing.
        /// </summary>
        private bool compareLogs()
        {
            string newlogfilesDir = strLogDir;
            string oldlogfilesDir = logFolderBrowserDialog.SelectedPath;
            string newlogfile, oldlogfile;
            Dictionary<string, string> badlogFilelist = new Dictionary<string, string>();

            // if they selected a log folder then compare log files
            if (oldlogfilesDir != "")
            {

                foreach (string logfilename in logFilelist)
                {
                    oldlogfile = logFolderBrowserDialog.SelectedPath + "\\" + logfilename;
                    if (File.Exists(oldlogfile))
                    {
                        newlogfile = newlogfilesDir + "\\" + logfilename;
                        if (!FileComparebyString(oldlogfile, newlogfile))
                        {
                            if (logfilename == "PIF_file_validate.txt")
                                badlogFilelist.Add("PIF_file_validate.txt", "Zip file not the same as previously tested.");
                            else
                                badlogFilelist.Add(logfilename, " Log file did not match current validation.");
                        }
                    }
                    else
                    {
                        badlogFilelist.Add(logfilename, " not found in folder.");
                    }
                    
                }

               
                //do logging
                string logHTMLTemplate = "";
                logHTMLTemplate = createLogValidateHTMLTemplate(badlogFilelist);
                System.IO.Directory.CreateDirectory(strLogDir);
                System.IO.File.WriteAllText(strLogDir + @"\log_compare_and_validation.html", logHTMLTemplate);

                if (badlogFilelist.Count > 0 )
                {
                    lbllogvalidate.ForeColor = Color.Crimson;
                    lbllogvalidate.Text = "Log files missing or did not match, Please view the logs above for details.";
                    lbllogvalidate.Visible = true;
                    warningLogsfail.Visible = true;
                    return false;
                }
                else
                {
                    lbllogvalidate.ForeColor = Color.OliveDrab;
                    lbllogvalidate.Text = "The log files selected were validated and correct.";
                    lbllogvalidate.Visible = true;
                    return true;

                }

            }
            
            return false;
        }

        /// <summary>
        /// Parses out the ADL test suite logs and creates a combined
        /// log file with information from all the logs created by this program
        /// </summary>
        /// 
        /// 
        private bool parseLogs()
        {
            string theADLLogdir = ADLlogFolderBrowserDialog.SelectedPath;
            XElement sl;
            List<string> CPSLManifestErrors = new List<string>();
            List<string> CPSLMetadataErrors = new List<string>();
            List<string> CPSLSCOtesterrors = new List<string>();

            if (theADLLogdir != "")
            {
                string[] ADLlogfiles = Directory.GetFiles(ADLlogFolderBrowserDialog.SelectedPath);
                List<string> ADLlogfileslist = ADLlogfiles.ToList();
                string thefile, summarylogfile = "", detailedlogfile = "";

                //First find the two log files we need:
                // yyyy-mm-dd_hh.mm.ssss_packagename_SummaryLog.xml
                // yyyy-mm-dd_hh.mm.ssss_packagename_DetailedLog.xml
                summarylogfile = ADLlogfileslist.Find(x => x.Contains("_SummaryLog.xml"));
                if (summarylogfile != null)
                {
                    thefile = summarylogfile.Substring(summarylogfile.LastIndexOf('\\') + 1).Substring(23).Replace("SummaryLog", "DetailedLog");
                    detailedlogfile = ADLlogfileslist.Find(x => x.Contains(thefile));
                }
                

                ADLTestSuiteSummaryfile = summarylogfile;
                ADLTestSuiteDetailsfile = detailedlogfile;

                if (ADLTestSuiteSummaryfile != "" && ADLTestSuiteDetailsfile != "")
                {

                    //ADL Summary Log parse
                    // summarylog manifest log

                    //pull in Summary log
                    sl = XElement.Load(ADLTestSuiteSummaryfile);
                    
                    var cList =
                    from te in sl.Elements("message")
                    where (string)te.Value == "Controlling Document(s) Required For XML Parsing Found at Root of the Content Package" || (te.Value.Contains("The IMS Manifest") == true && (string)te.Attribute("type").Value == "pass")
                    select te;

                    if (cList.Count() < 5)
                    {
                        var eList =
                             from te in sl.Elements("message")
                             where (te.Value.Contains("The IMS Manifest") == true && te.Attribute("type").Value == "fail")
                             select te;
                        foreach (string stre in eList)
                        {
                            CPSLManifestErrors.Add(stre.ToString());
                        }
                    }
                    // summary log metadata
                    var mList =
                    from te in sl.Elements("message")
                    where (te.Value.Contains("The Metadata XML") == true && (string)te.Attribute("type").Value == "pass")
                    select te;
                    if (mList.Count() < 2)
                    {
                        var m2List =
                             from te in sl.Elements("message")
                             where (te.Value.Contains("The Metadata XML") == true && te.Attribute("type").Value == "fail")
                             select te;
                        foreach (string stre in m2List)
                        {
                            CPSLManifestErrors.Add(stre.ToString());
                        }
                    }
                   


                    //ADL Detail Log parse 
                    List<string> CPDLWarnErrors = new List<string>();
                    bool CPDLWellness = false;
                    bool CDPLValid = false;
                    XElement sld;

                    //pull in Details log
                    sld = XElement.Load(detailedlogfile);
                    
                    var dmList =
                    from te in sld.Elements("message")
                    where (string)te.Value == "Validating the XML for Wellformedness"
                    select te;
                    if (dmList.Count() == 1)
                    {
                        CPDLWellness = true;
                    }
                    var dmvList =
                   from te in sld.Elements("message")
                   where (string)te.Value == "Validating the XML against the Controlling Documents"
                   select te;
                    if (dmvList.Count() == 1)
                    {
                        CDPLValid = true;
                    }

                    var dwmList =
                    from te in sld.Elements("message")
                    where te.Attribute("type").Value == "warn" || te.Attribute("type").Value == "fail"
                    select te;
                    if (dwmList.Count() > 0)
                    {
                        foreach (var h in dwmList)
                        {
                            CPDLWarnErrors.Add(h.Value.ToString());

                        }
                    }


                    //ADL Luanch Detail Log parse 
                    //This has the counts of SCOs, the interactions, and the Runtime info

                   // var lfile = from te in sl.Elements("message") where te.Value.Contains("Attempting to Launch SCO:") == true select te;
                    var lfile = from te in sld.Elements() where te.Name == "link" && (string)te.Attribute("type").Value == "SCO" select te;
                    if (lfile.Count() > 0)
                    {
                       // string launchdetailedlogfile = "";
                        int SCOcount = 1;
                        SCOInfo tempSCOinfo;
                        foreach (var lf in lfile)
                        {
                            /*   launchdetailedlogfile = lf.ToString().Replace("Attempting to Launch SCO:", "");
                               launchdetailedlogfile = launchdetailedlogfile.Substring(launchdetailedlogfile.LastIndexOf("\\") + 1);
                               launchdetailedlogfile = launchdetailedlogfile.Substring(0, launchdetailedlogfile.LastIndexOf("."));
                               ADLTestSuiteLuanchDetailsfile = ADLlogfileslist.Find(x => x.Contains(launchdetailedlogfile));
                               */
                            ADLTestSuiteLuanchDetailsfile = ADLlogfileslist.Find(x => x.Contains(lf.Value));

                            //create SCO object
                            tempSCOinfo = new SCOInfo("SCO_"+ SCOcount.ToString(), ADLTestSuiteLuanchDetailsfile);

                            // mainCGIcalls Initialize Terminate Commit GetLastError
                            sld = XElement.Load(ADLTestSuiteLuanchDetailsfile);
                            List<string> cgicallsMain = new List<string> { "Initialize", "Terminate", "Commit" };
                            if (ShouldBeQuiz)
                            {
                                cgicallsMain.Remove("Commit");
                                cgicallsMain.Add("GetLastError");
                                cgicallsMain.Add("Commit");

                            }
                            String tempstr = "";
                            LogValues tempLV;
                            foreach (string c in cgicallsMain)
                            {
                                tempLV = new LogValues();
                                if (c == "Commit" || c == "GetLastError") tempstr = "The " + c + "() method call finished successfully";
                                else tempstr = "The " + c + "() method finished successfully";
                                var tinvoked = from te in sld.Elements("message") where (string)te.Value == tempstr select te;
                                tempLV.item_name = c;
                                tempLV.twritten_Count = tinvoked.Count();
                                if (tinvoked.Count() < 1)
                                {
                                    SCO_runtime_conformant = false;
                                    tempLV.error = "Army Mandatory error: " + c + " is not invoked minimum number of times (1)";
                                }
                                else tempLV.error = "";
                                // mainCGIcalls.Add(tempLV);
                                tempSCOinfo.AddtomainCGIList(tempLV);
                            }

                            //extraCGIcalls
                            //cmi.interactions.n.result cmi.interactions.n.type cmi.interactions.n.learner_response cmi.interactions.n.correct_responses.n.pattern cmi.session_time cmi.location cmi.completion_status cmi.success_status cmi.exit

                            List<string> cgicallstemp = new List<string> { "cmi.exit", "cmi.success_status", "cmi.completion_status", "cmi.location", "cmi.session_time" };
                            if (ShouldBeQuiz) { cgicallstemp.Remove("cmi.location"); cgicallstemp.Remove("cmi.session_time"); cgicallstemp.Add("cmi.score.scaled"); cgicallstemp.Add("cmi.session_time"); }
                            foreach (string c in cgicallstemp)
                            {
                                tempLV = GetAPICallLogValues(c, 0, 1);
                                //extraCGIcalls.Add(tempLV);
                                tempSCOinfo.AddtoextraCGIList(tempLV);
                                if (tempLV.error != "") { SCO_runtime_conformant = false; }
                            }

                            if (ShouldBeQuiz)
                            {
                                cgicallstemp = new List<string> { "cmi.interactions.n.correct_responses.n.pattern", "cmi.interactions.n.learner_response", "cmi.interactions.n.type", "cmi.interactions.n.result" };
                                foreach (string c in cgicallstemp)
                                {
                                    tempLV = GetAPICallLogValues(c, "cmi.interactions", c.Substring(c.LastIndexOf('.') + 1), 0, 1);
                                    //extraCGIcalls.Add(tempLV);
                                    tempSCOinfo.AddtoextraCGIList(tempLV);
                                    if (tempLV.error != "") { SCO_runtime_conformant = false; }
                                }

                            }
                            SCOList.Add(tempSCOinfo);
                            SCOcount++;
                        }//end for each loop

                        //Now get SCO data (errors) from Summary log
                        var fl = from te in sl.Elements() where te.Name == "message" || te.Name == "link" select te;
                        List<string> temperrors = new List<string>();

                        foreach (XElement x in fl)
                        {
                            if (x.Name == "message" && x.Attribute("type").Value == "fail")
                            {
                                temperrors.Add(x.Value);
                            }
                            if (x.Name == "link" && x.Attribute("type").Value == "SCO")
                            {
                                //Add to scolist if errors
                                if (temperrors.Count() > 0)
                                {
                                    foreach(var s in SCOList)
                                    {
                                        if (s.match(x.Value))
                                        {
                                            s.setSummaryLogErrors(temperrors);
                                            SCO_conformant = false;
                                            temperrors.Clear();
                                            break;
                                        }
                                    }
                                }
                            }
                            if (x.Name == "link" && x.Attribute("type").Value != "SCO")
                            {
                                temperrors.Clear();
                            }
                        }
                        temperrors.Clear();
                    }
                    else { SCO_runtime_conformant = false; }

                    //do logging
                    // Generate logs parsed log (ParsedOutput.html)
                    
                    if (lfile.Count() < 1)
                    {
                        lblTSlogvalidate.ForeColor = Color.Crimson;
                        lblTSlogvalidate.Text = "ADL Test Suite Log files missing or inaccessable, Please view the logs above for details. ";
                        lblTSlogvalidate.Visible = true;
                        warningtestsuitelogfail.Visible = true;
                        displayErrorMsg("Log files not found", "INCORRECT FOLDER OR FILES: The ADL Testsuite log folder is missing critical files. It is liekly that the wrong folder was selected for parsing.");

                    }
                    else
                    {
                        lblTSlogvalidate.ForeColor = Color.OliveDrab;
                        lblTSlogvalidate.Text = "The log files selected were parsed and correct.";
                        lblTSlogvalidate.Visible = true;

                    }
                    string logHTMLTemplate = createLogParserHTMLTemplate(CPSLManifestErrors, CPSLMetadataErrors, CPSLSCOtesterrors, CPDLWarnErrors, CPDLWellness, CDPLValid);
                    System.IO.Directory.CreateDirectory(strLogDir);
                    System.IO.File.WriteAllText(strLogDir + @"\ParsedOutput.html", logHTMLTemplate);

                } //end check file if
                else
                {
                    lblTSlogvalidate.ForeColor = Color.Crimson;
                    lblTSlogvalidate.Text = "ADL Test Suite Log files missing or inaccessable, no log file was created.";
                    lblTSlogvalidate.Visible = true;
                    warningtestsuitelogfail.Visible = true;
                    displayErrorMsg("Log files not found", "The ADL Testsuite Log file(s) were not found or inaccessable in the specified dierctory folder.");
                    return false;
                }
            }
            
            return true;
        }

        /// <summary>
        /// Gets values for specified entries from the Luanch Details file
        /// </summary>
        /// <param name="apicall">Which API Call that needs it's data from the log file</param>
        /// <param name="minget">The minimum times it should be retrieved from the LMS</param>
        /// <param name="minset">The minimum times it should be set in the LMS</param>
        private LogValues GetAPICallLogValues(string apicall, int minget, int minset)
        {
            LogValues l = new LogValues();
            XElement sld;
            
            int sindex = 0;
            int eindex = 0;
            l.item_name = apicall;
            l.twritten_Count = 0;
            l.trecieved_Count = 0;
            l.twritten_value = "";
            l.trecieved_value = "";
            l.error = "";

            sld = XElement.Load(ADLTestSuiteLuanchDetailsfile);

            foreach (var et in sld.Elements("message").Where(x => x.Value.Contains("SetValue") == true && x.Value.Contains(apicall) == true))
            {
                l.twritten_Count++;
                l.twritten_value = et.Value.ToString();
            }

            sindex = l.twritten_value.IndexOf(',') + 2;
            eindex = l.twritten_value.LastIndexOf('"');
            int length = eindex - sindex;
            if (length > 0) l.twritten_value = l.twritten_value.Substring(sindex, length);
            if (minset > 0 && l.twritten_Count < minset) { l.error = "Army Mandatory error: " + l.item_name + " is not set the minimum number of times (" + minset.ToString() + ")"; }

            foreach (var et in sld.Elements("message").Where(x => x.Value.Contains("GetValue") == true && x.Value.Contains(apicall) == true))
            {
                l.trecieved_Count++;
                l.trecieved_value = et.NextNode.ToString();
            }
            l.trecieved_value = l.trecieved_value.Replace("The value returned from the GetValue() method call: [", "");
            l.trecieved_value = l.trecieved_value.Replace("]", "");
            if (minget > 0 && l.twritten_Count < minget) { l.error = "Army Mandatory error: " + l.item_name + " is not retrieved the minimum number of times (" + minget.ToString() + ")"; }

            return l;
        }

        /// <summary>
        /// Gets values for specified entries from the Luanch Details file
        /// </summary>
        /// <param name="apiname">Display name of the API call</param>
        /// <param name="apicall">Which API Call that needs it's data from the log file</param>
        /// <param name="interactiontype">Which type of cmi.interactions is being called</param>
        /// <param name="minget">The minimum times it should be retrieved from the LMS</param>
        /// <param name="minset">The minimum times it should be set in the LMS</param>
        private LogValues GetAPICallLogValues(string apiname,string apicall, string interactiontype, int minget, int minset)
        {
            LogValues l = new LogValues();
            XElement sld;

            int sindex = 0;
            int eindex = 0;
            l.item_name = apiname;
            l.twritten_Count = 0;
            l.trecieved_Count = 0;
            l.twritten_value = "";
            l.trecieved_value = "";
            l.error = "";

            sld = XElement.Load(ADLTestSuiteLuanchDetailsfile);

            foreach (var et in sld.Elements("message").Where(x => x.Value.Contains("SetValue") == true && (x.Value.Contains(apicall) == true && x.Value.Contains("." + interactiontype) == true)))
            {
                l.twritten_Count++;
                l.twritten_value = et.Value.ToString();
            }

            sindex = l.twritten_value.IndexOf(',') + 2;
            eindex = l.twritten_value.LastIndexOf('"');
            int length = eindex - sindex;
            l.twritten_value = l.twritten_value.Substring(sindex, length);
            if(minset > 0 && l.twritten_Count < minset) { l.error = "Army Mandatory error: " + l.item_name + " is not set the minimum number of times (" + minset.ToString() + ")"; }

            foreach (var et in sld.Elements("message").Where(x => x.Value.Contains("GetValue") == true && (x.Value.Contains(apicall) == true && x.Value.Contains(interactiontype) == true)))
            {
                l.trecieved_Count++;
                l.trecieved_value = et.NextNode.ToString();
            }
            l.trecieved_value = l.trecieved_value.Replace("The value returned from the GetValue() method call: [", "");
            l.trecieved_value = l.trecieved_value.Replace("]", "");
            if (minget > 0 && l.twritten_Count < minget) { l.error = "Army Mandatory error: " + l.item_name + " is not retrieved the minimum number of times (" + minget.ToString() + ")"; }

            return l;

        }

        /// <summary>
        /// Creates a checksum for the file specified using MD5
        /// </summary>
        /// <param name="input">path and file name of the file to create a hash for</param>
        protected string GetMD5HashFromFile(string filename)
    {
            try
            {
                using (var md5 = MD5.Create())
                {
                    using (var stream = File.OpenRead(filename))
                    {
                        return Encoding.Default.GetString(md5.ComputeHash(stream));
                    }
                }
            }
            catch { return "Error: unable to create Hash"; }
        }

        /// <summary>
        /// Does validation and sets the counts for the metadata files
        /// </summary>
        /// <param name="input">path and file name of the file to create a hash for</param>
        /// 
        private void ValidateMetadatafiles(IEnumerable<XElement> metadatafiles, ZipArchive ziparchive, string themetadatatype)
        {
            XmlDocument tempmd = new XmlDocument();
            ZipArchiveEntry tempzipentry;

            foreach (var c in metadatafiles)
            {
                string thename = c.Value.ToString();
                tempzipentry = ziparchive.GetEntry(thename);
                if (tempzipentry is null)
                {
                    nummissingmetadatafiles++;
                    metadataFilesMissing.Add(thename);
                } 
                else
                {
                    tempmd.Load(ziparchive.GetEntry(thename).Open());
                    MetadataFile themetadatafile = new MetadataFile(tempmd, thename, themetadatatype);
                    if (!themetadatafile.isValid())
                    {
                        metadatafilesErrors.AddRange(themetadatafile.getErrors());
                        numinvalidmetadatafiles++;
                    }
                    else
                    {
                        numvalidmetadatafiles++;
                    }
                }
                
            }
        }

        private void ValidateMetadatafile(string metadatafilename, ZipArchive ziparchive, string themetadatatype)
        {
            XmlDocument tempmd = new XmlDocument();
            ZipArchiveEntry tempzipentry;

            
                string thename = metadatafilename;
                tempzipentry = ziparchive.GetEntry(thename);
                if (tempzipentry is null)
                {
                    nummissingmetadatafiles++;
                    metadataFilesMissing.Add(thename);
                    Metadatafile_conformant = false;
                }
                else
                {
                    tempmd.Load(ziparchive.GetEntry(thename).Open());
                    MetadataFile themetadatafile = new MetadataFile(tempmd, thename, themetadatatype);
                    if (!themetadatafile.isValid())
                    {
                        metadatafilesErrors.AddRange(themetadatafile.getErrors());
                        numinvalidmetadatafiles++;
                        Metadatafile_conformant = false;
                    }
                    else
                    {
                        numvalidmetadatafiles++;
                    }
                }

            
        }




    }
}
