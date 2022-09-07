using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.GeoDatabaseDistributed;
using System.Reflection;
using PGE.Common.Delivery.Diagnostics;
using PGE.Common.ChangeDetectionAPI;
using System.Globalization;

namespace PGE.Common.ChangesManagerShared.Utilities
{
    public class AOHelper
    {
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "");

        /// <summary>
        /// Opens an SDE connection file from a file name and loads it into an IWorkspace.
        /// </summary>
        /// <param name="sFile">The file path of the desired SDE connection file.</param>
        /// <returns>An IWorkspace object from the connection.</returns>
        public static IWorkspace ArcSDEWorkspaceFromPath(string sFile)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name + " [ " + sFile + " ]");

            IWorkspace workspace = null;

            try
            {
                Type t = Type.GetTypeFromProgID("esriDataSourcesGDB.SdeWorkspaceFactory");
                IWorkspaceFactory workspaceFactory = (IWorkspaceFactory)Activator.CreateInstance(t);
                workspace = workspaceFactory.OpenFromFile(sFile, 0);
                LogSdeConnectionProperties(workspace);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
                throw new ErrorCodeException(ErrorCode.Workspace, "Workspace Can't be Opened at [ " + sFile + " ]", ex);
            }

            return workspace;

        }

        public static void LogSdeConnectionProperties(IWorkspace sdeWorkspace)
        {
            object names = null, values = null;
            sdeWorkspace.ConnectionProperties.GetAllProperties(out names, out values);
            IDictionary<string, object> properties = new Dictionary<string, object>();
            string[] nameArray = (string[])names;
            object[] valueArray = (object[])values;
            for (int i = 0; i < nameArray.Length; i++)
            {
                properties.Add(nameArray[i], valueArray[i]);
            }
            string user = properties["USER"].ToString();// ((IDatabaseConnectionInfo)sdeWorkspace).ConnectedUser;
            string instance = properties["INSTANCE"].ToString();
            
            string version = (properties.ContainsKey("VERSION")) ? properties["VERSION"].ToString() : "NO VERSION";
            _logger.Debug(String.Format("ArcSDE [ {0} {1} {2} ]", instance, user, version));
            
        }

        public static IWorkspace OleDbWorkspaceFromPath(string sFile)
        {
            IWorkspace workspace = null;

            IWorkspaceFactory workspaceFactory = new ESRI.ArcGIS.DataSourcesOleDB.OLEDBWorkspaceFactoryClass();
            workspace = workspaceFactory.OpenFromFile(sFile, 0);

            return workspace;

        }

        public static void EnsureInEditSession(IWorkspace workspace)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);
            if (workspace == null) return;

            if (!((IWorkspaceEdit)workspace).IsBeingEdited())
            {
                ((IWorkspaceEdit)workspace).StartEditing(false);
            }
        }

        public static void EnsureCloseEditSession(IWorkspace workspace, bool saveEdits = true)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);
            if (workspace == null) return;

            if (((IWorkspaceEdit)workspace).IsBeingEdited())
            {
                ((IWorkspaceEdit)workspace).StopEditing(saveEdits);
            }
        }


        static public IRow CreateRelatedRow(IRow parentRow, string relatedTable, bool storeNewRow = false)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            IWorkspace workspace = ((IDataset)parentRow.Table).Workspace;
            IRow newRow = null;
            bool parentRowIsSourceRelClass = true;
            IRelationshipClass relationshipClass = GetRelationshipClass(parentRow, relatedTable);
            if (relationshipClass == null)
            {
                throw new ApplicationException("No such relationshipClass for [ " + relatedTable + " ]");
            }

            if (((IDataset)relationshipClass.OriginClass).Name.ToUpper() == relatedTable)
            {
                parentRowIsSourceRelClass = false;
            }

            IEnumRelationship relationshipEnum = relationshipClass.GetRelationshipsForObject((IObject)parentRow);
            IRelationship relationship = relationshipEnum.Next();

            if (relationship == null)
            {
                IObjectClass objectClass = relationshipClass.DestinationClass;
                newRow = ((ITable)objectClass).CreateRow();

                ISubtypes subtypes = (ISubtypes)objectClass;
                IRowSubtypes rowSubtypes = (IRowSubtypes)newRow;
                if (subtypes.HasSubtype)// does the feature class have subtypes?
                {
                    rowSubtypes.SubtypeCode = subtypes.DefaultSubtypeCode;
                }

                // initalize any default values that the feature has
                rowSubtypes.InitDefaultValues();

                if (storeNewRow)
                {
                    newRow.Store();
                }
                if (parentRowIsSourceRelClass)
                {
                    relationshipClass.CreateRelationship(parentRow as IObject, newRow as IObject);
                }
                else
                {
                    relationshipClass.CreateRelationship(newRow as IObject, parentRow as IObject);
                }
            }

            return newRow;
        }


        public static string GlobalIDFieldName(ITable table)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            for (int i = 0; i < table.Fields.FieldCount; i++)
            {
                IField field = table.Fields.get_Field(i);
                if (field.Type == esriFieldType.esriFieldTypeGlobalID)
                {
                    return field.Name;
                }
            }
            return "";
        }

        public static int GlobalIDFieldPosition(ITable table)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            int globalIdFieldPosition = -1;

            for (int i = 0; i < table.Fields.FieldCount; i++)
            {
                IField field = table.Fields.get_Field(i);
                if (field.Type == esriFieldType.esriFieldTypeGlobalID)
                {
                    globalIdFieldPosition = i;
                    break;
                }
            }
            return globalIdFieldPosition;
        }


        static public IRow GetRelatedRow(IRow row, string relatedClass)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            IEnumRelationshipClass enumRelationshipClass = null;
            IRow relatedRow = null;

            try
            {
                enumRelationshipClass = ((row.Table as IObjectClass)).get_RelationshipClasses(esriRelRole.esriRelRoleAny);
                IRelationshipClass relationshipClass;
                bool containsRelatedClass = false;
                while ((relationshipClass = enumRelationshipClass.Next()) != null)
                {
                    containsRelatedClass =
                        relatedClass.Equals(((IDataset)relationshipClass.DestinationClass).Name.ToUpper());
                    if (!containsRelatedClass)
                    {
                        containsRelatedClass =
                            relatedClass.Equals(((IDataset)relationshipClass.OriginClass).Name.ToUpper());
                    }
                    if (containsRelatedClass)
                    {
                        _logger.Debug("Found RelationshipClass Orig [ " + relationshipClass.OriginClass.AliasName +
                                      " ] Dest [ " + relationshipClass.DestinationClass.AliasName + " ]");
                        ISet objectSet = relationshipClass.GetObjectsRelatedToObject(row as IObject);
                        if (objectSet != null)
                        {
                            relatedRow = objectSet.Next() as IRow;
                            if (relatedRow != null)
                            {
                                _logger.Debug("Found Related Object [ " + ((IDataset)relatedRow.Table).Name +
                                              " ] OID [ " + relatedRow.OID + " ]");
                                break;
                            }
                        }

                        _logger.Debug("No Related Objects for " + row.GetRowDescription());
                    }

                }

            }
            catch (Exception exception)
            {
                _logger.Error(exception.ToString(), exception);
                throw;
            }
            finally
            {
                if (enumRelationshipClass != null) Marshal.FinalReleaseComObject(enumRelationshipClass);
            }

            return relatedRow;
        }

        static public IRelationshipClass GetRelationshipClass(IRow row, string relatedClass)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            IEnumRelationshipClass enumRelationshipClass = null;
            IRelationshipClass relationshipClass = null;

            try
            {
                enumRelationshipClass = ((row.Table as IObjectClass)).get_RelationshipClasses(esriRelRole.esriRelRoleAny);
                bool containsRelatedClass = false;
                while ((relationshipClass = enumRelationshipClass.Next()) != null)
                {
                    containsRelatedClass =
                        relatedClass.Equals(((IDataset)relationshipClass.DestinationClass).Name.ToUpper());
                    if (!containsRelatedClass)
                    {
                        containsRelatedClass =
                            relatedClass.Equals(((IDataset)relationshipClass.OriginClass).Name.ToUpper());
                    }
                    if (containsRelatedClass)
                    {
                        _logger.Debug("Found RelationshipClass Orig [ " + relationshipClass.OriginClass.AliasName +
                                      " ] Dest [ " + relationshipClass.DestinationClass.AliasName + " ]");

                        return relationshipClass;
                    }

                }

                return null;

            }
            catch (Exception exception)
            {
                _logger.Error(exception.ToString(), exception);
                throw;
            }
            finally
            {
                if (enumRelationshipClass != null) Marshal.FinalReleaseComObject(enumRelationshipClass);
            }

        }


        static public bool AttributeValueExistsInTable(string whereClause, ITable table)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name + " whereClause [ " + whereClause + " ]");

            bool exists = false;

            IQueryFilter qf = new QueryFilterClass();

            try
            {
                //Check to make sure the grid doesn't already exist.
                qf.WhereClause = whereClause;
                if (table.RowCount(qf) > 0)
                {
                    _logger.Debug("  The attributeValue '" + whereClause + "' already exists in the output table and will be skipped.");
                    exists = true;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Issue checking for existing grids using grid '" + whereClause + "': " + ex.Message, ex);
            }
            finally
            {
                if (qf != null) Marshal.FinalReleaseComObject(qf);
            }
            return exists;
        }

        public static IWorkspace OpenWorkspaceInDifferentVersion(IWorkspace sdeWorkspace, string version)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name + " [ " + version + " ]");

            object names = null, values = null;
            sdeWorkspace.ConnectionProperties.GetAllProperties(out names, out values);
            IDictionary<string, object> properties = new Dictionary<string, object>();
            string[] nameArray = (string[])names;
            object[] valueArray = (object[])values;
            for (int i = 0; i < nameArray.Length; i++)
            {
                properties.Add(nameArray[i], valueArray[i]);
            }
            string user = ((IDatabaseConnectionInfo)sdeWorkspace).ConnectedUser;
            object password = properties["PASSWORD"];
            string instance = properties["INSTANCE"].ToString();
            IPropertySet propertySet = new PropertySetClass();
            propertySet.SetProperty("INSTANCE", instance);
            propertySet.SetProperty("USER", user);
            propertySet.SetProperty("PASSWORD", password);
            propertySet.SetProperty("VERSION", version);

            Type factoryType = Type.GetTypeFromProgID(
                "esriDataSourcesGDB.SdeWorkspaceFactory");
            IWorkspaceFactory workspaceFactory = (IWorkspaceFactory)Activator.CreateInstance
                (factoryType);

            return workspaceFactory.Open(propertySet, 0);
        }

        static public XElement GetRowChangeAsXElement(IRow row)
        {
            XElement rowElement = new XElement("ROW");

            for (int i = 0; i < row.Fields.FieldCount; i++)
            {
                IField field = row.Fields.get_Field(i);
                object valueObject = row.get_Value(i);
                string valueString = "NOSHOW";
                switch (field.Type)
                {
                    case esriFieldType.esriFieldTypeDate:
                        valueString = valueObject != null ? Convert.ToString(valueObject) : "";
                        break;
                    case esriFieldType.esriFieldTypeDouble:
                        valueString = valueObject != null ? Convert.ToString(valueObject) : "";
                        break;
                    case esriFieldType.esriFieldTypeGUID:
                    case esriFieldType.esriFieldTypeGlobalID:
                        valueString = valueObject != null ? Convert.ToString(valueObject) : "";
                        break;
                    case esriFieldType.esriFieldTypeOID:
                    case esriFieldType.esriFieldTypeInteger:
                        valueString = valueObject != null ? Convert.ToString(valueObject) : "";
                        break;
                    case esriFieldType.esriFieldTypeSingle:
                        valueString = valueObject != null ? Convert.ToString(valueObject) : "";
                        break;
                    case esriFieldType.esriFieldTypeSmallInteger:
                        valueString = valueObject != null ? Convert.ToString(valueObject) : "";
                        break;
                    case esriFieldType.esriFieldTypeString:
                        valueString = valueObject != null ? Convert.ToString(valueObject) : "";
                        break;
                    default:
                        break;
                }

                if (valueString != "NOSHOW")
                {
                    XElement fieldElement = new XElement(field.Name, valueString);
                    rowElement.Add(fieldElement);
                }
            }
            return rowElement;
        }

        static public XElement CDGetRowChangeAsXElement(DeleteFeat row,ITable table)
        {
            XElement rowElement = new XElement("ROW");
            IField field = default;
            string valueObject = string.Empty;
            string valueString = "NOSHOW";

            for (int i = 0; i < table.Fields.FieldCount; i++)
            {
                field = table.Fields.get_Field(i);
                valueObject = string.Empty;
                valueString = "NOSHOW";
                
                if (row.fields_Old.ContainsKey(field.Name.ToUpper()))
                {
                    valueObject = row.fields_Old[field.Name.ToUpper()];
                    //if (!string.IsNullOrWhiteSpace(valueObject))
                    {
                        switch (field.Type)
                        {
                            case esriFieldType.esriFieldTypeDate:
                                valueString = valueObject != null?((DateTime)DateTime.Parse(Convert.ToString(valueObject), new CultureInfo("en-US", true))).ToString("MM/dd/yyyy hh:mm:ss tt") : "";
                                break;
                            case esriFieldType.esriFieldTypeDouble:
                                valueString = valueObject != null ? Convert.ToString(valueObject) : "";
                                break;
                            case esriFieldType.esriFieldTypeGUID:
                            case esriFieldType.esriFieldTypeGlobalID:
                                valueString = valueObject != null ? Convert.ToString(valueObject) : "";
                                break;
                            case esriFieldType.esriFieldTypeOID:
                            case esriFieldType.esriFieldTypeInteger:
                                valueString = valueObject != null ? Convert.ToString(valueObject) : "";
                                break;
                            case esriFieldType.esriFieldTypeSingle:
                                valueString = valueObject != null ? Convert.ToString(valueObject) : "";
                                break;
                            case esriFieldType.esriFieldTypeSmallInteger:
                                valueString = valueObject != null ? Convert.ToString(valueObject) : "";
                                break;
                            case esriFieldType.esriFieldTypeString:
                                valueString = valueObject != null ? Convert.ToString(valueObject) : "";
                                break;
                            default:
                                break;
                        }
                    }
                }
                else
                {
                    valueString = "";
                }

                if (valueString != "NOSHOW")
                {
                    XElement fieldElement = new XElement(field.Name, valueString);
                    rowElement.Add(fieldElement);
                }
            }

            //foreach (string field in row.fields_Old.Keys)
            //{
            //    string valueObject = row.fields_Old[field];
            //    string valueString = "NOSHOW";

            //    if (!string.IsNullOrWhiteSpace(valueObject))
            //        valueString = valueObject;
                
            //    if (valueString != "NOSHOW")
            //    {
            //        XElement fieldElement = new XElement(field, valueString);
            //        rowElement.Add(fieldElement);
            //    }
            //}

            return rowElement;
        }

        /// <summary>
        /// Intended to be run after all processing is complete. Rolls the versions forward by deleting the child version
        /// and renaming the temporary version to that of the child version.
        /// </summary>
        static public void RollVersions(IWorkspace workspaceChangeDetection, string versionDailyChangeName, string versionTempName)
        {
            try
            {
                IVersionedWorkspace vWorkspace = (IVersionedWorkspace)workspaceChangeDetection;
                IVersion2 versDailyChange = (IVersion2)vWorkspace.FindVersion(versionDailyChangeName);
                _logger.Debug("Rolling Versions:");

                //Delete daily change version.
                string versionName = versDailyChange.VersionName;
                if (versionName.LastIndexOf('.') >= 0) versionName = versionName.Substring(versionName.LastIndexOf('.') + 1);
                versDailyChange.Delete();
                versDailyChange = null;
                _logger.Debug("  Deleted version (" + versionName + ").");

                //Rename the temporary version.
                IVersion2 versDailyChangeTemp = (IVersion2)vWorkspace.FindVersion(versionTempName);
                versDailyChangeTemp.VersionName = versionName;
                _logger.Debug("  Renamed version " + versionTempName + " to " + versionName + ".");
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Issue rolling versions forward: " + ex.Message, ex);
//                throw new ErrorCodeException(ErrorCode.VersionReset, "Issue rolling versions forward: " + ex.Message, ex);
            }
        }

        public static IWorkspace FileGdbWorkspaceFromPath(String path)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name + " for path [ " + path + " ]");

            Type factoryType = Type.GetTypeFromProgID(
                "esriDataSourcesGDB.FileGDBWorkspaceFactory");
            IWorkspaceFactory workspaceFactory = (IWorkspaceFactory)Activator.CreateInstance
                (factoryType);
            return workspaceFactory.OpenFromFile(path, 0);
        }
        public static IWorkspace ExtractGdbWorkspaceFromPath(String path)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name + " for path [ " + path + " ]");

            if (path.ToLower().Contains(".gdb"))
            {

                Type factoryType = Type.GetTypeFromProgID(
                    "esriDataSourcesGDB.FileGDBWorkspaceFactory");
                IWorkspaceFactory workspaceFactory = (IWorkspaceFactory)Activator.CreateInstance
                    (factoryType);
                return workspaceFactory.OpenFromFile(path, 0);
            }
            else
            {
                Type factoryType = Type.GetTypeFromProgID(
                    "esriDataSourcesGDB.AccessWorkspaceFactory");
                IWorkspaceFactory workspaceFactory = (IWorkspaceFactory)Activator.CreateInstance
                    (factoryType);
                return workspaceFactory.OpenFromFile(path, 0);
            }
        }

        /// <summary>
        /// Determines whether or not the version is a child of the specified potential parent version.
        /// </summary>
        /// <param name="version">The version to compare with a potential parent version.</param>
        /// <param name="potentialParent">The potential parent version.</param>
        /// <returns><c>true</c> if the version is a child of the parent, otherwise <c>false</c>.</returns>
        public static bool VersionIsChildOf(IVersion version, IVersion potentialParent)
        {
            if (!version.HasParent())
                return false;

            return version.VersionInfo.Parent.VersionName == potentialParent.VersionName;
        }

    }

}
