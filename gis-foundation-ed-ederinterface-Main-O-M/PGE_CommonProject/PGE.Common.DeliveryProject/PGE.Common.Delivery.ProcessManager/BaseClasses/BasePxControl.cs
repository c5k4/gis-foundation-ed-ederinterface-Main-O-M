using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using PGE.Common.Delivery.Diagnostics;
using Miner.Interop;
using Miner.Interop.Process;

namespace PGE.Common.Delivery.Process.BaseClasses
{
    /// <summary>
    /// Interface to be implemented by Form classes used as UIs for Px controls.
    /// </summary>
    public interface IPxControlUI
    {
        /// <summary>
        /// Loads the control.
        /// </summary>
        /// <param name="pxApp">The px app.</param>
        /// <param name="pxNode">The px node.</param>
        void LoadControl(IMMPxApplication pxApp, IMMPxNode pxNode);
        /// <summary>
        /// Gets or sets a value indicating whether there are pending updates.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if there are pending updates; otherwise, <c>false</c>.
        /// </value>
        bool PendingUpdates { get; set;}
        /// <summary>
        /// Applies the updates.
        /// </summary>
        void ApplyUpdates();
    }

    /// <summary>
    /// Base class for Process Framework node controls.
    /// </summary>
    public abstract class BasePxControl : IMMPxControl, IMMPxControl2, IMMPxDisplayName
    {
        /// <summary>
        /// The name of the control
        /// </summary>
        protected string _Name;
        /// <summary>
        /// The caption of the control
        /// </summary>
        protected string _Caption;
        /// <summary>
        /// The process framework application reference.
        /// </summary>
        protected IMMPxApplication _PxApp;
        /// <summary>
        /// The node that is associated with this control.
        /// </summary>
        protected IMMPxNode _Node;
        /// <summary>
        /// The underlying user form.
        /// </summary>
        protected Form _Host;
        /// <summary>
        /// If the control is enabled.
        /// </summary>
        protected bool _IsEnabled;
        /// <summary>
        /// If the state has completed.
        /// </summary>
        protected bool _IsStateComplete;

        #region Com Reg Methods
        /// <summary>
        /// Registers the specified reg key.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComRegisterFunction]
        static public void Register(string regKey)
        {

            Miner.ComCategories.MMProcessMgrControl.Register(regKey);
        }

        /// <summary>
        /// Unregisters the specified reg key.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComUnregisterFunction]
        static public void Unregister(string regKey)
        {
            Miner.ComCategories.MMProcessMgrControl.Unregister(regKey);
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="BasePxControl"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="uiForm">The UI form.</param>
        /// <param name="caption">The caption.</param>
        protected BasePxControl(string name, Form uiForm, string caption)
        {
            _Name = name;
            _Host = uiForm;
            _Caption = caption;
            _IsEnabled = true;
        }

        #endregion


        #region IMMPxControl Members

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="BasePxControl"/> is enabled.
        /// </summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        public bool Enabled
        {
            get
            {
                return _IsEnabled;
            }
            set
            {
                _IsEnabled = value;
            }
        }

        /// <summary>
        /// Initializes the specified v init data.
        /// </summary>
        /// <param name="vInitData">The v init data.</param>
        public void Initialize(object vInitData)
        {
            if (vInitData is IMMPxApplication)
                _PxApp = (IMMPxApplication) vInitData;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is initialized.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is initialized; otherwise, <c>false</c>.
        /// </value>
        public bool IsInitialized
        {
            get { return (_PxApp != null); }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return _Name; }
        }

        /// <summary>
        /// Sets the node.
        /// </summary>
        /// <value>The node.</value>
        public IMMPxNode Node
        {
            set 
            {
                try
                {
                    _Node = value;
                    IPxControlUI ui = (IPxControlUI)_Host;
                    ui.LoadControl(_PxApp, _Node);
                }
                catch (Exception e)
                {
                    EventLogger.Error(e, "Error Executing Px Control " + _Name);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether [state complete].
        /// </summary>
        /// <value><c>true</c> if [state complete]; otherwise, <c>false</c>.</value>
        public bool StateComplete
        {
            get { return _IsStateComplete; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="BasePxControl"/> is visible.
        /// </summary>
        /// <value><c>true</c> if visible; otherwise, <c>false</c>.</value>
        public bool Visible
        {
            get
            {
                if (_Host == null) return false;
                return _Host.Visible;
            }
            set
            {
            }
        }

        /// <summary>
        /// Gets the handle to the form.
        /// </summary>
        /// <value>The handle to the form..</value>
        public int hWnd
        {
            get 
            {
                if (_Host == null) return 0;
                return _Host.Handle.ToInt32();
            }
        }

        #endregion

        #region IMMPxControl2 Members

        /// <summary>
        /// Terminates the specified b terminate.
        /// </summary>
        /// <param name="bTerminate">if set to <c>true</c> [b terminate].</param>
        public void Terminate(ref bool bTerminate)
        {
            // Prompt user to apply any pending updates.
            IPxControlUI ui = (IPxControlUI)_Host;
            if (ui.PendingUpdates)
            {
                DialogResult result = MessageBox.Show("Do you want to apply the changes?",
                                                      _Caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                    ui.ApplyUpdates();
            }
        }

        #endregion

        #region IMMPxDisplayName Members

        /// <summary>
        /// Gets the display name.
        /// </summary>
        /// <value>The display name.</value>
        public string DisplayName
        {
            get {return _Name;}
        }

        #endregion

    }
}