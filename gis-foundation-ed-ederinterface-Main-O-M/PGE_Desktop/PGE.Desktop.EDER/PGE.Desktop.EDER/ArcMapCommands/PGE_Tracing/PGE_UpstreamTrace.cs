using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Miner.ComCategories;
using System.Drawing;
using Miner.Framework.Trace.Strategies;

namespace PGE.Desktop.EDER.ArcMapCommands.PGE_Tracing
{
    [Guid("248EAFB5-9774-4AA4-89A4-90D97EA7E98C")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("PGE.Desktop.EDER.ArcMapCommands.PGE_Tracing.PGE_UpstreamTrace")]
    [ComponentCategory(ComCategory.ArcMapCommands)]
    [ComponentCategory(ComCategory.ArcMapTraceTasks)]
    public class PGE_UpstreamTrace : PGE_Trace
    {
        #region Com Reg Methods

        /// <summary>
        /// Registers the specified reg key.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComRegisterFunction]
        private static void Register(string regKey)
        {
            Miner.ComCategories.ArcMapCommands.Register(regKey);
            Miner.ComCategories.ArcMapTraceTasks.Register(regKey);
        }
        /// <summary>
        /// Uns the register.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComUnregisterFunction]
        private static void UnRegister(string regKey)
        {
            Miner.ComCategories.ArcMapCommands.Unregister(regKey);
            Miner.ComCategories.ArcMapTraceTasks.Unregister(regKey);
        }

        #endregion

        public PGE_UpstreamTrace()
            : base("PGE Tools", "PGE Upstream Trace", "PGE Upstream Trace", "PGE Upstream Trace",
                "PGE Upstream Trace", new ElectricUpstreamTrace(), TraceType.Upstream)
        {
            base.m_bitmap = new Bitmap(base.Res.GetImage("upstream_trace_16"));
        }
    }
}
