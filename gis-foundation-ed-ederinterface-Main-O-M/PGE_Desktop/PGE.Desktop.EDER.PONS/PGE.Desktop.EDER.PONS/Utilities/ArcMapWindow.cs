////////////////////////////////////////////////////////////////////////
//	Purpose	        : for setting arcmap as parent to other forms
//	Date Created	:June 25, 2014
//	Author		    : Tata Consultancy Services
//////////////////////////////////////////////////////////////////////
using System;
using ESRI.ArcGIS.Framework;

namespace PGE.Desktop.EDER.ArcMapCommands.PONS
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
