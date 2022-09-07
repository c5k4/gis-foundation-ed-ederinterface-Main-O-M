using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

using ESRI.ArcGIS.ADF.BaseClasses;

using Miner;
using Miner.ComCategories;

using PGE.Common.Delivery.UI.Tools;

namespace PGE.Desktop.EDER.UFM.Operations
{
    [ComponentCategory(ComCategory.ArcMapCommands)]
    [ProgId("Pge.UfmSelectTool")]
    [ComVisible(true)]
    [Guid("FDD8A6D9-DB46-495a-8820-08FB994E4618")]
    public class UfmSelectTool : BaseTool
    {
        #region Events

        public event EventHandler<EventArgs> Deactivated;
        public event EventHandler<MousePositionEventArgs> MouseDown;
        public event EventHandler<MousePositionEventArgs> MouseMove;
        public event EventHandler<EventArgs> RefreshEvent;

        #endregion

        #region Constructor

        public UfmSelectTool()
        {
            base.m_cursor = Cursors.Cross;
        }

        #endregion

        #region Com Reg Methods

        /// <summary>
        /// Registers the specified reg key.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComRegisterFunction]
        private static void Register(string regKey)
        {
            Miner.ComCategories.ArcMapCommands.Register(regKey);
        }
        /// <summary>
        /// Uns the register.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComUnregisterFunction]
        private static void UnRegister(string regKey)
        {
            Miner.ComCategories.ArcMapCommands.Unregister(regKey);
        }

        #endregion

        #region Event overrides

        public override void OnCreate(object hook)
        {
        }

        public override bool Deactivate()
        {
            if (this.MouseMove != null)
            {
                this.MouseMove = null;
            }
            if (this.MouseDown != null)
            {
                this.MouseDown = null;
            }
            if (this.Deactivated != null)
            {
                EventArgs e = new EventArgs();
                this.Deactivated(this, e);
                this.Deactivated = null;
            }
            return base.Deactivate();
        }

        public override void OnMouseDown(int button, int shift, int x, int y)
        {
            base.OnMouseDown(button, shift, x, y);
            if (this.MouseDown != null)
            {
                MousePositionEventArgs e = new MousePositionEventArgs(button, shift, x, y);
                this.MouseDown(this, e);
            }
        }

        public override void OnMouseMove(int button, int shift, int x, int y)
        {
            base.OnMouseMove(button, shift, x, y);
            if (this.MouseMove != null)
            {
                MousePositionEventArgs e = new MousePositionEventArgs(button, shift, x, y);
                this.MouseMove(this, e);
            }
        }

        public override void Refresh(int hdc)
        {
            base.Refresh(hdc);
            if (this.RefreshEvent != null)
            {
                EventArgs e = new EventArgs();
                this.RefreshEvent(this, e);
            }
        }

        #endregion
    }
}
