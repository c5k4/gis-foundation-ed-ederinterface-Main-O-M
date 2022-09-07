using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;
using Miner.ComCategories;
using PGE.Common.Delivery.Diagnostics;
using PGE.Desktop.EDER.GDBM;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;

namespace PGE.Desktop.EDER.ArcMapCommands.PGE_Toolbar
{
    /// <summary>
    /// Summary description for PGE_DisconnectMultipleFeature_Tool.
    /// </summary>
    [Guid("1dd0819b-808f-4dd9-9e6e-5fe72a5f7895")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("PGE.Desktop.EDER.ArcMapCommands.PGE_Toolbar.PGE_DisconnectMultipleFeature_Tool")]
    [ComponentCategory(ComCategory.ArcMapCommands)]
    public sealed class PGE_DisconnectMultipleFeature_Tool : BaseCommand
    {
        #region COM Registration Function(s)
        [ComRegisterFunction()]
        [ComVisible(false)]
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
            MxCommandBars.Register(regKey);
        }
        /// <summary>
        /// Required method for ArcGIS Component Category unregistration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryUnregistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            MxCommandBars.Unregister(regKey);
        }

        #endregion
        #endregion

        private IApplication m_application;
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        public PGE_DisconnectMultipleFeature_Tool()
        {
            //
            // TODO: Define values for the public properties
            //
            base.m_category = "PGE Tools"; //localizable text
            base.m_caption = "PGE Disconnect Multiple Feature Tool";  //localizable text
            base.m_message = "PGE Disconnect Multiple Feature Tool";  //localizable text 
            base.m_toolTip = "PGE Disconnect Multiple Feature Tool";  //localizable text 
            base.m_name = "PGEDisconnecMultipleFeatureTool";   //unique id, non-localizable (e.g. "MyCategory_ArcMapCommand")


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
            if (hook == null)
                return;

            m_application = hook as IApplication;

            //Disable if it is not ArcMap
            if (hook is IMxApplication)
                base.m_enabled = true;
            else
                base.m_enabled = false;

            // TODO:  Add other initialization code
        }


        /// <summary>
        /// 
        /// </summary>
        public override bool Enabled
        {
            get
            {

                IMxDocument pdoc = m_application.Document as IMxDocument;
                IMap pMap = pdoc.FocusMap;
                if (pMap.SelectionCount == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        /// <summary>
        /// Occurs when this command is clicked
        /// </summary>
        public override void OnClick()
        {
            IStatusBar stbar = m_application.StatusBar;
            IMxApplication2 mxApp = (IMxApplication2)m_application;
            mxApp.PauseDrawing = true;
            try
            {

                stbar.Message[0] = "Disconnect Feature Task is in Progress.";
                IMxDocument pdoc = m_application.Document as IMxDocument;
                IMap pMap = pdoc.FocusMap;
                int count = 0;
                if ((new CommonFunctions()).CheckNetworkFeature((m_application.Document as IMxDocument).FocusMap, out count))
                {
                    IEnumFeature enumFeat = pMap.FeatureSelection as IEnumFeature;
                    IEnumFeatureSetup enumSetup = (IEnumFeatureSetup)enumFeat;
                    enumSetup.AllFields = true;
                    enumFeat.Reset();
                    IFeature Feat = null;
                     IDictionary<long, IFeature> dicIds = new Dictionary<long, IFeature>();

                    while ((Feat = enumFeat.Next()) != null)
                    {

                        if (Feat.Class is IFeatureClass)
                        {
                            if (!dicIds.ContainsKey(Feat.OID))
                            {
                                dicIds.Add(Feat.OID, Feat);
                            }
                        }
                    }

                    int dsconenctcount=0;
                    IFeatureClass Pfeatclass = null;
                    IFeature pFeature = null;
                    
                    foreach (int skey in dicIds.Keys)
                    {
                        pMap.ClearSelection();
                        pFeature = null;
                        dicIds.TryGetValue(skey, out pFeature);
                        Pfeatclass = pFeature.Class as IFeatureClass;
                        IFeatureLayer player = GetFeatureLayer(pMap,Pfeatclass.AliasName);
                        if (player != null)
                        {
                           
                            pMap.SelectFeature(player, pFeature);
                            if (pMap.SelectionCount == 1)
                            {
                                dsconenctcount = dsconenctcount + 1;
                                DisConnectfeature();
                            }
                        }

                    }
                    MessageBox.Show("Feature Disconnected Successfully--" + dsconenctcount);
                    enumFeat.Reset();
                    foreach (int skey in dicIds.Keys)
                    {

                        pFeature = null;
                        dicIds.TryGetValue(skey, out pFeature);
                        Pfeatclass = pFeature.Class as IFeatureClass;
                        IFeatureLayer player = GetFeatureLayer(pMap, Pfeatclass.AliasName);
                        if (player != null)
                        {

                            pMap.SelectFeature(player, pFeature);
                            
                        }

                    }

                    pdoc.ActiveView.Refresh();
                    
                }
                else
                {
                    MessageBox.Show("Please select valid network features.", "PGE Multiple Disconnect Tool",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show("Stopped Process as exception occurred.");
                _logger.Error(ex.Message, ex);
            }
            finally
            {
                stbar.Message[0] = "Disconnect feature task has been completed.";
                stbar.Message[0] = "";
                //do your process here
                if (mxApp != null)
                {
                    mxApp.PauseDrawing = false;
                    m_application.RefreshWindow();

                }
            }
        }

        #endregion

        #region Internal Methods
        /// <summary>
        /// Returns the layer for the supplied feature class
        /// </summary>
        /// <param name="map"></param>
        /// <param name="featureClass"></param>
        /// <returns></returns>
        private IFeatureLayer GetFeatureLayer(IMap map, string sname)
        {
            // Log entry
            string name = MethodInfo.GetCurrentMethod().Name;
            _logger.Debug("Entered " + name);

            IFeatureLayer featLayer = null;
            IEnumLayer enumLayer = null;

            try
            {
                // Get all layers in the map
                UID uid = new UID();
                uid.Value = "{40A9E885-5533-11d0-98BE-00805F7CED21}";
                enumLayer = map.get_Layers(uid, true);

                // For each layer
                ILayer layer;
                while ((layer = enumLayer.Next()) != null)
                {
                    // Check to see if the layer is for our feature class
                    if(layer is IFeatureLayer)
                    {

                        IFeatureClass pclass = (layer as IFeatureLayer).FeatureClass as IFeatureClass;
                        if (pclass != null)
                        {
                            if (pclass.AliasName == sname)
                            {
                                // If it is, save it and bail out
                                featLayer = layer as IFeatureLayer;
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // Release object
                if (enumLayer != null)
                {
                    Marshal.ReleaseComObject(enumLayer);
                }
            }

            return featLayer;
        }
        public void DisConnectfeature()
        {
            try
            {

                ESRI.ArcGIS.esriSystem.UID printCommandUID = new ESRI.ArcGIS.esriSystem.UIDClass();
                printCommandUID.Value = "{9ED89E80-EE8F-41eb-B524-EEBC7B781070}";
                // printCommandUID.SubType = ;
                ICommandItem ArcFMDisconnect = m_application.Document.CommandBars.Find(printCommandUID, false, false);
                if (ArcFMDisconnect == null)
                {
                    _logger.Debug("Failed to find ICommand with UID = {9ED89E80-EE8F-41eb-B524-EEBC7B781070} and Subtype=7");
                }
                else ArcFMDisconnect.Execute();

            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        #endregion
    }
}
