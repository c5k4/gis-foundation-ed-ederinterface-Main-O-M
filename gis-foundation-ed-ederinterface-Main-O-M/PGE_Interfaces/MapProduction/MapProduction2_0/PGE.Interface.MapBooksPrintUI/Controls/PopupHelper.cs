using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace PGE.Interfaces.MapBooksPrintUI.Controls
{
    /// <summary>
    /// PopupHelper
    /// </summary>
    public sealed class PopupHelper : IDisposable
    {
        private readonly Control _control;
        private readonly ToolStripDropDown _toolStripDD;
        private readonly Panel _hostPanel; // workarround - some controls don't display correctly if they are hosted directly in ToolStripControlHost

        public PopupHelper(Control control)
        {
            _hostPanel = new Panel();
            _hostPanel.Padding = Padding.Empty;
            _hostPanel.Margin = Padding.Empty;
            _hostPanel.TabStop = false;
            _hostPanel.BorderStyle = BorderStyle.None;
            _hostPanel.BackColor = Color.Transparent;

            _toolStripDD = new ToolStripDropDown();
            _toolStripDD.CausesValidation = false;

            _toolStripDD.Padding = Padding.Empty;
            _toolStripDD.Margin = Padding.Empty;
            _toolStripDD.Opacity = 0.9;

            _control = control;
            _control.CausesValidation = false;
            _control.Resize += MControlResize;

            _hostPanel.Controls.Add(_control);

            _toolStripDD.Padding = Padding.Empty;
            _toolStripDD.Margin = Padding.Empty;

            _toolStripDD.MinimumSize = _toolStripDD.MaximumSize = _toolStripDD.Size = control.Size;

            _toolStripDD.Items.Add(new ToolStripControlHost(_hostPanel));
        }

        private void ResizeWindow()
        {
            _toolStripDD.MinimumSize = _toolStripDD.MaximumSize = _toolStripDD.Size = _control.Size;
            _hostPanel.MinimumSize = _hostPanel.MaximumSize = _hostPanel.Size = _control.Size;
        }

        private void MControlResize(object sender, EventArgs e)
        {
            ResizeWindow();
        }

        /// <summary>
        /// Display the popup and keep the focus
        /// </summary>
        /// <param name="parentControl"></param>
        public void Show(Control parentControl)
        {
            if (parentControl == null) return;

            // position the popup window
            var loc = parentControl.PointToScreen(new Point(0, parentControl.Height));
            _toolStripDD.Show(loc);
            _control.Focus();
        }

        public void Close()
        {
            _toolStripDD.Close();
        }

        public void Dispose()
        {
            _control.Resize -= MControlResize;

            _toolStripDD.Dispose();
            _hostPanel.Dispose();
        }
    }
}