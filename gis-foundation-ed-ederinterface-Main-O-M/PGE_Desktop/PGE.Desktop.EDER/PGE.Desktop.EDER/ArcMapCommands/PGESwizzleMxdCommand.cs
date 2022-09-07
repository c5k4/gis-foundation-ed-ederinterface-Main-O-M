using System;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF.CATIDs;
using PGE.Common.Delivery.UI.Commands;
using Miner.ComCategories;
using System.Windows.Forms;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using PGE.Common.Delivery.Framework;
using PGE.Common.Delivery.Diagnostics;
using System.Reflection;
using System.Collections.Generic;
using Miner.Interop;
using PGE.Common.Delivery.ArcFM;

namespace PGE.Desktop.EDER.ArcMapCommands
{
    /// <summary>
    /// PGE Custom Print Command that sets print the Grid with buffer hash
    /// </summary>
    [Guid("25EC0176-6918-4F09-9A11-0E90FB33D390")]
    [ComponentCategory(ComCategory.ArcMapCommands)]
    [ProgId("PGE.Desktop.EDER.ArcMapCommands.PGESwizzleMxdCommand")]
    [ComVisible(true)]
    public sealed class PGESwizzleMxdCommand : BaseArcGISCommand
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

        #region Private Varibales
        /// <summary>
        /// Currently running ArcMap instance
        /// </summary>
        private IApplication _application = null;
        /// <summary>
        /// Logger to log error/debug messages
        /// </summary>
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        #endregion Private Varibales

        #region Constructor
        /// <summary>
        /// Creates an instance of <see cref="PGEPrintMapCommand"/>
        /// </summary>
        public PGESwizzleMxdCommand() :
            base("PGE_Swizzle_Mxd_Command", "PGE Swizzle Map Document", "PGE Admin Tools", "Repoint Layers to different Database", "Swizzle Map Documents")
        {
                    
        }
        #endregion Constructor

        #region Overridden Class Methods

        /// <summary>
        /// Occurs when this command is created
        /// </summary>
        /// <param name="hook">Instance of the application</param>
        public override void OnCreate(object hook)
        {
            if (hook == null) return;
            _application = hook as IApplication;
        }

        /// <summary>
        /// Occurs when this command is clicked
        /// </summary>
        public override void OnClick()
        {
            try
            {
                SwizzleLayersUI ui=new SwizzleLayersUI(_application);
                ui.Show();
                //ui.Close();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        #endregion Overridden Class Methods
    }
}
