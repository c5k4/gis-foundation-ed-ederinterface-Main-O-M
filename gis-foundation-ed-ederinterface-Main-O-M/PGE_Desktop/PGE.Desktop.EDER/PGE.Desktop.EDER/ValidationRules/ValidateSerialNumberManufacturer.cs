// ========================================================================
// Copyright © 2006 Telvent, Consulting Engineers, Inc.
// <history>
// ED0006 - SAP Integration Components
// Shaikh Rizuan 10/30/2012	Created
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
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PGE.Common.Delivery.Diagnostics;

namespace PGE.Desktop.EDER.ValidationRules
{
    /// <summary>
    /// Validate Serail Number and Manufacturer field that is present on many feature Classes and objects.
    /// </summary>
    [ComVisible(true)]
    [Guid("DDFBDF5C-8481-430B-A5A1-A9A7CF67CEBD")]
    [ComponentCategory(ComCategory.MMValidationRules)]
    public class ValidateSerialNumberManufacturer : BaseValidationRule
    {
        #region Class Variables
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        private static string _fieldWarningMsg = "Class Model name :{0} not found.";
        private static string _errorMsg = "{0} field cannot set to 'UNSPECIFIED' when the feature’s status is set to In Service.";
        private static string _duplicateErrorMsg = "{0} already contains an asset (OID: {1}) with Serial Number: {2} and Manufacturer: {3}.";
        private static string _fieldNullErrorMsg = "{0} field Cannot be empty or <Null>.";
        private static string _manuUnspcifiedValue = "XX";

        private const string WarnMissingModelNames = "Missing object class or field model names:\r\n{0}";
        #endregion


        #region Constructors
        /// <summary>
        /// Constructor.
        /// </summary>
        public ValidateSerialNumberManufacturer()
            : base("PGE Validate Serial Number Manufacturer", SchemaInfo.Electric.ClassModelNames.AssetInformation)
        {
        }
        #endregion

        #region Base Validation Rule Overrides
        /// <summary>
        /// Determines if the specified parameter is an object class that has been configured with a class model name identified
        /// in the _modelNames array.
        /// 
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
            //If this rule is being filtered out - do not run it 
            //if (!ValidationFilterEngine.Instance.IsQAQCRuleEnabled(_Name, base.Severity))
            //    return _ErrorList; 

            // Nuisance rule modification (only works when version used, not map selection).
            // This rule should only run on inserts.
            if (!CommonValidate.ExcludeRule("", esriDifferenceType.esriDifferenceTypeInsert, row))
            {
                _logger.Debug("Serial Number Manufacturer Validation excluded based on insert only.");
                return _ErrorList;
            }

            IObject Obj = row as IObject;
            #region Get Field and Field Index
            int manufactureFldIndx = ModelNameFacade.FieldIndexFromModelName(Obj.Class, SchemaInfo.Electric.FieldModelNames.Manufacturer);
            int statusFldIndx = ModelNameFacade.FieldIndexFromModelName(Obj.Class, SchemaInfo.Electric.FieldModelNames.Status);
            int serialNumIndex = ModelNameFacade.FieldIndexFromModelName(Obj.Class, SchemaInfo.Electric.FieldModelNames.SerialNumber);
            IField manuFld = ModelNameFacade.FieldFromModelName(Obj.Class, SchemaInfo.Electric.FieldModelNames.Manufacturer);
            IField SerialNumFld = ModelNameFacade.FieldFromModelName(Obj.Class, SchemaInfo.Electric.FieldModelNames.SerialNumber);
            #endregion

            if (manufactureFldIndx != -1 && statusFldIndx != -1)//if field is not null
            {
                //get manufacture field value
                object manufactureVal = Obj.get_Value(manufactureFldIndx);
                //get status field value
                object statusVal = Obj.get_Value(statusFldIndx);
                if (statusVal != System.DBNull.Value && !string.IsNullOrEmpty(statusVal.ToString()))//if status field value is not null
                {
                    if ((int.Parse(statusVal.ToString())) == 5)
                    {
                        if (manufactureVal != System.DBNull.Value && !string.IsNullOrEmpty(manufactureVal.ToString()))//if manufacture field value is not null
                        {
                            if (manufactureVal.ToString() == _manuUnspcifiedValue)//if matching
                            {
                                AddError(string.Format(_errorMsg, manuFld.AliasName));

                            }
                            //else
                            //{
                            if (serialNumIndex != -1)//if field is not null
                            {
                                //get serial number field value
                                object serialNumVal = Obj.get_Value(serialNumIndex);

                                _logger.Debug("Serial Number value= " + serialNumVal);
                                string objID = string.Empty;
                                if (serialNumVal != DBNull.Value)//if vlaue not null
                                {
                                    // manufactureVal = pObj.GetFieldValue(manuFld.Name, true, null);
                                    //Check for duplicate values
                                    bool isDuplicateValue = ISDuplicate(Obj, manuFld, manufactureVal, SerialNumFld, serialNumVal, out objID);

                                    if (isDuplicateValue) //if duplicate value found
                                    {

                                        object ManuDesc;// =string.Empty;
                                        object serialNumDesc;// = string.Empty;
                                        if (manuFld.Domain != null)//if domain is not assigned to Manufacture field
                                        {
                                            ManuDesc = Obj.GetFieldValue(manuFld.Name, true, SchemaInfo.Electric.FieldModelNames.Manufacturer).Convert<object>(System.DBNull.Value); //GetDomainDescriptionFromObject(pObj, manufactureFldIndx, out errorMsg);
                                            _logger.Debug("Manufactur domain name: " + ManuDesc);
                                            if (SerialNumFld.Domain != null) //if domain is not assigned to Serial number field
                                            {
                                                serialNumDesc = Obj.GetFieldValue(SerialNumFld.Name, true, SchemaInfo.Electric.FieldModelNames.SerialNumber).Convert<object>(System.DBNull.Value); //GetDomainDescriptionFromObject(pObj, serialNumIndex, out errorMsg);
                                                _logger.Debug("Seral number domain name" + serialNumDesc);
                                                if (ManuDesc != System.DBNull.Value && ManuDesc != null && serialNumDesc != System.DBNull.Value && serialNumDesc != null)//if not null
                                                {
                                                    AddError(string.Format(_duplicateErrorMsg, Obj.Class.AliasName, objID, serialNumDesc.ToString(), ManuDesc.ToString()));
                                                }

                                            }
                                            else
                                            {
                                                AddError(string.Format(_duplicateErrorMsg, Obj.Class.AliasName, objID, serialNumVal.ToString(), ManuDesc.ToString()));
                                            }

                                        }


                                    }
                                }
                                else
                                {
                                    AddError(string.Format(_fieldNullErrorMsg, SerialNumFld.AliasName));

                                }
                            }
                            else
                            {
                                _logger.Warn(string.Format(_fieldWarningMsg, SchemaInfo.Electric.FieldModelNames.SerialNumber));

                            }
                            //}
                        }
                        else
                        {
                            AddError(string.Format(_fieldNullErrorMsg, manuFld.AliasName));

                        }
                    }
                }
                else
                {
                    AddError(string.Format(_fieldNullErrorMsg, "Status"));
                }
            }
            else
            {
                if (manufactureFldIndx == -1)
                    _logger.Warn(string.Format(_fieldWarningMsg, SchemaInfo.Electric.FieldModelNames.Manufacturer));
                if (statusFldIndx == -1)
                    _logger.Warn(string.Format(_fieldWarningMsg, SchemaInfo.Electric.FieldModelNames.Status));

            }
            return _ErrorList;
        }
        #endregion
        #region Private Methods
        private bool ISDuplicate(IObject obj, IField manuFld, object manufactureVal, IField SerialNumFld, object serialNumVal, out string objID)
        {
            ICursor cusor = null;
            objID = string.Empty;
            List<string> getObjID = new List<string>();
            //int i = 0;
            try
            {
                //get table
                ITable table = obj.Class as ITable;
                IQueryFilter queryFiltr = new QueryFilterClass();
                //passing where clause where [manufacture] = <value> and [serialnumber]=<value>
                queryFiltr.WhereClause = manuFld.Name + " ='" + manufactureVal + "' AND " + SerialNumFld.Name + " ='" + serialNumVal + "'";
                //get cursor
                cusor = table.Search(queryFiltr, false);
                IRow row;

                var stringArray = new string[] { };
                // var items = new string[]{};
                //loop to get row
                while ((row = cusor.NextRow()) != null)
                {

                    // getObjID.Aggregate(a => pRow.OID.ToString());
                    if (row.OID != obj.OID)
                    {
                        getObjID.Add(row.OID.ToString());
                    }

                }
            }
            finally
            {
                //release COM Object
                while (Marshal.ReleaseComObject(cusor) > 0) { }
                cusor = null;
            }
            if (getObjID.Count > 0) // if count >0 means duplicacy exists
            {
                objID = string.Join(",", getObjID.ToArray());
                return true;
            }
            return false;
        }

        #endregion
    }
}