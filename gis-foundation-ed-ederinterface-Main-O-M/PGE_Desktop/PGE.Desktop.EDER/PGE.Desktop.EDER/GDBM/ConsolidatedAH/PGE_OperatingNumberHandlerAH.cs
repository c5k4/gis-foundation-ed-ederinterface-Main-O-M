using System;
using System.Collections.Generic;
using System.Data;
using ESRI.ArcGIS.Geodatabase;
using Miner.Geodatabase;
using Miner.Geodatabase.GeodatabaseManager.Logging;
using Miner.Geodatabase.GeodatabaseManager.Serialization;
using PGE.Desktop.EDER.Utility;
using PGE_DBPasswordManagement;

namespace PGE.Desktop.EDER.GDBM
{

    public class PGE_OperatingNumberHandlerAH
    {

        IDictionary<string, IObjectClass> FeatClassList = null;
        private INameLog Log;
        private GdbmReconcilePostService ServiceConfiguration;


        private const int BatchSize = 1000;
        public PGE_OperatingNumberHandlerAH(Miner.Geodatabase.GeodatabaseManager.Logging.INameLog iNameLog, GdbmReconcilePostService ServiceConfiguration)
        {
            // TODO: Complete member initialization
            this.Log = iNameLog;
            this.ServiceConfiguration = ServiceConfiguration;

        }

        [Obsolete]
        public bool UpdateOperatingNumber(IVersion PChildVersion)
        {
            IWorkspace workspace = null;
            IQueryFilter pQueryFilter = new QueryFilterClass();
            try
            {
                workspace = PChildVersion as IWorkspace;


                Dictionary<String, IList<String>> jobNumsDictOpNum = new Dictionary<String, IList<String>>();
                Dictionary<String, IList<String>> jobNumsDictCGC12 = new Dictionary<String, IList<String>>();

                try
                {

                    string selectQuery = "select s.Feat_OID,s.Feat_classname,g.type from " + SchemaInfo.General.pAHInfoTableName + " s inner join SDE.GDB_ITEMS g "
                  + " on s.feat_classname = g.PHYSICALNAME  where((versionname ='" + PChildVersion.VersionName.ToString()
                     + "' and s.Action in ('I')) and g.type in ('{70737809-852C-4A03-9E22-2CECEA5B9BFA}','{CD06BC3B-789D-4C51-AAFA-A467912B8965}'))";
                    // this.Log.Info(ServiceConfiguration.Name.ToString(), "Total Insert Count For  => " + skey.ToString() + " === " + DifferenceList[skey.ToString()].Action.Insert.Count);
                    FeatClassList = new Dictionary<string, IObjectClass>();
                    DataTable dt = (new DBHelper()).GetDataTable(selectQuery);
                    foreach (DataRow DR in dt.Rows)
                    {


                        IRow diffRow = GetRow(Convert.ToInt32(DR[0].ToString()), DR[1].ToString(), DR[2].ToString(), PChildVersion as IFeatureWorkspace);


                        if (ModelNameManager.Instance.ContainsClassModelName((IObjectClass)diffRow.Table, SchemaInfo.Electric.ClassModelNames.CGC12))
                        {
                            String jobNum = diffRow.get_Value(diffRow.Fields.FindField("INSTALLJOBNUMBER")).ToString();

                            if (jobNum != "0")
                            {
                                if (DBNull.Value.Equals(diffRow.get_Value(diffRow.Fields.FindField("CGC12"))))
                                {
                                    if (ModelNameManager.Instance.ContainsClassModelName((IObjectClass)diffRow.Table, SchemaInfo.Electric.ClassModelNames.OperatingNumber))
                                    {

                                        String opNum = diffRow.get_Value(diffRow.Fields.FindField("OPERATINGNUMBER")).ToString();
                                        try
                                        {
                                            if (!jobNumsDictOpNum.ContainsKey(jobNum)) jobNumsDictOpNum[jobNum] = new List<string>();
                                            jobNumsDictOpNum[jobNum].Add(opNum);

                                            this.Log.Info(ServiceConfiguration.Name.ToString(), " Values - Job Number " + jobNum + " Operating Number =  " + opNum);
                                        }
                                        catch (Exception EX)
                                        {
                                            this.Log.Error(EX.Message);
                                        }
                                    }
                                }
                                else
                                {
                                    String cgc12 = diffRow.get_Value(diffRow.Fields.FindField("CGC12")).ToString();
                                    try
                                    {
                                        if (!jobNumsDictCGC12.ContainsKey(jobNum)) jobNumsDictCGC12[jobNum] = new List<string>();
                                        jobNumsDictCGC12[jobNum].Add(cgc12);

                                        this.Log.Info(ServiceConfiguration.Name.ToString(), " Values - Job Number " + jobNum + " CGC12 =  " + cgc12);
                                    }
                                    catch (Exception EX)
                                    {
                                        this.Log.Error(EX.Message);
                                    }
                                }
                            }
                        }
                        else if (ModelNameManager.Instance.ContainsClassModelName((IObjectClass)diffRow.Table, SchemaInfo.Electric.ClassModelNames.OperatingNumber))
                        {
                            String jobNum = diffRow.get_Value(diffRow.Fields.FindField("INSTALLJOBNUMBER")).ToString();
                            String opNum = diffRow.get_Value(diffRow.Fields.FindField("OPERATINGNUMBER")).ToString();
                            try
                            {
                                if (!jobNumsDictOpNum.ContainsKey(jobNum)) jobNumsDictOpNum[jobNum] = new List<string>();
                                jobNumsDictOpNum[jobNum].Add(opNum);

                                this.Log.Info(ServiceConfiguration.Name.ToString(), " Values - Job Number " + jobNum + " Operating Number =  " + opNum);
                            }
                            catch (Exception EX)
                            {
                                this.Log.Error(EX.Message);
                            }
                        }


                    }
                }
                catch (Exception EX)
                {
                    this.Log.Error(EX.Message);
                }

                string username = "WIP_RO";

                CommonFunctions.gConnectionstring = ReadEncryption.GetConnectionStr("EDGIS@EDER");
                string password = ReadEncryption.GetPassword(username + "@WIP");
                WipDbApi api = new WipDbApi(username, password, this.Log);
                this.Log.Info(ServiceConfiguration.Name.ToString(), " Job Number and Operating Number Update  Started");
                if (jobNumsDictOpNum.Count > 0)
                {
                    api.CheckRetireInJETJobsTable(jobNumsDictOpNum, true);
                }
                if (jobNumsDictCGC12.Count > 0)
                {
                    api.CheckRetireInJETJobsTable(jobNumsDictCGC12, false);
                }
                this.Log.Info(ServiceConfiguration.Name.ToString(), " Job Number and Operating Number Update  Completed");
                return true;
            }
            catch (Exception EX)
            {
                this.Log.Error(EX.Message + " - " + EX.StackTrace);
                return false;
            }
            finally
            {
            }
        }

        public IRow GetRow(int OID, string sClassName, string classtype, IFeatureWorkspace pFWSpace)
        {
            IObjectClass pFeatureclass = null;
            IRow returnrow = null;
            try
            {

                FeatClassList.TryGetValue(sClassName, out pFeatureclass);
                if (pFeatureclass is null)
                {
                    if(classtype== "{70737809-852C-4A03-9E22-2CECEA5B9BFA}")
                    {
                        pFeatureclass = pFWSpace.OpenFeatureClass(sClassName);
                    }
                    else if(classtype == "{CD06BC3B-789D-4C51-AAFA-A467912B8965}")
                    {
                        pFeatureclass = pFWSpace.OpenTable(sClassName) as IObjectClass;
                    }
                }

                if (pFeatureclass != null)
                {
                    returnrow = (pFeatureclass as ITable).GetRow(OID);
                    if (!FeatClassList.ContainsKey(sClassName))
                    {
                        FeatClassList.Add(sClassName, pFeatureclass);
                    }
                }
                else
                {
                    Log.Info(ServiceConfiguration.Name.ToString(), "Featureclass not found---" + sClassName);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return returnrow;
        }
    }
}

