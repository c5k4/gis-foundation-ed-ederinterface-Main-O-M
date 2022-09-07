using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PGE.Desktop.EDER.ValidationRules.VoltageValidators
{
    /// <summary>
    /// A class used to determine the correct name for a related conductor info tables based on the o
    /// origin feature class name.  
    /// </summary>
    public class RelClassResolver
    {

        /// <summary>
        /// Dictionary to store origin FC names, and their corresponding related table names.
        /// </summary>
        private Dictionary<string, string> relClassNames;

        /// <summary>
        /// Singleton instance.
        /// </summary>
        private static RelClassResolver _instance;

        /// <summary>
        /// Singleton accessor.
        /// </summary>
        public static RelClassResolver Instance {
            
            get {
                // Initialize the singleton object if necessary and return it.
                if (_instance == null) _instance = new RelClassResolver();
                return _instance;
            }
            
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="RelClassResolver"/> class from being created.
        /// </summary>
        private RelClassResolver() {
            
            // Initialize the dictionary for PGE EDGIS data.  

            relClassNames = new Dictionary<string, string>();

            relClassNames["EDGIS.PriOHConductor"] = "EDGIS.PriOHConductor_PriOHConductorInfo";
            relClassNames["EDGIS.PriUGConductor"] = "EDGIS.PriUGConductor_PriUGConductorInfo";
            relClassNames["EDGIS.SecOHConductor"] = "EDGIS.SecOHConductor_SecOHConductorInfo";
            relClassNames["EDGIS.SecUGConductor"] = "EDGIS.SecUGConductor_SecUGConductorInfo";

            relClassNames["PriOHConductor"] = "PriOHConductor_PriOHConductorInfo";
            relClassNames["PriUGConductor"] = "PriUGConductor_PriUGConductorInfo";
            relClassNames["SecOHConductor"] = "SecOHConductor_SecOHConductorInfo";
            relClassNames["SecUGConductor"] = "SecUGConductor_SecUGConductorInfo";
        }

        /// <summary>
        /// Gets the name of the relationship class.
        /// </summary>
        /// <param name="featureClassName">Name of the feature class.</param>
        /// <returns></returns>
        public string GetRelClassName(string featureClassName) {
            
            string value = "";
            relClassNames.TryGetValue(featureClassName, out value);
            return value;
        }

    }
}
