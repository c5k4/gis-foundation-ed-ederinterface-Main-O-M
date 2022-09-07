using System;
using System.Collections.Generic;
using System.Json;

using ESRI.ArcGIS.Client;

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
    public class FieldTests
    {
        [TestMethod]
        public void FromJson_NullJson_ReturnsNull()
        {
            Assert.IsNull(Field.FromJsonObject(null));
        }

        [TestMethod]
        public void FromJson_EmptyJson_ReturnsNull()
        {
            Assert.IsNull(Field.FromJsonObject(new JsonObject(null)));
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void FromJson_NullName_ThrowException()
        {
            JsonObject json = new JsonObject(null);
            json.Add("name", null);
            Field.FromJsonObject(json);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void FromJson_NoName_ThrowException()
        {
            JsonObject json = new JsonObject();
            json.Add("test", "string");
            Field.FromJsonObject(json);
        }

        [TestMethod]
        public void FromJson_EmptyName_NoExceptions()
        {
            JsonObject json = new JsonObject(null);
            json.Add("name", "");
            Assert.IsNotNull(Field.FromJsonObject(json));
        }

        [TestMethod]
        public void FromJson_NullDomain_NoExceptions()
        {
            JsonObject json = new JsonObject(null);
            json.Add("name", "abc");
            json.Add("type", "System.Int32");
            json.Add("domain", null);
            Field fld = Field.FromJsonObject(json);
        }

        [TestMethod]
        public void FromJson_NoDomain_PartialResult()
        {
            JsonObject json = new JsonObject(null);
            json.Add("name", "abc");
            json.Add("type", "System.Int32");
            Field fld = Field.FromJsonObject(json);
            Assert.AreEqual(fld.Name, "abc");
            Assert.AreEqual(fld.Type, ESRI.ArcGIS.Client.Field.FieldType.Integer);
        }

        [TestMethod]
        public void FromJson_ValidJson_ValidResult()
        {
            JsonObject fldJson = new JsonObject(null);
            fldJson.Add("name", "abc");
            fldJson.Add("type", "System.Int32");

            fldJson.Add("domain", new JsonObject(new KeyValuePair<string, JsonValue>("type", "codedValue")));
            Field fld = Field.FromJsonObject(fldJson);

            Assert.AreEqual(fld.Name, "abc");
            Assert.AreEqual(fld.Type, ESRI.ArcGIS.Client.Field.FieldType.Integer);
            Assert.IsNotNull(fld.Domain);
            Assert.IsNotNull(fld.Domain as CodedValueDomain);
        }
    }
}
