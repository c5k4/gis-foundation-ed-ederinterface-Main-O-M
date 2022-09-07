/*
 * Rule1: if GDB service point XY location is not matched with GIS location (only one point in that location), then relocate that location
 * Rule2: if GDB service point XY location is not matched with GIS location (more points in that location), create new location and relate GDB point to new location and create pseudo service
 * Rule3: if more servicepoint in GDB related to one location in GIS, relocate that location if only these service point relations are found in GIS
 * Rule4: if more servicepoint in GDB related to one location in GIS, create new location and pseudo if more service point relations are found in GIS and xy locations are not matched
 * 
 * Pseudo service can be created from Transformer to Servicelocation
 * 
 * Pre-requisites:
 * If a location not connected to pseudo service, it cannot be relocated
 * If a location cannot be found for a GDB service point, report error but not create new location
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using System.Collections;
using System.Configuration;
using System.Runtime.InteropServices;
using Miner.Interop;
using ESRI.ArcGIS.CatalogUI;
using ESRI.ArcGIS.Catalog;
using System.Data.OleDb;
using System.IO;

namespace PGEReGeoCodingProcess
{
    public partial class FormReGeoCoding : Form
    {
        public FormReGeoCoding()
        {
            InitializeComponent();

            cmbAllFC.Enabled = false;
        }
        private static IMMAutoUpdater autoupdater = null;
        //geo process globals
        //global area
        public static System.Data.DataTable ObjReportTable;
        public static string strReporttableFormat = string.Empty;

        public static ITable pLocInfoTbl = null;
        public static ITable pNewLocTbl = null;

        public static IWorkspace pCedsaWsp = null;

        //ini file functionalities 
        public static InfoINI.InfoINI INIdefinitions = new InfoINI.InfoINI();

        public static IFeatureClass pSrvLocCls = null;
        public static IFeatureClass pXFRCls = null;
        public static IFeatureClass pOHsrvCls = null;
        public static IFeatureClass pUGsrvCls = null;
        public static ITable pSrvPtTbl = null;
        public static IFeatureClass pLOPC_FC = null;

        public static IRelationshipClass pSrvLoc_Pnt_relCls = null;
        public static IRelationshipClass pXFR_SrvPnt_relCls = null;

        public IFeatureWorkspace pCedsaFws = null;
        public IWorkspace2 pDesaWS2 = null;

        public IFeatureClass PGridfC = null;

        INetworkFeature _pNetworkFeature = null;
        IGeometricNetwork _pGeonetWork = null;

        // Checks for file GDB and ini files in User selected path..
        private void btnGDB_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fldrDia = new FolderBrowserDialog();
            fldrDia.Description = "Select Path for Log Files| Source GDB|PGEReGeoCoding.ini..!";
            if (fldrDia.ShowDialog() == DialogResult.OK)
            {
                clsCommonVariables.strLogsPath = fldrDia.SelectedPath;
                if (System.IO.File.Exists(clsCommonVariables.strLogsPath + "\\PGEReGeoCoding.ini"))
                {
                    //ini
                    clsCommonVariables.strINIpath = clsCommonVariables.strLogsPath + "\\PGEReGeoCoding.ini";
                    btnExecute.Enabled = true;
                    cmbAllFC.Enabled = true;
                }
                else
                {
                    MessageBox.Show("PGEReGeoCoding.ini file Not Found in Selected Path..!");
                    return;
                }
            }
            else
                MessageBox.Show("Select Logs path.. try again..!", "NO Path");
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            if (rad1_1.Checked != true && rad1_MMode.Checked != true)
            {
                MessageBox.Show("Choose 1-1 | 1-M option to execute..!");
                return;
            }
            try
            {

                pCedsaFws = null;
                pDesaWS2 = null;

                //get ini data
                string strGdbFile = INIdefinitions.GetEntryValue("GDBFile", "File", clsCommonVariables.strINIpath);
                string strSrvLocInfoTbl = INIdefinitions.GetEntryValue("ReGeocodingData", "ServiceLocationInfo", clsCommonVariables.strINIpath);
                string strNewLocDataTbl = INIdefinitions.GetEntryValue("ReGeocodingData", "NewLocationData", clsCommonVariables.strINIpath);
                string strLandbase = INIdefinitions.GetEntryValue("Landbase", "File", clsCommonVariables.strINIpath);

                //GeocodeGDB data
                pCedsaWsp = FileGdbWorkspaceFromPath(clsCommonVariables.strLogsPath + "\\" + strGdbFile);

                if (pCedsaWsp == null)
                {
                    MessageBox.Show("Unable to get Workspace from " + clsCommonVariables.strLogsPath + "\\" + strGdbFile);
                    ClsLogReports.writeLine("Unable to get Workspace from " + clsCommonVariables.strLogsPath + "\\" + strGdbFile);
                    return;
                }
                pDesaWS2 = (IWorkspace2)pCedsaWsp;
                pCedsaFws = (IFeatureWorkspace)pCedsaWsp;

                if (pDesaWS2.get_NameExists(esriDatasetType.esriDTTable, strSrvLocInfoTbl))
                {
                    pLocInfoTbl = pCedsaFws.OpenTable(strSrvLocInfoTbl);
                }
                else
                {
                    ClsLogReports.writeLine("Table :" + strSrvLocInfoTbl + " NOT Found in " + clsCommonVariables.strLogsPath + "\\" + strGdbFile);
                    return;
                }
                if (pDesaWS2.get_NameExists(esriDatasetType.esriDTTable, strNewLocDataTbl))
                {
                    pNewLocTbl = pCedsaFws.OpenTable(strNewLocDataTbl);
                }
                else
                {
                    ClsLogReports.writeLine("Table :" + strNewLocDataTbl + " NOT Found in " + clsCommonVariables.strLogsPath + "\\" + strGdbFile);
                    return;
                }

                if (rad1_1.Checked == true)
                {
                    if (cmbAllFC.SelectedItem.ToString() == "")
                    {
                        ClsLogReports.writeLine("Please select value in Combo box and proceed");
                        return;
                    }
                }

                PGridfC = pCedsaFws.OpenFeatureClass(strLandbase);

                if (PGridfC == null)
                {
                    ClsLogReports.writeLine("Featureclass :" + strLandbase + " NOT Found in " + clsCommonVariables.strLogsPath + "\\" + strGdbFile);
                    return;

                }
                pSrvLocCls = clsCommonVariables.pFeatWrkSpace.OpenFeatureClass("EDGIS.ServiceLocation");
                pXFRCls = clsCommonVariables.pFeatWrkSpace.OpenFeatureClass("EDGIS.Transformer");
                pOHsrvCls = clsCommonVariables.pFeatWrkSpace.OpenFeatureClass("EDGIS.SecOHConductor");
                pUGsrvCls = clsCommonVariables.pFeatWrkSpace.OpenFeatureClass("EDGIS.SecUGConductor");
                pSrvPtTbl = clsCommonVariables.pFeatWrkSpace.OpenTable("EDGIS.SERVICEPOINT");
                pLOPC_FC = clsCommonVariables.pFeatWrkSpace.OpenFeatureClass("EDGIS.PGE_LOPC");

                pSrvLoc_Pnt_relCls = clsCommonVariables.pFeatWrkSpace.OpenRelationshipClass("EDGIS.ServiceLocation_ServicePoint");
                pXFR_SrvPnt_relCls = clsCommonVariables.pFeatWrkSpace.OpenRelationshipClass("EDGIS.Transformer_ServicePoint");

                lblMsg.Text = "Setting Auto Updaters to feeder manager mode..Please wait..!";
                Application.DoEvents();
                ClsMM.SetFeederManagerModeAutoupdaters(ref autoupdater);

                //ClsMM.stopAutoupdaters(autoupdater);


                //log reports
                ClsLogReports.Common_initSummaryTable("ReGeoCodeProcess", "PGE ReGeoCode");
                strReporttableFormat = "ServiceLoc_GlobalID,ServiceLoc_OID,ConversionID,old_X,old_Y,New_X,New_Y,Comments";

                ClsLogReports.writeLine("");
                IPropertySet pCOnn = clsCommonVariables.pWrkSpace.ConnectionProperties;
                //ClsLogReports.writeLine("SDE Database :: " + pCOnn.GetProperty("Database").ToString());
                // ClsLogReports.writeLine("SDE Server :: " + pCOnn.GetProperty("Server").ToString());
                ClsLogReports.writeLine("SDE Version :: " + pCOnn.GetProperty("Version").ToString());
                ClsLogReports.writeLine("");

                ObjReportTable = new System.Data.DataTable();
                ClsLogReports.Common_addColumnToReportTable(ObjReportTable, strReporttableFormat);
                string strWhereClause = string.Empty;
                if (rad1_1.Checked == true)
                {
                    ClsLogReports.writeLine("Start Date and Time  :" + System.DateTime.Now);
                    strWhereClause = cmbAllFC.SelectedItem.ToString().Replace(">", "_");
                    strWhereClause = strWhereClause.Replace("<", "_");
                    Execute1_1GeoCode("SERVICEPT_COUNT = 1");
                    ClsLogReports.writeLine("END Date and Time  :" + System.DateTime.Now);
                }
                else if (rad1_MMode.Checked == true)
                {
                    ClsLogReports.writeLine("Start Date and Time  :" + System.DateTime.Now);
                    strWhereClause = "1_M";
                    Execute1_MGeoCode("SERVICEPT_COUNT > 1 order by SERVICELOCATIONCONVID");
                    // Execute1_MGeoCode("SERVICEPT_COUNT > 1 and SERVICELOCATION_OBJECTID in(1190692)");
                    ClsLogReports.writeLine("END Date and Time  :" + System.DateTime.Now);
                }

                ClsLogReports.GenerateTheReport(ObjReportTable, "PGE_ReGeoCoding" + strWhereClause);

                ClsLogReports.close();
                ObjReportTable.Clear();
                ObjReportTable.Dispose();

                lblMsg.Text = "Enabling Auto Updaters..Please wait..!";
                Application.DoEvents();
                ClsMM.startAutoupdaters(autoupdater);
                lblMsg.Text = "Process Completed..!";
                MessageBox.Show("Process Completed., please see the Report File.", "Report", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex) { throw ex; }
        }

        //1:M
        private void Execute1_MGeoCode(string strGDBQry)
        {
            IQueryFilter pQry = null;
            ICursor pCurs = null;
            string strTransFeederInfo = string.Empty;
            string strTransCirCuitID = string.Empty;
            try
            {
                pQry = new QueryFilter();

                //pQry.WhereClause = "trans_cedsaid =1820028036 and SERVICEPT_COUNT > 1 ";
                pQry.WhereClause = strGDBQry;
                pQry.SubFields = "SERVICELOCATIONCONVID,SERVICELOCATION_OBJECTID,SERVICELOCATION_GLOBALID,TRANS_CEDSAID,OBJECTID_1";
                int intCnt = pLocInfoTbl.RowCount(pQry);
                pCurs = pLocInfoTbl.Search(pQry, true);
                IRow pLocInfoRow = (IRow)pCurs.NextRow();


                //declare
                IFeature pServiceLocFeat = null, pTransFeat = null;
                IPoint pSrvLocPnt_new = null;
                string strLnkTyp = string.Empty, strLnkRat = string.Empty, strConvId = string.Empty, strObjId = string.Empty, strGlobalID = string.Empty;
                string strStatusComment = string.Empty, strXFRcedsaid = string.Empty, strXFRcgc12 = string.Empty, strTransID = string.Empty;

                IFeature pGridFeat = null;

                int intCntr = 0;
                object intOid_Exists = 0;
                int updCnt = 0;
                Boolean blnRelocated = false;

                clsCommonVariables.StartEditOperation();
                int saveCntr = 1;
                int rowsProcessed = 0;
                while (pLocInfoRow != null)
                {
                    rowsProcessed++;
                    intCntr++;
                    //init
                    strLnkTyp = string.Empty;
                    strLnkRat = string.Empty;
                    strConvId = string.Empty;
                    strObjId = string.Empty;
                    strGlobalID = string.Empty;
                    strTransID = string.Empty;
                    strXFRcedsaid = string.Empty;
                    strXFRcgc12 = string.Empty;
                    strStatusComment = string.Empty;
                    strReporttableFormat = string.Empty;
                    pSrvLocPnt_new = new PointClass();
                    blnRelocated = false;
                    intOid_Exists = 0;
                    pGridFeat = null;
                    try
                    {
                        lblMsg.Text = "Processing GDB ServicePoints " + intCntr + " of " + intCnt + "  Relocated: " + updCnt;
                        //formObj.stbStatusStrip.Refresh();                        
                        this.Refresh();

                        if (updCnt != 0 && saveCntr == 1)
                            saveCntr = updCnt;
                        if (saveCntr != 1 && updCnt % 500 == 0)
                        {
                            saveCntr = 1;
                            clsCommonVariables.StopEditOperation(true);
                            clsCommonVariables.StartEditOperation();
                        }

                        if (rowsProcessed > 500)
                        {
                            GC.Collect();
                            rowsProcessed = 0;
                        }

                        Application.DoEvents();

                        //get object id instead of COnversionid
                        strConvId = pLocInfoRow.get_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pLocInfoRow.Table, "SERVICELOCATIONCONVID")).ToString();
                        strObjId = pLocInfoRow.get_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pLocInfoRow.Table, "SERVICELOCATION_OBJECTID")).ToString();
                        strGlobalID = pLocInfoRow.get_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pLocInfoRow.Table, "SERVICELOCATION_GLOBALID")).ToString();
                        strTransID = pLocInfoRow.get_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pLocInfoRow.Table, "TRANS_CEDSAID")).ToString();

                        //Function to get Servation location with globalID
                        pServiceLocFeat = GetFeature_globalid(strGlobalID);

                        //Function to get Transformer with TransformerID
                        pTransFeat = GetFeature_TranformerID(strTransID);

                        if (pServiceLocFeat == null)
                        {
                            ClsLogReports.Common_addRowstoReportTable(ObjReportTable, strGlobalID + "," + strObjId + "," + strConvId + ",,,,,ServiceLocation Not Found");
                            pLocInfoRow = (IRow)pCurs.NextRow();
                            continue;
                        }

                        if (pTransFeat == null)
                        {
                            ClsLogReports.Common_addRowstoReportTable(ObjReportTable, strGlobalID + "," + strObjId + "," + strConvId + ",,,,,Transformer Not Found");
                            pLocInfoRow = (IRow)pCurs.NextRow();
                            continue;
                        }

                        if (_pNetworkFeature == null || _pGeonetWork == null)
                        {
                            _pNetworkFeature = (INetworkFeature)pTransFeat;
                            _pGeonetWork = _pNetworkFeature.GeometricNetwork;
                        }




                        clsArcGISfunctionality.GetFieldValue(pTransFeat, "feederinfo", ref strTransFeederInfo);
                        clsArcGISfunctionality.GetFieldValue(pTransFeat, "circuitid", ref strTransCirCuitID);

                        //get new location by validating no existing of servicelocation there, returns true if location not exists then relocate the Srvlocation feature
                        Boolean blnFeatFound = GetNewLocation_MN(pNewLocTbl, "SERVICELOCATION_GLOBALID", strGlobalID, ref pSrvLocPnt_new, ref intOid_Exists);

                        //Function to get Map Grid with Geocoding point
                        pGridFeat = getGridnamefromFeature(pSrvLocPnt_new);

                        if (pGridFeat == null)
                        {
                            ClsLogReports.Common_addRowstoReportTable(ObjReportTable, strGlobalID + "," + strObjId + "," + strConvId + ",,,,,Grid feat not found");
                            pLocInfoRow = (IRow)pCurs.NextRow();
                            continue;
                        }

                        double dblRelOcDist = clsArcGISfunctionality.GetDistance(pSrvLocPnt_new, (IPoint)pServiceLocFeat.Shape);
                        IPoint oldLoc = (IPoint)((IGeometry)pServiceLocFeat.Shape);
                        string strDist = dblRelOcDist.ToString();
                        strDist = strDist.Substring(0, strDist.Length - 4);
                        if (dblRelOcDist > 1 && dblRelOcDist < 9999)
                        {

                            //returns false if service point count mismatch in GDB and GIS MN
                            if (true != GetUpdateAPNnum_MN(pServiceLocFeat, pNewLocTbl, "SERVICELOCATION_GLOBALID", strGlobalID, "NEW_APNNUM", ref strStatusComment))
                            {
                                //store x,y and new loc oid
                                IList<string> lstLocationsNew = new List<string>();
                                IQueryFilter pQryLoc = new QueryFilter();


                                pQryLoc.WhereClause = "SERVICELOCATION_GLOBALID='" + strGlobalID + "'";
                                pQryLoc.SubFields = "NEW_XCORD,NEW_YCORD,SERVICEPOINTCONVID,NEW_APNNUM,TRANS_CGC12,TRANS_CEDSAID,OBJECTID_1";
                                ICursor pCursLoc = pNewLocTbl.Search(pQryLoc, true);

                                // IList<string> lstLocationsNew = new List<string>();
                                // IQueryFilter pQryLoc = new QueryFilter();
                                // pQryLoc.WhereClause = "SERVICELOCATION_GLOBALID='" + strGlobalID + "'";
                                //ICursor pCursLoc = pNewLocTbl.Search(pQryLoc, false);
                                IRow pGDBSrvPnt = (IRow)pCursLoc.NextRow();

                                //  IRow pGDBSrvPnt = (IRow)pCursLoc.NextRow();o

                                string xCoord = string.Empty;
                                string yCoord = string.Empty;

                                string servicePntConvID = string.Empty;
                                string servicePntAPNnum = string.Empty;
                                int intOid = 0;
                                while (pGDBSrvPnt != null)
                                {
                                    pSrvLocPnt_new = null;
                                    pSrvLocPnt_new = new PointClass();
                                    xCoord = string.Empty;
                                    yCoord = string.Empty;
                                    servicePntConvID = string.Empty;
                                    servicePntAPNnum = string.Empty;
                                    int lstCntr_Loc = 0;
                                    IFeature pNewLocFeature = null;
                                    ISet pRelSet = null;
                                    IRow pRelRow = null;
                                    string strval = string.Empty;
                                    IPoint Servicept = null;
                                    int serviceFound = 0;
                                    object intNewOid = 0;

                                    try
                                    {

                                        clsArcGISfunctionality.GetFieldValue(pGDBSrvPnt, "NEW_XCORD", ref xCoord);
                                        clsArcGISfunctionality.GetFieldValue(pGDBSrvPnt, "NEW_YCORD", ref yCoord);

                                        Boolean blnCreated = false;
                                        if (lstLocationsNew.Count == 0)
                                        {
                                            blnCreated = false;
                                        }
                                        else
                                        {
                                            //validate if new location already created                                        
                                            for (int loc = 0; loc < lstLocationsNew.Count; loc++)
                                            {
                                                if (lstLocationsNew[loc].Split('#')[0].Contains(xCoord + "," + yCoord))
                                                {
                                                    lstCntr_Loc = loc;
                                                    blnCreated = true;
                                                    break;
                                                }
                                            }
                                        }

                                        if (blnCreated == false)
                                        {
                                            pSrvLocPnt_new.PutCoords(Convert.ToDouble(xCoord), Convert.ToDouble(yCoord));
                                            if (true == clsArcGISfunctionality.IsPointFeatureExisting(pSrvLocPnt_new, pSrvLocCls, ref intOid))
                                            {

                                                //just relate srv point 
                                                pNewLocFeature = pSrvLocCls.GetFeature(intOid);
                                                //relate GIS SRV points
                                                clsArcGISfunctionality.GetFieldValue(pGDBSrvPnt, "SERVICEPOINTCONVID", ref servicePntConvID);

                                                string strnewGlbID = pNewLocFeature.get_Value(pNewLocFeature.Fields.FindField("GLOBALID")).ToString();
                                                string strnewConID = pNewLocFeature.get_Value(pNewLocFeature.Fields.FindField("CONVERSIONID")).ToString();

                                                //1-M mode
                                                //remove existing relation create new location and build this relation
                                                //get related SrvPoint record from GIS
                                                pRelSet = pSrvLoc_Pnt_relCls.GetObjectsRelatedToObject((IObject)pServiceLocFeat);
                                                pRelRow = (IRow)pRelSet.Next();
                                                strval = string.Empty;
                                                while (pRelRow != null)
                                                {
                                                    clsArcGISfunctionality.GetFieldValue(pRelRow, "CONVERSIONID", ref strval);
                                                    if (strval == servicePntConvID)
                                                    {
                                                        clsArcGISfunctionality.GetFieldValue(pGDBSrvPnt, "NEW_APNNUM", ref servicePntAPNnum);
                                                        clsArcGISfunctionality.GetFieldValue(pGDBSrvPnt, "TRANS_CGC12", ref strXFRcgc12);

                                                        //remove existing relations                                                   
                                                        pRelRow.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pRelRow.Table, pSrvLoc_Pnt_relCls.OriginForeignKey), pNewLocFeature.get_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pNewLocFeature.Table, "GLOBALID")));
                                                        pRelRow.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pRelRow.Table, "APNNUM"), servicePntAPNnum);
                                                        pRelRow.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pRelRow.Table, "CGC12"), strXFRcgc12);


                                                        pRelRow.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pRelRow.Table, pXFR_SrvPnt_relCls.OriginForeignKey), pTransFeat.get_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pTransFeat.Table, "GLOBALID")));


                                                        pRelRow.Store();


                                                        ClsLogReports.Common_addRowstoReportTable(ObjReportTable, strnewGlbID + "," + pNewLocFeature.OID + "," + strnewConID + "," + "" + "," + "" + "," + "" + "," + "" + "," + " New RelationUpdated_GDBpoint OID:" + pGDBSrvPnt.OID);
                                                        updCnt++;
                                                        break;
                                                    }
                                                    pRelRow = (IRow)pRelSet.Next();
                                                }

                                                pSrvLocPnt_new = null;
                                                pGDBSrvPnt = (IRow)pCursLoc.NextRow();
                                                continue;
                                            }
                                            else
                                            {
                                                Servicept = null;
                                                serviceFound = 0;
                                                intNewOid = 0;
                                                pGridFeat = null;

                                                strXFRcedsaid = string.Empty;
                                                strXFRcgc12 = string.Empty;

                                                pNewLocFeature = pSrvLocCls.CreateFeature();

                                                pSrvLocPnt_new.PutCoords(Convert.ToDouble(xCoord), Convert.ToDouble(yCoord));

                                                clsArcGISfunctionality.GetFieldValue(pGDBSrvPnt, "TRANS_CEDSAID", ref strXFRcedsaid);
                                                clsArcGISfunctionality.GetFieldValue(pGDBSrvPnt, "TRANS_CGC12", ref strXFRcgc12);

                                                pGridFeat = getGridnamefromFeature(pSrvLocPnt_new);
                                                if (pGridFeat != null)
                                                {

                                                    //true ==  service found in grid
                                                    if (true == GetintersetSecUGConductors(pGridFeat, ref Servicept, ref serviceFound))
                                                    {


                                                        double dblDist = clsArcGISfunctionality.GetDistance(Servicept, (IPoint)pServiceLocFeat.Shape);
                                                        //true ==  service location found in grid
                                                        if (false == clsArcGISfunctionality.IsPointFeatureExisting(Servicept, pSrvLocCls, ref intNewOid))
                                                        {

                                                            if (dblDist < 2000)
                                                            {
                                                                pNewLocFeature.Shape = (IGeometry)Servicept;
                                                            }
                                                            else
                                                            {
                                                                ClsLogReports.Common_addRowstoReportTable(ObjReportTable, strGlobalID + "," + pServiceLocFeat.OID + "," + strConvId + "," + oldLoc.X + "," + oldLoc.Y + "," + pSrvLocPnt_new.X + "," + pSrvLocPnt_new.Y + "," + "NOT Relocated @Dist: " + dblDist);
                                                                pSrvLocPnt_new = null;
                                                                pGDBSrvPnt = (IRow)pCursLoc.NextRow();
                                                                continue;
                                                            }
                                                            // pNewLocFeature.Shape = (IGeometry)Servicept;
                                                        }
                                                        else
                                                        {

                                                            if (true == UpdRelationwithExistingSerLoc(pTransFeat, pServiceLocFeat, pGDBSrvPnt, intNewOid, ref updCnt, strGlobalID, strConvId, oldLoc, ref blnRelocated, ref strReporttableFormat))
                                                            {

                                                                if (!string.IsNullOrEmpty(strReporttableFormat))
                                                                    ClsLogReports.Common_addRowstoReportTable(ObjReportTable, strReporttableFormat);

                                                                pSrvLocPnt_new = null;
                                                                pGDBSrvPnt = (IRow)pCursLoc.NextRow();
                                                                continue;
                                                            }
                                                            else
                                                            {
                                                                serviceFound = 0;
                                                                pNewLocFeature.Shape = (IGeometry)pSrvLocPnt_new;
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        pNewLocFeature.Shape = (IGeometry)pSrvLocPnt_new;
                                                    }
                                                }
                                                else
                                                {
                                                    pNewLocFeature.Shape = (IGeometry)pSrvLocPnt_new;
                                                }
                                                string str_locOffId_val = getLocalOfficeId(pSrvLocPnt_new);
                                                string str_SerLoc_Phase_fromTrans = getPhase_fromTransformer(pTransFeat);
                                                pNewLocFeature.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pNewLocFeature.Table, "CREATIONUSER"), "INFOTECH");
                                                pNewLocFeature.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pNewLocFeature.Table, "DATECREATED"), String.Format("{0:M/d/yyyy}", DateTime.Now));
                                                pNewLocFeature.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pNewLocFeature.Table, "CONVTRANSFORMERID"), strXFRcedsaid);
                                                pNewLocFeature.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pNewLocFeature.Table, "CGC12"), strXFRcgc12);
                                                pNewLocFeature.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pNewLocFeature.Table, "STATUS"), 5);
                                                pNewLocFeature.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pNewLocFeature.Table, "SUBTYPECD"), 1);
                                                pNewLocFeature.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pNewLocFeature.Table, "PHASEDESIGNATION"), str_SerLoc_Phase_fromTrans);
                                                //pNewLocFeature.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pNewLocFeature.Table, "PHASEDESIGNATION"), 4);                                            
                                                pNewLocFeature.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pNewLocFeature.Table, "CUSTOMEROWNED"), "N");
                                                pNewLocFeature.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pNewLocFeature.Table, "PHASINGVERIFIEDSTATUS"), "Estimated/Defaulted");
                                                pNewLocFeature.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pNewLocFeature.Table, "STATE"), "CA");
                                                pNewLocFeature.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pNewLocFeature.Table, "ENABLED"), "1");
                                                //pNewLocFeature.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pNewLocFeature.Table, "ELECTRICTRACEWEIGHT"), "1610612784");                                            
                                                //pNewLocFeature.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pNewLocFeature.Table, "feederinfo"), "1");
                                                pNewLocFeature.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pNewLocFeature.Table, "circuitid"), strTransCirCuitID);
                                                pNewLocFeature.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pNewLocFeature.Table, "Installjobnumber"), "0");
                                                //ELECTRICTRACEWEIGHT
                                                pNewLocFeature.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pNewLocFeature.Table, "LOCALOFFICEID"), str_locOffId_val);



                                                pNewLocFeature.Store();

                                                lstLocationsNew.Add(xCoord + "," + yCoord + "#" + pNewLocFeature.OID);

                                                if (serviceFound == 0)
                                                {
                                                    createPseudoService(pNewLocFeature, pTransFeat);
                                                }

                                                if (true == FunctiontofindSecCond(pSrvLocPnt_new))
                                                {
                                                    ClsLogReports.Common_addRowstoReportTable(ObjReportTable, strGlobalID + "," + pServiceLocFeat.OID + "," + strConvId + "," + oldLoc.X + "," + oldLoc.Y + "," + pSrvLocPnt_new.X + "," + pSrvLocPnt_new.Y + ", New location created OID:" + pNewLocFeature.OID + "   Extra Pseudo or Service Found at same Grid!! need to check");
                                                }
                                                else
                                                {
                                                    ClsLogReports.Common_addRowstoReportTable(ObjReportTable, strGlobalID + "," + pServiceLocFeat.OID + "," + strConvId + "," + oldLoc.X + "," + oldLoc.Y + "," + pSrvLocPnt_new.X + "," + pSrvLocPnt_new.Y + "," + strStatusComment + " New location created OID:" + pNewLocFeature.OID);
                                                }
                                                updCnt++;
                                            }
                                        }
                                        else
                                        {
                                            pNewLocFeature = ((IFeatureClass)pServiceLocFeat.Class).GetFeature(Convert.ToInt32(lstLocationsNew[lstCntr_Loc].Split('#')[1]));
                                        }

                                        //relate GIS SRV points

                                        clsArcGISfunctionality.GetFieldValue(pGDBSrvPnt, "SERVICEPOINTCONVID", ref servicePntConvID);
                                        //1-M mode
                                        //remove existing relation create new location and build this relation
                                        //get related SrvPoint record from GIS
                                        pRelSet = pSrvLoc_Pnt_relCls.GetObjectsRelatedToObject((IObject)pServiceLocFeat);
                                        pRelRow = (IRow)pRelSet.Next();
                                        /// string strPgdbOID = null;
                                        while (pRelRow != null)
                                        {

                                            clsArcGISfunctionality.GetFieldValue(pRelRow, "CONVERSIONID", ref strval);
                                            //clsArcGISfunctionality.GetFieldValue(pRelRow, "APNNUM", ref strval);
                                            if (strval == servicePntConvID)
                                            {
                                                clsArcGISfunctionality.GetFieldValue(pGDBSrvPnt, "NEW_APNNUM", ref servicePntAPNnum);
                                                clsArcGISfunctionality.GetFieldValue(pGDBSrvPnt, "TRANS_CGC12", ref strXFRcgc12);
                                                // clsArcGISfunctionality.GetFieldValue(pGDBSrvPnt, "OBJECTID", ref strPgdbOID);



                                                string strnewGlbID = pNewLocFeature.get_Value(pNewLocFeature.Fields.FindField("GLOBALID")).ToString();
                                                string strnewConID = pNewLocFeature.get_Value(pNewLocFeature.Fields.FindField("CONVERSIONID")).ToString();

                                                //remove existing relations
                                                //string tr = pSrvLoc_Pnt_relCls.OriginForeignKey;
                                                pRelRow.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pRelRow.Table, pSrvLoc_Pnt_relCls.OriginForeignKey), pNewLocFeature.get_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pNewLocFeature.Table, "GLOBALID")));
                                                pRelRow.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pRelRow.Table, "APNNUM"), servicePntAPNnum);
                                                pRelRow.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pRelRow.Table, "CGC12"), strXFRcgc12);


                                                pRelRow.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pRelRow.Table, pXFR_SrvPnt_relCls.OriginForeignKey), pTransFeat.get_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pTransFeat.Table, "GLOBALID")));

                                                pRelRow.Store();
                                                updCnt++;

                                                // ClsLogReports.Common_addRowstoReportTable(ObjReportTable, strGlobalID + "," + pServiceLocFeat.OID + "," + strConvId + "," + oldLoc.X + "," + oldLoc.Y + "," + pSrvLocPnt_new.X + "," + pSrvLocPnt_new.Y + "," + strStatusComment + " New location created OID:" + pNewLocFeature.OID);
                                                ClsLogReports.Common_addRowstoReportTable(ObjReportTable, strnewGlbID + "," + pNewLocFeature.OID + "," + strnewConID + ",,,,,New RelationUpdated_GDBpoint OID:" + pGDBSrvPnt.OID);

                                                //ClsLogReports.Common_addRowstoReportTable(ObjReportTable, strnewGlbID + "," + pNewLocFeature.OID + "," + "" + "," + "" + "," + "" + "," + "" + "," + "" + "," + "New RelationUpdated_GDBpoint OID:" + pGDBSrvPnt.OID);


                                                break;
                                            }
                                            pRelRow = (IRow)pRelSet.Next();
                                        }
                                    }
                                    finally
                                    {
                                        if (pRelSet != null) { while (Marshal.ReleaseComObject(pRelSet) > 0) { } }
                                        if (pRelRow != null) { while (Marshal.ReleaseComObject(pRelRow) > 0) { } }
                                        if (pNewLocFeature != null) { while (Marshal.ReleaseComObject(pNewLocFeature) > 0) { } }
                                    }

                                    pSrvLocPnt_new = null;
                                    pGDBSrvPnt = (IRow)pCursLoc.NextRow();
                                }
                                if (pQryLoc != null) { while (Marshal.ReleaseComObject(pQryLoc) > 0) { } }
                                if (pCursLoc != null) { while (Marshal.ReleaseComObject(pCursLoc) > 0) { } }
                            }
                            //relocate if valid psuedo service connected to service location
                            else if (true == IsValidPseudoService(pServiceLocFeat))
                            {
                                IRow pSrvPntRow_APN = clsArcGISfunctionality.GetPrntFeatRow_ApnNum(pNewLocTbl, pLocInfoRow, "SERVICEPOINTCONVID,NEW_APNNUM");
                                //If already sevice location found, then relate service points to that location else move sevice location to new XY coord
                                if (intOid_Exists != null && blnFeatFound == true)
                                {
                                    // Function to Update service points to already present service location in buffer
                                    FuntoUpdateRelationandServiceLoc(pTransFeat, pServiceLocFeat, pSrvPntRow_APN, pGridFeat, intOid_Exists, pSrvLocPnt_new, ref updCnt, strGlobalID, strConvId, oldLoc, ref blnRelocated, ref strReporttableFormat);
                                }
                                else
                                {
                                    //Function to move service location to geocoding point or end of service present in grid
                                    FuntoMoveServiceloctoGecodingPoint(pTransFeat, pServiceLocFeat, pSrvPntRow_APN, pGridFeat, intOid_Exists, pSrvLocPnt_new, ref updCnt, strGlobalID, strConvId, oldLoc, ref blnRelocated, ref strReporttableFormat);
                                }

                            }
                            else
                            {
                                strReporttableFormat = strGlobalID + "," + pServiceLocFeat.OID + "," + strConvId + "," + oldLoc.X + "," + oldLoc.Y + "," + pSrvLocPnt_new.X + "," + pSrvLocPnt_new.Y + ",NOT connected to PseudoService-Not Relocated";
                            }
                        }
                        else
                        {
                            strReporttableFormat = strGlobalID + "," + pServiceLocFeat.OID + "," + strConvId + "," + oldLoc.X + "," + oldLoc.Y + "," + pSrvLocPnt_new.X + "," + pSrvLocPnt_new.Y + ",NOT Relocated @Dist: " + strDist;
                        }

                        if (!string.IsNullOrEmpty(strReporttableFormat))
                            ClsLogReports.Common_addRowstoReportTable(ObjReportTable, strReporttableFormat);

                        if (pServiceLocFeat != null)
                        {
                            if (true == validateServiceLocAndDelete(pServiceLocFeat))
                            {
                                ClsLogReports.Common_addRowstoReportTable(ObjReportTable, "" + "," + pServiceLocFeat.OID + "," + "" + "," + "" + "," + "" + "," + "" + "," + "" + "," + "Service location & Psuedo Service deleted(Service points are moved to new Geocoded Locations)");
                            }
                        }

                        pSrvLocPnt_new = null;
                        pLocInfoRow = (IRow)pCurs.NextRow();
                    }
                    catch (Exception ex1)
                    {
                        ClsLogReports.Common_addRowstoReportTable(ObjReportTable, strGlobalID + "," + strObjId + "," + strConvId + ",,,,,ServiceLocation Not Found");
                        pLocInfoRow = (IRow)pCurs.NextRow();
                    }
                    finally
                    {
                        if (pSrvLocPnt_new != null) { while (Marshal.ReleaseComObject(pSrvLocPnt_new) > 0) { } }
                        if (pGridFeat != null) { while (Marshal.ReleaseComObject(pGridFeat) > 0) { } }
                        if (pServiceLocFeat != null) { while (Marshal.ReleaseComObject(pServiceLocFeat) > 0) { } }
                        if (pTransFeat != null) { while (Marshal.ReleaseComObject(pTransFeat) > 0) { } }
                    }
                }

                clsCommonVariables.StopEditOperation(true);
            }
            catch (Exception ex) { ClsLogReports.writeLine("EXCP@Execute_1_M_GeoCode " + ex.Message); }
            finally
            {
                clsArcGISfunctionality.ReleaseCOM(pQry);
                clsArcGISfunctionality.ReleaseCOM(pCurs);
            }
        }

        //1:1
        private void Execute1_1GeoCode(string strGDBQry)
        {
            IQueryFilter pQry = null;
            ICursor pCurs = null;
            try
            {
                pQry = new QueryFilter();

                pQry.WhereClause = strGDBQry;
                pQry.WhereClause = strGDBQry + " and " + cmbAllFC.SelectedItem.ToString() + " order by SERVICELOCATIONCONVID";

                // pQry.WhereClause = "trans_cedsaid =1820028047 and SERVICEPT_COUNT=1";
                // pQry.WhereClause = "trans_cedsaid in('1820028036','1820028047','1820028037')";

                pQry.SubFields = "SERVICELOCATIONCONVID,SERVICELOCATION_OBJECTID,SERVICELOCATION_GLOBALID,TRANS_CEDSAID";

                int intCnt = pLocInfoTbl.RowCount(pQry);

                pCurs = pLocInfoTbl.Search(pQry, true);
                IRow pLocInfoRow = (IRow)pCurs.NextRow();

                //declare
                IFeature pServiceLocFeat = null, pTransFeat = null;
                IFeature pGridFeat = null;
                IPoint pSrvLocPnt_new = null;
                string strLnkTyp = string.Empty;
                string strLnkRat = string.Empty;
                string strConvId = string.Empty;
                string strObjId = string.Empty, strGlobalID = string.Empty;
                string strStatusComment = string.Empty;
                string strXFRcedsaid = string.Empty;
                string strXFRcgc12 = string.Empty;
                string strTransID = string.Empty;

                string strTransFeederInfo = string.Empty;
                string strTransCirCuitID = string.Empty;

                int intCntr = 0;
                object intOid_Exists = 0;
                int updCnt = 0;
                Boolean blnRelocated = false;

                if (false == clsCommonVariables.StartEditOperation())
                {
                    MessageBox.Show("Unable to start Edit operation");
                    return;
                }
                int rowsProcessed = 0;
                int saveCntr = 1;
                while (pLocInfoRow != null)
                {
                    rowsProcessed++;
                    intCntr++;
                    //init
                    strLnkTyp = string.Empty;
                    strLnkRat = string.Empty;
                    strConvId = string.Empty;
                    strGlobalID = string.Empty;
                    strObjId = string.Empty;
                    strXFRcedsaid = string.Empty;
                    strXFRcgc12 = string.Empty;
                    strStatusComment = string.Empty;
                    strReporttableFormat = string.Empty;
                    pGridFeat = null;


                    strTransID = string.Empty;
                    pSrvLocPnt_new = new PointClass();
                    blnRelocated = false;
                    intOid_Exists = 0;
                    try
                    {
                        lblMsg.Text = "Processing " + pSrvLocCls.AliasName + " " + intCntr + " of " + intCnt + "  Relocated: " + updCnt;
                        //formObj.stbStatusStrip.Refresh();                        
                        this.Refresh();

                        if (updCnt != 0 && saveCntr == 1)
                            saveCntr = updCnt;
                        if (saveCntr != 1 && updCnt % 500 == 0)
                        {
                            saveCntr = 1;
                            clsCommonVariables.StopEditOperation(true);
                            clsCommonVariables.StartEditOperation();
                        }

                        if (rowsProcessed > 500)
                        {
                            GC.Collect();
                            rowsProcessed = 0;
                        }

                        //string strmsg = "Processing " + pSrvLocCls.AliasName + " " + intCntr + " of " + intCnt + "  Relocated:" + updCnt;
                        Application.DoEvents();

                        //get object id instead of COnversionid
                        strConvId = pLocInfoRow.get_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pLocInfoRow.Table, "SERVICELOCATIONCONVID")).ToString();
                        strObjId = pLocInfoRow.get_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pLocInfoRow.Table, "SERVICELOCATION_OBJECTID")).ToString();
                        strGlobalID = pLocInfoRow.get_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pLocInfoRow.Table, "SERVICELOCATION_GLOBALID")).ToString();
                        strTransID = pLocInfoRow.get_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pLocInfoRow.Table, "TRANS_CEDSAID")).ToString();




                        //if (strConvId != "31357712" && strConvId != "31240377" && strConvId != "31303919" && strConvId != "31297518")
                        //{
                        //    pLocInfoRow = (IRow)pCurs.NextRow();
                        //    continue;

                        //}

                        pServiceLocFeat = GetFeature_globalid(strGlobalID);
                        //Get transformer feat with input transformerID
                        pTransFeat = GetFeature_TranformerID(strTransID);

                        // pServiceLocFeat = pSrvLocCls.GetFeature(Convert.ToInt32(strObjId));

                        if (pServiceLocFeat == null)
                        {
                            ClsLogReports.Common_addRowstoReportTable(ObjReportTable, strGlobalID + "," + strObjId + "," + strConvId + ",,,,,ServiceLocation Not Found");
                            pLocInfoRow = (IRow)pCurs.NextRow();
                            continue;
                        }
                        if (pTransFeat == null)
                        {
                            ClsLogReports.Common_addRowstoReportTable(ObjReportTable, strGlobalID + "," + strObjId + "," + strConvId + ",,,,,Transformer Not Found");
                            pLocInfoRow = (IRow)pCurs.NextRow();
                            continue;
                        }

                        if (_pNetworkFeature == null || _pGeonetWork == null)
                        {
                            _pNetworkFeature = (INetworkFeature)pTransFeat;
                            _pGeonetWork = _pNetworkFeature.GeometricNetwork;
                        }

                        clsArcGISfunctionality.GetFieldValue(pTransFeat, "feederinfo", ref strTransFeederInfo);
                        clsArcGISfunctionality.GetFieldValue(pTransFeat, "circuitid", ref strTransCirCuitID);

                        //get new location by validating no existing of servicelocation there, 
                        //returns true if location not exists then relocate the Srvlocation feature
                        Boolean blnFeatFound = GetNewLocation(pNewLocTbl, "SERVICELOCATION_GLOBALID", strGlobalID, ref pSrvLocPnt_new, ref intOid_Exists);

                        //get landbase grid feat based on input x,y 
                        pGridFeat = getGridnamefromFeature(pSrvLocPnt_new);


                        if (pGridFeat == null)
                        {
                            ClsLogReports.Common_addRowstoReportTable(ObjReportTable, strGlobalID + "," + strObjId + "," + strConvId + ",,,,,GridFeat Not Found");
                            pLocInfoRow = (IRow)pCurs.NextRow();
                            continue;
                        }

                        double dblRelOcDist = clsArcGISfunctionality.GetDistance(pSrvLocPnt_new, (IPoint)pServiceLocFeat.Shape);
                        //1 - mode, update when location feature is found with same XY co-ords and objectID matches
                        if (intOid_Exists != null && intOid_Exists.ToString() == strObjId)
                        {
                            strStatusComment = string.Empty;
                            GetUpdateAPNnum(pServiceLocFeat, pNewLocTbl, "SERVICELOCATION_GLOBALID", strGlobalID, "NEW_APNNUM", ref strStatusComment, intOid_Exists);
                            if (strStatusComment.Contains("More Related Service Point(s)"))
                            {
                                string servicePntConvID = string.Empty;
                                string servicePntAPNnum = string.Empty;

                                IRow pSrvPntRow = clsArcGISfunctionality.GetPrntFeatRow(pNewLocTbl, pLocInfoRow, "SERVICEPOINTCONVID,NEW_APNNUM");


                                clsArcGISfunctionality.GetFieldValue(pSrvPntRow, "SERVICEPOINTCONVID", ref servicePntConvID);

                                clsArcGISfunctionality.GetFieldValue(pSrvPntRow, "NEW_APNNUM", ref servicePntAPNnum);

                                ISet pRelSet = pSrvLoc_Pnt_relCls.GetObjectsRelatedToObject((IObject)pServiceLocFeat);
                                IRow pRelRow = (IRow)pRelSet.Next();
                                string strval = string.Empty;
                                while (pRelRow != null)
                                {

                                    clsArcGISfunctionality.GetFieldValue(pRelRow, "CONVERSIONID", ref strval);
                                    //clsArcGISfunctionality.GetFieldValue(pRelRow, "APNNUM", ref strval);
                                    if (strval == servicePntConvID)
                                    {
                                        //remove existing relations
                                        //string tr = pSrvLoc_Pnt_relCls.OriginForeignKey;
                                        //pRelRow.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pRelRow.Table, pSrvLoc_Pnt_relCls.OriginForeignKey), pNewLocFeature.get_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pNewLocFeature.Table, "GLOBALID")));
                                        pRelRow.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pRelRow.Table, "APNNUM"), servicePntAPNnum);

                                        pRelRow.Store();
                                        strStatusComment = strStatusComment + "-APNNUM:" + servicePntAPNnum + " Updated";
                                        break;
                                    }
                                    if (pRelRow != null) { while (Marshal.ReleaseComObject(pRelRow) > 0) { } }
                                    pRelRow = (IRow)pRelSet.Next();
                                }

                                if (pRelSet != null) { while (Marshal.ReleaseComObject(pRelSet) > 0) { } }
                                if (pSrvPntRow != null) { while (Marshal.ReleaseComObject(pSrvPntRow) > 0) { } }
                            }

                            strReporttableFormat = strGlobalID + "," + pServiceLocFeat.OID + "," + strConvId + "," + "" + "," + "" + "," + "" + "," + "" + ",NOT Relocated-@Dist" + dblRelOcDist + " " + strStatusComment;
                            ClsLogReports.Common_addRowstoReportTable(ObjReportTable, strReporttableFormat);
                            pLocInfoRow = (IRow)pCurs.NextRow();
                            updCnt++;
                            continue;
                        }
                        //1 - mode, update when location feature is found with diff XY co-ords and distance is less than 9999
                        IPoint oldLoc = (IPoint)((IGeometry)pServiceLocFeat.Shape);
                        //double dblRelOcDist = clsArcGISfunctionality.GetDistance(pSrvLocPnt_new, (IPoint)pServiceLocFeat.Shape);
                        string strDist = dblRelOcDist.ToString();
                        strDist = strDist.Substring(0, strDist.Length - 4);
                        if (dblRelOcDist > 1 && dblRelOcDist < 9999)
                        {
                            //update APN num in related SrvPnt record if only one record found else create new servicelocation and update related sevice points
                            if (true != GetUpdateAPNnum(pServiceLocFeat, pNewLocTbl, "SERVICELOCATION_GLOBALID", strGlobalID, "NEW_APNNUM", ref strStatusComment, intOid_Exists))
                            {
                                IPoint Servicept = null;
                                int serviceFound = 0;
                                object intOid = 0;

                                string servicePntConvID = string.Empty;
                                string servicePntAPNnum = string.Empty;
                                IRow pSrvPntRow = clsArcGISfunctionality.GetPrntFeatRow(pNewLocTbl, pLocInfoRow, "SERVICEPOINTCONVID,NEW_APNNUM,TRANS_CEDSAID,TRANS_CGC12");

                                clsArcGISfunctionality.GetFieldValue(pSrvPntRow, "SERVICEPOINTCONVID", ref servicePntConvID);
                                clsArcGISfunctionality.GetFieldValue(pSrvPntRow, "NEW_APNNUM", ref servicePntAPNnum);
                                clsArcGISfunctionality.GetFieldValue(pSrvPntRow, "TRANS_CEDSAID", ref strXFRcedsaid);
                                clsArcGISfunctionality.GetFieldValue(pSrvPntRow, "TRANS_CGC12", ref strXFRcgc12);
                                //1-M mode
                                //remove existing relation create new location and build this relation
                                //get related SrvPoint record from GIS
                                IFeature pNewLocFeature = pSrvLocCls.CreateFeature();
                                if (true == GetintersetSecUGConductors(pGridFeat, ref Servicept, ref serviceFound))
                                {
                                    if (false == clsArcGISfunctionality.IsPointFeatureExisting(Servicept, pSrvLocCls, ref intOid))
                                    {

                                        pNewLocFeature.Shape = (IGeometry)Servicept;
                                    }
                                    else
                                    {

                                        if (true == UpdRelationwithExistingSerLoc(pTransFeat, pServiceLocFeat, pSrvPntRow, intOid, ref updCnt, strGlobalID, strConvId, oldLoc, ref blnRelocated, ref strReporttableFormat))
                                        {
                                            if (!string.IsNullOrEmpty(strReporttableFormat))
                                                ClsLogReports.Common_addRowstoReportTable(ObjReportTable, strReporttableFormat);

                                            pSrvLocPnt_new = null;
                                            pLocInfoRow = (IRow)pCurs.NextRow();
                                            continue;

                                        }
                                        else
                                        {
                                            serviceFound = 0;

                                            pNewLocFeature.Shape = (IGeometry)pSrvLocPnt_new;

                                        }
                                    }
                                }
                                else
                                {
                                    pNewLocFeature.Shape = (IGeometry)pSrvLocPnt_new;
                                }

                                string str_locOffId_val = getLocalOfficeId(pSrvLocPnt_new);
                                string str_SerLoc_Phase_fromTrans = getPhase_fromTransformer(pTransFeat);
                                pNewLocFeature.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pNewLocFeature.Table, "CREATIONUSER"), "INFOTECH");
                                pNewLocFeature.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pNewLocFeature.Table, "DATECREATED"), String.Format("{0:M/d/yyyy}", DateTime.Now));
                                pNewLocFeature.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pNewLocFeature.Table, "CONVTRANSFORMERID"), strXFRcedsaid);
                                pNewLocFeature.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pNewLocFeature.Table, "CGC12"), strXFRcgc12);
                                pNewLocFeature.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pNewLocFeature.Table, "STATUS"), 5);
                                pNewLocFeature.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pNewLocFeature.Table, "SUBTYPECD"), 1);
                                pNewLocFeature.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pNewLocFeature.Table, "PHASEDESIGNATION"), str_SerLoc_Phase_fromTrans);
                                //pNewLocFeature.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pNewLocFeature.Table, "PHASEDESIGNATION"), 4);
                                pNewLocFeature.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pNewLocFeature.Table, "CUSTOMEROWNED"), "N");
                                pNewLocFeature.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pNewLocFeature.Table, "PHASINGVERIFIEDSTATUS"), "Estimated/Defaulted");
                                pNewLocFeature.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pNewLocFeature.Table, "STATE"), "CA");
                                pNewLocFeature.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pNewLocFeature.Table, "ENABLED"), "1");

                                //pNewLocFeature.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pNewLocFeature.Table, "ELECTRICTRACEWEIGHT"), "1610612784");                                
                                //pNewLocFeature.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pNewLocFeature.Table, "feederinfo"), "1");

                                pNewLocFeature.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pNewLocFeature.Table, "circuitid"), strTransCirCuitID);
                                pNewLocFeature.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pNewLocFeature.Table, "Installjobnumber"), "0");
                                pNewLocFeature.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pNewLocFeature.Table, "LOCALOFFICEID"), str_locOffId_val);




                                pNewLocFeature.Store();
                                updCnt++;

                                ISet pRelSet = pSrvLoc_Pnt_relCls.GetObjectsRelatedToObject((IObject)pServiceLocFeat);
                                IRow pRelRow = (IRow)pRelSet.Next();
                                string strval = string.Empty;
                                while (pRelRow != null)
                                {


                                    clsArcGISfunctionality.GetFieldValue(pRelRow, "CONVERSIONID", ref strval);
                                    //clsArcGISfunctionality.GetFieldValue(pRelRow, "APNNUM", ref strval);
                                    if (strval == servicePntConvID)
                                    {
                                        //remove existing relations
                                        string tr = pSrvLoc_Pnt_relCls.OriginForeignKey;
                                        pRelRow.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pRelRow.Table, pSrvLoc_Pnt_relCls.OriginForeignKey), pNewLocFeature.get_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pNewLocFeature.Table, "GLOBALID")));
                                        pRelRow.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pRelRow.Table, "APNNUM"), servicePntAPNnum);


                                        pRelRow.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pRelRow.Table, pXFR_SrvPnt_relCls.OriginForeignKey), pTransFeat.get_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pTransFeat.Table, "GLOBALID")));
                                        pRelRow.Store();
                                        break;
                                    }
                                    if (pRelRow != null) { while (Marshal.ReleaseComObject(pRelRow) > 0) { } }
                                    pRelRow = (IRow)pRelSet.Next();
                                }

                                if (pRelSet != null) { while (Marshal.ReleaseComObject(pRelSet) > 0) { } }

                                ClsLogReports.Common_addRowstoReportTable(ObjReportTable, strGlobalID + "," + pServiceLocFeat.OID + "," + strConvId + "," + oldLoc.X + "," + oldLoc.Y + "," + pSrvLocPnt_new.X + "," + pSrvLocPnt_new.Y + "," + strStatusComment + " New location created OID:" + pNewLocFeature.OID);


                                if (serviceFound == 0)
                                {
                                    createPseudoService(pNewLocFeature, pTransFeat);
                                }
                                if (true == FunctiontofindSecCond(pSrvLocPnt_new))
                                {
                                    strReporttableFormat = "";
                                    ClsLogReports.Common_addRowstoReportTable(ObjReportTable, strGlobalID + "," + pServiceLocFeat.OID + "," + strConvId + "," + oldLoc.X + "," + oldLoc.Y + "," + pSrvLocPnt_new.X + "," + pSrvLocPnt_new.Y + ",Extra Pseudo or Service Found at same lot!!Manually need to check)");
                                }

                                if (pNewLocFeature != null) { while (Marshal.ReleaseComObject(pNewLocFeature) > 0) { } }
                                if (pSrvPntRow != null) { while (Marshal.ReleaseComObject(pSrvPntRow) > 0) { } }
                                pLocInfoRow = (IRow)pCurs.NextRow();
                                continue;
                            }
                            //relocate situation
                            else if (true == IsValidPseudoService(pServiceLocFeat))
                            {

                                IRow pSrvPntRow_APN = clsArcGISfunctionality.GetPrntFeatRow_ApnNum(pNewLocTbl, pLocInfoRow, "SERVICEPOINTCONVID,NEW_APNNUM");
                                //If already sevice location found, then relate service points to that location else move sevice location to new XY coord
                                if (intOid_Exists != null && blnFeatFound == true)
                                {
                                    // Function to Update service points to already present service location in buffer
                                    FuntoUpdateRelationandServiceLoc(pTransFeat, pServiceLocFeat, pSrvPntRow_APN, pGridFeat, intOid_Exists, pSrvLocPnt_new, ref updCnt, strGlobalID, strConvId, oldLoc, ref blnRelocated, ref strReporttableFormat);

                                }
                                else
                                {
                                    //Function to move service location to geocoding point or end of service present in grid
                                    FuntoMoveServiceloctoGecodingPoint(pTransFeat, pServiceLocFeat, pSrvPntRow_APN, pGridFeat, intOid_Exists, pSrvLocPnt_new, ref updCnt, strGlobalID, strConvId, oldLoc, ref blnRelocated, ref strReporttableFormat);

                                }
                            }
                            else
                            {
                                strReporttableFormat = strGlobalID + "," + pServiceLocFeat.OID + "," + strConvId + "," + oldLoc.X + "," + oldLoc.Y + "," + pSrvLocPnt_new.X + "," + pSrvLocPnt_new.Y + ",NOT connected to PseudoService-Not Relocated-" + strStatusComment;
                                updCnt++;
                            }

                            if (dblRelOcDist > 999 && string.IsNullOrEmpty(strReporttableFormat))
                            {
                                strReporttableFormat = strGlobalID + "," + pServiceLocFeat.OID + "," + strConvId + "," + oldLoc.X + "," + oldLoc.Y + "," + pSrvLocPnt_new.X + "," + pSrvLocPnt_new.Y + ",Relocated @Dist: " + strDist;
                                if (true == FunctiontofindSecCond(pSrvLocPnt_new))
                                {
                                    strReporttableFormat = "";
                                    strReporttableFormat = strGlobalID + "," + pServiceLocFeat.OID + "," + strConvId + "," + oldLoc.X + "," + oldLoc.Y + "," + pSrvLocPnt_new.X + "," + pSrvLocPnt_new.Y + ",Relocated(Extra Pseudo or Service Found at same lot!!Manually need to check)";
                                }
                            }
                            else if (string.IsNullOrEmpty(strReporttableFormat))
                            {
                                strReporttableFormat = strGlobalID + "," + pServiceLocFeat.OID + "," + strConvId + "," + oldLoc.X + "," + oldLoc.Y + "," + pSrvLocPnt_new.X + "," + pSrvLocPnt_new.Y + ",Relocated";

                                if (true == FunctiontofindSecCond(pSrvLocPnt_new))
                                {
                                    strReporttableFormat = "";
                                    strReporttableFormat = strGlobalID + "," + pServiceLocFeat.OID + "," + strConvId + "," + oldLoc.X + "," + oldLoc.Y + "," + pSrvLocPnt_new.X + "," + pSrvLocPnt_new.Y + ",Relocated(Extra Pseudo or Service Found at same lot!!Manually need to check)";

                                }
                            }
                        }
                        else
                        {
                            strReporttableFormat = strGlobalID + "," + pServiceLocFeat.OID + "," + strConvId + "," + oldLoc.X + "," + oldLoc.Y + "," + pSrvLocPnt_new.X + "," + pSrvLocPnt_new.Y + ",NOT Relocated @Dist: " + strDist;
                        }

                        if (!string.IsNullOrEmpty(strReporttableFormat))
                            ClsLogReports.Common_addRowstoReportTable(ObjReportTable, strReporttableFormat);
                        else
                            ClsLogReports.writeLine("Might be skipped. Check GDB Table (which has counts) RowID: " + pLocInfoRow.OID);

                        pLocInfoRow = (IRow)pCurs.NextRow();
                    }
                    catch (Exception ex1)
                    {
                        ClsLogReports.Common_addRowstoReportTable(ObjReportTable, strGlobalID + "," + strObjId + "," + strConvId + ",,,,,ServiceLocation Not Found");
                        pLocInfoRow = (IRow)pCurs.NextRow();
                    }
                    finally
                    {
                        if (pSrvLocPnt_new != null) { while (Marshal.ReleaseComObject(pSrvLocPnt_new) > 0) { } }
                        if (pGridFeat != null) { while (Marshal.ReleaseComObject(pGridFeat) > 0) { } }
                        if (pServiceLocFeat != null) { while (Marshal.ReleaseComObject(pServiceLocFeat) > 0) { } }
                        if (pTransFeat != null) { while (Marshal.ReleaseComObject(pTransFeat) > 0) { } }
                    }
                }

                clsCommonVariables.StopEditOperation(true);
            }
            catch (Exception ex) { ClsLogReports.writeLine("EXCP@Execute_1_1_GeoCode " + ex.Message); }
            finally
            {
                clsArcGISfunctionality.ReleaseCOM(pQry);
                clsArcGISfunctionality.ReleaseCOM(pCurs);
            }
            lblMsg.Text = "Process Completed..!";
        }

        private static string getPhase_fromTransformer(IFeature pTransFeat)
        {
            string strPhase = string.Empty;
            try
            {

                strPhase = pTransFeat.get_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pTransFeat.Table, "PHASEDESIGNATION")).ToString();

                if (strPhase == "")
                {
                    strPhase = "4";
                }
                else
                {
                    if (strPhase == "4" || strPhase == "6" || strPhase == "5" || strPhase == "7") //A,AB,AC,ABC
                    {
                        strPhase = "4";//A
                    }
                    else if (strPhase == "3") //BC
                    {
                        strPhase = "2"; //B
                    }
                }
                return strPhase;
            }
            catch (Exception ex)
            {
                ClsLogReports.writeLine("EXCP@getPhase_fromTransformer " + ex.Message);
                return strPhase;
            }
        }

        private bool UpdRelationwithExistingSerLoc(IFeature pTransFeat, IFeature pServiceLocFeat, IRow pSrvPntRow, object intOid, ref int updCnt, string strGlobalID, string strConvId, IPoint oldLoc, ref bool blnRelocated, ref string strReporttableFormat)
        {

            int featFound = 0;
            ISet pRelSet = null;
            IRow pRelRow = null;
            string strval = string.Empty, TransformerID = string.Empty, strCedsaId = string.Empty;
            string servicePntConvID = string.Empty;
            string servicePntAPNnum = string.Empty;
            string strXFRcgc12 = string.Empty;
            IFeature pNewLocFeature = null;

            try
            {

                pNewLocFeature = pSrvLocCls.GetFeature(Convert.ToInt32(intOid));
                string strCGCnewnum = pNewLocFeature.get_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pNewLocFeature.Table, "CGC12")).ToString();
                string strCGColdnum = pServiceLocFeat.get_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pServiceLocFeat.Table, "CGC12")).ToString();


                string strnewGlbID = pNewLocFeature.get_Value(pNewLocFeature.Fields.FindField("GLOBALID")).ToString();
                string strnewConID = pNewLocFeature.get_Value(pNewLocFeature.Fields.FindField("CONVERSIONID")).ToString();

                clsArcGISfunctionality.GetFieldValue(pSrvPntRow, "SERVICEPOINTCONVID", ref servicePntConvID);
                clsArcGISfunctionality.GetFieldValue(pSrvPntRow, "NEW_APNNUM", ref servicePntAPNnum);

                if ((strCGColdnum == strCGCnewnum))// && (strLocationID != null))
                {
                    try
                    {
                        pRelSet = pSrvLoc_Pnt_relCls.GetObjectsRelatedToObject((IObject)pServiceLocFeat);
                        pRelRow = (IRow)pRelSet.Next();
                        strval = string.Empty;
                        while (pRelRow != null)
                        {

                            clsArcGISfunctionality.GetFieldValue(pRelRow, "CONVERSIONID", ref strval);
                            if (strval == servicePntConvID)
                            {

                                featFound++;

                                //remove existing relations                                           
                                pRelRow.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pRelRow.Table, pSrvLoc_Pnt_relCls.OriginForeignKey), pNewLocFeature.get_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pNewLocFeature.Table, "GLOBALID")));
                                pRelRow.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pRelRow.Table, "APNNUM"), servicePntAPNnum);



                                pRelRow.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pRelRow.Table, pXFR_SrvPnt_relCls.OriginForeignKey), pTransFeat.get_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pTransFeat.Table, "GLOBALID")));
                                pRelRow.Store();


                                // strReporttableFormat = "" + "," + pServiceLocFeat.OID + "," + "" + "," + "" + "," + "" + "," + "" + "," + "" + "," + " New RelationUpdated_GDBpoint OID:" + pRelRow.OID;
                                ClsLogReports.Common_addRowstoReportTable(ObjReportTable, strnewGlbID + "," + pNewLocFeature.OID + "," + strnewConID + "," + "" + "," + "" + "," + "" + "," + "" + "," + " New RelationUpdated_GDBpoint OID:" + pRelRow.OID);
                                updCnt++;
                                break;
                            }
                            if (pRelRow != null) { while (Marshal.ReleaseComObject(pRelRow) > 0) { } }
                            pRelRow = (IRow)pRelSet.Next();
                        }
                        return true;
                    }
                    catch (Exception ex1)
                    {

                        //strReporttableFormat = "" + "," + pServiceLocFeat.OID + "," + "" + "," + "" + "," + "" + "," + "" + "," + "" + "," + "Manual check required ";
                        ClsLogReports.Common_addRowstoReportTable(ObjReportTable, strGlobalID + "," + pServiceLocFeat.OID + "," + strConvId + "," + "" + "," + "" + "," + "" + "," + "" + "," + " EXCP@UpdRelationwithExistingSerLoc:" + ex1.Message);

                    }
                }
            }
            finally
            {
                if (pRelSet != null) { while (Marshal.ReleaseComObject(pRelSet) > 0) { } }
                if (pRelRow != null) { while (Marshal.ReleaseComObject(pRelRow) > 0) { } }
                if (pNewLocFeature != null) { while (Marshal.ReleaseComObject(pNewLocFeature) > 0) { } }
            }
            return false;
        }

        //Function to move service location to geocoding point or end of service present in grid
        private void FuntoMoveServiceloctoGecodingPoint(IFeature pTransFeat, IFeature pServiceLocFeat, IRow pSrvPntRow_APN, IFeature pGridFeat, object intOid_Exists, IPoint pSrvLocPnt_new, ref int updCnt, string strGlobalID, string strConvId, IPoint oldLoc, ref bool blnRelocated, ref string strReporttableFormat)
        {
            IPoint Servicept = null;
            int serviceFound = 0;
            object intOid = 0;

            //True means Service present in grid 
            if (true == GetintersetSecUGConductors(pGridFeat, ref Servicept, ref serviceFound))
            {
                double dblRelOcDist = clsArcGISfunctionality.GetDistance(Servicept, (IPoint)pServiceLocFeat.Shape);

                //False means Service location not found at end of service
                if ((false == clsArcGISfunctionality.IsPointFeatureExisting(Servicept, pSrvLocCls, ref intOid)) && (dblRelOcDist < 2000))
                {
                    pServiceLocFeat.Shape = (IGeometry)Servicept;

                    deletePsuedoService(pServiceLocFeat);
                    strReporttableFormat = strGlobalID + "," + pServiceLocFeat.OID + "," + strConvId + "," + oldLoc.X + "," + oldLoc.Y + "," + pSrvLocPnt_new.X + "," + pSrvLocPnt_new.Y + ",Relocated at service end";

                    pServiceLocFeat.Store();
                    updCnt++;
                    blnRelocated = true;
                    strReporttableFormat = string.Empty;
                }
                else
                {
                    //Service location found at end of Serive.If cgc12 matches relation updated otherwise service location will be created at geocoded point
                    ISet pRelSet = null;
                    IRow pRelRow = null;
                    string strval = string.Empty, TransformerID = string.Empty, strCedsaId = string.Empty;
                    string servicePntConvID = string.Empty;
                    string servicePntAPNnum = string.Empty;
                    string strXFRcgc12 = string.Empty;
                    IFeature pNewLocFeature = null;
                    int featFound = 0;

                    try
                    {
                        pNewLocFeature = pSrvLocCls.GetFeature(Convert.ToInt32(intOid));

                        string strCGCnewnum = pNewLocFeature.get_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pNewLocFeature.Table, "CGC12")).ToString();

                        //strLocationID = pNewLocFeature.get_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pNewLocFeature.Table, "LOCATIONID")).ToString();
                        string strCGColdnum = pServiceLocFeat.get_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pServiceLocFeat.Table, "CGC12")).ToString();



                        string strnewGlbID = pNewLocFeature.get_Value(pNewLocFeature.Fields.FindField("GLOBALID")).ToString();
                        string strnewConID = pNewLocFeature.get_Value(pNewLocFeature.Fields.FindField("CONVERSIONID")).ToString();

                        // clsArcGISfunctionality.GetFieldValue(pSrvPntRow_APN, "SERVICEPOINTCONVID", ref servicePntConvID);
                        clsArcGISfunctionality.GetFieldValue(pSrvPntRow_APN, "NEW_APNNUM", ref servicePntAPNnum);



                        if ((strCGColdnum == strCGCnewnum))
                        {
                            try
                            {
                                //  clsArcGISfunctionality.GetFieldValue(pLocInfoRow, "SERVICEPOINTCONVID", ref servicePntConvID);
                                pRelSet = pSrvLoc_Pnt_relCls.GetObjectsRelatedToObject((IObject)pServiceLocFeat);
                                pRelRow = (IRow)pRelSet.Next();
                                strval = string.Empty;
                                while (pRelRow != null)
                                {

                                    // clsArcGISfunctionality.GetFieldValue(pRelRow, "CONVERSIONID", ref strval);
                                    //  if (strval == servicePntConvID)
                                    // {

                                    if (rad1_1.Checked == true)
                                    {

                                        if (pRelSet.Count > 1)
                                        {
                                            string strApn = pRelRow.get_Value(pRelRow.Fields.FindField("APNNUM")).ToString();
                                            if (strApn == "")
                                            {
                                                featFound++;
                                                //featFound++;
                                                //remove existing relations                                           
                                                pRelRow.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pRelRow.Table, pSrvLoc_Pnt_relCls.OriginForeignKey), pNewLocFeature.get_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pNewLocFeature.Table, "GLOBALID")));
                                                pRelRow.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pRelRow.Table, "APNNUM"), servicePntAPNnum);

                                                pRelRow.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pRelRow.Table, pXFR_SrvPnt_relCls.OriginForeignKey), pTransFeat.get_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pTransFeat.Table, "GLOBALID")));
                                                pRelRow.Store();
                                                strReporttableFormat = strnewGlbID + "," + pNewLocFeature.OID + "," + strnewConID + "," + "" + "," + "" + "," + "" + "," + "" + "," + " New RelationUpdated_GDBpoint OID:" + pRelRow.OID + " Deleted features:" + pServiceLocFeat.OID;
                                                updCnt++;
                                            }
                                        }
                                        else
                                        {

                                            featFound++;
                                            //featFound++;
                                            //remove existing relations                                           
                                            pRelRow.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pRelRow.Table, pSrvLoc_Pnt_relCls.OriginForeignKey), pNewLocFeature.get_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pNewLocFeature.Table, "GLOBALID")));
                                            pRelRow.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pRelRow.Table, "APNNUM"), servicePntAPNnum);

                                            pRelRow.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pRelRow.Table, pXFR_SrvPnt_relCls.OriginForeignKey), pTransFeat.get_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pTransFeat.Table, "GLOBALID")));
                                            pRelRow.Store();

                                            strReporttableFormat = strnewGlbID + "," + pNewLocFeature.OID + "," + strnewConID + "," + "" + "," + "" + "," + "" + "," + "" + "," + " New RelationUpdated_GDBpoint OID:" + pRelRow.OID + " Deleted features:" + pServiceLocFeat.OID;
                                            updCnt++;

                                        }
                                    }
                                    else
                                    {
                                        featFound++;
                                        //featFound++;
                                        //remove existing relations                                           
                                        pRelRow.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pRelRow.Table, pSrvLoc_Pnt_relCls.OriginForeignKey), pNewLocFeature.get_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pNewLocFeature.Table, "GLOBALID")));
                                        pRelRow.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pRelRow.Table, "APNNUM"), servicePntAPNnum);

                                        pRelRow.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pRelRow.Table, pXFR_SrvPnt_relCls.OriginForeignKey), pTransFeat.get_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pTransFeat.Table, "GLOBALID")));
                                        pRelRow.Store();
                                        strReporttableFormat = strnewGlbID + "," + pNewLocFeature.OID + "," + strnewConID + "," + "" + "," + "" + "," + "" + "," + "" + "," + " New RelationUpdated_GDBpoint OID:" + pRelRow.OID + " Deleted features:" + pServiceLocFeat.OID;
                                        updCnt++;

                                    }
                                    //}
                                    pRelRow = (IRow)pRelSet.Next();
                                }

                                pSrvLocPnt_new = null;


                                if (true == validateServiceLocAndDelete(pServiceLocFeat))
                                {
                                    ClsLogReports.Common_addRowstoReportTable(ObjReportTable, "" + "," + pServiceLocFeat.OID + "," + "" + "," + "" + "," + "" + "," + "" + "," + "" + "," + "Service location & Psuedo Service deleted(Service points are moved to new Geocoded Locations)");
                                }


                                ////Delete psuedo Service
                                //if (true == deletePsuedoService(pServiceLocFeat))
                                //{
                                //    pServiceLocFeat.Delete();
                                //}

                            }
                            catch (Exception ex1)
                            {


                                //strReporttableFormat = "" + "," + intOid + "," + "" + "," + "" + "," + "" + "," + "" + "," + "" + "," + "Manual check required ";

                                ClsLogReports.Common_addRowstoReportTable(ObjReportTable, strGlobalID + "," + pServiceLocFeat.OID + "," + strConvId + "," + "" + "," + "" + "," + "" + "," + "" + "," + " EXCP@FuntoMoveServiceloctoGecodingPoint:" + ex1.Message);
                            }
                        }
                        else
                        {
                            pServiceLocFeat.Shape = (IGeometry)pSrvLocPnt_new;
                            pServiceLocFeat.Store();
                            updCnt++;
                            blnRelocated = true;
                            strReporttableFormat = string.Empty;
                        }
                    }
                    finally
                    {
                        if (pRelSet != null) { while (Marshal.ReleaseComObject(pRelSet) > 0) { } }
                        if (pRelRow != null) { while (Marshal.ReleaseComObject(pRelRow) > 0) { } }
                        if (pNewLocFeature != null) { while (Marshal.ReleaseComObject(pNewLocFeature) > 0) { } }
                    }
                }
            }
            else
            {
                pServiceLocFeat.Shape = (IGeometry)pSrvLocPnt_new;
                pServiceLocFeat.Store();
                updCnt++;
                blnRelocated = true;
                strReporttableFormat = string.Empty;

            }
        }

        // Function to Update service points to already present service location in buffer
        private void FuntoUpdateRelationandServiceLoc(IFeature pTransFeat, IFeature pServiceLocFeat, IRow pSrvPntRow_APN, IFeature pGridFeat, object intOid_Exists, IPoint pSrvLocPnt_new, ref int updCnt, string strGlobalID, string strConvId, IPoint oldLoc, ref bool blnRelocated, ref string strReporttableFormat)
        {
            //lstLocationsNew.Add
            IList<string> sevlocations = new List<string>();

            IPoint Servicept = null;
            int serviceFound = 0;
            object intOid = 0;

            //Get service location at geocoding point
            IFeature pFeature = pSrvLocCls.GetFeature(Convert.ToInt32(intOid_Exists));

            try
            {

                //Fun to find any service already present in grid
                if (true == GetintersetSecUGConductors(pGridFeat, ref Servicept, ref serviceFound))
                {
                    //If service found in grid,Any service location connected to it in grid
                    if (true == clsArcGISfunctionality.IsPointFeatureExisting(Servicept, pSrvLocCls, ref intOid))
                    {
                        sevlocations.Add(intOid.ToString());
                    }
                    else
                    {
                        IsPointFeatureExisting_buffer(pFeature, pSrvLocCls, sevlocations, pServiceLocFeat);
                    }
                }
                else
                {
                    //Function to find service locations at Geococoding point with buffer
                    IsPointFeatureExisting_buffer(pFeature, pSrvLocCls, sevlocations, pServiceLocFeat);
                }

                if (sevlocations.Count == 1)
                {

                    for (int i = 0; i < sevlocations.Count; i++)
                    {
                        int featFound = 0;
                        ISet pRelSet = null;
                        IRow pRelRow = null;
                        string strval = string.Empty, TransformerID = string.Empty, strCedsaId = string.Empty;
                        string servicePntConvID = string.Empty;
                        string servicePntAPNnum = string.Empty;
                        string strXFRcgc12 = string.Empty;
                        IFeature pNewLocFeature = null;

                        pNewLocFeature = pSrvLocCls.GetFeature(Convert.ToInt32(sevlocations[i]));
                        string strCGCnewnum = pNewLocFeature.get_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pNewLocFeature.Table, "CGC12")).ToString();



                        string strnewGlbID = pNewLocFeature.get_Value(pNewLocFeature.Fields.FindField("GLOBALID")).ToString();
                        string strnewConID = pNewLocFeature.get_Value(pNewLocFeature.Fields.FindField("CONVERSIONID")).ToString();

                        //clsArcGISfunctionality.GetFieldValue(pSrvPntRow_APN, "SERVICEPOINTCONVID", ref servicePntConvID);
                        clsArcGISfunctionality.GetFieldValue(pSrvPntRow_APN, "NEW_APNNUM", ref servicePntAPNnum);

                        //strLocationID = pNewLocFeature.get_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pNewLocFeature.Table, "LOCATIONID")).ToString();
                        string strCGColdnum = pServiceLocFeat.get_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pServiceLocFeat.Table, "CGC12")).ToString();
                        if ((strCGColdnum == strCGCnewnum))// && (strLocationID != null))
                        {
                            try
                            {
                                pRelSet = pSrvLoc_Pnt_relCls.GetObjectsRelatedToObject((IObject)pServiceLocFeat);
                                pRelRow = (IRow)pRelSet.Next();
                                strval = string.Empty;
                                while (pRelRow != null)
                                {
                                    //clsArcGISfunctionality.GetFieldValue(pRelRow, "CONVERSIONID", ref strval);
                                    // if (strval == servicePntConvID)
                                    // {

                                    if (rad1_1.Checked == true)
                                    {
                                        if (pRelSet.Count > 1)
                                        {
                                            string strApn = pRelRow.get_Value(pRelRow.Fields.FindField("APNNUM")).ToString();
                                            if (strApn == "")
                                            {
                                                featFound++;
                                                //featFound++;
                                                //remove existing relations                                           
                                                pRelRow.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pRelRow.Table, pSrvLoc_Pnt_relCls.OriginForeignKey), pNewLocFeature.get_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pNewLocFeature.Table, "GLOBALID")));
                                                pRelRow.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pRelRow.Table, "APNNUM"), servicePntAPNnum);

                                                pRelRow.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pRelRow.Table, pXFR_SrvPnt_relCls.OriginForeignKey), pTransFeat.get_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pTransFeat.Table, "GLOBALID")));

                                                pRelRow.Store();
                                                strReporttableFormat = strnewGlbID + "," + pNewLocFeature.OID + "," + strnewConID + "," + "" + "," + "" + "," + "" + "," + "" + "," + " New RelationUpdated_GDBpoint OID:" + pRelRow.OID + " Deleted features:" + pServiceLocFeat.OID;
                                                updCnt++;


                                            }
                                        }
                                        else
                                        {
                                            featFound++;
                                            //featFound++;
                                            //remove existing relations                                           
                                            pRelRow.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pRelRow.Table, pSrvLoc_Pnt_relCls.OriginForeignKey), pNewLocFeature.get_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pNewLocFeature.Table, "GLOBALID")));
                                            pRelRow.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pRelRow.Table, "APNNUM"), servicePntAPNnum);

                                            pRelRow.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pRelRow.Table, pXFR_SrvPnt_relCls.OriginForeignKey), pTransFeat.get_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pTransFeat.Table, "GLOBALID")));
                                            pRelRow.Store();
                                            strReporttableFormat = strnewGlbID + "," + pNewLocFeature.OID + "," + strnewConID + "," + "" + "," + "" + "," + "" + "," + "" + "," + " New RelationUpdated_GDBpoint OID:" + pRelRow.OID + " Deleted features:" + pServiceLocFeat.OID;
                                            updCnt++;
                                        }
                                    }
                                    else
                                    {
                                        featFound++;
                                        //featFound++;
                                        //remove existing relations                                           
                                        pRelRow.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pRelRow.Table, pSrvLoc_Pnt_relCls.OriginForeignKey), pNewLocFeature.get_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pNewLocFeature.Table, "GLOBALID")));
                                        pRelRow.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pRelRow.Table, "APNNUM"), servicePntAPNnum);
                                        pRelRow.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pRelRow.Table, pXFR_SrvPnt_relCls.OriginForeignKey), pTransFeat.get_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pTransFeat.Table, "GLOBALID")));

                                        pRelRow.Store();
                                        strReporttableFormat = strnewGlbID + "," + pNewLocFeature.OID + "," + strnewConID + "," + "" + "," + "" + "," + "" + "," + "" + "," + " New RelationUpdated_GDBpoint OID:" + pRelRow.OID + " Deleted features:" + pServiceLocFeat.OID;
                                        updCnt++;

                                    }
                                    //}
                                    if (pRelRow != null) { while (Marshal.ReleaseComObject(pRelRow) > 0) { } }
                                    pRelRow = (IRow)pRelSet.Next();
                                }

                                pSrvLocPnt_new = null;

                                if (true == validateServiceLocAndDelete(pServiceLocFeat))
                                {
                                    ClsLogReports.Common_addRowstoReportTable(ObjReportTable, "" + "," + pServiceLocFeat.OID + "," + "" + "," + "" + "," + "" + "," + "" + "," + "" + "," + "Service location & Psuedo Service deleted(Service points are moved to new Geocoded Locations)");
                                }

                                //Delete psuedo Service
                                //if (true == deletePsuedoService(pServiceLocFeat))
                                //{
                                //    //DeleteService location
                                //    pServiceLocFeat.Delete();
                                //}
                                break;
                            }
                            catch (Exception ex1)
                            {

                                //strReporttableFormat = "" + "," + pFeature.OID + "," + "" + "," + "" + "," + "" + "," + "" + "," + "" + "," + "Manual check required ";
                                ClsLogReports.Common_addRowstoReportTable(ObjReportTable, strGlobalID + "," + pServiceLocFeat.OID + "," + strConvId + "," + "" + "," + "" + "," + "" + "," + "" + "," + " EXCP@FuntoUpdateRelationandServiceLoc:" + ex1.Message);
                            }
                            finally
                            {
                                if (pRelSet != null) { while (Marshal.ReleaseComObject(pRelSet) > 0) { } }
                            }
                        }
                        if (pNewLocFeature != null) { while (Marshal.ReleaseComObject(pNewLocFeature) > 0) { } }
                    }

                }
                else if (sevlocations.Count > 1)
                {

                    strReporttableFormat = "" + "," + pFeature.OID + "," + "" + "," + "" + "," + "" + "," + "" + "," + "" + "," + " Manual review required to relate approriate Service locations for :" + pServiceLocFeat.OID;

                }
                else if (sevlocations.Count == 0)
                {
                    pSrvLocPnt_new.X = pSrvLocPnt_new.X + 10;
                    pSrvLocPnt_new.Y = pSrvLocPnt_new.Y + 10;

                    if (true == GetintersetSecUGConductors(pGridFeat, ref Servicept, ref serviceFound))
                    {
                        if (false == clsArcGISfunctionality.IsPointFeatureExisting(Servicept, pSrvLocCls, ref intOid))
                        {
                            pServiceLocFeat.Shape = (IGeometry)Servicept;

                            deletePsuedoService(pServiceLocFeat);
                            strReporttableFormat = strGlobalID + "," + pServiceLocFeat.OID + "," + strConvId + "," + oldLoc.X + "," + oldLoc.Y + "," + pSrvLocPnt_new.X + "," + pSrvLocPnt_new.Y + ",Relocated at service end";
                        }
                        else
                        {
                            pServiceLocFeat.Shape = (IGeometry)pSrvLocPnt_new;
                        }
                    }
                    else
                    {
                        IFeature pTempFeat = null;
                        do
                        {
                            pSrvLocPnt_new.X = pSrvLocPnt_new.X + 10;
                            pSrvLocPnt_new.Y = pSrvLocPnt_new.Y + 10;

                        } while (true == clsArcGISfunctionality.IsPointFeatureExisting(pSrvLocPnt_new, pSrvLocCls, 1.0, ref pTempFeat));

                        pServiceLocFeat.Shape = (IGeometry)pSrvLocPnt_new;
                    }
                    //pServiceLocFeat.Shape = (IGeometry)pSrvLocPnt_new;
                    pServiceLocFeat.Store();
                    updCnt++;
                    blnRelocated = true;
                    strReporttableFormat = string.Empty;

                }

            }
            finally
            {
                if (pFeature != null) { while (Marshal.ReleaseComObject(pFeature) > 0) { } }
            }
        }

        private bool GetintersetSecUGConductors(IFeature pGridFeat, ref IPoint Servicept, ref int serviceFound)
        {

            IFeature pSrcFeat = null;
            string strSubType = null;
            int intSerFnd = 0, intstrFnd = 0, intEndFnd = 0, UGrowcount = 0;
            try
            {
                IFeatureCursor ConnectedMapCursor = null;
                IGeometry queryGeometry = pGridFeat.ShapeCopy;
                ISpatialFilter spatialFilter = new SpatialFilterClass();
                spatialFilter.Geometry = queryGeometry;
                spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                spatialFilter.SubFields = "OBJECTID,SUBTYPECD,SHAPE";
                using (ComReleaser comReleaser = new ComReleaser())
                {
                    ConnectedMapCursor = pUGsrvCls.Search(spatialFilter, true);
                    comReleaser.ManageLifetime(ConnectedMapCursor);
                    UGrowcount = pUGsrvCls.FeatureCount(spatialFilter);
                    if ((UGrowcount > 0))
                    {
                        IPolyline Ppoly = null;

                        IRelationalOperator2 relation = pGridFeat.Shape as IRelationalOperator2;
                        //int count = 0;
                        while ((pSrcFeat = ConnectedMapCursor.NextFeature()) != null)
                        {
                            comReleaser.ManageLifetime(pSrcFeat);
                            Ppoly = null;
                            Ppoly = pSrcFeat.ShapeCopy as IPolyline;
                            strSubType = pSrcFeat.get_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pSrcFeat.Table, "SUBTYPECD")).ToString();
                            if (strSubType == "3")//Service
                            {
                                //Service from and to points are in same grid,So we are checking at both ends
                                if (relation.Contains(Ppoly.FromPoint))
                                {
                                    if (true == functofindjunctions(pSrcFeat, Ppoly.FromPoint))
                                    {
                                        Servicept = Ppoly.FromPoint;
                                        intstrFnd = intstrFnd + 1;
                                        intSerFnd = intSerFnd + 1;

                                    }
                                }
                                if (relation.Contains(Ppoly.ToPoint))
                                {
                                    if (true == functofindjunctions(pSrcFeat, Ppoly.ToPoint))
                                    {
                                        Servicept = Ppoly.ToPoint;
                                        intEndFnd = intEndFnd + 1;
                                        intSerFnd = intSerFnd + 1;
                                    }
                                }

                            }

                        }
                        if ((intstrFnd > 0) && (intEndFnd > 0))
                        {
                            return false;
                        }

                        if (intSerFnd == 1)
                        {
                            serviceFound = intSerFnd;
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ClsLogReports.writeLine("EXCP@Error at GetintersetSecUGConductors " + ex.Message);
            }
            return false;

        }


        private bool functofindjunctions(IFeature pSrcFeat, IPoint iPoint)
        {
            bool status = false;
            int intLinearCnt_fromPnt = 0;
            int intLinearCnt_EndPnt = 0;
            try
            {
                IPolyline pPolyline = (IPolyline)pSrcFeat.Shape;
                IPointCollection pPointCollection = (IPointCollection)pPolyline;

                getConnectedConductors_AtPnt(pPointCollection.get_Point(0), ref intLinearCnt_fromPnt);
                getConnectedConductors_AtPnt(pPointCollection.get_Point(pPointCollection.PointCount - 1), ref intLinearCnt_EndPnt);

                if (intLinearCnt_fromPnt == 1)
                {
                    //IFeature pEndFeat = (IFeature)pfromJunctionFeature;
                    //IPoint pJuncpt = (IPoint)pEndFeat.Shape;
                    double Dist = clsArcGISfunctionality.GetDistance(pPointCollection.get_Point(0), iPoint);
                    if (Dist < 1)
                    {
                        return true;
                    }
                }
                else if (intLinearCnt_EndPnt == 1)
                {
                    //IFeature ptoFeat = (IFeature)pSimpleJunctionFeature;
                    //IPoint pJuncpt = (IPoint)ptoFeat.Shape;
                    double Dist = clsArcGISfunctionality.GetDistance(pPointCollection.get_Point(pPointCollection.PointCount - 1), iPoint);
                    if (Dist < 1)
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                ClsLogReports.writeLine("functofindjunctions " + ex.Message + pSrcFeat.OID);
            }
            return status;

            //bool status = false;

            //IEdgeFeature pEdgeFeature = (IEdgeFeature)pSrcFeat;
            //ISimpleJunctionFeature pfromJunctionFeature = (ISimpleJunctionFeature)pEdgeFeature.FromJunctionFeature;
            //ISimpleJunctionFeature pSimpleJunctionFeature = (ISimpleJunctionFeature)pEdgeFeature.ToJunctionFeature;
            //if (pfromJunctionFeature.EdgeFeatureCount == 1)
            //{
            //    IFeature pEndFeat = (IFeature)pfromJunctionFeature;
            //    IPoint pJuncpt = (IPoint)pEndFeat.Shape;
            //    double Dist = clsArcGISfunctionality.GetDistance(pJuncpt, iPoint);

            //    if (Dist < 1)
            //    {
            //        return true;
            //    }
            //}
            //else if (pSimpleJunctionFeature.EdgeFeatureCount == 1)
            //{
            //    IFeature ptoFeat = (IFeature)pSimpleJunctionFeature;
            //    IPoint pJuncpt = (IPoint)ptoFeat.Shape;
            //    double Dist = clsArcGISfunctionality.GetDistance(pJuncpt, iPoint);

            //    if (Dist < 1)
            //    {
            //        return true;
            //    }
            //}
            //return status;
        }
        /// <summary>
        /// Function to get linear features from GeonetWork at given input point 
        /// </summary>
        /// <param name="pTstPnt"></param>        
        /// <param name="intcnt"></param>
        /// <returns></returns>
        private void getConnectedConductors_AtPnt(IPoint pTstPnt, ref int intcnt)
        {
            IFeature pFeat = null;
            intcnt = 0;
            IEnumFeature pEnum = null;
            IEnumFeature pEnum2 = null;
            try
            {
                pEnum = _pGeonetWork.SearchForNetworkFeature(pTstPnt, esriFeatureType.esriFTComplexEdge);
                pEnum.Reset();

                pFeat = pEnum.Next();
                while (pFeat != null)
                {
                    intcnt++;
                    pFeat = pEnum.Next();
                }
                pEnum2 = _pGeonetWork.SearchForNetworkFeature(pTstPnt, esriFeatureType.esriFTSimpleEdge);
                pEnum2.Reset();

                pFeat = pEnum2.Next();
                while (pFeat != null)
                {
                    intcnt++;
                    pFeat = pEnum2.Next();
                }
            }
            catch (Exception ex)
            {
                ClsLogReports.writeLine("EXCP@getConnectedConductors_AtPnt " + ex.Message);
            }
            finally
            {
                if (pEnum != null)
                {
                    while (Marshal.ReleaseComObject(pEnum) > 0) { }
                    pEnum = null;
                }
                if (pEnum2 != null)
                {
                    while (Marshal.ReleaseComObject(pEnum2) > 0) { }
                    pEnum2 = null;
                }
            }
            ////Boolean blnStatus = false;
            //ISpatialFilter pSptlFilter = new SpatialFilterClass();
            ////IFeatureCursor NodeFeatCurs = null;
            //intcnt = 0;
            //try
            //{
            //    IBufferConstruction pBuffConst = new BufferConstructionClass();
            //    IGeometry pBuffGeom = pBuffConst.Buffer((IGeometry)pTstPnt, 0.5);

            //    pSptlFilter.Geometry = (IGeometry)pBuffGeom;
            //    pSptlFilter.GeometryField = pUGsrvCls.ShapeFieldName;
            //    pSptlFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
            //    //NodeFeatCurs = pUGsrvCls.Search(pSptlFilter, false);
            //    intcnt = pUGsrvCls.FeatureCount(pSptlFilter);
            //}
            //catch (Exception ex)
            //{
            //    ClsLogReports.writeLine("EXCP@getConnectedConductors_AtPnt " + ex.Message);
            //}
            //finally
            //{
            //    if (pSptlFilter != null)
            //    {
            //        while (Marshal.ReleaseComObject(pSptlFilter) > 0) { }
            //        pSptlFilter = null;
            //    }
            //}

        }






        //private bool functofindjunctions(IFeature pSrcFeat, IPoint iPoint)
        //{
        //    bool status = false;

        //    IEdgeFeature pEdgeFeature = (IEdgeFeature)pSrcFeat;

        //    ISimpleJunctionFeature pfromJunctionFeature = (ISimpleJunctionFeature)pEdgeFeature.FromJunctionFeature;
        //    ISimpleJunctionFeature pSimpleJunctionFeature = (ISimpleJunctionFeature)pEdgeFeature.ToJunctionFeature;


        //    if (pfromJunctionFeature.EdgeFeatureCount == 1)
        //    {
        //        IFeature pEndFeat = (IFeature)pfromJunctionFeature;
        //        IPoint pJuncpt = (IPoint)pEndFeat.Shape;
        //        double Dist = clsArcGISfunctionality.GetDistance(pJuncpt, iPoint);

        //        if (Dist < 1)
        //        {
        //            return true;
        //        }
        //    }
        //    else if (pSimpleJunctionFeature.EdgeFeatureCount == 1)
        //    {
        //        IFeature ptoFeat = (IFeature)pSimpleJunctionFeature;
        //        IPoint pJuncpt = (IPoint)ptoFeat.Shape;
        //        double Dist = clsArcGISfunctionality.GetDistance(pJuncpt, iPoint);

        //        if (Dist < 1)
        //        {
        //            return true;
        //        }
        //    }
        //    return status;

        //}

        private Boolean validateServiceLocAndDelete(IFeature pServiceLocFeat)
        {
            Boolean blnStatus = false;
            //object ObjectID = null;            
            string GlobalID = string.Empty;
            ICursor pFCursor = null;
            IRow pSerRow = null;
            IQueryFilter pFilter = new QueryFilterClass();
            string AnnoObjectID = string.Empty, FeatConversionID = string.Empty, AnnoClassID = string.Empty;
            try
            {
                GlobalID = pServiceLocFeat.get_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pServiceLocFeat.Table, "GLOBALID")).ToString();

                pFilter.WhereClause = "SERVICELOCATIONGUID ='" + GlobalID + "'";

                using (ComReleaser cr = new ComReleaser())
                {
                    //pFCursor = pSrvPtTbl.Search(pFilter, true);
                    //cr.ManageLifetime(pFCursor);
                    //pSerRow = pFCursor.NextRow();

                    int rowcount = pSrvPtTbl.RowCount(pFilter);
                    if (rowcount == 0)
                    {
                        if (true == deletePsuedoService(pServiceLocFeat))
                        {
                            pServiceLocFeat.Delete();
                            blnStatus = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ClsLogReports.writeLine("validateServiceLocAndDelete " + ex.Message + "  " + pServiceLocFeat.OID);
            }

            return blnStatus;
        }

        //Function to get feature based on TransformerID
        private IFeature GetFeature_TranformerID(string strTransID)
        {
            IQueryFilter pQry = null;
            IFeatureCursor pCurs = null;
            IFeature pfeat = null;
            try
            {
                //get GDB service points for the location
                pQry = new QueryFilter();
                pQry.WhereClause = " CEDSADEVICEID= '" + strTransID + "'";
                pQry.SubFields = "OBJECTID,SHAPE,FEEDERINFO,CIRCUITID,PHASEDESIGNATION,LOWSIDEVOLTAGE";
                pCurs = pXFRCls.Search(pQry, false);
                pfeat = pCurs.NextFeature();
            }
            catch (Exception ex)
            {
                ClsLogReports.writeLine("EXCP@GetFeature_TranformerID. Transformer_CedsaDeviceId: " + strTransID + "  " + ex.Message);
            }
            finally
            {
                if (pQry != null) { while (Marshal.ReleaseComObject(pQry) > 0) { } }
                if (pCurs != null) { while (Marshal.ReleaseComObject(pCurs) > 0) { } }
            }
            return pfeat;
        }

        //Function to get feature based on GlobalID
        private IFeature GetFeature_globalid(string strGlobalID)
        {
            IQueryFilter pQry = null;
            IFeatureCursor pCurs = null;
            IFeature pfeat = null;

            try
            {
                //get GDB service points for the location
                pQry = new QueryFilter();
                pQry.WhereClause = "GLOBALID = '" + strGlobalID + "'";
                pQry.SubFields = "OBJECTID,SHAPE,SUBTYPECD,CGC12,GLOBALID";
                pCurs = pSrvLocCls.Search(pQry, false);
                int recCnt = pSrvLocCls.FeatureCount(pQry);

                if (recCnt == 1)
                {
                    pfeat = pCurs.NextFeature();
                }
            }
            catch (Exception ex)
            {
                ClsLogReports.writeLine("EXCP@GetFeature_globalid " + ex.Message);
            }
            finally
            {
                if (pQry != null) { while (Marshal.ReleaseComObject(pQry) > 0) { } }
                if (pCurs != null) { while (Marshal.ReleaseComObject(pCurs) > 0) { } }
            }
            return pfeat;
        }

        //Function to get Service Locations in buffer distance
        private void IsPointFeatureExisting_buffer(IFeature pServiceLocFeat, IFeatureClass pSrvLocCls, IList<string> sevlocations, IFeature SerLoc)
        {
            ISpatialFilter pSptlFilter = new SpatialFilterClass();
            IFeatureCursor NodeFeatCurs = null;
            IFeature tmp = null;
            ISet pRelSet = null;
            IRow pRelRow = null;
            string strval = string.Empty;
            IList<string> StreetNum = new List<string>();
            try
            {

                IBufferConstruction pBuffConst = new BufferConstructionClass();
                IGeometry pBuffGeom = pBuffConst.Buffer(pServiceLocFeat.ShapeCopy, 15);

                pSptlFilter.Geometry = (IGeometry)pBuffGeom;
                pSptlFilter.GeometryField = pSrvLocCls.ShapeFieldName;
                pSptlFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                pSptlFilter.SubFields = "OBJECTID,CGC12,GLOBALID";
                NodeFeatCurs = pSrvLocCls.Search(pSptlFilter, true);

                string strCGColdnum = SerLoc.get_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)SerLoc.Table, "CGC12")).ToString();
                pRelSet = pSrvLoc_Pnt_relCls.GetObjectsRelatedToObject((IObject)SerLoc);
                pRelRow = (IRow)pRelSet.Next();
                while (pRelRow != null)
                {
                    strval = null;
                    strval = pRelRow.get_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pRelRow.Table, "STREETNUMBER")).ToString();
                    StreetNum.Add(strval);
                    if (pRelRow != null) { while (Marshal.ReleaseComObject(pRelRow) > 0) { } }
                    pRelRow = (IRow)pRelSet.Next();
                }

                if (pRelSet != null) { while (Marshal.ReleaseComObject(pRelSet) > 0) { } }

                int intCnt = pSrvLocCls.FeatureCount(pSptlFilter);
                if (intCnt != 0)
                {
                    while ((tmp = NodeFeatCurs.NextFeature()) != null)
                    {
                        string strCGCNewnum = tmp.get_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)tmp.Table, "CGC12")).ToString();
                        //Comparing Tranfo
                        if ((strCGColdnum == strCGCNewnum) && (SerLoc.OID.ToString() != tmp.OID.ToString()))
                        {
                            pRelSet = pSrvLoc_Pnt_relCls.GetObjectsRelatedToObject((IObject)tmp);
                            pRelRow = (IRow)pRelSet.Next();
                            int count = 0;
                            while (pRelRow != null)
                            {
                                strval = null;
                                strval = pRelRow.get_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pRelRow.Table, "STREETNUMBER")).ToString();
                                if (StreetNum.Contains(strval) == true)
                                {
                                    count++;
                                }
                                if (pRelRow != null) { while (Marshal.ReleaseComObject(pRelRow) > 0) { } }
                                pRelRow = (IRow)pRelSet.Next();
                            }

                            if (pRelSet != null) { while (Marshal.ReleaseComObject(pRelSet) > 0) { } }

                            if (count > 0)
                            {
                                sevlocations.Add(tmp.OID.ToString());
                            }
                        }
                    }
                    if (tmp != null) { while (Marshal.ReleaseComObject(tmp) > 0) { } }
                }
            }
            catch (Exception ex)
            {
                ClsLogReports.writeLine("EXCP@IsPointFeatureExisting_buffer " + ex.Message);
            }
            finally
            {
                if (NodeFeatCurs != null) { while (Marshal.ReleaseComObject(NodeFeatCurs) > 0) { } }
                if (pSptlFilter != null) { while (Marshal.ReleaseComObject(pSptlFilter) > 0) { } }
                if (pRelSet != null) { while (Marshal.ReleaseComObject(pRelSet) > 0) { } }
                if (pRelRow != null) { while (Marshal.ReleaseComObject(pRelRow) > 0) { } }
            }
        }

        //Function to delete Psuedo services after updating service points to related service locations
        private Boolean deletePsuedoService(IFeature pServiceLocFeat)
        {
            try
            {
                ISimpleJunctionFeature pJunc = (ISimpleJunctionFeature)pServiceLocFeat;
                if (pJunc.EdgeFeatureCount != 0)
                {
                    IFeature pSrvFeat = null;
                    try
                    {
                        pSrvFeat = (IFeature)pJunc.get_EdgeFeature(0);

                        string str = string.Empty;
                        clsArcGISfunctionality.getSubtypeName(pSrvFeat, ref str);

                        if (str.Contains("Pseudo"))
                        {
                            pSrvFeat.Delete();
                            return true;
                        }
                    }
                    finally
                    {
                        if (pSrvFeat != null) { while (Marshal.ReleaseComObject(pSrvFeat) > 0) { } }
                    }
                }
                else
                {
                    ClsLogReports.writeLine("Hanging Service Location. Service Location Objectid: " + pServiceLocFeat.OID);
                    return false;
                }
            }
            catch (Exception ex) { ClsLogReports.writeLine("EXCP@deletePsuedoService " + ex.Message); }
            return false;
        }

        //Function to find another service found in Grid
        private Boolean FunctiontofindSecCond(IPoint pSrvLocPnt_new)
        {
            // IFeatureClass PGridfC = null;
            IFeature pGridFeat = null;
            try
            {
                if (PGridfC != null)
                {
                    pGridFeat = getGridnamefromFeature(pSrvLocPnt_new);

                    if (pGridFeat != null)
                    {
                        if (true == GetintersetUgConductors(pGridFeat))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            finally
            {
                if (pGridFeat != null) { while (Marshal.ReleaseComObject(pGridFeat) > 0) { } }
            }
        }

        private Boolean GetintersetUgConductors(IFeature pGridFeat)
        {
            int intSerFnd = 0, intPsuFnd = 0, intStreetFnd = 0;

            try
            {
                FunctiontofincondinGrid(pGridFeat, pUGsrvCls, ref intPsuFnd, ref intSerFnd, ref intStreetFnd);

                FunctiontofincondinGrid(pGridFeat, pOHsrvCls, ref intPsuFnd, ref intSerFnd, ref intStreetFnd);

                if ((intPsuFnd > 0) && ((intSerFnd > 0) || intStreetFnd > 0))
                {
                    return true;
                }
                if (intPsuFnd > 1)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                ClsLogReports.writeLine("EXCP@Error at GetintersetUgConductors " + ex.Message);

            }
            return false;
        }

        private void FunctiontofincondinGrid(IFeature pGridFeat, IFeatureClass psrvCls, ref int intPsuFnd, ref int intSerFnd, ref int intStreetFnd)
        {
            IFeature pSrcFeat = null;
            string strSubType = null;
            int rowcount = 0;
            try
            {
                IFeatureCursor ConnectedMapCursor = null;
                IGeometry queryGeometry = pGridFeat.ShapeCopy;
                ISpatialFilter spatialFilter = new SpatialFilterClass();
                spatialFilter.Geometry = queryGeometry;
                spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                spatialFilter.SubFields = "SUBTYPECD,SHAPE";
                using (ComReleaser comReleaser = new ComReleaser())
                {
                    ConnectedMapCursor = psrvCls.Search(spatialFilter, true);
                    comReleaser.ManageLifetime(ConnectedMapCursor);
                    rowcount = psrvCls.FeatureCount(spatialFilter);

                    if ((rowcount > 0))
                    {
                        IPolyline Ppoly = null;

                        IRelationalOperator2 relation = pGridFeat.Shape as IRelationalOperator2;
                        //int count = 0;
                        while ((pSrcFeat = ConnectedMapCursor.NextFeature()) != null)
                        {
                            comReleaser.ManageLifetime(pSrcFeat);
                            Ppoly = null;
                            Ppoly = pSrcFeat.ShapeCopy as IPolyline;
                            strSubType = pSrcFeat.get_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pSrcFeat.Table, "SUBTYPECD")).ToString();

                            if ((strSubType == "6") || (strSubType == "5"))//Pseudo
                            {
                                if (psrvCls.AliasName.Contains("Overhead"))
                                {
                                    if (strSubType == "5")
                                    {
                                        if ((relation.Contains(Ppoly.FromPoint)) || (relation.Contains(Ppoly.ToPoint)))
                                        {
                                            intPsuFnd = intPsuFnd + 1;
                                        }
                                    }
                                }
                                else
                                {
                                    if (strSubType == "6")
                                    {
                                        if ((relation.Contains(Ppoly.FromPoint)) || (relation.Contains(Ppoly.ToPoint)))
                                        {
                                            intPsuFnd = intPsuFnd + 1;
                                        }
                                    }
                                }
                            }
                            else if (strSubType == "3")//Service
                            {
                                if ((relation.Contains(Ppoly.FromPoint)) || (relation.Contains(Ppoly.ToPoint)))
                                {
                                    intSerFnd = intSerFnd + 1;
                                }
                            }
                            else if (strSubType == "4")//StreetLight
                            {
                                if ((relation.Contains(Ppoly.FromPoint)) || (relation.Contains(Ppoly.ToPoint)))
                                {
                                    intStreetFnd = intStreetFnd + 1;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ClsLogReports.writeLine("EXCP@Error at FunctiontofincondinGrid " + ex.Message);
            }
        }

        private IFeature getGridnamefromFeature(IPoint pSrvLocPnt)
        {
            IFeature pSrcFeat = null, pGridFeat = null;

            try
            {
                IFeatureCursor ConnectedMapCursor = null;
                IGeometry queryGeometry = (IGeometry)pSrvLocPnt;
                ISpatialFilter spatialFilter = new SpatialFilterClass();
                spatialFilter.Geometry = queryGeometry;
                spatialFilter.SubFields = "OBJECTID,SHAPE";
                spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;

                using (ComReleaser comReleaser = new ComReleaser())
                {
                    ConnectedMapCursor = PGridfC.Search(spatialFilter, true);
                    comReleaser.ManageLifetime(ConnectedMapCursor);
                    while ((pSrcFeat = ConnectedMapCursor.NextFeature()) != null)
                    {
                        pGridFeat = pSrcFeat;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                ClsLogReports.writeLine("EXCP@Error at getGridnamefromFeature " + ex.Message);
            }
            return pGridFeat;
        }

        /// <summary>
        /// Retuens true if Service connected to point feature is of pseudo subtype
        /// </summary>
        /// <param name="pServiceLocFeat"></param>
        /// <returns></returns>
        private bool IsValidPseudoService(IFeature pServiceLocFeat)
        {
            Boolean blnStatus = false;
            string strSubtype = string.Empty;
            try
            {
                ISimpleJunctionFeature pJunc = (ISimpleJunctionFeature)pServiceLocFeat;
                if (pJunc.EdgeFeatureCount != 0)
                {
                    IFeature pSrvFeat = (IFeature)pJunc.get_EdgeFeature(0);

                    string str = string.Empty;
                    clsArcGISfunctionality.getSubtypeName(pSrvFeat, ref str);

                    if (str.Contains("Pseudo"))
                    {
                        blnStatus = true;
                    }
                }
                else
                {
                    ClsLogReports.writeLine(" Hanging Service Location. Service Location Objectid: " + pServiceLocFeat.OID);
                    blnStatus = false;
                }
            }
            catch (Exception ex)
            {
                ClsLogReports.writeLine("EXCP@IsValidPseudoService " + ex.Message + " ObjectID =" + pServiceLocFeat.OID);
            }
            return blnStatus;
        }

        private static void createPseudoService(IFeature pNewLocFeat, IFeature pTransFeat)
        {
            //get feeding transformer to new loc feat
            string strXFRcedsaID = string.Empty;
            string strXFRsubtype = string.Empty;
            IFeature pSrvFeat = null;
            string strTransFeederInfo = string.Empty;
            string strTransCirCuitID = string.Empty;
            string str_locOffId_val = string.Empty;
            string str_SerLoc_Phase_fromTrans = string.Empty;

            try
            {

                str_locOffId_val = getLocalOfficeId((IPoint)pNewLocFeat.Shape);
                str_SerLoc_Phase_fromTrans = getPhase_fromTransformer(pTransFeat);

                clsArcGISfunctionality.GetFieldValue(pNewLocFeat, "CONVTRANSFORMERID", ref strXFRcedsaID);
                IFeature pXFRfeat = clsArcGISfunctionality.getGISFeature(pXFRCls, "CEDSADEVICEID", (object)strXFRcedsaID, "INSTALLATIONTYPE,feederinfo,circuitid,SHAPE");
                //clsArcGISfunctionality.getSubtypeName(pXFRfeat, ref strXFRsubtype);
                clsArcGISfunctionality.GetFieldValue(pXFRfeat, "INSTALLATIONTYPE", ref strXFRsubtype);
                clsArcGISfunctionality.GetFieldValue(pXFRfeat, "feederinfo", ref strTransFeederInfo);
                clsArcGISfunctionality.GetFieldValue(pXFRfeat, "circuitid", ref strTransCirCuitID);



                if (strXFRsubtype.Contains("OH"))
                {
                    pSrvFeat = pOHsrvCls.CreateFeature();
                    pSrvFeat.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pSrvFeat.Table, "SUBTYPECD"), 5);
                }
                else
                {
                    pSrvFeat = pUGsrvCls.CreateFeature();
                    pSrvFeat.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pSrvFeat.Table, "SUBTYPECD"), 6);
                }
                //attributes
                pSrvFeat.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pSrvFeat.Table, "CREATIONUSER"), "INFOTECH");
                pSrvFeat.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pSrvFeat.Table, "DATECREATED"), String.Format("{0:M/d/yyyy}", DateTime.Now));

                pSrvFeat.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pSrvFeat.Table, "STATUS"), 5);
                //pSrvFeat.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pSrvFeat.Table, "PHASEDESIGNATION"), 4);
                pSrvFeat.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pSrvFeat.Table, "PHASEDESIGNATION"), str_SerLoc_Phase_fromTrans);
                pSrvFeat.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pSrvFeat.Table, "CUSTOMEROWNED"), "N");
                pSrvFeat.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pSrvFeat.Table, "PHASINGVERIFIEDSTATUS"), "Estimated/Defaulted");
                pSrvFeat.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pSrvFeat.Table, "OPERATINGVOLTAGE"), pTransFeat.get_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pTransFeat.Table, "LOWSIDEVOLTAGE")));
                //pSrvFeat.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pSrvFeat.Table, "ELECTRICTRACEWEIGHT"), "268435680");
                //pSrvFeat.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pSrvFeat.Table, "feederinfo"), "1");
                pSrvFeat.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pSrvFeat.Table, "TRACEABLE"), "0");
                pSrvFeat.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pSrvFeat.Table, "ENABLED"), "1");
                pSrvFeat.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pSrvFeat.Table, "circuitid"), strTransCirCuitID);
                pSrvFeat.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pSrvFeat.Table, "Installjobnumber"), "0");
                pSrvFeat.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pSrvFeat.Table, "LOCALOFFICEID"), str_locOffId_val);

                // string value = pTransFeat.get_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pTransFeat.Table, "LOWSIDEVOLTAGE")).ToString();


                IPolyline pln = new PolylineClass();
                pln.FromPoint = (IPoint)pXFRfeat.Shape;
                pln.ToPoint = (IPoint)pNewLocFeat.Shape;
                pSrvFeat.Shape = (IGeometry)pln;

                pSrvFeat.Store();

                if (pSrvFeat != null) { while (Marshal.ReleaseComObject(pSrvFeat) > 0) { } }
            }
            catch (Exception ex) { ClsLogReports.writeLine("EXCP@createPseudoService Transformer_oid:" + pTransFeat.OID + "  " + ex.Message); }
        }

        private static string getLocalOfficeId(IPoint pPnt)
        {
            Boolean blnStatus = false;
            string str_locOffId_val = string.Empty;
            string str_MAPTYPE_val = string.Empty;
            IFeatureCursor pFCursor = null;
            ISpatialFilter pSptlFltr = new SpatialFilterClass();
            IBufferConstruction pBuffConst = new BufferConstructionClass();
            IFeature pfeat = null;
            try
            {
                using (ESRI.ArcGIS.ADF.ComReleaser cr = new ESRI.ArcGIS.ADF.ComReleaser())
                {
                    str_locOffId_val = "";
                    pSptlFltr.Geometry = (IGeometry)pPnt;
                    pSptlFltr.GeometryField = pLOPC_FC.ShapeFieldName;
                    pSptlFltr.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                    pSptlFltr.WhereClause = "MAPTYPE IS NOT NULL order by MAPTYPE ASC";
                    pSptlFltr.SubFields = "LOCALOFFICEID,MAPTYPE";
                    //objectid is not null order by objectid
                    pFCursor = pLOPC_FC.Search(pSptlFltr, true);
                    cr.ManageLifetime(pFCursor);
                    pfeat = pFCursor.NextFeature();
                    cr.ManageLifetime(pfeat);
                    if (pfeat != null)
                    {
                        str_locOffId_val = pfeat.get_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pfeat.Table, "LOCALOFFICEID")).ToString();
                        str_MAPTYPE_val = pfeat.get_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pfeat.Table, "MAPTYPE")).ToString();
                        //pfeat = pFCursor.NextFeature();
                    }
                    return str_locOffId_val;
                }
            }
            catch (Exception ex)
            {
                ClsLogReports.writeLine("Error on getLocalOfficeId " + ex.Message);
                return str_locOffId_val;
            }
            finally
            {

            }
        }

        private static bool GetUpdateAPNnum_MN(IFeature pServiceLocFeat, ITable pNewLocTbl, string strConnCol, string strGlobalid, string strUpdColName, ref string strCommnet)
        {
            Boolean blnStatus = false;
            IQueryFilter pQry = null;
            ICursor pCurs = null;
            IRow pNewdataROw = null;
            ISet pRelSet = null;
            IRow pGisRow = null;
            string strdtVal = string.Empty;
            strCommnet = string.Empty;
            string strGDBid = string.Empty;
            string strGISid = string.Empty;
            string strStrnum = string.Empty;
            int updCnt = 0;
            IList<string> streetnum = new List<string>();
            int i = 0;

            try
            {
                //get GDB service points for the location
                pQry = new QueryFilter();
                pQry.WhereClause = strConnCol + "='" + strGlobalid + "'";
                pQry.SubFields = strUpdColName + ",SERVICEPOINTCONVID";
                pCurs = pNewLocTbl.Search(pQry, true);
                int recCnt = pNewLocTbl.RowCount(pQry);

                //get related SrvPoint record from GIS
                pRelSet = pSrvLoc_Pnt_relCls.GetObjectsRelatedToObject((IObject)pServiceLocFeat);

                pNewdataROw = (IRow)pCurs.NextRow();
                while (pNewdataROw != null)
                {
                    clsArcGISfunctionality.GetFieldValue(pNewdataROw, strUpdColName, ref strdtVal);
                    clsArcGISfunctionality.GetFieldValue(pNewdataROw, "SERVICEPOINTCONVID", ref strGDBid);

                    pRelSet.Reset();
                    pGisRow = (IRow)pRelSet.Next();
                    while (pGisRow != null)
                    {
                        clsArcGISfunctionality.GetFieldValue(pGisRow, "CONVERSIONID", ref strGISid);
                        clsArcGISfunctionality.GetFieldValue(pGisRow, "STREETNUMBER", ref strStrnum);

                        if (i == 0)
                        {
                            streetnum.Add(strStrnum);
                        }
                        else
                        {
                            if (streetnum.Contains(strStrnum) == false)
                            {
                                streetnum.Clear();
                                return blnStatus;
                            }
                        }
                        i++;
                        //IRow pSrvPntRow = (IRow)pRelSet.Next();
                        if (strGISid == strGDBid)
                        {
                            pGisRow.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pGisRow.Table, "APNNUM"), strdtVal);
                            pGisRow.Store();
                            updCnt++;
                            break;
                        }
                        pGisRow = (IRow)pRelSet.Next();
                    }
                    pNewdataROw = (IRow)pCurs.NextRow();
                }

                pRelSet.Reset();
                pGisRow = (IRow)pRelSet.Next();
                while (pGisRow != null)
                {
                    while (Marshal.ReleaseComObject(pGisRow) > 0) { }
                    pGisRow = (IRow)pRelSet.Next();
                }

                if (recCnt == pRelSet.Count && updCnt == recCnt)
                {
                    blnStatus = true;
                }
            }
            catch (Exception ex) { ClsLogReports.writeLine("EXCP@GetUpdateAPNnum_MN " + ex.Message); }
            finally
            {
                clsArcGISfunctionality.ReleaseCOM(pCurs);
                clsArcGISfunctionality.ReleaseCOM(pQry);
                if (pNewdataROw != null) { while (Marshal.ReleaseComObject(pNewdataROw) > 0) { } }
                if (pRelSet != null) { while (Marshal.ReleaseComObject(pRelSet) > 0) { } }
                if (pGisRow != null) { while (Marshal.ReleaseComObject(pGisRow) > 0) { } }
            }
            return blnStatus;
        }

        private static bool GetUpdateAPNnum(IFeature pServiceLocFeat, ITable pNewLocTbl, string strConnCol, string strGlobalID, string strUpdColName, ref string strCommnet, object intOid_Exists)
        {
            Boolean blnStatus = false;
            IQueryFilter pQry = null;
            ICursor pCurs = null;
            IRow pNewdataROw = null;
            string strdtVal = string.Empty;
            strCommnet = string.Empty;
            try
            {
                pQry = new QueryFilter();
                pQry.WhereClause = strConnCol + "= '" + strGlobalID + "'";
                pQry.SubFields = strUpdColName;
                pCurs = pNewLocTbl.Search(pQry, true);
                int recCnt = pNewLocTbl.RowCount(pQry);
                if (recCnt > 1)
                {
                    strCommnet = "More Related Service Point(s) in ReGeocode data";
                    return false;
                }

                pNewdataROw = (IRow)pCurs.NextRow();

                clsArcGISfunctionality.GetFieldValue(pNewdataROw, strUpdColName, ref strdtVal);

                //get related SrvPoint record from GIS
                ISet pRelSet = pSrvLoc_Pnt_relCls.GetObjectsRelatedToObject((IObject)pServiceLocFeat);

                if (pRelSet != null && pRelSet.Count != 0)
                {
                    if (pRelSet.Count == 1)
                    {
                        IRow pSrvPntRow = (IRow)pRelSet.Next();
                        pSrvPntRow.set_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pSrvPntRow.Table, "APNNUM"), strdtVal);
                        pSrvPntRow.Store();
                        if (pSrvPntRow != null)
                        { while (Marshal.ReleaseComObject(pSrvPntRow) > 0) { } }
                        blnStatus = true;
                        strCommnet = "APNNUM: " + strdtVal + " Updated";
                    }
                    else if (intOid_Exists != null)
                    {
                        blnStatus = true;
                    }
                    else if (pRelSet.Count > 1)
                    {
                        strCommnet = "(" + pRelSet.Count + ") More Related Service Point(s) in GIS data";
                        return false;
                    }

                }

            }
            catch (Exception ex) { ClsLogReports.writeLine("EXCP@GetUpdateAPNnum " + ex.Message); }
            finally
            {
                clsArcGISfunctionality.ReleaseCOM(pCurs);
                clsArcGISfunctionality.ReleaseCOM(pQry);
                if (pNewdataROw != null) { while (Marshal.ReleaseComObject(pNewdataROw) > 0) { } }
            }
            return blnStatus;
        }

        /// <summary>
        /// validates if location feature exists at new location xy
        /// </summary>
        /// <param name="pNewLocTbl"></param>
        /// <param name="strColName"></param>
        /// <param name="strColVal"></param>
        /// <param name="pSrvLocPnt_new"></param>
        /// <param name="intOid"></param>
        /// <returns></returns>
        private static bool GetNewLocation(ITable pNewLocTbl, string strColName, string strColVal, ref IPoint pSrvLocPnt_new, ref object intOid)
        {
            Boolean blnStatus = false;
            IQueryFilter pQry = new QueryFilter();
            ICursor pCurs = null;
            IRow pRow = null;
            string NEW_XCORD = string.Empty;
            string NEW_YCORD = string.Empty;
            intOid = 0;
            try
            {
                pQry.WhereClause = strColName + "= '" + strColVal + "'";
                pQry.SubFields = "NEW_XCORD,NEW_YCORD";
                pCurs = pNewLocTbl.Search(pQry, true);

                //if (intRowCnt > 1 || intRowCnt == 0)
                //return blnStatus;

                pRow = (IRow)pCurs.NextRow();

                clsArcGISfunctionality.GetFieldValue(pRow, "NEW_XCORD", ref NEW_XCORD);
                clsArcGISfunctionality.GetFieldValue(pRow, "NEW_YCORD", ref NEW_YCORD);

                pSrvLocPnt_new.PutCoords(Convert.ToDouble(NEW_XCORD), Convert.ToDouble(NEW_YCORD));

                if (pSrvLocPnt_new != null)
                {
                    //returns true if feature exists
                    if (true == clsArcGISfunctionality.IsPointFeatureExisting(pSrvLocPnt_new, pSrvLocCls, ref intOid))
                        blnStatus = true;
                }
            }
            catch (Exception ex) { ClsLogReports.writeLine("EXCP@GetNewLocation " + ex.Message); }
            finally
            {
                clsArcGISfunctionality.ReleaseCOM(pCurs);
                clsArcGISfunctionality.ReleaseCOM(pQry);
                if (pRow != null) { while (Marshal.ReleaseComObject(pRow) > 0) { } }
            }
            return blnStatus;
        }

        private static bool GetNewLocation_MN(ITable pNewLocTbl, string strColName, string strColVal, ref IPoint pSrvLocPnt_new, ref object intOid)
        {
            Boolean blnStatus = false;
            IQueryFilter pQry = new QueryFilter();
            ICursor pCurs = null;
            IRow pRow = null;
            string NEW_XCORD = string.Empty;
            string NEW_YCORD = string.Empty;
            intOid = 0;
            try
            {
                pQry.WhereClause = strColName + "='" + strColVal + "'";
                pQry.SubFields = "NEW_XCORD,NEW_YCORD";
                pCurs = pNewLocTbl.Search(pQry, true);

                //if (intRowCnt > 1 || intRowCnt == 0)
                //return blnStatus;

                pRow = (IRow)pCurs.NextRow();

                clsArcGISfunctionality.GetFieldValue(pRow, "NEW_XCORD", ref NEW_XCORD);
                clsArcGISfunctionality.GetFieldValue(pRow, "NEW_YCORD", ref NEW_YCORD);

                pSrvLocPnt_new.PutCoords(Convert.ToDouble(NEW_XCORD), Convert.ToDouble(NEW_YCORD));

                if (pSrvLocPnt_new != null)
                {
                    //returns true if feature exists
                    if (true == clsArcGISfunctionality.IsPointFeatureExisting(pSrvLocPnt_new, pSrvLocCls, ref intOid))
                        blnStatus = true;
                }
            }
            catch (Exception ex) { ClsLogReports.writeLine("EXCP@GetNewLocation_MN " + ex.Message); }
            finally
            {
                clsArcGISfunctionality.ReleaseCOM(pCurs);
                clsArcGISfunctionality.ReleaseCOM(pQry);
                if (pRow != null) { while (Marshal.ReleaseComObject(pRow) > 0) { } }
            }
            return blnStatus;
        }

        //for SDE connection
        private void btnSDEConn_Click(object sender, EventArgs e)
        {
            try
            {
                IWorkspace pWS = null;
                string strsdepath = "";
                clsArcGISfunctionality.GetSDEconnectionfromcatalog(ref strsdepath);
                if (strsdepath == "" || (strsdepath.ToUpper().EndsWith(".SDE".ToUpper()) == false))
                {
                    MessageBox.Show("Select SDE Connection properly", "EnableAnno", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                //txtSrcDBPath.Text = strsdepath;
                IWorkspaceFactory pWSFactory = new SdeWorkspaceFactoryClass();
                pWS = pWSFactory.OpenFromFile(strsdepath, 0);

                if (pWS == null)
                {
                    MessageBox.Show("Error in getting WS", "EnableAnno", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                //validate for versioned SDE conn selected <> default version
                string strDbDetails = string.Empty;
                if (true == clsArcGISfunctionality.GetVersionName(pWS, ref strDbDetails))
                {
                    btnExecute.Enabled = true;
                }
                else
                {
                    MessageBox.Show("Unable to get SDE access | Improper Version Selected .." + strDbDetails.Split(',')[1]);
                    btnExecute.Enabled = false;
                    return;
                }
                clsCommonVariables.pWrkSpace = pWS;
                clsCommonVariables.pFeatWrkSpace = (IFeatureWorkspace)pWS;
            }
            catch (Exception ex)
            {
                ClsLogReports.writeLine("EXCP@btnSDEConn_Click " + ex.Message);

            }
        }

        private static IWorkspace FileGdbWorkspaceFromPath(String path)
        {
            try
            {
                IAoInitialize aoInit = new AoInitializeClass();
                aoInit.Initialize(esriLicenseProductCode.esriLicenseProductCodeAdvanced);
                IWorkspaceFactory workspaceFactory = new FileGDBWorkspaceFactoryClass();
                return workspaceFactory.OpenFromFile(path, 0);
            }
            catch (Exception ex)
            {
                ClsLogReports.writeLine("EXCP@FileGdbWorkspaceFromPath " + ex.Message);
                return null;
            }
        }

        //function to split total objectID count in to parts to run the tool
        private void cmbAllFC_MouseClick(object sender, EventArgs e)
        {
            ITable pFC = null;
            ICursor pFCursor = null;
            IQueryFilter pFilter = new QueryFilterClass();
            int intRecCount = 0;
            int intMin = 0;
            int intMax = 0;
            int iterationCount = 0;
            int inOid = 0;
            List<string> lstString = new List<string>();

            try
            {
                if (cmbAllFC.SelectedItem == null)
                {
                    // cmbWhereClause.Enabled = true;
                    lblMsg.Text = "Processing wait... adding Where Clause..it take couple of mints ";
                    Application.DoEvents();

                    //  btnRun.Enabled = false;
                    //  btnClose.Enabled = false;
                    pCedsaFws = null;
                    pDesaWS2 = null;

                    //get ini data
                    string strGdbFile = INIdefinitions.GetEntryValue("GDBFile", "File", clsCommonVariables.strINIpath);

                    pCedsaWsp = FileGdbWorkspaceFromPath(clsCommonVariables.strLogsPath + "\\" + strGdbFile);

                    if (pCedsaWsp == null)
                    {
                        MessageBox.Show("Unable to get Workspace from " + clsCommonVariables.strLogsPath + "\\" + strGdbFile);
                        ClsLogReports.writeLine("Unable to get Workspace from " + clsCommonVariables.strLogsPath + "\\" + strGdbFile);
                        return;
                    }
                    pDesaWS2 = (IWorkspace2)pCedsaWsp;
                    pCedsaFws = (IFeatureWorkspace)pCedsaWsp;

                    pFC = pCedsaFws.OpenTable("UPD_REGEO_SERVPTREL");
                    string OIDfieldName = pFC.OIDFieldName;

                    // pFC = ClsGLobalFunctions.getFeatureclassByName(clsCommonVariables.pWrkSpace, "EDGIS.Transformer");
                    intRecCount = pFC.RowCount(null);

                    //pFilter.WhereClause = OIDfieldName + " is not null order by TRANS_CEDSAID" ;
                    pFilter.WhereClause = OIDfieldName + " is not null order by " + OIDfieldName;
                    // pFilter.WhereClause = "Objectid is not null order by objectid ";
                    pFCursor = pFC.Search(pFilter, false);

                    IDataStatistics dataStatistics = new DataStatisticsClass();
                    //dataStatistics.Field = "Objectid";
                    dataStatistics.Field = OIDfieldName;
                    dataStatistics.Cursor = (ICursor)pFCursor;
                    System.Collections.IEnumerator enumerator = dataStatistics.UniqueValues;
                    enumerator.Reset();

                    while (enumerator.MoveNext())
                    {
                        object myObject = enumerator.Current;
                        //lstUniqueFldVals.Add(myObject.ToString());
                        inOid = int.Parse(myObject.ToString());

                        if (iterationCount == 0)
                        {
                            intMin = inOid;
                        }
                        else if (iterationCount == 2000)
                        {
                            intMax = inOid;
                        }
                        iterationCount++;

                        if (iterationCount > 2000)
                        {
                            lstString.Add(OIDfieldName + " > " + intMin + " and " + OIDfieldName + " < " + intMax);
                            cmbAllFC.Items.Add(OIDfieldName + " > " + intMin + " and " + OIDfieldName + " < " + intMax);
                            iterationCount = 0;
                        }
                    }

                    if (iterationCount > 0 && iterationCount < 2000)
                    {
                        lstString.Add(OIDfieldName + " > " + intMin + " and " + OIDfieldName + " < " + inOid);
                        cmbAllFC.Items.Add(OIDfieldName + " > " + intMin + " and " + OIDfieldName + " < " + inOid);
                    }
                    cmbAllFC.Text = lstString[0];

                    lblMsg.Text = "Select WhereClause and click Geo Code button";
                    Application.DoEvents();
                    //btnRun.Enabled = true;
                    // btnClose.Enabled = true;
                }
                else
                {
                }
            }
            catch (Exception ex)
            {
                ClsLogReports.writeLine("EXCP@cmbAllFC_MouseClick " + ex.Message);
            }
            finally
            {
                if (pFCursor != null)
                {
                    while (Marshal.ReleaseComObject(pFCursor) > 0) { }
                }
            }
        }



        private IFeature getGridnamefromFeature_SerLoc(IFeature pSrvLocPnt)
        {
            IFeature pSrcFeat = null, pGridFeat = null;

            try
            {
                IFeatureCursor ConnectedMapCursor = null;
                IGeometry queryGeometry = pSrvLocPnt.ShapeCopy;
                ISpatialFilter spatialFilter = new SpatialFilterClass();
                spatialFilter.Geometry = queryGeometry;
                spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;

                using (ComReleaser comReleaser = new ComReleaser())
                {
                    ConnectedMapCursor = PGridfC.Search(spatialFilter, false);
                    comReleaser.ManageLifetime(ConnectedMapCursor);
                    int rowcount = PGridfC.FeatureCount(spatialFilter);
                    if (rowcount > 0)
                    {
                        //int count = 0;
                        while ((pSrcFeat = ConnectedMapCursor.NextFeature()) != null)
                        {
                            pGridFeat = pSrcFeat;
                        }
                    }
                    if (ConnectedMapCursor != null)
                    {
                        Marshal.ReleaseComObject(ConnectedMapCursor);
                        ConnectedMapCursor = null;
                    }
                }

            }
            catch (Exception ex)
            {

                ClsLogReports.writeLine("EXCP@Error at getGridnamefromFeature " + ex.Message);
                // Logs.writeLine("Error at getGridnamefromFeature. Processing " + pSrcFeat.Class.AliasName + pSrcFeat.OID + "  " + ex.Message);
            }
            return pGridFeat;


        }

        private void cmbAllFC_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void rad1_1_CheckedChanged(object sender, EventArgs e)
        {
            cmbAllFC.Enabled = true;
        }

        private void rad1_MMode_CheckedChanged(object sender, EventArgs e)
        {
            cmbAllFC.Enabled = false;
        }

    }
}
