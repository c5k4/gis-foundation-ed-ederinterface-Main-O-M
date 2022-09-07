using System;
using System.Reflection;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geodatabase;
using Miner.ComCategories;
using Miner.Interop;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Framework;

namespace PGE.Desktop.EDER.ValidationRules
{
    [ComVisible(true)]
    [Guid("6DFAFB67-B445-48BE-9B7E-4237BD28DDF5")]
    [ProgId("PGE.Desktop.EDER.SourceLinePhase")]
    [ComponentCategory(ComCategory.MMValidationRules)]
    public class SourceLinePhase : BaseValidationRule, IMMCurrentStatus
    {
        #region Private
        /// <summary>
        /// logger to log all the information, warning and errors.
        /// </summary>
        private static readonly PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        //private string _conductorModelName = SchemaInfo.Electric.ClassModelNames.Conductor;// "CONDUCTOR";
        private string _primaryOHModellName = SchemaInfo.Electric.ClassModelNames.PrimaryOverHead;//"PGE_PRIMARYOVERHEAD";
        private string _primaryUGModellName = SchemaInfo.Electric.ClassModelNames.PrimaryUnderGround;//"PGE_PRIMARYUNDERGROUND";
        private string _secondaryOHModellName = SchemaInfo.Electric.ClassModelNames.SecondaryOverHead;//"PGE_SECONDARYOVERHEAD";
        private string _secondaryUGModellName = SchemaInfo.Electric.ClassModelNames.SecondaryUnderGround;//"PGE_SECONDARYUNDERGROUND";
        // primary OH subtypes
        private int _primaryOHSinglePhaseSubtypeCode = 1;
        private int _primaryOHTwoPhaseSubtypeCode = 2;
        private int _primaryOHThreePhaseSubtypeCode = 3;
        // primary UG subtypes
        private int _primaryUGSinglePhaseSubtypeCode = 1;
        private int _primaryUGTwoPhaseSubtypeCode = 2;
        private int _primaryUGThreePhaseSubtypeCode = 3;
        // secondary OH subtypes
        private int _secondaryOHSinglePhaseSubtypeCode = 1;
        private int _secondaryOHThreePhaseSubtypeCode = 2;
        private int _secondaryOHServiceSubtypeCode = 3;
        private int _secondaryOHStreetlightSubtypeCode = 4;
        private int _secondaryOHPseudoServiceSubtypeCode = 5;
        // secondary UG subtypes
        private int _secondaryUGSinglePhaseSubtypeCode = 1;
        private int _secondaryUGThreePhaseSubtypeCode = 2;
        private int _secondaryUGServiceSubtypeCode = 3;
        private int _secondaryUGStreetlightSubtypeCode = 4;
        //private int _secondaryUGTieSubtypeCode = 4;
        private int _secondaryUGPseudoServiceSubtypeCode = 6;
        //private int _secondaryUGBusDuctSubtypeCode = 6;

        //private int _serverity = 8;
        //private int _handle = 0;
        //private string _errorMessage = "Invalid source line subtype. ";
        private string _errorDetail = string.Empty;
        private string _errorFeatureMsg = string.Empty;

        private static string[] _modelNames = new string[] { SchemaInfo.Electric.ClassModelNames.Conductor };
        private bool _targetIsPrimaryOH = false;
        private bool _targetIsPrimaryUG = false;
        private bool _targetIsSecondaryOH = false;
        private bool _targetIsSecondaryUG = false;
        private int _targetSubtypeCD = -1;
        private IFeature upstreamFeature = null;
        private IGeometricNetwork geomNetwork = null;


        /// <summary>
        /// Return the subtype code
        /// </summary>
        /// <param name="pRow"></param>
        /// <returns></returns>
        private int GetSubtypeCD(IRow pRow)
        {
            //cast the Table to IObject class
            int subtypecode = -1;
            IObjectClass objectClass = pRow.Table as IObjectClass;
            //check if casting is successful
            if (objectClass != null)
            {
                //cast the objectclass to ISybtypes
                ISubtypes subtypesOfClass = (ISubtypes)objectClass;
                //check if it has subtypes
                if (subtypesOfClass.HasSubtype)
                {
                    //return the subtypecode
                    _logger.Debug("Objectclass:" + objectClass.AliasName + " do have subtypes.");
                    return ((IRowSubtypes)pRow).SubtypeCode;
                }
                _logger.Debug("Objectclass:" + objectClass.AliasName + " doesn't have subtypes.");
            }
            //return the -1 value
            return subtypecode;
        }

        /// <summary>
        /// Return the subtype name
        /// </summary>
        /// <param name="pRow"></param>
        /// <returns></returns>
        private string GetSubtypeName(IRow pRow)
        {
            //cast Itable to IObjectclass
            string subtypename = string.Empty;
            IObjectClass objectClass = pRow.Table as IObjectClass;
            //check if casting is successful
            if (objectClass != null)
            {
                //cast IObjectclass to USubtypes
                ISubtypes subtypesOfClass = (ISubtypes)objectClass;
                //check if casting is successful
                if (subtypesOfClass != null)
                {
                    //get the subtype code
                    int subtypecode = Convert.ToInt32(pRow.get_Value(subtypesOfClass.SubtypeFieldIndex));
                    //get the subtype name for corresponding subtype code
                    _logger.Debug(string.Format("Subtype code retrieved {0}", subtypecode));
                    subtypename = subtypesOfClass.get_SubtypeName(subtypecode);
                    _logger.Debug(string.Format("Subtype name retrieved {0}", subtypename));
                }
                else
                {
                    _logger.Debug(string.Format("Casting to Isubtypes of Object class {0} failed.", objectClass.AliasName));
                }
            }
            else
            {
                _logger.Debug("Casting of Itable to IObjectclass is failed.");
            }
            return subtypename;
        }

        /// <summary>
        /// Apply all 12 rules relevant to sourcing
        /// </summary>
        /// <param name="upsteamConductor">The Upstream conductor.</param>
        /// <param name="source">The source.</param>
        /// <param name="fc">The featureclass.</param>
        /// <returns></returns>
        private bool UpstreamConductorPhaseViolation(IMMPathElement upsteamConductor, IFeature source, IFeatureClass fc)
        {
            upstreamFeature = GetFeatureForEID(fc, upsteamConductor.EID);
            if (upstreamFeature != null)
            {
                int upstreamSubtypeCD = GetSubtypeCD((IRow)upstreamFeature);
                _logger.Debug(string.Format("Subtype code is {0}.", upstreamSubtypeCD));
                if (upstreamSubtypeCD != -1)
                {
                    //set flag for PriOH, PriUG, SecOH or SecUG
                    bool upstreamIsPrimaryOH = ModelNameFacade.ContainsClassModelName(upstreamFeature.Class, _primaryOHModellName);
                    bool upstreamIsPrimaryUG = ModelNameFacade.ContainsClassModelName(upstreamFeature.Class, _primaryUGModellName);
                    bool upstreamIsSecondaryOH = ModelNameFacade.ContainsClassModelName(upstreamFeature.Class, _secondaryOHModellName);
                    bool upstreamIsSecondaryUG = ModelNameFacade.ContainsClassModelName(upstreamFeature.Class, _secondaryUGModellName);
                    _logger.Debug(string.Format("Featureclass {0}, IsPriOH: {1}, IsPriUG: {2}, IsSecOH: {3}, IsSecUG: {4}.", upstreamFeature.Class.AliasName, upstreamIsPrimaryOH, upstreamIsPrimaryUG, upstreamIsSecondaryOH, upstreamIsSecondaryUG));
                    
                    //EDARCFM0398	ArcFM shall ensure that Subtype Service cannot be the sourcing line for subtype single phase or three phase	Data Validation - Secondary OH conductor
                    if (_targetIsSecondaryOH && (_targetSubtypeCD == _secondaryOHSinglePhaseSubtypeCode || _targetSubtypeCD == _secondaryOHThreePhaseSubtypeCode))
                    {
                        if ((upstreamIsSecondaryOH && (upstreamSubtypeCD == _secondaryOHServiceSubtypeCode)) || (upstreamIsSecondaryUG && (upstreamSubtypeCD == _secondaryUGServiceSubtypeCode)))
                        {
                            _logger.Debug("Rule violation EDARCFM0398.");
                            return true;
                        }
                    }
                    //EDARCFM0399	ArcFM shall ensure that Subtype single phase cannot be the sourcing line for subtype three phase	Data Validation - Secondary OH conductor
                    if (_targetIsSecondaryOH && (_targetSubtypeCD == _secondaryOHThreePhaseSubtypeCode))
                    {
                        if ((upstreamIsSecondaryOH && (upstreamSubtypeCD == _secondaryOHSinglePhaseSubtypeCode)) || (upstreamIsSecondaryUG && (upstreamSubtypeCD == _secondaryUGThreePhaseSubtypeCode)))
                        {
                            _logger.Debug("Rule violation EDARCFM0399.");
                            return true;
                        }
                    }
                    // EDARCFM0400	ArcFM shall ensure that Subtype pseudo service cannot be the sourcing line for subtypes service, single phase, or three phase	Data Validation - Secondary OH conductor
                    if (_targetIsSecondaryOH && (_targetSubtypeCD == _secondaryOHServiceSubtypeCode || _targetSubtypeCD == _secondaryOHSinglePhaseSubtypeCode || _targetSubtypeCD == _secondaryOHThreePhaseSubtypeCode))
                    {
                        if ((upstreamIsSecondaryOH && (upstreamSubtypeCD == _secondaryOHPseudoServiceSubtypeCode)) || (upstreamIsSecondaryUG && (upstreamSubtypeCD == _secondaryUGPseudoServiceSubtypeCode)))
                        {
                            _logger.Debug("Rule violation EDARCFM0400.");
                            return true;
                        }
                    }
                    // EDARCFM0401	ArcFM shall ensure that Subtype streetlight cannot be the sourcing line for subtype service, single phase, or three phase	Data Validation - Secondary OH conductor
                    if (_targetIsSecondaryOH && (_targetSubtypeCD == _secondaryOHServiceSubtypeCode || _targetSubtypeCD == _secondaryOHSinglePhaseSubtypeCode || _targetSubtypeCD == _secondaryOHThreePhaseSubtypeCode))
                    {
                        if ((upstreamIsSecondaryOH && (upstreamSubtypeCD == _secondaryOHStreetlightSubtypeCode)) || (upstreamIsSecondaryUG && (upstreamSubtypeCD == _secondaryUGStreetlightSubtypeCode)))
                        {
                            _logger.Debug("Rule violation EDARCFM0401.");
                            return true;
                        }
                    }
                    // EDARCFM0520	ArcFM shall ensure that Single phase primary cannot be the sourcing line for two or three phase primary.	Data Validation - Primary OH conductor
                    if (_targetIsPrimaryOH && (_targetSubtypeCD == _primaryOHTwoPhaseSubtypeCode || _targetSubtypeCD == _primaryOHThreePhaseSubtypeCode))
                    {
                        if ((upstreamIsPrimaryOH && (upstreamSubtypeCD == _primaryOHSinglePhaseSubtypeCode)) || (upstreamIsPrimaryUG && (upstreamSubtypeCD == _primaryUGSinglePhaseSubtypeCode)))
                        {
                            _logger.Debug("Rule violation EDARCFM0520.");
                            return true;
                        }
                    }
                    // EDARCFM0521	ArcFM shall ensure that Two phase primary cannot be the sourcing line for three phase primary.	Data Validation - Primary OH conductor
                    if (_targetIsPrimaryOH && (_targetSubtypeCD == _primaryOHThreePhaseSubtypeCode))
                    {
                        if ((upstreamIsPrimaryOH && (upstreamSubtypeCD == _primaryOHTwoPhaseSubtypeCode)) || (upstreamIsPrimaryUG && (upstreamSubtypeCD == _primaryUGTwoPhaseSubtypeCode)))
                        {
                            _errorFeatureMsg = string.Format("upstream feature class : {0}  OID : {1}", upstreamFeature.Class.AliasName, upstreamFeature.OID);
                            _logger.Debug("Rule violation EDARCFM0521.");
                            _logger.Debug(_errorFeatureMsg);
                            return true;
                        }
                    }
                    // EDARCFM0629	ArcFM shall ensure that the Subtype Service cannot be the sourcing line for subtype single phase or three phase	Data Validation - Secondary UG conductor
                    if (_targetIsSecondaryUG && (_targetSubtypeCD == _secondaryUGSinglePhaseSubtypeCode || _targetSubtypeCD == _secondaryUGThreePhaseSubtypeCode))
                    {
                        if ((upstreamIsSecondaryOH && (upstreamSubtypeCD == _secondaryOHServiceSubtypeCode)) || (upstreamIsSecondaryUG && (upstreamSubtypeCD == _secondaryUGServiceSubtypeCode)))
                        {
                            _logger.Debug("Rule violation EDARCFM0629.");
                            return true;
                        }
                    }
                    // EDARCFM0630	ArcFM shall ensure that the Subtype single phase cannot be the sourcing line for subtype three phase	Data Validation - Secondary UG conductor
                    if (_targetIsSecondaryUG && (_targetSubtypeCD == _secondaryUGThreePhaseSubtypeCode))
                    {
                        if ((upstreamIsSecondaryOH && (upstreamSubtypeCD == _secondaryOHSinglePhaseSubtypeCode)) || (upstreamIsSecondaryUG && (upstreamSubtypeCD == _secondaryUGSinglePhaseSubtypeCode)))
                        {
                            _logger.Debug("Rule violation EDARCFM0630.");
                            return true;
                        }
                    }
                    // EDARCFM0631	ArcFM shall ensure that Subtype pseudo service cannot be the sourcing line for subtypes service, single phase, or three phase	Data Validation - Secondary UG conductor
                    if (_targetIsSecondaryUG && (_targetSubtypeCD == _secondaryUGServiceSubtypeCode || _targetSubtypeCD == _secondaryUGSinglePhaseSubtypeCode || _targetSubtypeCD == _secondaryUGThreePhaseSubtypeCode))
                    {
                        if ((upstreamIsSecondaryOH && (upstreamSubtypeCD == _secondaryOHPseudoServiceSubtypeCode)) || (upstreamIsSecondaryUG && (upstreamSubtypeCD == _secondaryUGPseudoServiceSubtypeCode)))
                        {
                            _logger.Debug("Rule violation EDARCFM0631.");
                            return true;
                        }
                    }
                    // EDARCFM0632	ArcFM shall ensure that Subtype streetlight cannot be the sourcing line for subtype service, single phase, or three phase	Data Validation - Secondary UG conductor
                    if (_targetIsSecondaryUG && (_targetSubtypeCD == _secondaryUGServiceSubtypeCode || _targetSubtypeCD == _secondaryUGSinglePhaseSubtypeCode || _targetSubtypeCD == _secondaryUGThreePhaseSubtypeCode))
                    {
                        if ((upstreamIsSecondaryOH && (upstreamSubtypeCD == _secondaryOHStreetlightSubtypeCode)) || (upstreamIsSecondaryUG && (upstreamSubtypeCD == _secondaryUGStreetlightSubtypeCode)))
                        {
                            _logger.Debug("Rule violation EDARCFM0632.");
                            return true;
                        }
                    }
                    // EDARCFM0669	ArcFM shall ensure that Single phase primary cannot be the sourcing line for two or three phase primary.	Data Validation - Primary UG conductor
                    if (_targetIsPrimaryUG && (_targetSubtypeCD == _primaryUGTwoPhaseSubtypeCode || _targetSubtypeCD == _primaryUGThreePhaseSubtypeCode))
                    {
                        if ((upstreamIsPrimaryOH && (upstreamSubtypeCD == _primaryOHSinglePhaseSubtypeCode)) || (upstreamIsPrimaryUG && (upstreamSubtypeCD == _primaryUGSinglePhaseSubtypeCode)))
                        {
                            _logger.Debug("Rule violation EDARCFM0669.");
                            return true;
                        }
                    }
                    // EDARCFM0670	ArcFM shall ensure that Two phase primary cannot be the sourcing line for three phase primary.	Data Validation - Primary UG conductor
                    if (_targetIsPrimaryUG && (_targetSubtypeCD == _primaryUGThreePhaseSubtypeCode))
                    {
                        if ((upstreamIsPrimaryOH && (upstreamSubtypeCD == _primaryOHTwoPhaseSubtypeCode)) || (upstreamIsPrimaryUG && (upstreamSubtypeCD == _primaryUGTwoPhaseSubtypeCode)))
                        {
                            _logger.Debug("Rule violation EDARCFM0670.");
                            return true;
                        }
                    }

                }
            }

            _logger.Debug("No rule violated.");
            return false;
        }

        /// <summary>
        /// Given 2 Conductor feature wll check if there are any invalid phase designations between the features.
        /// </summary>
        /// <param name="upstreamFeature">Connected Upstream Feature</param>
        /// <param name="source">Source Feature on which the Validation is run</param>
        /// <returns>Returns boolean indicating hte test conditions pass or failed.</returns>
        private bool UpstreamConductorPhaseViolation(IFeature upstreamFeature, IFeature source)
        {
            if (upstreamFeature != null)
            {
                int upstreamSubtypeCD = GetSubtypeCD((IRow)upstreamFeature);
                _logger.Debug(string.Format("Subtype code is {0}.", upstreamSubtypeCD));

                if (upstreamSubtypeCD != -1)
                {
                    bool upstreamIsPrimaryOH = ModelNameFacade.ContainsClassModelName(upstreamFeature.Class, _primaryOHModellName);
                    bool upstreamIsPrimaryUG = ModelNameFacade.ContainsClassModelName(upstreamFeature.Class, _primaryUGModellName);
                    bool upstreamIsSecondaryOH = ModelNameFacade.ContainsClassModelName(upstreamFeature.Class, _secondaryOHModellName);
                    bool upstreamIsSecondaryUG = ModelNameFacade.ContainsClassModelName(upstreamFeature.Class, _secondaryUGModellName);
                    _logger.Debug(string.Format("Featureclass {0}, IsPriOH: {1}, IsPriUG: {2}, IsSecOH: {3}, IsSecUG: {4}.", upstreamFeature.Class.AliasName, upstreamIsPrimaryOH, upstreamIsPrimaryUG, upstreamIsSecondaryOH, upstreamIsSecondaryUG));

                    //EDARCFM0398	ArcFM shall ensure that Subtype Service cannot be the sourcing line for subtype single phase or three phase	Data Validation - Secondary OH conductor
                    if (_targetIsSecondaryOH && (_targetSubtypeCD == _secondaryOHSinglePhaseSubtypeCode || _targetSubtypeCD == _secondaryOHThreePhaseSubtypeCode))
                    {
                        if ((upstreamIsSecondaryOH && (upstreamSubtypeCD == _secondaryOHServiceSubtypeCode)) || (upstreamIsSecondaryUG && (upstreamSubtypeCD == _secondaryUGServiceSubtypeCode)))
                        {
                            _logger.Debug("Rule violation EDARCFM0398.");
                            return true;
                        }
                    }
                    //EDARCFM0399	ArcFM shall ensure that Subtype single phase cannot be the sourcing line for subtype three phase	Data Validation - Secondary OH conductor
                    if (_targetIsSecondaryOH && (_targetSubtypeCD == _secondaryOHThreePhaseSubtypeCode))
                    {
                        if ((upstreamIsSecondaryOH && (upstreamSubtypeCD == _secondaryOHSinglePhaseSubtypeCode)) || (upstreamIsSecondaryUG && (upstreamSubtypeCD == _secondaryUGThreePhaseSubtypeCode)))
                        {
                            _logger.Debug("Rule violation EDARCFM0399.");
                            return true;
                        }
                    }
                    // EDARCFM0400	ArcFM shall ensure that Subtype pseudo service cannot be the sourcing line for subtypes service, single phase, or three phase	Data Validation - Secondary OH conductor
                    if (_targetIsSecondaryOH && (_targetSubtypeCD == _secondaryOHServiceSubtypeCode || _targetSubtypeCD == _secondaryOHSinglePhaseSubtypeCode || _targetSubtypeCD == _secondaryOHThreePhaseSubtypeCode))
                    {
                        if ((upstreamIsSecondaryOH && (upstreamSubtypeCD == _secondaryOHPseudoServiceSubtypeCode)) || (upstreamIsSecondaryUG && (upstreamSubtypeCD == _secondaryUGPseudoServiceSubtypeCode)))
                        {
                            _logger.Debug("Rule violation EDARCFM0400.");
                            return true;
                        }
                    }
                    // EDARCFM0401	ArcFM shall ensure that Subtype streetlight cannot be the sourcing line for subtype service, single phase, or three phase	Data Validation - Secondary OH conductor
                    if (_targetIsSecondaryOH && (_targetSubtypeCD == _secondaryOHServiceSubtypeCode || _targetSubtypeCD == _secondaryOHSinglePhaseSubtypeCode || _targetSubtypeCD == _secondaryOHThreePhaseSubtypeCode))
                    {
                        if ((upstreamIsSecondaryOH && (upstreamSubtypeCD == _secondaryOHStreetlightSubtypeCode)) || (upstreamIsSecondaryUG && (upstreamSubtypeCD == _secondaryUGStreetlightSubtypeCode)))
                        {
                            _logger.Debug("Rule violation EDARCFM0401.");
                            return true;
                        }
                    }
                    // EDARCFM0520	ArcFM shall ensure that Single phase primary cannot be the sourcing line for two or three phase primary.	Data Validation - Primary OH conductor
                    if (_targetIsPrimaryOH && (_targetSubtypeCD == _primaryOHTwoPhaseSubtypeCode || _targetSubtypeCD == _primaryOHThreePhaseSubtypeCode))
                    {
                        if ((upstreamIsPrimaryOH && (upstreamSubtypeCD == _primaryOHSinglePhaseSubtypeCode)) || (upstreamIsPrimaryUG && (upstreamSubtypeCD == _primaryUGSinglePhaseSubtypeCode)))
                        {
                            _logger.Debug("Rule violation EDARCFM0520.");
                            return true;
                        }
                    }
                    // EDARCFM0521	ArcFM shall ensure that Two phase primary cannot be the sourcing line for three phase primary.	Data Validation - Primary OH conductor
                    if (_targetIsPrimaryOH && (_targetSubtypeCD == _primaryOHThreePhaseSubtypeCode))
                    {
                        if ((upstreamIsPrimaryOH && (upstreamSubtypeCD == _primaryOHTwoPhaseSubtypeCode)) || (upstreamIsPrimaryUG && (upstreamSubtypeCD == _primaryUGTwoPhaseSubtypeCode)))
                        {
                            _errorFeatureMsg = string.Format("upstream feature class : {0}  OID : {1}", upstreamFeature.Class.AliasName, upstreamFeature.OID);
                            _logger.Debug("Rule violation EDARCFM0521.");
                            _logger.Debug(_errorFeatureMsg);
                            return true;
                        }
                    }
                    // EDARCFM0629	ArcFM shall ensure that the Subtype Service cannont be the sourcing line for subtype single phase or three phase	Data Validation - Secondary UG conductor
                    if (_targetIsSecondaryUG && (_targetSubtypeCD == _secondaryUGSinglePhaseSubtypeCode || _targetSubtypeCD == _secondaryUGThreePhaseSubtypeCode))
                    {
                        if ((upstreamIsSecondaryOH && (upstreamSubtypeCD == _secondaryOHServiceSubtypeCode)) || (upstreamIsSecondaryUG && (upstreamSubtypeCD == _secondaryUGServiceSubtypeCode)))
                        {
                            return true;
                        }
                    }
                    // EDARCFM0630	ArcFM shall ensure that the Subtype single phase cannot be the sourcing line for subtype three phase	Data Validation - Secondary UG conductor
                    if (_targetIsSecondaryUG && (_targetSubtypeCD == _secondaryUGThreePhaseSubtypeCode))
                    {
                        if ((upstreamIsSecondaryOH && (upstreamSubtypeCD == _secondaryOHSinglePhaseSubtypeCode)) || (upstreamIsSecondaryUG && (upstreamSubtypeCD == _secondaryUGSinglePhaseSubtypeCode)))
                        {
                            _logger.Debug("Rule violation EDARCFM0630.");
                            return true;
                        }
                    }
                    // EDARCFM0631	ArcFM shall ensure that Subtype pseudo service cannot be the sourcing line for subtypes service, single phase, or three phase	Data Validation - Secondary UG conductor
                    if (_targetIsSecondaryUG && (_targetSubtypeCD == _secondaryUGServiceSubtypeCode || _targetSubtypeCD == _secondaryUGSinglePhaseSubtypeCode || _targetSubtypeCD == _secondaryUGThreePhaseSubtypeCode))
                    {
                        if ((upstreamIsSecondaryOH && (upstreamSubtypeCD == _secondaryOHPseudoServiceSubtypeCode)) || (upstreamIsSecondaryUG && (upstreamSubtypeCD == _secondaryUGPseudoServiceSubtypeCode)))
                        {
                            _logger.Debug("Rule violation EDARCFM0631.");
                            return true;
                        }
                    }
                    // EDARCFM0632	ArcFM shall ensure that Subtype streetlight cannot be the sourcing line for subtype service, single phase, or three phase	Data Validation - Secondary UG conductor
                    if (_targetIsSecondaryUG && (_targetSubtypeCD == _secondaryUGServiceSubtypeCode || _targetSubtypeCD == _secondaryUGSinglePhaseSubtypeCode || _targetSubtypeCD == _secondaryUGThreePhaseSubtypeCode))
                    {
                        if ((upstreamIsSecondaryOH && (upstreamSubtypeCD == _secondaryOHStreetlightSubtypeCode)) || (upstreamIsSecondaryUG && (upstreamSubtypeCD == _secondaryUGStreetlightSubtypeCode)))
                        {
                            _logger.Debug("Rule violation EDARCFM0632.");
                            return true;
                        }
                    }
                    // EDARCFM0669	ArcFM shall ensure that Single phase primary cannot be the sourcing line for two or three phase primary.	Data Validation - Primary UG conductor
                    if (_targetIsPrimaryUG && (_targetSubtypeCD == _primaryUGTwoPhaseSubtypeCode || _targetSubtypeCD == _primaryUGThreePhaseSubtypeCode))
                    {
                        if ((upstreamIsPrimaryOH && (upstreamSubtypeCD == _primaryOHSinglePhaseSubtypeCode)) || (upstreamIsPrimaryUG && (upstreamSubtypeCD == _primaryUGSinglePhaseSubtypeCode)))
                        {
                            _logger.Debug("Rule violation EDARCFM0669.");
                            return true;
                        }
                    }
                    // EDARCFM0670	ArcFM shall ensure that Two phase primary cannot be the sourcing line for three phase primary.	Data Validation - Primary UG conductor
                    if (_targetIsPrimaryUG && (_targetSubtypeCD == _primaryUGThreePhaseSubtypeCode))
                    {
                        if ((upstreamIsPrimaryOH && (upstreamSubtypeCD == _primaryOHTwoPhaseSubtypeCode)) || (upstreamIsPrimaryUG && (upstreamSubtypeCD == _primaryUGTwoPhaseSubtypeCode)))
                        {
                            _logger.Debug("Rule violation EDARCFM0670.");
                            return true;
                        }
                    }

                }
            }
            _logger.Debug("No rule violated.");
            return false;
        }

        /// <summary>
        /// Returns the network id (EID) of the given feature
        /// </summary>
        /// <param name="feature"></param>
        /// <returns></returns>
        private int GetFeatureEID(IFeature feature)
        {
            //cast the feature class
            IFeatureClass fc = feature.Class as IFeatureClass;
            _logger.Debug(string.Format("Casted feature.Class to featureclass : {0}.", fc.AliasName));
            //cast newworl elements from network
            INetElements netElements = (INetElements)(feature as INetworkFeature).GeometricNetwork.Network;
            //get the EID corresponding to feature.OID
            IEnumNetEID eids = netElements.GetEIDs(fc.FeatureClassID, feature.OID, esriElementType.esriETEdge);
            //cehck if EID are not empty
            if (eids.Count == 0)
            {
                _logger.Debug(string.Format("Retrieved enum EID is empty."));
                return 0;
            }
            //return the first EID
            _logger.Debug(string.Format("Retrieved enum EID count is {0}.", eids.Count));
            return eids.Next();
        }

        /// <summary>
        /// Returns the feature associated to the given eid, for the given feature class
        /// </summary>
        /// <param name="featureClass"></param>
        /// <param name="eid"></param>
        /// <returns></returns>
        private IFeature GetFeatureForEID(IFeatureClass featureClass, int eid)
        {
            int userClassID = 0;
            int userID = 0;
            int userSubID = 0;
            //cast geaometry network to network elements
            INetElements netElements = (INetElements)geomNetwork.Network;
            //get the UserID from corresponding eid
            netElements.QueryIDs(eid, esriElementType.esriETEdge,
                        out userClassID, out userID, out userSubID);
            //retrieve the feature fro, UserID
            _logger.Debug("User ID retrived is :" + userID);
            return featureClass.GetFeature(userID);
        }

        private bool IsPhaseDesignationValid(IFeature validatingFeature, IFeature upstreamFeature)
        {
            bool retVal = false;
            int validatingFeaturePhase = validatingFeature.GetFieldValue(null, false, SchemaInfo.Electric.FieldModelNames.PhaseDesignation).Convert<int>(int.MinValue);
            int upstreamFeaturePhase = upstreamFeature.GetFieldValue(null, false, SchemaInfo.Electric.FieldModelNames.PhaseDesignation).Convert<int>(int.MinValue);
            if (validatingFeaturePhase > 0 && upstreamFeaturePhase >0)
            {
                //The Logical AND will result in ValidatingFeature's Phase if the Validating Feature's Phase is a subset of UpstreamFeature's Phase else it will be different.
                //0001 (1) & 0010 (2) == 0010 (2) so 2 is not a subset of 1 but 0001(1) & 0011(3) == 0010(1) and 0010(2) & 0011(3) == 0010(2) so in above scenarios 1&2 are subset of 3
                retVal = (validatingFeaturePhase & upstreamFeaturePhase) == validatingFeaturePhase;
            }
            return retVal;
        }
        #endregion Private

        #region Constructor
        public SourceLinePhase()
            : base("PGE Validate SourceLine Phase", _modelNames)
        {
        }
        #endregion Constructor

        #region Override for validation rule

        /// <summary>
        /// Flag an error if there is any upstream conductor that violates the phasing rules
        /// </summary>
        /// <param name="pRow">the conductor segment to validate</param>
        /// <returns>the error message</returns>
        protected override ID8List InternalIsValid(ESRI.ArcGIS.Geodatabase.IRow pRow)
        {
            IDataset pDS = null; 

            try
            {

                bool phasingInvalid = false;
                IObject obj = pRow as IObject;
                IFeature source = pRow as IFeature;
                pDS = (IDataset)pRow.Table; 

                _targetIsPrimaryOH = ModelNameFacade.ContainsClassModelName(obj.Class, _primaryOHModellName);
                _targetIsPrimaryUG = ModelNameFacade.ContainsClassModelName(obj.Class, _primaryUGModellName);
                _targetIsSecondaryOH = ModelNameFacade.ContainsClassModelName(obj.Class, _secondaryOHModellName);
                _targetIsSecondaryUG = ModelNameFacade.ContainsClassModelName(obj.Class, _secondaryUGModellName);
                _targetSubtypeCD = GetSubtypeCD(pRow);
                _logger.Debug(string.Format("Subtype: {0}, IsPriOH: {1}, IsPriUG: {2}, IsSecOH: {3}, IsSecUG: {4}", _targetSubtypeCD, _targetIsPrimaryOH, _targetIsPrimaryUG, _targetIsSecondaryOH, _targetIsSecondaryUG));

                #region Old Code
                //if (_targetSubtypeCD == -1)
                //    return null;

                //geomNetwork = (source as INetworkFeature).GeometricNetwork;
                //// Find the source EID.
                //int startEID = 0;

                //if (source is ISimpleEdgeFeature)
                //{
                //    startEID = (source as ISimpleEdgeFeature).EID;
                //}
                //else if (source is IComplexEdgeFeature)
                //{
                //    startEID = GetFeatureEID(source);
                //}
                //else
                //{
                //    return null;
                //}
                //int[] barrierJuncs = new int[0];
                //int[] barrierEdges = new int[0];
                //IMMFeedPath[] feedPaths;
                //IMMTraceStopper[] StopperJuncs;
                //IMMTraceStopper[] StopperEdges;

                //IMMElectricTraceSettings mmEleTraceSettings = new MMElectricTraceSettingsClass();
                //IMMElectricTracing mmEleTracing = new MMFeederTracerClass();

                ////Call the trace
                //mmEleTracing.FindFeedPaths(geomNetwork, mmEleTraceSettings, this, startEID, esriElementType.esriETEdge, SetOfPhases.abc,
                //                            barrierJuncs, barrierEdges, out feedPaths, out StopperJuncs, out StopperEdges);


                // go over the feedPaths and examine all the upstream conductors for phasing issues

                //// go over the feedPaths examing all the upstream conductors for phasing issues
                //if (feedPaths.Length == 0) return null;

                //IFeatureClassContainer fcContainer = (IFeatureClassContainer)geomNetwork;
                //IFeatureClass fc = null;
                //phasingInvalid = false;
                //foreach (IMMFeedPath path in feedPaths)
                //{
                //    foreach (IMMPathElement element in path.PathElements)
                //    {
                //        fc = fcContainer.get_ClassByID(element.FCID);
                //        if (ModelNameFacade.ContainsClassModelName(fc, _conductorModelName))
                //        {
                //            if (element.EID == startEID)
                //                continue;
                //            if (UpstreamConductorPhaseViolation(element, source, fc) == true)
                //            {
                //                phasingInvalid = true;
                //                break;
                //            }
                //        }
                //    }
                //    if (phasingInvalid) break;
                //}
                #endregion

                //get the upstream feature
                IFeature upstreamFeature = TraceFacade.GetFirstUpstreamEdge(source);
                //If Upstream feature is not found return
                if (upstreamFeature == null) return _ErrorList;
                IFeature phasingInvalidFeature = null;
                //foreach (IFeature upstreamFeature in upstreamFeatures)
                //{
                phasingInvalid = UpstreamConductorPhaseViolation(upstreamFeature, source);
                _logger.Debug(string.Format("Phasing is invalid: {0}", phasingInvalid));
                if (phasingInvalid)
                {
                    phasingInvalidFeature = upstreamFeature;
                    //break;
                }
                //}
                //ID8List list = null;
                if (phasingInvalid)
                {
                    ////phasing is invalid add the error to validation
                    //list = new D8List();
                    //IMMValidationError validationError = new MMValidationErrorClass();
                    //validationError.BitmapID = _handle;

                    //string upstreamSubtypeName = GetSubtypeName((IRow)upstreamFeature);
                    string upstreamSubtypeName = GetSubtypeName((IRow)phasingInvalidFeature);
                    // This feature cannot be sourced by PriUGConductor Subtype:SinglePhasePrimaryUnderground (OID.123)
                    _errorFeatureMsg = string.Format("This feature cannot be sourced by {0} Subtype:{1} (OID.{2})", phasingInvalidFeature.Class.AliasName, upstreamSubtypeName, phasingInvalidFeature.OID);
                    //validationError.ErrorMessage = _errorFeatureMsg;
                    //validationError.Severity = _serverity;
                    _logger.Debug(_errorFeatureMsg);
                    AddError(_errorFeatureMsg);
                    //list.Add(validationError as ID8ListItem);
                }
                //Check if PhaseDesignation is Invalid
                bool phaseDesignationValid = IsPhaseDesignationValid(source, upstreamFeature);
                if (!phaseDesignationValid)
                {
                    phasingInvalidFeature = upstreamFeature;
                    AddError(string.Format("The phase designation: {0} of the feature with OID:{1} is not valid for the source line feature with OID:{2} with phase designation of {3}",
                                            source.GetFieldValue(null, true, SchemaInfo.Electric.FieldModelNames.PhaseDesignation),
                                            source.OID,
                                            upstreamFeature.OID,
                                            upstreamFeature.GetFieldValue(null, true, SchemaInfo.Electric.FieldModelNames.PhaseDesignation)));
                }
                return _ErrorList;
            }
            catch (Exception ex) 
            {
                _logger.Debug("Error on sourcelinephase on feature " + 
                    pDS.Name + " oid: " + pRow.OID.ToString() + " : " + ex.Message);
                return _ErrorList;
            }
        }

        #endregion Override for validation rule

        #region Public
        /// <summary>
        /// Implement IMMCurrentStatus
        /// </summary>
        /// <param name="EID"></param>
        /// <param name="FCID"></param>
        /// <param name="OID"></param>
        /// <param name="SUBID"></param>
        /// <param name="ElementType"></param>
        /// <param name="weightVal"></param>
        public void GetCurrentStatusAsWeightValue(int EID, int FCID, int OID, int SUBID, esriElementType ElementType, ref int weightVal)
        {
            //Unused
        }
        #endregion Public
    }
}
