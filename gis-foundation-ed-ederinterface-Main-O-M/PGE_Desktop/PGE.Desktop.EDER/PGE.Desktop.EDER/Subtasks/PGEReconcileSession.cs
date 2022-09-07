using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;

using Miner.Interop;
using Miner.Interop.Process;
using Miner.ComCategories;
using Miner.Framework;

using PGE.Common.Delivery.Process.BaseClasses;
using PGE.Common.Delivery.ArcFM;

namespace PGE.Desktop.EDER.Subtasks
{
    /// <summary>
    /// Subtask that reconciles sessions with option to lock target version and to disable autoupdaters. 
    /// </summary>
    [ComVisible(true)]
    [Guid("4112341B-9FDD-4749-BC87-907AC0537133")]
    [ProgId("PGE.Desktop.EDER.ReconcileSession")]
    [ComponentCategory(ComCategory.MMPxSubtasks)]
    public class PGEReconcileSession : BasePxSubtask
    {
        #region Private Members

        private IMMSessionManager3 _sessionMgrExt;
        private bool _conflictsByColumn = false;
        private bool _disableAUs = false;
        private bool _lockTargetVersion = false;
        private bool _childWins = false;
        private bool _abortIfConflicts = false;
        private bool _useConflictsWindow = false;

        #endregion

        #region Constructor

        public PGEReconcileSession() : base("PGE Reconcile Session")
        {
            AddParameter("LockTargetVersion", "Enter one of the following: TRUE | FALSE");
            AddParameter("DisableAUs", "Enter one of the following: TRUE | FALSE");
            AddParameter("ConflictsByColumn", "Enter one of the following: TRUE | FALSE");
            AddParameter("InFavorOfChildVersion", "Enter one of the following: TRUE | FALSE");
            AddParameter("AbortIfConflicts", "Enter one of the following: TRUE | FALSE");
            AddParameter("UseConflictsWindow", "Enter one of the following: TRUE | FALSE");

            AddExtension(BasePxSubtask.SessionManagerExt);
        }

        #endregion

        #region BasePxSubtask Overrides

        /// <summary>
        /// Initializes the subtask
        /// </summary>
        /// <param name="pPxApp">The IMMPxApplication.</param>
        //// <returns><c>true</c> if intialized; otherwise <c>false</c>.</returns>
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
            IMMArcGISRuntimeEnvironment rte = new ArcGISRuntimeEnvironment();

            bool retVal = false;
            _disableAUs = GetParameter("DisableAUs").ToUpper() == "TRUE" ? true : false;
            _conflictsByColumn = GetParameter("ConflictsByColumn").ToUpper() == "TRUE" ? true : false;
            _lockTargetVersion = GetParameter("LockTargetVersion").ToUpper() == "TRUE" ? true : false;
            _childWins = GetParameter("InFavorOfChildVersion").ToUpper() == "TRUE" ? true : false;
            _abortIfConflicts = GetParameter("AbortIfConflicts").ToUpper() == "TRUE" ? true : false;
            _useConflictsWindow = GetParameter("UseConflictsWindow").ToUpper() == "TRUE" ? true : false;

            _logger.Debug("Parameter values:  DisableAUs=" + _disableAUs.ToString() + 
                " | ConflictsByColumn=" + _conflictsByColumn.ToString() + 
                " | LockTargetVersion=" + _lockTargetVersion.ToString() + 
                " | InFavorOfChildVersion=" + _childWins.ToString() + 
                " | AbortIfConflicts=" + _abortIfConflicts.ToString() + 
                " | UseConflictsWindow= " + _useConflictsWindow.ToString());

            int result = -1;

            Type auType = Type.GetTypeFromProgID("mmGeodatabase.MMAutoUpdater");
            object auObj = Activator.CreateInstance(auType);
            IMMAutoUpdater auFramework = auObj as IMMAutoUpdater;
            mmAutoUpdaterMode currentAUMode = auFramework.AutoUpdaterMode;

            try
            {
                //turn off AUs
                if (_disableAUs)
                {
                    auFramework.AutoUpdaterMode = mmAutoUpdaterMode.mmAUMNoEvents;
                }

                StartGlobeSpinning();

                System.Windows.Forms.Cursor.Current = Cursors.WaitCursor;

                string sourceVersionName;
                string targetVersionname;

                IMMVersioningUtils versionUtils = new MMVersioningUtilsClass();
                IWorkspace workspace = ((IMMPxApplicationEx2)_PxApp).Workspace;
                if (workspace == null)
                {
                    StopGlobeSpinning();
                    System.Windows.Forms.Cursor.Current = Cursors.Default;
                    return false;
                }

                IVersion sourceVersion = (IVersion)workspace;
                IVersionInfo versionInfo = sourceVersion.VersionInfo;
                if (versionInfo == null)
                {
                    StopGlobeSpinning();
                    System.Windows.Forms.Cursor.Current = Cursors.Default;
                    return false;
                }

                sourceVersionName = versionInfo.VersionName;
                targetVersionname = versionInfo.Parent.VersionName;

                string msg = "Reconcilling " + sourceVersionName + " with parent: " + targetVersionname;
                rte.SetStatusBarMessage(msg);
                _logger.Info(msg);

                if (versionUtils is IMMAdvancedReconcile)
                {
                    IMMAdvancedReconcile advReconcile = (IMMAdvancedReconcile)versionUtils;
                    result = advReconcile.ReconcileVersion(workspace, sourceVersionName, targetVersionname, _useConflictsWindow,
                        _lockTargetVersion, _abortIfConflicts, _childWins, _conflictsByColumn);
                }
                else
                {
                    if (_conflictsByColumn)
                    {
                        _logger.Info("The subtask is configured to define conflicts by column, but the" +
                            " MMVersioningUtils object does not support IMMAdvancedReconcile interface." +
                            " Conflicts will be defined by row instead.");
                    }
                    result = versionUtils.ReconcileVersion(workspace, sourceVersionName, targetVersionname, true);
                }

                //Get return value for subtask
                if (result == 0)
                {
                    retVal = true;
                    IActiveView activeView = rte.FocusMap as IActiveView;
                    activeView.PartialRefresh(esriViewDrawPhase.esriViewGeography | esriViewDrawPhase.esriViewGeoSelection, null, null);
                }
                else
                {
                    rte.SetStatusBarMessage("A reconcile error has occurred.");
                    _logger.Error("A reconcile error has occurred.");
                }

                rte.ClearStatusBarMessage();
                StopGlobeSpinning();
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
            }
            finally
            {
                if (_disableAUs)
                {
                    auFramework.AutoUpdaterMode = currentAUMode;
                }
            }
            
            return retVal;
        }


        #endregion
    }
}
