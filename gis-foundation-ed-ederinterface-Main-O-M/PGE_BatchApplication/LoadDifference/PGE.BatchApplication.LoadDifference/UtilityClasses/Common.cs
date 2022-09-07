// ========================================================================
// Copyright © 2021 PGE 
// <history>
//  Common Functions Methods 
// YXA6 4/14/2021	Created
// JeeraID-> EDGISRearch-376
// </history>
// All rights reserved.
// ========================================================================
using System;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;
using ADODB;
using ESRI.ArcGIS.DataSourcesOleDB;

namespace PGE.BatchApplication.LoadDifference
{
    public  class Common
    {
        
       
        public  IWorkspace GetWorkspace(string argStrworkSpaceConnectionstring)
        {
            Type t = null;
            IWorkspaceFactory workspaceFactory = null;
            IWorkspace workspace = null;
            try
            {

                t = Type.GetTypeFromProgID("esriDataSourcesGDB.SdeWorkspaceFactory");
                workspaceFactory = (IWorkspaceFactory)Activator.CreateInstance(t);
                workspace = workspaceFactory.OpenFromFile(argStrworkSpaceConnectionstring, 0);
            }
            catch (Exception exp)
            {
                Program._log.Error("Error in getting SDE Workspace, function GetWorkspace: " + exp.Message);
                Program._log.Info(exp.Message + "   " + exp.StackTrace);
                throw exp;
            }
            return workspace;
        }

        public ADODB.Recordset GetRecordset(string strQuery, IWorkspace pworkspace)
        {
            Recordset pRSet = new Recordset();
            Connection _adoConnection = new ConnectionClass();
            try
            {
                pRSet.CursorLocation = CursorLocationEnum.adUseClient;
                IFDOToADOConnection _fdoToadoConnection = new FdoAdoConnectionClass();

                _adoConnection = _fdoToadoConnection.CreateADOConnection(pworkspace as IWorkspace) as ADODB.Connection;

                pRSet.Open(strQuery, _adoConnection, ADODB.CursorTypeEnum.adOpenKeyset, ADODB.LockTypeEnum.adLockBatchOptimistic, 0);

            }
            catch (Exception ex)
            {
                pRSet = null;
            }
            finally
            {
                //if (_adoConnection.State == 1)
                //{
                //    _adoConnection.Close();
                //}
            }
            return pRSet;
        }

        public void Convert_DataTable_To_CSV(DataTable dt, string sCSVFolderPath, string CSVNAME)
        {
            
            StringBuilder sb = new StringBuilder();

            string[] columnNames = dt.Columns.Cast<DataColumn>().
                                              Select(column => column.ColumnName).
                                              ToArray();
            sb.AppendLine(string.Join(",", columnNames));

            foreach (DataRow row in dt.Rows)
            {
                string[] fields = row.ItemArray.Select(field => field.ToString()).
                                                ToArray();
                sb.AppendLine(string.Join(",", fields));
            }

            File.WriteAllText(sCSVFolderPath + "\\" + CSVNAME + ".csv", sb.ToString());
        }

        internal IRelationshipClass GetRelationshipClass(string OriginClassName, IFeatureClass pDestinationClass)
        {
            IRelationshipClass pRelClass = null;
            try
            {
                if (pDestinationClass != null)
                {
                    IEnumRelationshipClass enumrelClass = pDestinationClass.get_RelationshipClasses(ESRI.ArcGIS.Geodatabase.esriRelRole.esriRelRoleAny);
                    pRelClass = enumrelClass.Next();
                    while (pRelClass != null)
                    {
                        if ((pRelClass.OriginClass.AliasName.ToString().ToUpper() == OriginClassName.ToUpper()))
                        {

                            break;
                        }
                        pRelClass = enumrelClass.Next();
                    }
                }
                else
                {
                    Program._log.Debug(" Queried FeatureClass " + pDestinationClass.AliasName.ToString() + " is not present in the data, Please Verify and Try again later.");
                }

            }
            catch (Exception ex)
            {
                Program._log.Info(ex.Message + "   " + ex.StackTrace);
            }
            return pRelClass;
        }
       
        public IFeatureIndex CreateFeatureIndex(IFeatureClass pFeatureClass)
        {
            IFeatureIndex fi = new FeatureIndexClass();
            try
            {

                fi.FeatureClass = pFeatureClass;
                //create the index
                fi.Index(null, ((IGeoDataset)pFeatureClass).Extent);
            }
            catch (Exception ex)
            {
                Program._log.Error(ex.Message + " at " + ex.StackTrace);
            }
            return fi;
        }
       
        internal IFeatureClass GetFeatureClass(IFeatureWorkspace pFeatureWorkspace, string sClassName)
        {
            IFeatureClass returnFeatureclass = null;
            try
            {
                returnFeatureclass = pFeatureWorkspace.OpenFeatureClass(sClassName);
            }
            catch (Exception ex)
            {
                Program._log.Error("Unhandled exception occurred while getting the Featureclass-  ," + sClassName + ", Exception Message :-," + ex.Message.ToString() + " at " + ex.StackTrace);
                throw ex;
            }
            return returnFeatureclass;
        }
        
        #region Update log
        public void WriteLine_Info(string sMsg)
        {
            try
            {

                Program._log.Info(sMsg);
                //sMsg.Replace(')', ' ');
                //sMsg =sMsg + ", at , " +  DateTime.Now + ")";
                
                //StreamWriter pSWriter = default(StreamWriter);
                //pSWriter = File.AppendText(Program.m_sLogFileName);
                //pSWriter.WriteLine("INFO," + sMsg);
                ////DrawProgressBar();
                //Console.WriteLine(sMsg);
                //pSWriter.Close();
            }
            catch (Exception ex)
            {
                //WriteLine_Error("Error in Inserting Transformer Unit records through Stored Procedure," + ex.Message + " at " + ex.StackTrace);
            }
        }

        public void WriteLine_Warn(string sMsg)
        {
            try
            {

                Program._log.Warn(sMsg);
                //StreamWriter pSWriter = default(StreamWriter);
                //pSWriter = File.AppendText(Program.m_sLogFileName);
                //sMsg.Replace(')', ' ');
                //sMsg = sMsg + ", at , " + DateTime.Now + ")";
                //pSWriter.WriteLine("WARNING," + sMsg);
                ////DrawProgressBar();
                //Console.WriteLine(sMsg);
                //pSWriter.Close();
            }
            catch
            {
                //WriteLine_Error("Error in Inserting Transformer Unit records through Stored Procedure," + ex.Message + " at " + ex.StackTrace);
            }
        }

        public void WriteLine_Error(string sMsg)
        {
            try
            {
                Program._log.Error(sMsg);

                //StreamWriter pSWriter = default(StreamWriter);
                //pSWriter = File.AppendText(Program.m_sLogFileName);
                //sMsg.Replace(')', ' ');
                //sMsg = sMsg + ", at , " + DateTime.Now + ")";
                //pSWriter.WriteLine("ERROR," + sMsg);
                ////DrawProgressBar();
                //Console.WriteLine(sMsg);
                //pSWriter.Close();
            }
            catch (Exception ex)
            {
                //WriteLine_Error("Error in Inserting Transformer Unit records through Stored Procedure," + ex.Message + " at " + ex.StackTrace);
            }
        }

        # endregion


    }
}
