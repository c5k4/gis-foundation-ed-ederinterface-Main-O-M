using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;
using PGE.Common.Delivery.Diagnostics;
using System.Reflection;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using PGE.Common.Delivery.Framework;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.esriSystem;
using System.Collections;
using System.Configuration;
using Miner.Interop;
using ESRI.ArcGIS.Editor;

namespace PGE.Desktop.EDER.PLC
{
    public partial class FrmCreatePole : Form
    {
        #region Initialize Variables
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        public IMxDocument pMxDoc;
        public IApplication _app;
        public IGeometry gSupportStructure;
        public IFeatureClass gfcSupportStructure;
        static IFeatureSelection gpfeatSelect;
        static String SLookupTable = "EDGIS.PGE_PLC_CONFIG";

        IGraphicsLayer gobjGraphicLayer;
        IGraphicsContainer gpGraphicCont;
        String gstrJobNumber = String.Empty;
        //public Hashtable hshPoleObjid = new Hashtable();
        //public IWorkspaceEdit gSpaceEdit;
        IFeatureWorkspace giFeatWs;
        IRow codeRow = null;
        IRow PLDBStatusRow = null;

        IEditor _editor = null;
        # endregion

        # region Constructor
        public FrmCreatePole(IApplication frmApp, IWorkspaceEdit wSpaceEdit, string sJobNumber, out int rCnt)
        {
            try
            {
                InitializeComponent();

                initializeGlobalVar(frmApp, wSpaceEdit, sJobNumber);

                //Define Editor
                if (_editor == null)
                {
                    Type t = Type.GetTypeFromProgID("esriFramework.AppRef");
                    object o = Activator.CreateInstance(t);
                    frmApp = o as IApplication;
                    IMxDocument mxDocument = frmApp.Document as IMxDocument;
                    _editor = frmApp.FindExtensionByName("Esri Object Editor") as IEditor;
                }
                //Find RowCount for New Pole
                rCnt = 0;
                rCnt = fillDataGrid();

            }
            catch (Exception ex)
            {

                _logger.Error("PG&E PLC Create pole , Error in FrmCreatePole Function ", ex);
                throw ex;
            }

        }

        public void initializeGlobalVar(IApplication frmApp, IWorkspaceEdit wSpaceEdit, string sJobNumber)
        {
            QueryFilter queryFilter = null;
            try
            {
                _app = frmApp;
                pMxDoc = (IMxDocument)frmApp.Document;
                giFeatWs = (IFeatureWorkspace)wSpaceEdit;
                //gSpaceEdit = wSpaceEdit;

                //reading configuration table
                ITable tPLCLookup = new MMTableUtilsClass().OpenTable(SLookupTable, giFeatWs);

                if (tPLCLookup == null)
                    throw new Exception("Failed to load table " + SLookupTable);

                ICursor ICur = null;

                queryFilter = new QueryFilterClass();
                queryFilter.WhereClause = string.Format("ClassData='{0}'", "CREATE_NEW_POLE");
                ICur = tPLCLookup.Search(queryFilter, false);

                if (ICur == null) throw new Exception("Configuration Data not found for CREATE_POLE in " + SLookupTable + " table");
                codeRow = ICur.NextRow();
                // Added code get Status from lookup table 	
                PLDBStatusRow = GetConfigTableRowForNewExceptionTool(tPLCLookup);
                // end code 
                //Open Supprort structure feauture class
                IFeatureLayer featureLayerSS = new FeatureLayerClass();
                gfcSupportStructure = giFeatWs.OpenFeatureClass((codeRow.get_Value(codeRow.Fields.FindField("SupportStructure_TblName"))).ToString());
                featureLayerSS.FeatureClass = gfcSupportStructure;
                gpfeatSelect = featureLayerSS as IFeatureSelection;

                //Create graohic layer for display selected point on map
                ICompositeGraphicsLayer CGL = (ICompositeGraphicsLayer)pMxDoc.FocusMap.ActiveGraphicsLayer;
                gobjGraphicLayer = FindGraphicsLayer(ref CGL, "SupportStructureGL", _app);
                gpGraphicCont = (IGraphicsContainer)gobjGraphicLayer;

                gstrJobNumber = sJobNumber;
            }
            catch (Exception ex)
            {
                _logger.Error("PG&E PLC Create pole , Error in Initializing variables ", ex);
                throw ex;
            }
            Marshal.ReleaseComObject(queryFilter);
        }

        private int fillDataGrid()
        {
            IWorkspace wSpace = null;
            IFeatureWorkspace fWSpace = null;
            IQueryFilter qfilter = new QueryFilterClass();
            IFeatureCursor featCur = null;
            IFeatureClass fcPLD_Info = null;



            try
            {
                ILayer WIPLayer = null;
                if (!FindLayerByName("Wip", out WIPLayer))
                {
                    throw new COMException("WIP layer not present on Map.");
                }
                //Open wip database
                IFeatureClass wipFClass = ((IFeatureLayer)WIPLayer).FeatureClass;
                IDataset wipDSet = (IDataset)wipFClass;
                wSpace = wipDSet.Workspace;
                fWSpace = (IFeatureWorkspace)wSpace;
                fcPLD_Info = fWSpace.OpenFeatureClass((codeRow.get_Value(codeRow.Fields.FindField("Wip_tableName"))).ToString());

                // hshPoleObjid.Clear();
                String sJobNumber = gstrJobNumber;

                //Create Pole DataTable/DataGridView
                DataTable dt = new DataTable();
                dt.Columns.Add((codeRow.get_Value(codeRow.Fields.FindField("Dlg_JOBNUMBER_ColName"))).ToString());
                dt.Columns.Add((codeRow.get_Value(codeRow.Fields.FindField("Dlg_SKETCHLOCATION_ColName"))).ToString());
                dt.Columns.Add((codeRow.get_Value(codeRow.Fields.FindField("Dlg_PLDBID_ColName"))).ToString());
                dt.Columns.Add((codeRow.get_Value(codeRow.Fields.FindField("Dlg_LONGITUDE_ColName"))).ToString());
                dt.Columns.Add((codeRow.get_Value(codeRow.Fields.FindField("Dlg_LATITUDE_ColName"))).ToString());
                dt.Columns.Add((codeRow.get_Value(codeRow.Fields.FindField("Dlg_POLECLASS_ColName"))).ToString());
                dt.Columns.Add((codeRow.get_Value(codeRow.Fields.FindField("Dlg_POLEHEIGHT_ColName"))).ToString());
                dt.Columns.Add((PLDBStatusRow.get_Value(PLDBStatusRow.Fields.FindField("STATUS_COLNAME"))).ToString());

                //dt.Columns.Add((codeRow.get_Value(codeRow.Fields.FindField("Dlg_SPECIES_ColName"))).ToString());

                //create Query filter Subfields
                qfilter.SubFields = (codeRow.get_Value(codeRow.Fields.FindField("wip_OBJECTID_ColName"))).ToString() + "," +
                (codeRow.get_Value(codeRow.Fields.FindField("wip_SHAPE_ColName"))).ToString() + "," +
                (codeRow.get_Value(codeRow.Fields.FindField("wip_ORDER_NUMBER_ColName"))).ToString() + "," +
                (codeRow.get_Value(codeRow.Fields.FindField("wip_SKETCH_LOC_ColName"))).ToString() + "," +
                (codeRow.get_Value(codeRow.Fields.FindField("wip_PLDBID_ColName"))).ToString() + "," +
                (codeRow.get_Value(codeRow.Fields.FindField("wip_LONGITUDE_ColName"))).ToString() + "," +
                (codeRow.get_Value(codeRow.Fields.FindField("wip_LATITUDE_ColName"))).ToString() + "," +
                (codeRow.get_Value(codeRow.Fields.FindField("wip_CLASS_ColName"))).ToString() + "," +
                (codeRow.get_Value(codeRow.Fields.FindField("wip_LENGTHININCHES_ColName"))).ToString() + "," +
                (codeRow.get_Value(codeRow.Fields.FindField("WIP_STATUS_COLNAME"))).ToString();

                //+ "," + (codeRow.get_Value(codeRow.Fields.FindField("wip_SPECIES_ColName"))).ToString();

                //Create QueryFilter WhereCluse
                qfilter.WhereClause = " " + (codeRow.get_Value(codeRow.Fields.FindField("wip_ORDER_NUMBER_ColName"))).ToString() + " = '" + gstrJobNumber + "' AND " +
            (codeRow.get_Value(codeRow.Fields.FindField("wip_PLDBID_ColName"))).ToString() + " IS NOT NULL AND (" +
            (codeRow.get_Value(codeRow.Fields.FindField("wip_SAPEQUIPID_ColName"))).ToString() + " IS NULL OR " +
            (codeRow.get_Value(codeRow.Fields.FindField("wip_SAPEQUIPID_ColName"))).ToString() + " = 0) AND ( " +
             (PLDBStatusRow.get_Value(codeRow.Fields.FindField("wip_STATUS_COLNAME"))).ToString() + ")";


                featCur = fcPLD_Info.Search(qfilter, true);

                IFeature row = null;
                while ((row = featCur.NextFeature()) != null)
                {
                    //Add row in DataTable/DataGridview
                    IPoint pp = (IPoint)row.ShapeCopy;
                    dt.Rows.Add(row.get_Value(row.Fields.FindField((codeRow.get_Value(codeRow.Fields.FindField("wip_ORDER_NUMBER_ColName"))).ToString())).ToString(),
                        row.get_Value(row.Fields.FindField((codeRow.get_Value(codeRow.Fields.FindField("wip_SKETCH_LOC_ColName"))).ToString())).ToString(),
                        row.get_Value(row.Fields.FindField((codeRow.get_Value(codeRow.Fields.FindField("wip_PLDBID_ColName"))).ToString())).ToString(),
                        row.get_Value(row.Fields.FindField((codeRow.get_Value(codeRow.Fields.FindField("wip_LONGITUDE_ColName"))).ToString())).ToString(),
                        row.get_Value(row.Fields.FindField((codeRow.get_Value(codeRow.Fields.FindField("wip_LATITUDE_ColName"))).ToString())).ToString(),
                        row.get_Value(row.Fields.FindField((codeRow.get_Value(codeRow.Fields.FindField("wip_CLASS_ColName"))).ToString())).ToString(),
                        row.get_Value(row.Fields.FindField((codeRow.get_Value(codeRow.Fields.FindField("wip_LENGTHININCHES_ColName"))).ToString())).ToString(),
                        row.get_Value(row.Fields.FindField((codeRow.get_Value(codeRow.Fields.FindField("WIP_STATUS_COLNAME"))).ToString())).ToString());

                    //, row.get_Value(row.Fields.FindField((codeRow.get_Value(codeRow.Fields.FindField("wip_SPECIES_ColName"))).ToString())).ToString());

                    //IPoint pp = (IPoint)row.ShapeCopy;

                    //hshPoleObjid.Add(row.get_Value(row.Fields.FindField((codeRow.get_Value(codeRow.Fields.FindField("wip_PLDBID_ColName"))).ToString())).ToString() + "_X", pp.X);
                    //hshPoleObjid.Add(row.get_Value(row.Fields.FindField((codeRow.get_Value(codeRow.Fields.FindField("wip_PLDBID_ColName"))).ToString())).ToString() + "_Y", pp.Y);
                    //hshPoleObjid.Add(row.get_Value(row.Fields.FindField((codeRow.get_Value(codeRow.Fields.FindField("wip_PLDBID_ColName"))).ToString())).ToString() + "_OBJ", row.get_Value(row.Fields.FindField((codeRow.get_Value(codeRow.Fields.FindField("wip_ORDER_NUMBER_ColName"))).ToString())).ToString());
                }


                if (dt.Rows.Count > 0)
                {
                    dgvCreatePole.DataSource = dt;
                    dgvCreatePole.ClearSelection();

                }

                if (dt.Rows.Count == 0)
                {
                    System.Windows.Forms.MessageBox.Show("No pole is present for given JobNumber", "PG&E PLC Create Pole", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                return dt.Rows.Count;

            }
            catch (Exception ex)
            {
                _logger.Error("PG&E PLC Create pole , Fetching data from PLD_INFO table ", ex);
                throw ex;
            }
            finally
            {
                //Fix-ReqID-EDGISREARC-379(Exception new pole tool disables ArcFM locator  Priority)
                //Fix By -YXA6
                //if (wSpace != null) { Marshal.ReleaseComObject(wSpace); }
                //if (fWSpace != null) { Marshal.ReleaseComObject(fWSpace); }
                if (qfilter != null) { Marshal.ReleaseComObject(qfilter); }
                if (featCur != null) { Marshal.ReleaseComObject(featCur); }
            }

        }



        # endregion


        #region Buttons Method

        /// <summary>Zoom all the pole listed in the dialog box </summary>
        /// <param name="feat">esri feature</param>
        /// <param name="FeatClass">feature class to serch</param>
        /// <returns>return polygon feature</returns>
        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvCreatePole.Rows.Count > 0)
                {
                    if (dgvCreatePole.Rows.Count != dgvCreatePole.SelectedRows.Count)
                    {
                        IGeometryCollection m_pGeoColl = new GeometryBagClass();
                        IGeometryBag geoBag = new GeometryBagClass();

                        gpGraphicCont.DeleteAllElements();

                        dgvCreatePole.SelectAll();

                        int iCnt = dgvCreatePole.SelectedRows.Count;

                        for (int i = 0; i < iCnt; i++)
                        {
                            AddGraphicToMap(getGeom(i));
                            m_pGeoColl.AddGeometry(gSupportStructure);
                        }

                        geoBag = (IGeometryBag)m_pGeoColl;
                        //Change After Go-Live
                        ZoomOnGraphicElement(geoBag, 0);

                        dgvCreatePole.SelectAll();

                        pMxDoc.ActivatedView.Refresh();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("PG&E PLC Create pole , selecting all feature in SupportStructure Class by select button", ex);
            }
        }

        private void SelectAll()
        {
            try
            {
                if (dgvCreatePole.Rows.Count > 0)
                {
                    if (dgvCreatePole.Rows.Count == dgvCreatePole.SelectedRows.Count)
                    {
                        IGeometryCollection m_pGeoColl = new GeometryBagClass();
                        IGeometryBag geoBag = new GeometryBagClass();

                        gpGraphicCont.DeleteAllElements();

                        dgvCreatePole.SelectAll();

                        int iCnt = dgvCreatePole.SelectedRows.Count;

                        for (int i = 0; i < iCnt; i++)
                        {
                            AddGraphicToMap(getGeom(i));
                            m_pGeoColl.AddGeometry(gSupportStructure);
                        }

                        geoBag = (IGeometryBag)m_pGeoColl;
                        //Change After Go-Live
                        ZoomOnGraphicElement(geoBag, 0);

                        dgvCreatePole.SelectAll();

                        pMxDoc.ActivatedView.Refresh();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void btnDeSelectAll_Click(object sender, EventArgs e)
        {
            try
            {

                gpGraphicCont.DeleteAllElements();


                if (dgvCreatePole.SelectedRows.Count > 0)
                {
                    dgvCreatePole.ClearSelection();
                    pMxDoc.ActivatedView.Refresh();
                }



            }
            catch (Exception ex)
            {
                _logger.Error("PG&E PLC Create pole , Error in deselect features ", ex);
            }
        }

        /// <summary>Create new pole </summary>
        /// <param name="feat">esri feature</param>
        /// <param name="polyFeatClass">feature class to serch</param>
        private void btnMtE_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvCreatePole.SelectedRows.Count == 1)
                {
                    if (gSupportStructure != null)
                    {
                        //if (!gSpaceEdit.IsBeingEdited())
                        //    return;
                        //gSpaceEdit.StartEditOperation();

                        _editor.StartOperation();

                        gpGraphicCont.DeleteAllElements();
                        pMxDoc.ActivatedView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);

                        gpfeatSelect.Clear();
                        IFeature newFeature = gfcSupportStructure.CreateFeature();

                        //set attribute to the point feature
                        newFeature.Shape = gSupportStructure;
                        Double XCoord = Convert.ToDouble(dgvCreatePole.SelectedRows[0].Cells[(codeRow.get_Value(codeRow.Fields.FindField("Dlg_LONGITUDE_ColName"))).ToString()].Value.ToString());
                        Double YCoord = Convert.ToDouble(dgvCreatePole.SelectedRows[0].Cells[(codeRow.get_Value(codeRow.Fields.FindField("Dlg_LATITUDE_ColName"))).ToString()].Value.ToString());
                        newFeature.set_Value(newFeature.Fields.FindField((codeRow.get_Value(codeRow.Fields.FindField("InstallJobNumber_ColName"))).ToString()), dgvCreatePole.SelectedRows[0].Cells[(codeRow.get_Value(codeRow.Fields.FindField("Dlg_JOBNUMBER_ColName"))).ToString()].Value.ToString());
                        if (!String.IsNullOrEmpty((dgvCreatePole.SelectedRows[0].Cells[(codeRow.get_Value(codeRow.Fields.FindField("Dlg_PLDBID_ColName"))).ToString()].Value).ToString()))
                            newFeature.set_Value(newFeature.Fields.FindField((codeRow.get_Value(codeRow.Fields.FindField("Pldbid_ColName"))).ToString()), Convert.ToDouble(dgvCreatePole.SelectedRows[0].Cells[(codeRow.get_Value(codeRow.Fields.FindField("Dlg_PLDBID_ColName"))).ToString()].Value));
                        if (!String.IsNullOrEmpty((dgvCreatePole.SelectedRows[0].Cells[(codeRow.get_Value(codeRow.Fields.FindField("Dlg_POLEHEIGHT_ColName"))).ToString()].Value).ToString()))
                            newFeature.set_Value(newFeature.Fields.FindField((codeRow.get_Value(codeRow.Fields.FindField("Height_ColName"))).ToString()), dgvCreatePole.SelectedRows[0].Cells[(codeRow.get_Value(codeRow.Fields.FindField("Dlg_POLEHEIGHT_ColName"))).ToString()].Value.ToString());
                        if (!String.IsNullOrEmpty((dgvCreatePole.SelectedRows[0].Cells[(codeRow.get_Value(codeRow.Fields.FindField("Dlg_POLECLASS_ColName"))).ToString()].Value).ToString()))
                            newFeature.set_Value(newFeature.Fields.FindField((codeRow.get_Value(codeRow.Fields.FindField("Class_ColName"))).ToString()), dgvCreatePole.SelectedRows[0].Cells[(codeRow.get_Value(codeRow.Fields.FindField("Dlg_POLECLASS_ColName"))).ToString()].Value.ToString());
                        //if (!String.IsNullOrEmpty((dgvCreatePole.SelectedRows[0].Cells[(codeRow.get_Value(codeRow.Fields.FindField("Dlg_SPECIES_ColName"))).ToString()].Value).ToString()))
                        //{
                        //    String strDesc = (dgvCreatePole.SelectedRows[0].Cells[(codeRow.get_Value(codeRow.Fields.FindField("Dlg_SPECIES_ColName"))).ToString()].Value).ToString();
                        //    String strSpecies = String.Empty;
                        //    int fieldIx = newFeature.Fields.FindField((codeRow.get_Value(codeRow.Fields.FindField("Species_ColName"))).ToString());
                        //    object objcurrGridID = GetDescFromDomain(newFeature, newFeature.Fields.get_Field(fieldIx), strDesc);

                        //    if (objcurrGridID != null)
                        //    {
                        //        strSpecies = objcurrGridID.ToString();
                        //        newFeature.set_Value(newFeature.Fields.FindField((codeRow.get_Value(codeRow.Fields.FindField("Species_ColName"))).ToString()), objcurrGridID.ToString());
                        //    }
                        //}
                        newFeature.set_Value(newFeature.Fields.FindField((codeRow.get_Value(codeRow.Fields.FindField("GpsLongitude_ColName"))).ToString()), XCoord);
                        newFeature.set_Value(newFeature.Fields.FindField((codeRow.get_Value(codeRow.Fields.FindField("GpsLatitude_ColName"))).ToString()), YCoord);

                        newFeature.set_Value(newFeature.Fields.FindField((codeRow.get_Value(codeRow.Fields.FindField("subtypecd_ColName"))).ToString()), 1);
                        newFeature.set_Value(newFeature.Fields.FindField((codeRow.get_Value(codeRow.Fields.FindField("Datecreated_ColName"))).ToString()), System.DateTime.Now.ToString());

                        newFeature.set_Value(newFeature.Fields.FindField((codeRow.get_Value(codeRow.Fields.FindField("CustomerOwner_ColName"))).ToString()), "N");
                        newFeature.set_Value(newFeature.Fields.FindField((codeRow.get_Value(codeRow.Fields.FindField("Status_ColName"))).ToString()), (codeRow.get_Value(codeRow.Fields.FindField("STATUS_DEFAULT_VAL"))).ToString());

                        newFeature.set_Value(newFeature.Fields.FindField((codeRow.get_Value(codeRow.Fields.FindField("INSTALLJOBPREFIX_ColName"))).ToString()), (codeRow.get_Value(codeRow.Fields.FindField("INSTALLJOBPREFIX_DEFAULT_VAL"))).ToString());
                        newFeature.set_Value(newFeature.Fields.FindField((codeRow.get_Value(codeRow.Fields.FindField("POLEUSE_ColName"))).ToString()), (codeRow.get_Value(codeRow.Fields.FindField("POLEUSE_DEFAULT_VAL"))).ToString());
                        newFeature.set_Value(newFeature.Fields.FindField((codeRow.get_Value(codeRow.Fields.FindField("MATERIAL_ColName"))).ToString()), (codeRow.get_Value(codeRow.Fields.FindField("MATERIAL_DEFAULT_VAL"))).ToString());
                        newFeature.set_Value(newFeature.Fields.FindField((codeRow.get_Value(codeRow.Fields.FindField("JOINTCOUNT_ColName"))).ToString()), (codeRow.get_Value(codeRow.Fields.FindField("JOINTCOUNT_DEFAULT_VAL"))).ToString());
                        newFeature.set_Value(newFeature.Fields.FindField((codeRow.get_Value(codeRow.Fields.FindField("POLETYPE_ColName"))).ToString()), (codeRow.get_Value(codeRow.Fields.FindField("POLETYPE_DEFAULT_VAL"))).ToString());
                        this.Close();

                        gpfeatSelect.Add(newFeature);

                        //find intersection EDGIS.MIANTENANCEPLAT feature
                        String strLocalOfcId = String.Empty;
                        IFeatureClass featureClassMP = giFeatWs.OpenFeatureClass((codeRow.get_Value(codeRow.Fields.FindField("Maintenanceplat_TblName"))).ToString());
                        IFeature iFeatMp = GetFeatureFromSpatialQuery(newFeature, featureClassMP);

                        if (iFeatMp == null)
                            strLocalOfcId = "XJ";
                        else
                            strLocalOfcId = iFeatMp.get_Value(iFeatMp.Fields.FindField((codeRow.get_Value(codeRow.Fields.FindField("LocalOfficeId_ColName"))).ToString())).ToString().Trim();

                        newFeature.set_Value(newFeature.Fields.FindField((codeRow.get_Value(codeRow.Fields.FindField("LocalOfficeId_ColName"))).ToString()), strLocalOfcId);

                        int iOid = newFeature.OID;

                        newFeature.Store();

                        //DoIdentify(pMxDoc.ActivatedView, iOid);
                        //gSpaceEdit.StopEditOperation();
                        _editor.StopOperation("PGEPLCTools_CreatePoleCommand");
                    }
                }
                else
                {
                    MessageBox.Show("Please select single Row");
                }
            }
            catch (Exception ex)
            {
                _logger.Error("PG&E PLC Create pole , Error in creating SupportStructure feature ", ex);
                if (((IWorkspaceEdit2)_editor.EditWorkspace).IsInEditOperation)
                {

                    _editor.AbortOperation();
                }
            }
        }

        # endregion


        #region DataGridView Methods
        //Function to zoom to the selected one feature
        private void dgvCreatePole_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if ((dgvCreatePole.SelectedRows.Count > 1) && (dgvCreatePole.SelectedRows.Count != dgvCreatePole.RowCount))
                {
                    MessageBox.Show("Please select single Row");
                    return;
                }
                else
                {
                    if (dgvCreatePole.SelectedRows.Count == 1)
                    {
                        gSupportStructure = null;
                        gpGraphicCont.DeleteAllElements();
                        ////Changed after Go-Live
                        Double _dLatitude = Convert.ToDouble(dgvCreatePole.SelectedRows[0].Cells[codeRow.get_Value(codeRow.Fields.FindField("Dlg_LATITUDE_ColName")).ToString()].Value.ToString());
                        Double _dLongitude = Convert.ToDouble(dgvCreatePole.SelectedRows[0].Cells[codeRow.get_Value(codeRow.Fields.FindField("Dlg_LONGITUDE_ColName")).ToString()].Value.ToString());
                        Double XCoord = 0;// Convert.ToDouble(dgvCreatePole.SelectedRows[0].Cells["X"].Value.ToString());
                        Double YCoord = 0;//Convert.ToDouble(dgvCreatePole.SelectedRows[0].Cells["Y"].Value.ToString());
                        GetLatLongFromXY(gfcSupportStructure, _dLatitude, _dLongitude, out XCoord, out YCoord);
                        if (XCoord == 0 || YCoord == 0)
                        {
                            throw new Exception("PG&E PLC Create pole , Error in getting X,Y from Latitude and Longitude.");
                        }
                        IPoint pPole = new PointClass();
                        pPole.X = XCoord;
                        pPole.Y = YCoord;
                        gSupportStructure = (IGeometry)pPole;


                        AddGraphicToMap(gSupportStructure);

                        IGeometryCollection m_pGeoColl = new GeometryBagClass();
                        IGeometryBag geoBag = new GeometryBagClass();

                        m_pGeoColl.AddGeometry(gSupportStructure);

                        geoBag = (IGeometryBag)m_pGeoColl;
                        //Change After Go-Live
                        ZoomOnGraphicElement(geoBag, 60);

                        pMxDoc.ActivatedView.Refresh();
                    }

                    else if (dgvCreatePole.SelectedRows.Count == dgvCreatePole.RowCount)
                    {
                        SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("PG&E PLC Create pole , Zoom to selected feature by selected in dialog", ex);
            }
        }

        private void dgvCreatePole_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            dgvCreatePole.Rows[0].Selected = false;
        }


        #endregion


        # region Internal Methods
        //Function to convert XY to Latitude longitude
        public void GetLatLongFromXY(IFeatureClass pFClass, double _dLatitude, double _dLongitude, out double x, out double y)
        {
            x = 0;
            y = 0;
            try
            {

                ISpatialReference fromSR = ((IGeoDataset)pFClass).SpatialReference;
                //Create Spatial Reference Factory            
                ISpatialReferenceFactory srFactory = new SpatialReferenceEnvironmentClass();
                ISpatialReference sr1;
                //GCS to project from              
                IGeographicCoordinateSystem gcs = srFactory.CreateGeographicCoordinateSystem(4269);
                sr1 = gcs;
                //Projected Coordinate System to project into            

                ISpatialReference sr2;
                sr2 = fromSR;
                //Point to project             
                IPoint point = new PointClass() as IPoint;
                point.PutCoords(_dLongitude, _dLatitude);
                //Geometry Interface to do actual project             
                IGeometry geometry;
                geometry = point;
                geometry.SpatialReference = sr1;
                geometry.Project(sr2);
                point = geometry as IPoint;
                point.QueryCoords(out x, out y);

            }
            catch (Exception ex)
            {
                _logger.Error("PG&E PLC Create pole , Error in getting X,Y from Latitude and Longitude", ex);
            }
        }


        /// <summary>Get feature from spatial serch </summary>
        /// <param name="feat">esri feature</param>
        /// <param name="polyFeatClass">feature class to serch</param>
        /// <returns>return polygon feature</returns>
        private IFeature GetFeatureFromSpatialQuery(IFeature feat, IFeatureClass polyFeatClass)
        {
            IFeature searchedFeature = null;
            IFeatureCursor featCursor = null;
            try
            {
                //perform spatial filter
                ISpatialFilter spatialFilter = new SpatialFilterClass();
                spatialFilter.Geometry = feat.Shape;
                spatialFilter.GeometryField = polyFeatClass.ShapeFieldName;
                spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;

                //get featureCursor
                featCursor = polyFeatClass.Search(spatialFilter, false);


                //Get the feature from feature cursor
                while ((searchedFeature = featCursor.NextFeature()) != null)
                {
                    break;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("PG&E PLC Create pole , Zoom to the selected feature", ex);
            }
            finally
            {
                //Release COM object
                if (featCursor != null)
                {
                    Marshal.ReleaseComObject(featCursor);
                    featCursor = null;
                }

            }
            return searchedFeature;

        }

        public void DoIdentify(ESRI.ArcGIS.Carto.IActiveView activeView, int onjId)
        {
            try
            {
                activeView = (ESRI.ArcGIS.Carto.IActiveView)pMxDoc.FocusMap;
                if (activeView == null)
                    return;

                ESRI.ArcGIS.Carto.IMap map = activeView.FocusMap;
                ESRI.ArcGIS.CartoUI.IIdentifyDialog identifyDialog = new ESRI.ArcGIS.CartoUI.IdentifyDialogClass();
                identifyDialog.Map = map;

                //Clear the dialog on each mouse click  
                identifyDialog.ClearLayers();
                ESRI.ArcGIS.Display.IScreenDisplay screenDisplay = activeView.ScreenDisplay;

                ESRI.ArcGIS.Display.IDisplay display = screenDisplay;
                identifyDialog.Display = display;

                ESRI.ArcGIS.CartoUI.IIdentifyDialogProps identifyDialogProps = (ESRI.ArcGIS.CartoUI.IIdentifyDialogProps)identifyDialog;
                ESRI.ArcGIS.Carto.IEnumLayer enumLayer = identifyDialogProps.Layers;
                enumLayer.Reset();

                IFeatureLayer gfeatureLayerSS = new FeatureLayerClass();
                gfeatureLayerSS.FeatureClass = gfcSupportStructure;
                ILayer layer = (ILayer)gfeatureLayerSS;

                identifyDialog.AddLayerIdentifyOID(layer, onjId);

                identifyDialog.Show();
            }
            catch (Exception ex)
            {
                _logger.Error("PG&E PLC Create pole , Do identify error", ex);
                // MessageBox.Show("Error in zoomming selected feature");
            }

        }
        # endregion

        #region graphic code
        //Function to create IGeometry from Latitude longitude
        private IGeometry getGeom(int selRowNum)
        {
            try
            {
                gSupportStructure = null;
                //Changed after Go-Live
                Double _dLatitude = Convert.ToDouble(dgvCreatePole.SelectedRows[selRowNum].Cells[codeRow.get_Value(codeRow.Fields.FindField("Dlg_LATITUDE_ColName")).ToString()].Value.ToString());
                Double _dLongitude = Convert.ToDouble(dgvCreatePole.SelectedRows[selRowNum].Cells[codeRow.get_Value(codeRow.Fields.FindField("Dlg_LONGITUDE_ColName")).ToString()].Value.ToString());
                Double XCoord = 0;
                Double YCoord = 0;
                GetLatLongFromXY(gfcSupportStructure, _dLatitude, _dLongitude, out XCoord, out YCoord);
                //Double XCoord = Convert.ToDouble(dgvCreatePole.SelectedRows[selRowNum].Cells["X"].Value.ToString());
                //Double YCoord = Convert.ToDouble(dgvCreatePole.SelectedRows[selRowNum].Cells["Y"].Value.ToString());

                if (XCoord == 0 || YCoord == 0)
                {
                    MessageBox.Show("Error in getting X,Y from Lat Long ", "PG&E PLC Create pole", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    throw new Exception("PG&E PLC Create pole , Error in getting X,Y from Latitude and Longitude;");
                }
                IPoint pPole = new PointClass();
                pPole.X = XCoord;
                pPole.Y = YCoord;

                gSupportStructure = (IGeometry)pPole;

                return gSupportStructure;
            }
            catch (Exception ex)
            {
                _logger.Error("PG&E PLC Create pole , error in get geom ", ex);
                return null;
            }
        }
        //function to create graphic
        public void CreateGraphic(IGeometry pGeometry, IRgbColor pColor, string elementName)
        {
            try
            {
                if (pGeometry == null) return;
                DeleteGraphicsElement(elementName, false, gobjGraphicLayer);

                IElement element = null;
                IElementProperties pElementProperties = null;
                ISimpleMarkerSymbol simpleMarkerSymbol = new ESRI.ArcGIS.Display.SimpleMarkerSymbolClass();
                simpleMarkerSymbol.Color = pColor;
                simpleMarkerSymbol.Outline = true;
                //ESRI.ArcGIS.Display.IRgbColor outlineRgbColor
                //simpleMarkerSymbol.OutlineColor = outlineRgbColor;
                simpleMarkerSymbol.Size = 15;
                simpleMarkerSymbol.Style = ESRI.ArcGIS.Display.esriSimpleMarkerStyle.esriSMSCircle;


                ESRI.ArcGIS.Carto.IMarkerElement markerElement = new ESRI.ArcGIS.Carto.MarkerElementClass();
                markerElement.Symbol = simpleMarkerSymbol;
                element = (ESRI.ArcGIS.Carto.IElement)markerElement; // Explicit Cast
                pElementProperties = (IElementProperties)element;
                pElementProperties.Name = elementName;
                element.Geometry = pGeometry;
                element.Locked = true;
                //Get the graphics layer and screen display

                IScreenDisplay pScreenDisplay = pMxDoc.ActivatedView.ScreenDisplay;

                //Add the marker element to the layer graphics container
                gpGraphicCont.AddElement((IElement)markerElement, 0);
            }
            catch (Exception ex)
            {
                _logger.Error("PG&E PLC Create pole , error in function create graphic ", ex);
            }
        }

        //Function to add graphic to map
        public void AddGraphicToMap(IGeometry geometry)
        {
            try
            {
                Type appRefType = Type.GetTypeFromProgID("esriFramework.AppRef");
                object appRefObj = Activator.CreateInstance(appRefType);
                //IApplication arcMapApp = appRefObj as IApplication;
                //IMxDocument mxDoc = (IMxDocument)arcMapApp.Document;
                IMap map = pMxDoc.FocusMap;

                //ESRI.ArcGIS.Carto.IGraphicsContainer graphicsContainer = (ESRI.ArcGIS.Carto.IGraphicsContainer)map; // Explicit Cast
                ESRI.ArcGIS.Carto.IElement element = null;

                IRgbColor color = new RgbColorClass();
                color.Blue = 200;
                color.Red = 10;
                color.Green = 10;

                if ((geometry.GeometryType) == ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPoint)
                {
                    // Marker symbols
                    ESRI.ArcGIS.Display.ISimpleMarkerSymbol simpleMarkerSymbol = new ESRI.ArcGIS.Display.SimpleMarkerSymbolClass();
                    simpleMarkerSymbol.Color = color;
                    simpleMarkerSymbol.Outline = false;
                    simpleMarkerSymbol.Size = 10;
                    simpleMarkerSymbol.Style = ESRI.ArcGIS.Display.esriSimpleMarkerStyle.esriSMSCircle;

                    ESRI.ArcGIS.Carto.IMarkerElement markerElement = new ESRI.ArcGIS.Carto.MarkerElementClass();
                    markerElement.Symbol = simpleMarkerSymbol;
                    element = (ESRI.ArcGIS.Carto.IElement)markerElement; // Explicit Cast
                }
                else if ((geometry.GeometryType) == ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolyline)
                {
                    //  Line elements
                    ESRI.ArcGIS.Display.ISimpleLineSymbol simpleLineSymbol = new ESRI.ArcGIS.Display.SimpleLineSymbolClass();
                    simpleLineSymbol.Color = color;
                    simpleLineSymbol.Style = ESRI.ArcGIS.Display.esriSimpleLineStyle.esriSLSSolid;
                    simpleLineSymbol.Width = 1;

                    ESRI.ArcGIS.Carto.ILineElement lineElement = new ESRI.ArcGIS.Carto.LineElementClass();
                    lineElement.Symbol = simpleLineSymbol;
                    element = (ESRI.ArcGIS.Carto.IElement)lineElement; // Explicit Cast
                }
                else if ((geometry.GeometryType) == ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolygon)
                {
                    // Polygon elements
                    ESRI.ArcGIS.Display.ISimpleFillSymbol simpleFillSymbol = new ESRI.ArcGIS.Display.SimpleFillSymbolClass();
                    simpleFillSymbol.Color = color;
                    simpleFillSymbol.Style = ESRI.ArcGIS.Display.esriSimpleFillStyle.esriSFSForwardDiagonal;
                    ESRI.ArcGIS.Carto.IFillShapeElement fillShapeElement = new ESRI.ArcGIS.Carto.PolygonElementClass();
                    fillShapeElement.Symbol = simpleFillSymbol;
                    element = (ESRI.ArcGIS.Carto.IElement)fillShapeElement; // Explicit Cast
                }
                if (!(element == null))
                {
                    element.Geometry = geometry;
                    gpGraphicCont.AddElement(element, 0);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("PG&E PLC Create pole , error in function Zoom on graphic element ", ex);
            }
        }

        //Function to zoom to the selected feature
        //Change After Go-Live
        private void ZoomOnGraphicElement(IGeometryBag pGeomBag, int iScale)
        {
            try
            {
                IEnvelope IbagEnv = null;
                IbagEnv = pGeomBag.Envelope;

                IArea aa = (IArea)pGeomBag.Envelope;
                IbagEnv.CenterAt(aa.Centroid);// pPoint;
                IbagEnv.Expand(20, 20, false);
                pMxDoc.ActiveView.Extent = IbagEnv;
                //Change After Go-Live
                if (iScale > 0)
                {
                    ((IMap)pMxDoc.FocusMap).MapScale = iScale;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("PG&E PLC Create pole , error in function Zoom on graphic element ", ex);
            }

        }

        //Function to delete all craeted graphic from the map
        public void DeleteGraphicsElement(string strEleName, bool deleteAll, IGraphicsLayer objGraphicLayer)
        {
            try
            {
                if (deleteAll) { gpGraphicCont.DeleteAllElements(); return; }
                IElementProperties pElementProperties = null;
                IElement pElement = gpGraphicCont.Next();
                for (; pElement != null; pElement = gpGraphicCont.Next())
                {
                    pElementProperties = (IElementProperties)pElement;
                    if (pElementProperties.Name.ToUpper() == strEleName.ToUpper())
                    {
                        gpGraphicCont.DeleteElement(pElement);
                    }
                }
                pMxDoc.ActivatedView.Refresh();
            }
            catch (Exception ex)
            {
                _logger.Error("PG&E PLC Create pole , Error in function delete graphic element ", ex);
            }


        }

        //Function to find graphic layer on the map
        public IGraphicsLayer FindGraphicsLayer(ref ICompositeGraphicsLayer objCompGraphicsLayer, string layerName, IApplication objApp)
        {
            bool flag;
            ICompositeLayer objCompLayer;
            ILayer objLayer;
            IFeatureLayer objFeatureLayer;
            IGraphicsLayer pGraphicLayer = null;
            try
            {
                #region Section to retrieve reference of Graphics layer object
                IMxDocument objMxDoc = (IMxDocument)objApp.Document;
                //If refernce of Composite Graphics layer object is not set, then exit from the function
                if (objCompGraphicsLayer == null) return null;
                //Set Composite Graphic layer object to Composite Layer object
                objCompLayer = (ICompositeLayer)objCompGraphicsLayer;
                //Composite layer is a collection of multiple layers, it allows to iterate through each layer in collection and 
                //Access its relevant properties and functions.
                //Loop through each layer of composite layer object and check whether layer with matching layer name is exist.
                //If it found, then set a refernce of IGraphicsLayer object
                flag = false;
                for (int i = 0; i < objCompLayer.Count; i++)
                {//Set refernce of Layer object for given index
                    objLayer = objCompLayer.get_Layer(i);
                    //Check whether layer name is matching with required name.
                    if (objLayer.Name == layerName)
                    {//If it matches, then check whether current refernce of layer object is Graphics layer or not.
                        if (objLayer is IGraphicsLayer)
                        {//If layer is Graphics layer, then set a reference of graphic layer, set flag to true and exit from loop.
                            pGraphicLayer = (IGraphicsLayer)objLayer;
                            flag = true;
                            break;
                        }
                    }
                }
                //If no layer found for matching name, then create new graphic layer with required name and add on map.
                if (flag == false)
                {//Create new instance of FeatureLayer object, since reference of Graphics Layer object is retrieved from Feature layer object.
                    objFeatureLayer = new FeatureLayerClass();
                    //Set its visibility property to true.
                    objFeatureLayer.Visible = true;
                    //Add feature layer object into composite graphic layer collection, which inturn will add it on a map as a Graphics layer and 
                    //returns reference of added layer a Graphics Layer.
                    pGraphicLayer = objCompGraphicsLayer.AddLayer(layerName, objFeatureLayer);
                    //Activate returned refernce of Graphics layer object to adjust with screen display of map.
                    pGraphicLayer.Activate(objMxDoc.ActiveView.ScreenDisplay);
                }
                //Still, grpahics layer object is not found, then return null or return reference of GraphicsLayer object
                if (pGraphicLayer == null) return null;
                else return pGraphicLayer;
                #endregion
            }
            catch (Exception ex)
            {
                _logger.Error("PG&E PLC Create pole , Error in function Find graphic layer ", ex);
                return null;
            }
        }

        public ILayer getMapLayerByName(String LayerName)
        {
            ILayer featLayerSel;
            try
            {
                for (int lyrCnt = 0; lyrCnt < pMxDoc.FocusMap.LayerCount; lyrCnt++)
                {
                    featLayerSel = (ILayer)pMxDoc.FocusMap.get_Layer(lyrCnt);
                    if (featLayerSel.Name.ToUpper().ToString() == LayerName.ToUpper())
                        return featLayerSel;
                }
            }
            catch (Exception ex)
            {
            }
            return null;
        }

        public IWorkspace ArcSdeWorkspaceFromFile(String connectionFile)
        {
            return ((IWorkspaceFactory)Activator.CreateInstance(Type.GetTypeFromProgID("esriDataSourcesGDB.SdeWorkspaceFactory"))).
                OpenFromFile(connectionFile, 0);
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
                _logger.Error("PG&E PLC Create pole , error in function FindLayerByName ", ex);
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

                    if (lyr != null && lyr.Name.Equals(lyrName))
                    {
                        return lyr;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("PG&E PLC Create pole , error in function FindLayerByName ", ex);
            }

            return null;
        }

        public string GetDescFromDomain(IObject iObj, IField field, String strDes)
        {
            string valueFromDomain = null;

            try
            {
                if (iObj == null || field == null)
                {
                    return valueFromDomain;
                }

                //Get the field value
                valueFromDomain = iObj.get_Value(iObj.Fields.FindField(field.Name)).ToString();

                //Get the domain attached to the field
                IDomain domain = field.Domain;
                if (domain == null)
                {
                    return valueFromDomain;
                }

                ICodedValueDomain codeValueDomain = domain as ICodedValueDomain;
                if (codeValueDomain == null)
                {
                    return valueFromDomain;
                }

                int codeCount = codeValueDomain.CodeCount;
                if (codeCount < 1)
                {
                    return valueFromDomain;
                }

                //Get domain description matching domain code
                for (int i = 0; i < codeCount; i++)
                {
                    if (codeValueDomain.get_Name(i).ToString() == strDes)
                    {
                        valueFromDomain = codeValueDomain.get_Value(i).ToString();
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to retrieve Domain Value from Description.", ex);
            }

            return valueFromDomain;
        }


        #endregion

        private void FrmCreatePole_Load(object sender, EventArgs e)
        {

        }
        private IRow GetConfigTableRowForNewExceptionTool(ITable tPLCLookup)
        {
            IRow ConfigTableRow = null;
            try
            {
                ICursor ICur = null;

                QueryFilter queryFilter = null;
                queryFilter = new QueryFilterClass();
                queryFilter.WhereClause = string.Format("ClassData='{0}'", "EXP_PLD_STATUS");
                ICur = tPLCLookup.Search(queryFilter, false);
                ConfigTableRow = ICur.NextRow();
                if (ICur != null) { Marshal.ReleaseComObject(ICur); }
                if (ConfigTableRow == null) throw new Exception("Configuration Data not found for CREATE_POLE in " + SLookupTable + " table");
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ConfigTableRow;
        }



    }


}
