using System.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#if SILVERLIGHT
namespace Miner.Server.Client.UnitTests
#elif WPF
namespace Miner.Mobile.Client.UnitTests
#endif
{
    [TestClass]
    public class DomainTests
    {
        [TestMethod]
        public void FromJsonObject_NullJson_NullResult()
        {
            Assert.IsNull(Domain.FromJsonObject(null));
        }

        [TestMethod]
        public void FromJsonObject_JsonDoesNotContainType_NullResult()
        {
            string json = "{}";
            JsonObject jsonObject = JsonObject.Parse(json) as JsonObject;
            Assert.IsNull(Domain.FromJsonObject(jsonObject));
        }

        [TestMethod]
        public void FromJsonObject_CodedValueJson_ReturnsCodedValueDomain()
        {
            string json = @"{""type"":""codedValue""}";
            JsonObject jsonObject = JsonObject.Parse(json) as JsonObject;
            Assert.IsInstanceOfType(Domain.FromJsonObject(jsonObject), typeof(CodedValueDomain));
        }

        [TestMethod]
        public void FromJsonObject_NOTCodedValueJson_ReturnsNull()
        {
            string json = @"{""type"":""NOTcodedValue""}";
            JsonObject jsonObject = JsonObject.Parse(json) as JsonObject;
            Assert.IsNull(Domain.FromJsonObject(jsonObject));
        }
    }
}