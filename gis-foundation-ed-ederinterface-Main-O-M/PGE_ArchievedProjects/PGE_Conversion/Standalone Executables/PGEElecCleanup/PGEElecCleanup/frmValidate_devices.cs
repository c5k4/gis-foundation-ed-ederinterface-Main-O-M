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
    public partial class frmValidate_devices : Form
    {

        private IMMAutoUpdater autoupdater = null;
        private DataTable objReportTable = new DataTable();
        clsGlobalFunctions _clsGlobalFunctions = new clsGlobalFunctions();

        public frmValidate_devices()
        {
            InitializeComponent();
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            try
            {
                btnExecute.Enabled = false;
                btnExit.Enabled = false;

                stbInfo.Text = "Starting Process..";

                //stbInfo.Text = "Disable Autoupdaters ";
                //#region "Disable Autoupdaters "
                //mmAutoUpdaterMode objautoupdateroldmode = DisableAutoupdaters();
                //#endregion


                string strFCName = string.Empty;
                string strTolDist = txtDistTol.Text.ToString();
                objReportTable.Clear();
                objReportTable.Columns.Clear();
                
                _clsGlobalFunctions.Common_addColumnToReportTable(objReportTable, "FCNAME,GLOBALID,DISTINCE,REMARKS");


                //if (strFCName == "")
                //{
                //    MessageBox.Show("Please enter featureclass name and try again", "Report", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //    btnExecute.Enabled = true;
                //    btnExit.Enabled = true;
                //    return;
                //}
                if (strTolDist == "")
                {
                    MessageBox.Show("Please enter distance and try again", "Report", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    btnExecute.Enabled = true;
                    btnExit.Enabled = true;
                    return;
                }

                TreeNodeCollection treeNodesCol = treeViewTasks.Nodes;
                List<string> lstFCNames = new List<string>();

                string strSectionName = string.Empty ;
                foreach (TreeNode node in treeNodesCol)
                {
                    strSectionName = node.Text;
                    TreeNodeCollection chldCol = node.Nodes;  

                    for (int itr = 0; itr < chldCol.Count; itr++)
                    {
                        TreeNode chldnode = node.Nodes[itr];

                        if (chldnode.Checked == true)
                        {
                            lstFCNames.Add(chldnode.Text);
                        }
                    }
                }

                for (int itr = 0; itr < lstFCNames.Count; itr++)
                {
                    validateData(lstFCNames[itr], strTolDist);
                }

                //stbInfo.Text = "Enabling autoupdaters...";
                //#region start AU
                //if (autoupdater != null)
                //{
                //    autoupdater.AutoUpdaterMode = objautoupdateroldmode;
                //}
                //#endregion

                clsGFMSGlobalFunctions._GFMSGlobalFunctions.GenerateTheReport_MultipleFeatureClass(objReportTable, "Validate@features");
                MessageBox.Show("Process Completed, please see the Report File.", "Report", MessageBoxButtons.OK, MessageBoxIcon.Information);
                stbInfo.Text = "Process completed see the log file.";
                Logs.writeLine("Successfully Completed");

                btnExecute.Enabled = true;
                btnExit.Enabled = true;
            }
            catch (Exception ex)
            {
                btnExecute.Enabled = true;
                btnExit.Enabled = true;

                Logs.writeLine("Error on process start button " + ex.Message);
                stbInfo.Text = "Error occurred, Please see the log file.";
            }
            finally
            {

            }
        }

        private void validateData(string strFCName, string strTolDist)
        {
            IFeatureClass pfc = null;
            IFeature pFeat = null;
            IFeatureCursor pFCursor = null;
            IQueryFilter pFilter = new QueryFilterClass();
            int intTotCount = 0;
            int iterationCnt = 0;
            int intFeatcount = 0;
            string strGlobalId = string.Empty;
            double dblDist = 0.0;

            try
            {
                pfc = clsTestWorkSpace.FeatureWorkspace.OpenFeatureClass(strFCName);

                if (pfc == null)
                {
                    MessageBox.Show("Could not able to read feature calss" + strFCName, "Report", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                using (ESRI.ArcGIS.ADF.ComReleaser cr = new ESRI.ArcGIS.ADF.ComReleaser())
                {
                    //pFilter.WhereClause = "globalid ='{CC2C459E-5A8D-4AA3-80A4-597F9979C61B}'";

                    //intTotCount = pfc.FeatureCount(pFilter);
                    //pFCursor = pfc.Search(pFilter, false);

                    intTotCount = pfc.FeatureCount(null);
                    pFCursor = pfc.Search(null, false);
                    cr.ManageLifetime(pFCursor);
                    pFeat = pFCursor.NextFeature();
                    dblDist = Convert.ToDouble(strTolDist);
                    while (pFeat != null)
                    {
                        strGlobalId = pFeat.get_Value(pFeat.Fields.FindField("GLOBALID")).ToString();
                        try
                        {
                            iterationCnt++;
                            stbInfo.Text = "Processing...  " + strFCName+" " + iterationCnt + " from " + intTotCount;
                            Application.DoEvents();                            
                            intFeatcount = 0;
                            GetFeature_Cnt_At_SameLocation(pfc, (IPoint)pFeat.Shape, ref intFeatcount, dblDist);
                            if (intFeatcount > 1)
                            {
                                _clsGlobalFunctions.Common_addRowstoReportTable(objReportTable, strFCName + "," + strGlobalId + ","+dblDist+", Having more than one feature within tolerance distance");
                            }
                        }
                        catch (Exception ex)
                        {
                            Logs.writeLine("Error @ validateData. " + strGlobalId);

                        }
                        pFeat = pFCursor.NextFeature();

                        //if (iterationCnt == 100)
                        //{
                        //    pFeat = null;
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                if (pFCursor != null)
                {
                    Marshal.ReleaseComObject(pFCursor);
                }
            }
        }

        private void GetFeature_Cnt_At_SameLocation(IFeatureClass pfc, IPoint pPnt, ref int intFeatcount, double dblDist)
        {
            intFeatcount = 0;
            ISpatialFilter pSptlFilter = new SpatialFilterClass();
            IBufferConstruction pBuffConst = null;
            IGeometry pBuffGeom = null;
            string strGuid = string.Empty;
            IFeatureCursor pFCursor = null;
            //IFeature pFeat = null ;
            string strGlobalId = string.Empty;
            try
            {
                dblDist = 0.0;
                using (ComReleaser cr = new ComReleaser())
                {
                    if (dblDist != 0.0)
                    {
                        pBuffConst = new BufferConstructionClass();
                        pBuffGeom = pBuffConst.Buffer(pPnt, dblDist);
                        pSptlFilter.Geometry = (IGeometry)pBuffGeom;
                    }
                    else
                    {
                        pSptlFilter.Geometry = (IGeometry)pPnt;                    
                    }
                    
                    pSptlFilter.GeometryField = pfc.ShapeFieldName;
                    pSptlFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                    intFeatcount = pfc.FeatureCount(pSptlFilter);
                    pFCursor = pfc.Search(pSptlFilter, false);
                    cr.ManageLifetime(pFCursor);

                    //pFeat = pFCursor.NextFeature();
                    //while (pFeat != null)
                    //{
                    //    strGlobalId = pFeat.get_Value(pFeat.Fields.FindField("GLOBALID")).ToString();
                    //    pFeat = pFCursor.NextFeature();

                    //}
                }
            }
            catch (Exception ex)
            {
                Logs.writeLine("EXCP@getFeatCountInbuffer " + ex.Message);
                intFeatcount = 0;
            }
            finally
            {
                if (pSptlFilter != null)
                {
                    Marshal.ReleaseComObject(pSptlFilter);
                }
                if (pFCursor != null)
                {
                    Marshal.ReleaseComObject(pFCursor);
                }
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmValidate_devices_Load(object sender, EventArgs e)
        {
            List<string> lstEDClassNames = new List<string>();
            List<string> lstSUBClassNames = new List<string>();
            TreeNode trNode;
            _clsGlobalFunctions.Common_initSummaryTable("Validate@features", "Validate@features");

            try
            {

                addFeatClassNam_toLst(ref lstEDClassNames, ref lstSUBClassNames);
                

                TreeNode[] arrTreeNode = new TreeNode[lstEDClassNames.Count];
                for (int iitr = 0; iitr < lstEDClassNames.Count; iitr++)
                {
                    //child nodes
                    trNode = new TreeNode(lstEDClassNames[iitr].Trim());
                    arrTreeNode[iitr] = trNode;
                }

                TreeNode treeNode = new TreeNode("EDGIS.ElectricDataset", arrTreeNode);
                treeViewTasks.Nodes.Add(treeNode);
                /////////////
                TreeNode[] arrTreeNode1 = new TreeNode[lstSUBClassNames.Count];
                for (int iitr = 0; iitr < lstSUBClassNames.Count; iitr++)
                {
                    //child nodes
                    trNode = new TreeNode(lstSUBClassNames[iitr].Trim());
                    arrTreeNode1[iitr] = trNode;
                }

                TreeNode treeNode1 = new TreeNode("EDGIS.SubstationDataset", arrTreeNode1);
                treeViewTasks.Nodes.Add(treeNode1);
            }
            catch (Exception ex)
            {
                Logs.writeLine("EXCP@frmValidate_devices_Load " + ex.Message);                
            }
        }


        private void addFeatClassNam_toLst(ref List<string> lstPtFeatclassNames, ref List<string> lstSUBClassNames)
        {
            try
            {
                IEnumDataset pEnumDSname = clsTestWorkSpace.Workspace.get_Datasets(esriDatasetType.esriDTFeatureDataset);
                IDataset objFDS = pEnumDSname.Next();
                while (objFDS != null)
                {
                    if (objFDS.Name == "EDGIS.ElectricDataset" || objFDS.Name == "EDGIS.SubstationDataset")
                    {
                        IFeatureDataset pFDataset = (IFeatureDataset)objFDS;
                        IEnumDataset penumDS = pFDataset.Subsets;
                        IDataset dataset = penumDS.Next();
                        while (dataset != null)
                        {
                            if (dataset.Type == esriDatasetType.esriDTFeatureClass)
                            {
                                IFeatureClass pfeatureclass = (IFeatureClass)dataset;
                                //IFeatureClass pfeatureclass = clsGlobalFunctions._globalFunctions.getFeatureclassByName(clsTestWorkSpace.Workspace, dataset.Name);
                                if (pfeatureclass.ShapeType == esriGeometryType.esriGeometryPoint && objFDS.Name.ToUpper() == "EDGIS.ElectricDataset".ToUpper())
                                {
                                    lstPtFeatclassNames.Add(dataset.Name);                                    
                                }
                                else if (pfeatureclass.ShapeType == esriGeometryType.esriGeometryPoint && objFDS.Name.ToUpper() == "EDGIS.SubstationDataset".ToUpper())
                                {
                                    lstSUBClassNames.Add(dataset.Name); 
                                }
                            }
                            dataset = penumDS.Next();
                        }
                    }
                    objFDS = pEnumDSname.Next();
                }                
            }
            catch (Exception ex)
            {
                Logs.writeLine("Error @ addFeatClassNam_toLst " + ex);
            }
        }

        private void treeViewTasks_MouseClick(object sender, MouseEventArgs e)
        {
            
        }

        private void treeViewTasks_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            ManageTreeChecked(e.Node);
        }

        private void ManageTreeChecked(TreeNode node)
        {
            foreach (TreeNode n in node.Nodes)
            {
                n.Checked = node.Checked;
            }
        }
    }
}
