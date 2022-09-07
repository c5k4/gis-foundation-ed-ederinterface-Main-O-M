// ========================================================================
// Copyright © 2021 PGE.
// <history>
// Session Manager - Edit Session Limit subtask.
// TCS M4JF 07/04/2021 Created
// </history>
// All rights reserved.
// ========================================================================
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using Miner.ComCategories;
using Miner.Interop.Process;
using ESRI.ArcGIS.Framework;
using System.Windows.Forms;
using PGE.Common.Delivery.Process;
using System.Data;


namespace PGE.Desktop.EDER.Subtasks
{
    /// <summary>
    /// Subtask to restrict user from creating new sessions if user already have 5 or more In-Progress sessions.
    /// (M4JF)EDGISREARCH-378 - Session Manager - Edit Session Limit subtask.
    /// </summary>
    /// 
    [ComVisible(true)]
    [Guid("36541FF1-BE8A-4304-8733-8520A33CFCFF")]   
    [ProgId("PGE.Desktop.EDER.Subtasks.EditSessionLimitSubtask")]
    [ComponentCategory(ComCategory.MMPxSubtasks)]
      
    public class EditSessionLimitSubtask : IMMPxSubtask
    {
        protected static PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "");
        private static IApplication _app;
        private static IMMPxApplication _PxApp;       
        

        #region Component Registration

        /// <summary>
        /// Registers the specified reg key.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComRegisterFunction, ComVisible(false)]
        static void Register(string regKey)
        {
            Miner.ComCategories.MMPxSubtasks.Register(regKey);
        }

        /// <summary>
        /// Unregisters the specified reg key.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComUnregisterFunction, ComVisible(false)]
        static void Unregister(string regKey)
        {
            Miner.ComCategories.MMPxSubtasks.Unregister(regKey);
        }
        #endregion

        public bool Enabled(IMMPxNode pPxNode)
        {
            return true;
        }

        
        public bool Execute(IMMPxNode pPxNode) 
        {

            // Initialize variables for reading data from config table(PGEDATA.PGE_EDERCONFIG)

            int maxSessionsAllowed = 0;
            int sessioncount = 0;
            string message = string.Empty;
            PxDb localPXDb = new PxDb(_PxApp);
            string userId = string.Empty;
            string max_SessionCount_query = string.Empty;
            string message_query = string.Empty;
            string sqlQuery = string.Empty;
            System.Data.DataTable messageDT = new System.Data.DataTable();
            System.Data.DataTable sessionDetailsDT = new System.Data.DataTable();
            System.Data.DataTable max_session_countDT = new System.Data.DataTable();

            try
            {

                userId = _PxApp.User.Name;
                // query to get max no. of sessions allowed .
                max_SessionCount_query = "Select VALUE FROM " + SchemaInfo.General.pEDERCONFIGTABLE + " WHERE KEY = 'SESSION_MANAGER_MAX_INPROGRESS_SESSIONCOUNT'";
                max_session_countDT = localPXDb.ExecuteQuery(max_SessionCount_query);
                foreach (DataRow dr in max_session_countDT.Rows)
                {
                    maxSessionsAllowed = Convert.ToInt32(dr["VALUE"]);
                }
                //query to get Create session Limit Message .
                message_query = "Select VALUE FROM " + SchemaInfo.General.pEDERCONFIGTABLE + " WHERE KEY = 'SESSION_MANAGER_CREATE_SESSION_LIMIT_MESSAGE'";
                messageDT = localPXDb.ExecuteQuery(message_query);

                foreach (DataRow dr in messageDT.Rows)
                {
                    //Get message to display.
                    message = dr["VALUE"].ToString();
                }

                //query to check if user already hve 5 or more In-Progress sessions .
                sqlQuery = "Select s.SESSION_ID from process.mm_session s, process.mm_px_current_state st where s.CURRENT_OWNER = '" + userId + "' AND s.SESSION_ID = st.SOID AND st.STATE_ID = '1' AND s.HIDDEN = '0'";
                sessionDetailsDT = localPXDb.ExecuteQuery(sqlQuery);
                sessioncount = sessionDetailsDT.Rows.Count;

                // If user already have 5 or more in-Progress sessions then user will be restricted from creating any new sessions.
                if (sessioncount >= maxSessionsAllowed)
                {
                    // Display warning messgae .
                    MessageBox.Show(string.Format(message, maxSessionsAllowed), "Session Manager", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }



            }
            catch (Exception ex)
            {
               
                _logger.Error(ex.ToString()); 

            }

            return true;
        }

       /// <summary>
       /// Initialize Application .
       /// </summary>
       /// <param name="pPxApp"></param>
       /// <returns></returns>
       
        public bool Initialize(IMMPxApplication pPxApp)
        {
            _PxApp = pPxApp;
            if (_PxApp != null)
            {
                return true;
            }
            else return false;
        }
        /// <summary>
        /// Defines Subatask Name .
        /// </summary>
        public string Name
        {           
            get { return "Edit Session Limit SubTask"; }
        }

        public bool Rollback(IMMPxNode pPxNode)
        {
           throw new NotImplementedException();
        }
    }
}
