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
    public class SubTypeTests
    {
        [TestMethod]
        public void FromJson_NullJsonAndNullFields_NullResult()
        {
            Assert.IsNull(SubType.FromJson(null, null));
        }

        [TestMethod]
        public void FromJson_EmptyJsonAndNullFields_NoExceptions()
        {
            Assert.IsNotNull(SubType.FromJson(new JsonObject(null), null));
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void FromJson_NullIdAndNullFields_ThrowException()
        {
            JsonObject json = new JsonObject(null);
            json.Add("id", null);
            SubType.FromJson(json, null);
        }

        [TestMethod]
        public void FromJson_NullNameAndNullFields_NoExceptions()
        {
            JsonObject json = new JsonObject(null);
            json.Add("name", null);
            Assert.IsNotNull(SubType.FromJson(json, null));
        }

        [TestMethod]
        public void FromJson_EmptyNameAndNullFields_NoExceptions()
        {
            JsonObject json = new JsonObject(null);
            json.Add("name", "");
            Assert.IsNotNull(SubType.FromJson(json, null));
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void FromJson_NullDomainAndNullFields_ThrowException()
        {
            JsonObject json = new JsonObject(null);
            json.Add("domains", null);
            SubType.FromJson(json, null);
        }

        [TestMethod]
        public void FromJson_EmptyDomainAndNullFields_PartialResult()
        {
            JsonObject json = new JsonObject(null);
            json.Add("id", 12345);
            json.Add("name", "abc");
            json.Add("domains", new JsonObject(null));
            SubType type = SubType.FromJson(json, null);
            Assert.AreEqual(type.ID, 12345);
            Assert.AreEqual(type.Name, "abc");
            Assert.IsNotNull(type.Domains);
            Assert.AreEqual(type.Domains.Count, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FromJson_NoneEmptyDomainAndNullFields_ThrowException()
        {
            JsonObject json = new JsonObject(null);
            json.Add("id", 12345);
            json.Add("name", "abc");
            json.Add("domains", new JsonObject(new KeyValuePair<string, JsonValue>("def", new JsonObject(new KeyValuePair<string, JsonValue>("type", "inherited")))));
            SubType.FromJson(json, null);
        }

        [TestMethod]
        public void FromJson_NoneEmptyDomainAndEmptyFields_PartialResult()
        {
            JsonObject json = new JsonObject(null);
            json.Add("id", 12345);
            json.Add("name", "abc");
            json.Add("domains", new JsonObject(new KeyValuePair<string, JsonValue>("def", new JsonObject(new KeyValuePair<string, JsonValue>("type", "inherited")))));
            SubType type = SubType.FromJson(json, new List<Field>());
            Assert.AreEqual(type.ID, 12345);
            Assert.AreEqual(type.Name, "abc");
            Assert.IsNotNull(type.Domains);
            Assert.AreEqual(type.Domains.Count, 0);
        }

        [TestMethod]
        public void FromJson_NoneEmptyDomainAndNonEmptyFields_ValidResult()
        {
            JsonObject json = new JsonObject(null);
            json.Add("id", 12345);
            json.Add("name", "abc");
            json.Add("domains", new JsonObject(new KeyValuePair<string, JsonValue>("def", new JsonObject(new KeyValuePair<string, JsonValue>("type", "inherited")))));

            JsonObject fieldJson = new JsonObject(null);
            fieldJson.Add("name", "def");
            fieldJson.Add("domain", new JsonObject(new KeyValuePair<string, JsonValue>("type", "codedValue")));
            List<Field> fields = new List<Field>();
            fields.Add(Field.FromJsonObject(fieldJson));

            SubType type = SubType.FromJson(json, fields);
            Assert.AreEqual(type.ID, 12345);
            Assert.AreEqual(type.Name, "abc");
            Assert.IsNotNull(type.Domains);
            Assert.AreEqual(type.Domains.Count, 1);
        }
    }
}