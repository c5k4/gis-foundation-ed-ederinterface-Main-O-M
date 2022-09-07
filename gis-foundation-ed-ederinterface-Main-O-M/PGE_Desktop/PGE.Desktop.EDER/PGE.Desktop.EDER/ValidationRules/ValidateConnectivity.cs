/*
 *PreLoad info
* insert into sde.telvent_validation_severitymap values (sde.gdb_util.next_rowid('SDE', 'telvent_validation_severitymap'),'PGE Validate Connectivity',0);
*commit;
*
*Check box in ArcFM Properties - Object Info - Rules for Transformer ...
 */

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
    [Guid("74AAAE83-F420-45F5-A4A3-6471835A95FB")]
    [ProgId("PGE.Desktop.EDER.ValidateConnectivity")]
    [ComponentCategory(ComCategory.MMValidationRules)]
    public class ValidateConnectivity : BaseValidationRule
    {

        #region Private
      

        /// <summary>
        /// Model names to make enabled this validation rule.
        /// 
        /// </summary>
        private static string[] enabledModelNames = new string[] { SchemaInfo.Electric.ClassModelNames.PGEConductor, SchemaInfo.Electric.ClassModelNames.TransformerLead, 
                                                                   SchemaInfo.Electric.ClassModelNames.PGETransformer, SchemaInfo.Electric.ClassModelNames.PGEOpenPoint};

        private static readonly string _msgSecondaryToTransformer = "Non-Secondary Transformer OID:{0} may not be fed by {1} OID:{2}.";
        private static readonly string _msgPriSec = "{0}:{1} may not be fed by {2} OID: {3}.";
        private static readonly string _msgOpenPointBetween = "Proposed-Install OpenPoint OID:{0} may not be in between two In-Service Conductors.";
        /// <summary>
        /// logger to log all the information, waning and errors.
        /// </summary>
        private static readonly PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        #endregion Private

        /// <summary>
        /// Constructor.
        /// </summary>
        public ValidateConnectivity() : base("PGE Validate Connectivity", enabledModelNames)
        {
             
        }

        #region Override for validation rule
        /// <summary>
        /// Determines if the provided row is valid.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        protected override ID8List InternalIsValid(ESRI.ArcGIS.Geodatabase.IRow row)
        {
            try
            {
                IFeature feature = row as IFeature;
                //cehck if casting is successful
                if (feature == null)
                {
                    _logger.Debug("Row casting to Ifeature failed.");
                    return _ErrorList;
                }
                else
                {
                    if (feature.HasModelName(SchemaInfo.Electric.ClassModelNames.PGETransformer))
                    {
                        TransformerValidation(feature);
                    }
                    else if (feature.HasModelName(SchemaInfo.Electric.ClassModelNames.PGEConductor))
                    {
                        SecConductorValidation(feature);
                        PriConductorValidation(feature);
                    }
                    else if (feature.HasModelName(SchemaInfo.Electric.ClassModelNames.TransformerLead))
                    {
                        SecConductorValidation(feature);
                        PriConductorValidation(feature);
                    }
                    else if (feature.HasModelName(SchemaInfo.Electric.ClassModelNames.PGEOpenPoint))
                    {
                        OpenPointValidate(feature);
                    }
                }
                return _ErrorList;
            }
            catch (Exception ex)
            {
                _logger.Warn("Error occurred while validating Connectivity rule.", ex);
                return _ErrorList;
            }

        }
        #endregion Override for validation rule

        /// <summary>
        /// Validate Transformers connection to primary/secondary conductors.
        /// </summary>
        /// <param name="feature"></param>
        private void TransformerValidation(IFeature feature)
        {
            try
            {
                // Non-Secondary transformers (subtype != 8) may not be fed by a SecConductor or TransformerLead
                int subtype = int.Parse(feature.GetFieldValue("SUBTYPECD", false).ToString());
                if (subtype != 8)
                {
                    IFeature connectedSourceLine = TraceFacade.GetFirstUpstreamEdge(feature);
                    string[] modelNames = { SchemaInfo.Electric.ClassModelNames.SecondaryOHConductor, SchemaInfo.Electric.ClassModelNames.SecondaryUGConductor, 
                                            SchemaInfo.Electric.ClassModelNames.TransformerLead };
                    if (ModelNameFacade.ContainsClassModelName(connectedSourceLine.Class, modelNames))
                    {
                        AddError(string.Format(_msgSecondaryToTransformer, feature.OID, connectedSourceLine.Class.AliasName, connectedSourceLine.OID));
                    }
                }
            }
            catch (System.Exception ex)
            {
                _logger.Warn("Error in Connectivity.TransformerValidation rule.", ex);
            }
        }

        /// <summary>
        /// This function covers both Secondary Conductors and TransformerLeads.
        /// </summary>
        /// <param name="feature"></param>
        private void SecConductorValidation(IFeature feature)
        {
            try
            {
                // Can not connect to Primary Transformer
                List<IFeature> connectedSourceJunctions = TraceFacade.GetFirstDownstreamJunctions(feature);
                string[] tranmodelNames = { SchemaInfo.Electric.ClassModelNames.PGETransformer };
                string[] primodelNames = { SchemaInfo.Electric.ClassModelNames.PrimaryConductor };
                string[] secmodelNames = { SchemaInfo.Electric.ClassModelNames.SecondaryOHConductor, SchemaInfo.Electric.ClassModelNames.SecondaryUGConductor,
                                            SchemaInfo.Electric.ClassModelNames.TransformerLead };
                if (ModelNameFacade.ContainsClassModelName(feature.Class, secmodelNames))
                {
                    // Secondary shouldn't be connected to a primary transformer.
                    foreach (IFeature connectedSourceJunction in connectedSourceJunctions)
                    {
                        if (ModelNameFacade.ContainsClassModelName(connectedSourceJunction.Class, tranmodelNames))
                        {
                            // Non-Secondary transformers (subtype != 8) may not be fed by a SecConductor or TransformerLead
                            int subtype = int.Parse(connectedSourceJunction.GetFieldValue("SUBTYPECD", false).ToString());
                            if (subtype != 8)
                            {
                                AddError(string.Format(_msgSecondaryToTransformer, connectedSourceJunction.OID, feature.Class.AliasName, feature.OID));
                                break;
                            }
                        }
                    }

                    // Secondaries should not feed a Primary Conductor.
                    //  - May want to reformat to handle junctions as well.
                    IFeature nextFeat = GetFirstDownStreamFeature(feature);
                    if (nextFeat != null)
                    {
                        if (ModelNameFacade.ContainsAllClassModelNames(nextFeat.Class, primodelNames))
                        {
                            AddError(string.Format(_msgPriSec,nextFeat.Class.AliasName, nextFeat.OID, feature.Class.AliasName, feature.OID));
                          
                        }
                    }
                    //List<IFeature> connectedSourceEdge = TraceFacade.GetFirstDownstreamEdges(feature);
                    //foreach (IFeature edgeFeat in connectedSourceEdge)
                    //{
                    //    if (ModelNameFacade.ContainsAllClassModelNames(edgeFeat.Class, primodelNames))
                    //    {
                    //        AddError(string.Format(_msgPriSec, edgeFeat.OID, feature.Class.AliasName, feature.OID));
                    //        break;
                    //    }
                    //}
                }

            }
            catch (System.Exception ex)
            {
                _logger.Warn("Error in Connectivity.SecConductorValidation rule.", ex);
            }
        }

        /// <summary>
        /// Validates primary conductors connectivity. 
        /// </summary>
        /// <param name="feature"></param>
        private void PriConductorValidation(IFeature feature)
        {
            try
            {
                string[] secmodelNames = { SchemaInfo.Electric.ClassModelNames.SecondaryOHConductor, SchemaInfo.Electric.ClassModelNames.SecondaryUGConductor,
                                            SchemaInfo.Electric.ClassModelNames.TransformerLead };

                IFeature upstreamFeat = TraceFacade.GetFirstUpstreamEdge(feature);
                if (ModelNameFacade.ContainsAllClassModelNames(upstreamFeat.Class, secmodelNames))
                {
                    AddError(string.Format(_msgPriSec,feature.Class.AliasName, feature.OID, upstreamFeat.Class.AliasName, upstreamFeat.OID));
                }
                
            }
            catch (System.Exception ex)
            {
                _logger.Warn("Error in Connectivity.PriConductorValidation rule.", ex);
            }
        }

        /// <summary>
        /// Validate open point connectivity.
        /// </summary>
        /// <param name="feature"></param>
        private void OpenPointValidate(IFeature feature)
        {
            string subtype = feature.GetFieldValue("SUBTYPECD", false).ToString();
            // A proposed-install OpenPoint with Subtype IN (1,2) cannot be placed between two In-Service conductors.
            if (subtype == "1" || subtype == "2")
            {
                string status = feature.GetFieldValue(null, false, SchemaInfo.Electric.FieldModelNames.Status).ToString();
                if (status == "0")
                {
                    int cnt = 0;
                    List<IFeature> list = ConnectedEdgeFeatures(feature);
                    foreach (IFeature f in list)
                    {
                        string s = f.GetFieldValue(null, false, SchemaInfo.Electric.FieldModelNames.Status).ToString();
                        if (s == "5")
                            cnt++;
                    }
                    if (cnt > 1)
                    {
                        AddError(string.Format(_msgOpenPointBetween, feature.OID));
                    }
                }
            }
            

            

        }

        /// <summary>
        /// Gets connected edges of a junction.
        /// </summary>
        /// <param name="sourceFeature"></param>
        /// <returns>List of edges connected.</returns>
        private List<IFeature> ConnectedEdgeFeatures(IFeature sourceFeature)
        {
            List<IFeature> results = new List<IFeature>();
            if ((sourceFeature != null) && (sourceFeature is INetworkFeature) && (sourceFeature.FeatureType == esriFeatureType.esriFTSimpleJunction))
            {
                ISimpleJunctionFeature junctionFeature = sourceFeature as ISimpleJunctionFeature;
                IEdgeFeature edgeFeature = null;
                IFeature workfeature = null;

                for (int i = 0; i < junctionFeature.EdgeFeatureCount; i++)
                {
                    edgeFeature = junctionFeature.EdgeFeature[i];
                    if (edgeFeature is ISimpleEdgeFeature)
                    {
                        workfeature = edgeFeature as IFeature;
                        results.Add(workfeature);
                    }
                }

            }
            return results;
        }

        /// <summary>
        /// Gets first feature downstream of source.
        /// </summary>
        /// <param name="sourceFeature"></param>
        /// <returns></returns>
        private IFeature GetFirstDownStreamFeature(IFeature sourceFeature)
        {
            IFeature result = null;
            IMMTracedElements tracedJunctions;
            IMMTracedElements tracedEdges;
            int startEID = TraceFacade.GetFeatureEID(sourceFeature);
            IGeometricNetwork geomNetwork = (sourceFeature as INetworkFeature).GeometricNetwork;
            esriElementType elementType = TraceFacade.GetElementType(sourceFeature);

            TraceFacade.DownStreamTrace(startEID, elementType, geomNetwork, out tracedJunctions, out tracedEdges);
            List<int> tracedEIDs = TraceFacade.TracedEIDs(tracedJunctions, tracedEdges);
            int fromEID = 0;
            int toEID = 0;


            IEdgeFeature ef = sourceFeature as IEdgeFeature;
            fromEID = ef.FromJunctionEID;
            toEID = ef.ToJunctionEID;

            
            IFeature nextFeat = null;
            if (tracedEIDs.Contains(fromEID))
            {
                nextFeat = TraceFacade.GetFeaturefromEID(fromEID, esriElementType.esriETJunction, geomNetwork);
            }
            else if (tracedEIDs.Contains(toEID))
            {
                nextFeat = TraceFacade.GetFeaturefromEID(toEID, esriElementType.esriETJunction, geomNetwork);
            }

            if (nextFeat.Class.AliasName.Contains("ElectricDistNetwork_Junctions"))
            {
                // Junction feature we want what's next.
                ISimpleJunctionFeature junctFeature = nextFeat as ISimpleJunctionFeature;
                
                for (int i = 0; i < junctFeature.EdgeFeatureCount; i++)
                {
                    ISimpleEdgeFeature simpleEdgeFeature = junctFeature.EdgeFeature[i] as ISimpleEdgeFeature;

                    if (tracedEIDs.Contains(simpleEdgeFeature.EID) && simpleEdgeFeature.EID != startEID)
                    {
                        result = junctFeature.EdgeFeature[i] as IFeature;
                    }
                }
                int oid;

                if (result != null)
                    oid = result.OID;
                // test area
                for (int i = 0; i < junctFeature.EdgeFeatureCount; i++)
                {
                    ISimpleEdgeFeature simpleEdgeFeature = junctFeature.EdgeFeature[i] as ISimpleEdgeFeature;

                    if (tracedEIDs.Contains(simpleEdgeFeature.EID))
                    {
                        result = junctFeature.EdgeFeature[i] as IFeature;
                    }
                }
                if (result != null)
                    oid = result.OID;
            }

            return result;
        }
    }
}
