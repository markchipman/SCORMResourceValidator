using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using SCORMResourceValidator.Properties;

namespace SCORMResourceValidator
{
    class MetadataFile
    {
        private String metadataFileName = "";
        private String metadataType = "";
        private Boolean metadataFileExists = false;
        private XDocument metadataFileXmlObj = null;
        private int numErrors = 0;
        private string ErrList;

        public MetadataFile(XDocument md, string mdfilename, string mdtype)
        {
            metadataFileName = mdfilename;
            metadataType = mdtype;
            metadataFileXmlObj = md;
        }

        private void Validate()
        {
            XDocument m = metadataFileXmlObj;

          


        }

        public Boolean isValid()
        {
            return true;
        }

        public Boolean Exists()
        {
            return (metadataFileExists);
        }

        //Main Validate
        private Boolean Validate(XmlNode s)
        {
            // walk the DOM tree and validate...
            for (XmlNode sNode = s.FirstChild; sNode != null; sNode = sNode.NextSibling)
            {
                schemaElement inSe = new schemaElement(sNode);
                if (inSe.type.equals(CONTAINER_TYPE) && inSe.showTag())
                {
                    debug.print(metadataEditor.DEBUG, "	Validating CONTAINER_TYPE");
                    if (!validateContainer(inSe))
                    {
                        state = false;
                    }
                }
                else if (inSe.type.equals(LANGSTRING_TYPE) && inSe.showTag())
                {
                    debug
                            .print(metadataEditor.DEBUG,
                                    "	Validating LANGSTRING_TYPE");
                    if (!validateLangstring(inSe))
                    {
                        state = false;
                    }
                }
                else if (inSe.type.equals(STRING_TYPE) && inSe.showTag())
                {
                    debug.print(metadataEditor.DEBUG, "	Validating STRING_TYPE");
                    if (!validateString(inSe))
                    {
                        state = false;
                    }
                }

                else if (inSe.type.equals(VOCABULARY_TYPE) && inSe.showTag())
                {
                    debug
                            .print(metadataEditor.DEBUG,
                                    "	Validating VOCABULARY_TYPE");
                    if (!validateVocabulary(inSe))
                    {
                        state = false;
                    }
                }
                else if (inSe.type.equals(DATE_TYPE) && inSe.showTag())
                {
                    debug.print(metadataEditor.DEBUG, "	Validating DATE_TYPE");
                    if (!validateDate(inSe))
                    {
                        state = false;
                    }
                }
                begin(sNode);
            }
            if (state)
            {
                debug.print(metadataEditor.DEBUG, "	FILE IS VALID");
            }
            else
            {
                debug.print(metadataEditor.DEBUG, "	FILE IS IN-VALID");
            }
            debug.print(metadataEditor.DEBUG, "LEAVING Validate.begin()");
            debug.print(metadataEditor.DEBUG, "returning " + state);
            return (state);
        }

        //Get Functions ----------------------------------

        public string getFilename()
        {
            return metadataFileName;
        }

        public int getNumErrors()
        {
            return numErrors;
        }

        public string getErrors()
        {
            return ErrList;
        }

        public XDocument getmetadataFileXMLObj()
        {
            return metadataFileXmlObj;
        }

        //getMetaDataType
        //Retrieve the meta data type of the file
        // ...
        private string getMetaDataType()
        {
           return metadataType;
        }

        //helper functions -------------------------------------

        public static XmlNodeList getSchemaType(XmlDocument x)
        {
            XmlDocument schemaObj;
            try
            {
                schemaObj = new XmlDocument();
                schemaObj.Load(@"C:\Users\kthomann\Documents\Visual Studio 2017\Projects\SCORMResourceValidator\SCORMResourceValidator\Resources\ui_components2004.xml");
                Resources.
            }
            catch (Exception e)
            {
                return (null);
            }
            XmlNodeList sType = schemaObj.GetElementsByTagName("sco");// default
            // determine which schematype to use...

            if (x != null)
            {
                XmlNodeList aLevelTag = x.GetElementsByTagName("aggregationlevel");
                // if the tag name aggregationLevel does not exist
                if (aLevelTag.Count == 0)
                {
                    // use the location and format tags to determine the schema type
                    XmlNodeList docLocs = x.GetElementsByTagName("location");
                    XmlNodeList formats = x.GetElementsByTagName("format");

                    for (int i = 0; i < docLocs.Count; i++)
                    {
                        String loc = docLocs.Item(i).FirstChild.Value.Trim();

                        Regex rx = new Regex("([\\W*\\w*\\d*]+).(\\w+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                        MatchCollection matches = rx.Matches(loc);
                      
                        // if location is a filename and the number of formats <= 1
                        // then assume an ASSET type
                        if (matches.Count >= 1)
                        {
                            if (formats.Count <= 1)
                            {
                                sType = schemaObj.GetElementsByTagName("asset");
                            }
                        }
                    }
                }
                else
                // get aggregationLevel from document..
                {
                    for (XmlNode n = aLevelTag.Item(0).FirstChild; n != null; n = n.NextSibling)
                    {
                        if (n.Name.Equals("value"))
                        {
                            if (n.FirstChild.FirstChild != null)
                            {
                                int level = Int32.Parse(n.FirstChild.FirstChild.Value);

                                // default type = SCO as defined above in sType
                                // declaration, so its not listed here.
                                switch (level)
                                {
                                    case 1: // asset
                                        sType = schemaObj.GetElementsByTagName("asset");
                                        break;
                                    case 3: // content aggregation
                                        sType = schemaObj.GetElementsByTagName("ca");
                                        break;
                                }
                            }
                        }
                    }
                }
            }
            
            return (sType);

        } //end getSchemaType
    }
}
