using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SCORMResourceValidator
{
    class SchemaElement
    {
    private static  String     MINOCCURS 			= "minOccurs";
	private static  String     MAXOCCURS 			= "maxOccurs";
	private static  String     VOCAB 				= "vocab";
	private static  String     NAME	  			= "name";
	private static  String     TYPE 				= "type";
	private static  String     SHOWTAG				= "showTag";
	private static  String     ISSELECTABLE		= "isSelectable";
	private static  String     REQUIREDVALUE		= "requiredValue";
	private static  String     TYPE_DELIM 			= ":";
	private static  String     VOCAB_DELIM 		= ",";
	private static  String     BLANK		 		= "";
	public static  int UNBOUND = 999999;

    public int minOccurs = 0;
    public int maxOccurs = 0;
    public String type = "";
    public String name = "";
    private String tagName = "";
    private String requiredValue = "";
    private Boolean showTag = true;
    private Boolean isSelectable = true;
    private Boolean hasRequiredValue = false;
    public ArrayList vocab = new ArrayList();
    private XmlNode defNode = null;
    private XmlNode parentDefNode = null;
    private String pathFromRoot = "";

        public SchemaElement(XmlNode eDef)
        {
            //attributes define this tag, so get them...
            XmlNamedNodeMap atts = eDef.Attributes;

            defNode = eDef;
            parentDefNode = eDef.ParentNode;

            //physical tag name for this node
            tagName = eDef.Name;
            
            //get all the attributes and set the object instance variables.
            for (int i = 0; i < atts.Count; i++)
            {
                XmlNode thisAtt = atts.Item(i);
                if (thisAtt.Name.Equals(MINOCCURS))
                {
                    minOccurs = Int32.Parse(thisAtt.Value);
                }
                else if (thisAtt.Name.Equals(MAXOCCURS))
                {
                    if (thisAtt.Value.Equals(BLANK))
                    {
                        maxOccurs = UNBOUND;
                    }
                    else
                    {
                        maxOccurs = Int32.Parse(thisAtt.Value);
                    }
                }
                else if (thisAtt.Name.Equals(NAME))
                {
                    name = thisAtt.Value;
                    if (name.Equals(""))
                    {
                        name = eDef.Name.First().ToString().ToUpper() + eDef.Name.Substring(1);
                    }
                }
                else if (thisAtt.Name.Equals(TYPE))
                {
                    //StringTokenizer st = new StringTokenizer(thisAtt.Value, TYPE_DELIM);
                    string[] st = thisAtt.Value.Split(':');
                    if (st.Count() > 1)
                    {
                        type = st[1];
                    }
                    else
                    {
                        type = thisAtt.Value;
                    }
                }
                else if (thisAtt.Name.Equals(VOCAB))
                {
                   // StringTokenizer st = new StringTokenizer(thisAtt.Value, VOCAB_DELIM);
                    string[] st = thisAtt.Value.Split(',');
                    int numToks = st.Count();
                    vocab.Add("No Selection");
                    for (int j = 0; j < numToks; j++)
                    {
                        vocab.Add(st[1].Trim());
                    }
                    vocab.Add("Other");
                }
                else if (thisAtt.Name.Equals(SHOWTAG))
                {
                    if (thisAtt.Value.Equals("false"))
                    {
                        showTag = false;
                    }
                }
                else if (thisAtt.Name.Equals(ISSELECTABLE))
                {
                    if (thisAtt.Value.Equals("false"))
                    {
                        isSelectable = false;
                    }
                }
                else if (thisAtt.Name.Equals(REQUIREDVALUE))
                {
                    requiredValue = thisAtt.Value;
                    if (!requiredValue.Equals(""))
                    {
                        hasRequiredValue = true;
                    }
                }
            }
        } //end constructor

        //helper Functions ------------------------------------------------------------
        public Boolean ShowTag()
        {
            return (showTag);
        }

        //get functions ----------------------------------------------------------------
        public XmlNode getDefNode()
        {
            return (defNode);
        }

        public XmlNode getParentDefNode()
        {
            return (parentDefNode);
        }

        public String getXMLTagName()
        {
            return (tagName);
        }

        public String getPathFromSchemaRoot()
        {
            if (!pathFromRoot.Equals(""))
            {
                return pathFromRoot;
            }
            XmlElement root = defNode.OwnerDocument.DocumentElement;

            if (defNode.Equals(root))
            {
                return null;
            }

            XmlElement testEle = (XmlElement)defNode;
            String path = testEle.Name;
            testEle = (XmlElement)testEle.ParentNode;

            while (!testEle.Equals(root))
            {
                path = testEle.Name + "/" + path;
                testEle = (XmlElement)testEle.ParentNode;
            }
            pathFromRoot = path;
            return pathFromRoot;
        }

        public String getPathFromRoot(XmlNode testNode)
        {
            XmlElement root = testNode.OwnerDocument.DocumentElement;

            if (testNode.Equals(root))
            {
                return null;
            }

            XmlElement testEle = (XmlElement)testNode;
            String path = testEle.Name;

            while (!testEle.Equals(root))
            {
                testEle = (XmlElement)testEle.ParentNode;
                path = testEle.Name + "/" + path;
            }

            return path;
        }

        public Boolean samePathFromRoot(XmlNode testNode)
        {
            Boolean success = false;
            String schemaPath = getPathFromSchemaRoot().Substring(getPathFromSchemaRoot().IndexOf('/'));
            String nodePath = getPathFromRoot(testNode).Substring(getPathFromRoot(testNode).IndexOf('/'));

            success = schemaPath.Equals(nodePath);
            return success;
        }

        //Required Value functions -----------------------------------------------------
        public String getRequiredValue()
        {
            return (requiredValue);
        }
       
        public Boolean isRequiredValue(String val)
        {
            Boolean success = false;
            // Allow for delimited values in the reqValue ATTR of an Element
            string[] stringSeparators = new string[] { "-AND-" };
            String[] reqVals = requiredValue.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < reqVals.Count(); ++i)
            {
                if (val.Equals(reqVals[i].Trim()))
                {
                    success = true;
                    break;
                }
            }
            return success;
        }
       
        public Boolean gethasRequiredValue()
        {
            return (hasRequiredValue);
        }

    }
}
