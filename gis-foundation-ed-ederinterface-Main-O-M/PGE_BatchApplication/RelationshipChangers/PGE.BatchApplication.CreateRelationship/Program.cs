using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace PGE.BatchApplication.CreateRelationship
{
    class Program
    {
        private static LicenseInitializer m_AOLicenseInitializer = new PGE.BatchApplication.CreateRelationship.LicenseInitializer();

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
                    string relName = args[1];
                    string originTable = args[2];
                    string backwardPathLabel = args[3];
                    string originPK = args[4];
                    string originFK = args[5];
                    string destTable = args[6];
                    string forwardPathLabel = args[7];
                    string relCardinality = args[8];
                    string messageDirection = args[9];
                    bool isComposite = Convert.ToBoolean(args[10]);

                    RelationshipBuilder relRuleBuilder = new RelationshipBuilder(databaseLocation, relName, originTable, backwardPathLabel, originPK, originFK, destTable, forwardPathLabel, relCardinality, messageDirection, isComposite);
                    relRuleBuilder.Create();
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
