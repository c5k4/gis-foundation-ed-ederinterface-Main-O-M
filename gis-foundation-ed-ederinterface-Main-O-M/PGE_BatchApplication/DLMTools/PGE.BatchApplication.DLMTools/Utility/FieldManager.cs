using System.Collections.Generic;
using System.Reflection;
using PGE.BatchApplication.DLMTools.Utility.Logs;
using ESRI.ArcGIS.Geodatabase;

namespace PGE.BatchApplication.DLMTools.Utility
{
    /// <summary>
    /// Manages field index caches and special methods for accessing field values using the cache.
    /// </summary>
    internal static class FieldManager
    {
        static readonly Log4NetFileHelper _logHelper = new Log4NetFileHelper(MethodBase.GetCurrentMethod().DeclaringType);

        // Dictionary<ObjectClassID, Dictionary<FieldName, Index>>
        private static Dictionary<int, Dictionary<string, int>> _idxCache = new Dictionary<int, Dictionary<string, int>>();

        /// <summary>
        /// Gets a stored cache item for a field index based on the class and field name.
        /// </summary>
        /// <param name="objClass">The object class that the field index will be found within.</param>
        /// <param name="fieldName">The name of the desired field in the object class.</param>
        /// <returns>An integer corresponding to the field index on the object.</returns>
        internal static int GetIndex(IObjectClass objClass, string fieldName)
        {
            if (_idxCache.ContainsKey(objClass.ObjectClassID) && _idxCache[objClass.ObjectClassID].ContainsKey(fieldName))
                return _idxCache[objClass.ObjectClassID][fieldName];
            else
            {
                // Get field index.
                int idx = objClass.Fields.FindField(fieldName);

                // Add to cache
                if (!_idxCache.ContainsKey(objClass.ObjectClassID))
                    _idxCache.Add(objClass.ObjectClassID, new Dictionary<string, int>());
                if (!_idxCache[objClass.ObjectClassID].ContainsKey(fieldName))
                    _idxCache[objClass.ObjectClassID].Add(fieldName, idx);

                return idx;
            }
        }

        /// <summary>
        /// Gets the value of a field using the field index cache.
        /// </summary>
        /// <param name="obj">The object that the field value will be found within.</param>
        /// <param name="fieldName">The name of the desired field in the object class.</param>
        /// <returns>The value of the desired field.</returns>
        internal static object GetValue(IObject obj, string fieldName)
        {
            int idx = GetIndex(obj.Class, fieldName);
            if (idx > -1)
                return obj.get_Value(idx);
            return null;
        }

        /// <summary>
        /// Clears the static cache of field indices.
        /// </summary>
        internal static void ClearIdxCache()
        {
            _idxCache = new Dictionary<int, Dictionary<string, int>>();
            _logHelper.DefaultLogger.Info("Field index cache cleared.");
        }
    }
}
