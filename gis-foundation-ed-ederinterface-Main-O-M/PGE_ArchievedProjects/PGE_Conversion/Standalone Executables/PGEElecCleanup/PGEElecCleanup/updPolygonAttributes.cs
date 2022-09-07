using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Oracle.DataAccess.Client;
using System.IO;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using System.Collections;
using System.Configuration;
using System.Runtime.InteropServices;
using Miner.Interop;
using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.CatalogUI;
using ESRI.ArcGIS.Catalog;
using System.Diagnostics;
namespace PGEElecCleanup
{
    public partial class updPolygonAttributes : Form
    {
        private IMMAutoUpdater autoupdater = null;
        private DataTable objReportTable = new DataTable();
        clsGlobalFunctions _clsGlobalFunctions = new clsGlobalFunctions();
        ITable pPolygonFC_County = null;
        ITable pPolygonFC_District = null;
        ITable pPolygonFC_Divison = null;
        ITable pPolygonFC_Post_District = null;
        Dictionary<string, int> dist_Division_Vlas = new Dictionary<string, int>();
        Dictionary<string, int> dist_District_Vlas = new Dictionary<string, int>();
        Dictionary<string, int> dist_County_Vlas = new Dictionary<string, int>();
        List<string> lstFcNamesList = new List<string>();


        int intUpdCount = 0;
        int intUpdCntToStop = 0;

        public updPolygonAttributes()
        {
            InitializeComponent();
        }

        private void updPolygonAttributes_Load(object sender, EventArgs e)
        {
            //cmbAllFC.Items.Add("Division");
            //cmbAllFC.Items.Add("District");
            //cmbAllFC.Items.Add("County");
            //cmbAllFC.Items.Add("Zip");

            getFcName(ref  lstFcNamesList);

        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
               //Stopwatch totalExecutionTime = new Stopwatch();
                //totalExecutionTime.Start();
                btnStart.Enabled = false;
                btnExit.Enabled = false;
                btnCheckAll.Enabled = false;
                button1.Enabled = false;
                statusLabel.Text = "Starting Process..";
                Logs.writeLine(DateTime.Now + ": Starting Processing");
                //stbInfo.Text = "Disable Autoupdaters ";
                //#region "Disable Autoupdaters "
                //mmAutoUpdaterMode objautoupdateroldmode = DisableAutoupdaters();
                //#endregion

                string strChildTabName = string.Empty;
                objReportTable.Clear();
                objReportTable.Columns.Clear();
                _clsGlobalFunctions.Common_initSummaryTable("updPolygonAttributes", "updPolygonAttributes");
                _clsGlobalFunctions.Common_addColumnToReportTable(objReportTable, "FCNAME,GLOBALID,COUNTY,DISTRICT,DIVISION,ZIP,CITY,TAB_QRY,A_QRY");

                if (txtFGDpath.Text == "")
                {
                    MessageBox.Show("Could not able to read selected FGD.please check.", "Report", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                //IWorkspace pCedsaWsp = null;
                //pCedsaWsp = workspaceFactory.OpenFromFile(txtFGDpath.Text, 0);

                IWorkspaceFactory workspaceFactory = new FileGDBWorkspaceFactoryClass();
                IWorkspaceFactory pWSFactory = new SdeWorkspaceFactoryClass();
                IWorkspace pLBWS = null;

                string strPath = txtFGDpath.Text;
                if (strPath.ToUpper().EndsWith(".SDE"))
                {
                    pLBWS = pWSFactory.OpenFromFile(txtFGDpath.Text, 0);
                }
                if (strPath.ToUpper().EndsWith(".GDB"))
                {
                    pLBWS = workspaceFactory.OpenFromFile(txtFGDpath.Text, 0);
                }

                if (pLBWS == null)
                {
                    MessageBox.Show("Could not able to read selected Landbase database.please check.", "Report", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                IFeatureWorkspace pLBFws = (IFeatureWorkspace)pLBWS;

                //clsTestWorkSpace.StartEditOperation();
                //Logs.writeLine("StartEdit the database");
                addDomainvalues_to_dist();

                //chkFCListBox
                List<string> lstCheckedFCNames = new List<string>();
                for (int itr = 0; itr < chkFCListBox.CheckedItems.Count; itr++)
                {
                    lstCheckedFCNames.Add(chkFCListBox.CheckedItems[itr].ToString());
                }

                startprocess(pLBFws, lstCheckedFCNames);

                if (errors.Count > 0)
                {
                    Logs.writeLine("");
                    Logs.writeLine("The following errors were encountered during processing.");
                    foreach (string error in errors)
                    {
                        Logs.writeLine(error);
                    }
                }

                //processEachTable(cmbAllFC.SelectedItem.ToString(), strPolyFeatName);               
                //clsTestWorkSpace.StopEditOperation(true);

                //stbInfo.Text = "Enabling autoupdaters...";
                //#region start AU
                //if (autoupdater != null)
                //{
                //    autoupdater.AutoUpdaterMode = objautoupdateroldmode;
                //}
                //#endregion

               //totalExecutionTime.Stop();
                //MessageBox.Show("Total time to execute: " + totalExecutionTime.ElapsedMilliseconds / 1000.0);
                statusLabel.Text = "Process completed see the log file.";
                Logs.writeLine("Successfully Completed");
                MessageBox.Show("Process Completed, please see the Report File.", "Report", MessageBoxButtons.OK, MessageBoxIcon.Information);

                btnStart.Enabled = true;
                btnExit.Enabled = true;
            }
            catch (Exception ex)
            {
                Logs.writeLine("Error on process start button " + ex.Message);
                statusLabel.Text = "Error occurred, Please see the log file.";
            }
            finally
            {
                Logs.writeLine(DateTime.Now + ": Finished Processing");
                btnStart.Enabled = true;
                btnExit.Enabled = true;
                btnCheckAll.Enabled = true;
                button1.Enabled = true;
            }
        }
        //function to add domine values to dist
        private void addDomainvalues_to_dist()
        {
            dist_Division_Vlas.Clear();
            //Division
            //
            dist_Division_Vlas.Add("NORTH VALLEY", 2);
            dist_Division_Vlas.Add("SACRAMENTO", 3);
            dist_Division_Vlas.Add("SIERRA", 4);
            dist_Division_Vlas.Add("NORTH BAY", 5);
            dist_Division_Vlas.Add("EAST BAY", 6);
            dist_Division_Vlas.Add("SAN FRANCISCO", 7);
            dist_Division_Vlas.Add("PENINSULA", 8);
            dist_Division_Vlas.Add("DIABLO", 9);
            dist_Division_Vlas.Add("STOCKTON", 10);
            dist_Division_Vlas.Add("YOSEMITE", 11);
            dist_Division_Vlas.Add("FRESNO", 12);
            dist_Division_Vlas.Add("KERN", 13);
            dist_Division_Vlas.Add("MISSION", 14);
            dist_Division_Vlas.Add("SAN JOSE", 15);
            dist_Division_Vlas.Add("DE ANZA", 16);
            dist_Division_Vlas.Add("CENTRAL COAST", 17);
            dist_Division_Vlas.Add("LOS PADRES", 18);
            //North Coast,Skyline,Eureka - not mapping
            //
            //County
            dist_County_Vlas.Clear();
            dist_County_Vlas.Add("ALAMEDA", 1);
            dist_County_Vlas.Add("ALPINE", 2);
            dist_County_Vlas.Add("AMADOR", 3);
            dist_County_Vlas.Add("BUTTE", 4);
            dist_County_Vlas.Add("CALAVERAS", 5);
            dist_County_Vlas.Add("COLUSA", 6);
            dist_County_Vlas.Add("CONTRA COSTA", 7);
            dist_County_Vlas.Add("DEL NORTE", 8);
            dist_County_Vlas.Add("EL DORADO", 9);
            dist_County_Vlas.Add("FRESNO", 10);
            dist_County_Vlas.Add("GLENN", 11);
            dist_County_Vlas.Add("HUMBOLDT", 12);
            dist_County_Vlas.Add("IMPERIAL", 13);
            dist_County_Vlas.Add("INYO", 14);
            dist_County_Vlas.Add("KERN", 15);
            dist_County_Vlas.Add("KINGS", 16);
            dist_County_Vlas.Add("LAKE", 17);
            dist_County_Vlas.Add("LASSEN", 18);
            dist_County_Vlas.Add("LOS ANGELES", 19);
            dist_County_Vlas.Add("MADERA", 20);
            dist_County_Vlas.Add("MARIN", 21);
            dist_County_Vlas.Add("MARIPOSA", 22);
            dist_County_Vlas.Add("MENDOCINO", 23);
            dist_County_Vlas.Add("MERCED", 24);
            dist_County_Vlas.Add("MODOC", 25);
            dist_County_Vlas.Add("MONO", 26);
            dist_County_Vlas.Add("MONTEREY", 27);
            dist_County_Vlas.Add("NAPA", 28);
            dist_County_Vlas.Add("NEVADA", 29);
            dist_County_Vlas.Add("ORANGE", 30);
            dist_County_Vlas.Add("PLACER", 31);
            dist_County_Vlas.Add("PLUMAS", 32);
            dist_County_Vlas.Add("RIVERSIDE", 33);
            dist_County_Vlas.Add("SACRAMENTO", 34);
            dist_County_Vlas.Add("SAN BENITO", 35);
            dist_County_Vlas.Add("SAN BERNARDINO", 36);
            dist_County_Vlas.Add("SAN DIEGO", 37);
            dist_County_Vlas.Add("SAN FRANCISCO", 38);
            dist_County_Vlas.Add("SAN JOAQUIN", 39);
            dist_County_Vlas.Add("SAN LUIS OBISPO", 40);
            dist_County_Vlas.Add("SAN MATEO", 41);
            dist_County_Vlas.Add("SANTA BARBARA", 42);
            dist_County_Vlas.Add("SANTA CLARA", 43);
            dist_County_Vlas.Add("SANTA CRUZ", 44);
            dist_County_Vlas.Add("SHASTA", 45);
            dist_County_Vlas.Add("SIERRA", 46);
            dist_County_Vlas.Add("SISKIYOU", 47);
            dist_County_Vlas.Add("SOLANO", 48);
            dist_County_Vlas.Add("SONOMA", 49);
            dist_County_Vlas.Add("STANISLAUS", 50);
            dist_County_Vlas.Add("SUTTER", 51);
            dist_County_Vlas.Add("TEHAMA", 52);
            dist_County_Vlas.Add("TRINITY", 53);
            dist_County_Vlas.Add("TULARE", 54);
            dist_County_Vlas.Add("TUOLUMNE", 55);
            dist_County_Vlas.Add("VENTURA", 56);
            dist_County_Vlas.Add("YOLO", 57);
            dist_County_Vlas.Add("YUBA", 58);
            //District
            dist_District_Vlas.Clear();
            dist_District_Vlas.Add("SANTA MARIA", 1);
            dist_District_Vlas.Add("SAN LUIS OBISPO", 2);
            dist_District_Vlas.Add("PASO ROBLES", 3);
            dist_District_Vlas.Add("KING CITY", 4);
            dist_District_Vlas.Add("SALINAS", 5);
            dist_District_Vlas.Add("MONTEREY", 7);
            dist_District_Vlas.Add("HOLLISTER", 8);
            dist_District_Vlas.Add("COLUSA", 9);
            dist_District_Vlas.Add("MARYSVILLE", 11);
            dist_District_Vlas.Add("OROVILLE", 13);
            dist_District_Vlas.Add("CHICO", 16);
            dist_District_Vlas.Add("PARADISE", 17);
            dist_District_Vlas.Add("PLACER", 20);
            dist_District_Vlas.Add("EL DORADO", 21);
            dist_District_Vlas.Add("NEVADA", 22);
            dist_District_Vlas.Add("MISSION", 24);
            dist_District_Vlas.Add("CENTRAL", 26);
            dist_District_Vlas.Add("BAY", 28);
            dist_District_Vlas.Add("DIABLO", 30);
            dist_District_Vlas.Add("GARBERVILLE", 34);
            dist_District_Vlas.Add("EUREKA", 35);
            dist_District_Vlas.Add("FORTUNA", 36);
            dist_District_Vlas.Add("WILLOW CREEK", 38);
            dist_District_Vlas.Add("NORTH BAY", 40);
            dist_District_Vlas.Add("VALLEJO-NAPA", 42);
            dist_District_Vlas.Add("SANTA ROSA", 44);
            dist_District_Vlas.Add("UKIAH", 46);
            dist_District_Vlas.Add("SOLANO", 48);
            dist_District_Vlas.Add("YOLO", 50);
            dist_District_Vlas.Add("SACRAMENTO", 52);
            dist_District_Vlas.Add("SKYLINE", 56);
            dist_District_Vlas.Add("PENINSULA", 58);
            dist_District_Vlas.Add("SAN FRANCISCO", 60);
            dist_District_Vlas.Add("KERN", 66);
            dist_District_Vlas.Add("FRESNO", 70);
            dist_District_Vlas.Add("YOSEMITE", 72);
            dist_District_Vlas.Add("SAN JOSE", 78);
            dist_District_Vlas.Add("DE ANZA", 80);
            dist_District_Vlas.Add("STANISLAUS", 91);
            dist_District_Vlas.Add("DELTA", 93);
            dist_District_Vlas.Add("MOTHER LODE", 95);



        }

        //private void getpolygonName(ref string strPolyBoundaryname)
        //{
        //    strPolyBoundaryname = string.Empty;
        //    try
        //    {

        //        if (cmbAllFC.SelectedItem.ToString().ToUpper() == "District".ToUpper())
        //        {
        //            strPolyBoundaryname = "Districts".ToUpper();
        //        }
        //        else if (cmbAllFC.SelectedItem.ToString().ToUpper() == "Division".ToUpper())
        //        {
        //            strPolyBoundaryname = "Divison".ToUpper();

        //        }
        //        else if (cmbAllFC.SelectedItem.ToString().ToUpper() == "County".ToUpper())
        //        {
        //            strPolyBoundaryname = "County".ToUpper();

        //        }
        //        else if (cmbAllFC.SelectedItem.ToString().ToUpper() == "Zip".ToUpper())
        //        {
        //            strPolyBoundaryname = "Post_District".ToUpper();

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Logs.writeLine("Error on getpolygonName " + ex.Message);

        //    }
        //}

        private void startprocess(IFeatureWorkspace pLBFws, List<string> lstCheckedFCNames)
        {

            //string FieldName = cmbAllFC.SelectedItem.ToString();
            //List<string> lstPtFeatclassNames = new List<string>();
            List<IFeatureClass> lstPtFeatclassNames = new List<IFeatureClass>();
            //Function to get all point featureclasses from SDE

            int intLandbase_County_idx = -1;
            int intLandbase_Districts_idx = -1;
            int intLandbase_Divison_idx = -1;
            int intLandbase_ZIP_idx = -1;
            int intLandbase_CITY_idx = -1; // CITY comes from postdistrict polygon of NAME field

            string strLandbase_County = string.Empty;
            string strLandbase_Districts = string.Empty;
            string strLandbase_Divison = string.Empty;
            string strLandbase_ZIP = string.Empty;
            string strLandbase_CITY = string.Empty;
            string strReportVals = string.Empty;

            string strGis_Globalid = string.Empty;
            string strGis_County = string.Empty;
            string strGis_Districts = string.Empty;
            string strGis_Divison = string.Empty;
            string strGis_ZIP = string.Empty;
            string strGis_CITY = string.Empty;

            string strFCName = string.Empty;
            string strUpdQry = string.Empty;
            string strA_UpdQry = string.Empty;
            List<string> lstUpdVals = new List<string>();
            string strTabName = string.Empty;
            string strA_TabName = string.Empty;

            try
            {


                if (txtFGDpath.Text.ToUpper().EndsWith(".SDE"))
                {
                    pPolygonFC_County = pLBFws.OpenFeatureClass("LBGIS.CountyUnClipped") as ITable;//County
                    pPolygonFC_District = pLBFws.OpenFeatureClass("LBGIS.ElecMCDistricts") as ITable;//Districts
                    pPolygonFC_Divison = pLBFws.OpenFeatureClass("LBGIS.ElecDivision") as ITable;//Divison
                    pPolygonFC_Post_District = pLBFws.OpenFeatureClass("LBGIS.postdist") as ITable; //Post_District
                }
                if (txtFGDpath.Text.ToUpper().EndsWith(".GDB"))
                {
                    pPolygonFC_County = pLBFws.OpenFeatureClass("County") as ITable;//County
                    pPolygonFC_District = pLBFws.OpenFeatureClass("Districts") as ITable;//Districts
                    pPolygonFC_Divison = pLBFws.OpenFeatureClass("Divison") as ITable;//Divison
                    pPolygonFC_Post_District = pLBFws.OpenFeatureClass("Post_District") as ITable; //Post_District
                }

                if (pPolygonFC_County == null || pPolygonFC_District == null || pPolygonFC_Divison == null || pPolygonFC_Post_District == null)
                {
                    MessageBox.Show(" One of featureclasses (County,Districts,Divison,Post_District) not found in the input Database. Please check.", "Report", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                intLandbase_County_idx = pPolygonFC_County.FindField("CNTY_NAME");
                intLandbase_Districts_idx = pPolygonFC_District.FindField("District");
                intLandbase_Divison_idx = pPolygonFC_Divison.FindField("DIVISION");
                intLandbase_ZIP_idx = pPolygonFC_Post_District.FindField("POSTCODE");
                intLandbase_CITY_idx = pPolygonFC_Post_District.FindField("NAME"); //Name =City in GIS

                if (intLandbase_CITY_idx == -1 || intLandbase_County_idx == -1 || intLandbase_Districts_idx == -1 || intLandbase_Divison_idx == -1 || intLandbase_ZIP_idx == -1)
                {
                    MessageBox.Show(" One of the field (NAME(CITY),CNTY_NAME,District,Divison,POSTCODE) not found in the respective classes.please check.", "Report", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                IDataset postDistDS = pPolygonFC_Post_District as IDataset;
                IDataset districtDS = pPolygonFC_District as IDataset;
                IDataset divisionDS = pPolygonFC_Divison as IDataset;
                IDataset countyDS = pPolygonFC_County as IDataset;
                string postDistDSName = postDistDS.BrowseName;
                string districtDSName = districtDS.BrowseName;
                string divisionDSName = divisionDS.BrowseName;
                string countyDSName = countyDS.BrowseName;
                for (int itr = 0; itr < lstCheckedFCNames.Count; itr++)
                {
                    double overallPercentComplete = Math.Round((((double)itr) / ((double)lstCheckedFCNames.Count)) * 100.0, 1);
                    string overallStatus = "Total Completed: " + overallPercentComplete + "%";
                    strTabName = lstCheckedFCNames[itr].ToString().Split('|')[1];
                    strA_TabName = lstCheckedFCNames[itr].ToString().Split('|')[0];

                    Logs.writeLine(DateTime.Now + ": Starting Processing " + strTabName);

                    ITable pfc = clsTestWorkSpace.FeatureWorkspace.OpenFeatureClass(strTabName) as ITable;

                    Logs.writeLine(DateTime.Now + "     Processing: " + postDistDSName);
                    LandbaseSpatialQuery(overallStatus, ref pPolygonFC_Post_District, ref pfc, false, false, true, true, false);
                    Logs.writeLine(DateTime.Now + "     Processing: " + districtDSName);
                    LandbaseSpatialQuery(overallStatus, ref pPolygonFC_District, ref pfc, true, false, false, false, false);
                    Logs.writeLine(DateTime.Now + "     Processing: " + divisionDSName);
                    LandbaseSpatialQuery(overallStatus, ref pPolygonFC_County, ref pfc, false, false, false, false, true);
                    Logs.writeLine(DateTime.Now + "     Processing: " + countyDSName);
                    LandbaseSpatialQuery(overallStatus, ref pPolygonFC_Divison, ref pfc, false, true, false, false, false);

                    Logs.writeLine(DateTime.Now + "     Writing to output csv");
                    string strTime = DateTime.Today.Day + "-" + DateTime.Now.Hour + "-" + DateTime.Now.Minute + "-" + DateTime.Now.Second;
                    bool firstWrite = _clsGlobalFunctions.Common_addRowstoReportTable(objReportTable, CleanupInfoRecords(), strTabName, strA_TabName, "PolygonAttributes_report_" + strTabName, strTime);
                    clsGFMSGlobalFunctions._GFMSGlobalFunctions.GenerateTheReport_MultipleFeatureClass(objReportTable, "PolygonAttributes_report_" + strTabName, firstWrite, strTime);
                    //generateSQLfile(objReportTable, "PolygonAttributes_report_" + strTabName, strTime);
                    objReportTable.Rows.Clear();
                    featureInfo.Clear();

                    if (pfc != null) { while (Marshal.ReleaseComObject(pfc) > 0) { } }
                    GC.Collect();

                    Logs.writeLine(DateTime.Now + ": Finished Processing " + strTabName);
                }

            }
            catch (Exception ex)
            {
                Logs.writeLine("Error at startprocess " + ex.Message);

            }
            finally
            {
            }
        }

        public List<FeatureInfo> CleanupInfoRecords()
        {
            Dictionary<string, FeatureInfo> uniqueFeatureInfoValues = new Dictionary<string, FeatureInfo>();
            foreach (FeatureInfo info in featureInfo)
            {
                FeatureInfo uniqueInfo;
                if (uniqueFeatureInfoValues.ContainsKey(info.globalID))
                {
                    uniqueInfo = uniqueFeatureInfoValues[info.globalID];
                }
                else
                {
                    uniqueInfo = new FeatureInfo();
                }

                uniqueInfo.globalID = info.globalID;
                if (string.IsNullOrEmpty(uniqueInfo.County)) { uniqueInfo.County = info.County; }
                if (string.IsNullOrEmpty(uniqueInfo.Zip)) { uniqueInfo.Zip = info.Zip; }
                if (string.IsNullOrEmpty(uniqueInfo.Division)) { uniqueInfo.Division = info.Division; }
                if (string.IsNullOrEmpty(uniqueInfo.District)) { uniqueInfo.District = info.District; }
                if (string.IsNullOrEmpty(uniqueInfo.City)) { uniqueInfo.City = info.City; }
                uniqueFeatureInfoValues[info.globalID] = uniqueInfo;
            }
            return uniqueFeatureInfoValues.Values.ToList();
        }

        public struct FeatureInfo
        {
            public string globalID;
            public string District;
            public string Division;
            public string City;
            public string Zip;
            public string County;
        }

        List<string> errors = new List<string>();
        Dictionary<string, Dictionary<string, int>> FeatureClassToFieldIndices = new Dictionary<string, Dictionary<string, int>>();
        
        List<FeatureInfo> featureInfo = new List<FeatureInfo>();
        //Dictionary<string, string> QueriesByGlobalID = new Dictionary<string,string>();
        private void LandbaseSpatialQuery(string overallStatus, ref ITable landbaseTableToQuery, ref ITable featureClassToQuery, bool getDistrict, bool getDivision, bool getZip, bool getCity, bool getCounty)
        {
            int landbaseCountyIdx = -1;
            int landbaseDistrictIdx = -1;
            int landbaseDivisionIdx = -1;
            int landbaseCityIdx = -1;
            int landbaseZipIdx = -1;
            int globalIdx = -1;
            int countyIdx = -1;
            int districtIdx = -1;
            int divisionIdx = -1;
            int cityIdx = -1;
            int zipIdx = -1;
            int landbaseShapeIdx = GetIndex(ref landbaseTableToQuery, "SHAPE");
            int shapeIdx = GetIndex(ref featureClassToQuery, "SHAPE");
            globalIdx = GetIndex(ref featureClassToQuery, "GLOBALID");
            IDataset ds = featureClassToQuery as IDataset;
            IDataset landbaseDS = landbaseTableToQuery as IDataset;
            IQueryFilter qf = new QueryFilterClass();
            //string subFields = "SHAPE,GLOBALID";
            qf.SubFields = "SHAPE";
            if (getDistrict)
            {
                qf.SubFields += ",DISTRICT";
                //subFields += ",DISTRICT";
                landbaseDistrictIdx = GetIndex(ref landbaseTableToQuery, "DISTRICT");
                try
                {
                    districtIdx = GetIndex(ref featureClassToQuery, "DISTRICT");
                }
                catch
                {
                    string error = ds.BrowseName + " does not contain the DISTRICT field";
                    if (!errors.Contains(error)) { errors.Add(error); }
                    return;
                }
            }
            if (getDivision) 
            {
                qf.SubFields += ",DIVISION";
                //subFields += ",DIVISION";
                landbaseDivisionIdx = GetIndex(ref landbaseTableToQuery, "DIVISION");
                try
                {
                    divisionIdx = GetIndex(ref featureClassToQuery, "DIVISION");
                }
                catch
                {
                    string error = ds.BrowseName + " does not contain the DIVISION field";
                    if (!errors.Contains(error)) { errors.Add(error); }
                    return;
                }
            }
            if (getZip) 
            {
                qf.SubFields += ",POSTCODE";
                //subFields += ",ZIP";
                landbaseZipIdx = GetIndex(ref landbaseTableToQuery, "POSTCODE");
                try
                {
                    zipIdx = GetIndex(ref featureClassToQuery, "ZIP");
                }
                catch
                {
                    try { cityIdx = GetIndex(ref featureClassToQuery, "CITY"); }
                    catch { }
                    string error = ds.BrowseName + " does not contain the ZIP field";
                    if (!errors.Contains(error)) { errors.Add(error); }
                    if (cityIdx < 0) { return; }
                }
            }
            if (getCity) 
            {
                qf.SubFields += ",NAME";
                //subFields += ",CITY";
                landbaseCityIdx = GetIndex(ref landbaseTableToQuery, "NAME");
                try
                {
                    cityIdx = GetIndex(ref featureClassToQuery, "CITY");
                }
                catch
                {
                    string error = ds.BrowseName + " does not contain the CITY field";
                    if (!errors.Contains(error)) { errors.Add(error); }
                    if (zipIdx < 0) { return; }
                }
            }
            if (getCounty)
            {
                qf.SubFields += ",CNTY_NAME";
                //subFields += ",COUNTY";
                landbaseCountyIdx = GetIndex(ref landbaseTableToQuery, "CNTY_NAME");
                try
                {
                    countyIdx = GetIndex(ref featureClassToQuery, "COUNTY");
                }
                catch
                {
                    string error = ds.BrowseName + " does not contain the COUNTY field";
                    if (!errors.Contains(error)) { errors.Add(error); }
                    return;
                }
            }
            int processed = 0;
            int totalFeatures = 0;
            totalFeatures = landbaseTableToQuery.RowCount(qf);
            ICursor landbaseCursor = landbaseTableToQuery.Search(qf, false);
            IFeature row = null;
            try
            {
                using (ComReleaser cr = new ComReleaser())
                {
                    cr.ManageLifetime(landbaseCursor);
                    while ((row = landbaseCursor.NextRow() as IFeature) != null)
                    {
                        cr.ManageLifetime(row);
                        try
                        {
                            processed++;

                            string description = "";
                            int county = -1;
                            int district = -1;
                            int division = -1;
                            string city = "";
                            string zip = "";
                            ISpatialFilter sf = new SpatialFilterClass();                            
                            sf.Geometry = row.Shape;
                            sf.SubFields = "GLOBALID";
                            sf.GeometryField = "SHAPE";
                            sf.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                            string whereClause = "";
                            if (getCounty)
                            {
                                string countyName = row.get_Value(landbaseCountyIdx).ToString();
                                description += "County:" + countyName;
                                try
                                {
                                    county = dist_County_Vlas[row.get_Value(landbaseCountyIdx).ToString().ToUpper()];
                                    GetWhereClause("COUNTY", county.ToString(), ref whereClause);
                                }
                                catch (Exception ex)
                                {
                                    //If a domain value doesn't exist we can't do anything for it.
                                    string error = "Unable to find county in domain: " + countyName;
                                    if (!errors.Contains(error)) { errors.Add(error); }
                                    continue;
                                }
                            }
                            if (getDistrict)
                            {
                                string districtName = row.get_Value(landbaseDistrictIdx).ToString();
                                description += "District:" + districtName;
                                try
                                {
                                    district = dist_District_Vlas[row.get_Value(landbaseDistrictIdx).ToString().ToUpper()];
                                    GetWhereClause("DISTRICT", district.ToString(), ref whereClause);
                                }
                                catch (Exception ex)
                                {
                                    //If a domain value doesn't exist we can't do anything for it.
                                    string error = "Unable to find district in domain: " + districtName;
                                    if (!errors.Contains(error)) { errors.Add(error); }
                                    continue;
                                }
                            }
                            if (getDivision)
                            {
                                string divisionName = row.get_Value(landbaseDivisionIdx).ToString();
                                description += "Division:" + divisionName;
                                try
                                {
                                    division = dist_Division_Vlas[row.get_Value(landbaseDivisionIdx).ToString().ToUpper()];
                                    GetWhereClause("DIVISION", division.ToString(), ref whereClause);
                                }
                                catch (Exception ex)
                                {
                                    //If a domain value doesn't exist we can't do anything for it.
                                    string error = "Unable to find division in domain: " + divisionName;
                                    if (!errors.Contains(error)) { errors.Add(error); }
                                    continue;
                                }
                            }
                            if (getCity)
                            {
                                city = row.get_Value(landbaseCityIdx).ToString();
                                description += "City:" + city;
                                GetWhereClause("CITY", city, ref whereClause);
                            }
                            if (getZip)
                            {
                                zip = row.get_Value(landbaseZipIdx).ToString();
                                description += "\rZip:" + zip;
                                GetWhereClause("ZIP", zip, ref whereClause);
                            }
                            sf.WhereClause = whereClause;
                            double percentComplete = Math.Round((((double)processed) / ((double)totalFeatures)) * 100.0, 1);
                            statusLabel.Text = overallStatus + "\r" + percentComplete + "%:" + "\rFeature Class: " + ds.BrowseName + "\rLandbase Feature Class: " + landbaseDS.BrowseName + "\rLandbase Details\r" + description;
                            Application.DoEvents();

                            using (ComReleaser cr2 = new ComReleaser())
                            {
                                ICursor featureCursor = featureClassToQuery.Search(sf, false);
                                cr2.ManageLifetime(featureCursor);
                                cr.ManageLifetime(sf);
                                IRow featureRow = null;                                
                                while ((featureRow = featureCursor.NextRow()) != null)
                                {
                                    cr2.ManageLifetime(featureRow);
                                    try
                                    {
                                        FeatureInfo newInfo = new FeatureInfo();
                                        newInfo.globalID = featureRow.get_Value(globalIdx).ToString();
                                        if (getCounty) { newInfo.County = county.ToString(); }
                                        if (getDistrict) { newInfo.District = district.ToString(); }
                                        if (getDivision) { newInfo.Division = division.ToString(); }
                                        if (getCity) { newInfo.City = city; }
                                        if (getZip) { newInfo.Zip = zip; }
                                        featureInfo.Add(newInfo);
                                    }
                                    catch (Exception ex)
                                    {
                                        errors.Add(ex.Message);
                                    }
                                    finally
                                    {
                                        while (Marshal.ReleaseComObject(featureRow) > 0) { }
                                    }
                                }
                                if (featureRow != null) { while (Marshal.ReleaseComObject(featureRow) > 0) { } }
                                if (featureCursor != null) { while (Marshal.ReleaseComObject(featureCursor) > 0) { } }
                                if (sf != null) { while (Marshal.ReleaseComObject(sf) > 0) { } }
                            }
                        }
                        catch (Exception ex)
                        {
                            errors.Add(ex.Message);
                        }
                        finally
                        {
                            if (row != null) { while (Marshal.ReleaseComObject(row) > 0) { } }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                errors.Add(ex.Message);
            }
            finally
            {
                if (row != null) { while (Marshal.ReleaseComObject(row) > 0) { } }
                if (landbaseCursor != null) { while (Marshal.ReleaseComObject(landbaseCursor) > 0) { } }
                if (qf != null) { while (Marshal.ReleaseComObject(qf) > 0) { } }
            }
        }

        private void GetWhereClause(string fieldName, string fieldValue, ref string whereClause)
        {
            if (string.IsNullOrEmpty(whereClause))
            {
                whereClause = fieldName + " <> '" + fieldValue + "' OR " + fieldName + " IS NULL";
            }
            else
            {
                whereClause += " OR " + fieldName + " <> '" + fieldValue + "' OR " + fieldName + " IS NULL";
            }
        }

        private int GetIndex(ref ITable table, string fieldName)
        {
            IDataset ds = table as IDataset;
            try
            {
                return FeatureClassToFieldIndices[ds.BrowseName][fieldName];
            }
            catch
            {
                if (!FeatureClassToFieldIndices.ContainsKey(ds.BrowseName))
                {
                    FeatureClassToFieldIndices.Add(ds.BrowseName, new Dictionary<string, int>());
                }
                int index = table.Fields.FindField(fieldName);
                if (index < 0) { throw new Exception("Field Name not found"); }
                FeatureClassToFieldIndices[ds.BrowseName].Add(fieldName, index);
                return index;
            }
        }

        //function to form update statement
        private void formUpdQry(List<string> lstUpdVals, ref string strUpdQry, ref string strA_UpdQry, string strGis_Globalid)
        {
            try
            {
                string strUpdVal = string.Empty;
                strUpdVal = " set ";
                if (lstUpdVals.Count == 0)
                {
                    strUpdQry = "";
                }
                else
                {
                    for (int i = 0; i < lstUpdVals.Count; i++)
                    {
                        strUpdVal = strUpdVal + lstUpdVals[i] + "|";
                    }
                    if (strUpdVal.EndsWith("|"))
                    {
                        strUpdVal = strUpdVal.TrimEnd('|');
                    }
                    strUpdQry = strUpdQry + strUpdVal + " where globalid ='" + strGis_Globalid + "';";
                    strA_UpdQry = strA_UpdQry + strUpdVal + " where globalid ='" + strGis_Globalid + "';";
                }
            }
            catch (Exception ex)
            {
                Logs.writeLine("Error on formUpdQry " + ex.Message);

            }
        }

        private void getFcName(ref List<string> lstFcNamesList)
        {
            try
            {
                lstFcNamesList.Clear();
                string strPath = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
                strPath = strPath + "\\PolygonAttributes_fcnames.txt";
                TextReader tx = new StreamReader(strPath);
                string s = tx.ReadLine();
                while (s != null)
                {
                    lstFcNamesList.Add(s.ToUpper());
                    chkFCListBox.Items.Add(s);
                    s = tx.ReadLine();

                }
                tx.Close();
            }
            catch (Exception ex)
            {
                Logs.writeLine("Error at adding getFcName list from the input txt file #getFcName " + ex.Message);
            }
        }

        //get field value from intersect polyboundry
        private void getFldVal_fromboundry(IFeature pfeat, IFeatureClass pfc, int intFldIdx, ref string strLandbase_Val, string strFldName)
        {
            ISpatialFilter pSptlFilter = new SpatialFilterClass();
            IFeatureCursor pFCursor = null;
            IFeature pFeat = null;
            strLandbase_Val = "";
            string strVal = string.Empty;
            try
            {
                using (ComReleaser cr = new ComReleaser())
                {
                    pSptlFilter.Geometry = (IGeometry)pfeat.Shape;
                    pSptlFilter.GeometryField = pfc.ShapeFieldName;
                    pSptlFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                    int intCnt = pfc.FeatureCount(pSptlFilter);
                    pFCursor = pfc.Search(pSptlFilter, false);
                    cr.ManageLifetime(pSptlFilter);
                    cr.ManageLifetime(pFCursor);
                    pFeat = pFCursor.NextFeature();
                    if (pFeat != null)
                    {
                        strVal = pFeat.get_Value(intFldIdx).ToString();

                        if (strFldName.ToUpper() == "CNTY_NAME")
                        {
                            strVal = dist_County_Vlas[strVal.ToUpper()].ToString();
                        }
                        else if (strFldName.ToUpper() == "DISTRICT")
                        {
                            strVal = dist_District_Vlas[strVal.ToUpper()].ToString();

                        }
                        else if (strFldName.ToUpper() == "DIVISION")
                        {
                            strVal = dist_Division_Vlas[strVal.ToUpper()].ToString();
                        }
                        else
                        {
                            //if it is zip
                            strVal = pFeat.get_Value(intFldIdx).ToString();
                        }

                        strLandbase_Val = strVal;// pFeat.get_Value(intFldIdx).ToString();                        
                    }
                    if (pFeat != null) { while (Marshal.ReleaseComObject(pFeat) > 0) { } }
                }
            }
            catch (Exception ex)
            {
                Logs.writeLine("EXCP@getFldVal_fromboundry " + ex.Message);
            }
            finally
            {
                if (pFCursor != null) { while (Marshal.ReleaseComObject(pFCursor) > 0) { } }
                if (pSptlFilter != null) { while (Marshal.ReleaseComObject(pSptlFilter) > 0) { } }
                //if (pFCursor != null)
                //    Marshal.ReleaseComObject(pFCursor);
                //if (pSptlFilter != null)
                //    Marshal.ReleaseComObject(pSptlFilter);

            }
        }

        //private void getPolygonAttributesAndUpdate(IFeature pPolyFeat, string fieldName, string UpdateValue, IFeatureClass Pointfeatureclass, IFeatureClass pPolyfeatclass, int featcount)
        //{
        //    IFeatureCursor ConnectedMapCursor = null;
        //    try
        //    {
        //        IGeometry queryGeometry = null;
        //        ISpatialFilter spatialFilter = null;
        //        int iterationCnt = 0, updatecnt = 0;
        //        ConnectedMapCursor = null;
        //        string stringUpd_DomainVal = string.Empty;

        //        queryGeometry = pPolyFeat.ShapeCopy;
        //        spatialFilter = new SpatialFilterClass();
        //        spatialFilter.Geometry = queryGeometry;
        //        spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;                
        //        spatialFilter.WhereClause = fieldName + " IS NULL";
        //        // spatialFilter.SubFields =  fieldName;
        //        //stbInfo.Text = "Processing... " + UpdateValue + "(" + intcount + " from " + Totalcount + ")";

        //        // Execute the query and iterate through the cursor's results.
        //        using (ComReleaser comReleaser = new ComReleaser())
        //        {
        //            IFeature pPntFeat = null;
        //            String FieldValue = null;
        //            IField Domainname = null;

        //            int intFldIdx = 0;

        //            ConnectedMapCursor = Pointfeatureclass.Search(spatialFilter, false);
        //            int count = Pointfeatureclass.FeatureCount(spatialFilter);
        //            comReleaser.ManageLifetime(ConnectedMapCursor);
        //            pPntFeat = ConnectedMapCursor.NextFeature();

        //            while (pPntFeat != null)
        //            {

        //                iterationCnt++;
        //                updatecnt++;
        //                stbInfo.Text = Pointfeatureclass.AliasName + " " +intUpdCount + " Updated";
        //                Application.DoEvents();

        //                //stbStatus.Text = "Processing... " + Polyfeatureclass.AliasName + " " + intcount + "(" + UpdateValue + ")" + "(" + Pointfeatureclass.AliasName + " " + iterationCnt + " from " + count + ")" + " of " + Totalcount;
        //                //Application.DoEvents();
        //                intFldIdx = 0;
        //                FieldValue = string.Empty;

        //                intFldIdx = pPntFeat.Fields.FindField(fieldName); 
        //                if (UpdateValue != FieldValue)
        //                {
        //                    if ((fieldName.ToUpper() == "Division".ToUpper()) || (fieldName.ToUpper() == "District".ToUpper()) || (fieldName.ToUpper() == "County".ToUpper()))
        //                    {
        //                        // mainvalue = Convert.ToInt16(FieldValue);
        //                        Domainname = Pointfeatureclass.Fields.get_Field(intFldIdx);
        //                        stringUpd_DomainVal = getCodedDomainDescription_new(clsTestWorkSpace.Workspace, Domainname.Domain.Name, UpdateValue);
        //                        intUpdCount++;
        //                        intUpdCntToStop++;
        //                        pPntFeat.set_Value(intFldIdx, stringUpd_DomainVal);
        //                        pPntFeat.Store();
        //                        _clsGlobalFunctions.Common_addRowstoReportTable(objReportTable, pPolyfeatclass.AliasName + "," + Pointfeatureclass.AliasName + "," + pPntFeat.get_Value(pPntFeat.Fields.FindField("GLOBALID")).ToString() + "," + fieldName + "," + FieldValue + "," + stringUpd_DomainVal + "(" + UpdateValue + ")" + "," + " Feature updated");
        //                    }
        //                    else
        //                    {
        //                        intUpdCount++;
        //                        intUpdCntToStop++;
        //                        pPntFeat.set_Value(intFldIdx, UpdateValue);
        //                        pPntFeat.Store();
        //                        _clsGlobalFunctions.Common_addRowstoReportTable(objReportTable, pPolyfeatclass.AliasName + "," + Pointfeatureclass.AliasName + "," + pPntFeat.get_Value(pPntFeat.Fields.FindField("GLOBALID")).ToString() + "," + fieldName + "," + FieldValue + "," + UpdateValue + "," + " Feature updated");
        //                    }
        //                }
        //                pPntFeat = ConnectedMapCursor.NextFeature();
        //                if (intUpdCount == 1000)
        //                {
        //                    clsTestWorkSpace.StopEditOperation(true);
        //                    clsTestWorkSpace.StartEditOperation();
        //                    intUpdCount = 0;
        //                }

        //                if (intUpdCntToStop > 5000)
        //                {
        //                    pPntFeat = null;
        //                }                        
        //            }                   
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        Logs.writeLine("Error @ getPolygonAttributesAndUpdate " + ex + "  " + Pointfeatureclass.AliasName + "," + pPolyfeatclass.AliasName);
        //    }
        //    finally
        //    {

        //        GC.Collect();
        //        GC.WaitForPendingFinalizers();
        //        if (ConnectedMapCursor != null)
        //        {
        //            Marshal.ReleaseComObject(ConnectedMapCursor);
        //            ConnectedMapCursor = null;
        //        }

        //    }
        //}

        //private string getCodedDomainDescription_new(IWorkspace pWorkspace, string strDomainName, string strValue)
        //{
        //    string strVal = string.Empty;

        //    try
        //    {
        //        IWorkspaceDomains2 pWorkspaceDomain = (IWorkspaceDomains2)pWorkspace;

        //        IDomain pDomain = pWorkspaceDomain.get_DomainByName(strDomainName);
        //        ICodedValueDomain pCodeDomain = (ICodedValueDomain)pDomain;               
        //        for (int i = 0; i < pCodeDomain.CodeCount; i++)
        //        {
        //            if (strVal.Length > 0)
        //            {
        //                break;                    
        //            }

        //            string hii = pCodeDomain.get_Name(i).ToString().ToUpper();
        //            if (pCodeDomain.get_Name(i).ToString().ToUpper() == strValue.ToString().ToUpper())
        //                strVal = pCodeDomain.get_Value(i).ToString();                   
        //        }
        //        //MessageBox.Show(strVal);
        //        return strVal;
        //    }
        //    catch (Exception ex)
        //    {
        //        Logs.writeLine("Error @ getCodedDomainDescription_new " + ex.Message);
        //        return strVal;

        //    }
        //}

        //private string[] GetPolyGonFeatures(string FieldName, IFeatureClass pPolyfeatclass)
        //{            
        //    IFeatureCursor objFeatCur = null;
        //    IFeature objfeat = null;
        //    int count = 0, intCntr = 0;
        //    string[] Names = null;
        //    IList<string> tablename = new List<string>();

        //    try
        //    {
        //        //pPolyfeatclass = clsGlobalFunctions._globalFunctions.getFeatureclassByName(pPolygonWorkspace, strPolyFeatName);
        //        objFeatCur = pPolyfeatclass.Search(null, false);
        //        objfeat = objFeatCur.NextFeature();
        //        count = pPolyfeatclass.FeatureCount(null);
        //        intCntr = 0;
        //        int index = 0;

        //        if (FieldName == "County")
        //        {
        //            index = pPolyfeatclass.Fields.FindField("CNTY_NAME");
        //        }
        //        else if (FieldName == "Zip")
        //        {
        //            index = pPolyfeatclass.Fields.FindField("POSTCODE");
        //        }
        //        else
        //        {
        //            index = pPolyfeatclass.Fields.FindField(FieldName);
        //        }
        //        if (index != -1)
        //        {
        //            while (objfeat != null)
        //            {
        //                intCntr++;
        //                String updateValue = objfeat.get_Value(index).ToString();
        //                String ObjectId = objfeat.OID.ToString();
        //                tablename.Add(updateValue + "," + ObjectId);
        //                //one by one features form contors layer here....                
        //                objfeat = objFeatCur.NextFeature();
        //            }

        //            Names = tablename.ToArray<string>();
        //        }
        //        if (objFeatCur != null)
        //        {
        //            Marshal.ReleaseComObject(objFeatCur);
        //            objFeatCur = null;
        //        }

        //        return Names;
        //    }
        //    catch (Exception ex)
        //    {
        //        Logs.writeLine("Error @ GetPolyGonFeatures " + ex);
        //    }
        //    return Names;
        //}
        //private void addFeatClassNam_toLst(ref List<IFeatureClass> lstPtFeatclassNames, string FieldName)
        //{
        //    try
        //    {
        //        IEnumDataset pEnumDSname = clsTestWorkSpace.Workspace.get_Datasets(esriDatasetType.esriDTFeatureDataset);
        //        IDataset objFDS = pEnumDSname.Next();
        //        while (objFDS != null)
        //        {
        //            if (objFDS.Name == "EDGIS.ElectricDataset")
        //            {
        //                IFeatureDataset pFDataset = (IFeatureDataset)objFDS;
        //                IEnumDataset penumDS = pFDataset.Subsets;
        //                IDataset dataset = penumDS.Next();
        //                while (dataset != null)
        //                {
        //                    if (dataset.Type == esriDatasetType.esriDTFeatureClass)
        //                    {
        //                        // IFeatureClass pfeatureclass = (IFeatureClass)dataset;
        //                        IFeatureClass pfeatureclass = clsGlobalFunctions._globalFunctions.getFeatureclassByName(clsTestWorkSpace.Workspace, dataset.Name);
        //                        if (pfeatureclass.ShapeType == esriGeometryType.esriGeometryPoint || pfeatureclass.ShapeType == esriGeometryType.esriGeometryPolyline)
        //                        {
        //                            if (pfeatureclass.Fields.FindField(FieldName) != -1)
        //                            {
        //                                Logs.writeLine(dataset.Name);
        //                                lstPtFeatclassNames.Add(pfeatureclass);
        //                            }
        //                        }
        //                    }
        //                    dataset = penumDS.Next();
        //                }

        //            }
        //            objFDS = pEnumDSname.Next();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Logs.writeLine("Error @ addFeatClassNam_toLst " + ex);
        //    }
        //}
        private mmAutoUpdaterMode DisableAutoupdaters()
        {
            object objAutoUpdater = null;

            //Create an MMAutoupdater 
            objAutoUpdater = Activator.CreateInstance(Type.GetTypeFromProgID("mmGeodatabase.MMAutoUpdater"));
            autoupdater = objAutoUpdater as IMMAutoUpdater;
            //Save the existing mode
            mmAutoUpdaterMode oldMode = autoupdater.AutoUpdaterMode;
            //Turn off autoupdater events
            autoupdater.AutoUpdaterMode = mmAutoUpdaterMode.mmAUMNoEvents;
            // insert code that needs to execute while autoupdaters 
            return oldMode;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                //IWorkspace pWS = null;
                string strsdepath = "";
                GetSDEconnectionfromcatalog(ref strsdepath);
                if (strsdepath == "" || ((strsdepath.ToUpper().EndsWith(".SDE".ToUpper()) == false) && strsdepath.ToUpper().EndsWith(".GDB".ToUpper()) == false))
                {
                    MessageBox.Show("Select SDE Connection properly", "EnableAnno", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                txtFGDpath.Text = strsdepath;

                //txtSrcDBPath.Text = strsdepath;
                //IWorkspaceFactory pWSFactory = new SdeWorkspaceFactoryClass();
                //pWS = pWSFactory.OpenFromFile(strsdepath, 0);

                //if (pWS == null)
                //{
                //    MessageBox.Show("Error in getting WS", "EnableAnno", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //    return;
                //}
            }


            catch (Exception ex)
            {
                Logs.writeLine("EXCP@button1_Click " + ex.Message);

            }

            //try
            //{
            //    FolderBrowserDialog fldrDia = new FolderBrowserDialog();
            //    fldrDia.Description = "Select FGD database for Controller generation..";
            //    if (fldrDia.ShowDialog() == DialogResult.OK)
            //    {
            //        txtFGDpath.Text = fldrDia.SelectedPath;
            //    }
            //    if (txtFGDpath.Text.Length == 0)
            //    {
            //        MessageBox.Show("Select FileGeoDatabase", "Message", MessageBoxButtons.OK);
            //    }
            //}
            //catch (System.Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
        }

        private void GetSDEconnectionfromcatalog(ref string strSelectPath)
        {
            try
            {
                IGxDialog pGxDlgSel;
                IGxFileFilter pGxFltrOpt;
                IEnumGxObject pEnmGxObjs;
                IGxObject pGxObj;
                IName pName;

                pGxFltrOpt = new GxFileFilterClass();
                pGxDlgSel = new GxDialogClass();
                //pGxDlgSel.ObjectFilter = pGxFltrOpt;
                pGxDlgSel.AllowMultiSelect = false;
                pGxDlgSel.ButtonCaption = "OK";
                pGxDlgSel.Title = "Select SDE Connection";

                pGxDlgSel.DoModalOpen(0, out pEnmGxObjs);
                pGxObj = pEnmGxObjs.Next();

                if (pGxObj != null)
                {
                    pName = pGxObj.InternalObjectName;
                    strSelectPath = pGxObj.FullName;
                }
                else
                {
                    MessageBox.Show("Select SDE Connection properly", "MessageBox", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message, "MessageBox", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }

        private void btnCheckAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < chkFCListBox.Items.Count; i++)
            {
                chkFCListBox.SetItemChecked(i, true);
            }
        }
        private void generateSQLfile(DataTable objReportTable, string strFCName, string strTime)
        {
            string strFilePath = string.Empty;
            string strUpdBasetab = string.Empty;
            string strUpdAtab = string.Empty;
            string strSpoolStatement = string.Empty;
            int intIterateCnt = 0;
            //TAB_QRY,A_QRY
            try
            {
                strSpoolStatement = "SPOOL C:\\UDC\\Logs\\" + strFCName + ".txt";
                string strLogFilePath = System.Configuration.ConfigurationManager.AppSettings["LogFilePath"].ToString();

                // check whether selected path exists or not
                if (Directory.Exists(strLogFilePath) == false)
                {
                    Directory.CreateDirectory(strLogFilePath);
                }
                //get current time to make filename                

                //if the file already exists then append to the existing file
                strFilePath = strLogFilePath + strTime + "_" + strFCName + ".sql";
                StreamWriter objTxtWriter = new StreamWriter(strFilePath);

                objTxtWriter.WriteLine(strSpoolStatement);
                objTxtWriter.WriteLine("SET DEFINE OFF");

                foreach (DataRow row in objReportTable.Rows)
                {
                    intIterateCnt++;
                    strUpdBasetab = row["TAB_QRY"].ToString();
                    strUpdBasetab = strUpdBasetab.Replace('|', ',');
                    strUpdAtab = row["A_QRY"].ToString();
                    strUpdAtab = strUpdAtab.Replace('|', ',');

                    objTxtWriter.WriteLine(strUpdBasetab);
                    objTxtWriter.WriteLine(strUpdAtab);
                    if (intIterateCnt == 10000)
                    {
                        objTxtWriter.WriteLine("COMMIT;");
                        intIterateCnt = 0;
                    }
                }
                objTxtWriter.WriteLine("COMMIT;");
                objTxtWriter.WriteLine("SPOOL OFF");
                objTxtWriter.Close();

            }
            catch (Exception exce)
            {
                throw new Exception("Error occured in createLogfile" + exce.Message);
            }

        }
    }
}
