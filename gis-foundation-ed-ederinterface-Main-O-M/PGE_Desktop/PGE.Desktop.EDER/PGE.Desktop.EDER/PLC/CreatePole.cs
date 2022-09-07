using System;
using System.Drawing;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Geodatabase;
using Miner.Geodatabase.Edit;
using PGE.Common.Delivery.UI.Commands;
using Miner.ComCategories;
using Miner.Interop;
using PGE.Common.Delivery.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;

namespace PGE.Desktop.EDER.PLC
{
    /// <summary>
    /// Command that works in ArcMap/Map/PageLayout
    /// </summary>
    [Guid("1d6cffe2-f230-4919-9ee4-4453c84c236f")]
    //[ClassInterface(ClassInterfaceType.None)]
    [ComponentCategory(ComCategory.ArcMapCommands)]
    [ProgId("PGE.Desktop.EDER.PLC.CreatePole")]
    [ComVisible(true)]

    public sealed class CreatePole : BaseArcGISCommand
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

        public CreatePole() :
            base("PGEPLCTools_CreatePoleCommand", "PLC New Pole", "SPoT Tools", "Place Pole (PLDB)", "Place Pole (PLDB)")
        {

            base.m_name = "PGEPLCTools_CreatePoleCommand";
            try
            {
                Bitmap bmp = null;
                string path = GetType().Assembly.GetName().Name + ".PLC." + GetType().Name + ".bmp";
                ////Get bitmap image
                bmp = new Bitmap(GetType().Assembly.GetManifestResourceStream(path));
                ////Assign bitmap image
                UpdateBitmap(bmp, 0, 0);
            }
            catch (Exception ex)
            {
                //_logger.Warn("Invalid Bitmap" + ex.Message);
                System.Diagnostics.Trace.WriteLine(ex.Message, "Invalid Bitmap");
            }
        }

        private IHookHelper m_hookHelper;
        private IApplication _application;
       
        public FrmCreatePole frm;
        
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");


        #region Overridden Class Methods

        /// <summary>
        /// Occurs when this command is created
        /// </summary>
        /// <param name="hook">Instance of the application</param>
        public override void OnCreate(object hook)
        {
            if (hook == null)
                return;

            try
            {
                
                m_hookHelper = new HookHelperClass();
                m_hookHelper.Hook = hook;
                if (m_hookHelper.ActiveView == null)
                    m_hookHelper = null;
            }
            catch
            {
                m_hookHelper = null;
            }

            if (m_hookHelper == null)
                base.m_enabled = false;
            else
                base.m_enabled = true;

            _application = hook as IApplication;

        }

        /// <summary>
        /// Occurs when this command is clicked
        /// </summary>
        protected override void InternalClick()
        {

            try
            {
                

                string jobNumberFromMemory = PGE.Desktop.EDER.ArcMapCommands.PopulateLastJobNumberUC.jobNumber;
                //if the jobnumber is null or empty then show message else assign the value to current obj.
                if (string.IsNullOrEmpty(jobNumberFromMemory) || jobNumberFromMemory.Trim().Length < 1)
                {
                    //System.Windows.Forms.MessageBox.Show("Please provide a Job Number within the PG&E Job Number toolbar and press Enter before creating assets.");

                    throw new COMException(
                        "Please provide a Job Number within the PG&E Job Number toolbar and press Enter before creating assets.",
                        (int)mmErrorCodes.MM_E_CANCELEDIT);
                }
                //// if ((Regex.IsMatch(_jobNumberFromMemory, @"^[a-zA-Z]+$") || Regex.IsMatch(_jobNumberFromMemory, @"[^a-zA-Z0-9]")))
                //int n;
                //bool isNumeric = int.TryParse(jobNumberFromMemory, out n);
                //if (isNumeric == true)
                //{
                //    throw new COMException(
                //        "Please provide a valid Job Number within the PG&E Job Number toolbar and press Enter before creating assets.",
                //        (int)mmErrorCodes.MM_E_CANCELEDIT);
                //}
                int rowCnt = -1;
                if (frm == null || frm.IsDisposed)
                {

                    frm = new FrmCreatePole(_application,(IWorkspaceEdit)(Editor.EditWorkspace), jobNumberFromMemory, out rowCnt);

                    if (rowCnt > 0)
                    {
                        frm.Show();
                    }
                    else
                    {
                        frm = null;
                    }
                }
                else
                {
                    frm.BringToFront();
                }
            }
            catch(Exception ex)
            {
                _logger.Error("PG&E PLC Create pole , Erro in Internal CLick function", ex);
                System.Windows.Forms.MessageBox.Show(ex.Message.ToString(),"PG&E PLC Create Pole",System.Windows.Forms.MessageBoxButtons.OK,System.Windows.Forms.MessageBoxIcon.Error);
            }
        }
        public override bool Enabled
        {
            get
            {
                if (((IWorkspaceEdit)(Editor.EditWorkspace)) == null)
                {
                    base.m_enabled = false;
                }
                else
                {
                    if (((IWorkspaceEdit)(Editor.EditWorkspace)).IsBeingEdited())
                        base.m_enabled = true;
                    else
                        base.m_enabled = false;
                }

                return base.m_enabled;
            }
        }

        #endregion
    }
}
