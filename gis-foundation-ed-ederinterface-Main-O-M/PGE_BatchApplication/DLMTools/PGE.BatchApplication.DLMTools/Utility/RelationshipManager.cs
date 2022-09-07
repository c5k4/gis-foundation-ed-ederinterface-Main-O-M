using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PGE.BatchApplication.DLMTools.Utility.Logs;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;

namespace PGE.BatchApplication.DLMTools.Utility
{
    /// <summary>
    /// Manages relationship caches and special methods for accessing related objects using the cache.
    /// </summary>
    internal static class RelationshipManager
    {
        static readonly Log4NetFileHelper _logHelper = new Log4NetFileHelper(MethodBase.GetCurrentMethod().DeclaringType);

        // This got a little messy. Sorry!
        // Dictionary<ObjectClassID, Dictionary<ModelName, List<RelationshipClass;RelationshipRole>>>
        private static Dictionary<int, Dictionary<string, List<KeyValuePair<IRelationshipClass, esriRelRole>>>> _relCache = new Dictionary<int, Dictionary<string, List<KeyValuePair<IRelationshipClass, esriRelRole>>>>();

        /// <summary>
        /// Gets a stored cache item for an object class based on the class, model name, and relationship role.
        /// </summary>
        /// <param name="objClass">The object class from which to find relationships.</param>
        /// <param name="relationshipRole">The role in which you would like the object class to serve in the relationship.</param>
        /// <param name="classMN">The model name that related object classes must contain in order to be considered eligible.</param>
        /// <returns>A list of eligible relationship classes.</returns>
        internal static List<IRelationshipClass> GetRelCache(IObjectClass objClass, esriRelRole relationshipRole, string classMN)
        {
            List<KeyValuePair<IRelationshipClass, esriRelRole>> selectedList = new List<KeyValuePair<IRelationshipClass, esriRelRole>>();

            if (_relCache.ContainsKey(objClass.ObjectClassID) && _relCache[objClass.ObjectClassID].ContainsKey(classMN))
                selectedList = _relCache[objClass.ObjectClassID][classMN];
            else
            {
                // Get all relationship classes
                List<KeyValuePair<IRelationshipClass, esriRelRole>> eligibleRelClasses = new List<KeyValuePair<IRelationshipClass, esriRelRole>>();
                IEnumRelationshipClass allRelClasses = objClass.get_RelationshipClasses(relationshipRole);
                allRelClasses.Reset();
                for (IRelationshipClass relClass = allRelClasses.Next(); relClass != null; relClass = allRelClasses.Next())
                {
                    esriRelRole objRole = relationshipRole;
                    if (objRole == esriRelRole.esriRelRoleAny)
                        objRole = (objClass.ObjectClassID == relClass.OriginClass.ObjectClassID) ? esriRelRole.esriRelRoleOrigin : esriRelRole.esriRelRoleDestination;

                    IObjectClass relatedOC = (objRole == esriRelRole.esriRelRoleOrigin) ? relClass.DestinationClass : relClass.OriginClass;

                    if (ModelNames.Manager.ContainsClassModelName(relatedOC, classMN))
                        eligibleRelClasses.Add(new KeyValuePair<IRelationshipClass, esriRelRole>(relClass, objRole));
                }

                // Add to cache
                if (!_relCache.ContainsKey(objClass.ObjectClassID))
                    _relCache.Add(objClass.ObjectClassID, new Dictionary<string, List<KeyValuePair<IRelationshipClass, esriRelRole>>>());
                if (!_relCache[objClass.ObjectClassID].ContainsKey(classMN))
                    _relCache[objClass.ObjectClassID].Add(classMN, eligibleRelClasses);

                selectedList = eligibleRelClasses;
            }

            if (relationshipRole == esriRelRole.esriRelRoleAny)
                return selectedList.Select(l => l.Key).ToList();
            else
                return selectedList.Where(l => l.Value == relationshipRole).Select(l => l.Key).ToList();
        }

        /// <summary>
        /// Clears the static cache of relationships.
        /// </summary>
        internal static void ClearRelCache()
        {
            _relCache = new Dictionary<int, Dictionary<string, List<KeyValuePair<IRelationshipClass, esriRelRole>>>>();
            _logHelper.DefaultLogger.Info("Relationship cache cleared.");
        }
        /// <summary>
        /// Uses the relationship cache to find the relevant relationship, then finds all related objects with the given model name.
        /// Related objects that are being deleted will not be returned.
        /// </summary>
        /// <param name="obj">The object from which to find relationships.</param>
        /// <param name="relationshipRole">The role in which you would like the object to serve in the relationship.</param>
        /// <param name="classMN">The model name that related object classes must contain in order to be considered eligible.</param>
        /// <param name="deletedRelatedObject">If applicable, a tuple of ObjectClassID/ObjectID corresponding to a relationship object that is currently being deleted.</param>
        /// <returns>A list of related objects.</returns>
        internal static List<IObject> GetRelated(IObject obj, esriRelRole relationshipRole, string classMN, KeyValuePair<int,int>? deletedRelatedObject)
        {
            // Get all relationship classes
            List<IRelationshipClass> eligibleRelClasses = GetRelCache(obj.Class, relationshipRole, classMN);

            // Loop through, check for model name.
            List<IObject> allRelatedObjs = new List<IObject>();
            foreach (IRelationshipClass relClass in eligibleRelClasses)
            {
                ISet relatedObjects = relClass.GetObjectsRelatedToObject(obj);
                relatedObjects.Reset();
                for (IObject relObj = relatedObjects.Next() as IObject; relObj != null; relObj = relatedObjects.Next() as IObject)
                {
                    //Filter out any objects being deleted from the returnset.
                    if (deletedRelatedObject.HasValue
                        && deletedRelatedObject.Value.Key == relObj.Class.ObjectClassID
                        && deletedRelatedObject.Value.Value == relObj.OID)
                        continue;

                    allRelatedObjs.Add(relObj);
                }
            }

            return allRelatedObjs;
        }
    }
}
