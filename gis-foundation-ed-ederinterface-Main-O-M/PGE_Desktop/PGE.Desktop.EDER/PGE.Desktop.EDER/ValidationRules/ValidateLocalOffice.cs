using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geodatabase;
using Miner.ComCategories;
using Miner.Interop;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Framework;

namespace PGE.Desktop.EDER.ValidationRules
{
    [ComVisible(true)]
    [Guid("CF6A84A3-5116-4FD0-BCC8-F5EEA6EB3B92")]
    [ProgId("PGE.Desktop.EDER.ValidateLocalOffice")]
    [ComponentCategory(ComCategory.MMValidationRules)]

    public class ValidateLocalOffice: BaseValidationRule
    {

        #region private

        private const string _errMsg = "The feature ({0} OID: {1}) does not have the correct local office ID assigned.";
        //Bug#14312
        private const string _fieldModelName = SchemaInfo.Electric.FieldModelNames.ElecMapNumber;// SchemaInfo.Electric.FieldModelNames.LocalOfficeID;
        private static readonly PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        /// <summary>
        /// Returns the field model names for which this validation rule is defined for
        /// </summary>
        /// <returns></returns>
        #endregion private

        #region Constructors
        public ValidateLocalOffice()
            : base("PGE Validate Local Office", _fieldModelName)
        {
        }
        #endregion Constructors

        #region Overrides for validate regional attributes

        /// <summary>
        /// Validation rule to perform check as per defined rule.
        /// </summary>
        /// <param name="row">The Row being validated (QA/QC).</param>
        /// <returns></returns>
        protected override ID8List InternalIsValid(IRow row)
        {
            //Cast IRow to IObject and check for valid casting by checking null value
            IObject rowObject = row as IObject;
            if (rowObject != null)
            {
                IObjectClass rowObjectClass = rowObject.Class;
                string featureClassName = rowObjectClass.AliasName;
                int OID = row.HasOID ? row.OID : -1;

                //*******************************************************************

                IFeatureCursor featCursor = null;
                IFeature localOfficePolygon = null;

                try
                {
                    //prepare error message
                    string errMsg = string.Format(_errMsg, featureClassName, OID == -1 ? "" : OID + "");

                    IFeature pFeature = rowObject as IFeature;
                    int mapNoElecFieldIdx = ModelNameFacade.FieldIndexFromModelName(pFeature.Class, _fieldModelName); //Bug#14312
                    if (mapNoElecFieldIdx != -1)
                    {
                        object mapNoElecFieldValue = pFeature.get_Value(mapNoElecFieldIdx);
                        if (mapNoElecFieldValue != null)
                        {
                            string value = Convert.ToString(mapNoElecFieldValue);
                            if (value == PopulateDistMapNo.WRONGLO_MAPNUMBER)
                            {
                                _logger.Debug(errMsg);
                                AddError(errMsg);
                            }
                        }
                    }
                    else
                    {
                        _logger.Debug(_fieldModelName + " field model names doesnt exist on " + rowObjectClass.AliasName + ".");
                    }
                    /*
                    // Commented because the logic has been changed due to Bug#14312
                    //===============================================================
                     
                 
                    IFeatureClass localOfficeFeatureClass = ModelNameFacade.FeatureClassByModelName(((IDataset)pFeature.Class).Workspace, SchemaInfo.General.ClassModelNames.LOPC);

                    //prepare error message
                    string errMsg = string.Format(_errMsg, featureClassName, OID == -1 ? "" : OID + "");

                    //Set the query for spatial filter
                    ISpatialFilter spatialFilter = new SpatialFilterClass();
                    spatialFilter.Geometry = pFeature.ShapeCopy;
                    spatialFilter.GeometryField = localOfficeFeatureClass.ShapeFieldName;
                    spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                    featCursor = localOfficeFeatureClass.Search(spatialFilter, true);
                    localOfficePolygon = featCursor.NextFeature();
                    if (localOfficePolygon != null)
                    {
                        int locOfficeIndex = ModelNameFacade.FieldIndexFromModelName(localOfficeFeatureClass, _fieldModelName);
                        int featureLocOfficeFieldIndex = ModelNameFacade.FieldIndexFromModelName(pFeature.Class, _fieldModelName);

                        if ((locOfficeIndex == -1) || (featureLocOfficeFieldIndex == -1))
                        {
                            _logger.Debug(_fieldModelName + " field model names doesnt exist on " + rowObjectClass.AliasName + ".");
                        }

                        string locOfficeID = localOfficePolygon.get_Value(locOfficeIndex) as string;
                        string featureLocOfficeID = pFeature.get_Value(featureLocOfficeFieldIndex) as string;

                        if (locOfficeID != featureLocOfficeID)
                        {
                            _logger.Debug(errMsg);
                            AddError(errMsg);
                        }
                    }
                     
                     */
                }
                catch (Exception e)
                {
                    _logger.Error("PGE Validate Local Office validation rule failed. Message: " + e.Message + " Stacktrace: " + e.StackTrace);
                }
                finally
                {
                    if (localOfficePolygon != null) { while (Marshal.ReleaseComObject(localOfficePolygon) > 0) { } }
                    if (featCursor != null) { while (Marshal.ReleaseComObject(featCursor) > 0) { } }
                }
            }

            return _ErrorList;
        }

        /// <summary>
        /// Returns true if "LocalOfficeID" Field ModelName exist on Objectclass. 
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

        //    //Check if FieldModelName exist on current ObjectClass fields
        //    bool enableForFieldModel = ModelNameFacade.ContainsFieldModelName(oclass, _fieldModelName);
        //    _logger.Debug("FieldModelName:" + _fieldModelName + ", in ObjectClass :" + oclass.AliasName + "exist(" + enableForFieldModel + ")");

        //    _logger.Debug(string.Format("Returning Visible:{0}", enableForFieldModel));

        //    return (enableForFieldModel);
        //}
        #endregion Overrides for validate regional attributes
    }
}
