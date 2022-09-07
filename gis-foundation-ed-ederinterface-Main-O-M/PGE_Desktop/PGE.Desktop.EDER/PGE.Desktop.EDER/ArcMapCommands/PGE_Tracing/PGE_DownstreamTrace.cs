using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Miner.Framework.Trace.Strategies;
using System.Drawing;
using Miner.ComCategories;
using Miner.FrameworkUI.Trace;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.EditorExt;

namespace PGE.Desktop.EDER.ArcMapCommands.PGE_Tracing
{
    [Guid("24b7f4be-af09-46ba-85aa-9e2b7413342c")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("PGE.Desktop.EDER.ArcMapCommands.PGE_Tracing.PGE_DownstreamTrace")]
    [ComponentCategory(ComCategory.ArcMapCommands)]
    [ComponentCategory(ComCategory.ArcMapTraceTasks)]
    public class PGE_DownstreamTrace : PGE_Trace
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

        public PGE_DownstreamTrace()
            : base("PGE Tools", "PGE Downstream Trace", "PGE Downstream Trace", "PGE Downstream Trace",
                "PGE Downstream Trace", new ElectricDownstreamTrace(), TraceType.Downstream)
        {
            base.m_bitmap = new Bitmap(base.Res.GetImage("downstream_trace_16"));
            
        }
        /*
        public PGE_DownstreamTrace()
            : base(new ElectricDownstreamTrace())
        {
            base.m_category = "PGE Tools";
            base.m_caption = "PGE Downstream Trace";
            base.m_message = "Traces downstream features";
            base.m_toolTip = "Traces downstream features";
            base.m_name = "PGE Downstream Trace";
            base.m_bitmap = new Bitmap(base.res.GetImage("downstream_trace_16"));
        }
        */


    }
}
