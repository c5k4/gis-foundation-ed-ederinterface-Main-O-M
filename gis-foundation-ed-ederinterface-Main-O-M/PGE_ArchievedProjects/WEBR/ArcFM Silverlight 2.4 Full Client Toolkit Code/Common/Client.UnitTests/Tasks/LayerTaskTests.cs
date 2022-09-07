using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ESRI.ArcGIS.Client.Tasks;

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
    public class LayerTaskTests
    {
        [TestMethod]
        public void DefaultConstructor_NoExceptions()
        {
            LayerTask task = new LayerTask();
        }

        [TestMethod]
        public void Constructor_ParameterIsNull_IsNull()
        {
            LayerTask task = new LayerTask(null);
            Assert.IsNull(task.Url);
        }

        [TestMethod]
        public void Constructor_ParameterIsEmpty_IsEmpty()
        {
            LayerTask task = new LayerTask("");
            Assert.AreEqual("", task.Url);
        }

        [TestMethod]
        public void Constructor_ParameterNotEmpty_IsNotNull()
        {
            LayerTask task = new LayerTask("abc");
            Assert.IsNotNull(task.Url);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ExecuteAsync_NullUserTokenAndNullUrl_ThrowsException()
        {
            LayerTask task = new LayerTask(null);
            task.ExecuteAsync(null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ExecuteAsync_NullUserTokenAndEmptyUrl_ThrowsException()
        {
            LayerTask task = new LayerTask("");
            task.ExecuteAsync(null);
        }
    }
}