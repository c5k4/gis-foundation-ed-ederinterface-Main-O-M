//ME Q2 2018 DA Item# 21-Prevent Duplicate SAP Equipment ID’s & PLDBID#’s

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Reflection;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using Telvent.Delivery.ArcFM;
using Telvent.Delivery.Diagnostics;
using log4net;
using Miner.Interop;
using Miner.ComCategories;
using Telvent.Delivery.Framework;


namespace Telvent.PGE.ED.Desktop.ValidationRules
{
    [ComVisible(true)]
    [Guid("93C58F69-9403-41D7-A16B-EBAB2C27A6C8")]
    [ProgId("Telvent.PGE.ED.UniqueSapEquipmentId")]
    [ComponentCategory(ComCategory.MMValidationRules)]
    public class UniqueSapEquipmentId : BaseValidationRule
    {
        #region Class Variables
        private string _EquiperrorMessage = "Duplicate SAP EQUIPMENT ID found.";
        private const string _errorMsg = "PGE Validate Unique SAP EQUIPMENT ID: Error in validating unique SAP EQUIPMENT ID: the field with the model name : {0} on the table not assigned or {1} has a <Null> value";
        private static string[] _modelNames = new string[] { SchemaInfo.Electric.ClassModelNames.SapEquipIdClassMn };
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        #endregion

        public UniqueSapEquipmentId()
            : base("PGE Validate Unique SAP EQUIPMENT ID", _modelNames)
        {
        }

        /// <summary>
        /// Flag an error if the SAP EQUIPMENT ID is not unique
        /// </summary>
        /// <param name="row">the record to check</param>
        /// <returns>error message</returns>
        protected override ID8List InternalIsValid(ESRI.ArcGIS.Geodatabase.IRow row)
        {

            IMMModelNameManager mnMgr = Miner.Geodatabase.ModelNameManager.Instance;
            object currentSapEquipIdObject = (row as IObject).GetFieldValue(null, false, SchemaInfo.Electric.FieldModelNames.UNIQUE_SAPEQUIPID);
            if (currentSapEquipIdObject is System.DBNull)
            {
                _logger.Warn(string.Format(_errorMsg, SchemaInfo.Electric.FieldModelNames.UNIQUE_SAPEQUIPID, ((IDataset)row.Table).Name));
                return null;
            }
            _logger.Debug("Sap Equipment Id Field value: " + currentSapEquipIdObject);
            double currentSapEquipId = Convert.ToDouble(currentSapEquipIdObject);

            #region Get Workspace
            IWorkspace ws = null;
            ITable table = row.Table;
            IDataset ds = table as IDataset;
            ws = ds.Workspace;
            #endregion

            bool currentSapEquipIdAlreadyInUse = false;
            //get the object classes from model manager
            IMMEnumObjectClass objectClassesWithModelName = mnMgr.ObjectClassesFromModelNameWS(ws, SchemaInfo.Electric.ClassModelNames.SapEquipIdClassMn);
            objectClassesWithModelName.Reset();
            IObjectClass objectClassWithModelName = objectClassesWithModelName.Next();

            //loop through each classes
            while (objectClassWithModelName != null)
            {
                // if this table is the same as the one that's being validated, there should only be one occurance of the CGC12 value
                // else there should be zero occurances of the value
                IField fieldWithModelName = ModelNameFacade.FieldFromModelName(objectClassWithModelName, SchemaInfo.Electric.FieldModelNames.UNIQUE_SAPEQUIPID); //mnMgr.FieldFromModelName(objectClassWithModelName, SchemaInfo.Electric.FieldModelNames.FieldModelCGC12);
                if (fieldWithModelName != null)
                {
                    // query the table for this field = the current value 
                    QueryFilter qf = new QueryFilter();
                    qf.WhereClause = string.Format("{0} = '{1}'", fieldWithModelName.Name, currentSapEquipIdObject);

                    int matchLimit = 0;

                    if (((IDataset)table).Name.ToUpper() == ((IDataset)objectClassWithModelName).Name.ToUpper())//if true
                    {
                        matchLimit = 1;
                    }
                    _logger.Debug("MatchLimit :" + matchLimit);
                    // get count for matching values
                    int matchCount = ((ITable)objectClassWithModelName).RowCount(qf);
                    _logger.Debug("RowCount: " + matchCount);
                    if (qf != null)
                    {
                        //release com object
                        Marshal.FinalReleaseComObject(qf);
                    }
                    if (matchCount > matchLimit)//if match count greater than match limit i.e. 1
                    {
                        currentSapEquipIdAlreadyInUse = true;
                        break;
                    }
                }
                objectClassWithModelName = objectClassesWithModelName.Next();
            }

            //ID8List list = null;
            if (currentSapEquipIdAlreadyInUse)//if true
            {
                AddError(_EquiperrorMessage);
            }


            return _ErrorList;
        }

    }
}





