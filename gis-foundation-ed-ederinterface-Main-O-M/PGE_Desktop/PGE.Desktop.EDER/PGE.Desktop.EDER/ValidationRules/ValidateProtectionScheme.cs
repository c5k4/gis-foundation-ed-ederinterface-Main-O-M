// ========================================================================
// Copyright © 2006 Telvent, Consulting Engineers, Inc.
// <history>
// Data Validation-Protection Scheme Modified - EDER Component Specification
// Shaikh Rizuan 09/21/2012	Created
// </history>
// All rights reserved.
// ========================================================================
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;

using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;

using Miner.ComCategories;
using Miner.Interop;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Diagnostics;

using log4net;

namespace PGE.Desktop.EDER.ValidationRules
{

    /// <summary>
    /// Validate PrimaryGeneration and SecondaryGeneration feature classes to make sure  at least one Generator record shall be created and related to the ProtectiveDevice object.
    /// </summary>
    [ComVisible(true)]
    [Guid("BD691B83-64C0-456E-BE47-BEF5373AC5C4")]
    [ProgId("PGE.Desktop.EDER.ValidateProtectionScheme")]
    [ComponentCategory(ComCategory.MMValidationRules)]
   public class ValidateProtectionScheme : BaseValidationRule
    {
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

         #region Constructors
        /// <summary>
        /// Constructor.
        /// </summary>
        public ValidateProtectionScheme()
            : base("PGE Validate Protection Scheme",SchemaInfo.Electric.ClassModelNames.ProtectiveDevice)
        {
        }
        #endregion

        #region Base Validation Rule Overrides
        
        /// <summary>
        /// Validate the row.
        /// </summary>
        /// <param name="row"> The row data being validated with Generator table </param>
        /// <returns> A list of errors or nothing. </returns>
        /// 
        protected override ID8List InternalIsValid(IRow row)
        {
            //Change to address INC000003803926 QA/QC freezing on the QAQC subtask 
            //if (!ValidationFilterEngine.Instance.IsQAQCRuleEnabled(_Name, base.Severity))
            //    return _ErrorList; 

            ITable tbl = row.Table;
            IDataset dataSt = tbl as IDataset;
            IWorkspace wkSpace = dataSt.Workspace;
            IMMTableResolver tblResolver = new MMTableUtilsClass();
            IFeatureWorkspace featureWkSpace = wkSpace as IFeatureWorkspace;
            IObject obj = row as IObject;

            //get relationship name
            string relationshipClassProtectiveDeviceGenerator = tblResolver.ResolveTableName(SchemaInfo.Electric.RelationshipNames.ProtectiveDevice_Generator, featureWkSpace);
            _logger.Debug("Relationship between protective device and generator viz :" + SchemaInfo.Electric.RelationshipNames.ProtectiveDevice_Generator + "is not present.");
            if (!string.IsNullOrEmpty(relationshipClassProtectiveDeviceGenerator))
            {
                IEnumerable<IObject> getRelRecords = obj.GetRelatedObjects(null, esriRelRole.esriRelRoleAny, SchemaInfo.Electric.ClassModelNames.Generator);
                if (getRelRecords.Count() > 0)//if count is greater than zero
                {
                }
                else
                { // if relateObjects Count is zero then add error message

                    string errorMsg = string.Format("There are no related Generators.");
                    AddError(errorMsg);
                }
            }
            return _ErrorList;
        }

        #endregion
    }
}