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
using PGE.BatchApplication.GDBMAHBatchJobs.UtilityClasses;

namespace PGE.BatchApplication.GDBMAHBatchJobs
{
    public  class Common
    {
        
        public static Log4NetLogger _log = new Log4NetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, ReadConfigurations.LOGCONFIG);
       
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
                Common._log.Error("Error in getting SDE Workspace, function GetWorkspace: " + exp.Message);
                Common._log.Info(exp.Message + "   " + exp.StackTrace);
                throw exp;
            }
            return workspace;
        }

        public  void Convert_DataTable_To_CSV(DataTable dt, string sCSVFolderPath, string CSVNAME)
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
                   _log.Debug(" Queried FeatureClass " + pDestinationClass.AliasName.ToString() + " is not present in the data, Please Verify and Try again later.");
                }

            }
            catch (Exception ex)
            {
                Common._log.Info(ex.Message + "   " + ex.StackTrace);
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
                Common._log.Error(ex.Message + " at " + ex.StackTrace);
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
                _log.Error("Unhandled exception occurred while getting the Featureclass-  ," + sClassName + ", Exception Message :-," + ex.Message.ToString() + " at " + ex.StackTrace);
                throw ex;
            }
            return returnFeatureclass;
        }
        /// <summary>
        /// Getting AHTable Row from FeatureClass
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="pFWSpace"></param>
        /// <returns></returns>
        public  IRow GetRow(DataRow dr, IFeatureWorkspace pFWSpace)
        {
            IFeatureClass pFeatureclass = null;
            IRow returnrow = null;
            try
            {
                string sclassname = dr["FEATURECLASSNAME"].ToString();
                ChildProcess.Featureclasslist.TryGetValue(sclassname, out pFeatureclass);
                if (pFeatureclass is null)
                {
                    pFeatureclass = pFWSpace.OpenFeatureClass(dr["FEATURECLASSNAME"].ToString());
                }
                if (pFeatureclass != null)
                {
                    returnrow = (IRow)pFeatureclass.GetFeature(Convert.ToInt32( dr["FEATOID"].ToString()));
                    if (!ChildProcess.Featureclasslist.ContainsKey(dr["FEATURECLASSNAME"].ToString()))
                    {
                        ChildProcess.Featureclasslist.Add(dr["FEATURECLASSNAME"].ToString(), pFeatureclass);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error("Unhandled exception occurred while getting the Feature " + ex.Message.ToString() );
                throw ex;
            }
            return returnrow;
        }
        #region Update log
        public void WriteLine_Info(string sMsg)
        {
            try
            {
                if (MainClass.m_blIfChild == false)
                {
                    _log.Info(sMsg);
                    return;
                }
                sMsg.Replace(')', ' ');
                sMsg =sMsg + ", at , " +  DateTime.Now + ")";
                
                StreamWriter pSWriter = default(StreamWriter);
                pSWriter = File.AppendText(MainClass.m_sLogFileName);
                pSWriter.WriteLine("INFO," + sMsg);
                //DrawProgressBar();
                Console.WriteLine(sMsg);
                pSWriter.Close();
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
                if (MainClass.m_blIfChild == false)
                {
                    _log.Warn(sMsg);
                    return;
                }

                StreamWriter pSWriter = default(StreamWriter);
                pSWriter = File.AppendText(MainClass.m_sLogFileName);
                sMsg.Replace(')', ' ');
                sMsg = sMsg + ", at , " + DateTime.Now + ")";
                pSWriter.WriteLine("WARNING," + sMsg);
                //DrawProgressBar();
                Console.WriteLine(sMsg);
                pSWriter.Close();
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
                if (MainClass.m_blIfChild == false)
                {
                    _log.Error(sMsg);
                    return;
                }
                StreamWriter pSWriter = default(StreamWriter);
                pSWriter = File.AppendText(MainClass.m_sLogFileName);
                sMsg.Replace(')', ' ');
                sMsg = sMsg + ", at , " + DateTime.Now + ")";
                pSWriter.WriteLine("ERROR," + sMsg);
                //DrawProgressBar();
                Console.WriteLine(sMsg);
                pSWriter.Close();
            }
            catch (Exception ex)
            {
                //WriteLine_Error("Error in Inserting Transformer Unit records through Stored Procedure," + ex.Message + " at " + ex.StackTrace);
            }
        }

        # endregion


    }
}
