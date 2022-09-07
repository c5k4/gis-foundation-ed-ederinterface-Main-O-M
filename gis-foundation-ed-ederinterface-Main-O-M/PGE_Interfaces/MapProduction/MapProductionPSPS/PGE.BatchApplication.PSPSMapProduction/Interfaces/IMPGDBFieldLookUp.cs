using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PGE.BatchApplication.PSPSMapProduction
{
    /// <summary>
    /// Field Look up for the Map Polygon featureclass from the Geodatabase
    /// </summary>
    public interface IMPGDBFieldLookUp
    {
        /// <summary>
        /// Class Modelname assigned to the Map Poygon featureclass
        /// </summary>
        string PriOHConductorModelName
        {
            get;
        }    

        /// <summary>
        /// Field modelname for the Circuit Id in PriOHConductor layer
        /// </summary>
        string CircuitIdModelName
        {
            get;
        }
        /// <summary>
        /// Field modelname for the Circuit Name in PriOHConductor layer
        /// </summary>
        string CircuitNameModelName
        {
            get;
        }
        /// <summary>
        /// PSPS Segment Name
        /// </summary>
        string PSPSSegmentNameModelName
        {
            get;
        }
        /// <summary>
        /// Modelnames for other fields that are optional to be stored
        /// </summary>
        List<string> OtherFieldModelNames
        {
            get;
        }
        /// <summary>
        /// Modelnames for the fields that form the key for a given map polygon feature excluding the MapNumber field.
        /// The modelnames added here should also be added to OtherfieldsModelname
        /// </summary>
        List<string> KeyFieldModelNames
        {
            get;
        }

    }
}
