using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

using Miner.Interop;
using Miner.Geodatabase;
using Miner.ComCategories;
using log4net;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Diagnostics;


namespace PGE.Desktop.EDER.AutoUpdaters.Special
{
    /// <summary>
    /// Class to Reset Anno Symbol Alignment to its initial state
    /// </summary>
    [Guid("9EB2769B-5B21-4027-A796-675E268F2BE4")]
    [ProgId("PGE.ED.AlignAnnoAU")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    public class AlignAnnoAU : BaseSpecialAU
    {
        /// <summary>
        /// Initializes the new instance of <see cref="AlignAnnoAU"/>
        /// </summary>
        public AlignAnnoAU() : base("PGE Align Anno AU") { }

        #region Private Variables
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        #endregion Private Variables

        #region Override Methods
        /// <summary>
        /// Enable the AU when the Create/Update object AU category is selected.
        /// </summary>
        /// <param name="pObjectClass">The selected object class.</param>
        /// <param name="eEvent">The AU event type.</param>
        /// <returns>True if condition satisfied else False.</returns>
        protected override bool InternalEnabled(IObjectClass objectClass, mmEditEvent eEvent)
        {
            //Enable if Feature Event type is Create or Update 
            if (eEvent == mmEditEvent.mmEventFeatureCreate || eEvent == mmEditEvent.mmEventFeatureUpdate || eEvent == mmEditEvent.mmEventAfterFeatureSplit)
            {
                IAnnoClass annoClass = objectClass.Extension as IAnnoClass;

                if (annoClass != null)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Execute the AU while working with ArcMap Application only.
        /// </summary>
        /// <param name="eAUMode">The AU application mode.</param>
        /// <returns>True if condition satisfied else false.</returns>
        protected override bool CanExecute(mmAutoUpdaterMode eAUMode)
        {
            //Enable if Application type is ArcMap
            if (eAUMode == mmAutoUpdaterMode.mmAUMArcMap || eAUMode == mmAutoUpdaterMode.mmAUMSplit)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Execute Annotation Horizontal Alignment AU
        /// </summary>
        /// <param name="obj">The Object being Updated.</param>
        /// <param name="eAUMode">The AU Mode.</param>
        /// <param name="eEvent">The edit event.</param>
        protected override void InternalExecute(IObject pObject, mmAutoUpdaterMode eAUMode, mmEditEvent eEvent)
        {
            try
            {
                _logger.Debug("Starting " + Name + ".");

                //if create event then update the alignment and annotation properties and store it
                if (eEvent == mmEditEvent.mmEventFeatureCreate || eEvent == mmEditEvent.mmEventFeatureUpdate || eEvent == mmEditEvent.mmEventAfterFeatureSplit || eEvent == mmEditEvent.mmEventFeatureUpdate)
                {
                    //Get all RC and update all annotations
                    UpdateAnnoFeature((IFeature)pObject);
                }
            }
            catch (Exception e)
            {
                _logger.Error("Exception in Execute() for " + Name + ":" + e.ToString());
            }
            finally
            {
                _logger.Debug("Finished executing " + Name + ".");
            }

        }
        #endregion Override Methods

        #region Private Methods

        ///// <summary>
        ///// Checks the ObjectClass fields for changes or LabelText and LabelText2.
        ///// </summary>
        ///// <param name="pObject">The Object being created or updated.</param>
        ///// <returns>Return as True if changed Else False.</returns>
        //private bool IsRowAndFeatureChanged(IObject pObject)
        //{
        //    IRowChanges rowChanges = pObject as IRowChanges;
        //    IObjectClass objClass = pObject.Class;
        //    foreach (string mn in _fieldModelNameDrivingAnno)
        //    {
        //        List<int> fldIdx = ModelNameFacade.FieldIndicesFromModelName(objClass, mn);
        //        foreach (int idx in fldIdx)
        //        {
        //            if (rowChanges.get_ValueChanged(idx))
        //            {
        //                return true;
        //            }
        //        }
        //    }
        //    return false;
        //}

        /// <summary>
        /// Updates Alignment and Annotations properties
        /// </summary>
        /// <param name="annoFeature">AnnotationFeature to be updated</param>
        private void UpdateAnnoFeature(IFeature feature)
        {
            _logger.Debug("Querying Feature [ " + ((IDataset)feature.Class).Name + " ] OID [ " + feature.OID.ToString() + " ]");

            //set annotation property and its alignment and store it
            IAnnotationFeature annoFeature = feature as IAnnotationFeature;

            // Get alignment properties
            IAnnoClass annoClass = feature.Class.Extension as IAnnoClass;
            IAnnotateLayerPropertiesCollection annotateLayerPropertiesCollection = annoClass.AnnoProperties;
            IAnnotationFeature2 annotationFeature2 = annoFeature as IAnnotationFeature2;
            IAnnotateLayerProperties annotateLayerProperties = null;
            IElementCollection elementCollection = null;
            IElementCollection elementCollectionUnplaced = null;
            annotateLayerPropertiesCollection.QueryItem(annotationFeature2.AnnotationClassID, out annotateLayerProperties, out elementCollection, out elementCollectionUnplaced);
            ILabelEngineLayerProperties2 labelEngineLayerProperties2 = annotateLayerProperties as ILabelEngineLayerProperties2;
            
            //get AnnotationFeature.Annotation as IElement
            IElement element = annoFeature.Annotation;
            ITextElement tElement = element as ITextElement;
            //cast IElement object to ISymbolCollectionElement
            ISymbolCollectionElement sce = element as ISymbolCollectionElement;
            if (sce != null)
            {
                // Reset Alignment
                _logger.Debug("Resetting alignment to Vertical [ " + Enum.GetName(typeof(esriTextVerticalAlignment), labelEngineLayerProperties2.Symbol.VerticalAlignment) +
                    " ] Horizontal [ " + Enum.GetName(typeof(esriTextHorizontalAlignment), labelEngineLayerProperties2.Symbol.HorizontalAlignment) + " ]");
                sce.HorizontalAlignment = labelEngineLayerProperties2.Symbol.HorizontalAlignment;
                sce.VerticalAlignment = labelEngineLayerProperties2.Symbol.VerticalAlignment;
                annoFeature.Annotation = sce as IElement;
                feature = annoFeature as IFeature;
                feature.Store();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        private void UpdateAllAnnotations(IObject obj)
        {
            IEnumerable<IRelationshipClass> relClasses = GetAnnotationRelationshipClass(obj);
            foreach (IRelationshipClass rc in relClasses)
            {
                if (ModelNameManager.ContainsClassModelName(rc.DestinationClass, "PGE_ALIGNANNO"))
                {
                    ESRI.ArcGIS.esriSystem.ISet relSet = rc.GetObjectsRelatedToObject(obj);
                    IFeature feat = null;
                    while ((feat = relSet.Next() as IFeature) != null)
                    {
                        UpdateAnnoFeature(feat);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private IEnumerable<IRelationshipClass> GetAnnotationRelationshipClass(IObject obj)
        {
            return obj.GetRelationships(null)
                .Where(rc => (rc.DestinationClass is IFeatureClass && (rc.DestinationClass as IFeatureClass)
                    .FeatureType == esriFeatureType.esriFTAnnotation));
        }
        #endregion Private Methods
    }
}
