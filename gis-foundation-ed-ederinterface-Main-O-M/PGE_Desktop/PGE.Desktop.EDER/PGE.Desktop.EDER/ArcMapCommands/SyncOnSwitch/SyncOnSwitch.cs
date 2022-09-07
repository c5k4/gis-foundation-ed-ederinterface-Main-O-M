using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Framework;
using Miner.ComCategories;
using PGE.Common.Delivery.Diagnostics;
using PGE.Common.Delivery.UI.Commands;

namespace PGE.Desktop.EDER.ArcMapCommands.SyncOnSwitch
{
    /// <summary>
    /// Summary description for SyncOnSwitch.
    /// </summary>
    [Guid("98481793-2768-4eef-bf38-becae6c884f2")]
    [ComponentCategory(ComCategory.ArcMapCommands)]
    [ProgId("PGE.Desktop.EDER.SyncOnSwitch")]
    [ComVisible(true)]
    public sealed class SyncOnSwitch : BaseArcGISCommand
    {
        #region COM Registration Function(s)
        [ComRegisterFunction()]
        [ComVisible(false)]
        static void RegisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryRegistration(registerType);

            //
            // TODO: Add any COM registration code here
            //
        }

        [ComUnregisterFunction()]
        [ComVisible(false)]
        static void UnregisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryUnregistration(registerType);

            //
            // TODO: Add any COM unregistration code here
            //
        }

        #region ArcGIS Component Category Registrar generated code
        /// <summary>
        /// Required method for ArcGIS Component Category registration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryRegistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            MxCommands.Register(regKey);

        }
        /// <summary>
        /// Required method for ArcGIS Component Category unregistration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryUnregistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            MxCommands.Unregister(regKey);

        }

        #endregion
        #endregion

        private IApplication _mApplication;
        private IActiveView _inFocusView;
        private Boolean _isActive;

        public SyncOnSwitch()
            : base(
                "PGE_SyncOnSwitch", "PGE Sync On Switch", "PGE Tools",
                "Synchronize the views between all data frames", "Synchronize view on data frame switch") 
        {
            try
            {
                string offBitmapResourceName = GetType().Name + "Off.bmp";
                Bitmap offBitmap = new Bitmap(GetType(), offBitmapResourceName);
                UpdateBitmap(offBitmap, 0, 0);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message, "Invalid Bitmap");
            }
        }

        #region Overridden Class Methods

        /// <summary>
        /// Occurs when this command is created
        /// </summary>
        /// <param name="hook">Instance of the application</param>
        public override void OnCreate(object hook)
        {
            if (hook == null)
                return;

            _mApplication = hook as IApplication;
            _isActive = false;

            //Disable if it is not ArcMap
            if (hook is IMxApplication)
                m_enabled = true;
            else
                m_enabled = false;
        }

        /// <summary>
        /// Occurs when this command is clicked
        /// </summary>
        public override void OnClick()
        {
            IDocumentEvents_Event docEvents = (IDocumentEvents_Event) (IMxDocument) _mApplication.Document;
            
            if (!_isActive)
            {
                docEvents.ActiveViewChanged += OnActiveViewChanged;
                _inFocusView = (IActiveView)((IMxDocument)_mApplication.Document).ActiveView.FocusMap;

                try
                {
                    UpdateBitmap(new Bitmap(GetType(), GetType().Name + ".bmp"));
                }
                catch (Exception e)
                {
                    Logger.WriteLine(TraceEventType.Error, "Unable to find 'on' bitmap for SyncOnSwitch tool.");
                }
            }
            else
            {
                docEvents.ActiveViewChanged -= OnActiveViewChanged;

                try
                {
                    UpdateBitmap(new Bitmap(GetType(), GetType().Name+"Off.bmp"));
                }
                catch (Exception e)
                {
                    Logger.WriteLine(TraceEventType.Error, "Unable to find 'off' bitmap for SyncOnSwitch tool.");
                }
            }

            _isActive = !_isActive;
        }

        #endregion

        /// <summary>
        /// FocusMapChanged Event handler
        /// </summary>
        /// <remarks></remarks>
        private void OnActiveViewChanged()
        {
            IActiveView newFocusView = (IActiveView)((IMxDocument)_mApplication.Document).ActiveView.FocusMap;
            newFocusView.Extent = _inFocusView.Extent;
            _inFocusView = newFocusView;
        }
    }
}
