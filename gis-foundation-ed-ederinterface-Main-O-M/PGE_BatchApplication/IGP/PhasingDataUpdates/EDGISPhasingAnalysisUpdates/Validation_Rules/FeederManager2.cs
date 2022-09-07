using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using Miner.Geodatabase;
using Miner.Geodatabase.Network;
using Miner.Geodatabase.FeederManager;
//using Telvent.Delivery.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
//using System.Windows.Forms;
using Miner.Interop;
using PGE.BatchApplication.IGPPhaseUpdate;
using System.IO;

namespace Telvent.Delivery.Framework.FeederManager
{
    public static class FeederManager2
    {
        public static string[] FeederManagerValidationRules = {"PGE Validate Island Feature", "PGE Validate Loop Feature", "PGE Validate Multi-feed Feature","PGE Validate Source Connectivity"};
        private const string EditedFeaturesTableModelName = "PGE_MM_EDITED_FEATURES";
        private const string EditedFeaturesClassIDFieldName = "CLASSID";
        private const string EditedFeaturesObjectIDFieldName = "FEATUREID";
        private const string EditedFeaturesVersionFieldName = "VERSION";
        private const string DeviceGroupModelName = "PGE_DEVICEGROUP";
        private const string CircuitSourceModelName = "CIRCUITSOURCE";
        private const string FeederIDModelName = "FEEDERID";
        private const string PGEFeederIDModelName = "PGE_CIRCUITID";

        //private static Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "");
        public static Log4NetLogger _log = new Log4NetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, ReadConfigurations.LOGCONFIG);
        private static Dictionary<IWorkspace, FeederInfoProvider> feederInfoProviders = new Dictionary<IWorkspace, FeederInfoProvider>();

        /// <summary>
        /// This method returns an objectID list for any features that were "edited" by feeder manager 2 in the specified version
        /// </summary>
        /// <param name="workspace">Workspace of the database to use</param>
        /// <param name="versionName">Name of the version to check</param>
        /// <param name="classID">Class ID for the feature class to check</param>
        /// <returns></returns>
        public static List<int> GetEditedFeatures(IFeatureWorkspace workspace, string versionName, IObjectClass classToCheck)
        {
            return GetEditedFeatures(workspace, versionName, classToCheck.ObjectClassID);
        }

        private static ITable GetMMEditedFeaturesTable(IWorkspace ws)
        {
            ITable table = null;
            try
            {
                IMMEnumTable enumObjClass = ModelNameManager.Instance.TablesFromModelNameWS(ws, FeederManager2.EditedFeaturesTableModelName);
                enumObjClass.Reset();
                table = enumObjClass.Next();
                if (table == null)
                {
                    //_logger.Error("Unable to find MM_EDITED_FEATURES table.  Is the feeder sync tool enabled?");
                    WriteLine_Error("Unable to find MM_EDITED_FEATURES table. Is the feeder sync tool enabled?"); 
                }
            }
            catch(Exception ex)
            {
                //_logger.Error("Unable to find MM_EDITED_FEATURES table.  Is the feeder sync tool enabled?");
                WriteLine_Error("Unable to find MM_EDITED_FEATURES table. Is the feeder sync tool enabled?"); 
            }
            return table;
        }

        /// <summary>
        /// This method returns an objectID list for any features that were "edited" by feeder manager 2 in the specified version
        /// </summary>
        /// <param name="workspace">Workspace of the database to use</param>
        /// <param name="versionName">Name of the version to check</param>
        /// <param name="classID">Class ID for the feature class to check</param>
        /// <returns></returns>
        public static List<int> GetEditedFeatures(IFeatureWorkspace workspace, string versionName, int classToCheck)
        {
            List<int> editedOIDs = new List<int>();
            ITable editedFeaturesTable = null;
            ICursor cursor = null;
            IQueryFilter qf = null;
            IRow row = null;
            try
            {
                editedFeaturesTable = GetMMEditedFeaturesTable(workspace as IWorkspace);
                int featureIDIndex = editedFeaturesTable.FindField(EditedFeaturesObjectIDFieldName);
                qf = new QueryFilterClass();
                qf.SubFields = EditedFeaturesObjectIDFieldName;
                qf.WhereClause = EditedFeaturesVersionFieldName + " = '" + versionName + "' AND " + EditedFeaturesClassIDFieldName + " = " + classToCheck;
                cursor = editedFeaturesTable.Search(qf, false);
                while ((row = cursor.NextRow()) != null)
                {
                    try
                    {
                        int oid = Int32.Parse(row.get_Value(featureIDIndex).ToString());
                        editedOIDs.Add(oid);
                    }
                    catch (Exception ex)
                    {
                        WriteLine_Error("Error getting " + EditedFeaturesObjectIDFieldName + " from " + editedFeaturesTable + " table");
                    }
                    if (row != null) { while (Marshal.ReleaseComObject(row) > 0) { } }
                }
            }
            catch (Exception ex)
            {
                //_logger.Error("Failed to obtain list of feeder manager 2.0 circuit changes: " + ex.Message);
                WriteLine_Error("Failed to obtain list of feeder manager 2.0 circuit changes: " + ex.Message);
            }
            finally
            {
                if (qf != null) { while (Marshal.ReleaseComObject(qf) > 0) { } }
                if (row != null) { while (Marshal.ReleaseComObject(row) > 0) { } }
                if (cursor != null) { while (Marshal.ReleaseComObject(cursor) > 0) { } }
                if (editedFeaturesTable != null) { while (Marshal.ReleaseComObject(editedFeaturesTable) > 0) { } }
            }

            return editedOIDs.Distinct().ToList();
        }

        /// <summary>
        /// This method returns dictionary of class IDs and object ids for any features that were "edited" by feeder manager 2 in the specified version
        /// </summary>
        /// <param name="workspace">Workspace of the database to use</param>
        /// <param name="versionName">Name of the version to check</param>
        /// <returns></returns>
        public static Dictionary<int,List<int>> GetEditedFeatures(IFeatureWorkspace workspace, string versionName)
        {
            Dictionary<int, List<int>> editedOIDsByClassID = new Dictionary<int, List<int>>();
            ITable editedFeaturesTable = null;
            ICursor cursor = null;
            IQueryFilter qf = null;
            IRow row = null;
            try
            {
                editedFeaturesTable = GetMMEditedFeaturesTable(workspace as IWorkspace);
                int classIDFieldIndex = editedFeaturesTable.FindField(EditedFeaturesClassIDFieldName);
                qf = new QueryFilterClass();
                qf.SubFields = EditedFeaturesClassIDFieldName;
                qf.WhereClause = EditedFeaturesVersionFieldName + " = '" + versionName + "'";
                List<int> classIDsToGetEdits = new List<int>();
                cursor = editedFeaturesTable.Search(qf, false);
                while ((row = cursor.NextRow()) != null)
                {
                    try
                    {
                        int classID = Int32.Parse(row.get_Value(classIDFieldIndex).ToString());
                        classIDsToGetEdits.Add(classID);
                    }
                    catch (Exception ex)
                    {
                        //_logger.Error("Error getting " + EditedFeaturesClassIDFieldName + " from " + editedFeaturesTable + " table");
                        WriteLine_Error("Error getting " + EditedFeaturesClassIDFieldName + " from " + editedFeaturesTable + " table");
                    }
                    if (row != null) { while (Marshal.ReleaseComObject(row) > 0) { } }
                }

                classIDsToGetEdits = classIDsToGetEdits.Distinct().ToList();
                foreach (int classID in classIDsToGetEdits)
                {
                    editedOIDsByClassID.Add(classID, GetEditedFeatures(workspace, versionName, classID));
                }
            }
            catch (Exception ex)
            {
               // _logger.Error("Failed to obtain list of feeder manager 2.0 circuit changes: " + ex.Message);
                WriteLine_Error("Failed to obtain list of feeder manager 2.0 circuit changes: " + ex.Message);
            }
            finally
            {
                if (qf != null) { while (Marshal.ReleaseComObject(qf) > 0) { } }
                if (row != null) { while (Marshal.ReleaseComObject(row) > 0) { } }
                if (cursor != null) { while (Marshal.ReleaseComObject(cursor) > 0) { } }
                if (editedFeaturesTable != null) { while (Marshal.ReleaseComObject(editedFeaturesTable) > 0) { } }
            }

            return editedOIDsByClassID;
        }

        /// <summary>
        /// Returns the circuit ID for a provided row object
        /// </summary>
        /// <param name="row">IRow to obtain the circuit ID for</param>
        /// <returns></returns>
        public static string[] GetCircuitIDs(IRow row)
        {
            string[] circuitIDs = null;

            try
            {
                IFeederInfo<FeatureKey> feederInfo = GetFeederInfo(row);

                if (feederInfo == null || (circuitIDs = feederInfo.FeederIDs).Length == 0) //S2NN: change to check if Feederinfo is not returning FeederID
                {
                    //If feeder info is null, let's check if this is the circuitsource table.
                    if (ModelNameManager.Instance.ContainsClassModelName(row.Table as IObjectClass, CircuitSourceModelName)
                        || ModelNameManager.Instance.ContainsClassModelName(row.Table as IObjectClass, DeviceGroupModelName))
                    {
                        //This is the circuitsource or devicegroup table so we want to use the circuit ID field.
                        IField circuitIDField = ModelNameManager.Instance.FieldFromModelName(row.Table as IObjectClass, FeederIDModelName);
                        if (circuitIDField == null) { circuitIDField = ModelNameManager.Instance.FieldFromModelName(row.Table as IObjectClass, PGEFeederIDModelName); }
                        int circuitIDFieldIdx = row.Table.FindField(circuitIDField.Name);
                        circuitIDs = new string[1];
                        circuitIDs[0] = row.get_Value(circuitIDFieldIdx).ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                //_logger.Error("Failed to determine Circuit ID: " + ex.Message + "\nStackTrace:" + ex.StackTrace);
                WriteLine_Error("Failed to determine Circuit ID: " + ex.Message + "\nStackTrace:" + ex.StackTrace);
            }
            return circuitIDs;
        }

        /// <summary>
        /// Returns the Feeder information for a provided row object
        /// </summary>
        /// <param name="row">IRow to obtain feeder information for</param>
        /// <returns></returns>
        public static IFeederInfo<FeatureKey> GetFeederInfo(IRow row)
        {
            IFeederInfo<FeatureKey> feederInfo = null;
            try
            {
                ITable pRowTable = GetTableFromRow(row);
                IDataset ds = pRowTable as IDataset;
                if (ds != null && ds.Workspace != null)
                {
                    if (!feederInfoProviders.ContainsKey(ds.Workspace))
                    {
                        IConnectionProperties connectionProps = new ConnectionProperties(ds.Workspace);
                        feederInfoProviders.Add(ds.Workspace, new FeederInfoProvider(connectionProps));
                    }

                    if (feederInfoProviders.ContainsKey(ds.Workspace))
                    {
                        FeatureKey featKey = new FeatureKey(((IObjectClass)pRowTable).ObjectClassID, row.OID);
                        feederInfo = feederInfoProviders[ds.Workspace].GetFeederInfo(featKey);
                    }
                }
            }
            catch(Exception ex)
            {
                /*Failed to obtain via feeder info.*/
                //_logger.Error("Failed to determine Feeder information: " + ex.Message + "\nStackTrace:" + ex.StackTrace);
                WriteLine_Error("Failed to determine Feeder information: " + ex.Message + "\nStackTrace:" + ex.StackTrace);
            }           

            return feederInfo;
        }


        /// <summary>
        /// Determines whether a give row is multifed or not
        /// </summary>
        /// <param name="row">IRow to determine whether multifed</param>
        /// <returns></returns>
        public static bool IsMultiFed(IRow row)
        {
            bool isMultiFed = false;
            try
            {
                IFeederInfo<FeatureKey> feederInfo = GetFeederInfo(row);
                isMultiFed = feederInfo.IsMultifed;
            }
            catch (Exception ex)
            {
                /*Failed to obtain via feeder info.*/
                //_logger.Error("Failed to determine multi feed status: " + ex.Message + "\nStackTrace:" + ex.StackTrace);
                WriteLine_Error("Failed to determine multi feed status: " + ex.Message + "\nStackTrace:" + ex.StackTrace);
            }
            return isMultiFed;
        }

        /// <summary>
        /// Determines whether a given row is in a loop or not
        /// </summary>
        /// <param name="row">IRow to determine whether in loop</param>
        /// <returns></returns>
        public static bool IsInLoop(IRow row)
        {
            bool isInLoop = false;
            try
            {
                IFeederInfo<FeatureKey> feederInfo = GetFeederInfo(row);
                isInLoop = feederInfo.IsLooped;
            }
            catch (Exception ex)
            {
                /*Failed to obtain via feeder info.*/
                //_logger.Error("Failed to determine loop status: " + ex.Message + "\nStackTrace:" + ex.StackTrace);
                WriteLine_Error("Failed to determine loop status: " + ex.Message + "\nStackTrace:" + ex.StackTrace);
            }
            return isInLoop;
        }

        /// <summary>
        /// Determine whether a give feature is a tie device
        /// </summary>
        /// <param name="row">IRow to determine whether Tie Device</param>
        /// <returns></returns>
        public static bool IsTieDevice(IRow row)
        {
            bool isTieDevice = false;
            try
            {
                IFeederInfo<FeatureKey> feederInfo = GetFeederInfo(row);
                isTieDevice = feederInfo.IsTieDevice;
            }
            catch (Exception ex)
            {
                /*Failed to obtain via feeder info.*/
                //_logger.Error("Failed to determine tie device status: " + ex.Message + "\nStackTrace:" + ex.StackTrace);
                WriteLine_Error("Failed to determine tie device status: " + ex.Message + "\nStackTrace:" + ex.StackTrace);
            }
            return isTieDevice;
        }

        /// <summary>
        /// Determines the IObjectClass associated with the row.  For feeder manager 2.0 joins it must use the IRelQueryTable interface
        /// to determine the object class
        /// </summary>
        /// <param name="row">IRow object to get IObjectClass for</param>
        /// <returns>Return the IObjectClass associated with the specified IRow</returns>
        public static IObjectClass GetObjectClassFromRow(IRow row)
        {
            IObjectClass returnClass = null;
            try
            {
                returnClass = row.Table as IObjectClass;
                if (row.Table is RelQueryTable)
                {
                    IRelQueryTable relQueryTable = row.Table as IRelQueryTable;

                    if (relQueryTable != null && relQueryTable.SourceTable != null)
                    {
                        //For feeder manager 2.0 the source table will be the actual feature class
                        returnClass = relQueryTable.SourceTable as IObjectClass;
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLine_Error(ex.Message + "   " + ex.StackTrace);
            }
            return returnClass;
        }

        public static IObjectClass GetObjectClassFromObject(IObject obj)
        {
            IObjectClass returnClass = null;
            try
            {
                returnClass = obj.Class as IObjectClass;
                if (obj.Class is RelQueryTable)
                {
                    IRelQueryTable relQueryTable = obj.Class as IRelQueryTable;

                    if (relQueryTable != null && relQueryTable.SourceTable != null)
                    {
                        //For feeder manager 2.0 the source table will be the actual feature class
                        returnClass = relQueryTable.SourceTable as IObjectClass;
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLine_Error(ex.Message + "   " + ex.StackTrace);
            }
            return returnClass;
        }

        public static ITable GetTableFromRow(IRow row)
        {
            return GetObjectClassFromRow(row) as ITable;
        }

        public static IFeatureClass GetFeatureClassFromRow(IRow row)
        {
            return GetObjectClassFromRow(row) as IFeatureClass;
        }

        public static void WriteLine_Error(string sMsg)
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
    }
}
