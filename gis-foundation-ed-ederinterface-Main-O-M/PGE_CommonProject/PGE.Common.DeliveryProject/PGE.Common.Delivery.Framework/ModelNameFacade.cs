using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Carto;
using Miner.Interop;

namespace PGE.Common.Delivery.Framework
{
    #region Model Name Framework
    /// <summary>
    /// Class to Manage ArcFM Model Name Framework
    /// </summary>
    public class ModelNameFacade
    {
        /// <summary>
        /// Static instance of ModelName Manager
        /// </summary>
        private static IMMModelNameManager _mnManager = Miner.Geodatabase.ModelNameManager.Instance;

        /// <summary>
        /// Instance of the ModelNameManager
        /// </summary>
        public static IMMModelNameManager ModelNameManager
        {
            get { return _mnManager; }
            set { _mnManager = value; }
        }

        /// <summary>
        /// Gets the feature class from dataset.
        /// </summary>
        /// <param name="dataset">The dataset</param>
        /// <param name="modelName">Name of the s model.</param>
        /// <returns>IFeatureClass</returns>
        public static IFeatureClass FeatureClassFromDataset(IDataset dataset, string modelName)
        {
            try
            {
                IEnumFeatureClass pEnumFC = ModelNameManager.FeatureClassesFromModelNameDS(dataset, modelName);
                pEnumFC.Reset();
                return pEnumFC.Next();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// Given a ModelName and Workspace gets the First featureclass with the given modelname from the Workspace
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="modelName"></param>
        /// <returns></returns>
        public static IFeatureClass FeatureClassByModelName(IWorkspace ws, string modelName)
        {
            try
            {
                IEnumFeatureClass pEnumFC = ModelNameManager.FeatureClassesFromModelNameWS(ws, modelName);
                pEnumFC.Reset();
                return pEnumFC.Next();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// Given a Workspace and ModelName gets the Objectclass with the given modelname from workspace
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="modelName"></param>
        /// <returns></returns>
        public static IObjectClass ObjectClassByModelName(IWorkspace ws, string modelName)
        {
            try
            {
                IMMEnumObjectClass pEnumOC = ModelNameManager.ObjectClassesFromModelNameWS(ws, modelName);
                pEnumOC.Reset();
                return pEnumOC.Next();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// Finds the <see cref="IRelationshipClass"/> collection using the specified <paramref name="oclass"/> and <paramref name="relRole"/> that has been assigned
        /// the <paramref name="modelNames"/>.
        /// </summary>
        /// <param name="oclass">The oclass.</param>
        /// <param name="relRole">
        /// The rel role that the passed in object plays in the relationship. 
        /// if esriRelRoleAny is passed all the relationship in which the objectclass participates will be used. 
        /// if esriRelRoleOrigin is passed all the relationship in which the passed in objectclass acts as the origin class will be used
        /// if esriRelRoleDestination is passed all the relationship in which the passed in objectclass acts as the Destination class will be used.
        /// </param>
        /// <param name="modelNames">The model names.</param>
        /// <returns>
        /// The <see cref="IRelationshipClass"/>, otherwise <c>null</c>.
        /// </returns>
        /// <exception cref="NullReferenceException">Unable to obtain the relationship class with the model name.</exception>
        public static List<IRelationshipClass> RelationshipClassesFromModelName(IObjectClass oclass, esriRelRole relRole, params string[] modelNames)
        {
            var relClasses = new List<IRelationshipClass>();

            var enumClasses = oclass.RelationshipClasses[relRole];
            try
            {
                enumClasses.Reset();

                IRelationshipClass relClass;
                while ((relClass = enumClasses.Next()) != null)
                {
                    switch (relRole)
                    {
                        case esriRelRole.esriRelRoleAny:
                            if (ContainsClassModelName(relClass.DestinationClass, modelNames) || ContainsClassModelName(relClass.OriginClass, modelNames))
                                relClasses.Add(relClass);

                            break;
                        case esriRelRole.esriRelRoleDestination:
                            if (ContainsClassModelName(relClass.OriginClass, modelNames))
                                relClasses.Add(relClass);

                            break;
                        case esriRelRole.esriRelRoleOrigin:
                            if (ContainsClassModelName(relClass.DestinationClass, modelNames))
                                relClasses.Add(relClass);

                            break;
                    }
                }
            }
            finally
            {
                while (Marshal.ReleaseComObject(enumClasses) > 0) { }
            }

            if (relClasses.Count == 0)
                throw new NullReferenceException("Unable to obtain the object class with the model name: " + string.Join(" or ", modelNames) + " via the relationship to the " + oclass.AliasName + " class with a role of " + relRole + ".");

            return relClasses;
        }

        /// <summary>
        /// Finds the <see cref="IRelationshipClass"/> using the specified <paramref name="oclass"/> and <paramref name="relRole"/> that has been assigned
        /// the <paramref name="modelNames"/>.
        /// </summary>
        /// <param name="oclass">The oclass.</param>
        /// <param name="relRole">The rel role.</param>
        /// <param name="modelNames">The model names.</param>
        /// <returns>
        /// The <see cref="IRelationshipClass"/>, otherwise <c>null</c>.
        /// </returns>
        /// <exception cref="NullReferenceException">Unable to obtain the relationship with the model name.</exception>
        public static IRelationshipClass RelationshipClassFromModelName(IObjectClass oclass, esriRelRole relRole,
            params string[] modelNames)
        {
            var enumClasses = oclass.RelationshipClasses[relRole];
            try
            {
                enumClasses.Reset();

                IRelationshipClass relClass;
                while ((relClass = enumClasses.Next()) != null)
                {
                    switch (relRole)
                    {
                        case esriRelRole.esriRelRoleAny:
                            if (ContainsClassModelName(relClass.DestinationClass, modelNames) ||
                                ContainsClassModelName(relClass.OriginClass, modelNames))
                                return relClass;

                            break;
                        case esriRelRole.esriRelRoleDestination:
                            if (ContainsClassModelName(relClass.OriginClass, modelNames))
                                return relClass;

                            break;
                        case esriRelRole.esriRelRoleOrigin:
                            if (ContainsClassModelName(relClass.DestinationClass, modelNames))
                                return relClass;

                            break;
                    }
                }
            }
            finally
            {
                while (Marshal.ReleaseComObject(enumClasses) > 0) { }
            }

            throw new NullReferenceException("Unable to obtain the object class with the model name: " + string.Join(" or ", modelNames) + " via the relationship to the " + oclass.AliasName + " class with a role of " + relRole + ".");
        }

        /// <summary>
        /// Gets a List of IFEatureLayer given a IMap and ModelName. 
        /// If all is true then all the featurelayer with the given modelname is returned 
        /// else the first featurelayer with the given modelname is returned
        /// </summary>
        /// <param name="featureClassModelName"></param>
        /// <param name="map"></param>
        /// <param name="all"></param>
        /// <returns></returns>
        public static List<IFeatureLayer> FeatureLayerByModelName(string featureClassModelName, IMap map, bool all)
        {
            List<IFeatureLayer> flList = new List<IFeatureLayer>();
            try
            {
                UID uid = new UIDClass();
                uid.Value = "{40A9E885-5533-11d0-98BE-00805F7CED21}";
                IEnumLayer enumLayer = map.get_Layers(uid, true);
                enumLayer.Reset();
                IFeatureLayer featLayer = null;
                while ((featLayer = (IFeatureLayer)enumLayer.Next()) != null)
                {
                    if (featLayer.FeatureClass == null) continue;
                    if (ModelNameFacade.ModelNameManager.ContainsClassModelName(featLayer.FeatureClass, featureClassModelName))
                    {
                        flList.Add(featLayer);
                        if (!all)
                        {
                            return flList;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            return flList;
        }

        /// <summary>
        /// Given a ObjectClass and a modelname gets the first field that has the modelname assigned.
        /// </summary>
        /// <param name="pOC">An object of type IObjectClass</param>
        /// <param name="sModelName">Modelname to check</param>
        /// <returns>Returns the first field that has the given modelname assigned</returns>
        public static IField FieldFromModelName(IObjectClass pOC, string sModelName)
        {
            try
            {
                return ModelNameManager.FieldFromModelName(pOC, sModelName);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// Given a ObjectClass and a modelname gets the first field index that has the modelname assigned.
        /// </summary>
        /// <param name="objectClass">An object of type IObjectClass</param>
        /// <param name="modelName">Modelname to check</param>
        /// <returns>Returns the first field index that has the given modelname assigned</returns>
        public static int FieldIndexFromModelName(IObjectClass objectClass, string modelName)
        {
            int iRetVal = -1;
            try
            {
                IField field = FieldFromModelName(objectClass, modelName);
                if (field != null)
                    return objectClass.Fields.FindField(field.Name);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            return iRetVal;
        }

        /// <summary>
        /// Given a ObjectClass and a modelname gets the first field name that has the modelname assigned.
        /// </summary>
        /// <param name="objectClass">An object of type IObjectClass</param>
        /// <param name="modelName">Modelname to check</param>
        /// <returns>Returns the first field name that has the given modelname assigned</returns>
        public static string FieldNameFromModelName(IObjectClass objectClass, string modelName)
        {
            string sRetVal = string.Empty;
            try
            {
                IField field = FieldFromModelName(objectClass, modelName);
                if (field != null)
                    return field.Name;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            return sRetVal;
        }

        /// <summary>
        /// Given a list of ModelNames returns a dictionary of modelname string and List of all the fields that has the modelname assigned
        /// </summary>
        /// <param name="objectClass">An object of type IObjectClass</param>
        /// <param name="fieldModelNames">Modelnames that should be searched in the given objectclass</param>
        /// <returns>A dictionary of String and List field names that has the modelname assigned </returns>
        public static Dictionary<string, List<string>> FieldNamesFromModelNames(IObjectClass objectClass, List<string> fieldModelNames)
        {
            try
            {
                Dictionary<string, List<string>> fieldNames = new Dictionary<string, List<string>>();
                foreach (string fieldMN in fieldModelNames)
                {
                    fieldNames.Add(fieldMN, FieldNamesFromModelName(objectClass, fieldMN));
                }
                return fieldNames;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// Given a modelname and objectclass gets all the field names that has the given modelname assigned.
        /// </summary>
        /// <param name="objectClass">An object of type IObjectClass</param>
        /// <param name="modelName">Modelname to check</param>
        /// <returns>Returns a list of field names that has the given modelname assigned. If the there are no fields with the modelname an empty list is returned.</returns>
        public static List<string> FieldNamesFromModelName(IObjectClass objectClass,string modelName)
        {
            List<string> retVal = new List<string>();
            IMMEnumField enumField = ModelNameFacade.ModelNameManager.FieldsFromModelName(objectClass, modelName);
            enumField.Reset();
            IField fld = enumField.Next();
            while (fld != null)
            {
                retVal.Add(fld.Name);
                fld = enumField.Next();
            }
            return retVal;
        }

        /// <summary>
        /// Given a modelname and objectclass gets all the field names that has the given modelname assigned.
        /// </summary>
        /// <param name="objectClass">An object of type IObjectClass</param>
        /// <param name="modelName">Modelname to check</param>
        /// <returns>Returns a list of field names that has the given modelname assigned. If the there are no fields with the modelname an empty list is returned.</returns>
        public static List<int> FieldIndicesFromModelName(IObjectClass objectClass, string modelName)
        {
            List<int> retVal = new List<int>();
            IMMEnumField enumField = ModelNameFacade.ModelNameManager.FieldsFromModelName(objectClass, modelName);
            enumField.Reset();
            IField fld = enumField.Next();
            while (fld != null)
            {
                retVal.Add(objectClass.FindField(fld.Name)); 
                fld = enumField.Next();
            }
            return retVal;
        }

        /// <summary>
        /// Given a list of ModelNames returns a dictionary of modelname string and List of all the field indices that has the modelname assigned
        /// </summary>
        /// <param name="objectClass">An object of type IObjectClass</param>
        /// <param name="fieldModelNames">Modelnames that should be searched in the given objectclass</param>
        /// <returns>A dictionary of String and List field indices that has the modelname assigned </returns>
        public static Dictionary<string, List<int>> FieldIndicesFromModelNames(IObjectClass objectClass, List<string> fieldModelNames)
        {
            try
            {
                Dictionary<string, List<int>> fieldNames = new Dictionary<string, List<int>>();
                foreach (string fieldMN in fieldModelNames)
                {
                    fieldNames.Add(fieldMN, FieldIndicesFromModelName(objectClass, fieldMN));
                }
                return fieldNames;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// Determines whether the object class contains any of the field model names.
        /// </summary>
        /// <param name="classIn">The class in.</param>
        /// <param name="modelNames">The model names.</param>
        /// <returns>
        /// 	<c>true</c> if object class contains any of the field model name; otherwise, <c>false</c>.
        /// </returns>
        public static bool ContainsFieldModelName(IObjectClass classIn, params string[] modelNames)
        {
            if (modelNames == null) return false;

            foreach (string name in modelNames)
            {
                if (ModelNameManager.FieldFromModelName(classIn, name) != null)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Determine object class contains any of the class model name
        /// </summary>
        /// <param name="classIn">IObjectClass to check for model names</param>
        /// <param name="modelNames">The model names.</param>
        /// <returns><c>true</c> if object class contains any of the class model name; otherwise, <c>false</c>.</returns>
        public static bool ContainsClassModelName(IObjectClass classIn, params string[] modelNames)
        {
            if (modelNames == null) return false;

            foreach (string name in modelNames)
            {
                if (ModelNameManager.ContainsClassModelName(classIn, name))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Determine object class contains all of the class model name
        /// </summary>
        /// <param name="classIn">IObjectClass to check for model names</param>
        /// <param name="modelNames">The model names.</param>
        /// <returns>
        /// 	<c>true</c> if object class contains any of the class model name; otherwise, <c>false</c>.
        /// </returns>
        public static bool ContainsAllClassModelNames(IObjectClass classIn, params string[] modelNames)
        {
            if (modelNames == null) return false;
            int c = 0;
            foreach (string name in modelNames)
            {
                if (ModelNameManager.ContainsClassModelName(classIn, name))
                    c++;
            }

            return (modelNames.Length == c);
        }

        /// <summary>
        /// Determine object class contains all of the field model name
        /// </summary>
        /// <param name="classIn">IObjectClass to check for model names</param>
        /// <param name="modelNames">The model names.</param>
        /// <returns>
        /// 	<c>true</c> if object class contains any of the field model name; otherwise, <c>false</c>.
        /// </returns>
        public static bool ContainsAllFieldModelNames(IObjectClass classIn, params string[] modelNames)
        {
            if (modelNames == null) return false;
            int c = 0;
            foreach (string name in modelNames)
            {
                if (ModelNameManager.FieldFromModelName(classIn, name) != null)
                    c++;
            }

            return (modelNames.Length == c);
        }
    }
    #endregion

}
