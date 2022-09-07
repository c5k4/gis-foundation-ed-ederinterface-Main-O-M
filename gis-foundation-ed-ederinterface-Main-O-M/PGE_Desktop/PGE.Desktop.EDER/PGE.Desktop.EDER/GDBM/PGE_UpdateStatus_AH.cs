// ========================================================================
// Copyright © 2021 PGE 
// <history>
// Update data in Staging Area for further processing of batch server to Update SSD information,Device Information
// Jet Operating Number , update Circuit Color ,MapProduction and DMS
// YXA6 4/14/2021	Created
// </history>
// All rights reserved.
// ========================================================================

using ESRI.ArcGIS;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.GeoDatabaseDistributed;
using Miner.Geodatabase.GeodatabaseManager;
using Miner.Geodatabase.GeodatabaseManager.Serialization;
using Miner.Process.GeodatabaseManager;
using Miner.Process.GeodatabaseManager.ActionHandlers;
using PGE.Common.Delivery.Framework.FeederManager;
using PGE.Common.Delivery.Geodatabase.ChangeDetection;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace PGE.Desktop.EDER.GDBM
{
    public class PGE_UpdateStatus_AH : PxActionHandler
    {
        public PGE_UpdateStatus_AH()
        {
            this.Name = "PGE Update Posted Status";
            this.Description = "Updates status as 'P' for those session which are POSTED through GDBM.";
           
        }
        #region Override Functions
        /// <summary>
        /// Apply teh Action handler before post on data Processsing Queue
        /// </summary>
        /// <param name="actionType"></param>
        /// <param name="gdbmAction"></param>
        /// <param name="versionType"></param>
        /// <returns></returns>
        public override bool Enabled(ActionType actionType, Actions gdbmAction, PostVersionType versionType)
        {
           
            if (gdbmAction == Miner.Geodatabase.GeodatabaseManager.Actions.AfterPost)
            {
                
                return true;
            }
            else
            {
                return false;
            }    

        }
         /// <summary>
        /// Upadte the Staging Area(PGEDATA.PGE_AH_Info and PGEData.PGE_CH_Info)
        /// </summary>
        /// <param name="actionData"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected override bool PxSubExecute(PxActionData actionData, GdbmParameter[] parameters)
        {
           

            if (ExecuteActionHandler(actionData.Version) == true) 
            {
                return true;
            }
            else 
            {

                return false;
            }

        }
        #endregion

        #region Internal Methods
        /// <summary>
        /// Execute Action Handler to update the staging area
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        private bool ExecuteActionHandler(IVersion version)
        {

            bool success = false;
           
            try
            {
                
               (new DBHelper()).UpdateStatusWithPostedTime(version.VersionName.ToString());
            }
            catch (Exception ex)
            {
                this.Log.Error(ServiceConfiguration.Name, "Error during Updating the Staging table status as  'P': " + ex.Message + " StackTrace: " + ex.StackTrace);
                

                
            }
           
            return success;
        }

     
        #endregion
    }
}
