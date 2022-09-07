using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ESRI.ArcGIS.Client;

#if SILVERLIGHT
namespace Miner.Server.Client.UnitTests
#elif WPF
namespace Miner.Mobile.Client.UnitTests
#endif
{
    [TestClass]
    public class ShapeDrawTests
    {
        [TestMethod]
        public void DefaultConstructor_NoExceptions()
        {
            ShapeDraw draw = new ShapeDraw();
            Assert.IsNotNull(draw);
        }

        [TestMethod]
        public void MapConstructor_MapNull_NoExceptions()
        {
            ShapeDraw draw = new ShapeDraw(null, DrawMode.Arrow);
            Assert.IsNotNull(draw);
        }

        [TestMethod]
        public void MapConstructor_MapNotNull_NoExceptions()
        {
            ShapeDraw draw = new ShapeDraw(new Map(), DrawMode.Arrow);
            Assert.IsNotNull(draw);
        }
    }
}
