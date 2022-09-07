using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using Miner.ComCategories;
using PGE.Desktop.EDER.AutoUpdaters.Substation.BaseClasses;
using PGE.Desktop.EDER.Utility;
using PGE.Common.Delivery.Diagnostics;

namespace PGE.Desktop.EDER.AutoUpdaters.Substation
{
    [Guid("C2D20B0C-5AE4-4336-9935-68246A3B401B")]
    [ProgId("PGE.Substation.SubVoltageRegulatorLabelAU")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    public class SubVoltageRegulatorLabelAU : SubstationLabelTextSpecialAU
    {
        #region Field Constants
        public const string FLD_BANK_NUMBEROFUNITS = "NUMBEROFUNITS";
        public const string FLD_BANK_NOMINALVOLTAGE = "NOMINALVOLTAGE";
        public const string FLD_BANK_TEMPERATURERISE = "TEMPERATURERISE";
        public const string FLD_UNIT_KVA = "RATEDKVA";
        public const string FLD_UNIT_BOOSTPERCENT = "BOOSTPERCENT";
        public const string FLD_UNIT_BUCKPERCENT = "BUCKPERCENT";
        #endregion

        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        public SubVoltageRegulatorLabelAU() : base() { }

        /// <param name="forceStore"><c>true</c> if the object should be stored (in the event of a manual execution)</param>
        public SubVoltageRegulatorLabelAU(bool forceStore) : base(forceStore) { }

        public override string AutoUpdaterName
        {
            get { return "PGE SubVoltageRegulator LabelText AU"; }
        }

        public override string UniqueFeatureModelName
        {
            get { return ModelNames.VOLTAGEREGULATOR; }
        }

        public override AnnoHorizontalAlignment AnnoAlignment
        {
            get { return AnnoHorizontalAlignment.Center; }
        }

        public override string GenerateLabelText(IObject featureObject)
        {
            // Get all related units.
            List<IObject> transformerUnits = RelationshipManager.GetRelated(featureObject, esriRelRole.esriRelRoleOrigin, ModelNames.UNIT, SubstationRelationshipLabelAU.DeletedRelatedObject);

            _logger.Debug("Found " + transformerUnits.Count + " units.");

            StringBuilder labelText = new StringBuilder();

            // Bank attributes
            string numberOfUnits = FieldManager.GetValue(featureObject, FLD_BANK_NUMBEROFUNITS).ToString().Trim();
            string nominalVoltage = FieldManager.GetValue(featureObject, FLD_BANK_NOMINALVOLTAGE).ToString().Trim();
            string temperatureRise = FieldManager.GetValue(featureObject, FLD_BANK_TEMPERATURERISE).ToString().Trim();

            // Unit attributes (assume only one distinct unit value for each)
            string kva = string.Empty, boostPct = string.Empty, buckPct = string.Empty;
            IObject unit = transformerUnits.DefaultIfEmpty(null).First();
            if (unit != null)
            {
                kva = FieldManager.GetValue(unit, FLD_UNIT_KVA).ToString().Trim();
                boostPct = FieldManager.GetValue(unit, FLD_UNIT_BOOSTPERCENT).ToString().Trim();
                buckPct = FieldManager.GetValue(unit, FLD_UNIT_BUCKPERCENT).ToString().Trim();
            }

            // Start writing the string.
            bool hasUnits = !string.IsNullOrEmpty(numberOfUnits);
            bool hasNominalVoltage = !string.IsNullOrEmpty(nominalVoltage);
            bool hasBoostPct = !string.IsNullOrEmpty(boostPct) && boostPct != "UNK";  // can be Unknown ("UNK")
            bool hasBuckPct = !string.IsNullOrEmpty(buckPct) && buckPct != "UNK";    // can be Unknown ("UNK")
            bool hasKva = !string.IsNullOrEmpty(kva);
            bool hasTempRise = !string.IsNullOrEmpty(temperatureRise);
            bool pctEqual = boostPct == buckPct;
            if (hasUnits)
            {
                labelText.Append(BuildPhasePrefix(featureObject));

                if (hasNominalVoltage || hasBuckPct || hasBoostPct)
                    labelText.Append(", ");
            }
            if (hasNominalVoltage)
            {
                labelText.Append(nominalVoltage + " KV");

                if (hasBuckPct || hasBoostPct)
                    labelText.Append(" +/-");
            }
            if (hasBoostPct | hasBuckPct)
            {
                if (labelText.Length > 0) labelText.Append(" ");
                if (hasBoostPct) labelText.Append(boostPct + "%" + (pctEqual ? "" : " Boost"));
                if (hasBoostPct & hasBuckPct) labelText.Append(" ");
                if (hasBuckPct && !pctEqual) labelText.Append(buckPct + "% Buck");
            }
            if (labelText.Length > 0)
                labelText.AppendLine();

            //Line 2
            if (hasKva)
            {
                labelText.Append(kva + " KVA");

                if (hasTempRise)
                    labelText.Append(" @ ");
            }
            if (hasTempRise)
            {
                labelText.Append(temperatureRise + " C");
            }

            return labelText.ToString();
        }

        public override bool AllFieldsFound(IObjectClass featureObjectClass)
        {
            // Find all fields on the bank.
            if (FieldManager.GetIndex(featureObjectClass, FLD_BANK_NUMBEROFUNITS) == -1) return false;
            if (FieldManager.GetIndex(featureObjectClass, FLD_BANK_NOMINALVOLTAGE) == -1) return false;
            if (FieldManager.GetIndex(featureObjectClass, FLD_BANK_TEMPERATURERISE) == -1) return false;
            // Find all fields on the unit.
            foreach (IRelationshipClass relClass in RelationshipManager.GetRelCache(featureObjectClass, esriRelRole.esriRelRoleOrigin, ModelNames.UNIT))
            {
                if (FieldManager.GetIndex(relClass.DestinationClass, FLD_UNIT_KVA) == -1) return false;
                if (FieldManager.GetIndex(relClass.DestinationClass, FLD_UNIT_BOOSTPERCENT) == -1) return false;
                if (FieldManager.GetIndex(relClass.DestinationClass, FLD_UNIT_BUCKPERCENT) == -1) return false;
            }

            return true;
        }
    }
}
