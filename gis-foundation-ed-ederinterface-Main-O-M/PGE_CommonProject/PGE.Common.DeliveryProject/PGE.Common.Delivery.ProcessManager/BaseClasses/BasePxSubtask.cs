using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections;
using System.Reflection;

using ESRI.ArcGIS.esriSystem;
using Miner.Interop.Process;

using PGE.Common.Delivery.Diagnostics;
using PGE.Common.Delivery.ArcFM;
using IDictionary = Miner.Interop.Process.IDictionary;

namespace PGE.Common.Delivery.Process.BaseClasses
{
    /// <summary>
    /// Represents an abstract clas for process framework subtask implementations
    /// </summary>
    /// 
    [ComVisible(true)]
    abstract public class BasePxSubtask : IMMPxSubtask, IMMPxSubtask2
    {
        protected static PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger = new Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "");

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

        /// <summary>
        /// The process framework application reference
        /// </summary>
        protected IMMPxApplication _PxApp;
        /// <summary>
        /// The name of the subtask
        /// </summary>
        protected string _Name;
        /// <summary>
        /// The parent task to this subtask
        /// </summary>
        protected IMMPxTask _Task;
        /// <summary>
        /// 
        /// </summary>
        protected const string SessionManagerExt = "MMSessionManager";
        /// <summary>
        /// 
        /// </summary>
        protected const string SessionNodeName = "Session";
        /// <summary>
        /// 
        /// </summary>
        protected const int SessionNodeType = 3;
        /// <summary>
        /// 
        /// </summary>
        protected const string WorkflowManagerExt = "MMWorkflowManager";
        /// <summary>
        /// 
        /// </summary>
        protected const string WorkRequestNodeName = "WorkRequest";
        /// <summary>
        /// 
        /// </summary>
        protected const int WorkRequestNodeType = 10;
        /// <summary>
        /// 
        /// </summary>
        protected const string DesignNodeName = "Design";
        /// <summary>
        /// 
        /// </summary>
        protected const int DesignNodeType = 11;

        private IDictionary _Parameters;
        private IDictionary _SupportedParameters;
        private IMMEnumExtensionNames _SupportedExtensions;
        
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="BasePxSubtask"/> class.
        /// </summary>
        /// <param name="name">The name </param>
        protected BasePxSubtask(string name)
        {
            _Name = name;
        }
        #endregion

        #region IMMPxSubtask Members

        /// <summary>
        /// Executes the subtask using the specified px node.
        /// </summary>
        /// <param name="pPxNode">The px node.</param>
        /// <returns><c>true</c> if the success; otherwise false.</returns>
        public virtual bool Execute(IMMPxNode pPxNode)
        {
            DateTime startTime = DateTime.Now;
            DateTime endTime;

            try
            {
                _logger.Debug(DateTime.Now.ToLongTimeString() + " ==== BEGIN EXECUTE: " + this._Name);

                return InternalExecute(pPxNode);
            }
            catch (Exception ex)
            {
                EventLogger.Error(ex, "Error Executing Subtask " + _Name);
                return false;
            }
            finally
            {
                endTime = DateTime.Now;
                TimeSpan ts = endTime - startTime;
                _logger.Debug("TOTAL EXECUTION TIME ==== " + ts.TotalSeconds.ToString());
            }

        }

        /// <summary>
        /// Gets if the subtask is enabled for the specified px node.
        /// </summary>
        /// <param name="pPxNode">The px node.</param>
        /// <returns><c>true</c> if enabled; otherwise <c>false</c>.</returns>
        public virtual bool Enabled(IMMPxNode pPxNode)
        {
            try
            {
                return InternalEnabled(pPxNode);
            }
            catch (Exception ex)
            {
                EventLogger.Error(ex, "Error Enabling Subtask " + _Name);
            }

            return false;
        }

        /// <summary>
        /// Initializes the subtask
        /// </summary>
        /// <param name="pPxApp">The IMMPxApplication.</param>
        //// <returns><c>true</c> if intialized; otherwise <c>false</c>.</returns>
        public virtual bool Initialize(IMMPxApplication pPxApp)
        {
            _PxApp = pPxApp;
            return true;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return _Name; }
        }

        /// <summary>
        /// Rollbacks the subtask using the specified px node.
        /// </summary>
        /// <param name="pPxNode">The px node.</param>
        /// <returns><c>true</c> if rollback successfully; otherwise <c>false</c>.</returns>
        public virtual bool Rollback(IMMPxNode pPxNode)
        {
            return true;
        }

        #endregion

        #region IMMPxSubtask2 Members

        /// <summary>
        /// Sets the parameters.
        /// </summary>
        /// <value>The parameters.</value>
        public Miner.Interop.Process.IDictionary Parameters
        {
            set { _Parameters = value; }
        }

        /// <summary>
        /// Gets the supported extensions.
        /// </summary>
        /// <value>The supported extensions.</value>
        public IMMEnumExtensionNames SupportedExtensions
        {
            get
            {
                return _SupportedExtensions;
            }
        }

        /// <summary>
        /// Gets the supported parameter names.
        /// </summary>
        /// <value>The supported parameter names.</value>
        public Miner.Interop.Process.IDictionary SupportedParameterNames
        {
            get { return _SupportedParameters; }
        }

        /// <summary>
        /// Sets the task.
        /// </summary>
        /// <value>The task.</value>
        public IMMPxTask Task
        {
            set { _Task = value; }
        }

        #endregion        

        #region Protected Properties
        /// <summary>
        /// Gets the Executing Px Task for the subtask.
        /// </summary>
        public IMMPxTask ExecutingTask
        {
            get { return _Task; }
        }

        #endregion

        #region Protected Methods
        /// <summary>
        /// Executes the subtask using the specified <see cref="IMMPxNode"/>.
        /// </summary>
        /// <param name="pPxNode">The px node.</param>
        /// <returns><c>true</c> if executes successfully; otherwise <c>false</c>.</returns>
        /// <remarks>This method will be called from IMMPxSubtask::Execute
        /// and is wrapped within the exception handling for that method.</remarks>
        protected virtual bool InternalExecute(IMMPxNode pPxNode)
        {
            return false;
        }

        /// <summary>
        /// Determines if the subtask should be enabled for the specified <see cref="IMMPxNode"/>.
        /// </summary>
        /// <param name="pPxNode">The px node.</param>
        /// <returns>
        /// 	<c>true</c> if the Subtask should be enabled; otherwise <c>false</c>
        /// </returns>
        /// <remarks>This method will be called from IMMPxSubtask::Enabled
        /// and is wrapped within the exception handling for that method.</remarks>
        protected virtual bool InternalEnabled(IMMPxNode pPxNode)
        {
            return (_PxApp != null);
        }

        /// <summary>
        /// Adds the extension.
        /// </summary>
        /// <param name="extensionName">Name of the extension.</param>
        protected void AddExtension(string extensionName)
        {
            if (_SupportedExtensions == null)
                _SupportedExtensions = new PxExtensionNamesClass();

            _SupportedExtensions.Add(extensionName);
        }

        /// <summary>
        /// Adds a parameter to the Supported Parameters dictionary.
        /// </summary>
        /// <param name="key">The key of the parameter.</param>
        /// <param name="description">The description of the parameter.</param>
        protected void AddParameter(string key, string description)
        {
            if (_SupportedParameters == null)
                _SupportedParameters = new MMPxNodeListClass();

            object k = key;
            object d = description;
            _SupportedParameters.Add(ref k, ref d);
        }

        /// <summary>
        /// Determines the configured Parameter value as a string..
        /// </summary>
        /// <returns>Parameter value as string.</returns>
        protected string GetParameter(string key)
        {
            object objKey = key;
            string val = null;
            if (_Parameters.Exists(ref objKey))
            {
                object obj = _Parameters.get_Item(ref objKey);
                if (obj != null)
                    val = obj.ToString();
            }
            return val;
        }

        /// <summary>
        /// Adds the history.
        /// </summary>
        /// <param name="pxNode">The px node</param>
        /// <param name="description">The description</param>
        /// <param name="extraData">The extra data</param>
        /// <returns>Boolean</returns>
        protected bool AddHistory(IMMPxNode pxNode, string description, string extraData)
        {
            IMMPxNodeHistory nodeHistory = new PxNodeHistoryClass();
            if (!nodeHistory.Init(_PxApp.Connection, _PxApp.Login.SchemaName, pxNode.Id, pxNode.NodeType, null)) return false;

            IMMPxHistory history = new PxHistoryClass();
            history.CurrentUser = _PxApp.User.Id;
            history.CurrentUserName = _PxApp.User.Name;
            history.Date = DateTime.Now;
            history.Description = description;
            history.NodeId = pxNode.Id;
            history.nodeTypeId = pxNode.NodeType;

            ADODB.Property property = _PxApp.Connection.Properties["Data Source Name"];
            string datasource = (property.Value != null) ? property.Value.ToString() : "";

            if (_PxApp.Connection.Provider.Contains(".jet."))
            {
                FileInfo fileInfo = new FileInfo(datasource);
                history.Server = fileInfo.Name;
            }
            else
            {
                history.Server = datasource;
            }

            history.ExtraData = extraData;
            nodeHistory.Add(history);

            return true;
        }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <param name="configName">Name of the config.</param>
        /// <returns>String</returns>
        protected string GetConfiguration(string configName)
        {
            IMMPxHelper2 helper = (IMMPxHelper2)_PxApp.Helper;
            return helper.GetConfigValue(configName);
        }

        /// <summary>
        /// Finds Px Users by user name.
        /// </summary>
        /// <param name="userName">The "Name" column value from MM_PX_USERS table.</param>
        /// <returns>Px User object.</returns>
        protected IMMPxUser FindUser(string userName)
        {
            IMMPxUser retVal = null;

            IMMEnumPxUser allUsers = _PxApp.Users;
            allUsers.Reset();
            IMMPxUser2 thisUser = (IMMPxUser2)allUsers.Next();

            while (thisUser != null)
            {
                if ((thisUser.Name.ToUpper() == userName.ToUpper()) | (thisUser.DisplayName.ToUpper() == userName.ToUpper()))
                {
                    retVal = (IMMPxUser)thisUser;
                    break;
                }

                thisUser = (IMMPxUser2)allUsers.Next();
            }

            return retVal;
        }

        /// <summary>
        /// Finds Px Users by User ID
        /// </summary>
        /// <param name="userID">The "ID" column value from MM_PX_USERS table.</param>
        /// <returns>Px User object.</returns>
        protected IMMPxUser FindUser(int userID)
        {

            IMMPxUser retVal = null;

            IMMEnumPxUser allUsers = _PxApp.Users;
            allUsers.Reset();
            IMMPxUser2 thisUser = (IMMPxUser2)allUsers.Next();

            while (thisUser != null)
            {
                if (thisUser.Id == userID)
                {
                    retVal = (IMMPxUser)thisUser;
                    break;
                }

                thisUser = (IMMPxUser2)allUsers.Next();
            }

            return retVal;
        }

        /// <summary>
        /// Parses a comma delimited list from parameter value.
        /// </summary>
        /// <param name="listOfValues">Comma delimited list to split.</param>
        /// <returns>ArrayList of strings from parameter value.</returns>
        protected ArrayList GetListOfValuesFromParameter(string listOfValues)
        {
            ArrayList retVal = null;

            if ((listOfValues == null) || (listOfValues == "") || (listOfValues == string.Empty)) { return retVal; }

            char[] splitChar = ",".ToCharArray();
            string[] stateNameArray = listOfValues.Split(splitChar);
            for (int i = 0; i < stateNameArray.Length; i++)
            {
                char[] removeArray = " ".ToCharArray();
                string tempName = stateNameArray[i].TrimStart(removeArray).ToUpper();

                if (retVal == null) { retVal = new ArrayList(); }

                if (retVal.Contains(tempName) == false)
                {
                    retVal.Add(tempName);
                }
            }

            return retVal;
        }

        /// <summary>
        /// Checks to see if the user role has permissions on a specific state.
        /// </summary>
        /// <param name="pxUserRole">Role to check</param>
        /// <param name="pxStateRoles">Roles assigned to a state.</param>
        /// <returns></returns>
        protected bool DoesUserHaveRoleForState(string pxUserRole, ArrayList pxStateRoles)
        {
            bool retVal = false;

            if (pxStateRoles == null) { return retVal; }

            foreach (string roleName in pxStateRoles)
            {
                if (roleName == "")
                {
                    break;
                }

                if (pxUserRole.ToUpper() == roleName.ToUpper())
                {
                    retVal = true;
                    break;
                }
            }

            return retVal;
        }

        /// <summary>
        /// Checks to see if the user role has permissions on a specific state.
        /// </summary>
        /// <param name="pxUserRole">Role to check</param>
        /// <param name="pxStateRoles">Roles assigned to a state.</param>
        /// <returns></returns>
        protected bool DoesUserHaveRoleForState(string pxUserRole, IMMEnumPxRole pxStateRoles)
        {
            bool retVal = false;

            pxStateRoles.Reset();
            string role2;
            role2 = pxStateRoles.Next();

            while (role2 != null)
            {
                if (role2 == "")
                {
                    break;
                }

                if (pxUserRole.ToUpper() == role2.ToUpper())
                {
                    retVal = true;
                    break;
                }

                role2 = pxStateRoles.Next();
            }

            return retVal;
        }

        /// <summary>
        /// Gets the wms node from the pxNode
        /// </summary>
        /// <param name="pxnode">PxNode to convert</param>
        /// <param name="nodeName">String name of node type</param>
        /// <returns></returns>
        protected IMMWMSNode GetWmsNode(IMMPxNode pxnode, string nodeName)
        {
            return GetWmsNode(pxnode.Id, nodeName);
        }

        /// <summary>
        /// Get the wms node from the node id
        /// </summary>
        /// <param name="id">PxNode Id</param>
        /// <param name="nodeName">String name of node type</param>
        /// <returns></returns>
        protected IMMWMSNode GetWmsNode(int id, string nodeName)
        {
            bool readOnly = false, suppress = true;

            IMMWorkflowManager wfm = (IMMWorkflowManager)_PxApp.FindPxExtensionByName(BasePxSubtask.WorkflowManagerExt);

            IMMWMSNode wmsNode = wfm.GetWMSNode(ref nodeName, ref id, ref readOnly, ref suppress);

            return wmsNode;
        }

        /// <summary>
        /// Get the session node from the node id
        /// </summary>
        /// <param name="id">PxNode Id</param>
        /// <returns></returns>
        protected IMMSession GetSessionNode(int id)
        {
            bool readOnly = false;

            IMMSessionManager sessionMgr = (IMMSessionManager)_PxApp.FindPxExtensionByName(BasePxSubtask.SessionManagerExt);

            IMMSession sessionNode = sessionMgr.GetSession(ref id, ref readOnly);

            return sessionNode;
        }

        /// <summary>
        /// Starts the ArcMap globe spinning.
        /// </summary>
        protected void StartGlobeSpinning()
        {
            if (ApplicationFacade.Application == null) return;

            IStatusBar statusBar = ApplicationFacade.Application.StatusBar;
            statusBar.ProgressAnimation.Show();
            statusBar.PlayProgressAnimation(true);
        }

        /// <summary>
        /// Stops the ArcMap globe spinning.
        /// </summary>
        protected void StopGlobeSpinning()
        {
            if (ApplicationFacade.Application == null) return;

            IStatusBar statusBar = ApplicationFacade.Application.StatusBar;
            statusBar.PlayProgressAnimation(false);
            statusBar.ProgressAnimation.Hide();
        }


        #endregion
    }
}
