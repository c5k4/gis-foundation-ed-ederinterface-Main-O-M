using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ESRI.ArcGIS.Geodatabase;

namespace PGE.Desktop.EDER.AutoUpdaters
{
    /// <summary>
    /// IEvaluationCriteria interface provides access to initialization and evaluation logic 
    /// for symbol number criteria.
    /// </summary>
    interface IEvaluationCriteria
    {
        /// <summary>
        /// Initializes the specified object class.
        /// </summary>
        /// <param name="objectClass">The object class.</param>
        void Initialize(IObjectClass objectClass);

        /// <summary>
        /// Evaluates the specified feature, and returns the symbol number defined by the criteria.
        /// </summary>
        /// <param name="feature">The feature.</param>
        /// <returns></returns>
        bool Evaluate(IObject feature);
    }
}
