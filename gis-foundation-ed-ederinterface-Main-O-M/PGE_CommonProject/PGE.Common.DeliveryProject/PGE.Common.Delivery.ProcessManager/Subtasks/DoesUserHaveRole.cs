using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Collections;

using log4net;

using Miner.Interop.Process;
using Miner.Interop;
using Miner.ComCategories;

using PGE.Common.Delivery.Process.BaseClasses;

namespace PGE.Common.Delivery.Process.Subtasks
{
    /// <summary>
    /// PxSubtask that enables if the the current user has/doesn't have (Yes or No parameter) specified PxRoles (Role Name(s) parameter). 
    /// </summary>
    [ComVisible(true)]
    [Guid("5F245BE4-5F64-4268-9C49-74E0305D40D0")]
    [ProgId("PGE.Common.SubTask.DoesUserHaveRole")]
    [ComponentCategory(ComCategory.MMPxSubtasks)]
    public class DoesUserHaveRole : BasePxSubtask
    {

        #region Private Fields

        private string _roleListKey;
        private string _yesNoKey;

        #endregion

        #region Constructors / Destructors
        /// <summary>
        /// 
        /// </summary>
        public DoesUserHaveRole() : base("Does user have role?")
        {
            //Add Extenstions
            AddExtension(SessionManagerExt);
            AddExtension(WorkflowManagerExt);

            _roleListKey = "Role Name(s)";
            string roleListDesc = "Enter Role name(s) separated by a comma.";
            AddParameter(_roleListKey, roleListDesc);

            _yesNoKey = "Yes or No";
            string paramYesNoDesc = "Choose one of the following: YES | NO";
            AddParameter(_yesNoKey, paramYesNoDesc);
        }

        #endregion

        #region BasePxSubtask Overrides

        /// <summary>
        /// Gets if the subtask is enabled for the specified px node.
        /// </summary>
        /// <param name="pPxNode">The px node.</param>
        /// <returns><c>true</c> if enabled; otherwise <c>false</c>.</returns>
        protected override bool InternalEnabled(IMMPxNode pPxNode)
        {
            try
            {
                _logger.Debug("ENTER InternalEnabled");

                bool retVal = false;

                //Get parameter values
                string roleListValue = GetParameter(_roleListKey);
                bool yes = (GetParameter(_yesNoKey).ToUpper() == "YES") ? true : false;

                bool hasRole = false;
                IMMPxUser user = _PxApp.User;
                ArrayList assignedRoles = new ArrayList();
                IMMEnumPxRole userRoles = user.Roles;
                userRoles.Reset();
                string role = userRoles.Next();

                _logger.Debug("Getting list of roles from parameter.");
                ArrayList roleList = GetListOfValuesFromParameter(roleListValue);

                while (role != null)
                {
                    if (roleList.Contains(role.ToUpper()))
                    {
                        _logger.Debug("User has one of the roles in parameter list.");
                        hasRole = true;
                        break;
                    }

                    role = userRoles.Next();
                }

                if (yes && hasRole) { retVal = true; }
                if (!yes && !hasRole) { retVal = true; }

                _logger.Debug("Subtask enabled.");
                return retVal;
            }
            catch (Exception exp)
            {
                _logger.Error("Error encountered: " + exp.Message, exp);
                return false;
            }
            finally
            {
                _logger.Debug("EXIT InternalEnabled");
            }

        }

        /// <summary>
        /// Executes the subtask using the specified px node.
        /// </summary>
        /// <param name="pPxNode">The px node.</param>
        /// <returns><c>true</c> if successfully executed; otherwise false.</returns>
        protected override bool InternalExecute(IMMPxNode pPxNode)
        {
            return true;
        }

        #endregion
    }
}
