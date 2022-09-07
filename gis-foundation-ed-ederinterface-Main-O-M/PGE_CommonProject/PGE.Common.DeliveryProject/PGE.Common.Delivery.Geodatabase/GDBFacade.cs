using System;
using System.Collections.Generic;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using Miner.Geodatabase;

using PGE.Common.Delivery.Framework;
using PGE.Common.Delivery.Systems;

namespace PGE.Common.Delivery.Geodatabase
{
    /// <summary>
    /// 
    /// </summary>
    public class GDBFacade
    {
        #region Feature Methods
        /// <summary>
        /// Given a featureclass will create a feature and sets the subtype to default value and sets all the default values for fields that has a default value.
        /// </summary>
        /// <param name="featureClass">An object of type IFeatureclass.</param>
        /// <returns>A feature associated to a featureclass passed in</returns>
        /// <remarks>If the featureclass does not have a subtype associated with it then only the default values for the fields are initialized</remarks>
        public static IFeature CreateFeatureWithDefaultSubtypeAndValues(IFeatureClass featureClass)
        {
            if (featureClass == null) throw new ArgumentNullException();
            IFeature feature = featureClass.CreateFeature();
            if (feature != null)
            {
                ISubtypes subtypes = (ISubtypes)featureClass;
                IRowSubtypes rowSubtypes = (IRowSubtypes)feature;
                if (rowSubtypes != null)
                {
                    if (subtypes.HasSubtype)
                    {
                        rowSubtypes.SubtypeCode = subtypes.DefaultSubtypeCode;
                    }
                    rowSubtypes.InitDefaultValues();// initialize any default values
                }
            }
            return feature;
        }

        #endregion

        #region Field Methods

        /// <summary>
        /// Gets the field with the associated model name from the object class
        /// </summary>
        /// <param name="ObjectClass">The object class that has the field</param>
        /// <param name="FieldModelName">The model name on the field</param>
        /// <returns>The field, if it is found. Otherwise, null.</returns>
        public static IField GetFieldFromModelName(IObjectClass ObjectClass, string FieldModelName)
        {
            if (ObjectClass == null) throw new ArgumentNullException("ObjectClass");
            if (String.IsNullOrEmpty(FieldModelName)) throw new ArgumentNullException("FieldModelName");

            IField field = ModelNameFacade.FieldFromModelName(ObjectClass, FieldModelName);

            return field;
        }

        /// <summary>
        /// Gets the value for a field that has the associated field modelname in the given feature
        /// </summary>
        /// <param name="feature">The feature that has the field</param>
        /// <param name="fieldModelName">The model name on the field</param>
        /// <returns>The value of the field, if it is found. Otherwise, an empty string</returns>
        public static object GetFieldValueFromModelName(IFeature feature, string fieldModelName)
        {
            if (feature == null) throw new ArgumentNullException("Feature");
            if (String.IsNullOrEmpty(fieldModelName)) throw new ArgumentNullException("FieldModelName");

            IField field = GetFieldFromModelName(feature.Class, fieldModelName);
            if (field != null)
            {
                return GetFieldValueFromFieldName(feature, field.Name);
            }
            return null;
        }

        /// <summary>
        /// Returns the value in the row that has been assigned the given field model name.
        /// </summary>
        /// <typeparam name="T">The data type of the conversion.</typeparam>
        /// <param name="obj">The obj.</param>
        /// <param name="modelName">Name of the model.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The value converted to the specified type.</returns>
        /// <exception cref="NullReferenceException">Unable to obtain the field with the model name.</exception>
        public static T FieldValueFromModelName<T>(IObject obj, string modelName, T defaultValue)
        {
            string fieldName = ModelNameFacade.FieldNameFromModelName(obj.Class, modelName);
            int pos = obj.Class.FindField(fieldName);
            return TypeCastFacade.Cast(obj.get_Value(pos), defaultValue);
        }

        /// <summary>
        /// Gets the value for a field that has the given field name in the given feature
        /// </summary>
        /// <param name="feature">The feature that has the field</param>
        /// <param name="fieldName">The name of the field</param>
        /// <returns>The value of the field, if it is found. Otherwise, an empty string</returns>
        public static object GetFieldValueFromFieldName(IFeature feature, string fieldName)
        {
            if (feature == null) throw new ArgumentNullException("Feature");
            if (String.IsNullOrEmpty(fieldName)) throw new ArgumentNullException("FieldName");

            int fldIndex = feature.Fields.FindField(fieldName);
            if (fldIndex != -1)
            {
                return feature.get_Value(fldIndex);
            }
            return null;
        }

        /// <summary>
        /// Sets the value for a field that has the associated field modelname in the given feature
        /// </summary>
        /// <param name="feature">Feature to be updated</param>
        /// <param name="fieldModelName">Modelname of the field that should be updated</param>
        /// <param name="value">The value with which the field should be updated</param>
        /// <returns>True if the feature was changed</returns>
        public static bool SetFieldValueByModelName(IFeature feature, string fieldModelName, object value)
        {
            if (feature == null) throw new ArgumentNullException("Feature");
            if (String.IsNullOrEmpty(fieldModelName)) throw new ArgumentNullException("FieldModelName");

            IField field = GetFieldFromModelName(feature.Class, fieldModelName);
            if (field != null)
            {
                return SetFieldValueByFieldName(feature, field.Name, value);
            }
            return false;
        }

        /// <summary>
        /// Sets the value for a field that has the given field name in the given feature
        /// </summary>
        /// <param name="feature">Feature to be updated</param>
        /// <param name="fieldName">Name of the field that should be updated</param>
        /// <param name="value">The value with which the field should be updated</param>
        /// <returns>True if the feature was changed</returns>
        public static bool SetFieldValueByFieldName(IFeature feature, string fieldName, object value)
        {
            if (feature == null) throw new ArgumentNullException("FeatureToSet");
            if (String.IsNullOrEmpty(fieldName)) throw new ArgumentNullException("FieldName");

            int fldIndex = feature.Fields.FindField(fieldName);
            if (fldIndex >= 0)
            {
                if (!feature.get_Value(fldIndex).Equals(value))
                {
                    feature.set_Value(fldIndex, value);
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region Domain Methods

        /// <summary>
        /// Given a Domain gets the Domain as Dictionary, with the domain code as Key and Domain description as value.
        /// </summary>
        /// <param name="codedDomain">An object of type ICodedDomain whose value has to be converted.</param>
        /// <returns>Returns a Dictionary of type string,string</returns>
        public static Dictionary<string, string> GetDomainAsDictionary(ICodedValueDomain codedDomain)
        {
            Dictionary<string, string> retValue = new Dictionary<string, string>();
            try
            {
                for (int i = 0; i < codedDomain.CodeCount; i++)
                {
                    retValue.Add(codedDomain.get_Value(i).ToString(), codedDomain.get_Name(i));
                }
                return retValue;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// Given a DomainName and workspace gets the domain and covnerts it to Dictionary
        /// </summary>
        /// <param name="domainName"></param>
        /// <param name="ws"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetDomainAsDictionary(string domainName, IWorkspace ws)
        {
            Dictionary<string, string> retValue = new Dictionary<string, string>();
            try
            {
                IWorkspaceDomains wsDomains = (IWorkspaceDomains)ws;
                IDomain domain = wsDomains.get_DomainByName(domainName);
                if (domain is ICodedValueDomain)
                {
                    retValue = GetDomainAsDictionary(domain as ICodedValueDomain);
                }
                return retValue;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// Gets the Domain Code given the Domain Description or the Domain Description given Domain Code
        /// </summary>
        /// <param name="domainToCheck">Domain that should be used to decode the value</param>
        /// <param name="codeDescription">Domain Code or Description that should be used</param>
        /// <param name="isCode">Set this to true if the passed in codeDescription value is of Domain Code</param>
        /// <returns>Returns a string. The returned value is domain description if the isCode value passed in is true otherwise the returned value is the domain code.</returns>
        public static string GetDomainDescriptionOrCode(ICodedValueDomain domainToCheck, string codeDescription, bool isCode)
        {
            try
            {
                for (int i = 0; i < domainToCheck.CodeCount; i++)
                {
                    if (isCode)
                    {
                        if (domainToCheck.get_Value(i).ToString().ToUpper() == codeDescription.ToUpper())
                            return domainToCheck.get_Name(i);
                    }
                    else
                    {
                        if (domainToCheck.get_Name(i).ToString().ToUpper() == codeDescription.ToUpper())
                            return domainToCheck.get_Value(i).ToString();
                    }
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// Finds the <see cref="IDomain"/> that equals the specified <paramref name="domainName"/> using a non case sensitive comparison.
        /// </summary>
        /// <param name="workspace">The workspace.</param>
        /// <param name="domainName">Name of the domain.</param>
        /// <returns>
        /// A <see cref="IDomain"/>, otherwise <c>null</c>.
        /// </returns>
        public static IDomain FindByName(IWorkspace workspace, string domainName)
        {
            IWorkspaceDomains wd = (IWorkspaceDomains)workspace;
            IEnumDomain enumDomains = wd.Domains;
            IDomain domain;

            while ((domain = enumDomains.Next()) != null)
            {
                if (domain.Name.Equals(domainName, StringComparison.InvariantCultureIgnoreCase))
                    return domain;
            }

            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="featureclass"></param>
        /// <returns></returns>
        public static Dictionary<int,Dictionary<int,IDomain>> GetDomains(IFeatureClass featureclass)
        {
            Dictionary<int, Dictionary<int, IDomain>> retVal = new Dictionary<int, Dictionary<int, IDomain>>();
            ISubtypes subtypes = (ISubtypes)featureclass;
            IEnumSubtype enumsubtypes = subtypes.Subtypes;
            int subtypeCode=-1;
            enumsubtypes.Next(out subtypeCode);
            while (subtypeCode >= 0)
            {
                Dictionary<int, IDomain> domains = new Dictionary<int, IDomain>();
                for (int i = 0; i < featureclass.Fields.FieldCount; i++)
                {
                    domains.Add(subtypeCode,subtypes.get_Domain(subtypeCode, featureclass.Fields.get_Field(i).Name)); 
                }
                retVal.Add(subtypeCode, domains);
            }
            return retVal;
        }
        #endregion

        #region ObjectClass/Featureclass Methods

        /// <summary>
        /// Gets the class name from feature class.
        /// </summary>
        /// <param name="workspace">The workspace</param>
        /// <param name="classId">The class id</param>
        /// <returns>String</returns>
        public static string ObjectNameFromClassId(IWorkspace workspace, int classId)
        {
            try
            {
                IFeatureWorkspaceManage2 manage = (IFeatureWorkspaceManage2)workspace;
                string name = manage.GetObjectClassNameByID(classId);
                return name;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

    }
}
