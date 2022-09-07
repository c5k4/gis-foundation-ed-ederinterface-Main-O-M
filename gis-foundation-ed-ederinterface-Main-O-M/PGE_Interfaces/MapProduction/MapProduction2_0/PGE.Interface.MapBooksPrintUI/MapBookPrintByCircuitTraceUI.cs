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
using System.Runtime.InteropServices;
using PGE.Interfaces.MapBooksPrintUI.Controls;

using ESRI.ArcGIS;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.NetworkAnalysis;

using Miner.Interop;

using PGE.Common.Delivery.Framework;
using PGE.Common.Delivery.Systems.Configuration;
using LicenseManager = PGE.Common.Delivery.Framework.LicenseManager;

namespace PGE.Interfaces.MapBooksPrintUI
{
    public partial class MapBookPrintByCircuitTraceUI : Form
    {
        #region Private Members
        private string[] _mapTypeDirectories;
        private Thread _gridLoadingThread = null;
        private bool _isChangingGrids = false;
        private const string _CAPTION = "Print Options - Standard Map";
        private const int _WindowFullHeight = 720;
        private readonly int _grpMapPrintHeight;
        private readonly int _grpNetworkLeft;
        private readonly int _grpNetworkTop;
        private readonly int _grpFilterLeft;
        private readonly int _grpFilterTop;
        private readonly int _btnPrintPDFsTop;
        private readonly int _btnCloseTop;

        private LicenseManager _licenceManager;
        private IWorkspace _workspace;
        private Dictionary<string, IGeometricNetwork> _geomNetworks;
        private ITable _circuitSourceTable;
        private bool _isTraced = false;
        private List<string> _tracedMapByCircuitList = null;
        List<string> _mapGridList = null;
        IAsyncResult _asyncInvokeResult = null;

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
        public MapBookPrintByCircuitTraceUI()
        {
            //Read our configuration file
            ReadConfiguration();

            //Initialize map directory root folders.
            _mapTypeDirectories = Directory.GetDirectories(Common.PDFBaseDirectory, "*", SearchOption.TopDirectoryOnly);

            //Initialize Form
            InitializeComponent();

            //Initialize our drop down selection lists
            InitializeDropDowns();

            this._grpNetworkTop = grpNetwork.Top;
            this._grpFilterTop = grpFilter.Top;
            this._btnPrintPDFsTop = btnPrintPDFs.Top;
            this._btnCloseTop = btnClose.Top;

            //btnPrintPDFs.Top = grpFilter.Top;
            //btnClose.Top = grpFilter.Top;

            grpNetwork.Visible = false;
            grpFilter.Visible = false;

            foreach (Control ctl in grpFilter.Controls)
            {
                ctl.Visible = false;
            }

            this.Size = new Size(this.Width, _WindowFullHeight - grpNetwork.Height - grpFilter.Height);

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
            if ((IsChangingGrids && _gridLoadingThread != null && _gridLoadingThread.IsAlive))
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
            MethodInvoker inv = delegate() { cboGridNumber.Enabled = false; cboGridNumber.ClearItems(); };
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

            _mapGridList = toAdd;

            inv = delegate()
            {
                cboGridNumber.BeginUpdate();
                toAdd.Sort();
                foreach (string s in toAdd) { cboGridNumber.AddItem(s); }
                cboGridNumber.EndUpdate();
                cboGridNumber.Enabled = true;
            };

            _asyncInvokeResult = cboGridNumber.BeginInvoke(inv);

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

            dir = System.IO.Path.Combine(dir, cboMapType.ToString() + "-" + cboScale.ToString());

            if (!cboRegion.HasSelectedValue)
                return dir;

            dir = System.IO.Path.Combine(dir, cboRegion.ToString());

            if (!cboDivision.HasSelectedValue)
                return dir;

            dir = System.IO.Path.Combine(dir, cboDivision.ToString());

            if (!cboDistrict.HasSelectedValue)
                return dir;

            dir = System.IO.Path.Combine(dir, cboDistrict.ToString());

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


        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAdvanced_Click(object sender, EventArgs e)
        {
            if (grpFilter.Visible == true)
            {
                grpFilter.Visible = false;
                btnAdvanced.Text = "&Advanced ▾";

                foreach (Control ctl in grpFilter.Controls)
                {
                    ctl.Visible = false;
                }

                if (grpNetwork.Visible)
                {
                    grpNetwork.Top = grpFilter.Top;
                    this.Size = new Size(this.Width, _WindowFullHeight - grpFilter.Height);
                }
                else
                {
                    this.Size = new Size(this.Width, _WindowFullHeight - grpFilter.Height);
                    //this.Height = _WindowFullHeight - grpFilter.Height;
                }
            }
            else
            {
                grpFilter.Visible = true;
                btnAdvanced.Text = "&Advanced ▴";

                foreach (Control ctl in grpFilter.Controls)
                {
                    ctl.Visible = true;
                }
            }

            if (!grpNetwork.Visible && grpFilter.Visible)
            {
                this.Size = new Size(this.Width, _WindowFullHeight - grpNetwork.Height);
                this.Height = _WindowFullHeight - grpNetwork.Height;
            }
            else if (grpNetwork.Visible && grpFilter.Visible)
            {
                grpNetwork.Top = _grpNetworkTop;
                this.Size = new Size(this.Width, _WindowFullHeight);
                this.CenterToScreen();
            }

            this.StartPosition = FormStartPosition.CenterScreen;

            //Expand or collapse window size.
            //if (Height < Common.WindowFullHeight)
            //{
            //    while (Height < Common.WindowFullHeight)
            //        Height += Common.WindowResizeIncrement;
            //    btnAdvanced.Text = "Advanced ▴";

            //}
            //else
            //{
            //    if (cboRegion.SelectedItem == null || string.IsNullOrEmpty(cboRegion.SelectedItem.ToString())
            //        || MessageBox.Show("Would you like to clear the current filter?", Common.PromptWindowTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            //    {
            //        while (Height > Common.WindowCondensedHeight)
            //            Height -= Common.WindowResizeIncrement;
            //        btnAdvanced.Text = "Advanced ▾";
            //        cboRegion.SelectedItem = "";
            //    }
            //}
        }

        public void DefaultDropDown_IndexChangeCompleted(object sender, EventArgs e)
        {
            _isTraced = false;
            lblTraceStatus.Text = "";

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

            if (_isTraced && _tracedMapByCircuitList.Count > 0)
            {
                SelectMapByCircuit(_tracedMapByCircuitList);
            }

            UpdateMultiSelectIndicator(0);
        }
        #endregion

        private void MapBookPrintByCircuitTraceUI_Load(object sender, EventArgs e)
        {
            string initializeMessage;
            string message;

            //UserRegistry userReg = new UserRegistry(Common.MapBookDBConnectionRegKey);

            _licenceManager = new LicenseManager();

            //Check ESRI license
            if (CheckLicense(out initializeMessage))
            {
                if (string.IsNullOrEmpty(initializeMessage))
                {
                    DBConnectionDialogBox dlg = new DBConnectionDialogBox();
                    //Connect to SDE
                    while (true)
                    {
                        if (GetConnection(out message)) 
                        {
                            this.Text = _CAPTION + string.Format("   ( {0})", message);
                            break; 
                        }
                        else if (dlg.IsCancelled)
                        {
                            this.Close();
                            break;
                        }

                        if (!string.IsNullOrEmpty(message)) dlg.lblMessage.Text = message;
                        dlg.ShowDialog();
                    }
                }
                else
                {
                    MessageBox.Show(initializeMessage);
                }
            }

            lblTraceStatus.Text = "";

        }

        private void MapBookPrintByCircuitTraceUI_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_workspace != null)
            {
                while (Marshal.ReleaseComObject(_workspace) > 0) ;
                _workspace = null;
            }
            _licenceManager.Dispose();
        }

        private void btnTrace_Click(object sender, EventArgs e)
        {
            string message = string.Empty;
            object feederID = null;

            if ((cboScale.SelectedIndex == -1) || (string.IsNullOrEmpty(cboScale.SelectedItem.ToString())))
            {
                message = "Please select map scale.";
            }
            else
            {
                System.Windows.Forms.Cursor.Current = Cursors.WaitCursor;

                if ((lstCircuitList.SelectedIndices.Count > 0) && (lstCircuitList.Items[lstCircuitList.SelectedIndices[0]].Checked))
                {

                    if (!_isTraced) //populate cboGridNumber if anything has been changed (circuit id/ sub-station id etc)
                    {
                        if ((_mapGridList != null) && _mapGridList.Count > 0)
                        {
                            cboGridNumber.Items.Clear();
                            foreach (string s in _mapGridList) { cboGridNumber.AddItem(s); }
                        }
                    }
                    //Clear the traced list
                    _tracedMapByCircuitList.Clear();

                    try
                    {
                        feederID = lstCircuitList.SelectedItems[0].SubItems[1].Text;
                    }
                    catch
                    {
                        message = "Please select a circuit.";
                        MessageBox.Show(message);
                        return;
                    }
                    string selectedGeoNetwork = cboNetwork.SelectedItem.ToString();
                    IGeometricNetwork geomNetwork = _geomNetworks[selectedGeoNetwork];

                    IEnumNetEID junctions;
                    IEnumNetEID edges;

                    message = TraceDownstream(geomNetwork, feederID, out junctions, out edges);

                    //MessageBox.Show(message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    if (_isTraced)
                    {
                        lblTraceStatus.Text = "Tracing Status: successful";
                        lblTraceStatus.ForeColor = Color.Green;
                    }

                }
                else
                {
                    message = "Please select a circuit.";
                }
            }

            if (!string.IsNullOrEmpty(message))
            {
                MessageBox.Show(message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            System.Windows.Forms.Cursor.Current = Cursors.Default;
        }

        private bool CheckLicense(out string message)
        {
            bool isOk = false;
            message = string.Empty;

            try
            {
                isOk = _licenceManager.Initialize(esriLicenseProductCode.esriLicenseProductCodeAdvanced, mmLicensedProductCode.mmLPArcFM);
            }
            catch
            {
                message = "Error initializing ArcGIS Desktop license";
            }

            return isOk;
        }

        private void PopulateNetworkDropDown()
        {
            List<string> networkNameList;
            _geomNetworks = Common.GetNetworks(_workspace, out networkNameList, "MMElectricTraceWeight");
            networkNameList.Sort();

            string[] networkNames = networkNameList.ToArray<string>();
            for (int index = 0; index <= networkNames.Length - 1; index++)
            {
                cboNetwork.Items.Add(networkNames[index]);
            }

            cboNetwork.SelectedIndex = 0;

        }

        private void PopulateSubstationDropDown()
        {
            cboSubStation.Sorted = false;
            cboSubStation.Items.Clear();
            List<string> cboItems = new List<string>();
            cboSubStation.Items.Add("");
            cboSubStation.Items.Add("All");
            string subStaionIDFiledName = (ModelNameFacade.FieldFromModelName((IObjectClass)_circuitSourceTable, Common.FieldModelNames.SubstationID)).Name;
            IQueryFilter queryFilter = new QueryFilterClass();
            queryFilter.SubFields = string.Format("DISTINCT {0}", subStaionIDFiledName);
            queryFilter.WhereClause = string.Format("{0} IS NOT NULL OR {0}!='' ", subStaionIDFiledName);

            IQueryFilterDefinition queryFilterDef = (IQueryFilterDefinition)queryFilter;
            queryFilterDef.PostfixClause = string.Format("ORDER BY {0}", subStaionIDFiledName);

            ICursor circuitSourceCursor = null;
            try
            {
                circuitSourceCursor = _circuitSourceTable.Search(queryFilter, true);
                int subStationIDIndex = ModelNameFacade.FieldIndexFromModelName((IObjectClass)_circuitSourceTable, Common.FieldModelNames.SubstationID);
                IRow row;
                while ((row = circuitSourceCursor.NextRow()) != null)
                {
                    cboItems.Add(Convert.ToString(row.get_Value(subStationIDIndex)));
                }


                cboItems.Sort();
                cboSubStation.Items.AddRange(cboItems.ToArray());
                cboSubStation.SelectedIndex = -1;
            }
            finally
            {

                if (circuitSourceCursor != null) { while (Marshal.ReleaseComObject(circuitSourceCursor) > 0);}
                if (queryFilter != null) { while (Marshal.ReleaseComObject(queryFilter) > 0);}
            }
        }

        private void PopulateCircuitList(IGeometricNetwork geomNetwork)
        {
            IMMFeederExt feederExt = null;
            IMMFeederSpace feederSpace = null;
            IMMFeederSource feederSource = null;

            Type type = Type.GetTypeFromProgID("mmFramework.MMFeederExt");
            object obj = Activator.CreateInstance(type);

            string subStaionIDFiledName = (ModelNameFacade.FieldFromModelName((IObjectClass)_circuitSourceTable, Common.FieldModelNames.SubstationID)).Name;
            string circuitIDFieldName = (ModelNameFacade.FieldFromModelName((IObjectClass)_circuitSourceTable, Common.FieldModelNames.FeederID)).Name;
            string circuitNameFieldName = (ModelNameFacade.FieldFromModelName((IObjectClass)_circuitSourceTable, Common.FieldModelNames.FeederName)).Name;

            IQueryFilter queryFilter = new QueryFilterClass();
            queryFilter.SubFields = string.Format("{0}, {1}, {2}", subStaionIDFiledName, circuitIDFieldName, circuitNameFieldName);
            if (cboSubStation.SelectedIndex > 1)
            {
                queryFilter.WhereClause = string.Format("SUBSTATIONID = '{0}'", cboSubStation.SelectedItem.ToString());
            }
            IQueryFilterDefinition queryFilterDef = (IQueryFilterDefinition)queryFilter;
            queryFilterDef.PostfixClause = string.Format("ORDER BY {0}, {1}", subStaionIDFiledName, circuitIDFieldName);

            ICursor circuitSourceCursor = null;
            try
            {

                circuitSourceCursor = _circuitSourceTable.Search(queryFilter, true);

                int subStationIDIndex = ModelNameFacade.FieldIndexFromModelName((IObjectClass)_circuitSourceTable, Common.FieldModelNames.SubstationID);
                int circuitIDIndex = ModelNameFacade.FieldIndexFromModelName((IObjectClass)_circuitSourceTable, Common.FieldModelNames.FeederID);
                int circuitNameIndex = ModelNameFacade.FieldIndexFromModelName((IObjectClass)_circuitSourceTable, Common.FieldModelNames.FeederName);

                IRow row;
                while ((row = circuitSourceCursor.NextRow()) != null)
                {
                    object circuitID = Convert.ToString(row.get_Value(circuitIDIndex));

                    feederExt = obj as IMMFeederExt;
                    feederSpace = feederExt.get_FeederSpace(geomNetwork, false);

                    if (feederSpace != null)
                    {
                        //Get the feeder source
                        feederSource = feederSpace.FeederSources.get_FeederSourceByFeederID(circuitID);
                        if (feederSource != null)
                        {
                            ListViewItem item = new ListViewItem();
                            item.SubItems[0].Text = Convert.ToString(row.get_Value(subStationIDIndex));
                            item.SubItems.Add(Convert.ToString(row.get_Value(circuitIDIndex)));
                            item.SubItems.Add(Convert.ToString(row.get_Value(circuitNameIndex)));

                            lstCircuitList.Items.Add(item);
                        }

                    }
                }

                lstCircuitList.Refresh();
            }
            finally
            {

                if (circuitSourceCursor != null) { while (Marshal.ReleaseComObject(circuitSourceCursor) > 0);}
                if (queryFilter != null) { while (Marshal.ReleaseComObject(queryFilter) > 0);}
            }
        }

        private string TraceDownstream(IGeometricNetwork geomNetwork, object feederID, out IEnumNetEID junctions, out IEnumNetEID edges)
        {
            string message = string.Empty;
            IMMFeederExt feederExt = null;
            IMMFeederSpace feederSpace = null;
            IMMFeederSource feederSource = null;
            IMMElectricNetworkTracing downstreamTracer = null;
            IMMNetworkAnalysisExtForFramework networkAnalysisExtForFramework = null;
            IMMElectricTraceSettingsEx electricTraceSettings = null;

            Type type = Type.GetTypeFromProgID("mmFramework.MMFeederExt");
            object obj = Activator.CreateInstance(type);

            junctions = null;
            edges = null;

            try
            {
                feederExt = obj as IMMFeederExt;
                feederSpace = feederExt.get_FeederSpace(geomNetwork, false);
                if (feederSpace == null)
                {
                    message = "Incorrect network. Please select network properly.";
                    _isTraced = false;
                    return message;
                }

                //Get the feeder source
                feederSource = feederSpace.FeederSources.get_FeederSourceByFeederID(feederID);

                if (feederSource == null)
                {
                    message = "Feeder source not found. Tracing is not successful.";
                    _isTraced = false;
                    return message;
                }

                downstreamTracer = new MMFeederTracerClass();

                networkAnalysisExtForFramework = new MMNetworkAnalysisExtForFrameworkClass();
                networkAnalysisExtForFramework.DrawComplex = true;
                networkAnalysisExtForFramework.SelectionSemantics = mmESRIAnalysisOptions.mmESRIAnalysisOnAllFeatures;

                electricTraceSettings = new MMElectricTraceSettingsClass();
                electricTraceSettings.RespectESRIBarriers = true;
                electricTraceSettings.UseFeederManagerCircuitSources = true;
                electricTraceSettings.UseFeederManagerProtectiveDevices = true;

                //start EID
                int startEID = feederSource.JunctionEID;

                //Phases to trace.
                mmPhasesToTrace phasesToTrace = mmPhasesToTrace.mmPTT_Any;

                //Trace
                downstreamTracer.TraceDownstream(geomNetwork, networkAnalysisExtForFramework, electricTraceSettings, startEID,
                                                    esriElementType.esriETJunction, phasesToTrace, out junctions, out edges);
                message = "The circuit has been successfully traced.";

                //Now serch for the MapGrids
                IGeometry geo = ConvertTracedEIDsToGeometry(geomNetwork, junctions, edges);
                IFeatureClass mapGridFeature = ModelNameFacade.FeatureClassByModelName(_workspace, Common.ClassModelNames.MapPrint);
                ISpatialFilter spatialFilter = new SpatialFilterClass();
                spatialFilter.Geometry = geo;
                spatialFilter.GeometryField = mapGridFeature.ShapeFieldName;
                spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;

                IFeatureCursor featureCursor = mapGridFeature.Search(spatialFilter, true);
                IFeature feature = null;
                List<String> result = new List<string>();
                while ((feature = featureCursor.NextFeature()) != null)
                {
                    int gridIdx = ModelNameFacade.FieldIndexFromModelName(mapGridFeature, Common.FieldModelNames.MapGridName);
                    string gridNameValue = Convert.ToString(feature.get_Value(gridIdx));
                    result.Add(gridNameValue);

                }

                _tracedMapByCircuitList = result;
                SelectMapByCircuit(result);

                _isTraced = true;
            }
            catch
            {
                message = "Error in circuit tracing.";
            }
            finally
            {
                if (feederExt != null) { while (Marshal.ReleaseComObject(feederExt) > 0);}
                if (feederSpace != null) { while (Marshal.ReleaseComObject(feederSpace) > 0);}
                if (feederSource != null) { while (Marshal.ReleaseComObject(feederSource) > 0);}
                if (downstreamTracer != null) { while (Marshal.ReleaseComObject(downstreamTracer) > 0);}
                if (networkAnalysisExtForFramework != null) { while (Marshal.ReleaseComObject(networkAnalysisExtForFramework) > 0);}
                if (electricTraceSettings != null) { while (Marshal.ReleaseComObject(electricTraceSettings) > 0);}
            }

            return message;
        }

        public static IGeometry ConvertTracedEIDsToGeometry(IGeometricNetwork geoNW, IEnumNetEID junctionEIDs, IEnumNetEID edgeEIDs)
        {
            IGeometryCollection edges = ConvertEnumNetEIDtoGeometry(geoNW, edgeEIDs);
            IGeometryCollection junctions = ConvertEnumNetEIDtoGeometry(geoNW, junctionEIDs);

            IGeometryCollection tracedGeometry = new GeometryBagClass();
            tracedGeometry.SetGeometryCollection(edges);
            tracedGeometry.AddGeometryCollection(junctions);

            return (IGeometry)tracedGeometry;
        }

        public static IGeometryCollection ConvertEnumNetEIDtoGeometry(IGeometricNetwork geoNW, IEnumNetEID eids)
        {
            IEIDHelper helper = new EIDHelperClass();
            helper.ReturnGeometries = true;
            helper.GeometricNetwork = geoNW;

            IEnumEIDInfo infoEnum = helper.CreateEnumEIDInfo(eids);

            IGeometryCollection geometry = new GeometryBagClass();
            IGeometry eidGeometry = null;
            IGeometry[] geometries = new IGeometry[eids.Count];

            int i = 0;
            infoEnum.Reset();
            IEIDInfo info = infoEnum.Next();
            while (info != null)
            {
                eidGeometry = info.Geometry;
                if (eidGeometry != null)
                {
                    geometries[i++] = eidGeometry;
                }
                info = infoEnum.Next();
            }


            GeometryEnvironmentClass geoClass = new GeometryEnvironmentClass();
            geoClass.AddGeometries(geometry, ref geometries);

            return geometry;
        }


        private void SelectMapByCircuit(List<String> result) //cboGridNumber_Selected
        {
            if (_asyncInvokeResult != null)
            {
                cboGridNumber.EndInvoke(_asyncInvokeResult); //Wait for cboGridNumber to be populated completely

                int itemIndex = 0;
                List<String> cboItems = new List<String>();

                if ((cboGridNumber as MultiSelect).UseMultiOption)
                {
                    foreach (var item in result)
                    {
                        if ((cboGridNumber.DropDownControl as CheckedListBox).Items.Contains(item.ToString()))
                        {
                            int index = (cboGridNumber.DropDownControl as CheckedListBox).Items.IndexOf(item.ToString());
                            string itm = (cboGridNumber.DropDownControl as CheckedListBox).Items[index] as string;
                            cboItems.Add(itm);
                        }
                        itemIndex++;
                    }
                    cboItems.Sort();
                    (cboGridNumber.DropDownControl as CheckedListBox).Items.Clear();
                    foreach (object itm in cboItems)
                    {
                        (cboGridNumber.DropDownControl as CheckedListBox).Items.Add(itm);
                    }
                }
                else
                {
                    foreach (var item in result)
                    {
                        if (cboGridNumber.Items.Contains(item.ToString()))
                        {
                            int index = cboGridNumber.Items.IndexOf(item.ToString());
                            string itm = cboGridNumber.Items[index] as string;
                            cboItems.Add(itm);
                        }
                        itemIndex++;
                    }
                    cboItems.Sort();
                    cboGridNumber.Items.Clear();
                    foreach (object itm in cboItems)
                    {
                        cboGridNumber.Items.Add(itm);
                    }
                }
            }

            //foreach (var item in result)
            //{
            //   if ((cboGridNumber.DropDownControl as CheckedListBox).Items.Contains(item.ToString()))
            //   {
            //       (cboGridNumber.DropDownControl as CheckedListBox).SetItemChecked(cboGridNumber.Items.IndexOf(item.ToString()), true);
            //       //(cboGridNumber.DropDownControl as CheckedListBox).SetItemChecked(cboGridNumber.Items.IndexOf(item.ToString()), true);
            //   }                   
            //   itemIndex++;

            //}

        }

        private void cboSubStation_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedGeoNetwork = cboNetwork.SelectedItem.ToString();
            IGeometricNetwork geomNetwork = _geomNetworks[selectedGeoNetwork];

            lstCircuitList.Items.Clear();

            if (cboSubStation.SelectedIndex > 0)
            {
                PopulateCircuitList(geomNetwork);
            }

            _isTraced = false;
            lblTraceStatus.Text = "";

            if (_tracedMapByCircuitList != null) _tracedMapByCircuitList.Clear();
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
                    selectedGrids.ForEach(g =>
                    {
                        if (file.ToUpper().Contains("_" + g.ToUpper() + "_") && !selectedFiles.ContainsKey(g))
                            selectedFiles.Add(g, d);
                    });
                });

                PDFMassPrinter pdfPrinter = new PDFMassPrinter(selectedFiles.Values.ToList(), printerName);
                pdfPrinter.PrintAllPDFs();
            }
        }

        private void btnNetwork_Click(object sender, EventArgs e)
        {
            if (grpNetwork.Visible == true)
            {
                grpNetwork.Visible = false;
                btnNetwork.Text = "&Trace Circuit ▾";
                //this.Height = _WindowFullHeight - grpNetwork.Height;
                this.Size = new Size(this.Width, _WindowFullHeight - grpNetwork.Height);
            }
            else
                grpNetwork.Visible = true;

            if (!grpFilter.Visible && grpNetwork.Visible)
            {
                grpNetwork.Top = grpFilter.Top;
                btnNetwork.Text = "&Trace Circuit ▴";
                //this.Height = _WindowFullHeight - grpFilter.Height;
                this.Size = new Size(this.Width, _WindowFullHeight - grpFilter.Height);
            }
            else if (grpFilter.Visible && grpNetwork.Visible)
            {
                grpNetwork.Top = _grpNetworkTop;
                btnNetwork.Text = "&Trace Circuit ▴";
                this.Size = new Size(this.Width, _WindowFullHeight);
            }

            this.CenterToScreen();
        }

        private void cboScale_SelectedIndexChanged(object sender, EventArgs e)
        {
            _isTraced = false;
            lblTraceStatus.Text = "";

            if (_tracedMapByCircuitList != null) _tracedMapByCircuitList.Clear();
            for (int i = 0; i <= lstCircuitList.Items.Count - 1; i++)
            {
                lstCircuitList.Items[i].Checked = false;
            }
            lstCircuitList.SelectedIndices.Clear();
        }

        private void dBConnectionPropertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string message = string.Empty;
            DBConnectionDialogBox dlg = new DBConnectionDialogBox();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                //Connect to SDE
                while (true)
                {
                    if (dlg.IsCancelled)
                    {
                        DialogResult result = MessageBox.Show("This will close the Map book print UI... Preceed?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (result == System.Windows.Forms.DialogResult.Yes)
                        {
                            this.Close();
                            break;
                        }
                    }
                    else if (GetConnection(out message))
                    {
                        this.Text = _CAPTION + string.Format("   ( {0})", message);
                        break;
                    }

                    if (!string.IsNullOrEmpty(message)) dlg.lblMessage.Text = message;
                    dlg.ShowDialog();
                }
            }
        }

        private bool GetConnection(out string message)
        {
            string connectionString = null;
            string usr = null;
            string pwd = null;

            bool isConnected = false;
            message = string.Empty;

            UserRegistry userReg = new UserRegistry(Common.MapBookDBConnectionRegKey);
            try
            {
                connectionString = userReg.GetSetting<string>(Common.MapBookDBConnectionKeyName, "");
                usr = userReg.GetSetting<string>(Common.MapBookDBUserKeyName, "");
                pwd = EncryptionFacade.Decrypt(userReg.GetSetting<string>(Common.MapBookDBPasswordKeyName, ""));
            }
            catch { }

            if (!string.IsNullOrEmpty(connectionString) && !string.IsNullOrEmpty(usr) && !string.IsNullOrEmpty(pwd))
            {
                try
                {
                    if (_workspace != null)
                    {
                        while (Marshal.ReleaseComObject(_workspace) > 0) ;
                        _workspace = null;
                    }
                    _workspace = Common.OpenWorkspace(connectionString, usr, pwd);

                    if (_workspace != null)
                    {
                        _circuitSourceTable = ModelNameFacade.ObjectClassByModelName(_workspace, Common.ClassModelNames.CircuitSource) as ITable;

                        _tracedMapByCircuitList = new List<string>();

                        PopulateSubstationDropDown();
                        PopulateNetworkDropDown();

                        message = string.Format("Connected to {0}", connectionString.Substring(connectionString.ToLower().LastIndexOf(":") + 1).ToUpper());
                        isConnected = true;
                    }
                }
                catch(Exception ex)
                { message = "Invalid database connection"; }
            }
            else
            {
                message = "Please enter valid database connection details";
            }

            return isConnected;
        }

        private void lstCircuitList_SelectedIndexChanged(object sender, EventArgs e)
        {
            _isTraced = false;
            lblTraceStatus.Text = "Tracing Status: In progress";
            lblTraceStatus.ForeColor = Color.Black;

            if (lstCircuitList.SelectedIndices.Count > 0)
            {
                int selectedIndex = lstCircuitList.SelectedIndices[0];

                for (int i = 0; i <= lstCircuitList.Items.Count - 1; i++)
                {
                    if (i != selectedIndex)
                        lstCircuitList.Items[i].Checked = false;
                }

                if (!lstCircuitList.Items[selectedIndex].Checked)
                {
                    lstCircuitList.Items[selectedIndex].Checked = true;
                }
            }
        }

        private void lstCircuitList_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            int index = e.Item.Index; //e.Index;

            if (lstCircuitList.Items[index].Checked)
            {
                for (int i = 0; i <= lstCircuitList.Items.Count - 1; i++)
                {
                    if (i != index && lstCircuitList.Items[i].Checked)
                    {
                        lstCircuitList.Items[i].Checked = false;
                    }
                    else if (i == index)
                    {
                        lstCircuitList.SelectedIndices.Clear();
                        lstCircuitList.SelectedIndices.Add(index);
                    }
                }
            }
        }

        private void cboRegion_SelectedIndexChanged(object sender, EventArgs e)
        {
            cboSubStation.SelectedIndex = -1;
        }

        private void cboDivision_SelectedIndexChanged(object sender, EventArgs e)
        {
            cboSubStation.SelectedIndex = -1;
        }

        private void cboDistrict_SelectedIndexChanged(object sender, EventArgs e)
        {
            cboSubStation.SelectedIndex = -1;
        }

    }
}

