using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace PGE.BatchApplication.CircuitProtectionZone.Util
{
    /// <summary>
    /// Utility class for working with Esri
    /// </summary>
    public class EsriUtil
    {
        public static string GetFeatureClassName(IFeature feature)
        {
            string featureClassName = ((IDataset)feature.Class).Name;

            int index = featureClassName.LastIndexOf('.');
            if (index > -1)
            {
                featureClassName = featureClassName.Substring(index + 1);
            }

            return featureClassName;
        }

        /// <summary>
        /// Gets the field value.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static object GetFieldValue(IObject row, string fieldName)
        {
            object value = null;
            if (string.IsNullOrEmpty(fieldName) == false)
            {
                int fieldIndex = row.Fields.FindField(fieldName);
                if (fieldIndex != -1)
                {
                    value = row.get_Value(fieldIndex);
                }
            }

            return value;
        }

    }
}