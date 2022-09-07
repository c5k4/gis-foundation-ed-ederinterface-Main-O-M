using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.ADF.BaseClasses;
using System.Runtime.InteropServices;
using Miner.ComCategories;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;

namespace PGE.Desktop.EDER.ArcMapCommands.AnnotationComposite
{
    /// <summary>
    /// An ESRI tool that exists to place Windows Forms controls onto the CALM Toolbar.
    /// This tool utilizes the <see cref="UI.CALMToolbarUC"/> control.
    /// </summary>
    [Guid("C7CCEE73-F97A-4AC3-A1E6-9353A5903EEC")]
    [ComponentCategory(ComCategory.ArcMapCommands)]
    [ProgId("PGE.Desktop.EDER.CALMToolbarComposite")]
    [ComVisible(true)]
    public class CALMToolbarComposite : BaseTool, IToolControl, IDisposable
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

        #region Private Members
        private UI.CALMToolbarUC _replaceAnnoUC = null;

        private IApplication _application = null;
        private IMxDocument _mxDoc = null;
        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public CALMToolbarComposite()
        {
            base.m_category = "PGE Tools"; //localizable text 
            base.m_caption = "Replace Composite Annotation";  //localizable text 
            base.m_message = "Replace Composite Annotation";  //localizable text
            base.m_toolTip = "Replace Composite Annotation";  //localizable text
            base.m_name = "Replace Composite Annotation";   //unique id, non-localizable (e.g. "MyCategory_ArcMapTool")
        }

        #endregion constructor

        #region Overridden Class Methods

        /// <summary>
        /// Occurs when this tool is created
        /// </summary>
        /// <param name="hook">Instance of the application</param>
        public override void OnCreate(object hook)
        {
            _application = hook as IApplication;

            //Disable if it is not ArcMap
            if (hook is IMxApplication)
            {
                base.m_enabled = true;
                _mxDoc = _application.Document as IMxDocument;
            }
            else
                base.m_enabled = false;
        }

        #region Overriden ToolControl Methods

        public bool OnDrop(esriCmdBarType barType)
        {
            return true;
        }

        public void OnFocus(ICompletionNotify complete)
        {
        }

        /// <summary>
        /// Used to return the handle of the desired child control and set it as this tool's content.
        /// </summary>
        public int hWnd
        {
            get
            {
                //Instantiate the control if necessary, then grab its handle.
                if (_replaceAnnoUC == null)
                {
                    _replaceAnnoUC = new UI.CALMToolbarUC();
                    _replaceAnnoUC.CreateControl();
                }
                return _replaceAnnoUC.Handle.ToInt32();
            }
        }

        #endregion Overriden ToolControl Methods

        #endregion Overridden Class Methods

        #region IDisposable Methods

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_replaceAnnoUC != null)
                    _replaceAnnoUC.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}
