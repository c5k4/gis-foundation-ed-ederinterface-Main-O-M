using System;
using System.Drawing;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Controls;
using System.Windows.Forms;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;
using System.Collections.Generic;
using ESRI.ArcGIS.esriSystem;
using System.Reflection;
using PGE.Common.Delivery.Diagnostics;
using System.Diagnostics;
using System.Collections;
using Miner.ComCategories;



namespace PGE.Desktop.EDER
{
    /// <summary>
    /// Command that works in ArcMap/Map/PageLayout
    /// </summary>
    /// This s a Button -  For selecting edited features in the session

    [Guid("95be5cfb-1635-497f-8f0d-2cd19ad05077")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("PGE.Desktop.EDER.SelectEdits")]
    [ComponentCategory(ComCategory.ArcMapCommands)]
    public sealed class SelectEdits : BaseCommand
    {

        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
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
            MxCommands.Register(regKey);
            ControlsCommands.Register(regKey);
        }
        /// <summary>
        /// Required method for ArcGIS Component Category unregistration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryUnregistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            MxCommands.Unregister(regKey);
            ControlsCommands.Unregister(regKey);
        }

        #endregion
        #endregion
        private IApplication m_application;// this line added by Infy team 
        private IHookHelper m_hookHelper = null;
        public SelectEdits()
        {
            //
            // TODO: Define values for the public properties
            //
            base.m_category = "PGE Tools"; //localizable text
            base.m_caption = "Session Edits Select";  //localizable text 
            base.m_message = "";  //localizable text
            base.m_toolTip = "Selection for new & edit features in the session";  //localizable text
            base.m_name = "PG&E Session Edits Select";   //unique id, non-localizable (e.g. "MyCategory_MyCommand")

            try
            {
                //
                // TODO: change bitmap name if necessary
                //
                //string bitmapResourceName = GetType().Name + ".bmp";
                //base.m_bitmap = new Bitmap(GetType(), bitmapResourceName);
                base.m_bitmap = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("PGE.Desktop.EDER.Bitmaps.SelectEdits.bmp"));
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

            try
            {
                m_hookHelper = new HookHelperClass();
                m_hookHelper.Hook = hook;
                m_application = (IApplication)hook;// this line added by infosys team 
                if (m_hookHelper.ActiveView == null)
                    m_hookHelper = null;
            }
            catch
            {
                m_hookHelper = null;
            }

            if (m_hookHelper == null)
                base.m_enabled = false;
            else
                base.m_enabled = true;

            // TODO:  Add other initialization code
        }

        /// <summary>
        /// Occurs when this command is clicked
        /// </summary>
        /// 
        IMxDocument _pmxDocument;
        IMap _map;
        // Global Varibles
        int _editCount = 0;

        public override void OnClick()
        {
            try
            {
                // this block of code added by infosys team 
                System.Windows.Forms.Cursor.Current = Cursors.WaitCursor;
                _editCount = 0;
                _pmxDocument = (IMxDocument)m_application.Document;
                _map = _pmxDocument.FocusMap;
                if (_map.FeatureSelection != null)
                {
                     _map.ClearSelection();
                }
                // 
                GetLayersAttached();
                // to check edit count 
                if (_editCount == 0)
                {
                    //MessageBox.Show("No Edits Done");
                }
                else
                {
                    _pmxDocument.ActiveView.Refresh();
                }
                System.Windows.Forms.Cursor.Current = Cursors.Default;
                m_application.StatusBar.set_Message(0, "Selection finished.");
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                _logger.Debug("Message From SessionChecker -" + ex.Message + " StackTrace:" + ex.StackTrace);
            }
        }
        public void GetLayersAttached()
        {

            if (_map.LayerCount != 0)
            {

                GetFcs(_map);

            }

        }
        private void GetFcs(IMap map)
        {
            if (map != null)
            {
                //IEnumLayer enumLayer = map.Layers;
                //ILayer layer = null;
                //while ((layer = enumLayer.Next()) != null)
                //{
                //    if (!layer.Valid) continue;
                //    //Add the reference of this layer and layer name to the List
                //    GetFcsFromGroup(map, layer);
                //}
                UID id = new UIDClass();
                id.Value = "{E156D7E5-22AF-11D3-9F99-00C04F6BC78E}"; //IGeoFeatureLayer

                IEnumLayer enumLayer = map.get_Layers(id, true);
                ILayer layer = enumLayer.Next();
                while (layer != null)
                {
                    IFeatureLayer fLayer = layer as IFeatureLayer;
                    if (fLayer != null)
                    {
                        IFeatureClass fClass = fLayer.FeatureClass as IFeatureClass;
                        if (fClass != null)
                        {
                            //Run with layer
                            Hashtable OIDs = VersionDifferences(fLayer);
                            if (OIDs != null)
                            {
                                SelectFeatures(fLayer, OIDs);
                            }
                        }
                        ReleaseCOMObject(fClass);
                        ReleaseCOMObject(fLayer);
                    }
                    layer = enumLayer.Next();
                }
                ReleaseCOMObject(layer);
            }
        }

        private void SelectFeatures(IFeatureLayer fLayer, Hashtable OIDs)
        {
            try
            {
                //If there is a layer definition we skip selection on this layer
                IFeatureLayerDefinition pFDef = (IFeatureLayerDefinition)fLayer;

                IFeatureSelection fSel = (IFeatureSelection)fLayer;

                IDataset pDS = (IDataset)fLayer;

                //Now loop through the OIDs that we have gathered up
                int cnt = 0;
                foreach (int oid in OIDs.Keys)
                {
                    IFeature testFeature = fLayer.FeatureClass.GetFeature(oid);
                    if (testFeature != null)
                    {

                        cnt++;
                        //This will select the features in their respective layers
                        fSel.SelectionSet.Add(testFeature.OID);

                        //This will select the features in their respective layers b
                        //This fires an event so that the M&M attribute editor knows about the selection
                        _map.SelectFeature(fLayer, testFeature);

                        fSel.SelectionChanged();
                        //It seems to be faster to add all the features to the SelectionSet and then run the 
                        //layer.SelectFeature() once per layer 
                        //I'm doing it this way for now just to cut back on code while we still have a selection error
                    }
                }

                //If there is a layer definition we requery our selected set to match the definition 
                if (pFDef.DefinitionExpression != string.Empty)
                {
                    IQueryFilter pQF2 = new QueryFilterClass();
                    pQF2.WhereClause = pFDef.DefinitionExpression;
                    fSel.SelectFeatures(pQF2, esriSelectionResultEnum.esriSelectionResultAnd, false);
                }
            }
            catch (Exception ex)
            {
                _logger.Debug("SelectEditFeatures:VersionDifferences" + (ex.Message + ("::" + ex.StackTrace)));
            }
        }

        /// <summary>
        /// Helper Method to return version differences
        /// </summary>
        /// <param name="fLayer"></param>
        /// <returns></returns>
        private Hashtable VersionDifferences(IFeatureLayer fLayer)
        {
            try
            {

                IFeatureSelection fSel = fLayer as IFeatureSelection;

                Hashtable OIDs = new Hashtable();

                IDataset pDS = fLayer as IDataset;

                IWorkspace pWorkspace = pDS.Workspace;

                if (!(pWorkspace is IVersionedWorkspace)) return null;

                IVersionedWorkspace pVerWorkspace = pWorkspace as IVersionedWorkspace;

                IFeatureWorkspaceManage fWM = pVerWorkspace as IFeatureWorkspaceManage;
                IDatasetName dsName = pDS.FullName as IDatasetName;
                IName pname = dsName as IName;
                if (fWM.IsRegisteredAsVersioned(pname) == false)
                {
                    return null;
                }

                IVersion pVersion1 = (IVersion)pVerWorkspace;
                if (pVersion1.HasParent() == false)
                {
                    return null;
                }

                IVersion pVersion2 = (IVersion)pVerWorkspace.FindVersion(pVersion1.VersionInfo.Parent.VersionName);
                IFeatureWorkspace pFeatWork1 = (IFeatureWorkspace)pVersion1;
                IFeatureWorkspace pFeatWork2 = (IFeatureWorkspace)pVersion2;
                ITable pTable1 = (ITable)pFeatWork1.OpenFeatureClass(pDS.BrowseName);
                ITable pTable2 = (ITable)pFeatWork2.OpenFeatureClass(pDS.BrowseName);
                IVersionedTable pVerTable1 = (IVersionedTable)pTable1;
                IVersionedTable pVerTable2 = (IVersionedTable)pTable2;

                m_application.StatusBar.set_Message(0, "Searching " + pDS.Name + " for inserts and updates.");

                IQueryFilter pQF = new QueryFilterClass();
                pQF.SubFields = fLayer.FeatureClass.OIDFieldName;

                IDifferenceCursor pDiffCursor1 = pVerTable1.Differences(pTable2, esriDifferenceType.esriDifferenceTypeInsert, pQF);
                IRow differenceRow;
                int objectID;
                pDiffCursor1.Next(out objectID, out differenceRow);

                while (objectID != -1)
                {
                    if (OIDs.Contains(objectID) == false)
                    {
                        OIDs.Add(objectID, objectID);
                    }
                    pDiffCursor1.Next(out objectID, out differenceRow);
                }
                ReleaseCOMObject(pDiffCursor1);

                IDifferenceCursor pDiffCursor2 = pVerTable1.Differences(pTable2, esriDifferenceType.esriDifferenceTypeUpdateNoChange, pQF);
                pDiffCursor2.Next(out objectID, out differenceRow);
                while (objectID != -1)
                {
                    if (OIDs.Contains(objectID) == false)
                    {
                        OIDs.Add(objectID, objectID);
                    }
                    pDiffCursor2.Next(out objectID, out differenceRow);
                }
                ReleaseCOMObject(pDiffCursor2);

                IDifferenceCursor pDiffCursor3 = pVerTable1.Differences(pTable2, esriDifferenceType.esriDifferenceTypeUpdateDelete, pQF);
                pDiffCursor3.Next(out objectID, out differenceRow);
                while (objectID != -1)
                {
                    if (OIDs.Contains(objectID) == false)
                    {
                        OIDs.Add(objectID, objectID);
                    }
                    pDiffCursor3.Next(out objectID, out differenceRow);
                }
                ReleaseCOMObject(pDiffCursor3);

                IDifferenceCursor pDiffCursor4 = pVerTable1.Differences(pTable2, esriDifferenceType.esriDifferenceTypeUpdateUpdate, pQF);
                pDiffCursor4.Next(out objectID, out differenceRow);
                while (objectID != -1)
                {
                    if (OIDs.Contains(objectID) == false)
                    {
                        OIDs.Add(objectID, objectID);
                    }
                    pDiffCursor4.Next(out objectID, out differenceRow);
                }
                ReleaseCOMObject(pDiffCursor4);

                pVerTable2 = null;
                ReleaseCOMObject(pVerTable2);
                pVerTable1 = null;
                ReleaseCOMObject(pVerTable1);
                pTable2 = null;
                ReleaseCOMObject(pTable2);
                pTable1 = null;
                ReleaseCOMObject(pTable1);
                pFeatWork2 = null;
                ReleaseCOMObject(pFeatWork2);
                pFeatWork1 = null;
                ReleaseCOMObject(pFeatWork1);
                pVersion2 = null;
                ReleaseCOMObject(pVersion2);
                pVersion1 = null;
                ReleaseCOMObject(pVersion1);
                pVerWorkspace = null;
                ReleaseCOMObject(pVerWorkspace);

                if (OIDs.Count > 0)
                {
                    return OIDs;
                }

                return null;

            }
            catch (Exception ex)
            {
                _logger.Debug("SelectEditFeatures:VersionDifferences" + (ex.Message + ("::" + ex.StackTrace)));
                return null;
            }
            finally
            {
                GC.Collect();
            }
        }

       
        #endregion

        private void ReleaseCOMObject(object obj)
        {
            try
            {
                if (obj != null)
                {
                    int refsLeft = 0;
                    do
                    {
                        refsLeft = Marshal.ReleaseComObject(obj);
                    }
                    while (refsLeft > 0);
                }
            }
            catch (Exception ex)
            {

                //MessageBox.Show(ex.Message);
                //_logger.Debug("Message From SessionChecker -" + ex.Message + " StackTrace:" + ex.StackTrace);
            }
        }
    }
}
