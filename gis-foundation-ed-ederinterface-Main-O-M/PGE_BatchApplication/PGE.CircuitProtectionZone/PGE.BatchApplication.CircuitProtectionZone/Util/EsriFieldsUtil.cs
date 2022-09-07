using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;

namespace PGE.BatchApplication.CircuitProtectionZone.Util
{
    public class EsriFieldsUtil
    {
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
                    value = row.Value[fieldIndex];
                }
            }

            return value;
        }

        /// <summary>
        /// Sets the field value.  Will only set if the field type matches the expected field type.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        /// <param name="expectedFieldType"></param>
        public static void SetField(IObject obj, string fieldName, object value, esriFieldType expectedFieldType)
        {
            int fieldIndex = obj.Class.FindField(fieldName);
            if (fieldIndex != -1)
            {
                if (obj.Class.Fields.get_Field(fieldIndex).Type == expectedFieldType)
                {
                    obj.set_Value(fieldIndex, value);
                }
            }
        }

        /// <summary>
        /// Sets the Field value.  Will only match if field type matches the .net type.
        /// Not all types are supported.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="fieldName"></param>
        /// <param name="value">value must be of correct type for field otherwise an exception will be thrown.</param>
        public static bool SetField(IObject obj, string fieldName, object value)
        {
            int fieldIndex = obj.Class.FindField(fieldName);
            return SetField(obj, fieldIndex, value);
        }

        
        /// <summary>
        /// Sets the Field value.  Will only match if field type matches the .net type.
        /// Not all types are supported.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="fieldIndex"></param>
        /// <param name="value">value must be of correct type for field otherwise an exception will be thrown.</param>
        public static bool SetField(IObject obj, int fieldIndex, object value)
        {
            try
            {
                if (fieldIndex != -1)
                {
                    if (value != null)
                    {
                        bool typesMatch = false;
                        bool changeToString = false;
                        bool changeToInt = false;

                        switch (obj.Class.Fields.get_Field(fieldIndex).Type)
                        {
                            case esriFieldType.esriFieldTypeDouble:
                                typesMatch = value is double || value is float || value is int || value is short;
                                break;
                            case esriFieldType.esriFieldTypeSingle:
                                typesMatch = value is float || value is int || value is short;
                                break;
                            case esriFieldType.esriFieldTypeInteger:
                                changeToInt = true;
                                typesMatch = value is long || value is int || value is short || value is double || value is string;
                                break;
                            case esriFieldType.esriFieldTypeSmallInteger:
                                typesMatch = value is int || value is short;
                                break;
                            case esriFieldType.esriFieldTypeDate:
                                typesMatch = value is DateTime;
                                break;
                            case esriFieldType.esriFieldTypeString:
                                typesMatch = true;
                                changeToString = true;
                                break;
                            case esriFieldType.esriFieldTypeGUID:
                                typesMatch = true;
                                break;
                        }

                        if (typesMatch == true)
                        {
                            if (changeToString == true)
                            {
                                string valueAsString = value.ToString();

                                if (valueAsString.Length > obj.Class.Fields.get_Field(fieldIndex).Length)
                                {
                                    valueAsString = valueAsString.Substring(0, obj.Class.Fields.get_Field(fieldIndex).Length);
                                }

                                obj.set_Value(fieldIndex, valueAsString);
                            }
                            else if (changeToInt == true)
                            {
                                obj.set_Value(fieldIndex, value);
                            }
                            else
                            {
                                obj.set_Value(fieldIndex, value);
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        obj.set_Value(fieldIndex, value);
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Is the field supporded byt the feature class.
        /// </summary>
        /// <param name="featureClass"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static bool IsFieldSupported(IFeatureClass featureClass, string fieldName)
        {
            int index = featureClass.FindField(fieldName);
            return (index != -1);
        }

        /// <summary>
        /// Is the field supported by the table.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static bool IsFieldSupported(ITable table, string fieldName)
        {
            int index = table.FindField(fieldName);
            return (index != -1);
        }

        public static string GetFieldDomainValue(IFeature feature, string fieldName, object fieldValue)
        {
            string returnValue = null;

            int fieldIndex = feature.Fields.FindField(fieldName);
            if (fieldIndex > -1)
            {
                IField field = feature.Fields.Field[fieldIndex];
                if (field != null)
                {
                    if (field.Domain != null)
                    {
                        IDomain domain = field.Domain;
                        if (domain is ICodedValueDomain)
                        {
                            for (int i = 0; i < ((ICodedValueDomain)domain).CodeCount; i++)
                            {
                                if (((ICodedValueDomain)domain).Value[i].Equals(fieldValue))
                                {
                                    returnValue = ((ICodedValueDomain)domain).Name[i];
                                    return returnValue;
                                }
                            }
                        }
                    }
                }
            }

            return returnValue;
        }

        public static string GetFieldDomainValue(IFeatureClass featureClass, string fieldName, object fieldValue)
        {
            string returnValue = null;

            int fieldIndex = featureClass.Fields.FindField(fieldName);
            if (fieldIndex > -1)
            {
                IField field = featureClass.Fields.Field[fieldIndex];
                if (field != null)
                {
                    if (field.Domain != null)
                    {
                        IDomain domain = field.Domain;
                        if (domain is ICodedValueDomain)
                        {
                            for (int i = 0; i < ((ICodedValueDomain)domain).CodeCount; i++)
                            {
                                if (((ICodedValueDomain)domain).Value[i].Equals(fieldValue))
                                {
                                    returnValue = ((ICodedValueDomain)domain).Name[i];
                                    return returnValue;
                                }
                            }
                        }
                    }
                }
            }

            return returnValue;
        }

        public static string GetFieldValueSyntax(IFeatureClass featureClass, string fieldName, object value)
        {
            string querySyntax = null;

            if (IsFieldSupported(featureClass, fieldName) == true)
            {
                int fieldIndex = featureClass.Fields.FindField(fieldName);

                if (fieldIndex > -1)
                {
                    if (value != null)
                    {
                        switch (featureClass.Fields.get_Field(fieldIndex).Type)
                        {
                            case esriFieldType.esriFieldTypeDouble:
                            case esriFieldType.esriFieldTypeSingle:
                            case esriFieldType.esriFieldTypeInteger:
                            case esriFieldType.esriFieldTypeSmallInteger:
                            case esriFieldType.esriFieldTypeOID:
                                querySyntax = string.Format("{0} = {1}", fieldName, value);
                                break;
                            case esriFieldType.esriFieldTypeDate:
                                querySyntax = string.Format("{0} = {1}", fieldName, value);
                                break;
                            case esriFieldType.esriFieldTypeString:
                            case esriFieldType.esriFieldTypeGUID:
                                querySyntax = string.Format("{0} = '{1}'", fieldName, value.ToString());
                                break;
                        }
                    }
                }
            }
            return querySyntax;
        }

        public static string GetFieldValueSyntax(ITable table, string fieldName, object value)
        {
            string querySyntax = null;

            if (IsFieldSupported(table, fieldName) == true)
            {
                int fieldIndex = table.Fields.FindField(fieldName);

                if (fieldIndex > -1)
                {
                    if (value != null)
                    {
                        switch (table.Fields.get_Field(fieldIndex).Type)
                        {
                            case esriFieldType.esriFieldTypeDouble:
                            case esriFieldType.esriFieldTypeSingle:
                            case esriFieldType.esriFieldTypeInteger:
                            case esriFieldType.esriFieldTypeSmallInteger:
                                querySyntax = string.Format("{0} = {1}", fieldName, value);
                                break;
                            case esriFieldType.esriFieldTypeDate:
                                querySyntax = string.Format("{0} = {1}", fieldName, value);
                                break;
                            case esriFieldType.esriFieldTypeString:
                            case esriFieldType.esriFieldTypeGUID:
                                querySyntax = string.Format("{0} = '{1}'", fieldName, value.ToString());
                                break;
                        }
                    }
                }
            }
            return querySyntax;
        }
    }
}
