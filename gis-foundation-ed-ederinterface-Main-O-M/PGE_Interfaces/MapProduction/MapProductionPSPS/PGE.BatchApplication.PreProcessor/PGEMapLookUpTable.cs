using PGE.BatchApplication.PSPSMapProduction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PGE.BatchApplication.PreProcessor
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
        /// </summary>
        public override List<string> OtherFields
        {
            get
            {
                if (_otherFields == null)
                {
                    _otherFields = new List<string>();
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
                }
                return _keyFields;
            }
        }

        #endregion
    }
}
