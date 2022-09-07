using PGE.Interfaces.Integration.Framework;
using PGE.Interfaces.Integration.Framework.Data;
using System;

namespace PGE.Interfaces.SAP.Data
{
    /// <summary>
    /// Data structure for storing SAP specific data. Inherits from RowData
    /// </summary>
    [Serializable]
    public class SAPRowData : RowData, ISAPRowData
    {
        private bool _isValidInternal;
        /// <summary>
        /// Create a new SAPRowData from an existing IRowData object
        /// </summary>
        /// <param name="rowData">The IRowData to copy from</param>
        public SAPRowData(IRowData rowData)
        {
            this.FieldValues = rowData.FieldValues;
            this.AssetID = rowData.AssetID;
            _isValidInternal = rowData.Valid;
            this.Valid = rowData.Valid;
            this.ErrorMessage = rowData.ErrorMessage;
            this.FeatureClassName = rowData.FeatureClassName;
            //Below code is added for EDGIS Rearch Project 2021 edgisrearch-374 -v1t8
            this.FieldKeyValue = rowData.FieldKeyValue;
            if(rowData is RowData){
                this.OID = ((RowData)rowData).OID;
                this.FCID = ((RowData)rowData).FCID;
            }
        }
        /// <summary>
        /// Create a SAPRowData and populate it with data. Only used for unit tests.
        /// </summary>
        /// <param name="sequence">List of field positions. Order must match values order</param>
        /// <param name="values">Data values. Order must match sequences.</param>
        public SAPRowData(int[] sequence, string[] values) :base(sequence,values)
        {
        }
        #region ISAPDataRow Members
        /// <summary>
        /// The type of SAP record this SAPRowData represents
        /// </summary>
        public SAPType SAPType
        {
            get;
            set;
        }
        /// <summary>
        /// The SAP action type for this SAPRowData
        /// </summary>
        public ActionType ActionType
        {
            get
            {
                ActionType action = ActionType.NotApplicable;
                if (FieldValues != null && FieldValues.ContainsKey(1))
                {
                    action = (ActionType)Convert.ToInt32(FieldValues[1].ToCharArray()[0]);
                }

                return action;
            }
            set
            {
                FieldValues[1] = Convert.ToString((char)(int)value);
            }
        }

        #endregion
        /// <summary>
        /// Date record was processed. Used for debugging.
        /// </summary>
        public DateTime DateProcessed { get { return DateTime.Now; } }

        // Making this a method instead of a property in case it gets erroneously serialized in lieu of a regression test
        // Someone with more time on their hands can assess / change appropriately :)
        public bool IsValidInternal()
        {
            return _isValidInternal;
        }
    }
}
