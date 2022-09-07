using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Catalog;
using ESRI.ArcGIS.CatalogUI;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;
using System.Reflection;
using PGE.Common.Delivery.Diagnostics;

namespace PGE.Desktop.EDER
{
    /// <summary>
    /// Summary description for PGE_SessionConfig.
    /// </summary>
    [Guid("e2a1b83c-a0d9-4e3c-8fd8-9cdf9a98a22f")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("PGE.Desktop.EDER.UploadSessionConfig")]
    public sealed class UploadSessionConfig : BaseCommand
    {
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");     
        #region COM Registration Function(s)
        [ComRegisterFunction()]
        [ComVisible(false)]
        static void RegisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryRegistration(registerType);

            //
            // TODO: Add any COM registration code here
            //
        }

        [ComUnregisterFunction()]
        [ComVisible(false)]
        static void UnregisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryUnregistration(registerType);

            //
            // TODO: Add any COM unregistration code here
            //
        }

        #region ArcGIS Component Category Registrar generated code
        /// <summary>
        /// Required method for ArcGIS Component Category registration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryRegistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            GxCommands.Register(regKey);

        }
        /// <summary>
        /// Required method for ArcGIS Component Category unregistration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryUnregistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            GxCommands.Unregister(regKey);

        }

        #endregion
        #endregion

        public override bool Enabled
        {
            get
            {

                if (m_application != null)
                {
                    try
                    {
                        IGxApplication pGxApp = m_application as IGxApplication;
                        IGxObject pGxObject = pGxApp.SelectedObject;

                        if (pGxObject != null)
                        {
                            IGxDatabase database = pGxObject as IGxDatabase;
                            if (database != null)
                            {
                                if (database.IsConnected)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        _logger.Debug("Message From SessionChecker -" + ex.Message + " StackTrace:" + ex.StackTrace);
                        return false;
                    }

                }

                return false;
            }
        }


        private IApplication m_application;
        public UploadSessionConfig()
        {
            //
            // TODO: Define values for the public properties
            //
            base.m_category = "PGE Tools"; //localizable text
            base.m_caption = "PGE Upload Session Config";  //localizable text 
            base.m_message = "Upload Session Checker Config";  //localizable text
            base.m_toolTip = "Upload Session Checker Config";  //localizable text
            base.m_name = "UploadSessionConfig";   //unique id, non-localizable (e.g. "MyCategory_MyCommand")

            try
            {
                //
                // TODO: change bitmap name if necessary
                //
               // string bitmapResourceName = GetType().Name + ".bmp";
               // base.m_bitmap = new Bitmap(GetType(), bitmapResourceName);
                base.m_bitmap = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("PGE.Desktop.EDER.Bitmaps.UploadSessionConfig.bmp"));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message, "Invalid Bitmap");
            }
        }

        #region Overridden Class Methods

        /// <summary>
        /// Occurs when this command is created
        /// </summary>
        /// <param name="hook">Instance of the application</param>
        public override void OnCreate(object hook)
        {
            if (hook == null)
                return;

            m_application = hook as IApplication;


            //Disable if it is not ArcCatalog
            if (hook is IGxApplication)
                base.m_enabled = true;
            else
                base.m_enabled = false;

            // TODO:  Add other initialization code
        }

        /// <summary>
        /// Occurs when this command is clicked
        /// </summary>
        /// 
        string tablename = "EDGIS.sessionconfig";
        IWorkspace pWorkspace;
        ITable mtable;
        List<Dictionary<string, object>> insertItemList = new List<Dictionary<string, object>>();
        //  pGxObject = m_application.SelectedObject;
        public override void OnClick()
        {

            IGxApplication pGxApp = m_application as IGxApplication;
            IGxObject pGxObject = pGxApp.SelectedObject;

            if (pGxObject != null)
            {
                IGxDatabase database = pGxObject as IGxDatabase;
                if (database != null)
                {
                    if (database.IsConnected)
                    {
                        pWorkspace = database.Workspace;
                        if (pWorkspace != null)
                        {

                            IFeatureWorkspace fws = pWorkspace as IFeatureWorkspace;
                            if (fws != null)
                            {
                                try
                                {
                                    mtable = fws.OpenTable(tablename);
                                    if (mtable != null)
                                    {
                                        getExcelRecords();
                                    }
                                    else
                                    {
                                        MessageBox.Show("SessionConfig Table is not available");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("SessionConfig Table is not available");
                                   // MessageBox.Show(ex.Message);
                                    _logger.Debug("Message From SessionChecker -" + ex.Message + " StackTrace:" + ex.StackTrace);
                                    return;
                                }

                            }
                        }
                    }
                    else
                        MessageBox.Show("Please connect to workspace");
                }
                else
                    MessageBox.Show("Please select workspace");

            }
            else
                MessageBox.Show("Please select workspace");
        }
        public void getExcelRecords()
        {

            // declaration of required varibles 
            string ObjectClassRo, SubtypeRo, FieldRo, DatasetRo, SubtypeCodeRo, FieldAliasRo, FIndex, mmFSVisible;
            string PreMapMode, CmcsMode, AsBuiltMode;
            int PreMapModeColIndex, CmcsModeColIndex, AsBuiltModeColIndex;
            OpenFileDialog op1 = new OpenFileDialog();
            op1.Multiselect = false;
            op1.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm;*.csv";
            if (op1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    PreMapModeColIndex = 12;
                    CmcsModeColIndex = 13;
                    AsBuiltModeColIndex = 14;
                    insertItemList = new List<Dictionary<string, object>>();
                    string ConnString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + op1.FileName + ";Extended Properties=Excel 8.0;Persist Security Info=False";
                    DataTable Data = new DataTable();
                    using (OleDbConnection conn = new OleDbConnection(ConnString))
                    {
                        conn.Open();
                        DataTable dbSchema = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                        if (dbSchema == null || dbSchema.Rows.Count < 1)
                        {
                            throw new Exception("Error: Could not determine the name of the first worksheet.");
                        }
                        string firstSheetName = dbSchema.Rows[0]["TABLE_NAME"].ToString();
                        OleDbCommand cmd = new OleDbCommand(String.Format("SELECT * FROM [{0}]", firstSheetName), conn);
                        OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);
                        adapter.Fill(Data);
                        OleDbDataReader o_dr = cmd.ExecuteReader();
                        while (o_dr.Read())
                        {

                            ObjectClassRo = o_dr[0].ToString().Replace('"', ' ').Trim();
                            SubtypeRo = o_dr[1].ToString().Replace('"', ' ').Trim();
                            FieldRo = o_dr[2].ToString().Replace('"', ' ').Trim();
                            DatasetRo = o_dr[3].ToString().Replace('"', ' ').Trim();
                            SubtypeCodeRo = o_dr[4].ToString().Replace('"', ' ').Trim();
                            FieldAliasRo = o_dr[5].ToString().Replace('"', ' ').Trim();
                            FIndex = o_dr[6].ToString().Replace('"', ' ').Trim();
                            mmFSVisible = o_dr[7].ToString().Replace('"', ' ').Trim();
                            PreMapMode = o_dr[PreMapModeColIndex].ToString().Replace('"', ' ').Trim();
                            CmcsMode = o_dr[CmcsModeColIndex].ToString().Replace('"', ' ').Trim();
                            AsBuiltMode = o_dr[AsBuiltModeColIndex].ToString().Replace('"', ' ').Trim();
                            //ReviewMode = o_dr[8].ToString().Replace('"', ' ').Trim();

                            Dictionary<string, object> insertItemIntoJQPreMap = new Dictionary<string, object>();
                            Dictionary<string, object> insertItemIntoJQCmcs = new Dictionary<string, object>();
                            Dictionary<string, object> insertItemIntoJQAsBuilt = new Dictionary<string, object>();
                            if (ObjectClassRo != "" && mmFSVisible != "" && PreMapMode != "")
                            {
                                insertItemIntoJQPreMap.Add("OBJECTCLASSRO", ObjectClassRo);
                                insertItemIntoJQPreMap.Add("SUBTYPERO", SubtypeRo);
                                insertItemIntoJQPreMap.Add("FIELDRO", FieldRo);
                                insertItemIntoJQPreMap.Add("DATASETRO", DatasetRo);
                                insertItemIntoJQPreMap.Add("SUBTYPECODERO", SubtypeCodeRo);
                                insertItemIntoJQPreMap.Add("FIELDALIASRO", FieldAliasRo);
                                insertItemIntoJQPreMap.Add("FINDEX", FIndex);
                                insertItemIntoJQPreMap.Add("MMFSVISIBLE", "TRUE");
                                insertItemIntoJQPreMap.Add("RMODE", "Pre-Map");
                                insertItemList.Add(insertItemIntoJQPreMap);
                            }

                            if (ObjectClassRo != "" && mmFSVisible != "" && CmcsMode != "")
                            {
                                insertItemIntoJQCmcs.Add("OBJECTCLASSRO", ObjectClassRo);
                                insertItemIntoJQCmcs.Add("SUBTYPERO", SubtypeRo);
                                insertItemIntoJQCmcs.Add("FIELDRO", FieldRo);
                                insertItemIntoJQCmcs.Add("DATASETRO", DatasetRo);
                                insertItemIntoJQCmcs.Add("SUBTYPECODERO", SubtypeCodeRo);
                                insertItemIntoJQCmcs.Add("FIELDALIASRO", FieldAliasRo);
                                insertItemIntoJQCmcs.Add("FINDEX", FIndex);
                                insertItemIntoJQCmcs.Add("MMFSVISIBLE", "TRUE");
                                insertItemIntoJQCmcs.Add("RMODE", "CMCS");
                                insertItemList.Add(insertItemIntoJQCmcs);
                            }

                            if (ObjectClassRo != "" && mmFSVisible != "" && AsBuiltMode != "")
                            {
                                insertItemIntoJQAsBuilt.Add("OBJECTCLASSRO", ObjectClassRo);
                                insertItemIntoJQAsBuilt.Add("SUBTYPERO", SubtypeRo);
                                insertItemIntoJQAsBuilt.Add("FIELDRO", FieldRo);
                                insertItemIntoJQAsBuilt.Add("DATASETRO", DatasetRo);
                                insertItemIntoJQAsBuilt.Add("SUBTYPECODERO", SubtypeCodeRo);
                                insertItemIntoJQAsBuilt.Add("FIELDALIASRO", FieldAliasRo);
                                insertItemIntoJQAsBuilt.Add("FINDEX", FIndex);
                                insertItemIntoJQAsBuilt.Add("MMFSVISIBLE", "TRUE");
                                insertItemIntoJQAsBuilt.Add("RMODE", "As-Built");
                                insertItemList.Add(insertItemIntoJQAsBuilt);
                            }
                        }
                        conn.Close();
                        if (insertItemList.Count > 0)
                        {
                            // Delete existing records
                            QueryFilter queryFilter = new QueryFilter();
                            IQueryFilter qFilter = new QueryFilterClass();
                            mtable.DeleteSearchedRows(qFilter);
                            // CALL FOR NEW DATA INSERTION
                            InsertIntoTable(insertItemList, mtable);
                        }
                        
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    _logger.Debug("Message From SessionChecker -" + ex.Message + " StackTrace:" + ex.StackTrace);

                }
            }
            else
            {
                //  MessageBox.Show("Please Select Excel File");
            }
        }

        public void InsertIntoTable(List<Dictionary<string, object>> insertItemList, ITable mtable)
        {
            ICursor insertCur = null;
            try
            {
                IRowBuffer rowBuffer = mtable.CreateRowBuffer();
                insertCur = mtable.Insert(true);
                foreach (var insertItem in insertItemList)
                {
                    foreach (string key in insertItem.Keys)
                    {
                        rowBuffer.set_Value(mtable.FindField(key), insertItem[key]);
                    }

                    insertCur.InsertRow(rowBuffer);

                }
                insertCur.Flush();
                MessageBox.Show("Data Inserted to SessionConfig");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                _logger.Debug("Message From SessionChecker -" + ex.Message + " StackTrace:" + ex.StackTrace);
            }
            finally
            {
                //release COM object        
                ReleaseCOMObject(insertCur);
            }
        }
        private void ReleaseCOMObject(object obj)
        {
            try
            {
                if (obj != null)
                {
                    int refsLeft = 0;
                    do
                    {
                        refsLeft = Marshal.ReleaseComObject(obj);
                    }
                    while (refsLeft > 0);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
                _logger.Debug("Message From SessionChecker -" + ex.Message + " StackTrace:" + ex.StackTrace);
            }
        }

        #endregion
    }
}
