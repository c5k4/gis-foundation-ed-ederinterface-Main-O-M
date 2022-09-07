using System;
using System.Collections.Generic;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace PGE.BatchApplication.ApplyDatabaseAttributes
{
    class Program
    {
        private static LicenseInitializer m_AOLicenseInitializer = new PGE.BatchApplication.ApplyDatabaseAttributes.LicenseInitializer();
    
        [STAThread()]
        static void Main(string[] args)
        {
            //ESRI License Initializer generated code.
            bool success = m_AOLicenseInitializer.InitializeApplication(new esriLicenseProductCode[] { esriLicenseProductCode.esriLicenseProductCodeAdvanced },
            new esriLicenseExtensionCode[] { });

            if (success)
            {
                string xslLocation = args[0];
                string databaseLocation = args[1];
                string databaseType = args[2];

                ImportAttributes importAttrib = new ImportAttributes(xslLocation, databaseLocation, databaseType);
                importAttrib.Import();
            }
            else
            {
                Console.WriteLine("Unable to check out Esri ArcInfo License");
            }

            //ESRI License Initializer generated code.
            //Do not make any call to ArcObjects after ShutDownApplication()
            m_AOLicenseInitializer.ShutdownApplication();
        }
    }
}
