using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Reflection;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;
using log4net;
using Miner.ComCategories;
using Miner.Geodatabase;
using Miner.Interop;
using PGE.Common.Delivery.ArcFM;
using System.Collections.Generic;
using PGE.Common.Delivery.Framework;
using PGE.Common.Delivery.Diagnostics;

namespace PGE.Desktop.EDER.ValidationRules
{
    /// <summary>
    /// Validate transformer features to make sure a transformer feature's related GroundingBank object has DetectionScheme value.
    /// </summary>
    [Guid("430919BD-F759-435B-9296-08A3F7AB916A")]
    [ProgId("PGE.Desktop.EDER.TransformerDetectionScheme")]
    [ComponentCategory(ComCategory.MMValidationRules)]
    public class TransformerDetectionScheme : BaseValidationRule
    {
        #region
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        private const string _warningMsg = "GroundingBank table Field Model name :{0} not found.";
        private const string _errorMsg = "The related GroundingBank has a <Null> DetectionScheme value.";
        #endregion
        #region Constructors
        /// <summary>
        /// Constructor.
        /// </summary>
        public TransformerDetectionScheme()
            : base("PGE Validate XFR Detection Scheme", SchemaInfo.Electric.ClassModelNames.Transformer)
        {
        }
        #endregion

        #region Override
        /// <summary>
        /// Validate the row.
        /// </summary>
        /// <param name="row"> The row data being validated. </param>
        /// <returns> A list of errors or nothing. </returns>
        protected override ID8List InternalIsValid(IRow row)
        {
            //Change to address INC000003803926 QA/QC freezing on the QAQC subtask 
            //if (!ValidationFilterEngine.Instance.IsQAQCRuleEnabled(_Name, base.Severity))
            //    return _ErrorList;  

            IObject obj = (IObject)row;

            IEnumerable<IObject> relRecords = obj.GetRelatedObjects("Grounding Bank", esriRelRole.esriRelRoleAny, null);
            int vvnt = relRecords.Count();
            //get related records
            var groundingBankObj = relRecords.FirstOrDefault(); //obj.GetRelatedObjects(aliasName: "Grounding Bank")
                                      //.FirstOrDefault();

            
            if (groundingBankObj == null) //if there are no related objects
            {
                _logger.Debug("Transformer has no grounding bank records.");
                return _ErrorList;
            }

            var fieldIndexDetectionScheme = ModelNameFacade.FieldIndexFromModelName(groundingBankObj.Class,
                                                                                    SchemaInfo.General.FieldModelNames.DetectionScheme);
            
            if (fieldIndexDetectionScheme == -1)
            {
                _logger.Warn(string.Format(_warningMsg, SchemaInfo.General.FieldModelNames.DetectionScheme));
                return _ErrorList;
            }

            var fldVal = groundingBankObj.Value[fieldIndexDetectionScheme];
                    
            if (fldVal == null || fldVal == DBNull.Value)
            {
                _logger.Debug("Transformer GroundingBank detection scheme: <null>");
                AddError(_errorMsg);
                return _ErrorList;
            }
            _logger.Debug("Transformer GroundingBank detection scheme: " + fldVal);

            return _ErrorList;
        }

        #endregion
    }
}
