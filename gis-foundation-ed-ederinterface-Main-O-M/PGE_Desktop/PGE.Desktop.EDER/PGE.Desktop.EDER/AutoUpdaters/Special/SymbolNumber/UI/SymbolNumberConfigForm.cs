using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.Reflection;

using ESRI.ArcGIS.Geodatabase;

using PGE.Common.Delivery.Framework;
using PGE.Common.Delivery.Systems;
using PGE.Common.Delivery.Diagnostics;

using PGE.Desktop.EDER.AutoUpdaters;
using PGE.Desktop.EDER.SymbolNumber.Schema;

namespace PGE.Desktop.EDER.SymbolNumber.UI
{
    /// <summary>
    /// A form that allows user to edit Symbol number configuration documents stored in the database.
    /// </summary>
    public partial class SymbolNumberConfigForm : Form
    {

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SymbolNumberConfigForm"/> class.
        /// Uses model name facade to locate the symbol number rules table and finds the index of the config 
        /// field.
        /// </summary>
        /// <param name="workspace">The workspace.</param>
        public SymbolNumberConfigForm(IWorkspace workspace, string rulesTableModelName)
        {
            //this.rulesTableName = rulesTableName;
            this.workspace = workspace;

            ruleTable = ModelNameFacade.ObjectClassByModelName(workspace, rulesTableModelName) as ITable;
            xmlFieldIx = ModelNameFacade.FieldIndexFromModelName((IObjectClass)ruleTable, SchemaInfo.Electric.FieldModelNames.SymbolNumberXML);
            fcNameFieldIx = ModelNameFacade.FieldIndexFromModelName((IObjectClass)ruleTable, SchemaInfo.Electric.FieldModelNames.SymbolNumberFCName);

            xmlFieldName = ModelNameFacade.FieldNameFromModelName((IObjectClass)ruleTable, SchemaInfo.Electric.FieldModelNames.SymbolNumberXML);
            fcNameFieldName = ModelNameFacade.FieldNameFromModelName((IObjectClass)ruleTable, SchemaInfo.Electric.FieldModelNames.SymbolNumberFCName);

            maxXMLLength = ruleTable.Fields.get_Field(xmlFieldIx).Length;
            InitializeComponent();
        }
        #endregion Constructor

        #region Private
        /// <summary>
        /// logger to log all the information, warning, and errors.
        /// </summary>
        private static readonly PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        /// <summary>
        /// Index of the XML field.  
        /// </summary>
        private int xmlFieldIx = 0;

        /// <summary>
        /// Name of the XML field.
        /// </summary>
        private string xmlFieldName = "";

        /// <summary>
        /// Index of the feature class name field.
        /// </summary>
        private int fcNameFieldIx = 0;

        /// <summary>
        /// Name of the feature class name field.
        /// </summary>
        private string fcNameFieldName = "";

        /// <summary>
        /// Workspace containing the PGS_SYMBOL_NUMBER_RULES tables.  Set by constructor.
        /// </summary>
        private IWorkspace workspace;

        /// <summary>
        ///  PGS_SYMBOL_NUMBER_RULES table.
        /// </summary>
        private ITable ruleTable;

        /// <summary>
        /// Maximum length of XML configuration documents.
        /// </summary>
        private int maxXMLLength = 0;

        /// <summary>
        /// Handles the Click event of the cancelButton control.
        /// Closes the form without saving changes.  
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void cancelButton_Click(object sender, EventArgs e)
        {
            //check if rule is modified
            if (ruleTextBox.Modified)
            {
                _logger.Debug("Rule is modified, confirm cancellation with user.");
                //confirm action and inform user that changes will be discarded.
                DialogResult result = MessageBox.Show("Changes will be discarded.  Do you wish to continue?", "Warning", MessageBoxButtons.YesNo);

                if (result == DialogResult.No)
                {
                    _logger.Debug("Rule is modified, cancelled by user.");
                    return;
                }
                _logger.Debug("Rule is modified, closing confirmed.");
            }

            this.Hide();
        }

        /// <summary>
        /// Handles the Load event of the SymbolNumberConfigForm control.
        /// 
        /// Clear the UI and load feature class names.  
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void SymbolNumberConfigForm_Load(object sender, EventArgs e)
        {
            //reset the config UI
            resetUI();

            //clear and load Dataset from current workspace
            datasetComboBox.BeginUpdate();
            datasetComboBox.Items.Clear();
            datasetComboBox.Items.Add("<Select Dataset>");

            //clear the featureclass combo and then add the very first value as <No Feature...
            featureClassComboBox.BeginUpdate();
            featureClassComboBox.Items.Clear();
            featureClassComboBox.Items.Add("<No Feature Class to Select>");
            featureClassComboBox.EndUpdate();

            IEnumDataset datasets = workspace.get_Datasets(esriDatasetType.esriDTFeatureDataset);
            try
            {
                _logger.Debug("Loading list of feature datasets in dataset combobox.");
                datasets.Reset();

                IDataset dataset = null;
                while ((dataset = datasets.Next()) != null)
                {
                    //append all the feature dataset to dataset combo box.
                    _logger.Debug("Adding dataset " + dataset.Name + ".");
                    datasetComboBox.Items.Add(new DatasetListItem(dataset));
                }
            }
            finally
            {
                //finally release the dataset com object
                _logger.Debug("Rleasing the COM object IEnumDataset");
                while (Marshal.ReleaseComObject(datasets) > 0) { }
            }

            datasetComboBox.EndUpdate();
            datasetComboBox.SelectedIndex = 0;
            featureClassComboBox.SelectedIndex = 0;
            datasetComboBox.Focus();
        }

        /// <summary>
        /// Resets the UI.  Clear the feature class name combo box and disable the save button.
        /// </summary>
        private void resetUI()
        {
            //reset the UI
            ruleTextBox.Clear();
            saveButton.Enabled = false;
            ruleTextBox.Enabled = false;
            validateButton.Enabled = false;
        }

        /// <summary>
        /// Loads the feature classes in the selected dataset.
        /// </summary>
        /// <param name="dataset">The dataset.</param>
        private void loadFeatureClasses(IDataset dataset)
        {
            //reset the UI
            resetUI();

            featureClassComboBox.BeginUpdate();
            featureClassComboBox.Items.Clear();

            if (datasetComboBox.SelectedIndex == 0)
            {
                //dataset combo box is selected very first option [Select]
                featureClassComboBox.Items.Add("<No Feature Class to Select>");
                featureClassComboBox.EndUpdate();
                featureClassComboBox.SelectedIndex = 0;
                return;
            }
            featureClassComboBox.Items.Add("<Select Feature Class>");
            IDataset subset = null;

            IEnumDataset subsets = dataset.Subsets;
            try
            {
                //reset the subset
                subsets.Reset();
                _logger.Debug("Loading feature class combobox for selected feature dataset.");
                while ((subset = subsets.Next()) != null)
                {
                    //check if current subset is type ofIfeatureClass
                    if (subset is IFeatureClass)
                    {
                        //add the datasetListItem to combobox
                        _logger.Debug(string.Format("Adding Featureclass {0} for {1} feature dataset.", subset.Name, dataset.Name));
                        featureClassComboBox.Items.Add(new DatasetListItem(subset));
                    }
                }
            }
            finally
            {
                //release the COM Object
                _logger.Debug("Releasing the COM object subsets.");
                while (Marshal.ReleaseComObject(subsets) > 0) { }
            }

            featureClassComboBox.EndUpdate();
            featureClassComboBox.SelectedIndex = 0;
        }

        /// <summary>
        /// Loads a rule definition from the database and copies rule XML to the form's textbox.
        /// </summary>
        /// <param name="featureClassName">Name of the feature class.</param>
        private void loadRuleDefinition(IDataset dataset)
        {
            //reset the UI
            resetUI();
            if (featureClassComboBox.SelectedIndex == 0)
            {
                //by reset ui textbox is already cleared and disabled
                return;
            }
            ruleTextBox.Enabled = true;
            validateButton.Enabled = true; 
            ICursor cursor = null;
            try
            {
                // Create the query filter and read XML configuration in to user interface. 
                IQueryFilter query = new QueryFilterClass() as IQueryFilter;
                query.WhereClause = string.Format("{1}='{0}'", dataset.Name, fcNameFieldName);
                _logger.Debug(string.Format("QueryFilter with where clause: {0}. is prepared.", query.WhereClause));
                cursor = ruleTable.Search(query, true);

                IRow row = null;
                if ((row = cursor.NextRow()) != null)
                {
                    //set the ruleTextBox value to xmlconfig.
                    _logger.Debug(string.Format("XML configuration of feature class {0} found.", dataset.Name));
                    ruleTextBox.Text = StringFacade.GetDefaultNullString(row.get_Value(xmlFieldIx), string.Empty);
                    deleteButton.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                _logger.Debug("Error occurred while loading XML configuration.", ex);
            }
            finally
            {
                // Release all COM Object cursor references.
                _logger.Debug("Releasing the COM object cursor.");
                if (cursor != null)
                {
                    while (Marshal.ReleaseComObject(cursor) > 0) { }
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the saveButton.  Validates the XML and saves valid XML configuration 
        /// to the database or produces an error message.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void saveButton_Click(object sender, EventArgs e)
        {
            //check if selected feature class name is not null or empty
            string featureClassName = featureClassComboBox.SelectedItem.ToString();
            if (featureClassName == null || featureClassName.Length == 0)
            {
                _logger.Debug("Selected feature class name is <Null> or empty.");
                return;
            }
            //check if ruletextbox length is not greater than MaxLength
            if (ruleTextBox.Text.Length > maxXMLLength)
            {
                _logger.Warn("Document length exceeds maximum field width of " + maxXMLLength + ".");
                return;
            }

            ICursor cursor = null;

            try
            {
                IQueryFilter query = new QueryFilterClass() as IQueryFilter;
                query.WhereClause = string.Format("{1}='{0}'", featureClassName, fcNameFieldName);
                _logger.Debug(string.Format("Query filter with where clause {0} is prepared.", query.WhereClause));
                cursor = ruleTable.Search(query, false);

                IRow row = null;

                if ((row = cursor.NextRow()) != null)
                {
                    //rule already exist it need to be updated.
                    _logger.Debug("Rule already exist, updating rule with new definition.");
                    row.set_Value(xmlFieldIx, ruleTextBox.Text);
                }
                else
                {
                    //rule doesn't exist it need to be added.
                    _logger.Debug("Rule doesn't exist, Adding rule with new definition.");
                    row = ruleTable.CreateRow();
                    row.set_Value(fcNameFieldIx, featureClassName);
                    row.set_Value(xmlFieldIx, ruleTextBox.Text);
                }

                //finally call store to save / commit values in database.
                row.Store();

                //reset the old definition of rulefor same feature class
                _logger.Debug("Removing rule if already cached for " + featureClassName + ".");
                EvaluationEngine.Instance.ResetFeatureClass(featureClassName);
                ruleTextBox.Modified = false;
                saveButton.Enabled = false;
                deleteButton.Enabled = true;
            }
            finally
            {
                // Release all cursor references.
                _logger.Debug("Releasing the COM Object cursor.");
                while (Marshal.ReleaseComObject(cursor) > 0) { }
            }
        }

        /// <summary>
        /// Handles the TextChanged event of the ruleTextBox control.  Disables the save button until the 
        /// configuration is validated.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ruleTextBox_TextChanged(object sender, EventArgs e)
        {
            //if save button is already enabled 
            //jus disable it so before saving changes we can validate it
            if (saveButton.Enabled)
            {
                //make save button disabled
                _logger.Debug("Making save button disabled.");
                saveButton.Enabled = false;
            }
        }

        /// <summary>
        /// Handles the Click event of the validateButton control.
        /// Parse the XML in the text editor and enable the save button if the xml is well formed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void validateButton_Click(object sender, EventArgs e)
        {
            try
            {
                //check if new rule is valid
                _logger.Debug("Validating the XML rule.");
                if (validateRuleXML())
                {
                    //enable the save button
                    _logger.Debug("Enabling the save button.");
                    saveButton.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                _logger.Warn("Exception occurred while validating the XML rule.", ex);
                System.Diagnostics.Trace.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// Executes validation and and displays error messages. 
        /// </summary>
        /// <returns></returns>
        private bool validateRuleXML()
        {
            bool valid = true;

            // Memory stream to read text from UI.
            MemoryStream xmlStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(ruleTextBox.Text));
            List<string> warnings = new List<string>();
            SchemaErrorList errorList = SymbolNumberRules.ValidateXML(xmlStream);
            foreach (string warning in errorList.Warnings)
            {
                //add the warnings to the list
                warnings.Add(warning);
            }

            StringBuilder message = new StringBuilder();
            if (ruleTextBox.Text.Length > maxXMLLength)
            {
                //log the message for max length
                _logger.Debug("XML Configuration exceeds maximum length of " + maxXMLLength + " characters.");
                MessageBox.Show("XML Configuration exceeds maximum length of " + maxXMLLength + " characters.", "Validation failed!!!");
                return false;
            }

            //check if no errors are reported
            if (errorList.Errors.Count == 0)
            {
                _logger.Debug("No error reported while validating XML rule.");
                message.Append("Validation successful.");
            }
            else
            {
                _logger.Debug("Errors are reported while validating XML rule.");
                message.Append("Validation failed schema check:");
                valid = false;
                foreach (string error in errorList.Errors)
                {
                    message.AppendLine();
                    message.AppendLine(error);
                }
                _logger.Debug("Errors:" + message.ToString());
                MessageBox.Show(message.ToString(),"Validation failed!!!");
                return false;
            }

            try
            {

                xmlStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(ruleTextBox.Text));
                // Deserialize XML and listen for exceptions.
                SymbolNumberRules criteria = SymbolNumberRules.Deserialize(xmlStream);
                //if (criteria.featureClass == null || criteria == null)
                //{
                //    MessageBox.Show("Invalid XML.");
                //}
                List<string> modelNames = criteria.GetModelNames();
                IFeatureClass fc = (featureClassComboBox.SelectedItem as DatasetListItem).Dataset as IFeatureClass;
                //check if all the model names are valid
                foreach (string modelName in modelNames)
                {
                    int fieldIndex = 0;
                    try
                    {

                        fieldIndex = ModelNameFacade.FieldIndexFromModelName(fc, modelName);
                        if (fieldIndex == -1)
                        {
                            //append the warning for model name doesn't exist
                            warnings.Add(string.Format(modelName));
                            _logger.Debug("Field modelname:" + modelName + " doesn't exist.");
                        }
                    }
                    catch
                    {
                        //add the model name which caused exception
                        _logger.Debug("Field modelname:" + modelName + " caused exception.");
                        warnings.Add(string.Format(modelName));
                    }
                }
                //check if warning count isgreater than 0
                if (warnings.Count > 0)
                {
                    //append all the model names
                    message.Append("\n\nWarning: Configuration contains following model names which can not be resolved.  Additional configuration may be necessary.\n\nFeature Class: ");
                    message.Append(fc.AliasName);
                    foreach (string warning in warnings)
                    {
                        message.Append('\n');
                        message.Append(warning);
                    }
                }

                // show warnings
                string msgTitle = warnings.Count > 0 ? "Warnings" : "Validation Successful";
                _logger.Debug(msgTitle+":"+message.ToString());
                MessageBox.Show(message.ToString(), msgTitle);
            }
            catch (InvalidOperationException ex)
            {
                // XML validation failed notify the user.
                _logger.Debug("Exception occurred while validating XML rule.", ex);
                MessageBox.Show("Validation failed:\n" + ex.InnerException.Message, "Validation failed!!!");
                valid = false;
            }
            finally
            {
                // Cleanup IO streams.
                if (xmlStream != null)
                {
                    _logger.Debug("Closing the xml stream.");
                    xmlStream.Close();
                }
            }

            return valid;
        }

        /// <summary>
        /// Handles the SelectionChangeCommitted event of the featureClassComboBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void featureClassComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            //load the definition from rules table and load it in ruleTextbox
            _logger.Debug("Loading rule definition for feature class:" + featureClassComboBox.SelectedItem.ToString());
            deleteButton.Enabled = false;
            //handle select featureclass case
            if (featureClassComboBox.SelectedIndex == 0)
            {
                //just pass null value instead of valid dataaset
                loadRuleDefinition(null);
            }
            else
            {
                loadRuleDefinition((featureClassComboBox.SelectedItem as DatasetListItem).Dataset);
            }
            _logger.Debug("Successfully Loaded rule definition for feature class:" + featureClassComboBox.SelectedItem.ToString());
        }

        /// <summary>
        /// Handles the SelectionChangeCommitted event of the datasetComboBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void datasetComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            //load feature class for selected feature dataset
            _logger.Debug("Loading feature classes for Feature dataset:" + datasetComboBox.SelectedItem.ToString());
            deleteButton.Enabled = false;
            if (datasetComboBox.SelectedIndex == 0)
            {
                loadFeatureClasses(null);
            }
            else
            {
                loadFeatureClasses((datasetComboBox.SelectedItem as DatasetListItem).Dataset);
            }
            _logger.Debug("Successfully loaded feature classes for Feature dataset:" + datasetComboBox.SelectedItem.ToString());
        }

        /// <summary>
        /// Handles the VisibleChanged event of the SymbolNumberConfigForm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void SymbolNumberConfigForm_VisibleChanged(object sender, EventArgs e)
        {
            //check if form isvisible
            if (Visible)
            {
                //get the selected featureclass
                DatasetListItem selected = featureClassComboBox.SelectedItem as DatasetListItem;

                //check if selected feature class is not null
                if (selected != null)
                {
                    //load the xml rule definition for selected feature class
                    _logger.Debug("Loading XML rule definition for selected feature class:" + selected.Dataset.Name);
                    if (featureClassComboBox.SelectedIndex == 0)
                    {
                        loadRuleDefinition(null);
                    }
                    else
                    {
                        loadRuleDefinition(selected.Dataset);
                    }
                    _logger.Debug("successfully loaded XML rule definition for selected feature class:" + selected.Dataset.Name);
                }
                datasetComboBox.Focus();
            }
        }

        /// <summary>
        /// Handes the KeyDown event of the ruleTextBox
        /// </summary>
        /// <param name="sender">The Source of the event.</param>
        /// <param name="e">The <see cref="System.KeyEventArgs"/> instance containing the event data.</param>
        private void ruleTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            //check if ctrl+A key is pressed
            if (e.Control & e.KeyCode == Keys.A)
            {
                //highlight all the text
                _logger.Debug("Ctrl+A key pressed on ruleTextBox executing select all.");
                ruleTextBox.SelectAll();
            }
        }

        #endregion Private

        #region Override for Form

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Form.FormClosing"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.FormClosingEventArgs"/> that contains the event data.</param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            //check for the closing reason
            if (e.CloseReason == CloseReason.UserClosing)
            {
                //cancel the closing
                e.Cancel = true;
                //check if ruleTextBox is modified
                if (ruleTextBox.Modified)
                {
                    _logger.Debug("Rule test box is modified and need to ask used regarding changes will be discarded if closed without saving.");
                    DialogResult result = MessageBox.Show("Changes will be discarded.  Do you wish to continue?", "Warning", MessageBoxButtons.YesNo);
                    if (result == DialogResult.No)
                    {
                        //if user cancells close action then no need to hide
                        _logger.Debug("User cancelled the close action.");
                        return;
                    }
                }
                //hide the form no need to dispose and recreate in case opened again
                _logger.Debug("Hiding the form instead of closing.");
                this.Hide();
            }
            else
            {
                //execute base.OnFormClosing since its not because user is closing this form.
                _logger.Debug("Closing reason is not UserClosing so executing base.OnFormClosing.");
                base.OnFormClosing(e);
            }

        }

        #endregion Override for Form

        private void deleteButton_Click(object sender, EventArgs e)
        {
            //check if selected feature class name is not null or empty
            string featureClassName = featureClassComboBox.SelectedItem.ToString();
            if (featureClassName == null || featureClassName.Length == 0)
            {
                _logger.Debug("Selected feature class name is <Null> or empty.");
                return;
            }
            
            //askuser first to confirm if user wants to delete the configuration for selected featureclass
            if (MessageBox.Show("Do you want to delete the configuration for feature class:" + featureClassName + "?", "Delete Configuration?", MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
            {
                _logger.Debug("User cancelled the configuration deletion for feature class:"+ featureClassName+".");
                return;
            }

            try
            {
                IQueryFilter query = new QueryFilterClass() as IQueryFilter;
                query.WhereClause = string.Format("{1}='{0}'", featureClassName, fcNameFieldName);
                _logger.Debug(string.Format("Query filter with where clause {0} is prepared.", query.WhereClause));
                ruleTable.DeleteSearchedRows(query);

                //reset the old definition of rulefor same feature class
                _logger.Debug("Removing rule if already cached for " + featureClassName + ".");
                EvaluationEngine.Instance.ResetFeatureClass(featureClassName);
                ruleTextBox.Text = "";
                ruleTextBox.Modified = false;
                saveButton.Enabled = false;
                deleteButton.Enabled = false;
            }
            catch (Exception ex)
            {
                _logger.Debug("Error occurred while deleting. FeatureClass:" + featureClassName, ex);
            }
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            _logger.Debug("User clicked close on UI form. Now hiding the form.");
            this.Hide();
        }

    }
}
