using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using PGE.Common.Delivery.Process.BaseClasses;
using Miner.Interop.Process;
using System.Runtime.InteropServices;
using Miner.ComCategories;
using Miner.Interop;
using PGE.Common.Delivery.ArcFM;
using Miner.Geodatabase.Edit;
using System.Windows.Forms;
using ESRI.ArcGIS.esriSystem;
using log4net;
using System.Reflection;
using PGE.Common.Delivery.Diagnostics;
using PGE.Common.Delivery.Framework;
using System.Collections;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.GeoDatabaseDistributed;
using PGE.Desktop.EDER.ValidationRules;

namespace PGE.Desktop.EDER.Subtasks
{
    [ComVisible(true)]
    [Guid("d5253421-d52e-4f09-8f2b-03c0a12525ce")]
    [ProgId("PGE.Desktop.EDER.DeleteOrphanRows")]
    [ComponentCategory(ComCategory.MMPxSubtasks)]
    public class DeleteOrphanRows : BasePxSubtask
    {
        private IApplication _app;
        private IMMSessionManager3 _sessionMgrExt;

        /// <summary>
        /// Constructor
        /// </summary>
        public DeleteOrphanRows()
            : base("PGE Delete Orphan Rows")
        {

        }

        //ArcMap application handle 
        protected IApplication Application
        {
            get
            {
                if (_app == null) _app = ApplicationFacade.Application;
                return _app;
            }
        }

        /// <summary>
        /// Initializes the subtask
        /// </summary>
        /// <param name="pPxApp">The IMMPxApplication.</param>
        //// <returns><c>true</c> if intialized; otherwise <c>false</c>.</returns>
        public override bool Initialize(IMMPxApplication pPxApp)
        {
            base.Initialize(pPxApp);

            //Check application and session manager
            if (base._PxApp == null) return false;

            _sessionMgrExt = _PxApp.FindPxExtensionByName(BasePxSubtask.SessionManagerExt) as IMMSessionManager3;
            if (_sessionMgrExt == null) return false;

            return true;
        }

        /// <summary>
        /// Gets if the subtask is enabled for the specified px node.
        /// </summary>
        /// <param name="pPxNode">The px node.</param>
        /// <returns><c>true</c> if enabled; otherwise <c>false</c>.</returns>
        protected override bool InternalEnabled(IMMPxNode pPxNode)
        {
            return base.InternalEnabled(pPxNode);
        }

        /// <summary>
        /// Executes the Actual QA by obtaining the Version difference from the DesktopVersionDifference Processor.
        /// </summary>
        /// <param name="pPxNode">Session Node</param>
        /// <returns></returns>
        protected override bool InternalExecute(IMMPxNode pPxNode)
        {
            bool retVal = false;
            try
            {
                //Delete the orphan rows 
                retVal = DeleteRows();

                //Return true for the task 
                retVal = true;
            }
            catch (Exception ex)
            {
                _logger.Error("Failed processing the subtask:" + this._Name, ex);
            }
            finally
            {
                //if required 
            }
            return retVal;
        }

        /// <summary>
        /// Confirms if eahc row is in fact an orphan and if so 
        /// deletes the row 
        /// </summary>
        /// <param name="hshDeletes"></param>
        private bool DeleteRows()
        {
            bool retVal = false;
            IWorkspaceEdit pWSE = null;
            bool hasEditOperation = false;

            try
            {

                //Start edit operation 

                pWSE = (IWorkspaceEdit)Editor.EditWorkspace;
                if ((pWSE == null) || !(pWSE.IsBeingEdited()))
                    return false;
                pWSE.StartEditOperation();
                hasEditOperation = true;

                //Open the PGE_ORPHAN_ROWS table 
                ITable pOrphanRowsTable = (ITable)ModelNameFacade.ObjectClassByModelName(Editor.EditWorkspace,
                    SchemaInfo.Electric.ClassModelNames.OrphanRows);
                IVersion pVersion = (IVersion)Editor.EditWorkspace;
                IQueryFilter pQF = new QueryFilterClass();
                pQF.WhereClause = "VERSION_NAME" + " = " + "'" + pVersion.VersionName.ToUpper() + "'";
                ICursor pCursor = pOrphanRowsTable.Search(pQF, false);
                Hashtable hshOrphanRows = new Hashtable();
                Hashtable hshOids;

                //Get column indexes 
                int fldIdxDateTime = pOrphanRowsTable.Fields.FindField("DATE_TIME");
                int fldIdxVersionName = pOrphanRowsTable.Fields.FindField("VERSION_NAME");
                int fldIdxDatasetName = pOrphanRowsTable.Fields.FindField("DATASET_NAME");
                int fldIdxOrphanRowOId = pOrphanRowsTable.Fields.FindField("ORPHAN_OID");
                string datasetName = "";
                int orphanOId = -1;

                IRow pRow = pCursor.NextRow();
                while (pRow != null)
                {
                    datasetName = string.Empty;
                    orphanOId = -1;

                    if (pRow.get_Value(fldIdxDatasetName) != DBNull.Value)
                        datasetName = pRow.get_Value(fldIdxDatasetName).ToString();
                    if (pRow.get_Value(fldIdxOrphanRowOId) != DBNull.Value)
                        orphanOId = Convert.ToInt32(pRow.get_Value(fldIdxOrphanRowOId));

                    if ((datasetName != string.Empty) && (orphanOId != -1))
                    {
                        if (!hshOrphanRows.ContainsKey(datasetName))
                            hshOrphanRows.Add(datasetName, new Hashtable());
                        hshOids = (Hashtable)hshOrphanRows[datasetName];
                        if (!hshOids.ContainsKey(orphanOId))
                        {
                            hshOids.Add(orphanOId, 0);
                        }
                        hshOrphanRows[datasetName] = hshOids;
                    }
                    pRow = pCursor.NextRow();
                }
                Marshal.FinalReleaseComObject(pCursor);

                //Check the row is still an orphan and if so delete it 
                IObjectClass pTargetClass = null;
                IObject pTargetObject = null;
                bool targetObjectisOrphan = false;
                IFeatureWorkspace pFWS = (IFeatureWorkspace)Editor.EditWorkspace;

                foreach (string dataset in hshOrphanRows.Keys)
                {
                    hshOids = (Hashtable)hshOrphanRows[dataset];
                    pTargetClass = (IObjectClass)pFWS.OpenTable(dataset);
                    IEnumRelationshipClass pEnumRelClass = pTargetClass.get_RelationshipClasses(esriRelRole.esriRelRoleAny);

                    foreach (int oid in hshOids.Keys)
                    {
                        //Look for that object - remembering it may have been deleted 
                        pTargetObject = GetObjectByOId((ITable)pTargetClass, oid);
                        targetObjectisOrphan = true;
                        if (pTargetObject == null)
                            continue;

                        pEnumRelClass.Reset();
                        IRelationshipClass pRelClass = pEnumRelClass.Next();
                        while (pRelClass != null)
                        {
                            if (pTargetObject != null)
                            {
                                ESRI.ArcGIS.esriSystem.ISet relatedSet = pRelClass.GetObjectsRelatedToObject(pTargetObject);
                                relatedSet.Reset();
                                if (relatedSet.Next() != null)
                                {
                                    //We have 1 or more related objects for the target 
                                    //object so it must not be an orphan 
                                    targetObjectisOrphan = false;
                                }
                                Marshal.FinalReleaseComObject(relatedSet);
                            }
                            pRelClass = pEnumRelClass.Next();
                        }
                        //Marshal.FinalReleaseComObject(pEnumRelClass);

                        //Delete it if it is an orphan
                        if (targetObjectisOrphan)
                            pTargetObject.Delete();
                    }
                    Marshal.FinalReleaseComObject(pEnumRelClass);
                }

                //Clean out this table 
                try
                {
                    ICursor pDelCursor = pOrphanRowsTable.Search(null, false);
                    IRow pDelRow = null;
                    try
                    {
                        while ((pDelRow = pDelCursor.NextRow()) != null)
                        {
                            pDelRow.Delete();
                        }
                    }
                    catch (Exception ex)
                    {
                        // Handle any errors that might occur on NextFeature().
                        _logger.Debug("DeleteRows routine failed with error: " + ex.Message);
                    }
                    Marshal.FinalReleaseComObject(pDelCursor);
                }
                catch
                {
                    //Do nothing 
                }

                //Stop the edit operation 
                pWSE.StopEditOperation();
                hasEditOperation = false;
                retVal = true;
            }
            catch (Exception ex)
            {
                if (hasEditOperation)
                    pWSE.AbortEditOperation();
                _logger.Debug("Entering error handler for DeleteRows with error: " + ex.Message);
                throw new Exception("Error deleting orphan rows");
            }
            return retVal;
        }

        private IObject GetObjectByOId(ITable pTable, int OId)
        {
            IObject pObj = null;
            try
            {
                IQueryFilter pQF = new QueryFilterClass();
                pQF.WhereClause = ((IObjectClass)pTable).OIDFieldName + " = " + OId.ToString();
                ICursor pCursor = pTable.Search(pQF, false);
                IRow pRow = pCursor.NextRow();
                if (pRow != null)
                {
                    pObj = (IObject)pRow;
                }
                Marshal.FinalReleaseComObject(pCursor);
                return pObj;
            }
            catch
            {
                return null;
            }
        }

    }
}

