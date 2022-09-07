using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Miner.Interop.Process;
using System.Runtime.InteropServices;
using Miner.ComCategories;
using Miner.Interop;
using PGE.Common.Delivery.Process;
using System.Windows.Forms;
using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.Geodatabase;
using PGE.Common.Delivery.Systems;
using Miner.Controls;
//using ClosedXML.Excel;
using Microsoft.VisualBasic;
//using Microsoft.Office.Core;
//using Microsoft.Office.Interop.Excel;
using System.Data.OleDb;


namespace PGE.Desktop.EDER.Filters
{
    
        [ComVisible(true)]
        [Guid("2606CB60-216F-46E3-BE33-CB952130E197")]
        [ProgId("PGE.Desktop.EDER.Filters.PGEExportSessions")]
        [ComponentCategory(ComCategory.MMFilter)]
        public class ExportSessionFilter : PGE.Common.Delivery.Process.BaseClasses.BasePxFilter
        {
            public ExportSessionFilter()
                : base("Session Report", "PGE.Desktop.EDER.Filters.PGEExportSessions", 5,
                    "PGEExportSessions", "PGE Export Session Filter", "Session Manager", "MMSessionManager")
            {
               // this.LargeImage=
            }
           
           // Need to correct the logic for execute
            public override ID8ListItem Execute()
                {
                    ID8ListItem _pList1 = default(ID8ListItem);
                    System.Data.DataTable dt = null;
                    System.Data.DataTable dtUser = null;
                    System.Data.DataTable dtStatus = null;
                    PxDb _pxDb = new PxDb(_PxApp);
                    string p_String = string.Empty;
                    string selectedUser = string.Empty;
                    string selectedStatus = string.Empty;
                    string searchText = string.Empty;

                    string userSql = "select user_name from process.mm_px_user order by USER_NAME asc";
                    dtUser = _pxDb.ExecuteQuery(userSql);
                    string statusSql = "select name from process.mm_px_state where state_id not in ('19','20','6','3','2') order by name asc";
                    dtStatus = _pxDb.ExecuteQuery(statusSql);
                
                
                    ExportSessionsForm exportForm = new ExportSessionsForm(_PxApp,dtUser,dtStatus);
                    DialogResult result = exportForm.ShowDialog();

                    if (result == DialogResult.OK)
                    {
                        p_String = exportForm.selectedRadio.ToString();
                        selectedStatus = exportForm.selectedStatus;
                        selectedUser = exportForm.selectedUser;
                        searchText = exportForm.searchText;                            

                        if (p_String.Equals("My Sessions"))
                        {
                            string sqlQuery = "select s.SESSION_ID,s.SESSION_NAME,s.CREATE_DATE,s.CREATE_USER,s.CURRENT_OWNER,s.DESCRIPTION,sta.NAME Status from process.mm_session s, process.mm_px_current_state st, process.mm_px_state sta where s.SESSION_ID= st.SOID and st.STATE_ID= sta.STATE_ID and s.current_owner='" + _PxApp.User.Name + "'";
                            dt = _pxDb.ExecuteQuery(sqlQuery);
                            if (dt.Rows.Count != 0)
                            {
                                try
                                {
                                    string sPath = GetPath("xls");
                                    SaveToExcel(dt, sPath);
                                    
                                }
                                catch (Exception ex)
                                {

                                }

                            }
                            else
                            {
                                MessageBox.Show("Nothing to export");
                            }
                        }

                        if (p_String.Equals("User/Status Sessions"))
                        {
                            selectedUser= exportForm.selectedUser;
                            selectedStatus= exportForm.selectedStatus;
                            string sqlQuery = "select s.SESSION_ID,s.SESSION_NAME,s.CREATE_DATE,s.CREATE_USER,s.CURRENT_OWNER,s.DESCRIPTION,sta.NAME Status from process.mm_session s, process.mm_px_current_state st, process.mm_px_state sta where s.SESSION_ID= st.SOID and st.STATE_ID= sta.STATE_ID and s.current_owner='"+selectedUser+"' and sta.name='"+selectedStatus+"'";
                            if (selectedStatus.Equals("ALL STATUS"))
                            {
                              sqlQuery = "select s.SESSION_ID,s.SESSION_NAME,s.CREATE_DATE,s.CREATE_USER,s.CURRENT_OWNER,s.DESCRIPTION,sta.NAME Status from process.mm_session s, process.mm_px_current_state st, process.mm_px_state sta where s.SESSION_ID= st.SOID and st.STATE_ID= sta.STATE_ID and s.current_owner='" + selectedUser + "'";
                            }
                            if (selectedUser.Equals("ALL USERS"))
                            {
                                sqlQuery = "select s.SESSION_ID,s.SESSION_NAME,s.CREATE_DATE,s.CREATE_USER,s.CURRENT_OWNER,s.DESCRIPTION,sta.NAME Status from process.mm_session s, process.mm_px_current_state st, process.mm_px_state sta where s.SESSION_ID= st.SOID and st.STATE_ID= sta.STATE_ID and sta.name='" + selectedStatus + "'";
                            }
                            if (selectedUser.Equals("ALL USERS") && selectedStatus.Equals("ALL STATUS"))
                            {
                                sqlQuery = "select s.SESSION_ID,s.SESSION_NAME,s.CREATE_DATE,s.CREATE_USER,s.CURRENT_OWNER,s.DESCRIPTION,sta.NAME Status from process.mm_session s, process.mm_px_current_state st, process.mm_px_state sta where s.SESSION_ID= st.SOID and st.STATE_ID= sta.STATE_ID";
                            }
                            dt = _pxDb.ExecuteQuery(sqlQuery);
                            if (dt.Rows.Count != 0)
                            {
                                try
                                {
                                    string sPath = GetPath("xls");
                                    SaveToExcel(dt, sPath);

                                }
                                catch (Exception ex)
                                {

                                }

                            }
                            else
                            {
                                MessageBox.Show("Nothing to export");
                            }
                        }

                        if (p_String.Equals("All Sessions"))
                        {
                            string sqlQuery = "select s.SESSION_ID,s.SESSION_NAME,s.CREATE_DATE,s.CREATE_USER,s.CURRENT_OWNER,s.DESCRIPTION,sta.NAME Status from process.mm_session s, process.mm_px_current_state st, process.mm_px_state sta where s.SESSION_ID= st.SOID and st.STATE_ID= sta.STATE_ID";
                            dt = _pxDb.ExecuteQuery(sqlQuery);
                            if (dt.Rows.Count != 0)
                            {
                                try
                                {
                                    string sPath = GetPath("xls");
                                    SaveToExcel(dt, sPath);

                                }
                                catch (Exception ex)
                                {

                                }

                            }
                            else
                            {
                                MessageBox.Show("Nothing to export");
                            }
                        }


                        if (p_String.Equals("PM Search"))
                        {
                            string value = string.Format("{0} LIKE '%{1}%'", "UPPER(s.SESSION_NAME)", searchText.ToUpper());

                            if (searchText.Equals("_"))
                            {
                                searchText = "'%\\_%' ESCAPE '\\'";
                                value = string.Format("{0} LIKE {1}", "UPPER(s.SESSION_NAME)", searchText.ToUpper());
                            }
                           // string sqlQuery = "Select * from process.mm_session where upper(session_name) like '%"+searchText+"%'";
                            string sqlQuery = "select s.SESSION_ID,s.SESSION_NAME,s.CREATE_DATE,s.CREATE_USER,s.CURRENT_OWNER,s.DESCRIPTION,sta.NAME Status from process.mm_session s, process.mm_px_current_state st, process.mm_px_state sta where s.SESSION_ID= st.SOID and st.STATE_ID= sta.STATE_ID and " + value +"";
                            dt = _pxDb.ExecuteQuery(sqlQuery);
                            if (dt.Rows.Count != 0)
                            {
                                try
                                {
                                    string sPath = GetPath("xls");
                                    SaveToExcel(dt, sPath);

                                }
                                catch (Exception ex)
                                {

                                }

                            }
                            else
                            {
                                MessageBox.Show("Nothing to export");
                            }
                        }

                        if (p_String.Equals("Unknown"))
                        {
                            //_PxApp.Show();
                        }



                        else
                        {
                            //return false;
                            //_PxApp.Show();
                        }
                    }

                    else
                    {

                    }
                    //return true;

                    return (ID8ListItem)_pList1;
                    //_PxApp.Show();

                }

            //Export of SessionList and save to P drive
            //public static bool SaveToExcel(System.Data.DataTable SessionList,string path)
            //{
                
            //    try
            //    {
            //        if (SessionList == null || SessionList.Columns.Count == 0)
            //            throw new Exception("SaveToExcel: Null or empty input datatable");

            //        // load excel, and create a new workbook
            //        Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
            //        excelApp.Workbooks.Add();

            //        // add first worksheet
            //        Microsoft.Office.Interop.Excel._Worksheet workSheet = (Microsoft.Office.Interop.Excel._Worksheet)excelApp.ActiveSheet;

            //        //  datatable column headings 
            //        for (int i = 0; i < SessionList.Columns.Count; i++)
            //        {
            //            workSheet.Cells[1, (i + 1)] = SessionList.Columns[i].ColumnName;
            //        }

            //        // Datatable rows
            //        for (int i = 0; i < SessionList.Rows.Count; i++)
            //        {
            //            // format datetime values before printing
            //            for (int j = 0; j < SessionList.Columns.Count; j++)
            //            {
            //                workSheet.Cells[(i + 2), (j + 1)] = SessionList.Rows[i][j];
            //            }
            //        }

            //        // check fielpath
            //        if (path != null && path != "")
            //        {
            //            try
            //            {
            //                workSheet.SaveAs(path);
            //                excelApp.Quit();
            //                MessageBox.Show("Session Export is downloaded at path : " + path);
            //            }
            //            catch (Exception ex)
            //            {
            //                throw new Exception("SaveToExcel: Excel file could not be saved! Check filepath.\n"
            //                    + ex.Message);
            //            }
            //        }
            //        else    // no filepath is given
            //        {
            //            excelApp.Visible = true;
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        throw new Exception("ExportToExcel: \n" + ex.Message);
            //    }
                

            //    return true;
            //}

            public void SaveToExcel(DataTable dt, string strReportpath)
            {
                System.Windows.Forms.Cursor.Current = Cursors.WaitCursor;
                string stableName = "Session_Report";
                try
                {
                    if (dt.Rows.Count == 0)
                    {
                        return;
                    }
                    System.Data.OleDb.OleDbConnection MyConnection;
                    System.Data.OleDb.OleDbCommand myCommand = new System.Data.OleDb.OleDbCommand();
                    string sql = null;
                    MyConnection = new System.Data.OleDb.OleDbConnection("provider=Microsoft.Jet.OLEDB.4.0;Data Source='" + strReportpath + "';Extended Properties=Excel 8.0;");
                    MyConnection.Open();

                    myCommand.Connection = MyConnection;
                    //Create a Table;;

                    int i = 0;
                    //making table query
                    string strTableQ = "CREATE TABLE [" + stableName + "](";
                    int j = 0;
                    for (j = 0; j <= dt.Columns.Count - 1; j++)
                    {
                        DataColumn dCol = null;
                        dCol = dt.Columns[j];
                        strTableQ += " [" + dCol.ColumnName + "] varchar(255) , ";
                    }
                    strTableQ = strTableQ.Substring(0, strTableQ.Length - 2);
                    strTableQ += ")";
                    OleDbCommand cmd = new OleDbCommand(strTableQ, MyConnection);
                    cmd.ExecuteNonQuery();

                    string strCoumnvalue = "";
                    strCoumnvalue += " (";
                    //making insert query
                    for (int l = 0; l <= dt.Columns.Count - 1; l++)
                    {
                        DataColumn dCol = null;
                        dCol = dt.Columns[l];
                        strCoumnvalue += dCol.ColumnName + ", ";

                    }
                    strCoumnvalue = strCoumnvalue.Substring(0, strCoumnvalue.Length - 2);
                    strCoumnvalue += ")";
                    string strvalues = "";
                    strvalues += " ( ";
                    foreach (DataRow dr in dt.Rows)
                    {
                        strvalues = "";
                        strvalues += " ( ";
                        for (int k = 0; k < dt.Columns.Count; k++)
                        {

                            strvalues += "'" + dr[k].ToString() + "',";


                        }
                        strvalues = strvalues.Substring(0, strvalues.Length - 1);
                        strvalues += ")";

                        sql = "Insert into " + stableName + " values" + strvalues + "";
                        myCommand.CommandText = sql;
                        try
                        {
                            myCommand.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            //WriteLine_Error("Exception occurred while inserting the row ." + ex.Message + " at " + ex.StackTrace);
                        }
                    }

                    MyConnection.Close();
                    MessageBox.Show("Session Export is downloaded at path : " + strReportpath);
                    System.Windows.Forms.Cursor.Current = Cursors.Default;
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.Cursor.Current = Cursors.Default;
                    throw new Exception("SaveToExcel: Excel file could not be saved! Check filepath.\n"
                              + ex.Message);
                    //WriteLine_Error("Exception occurred while exporting the excel.");
                    //WriteLine_Error(ex.Message + " at " + ex.StackTrace);
                }
                finally { System.Windows.Forms.Cursor.Current = Cursors.Default; }
            }


            public static string GetPath(string sModule)
            {
                string sPath = string.Empty;
                //string sPath1 = string.Empty;
                try
                {
                    sPath = (Directory.Exists(@"P:")) ? @"P:\" : Path.GetTempPath();
                    if (Directory.Exists(sPath))
                    {
                        //sPath = (Directory.Exists(@"P:\Session Data")) ? @"P:\Session Data" : Path.GetTempPath();
                        sPath = sPath+ "Session Data";
                        // Try to create the directory.
                        if (!Directory.Exists(sPath))
                        {
                            DirectoryInfo di = Directory.CreateDirectory(sPath);
                        }
                    }

                    else
                    {
                        MessageBox.Show("P Drive does not exist");
                    }
                    string[] sFiles = Directory.GetFiles(sPath, "SessionExport_*" + sModule);
                    //foreach (string sFile in sFiles)
                    //{
                    //    try
                    //    {
                    //        File.Delete(sFile);
                    //    }
                    //    catch { }
                    //}
                    sPath += "\\SessionExport_" + DateTime.Now.Ticks + "." + sModule;
                }
                catch (Exception ex)
                {
                    //_logger.Error("Error getting temp path " + ex.Message);
                }
                //_logger.Info("Error temp file path " + sPath);
                return sPath;
            }
            }

    
}
