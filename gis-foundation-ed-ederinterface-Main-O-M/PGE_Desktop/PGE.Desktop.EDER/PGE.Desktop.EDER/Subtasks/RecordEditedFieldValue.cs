using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PGE.Common.Delivery.Process.BaseClasses;
using Miner.Interop.Process;
using System.Runtime.InteropServices;
using Miner.ComCategories;
using Miner.Interop;
using PGE.Common.Delivery.Geodatabase;
using Miner.Geodatabase.Edit;
using PGE.Common.Delivery.ArcFM;
using ESRI.ArcGIS.Geodatabase;
using PGE.Common.Delivery.Framework;
using System.Windows.Forms;
using log4net;
using System.Reflection;
using PGE.Common.Delivery.Diagnostics;

namespace PGE.Desktop.EDER.Subtasks
{
    /// <summary>
    /// Subtask to record field value from field with modelname from all the edited objects in the session and persist it to a database table.
    /// The Database table should be assigned a model name and the model name should be configured in the database. 
    /// The table structure is fixed it should have 2 Text fields named "VERSION" and "VALUE"
    /// </summary>
    [ComVisible(true)]
    [Guid("B7F77E57-A659-4C53-8D10-BFD1694F67BA")]
    [ProgId("PGE.Desktop.Subtask.RecordEditedFieldValue")]
    [ComponentCategory(ComCategory.MMPxSubtasks)]
    public class RecordEditedFieldValue : BaseDesktopVersionDifferenceSubtask
    {
        #region Private Memebers
        private static Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private const string _versionNameField = "Version";
        private const string _fieldValueField = "Value";
        private const string _modefiedDateField = "ModifiedDate";
        private const string _modelName = "FieldModelName";
        private const string _tableModelName = "TableModelName";
        private const string _processFeatureOnly = "FeatureOnly";
        private List<string> _valuesToPersist = null;
        #endregion

        /// <summary>
        /// Constructor. Adds the parameters needed for this subtask and initializes the Subtask
        /// </summary>
        public RecordEditedFieldValue()
            : base("PGE Record Edited Field Value")
        {
            base.AddParameter(_modelName, "Modelname of the field that should be recorded.");
            base.AddParameter(_tableModelName, "Model Name of the table into which the changes should be recorded.");
            base.AddParameter(_processFeatureOnly, "'TRUE' if the subtask has to process just features, else 'FALSE'.");
        }

        /// <summary>
        /// Executes the logic to populate the field values into a staging table based on the configured modelname and configured table name.
        /// </summary>
        /// <param name="pPxNode">Session Node</param>
        /// <returns>True if the Subtask is completed successfully</returns>
        protected override bool InternalExecute(IMMPxNode pPxNode)
        {
            IRow rowToPersist = null;
            bool startedOperation = false;
            bool stoppedEditing = false;
            //Get the Workspace and start an Edit operation.
            //Since this is run from Subtask assuming there is no Open Operation already.
            IWorkspaceEdit wkspcEdit = (IWorkspaceEdit)Editor.EditWorkspace;
            try
            {
                //Obtain the parameters as configured in the subtask.
                string tableModelName = base.GetParameter(_tableModelName);
                string modelName = base.GetParameter(_modelName);
                string featuresOnly = base.GetParameter(_processFeatureOnly);
                _logger.Debug("Getting configured parameters");
                _logger.Debug(string.Format("Paramaters obtained. {0}:{1},{2}:{3},{4}:{5}", _tableModelName, tableModelName, _modelName, modelName, _processFeatureOnly, featuresOnly));
                //If any of the required configuration is not configured fail and return.
                if (string.IsNullOrEmpty(tableModelName) || string.IsNullOrEmpty(modelName))
                {
                    MessageBox.Show("Parameter configuration error", "PGE Subtasks", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
                //First check if the table to persist the changes is found if not do not waste time on getting teh version difference and other expensive stuff
                _logger.Debug("Opening the table using configured TableName parameter:" + tableModelName);
                ITable tableToPersist = ModelNameFacade.ObjectClassByModelName(Editor.EditWorkspace, tableModelName) as ITable;
                _logger.Debug("Got table for tablename:" + tableModelName);
                if (tableToPersist != null)
                {
                    wkspcEdit.StartEditOperation();
                    //indicator for the finally block to abort operation if the code fails before the edit operation is stopped
                    startedOperation = true;
                    bool onlyFeatures = string.IsNullOrEmpty(featuresOnly) || featuresOnly.Equals("FALSE", StringComparison.InvariantCultureIgnoreCase) ? false : true;
                    _logger.Debug("Getting version difference from Dekstop Version Difference class for Editor.EditWorkspace");
                    ID8List versionDifference = DesktopVersionDifference.GetVersionDifference(Editor.EditWorkspace);
                    versionDifference.Reset();
                    _logger.Debug("Processing features from Version Difference");
                    //Instantiate the List to hold the values to persist
                    _valuesToPersist = new List<string>();
                    //Instantiate the List that holds the already processed features to avoid D8ListItems that are looped multiple times because of relationships
                    _processedList = new List<string>();
                    //Process the Version Difference in Source and populate the Values
                    ProcessD8List(versionDifference, onlyFeatures);
                    //Process the Deleted features
                    ProcessD8List(DesktopVersionDifference.GetVersionDifference(Editor.EditWorkspace, new esriDifferenceType[] { esriDifferenceType.esriDifferenceTypeDeleteNoChange, esriDifferenceType.esriDifferenceTypeDeleteUpdate }), onlyFeatures);
                    //Create Row
                    rowToPersist = tableToPersist.CreateRow();
                    _logger.Debug(string.Format("Created a new row with OID:{0} on table :{1}", rowToPersist.OID, ((IDataset)tableToPersist).Name));
                    _logger.Debug("setting version Name");
                    rowToPersist.set_Value(rowToPersist.Fields.FindField(_versionNameField), ((IVersion)Editor.EditWorkspace).VersionName);
                    _logger.Debug("Setting field Value");
                    rowToPersist.set_Value(rowToPersist.Fields.FindField(_fieldValueField), string.Join(",", _valuesToPersist.ToArray()));
                    _logger.Debug("Setting date Value");
                    rowToPersist.set_Value(rowToPersist.Fields.FindField(_modefiedDateField), DateTime.Today.ToShortDateString());
                    rowToPersist.Store();
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
                    return true;
                }
                else
                {
                    _logger.Debug("Table with modelname:" + _tableModelName + "not found");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Failed processing the subtask:" + this._Name, ex);
                return false;
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
                if (rowToPersist != null)
                {
                    while (Marshal.ReleaseComObject(rowToPersist) > 0) { } 
                }
            }
        }

        /// <summary>
        /// Loops through each element in the Passed in ID8List and if a ID8ListItem of type ID8GeoAssoc is found will get the field value from the model name and add the value to the Value to Persist List.
        /// </summary>
        /// <param name="d8List">Root D8List from version Difference</param>
        /// <param name="onlyFeatures">True will process only IFeature. False will process IFeature and IObject</param>
        private void ProcessD8List(ID8List d8List,bool onlyFeatures)
        {
            if (d8List is ID8GeoAssoc)
            {
                if (base.AlreadyProcessed(d8List))
                {
                    _logger.Debug("List with Display Name:" + ((ID8ListItem)d8List).DisplayName + " already processed. Skipping processing");
                    return;
                }
                string fieldValue = GetFieldData(d8List, onlyFeatures);
                if (!_valuesToPersist.Contains(fieldValue) && !string.IsNullOrEmpty(fieldValue))
                {
                    _logger.Debug("Adding value:" + fieldValue + " to the list of values to persist");
                    _valuesToPersist.Add(fieldValue);
                }
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
                        ProcessD8List((ID8List)listItem, onlyFeatures);
                    }
                }
            }
        }

        private string GetFieldData(ID8List d8List,bool onlyFeatures)
        {
            string retVal = string.Empty;
            ID8GeoAssoc geoAssoc = (ID8GeoAssoc)d8List;
            IRow row = geoAssoc.AssociatedGeoRow;
            if (onlyFeatures && !(row is IFeature))
            {
                return string.Empty;
            }
            int fieldIdx=ModelNameFacade.FieldIndexFromModelName(((IObject)row).Class,base.GetParameter(_modelName));
            if (fieldIdx>-1)
            {
                retVal=row.get_Value(fieldIdx).Convert(string.Empty);
            }
            return retVal;
        }
    }
}
