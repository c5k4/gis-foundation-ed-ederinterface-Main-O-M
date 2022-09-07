using System;
using System.Collections.Generic;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace PoleLoadExtract
{
    class Program
    {
        private static LicenseInitializer m_AOLicenseInitializer = new PoleLoadExtract.LicenseInitializer();
    
        [STAThread()]
        static void Main(string[] args)
        {
            //ESRI License Initializer generated code.
            m_AOLicenseInitializer.InitializeApplication(new esriLicenseProductCode[] { esriLicenseProductCode.esriLicenseProductCodeAdvanced },
            new esriLicenseExtensionCode[] { });
            string lastDigitsOfOId = string.Empty;
            string processSuffix = string.Empty;
            string fileName = string.Empty;

            //Arguments 
            //----------------
            // -l 
            // is an arg to indicate a comma separates list of 
            // the last digits of the ObjectId of SupportStructure 
            // to be processed by the currently running process 
            // (this is a way of dividing up the work between 
            //concurrently running processes 
            // -p 
            // is an arg which identifies the currently running 
            // process by way of an index 

            // Example 
            // PoleLoadExtract.exe -l 0,1,2 -p 1 
            // So this configuration means it will process all poles 
            // where the last digit of the objectid is a 0 or 1 or 2 
            // and tags the process with an index of 1. The idea is  
            // not to assign the same Poles to two concurrently running 
            // processes   

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-l")
                {
                    lastDigitsOfOId = args[i + 1];
                }
                if (args[i] == "-p")
                {
                    processSuffix = args[i + 1];
                }
                if (args[i] == "-f")
                {
                    fileName = args[i + 1];
                }
            }
            
            //Perform the extract here 
            //Pass args for process number 
            PoleLoad pPoleLoad = new PoleLoad();
            //pPoleLoad.PopulatePLC_InfoFeatureclass(
            //    processSuffix);
            pPoleLoad.ProcessPoles(lastDigitsOfOId, processSuffix); 
            m_AOLicenseInitializer.ShutdownApplication();
        }
    }
}
