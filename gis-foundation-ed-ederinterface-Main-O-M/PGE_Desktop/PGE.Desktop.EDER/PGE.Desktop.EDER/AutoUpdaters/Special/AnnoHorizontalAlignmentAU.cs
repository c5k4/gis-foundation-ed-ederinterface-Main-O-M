using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

using Miner.Interop;
using Miner.Geodatabase;
using Miner.ComCategories;
using PGE.Common.Delivery.Diagnostics;
using PGE.Common.Delivery.Framework;
using PGE.Common.Delivery.ArcFM;
using log4net;
using ESRI.ArcGIS.Carto;

namespace PGE.Desktop.EDER.AutoUpdaters.Special
{
    /// <summary>
    /// Class to Manage PG&E Unique Circuit Id
    /// </summary>
    [ComVisible(true)]
    [Guid("CD3C8D59-B594-48F8-A52D-119112E93DF4")]
    [ProgId("PGE.Desktop.EDER.AnnoHorizontalAlignmentAU")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    public class AnnoHorizontalAlignmentAU : BaseSpecialAU
    {
        /// <summary>
        /// Initializes the new instance of <see cref="AnnoHorizontalAlignmentAU"/>
        /// </summary>
        public AnnoHorizontalAlignmentAU() : base("PGE Annotation Horizontal Alignment AU") { }

        #region Private Variables
        /// <summary>
        /// Logs the debug/error information
        /// </summary>
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        #endregion Private Variables

        #region Override Methods
        /// <summary>
        /// Enable the AU when the Create/Update object AU category only if it is an annotation feature class.
        /// </summary>
        /// <param name="pObjectClass">The selected object class.</param>
        /// <param name="eEvent">The AU event type.</param>
        /// <returns>True if condition satisfied else False.</returns>
        protected override bool InternalEnabled(IObjectClass objectClass, mmEditEvent eEvent)
        {
            //Enable if Feature Event type is Create or Update 
            if (eEvent == mmEditEvent.mmEventFeatureCreate || eEvent == mmEditEvent.mmEventFeatureUpdate)
            {
                if (objectClass is IFeatureClass)
                {
                    IFeatureClass featClass = objectClass as IFeatureClass;
                    if (featClass.FeatureType == esriFeatureType.esriFTAnnotation)
                    {
                        return true;
                    }
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
            mmAutoUpdaterMode currentAUMode = eAUMode;
            Type auType = Type.GetTypeFromProgID("mmGeodatabase.MMAutoUpdater");
            object auObj = Activator.CreateInstance(auType);
            IMMAutoUpdater immAutoupdater = auObj as IMMAutoUpdater;

            try
            {
                //Ensure this is an annotation feature class
                if (pObject.Class is IFeatureClass)
                {
                    IFeatureClass featClass = pObject.Class as IFeatureClass;
                    if (featClass.FeatureType != esriFeatureType.esriFTAnnotation)
                    {
                        return;
                    }
                }

                IFeature feat = pObject as IFeature;

                //check if event is Create or Update
                //if create event then update the alignment and annotation properties and store it
                if (eEvent == mmEditEvent.mmEventFeatureCreate)
                {
                    immAutoupdater.AutoUpdaterMode = mmAutoUpdaterMode.mmAUMNoEvents;
                    UpdateAnnoFeature(feat);
                }
                //else if event is update make sure fields updated/changed are belongs to FieldModel name which involves in 
                else if (eEvent == mmEditEvent.mmEventFeatureUpdate)
                {
                    immAutoupdater.AutoUpdaterMode = mmAutoUpdaterMode.mmAUMNoEvents;
                    UpdateAnnoFeature(feat);
                }
            }
            catch (Exception e)
            {
                _logger.Error("Error executing annotation horizontal alignment AU. Message: " + e.Message);
            }
            finally
            {
                immAutoupdater.AutoUpdaterMode = currentAUMode;
            }

        }
        #endregion Override Methods

        #region Private Methods

        /// <summary>
        /// Updates Alignment and Annotations properties
        /// </summary>
        /// <param name="annoFeature">AnnotationFeature to be updated</param>
        private void UpdateAnnoFeature(IFeature feature)
        {
            //set annotation property and its alignment and store it
            IAnnotationFeature annoFeature = feature as IAnnotationFeature;
            //get AnnotationFeature.Annotation as IElement
            IElement element = annoFeature.Annotation;

            ITextElement tElement = element as ITextElement;

            //cast IElement object to ISymbolCollectionElement
            ISymbolCollectionElement sce = element as ISymbolCollectionElement;
            if (sce != null)
            {
                sce.HorizontalAlignment = ESRI.ArcGIS.Display.esriTextHorizontalAlignment.esriTHACenter;
                annoFeature.Annotation = sce as IElement;
                //cast AnnotationFeature to IFeature
                feature = annoFeature as IFeature;
                
                feature.Store();
            }
        }

        #endregion Private Methods
    }
}
