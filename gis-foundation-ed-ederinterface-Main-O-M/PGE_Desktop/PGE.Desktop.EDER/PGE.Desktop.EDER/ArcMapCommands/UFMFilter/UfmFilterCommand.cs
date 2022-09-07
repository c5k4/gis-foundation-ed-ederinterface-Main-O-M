using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.SystemUI;

using Miner.ComCategories;
using Miner.Interop;

using PGE.Common.Delivery.UI.Commands;
using PGE.Desktop.EDER.ArcMapCommands;
using PGE.Desktop.EDER.ArcMapCommands.UFMFilter.UC;

namespace PGE.Desktop.EDER.ArcMapCommands.UFMFilter
{
    /// <summary>
    /// ToolControl implementation
    /// </summary>
    [Guid("2BFC28FE-0DBF-4768-9AFF-6D4377297F64")]
    [ComponentCategory(ComCategory.ArcMapCommands)]
    [ProgId("PGE.Desktop.EDER.UfmFilterCommand")]
    [ComVisible(true)]
    public class UfmFilterCommand : BaseArcGISCommand, IToolControl, IDisposable
    {       
        #region Private Members

        private UfmFilter _ufmFilterUC = null;
        private ICompletionNotify _complete = null;
        //private IMap _map = null;
        private IApplication _app = null;
        
        #endregion

        #region constructor

        public UfmFilterCommand()
            : base("PGETools_UfmFilterCommand", "PG&E Vault Filter", "PGE", "PG&E Vault Filter", "PG&E Vault Filter")
        {
            base.m_name = "PGETools_UfmFilterCommand";
        }
        
        #endregion constructor

        /// <summary>
        /// Occurs when this command is created
        /// </summary>
        /// <param name="hook">Instance of the application</param>
        public override void OnCreate(object hook)
        {
            if (hook == null)
            {
                return;
            }

            // Store the application and the map
            _app = hook as IApplication;
            //IMxDocument mxDocument = _application.Document as IMxDocument;
            //IActiveView activeView = mxDocument.FocusMap as IActiveView;
            //_map = mxDocument.FocusMap as IMap;
        }

        #region Overriden ToolControl Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="barType"></param>
        /// <returns></returns>
        public bool OnDrop(esriCmdBarType barType)
        {
            return true;
        }

        /// <summary>
        /// Unused.
        /// </summary>
        /// <param name="complete"></param>
        public void OnFocus(ICompletionNotify complete)
        {
            _complete = complete;
        }

        public int hWnd
        {
            get {

                if (null == _ufmFilterUC)
                {
                    // Create a new filter control
                    _ufmFilterUC = new UfmFilter();
                    _ufmFilterUC.CreateControl();

                    // Get a list of all vaults currently in the map
                    _ufmFilterUC.Configure(_app);
                }

                // Return the control handle
                return _ufmFilterUC.Handle.ToInt32();
            }
        }

        #endregion Overriden ToolControl Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_ufmFilterUC != null)
                    _ufmFilterUC.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
