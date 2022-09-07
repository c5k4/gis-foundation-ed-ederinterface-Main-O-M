using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Management;
using System.Runtime.InteropServices;

namespace PGE.Interfaces.MapBooksPrintUI
{
    /// <summary>
    /// Import Methods from WinSpool to get/set default printers.
    /// </summary>
    public static class PrinterUtil
    {
        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetDefaultPrinter(string Name);
        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool GetDefaultPrinter(StringBuilder pszBuffer, ref int size);

        public static string DefaultPrinter()
        {
            StringBuilder dp = new StringBuilder(256);
            int size = dp.Capacity;
            if (GetDefaultPrinter(dp, ref size))
                return dp.ToString().Trim();

            return null;
        }
    }

    public class PDFMassPrinter
    {
        public static Process PDFParentProcess { get; set; }

        private Process _pdfPrintProcess;
        private Process PDFPrintProcess
        {
            get { return _pdfPrintProcess; }
            set
            {
                if (_pdfPrintProcess != null)
                    _pdfPrintProcess.Exited -= PDFPrintProcess_Exited;
                _pdfPrintProcess = value;
                _pdfPrintProcess.EnableRaisingEvents = true;
                if (_pdfPrintProcess != null)
                    _pdfPrintProcess.Exited += PDFPrintProcess_Exited;
            }
        }

        private List<string> _pdfFiles = null;
        private string _printerName;
        private string _originalDefaultPrinter;

        public PDFMassPrinter(List<string> pdfFiles, string printerName)
        {
            _pdfFiles = pdfFiles ?? new List<string>();
            _printerName = printerName;
        }

        void PDFPrintProcess_Exited(object sender, EventArgs e)
        {
            PrintFirstPDF();
        }

        public void PrintAllPDFs()
        {
            //The first PDF print will trigger all other prints when each process has ended.
            try
            {
                //Store current default printer, set the selected one.
                _originalDefaultPrinter = PrinterUtil.DefaultPrinter();
                PrinterUtil.SetDefaultPrinter(_printerName);

                //Initialize and start PDF parent process so that the Exited event works. (otherwise the first PDF process stays open)
                if (PDFParentProcess == null || PDFParentProcess.HasExited)
                {
                    PDFParentProcess = new Process();
                    PDFParentProcess.StartInfo = new ProcessStartInfo(Common.AdobeReaderExe);
                    PDFParentProcess.StartInfo.Arguments = "/h";
                    PDFParentProcess.StartInfo.UseShellExecute = false;
                    PDFParentProcess.StartInfo.RedirectStandardOutput = true;
                    PDFParentProcess.StartInfo.RedirectStandardError = true;

                    PDFParentProcess.Start();
                    System.Threading.Thread.Sleep(5000);
                }

                //Initialize process (this also initializes events)
                PDFPrintProcess = new Process();

                PrintFirstPDF();
            }
            catch
            {
            }
        }

        private void PrintFirstPDF()
        {
            try
            {
                if (_pdfFiles.Count == 0)
                {
                    //End of execution change for "print all".
                    PDFPrintProcess.Dispose();

                    System.Threading.Thread.Sleep(5000);

                    //Store original default printer.
                    PrinterUtil.SetDefaultPrinter(_originalDefaultPrinter);
                    return;
                }

                string currentPDF = _pdfFiles.First();
                _pdfFiles.Remove(currentPDF);

                /*
                 * Note: AcroRd32.exe /t [File] [Printer] "" "" works by allowing a printer to be specified,
                 * but it doesn't actually scale the page size to fit.
                 * AcroRd32.exe /p /h DOES scale the page size. Because of this, it's more efficient to set the
                 * default printer in Windows for the print job and use /p /h instead.
                 */

                //Define location of adobe reader/command line
                //switches to launch adobe in "print" mode
                ProcessStartInfo startInfo = new ProcessStartInfo(Common.AdobeReaderExe);
                startInfo.Arguments = "/p /h \"" + currentPDF + "\"";
                startInfo.UseShellExecute = false;
                startInfo.RedirectStandardOutput = true;
                startInfo.RedirectStandardError = true;

                PDFPrintProcess.StartInfo = startInfo;
                PDFPrintProcess.Start();
                PDFPrintProcess.CloseMainWindow();
            }
            catch
            {
                throw;
            }
        }
    }
}
