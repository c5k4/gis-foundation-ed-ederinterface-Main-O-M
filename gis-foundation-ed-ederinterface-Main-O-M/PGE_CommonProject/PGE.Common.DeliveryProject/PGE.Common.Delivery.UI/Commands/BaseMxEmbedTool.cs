using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;

using Miner.ComCategories;
using PGE.Common.Delivery.UI.Commands;


namespace PGE.Common.Delivery.UI.Tools
{
    /// <summary>
    /// A tool that can be used when map interaction is required from a Windows Form.
    /// It is not intended to be used as tool from any ArcMap command bars.
    /// </summary>
    [ComVisible(true)]
    [Guid("391D6453-3A3B-4AAD-A87B-FB3847551CAF")]
    [ProgId("PGE.Common.Delivery.UI.Tools.MxEmbedTool")]
    [ComponentCategory(ComCategory.ArcMapCommands)]
    public sealed class BaseMxEmbedTool : BaseMxTool
    {
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<MousePositionEventArgs> MouseDown;
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<MousePositionEventArgs> MouseMove;
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<KeyEventArgs> KeyDown;
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<KeyEventArgs> KeyUp;
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<EventArgs> Deactivated;
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<EventArgs> RefreshEvent;

        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        public BaseMxEmbedTool()
            : base("MxEmbedTool", "Mx Tool in Form", "SPPG Tools", "", "")
        {

        }
        #endregion
        /// <summary>
        /// Sets the cursor for the map tool.
        /// If this is not set then default cursor from base is used.
        /// </summary>
        public Cursor SetCursor
        {
            set
            {
                base._Cursor = value;
            }
        }

        #region Overrides
        /// <summary>
        /// Mouse Down on map event 
        /// </summary>
        /// <param name="button"></param>
        /// <param name="shift"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public override void OnMouseDown(int button, int shift, int x, int y)
        {
            base.OnMouseDown(button, shift, x, y);
            if (MouseDown != null)
            {
                MousePositionEventArgs args = new MousePositionEventArgs(button, shift, x, y);
                MouseDown(this, args);
            }
        }
        /// <summary>
        /// Mouse move in map event
        /// </summary>
        /// <param name="button"></param>
        /// <param name="shift"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public override void OnMouseMove(int button, int shift, int x, int y)
        {
            base.OnMouseMove(button, shift, x, y);
            if (MouseMove != null)
            {
                MousePositionEventArgs args = new MousePositionEventArgs(button, shift, x, y);
                MouseMove(this, args);
            }
        }
        /// <summary>
        /// Keydown on map event
        /// </summary>
        /// <param name="keyCode"></param>
        /// <param name="shift"></param>
        public override void OnKeyDown(int keyCode, int shift)
        {
            base.OnKeyDown(keyCode, shift);
            if (KeyDown != null)
            {
                KeyEventArgs args = new KeyEventArgs(keyCode, shift);
                KeyDown(this, args);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyCode"></param>
        /// <param name="shift"></param>
        public override void OnKeyUp(int keyCode, int shift)
        {
            base.OnKeyUp(keyCode, shift);
            if (KeyUp != null)
            {
                KeyEventArgs args = new KeyEventArgs(keyCode, shift);
                KeyUp(this, args);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hdc"></param>
        public override void Refresh(int hdc)
        {
            base.Refresh(hdc);
            if (RefreshEvent != null)
            {
                EventArgs args = new EventArgs();
                RefreshEvent(this, args);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool Deactivate()
        {
            if (MouseMove != null) MouseMove = null;
            if (MouseDown != null) MouseDown = null;
            if (Deactivated != null)
            {
                EventArgs args = new EventArgs();
                Deactivated(this, args);
                Deactivated = null;
            }
            return base.Deactivate();
        }
        #endregion

    }
    /// <summary>
    /// 
    /// </summary>
    public sealed class MousePositionEventArgs : EventArgs
    {
        private int _x;
        private int _y;
        private int _button;
        private int _shift;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="button"></param>
        /// <param name="shift"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public MousePositionEventArgs(int button, int shift, int x, int y)
        {
            _x = x;
            _y = y;
            _button = button;
            _shift = shift;
        }

        /// <summary>
        /// 
        /// </summary>
        public int X { get { return _x; } }
        /// <summary>
        /// 
        /// </summary>
        public int Y { get { return _y; } }
        /// <summary>
        /// 
        /// </summary>
        public int Button { get { return _button; } }
        /// <summary>
        /// 
        /// </summary>
        public int Shift { get { return _shift; } }
    }
    /// <summary>
    /// 
    /// </summary>
    public sealed class KeyEventArgs : EventArgs
    {
        private int _KC;
        private int _SH;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyCode"></param>
        /// <param name="shiftKey"></param>
        public KeyEventArgs(int keyCode, int shiftKey)
        {
            _KC = keyCode;
            _SH = shiftKey;
        }
        /// <summary>
        /// 
        /// </summary>
        public int KeyCode { get { return _KC; } }
        /// <summary>
        /// 
        /// </summary>
        public int Shift { get { return _SH; } }

    }



}
