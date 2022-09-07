using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using PGE.Common.ChangeDetectionAPI;

namespace PGE.Interfaces.Integration.Framework
{
    /// <summary>
    /// Defines a method for transforming data
    /// </summary>
    /// <typeparam name="T">The type of data to transform, i.e. IRow</typeparam>
    public interface IFieldTransformer<T>
    {
        /// <summary>
        /// Get a value based on the input data
        /// </summary>
        /// <typeparam name="V">The type of data to return</typeparam>
        /// <param name="row">The input data</param>
        /// <returns>The transformed value</returns>
        V GetValue<V>(T row);

        /// <summary>
        /// Get a value based on the input data
        /// </summary>
        /// <typeparam name="V">The type of data to return</typeparam>
        /// <param name="row">The input data</param>
        /// <returns>The transformed value</returns>
        V GetValue<V>(DeleteFeat row,ITable FCName);
    }
}
