using System;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Windows.Forms;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Framework;
using Miner.ComCategories;
using PGE.Common.Delivery.Diagnostics;

namespace PGE.Desktop.EDER.ArcMapCommands.PONS
{
    /// <summary>
    /// Summary description for PONSCommand.
    /// </summary>
    //[ClassInterface(ClassInterfaceType.None)]
    [Guid("7cf8a9f4-0cbe-4b3f-988a-2f663444415b")]
    [ComponentCategory(ComCategory.ArcMapCommands)]
    [ComVisible(true)]
    [ProgId("PGE.Desktop.EDER.ArcMapCommands.PONS")]
    public sealed class PONSCommand : BaseCommand
    {
        #region COM Registration Function(s)
        [ComRegisterFunction()]
        [ComVisible(true)]
        static void RegisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryRegistration(registerType);

            //
            // TODO: Add any COM registration code here
            //
        }

        [ComUnregisterFunction()]
        [ComVisible(false)]
        static void UnregisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryUnregistration(registerType);

            //
            // TODO: Add any COM unregistration code here
            //
        }

        #region ArcGIS Component Category Registrar generated code
        /// <summary>
        /// Required method for ArcGIS Component Category registration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryRegistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            MxCommands.Register(regKey);

        }
        /// <summary>
        /// Required method for ArcGIS Component Category unregistration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryUnregistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            MxCommands.Unregister(regKey);

        }

        #endregion
        #endregion

        private IApplication m_application;
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        public PONSCommand()
        {
            //
            // TODO: Define values for the public properties
            //
            base.m_category = "PG&E EDER PONS"; //localizable text
            base.m_caption = "Planned Outage Notification";  //localizable text
            base.m_message = "Starts the Planned Outage Notification tool";  //localizable text 
            base.m_toolTip = "Opens Planned Outage Notification tool";  //localizable text 
            base.m_name = "PGE.Desktop.EDER.ArcMapCommands.PONS";   //unique id, non-localizable (e.g. "MyCategory_ArcMapCommand")

            try
            {
                //
                // TODO: change bitmap name if necessary
                //
                string bitmapResourceName = GetType().Name + ".bmp";
                base.m_bitmap = new Bitmap(GetType(), bitmapResourceName);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message, "Invalid Bitmap");
            }
        }

        #region Overridden Class Methods

        /// <summary>
        /// Occurs when this command is created
        /// </summary>
        /// <param name="hook">Instance of the application</param>
        public override void OnCreate(object hook)
        {
            try
            {
                if (hook == null)
                    return;

                m_application = hook as IApplication;

                //Disable if it is not ArcMap
                if (hook is IMxApplication)
                    base.m_enabled = CheckifPONSUser();
                else
                    base.m_enabled = false;

                //base.m_enabled = false;

                // TODO:  Add other initialization code
                if (m_application != null)
                    PGEGlobal.Application = m_application;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
        }

        private static ConfigurationHelper confighelp = new ConfigurationHelper();
        private static UtilityFunctions utilityFunctions = new UtilityFunctions();
        private bool CheckifPONSUser()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(utilityFunctions.ReadConfigurationValue("PONS_AD_GROUPNAME"));
            //return true;
        }


        PONSHomeScreen homeScreen = null;//new PONSHomeScreen();

        /// <summary>
        /// Occurs when this command is clicked
        /// </summary>
        public override void OnClick()
        {
            // TODO: Add PONSCommand.OnClick implementation
            try
            {
                //
                //  TODO: Sample code showing how to access button host
                //
                UtilityFunctions utilityFunctions = null;
                utilityFunctions = new UtilityFunctions();
                if (utilityFunctions != null)
                {
                    try
                    {
                        ConfigurationHelper configurationHelper = new ConfigurationHelper(/*PGEGlobal.Logger*/);

                        // string[] MapLayerList = { "Transformer", "Substation", "Fuse", "Switch", "OpenPoint", "PriOHConductor", "PriUGConductor", "SUBTransformerBank", "ElectricStitchPoint" };
                        string[] MapLayerList = utilityFunctions.ReadConfigurationValue("Workspace_MapLayerList").Split(','); //{ "Transformer", "Substation", "Fuse", "Switch", "OpenPoint", "PriOHConductor", "PriUGConductor", "ElectricStitchPoint" };
                        MapUtility objMapUtility = new MapUtility();
                        PGEFeatureClass objPGEFeatClass = default(PGEFeatureClass);

                        foreach (string sLayerName in MapLayerList)
                        {
                            if (PGEGlobal.WORKSPACE_MAP == null && objMapUtility != null)
                            {
                                objPGEFeatClass = default(PGEFeatureClass);
                                objPGEFeatClass = objMapUtility.GetFeatureClassByName(utilityFunctions.ReadConfigurationValue(sLayerName));
                                if (objPGEFeatClass != null)
                                {
                                    if (objPGEFeatClass.LayerAddedToMap && objPGEFeatClass.FeatureClass != null)
                                        if (objPGEFeatClass.FeatureClass.FeatureDataset != null)
                                        {
                                            PGEGlobal.WORKSPACE_MAP = objPGEFeatClass.FeatureClass.FeatureDataset.Workspace;
                                            break;
                                        }
                                    objPGEFeatClass.Dispose();
                                }
                            }                
                            
                        }

                        if ((PGEGlobal.pFL_CircuitSource = objMapUtility.GetFeatureLayerfromFCName(utilityFunctions.ReadConfigurationValue("ElectricStitchPoint"))) == null)
                        {
                            MessageBox.Show("Unable to load Circuit Source layer from Map.\nPlease add valid Circuit Source layer and retry.", "PONS", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                            return;
                        }

                        if (PGEGlobal.WORKSPACE_MAP == null)
                        {
                            MessageBox.Show("Stored Display should be loaded to use PONS tool", "PONS", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                            return;
                        }

                        try
                        {
                            if (PGEGlobal.WORKSPACE_EDER == null || PGEGlobal.WORKSPACE_EDERSUB == null)
                            {
                                // Open feature class from SDE files
                                string _SDEFilePath = string.Empty;
                                _SDEFilePath = configurationHelper.GetConfigPath();
                                if (!string.IsNullOrEmpty(_SDEFilePath))
                                {
                                    System.IO.DirectoryInfo pDirectoryInfo = System.IO.Directory.GetParent(_SDEFilePath);
                                    _SDEFilePath = pDirectoryInfo.Parent.FullName;

                                    if (!string.IsNullOrEmpty(_SDEFilePath))
                                    {
                                        string configValue = string.Empty;

                                        configValue = utilityFunctions.ReadSDEConfigValue("EDER_OSUSER_SDE_FILENAME");

                                        //IWorkspaceFactory pSDEWSFactory = default(IWorkspaceFactory);
                                        //IWorkspace pSDEWorkspace = default(IWorkspace);

                                        //configValue = utilityFunctions.ReadSDEConfigValue("EDER_OSUSER_SDE_FILENAME");
                                        if (!string.IsNullOrEmpty(configValue))
                                        {
                                            PGEGlobal.WORKSPACE_EDER = utilityFunctions.GetWorkspaceFromSDEFile(_SDEFilePath, configValue);
                                        //    pSDEWSFactory = new SdeWorkspaceFactoryClass();
                                        //    pSDEWorkspace = pSDEWSFactory.OpenFromFile(System.IO.Path.Combine(_SDEFilePath, configValue), 0);

                                        //    if (pSDEWorkspace != null)
                                        //        PGEGlobal.WORKSPACE_EDER = pSDEWorkspace;
                                        }

                                        if (PGEGlobal.WORKSPACE_EDER == null)
                                        {
                                            configValue = utilityFunctions.ReadSDEConfigValue("EDER_PONSUSER_SDE_FILENAME");
                                            if (!string.IsNullOrEmpty(configValue))
                                            {
                                                PGEGlobal.WORKSPACE_EDER = utilityFunctions.GetWorkspaceFromSDEFile(_SDEFilePath, configValue);
                                                //pSDEWSFactory = new SdeWorkspaceFactoryClass();
                                                //pSDEWorkspace = pSDEWSFactory.OpenFromFile(System.IO.Path.Combine(_SDEFilePath, configValue), 0);

                                                //if (pSDEWorkspace != null)
                                                //    PGEGlobal.WORKSPACE_EDER = pSDEWorkspace;
                                            }
                                        }

                                        configValue = string.Empty;
                                        configValue = utilityFunctions.ReadSDEConfigValue("EDERSUB_OSUSER_SDE_FILENAME");
                                        if (!string.IsNullOrEmpty(configValue))
                                        {
                                            PGEGlobal.WORKSPACE_EDERSUB = utilityFunctions.GetWorkspaceFromSDEFile(_SDEFilePath, configValue);
                                            //pSDEWSFactory = new SdeWorkspaceFactoryClass();
                                            //pSDEWorkspace = pSDEWSFactory.OpenFromFile(System.IO.Path.Combine(_SDEFilePath, configValue), 0);

                                            //if (pSDEWorkspace != null)
                                            //    PGEGlobal.WORKSPACE_EDERSUB = pSDEWorkspace;
                                        }

                                        if (PGEGlobal.WORKSPACE_EDERSUB == null)
                                        {
                                            configValue = string.Empty;
                                            configValue = utilityFunctions.ReadSDEConfigValue("EDERSUB_PONSUSER_SDE_FILENAME");
                                            if (!string.IsNullOrEmpty(configValue))
                                            {
                                                PGEGlobal.WORKSPACE_EDERSUB = utilityFunctions.GetWorkspaceFromSDEFile(_SDEFilePath, configValue);
                                                //pSDEWSFactory = new SdeWorkspaceFactoryClass();
                                                //pSDEWorkspace = pSDEWSFactory.OpenFromFile(System.IO.Path.Combine(_SDEFilePath, configValue), 0);

                                                //if (pSDEWorkspace != null)
                                                //    PGEGlobal.WORKSPACE_EDERSUB = pSDEWorkspace;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        { throw ex; }

                        if (!configurationHelper.CheckPresenceofApplicationConfigfile())
                        {
                            System.Windows.Forms.MessageBox.Show("The PONS Desktop Application configuration file is missing.");
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                //PGEGlobal.Application.CurrentTool = null;
                //if (PGEGlobal.CheckIfFormIsOpen("PONSHomeScreen") == false)
                //{
                //    PONSHomeScreen homeScreen = new PONSHomeScreen();
                //    homeScreen.Show();
                //}
                //else
                //{
                //    FormCollection fc = Application.OpenForms;

                //    foreach (Form frm in fc)
                //    {
                //        if (frm.Name == "PONSHomeScreen")
                //        {
                //            frm.WindowState = FormWindowState.Normal;
                //            frm.Focus();
                //        }
                //    }
                //}

                if (homeScreen != null && !homeScreen.IsDisposed)
                {
                    homeScreen.Focus();
                    homeScreen.WindowState = FormWindowState.Normal;
                }
                else //if (homeScreen == null)
                {
                    homeScreen = new PONSHomeScreen();
                    homeScreen.Show(new ArcMapWindow(this.m_application));
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                MessageBox.Show(ex.Message);
            }
        }

        #endregion
    }
}
