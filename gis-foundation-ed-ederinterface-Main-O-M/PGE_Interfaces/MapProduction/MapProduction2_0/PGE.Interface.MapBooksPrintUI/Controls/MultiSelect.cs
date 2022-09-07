using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PGE.Interfaces.MapBooksPrintUI.Controls
{
    public class MultiSelect : ComboBox
    {
        private Control _ddControl;
        private ToolStripDropDown _toolStripDD;
        private Panel _hostPanel; // workarround - some controls don't display correctly if they are hosted directly in ToolStripControlHost
        int _fullDropDownHeight;
        private bool _initialized = false;
        private bool _useMulti = false;

        public Control DropDownControl
        {
            get { return _ddControl; }
            set
            {
                _ddControl = value;
                InitializeControl();
            }
        }

        public bool UseMultiOption
        {
            get { return _useMulti; }
            set
            {
                _useMulti = value;
                if (_useMulti)
                    this.SelectedItem = null;
            }
        }

        public MultiSelect() : base()
        {
        }

        private void InitializeControl()
        {
            if (_initialized)
                return;

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

            _ddControl.Parent = null;
            _ddControl.CausesValidation = false;
            _ddControl.Resize += MControlResize;

            _hostPanel.Controls.Add(_ddControl);

            _toolStripDD.Padding = Padding.Empty;
            _toolStripDD.Margin = Padding.Empty;

            _toolStripDD.MinimumSize = _toolStripDD.MaximumSize = _toolStripDD.Size = _ddControl.Size;

            _toolStripDD.Items.Add(new ToolStripControlHost(_hostPanel));

            this.DropDown += new EventHandler(MultiSelect_DropDown);
            _ddControl.KeyDown += new KeyEventHandler(SubControl_KeyDown);

            ClearItems();

            _initialized = true;
        }

        void SubControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Escape)
            {
                Close();
            }
        }

        void MultiSelect_DropDown(object sender, EventArgs e)
        {
            if (_fullDropDownHeight <= 0)
                _fullDropDownHeight = DropDownHeight;

            if (_useMulti)
            {
                DropDownHeight = 1;
                ShowPopup();
            }
            else
            {
                DropDownHeight = _fullDropDownHeight;
            }
        }

        public void AddItem(object value)
        {
            if (this.Items.Contains(value))
                return;

            this.Items.Add(value);
            if (_initialized && _ddControl is ListBox)
                (_ddControl as ListBox).Items.Add(value);
        }

        public void ClearItems()
        {
            base.Items.Clear();
            if (_initialized && _ddControl is ListBox)
            {
                (_ddControl as ListBox).Items.Clear();
                (_ddControl as ListBox).Items.Add(Common.SelectAllText);
            }
        }

        private void ResizeWindow()
        {
            if (!_initialized) return;

            _toolStripDD.MinimumSize = _toolStripDD.MaximumSize = _toolStripDD.Size = _ddControl.Size;
            _hostPanel.MinimumSize = _hostPanel.MaximumSize = _hostPanel.Size = _ddControl.Size;
        }

        private void MControlResize(object sender, EventArgs e)
        {
            if (!_initialized) return;

            ResizeWindow();
        }

        /// <summary>
        /// Display the popup and keep the focus
        /// </summary>
        /// <param name="parentControl"></param>
        public void ShowPopup()
        {
            if (!_initialized || !_useMulti) return;

            // position the popup window
            var loc = this.PointToScreen(new Point(0, this.Height));
            _toolStripDD.Show(loc);
            BeginInvoke(new MethodInvoker(delegate { _ddControl.Focus(); }));
        }

        public void Close()
        {
            if (!_initialized) return;

            _toolStripDD.Close();
        }

        public new void Dispose()
        {
            base.Dispose();

            if (!_initialized) return;

            _ddControl.Resize -= MControlResize;

            _toolStripDD.Dispose();
            _hostPanel.Dispose();
        }
    }
}
