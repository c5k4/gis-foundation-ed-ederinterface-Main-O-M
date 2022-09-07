using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using ESRI.ArcGIS.Framework;

using Miner.ComCategories;

namespace PGE.Common.Delivery.UI.Commands
{
    /// <summary>
    /// An abstract class used to create a buttons in ArcMap.
    /// </summary>
    public abstract class BaseMxCommand : BaseArcGISCommand
    {
        #region Fields

        /// <summary>
        /// The application reference.
        /// </summary>
        protected IApplication _App;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseMxCommand"/> class.
        /// </summary>
        /// <param name="name">The name</param>
        /// <param name="caption">The caption</param>
        /// <param name="message">The message</param>
        /// <param name="toolTip">The tool tip</param>
        protected BaseMxCommand(string name, string caption, string message, string toolTip)
            : base(name, caption, "PGE Tools", message, toolTip)
        {
        }

        #endregion

        #region Protected Properties

        /// <summary>
        /// Gets the application reference
        /// </summary>
        /// <value>The app ref.</value>
        protected IApplication Application
        {
            get
            {
                return _App;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the windows form is closed.
        /// </summary>
        /// <value><c>true</c> if the windows form is closed; otherwise, <c>false</c>.</value>
        protected bool WindowsFormClosed
        {
            get
            {
                return !m_checked;
            }
            set
            {
                m_checked = !value;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Registers the specified reg key.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComRegisterFunction]
        public static void Register(string regKey)
        {
            ArcMapCommands.Register(regKey);
        }

        /// <summary>
        /// Unregisters the specified reg key.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComUnregisterFunction]
        public static void Unregister(String regKey)
        {
            ArcMapCommands.Unregister(regKey);
        }

        /// <summary>
        /// Called when the command is created inside the application.
        /// </summary>
        /// <param name="hook">A reference to the application in which the command was created.
        /// The hook may be an IApplication reference (for commands created in ArcGIS Desktop applications)
        /// or an IHookHelper reference (for commands created on an Engine ToolbarControl).</param>
        public override void OnCreate(object hook)
        {
            if (hook is IApplication)
                _App = (IApplication)hook;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Manages the lifetime of the windows form.
        /// </summary>
        /// <param name="windowsForm">The windows form.</param>
        protected void ManageFormClosed(Form windowsForm)
        {
            if (windowsForm == null)
                return;

            windowsForm.FormClosed += new FormClosedEventHandler(OnFormClosed);
            WindowsFormClosed = false;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Called when the form is closed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.FormClosedEventArgs"/> instance containing the event data.</param>
        void OnFormClosed(object sender, FormClosedEventArgs e)
        {
            WindowsFormClosed = true;
            m_checked = false;
        }

        #endregion
    }
}