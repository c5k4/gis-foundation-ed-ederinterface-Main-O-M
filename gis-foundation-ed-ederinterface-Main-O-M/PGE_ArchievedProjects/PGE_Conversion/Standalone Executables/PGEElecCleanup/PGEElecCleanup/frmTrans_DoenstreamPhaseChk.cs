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
    public partial class frmTrans_DoenstreamPhaseChk : Form
    {
        private IMMAutoUpdater autoupdater = null;
        clsGlobalFunctions _clsGlobalFunctions = new clsGlobalFunctions();
       
        INetworkFeature _pNetworkFeature = null;
        IGeometricNetwork _pGeonetWork = null;
      
        List<string> lstAllTabs = new List<string>();
        private DataTable objReportTable = new DataTable();

        List<int> lstJunctionIds = new List<int>();
        List<int> lstEdgeIds = new List<int>();


        //private Dictionary<int, IFeature> _JunctionsByEID = new Dictionary<int, IFeature>();
        //private Dictionary<int, IFeature> _EdgesByEID = new Dictionary<int, IFeature>();

        List<int> _JunctionsByEID = new List<int>();
        List<int> _EdgesByEID = new List<int>();


        List<int> tracedJunctions = new List<int>();
        List<int> tracedEdges = new List<int>();
        private Dictionary<int, int> _EdgeByJunction = new Dictionary<int, int>();

        bool bolFirstPriUGFound = false;
        string strPhaseFeedingUGCond = string.Empty;
        private IFeature pUpstreamFeat = null;

        public frmTrans_DoenstreamPhaseChk()
        {
            InitializeComponent();
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            try
            {
                btnRun.Enabled = false;
                btnClose.Enabled = false;
                string strTabName = string.Empty;
                string strWhereClause = string.Empty;
                string strfileName = string.Empty;

                if (txt_ManualCls.Text == "")
                {
                    strWhereClause = cmbWhereClause.SelectedItem.ToString().Replace(">", "_");
                    strWhereClause = strWhereClause.Replace("<", "_");
                    strfileName = cmbAllFC.SelectedItem.ToString() + "_" + strWhereClause;

                    strWhereClause = cmbWhereClause.SelectedItem.ToString();

                    //strfileName = "testc";
                }
                else
                {
                    strWhereClause = txt_ManualCls.Text;
                }
                              
                _clsGlobalFunctions.Common_initSummaryTable(strfileName, strfileName);
                _clsGlobalFunctions.Common_addColumnToReportTable(objReportTable, "COND_FC_NAME,COND_GLOBALID,COND_CIRCUITID,COND_PHASE,FC_NAME,TRANSFORMER_GLOBALID,TRANSFORMER_PHASE,FEEDPATH_COUNT,C,REMARKS");
                
                stbStatus.Text = "Starting Process..";
                //stbStatus.Text = "Disable Autoupdaters ";
                //Logs.writeLine("Disable Autoupdaters ");
                //#region "Disable Autoupdaters "
                //mmAutoUpdaterMode objautoupdateroldmode = DisableAutoupdaters();
                //#endregion

              
                Logs.writeLine("Start Date and Time  :" + System.DateTime.Now);
                
                startprocess(cmbAllFC.SelectedItem.ToString(), strWhereClause);                
                Logs.writeLine("END Date and Time  :" + System.DateTime.Now);
                Logs.writeLine("");
                stbStatus.Text = "Enabling autoupdaters...";
                Logs.writeLine("Enabling autoupdaters...");
                //#region start AU
                //if (autoupdater != null)
                //{
                //    autoupdater.AutoUpdaterMode = objautoupdateroldmode;
                //}
                //#endregion

                clsGFMSGlobalFunctions._GFMSGlobalFunctions.GenerateTheReport_MultipleFeatureClass(objReportTable, strfileName);
                MessageBox.Show("Process Completed, please see the Report File.", "Report", MessageBoxButtons.OK, MessageBoxIcon.Information);
                stbStatus.Text = "Process completed see the log file.";
                Logs.writeLine("Successfully Completed");

                btnRun.Enabled = true;
                btnClose.Enabled = true;
            }
            catch (Exception ex)
            {
                btnRun.Enabled = true;
                btnClose.Enabled = true;

                Logs.writeLine("Error on process start button " + ex.Message);
                stbStatus.Text = "Error occurred, Please see the log file.";
            }
            finally
            {

            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
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

        private void startprocess(string strTabName, string strWhereClause)
        {
            IFeatureClass pCondFC = null;

            IFeature pCondFeat = null;
            IFeatureCursor pFCursor = null;
            IQueryFilter pQFilter = new QueryFilterClass();
            string strCondPhase = string.Empty;
            string strCondCircuitID = string.Empty;
            int intRecCnt = 0;
            int intProcessFeatCount = 0;
            int intUpdCount = 0;
            Boolean bolMovetoNextStep = false;

            pCondFC = clsTestWorkSpace.FeatureWorkspace.OpenFeatureClass(strTabName);

            if (pCondFC == null) return;

            using (ESRI.ArcGIS.ADF.ComReleaser cr = new ESRI.ArcGIS.ADF.ComReleaser())
            {
                try
                {
                    //if (cmbWhereClause.SelectedItem.ToString().Length == 0)
                    //{
                    //    MessageBox.Show("Please select whereclause and then click Run button", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //    return;
                    //}

                    pQFilter.WhereClause = strWhereClause;// cmbWhereClause.SelectedItem.ToString(); //; cmbWhereClause.SelectedItem.ToString();
                    //pQFilter.WhereClause = "GLOBALID in('{F5EEBF26-61D0-4502-ABD6-8EC5B7454D5F}','{4AA5EE56-9D21-4981-9279-B49E7A5C953C}','{94FA6E54-5FC7-4D41-8963-C61CD4230F01}','{1F55BA43-948A-419C-9170-8E8A5437153C}')";
                    intRecCnt = pCondFC.FeatureCount(pQFilter);
                    pFCursor = pCondFC.Search(pQFilter, false);


                    //intRecCnt = pCondFC.FeatureCount(null);
                    //pFCursor = pCondFC.Search(null, false);
                    
                    cr.ManageLifetime(pFCursor);
                    pCondFeat = pFCursor.NextFeature();
                    while (pCondFeat != null)
                    {
                        try
                        {
                            intProcessFeatCount++;
                            stbStatus.Text = "Processing Features " + intProcessFeatCount + " from " + intRecCnt;
                            Application.DoEvents();
                            strCondPhase = _clsGlobalFunctions.Cast(pCondFeat.get_Value(pCondFC.FindField("PHASEDESIGNATION")).ToString(), string.Empty);
                            strCondCircuitID = _clsGlobalFunctions.Cast(pCondFeat.get_Value(pCondFC.FindField("CIRCUITID")).ToString(), string.Empty);
                            IEdgeFeature _pEdgeFeat = (IEdgeFeature)pCondFeat;
                            if (_pNetworkFeature == null || _pGeonetWork == null)
                            {
                                _pNetworkFeature = (INetworkFeature)pCondFeat;
                                _pGeonetWork = _pNetworkFeature.GeometricNetwork;
                            }
                            //chk transformer having eaither side of the conductor 
                            bolMovetoNextStep = ValidateFromTojunctionfeats(pCondFeat, strCondPhase);
                            if (bolMovetoNextStep == true)
                            {
                                //find feeding transformer
                                findFeedTrans(pCondFeat, strCondPhase,strCondCircuitID);
                                
                            }
                            pCondFeat = pFCursor.NextFeature();
                            //pCondFeat = null;
                        }
                        catch (Exception ex)
                        {
                            Logs.writeLine("Error at  Processing startprocess : Objectid:" + pCondFeat.OID + " " + ex.Message);
                            pCondFeat = pFCursor.NextFeature();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logs.writeLine("Error on startprocess   " + ex.Message);
                }
            }
        }

        private Boolean ValidateFromTojunctionfeats(IFeature pCondFeat, string strCondPhase)
        {
            Boolean bolMovetoNextStep = true;
            try
            {
                IEdgeFeature _pEdgeFeat = (IEdgeFeature)pCondFeat;
                IFeature pfromJunFeat = this.GetFeature(_pEdgeFeat.FromJunctionEID, esriElementType.esriETJunction, _pGeonetWork);
                IFeature pToJunFeat = this.GetFeature(_pEdgeFeat.ToJunctionEID, esriElementType.esriETJunction, _pGeonetWork);

                if (pfromJunFeat.Class.AliasName.ToUpper() == "Transformer".ToUpper())
                {
                    //strCondPhase = _clsGlobalFunctions.Cast(junction1.get_Value(junction1.Fields.FindField("globalid")).ToString(), string.Empty);
                    ValidatePhase(pCondFeat, pfromJunFeat, "1");
                    bolMovetoNextStep = false;
                }
                else if (pToJunFeat.Class.AliasName.ToUpper() == "Transformer".ToUpper())
                {
                    ValidatePhase(pCondFeat, pToJunFeat, "1");
                    bolMovetoNextStep = false;
                }
                return bolMovetoNextStep;
            }
            catch (Exception ex)
            {
                Logs.writeLine("Error on ValidateFromTojunctionfeats  " + pCondFeat.Class.AliasName + ":" + pCondFeat.get_Value(pCondFeat.Fields.FindField("GLOBALID")).ToString() + " " + ex.Message);
                return bolMovetoNextStep;
            }


        }
        //function to find feeding transformer and validate phase with input feature phase.
        private void findFeedTrans(IFeature pCondFeat, string strCondPhase, string strCondCircuitID)
        {

            bool found = false;
            IMMFeedPath[] feedPaths = new IMMFeedPath[0];
            IMMFeedPath feedPath;
            try
            {
                //get feedpaths from inpur feature
                feedPaths = TraceMM2(pCondFeat);

                if (feedPaths.Count() != 0)
                {
                    int intPathCnt = feedPaths.Count();
                    //iterate feedpaths until get first Transformer
                    for (int feedPath_ct = 0; feedPath_ct <= feedPaths.Count() - 1; ++feedPath_ct)
                    {
                        if (found == true)
                            break;
                        feedPath = feedPaths[feedPath_ct];
                        for (int ele_ct = 1; ele_ct < feedPath.PathElements.Count(); ++ele_ct)
                        {
                            IMMPathElement pathElement = feedPath.PathElements[ele_ct];
                            if (pathElement.ElementType == esriElementType.esriETJunction)
                            {
                                IFeature junction = this.GetFeature(pathElement.EID, esriElementType.esriETJunction, _pGeonetWork);
                                if (junction.Class.AliasName.ToUpper() == "Transformer".ToUpper())
                                {
                                    ValidatePhase(pCondFeat, junction, feedPaths.Count().ToString());
                                    found = true;
                                    if (found == true)
                                        break;
                                    // string strCondPhase = _clsGlobalFunctions.Cast(junction.get_Value(junction.Fields.FindField("globalid")).ToString(), string.Empty);
                                    //Logs.writeLine(junction1.Class.AliasName + ":" + strCondPhase);
                                }
                            }
                        }
                    }
                    //Report conductor if feeding Transformer not found  
                    if (found == false)
                    {
                        _clsGlobalFunctions.Common_addRowstoReportTable(objReportTable, pCondFeat.Class.AliasName + "," + pCondFeat.get_Value(pCondFeat.Fields.FindField("GLOBALID")).ToString() + "," +strCondCircuitID+","+ strCondPhase + ",,,,,No feeding transformer found");
                    
                    }
                }
                else
                {
                    Logs.writeLine(pCondFeat.Class.AliasName + ":" + pCondFeat.get_Value(pCondFeat.Fields.FindField("GLOBALID")).ToString() + " No feedpaths returned by upstream trace ");
                
                }
            }
            catch (Exception ex)
            {
                Logs.writeLine("Error on findFeedTrans  " + pCondFeat.Class.AliasName + ":" + pCondFeat.get_Value(pCondFeat.Fields.FindField("GLOBALID")).ToString() + " " + ex.Message);
            }



        }

        private IMMFeedPath[] TraceMM2(IFeature pCondFeat)
        {
            IEdgeFeature pEdgeFeat = (IEdgeFeature)pCondFeat;
            
            int this_EID = pEdgeFeat.FromJunctionEID;
            //m_DebugLog.WriteLine(DateTime.Now + " Starting MMTrace2...EID = " + this_EID.ToString());
            //m_DebugLog.Flush();
            //
            int[] barrierJncts = new int[0];
            int[] barrierEdges = new int[0];
            IMMFeedPath[] feedPath = new IMMFeedPath[0];
            IMMCurrentStatus currentStatus = null;
            IMMTraceStopper[] traceStopperJunctions = null;
            IMMTraceStopper[] traceStopperEdges = null;
            IMMElectricTraceSettings settings = new MMElectricTraceSettings();
            settings.RespectEnabledField = true;
            IMMElectricTracing elecTrace = new MMFeederTracerClass();
            //
            try
            {
                // New
                elecTrace.FindFeedPaths(
                    _pGeonetWork,
                    settings,
                    currentStatus,
                    this_EID,
                    esriElementType.esriETJunction,
                    SetOfPhases.abc,
                    barrierJncts,
                    barrierEdges,
                    out feedPath,
                    out traceStopperJunctions,
                    out traceStopperEdges);
            }
            catch (Exception ex)
            {
                Logs.writeLine("Error on TraceMM2  " + pCondFeat.Class.AliasName +":"+ pCondFeat.get_Value(pCondFeat.Fields.FindField("GLOBALID")).ToString() + " " + ex.Message);
                return feedPath;
            }          
            return feedPath;
        }








      
        private bool ValidatePhase(IFeature pCondFeat, IFeature pdevFeat,string strFeedpathCnt)
        {
            Boolean bolPhaseVal = false;
            Boolean bolTwoPhaseTrans = false;
            string strPhase = string.Empty;
            string strCondPhase = string.Empty;
            string strCondCircuitID = string.Empty;
            string strDevPhase = string.Empty;
            

            //4-A,//B-2
            //C-1
            //AB-6
            //AC-5
            //BC-3
            try
            {
                strCondCircuitID = _clsGlobalFunctions.Cast(pCondFeat.get_Value(pCondFeat.Fields.FindField("CIRCUITID")).ToString(), string.Empty);
                strCondPhase = _clsGlobalFunctions.Cast(pCondFeat.get_Value(pCondFeat.Fields.FindField("PHASEDESIGNATION")).ToString(), string.Empty);
                strDevPhase = _clsGlobalFunctions.Cast(pdevFeat.get_Value(pdevFeat.Fields.FindField("PHASEDESIGNATION")).ToString(), string.Empty);



                if ((strCondPhase == "4" || strCondPhase == "7") && (strDevPhase == "4" || strDevPhase == "6" || strDevPhase == "5" || strDevPhase == "7"))
                {
                    bolPhaseVal = true; 
                
                }
                else if (strCondPhase == "2" && (strDevPhase == "3" || strDevPhase == "2"))
                {
                    bolPhaseVal = true;                 
                }
                else if (strCondPhase == strDevPhase )
                {
                    bolPhaseVal = true; 
                }

                ////AB-6
                //if (strDevPhase == "6" && (strCondPhase == "4" || strCondPhase == "2" || strCondPhase == "6"))
                //{
                //    bolPhaseVal = true;                    
                //}

                ////AC-5
                //else if (strDevPhase == "5" && (strCondPhase == "4" || strCondPhase == "1" || strCondPhase == "5"))
                //{
                //    bolPhaseVal = true;                    
                //}
                ////BC-3
                //else if (strDevPhase == "3" && (strCondPhase == "2" || strCondPhase == "1" || strCondPhase == "3"))
                //{
                //    bolPhaseVal = true;                    
                //}
                ////A-4,B-2,C-1
                //else if (strDevPhase == strCondPhase)
                //{
                //    bolPhaseVal = true;
                //}
                //else if (strDevPhase == "7")
                //{
                //    bolPhaseVal = true;
                //}
                //else
                //{
                //    bolPhaseVal = false;
                //}


                //COND_FC_NAME,COND_GLOBALID,COND_PHASE,FC_NAME,TRANSFORMER_GLOBALID,TRANSFORMER_PHASE,REMARKS
                if (bolPhaseVal == false)
                {
                    //if (strDevPhase == "6" || strDevPhase == "5" || strDevPhase == "3")
                    //{

                    //    _clsGlobalFunctions.Common_addRowstoReportTable(objReportTable, pCondFeat.Class.AliasName + "," + pCondFeat.get_Value(pCondFeat.Fields.FindField("GLOBALID")).ToString() + "," + strCondPhase + "," + pdevFeat.Class.AliasName + "," + pdevFeat.get_Value(pdevFeat.Fields.FindField("GLOBALID")).ToString() + "," + strDevPhase + "," + strFeedpathCnt + ",Conductor phase wrong.Manually need to check");
                    //}
                    //else
                    //{
                        _clsGlobalFunctions.Common_addRowstoReportTable(objReportTable, pCondFeat.Class.AliasName + "," + pCondFeat.get_Value(pCondFeat.Fields.FindField("GLOBALID")).ToString() + "," +strCondCircuitID+","+ strCondPhase + "," + pdevFeat.Class.AliasName + "," + pdevFeat.get_Value(pdevFeat.Fields.FindField("GLOBALID")).ToString() + "," + strDevPhase + "," + strFeedpathCnt + ",Conductor phase wrong");
                    //}
                }


                return bolPhaseVal;             

            }
            catch (Exception ex)
            {
                Logs.writeLine("Error at ValidatePhase #" + ex.Message);
                return bolPhaseVal;
            }
        }


        // function to add whereclauses into cmbWhereClause comboBox
         private void cmbAllFC_SelectedIndexChanged(object sender, EventArgs e)
         {

             if (txt_ManualCls.Text == "")
             {
                 IFeatureClass pFC = null;
                 IFeatureCursor pFCursor = null;
                 IQueryFilter pFilter = new QueryFilterClass();
                 IFeature pfeat = null;
                 int intRecCount = 0;
                 int intMin = 0;
                 int intMax = 0;
                 int iterationCount = 0;
                 int intempMaxoid = 0;
                 List<string> lstString = new List<string>();

                 try
                 {
                     lstString.Clear();
                     cmbWhereClause.Enabled = true;
                     stbStatus.Text = "Processing wait... adding Where Clause..it take couple of mints ";
                     Application.DoEvents();

                     btnRun.Enabled = false;
                     btnClose.Enabled = false;

                     pFC = clsTestWorkSpace.FeatureWorkspace.OpenFeatureClass(cmbAllFC.SelectedItem.ToString());

                     pFilter.WhereClause = "objectid is not null order by objectid ";
                     pFCursor = pFC.Search(pFilter, false);
                     intRecCount = pFC.FeatureCount(null);


                     IFeature pFeat = pFCursor.NextFeature();

                     while (pFeat != null)
                     {
                         stbStatus.Text = pFeat.OID.ToString();
                         Application.DoEvents();
                         intempMaxoid = pFeat.OID;
                         if (iterationCount == 0)
                         {
                             intMin = pFeat.OID;
                         }
                         else if (iterationCount == 50000)
                         {
                             intMax = pFeat.OID;
                         }
                         iterationCount++;

                         if (iterationCount > 50000)
                         {
                             lstString.Add("Objectid > " + intMin + " and Objectid < " + intMax);
                             cmbWhereClause.Items.Add("Objectid > " + intMin + " and Objectid < " + intMax);
                             iterationCount = 0;
                         }


                         pFeat = pFCursor.NextFeature();

                     }
                     if (iterationCount > 0 && iterationCount < 50000)
                     {
                         lstString.Add("Objectid > " + intMin + " and Objectid < " + intempMaxoid);
                         cmbWhereClause.Items.Add("Objectid > " + intMin + " and Objectid < " + intempMaxoid);
                     }
                     cmbWhereClause.Text = lstString[0];


                     stbStatus.Text = "Select WhereClause and click Run button";
                     Application.DoEvents();
                     btnRun.Enabled = true;
                     btnClose.Enabled = true;

                 }
                 catch (Exception ex)
                 {
                     Logs.writeLine("Error @ cmbAllFC_SelectedIndexChanged  " + ex.Message);
                 }
                 finally
                 {
                     if (pFCursor != null)
                     {
                         Marshal.ReleaseComObject(pFCursor);
                     }
                 }
             }
         }





   
         // function to get feature from the network based on eid and elementtype
         private IFeature GetFeature( int eid, esriElementType elType,IGeometricNetwork geoNetwork)
         {
             if (eid == 0) return null;

             INetElements netElements = (INetElements)geoNetwork.Network;

             int classID, userID, subID;
             netElements.QueryIDs(eid, elType, out classID, out userID, out subID);

             IFeatureClass fclass = ((IFeatureClassContainer)geoNetwork).get_ClassByID(classID);

             //Logs.writeLine("FeatureClass:" + fclass.AliasName + ":," + userID.ToString() + "EleType:" + elType.ToString() + ",Eid:" + eid.ToString());
             return fclass.GetFeature(userID);
         }

         private void frmTrans_DoenstreamPhaseChk_Load(object sender, EventArgs e)
         {

             cmbAllFC.Items.Add("EDGIS.SecOHConductor");
             cmbAllFC.Items.Add("EDGIS.SecUGConductor");
         }
    }
}
