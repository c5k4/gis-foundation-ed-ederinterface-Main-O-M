using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.GeoDatabaseDistributed;
using ESRI.ArcGIS.Geometry;
using PGE.Common.ChangesManagerShared.Interfaces;
using PGE.Common.ChangesManagerShared;
using System.Reflection;
using PGE.Common.Delivery.Diagnostics;

namespace PGE.Common.ChangesManagerShared
{
    // This class is the holding place for the featureclass/attribute restrictions for schematics
    public class SchemaRestrictor
    {
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "");

        private IDictionary<string, string[]> _featureClassChangeRestrictions = new Dictionary<string, string[]>();

        public IList<string> UpdateOnlyFilter { get; set; } 

        public string InServiceFieldValue { get; set; }

        public string[] GetAttributeRestrictionsByFeatureClass(string featureClassName)
        {
            if (featureClassName.Contains("."))
            {
                featureClassName = featureClassName.Substring(featureClassName.LastIndexOf(".") + 1).ToUpper();
            }
            if (_featureClassChangeRestrictions.ContainsKey(featureClassName.ToUpper()))
            {
                return _featureClassChangeRestrictions[featureClassName];
            }
            else
            {
                return null;
            }
        }

        public SchemaRestrictor(IDictionary<string, string> featureClassChangeRestrictionsCSV)
        {
            foreach (string key in featureClassChangeRestrictionsCSV.Keys)
            {
                if (!String.IsNullOrEmpty(featureClassChangeRestrictionsCSV[key]))
                {
                    _featureClassChangeRestrictions.Add(key.ToUpper(), featureClassChangeRestrictionsCSV[key].Split(','));                
                }
                else
                {
                    _featureClassChangeRestrictions.Add(key.ToUpper(), null);                                    
                }
            }
        }

        public bool FeatureClassIsIncluded(string featureClassName)
        {
            if (featureClassName.Contains("."))
            {
                featureClassName = featureClassName.Substring(featureClassName.LastIndexOf(".") + 1);
            }
            if (_featureClassChangeRestrictions.ContainsKey(featureClassName.ToUpper()))
            {
                return true;
            }

            return false;
        }

        public bool AttributeIsIncluded(string featureClassName, string attributeName)
        {
            string fcUpper = featureClassName.ToUpper();
            if (_featureClassChangeRestrictions.ContainsKey(fcUpper))
            {
                if (_featureClassChangeRestrictions[fcUpper].Contains(attributeName.ToUpper()))
                {
                    return true;                    
                }
            }

            return false;
        }
    }
}
