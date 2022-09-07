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


namespace PGE.Desktop.EDER.ValidationRules
{
    [Guid("CEAD4037-9EC3-4B2B-9123-709E5E9A1950")]
    [ProgId("PGE.Desktop.EDER.ValidatePrimaryMeterConnectivity")]
    [ComponentCategory(ComCategory.MMValidationRules)]
    public class ValidatePrimaryMeterConnectivity : BaseValidationRule
    {
        #region Private Variables

        private static readonly string[] _enabledModelNames = new string[] { SchemaInfo.Electric.ClassModelNames.PrimaryMeter };
        private static readonly string[] _secondaryModelNames = new string[] {

            SchemaInfo.Electric.ClassModelNames.TransformerLead,
            SchemaInfo.Electric.ClassModelNames.SecondaryOHConductor,
            SchemaInfo.Electric.ClassModelNames.SecondaryUGConductor

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
        public ValidatePrimaryMeterConnectivity()
            : base("PGE Validate PrimaryMeter Connectivity", _enabledModelNames)
        {
        }
        #endregion Constructors


        #region Override for validation rule
        /// <summary>
        /// Determines if the provided row is valid.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        protected override ID8List InternalIsValid(IRow row)
        {

            try
            {

                if (row == null)
                {
                    _logger.Debug("PrimaryMeter row was null when Validating PrimaryMeter Connectivity.");
                    return _ErrorList;
                }
                ISimpleJunctionFeature feature = (ISimpleJunctionFeature)row;
                if (feature == null)
                {
                    _logger.Warn("PrimaryMeter couldn't be converted to a SimpleJunctionFeature when Validating PrimaryMeter Connectivity.");
                    return _ErrorList;
                }
                IGeometricNetwork geomNetwork = (feature as INetworkFeature).GeometricNetwork;
                if (geomNetwork == null)
                {
                    _logger.Warn("The geometric network could not be found when Validating PrimaryMeter Connectivity.");
                    return _ErrorList;
                }
                IMMTracedElements tracedJunctions = null;
                IMMTracedElements tracedEdges = null;
                IMMTracedElement tempElement = null;
                TraceFacade.DownStreamTrace(feature.EID, esriElementType.esriETJunction, geomNetwork, out tracedJunctions, out tracedEdges);
                tracedEdges.Reset();
                while ((tempElement = tracedEdges.Next()) != null)
                {
                    IFeature downstreamLine = TraceFacade.GetFeaturefromEID(tempElement.EID, esriElementType.esriETEdge, geomNetwork);
                    if (downstreamLine == null)
                    {
                        _logger.Debug("Traced feature could not be converted to an IFeature when Validating PrimaryMeter Connectivity.");
                    }
                    else if (IsSecondary(downstreamLine))
                    {
                        AddError(String.Format("Primary Meter should not source secondary, {0} OID: {1}", downstreamLine.Class.AliasName, downstreamLine.OID));
                        break;
                    }


                }

            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred while validating PrimaryMeter Connectivity rule.", ex);
            }

            return _ErrorList;
        }

        #endregion Override for validation rule

        #region private methods
        /// <summary>
        /// Determines if feature being checked is secondary. 
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private bool IsSecondary(IFeature feature)
        {
            bool isSecondary = false;
            if (ModelNameFacade.ContainsClassModelName(feature.Class, _secondaryModelNames))
            {
                isSecondary = true;
            }
            else if (ModelNameFacade.ContainsClassModelName(feature.Class, _busbarModelNames))
            {
                isSecondary = feature.HasSubtypeCode(2);
            }
            return isSecondary;
        }

        #endregion private methods
    }
}
