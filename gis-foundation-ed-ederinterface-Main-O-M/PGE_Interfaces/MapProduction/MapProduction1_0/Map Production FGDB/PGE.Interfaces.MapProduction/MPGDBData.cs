using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace PGE.Interfaces.MapProduction
{
    /// <summary>
    /// Base Implementaion of IMPGDBData.
    /// </summary>
    [Serializable]
    public class MPGDBData : IMPGDBData
    {
        private bool _isValid = true;
        private List<string> _polygonCoordinates;
        #region IMPGDBData impelmentation
        /// <summary>
        /// Holds the MapNumber data
        /// </summary>
        public string MapNumber
        {
            get;
            set;
        }
        /// <summary>
        /// Holds the XMin data
        /// </summary>
        public double XMin
        {
            get;
            set;
        }
        /// <summary>
        /// Holds the YMin data
        /// </summary>
        public double YMin
        {
            get;
            set;
        }
        /// <summary>
        /// Holds the XMax data
        /// </summary>
        public double XMax
        {
            get;
            set;
        }
        /// <summary>
        /// Holds the YMax data
        /// </summary>
        public double YMax
        {
            get;
            set;
        }
        /// <summary>
        /// Holds the MapScale data
        /// </summary>
        public int MapScale
        {
            get;
            set;
        }
        /// <summary>
        /// Holds the Polygon Geometry
        /// </summary>
        public List<string> PolygonCoordinates
        {
            get
            {
                if (_polygonCoordinates == null)
                {
                    _polygonCoordinates = new List<string>();
                }
                return _polygonCoordinates;
            }
            set
            {
                _polygonCoordinates = value;
            }
        }
        /// <summary>
        /// Returns the Polygon Gemoetry in a WKT Fortmat
        /// </summary>
        public string WKTPolygon
        {
            get { return "POLYGON(" + string.Join(",", PolygonCoordinates.ToArray()) + ")"; }
        }
        /// <summary>
        /// Holds the field Values for OtherFields as a ModelName, Value combination
        /// </summary>
        [XmlIgnore]
        public Dictionary<string, string> OtherFieldValues
        {
            get;
            set;
        }
        /// <summary>
        /// Holds the field Values for KeyFields as a ModelName, Value combination
        /// This is a subset of the OtherFieldValues
        /// </summary>
        [XmlIgnore]
        public Dictionary<string, string> KeyFieldValues
        {
            get;
            set;
        }
        /// <summary>
        /// Returns a string that is concatenation of MapNumber+":"+all the field values as defined by KeyFieldValues property.
        /// If the KeyFieldValue is null or does not have any entry will return MapNumber as the Key
        /// </summary>
        [XmlIgnore]
        public string Key
        {
            get
            {
                return (KeyFieldValues == null || KeyFieldValues.Count == 0) ? MapNumber : MapNumber + ":" + string.Join(":", KeyFieldValues.Values.ToArray());
            }
        }
        /// <summary>
        /// Returns if the Instance of MPGDBData is valid
        /// </summary>
        [XmlIgnore]
        public virtual bool IsValid
        {
            get
            {
                Validate();
                return _isValid;
            }
            protected set
            {
                _isValid = value;
            }
        }
        /// <summary>
        /// Checks if the current instance og MPGDBData is valid by chekcing
        /// XMax!=int.MinValue
        /// YMax!=int.MinValue
        /// YMin!=int.MaxValue
        /// XMin!=int.MaxValue
        /// Every field as defined by the KeyFields is not null or empty string
        /// MapNumber is not null or emmpty
        /// </summary>
        public virtual void Validate()
        {
            if (string.IsNullOrEmpty(MapNumber) ||
                this.XMax == int.MinValue ||
                this.YMax == int.MinValue ||
                this.XMin == int.MaxValue ||
                this.YMin == int.MaxValue)
            { IsValid = false; return; }
            foreach (KeyValuePair<string, string> kp in KeyFieldValues)
            {
                if (string.IsNullOrEmpty(kp.Value))
                {
                    IsValid = false;
                    break;
                }
            }
        }
        #endregion
    }
}
