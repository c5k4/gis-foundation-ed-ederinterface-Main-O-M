using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace PGE.Interfaces.MapBooksPrintUI
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
            //MapBookPrintUI ui = new MapBookPrintUI();
            MapBookPrintByCircuitTraceUI ui = new MapBookPrintByCircuitTraceUI();
            ui.FormClosing += new FormClosingEventHandler(ui_FormClosing);
            Application.Run(ui);
        }

        static void ui_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Kill the parent process necessary to run the print jobs properly.
            if (PDFMassPrinter.PDFParentProcess != null)
            {
                try
                {
                    if (!PDFMassPrinter.PDFParentProcess.HasExited) 
                        PDFMassPrinter.PDFParentProcess.Kill();
                }
                catch { }
            }
        }
    }
}
