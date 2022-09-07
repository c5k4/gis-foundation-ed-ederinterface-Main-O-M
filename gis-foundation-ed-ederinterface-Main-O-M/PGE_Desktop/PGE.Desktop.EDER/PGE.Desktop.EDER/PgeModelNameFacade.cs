using System;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using PGE.Common.Delivery.Framework;

namespace PGE.Desktop.EDER
{
    static class PgeModelNameFacade
    {

        /// <summary>
        /// Returns an ISet of objects related to the supplied pRow of the class identified by the 
        /// relatedObjectModelName model name.
        /// </summary>
        /// <param name="pRow"></param>
        /// <param name="relatedObjectModelName"></param>
        /// <returns></returns>
        public static ISet GetRelatedObjects(IRow pRow, string relatedObjectModelName)
        {
            ISet relatedObjects = null;

            // Get the relationship class
            IRelationshipClass rc = GetRelationshipByModelName((pRow as IObject).Class, relatedObjectModelName);

            // If we found it, get the related objects
            if (rc != null)
            {
                relatedObjects = rc.GetObjectsRelatedToObject(pRow as IObject);
            }

            // Return the result
            return relatedObjects;
        }

        /// <summary>
        /// Gets the relationship class by its model name for the given object class.
        /// </summary>
        /// <param name="pObject"></param>
        /// <param name="relatedObjectModelName"></param>
        /// <returns></returns>
        public static IRelationshipClass GetRelationshipByModelName(IObjectClass pObjectClass, string relatedObjectModelName)
        {
            IRelationshipClass relClass = null;

            try
            {
                // Get a list of relationship classes
                IEnumRelationshipClass enumRelClass = pObjectClass.get_RelationshipClasses(esriRelRole.esriRelRoleAny);

                // For each one...
                enumRelClass.Reset();
                IRelationshipClass rc = enumRelClass.Next();
                while (rc != null)
                {
                    // If the current class is the one were looking for
                    if (ModelNameFacade.ContainsClassModelName(rc.OriginClass, relatedObjectModelName) ||
                        ModelNameFacade.ContainsClassModelName(rc.DestinationClass, relatedObjectModelName))
                    {
                        relClass = rc;
                        break;
                    }

                    // Move to the next one
                    rc = enumRelClass.Next();
                }
            }
            catch (Exception ex)
            {
                string msg = "Error getting the relationship. Error: " + ex.Message;
                throw;
            }

            // Return the result
            return relClass;
        }
    }
}
