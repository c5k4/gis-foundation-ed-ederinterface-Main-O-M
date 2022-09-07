using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

using Oracle.DataAccess.Client;

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;

namespace PGE.Interfaces.Settings.DivDist
{
    class DivDistUtility
    {
        private IFeatureWorkspace _FWorkspace = null;

        private static object _DivDistLocker = new object();

        private const string DISTRICT = "DISTRICT";
        private const string DIVISION = "DIVISION";
        private const string GLOBALID = "GLOBALID";

        public DivDistUtility(IFeatureWorkspace fWorkspace)
        {
            _FWorkspace = fWorkspace;
        }

        public DataTable Process(IFeature serviceArea)
        {
            DataTable dt = null;
            
            try
            {
                string table = ConfigurationManager.AppSettings["DataTable"];
                dt = DivDistUtility.GetDataTable(table);
                dt.BeginLoadData();

                int districtIndx = serviceArea.Fields.FindField(DISTRICT);
                int divisionIndx = serviceArea.Fields.FindField(DIVISION);

                string district = serviceArea.get_Value(districtIndx) as string;
                string division = serviceArea.get_Value(divisionIndx) as string;

                List<string> featureClassNameList = Regex.Split(ConfigurationManager.AppSettings["FeatureClassList"], ",").ToList<string>();
                List<string> subFeatureClassNameList = Regex.Split(ConfigurationManager.AppSettings["SUBFeatureClassList"], ",").ToList<string>();

                foreach (string featureClassName in featureClassNameList)
                {
                    IFeatureCursor featureCursor = null;
                    try
                    {
                        ITable oc = _FWorkspace.OpenTable(featureClassName);
                        if (oc is IFeatureClass)
                        {
                            IFeatureClass fc = _FWorkspace.OpenFeatureClass(featureClassName);
                            ISpatialFilter filter = new SpatialFilterClass();
                            filter.Geometry = serviceArea.Shape;
                            filter.GeometryField = fc.ShapeFieldName;
                            filter.SpatialRel = esriSpatialRelEnum.esriSpatialRelContains;
                            featureCursor = fc.Search(filter, true);

                            IFeature feature = null;
                            while ((feature = featureCursor.NextFeature()) != null)
                            {
                                int guidIndx = feature.Fields.FindField(GLOBALID);
                                string guid = feature.get_Value(guidIndx) as string;

                                DataRow newRow = dt.NewRow();
                                newRow["GLOBAL_ID"] = guid;
                                newRow["DIVISION"] = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(division.ToLower());
                                newRow["DISTRICT"] = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(district.ToLower());
                                
                                dt.Rows.Add(newRow);
                            }

                        }
                        else
                        {
                            Console.WriteLine("Warning: " + featureClassName + " is not a feature class...");
                            Common.WriteToLog(featureClassName + " is not a feature class...", LoggingLevel.Warning);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error in : " + featureClassName + ".   " + ex.Message);
                        Common.WriteToLog("Error in : " + featureClassName + ".   " + ex.Message, LoggingLevel.Error);
                    }
                    finally
                    {
                        if (featureCursor != null) { while (Marshal.ReleaseComObject(featureCursor) > 0);}
                        featureCursor = null;
                    }
                }

                //SUB features...
                IWorkspace subWorkSpace = Common.SetWorkspace("SDESUBConnectionFile");
                IFeatureWorkspace fSubWorkspace = subWorkSpace as IFeatureWorkspace;

                foreach (string featureClassName in subFeatureClassNameList)
                {
                    IFeatureCursor featureCursor = null;
                    try
                    {
                        ITable objtable = fSubWorkspace.OpenTable(featureClassName);
                        if (objtable is IFeatureClass)
                        {
                            int recordCount = 0;
                            
                            IFeatureClass fc = fSubWorkspace.OpenFeatureClass(featureClassName);
                            ISpatialFilter filter = new SpatialFilterClass();
                            filter.Geometry = serviceArea.Shape;
                            filter.GeometryField = fc.ShapeFieldName;
                            filter.SpatialRel = esriSpatialRelEnum.esriSpatialRelContains;
                            featureCursor = fc.Search(filter, true);

                            IFeature feature = null;

                            while ((feature = featureCursor.NextFeature()) != null)
                            {
                                int guidIndx = feature.Fields.FindField(GLOBALID);
                                string guid = feature.get_Value(guidIndx) as string;

                                DataRow newRow = dt.NewRow();
                                newRow["GLOBAL_ID"] = guid;
                                newRow["DIVISION"] = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(division.ToLower());
                                newRow["DISTRICT"] = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(district.ToLower());

                                dt.Rows.Add(newRow);
                                recordCount++;
                            }

                            if(recordCount > 0)
                                Console.WriteLine("Feature Class Name : " + featureClassName + "... Record Updated=> " + recordCount);
                        }
                        else
                        {
                            Console.WriteLine("Warning: " + featureClassName + " is not a feature class...");
                            Common.WriteToLog(featureClassName + " is not a feature class...", LoggingLevel.Warning);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error in : " + featureClassName + ".   " + ex.Message);
                        Common.WriteToLog("Error in : " + featureClassName + ".   " + ex.Message, LoggingLevel.Error);
                    }
                    finally
                    {
                        if (featureCursor != null) { while (Marshal.ReleaseComObject(featureCursor) > 0);}
                        featureCursor = null;
                    }
                }

                dt.EndLoadData();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                Common.WriteToLog("Error: " + ex.Message, LoggingLevel.Error);
            }

            return dt;
        }


        internal static void SaveToDatabase(DataTable dt, string databaseTableName)
        {
            try
            {
                Thread.Sleep(100);

                using (OracleConnection oraConn = Common.GetDBConnection())
                {
                    using (OracleBulkCopy bulkCopy = new OracleBulkCopy(oraConn, OracleBulkCopyOptions.Default))
                    {
                        bulkCopy.BulkCopyTimeout = Common.OracleTimeout;
                        bulkCopy.BatchSize = 1000;
                        bulkCopy.DestinationTableName = databaseTableName;
                        
                        bulkCopy.WriteToServer(dt);

                        bulkCopy.Close();
                        bulkCopy.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving data... " + ex.Message);
                Common.WriteToLog("Error saving data... " + ex.Message, LoggingLevel.Error);
                throw ex;
            }
        }

        public static DataTable GetDataTable(string tableNameInput)
        {
            OracleConnection conn = null;

            string schema = "";
            string tableName = "";
            try
            {
                string tableNameFromConfig = tableNameInput.ToUpper();
                schema = tableNameFromConfig.Substring(0, tableNameFromConfig.IndexOf('.'));
                tableName = tableNameFromConfig.Substring(tableNameFromConfig.IndexOf('.') + 1, tableNameFromConfig.Length - tableNameFromConfig.IndexOf('.') - 1);

                conn = Common.GetDBConnection();
                DataTable oleschema = conn.GetSchema("Tables");//This returns a table with a list of all the tables in the connected schema
                System.Data.Common.DbCommand GetTableCmd = conn.CreateCommand();
                System.Data.Common.DbDataAdapter ODA = null;

                ODA = new OracleDataAdapter() as System.Data.Common.DbDataAdapter;

                ODA.SelectCommand = GetTableCmd;
                foreach (DataRow row in oleschema.Rows)
                {
                    if (row["OWNER"].Equals(schema))
                    {
                        DataTable DBTable = new DataTable();
                        GetTableCmd.CommandText = "SELECT * FROM " + row["OWNER"] + "." + row["TABLE_NAME"];
                        ODA.FillSchema(DBTable, SchemaType.Source);//This pulls down the schema for the given table
                        if (DBTable.TableName.ToUpper() != tableName.ToUpper())
                        {
                            DBTable = null;
                            continue;
                        }
                        DBTable.TableName = row["OWNER"] + "." + DBTable.TableName;
                        return DBTable;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Common.WriteToLog("Error Getting table: " + tableName + System.Environment.NewLine + ex.ToString(), LoggingLevel.Error);
                throw ex;
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                    conn = null;
                }
            }
        }

    }
}
