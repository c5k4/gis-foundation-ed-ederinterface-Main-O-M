using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Miner.ComCategories;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Diagnostics;
using System.Reflection;
using PGE.Common.Delivery.Framework;
using ESRI.ArcGIS.Geodatabase;
using Miner;
using Miner.Interop;
using Miner.Geodatabase.Edit;
using ESRI.ArcGIS.Framework;

namespace PGE.Desktop.EDER.AutoUpdaters
{
    [ComVisible(true)]
    [Guid("255ee436-3f9e-46b3-ad5d-c402a6697bd3")]
    [ProgId("PGE.Desktop.EDER.CleanupOrphanRows")]
    [ComponentCategory(ComCategory.RelationshipAutoupdateStrategy)]
    public class CleanupOrphanRows : BaseRelationshipAU
    {

        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        public CleanupOrphanRows()
            : base("PGE Cleanup Orphan Rows")
        {

        }

        protected override void InternalExecute(IRelationship relationship, mmAutoUpdaterMode auMode, mmEditEvent eEvent)
        {
            try
            {

                IObjectClass pTargetClass = null;
                IObject pTargetObject = null;
                IObject pOtherObject = null;

                BaseRelationshipAU.IsRunningAsRelAU = true;
                if (eEvent == mmEditEvent.mmEventRelationshipDeleted)
                {
                    //If the other object has no related objects apart from 
                    //the one being removed then it can be deleted
                    bool targetObjectisOrphan = true;
                    if (ModelNameFacade.ContainsClassModelName(
                        relationship.OriginObject.Class,
                        new string[] { SchemaInfo.Electric.ClassModelNames.OrphanCleanup }))
                    {
                        pTargetClass = relationship.OriginObject.Class;
                        pTargetObject = relationship.OriginObject;
                        pOtherObject = relationship.DestinationObject;
                    }
                    else if (ModelNameFacade.ContainsClassModelName(
                        relationship.DestinationObject.Class,
                        new string[] { SchemaInfo.Electric.ClassModelNames.OrphanCleanup }))
                    {
                        pTargetClass = relationship.DestinationObject.Class;
                        pTargetObject = relationship.DestinationObject;
                        pOtherObject = relationship.OriginObject;
                    }
                    if (pTargetClass == null)
                        throw new Exception("Error in CleanupOrphanRecords one side of relationship must have modelname: " +
                            SchemaInfo.Electric.ClassModelNames.OrphanCleanup);


                    IEnumRelationshipClass pEnumRelClass = pTargetClass.get_RelationshipClasses(esriRelRole.esriRelRoleAny);
                    IRelationshipClass pRelClass = pEnumRelClass.Next();
                    while (pRelClass != null)
                    {

                        ESRI.ArcGIS.esriSystem.ISet relatedSet = pRelClass.GetObjectsRelatedToObject(pTargetObject);
                        relatedSet.Reset();
                        IRow relatedRow = (IRow)relatedSet.Next();

                        while (relatedRow != null)
                        {
                            if (relatedRow.OID != pOtherObject.OID)
                            {
                                //We have found another related object for the target 
                                //so it must not be an orphan 
                                _logger.Debug(((IDataset)pTargetObject.Class).BrowseName +
                                    pTargetObject.OID.ToString() +
                                    " is not an orphan because it is related to: " +
                                    ((IDataset)relatedRow.Table).BrowseName +
                                    " OId: " + relatedRow.OID.ToString());
                                targetObjectisOrphan = false;
                                break;
                            }
                            relatedRow = (IRow)relatedSet.Next();
                        }

                        Marshal.FinalReleaseComObject(relatedSet);
                        pRelClass = pEnumRelClass.Next();
                    }
                    Marshal.FinalReleaseComObject(pEnumRelClass);


                    //If it is an orphan then record it to be deleted later in the 
                    //submit to post subtask 
                    if (targetObjectisOrphan)
                    {
                        _logger.Debug(
                            ((IDataset)pTargetObject.Class).BrowseName +
                            pTargetObject.OID.ToString() +
                            " is an orphan and will be added to the PGE_ORPHAN_ROWS table");
                        RecordOrphanRow(
                            ((IDataset)pTargetObject.Class).Name,
                            pTargetObject.OID);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Debug("An error occurred in: " + "PGE Cleanup Orphan JointOwners:" + ex.Message);
            }
            finally
            {
                BaseRelationshipAU.IsRunningAsRelAU = false;
            }
        }

        /// <summary>
        /// Records the records that were discovered to be orphans (without any 
        /// related records) in a table. These records will be later deleted in 
        /// the submit to post subtask 
        /// </summary>
        /// <param name="hshDeletes"></param>
        private void RecordOrphanRow(string datasetName, int oid)
        {
            try
            {
                //Open the PGE_ORPHAN_ROWS table 
                ITable pOrphanRowsTable = (ITable)ModelNameFacade.ObjectClassByModelName(Editor.EditWorkspace,
                    SchemaInfo.Electric.ClassModelNames.OrphanRows);
                IVersion pVersion = (IVersion)Editor.EditWorkspace;

                //Get column indexes 
                int fldIdxDateTime = pOrphanRowsTable.Fields.FindField("DATE_TIME");
                int fldIdxVersionName = pOrphanRowsTable.Fields.FindField("VERSION_NAME");
                int fldIdxDatasetName = pOrphanRowsTable.Fields.FindField("DATASET_NAME");
                int fldIdxOrphanRowOId = pOrphanRowsTable.Fields.FindField("ORPHAN_OID");

                //Record the orphan 
                IRow pNewRow = pOrphanRowsTable.CreateRow();
                pNewRow.set_Value(fldIdxDateTime, DateTime.Now);
                pNewRow.set_Value(fldIdxVersionName, pVersion.VersionName.ToUpper());
                pNewRow.set_Value(fldIdxDatasetName, datasetName.ToUpper());
                pNewRow.set_Value(fldIdxOrphanRowOId, oid);
                pNewRow.Store();
            }
            catch
            {
                throw new Exception("Error recording orphan rows");
            }
        }

    }
}


