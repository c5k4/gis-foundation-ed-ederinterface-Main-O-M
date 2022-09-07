using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Miner.ComCategories;
using PGE.Common.Delivery.Diagnostics;
using System.Reflection;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Framework;
using Miner.Interop;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;

namespace PGE.Desktop.EDER.AutoUpdaters.Special
{
    [Guid("e2dffbb8-f3d5-448e-bf38-1684320535fc")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("PGE.Desktop.EDER.AutoUpdaters.Special.PGEAnnoStatusSync")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    public class PGEAnnoStatusSync : BaseSpecialAU
    {
        protected static Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        public PGEAnnoStatusSync() : base("PGE Anno Status Sync")
        {

        }

        #region Base special AU Overrides
        /// <summary>
        /// Determines in which class the AU will be enabled
        /// </summary>
        /// <param name="objectClass"> Object's class. </param>
        /// <param name="eEvent">The edit event.</param>
        /// <returns> <c>true</c> if the AuoUpdater should be enabled; otherwise <c>false</c> </returns>
        protected override bool InternalEnabled(IObjectClass objectClass, mmEditEvent eEvent)
        {
            //Enabled if this feature class contains a status field
            if (ModelNameManager.FieldFromModelName(objectClass, SchemaInfo.Electric.FieldModelNames.Status) != null)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Determines whether actually this AU should be run, based on the AU Mode.
        /// </summary>
        /// <param name="eAUMode"> The auto updater mode. </param>
        /// <returns> <c>true</c> if the AuoUpdater should be executed; otherwise <c>false</c> </returns>
        protected override bool CanExecute(mmAutoUpdaterMode eAUMode)
        {
            if (eAUMode != mmAutoUpdaterMode.mmAUMNoEvents)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Implementation of AutoUpdater Execute Ex method for derived classes.
        /// </summary>
        /// <param name="obj">The object that triggered the AutoUpdater.</param>
        /// <param name="eAUMode">The auto updater mode.</param>
        /// <param name="eEvent">The edit event.</param>
        protected override void InternalExecute(IObject obj, mmAutoUpdaterMode eAUMode, mmEditEvent eEvent)
        {
            if (eEvent == mmEditEvent.mmEventFeatureCreate || eEvent == mmEditEvent.mmEventFeatureUpdate)
            {
                if (!IsAnnotation(obj.Class))
                {
                    IField statusField = ModelNameManager.FieldFromModelName(obj.Class, SchemaInfo.Electric.FieldModelNames.Status);
                    if (statusField != null)
                    {
                        int statusIdx = obj.Class.FindField(statusField.Name);
                        if (statusIdx >= 0)
                        {
                            object currentStatus = obj.get_Value(statusIdx);
                            SetRelatedAnnotationFeaturesStatus(obj as IFeature, currentStatus);
                        }
                    }
                }
                else
                {
                    //Get parent feature by utilizing the "FeatureID" field which maps to the actual objectID in the parent feature class.
                    SetAnnotationConstructionStatus(obj);
                }
            }
        }

        #endregion

        private void SetAnnotationConstructionStatus(IObject annotationObject)
        {
            Type auType = Type.GetTypeFromProgID("mmGeodatabase.MMAutoUpdater");
            object auObj = Activator.CreateInstance(auType);
            IMMAutoUpdater immAutoupdater = auObj as IMMAutoUpdater;
            mmAutoUpdaterMode currentAUMode = immAutoupdater.AutoUpdaterMode;
            immAutoupdater.AutoUpdaterMode = mmAutoUpdaterMode.mmAUMNoEvents;
            try
            {
                if (annotationObject is IFeature)
                {
                    //Should only be one relationship with the annotation feature class.
                    object currentAnnoConstructionStatus = null;
                    IFeature annoFeature = annotationObject as IFeature;
                    IField annoStatusfield = ModelNameManager.FieldFromModelName(annoFeature.Class, SchemaInfo.Electric.FieldModelNames.Status);
                    int annoStatusIdx = -1;
                    if (annoStatusfield != null)
                    {
                        annoStatusIdx = annoFeature.Class.FindField(annoStatusfield.Name);
                        if (annoStatusIdx >= 0)
                        {
                            currentAnnoConstructionStatus = annoFeature.get_Value(annoStatusIdx);
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        return;
                    }

                    IEnumRelationshipClass relationshipClasses = annoFeature.Class.get_RelationshipClasses(esriRelRole.esriRelRoleDestination);
                    relationshipClasses.Reset();
                    IRelationshipClass relClass = null;
                    while ((relClass = relationshipClasses.Next()) != null)
                    {
                        object relatedObject = null;
                        ISet relatedObjects = relClass.GetObjectsRelatedToObject(annoFeature);
                        while ((relatedObject = relatedObjects.Next()) != null)
                        {
                            if (relatedObject is IFeature)
                            {
                                IFeature relatedFeature = relatedObject as IFeature;
                                IField field = ModelNameManager.FieldFromModelName(relatedFeature.Class, SchemaInfo.Electric.FieldModelNames.Status);
                                if (field != null)
                                {
                                    int statusIdx = relatedFeature.Class.FindField(field.Name);
                                    if (statusIdx >= 0)
                                    {
                                        object statusValue = null;
                                        statusValue = relatedFeature.get_Value(statusIdx);
                                        if ((currentAnnoConstructionStatus != null && statusValue == null) || (statusValue != null && currentAnnoConstructionStatus == null)
                                            || ((currentAnnoConstructionStatus != null && statusValue != null) && (currentAnnoConstructionStatus.ToString() != statusValue.ToString())))
                                        {
                                            //Only set the value as this will be executing on the feature it is assigned
                                            annoFeature.set_Value(annoStatusIdx, statusValue);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to set status value for annotation feature: " + ex.Message);
            }
            finally
            {
                immAutoupdater.AutoUpdaterMode = currentAUMode;
            }
        }
        
        private void SetRelatedAnnotationFeaturesStatus(IFeature feat, object statusValue)
        {
            Type auType = Type.GetTypeFromProgID("mmGeodatabase.MMAutoUpdater");
            object auObj = Activator.CreateInstance(auType);
            IMMAutoUpdater immAutoupdater = auObj as IMMAutoUpdater;
            mmAutoUpdaterMode currentAUMode = immAutoupdater.AutoUpdaterMode;
            immAutoupdater.AutoUpdaterMode = mmAutoUpdaterMode.mmAUMNoEvents;
            try
            {
                IEnumRelationshipClass relationshipClasses = feat.Class.get_RelationshipClasses(esriRelRole.esriRelRoleOrigin);
                relationshipClasses.Reset();
                IRelationshipClass relClass = null;
                while ((relClass = relationshipClasses.Next()) != null)
                {
                    if (ModelNameManager.ContainsClassModelName(relClass.DestinationClass, SchemaInfo.Electric.ClassModelNames.AnnotationStatusSync))
                    {
                        object relatedObject = null;
                        ISet relatedObjects = relClass.GetObjectsRelatedToObject(feat);
                        while ((relatedObject = relatedObjects.Next()) != null)
                        {
                            if (relatedObject is IFeature)
                            {
                                IFeature relatedFeature = relatedObject as IFeature;
                                IField field = ModelNameManager.FieldFromModelName(relatedFeature.Class, SchemaInfo.Electric.FieldModelNames.Status);
                                if (field != null)
                                {
                                    int statusIdx = relatedFeature.Class.FindField(field.Name);
                                    if (statusIdx >= 0)
                                    {
                                        object currentAnnoStatus = relatedFeature.get_Value(statusIdx);
                                        if ((currentAnnoStatus != null && statusValue == null) || (statusValue != null && currentAnnoStatus == null)
                                            || ((currentAnnoStatus != null && statusValue != null) && (currentAnnoStatus.ToString() != statusValue.ToString())))
                                        {
                                            //Need to set value and call store as this will be setting a related features value
                                            relatedFeature.set_Value(statusIdx, statusValue);
                                            relatedFeature.Store();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to set status value for related annotation: " + ex.Message);
            }
            finally
            {
                immAutoupdater.AutoUpdaterMode = currentAUMode;
            }
        }

        /// <summary>
        /// Checks if given feature class is an annotation feature class.
        /// </summary>
        /// <param name="objClass">IObjectClass to check</param>
        /// <returns></returns>
        private bool IsAnnotation(IObjectClass objClass)
        {
            if (objClass is IFeatureClass && ((IFeatureClass)objClass).FeatureType == esriFeatureType.esriFTAnnotation)
            {
                return true;
            }
            return false;
        }
    }
}
