using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;
using Miner.ComCategories;
using ESRI.ArcGIS.Geodatabase;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Framework;
using PGE.Common.Delivery.Geodatabase;
using PGE.Common.Delivery.Diagnostics;
using log4net;

namespace PGE.Desktop.EDER.ValidationRules
{
    /// <summary>
    /// Validates the Phase Designation on InService features.
    /// </summary>
    [ProgId("PGE.Desktop.EDER.ValidationRules.ValidatePhaseDesignation")]
    [Guid("C76B6A80-124D-46B7-9B56-CC6A273BC327")]
    [ComVisible(true)]
    [Miner.ComCategories.ComponentCategory(ComCategory.MMValidationRules)]
    public class ValidatePhaseDesignation:BaseValidationRule
    {
        #region Private Variables
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        // Q1 -2021 QA/QC phase rule change for ED-GIS Scripting project make this available
        // for all Phase Designation features, but it will not be assigned to secondary until phase 2.
        private static string[] _modelNames = { SchemaInfo.Electric.FieldModelNames.PhaseDesignation };
        // private static string[] _modelNames = { SchemaInfo.Electric.FieldModelNames.PhaseDesignation, SchemaInfo.Electric.ClassModelNames.PGEConductor, SchemaInfo.Electric.ClassModelNames.PGESecondaryLoadPoint, SchemaInfo.Electric.ClassModelNames.PGEPhaseValidationForUnitTable };

        #endregion Private Variables

        #region Constructor
        /// <summary>
        /// Initializes the instance of <see cref="ValidatePhaseDesignation"/>.
        /// </summary>
        public ValidatePhaseDesignation()
            : base("PGE Validate Phase Designation", _modelNames)
        { }
        #endregion Constructor

        #region Overridden Methods
        /// <summary>
        /// Validates the feature for phase designation.
        /// </summary>
        /// <param name="row">Instance of the feature to be validated.</param>
        /// <returns>Returns the list of errors of the PhaseDesignation either null or empty.</returns>
        protected override Miner.Interop.ID8List InternalIsValid(ESRI.ArcGIS.Geodatabase.IRow row)
        {
            try
            {
                // Do not validate any Proposed or Idle features.
                if (StatusHelper.IsInService(row as IObject))
                {
                    // Refactored code so that we could split the rule into errors for inservice and warnings for proposed.
                    List<string> errors = PhaseValidator.Validate(row);
                    _logger.Debug(string.Format("Validation complete received {0} errors.", errors.Count));
                    foreach (string error in errors)
                    {
                        _logger.Debug(error);
                        AddError(error);
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred while validating Phase Designation rule.", ex);
            }
            return base._ErrorList;
        }

        #region Old Code

        // Q1 -2021 QA/QC phase rule change for ED-GIS Scripting project.
        // Old code before change.
        // protected override Miner.Interop.ID8List InternalIsValid(ESRI.ArcGIS.Geodatabase.IRow row)
        // {

        //    IFeature feature = null;
        //    if (row is IFeature)
        //    {
        //        feature = row as IFeature;
        //    }

        //    IObjectClass rowObjectClass = row.Table as IObjectClass;
        //    IDomain domain = GDBFacade.FindByName((rowObjectClass as IDataset).Workspace, SchemaInfo.General.PhaseDesignationDomainName);
        //    ICodedValueDomain codedValueDomain = domain as ICodedValueDomain;

        //    int phaseDesignationFldIx = ModelNameFacade.FieldIndexFromModelName(row.Table as IObjectClass, SchemaInfo.Electric.FieldModelNames.PhaseDesignation);
        //    if (phaseDesignationFldIx == -1) return base._ErrorList;
        //    object fieldValue = row.get_Value(phaseDesignationFldIx);

        //    //Validate
        //    if (fieldValue == null || fieldValue == DBNull.Value || fieldValue.ToString() == string.Empty)
        //    {
        //        base.AddError("Phase Designation is missing.");
        //    }
        //    else if (ModelNameFacade.ContainsClassModelName(rowObjectClass, SchemaInfo.Electric.ClassModelNames.PGESecondaryLoadPoint))
        //    {
        //        //Bug#13027

        //        if (feature != null && feature is INetworkFeature && feature.FeatureType == esriFeatureType.esriFTSimpleJunction)
        //        {
        //            ISimpleJunctionFeature junctionFeature = feature as ISimpleJunctionFeature;
        //            IEdgeFeature edgeFeature = junctionFeature.get_EdgeFeature(0);
        //            IObjectClass edgeObjectClass = (edgeFeature as IFeature).Class as IObjectClass;

        //            int edgePhaseDesignationFldIx = ModelNameFacade.FieldIndexFromModelName(edgeObjectClass, SchemaInfo.Electric.FieldModelNames.PhaseDesignation);
        //            if (edgePhaseDesignationFldIx == -1) return base._ErrorList;

        //            object objPhaseDesignation = (edgeFeature as IFeature).get_Value(edgePhaseDesignationFldIx);
        //            string edgePhaseDesignation = objPhaseDesignation.Convert<string>(string.Empty);

        //            char[] junctionPhaseDesignationDesc = GDBFacade.GetDomainDescriptionOrCode(codedValueDomain, fieldValue.ToString(), true).ToArray<char>();
        //            char[] edgePhaseDesignationDesc = GDBFacade.GetDomainDescriptionOrCode(codedValueDomain, edgePhaseDesignation, true).ToArray<char>();

        //            if (edgePhaseDesignationDesc.Length >= junctionPhaseDesignationDesc.Length)
        //            {
        //                foreach (char phase in junctionPhaseDesignationDesc)
        //                {
        //                    if (!edgePhaseDesignationDesc.Contains(phase))
        //                    {
        //                        base.AddError(rowObjectClass.AliasName + "'s (OID: " + feature.OID + ") Phase Designation does not match with " + edgeObjectClass.AliasName + "'s (OID: " + (edgeFeature as IFeature).OID + ") Phase Designation.");
        //                        break;
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                base.AddError(rowObjectClass.AliasName + "'s (OID: " + feature.OID + ") Phase Designation does not match with " + edgeObjectClass.AliasName + "'s (OID: " + (edgeFeature as IFeature).OID + ") Phase Designation.");
        //            }
        //        }
        //    }

        //    //Validate unit table
        //    if (ModelNameFacade.ContainsClassModelName(rowObjectClass, SchemaInfo.Electric.ClassModelNames.PGEPhaseValidationForUnitTable))
        //    {
        //        //Bug#13037
        //        IObject rowObject = row as IObject;

        //        char[] phaseDesignations = null;
        //        try
        //        {
        //            phaseDesignations = GDBFacade.GetDomainDescriptionOrCode(codedValueDomain, fieldValue.ToString(), true).ToCharArray();
        //        }
        //        catch(Exception ex)
        //        {
        //            _logger.Debug("ValidatePhaseDesignation (Validate unit table)... " + ex.Message);
        //        }

        //        byte featurePhaseDesignation = 0;

        //        //Get Phase Designation of feature class in byte
        //        if (phaseDesignations != null)
        //        {
        //            foreach (char pd in phaseDesignations)
        //            {
        //                try
        //                {
        //                    byte value = Convert.ToByte(Enum.Parse(typeof(SchemaInfo.Electric.PhaseDesignationEnum), pd.ToString()));
        //                    //bitwise OR adds each individual phase (A, B or C). If phase value already exists, no change will happen in featurePhaseDesignation.
        //                    featurePhaseDesignation |= value; 
        //                }
        //                catch (Exception ex)
        //                {
        //                    _logger.Debug("ValidatePhaseDesignation (Validate unit table -- Get Phase Designation of feature class in byte -- ToByte)... " + ex.Message);
        //                }
        //            }
        //        }

        //        IEnumerable<IObject> unitObjects = rowObject.GetRelatedObjects(null, esriRelRole.esriRelRoleAny, SchemaInfo.Electric.ClassModelNames.PGEUnitTable);
        //        byte phaseDesignation = 0;

        //        //Get Phase Designation of all units in byte
        //        foreach (IObject unit in unitObjects)
        //        {
        //            char[] unitPhaseDesignations = null;

        //            int unitPhaseDesignationFldIx = ModelNameFacade.FieldIndexFromModelName(unit.Class, SchemaInfo.Electric.FieldModelNames.PhaseDesignation);
        //            if (unitPhaseDesignationFldIx != -1)
        //            {
        //                try
        //                {
        //                    unitPhaseDesignations = GDBFacade.GetDomainDescriptionOrCode(codedValueDomain, (unit.get_Value(unitPhaseDesignationFldIx).Convert<string>(string.Empty)), true).ToCharArray();
        //                    if (unitPhaseDesignations != null)
        //                    {
        //                        foreach (char upd in unitPhaseDesignations)
        //                        {
        //                            try
        //                            {
        //                                byte value = Convert.ToByte(Enum.Parse(typeof(SchemaInfo.Electric.PhaseDesignationEnum), upd.ToString()));
        //                                //bitwise OR adds each individual phase (A, B or C). If phase value already exists, no change will happen in phaseDesignation.
        //                                phaseDesignation |= value; 
        //                            }
        //                            catch (Exception ex)
        //                            {
        //                                _logger.Debug("ValidatePhaseDesignation (Get Phase Designation of all units in byte -- ToByte)... " + ex.Message);
        //                            }
        //                        }
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    _logger.Debug("ValidatePhaseDesignation (Get Phase Designation of all units in byte -- unitPhaseDesignations = GDBFacade.GetDomainDescriptionOrCode)... " + ex.Message);
        //                }
        //            }
        //        }

        //        //Compare the phase designations 
        //        if (featurePhaseDesignation != phaseDesignation)
        //        {
        //            base.AddError(rowObjectClass.AliasName + "'s (OID: " + rowObject.OID + ") Phase Designation does not match with unit's Phase Designation.");
        //        }
        //    }

        //    return base._ErrorList;
        // }
        #endregion Old Code
        #endregion Overridden Methods
    }
}
