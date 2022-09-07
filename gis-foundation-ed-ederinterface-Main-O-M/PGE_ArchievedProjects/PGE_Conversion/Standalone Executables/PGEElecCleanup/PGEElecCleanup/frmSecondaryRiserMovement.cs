using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Geodatabase;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geometry;
using Miner.Interop;
using ESRI.ArcGIS.Carto;
using System.Collections;

namespace PGEElecCleanup
{
    public partial class frmSecondaryRiserMovement : Form
    {
        private DataTable objReportTable = new DataTable();
        clsGlobalFunctions _clsGlobalFunctions = new clsGlobalFunctions();
        IFeatureClass pRiserFCls = null;
        int intStructGUID = -1;
        private IMMAutoUpdater autoupdater = null;

        public frmSecondaryRiserMovement()
        {
            InitializeComponent();
        }

        private void frmSecondaryRiserMovement_Load(object sender, EventArgs e)
        {
            txtDistance.Text = "9.5";
            pRiserFCls = clsTestWorkSpace.FeatureWorkspace.OpenFeatureClass("EDGIS.SecondaryRiser");
            intStructGUID = pRiserFCls.Fields.FindField("STRUCTUREGUID");
            if (pRiserFCls == null)
            {
                MessageBox.Show("Unable to get SecondaryRiser featureclass..!");
                return;
            }
            if (intStructGUID == -1)
            {
                MessageBox.Show("STRUCTUREGUID field doesnot exists in SecondaryRiser featureclass..!");
                return;
            }
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            IQueryFilter pQryFltr = new QueryFilterClass();
            IFeatureCursor pRiserCurs = null;
            IFeature pRsrFeat = null;
            int intTotalFeatCnt = 0;
            int intProcCnt = 0;
            int intItrtnCnt = 0;

            try
            {

                btnExecute.Enabled = false;
                btnExit.Enabled = false;
                objReportTable.Clear();
                objReportTable.Columns.Clear();
                _clsGlobalFunctions.Common_initSummaryTable("SecondaryRiserMovemntFromSprtStruct", "SecondaryRiserMovemntFromSprtStruct");
                _clsGlobalFunctions.Common_addColumnToReportTable(objReportTable, "SecondaryRiser_OID,SupportStruct_OID,CURRENTDIST,REMARKS");

                if (string.IsNullOrEmpty(txtDistance.Text))
                {
                    MessageBox.Show("Enter the distance to which the Riser should move");
                    return;
                }
                stbStatus.Text = "Starting Process..";

                stbStatus.Text = "Disable Autoupdaters ";
                #region "Disable Autoupdaters "
                mmAutoUpdaterMode objautoupdateroldmode = DisableAutoupdaters();
                #endregion

                //stbStatus.Text = "Stopping Auto Creation of Annotations in Destination database ";
                //#region "Stop Auto Creation of Annotations in Destination database"
                //IEnumDataset DestinationEnumdataset = clsTestWorkSpace.Workspace.get_Datasets(esriDatasetType.esriDTAny);
                ////Loop through all the Destination datasets to append the data 
                //IDataset Destinationdataset = DestinationEnumdataset.Next();
                //ArrayList objarraylist = null;
                //while (Destinationdataset != null)
                //{
                //    Application.DoEvents();
                //    AutoAnotationcreate(Destinationdataset, false, ref objarraylist);
                //    Destinationdataset = DestinationEnumdataset.Next();
                //}
                //#endregion


                clsTestWorkSpace.StartEditOperation();

                //pQryFltr.WhereClause = "STRUCTUREGUID IS NOT NULL";

                //To get all SecondaryRiser features
                IFeatureClass pSuprtStructFCls = clsTestWorkSpace.FeatureWorkspace.OpenFeatureClass("EDGIS.SupportStructure");

                IQueryFilter pQF = new QueryFilterClass();
                //pQF.WhereClause ="objectid in (2916579,27074,2916580,1033,34266,164594,10368,12117,13381,23022,27529,27933,28387)";
                //pQF.WhereClause = "objectid in (33554)";

                //intTotalFeatCnt = pRiserFCls.FeatureCount(pQF);
                //pRiserCurs = pRiserFCls.Search(pQF, false);

                intTotalFeatCnt = pRiserFCls.FeatureCount(null);
                pRiserCurs = pRiserFCls.Search(null, false);
                pRsrFeat = pRiserCurs.NextFeature();
                double dbRsrDistance = double.Parse(txtDistance.Text.ToString());
                while (pRsrFeat != null)
                {
                    intProcCnt++;
                    intItrtnCnt++;
                    stbStatus.Text = "Processing Riser features " + intProcCnt + "/" + intTotalFeatCnt;
                    Application.DoEvents();

                    findRelatedSupportStructure(pRsrFeat, pSuprtStructFCls, dbRsrDistance);

                    if (intItrtnCnt == 1000)
                    {
                        clsTestWorkSpace.StopEditOperation(true);
                        clsTestWorkSpace.StartEditOperation();
                        intItrtnCnt = 0;
                    }

                    pRsrFeat = pRiserCurs.NextFeature();
                }
                clsTestWorkSpace.StopEditOperation(true);


                stbStatus.Text = "Enabling autoupdaters...";
                #region start AU
                if (autoupdater != null)
                {
                    autoupdater.AutoUpdaterMode = objautoupdateroldmode;
                }
                #endregion

                clsGFMSGlobalFunctions._GFMSGlobalFunctions.GenerateTheReport_MultipleFeatureClass(objReportTable, "SecondaryRiserMovemntFromSprtStruct");

                stbStatus.Text = "Process completed see the log file.";
                Logs.writeLine("Successfully Completed");
                MessageBox.Show("Process Completed, please see the Report File.", "Report", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Logs.writeLine("Error in btnExecute_Click() :" + ex.ToString());//("EXCP@btnExecute_Click " + ex.Message); 
            }
            finally
            {
                if (pRiserCurs != null)
                {
                    Marshal.ReleaseComObject(pRiserCurs);
                    pRiserCurs = null;
                }
                if (pQryFltr != null)
                {
                    Marshal.ReleaseComObject(pQryFltr);
                    pQryFltr = null;
                }
                btnExecute.Enabled = true;
                btnExit.Enabled = true;
            }
        }

        private void findRelatedSupportStructure(IFeature pRiserFeature, IFeatureClass pSuprtStructFCls, double dbRsrDistance)// string strStructGUID)
        {
            //IFeatureClass pSuprtStructFCls = null;
            IFeatureCursor pSuprtStructFCurs = null;
            IFeature pSuprtStructFeat = null;
            IQueryFilter pQF = new QueryFilterClass();
            IPoint pStructPnt = new PointClass();
            IPoint pRsrPnt = new PointClass();

            try
            {
                string strStructGUID = pRiserFeature.get_Value(intStructGUID).ToString();
                if (strStructGUID == "")
                {

                    getStructure_AndRelate(pSuprtStructFCls, pRiserFeature, dbRsrDistance);

                    //_clsGlobalFunctions.Common_addRowstoReportTable(objReportTable, pRiserFeature.OID.ToString() + ",,Riser not related any structure"); 

                }
                else if (pRiserFeature.Shape != null)
                {
                    pRsrPnt = pRiserFeature.Shape as IPoint;

                    pQF.WhereClause = "GLOBALID = '" + strStructGUID + "'";
                    //pSuprtStructFCls = clsTestWorkSpace.FeatureWorkspace.OpenFeatureClass("EDGIS.SupportStructure");
                    //To find the SupportStructure related to the SecondaryRiser 
                    pSuprtStructFCurs = pSuprtStructFCls.Search(pQF, false);
                    pSuprtStructFeat = pSuprtStructFCurs.NextFeature();
                    while (pSuprtStructFeat != null)
                    {
                        if (pSuprtStructFeat.Shape != null)
                        {
                            pStructPnt = pSuprtStructFeat.Shape as IPoint;

                            ILine pLine = new LineClass();
                            pLine.PutCoords(pStructPnt, pRsrPnt);
                            //report for manual review if riser > 25 from its related structure
                            if (pLine.Length > 25.0)
                            {
                                _clsGlobalFunctions.Common_addRowstoReportTable(objReportTable, pRiserFeature.OID.ToString() + "," + pSuprtStructFeat.OID.ToString() + "," + pLine.Length.ToString() + ",Distance between Riser to related structure is not valid.Manual check required");

                            }
                            else if (pLine.Length < 9.49 || pLine.Length > 9.51) //(pLine.Length != dbRsrDistance)
                            {

                                //To get the Angle between SupportStructure and Riser
                                double dblAng1e = GetAngleBetweenPoints(pStructPnt, pRsrPnt);

                                IPoint pNewPnt = new PointClass();
                                //To get the Projection Point
                                pNewPnt = getProjectionPoint(pStructPnt, dblAng1e, dbRsrDistance);

                                //getDist(pRiserFeature, (IPoint)pSuprtStructFeat.Shape, pNewPnt);

                                IGeometry pNewGeom = pNewPnt as IGeometry;
                                //pRiserFeature.Shape = pNewGeom;//Assigning the new geometry to riser
                                pRiserFeature.Shape = pNewPnt;//Assigning the new geometry to riser
                                pRiserFeature.Store();

                                _clsGlobalFunctions.Common_addRowstoReportTable(objReportTable, pRiserFeature.OID.ToString() + "," + pSuprtStructFeat.OID.ToString() + "," + pLine.Length.ToString() + ",moved to 9.5ft from structure");
                            }
                            else
                            {
                            }
                        }
                        else
                            Logs.writeLine("Support Structure geometry is null for the feature with OID:" + pRiserFeature.OID.ToString());


                        pSuprtStructFeat = pSuprtStructFCurs.NextFeature();
                    }
                }
                else
                    Logs.writeLine("Riser geometry is null for the feature with OID:" + pRiserFeature.OID.ToString());
            }
            catch (Exception ex)
            {
                Logs.writeLine("Error in btnExecute_Click() :" + ex.ToString());//("EXCP@btnExecute_Click " + ex.Message); 
            }
            finally
            {
                if (pSuprtStructFCurs != null)
                {
                    Marshal.ReleaseComObject(pSuprtStructFCurs);
                    pSuprtStructFCurs = null;
                }
                if (pQF != null)
                {
                    Marshal.ReleaseComObject(pQF);
                    pQF = null;
                }
            }
        }

        private void getStructure_AndRelate(IFeatureClass pSuprtStructFCls, IFeature pRiserFeat, double dbRsrDistance)
        {
            int intBuffCount = 0;
            ISpatialFilter pSptlFilter = new SpatialFilterClass();
            IFeatureCursor pFCursor = null;
            IFeature pSuprtStructFeat = null;
            string strStructGUID = string.Empty;

            string strGuid = string.Empty;
            try
            {
                using (ESRI.ArcGIS.ADF.ComReleaser cr = new ESRI.ArcGIS.ADF.ComReleaser())
                {
                    IBufferConstruction pBuffConst = new BufferConstructionClass();
                    IGeometry pBuffGeom = pBuffConst.Buffer((IPoint)pRiserFeat.Shape, 15.0);
                    pSptlFilter.Geometry = (IGeometry)pBuffGeom;
                    pSptlFilter.GeometryField = pSuprtStructFCls.ShapeFieldName;
                    pSptlFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                    intBuffCount = pSuprtStructFCls.FeatureCount(pSptlFilter);
                    pFCursor = pSuprtStructFCls.Search(pSptlFilter, false);
                    cr.ManageLifetime(pSptlFilter);
                    cr.ManageLifetime(pFCursor);

                    if (intBuffCount == 1)
                    {
                        pSuprtStructFeat = pFCursor.NextFeature();

                        ILine pLine = new LineClass();
                        pLine.PutCoords((IPoint)pSuprtStructFeat.Shape, (IPoint)pRiserFeat.Shape);


                        //To get the Angle between SupportStructure and Riser
                        double dblAng1e = GetAngleBetweenPoints((IPoint)pSuprtStructFeat.Shape, (IPoint)pRiserFeat.Shape);
                        IPoint pNewPnt = new PointClass();
                        //To get the Projection Point
                        pNewPnt = getProjectionPoint((IPoint)pSuprtStructFeat.Shape, dblAng1e, dbRsrDistance);
                        strStructGUID = pSuprtStructFeat.get_Value(pSuprtStructFeat.Fields.FindField("GLOBALID")).ToString();

                        if (pLine.Length < 9.49 || pLine.Length > 9.51)
                        {
                            //build relation
                            //getDist(pRiserFeat, (IPoint)pSuprtStructFeat.Shape, pNewPnt);
                            pRiserFeat.set_Value(intStructGUID, strStructGUID);
                            pRiserFeat.Shape = pNewPnt;//Assigning the new geometry to riser
                            pRiserFeat.Store();

                            _clsGlobalFunctions.Common_addRowstoReportTable(objReportTable, pRiserFeat.OID.ToString() + "," + strStructGUID + "," + pLine.Length.ToString() + ",Relation build with nearest structure and moved to 9.5ft from structure");
                        }
                    }
                    else if (intBuffCount > 1)
                    {
                        _clsGlobalFunctions.Common_addRowstoReportTable(objReportTable, pRiserFeat.OID.ToString() + "," + strStructGUID + ",,Riser not related with any structure and more than one structure found with in 15.0ft.Manual check required");
                    }
                    else
                    {
                        _clsGlobalFunctions.Common_addRowstoReportTable(objReportTable, pRiserFeat.OID.ToString() + "," + strStructGUID + ",,Riser not related with any structure and there is no structure with in 15.0ft to relate.Manual check required");
                    }
                }
            }
            catch (Exception ex)
            {
                Logs.writeLine("EXCP@getStructure_AndRelate. Riser_Oid: " + pRiserFeat.OID + " " + ex.Message);
                intBuffCount = 0;
            }
        }

        private void getDist(IFeature pRiserFeat, IPoint pPnt, IPoint pNewPnt)
        {
            ILine pLine = new LineClass();
            pLine.PutCoords(pPnt, pNewPnt);

            Logs.writeLine("Dist: " + pLine.Length);
        }

        internal static double GetAngleBetweenPoints(IPoint pStrtPnt, IPoint pEndPnt)
        {
            double x1 = pStrtPnt.X;
            double y1 = pStrtPnt.Y;
            double x2 = pEndPnt.X;
            double y2 = pEndPnt.Y;
            double pxRes = x2 - x1;
            double pyRes = y2 - y1;
            double angle = 0.0;
            // Calculate the angle 
            if (pxRes == 0.0)
            {
                if (pyRes == 0.0)
                    angle = 0.0;
                else if (pyRes > 0.0) angle = System.Math.PI / 2.0;
                else

                    angle = System.Math.PI * 3.0 / 2.0;
            }
            else if (pyRes == 0.0)
            {
                if (pxRes > 0.0)
                    angle = 0.0;
                else
                    angle = System.Math.PI;
            }
            else
            {
                if (pxRes < 0.0)
                    angle = System.Math.Atan(pyRes / pxRes) + System.Math.PI;

                else if (pyRes < 0.0) angle = System.Math.Atan(pyRes / pxRes) + (2 * System.Math.PI);
                else

                    angle = System.Math.Atan(pyRes / pxRes);
            }
            // Convert to degrees 
            //angle = angle * 180 / System.Math.PI; 
            return angle;
        }

        internal static IPoint getProjectionPoint(IPoint pInputPnt, double Inputangle, double distance)
        {
            IPoint pOutputPnt = new PointClass();

            try
            {
                pOutputPnt.PutCoords(pInputPnt.X + (distance * Math.Cos(Inputangle)), pInputPnt.Y + (distance * Math.Sin(Inputangle)));

                return pOutputPnt;
            }
            catch (Exception ex)
            {
                throw ex;
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
            autoupdater.AutoUpdaterMode = mmAutoUpdaterMode.mmAUMNoEvents;
            // insert code that needs to execute while autoupdaters 
            return oldMode;
        }

        /// <summary>
        /// This method is to find the feature class and set the auto anotation create false 
        /// </summary>
        /// <param name="pdestinationdataset"></param>
        private void AutoAnotationcreate(IDataset pdestinationdataset, Boolean status, ref ArrayList strAnnoclass)
        {
            IFeatureClass objfeatureclass = null;
            IAnnoClassAdmin2 objannotation = null;
            IEnumDataset Enumdataset = null;
            IDataset enudataset = null;

            try
            {
                switch (pdestinationdataset.Type)
                {
                    case esriDatasetType.esriDTFeatureDataset:
                        {
                            Enumdataset = pdestinationdataset.Subsets;
                            enudataset = Enumdataset.Next();
                            while (enudataset != null)
                            {
                                if (enudataset.Type == esriDatasetType.esriDTFeatureClass)
                                {
                                    objfeatureclass = clsGlobalFunctions._globalFunctions.getFeatureclassByName(clsTestWorkSpace.Workspace, enudataset.Name);
                                    if (objfeatureclass.FeatureType == esriFeatureType.esriFTAnnotation)
                                    {

                                        objannotation = (IAnnoClassAdmin2)objfeatureclass.Extension;
                                        objannotation.AutoCreate = status;

                                        objannotation.UpdateProperties();
                                    }
                                }
                                enudataset = Enumdataset.Next();
                            }
                            break;
                        }
                    case esriDatasetType.esriDTFeatureClass:
                        {
                            objfeatureclass = (IFeatureClass)pdestinationdataset;
                            if (objfeatureclass.FeatureType == esriFeatureType.esriFTAnnotation)
                            {
                                objannotation = (IAnnoClassAdmin2)objfeatureclass.Extension;
                                objannotation.AutoCreate = status;
                                objannotation.UpdateProperties();
                            }
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
            catch
            {
                Logs.writeLine("Error occured in AutoAnotationcreate method ");
            }
        }


        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }


    }
}
