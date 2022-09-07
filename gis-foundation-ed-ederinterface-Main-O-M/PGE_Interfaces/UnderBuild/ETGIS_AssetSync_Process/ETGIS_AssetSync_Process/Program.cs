using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Reflection;
using System.Text.RegularExpressions;
using ESRI.ArcGIS.Geodatabase;
using System.Diagnostics;
using PGE_DBPasswordManagement;
using System.Threading;
using System.Runtime.InteropServices;
using System.Data;
using Miner.Interop;
using System.IO;
using Miner.Interop.Process;

namespace ETGIS_AssetSync_Process
{
    class Program
    {
        private static Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "ETGISAssetSync.log4net.config");
        static IWorkspace wEDSpace;
        static IWorkspace wETSpace;
        static IVersion cVersion;       
        private static StringBuilder remark = default;
        private static StringBuilder Argumnet = default;
        private static string InterfaceName = default;
        static IList<string> _listGlobalID = new List<string>();
        public static int SessioniD;
        public static int pole_count = 0;
        public static int tower_count = 0;
        static void Main(string[] args)
        {
            string startTime = DateTime.Now.ToString();
            bool execution = true;
            try 
            {
               execution = Createsession.SessionCreate();               
                if (execution == false)
                {
                    _logger.Error("Sesssion creation get failed.");
                    throw new Exception("Sesssion creation get failed.");                    
                    //execution = false;
                    //return;
                }
                wEDSpace = GetSDEWorkSpace(ReadEncryption.GetSDEPath(ConfigurationManager.AppSettings["ED_SDEConnection"]).ToUpper());
                wETSpace = GetSDEWorkSpace(ReadEncryption.GetSDEPath(ConfigurationManager.AppSettings["ET_SDEConnection"]).ToUpper());                
                if (wEDSpace == null)
                {
                    _logger.Error("Could not get the ED workspace from the SDE file provided");
                    throw new Exception("Could not get the ED workspace from the SDE file provided");
                    //execution = false;
                    //return;
                }
                if (wETSpace == null)
                {
                    _logger.Error("Could not get the ED workspace from the SDE file provided");
                    throw new Exception("Could not get the ED workspace from the SDE file provided");
                    //execution = false;
                    //return;
                }
                cVersion = Createsession.editVersion;
                if (cVersion == null)
                {
                    _logger.Error("Sesssion not found.");
                    throw new Exception("Sesssion not found.");
                    //execution = false;
                    //return;
                }
                //Code block to disable Auto Updaters
                Type type = Type.GetTypeFromProgID("mmGeoDatabase.MMAutoUpdater");
                IMMAutoUpdater auController = (IMMAutoUpdater)Activator.CreateInstance(type);
                mmAutoUpdaterMode prevAUMode = auController.AutoUpdaterMode;
                auController.AutoUpdaterMode = mmAutoUpdaterMode.mmAUMNoEvents;
                try
                {
                    DataTable DT_Pole = ClsDBHelper.GetDataTableByQuery(ConfigurationManager.AppSettings["ED_Pole_Query"]);
                    DataTable DT_Tower = ClsDBHelper.GetDataTableByQuery(ConfigurationManager.AppSettings["ED_Tower_Query"]);
                    Program P1 = new Program();
                    P1.processDatafromET2EDPole(wEDSpace, wETSpace, cVersion, DT_Pole);
                    P1.processDatafromET2EDTower(wEDSpace, wETSpace, cVersion, DT_Tower);                                        
                    if (execution == false)
                    {
                        _logger.Error("Sesssion not found.");
                        throw new Exception("Sesssion not found.");
                        //execution = false;
                        //return;
                    }
                    if (_listGlobalID != null)
                    {
                        var loc = new Uri(Assembly.GetExecutingAssembly().CodeBase);
                        String path = Path.GetDirectoryName(loc.LocalPath);
                        path = path + "\\GlobalIDList.txt";

                        System.IO.File.WriteAllLines(path , _listGlobalID);
                        _listGlobalID.Clear();
                        _listGlobalID.Add(path);                        
                    }
                    execution = Createsession.SubmitSessionToGDBM();
                    P1.sendmail(_listGlobalID);
                }
                catch (Exception ex)
                {
                    _logger.Error("Error Main : " + ex.ToString());
                    execution = false;
                    Environment.ExitCode = 1;
                }
                finally
                {
                    //Code to enable Auto Updaters
                    auController.AutoUpdaterMode = prevAUMode;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error Main : " + ex.ToString());
                //throw new Exception("Sesssion not found.");
                //execution = false;                
                Environment.ExitCode = 1;
            }
            finally
            {
                if (execution == true)
                {
                    ExecutionSummary(startTime, "This application run successfully.", "P");
                }
                else
                {
                    ExecutionSummary(startTime, "This application has given error.", "E");
                }
            }
        }
        private static IWorkspace GetSDEWorkSpace(string sdepath)
        {
            IWorkspace workspace = null;
            try
            {
                // Create an SDE workspace factory and open the workspace.
                Type factoryType = Type.GetTypeFromProgID("esriDataSourcesGDB.SdeWorkspaceFactory");
                IWorkspaceFactory workspaceFactory = (IWorkspaceFactory)Activator.CreateInstance(factoryType);
                workspace = workspaceFactory.OpenFromFile(sdepath, 0);
            }
            catch (Exception ex)
            {
                _logger.Error("Error getting workspace." + ex.ToString());
                throw ex;
                //return null;
            }
            return workspace;
        }
        public void processDatafromET2EDPole(IWorkspace wkEDsp, IWorkspace wkETsp, IVersion cVersion, DataTable DT_Pole)
        {
            string ED_table = string.Empty;
            string ET_PoleTable = string.Empty;
            IFeatureClass ED_supportStructure_fc = null;
            ITable ET_PoleInfo = null;
            bool found = false;
            try
            {
                IFeatureWorkspace featversionWorkspace = (IFeatureWorkspace)cVersion;
                IWorkspaceEdit editSession = (IWorkspaceEdit)featversionWorkspace;
                editSession.StartEditing(true);
                editSession.StartEditOperation();

                //Get the support structure table names from config file
                ED_table = System.Configuration.ConfigurationManager.AppSettings["ED_Table"];
                ET_PoleTable = System.Configuration.ConfigurationManager.AppSettings["ET_PoleTable"];
                _logger.Debug("getting support structure feature class");
                ED_supportStructure_fc = GetfeatureTable(cVersion as IWorkspace, ED_table);
                _logger.Debug("ED support structure table feature class found");
                ET_PoleInfo = GetTable(wkETsp as IWorkspace, ET_PoleTable);
                _logger.Debug("ET Pole Info table feature class found");
                IQueryFilter EDfilter = new QueryFilter();
                IQueryFilter ETfilter = new QueryFilter();
                int tldbIndex = ED_supportStructure_fc.FindField(System.Configuration.ConfigurationManager.AppSettings["GLOBALID"]);
                int ET_field = ET_PoleInfo.FindField(System.Configuration.ConfigurationManager.AppSettings["ET_field"]);
                int ETGIS_ID_field = ET_PoleInfo.FindField(System.Configuration.ConfigurationManager.AppSettings["ED_updatefield"]);
                int EDGIS_ID_field = ED_supportStructure_fc.FindField(System.Configuration.ConfigurationManager.AppSettings["ED_updatefield"]);
                IFeature row = null;
                IRow ET_row = null;

                if (DT_Pole != null)
                {
                    if (DT_Pole.Rows.Count > 0)
                    {
                        foreach (DataRow pRow in DT_Pole.Rows)
                        {                            
                            found = false;
                            ETfilter.WhereClause = ConfigurationManager.AppSettings["ET_Query"] + pRow[0].ToString() + "'";
                            ICursor cursor_pole = ET_PoleInfo.Search(ETfilter, false);
                            if (ETGIS_ID_field != -1)
                            {
                                while ((ET_row = cursor_pole.NextRow()) != null)
                                {
                                    object ETGIS_ID_value = ET_row.get_Value(ETGIS_ID_field);
                                    EDfilter.WhereClause = ConfigurationManager.AppSettings["GLOBALID"] + "= '" + pRow[0].ToString() + "'";
                                    IFeatureCursor cursor = ED_supportStructure_fc.Search(EDfilter, false);
                                    while ((row = cursor.NextFeature()) != null)
                                    {
                                        _logger.Debug("Process Pole GlobalID list :- " + pRow[0].ToString());
                                        row.set_Value(EDGIS_ID_field, ETGIS_ID_value);
                                        //row.set_Value(EDGIS_ID_field, "123456");
                                        //cursor.UpdateFeature(row);
                                        row.Store();
                                        pole_count = pole_count + 1;
                                        found = true;
                                    }
                                    if (cursor != null)
                                    {
                                        Marshal.ReleaseComObject(cursor);
                                    }
                                }
                                if (cursor_pole != null)
                                {
                                    Marshal.ReleaseComObject(cursor_pole);
                                }
                            }
                            if (found == false)
                            {
                                _listGlobalID.Add(pRow[0].ToString());
                            }
                        }
                    }
                }                
                else
                {
                    _logger.Error("GloabalID field not found in support structure");
                    return;
                }
                editSession.StopEditing(true);
                editSession.StopEditOperation();
            }
            catch (Exception ex)
            {
                _logger.Error("Error occured" + ex.ToString());
                throw ex;
            }
        }
        public void processDatafromET2EDTower(IWorkspace wkEDsp, IWorkspace wkETsp, IVersion cVersion, DataTable DT_Tower)
        {
            string ED_table = string.Empty;
            string ET_TowerTable = string.Empty;
            IFeatureClass ED_supportStructure_fc = null;
            ITable ET_TowerInfo = null;
            bool found = false;
            try
            {
                IFeatureWorkspace featversionWorkspace = (IFeatureWorkspace)cVersion;
                IWorkspaceEdit editSession = (IWorkspaceEdit)featversionWorkspace;
                editSession.StartEditing(true);
                editSession.StartEditOperation();

                //Get the support structure table names from config file
                ED_table = System.Configuration.ConfigurationManager.AppSettings["ED_Table"];
                ET_TowerTable = System.Configuration.ConfigurationManager.AppSettings["ET_TowerTable"];
                _logger.Debug("getting support structure feature class");
                ED_supportStructure_fc = GetfeatureTable(cVersion as IWorkspace, ED_table);
                _logger.Debug("ED T_TOWERINFO table feature class found");
                ET_TowerInfo = GetTable(wkETsp as IWorkspace, ET_TowerTable);
                _logger.Debug("ET Pole Info table feature class found");
                IQueryFilter EDfilter = new QueryFilter();
                IQueryFilter ETfilter = new QueryFilter();
                int tldbIndex = ED_supportStructure_fc.FindField(System.Configuration.ConfigurationManager.AppSettings["GLOBALID"]);
                int ET_field = ET_TowerInfo.FindField(System.Configuration.ConfigurationManager.AppSettings["ET_field"]);
                int ETGIS_ID_field = ET_TowerInfo.FindField(System.Configuration.ConfigurationManager.AppSettings["ED_updatefield"]);
                int EDGIS_ID_field = ED_supportStructure_fc.FindField(System.Configuration.ConfigurationManager.AppSettings["ED_updatefield"]);
                IFeature row = null;
                IRow ET_row = null;
                if (DT_Tower != null)
                {
                    if (DT_Tower.Rows.Count > 0)
                    {
                        foreach (DataRow pRow in DT_Tower.Rows)
                        {                            
                            found = false;
                            ETfilter.WhereClause = ConfigurationManager.AppSettings["ET_Query"] + pRow[0].ToString() + "'";
                            ICursor cursor_pole = ET_TowerInfo.Search(ETfilter, false);
                            if (ETGIS_ID_field != -1)
                            {
                                while ((ET_row = cursor_pole.NextRow()) != null)
                                {
                                    object ETGIS_ID_value = ET_row.get_Value(ETGIS_ID_field);
                                    EDfilter.WhereClause = ConfigurationManager.AppSettings["GLOBALID"] + "= '" + pRow[0].ToString() + "'";
                                    IFeatureCursor cursor = ED_supportStructure_fc.Search(EDfilter, false);
                                    while ((row = cursor.NextFeature()) != null)
                                    {
                                        _logger.Debug("Process Tower GlobalID list :- " + pRow[0].ToString());
                                        row.set_Value(EDGIS_ID_field, ETGIS_ID_value);
                                        //cursor.UpdateFeature(row);
                                        row.Store();
                                        tower_count = tower_count + 1;
                                        found = true;
                                    }
                                    if (cursor != null)
                                    {
                                        Marshal.ReleaseComObject(cursor);
                                    }
                                }
                                if (cursor_pole != null)
                                {
                                    Marshal.ReleaseComObject(cursor_pole);
                                }
                            }
                            if (found == false)
                            {
                                _listGlobalID.Add(pRow[0].ToString());
                            }
                        }
                    }
                }                
                else
                {
                    _logger.Error("GloabalID field not found in support structure");
                    return;
                }
                editSession.StopEditing(true);
                editSession.StopEditOperation();
            }
            catch (Exception ex)
            {
                _logger.Error("Error occured" + ex.ToString());
                ((IWorkspaceEdit)cVersion).AbortEditOperation();
                throw ex;
            }
        }
        protected IFeatureClass GetfeatureTable(IWorkspace wSpace, string tableName)
        {
            if (wSpace == null) return null;
            if (string.IsNullOrEmpty(tableName)) return null;
            IWorkspace2 wSpace2 = wSpace as IWorkspace2;
            IFeatureClass table = null;
            IFeatureWorkspace featureWSpace = wSpace as IFeatureWorkspace;
            if (wSpace2.get_NameExists(esriDatasetType.esriDTFeatureClass, tableName))
            {
                table = featureWSpace.OpenFeatureClass(tableName) as IFeatureClass;
            }
            if (table == null)
            {
                _logger.Debug(tableName + " does not exist in " + wSpace.PathName);
            }
            return table;
        }
        protected ITable GetTable(IWorkspace wSpace, string tableName)
        {
            if (wSpace == null) return null;
            if (string.IsNullOrEmpty(tableName)) return null;
            IWorkspace2 wSpace2 = wSpace as IWorkspace2;
            ITable table = null;
            IFeatureWorkspace featureWSpace = wSpace as IFeatureWorkspace;
            if (wSpace2.get_NameExists(esriDatasetType.esriDTTable, tableName))
            {
                table = featureWSpace.OpenTable(tableName) as ITable;
            }
            if (table == null)
            {
                _logger.Debug(tableName + " does not exist in " + wSpace.PathName);
            }
            return table;
        }
        public void sendmail(IList<string> _listGlobalID)
        {
            String strToList = String.Empty;
            String strFromList = String.Empty;
            String strSubject = String.Empty;
            String strMessage = String.Empty;
            String strHost = String.Empty;
            try
            {
                string output = string.Join(Environment.NewLine, _listGlobalID);
                strFromList = ConfigurationManager.AppSettings["strFromList"];
                strToList = ConfigurationManager.AppSettings["strToList"];
                strMessage = ConfigurationManager.AppSettings["strMessage"] + "Pole process count :- " + pole_count + "Tower process count :- " + tower_count;
                strSubject = ConfigurationManager.AppSettings["strSubject"];
                strHost = ConfigurationManager.AppSettings["strHost"];

                EmailService.Send(mail =>
                {
                    mail.From = strFromList;
                    mail.FromDisplayName = strFromList;
                    mail.To = strToList;
                    mail.Subject = strSubject;
                    mail.BodyText = strMessage;
                    mail.Attachments = _listGlobalID;
                });
            }
            catch (Exception ex) { throw ex; }
        }
        public static void ExecutionSummary(string startTime, string comment, string status)
        {
            try
            {
                Argumnet = new StringBuilder();
                remark = new StringBuilder();
                //Setting arguments

                Argumnet.Append(InterfaceName);
                Argumnet.Append("Outbound;");
                Argumnet.Append("Sync");
                Argumnet.Append(startTime + ";");
                remark.Append(comment);
                Argumnet.Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + ";");
                Argumnet.Append(status);
                Argumnet.Append(remark);

                //To execution for interface execution summary exe.This exe must need some input argument to run successffully. 
                ProcessStartInfo processStartInfo = new ProcessStartInfo(ConfigurationManager.AppSettings["IntExecutionSummaryExePath"].ToString(), "\"" + Convert.ToString(Argumnet) + "\"");
                processStartInfo.UseShellExecute = false;
                processStartInfo.RedirectStandardOutput = true;
                processStartInfo.CreateNoWindow = true;

                Process proc = new Process();
                proc.StartInfo = processStartInfo;
                proc.EnableRaisingEvents = true;
                proc.Start();
                proc.BeginOutputReadLine();
                while (!proc.HasExited)
                {
                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception occures while executing the Interface Summary exe" + ex.ToString());
                throw ex;
                //_logger.Info(ex.Message + "   " + ex.StackTrace);
            }
        }
    }
}
