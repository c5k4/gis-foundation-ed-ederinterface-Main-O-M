using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Miner.ComCategories;
using Miner.Framework.Trace.Strategies;
using System.Drawing;

namespace PGE.Desktop.EDER.ArcMapCommands.PGE_Tracing
{
    [Guid("7f9b35f9-db11-4fe4-bbe1-650e7d82a060")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("PGE.Desktop.EDER.ArcMapCommands.PGE_Tracing.PGE_DownstreamProtTrace")]
    [ComponentCategory(ComCategory.ArcMapCommands)]
    [ComponentCategory(ComCategory.ArcMapTraceTasks)]
    public class PGE_DownstreamProtTrace : PGE_Trace
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

        public PGE_DownstreamProtTrace()
            : base("PGE Tools", "PGE Downstream Protective Device Trace", "PGE Downstream Protective Device Trace", "PGE Downstream Protective Device Trace",
                "PGE Downstream Protective Device Trace", new ElectricDownstreamTrace(), TraceType.DownstreamProtective)
        {
            base.m_bitmap = new Bitmap(base.Res.GetImage("downstream_protective-device_trace_16"));
        }
    }
}
