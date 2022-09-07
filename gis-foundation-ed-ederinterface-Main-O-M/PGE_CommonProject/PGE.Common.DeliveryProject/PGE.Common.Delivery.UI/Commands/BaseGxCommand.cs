using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using ESRI.ArcGIS.CatalogUI;

using Miner.ComCategories;

namespace PGE.Common.Delivery.UI.Commands
{
    /// <summary>
    /// An abstract class used to create a buttons in ArcCatalog.
    /// </summary>
    public abstract class BaseGxCommand : BaseArcGISCommand
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseGxCommand"/> class.
        /// </summary>
        /// <param name="name">The name</param>
        /// <param name="caption">The caption</param>
        /// <param name="category"></param>
        /// <param name="message">The message</param>
        /// <param name="toolTip">The tool tip</param>
        public BaseGxCommand(string name, string caption, string category, string message, string toolTip)
            : base(name, caption, "PGE Tools", message, toolTip)
        {
        }

        #endregion

        #region Protected Properties

        /// <summary>
        /// Gets or sets the gx application.
        /// </summary>
        /// <value>The gx application.</value>
        protected IGxApplication GxApplication
        {
            get;
            set;
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
        public static void Register(String regKey)
        {
            ArcCatalogCommands.Register(regKey);
        }

        /// <summary>
        /// Unregisters the specified reg key.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComUnregisterFunction]
        public static void Unregister(String regKey)
        {
            ArcCatalogCommands.Unregister(regKey);
        }

        /// <summary>
        /// Called when the command is created inside the application.
        /// </summary>
        /// <param name="hook">A reference to the application in which the command was created.
        /// The hook may be an IApplication reference (for commands created in ArcGIS Desktop applications)
        /// or an IHookHelper reference (for commands created on an Engine ToolbarControl).</param>
        /// <remarks>Note to inheritors: classes inheriting from BaseCommand must always
        /// override the OnCreate method. Use this method to store a reference to the host
        /// application, passed in via the hook parameter.</remarks>
        public override void OnCreate(object hook)
        {
            base.OnCreate(hook);

            this.GxApplication = (IGxApplication)hook;
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