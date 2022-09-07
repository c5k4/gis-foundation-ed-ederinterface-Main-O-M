using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesGDB;
using System.Runtime.InteropServices;

namespace PGE.BatchApplication.RemoveDefaultValue
{
    public class FieldDefaultHelper : IDisposable
    {
        IWorkspace _ws = null;
        ITable _table = null;
        string _tableName = null;
        string _fieldName = null;

        public FieldDefaultHelper(string databaseLocation, string tableName, string fieldName)
        {
            _tableName = tableName;
            try
            {
                FileInfo fi = new FileInfo(databaseLocation);
                if (fi.Exists && fi.Extension.ToUpper() == ".SDE")
                {
                    SdeWorkspaceFactory wsFactory = new SdeWorkspaceFactory();
                    _ws = wsFactory.OpenFromFile(databaseLocation, 0);
                }
                else
                {
                    AccessWorkspaceFactory accessWSFactory = new AccessWorkspaceFactoryClass();
                    _ws = accessWSFactory.OpenFromFile(databaseLocation, 0);
                }

                _table = (_ws as IFeatureWorkspace).OpenTable(tableName);
                _fieldName = fieldName;
            }
            catch
            {
                Console.WriteLine("Could not connect to database and table.");
            }
        }

        public void RemoveDefault(int? applyToSubtype)
        {
            ISchemaLock schemaLock = null;
            try
            {
                if (_table != null)
                {
                    Console.WriteLine("\nProcessing Table: \"" + _tableName + "\"");

                    // Set an exclusive schemalock on the table before modifying field properties
                    schemaLock = _table as ISchemaLock;
                    try
                    {
                        schemaLock.ChangeSchemaLock(esriSchemaLock.esriExclusiveSchemaLock);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("\n     ERROR: An exclusive lock on the table \"" + _tableName + "\" could not be obtained\n");
                        return;
                    }

                    // Retrieve field.
                    int fieldIndex = _table.Fields.FindField(_fieldName);
                    if (fieldIndex == -1)
                    {
                        Console.WriteLine("     ERROR: Unable to find field \"" + _fieldName + "\"");
                        return;
                    }

                    IField field = _table.Fields.get_Field(fieldIndex);

                    if (applyToSubtype.HasValue)
                    {
                        ISubtypes subtypes = _table as ISubtypes;
                        if (subtypes.HasSubtype)
                        {
                            string subtypeName = null;
                            try
                            {
                                subtypeName = subtypes.get_SubtypeName(applyToSubtype.Value);
                            }
                            catch
                            {
                                Console.WriteLine("     ERROR: The subtype specified (" + applyToSubtype.Value.ToString() + ") does not exist.");
                                return;
                            }

                            Console.WriteLine("     Removing default value for field " + _fieldName + ", subtype " + applyToSubtype.Value.ToString() + " : " + subtypeName);
                            subtypes.set_DefaultValue(applyToSubtype.Value, _fieldName, null);
                        }
                        else
                        {
                            Console.WriteLine("     ERROR: No subtypes exist for this feature.");
                            return;
                        }
                    }
                    else
                    {
                        IClassSchemaEdit schemaEdit = _table as IClassSchemaEdit;

                        Console.WriteLine("     Removing default value for field " + _fieldName);
                        schemaEdit.AlterDefaultValue(_fieldName, null);
                    }
                    Console.WriteLine("     Successfully removed default value.");

                }
            }
            catch (Exception e)
            {
                Console.WriteLine("     ERROR: " + e.Message);
                Console.WriteLine(e.ToString());
            }
            finally
            {
                if (schemaLock != null)
                {
                    schemaLock.ChangeSchemaLock(esriSchemaLock.esriSharedSchemaLock);
                }
            }
        }

        public void Dispose()
        {
            if (_table != null) { while (Marshal.ReleaseComObject(_table) > 0) { } }
        }
    }
}
