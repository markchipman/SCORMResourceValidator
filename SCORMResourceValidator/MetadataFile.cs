using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using SCORMResourceValidator.Properties;
using static System.Net.Mime.MediaTypeNames;

namespace SCORMResourceValidator
{
    class MetadataFile
    {
        private String metadataFileName = "";
        private String metadataType = "";
        private Boolean metadataFileExists = false;
        private XmlDocument metadataFileXmlObj = null;
        private int numErrors = 0;
        private List<string> ErrList = new List<string>();
        private Boolean isvalid = true;
        private int tagFound = 0;

        public MetadataFile(XmlDocument metadatafile, string metadatafilename, string metadatatype)
        {
            metadataFileName = metadatafilename;
            metadataType = metadatatype;
            metadataFileXmlObj = metadatafile;

            XmlNode schemaRoot = getSchemaType(metadatafile).Item(0);

            //isvalid = Validate(schemaRoot);
            Validate(schemaRoot);
        }

        public Boolean isValid()
        {
            return isvalid;
        }

        public Boolean Exists()
        {
            //not checking for this yet, not really needed
            //TODO: 
            return (metadataFileExists);
        }

        //Main Validate
        private void Validate(XmlNode s)
        {
           // Boolean state = true;
            // walk the DOM tree and validate...
            for (XmlNode sNode = s.FirstChild; sNode != null; sNode = sNode.NextSibling)
            {
                SchemaElement inSe = new SchemaElement(sNode);
                if (inSe.type.Equals("container") && inSe.ShowTag())
                {
                   // Validating CONTAINER_TYPE
                    if (!validateContainer(inSe))
                    {
                        isvalid = false;
                    }
                }
                else if (inSe.type.Equals("langstring") && inSe.ShowTag())
                {
                    //  Validating LANGSTRING_TYPE
                    if (!validateLangstring(inSe))
                    {
                        isvalid = false;
                    }
                }
                else if (inSe.type.Equals("string") && inSe.ShowTag())
                {
                    // Validating STRING_TYPE
                    if (!validateString(inSe))
                    {
                        isvalid = false;
                    }
                }

                else if (inSe.type.Equals("vocabulary") && inSe.ShowTag())
                {
                    // Validating VOCABULARY_TYPE
                    if (!validateVocabulary(inSe))
                    {
                        isvalid = false;
                    }
                }
                else if (inSe.type.Equals("date") && inSe.ShowTag())
                {
                    // Validating DATE_TYPE
                    if (!validateDate(inSe))
                    {
                        isvalid = false;
                    }
                }
                Validate(sNode);
            }
           
           // return (state);
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
          //  try
           // {
                schemaObj = new XmlDocument();
               // schemaObj.Load(@"C:\Users\kthomann\Documents\Visual Studio 2017\Projects\SCORMResourceValidator\SCORMResourceValidator\Resources\ui_components2004.xml");
                schemaObj.LoadXml(Resources.ui_components2004);
           /* }
            catch (Exception e)
            {
                MessageBox.Show("Error:" + e.Message);
                return (null);
            }*/
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

        private Boolean GoodVocabValue(XmlNode vocabNode, SchemaElement se)
        {
            String value = "";
            Boolean goodValue = false;

            // checks the value of vocabNode from the XML file and determines if it
            // is a good value
            for (XmlNode n = vocabNode.FirstChild; n != null; n = n.NextSibling)
            {
                if (n.Name.Equals("value"))
                {
                    XmlNodeList valueList = n.ChildNodes;
                    for (int num = 0; num < valueList.Count; ++num)
                    {
                        XmlNode valueNode = valueList.Item(num);
                        if (valueNode is XmlText)
                        {
                            XmlText valueText = (XmlText)valueNode;
                            value += valueText.Data;
                        }
                        else
                        {
                            goodValue = false;
                            value = "NON-STRING DATA";
                            break;
                        }
                    }
                    break;
                }
            }
		if (!value.Equals("")) // if not blank, check to see if the actual value exists in the defined vocabulary of se 
		{
			//System.out.println("Testing value: " + value);
			for (int i = 0; i<se.vocab.Count; i++)
			{
				String vVal = (String)se.vocab[i];
				if (vVal.ToLower().Equals(value.ToLower()))
				{
					String reqVal = se.getRequiredValue();
					if(!reqVal.Equals(""))
					{
						goodValue = reqVal.Equals(value);
					}
					else 
					{
						goodValue = true;
					}
					
				}
			}
		}
		if (!goodValue)
		{
                ErrList.Add(metadataFileName + "~" + se.getXMLTagName()
					+ " contains an invalid vocabulary value [" + value + "]");
		}

		return (goodValue);

	} // end GoodVocabValue

	    private  Boolean GoodDateValue(XmlNode dateNode, SchemaElement se)
    {
        String dateValue = "";
        Boolean goodValue = false;

        // checks the value of dateNode from the XML file and determines if it
        // is a good value
        for (XmlNode n = dateNode.FirstChild; n != null; n = n.NextSibling)
        {
            if (n.Name.Equals("datetime"))
            {
                dateValue = n.FirstChild.Value;

                Regex rx = new Regex("(\\d{4})-(\\d{2})-(\\d{2})", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                MatchCollection matches = rx.Matches(dateValue.Trim());
                if (matches.Count > 0)
                {
                        /*
                        int year = Integer.parseInt(matcher.group(1));
                        int month = Integer.parseInt(matcher.group(2));
                        int day = Integer.parseInt(matcher.group(3));
                        if (year > 0)
                        {
                            if (month > 0 && month <= 12)
                            {
                                if (day > 0 && day <= 31)
                                {
                                    goodValue = true;
                                }
                            }
                        }
                        */
                    DateTime tempD;
                    if(DateTime.TryParse(dateValue.Trim(), out tempD))
                    {
                            goodValue = true;
                    }
                }
            }
        }
        if (!goodValue)
        {
                    ErrList.Add(metadataFileName + "~" + se.getXMLTagName()
                    + " contains an invalid date [" + dateValue + "]");
        }
        return (goodValue);
    }

        private Boolean validateContainer(SchemaElement se)
        {
            tagFound = 0;
            
            // get all elements in document that have tagname = se.getXMLTagName()
            XmlNodeList nl = metadataFileXmlObj.GetElementsByTagName(se.getXMLTagName());
            int numElements = nl.Count;

            for (int i = 0; i < numElements; i++)
            {
                XmlNode n = (XmlNode)nl.Item(i);
                // check the element to see if it is one we are looking for
               
                if (se.getParentDefNode().Name.Equals("asset")
                        || se.getParentDefNode().Name.Equals("sco")
                        || se.getParentDefNode().Name.Equals("ca")
                        && (n.ParentNode.Name.Equals("lom")))
                {
                   tagFound++;
                }
                else if (se.samePathFromRoot(n))
                {
                    tagFound++;
                }
            }
            
            // if the number of elements found is >= min and <= max, then return
            // true (good)
            if (tagFound >= se.minOccurs && tagFound <= se.maxOccurs)
            {
                return (true);
            }
            // else we have problems...
            numErrors++;
            if (tagFound < se.minOccurs)
            {
                ErrList.Add(metadataFileName + "~" + se.getXMLTagName()
                        + " occurs less than the defined minimum of "
                        + se.minOccurs + " [" + tagFound + "]");
            }
            else if (tagFound > se.maxOccurs)
            {
                ErrList.Add(metadataFileName + "~" + se.getXMLTagName()
                        + " occurs more than the defined maximum of "
                        + se.maxOccurs + " [" + tagFound + "]");
            }
            return (false);
        } //end validateContainer

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


        } // end validateLangstring

        private Boolean validateString(SchemaElement se)
        {
            // this is essentially the same as validateLangstring, but I choose to
            // keep it seperate for future scaleability...

            // get all elements in document that have tagname = se.getXMLTagName()
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
                    XmlNode child = n.FirstChild;
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
                                && !se.isRequiredValue(child.Value))
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

        } // end validateString

        private Boolean validateVocabulary(SchemaElement se)
        {

            XmlNodeList nl = metadataFileXmlObj.GetElementsByTagName(se.getXMLTagName());
            int numElements = nl.Count;
            Boolean goodValue = true;

            tagFound = 0;
            for (int i = 0; i < numElements; i++)
            {
                XmlNode n = (XmlNode)nl.Item(i);
                // compare the current schema node (se) with the XML doc node (n),
                // if they at the same level, then do some validating...
                if (se.samePathFromRoot(n))
                {
                    tagFound = countTag(n.ParentNode, se); // how many times
                                                                // does the current
                                                                // schemaElement tag
                                                                // (se) occur under
                                                                // parent of n?
                    if (!GoodVocabValue(n, se))
                    {
                        goodValue = false;
                    }
                }
            }
            
            if (tagFound >= se.minOccurs && tagFound <= se.maxOccurs && goodValue)
            {
                return (true);
            }
            // we have an invalid tag...
            numErrors++;
            if (tagFound < se.minOccurs)
            {
                ErrList.Add(metadataFileName + "~" + se.getXMLTagName()
                        + " occurs less than the defined minimum of "
                        + se.minOccurs + " [" + tagFound + "]");
            }
            else if (tagFound > se.maxOccurs)
            {
                ErrList.Add(metadataFileName + "~" + se.getXMLTagName()
                        + " occurs more than the defined maximum of "
                        + se.maxOccurs + " [" + tagFound + "]");
            }
            return (false);
        } //end validateVocabulary

        private Boolean validateDate(SchemaElement se)
        {
            XmlNodeList nl = metadataFileXmlObj.GetElementsByTagName(se.getXMLTagName());
            int numElements = nl.Count;
            Boolean goodValue = true;

            for (int i = 0; i < numElements; i++)
            {
                XmlNode n = (XmlNode)nl.Item(i);
                // compare the current schema node (se) with the XML doc node (n),
                // if they at the same level, then do some validating...
                if (se.samePathFromRoot(n))
                {
                    tagFound = countTag(n.ParentNode, se); // how many times
                                                                // does the current
                                                                // schemaElement tag
                                                                // (se) occur under
                                                                // parent of n?
                    if (!GoodDateValue(n, se))
                    {
                        goodValue = false;
                    }
                }
            }
           
            if (tagFound >= se.minOccurs && tagFound <= se.maxOccurs && goodValue)
            {
                return (true);
            }
            // we have an invalid tag...
            numErrors++;
            if (tagFound < se.minOccurs)
            {
                ErrList.Add(metadataFileName + "~" + se.getXMLTagName()
                        + " occurs less than the defined minimum of "
                        + se.minOccurs + " [" + tagFound + "]");
            }
            else if (tagFound > se.maxOccurs)
            {
                ErrList.Add(metadataFileName + "~" + se.getXMLTagName()
                        + " occurs more than the defined maximum of "
                        + se.maxOccurs + " [" + tagFound + "]");
            }
            return (false);
        } // end validateDate
    }
}
