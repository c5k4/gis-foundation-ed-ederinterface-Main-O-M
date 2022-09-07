using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Telvent.PGE.MapProduction
{
    /// <summary>
    /// Base implemnentation IMMGDBFieldLookUp for sample
    /// </summary>
    public abstract class BaseGDBFieldLookUp:IMPGDBFieldLookUp
    {
        /// <summary>
        /// Map Polygon class model name
        /// </summary>
        public virtual string MapPolygonModelName
        {
            get { return "MAPPRODUCTION"; }
        }
        /// <summary>
        /// Map number field modelname
        /// </summary>
        public virtual string MapNumberModelName
        {
            get { return "MAPNUMBER"; }
        }
        /// <summary>
        /// Mapscale field modelname
        /// </summary>
        public virtual string MapScaleModelName
        {
            get { return "MAPSCALE"; }
        }
        /// <summary>
        /// Subtypes to use. Returns a new instance of <see cref="List<int>"/>
        /// </summary>
        public virtual List<int> Subtype
        {
            get { return new List<int>(); }
        }
        /// <summary>
        /// abstract implementaiton of the Otherfieldmodelnames
        /// </summary>
        public abstract List<string> OtherFieldModelNames
        {
            get;
        }
        /// <summary>
        /// abstract implementaiton of the Keyfieldmodelnames
        /// </summary>
        public abstract List<string> KeyFieldModelNames
        {
            get;
        }
    }
}
