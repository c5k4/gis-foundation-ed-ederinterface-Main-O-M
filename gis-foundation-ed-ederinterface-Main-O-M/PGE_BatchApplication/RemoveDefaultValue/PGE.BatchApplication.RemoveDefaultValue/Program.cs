using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace PGE.BatchApplication.RemoveDefaultValue
{
    class Program
    {
        private static LicenseInitializer m_AOLicenseInitializer = new PGE.BatchApplication.RemoveDefaultValue.LicenseInitializer();
    
        static void Main(string[] args)
        {
            //ESRI License Initializer generated code.
            bool success = m_AOLicenseInitializer.InitializeApplication(new esriLicenseProductCode[] { esriLicenseProductCode.esriLicenseProductCodeAdvanced },
            new esriLicenseExtensionCode[] { });

            if (success)
            {
                try
                {
                    string databaseLocation = args[0];
                    string featClassName = args[1];
                    string fieldName = args[2];
                    int? applyToSubtype = null;

                    if (args.Length > 3 && !string.IsNullOrEmpty(args[3]))
                    {
                        applyToSubtype = Convert.ToInt32(args[3]);
                    }

                    FieldDefaultHelper fieldHelper = new FieldDefaultHelper(databaseLocation, featClassName, fieldName);
                    fieldHelper.RemoveDefault(applyToSubtype);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error in operation: " + ex.Message);
                }
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
