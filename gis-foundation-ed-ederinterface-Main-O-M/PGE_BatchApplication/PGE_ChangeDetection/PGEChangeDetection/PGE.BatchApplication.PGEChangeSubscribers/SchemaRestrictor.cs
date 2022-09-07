using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.GeoDatabaseDistributed;
using ESRI.ArcGIS.Geometry;
using PGE.ChangesManagerShared.Interfaces;
using PGE.ChangesManagerShared;
using PGE.Diagnostics;
using System.Reflection;

namespace PGE.ChangeSubscribers
{
    // This class is the holding place for the featureclass/attribute restrictions for schematics
    public class SchemaRestrictor
    {
        private IDictionary<string, string[]> _featureClassChangeRestrictions = new Dictionary<string, string[]>();

        public SchemaRestrictor(IDictionary<string, string> featureClassChangeRestrictionsCSV)
        {
            foreach (string key in featureClassChangeRestrictionsCSV.Keys)
            {
                if (!String.IsNullOrEmpty(featureClassChangeRestrictionsCSV[key]))
                {
                    _featureClassChangeRestrictions.Add(key, featureClassChangeRestrictionsCSV[key].Split(','));                
                }
                else
                {
                    _featureClassChangeRestrictions.Add(key, null);                                    
                }
            }
        }
    }
}
