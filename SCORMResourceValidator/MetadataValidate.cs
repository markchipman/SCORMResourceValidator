using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using System.IO;
using System.Xml;
using SCORMResourceValidator.Properties;

namespace SCORMResourceValidator
{
    class MetadataValidate
    {
        
        private static string errormsgs = "";
        private XmlSchemaSet schemas = new XmlSchemaSet();
       // private XmlSchema tempschema;
        private string[] ValidationSchemas = {
                "adlcp_v1p3.xsd",
                "adlnav_v1p3.xsd",
                "adlseq_v1p3.xsd",
                "imscp_v1p1.xsd",
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
                "xml.xsd",
                "XMLSchema.dtd"
               };


        public void Check(Stream theMetaFile)
        {

            //grab Schemas
            XmlSchema schema;
            foreach (string s in ValidationSchemas)
            {
                try
                {
                var fs = File.OpenRead(@"C:\Users\kthomann\Documents\Visual Studio 2017\Projects\SCORMResourceValidator\SCORMResourceValidator\Resources\" + s);
                schema = XmlSchema.Read(fs, ValidationCallBack_schema);
                schemas.Add(schema);

                }
                catch(Exception c)
                { }

            }

            // Set the validation settings.
            XmlReaderSettings settings = new XmlReaderSettings();
            XmlUrlResolver resolver = new XmlUrlResolver();
            Uri uri = new Uri(@"C:\Users\kthomann\Documents\Visual Studio 2017\Projects\SCORMResourceValidator\SCORMResourceValidator\Resources\");
            resolver.ResolveUri(uri,"");


            settings.ValidationType = ValidationType.Schema;
            //settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessInlineSchema;
            //settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessSchemaLocation;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
            settings.ValidationEventHandler += new ValidationEventHandler(ValidationCallBack);
            settings.Schemas.Add(schemas);
            settings.XmlResolver = resolver;

            // Create the XmlReader object.
            XmlReader reader = XmlReader.Create(theMetaFile, settings);
           

            // Parse the file. 
            while (reader.Read()) ;
            reader.Close();

          

        } // end init

        private static void ValidationCallBack(object sender, ValidationEventArgs args)
        {
            if (args.Severity == XmlSeverityType.Warning)
                errormsgs = errormsgs + "\r\n Warning: Matching schema not found.  No validation occurred. " + args.Message;
            else
                errormsgs = errormsgs + "\r\n Validation error: " + args.Message;
                

        } // end ValidationCallBack

        private static void ValidationCallBack_schema(object sender, ValidationEventArgs args)
        {
            if (args.Severity == XmlSeverityType.Warning)
                errormsgs = errormsgs + "\r\n Warning: Schema issue: " + args.Message;
            else
                errormsgs = errormsgs + "\r\n Schema error: " + args.Message;


        } // end ValidationCallBack

        public string geterrors()
        {
            return errormsgs;
        }

        public XmlSchemaSet getSchemaSets()
        {
            return schemas;
        }

    }
}
