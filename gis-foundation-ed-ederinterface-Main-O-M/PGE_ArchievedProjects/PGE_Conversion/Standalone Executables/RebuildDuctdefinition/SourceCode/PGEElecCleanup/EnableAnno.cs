using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.OracleClient;
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
    public partial class EnableAnno : Form
    {
        private IMMAutoUpdater autoupdater = null;
        private DataTable objReportTable = new DataTable();
        clsGlobalFunctions _clsGlobalFunctions = new clsGlobalFunctions();
        public EnableAnno()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                _clsGlobalFunctions.Common_initSummaryTable("CheckAnno", "CheckAnno");
                ArrayList objarraylist = null;
                stbInfo.Text = "Reseting auto creation of annotations...";
                Logs.writeLine("Reseting auto creation of annotations... ");
                #region "Start Auto Creation of Annotations in Destination database"
                IEnumDataset DestinationEnumdataset = clsTestWorkSpace.Workspace.get_Datasets(esriDatasetType.esriDTAny);
                DestinationEnumdataset.Reset();
                IDataset Destinationdataset = DestinationEnumdataset.Next();               

                while (Destinationdataset != null)
                {
                    Application.DoEvents();
                    AutoAnotationcreate(Destinationdataset, true, ref objarraylist);
                    Destinationdataset = DestinationEnumdataset.Next();
                }
                #endregion    
                stbInfo.Text = "Process completed see the log file.";
                Logs.writeLine("Successfully Completed");
            }
            catch (Exception ex)
            {          
                Logs.writeLine("Error on process start button " + ex.Message);
                stbInfo.Text = "Error occurred, Please see the log file.";
            }            
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
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

        private void btnStopAnnos_Click(object sender, EventArgs e)
        {
            _clsGlobalFunctions.Common_initSummaryTable("CheckAnno", "CheckAnno");
            stbInfo.Text = "Stopping Auto Creation of Annotations in Destination database ";
            Logs.writeLine("Stopping Auto Creation of Annotations in Destination database " );
            #region "Stop Auto Creation of Annotations in Destination database"
            IEnumDataset DestinationEnumdataset = clsTestWorkSpace.Workspace.get_Datasets(esriDatasetType.esriDTAny);
            //Loop through all the Destination datasets to append the data 
            IDataset Destinationdataset = DestinationEnumdataset.Next();
            ArrayList objarraylist = null;
            while (Destinationdataset != null)
            {
                Application.DoEvents();
                AutoAnotationcreate(Destinationdataset, false, ref objarraylist);
                Destinationdataset = DestinationEnumdataset.Next();
            }
            #endregion
            stbInfo.Text = "Successfully Completed ";
            Logs.writeLine("Successfully Completed");
        }
    }
}
