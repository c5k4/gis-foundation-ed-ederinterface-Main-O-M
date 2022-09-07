using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Collections;

using Miner.Interop.Process;
using Miner.Interop;
using Miner.ComCategories;

using log4net;

using PGE.Common.Delivery.Process.BaseClasses;
using PGE.Common.Delivery.Process;
using System.Windows.Forms;

using System.Data;


namespace PGE.Common.Delivery.SessionManager.Subtasks
{
    /// <summary>
    /// Change the owner to a user that has permissions on the To State of the assigned transition, plus options for showing ALL USERS, setting default user.
    /// </summary>
    [ComVisible(true)]
    [Guid("CDB9446A-502C-49F0-B0A8-8B131BAA1152")]
    [ProgId("PGE.Common.SubTask.SMChangeOwnerByRole")]
    [ComponentCategory(ComCategory.MMPxSubtasks)]
    public class SMChangeOwnerByRole : BaseChangeOwnerByRole
    {
       
        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        public SMChangeOwnerByRole() : base(BasePxSubtask.SessionManagerExt, "Change Session Owner by Role")
        {

        }

        #endregion

        #region WMSChangeOwnerByRole Overrides
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pxNode"></param>
        /// <param name="newUser"></param>
        /// <param name="oldUser"></param>
        /// <returns></returns>
        public override bool ChangeOwnerOnNode(IMMPxNode pxNode, IMMPxUser newUser, ref IMMPxUser oldUser)
        {
            try
            {
                _logger.Debug("ENTER ChangeOwnerOnNode");

                IMMSessionManager2 sm2 = (IMMSessionManager2)_PxApp.FindPxExtensionByName(BasePxSubtask.SessionManagerExt);
                IMMSessionDBInfo smDBInfo = (IMMSessionDBInfo)sm2;
                
                // (M4JF) EDGISREARCH-378 - Session Manager Edit session limit (In-Progress Session will not be reassigned to selected user if user already have max In-Progress sessions allowed.)
                // get selected new Owner name and state id of session to be reassigned
                string newOwner = newUser.Name;
                string stateID = getSessionStatus(pxNode.Id.ToString());

                // (M4JF) EDGISREARCH-378 - Session Manager Edit session limit .
                //check if session is in In-Progress state
                if (stateID == "1")
                {
                    
                    int currentOwnerSessionCount = getCurrentOwnerSessionCount(newOwner);
                    int maxSessionAllowed = getMaxSessionLimit();
                    // (M4JF) EDGISREARCH-378 - Session Manager Edit session limit .
                    //check if selected current owner already have 5 or more In-Progress sessions ,if yes , then No more In-Progress sessions will be assigned to selected user.
                    if (currentOwnerSessionCount >= maxSessionAllowed)
                    {
                      
                        string messageToDisplay = getDisplaymessage();
                        // (M4JF) EDGISREARCH-378 - Session Manager Edit session limit .
                        //Display warning message .
                        MessageBox.Show(string.Format(messageToDisplay , maxSessionAllowed) , "Session Manager", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                }
                if (((_PxApp != null) && (sm2.CurrentOpenSession != null)))
                {
                    _logger.Debug("Close the Session if the current user is not the owner.");
                    //Close the design if the current user is not the owner.
                    if (_PxApp.User.Id != newUser.Id)
                    {

                        IMMPxTask closeTask = ((IMMPxNode3)pxNode).GetTaskByName("Close Session");

                        if (closeTask == null)
                        {
                            _logger.Debug("Close Session PxTask not found.");
                            return false;
                        }

                        _logger.Debug("Found Close Session Task.");
                        closeTask.Execute(pxNode);
                        //User may have cancelled the close session
                        if (sm2.CurrentOpenSession != null) { return false; }
                    }
                }

                //Attempt to switch ownership
                string sql = "SELECT * FROM " + smDBInfo.SchemaName + " MM_SESSION WHERE SESSION_ID=" + pxNode.Id;
                _logger.Debug("SELECT SQL:  " + sql);

                PxDb localPXDb = new PxDb(_PxApp);

                Session currentNode = new Session(_PxApp, pxNode.Id);
                oldUser = FindUser(currentNode.Owner);

                _logger.Debug("Updating Owner from " + oldUser.Name + " to: " + newUser.Name + ".");
                localPXDb.Update(sql, new object[] { "CURRENT_OWNER" }, new object[] { newUser.Name });

                int clearSessionID = -1;
                bool clearBool = true;
                _logger.Debug("Clearing Session cache.");
                ((IMMSessionManager)sm2).GetSession(ref clearSessionID, ref clearBool);

                //Rebuild the Session List
                IMMDynamicList dynoList = (IMMDynamicList)pxNode;

                object newUserName = newUser.Name;

                _logger.Debug("Rebuilding Node Tree.");
                dynoList.BuildObject.UpdateNode(ref newUserName, (ID8ListItem)pxNode);
            }
            catch (Exception exp)
            {
                _logger.Error("Error encountered: " + exp.Message, exp);
                return false;
            }

            _logger.Debug("ENTER ChangeOwnerOnNode");
            return true;
        }


        #endregion
        /// <summary>
        /// // (M4JF) EDGISREARCH-378 - Session Manager Edit session limit .
        /// Get Display Message for Re-Assign session limit.
        /// </summary>
        /// <returns></returns>
        public string getDisplaymessage()
        {

            // Initialize variables .
            string message = string.Empty;
            PxDb localPXDb = new PxDb(_PxApp);
            System.Data.DataTable dt = new System.Data.DataTable();
            string messageQuery = string.Empty;

            try
            {
                
                // Get display message string for Re-Assign session limit.
                messageQuery = "Select VALUE FROM PGEDATA.PGE_EDERCONFIG WHERE KEY = 'SESSION_MANAGER_RE-ASSIGN_SESSION_LIMIT_MESSAGE'";
                dt = localPXDb.ExecuteQuery(messageQuery);
                foreach (DataRow msg in dt.Rows)
                {
                    // Get message .
                    message = msg["VALUE"].ToString();

                }

                return message;

            }
            catch (Exception ex)
            {

                throw ex;

            }
            
        }

        // <summary>
        ///// (M4JF) EDGISREARCH-378 - Session Manager Edit session limit .
        /// Get Display Message for Re-Assign session limit.
        /// </summary>
        /// <returns></returns>
        public int getMaxSessionLimit()
        {

            // Initialize variables .
            int maxSessionCount = 0;
            System.Data.DataTable dt = new System.Data.DataTable();
            PxDb localPXDb = new PxDb(_PxApp);
            string Query = string.Empty;
            try
            {
                                           
                // Get display message string for Re-Assign session limit.
                Query = "Select VALUE FROM PGEDATA.PGE_EDERCONFIG WHERE KEY = 'SESSION_MANAGER_MAX_INPROGRESS_SESSIONCOUNT'";
                dt = localPXDb.ExecuteQuery(Query);
                foreach (DataRow dr in dt.Rows)
                {
                    maxSessionCount = Convert.ToInt32(dr["VALUE"]);
                }

                return maxSessionCount;

            }
            catch (Exception ex)
            {

                throw ex;

            }
            

        }

        /// <summary>
        /// // (M4JF) EDGISREARCH-378 - Session Manager Edit session limit .
        /// Get In-Progress session Count of selected Current Owner .
        /// </summary>
        /// <param name="newOwner">Selected New Owner name for reassigning session.</param>
        /// <returns></returns>
        public int getCurrentOwnerSessionCount(string newOwner)
        {

            // Initialize variables .
            int sessioncount = 0;
            PxDb localPXDb = new PxDb(_PxApp);
            System.Data.DataTable dt = new System.Data.DataTable();
            string sqlQuery = string.Empty;

            try
            {

                // Query to get In-progress sessions with selected user for Re-Assign.
                sqlQuery = "Select s.SESSION_ID from process.mm_session s, process.mm_px_current_state st where s.CURRENT_OWNER = '" + newOwner + "' AND s.SESSION_ID = st.SOID AND st.STATE_ID = '1'";
                dt = localPXDb.ExecuteQuery(sqlQuery);
                // Get session Count to return .
                sessioncount = dt.Rows.Count;
                return sessioncount;

            }
            catch (Exception ex)
            {

                throw ex;

            }
           
        }
        /// <summary>
        /// // (M4JF) EDGISREARCH-378 - Session Manager Edit session limit .
        /// Returns status of selected session for Re-Assign
        /// </summary>
        /// <param name="sessionID">Session Id to get session Status .</param>
        /// <returns></returns>

        public string getSessionStatus(string sessionID)
        {
            // Initialize variables .
            string status = string.Empty;
            PxDb localPXDb = new PxDb(_PxApp);
            System.Data.DataTable dt = new System.Data.DataTable();
            string sql = string.Empty;
            try
            {
                
                // Get session status of selected session for re-Assign .
                sql = "Select st.STATE_ID  FROM process.mm_px_current_state st where st.SOID = '" + sessionID + "'";
                dt = localPXDb.ExecuteQuery(sql);
                foreach (DataRow statusID in dt.Rows)
                {
                    // Get Status .
                    status = statusID["STATE_ID"].ToString();

                }

                return status;

            }
            catch (Exception ex)
            {

                throw ex;

            }
            
        }
       
        #region BasePxSubtask Overrides
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pPxNode"></param>
        /// <returns></returns>
        protected override bool InternalEnabled(IMMPxNode pPxNode)
        {
            return true;
        }

        #endregion
    }
}



