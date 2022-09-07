using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.esriSystem;

using Miner.Interop;
using Miner.ComCategories;

using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Diagnostics;
using PGE.Common.Delivery.Framework;

namespace PGE.Desktop.EDER.AutoUpdaters.Special
{
    [ComVisible(true)]
    [ProgId("PGE.Desktop.EDER.RestoreRelatedObjects")]
    [Guid("fb4de353-e5b1-44fb-be4d-301497ce08b2")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]

    public class RestoreRelatedObjects : BaseSplitAU
    {

        #region Constuctor

        public RestoreRelatedObjects() : base("PGE Restore Related Objects") { }

        #endregion

        #region BaseSplitAU Overrides

        protected override void InternalExecuteExtended(IObject obj, mmAutoUpdaterMode eAUMode, mmEditEvent eEvent)
        {
            //This is only applicable to aftersplit event 
            if (eEvent == mmEditEvent.mmEventBeforeFeatureSplit) { return; }

            //Recreate the relationships to all objects with modelname: PGE_RESTORERELS 
            RestoreRelObjects(obj);
        }

        private void RestoreRelObjects(IObject obj)
        {
            try
            {
                IMMSplitCGO splitCGO = obj as IMMSplitCGO;
                if (splitCGO == null) return;
                Hashtable hshOids = null;
                ITable pOtherTable = null;
                IObject pRelatedObject = null;
                IRelationshipClass pTargetRelClass = null;

                foreach (string dataset in _originalRelatedObjects.Keys)
                {
                    //Find the relationshipclass between the split class and 
                    //the restorerel class 
                    pTargetRelClass = null;
                    List<IRelationshipClass> pRestoreRelClasses = ModelNameFacade.RelationshipClassesFromModelName(
                            obj.Class, esriRelRole.esriRelRoleAny, SchemaInfo.Electric.ClassModelNames.RestoreRels);
                    foreach (IRelationshipClass pRelClass in pRestoreRelClasses)
                    {
                        if (((IDataset)pRelClass.OriginClass).Name.ToUpper() == dataset)
                        {
                            pTargetRelClass = pRelClass;
                            pOtherTable = (ITable)pRelClass.OriginClass;
                            break;
                        }
                        else if (((IDataset)pRelClass.DestinationClass).Name.ToUpper() == dataset)
                        {
                            pTargetRelClass = pRelClass;
                            pOtherTable = (ITable)pRelClass.DestinationClass;
                            break;
                        }
                    }

                    if (pTargetRelClass != null)
                    {
                        hshOids = (Hashtable)_originalRelatedObjects[dataset];
                        if (hshOids.Count > 0)
                        {
                            //Create the relationship to the 2 split parts 
                            ISet newEdges = splitCGO.GetNewEdges();
                            newEdges.Reset();
                            IObject newEdge = (IObject)newEdges.Next();

                            //Restore the rels to all of the related objects 
                            foreach (int oid in hshOids.Keys)
                            {
                                if (pOtherTable != null)
                                {
                                    pRelatedObject = GetObjectByOId(pOtherTable, oid);
                                    if (pRelatedObject != null)
                                    {
                                        while (newEdge != null)
                                        {
                                            pTargetRelClass.CreateRelationship(newEdge, pRelatedObject);
                                            newEdge = (IObject)newEdges.Next();
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        _logger.Error("Splitting error relationship not found," +
                            " it is likely that modelname: " + SchemaInfo.Electric.ClassModelNames.RestoreRels +
                            " has not been applied");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error restoring related objects. ", ex);
                _originalRelatedObjects.Clear();
                _originalRelatedObjects = null;
            }
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

        #endregion
    }
}


