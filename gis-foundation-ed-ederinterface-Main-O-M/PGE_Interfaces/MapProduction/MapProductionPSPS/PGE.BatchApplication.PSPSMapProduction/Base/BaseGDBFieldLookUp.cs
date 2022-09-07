using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PGE.BatchApplication.PSPSMapProduction
{
    /// <summary>
    /// Base implemnentation IMMGDBFieldLookUp for sample
    /// </summary>
    public abstract class BaseGDBFieldLookUp:IMPGDBFieldLookUp
    {
        /// <summary>
        /// Map Polygon class model name
        /// </summary>
        public virtual string PriOHConductorModelName
        {
            get { return "PRIOHCONDUCTOR"; }
        }
        /// <summary>
        /// Circuit Id field modelname
        /// </summary>
        public virtual string CircuitIdModelName
        {
            get { return "CIRCUITID"; }
        }
        /// <summary>
        /// Circuit Name field modelname
        /// </summary>
        public virtual string CircuitNameModelName
        {
            get { return "CIRCUITNAME"; }
        }
        /// <summary>
        /// PSPS Segment Name field modelname
        /// </summary>
        public virtual string PSPSSegmentNameModelName
        {
            get { return "PSPS_SEGMENT"; }
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
