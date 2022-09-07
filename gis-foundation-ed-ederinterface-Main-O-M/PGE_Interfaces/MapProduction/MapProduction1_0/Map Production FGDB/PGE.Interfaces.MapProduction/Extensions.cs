#region

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Data;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.esriSystem;
using Miner.Framework;
using Miner.Geodatabase;
using PGE.Common.Delivery.Framework;
using System.Reflection;
using System.Diagnostics;
using PGE.Common.Delivery.Diagnostics;

#endregion

namespace PGE.Interfaces.ED
{
    /// <summary>
    ///   Provides extension methods.
    /// </summary>
    public static class Extensions
    {

        public static bool Contains(this string self, string other, StringComparison comparisonType)
        {
            if (other == null)
                other = string.Empty;
            if (self == null)
                self = string.Empty;
            return self.IndexOf(other, comparisonType) != -1;
        }
        
        /// <summary>
        /// Get the description for the coded value domain
        /// </summary>
        /// <param name="pObj">pass object</param>
        /// <param name="pFieldIndex">pass the field Index for the field</param>
        /// <param name="pErrorMessage">out error message if domain is null</param>
        /// <returns>Returns description for the field value</returns>
        public static String GetDomainDescription(this IObject pObj, int pFieldIndex, out String pErrorMessage)
        {
            // Varibale Declarations
            pErrorMessage = string.Empty;
            String valueResult = string.Empty;
            ICodedValueDomain pCodedValueDomain = null;

            // Check for field index
            if (pFieldIndex == -1)
            {
                pErrorMessage = ("Field index should not be -1. Please check the field exist in layer or not.");
                return string.Empty;
            }


            IField pField = pObj.Fields.get_Field(pFieldIndex);

            // if domain is null return message
            if (pField.Domain == null)
            {
                pErrorMessage = "Domain is not assigned to field '" + pField.Name + "'.";
                return string.Empty;
            }

            IDomain pDomain = pField.Domain;

            //Check for domain type
            if (pDomain.Type == esriDomainType.esriDTCodedValue)
            {
                pCodedValueDomain = pDomain as ICodedValueDomain;
                string featureValue = pObj.get_Value(pFieldIndex).ToString();

                //Get coded value name and value and check with given value
                for (int j = 0; j < pCodedValueDomain.CodeCount; j++)
                {
                    string codedValueName = pCodedValueDomain.get_Name(j);
                    string codedValueValue = pCodedValueDomain.get_Value(j).ToString();

                    if (codedValueValue.ToString().Equals(featureValue))
                    {
                        valueResult = codedValueName;
                        break;
                    }
                }
            }
            else
            {
                pErrorMessage = "Domain is not codedvalue.";
                return string.Empty;
            }
            return valueResult;

        }

        /// <summary>
        /// Provides an enumeration for the given set.
        /// </summary>
        /// <param name="self">The instance of ISet to enumerate on.</param>
        public static IEnumerable<IObject> AsEnumerable(this ISet set)
        {
            set.Reset();
            IObject unitObj;
            while ((unitObj = set.Next() as IObject) != null)
                yield return unitObj;
        }

        public static IEnumerable<IRelationshipClass> AsEnumerable(this IEnumRelationshipClass self)
        {
            self.Reset();
            IRelationshipClass current;
            while ((current = self.Next()) != null)
            {
                yield return current;
            }
        }

        public static IEnumerable<string> AsEnumerable(this IEnumBSTR self)
        {
            self.Reset();
            string current;
            while ((current = self.Next()) != null)
                yield return current;
        }

        public static FieldInstance FieldInstanceFromModelName(this IObject self, string modelName)
        {
            return new FieldInstance(self, ModelNameFacade.FieldFromModelName(self.Class, modelName));
        }
        
        /// <summary>
        /// Gets an enumeration of all the junctions from the given edge feature.
        /// </summary>
        /// <param name="edgeFeature">The feature to get junctions from.</param>
        /// <returns>An enumeration of all the junctions from the given edge feature</returns>
        public static IEnumerable<IJunctionFeature> GetJunctions(this IEdgeFeature edgeFeature)
        {
            if (edgeFeature is IComplexEdgeFeature)
            {
                var cef = edgeFeature as IComplexEdgeFeature;
                if (cef == null)
                    yield break;
                for (int i = 0; i < cef.JunctionFeatureCount; i++)
                {
                    var jf = cef.JunctionFeature[i];
                    if(jf != null)
                        yield return jf;
                }
            }
            else
            {
                yield return edgeFeature.FromJunctionFeature;
                yield return edgeFeature.ToJunctionFeature;
            }
        }

        /// <summary>
        /// Gets all the edges from all the junctions in the given edge feature.
        /// </summary>
        /// <param name="edgeFeature">The feature to get edges from.</param>
        /// <returns>An enumeration of all the edges in all the junction on the given edge feature.</returns>
        public static IEnumerable<IEdgeFeature> GetEdges(this IEdgeFeature edgeFeature)
        {
            // Get the connected edges to the junctions.
            foreach (IJunctionFeature jf in edgeFeature.GetJunctions())
            {
                ISimpleJunctionFeature sjf = (ISimpleJunctionFeature)jf;
                for (int j = 0; j < sjf.EdgeFeatureCount; j++)
                {
                    IEdgeFeature ef = sjf.get_EdgeFeature(j);
                    if (ef == edgeFeature) continue;
                    yield return ef;
                }
            }
        }

        /// <summary>
        /// Checks if this IObject instance is the given subtype.
        /// </summary>
        /// <param name="self">The IObject instance to check.</param>
        /// <param name="subtypeName">The name of the subtype to check for.</param>
        /// <returns>true if this instance is the given subtype; otherwise false.</returns>
        public static bool IsSubtype(this IObject self, string subtypeName)
        {
            var subtypes = self.Class as ISubtypes;
            if (subtypes == null || !subtypes.HasSubtype) return false;
            IRowSubtypes rowSubtypes = (IRowSubtypes)self;
            int selfSubtypeCode = rowSubtypes.SubtypeCode;
            string selfSubtypeName = subtypes.SubtypeName[selfSubtypeCode];
            return selfSubtypeName.Equals(subtypeName, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Checks if this IObject instance is a subtype with a name containing the given string.
        /// </summary>
        /// <param name="self">The IObject instance to check.</param>
        /// <param name="subtypeName">The substring to look for in the subtype name.</param>
        /// <returns>true if this instance is the given subtype; otherwise false.</returns>
        public static bool SubtypeNameContains(this IObject self, string substring)
        {
            var subtypes = self.Class as ISubtypes;
            if (subtypes == null || !subtypes.HasSubtype) return false;
            IRowSubtypes rowSubtypes = (IRowSubtypes)self;
            int selfSubtypeCode = rowSubtypes.SubtypeCode;
            string selfSubtypeName = subtypes.SubtypeName[selfSubtypeCode];
            return selfSubtypeName.Contains(substring, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Gets the description (aka the name) of this instance's subtype.
        /// </summary>
        /// <param name="self">The instance to get the subtype description from.</param>
        /// <returns>The description of the subtype.</returns>
        public static string GetSubtypeDescription(this IObject self)
        {
            var subtypes = self.Class as ISubtypes;
            if (subtypes == null || !subtypes.HasSubtype) return null;
            IRowSubtypes rowSubtypes = (IRowSubtypes)self;
            int selfSubtypeCode = rowSubtypes.SubtypeCode;
            return subtypes.SubtypeName[selfSubtypeCode];
        }

        /// <summary>
        /// Checks to see whether this instance contains all of the given object model names.
        /// </summary>
        /// <param name="self">The instance to check for model names.</param>
        /// <param name="modelNames">A list of the model names to check for.</param>
        /// <returns>true if this instance contains the object model names; otherwise false.</returns>
        public static bool HasModelName(this IObject self, params string[] modelNames)
        {
            return ModelNameFacade.ContainsAllClassModelNames(self.Class, modelNames);
        }

        /// <summary>
        /// Removes the specified text from the string.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="text">The text to remove.</param>
        /// <returns>The input string with the specified text removed.</returns>
        public static string Remove(this string value, string text)
        {
            return value.Replace(text, string.Empty);
        }

        /// <summary>
        /// Indicates whether the specified string is null or an <see cref="F:System.String.Empty"/> string.
        /// </summary>
        /// <returns>
        /// true if the <paramref name="value"/> parameter is null or an empty string (""); otherwise, false.
        /// </returns>
        /// <param name="value">The string to test. </param>
        /// <filterpriority>1</filterpriority>
        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        /// <summary>
        /// Indicates whether a specified string is null, empty, or consists only of white-space characters.
        /// </summary>
        /// <returns>
        /// true if the <paramref name="value"/> parameter is null or <see cref="F:System.String.Empty"/>, or if <paramref name="value"/> consists exclusively of white-space characters.
        /// </returns>
        /// <param name="value">The string to test.</param>
        public static bool IsNullOrWhitespace(this string value)
        {
            return string.IsNullOrEmpty(value) || value.All(char.IsWhiteSpace);
        }


        /// <summary>
        ///   Gets the relationships that the object itself plays the specified role and filtered by origin/destination class model name.
        /// </summary>
        /// <param name="modelName"> Model Name of origin/destination object class. </param>
        /// <returns> IEnumerable of RelationshipClass. </returns>
        public static IEnumerable<IRelationshipClass> GetRelationships(this IObject self, esriRelRole relationshipRole, params string[] modelNames)
        {
            var objClass = self.Class;

            if (modelNames == null)
            {
                return objClass.RelationshipClasses[relationshipRole].AsEnumerable();
            }
            
            return objClass.RelationshipClasses[relationshipRole]
                           .AsEnumerable()
                           .Where(rc => ModelNameFacade.ContainsClassModelName(rc.OriginClass, modelNames)
                                        || ModelNameFacade.ContainsClassModelName(rc.DestinationClass, modelNames));
        }
        
        /// <summary>
        ///   Gets the relationships that the object itself plays the specified role and filtered by origin/destination class model name.
        /// </summary>
        /// <param name="modelName"> Model Name of origin/destination object class. </param>
        /// <returns> IEnumerable of RelationshipClass. </returns>
        public static IEnumerable<IRelationshipClass> GetRelationships(this IObject self, params string[] modelNames)
        {
            var objClass = self.Class;
            var classes = objClass.RelationshipClasses[esriRelRole.esriRelRoleAny].AsEnumerable();

            if (modelNames == null)
            {
                return classes;
            }

            return classes.Where(rc => ModelNameFacade.ContainsClassModelName(rc.OriginClass, modelNames)
                                        || ModelNameFacade.ContainsClassModelName(rc.DestinationClass, modelNames));
        }

        /// <summary>
        ///   Gets the relationships that the object itself plays the specified role and filtered by origin/destination class model name.
        /// </summary>
        /// <param name="modelName"> Model Name of origin/destination object class. </param>
        /// <returns> IEnumerable of RelationshipClass. </returns>
        public static IEnumerable<IRelationshipClass> GetRelationships(this IFeature self, params string[] modelNames)
        {
            return GetRelationships((IObject)self, esriRelRole.esriRelRoleAny, modelNames);
        }

        public static string GetFieldAlias(this IObject self, string modelName)
        {
            return ModelNameFacade.FieldFromModelName(self.Class, modelName).AliasName;
        }

        /// <summary>
        ///   Returns the given field's value, or null if no field with the given name is found.
        /// </summary>
        /// <param name="fieldName"> The name of the field to look for. </param>
        /// <param name="useDomain"> true if the field's domain should be used; otherwise false. </param>
        /// <returns>The value of the given field.</returns>
        public static object GetFieldValue(this IObject self, string fieldName = null, bool useDomain = true, string modelName = null)
        {
            var callingMethod = new StackFrame(1).GetMethod();
            var _logger = new Log4NetLogger(callingMethod.DeclaringType, "MapProduction1.0.log4net.config");
            
            IField field;
            int pos;
            if (fieldName == null)
            {
                if (modelName == null)
                {
                    _logger.Debug("Misuse of IObject.GetFieldValue(...) - must specify either a field or model name.");
                    throw new NotSupportedException("field selection requires either a field name or a field model name.");
                }

                _logger.Debug("Looking for field with model name " + modelName + "...");

                field = ModelNameFacade.FieldFromModelName(self.Class, modelName);
                if (field == null)
                {
                    _logger.Debug("Couldn't find field with model name " + modelName);
                    return null;
                }
                fieldName = field.Name;
                _logger.Debug("Found field " + fieldName + " with model " + modelName);
            }

            pos = self.Class.FindField(fieldName);
            if (pos == -1) return null;
            _logger.Debug("Field index of " + fieldName + " is " + pos);

            field = self.Fields.Field[pos];
            var valueAsObject = self.Value[pos];
            if (valueAsObject == null)
            {
                _logger.Debug(fieldName + " = <null>");
                return null;
            }

            _logger.Debug(valueAsObject.GetType().Name + ' ' + fieldName + " = " + valueAsObject.ToString());
            
            if (!useDomain)
                return valueAsObject;
            _logger.Debug("Looking up domain for field " + fieldName);
            // If there is a domain configured and it's specified to use a domain then get the description.
            var valueAsText = valueAsObject.Convert(String.Empty);
            if (valueAsText != String.Empty)
            {
                //if the domain is not assigned for each sub-type then ArcObjects throws an error while trying to get the 
                //domain by sub-type.
                //resolution is to check the domain on field first and if not found then for subtypes
                var domain = field.Domain;
                if (domain == null)
                {
                    domain = ((ISubtypes)self.Table).HasSubtype
                        ? ((ISubtypes)self.Table).get_Domain(((IRowSubtypes)self).SubtypeCode, fieldName)
                        : null;
                }
                var result = domain == null ? valueAsObject : domain.Lookup(valueAsText) ?? valueAsObject;
                _logger.Debug(fieldName + " domain result is " + result.ToString());
                return result;
            }
            return null;
        }

        /// <summary>
        /// Attempts to match the domain code to a name, optionally returning a default string if the code is not found.
        /// If no default value string is provided, null is returned.
        /// </summary>
        /// <param name="domain">The domain to search on.</param>
        /// <param name="domainCode">The code to look for in the domain.</param>
        /// <param name="defaultValue">An optional default value to return in liu of a result.</param>
        /// <returns></returns>
        private static object Lookup(this IDomain domain, string domainCode, string defaultValue = null)
        {
            var codedValueDomain = domain as ICodedValueDomain;
            if (codedValueDomain != null)
            {
                for (var i = 0; i < codedValueDomain.CodeCount; i++)
                {
                    var code = codedValueDomain.Value[i].Convert(String.Empty);
                    if (domainCode.Equals(code)) return codedValueDomain.Name[i];
                }
            }
            return defaultValue;
        }


        /// <summary>
        /// Gets information about all the fields on this instance.
        /// </summary>
        /// <param name="self">The instance to gather field info on.</param>
        /// <returns>An enumeration of FieldInstances containing information about the fields on an object.</returns>
        public static IEnumerable<FieldInstance> GetFields(this IObject self)
        {
            for (var i = 0; i < self.Fields.FieldCount; i++)
            {
                var field = self.Fields.Field[i];
                if (field != null) yield return new FieldInstance(self, field);
            }
        }

        /// <summary>
        /// Gets information about all the fields on this instance, filtered by model name.
        /// </summary>
        /// <param name="self">The instance to gather field info on.</param>
        /// <returns>An enumeration of FieldInstances containing information about the fields on an object.</returns>
        public static IEnumerable<FieldInstance> GetFields(this IObject self, params string[] modelName)
        {
            return self.GetFields().Where(f => f.HasModel(modelName));
        }

        /// <summary>
        /// Gets objects related to this instance, optionally filtered by model name, alias, and/or role.
        /// </summary>
        /// <param name="self">The instance the objects are related to.</param>
        /// <param name="aliasName">The alias of objects to filter by.</param>
        /// <param name="modelName">The model name to filter by.</param>
        /// <param name="relationshipRole">The relationship role to filter by.</param>
        /// <returns>An enumeration of IObjects related to this instance.</returns>
        public static IEnumerable<IObject> GetRelatedObjects(this IObject self, string aliasName = null, esriRelRole relationshipRole = esriRelRole.esriRelRoleAny, params string[] modelNames)
        {
            var relatedObjects = self.GetRelationships(relationshipRole, modelNames)
                                     .Select(relationship => relationship.GetObjectsRelatedToObject(self).AsEnumerable())
                                     .SelectMany(relSet => relSet);

            if (aliasName == null)
            {
                return relatedObjects;
            }

            return relatedObjects.Where(o => o.Class.AliasName == aliasName);
        }


        /// <summary>
        /// Checks to see if this instance has the given subtype code.
        /// </summary>
        /// <param name="self">The instance to check subtype on.</param>
        /// <param name="subtypeCode">The subtype code to check for.</param>
        /// <returns>true if this instance has the given subtype; otherwise false.</returns>
        public static bool HasSubtypeCode(this IObject self, int subtypeCode)
        {
            return ((IRowSubtypes)self).SubtypeCode == subtypeCode;
        }

        /// <summary>
        /// Gets this instance's subtype code and casts it to the given enum type, which must be specified as a type parameter.
        /// </summary>
        /// <typeparam name="T">The enum type to convert to.</typeparam>
        /// <param name="self">The instance to gather subtype info on.</param>
        /// <returns>An enum value from the subtype code.</returns>
        public static T SubtypeCodeAsEnum<T>(this IObject self)
            where T : struct
        {
            var enumType = typeof(T);
            if (!enumType.IsEnum)
            {
                throw new ArgumentException("Type argument must be Enum type", "T");
            }

            var subtypes = (ISubtypes)self.Class;

            if (subtypes == null || !subtypes.HasSubtype)
            {
                return default(T);
            }

            IRowSubtypes rowSubtypes = (IRowSubtypes)self;
            return (T)(object)rowSubtypes.SubtypeCode;
        }
        


        /// <summary>
        ///   Refreshes the annotation on this instance.
        /// </summary>
        /// <param name="self">The instance of IObject to refresh annotation on.</param>
        public static void RefreshAnnotation(this IObject self)
        {
            if (Document.ActiveView == null)
                return;

            var oclass = self.Class;
            var enumRelClass = oclass.RelationshipClasses[esriRelRole.esriRelRoleOrigin];
            enumRelClass.Reset();
            IRelationshipClass relClass;
            while ((relClass = enumRelClass.Next()) != null)
            {
                var destClass = relClass.DestinationClass as IFeatureClass;
                if (destClass == null || destClass.FeatureType != esriFeatureType.esriFTAnnotation)
                    continue;

                var relSet = relClass.GetObjectsRelatedToObject(self);
                if (relSet.Count == 0) continue; // There are no related objects continue.

                // We need to get a combined extent of all the related annotations. (Typically there is only one).
                var extent = new EnvelopeClass();
                IFeature relObj;
                while ((relObj = relSet.Next() as IFeature) != null)
                {
                    if (relObj.ShapeCopy == null || relObj.ShapeCopy.IsEmpty) continue;

                    extent.Union(relObj.ShapeCopy.Envelope);
                }

                // There are no annotation geometries.
                if (extent.IsEmpty) continue;

                // Call partial refresh
                Document.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, extent);
            }
        }

        /// <summary>
        ///   Casts or converts this instance to the given type if possible.
        ///   Otherwise returns a default value.
        /// </summary>
        /// <typeparam name="T"> The type to cast this instance to. </typeparam>
        /// <param name="obj"> The object to cast. </param>
        /// <param name="defaultValue"> If the cast fails, yield this value. </param>
        /// <returns> This instance converted to T; otherwise defaultValue. </returns>
        public static T Convert<T>(this object obj, T defaultValue = default(T))
        {
            if (obj != null && obj != DBNull.Value)
            {
                string objString = obj.ToString();
                if (objString.IsNullOrWhitespace())
                {
                    return defaultValue;
                }
                else
                {
                    if (obj is T)
                    {
                        return (T)obj;
                    }
                    else
                    {
                        try
                        {
                            return (T)System.Convert.ChangeType(obj, typeof(T), System.Globalization.CultureInfo.InvariantCulture);
                        }
                        catch
                        {
                            return defaultValue;
                        }

                    }
                }
            }

            return defaultValue;
        }

        /// <summary>
        ///   Attempts to cast this instance to the given type. Includes special handling for DBNull.
        /// </summary>
        /// <typeparam name="T"> The type to cast this instance to. </typeparam>
        /// <param name="instance"> The instance to cast. </param>
        /// <param name="result"> This instance casted to T; otherwise defaultValue. </param>
        /// <param name="defaultValue"> If the cast fails, yield this value. </param>
        /// <returns> True if the cast succeeded; otherwise false. </returns>
        public static bool TryCast<T>(this object instance, out T result, T defaultValue = default (T))
        {
            if (instance is T)
            {
                result = (T)instance;
                return true;
            }

            var canCast = !System.Convert.IsDBNull(instance);
            result = canCast
                         ? (T)System.Convert.ChangeType(instance, typeof(T), CultureInfo.InvariantCulture)
                         : defaultValue;
            return canCast;
        }

        /// <summary>
        /// Converts the given domain into a dictionary for faster lookup, converting each value to the given type.
        /// If a domain name's value is invalid, uses the default value for that type if no value is given.
        /// Really only useful for looking up on really large domains (17+ codes) or large numbers of them.
        /// </summary>
        /// <typeparam name="T">The type to convert the domain values to.</typeparam>
        /// <param name="domain">The instance of domain to convert to a dictionary.</param>
        /// <param name="defaultValue">The value to use in the event a domain value is invalid.</param>
        /// <returns>A dictionary representing the domain.</returns>
        public static Dictionary<string, T> ToDictionary<T>(this IDomain domain, T defaultValue = default (T))
        {
            if (domain is ICodedValueDomain)
            {
                var codedValueDomain = (ICodedValueDomain)domain;
                return codedValueDomain.CodeCount == 0
                           ? new Dictionary<string, T>()
                           : Enumerable.Range(0, codedValueDomain.CodeCount - 1)
                                 .ToDictionary(i => codedValueDomain.Name[i],
                                               i => (T)codedValueDomain.Value[i].Convert(defaultValue));
            }
            throw new NotSupportedException("IDomans which are not ICodedValueDomains are not supported.");
        }

        /// <summary>
        ///   Concatenates the given enumeration of strings, optionally with a separator between each.
        /// </summary>
        /// <param name="items"> </param>
        /// <param name="seperator"> </param>
        /// <returns> </returns>
        public static string Concatenate(this IEnumerable<string> items, string seperator = "")
        {
            if (seperator == null) seperator = "";
            var builder = new StringBuilder();
            builder.Append(items.FirstOrDefault() ?? "");
            foreach (var item in items.Skip(1))
            {
                builder.Append(seperator);
                builder.Append(item);
            }
            return builder.ToString();
        }

        #region Nested Types
        class DisposableRow : IRow, IDisposable
        {
            private readonly IRow _basis;
            bool _disposed;

            /// <summary>
            /// Creates a new instance of this class.
            /// </summary>
            /// <param name="basis">The row this instance will handle release for.</param>
            public DisposableRow(IRow basis)
            {
                _basis = basis;
            }

            /// <summary>
            /// Finalizes release of the row in the event that Dispose is not called.
            /// </summary>
            ~DisposableRow()      
            {
                Dispose(false);
            }

            /// <summary>
            ///    Performs application-defined tasks associated with freeing, releasing, or
            ///    resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            /// <summary>
            ///    Performs application-defined tasks associated with freeing, releasing, or
            ///    resetting unmanaged resources.
            /// </summary>
            /// <param name="disposing">If false, this method was called by the finalizer; otherwise true.</param>
            private void Dispose(bool disposing)
            {
                if (!_disposed && _basis != null)
                {
                    //if (disposing)
                    //{
                        // TODO: Call dispose on private (and protected, if this class is sealed) IDisposable fields here.
                        // NOTE: There are no such fields in this class, so this code is merely provided for future generations.
                    //}

                    // Release unmanaged resources here.
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(_basis);
                }
                _disposed = true;
            }

            /// <summary>
            /// Deletes the row.
            /// </summary>
            public void Delete()
            {
                if (_disposed) throw new ObjectDisposedException("DisposableRow");
                _basis.Delete();
            }


            /// <summary>
            /// The fields Collection for this row buffer.
            /// </summary>
            public IFields Fields
            {
                get
                {
                    if (_disposed) throw new ObjectDisposedException("DisposableRow");
                    return _basis.Fields;
                }
            }

            /// <summary>
            /// Indicates if the row has an OID.
            /// </summary>
            public bool HasOID
            {
                get
                {
                    if (_disposed) throw new ObjectDisposedException("DisposableRow");
                    return _basis.HasOID;
                }
            }

            /// <summary>
            /// The row's OID.
            /// </summary>
            public int OID
            {
                get
                {
                    if (_disposed) throw new ObjectDisposedException("DisposableRow");
                    return _basis.OID;
                }
            }

            /// <summary>
            /// Stores the row.
            /// </summary>
            public void Store()
            {
                if (_disposed) throw new ObjectDisposedException("DisposableRow");
                _basis.Store();
            }

            /// <summary>
            /// The Table for the row.
            /// </summary>
            public ITable Table
            {
                get
                {
                    if (_disposed) throw new ObjectDisposedException("DisposableRow");
                    return _basis.Table;
                }
            }

            /// <summary>
            /// Gets the value of the cell at the given index in the row.
            /// </summary>
            /// <param name="Index">The index of the cell to get the value from.</param>
            /// <returns>The value of the cell at the given index.</returns>
            public object get_Value(int index)
            {
                if (_disposed) throw new ObjectDisposedException("DisposableRow");
                return _basis.get_Value(index);
            }

            /// <summary>
            /// Sets the value of the cell at the given index in the row.
            /// </summary>
            /// <param name="Index">The index of the cell to get the value from.</param>
            /// <param name="value">The value of the cell at the given index.</param>
            public void set_Value(int index, object value)
            {
                if (_disposed) throw new ObjectDisposedException("DisposableRow");
                _basis.set_Value(index, value);
            }
        }
        public class CursorEnumerator : IEnumerator<IRow>, IEnumerable<IRow>
        {
            private readonly Func<ICursor> _getCursor;
            ICursor _currentCursor;
            IRow _currentRow;
            bool _disposed;

            /// <summary>
            /// Creates an instance of this class.
            /// </summary>
            /// <param name="getCursor">A method which will return an ICursor.</param>
            public CursorEnumerator(Func<ICursor> getCursor)
            {
                _getCursor = getCursor;
            }

            /// <summary>
            ///     Gets the current element in the collection.
            /// </summary>
            /// <returns>
            ///     The current element in the collection.
            /// </returns>
            public IRow Current
            {
                get
                {
                    if (_disposed) throw new ObjectDisposedException("CursorEnumerator");
                    return _currentRow;
                }
            }

            ~CursorEnumerator()      
            {
                Dispose(false);
            }

            /// <summary>
            ///    Performs application-defined tasks associated with freeing, releasing, or
            ///    resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            /// <summary>
            ///    Performs application-defined tasks associated with freeing, releasing, or
            ///    resetting unmanaged resources.
            /// </summary>
            /// <param name="disposing">If false, this method was called by the finalizer; otherwise true.</param>
            private void Dispose(bool disposing)
            {
                if (!_disposed)
                {
                    if (_currentCursor != null)
                    {
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(_currentCursor);
                        _currentCursor = null;
                    }
                }
                _disposed = true;
            }

            /// <summary>
            ///     Gets the current element in the collection.
            /// </summary>
            /// <returns>
            ///     The current element in the collection.
            /// </returns>
            /// <exception cref="System.InvalidOperationException">
            ///     The enumerator is positioned before the first element of the collection or
            ///     after the last element.
            /// </exception>
            object System.Collections.IEnumerator.Current
            {
                get
                {
                    return Current;
                }
            }

            /// <summary>
            ///     Advances the enumerator to the next element of the collection.
            /// </summary>
            /// <returns>
            ///     true if the enumerator was successfully advanced to the next element; false
            ///     if the enumerator has passed the end of the collection.
            /// </returns>
            /// <exception cref="System.InvalidOperationException">
            ///     The collection was modified after the enumerator was created.
            /// </exception>
            public bool MoveNext()
            {
                if (_disposed) throw new ObjectDisposedException("CursorEnumerator");
                if (_currentCursor == null) return false;
                var nextRow = _currentCursor.NextRow();
                if(nextRow == null) return false;
                _currentRow = new DisposableRow(nextRow);
                return true;
            }

            /// <summary> 
            ///    Sets the enumerator to its initial position, which is before the first element
            ///    in the collection.
            /// </summary>
            /// <exception cref="System.InvalidOperationException">
            ///     The collection was modified after the enumerator was created.
            /// </exception>
            /// <exception cref="System.InvalidOperationException">
            ///     The collection was modified after the enumerator was created.
            /// </exception>
            public void Reset()
            {
                if (_disposed) throw new ObjectDisposedException("CursorEnumerator");
                if (_currentCursor != null)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(_currentCursor);
                _currentCursor = _getCursor();
                _currentRow = null;
            }

            ///<summary>
            ///     Returns an enumerator that iterates through the collection.
            ///</summary>
            ///<returns>
            ///     A System.Collections.Generic.IEnumerator<IRow> that can be used to iterate through
            ///     the collection.
            ///</returns>
            public IEnumerator<IRow> GetEnumerator()
            {
                if (_disposed) throw new ObjectDisposedException("CursorEnumerator");
                Reset();
                return this;
            }

            ///<summary>
            ///     Returns an enumerator that iterates through the collection.
            ///</summary>
            ///<returns>
            ///     A System.Collections.Generic.IEnumerator that can be used to iterate through
            ///     the collection.
            ///</returns>
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
        #endregion
    }
}