using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using PGE.Desktop.EDER.AutoUpdaters.Special;
using Miner.Interop;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using PGE.Common.Delivery.Diagnostics;
using System.Reflection;
using PGE.Common.Delivery.Systems.Configuration;

namespace PGE.Desktop.EDER.ArcMapCommands
{
    public partial class PGE_SelectRecordForSAPCommandForm : Form
    {
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        public PGE_SelectRecordForSAPCommandForm()
        {
            InitializeComponent();

            lblProgress.Text = "";
        }

        public IMap map { get; set; }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            ExecuteUpdates();
        }

        private void ExecuteUpdates()
        {
            int featuresToProcess = 0;
            int GlobalIDIndex = -1;
            String AssetID = string.Empty;
            string FeatureClassName = string.Empty;
            try
            {
                // TODO: Add PGE_SelectRecordForSAPCommand.OnClick implementation

                if (map.SelectionCount != 0)
                {
                    IEnumFeature pEnumFeat = map.FeatureSelection as IEnumFeature;
                    pEnumFeat.Reset();
                    IFeature pfeat = pEnumFeat.Next();
                    while (pfeat != null)
                    {
                        IFeatureClass originClass = pfeat.Class as IFeatureClass;
                        //Check feature class is not Annotation feature Class
                        if (originClass.FeatureType != esriFeatureType.esriFTAnnotation)
                        {
                            featuresToProcess++;
                        }
                        pfeat = pEnumFeat.Next();
                    }

                    DialogResult result = DialogResult.Yes;
                    int warningThreshold = WarningThreshold();
                    if (featuresToProcess > warningThreshold)
                    {
                        //For now, this warning threshold will actually restrict a user from adding this many features
                        result = MessageBox.Show(string.Format("{0} records were selected. Current configuration restricts sending more than {1} records at one time.", featuresToProcess, warningThreshold), "Max threshold exceeded", MessageBoxButtons.OK);
                    }

                    if (result == DialogResult.Yes)
                    {
                        int featuresProcessed = 0;
                        UpdateProgress(featuresToProcess, featuresProcessed);
                        Application.DoEvents();
                        pEnumFeat.Reset();
                        pfeat = pEnumFeat.Next();
                        while (pfeat != null)
                        {
                            IFeatureClass originClass = pfeat.Class as IFeatureClass;
                            //Check feature class is not Annotation feature Class
                            if (originClass.FeatureType != esriFeatureType.esriFTAnnotation)
                            {
                                GlobalIDIndex = pfeat.Fields.FindField("GLOBALID");
                                AssetID = pfeat.get_Value(GlobalIDIndex).ToString();
                                FeatureClassName = ((IDataset)pfeat.Class).Name.ToString();
                                UpdateGISSAPReprocessASSetSynch(((IDataset)originClass).Workspace, AssetID, FeatureClassName, map);
                                featuresProcessed++;
                            }
                            pfeat = pEnumFeat.Next();

                            if ((featuresProcessed % 10) == 0)
                            {
                                UpdateProgress(featuresToProcess, featuresProcessed);
                                Application.DoEvents();
                            }
                        }

                        UpdateProgress(featuresToProcess, featuresProcessed);
                        Application.DoEvents();
                        MessageBox.Show(string.Format("{0} records were inserted for SAP processing:", featuresProcessed));
                    }
                }
                else
                {
                    _logger.Debug("PGE_SelectRecordForSAPCommand:" + " There is no recored seleted");
                    MessageBox.Show("No record is selected.Please select the records for SAP processing:");
                }
            }

            catch (Exception ex)
            {
                _logger.Error("PGE_SelectRecordForSAPCommand", ex);
                MessageBox.Show("Failed to process records due to unexpected error: " + ex.Message);
            }
            finally
            {
                this.Close();
            }
        }

        private int WarningThreshold()
        {
            //Read the config location from the Registry Entry
            int returnValue = 1000;
            int _registryDefaultVlue = 1000;
            SystemRegistry sysRegistry = new SystemRegistry("PGE");

            try
            {
                //Set the default value
                if (!sysRegistry.Contains("SendToSAPWarningThreshold"))
                {
                    sysRegistry.SetSetting<int>("SendToSAPWarningThreshold", _registryDefaultVlue);
                }
            }
            catch { }

            returnValue = sysRegistry.GetSetting<int>("SendToSAPWarningThreshold", _registryDefaultVlue);
            return returnValue;
        }

        private void UpdateGISSAPReprocessASSetSynch(IWorkspace pWorkspace, string AssetID, string FeatureClassName, IMap pMap)
        {
            ITable pTable = null;
            int ASSETIDindex = -1;
            int FeatureClassNameIndex = -1;
            int DateCreatedIndex = -1;
            int CreatedUserIndex = -1;
            string Username = "";
            try
            {
                //Get the WorkSpace
                if (pWorkspace != null)
                {
                    Username = GetUserName(pWorkspace);
                    pTable = GetResynchTable(pWorkspace);

                    if (pTable != null)
                    {
                        ASSETIDindex = pTable.FindField("ASSETID");
                        FeatureClassNameIndex = pTable.FindField("FEATURECLASSNAME");
                        DateCreatedIndex = pTable.FindField("DATECREATED");
                        CreatedUserIndex = pTable.FindField("CREATEDUSER");
                        IRow pRow = pTable.CreateRow();
                        pRow.set_Value(ASSETIDindex, AssetID);
                        pRow.set_Value(FeatureClassNameIndex, FeatureClassName);
                        pRow.set_Value(DateCreatedIndex, System.DateTime.Now);
                        pRow.set_Value(CreatedUserIndex, Username);
                        pRow.Store();
                    }
                    else
                    {
                        MessageBox.Show("Unable to find the 'EDGIS.PGE_GISSAP_REPROCESSASSETSYNC' table");
                    }
                }

                else
                {
                    _logger.Debug("PGE_SelectRecordForSAPCommand:UpdateGISSAPReprocessASSetSynch" + " WorkSpace is null");
                    MessageBox.Show("WorkSpace is null:");
                }

            }
            catch (Exception ex)
            {
                _logger.Error("PGE_SelectRecordForSAPCommand:UpdateGISSAPReprocessASSetSynch", ex);

            }

        }

        private ITable ResynchTable = null;
        private ITable GetResynchTable(IWorkspace pWorkspace)
        {
            if (ResynchTable == null)
            {
                IFeatureWorkspace pFeatureWorkspace = pWorkspace as IFeatureWorkspace;

                if (pFeatureWorkspace != null)
                {
                    //Open the PGE_GISSAP_REPROCESSASSETSYNC table to insert the selected record
                    ResynchTable = pFeatureWorkspace.OpenTable("EDGIS.PGE_GISSAP_REPROCESSASSETSYNC");
                }
            }
            return ResynchTable;
        }

        private string UserName = "";
        private string GetUserName(IWorkspace pWorkspace)
        {
            if (string.IsNullOrEmpty(UserName))
            {
                IPropertySet propertySet = pWorkspace.ConnectionProperties;
                if (propertySet != null)
                {
                    UserName = propertySet.GetProperty("USER").ToString();
                }
            }
            return UserName;
        }

        private void UpdateProgress(int totalToProcess, int totalProcessed)
        {
            try
            {
                double progress = Math.Round((((double)totalProcessed) / ((double)totalToProcess)) * 100.0, 2);
                int currentProgress = Int32.Parse(Math.Floor(progress).ToString());
                lblProgress.Text = string.Format("{2}% complete: {0} out of {1} processed", totalProcessed, totalToProcess, currentProgress);
                prgProgressBar.Value = currentProgress;
            }
            catch (Exception ex)
            {

            }
        }

        private bool CancelUpdates = false;
        private void btnCancelUpdates_Click(object sender, EventArgs e)
        {
            CancelUpdates = true;
        }
    }
}
