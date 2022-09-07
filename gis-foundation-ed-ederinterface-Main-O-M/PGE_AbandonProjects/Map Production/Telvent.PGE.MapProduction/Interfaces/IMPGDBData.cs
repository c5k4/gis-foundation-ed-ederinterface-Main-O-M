using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Telvent.PGE.MapProduction
{
    /// <summary>
    /// Interface that gives access to the GDB Data of the Map Production polygon featureclass
    /// </summary>
    public interface IMPGDBData
    {
        /// <summary>
        /// Gets or sets the Map Number for a given Record
        /// </summary>
        string MapNumber
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the XMin value for the extent of the map polygon in Geodatabase
        /// </summary>
        double XMin
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the YMin value for the extent of the map polygon in Geodatabase
        /// </summary>
        double YMin
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the XMax value for the extent of the map polygon in Geodatabase
        /// </summary>
        double XMax
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the YMax value for the extent of the map polygon in Geodatabase
        /// </summary>
        double YMax
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the MapScale to be used for a given map polygon in Geodatabase
        /// </summary>
        int MapScale
        {
            get;
            set;
        }
        List<string> PolygonCoordinates
        {
            get;
            set;
        }
        /// <summary>
        /// Well Known Text string of the Polygon Object
        /// </summary>
        string WKTPolygon
        {
            get;
        }
        /// <summary>
        /// For each OtherFields property in IMPGDBFieldLookUp will provide the field values for the fields by modelname
        /// </summary>
        Dictionary<string, string> OtherFieldValues
        {
            get;
            set;
        }
        /// <summary>
        /// For each KeyFields property in IMPGDBFieldLookUp will provide the field values for the fields by modelname
        /// </summary>
        Dictionary<string, string> KeyFieldValues
        {
            get;
            set;
        }
        /// <summary>
        /// Gives the Key to be used to uniquely identify this instance IMPGDBData
        /// </summary>
        string Key
        {
            get;
        }
        /// <summary>
        /// Checks and returns the validity of this instance of IMPGDBData
        /// </summary>
        bool IsValid
        {
            get;
        }
        /// <summary>
        /// Called by IsValid to verify the if this instance of IMPGDBData
        /// </summary>
        void Validate();
    }
}
