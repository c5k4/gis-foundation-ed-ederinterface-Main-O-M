// ========================================================================
// Copyright © 2006 Telvent, Consulting Engineers, Inc.
// <history>
// Data Validation-Misc XFR - EDER Component Specification
// Shaikh Rizuan 10/11/2012	Created
// </history>
// All rights reserved.
// ========================================================================
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using log4net;
using Miner.ComCategories;
using Miner.Interop;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Framework;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using PGE.Common.Delivery.Diagnostics;

namespace PGE.Desktop.EDER.ValidationRules
{
    /// <summary>
    /// Validate  that customers assigned to transformers in another division are correct.
    /// </summary>
    [ComVisible(true)]
    [Guid("967D6AF2-46AD-427D-AC5B-467BC53E8D98")]
    [ProgId("PGE.Desktop.EDER.ValidateXFRServicePointDivision")]
    [ComponentCategory(ComCategory.MMValidationRules)]
    public class ValidateXFRServicePointDivision : BaseValidationRule
    {

        #region Class Variables
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        #endregion
        #region Constructors
        /// <summary>
        /// Constructor.
        /// </summary>
        public ValidateXFRServicePointDivision()
            : base("PGE Validate XFR / ServicePoint Division", SchemaInfo.Electric.ClassModelNames.Transformer)
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

            _logger.Warn(string.Format("Object model class {0} not found.", _ModelNames));
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

            #region
            //ITable tbl = row.Table;
            //IDataset dataSt = tbl as IDataset; 
            //IWorkspace wkSpace = dataSt.Workspace;
            //IMMTableResolver tblResolver = new MMTableUtilsClass();
            //IFeatureWorkspace featureWkSpace = wkSpace as IFeatureWorkspace;
            //ISet relatedObjects = null;
            #endregion
            IObject obj = row as IObject;
            string divsnName = string.Empty;
            //get field value for division
            object transDivName = obj.GetFieldValue(null, true, SchemaInfo.Electric.FieldModelNames.Division);
            if (transDivName == System.DBNull.Value || transDivName==null)
            {
                _logger.Debug("Model name: " + SchemaInfo.Electric.FieldModelNames.Division + " not assigned to Division Field or Field Value is <Null>.");
                return _ErrorList;
            }
            //get the relationship records from object
            IEnumerable<IObject> relRecords = obj.GetRelatedObjects(null, esriRelRole.esriRelRoleAny, SchemaInfo.Electric.ClassModelNames.ServicePoint);
            IObject relObj = null;
            bool divModelnamePresent = false;
            if (relRecords.Count() > 0)
            {
                //IEnumerator<IObject> relatedRec = relRecords.GetEnumerator();
               // relRecords.
                //relObj = relatedRec.Current;
                relObj = ((IEnumerator<IObject>)relRecords).Current;
                divModelnamePresent = ModelNameFacade.ContainsFieldModelName(relObj.Class, SchemaInfo.Electric.FieldModelNames.Division);
            }
         //  bool divsnFld = ModelNameFacade.ContainsFieldModelName(obj.Class, SchemaInfo.Electric.FieldModelNames.Division);
            if (divModelnamePresent)
            {
                IEnumerable<string> divNames = relRecords.Select(o => o.GetFieldValue(null, true, SchemaInfo.Electric.FieldModelNames.Division).Convert("<Null>"));
                string[] strArrDivNames = divNames.Where(o => o != transDivName.ToString()).ToArray();
                if (strArrDivNames.Length > 0)//if length is greater than zero
                {
                    for (int i = 0; i < strArrDivNames.Length; i++)
                    {
                        if (string.IsNullOrEmpty(divsnName))
                        {
                            
                              
                            divsnName = strArrDivNames[i];
                        }
                        else
                        {
                            if (!divsnName.Contains(strArrDivNames[i]))
                            {
                               
                                divsnName += "," + strArrDivNames[i];
                            }

                        }
                    }
                    //divsnName = string.Join(",", strArrDivNames);//join strings with comma separated
                    AddError("A Transformer in division " + transDivName.ToString() + " can’t serve Service Points from division " + divsnName + ".");

                }

            }
            return _ErrorList;
            #region old code owner: shaikh rizuan Dt: 1/10/2012
            //if (relRecords.Count() > 0)
            //{
            //    foreach (IObject relObj in relRecords)
            //    {
            //        object serviceDivName = relObj.GetFieldValue(null, true, SchemaInfo.Electric.FieldModelNames.Division);
            //        //check if value is not null and division name for service point value is not equal to transformer
            //        if (serviceDivName != System.DBNull.Value && serviceDivName.ToString() != transDivName.ToString())
            //        {
            //            //add the service point division name to list

            //            divNames.Add(serviceDivName.ToString());
            //        }

            //    }
            //    if (divNames.Count > 0)//if count is greater than zero
            //    {
            //        //join the string with comman separated
            //        divsnName = string.Join(",", divNames.ToArray());
            //        AddError("A Transformer in division " + transDivName.ToString() + " can’t serve ServicePoints from division " + divsnName + ".");
            //    }
            #endregion
        }



        #endregion
    }
        
}
      

      
    