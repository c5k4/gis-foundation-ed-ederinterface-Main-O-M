using System;
using System.Collections.Generic;
using System.Linq;
using Telvent.Delivery.Framework;
using System.Windows.Forms;

namespace Telvent.PGE.MapProduction.SwizzleLayers
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new SwizzleLayersUI());
        }
    }
}
