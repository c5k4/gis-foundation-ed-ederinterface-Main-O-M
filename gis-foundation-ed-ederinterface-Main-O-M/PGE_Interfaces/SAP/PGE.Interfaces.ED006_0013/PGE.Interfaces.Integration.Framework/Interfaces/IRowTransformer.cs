using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PGE.Interfaces.Integration.Framework.Data;
using ESRI.ArcGIS.Geodatabase;
using System.Xml;
using PGE.Interfaces.Integration.Framework;
using PGE.Common.ChangeDetectionAPI;

namespace PGE.Interfaces.Integration.Framework
{
    /// <summary>
    /// Defines methods for transforming data
    /// </summary>
    /// <typeparam name="T">The type of data to be transformed, i.e. IRow</typeparam>
    public interface IRowTransformer<T>
    {
        /// <summary>
        /// The subtypes of the feature class this transformer acts upon
        /// </summary>
        List<string> Subtypes { get; }
        /// <summary>
        /// The feature class this transformer acts upon
        /// </summary>
        string SourceClass { get; }
        /// <summary>
        /// 
        /// </summary>
        List<string> SupportClasses { get; }
        /// <summary>
        /// Name of the tracked class
        /// </summary>
        string OutName { get; }
        /// <summary>
        /// The list of fields this transformer processes
        /// </summary>
        List<MappedField> Fields { get; }
        /// <summary>
        /// Transform a row of type T into an IRowData structure
        /// </summary>
        /// <param name="sourceRow">The edited row</param>
        /// <param name="targetRow">The row before it was edited</param>
        /// <param name="changeType">The type of edit</param>
        /// <returns></returns>
        List<IRowData> ProcessRow(T sourceRow,T targetRow,ChangeType changeType);

        /// <summary>
        /// Transform a row of type T into an IRowData structure
        /// </summary>
        /// <param name="sourceRow">The edited row</param>
        /// <param name="targetRow">The row before it was edited</param>
        /// <param name="changeType">The type of edit</param>
        /// <returns></returns>
        List<IRowData> ProcessRow(T sourceRow, UpdateFeat targetRow, ChangeType changeType);

        /// <summary>
        /// Transform a row of type T into an IRowData structure
        /// </summary>
        /// <param name="sourceRow">The edited row</param>
        /// <param name="changeType">The type of edit</param>
        /// <returns></returns>
        List<IRowData> ProcessRow(DeleteFeat sourceRow, ChangeType changeType);

        /// <summary>
        /// Initialize the transformer with references to its parents.
        /// </summary>
        /// <param name="mapper">The root mapper</param>
        /// <param name="trackedClass">This transformer's tracked class</param>
        void Initialize(SystemMapper mapper, TrackedClass trackedClass);

        //V3SF (28-Mar-2022) Added Additional Parameter to display Skipping Record in Staging
        //IVARA
        /// <summary>
        /// Validates the feature if it needs any special condition to satisfy. 
        /// For example network switch or network open point features
        /// </summary>
        /// <param name="sourceRow">Source row</param>
        /// <param name="errorMsg"></param>
        /// <returns>true or false</returns>
        bool IsValid(IRow sourceRow, out string errorMsg);

        //V3SF (28-Mar-2022) Added Additional Parameter to display Skipping Record in Staging
        /// <summary>
        /// Validates the feature if it needs any special condition to satisfy. 
        /// For example network switch or network open point features
        /// </summary>
        /// <param name="sourceRow">Source row</param>
        /// <param name="errorMsg"></param>
        /// <returns>true or false</returns>
        bool IsValid(DeleteFeat sourceRow, out string errorMsg);
    }
}
