using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms; 

using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem; 
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.GeoDatabaseDistributed;
using ESRI.ArcGIS.Geometry;

using Miner.Interop;
using Miner.Interop.Process;
using Miner.ComCategories;

using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Diagnostics;
using PGE.Common.Delivery.Framework;
using PGE.Common.Delivery.Systems;
using PGE.Common.Delivery.UI.Commands; 


using PGE.Desktop.EDER.ValidationRules.UI; 




namespace PGE.Desktop.EDER.ArcMapCommands.MapUtilities
{
    /// <summary>
    /// Summary description for TNSPingCommand.
    /// </summary>
    [Guid("deb9a62a-d2eb-4be4-bbfb-90f5aecadc55")]
    [ComponentCategory(ComCategory.ArcMapCommands)]
    [ProgId("PGE.Desktop.EDER.ArcMapCommands.MapUtilities.TNSPingCommand")]
    [ComVisible(true)]
    public sealed class TNSPingCommand : BaseArcGISCommand
    {
        #region Com Reg Methods

        /// <summary>
        /// Registers the specified reg key.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComRegisterFunction]
        private static void Register(string regKey)
        {
            Miner.ComCategories.ArcMapCommands.Register(regKey);
        }
        /// <summary>
        /// Uns the register.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComUnregisterFunction]
        private static void UnRegister(string regKey)
        {
            Miner.ComCategories.ArcMapCommands.Unregister(regKey);
        }

        #endregion

        private IApplication _application;
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");


        public TNSPingCommand() :
            base("PGETools_TNSPingCommand", "TNSPing Connected Map Workspaces", "PGE Tools", "TNSPing Connected Map Workspaces", "TNSPing Connected Map Workspaces")
        {

            base.m_name = "PGETools_TNSPingCommand";
            try
            {
                Bitmap bmp = null;
                //Get path for bitmap image 
                string path = GetType().Assembly.GetName().Name + ".ArcMapCommands.MapUtilities." + GetType().Name + ".bmp";
                //Get bitmap image
                _logger.Debug("Bitmap image path" + path); 
                bmp = new Bitmap(GetType().Assembly.GetManifestResourceStream(path));
                //Assign bitmap image
                UpdateBitmap(bmp, 0, 0);
            }
            catch (Exception ex)
            {
                _logger.Warn("Invalid Bitmap" + ex.Message);
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
            if (hook == null) return;
            _application = hook as IApplication;
        }

        /// <summary>
        /// Occurs when this command is clicked
        /// </summary>
        protected override void InternalClick()
        {
            try
            {

                //Look through all the feature layers 
                //Find all the workspaces 
                IMxDocument pMxDoc = (IMxDocument)_application.Document;
                IMap pMap = pMxDoc.FocusMap; 
                IEnumLayer enumLayer = pMap.get_Layers(CartoFacade.UIDFacade.FeatureLayers, true);
                enumLayer.Reset();
                ILayer layer;
                IFeatureLayer pFL = null;
                IDataset pDS = null;
                IWorkspace pWS = null; 
                IPropertySet pPropSet = null;
                Hashtable hshTNSEntries = new Hashtable();

                //Constants 
                const string SERVICE_NAME_STRING = "SERVICE_NAME = ";
                const string CLOSE_BRACKET_STRING = ")";
                const string INSTANCE_PROP_NAME = "INSTANCE";
                const string TNS_PING_COMMAND_NAME = "tnsping";
                const string USER_MSG_TNS_ENTRY = "TNS Entry: ";
                const string USER_MSG_SPACE = " ";
                const string USER_MSG_MAPS_TO = " maps to: ";

                //loop through map feature layers  
                while ((layer = enumLayer.Next()) != null)
                {
                    //Validate the layer and check layer name
                    if (layer is IFeatureLayer)
                    {
                        pFL = (IFeatureLayer)layer;
                        if (pFL.FeatureClass != null)
                        {
                            pDS = (IDataset)pFL.FeatureClass;
                            pWS = pDS.Workspace;
                            pPropSet = pWS.ConnectionProperties;

                            object propertyNames;
                            object propertyValues;
                            pPropSet.GetAllProperties(out propertyNames, out propertyValues);
                            System.Array propNameArray = (System.Array)propertyNames;
                            System.Array propValuesArray = (System.Array)propertyValues;
                            string instance = "";
                            for (int i = 0; i < propNameArray.Length; i++)
                            {
                                Debug.Print("{0} = {1}", propNameArray.GetValue(i), propValuesArray.GetValue(i));
                                if (propNameArray.GetValue(i).ToString().ToUpper() == INSTANCE_PROP_NAME)
                                {
                                    instance = propValuesArray.GetValue(i).ToString();
                                    break;
                                }
                            }

                            if (instance != "")
                            {
                                string tnsName = "";
                                int posOfLastPeriod = -1;
                                posOfLastPeriod = instance.LastIndexOf("=");
                                if (posOfLastPeriod != -1)
                                {
                                    tnsName = instance.Substring(
                                        posOfLastPeriod + 1,
                                        (instance.Length - posOfLastPeriod) - 1);
                                    tnsName.Trim();
                                    if (!hshTNSEntries.ContainsKey(tnsName))
                                    {
                                        hshTNSEntries.Add(tnsName, 0);
                                    }
                                }
                            }
                        }
                    }
                }

                Hashtable hshTNSMsg = new Hashtable();
                Process p = new Process();

                foreach (string tnsName in hshTNSEntries.Keys)
                {
                    
                    // Redirect the output stream of the child process.
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.RedirectStandardOutput = true;
                    p.StartInfo.Arguments = " " + tnsName;
                    p.StartInfo.FileName = TNS_PING_COMMAND_NAME;
                    p.Start();
                    // Do not wait for the child process to exit before
                    // reading to the end of its redirected stream.
                    // p.WaitForExit();
                    // Read the output stream first and then wait.
                    string output = p.StandardOutput.ReadToEnd();
                    p.WaitForExit();

                    if (output.Contains(SERVICE_NAME_STRING))
                    {
                        string serviceNameAnd = "";
                        string serviceName = ""; 
                        int posOfServiceNameEquals = -1;
                        int posOfCloseBracket = -1;
                        posOfServiceNameEquals = output.LastIndexOf(SERVICE_NAME_STRING);
                        if (posOfServiceNameEquals != -1)
                        {
                            serviceNameAnd = output.Substring(
                                posOfServiceNameEquals + 1,
                                (output.Length - posOfServiceNameEquals) - 1);
                            serviceNameAnd.Trim();
                            if (serviceNameAnd.Contains(CLOSE_BRACKET_STRING))
                            {
                                posOfCloseBracket = serviceNameAnd.IndexOf(CLOSE_BRACKET_STRING);
                                if (posOfCloseBracket != -1)
                                {
                                    serviceName = serviceNameAnd.Substring(
                                        (SERVICE_NAME_STRING.Length - 1), 
                                        (posOfCloseBracket - (SERVICE_NAME_STRING.Length - 1)));
                                    serviceName.Trim(); 
                                }
                            }
                        }

                        if (serviceName != "")
                        {
                            hshTNSMsg.Add(tnsName, serviceName);   
                        }
                    }
                }

                string msg = "";
                string currentEntry = "";
                foreach (string key in hshTNSMsg.Keys)
                {
                    currentEntry =  
                        USER_MSG_TNS_ENTRY + key.ToString().ToUpper() + 
                        USER_MSG_MAPS_TO +  
                        USER_MSG_SPACE + hshTNSMsg[key].ToString();
                    if (msg == "")
                        msg = currentEntry;
                    else
                        msg = msg + Environment.NewLine + currentEntry; 
                }

                //Display message 
                MessageBox.Show(msg, "PGE TNSPing Command", MessageBoxButtons.OK, MessageBoxIcon.Information); 

            }
            catch (Exception ex)
            {
                MessageBox.Show("En error occurred in the TNSPing Tool: " + ex.Message); 
            }
        }

        #endregion
    }
}
