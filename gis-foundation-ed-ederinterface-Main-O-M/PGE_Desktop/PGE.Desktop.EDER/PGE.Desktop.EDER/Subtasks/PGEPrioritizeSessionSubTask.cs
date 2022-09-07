using System;
using ESRI.ArcGIS.Geodatabase;
using Miner.ComCategories;
using System.Runtime.InteropServices;
using PGE.Common.Delivery.Process.BaseClasses;
using Miner.Interop.Process;
using Miner.Geodatabase.Edit;
using ESRI.ArcGIS.Framework;

namespace PGE.Desktop.EDER.Subtasks
{
    /// <summary>
    /// Subtask that set the priority of the session based on keywords(CMCS,PARADISE,HIGH). 
    /// </summary>
    [ComVisible(true)]
    [Guid("53AAD1E0-E77C-4FBA-9604-C0BEEED0C23A")]
    [ProgId("PGE.Desktop.EDER.SubTasks.PGE_PrioritizeSessionSubTask")]
    [ComponentCategory(ComCategory.MMPxSubtasks)]
    public class PGEPrioritizeSessionSubTask : BasePxSubtask
    {
        
        private IMMSessionManager3 _sessionMgrExt;
        private string _Keywords;
        
        #region Constructor

        public PGEPrioritizeSessionSubTask() : base("PGEPrioritizeSession")
        {
            _Keywords = "Keywords";
            string roleListDesc = "Enter Keywords separated by a comma.";
            AddParameter(_Keywords, roleListDesc);

            
        }

        #endregion


        #region BasePxSubtask Overrides

        /// <summary>
        /// Initializes the subtask
        /// </summary>
        /// <param name="pPxApp">The IMMPxApplication.</param>
        /// <returns><c>true</c> if intialized; otherwise <c>false</c>.</returns>
        public override bool Initialize(IMMPxApplication pPxApp)
        {
            base.Initialize(pPxApp);

            //Check application and session manager
            if (base._PxApp == null) return false;

            _sessionMgrExt = _PxApp.FindPxExtensionByName(BasePxSubtask.SessionManagerExt) as IMMSessionManager3;
            if (_sessionMgrExt == null) return false;

            return true;
        }

        /// <summary>
        /// Gets if the subtask is enabled for the specified px node.
        /// </summary>
        /// <param name="pPxNode">The px node.</param>
        /// <returns><c>true</c> if enabled; otherwise <c>false</c>.</returns>
        protected override bool InternalEnabled(IMMPxNode pPxNode)
        {
            //Verify incoming node is the expected session node type.
            if (pPxNode == null) return false;
            if (pPxNode.NodeType != BasePxSubtask.SessionNodeType) return false;

            //Check if Px is in standalone mode
            if (_PxApp.StandAlone == true) return false;
            if (_sessionMgrExt.CurrentOpenSession == null) return false;

            //Verify that a session is open for editing
            IMMSessionEx sessionEx = _sessionMgrExt.CurrentOpenSession as IMMSessionEx;
            if (sessionEx.get_ViewOnly() == true) return false;

            return true;

        }

        /// <summary>
        /// Executes the subtask.
        /// </summary>
        /// <param name="pPxNode">Selected node.</param>
        /// <returns><c>true</c> if executes successfully; otherwise <c>false</c>.</returns>
        protected override bool InternalExecute(Miner.Interop.Process.IMMPxNode pPxNode)
        {

            bool success = false;
            {
                try
                {
                    IWorkspaceEdit pWSE = (IWorkspaceEdit)Editor.EditWorkspace;
                    IMMSession psession = _sessionMgrExt.GetSession(pPxNode.Id, true);

                    string sessioname = psession.get_Name();
                    
                    if ((pWSE == null) || !(pWSE.IsBeingEdited()))
                        return false;
                    string updatequery = string.Empty;
                    //Set Priority based on keywords
                    string PrioritizeKeywords = GetParameter(_Keywords);
                    string[] sKeywords = PrioritizeKeywords.ToString().Split(',');
                    if (sKeywords.Length > 0)
                    {
                        for (int i = 0; i < sKeywords.Length; i++)
                        {
                           
                            if (sessioname.ToUpper().Contains(sKeywords[i].ToUpper()))
                            {
                                updatequery =  "UPDATE SDE.GDBM_POST_QUEUE SET PRIORITY = 10  where VERSION_NAME='SN_" + psession.get_ID() + "'";
                                ((IWorkspace)pWSE).ExecuteSQL(updatequery);
                                break;
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("Please check with administrator as there is no keywords present in the GDBM Parameter list");
                    }
                    success = true;

                }
                catch (Exception ex)
                {
                    _logger.Debug("PGE_SesssionPriority_SubTask: Error during prioritize session based on keywords: " + ex.Message + " StackTrace: " + ex.StackTrace);
                }


            }
            return success;
        }

        /// <summary>
        /// Create Query to Prioritize the session in GDBM
        /// </summary>
        /// <param name="sKeyword"></param>
        /// <returns></returns>
        private string CreateQuery_PrioritizeSessionBasedonKeywords(string sKeyword)
        {

            //--set the priority high where the keywords is present in the session name
            string updatequery = "UPDATE SDE.GDBM_POST_QUEUE SET PRIORITY = 10  ";

            return updatequery;
        }

        #endregion



    }
}
