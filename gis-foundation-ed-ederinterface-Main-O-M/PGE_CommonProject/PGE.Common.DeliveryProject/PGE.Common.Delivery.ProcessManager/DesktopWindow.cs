using System.Windows.Forms;
using System.Runtime.InteropServices;
using System;

namespace PGE.Common.Delivery.Process
{
    /// <summary>
    /// This class is necessary so that when the login form is shown it doesn't
    /// immediately disappear.
    /// </summary>
    /// 
	public class DesktopWindow : IWin32Window
	{
		private static DesktopWindow _window = new DesktopWindow();
		private DesktopWindow(){}
        /// <summary>
        /// 
        /// </summary>
		public static IWin32Window Instance 
		{ 
			get { return _window; } 
		}

		[DllImport("user32.dll")]
		private static extern IntPtr GetDesktopWindow();

		IntPtr IWin32Window.Handle
		{
			get 
			{ 
				return GetDesktopWindow();
			}
		}
	}
}