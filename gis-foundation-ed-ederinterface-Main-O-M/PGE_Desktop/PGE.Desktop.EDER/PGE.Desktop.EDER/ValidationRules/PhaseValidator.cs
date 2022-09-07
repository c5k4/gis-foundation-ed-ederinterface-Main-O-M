using ESRI.ArcGIS.Geodatabase;
using Miner.NetworkModel.Electric;
using PGE.Common.Delivery.Diagnostics;
using PGE.Common.Delivery.Framework;
using PGE.Common.Delivery.Framework.FeederManager;
using PGE.Common.Delivery.Geodatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PGE.Desktop.EDER.ValidationRules
{
    public static class PhaseValidator
    {

        #region Private Variables
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        private static readonly string _phaseDesingationModelName = SchemaInfo.Electric.FieldModelNames.PhaseDesignation;
        private static readonly string[] _primaryModelNames = new string[] {
            SchemaInfo.Electric.ClassModelNames.PGEBusBar,
            SchemaInfo.Electric.ClassModelNames.PrimaryConductor,
            };
        #endregion

        #region Public Methods
        /// <summary>
        /// Phase Designation validation, refactored so that rules can be split into errors and warnings.
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public static List<string> Validate(IRow row)
        {

            List<string> errors = new List<string>();
            IFeature feature = null;
            if (row is IFeature)
            {
                feature = row as IFeature;
            }

            IObjectClass rowObjectClass = row.Table as IObjectClass;
            _logger.Debug(string.Format("Starting Phase Validation on {0} OID: {1}.", rowObjectClass.AliasName, row.OID));
            IDomain domain = GDBFacade.FindByName((rowObjectClass as IDataset).Workspace, SchemaInfo.General.PhaseDesignationDomainName);
            ICodedValueDomain codedValueDomain = domain as ICodedValueDomain;

            int phaseDesignationFldIx = ModelNameFacade.FieldIndexFromModelName(row.Table as IObjectClass, SchemaInfo.Electric.FieldModelNames.PhaseDesignation);
            if (phaseDesignationFldIx == -1) return errors;
            object fieldValue = row.get_Value(phaseDesignationFldIx);
            Phases energizedPhases = FeederManager2.GetEnergizedPhases(row);

            // Validate.
            if ((fieldValue == null) || (fieldValue == DBNull.Value) || (fieldValue.ToString() == string.Empty))
            {
                errors.Add("Phase Designation is missing.");

            }

            else
            {
                // Get the upstream features.
                List<IFeature> upstreamEdges = GetUpstream(feature);
                int upstreamPrimaryPhase = 0;
                if ((upstreamEdges != null) && (upstreamEdges.Count > 0))
                {
                    // Skip validation if upstream has no primary, we don't care about secondary features yet.
                    if (HasPrimaryUpstream(upstreamEdges))
                    {

                        var energizedCode = LookupDomainCode(codedValueDomain, energizedPhases.ToString(), "None");
                        int featurePhaseDesignation = (int)fieldValue;
                        bool isEnergizedPhaseValid = false;
                        if (!energizedCode.Equals(Phases.None))
                        {
                            isEnergizedPhaseValid = (featurePhaseDesignation & (int)energizedCode) == featurePhaseDesignation;
                        }
                        if (!isEnergizedPhaseValid)
                        {
                            string phaseDescription = feature.GetFieldValue(null, true, _phaseDesingationModelName).ToString();
                            errors.Add(string.Format("Phase Designation: ({0}) has phases that are not included in Energized Phases: ({1}). Source: ({2})", phaseDescription, energizedPhases.ToString(), GetUpstreamErrorDetail(upstreamEdges)));
                        }
                        else
                        {
                            GetUpstreamPrimaryLinePhases(upstreamEdges, ref upstreamPrimaryPhase);
                            if ((upstreamPrimaryPhase > 0) && (featurePhaseDesignation > 0))
                            {
                                // The Logical AND will result in Feature's Phase if the Validating Feature's Phase
                                // is a subset of UpstreamFeature's Phase else it will be different.
                                // 0001 (1) & 0010 (2) == 0010 (2) so 2 is not a subset of 1
                                // but 0001(1) & 0011(3) == 0010(1) and 0010(2) & 0011(3) == 0010(2)
                                // so in above scenarios 1&2 are subset of 3.

                                bool isPhaseValid = (featurePhaseDesignation & upstreamPrimaryPhase) == featurePhaseDesignation;
                                if (!isPhaseValid)
                                {
                                    // Phase missmatch - format error description.
                                    string phaseDescription = feature.GetFieldValue(null, true, _phaseDesingationModelName).ToString();
                                    string sourceLineDetails = GetUpstreamErrorDetail(upstreamEdges);
                                    errors.Add(string.Format("Phase Designation: {0} has phases not contained by sourceline(s): ({1}).", phaseDescription, sourceLineDetails));
                                }

                            }
                            else
                            {
                                errors.Add("Cannot verify Phase Designation: Sourceline Phase Designation is missing.");
                            }
                        }

                    }
                    else
                    {
                        // No primary upstream.

                        _logger.Info(string.Format("{0} OID: {1} Doesn't have any primary upstream, skipping Validate Phase Designation Rule.", rowObjectClass.AliasName, row.OID));
                    }
                }
                else
                {
                    // No upstream edges.
                    _logger.Info(string.Format("{0} OID: {1} Doesn't have any upstream edges, skipping Validate Phase Designation Rule.", rowObjectClass.AliasName, row.OID));

                }


            }
            return errors;
        }
        #endregion 

        #region private methods
        /// <summary>
        /// Determines Phase of all primary upstream lines.
        /// </summary>
        /// <param name="upstreamEdges"></param>
        /// <param name="phase"></param>
        private static void GetUpstreamPrimaryLinePhases(List<IFeature> upstreamEdges, ref int phase)
        {
            foreach (IFeature line in upstreamEdges)
            {
                if (IsPrimaryLine(line))
                {

                    int phaseDesignationValue = (int)line.GetFieldValue(null, false, _phaseDesingationModelName);
                    // Bitwise OR to combine all phases into one superset.
                    phase = phase | phaseDesignationValue;
                }
            }

        }

        /// <summary>
        /// Determine if upstream line(s) have any primary, used to filter out possible secondary feature such as some of the open points.
        /// </summary>
        /// <param name="upstreamEdges"></param>
        /// <returns></returns>
        private static bool HasPrimaryUpstream(List<IFeature> upstreamEdges)
        {
            bool hasPrimary = false;
            foreach (IFeature line in upstreamEdges)
            {
                if (IsPrimaryLine(line))
                {
                    hasPrimary = true;
                }
            }
            return hasPrimary;
        }

        /// <summary>
        /// Determines if line is primary.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private static bool IsPrimaryLine(IFeature line)
        {
            bool isPrimary = false;
            if (ModelNameFacade.ContainsClassModelName(line.Class, SchemaInfo.Electric.ClassModelNames.PGEBusBar))
            {
                // Case for Busbar.
                if (line.HasSubtypeCode(1)) 
                {
                    // PrimaryBusBar.
                    isPrimary = true;
                }
            }
            else if (ModelNameFacade.ContainsClassModelName(line.Class, _primaryModelNames))
            {
                // Primary conductor.
                isPrimary = true;
            }
            return isPrimary;

        }

        /// <summary>
        /// Get UpstreamLines.
        /// </summary>
        /// <param name="feature">The feature being validated.</param>
        /// <returns></returns>
        private static List<IFeature> GetUpstream(IFeature feature)
        {
            List<IFeature> upstreamSourceLines = new List<IFeature>();

            // Open devices usually don't trace, need to get the upstream connected line instead.
            if (IsOpen(feature))
            {
                ISimpleJunctionFeature simpleJunction = feature as ISimpleJunctionFeature;
                int[] conductorEdgeIndexs = ValidateClosedDevice.GetConductorsAtJunction(simpleJunction);
                // Get the Features connected to the Switch.
                for (int i = 0; i < conductorEdgeIndexs.Count(); i++)
                {
                    IFeature source = simpleJunction.get_EdgeFeature(conductorEdgeIndexs[i]) as IFeature;
                    if (source != null)
                    {
                        upstreamSourceLines.Add(source);
                    }
                }
            }
            else
            {
                // Otherwise trace to get all upstream edges.
                upstreamSourceLines = TraceFacade.GetFirstUpstreamEdgePerPath(feature);
            }

            // If still no upstream lets try just getting the first one.
            if ((upstreamSourceLines == null) || (upstreamSourceLines.Count == 0))
            {
                IFeature source = TraceFacade.GetFirstUpstreamEdge(feature);
                if (source != null)
                {
                    upstreamSourceLines.Add(source);
                }
            }

            return upstreamSourceLines;
        }

        /// <summary>
        /// Check the normal position fields and determine if this device is open.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static bool IsOpen(IFeature feat)
        {
            // Assign normal positions A,B,C to string array.
            string[] normalPostionModelNames = { SchemaInfo.Electric.FieldModelNames.NormalpositionA, SchemaInfo.Electric.FieldModelNames.NormalpositionB, SchemaInfo.Electric.FieldModelNames.NormalPositionC };
            bool isOpen = false;

            foreach (string normalPostionModelName in normalPostionModelNames)
            {
                int? normalstatus = (int?)feat.GetFieldValue(null, false, normalPostionModelName);
                // If value equal to 0 it's open.
                if (normalstatus == 0)
                {
                    isOpen = true;
                    break;
                }
            }
            return isOpen;
        }

        /// <summary>
        /// Creates upstream details for error message.
        /// </summary>
        /// <param name="upstreamfeatures"></param>
        /// <returns></returns>
        private static string GetUpstreamErrorDetail(List<IFeature> upstreamfeatures)
        {
            string retValue = "";
            List<string> returnList = new List<string>();
            foreach (var feature in upstreamfeatures)
            {
                returnList.Add(feature.Class.AliasName + " OID: " + feature.OID.ToString() + " Phase: " + feature.GetFieldValue(null, true, _phaseDesingationModelName));

            }
            retValue = string.Join(", ", returnList.ToArray());
            return retValue;
        }
        /// <summary>
        /// Attempts to match the domain description to a code, optionally returning a default string if the code is not found.
        /// If no default value string is provided, null is returned.
        /// </summary>
        /// <param name="codedValueDomain">The domain to search on.</param>
        /// <param name="domainDescription">The description to look for in the domain.</param>
        /// <param name="defaultValue">An optional default value to return in liu of a result.</param>
        /// <returns></returns>
        private static object LookupDomainCode(ICodedValueDomain codedValueDomain, string domainDescription, string defaultValue = null)
        {
            if (codedValueDomain != null)
            {
                for (var i = 0; i < codedValueDomain.CodeCount; i++)
                {
                    if (codedValueDomain.Name[i] == domainDescription)
                    {
                        return codedValueDomain.Value[i];
                    }

                }
            }
            return defaultValue;
        }
        #endregion private methods

    }
}
