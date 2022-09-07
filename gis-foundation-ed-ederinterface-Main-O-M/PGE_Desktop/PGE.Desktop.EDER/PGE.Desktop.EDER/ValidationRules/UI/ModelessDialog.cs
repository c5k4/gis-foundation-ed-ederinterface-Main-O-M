using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PGE.Desktop.EDER.ValidationRules.UI
{
    #region "ModelessDialog"
    public class ModelessDialog : System.Windows.Forms.IWin32Window
    {
        private System.IntPtr hwnd;

        /// <summary> 
        /// Returns the form handle. 
        /// </summary> 
        /// <value></value> 
        /// <returns></returns> 
        /// <remarks></remarks> 
        public System.IntPtr Handle
        {
            get { return hwnd; }
        }

        /// <summary> 
        /// Constructor. 
        /// </summary> 
        /// <param name="handle">The form handle passed as an IntPtr</param> 
        /// <remarks></remarks> 
        public ModelessDialog(System.IntPtr handle)
        {
            hwnd = handle;
        }

        /// <summary> 
        /// Constructor. 
        /// </summary> 
        /// <param name="handle">The form handle passed as an integer</param> 
        /// <remarks></remarks> 
        public ModelessDialog(int handle)
        {
            hwnd = (IntPtr)handle;
        }
    }
    #endregion
}
