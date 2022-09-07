using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;

using PGE.Common.Delivery.Diagnostics;
using PGE.Common.Delivery.Framework;
using PGE.Common.Delivery.Framework.FeederManager;
using PGE.Desktop.EDER.AutoUpdaters.LabelText;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace PGE.Desktop.EDER.UFM
{
    public static class XSectionConductorHelper
    {
        #region Constants

        private const string SPACE = " ";

        #endregion

        #region Member vars

        // Replace with Log4NetLogger
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        #endregion

        #region Private methods

        public static string CalculateXSectionConductorText(IObject pObject, string currentLabeltext = "")
        {
            // Get the labeltext value
            string conductorText = string.Empty;

            // X-Section Conductor Anno = 
            // <Conductor Type> <Feeder> <Labeltext>
            // eg: P X-1101 3-1/0 12KV
            //     = ====== ==========

            // Get each constituent part
            conductorText = GetConductorType(pObject) + SPACE;
            conductorText += GetFeeder(pObject);
            conductorText += GetLabelText(pObject, currentLabeltext);
            conductorText += GetIdle(pObject);

            // Return the result
            return conductorText;
        }

        #region Get Conductor Type

        /// <summary>
        /// Returns the conductor type x-section annotation string based on the supplied feature
        /// </summary>
        /// <param name="pObject"></param>
        /// <returns></returns>
        public static string GetConductorType(IObject pObject)
        {
            const string ConductorTypeDc = "DC";
            const string ConductorTypeNeutral = "N";
            const string ConductorTypePrimary = "P";
            const string ConductorTypeSecondary = "S";

            // log entry
            _logger.Debug("Getting conductor type for: " + pObject.Class.AliasName);

            // Default return value
            string value = string.Empty;

            try
            {
                // Return P for primary, S for secondary and DC for, er, DC
                if (ModelNameFacade.ContainsClassModelName(pObject.Class, SchemaInfo.UFM.ClassModelNames.PrimaryConductor))
                {
                    value = ConductorTypePrimary;
                }
                else if (ModelNameFacade.ContainsClassModelName(pObject.Class, SchemaInfo.UFM.ClassModelNames.SecondaryConductor))
                {
                    value = ConductorTypeSecondary;
                }
                else if (ModelNameFacade.ContainsClassModelName(pObject.Class, SchemaInfo.UFM.ClassModelNames.DcConductor))
                {
                    value = ConductorTypeDc;
                }
                else if (ModelNameFacade.ContainsClassModelName(pObject.Class, SchemaInfo.Electric.ClassModelNames.PGENeutralConductor))
                {
                    value = ConductorTypeNeutral;
                }
                else if (ModelNameFacade.ContainsClassModelName(pObject.Class, SchemaInfo.Electric.ClassModelNames.PGEDeactivatedElectricLineSegment))
                {
                    // Conductor type is based on the type of related features
                    value = GetDeactivatedConductorType(pObject);
                }
            }
            catch (Exception ex)
            {
                _logger.Warn("Could not determine conductor type: " + ex.ToString());
            }

            return value;
        }

        private static string GetDeactivatedConductorType(IObject obj)
        {
            const string ConductorTypePrimary = "P";
            const string ConductorTypeSecondary = "S";

            // log entry
            _logger.Debug("Getting deactivated conductor type for: " + obj.Class.AliasName);

            // Default return value is nada
            string deactivatedConductorType = string.Empty;

            // If there are related primary conductor info records, its a P
            ISet primaryInfo = UfmHelper.GetRelatedObjects(obj as IRow, SchemaInfo.Electric.ClassModelNames.PrimaryUGConductorInfo);
            if (primaryInfo.Count > 0)
            {
                if (primaryInfo != null)
                {
                    deactivatedConductorType = ConductorTypePrimary;
                }
            }

            // If still empty, check for secondary conductor info records
            if (deactivatedConductorType == string.Empty)
            {
                ISet secondaryInfo = UfmHelper.GetRelatedObjects(obj as IRow, SchemaInfo.Electric.ClassModelNames.SecondaryUGConductorInfo);
                if (secondaryInfo != null)
                {
                    if (secondaryInfo.Count > 0)
                    {
                        // If found, its an S
                        deactivatedConductorType = ConductorTypeSecondary;
                    }
                }
            }

            // Return the result
            return deactivatedConductorType;
        }

        #endregion

        #region Get Feeder Label

        /// <summary>
        /// Return the feederID stored on the supplied feature
        /// </summary>
        /// <param name="pObject"></param>
        /// <returns></returns>
        private static string GetFeeder(IObject pObject)
        {
            // Log entry 
            _logger.Debug("Getting feeder for: " + pObject.OID);

            // Assume nothing
            string subDesc = string.Empty;

            try
            {
                if (ModelNameFacade.ModelNameManager.ContainsClassModelName(pObject.Class, SchemaInfo.UFM.ClassModelNames.PrimaryConductor) == true)
                {
                    // Get the full circuit                   
                    string[] feeders = FeederManager2.GetCircuitIDs(pObject as IRow);
                    string fullCircuit = string.Empty;
                    if (feeders.Length > 0)
                    {
                        fullCircuit = feeders[0];

                        // Get the substation description and append a space if we got something
                        subDesc = GetSubstationDescription(fullCircuit, (pObject.Class as IDataset).Workspace);
                        if (subDesc != string.Empty)
                        {
                            subDesc += SPACE;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log a warning and return a blank string
                _logger.Warn(ex.Message);
                subDesc = string.Empty;
            }

            return subDesc;
        }

        /// <summary>
        /// Get the substation description for the supplied feature and circuit
        /// </summary>
        /// <param name="fullCircuitName"></param>
        /// <param name="ws"></param>
        /// <returns></returns>
        private static string GetSubstationDescription(string fullCircuitName, IWorkspace ws)
        {
            const string DEFAULT_SUB_DESC = "";

            // Log entry 
            _logger.Debug("Getting substation description for circuit: " + fullCircuitName);

            // Defaults
            string subDesc = string.Empty;
            string subReturnDesc = DEFAULT_SUB_DESC;
            string subId = string.Empty;

            ICursor circuits = null;

            try
            {
                // Get the table with the data we need
                IObjectClass oc = ModelNameFacade.ObjectClassByModelName(ws, SchemaInfo.Electric.ClassModelNames.CircuitSource);

                // Filter based on the supplied circuit ID 
                IQueryFilter qf = new QueryFilterClass();
                qf.WhereClause = "CIRCUITID='" + fullCircuitName + "'";
                circuits = (oc as ITable).Search(qf, false);

                // If a record was returned, pull out the circuit name
                if (circuits != null)
                {
                    IRow circuit = circuits.NextRow();
                    if (circuit != null)
                    {
                        subReturnDesc = GetFieldValue(circuit as IObject, SchemaInfo.UFM.FieldModelNames.CircuitName2);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log a warning and return a blank string
                _logger.Warn(ex.Message);
                subReturnDesc = DEFAULT_SUB_DESC;
            }
            finally
            {
                // Clean up the cursor
                if (circuits != null)
                {
                    Marshal.ReleaseComObject(circuits);
                }
            }

            // Return the result
            return subReturnDesc;
        }

        #endregion

        #region Get Label Text

        /// <summary>
        /// Returns the labeltext, after some post-processing, for the supplied object
        /// </summary>
        /// <param name="pObject"></param>
        /// <returns></returns>
        private static string GetLabelText(IObject pObject, string currentLabeltext)
        {
            // Log entry
            _logger.Debug("Getting labeltext value");

            string labeltext = string.Empty;

            try
            {
                // If deactivate or neutral then we need a special function
                if (ModelNameFacade.ContainsClassModelName(pObject.Class,
                    SchemaInfo.Electric.ClassModelNames.PGEDeactivatedElectricLineSegment))
                {
                    labeltext = GetDeactivatedLabelText(pObject);
                }
                    //else if (ModelNameFacade.ContainsClassModelName(pObject.Class, SchemaInfo.Electric.ClassModelNames.PGENeutralConductor) == true)
                    //{
                    //    labeltext = GetNeutralLabelText(pObject);
                    //}
                else
                {
                    // Otherwise, just get the labeltext field value

                    // Get the raw value
                    labeltext = currentLabeltext == string.Empty ? GetFieldValue(pObject, SchemaInfo.UFM.FieldModelNames.LabelText) : currentLabeltext;
                }

                // Strip off any 'DC' values
                labeltext = labeltext.Replace("DC", string.Empty);

                // Reduce conductor type (check for extra spaces)
                labeltext = labeltext.Replace("  TPX", "T");
                labeltext = labeltext.Replace(" TPX", "T");
                labeltext = labeltext.Replace("TPX", "T");
                labeltext = labeltext.Replace("  QPX", "Q");
                labeltext = labeltext.Replace(" QPX", "Q");
                labeltext = labeltext.Replace("QPX", "Q");

                // Eliminate joint trench
                labeltext = labeltext.Replace(" JT", string.Empty);

                // Eliminate voltage
                Regex expr = new Regex(" [0-9]+kV ");
                foreach (Match matchItem in expr.Matches(labeltext))
                {
                    labeltext = labeltext.Replace(matchItem.Value, " ");
                }
                expr = new Regex(" [0-9]+kV");
                foreach (Match matchItem in expr.Matches(labeltext))
                {
                    labeltext = labeltext.Replace(matchItem.Value, "");
                }
                expr = new Regex(" [0-9].[0-9]+kV ");
                foreach (Match matchItem in expr.Matches(labeltext))
                {
                    labeltext = labeltext.Replace(matchItem.Value, "");
                }
                expr = new Regex(" [0-9].[0-9]+kV");
                foreach (Match matchItem in expr.Matches(labeltext))
                {
                    labeltext = labeltext.Replace(matchItem.Value, "");
                }
                expr = new Regex(" [0-9]+.[0-9]+kV ");
                foreach (Match matchItem in expr.Matches(labeltext))
                {
                    labeltext = labeltext.Replace(matchItem.Value, "");
                }
                expr = new Regex(" [0-9]+.[0-9]+kV");
                foreach (Match matchItem in expr.Matches(labeltext))
                {
                    labeltext = labeltext.Replace(matchItem.Value, "");
                }

                // Eliminate single conductor counts
                labeltext = labeltext.Replace(" 1-", " ");
                labeltext = labeltext.Replace("1-", string.Empty);
                labeltext = labeltext.Trim();
            }
            catch (Exception ex)
            {
                _logger.Warn("Failed to get LabelText value" + ex.ToString());
                labeltext = string.Empty;
            }

            // Return the result
            return labeltext;
        }

        private static string GetNeutralLabelText(IObject pObject)
        {
            string neutralLabel = string.Empty;

            // Rebuilt logic from annotation class
            // Currently: [CONDUCTORCOUNT]+"-"+ [CONDUCTORSIZE] + [MATERIAL]
            //string label = Labelte
            string count = GetFieldValue(pObject, SchemaInfo.Electric.FieldModelNames.CondutorCount);
            string size = GetFieldValue(pObject, SchemaInfo.Electric.FieldModelNames.ConductorSize);
            string material = GetFieldValue(pObject, SchemaInfo.Electric.FieldModelNames.ConductorMaterial);
            neutralLabel += count + "-" + size + material;

            // Return the result
            return neutralLabel;
        }

        private static string GetDeactivatedLabelText(IObject obj)
        {
            const string DEACTIVATED = "DEACT ";
            string deactivatedLabelText = string.Empty;

            try
            {
                // If there are related primary conductor info records, query the primary label AU for its text
                ISet primaryInfo = UfmHelper.GetRelatedObjects(obj as IRow, SchemaInfo.Electric.ClassModelNames.PrimaryUGConductorInfo);
                if (primaryInfo != null)
                {
                    if (primaryInfo.Count > 0)
                    {
                        // Calculate the labeltext value by siphoning the value from the labeltext AU
                        string labelText;
                        string labelText2;
                        PriUGConductorLabel primaryLabelAU = new PriUGConductorLabel();
                        primaryLabelAU.BuildLabelTexts(obj, out labelText, out labelText2);
                        deactivatedLabelText = labelText;
                    }
                }

                // If nothing got set...
                if (deactivatedLabelText == string.Empty)
                {
                    // Check secondary instead
                    ISet secondaryInfo = UfmHelper.GetRelatedObjects(obj as IRow, SchemaInfo.Electric.ClassModelNames.SecondaryUGConductorInfo);
                    if (secondaryInfo.Count > 0)
                    {
                        // And use the secondary UG label AU to get the text
                        SecUGConductorLabel secondaryLabelAU = new SecUGConductorLabel();
                        deactivatedLabelText = secondaryLabelAU.BuildLabelText(obj);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log a warning, use the default empty value and move on
                _logger.Warn("Error calculating cross section conductor anno for (" + obj.Class.AliasName + ":" + obj.OID.ToString() + "): " + ex.ToString());
            }

            // Return the result prepended with the deactivated text
            return DEACTIVATED + deactivatedLabelText;
        }

        #endregion

        #region Get status

        private static string GetIdle(IObject pObject)
        {
            const string STATUS_IDLE = "30";
            string status = string.Empty;

            if (ModelNameFacade.ContainsClassModelName(pObject.Class, SchemaInfo.UFM.ClassModelNames.PrimaryConductor) ||
               ModelNameFacade.ContainsClassModelName(pObject.Class, SchemaInfo.UFM.ClassModelNames.SecondaryConductor) ||
               ModelNameFacade.ContainsClassModelName(pObject.Class, SchemaInfo.UFM.ClassModelNames.DcConductor))
            {                
                string val = GetFieldValue(pObject, SchemaInfo.Electric.FieldModelNames.Status);
                if (val == STATUS_IDLE)
                {
                    status = SPACE + "IDLE";
                }
            }

            return status;
        }

        #endregion

        #region Common

        /// <summary>
        /// Get the field value of the supplied feature and field
        /// </summary>
        /// <param name="pObject"></param>
        /// <param name="modelName"></param>
        /// <returns></returns>
        private static string GetFieldValue(IObject pObject, string fieldModelName)
        {
            // log entry
            _logger.Debug("Getting field value");
            _logger.Debug("Field model name: " + fieldModelName);

            // Default return value
            string value = string.Empty;

            // Get the field index from the model name
            int index = ModelNameFacade.FieldIndexFromModelName(pObject.Class, fieldModelName);

            // If we found it...
            if (index != -1)
            {
                // Get the value
                object objValue = pObject.get_Value(index);
                if (objValue != null)
                {
                    value = objValue.ToString();
                }
            }

            // Return the result
            return value;
        }

        #endregion

        #endregion
    }
}
