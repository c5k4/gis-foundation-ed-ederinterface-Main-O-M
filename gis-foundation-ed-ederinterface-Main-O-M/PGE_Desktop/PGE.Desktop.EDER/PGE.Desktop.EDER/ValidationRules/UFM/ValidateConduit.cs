using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

using Miner.ComCategories;
using Miner.Interop;

using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Diagnostics;
using PGE.Desktop.EDER;
using PGE.Desktop.EDER.UFM;

namespace PGE.Desktop.EDER.ValidationRules.UFM
{
    [ComVisible(true)]
    [Guid("EF918768-03A1-41D6-B0A0-F78F88B2D8B2")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComponentCategory(ComCategory.MMValidationRules)]
    public class ValidateConduit : BaseValidationRule
    {
        #region Constants

        // For managing errors
        private const string UFM_DIAGRAM_VALIDATION_RULE = "PGE Validate Butterfly Diagrams";
        private const int VALIDATION_TYPE_WARN = 1;

        #endregion

        #region Member vars

        // For error handling
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public ValidateConduit() : base(UFM_DIAGRAM_VALIDATION_RULE, SchemaInfo.UFM.ClassModelNames.Conduit)
        {
        }

        #endregion

        #region Base Validation Rule Overrides

        protected override ID8List InternalIsValid(IRow row)
        {
            // Log entry
            _logger.Debug("Validating UFM Conduit");
            if (row != null)
            {
                _logger.Debug("Class: " + (row as IObject).Class.AliasName);
                _logger.Debug("OID  : " + row.OID.ToString());
            }

            try
            {
                // If this rule is being filtered out - do not run it 
                if (ValidationFilterEngine.Instance.IsQAQCRuleEnabled(_Name, VALIDATION_TYPE_WARN) &&
                    ShouldValidate(row))
                {
                    // Checks - Ensure direction is set, blob set on conduit, and both ends of conduit have butterfly
                    object directionObj = CheckAndGetDirection(row);
                    IMMDuctBankConfig dbc = CheckAndGetDuctBankConfig(row);
                    ISet ductBanks = CheckAndGetDiagrams(row);

                    if (directionObj != null && dbc != null && ductBanks != null &&
                        ductBanks.Count == 2)
                    {
                        // Check #4 - Check the butterfly diagrams on each wall match the blob
                        CheckButterflies(row, directionObj, dbc, ductBanks);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log and let the user know we failed
                string message = "UFM diagram validation failed to validate. See error log for more details.";
                _logger.Warn(message + ": " + ex.ToString());
                AddError(message);
            }

            // Return the result
            return _ErrorList;
        }

        #endregion

        #region Private methods

        #region Code to determine if we need to validate

        private bool ShouldValidate(IRow pRow)
        {
            //subtype is ductbank
            IRowSubtypes subType = pRow as IRowSubtypes;
            return subType.SubtypeCode == 1;
        }

        #endregion

        #region Code for check #1

        private object CheckAndGetDirection(IRow pRow)
        {
            _logger.Debug("Validating direction present");
            object directionObj = null;            

            try
            {
                // Check #1 - Is direction set on the conduit                
                directionObj = UfmHelper.GetFieldValue(pRow, SchemaInfo.UFM.FieldModelNames.Direction);
                if (directionObj == null)
                {
                    AddError("Conduit direction indicator not set");
                }
            }
            catch (Exception ex)
            {
                _logger.Warn("Failed to check direction: " + ex.ToString());
                throw ex;
            }

            // Return the result
            return directionObj;
        }

        #endregion

        #region Code for check #2

        private IMMDuctBankConfig CheckAndGetDuctBankConfig(IRow pRow)
        {            
            _logger.Debug("Validating blob present");
            IMMDuctBankConfig dbc = null;

            try
            {
                // Check #2 - Check that the configuration blob field is set
                dbc = UfmHelper.GetDuctBankConfig(pRow as IFeature);
                if (dbc == null)
                {
                    AddError("Conduit configuration not set");
                }
            }
            catch (Exception ex)
            {
                _logger.Warn("Failed to check if duct bank config is set: " + ex.ToString());
                throw ex;
            }

            return dbc;
        }

        #endregion

        #region Code for check #3

        private ISet CheckAndGetDiagrams(IRow pRow)
        {
            _logger.Debug("Validating duct banks present");
            ISet objSet = null;

            try
            {
                // Check #3 - Ensure two butterfly diagrams are present
                objSet = UfmHelper.GetRelatedObjects(pRow, SchemaInfo.UFM.ClassModelNames.UfmDuctBank);
                if (objSet != null)
                {
                    if (objSet.Count != 2)
                    {
                        AddError(objSet.Count.ToString() + " duct bank butterfly representation(s) found. Conduit must have exactly two butterfly representations");
                    }
                }
                else
                {
                    AddError("No duct bank butterfly representations found. Conduit must have exactly two butterfly representations");
                }
            }
            catch (Exception ex)
            {
                _logger.Warn("Failed to check if butterfly diagrams are present: " + ex.ToString());
                throw ex;
            }

            return objSet;
        }

        #endregion

        #region Code for check #4

        private void CheckButterflies(IRow pRow, object directionObj, IMMDuctBankConfig dbc, ISet ductBanks)
        {
            // Check #4 - Make sure the butterflies match the blob
            _logger.Debug("Validating butterfly diagrams accurate");

            try
            {
                // Check #4.1 - Get matching wall butterfly based on direction value
                IFeature ductBankFeature = GetMatchingDuctBankFeature(pRow, ductBanks, directionObj.ToString());
                if (ductBankFeature == null)
                {
                    AddError("Could not find matching wall butterfly diagram based on direction");
                }

                // Check #4.2 - Get opposite wall butterfly based on direction value
                IFeature oppositeDuctBankFeature = GetNonMatchingDuctBankFeature(pRow, ductBanks, directionObj.ToString());
                if (oppositeDuctBankFeature == null)
                {
                    AddError("Could not find opposite wall butterfly diagram based on direction");
                }

                // If we found both...
                if (ductBankFeature != null && oppositeDuctBankFeature != null)
                {
                    // Create UFMDuctBank objects from the blob and duct banks, so we can compare them
                    UfmDuctBank blobDiagram = new UfmDuctBank(dbc);

                    double matchingAngle = UfmDuctBank.CalculateRotationAngle(ductBankFeature);
                    double oppositeAngle = UfmDuctBank.CalculateRotationAngle(oppositeDuctBankFeature);

                    oppositeAngle = 180 - oppositeAngle;
                    matchingAngle *= -1;

                    UfmDuctBank matchingWallButterfly = new UfmDuctBank(ductBankFeature, matchingAngle, false);
                    UfmDuctBank oppositeWallButterfly = new UfmDuctBank(oppositeDuctBankFeature, oppositeAngle, true);

                    // Check #4.3 - Validate that the matching wall butterfly is the same as the blobs
                    if (!blobDiagram.Compare(matchingWallButterfly))
                    {
                        AddError("Matching wall butterfly diagram does not match cross section annotation");
                    }

                    // Check #4.4 - Validate that the opposite wall butterfly is the 'opposite' of the blob
                    if (!blobDiagram.IsMirrored(oppositeWallButterfly))
                    {
                        AddError("Opposite wall butterfly diagram does not match cross section annotation");
                    }

                    // Check #4.5 - Validate that the wall butterfly diagrams are 'opposite' from each other
                    if (matchingWallButterfly.IsMirrored(oppositeWallButterfly) == false)
                    {
                        AddError("Opposing wall diagrams do not match");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Warn("Failed to check if butterfly diagrams match: " + ex.ToString());
                throw ex;
            }
        }

        #endregion

        #region Code to get butterfly diagrams

        /// <summary>
        /// Returns the duct bank feature from the supplied set of duct banks that matches the direction
        /// on the supplied conduit.
        /// </summary>
        /// <param name="conduit"></param>
        /// <param name="ductBanks"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        private IFeature GetMatchingDuctBankFeature(IRow conduit, ISet ductBanks, string direction)
        {
            IFeature matchingDuctBankFeature = null;

            try
            {
                // Get the first bank feature
                ductBanks.Reset();
                IFeature ductBankFeature = ductBanks.Next() as IFeature;

                // While there are still features to check
                while (ductBankFeature != null)
                {
                    // Get its direction 
                    string bankDirection = GetDirection(conduit, ductBankFeature);

                    // If it matches
                    if (bankDirection == direction)
                    {
                        // We got our feature, bail out
                        matchingDuctBankFeature = ductBankFeature;
                        break;
                    }

                    // Check the next duct bank
                    ductBankFeature = ductBanks.Next() as IFeature;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to get matching duct bank feature: " + ex.ToString());
                throw ex;
            }

            // Return the result
            return matchingDuctBankFeature;
        }

        /// <summary>
        /// Returns the duct bank feature from the supplied set of duct banks that does not match the
        /// direction on the supplied conduit.
        /// </summary>
        /// <param name="conduit"></param>
        /// <param name="ductBanks"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        private IFeature GetNonMatchingDuctBankFeature(IRow conduit, ISet ductBanks, string direction)
        {
            IFeature nonMatchingDuctBankFeature = null;

            try
            {
                // Get the first bank feature
                ductBanks.Reset();
                IFeature ductBankFeature = ductBanks.Next() as IFeature;

                // While there are still features to check
                while (ductBankFeature != null)
                {
                    // Get its direction 
                    string bankDirection = GetDirection(conduit, ductBankFeature);

                    // If it doesn't match
                    if (bankDirection != direction)
                    {
                        // We got out feature, bail out
                        nonMatchingDuctBankFeature = ductBankFeature;
                        break;
                    }

                    ductBankFeature = ductBanks.Next() as IFeature;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to get non-matching duct bank feature: " + ex.ToString());
                throw ex;
            }


            // Return the result
            return nonMatchingDuctBankFeature;
        }

        /// <summary>
        /// Calculates the direction of the supplied conduit relative to the supplied duct bank
        /// </summary>
        /// <param name="conduit"></param>
        /// <param name="ductBank"></param>
        /// <returns>N, E, S, W, NE, SE, SW  or NW</returns>
        private string GetDirection(IRow conduit, IFeature ductBank)
        {
            string direction = string.Empty;

            try
            {
                // Get the centroid of the duct bank
                double x1 = (ductBank.Shape as IArea).Centroid.X;
                double y1 = (ductBank.Shape as IArea).Centroid.Y;

                // Get the centroid of the conduit
                IEnvelope env = (conduit as IFeature).Shape.Envelope;
                double x2 = ((env.XMax - env.XMin) / 2) + env.XMin;
                double y2 = ((env.YMax - env.YMin) / 2) + env.YMin;

                // Calculate the angle
                double xDiff = x2 - x1;
                double yDiff = y2 - y1;
                double angle = Math.Atan2(yDiff, xDiff) * 180.0 / Math.PI;

                // Determine the direction based on the angle
                direction = GetDirection(angle);
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to calculate angle of conduit relative to its duct bank: " + ex.ToString());
                throw ex;
            }

            // Return the result
            return direction;
        }

        /// <summary>
        /// Converts an angle in degrees to a compass direction
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        private string GetDirection(double angle)
        {
            string direction = string.Empty;

            // Approximate the direction based on the angle
            if (angle >= 22.5 && angle < 67.5)
            {
                direction = "NE";
            }
            else if (angle >= 67.5 && angle < 112.5)
            {
                direction = "N";
            }
            else if (angle >= 112.5 && angle < 157.5)
            {
                direction = "NW";
            }
            else if (angle >= -22.5 && angle < 22.5)
            {
                direction = "E";
            }
            else if (angle >= -67.5 && angle < -22.5)
            {
                direction = "SE";
            }
            else if (angle >= -112.5 && angle < -67.5)
            {
                direction = "S";
            }
            else if (angle >= -157.5 && angle < -112.5)
            {
                direction = "SW";
            }
            else if (angle < -157.5 || angle >= 157.5)
            {
                direction = "W";
            }

            // If the duct 
            return direction;
        }

        #endregion

        #endregion
    }
}
