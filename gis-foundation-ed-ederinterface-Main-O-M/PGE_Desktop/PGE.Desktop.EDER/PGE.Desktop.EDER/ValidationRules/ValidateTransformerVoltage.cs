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
using PGE.Desktop.EDER.AutoUpdaters;
using PGE.Common.Delivery.Diagnostics;
using PGE.Common.Delivery.Systems.Configuration;
using System.Xml;
using System.IO;

namespace PGE.Desktop.EDER.ValidationRules
{
    /// <summary>
    /// Validate  that transformer voltage with source conductors.
    /// </summary>
    /// 
    [ComVisible(true)]
    [Guid("999AB07B-1A6A-4D3F-9FBC-AABCDFBAD5EF")]
    [ProgId("PGE.Desktop.EDER.ValidateTransformerVoltage")]
    [ComponentCategory(ComCategory.MMValidationRules)]
   public class ValidateTransformerVoltage:BaseValidationRule
    {
        #region Class Variables
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        private const string _errorMsg = "Operating Voltage is not equal to source conductor.";
        private const string _lowSideVoltage ="LOWSIDEVOLTAGE";
        private static string _defaultIntegration = string.Empty;
        private static string _xmlFilePath =string.Empty;// @"C:\Projects\pge\ED\Desktop\PGE.Desktop.EDER\ValidationRules\Config\";
        public static string _integratnName = "Transformer Volatge";
        public static string _domain = "SquareRoot3Voltage";
        #endregion

        /// <summary>
        /// get the domain mapping name from XML
        /// </summary>
        public static string Initialize
        {
            get
            {
                if (string.IsNullOrEmpty(_defaultIntegration))
                {
                    //Read the config location from the Registry Entry
                    SystemRegistry sysRegistry = new SystemRegistry("PGE");
                   _xmlFilePath = sysRegistry.ConfigPath;
                    //get file path name
                    string domainsXmlFilePath = System.IO.Path.Combine(_xmlFilePath, "ExternalTransformerVoltage.xml");
                    XmlDocument document = new XmlDocument();
                    if (File.Exists(domainsXmlFilePath))//check for file path
                    {
                        //load file
                        document.Load(domainsXmlFilePath);
                        //initialize the domain manager
                        DomainManager.Instance.Initialize(document.FirstChild, _integratnName);
                        _defaultIntegration = DomainManager.Instance.DefaultIntegration;
                    }
                }
                return _defaultIntegration;
            }
        }
         #region Constructors
         /// <summary>
        /// Constructor.
        /// </summary>
        public ValidateTransformerVoltage()
            : base("PGE Validate Transformer Voltage", SchemaInfo.Electric.ClassModelNames.Transformer)
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

             IFeature transformerFeatre = row as IFeature;
             System.Collections.Generic.List<IFeature> upStreamFeature = new System.Collections.Generic.List<IFeature>();
            //IFeature edgeFeature = TraceFacade.GetFirstUpstreamEdge(transformerFeatre);
              upStreamFeature= TraceFacade.GetFirstUpstreamEdges(transformerFeatre);

             if (upStreamFeature != null && upStreamFeature.Count>0)
             {
                 _logger.Debug(upStreamFeature[0].Class.AliasName + " OBJID: " + upStreamFeature[0].OID.ToString());
                 //get tansformer operating voltage value 
                 object transformerVal = System.DBNull.Value; 
                 
                 //get conductor operating voltage
                 object conductorVal = upStreamFeature[0].GetFieldValue(null, true, SchemaInfo.Electric.FieldModelNames.OperatingVoltage4TransformerValidation).Convert<object>(System.DBNull.Value);
                 _logger.Debug(upStreamFeature[0].Class.AliasName + " field Value : " + conductorVal);
                 if (conductorVal != System.DBNull.Value && !string.IsNullOrEmpty(conductorVal.ToString()))
                 {
#region if Primary Conductor(OH and UG)
                     if (ModelNameFacade.ContainsClassModelName(upStreamFeature[0].Class as IObjectClass, SchemaInfo.Electric.ClassModelNames.PrimaryConductor))
                     {
                         _logger.Debug(SchemaInfo.Electric.ClassModelNames.PrimaryConductor + " : True");
                         transformerVal = transformerFeatre.GetFieldValue(null, true, SchemaInfo.Electric.FieldModelNames.OperatingVoltage4TransformerValidation).Convert<object>(System.DBNull.Value);
                         _logger.Debug(transformerFeatre.Class.AliasName + " field Value : " + transformerVal);

                         if (transformerVal != System.DBNull.Value)
                         {
                             if (transformerVal.Equals(conductorVal))
                             {                              

                                 return _ErrorList;
                             }
                             bool chkTransVoltage = CheckSquareRootTransformerVoltage(transformerVal.ToString(), conductorVal.ToString());
                             if (chkTransVoltage)
                             {                               

                                 return _ErrorList;
                             }
                             IRowSubtypes priRowSubtype = upStreamFeature[0] as IRowSubtypes;
                             if (priRowSubtype.SubtypeCode == 1)//single phase
                             {
                                 //AddError("Validate transformer voltage INCOMPLETE: Pending information from PGE.");
                                 AddError("A transformer fed by a Primary Single Phase conductor must have the same operating voltage.");
                                 return _ErrorList;
                             }
                             bool isNeutral = NeutralPhaseHelper.Instance.HasNeutral(upStreamFeature[0]);
                             if (!isNeutral)
                             {
                                 AddError("A transformer fed by a primary conductor without a neutral wire must have the same operating voltage.");
                                 return _ErrorList;
                             }
                             else
                             {
                                 AddError(_errorMsg);
                                 return _ErrorList;
                             }
                         }
                         else
                         {
                             AddError("Operating Voltage is missing on " + transformerFeatre.Class.AliasName + ".");
                         }
                     }
#endregion
#region if conductor is secondary (OH and UG)
                     if (ModelNameFacade.ContainsClassModelName(upStreamFeature[0].Class as IObjectClass, SchemaInfo.Electric.ClassModelNames.SecondaryOHConductor) || ModelNameFacade.ContainsClassModelName(upStreamFeature[0].Class as IObjectClass, SchemaInfo.Electric.ClassModelNames.SecondaryUGConductor))
                     {
                         transformerVal = transformerFeatre.GetFieldValue(_lowSideVoltage, true, null).Convert<object>(System.DBNull.Value);
                         if (transformerVal != System.DBNull.Value)
                         {
                             if (transformerVal.Equals(conductorVal))
                             {
                                 return _ErrorList;
                             }
                             else
                             {
                                 AddError(_errorMsg);
                                 return _ErrorList;
                             }
                         }
                         else
                         {
                             AddError(_lowSideVoltage + " is missing on " + transformerFeatre.Class.AliasName + ".");
                         }
                     }
#endregion
                 
                     else
                     {
                         _logger.Warn("Class model Name: " + SchemaInfo.Electric.ClassModelNames.PrimaryConductor + "," + SchemaInfo.Electric.ClassModelNames.SecondaryOHConductor + "," + SchemaInfo.Electric.ClassModelNames.SecondaryUGConductor + " assigned to " + upStreamFeature[0].Class.AliasName);
                     }
                 }
                 else
                 {
                     _logger.Warn("field : OperatingVoltage cannot be <Null> for "  + upStreamFeature[0].Class.AliasName );

                 }
             }
             else
             {
                 _logger.Error(transformerFeatre.Class.AliasName +" is not connected with source conductor.");
             }
             
             return _ErrorList;
         }


        /// <summary>
        /// Check for equality for transformer voltage
        /// </summary>
        /// <param name="transformerVal">transformer operating voltage</param>
        /// <param name="conductorVal">conductor operating voltage</param>
        /// <returns>Return boolean value</returns>
         private bool CheckSquareRootTransformerVoltage(string transformerVal, string conductorVal)
         {
             string value = GetValueFromXml(_domain, conductorVal);
             if (value == transformerVal)//if matches
             {
                 return true;
             }
             return false;

         }

         /// <summary> Get value from XML </summary>
         /// <param name="domainsXmlFilePath">File Path</param>
         /// <param name="domain">Domain Name</param>
         /// <param name="value">From to Value</param>
         /// <param name="errorMsg">error messgae if file path not found</param>
         /// <returns> return to value from xml</returns>
         public static string GetValueFromXml(string domain, string value)
         {
             string result = string.Empty;

             // XmlDocument document = new XmlDocument();
             //if (File.Exists(domainsXmlFilePath))
             //{
             try
             {
                 if (DomainManager.Instance.DefaultIntegration == Initialize)
                 {
                     result = DomainManager.Instance.GetValue(domain, value);
                     _logger.Debug("Domain Value: " + result);
                 }
                 else
                 {
                     DomainManager.Instance.DefaultIntegration = Initialize;
                     result = DomainManager.Instance.GetValue(domain, value);
                     _logger.Debug("Domain Value: " + result);

                 }
             }
             catch (Exception ex)
             {
                 _logger.Error(ex.Message);
                 return "";
             }

             return result;
         }
    #endregion
    }
}
