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
using Miner.Geodatabase.GeodatabaseManager.Logging;
using Miner.Geodatabase.GeodatabaseManager.Serialization;
using ADODB;
using ESRI.ArcGIS.DataSourcesOleDB;
using Oracle.DataAccess.Client;
using System.Data.OleDb;
using ESRI.ArcGIS.Geometry;
using Miner.Process.GeodatabaseManager;
using System.Collections.Generic;

namespace PGE.Desktop.EDER.GDBM
{
    public  class CommonFunctions
    {
        DBHelper clsdbhelper = new DBHelper();
        public static string gConnectionstring = string.Empty;
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
                //Common._log.Error("Error in getting SDE Workspace, function GetWorkspace: " + exp.Message);
                //Common._log.Info(exp.Message + "   " + exp.StackTrace);
                throw exp;
            }
            return workspace;
        }

        internal IRelationshipClass GetRelationshipClassbyFeatureClassAliasName(IObject pObject, string sClassName)
        {
            IRelationshipClass pSearchRelationshipClass = null;
            try
            {
                IEnumRelationshipClass enumrelClass = pObject.Class.get_RelationshipClasses(ESRI.ArcGIS.Geodatabase.esriRelRole.esriRelRoleAny);
                IRelationshipClass pRelationshipClass = (IRelationshipClass)enumrelClass.Next();

                while (pRelationshipClass != null)
                {

                    if ((pRelationshipClass.DestinationClass.AliasName == sClassName) || (pRelationshipClass.OriginClass.AliasName == sClassName))
                    {
                        pSearchRelationshipClass = pRelationshipClass;
                        break;
                    }
                    pRelationshipClass = (IRelationshipClass)enumrelClass.Next();
                }
            }
            catch
            {

            }
            return pSearchRelationshipClass;

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
                //_log.Error("Unhandled exception occurred while getting the Featureclass-  ," + sClassName + ", Exception Message :-," + ex.Message.ToString() + " at " + ex.StackTrace);
                throw ex;
            }
            return returnFeatureclass;
        }
        public  string geometryToString(IGeometry geometry, out int esriGeomType)
        {
            string retStr = string.Empty;
            esriGeomType = -1;
            try
            {
                if (geometry != null)
                {
                    esriGeomType = (int)geometry.GeometryType;
                    ESRI.ArcGIS.Geometry.IWkb wkb = geometry as ESRI.ArcGIS.Geometry.IWkb;
                    byte[] byt = new byte[wkb.WkbSize];
                    int size = wkb.WkbSize;
                    wkb.ExportToWkb(ref size, out byt[0]); retStr = string.Join(", ", byt);
                }
                
            }
            catch (Exception ex)
            {
               
                throw ex;
            }
            return retStr;
        }
        /// <summary>
        /// Usage for <to stop >  all action handler except QAQC
        /// </summary>
        /// <param name="pversion"></param>
        /// <param name="log"></param>
        /// <param name="serviceConfiguration"></param>
        /// <param name="sUsageString"></param>
        /// <returns></returns>
        internal bool CheckUsagewithUsageString(IVersion pversion, INameLog log, GdbmReconcilePostService serviceConfiguration, out string sUsageString)
        {
            bool brunAH = false;
            string strQuery = string.Empty;
            sUsageString = string.Empty;
            try
            {
                // CHeck data
                strQuery = "SELECT value from "  + SchemaInfo.General.pEDERCONFIGTABLE  + " where key='GDBMBYPASS_NOAH'";
                DataRow DR = clsdbhelper.GetSingleDataRowByQuery(strQuery);
                if (DR != null)
                {
                    string sValue = DR[0].ToString();
                    if (!string.IsNullOrEmpty(sValue))
                    {
                        string[] snames = sValue.Split(',');
                        string sSessionName = GetSessionNamefromVersionName(pversion);
                        if (!string.IsNullOrEmpty(sSessionName))
                        {
                            for (int i = 0; i < snames.Length; i++)
                            {

                                if (sSessionName.ToUpper().Contains(snames[i].ToUpper()))
                                {
                                    brunAH = true;
                                    sUsageString = snames[i];
                                    break;
                                }
                            }
                        }
                    }


                }
            }
            catch (Exception ex)
            {
            }
            return brunAH;
        }


        /// <summary>
        /// Modified for GDBM QAQC
        /// </summary>
        /// <param name="DR"></param>
        /// <param name="log"></param>
        /// <param name="serviceConfiguration"></param>
        /// <param name="sSessionName"></param>
        /// <param name="sUsageString"></param>
        /// <returns></returns>
        internal string CheckUsagewithUsageString(DataTable DT, INameLog log, GdbmReconcilePostService serviceConfiguration,string sSessionName)
        {
            bool brunAH = false;
            string strQuery = string.Empty;
            string sUsageString = string.Empty;
            try
            {
                // CHeck data
                if (DT == null) return string.Empty; 
                foreach (DataRow DR in DT.Rows)
                {
                    string sValue = DR[1].ToString();
                    if (!string.IsNullOrEmpty(sValue))
                    {
                        string[] snames = sValue.Split(',');
                        if (!string.IsNullOrEmpty(sSessionName))
                        {
                            for (int i = 0; i < snames.Length; i++)
                            {
                                if (sSessionName.ToUpper().Contains(snames[i].ToUpper()))
                                {
                                    brunAH = true;
                                    if (!string.IsNullOrEmpty(sUsageString))
                                    {
                                        sUsageString = sUsageString + "," + DR[0].ToString().Split('_')[1].ToString();
                                        
                                        break;
                                    }
                                    else
                                    {
                                        sUsageString = DR[0].ToString().Split('_')[1].ToString();
                                        break;
                                    }
                                }
                            }
                        }
                    }

                }




            }
            catch (Exception ex)
            {
            }
            return sUsageString;
        }

        internal bool CheckNetworkFeature(IMap pMap,out int count)
        {
            bool NetworkFeature = false;
            count = 0;
            try
            {
                IEnumFeature enumFeat = pMap.FeatureSelection as IEnumFeature;
                IEnumFeatureSetup enumSetup = (IEnumFeatureSetup)enumFeat;
                enumSetup.AllFields = true;
                enumFeat.Reset();
                IFeature Feat = null;
              

                while ((Feat = enumFeat.Next()) != null)
                {
                    try
                    {

                        INetworkFeature netFeat = Feat as INetworkFeature;
                        if (netFeat != null)
                        {
                            if ((Feat.Fields.FindField(SchemaInfo.Electric.Phasedesignation) != -1) || (Feat.Fields.FindField(SchemaInfo.Electric.CircuitID) != -1))
                            {
                                count = count + 1;
                            }
                        }
                    }
                    catch 
                    {
                    }
                }
                if (count > 0)
                {
                    NetworkFeature = true;
                }

            }
            catch
            {
            }
            return NetworkFeature;
        }

        internal bool CheckUsage(IVersion pversion, INameLog log, GdbmReconcilePostService serviceConfiguration)
        {
            bool brunAH = false;
            string strQuery = string.Empty;
            
            try
            {
                // CHeck data
                strQuery = "SELECT value from " + SchemaInfo.General.pEDERCONFIGTABLE + " where key='GDBMBYPASS_NOAH'";
                DataRow DR = clsdbhelper.GetSingleDataRowByQuery(strQuery);
                if (DR != null)
                {
                    string sValue = DR[0].ToString();
                    if (!string.IsNullOrEmpty(sValue))
                    {
                        string[] snames = sValue.Split(',');
                        string sSessionName = GetSessionNamefromVersionName(pversion);
                        if (!string.IsNullOrEmpty(sSessionName))
                        {
                            for (int i = 0; i < snames.Length; i++)
                            {

                                if (sSessionName.ToUpper().Contains(snames[i].ToUpper()))
                                {
                                    brunAH = true;
                                   
                                    break;
                                }
                            }
                        }
                    }


                }
               
            }
            catch (Exception ex)
            {
            }
            return brunAH;
        }


        public ADODB.Recordset GetRecordset(string strQuery, IWorkspace pworkspace)
        {
            Recordset pRSet = new Recordset();
            ADODB.Connection _adoConnection = new ADODB.ConnectionClass();
            try
            {
                pRSet.CursorLocation = CursorLocationEnum.adUseClient;
                IFDOToADOConnection _fdoToadoConnection = new FdoAdoConnectionClass();

                _adoConnection = _fdoToadoConnection.CreateADOConnection(pworkspace as IWorkspace) as ADODB.Connection;

                pRSet.Open(strQuery, _adoConnection, ADODB.CursorTypeEnum.adOpenKeyset , ADODB.LockTypeEnum.adLockBatchOptimistic, 0);

            }
            catch(Exception ex)
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

        public string GetSessionNamefromVersionName(IVersion pversion)
        {
            string sValue = string.Empty;
            try
            {
                string[] psubversioname = pversion.VersionName.Split('.');

                string sversionname = string.Empty;
                if (psubversioname.Length > 1)
                {
                    sversionname = psubversioname[1];
                }
                else if(psubversioname.Length == 1)
                {
                    sversionname = psubversioname[0];
                }
                string strQuery = "select SESSION_NAME from PROCESS.MM_SESSION where SESSION_ID='" + sversionname.Split('_')[1] + "'";
                DataRow DR = clsdbhelper.GetSingleDataRowByQuery(strQuery);
               
                if (!string.IsNullOrEmpty(DR[0].ToString()))
                {
                     sValue = DR[0].ToString();
                   

                }
                
               }
            catch { }
            return sValue;
        }
       

        internal void ExecuteQuery(string updateQuery,string strConnectionsstring)
        {
            try
            {
               OracleConnection  connection = new OracleConnection(strConnectionsstring);
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                    using (var cmd = new OracleCommand(updateQuery, connection))
                    {
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Insert data  where Action ='I' or 'U' or 'D'
        /// </summary>
        /// <param name="pObject"></param>
        /// <param name="sVersionName"></param>
        /// <param name="SsessionName"></param>
        /// <param name="sAction"></param>
        /// <param name="sUsageName"></param>
        internal void UpdateBulkData(ref IDictionary<string, IList<int>> Stageddata,long objectid,IObject pObject, string pOID, string sFeatClassName,string sVersionName, 
            string SsessionName, string sAction, string sUsageName, ref DataTable dtStaging,string sservicename,INameLog _nlog)
        {
            long lOID = Convert.ToInt32(pOID);
            int iGlobalidIndex = -1;
            try
            {
                string sReplaceGUID = string.Empty;
                DataRow DR = dtStaging.NewRow();
                #region Check Duplicate Records and set the Status Based on that
                if ((Stageddata.ContainsKey(sFeatClassName.ToUpper())) && (Stageddata[sFeatClassName.ToUpper()].Contains(Convert.ToInt32(pOID))))
                {
                    
                        DR["STATUS"] = "X";

                }
                else
                {
                    if (!Stageddata.ContainsKey(sFeatClassName.ToUpper()))
                    {
                        Stageddata.Add(sFeatClassName.ToUpper(), new List<int>());
                    }

                    Stageddata[sFeatClassName.ToUpper()].Add(Convert.ToInt32(pOID));
                    DR["STATUS"] = "N";
                   
                }
                #endregion
                if ((sAction == "U") || (sAction == "I"))
                {
                    if (sAction == "I")
                    {
                        int ireplaceGUIDIndex = pObject.Fields.FindField("REPLACEGUID");
                        if (ireplaceGUIDIndex != -1)
                        {
                            DR["FEAT_REPLACEGUID_OLD"] = pObject.get_Value(ireplaceGUIDIndex).ToString();
                        }
                    }
                    iGlobalidIndex = pObject.Fields.FindField("GLOBALID");
                    if (iGlobalidIndex == -1)
                    {
                        iGlobalidIndex = pObject.Fields.FindField("GUID");
                    }
                    if (iGlobalidIndex == -1)
                    {
                        iGlobalidIndex = pObject.Fields.FindField("GLOBALID_1");
                    }
                    if (iGlobalidIndex != -1)
                    {
                        DR["FEAT_GLOBALID"] = pObject.get_Value(iGlobalidIndex).ToString();
                        //  DRTotal["FEAT_GLOBALID"] = pObject.get_Value(iGlobalidIndex).ToString();
                    }
                    else
                    {
                        DR["FEAT_GLOBALID"] = string.Empty;
                        //DRTotal["FEAT_GLOBALID"] = string.Empty;
                    }
                }
                DR["OBJECTID"] = objectid;
                DR["FEAT_OID"] = Convert.ToInt32(pOID.ToString());
                DR["FEAT_CLASSNAME"] = sFeatClassName.ToString().ToUpper();
                DR["SESSIONNAME"] = SsessionName;
                   
                DR["ACTION"] = sAction;
                DR["VERSIONNAME"] = sVersionName;
                DR["USAGE"] = sUsageName;

                dtStaging.Rows.Add(DR);
                

            }
            catch (Exception ex)
            {
                _nlog.Debug(sservicename, "Exception occurred while inserting record in Datatable  with OBJECTID -- " + pOID );


                throw ex;
            }
            finally
            {
                if(dtStaging.Rows.Count>5000)
                {
                   
                    clsdbhelper.BulkCopyDataFromDataTable(dtStaging, SchemaInfo.General.pAHInfoTableName );
                    _nlog.Debug(sservicename, "Records inserted in " + SchemaInfo.General.pAHInfoTableName + " - " + dtStaging.Rows.Count.ToString());
                    dtStaging =CreateDTStaging();

                }
            }

        }

        private bool CheckDuplicateData(ref Dictionary<long, string> stageddata, long v, string sFeatClassName)
        {
            throw new NotImplementedException();
        }

        public DataTable CreateDTStaging()
        {
            DataTable pDT = new DataTable();
            try
            {
                DataColumn DC = new DataColumn("FEAT_GLOBALID");
                pDT.Columns.Add(DC);
                DC = new DataColumn("FEAT_CLASSNAME");
                pDT.Columns.Add(DC);
                DC = new DataColumn("SESSIONNAME");
                pDT.Columns.Add(DC);
                DC = new DataColumn("FEAT_CIRCUITID_OLD");
                pDT.Columns.Add(DC);
                DC = new DataColumn("STATUS");
                pDT.Columns.Add(DC);
                DC = new DataColumn("OBJECTID");
                DC.DataType = typeof(Int32);
                pDT.Columns.Add(DC);

                DC = new DataColumn("ACTION");
                pDT.Columns.Add(DC);
                DC = new DataColumn("FEAT_REPLACEGUID_OLD");
                pDT.Columns.Add(DC);
                DC = new DataColumn("FEAT_OID");
                DC.DataType = typeof(Int32);
                pDT.Columns.Add(DC);

                DC = new DataColumn("VERSIONNAME");
                pDT.Columns.Add(DC);
                DC = new DataColumn("USAGE");
                pDT.Columns.Add(DC);
                DC = new DataColumn("FEAT_SAPEQIPID_OLD");
                pDT.Columns.Add(DC);
                DC = new DataColumn("FEAT_OPERATINGNO_OLD");
                pDT.Columns.Add(DC);

                DC = new DataColumn("CAPTURE_DATE");
                pDT.Columns.Add(DC);


                DC = new DataColumn("FEAT_FIELDS_LIST");
                pDT.Columns.Add(DC);
                DC = new DataColumn("FEAT_SHAPE_OLD");
                pDT.Columns.Add(DC);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return pDT;
        }

        /// <summary>
        /// checkDuplicate
        /// </summary>
        /// <param name="sOID"></param>
        /// <param name="DT"></param>
        /// <param name="sfield"></param>
        /// <returns></returns>
        private bool ContainDataRowInDataTable(string sOID, DataTable DT, string sfield)
        {
            bool _retval = true;
            try
            {
                string valueToSearch = sOID;
                string columnName = sfield;
                int count = (int)DT.Compute(string.Format("count({0})", columnName),
                        string.Format("{0} like '{1}'", columnName, valueToSearch));
                if (count == 0)
                {
                    _retval = false;
                }
            }
            catch (Exception ex)
            {
               // _lo(ex.Message + " at " + ex.StackTrace);
            }

            return _retval;
        }

        /// <summary>
        /// Insert data  where Action ='I' or 'U'
        /// </summary>
        /// <param name="pObject"></param>
        /// <param name="sVersionName"></param>
        /// <param name="SsessionName"></param>
        /// <param name="sAction"></param>
        /// <param name="sUsageName"></param>
        internal void UpdateData(IObject pObject, string sVersionName, string SsessionName, string sAction, string sUsageName)
        {
            int iGlobalidIndex = -1;
            try
            {
                iGlobalidIndex = pObject.Fields.FindField("GLOBALID");
                ////try
                ////{
                ////    if (CheckDuplicateData(pObject.OID, sVersionName, ((IDataset)pObject.Class).BrowseName)) return;
                ////}
                //catch { }
                #region Get max ObjectID
                long objID = -1;
                string sQuery = "select max(OBJECTID)+1 OBJECTID from " + SchemaInfo.General.pAHInfoTableName ;
                DataRow DR = clsdbhelper.GetSingleDataRowByQuery(sQuery);

                if (!string.IsNullOrEmpty(DR[0].ToString()))
                {
                    try { objID = Convert.ToInt32(DR[0].ToString()); } catch { }

                }
                #endregion

                //string UpdateQuery = "INSERT INTO "  + SchemaInfo.General.pAHInfoTableName  +
                //            " (OBJECTID,FEAT_GLOBALID,FEAT_OID, FEAT_CLASSNAME,VERSIONNAME, SESSIONNAME, STATUS, ACTION,USAGE)" +
                //            " VALUES(:OBJECTID,:GLOBALID, :OLD_OID, :featureclassname, :versionname, :sessionname, :status, :saction, :susername)";
                string UpdateQuery = string.Empty;

                if (iGlobalidIndex == -1)
                {
                    UpdateQuery = "INSERT INTO " + SchemaInfo.General.pAHInfoTableName + 
                           " (OBJECTID,FEAT_OID, FEAT_CLASSNAME,VERSIONNAME, SESSIONNAME, STATUS, ACTION,USAGE)" +
                           " VALUES(" + objID + "," + pObject.OID + ",'" +
                             ((IDataset)pObject.Class).BrowseName.ToString().ToUpper() + "','" + sVersionName + "','" + SsessionName + "','N','" + sAction + "','" + sUsageName + "')";


                    //clsdbhelper.executeparmeterQuery(UpdateQuery, string.Empty, pObject.OID, ((IDataset)pObject.Class).BrowseName,
                    //    sVersionName, SsessionName, sAction, sUsageName, objID);
                }
                else
                {
                    UpdateQuery = "INSERT INTO " + SchemaInfo.General.pAHInfoTableName + 
                           " (OBJECTID,FEAT_GLOBALID,FEAT_OID, FEAT_CLASSNAME,VERSIONNAME, SESSIONNAME, STATUS, ACTION,USAGE)" +
                           " VALUES(" + objID + ",'" + pObject.get_Value(iGlobalidIndex).ToString() + "'," + pObject.OID + ",'" +
                             ((IDataset)pObject.Class).BrowseName.ToString().ToUpper() + "','" + sVersionName + "','" + SsessionName + "','N','" + sAction + "','" + sUsageName + "')";


                    //clsdbhelper.executeparmeterQuery(UpdateQuery, pObject.get_Value(iGlobalidIndex).ToString(), pObject.OID, ((IDataset)pObject.Class).BrowseName,
                    //    sVersionName, SsessionName, sAction, sUsageName, objID);
                }
                clsdbhelper.UpdateQuery(UpdateQuery);
            }
            catch(Exception ex) 
            {
                throw ex;
            }
        }

        //internal void UpdateDeletedData(int pOID, string sFeatClassName, string sVersionName, string SsessionName, string sAction, string sUsageName)
        //{
        //    try
        //    {

        //        if (CheckDuplicateData(pOID,sVersionName )) return;

        //        string UpdateQuery = " INSERT INTO "  + SchemaInfo.General.pAHInfoTableName  +
        //                    " (FEAT_GLOBALID,FEAT_OID, FEAT_CLASSNAME,VERSIONNAME, SESSIONNAME, STATUS, ACTION,USAGE)" +
        //                    " VALUES(:GLOBALID, :OLD_OID, :featureclassname, :versionname, :sessionname, :status, :saction, :susername)";

        //        clsdbhelper.executeparmeterQuery(UpdateQuery, string.Empty, pOID, sFeatClassName,
        //               sVersionName, SsessionName, sAction, sUsageName);

        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        /// <summary>
        /// Check Duplicate data
        /// </summary>
        /// <param name="oID"></param>
        /// <returns></returns>
        private bool CheckDuplicateData(int oID,string versionname,string sFeatClassName)
        {
            bool recordexist = false;
            try
            {
                string checkDataexistQuery = "Select count(*) from " + SchemaInfo.General.pAHInfoTableName + " where feat_OID=" + oID +
                    " and  VERSIONNAME='" + versionname + "' and FEAT_CLASSNAME='" + sFeatClassName.ToUpper() + "'";
                DataRow DRCheck = clsdbhelper.GetSingleDataRowByQuery(checkDataexistQuery);
                if (DRCheck != null)
                {
                    int iValue = Convert.ToInt32(DRCheck[0].ToString());
                    if (iValue > 0)
                    {
                        recordexist = true;
                    }

                }
            }
            catch {   }
            return recordexist;
        }

    }
}
