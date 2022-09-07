using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.SystemUI;
using System.Runtime.InteropServices;
using Miner.ComCategories;

namespace PGE.Desktop.EDER.ArcMapCommands.PGE_Tracing
{
    [Guid("1DA51DCA-283C-4E8E-BF18-12BD34FA6DE5")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("PGE.Desktop.EDER.ArcMapCommands.PGE_Tracing.PGE_TracingToolbar")]
    [ComponentCategory(ComCategory.ArcMapCommandBars)]
    public class PGE_TracingToolbar : IToolBarDef
    {
        #region Com Reg Methods

        /// <summary>
        /// Registers the specified reg key.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComRegisterFunction]
        private static void Register(string regKey)
        {
            Miner.ComCategories.ArcMapCommandBars.Register(regKey);
        }
        /// <summary>
        /// Uns the register.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComUnregisterFunction]
        private static void UnRegister(string regKey)
        {
            Miner.ComCategories.ArcMapCommandBars.Unregister(regKey);
        }

        #endregion

        public PGE_TracingToolbar()
        {

        }

        public string Caption
        {
            get { return "PGE Tracing"; }
        }

        public void GetItemInfo(int pos, IItemDef itemDef)
        {
            switch (pos)
            {
                case 0:
                    itemDef.ID = "PGE.Desktop.EDER.ArcMapCommands.PGE_Tracing.PGE_DownstreamTrace";
                    itemDef.Group = false;
                    break;
                case 1:
                    itemDef.ID = "PGE.Desktop.EDER.ArcMapCommands.PGE_Tracing.PGE_UpstreamTrace";
                    itemDef.Group = false;
                    break;
                case 2:
                    itemDef.ID = "PGE.Desktop.EDER.ArcMapCommands.PGE_Tracing.PGE_DownstreamProtTrace";
                    itemDef.Group = false;
                    break;
                case 3:
                    itemDef.ID = "PGE.Desktop.EDER.ArcMapCommands.PGE_Tracing.PGE_UpstreamProtTrace";
                    itemDef.Group = false;
                    break;
                case 4:
                    itemDef.ID = "PGE.Desktop.EDER.ArcMapCommands.PGE_Toolbar.PGE_DisconnectMultipleFeature_Tool";
                    itemDef.Group = false;
                    break;
                case 5:
                    itemDef.ID = "Miner.Desktop.FeederManager.Commands.Connect";
                    itemDef.Group = false;
                    break;
                default:
                    itemDef.ID = "";
                    itemDef.Group = false;
                    break;
            }
        }

        public int ItemCount
        {
            get { return 4; }
        }

        public string Name
        {
            get { return "PGE Tracing"; }
        }
    }
}
