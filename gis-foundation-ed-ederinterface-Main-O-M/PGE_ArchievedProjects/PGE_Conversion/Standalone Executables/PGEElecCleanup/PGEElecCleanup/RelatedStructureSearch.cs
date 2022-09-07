using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Geodatabase;
using System.Collections;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;
using System.Threading;
using System.IO;
using System.Configuration;

namespace PGEElecCleanup
{
    /// <summary>
    /// This tool search for related structure from list of feature classes.
    /// </summary>
    public partial class RelatedStructureSearch : Form
    {
        private List<KeyValuePair<IFeatureClass, IFeatureClass>> sortedList = new List<KeyValuePair<IFeatureClass, IFeatureClass>>();
        private Dictionary<KeyValuePair<IFeatureClass, IFeatureClass>, IRelationshipClass> featureClassRelationshipDic = new Dictionary<KeyValuePair<IFeatureClass, IFeatureClass>, IRelationshipClass>();
        private static System.Data.DataTable _dataTable = new System.Data.DataTable();
        bool reportSuccess = false;

        public RelatedStructureSearch()
        {
            InitializeComponent();
            try
            {
                #region DataTable Adding Code
                _dataTable.Clear();
                _dataTable.Columns.Clear();
                DataRow dataRow = _dataTable.NewRow();
                _dataTable.Columns.Add("Feature Class");
                _dataTable.Columns.Add("Subtype");
                _dataTable.Columns.Add("GlobalIDFC");
                _dataTable.Columns.Add("Related Structure");
                _dataTable.Columns.Add("GlobalIDRelatedClass");
                _dataTable.Columns.Add("Distance");

                dataRow[0] = "Feature Class";
                dataRow[1] = "Subtype";
                dataRow[2] = "GlobalID_FC";
                dataRow[3] = "Related Structure";
                dataRow[4] = "GlobalID_RelatedClass";
                dataRow[5] = "Distance";
                _dataTable.Rows.Add(dataRow);

                #endregion
            }
            catch (Exception ex)
            {
                Logs.writeLine("Error while loading the feature classes: " + ex.Message);
            }
        }

        private void btnLoadFeatureClass_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;                
                btnLoadFeatureClass.Enabled = false;
                buttonGenerateReport.Enabled = false;
                backgroundWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                Logs.writeLine("Error while loading the feature classes: " + ex.Message);
            }
            finally
            {
                this.Cursor = Cursors.Default;
                btnLoadFeatureClass.Enabled = true;
                buttonGenerateReport.Enabled = true;
            }
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {                
                featureClassRelationshipDic.Clear();
                checkedListBoxSelectedFeature.Nodes.Clear();
                sortedList.Clear();
                int totalSteps = 520, count = 1, progress = 0;
                backgroundWorker.ReportProgress(1);
                IWorkspace featureWorkspace = clsTestWorkSpace.FeatureWorkspace as IWorkspace;
                IEnumDataset enumDataSet = featureWorkspace.get_Datasets(esriDatasetType.esriDTAny);
                enumDataSet.Reset();
                IDataset featureDataSet = null;
                while ((featureDataSet = enumDataSet.Next()) != null)
                {
                    Application.DoEvents();
                    if (featureDataSet.Type == esriDatasetType.esriDTFeatureClass)
                    {
                        progress = count * 100 / totalSteps;
                        backgroundWorker.ReportProgress(progress);
                        count++;
                        IFeatureClass featureClass = featureDataSet as IFeatureClass;
                        if (featureClass.FeatureType != esriFeatureType.esriFTAnnotation)
                        {
                            IEnumRelationshipClass enumRelationshipClass = featureClass.get_RelationshipClasses(esriRelRole.esriRelRoleAny);
                            IRelationshipClass relationshipClass = null;
                            while ((relationshipClass = enumRelationshipClass.Next()) != null)
                            {
                                IFeatureClass originFeatureClass = relationshipClass.OriginClass as IFeatureClass;
                                if (originFeatureClass != null && originFeatureClass.FeatureType != esriFeatureType.esriFTAnnotation && originFeatureClass != featureClass)
                                {
                                    sortedList.Add(new KeyValuePair<IFeatureClass, IFeatureClass>(featureClass, originFeatureClass));
                                }
                                count++;
                            }
                        }
                    }
                    else if (featureDataSet.Type == esriDatasetType.esriDTFeatureDataset)
                    {
                        IEnumDataset enumDS = featureDataSet.Subsets;
                        enumDS.Reset();
                        while ((featureDataSet = enumDS.Next()) != null)
                        {
                            progress = count * 100 / totalSteps;
                            backgroundWorker.ReportProgress(progress);
                            count++;
                            if (featureDataSet.Type == esriDatasetType.esriDTFeatureClass)
                            {
                                IFeatureClass featureClass = featureDataSet as IFeatureClass;
                                if (featureClass.FeatureType != esriFeatureType.esriFTAnnotation)
                                {
                                    IEnumRelationshipClass enumRelationshipClass = featureClass.get_RelationshipClasses(esriRelRole.esriRelRoleAny);
                                    IRelationshipClass relationshipClass = null;
                                    while ((relationshipClass = enumRelationshipClass.Next()) != null)
                                    {
                                        IFeatureClass originFeatureClass = relationshipClass.OriginClass as IFeatureClass;
                                        if (originFeatureClass != null && originFeatureClass.FeatureType != esriFeatureType.esriFTAnnotation && originFeatureClass != featureClass)
                                        {
                                            sortedList.Add(new KeyValuePair<IFeatureClass, IFeatureClass>(featureClass, originFeatureClass));
                                            featureClassRelationshipDic.Add(new KeyValuePair<IFeatureClass, IFeatureClass>(featureClass, originFeatureClass), relationshipClass);
                                        }
                                        count++;
                                    }
                                }
                            }
                        }
                    }
                }
                backgroundWorker.ReportProgress(100);
            }
            catch (Exception ex)
            {
                Logs.writeLine("Error while loading the feature classes: " + ex.Message);
            }
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                if (e.ProgressPercentage < 101)
                {
                    progressBar.Value = e.ProgressPercentage;
                }
            }
            catch (Exception ex)
            {
                Logs.writeLine("Error while loading the feature classes: " + ex.Message);
            }
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                string featureClassname = string.Empty;
                TreeNode node = null;
                List<KeyValuePair<IFeatureClass, IFeatureClass>> sortedList2 = sortedList.OrderBy(X => X.Key.AliasName).ToList();
                foreach (var item in sortedList2)
                {
                    if (String.IsNullOrEmpty(featureClassname))
                    {
                        featureClassname = item.Key.AliasName;
                        node = checkedListBoxSelectedFeature.Nodes.Add(item.Key.AliasName, item.Key.AliasName);
                        node.Nodes.Add(item.Value.AliasName, item.Value.AliasName);
                    }
                    else if (featureClassname == item.Key.AliasName)
                    {
                        node.Nodes.Add(item.Value.AliasName, item.Value.AliasName);
                    }
                    else if (featureClassname != item.Key.AliasName)
                    {
                        featureClassname = item.Key.AliasName;
                        node = checkedListBoxSelectedFeature.Nodes.Add(item.Key.AliasName, item.Key.AliasName);
                        node.Nodes.Add(item.Value.AliasName, item.Value.AliasName);
                    }
                }
                progressBar.Value = 0;
            }
            catch (Exception ex)
            {
                Logs.writeLine("Error while loading the feature classes: " + ex.Message);
            }
        }

        private void backgroundWorkerGenerateReport_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                int totalSteps = checkedListBoxSelectedFeature.GetNodeCount(true), progress = 0, count = 0;
                _dataTable.Clear();
                foreach (TreeNode node in checkedListBoxSelectedFeature.Nodes)
                {
                    progress = count * 100 / totalSteps;
                    backgroundWorkerGenerateReport.ReportProgress(progress);
                    count++;
                    if (node.Checked)
                    {
                        foreach (TreeNode child in node.Nodes)
                        {
                            if (child.Checked)
                            {
                                var KVFeatureClass = sortedList.Where(X => (X.Key.AliasName == node.Text) && (X.Value.AliasName == child.Text));
                                DataTable expertInfo = new DataTable();
                                foreach (var fcItem in KVFeatureClass)
                                {
                                    var relationString = featureClassRelationshipDic.Where(X => (X.Key.Key == fcItem.Key) && (X.Key.Value == fcItem.Value));
                                    foreach (var item in relationString)
                                    {
                                        GenerateReport(fcItem.Key, fcItem.Value, item.Value);
                                    }
                                }
                            }
                        }
                        DataTable2CSV(_dataTable);
                        reportSuccess = true;
                    }
                }
                backgroundWorkerGenerateReport.ReportProgress(100);
                Thread.Sleep(10000);
            }
            catch (Exception ex)
            {
                Logs.writeLine("Error while generating the report " + ex.Message);
            }           
        }

        public void DataTable2CSV(System.Data.DataTable table)
        {
            StreamWriter writer = new StreamWriter(ConfigurationManager.AppSettings["ReportFilePath"] + "\\RelatedStructureSearch.csv");
            try
            {
                string sepraror = ",";
                System.Text.StringBuilder builder = new System.Text.StringBuilder();
                // then write all the rows
                foreach (DataRow row in table.Rows)
                {
                    builder = new System.Text.StringBuilder();
                    foreach (System.Data.DataColumn col in table.Columns)
                    {
                        builder.Append(row[col.ColumnName]).Append(sepraror);
                    }
                    writer.WriteLine(builder.ToString());
                }
            }
            catch (Exception ex)
            {
                Logs.writeLine("Debug error logging " + ex);
            }
            finally
            {
                if ((writer != null)) writer.Close();
            }
        }

        private void buttonGenerateReport_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                btnLoadFeatureClass.Enabled = false;
                buttonGenerateReport.Enabled = false;
                backgroundWorkerGenerateReport.RunWorkerAsync();
                buttonGenerateReport.Enabled = true;
            }
            catch (Exception ex)
            {
                Logs.writeLine("Error while generating the report " + ex.Message);
            }
            finally
            {
                this.Cursor = Cursors.Default;  
                btnLoadFeatureClass.Enabled = true;
            }
        }

        private void GenerateReport(IFeatureClass baseFeatureClass, IFeatureClass relatedTable, IRelationshipClass relationshipClass)
        {
            try
            {
                IQueryFilter filter = new QueryFilter();
                IFeatureCursor fcursor = baseFeatureClass.Search(filter, true);
                //lblTotalRecordProcessed.Text = "Currently processing " + _aliasName;
                //  lblTotalRecordProcessed.
                IFeature selectedFeature = null;
                ISet relatedObjects = null;
                int indexSubtype = -1, indexGlobalId = -1, count = 1;
                bool indexAdded = false;
                while ((selectedFeature = fcursor.NextFeature()) != null)
                {
                    relatedObjects = relationshipClass.GetObjectsRelatedToObject(selectedFeature as IObject);
                    relatedObjects.Reset();
                    //IPointCollection ductBankVertices = selectedFeature.Shape as IPointCollection;
                    IPoint currPoint = selectedFeature.Shape as IPoint;
                    IPolyline currPoint2 = selectedFeature.Shape as IPolyline;
                    if (relatedObjects.Count > 0)
                    {
                        IFeature featureObject = null;
                        while ((featureObject = relatedObjects.Next() as IFeature) != null)
                        {
                            count++;
                            if (featureObject.Class.AliasName == relatedTable.AliasName)
                            {
                                IGeometry geom = featureObject.Shape as IGeometry;
                                IProximityOperator op = null;
                                if (currPoint != null)
                                {
                                     op = currPoint as IProximityOperator;
                                }
                                else
                                {
                                    op = currPoint2 as IProximityOperator;
                                }
                                double currDistance = op.ReturnDistance(geom);                               
                                string objectID = selectedFeature.get_Value(0).ToString();
                                string objectID2 = featureObject.get_Value(0).ToString();
                                if (!indexAdded)
                                {
                                    indexSubtype = baseFeatureClass.FindField("Subtypecd");
                                    indexGlobalId = baseFeatureClass.FindField("GLOBALID");
                                    indexAdded = true;
                                }
                                DataRow dataRow = _dataTable.NewRow();
                                dataRow[0] = baseFeatureClass.AliasName;
                                dataRow[1] = Convert.ToString(selectedFeature.get_Value(indexSubtype));
                                dataRow[2] = Convert.ToString(selectedFeature.get_Value(indexGlobalId));
                                dataRow[3] = relatedTable.AliasName;
                                dataRow[4] = featureObject.get_Value(relatedTable.FindField("GLOBALID"));
                                dataRow[5] = currDistance;
                                _dataTable.Rows.Add(dataRow);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logs.writeLine("Error while generating the report " + ex.Message);
            }
        }

        private void backgroundWorkerGenerateReport_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                if (e.ProgressPercentage < 101)
                {
                    progressBar.Value = e.ProgressPercentage;
                }
            }
            catch (Exception ex)
            {
                Logs.writeLine("Error while loading the feature classes: " + ex.Message);
            }
        }

        private void backgroundWorkerGenerateReport_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (reportSuccess)
                {
                    MessageBox.Show("Report Generated Successfully.");
                }
            }
            catch (Exception ex)
            {
                Logs.writeLine("Error : " + ex.Message);
            }
            finally
            {
                progressBar.Value = 0; 
            }
        }
    }
}
