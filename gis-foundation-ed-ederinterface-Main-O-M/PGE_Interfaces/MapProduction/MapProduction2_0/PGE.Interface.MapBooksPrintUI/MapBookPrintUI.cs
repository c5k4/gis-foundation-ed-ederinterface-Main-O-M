using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using PGE.Interfaces.MapBooksPrintUI.Controls;

namespace PGE.Interfaces.MapBooksPrintUI
{
    public partial class MapBookPrintUI : Form
    {
        #region Private Members
        private string[] _mapTypeDirectories;
        private Thread _gridLoadingThread = null;
        private bool _isChangingGrids = false;
        #endregion

        #region Properties
        /// <summary>
        /// Indicates whether or not the grid property is already being changed.
        /// </summary>
        public bool IsChangingGrids
        {
            get { return _isChangingGrids; }
            set { _isChangingGrids = value; gridLoading.BeginInvoke(new MethodInvoker(delegate { gridLoading.Visible = _isChangingGrids; })); }
        }
        #endregion

        #region Constructor
        public MapBookPrintUI()
        {
            //Read our configuration file
            ReadConfiguration();

            //Initialize map directory root folders.
            _mapTypeDirectories = Directory.GetDirectories(Common.PDFBaseDirectory, "*", SearchOption.TopDirectoryOnly);

            //Initialize Form
            InitializeComponent();

            //Initialize our drop down selection lists
            InitializeDropDowns();

            //Expand if necessary.
            if (Common.ExpandByDefault)
                btnAdvanced_Click(btnAdvanced, new EventArgs());

        }
        #endregion

        #region Private Startup Methods
        /// <summary>
        /// Reads the app.config file for any necessary configuration information
        /// </summary>
        private void ReadConfiguration()
        {
            Common.PDFBaseDirectory = ConfigurationManager.AppSettings[Common.KeyPDFBaseDirectory];
            Common.AdobeReaderExe = ConfigurationManager.AppSettings[Common.KeyAdobeReaderDirectory];
            Boolean.TryParse(ConfigurationManager.AppSettings[Common.KeyExpandByDefault], out Common.ExpandByDefault);
        }

        /// <summary>
        /// This method will initialize the drop down menus for non-dependent dropdowns
        /// based on the directory and file structure in the base directory
        /// </summary>
        private void InitializeDropDowns()
        {
            //Initialize MapType here - it won't need to be re-initialized so this makes things faster.
            //Otherwise we would merge this logic with the InitializeDropDown(FolderDependentComboBox) method.
            for (int i = 0; i < _mapTypeDirectories.Length; i++)
            {
                string folderName = _mapTypeDirectories[i].Substring(_mapTypeDirectories[i].LastIndexOf("\\") + 1, _mapTypeDirectories[i].Length - _mapTypeDirectories[i].LastIndexOf("\\") - 1);
                string[] info = Regex.Split(folderName, "-");

                if (!cboMapType.Items.Contains(info[0]))
                {
                    cboMapType.Items.Add(info[0]);
                }
            }

            //Add default selected values.
            cboPageSize.Items.Add("24x36");
            cboPageSize.SelectedIndex = 0;
            cboMapType.SelectedIndex = 0;
        }
        #endregion

        #region Private Control Event Helpers
        /// <summary>
        /// This method will initialize the drop down menus for a dependent dropdown
        /// by traversing the structure starting with the base directory and getting
        /// the relevant information about the current folder.
        /// </summary>
        /// <param name="cboBox">The dropdown to initialize.</param>
        private void InitializeDropDown(FolderDependentComboBox cboBox)
        {
            cboBox.Items.Clear();
            if (cboBox.NullOption)
                cboBox.Items.Add(string.Empty);

            for (int i = 0; i < _mapTypeDirectories.Length; i++)
            {
                string folderName = _mapTypeDirectories[i].Substring(_mapTypeDirectories[i].LastIndexOf("\\") + 1, _mapTypeDirectories[i].Length - _mapTypeDirectories[i].LastIndexOf("\\") - 1);
                string[] info = Regex.Split(folderName, "-");

                if (!cboMapType.HasSelectedValue)
                    return;

                if (info.Length >= 2)
                {
                    if (info[0] == cboMapType.ToString())
                    {
                        if (cboBox.Name == cboScale.Name)
                        {
                            //Second value is scale
                            KeyValuePair<string, string> scale = new KeyValuePair<string, string>(info[1], "1:" + info[1]);
                            if (!cboBox.Items.Contains(scale))
                                cboBox.Items.Add(scale);

                            continue;
                        }

                        if (!cboScale.HasSelectedValue)
                            return;

                        if (info[1] == cboScale.ToString())
                        {
                            if (cboBox.Name == cboRegion.Name)
                            {
                                //Add all of the regions in this folder to the drop down
                                ParseDirectory(_mapTypeDirectories[i], cboBox);
                                continue;
                            }

                            if (!cboRegion.HasSelectedValue)
                                return;

                            string[] RegionDirectories = Directory.GetDirectories(_mapTypeDirectories[i], "*", SearchOption.TopDirectoryOnly);
                            for (int j = 0; j < RegionDirectories.Length; j++)
                            {
                                string regionFolderName = RegionDirectories[j].Substring(RegionDirectories[j].LastIndexOf("\\") + 1, RegionDirectories[j].Length - RegionDirectories[j].LastIndexOf("\\") - 1);
                                if (regionFolderName == cboRegion.ToString())
                                {
                                    if (cboBox.Name == cboDivision.Name)
                                    {
                                        //Add all of the divisions in this directory to the drop down
                                        ParseDirectory(RegionDirectories[j], cboBox);
                                        continue;
                                    }

                                    if (!cboDivision.HasSelectedValue)
                                        return;

                                    if (cboBox.Name == cboDistrict.Name)
                                    {
                                        string[] DivisionDirectories = Directory.GetDirectories(RegionDirectories[j], "*", SearchOption.TopDirectoryOnly);
                                        for (int k = 0; k < DivisionDirectories.Length; k++)
                                        {
                                            string divisionFolderName = DivisionDirectories[k].Substring(DivisionDirectories[k].LastIndexOf("\\") + 1, DivisionDirectories[k].Length - DivisionDirectories[k].LastIndexOf("\\") - 1);
                                            if (divisionFolderName == cboDivision.ToString())
                                            {
                                                //Add all of the Districts to its drop down
                                                ParseDirectory(DivisionDirectories[k], cboDistrict);
                                            }
                                        }

                                        continue;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Loads the grid dropdown values. Performs the process in another thread.
        /// Restarts the thread if things have changed, but not if it's multiple times in the same event chain.
        /// </summary>
        private void GetGrids()
        {
            if (IsChangingGrids && _gridLoadingThread != null && _gridLoadingThread.IsAlive)
            {
                _gridLoadingThread.Abort();
                IsChangingGrids = false;
            }

            //Start the grid loading in another thread so that things can keep going while this is occurring.
            if (Handle != null)
            {
                IsChangingGrids = true;
                string ddMapTypeValue = cboMapType.ToString();
                string ddScaleValue = cboScale.ToString();
                string selectedDirectory = GetSelectedDirectory();
                _gridLoadingThread = new Thread(() => PopulateGridDropDown(ddMapTypeValue, ddScaleValue, selectedDirectory));
                _gridLoadingThread.Start();
            }
        }

        /// <summary>
        /// Thread-safe load of Grid dropdown values. Sets a value to false upon completion to hide the progress bar
        /// and allow more grid loading if necessary.
        /// </summary>
        /// <param name="ddMapTypeValue">The value of the Map Type dropdown.</param>
        /// <param name="ddScaleValue">The value of the Scale dropdown.</param>
        /// <param name="directory">The current directory based on selected dropdown values.</param>
        private void PopulateGridDropDown(string ddMapTypeValue, string ddScaleValue, string directory)
        {
            MethodInvoker inv = delegate() { cboGridNumber.Enabled = false;  cboGridNumber.ClearItems(); };
            cboGridNumber.BeginInvoke(inv);

            if (string.IsNullOrEmpty(ddMapTypeValue) || string.IsNullOrEmpty(ddScaleValue))
            {
                IsChangingGrids = false;
                return;
            }

            List<string> toAdd = new List<string>();

            string ext = ".PDF";
            string prefix = ddMapTypeValue + "_";
            foreach (string fileName in Directory.GetFiles(directory, "*.PDF", SearchOption.AllDirectories))
            {
                string file = fileName.Substring(fileName.LastIndexOf("\\") + 1, fileName.Length - fileName.LastIndexOf("\\") - 1);
                if (file.Length > ext.Length + prefix.Length)
                {
                    string filteredFileName = file.Substring(prefix.Length);

                    toAdd.Add(filteredFileName.Substring(0, filteredFileName.IndexOf('_')));
                }
            }

            inv = delegate() {
                cboGridNumber.BeginUpdate();
                toAdd.Sort();
                foreach (string s in toAdd) { cboGridNumber.AddItem(s); }
                cboGridNumber.EndUpdate();
                cboGridNumber.Enabled = true;
            };
            cboGridNumber.BeginInvoke(inv);

            IsChangingGrids = false;
        }

        /// <summary>
        /// Updates the multi-select indicator (showing in the combobox how many items are selected).
        /// This is required in any event that may change whether or not multiple items can be checked
        /// and in any event that may change the list of available items.
        /// </summary>
        /// <param name="modifier">
        ///     The modifier to add to the list of selected items (can be negative).
        ///     Used when an item is currently being checked or unchecked.
        /// </param>
        private void UpdateMultiSelectIndicator(int modifier)
        {
            if (cboGridNumber.Items.Count <= 0 || cboGridNumber.ChecksUpdating)
                return;

            int selectedAmount = cboGridNumber.GetSelectedItemList().Count + modifier;

            if (selectedAmount > 0 && cboGridNumber.UseMultiOption)
            {
                string newValue = "[" + selectedAmount + " selected]";

                if (!cboGridNumber.Items[0].ToString().Contains("selected]"))
                    cboGridNumber.Items.Insert(0, newValue);
                else
                    cboGridNumber.Items[0] = newValue;

                cboGridNumber.SelectedIndex = 0;
            }
            else
            {
                if (cboGridNumber.Items[0].ToString().Contains("selected]"))
                    cboGridNumber.Items.RemoveAt(0);
            }
        }
        #endregion

        #region Private File System Methods
        /// <summary>
        /// Gets the current directory based on selected values.
        /// </summary>
        /// <returns>The current PDF directory based on selected values.</returns>
        private string GetSelectedDirectory()
        {
            string dir = Common.PDFBaseDirectory;

            if (!cboMapType.HasSelectedValue || !cboScale.HasSelectedValue)
                return dir;

            dir = Path.Combine(dir, cboMapType.ToString() + "-" + cboScale.ToString());

            if (!cboRegion.HasSelectedValue)
                return dir;

            dir = Path.Combine(dir, cboRegion.ToString());

            if (!cboDivision.HasSelectedValue)
                return dir;

            dir = Path.Combine(dir, cboDivision.ToString());

            if (!cboDistrict.HasSelectedValue)
                return dir;

            dir = Path.Combine(dir, cboDistrict.ToString());

            return dir;
        }

        /// <summary>
        /// This will add all the unique folder names in a directory to the specified combo box
        /// </summary>
        /// <param name="directory">Directory to parse for other directory names</param>
        /// <param name="cbo">ComboBox to add the found directories to its list</param>
        private void ParseDirectory(string directory, ComboBox cbo)
        {
            string[] Directories = Directory.GetDirectories(directory);
            for (int j = 0; j < Directories.Length; j++)
            {
                string folderName = Directories[j].Substring(Directories[j].LastIndexOf("\\") + 1, Directories[j].Length - Directories[j].LastIndexOf("\\") - 1);
                //Add the name to the drop down
                if (!cbo.Items.Contains(folderName))
                {
                    cbo.Items.Add(folderName);
                }
            }
        }
        #endregion

        #region Private PDF Printing Methods
        public Boolean PrintPDF(string pdfFileName, PrinterSettings printSettings)
        {
            try
            {
                Process proc = new Process();
                proc.StartInfo.Verb = "print";

                /*
                //Get Printer Settings, use ManagementObjects to get port and driver.
                string printerName = printSettings.PrinterName;
                string printerPort = "", printerDriver = "";
                string query = string.Format("SELECT * from Win32_Printer WHERE Name LIKE '%{0}'", printerName);
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
                ManagementObjectCollection coll = searcher.Get();

                foreach (ManagementObject printer in coll)
                {
                    printerDriver = printer.Properties["DriverName"].Value.ToString();
                    printerPort = printer.Properties["PortName"].Value.ToString();
                }
                 */

                //Define location of adobe reader/command line
                //switches to launch adobe in "print" mode
                ProcessStartInfo startInfo = new ProcessStartInfo(Common.AdobeReaderExe);
                startInfo.Arguments = String.Format("/s /n /p \"" + pdfFileName + "\"");
                //startInfo.Arguments = String.Format("/h /t \"" + pdfFileName + "\" \"" + printerName + "\" \"" + printerDriver + "\" \"" + printerPort + "\"");
                startInfo.UseShellExecute = false;
                startInfo.RedirectStandardOutput = true;
                startInfo.RedirectStandardError = true;

                proc.StartInfo.FileName = Common.AdobeReaderExe;
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.CreateNoWindow = true;
                proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

                proc.Start();
                
                

                //proc.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region All Form Control Events
        private void btnPrintPDFs_Click(object sender, EventArgs e)
        {
            //Get list of selected grids based on the type of selection (single or multiple)
            List<string> selectedGrids = new List<string>();
            if (cboGridNumber.UseMultiOption)
            {
                selectedGrids = cboGridNumber.GetSelectedItemList();
            }
            else
            {
                if (cboGridNumber.SelectedItem != null && cboGridNumber.SelectedItem.ToString() != string.Empty)
                    selectedGrids.Add(cboGridNumber.SelectedItem.ToString());
            }

            if (selectedGrids.Count == 0)
            {
                MessageBox.Show("Please select a grid to export.", Common.PromptWindowTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            List<string> printers = new List<string>();
            foreach (string printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
                printers.Add(printer);

            SelectPrinter selectPrinter = new SelectPrinter(printers);
            selectPrinter.ShowDialog();

            if (!selectPrinter.Cancelled)
            {
                string printerName = selectPrinter.SelectedPrinter;
                string directory = GetSelectedDirectory();

                Dictionary<string, string> selectedFiles = new Dictionary<string, string>();
                Directory.GetFiles(directory, "*", SearchOption.AllDirectories).ToList().ForEach(d =>
                {
                    string file = d.Substring(d.LastIndexOf("\\") + 1, d.Length - d.LastIndexOf("\\") - 1);
                    selectedGrids.ForEach(g => {
                        if (file.ToUpper().Contains("_" + g.ToUpper() + "_") && !selectedFiles.ContainsKey(g))
                            selectedFiles.Add(g, d);
                    });
                });

                PDFMassPrinter pdfPrinter = new PDFMassPrinter(selectedFiles.Values.ToList(), printerName);
                pdfPrinter.PrintAllPDFs();
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAdvanced_Click(object sender, EventArgs e)
        {
            //Expand or collapse window size.
            if (Height < Common.WindowFullHeight)
            {
                while (Height < Common.WindowFullHeight)
                    Height += Common.WindowResizeIncrement;
                btnAdvanced.Text = "Advanced ▴";

            }
            else
            {
                if (cboRegion.SelectedItem == null || string.IsNullOrEmpty(cboRegion.SelectedItem.ToString())
                    || MessageBox.Show("Would you like to clear the current filter?", Common.PromptWindowTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    while (Height > Common.WindowCondensedHeight)
                        Height -= Common.WindowResizeIncrement;
                    btnAdvanced.Text = "Advanced ▾";
                    cboRegion.SelectedItem = "";
                }
            }
        }

        public void DefaultDropDown_IndexChangeCompleted(object sender, EventArgs e)
        {
            GetGrids();

            UpdateMultiSelectIndicator(0);
        }

        public void DefaultDropDown_Initializing(FolderDependentComboBox comboBox)
        {
            InitializeDropDown(comboBox);

            UpdateMultiSelectIndicator(0);
        }

        private void cbPrintMultiple_CheckedChanged(object sender, EventArgs e)
        {
            cboGridNumber.UseMultiOption = cbPrintMultiple.Checked;

            UpdateMultiSelectIndicator(0);
        }

        private void cboGridNumber_MultiItemChecked(object sender, ItemCheckEventArgs e)
        {
            int modifier = 0;

            if ((cboGridNumber.DropDownControl as CheckedListBox).Items[e.Index].ToString() != Common.SelectAllText)
            {
                if (e.NewValue == CheckState.Checked)
                    modifier++;
                else
                    modifier--;
            }

            UpdateMultiSelectIndicator(modifier);
        }

        private void cboGridNumber_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboGridNumber.Items.Count <= 0 || cboGridNumber.ChecksUpdating || cboGridNumber.SelectedIndex < 0)
                return;

            if (cboGridNumber.Items[0].ToString().Contains("selected]") && cboGridNumber.SelectedIndex == 0)
                return;

            if (cboGridNumber.UseMultiOption)
            {
                //Instead of updating the grid, open the multi-select dialog
                cboGridNumber.SelectedItem = null;
                UpdateMultiSelectIndicator(0);
                cboGridNumber.ShowPopup();
            }
        }
        #endregion
    }
}
