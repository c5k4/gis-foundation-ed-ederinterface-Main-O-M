using System;

using Miner.Geodatabase.GeodatabaseManager;
using Miner.Geodatabase.GeodatabaseManager.Serialization;
using Miner.Geodatabase.GeodatabaseManager.ActionHandlers;
//using Miner.Process.GeodatabaseManager;
//using Miner.Process.GeodatabaseManager.ActionHandlers;


namespace PGE.Interfaces.SAP.GDBM.ActionHandlers
{
    /// <summary>
    /// Persists the information collected by SAP Data Collector action handler to staging table.
    /// </summary>
    public class DataPersistor : ActionHandler
    {
        //private static ILog _logger = LogManager.GetLogger("MinerGdbManager", MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Constructor. ActionHandler's Name and Description are set.
        /// </summary>
        public DataPersistor()
            : base()
        {
            this.Name = "SAP Data Persistor";
            this.Description = "Persists the information collected by SAP Data Collector action handler to staging table";
        }

        /// <summary>
        /// Write data passed via ActionData into a table.
        /// </summary>
        /// <param name="actionData">Pass data between ActionHandlers</param>
        /// <param name="parameters"></param>
        /// <returns>true if executed successfully, or false</returns>
        protected override bool SubExecute(ActionData actionData, GdbmParameter[] parameters)
        {
            Log.Info(ServiceConfiguration.Name, string.Format("Persisting to the staging table for Session: {0}", actionData.VersionName));
            bool success = false;

            //Dictionary<string, Dictionary<string, IRowData>> assetIDandDataByClass = (Dictionary<string, Dictionary<string, IRowData>>)actionData["SAPData"];

            using (PGE.Interfaces.SAP.AssetProcessor assetProcesor = new PGE.Interfaces.SAP.AssetProcessor())
            {
                try
                {
                    //start transaction
                    assetProcesor.BeginTransaction();

                    success = assetProcesor.Process(actionData.VersionName);

                    if (success)
                    {
                        Log.Debug(ServiceConfiguration.Name, string.Format("Session: {0} processed successfully.", actionData.VersionName));
                    }
                    else
                    {
                        Log.Error(ServiceConfiguration.Name, string.Format("Session: {0} not processed successfully. ", actionData.VersionName));
                        Log.Error(ServiceConfiguration.Name, string.Format("Generating script for the session: {0}. ", actionData.VersionName));

                        try
                        {
                            assetProcesor.GenerateScript(actionData.VersionName);
                        }
                        catch (Exception e)
                        {
                            Log.Fatal(ServiceConfiguration.Name, "Failed to generate script after failing to process session.", e);
                            assetProcesor.Rollback();
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ServiceConfiguration.Name, string.Format("Failed to process session: {0}.", actionData.VersionName), ex);
                    Log.Error(ServiceConfiguration.Name, string.Format("Generating script for the session: {0}. ", actionData.VersionName));

                    try
                    {
                        assetProcesor.GenerateScript(actionData.VersionName);
                    }
                    catch (Exception e)
                    {
                        Log.Fatal(ServiceConfiguration.Name, "Failed to generate script after failing to process the session.", e);
                        assetProcesor.Rollback();
                        return false;
                    }
                }

                //Commit transaction
                try
                {
                    assetProcesor.Commit();
                }
                catch (Exception ex)
                {
                    Log.Fatal(ServiceConfiguration.Name, string.Format("Failed to commit transaction into staging table for session: {0} .", actionData.VersionName), ex);
                    return false;
                }
            }

            GC.Collect();

            // TEMP for testing
            //return false;
            return true;
        }

        /// <summary>
        /// Enable your action handler for certain situations, e.g. AfterPost.
        /// </summary>
        /// <param name="actionType"></param>
        /// <param name="gdbmAction"></param>
        /// <param name="versionType"></param>
        /// <returns></returns>
        
        public override bool Enabled(Miner.Geodatabase.GeodatabaseManager.ActionType actionType, Actions gdbmAction, PostVersionType versionType)
        {
            // enabled AfterPost
            bool enabled = false;
            if (gdbmAction == Actions.AfterPost || gdbmAction == Actions.Post)
            {
                enabled = true;
            }

            return enabled;
        }
    }
}
