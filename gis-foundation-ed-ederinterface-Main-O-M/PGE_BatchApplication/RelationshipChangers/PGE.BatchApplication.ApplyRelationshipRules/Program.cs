using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace PGE.BatchApplication.ApplyRelationshipRules
{
    class Program
    {
        private static LicenseInitializer m_AOLicenseInitializer = new PGE.BatchApplication.ApplyRelationshipRules.LicenseInitializer();

        [STAThread()]
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
                    string relClassName = args[1];
                    int destSubtype = Convert.ToInt32(args[2]);
                    int[] destCardinality = new int[2];
                    int originSubtype = Convert.ToInt32(args[4]);
                    int[] originCardinality = new int[2];

                    for (int x = 0; x < 2; x++)
                    {
                        destCardinality[x] = Convert.ToInt32(args[3].Split('-')[x]);
                        originCardinality[x] = Convert.ToInt32(args[5].Split('-')[x]);
                    }

                    RelRuleBuilder relRuleBuilder = new RelRuleBuilder(relClassName, databaseLocation, destSubtype, destCardinality, originSubtype, originCardinality);
                    relRuleBuilder.Attach();
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
