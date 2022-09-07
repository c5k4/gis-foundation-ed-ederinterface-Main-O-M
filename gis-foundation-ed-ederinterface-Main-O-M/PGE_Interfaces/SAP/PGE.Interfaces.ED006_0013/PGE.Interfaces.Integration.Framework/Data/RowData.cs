using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PGE.Interfaces.Integration.Framework;

namespace PGE.Interfaces.Integration.Framework.Data
{
    /// <summary>
    /// A data structure to store field values of a row and additional information
    /// </summary>
    [Serializable]
    public class RowData:IRowData
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public RowData()
        {
            this.Valid = true;
        }

        /// <summary>
        /// Constructor to quickly initialize the data. Currently this is only used by Unit Tests
        /// </summary>
        /// <param name="sequence">A list of sequences. Order needs to match values.</param>
        /// <param name="values">A list of values. Order needs to match sequences.</param>
        public RowData(int[] sequence, string[] values)
        {
            FieldValues = new Dictionary<int, string>();
            for (int i = 0; i < sequence.Length; i++)
            {
                FieldValues.Add(sequence[i], values[i]);
            }
        }

        /// <summary>
        /// The unique ID of the row.
        /// </summary>
        public string AssetID
        {
            get;
            set;
        }

        /// <summary>
        /// The object ID of the row. Used for debugging
        /// </summary>
        public int OID
        {
            get;
            set;
        }

        /// <summary>
        /// The Feature Class ID of the row. Used for debugging
        /// </summary>
        public int FCID
        {
            get;
            set;
        }

        public string ErrorMessage
        {
            get;
            set;
        }

        public string FeatureClassName
        {
            get;
            set;
        }
        #region IDataRow Members

        /// <summary>
        /// Field values, the key is the sequence of the value for when order matters, ie when the output is a csv file.
        /// </summary>
        public Dictionary<int, string> FieldValues
        {
            get;
            set;
        }


        /// <summary>
        ///  Data in string form keyed by the order it should appear as key value- EDGIS Rearch Project 2021 edgisrearch-374 -v1t8
        /// </summary>
        public  Dictionary<string, string> FieldKeyValue { get; set; }
        #endregion

        /// <summary>
        /// Test whether this RowData is the same as another. Currently only used in Unit Tests.
        /// Typically just checking the AssetID is good enough and faster.
        /// </summary>
        /// <param name="obj">A RowData object</param>
        /// <returns>True if they are the same else False</returns>
        public override bool Equals(object obj)
        {
            bool output = false;
            if (obj is IRowData)
            {
                IRowData other = (IRowData)obj;
                if (other.FieldValues != null && this.FieldValues != null)
                {
                    foreach (int key in this.FieldValues.Keys)
                    {
                        if (other.FieldValues.ContainsKey(key))
                        {
                            if (!this.FieldValues[key].Equals(other.FieldValues[key]))
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                output = true;
            }
            return output;
        }
        /// <summary>
        /// Create a hashcode based on the asset ID
        /// </summary>
        /// <returns>The hashcode</returns>
        public override int GetHashCode()
        {
            if (!String.IsNullOrEmpty(AssetID))
            {
                return AssetID.GetHashCode();
            }
            return base.GetHashCode();
        }

        public bool Valid
        {
            get;
            set;
        }
    }
}
