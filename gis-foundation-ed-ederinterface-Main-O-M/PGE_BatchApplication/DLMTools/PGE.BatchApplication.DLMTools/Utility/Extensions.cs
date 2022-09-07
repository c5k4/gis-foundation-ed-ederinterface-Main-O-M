using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using Miner.Framework;

namespace PGE.BatchApplication.DLMTools.Utility
{
    /// <summary>
    ///   Provides extension methods.
    /// </summary>
    public static class Extensions
    {
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
        /// Checks the ObjectClass fields for changes or LabelText and LabelText2.
        /// </summary>
        /// <param name="pObject">The Object being created or updated.</param>
        /// <returns>Return as True if changed Else False.</returns>
        public static bool IsRowAndFeatureChanged(this IObject self, IEnumerable<string> fieldModelNames)
        {
            IRowChanges rowChanges = self as IRowChanges;
            IObjectClass objClass = self.Class;
            foreach (string mn in fieldModelNames)
            {
                List<int> fldIdx = ModelNameFacade.FieldIndicesFromModelName(objClass, mn);
                foreach (int idx in fldIdx)
                {
                    if (rowChanges.get_ValueChanged(idx))
                    {
                        return true;
                    }
                }
            }
            return false;
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
                        return (T)System.Convert.ChangeType(obj, typeof(T), System.Globalization.CultureInfo.InvariantCulture);

                    }
                }
            }

            return defaultValue;
        }

    }
}
