using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCORMResourceValidator
{
    class SCOInfo
    {
        private readonly string SCO_name;
        private readonly string theLaunchFile;
       
        private List<Form1.LogValues> mainCGIcalls = new List<Form1.LogValues>();
        private List<Form1.LogValues> extraCGIcalls = new List<Form1.LogValues>();
        private List<string> SummarylogErrors = new List<string>();

        public SCOInfo(string SCOname, string launchfile)
        {
            SCO_name = SCOname;
            theLaunchFile = launchfile;


        }

        public string getSCOname() { return SCO_name; }

        public string getLaunchfile() { return theLaunchFile; }

        public void AddmainCGIcallsList(List<Form1.LogValues> maincgicalls)
        {
            mainCGIcalls = maincgicalls;
        }

        public void AddextraCGIcallsList(List<Form1.LogValues> extracgicalls)
        {
            extraCGIcalls = extracgicalls;
        }

        public void AddtomainCGIList( Form1.LogValues Logitem)
        {
            mainCGIcalls.Add(Logitem);
        }

        public void AddtoextraCGIList(Form1.LogValues Logitem)
        {
            extraCGIcalls.Add(Logitem);
        }

        public List<Form1.LogValues> getmainCGIcalls()
        {
            return mainCGIcalls;
        }

        public List<Form1.LogValues> getextraCGIcalls()
        {
            return extraCGIcalls;
        }

        public void AddtoSummaryLogErrors(string errormsg)
        {
            SummarylogErrors.Add(errormsg);
        }

        public void setSummaryLogErrors(List<string> logerrors)
        {
            SummarylogErrors.AddRange(logerrors);
        }

        public int getSummaryLogErrorsCount()
        {
            return SummarylogErrors.Count();
        }

        public List<string> getSummaryLogErrors()
        {
            return SummarylogErrors;
        }

        public void clearSummarylogErrors()
        {
            SummarylogErrors.Clear();
        }

        //this just matches the Detail Luanch file
        public bool match(string lfile)
        {
            string launchdetailedlogfile = theLaunchFile.Substring(theLaunchFile.LastIndexOf("\\") + 1);
            if (lfile == launchdetailedlogfile)
                return true;
            else return false;
        }


    }
}
