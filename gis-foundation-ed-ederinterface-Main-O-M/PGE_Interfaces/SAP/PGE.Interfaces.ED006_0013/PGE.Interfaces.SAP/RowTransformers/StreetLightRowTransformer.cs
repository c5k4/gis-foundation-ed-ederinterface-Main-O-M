using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using PGE.Common.ChangeDetectionAPI;
using PGE.Common.Delivery.Framework;
using PGE.Interfaces.Integration.Framework;

namespace PGE.Interfaces.SAP.RowTransformers
{
     public class StreetLightRowTransformer : SAPDeviceRowTransformer 
     
    {
         /// <summary>
         /// return Y if the configured field has value like Cust Owned...
         /// return N if the configured field has other value 
         /// return "" if there is no configured field or the configured field is not found
         /// </summary>
         /// <param name="row"></param>
        /// <returns>return Y or N or "" as if customer owned indicator.</returns>
         protected override string GetCustomerOwnedValue(IRow row)
         {
             Settings settings = _trackedClass.Settings;
             if (settings != null && !string .IsNullOrEmpty( settings["CustomerOwnedFilterName"] ) )
             {
                 int fieldIndex = -1;

                 string customerOwnedFilterName = settings["CustomerOwnedFilterName"];

                 ITable tbl = row.Table;
                 fieldIndex = BaseRowTransformer.GetFieldIndex((IObjectClass)tbl, customerOwnedFilterName);


                 if (fieldIndex == -1)
                 {
                    GISSAPIntegrator._errorMessage.AppendLine(row.OID + " Customer Owned not found " + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                    GISSAPIntegrator._logger.Info("OID :: " + row.OID + " Customer Owned not found ");
                    GISSAPIntegrator._logger.Error("OID :: " + row.OID + " Customer Owned not found ");
                    //CusomerOwned field not found
                    return "";
                 }

                 string fieldValue = row.get_Value((int)fieldIndex).ToString();

                 IObjectClass cls = tbl as IObjectClass;
                 ICodedValueDomain domain = cls.Fields.get_Field(fieldIndex).Domain as ICodedValueDomain ;

                 string codeDescription = "";
                 for (int i = 0; i < domain.CodeCount; i++)
                 {
                     if (domain.get_Value(i).ToString().ToUpper() == fieldValue.ToUpper())
                     {
                         codeDescription = domain.get_Name(i);
                         break;
                     }

                 }

                 string customerOwnedFilterCriteria = settings["CustomerOwnedFilterCriteria"];

                 return codeDescription.ToUpper ().Contains(customerOwnedFilterCriteria.ToUpper ()) ? "Y" : "N";
             }
            GISSAPIntegrator._errorMessage.AppendLine(row.OID + " Customer Owned not found " + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
            GISSAPIntegrator._logger.Info("OID :: " + row.OID + " Customer Owned not found ");
            GISSAPIntegrator._logger.Error("OID :: " + row.OID + " Customer Owned not found ");
            return "";
         }

        /// <summary>
        /// return Y if the configured field has value like Cust Owned...
        /// return N if the configured field has other value 
        /// return "" if there is no configured field or the configured field is not found
        /// </summary>
        /// <param name="row"></param>
        /// <returns>return Y or N or "" as if customer owned indicator.</returns>
        protected override string GetCustomerOwnedValue(DeleteFeat row)
        {
            Settings settings = _trackedClass.Settings;
            if (settings != null && !string.IsNullOrEmpty(settings["CustomerOwnedFilterName"]))
            {
                int fieldIndex = -1;

                string customerOwnedFilterName = settings["CustomerOwnedFilterName"];

                ITable tbl = row.Table;
                fieldIndex = BaseRowTransformer.GetFieldIndex((IObjectClass)tbl, customerOwnedFilterName);


                if (fieldIndex == -1)
                {
                    GISSAPIntegrator._errorMessage.AppendLine(row.OID + " Customer Owned not found " + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                    GISSAPIntegrator._logger.Info("OID :: " + row.OID + " Customer Owned not found ");
                    GISSAPIntegrator._logger.Error("OID :: " + row.OID + " Customer Owned not found ");
                    //CusomerOwned field not found
                    return "";
                }

                string fieldValue = string.Empty;//row.get_Value((int)fieldIndex).ToString();

                if (row.fields_Old.ContainsKey(customerOwnedFilterName.ToUpper()))
                    fieldValue = row.fields_Old[customerOwnedFilterName.ToUpper()];

                IObjectClass cls = tbl as IObjectClass;
                ICodedValueDomain domain = cls.Fields.get_Field(fieldIndex).Domain as ICodedValueDomain;

                string codeDescription = "";
                for (int i = 0; i < domain.CodeCount; i++)
                {
                    if (domain.get_Value(i).ToString().ToUpper() == fieldValue.ToUpper())
                    {
                        codeDescription = domain.get_Name(i);
                        break;
                    }

                }

                string customerOwnedFilterCriteria = settings["CustomerOwnedFilterCriteria"];

                return codeDescription.ToUpper().Contains(customerOwnedFilterCriteria.ToUpper()) ? "Y" : "N";
            }
            GISSAPIntegrator._errorMessage.AppendLine(row.OID + " Customer Owned not found " + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
            GISSAPIntegrator._logger.Info("OID :: " + row.OID + " Customer Owned not found ");
            GISSAPIntegrator._logger.Error("OID :: " + row.OID + " Customer Owned not found ");
            return "";
        }
    }
}
