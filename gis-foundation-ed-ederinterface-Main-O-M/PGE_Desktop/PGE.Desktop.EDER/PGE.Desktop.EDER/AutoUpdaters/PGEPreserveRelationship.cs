using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geodatabase;
using Miner.ComCategories;
using Miner.Interop;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Diagnostics;

namespace PGE.Desktop.EDER.AutoUpdaters
{
    [Guid("D288C470-0F53-49F9-9E9C-82CBFAA35BEE")]
    [ProgId("PGE.Desktop.EDER.AutoUpdaters.PGEPreserveRelationship")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    public class PGEPreserveRelationship : BaseSpecialAU
    {
        #region Private variable

        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        private static List<IObject> parentObjectList;

        #endregion

        #region Constructor

        /// <summary>
        /// Public default constructor
        /// </summary>
        public PGEPreserveRelationship()
            : base("PGE Preserve Relationship AU")
        {

        }

        #endregion

        #region BaseSpecialAU implementation

        /// <summary>
        /// Check's the condition for autoupdator enabling.
        /// </summary>
        /// <param name="objectClass"></param>
        /// <param name="eEvent"></param>
        /// <returns></returns>
        protected override bool InternalEnabled(IObjectClass objectClass, mmEditEvent eEvent)
        {
            if (objectClass is IFeatureClass)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Enable AU if Application type is ArcMap
        /// </summary>
        /// <param name="eAUMode"></param>
        /// <returns></returns>
        protected override bool CanExecute(mmAutoUpdaterMode eAUMode)
        {
            if (eAUMode == mmAutoUpdaterMode.mmAUMArcMap)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Assign the relationship to deactivated line segment.
        /// </summary>
        /// <param name="pObject"></param>
        /// <param name="eAUMode"></param>
        /// <param name="eEvent"></param>
        protected override void InternalExecute(IObject pObject, mmAutoUpdaterMode eAUMode, mmEditEvent eEvent)
        {
            mmAutoUpdaterMode currentAUMode = eAUMode;
            Type auType = Type.GetTypeFromProgID("mmGeodatabase.MMAutoUpdater");
            object auObj = Activator.CreateInstance(auType);

            IMMAutoUpdater immAutoupdater = auObj as IMMAutoUpdater;
            try
            {
                IRelationshipClass relationshipClass = null;
                parentObjectList = PGE.Desktop.EDER.D8TreeTools.PGE_DeactivationTool.parentObjectList;
                Dictionary<string, Dictionary<string, string>> parentObjectKayVal = PGE.Desktop.EDER.D8TreeTools.PGE_DeactivationTool.parentObjectKayVal;
                foreach (IObject parentObject in parentObjectList)
                {
                    IEnumRelationshipClass enumRelationshipClass = pObject.Class.get_RelationshipClasses(esriRelRole.esriRelRoleDestination);
                    enumRelationshipClass.Reset();
                    IRelationship relationship = null;
                    while ((relationshipClass = enumRelationshipClass.Next()) != null)
                    {
                        if (relationshipClass.BackwardPathLabel == SchemaInfo.Electric.Rel_BackwardPathLabel)//"Conduit System")
                        {
                            relationship = relationshipClass.CreateRelationship(parentObject as IObject, pObject as IObject);

                            ITable parentRelationShip = relationshipClass as ITable;
                            int row = parentRelationShip.FindField(SchemaInfo.Electric.UlsObjectId);// "ULSOBJECTID");
                            int uLS_POS = parentRelationShip.FindField(SchemaInfo.Electric.UlsPosition);//"ULS_POSITION");
                            int phase = parentRelationShip.FindField(SchemaInfo.Electric.Phasedesignation);//"PHASEDESIGNATION");
                            string whereClause = string.Format("{0}='{1}' and {2}='{3}'", SchemaInfo.Electric.UlsObjectId, parentObject.OID, SchemaInfo.Electric.UGObjectId, pObject.OID);
                            IQueryFilter filter = new QueryFilterClass();
                            filter.WhereClause = whereClause;
                            ICursor featureCursor = parentRelationShip.Search(filter, false);
                            IRow uLS_row;
                            while ((uLS_row = featureCursor.NextRow()) != null)
                            {

                                foreach (var item in parentObjectKayVal)
                                {
                                    if (item.Key == uLS_row.get_Value(row).ToString())//parentObject)  //relationshipClass
                                    {
                                        Dictionary<string, string> value = item.Value;
                                        foreach (var valItem in value)
                                        {
                                            if (!string.IsNullOrEmpty(valItem.Key.ToString()))
                                            {
                                                immAutoupdater.AutoUpdaterMode = mmAutoUpdaterMode.mmAUMNoEvents;
                                                uLS_row.set_Value(uLS_POS, valItem.Key);
                                                //uLS_row.set_Value(phase, valItem.Value); //Uncomment if phase-designation should updated.
                                                uLS_row.Store();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Error("Error while running Preserve Relationship: " + e.Message);
            }
            finally
            {
                immAutoupdater.AutoUpdaterMode = currentAUMode;
            }
        }

        #endregion
    }
}
