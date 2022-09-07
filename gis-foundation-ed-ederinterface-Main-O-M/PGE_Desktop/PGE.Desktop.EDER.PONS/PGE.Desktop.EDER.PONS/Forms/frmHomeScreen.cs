using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.Geodatabase;
using PGE.Common.Delivery.Diagnostics;

namespace PGE.Desktop.EDER.ArcMapCommands.PONS
{
    public partial class PONSHomeScreen : Form
    {
        public IFeature pConfirmedFeature = default(IFeature);
        public List<string> divisionList = new List<string>();
        //public Dictionary<string, string> SubstationsDict = new Dictionary<string, string>();
        //public Dictionary<string, string> SubstationBankDict = new Dictionary<string, string>();
        //public Dictionary<string, string> SubstationCircuitsDict = new Dictionary<string, string>();
        //private static IDictionary pDic_CGC_TNUM = new Hashtable();
        private MapUtility objMapUtility = null;
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        private DataTable pDT_DIV_SUB_BNK_CID = new DataTable();

        public PONSHomeScreen()
        {
            InitializeComponent();
        }

        private void btnWelcomeProceed_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbDivisionWelcome.SelectedIndex < 1)
                {
                    MessageBox.Show("Please select a division to proceed further.", "Customer Notification");
                    return;
                }
                else
                {
                    PGEGlobal.WIZARD_DIVISION = cmbDivisionWelcome.SelectedItem.ToString();
                    MapUtility mu = new MapUtility();

                    PGEGlobal.WIZARD_DIVISION_CODE = mu.GetSelectedDivisionCode();
                    //SubstationsDict.Clear();
                    //SubstationsDict.Add("<Select>", "<Select>");
                    //Dictionary<string, string> dict = new Dictionary<string, string>();
                    //dict = mu.GetSubStations();

                    ////dict = dict.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
                    //foreach (KeyValuePair<string, string> kvp in dict)
                    //{
                    //    SubstationsDict.Add(kvp.Key, kvp.Value);
                    //}

                    pDT_DIV_SUB_BNK_CID = mu.GetCircuitID_Bank_SS_Div();
                    this.cmbSubstation.Items.Clear();
                    this.cmbSubstation.Items.AddRange(new object[] { "<Select>" });                   

                    this.cmbSubstation.SelectedIndexChanged -= new System.EventHandler(this.cmbSubstation_SelectedIndexChanged);
                    foreach (DataRow pDRow in pDT_DIV_SUB_BNK_CID.Rows)
                        if (!this.cmbSubstation.Items.Contains(Convert.ToString(pDRow["STATIONNUMBER"]) + (string.IsNullOrEmpty(Convert.ToString(pDRow["STATIONNUMBER"])) ? "" : " ") + Convert.ToString(pDRow["SUBSTATIONNAME"])))
                            this.cmbSubstation.Items.AddRange(new object[] { Convert.ToString(pDRow["STATIONNUMBER"]) + (string.IsNullOrEmpty(Convert.ToString(pDRow["STATIONNUMBER"])) ? "" : " ") + Convert.ToString(pDRow["SUBSTATIONNAME"]) });
                    //cmbSubstation.DataSource = new BindingSource(SubstationsDict, null);
                    //cmbSubstation.ValueMember = "Key";
                    //cmbSubstation.DisplayMember = "Value";
                    cmbSubstation.SelectedIndex = 0;
                    this.cmbSubstation.SelectedIndexChanged += new System.EventHandler(this.cmbSubstation_SelectedIndexChanged);
                    //this.cmbBank.DataSource = this.cmbCircuit.DataSource = null;
                    //this.cmbBank.Items.Clear();
                    //this.cmbCircuit.Items.Clear();
                    //this.cmbBank.Items.AddRange(new object[] { "<Select>" });
                    //this.cmbCircuit.Items.AddRange(new object[] { "<Select>" });
                    this.txtStartDevice.Text = this.txtEndDevice.Text = this.txtTransformer.Text = string.Empty;
                }
                ActivatePanel(pnlSearch);
                this.cmbDivisionWelcome.Enabled = false;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                MessageBox.Show(ex.Message);
            }

        }

        private void rbBetweenDevices_CheckedChanged(object sender, EventArgs e)
        {
            if (rbBetweenDevicesSearch.Checked)
            {
                pnlSearchInputBetweenDevices.Visible = true;
                pnlSearchInputTransformer.Visible = false;
                pnlSearchInputSubCircuit.Visible = false;
                txtStartDevice.Focus();
            }
        }

        private void rbTransformerSearch_CheckedChanged(object sender, EventArgs e)
        {
            if (rbTransformerSearch.Checked)
            {
                pnlSearchInputBetweenDevices.Visible = false;
                pnlSearchInputTransformer.Visible = true;
                pnlSearchInputSubCircuit.Visible = false;
                txtTransformer.Focus();
            }
        }

        private void rbSubCircuitSearch_CheckedChanged(object sender, EventArgs e)
        {
            if (rbSubCircuitSearch.Checked)
            {
                pnlSearchInputBetweenDevices.Visible = false;
                pnlSearchInputTransformer.Visible = false;
                pnlSearchInputSubCircuit.Visible = true;
                cmbSubstation.Focus();
            }
        }
        CustomerReport pReport = new CustomerReport();

        private void btnGetCustomer_Click(object sender, EventArgs e)
        {
            //dgCustomerList.Rows.Add("TRUE", "DOM", "1275397305", "William Pennington 9740 Los Lagos Cir N Granite Bay,CA 95746-5852");
            //Demo(@"C:\Temp\User.csv");
            //LoadConsumerstoGrid();
            //ActivatePanel(pnlCustomerList);
            //return;

            try
            {
                #region variable declaration
                iSNO = 0;
                this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
                // string transformercgcs = "", substationcircuitids = "", substationcircuitidspart = "", PrimaryMeterCGCs = string.Empty;
                string substationcircuitids = "", substationcircuitidspart = string.Empty;

                string[] tmpcircuitids, tmpcircuitidspart;
                IList<string> transformercgcs = new List<string>(), PrimaryMeterCGCs = new List<string>();

                UtilityFunctions objUtilityFunctions = new UtilityFunctions();
                IList<Customer> CustomerList = new List<Customer>();
                //Customer pCustomer = default(Customer);
                int rownum = 0;

                string PriMeter_FCName = string.Empty,
                    Transformer_FCName = string.Empty;

                Utilities.PGE_EDGIS_Tracing objTracing = new Utilities.PGE_EDGIS_Tracing();

                #endregion variable declaration
                pReport.ExcludeCustomer_Checked = checkBoxExclude.Checked;
                // Commented by SB on 02-15-2016
                IEditor pEditor = objUtilityFunctions.GetArcMapExtensionByCLSID("esriEditor.Editor") as IEditor;


                PriMeter_FCName = objUtilityFunctions.ReadConfigurationValue("PrimaryMeter");
                Transformer_FCName = objUtilityFunctions.ReadConfigurationValue("Transformer");

                for (rownum = 0; rownum < dgdeviceView.RowCount; rownum++)
                {
                    if (Convert.ToString(this.dgdeviceView.Rows[rownum].Cells["SearchType"].Value).Equals("TransformerSearch"))
                    {
                        if (Convert.ToString(this.dgdeviceView.Rows[rownum].Cells["InputID2"].Value).Equals(PriMeter_FCName))
                        {
                            #region PriMeterSearch
                            PrimaryMeterCGCs.Add(Convert.ToString(this.dgdeviceView.Rows[rownum].Cells["InputID1"].Value));
                            //if (PrimaryMeterCGCs.Length == 0)
                            //{
                            //    PrimaryMeterCGCs = Convert.ToString(this.dgdeviceView.Rows[rownum].Cells["InputID1"].Value);
                            //}
                            //else
                            //{
                            //    PrimaryMeterCGCs = PrimaryMeterCGCs + "," + Convert.ToString(this.dgdeviceView.Rows[rownum].Cells["InputID1"].Value);
                            //}
                            #endregion PriMeterSearch
                        }
                        else if (Convert.ToString(this.dgdeviceView.Rows[rownum].Cells["InputID2"].Value).Equals(Transformer_FCName))
                        {
                            #region TransformerSearch
                            transformercgcs.Add(Convert.ToString(this.dgdeviceView.Rows[rownum].Cells["InputID1"].Value));

                            //if (transformercgcs.Length == 0)
                            //{
                            //    transformercgcs = Convert.ToString(this.dgdeviceView.Rows[rownum].Cells["InputID1"].Value);
                            //}
                            //else
                            //{
                            //    transformercgcs = transformercgcs + "," + Convert.ToString(this.dgdeviceView.Rows[rownum].Cells["InputID1"].Value);
                            //}
                            #endregion TransformerSearch
                        }
                    }
                    else if (Convert.ToString(this.dgdeviceView.Rows[rownum].Cells["SearchType"].Value).Equals("SubstationSearch"))
                    {
                        #region SubstationSearch

                        if (substationcircuitids.Length == 0)
                        {
                            substationcircuitids = Convert.ToString(this.dgdeviceView.Rows[rownum].Cells["InputID1"].Value);
                        }
                        else
                        {
                            tmpcircuitids = substationcircuitids.Split(',');
                            tmpcircuitidspart = (Convert.ToString(this.dgdeviceView.Rows[rownum].Cells["InputID1"].Value)).Split(',');
                            foreach (string str in tmpcircuitidspart)
                            {

                                if (substationcircuitidspart.Length == 0)
                                {
                                    if (System.Array.FindIndex(tmpcircuitids, x => x == str) == -1)
                                    {
                                        substationcircuitidspart = str;
                                    }
                                }
                                else
                                {
                                    if (System.Array.FindIndex(tmpcircuitids, x => x == str) == -1)
                                    {
                                        substationcircuitidspart = substationcircuitidspart + "," + str;
                                    }
                                }
                            }
                            if (substationcircuitidspart.Length > 0)
                                substationcircuitids = substationcircuitids + "," + substationcircuitidspart;
                        }
                        #region Feeder Fed Feeder

                        DataTable dtFedCircuitIDs = objUtilityFunctions.ExecuteQuery("SELECT TO_CIRCUITID FROM " +
                                    objUtilityFunctions.ReadConfigurationValue("FEEDERFEDINFOTABLE") +
                                    " WHERE FROM_CIRCUITID IN ('" + substationcircuitids.Replace(",", "','") + "')", objUtilityFunctions.ReadConnConfigValue("EDER_ConnectionString"));
                        foreach (DataRow pDRow in dtFedCircuitIDs.Rows)
                            if (substationcircuitids.Length > 0)
                                substationcircuitids = substationcircuitids + "," + Convert.ToString(pDRow[0]);
                        dtFedCircuitIDs.Clear();

                        #endregion


                        #endregion SubstationSearch
                    }
                    else if (Convert.ToString(this.dgdeviceView.Rows[rownum].Cells["SearchType"].Value).Equals("BetweenDevicesSearch"))
                    {
                        #region BetweenDevicesSearch

                        #region Variable Declaration
                        //IFeatureClass pStartFeatClass = null, pEndFeatClass = null;
                        IFeature startFeature = null, endFeature = null;
                        bool bEndDeviceMentioned = false;
                        string[] startDevice = null, endDevice = null;
                        string SPName = string.Empty;
                        PGEFeatureClass startPGEFeatClass = null, endPGEFeatClass = null;
                        #endregion Variable Declaration

                        startDevice = Convert.ToString(this.dgdeviceView.Rows[rownum].Cells["InputID1"].Value).Split(';');
                        //pStartFeatClass = (PGEGlobal.WORKSPACE_MAP as IFeatureWorkspace).OpenFeatureClass(
                        //    startDevice[0]);
                        //if(pStartFeatClass != null)
                        //    startFeature = pStartFeatClass.GetFeature(Convert.ToInt32(startDevice[1]));

                        startPGEFeatClass = objMapUtility.GetFeatureClassByName(startDevice[0]);
                        if (startPGEFeatClass == null)
                        {
                            MessageBox.Show("Could not open feature class: " + startDevice[0]);
                            continue;
                        }

                        if (startPGEFeatClass.FeatureClass == null)
                        {
                            MessageBox.Show("Could not open feature class: " + startDevice[0]);
                            continue;
                        }

                        if (startPGEFeatClass.FeatureClass != null)
                            startFeature = startPGEFeatClass.FeatureClass.GetFeature(Convert.ToInt32(startDevice[1]));

                        if (startFeature == null)
                        {
                            MessageBox.Show("Could not find start feature: " + this.dgdeviceView.Rows[rownum].Cells["InputID1"].Value);
                            continue;
                        }

                        bEndDeviceMentioned = !string.IsNullOrEmpty(Convert.ToString(this.dgdeviceView.Rows[rownum].Cells["InputID2"].Value));
                        if (bEndDeviceMentioned)
                        {
                            if (this.dgdeviceView.Rows[rownum].Cells["InputID1"].Value.Equals(this.dgdeviceView.Rows[rownum].Cells["InputID2"].Value))
                                continue;

                            endDevice = Convert.ToString(this.dgdeviceView.Rows[rownum].Cells["InputID2"].Value).Split(';');
                            //pEndFeatClass = (PGEGlobal.WORKSPACE_MAP as IFeatureWorkspace).OpenFeatureClass(endDevice[0]);
                            //if (pEndFeatClass != null)

                            endPGEFeatClass = objMapUtility.GetFeatureClassByName(endDevice[0]);
                            if (endPGEFeatClass == null)
                            {
                                MessageBox.Show("Could not open feature class: " + endDevice[0]);
                                continue;
                            }

                            if (endPGEFeatClass.FeatureClass == null)
                            {
                                MessageBox.Show("Could not open feature class: " + endDevice[0]);
                                continue;
                            }

                            if (endPGEFeatClass.FeatureClass != null)
                            {
                                endFeature = endPGEFeatClass.FeatureClass.GetFeature(Convert.ToInt32(endDevice[1]));
                            }

                            if (endFeature == null)
                            {
                                MessageBox.Show("Could not find end feature");
                                return;
                            }
                        }

                        // Commented by SB on 02-15-2016
                        // if (bEditingStarted && startPGEFeatClass.LayerAddedToMap && (bEndDeviceMentioned ? endPGEFeatClass.LayerAddedToMap : true))


                        //if (startPGEFeatClass.LayerAddedToMap && (bEndDeviceMentioned ? endPGEFeatClass.LayerAddedToMap : true))
                        {
                            #region In Editing State

                            string sDatasetName = string.Empty,
                                sVersionName = string.Empty;
                            if (startPGEFeatClass.FeatureClass != null)
                            {
                                IDataset pFeatDataset = startPGEFeatClass.FeatureClass as IDataset;
                                if (pFeatDataset != null)
                                    sDatasetName = pFeatDataset.Name;

                                sVersionName = GetCurrentVersionName(sDatasetName);
                                if (sVersionName.Length == 0)
                                    return;

                                //string strServicePointIDs = objTracing.RunTrace(startFeature, endFeature, this.checkBoxExclude.Checked, sVersionName);
                                //if(string.IsNullOrEmpty(strServicePointIDs) || strServicePointIDs.Length <= 0)
                                //    strServicePointIDs = objTracing.RunTrace(endFeature, startFeature, this.checkBoxExclude.Checked, sVersionName);

                                IList<string> tempTransCGCs = new List<string>(),
                                    tempPriCGCs = new List<string>();
                                bool bStatus = false;
                                bool isOnSameNetwork = false;
                                //bStatus = objTracing.RunTrace(startFeature, endFeature, this.checkBoxExclude.Checked, sVersionName, out tempTransCGCs, out tempPriCGCs);
                                bStatus = objTracing.RunTrace(startFeature, endFeature, !true, sVersionName, out tempTransCGCs, out tempPriCGCs, ref isOnSameNetwork);
                                if (!bStatus || (tempTransCGCs.Count <= 0 && tempPriCGCs.Count <= 0))
                                    //bStatus = objTracing.RunTrace(endFeature, startFeature, this.checkBoxExclude.Checked, sVersionName, out tempTransCGCs, out tempPriCGCs);
                                    bStatus = objTracing.RunTrace(endFeature, startFeature, !true, sVersionName, out tempTransCGCs, out tempPriCGCs, ref isOnSameNetwork);

                                if (!isOnSameNetwork && bEndDeviceMentioned)
                                {
                                    MessageBox.Show(Convert.ToString(this.dgdeviceView.Rows[rownum].Cells[0].Value) + "\n\nThe end device is not downstream of the start device.\nPlease select start and end devices on the same branch of the same circuit.", "PONS", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                                    return;
                                }

                                if (bStatus && tempTransCGCs.Count > 0)
                                    ((List<string>)transformercgcs).AddRange(tempTransCGCs);

                                if (bStatus && tempPriCGCs.Count > 0)
                                    ((List<string>)PrimaryMeterCGCs).AddRange(tempPriCGCs);


                                //if (!string.IsNullOrEmpty(strServicePointIDs))
                                //{
                                //    {
                                //        DataTable dtParams = new DataTable();
                                //        dtParams.Columns.Add("Param_Name", typeof(string));
                                //        dtParams.Columns.Add("Param_Type", typeof(string));
                                //        dtParams.Columns.Add("Param_Value", typeof(string));
                                //        dtParams.Rows.Add(new object[] { "P_INPUTSTRING", "OracleType.VarChar", strServicePointIDs });

                                //        if (this.checkBoxExclude.Checked)
                                //        {
                                //            SPName = "SP_CustInfo_ByServicePointIDs_EXCL";
                                //        }
                                //        else
                                //        {
                                //            SPName = "SP_CustInfo_ByServicePointIDs_INCL";
                                //        }

                                //        DataTable dtCustomerList = objUtilityFunctions.ExecuteSP(objUtilityFunctions.ReadConnConfigValue("EDER_ConnectionString"),
                                //            objUtilityFunctions.ReadConfigurationValue(SPName), dtParams, sVersionName);

                                //        #region Commented SB 03-19-2016
                                //        //IList<Customer> pTracedCustomerList = new List<Customer>();
                                //        //pCustomer = new Customer();

                                //        //foreach (DataRow drCustomer in dtCustomerList.Rows)
                                //        //{
                                //        //    pCustomer = default(Customer);
                                //        //    pCustomer = new Customer();
                                //        //    pCustomer.ServicePointID = Convert.ToString(drCustomer["ServicePointID"]);
                                //        //    pCustomer.MeterNumber = Convert.ToString(drCustomer["MeterNumber"]);
                                //        //    pCustomer.CustomerType = Convert.ToString(drCustomer["CustomerType"]);
                                //        //    pCustomer.CGC12 = Convert.ToString(drCustomer["CGC12"]);
                                //        //    //pCustomer.TransformerOperatingNumber = Convert.ToString(drCustomer["OperatingNumber"]);
                                //        //    pCustomer.SSD = Convert.ToString(drCustomer["SourceSideDeviceId"]);

                                //        //    try
                                //        //    {
                                //        //        pCustomer.CustomerName.MailName1 = Convert.ToString(drCustomer["MAILNAME1"]);
                                //        //        pCustomer.CustomerName.MailName2 = Convert.ToString(drCustomer["MAILNAME2"]);
                                //        //        pCustomer.PhoneNumber.PhoneNumber = Convert.ToString(drCustomer["PHONENUM"]);

                                //        //        pCustomer.CustomerAccountNumber = Convert.ToString(drCustomer["AccountID"]);
                                //        //    }
                                //        //    catch (Exception)
                                //        //    { }

                                //        //    try
                                //        //    {
                                //        //        pCustomer.Division = Convert.ToString(drCustomer["Division"]);
                                //        //    }
                                //        //    catch (Exception)
                                //        //    {
                                //        //        pCustomer.Division = PGEGlobal.WIZARD_DIVISION;
                                //        //    }

                                //        //    Address pCustomerAddress = new Address();

                                //        //    if (dtCustomerList.Columns.Contains("City"))
                                //        //    {
                                //        //        pCustomerAddress.City = Convert.ToString(drCustomer["City"]);
                                //        //        pCustomerAddress.State = Convert.ToString(drCustomer["State"]);
                                //        //        pCustomerAddress.StreetName1 = Convert.ToString(drCustomer["StreetName1"]);
                                //        //        pCustomerAddress.StreetName2 = Convert.ToString(drCustomer["StreetName2"]);
                                //        //        pCustomerAddress.StreetNumber = Convert.ToString(drCustomer["StreetNumber"]);
                                //        //        pCustomerAddress.ZIPCode = Convert.ToString(drCustomer["Zip"]);
                                //        //        pCustomer.CustomerAddress = pCustomerAddress;
                                //        //    }
                                //        //    else
                                //        //    {
                                //        //        pCustomerAddress.City = "";
                                //        //        pCustomerAddress.State = "";
                                //        //        pCustomerAddress.StreetName1 = "";
                                //        //        pCustomerAddress.StreetName2 = "";
                                //        //        pCustomerAddress.StreetNumber = "";
                                //        //        pCustomerAddress.ZIPCode = "";
                                //        //        pCustomer.CustomerAddress = new Address();
                                //        //    }

                                //        //    Address pMailAddress = new Address();
                                //        //    if (dtCustomerList.Columns.Contains("MailStreetName1"))
                                //        //    {
                                //        //        pMailAddress.City = Convert.ToString(drCustomer["MailCity"]);
                                //        //        pMailAddress.State = Convert.ToString(drCustomer["MailState"]);
                                //        //        pMailAddress.StreetName1 = Convert.ToString(drCustomer["MailStreetName1"]);
                                //        //        pMailAddress.StreetName2 = Convert.ToString(drCustomer["MailStreetName2"]);
                                //        //        pMailAddress.StreetNumber = Convert.ToString(drCustomer["MailStreetNum"]);
                                //        //        pMailAddress.ZIPCode = Convert.ToString(drCustomer["MailZipCode"]);
                                //        //    }
                                //        //    else
                                //        //    {
                                //        //        pMailAddress.City = "";
                                //        //        pMailAddress.State = "";
                                //        //        pMailAddress.StreetName1 = "";
                                //        //        pMailAddress.StreetName2 = "";
                                //        //        pMailAddress.StreetNumber = "";
                                //        //        pMailAddress.ZIPCode = "";
                                //        //    }
                                //        //    pCustomer.MailAddress = pMailAddress;

                                //        //    pTracedCustomerList.Add(pCustomer);
                                //        //}
                                //        #endregion Commented SB 03-19-2016

                                //        IList<Customer> pTracedCustomerList = FillCustomerList(dtCustomerList);

                                //        if (pTracedCustomerList.Count > 0)
                                //            ((List<Customer>)CustomerList).AddRange(pTracedCustomerList);
                                //    }
                                //}
                            }
                            #endregion In Editing State
                        }
                        #region Commented by SB on 02-15-2016
                        //else
                        //{
                        //    #region Not Editing

                        //    {
                        //        string startFCID = string.Empty, FeederID = string.Empty, startFeatureGUID = string.Empty;
                        //        FeederID = Convert.ToString(this.dgdeviceView.Rows[rownum].Cells["Feeder"].Value);

                        //        //string sTransformerFC = objUtilityFunctions.ReadConfigurationValue("Transformer");

                        //        switch (startDevice[0])
                        //        {
                        //            case "EDGIS.Transformer":
                        //                startFCID = "FCID_TRANSFORMER";
                        //                break;
                        //            case "EDGIS.Fuse":
                        //                startFCID = "FCID_FUSE";
                        //                break;
                        //            case "EDGIS.Switch":
                        //                startFCID = "FCID_SWITCH";
                        //                break;
                        //            case "EDGIS.OpenPoint":
                        //                startFCID = "FCID_OPENPOINT";
                        //                break;
                        //            case "EDGIS.PriOHConductor":
                        //                startFCID = "FCID_PRIOHCONDUCTOR";
                        //                break;
                        //            case "EDGIS.PriUGConductor":
                        //                startFCID = "FCID_PRIUGCONDUCTOR";
                        //                break;
                        //        }

                        //        if (string.IsNullOrEmpty(startFCID))
                        //            throw new Exception("Invalid Start device");

                        //        startFCID = objUtilityFunctions.ReadConfigurationValue(startFCID);
                        //        startFeatureGUID = Convert.ToString(startFeature.get_Value(startFeature.Fields.FindField("GLOBALID")));


                        //        string endFCID = string.Empty, endFeatureGUID = string.Empty;
                        //        if (bEndDeviceMentioned)
                        //        {
                        //            switch (endDevice[0])
                        //            {
                        //                case "EDGIS.Transformer":
                        //                    endFCID = "FCID_TRANSFORMER";
                        //                    break;
                        //                case "EDGIS.Fuse":
                        //                    endFCID = "FCID_FUSE";
                        //                    break;
                        //                case "EDGIS.Switch":
                        //                    endFCID = "FCID_SWITCH";
                        //                    break;
                        //                case "EDGIS.OpenPoint":
                        //                    endFCID = "FCID_OPENPOINT";
                        //                    break;
                        //                case "EDGIS.PriOHConductor":
                        //                    startFCID = "FCID_PRIOHCONDUCTOR";
                        //                    break;
                        //                case "EDGIS.PriUGConductor":
                        //                    startFCID = "FCID_PRIUGCONDUCTOR";
                        //                    break;
                        //            }

                        //            if (string.IsNullOrEmpty(endFCID))
                        //                throw new Exception("Invalid End device");

                        //            endFCID = objUtilityFunctions.ReadConfigurationValue(endFCID);
                        //            endFeatureGUID = Convert.ToString(endFeature.get_Value(endFeature.Fields.FindField("GLOBALID")));
                        //        }

                        //        IList<Customer> objTracedCustomerList = objUtilityFunctions.GetCustomerListWEBRTrace(startFeature, endFeature, FeederID, startFCID, endFCID, this.checkBoxExclude.Checked);
                        //        //List2 = new List<TestClass>(List2.Concat(List1))
                        //        ((List<Customer>)CustomerList).AddRange(objTracedCustomerList);
                        //        //CustomerList.Concat(objTracedCustomerList);
                        //        //foreach (Customer pTracedCustomer in objTracedCustomerList)
                        //        //    CustomerList.Add(pTracedCustomer);
                        //    }

                        //    #endregion Not Editing
                        //}
                        #endregion Commented by SB on 02-15-2016

                        #endregion BetweenDevicesSearch
                    }
                }

                #region if Editing then for substation
                if (pEditor.EditState == esriEditState.esriStateEditing && !string.IsNullOrEmpty(substationcircuitids))
                {
                    string ssubstationcircuitids = "'" + substationcircuitids.Replace(",", "','") + "'";
                    PGEFeatureClass pPFClass_CisuitSource = objMapUtility.GetFeatureClassByName(objUtilityFunctions.ReadConfigurationValue("Search3_CircuitSource"));
                    IFeatureCursor pFCursor = pPFClass_CisuitSource.FeatureClass.Search(new QueryFilterClass() { WhereClause = "CIRCUITID IN (" + ssubstationcircuitids + ")" }, false);

                    IFeature pFeat_CS = default(IFeature);
                    while ((pFeat_CS = pFCursor.NextFeature()) != null)
                    {
                        IList<string> tempTransCGCs = new List<string>(),
                           tempPriCGCs = new List<string>();
                        bool bStatus = false;
                        //bStatus = objTracing.RunTrace(startFeature, endFeature, this.checkBoxExclude.Checked, sVersionName, out tempTransCGCs, out tempPriCGCs);
                        bool isOnSameNetwork1 = false;
                        bStatus = objTracing.RunTrace(pFeat_CS, null, !true, GetCurrentVersionName(((IDataset)pPFClass_CisuitSource.FeatureClass).Name), out tempTransCGCs, out tempPriCGCs, ref isOnSameNetwork1);

                        if (bStatus && tempTransCGCs.Count > 0)
                            ((List<string>)transformercgcs).AddRange(tempTransCGCs);

                        if (bStatus && tempPriCGCs.Count > 0)
                            ((List<string>)PrimaryMeterCGCs).AddRange(tempPriCGCs);
                    }
                    substationcircuitids = string.Empty;
                    pFCursor = null; pPFClass_CisuitSource = null;
                    //if (pFCursor != null) Marshal.FinalReleaseComObject(pFCursor);
                    //if (pPFClass_CisuitSource != null) Marshal.FinalReleaseComObject(pPFClass_CisuitSource);
                }
                #endregion

                if (transformercgcs.Count > 0)
                {
                    #region Transformer Search
                    string SPName = string.Empty,
                        CS_transformerCGCs = string.Empty;

                    //foreach (string CGC in transformercgcs)
                    //    CS_transformerCGCs += CGC + ",";
                    //CS_transformerCGCs.TrimEnd(',');

                    foreach (string s in transformercgcs)
                    {
                        CS_transformerCGCs += "," + s;
                    }
                    CS_transformerCGCs = CS_transformerCGCs.Substring(1);

                    //CS_transformerCGCs = transformercgcs.Concatenate(",");


                    DataTable dtParams = new DataTable();
                    dtParams.Columns.Add("Param_Name", typeof(string));
                    dtParams.Columns.Add("Param_Type", typeof(string));
                    dtParams.Columns.Add("Param_Value", typeof(string));
                    dtParams.Rows.Add(new object[] { "P_INPUTSTRING", "OracleType.VarChar", CS_transformerCGCs });
                    //dtParams.Rows.Add(new object[] { "P_INPUTSTRING", "OracleType.VarChar", transformercgcs });
                    //dtParams.Rows.Add(new object[] { "P_DIVISION", "OracleType.Number", PGEGlobal.WIZARD_DIVISION_CODE});

                    if (!true)//if (checkBoxExclude.Checked)
                    {
                        //dtParams.Rows.Add(new object[] { "p_flag", "OracleType.NVarChar", "1" });
                        SPName = "TransformerServicePointStoredProcedureName_EXCL";
                    }
                    else
                    {
                        //dtParams.Rows.Add(new object[] { "p_flag", "OracleType.NVarChar", "0" });
                        SPName = "TransformerServicePointStoredProcedureName_INCL";
                    }

                    string versionname = GetCurrentVersionName(objUtilityFunctions.ReadConfigurationValue("Transformer"));

                    if (versionname.Length == 0)
                        return;
                    DataTable dtCustomerList = objUtilityFunctions.ExecuteSP(objUtilityFunctions.ReadConnConfigValue("EDER_ConnectionString"),
                        objUtilityFunctions.ReadConfigurationValue(SPName), dtParams,
                        //(m_editor.EditWorkspace as IVersion).VersionName
                        versionname);

                    IList<Customer> pReturnCustomers = FillCustomerList(dtCustomerList);

                    if (pReturnCustomers.Count > 0)
                        ((List<Customer>)CustomerList).AddRange(pReturnCustomers);

                    #region Commented
                    //foreach (DataRow drCustomer in dtCustomerList.Rows)
                    //{
                    //    pCustomer = default(Customer);
                    //    pCustomer = new Customer();
                    //    pCustomer.ServicePointID = Convert.ToString(drCustomer["ServicePointID"]);
                    //    pCustomer.MeterNumber = Convert.ToString(drCustomer["MeterNumber"]);
                    //    pCustomer.CustomerType = Convert.ToString(drCustomer["CustomerType"]);
                    //    pCustomer.CGC12 = Convert.ToString(drCustomer["CGC12"]);
                    //    //pCustomer.TransformerOperatingNumber = Convert.ToString(drCustomer["OperatingNumber"]);
                    //    pCustomer.SSD = Convert.ToString(drCustomer["SourceSideDeviceId"]);

                    //    try
                    //    {
                    //        pCustomer.CustomerName.MailName1 = Convert.ToString(drCustomer["MAILNAME1"]);
                    //        pCustomer.CustomerName.MailName2 = Convert.ToString(drCustomer["MAILNAME2"]);
                    //        pCustomer.PhoneNumber.PhoneNumber = Convert.ToString(drCustomer["PHONENUM"]);

                    //        pCustomer.CustomerAccountNumber = Convert.ToString(drCustomer["AccountID"]);
                    //    }
                    //    catch (Exception)
                    //    { }

                    //    try
                    //    {
                    //        pCustomer.Division = Convert.ToString(drCustomer["Division"]);
                    //    }
                    //    catch (Exception)
                    //    {
                    //        pCustomer.Division = PGEGlobal.WIZARD_DIVISION;
                    //    }

                    //    Address pCustomerAddress = new Address();

                    //    if (dtCustomerList.Columns.Contains("City"))
                    //    {
                    //        pCustomerAddress.City = Convert.ToString(drCustomer["City"]);
                    //        pCustomerAddress.State = Convert.ToString(drCustomer["State"]);
                    //        pCustomerAddress.StreetName1 = Convert.ToString(drCustomer["StreetName1"]);
                    //        pCustomerAddress.StreetName2 = Convert.ToString(drCustomer["StreetName2"]);
                    //        pCustomerAddress.StreetNumber = Convert.ToString(drCustomer["StreetNumber"]);
                    //        pCustomerAddress.ZIPCode = Convert.ToString(drCustomer["Zip"]);
                    //        pCustomer.CustomerAddress = pCustomerAddress;
                    //    }
                    //    else
                    //    {
                    //        pCustomerAddress.City = "";
                    //        pCustomerAddress.State = "";
                    //        pCustomerAddress.StreetName1 = "";
                    //        pCustomerAddress.StreetName2 = "";
                    //        pCustomerAddress.StreetNumber = "";
                    //        pCustomerAddress.ZIPCode = "";
                    //        pCustomer.CustomerAddress = new Address();
                    //    }

                    //    Address pMailAddress = new Address();
                    //    if (dtCustomerList.Columns.Contains("MailStreetName1"))
                    //    {
                    //        pMailAddress.City = Convert.ToString(drCustomer["MailCity"]);
                    //        pMailAddress.State = Convert.ToString(drCustomer["MailState"]);
                    //        pMailAddress.StreetName1 = Convert.ToString(drCustomer["MailStreetName1"]);
                    //        pMailAddress.StreetName2 = Convert.ToString(drCustomer["MailStreetName2"]);
                    //        pMailAddress.StreetNumber = Convert.ToString(drCustomer["MailStreetNum"]);
                    //        pMailAddress.ZIPCode = Convert.ToString(drCustomer["MailZipCode"]);
                    //    }
                    //    else
                    //    {
                    //        pMailAddress.City = "";
                    //        pMailAddress.State = "";
                    //        pMailAddress.StreetName1 = "";
                    //        pMailAddress.StreetName2 = "";
                    //        pMailAddress.StreetNumber = "";
                    //        pMailAddress.ZIPCode = "";
                    //    }
                    //    pCustomer.MailAddress = pMailAddress;


                    //    CustomerList.Add(pCustomer);

                    //    #region Available attributes from Stored Procedure
                    //    /*
                    //     * a.CustomerType, 
                    //        a.StreetNumber,
                    //        a.StreetName1,
                    //        a.StreetName2,
                    //        a.MailStreetNum,
                    //        a.MailStreetName1,
                    //        a.MailStreetName2,
                    //        a.Zip,
                    //        a.MailZipCode, 
                    //        a.City,
                    //        a.MailCity,
                    //        a.State,
                    //        a.MailState,
                    //        a.CGC12,
                    //        b.OperatingNumber,
                    //        b.SourceSideDeviceId 
                    //                             * 
                    //                             * Customer type     Zz_mv_ServicePoint.CustomerType 
                    //        Customer Name       Customer_info.mailname1
                    //        Customer_info.mailname2
                    //        Address       Zz_mv_ServicePoint.StreetNumber
                    //        Zz_mv_ServicePoint.StreetName1
                    //        Zz_mv_ServicePoint.StreetName2
                    //        Customer_info.MailStreetNum
                    //        Customer_info.MailStreetName1
                    //        Customer_info.MailStreetName2
                    //        ZIP   Zz_mv_ServicePoint.Zip
                    //        Customer_info.MailZipCode 
                    //        City  Zz_mv_ServicePoint.City
                    //        Customer_info.MailCity 
                    //        State Zz_mv_ServicePoint.State
                    //        Customer_info.MailState 
                    //        CCG    Zz_mv_ServicePoint.CGC12
                    //        TNum   Zz_mv_transformer.OperatingNumber
                    //        Zz_mv_PrimaryMeter.OperatingNumber
                    //        SSD    Zz_mv_transformer.SourceSideDeviceId
                    //        <need to trace for Primary meter’s SSD>
                    //        Phone Customer_info.areacode
                    //        Customer_info.phonenum


                    //     * 
                    //     * */
                    //    #endregion Available attributes from Stored Procedure
                    //}
                    #endregion Commented
                    #endregion Transformer Search
                }

                // Added by SB on 02-22-2016
                if (PrimaryMeterCGCs.Count > 0)
                {
                    #region PrimaryMeterCGCs Search
                    string SPName = string.Empty,
                        CS_PriMeterCGCs = string.Empty;

                    //foreach (string CGC in PrimaryMeterCGCs)
                    //    CS_PriMeterCGCs += CGC + ",";
                    //CS_PriMeterCGCs.TrimEnd(',');

                    foreach (string s in PrimaryMeterCGCs)
                    {
                        CS_PriMeterCGCs += "," + s;
                    }
                    CS_PriMeterCGCs = CS_PriMeterCGCs.Substring(1);

                    //CS_PriMeterCGCs = PrimaryMeterCGCs.Concatenate(",");

                    DataTable dtParams = new DataTable();
                    dtParams.Columns.Add("Param_Name", typeof(string));
                    dtParams.Columns.Add("Param_Type", typeof(string));
                    dtParams.Columns.Add("Param_Value", typeof(string));
                    //dtParams.Rows.Add(new object[] { "P_INPUTSTRING", "OracleType.VarChar", PrimaryMeterCGCs });
                    dtParams.Rows.Add(new object[] { "P_INPUTSTRING", "OracleType.VarChar", CS_PriMeterCGCs });

                    if (!true)//if (checkBoxExclude.Checked)
                    {
                        //dtParams.Rows.Add(new object[] { "p_flag", "OracleType.NVarChar", "1" });
                        SPName = "SP_Search2_PriMeter_EXCL";
                    }
                    else
                    {
                        //dtParams.Rows.Add(new object[] { "p_flag", "OracleType.NVarChar", "0" });
                        SPName = "SP_Search2_PriMeter_INCL";
                    }

                    string versionname = GetCurrentVersionName(objUtilityFunctions.ReadConfigurationValue("PrimaryMeter"));

                    if (versionname.Length == 0)
                        return;
                    DataTable dtCustomerList = objUtilityFunctions.ExecuteSP(objUtilityFunctions.ReadConnConfigValue("EDER_ConnectionString"),
                        objUtilityFunctions.ReadConfigurationValue(SPName), dtParams,
                        //(m_editor.EditWorkspace as IVersion).VersionName
                        versionname);

                    IList<Customer> pTracedCustomerList = FillCustomerList(dtCustomerList);

                    if (pTracedCustomerList.Count > 0)
                        ((List<Customer>)CustomerList).AddRange(pTracedCustomerList);

                    #region Commented
                    //foreach (DataRow drCustomer in dtCustomerList.Rows)
                    //{
                    //    pCustomer = default(Customer);
                    //    pCustomer = new Customer();
                    //    pCustomer.ServicePointID = Convert.ToString(drCustomer["ServicePointID"]);
                    //    pCustomer.MeterNumber = Convert.ToString(drCustomer["MeterNumber"]);
                    //    pCustomer.CustomerType = Convert.ToString(drCustomer["CustomerType"]);
                    //    pCustomer.CGC12 = Convert.ToString(drCustomer["CGC12"]);
                    //    //pCustomer.TransformerOperatingNumber = Convert.ToString(drCustomer["OperatingNumber"]);
                    //    pCustomer.SSD = Convert.ToString(drCustomer["SourceSideDeviceId"]);

                    //    try
                    //    {
                    //        pCustomer.CustomerName.MailName1 = Convert.ToString(drCustomer["MAILNAME1"]);
                    //        pCustomer.CustomerName.MailName2 = Convert.ToString(drCustomer["MAILNAME2"]);
                    //        pCustomer.PhoneNumber.PhoneNumber = Convert.ToString(drCustomer["PHONENUM"]);

                    //        pCustomer.CustomerAccountNumber = Convert.ToString(drCustomer["AccountID"]);
                    //    }
                    //    catch (Exception)
                    //    { }

                    //    try
                    //    {
                    //        pCustomer.Division = Convert.ToString(drCustomer["Division"]);
                    //    }
                    //    catch (Exception)
                    //    {
                    //        pCustomer.Division = PGEGlobal.WIZARD_DIVISION;
                    //    }

                    //    Address pCustomerAddress = new Address();

                    //    if (dtCustomerList.Columns.Contains("City"))
                    //    {
                    //        pCustomerAddress.City = Convert.ToString(drCustomer["City"]);
                    //        pCustomerAddress.State = Convert.ToString(drCustomer["State"]);
                    //        pCustomerAddress.StreetName1 = Convert.ToString(drCustomer["StreetName1"]);
                    //        pCustomerAddress.StreetName2 = Convert.ToString(drCustomer["StreetName2"]);
                    //        pCustomerAddress.StreetNumber = Convert.ToString(drCustomer["StreetNumber"]);
                    //        pCustomerAddress.ZIPCode = Convert.ToString(drCustomer["Zip"]);
                    //        pCustomer.CustomerAddress = pCustomerAddress;
                    //    }
                    //    else
                    //    {
                    //        pCustomerAddress.City = "";
                    //        pCustomerAddress.State = "";
                    //        pCustomerAddress.StreetName1 = "";
                    //        pCustomerAddress.StreetName2 = "";
                    //        pCustomerAddress.StreetNumber = "";
                    //        pCustomerAddress.ZIPCode = "";
                    //        pCustomer.CustomerAddress = new Address();
                    //    }

                    //    Address pMailAddress = new Address();
                    //    if (dtCustomerList.Columns.Contains("MailStreetName1"))
                    //    {
                    //        pMailAddress.City = Convert.ToString(drCustomer["MailCity"]);
                    //        pMailAddress.State = Convert.ToString(drCustomer["MailState"]);
                    //        pMailAddress.StreetName1 = Convert.ToString(drCustomer["MailStreetName1"]);
                    //        pMailAddress.StreetName2 = Convert.ToString(drCustomer["MailStreetName2"]);
                    //        pMailAddress.StreetNumber = Convert.ToString(drCustomer["MailStreetNum"]);
                    //        pMailAddress.ZIPCode = Convert.ToString(drCustomer["MailZipCode"]);
                    //    }
                    //    else
                    //    {
                    //        pMailAddress.City = "";
                    //        pMailAddress.State = "";
                    //        pMailAddress.StreetName1 = "";
                    //        pMailAddress.StreetName2 = "";
                    //        pMailAddress.StreetNumber = "";
                    //        pMailAddress.ZIPCode = "";
                    //    }
                    //    pCustomer.MailAddress = pMailAddress;


                    //    CustomerList.Add(pCustomer);

                    //    #region Available attributes from Stored Procedure
                    //    /*
                    //     * a.CustomerType, 
                    //        a.StreetNumber,
                    //        a.StreetName1,
                    //        a.StreetName2,
                    //        a.MailStreetNum,
                    //        a.MailStreetName1,
                    //        a.MailStreetName2,
                    //        a.Zip,
                    //        a.MailZipCode, 
                    //        a.City,
                    //        a.MailCity,
                    //        a.State,
                    //        a.MailState,
                    //        a.CGC12,
                    //        b.OperatingNumber,
                    //        b.SourceSideDeviceId 
                    //                             * 
                    //                             * Customer type     Zz_mv_ServicePoint.CustomerType 
                    //        Customer Name       Customer_info.mailname1
                    //        Customer_info.mailname2
                    //        Address       Zz_mv_ServicePoint.StreetNumber
                    //        Zz_mv_ServicePoint.StreetName1
                    //        Zz_mv_ServicePoint.StreetName2
                    //        Customer_info.MailStreetNum
                    //        Customer_info.MailStreetName1
                    //        Customer_info.MailStreetName2
                    //        ZIP   Zz_mv_ServicePoint.Zip
                    //        Customer_info.MailZipCode 
                    //        City  Zz_mv_ServicePoint.City
                    //        Customer_info.MailCity 
                    //        State Zz_mv_ServicePoint.State
                    //        Customer_info.MailState 
                    //        CCG    Zz_mv_ServicePoint.CGC12
                    //        TNum   Zz_mv_transformer.OperatingNumber
                    //        Zz_mv_PrimaryMeter.OperatingNumber
                    //        SSD    Zz_mv_transformer.SourceSideDeviceId
                    //        <need to trace for Primary meter’s SSD>
                    //        Phone Customer_info.areacode
                    //        Customer_info.phonenum


                    //     * 
                    //     * */
                    //    #endregion Available attributes from Stored Procedure
                    //}
                    #endregion Commented
                    #endregion PrimaryMeterCGCs Search
                }

                if (substationcircuitids.Length > 0)
                {
                    #region Substation search
                    string SPName = string.Empty;
                    //add code to call stored procedure
                    //p_circuitIdstring in NVARCHAR2,
                    //                p_version_nm in NVARCHAR2,
                    //                p_division in number,
                    //                P_FLAG IN boolean,
                    //                p_cursor out sys_refcursor,
                    //                p_error OUT VARCHAR2,
                    //                p_success OUT NUMBER); OracleType.Number

                    DataTable dtParams = new DataTable();
                    dtParams.Columns.Add("Param_Name", typeof(string));
                    dtParams.Columns.Add("Param_Type", typeof(string));
                    dtParams.Columns.Add("Param_Value", typeof(string));
                    dtParams.Rows.Add(new object[] { "P_INPUTSTRING", "OracleType.NVarChar", substationcircuitids });

                    string versionname = GetCurrentVersionName(objUtilityFunctions.ReadConfigurationValue("Substation"));

                    if (versionname.Length == 0)
                        return;
                    //dtParams.Rows.Add(new object[] { "P_VERSION", "OracleType.NVarChar", versionname });
                    //dtParams.Rows.Add(new object[] { "P_DIVISION", "OracleType.Number", Convert.ToInt32(PGEGlobal.WIZARD_DIVISION_CODE) });

                    if (!true)//if (checkBoxExclude.Checked)
                    {
                        //dtParams.Rows.Add(new object[] { "P_FLAG", "OracleType.VarChar", "1" });
                        SPName = "SubstationSearchStoredProcedureName_EXCL";
                    }
                    else
                    {
                        //dtParams.Rows.Add(new object[] { "P_FLAG", "OracleType.NVarChar", "0" });
                        SPName = "SubstationSearchStoredProcedureName_INCL";
                    }

                    DataTable dtCustomerList = objUtilityFunctions.ExecuteSP(objUtilityFunctions.ReadConnConfigValue("EDER_ConnectionString"),
                        objUtilityFunctions.ReadConfigurationValue(SPName), dtParams, versionname);


                    IList<Customer> pTracedCustomerList = FillCustomerList(dtCustomerList);

                    if (pTracedCustomerList.Count > 0)
                        ((List<Customer>)CustomerList).AddRange(pTracedCustomerList);

                    #region Commented SB 03-19-2016
                    //foreach (DataRow drCustomer in dtCustomerList.Rows)
                    //{
                    //    pCustomer = default(Customer);
                    //    pCustomer = new Customer();
                    //    pCustomer.ServicePointID = Convert.ToString(drCustomer["ServicePointID"]);
                    //    pCustomer.MeterNumber = Convert.ToString(drCustomer["MeterNumber"]);
                    //    pCustomer.CustomerType = Convert.ToString(drCustomer["CustomerType"]);
                    //    pCustomer.CGC12 = Convert.ToString(drCustomer["CGC12"]);
                    //    //pCustomer.TransformerOperatingNumber = Convert.ToString(drCustomer["OperatingNumber"]);
                    //    pCustomer.SSD = Convert.ToString(drCustomer["SourceSideDeviceId"]);

                    //    try
                    //    {
                    //        pCustomer.CustomerName.MailName1 = Convert.ToString(drCustomer["MAILNAME1"]);
                    //        pCustomer.CustomerName.MailName2 = Convert.ToString(drCustomer["MAILNAME2"]);
                    //        pCustomer.PhoneNumber.PhoneNumber = Convert.ToString(drCustomer["PHONENUM"]);

                    //        pCustomer.CustomerAccountNumber = Convert.ToString(drCustomer["AccountID"]);
                    //    }
                    //    catch (Exception)
                    //    { }

                    //    try
                    //    {
                    //        pCustomer.Division = Convert.ToString(drCustomer["Division"]);
                    //    }
                    //    catch (Exception)
                    //    {
                    //        pCustomer.Division = PGEGlobal.WIZARD_DIVISION;
                    //    }

                    //    Address pCustomerAddress = new Address();

                    //    if (dtCustomerList.Columns.Contains("City"))
                    //    {
                    //        pCustomerAddress.City = Convert.ToString(drCustomer["City"]);
                    //        pCustomerAddress.State = Convert.ToString(drCustomer["State"]);
                    //        pCustomerAddress.StreetName1 = Convert.ToString(drCustomer["StreetName1"]);
                    //        pCustomerAddress.StreetName2 = Convert.ToString(drCustomer["StreetName2"]);
                    //        pCustomerAddress.StreetNumber = Convert.ToString(drCustomer["StreetNumber"]);
                    //        pCustomerAddress.ZIPCode = Convert.ToString(drCustomer["Zip"]);
                    //        pCustomer.CustomerAddress = pCustomerAddress;
                    //    }
                    //    else
                    //    {
                    //        pCustomerAddress.City = "";
                    //        pCustomerAddress.State = "";
                    //        pCustomerAddress.StreetName1 = "";
                    //        pCustomerAddress.StreetName2 = "";
                    //        pCustomerAddress.StreetNumber = "";
                    //        pCustomerAddress.ZIPCode = "";
                    //        pCustomer.CustomerAddress = new Address();
                    //    }

                    //    Address pMailAddress = new Address();
                    //    if (dtCustomerList.Columns.Contains("MailStreetName1"))
                    //    {
                    //        pMailAddress.City = Convert.ToString(drCustomer["MailCity"]);
                    //        pMailAddress.State = Convert.ToString(drCustomer["MailState"]);
                    //        pMailAddress.StreetName1 = Convert.ToString(drCustomer["MailStreetName1"]);
                    //        pMailAddress.StreetName2 = Convert.ToString(drCustomer["MailStreetName2"]);
                    //        pMailAddress.StreetNumber = Convert.ToString(drCustomer["MailStreetNum"]);
                    //        pMailAddress.ZIPCode = Convert.ToString(drCustomer["MailZipCode"]);
                    //    }
                    //    else
                    //    {
                    //        pMailAddress.City = "";
                    //        pMailAddress.State = "";
                    //        pMailAddress.StreetName1 = "";
                    //        pMailAddress.StreetName2 = "";
                    //        pMailAddress.StreetNumber = "";
                    //        pMailAddress.ZIPCode = "";
                    //    }
                    //    pCustomer.MailAddress = pMailAddress;


                    //    CustomerList.Add(pCustomer);


                    //}
                    #endregion Commented SB 03-19-2016
                    #endregion Substation search
                }

                #region Customer List Computation
                iSNO = 0;
                pReport.AffectedCustomers_Original.Clear(); pReport.AffectedCustomers.Clear();
                pReport.AffectedCustomers_Original = CustomerList.GroupBy(c => new { c.ServicePointID, c.ORD })
                                                                 .Select(c => c.First())
                                                                 .ToList()
                                                                 .OrderBy(c => c.SSD, new CustomComparer())
                                                                 .ThenBy(c => c.CGC12, new CustomComparer())                                                               
                                                                 .ThenBy(c => c.CustomerAddress.StreetName1 + c.CustomerAddress.StreetName2)
                                                                 .ThenBy(c => c.CustomerAddress.StreetNumber)
                                                                 .ToList();

                //.ThenBy(c => (c.ORD == 1 ? (c.MailAddress.StreetName1 + c.MailAddress.StreetName2) : (c.CustomerAddress.StreetName1 + c.CustomerAddress.StreetName2)))
                                                                // .ThenBy(c => (c.ORD == 1 ? (c.MailAddress.StreetNumber) : (c.CustomerAddress.StreetNumber)))
                                                                

                //pReport.AffectedCustomers_Original.Where(c => c.ORD == 2 
                //                                        && pReport.AffectedCustomers_Original.Except(new List<Customer> { c })
                //                                                                             .Any(d => d.ServicePointID == c.ServicePointID))
                //                                  .ToList()
                //                                  .ForEach(c => { c.CustomerName.MailName1 = string.Empty; c.CustomerName.MailName2 = string.Empty; });

                ArrayList pSNO_Added = new ArrayList();
                //for (int iCount = 0; iCount < pReport.AffectedCustomers_Original.Count; ++iCount)
                foreach (Customer customer in pReport.AffectedCustomers_Original)
                {
                    //Customer customer = pReport.AffectedCustomers_Original[iCount];

                    //if (pReport.AffectedCustomers_Original.Count(c => c.ServicePointID == pReport.AffectedCustomers_Original[iCount].ServicePointID) > 1)
                    //    if (pReport.AffectedCustomers_Original[iCount].ORD == 2)
                    //    {
                    //        pReport.AffectedCustomers_Original[iCount].CustomerName.MailName1 = string.Empty;
                    //        pReport.AffectedCustomers_Original[iCount].CustomerName.MailName2 = string.Empty;
                    //    }
                    //pReport.AffectedCustomers[iCount].CustomerName = (pReport.AffectedCustomers[iCount].ORD == 2) ? new Name() : pReport.AffectedCustomers[iCount].CustomerName;
                    if (pSNO_Added.Contains(customer.SNO)) continue;
                    if (pReport.AffectedCustomers_Original.Count(c => c.ServicePointID == customer.ServicePointID) > 1)
                    {
                        if (customer.ORD == 1) continue;
                        if (customer.ORD == 1)
                        {
                            pReport.AffectedCustomers.Add(customer);
                            pSNO_Added.Add(customer.SNO);

                            Customer cTemp = pReport.AffectedCustomers_Original.First(c => c.ServicePointID == customer.ServicePointID && c.ORD != customer.ORD);

                            if (!pReport.ExcludeCustomer_Checked)
                            {
                                cTemp.CustomerName.MailName1 = string.Empty;
                                cTemp.CustomerName.MailName2 = string.Empty;
                                pReport.AffectedCustomers.Add(cTemp);
                            }
                            pSNO_Added.Add(cTemp.SNO);
                        }
                        else
                        {
                            Customer cTemp = pReport.AffectedCustomers_Original.First(c => c.ServicePointID == customer.ServicePointID && c.ORD != customer.ORD);
                            pReport.AffectedCustomers.Add(cTemp);
                            pSNO_Added.Add(cTemp.SNO);

                            if (!pReport.ExcludeCustomer_Checked)
                            {
                                customer.CustomerName.MailName1 = string.Empty;
                                customer.CustomerName.MailName2 = string.Empty;
                                pReport.AffectedCustomers.Add(customer);
                            }
                            pSNO_Added.Add(customer.SNO);
                        }
                    }
                    else
                    {
                        pReport.AffectedCustomers.Add(customer);
                        pSNO_Added.Add(customer.SNO);
                    }
                }

                pSNO_Added.Clear();
                LoadConsumerstoGrid();
                #endregion

                ActivatePanel(pnlCustomerList);

                //PGEGlobal.Logger.Info("get customer button clicked");

            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                MessageBox.Show(ex.Message);
            }
            finally
            {
                this.Cursor = System.Windows.Forms.Cursors.Default;
            }
        }

        static int iSNO = 0;
        private IList<Customer> FillCustomerList(DataTable dtCustomerList)
        {
            Customer pCustomer = default(Customer);
            IList<Customer> pTracedCustomerList = default(IList<Customer>);
            try
            {
                pTracedCustomerList = new List<Customer>();
                pCustomer = new Customer();

                foreach (DataRow drCustomer in dtCustomerList.Rows)
                {
                    pCustomer = default(Customer);
                    pCustomer = new Customer();
                    pCustomer.ServicePointID = Convert.ToString(drCustomer["ServicePointID"]);
                    pCustomer.MeterNumber = Convert.ToString(drCustomer["MeterNumber"]);
                    pCustomer.CustomerType = Convert.ToString(drCustomer["CustomerType"]);
                    //pCustomer.CGC12 = (pDic_CGC_TNUM.Contains(Convert.ToString(drCustomer["CGC12"])) ? Convert.ToString(pDic_CGC_TNUM[Convert.ToString(drCustomer["CGC12"])]) : Convert.ToString(drCustomer["CGC12"]));
                    pCustomer.CGC12 = Convert.ToString(drCustomer["CGC12"]);
                    //pCustomer.TransformerOperatingNumber = Convert.ToString(drCustomer["OperatingNumber"]);
                    pCustomer.SSD = Convert.ToString(drCustomer["SourceSideDeviceId"]);
                    pCustomer.ORD = Convert.ToInt32(drCustomer["ORD"]);
                    try
                    {
                        pCustomer.CustomerName.MailName1 = Convert.ToString(drCustomer["MAILNAME1"]);
                        pCustomer.CustomerName.MailName2 = Convert.ToString(drCustomer["MAILNAME2"]);
                        pCustomer.PhoneNumber.PhoneNumber = Convert.ToString(drCustomer["PHONENUM"]);
                        pCustomer.CustomerAccountNumber = Convert.ToString(drCustomer["AccountID"]);
                    }
                    catch (Exception)
                    { }

                    try
                    {
                        pCustomer.Division = Convert.ToString(drCustomer["Division"]);
                    }
                    catch (Exception)
                    {
                        pCustomer.Division = PGEGlobal.WIZARD_DIVISION;
                    }

                    //Address pCustomerAddress = new Address();

                    //if (dtCustomerList.Columns.Contains("City"))
                    //{
                    //    pCustomerAddress.City = Convert.ToString(drCustomer["City"]);
                    //    pCustomerAddress.State = Convert.ToString(drCustomer["State"]);
                    //    pCustomerAddress.StreetName1 = Convert.ToString(drCustomer["StreetName1"]);
                    //    pCustomerAddress.StreetName2 = Convert.ToString(drCustomer["StreetName2"]);
                    //    pCustomerAddress.StreetNumber = Convert.ToString(drCustomer["StreetNumber"]);
                    //    pCustomerAddress.ZIPCode = Convert.ToString(drCustomer["Zip"]);
                    //    pCustomer.CustomerAddress = pCustomerAddress;
                    //}
                    //else
                    //{
                    //    pCustomerAddress.City = "";
                    //    pCustomerAddress.State = "";
                    //    pCustomerAddress.StreetName1 = "";
                    //    pCustomerAddress.StreetName2 = "";
                    //    pCustomerAddress.StreetNumber = "";
                    //    pCustomerAddress.ZIPCode = "";
                    //    pCustomer.CustomerAddress = new Address();
                    //}

                    Address pAddress = new Address();
                    if (dtCustomerList.Columns.Contains("MailStreetName1"))
                    {
                        pAddress.City = Convert.ToString(drCustomer["MailCity"]);
                        pAddress.State = Convert.ToString(drCustomer["MailState"]);
                        pAddress.StreetName1 = Convert.ToString(drCustomer["MailStreetName1"]);
                        pAddress.StreetName2 = Convert.ToString(drCustomer["MailStreetName2"]);
                        pAddress.StreetNumber = Convert.ToString(drCustomer["MailStreetNum"]);
                        pAddress.ZIPCode = Convert.ToString(drCustomer["MailZipCode"]);
                    }
                    else
                    {
                        pAddress.City = "";
                        pAddress.State = "";
                        pAddress.StreetName1 = "";
                        pAddress.StreetName2 = "";
                        pAddress.StreetNumber = "";
                        pAddress.ZIPCode = "";
                    }
                    if (pCustomer.ORD == 1)
                    {
                        pCustomer.MailAddress = pAddress;
                        pCustomer.CustomerAddress = new Address();
                    }
                    else
                    {
                        pCustomer.CustomerAddress = pAddress;
                        pCustomer.MailAddress = new Address();
                    }

                    pCustomer.SNO = (++iSNO);
                    pTracedCustomerList.Add(pCustomer);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return pTracedCustomerList;
        }

        private string GetCurrentVersionName(string FeatureClass)
        {
            string sReturn = string.Empty;
            try
            {
                MapUtility pMapUtility = new MapUtility();
                PGEFeatureClass objPGEFeatClass = pMapUtility.GetFeatureClassByName(FeatureClass);
                if (objPGEFeatClass != null)
                {
                    if (objPGEFeatClass.FeatureClass != null)
                    {
                        IVersion pVersion = null;
                        //if (pFeatureClass != null)
                        {
                            pVersion = objPGEFeatClass.FeatureClass as IVersion;
                            IVersionedWorkspace pVersionWS = objPGEFeatClass.FeatureClass.FeatureDataset.Workspace as IVersionedWorkspace;
                            pVersion = pVersionWS as IVersion;
                        }
                        sReturn = pVersion.VersionName;
                    }

                    #region Commented by SB on 02-18-2016
                    //if (objPGEFeatClass.LayerAddedToMap)
                    //{
                    //    if (objPGEFeatClass.FeatureClass != null)
                    //    {
                    //        //IFeatureLayer pFeatureLayer =  //pMapUtility.GetFeatureLayerfromFCName("EDGIS.Transformer");
                    //        //IFeatureClass pFeatureClass = null;
                    //        //if (pFeatureLayer == null)
                    //        //{
                    //        //    return sReturn;
                    //        //}
                    //        //if (pFeatureLayer != null)
                    //        //    pFeatureClass = pFeatureLayer.FeatureClass;

                    //        //pFeatureClass = objPGEFeatClass
                    //        IVersion pVersion = null;
                    //        //if (pFeatureClass != null)
                    //        {
                    //            pVersion = objPGEFeatClass.FeatureClass as IVersion;
                    //            IVersionedWorkspace pVersionWS = objPGEFeatClass.FeatureClass.FeatureDataset.Workspace as IVersionedWorkspace;
                    //            pVersion = pVersionWS as IVersion;
                    //        }
                    //        sReturn = pVersion.VersionName;
                    //    }
                    //}
                    //else
                    //{
                    //    IVersion pVersion = null;
                    //    //if (pFeatureClass != null)
                    //    {
                    //        pVersion = objPGEFeatClass.FeatureClass as IVersion;
                    //        IVersionedWorkspace pVersionWS = objPGEFeatClass.FeatureClass.FeatureDataset.Workspace as IVersionedWorkspace;
                    //        pVersion = pVersionWS as IVersion;
                    //    }
                    //    sReturn = pVersion.VersionName;
                    //    //throw new Exception("Unable to read current edit version as Transformer layer is not available in TOC");
                    //}
                    #endregion Commented by SB on 02-18-2016
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return sReturn;
        }

        private void LoadConsumerstoGrid()
        {
            try
            {
                dgCustomerList.Rows.Clear();
                //int icountEx = 0;
                //foreach (Customer customer in pReport.AffectedCustomers)
                //List<Customer> pList_Customers = ReportUtilities.SortedCustomers(pReport).Cast<Customer>().ToList();
                //List<Customer> pList_Customers = pReport.AffectedCustomers.ToList();
                //for (int iCount = 0; iCount < pList_Customers.Count; ++iCount)

                //int iLength_Name = pReport.AffectedCustomers.Max(c => c.CustomerName.MailName1.Length + c.CustomerName.MailName2.Length) + 5;
                //int iLength_Name = pReport.AffectedCustomers.Aggregate("", (c, cur) => c.Length > (c.CustomerName.MailName1 + c.CustomerName.MailName2).Length ? c : cur) + 5;

                foreach (Customer customer in pReport.AffectedCustomers)
                {
                    //Customer customer = pList_Customers[iCount];
                    if (customer.Excluded) continue;
                    string sAddressTemp = string.Empty, sNameTemp = string.Empty;
                    sNameTemp += customer.CustomerName.MailName1;
                    sNameTemp += (!string.IsNullOrEmpty(sNameTemp.Trim()) ? " " : "") + customer.CustomerName.MailName2;
                    //sNameAddressTemp += " ";
                    //sNameAddressTemp += customer.CustomerName.MailName2;

                    //int iLength_Name_Temp = sNameAddressTemp.Length;
                    //for (int iCount = 0; iCount < iLength_Name - iLength_Name_Temp; ++iCount) sNameAddressTemp += " ";

                    if (customer.ORD == 1)
                    {
                        sAddressTemp += customer.MailAddress.StreetNumber;
                        sAddressTemp += (!string.IsNullOrEmpty(sAddressTemp.Trim()) ? " " : "") + customer.MailAddress.StreetName1;
                        sAddressTemp += (!string.IsNullOrEmpty(sAddressTemp.Trim()) ? " " : "") + customer.MailAddress.StreetName2;
                        sAddressTemp += (!string.IsNullOrEmpty(sAddressTemp.Trim()) ? ", " : "") + customer.MailAddress.City;
                        sAddressTemp += (!string.IsNullOrEmpty(sAddressTemp.Trim()) && !string.IsNullOrEmpty(customer.MailAddress.City.Trim()) ? ", " : "") + customer.MailAddress.State;
                        sAddressTemp += (!string.IsNullOrEmpty(sAddressTemp.Trim()) ? " " : "") + customer.MailAddress.ZIPCode;
                    }
                    else
                    {
                        sAddressTemp += customer.CustomerAddress.StreetNumber;
                        sAddressTemp += (!string.IsNullOrEmpty(sAddressTemp.Trim()) ? " " : "") + customer.CustomerAddress.StreetName1;
                        sAddressTemp += (!string.IsNullOrEmpty(sAddressTemp.Trim()) ? " " : "") + customer.CustomerAddress.StreetName2;
                        sAddressTemp += (!string.IsNullOrEmpty(sAddressTemp.Trim()) ? ", " : "") + customer.CustomerAddress.City;
                        sAddressTemp += (!string.IsNullOrEmpty(sAddressTemp.Trim()) && !string.IsNullOrEmpty(customer.CustomerAddress.City.Trim()) ? ", " : "") + customer.MailAddress.State;
                        sAddressTemp += (!string.IsNullOrEmpty(sAddressTemp.Trim()) ? " " : "") + customer.CustomerAddress.ZIPCode;
                    }
                    sAddressTemp = sAddressTemp.TrimEnd(new char[] { ',', ' ' }).Replace(" ,", ",").Replace("  ", " ");
                    dgCustomerList.Rows.Add(!customer.Excluded, customer.CustomerType, customer.MeterNumber, sNameTemp, sAddressTemp, customer.ServicePointID, customer.SNO);
                    //icountEx += customer.Excluded ? 0 : 1;
                }
                SetLabelCount();
                dgCustomerList.SuspendLayout();
                dgCustomerList.ResumeLayout();
                btncustlistNext.Enabled = Convert.ToInt32(lblcustlistMeterNo.Text) > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void SetLabelCount()
        {
            lblcustlistMeterNo.Text = pReport.AffectedCustomers.Where(c => !c.Excluded).Select(c => c.ServicePointID).Distinct().Count().ToString();
            lblcustlistLetterNo.Text = pReport.AffectedCustomers.Where(c => !c.Excluded).Count().ToString();
        }

        private void Demo(string sPathCSV)
        {
            string[] sUsers = File.ReadAllLines(sPathCSV);
            pReport.AffectedCustomers = new List<Customer>();
            foreach (string sUser in sUsers)
            {
                string[] sValue = sUser.Split(',');
                pReport.AffectedCustomers.Add(new Customer() { CGC12 = sValue[1], CustomerAccountNumber = sValue[0], CustomerAddress = new Address() { City = sValue[8], State = sValue[7], StreetName1 = sValue[4], ZIPCode = sValue[9] }, CustomerName = new Name() { MailName1 = sValue[3] }, CustomerType = sValue[2], Excluded = false, MeterNumber = sValue[10], PhoneNumber = new Phone() { AreaCode = "123", PhoneNumber = "456 7890" }, ServicePointID = sValue[5] });
            }

            pReport.SubmitterID = "S2NN";
        }

        private void btnCustListPrevious_Click(object sender, EventArgs e)
        {
            if (pReport.AffectedCustomers.Count != pReport.AffectedCustomers.Count(c => !c.Excluded))
            {
                if (MessageBox.Show("You have marked customer to be excluded, if you go to the previous screen, these customers will no longer be excluded. Do you want to continue?", "PONS", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.Yes)
                    ActivatePanel(pnlSearch);
            }
            else
            {
                ActivatePanel(pnlSearch);
            }
                
            //pnlSearch.Visible = true;
        }
        DataSet pPSC = new DataSet();
        private void btnWelcomeClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private static string sPnlActivated = string.Empty;
        private void ActivatePanel(Panel p)
        {
            //this.Width = 740;
            //this.Height = 400;
            pnlWelcome.Enabled = pnlWelcome.Visible = false;
            pnlSearch.Enabled = pnlSearch.Visible = false;
            pnlCustomerList.Enabled = pnlCustomerList.Visible = false;
            pnlOutageInfo.Enabled = pnlOutageInfo.Visible = false;
            pnlCustomerNotification.Enabled = pnlCustomerNotification.Visible = false;
            pnlCustomerNotificationPrint.Enabled = pnlCustomerNotificationPrint.Visible = false;
            pnlEmail.Enabled = pnlEmail.Visible = false;
            pnlAveryLabel.Enabled = pnlAveryLabel.Visible = false;

            p.Enabled = p.Visible = true;
            sPnlActivated = p.Name;
            p.Dock = DockStyle.Fill;
            FocusControlPanel();
        }

        private void FocusControlPanel()
        {
            switch (sPnlActivated)
            {
                case "pnlWelcome":
                    this.cmbDivisionWelcome.Focus();
                    this.AcceptButton = btnWelcomeProceed;
                    break;
                case "pnlSearch":
                    if (rbBetweenDevicesSearch.Checked)
                        this.txtStartDevice.Focus();
                    else if (rbTransformerSearch.Checked)
                        this.txtTransformer.Focus();
                    else if (rbSubCircuitSearch.Checked)
                        this.cmbSubstation.Focus();
                    this.AcceptButton = (rbBetweenDevicesSearch.Checked || rbTransformerSearch.Checked || rbSubCircuitSearch.Checked) ? btnSearchAdd : btnGetCustomer;
                    break;
                case "pnlCustomerList":
                    this.dgCustomerList.Focus();
                    this.AcceptButton = btncustlistNext;
                    break;
                case "pnlOutageInfo":
                    this.dtpshutdowndate.Focus();
                    this.AcceptButton = btnoutageNext;
                    break;
                case "pnlCustomerNotification":
                    this.btncustNext.Focus();
                    this.AcceptButton = btncustNext;
                    break;
                case "pnlCustomerNotificationPrint":
                    this.btnPrint.Focus();
                    this.AcceptButton = btnPrintNext;
                    break;
                case "pnlEmail":
                    this.cmbPSCUserList.Focus();
                    this.AcceptButton = btnEmailNext;
                    break;
                case "pnlAveryLabel":
                    this.txtLabelsFirstSheet.Focus();
                    this.AcceptButton = btnAveryPrint;
                    break;
            }
        }

        private void PONSHomeScreen_Load(object sender, EventArgs e)
        {
            try
            {
                //AcceptButtonCreate();
                //this.StartPosition = FormStartPosition.CenterScreen;
                //int X = this.Location.X, Y = this.Location.Y;
                //this.Width = 740;
                //this.Height = 400;
                objMapUtility = new MapUtility();
                ActivatePanel(pnlWelcome);
                //this.Location = new System.Drawing.Point() { X = X, Y = Y };
                ConfigurationHelper configurationHelper = new ConfigurationHelper(/*PGEGlobal.Logger*/);
                divisionList.Add("Select…");
                divisionList.AddRange(configurationHelper.GetListSeparatedbyComma("DivisionList"));
                cmbDivisionWelcome.Items.Clear();
                cmbDivisionWelcome.Items.AddRange(divisionList.ToArray());
                dtpshutdowndate.Value = DateTime.Today.AddDays(1);

                rbBetweenDevicesSearch.Checked = true;
                cmbDivisionWelcome.Enabled = true;
                if (PGEGlobal.WIZARD_DIVISION.Length == 0)
                {
                    cmbDivisionWelcome.SelectedIndex = 0;
                    //cmbDivisionWelcome.Enabled = true;
                }
                else
                {
                    selectfirstdropdownentry(PGEGlobal.WIZARD_DIVISION, cmbDivisionWelcome);
                    if (cmbDivisionWelcome.SelectedIndex > 0)
                    {
                        //cmbDivisionWelcome.Enabled = false;
                    }
                }
                this.btnDeleteCustSearch.Enabled = false;
                this.btnGetCustomer.Enabled = false;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                MessageBox.Show(ex.Message);
            }
        }

        private void AcceptButtonCreate()
        {
            Button btnFormAccept = new Button()
            {
                Visible = false,
                Text = "Accept Button",
                Location = new System.Drawing.Point(0, 0),
                Size = new System.Drawing.Size(0, 0)
            };
            btnFormAccept.Click += new EventHandler(btnFormAccept_Click);
            btnFormAccept.KeyUp += new KeyEventHandler(btnFormAccept_Click);
            this.Controls.Add(btnFormAccept);
            this.AcceptButton = btnFormAccept;
        }

        void btnFormAccept_Click(object sender, EventArgs e)
        {
            switch (sPnlActivated)
            {
                case "pnlWelcome":
                    if (btnWelcomeProceed.Enabled)
                        btnWelcomeProceed_Click(sender, e);
                    break;
                case "pnlSearch":
                    if (txtStartDevice.Focused || txtEndDevice.Focused || txtTransformer.Focused || cmbSubstation.Focused || cmbBank.Focused || cmbCircuit.Focused)
                        if (btnSearchAdd.Enabled)
                            btnSearchAdd_Click(sender, e);
                        else
                            if (btnGetCustomer.Enabled)
                                btnGetCustomer_Click(sender, e);
                    break;
                case "pnlCustomerList":
                    if (btncustlistNext.Enabled)
                        btncustNext_Click(sender, e);
                    break;
                case "pnlOutageInfo":
                    if (btnoutageNext.Enabled)
                        btnoutageOk_Click(sender, e);
                    break;
                case "pnlCustomerNotification":
                    if (btncustNext.Enabled)
                        btncustNext_Click(sender, e);
                    break;
                case "pnlCustomerNotificationPrint":
                    if (btnPrintNext.Enabled)
                        btnPrintNext_Click(sender, e);
                    break;
                case "pnlEmail":
                    if (btnEmailNext.Enabled)
                        btnEmailNext_Click(sender, e);
                    break;
                case "pnlAveryLabel":
                    if (btnAveryPrint.Enabled)
                        btnPrintaverylebelprint_Click(sender, e);
                    break;
            }
        }

        private void selectfirstdropdownentry(string itemtext, ComboBox cmb)
        {
            try
            {
                if (cmb.Items.Count < 2)
                {
                    cmb.SelectedIndex = 0;
                }
                else
                {
                    cmb.SelectedIndex = 0;
                    int i = 0;
                    for (i = 0; i < cmb.Items.Count; i++)
                    {
                        if (cmb.Items[i].ToString().ToUpper() == itemtext.ToUpper())
                        {
                            cmb.SelectedIndex = i;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                throw ex;
            }
        }

        private void btnoutageOk_Click(object sender, EventArgs e)
        {
            try
            {
                if (dtpshutdowntimeon.Value.TimeOfDay <= dtpshutdowntimeoff.Value.TimeOfDay ||
                    (chkAdditional1.Checked && (dtpshutdowntimeon2.Value.TimeOfDay <= dtpshutdowntimeoff2.Value.TimeOfDay)))
                {
                    //MessageBox.Show("Time ON cannot be less than or equal to Time Off", "PONS", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    //return;
                }
                pReport.OutageDetails.Clear();
                pReport.OutageDetails.Add(new OutageTimeDetail() { Outage_Date_OFF = dtpshutdowndate.Value.ToLongDateString(), OFF_Time = dtpshutdowntimeoff.Value.ToShortTimeString(), /*Outage_Date_ON = dtpshutdowndateON.Value.ToLongDateString(),*/ ON_Time = dtpshutdowntimeon.Value.ToShortTimeString() });
                if (chkAdditional1.Checked)
                {
                    pReport.OutageDetails.Add(new OutageTimeDetail() { Outage_Date_OFF = dtpshutdowndate2.Value.ToLongDateString(), OFF_Time = dtpshutdowntimeoff2.Value.ToShortTimeString(), /*Outage_Date_ON = dtpshutdowndateON2.Value.ToLongDateString(),*/ ON_Time = dtpshutdowntimeon2.Value.ToShortTimeString() });
                }
                pReport.Description = txtDescription.Text;
                ActivatePanel(pnlCustomerNotification);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                MessageBox.Show(ex.Message);
            }
        }

        private void btncustNext_Click(object sender, EventArgs e)
        {
            try
            {
                if (chkPSC.Checked)
                {
                    LoadPSCList();
                    chkCCMe.Checked = pReport.CC_Me_Mail;
                    ActivatePanel(pnlEmail);
                    btnEmailNext.Enabled = chkCLR.Checked || chkAddress.Checked;
                }
                else if (chkCLR.Checked)
                {
                    btnPrintNext.Enabled = chkAddress.Checked;
                    ActivatePanel(pnlCustomerNotificationPrint);
                }

                //{
                //    btnPrintNext.Enabled = chkAddress.Checked;
                //    ActivatePanel(pnlCustomerListingReport);
                //}
                else if (chkAddress.Checked)
                {
                    ActivatePanel(pnlAveryLabel);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                MessageBox.Show(ex.Message);
            }
        }

        private void LoadPSCList()
        {
            try
            {
                //if (!String.IsNullOrEmpty(cmbPSCUserList.Text)) return;
                if (cmbPSCUserList.Items.Count > 0) return;
                if (pPSC == null || pPSC.Tables.Count == 0)
                    if ((pPSC = ReportUtilities.PSCNameList()) == null) return;

                //DataRow[] pDRColl = pPSC.Tables[0].Select("Division = '" + cmbDivisionWelcome.Text + "'");
                Dictionary<string, string> combosource = new Dictionary<string, string>();

                //DataRow[] pDRColl = pPSC.Tables[0].Select(string.Empty, "LANID ASC");
                DataRow[] pDRColl = pPSC.Tables[0].Select();
                if (pDRColl.Length == 0) pDRColl = pPSC.Tables[0].Select();
                foreach (DataRow pRow in pDRColl)
                {
                    combosource.Add(pRow["LANID"].ToString().Trim() + "," + pRow["ContactID"].ToString().Trim(), "[" + pRow["Division"].ToString().Trim() + "] " + pRow["FName"] + " " + pRow["LName"]);
                    //cmbPSCUserList.Items.Add(pRow["FName"] + " " + pRow["LName"] + " [" + pRow["Division"].ToString().Trim() + "]");
                    //cmbPSCUserList.Sorted = true;
                }

                cmbPSCUserList.DataSource = new BindingSource(combosource, null);
                cmbPSCUserList.DisplayMember = "Value";
                cmbPSCUserList.ValueMember = "Key";

                cmbPSCUserList.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void btnEmailPrev_Click(object sender, EventArgs e)
        {
            ActivatePanel(pnlCustomerNotification);
        }

        private void btncustlistNext_Click(object sender, EventArgs e)
        {
            ActivatePanel(pnlOutageInfo);
        }

        private void btnAveryPrevious_Click(object sender, EventArgs e)
        {
            if (chkCLR.Checked)
            {
                btnPrintNext.Enabled = chkAddress.Checked;
                ActivatePanel(pnlCustomerNotificationPrint);
            }
            else if (chkPSC.Checked)
                ActivatePanel(pnlEmail);
            else
                ActivatePanel(pnlCustomerNotification);
        }

        private void btnSearchAdd_Click(object sender, EventArgs e)
        {
            try
            {
                string CircuitID = string.Empty;
                if (this.rbBetweenDevicesSearch.Checked)
                {
                    this.txtStartDevice.Text = this.txtStartDevice.Text.ToUpper();
                    this.txtEndDevice.Text = this.txtEndDevice.Text.ToUpper();
                    if (string.IsNullOrEmpty(this.txtStartDevice.Text))
                    {
                        MessageBox.Show("Start device ID is mandatory!");
                        return;
                    }

                    UtilityFunctions objUtilityFunctions = new UtilityFunctions();
                    string[] sDeviceFeatureClasses = objUtilityFunctions.ReadConfigurationValue("SearchBwDevicePointClasses").Split(';'),
                    sDeviceFCPrimaryFields = objUtilityFunctions.ReadConfigurationValue("SearchBwDevicePointClassUniqueFields").Split(';');

                    if (ValidateStartEndDevices(sDeviceFeatureClasses, sDeviceFCPrimaryFields))
                    {
                        bool bEndDeviceMentioned = !string.IsNullOrEmpty(this.txtEndDevice.Text);

                        int iNewRowIndex = this.dgdeviceView.Rows.Add(new object[] { bEndDeviceMentioned ? 
                            ("Customers between " + PGEGlobal.START_EQUIPMENT.ObjectClassDisplayName + " " + this.txtStartDevice.Text + " and " +
                            PGEGlobal.END_EQUIPMENT.ObjectClassDisplayName + " " + this.txtEndDevice.Text) :
                            ("Customers fed by " + PGEGlobal.START_EQUIPMENT.ObjectClassDisplayName + " " + this.txtStartDevice.Text),                        
                        PGEGlobal.START_EQUIPMENT.CircuitID, "BetweenDevicesSearch", 
                        PGEGlobal.START_EQUIPMENT.ObjectClassName + ";" + Convert.ToString(PGEGlobal.START_EQUIPMENT.ObjectID), 
                        (bEndDeviceMentioned ? PGEGlobal.END_EQUIPMENT.ObjectClassName + ";" + Convert.ToString(PGEGlobal.END_EQUIPMENT.ObjectID) : string.Empty)});

                        RemoveIfDuplicate(iNewRowIndex);
                        this.dgdeviceView.ClearSelection();
                    }
                }
                else if (this.rbTransformerSearch.Checked)
                {
                    this.txtTransformer.Text = this.txtTransformer.Text.ToUpper();
                    CircuitID = string.Empty;
                    string FeatureClass = string.Empty, FeatCGC = string.Empty;

                    if (ValidateTransformerInputValue(this.txtTransformer.Text, out CircuitID, out FeatureClass, out FeatCGC))
                    {
                        int iNewRowIndex = this.dgdeviceView.Rows.Add(new object[] {
                            "Customers on " + (FeatureClass.Contains('.') ? FeatureClass.Split('.')[1] : FeatureClass) + " " + this.txtTransformer.Text,
                        CircuitID, "TransformerSearch", FeatCGC /*this.txtTransformer.Text*/, FeatureClass});

                        RemoveIfDuplicate(iNewRowIndex);
                        this.dgdeviceView.ClearSelection();
                    }
                }
                else if (this.rbSubCircuitSearch.Checked)
                {
                    if (this.cmbCircuit.Items.Count <= 0)
                    {
                        MessageBox.Show("Please select a circuit.");
                        return;
                    }
                    if (this.cmbCircuit.SelectedIndex <= 0)
                    {
                        MessageBox.Show("Please select a circuit.");
                        return;
                    }

                    string Description = "", feederid = "", inputid1 = "";
                    UtilityFunctions objUtilityFunctions = new UtilityFunctions();
                    string istrim = objUtilityFunctions.ReadConfigurationValue("TrimCircuitID");
                    CircuitID = string.Empty;
                    if (cmbCircuit.Items.Count < 2)
                    {
                        MessageBox.Show("No Circuit found.");
                        return;
                    }
                    if (cmbSubstation.SelectedIndex < 1)
                    {
                        MessageBox.Show("Please select a substation.");
                        return;
                    }
                    //Description = "Substation:" + ((KeyValuePair<string, string>)cmbSubstation.SelectedItem).Value;
                    Description = "Substation:" + (char.IsNumber(cmbSubstation.Text, 0) ? cmbSubstation.Text.Substring(4) : cmbSubstation.Text);
                    if (cmbBank.SelectedIndex > 0)
                    {
                        //Description = Description + " Bank:" + ((KeyValuePair<string, string>)cmbBank.SelectedItem).Value;
                        Description = Description + " Bank: " + cmbBank.Text;
                    }
                    if (cmbCircuit.SelectedIndex > 0)
                    {
                        //feederid = ((KeyValuePair<string, string>)cmbCircuit.SelectedItem).Key;
                        feederid = cmbCircuit.Text;
                        Description += " Circuit: " + feederid;

                        if (istrim == "true")
                        {
                            var firstLetterints = feederid.TakeWhile(c => Char.IsDigit(c));
                            string newText = new string(firstLetterints.ToArray());
                            inputid1 = newText;
                        }
                        else
                        {
                            inputid1 = feederid;
                        }
                    }
                    else
                    {
                        feederid = "";
                        inputid1 = "";
                        for (int i = 1; i < cmbCircuit.Items.Count; i++)
                        {
                            if (istrim != "true")
                            {
                                if (inputid1 == "")
                                {
                                    inputid1 = cmbCircuit.Items[i].ToString();
                                }
                                else
                                {
                                    inputid1 = inputid1 + "," + cmbCircuit.Items[i].ToString();
                                }
                            }
                            else
                            {
                                string tmpstr = cmbCircuit.Items[i].ToString();
                                var firstLetterints = tmpstr.TakeWhile(c => Char.IsDigit(c));
                                string newText = new string(firstLetterints.ToArray());
                                if (inputid1 == "")
                                {
                                    inputid1 = newText;
                                }
                                else
                                {
                                    inputid1 = inputid1 + "," + newText;
                                }
                            }

                        }
                    }


                    int iNewRowIndex = this.dgdeviceView.Rows.Add(new object[] {Description,
                        feederid, "SubstationSearch", inputid1,""});

                    RemoveIfDuplicate(iNewRowIndex);
                    this.dgdeviceView.ClearSelection();
                }
                if (this.dgdeviceView.Rows.Count > 0)
                {
                    this.btnDeleteCustSearch.Enabled = true;
                    this.btnGetCustomer.Enabled = true;
                }
                this.txtTransformer.Text = string.Empty;
                this.txtStartDevice.Text = string.Empty;
                this.txtEndDevice.Text = string.Empty;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                MessageBox.Show(ex.Message);
            }

        }

        private void RemoveIfDuplicate(int iNewRowIndex)
        {
            try
            {
                for (int iRowIndex = 0; iRowIndex < this.dgdeviceView.RowCount; iRowIndex++)
                {
                    if (iRowIndex == iNewRowIndex)
                        continue;
                    int iColIndex = 0;
                    for (; iColIndex < this.dgdeviceView.ColumnCount; iColIndex++)
                        if (!this.dgdeviceView.Rows[iRowIndex].Cells[iColIndex].Value.Equals(this.dgdeviceView.Rows[iNewRowIndex].Cells[iColIndex].Value))
                            break;

                    if (iColIndex == this.dgdeviceView.ColumnCount)
                    {
                        this.dgdeviceView.Rows.RemoveAt(iNewRowIndex);
                        return;
                    }
                }
            }
            catch (Exception ex)
            { throw ex; }
        }

        private bool ValidateStartEndDevices(string[] DeviceFeatureClasses, string[] DeviceFCPrimaryFields)
        {
            bool bReturn = false;

            #region Variable declaration
            PGEGlobal.START_EQUIPMENT = PGEGlobal.END_EQUIPMENT = default(Equipment);
            UtilityFunctions objUtilityFunctions = new UtilityFunctions();
            string sPrimaryField = string.Empty, sFeatureClassName = string.Empty, sPrimaryValue = string.Empty;
            bool bEndDeviceMentioned = false;
            IList<Equipment> objStartEquipmentList = new List<Equipment>(), objEndEquipmentList = new List<Equipment>();
            Equipment pEquipment = default(Equipment);
            MapUtility objMapUtility = new MapUtility();
            PGEFeatureClass objPGEFeatClass = null;
            IFeatureCursor pFeatureCursor = default(IFeatureCursor);
            IFeature pFeature = default(IFeature);
            IQueryFilter pQueryFilter = default(IQueryFilter);
            #endregion Variable declaration

            try
            {
                if (!string.IsNullOrEmpty(this.txtEndDevice.Text))
                    bEndDeviceMentioned = true;

                for (int iArrayIndex = 0; iArrayIndex < DeviceFeatureClasses.Length; iArrayIndex++)
                {
                    sFeatureClassName = DeviceFeatureClasses[iArrayIndex];
                    sPrimaryField = DeviceFCPrimaryFields[iArrayIndex];

                    if (objMapUtility == null)
                        continue;

                    objPGEFeatClass = objMapUtility.GetFeatureClassByName(sFeatureClassName);
                    if (objPGEFeatClass == null)
                        continue;
                    if (objPGEFeatClass.FeatureClass == null)
                        continue;

                    pQueryFilter = new QueryFilterClass();
                    pQueryFilter.WhereClause = "DIVISION = '" + Convert.ToInt32(PGEGlobal.WIZARD_DIVISION_CODE) + "' AND ";

                    if (bEndDeviceMentioned)
                        pQueryFilter.WhereClause += sPrimaryField + " in ('" + this.txtStartDevice.Text + "', '" + this.txtEndDevice.Text + "')";
                    else
                        pQueryFilter.WhereClause += sPrimaryField + " = '" + this.txtStartDevice.Text + "'";

                    pFeatureCursor = objPGEFeatClass.FeatureClass.Search(pQueryFilter, false);

                    pFeature = pFeatureCursor.NextFeature();
                    while (pFeature != null)
                    {
                        pEquipment = new Equipment();
                        pEquipment.CircuitID = Convert.ToString(pFeature.get_Value(objPGEFeatClass.FeatureClass.FindField(
                            objUtilityFunctions.ReadConfigurationValue("FieldCircuitID"))));
                        pEquipment.CircuitID2 = Convert.ToString(pFeature.get_Value(objPGEFeatClass.FeatureClass.FindField(
                            objUtilityFunctions.ReadConfigurationValue("FieldCircuitID2"))));
                        pEquipment.ObjectClassDisplayName = objPGEFeatClass.FeatureClass.AliasName;
                        pEquipment.ObjectClassName = sFeatureClassName;
                        pEquipment.ObjectID = pFeature.OID;

                        sPrimaryValue = Convert.ToString(pFeature.get_Value(objPGEFeatClass.FeatureClass.FindField(sPrimaryField)));
                        pEquipment.ObjectDisplayName = sPrimaryValue;

                        if (bEndDeviceMentioned)
                        {
                            if (this.txtStartDevice.Text.Equals(sPrimaryValue))
                                objStartEquipmentList.Add(pEquipment);

                            if (this.txtEndDevice.Text.Equals(sPrimaryValue))
                                objEndEquipmentList.Add(pEquipment);
                        }
                        else
                            objStartEquipmentList.Add(pEquipment);

                        pFeature = pFeatureCursor.NextFeature();
                    }

                    objPGEFeatClass.Dispose();
                }

                if ((objStartEquipmentList.Count <= 0) || (bEndDeviceMentioned && objEndEquipmentList.Count <= 0))
                {
                    IList<Equipment> objStartLinearEquipmentList = null, objEndLinearEquipmentList = null;

                    if (objStartEquipmentList.Count <= 0)
                        objStartLinearEquipmentList = new List<Equipment>();
                    if (objEndEquipmentList.Count <= 0)
                        objEndLinearEquipmentList = new List<Equipment>();

                    DeviceFeatureClasses = objUtilityFunctions.ReadConfigurationValue("SearchBwDevicesLinearClasses").Split(';');
                    DeviceFCPrimaryFields = objUtilityFunctions.ReadConfigurationValue("SearchBwDevicesLinearClassUniqueFields").Split(';');

                    for (int iArrayIndex = 0; iArrayIndex < DeviceFeatureClasses.Length; iArrayIndex++)
                    {
                        sFeatureClassName = DeviceFeatureClasses[iArrayIndex];
                        sPrimaryField = DeviceFCPrimaryFields[iArrayIndex];

                        if (objMapUtility == null)
                            continue;

                        objPGEFeatClass = objMapUtility.GetFeatureClassByName(sFeatureClassName);
                        if (objPGEFeatClass == null)
                            continue;
                        if (objPGEFeatClass.FeatureClass == null)
                            continue;

                        long num = 0;
                        if (sPrimaryField.Equals(objPGEFeatClass.FeatureClass.OIDFieldName))
                            if (!Int64.TryParse(this.txtStartDevice.Text, out num))
                                continue;

                        if (objStartEquipmentList.Count <= 0)
                        {
                            {
                                pQueryFilter = new QueryFilterClass();
                                pQueryFilter.WhereClause = "DIVISION = '" + Convert.ToInt32(PGEGlobal.WIZARD_DIVISION_CODE) + "' AND " +
                                    sPrimaryField + " = '" + this.txtStartDevice.Text + "'";

                                pFeatureCursor = objPGEFeatClass.FeatureClass.Search(pQueryFilter, false);

                                pFeature = pFeatureCursor.NextFeature();
                                while (pFeature != null)
                                {
                                    pEquipment = new Equipment();
                                    pEquipment.CircuitID = Convert.ToString(pFeature.get_Value(objPGEFeatClass.FeatureClass.FindField(
                                        objUtilityFunctions.ReadConfigurationValue("FieldCircuitID"))));
                                    pEquipment.CircuitID2 = Convert.ToString(pFeature.get_Value(objPGEFeatClass.FeatureClass.FindField(
                                        objUtilityFunctions.ReadConfigurationValue("FieldCircuitID2"))));
                                    pEquipment.ObjectClassDisplayName = objPGEFeatClass.FeatureClass.AliasName;
                                    pEquipment.ObjectClassName = sFeatureClassName;
                                    pEquipment.ObjectID = pFeature.OID;

                                    sPrimaryValue = Convert.ToString(pFeature.get_Value(objPGEFeatClass.FeatureClass.FindField(sPrimaryField)));
                                    pEquipment.ObjectDisplayName = sPrimaryValue;

                                    objStartLinearEquipmentList.Add(pEquipment);

                                    pFeature = pFeatureCursor.NextFeature();
                                }
                            }
                        }

                        if (bEndDeviceMentioned && objEndEquipmentList.Count <= 0)
                        {
                            {
                                pQueryFilter = new QueryFilterClass();
                                pQueryFilter.WhereClause = "DIVISION = '" + Convert.ToInt32(PGEGlobal.WIZARD_DIVISION_CODE) + "' AND " +
                                    sPrimaryField + " = '" + this.txtEndDevice.Text + "'";

                                pFeatureCursor = objPGEFeatClass.FeatureClass.Search(pQueryFilter, false);

                                pFeature = pFeatureCursor.NextFeature();
                                while (pFeature != null)
                                {
                                    pEquipment = new Equipment();
                                    pEquipment.CircuitID = Convert.ToString(pFeature.get_Value(objPGEFeatClass.FeatureClass.FindField(
                                        objUtilityFunctions.ReadConfigurationValue("FieldCircuitID"))));
                                    pEquipment.CircuitID2 = Convert.ToString(pFeature.get_Value(objPGEFeatClass.FeatureClass.FindField(
                                        objUtilityFunctions.ReadConfigurationValue("FieldCircuitID2"))));
                                    pEquipment.ObjectClassDisplayName = objPGEFeatClass.FeatureClass.AliasName;
                                    pEquipment.ObjectClassName = sFeatureClassName;
                                    pEquipment.ObjectID = pFeature.OID;

                                    sPrimaryValue = Convert.ToString(pFeature.get_Value(objPGEFeatClass.FeatureClass.FindField(sPrimaryField)));
                                    pEquipment.ObjectDisplayName = sPrimaryValue;

                                    objEndLinearEquipmentList.Add(pEquipment);

                                    pFeature = pFeatureCursor.NextFeature();
                                }
                            }
                        }
                        objPGEFeatClass.Dispose();
                    }

                    if (objStartEquipmentList.Count <= 0)
                        if (objStartLinearEquipmentList.Count > 0)
                        {
                            ((List<Equipment>)objStartEquipmentList).AddRange(objStartLinearEquipmentList);
                            objStartLinearEquipmentList.Clear();
                        }

                    if (bEndDeviceMentioned && objEndEquipmentList.Count <= 0)
                        if (objEndLinearEquipmentList.Count > 0)
                        {
                            ((List<Equipment>)objEndEquipmentList).AddRange(objEndLinearEquipmentList);
                            objEndLinearEquipmentList.Clear();
                        }
                }

                frmConfirmation objFrmConfirmation = default(frmConfirmation);

                if (bEndDeviceMentioned)
                {
                    if (objStartEquipmentList.Count <= 0 && objEndEquipmentList.Count <= 0)
                    {
                        MessageBox.Show("Start and End devices not found!");
                        return bReturn;
                    }

                    if (objEndEquipmentList.Count <= 0)
                    {
                        MessageBox.Show("End device not found!");
                        return bReturn;
                    }
                }

                if (objStartEquipmentList.Count <= 0)
                {
                    MessageBox.Show("Start device not found!");
                    return bReturn;
                }

                if (objStartEquipmentList.Count > 1)
                {
                    objFrmConfirmation = default(frmConfirmation);
                    objFrmConfirmation = new frmConfirmation(objStartEquipmentList, "START");
                    //objFrmConfirmation.ShowDialog();
                    if (objFrmConfirmation.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                        return bReturn;
                }
                else if (objStartEquipmentList.Count == 1)
                {
                    PGEGlobal.START_EQUIPMENT = objStartEquipmentList[0];
                }

                if (Convert.ToBoolean(objUtilityFunctions.ReadConfigurationValue("SameCircuitEndDevice")) && objEndEquipmentList.Count > 0)
                {
                    IList<Equipment> objEndEquipmentListTemp = new List<Equipment>();
                    foreach (Equipment pEquip in objEndEquipmentList)
                    {
                        if (pEquip.CircuitID.Equals(PGEGlobal.START_EQUIPMENT.CircuitID))
                        {
                            objEndEquipmentListTemp.Add(pEquip);
                            continue;
                        }

                        if (!string.IsNullOrEmpty(pEquip.CircuitID2))
                            if (PGEGlobal.START_EQUIPMENT.CircuitID.Equals(pEquip.CircuitID2))
                            {
                                objEndEquipmentListTemp.Add(pEquip);
                                continue;
                            }

                        if (!string.IsNullOrEmpty(PGEGlobal.START_EQUIPMENT.CircuitID2))
                        {
                            if (PGEGlobal.START_EQUIPMENT.CircuitID2.Equals(pEquip.CircuitID))
                            {
                                objEndEquipmentListTemp.Add(pEquip);
                                continue;
                            }

                            if (!string.IsNullOrEmpty(pEquip.CircuitID2))
                                if (PGEGlobal.START_EQUIPMENT.CircuitID2.Equals(pEquip.CircuitID2))
                                {
                                    objEndEquipmentListTemp.Add(pEquip);
                                    continue;
                                }
                        }
                    }

                    if (objEndEquipmentListTemp.Count <= 0)
                    {
                        MessageBox.Show("No End device found on same circuit as Start device. Please try again!");
                        return bReturn;
                    }

                    objEndEquipmentList.Clear();
                    ((List<Equipment>)objEndEquipmentList).AddRange(objEndEquipmentListTemp);
                }

                if (objEndEquipmentList.Count > 1)
                {
                    objFrmConfirmation = default(frmConfirmation);
                    objFrmConfirmation = new frmConfirmation(objEndEquipmentList, "END");
                    //objFrmConfirmation.ShowDialog();
                    if (objFrmConfirmation.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                        return bReturn;
                }
                else if (objEndEquipmentList.Count == 1)
                {
                    PGEGlobal.END_EQUIPMENT = objEndEquipmentList[0];
                }

                if (bEndDeviceMentioned && (!PGEGlobal.START_EQUIPMENT.CircuitID.ToUpper().Equals(PGEGlobal.END_EQUIPMENT.CircuitID.ToUpper())))
                    MessageBox.Show("Start and End devices do not belong to same ciruit");
                else
                    bReturn = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (pFeatureCursor != null) { Marshal.ReleaseComObject(pFeatureCursor); pFeatureCursor = default(IFeatureCursor); }
                if (pFeature != null) { Marshal.ReleaseComObject(pFeature); pFeature = default(IFeature); }
                if (pQueryFilter != null) { Marshal.ReleaseComObject(pQueryFilter); pQueryFilter = default(IQueryFilter); }
            }

            return bReturn;
        }

        private bool ValidateTransformerInputValue(string InputText, out string CircuitID, out string FeatureClass, out string CGC)
        {
            bool bReturn = false;
            FeatureClass = string.Empty;
            CGC = string.Empty;
            try
            {
                //Int64 num;
                if (InputText.Length == 0)
                {
                    MessageBox.Show("Enter CGC/TNum number.");
                    CircuitID = "";
                    return bReturn;
                }
                //bool isNum = Int64.TryParse(InputText, out num);

                //if (!isNum)
                //{
                //    MessageBox.Show("CGC Number should be in proper format");
                //    CircuitID = "";
                //    return bReturn;
                //}

                MapUtility mu = new MapUtility();
                //  List<string> circuitids = mu.GetTransformerCircuitIds(p);
                string sFeat_CGC = string.Empty;
                List<TransformerDetails> circuitids = mu.GetTransformerCircuitIdsdetails(InputText, out FeatureClass, out sFeat_CGC);
                //if (pDic_CGC_TNUM.Contains(sFeat_CGC))
                //    pDic_CGC_TNUM.Remove(sFeat_CGC);
                //pDic_CGC_TNUM.Add(sFeat_CGC, InputText);

                if (circuitids.Count == 0)
                {
                    MessageBox.Show("CGC/TNum number does not exist");
                    CircuitID = "";
                    return bReturn;
                }
                else if (circuitids.Count == 1)
                {
                    CircuitID = circuitids[0].CIRCUITID;
                    CGC = sFeat_CGC;
                    bReturn = true;
                    return bReturn;
                }
                else
                {
                    frmConfirmation con = new frmConfirmation(InputText, circuitids);

                    if (con.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        if (PGEGlobal.SELECTED_CIRCUITID.Length != 0)
                        {
                            CircuitID = PGEGlobal.SELECTED_CIRCUITID;
                            PGEGlobal.SELECTED_CIRCUITID = "";
                            CGC = sFeat_CGC;
                            bReturn = true;
                            return bReturn;
                        }
                    }
                }

                CircuitID = "";
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return bReturn;


            //IFeatureClass pTransformerFC = (PGEGlobal.WORKSPACE as IFeatureWorkspace).OpenFeatureClass((new UtilityFunctions()).ReadConfigurationValue("TRANSFORMER_FEATURE_CLASSNAME"));
            //if (pTransformerFC != null)
            //{
            //    IQueryFilter pQueryFilter = new QueryFilterClass();
            //    pQueryFilter.WhereClause = "CGC12 = '" + p + "'";
            //    IFeatureCursor pFeatureCursor = pTransformerFC.Search(pQueryFilter, false);
            //    IFeature pTransformerFeature = pFeatureCursor.NextFeature();
            //    if (pTransformerFC.FeatureCount(pQueryFilter) > 1)
            //    {
            //        while (pTransformerFeature != null)
            //        {

            //            pTransformerFeature = pFeatureCursor.NextFeature();
            //        }
            //    }
            //    else
            //        CircuitID = Convert.ToString(pTransformerFeature.get_Value(pTransformerFeature.Fields.FindField("CIRCUITID")));
            //}

            ////throw new NotImplementedException();
            //CircuitID = "1376BK1";
            //return true;

        }

        private void btnPrintNext_Click(object sender, EventArgs e)
        {
            ActivatePanel(pnlAveryLabel);
        }

        private void btnAveryClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnPrintaverylebelprint_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                string sPath = ReportUtilities.CreateLBL(pReport, Convert.ToInt32(txtLabelsFirstSheet.Text));
                if (!string.IsNullOrEmpty(sPath))
                {
                    if (!sPath.StartsWith("P:"))
                    {
                        SaveFileDialog pSaveDialog = new SaveFileDialog();
                        pSaveDialog.AddExtension = true;
                        pSaveDialog.Filter = "XPS Document (*.xps)|*.xps";
                        pSaveDialog.Title = "Save PONS Labels to Shared Network Location";
                        DialogResult pDialogResult = pSaveDialog.ShowDialog();
                        if (pDialogResult == DialogResult.OK || pDialogResult == DialogResult.Yes)
                        {
                            try
                            {
                                if (File.Exists(pSaveDialog.FileName))
                                    File.Delete(pSaveDialog.FileName);
                            }
                            catch { }
                            File.Move(sPath, pSaveDialog.FileName);
                        }
                        sPath = String.IsNullOrEmpty(pSaveDialog.FileName) ? sPath : pSaveDialog.FileName;
                    }
                    Clipboard.SetText(sPath, TextDataFormat.Text);
                    MessageBox.Show("Avery Labels has been saved at\n" + sPath + "\n(File path has been copied to Clipboard)", "PONS", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                };
                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                MessageBox.Show(ex.Message);
            }
        }

        private void chkAdditional1_CheckedChanged(object sender, EventArgs e)
        {
            pnlAdditionalOutageTime1.Enabled = chkAdditional1.Checked;
        }

        private void dtpshutdowndate_ValueChanged(object sender, EventArgs e)
        {
            //this.dtpshutdowndateON.MinDate = dtpshutdowndate.Value;
            dtpshutdowndate2.MinDate = dtpshutdowndate.Value;
        }

        private void dtpshutdowndate2_ValueChanged(object sender, EventArgs e)
        {
            // this.dgCustomerList..MinDate = dtpshutdowndate2.Value;
        }

        private void dtpshutdowntimeoff_ValueChanged(object sender, EventArgs e)
        {
            //dtpshutdowntimeon.MinDate = (dtpshutdowndateON.Value.ToShortDateString() != dtpshutdowndate.Value.ToShortDateString()) ?
            //Convert.ToDateTime("00:00:00") : DateTime.Parse(dtpshutdowntimeoff.Value.ToShortTimeString());
        }

        private void dtpshutdowntimeoff2_ValueChanged(object sender, EventArgs e)
        {
            //    dtpshutdowntimeon2.MinDate = (dtpshutdowndateON2.Value.ToShortDateString() != dtpshutdowndate2.Value.ToShortDateString()) ?
            //Convert.ToDateTime("00:00:00") : DateTime.Parse(dtpshutdowntimeoff2.Value.ToShortTimeString());
        }

        private void dtpshutdowntimeoffadditional_ValueChanged(object sender, EventArgs e)
        {
            // dtpshutdowntimeonadditional.MinDate = DateTime.Parse(dtpshutdowntimeoffadditional.Value.ToShortTimeString());
        }

        private void btnEmailSend_Click(object sender, EventArgs e)
        {
            try
            {
                //DataRow[] pDR_SelectedPSC = pPSC.Tables[0].Select("FName = '" + cmbPSCUserList.Text.Split(' ')[0] + "' AND LName = '" + cmbPSCUserList.Text.Split(' ')[1] + "' AND Division = '" + cmbPSCUserList.Text.Split(' ')[2].Replace("[","").Replace("]","") + "'");
                this.Cursor = Cursors.WaitCursor;
                pReport.PSC_ID = ((KeyValuePair<string, string>)cmbPSCUserList.SelectedItem).Key.Split(',')[0];
                pReport.Contact_ID = ((KeyValuePair<string, string>)cmbPSCUserList.SelectedItem).Key.Split(',')[1];

                //pReport.PSC_ID = cmbPSCUserList.Text.Split('(')[1].Replace(")", "");
                //pReport.ReportID = "";//Yet to get
                pReport.SubmitterID = Environment.UserName;
                pReport.CC_Me_Mail = chkCCMe.Checked;
                //pReport.Contact_ID = Convert.ToString(pPSC.Tables[0].Select("LANID = '" + pReport.PSC_ID + "'")[0]["ContactID"]);
                if (pReport.Contact_ID.Length == 0) pReport.Contact_ID = "0";
                if (ReportUtilities.SendMail(ref pReport))
                    MessageBox.Show("Notification request has been sent.\nIf you need to refer this request, the request ID is " + pReport.ReportID, "PONS", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                else
                    MessageBox.Show("Error Sending Email.\nPlease contact system administrator", "PONS", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                MessageBox.Show(ex.Message);
            }
        }

        private void btnEmailClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnEmailNext_Click(object sender, EventArgs e)
        {
            if (chkCLR.Checked)
            {
                btnPrintNext.Enabled = chkAddress.Checked;
                ActivatePanel(pnlCustomerNotificationPrint);
            }
            else if (chkAddress.Checked)
            {
                ActivatePanel(pnlAveryLabel);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                string sFilter = "All Files (*.*)|*.*";
                this.Cursor = Cursors.WaitCursor;
                string sPath = string.Empty;
                if (rdbText.Checked)
                {
                    sPath = ReportUtilities.CreateTXT(pReport);
                    sFilter = "Text Files (*.txt)|*.txt";
                }
                else if (rdbXLSX.Checked)
                {
                    sPath = ReportUtilities.CreateEXL(pReport);
                    sFilter = "Spreadsheet Files (*.xlsx)|*.xlsx";
                }
                else
                {

                    sPath = ReportUtilities.CreatePDF(pReport);
                    sFilter = "PDF Files (*.pdf)|*.pdf";
                }
                if (!string.IsNullOrEmpty(sPath))
                {
                    if (!sPath.StartsWith("P:"))
                    {
                        SaveFileDialog pSaveDialog = new SaveFileDialog();
                        pSaveDialog.AddExtension = true;
                        pSaveDialog.Filter = sFilter;
                        pSaveDialog.Title = "Save PONS Report to Shared Network Location";
                        DialogResult pDialogResult = pSaveDialog.ShowDialog();
                        if (pDialogResult == DialogResult.OK || pDialogResult == DialogResult.Yes)
                        {
                            try
                            {
                                if (File.Exists(pSaveDialog.FileName))
                                    File.Delete(pSaveDialog.FileName);
                            }
                            catch { }
                            File.Move(sPath, pSaveDialog.FileName);
                        }
                        sPath = String.IsNullOrEmpty(pSaveDialog.FileName) ? sPath : pSaveDialog.FileName;
                    }
                    Clipboard.Clear();
                    Clipboard.SetText(sPath, TextDataFormat.Text);
                    MessageBox.Show("Report has been saved at\n" + sPath + "\n(File path has been copied to Clipboard)", "PONS", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                }
                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                MessageBox.Show(ex.Message);
            }
        }

        private void btnPrintClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnPrintPrev_Click(object sender, EventArgs e)
        {
            if (chkPSC.Checked)
                ActivatePanel(pnlEmail);
            else
                ActivatePanel(pnlCustomerNotification);
        }

        private void txtLabelsFirstSheet_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtLabelsFirstSheet.Text.Length == 0) txtLabelsFirstSheet.Text = "0";
                Int32.Parse(txtLabelsFirstSheet.Text);
                if (Convert.ToInt32(txtLabelsFirstSheet.Text) > 29 || Convert.ToInt32(txtLabelsFirstSheet.Text) < 0)
                {
                    //txtLabelsFirstSheet.Text = "0";
                    throw new Exception();
                }
                this.btnAveryPrint.Enabled = true;
            }
            catch
            {
                this.btnAveryPrint.Enabled = false;
                MessageBox.Show("Please enter numeric value between 0 to 29 only.", "PONS", MessageBoxButtons.OK, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button1);
                this.txtLabelsFirstSheet.SelectAll();
                //txtLabelsFirstSheet.Text = "0";
            }
        }

        private void chkPSC_CheckedChanged(object sender, EventArgs e)
        {
            btncustNext.Enabled = chkAddress.Checked || chkCLR.Checked || chkPSC.Checked;
        }

        private void chkCLR_CheckedChanged(object sender, EventArgs e)
        {
            btncustNext.Enabled = chkAddress.Checked || chkCLR.Checked || chkPSC.Checked;
        }

        private void chkAddress_CheckedChanged(object sender, EventArgs e)
        {
            btncustNext.Enabled = chkAddress.Checked || chkCLR.Checked || chkPSC.Checked;
        }

        private void btnSearchCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnCloseCustomerList_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnDeleteCustSearch_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.dgdeviceView.SelectedRows.Count <= 0)
                    return;

                dgdeviceView.Rows.Remove(dgdeviceView.SelectedRows[0]);
                if (this.dgdeviceView.Rows.Count <= 0)
                {
                    this.btnDeleteCustSearch.Enabled = false;
                    this.btnGetCustomer.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
        }

        private void btnSearchClear_Click(object sender, EventArgs e)
        {
            txtTransformer.Text = "";
            txtStartDevice.Text = "";
            txtEndDevice.Text = "";
            this.cmbSubstation.SelectedIndexChanged -= new System.EventHandler(this.cmbSubstation_SelectedIndexChanged);
            cmbSubstation.SelectedIndex = 0;
            this.cmbSubstation.SelectedIndexChanged += new System.EventHandler(this.cmbSubstation_SelectedIndexChanged);
            this.cmbBank.SelectedIndexChanged -= new System.EventHandler(this.cmbBank_SelectedIndexChanged);
            cmbBank.SelectedIndex = 0;
            this.cmbBank.SelectedIndexChanged += new System.EventHandler(this.cmbBank_SelectedIndexChanged);
            this.cmbCircuit.SelectedIndexChanged -= new System.EventHandler(this.cmbCircuit_SelectedIndexChanged);
            cmbCircuit.SelectedIndex = 0;
            this.cmbCircuit.SelectedIndexChanged += new System.EventHandler(this.cmbCircuit_SelectedIndexChanged);
            this.rbBetweenDevicesSearch.Checked = this.rbSubCircuitSearch.Checked = this.rbTransformerSearch.Checked = false;
        }

        private void btnPrevSearch_Click(object sender, EventArgs e)
        {
            ActivatePanel(pnlWelcome);
        }

        private void btncustprintClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnCustNotPrev_Click(object sender, EventArgs e)
        {
            ActivatePanel(pnlOutageInfo);
        }

        private void btnoutageReset_Click(object sender, EventArgs e)
        {
            this.txtDescription.Text = string.Empty;
            this.chkAdditional1.Checked = false;
            this.dtpshutdowndate.MinDate = DateTime.Today.AddDays(1);
            this.dtpshutdowndate2.MinDate = DateTime.Today.AddDays(1);
            this.dtpshutdowntimeon.Value = new System.DateTime(dtpshutdowndate.Value.Year, dtpshutdowndate.Value.Month, dtpshutdowndate.Value.Day, 12, 30, 00, 0);
            this.dtpshutdowntimeoff.Value = new System.DateTime(dtpshutdowndate.Value.Year, dtpshutdowndate.Value.Month, dtpshutdowndate.Value.Day, 12, 00, 00, 0);
            this.dtpshutdowntimeon2.Value = new System.DateTime(dtpshutdowndate2.Value.Year, dtpshutdowndate2.Value.Month, dtpshutdowndate2.Value.Day, 12, 30, 00, 0);
            this.dtpshutdowntimeoff2.Value = new System.DateTime(dtpshutdowndate2.Value.Year, dtpshutdowndate2.Value.Month, dtpshutdowndate2.Value.Day, 12, 00, 00, 0);
        }

        private void btnOutagePrev_Click(object sender, EventArgs e)
        {
            ActivatePanel(pnlCustomerList);
        }

        private void cmbSubstation_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbSubstation.SelectedIndex == 0)
                {
                    //cmbBank.Items.Clear();
                    //cmbBank.DataSource = null;
                    cmbBank.Items.Clear();
                    cmbBank.Items.Add("<Select>");

                    cmbCircuit.Items.Clear();
                    cmbCircuit.Items.Add("<Select>");
                }
                else
                {
                    this.cmbBank.SelectedIndexChanged -= new System.EventHandler(this.cmbBank_SelectedIndexChanged);
                    
                    this.cmbBank.Items.Clear();
                    this.cmbBank.Items.Add("<Select>");
                    this.cmbCircuit.Items.Clear();
                    this.cmbCircuit.Items.Add("<Select>");

                    if (cmbSubstation.Text != "<Select>")
                        foreach (DataRow pDRow in pDT_DIV_SUB_BNK_CID.Select("SUBSTATIONNAME = '" + (char.IsNumber(cmbSubstation.Text, 0) ? cmbSubstation.Text.Substring(4) : cmbSubstation.Text) + "'"))
                            if (!this.cmbBank.Items.Contains(Convert.ToString(pDRow["TX_BANK_CODE"])))
                                this.cmbBank.Items.AddRange(new object[] { Convert.ToString(pDRow["TX_BANK_CODE"]) });
                    cmbBank.SelectedIndex = 0;
                    cmbCircuit.SelectedIndex = 0;
                    this.cmbBank.SelectedIndexChanged += new System.EventHandler(this.cmbBank_SelectedIndexChanged);

                    //MapUtility mu = new MapUtility();

                    //SubstationCircuitsDict.Clear();
                    //SubstationCircuitsDict.Add("<Select>", "<Select>");
                    //Dictionary<string, int> dict = new Dictionary<string, int>();
                    //int iSubOID = Convert.ToInt32(((KeyValuePair<string, string>)cmbSubstation.SelectedItem).Key);

                    //dict = mu.FindSpatiallyConnectedCircuits(iSubOID);
                    ////dict = dict.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
                    //foreach (KeyValuePair<string, int> kvp in dict)
                    //    SubstationCircuitsDict.Add(kvp.Key, Convert.ToString(kvp.Value));

                    //this.cmbBank.SelectedIndexChanged -= new System.EventHandler(this.cmbBank_SelectedIndexChanged);
                    //cmbBank.DataSource = new BindingSource(SubstationCircuitsDict, null);
                    //cmbBank.ValueMember = "Key";
                    //cmbBank.DisplayMember = "Value";
                    //cmbBank.SelectedIndex = 0;
                    //this.cmbBank.SelectedIndexChanged += new System.EventHandler(this.cmbBank_SelectedIndexChanged);

                    //cmbCircuit.DataSource = new BindingSource(SubstationCircuitsDict, null);
                    //cmbCircuit.ValueMember = "Value";
                    //cmbCircuit.DisplayMember = "Key";
                    //cmbCircuit.SelectedIndex = 0;
                    ////cmbCircuit.SelectedIndex = 0;

                    //// Commented by SB on 02-16-2016
                    ////SubstationBankDict.Clear();
                    ////SubstationBankDict.Add("<Select>", "<Select>");
                    ////Dictionary<string, string> dict = new Dictionary<string, string>();
                    ////string sid = ((KeyValuePair<string, string>)cmbSubstation.SelectedItem).Key;
                    ////dict = mu.GetSubStationBanks(sid);

                    ////dict = dict.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
                    ////foreach (KeyValuePair<string, int> kvp in dict)
                    ////    SubstationBankDict.Add(kvp.Key, kvp.Value);

                    ////this.cmbBank.SelectedIndexChanged -= new System.EventHandler(this.cmbBank_SelectedIndexChanged);
                    ////cmbBank.DataSource = new BindingSource(SubstationBankDict, null);
                    ////cmbBank.ValueMember = "Key";
                    ////cmbBank.DisplayMember = "Value";
                    ////cmbBank.SelectedIndex = 0;
                    ////this.cmbBank.SelectedIndexChanged += new System.EventHandler(this.cmbBank_SelectedIndexChanged);
                    ////populatecircuitids
                    ////List<string> CircuitList = new List<string>();
                    ////mu = new MapUtility();
                    ////CircuitList.Add("<Select>");

                    ////foreach (KeyValuePair<string, string> kvpbank in SubstationBankDict)
                    ////{
                    ////    if (kvpbank.Key != "<Select>")
                    ////    {
                    ////        CircuitList.AddRange(mu.GetCircuitIdsforbank(sid, kvpbank.Key));
                    ////    }
                    ////}

                    ////cmbCircuit.Items.Clear();
                    ////cmbCircuit.Items.AddRange(CircuitList.ToArray());
                    ////cmbCircuit.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                MessageBox.Show(ex.Message);
            }
        }

        private void cmbBank_SelectedIndexChanged(object sender, EventArgs e)
        {
            //cmbCircuit.SelectedIndex = this.cmbBank.SelectedIndex;
            this.cmbCircuit.Items.Clear();
            this.cmbCircuit.Items.Add("<Select>");
            if (cmbBank.Text != "<Select>")
                foreach (DataRow pDRow in pDT_DIV_SUB_BNK_CID.Select("SUBSTATIONNAME = '" + (char.IsNumber(cmbSubstation.Text, 0) ? cmbSubstation.Text.Substring(4) : cmbSubstation.Text) + "' AND TX_BANK_CODE = '" + cmbBank.Text + "'"))
                    this.cmbCircuit.Items.AddRange(new object[] { Convert.ToString(pDRow["TO_CIRCUITID"]) });
            cmbCircuit.SelectedIndex = 0;
        }

        private void dgCustomerList_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0) return;
                if (e.ColumnIndex == 0)
                {
                    //int iCountSelected = 0;
                    //foreach (DataGridViewRow drvRow in dgCustomerList.Rows)
                    //for (int iCount_GridRow = 0; iCount_GridRow < dgCustomerList.Rows.Count; ++iCount_GridRow)
                    //{
                    //    iCountSelected += Convert.ToBoolean(((DataGridViewCheckBoxCell)dgCustomerList.Rows[iCount_GridRow].Cells["clIncludeCustomer"]).Value) ? 1 : 0;
                    //    pReport.AffectedCustomers[iCount_GridRow].Excluded = !Convert.ToBoolean(((DataGridViewCheckBoxCell)dgCustomerList.Rows[iCount_GridRow].Cells["clIncludeCustomer"]).Value);
                    //}
                    //int iCountSelected = Convert.ToInt32(lblcustlistNo.Text);
                    //iCountSelected += (pReport.AffectedCustomers[e.RowIndex].Excluded = Convert.ToBoolean(((DataGridViewCheckBoxCell)dgCustomerList.Rows[e.RowIndex].Cells["clIncludeCustomer"]).Value)) ? 1 : -1;

                    for (int i = 0; i < pReport.AffectedCustomers.Count; ++i)
                    {
                        if (pReport.AffectedCustomers[i].SNO == Convert.ToInt32(dgCustomerList.Rows[e.RowIndex].Cells["SNO"].Value))
                            pReport.AffectedCustomers[i].Excluded = !Convert.ToBoolean(((DataGridViewCheckBoxCell)dgCustomerList.Rows[e.RowIndex].Cells["clIncludeCustomer"]).Value);
                        //iCountSelected += pReport.AffectedCustomers[i].Excluded ? 0 : 1;
                    }

                    //foreach (Customer customer in pReport.AffectedCustomers.Where(c => c.SNO == Convert.ToInt32(dgCustomerList.Rows[e.RowIndex].Cells["SNO"].Value)))
                    //{
                    //    customer.Excluded = !Convert.ToBoolean(((DataGridViewCheckBoxCell)dgCustomerList.Rows[e.RowIndex].Cells["clIncludeCustomer"]).Value);
                    //}
                    lblcustlistMeterNo.Text = pReport.AffectedCustomers.Where(c => !c.Excluded).Select(c => c.MeterNumber).Distinct().Count().ToString();
                    lblcustlistLetterNo.Text = pReport.AffectedCustomers.Where(c => !c.Excluded).Count().ToString();// iCountSelected.ToString();
                }
                btncustlistNext.Enabled = Convert.ToInt32(lblcustlistMeterNo.Text) > 0;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                MessageBox.Show(ex.Message);
            }
        }

        private void dtpshutdowndateON_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                //this.dtpshutdowndate2.MinDate = dtpshutdowndateON.Value;
                //dtpshutdowntimeon.MinDate = (dtpshutdowndateON.Value.ToShortDateString() != dtpshutdowndate.Value.ToShortDateString()) ?
                //Convert.ToDateTime("00:00:00") : DateTime.Parse(dtpshutdowntimeoff.Value.ToShortTimeString());
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                MessageBox.Show(ex.Message);
            }
        }

        private void dtpshutdowndateON2_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                //    dtpshutdowntimeon2.MinDate = (dtpshutdowndateON2.Value.ToShortDateString() != dtpshutdowndate2.Value.ToShortDateString()) ?
                //Convert.ToDateTime("00:00:00") : DateTime.Parse(dtpshutdowntimeoff2.Value.ToShortTimeString());
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                MessageBox.Show(ex.Message);
            }
        }

        private void dgCustomerList_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex != 0) return;

            dgCustomerList.EndEdit();
        }

        private void dgCustomerList_KeyUp(object sender, KeyEventArgs e)
        {
            dgCustomerList.EndEdit();
        }

        private void PONSHomeScreen_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Dispose(true);
        }

        private void cmbCircuit_SelectedIndexChanged(object sender, EventArgs e)
        {
            //this.cmbBank.SelectedIndex = cmbCircuit.SelectedIndex;
        }

        private void btnCloseOutageinfo_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dtpshutdowntimeon_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}
