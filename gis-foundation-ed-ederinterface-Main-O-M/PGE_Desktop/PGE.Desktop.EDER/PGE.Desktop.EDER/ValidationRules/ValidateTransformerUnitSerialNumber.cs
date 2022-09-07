using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

using Miner.Interop;
using Miner.Geodatabase;
using Miner.ComCategories;

using PGE.Common.Delivery.Framework;
using PGE.Common.Delivery.ArcFM;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;

namespace PGE.Desktop.EDER.ValidationRules
{
    [ComVisible(true)]
    [Guid("d75de609-0dec-4d86-8ce3-0c066c3ef69b")]
    [ProgId("PGE.Desktop.EDER.ValidateTransformerUnitSerialNumber")]
    [ComponentCategory(ComCategory.MMValidationRules)]
    public class ValidateTransformerUnitSerialNumber : BaseValidationRule
    {

        /// <summary>
        /// Constructor calls the Base constructor with PGE Validate Transformer Serial Number as the name of validation Rule and Transformer as the modelname.
        /// 
        /// </summary>
        public ValidateTransformerUnitSerialNumber()
            : base("PGE Validate TransformerUnit Serial Number", SchemaInfo.Electric.ClassModelNames.TransformerUnit)
        {

        }

        protected override ID8List InternalIsValid(IRow row)
        {
            // Nuisance rule modification (only works when version used, not map selection).
            // This rule should only run on inserts.
            if (!CommonValidate.ExcludeRule("",esriDifferenceType.esriDifferenceTypeInsert, row))
            {
                return _ErrorList;
            }

            var obj = row as IFeature;
            if (row == null)
            {
                return _ErrorList;
            }

            //Check if the serial number is null or empty string 
            bool hasSerialNumber = false;
            int serialNumFldIndex = ModelNameFacade.FieldIndexFromModelName(
                    (IObjectClass)row.Table, SchemaInfo.Electric.FieldModelNames.SerialNumber);
            if (serialNumFldIndex != -1)
            {
                if (row.get_Value(serialNumFldIndex) != DBNull.Value)
                {
                    if (row.get_Value(serialNumFldIndex).ToString().Trim() != string.Empty)
                        hasSerialNumber = true;
                }
            }

            //Check if the construction status is In Service 
            if (!hasSerialNumber)
            {
                int statusFldIndex = ModelNameFacade.FieldIndexFromModelName(
                    (IObjectClass)row.Table, SchemaInfo.Electric.FieldModelNames.Status);
                object statusVal = row.get_Value(statusFldIndex);
                if (statusVal != System.DBNull.Value && !string.IsNullOrEmpty(statusVal.ToString()))
                {
                    //In service 
                    if ((int.Parse(statusVal.ToString())) == 5)
                    {
                        //Error condition has been reached 
                        AddError("In Service transformerunit must have a serial number");
                    }
                }
            }

            return _ErrorList;
        }
    }
}