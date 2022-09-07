using System;
using System.Collections.Generic;
using ESRI.ArcGIS.Geodatabase;
using Miner.Interop;
using System.Runtime.InteropServices;
using System.Collections;
using System.Xml.Serialization;
using System.IO;
using System.Data;
using ESRI.ArcGIS.Geometry;
using ADODB;
using ESRI.ArcGIS.Carto;

namespace PGE.BatchApplication.LoadDifference
{
    public class LoadDifference
    {
        public static string oraclestr = string.Empty;
        DBHelper clsdbhelper = new DBHelper();
        Common clscommon = new Common();

        public bool LoadDifferenceData(IVersion pOldVersion, IFeatureWorkspace EDERDefaultVersion,string sDBType)
        {
            bool _success = false;
            try
            {
                oraclestr = ReadConfigurations.OracleConnString;
                DataTable ObjectIDList = (new DBHelper()).GetDataTable(ReadConfigurations.UpdateQuery);
                if (ObjectIDList.Rows.Count > 0)
                {
                    clscommon.WriteLine_Info("Update Staging Data Started for Update/Delete");
                    UpdateStagingDataForUpdateAndDelete(ObjectIDList, pOldVersion as IWorkspace,EDERDefaultVersion,sDBType );
                    clscommon.WriteLine_Info("Update Staging Data Completed for Update/Delete");
                }
                else { clscommon.WriteLine_Info("No Update Case Present."); }
                ObjectIDList = (new DBHelper()).GetDataTable(ReadConfigurations.InsertQuery);
                    
                
                if (ObjectIDList.Rows.Count > 0)
                {
                    clscommon.WriteLine_Info("Update Staging Data Started for Insert");
                    UpdateStagingDataForInsert(ObjectIDList, EDERDefaultVersion as IWorkspace);
                    clscommon.WriteLine_Info("Update Staging Data Completed for Insert");
                }
                else { clscommon.WriteLine_Info("No Insert Case Present."); }
                _success = true;
            }
            catch (Exception ex)
            {
               
                throw ex;
            }
            finally
            {
            }

            return _success;
        }
        private void UpdateStagingDataForUpdateAndDelete(DataTable ObjectIDList, IWorkspace pCurrentWorkspace, IFeatureWorkspace EDERDefaultVersion,string sDBtype)
        {
            try
            {

                #region //JeeraId-382-YXA6 Update Staging Data for Change Detectection API
                if (ReadConfigurations.FORRecovery == "TRUE")
                {
                    pCurrentWorkspace = (new GeoDBHelper()).GetWorkspace(ReadConfigurations.TempConnectionString);
                }
                IWorkspace EDGMCWorkspace = (new GeoDBHelper()).GetWorkspace(ReadConfigurations.EDGMCConnectionString);
                DataView view = new DataView(ObjectIDList);
                DataTable VersionNamedistinctValues = view.ToTable(true, "VERSIONNAME");
                //Logged all the version edited through GDBM
                foreach (DataRow Versionrow in VersionNamedistinctValues.Rows)
                {
                    clscommon.WriteLine_Info("Process Run For VersionName- " + Versionrow["VERSIONNAME"].ToString());
                }

                Hashtable VersionNameList = new Hashtable();
                string soldGlobalID = string.Empty, strQuery = string.Empty,
                scircuitid_old = string.Empty,
                sreplaceGUID_old = string.Empty, 
                OPTNUMBER_old = string.Empty,
                SAP_EQUIPID_old = string.Empty;
                System.IO.StringWriter writer = new System.IO.StringWriter();
                IObject OldFeatureForShape = null;
                //DeleteOldExistingData(ReadConfigurations.STATUS.sNEW);
               
                int count =0;
                string UpdateQuery = string.Empty;
                foreach(DataRow dr in ObjectIDList.Rows)
                {
                    count = count + 1;
                   
                    Console.WriteLine(count + "  record Processed , out of total no of records- " + ObjectIDList.Rows.Count);
                    try
                    {
                        scircuitid_old = string.Empty;
                        sreplaceGUID_old = string.Empty;
                        OPTNUMBER_old = string.Empty;
                        SAP_EQUIPID_old = string.Empty;
                        writer = new System.IO.StringWriter();
                        OldFeatureForShape = null;
                        DataTable dt = new DataTable();
                        string strShape = string.Empty;
                        int geotype = -1;
                        soldGlobalID = string.Empty;
                        #region Update old feature information in staging table
                        OldFeatureForShape = null;
                        soldGlobalID = string.Empty;
                        // Find Old Feature Shape
                        if (Program.ClassNotExist.Contains(dr[1].ToString().ToUpper())) continue;
                        OldFeatureForShape = FindOldFeatureShape(pCurrentWorkspace, EDERDefaultVersion, EDGMCWorkspace, dr, sDBtype);
                        if (OldFeatureForShape != null)
                        {
                            if (OldFeatureForShape is IFeature)
                            {
                                strShape = geometryToString((OldFeatureForShape as IFeature).ShapeCopy as IGeometry, out geotype);
                                if (!string.IsNullOrEmpty(strShape))
                                {
                                    strShape = strShape + "#" + geotype;
                                }
                            }
                            try { scircuitid_old = OldFeatureForShape.get_Value(OldFeatureForShape.Fields.FindField("CIRCUITID")).ToString(); } catch { }
                            try { sreplaceGUID_old = OldFeatureForShape.get_Value(OldFeatureForShape.Fields.FindField("REPLACEGUID")).ToString(); } catch { }
                            try { OPTNUMBER_old = OldFeatureForShape.get_Value(OldFeatureForShape.Fields.FindField("OPERATINGVOLTAGE")).ToString(); } catch { }
                            try { SAP_EQUIPID_old = OldFeatureForShape.get_Value(OldFeatureForShape.Fields.FindField("SAPEQUIPID")).ToString(); } catch { }
                            soldGlobalID = GetGlobalID(OldFeatureForShape);

                            //Find Old feature detailed XML
                            dt = ConvertFieldsIntoDatatable(OldFeatureForShape as IObject);
                            if (dt != null)
                            {
                                dt.TableName = "XML_FIELDS";
                                checkdt(dt);
                                dt.WriteXml(writer, true);
                            }

                            UpdateQuery = " Update " + ReadConfigurations.pAHInfoTableName +
                                "  set FEAT_REPLACEGUID_OLD= :FEAT_REPLACEGUID_OLD,FEAT_GLOBALID=:FEAT_GLOBALID, STATUS=:STATUS,FEAT_OPERATINGNO_OLD=:FEAT_OPERATINGNO_OLD, FEAT_SAPEQIPID_OLD=:FEAT_SAPEQIPID_OLD," +
                        "  FEAT_CIRCUITID_OLD=:FEAT_CIRCUITID_OLD, FEAT_FIELDS_LIST=:FEAT_FIELDS_LIST, FEAT_SHAPE_OLD=:FEAT_SHAPE_OLD " +
                        " where feat_oid= :Feat_OID and objectid=:OBJECTID";


                            clsdbhelper.executeparmeterQuery(UpdateQuery, sreplaceGUID_old, soldGlobalID, OPTNUMBER_old, SAP_EQUIPID_old,
                            scircuitid_old, writer.ToString(), strShape, Convert.ToInt32(dr[0].ToString()), Convert.ToInt32(dr[3].ToString()));

                        }


                        #endregion


                    }
                    catch (Exception ex)
                    {
                        clscommon.WriteLine_Error("Exception occurred while updating the feature - OID-" + dr[0].ToString() + "---Exception= " + ex.Message.ToString());
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
               
                throw ex;
            }
        }

        private IObject FindOldFeatureShape(IWorkspace pCurrentWorkspace, IFeatureWorkspace eDERDefaultVersion, IWorkspace EDGMCWorkspace, DataRow dr ,string sDBtype)
        {
            IObject Oldfeature = null;
            try
            {
                IObjectClass objClass = null;
                bool FeatureExist = false;
                Oldfeature = GetFeatureFROMBackUpVersion(dr[1].ToString(), pCurrentWorkspace as IFeatureWorkspace, Convert.ToInt32(dr[0].ToString()), out FeatureExist, out objClass);
                if (objClass == null) return null; 
                if (Oldfeature == null)
                {

                    if (sDBtype == "EDER")
                    {
                        //Get Feature info from EDGMC database
                        Oldfeature = GetFeatureFROMEDGMC(dr[1].ToString(), EDGMCWorkspace as IFeatureWorkspace, Convert.ToInt32(dr[0].ToString()), out FeatureExist);
                    }
                    if (Oldfeature == null)
                    {

                        #region Check Same day Insert Update
                        if (Program.InsertTable.Rows.Count > 0)
                        {
                            DataRow[] CheckDuplicateRow = Program.InsertTable.Select("feat_oid= " + Convert.ToInt32(dr[0].ToString()) + " and feat_classname = '" + dr[1].ToString() + "'");
                            // OldFeatureForShape = GetFeature(dr[1].ToString(), EDERDefaultVersion as IFeatureWorkspace, Convert.ToInt32(dr[0].ToString()), out FeatureExist);
                            if (CheckDuplicateRow.Length < 1)
                            {
                                if (ReadConfigurations.FINDFEATUREInINTDATAARACH == "Y")
                                {
                                    //Get Feature from Backupversion for delete case only
                                    if (GetFeatureFromIntDataArch(Convert.ToInt32(dr[0].ToString()), dr[1].ToString(), dr[2].ToString()))
                                    {
                                        clscommon.WriteLine_Info("Feature found from IntDataRearch  - " + dr[0].ToString() + " and class name-- " + dr[1].ToString());
                                    }
                                    else
                                    {
                                        clscommon.WriteLine_Info("Feature Not Found in Same Day Insert Update Data  - " + dr[0].ToString() + " and class name-- " + dr[1].ToString());

                                    }
                                }
                                else
                                {
                                    clscommon.WriteLine_Info("Feature Not Found in Same Day Insert Update Data  - " + dr[0].ToString() + " and class name-- " + dr[1].ToString());

                                }
                                return null;
                            }
                            else
                            {
                                if (dr[2].ToString() == "U")
                                {
                                    clsdbhelper.UpdateQuery("Update " + ReadConfigurations.pAHInfoTableName + " set status = 'S' where  feat_oid = " + Convert.ToInt32(dr[0].ToString()) + " and feat_classname = '" + dr[1].ToString() + "'" +
                                        " and ACTION='U' ");
                                    return null;
                                }
                                else if (dr[2].ToString() == "D")
                                {
                                    clsdbhelper.UpdateQuery("Update " + ReadConfigurations.pAHInfoTableName + " set status = 'S' where  feat_oid = " + Convert.ToInt32(dr[0].ToString()) + " and feat_classname = '" + dr[1].ToString() + "'" +
                                        " and ACTION='D'");
                                    return null;
                                }
                                clscommon.WriteLine_Info("Feature  found as its same day insert/update/delete - " + dr[0].ToString() + " and class name-- " + dr[1].ToString());
                            }

                        }
                        else
                        {
                            Oldfeature = GetFeatureFROMDefaultVersion(dr[1].ToString(), eDERDefaultVersion as IFeatureWorkspace, Convert.ToInt32(dr[0].ToString()), out FeatureExist);
                            if (Oldfeature == null)
                            {
                                if (ReadConfigurations.FINDFEATUREInINTDATAARACH == "Y")
                                {
                                    //Get Feature from Backupversion for delete case only
                                    if (GetFeatureFromIntDataArch(Convert.ToInt32(dr[0].ToString()), dr[1].ToString(), dr[2].ToString()))
                                    {
                                        clscommon.WriteLine_Info("Feature found from IntDataRearch  - " + dr[0].ToString() + " and class name-- " + dr[1].ToString());
                                    }
                                    else
                                    {
                                        clscommon.WriteLine_Info("Feature not found neither in backup nor in geomart or same day insert/update/delete version or IntDataArchSchema- " + dr[0].ToString() + " and class name-- " + dr[1].ToString());

                                    }

                                    return null;
                                }
                                else
                                {
                                    clscommon.WriteLine_Info("Feature not found neither in backup nor in geomart or same day insert/update/delete version " + dr[0].ToString() + " and class name-- " + dr[1].ToString());

                                }


                            }
                        }
                        #endregion
                    }
                    else
                    {
                        clscommon.WriteLine_Info("Feature  found in EDGMC Database - " + dr[0].ToString() + " and class name-- " + dr[1].ToString());
                    }

                }


            }
            catch (Exception ex)
            {
                clscommon.WriteLine_Error("Exception occurred while getting the old feature information -- OID-> " + dr[0].ToString() + "  and FeatureClass--" + dr[1].ToString() + "Exception --" + ex.Message.ToString());
            }
            return Oldfeature;
        }

        private bool GetFeatureFromIntDataArch(int fOID, string pClassName, string sAction)
        {
            bool getdata = false;
            try 
            {
                string strfindQuery = "Select FEAT_FIELDS_LIST,FEAT_SHAPE_OLD,FEAT_REPLACEGUID_OLD,feat_globalid,FEAT_OPERATINGNO_OLD,FEAT_SAPEQIPID_OLD,FEAT_CIRCUITID_OLD,OBJECTID " +
                    " from INTDATAARCH.PGE_GDBM_AH_INFO where  (feat_oid = " + fOID 
                    + ") and (feat_classname = '" + pClassName + "') and (FEAT_FIELDS_LIST is not null) and (status <>'X') order by CAPTURE_DATE desc  ";
                DataRow dr_Query = clsdbhelper.GetfromGMC(strfindQuery);
                if (dr_Query != null)
                {
                    string UpdateQuery = " Update " + ReadConfigurations.pAHInfoTableName +
         " set FEAT_REPLACEGUID_OLD= '" + Convert.ToString(dr_Query[2]) + "' ,FEAT_GLOBALID='" + Convert.ToString(dr_Query[3]) + "', STATUS='C'," +
         "FEAT_OPERATINGNO_OLD='" + Convert.ToString(dr_Query[4]) + "'," +
         " FEAT_SAPEQIPID_OLD='" + Convert.ToString(dr_Query[5]) + "', FEAT_CIRCUITID_OLD='" + Convert.ToString(dr_Query[6]) + "', " +
         "FEAT_FIELDS_LIST='" + Convert.ToString(dr_Query[0]) + "'," +
         " FEAT_SHAPE_OLD='" + Convert.ToString(dr_Query[1]) + "' where OBJECTID = " + Convert.ToInt32(dr_Query[7].ToString()) + " and feat_oid= " + fOID + "  and feat_classname = '" + pClassName + "'  and ACTION in ('D','U')" ;

                    if (clsdbhelper.UpdateQuery(UpdateQuery) > -1) { getdata = true; }
                }
            }
            catch (Exception ex){ }

            return getdata;
        }

        private string GetGlobalID(IObject oldFeatureForShape)
        {
            string sreturnGlobalID = string.Empty;
            try
            {
                try { sreturnGlobalID = oldFeatureForShape.get_Value(oldFeatureForShape.Fields.FindField("GLOBALID")).ToString(); } catch { }

                try
                {
                    if (string.IsNullOrEmpty(sreturnGlobalID))
                    {
                        sreturnGlobalID = oldFeatureForShape.get_Value(oldFeatureForShape.Fields.FindField("GUID")).ToString();
                    }
                }
                catch { }
                try
                {
                    if (string.IsNullOrEmpty(sreturnGlobalID))
                    {
                        sreturnGlobalID = oldFeatureForShape.get_Value(oldFeatureForShape.Fields.FindField("GLOBALID_1")).ToString();
                    }
                }
                catch { }

            }
            catch 
            {
            }
            return sreturnGlobalID;
        }

        private void UpdateStagingDataForInsert(DataTable ObjectIDList, IWorkspace pCurrentWorkspace)
        {
            try
            {

                #region //JeeraId-382-YXA6 Update Staging Data for Change Detectection API
                
                IObject pFeature= null;
                //DeleteOldExistingData(ReadConfigurations.STATUS.sNEW);
                if (ReadConfigurations.FORRecovery == "TRUE")
                {
                    pCurrentWorkspace = (new GeoDBHelper()).GetWorkspace(ReadConfigurations.TempConnectionString);
                }
               
                int count = 0;
                string UpdateQuery = string.Empty;
                foreach (DataRow dr in ObjectIDList.Rows)
                {
                    count = count + 1;

                    Console.WriteLine(count + "  record Processed , out of total no of records- " + ObjectIDList.Rows.Count);
                    try
                        {
                        #region Update old feature information in staging table
                        pFeature = null;
                        bool FeatureExist = false;
                        string soldGlobalID = string.Empty;
                        pFeature = GetFeatureFROMDefaultVersion(dr[1].ToString(), pCurrentWorkspace as IFeatureWorkspace, Convert.ToInt32(dr[0].ToString()), out FeatureExist);
                        if (pFeature == null)
                        {
                            clscommon.WriteLine_Info("Feature not found in Default Version For INSERT Case- " + dr[0].ToString() + " and class name-- " + dr[1].ToString());
                            //clsdbhelper.UpdateQuery("Update " + ReadConfigurations.pAHInfoTableName + " set status='E' where feat_oid= " + dr[0].ToString() + " and OBJECTID= " + dr[3].ToString());

                            continue;
                        }
                        try { soldGlobalID = pFeature.get_Value(pFeature.Fields.FindField("GLOBALID")).ToString(); } catch { }

                        try
                        {
                            if (string.IsNullOrEmpty(soldGlobalID))
                            {
                                if (pFeature.Fields.FindField("GUID") != -1)
                                {
                                    soldGlobalID = pFeature.get_Value(pFeature.Fields.FindField("GUID")).ToString();
                                }
                                else if (pFeature.Fields.FindField("GENERATIONGUID") != -1)
                                {
                                    soldGlobalID = pFeature.get_Value(pFeature.Fields.FindField("GENERATIONGUID")).ToString();
                                }
                            }

                        }
                        catch { }

                         
                            UpdateQuery = " Update " + ReadConfigurations.pAHInfoTableName +
                            "  set FEAT_REPLACEGUID_OLD= :FEAT_REPLACEGUID_OLD,FEAT_GLOBALID=:FEAT_GLOBALID, STATUS=:STATUS,FEAT_OPERATINGNO_OLD=:FEAT_OPERATINGNO_OLD, FEAT_SAPEQIPID_OLD=:FEAT_SAPEQIPID_OLD," +
                        "  FEAT_CIRCUITID_OLD=:FEAT_CIRCUITID_OLD, FEAT_FIELDS_LIST=:FEAT_FIELDS_LIST, FEAT_SHAPE_OLD=:FEAT_SHAPE_OLD " +
                        " where feat_oid= :Feat_OID and objectid=:OBJECTID";


                            clsdbhelper.executeparmeterQuery(UpdateQuery, string.Empty, soldGlobalID, string.Empty,string.Empty,
                            string.Empty, string.Empty, string.Empty, Convert.ToInt32(dr[0].ToString()), Convert.ToInt32(dr[3].ToString()));






                        #endregion


                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private DataTable ConvertFieldsIntoDatatable(IObject oldFeatureForShape)
        {
            DataTable dt = new DataTable();
            try
            {
                for (int i = 0; i < oldFeatureForShape.Fields.FieldCount; i++)
                {
                    dt.Columns.Add(oldFeatureForShape.Fields.Field[i].Name);
                }

                DataRow dr = dt.Rows.Add();
                for (int i = 0; i < oldFeatureForShape.Fields.FieldCount; i++)
                {
                    dr[i]= oldFeatureForShape.get_Value(i).ToString();
                }
                try
                {
                    dt.Rows.Add(dr);

                }
                catch { }
            }
            catch (Exception ex)
            {
                dt = null;
            }
            return dt;
        }

        public string geometryToString(IGeometry geometry, out int esriGeomType)
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

       
        private void checkdt(DataTable dt)
        {
            try 
            {
                foreach (DataRow dr in dt.Rows)
                {
                    for (int i = 0; i < dr.ItemArray.Length; i++)
                    {
                        string svalue = dr[i].ToString();
                        if (svalue.Contains("'"))
                        {

                            svalue =svalue.Replace("'","\\");
                            dr[i] = svalue;
                            dr.AcceptChanges();
                        }
                        else if (svalue.Contains("&"))
                        {

                            svalue = svalue.Replace("&", "\\");
                            dr[i] = svalue;
                            dr.AcceptChanges();
                        }
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// Getting Feature
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="pFWSpace"></param>
        /// <returns></returns>
        public IObject GetFeatureFROMBackUpVersion(string FeatureClassName, IFeatureWorkspace pFWSpace, int lOID,out bool bFeature, out IObjectClass pFeatureclass)
        {
            pFeatureclass = null;
            IObject ReturnFeature = null;
            bFeature = false;
            try
            {
                
                //trying to find the featureclass from the list 
                Program.Featureclasslist.TryGetValue(FeatureClassName, out pFeatureclass);
                //If not then get the featureclass from workspace
                if (pFeatureclass is null)
                {
                    pFeatureclass = GetObjectclass(FeatureClassName,pFWSpace);

                    //Add featureclass in the list so that next time no need to open from workspace
                    if (pFeatureclass != null)
                    {
                        if (!Program.Featureclasslist.ContainsKey(FeatureClassName))
                        {
                            Program.Featureclasslist.Add(FeatureClassName, pFeatureclass);
                        }
                    }
                }
                //Get the feature 
                if (pFeatureclass != null)
                {
                    if (pFeatureclass is IFeatureClass)
                    {
                        try
                        {
                            ReturnFeature = (pFeatureclass as IFeatureClass).GetFeature(lOID);

                           
                        }
                        catch { }

                    }
                    else if (pFeatureclass is ITable)
                    {
                        ITable ptable = pFeatureclass as ITable;
                        ReturnFeature = ptable.GetRow(lOID) as IObject;

                    }
                    else { }
                    
                }

            }
            catch (Exception ex)
            {

                
            }
            return ReturnFeature;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="featureClassName"></param>
        /// <param name="pFWSpace"></param>
        /// <returns></returns>
        public IObject GetFeatureFROMEDGMC(string FeatureClassName, IFeatureWorkspace pFWSpace, int lOID, out bool bFeature)
        {
            IObjectClass pFeatureclass = null;
            IObject ReturnFeature = null;
            bFeature = false;
            try
            {

                //trying to find the featureclass from the list 
                Program.EDGMCFeatureclasslist.TryGetValue(FeatureClassName, out pFeatureclass);
                //If not then get the featureclass from workspace
                if (pFeatureclass is null)
                {
                    pFeatureclass = GetObjectclass(FeatureClassName, pFWSpace);

                    //Add featureclass in the list so that next time no need to open from workspace
                    if (pFeatureclass != null)
                    {
                        if (!Program.EDGMCFeatureclasslist.ContainsKey(FeatureClassName))
                        {
                            Program.EDGMCFeatureclasslist.Add(FeatureClassName, pFeatureclass);
                        }
                    }
                }
                //Get the feature 
                if (pFeatureclass != null)
                {
                    if (pFeatureclass is IFeatureClass)
                    {
                        try
                        {
                            ReturnFeature = (pFeatureclass as IFeatureClass).GetFeature(lOID);


                        }
                        catch { }


                    }
                    else if (pFeatureclass is ITable)
                    {
                        ITable ptable = pFeatureclass as ITable;
                        ReturnFeature = ptable.GetRow(lOID) as IObject;

                    }
                }

            }
            catch (Exception ex)
            {


            }
            return ReturnFeature;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="featureClassName"></param>
        /// <param name="pFWSpace"></param>
        /// <returns></returns>
        public IObject GetFeatureFROMDefaultVersion(string FeatureClassName, IFeatureWorkspace pFWSpace, int lOID, out bool bFeature)
        {
            IObjectClass pFeatureclass = null;
            IObject ReturnFeature = null;
            bFeature = false;
            try
            {

                //trying to find the featureclass from the list 
                Program.DefaultFeatureclasslist.TryGetValue(FeatureClassName, out pFeatureclass);
                //If not then get the featureclass from workspace
                if (pFeatureclass is null)
                {
                    pFeatureclass = GetObjectclass(FeatureClassName, pFWSpace);

                    //Add featureclass in the list so that next time no need to open from workspace
                    if (pFeatureclass != null)
                    {
                        if (!Program.DefaultFeatureclasslist.ContainsKey(FeatureClassName))
                        {
                            Program.DefaultFeatureclasslist.Add(FeatureClassName, pFeatureclass);
                        }
                    }
                }
                //Get the feature 
                if (pFeatureclass != null)
                {
                    if (pFeatureclass is IFeatureClass)
                    {
                        string ss = pFeatureclass.AliasName.ToString();
                        bFeature = true;
                        try
                        {
                            ReturnFeature = (pFeatureclass as IFeatureClass).GetFeature(lOID);


                        }
                        catch (Exception ex) { }

                        ESRI.ArcGIS.esriSystem.IPropertySet pset = ((IWorkspace)pFeatureclass).ConnectionProperties;
                        object v1, v2;
                        pset.GetAllProperties(out v1, out v2);
                    }
                    else if (pFeatureclass is ITable)
                    {
                        ITable ptable = pFeatureclass as ITable;
                        ReturnFeature = ptable.GetRow(lOID) as IObject;

                    }
                    else { }

                }

            }
            catch (Exception ex)
            {


            }
            return ReturnFeature;
        }
        private IObjectClass GetObjectclass(string featureClassName, IFeatureWorkspace pFWSpace)
        {
            IObjectClass pFeatureclass = null;
            if (featureClassName.ToUpper().Contains("ANNO"))
            {
                try
                {
                    pFeatureclass = pFWSpace.OpenFeatureClass(featureClassName) as IObjectClass;
                }
                catch
                { }
            }
            else
            {
                try
                {
                    pFeatureclass = pFWSpace.OpenFeatureClass(featureClassName) as IObjectClass;
                }
                catch
                {
                    try
                    {
                        pFeatureclass = pFWSpace.OpenTable(featureClassName) as IObjectClass;
                    }
                    catch { }
                }

            }
            try
            {
                if (pFeatureclass == null)
                {
                    if(!Program.ClassNotExist.Contains (featureClassName ))
                    Program.ClassNotExist.Add(featureClassName);
                }
            }
            catch { }
            return pFeatureclass;

        }
    }
}
