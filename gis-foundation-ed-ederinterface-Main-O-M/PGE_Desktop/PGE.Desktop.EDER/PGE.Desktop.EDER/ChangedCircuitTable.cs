using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Miner.Interop;
using ESRI.ArcGIS.Geodatabase;
using Miner;

namespace PGE.Desktop.EDER.AutoUpdaters
{
    /// <summary>
    /// The facade which handles interaction with the "Changed Circuit" table.
    /// </summary>
    public sealed class ChangedCircuitTable
    {
        const string TableNameChangedCircuit = "PGE_CHANGED_CIRCUIT";
        readonly string[] ChangedCircuitFieldNames =
            new[]
            {
                "CIRCUITID",
                "USERID",
                "POSTDATE",
                "CHANGED_ACTION"
            };

        readonly ITable _tableChangedCircuit;
        static int[] FieldIndexes;

        /// <summary>
        /// Constructs an instance of ChangedCircuitTable
        /// </summary>
        /// <param name="featureWorkspace">The feature workspace to draw the table information from.</param>
        public ChangedCircuitTable(IFeatureWorkspace featureWorkspace)
        {
            _tableChangedCircuit = new MMTableUtilsClass().OpenTable(TableNameChangedCircuit, featureWorkspace);
            if (_tableChangedCircuit == null)
                throw new CancelEditException("Failed to load table " + TableNameChangedCircuit);

            if(FieldIndexes == null)
                FieldIndexes = ChangedCircuitFieldNames
                               .Select(name => _tableChangedCircuit.Fields.FindField(name))
                               .ToArray();

            var missingFields = Enumerable
                                .Range(0, FieldIndexes.Length)
                                .Where(i => FieldIndexes[i] == -1)
                                .ToArray();
            if (missingFields.Length > 0)
            {
                var message = TableNameChangedCircuit
                              + " is missing fields: "
                              + missingFields.Select(i => ChangedCircuitFieldNames[i]).Concatenate(", ");
                throw new CancelEditException(message);
            }
        }

        /// <summary>
        /// Records a deletion in the table.
        /// </summary>
        /// <param name="feederId">The ID of the deleted feature.</param>
        /// <param name="userName">The name used by the person who deleted the feature.</param>
        public void RecordDeleted(object feederId, string userName)
        {

            var fieldValues = new[]
                              {
                                  feederId,
                                  userName,
                                  DateTime.Today,
                                  "DELETE"
                              };

            try
            {
                var newRow = _tableChangedCircuit.CreateRow();
                for (int i = 0; i < FieldIndexes.Length; i++)
                    newRow.Value[FieldIndexes[i]] = fieldValues[i];
                newRow.Store();
            }
            catch (Exception ex)
            {
                var message = "Error creating row in " + TableNameChangedCircuit + ":\r\n" + ex.Message;
                throw new CancelEditException(message);
            }
        }
    }
}
