using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PGE.Interfaces.Integration.Framework
{
    /// <summary>
    /// Defines a method for transferring data between components
    /// </summary>
    public interface IRowData
    {
        /// <summary>
        /// The unique ID for this set of data
        /// </summary>
        string AssetID { get; set; }
        /// <summary>
        /// Data in string form keyed by the order it should appear
        /// </summary>
        Dictionary<int, string> FieldValues { get; set; }

        /// <summary>
        ///  Data in string form keyed by the order it should appear as key value- EDGIS Rearch Project 2021 edgisrearch-374 -v1t8
        /// </summary>
         Dictionary<string, string> FieldKeyValue{ get; set; }

        /// <summary>
        /// Indicates whether the row from GIS is transformed correctly for SAP.
        /// </summary>
        bool Valid
        {
            get;
            set;
        }

        string ErrorMessage
        {
            get;
            set;
        }

        int OID
        {
            get;
            set;
        }

        int FCID
        {
            get;
            set;
        }

        string FeatureClassName
        {
            get;
            set;
        }
    }
}
