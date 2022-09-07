using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Miner.ComCategories;
using PGE.Common.Delivery.ArcFM;
using Miner.Interop;
using ESRI.ArcGIS.Geodatabase;
using PGE.Common.Delivery.Framework;
using ESRI.ArcGIS.esriSystem;
using System.Collections;
using System.Data.OleDb;
using System.Data;
using Miner.Interop.Process;
using System.Reflection;

namespace PGE.Desktop.EDER.ValidationRules.PLC
{
    [ComVisible(true)]
    [Guid("c5dbab34-e98d-4eaa-810b-95365f608ba7")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("PGE.Desktop.EDER.ValidationRules.PLC.ValidateNullPLDBID")]
    [ComponentCategory(ComCategory.MMValidationRules)]
    public class ValidateNullPLDBID : BaseValidationRule
    {
        private static readonly PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        public ValidateNullPLDBID()
            : base("PGE Validate PLDBID", SchemaInfo.Electric.ClassModelNames.SupportStructure)
        { }

        private const int SUBTYPE_POLE = 1;
        private const int SUBTYPE_GUY_STUB = 4;
        private const int SUBTYPE_PUSH_BRACE = 5;
        private const int SUBTYPE_POLE_STUB = 8;
        private const string CUST_OWNED_NO = "N"; 

        protected override ID8List InternalIsValid(IRow row)
        {
            try
            {
                var obj = row as IObject;
                if (obj == null)
                    return base._ErrorList;

                //Get the PLDBID field Index 
                int idxPLDBID = ModelNameFacade.FieldIndexFromModelName( 
                    row.Table as IObjectClass, 
                    SchemaInfo.Electric.FieldModelNames.PLDBID);
                int idxCustomerOwned = ModelNameFacade.FieldIndexFromModelName(
                    row.Table as IObjectClass,
                    SchemaInfo.Electric.FieldModelNames.CustomerOwned);

                //Get the subtype field index 
                int idxSubtype = -1;
                int subtypeCd = -1;
                string custOwned = string.Empty;
                long pldbid = -1; 
                ISubtypes pSubTypes = (ISubtypes)row.Table;
                if (pSubTypes != null)
                    idxSubtype = pSubTypes.SubtypeFieldIndex;

                //Check for null PLDBID 
                if (idxPLDBID != -1)
                {
                    //Check the subtype code, as the PLDBID cannot be 
                    //null for customerowned pole with subtype: 
                    //Pole:         subtypecode = 1 
                    //Guy Stub:     subtypecode = 4 
                    //Push Brace:   subtypecode = 5 
                    //Pole Stub:    subtypecode = 8 
                    if (row.get_Value(idxSubtype) != DBNull.Value)
                        subtypeCd = Convert.ToInt32(row.get_Value(idxSubtype));
                    if (row.get_Value(idxCustomerOwned) != DBNull.Value)
                        custOwned = row.get_Value(idxCustomerOwned).ToString().ToUpper();
                    if (row.get_Value(idxPLDBID) != DBNull.Value)
                        pldbid = Convert.ToInt64(row.get_Value(idxPLDBID));

                    if ((custOwned == CUST_OWNED_NO) &&
                        ((subtypeCd == SUBTYPE_POLE) || (subtypeCd == SUBTYPE_GUY_STUB) ||
                        (subtypeCd == SUBTYPE_PUSH_BRACE) || (subtypeCd == SUBTYPE_POLE_STUB)))
                    {
                        //must have a non-null value
                        if (pldbid == -1)
                            base.AddError("The PLDBID MUST not be null for non-customer owned Poles of this subtype, please be sure to use the correct tool");
                    }

                    //Check for uniqueness 
                    if (pldbid != -1)
                    {
                        IQueryFilter pQF = new QueryFilterClass();
                        pQF.WhereClause = row.Fields.get_Field(idxPLDBID).Name + " = " + pldbid;
                        if (row.Table.RowCount(pQF) > 1)
                            base.AddError("The PLDBID MUST be unique, there is at least one other Pole with PLDBID = " + 
                                pldbid.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Warn("Error occurred while validating PLDBID rule: " + ex.Message);
            }
           
            return base._ErrorList;
        }
    }
}
