using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using Miner.ComCategories;
using Miner.Interop;
using PGE.Common.Delivery.Diagnostics;
using PGE.Desktop.EDER.AutoUpdaters.Substation.BaseClasses;
using PGE.Desktop.EDER.Utility;

namespace PGE.Desktop.EDER.AutoUpdaters.Substation
{
    [Guid("6A2DC9FB-45A5-48EC-96CA-DEB0BFDC0FA4")]
    [ProgId("PGE.Substation.SubstationTransformerLabelAU")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    public class SubstationTransformerLabelTextAU : SubstationLabelTextSpecialAU
    {
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        public SubstationTransformerLabelTextAU() : base() { }

        /// <param name="forceStore"><c>true</c> if the object should be stored (in the event of a manual execution)</param>
        public SubstationTransformerLabelTextAU(bool forceStore) : base(forceStore) { }

        public override string AutoUpdaterName
        {
            get { return "PGE SubstationTransformer LabelText AU"; }
        }

        public override string UniqueFeatureModelName
        {
            get { return ModelNames.SUBSTATIONTRANSFORMER; }
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

            labelText.Append(BuildPhasePrefix(featureObject));

            string kvaLabelPart = BuildKVAString(transformerUnits);

            if (labelText.Length > 0 && !String.IsNullOrEmpty(kvaLabelPart))
            {
                labelText.Append(", ");
            }

            labelText.Append(kvaLabelPart);

            return labelText.ToString();
        }

        private string BuildKVAString(IList<IObject> transformerUnits)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            // This does the following: groups ratedKVA, filters out nulls, gets a count
            var groupedRatedKVACounts = transformerUnits.GroupBy(o => o.get_Value(o.Class.FindField(SchemaInfo.General.Fields.RatedKVA)) == null || o.get_Value(o.Class.FindField(SchemaInfo.General.Fields.RatedKVA)) == DBNull.Value ?
                "" : o.get_Value(o.Class.FindField(SchemaInfo.General.Fields.RatedKVA)).ToString())
                .Where(w => w.Key != "")
                .Select(g => new
                {
                    RATEDKVA = g.Key,
                    COUNT = g.Distinct().Count()
                })
                .OrderBy(g => g.RATEDKVA)
                .ToList();


            // There may be a way to do this in the previous LINQ statement
            string label = "";
            if (groupedRatedKVACounts.Count > 1)
            {
                label = groupedRatedKVACounts
                    .Select(l => l.COUNT.ToString() + "-" + l.RATEDKVA + " KVA")
                    .Concatenate(", ");
            }
            else if (groupedRatedKVACounts.Count == 1)
            {
                label = groupedRatedKVACounts[0].RATEDKVA + " KVA";
            }

            return label;
        }

        public override bool AllFieldsFound(IObjectClass featureObjectClass)
        {
            // Find all fields on the bank.
            if (FieldManager.GetIndex(featureObjectClass, SchemaInfo.General.Fields.NumberOfUnits) == -1) return false;
            if (FieldManager.GetIndex(featureObjectClass, SchemaInfo.General.Fields.PhaseDesignation) == -1) return false;
            // Find all fields on the unit.
            foreach (IRelationshipClass relClass in RelationshipManager.GetRelCache(featureObjectClass, esriRelRole.esriRelRoleOrigin, ModelNames.UNIT))
            {
                if (FieldManager.GetIndex(relClass.DestinationClass, SchemaInfo.General.Fields.RatedKVA) == -1) return false;
            }

            return true;
        }
    
    }
}
