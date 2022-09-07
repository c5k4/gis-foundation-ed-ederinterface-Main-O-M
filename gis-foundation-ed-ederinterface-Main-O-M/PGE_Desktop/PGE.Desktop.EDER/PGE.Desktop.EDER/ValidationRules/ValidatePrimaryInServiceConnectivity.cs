using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.Geodatabase;

using Miner.ComCategories;
using Miner.Interop;

using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Diagnostics;
using PGE.Common.Delivery.Framework;

using PGE.Desktop.EDER.AutoUpdaters;

namespace PGE.Desktop.EDER.ValidationRules
{
    [Guid("FEA6D4D3-D4B2-40C6-8642-9B5FFD13C81A")]
    [ProgId("PGE.Desktop.EDER.ValidatePrimaryInServiceConnectivity")]
    [ComponentCategory(ComCategory.MMValidationRules)]
    public class ValidatePrimaryInServiceConnectivity : BaseValidationRule
    {
        #region Private Variables

        private static readonly string[] _enabledModelNames = new string[] {
            SchemaInfo.Electric.ClassModelNames.PrimaryOHConductor,
            SchemaInfo.Electric.ClassModelNames.PrimaryUGConductor,
            SchemaInfo.Electric.ClassModelNames.PGEBusBar
        };

        private static readonly string[] _busbarModelNames = new string[] { SchemaInfo.Electric.ClassModelNames.PGEBusBar };

        /// <summary>
        /// Logger to log all the information, warning and errors.
        /// </summary>
        /// 
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor.
        /// </summary>
        public ValidatePrimaryInServiceConnectivity()
            : base("PGE Validate Primary InService Connectivity", _enabledModelNames)
        {
        }
        #endregion Constructors


        #region Override for validation rule
        /// <summary>
        /// Determines if the provided row is valid. Errors are Proposed Primary Lines upstream of InService Primary Lines, 
        /// or InService Primary Lines Downstream of Proposed Primary Lines, Idle features are not validated or considered.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        protected override ID8List InternalIsValid(IRow row)
        {
            try
            {

                if (row == null)
                {
                    _logger.Debug("Primary Line row was null when Validating Primary InService Connectivity.");
                    return _ErrorList;
                }
                int testOID = row.OID;
                ISimpleEdgeFeature simpleLine = (ISimpleEdgeFeature)row;
                IFeature feature = (IFeature)row;
                if ((simpleLine == null) || (feature == null))
                {
                    _logger.Warn(string.Format("Primary Line {0} OID: {1} couldn't be converted to a Feature when Validating Primary InService Connectivity.", row.Table, row.OID));
                    return _ErrorList;
                }
                if (StatusHelper.IsIdle(feature))
                {
                    _logger.Info(string.Format("Primary Line {0} OID: {1} has an Idle Status, Skipping Validating Primary InService Connectivity.", feature.Class.AliasName, feature.OID));
                    return _ErrorList;
                }
                if (!IsPrimaryLine(feature))
                {
                    _logger.Debug(string.Format("Skipping Primary InService Connectivity validation, determined that {0} OID: {1} is not a primary line.", feature.Class.AliasName, feature.OID));
                    return _ErrorList;
                }
                IGeometricNetwork geomNetwork = (simpleLine as INetworkFeature).GeometricNetwork;
                if (geomNetwork == null)
                {
                    _logger.Warn("The geometric network could not be found when Validating Primary InService Connectivity.");
                    return _ErrorList;
                }

                // If primary line is proposed, then do a downstream trace and search for inservice primary.
                if (StatusHelper.IsProposed(feature))
                {
                    IMMTracedElements tracedJunctions = null;
                    IMMTracedElements tracedEdges = null;
                    IMMTracedElement tempElement = null;
                    TraceFacade.DownStreamTrace(simpleLine.EID, esriElementType.esriETEdge, geomNetwork, out tracedJunctions, out tracedEdges);
                    tracedEdges.Reset();
                    while ((tempElement = tracedEdges.Next()) != null)
                    {
                        IFeature downstreamLine = TraceFacade.GetFeaturefromEID(tempElement.EID, esriElementType.esriETEdge, geomNetwork);
                        if (downstreamLine == null)
                        {
                            _logger.Debug("Traced downstream line could not be converted to an IFeature from EID when Validating Primary InService Connectivity.");
                        }
                        // InService downstream of a Proposed Feature is an error, Idle doesn't matter.
                        if ((IsPrimaryLine(downstreamLine)) && (StatusHelper.IsInService(downstreamLine)))
                        {

                            AddError(String.Format("A In-Service feature {0} OID: {1} cannot be downstream of an Proposed-Install feature", downstreamLine.Class.AliasName, downstreamLine.OID));
                            break;
                        }
                    }
                }
                // If primary line is InService, then do an upstream trace and search for proposed primary.
                else if (StatusHelper.IsInService(feature))
                {
                    IEnumNetEID junctions = null;
                    IEnumNetEID edges = null;
                    TraceFacade.UpStreamTrace(simpleLine.EID, esriElementType.esriETEdge, geomNetwork, out junctions, out edges);
                    edges.Reset();
                    int edgeEID = -1;
                    while ((edgeEID = edges.Next()) > 0)
                    {
                        IFeature upstreamLine = TraceFacade.GetFeaturefromEID(edgeEID, esriElementType.esriETEdge, geomNetwork);
                        if (upstreamLine == null)
                        {
                            _logger.Debug("Traced upstream line could not be converted to an IFeature from EID when Validating Primary InService Connectivity.");
                        }
                        // Proposed pstream of an InService  Feature is an error , Idle doesn't matter.
                        if ((IsPrimaryLine(upstreamLine)) && (StatusHelper.IsProposed(upstreamLine)))
                        {

                            AddError(String.Format("A Proposed-Install feature {0} OID: {1} cannot be upstream of an In-Service feature", upstreamLine.Class.AliasName, upstreamLine.OID));
                            break;
                        }

                    }

                }

            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred while validating Primary InService Connectivity rule.", ex);
            }

            return _ErrorList;

        }

        #endregion Override for validation rule

        #region private methods

        /// <summary>
        /// Checks to see if a line is a primary conductor or a primary busbar.
        /// </summary>
        /// <param name="line">upstream line</param>
        /// <returns>True if it's primary or false if it's not.</returns>
        private bool IsPrimaryLine(IFeature line)
        {
            bool isPrimary = false;
            if (ModelNameFacade.ContainsClassModelName(line.Class, _busbarModelNames))
            {
                // Case for PrimaryBusBar.
                isPrimary = line.HasSubtypeCode(1);
            }
            else if (ModelNameFacade.ContainsClassModelName(line.Class, _enabledModelNames))
            {
                isPrimary = true;
            }
            return isPrimary;

        }

        #endregion private methods
    }
}
