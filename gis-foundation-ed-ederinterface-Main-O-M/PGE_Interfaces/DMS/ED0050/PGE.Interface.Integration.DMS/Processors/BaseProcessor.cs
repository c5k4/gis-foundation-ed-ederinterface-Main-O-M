using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Miner.Geodatabase.Integration;
using Miner.Geodatabase.Integration.Electric;
using PGE.Common.Delivery.Framework.Exceptions;
using PGE.Common.Delivery.Diagnostics;
using PGE.Interface.Integration.DMS.Common;
using PGE.Common.Delivery.Framework;
using System.Diagnostics;
// Changes for ENOS to SAP migration - DMS 
using PGE.Interface.Integration.DMS.Manager;

namespace PGE.Interface.Integration.DMS.Processors
{
    /// <summary>
    /// This class contains common logic for converting GIS data from network adapter to the Staging schema.
    /// </summary>
    public class BaseProcessor
    {
        /// <summary>
        /// Common logger for logging messages
        /// </summary>
        protected static Log4NetLogger _log4 = new Log4NetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "ED50.log4net.config");
        /// <summary>
        /// The in memory table in the Staging schema this processor creates data for. This will be sent to the database at the end of the processing.
        /// </summary>
        protected DataTable _table;
        /// <summary>
        /// A reference to the in memory staging schema data.
        /// </summary>
        protected DataSet _data;
        /// <summary>
        /// The name of the table in the Staging schema this processor creates data for.
        /// </summary>
        protected string _tableName;
        private FeederInfo _curFeederInfo;
        private Dictionary<string, int> _fcids;

        /// <summary>
        /// A reference to the feeder being processed and all its lines and devices.
        /// </summary>
        public FeederInfo CurFeederInfo
        {
            get { return _curFeederInfo; }
            set { _curFeederInfo = value; }
        }
        /// <summary>
        /// Initialize the processor
        /// </summary>
        /// <param name="data">A reference to the in memory staging schema data.</param>
        /// <param name="tableName">The name of the table in the Staging schema this processor creates data for.</param>
        public BaseProcessor(DataSet data, string tableName)
        {
            _data = data;
            _tableName = tableName;
            if (data.Tables.Contains(tableName))
            {
                _table = data.Tables[tableName];
            }
            _fcids = new Dictionary<string, int>();
        }
        /// <summary>
        /// Set common devicegroup values (GUID,Description)
        /// </summary>
        /// <param name="info">The device from network adapter</param>
        /// <param name="row">The staging table row to set the values on</param>
        protected void GetDeviceGroupValues(FeatureInfo info, DataRow row)
        {
            ObjectInfo devgroup = Utilities.getRelatedObject(info, FCID.Value[FCID.DeviceGroup]);
            if (devgroup != null)
            {
                row["DEVICE_GROUP_ID"] = devgroup.Fields["GLOBALID"].FieldValue.ToString();
                string devtype = Utilities.GetDeviceGroupType(devgroup);
                if (devtype != null)
                {
                    row["DEVICE_GROUP_DESC"] = devtype;
                }
            }
        }
        /// <summary>
        /// Set common line values (feeder/substation ID, phase)
        /// </summary>
        /// <param name="edge">The line from network adapter</param>
        /// <param name="row">The staging table row to set the values on</param>
        protected void GetElectricEdgeValues(ElectricEdge edge, DataRow row)
        {
            if (!NAStandalone.Substation)//we wont use feederid for substation
            {
                int fdrid = 0;
                if (_fcids.ContainsKey(edge.Feeder.FeederID))
                {
                    fdrid = _fcids[edge.Feeder.FeederID];
                }
                else
                {
                    fdrid = Convert.ToInt32(edge.Feeder.FeederID);
                }
                row["FDR_SUB_ID"] = fdrid;
            }
            if (edge.OperationalPhases != Miner.Interop.SetOfPhases.None)
            {
                row["PHASE_DESIGNATION"] = edge.OperationalPhases.ToString();
            }
        }
        /// <summary>
        /// Set common device values (feeder/substation ID, phase)
        /// </summary>
        /// <param name="junct">The device from network adapter</param>
        /// <param name="row">The staging table row to set the values on</param>
        protected void GetElectricJunctionValues(ElectricJunction junct, DataRow row)
        {
            if (!NAStandalone.Substation)//we wont use feederid for substation
            {
                int fdrid = 0;
                if (_fcids.ContainsKey(junct.Feeder.FeederID))
                {
                    fdrid = _fcids[junct.Feeder.FeederID];
                }
                else
                {
                    fdrid = Convert.ToInt32(junct.Feeder.FeederID);
                }
                row["FDR_SUB_ID"] = fdrid;
            }
            if (junct.OperationalPhases != Miner.Interop.SetOfPhases.None)
            {
                if (row.Table.Columns.Contains("PHASE_DESIGNATION"))
                {
                    row["PHASE_DESIGNATION"] = junct.OperationalPhases.ToString();
                }
            }
        }
        /// <summary>
        /// Set common values (GlobalID,Subtype,Substation Indicator)
        /// Copy field data from the network adapter feature to the staging schema row and, if the field is
        /// domained convert the code to the description. This is based off the settings in the Columns.xml file.
        /// </summary>
        /// <param name="info">The feature (device or line) from network adapter</param>
        /// <param name="row">The staging table row to set the values on</param>
        protected void GetMappedValues(ObjectInfo info, System.Data.DataRow row)
        {
            //first populate global values
            row["FEATURE_CLASS"] = info.TableName;
            if (info.Fields.ContainsKey("GLOBALID"))
            {
                row["GUID"] = info.Fields["GLOBALID"].FieldValue;
            }
            //sometimes in PGDBs the field name has mixed case, can probably remove this one substation is finally loaded into Oracle
            if (info.Fields.ContainsKey("GlobalID"))
            {
                row["GUID"] = info.Fields["GlobalID"].FieldValue;
            }
            object code = null;
            if (info.Fields.ContainsKey("SUBTYPECD"))
            {
                
                code = info.Fields["SUBTYPECD"].FieldValue;
                if (code != null && !(code is DBNull))
                {
                    if (CADOPS.Subtypes.SubtypeMap.ContainsKey(info.ObjectClassID))
                    {
                        if (CADOPS.Subtypes.SubtypeMap[info.ObjectClassID].ContainsKey((int)code))
                        {
                            row["FC_SUB_TYPE"] = CADOPS.Subtypes.SubtypeMap[info.ObjectClassID][(int)code];
                        }
                        else
                        {
                            string warning = info.TableName + " " + info.ObjectID + " subtype " + code + " not found";
                            _log4.Warn(warning);
                        }
                    }
                    else
                    {
                        string warning = info.TableName + " " + info.ObjectID + " has no subtypes";
                        _log4.Warn(warning);
                    }
                }
                else
                {
                    string warning = info.TableName + " " + info.ObjectID + " has a null subtype";
                    _log4.Warn(warning);
                }
            }
            if (NAStandalone.Substation)
            {
                row["SUBSTATION_OBJ_IDC"] = "Y";
            }
            else
            {
                row["SUBSTATION_OBJ_IDC"] = "N";
            }
            //now populate the mapped values
            if (ColumnMapper.Instance.Data.ContainsKey(_tableName))
            {
                if (ColumnMapper.Instance.Data[_tableName].ContainsKey(info.ObjectClassID))
                {
                    List<ColumnMap> list = ColumnMapper.Instance.Data[_tableName][info.ObjectClassID];
                    foreach (ColumnMap map in list)
                    {
                        object value = Utilities.GetDBFieldValue(info, map.GISField);
                        if (map.Domain != null)
                        {
                            //if the domain manager has the domain use that otherwise try looking up the subtype domain
                            if (DomainManager.Instance.ContainsDomain(map.Domain))
                            {
                                if (value is DBNull)
                                {
                                    value = DomainManager.Instance.GetValue(map.Domain,null);
                                }
                                else
                                {
                                    value = DomainManager.Instance.GetValue(map.Domain, value.ToString());
                                }
                            }
                            else
                            {
                                if (code != null)
                                {
                                    string domain = CADOPS.Subtypes.GetDomain(info.ObjectClassID, map.GISField, (int)code);
                                    if (domain != null)
                                    {
                                        if (value is DBNull)
                                        {
                                            value = DomainManager.Instance.GetValue(domain, null);
                                        }
                                        else
                                        {
                                            value = DomainManager.Instance.GetValue(domain, value.ToString());
                                        }
                                    }
                                }
                            }
                        }
                        if (map.DateFormat != null)
                        {
                            if (value is DateTime)
                            {
                                value = ((DateTime)value).ToString(map.DateFormat);
                            }
                        }
                        if (value == null)
                        {
                            value = DBNull.Value;
                        }
                        if (map.Replace != null)
                        {
                            if (!(value is DBNull))
                            {
                                foreach (string key in map.Replace.Keys)
                                {
                                    value = value.ToString().Replace(key, map.Replace[key]);
                                }
                            }
                        }
                        row[map.DMSField] = value;
                    }
                }
            }
        }

        /*Changes for ENOS to SAP migration - DMS ..Start */

        /// <summary>
        /// Set related columns for Servicelocation ( ServicePoint and GenerationInfo Columns ) 
        /// GENERATIONINFO - PROJECTNAME,GENTYPE,GLOBALID,EFFERATINGMACHKW,EFFRATINGINVKW
        /// SERVICEPOINT - SERVICEPOINTID
        /// Copy field data from the network adapter feature to the staging schema row and, if the field is
        /// domained convert the code to the description. This is based off the settings in the Columns.xml file.
        /// </summary>
        /// <param name="info">The feature (device or line) from network adapter</param>
        /// <param name="row">The staging table row to set the values on</param>
        protected void GetRelatedValuesForServiceLocation(ObjectInfo info, System.Data.DataRow row, ControlTable argControlTable)
        {
            ObjectInfo objInfoServPt = null;
            ObjectInfo objInfoGen = null;
            bool bRelatedPrimMeterTransformerFieldCaptured = false;
            try
            {
                // This function will run only for Service location 
                if (info.ObjectClassID != FCID.Value[FCID.ServiceLocation])
                    return;
                _log4.Info("Start Capturing related attributes for service location : " + info.ObjectID);
                try
                {
                    #region Capturing ServicePoint Attributes

                    // Find related Service Point and take field values -- 952 is FCID for Service point -- make it a variable                   
                    objInfoServPt = GetServicePointRelatedtoPrimGenInfo(info, ref objInfoGen);

                    if (objInfoServPt != null)
                    {
                        if (ColumnMapper.Instance.Data.ContainsKey(_tableName))
                        {
                            if (ColumnMapper.Instance.Data[_tableName].ContainsKey(FCID.Value[FCID.ServicePoint]))
                            {
                                List<ColumnMap> list = ColumnMapper.Instance.Data[_tableName][FCID.Value[FCID.ServicePoint]];
                                foreach (ColumnMap map in list)
                                {
                                    object value = Utilities.GetDBFieldValue(objInfoServPt, map.GISField);
                                    if (map.Domain != null)
                                    {
                                        //if the domain manager has the domain use that otherwise try looking up the subtype domain
                                        if (DomainManager.Instance.ContainsDomain(map.Domain))
                                        {
                                            if (value is DBNull)
                                            {
                                                value = DomainManager.Instance.GetValue(map.Domain, null);
                                            }
                                            else
                                            {
                                                value = DomainManager.Instance.GetValue(map.Domain, value.ToString());
                                            }
                                        }
                                    }
                                    if (map.DateFormat != null)
                                    {
                                        if (value is DateTime)
                                        {
                                            value = ((DateTime)value).ToString(map.DateFormat);
                                        }
                                    }
                                    if (value == null)
                                    {
                                        value = DBNull.Value;
                                    }
                                    if (map.Replace != null)
                                    {
                                        if (!(value is DBNull))
                                        {
                                            foreach (string key in map.Replace.Keys)
                                            {
                                                value = value.ToString().Replace(key, map.Replace[key]);
                                            }
                                        }
                                    }
                                    row[map.DMSField] = value;
                                }
                            }
                        }
                    }
                    else
                    {
                        string warning = "No service point found related to service location.";
                        _log4.Warn(warning);
                    }

                    #endregion Capturing ServicePoint Attributes
                }
                catch (Exception exp)
                {
                    _log4.Error(exp.Message +" at "+exp.StackTrace);
                }

                try
                {
                    #region Capturing GenerationInfoAttributes
                    // Find generationinfo related to Service Point and take field values -- 21722 is FCID for Generation Info -- make it a variable
                    if (objInfoGen != null)
                    {
                        if (ColumnMapper.Instance.Data.ContainsKey(_tableName))
                        {
                            if (ColumnMapper.Instance.Data[_tableName].ContainsKey(FCID.Value[FCID.GenerationInfo]))
                            {
                                List<ColumnMap> list = ColumnMapper.Instance.Data[_tableName][FCID.Value[FCID.GenerationInfo]];
                                foreach (ColumnMap map in list)
                                {
                                    object value = Utilities.GetDBFieldValue(objInfoGen, map.GISField);
                                    if (map.Domain != null)
                                    {
                                        //if the domain manager has the domain use that otherwise try looking up the subtype domain
                                        if (DomainManager.Instance.ContainsDomain(map.Domain))
                                        {
                                            if (value is DBNull)
                                            {
                                                value = DomainManager.Instance.GetValue(map.Domain, null);
                                            }
                                            else
                                            {
                                                value = DomainManager.Instance.GetValue(map.Domain, value.ToString());
                                            }
                                        }
                                    }
                                    if (map.DateFormat != null)
                                    {
                                        if (value is DateTime)
                                        {
                                            value = ((DateTime)value).ToString(map.DateFormat);
                                        }
                                    }
                                    if (value == null)
                                    {
                                        value = DBNull.Value;
                                    }
                                    if (map.Replace != null)
                                    {
                                        if (!(value is DBNull))
                                        {
                                            foreach (string key in map.Replace.Keys)
                                            {
                                                value = value.ToString().Replace(key, map.Replace[key]);
                                            }
                                        }
                                    }
                                    row[map.DMSField] = value;
                                }
                            }
                        }
                    }

                    #endregion Capturing GenerationInfoAttributes
                }
                catch (Exception exp)
                {
                    // throw;
                }

                #region Capturing attribute OPERATINGVOLTAGE from Primary Meter/Transformer
                ESRI.ArcGIS.Geodatabase.IQueryFilter pQf = null;
                ESRI.ArcGIS.Geodatabase.ICursor pCur = null;
                try
                {
                    //// Find related Primary/Meter/Transformer and take field values
                    string objTableName = argControlTable.GetObjectClassName(FCID.Value[FCID.ServicePoint]);
                    ESRI.ArcGIS.Geodatabase.ITable servPointTable = CADOPS.GetTable(objTableName);
                    pQf = new ESRI.ArcGIS.Geodatabase.QueryFilterClass();
                    pQf.WhereClause = "OBJECTID=" + objInfoServPt.ObjectID;

                    pCur = servPointTable.Search(pQf, false);
                    ESRI.ArcGIS.Geodatabase.IRow pRow = pCur.NextRow();
                    if (pRow != null)
                    {
                        ESRI.ArcGIS.Geodatabase.IEnumRelationshipClass enumRelClass = ((ESRI.ArcGIS.Geodatabase.IObjectClass)pRow.Table).get_RelationshipClasses(ESRI.ArcGIS.Geodatabase.esriRelRole.esriRelRoleDestination);
                        ESRI.ArcGIS.Geodatabase.IRelationshipClass relClass = enumRelClass.Next();
                        while (relClass != null)
                        {
                            if (relClass.OriginClass.ObjectClassID == FCID.Value[FCID.PrimaryMeter])
                            {
                                #region Primary Meter

                                ESRI.ArcGIS.esriSystem.ISet relSet = relClass.GetObjectsRelatedToObject((ESRI.ArcGIS.Geodatabase.IObject)pRow);
                                relSet.Reset();

                                ESRI.ArcGIS.Geodatabase.IFeature originFeature = (ESRI.ArcGIS.Geodatabase.IFeature)relSet.Next();
                                if (originFeature != null)
                                {
                                    if (ColumnMapper.Instance.Data.ContainsKey(_tableName))
                                    {
                                        if (ColumnMapper.Instance.Data[_tableName].ContainsKey(FCID.Value[FCID.PrimaryMeter]))
                                        {
                                            List<ColumnMap> list = ColumnMapper.Instance.Data[_tableName][FCID.Value[FCID.PrimaryMeter]];
                                            foreach (ColumnMap map in list)
                                            {
                                                if (originFeature.Fields.FindField(map.GISField) == -1)
                                                    continue;

                                                object value = originFeature.get_Value(originFeature.Fields.FindField(map.GISField));
                                                if (map.Domain != null)
                                                {
                                                    //if the domain manager has the domain use that otherwise try looking up the subtype domain
                                                    if (DomainManager.Instance.ContainsDomain(map.Domain))
                                                    {
                                                        if (value is DBNull)
                                                        {
                                                            value = DomainManager.Instance.GetValue(map.Domain, null);
                                                        }
                                                        else
                                                        {
                                                            value = DomainManager.Instance.GetValue(map.Domain, value.ToString());
                                                        }
                                                    }
                                                }
                                                if (map.DateFormat != null)
                                                {
                                                    if (value is DateTime)
                                                    {
                                                        value = ((DateTime)value).ToString(map.DateFormat);
                                                    }
                                                }
                                                if (value == null)
                                                {
                                                    value = DBNull.Value;
                                                }
                                                if (map.Replace != null)
                                                {
                                                    if (!(value is DBNull))
                                                    {
                                                        foreach (string key in map.Replace.Keys)
                                                        {
                                                            value = value.ToString().Replace(key, map.Replace[key]);
                                                        }
                                                    }
                                                }
                                                row[map.DMSField] = value;
                                                bRelatedPrimMeterTransformerFieldCaptured = true;
                                                break;
                                            }
                                            if (bRelatedPrimMeterTransformerFieldCaptured)
                                                break;
                                        }
                                    }
                                }
                                #endregion Primary Meter
                            }
                            // Transformer
                            else if (relClass.OriginClass.ObjectClassID == FCID.Value[FCID.Transformer])
                            {
                                #region Transformer

                                ESRI.ArcGIS.esriSystem.ISet relSet = relClass.GetObjectsRelatedToObject((ESRI.ArcGIS.Geodatabase.IObject)pRow);
                                relSet.Reset();

                                ESRI.ArcGIS.Geodatabase.IFeature originFeature = (ESRI.ArcGIS.Geodatabase.IFeature)relSet.Next();
                                if (originFeature != null)
                                {

                                    if (ColumnMapper.Instance.Data.ContainsKey(_tableName))
                                    {
                                        if (ColumnMapper.Instance.Data[_tableName].ContainsKey(FCID.Value[FCID.Transformer]))
                                        {
                                            List<ColumnMap> list = ColumnMapper.Instance.Data[_tableName][FCID.Value[FCID.Transformer]];
                                            foreach (ColumnMap map in list)
                                            {
                                                if (originFeature.Fields.FindField(map.GISField) == -1)
                                                    continue;

                                                object value = originFeature.get_Value(originFeature.Fields.FindField(map.GISField));
                                                if (map.Domain != null)
                                                {
                                                    //if the domain manager has the domain use that otherwise try looking up the subtype domain
                                                    if (DomainManager.Instance.ContainsDomain(map.Domain))
                                                    {
                                                        if (value is DBNull)
                                                        {
                                                            value = DomainManager.Instance.GetValue(map.Domain, null);
                                                        }
                                                        else
                                                        {
                                                            value = DomainManager.Instance.GetValue(map.Domain, value.ToString());
                                                        }
                                                    }
                                                }
                                                if (map.DateFormat != null)
                                                {
                                                    if (value is DateTime)
                                                    {
                                                        value = ((DateTime)value).ToString(map.DateFormat);
                                                    }
                                                }
                                                if (value == null)
                                                {
                                                    value = DBNull.Value;
                                                }
                                                if (map.Replace != null)
                                                {
                                                    if (!(value is DBNull))
                                                    {
                                                        foreach (string key in map.Replace.Keys)
                                                        {
                                                            value = value.ToString().Replace(key, map.Replace[key]);
                                                        }
                                                    }
                                                }
                                                row[map.DMSField] = value;
                                                bRelatedPrimMeterTransformerFieldCaptured = true;
                                                break;
                                            }
                                            if (bRelatedPrimMeterTransformerFieldCaptured)
                                                break;
                                        }
                                    }
                                }

                                #endregion Transformer
                            }
                            relClass = enumRelClass.Next();
                        }
                    }

                #endregion Capturing attribute OPERATINGVOLTAGE from Primary Meter/Transformer

                    _log4.Info("End Capturing related attributes for service location : " + info.ObjectID);
                }
                catch (Exception exp)
                {
                    _log4.Error(exp.Message + " at " + exp.StackTrace);
                }
                finally
                {
                    if (pCur != null)
                    {
                        System.Runtime.InteropServices.Marshal.FinalReleaseComObject(pCur);
                    }

                    if (pQf != null)
                    {
                        System.Runtime.InteropServices.Marshal.FinalReleaseComObject(pQf);
                    }
                }
            }
            catch (Exception exp)
            {
                _log4.Error(exp.Message + " at " + exp.StackTrace);
            }
        }


        /// <summary>
        /// This function returns the specific service point which has related gen info considered as Primary
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        protected ObjectInfo GetServicePointRelatedtoPrimGenInfo(ObjectInfo info, ref ObjectInfo objInfoGen)
        {
            List<ObjectInfo> lstServPoints = null;
            ObjectInfo objInfoGenInfo = null;
            ObjectInfo objInfoServPtToReturn = null;
            try
            {
                lstServPoints = Utilities.getAllRelatedObjects(info, FCID.Value[FCID.ServicePoint]);

                foreach (ObjectInfo objInfoServPt in lstServPoints)
                {
                    objInfoGenInfo = Utilities.getRelatedObject(objInfoServPt, FCID.Value[FCID.GenerationInfo]);
                    if (objInfoGenInfo != null)
                    {
                        object value = Utilities.GetDBFieldValue(objInfoGenInfo, Configuration.GenSymbologyFieldName);
                        if (Convert.ToString(value).ToUpper() == Configuration.GenCategoryValueForPrimary.ToUpper())
                        {
                            objInfoServPtToReturn = objInfoServPt;
                            objInfoGen = objInfoGenInfo;
                            break;
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                _log4.Error(exp.Message + " at " + exp.StackTrace);
            }
            return objInfoServPtToReturn;
        }

        // Changes for sending all generations to DMS when service location : service point : generation is 1:n:n -- Start 
        protected Dictionary<ObjectInfo, ObjectInfo> GetAllServicePointRelatedtoPrimGenInfo(ObjectInfo info)
        {
            Dictionary<ObjectInfo, ObjectInfo> dictServPointGenInfo = null;
            List<ObjectInfo> lstServPoints = null;
            ObjectInfo objInfoGenInfo = null;            
            try
            {
                dictServPointGenInfo = new Dictionary<ObjectInfo, ObjectInfo>();
                lstServPoints = Utilities.getAllRelatedObjects(info, FCID.Value[FCID.ServicePoint]);

                foreach (ObjectInfo objInfoServPt in lstServPoints)
                {
                    objInfoGenInfo = Utilities.getRelatedObject(objInfoServPt, FCID.Value[FCID.GenerationInfo]);
                    if (objInfoGenInfo != null)
                    {
                        object value = Utilities.GetDBFieldValue(objInfoGenInfo, Configuration.GenSymbologyFieldName);
                        if (Convert.ToString(value).ToUpper() == Configuration.GenCategoryValueForPrimary.ToUpper())
                        {     
                            dictServPointGenInfo.Add(objInfoServPt, objInfoGenInfo);                           
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                _log4.Error(exp.Message + " at " + exp.StackTrace);
            }
            return dictServPointGenInfo;
        }

        /// <summary>
        /// Set related columns for Servicelocation ( ServicePoint and GenerationInfo Columns ) 
        /// GENERATIONINFO - PROJECTNAME,GENTYPE,GLOBALID,EFFERATINGMACHKW,EFFRATINGINVKW
        /// SERVICEPOINT - SERVICEPOINTID
        /// Copy field data from the network adapter feature to the staging schema row and, if the field is
        /// domained convert the code to the description. This is based off the settings in the Columns.xml file.
        /// </summary>
        /// <param name="info">The feature (device or line) from network adapter</param>
        /// <param name="row">The staging table row to set the values on</param>
        protected void GetRelatedValuesForServiceLocationForGivenServicePointAndGenInfo(ObjectInfo info, System.Data.DataRow row, ControlTable argControlTable, ObjectInfo objInfoServPt, ObjectInfo objInfoGen)
        {           
            bool bRelatedPrimMeterTransformerFieldCaptured = false;
            try
            {               
                _log4.Info("Start Capturing related attributes for service location : " + info.ObjectID);
                try
                {
                    #region Capturing ServicePoint Attributes                  

                    if (objInfoServPt != null)
                    {
                        if (ColumnMapper.Instance.Data.ContainsKey(_tableName))
                        {
                            if (ColumnMapper.Instance.Data[_tableName].ContainsKey(FCID.Value[FCID.ServicePoint]))
                            {
                                List<ColumnMap> list = ColumnMapper.Instance.Data[_tableName][FCID.Value[FCID.ServicePoint]];
                                foreach (ColumnMap map in list)
                                {
                                    object value = Utilities.GetDBFieldValue(objInfoServPt, map.GISField);
                                    if (map.Domain != null)
                                    {
                                        //if the domain manager has the domain use that otherwise try looking up the subtype domain
                                        if (DomainManager.Instance.ContainsDomain(map.Domain))
                                        {
                                            if (value is DBNull)
                                            {
                                                value = DomainManager.Instance.GetValue(map.Domain, null);
                                            }
                                            else
                                            {
                                                value = DomainManager.Instance.GetValue(map.Domain, value.ToString());
                                            }
                                        }
                                    }
                                    if (map.DateFormat != null)
                                    {
                                        if (value is DateTime)
                                        {
                                            value = ((DateTime)value).ToString(map.DateFormat);
                                        }
                                    }
                                    if (value == null)
                                    {
                                        value = DBNull.Value;
                                    }
                                    if (map.Replace != null)
                                    {
                                        if (!(value is DBNull))
                                        {
                                            foreach (string key in map.Replace.Keys)
                                            {
                                                value = value.ToString().Replace(key, map.Replace[key]);
                                            }
                                        }
                                    }
                                    row[map.DMSField] = value;
                                }
                            }
                        }
                    }
                    else
                    {
                        string warning = "No service point found related to service location.";
                        _log4.Warn(warning);
                    }

                    #endregion Capturing ServicePoint Attributes
                }
                catch (Exception exp)
                {
                    _log4.Error(exp.Message + " at " + exp.StackTrace);
                }

                try
                {
                    #region Capturing GenerationInfoAttributes
                    // Find generationinfo related to Service Point and take field values -- 21722 is FCID for Generation Info -- make it a variable
                    if (objInfoGen != null)
                    {
                        if (ColumnMapper.Instance.Data.ContainsKey(_tableName))
                        {
                            if (ColumnMapper.Instance.Data[_tableName].ContainsKey(FCID.Value[FCID.GenerationInfo]))
                            {
                                List<ColumnMap> list = ColumnMapper.Instance.Data[_tableName][FCID.Value[FCID.GenerationInfo]];
                                foreach (ColumnMap map in list)
                                {
                                    object value = Utilities.GetDBFieldValue(objInfoGen, map.GISField);
                                    if (map.Domain != null)
                                    {
                                        //if the domain manager has the domain use that otherwise try looking up the subtype domain
                                        if (DomainManager.Instance.ContainsDomain(map.Domain))
                                        {
                                            if (value is DBNull)
                                            {
                                                value = DomainManager.Instance.GetValue(map.Domain, null);
                                            }
                                            else
                                            {
                                                value = DomainManager.Instance.GetValue(map.Domain, value.ToString());
                                            }
                                        }
                                    }
                                    if (map.DateFormat != null)
                                    {
                                        if (value is DateTime)
                                        {
                                            value = ((DateTime)value).ToString(map.DateFormat);
                                        }
                                    }
                                    if (value == null)
                                    {
                                        value = DBNull.Value;
                                    }
                                    if (map.Replace != null)
                                    {
                                        if (!(value is DBNull))
                                        {
                                            foreach (string key in map.Replace.Keys)
                                            {
                                                value = value.ToString().Replace(key, map.Replace[key]);
                                            }
                                        }
                                    }
                                    row[map.DMSField] = value;
                                }
                            }
                        }
                    }

                    #endregion Capturing GenerationInfoAttributes
                }
                catch (Exception exp)
                {
                    // throw;
                }

                #region Capturing attribute OPERATINGVOLTAGE from Primary Meter/Transformer
                ESRI.ArcGIS.Geodatabase.IQueryFilter pQf = null;
                ESRI.ArcGIS.Geodatabase.ICursor pCur = null;
                try
                {
                    //// Find related Primary/Meter/Transformer and take field values
                    string objTableName = argControlTable.GetObjectClassName(FCID.Value[FCID.ServicePoint]);
                    ESRI.ArcGIS.Geodatabase.ITable servPointTable = CADOPS.GetTable(objTableName);
                    pQf = new ESRI.ArcGIS.Geodatabase.QueryFilterClass();
                    pQf.WhereClause = "OBJECTID=" + objInfoServPt.ObjectID;

                    pCur = servPointTable.Search(pQf, false);
                    ESRI.ArcGIS.Geodatabase.IRow pRow = pCur.NextRow();
                    if (pRow != null)
                    {
                        ESRI.ArcGIS.Geodatabase.IEnumRelationshipClass enumRelClass = ((ESRI.ArcGIS.Geodatabase.IObjectClass)pRow.Table).get_RelationshipClasses(ESRI.ArcGIS.Geodatabase.esriRelRole.esriRelRoleDestination);
                        ESRI.ArcGIS.Geodatabase.IRelationshipClass relClass = enumRelClass.Next();
                        while (relClass != null)
                        {
                            if (relClass.OriginClass.ObjectClassID == FCID.Value[FCID.PrimaryMeter])
                            {
                                #region Primary Meter

                                ESRI.ArcGIS.esriSystem.ISet relSet = relClass.GetObjectsRelatedToObject((ESRI.ArcGIS.Geodatabase.IObject)pRow);
                                relSet.Reset();

                                ESRI.ArcGIS.Geodatabase.IFeature originFeature = (ESRI.ArcGIS.Geodatabase.IFeature)relSet.Next();
                                if (originFeature != null)
                                {
                                    if (ColumnMapper.Instance.Data.ContainsKey(_tableName))
                                    {
                                        if (ColumnMapper.Instance.Data[_tableName].ContainsKey(FCID.Value[FCID.PrimaryMeter]))
                                        {
                                            List<ColumnMap> list = ColumnMapper.Instance.Data[_tableName][FCID.Value[FCID.PrimaryMeter]];
                                            foreach (ColumnMap map in list)
                                            {
                                                if (originFeature.Fields.FindField(map.GISField) == -1)
                                                    continue;

                                                object value = originFeature.get_Value(originFeature.Fields.FindField(map.GISField));
                                                if (map.Domain != null)
                                                {
                                                    //if the domain manager has the domain use that otherwise try looking up the subtype domain
                                                    if (DomainManager.Instance.ContainsDomain(map.Domain))
                                                    {
                                                        if (value is DBNull)
                                                        {
                                                            value = DomainManager.Instance.GetValue(map.Domain, null);
                                                        }
                                                        else
                                                        {
                                                            value = DomainManager.Instance.GetValue(map.Domain, value.ToString());
                                                        }
                                                    }
                                                }
                                                if (map.DateFormat != null)
                                                {
                                                    if (value is DateTime)
                                                    {
                                                        value = ((DateTime)value).ToString(map.DateFormat);
                                                    }
                                                }
                                                if (value == null)
                                                {
                                                    value = DBNull.Value;
                                                }
                                                if (map.Replace != null)
                                                {
                                                    if (!(value is DBNull))
                                                    {
                                                        foreach (string key in map.Replace.Keys)
                                                        {
                                                            value = value.ToString().Replace(key, map.Replace[key]);
                                                        }
                                                    }
                                                }
                                                row[map.DMSField] = value;
                                                bRelatedPrimMeterTransformerFieldCaptured = true;
                                                break;
                                            }
                                            if (bRelatedPrimMeterTransformerFieldCaptured)
                                                break;
                                        }
                                    }
                                }
                                #endregion Primary Meter
                            }
                            // Transformer
                            else if (relClass.OriginClass.ObjectClassID == FCID.Value[FCID.Transformer])
                            {
                                #region Transformer

                                ESRI.ArcGIS.esriSystem.ISet relSet = relClass.GetObjectsRelatedToObject((ESRI.ArcGIS.Geodatabase.IObject)pRow);
                                relSet.Reset();

                                ESRI.ArcGIS.Geodatabase.IFeature originFeature = (ESRI.ArcGIS.Geodatabase.IFeature)relSet.Next();
                                if (originFeature != null)
                                {

                                    if (ColumnMapper.Instance.Data.ContainsKey(_tableName))
                                    {
                                        if (ColumnMapper.Instance.Data[_tableName].ContainsKey(FCID.Value[FCID.Transformer]))
                                        {
                                            List<ColumnMap> list = ColumnMapper.Instance.Data[_tableName][FCID.Value[FCID.Transformer]];
                                            foreach (ColumnMap map in list)
                                            {
                                                if (originFeature.Fields.FindField(map.GISField) == -1)
                                                    continue;

                                                object value = originFeature.get_Value(originFeature.Fields.FindField(map.GISField));
                                                if (map.Domain != null)
                                                {
                                                    //if the domain manager has the domain use that otherwise try looking up the subtype domain
                                                    if (DomainManager.Instance.ContainsDomain(map.Domain))
                                                    {
                                                        if (value is DBNull)
                                                        {
                                                            value = DomainManager.Instance.GetValue(map.Domain, null);
                                                        }
                                                        else
                                                        {
                                                            value = DomainManager.Instance.GetValue(map.Domain, value.ToString());
                                                        }
                                                    }
                                                }
                                                if (map.DateFormat != null)
                                                {
                                                    if (value is DateTime)
                                                    {
                                                        value = ((DateTime)value).ToString(map.DateFormat);
                                                    }
                                                }
                                                if (value == null)
                                                {
                                                    value = DBNull.Value;
                                                }
                                                if (map.Replace != null)
                                                {
                                                    if (!(value is DBNull))
                                                    {
                                                        foreach (string key in map.Replace.Keys)
                                                        {
                                                            value = value.ToString().Replace(key, map.Replace[key]);
                                                        }
                                                    }
                                                }
                                                row[map.DMSField] = value;
                                                bRelatedPrimMeterTransformerFieldCaptured = true;
                                                break;
                                            }
                                            if (bRelatedPrimMeterTransformerFieldCaptured)
                                                break;
                                        }
                                    }
                                }

                                #endregion Transformer
                            }
                            relClass = enumRelClass.Next();
                        }
                    }

                #endregion Capturing attribute OPERATINGVOLTAGE from Primary Meter/Transformer

                    _log4.Info("End Capturing related attributes for service location : " + info.ObjectID);
                }
                catch (Exception exp)
                {
                    _log4.Error(exp.Message + " at " + exp.StackTrace);
                }
                finally
                {
                    if (pCur != null)
                    {
                        System.Runtime.InteropServices.Marshal.FinalReleaseComObject(pCur);
                    }

                    if (pQf != null)
                    {
                        System.Runtime.InteropServices.Marshal.FinalReleaseComObject(pQf);
                    }
                }
            }
            catch (Exception exp)
            {
                _log4.Error(exp.Message + " at " + exp.StackTrace);
            }
        }
        
        // Changes for sending all generations to DMS when service location : service point : generation is 1:n:n -- End 


        /*Changes for ENOS to SAP migration - DMS ..End */
    }
}
