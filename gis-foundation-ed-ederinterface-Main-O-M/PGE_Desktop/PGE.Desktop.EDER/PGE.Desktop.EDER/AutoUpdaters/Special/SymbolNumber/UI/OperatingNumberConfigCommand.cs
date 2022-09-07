using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Windows.Forms;

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;

using Miner.Interop;
using Miner.ComCategories;

using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Framework;
using PGE.Common.Delivery.UI;
using PGE.Common.Delivery.UI.Commands;
using PGE.Common.Delivery.Diagnostics;

namespace PGE.Desktop.EDER.SymbolNumber.UI
{
    /// <summary>
    /// The SymbolNumberConfigCommand class launches the SymbolNumberConfigForm to allow users configure the Symbol Number Auto Updater.
    /// </summary>
    [Guid("39374838-9586-457D-BD43-12AA0AA7E4DB")]
    [ProgId("PGE.Desktop.EDER.OperatingNumberConfigCommand")]
    [ComponentCategory(ComCategory.ArcMapCommands)]
    [ComVisible(true)]
    public class OperatingNumberConfigCommand : BaseMxCommand, IDisposable
    {
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        private const string _err_no_login = "Before using this tool, please login to a configured ArcFM geodatabase.";

        /// <summary>
        /// Initializes a new instance of the <see cref="SymbolNumberConfigCommand"/> class.
        /// </summary>
        public OperatingNumberConfigCommand() : 
            base("Operating Number Config", "Operating Number Config", "Operating Number Config", "Edit and Validate Operating Number Rules.") 
        {
            base.m_category = "PGE Admin Tools";
        }

        /// <summary>
        /// The symbol number configuration form.  Allows the user to edit symbol number configuration documents.
        /// </summary>
        private SymbolNumberConfigForm form = null;

        /// <summary>
        /// Overridden Property to determine if command will be in enabled mode or not.
        /// </summary>
        public override bool Enabled
        {
            get
            {
                IWorkspace wkSpace = getWorkspace();
                if (wkSpace == null)
                {
                    m_enabled = false;
                    return m_enabled;
                }
                m_enabled = true;
                return m_enabled;
            }
        }

        /// <summary>
        /// Display the Symbol Number Config form. 
        /// </summary>
        protected override void InternalClick()
        {
            // Only display a new form if this is the first call of if the original form is disposed.  This allows 
            // the UI to retain it's current state if the user closes the form.
            IntPtr p = new IntPtr(_App.hWnd);
            IWorkspace wrkSpace = getWorkspace();
            if (wrkSpace == null)
            {
                MessageBox.Show(_err_no_login, "Operating Number Config", MessageBoxButtons.OK, MessageBoxIcon.Information);
                _logger.Warn("No workspace found.");
                return;
            }
            else
            {
                if (wrkSpace.Type != esriWorkspaceType.esriRemoteDatabaseWorkspace)
                {
                    MessageBox.Show(_err_no_login, "Operating Number Config", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _logger.Warn("Not an SDE/Remote database.");
                    return;
                }
            }

            //  Form is not initialized.  Create a new ofrm and display it.
            if (form == null) {
                form = new SymbolNumberConfigForm(wrkSpace, SchemaInfo.General.ClassModelNames.OperatingNumberRules);
                // Keep the form on top of the ArcMap window
                form.Show((Form)System.Windows.Forms.Form.FromHandle(p));

            } 
            else if (form.IsDisposed)
            {
                // Form is disposed release it and create a new one.
                form = null;
                form = new SymbolNumberConfigForm(wrkSpace, SchemaInfo.General.ClassModelNames.OperatingNumberRules);
                // Keep the form on top of the ArcMap window
                form.Show((Form)System.Windows.Forms.Form.FromHandle(p));

            }
            else if (!form.Visible)
            {
                // The form is hidden.  Show it.
                form.Show((Form)System.Windows.Forms.Form.FromHandle(p));
            }
            else {
                // Bring the form to the front 
                form.BringToFront();
            }
        }

        /// <summary>
        /// Gets the workspace containing the PGE_SYMBOL_NUMBER_RULES table.
        /// </summary>
        /// <returns></returns>
        private IWorkspace getWorkspace()
        {

            Type type = Type.GetTypeFromProgID("esriFramework.AppRef");
            object obj = Activator.CreateInstance(type);
            IApplication app = obj as IApplication;

            // get the workspace based on the ArcFM login object
            IMMLogin2 log2 = (IMMLogin2)app.FindExtensionByName("MMPROPERTIESEXT");
            return log2.LoginObject.LoginWorkspace;
            
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (form != null)
                    form.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
