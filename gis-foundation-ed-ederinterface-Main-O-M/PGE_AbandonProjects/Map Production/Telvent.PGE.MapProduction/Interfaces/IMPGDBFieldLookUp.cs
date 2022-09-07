using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Telvent.PGE.MapProduction
{
    /// <summary>
    /// Field Look up for the Map Polygon featureclass from the Geodatabase
    /// </summary>
    public interface IMPGDBFieldLookUp
    {
        /// <summary>
        /// Class Modelname assigned to the Map Poygon featureclass
        /// </summary>
        string MapPolygonModelName
        {
            get;
        }
        /// <summary>
        /// Field modelname assigned on the Map Number field
        /// </summary>
        string MapNumberModelName
        {
            get;
        }
        /// <summary>
        /// Field modelname for the Map Scale field in the Map Polygon featureclass
        /// </summary>
        string MapScaleModelName
        {
            get;
        }
        /// <summary>
        /// Subtypes to use in the Map Polygon featurecalss
        /// </summary>
        List<int> Subtype
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
