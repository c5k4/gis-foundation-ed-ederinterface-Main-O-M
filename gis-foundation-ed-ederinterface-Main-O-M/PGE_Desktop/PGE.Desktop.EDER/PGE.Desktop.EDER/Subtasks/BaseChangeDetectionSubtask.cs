using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PGE.Common.Delivery.Process.BaseClasses;
using Miner.Interop;
using Miner.Geodatabase.Edit;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Diagnostics;
using System.Reflection;
using ESRI.ArcGIS.Geodatabase;

namespace PGE.Desktop.EDER.Subtasks
{
    /// <summary>
    /// Base class that provides methods to detect the changes and persist to the changes tables
    /// </summary>
    public abstract class BaseChangeDetectionSubtask:BaseDesktopVersionDifferenceSubtask
    {
        #region Private Variables
        /// <summary>
        /// Logs the messages
        #endregion Private Variables

        #region Constructor
        /// <summary>
        /// Initialises the new instance of <see cref="BaseChangeDetectionSubtask"/> class
        /// </summary>
        public BaseChangeDetectionSubtask(string name):base(name)
        {
        }
        #endregion Constructor

        #region Protected Properties

        /// <summary>
        /// Holds an instance to read and persist changes to the tables
        /// </summary>
        protected abstract IChange Changedetector
        {
            get;
        }

        #endregion Protected Properties

        #region Protected Methods

        /// <summary>
        /// Executes the Subtask. Persists the changes to the tables
        /// </summary>
        /// <param name="pPxNode"></param>
        /// <returns>Returns true if process is successful; false, otherwise.</returns>
        protected override bool InternalExecuteExtended(Miner.Interop.Process.IMMPxNode pPxNode)
        {
            bool retVal = true;
            IWorkspaceEdit wkspcEdit = (IWorkspaceEdit)Editor.EditWorkspace;
            bool startedOperation = false;
            bool stoppedEditing = false;
            try
            {
                if (Changedetector != null)
                {
                    Changedetector.Workspace = Editor.EditWorkspace;
                 //Get the difference between the current session and base default session
                    ID8List versionDifference = DesktopVersionDifference.GetVersionDifference(Editor.EditWorkspace);
                    //Get the list of changes
                    ProcessD8List(versionDifference);
                    wkspcEdit.StartEditOperation();
                    //indicator for the finally block to abort operation if the code fails before the edit operation is stopped
                    startedOperation = true;
                    //Persist the changes the tables
                    retVal = Changedetector.PersistChange();
                    wkspcEdit.StopEditOperation();
                    startedOperation = false;
                    //Save the Edits and restart editing
                    _logger.Debug("Saving edits by stoppping Edit");
                    wkspcEdit.StopEditing(true);
                    //indicator for the finally block if the code fails here so the editing can be restarted
                    stoppedEditing = true;
                    _logger.Debug("Restarting Edit");
                    wkspcEdit.StartEditing(true);
                    stoppedEditing = false;
                }
            }
            catch (Exception ex)
            {
                retVal = false;
                _logger.Error("Failed executing change detection", ex);
            }
            finally
            {
                //If the code failed in between a operation we need to stop the operation and abort any changes.
                if (startedOperation)
                {
                    wkspcEdit.AbortEditOperation();
                }
                //This condition would arise if the code failed after stopping the edit session. So just restart the edit session.
                if (stoppedEditing)
                {
                    wkspcEdit.StartEditing(true);
                }
            }
            return retVal;
        }

        #endregion Protected Methods

         #region Private Methods

        /// <summary>
        /// Loops through each element in the Passed in ID8List and if a ID8ListItem of type ID8GeoAssoc is found will get the field value from the model name and add the value to the Value to Persist List.
        /// </summary>
        /// <param name="d8List">Root D8List from version Difference</param>
        /// <param name="onlyFeatures">True will process only IFeature. False will process IFeature and IObject</param>
        private void ProcessD8List(ID8List d8List)
        {
            if (d8List is ID8GeoAssoc)
            {
                if (base.AlreadyProcessed(d8List))
                {
                    _logger.Debug("List with Display Name:" + ((ID8ListItem)d8List).DisplayName + " already processed. Skipping processing");
                    return;
                }
                Changedetector.AddChange(d8List);
            }
            //We need not look at the related record. So Once a ID8GeoAssoc is found we need not loop thru' that.
            //QAQC returns the IObjects in the core list if they are edited. This is to improve Performance.
            else
            {
                if (d8List.HasChildren)
                {
                    d8List.Reset();
                    ID8ListItem listItem = null;
                    while ((listItem = d8List.Next()) != null)
                    {
                        _logger.Debug("Processing item with display name:" + listItem.DisplayName);
                        ProcessD8List((ID8List)listItem);
                    }
                }
            }
        }

         #endregion Private Methods
    }
}
