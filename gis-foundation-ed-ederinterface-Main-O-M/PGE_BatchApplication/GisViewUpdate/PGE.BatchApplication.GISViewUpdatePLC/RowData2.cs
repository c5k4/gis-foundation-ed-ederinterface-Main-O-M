using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PGE.BatchApplication.GISViewUpdatePLC
{
      public class RowData2 : IRowData2
    {
        #region Constructor

        /// <summary>
        /// Initializes the instance of <see cref="RowData2"/> object
        /// </summary>
        public RowData2()
        {
            this.Valid = true;
            this.OID = -1;
        }

        #endregion Constructor

        #region Public Properties

        /// <summary>
        /// Stores the Field Names and Values
        /// </summary>
        public Dictionary<string, string> FieldValues
        {
            get;
            set;
        }

        /// <summary>
        /// Indicates whether the record is valid
        /// </summary>
        public bool Valid
        {
            get;
            set;
        }

        /// <summary>
        /// OID of the data, if any
        /// </summary>
        public int OID
        {
            get;
            set;
        }

        /// <summary>
        /// FacilityID of the RowData
        /// </summary>
        public string FacilityID
        {
            get;
            set;
        }

        #endregion Public Properties

        #region Public Overriden Methods

        /// <summary>
        /// Test whether this RowData is the same as another.
        /// Typically just checking the FacilityID is good enough and faster.
        /// </summary>
        /// <param name="obj">A RowData object</param>
        /// <returns>True if they are the same else False</returns>
        public override bool Equals(object obj)
        {
            bool output = false;
            if (obj is IRowData2)
            {
                IRowData2 other = (IRowData2)obj;                
                if (other.FacilityID == this.FacilityID) return true;
                else return false;
                if (other.FieldValues != null && this.FieldValues != null)
                {
                    foreach (string key in this.FieldValues.Keys)
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
        /// Create a hashcode based on the Facility ID
        /// </summary>
        /// <returns>Returns the hashcode</returns>
        public override int GetHashCode()
        {
            if (!string.IsNullOrEmpty(this.FacilityID))
            {
                return this.FacilityID.GetHashCode();
            }
            return base.GetHashCode();
        }

        #endregion Public Overriden Methods
    }

}
