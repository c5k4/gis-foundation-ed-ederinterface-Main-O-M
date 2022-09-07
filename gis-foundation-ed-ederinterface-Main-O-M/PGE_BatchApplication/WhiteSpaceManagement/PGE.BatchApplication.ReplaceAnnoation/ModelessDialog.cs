using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PGE.BatchApplication.ReplaceAnnoation
{
    //This class can be used in the Show method of a dialog in conjunction with the hwnd of the
    //IApplication.  This allows the form to always be on top, but only in the ArcMap window.
    public class ModelessDialog : System.Windows.Forms.IWin32Window
    {
        private System.IntPtr hwnd;

        public ModelessDialog(IntPtr handle)
        {
            hwnd = handle;
        }

        public ModelessDialog(int handle)
        {

            hwnd = new IntPtr(handle);
        }

        IntPtr System.Windows.Forms.IWin32Window.Handle
        {
            get { return hwnd; }
        }
    }
}
