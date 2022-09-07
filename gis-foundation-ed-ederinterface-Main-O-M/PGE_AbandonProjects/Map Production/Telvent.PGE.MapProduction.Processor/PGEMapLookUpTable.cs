using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Telvent.PGE.MapProduction.Processor
{
    /// <summary>
    /// PGE Specific implementation of IMPMapLookUpTable.
    /// Extends BaseMapLookUpTable
    /// </summary>
    public class PGEMapLookUpTable:BaseMapLookUpTable
    {
        #region private members
        List<string> _otherFields = null;
        List<string> _keyFields = null;
        #endregion

        #region public overrides

        /// <summary>
        /// List of other fields to be persisted in LookUp Table
        /// "LEGACYCOORDINATE"
        /// "MAPOFFICE"
        /// </summary>
        public override List<string> OtherFields
        {
            get
            {
                if (_otherFields == null)
                {
                    _otherFields = new List<string>();
                    _otherFields.Add("LEGACYCOORDINATE");
                    _otherFields.Add("MAPOFFICE");
                    _otherFields.Add("MAPID");
                }
                return _otherFields;
            }
        }

        /// <summary>
        /// List of key fields to uniquely identify a Map
        /// "MAPOFFICE". At PGE the map numbers are not unique across the organization. The combination of MapOffice and MapNumber makes it Unique
        /// </summary>
        public override List<string> KeyFields
        {
            get
            {
                if (_keyFields == null)
                {
                    _keyFields = new List<string>();
                    _keyFields.Add("MAPOFFICE");
                }
                return _keyFields;
            }
        }

        /// <summary>
        /// Name of the table that holds the Map look up information
        /// "PGE_MAPNUMBERCOORDLUT"
        /// </summary>
        public override string TableName
        {
            get
            {
                return string.IsNullOrEmpty(MapProductionConfigurationHandler.GetSettingValue("TableOwner")) ? "PGE_MAPNUMBERCOORDLUT" : MapProductionConfigurationHandler.GetSettingValue("TableOwner") + ".PGE_MAPNUMBERCOORDLUT";
            }
        }

        #endregion
    }
}
