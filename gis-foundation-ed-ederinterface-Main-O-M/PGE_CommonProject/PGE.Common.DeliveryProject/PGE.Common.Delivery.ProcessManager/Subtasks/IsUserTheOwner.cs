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
    /// PxSubtask that enables if the the specified user (User ID parameter) is/is not (Yes or No parameter) the owner of the current PxNode.
    /// </summary>
    [ComVisible(true)]
    [Guid("DB1695A0-4072-480D-8B06-7AEADCC6F906")]
    [ProgId("PGE.Common.Subtask.IsUsertheOwner")]
    [ComponentCategory(ComCategory.MMPxSubtasks)]
    public class IsUsertheOwner : BasePxSubtask
    {

        #region Private Fields

        string _userNameKey;
        private string _yesNoKey;

        #endregion

        #region Constructors / Destructors
        /// <summary>
        /// 
        /// </summary>
        public IsUsertheOwner() : base("Is user the owner?")
        {
            AddExtension(BasePxSubtask.SessionManagerExt);
            AddExtension(BasePxSubtask.WorkflowManagerExt);

            _userNameKey = "User Name";
            string userIDDesc = "Enter user name or leave blank for logged in user. If user is not found, logged in user will be used.";
            AddParameter(_userNameKey, userIDDesc);

            _yesNoKey = "Yes or No";
            string yesNoParamDesc = "Choose one of the following: YES | NO";
            AddParameter(_yesNoKey, yesNoParamDesc);
        }

        #endregion

        #region BasePx Overrides

        /// <summary>
        /// Gets if the subtask is enabled for the specified px node.
        /// </summary>
        /// <param name="pPxNode">The px node.</param>
        /// <returns><c>true</c> if enabled; otherwise <c>false</c>.</returns>
        protected override bool InternalEnabled(Miner.Interop.Process.IMMPxNode pPxNode)
        {
            try
            {
                _logger.Debug("ENTER InternalEnabled.");

                bool retVal = false;

                bool isUserOwner = false;
                bool yes = (GetParameter(_yesNoKey).ToUpper() == "YES") ? true : false;
                string userNameToCheck = _PxApp.User.Name;

                _logger.Debug("Get PxUser from parameter");
                string username = GetParameter(_userNameKey);
                if ((username != null) && (username != string.Empty) && (username != ""))
                {
                    IMMPxUser pxUser = FindUser(username);
                    if (pxUser != null)
                    {
                        _logger.Debug("User in paramater found: " + pxUser.Name);
                        userNameToCheck = pxUser.Name;
                    }
                    else
                    {
                        _logger.Warn("User in parameter not found.  Using Logged in user: " + _PxApp.User.Name);
                    }
                }

                IMMPxUser pxCurrentOwner = null;
 
                _logger.Debug("Getting current owner PxUser.Id");
                switch (pPxNode.NodeType)
                {
                    case BasePxSubtask.DesignNodeType:
                        IMMWMSDesign wmsDesign = (IMMWMSDesign)GetWmsNode(pPxNode, BasePxSubtask.DesignNodeName);
                        pxCurrentOwner = FindUser(wmsDesign.get_OwnerID());
                        break;
                    case BasePxSubtask.WorkRequestNodeType:
                        IMMWMSWorkRequest wmsWorkRequest = (IMMWMSWorkRequest)GetWmsNode(pPxNode, BasePxSubtask.WorkRequestNodeName);
                        pxCurrentOwner = FindUser(wmsWorkRequest.get_OwnerID());
                        break;
                    case BasePxSubtask.SessionNodeType:
                        IMMSession session = GetSessionNode(pPxNode.Id);
                        pxCurrentOwner = FindUser(session.get_Owner());
                        break;
                    default:
                        break;
                }

                if (pxCurrentOwner != null)
                {

                    _logger.Debug("Current owner is: " + pxCurrentOwner.Name);
                }
                
                if (userNameToCheck.ToUpper() == pxCurrentOwner.Name.ToUpper())
                {
                    isUserOwner = true;
                }

                if (yes && isUserOwner) { retVal = true; }
                if (!yes && !isUserOwner) { retVal = true; }

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
