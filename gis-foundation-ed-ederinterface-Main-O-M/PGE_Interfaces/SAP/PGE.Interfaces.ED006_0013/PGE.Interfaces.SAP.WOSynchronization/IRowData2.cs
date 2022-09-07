using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PGE.Interfaces.SAP.WOSynchronization
{ 
    /// <summary>
    /// Class that holds the data of a row
    /// </summary>
    public interface IRowData2
    {
        /// <summary>
        /// Data in string form keyed by the order it should appear
        /// </summary>
        Dictionary<string, string> FieldValues { get; set; }

        /// <summary>
        /// Indicates whether the row from GIS is transformed correctly for SAP.
        /// </summary>
        bool Valid
        {
            get;
            set;
        }

        int OID
        {
            get;
            set;
        }

        /// <summary>
        /// UniqueID of the data
        /// </summary>
        string FacilityID { get; set; }
    }
}
