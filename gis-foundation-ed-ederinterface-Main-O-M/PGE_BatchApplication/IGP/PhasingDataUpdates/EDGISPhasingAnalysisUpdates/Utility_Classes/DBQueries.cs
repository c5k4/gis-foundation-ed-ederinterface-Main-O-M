using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PGE.BatchApplication.IGPPhaseUpdate
{
    public  class DBQueries
    {
        Common  CommonFuntions  = new Common ();
        public  string GetQueryForNextConductors(string sFeederID, string sOID_Transformer)
        {
            string sQuery = null;
            string sFeederNetworkTraceTable = null;
            try
            {
                sFeederNetworkTraceTable = ReadConfigurations.FeederNetworkTraceTableName;

                sQuery = "select a.ORDER_NUM,a.FROM_FEATURE_EID,a.TO_FEATURE_EID, a.TO_FEATURE_FCID,b.physicalname, a.TO_FEATURE_OID,a.TO_FEATURE_GLOBALID,a.MIN_BRANCH,a.MAX_BRANCH,a.TREELEVEL,a.FEEDERFEDBY,a.FEEDERID" +
                    " from " + sFeederNetworkTraceTable + " a left join " + ReadConfigurations.GDBITEMSTableName + " b" +
                    " on a.to_feature_fcid = b.objectid where" +
                    " (a.FEEDERID = '" + sFeederID + "' OR a.FEEDERFEDBY = '" + sFeederID + "')  " +
                    " and a.MIN_BRANCH>=(select distinct MIN_BRANCH from " + sFeederNetworkTraceTable + " where TO_FEATURE_OID =" + sOID_Transformer + " and TO_FEATURE_FCID = 1001 AND  FEEDERID = '" + sFeederID + "' and MIN_BRANCH <= MAX_BRANCH and rownum = 1)" +
                    " AND a.MAX_BRANCH <= (select distinct MAX_BRANCH from " + sFeederNetworkTraceTable + " where TO_FEATURE_OID =" + sOID_Transformer + " and TO_FEATURE_FCID = 1001 AND FEEDERID = '" + sFeederID + "' and MIN_BRANCH <= MAX_BRANCH and rownum = 1) " +
                    " AND a.TREELEVEL = 1+ (select distinct TREELEVEL from " + sFeederNetworkTraceTable + " where TO_FEATURE_OID = " + sOID_Transformer + " and  TO_FEATURE_FCID = 1001 AND FEEDERID = '" + sFeederID + "' and MIN_BRANCH <= MAX_BRANCH and rownum = 1)  " +
                    " AND a.ORDER_NUM <= (select distinct ORDER_NUM from " + sFeederNetworkTraceTable + " where TO_FEATURE_OID =" + sOID_Transformer + " and TO_FEATURE_FCID = 1001 AND FEEDERID = '" + sFeederID + "' and MIN_BRANCH <= MAX_BRANCH and rownum = 1) " +
                    " and b.PHYSICALNAME not in ('EDGIS.PRIOHCONDUCTOR','EDGIS.PRIUGCONDUCTOR','EDGIS.DISTBUSBAR') order by a.order_num desc";
            }
            catch (Exception exp)
            {
                CommonFuntions.WriteLine_Error("Exception occurred." + exp.Message.ToString());
            }
            return sQuery;
        }

    
        public  string GetDownStreamFeatures(string sFeederID, string sObjectId, string sPhysicalName)
        {
            string sQuery = null;
            string sFeederNetworkTraceTable = null;
            try
            {
                sFeederNetworkTraceTable = ReadConfigurations.FeederNetworkTraceTableName;

                sQuery = "select a.ORDER_NUM,a.FROM_FEATURE_EID,a.TO_FEATURE_EID, a.TO_FEATURE_FCID,b.PHYSICALNAME, a.TO_FEATURE_OID,a.TO_FEATURE_GLOBALID," +
                    " a.MIN_BRANCH,a.MAX_BRANCH,a.TREELEVEL,a.FEEDERFEDBY,a.FEEDERID,a.TO_FEATURE_FEEDERINFO " +
                    " from " + sFeederNetworkTraceTable + " a left join " + ReadConfigurations.GDBITEMSTableName + " b " +
                    " on a.to_feature_fcid = b.objectid where " +
                    " (a.FEEDERID = '" + sFeederID + "' OR a.FEEDERFEDBY = '" + sFeederID + "')   " +
                    " and a.MIN_BRANCH>=(select MIN_BRANCH from " + sFeederNetworkTraceTable + " a left join " + ReadConfigurations.GDBITEMSTableName + " b " +
                    " on a.to_feature_fcid = b.objectid where TO_FEATURE_OID ='" + sObjectId + "' AND  FEEDERID = '" + sFeederID + "' and b.physicalname = '" + sPhysicalName + "' ) " +
                    " AND a.MAX_BRANCH <= (select MAX_BRANCH from " + sFeederNetworkTraceTable + " a left join " + ReadConfigurations.GDBITEMSTableName + " b " +
                    " on a.to_feature_fcid = b.objectid where TO_FEATURE_OID ='" + sObjectId + "' AND FEEDERID = '" + sFeederID + "' and b.physicalname = '" + sPhysicalName + "' )  " +
                    " AND a.TREELEVEL >= (select TREELEVEL from " + sFeederNetworkTraceTable + " a left join " + ReadConfigurations.GDBITEMSTableName + " b " +
                    " on a.to_feature_fcid = b.objectid where TO_FEATURE_OID = '" + sObjectId + "' and   FEEDERID = '" + sFeederID + "' and b.physicalname = '" + sPhysicalName + "' )   " +
                    " AND a.ORDER_NUM <= (select ORDER_NUM from " + sFeederNetworkTraceTable + " a left join " + ReadConfigurations.GDBITEMSTableName + " b " +
                    " on a.to_feature_fcid = b.objectid where TO_FEATURE_OID ='" + sObjectId + "' AND FEEDERID = '" + sFeederID + "' and b.physicalname = '" + sPhysicalName + "')  " +
                    " order by a.order_num desc";
            }
            catch (Exception exp)
            {
                CommonFuntions.WriteLine_Error("Exception occurred." + exp.Message.ToString());
            }
            return sQuery;
        }

        public  string GetUpstreamFeaturesTillDtr(string sFeederId, int iDtrOid, int iOid, int iMinBranch)
        {
            string sQuery = null;
            string sFeederNetworkTraceTable = null;
            try
            {
                sFeederNetworkTraceTable = ReadConfigurations.FeederNetworkTraceTableName;

                sQuery = "select a.ORDER_NUM,a.TO_FEATURE_FCID,b.physicalname, a.TO_FEATURE_OID,a.TO_FEATURE_GLOBALID,a.MIN_BRANCH,a.MAX_BRANCH,a.TREELEVEL,a.FEEDERFEDBY," +
                    "a.FEEDERID,a.TO_FEATURE_FEEDERINFO,a.TO_FEATURE_EID from " + sFeederNetworkTraceTable + " a left join " + ReadConfigurations.GDBITEMSTableName + " b on a.to_feature_fcid = b.objectid  where (a.FEEDERID = (select FEEDERID " +
                    "from " + sFeederNetworkTraceTable + " where TO_FEATURE_oid = " + iOid + " AND FEEDERID = '" + sFeederId + "' AND MIN_BRANCH = " + iMinBranch + ") OR a.FEEDERFEDBY = (select FEEDERFEDBY from " +
                    "" + sFeederNetworkTraceTable + " where TO_FEATURE_oid = " + iOid + " AND FEEDERID = '" + sFeederId + "' AND MIN_BRANCH = " + iMinBranch + ")) AND a.MIN_BRANCH <= (select MIN_BRANCH from " +
                    "" + sFeederNetworkTraceTable + " where TO_FEATURE_oid = " + iOid + " AND FEEDERID = '" + sFeederId + "' AND MIN_BRANCH = " + iMinBranch + ") AND a.MAX_BRANCH >= (select MAX_BRANCH from " +
                    "" + sFeederNetworkTraceTable + " where TO_FEATURE_oid = " + iOid + " AND FEEDERID = '" + sFeederId + "' AND MIN_BRANCH = " + iMinBranch + ") AND a.TREELEVEL <= (select TREELEVEL from " +
                    "" + sFeederNetworkTraceTable + " where TO_FEATURE_oid = " + iOid + " AND FEEDERID = '" + sFeederId + "' AND MIN_BRANCH = " + iMinBranch + ") AND a.ORDER_NUM between (select ORDER_NUM from " +
                    "" + sFeederNetworkTraceTable + " where TO_FEATURE_oid = " + iOid + " AND FEEDERID = '" + sFeederId + "' AND MIN_BRANCH = " + iMinBranch + ") and (SELECT a.ORDER_NUM FROM " + sFeederNetworkTraceTable + " a " +
                    "LEFT JOIN " + ReadConfigurations.GDBITEMSTableName + " b on a.TO_FEATURE_FCID = b.OBJECTID where a.TO_FEATURE_OID = " + iDtrOid + " and b.physicalname = 'EDGIS.TRANSFORMER') order by a.order_num asc";

            }
            catch (Exception exp)
            {
                CommonFuntions.WriteLine_Error("Exception occurred." + exp.Message.ToString());
            }
            return sQuery;
        }

        public  string GetQuery_DownstreamFeaturePhaseDesignation(string sOid, string sFeederID)
        {
            string sQuery = null;
            try
            {
                sQuery = "select a.TO_Feature_GLOBALID,b.physicalname,a.TO_FEATURE_OID from " + ReadConfigurations.FeederNetworkTraceTableName + " a left join " + ReadConfigurations.GDBITEMSTableName + " b " +
                       " on a.to_feature_fcid = b.objectid where " +
                       " (a.FEEDERID = '" + sFeederID + "' OR a.FEEDERFEDBY = '" + sFeederID + "') and to_feature_FCID in ('1021','1023','1019') " +
                       " and a.MIN_BRANCH>=(select MIN_BRANCH from " + ReadConfigurations.FeederNetworkTraceTableName + " where TO_FEATURE_OID =" + sOid + " AND  FEEDERID = '" + sFeederID + "' and ROWNUM=1 )" +
                       " AND a.MAX_BRANCH <= (select MAX_BRANCH from " + ReadConfigurations.FeederNetworkTraceTableName + " where TO_FEATURE_OID =" + sOid + " AND FEEDERID = '" + sFeederID + "' and ROWNUM=1 ) " +
                       " AND a.TREELEVEL =1+ (select TREELEVEL from " + ReadConfigurations.FeederNetworkTraceTableName + " where TO_FEATURE_OID =	" + sOid + " and   FEEDERID = '" + sFeederID + "'and ROWNUM=1  )  " +
                       " AND a.ORDER_NUM <= (select ORDER_NUM from " + ReadConfigurations.FeederNetworkTraceTableName + " where TO_FEATURE_OID =" + sOid + " AND FEEDERID = '" + sFeederID + "'and ROWNUM=1 ) " +
                       " order by a.order_num desc";
            }
            catch (Exception exp)
            {
                CommonFuntions.WriteLine_Error("Exception occurred." + exp.Message.ToString());
            }
            return sQuery;
        }

        public string GetConnectedFeatures(string sFeederID, string sOID, string FCID)
        {
            string sQuery = null;
            string sFeederNetworkTraceTable = null;
            try
            {
                sFeederNetworkTraceTable = ReadConfigurations.FeederNetworkTraceTableName;

                sQuery = "select a.FROM_FEATURE_EID,a.TO_FEATURE_EID, a.TO_FEATURE_FCID,b.physicalname, a.TO_FEATURE_OID  ,TO_FEATURE_GLOBALID " +
                    " from " + sFeederNetworkTraceTable + " a left join " + ReadConfigurations.GDBITEMSTableName + " b" +
                    " on a.to_feature_fcid = b.objectid where" +
                    " (a.FEEDERID = '" + sFeederID + "' OR a.FEEDERFEDBY = '" + sFeederID + "')  " +
                    " and a.MIN_BRANCH>=(select distinct MIN_BRANCH from " + sFeederNetworkTraceTable + " where TO_FEATURE_OID =" + sOID + " and TO_FEATURE_FCID = " + FCID + " AND  FEEDERID = '" + sFeederID + "' and MIN_BRANCH <= MAX_BRANCH and rownum = 1)" +
                    " AND a.MAX_BRANCH <= (select distinct MAX_BRANCH from " + sFeederNetworkTraceTable + " where TO_FEATURE_OID =" + sOID + " and TO_FEATURE_FCID = " + FCID + " AND FEEDERID = '" + sFeederID + "' and MIN_BRANCH <= MAX_BRANCH and rownum = 1) " +
                    " AND a.TREELEVEL = 1+ (select distinct TREELEVEL from " + sFeederNetworkTraceTable + " where TO_FEATURE_OID = " + sOID + " and  TO_FEATURE_FCID = " + FCID + " AND FEEDERID = '" + sFeederID + "' and MIN_BRANCH <= MAX_BRANCH and rownum = 1)  " +
                    " AND a.ORDER_NUM <= (select distinct ORDER_NUM from " + sFeederNetworkTraceTable + " where TO_FEATURE_OID =" + sOID + " and TO_FEATURE_FCID = " + FCID + " AND FEEDERID = '" + sFeederID + "' and MIN_BRANCH <= MAX_BRANCH and rownum = 1) " +
                    " order by a.order_num desc";
            }
            catch (Exception exp)
            {
                CommonFuntions.WriteLine_Error("Exception occurred." + exp.Message.ToString());
            }
            return sQuery;
        }

        internal string GetBusbarwithGetParentFeature(string sCircuitID)
        {
            string sReturnQuery = string.Empty;
            try
            {
                sReturnQuery = "select c.to_feature_oid as BusBar_OID, (select name from " + ReadConfigurations.GDBITEMSTableName + " WHERE objectid= d.to_feature_fcid ) as FeatureClass, d.to_feature_oid,d.to_feature_globalid from "
                    + " ( select a.to_feature_oid,b.from_feature_eid    from " + ReadConfigurations.FeederNetworkTraceTableName + " a join " + ReadConfigurations.FeederNetworkTraceTableName + " b on a.from_feature_eid= b.to_feature_eid"
                    + " where a.feederid='" + sCircuitID + "' and a.to_feature_fcid=1019 and b.to_feature_fcid in (19469) )c join " + ReadConfigurations.FeederNetworkTraceTableName + " d on c.from_feature_eid= d.to_feature_eid "
                    + " and d.to_feature_fcid in (1019,1021,1023)";
            }
            catch(Exception ex)
            {
                CommonFuntions.WriteLine_Error("Exception Occurred while creating the Busbar Query to get parent information ," + ex.Message + " at " + ex.StackTrace);
                sReturnQuery = string.Empty;
                throw ex;
            }
            return sReturnQuery;
        }



        internal string GetTraceTableDownStreamFeaturesForPhaseUpdate(string sCircuitID)
        {
            string sReturnQuery = string.Empty;
            try
            {
                sReturnQuery = "select a.ORDER_NUM,a.FROM_FEATURE_EID,a.TO_FEATURE_EID, a.TO_FEATURE_FCID,b.physicalname, a.TO_FEATURE_OID,a.TO_FEATURE_GLOBALID,a.MIN_BRANCH,a.MAX_BRANCH,a.TREELEVEL,a.FEEDERFEDBY,a.FEEDERID " +
                                             " from " + ReadConfigurations.FeederNetworkTraceTableName + " a left join " + ReadConfigurations.GDBITEMSTableName + " b" +
                                         " on a.to_feature_fcid = b.objectid where " +
                                         " (a.FEEDERID = '" + sCircuitID + "' OR a.FEEDERFEDBY = '" + sCircuitID + "')  " +
                                         " and a.MIN_BRANCH>=(select MIN_BRANCH from " + ReadConfigurations.FeederNetworkTraceTableName + " where FROM_FEATURE_EID = -1 AND FEEDERID = '" + sCircuitID + "' ) " +
                                         " AND a.MAX_BRANCH <= (select MAX_BRANCH from " + ReadConfigurations.FeederNetworkTraceTableName + " where FROM_FEATURE_EID = -1 AND FEEDERID = '" + sCircuitID + "' ) " +
                                         " AND a.TREELEVEL >= (select TREELEVEL from " + ReadConfigurations.FeederNetworkTraceTableName + " where FROM_FEATURE_EID = -1 AND FEEDERID = '" + sCircuitID + "' ) " +
                                         " AND a.ORDER_NUM <= (select ORDER_NUM from " + ReadConfigurations.FeederNetworkTraceTableName + " where FROM_FEATURE_EID = -1 AND FEEDERID = '" + sCircuitID + "') " +
                                         " order by a.order_num desc";
            }
            catch (Exception ex)
            {
                CommonFuntions.WriteLine_Error("Exception Occurred while creating the Busbar Query to get parent information ," + ex.Message + " at " + ex.StackTrace);
                sReturnQuery = string.Empty;
                throw ex;
            }
            return sReturnQuery;
        }


        internal string GetDevicesFromTraceTable(string sCircuitID,string sClassName)
        {
            string sReturnQuery = string.Empty;
            try
            {
                sReturnQuery = "select a.ORDER_NUM,a.FROM_FEATURE_EID,a.TO_FEATURE_EID, a.TO_FEATURE_FCID,b.physicalname, a.TO_FEATURE_OID,a.TO_FEATURE_GLOBALID,a.MIN_BRANCH,a.MAX_BRANCH,a.TREELEVEL,a.FEEDERFEDBY,a.FEEDERID " +
                                             " from " + ReadConfigurations.FeederNetworkTraceTableName + " a left join " + ReadConfigurations.GDBITEMSTableName + " b" +
                                         " on a.to_feature_fcid = b.objectid where " +
                                         " (a.FEEDERID = '" + sCircuitID + "' OR a.FEEDERFEDBY = '" + sCircuitID + "')  " +
                                         " and b.physicalname = '" + sClassName + "'" + 
                                         " and a.MIN_BRANCH>=(select MIN_BRANCH from " + ReadConfigurations.FeederNetworkTraceTableName + " where FROM_FEATURE_EID = -1 AND FEEDERID = '" + sCircuitID + "' ) " +
                                         " AND a.MAX_BRANCH <= (select MAX_BRANCH from " + ReadConfigurations.FeederNetworkTraceTableName + " where FROM_FEATURE_EID = -1 AND FEEDERID = '" + sCircuitID + "' ) " +
                                         " AND a.TREELEVEL >= (select TREELEVEL from " + ReadConfigurations.FeederNetworkTraceTableName + " where FROM_FEATURE_EID = -1 AND FEEDERID = '" + sCircuitID + "' ) " +
                                         " AND a.ORDER_NUM <= (select ORDER_NUM from " + ReadConfigurations.FeederNetworkTraceTableName + " where FROM_FEATURE_EID = -1 AND FEEDERID = '" + sCircuitID + "') " +
                                         " order by a.order_num desc";
            }
            catch (Exception ex)
            {
                CommonFuntions.WriteLine_Error("Exception Occurred while creating the Busbar Query to get parent information ," + ex.Message + " at " + ex.StackTrace);
                sReturnQuery = string.Empty;
                throw ex;
            }
            return sReturnQuery; 
        }
    }
}
