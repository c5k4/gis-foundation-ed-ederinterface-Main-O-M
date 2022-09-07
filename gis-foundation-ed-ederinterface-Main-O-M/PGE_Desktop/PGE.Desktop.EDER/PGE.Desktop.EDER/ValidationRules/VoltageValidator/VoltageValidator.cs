using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;

namespace PGE.Desktop.EDER.ValidationRules.VoltageValidators
{
    /// <summary>
    /// Base class for voltage validators provides default error message encapsulation.
    /// </summary>
    public abstract class VoltageValidator 
    {

        /// <summary>
        /// Builds a consistent error string for voltage validators.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="feature">The feature.</param>
        /// <returns></returns>
        
        // example from Shirley: Load line voltage is not allowed with current StepDown voltage or Output voltage (featureclass.name ObjectID: xxxx)
        protected string buildErrorString(string message, IFeature feature)
        {

            StringBuilder sb = new StringBuilder();

            sb.Append(message);
            sb.Append(" (");
            sb.Append(feature.Class.AliasName);
            sb.Append(" Object ID: ");
            sb.Append(feature.OID);
            sb.Append(").");

            return sb.ToString();
        }
    }
}
