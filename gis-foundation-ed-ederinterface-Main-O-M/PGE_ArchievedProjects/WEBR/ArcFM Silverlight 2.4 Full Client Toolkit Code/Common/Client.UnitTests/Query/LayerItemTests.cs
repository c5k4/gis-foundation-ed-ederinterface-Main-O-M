using Microsoft.VisualStudio.TestTools.UnitTesting;

#if SILVERLIGHT
namespace Miner.Server.Client.UnitTests
#elif WPF
namespace Miner.Mobile.Client.UnitTests
#endif
{
    [TestClass]
    public class LayerItemTests
    {
        [TestMethod]
        public void Constructor_NoExceptions()
        {
            var item = new LayerItem(null, null, 0, false, false);

            Assert.IsNotNull(item);
            Assert.IsNull(item.Layer);
            Assert.IsNull(item.Label);
            Assert.AreEqual(item.SubLayerID, 0);
            Assert.IsFalse(item.IsVisible);
            Assert.IsFalse(item.IsEnabled);
        }
    }
}
