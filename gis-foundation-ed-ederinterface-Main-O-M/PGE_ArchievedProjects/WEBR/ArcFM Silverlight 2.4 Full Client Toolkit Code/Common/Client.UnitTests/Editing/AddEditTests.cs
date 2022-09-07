using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ESRI.ArcGIS.Client;

#if SILVERLIGHT
using Miner.Server.Client.Editing;
#elif WPF
using Miner.Mobile.Client.Editing;
#endif

#if SILVERLIGHT
namespace Miner.Server.Client.UnitTests
#elif WPF
namespace Miner.Mobile.Client.UnitTests
#endif
{
    [TestClass]
    public class AddEditTests
    {
        [TestMethod]
        public void LayerConstructor_LayerNull_NoExceptions()
        {
            AddEdit edit = new AddEdit(null);
            Assert.IsNotNull(edit);
        }

        [TestMethod]
        public void LayerConstructor_LayerNotNull_NoExceptions()
        {
            AddEdit edit = new AddEdit(new GraphicsLayer());
            Assert.IsNotNull(edit);
        }

        [TestMethod]
        public void Replay_LayerNull_AddNoGraphic()
        {
            AddEdit edit = new AddEdit(null);
            edit.Replay();
            Assert.IsNull(edit.Layer);
        }

        [TestMethod]
        public void Replay_LayerNotNull_AddOneGraphic()
        {
            AddEdit edit = new AddEdit(new GraphicsLayer());
            edit.Graphic = new Graphic();
            edit.Replay();
            Assert.AreEqual(edit.Layer.Graphics.Count, 1);
        }
    }
}
