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
        private List<string> metadatafilesErrors;

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
            logFilelist.Add("metadata_file_report.doc");
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


        private void btnValidate_Click(object sender, EventArgs e)
        {
          //  resettheForm();
            if (readPIFFile())
            {
                turnOnGUI();
                createLogs();
                compareLogs();
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

            //optional Log file folder chooser
            logFolderBrowserDialog.Reset();
            lblLogFileDir.Text = "No Directory selected";
            lbllogvalidate.Visible = false;
            warningLogsfail.Visible = false;
            warningPiffmissing.Visible = false;
            warningMFMising.Visible = false;

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
            XDocument tempXml;
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

                //intPIFFilesFoundCount = archive.Entries.Count;

                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    
                    
                    String entryFilename = WebUtility.UrlDecode(entry.FullName);
                    if(!entryFilename.Substring(entryFilename.Length - 1).Equals("/"))
                        { intPIFFilesFoundCount++; }

                    if (!entryFilename.Substring(entryFilename.Length - 1).Equals("/") && !isSCORMSchemaFile(entryFilename))
                    {
                        pifFiles.Add(entryFilename);
                        listPIFFilesFound.Items.Add(entryFilename);
                    }

                    if (!entryFilename.Substring(entryFilename.Length - 3).Equals("dtd") && isSCORMSchemaFile(entryFilename))
                    {
                       // tempschema = XmlSchema.Read(entry.Open(),null);
                       // schemas.Add(tempschema);
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

                        // This creates the lists and counts of all the files listed in the Manifest
                        // TODO: Also figures out what it's metadata type is
                        var mdFiles = manifest.Root.Descendants(ns + "metadata").Descendants(mdns + "location");
                        var files = manifest.Root.Descendants(ns + "file");
                       
                        foreach (var file in files)
                        {
                            String filename = WebUtility.UrlDecode(file.Attribute("href").Value);
                            if (!isSCORMSchemaFile(filename))
                            {
                                manifestFiles.Add(filename);
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
                        
                        ContentAgmetafiles = manifest.Root.Descendants(ns + "organizations").Descendants(ns + "metadata").Descendants(mdns + "location");
                        foreach (var ContentAgmetafile in ContentAgmetafiles)
                        {
                            numContentAgMetadatafiles++;
                            numMetadatafiles++;
                            metadataXMLfiles.Add(ContentAgmetafile.Value.ToString());
                        }

                        SCOmetafiles = manifest.Root.Descendants(ns + "resources").Descendants(ns + "resource").Where(el => el.Attribute(mdns + "scormType").Value == "sco").Descendants(mdns + "location");
                        foreach (var SCOfile in SCOmetafiles)
                        {
                            numSCOmetadatafiles++;
                            numMetadatafiles++;
                            metadataXMLfiles.Add(SCOfile.Value.ToString());
                        }

                        Assetmetafiles = manifest.Root.Descendants(ns + "resources").Descendants(ns + "resource").Where(el => el.Attribute(mdns + "scormType").Value == "asset").Descendants(mdns + "location");
                        foreach (var Assetfile in Assetmetafiles)
                        {
                            numAssetmetadatafiles++;
                            numMetadatafiles++;
                            metadataXMLfiles.Add(Assetfile.Value.ToString());
                            
                        }
                        
                        // Set styles
                        lblStatus.BorderStyle = BorderStyle.FixedSingle;
                        lblStatus.Text = "Success! Saved logs for " + strPIFFilename + ".";
                        lblStatus.BackColor = Color.FromArgb(200, 255, 200);
                        linkViewLogs.Visible = true;
                    } // end imsmanifest if
                    else if (entryFilename.Contains(".xml"))
                    {
                       // tempXml = XDocument.Load(entry.Open());

                        /*
                        if (isValidXML(entry)) validXMLfiles.Add(entryFilename);
                        else invalidXMLfiles.Add(entryFilename);
                        */

                    }


                } // end foreach for ZipArchive


                // Validate metadata files
                XmlDocument tempmd = new XmlDocument();

                foreach (var c in ContentAgmetafiles)
                {
                    string thename = c.Value.ToString();
                    tempmd.Load(archive.GetEntry(thename).Open());
                    MetadataFile contentmetadatafile = new MetadataFile(tempmd, thename, "content aggregation");
                    if (!contentmetadatafile.isValid())
                    {
                        metadatafilesErrors.AddRange(contentmetadatafile.getErrors());
                        numinvalidmetadatafiles++;
                    }
                }

                foreach (var s in SCOmetafiles)
                {
                    string thename = s.Value.ToString();
                    tempmd.Load(archive.GetEntry(thename).Open());
                    MetadataFile scometadatafile = new MetadataFile(tempmd, thename, "sco");
                    if (!scometadatafile.isValid())
                    {
                        metadatafilesErrors.AddRange(scometadatafile.getErrors());
                        numinvalidmetadatafiles++;
                    }
                }

                foreach (var a in Assetmetafiles)
                {
                    string thename = a.Value.ToString();
                    tempmd.Load(archive.GetEntry(thename).Open());
                    MetadataFile assetmetadatafile = new MetadataFile(tempmd, thename, "asset");
                    if (!assetmetadatafile.isValid())
                    {
                        metadatafilesErrors.AddRange(assetmetadatafile.getErrors());
                        numinvalidmetadatafiles++;
                    }
                }

                // Compare PIF and manifest lists to generate the lists of missing files
                var pifFilesMissing = manifestFiles.Where(mFile => !pifFiles.Contains(mFile)); // these are files listed in the manifest but not found in the PIF
                var manifestFilesMissing = pifFiles.Where(pFile => !manifestFiles.Contains(pFile)); // these are non-schema files found in the PIF but are not listed in the manifest

                var metadataFilesMissing = metadataXMLfiles.Where(mFile => !pifFiles.Contains(mFile)); // metadata files listed in the manifest but not found in the PIF
                nummissingmetadatafiles = metadataFilesMissing.Count();

                var invalidmetadataFiles = invalidXMLfiles.Where(mFile => metadataXMLfiles.Contains(mFile)); //fix this or remove
                numinvalidmetadatafiles = invalidmetadataFiles.Count();

                var validmetadataFiles = validXMLfiles.Where(mFile => metadataXMLfiles.Contains(mFile));
                numvalidmetadatafiles = validmetadataFiles.Count();

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
             System.IO.File.WriteAllText(strLogDir + @"\metadata_file_report.doc", logHTMLTemplate);
             

            //Generate checksum
            string sdf = "";
            sdf = GetMD5HashFromFile(openFileDialog1.FileName);
            System.IO.File.WriteAllText(strLogDir + @"\PIF_file_validate.txt", sdf);

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

            // TODO: add list of INVALID metadata files
            logTemplate = logTemplate + metadatafiles_errors;

            logTemplate = logTemplate + metadatafilesErrors + "\r\n";

            

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
            System.Windows.Forms.MessageBox.Show("SCORM Resource Validator v2.0\n© 2017 JANUS Research Group", "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
        /// Validate if a document from a zip file is a valid XML file
        /// </summary>
        /// <param name="xmlentry"></param>
        /// <returns>bool</returns>
        private Boolean isValidXML(ZipArchiveEntry xmlentry)
        {
             try
             {
                 XDocument m = XDocument.Load(xmlentry.Open());
                MetadataValidate metadataValidate = new MetadataValidate();
                metadataValidate.Check(xmlentry.Open());
                m.Validate(metadataValidate.getSchemaSets(), (o, e) =>
                {
                    metadatafiles_errors = metadatafiles_errors + e.Message;
                });
             }
             catch 
             {
                return false;
             } 
             
            
            /*

            if (metadataValidate.geterrors() != "")
            {
                metadatafiles_errors = metadatafiles_errors + metadataValidate.geterrors();
                return false;
            }
            */
            return true;
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
                    lbllogvalidate.Text = "Log files missing or did not match, Please see logs above for details.";
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
            
            return true;
        }

        /// <summary>
        /// Creates a checksum for the file specified using MD5
        /// </summary>
        /// <param name="input">path and file name of the file to create a hash for</param>
        protected string GetMD5HashFromFile(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    return Encoding.Default.GetString(md5.ComputeHash(stream));
                }
            }
        }

      


    }
}
