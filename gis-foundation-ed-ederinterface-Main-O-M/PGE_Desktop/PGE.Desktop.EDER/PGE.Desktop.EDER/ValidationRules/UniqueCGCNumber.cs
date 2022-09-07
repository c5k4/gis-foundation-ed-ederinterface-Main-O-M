using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Reflection;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Diagnostics;
using log4net;
using Miner.Interop;
using Miner.ComCategories;
using PGE.Common.Delivery.Framework;

namespace PGE.Desktop.EDER.ValidationRules
{
    [ComVisible(true)]
    [Guid("3C71B8FD-3E5E-4171-91E6-0112139F0F1B")]
    [ProgId("PGE.Desktop.EDER.UniqueCGCNumber")]
    [ComponentCategory(ComCategory.MMValidationRules)]
    public class UniqueCGCNumber : BaseValidationRule
    {
        #region Class Variables
        // private string _classModelName = "PGE_CGC12";
        //private string _fieldModelName = "PGE_CGC12";
        //private int _serverity = 8;
        //private int _handle = 0;
        private string _CGCerrorMessage = "Duplicate CGC12 found.";
        private const string _errorMsg = "PGE Validate Unique CGC Number: Error in validating unique CGC number: the field with the model name : {0} on the table not assigned or {1} has a <Null> value";
        private static string[] _modelNames = new string[] { SchemaInfo.Electric.ClassModelNames.CGC12ClassMN };
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        private double _cgcMinValue = 0;
        private double _cgcMaxValue = 0; 


        #endregion

        public UniqueCGCNumber()
            : base("PGE Validate Unique CGC Number", _modelNames)
        {
        }

        /// <summary>
        /// Flag an error if the CGC number is not unique
        /// </summary>
        /// <param name="row">the record to check</param>
        /// <returns>error message</returns>
        protected override ID8List InternalIsValid(ESRI.ArcGIS.Geodatabase.IRow row)
        {

            IMMModelNameManager mnMgr = Miner.Geodatabase.ModelNameManager.Instance;
            object currentCGCObject = (row as IObject).GetFieldValue(null, false, SchemaInfo.Electric.FieldModelNames.FieldModelCGC12);
            if (currentCGCObject is System.DBNull)
            {
                _logger.Warn(string.Format(_errorMsg,SchemaInfo.Electric.FieldModelNames.FieldModelCGC12, ((IDataset)row.Table).Name));
                return null;
            }
            _logger.Debug("CGC12 Field value: " + currentCGCObject);
            double currentCGC = Convert.ToDouble(currentCGCObject);

            #region Get Workspace
            IWorkspace ws = null;
            ITable table = row.Table;
            IDataset ds = table as IDataset;
            ws = ds.Workspace;
            #endregion

            bool currentCGCAlreadyInUse = false;
            //get the object classes from model manager
            IMMEnumObjectClass objectClassesWithModelName = mnMgr.ObjectClassesFromModelNameWS(ws, SchemaInfo.Electric.ClassModelNames.CGC12ClassMN);
            objectClassesWithModelName.Reset();
            IObjectClass objectClassWithModelName = objectClassesWithModelName.Next();

            //loop through each classes
            while (objectClassWithModelName != null)
            {
                // if this table is the same as the one that's being validated, there should only be one occurance of the CGC12 value
                // else there should be zero occurances of the value
                IField fieldWithModelName =ModelNameFacade.FieldFromModelName(objectClassWithModelName,SchemaInfo.Electric.FieldModelNames.FieldModelCGC12); //mnMgr.FieldFromModelName(objectClassWithModelName, SchemaInfo.Electric.FieldModelNames.FieldModelCGC12);
                if (fieldWithModelName != null)
                {
                    // query the table for this field = the current value 
                    QueryFilter qf = new QueryFilter();
                    //whereClause = {CGC12}=<field value>
                    qf.WhereClause = string.Format("{0} = {1}", fieldWithModelName.Name, currentCGCObject);
                    int matchLimit = 0;
                    
                    if (((IDataset)table).Name.ToUpper() == ((IDataset)objectClassWithModelName).Name.ToUpper())//if true
                    {
                        matchLimit = 1;
                    }
                    _logger.Debug("MatchLimit :" + matchLimit);
                    //get count for matching values
                    int matchCount = ((ITable)objectClassWithModelName).RowCount(qf);
                    _logger.Debug("RowCount: " + matchCount);
                    if (qf != null)
                    {
                        //release com object
                        Marshal.FinalReleaseComObject(qf);
                    }
                    if (matchCount > matchLimit)//if match count greater than match limit i.e. 1
                    {
                        currentCGCAlreadyInUse = true;
                        break;
                    }
                }
                objectClassWithModelName = objectClassesWithModelName.Next();
            }

            //ID8List list = null;
            if (currentCGCAlreadyInUse)//if true
            {
                AddError(_CGCerrorMessage);
            }

            //INC3895203 Check the range domain of the CGC number in here 
            IField pCGCField = ModelNameFacade.FieldFromModelName((row as IObject).Class, 
                SchemaInfo.Electric.FieldModelNames.FieldModelCGC12);
            if ((_cgcMinValue == 0) || (_cgcMaxValue == 0))
            {
                if (pCGCField.Domain != null)
                {
                    if (pCGCField.Domain is IRangeDomain)
                    {
                        IRangeDomain pRangeDomain = (IRangeDomain)pCGCField.Domain;
                        if (!double.TryParse(pRangeDomain.MinValue.ToString(), out _cgcMinValue))
                            _cgcMinValue = 0;
                        if (!double.TryParse(pRangeDomain.MaxValue.ToString(), out _cgcMaxValue))
                            _cgcMaxValue = 0;
                    }
                }
            }           
            
            //check the error condition and display the error 
            if ((_cgcMinValue != 0) && (_cgcMaxValue != 0))
            {
                //Add the error if it was found
                string errorMsg = "Field " + pCGCField.Name + " attribute value is not within range of " +
                    _cgcMinValue.ToString() + " and " + _cgcMaxValue.ToString() + " of range domain"; 
                if (currentCGC < _cgcMinValue)
                    AddError(errorMsg);
                else if (currentCGC > _cgcMaxValue)
                    AddError(errorMsg);
            }

            return _ErrorList;
        }

    }
}
