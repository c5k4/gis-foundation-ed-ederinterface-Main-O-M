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
using System.Text.RegularExpressions;

namespace PGEElecCleanup
{
    public partial class FrmValElbowOperatingNum : Form
    {
        private IMMAutoUpdater autoupdater = null;
        private DataTable objReportTable = new DataTable();
        clsGlobalFunctions _clsGlobalFunctions = new clsGlobalFunctions();
        int intUpdCount = 0;

        public IRelationshipClass pRelCls = null;

        public FrmValElbowOperatingNum()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (rbValidate.Checked != true && raUpdate.Checked != true)
            {
                MessageBox.Show("Choose one option Validate | Update..!");
                return;
            }

            btnStart.Enabled = false;
            btnExit.Enabled = false;

            //logs
            objReportTable.Clear();
            objReportTable.Columns.Clear();
            _clsGlobalFunctions.Common_initSummaryTable("ValidateOpenPoint_OperatingNum", "ValidateOpenPoint_OperatingNum");
            _clsGlobalFunctions.Common_addColumnToReportTable(objReportTable, "FEATURECLASS_NAME,FEAT_OID,GlobalId,CEDSADEVICEID,Old_OpNum,DeviceGrpName,REMARKS");

            if (raUpdate.Checked == true)
            {
                stbInfo.Text = "Starting Process..";
                Logs.writeLine("Disable Autoupdaters...");
                stbInfo.Text = "Disable Autoupdaters ";
                #region "Disable Autoupdaters "
                mmAutoUpdaterMode objautoupdateroldmode = DisableAutoupdaters();
                #endregion

                clsTestWorkSpace.StartEditOperation();
                BeginProcess();
                clsTestWorkSpace.StopEditOperation(true);

                Logs.writeLine("Enabling autoupdaters...");
                stbInfo.Text = "Enabling autoupdaters...";
                #region start AU
                if (autoupdater != null)
                {
                    autoupdater.AutoUpdaterMode = objautoupdateroldmode;
                }
                #endregion
            }

            if (raUpdate.Checked != true)
                BeginProcess();

            clsGFMSGlobalFunctions._GFMSGlobalFunctions.GenerateTheReport_MultipleFeatureClass(objReportTable, "ValidateOpenPoint_OperatingNum");
            MessageBox.Show("Process Completed, please see the Report File.", "Report", MessageBoxButtons.OK, MessageBoxIcon.Information);
            stbInfo.Text = "Process completed see the log file.";
            Logs.writeLine("Successfully Completed");

            stbInfo.Enabled = true;
            btnExit.Enabled = true;
            btnStart.Enabled = true;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void BeginProcess()
        {
            IQueryFilter pQry = null;
            IFeatureCursor pFeatCurs = null;
            IFeatureClass pOpenPntCls = null;
            try
            {
                pOpenPntCls = clsTestWorkSpace.FeatureWorkspace.OpenFeatureClass("EDGIS.OpenPoint");
                pRelCls = clsTestWorkSpace.FeatureWorkspace.OpenRelationshipClass("EDGIS.DeviceGroup_OpenPoint");

                pQry = new QueryFilter();
                pQry.WhereClause = "SUBTYPECD in ('1','2') and CEDSADEVICEID = 9999 order by OBJECTID";
                //pQry.WhereClause = "SUBTYPECD in ('1','2') and globalid='{4E7F4F23-E17F-4A36-94F2-4BA2611B304B}'";

               
                pFeatCurs = pOpenPntCls.Search(pQry, false);
                int totCnt = pOpenPntCls.FeatureCount(pQry);

                IFeature pOpenPntFeat = (IFeature)pFeatCurs.NextFeature();
                //declare
                string strGloID = string.Empty;
                string strPntOPnum = string.Empty;
                string strDevGrpName = string.Empty;
                string strPntDevID = string.Empty;
                ISet pRelSet = null;
                int intCntr = 0;
                int err = 0;
                Boolean blnErrReported = false;
                int intUpdCount = 0;
                while (pOpenPntFeat != null)
                {
                    intCntr++;

                    if (raUpdate.Checked == true)
                        stbInfo.Text = "Processing EDGIS.OpenPoint : " + intCntr + " of " + totCnt + "  Updated:" + err;
                    else
                        stbInfo.Text = "Processing EDGIS.OpenPoint : " + intCntr + " of " + totCnt + "  Errors:" + err;

                    Application.DoEvents();

                    if (err % 500 == 0)
                    {                        
                        if (raUpdate.Checked == true)
                        {
                        clsTestWorkSpace.StopEditOperation(true);
                        clsTestWorkSpace.StartEditOperation();
                        err++;
                        }
                    }
                    
                    strGloID = string.Empty;
                    strPntOPnum = string.Empty;
                    strDevGrpName = string.Empty;
                    strPntDevID = string.Empty;
                    pRelSet = null;
                    blnErrReported = false;
                    try
                    {
                        //get data 
                        GetFieldValue((IRow)pOpenPntFeat, "GLOBALID", ref strGloID);
                        GetFieldValue((IRow)pOpenPntFeat, "OPERATINGNUMBER", ref strPntOPnum);
                        GetFieldValue((IRow)pOpenPntFeat, "CEDSADEVICEID", ref strPntDevID);


                        //if (strPntOPnum.Length != 0)
                        //{
                        //    if (true == IsAlphaOnly(strPntOPnum.Substring(strPntOPnum.Length - 1, 1).ToString()))
                        //    {
                                //get related DeviceGroup
                                pRelSet = pRelCls.GetObjectsRelatedToObject((IObject)pOpenPntFeat);

                                if (pRelSet != null && pRelSet.Count != 0)
                                {
                                    pRelSet.Reset();
                                    IRow pRow = (IRow)pRelSet.Next();
                                    while (pRow != null)
                                    {
                                        GetFieldValue(pRow, "DEVICEGROUPNAME", ref strDevGrpName);
                                        pRow = (IRow)pRelSet.Next();
                                    }
                                }
                                else
                                {
                                    _clsGlobalFunctions.Common_addRowstoReportTable(objReportTable, "EDGIS.OpenPoint," + pOpenPntFeat.OID + "," + strGloID + "," + strPntDevID + "," + strPntOPnum + ",,Related DeviceGroup NOT Found");
                                    err++;
                                    blnErrReported = true;
                                }
                       
                        //if devicegroupname is null and openpoint->operatingnumber is not null then report for manual check 
                        if (strPntOPnum.Length != 0 && strDevGrpName.Length == 0)
                        {
                            _clsGlobalFunctions.Common_addRowstoReportTable(objReportTable, "EDGIS.OpenPoint," + pOpenPntFeat.OID + "," + strGloID + "," + strPntDevID + "," + strPntOPnum + "," + strDevGrpName + ",OperatingNum | DeviceGrpName Mis-Match|Need to check manually");                                        
                        }
                        else if (strPntOPnum.Length != 0 && strDevGrpName.Length != 0 && strDevGrpName.ToUpper() != strPntOPnum.ToUpper())
                        {
                            if (true == IsAlphaOnly(strPntOPnum.Substring(strPntOPnum.Length - 1, 1).ToString()))
                            {
                                string strPntOPnumTrimAlpha = strPntOPnum.Substring(0, strPntOPnum.Length - 1).Trim();
                                //update    
                                if (strPntOPnumTrimAlpha == strDevGrpName)
                                {
                                    if (rbValidate.Checked == true)
                                    {
                                        _clsGlobalFunctions.Common_addRowstoReportTable(objReportTable, "EDGIS.OpenPoint," + pOpenPntFeat.OID + "," + strGloID + "," + strPntDevID + "," + strPntOPnum + "," + strDevGrpName + ",OperatingNum | DeviceGrpName Mis-Match");
                                        err++;
                                    }
                                    else if (raUpdate.Checked == true)
                                    {
                                        if (strDevGrpName.Length < 9)
                                        {
                                            pOpenPntFeat.set_Value(pOpenPntFeat.Fields.FindField("OPERATINGNUMBER"), strDevGrpName);
                                            pOpenPntFeat.Store();
                                            _clsGlobalFunctions.Common_addRowstoReportTable(objReportTable, "EDGIS.OpenPoint," + pOpenPntFeat.OID + "," + strGloID + "," + strPntDevID + "," + strPntOPnum + "," + strDevGrpName + ",Record Updated");
                                            err++;
                                            intUpdCount++;
                                        }
                                        else
                                        {
                                            _clsGlobalFunctions.Common_addRowstoReportTable(objReportTable, "EDGIS.OpenPoint," + pOpenPntFeat.OID + "," + strGloID + "," + strPntDevID + "," + strPntOPnum + "," + strDevGrpName + ",OperatingNum | DeviceGrpName Mis-Match|Need to check manually");
                                            err++;
                                        }
                                    }
                                }
                                else
                                {
                                    _clsGlobalFunctions.Common_addRowstoReportTable(objReportTable, "EDGIS.OpenPoint," + pOpenPntFeat.OID + "," + strGloID + "," + strPntDevID + "," + strPntOPnum + "," + strDevGrpName + ",OperatingNum | DeviceGrpName Mis-Match|Need to check manually");
                                    err++;
                                }
                            }
                            else
                            {
                                //strPntOPnum not end with alphabit and strPntOPnum and strDevGrpName not same
                                _clsGlobalFunctions.Common_addRowstoReportTable(objReportTable, "EDGIS.OpenPoint," + pOpenPntFeat.OID + "," + strGloID + "," + strPntDevID + "," + strPntOPnum + "," + strDevGrpName + ",OperatingNum | DeviceGrpName Mis-Match|Need to check manually");
                                err++;
                            }

                        }
                        else if(strPntOPnum.Length == 0 && strDevGrpName.Length != 0)
                        {
                            if (rbValidate.Checked == true)
                            {
                                _clsGlobalFunctions.Common_addRowstoReportTable(objReportTable, "EDGIS.OpenPoint," + pOpenPntFeat.OID + "," + strGloID + "," + strPntDevID + "," + strPntOPnum + "," + strDevGrpName + ",OperatingNum | DeviceGrpName Mis-Match");
                                err++;
                            }
                            else if (raUpdate.Checked == true)
                            {
                                if (strDevGrpName.Length < 8)
                                {
                                    pOpenPntFeat.set_Value(pOpenPntFeat.Fields.FindField("OPERATINGNUMBER"), strDevGrpName);
                                    pOpenPntFeat.Store();
                                    _clsGlobalFunctions.Common_addRowstoReportTable(objReportTable, "EDGIS.OpenPoint," + pOpenPntFeat.OID + "," + strGloID + "," + strPntDevID + "," + strPntOPnum + "," + strDevGrpName + ",Record Updated");
                                    err++;
                                    intUpdCount++;
                                }
                                else
                                {
                                    _clsGlobalFunctions.Common_addRowstoReportTable(objReportTable, "EDGIS.OpenPoint," + pOpenPntFeat.OID + "," + strGloID + "," + strPntDevID + "," + strPntOPnum + "," + strDevGrpName + ",OperatingNum | DeviceGrpName Mis-Match|Need to check manually");
                                    err++;
                                }
                            }                        
                        }


                        pOpenPntFeat = (IFeature)pFeatCurs.NextFeature();
                        //if intUpdCount is 5000 stop execution(EDO6 not supports more that 5000 updates to post the data)
                        if (intUpdCount == 5000)
                        {
                            pOpenPntFeat = null;
                        }
                    }
                    catch (Exception ex1) { pOpenPntFeat = (IFeature)pFeatCurs.NextFeature(); }
                }

            }
            catch (Exception ex) { Logs.writeLine("EXCP@BeginProcess " + ex.Message); }
        }

        private bool IsAlphaOnly(string strToCheck)
        {
            Regex objAlphaNumericPattern = new Regex("[^a-zA-Z]");
            return !objAlphaNumericPattern.IsMatch(strToCheck);
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
            autoupdater.AutoUpdaterMode = mmAutoUpdaterMode.mmAUMNoEvents;
            // insert code that needs to execute while autoupdaters 
            return oldMode;
        }
        //public bool IsAlphaOnly(String strToCheck)
        //{
        //    Regex objAlphaNumericPattern = new Regex("[^a-zA-Z]");
        //    return !objAlphaNumericPattern.IsMatch(strToCheck);
        //}
        internal static bool GetFieldValue(IRow pFeat, string strColname, ref string intConvID)
        {
            Boolean blnStatus = false;
            intConvID = string.Empty;
            try
            {
                int intConvIndx = pFeat.Fields.FindField(strColname);

                if (intConvIndx != -1)
                {
                    intConvID = pFeat.get_Value(intConvIndx).ToString();
                    blnStatus = true;
                }
            }
            catch (System.Exception ex)
            {
                Logs.writeLine("EXCP@GetFieldValue " + ex.Message);
            }
            return blnStatus;
        }
    }
}
