#region References
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
#endregion

namespace PGE.Desktop.EDER.ArcMapCommands.PSPS_CircuitFilter
{
    public partial class PSPS_CircuitFilter_Form : Form
    {
        #region Global varriables decalaration
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        private IApplication _appCopy = null;
        //private static string PSPS_Circuit_table = "EDGIS.PGE_PSPSMAPROWS";
        private static string Circuit_Source_table = "EDGIS.circuitsource";
        private static string PriOHConductor_table = "EDGIS.PRIOHCONDUCTOR";
        private static string PSPS_Circuit_Feeder_table = "EDGIS.CIRCUIT_FEEDER";
        private static string PSPS_lyrDefExp_table = "EDGIS.PGE_PSPS_StoredDisp_lyrDefExp";
        public static IWorkspace loginWorkspace;
        public static ITable PSPSlyrDefExpTbl;
        //public static ITable PSPSMapRowsTbl;
        public static ITable CircuiSourceTbl;
        public static IFeatureClass PriOHConductorClass;
        public static ITable PSPSCircuitFeederTbl;
        //private static List<string> layersList = new List<string>{"Dynamic Protective Device_SEGMENT", "switch_SEGMENT","fuse Segment","Dynamic Protective Device Open","switch Open","fuse Open","Dynamic Protective Device","switch","fuse","PrimaryOH_FIA", "Patrollines"};
        private static List<string> layersList = new List<string>();
        private static List<string> segmentLayersList = new List<string>();
        private static List<string> lyrSourceList = new List<string>();
        private Label label1;
        AutoCompleteStringCollection suggestionData = new AutoCompleteStringCollection();

        #endregion

        #region Constructor
        public PSPS_CircuitFilter_Form(IApplication _App)
        {
            InitializeComponent();
            intializeVarriables();
            CircuitID_TextBox.Enabled = true;
            //Segment_ComboBox.Items.Insert(0, "-SELECT-");
            Segment_ComboBox.Text = "-SELECT-";
            //Segment_ComboBox.SelectedIndex = 0;
            Segment_ComboBox.Enabled = false;
            _appCopy = _App;
            label1.Visible = false;
        }

        #endregion

        #region Global varriable initialization
        private void intializeVarriables()
        {
            try
            {
                loginWorkspace = GetWorkspace();
                //PSPSMapRowsTbl = ((IFeatureWorkspace)loginWorkspace).OpenTable(PSPS_Circuit_table);
                CircuiSourceTbl = ((IFeatureWorkspace)loginWorkspace).OpenTable(Circuit_Source_table);
                PriOHConductorClass = ((IFeatureWorkspace)loginWorkspace).OpenFeatureClass(PriOHConductor_table);
                PSPSCircuitFeederTbl = ((IFeatureWorkspace)loginWorkspace).OpenTable(PSPS_Circuit_Feeder_table);
                PSPSlyrDefExpTbl = ((IFeatureWorkspace)loginWorkspace).OpenTable(PSPS_lyrDefExp_table);
                if (PSPSlyrDefExpTbl != null)
                {
                    layersList.Clear();
                    lyrSourceList.Clear();
                    IQueryFilter pQf = new QueryFilterClass();
                    pQf.SubFields = "*";
                    pQf.WhereClause = "STORED_DISPLAY = 'PSPS Maps'";
                    ICursor pCur = PSPSlyrDefExpTbl.Search(pQf, true);
                    IRow pRow = null;
                    int lyrNmIndex = pCur.FindField("LAYER_NAME");
                    int srcIndex = pCur.FindField("SOURCE_FC_NAME");
                    int segIndex = pCur.FindField("SEGMENT_LAYER");
                    while ((pRow = pCur.NextRow()) != null)
                    {
                        layersList.Add(pRow.get_Value(lyrNmIndex).ToString());
                        lyrSourceList.Add(pRow.get_Value(srcIndex).ToString());
                        if (pRow.get_Value(segIndex).ToString().ToUpper() == "YES")
                        {
                            segmentLayersList.Add(pRow.get_Value(lyrNmIndex).ToString());
                        }
                    }
                    layersList = layersList.Distinct().ToList();
                    lyrSourceList = lyrSourceList.Distinct().ToList();
                    segmentLayersList = segmentLayersList.Distinct().ToList();
                    if (pCur != null)
                    {
                        Marshal.ReleaseComObject(pCur);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public IMap map { get; set; }

        #endregion

        #region Form Load

        private void PSPS_CircuitFilter_Form_Load(object sender, EventArgs e)
        {
            AutoSuggestion_Circuits();
        }

        #endregion

        #region Suggestion Data
        private void AutoSuggestion_Circuits()
        {
            suggestionData = populateCircuitSuggestionData();
            #region CircuitID Suggestions            
            CircuitID_TextBox.AutoCompleteMode = AutoCompleteMode.Suggest;
            CircuitID_TextBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
            CircuitID_TextBox.AutoCompleteCustomSource = suggestionData;
            #endregion
        }

        private AutoCompleteStringCollection populateCircuitSuggestionData()
        {
            AutoCompleteStringCollection col = new AutoCompleteStringCollection();
            IQueryFilter qf = new QueryFilterClass();
            //qf.SubFields = "distinct circuitid, circuitname";
            //qf.WhereClause = "circuitid is not null OR circuitname is not null";
            //ICursor cur = PSPSMapRowsTbl.Search(qf, true);
            qf.SubFields = "circuitid,substationname,circuitname";
            qf.WhereClause = "substationname is not null and circuitname is not null and circuitid is not null";
            ICursor cur = CircuiSourceTbl.Search(qf, true);
            IRow pRow = null;
            int circuitIDIndex = cur.FindField("circuitid");
            int circuitNmIndex = cur.FindField("circuitname");
            int substationNmIndex = cur.FindField("substationname");
            string suggestStr = string.Empty;
            while ((pRow = cur.NextRow()) != null)
            {
                suggestStr = pRow.get_Value(circuitIDIndex).ToString() + "-" + pRow.get_Value(substationNmIndex).ToString() + " " + pRow.get_Value(circuitNmIndex).ToString();
                col.Add(suggestStr);
                suggestStr = pRow.get_Value(substationNmIndex).ToString() + " " + pRow.get_Value(circuitNmIndex).ToString() + "-" + pRow.get_Value(circuitIDIndex).ToString();
                col.Add(suggestStr);
            }
            if (cur != null)
            {
                Marshal.ReleaseComObject(cur);
            }
            return col;
        }

        #endregion

        #region events
        private void CircuitID_TextBox_TextChanged(object sender, EventArgs e)
        {            
            label1.Visible = false;

            foreach (string str in suggestionData)
            {
                if (str.Equals(CircuitID_TextBox.Text))
                {
                    ResetDefinitionExpression();
                    string[] circuitIDStr = CircuitID_TextBox.Text.Split('-');
                    string secondStr = circuitIDStr[1];
                    if (!String.IsNullOrEmpty(secondStr) && Char.IsLetter(secondStr[1]))
                    {
                        FilterLayersByCircuitIDGen(circuitIDStr[0]);
                    }
                    else
                    {
                        FilterLayersByCircuitIDGen(circuitIDStr[1]);
                    }

                    break;
                }
                else
                {
                    Segment_ComboBox.Enabled = false;
                    Segment_ComboBox.Text = "-SELECT-";
                }
                //Segment_ComboBox.DataSource = ;
            }

        }

        private void Segment_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Segment_ComboBox.SelectedItem != null)
            {
                if (Segment_ComboBox.SelectedItem.ToString() != "" && Segment_ComboBox.SelectedItem.ToString() != "-SELECT-")
                {
                    string[] circuitIDStr = CircuitID_TextBox.Text.Split('-');
                    string secondStr = circuitIDStr[1];
                    if (!String.IsNullOrEmpty(secondStr) && Char.IsLetter(secondStr[1]))
                    {
                        ShowSelectedLayersByPSPS_Segment(Segment_ComboBox.SelectedItem, circuitIDStr[0]);
                    }
                    else
                    {
                        ShowSelectedLayersByPSPS_Segment(Segment_ComboBox.SelectedItem, circuitIDStr[1]);
                    }

                }
            }

            else
            {
                //Clear selection if any
                ClearMapSelection(_appCopy);
                //Refresh map
                RefreshMap(_appCopy);
            }

        }

        #endregion

        #region Buttons
        private void Close_Btn_Click(object sender, EventArgs e)
        {
            ResetDefinitionExpression();
            this.Close();
        }

        private void ResetAll_Btn_Click(object sender, EventArgs e)
        {
            if (CircuitID_TextBox.Text != null && CircuitID_TextBox.Text != "")
            {
                ResetDefinitionExpression();
                ClearMapSelection(_appCopy);
                RefreshMap(_appCopy);
                CircuitID_TextBox.Text = string.Empty;
                //Segment_ComboBox.DataSource = null;
                Segment_ComboBox.Text = "-SELECT-";
                Segment_ComboBox.Enabled = false;
            }
        }

        #endregion

        #region private methods

        private void FilterLayersByCircuitIDGen(string circuitID)
        {
            ILayer pLayer = null;

            List<string> circuitListwithFeederFed = new List<string>();
            circuitListwithFeederFed = getFeederFedCircuits(circuitID);
            if (circuitListwithFeederFed.Count > 0)
            {
                circuitID = string.Join(",", circuitListwithFeederFed);
            }

            foreach (string lyr in layersList)
            {
                pLayer = FindLayerByName(lyr);
                IFeatureLayer featLayer = pLayer as IFeatureLayer;
                if (featLayer != null)
                {
                    IDataset ds = featLayer as IDataset;
                    string layerNm = ds.Name.ToUpper();
                    ITableDefinition tableDef = (ITableDefinition)featLayer;
                    string defQuery = tableDef.DefinitionExpression;
                    if (ds.Name.ToUpper() == "Patrollines".ToUpper())
                    {
                        //Data source is FGDB hence sending feature class name as Null
                        setDefinitionExpression(tableDef, circuitID, null);
                    }
                    else
                    {
                        setDefinitionExpression(tableDef, circuitID, ds.BrowseName.ToUpper());
                    }

                }
            }
            ZoomToFilteredPSPSCircuit(circuitID);
        }

        private void ZoomToFilteredPSPSCircuit(string circuitID)
        {
            string whereClause = string.Empty;
            if (circuitID.Contains(","))
            {
                string circuitIDs = null;
                string[] str = circuitID.Split(',');
                for (int i = 0; i < str.Length; i++)
                {
                    circuitIDs = circuitIDs + "'" + str[i] + "',";
                }
                circuitIDs = circuitIDs.TrimEnd(',');
                whereClause = "PSPS_SEGMENT is not null AND PSPS_SEGMENT != 'N/A' AND CIRCUITID IN (" + circuitIDs + ")";
            }
            else
            {
                whereClause = "PSPS_SEGMENT is not null AND PSPS_SEGMENT != 'N/A' AND CIRCUITID IN ('" + circuitID + "')";
            }
            List<string> pspsSegmentList = new List<string>();
            pspsSegmentList.Clear();
            List<string> pspsSegmentListAll = new List<string>();
            pspsSegmentListAll.Clear();
            pspsSegmentListAll.Add("-SELECT-");
            //List<string> XMin_ls = new List<string>();
            //List<string> XMax_ls = new List<string>();
            //List<string> YMin_ls = new List<string>();
            //List<string> YMax_ls = new List<string>();
            //IEnvelope env = new EnvelopeClass();
            if (PriOHConductorClass != null)
            {
                IQueryFilter pQf = new QueryFilterClass();
                //pQf.SubFields = "XMin,Ymin,XMax,YMax,circuitid, pspsname";
                pQf.SubFields = "distinct PSPS_SEGMENT";
                pQf.WhereClause = whereClause;
                IFeatureCursor pCur = PriOHConductorClass.Search(pQf, true);
                //int indXmin = pCur.FindField("XMin");
                //int indYmin = pCur.FindField("Ymin");
                //int indXMax = pCur.FindField("XMax");
                //int indYMax = pCur.FindField("YMax");
                //int indcircuitid = pCur.FindField("circuitid");
                //int indpspsname = pCur.FindField("pspsname");
                int indpspsname = pCur.FindField("PSPS_SEGMENT");
                IField pspsNameField = PriOHConductorClass.Fields.get_Field(indpspsname);
                // query the domain values
                IDictionary<string, string> pspsSegmentDomainDict = GetDomainDictionary(pspsNameField.Domain);
                IFeature pRow = null;
                while ((pRow = pCur.NextFeature()) != null)
                {
                    //ZoomToUserGivenExtents("1778305.45,13786875.17,1798149.78,13800445.57", _appCopy);
                    /*
                    if (pRow.get_Value(indXmin) != null)
                    {
                        if (pRow.get_Value(indXmin).ToString() != "")
                        {
                            XMin_ls.Add(pRow.get_Value(indXmin).ToString());
                        }
                    }
                    if (pRow.get_Value(indYmin) != null)
                    {
                        if (pRow.get_Value(indYmin).ToString() != "")
                        {
                            YMin_ls.Add(pRow.get_Value(indYmin).ToString());
                        }
                    }
                    if (pRow.get_Value(indXMax) != null)
                    {
                        if (pRow.get_Value(indXMax).ToString() != "")
                        {
                            XMax_ls.Add(pRow.get_Value(indXMax).ToString());
                        }
                    }
                    if (pRow.get_Value(indYMax) != null)
                    {
                        if (pRow.get_Value(indYMax).ToString() != "")
                        {
                            YMax_ls.Add(pRow.get_Value(indYMax).ToString());
                        }
                    }
                    */
                    if (pRow.get_Value(indpspsname) != null)
                    {
                        if (pRow.get_Value(indpspsname).ToString() != "")
                        {
                            string PSPSSegmentName = pspsSegmentDomainDict[pRow.get_Value(indpspsname).ToString().ToUpper()];
                            pspsSegmentList.Add(PSPSSegmentName.ToUpper());
                        }
                    }
                }
                if (pCur != null)
                {
                    Marshal.ReleaseComObject(pCur);
                }
                //string strMapExtents = XMin_ls.Min(z => z) + "," + YMin_ls.Min(z => z) + "," + XMax_ls.Max(z => z) + "," + YMax_ls.Max(z => z);

                //ZoomToUserGivenExtents(strMapExtents, _appCopy);
                ZoomToLayers(_appCopy);

                //Populate PSPS Segment drop down list
                if (pspsSegmentList.Count > 1)
                {
                    Segment_ComboBox.Enabled = true;
                    pspsSegmentList.Sort();
                    Segment_ComboBox.DataSource = pspsSegmentListAll.Concat(pspsSegmentList).Distinct().ToList();
                }
                else
                {
                    label1.Font = new Font(Font, FontStyle.Bold);
                    label1.Text = "No PSPS Segment found and It's not PSPS circuit";
                    label1.Visible = true;
                }
                //Refresh map
                RefreshMap(_appCopy);
                //Clear selection if any
                ClearMapSelection(_appCopy);

            }

        }

        private void ZoomToLayers(IApplication appCopy)
        {
            ILayer lyr = null;
            IFeatureLayer fLayer = null;
            IFeatureSelection fsel = null;
            IQueryFilter pQf = null;
            //Clear selection if any
            ClearMapSelection(_appCopy);
            foreach (string lyrNm in layersList)
            {
                lyr = FindLayerByName(lyrNm);
                fLayer = lyr as IFeatureLayer;
                fsel = fLayer as IFeatureSelection;
                pQf = new QueryFilterClass();
                fsel.SelectFeatures(pQf, esriSelectionResultEnum.esriSelectionResultAdd, false);
            }
            UID zoomToSelected = new UIDClass();
            zoomToSelected.Value = "esriArcMapUI.ZoomToSelectedCommand";
            ICommandBars pbars = (ICommandBars)_appCopy.Document.CommandBars;
            ICommandItem pCmd = pbars.Find(zoomToSelected, false, false);
            zoomToSelected.SubType = 3;
            pCmd.Execute();

            double currentMapScale = getCurrentMapScale(_appCopy);
            double gLayer_MinimumScale = 120000;
            ILayer gLayer = getGroupLayerByName(_appCopy, "Segment Devices");
            if (gLayer != null)
            {
                gLayer_MinimumScale = gLayer.MinimumScale;
            }
            if (currentMapScale > gLayer_MinimumScale)
            {
                changeMapZoomInScale(_appCopy, 120000);
            }
            ClearMapSelection(_appCopy);
        }

        private double getCurrentMapScale(IApplication pApp)
        {
            IMxDocument MXDOC = pApp.Document as IMxDocument;
            IMap pMap = MXDOC.FocusMap;
            return pMap.MapScale;
        }

        private void changeMapZoomInScale(IApplication pApp, double scale)
        {
            IMxDocument MXDOC = pApp.Document as IMxDocument;
            IMap pMap = MXDOC.FocusMap;
            pMap.MapScale = scale;
        }

        public IGroupLayer getGroupLayerByName(IApplication pApp, string gLayerName)
        {
            ILayer layer = null;
            ILayer player = null;
            IGroupLayer gLayer = null;
            IMxDocument MXDOC = pApp.Document as IMxDocument;
            IMap pMap = MXDOC.FocusMap;
            IEnumLayer pEnumLayers = null;
            int cnt = pMap.LayerCount;

            pEnumLayers = pMap.Layers;
            string layerNm = null;
            while ((layer = pEnumLayers.Next()) != null)
            {
                layerNm = layer.Name;
                var comLayer = layer as ICompositeLayer;
                gLayer = layer as IGroupLayer;
                if ((comLayer != null) && (gLayer != null) && (layer.Name.Equals(gLayerName)))
                    return gLayer;
            }
            //gLayer = player as IGroupLayer;
            return gLayer;
        }

        private void ZoomToUserGivenExtents(string strExtentsCoordinates, IApplication objApp)
        {
            try
            {
                IMxDocument objMxDoc = (IMxDocument)objApp.Document;

                IEnvelope objEnvlp = objMxDoc.ActiveView.Extent;
                string[] strExtents = Convert.ToString(strExtentsCoordinates).Split(',');
                if ((strExtents != null) & ((strExtents.Length == 4)))
                {
                    objEnvlp.XMin = Convert.ToDouble(strExtents[0]);
                    objEnvlp.YMin = Convert.ToDouble(strExtents[1]);
                    objEnvlp.XMax = Convert.ToDouble(strExtents[2]);
                    objEnvlp.YMax = Convert.ToDouble(strExtents[3]);

                    //IArea aa = (IArea)objEnvlp;
                    //objEnvlp.CenterAt(aa.Centroid);

                    //objEnvlp.Expand(1.2, 1.2, true);

                    objMxDoc.ActiveView.Extent = objEnvlp;
                    //objMxDoc.FocusMap.SelectByShape(objEnvlp, (objApp as IMxApplication).SelectionEnvironment, true);
                    // objMxDoc.ActiveView.Refresh();
                }

            }
            catch (Exception ex)
            {
                //throw CEx;
            }

        }
        private void setDefinitionExpression(ITableDefinition tableDef, string circuitID, string featureClassName)
        {
            string defQuery = tableDef.DefinitionExpression;
            string circuitIDColumn = null;
            string circuitID2Column = null;
            if (featureClassName != null)
            {
                circuitIDColumn = featureClassName + ".CIRCUITID";
                circuitID2Column = featureClassName + ".CIRCUITID2";
            }
            else
            {
                circuitIDColumn = "CIRCUITID";
                circuitID2Column = "CIRCUITID2";
            }

            if (circuitID.Contains(","))
            {
                string circuitIDs = null;
                string[] str = circuitID.Split(',');
                for (int i = 0; i < str.Length; i++)
                {
                    circuitIDs = circuitIDs + "'" + str[i] + "',";
                }
                circuitIDs = circuitIDs.TrimEnd(',');
                tableDef.DefinitionExpression = defQuery + " AND (" + circuitIDColumn + " IN (" + circuitIDs + ") OR " + circuitID2Column + " IN (" + circuitIDs + "))";
            }
            else
            {
                tableDef.DefinitionExpression = defQuery + " AND (" + circuitIDColumn + " IN ('" + circuitID + "') OR " + circuitID2Column + " IN ('" + circuitID + "'))";
            }
        }

        private void ShowSelectedLayersByPSPS_Segment(object selectedSegment, string circuitID)
        {
            ILayer segmentLayer = null;
            string whereClause = string.Empty;
            //Clear selection if any
            ClearMapSelection(_appCopy);
            //get feederFed circuits
            List<string> circuitListwithFeederFed = new List<string>();
            circuitListwithFeederFed = getFeederFedCircuits(circuitID);
            if (circuitListwithFeederFed.Count > 0)
            {
                circuitID = string.Join(",", circuitListwithFeederFed);
            }
            if (circuitID.Contains(","))
            {
                string circuitIDs = null;
                string[] str = circuitID.Split(',');
                for (int i = 0; i < str.Length; i++)
                {
                    circuitIDs = circuitIDs + "'" + str[i] + "',";
                }
                circuitIDs = circuitIDs.TrimEnd(',');
                whereClause = " (CIRCUITID IN (" + circuitIDs + ") OR CIRCUITID2 IN (" + circuitIDs + ")) AND PSPS_SEGMENT = '" + selectedSegment.ToString().Substring(0, 1).ToUpper() + "'";

            }
            else
            {
                whereClause = " (CIRCUITID IN ('" + circuitID + "') OR CIRCUITID2 IN ('" + circuitID + "')) AND PSPS_SEGMENT = '" + selectedSegment.ToString().Substring(0, 1).ToUpper() + "'";
            }

            foreach (string lyrSeg in segmentLayersList)
            {
                segmentLayer = FindLayerByName(lyrSeg);
                IFeatureLayer fLayer = segmentLayer as IFeatureLayer;
                IFeatureSelection fsel = fLayer as IFeatureSelection;
                IQueryFilter pQf = new QueryFilterClass();
                pQf.WhereClause = whereClause;
                //IFeatureCursor pFCur = fLayer.Search(pQf, true);                
                fsel.SelectFeatures(pQf, esriSelectionResultEnum.esriSelectionResultAdd, false);
            }

            UID zoomToSelected = new UIDClass();
            zoomToSelected.Value = "esriArcMapUI.ZoomToSelectedCommand";
            ICommandBars pbars = (ICommandBars)_appCopy.Document.CommandBars;
            ICommandItem pCmd = pbars.Find(zoomToSelected, false, false);
            zoomToSelected.SubType = 3;
            pCmd.Execute();

            double currentMapScale = getCurrentMapScale(_appCopy);
            double gLayer_MinimumScale = 120000;
            ILayer gLayer = getGroupLayerByName(_appCopy, "Segment Devices");
            if (gLayer != null)
            {
                gLayer_MinimumScale = gLayer.MinimumScale;
            }
            if (currentMapScale > gLayer_MinimumScale)
            {
                changeMapZoomInScale(_appCopy, 120000);
            }

            //Refresh map
            RefreshMap(_appCopy);
            //Refresh Attribute Editor after selection
            refreshAttributeEditor(_appCopy);
        }

        private ILayer FindLayerByName(String name)
        {
            ILayer resLayer = null;
            try
            {
                Type appRefType = Type.GetTypeFromProgID("esriFramework.AppRef");
                object appRefObj = Activator.CreateInstance(appRefType);
                IApplication arcMapApp = appRefObj as IApplication;
                if (arcMapApp == null)
                {
                    resLayer = null;
                    return resLayer;
                }
                IMxDocument mxDoc = (IMxDocument)arcMapApp.Document;
                IMap map = mxDoc.FocusMap;
                resLayer = FindLayerHelper(map, null, name);
                return resLayer;
            }
            catch (Exception ex)
            {
                _logger.Error("PSPS Circuit Filter Form , error in function FindLayerByName ", ex);
                resLayer = null;
                return resLayer;
            }
        }

        private ILayer FindLayerHelper(IMap map, ICompositeLayer layers, string lyrName)
        {
            try
            {
                for (int i = 0; i < (map != null ? map.LayerCount : layers.Count); i++)
                {
                    ILayer lyr = map == null ? layers.Layer[i] : map.Layer[i];

                    if (lyr is ICompositeLayer) lyr = FindLayerHelper(null, (ICompositeLayer)lyr, lyrName);

                    if (lyr != null && lyr.Name.ToUpper().Equals(lyrName.ToUpper()))
                    {
                        return lyr;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("PSPS Circuit Filter Form, error in function FindLayerHelper ", ex);
            }

            return null;
        }

        private void RefreshMap(IApplication appCopy)
        {
            IMxDocument MXDOC = appCopy.Document as IMxDocument;
            IMap pMap = MXDOC.FocusMap;
            MXDOC.ActiveView.Refresh();
            appCopy.RefreshWindow();
        }

        private void refreshAttributeEditor(IApplication appCopy)
        {
            IMxDocument MXDOC = appCopy.Document as IMxDocument;
            IMap pMap = MXDOC.FocusMap;
            ISelectionEvents pSelectionEvents;
            pSelectionEvents = pMap as ISelectionEvents;
            pSelectionEvents.SelectionChanged();
        }

        private void ClearMapSelection(IApplication objApp)
        {
            IMxDocument objMxDoc = (IMxDocument)objApp.Document;
            objMxDoc.FocusMap.ClearSelection();
        }

        private static IWorkspace GetWorkspace()
        {
            // Get and return the logged in workspace
            IMMLoginUtils utils = new MMLoginUtils();
            loginWorkspace = utils.LoginWorkspace;
            return loginWorkspace;
        }

        private void ResetDefinitionExpression()
        {
            ILayer rLayer = null;

            if (PSPSlyrDefExpTbl != null)
            {
                IQueryFilter pQf = new QueryFilterClass();
                pQf.SubFields = "*";
                pQf.WhereClause = " STORED_DISPLAY = 'PSPS Maps'";
                ICursor pCur = PSPSlyrDefExpTbl.Search(pQf, true);
                IRow pRow = null;
                int lyrNmIndex = pCur.FindField("LAYER_NAME");
                int defExpIndex = pCur.FindField("DEF_EXPRESSION");
                while ((pRow = pCur.NextRow()) != null)
                {
                    rLayer = FindLayerByName(pRow.get_Value(lyrNmIndex).ToString());
                    IFeatureLayer featLayer = rLayer as IFeatureLayer;
                    if (featLayer != null)
                    {
                        IDataset ds = featLayer as IDataset;
                        string layerNm = ds.Name.ToUpper();
                        ITableDefinition tableDef = (ITableDefinition)featLayer;
                        tableDef.DefinitionExpression = pRow.get_Value(defExpIndex).ToString();
                    }
                }
                //Refresh Active view map
                RefreshMap(_appCopy);
                if (pCur != null)
                {
                    Marshal.ReleaseComObject(pCur);
                }
            }

        }

        //Logic to include feeder fed circuits to list
        private List<string> getFeederFedCircuits(string circuitId)
        {
            List<string> circuitIDList = new List<string>();
            circuitIDList.Clear();
            circuitIDList.Add(circuitId);
            List<string> circuitListFeederFed = new List<string>();
            circuitListFeederFed.Clear();
            string selectSql = string.Empty;
            bool parentFeeder = false;
            //Check for Parent feeder first                
            //selectSql = "SELECT " + CircuitID_Column + " FROM " + PSPS_Circuit_Table + " WHERE " + FeederFedBy_CircuitID_Column + " = '" + circuitId + "'";
            List<string> circuitList = getQueryResultListFromTable(PSPSCircuitFeederTbl, "CIRCUITID", "FEEDER_CIRCUIT = '" + circuitId + "'");
            if (circuitList != null)
            {
                if (circuitList.Count > 0)
                {
                    parentFeeder = true;
                    // add feeder fed circuit to this list circuitListFeederFed
                    foreach (string str in circuitList)
                    {
                        circuitListFeederFed.Add(str);
                    }
                }
            }
            //If this circuitid is not found in parent feeder then check in child circuit column to get parent feeder circuit id
            if (!parentFeeder)
            {
                //selectSql = "SELECT DISTINCT " + FeederFedBy_CircuitID_Column + " FROM " + PSPS_Circuit_Table + " WHERE " + CircuitID_Column + " = '" + circuitId + "'";
                List<string> circuitListParent = getQueryResultListFromTable(PSPSCircuitFeederTbl, "FEEDER_CIRCUIT", "CIRCUITID = '" + circuitId + "'");
                if (circuitListParent != null)
                {
                    if (circuitListParent.Count > 0)
                    {
                        //string selectSql1 = "SELECT DISTINCT " + CircuitID_Column + " FROM " + PSPS_Circuit_Table + " WHERE " + FeederFedBy_CircuitID_Column + " = '" + circuitDTParent.Rows[0][0].ToString() + "'";
                        string circuitParent = string.Empty;
                        foreach (string s in circuitListParent)
                        {
                            circuitParent = s;
                        }
                        List<string> circuitListChild = getQueryResultListFromTable(PSPSCircuitFeederTbl, "CIRCUITID", "FEEDER_CIRCUIT = '" + circuitParent + "'");
                        if (circuitListChild != null)
                        {
                            if (circuitListChild.Count > 0)
                            {
                                // add feeder fed circuit to this list circuitListFeederFed                                
                                foreach (string strChild in circuitListChild)
                                {
                                    circuitListFeederFed.Add(strChild);
                                }
                            }
                        }
                    }
                }

            }


            if (circuitListFeederFed.Count > 0)
            {
                //Append child circuits list to its parent lists
                return circuitIDList.Concat(circuitListFeederFed).ToList().Distinct().ToList();
            }
            else
            {
                return circuitIDList;
            }
        }

        private List<string> getQueryResultListFromTable(ITable table, string attribute, string whereClause)
        {
            ICursor tableCursor = null;
            IQueryFilter queryFilter = new QueryFilter();
            List<string> resultList = new List<string>();
            resultList.Clear();
            try
            {
                if (table != null)
                {
                    attribute = attribute.Trim();
                    queryFilter.WhereClause = whereClause;
                    tableCursor = table.Search(queryFilter, true);
                    IRow tmpRow = null;

                    if (tableCursor.FindField(attribute) == -1)
                    {
                        return resultList;
                    }
                    while ((tmpRow = tableCursor.NextRow()) != null)
                    {
                        if (tmpRow.get_Value(tmpRow.Fields.FindField(attribute)) == null)
                        {
                            //
                        }
                        else
                            resultList.Add(tmpRow.get_Value(tmpRow.Fields.FindField(attribute)).ToString());
                    }
                }

            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (tableCursor != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(tableCursor);
                }
            }
            return resultList;
        }

        private IDictionary<string, string> GetDomainDictionary(IDomain domain)
        {
            IDictionary<string, string> domainDict = new Dictionary<string, string>();
            ICodedValueDomain codedDomain = domain as ICodedValueDomain;

            for (int i = 0; i < codedDomain.CodeCount - 1; i++)
            {
                domainDict.Add(codedDomain.get_Value(i).ToString(), codedDomain.get_Name(i));
            }
            return domainDict;
        }



        #endregion
                
    }
}
