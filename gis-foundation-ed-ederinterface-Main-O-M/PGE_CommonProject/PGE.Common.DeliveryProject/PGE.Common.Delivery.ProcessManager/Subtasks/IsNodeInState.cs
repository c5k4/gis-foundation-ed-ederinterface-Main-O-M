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
    /// PxSubtask that enables if the the current user has/doesn't have (Yes or No parameter) specified PxStates (State Name(s) parameter).
    /// </summary>
    [ComVisible(true)]
    [Guid("57A4CFC3-E5EA-4C33-87EF-985D26A6445D")]
    [ProgId("PGE.Common.Subtask.IsNodeInState")]
    [ComponentCategory(ComCategory.MMPxSubtasks)]
    public class IsNodeInState : BasePxSubtask              
    {

        #region Private Fields

        string _stateListParamKey;
        string _yesNoKey;

        #endregion

        #region Constructors / Destructors
        /// <summary>
        /// 
        /// </summary>
        public IsNodeInState() : base("Is node in state?")
        {
            //Add Extenstions
            AddExtension(BasePxSubtask.SessionManagerExt);
            AddExtension(BasePxSubtask.WorkflowManagerExt);

            //Add Parameters
            _stateListParamKey = "State Name(s)";
            string stateParamDesc = "Please enter state names to verify, separated by a comma.";
            AddParameter(_stateListParamKey, stateParamDesc);

            _yesNoKey = "Yes or No";
            string yesNoParamDesc = "Choose one of the following: YES | NO";
            AddParameter(_yesNoKey, yesNoParamDesc);

        }

        #endregion

        #region BasePxSubtask Overrides

        /// <summary>
        /// Executes the subtask using the specified px node.
        /// </summary>
        /// <param name="pPxNode">The px node.</param>
        /// <returns><c>true</c> if successfully executed; otherwise false.</returns>
        protected override bool InternalExecute(IMMPxNode pPxNode)
        {
            return true;
        }

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
                bool containsStateName = false;

                string stateNameListValue = GetParameter(_stateListParamKey);
                string yesNo = GetParameter(_yesNoKey);

                _logger.Debug("Getting list of state names to check");
                ArrayList stateNameList = GetListOfValuesFromParameter(stateNameListValue);

                if (stateNameList.Contains(pPxNode.State.Name.ToUpper()) == true) { containsStateName = true; }

                // depending on the parameter selected and the state of the node, return true or false
                if (yesNo.ToUpper() == "YES" && containsStateName) { retVal = true; }
                if (yesNo.ToUpper() == "NO" && !containsStateName) { retVal = true; }

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

        #endregion

    }
}
