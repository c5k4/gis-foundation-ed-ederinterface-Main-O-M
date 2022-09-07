using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Reflection;
using System.Collections.Generic;
using System.Drawing;
using System.Collections;

using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;

using Miner.ComCategories;
using Miner.Interop;

using PGE.Common.Delivery.UI.Commands;
using PGE.Common.Delivery.Framework;
using PGE.Common.Delivery.Diagnostics;
using PGE.Common.Delivery.ArcFM;
using PGE.Desktop.EDER.AutoTextElements;
using PGE.Desktop.EDER.ValidationRules.UI;
using PGE.Desktop.EDER.ValidationRules; 

namespace PGE.Desktop.EDER.ArcMapCommands
{
    /// <summary>
    /// PGE Custom Print Command that sets print the Grid with buffer hash
    /// </summary>
    [Guid("D0B13E4F-6AA1-4681-B8B1-2748C20601FD")]
    [ComponentCategory(ComCategory.ArcMapCommands)]
    [ProgId("PGE.Desktop.EDER.ArcMapCommands.PGERunQAQCCommand")]
    [ComVisible(true)]
    public sealed class PGERunQAQCCommand : BaseArcGISCommand
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


        #endregion Private Variables

        #region Constructor
        /// <summary>
        /// Creates an instance of <see cref="PGERunQAQCCommand"/>
        /// </summary>
        public PGERunQAQCCommand() :
            base("PGETools_PGERunQAQCCommand", "PGE Run QAQC", "PGE Tools", "PGE Run QAQC", "PGE Run QAQC")
        {
            base.m_name = "PGETools_PGERunQAQCCommand";
            try
            {
                //
                // TODO: change bitmap name if necessary
                //
                Bitmap bmp = null;
                //Get path for bitmap image 
                string path = GetType().Assembly.GetName().Name + ".ArcMapCommands.MapUtilities." + GetType().Name + ".bmp";
                _logger.Debug("Bitmap image path" + path);
                //Get bitmap image
                bmp = new Bitmap(GetType().Assembly.GetManifestResourceStream(path));
                //Assign bitmap image
                UpdateBitmap(bmp, 0, 0);
            }
            catch (Exception ex)
            {
                _logger.Warn("Invalid Bitmap" + ex.Message);
                System.Diagnostics.Trace.WriteLine(ex.Message, "Invalid Bitmap");
            }
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
        protected override void InternalClick()
        {
            try
            {
                ValidationEngine.Instance.Application = _application;
                ValidationEngine.Instance.OpenRunQAQCForm();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);

                const string errorText = "Error loading Severities - validation severity model name not found";
                MessageBox.Show(errorText);
            }
        }

        #endregion Overridden Class Methods
    }
}
