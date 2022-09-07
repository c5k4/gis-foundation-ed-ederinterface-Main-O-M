using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Reflection;

using ESRI.ArcGIS.Geodatabase;
using Miner.ComCategories;
using Miner.Interop.Process;

using PGE.Common.Delivery.Diagnostics;
using PGE.Common.Delivery.Process.BaseClasses;
using System.Windows.Forms;

namespace PGE.Desktop.EDER.Subtasks
{
    /// <summary>
    /// A subtask to record the changes in Circuits / Substations
    /// </summary>
    [ComVisible(true)]
    [Guid("24196313-4601-494D-8B84-7256346851FC")]
    [ProgId("PGE.Desktop.EDER.DisplaySAPErrorsSubtask")]
    [ComponentCategory(ComCategory.MMPxSubtasks)]
    public class DisplaySAPErrorsSubtask : BaseSessionSubtask
    {
        #region Private Variables
        /// <summary>
        /// Logs the messages
        /// </summary>
        private const string SAPERRORSTABLENAME = "";
        private const string SAPERRORS_VERSION_FIELDNAME = "";
        private const string SAPERRORS_ERRORS_FIELDNAME = "";
        #endregion Private Variables

        #region Constructor

        public DisplaySAPErrorsSubtask()
            : base("Display SAP Errors")
        { }

        #endregion Constructor

        #region Overridden Methods

        protected override bool InternalEnabled(Miner.Interop.Process.IMMPxNode pPxNode)
        {
            return base.InternalEnabled(pPxNode);
        }

        protected override bool InternalExecute(Miner.Interop.Process.IMMPxNode pPxNode)
        {
            bool executed = false;            

            IWorkspace wSpace = Miner.Geodatabase.Edit.Editor.EditWorkspace;
            if (wSpace == null)
            {
                MessageBox.Show("There are no SAP errors. Please check the Geodatabase Manager log for more information.", "SAP Errors", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return executed;
            }
            //Validate and open errors table
            if ((wSpace as IWorkspace2).get_NameExists(esriDatasetType.esriDTTable, SAPERRORSTABLENAME) == false) return executed;
            ITable errorsTable = (wSpace as IFeatureWorkspace).OpenTable(SAPERRORSTABLENAME);
            //Get the errors from the current version
            string versionName = (wSpace as IVersion).VersionName;
            //Get the rows where VersionName= current verison name
            IQueryFilter filter = new QueryFilterClass();
            filter.WhereClause = SAPERRORS_VERSION_FIELDNAME + "='" + versionName.ToUpper() + "'";
            ICursor errorCursor = errorsTable.Search(filter, true);
            StringBuilder errors = new StringBuilder();
            IRow errorRow = null;
            int errorFldIx = errorsTable.FindField(SAPERRORS_ERRORS_FIELDNAME);
            while ((errorRow = errorCursor.NextRow()) != null)
            {
                errors.AppendLine(Convert.ToString(errorRow.get_Value(errorFldIx)));
            }
            string[] errorList = errors.ToString().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            if (errorList == null || errorList.Length < 1) return executed;
            SAPErrorsUI errorUI = new SAPErrorsUI(errorList);
            errorUI.ShowDialog();
            return executed = true;
        }

        #endregion Overridden Methods
    }
}
