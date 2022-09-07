////////////////////////////////////////////////////////////////////////
//	Purpose	        : global static class to share info between different projects
//	Date Created	: June 10, 2014
//	Author		    : Tata Consultancy Services
//////////////////////////////////////////////////////////////////////
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;
using Telvent.Delivery.Diagnostics;
using System.Reflection;


namespace Telvent.PGE.ED.Desktop.ArcMapCommands.PONS
{
    static class PGEGlobal
    {
        //Logger to be accessed by all projects
        #region private variables
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        //private static log4net.ILog logger = null;
        private static string wizardDivision = "";
        private static string wizardDivisionCode = "";
        private static string selectedCircuitId = "";
        private static IApplication pApplication = null;
        private static string sConfigFilePath = string.Empty;
 
        #endregion
        public static IWorkspace WORKSPACE_MAP=null;
        public static IWorkspace WORKSPACE_EDER = null;
        public static IWorkspace WORKSPACE_EDERSUB = null;
       
        #region Properties
        public static IApplication Application
        {
            get
            { return pApplication; }
            set
            { pApplication = value; }
        }

        //public static log4net.ILog Logger
        //{
        //    get
        //    {
        //        return logger;
        //    }
        //    set
        //    {
        //        logger = value;
        //    }
        //}

        public static string WIZARD_DIVISION
        {
            get
            {
                return wizardDivision;
            }
            set
            {
                wizardDivision = value;
            }
        }
        public static string WIZARD_DIVISION_CODE
        {
            get
            {
                return wizardDivisionCode;
            }
            set
            {
                wizardDivisionCode = value;
            }
        }

        public static string SELECTED_CIRCUITID
        {
            get
            {
                return selectedCircuitId;
            }
            set
            {
                selectedCircuitId = value;
            }
        }

        public static string ConfigFilePath
        {
            get { return sConfigFilePath; }
            set { sConfigFilePath = value; }
        }

        //private static string SelectedStartObjectClassName = string.Empty, SelectedEndObjectClassName = string.Empty,
        //    SelectedStartDeviceCircuitID = string.Empty, SelectedEndDeviceCircuitID = string.Empty;
        //private static long SelectedStartDeviceOID = -1, SelectedEndDeviceOID = -1;

        //public static string SELECTED_START_OBJECT_CLASS_NAME
        //{
        //    get
        //    {
        //        return SelectedStartObjectClassName;
        //    }
        //    set
        //    {
        //        SelectedStartObjectClassName = value;
        //    }
        //}

        //public static string SELECTED_END_OBJECT_CLASS_NAME
        //{
        //    get
        //    {
        //        return SelectedEndObjectClassName;
        //    }
        //    set
        //    {
        //        SelectedEndObjectClassName = value;
        //    }
        //}

        //public static string SELECTED_START_DEVICE_CIRCUITID
        //{
        //    get
        //    {
        //        return SelectedStartDeviceCircuitID;
        //    }
        //    set
        //    {
        //        SelectedStartDeviceCircuitID = value;
        //    }
        //}

        //public static string SELECTED_END_DEVICE_CIRCUITID
        //{
        //    get
        //    {
        //        return SelectedEndDeviceCircuitID;
        //    }
        //    set
        //    {
        //        SelectedEndDeviceCircuitID = value;
        //    }
        //}

        //public static long SELECTED_START_DEVICE_OID
        //{
        //    get { return SelectedStartDeviceOID; }
        //    set { SelectedStartDeviceOID = value; }
        //}

        //public static long SELECTED_END_DEVICE_OID
        //{
        //    get { return SelectedEndDeviceOID; }
        //    set { SelectedEndDeviceOID = value; }
        //}

        private static Equipment startEquipment = default(Equipment), endEquipment = default(Equipment);

        public static Equipment START_EQUIPMENT
        {
            get { return startEquipment; }
            set { startEquipment = value; }
        }

        public static Equipment END_EQUIPMENT
        {
            get { return endEquipment; }
            set { endEquipment = value; }
        }
     
        #endregion

        #region Private methods
        
        #endregion Private methods

        #region Public methods

        /// <summary>
        /// LayerPresentonMap
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static bool LayerPresentonMap(IApplication app)
        {
            IMxDocument mxdoc = app.Document as IMxDocument;
            int i = mxdoc.FocusMap.LayerCount;
            if (i > 0)
                return true;
            return false;
        }
        /// <summary>
        /// CheckIfFormIsOpen
        /// </summary>
        /// <param name="formname"></param>
        /// <returns></returns>
        public static bool CheckIfFormIsOpen(string formname)
        {

           bool formOpen=  System.Windows.Forms.Application.OpenForms.Cast<Form>().Any(form => form.Name == formname);

            return formOpen;
        }
          /// <summary>
        /// ExportToExcelwithoutLIC
          /// </summary>
          /// <param name="Tbl"></param>
          /// <param name="excelfileName"></param>
   
        public static void ExportToExcelwithoutLIC(System.Data.DataTable Tbl,string excelfileName)
      {
          //try
          //{
          //    if (Tbl == null || Tbl.Columns.Count == 0)
          //        return;
          //    Boolean showmsg = true;
          //    string appLocalUserPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
          //    string docPath = System.IO.Path.Combine(appLocalUserPath, Properties.Resources.PRINT_REL_PATH);
          //    string date=DateTime.Now.ToString("yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture);
          //    string path = docPath + @"\" + date;

          //    if (excelfileName.Length == 0)//give option to browse
          //    {
          //        SaveFileDialog saveDlg = new SaveFileDialog();
          //        saveDlg.InitialDirectory = @"C:\";
          //        saveDlg.FileName = "Report1";
          //        saveDlg.Filter = "Excel Worksheets|*.xls";
          //        if (saveDlg.ShowDialog(new ArcMapWindow(ArcMap.Application)) == DialogResult.OK)
          //        {
          //            path = saveDlg.FileName;
          //            showmsg = false;
          //        }
          //        else
          //        {
          //            return;
          //        }

          //    }
          //    if (showmsg)
          //    {
          //        DirectoryInfo di = Directory.CreateDirectory(path);
          //        path = path + @"\" + excelfileName;
          //    }



          //    System.IO.FileStream stream = new System.IO.FileStream(path , FileMode.Create);
          //    ExcelWriter writer = new ExcelWriter(stream);
          //    writer.BeginWrite();

          //    // column headings
          //    for (int i = 0; i < Tbl.Columns.Count; i++)
          //    {
          //        writer.WriteCell(0, (i),Tbl.Columns[i].Caption);
          //    }


          //    // rows
          //    for (int i = 0; i < Tbl.Rows.Count; i++)
          //    {
          //        // to do: format datetime values before printing
          //        for (int j = 0; j < Tbl.Columns.Count; j++)
          //        {
          //            writer.WriteCell((i + 1), (j), Tbl.Rows[i][j].ToString());                     
          //        }
          //    }
             
          //    writer.EndWrite();
          //    stream.Close();
          //    if (showmsg)
          //    {
          //        System.Windows.Forms.MessageBox.Show("The data exported at " + path + @"\" + excelfileName + ".", "PGE ETGIS", MessageBoxButtons.OK, MessageBoxIcon.Information);
          //    }
                           
          //}
          //catch (Exception ex)
          //{
             
          //}
      }
        /// <summary>
        /// ExportToExcelwithoutLIC
        /// </summary>
        /// <param name="Tbl"></param>
        /// <param name="excelfileName"></param>
        /// <param name="showMessage"></param>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public static string ExportToExcelwithoutLIC(System.Data.DataTable Tbl, string excelfileName,Boolean showMessage,Boolean fullPath)
      {
          //try
          //{
          //    if (Tbl == null || Tbl.Columns.Count == 0)
          //        return "";
          //    string path="";
          //    DirectoryInfo di;
          //    System.IO.FileStream stream;
          //    if (fullPath==false)
          //    {
          //        string appLocalUserPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
          //        string docPath = System.IO.Path.Combine(appLocalUserPath, Properties.Resources.PRINT_REL_PATH);
          //        string date = DateTime.Now.ToString("yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture);
          //        path = docPath + @"\" + date.Substring(0, date.Length - 2);
          //        di = Directory.CreateDirectory(path);
          //    }
          //    else
          //    {
          //        path = excelfileName.Substring(0, excelfileName.LastIndexOf('\\'));
          //        if (!Directory.Exists(path))
          //        {
          //            di = Directory.CreateDirectory(path);
          //        }
          //    }

          //    if (fullPath == false)
          //    {
          //       stream = new System.IO.FileStream(path + @"\" + excelfileName, FileMode.Create);
          //    }
          //    else
          //    {
          //        stream = new System.IO.FileStream(excelfileName, FileMode.Create);
          //    }
          //    //ExcelWriter writer = new ExcelWriter(stream);
          //    //writer.BeginWrite();

          //    // column headings
          //    for (int i = 0; i < Tbl.Columns.Count; i++)
          //    {
          //        writer.WriteCell(0, (i), Tbl.Columns[i].Caption);
          //    }


          //    // rows
          //    for (int i = 0; i < Tbl.Rows.Count; i++)
          //    {
          //        // to do: format datetime values before printing
          //        for (int j = 0; j < Tbl.Columns.Count; j++)
          //        {
          //            writer.WriteCell((i + 1), (j), Tbl.Rows[i][j].ToString());
          //        }
          //    }

          //    writer.EndWrite();
          //    stream.Close();
          //    if (showMessage)
          //    {
          //         System.Windows.Forms.MessageBox.Show("The data exported at " + path + @"\" + excelfileName + ".", "PGE ETGIS", MessageBoxButtons.OK, MessageBoxIcon.Information);

          //    } 
          //    return path + @"\" + excelfileName;
          //}
          //catch (Exception ex)
          //{
          //    Logger.Error(ex.Message);
          //}
          return "";
      }
      
        /// <summary>
        /// ExportToExcelDGV
        /// </summary>
        /// <param name="dgv"></param>
        /// <param name="excelfilename"></param>
        public static void ExportToExcelDGV(DataGridView dgv,string excelfilename)
      {
          try
          {
              System.Data.DataTable Tbl = null;
              if (excelfilename == null)
              {
                   Tbl = ToDataTable(dgv, "PGE_Exported_for_Print");
              }
              else if (excelfilename.Length == 0)
              {
                 Tbl = ToDataTable(dgv, "PGE_Exported_for_Print");
              }
              else
              {
                   Tbl = ToDataTable(dgv, excelfilename);
              }

              ExportToExcelwithoutLIC(Tbl,excelfilename);
          }
          catch (Exception ex)
          {
              _logger.Error(ex.Message);
          }
      }
        /// <summary>
        /// ExportToExcelDGVwithpath
        /// </summary>
        /// <param name="dgv"></param>
        /// <param name="excelfilename"></param>
        /// <returns></returns>
        public static string ExportToExcelDGVwithpath(DataGridView dgv, string excelfilename)
       {
           try
           {
               System.Data.DataTable Tbl = null;
               if (excelfilename == null)
               {
                   Tbl = ToDataTable(dgv, "PGE_Exported_for_Print");
               }
               else if (excelfilename.Length == 0)
               {
                   Tbl = ToDataTable(dgv, "PGE_Exported_for_Print");
               }
               else
               {
                   Tbl = ToDataTable(dgv, excelfilename);
               }

              return ExportToExcelwithoutLIC(Tbl, excelfilename,false,true);
           }
           catch (Exception ex)
           {
               _logger.Error(ex.Message);
           }
           return "";
       }
          /// <summary>
        /// ToDataTable
          /// </summary>
          /// <param name="dataGridView"></param>
          /// <param name="tableName"></param>
          /// <returns></returns>
 
        public static System.Data.DataTable ToDataTable(this DataGridView dataGridView, string tableName)
      {
          System.Data.DataTable table = new System.Data.DataTable(tableName);
          try
          {
          DataGridView dgv = dataGridView;
          
          int icolt = 0;
          // Crea las columnas 
          for (int iCol = 0; iCol < dgv.Columns.Count; iCol++)
          {
              if (dgv.Columns[iCol].Visible)
              {
                  table.Columns.Add(dgv.Columns[iCol].Name);
                  table.Columns[table.Columns.Count - 1].Caption = dgv.Columns[iCol].HeaderText;
              }
          }

        
          foreach (DataGridViewRow row in dgv.Rows)
          {

              DataRow datarw = table.NewRow();

              for (int iCol = 0; iCol < dgv.Columns.Count; iCol++)
              {
                  if (dgv.Columns[iCol].Visible)
                  {
                      datarw[icolt] = row.Cells[iCol].Value;
                      icolt = icolt + 1;
                  }
                  
              }
              icolt = 0;
              table.Rows.Add(datarw);
          }
          }
          catch (Exception ex)
          {
              _logger.Error(ex.Message);
          }

          return table;
      }


        #endregion Public methods
    }
   
}
