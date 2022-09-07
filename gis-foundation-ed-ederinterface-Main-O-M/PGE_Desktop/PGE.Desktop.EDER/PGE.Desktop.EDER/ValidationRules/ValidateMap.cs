using System;
using System.Reflection;
using System.Collections;
using System.Linq;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geodatabase;
using Miner.ComCategories;
using Miner.Interop;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Framework;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using System.Collections.Generic;

namespace PGE.Desktop.EDER.ValidationRules
{
    [ComVisible(true)]
    [Guid("40546C60-2BC2-4B27-AA84-53D82F310A72")]
    [ProgId("PGE.Desktop.EDER.ValidateMap")]
    [ComponentCategory(ComCategory.MMValidationRules)]


    public class ValidateMap : BaseValidationRule
    {
        #region Private
        /// <summary>
        /// constant error message
        /// </summary>

        private const string _errMessage = "Feature not associated with Grid";

        private const string _mapDistModelName = SchemaInfo.Electric.ClassModelNames.DistributionMap;
        private const string _validationModelName = SchemaInfo.Electric.FieldModelNames.ElecMapNumber;

        /// <summary>
        /// logger to log all the information, warning and errors.
        /// </summary>
        private static readonly PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        #endregion Private

        #region Constructor
        public ValidateMap()
            : base("PGE Validate Map", _validationModelName)
        {
        }
        #endregion Constructor

        //public stdole.IPictureDisp Bitmap
        //{
        //    get { return null; }
        //}



        #region Overrides for validation rule
        protected override ID8List InternalIsValid(IRow pRow)
        {
            IFeature pFeature = null;
            IFeatureClass mapGridFeatureClass = null;
            IFeatureCursor featureCursor = null;
            IFeature mapGridFeature = null;

            try
            {
                pFeature = pRow as IFeature;

                //Bug#14313
                int mapNoElecFieldIdx = ModelNameFacade.FieldIndexFromModelName(pFeature.Class, _validationModelName); 
                object mapNoElecFieldValue = pFeature.get_Value(mapNoElecFieldIdx);
                if (mapNoElecFieldValue != null)
                {
                    string value = Convert.ToString(mapNoElecFieldValue);

                    switch (value)
                    {

                        case PopulateDistMapNo.DUPMAP_MAPNUMBER:
                            AddError("DUP Map – The feature is associated with more than one map grid.");
                            break;
                        case PopulateDistMapNo.NOMAP_MAPNUMBER:
                            AddError("No Map -- Feature is not associated with map grid.");
                            break;
                    }
                }
                else
                {
                    AddError("Null Map - Feature is not associated with a map number.");
                }

                /*
                    // Commented because the logic has been changed due to Bug#14313
                    //===============================================================

                mapGridFeatureClass = ModelNameFacade.FeatureClassByModelName(((IDataset)pFeature.Class).Workspace, _mapDistModelName);
                ISpatialFilter spatialFilter = new SpatialFilterClass();
                spatialFilter.Geometry = (IGeometry)((IFeature)pRow).Shape;
                spatialFilter.GeometryField = mapGridFeatureClass.ShapeFieldName;
                spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                
                featureCursor = mapGridFeatureClass.Search(spatialFilter, true);
                mapGridFeature = featureCursor.NextFeature();

                int mapNumberFieldIndex = ModelNameFacade.FieldIndexFromModelName(pFeature.Class as IObjectClass, SchemaInfo.Electric.FieldModelNames.ElecMapNumber);


                if (mapGridFeature == null)
                {
                    AddError("No Map -- Feature is not associated with map grid.");
                }
                else
                {

                    object fieldValue = pFeature.get_Value(mapNoElecFieldIdx);
                    if ((fieldValue == null) || (fieldValue.ToString() == string.Empty)) //Bug#13137
                    {
                        AddError("Null Map - Feature is not associated with a map number.");
                    }

                }

                while ((mapGridFeature = featureCursor.NextFeature()) != null)
                {
                    AddError("DUP Map – The feature is associated with more than one map grid.");
                    break;
                }

                 */
            }
            catch (Exception ex)
            {
                _logger.Error("PGE Validate Map validation rule failed. Message: " + ex.Message + " Stacktrace: " + ex.StackTrace);
            }
            finally
            {
                //if (pFeature != null) { while (Marshal.ReleaseComObject(pFeature) > 0) { } }
                if (mapGridFeatureClass != null) { while (Marshal.ReleaseComObject(mapGridFeatureClass) > 0) { } }
                if (featureCursor != null) { while (Marshal.ReleaseComObject(featureCursor) > 0) { } }
                if (mapGridFeature != null) { while (Marshal.ReleaseComObject(mapGridFeature) > 0) { } }
            }

            return _ErrorList;

        }


        /// <summary>
        /// Returns true if "PGE_VALIDATEMAPGRID" Class ModelName exist on Objectclass. 
        /// </summary>
        /// <param name="param">the ObjectClass</param>
        /// <returns></returns>
        //protected override bool EnableByModelNames(object param)
        //{
        //    if (!(param is IObjectClass))
        //    {
        //        _logger.Debug("Parameter is not type of IObjectClass, exiting");
        //        return false;
        //    }

        //    IObjectClass oclass = param as IObjectClass;
        //    _logger.Debug("ObjectClass:" + oclass.AliasName);

        //    //Check if ClassModelName exist on current ObjectClass fields
        //    bool enableForClassModel = ModelNameFacade.ContainsClassModelName(oclass, _validationModelName);
        //    _logger.Debug("ClassModelName:" + _validationModelName + ", in ObjectClass :" + oclass.AliasName + "exist(" + enableForClassModel + ")");

        //    _logger.Debug(string.Format("Returning Visible:{0}", enableForClassModel));

        //    return (enableForClassModel);
        //}

        #endregion Overrides for validation rule

    }
}
