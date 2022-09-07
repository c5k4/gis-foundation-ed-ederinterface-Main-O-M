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
    /// Class to Manage 500Anno in 250 scale schematic grid
    /// </summary>
    [ComVisible(true)]
    [Guid("C528DB69-512A-4FC1-8965-4B26E928D57B")]
    [ProgId("PGE.Desktop.EDER.AnnoHalfFontSizeAU")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    public class AnnoHalfFontSizeAU : BaseSpecialAU
    {
        /// <summary>
        /// Initializes the new instance of <see cref="AnnoHalfFontSizeAU"/>
        /// </summary>
        public AnnoHalfFontSizeAU() : base("PGE Annotation Half Font Size AU") { }

        #region Private Variables
        /// <summary>
        /// Logs the debug/error information
        /// </summary>
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        #endregion Private Variables

        #region Override Methods
        /// <summary>
        /// Enable the AU when the Create object AU category only if it is an 500 scale annotation feature class.
        /// </summary>
        /// <param name="pObjectClass">The selected object class.</param>
        /// <param name="eEvent">The AU event type.</param>
        /// <returns>True if condition satisfied else False.</returns>
        protected override bool InternalEnabled(IObjectClass objectClass, mmEditEvent eEvent)
        {
            //Enable if Feature Event type is Create or Update 
            if (eEvent == mmEditEvent.mmEventFeatureCreate)
            {
                if (objectClass is IFeatureClass)
                {
                    IFeatureClass featClass = objectClass as IFeatureClass;
                    if (featClass.FeatureType == esriFeatureType.esriFTAnnotation)
                    {
                        if (ModelNameFacade.ModelNameManager.ContainsClassModelName(objectClass, SchemaInfo.General.ClassModelNames.Schematic500Anno))
                        {
                            return true;
                        }
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
        /// Execute Annotation Half Font Size AU
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

                //check if event is Create or Update then update the font size and store it
                
                if (eEvent == mmEditEvent.mmEventFeatureCreate)
                {
                    immAutoupdater.AutoUpdaterMode = mmAutoUpdaterMode.mmAUMNoEvents;
                    UpdateAnnoFeature(feat);
                }
            }
            catch (Exception e)
            {
                _logger.Error("Error executing annotation half font size AU. Message: " + e.Message);
            }
            finally
            {
                immAutoupdater.AutoUpdaterMode = currentAUMode;
            }

        }
        #endregion Override Methods

        #region Private Methods

        /// <summary>
        /// Updates Font Size
        /// </summary>
        /// <param name="feature">AnnotationFeature to be updated</param>
        private void UpdateAnnoFeature(IFeature feature)
        {
            IFeatureClass schmGridFeatureClass;
            ISpatialFilter spatialFilter = new SpatialFilterClass();
            IFeatureCursor featureCursor = null;
            try
            {
                schmGridFeatureClass = ModelNameFacade.FeatureClassByModelName((feature.Class as IDataset).Workspace, SchemaInfo.General.ClassModelNames.SchematicsGrid);
                spatialFilter.Geometry = feature.Shape;
                spatialFilter.GeometryField = schmGridFeatureClass.ShapeFieldName;
                spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                featureCursor = schmGridFeatureClass.Search(spatialFilter, true);

                int gridScaleFldIx = ModelNameFacade.FieldIndexFromModelName(schmGridFeatureClass, SchemaInfo.General.FieldModelNames.Scale);
                int fontSizeFldIx = ModelNameFacade.FieldIndexFromModelName(feature.Class, SchemaInfo.General.FieldModelNames.FontSize);

                if (gridScaleFldIx == -1)
                {
                    string[] modelNames = new string[] { SchemaInfo.General.FieldModelNames.Scale, SchemaInfo.General.ClassModelNames.SchematicsGrid };
                    _logger.Debug(string.Format("PGE Annotation Half Font Size AU: {0} field model name is missing on class with {1} model name", modelNames));
                    return;
                }
                if (fontSizeFldIx == -1)
                {
                    string[] modelNames = new string[] { SchemaInfo.General.FieldModelNames.FontSize, SchemaInfo.General.ClassModelNames.Schematic500Anno };
                    _logger.Debug(string.Format("PGE Annotation Half Font Size AU: {0} field model name is missing on class with {1} model name", modelNames));
                    return;
                }

                object scaleValue;
                IFeature schmGridFeature;
                bool is250 = false;

                while ((schmGridFeature = featureCursor.NextFeature()) != null)
                {
                    //Get Map Scale
                    scaleValue = schmGridFeature.get_Value(gridScaleFldIx);
                    if (scaleValue == null || scaleValue == DBNull.Value)
                    {
                        _logger.Debug(string.Format("PGE Annotation Half Font Size AU: Schematic Grid Scale is null for OID ={0}", schmGridFeature.OID));
                        continue;
                    }

                    //Check if the anno falls within 250 scale grid
                    is250 = (Convert.ToInt32(scaleValue) == 250);

                    if (is250)
                    {
                        IAnnotationFeature annoFeature = feature as IAnnotationFeature;
                        //get AnnotationFeature.Annotation as IElement
                        IElement element = annoFeature.Annotation;

                        //cast IElement object to ISymbolCollectionElement
                        ISymbolCollectionElement sce = element as ISymbolCollectionElement;
                        if (sce != null)
                        {
                            int fontSize = Convert.ToInt32(sce.Size);
                            sce.Size = fontSize / 2;

                            //Set the font size to half the given font size of the annotation.                           
                            annoFeature.Annotation = sce as IElement;
                            feature = annoFeature as IFeature;
                            feature.Store();
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Debug(string.Format("PGE Annotation Half Font Size AU: {0}", ex.Message));
            }
            finally
            {
                while (Marshal.ReleaseComObject(featureCursor) > 0) { }
                while (Marshal.ReleaseComObject(spatialFilter) > 0) { }               
            }
        }

        #endregion Private Methods
    }
}
