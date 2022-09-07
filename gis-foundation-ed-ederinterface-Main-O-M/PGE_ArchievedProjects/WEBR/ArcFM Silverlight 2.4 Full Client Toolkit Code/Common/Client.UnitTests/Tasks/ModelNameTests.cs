using System;
using System.Collections.Generic;
using System.Json;

#if SILVERLIGHT
using Miner.Server.Client.Tasks;
#elif WPF
using Miner.Mobile.Client.Tasks;
#endif

using Microsoft.VisualStudio.TestTools.UnitTesting;

#if SILVERLIGHT
namespace Miner.Server.Client.UnitTests
#elif WPF
namespace Miner.Mobile.Client.UnitTests
#endif
{
    [TestClass]
    public class ModelNameTests
    {
        [TestMethod]
        public void FromResults_NullJsonAndNullUrl_EmptyResults()
        {
            List<ModelName> results = ModelName.FromResults(null, null);
            Assert.IsNotNull(results);
            Assert.AreEqual(results.Count, 0);
        }

        [TestMethod]
        public void FromResults_EmptyJsonAndNullUrl_EmptyResults()
        {
            List<ModelName> results = ModelName.FromResults("", null);
            Assert.IsNotNull(results);
            Assert.AreEqual(results.Count, 0);
        }

        [TestMethod]
#if SILVERLIGHT
        [ExpectedException(typeof(NullReferenceException))]
#elif WPF
        [ExpectedException(typeof(ArgumentException))]
#endif
        public void FromResults_InvalidJsonAndNullUrl_ThrowException()
        {
            ModelName.FromResults(" ", null);
        }

        [TestMethod]
        public void FromResults_ErrorJsonAndNullUrl_EmptyResults()
        {
            List<ModelName> results = ModelName.FromResults(@"{""error"":""""}", null);
            Assert.IsNotNull(results);
            Assert.AreEqual(results.Count, 0);
        }

        [TestMethod]
        public void FromResults_MinimalJsonAndNullUrl_OneResult()
        {
            List<ModelName> results = ModelName.FromResults(@"{""IdentifyModelNames"":[{""text"":""Electric Line Features"",""value"":""Electric Line Features"",""layerIDs"":[15,16,17,18,20,21,22,23,25,26,27,28,30,31,32,33]}],""ProtectiveDevices"":{""layerIDs"":[1,6,7,10,11]}}", null);
            Assert.IsNotNull(results);
            Assert.AreEqual(results.Count, 1);
        }

        [TestMethod]
        public void FromResults_MinimalJsonAndEmptyUrl_OneResult()
        {
            List<ModelName> results = ModelName.FromResults(@"{""IdentifyModelNames"":[{""text"":""Electric Line Features"",""value"":""Electric Line Features"",""layerIDs"":[15,16,17,18,20,21,22,23,25,26,27,28,30,31,32,33]}],""ProtectiveDevices"":{""layerIDs"":[1,6,7,10,11]}}", "");
            Assert.IsNotNull(results);
            Assert.AreEqual(results.Count, 1);
        }

        [TestMethod]
        public void FromResults_ValidJsonAndNoneEmptyUrl_OneResult()
        {
            List<ModelName> results = ModelName.FromResults(@"{""id"":[{""id"":0,""name"":""Switchable Devices"",""type"":""Group Layer"",""parentLayerId"":-1,""subLayerIds"":[1,2,3]},{""id"":1,""name"":""Transformer"",""type"":""Feature Layer"",""parentLayerId"":0,""subLayerIds"":null},{""id"":2,""name"":""Switch"",""type"":""Feature Layer"",""parentLayerId"":0,""subLayerIds"":null},{""id"":3,""name"":""Fuse"",""type"":""Feature Layer"",""parentLayerId"":0,""subLayerIds"":null},{""id"":4,""name"":""Terminal Devices"",""type"":""Group Layer"",""parentLayerId"":-1,""subLayerIds"":[5,6,7,8]},{""id"":5,""name"":""Dynamic Protective Device"",""type"":""Feature Layer"",""parentLayerId"":4,""subLayerIds"":null},{""id"":6,""name"":""Fuse"",""type"":""Feature Layer"",""parentLayerId"":4,""subLayerIds"":null},{""id"":7,""name"":""Open Point"",""type"":""Feature Layer"",""parentLayerId"":4,""subLayerIds"":null},{""id"":8,""name"":""Switch"",""type"":""Feature Layer"",""parentLayerId"":4,""subLayerIds"":null},{""id"":9,""name"":""Tie Devices"",""type"":""Group Layer"",""parentLayerId"":-1,""subLayerIds"":[10,11,12,13]},{""id"":10,""name"":""Switch"",""type"":""Feature Layer"",""parentLayerId"":9,""subLayerIds"":null},{""id"":11,""name"":""Dynamic Protective Device"",""type"":""Feature Layer"",""parentLayerId"":9,""subLayerIds"":null},{""id"":12,""name"":""Fuse"",""type"":""Feature Layer"",""parentLayerId"":9,""subLayerIds"":null},{""id"":13,""name"":""Open Point"",""type"":""Feature Layer"",""parentLayerId"":9,""subLayerIds"":null},{""id"":14,""name"":""Conductors"",""type"":""Group Layer"",""parentLayerId"":-1,""subLayerIds"":[15,16,17,18]},{""id"":15,""name"":""Primary Overhead Conductor"",""type"":""Feature Layer"",""parentLayerId"":14,""subLayerIds"":null},{""id"":16,""name"":""Primary Underground Conductor"",""type"":""Feature Layer"",""parentLayerId"":14,""subLayerIds"":null},{""id"":17,""name"":""Secondary Overhead Conductor"",""type"":""Feature Layer"",""parentLayerId"":14,""subLayerIds"":null},{""id"":18,""name"":""Secondary Underground Conductor"",""type"":""Feature Layer"",""parentLayerId"":14,""subLayerIds"":null},{""id"":19,""name"":""Islands - Conductors Only"",""type"":""Group Layer"",""parentLayerId"":-1,""subLayerIds"":[20,21,22,23]},{""id"":20,""name"":""PriOH Islands"",""type"":""Feature Layer"",""parentLayerId"":19,""subLayerIds"":null},{""id"":21,""name"":""PriUG Islands"",""type"":""Feature Layer"",""parentLayerId"":19,""subLayerIds"":null},{""id"":22,""name"":""SecOH Islands"",""type"":""Feature Layer"",""parentLayerId"":19,""subLayerIds"":null},{""id"":23,""name"":""SecUG Islands"",""type"":""Feature Layer"",""parentLayerId"":19,""subLayerIds"":null},{""id"":24,""name"":""Loops - Conductors Only"",""type"":""Group Layer"",""parentLayerId"":-1,""subLayerIds"":[25,26,27,28]},{""id"":25,""name"":""PriOH Loops"",""type"":""Feature Layer"",""parentLayerId"":24,""subLayerIds"":null},{""id"":26,""name"":""PriUG Loops"",""type"":""Feature Layer"",""parentLayerId"":24,""subLayerIds"":null},{""id"":27,""name"":""SecOH Loops"",""type"":""Feature Layer"",""parentLayerId"":24,""subLayerIds"":null},{""id"":28,""name"":""SecUG Loops"",""type"":""Feature Layer"",""parentLayerId"":24,""subLayerIds"":null},{""id"":29,""name"":""Multiple Feed Conductors"",""type"":""Group Layer"",""parentLayerId"":-1,""subLayerIds"":[30,31,32,33]},{""id"":30,""name"":""PriOH Multiple Feed"",""type"":""Feature Layer"",""parentLayerId"":29,""subLayerIds"":null},{""id"":31,""name"":""PriUG Multiple Feed"",""type"":""Feature Layer"",""parentLayerId"":29,""subLayerIds"":null},{""id"":32,""name"":""SecOH Multiple Feed"",""type"":""Feature Layer"",""parentLayerId"":29,""subLayerIds"":null},{""id"":33,""name"":""SecUG Multiple Feed"",""type"":""Feature Layer"",""parentLayerId"":29,""subLayerIds"":null},{""id"":34,""name"":""Bus Bar"",""type"":""Feature Layer"",""parentLayerId"":-1,""subLayerIds"":null},{""id"":35,""name"":""Support Structure"",""type"":""Feature Layer"",""parentLayerId"":-1,""subLayerIds"":null},{""id"":36,""name"":""Dynamic Protective Devices"",""type"":""Feature Layer"",""parentLayerId"":-1,""subLayerIds"":null},{""id"":37,""name"":""Power Factor Correcting Equipment"",""type"":""Feature Layer"",""parentLayerId"":-1,""subLayerIds"":null},{""id"":38,""name"":""Miscellaneous Network Features"",""type"":""Feature Layer"",""parentLayerId"":-1,""subLayerIds"":null},{""id"":39,""name"":""Street Light"",""type"":""Feature Layer"",""parentLayerId"":-1,""subLayerIds"":null},{""id"":40,""name"":""Primary Meter"",""type"":""Feature Layer"",""parentLayerId"":-1,""subLayerIds"":null},{""id"":41,""name"":""Service Point"",""type"":""Feature Layer"",""parentLayerId"":-1,""subLayerIds"":null},{""id"":42,""name"":""Electric Station"",""type"":""Feature Layer"",""parentLayerId"":-1,""subLayerIds"":null},{""id"":43,""name"":""Undisplayed Network Features"",""type"":""Group Layer"",""parentLayerId"":-1,""subLayerIds"":[44,45,46,47,48]},{""id"":44,""name"":""Delivery Point"",""type"":""Feature Layer"",""parentLayerId"":43,""subLayerIds"":null},{""id"":45,""name"":""ElecGeomNetwork_Junctions"",""type"":""Feature Layer"",""parentLayerId"":43,""subLayerIds"":null},{""id"":46,""name"":""Voltage Regulator"",""type"":""Feature Layer"",""parentLayerId"":43,""subLayerIds"":null},{""id"":47,""name"":""Open Point"",""type"":""Feature Layer"",""parentLayerId"":43,""subLayerIds"":null},{""id"":48,""name"":""Generator"",""type"":""Feature Layer"",""parentLayerId"":43,""subLayerIds"":null},{""id"":49,""name"":""Street Centerline"",""type"":""Feature Layer"",""parentLayerId"":-1,""subLayerIds"":null},{""id"":50,""name"":""Parcel"",""type"":""Feature Layer"",""parentLayerId"":-1,""subLayerIds"":null},{""id"":51,""name"":""Map Grid"",""type"":""Feature Layer"",""parentLayerId"":-1,""subLayerIds"":null},{""id"":52,""name"":""Street Edge"",""type"":""Feature Layer"",""parentLayerId"":-1,""subLayerIds"":null},{""id"":53,""name"":""Assembly"",""type"":""Table""},{""id"":54,""name"":""AbandonedRemvdElecLineSegment"",""type"":""Table""},{""id"":55,""name"":""CapacitorControl"",""type"":""Table""},{""id"":56,""name"":""CapacitorUnit"",""type"":""Table""},{""id"":57,""name"":""CircuitSource"",""type"":""Table""},{""id"":58,""name"":""CONDUCTORINFO"",""type"":""Table""},{""id"":59,""name"":""CUSTOMERINFO"",""type"":""Table""},{""id"":60,""name"":""DUCTDEFINITION"",""type"":""Table""},{""id"":61,""name"":""FUSEUNIT"",""type"":""Table""},{""id"":62,""name"":""InductionMotor"",""type"":""Table""},{""id"":63,""name"":""JOINTUSEATTACHMENT"",""type"":""Table""},{""id"":64,""name"":""LOADSUMMARY"",""type"":""Table""},{""id"":65,""name"":""LoadTapChanger"",""type"":""Table""},{""id"":66,""name"":""NetworkProtector"",""type"":""Table""},{""id"":67,""name"":""RecloserUnit"",""type"":""Table""},{""id"":68,""name"":""SectionalizerUnit"",""type"":""Table""},{""id"":69,""name"":""SERVICEADDRESS"",""type"":""Table""},{""id"":70,""name"":""SWITCHUNIT"",""type"":""Table""},{""id"":71,""name"":""SynchronousMotor"",""type"":""Table""},{""id"":72,""name"":""TRANSFORMERUNIT"",""type"":""Table""},{""id"":73,""name"":""VoltageRegulatorUnit"",""type"":""Table""}],""IdentifyModelNames"":[{""text"":""Electric Line Features"",""value"":""Electric Line Features"",""layerIDs"":[15,16,17,18,20,21,22,23,25,26,27,28,30,31,32,33]}],""ProtectiveDevices"":{""layerIDs"":[1,6,7,10,11]}}", "abc");
            Assert.IsNotNull(results);
            Assert.AreEqual(results.Count, 1);
        }

        //[TestMethod]
        //public void FromJsonObject_MinimalJson_NotNullResult()
        //{
        //    string json = @""{""""type"""":""""codedValue""""}"";
        //    JsonObject jsonObject = JsonObject.Parse(json) as JsonObject;
        //    Assert.IsNotNull(CodedValueDomain.FromJsonObject(jsonObject));
        //}

        //[TestMethod]
        //public void FromJsonObject_MinimalJson_NullNameAndCodedValues()
        //{
        //    string json = @""{""""type"""":""""codedValue""""}"";
        //    JsonObject jsonObject = JsonObject.Parse(json) as JsonObject;
        //    CodedValueDomain domain = CodedValueDomain.FromJsonObject(jsonObject);
        //    Assert.IsNull(domain.Name);
        //    Assert.IsNull(domain.CodedValues);
        //}

        //[TestMethod]
        //public void FromJsonObject_JsonContainsName_NotNullName()
        //{
        //    string json = @""{""""type"""":""""codedValue"""",""""name"""":""""EnabledDomain""""}"";
        //    JsonObject jsonObject = JsonObject.Parse(json) as JsonObject;
        //    CodedValueDomain domain = CodedValueDomain.FromJsonObject(jsonObject);
        //    Assert.IsNotNull(domain.Name);
        //    Assert.AreEqual(domain.Name, ""EnabledDomain"");
        //}

        //[TestMethod]
        //public void FromJsonObject_FullyValidJson_FullyPopulatedDomain()
        //{
        //    string json = @""{""""type"""":""""codedValue"""",""""name"""":""""EnabledDomain"""",""""codedValues"""":[{""""name"""":""""False"""",""""code"""":0},{""""name"""":""""True"""",""""code"""":1}]}"";
        //    JsonObject jsonObject = JsonObject.Parse(json) as JsonObject;
        //    CodedValueDomain domain = CodedValueDomain.FromJsonObject(jsonObject);
        //    Assert.AreEqual(domain.Name, ""EnabledDomain"");
        //    Assert.IsNotNull(domain.CodedValues);
        //}
    }
}