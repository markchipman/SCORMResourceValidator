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
        private XmlDocument metadataFileXmlObj = null;
        private int numErrors = 0;
       // private string ErrList;
        private List<string> ErrList = new List<string>();
        private Boolean isvalid = true;
        private int tagFound = 0;

        public MetadataFile(XmlDocument md, string mdfilename, string mdtype)
        {
            metadataFileName = mdfilename;
            metadataType = mdtype;
            metadataFileXmlObj = md;
        }

        public Boolean isValid()
        {
            return isvalid;
        }

        public Boolean Exists()
        {
            return (metadataFileExists);
        }

        //Main Validate
        private Boolean Validate(XmlNode s)
        {
            Boolean state = true;
            // walk the DOM tree and validate...
            for (XmlNode sNode = s.FirstChild; sNode != null; sNode = sNode.NextSibling)
            {
                SchemaElement inSe = new SchemaElement(sNode);
                if (inSe.type.Equals("container") && inSe.ShowTag())
                {
                   // Validating CONTAINER_TYPE
                    if (!validateContainer(inSe))
                    {
                        state = false;
                    }
                }
                else if (inSe.type.Equals("langstring") && inSe.ShowTag())
                {
                    //  Validating LANGSTRING_TYPE
                    if (!validateLangstring(inSe))
                    {
                        state = false;
                    }
                }
                else if (inSe.type.Equals("string") && inSe.ShowTag())
                {
                    // Validating STRING_TYPE
                    if (!validateString(inSe))
                    {
                        state = false;
                    }
                }

                else if (inSe.type.Equals("vocabulary") && inSe.ShowTag())
                {
                    // Validating VOCABULARY_TYPE
                    if (!validateVocabulary(inSe))
                    {
                        state = false;
                    }
                }
                else if (inSe.type.Equals("date") && inSe.ShowTag())
                {
                    // Validating DATE_TYPE
                    if (!validateDate(inSe))
                    {
                        state = false;
                    }
                }
                Validate(sNode);
            }
           
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

        public List<string> getErrors()
        {
            return ErrList;
        }

        public XmlDocument getmetadataFileXMLObj()
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

        public static XmlNodeList getSchemaType(XmlDocument x)
        {
            XmlDocument schemaObj;
            try
            {
                schemaObj = new XmlDocument();
               // schemaObj.Load(@"C:\Users\kthomann\Documents\Visual Studio 2017\Projects\SCORMResourceValidator\SCORMResourceValidator\Resources\ui_components2004.xml");
                schemaObj.Load(Resources.ui_components2004);
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

        //helper functions -------------------------------------

        private static int countTag(XmlNode p, SchemaElement se)
        {
            //count the time the tag defined by se occurs under the parent node p in xml document
            int numFound = 0;

            for (XmlNode c = p.FirstChild; c != null; c = c.NextSibling)
            {
                if (c.Name.Equals(se.getXMLTagName()))
                {
                    numFound++;
                }
            }
            return (numFound);
        }

        private Boolean validateContainer(SchemaElement se)
        {
            return true;
        }

        private Boolean validateLangstring(SchemaElement se)
        {
           //ENTERING validateLangstring

            // get all elements in document that have tagname = se.getXMLTagName()
            //XmlNodeList nl = doc.getElementsByTagName(se.getXMLTagName());
            XmlNodeList nl = metadataFileXmlObj.GetElementsByTagName(se.getXMLTagName());
            int numElements = nl.Count;
            Boolean goodElement = true;


            tagFound = 0;
            for (int i = 0; i < numElements; i++)
            {
                XmlNode n = (XmlNode)nl.Item(i);
                // compare the current schema node (se) with the XML doc node (n),
                // if they at the same level, then do some validating...
                if (se.samePathFromRoot(n))
                {
                    XmlNode child = n.FirstChild.FirstChild;
                    if (child != null && !child.Value.Trim().Equals(""))
                    {
                        tagFound = countTag(n.ParentNode, se); // how many
                                                                    // times does
                                                                    // the current
                                                                    // schemaElement
                                                                    // tag (se)
                                                                    // occur under
                                                                    // parent of n?
                        
                        // if tagFound < min OR tagFound > max..then bad
                        if (tagFound < se.minOccurs || tagFound > se.maxOccurs)
                        {
                            goodElement = false;
                            numErrors++;
                        }
                        // if a requiredValue is defined but not present..then bad
                        if (se.gethasRequiredValue()
                                && !se.getRequiredValue().Equals(
                                        child.Value))
                        {
                            goodElement = false;
                            numErrors++;
                            ErrList.Add(metadataFileName + "~"
                                    + se.getXMLTagName()
                                    + " has a required value of "
                                    + se.getRequiredValue() + " ["
                                    + child.Value + "]");
                        }
                        if (tagFound < se.minOccurs)
                        {
                            ErrList.Add(metadataFileName + "~"
                                    + se.getXMLTagName()
                                    + " occurs less than the defined minimum of "
                                    + se.minOccurs + " [" + tagFound + "]");
                        }
                        else if (tagFound > se.maxOccurs)
                        {
                            ErrList.Add(metadataFileName + "~"
                                    + se.getXMLTagName()
                                    + " occurs more than the defined maximum of "
                                    + se.maxOccurs + " [" + tagFound + "]");
                        }
                    }
                    else
                    {
                        goodElement = false;
                        numErrors++;
                        ErrList.Add(metadataFileName + "~"
                                + se.getXMLTagName() + " is a blank element");
                    }
                }
            }
            
            // if the number of elements found is >= min and <= max, then return
            // true (good)
            return (goodElement);



           // return true;
        }

        private Boolean validateString(SchemaElement se)
        {
            return true;
        }

        private Boolean validateVocabulary(SchemaElement se)
        {
            return true;
        }

        private Boolean validateDate(SchemaElement se)
        {
            return true;
        }
    }
}
