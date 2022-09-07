using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using PGE.Common.Delivery.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using Miner.Interop;
using Miner.Geodatabase.Edit;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geometry;
using Miner;
using ESRI.ArcGIS.esriSystem;
using PGE.Common.Delivery.Process;
using Miner.Interop.Process;
using System.Security.Principal;

namespace PGE.Desktop.EDER.ArcMapCommands
{
    public partial class PGE_TransformerSearchForm : Form
    {
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        private static string tableName_CustAddress = "EDGIS.T_CUSTADD";
        private static string tableName_TransSearch = "EDGIS.TRANSFORMER_SEARCH";
        public static string tableName_Tx_form_save = "EDGIS.TX_SEARCH_FORM_LOC";
        public static IWorkspace _loginWorkspace;
        private IStepProgressor sProgressor;
        private int maxResultsCount;
        //private bool exceededThreshold;
        //ME Q4-19 for DA#190903
        IApplication _AppCopy = null;
        int resultCount = 0;
        int resultCountMeter = 0;
        //string searchResultMessage = string.Empty;
        string searchTrMsg = string.Empty;
        string searchMeterMsg = string.Empty;

        public static string fLocation = string.Empty;
        public static string fState = "OPEN";  //default value
        public static string filter = string.Empty;
        public static string userLanID = string.Empty;
        public static string formName = string.Empty;
        bool firstFrmMove = false;
        //IMMPxApplication _pxApp = null;
        //PxDb _pxDb = null;

        public PGE_TransformerSearchForm(IApplication _App)
        {
            InitializeComponent();
            ValueTextBox.Enabled = false;
            SearchBtn.Enabled = false;
            //ME Q4-19 for DA#190903
            //FilterComboBox.Items.Insert(0, "-SELECT-");

            FilterComboBox.SelectedIndex = 0;

            //IMMPxIntegrationCache intcache = (IMMPxIntegrationCache)_App.FindExtensionByName("Session Manager Integration Extension");
            //_pxApp = intcache.Application;
            //_pxDb = new PxDb(_pxApp);

            System.Security.Principal.WindowsIdentity wi = System.Security.Principal.WindowsIdentity.GetCurrent();

            userLanID = wi.Name.Substring(wi.Name.LastIndexOf("\\") + 1);

            //string sqlFormEnable = "Select FORM_LOCATION,LAST_FILTER from " + tableName_Tx_form_save + " where LAN_ID = '" + userLanID.ToUpper() + "'";
            System.Data.DataTable dt = PGEExtension.pDataTableFormInfo;
            if (dt !=null )
            {
                
                if (dt.Rows.Count > 0)
                {
                    System.Drawing.Point p = new System.Drawing.Point();
                    string[] form_location = dt.Rows[0]["FORM_LOCATION"].ToString().Split(',');   //{X=67,Y=190}
                    p.X = Convert.ToInt32(form_location[0].Substring(3));
                    p.Y = Convert.ToInt32(form_location[1].Substring(2).Remove("}"));
                    // p.X = 154; p.Y = 390;

                    this.Location = p;
                    firstFrmMove = true;
                    this.FilterComboBox.SelectedItem = dt.Rows[0]["LAST_FILTER"].ToString();

                }
                else
                {

                    Rectangle screenRect = Screen.GetBounds(Bounds);
                    // get the Screen Boundy
                   // ClientSize = new Size((int)(screenRect.Width / 2), (int)(screenRect.Height / 2)); // set the size of the form
                    this.Location = new System.Drawing.Point(screenRect.Width / 2 - ClientSize.Width / 2, screenRect.Height / 2 - ClientSize.Height / 2); 
                    
                    FilterComboBox.SelectedIndex = 0;
                }
            }
            
            _AppCopy = _App;
            IStatusBar stbar = _App.StatusBar;
            sProgressor = stbar.ProgressBar;
            
            getMaxResultsCnt();

            ValueTextBox.KeyUp += new KeyEventHandler(ValueTextBox_KeyUp);

            

        }
        public IMap map { get; set; }

        private void getMaxResultsCnt()
        {
             IWorkspace loginWorkspace = GetWorkspace();

             if (loginWorkspace != null)
             {
                 if (((IWorkspace2)loginWorkspace).NameExists[esriDatasetType.esriDTTable, tableName_TransSearch])
                 {
                     //query on Customer Address table and get Transformer Global IDs
                     ITable transSearchTable = ((IFeatureWorkspace)loginWorkspace).OpenTable(tableName_TransSearch);
                     if (transSearchTable != null)
                     {
                         IQueryFilter pQf = new QueryFilterClass();
                         pQf.SubFields = "VALUE";
                         pQf.WhereClause = "FIELD = 'MAXGUIDINQUERY'";

                         ICursor pCur = transSearchTable.Search(pQf, true);

                         IRow pRow = pCur.NextRow();
                         if (pRow != null)
                         {
                             maxResultsCount = Convert.ToInt16(Convert.ToString(pRow.get_Value(pRow.Fields.FindField("VALUE"))));
                         }

                         Marshal.ReleaseComObject(pCur);
                     }
                 }
             }
        }

        private void FilterComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (FilterComboBox.SelectedItem.ToString() != "" && FilterComboBox.SelectedItem.ToString() != "-SELECT-")
            {
                ValueTextBox.Enabled = true;
                IWorkspace pLoginWorkspace1 = GetWorkspace();
                string sqlFilterUpdate = "update " + tableName_Tx_form_save + " set form_state = 'OPEN' , last_filter ='" + FilterComboBox.SelectedItem.ToString() + "', form_location = '" + this.Location.ToString() + "' where LAN_ID = '" + userLanID.ToUpper() + "'";
                pLoginWorkspace1.ExecuteSQL(sqlFilterUpdate);
            }
            else
            {
                ValueTextBox.Enabled = false;
                SearchBtn.Enabled = false;
            }
        }

        private void ValueTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ValueTextBox.Text.Trim().Length > 0 && FilterComboBox.SelectedItem.ToString() != "" && FilterComboBox.SelectedItem.ToString() != "-SELECT-")
                SearchBtn.Enabled = true;
            else
                SearchBtn.Enabled = false;
        }

        private void ValueTextBox_KeyUp(object sender, KeyEventArgs e)   
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                SearchBtn_Click(null, null);
            }
        }

        private void SearchBtn_Click(object sender, EventArgs e)
        {
            try
            {
                ResultLabel.Text = "";
                //ME Q4-19 for DA#190903
                sProgressor.Message = "Searching Transformers/Primary Meters....";
                //exceededThreshold = false;

                ILayer TransformerLayer = null;
                ILayer MeterLayer = null;
                if (!FindLayerByName("Transformer", out TransformerLayer))    //get Transformer layer.
                {
                    MessageBox.Show("Transformer/Primary meter layer is not present on Map. Unable to perform search.", "Error", System.Windows.Forms.MessageBoxButtons.OK);
                    ResultLabel.Text = "";
                    sProgressor.Message = "";
                    return;
                    // throw new COMException("Transformer layer is not present on Map. Unable to perform search");
                }
                //ME Q4-19 for DA#190903
                if (!FindLayerByName("Primary Meter", out MeterLayer))    //get Meter layer.
                {
                    MessageBox.Show("Transformer/Primary meter layer is not present on Map. Unable to perform search.", "Error", System.Windows.Forms.MessageBoxButtons.OK);
                    ResultLabel.Text = "";
                    sProgressor.Message = "";
                    return;
                    // throw new COMException("Transformer layer is not present on Map. Unable to perform search");
                }

                //clear previous results
                IFeatureSelection pFeatureSelection = (IFeatureSelection)TransformerLayer;
                if (pFeatureSelection != null) pFeatureSelection.Clear();
                //ME Q4-19 for DA#190903
                IFeatureSelection pFeatureSelectionMeter = (IFeatureSelection)MeterLayer;
                if (pFeatureSelectionMeter != null) pFeatureSelectionMeter.Clear();

                //for CGC Number search
                if (FilterComboBox.SelectedItem.ToString() == "CGC Number")
                {
                    IWorkspace wSpace = null;
                    IWorkspace wSpaceMeter = null;
                    IFeatureWorkspace fWSpace = null;
                    IFeatureWorkspace fWSpaceMeter = null;
                    IQueryFilter qfilter = new QueryFilterClass();
                    IFeatureCursor featCur = null;
                    IFeatureCursor featCurMeter = null;
                    IFeatureClass fTrans = null;
                    IFeatureClass fMeter = null;
                    try
                    {
                        IFeatureClass TransFClass = ((IFeatureLayer)TransformerLayer).FeatureClass;
                        IDataset transDSet = (IDataset)TransFClass;
                        wSpace = transDSet.Workspace;
                        fWSpace = (IFeatureWorkspace)wSpace;
                        fTrans = fWSpace.OpenFeatureClass("EDGIS.Transformer");


                        //Primary Meter CGC search
                        IFeatureClass MeterFClass = ((IFeatureLayer)MeterLayer).FeatureClass;
                        IDataset meterDSet = (IDataset)MeterFClass;
                        wSpaceMeter = meterDSet.Workspace;
                        fWSpaceMeter = (IFeatureWorkspace)wSpaceMeter;
                        fMeter = fWSpace.OpenFeatureClass("EDGIS.PrimaryMeter");

                        string queryValue = Convert.ToString(ValueTextBox.Text).Trim();
                        if (ReturnQuery(queryValue) != "")
                            qfilter.WhereClause = ReturnQuery(queryValue);
                        else
                        {
                            ResultLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
                            ResultLabel.ForeColor = System.Drawing.Color.Red;
                            ResultLabel.Text = "No Transformers/Primary Meters found.";
                            sProgressor.Message = "";
                            //MessageBox.Show("No features found.", "Result", System.Windows.Forms.MessageBoxButtons.OK);
                            return;
                        }

                        featCur = (fTrans.Search(qfilter, false));
                        featCurMeter = (fMeter.Search(qfilter, false));

                        AddToSelectionAndZoom(featCur, TransformerLayer);
                        AddToSelectionAndZoom(featCurMeter, MeterLayer);

                        if (resultCount > 0 || resultCountMeter > 0)
                        {
                            ResultLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
                            ResultLabel.ForeColor = System.Drawing.Color.Black;
                            ResultLabel.Text = searchTrMsg.Remove("found") + "and " + searchMeterMsg;
                            
                        }
                        else
                        {
                            ResultLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
                            ResultLabel.ForeColor = System.Drawing.Color.Red;
                            ResultLabel.Text = "No Transformers/Primary Meters found.";
                        }
                        searchTrMsg = string.Empty;
                        searchMeterMsg = string.Empty;
                        resultCount = 0;
                        resultCountMeter = 0;
                    }
                    catch (Exception ex)
                    {
                        _logger.Error("PGE Transformer Search on CGC Number, Fetching data from Transformer layer ", ex);
                        MessageBox.Show("Error in performing Transformer Search.", "Error", System.Windows.Forms.MessageBoxButtons.OK);
                        ResultLabel.Text = "";
                        sProgressor.Message = "";
                    }
                    finally
                    {
                        if (wSpace != null) { Marshal.ReleaseComObject(wSpace); }
                        if (fWSpace != null) { Marshal.ReleaseComObject(fWSpace); }
                        if (qfilter != null) { Marshal.ReleaseComObject(qfilter); }
                        if (featCur != null) { Marshal.ReleaseComObject(featCur); }
                    }
                }

                //for Transformer address, Customer Address, meter number and service point id search
                else if (FilterComboBox.SelectedItem.ToString() != "" && FilterComboBox.SelectedItem.ToString() != "-SELECT-")
                {
                    IWorkspace loginWorkspace = GetWorkspace();

                    if (loginWorkspace != null)
                    {
                        if (((IWorkspace2)loginWorkspace).NameExists[esriDatasetType.esriDTTable, tableName_CustAddress])
                        {
                            //query on Customer Address table and get Transformer Global IDs
                            ITable custAddTable = ((IFeatureWorkspace)loginWorkspace).OpenTable(tableName_CustAddress);
                            if (custAddTable != null)
                            {

                                //For Primary Meter
                                IQueryFilter pQfMeter = new QueryFilterClass();
                                pQfMeter.SubFields = "PRIMARYMETERGUID";
                                pQfMeter.WhereClause = getCustAddWhereClause();

                                ICursor pCurMeter = custAddTable.Search(pQfMeter, true);
                                string meterGlobalIds = getMeterGlobalIds(pCurMeter);

                                Marshal.ReleaseComObject(pCurMeter);

                                if (meterGlobalIds.Length > 0)
                                    getMetersFromGUID(meterGlobalIds);
                                else
                                {
                                    ResultLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
                                    ResultLabel.ForeColor = System.Drawing.Color.Red;
                                    ResultLabel.Text = "0 Primary Meter found";
                                    searchMeterMsg = "0 Primary Meter found";
                                    sProgressor.Message = "";
                                }

                                //For Transformer
                                IQueryFilter pQf = new QueryFilterClass();
                                pQf.SubFields = "TRANSFORMERGUID";
                                pQf.WhereClause = getCustAddWhereClause();

                                ICursor pCur = custAddTable.Search(pQf, true);
                                string transGlobalIds = getTransGlobalIds(pCur);

                                Marshal.ReleaseComObject(pCur);

                                if (transGlobalIds.Length > 0)
                                    getTransformersFromGUID(transGlobalIds);
                                else
                                {
                                    ResultLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
                                    ResultLabel.ForeColor = System.Drawing.Color.Red;
                                    ResultLabel.Text = "0 Transformers found";
                                    searchTrMsg = "0 Transformer found";
                                    sProgressor.Message = "";
                                }
                                    //MessageBox.Show("No features found.", "Result", System.Windows.Forms.MessageBoxButtons.OK);

                               

                                if (resultCount > 0 || resultCountMeter > 0)
                                {
                                    ResultLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
                                    ResultLabel.ForeColor = System.Drawing.Color.Black;
                                    ResultLabel.Text = searchTrMsg.Remove("found") + "and " + searchMeterMsg;

                                }
                                else
                                {
                                    ResultLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
                                    ResultLabel.ForeColor = System.Drawing.Color.Red;
                                    ResultLabel.Text = "No Transformers/Primary Meters found.";
                                }
                                searchTrMsg = string.Empty;
                                searchMeterMsg = string.Empty;
                                resultCount = 0;
                                resultCountMeter = 0;

                            }
                            //Zoom to Selected both Transformer and Primary  Meter
                            if (resultCount > 0 && resultCountMeter > 0)
                            {
                                UID zoomToSelected = new UIDClass();
                                zoomToSelected.Value = "esriArcMapUI.ZoomToSelectedCommand";
                                ICommandBars pbars = (ICommandBars)_AppCopy.Document.CommandBars;
                                ICommandItem pCmd = pbars.Find(zoomToSelected, false, false);
                                zoomToSelected.SubType = 3;
                                pCmd.Execute();
                            }
                        }
                        else
                        {
                            MessageBox.Show(tableName_CustAddress + " table was not found in the logged in workspace", "Table Not Found", System.Windows.Forms.MessageBoxButtons.OK);
                            ResultLabel.Text = "";
                            sProgressor.Message = "";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                MessageBox.Show("Error in performing Transformer Search.", "Error", System.Windows.Forms.MessageBoxButtons.OK);
                ResultLabel.Text = "";
                sProgressor.Message = "";
            }
        }

        //get query string for CGC search on Transformer
        private string ReturnQuery(string query)
        {
            int _strLen = query.Trim().Length;
            string _query = "";
            switch (_strLen)
            {
                case 12:
                    {
                        _query = "CGC12 = '" + query + "'";
                        break;
                    }
                case 8:
                    {
                        _query = "CGC12 LIKE '%" + query.Trim().Substring(0, 4) + "%" + query.Trim().Substring(4, 4) + "'";
                        break;
                    }
                case 9:
                    {
                        string[] _arry = null;
                        _arry = query.Trim().Split('-');
                        if (_arry.Length == 2)
                        {
                            _query = "CGC12 LIKE '%" + _arry[0] + "%" + _arry[1] + "'";
                        }
                        break;
                    }
            }

            return _query;
        }

        //select and zoom on result features
        private void AddToSelectionAndZoom(IFeatureCursor pFeatureCursor, ILayer pLayer)
        {
            try
            {
                IEnvelope pEnv = new EnvelopeClass();
                IFeature feature;
                //IFeatureSelection pFeatureSelection = (IFeatureSelection)pLayer;
                //pFeatureSelection.Clear();
                //int resultCount = 0;
                //int resultCountMeter = 0;
                ////string searchResultMessage = string.Empty;
                //string searchTrMsg = string.Empty;
                //string searchMeterMsg = string.Empty;
                IEnvelope pEnvMeter = new EnvelopeClass();
                IEnvelope pEnvFull = new EnvelopeClass();
                if (pLayer.Name.Equals("Transformer"))
                {
                    while ((feature = pFeatureCursor.NextFeature()) != null)
                    {
                        pEnv.Union(feature.ShapeCopy.Envelope);

                        ((IMxDocument)PGE.Common.Delivery.ArcFM.ApplicationFacade.Application.Document).FocusMap.SelectFeature(pLayer, feature);
                        resultCount++;
                    }

                    if (resultCount > 0)
                    {
                        //Once the envelope is build we can zoom to the envelope in the same manner that ArcFM currently does
                        double zoomScale = 5.0;
                        IMMRegistry registry = new MMRegistry();
                        registry.OpenKey(mmHKEY.mmHKEY_CURRENT_USER, mmBaseKey.mmArcFM, "General");
                        try
                        {
                            zoomScale = Convert.ToDouble(registry.Read("Zoom To Buffer Size", 5));
                        }
                        catch
                        {
                            zoomScale = 5.0;
                        }
                        IMMMapUtilities mapUtils = new mmMapUtilsClass();
                        mapUtils.ZoomTo(pEnv, ((IMxDocument)PGE.Common.Delivery.ArcFM.ApplicationFacade.Application.Document).FocusMap as IActiveView, zoomScale);

                        ResultLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular);
                        ResultLabel.ForeColor = System.Drawing.Color.Black;

                        if (resultCount == 1)
                        {
                            searchTrMsg = "1 Transformer found";
                            ResultLabel.Text = "1 Transformer found";
                        }
                        else
                        {
                            ResultLabel.Text = resultCount.ToString() + " Transformers found";
                            searchTrMsg = resultCount.ToString() + " Transformers found";
                        }
                        
                        //if(exceededThreshold)
                        //    ResultLabel.Text += "Exceeded result Threshold.";

                        sProgressor.Message = "";
                        //MessageBox.Show(resultCount.ToString() + " features found.", "Result", System.Windows.Forms.MessageBoxButtons.OK);
                        //this.Close();
                    }
                    else
                    {
                        ResultLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
                        ResultLabel.ForeColor = System.Drawing.Color.Red;
                        ResultLabel.Text = "0 Transformer found";
                        searchTrMsg = "0 Transformer found";
                        sProgressor.Message = "";
                        //MessageBox.Show("No features found.", "Result", System.Windows.Forms.MessageBoxButtons.OK);
                    }
                }
                //Add Primary Meter in selection
                else if (pLayer.Name.Equals("Primary Meter"))
                {
                    while ((feature = pFeatureCursor.NextFeature()) != null)
                    {
                        pEnvMeter.Union(feature.ShapeCopy.Envelope);

                        ((IMxDocument)PGE.Common.Delivery.ArcFM.ApplicationFacade.Application.Document).FocusMap.SelectFeature(pLayer, feature);
                        resultCountMeter++;
                    }

                    if (resultCountMeter > 0)
                    {
                        //Once the envelope is build we can zoom to the envelope in the same manner that ArcFM currently does
                        double zoomScale = 5.0;
                        IMMRegistry registry = new MMRegistry();
                        registry.OpenKey(mmHKEY.mmHKEY_CURRENT_USER, mmBaseKey.mmArcFM, "General");
                        try
                        {
                            zoomScale = Convert.ToDouble(registry.Read("Zoom To Buffer Size", 5));
                        }
                        catch
                        {
                            zoomScale = 5.0;
                        }
                        //pEnvMeter.Union(pEnv);
                        IMMMapUtilities mapUtils = new mmMapUtilsClass();
                        mapUtils.ZoomTo(pEnvMeter, ((IMxDocument)PGE.Common.Delivery.ArcFM.ApplicationFacade.Application.Document).FocusMap as IActiveView, zoomScale);
                        
                        ResultLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular);
                        ResultLabel.ForeColor = System.Drawing.Color.Black;

                        if (resultCountMeter == 1)
                        {
                            ResultLabel.Text = "1 Primary Meter found";
                            searchMeterMsg = "1 Primary Meter found";
                        }
                        else
                        {
                            ResultLabel.Text = resultCountMeter.ToString() + " Primary Meters found";
                            searchMeterMsg = resultCountMeter.ToString() + " Primary Meters found";
                        }
                        //if(exceededThreshold)
                        //    ResultLabel.Text += "Exceeded result Threshold.";

                        sProgressor.Message = "";
                        //MessageBox.Show(resultCount.ToString() + " features found.", "Result", System.Windows.Forms.MessageBoxButtons.OK);
                        //this.Close();
                    }
                    else
                    {
                        ResultLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
                        ResultLabel.ForeColor = System.Drawing.Color.Red;
                        ResultLabel.Text = "0 Primary Meter found";
                        searchMeterMsg = "0 Primary Meter found";
                        sProgressor.Message = "";
                        //MessageBox.Show("No features found.", "Result", System.Windows.Forms.MessageBoxButtons.OK);
                    }
                }

                
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                MessageBox.Show("Error in performing Transformer/Primary Meter Search.", "Error", System.Windows.Forms.MessageBoxButtons.OK);
                ResultLabel.Text = "";
                sProgressor.Message = "";
            }
        }

        //get query whereclause for search other than CGC
        private string getCustAddWhereClause()
        {
            string whereClause = string.Empty;
            string attributeName = string.Empty;
            string searchType = FilterComboBox.SelectedItem.ToString();
            if (searchType == "Transformer/Primary Meter Address")
                attributeName = "TXADDRESS,METERADDRESS";
            else if (searchType == "Customer Address")
                attributeName = "SPADDRESS";
            else if (searchType == "Meter Number")
                attributeName = "METERNUMBER";
            else if (searchType == "Service Point ID")
                attributeName = "SERVICEPOINTID";

            whereClause = "UPPER(" + attributeName + ") LIKE ('%" + RemoveSpecialChars(ValueTextBox.Text.ToUpper()) + "%')";
            if (attributeName.Equals("TXADDRESS,METERADDRESS"))
            {
                string [] atrAddress = attributeName.Split(',');
                whereClause = "UPPER(" + atrAddress[0] + ") LIKE ('%" + RemoveSpecialChars(ValueTextBox.Text.ToUpper()) + "%')" + " OR " + "UPPER(" + atrAddress[1] + ") LIKE ('%" + RemoveSpecialChars(ValueTextBox.Text.ToUpper()) + "%')";
            }
            return whereClause;
        }

        private string RemoveSpecialChars(string str)
        {
            // Create  a string array and add the special characters you want to remove

            string[] chars = new string[] { " ", ",", ".", "/", "!", "@", "#", "$", "^", "&", "*", "'", "\"", ";", "_", "(", ")", ":", "|", "[", "]" };
            //Iterate the number of times based on the String array length.
            for (int i = 0; i < chars.Length; i++)
            {
                if (str.Contains(chars[i]))
                {
                    str = str.Replace(chars[i], "%").ToUpper();
                }
            }

            return str;
        }

        //get comma separated transformer global IDs for where clause
        private string getTransGlobalIds(ICursor pCur)
        {
            //exceededThreshold = false;
            string transGlobalIds = "";
            IRow pRow = null;
            List<string> guidList = new List<string>();
            while ((pRow = pCur.NextRow()) != null)
            {
                string tGlobalId = Convert.ToString(pRow.get_Value(pRow.Fields.FindField("TRANSFORMERGUID"))).ToUpper();
                if (!string.IsNullOrEmpty(tGlobalId))
                {
                    if (guidList.Count >= maxResultsCount)
                    {
                        //exceededThreshold = true;
                        break;
                    }

                    if (!guidList.Contains(tGlobalId))
                        guidList.Add("'" + tGlobalId + "'");                    
                }
            }
            if (guidList.Count > 0)
                transGlobalIds = string.Join(",", guidList.Select(x => x.ToString()).ToArray());

            return transGlobalIds;
        }

        //get comma separated Primary Meter global IDs for where clause
        private string getMeterGlobalIds(ICursor pCurMeter)
        {
            //exceededThreshold = false;
            string meterGlobalIds = "";
            IRow pRowMeter = null;
            List<string> guidListMeter = new List<string>();
            while ((pRowMeter = pCurMeter.NextRow()) != null)
            {
                string tGlobalId = Convert.ToString(pRowMeter.get_Value(pRowMeter.Fields.FindField("PRIMARYMETERGUID"))).ToUpper();
                if (!string.IsNullOrEmpty(tGlobalId))
                {
                    if (guidListMeter.Count >= maxResultsCount)
                    {
                        //exceededThreshold = true;
                        break;
                    }

                    if (!guidListMeter.Contains(tGlobalId))
                        guidListMeter.Add("'" + tGlobalId + "'");
                }
            }
            if (guidListMeter.Count > 0)
                meterGlobalIds = string.Join(",", guidListMeter.Select(x => x.ToString()).ToArray());

            return meterGlobalIds;
        }

        //get features from Transformer layer using Global ID
        private void getTransformersFromGUID(string transGlobalIds)
        {
            IWorkspace wSpace = null;
            IFeatureWorkspace fWSpace = null;
            IQueryFilter qfilter = new QueryFilterClass();
            IFeatureCursor featCur = null;
            IFeatureClass fTrans = null;

            try
            {
                ILayer TransformerLayer = null;
                if (!FindLayerByName("Transformer", out TransformerLayer))
                {
                  //  throw new COMException("Transformer layer is not present on Map. Unable to perform search");
                    MessageBox.Show("Transformer layer is not present on Map. Unable to perform search.", "Error", System.Windows.Forms.MessageBoxButtons.OK);
                    ResultLabel.Text = "";
                    sProgressor.Message = "";
                    return;
                }

                IFeatureClass TransFClass = ((IFeatureLayer)TransformerLayer).FeatureClass;
                IDataset transDSet = (IDataset)TransFClass;
                wSpace = transDSet.Workspace;
                fWSpace = (IFeatureWorkspace)wSpace;
                fTrans = fWSpace.OpenFeatureClass("EDGIS.Transformer");

                qfilter.WhereClause = "UPPER(GLOBALID) IN (" + transGlobalIds + ")"; 
                featCur = (fTrans.Search(qfilter, false));

                AddToSelectionAndZoom(featCur, TransformerLayer);
            }
            catch (Exception ex)
            {
                _logger.Error("PGE Transformer Search , Fetching data from Transformer layer ", ex);
                throw ex;
            }
            finally
            {
                if (wSpace != null) { Marshal.ReleaseComObject(wSpace); }
                if (fWSpace != null) { Marshal.ReleaseComObject(fWSpace); }
                if (qfilter != null) { Marshal.ReleaseComObject(qfilter); }
                if (featCur != null) { Marshal.ReleaseComObject(featCur); }
            }
        }

        //get features from Primary Meter layer using Global ID
        private void getMetersFromGUID(string metersGlobalIds)
        {
            IWorkspace wSpaceMeter = null;
            IFeatureWorkspace fWSpaceMeter = null;
            IQueryFilter qfilterMetre = new QueryFilterClass();
            IFeatureCursor featCurMeter = null;
            IFeatureClass fTMeters = null;

            try
            {
                ILayer PrimaryMeterLayer = null;
                if (!FindLayerByName("Primary Meter", out PrimaryMeterLayer))
                {
                    //  throw new COMException("Primary meter layer is not present on Map. Unable to perform search");
                    MessageBox.Show("Primary meter layer is not present on Map. Unable to perform search.", "Error", System.Windows.Forms.MessageBoxButtons.OK);
                    ResultLabel.Text = "";
                    sProgressor.Message = "";
                    return;
                }

                IFeatureClass meterFClass = ((IFeatureLayer)PrimaryMeterLayer).FeatureClass;
                IDataset meterDSet = (IDataset)meterFClass;
                wSpaceMeter= meterDSet.Workspace;
                fWSpaceMeter = (IFeatureWorkspace)wSpaceMeter;
                fTMeters = fWSpaceMeter.OpenFeatureClass("EDGIS.PrimaryMeter");

                qfilterMetre.WhereClause = "UPPER(GLOBALID) IN (" + metersGlobalIds + ")";
                featCurMeter = (fTMeters.Search(qfilterMetre, false));

                AddToSelectionAndZoom(featCurMeter, PrimaryMeterLayer);
            }
            catch (Exception ex)
            {
                _logger.Error("PGE Transformer Search , Fetching data from Transformer layer ", ex);
                throw ex;
            }
            finally
            {
                if (wSpaceMeter != null) { Marshal.ReleaseComObject(wSpaceMeter); }
                if (fWSpaceMeter != null) { Marshal.ReleaseComObject(fWSpaceMeter); }
                if (qfilterMetre != null) { Marshal.ReleaseComObject(qfilterMetre); }
                if (featCurMeter != null) { Marshal.ReleaseComObject(featCurMeter); }
            }
        }
        //Paste Button
        private void ResetBtn_Click(object sender, EventArgs e)
        {
            try
            {
                //if (FilterComboBox.Items.Count > 0)
                //    FilterComboBox.SelectedIndex = 0;
                //Paste the value from clip board into text box
                ValueTextBox.Clear();
                string textClipBoard = System.Windows.Clipboard.GetText();
                if (!(textClipBoard.IsNullOrEmpty()))
                {
                    textClipBoard = RemoveSpecialChars(textClipBoard);
                    textClipBoard = textClipBoard.Remove("%");
                    textClipBoard = textClipBoard.Remove("-");
                    ValueTextBox.Text = textClipBoard;
                }
                ResultLabel.Text = "";
            }
            catch (Exception ex)
            {
                _logger.Error("PGE Transformer Search: Reset button click - " + ex.Message, ex);
                MessageBox.Show(ex.Message);
            }
        }

        private void CloseBtn_Click(object sender, EventArgs e)
        {
            try
            {
                _logger.Debug("User clicked close on PGE Transformer Search UI form. Now hiding the form.");
                this.Hide();
                //this.Dispose();
                //ME Q4-19 for DA#190903
                //if (!SearchBtn.Enabled) { if (FilterComboBox.Items.Count > 0) { FilterComboBox.SelectedIndex = 0; } }
                //Deleting the saved from location as User closes the form
                IWorkspace loginWorkspaceCl = GetWorkspace();
                string SQL = "update " + tableName_Tx_form_save + " set form_state = 'CLOSE' , last_filter ='" + FilterComboBox.SelectedItem.ToString() + "', form_location = '" + this.Location.ToString() + "' where LAN_ID = '" + userLanID.ToUpper() + "'";

                // Update the matching rows.
                loginWorkspaceCl.ExecuteSQL(SQL);
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #region Override for Form

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Form.FormClosing"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.FormClosingEventArgs"/> that contains the event data.</param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            try
            {
                //check for the closing reason
                if (e.CloseReason == CloseReason.UserClosing)
                {
                    //cancel the closing
                    e.Cancel = true;

                    //hide the form no need to dispose and recreate in case opened again
                    _logger.Debug("Hiding the form instead of closing.");
                    this.Hide();

                    IWorkspace loginWorkspaceCl = GetWorkspace();
                    string SQL = "update " + tableName_Tx_form_save + " set form_state = 'CLOSE' , last_filter ='" + FilterComboBox.SelectedItem.ToString() + "', form_location = '" + this.Location.ToString() + "' where LAN_ID = '" + userLanID.ToUpper() + "'";

                    // Update the matching rows.
                    loginWorkspaceCl.ExecuteSQL(SQL);
                }
                else
                {
                    //execute base.OnFormClosing since its not because user is closing this form.
                    _logger.Debug("Closing reason is not UserClosing so executing base.OnFormClosing.");
                    base.OnFormClosing(e);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        #endregion Override for Form

        #region Helper Methods

        private static IWorkspace GetWorkspace()
        {
            // Get and return the logged in workspace
            IMMLoginUtils utils = new MMLoginUtils();
            _loginWorkspace = utils.LoginWorkspace;
            return _loginWorkspace;
        }

        public bool FindLayerByName(String name, out ILayer resLayer)
        {
            try
            {
                Type appRefType = Type.GetTypeFromProgID("esriFramework.AppRef");
                object appRefObj = Activator.CreateInstance(appRefType);
                IApplication arcMapApp = appRefObj as IApplication;
                if (arcMapApp == null)
                {
                    resLayer = null;
                    return false;
                }

                IMxDocument mxDoc = (IMxDocument)arcMapApp.Document;
                IMap map = mxDoc.FocusMap;

                resLayer = FindLayerHelper(map, null, name);
                return resLayer != null;
            }
            catch (Exception ex)
            {
                _logger.Error("PGE Transformer Search , error in function FindLayerByName ", ex);
                resLayer = null;
                return false;
            }
        }

        public ILayer FindLayerHelper(IMap map, ICompositeLayer layers, string lyrName)
        {
            try
            {
                for (int i = 0; i < (map != null ? map.LayerCount : layers.Count); i++)
                {
                    ILayer lyr = map == null ? layers.Layer[i] : map.Layer[i];

                    if (lyr is ICompositeLayer) lyr = FindLayerHelper(null, (ICompositeLayer)lyr, lyrName);

                    if (lyr != null && lyr.Name.ToUpper().Contains(lyrName.ToUpper()))
                    {
                        return lyr;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("PGE Transformer Search, error in function FindLayerHelper ", ex);
            }

            return null;
        }
        #endregion

        private void PGE_TransformerSearchForm_Load(object sender, EventArgs e)
        {
            //Openning the form at the saved from location if user hadn't closed the form previously
            //string sqlFormLoc = "Select Form_Location , LAST_FILTER from EDGIS.TX_SEARCH_FORM_LOC where LAN_ID = '" + userLanID.ToUpper() + "'";
            DataTable dt = PGEExtension.pDataTableFormInfo; ;
            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    System.Drawing.Point p = new System.Drawing.Point();
                    string[] form_location = dt.Rows[0]["FORM_LOCATION"].ToString().Split(',');   //{X=67,Y=190}
                    p.X = Convert.ToInt32(form_location[0].Substring(3));
                    p.Y = Convert.ToInt32(form_location[1].Substring(2).Remove("}"));
                    //p.X = 872; p.Y = 212;

                    this.Location = p;
                    this.FilterComboBox.SelectedItem = dt.Rows[0]["LAST_FILTER"].ToString();
                }
                else
                {
                    IWorkspace loginWorkspace1 = GetWorkspace();
                    string sqlDel = "Delete from " + tableName_Tx_form_save + " where LAN_ID = '" + userLanID.ToUpper() + "'";
                    loginWorkspace1.ExecuteSQL(sqlDel);
                    string sqlInsert = "Insert into EDGIS.TX_SEARCH_FORM_LOC (LAN_ID , FORM_NAME , FORM_STATE , FORM_LOCATION , LAST_FILTER , REC_INSERT_DT) values ('" + userLanID.ToUpper() + "','" + this.Name.ToString() + "','OPEN','" + this.Location.ToString() + "','" + FilterComboBox.SelectedItem.ToString() + "','')";
                    loginWorkspace1.ExecuteSQL(sqlInsert);
                }
            }
        }

        //ME Q4-19 for DA#190903
        private void Clear_Click(object sender, EventArgs e)
        {
            //Clear the selection on Map on Clear button click
           //IMxDocument _mxDoc = (IMxDocument) _AppCopy.Document;
           //IMap _map = _mxDoc.FocusMap;
           //_map.ClearSelection();
           
            //Refresh the active view
           //_map.FeatureSelection.Clear();
           //_mxDoc.ActiveView.Refresh();
            ValueTextBox.Clear();
           ResultLabel.Text = "";
           sProgressor.Message = "";
        }

        //On Form move
        private void PGE_TransformerSearchForm_Move(object sender, EventArgs e)
        {
            try
            {
                IWorkspace loginWorkspace1 = GetWorkspace();
                ITable formtable = ((IFeatureWorkspace)loginWorkspace1).OpenTable(tableName_Tx_form_save);

                //ADODB.Command cmd =  loginWorkspace1.ConnectionProperties;
                System.Security.Principal.WindowsIdentity wi = System.Security.Principal.WindowsIdentity.GetCurrent();

                userLanID = wi.Name.Substring(wi.Name.LastIndexOf("\\") + 1);
                userLanID = userLanID.ToUpper();
                fLocation = this.Location.ToString();
                formName = this.Name.ToString();
                filter = FilterComboBox.SelectedItem.ToString();
                DateTime date = DateTime.Now;

                //string sql = "Insert into EDGIS.TX_SEARCH_FORM_LOC (LAN_ID , FORM_NAME , FORM_STATE , FORM_LOCATION , LAST_FILTER , REC_INSERT_DT) values ('"+userLanID+"','"+formName+"','"+fState+"','"+fLocation+"','"+filter+"','')";
                
                if (firstFrmMove)
                {
                    string SQL = "update " + tableName_Tx_form_save + " set form_state = 'OPEN' , last_filter ='" + FilterComboBox.SelectedItem.ToString() + "', form_location = '" + this.Location.ToString() + "' where LAN_ID = '" + userLanID.ToUpper() + "'";

                    // Update the matching rows.
                    loginWorkspace1.ExecuteSQL(SQL);

                    
                }
            }
            catch (Exception ex) { };
            
        }

       

        


    }
}
