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

namespace PGEElecCleanup
{
    public partial class frmComplexDeviceIndicatorChk : Form
    {
        private IMMAutoUpdater autoupdater = null;
        private DataTable objReportTable = new DataTable();
        clsGlobalFunctions _clsGlobalFunctions = new clsGlobalFunctions();
        public frmComplexDeviceIndicatorChk()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                btnStart.Enabled = false;
                btnExit.Enabled = false;

                stbInfo.Text = "Starting Process..";

                //stbInfo.Text = "Disable Autoupdaters ";
                #region "Disable Autoupdaters "
                mmAutoUpdaterMode objautoupdateroldmode = DisableAutoupdaters();
                #endregion


                string strChildTabName = string.Empty;
                objReportTable.Clear();
                objReportTable.Columns.Clear();
                _clsGlobalFunctions.Common_initSummaryTable("ComplexDeviceIndicatorChk", "ComplexDeviceIndicatorChk");
                _clsGlobalFunctions.Common_addColumnToReportTable(objReportTable, "DEVICEGROUP_GUID,RELATED_DEVICE_CNT,TOT_RELATED_DEVICE_CNT,DEVICEFC_NAME,DEVICE_GLOBALID,DEVICE_COMPLEXDEVICEIDC,DEVICE_CREATIONUSER,DEVICE_STATUS,REMARKS");

                //clsTestWorkSpace.StartEditOperation();
                //Logs.writeLine("StartEdit the database");
                startprocess();

                //clsTestWorkSpace.StopEditOperation(true);
                //stbInfo.Text = "Enabling autoupdaters...";
                #region start AU
                if (autoupdater != null)
                {
                    autoupdater.AutoUpdaterMode = objautoupdateroldmode;
                }
                #endregion
                clsGFMSGlobalFunctions._GFMSGlobalFunctions.GenerateTheReport_MultipleFeatureClass(objReportTable, "ComplexDeviceIndicatorChk");
                MessageBox.Show("Process Completed, please see the Report File.", "Report", MessageBoxButtons.OK, MessageBoxIcon.Information);
                stbInfo.Text = "Process completed see the log file.";
                Logs.writeLine("Successfully Completed");

                btnStart.Enabled = true;
                btnExit.Enabled = true;
            }
            catch (Exception ex)
            {
                btnStart.Enabled = true;
                btnExit.Enabled = true;

                Logs.writeLine("Error on process start button " + ex.Message);
                stbInfo.Text = "Error occurred, Please see the log file.";
            }
            finally
            {

            }
        }

        private void startprocess()
        {
            Logs.writeLine("Beginning processing: " + DateTime.Now);
            int intTotalRelCnt = 0;
            string strDG_Globalid = string.Empty;
            IFeatureClass pfc_DG = null, pfc_SWITCH = null, pfc_DPD = null, pfc_TRANSFORMER = null, pfc_FUSE = null;
            int intDPDCnt = 0;
            int intSwitchCnt = 0;
            int intFuseCnt = 0;
            int intXfmrCnt = 0;
            string strRecCount = string.Empty;

            IFeature pFeat = null;
            IFeatureCursor pFCursor = null;
            IQueryFilter pQFilter = new QueryFilterClass();
            int intRecCnt = 0;
            int intProcessFeatCount = 0;


            int intiterationCount = 0;

            IRelationshipClass pRel_Fuse_Dgroup = clsTestWorkSpace.FeatureWorkspace.OpenRelationshipClass("EDGIS.DeviceGroup_Fuse");
            IRelationshipClass pRel_Switch_Dgroup = clsTestWorkSpace.FeatureWorkspace.OpenRelationshipClass("EDGIS.DeviceGroup_Switch");
            IRelationshipClass pRel_DPD_Dgroup = clsTestWorkSpace.FeatureWorkspace.OpenRelationshipClass("EDGIS.DeviceGroup_DynProtDevice");
            IRelationshipClass pRel_XFMR_Dgroup = clsTestWorkSpace.FeatureWorkspace.OpenRelationshipClass("EDGIS.DeviceGroup_Transformer");

            IWorkspaceEdit workspaceEdit = clsTestWorkSpace.FeatureWorkspace as IWorkspaceEdit;
            //Start edit session. If already started the avoid restarting this...
            workspaceEdit.StartEditing(false);
            workspaceEdit.StartEditOperation();

            pfc_DG = clsTestWorkSpace.FeatureWorkspace.OpenFeatureClass("EDGIS.DeviceGroup");
            pfc_DPD = clsTestWorkSpace.FeatureWorkspace.OpenFeatureClass("EDGIS.DynamicProtectiveDevice");
            pfc_TRANSFORMER = clsTestWorkSpace.FeatureWorkspace.OpenFeatureClass("EDGIS.Transformer");
            pfc_FUSE = clsTestWorkSpace.FeatureWorkspace.OpenFeatureClass("EDGIS.Fuse");
            pfc_SWITCH = clsTestWorkSpace.FeatureWorkspace.OpenFeatureClass("EDGIS.Switch");

            // pQFilter.WhereClause = "GLOBALID ='{4199A3D5-DDAD-448E-9F05-D614A4F80001}'";
            using (ESRI.ArcGIS.ADF.ComReleaser cr = new ESRI.ArcGIS.ADF.ComReleaser())
            {
                try
                {
                    //intRecCnt = pfc_DG.FeatureCount(pQFilter);
                    //pFCursor = pfc_DG.Search(pQFilter, false);

                    intRecCnt = pfc_DG.FeatureCount(null);
                    pFCursor = pfc_DG.Search(null, false);

                    cr.ManageLifetime(pFCursor);
                    pFeat = pFCursor.NextFeature();
                    ISet pFuse_Set = null;
                    ISet pSwitch_Set = null;
                    ISet pDPD_Set = null;
                    ISet pXfmr_Set = null;
                    while (pFeat != null)
                    {
                        try
                        {

                            intiterationCount++;
                            intProcessFeatCount++;
                            stbInfo.Text = "Processing... " + pfc_DG.AliasName + " " + intProcessFeatCount + " from " + intRecCnt;
                            Application.DoEvents();
                            strDG_Globalid = _clsGlobalFunctions.Cast(pFeat.get_Value(pFeat.Fields.FindField("GLOBALID")).ToString(), string.Empty);

                            //pFuse_Set = pRel_Fuse_Dgroup.GetObjectsRelatedToObject((IObject)pFeat);
                            //pSwitch_Set = pRel_Switch_Dgroup.GetObjectsRelatedToObject((IObject)pFeat);
                            //pDPD_Set = pRel_DPD_Dgroup.GetObjectsRelatedToObject((IObject)pFeat);
                            //pXfmr_Set = pRel_XFMR_Dgroup.GetObjectsRelatedToObject((IObject)pFeat);
                            //intTotalRelCnt = pFuse_Set.Count + pSwitch_Set.Count + pDPD_Set.Count + pXfmr_Set.Count;
                            intTotalRelCnt = 0;
                            getRelateFeatCount(pfc_DPD, strDG_Globalid, ref intDPDCnt);
                            getRelateFeatCount(pfc_FUSE, strDG_Globalid, ref intFuseCnt);
                            getRelateFeatCount(pfc_SWITCH, strDG_Globalid, ref intSwitchCnt);
                            getRelateFeatCount(pfc_TRANSFORMER, strDG_Globalid, ref intXfmrCnt);

                            intTotalRelCnt = intDPDCnt + intFuseCnt + intSwitchCnt + intXfmrCnt;
                            strRecCount = "DPD_" + intDPDCnt + "|FUSE_" + intFuseCnt + "|SWITCH_" + intSwitchCnt + "|TRANSFORMER_" + intXfmrCnt;

                            //devicegroup related device count is more than 1. Devicecomplexinc should be 'Y'. report if not 'Y'
                            if (intTotalRelCnt > 1)
                            {
                                addToReport(pfc_DPD, strDG_Globalid, strRecCount, intTotalRelCnt);
                                //UpdateFeatures(pfc_DPD, strDG_Globalid);
                                addToReport(pfc_FUSE, strDG_Globalid, strRecCount, intTotalRelCnt);
                                // UpdateFeatures(pfc_FUSE, strDG_Globalid);
                                addToReport(pfc_SWITCH, strDG_Globalid, strRecCount, intTotalRelCnt);
                                //UpdateFeatures(pfc_SWITCH, strDG_Globalid);
                            }

                            if (pFuse_Set != null) { while (Marshal.ReleaseComObject(pFuse_Set) > 0) { } }
                            pFuse_Set = null;
                            if (pSwitch_Set != null) { while (Marshal.ReleaseComObject(pSwitch_Set) > 0) { } }
                            pSwitch_Set = null;
                            if (pDPD_Set != null) { while (Marshal.ReleaseComObject(pDPD_Set) > 0) { } }
                            pDPD_Set = null;


                            if (pFeat != null) { while (Marshal.ReleaseComObject(pFeat) > 0) { } }
                            pFeat = pFCursor.NextFeature();

                        }
                        catch (Exception ex)
                        {
                            Logs.writeLine("Error at  " + pfc_SWITCH.AliasName + " : Objectid:" + pFeat.OID + " " + ex.Message);
                            pFeat = pFCursor.NextFeature();
                        }
                    }
                    if (pFCursor != null)
                    {
                        if (pFCursor != null) { while (Marshal.ReleaseComObject(pFCursor) > 0) { } }
                        pFCursor = null;
                    }
                }
                catch (Exception ex)
                {
                    Logs.writeLine("Error on startprocess " + pfc_SWITCH.AliasName + "  " + ex.Message);
                }
            }
            //Stop the edit session
            workspaceEdit.StopEditing(true);
            workspaceEdit.StopEditOperation();
            Logs.writeLine("Finished processing: " + DateTime.Now);
        }


        private void getRelateFeatCount(IFeatureClass pFC, string strDG_Guid, ref int intRelCnt)
        {
            IQueryFilter pFilter = new QueryFilterClass();
            int intCnt = 0;
            intRelCnt = 0;
            try
            {
                pFilter.WhereClause = "STRUCTUREGUID ='" + strDG_Guid + "' ";
                intCnt = pFC.FeatureCount(pFilter);
                intRelCnt = intCnt;
            }
            catch (Exception ex)
            {
                Logs.writeLine("Error at getRelateFeatCount for DEVICEGROUP:" + strDG_Guid + " " + ex.Message);
            }
            finally
            {
                if (pFilter != null) { while (Marshal.ReleaseComObject(pFilter) > 0) { } }
            }
        }


        private void addToReport(IFeatureClass pFC, string strDG_Globalid, string strRecCount, int intTotalRelCnt)
        {
            IFeature pfeat = null;
            IFeatureCursor pFCursor = null;
            string strComp_Dev_Idc = string.Empty;
            string strComp_Dev_Guid = string.Empty;
            string str_Dev_Status = string.Empty;
            string str_Dev_CreationUser = string.Empty;

            IQueryFilter pFilter = new QueryFilterClass();
            int intCnt = 0;
            try
            {
                pFilter.WhereClause = "STRUCTUREGUID ='" + strDG_Globalid + "' ";
                intCnt = pFC.FeatureCount(pFilter);
                if (intCnt != 0)
                {
                    pFCursor = pFC.Update(pFilter, false);
                    pfeat = pFCursor.NextFeature();
                    while (pfeat != null)
                    {
                        int indexCOMPLEXDEVICEIDC = pfeat.Fields.FindField("COMPLEXDEVICEIDC");
                        strComp_Dev_Idc = _clsGlobalFunctions.Cast(pfeat.get_Value(indexCOMPLEXDEVICEIDC).ToString(), string.Empty);
                        strComp_Dev_Guid = _clsGlobalFunctions.Cast(pfeat.get_Value(pfeat.Fields.FindField("GLOBALID")).ToString(), string.Empty);
                        str_Dev_CreationUser = _clsGlobalFunctions.Cast(pfeat.get_Value(pfeat.Fields.FindField("CREATIONUSER")).ToString(), string.Empty);
                        str_Dev_Status = _clsGlobalFunctions.Cast(pfeat.get_Value(pfeat.Fields.FindField("STATUS")).ToString(), string.Empty);
                        if (strComp_Dev_Idc == "N" || strComp_Dev_Idc == "")
                        {
                            UpdateFeatures(pfeat, pFCursor, indexCOMPLEXDEVICEIDC);
                            //DEVICEGROUP_GUID,RELATED_DEVICE_CNT,TOT_RELATED_DEVICE_CNT,DEVICEFC_NAME,DEVICE_GLOBALID,DEVICE_COMPLEXDEVICEIDC,DEVICE_CREATIONUSER,DEVICE_STATUS,REMARKS
                            _clsGlobalFunctions.Common_addRowstoReportTable(objReportTable, strDG_Globalid + "," + strRecCount + "," + intTotalRelCnt + "," + pfeat.Class.AliasName + "," + strComp_Dev_Guid + "," + strComp_Dev_Idc + "," + str_Dev_CreationUser + "," + str_Dev_Status + ",COMPLEXDEVICEIDC should be 'Y'. DeviceGroup related more than one device(FUSE|DPD|SWITCH)");
                        }
                        if (pfeat != null) { while (Marshal.ReleaseComObject(pfeat) > 0) { } }
                        pfeat = null;
                        pfeat = pFCursor.NextFeature();
                    }
                }
            }
            catch (Exception ex)
            {
                Logs.writeLine("Error at addToReport for DEVICEGROUP:" + strDG_Globalid + " " + ex.Message);
            }
            finally
            {
                if (pFilter != null) { while (Marshal.ReleaseComObject(pFilter) > 0) { } }

            }
        }

        private void UpdateFeatures(IFeature feature, IFeatureCursor updateCursor, int indexCOMPLEXDEVICEIDC)
        {
            try
            {
                //if more the one related feature count found the update the feature class. "Author: Bhaskar"
                feature.set_Value(indexCOMPLEXDEVICEIDC, "Y");
                updateCursor.UpdateFeature(feature);
                feature.Store();
                //Code Changes End."Author: Bhaskar"
            }
            catch (Exception ex)
            {
                Logs.writeLine("Error on UpdateFeatures " + ex.Message);
            }
        }

        private mmAutoUpdaterMode DisableAutoupdaters()
        {
            object objAutoUpdater = null;

            //Create an MMAutoupdater 
            objAutoUpdater = Activator.CreateInstance(Type.GetTypeFromProgID("mmGeodatabase.MMAutoUpdater"));
            autoupdater = objAutoUpdater as IMMAutoUpdater;
            //Save the existing mode
            mmAutoUpdaterMode oldMode = autoupdater.AutoUpdaterMode;
            //Turn off autoupdater events
            autoupdater.AutoUpdaterMode = mmAutoUpdaterMode.mmAUMStandAlone;
            // insert code that needs to execute while autoupdaters 
            return oldMode;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
