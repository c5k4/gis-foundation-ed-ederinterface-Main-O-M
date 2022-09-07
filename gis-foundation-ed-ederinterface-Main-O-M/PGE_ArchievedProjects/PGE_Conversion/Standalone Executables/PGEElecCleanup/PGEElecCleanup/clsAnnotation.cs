using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.DataSourcesGDB;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Carto;
using Miner.Interop;
using System.Collections;

namespace PGEElecCleanup
{
    class clsAnnotation : clsGlobalFunctions
    {
        DataTable dtSummaryList = null;
        int intTotalObjects = 0;
        int intTotalUpdated = 0;
        int intTotalError = 0;
        DataTable dtModifiedAnnoList = null;
        public StatusBar mStatusBar = null;
        private IMMAutoUpdater autoupdater = null;

        public bool startProcess(List<string> LstFCnames)
        {
            try
            {
                //initialize the summary table
                initSummaryTable();

                updStatus = "Disabling autoupdaters...";
                #region "Disable Autoupdaters "
                mmAutoUpdaterMode objautoupdateroldmode = DisableAutoupdaters();
                #endregion

                updStatus = "Stoping auto creation of annotations...";
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

                for (int intC = 0; intC < LstFCnames.Count; intC++)
                {
                    string strFCName = LstFCnames[intC].ToString();

                    updStatus = "Processing..." + strFCName;
                    bool status = updateAnnoConstructionStatus(strFCName);
                    createReporFile(strFCName);
                    addSummaryRow(strFCName);
                }

                updStatus = "Reseting auto creation of annotations...";
                #region "Start Auto Creation of Annotations in Destination database"

                // Sbmessage.Panels["stsBarPanelToolStatus"].Text = "Start Auto Creation of Annotations in Destination database......";
                DestinationEnumdataset.Reset();
                Destinationdataset = DestinationEnumdataset.Next();
                while (Destinationdataset != null)
                {
                    Application.DoEvents();
                    AutoAnotationcreate(Destinationdataset, true, ref objarraylist);
                    Destinationdataset = DestinationEnumdataset.Next();
                }
                #endregion

                updStatus = "Enabling autoupdaters...";
                #region start AU
                if (autoupdater != null)
                {
                    autoupdater.AutoUpdaterMode = objautoupdateroldmode;
                }
                #endregion

                createSummaryRepor();

                MessageBox.Show("Successfully Completed, See log file...", "PGE", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            catch (Exception Ex)
            {
                Logs.writeLine("Error on process " + Ex.Message);
                return false;
            }
            finally
            {
                Logs.close();
            }
        }

        public bool updateAnnoConstructionStatus(string strAnnFClassName)
        {
            IFeatureClass pAnnoFeaureclass = null;
            IFeatureClass pParentFClass = null;
            IRelationshipClass pRelClass = null;
            try
            {
                string strAnnoFCName = string.Empty;
                string strRelClassName = string.Empty;
                string strParentFCname = string.Empty;

                //initialize the varialbles
                intTotalObjects = 0;
                intTotalUpdated = 0;
                intTotalError = 0;

                strAnnoFCName = strAnnFClassName;
                Logs.writeLine("Processing featureclass..." + strAnnFClassName);

                //initialize datatable.
                initDataTableForReports();

                updStatus = "Opening featureclass...";
                //check Featureclass exist in the database
                if (isFCExist(strAnnFClassName, clsTestWorkSpace.Workspace) == false) return false;

                //open anno featureclass from database
                pAnnoFeaureclass = getFeatureclassByName(clsTestWorkSpace.Workspace, strAnnFClassName);
                if (pAnnoFeaureclass == null) return false;

                if (pAnnoFeaureclass.FeatureType != esriFeatureType.esriFTAnnotation)
                {
                    Logs.writeLine("Not a annotation featureclass : " + strAnnFClassName);
                    return false;
                }

                //get relationshipclass
                pRelClass = getRelationShipClass(strAnnFClassName, pAnnoFeaureclass);

                //if the object not found return to main 
                if (pRelClass == null) return false;
                strRelClassName = ((IDataset)pRelClass).Name;

                strParentFCname = ((IDataset)pRelClass.OriginClass).Name;

                updStatus = "start editing...";
                Logs.writeLine("  START Editing...");
                //start editing the workspace
                clsTestWorkSpace.StartEditOperation();

                Logs.writeLine("  Data Processing...");
                bool blnSuccess = processForUpdate(pAnnoFeaureclass, pRelClass, strAnnFClassName, strRelClassName, strParentFCname);

                updStatus = "  STOP Editing the database...";
                Logs.writeLine("  STOP Editing...SaveEdits : " + blnSuccess.ToString());
                //stop editing operation on success
                clsTestWorkSpace.StopEditOperation(blnSuccess);

                Logs.writeLine("  STOP Editing Completed");
                Logs.writeLine("Completed Featureclass..." + strAnnFClassName);
                return true;
            }
            catch (Exception ex)
            {
                Logs.writeLine("Error " + ex.Message);
                return false;
            }
            finally
            {
                if (pAnnoFeaureclass != null)
                {
                    Marshal.ReleaseComObject(pAnnoFeaureclass);
                    pAnnoFeaureclass = null;
                }

                if (pRelClass != null)
                {
                    Marshal.ReleaseComObject(pRelClass);
                    pRelClass = null;
                }
            }
        }

        private bool processForUpdate(IFeatureClass pAnnoFeatureclass, IRelationshipClass pRelClass, string strAnnFClassName, string strRelClassName, string strParentFCname)
        {
            IFeatureCursor pAnnoFeatureCursor = null;
            try
            {
                intTotalObjects = 0;
                intTotalUpdated = 0;
                intTotalError = 0;

                string strAnnConsoValue = string.Empty;
                string strParentConstruValue = string.Empty;
                string strParentFOID = string.Empty;

                int intProcess = 1;
                int intAnnoConsrtFiledIndex = pAnnoFeatureclass.FindField(ConstantValues.CONSTRUCTIONSTATUS);

                //check features exits for update construction status
                intTotalObjects = pAnnoFeatureclass.FeatureCount(null);
                if (intTotalObjects == 0) return true;

                pAnnoFeatureCursor = pAnnoFeatureclass.Search(null, false);
                IFeature pAnnoFeature = pAnnoFeatureCursor.NextFeature();
                IObject relObj = null;

                while (pAnnoFeature != null)
                {
                    try
                    {
                        strAnnConsoValue = "";
                        strParentConstruValue = "";
                        strParentFOID = "";

                        updStatus = "Processing for" + strAnnFClassName + "..." + intProcess + " Of " + intTotalObjects;
                        intProcess = intProcess + 1;
                        //get anno feature construction status
                        strAnnConsoValue = getStringVal(pAnnoFeature.get_Value(intAnnoConsrtFiledIndex), pAnnoFeature.OID.ToString(), ConstantValues.CONSTRUCTIONSTATUS);

                        //get parent feature
                        ISet pSet = pRelClass.GetObjectsRelatedToObject((IObject)pAnnoFeature);
                        if (pSet.Count > 0)
                        {
                            strParentConstruValue = "";
                            while ((relObj = pSet.Next() as IObject) != null)
                            {
                                strParentConstruValue = getStringVal(relObj.get_Value(relObj.Fields.FindField(ConstantValues.CONSTRUCTIONSTATUS)),relObj.OID.ToString(),ConstantValues.CONSTRUCTIONSTATUS);
                                strParentFOID = relObj.OID.ToString();
                            }

                            if (strParentConstruValue.Trim().ToUpper() != strAnnConsoValue.Trim().ToUpper())
                            {
                                if (isValidContructionStatusValue(strParentConstruValue))
                                {
                                    //update constructionstatus for ann feature
                                    pAnnoFeature.set_Value(intAnnoConsrtFiledIndex, strParentConstruValue);
                                    pAnnoFeature.Store();

                                    DataRow objReoprtRow = dtModifiedAnnoList.NewRow();
                                    objReoprtRow["AnnoFC_Name"] = strAnnFClassName;
                                    objReoprtRow["ParentFC_Name"] = strParentFCname;
                                    objReoprtRow["AnnFeature_OID-ParentFeature_OID"] = pAnnoFeature.OID.ToString() + "/" + strParentFOID;
                                    objReoprtRow["AnnConstStatus_FeatConstStatus"] = strAnnConsoValue + "/" + strParentConstruValue;
                                    objReoprtRow["Updated_Val"] = strParentConstruValue;
                                    dtModifiedAnnoList.Rows.Add(objReoprtRow);
                                    intTotalUpdated = intTotalUpdated + 1;
                                }
                                else
                                {
                                    DataRow objReoprtRow = dtModifiedAnnoList.NewRow();
                                    objReoprtRow["AnnoFC_Name"] = strAnnFClassName;
                                    objReoprtRow["ParentFC_Name"] = strParentFCname;
                                    objReoprtRow["AnnFeature_OID-ParentFeature_OID"] = pAnnoFeature.OID.ToString() + "/" + strParentFOID;
                                    objReoprtRow["AnnConstStatus_FeatConstStatus"] = strAnnConsoValue + "/" + strParentConstruValue;
                                    objReoprtRow["Updated_Val"] = "Parent Feature has no Construction status or Invalid value";
                                    dtModifiedAnnoList.Rows.Add(objReoprtRow);
                                    intTotalError = intTotalError + 1;
                                }
                            }
                            /////////////Task#50 : 281010 : update the prilim anno constructionstatus to italic

                            if (strParentConstruValue.Trim().ToUpper() == "P")
                            {
                                string strLblTxt = pAnnoFeature.get_Value(pAnnoFeature.Fields.FindField("TEXTSTRING")).ToString();

                                if (strLblTxt.Contains("<ITA>") == false)
                                {
                                    strLblTxt = "<ITA>" + strLblTxt + "</ITA>";
                                    pAnnoFeature.set_Value(pAnnoFeature.Fields.FindField("TEXTSTRING"), strLblTxt);
                                    pAnnoFeature.Store();
                                }
                            }
                        }
                        else
                        {
                            DataRow objReoprtRow = dtModifiedAnnoList.NewRow();
                            objReoprtRow["AnnoFC_Name"] = strAnnFClassName;
                            objReoprtRow["ParentFC_Name"] = strParentFCname;
                            objReoprtRow["AnnFeature_OID-ParentFeature_OID"] = pAnnoFeature.OID.ToString();
                            objReoprtRow["AnnConstStatus_FeatConstStatus"] = "";
                            objReoprtRow["Updated_Val"] = "Parent Feature not found";
                            dtModifiedAnnoList.Rows.Add(objReoprtRow);
                            intTotalError = intTotalError + 1;
                        }
                    }
                    catch (Exception Ex)
                    {
                        Logs.writeLine("Unable To update for feature OID : " + pAnnoFeature.OID.ToString() + "<Error : " + Ex.Message + ">");

                        DataRow objReoprtRow = dtModifiedAnnoList.NewRow();
                        objReoprtRow["AnnoFC_Name"] = strAnnFClassName;
                        objReoprtRow["ParentFC_Name"] = strParentFCname;
                        objReoprtRow["AnnFeature_OID-ParentFeature_OID"] = pAnnoFeature.OID.ToString();
                        objReoprtRow["AnnConstStatus_FeatConstStatus"] = "";
                        objReoprtRow["Updated_Val"] = "ERROR : " + Ex.Message;
                        dtModifiedAnnoList.Rows.Add(objReoprtRow);
                        intTotalError = intTotalError + 1;
                    }

                    pAnnoFeature = pAnnoFeatureCursor.NextFeature();
                }

                return true;
            }
            catch (Exception Ex)
            {
                Logs.writeLine("Error on updateConstructionStatus : " + strAnnFClassName);
                return false;
            }
            finally
            {
                if (pAnnoFeatureCursor != null)
                {
                    Marshal.ReleaseComObject(pAnnoFeatureCursor);
                    pAnnoFeatureCursor = null;
                }
            }
        }
        private bool isValidContructionStatusValue(string strParentConstruValue)
        {
            if (strParentConstruValue.Length == 0) return false;
            if (strParentConstruValue.ToUpper() == "A" || strParentConstruValue.ToUpper() == "P" || strParentConstruValue.ToUpper() == "R")
            {
                return true;
            }
            return false;
        }
        private string getStringVal(object p, string strFC_OID, string strField)
        {
            try
            {
                if (p == DBNull.Value || p == null)
                {
                    return string.Empty;
                }
                return p.ToString().Trim();
            }
            catch (Exception Ex)
            {
                Logs.writeLine("Invalid values found Feature objectid-FieldName : " + strFC_OID + "-" + strField);
                return "";
            }
        }

        private IRelationshipClass getRelationShipClass(string strAnnFClassName, IFeatureClass pAnnoFeaureclass)
        {
            try
            {
                IObjectClass pObjClass = (IObjectClass)pAnnoFeaureclass;

                IEnumRelationshipClass pEnumRelClass = pObjClass.get_RelationshipClasses(esriRelRole.esriRelRoleDestination);
                pEnumRelClass.Reset();

                IRelationshipClass pRelClass = pEnumRelClass.Next();
                while (pRelClass != null)
                {
                    return pRelClass;
                }

                return null;
            }
            catch (Exception Ex)
            {
                Logs.writeLine("Relationshipclass not found for " + strAnnFClassName + "\nError " + Ex.Message);
                return null;
            }            
        }

        private void initDataTableForReports()
        {
            try
            {
                //initialize the datatable
                if (dtModifiedAnnoList == null)
                {
                    dtModifiedAnnoList = new DataTable();
                    dtModifiedAnnoList.Columns.Add("AnnoFC_Name");
                    dtModifiedAnnoList.Columns.Add("ParentFC_Name");
                    dtModifiedAnnoList.Columns.Add("AnnFeature_OID-ParentFeature_OID");
                    dtModifiedAnnoList.Columns.Add("AnnConstStatus_FeatConstStatus");
                    dtModifiedAnnoList.Columns.Add("Updated_Val");
                }
                else
                {
                    dtModifiedAnnoList.Clear();
                }
            }
            catch (Exception Ex)
            {
                Logs.writeLine("Erroe on initDataTableForReports method : " + Ex.Message);
            }
        }

        private void initSummaryTable()
        {
            try
            {
                //initialize the datatable
                //Update project details in log file.
                Logs.createLogfile("updateAnnotationConstStatus.log");
                Logs.writeLine("Project Name :" + "PGE ELECTRIC");
                Logs.writeLine("Application Name :" + "Update constructionstatus for annotation");
                Logs.writeLine("Start Date and Time  :" + System.DateTime.Now);
                Logs.writeLine("******************************************************************************");
                Logs.writeLine("Database details");
                Logs.writeLine("******************************************************************************");

                //report database details
                Logs.writeLine("Databse : " + clsTestWorkSpace.strDATABASE + " INSTANCE :" + clsTestWorkSpace.strINSTANCE);
                Logs.writeLine("USER : " + clsTestWorkSpace.strUSER + " PASSWORD :" + clsTestWorkSpace.strPASSWORD);
                Logs.writeLine("VERSION :" + clsTestWorkSpace.strVERSION);
                Logs.writeLine("******************************************************************************");

                //initialize the datatable
                dtSummaryList = new DataTable();
                dtSummaryList.Columns.Add("FC_Name");
                dtSummaryList.Columns.Add("Total_Annotations");
                dtSummaryList.Columns.Add("Updated_Annotations");
                dtSummaryList.Columns.Add("ERROR");

                //Update project details in datatable.
                addRow("Project Name :" + "PGE ELECTRIC");
                addRow("Application Name :" + "Update constructionstatus for annotation");
                addRow("Start Date and Time  :" + System.DateTime.Now);
                addRow("******************************************************************************");
                addRow("Database details");
                addRow("******************************************************************************");

                //report database details
                addRow("Databse : " + clsTestWorkSpace.strDATABASE + " INSTANCE :" + clsTestWorkSpace.strINSTANCE);
                addRow("USER : " + clsTestWorkSpace.strUSER + " PASSWORD :" + clsTestWorkSpace.strPASSWORD);
                addRow("VERSION :" + clsTestWorkSpace.strVERSION);
                addRow("******************************************************************************");
                addRow("");

                DataRow pDataRow = dtSummaryList.NewRow();
                pDataRow["FC_Name"] = "FC_Name";
                pDataRow["Total_Annotations"] = "Total_Annotations";
                pDataRow["Updated_Annotations"] = "Updated_Annotations";
                pDataRow["ERROR"] = "ERROR";
                dtSummaryList.Rows.Add(pDataRow);
            }
            catch (Exception Ex)
            {
                Logs.writeLine("Erroe on initialize the summary table method : " + Ex.Message);
            }
        }

        private string updStatus
        {
            set
            {
                mStatusBar.Text = value;
                Application.DoEvents();
            }
        }

        private void addRow_annFc(string strInfo)
        {
            DataRow pDataRow = dtModifiedAnnoList.NewRow();
            pDataRow["AnnoFC_Name"] = strInfo;
            dtModifiedAnnoList.Rows.Add(pDataRow);
        }

        private void addRow(string strInfo)
        {
            DataRow pDataRow = dtSummaryList.NewRow();
            pDataRow["FC_Name"] = strInfo;
            dtSummaryList.Rows.Add(pDataRow);
        }

        private void addSummaryRow(string strFCName)
        {
            try
            {
                DataRow pDataRow = dtSummaryList.NewRow();
                pDataRow["FC_Name"] = strFCName;
                pDataRow["Total_Annotations"] = intTotalObjects;
                pDataRow["Updated_Annotations"] = intTotalUpdated;
                pDataRow["ERROR"] = intTotalError;
                dtSummaryList.Rows.Add(pDataRow);
            }
            catch (Exception Ex)
            {
                Logs.writeLine(Ex.Message);
            }
        }

        private void createReporFile(string strFCName)
        {
            try
            {
                if (intTotalObjects == 0) return;
                addRow_annFc("");
                addRow_annFc("Total_Annotations :" + intTotalObjects);
                addRow_annFc("Updated_Annotations :" + intTotalUpdated);
                addRow_annFc("ERROR :" + intTotalError);

                string strFilePath = System.Configuration.ConfigurationManager.AppSettings["LogFilePath"].ToString();
                string strTime = DateTime.Today.Day + "-" + DateTime.Now.Hour + "-" + DateTime.Now.Minute + "-" + DateTime.Now.Second;
                string strFile = strFilePath + strTime + "_" + strFCName + "_ConsStaReport.csv";
                DataTable2CSV(dtModifiedAnnoList, strFile,true);
            }
            catch (Exception Ex)
            {
                Logs.writeLine(Ex.Message);
            }
        }

        private void createSummaryRepor()
        {
            try
            {
                string strFilePath = System.Configuration.ConfigurationManager.AppSettings["LogFilePath"].ToString();
                string strFile = strFilePath + DateTime.Now.ToLongDateString() + "SummaryReport_ConsStaReport.csv";
                DataTable2CSV(dtSummaryList, strFile,false);
            }
            catch (Exception Ex)
            {
                Logs.writeLine(Ex.Message);
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
                                    // objfeatureclass = (IFeatureClass)enudataset;
                                    objfeatureclass = getFeatureclassByName(clsTestWorkSpace.Workspace, enudataset.Name);
                                    if (objfeatureclass.FeatureType == esriFeatureType.esriFTAnnotation)
                                    {
                                        objannotation = (IAnnoClassAdmin2)objfeatureclass.Extension;

                                        if (status == false)
                                        {
                                            objannotation.AutoCreate = status;
                                            objannotation.UpdateOnShapeChange = status;
                                        }
                                        else
                                        {
                                            objannotation.AutoCreate = status;
                                        }
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

    }
}
