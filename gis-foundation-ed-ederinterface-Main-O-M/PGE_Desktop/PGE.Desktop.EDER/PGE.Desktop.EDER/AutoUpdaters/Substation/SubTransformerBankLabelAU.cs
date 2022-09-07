using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using Miner.ComCategories;
using Miner.Interop;
using PGE.Desktop.EDER.AutoUpdaters.Substation.BaseClasses;
using PGE.Common.Delivery.Diagnostics;
using PGE.Desktop.EDER.Utility;

namespace PGE.Desktop.EDER.AutoUpdaters.Substation
{
    [Guid("03F32DAD-07F5-4645-A7DF-66E49B2E6EE1")]
    [ProgId("PGE.Substation.SubTransformerBankLabelAU")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    public class SubTransformerBankLabelAU : SubstationLabelTextSpecialAU
    {
        #region Field Constants
        public const string FLD_BANK_NUMBEROFUNITS = "NUMBEROFUNITS";
        public const string FLD_BANK_VOLTAGEHIGHSIDE = "HIGHSIDEVOLTAGE";
        public const string FLD_BANK_VOLTAGELOWSIDE_LN = "LOWSIDELNVOLTAGE";
        public const string FLD_BANK_VOLTAGELOWSIDE_LL = "LOWSIDELLVOLTAGE";
        public const string FLD_BANK_VOLTAGETERTIARY = "TERTIARYVOLTAGE";
        public const string FLD_BANK_TERTIARYMVAPERCENT = "TERTIARYMVAPERCENT";
        public const string FLD_RATING_MVARATING = "MVARATING";
        public const string FLD_RATING_CLASS = "RATINGCLASS";
        public const string FLD_RATING_TEMPERATURERISE = "TEMPERATURERISE";
        public const string FLD_LTC_BOOSTPERCENT = "BOOSTPERCENT";
        public const string FLD_LTC_BUCKPERCENT = "BUCKPERCENT";
        #endregion

        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        public SubTransformerBankLabelAU() : base() { }

        /// <param name="forceStore"><c>true</c> if the object should be stored (in the event of a manual execution)</param>
        public SubTransformerBankLabelAU(bool forceStore) : base(forceStore) { }

        public override string AutoUpdaterName
        {
            get { return "PGE SubTransformerBank LabelText AU"; }
        }

        public override string UniqueFeatureModelName
        {
            get { return ModelNames.SUBTRANSFORMERBANK; }
        }

        public override AnnoHorizontalAlignment AnnoAlignment
        {
            get { return AnnoHorizontalAlignment.Center; }
        }

        public override string GenerateLabelText(IObject featureObject)
        {
            // Get all related units.
            List<IObject> transformerUnits = RelationshipManager.GetRelated(featureObject, esriRelRole.esriRelRoleOrigin, ModelNames.UNIT, SubstationRelationshipLabelAU.DeletedRelatedObject);

            // Get the ratings for each unit.
            Dictionary<int, IObject> transformerRatings = new Dictionary<int, IObject>();
            transformerUnits.ForEach(u =>
            {
                IEnumerable<IObject> ratingList = RelationshipManager.GetRelated(u, esriRelRole.esriRelRoleOrigin, ModelNames.SUBRATING,
                    SubstationRelationshipLabelAU.DeletedRelatedObject).DefaultIfEmpty(null);
                
                foreach (IObject rating in ratingList)
                {
                    if (rating != null)
                    {
                        //start- INC000003955512- Adding line of code as one(same) transformer rating table is present in two different transformer units
                        if (!transformerRatings.ContainsKey(rating.OID))
                            //end
                        {
                            transformerRatings.Add(rating.OID, rating);
                        }
                    }
                }
            });

            // Get all related LoadTapChangers.
            List<IObject> transformerLTCs = RelationshipManager.GetRelated(featureObject, esriRelRole.esriRelRoleOrigin, ModelNames.SUBLOADTAPCHANGER, SubstationRelationshipLabelAU.DeletedRelatedObject);

            _logger.Debug("Found " + transformerUnits.Count + " units, " + transformerRatings.Count + " ratings, and " + transformerLTCs.Count + " load tap changers.");

            StringBuilder labelText = new StringBuilder();

            // Bank attributes
            string numberOfUnits = FieldManager.GetValue(featureObject, FLD_BANK_NUMBEROFUNITS).ToString().Trim();
            string voltageHighSide = FieldManager.GetValue(featureObject, FLD_BANK_VOLTAGEHIGHSIDE).ToString().Trim();
            string voltageLowSideLN = FieldManager.GetValue(featureObject, FLD_BANK_VOLTAGELOWSIDE_LN).ToString().Trim();
            string voltageLowSideLL = FieldManager.GetValue(featureObject, FLD_BANK_VOLTAGELOWSIDE_LL).ToString().Trim();
            string voltageTertiary = FieldManager.GetValue(featureObject, FLD_BANK_VOLTAGETERTIARY).ToString().Trim();
            string tertiaryMVA = FieldManager.GetValue(featureObject, FLD_BANK_TERTIARYMVAPERCENT).ToString().Trim();

            // LoadTapChanger attributes (assume only one distinct LTC value for each)
            string boostPct = string.Empty, buckPct = string.Empty;
            IObject ltc = transformerLTCs.DefaultIfEmpty(null).First();
            if (ltc != null)
            {
                boostPct = FieldManager.GetValue(ltc, FLD_LTC_BOOSTPERCENT).ToString().Trim();
                buckPct = FieldManager.GetValue(ltc, FLD_LTC_BUCKPERCENT).ToString().Trim();
            }

            // Start writing the string.
            if (!string.IsNullOrEmpty(numberOfUnits))
            {
                labelText.AppendLine(BuildPhasePrefix(featureObject));
            }
            
            string unitsMVARating = string.Empty, unitsRatingClass = string.Empty;
            
            //Statement that first groups all ratings by their MVARating value, orders by the MVARating value, gets the last of those 
            //MVARating groups (the largest group), orders by the temperature rise of those MVARating records, and finally gets the last of those, which is also
            //the record with the largest MVARating and the largest TemperatureRise.
            IObject highestRatingWithHighestTempRise = transformerRatings.Values.GroupBy(rating => FieldManager.GetValue(rating, FLD_RATING_MVARATING)
                .ToString()).OrderBy(group => { double n = -1; Double.TryParse(group.Key, out n); return n; }).Last().OrderBy(
                rating => { double n = -1; Double.TryParse(FieldManager.GetValue(rating, FLD_RATING_TEMPERATURERISE).ToString().Trim(), out n); return n; }).Last();

            string rMVA = FieldManager.GetValue(highestRatingWithHighestTempRise, FLD_RATING_MVARATING).ToString().Trim();
            string rClass = FieldManager.GetValue(highestRatingWithHighestTempRise, FLD_RATING_CLASS).ToString().Trim();
            string rise = FieldManager.GetValue(highestRatingWithHighestTempRise, FLD_RATING_TEMPERATURERISE).ToString().Trim();

            if (!string.IsNullOrEmpty(rMVA))
            {
                labelText.Append(rMVA + " MVA");
                if (!string.IsNullOrEmpty(rClass))
                {
                    labelText.Append(" " + rClass);
                }
                if (!string.IsNullOrEmpty(rise))
                {
                    labelText.Append(" " + rise + "C RISE");
                }
                labelText.AppendLine();
            }

            // Writing Bank attributes
            if (!string.IsNullOrEmpty(voltageHighSide))
                labelText.AppendLine("HV: " + voltageHighSide + " KV");

            bool lowLN = !string.IsNullOrEmpty(voltageLowSideLN);
            bool lowLL = !string.IsNullOrEmpty(voltageLowSideLL);
            if (lowLN | lowLL)
            {
                labelText.Append("LV: ");
                if (lowLN) labelText.Append(voltageLowSideLN + " KV");
                if (lowLN & lowLL) labelText.Append("/");
                if (lowLL) labelText.Append(voltageLowSideLL + " KV");
                labelText.AppendLine();
            }

            bool tVolt = !string.IsNullOrEmpty(voltageTertiary);
            bool tMVA = !string.IsNullOrEmpty(tertiaryMVA);
            if (tVolt | tMVA)
            {
                labelText.Append("TV: ");
                if (tVolt) labelText.Append(voltageTertiary + " KV");
                if (tVolt & tMVA) labelText.Append(" @ ");
                if (tMVA) labelText.Append(tertiaryMVA + "%");
                labelText.AppendLine();
            }

            // Writing LTC Attribtues - these can be Unknown ("UNK") so we're filtering that out as well.
            bool ltcBoost = !string.IsNullOrEmpty(boostPct) && boostPct != "UNK";
            bool ltcBuck = !string.IsNullOrEmpty(buckPct) && buckPct != "UNK";
            bool ltcEqual = boostPct == buckPct;
            if (ltcBoost | ltcBuck)
            {
                labelText.Append("LTC: ");
                if (ltcBoost) labelText.Append(boostPct + "%" + (ltcEqual ? "" : " Boost"));
                if (ltcBoost & ltcBuck) labelText.Append(" ");
                if (ltcBuck && !ltcEqual) labelText.Append(buckPct + "% Buck");
                labelText.AppendLine();
            }

            //Remove the last line break from the string.
            int removeLen = Environment.NewLine.Length;
            if (labelText.Length > removeLen)
                labelText.Remove(labelText.Length - removeLen, removeLen);

            return labelText.ToString();
        }

        public override bool AllFieldsFound(IObjectClass featureObjectClass)
        {
            // Find all fields on the bank.
            if (FieldManager.GetIndex(featureObjectClass, FLD_BANK_NUMBEROFUNITS) == -1) return false;
            if (FieldManager.GetIndex(featureObjectClass, FLD_BANK_VOLTAGEHIGHSIDE) == -1) return false;
            if (FieldManager.GetIndex(featureObjectClass, FLD_BANK_VOLTAGELOWSIDE_LN) == -1) return false;
            if (FieldManager.GetIndex(featureObjectClass, FLD_BANK_VOLTAGELOWSIDE_LL) == -1) return false;
            if (FieldManager.GetIndex(featureObjectClass, FLD_BANK_VOLTAGETERTIARY) == -1) return false;
            if (FieldManager.GetIndex(featureObjectClass, FLD_BANK_TERTIARYMVAPERCENT) == -1) return false;
            // Find all fields on the LoadTapChanger.
            foreach (IRelationshipClass relClass in RelationshipManager.GetRelCache(featureObjectClass, esriRelRole.esriRelRoleOrigin, ModelNames.SUBLOADTAPCHANGER))
            {
                if (FieldManager.GetIndex(relClass.DestinationClass, FLD_LTC_BOOSTPERCENT) == -1) return false;
                if (FieldManager.GetIndex(relClass.DestinationClass, FLD_LTC_BUCKPERCENT) == -1) return false;
            }
            // Find all fields on the rating.
            foreach (IRelationshipClass relClass in RelationshipManager.GetRelCache(featureObjectClass, esriRelRole.esriRelRoleOrigin, ModelNames.UNIT))
            {
                foreach (IRelationshipClass ratingRelClass in RelationshipManager.GetRelCache(relClass.DestinationClass, esriRelRole.esriRelRoleOrigin, ModelNames.SUBRATING))
                {
                    if (FieldManager.GetIndex(ratingRelClass.DestinationClass, FLD_RATING_MVARATING) == -1) return false;
                    if (FieldManager.GetIndex(ratingRelClass.DestinationClass, FLD_RATING_CLASS) == -1) return false;
                    if (FieldManager.GetIndex(ratingRelClass.DestinationClass, FLD_RATING_TEMPERATURERISE) == -1) return false;
                }
            }

            return true;
        }
    }
}
