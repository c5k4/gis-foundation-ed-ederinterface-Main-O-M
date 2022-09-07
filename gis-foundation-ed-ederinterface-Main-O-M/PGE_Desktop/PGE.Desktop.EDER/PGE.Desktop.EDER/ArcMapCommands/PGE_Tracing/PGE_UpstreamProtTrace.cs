using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Miner.ComCategories;
using Miner.Framework.Trace.Strategies;
using System.Drawing;

namespace PGE.Desktop.EDER.ArcMapCommands.PGE_Tracing
{
    [Guid("dc5ce207-4f35-4def-ad7b-ec0c1d763825")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("PGE.Desktop.EDER.ArcMapCommands.PGE_Tracing.PGE_UpstreamProtTrace")]
    [ComponentCategory(ComCategory.ArcMapCommands)]
    [ComponentCategory(ComCategory.ArcMapTraceTasks)]
    public class PGE_UpstreamProtTrace : PGE_Trace
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

        public PGE_UpstreamProtTrace()
            : base("PGE Tools", "PGE Upstream Protective Device Trace", "PGE Upstream Protective Device Trace", "PGE Upstream Protective Device Trace",
                "PGE Upstream Protective Device Trace", new ElectricUpstreamTrace(), TraceType.UpstreamProtective)
        {
            base.m_bitmap = new Bitmap(base.Res.GetImage("upstream_protective-device_trace_16"));
            
        }
    }
}
