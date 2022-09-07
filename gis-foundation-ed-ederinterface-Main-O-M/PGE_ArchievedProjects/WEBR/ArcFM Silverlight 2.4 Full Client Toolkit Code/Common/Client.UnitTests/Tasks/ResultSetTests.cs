using System;
using System.Collections.Generic;

using ESRI.ArcGIS.Client.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

#if SILVERLIGHT
using Miner.Server.Client.Tasks;
#elif WPF
using Miner.Mobile.Client.Tasks;
#endif

#if SILVERLIGHT
namespace Miner.Server.Client.UnitTests
#elif WPF
namespace Miner.Mobile.Client.UnitTests
#endif
{
    [TestClass]
    public class ResultSetTests
    {
        [TestMethod]
        public void DefaultConstructor_NotNull()
        {
            ResultSet set = new ResultSet();
            Assert.IsNotNull(set);
        }

        [TestMethod]
        public void DefaultConstructor_IdIsNegativeOne()
        {
            ResultSet set = new ResultSet();
            Assert.AreEqual(set.ID, -1);
        }

        [TestMethod]
        public void DefaultConstructor_FeaturesIsEmpty()
        {
            ResultSet set = new ResultSet();
            Assert.AreEqual(set.Features.Count, 0);
        }

        [TestMethod]
        public void DefaultConstructor_RelationshipServiceIsNotNull()
        {
            ResultSet set = new ResultSet();
            Assert.IsNotNull(set.RelationshipService);
            Assert.IsInstanceOfType(set.RelationshipService, typeof(RelationshipService));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FeatureSetConstructor_ParameterIsNull_ThrowsException()
        {
            ResultSet set = new ResultSet(null);
        }

        [TestMethod]
        public void FeatureSetConstructor_ParameterNotNull_IsNotNull()
        {
            ResultSet set = new ResultSet(new FeatureSet());
            Assert.IsNotNull(set);
        }

        [TestMethod]
        public void FromResults_NullJson_NotNullResult()
        {
            List<IResultSet> sets = ResultSet.FromResults(null, null, null);
            Assert.IsNotNull(sets);
        }

        [TestMethod]
        public void FromResults_NullJson_EmptyResult()
        {
            List<IResultSet> sets = ResultSet.FromResults(null, null, null);
            Assert.AreEqual(sets.Count, 0);
        }

        [TestMethod]
        public void FromResults_JsonForOneFeatureWithoutAttributes_OneResultWithOneFeatureWithOneAttribute()
        {
            string json = @"{""results"":[{""displayFieldName"":""FacilityID"",""fieldAliases"":{""OBJECTID"":""Object ID""},""geometryType"":""esriGeometryPoint"",""spatialReference"":{""wkid"":26753},""fields"":[{""name"":""OBJECTID"",""type"":""esriFieldTypeOID"",""alias"":""Object ID""}],""features"":[{""attributes"":{""OBJECTID"":12},""geometry"":{""x"":2214155.5223696935,""y"":399668.75619776896}}],""name"":""Transformer"",""id"":0,""exceededThreshold"":false}]}";
            List<IResultSet> sets = ResultSet.FromResults(json);
            Assert.AreEqual(sets.Count, 1);
            Assert.AreEqual(sets[0].Features.Count, 1);
            // Test for two attributes because we add "RowIndex"
            Assert.AreEqual(sets[0].Features[0].Attributes.Count, 2);
        }

                [TestMethod]
        public void FromResults_JsonForTwoFeaturesWithoutAttributes_OneResultWithTwoFeaturesWithOneAttribute()
        {
            string json = @"{""results"":[{""displayFieldName"":""FacilityID"",""fieldAliases"":{""OBJECTID"":""Object ID""},""geometryType"":""esriGeometryPoint"",""spatialReference"":{""wkid"":26753},""fields"":[{""name"":""OBJECTID"",""type"":""esriFieldTypeOID"",""alias"":""Object ID""}],""features"":[{""attributes"":{""OBJECTID"":70},""geometry"":{""x"":2221444.1893524816,""y"":394108.44483259053}},{""attributes"":{""OBJECTID"":71},""geometry"":{""x"":2221759.9912887756,""y"":393750.16577625682}}],""name"":""Transformer"",""id"":0,""exceededThreshold"":false}]}";
            List<IResultSet> sets = ResultSet.FromResults(json);
            Assert.AreEqual(sets.Count, 1);
            Assert.AreEqual(sets[0].Features.Count, 2);
            // Test for two attributes because we add "RowIndex"
            Assert.AreEqual(sets[0].Features[0].Attributes.Count, 2);
            Assert.AreEqual(sets[0].Features[1].Attributes.Count, 2);
        }

        [TestMethod]
        public void FromResults_JsonForOneFeatureWithAttributes_OneResultWithOneFeatureWithManyAttributes()
        {
            string json = @"{""results"":[{""displayFieldName"":""FacilityID"",""fieldAliases"":{""OBJECTID"":""Object ID"",""ANCILLARYROLE"":""ANCILLARYROLE"",""ENABLED"":""Enabled"",""CREATIONUSER"":""Creation User"",""DATECREATED"":""Date Created"",""DATEMODIFIED"":""Date Modified"",""LASTUSER"":""Last User"",""SUBTYPECD"":""SubtypeCD"",""FACILITYID"":""Facility ID"",""FEEDERID"":""Feeder ID"",""FEEDERID2"":""Feeder ID 2"",""OPERATINGVOLTAGE"":""Operating Voltage"",""COMMENTS"":""XFR Comments"",""WORKORDERID"":""Work Order ID"",""INSTALLATIONDATE"":""Installation Date"",""ELECTRICTRACEWEIGHT"":""Electric Trace Weight"",""FEEDERINFO"":""Feeder Information"",""SYMBOLROTATION"":""SymbolRotation"",""GROUNDREACTANCE"":""Ground Reactance"",""GROUNDRESISTANCE"":""Ground Resistance"",""HIGHSIDEGROUNDREACTANCE"":""High Side Ground Reactance"",""HIGHSIDEGROUNDRESISTANCE"":""High Side Ground Resistance"",""HIGHSIDEPROTECTION"":""High Side Protection"",""LOCATIONTYPE"":""Location Type"",""MAGNETIZINGREACTANCE"":""Magnetizing Reactance"",""MAGNETIZINGRESISTANCE"":""Magnetizing Resistance"",""LABELTEXT"":""Label Text"",""PHASEDESIGNATION"":""Phase Designation"",""NOMINALVOLTAGE"":""Nominal Voltage"",""RATEDKVA"":""Rated kVA"",""HIGHSIDECONFIGURATION"":""High Side Configuration"",""LOWSIDECONFIGURATION"":""Low Side Configuration"",""LOADTAPCHANGERINDICATOR"":""Load Tap Changer Indicator"",""LOWSIDEGROUNDREACTANCE"":""Low Side Ground Reactance"",""LOWSIDEGROUNDRESISTANCE"":""Low Side Ground Resistance"",""LOWSIDEPROTECTION"":""Low Side Protection"",""LOWSIDEVOLTAGE"":""Low Side Voltage"",""RATEDKVA65RISE"":""Rated kVA 65 Rise"",""RATEDTERTIARYKVA"":""Rated Tertiary kVA"",""SWITCHTYPE"":""Switch Type"",""TERTIARYCONFIGURATION"":""Tertiary Configuration"",""TERTIARYVOLTAGE"":""Tertiary Voltage"",""WORKREQUESTID"":""Work Request ID"",""DESIGNID"":""Design ID"",""WORKLOCATIONID"":""Work Location ID"",""WORKFLOWSTATUS"":""Work Flow Status"",""WORKFUNCTION"":""Work Function"",""GlobalID"":""GlobalID"",""ParentCircuitSourceID"":""ParentCircuitSourceID"",""CircuitSourceID"":""CircuitSourceID"",""SubSource"":""SubSource"",""FilledWeight"":""FilledWeight"",""EmptyWeight"":""EmptyWeight"",""HeightBushings"":""HeightBushings"",""HeightNoBushings"":""HeightNoBushings"",""AlternateX"":""AlternateX"",""AlternateY"":""AlternateY"",""AlternateZ"":""AlternateZ"",""AlternateSource"":""AlternateSource""},""geometryType"":""esriGeometryPoint"",""spatialReference"":{""wkid"":26753},""fields"":[{""name"":""OBJECTID"",""type"":""esriFieldTypeOID"",""alias"":""Object ID""},{""name"":""ANCILLARYROLE"",""type"":""esriFieldTypeSmallInteger"",""alias"":""ANCILLARYROLE""},{""name"":""ENABLED"",""type"":""esriFieldTypeSmallInteger"",""alias"":""Enabled""},{""name"":""CREATIONUSER"",""type"":""esriFieldTypeString"",""alias"":""Creation User"",""length"":50},{""name"":""DATECREATED"",""type"":""esriFieldTypeDate"",""alias"":""Date Created"",""length"":8},{""name"":""DATEMODIFIED"",""type"":""esriFieldTypeDate"",""alias"":""Date Modified"",""length"":8},{""name"":""LASTUSER"",""type"":""esriFieldTypeString"",""alias"":""Last User"",""length"":50},{""name"":""SUBTYPECD"",""type"":""esriFieldTypeInteger"",""alias"":""SubtypeCD""},{""name"":""FACILITYID"",""type"":""esriFieldTypeString"",""alias"":""Facility ID"",""length"":20},{""name"":""FEEDERID"",""type"":""esriFieldTypeString"",""alias"":""Feeder ID"",""length"":20},{""name"":""FEEDERID2"",""type"":""esriFieldTypeString"",""alias"":""Feeder ID 2"",""length"":20},{""name"":""OPERATINGVOLTAGE"",""type"":""esriFieldTypeInteger"",""alias"":""Operating Voltage""},{""name"":""COMMENTS"",""type"":""esriFieldTypeString"",""alias"":""XFR Comments"",""length"":100},{""name"":""WORKORDERID"",""type"":""esriFieldTypeString"",""alias"":""Work Order ID"",""length"":20},{""name"":""INSTALLATIONDATE"",""type"":""esriFieldTypeDate"",""alias"":""Installation Date"",""length"":8},{""name"":""ELECTRICTRACEWEIGHT"",""type"":""esriFieldTypeInteger"",""alias"":""Electric Trace Weight""},{""name"":""FEEDERINFO"",""type"":""esriFieldTypeInteger"",""alias"":""Feeder Information""},{""name"":""SYMBOLROTATION"",""type"":""esriFieldTypeDouble"",""alias"":""SymbolRotation""},{""name"":""GROUNDREACTANCE"",""type"":""esriFieldTypeInteger"",""alias"":""Ground Reactance""},{""name"":""GROUNDRESISTANCE"",""type"":""esriFieldTypeInteger"",""alias"":""Ground Resistance""},{""name"":""HIGHSIDEGROUNDREACTANCE"",""type"":""esriFieldTypeInteger"",""alias"":""High Side Ground Reactance""},{""name"":""HIGHSIDEGROUNDRESISTANCE"",""type"":""esriFieldTypeInteger"",""alias"":""High Side Ground Resistance""},{""name"":""HIGHSIDEPROTECTION"",""type"":""esriFieldTypeString"",""alias"":""High Side Protection"",""length"":5},{""name"":""LOCATIONTYPE"",""type"":""esriFieldTypeString"",""alias"":""Location Type"",""length"":5},{""name"":""MAGNETIZINGREACTANCE"",""type"":""esriFieldTypeDouble"",""alias"":""Magnetizing Reactance""},{""name"":""MAGNETIZINGRESISTANCE"",""type"":""esriFieldTypeDouble"",""alias"":""Magnetizing Resistance""},{""name"":""LABELTEXT"",""type"":""esriFieldTypeString"",""alias"":""Label Text"",""length"":100},{""name"":""PHASEDESIGNATION"",""type"":""esriFieldTypeInteger"",""alias"":""Phase Designation""},{""name"":""NOMINALVOLTAGE"",""type"":""esriFieldTypeInteger"",""alias"":""Nominal Voltage""},{""name"":""RATEDKVA"",""type"":""esriFieldTypeDouble"",""alias"":""Rated kVA""},{""name"":""HIGHSIDECONFIGURATION"",""type"":""esriFieldTypeString"",""alias"":""High Side Configuration"",""length"":5},{""name"":""LOWSIDECONFIGURATION"",""type"":""esriFieldTypeString"",""alias"":""Low Side Configuration"",""length"":5},{""name"":""LOADTAPCHANGERINDICATOR"",""type"":""esriFieldTypeString"",""alias"":""Load Tap Changer Indicator"",""length"":5},{""name"":""LOWSIDEGROUNDREACTANCE"",""type"":""esriFieldTypeDouble"",""alias"":""Low Side Ground Reactance""},{""name"":""LOWSIDEGROUNDRESISTANCE"",""type"":""esriFieldTypeDouble"",""alias"":""Low Side Ground Resistance""},{""name"":""LOWSIDEPROTECTION"",""type"":""esriFieldTypeString"",""alias"":""Low Side Protection"",""length"":20},{""name"":""LOWSIDEVOLTAGE"",""type"":""esriFieldTypeInteger"",""alias"":""Low Side Voltage""},{""name"":""RATEDKVA65RISE"",""type"":""esriFieldTypeInteger"",""alias"":""Rated kVA 65 Rise""},{""name"":""RATEDTERTIARYKVA"",""type"":""esriFieldTypeInteger"",""alias"":""Rated Tertiary kVA""},{""name"":""SWITCHTYPE"",""type"":""esriFieldTypeString"",""alias"":""Switch Type"",""length"":20},{""name"":""TERTIARYCONFIGURATION"",""type"":""esriFieldTypeString"",""alias"":""Tertiary Configuration"",""length"":20},{""name"":""TERTIARYVOLTAGE"",""type"":""esriFieldTypeInteger"",""alias"":""Tertiary Voltage""},{""name"":""WORKREQUESTID"",""type"":""esriFieldTypeString"",""alias"":""Work Request ID"",""length"":20},{""name"":""DESIGNID"",""type"":""esriFieldTypeString"",""alias"":""Design ID"",""length"":20},{""name"":""WORKLOCATIONID"",""type"":""esriFieldTypeString"",""alias"":""Work Location ID"",""length"":20},{""name"":""WORKFLOWSTATUS"",""type"":""esriFieldTypeInteger"",""alias"":""Work Flow Status""},{""name"":""WORKFUNCTION"",""type"":""esriFieldTypeInteger"",""alias"":""Work Function""},{""name"":""GlobalID"",""type"":""esriFieldTypeGlobalID"",""alias"":""GlobalID"",""length"":38},{""name"":""ParentCircuitSourceID"",""type"":""esriFieldTypeInteger"",""alias"":""ParentCircuitSourceID""},{""name"":""CircuitSourceID"",""type"":""esriFieldTypeInteger"",""alias"":""CircuitSourceID""},{""name"":""SubSource"",""type"":""esriFieldTypeSmallInteger"",""alias"":""SubSource""},{""name"":""FilledWeight"",""type"":""esriFieldTypeString"",""alias"":""FilledWeight"",""length"":50},{""name"":""EmptyWeight"",""type"":""esriFieldTypeString"",""alias"":""EmptyWeight"",""length"":50},{""name"":""HeightBushings"",""type"":""esriFieldTypeString"",""alias"":""HeightBushings"",""length"":50},{""name"":""HeightNoBushings"",""type"":""esriFieldTypeString"",""alias"":""HeightNoBushings"",""length"":50},{""name"":""AlternateX"",""type"":""esriFieldTypeString"",""alias"":""AlternateX"",""length"":50},{""name"":""AlternateY"",""type"":""esriFieldTypeString"",""alias"":""AlternateY"",""length"":50},{""name"":""AlternateZ"",""type"":""esriFieldTypeString"",""alias"":""AlternateZ"",""length"":50},{""name"":""AlternateSource"",""type"":""esriFieldTypeString"",""alias"":""AlternateSource"",""length"":50}],""features"":[{""attributes"":{""OBJECTID"":12,""ANCILLARYROLE"":null,""ENABLED"":1,""CREATIONUSER"":""kellyl"",""DATECREATED"":988329600000,""DATEMODIFIED"":1027382400000,""LASTUSER"":""lisa"",""SUBTYPECD"":2,""FACILITYID"":""XFR15"",""FEEDERID"":""GR-808"",""FEEDERID2"":null,""OPERATINGVOLTAGE"":160,""COMMENTS"":null,""WORKORDERID"":null,""INSTALLATIONDATE"":null,""ELECTRICTRACEWEIGHT"":805322776,""FEEDERINFO"":4,""SYMBOLROTATION"":310.91000000000003,""GROUNDREACTANCE"":null,""GROUNDRESISTANCE"":null,""HIGHSIDEGROUNDREACTANCE"":0,""HIGHSIDEGROUNDRESISTANCE"":0,""HIGHSIDEPROTECTION"":""BR"",""LOCATIONTYPE"":""SVC"",""MAGNETIZINGREACTANCE"":null,""MAGNETIZINGRESISTANCE"":null,""LABELTEXT"":null,""PHASEDESIGNATION"":1,""NOMINALVOLTAGE"":210,""RATEDKVA"":37.5,""HIGHSIDECONFIGURATION"":""LL"",""LOWSIDECONFIGURATION"":""SP"",""LOADTAPCHANGERINDICATOR"":""NA"",""LOWSIDEGROUNDREACTANCE"":0,""LOWSIDEGROUNDRESISTANCE"":0,""LOWSIDEPROTECTION"":""NA"",""LOWSIDEVOLTAGE"":30,""RATEDKVA65RISE"":0,""RATEDTERTIARYKVA"":0,""SWITCHTYPE"":null,""TERTIARYCONFIGURATION"":""NA"",""TERTIARYVOLTAGE"":0,""WORKREQUESTID"":null,""DESIGNID"":null,""WORKLOCATIONID"":null,""WORKFLOWSTATUS"":0,""WORKFUNCTION"":0,""GlobalID"":""{4A8C85E2-0028-4A06-AD83-A5A469973060}"",""ParentCircuitSourceID"":13,""CircuitSourceID"":null,""SubSource"":null,""FilledWeight"":null,""EmptyWeight"":null,""HeightBushings"":null,""HeightNoBushings"":null,""AlternateX"":null,""AlternateY"":null,""AlternateZ"":null,""AlternateSource"":null},""geometry"":{""x"":2214155.5223696935,""y"":399668.75619776896}}],""name"":""Transformer"",""id"":0,""exceededThreshold"":false}]}";
            List<IResultSet> sets = ResultSet.FromResults(json);
            Assert.AreEqual(sets.Count, 1);
            Assert.AreEqual(sets[0].Features.Count, 1);
            Assert.IsTrue(sets[0].Features[0].Attributes.Count > 2);
        }

    }
}