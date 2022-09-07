using System;
using System.Reflection;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.Geodatabase;

using Miner.ComCategories;
using Miner.Interop;

using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Framework;
using ESRI.ArcGIS.esriSystem;
namespace PGE.Desktop.EDER.AutoUpdaters.Special
{
    /// <summary>
    /// Changes for ENOS to SAP migration, Updates Gen Category field in Service location feature class if there is relationship change in service point with primary meter or transformer
    /// </summary>
    [Guid("1C4AC9A6-04AA-4AE6-81D1-D056BEA890AE")]
    [ProgId("PGE.Desktop.EDER.PGEUpdateGenCategory")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    [ComVisible(true)]
    public class PGEUpdateGenCategory : BaseSpecialAU
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PGEUpdateGenCategory"/> class.  
        /// </summary>
        public PGEUpdateGenCategory() : base("PGE Update Gen Category AU") { }
        #endregion Constructors

        #region Privates

        private static readonly PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        private static string _genCategoryFldname = "GENCATEGORY";
        #endregion Privates

        #region Special AU Overrides
        /// <summary>
        /// Enable the AU when the update object AU category is selected.
        /// </summary>
        /// <param name="pObjectClass">The selected object class.</param>
        /// <param name="eEvent">The AU event type.</param>
        /// <returns></returns>
        protected override bool InternalEnabled(IObjectClass pObjectClass, Miner.Interop.mmEditEvent eEvent)
        {
            string[] modelNames = new string[] { SchemaInfo.Electric.ClassModelNames.ServicePoint};
            bool enabled = ModelNameFacade.ContainsClassModelName(pObjectClass, modelNames);
            _logger.Debug(string.Format("ClassModelName : {0}, exist:{1}", SchemaInfo.Electric.ClassModelNames.ServicePoint, enabled));
            if (eEvent == Miner.Interop.mmEditEvent.mmEventFeatureCreate && enabled)
            {
                _logger.Debug("Returning Visible: true.");
                return true;
            }
            _logger.Debug("Returning Visible: false.");
            return false;
        }

        /// <summary>
        /// Executes the PGEUpdateGenCategory AU.
        /// </summary>
        /// <param name="pObject">The object being updated.</param>
        /// <param name="eAUMode">The AU mode.</param>
        /// <param name="eEvent">The edit event.</param>
        protected override void InternalExecute(IObject pObject, Miner.Interop.mmAutoUpdaterMode eAUMode, Miner.Interop.mmEditEvent eEvent)
        {

            if (ModelNameFacade.ContainsClassModelName(pObject.Class, SchemaInfo.Electric.ClassModelNames.ServicePoint))
            {
                int transformerGuidFldId = pObject.Class.Fields.FindField("TRANSFORMERGUID");
                int primaryMeterGuidFldId = pObject.Class.Fields.FindField("PRIMARYMETERGUID");
                if (eEvent == mmEditEvent.mmEventFeatureCreate)
                {
                    if (pObject.get_Value(transformerGuidFldId) != null)
                    {
                        UpdateGenCategory(pObject, 2);
                    }
                    else if (pObject.get_Value(primaryMeterGuidFldId) != null)
                    {
                        UpdateGenCategory(pObject, 1);
                    }
                    else if (pObject.get_Value(transformerGuidFldId) == null && pObject.get_Value(primaryMeterGuidFldId) == null)
                    {
                        UpdateGenCategory(pObject, 0);
                    }

                }
                //else if (eEvent == mmEditEvent.mmEventFeatureUpdate)
                //{
                //    IRowChanges rowChanges = pObject as IRowChanges;
                //    if (rowChanges != null)
                //    {
                //        if (rowChanges.ValueChanged[transformerGuidFldId] || rowChanges.ValueChanged[primaryMeterGuidFldId])
                //        {
                //            //both values are changed by a relationship modification on service point
                //            if (rowChanges.get_OriginalValue(transformerGuidFldId) == null && pObject.get_Value(transformerGuidFldId) != null)
                //            {
                //                //this means service point has new relationship with a transformer, so update Gen category as 2
                //                UpdateGenCategory(pObject, 2);
                //            }
                //            else if (rowChanges.get_OriginalValue(primaryMeterGuidFldId) == null && pObject.get_Value(primaryMeterGuidFldId) != null)
                //            {
                //                //this means service point has new relationship with a transformer, so update Gen category as 1
                //                UpdateGenCategory(pObject, 1);
                //            }
                //        }
                //        else
                //        {

                //        }
                //    }
                //}
                //else if (eEvent == mmEditEvent.mmEventFeatureDelete)
                //{
                //    UpdateGenCategory(pObject, 0);
                //}
            }

        }
        /// <summary>
        /// Gets related service point for a Generation Info, and Updates GENCATEGORY feild value in related service location
        /// </summary>
        /// <param name="pObject"></param>
        private void UpdateRelatedServiceLocation(IObject pObject)
        {
            IRelationshipClass iRelationshipClass = ModelNameFacade.RelationshipClassFromModelName(pObject.Class, esriRelRole.esriRelRoleAny, SchemaInfo.Electric.ClassModelNames.ServicePoint);
            if (iRelationshipClass != null)
            {
                ISet relatedSPObjects = iRelationshipClass.GetObjectsRelatedToObject(pObject as IObject);
                if (relatedSPObjects.Count > 0)
                {
                    IObject relatedSPObject = null;
                    relatedSPObjects.Reset();
                    //relatedObject = relatedObjects.Next() as IObject;
                    while ((relatedSPObject = relatedSPObjects.Next() as IObject) != null)
                    {
                        int transformerGuidFldId = relatedSPObject.Class.Fields.FindField("TRANSFORMERGUID");
                        int primaryMeterGuidFldId = relatedSPObject.Class.Fields.FindField("PRIMARYMETERGUID");
                        if (relatedSPObject.get_Value(transformerGuidFldId) != null)
                        {
                            //If the related service point has value in TRANSFORMERGUID feild, that means 'GENCATEGORY' in service location should be '2' [secondary]
                            UpdateGenCategory(relatedSPObject, 2);
                        }
                        else if (relatedSPObject.get_Value(primaryMeterGuidFldId) != null)
                        {
                            //If the related service point has value in PRIMARYMETERGUID feild, that means 'GENCATEGORY' in service location should be '1' [Primary]
                            UpdateGenCategory(relatedSPObject, 1);
                        }
                        else if (relatedSPObject.get_Value(transformerGuidFldId) == null && relatedSPObject.get_Value(primaryMeterGuidFldId) == null)
                        {
                            //If service point has null values in both of Primary meter and transformer guid fields, GENCATEGORY should be NONE
                            UpdateGenCategory(relatedSPObject, 0);
                        }
                    }
                }
            }
        }
        #endregion Special AU Overrides
        /// <summary>
        /// Updates GENCATEGORY field in service location feature class
        /// </summary>
        /// <param name="servicePoint"></param>
        /// <param name="value"></param>
        private void UpdateGenCategory(IObject servicePoint, int value)
        {
            IRelationshipClass iRelationshipClass = ModelNameFacade.RelationshipClassFromModelName(servicePoint.Class, esriRelRole.esriRelRoleAny, SchemaInfo.Electric.ClassModelNames.ServiceLocation);
            if (iRelationshipClass != null)
            {
                ISet relatedObjects = iRelationshipClass.GetObjectsRelatedToObject(servicePoint as IObject);
                if (relatedObjects.Count > 0)
                {
                    IObject relatedObject = null;
                    relatedObjects.Reset();
                    //relatedObject = relatedObjects.Next() as IObject;
                    while ((relatedObject = relatedObjects.Next() as IObject) != null)
                    {
                        relatedObject.set_Value((relatedObject.Fields.FindField(_genCategoryFldname)), value);
                        relatedObject.Store();
                    }
                }
            }
        }
    }
}
