using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ESRI.ArcGIS.Geodatabase;
using PGE.Common.Delivery.Geodatabase.ChangeDetection;
using ESRI.ArcGIS.GeoDatabaseDistributed;
using System.Linq;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;


namespace PGE.BatchApplication.UDC_FAI_Tools
{
    public class CGC12Updater : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public CGC12Updater()
        {
        }

        protected override void OnClick()
        {
            CGC12UpdaterForm cgc12UpdateForm = new CGC12UpdaterForm();
            cgc12UpdateForm.ShowDialog(new ArcMapWindow(ArcMap.Application));
        }
        protected override void OnUpdate()
        {
            Enabled = ArcMap.Application != null;
        }

        private void GetUpdatedCircuits()
        {

        }

        
    }

    public class ArcMapWindow : System.Windows.Forms.IWin32Window
    {
        private IApplication m_app;

        public ArcMapWindow(IApplication application)
        {
            m_app = application;
        }
        /// <summary>
        /// Handle
        /// </summary>
        public System.IntPtr Handle
        {
            get { return new IntPtr(m_app.hWnd); }
        }
    }
}
