using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace PGE.BatchApplication.GenerateRequiredMxds
{
    class Program
    {
        private static LicenseInitializer m_AOLicenseInitializer = new LicenseInitializer();

        [STAThread()]
        static void Main(string[] args)
        {

            //ESRI License Initializer generated code.
            Console.WriteLine("Checking out Esri license...");
            bool licenseCheckoutSuccess = m_AOLicenseInitializer.InitializeApplication(new esriLicenseProductCode[] { esriLicenseProductCode.esriLicenseProductCodeAdvanced },
            new esriLicenseExtensionCode[] { });

            if (licenseCheckoutSuccess)
            {
                try
                {
                    Console.WriteLine("Checking out ArcFM license...");
                    m_AOLicenseInitializer.GetArcFMLicense(Miner.Interop.mmLicensedProductCode.mmLPArcFM);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unable to checkout ArcFM license");
                    Console.WriteLine(e.Message);
                    licenseCheckoutSuccess = false;
                }
            }


            if (licenseCheckoutSuccess)
            {
                string sdeFileLocation = "";
                string mxdName = "";
                string layersToRemove = "";
                string layersToAdd = "";
                bool isTiff = false;
                string projection = "";

                //Parse our arguments first
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i] == "-i")
                    {
                        sdeFileLocation = args[i + 1];
                    }
                    else if (args[i] == "-M")
                    {
                        mxdName = args[i + 1];
                    }
                    else if (args[i] == "-r")
                    {
                        layersToRemove = args[i + 1];
                    }
                    else if (args[i] == "-a")
                    {
                        layersToAdd = args[i + 1];
                    }
                    else if (args[i] == "-T")
                    {
                        isTiff = true;
                    }
                    else if (args[i] == "-proj")
                    {
                        projection = args[i + 1];
                    }
                }

                CreateNewMxd createMxd = new CreateNewMxd(sdeFileLocation, mxdName, layersToRemove, layersToAdd, isTiff, projection);
                try
                {
                    createMxd.CreateMxd();
                }
                catch (Exception e)
                {
                    createMxd.Log("Error processing Mxd: " + mxdName + " Error: " + e.Message + " StackTrace: " + e.StackTrace);
                }

            }

            try
            {
                m_AOLicenseInitializer.ReleaseArcFMLicense();
            }
            catch { }

            try
            {
                //Do not make any call to ArcObjects after ShutDownApplication()
                m_AOLicenseInitializer.ShutdownApplication();
            }
            catch { }
        }
    }
}
