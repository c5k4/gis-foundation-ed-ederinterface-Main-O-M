using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Framework;

namespace PGE.Desktop.EDER
{
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
