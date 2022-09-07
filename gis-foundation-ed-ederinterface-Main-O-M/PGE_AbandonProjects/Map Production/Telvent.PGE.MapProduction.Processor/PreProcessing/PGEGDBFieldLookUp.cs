using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Telvent.PGE.MapProduction.PreProcessing
{
    /// <summary>
    /// Implementation of PGE specific IMPGDBFieldLoopUp interface.
    /// </summary>
    public class PGEGDBFieldLookUp:IMPGDBFieldLookUp
    {
        public struct PGEGDBFields
        {
            public const string MapOfficeMN = "PGE_MAPOFFICE";
            public const string LegacyCorrdSystemMN = "PGE_LEGACYCOORDSYSTEM";
            public const string DistributionMapFCMN = "PGE_DISTRIBUTIONMAP";
            public const string MapNumberMN = "PGE_DISTRIBUTIONMAPNUMBER";
            public const string MapTypeMN = "PGE_MAPTYPE";
            public const string LocalOfficeMN = "PGE_LOCALOFFICE";
            public const string MapIDMN = "PGE_LEGACYMAPID";
        }

        #region private members
        /// <summary>
        /// List of subtype to be considered
        /// </summary>
        List<int> _subTypes = null;
        /// <summary>
        /// Model Names of the fields assigned than MapNumber and MapSclae fields
        /// </summary>
        List<string> _otherModelNames = null;
        /// <summary>
        /// Model Names assigned to the key fields.
        /// </summary>
        List<string> _keyModelNames = null;
        /// <summary>
        /// Subtype config key
        /// </summary>
        private const string _subtype = "subtype";
        #endregion

        #region public properties
        /// <summary>
        /// Map Polygon modelname "PGE_DISTRIBUTIONMAP"
        /// </summary>
        public string MapPolygonModelName
        {
            get { return PGEGDBFields.DistributionMapFCMN; }
        }
        /// <summary>
        /// Map Number Field ModelName "PGE_DISTRIBUTIONMAPNUMBER"
        /// </summary>
        public string MapNumberModelName
        {
            get { return PGEGDBFields.MapNumberMN; }
        }
        /// <summary>
        /// MapScale field modelname "PGE_MAPTYPE"
        /// </summary>
        public string MapScaleModelName
        {
            get { return PGEGDBFields.MapTypeMN; }
        }
        /// <summary>
        /// Subtype to use from the MapPolygon featureclass as configured in the Configuration file
        /// The configuration file holds the SubtypeCode
        /// </summary>
        public List<int> Subtype
        {
            get 
            {
                if (_subTypes == null)
                {
                    _subTypes = new List<int>();
                    string subtype = MapProductionConfigurationHandler.GetSettingValue(_subtype);
                    if (!string.IsNullOrEmpty(subtype))
                    {
                        string[] subtypes = subtype.Split(",".ToCharArray());
                        int intSubtype=-1;
                        foreach (string subtypeString in subtypes)
                        {
                            if (int.TryParse(subtypeString, out intSubtype))
                                _subTypes.Add(intSubtype);
                        }
                    }
                    
                }
                return _subTypes;
            }
        }

        /// <summary>
        /// OtherFieldModelNames to be persisted.
        /// "PGE_MAPOFFICE"
        /// "PGE_LEGACYCOORDSYSTEM"
        /// </summary>
        public List<string> OtherFieldModelNames
        {
            get
            {
                if (_otherModelNames == null)
                {
                    _otherModelNames = new List<string>();
                    _otherModelNames.Add(PGEGDBFields.MapOfficeMN);
                    _otherModelNames.Add(PGEGDBFields.LegacyCorrdSystemMN);
                    _otherModelNames.Add(PGEGDBFields.MapIDMN);
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
                    _keyModelNames.Add(PGEGDBFields.MapOfficeMN);
                }
                return _keyModelNames;
            }
        }
        #endregion
    }
}
