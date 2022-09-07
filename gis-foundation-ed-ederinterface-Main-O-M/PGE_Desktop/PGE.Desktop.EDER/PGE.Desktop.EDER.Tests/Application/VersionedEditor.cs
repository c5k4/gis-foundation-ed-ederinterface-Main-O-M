using System;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geodatabase;
using PGE.Desktop.EDER.Tests.Common;

namespace PGE.Desktop.Tests.Application
{
    /// <summary>
    /// Class to create and edit in a version in an SDE
    /// </summary>
    public class VersionedEditor
    {
        private IFeatureWorkspace _featWksp;
        private IWorkspaceEdit _editSession;
        private IVersion _version;

        /// <summary>
        /// Opens a workspace, creates a version, and sets the class variables up to start editing in the version
        /// </summary>
        /// <param name="sdeFilepath">Filepath for the .sde file</param>
        /// <param name="versionName">Name of the version</param>
        public VersionedEditor(string sdeFilepath, string versionName, bool createNew)
        {
            _featWksp = (IFeatureWorkspace) Common.Common.OpenWorkspaceFromSDEFile(sdeFilepath);
            _version = createNew ? CreateOrReCreateVersion(versionName) : FindExistingVersion(versionName);
            _featWksp = (IFeatureWorkspace) _version;
            _editSession = (IWorkspaceEdit)_featWksp;
        }

        public void DeleteVersion(string versionName)
        {
            IVersionedWorkspace vw = (IVersionedWorkspace) _featWksp;

            vw.FindVersion(versionName).Delete();
        }

        public IVersion GetVersion()
        {
            return _version;
        }

        /// <summary>
        /// First tries to find a version with the specified name. If this fails, it tries to create a version
        /// </summary>
        /// <param name="vName">Name of the version to find/create</param>
        /// <returns>The created version or null if it could not create a version</returns>
        public IVersion CreateOrReCreateVersion(string vName)
        {
            IVersionedWorkspace vw = (IVersionedWorkspace)_featWksp;
            IVersion version = null;

            try
            {
                version = vw.FindVersion(vName);
                version.Delete();
                version = ((IVersionedWorkspace)_featWksp).DefaultVersion.CreateVersion(vName);
                version.Access = esriVersionAccess.esriVersionAccessPublic;
            }
            catch (Exception e)
            {
                LogManager.WriteLine("Could not find version in instance. Trying to create.");
                try
                {
                    version = ((IVersionedWorkspace)_featWksp).DefaultVersion.CreateVersion(vName);
                    version.Access = esriVersionAccess.esriVersionAccessPublic;
                }
                catch (Exception e2)
                {
                    LogManager.WriteLine("Could not find existing version with name: " + vName);
                    LogManager.WriteLine(e.ToString());
                    LogManager.WriteLine("Could not create version from default. Tried using " + vName + " as name.");
                    LogManager.WriteLine(e2.ToString());

                    return null;
                }
            }


            return version;
        }

        public IVersion FindExistingVersion(string vName)
        {
            return ((IVersionedWorkspace)_featWksp).FindVersion(vName);
        }

        public void StartEditSession()
        {
            _editSession.StartEditing(false);
            _editSession.StartEditOperation();
        }

        public void FinishEditSession(bool saveEdits)
        {
            _editSession.StopEditOperation();
            _editSession.StopEditing(saveEdits);
        }

        public IFeatureClass OpenFeatureClass(string featureClassName)
        {
            return _featWksp.OpenFeatureClass(featureClassName);
        }

        public ITable OpenTable(string tableName)
        {
            return _featWksp.OpenTable(tableName);
        }

        public IFeatureWorkspace GetFeatureWorkspace()
        {
            return _featWksp;
        }

        /// <summary>
        /// If data has changed between the old and new rows, the method updates the old row with data from the same field in the new row
        /// </summary>
        /// <param name="rowToUpdate">Old data that is used to check for differences</param>
        /// <param name="dataSource">The new data that will be written to the old row if a change is detected</param>
        /// <returns>Whether the operation was successful</returns>
        public static bool UpdateAllChangedFields(IRow rowToUpdate, IRow dataSource)
        {
            IField spField;
            object oldValue, newValue;
            int index;
            object value;

            try
            {
                //For each field in the old row, update the field with data from the corresponding field in the new row if the data has changed
                for (int i = 0; i < rowToUpdate.Fields.FieldCount; i++)
                {
                    spField = rowToUpdate.Fields.get_Field(i);
                    oldValue = rowToUpdate.get_Value(i);

                    try
                    {
                        newValue = dataSource.get_Value(dataSource.Fields.FindField(spField.Name));
                    }
                    catch (Exception e)
                    {
                        //The field does not exist in the new table. I.E. there is no data for the field from CEDSA
                        //We can safely ignore the lack of data and keep the existing GIS value for the field
                        continue;
                    }

                    if (!(oldValue.Equals(newValue) || newValue.Equals("")))
                    {
                        index = i;
                        value = dataSource.get_Value(dataSource.Fields.FindField(spField.Name));

                        rowToUpdate.set_Value(index, value);
                    }
                }
            }
            catch (Exception e)
            {
               LogManager.WriteLine(e.ToString());
                return false;
            }

            return true;
        }

        /// <summary>
        /// Writes all fields of a row to a new row
        /// </summary>
        /// <param name="oldRow">The source of the data</param>
        /// <param name="newRow">The row to which the data is written</param>
        /// <returns>Whether the operation was successful</returns>
        public static bool WriteAllFieldsToNewRow(IRow oldRow, IRowBuffer newRow)
        {
            try
            {
                //Loops through each field in the old row and references the new row's fields by name to find the corresponding field in the new row
                for (int i = 0; i < oldRow.Fields.FieldCount; i++)
                {
                    IField oldFieldAtIndex_i = oldRow.Fields.get_Field(i);
                    int index = newRow.Fields.FindField(oldFieldAtIndex_i.Name);
                    object value = oldRow.get_Value(oldRow.Fields.FindField(oldFieldAtIndex_i.Name));

                    if (index < 0)
                    {
                        throw new IndexOutOfRangeException("Row referenced in old table is not in new table");
                    }

                    newRow.set_Value(index, value);
                }
            }
            catch (Exception e)
            {
               LogManager.WriteLine(e.ToString());
                return false;
            }

            return true;
        }

        /// <summary>
        /// Gets the string value of a row by printing all fields of the row
        /// </summary>
        /// <param name="row">The row to toString</param>
        /// <returns>The string version of the row</returns>
        public static string RowToString(IRow row)
        {
            string print = Environment.NewLine;
            for (int i = 0; i < row.Fields.FieldCount; i++)
            {
                print += row.Fields.get_Field(i).Name + "," + row.get_Value(i) + Environment.NewLine;
            }

            return print;
        }
    }
}
