using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Collections;
using System.Windows.Forms;

using log4net;

using Miner.Interop.Process;
using Miner.Interop;
using Miner.ComCategories;
using PGE.Common.Delivery.Process.Forms;
using PGE.Common.Delivery.Process.BaseClasses;
using PGE.Common.Delivery.Process;

namespace PGE.Common.Delivery.Process.BaseClasses
{
    /// <summary>
    /// Change the owner to a user that has permissions on the To State of the assigned transition, plus options for showing ALL USERS, setting default user.
    /// </summary>
    [ComVisible(true)]
    public class BaseChangeOwnerByRole : BasePxSubtask
    {

        #region Private Fields

        private string _defaultUserKey;
        private string _allUsersYesNoKey;
        private string _targetStateOnlyYesNoKey;
        private string _specificRolesKey;

        #endregion

        #region Constructors / Destructors
        /// <summary>
        /// 
        /// </summary>
        /// <param name="supportedExtension"></param>
        /// <param name="subtaskName"></param>
        public BaseChangeOwnerByRole(string supportedExtension, string subtaskName) : base(subtaskName)
        {
            //Add Extenstions
            AddExtension(supportedExtension);

            //Add Parameters
            _defaultUserKey = "DefaultUser";
            string stateParamDesc = "Enter a default User Name or Display Name from Px Admin Tool.";
            AddParameter(_defaultUserKey, stateParamDesc);

            _allUsersYesNoKey = "All Users?";
            string allUsersYesNoParamDesc = "Choose one of the following: YES | NO";
            AddParameter(_allUsersYesNoKey, allUsersYesNoParamDesc);

            _targetStateOnlyYesNoKey = "Transition To State Only?";
            string targetStateOnlyYesNoParamDesc = "Choose one of the following: YES | NO";
            AddParameter(_targetStateOnlyYesNoKey, targetStateOnlyYesNoParamDesc);

            _specificRolesKey = "Specific Role(s)?";
            string specificRoleOnlyYesNoParamDesc = "Enter a role(s) that will be allowed to accept ownership.";
            AddParameter(_specificRolesKey, specificRoleOnlyYesNoParamDesc);
        }

        #endregion

        #region BasePxSubtask Overrides
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pPxNode"></param>
        /// <returns></returns>
        protected override bool InternalExecute(IMMPxNode pPxNode)
        {
            _logger.Debug("ENTER BaseChangeOwnwerByRole.InternalExecute");

            try
            {

                //Get the default user if the parameter has been set.
                string defaultUser = GetParameter(_defaultUserKey);
                bool allUsers = (GetParameter(_allUsersYesNoKey) == "YES") ? true : false;
                bool targetStateOnly = (GetParameter(_targetStateOnlyYesNoKey) == "YES") ? true : false;
                ArrayList specificRoles = GetListOfValuesFromParameter(GetParameter(_specificRolesKey));

                IMMPxUser defaultPxUser = null;
                Dictionary<string, IMMPxUser> usersToChooseFrom = null;

                DateTime getPxUsersStart = DateTime.Now;
                _logger.Debug("Begin checking PxUsers at " + getPxUsersStart.TimeOfDay.ToString());

                if ((defaultUser != null) && (defaultUser.Length > 0))
                {
                    defaultPxUser = FindUser(defaultUser);
                    _logger.Debug("Default User found: " + defaultPxUser.Name);
                    if (defaultPxUser == null)
                    {
                        return false;
                    }
                }
                else
                {
                    //Find the current task and transition
                    IMMPxTask pxExecutingTask = base.ExecutingTask;
                    IMMPxState pxTargetState = GetTargetState();

                    #region ME Q4 Release Item
                    //ME Q4 Release Item
                    // Task "PGE Assign Multiple Sessions" from All Session execution to assign multiple Sesions to one user
                    if ((pxExecutingTask.Name).Equals("PGE Assign Multiple All Sessions"))
                    {
                        Cursor.Current = Cursors.WaitCursor;
                        IMMPxNode pPxNodeCopy = null;
                        pPxNodeCopy = pPxNode;
                        IMMPxNode _pxNode = null;
                        PxDb _pxDb = new PxDb(_PxApp);
                        //get current owner of _pxApp
                        string currentUser = _PxApp.User.Name;
                        System.Data.DataTable dtSessions = new System.Data.DataTable();
                        string session_Id = @"""Session ID""";
                        string sessionName = @"""Session Name""";
                        string CreateDate = @"""Create Date""";
                        string createdUser = @"""Created User""";
                        string curOwner = @"""Current Owner""";
                        string Sessiondescription = @"""Description""";
                        string sessionStatus = @"""Status""";
                        //string sqlQuery = "select SESSION_ID " + session_Id + ",SESSION_NAME " + sessionName + ",Description " +Sessiondescription+ " ,create_date " + CreateDate + " from PROCESS.MM_SESSION where CURRENT_OWNER='" + currentUser + "'";
                        //string sqlQuery = "select s.SESSION_ID " + session_Id + ",s.SESSION_NAME " + sessionName + ", s.Description " + Sessiondescription + " ,s.create_date " + CreateDate + ", sta.name " + sessionStatus + " from process.mm_session s, process.mm_px_current_state st, process.mm_px_state sta where s.SESSION_ID= st.SOID and st.STATE_ID= sta.STATE_ID and s.CURRENT_OWNER='" + currentUser + "'";
                        string sqlQuery = "select s.SESSION_ID " + session_Id + ",s.SESSION_NAME " + sessionName + ", s.Description " + Sessiondescription + ", s.CREATE_USER " + createdUser + " , s.CURRENT_OWNER " + curOwner + " ,s.create_date " + CreateDate + ", sta.name " + sessionStatus + " from process.mm_session s, process.mm_px_current_state st, process.mm_px_state sta where s.SESSION_ID= st.SOID and st.STATE_ID= sta.STATE_ID";
                        dtSessions = _pxDb.ExecuteQuery(sqlQuery);
                        if (dtSessions.Rows.Count != 0)
                        {
                            //Initialize the AssignMent form
                            Dictionary<string, IMMPxUser> userList = FindUsers();
                            // Removing Current user from userlist
                            //userList.Remove(currentUser);   -- Commenting After UAT Feedback
                            MoveSessions assignForm = new MoveSessions(dtSessions, userList);

                            if (assignForm.ShowDialog(DesktopWindow.Instance) == DialogResult.OK)
                            {
                                //session assignement start here
                                int newOwnerID;
                                int sessionId;
                                newOwnerID = assignForm.SelectedUser.Id;
                                IMMPxUser oldUser1 = null;
                                if (!(assignForm.SelectedUser == null || assignForm.selectedRows.Count == 0))
                                {
                                    foreach (string session in assignForm.selectedRows)
                                    {
                                        sessionId = Convert.ToInt32(session);
                                        // getting pxNode by session ID
                                        AssignSession node = new AssignSession(_PxApp, sessionId);
                                        _pxNode = node.PxNode;
                                        if (ChangeOwnerOnNode(_pxNode, assignForm.SelectedUser, ref oldUser1) == false)
                                        {
                                            bool rebuildSessionList = false;
                                            try
                                            {

                                                object newUserName = assignForm.SelectedUser.Name;
                                                //Rebuild the Session List
                                                IMMDynamicList dynoList = (IMMDynamicList)pPxNode;
                                                MMPxNodeListClass _pxNodeList = (MMPxNodeListClass)_pxNode;
                                                _pxNodeList.BuildObject = dynoList.BuildObject;
                                                _pxNode = (IMMPxNode)_pxNodeList;
                                                _logger.Debug("Rebuilding Node Tree.");
                                                dynoList.BuildObject.UpdateNode(ref newUserName, (ID8ListItem)_pxNode);

                                                // Extra Data for session history
                                                string extraData1 = "<NEW_OWNER>" + ((IMMPxUser2)assignForm.SelectedUser).DisplayName + "</NEW_OWNER>";
                                                extraData1 = extraData1 + "<NEW_OWNER_ID>" + newOwnerID.ToString() + "</NEW_OWNER_ID>";

                                                // Adding history in PxHistory
                                                _logger.Debug("Adding entry in PxHistory.");
                                                AddHistory(_pxNode, "Owner changed from " + ((IMMPxUser2)oldUser1).DisplayName + " to " + ((IMMPxUser2)assignForm.SelectedUser).DisplayName, extraData1);

                                                _logger.Debug("Assignment history added");
                                                rebuildSessionList = true;

                                            }
                                            catch (Exception exp)
                                            {
                                                _logger.Error("Error encountered: " + exp.Message, exp);
                                                rebuildSessionList = false;
                                            }
                                            if (!rebuildSessionList)
                                            {
                                                MessageBox.Show("Unable to change owner to: " + assignForm.SelectedUser.Name + ". Please see an Administrator.");
                                                _logger.Error("Unable to change owner to: " + assignForm.SelectedUser.Name + ". Please see an Administrator.");
                                                return false;
                                            }
                                        }

                                        else
                                        {
                                            string extraData1 = "<NEW_OWNER>" + ((IMMPxUser2)assignForm.SelectedUser).DisplayName + "</NEW_OWNER>";
                                            extraData1 = extraData1 + "<NEW_OWNER_ID>" + newOwnerID.ToString() + "</NEW_OWNER_ID>";

                                            // Adding history in PxHistory
                                            _logger.Debug("Adding entry in PxHistory.");
                                            AddHistory(_pxNode, "Owner changed from " + ((IMMPxUser2)oldUser1).DisplayName + " to " + ((IMMPxUser2)assignForm.SelectedUser).DisplayName, extraData1);

                                            _logger.Debug("EXIT BaseChangeOwnwerByRole.InternalExecute");

                                            return true;
                                        }
                                    }

                                    _PxApp.Show();   // to refresh PxApplication
                                    _logger.Debug("_PxApp is refreshed");
                                    _logger.Debug("Session Assignmnet done: ");
                                    return true;
                                }
                            }
                            else
                            {
                                _logger.Debug("User Cancelled the Select New dialog.");
                                return false;
                            }
                        }
                        else
                        {
                            MessageBox.Show("Currently you have not any session to assign");
                            return false;
                        }
                    }

                    // Assign Multiple sessions from My Session filter
                    if ((pxExecutingTask.Name).Equals("PGE Assign Multiple Sessions"))
                    //if ((pxExecutingTask.Name).Equals("PGE Assign Multiple All Sessions"))
                    {
                        IMMPxNode pPxNodeCopy = null;
                        pPxNodeCopy = pPxNode;
                        IMMPxNode _pxNode = null;
                        PxDb _pxDb = new PxDb(_PxApp);
                        //get current owner of _pxApp
                        string currentUser = _PxApp.User.Name;
                        System.Data.DataTable dtSessions = new System.Data.DataTable();
                        string session_Id = @"""Session ID""";
                        string sessionName = @"""Session Name""";
                        string CreateDate = @"""Create Date""";
                        string Sessiondescription = @"""Description""";
                        string createdUser = @"""Created User""";
                        string curOwner = @"""Current Owner""";
                        string sessionStatus = @"""Status""";
                        //string sqlQuery = "select SESSION_ID " + session_Id + ",SESSION_NAME " + sessionName + ",Description " +Sessiondescription+ " ,create_date " + CreateDate + " from PROCESS.MM_SESSION where CURRENT_OWNER='" + currentUser + "'";
                        //string sqlQuery = "select s.SESSION_ID " + session_Id + ",s.SESSION_NAME " + sessionName + ", s.Description " + Sessiondescription + " ,s.create_date " + CreateDate + ", sta.name " + sessionStatus + " from process.mm_session s, process.mm_px_current_state st, process.mm_px_state sta where s.SESSION_ID= st.SOID and st.STATE_ID= sta.STATE_ID and s.CURRENT_OWNER='" + currentUser + "'";
                        string sqlQuery = "select s.SESSION_ID " + session_Id + ",s.SESSION_NAME " + sessionName + ", s.Description " + Sessiondescription + ", s.CREATE_USER " + createdUser + " , s.CURRENT_OWNER " + curOwner + " ,s.create_date " + CreateDate + ", sta.name " + sessionStatus + " from process.mm_session s, process.mm_px_current_state st, process.mm_px_state sta where s.SESSION_ID= st.SOID and st.STATE_ID= sta.STATE_ID and s.CURRENT_OWNER='" + currentUser + "'";
                        dtSessions = _pxDb.ExecuteQuery(sqlQuery);
                        if (dtSessions.Rows.Count != 0)
                        {
                            //Initialize the AssignMent form
                            Dictionary<string, IMMPxUser> userList = FindUsers();
                            // Removing Current user from userlist
                            userList.Remove(currentUser);   //-- Commenting After UAT Feedback
                            MoveSessions assignForm = new MoveSessions(dtSessions, userList);

                            if (assignForm.ShowDialog(DesktopWindow.Instance) == DialogResult.OK)
                            {
                                //session assignement start here
                                int newOwnerID;
                                int sessionId;
                                newOwnerID = assignForm.SelectedUser.Id;
                                IMMPxUser oldUser1 = null;
                                if (!(assignForm.SelectedUser == null || assignForm.selectedRows.Count == 0))
                                {
                                    foreach (string session in assignForm.selectedRows)
                                    {
                                        sessionId = Convert.ToInt32(session);
                                        // getting pxNode by session ID
                                        AssignSession node = new AssignSession(_PxApp, sessionId);
                                        _pxNode = node.PxNode;
                                        if (ChangeOwnerOnNode(_pxNode, assignForm.SelectedUser, ref oldUser1) == false)
                                        {
                                            bool rebuildSessionList = false;
                                            try
                                            {

                                                object newUserName = assignForm.SelectedUser.Name;
                                                //Rebuild the Session List
                                                IMMDynamicList dynoList = (IMMDynamicList)pPxNode;
                                                MMPxNodeListClass _pxNodeList = (MMPxNodeListClass)_pxNode;
                                                _pxNodeList.BuildObject = dynoList.BuildObject;
                                                _pxNode = (IMMPxNode)_pxNodeList;
                                                _logger.Debug("Rebuilding Node Tree.");
                                                dynoList.BuildObject.UpdateNode(ref newUserName, (ID8ListItem)_pxNode);

                                                // Extra Data for session history
                                                string extraData1 = "<NEW_OWNER>" + ((IMMPxUser2)assignForm.SelectedUser).DisplayName + "</NEW_OWNER>";
                                                extraData1 = extraData1 + "<NEW_OWNER_ID>" + newOwnerID.ToString() + "</NEW_OWNER_ID>";

                                                // Adding history in PxHistory
                                                _logger.Debug("Adding entry in PxHistory.");
                                                AddHistory(_pxNode, "Owner changed from " + ((IMMPxUser2)oldUser1).DisplayName + " to " + ((IMMPxUser2)assignForm.SelectedUser).DisplayName, extraData1);

                                                _logger.Debug("Assignment history added");
                                                rebuildSessionList = true;

                                            }
                                            catch (Exception exp)
                                            {
                                                _logger.Error("Error encountered: " + exp.Message, exp);
                                                rebuildSessionList = false;
                                            }
                                            if (!rebuildSessionList)
                                            {
                                                MessageBox.Show("Unable to change owner to: " + assignForm.SelectedUser.Name + ". Please see an Administrator.");
                                                _logger.Error("Unable to change owner to: " + assignForm.SelectedUser.Name + ". Please see an Administrator.");
                                                return false;
                                            }
                                        }

                                        else
                                        {
                                            string extraData1 = "<NEW_OWNER>" + ((IMMPxUser2)assignForm.SelectedUser).DisplayName + "</NEW_OWNER>";
                                            extraData1 = extraData1 + "<NEW_OWNER_ID>" + newOwnerID.ToString() + "</NEW_OWNER_ID>";

                                            // Adding history in PxHistory
                                            _logger.Debug("Adding entry in PxHistory.");
                                            AddHistory(_pxNode, "Owner changed from " + ((IMMPxUser2)oldUser1).DisplayName + " to " + ((IMMPxUser2)assignForm.SelectedUser).DisplayName, extraData1);

                                            _logger.Debug("EXIT BaseChangeOwnwerByRole.InternalExecute");

                                            return true;
                                        }
                                    }

                                    _PxApp.Show();   // to refresh PxApplication
                                    _logger.Debug("_PxApp is refreshed");
                                    _logger.Debug("Session Assignmnet done: ");
                                    return true;
                                }
                            }
                            else
                            {
                                _logger.Debug("User Cancelled the Select New dialog.");
                                return false;
                            }
                        }
                        else
                        {
                            MessageBox.Show("Currently you have not any session to assign");
                            return false;
                        }
                    }
                    #endregion
                    //  ### End of ME Q4 Release Item

                    if (allUsers)
                    {
                        //Find all users if parameter allusers is YES
                        usersToChooseFrom = FindUsers();
                    }
                    else if ((targetStateOnly) && (specificRoles == null))
                    {
                        //Find all users who have permission on the TARGET state,
                        usersToChooseFrom = FindUsers(pxTargetState, null, false);
                    }
                    else if ((!targetStateOnly) && (specificRoles.Count >= 1))
                    {
                        //Or, users who are of a specific Roles,
                        usersToChooseFrom = FindUsers(null, specificRoles, false);
                    }
                    else if ((targetStateOnly) && (specificRoles.Count >= 1))
                    {
                        //Or, users who have permissions on the TARGET state AND 
                        //have a specific Role that is assigned to the target state.
                        usersToChooseFrom = FindUsers(pxTargetState, specificRoles, false);
                    }
                }

                //If there are no users get the users on the CURRENT state
                if (usersToChooseFrom.Count == 0)
                {
                    _logger.Warn("No users with paramter criteria found.  Getting PxUsers with roles on Current PxState: " + pPxNode.State.Name);
                    usersToChooseFrom = FindUsers(pPxNode.State, null, false);
                }

                if (usersToChooseFrom.Count == 0)
                {
                    _logger.Error("No users found. Check subtask configuration.");
                    return false;
                }

                DateTime getPxUsersEnd = DateTime.Now;
                TimeSpan totaltime = getPxUsersStart.Subtract(getPxUsersEnd);
                _logger.Debug("End checking PxUsers at " + getPxUsersEnd.TimeOfDay.ToString() + "TOTAL TIME: " + totaltime.ToString());

                IMMPxUser targetUser = null;
                int targetUserID;

                DialogResult dr = DialogResult.Ignore;
                if ((defaultUser != null) && (usersToChooseFrom == null))
                {
                    //Use the default user
                    targetUser = defaultPxUser;
                    targetUserID = defaultPxUser.Id;
                    _logger.Debug("Default User " + targetUser.Name + " chosen as specified in subtask parameter: All Users?.");
                }
                else if ((defaultUser != null) && (usersToChooseFrom.ContainsValue(defaultPxUser)))
                {
                    //Use the default user
                    targetUser = defaultPxUser;
                    targetUserID = defaultPxUser.Id;
                    _logger.Debug("Default User " + targetUser.Name + " chosen as specified in subtask parameters: All Users? AND other parameters.");
                }
                else
                {
                    //Ask the user to specify the new owner.
                    string prompt = "";

                    _logger.Debug("Current user must choose new owner.");
                    targetUser = PromptForUser(usersToChooseFrom, prompt, out dr);
                    _logger.Debug("New owner is: " + targetUser.Name);
                }

                //If there is no TargetUser or if the user cancelled the prompt, return false
                if ((targetUser == null) && (dr == DialogResult.OK))
                {
                    MessageBox.Show("You must choose a user in the list.");
                    _logger.Debug("No user chosen exiting.");
                    return false;
                }
                else if (dr == DialogResult.Cancel)
                {
                    _logger.Debug("Curren tuser cancelled out of Select New Owner dialog.");
                    return false;
                }

                _logger.Debug("Attempting to change owner.");
                targetUserID = targetUser.Id;
                IMMPxUser oldUser = null;
                if (ChangeOwnerOnNode(pPxNode, targetUser, ref oldUser) == false)
                {
                    MessageBox.Show("Unable to change owner to: " + targetUser.Name + ". Please see an Administrator.");
                    _logger.Error("Unable to change owner to: " + targetUser.Name + ". Please see an Administrator.");
                    return false;
                }

                string extraData = "<NEW_OWNER>" + ((IMMPxUser2)targetUser).DisplayName + "</NEW_OWNER>";
                extraData = extraData + "<NEW_OWNER_ID>" + targetUserID.ToString() + "</NEW_OWNER_ID>";

                _logger.Debug("Adding entry in PxHistory.");
                AddHistory(pPxNode, "Owner changed from " + ((IMMPxUser2)oldUser).DisplayName + " to " + ((IMMPxUser2)targetUser).DisplayName, extraData);

                _logger.Debug("EXIT BaseChangeOwnwerByRole.InternalExecute");

                return true;

            }
            catch (Exception exp)
            {
                Cursor.Current = Cursors.Default;
                _logger.Error("Error encountered: " + exp.Message, exp);
                return false;
            }
            finally { Cursor.Current = Cursors.Default; }

        }


        #endregion

        #region Protected Abstract Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pxNode"></param>
        /// <param name="newUser"></param>
        /// <param name="oldUser"></param>
        /// <returns></returns>
        public virtual bool ChangeOwnerOnNode(IMMPxNode pxNode, IMMPxUser newUser, ref IMMPxUser oldUser)
        {
            return true;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Returns the target state of a transition assigned to the executing PxTask.
        /// </summary>
        /// <returns>To State of the PxTransition.</returns>
        private IMMPxState GetTargetState()
        {
            _logger.Debug("ENTER GetTargetState");

            IMMPxState retVal = null;

            IMMPxTask pxExecutingTask = base.ExecutingTask;
            _logger.Debug("ExecutingTask: " + pxExecutingTask.Name);

            if (((IMMPxTask2)pxExecutingTask).Transition == null) 
            {
                _logger.Debug("No Transition assigned to ExecutingTask.");
                return retVal; 
            }

            IMMPxTransition pxExecutingTransition = ((IMMPxTask2)pxExecutingTask).Transition;
            _logger.Debug("Transition found for Executing Task: " + pxExecutingTransition.Name);

            IMMEnumPxState pxTargetStates = pxExecutingTransition.ToStates;
            pxTargetStates.Reset();

            retVal = pxTargetStates.Next();
            _logger.Debug("Target State found: " + retVal.Name);

            _logger.Debug("EXIT GetTargetState");
            return retVal;
            
        }

        /// <summary>
        /// Dialog that presents users with a list of Px Users to choose from.
        /// </summary>
        /// <param name="usersToChooseFrom">Filtered dictionary of Px Users.</param>
        /// <param name="prompt">Additional text for the caption of the form.</param>
        /// <param name="promptDR">Resulting action of the user.</param>
        /// <returns></returns>
        private IMMPxUser PromptForUser(Dictionary<string, IMMPxUser> usersToChooseFrom, string prompt, out DialogResult promptDR)
        {
            _logger.Debug("ENTER PromptForUser");

            IMMPxUser retVal = null;

            if ((usersToChooseFrom == null) || (usersToChooseFrom.Count == 0)) 
            { 
                promptDR = DialogResult.Cancel;
                _logger.Error("No Users found.  Check parameters of Subtask or assign a transition to the Task.");
                return retVal; 
            }

            ChooseUser selectUser = new ChooseUser(usersToChooseFrom);
            if (selectUser.ShowDialog(DesktopWindow.Instance) == DialogResult.OK)
            {
                promptDR = DialogResult.OK;
                retVal = selectUser.SelectedUser;
                _logger.Debug("Current user selected new owner: " + retVal.Name);
            }
            else
            {
                _logger.Debug("User Cancelled the Select New User dialog.");
                promptDR = DialogResult.Cancel;
            }

            _logger.Debug("EXIT PromptForUser");
            return retVal;
        }

        /// <summary>
        /// Finds all Px Users.
        /// </summary>
        /// <returns>Dictionary of Px Users.</returns>
        private Dictionary<string, IMMPxUser> FindUsers()
        {
            _logger.Debug("ENTER FindUsers Property = All Users.");
            Dictionary<string, IMMPxUser> retVal = new Dictionary<string, IMMPxUser>();

            retVal = FindUsers(null, null, true);

            _logger.Debug("EXIT FindUsers Property = All Users.");
            return retVal;
        }

        /// <summary>
        /// Finds Px Users based on target state and specific role names.
        /// </summary>
        /// <param name="pxTargetState">Target PxState to check for roles.</param>
        /// <param name="pxSpecifiedRoleNames">Specific PxRoles to check for.</param>
        /// <param name="allUsers">if <c>true</c> all parameters will be overriden.  All users will be returned.</param>
        /// <returns>Px Users fitting criteria of parameters.</returns>
        private Dictionary<string, IMMPxUser> FindUsers(IMMPxState pxTargetState, ArrayList pxSpecifiedRoleNames, bool allUsers)
        {
            _logger.Debug("ENTER FindUsers with parameters.");

            Dictionary<string, IMMPxUser> retVal = new Dictionary<string, IMMPxUser>();

            IMMEnumPxUser pxUsers = _PxApp.Users;
            pxUsers.Reset();

            //if include all users then add EVERY USER in PX.
            if (allUsers)
            {
                _logger.Debug("All users parameter set. Finding all users.");
                IMMPxUser pxUser = pxUsers.Next();
                while (pxUser != null)
                {
                    if (retVal.ContainsValue(pxUser) == false)
                    {
                        retVal.Add(((IMMPxUser2)pxUser).DisplayName, pxUser);
                    }

                    pxUser = pxUsers.Next();
                }
                _logger.Debug("All users added to list.");
            }
            else
            {
                bool roleInState = false;
                bool roleInList = false;
                bool addToFoundUsers = false;

                ArrayList pxTargetStateRoles = null;

                //If parameter set get the Roles assigned to target state.
                if (pxTargetState != null)
                {
                    _logger.Debug("Finding Px Roles for target state: " + pxTargetState.Name);
                    IMMEnumPxRole pxRoles = pxTargetState.Roles;
                    if (pxRoles != null)
                    {
                        pxTargetStateRoles = new ArrayList();
                        pxTargetStateRoles = ConvertEnumPxRolesToArray(pxRoles);
                    }
                    else
                    {
                        _logger.Warn("No PxRoles assigned to target state.");
                    }
                }

                //filter the roles to check.
                if ((pxSpecifiedRoleNames != null) && (pxTargetStateRoles != null))
                {
                    _logger.Debug("Filtering target roles by parameter roles.");
                    ArrayList tempList = FilterRolesToCheck(pxTargetStateRoles, pxSpecifiedRoleNames);
                    pxSpecifiedRoleNames = null;
                    pxSpecifiedRoleNames = tempList;
                }

                IMMPxUser pxUser = pxUsers.Next();
                while (pxUser != null)
                {
                    roleInList = false;
                    roleInState = false;
                    addToFoundUsers = false;

                    IMMEnumPxRole pxUserRoles = pxUser.Roles;
                    pxUserRoles.Reset();
                    string pxUserRole = pxUserRoles.Next();

                    if (pxTargetStateRoles != null)
                    {
                        _logger.Debug("Checking if PxUser " + pxUser.Name + " has role for target state.");
                        while (pxUserRole != null)
                        {
                            if (DoesUserHaveRoleForState(pxUserRole, pxTargetStateRoles))
                            {
                                roleInState = true;
                                _logger.Debug("User has role.");
                                break;
                            }

                            pxUserRole = pxUserRoles.Next();
                        }
                    }

                    if (pxSpecifiedRoleNames != null)
                    {
                        _logger.Debug("Checking if PxUser " + pxUser.Name + " has role specified in parameter.");
                        if (DoesUserHaveRoleInList(pxUser, pxSpecifiedRoleNames))
                        {
                            roleInList = true;
                            _logger.Debug("User has role in specfied parameter list.");
                        }
                    }

                    //Add to list of found users if appropriate
                    //if target state only
                    if ((!allUsers) && (roleInState) && (pxTargetState != null) && (pxSpecifiedRoleNames == null))
                    {
                        addToFoundUsers = true;
                        _logger.Debug("Adding user because PxUser has role in target state specified in Subtask Parameter: Target To State Only?.");
                    }

                    //if specific roles only
                    if ((!roleInState) && (roleInList) && (!allUsers) && (pxSpecifiedRoleNames.Count >= 1))
                    {
                        addToFoundUsers = true;
                        _logger.Debug("Adding user because PxUser has role in list of roles specified in Subtask Parameter: Specific Role(s).");
                    }

                    //target state only and specific roles
                    if ((!allUsers) && (roleInState) && (roleInList))
                    {
                        addToFoundUsers = true;
                        _logger.Debug("Adding user because PxUser has role in list of roles (and the roles are in the Target State roles)" +
                            " specified in Subtask Parameter: Target State Only? AND Specific Role(s).");
                    }

                    if ((addToFoundUsers) && (retVal.ContainsValue(pxUser) == false))
                    {
                        retVal.Add(((IMMPxUser2)pxUser).DisplayName, pxUser);
                    }

                    pxUser = pxUsers.Next();
                }
            }

            _logger.Debug("EXIT FindUsers with parameters.");

            return retVal;
        }

        /// <summary>
        /// Converts an enumeration of PxRoles to an ArrayList object.
        /// </summary>
        /// <param name="pxRoles">Enumeration of Px Roles</param>
        /// <returns>ArrayList of strings.</returns>
        protected ArrayList ConvertEnumPxRolesToArray(IMMEnumPxRole pxRoles)
        {
            _logger.Debug("ENTER ConvertEnumPxRolesToArray");

            ArrayList retVal = new ArrayList();

            pxRoles.Reset();
            string role;
            role = pxRoles.Next();

            _logger.Debug("Converting PxRoles");
            while (role != null)
            {
                if (role == "")
                {
                    break;
                }

                if (!retVal.Contains(role))
                {
                    _logger.Debug("Role added: " + role);
                    retVal.Add(role);
                }

                role = pxRoles.Next();
            }

            _logger.Debug("EXIT ConvertEnumPxRolesToArray");
            return retVal;

        }


        /// <summary>
        /// Removes all but specified Px Roles from the input list.
        /// </summary>
        /// <param name="pxStateRoles">Roles assigned to a Px Node.</param>
        /// <param name="pxSpecifiedRoleNames">Roles specified in in the Px Admin Tool.</param>
        /// <returns></returns>
        private ArrayList FilterRolesToCheck(ArrayList pxStateRoles, ArrayList pxSpecifiedRoleNames)
        {
            _logger.Debug("ENTER FilterRolesToCheck");

            ArrayList tempList = new ArrayList();
            //filter the pxStateRoles to remove all but that are in the target roles
            _logger.Debug("Filtering the pxStateRoles to remove all but that are in the target roles.");
            foreach (string targetRole in pxStateRoles)
            {
                if (!pxSpecifiedRoleNames.Contains(targetRole))
                {
                    if (!tempList.Contains(targetRole))
                    {
                        tempList.Add(targetRole);
                    }
                }
            }

            //remove the non matching roles from the roles to check.
            _logger.Debug("Removing the non-matching roles from the roles to check.");
            foreach (string removeRole in tempList)
            {
                _logger.Debug("Removed role: " + removeRole);
                pxStateRoles.Remove(removeRole);
            }

            if (pxStateRoles.Count == 0)
            {
                _logger.Debug("All roles filtered.  Returning Null.");
                pxStateRoles = null;
            }

            _logger.Debug("EXIT FilterRolesToCheck");

            return pxStateRoles;
        }

        /// <summary>
        /// Verify that the Px User has at least one of the target Px Role Names.
        /// </summary>
        /// <param name="pxUser">User defined in the Px Admin Tool.</param>
        /// <param name="pxTargetRoleNames">Roles to check.</param>
        /// <returns>True if the Px User has one of the specified roles.</returns>
        private bool DoesUserHaveRoleInList(IMMPxUser pxUser, ArrayList pxTargetRoleNames)
        {
            _logger.Debug("ENTER DoesUserHaveRoleInList");

            bool retVal = false;

            IMMEnumPxRole userRoles = pxUser.Roles;
            userRoles.Reset();
            string userRole = userRoles.Next();

            while (userRole != null)
            {
                if (pxTargetRoleNames.Contains(userRole))
                {
                    retVal = true;
                    _logger.Debug("PxUser " + pxUser.Name + " has role in list.");
                    break;
                }

                userRole = userRoles.Next();
            }

            _logger.Debug("EXIT DoesUserHaveRoleInList");

            return retVal;
        }

        #endregion
    }
}
