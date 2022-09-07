using PGE.BatchApplication.PSPSMapProduction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PGE.BatchApplication.PreProcessing
{
    /// <summary>
    /// Implementation of PGE specific IMPGDBFieldLoopUp interface.
    /// </summary>
    public class PGEGDBFieldLookUp:IMPGDBFieldLookUp
    {
        public struct PGEGDBFields
        {
            public const string CircuitIdModelName = "CIRCUITID";
            public const string CircuitNameModelName = "CIRCUITNAME";
            public const string PSPSSegmentNameModelName = "PSPS_SEGMENT";
            public const string PriOHConductorModelName = "PRIOHCONDUCTOR";         
        }

        #region private members

        /// <summary>
        /// Model Names of the fields assigned than MapNumber and MapSclae fields
        /// </summary>
        List<string> _otherModelNames = null;
        /// <summary>
        /// Model Names assigned to the key fields.
        /// </summary>
        List<string> _keyModelNames = null;

        #endregion

        #region public properties
        /// <summary>
        /// Map Primary OH Conductor modelname "PriOHConductor"
        /// </summary>
        public string PriOHConductorModelName
        {
            get { return PGEGDBFields.PriOHConductorModelName; }
        }
        /// <summary>
        /// CircuitId Field ModelName "CIRCUITiD"
        /// </summary>
        public string CircuitIdModelName
        {
            get { return PGEGDBFields.CircuitIdModelName; }
        }
        /// <summary>
        /// MapScale field modelname "PGE_MAPTYPE"
        /// </summary>
        public string CircuitNameModelName
        {
            get { return PGEGDBFields.CircuitNameModelName; }
        }
        /// <summary>
        /// PSPS Segment field modelname
        /// </summary>
        public string PSPSSegmentNameModelName
        {
            get { return PGEGDBFields.PSPSSegmentNameModelName; }
        }

        /// <summary>
        /// OtherFieldModelNames to be persisted.
        /// </summary>
        public List<string> OtherFieldModelNames
        {
            get
            {
                if (_otherModelNames == null)
                {
                    _otherModelNames = new List<string>();
                    _otherModelNames.Add(PGEGDBFields.CircuitIdModelName);
                    _otherModelNames.Add(PGEGDBFields.CircuitNameModelName);
                    _otherModelNames.Add(PGEGDBFields.PSPSSegmentNameModelName);
                }
                return _otherModelNames;
            }
        }

        /// <summary>
        /// PGE Specific key fields
        /// "PGE_MAPOFFICE". At PGE the map numbers are not unique across the organization. The combination of MapOffice and MapNumber makes it Unique
        /// </summary>
        public List<string> KeyFieldModelNames
        {
            get
            {
                if (_keyModelNames == null)
                {
                    _keyModelNames = new List<string>();
                    _keyModelNames.Add(PGEGDBFields.CircuitIdModelName);
                }
                return _keyModelNames;
            }
        }
        #endregion
    }
}
