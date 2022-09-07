using System;
using System.ComponentModel;
using System.Data;
using System.IO;

using PGE.Common.Delivery.Systems.Data.OleDb;

namespace PGE.Common.Delivery.Systems.IO
{
    /// <summary>
    /// A static class that reads and writes comma separated value files using a <see cref="DataTable"/> as the primary facilitator of the data.
    /// </summary>
    public static class CsvContext
    {
        #region Public Methods

        /// <summary>
        /// Reads the contents of the specified file into a <see cref="DataTable"/>.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>
        /// A <see cref="DataTable"/> with the contents of the file.
        /// </returns>
        public static DataTable Read(string fileName)
        {
            string connectionString = string.Format(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=""Text"";", Path.GetDirectoryName(fileName));

            // Create a connection to the folder.
            using (OleDbDatabaseConnection cn = new OleDbDatabaseConnection(connectionString))
            {
                // Select the file name from the connection.
                return cn.Fill(@"SELECT * FROM " + Path.GetFileName(fileName), Path.GetFileNameWithoutExtension(fileName));
            }
        }

        /// <summary>
        /// Writes the specified data to the file name using the data from the <see cref="DataTable"/>.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="table">The table.</param>
        public static void Write(string fileName, DataTable table)
        {
            Write(fileName, table, null);
        }

        /// <summary>
        /// Writes the specified data to the file name using the data from the <see cref="DataTable"/>.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="table">The table.</param>
        /// <param name="eventHandler">The event handler.</param>
        public static void Write(string fileName, DataTable table, ProgressChangedEventHandler eventHandler)
        {
            // Create a new file using the stream writer.
            using (StreamWriter sw = new StreamWriter(fileName, false))
            {
                string delimiter = string.Empty;

                // Add the column headers from the table.
                foreach (DataColumn column in table.Columns)
                {
                    sw.Write(delimiter);
                    sw.Write("\"");
                    sw.Write(column.ColumnName);
                    sw.Write("\"");
                    delimiter = ",";
                }

                // Add a new line.
                sw.Write(Environment.NewLine);

                int value = 0;
                int maximum = table.Rows.Count;

                // Iterate through all of the rows.
                foreach (DataRow row in table.Rows)
                {
                    delimiter = string.Empty;
                    value++;

                    // Add the data from the rows for each column.
                    foreach (object o in row.ItemArray)
                    {
                        sw.Write(delimiter);
                        sw.Write("\"");
                        sw.Write(o);
                        sw.Write("\"");

                        delimiter = ",";
                    }

                    // Add a new line.
                    sw.Write(Environment.NewLine);

                    // Raise the event if it was provided.
                    if (eventHandler != null)
                    {
                        int percentComplete = (int)((float)value / (float)maximum * 100);
                        eventHandler(table, new ProgressChangedEventArgs(percentComplete, row));
                    }
                }
            }
        }

        #endregion
    }
}