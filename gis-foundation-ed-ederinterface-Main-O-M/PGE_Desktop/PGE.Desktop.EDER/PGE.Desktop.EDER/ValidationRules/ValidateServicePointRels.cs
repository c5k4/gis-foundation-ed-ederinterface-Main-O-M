using System;
using System.Reflection;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geodatabase;
using Miner.ComCategories;
using Miner.Interop;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Framework;
using System.Collections.Generic;
using ESRI.ArcGIS.NetworkAnalysis;
using ESRI.ArcGIS.Geometry;
using System.Timers;
using System.Diagnostics;
using Miner.Geodatabase.Edit;
using PGE.Common.Delivery.Framework.FeederManager;

namespace PGE.Desktop.EDER.ValidationRules
{
    [ComVisible(true)] 
    [Guid("3b06bdd7-88ce-4069-b563-c94f4a453620")]
    [ProgId("PGE.Desktop.EDER.ValidateServicePointRels")]
    [ComponentCategory(ComCategory.MMValidationRules)] 
    public class ValidateServicePointRels : BaseValidationRule
    {
        #region Constructors
        public ValidateServicePointRels()
            : base("PGE Validate ServicePoint Relationships", _modelNames)
        {    
        }
        #endregion Constructors

        #region Private

        private static readonly PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        private static string[] _modelNames = new string[] { SchemaInfo.Electric.ClassModelNames.ServicePoint };
        private const string _errMsg = "SericePoint has relationship to both transformer and primarymeter";
        private const string _primaryMeterGUIDField = "primarymeterguid";
        private const string _transformerGUIDField = "transformerguid";

        #endregion Private

        #region Override for ServicePoint Relationship validator
        /// <summary>
        /// Validates the object for defined rule.
        /// </summary>
        /// <param name="row">the Object to be validated.</param>
        /// <returns>Error list</returns>
        protected override ID8List InternalIsValid(IRow row)
        {
            IFeature feature = row as IFeature;
            IDataset pDS = null; 
            
            //check transformerguid field and primarymeterguid field is not null
            int primMeterGUIDFldIdx = row.Fields.FindField(_primaryMeterGUIDField);
            int transformerGUIDFldIdx = row.Fields.FindField(_transformerGUIDField);

            if ((primMeterGUIDFldIdx != -1) && (transformerGUIDFldIdx != -1))
            {
                if ((row.get_Value(primMeterGUIDFldIdx) != DBNull.Value) &&
                    (row.get_Value(primMeterGUIDFldIdx) != DBNull.Value))
                {
                    //Sometimes a GUID is present in the foreign key rel 
                    //field but a relationship does not actually exits so 
                    //verify both relationships actually exist 

                    string[] modelNamesTx = new string[] { SchemaInfo.Electric.ClassModelNames.PGETransformer };
                    string[] modelNamesPrimMeter = new string[] { SchemaInfo.Electric.ClassModelNames.PrimaryMeter };

                    bool hasRelatedTx = false;
                    bool hasrelatedPrimMeter = false; 
                    IEnumRelationshipClass pEnumRels = 
                        ((IObjectClass)row.Table).get_RelationshipClasses(
                        esriRelRole.esriRelRoleAny);
                    pEnumRels.Reset(); 
                    IRelationshipClass pRelCls = pEnumRels.Next();
                    while (pRelCls != null)
                    {
                        pDS =(IDataset)pRelCls;
                        Debug.Print(pDS.Name); 

                        if ((ModelNameFacade.ContainsClassModelName(pRelCls.OriginClass, modelNamesTx)) ||
                            (ModelNameFacade.ContainsClassModelName(pRelCls.DestinationClass, modelNamesTx)))
                        {
                            //This is the transformer relationship
                            if (pRelCls.GetObjectsRelatedToObject((IObject)row).Count != 0)
                                hasRelatedTx = true; 
                        }
                        else if ((ModelNameFacade.ContainsClassModelName(pRelCls.OriginClass, modelNamesPrimMeter)) ||
                            (ModelNameFacade.ContainsClassModelName(pRelCls.DestinationClass, modelNamesPrimMeter)))
                        {
                            //This is the primarymeter relationship 
                            if (pRelCls.GetObjectsRelatedToObject((IObject)row).Count != 0)
                                hasrelatedPrimMeter = true; 
                        }
                        pRelCls = pEnumRels.Next(); 
                    }
                    //If both rels exist then add the error 
                    if (hasRelatedTx && hasrelatedPrimMeter)
                        AddError(_errMsg); 
                }
            }

            return _ErrorList;
        }
        #endregion Override for Source Connectivity validator
    }
}
