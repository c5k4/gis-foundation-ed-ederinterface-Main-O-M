using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.ArcMapUI;
using Miner.ComCategories;
using Miner.Interop;
using Miner.Interop.Process;
using PGE.Common.Delivery.ArcFM;
using PGE.Desktop.EDER.AutoUpdaters.Attribute;
using PGE.Common.Delivery.Diagnostics;
using System.Reflection;

namespace PGE.Desktop.EDER.D8TreeTools
{
    /// <summary>
    /// This tree view tool will display a form, with drop-down box that gives user the option
    /// to pick CNodeID from Substation FNM table.
    /// On picking the ID, and clicking OK, it will create the relationship between PNode feature and slected FNM object.
    /// This is the PG&E customized tool
    /// </summary>
    [Guid("406CE0BD-DBC8-4E1B-B510-F6885379E9FB")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComponentCategory(ComCategory.D8SelectionTreeTool)]
    public class PGE_PNodeUpdate2: IMMTreeViewTool
    {
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");


        #region Com Reg Methods

        /// <summary>
        /// Registers the specified reg key.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComRegisterFunction]
        private static void Register(string regKey)
        {
            Miner.ComCategories.D8SelectionTreeTool.Register(regKey);
        }
        /// <summary>
        /// Uns the register.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComUnregisterFunction]
        private static void UnRegister(string regKey)
        {
            Miner.ComCategories.D8SelectionTreeTool.Unregister(regKey);
        }

        #endregion

        public PGE_PNodeUpdate2()
        {
        }

        #region IMMTreeViewTool
        /// <summary>
        /// Use the same category as the OOTB abandon and remove tool
        /// </summary>
        public int Category
        {
            get { return 16; }
        }

        /// <summary>
        /// Launches the Update PNODEID form.
        /// </summary>
        /// <param name="enumItems"></param>
        /// <param name="itemCount"></param>
        public void Execute(IMMTreeViewSelection pSelection)
        {
            IWorkspaceEdit wkEdit = null;
            bool hasEditOperation = false;
            try
            {
                //Display a form, with drop-down box that gives user the option to pick CNodeID from Substation FNM table.
                ID8EnumListItem enumListItems = pSelection as ID8EnumListItem;
                enumListItems.Reset();

                ID8ListItem selectedItem = enumListItems.Next();
                ID8GeoAssoc geoAssoc = (ID8GeoAssoc)selectedItem;
                IRow featPNode = geoAssoc.AssociatedGeoRow as IRow;
                PNodeInitialize.Workspace = (IWorkspace)((IDataset)((IRow)featPNode).Table).Workspace;

                //Display IDs form.
                PNodeIDDisplayForm displayform = new PNodeIDDisplayForm();
                displayform.ShowDialog();
                if (string.IsNullOrEmpty(PNodeInitialize.PNodeIDValue))
                    return;

                if (string.IsNullOrEmpty(PNodeInitialize.FnmGUIDValue))
                {
                    MessageBox.Show("Failed to get FNM GUID.",
                        "NODE ID",
                        System.Windows.Forms.MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                wkEdit = (IWorkspaceEdit)PNodeInitialize.Workspace;
                if ((wkEdit == null) || !(wkEdit.IsBeingEdited()))
                    throw new Exception("Session not in Edit.");
                wkEdit.StartEditOperation();
                hasEditOperation = true;

                //Set CNODEID and FNM-Guid values.
                Int32 fnmOID = GetOID(PNodeInitialize.FnmGUIDValue, PNodeInitialize.FNMTable);
                PNodeInitialize.CreatePNodeFNMRelationship(featPNode.OID, fnmOID, PNodeInitialize.Workspace);
                //PNodeInitialize.SetFldVl(featPNode as IRow, PNodeInitialize.FnmGUIDValue, PNodeInitialize.PNodeFNMGUIDFldIdx);
                PNodeInitialize.SetFldVl(featPNode as IRow, PNodeInitialize.PNodeIDValue, PNodeInitialize.PNodeCNODEIDFldIdx);
                featPNode.Store();
                //Stop the edit operation 
                wkEdit.StopEditOperation();
                hasEditOperation = false;

                //Refresh map and ArcFM attribute editor selection tab
                //AppRef is a singleton
                IApplication application = GetArcMapApplication();

                //This is executed in ArcMap.  No need to surpress the message
                if (application == null)
                {
                    throw new Exception("Failed to get access to application.");
                }

                //Refresh the active view.
                IMxDocument mxDoc = application.Document as IMxDocument;
                ESRI.ArcGIS.Carto.IActiveView activeView = mxDoc.ActiveView;
                ((ESRI.ArcGIS.Carto.ISelectionEvents)activeView).SelectionChanged();
                activeView.Refresh();
            }
            catch (Exception Ex)
            {
                if (hasEditOperation)
                    wkEdit.AbortEditOperation();
                PNodeInitialize.ErrorMessage(Ex, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            }

        }

        /// <summary>
        /// Set name 
        /// </summary>
        public string Name
        {
            get { 
                return "PGE - Display PNode-IDs"; }
        }

        /// <summary>
        /// Set the priority 
        /// </summary>
        public int Priority
        {
            get { return 1; }
        }

        /// <summary>
        /// Returns True if a PNODE Record is selected
        /// </summary>
        /// <param name="enumItems"></param>
        /// <param name="itemCount"></param>
        /// <returns></returns>
        public int get_Enabled(IMMTreeViewSelection pSelection)
        {
            mmToolState result = mmToolState.mmTSNone;
            try
            {
                ID8EnumListItem enumListItems = pSelection as ID8EnumListItem;
                enumListItems.Reset();
                ID8ListItem pLI = enumListItems.Next();

                //enumItems.Reset();

                //Do not enable if multiple items are selected in selection Tab
                //if (itemCount != 1)
                //{
                //    return result;
                //}

                //Do not enable if nothing is selected
                //ID8ListItem pLI = enumItems.Next();
                if (pLI == null)
                {
                    return (int)result;
                }
                if (!(pLI is IMMProposedObject))
                {
                    return (int)mmToolState.mmTSVisible;
                }
                //Disable if selected item is not a Feature
                if (pLI.ItemType != mmd8ItemType.mmd8itFeature)
                {
                    return (int)mmToolState.mmTSVisible;
                }
                if (pLI.ItemType == mmd8ItemType.mmd8itFeature)
                {
                    //***
                    ID8GeoAssoc geoAssoc = pLI as ID8GeoAssoc;
                    if (geoAssoc.AssociatedGeoRow is IFeature)
                    {
                        IFeature feature = geoAssoc.AssociatedGeoRow as IFeature;
                        IObjectClass cls = feature.Class as IObjectClass;
                        IDataset _dataSet = cls as IDataset;

                        IWorkspace workSpace = _dataSet.Workspace;
                        IWorkspaceEdit editWk = (IWorkspaceEdit)workSpace;
                        PNodeInitialize.Workspace = workSpace;
                        if (!editWk.IsBeingEdited())
                            return (int)mmToolState.mmTSVisible;

                        //IDataset dataSet = pTable as IDataset;
                        //Ensure selected item is 'PNODE'
                        if (_dataSet.Name.Equals(PNodeInitialize.PNODE_FC_NAME, StringComparison.OrdinalIgnoreCase))
                            result = ((mmToolState.mmTSVisible) | (mmToolState.mmTSEnabled));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("ERROR - ", ex);
                //ignore the error.
            }
            return (int)result;
        }
        #endregion

        #region Private Members

        private IApplication GetArcMapApplication()
        {
            Type t = Type.GetTypeFromCLSID(typeof(AppRefClass).GUID);
            System.Object obj = Activator.CreateInstance(t);
            IApplication app = obj as IApplication;
            return app;
        }
        private Int32 GetOID(string GUIDValue, ITable tableSearch)
        {
            ICursor curSearch = null;
            try
            {
                string sqlDate = @"GLOBALID = '" + GUIDValue + "'";
                curSearch = null; 
                PNodeInitialize.GetCursor(out curSearch, tableSearch, sqlDate);

                IRow FNMRow = curSearch.NextRow();
                if (FNMRow != null)
                    return FNMRow.OID;
            }
            catch (Exception Ex)
            {
                PNodeInitialize.ErrorMessage(Ex, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            }
            finally
            {
                if (curSearch != null)
                {
                    while (Marshal.ReleaseComObject(curSearch) > 0) { }
                }
            }
            return -1;
        }

        #endregion
               
    }
}
