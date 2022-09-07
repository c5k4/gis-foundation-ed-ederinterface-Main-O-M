
// ========================================================================
// Copyright © 2006 Telvent, Consulting Engineers, Inc.
// <history>
// Status-based Field Required - EDER Component Specification
// Shaikh Rizuan 10/10/2012	Created
// </history>
// All rights reserved.
// ========================================================================
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geodatabase;
using log4net;
using Miner.ComCategories;
using Miner.Interop;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Framework;
using PGE.Common.Delivery.Diagnostics;
using System.Collections.Generic;

namespace PGE.Desktop.EDER.ValidationRules
{
    /// <summary>
    /// Validate InstallationDate and InstallJobYear field that is present on many feature Classes and objects.
    /// </summary>
    [ComVisible(true)]
    [Guid("BD1F49CD-A6CF-4716-803C-F369C54EADF3")]
    [ProgId("PGE.Desktop.EDER.ValidateFieldsInServiceStatus")]
    [ComponentCategory(ComCategory.MMValidationRules)]
    public class ValidateFieldsInServiceStatus : BaseValidationRule
    {
        #region Class Variables
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        private const string FieldNameInService = "IN SERVICE";

        private const string WarnMissingModelNames = "Missing object class or field model names:\r\n{0}";
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor.
        /// </summary>
        public ValidateFieldsInServiceStatus()
            : base("PGE Validate Fields InService status", SchemaInfo.Electric.ClassModelNames.InServiceClsModelName)
        {
        }
       #endregion

        #region Base Validation Rule Overrides
        /// <summary>
        /// Determines if the specified parameter is an object class that has been configured with a class model name identified
        /// in the _modelNames array.
        /// </summary>
        /// <param name="param">The object class to validate.</param>
        /// <returns>Boolean indicating if the specified object class has any of the appropriate model name(s).</returns>
        protected override bool EnableByModelNames(object param)
        {
            if (base.EnableByModelNames(param))
            {
                return true;
            }

            _logger.Warn(string.Format(WarnMissingModelNames, _ModelNames.Concatenate("\r\n")));
            return false;
        }
        /// Validate the row.
        /// </summary>
        /// <param name="row"> The row data being validated with Generator table </param>
        /// <returns> A list of errors or nothing. </returns>
        /// 
        protected override ID8List InternalIsValid(IRow row)
        {

            IObject Obj = row as IObject;
            string fieldsName = string.Empty;
            string errorMsg=string.Empty;
            //get array of field model names
            string[] strFldModelNames = new string[] { SchemaInfo.Electric.FieldModelNames.InstallationDate, SchemaInfo.Electric.FieldModelNames.InstallationJobYear, SchemaInfo.Electric.FieldModelNames.SerialNumber, SchemaInfo.Electric.FieldModelNames.Manufacturer, SchemaInfo.Electric.FieldModelNames.DistMap };
            int statusFldIndex= ModelNameFacade.FieldIndexFromModelName(Obj.Class,SchemaInfo.Electric.FieldModelNames.Status);
            //int[] intArray=new int[strFldModelNames.Length];

           
            if(statusFldIndex !=-1)
            {
                //get stattus field value
                object fieldDescription = Obj.GetFieldValue(null, true, SchemaInfo.Electric.FieldModelNames.Status).Convert<object>(System.DBNull.Value);
                _logger.Debug("Status Field Value: " + fieldDescription);
                if (fieldDescription != System.DBNull.Value && fieldDescription !=null && FieldNameInService.Equals(fieldDescription.Convert<string>().ToUpper())) //if status field value is not null
                {
                    //loop through model names
                    for (int k = 0; k < strFldModelNames.Length; k++)
                    {
                        //get fied Index
                        int fldIndex = ModelNameFacade.FieldIndexFromModelName(Obj.Class, strFldModelNames[k]);
                        _logger.Debug(strFldModelNames[k] + " = " + fldIndex);
                       
                        if (fldIndex != -1)//if model name assigned
                        {
                            //if field value is null add error message
                            if (Obj.get_Value(fldIndex) == System.DBNull.Value && string.IsNullOrEmpty(Obj.get_Value(fldIndex).ToString()))
                            {
                                AddError(Obj.Fields.Field[fldIndex].AliasName + " field cannot be <Null> when the status is In Service.");
                            }
                        }
                        else
                        {
                            IRelationshipClass relClass = null;
                            try
                            {
                                //if field model name not found
                                relClass = ModelNameFacade.RelationshipClassFromModelName(Obj.Class, esriRelRole.esriRelRoleOrigin, SchemaInfo.Electric.ClassModelNames.PGEInServiceChild);
                            }
                            catch (Exception ex)
                            {
                                _logger.Info(ex.Message);
                            }

                            if (relClass == null)
                            {
                                _logger.Warn(string.Format(WarnMissingModelNames, strFldModelNames[k]));
                                continue;
                            }
                            else
                            {
                                _logger.Info(string.Format(WarnMissingModelNames, strFldModelNames[k]) + " Checking on Child.");
                            }
                            IObjectClass relatedObjectClass = relClass.DestinationClass;
                            fldIndex = ModelNameFacade.FieldIndexFromModelName(relatedObjectClass, strFldModelNames[k]);
                            if (fldIndex != -1)
                            {
                                IEnumerable<IObject> relatedObjectsEnum = Obj.GetRelatedObjects(null, esriRelRole.esriRelRoleOrigin, SchemaInfo.Electric.ClassModelNames.PGEInServiceChild);
                                foreach(IObject childObj in relatedObjectsEnum)
                                {
                                    //if field value is null add error message
                                    if (childObj.get_Value(fldIndex) == System.DBNull.Value && string.IsNullOrEmpty(childObj.get_Value(fldIndex).ToString()))
                                    {
                                        AddError(relatedObjectClass.AliasName + " (ObjectID: " + childObj.OID + ") " + childObj.Fields.Field[fldIndex].AliasName + " field cannot be <Null> when the status is In Service.");
                                    }
                                }
                            }
                            else
                            {
                                _logger.Warn(string.Format(WarnMissingModelNames, strFldModelNames[k])+" Child also doesnt have this field model name.");
                            }
                        }
                    }
                }
            }
            else
            {
                _logger.Warn(string.Format(WarnMissingModelNames, SchemaInfo.Electric.FieldModelNames.Status));
            }
            return _ErrorList;
        }


        #endregion

    }
}
