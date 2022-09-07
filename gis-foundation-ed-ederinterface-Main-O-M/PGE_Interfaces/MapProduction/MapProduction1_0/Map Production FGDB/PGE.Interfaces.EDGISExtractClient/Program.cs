using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ESRI.ArcGIS.esriSystem;
using Miner.Interop; 

namespace PGE.Interfaces.EDGISExtract
{
    static class Program
    {
        private static LicenseInitializer m_AOLicenseInitializer = new PGE.Interfaces.EDGISExtract.LicenseInitializer();
        private static IMMAppInitialize _appInitialize;

        /// <summary>
        /// The main entry point for the application.
        /// 
        /// </summary>
        [STAThread]
        static int Main(string[] args)
        {
            try
            {
                //Call the extract client 
                bool hasError = false;
                bool compactGDB = false;
                bool createLabels = false;
                string masterGDBPath = string.Empty;
                string mapGeo = string.Empty;
                int processId = -1;
                ExtractType pExtractType = ExtractType.ExtractTypeEDGIS;
                Extracter pExtracter = null;

                for (int i = 0; i < args.Length; i++)
                {
                    switch (args[i])
                    {
                        //Parameter list 
                        case "-p": //defines the process index 
                            processId = Convert.ToInt32(args[i + 1]);
                            break;
                        case "-o": //defines the map geo (state plane coord sys)
                            mapGeo = args[i + 1].ToString();
                            break;
                        case "-m": //flag to indicate compact function, and move fcs into feature dataset 
                            compactGDB = true;
                            break;
                        case "-c": //flag to indicate creating labels 
                            createLabels = true;
                            break;
                        case "-f": //required parameter for creating labels                            
                            masterGDBPath = args[i + 1].ToString(); 
                            break; 
                        case "-l": //flag to indicate landbase extract  
                            pExtractType = ExtractType.ExtractTypeLandbase;
                            break;
                    }
                }

                //System.Windows.Forms.MessageBox.Show( 
                //    "compactGBD: " + compactGDB.ToString() + " " + 
                //    "pExtractType: " + pExtractType.ToString() + " " +
                //    "mapGeo: " + mapGeo + " " +
                //    "processId: " + processId.ToString()); 


                //ESRI License Initializer generated code.
                m_AOLicenseInitializer.InitializeApplication(new esriLicenseProductCode[] { esriLicenseProductCode.esriLicenseProductCodeAdvanced },
                new esriLicenseExtensionCode[] { esriLicenseExtensionCode.esriLicenseExtensionCodeNetwork, esriLicenseExtensionCode.esriLicenseExtensionCodeMLE });
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                //Check out PGE.Intefaces license
                if (CheckOutArcFMLicense(mmLicensedProductCode.mmLPArcFM) != mmLicenseStatus.mmLicenseCheckedOut)
                {
                    System.Console.WriteLine("This application could not initialize with the correct ArcFM license and will shutdown.");
                    return 1;
                }

                pExtracter = new Extracter(processId, pExtractType);
                if (compactGDB)
                {
                    //rebuild indexes for all datasets and compacts GDB 
                    //and then copies to central location on file system 
                    pExtracter.PackageOutputGDB(mapGeo, pExtractType);
                }
                else if (createLabels)
                {
                    //Builds the file GDB featureclasses for the maintenanceplat labels 
                    pExtracter.CreatePlatLabels(masterGDBPath);
                }
                else
                {
                    //client loading process (for Landbase or ED Extract) 
                    bool useChangeDetection = false;
                    if (pExtractType == ExtractType.ExtractTypeEDGIS)
                        useChangeDetection = true;
                    hasError = !(pExtracter.ProcessExtract(
                        mapGeo,
                        useChangeDetection));
                }                

                //Return windows exit code 
                if (hasError)
                    return 1;
                else
                    return 0;
            }
            catch
            {
                return 1;
            }
            finally
            {
                //ESRI License Initializer generated code.
                //Do not make any call to ArcObjects after ShutDownApplication()
                m_AOLicenseInitializer.ShutdownApplication();
            }
        }

        private static mmLicenseStatus CheckOutArcFMLicense(mmLicensedProductCode productCode)
        {
            if (_appInitialize == null) _appInitialize = new MMAppInitializeClass();

            var licenseStatus = _appInitialize.IsProductCodeAvailable(productCode);
            if (licenseStatus == mmLicenseStatus.mmLicenseAvailable)
            {
                licenseStatus = _appInitialize.Initialize(productCode);
            }

            return licenseStatus;
        }

    }
}