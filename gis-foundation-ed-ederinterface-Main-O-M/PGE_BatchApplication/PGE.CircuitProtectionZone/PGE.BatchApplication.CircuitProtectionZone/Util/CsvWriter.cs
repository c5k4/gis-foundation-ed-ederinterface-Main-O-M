using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PGE.BatchApplication.CircuitProtectionZone.Util
{
    public class CsvWriter
    {
        /// <summary>
        /// Class to write data to a CSV file
        /// </summary>
        public class CsvFileWriter : StreamWriter
        {
            public CsvFileWriter(Stream stream)
                : base(stream)
            {
            }

            public CsvFileWriter(string filename)
                : base(filename)
            {
            }

            public CsvFileWriter(string fileName, bool append) : base(fileName, append)
            {
            }

            /// <summary>
            ///     Writes a single row to a CSV file.
            /// </summary>
            /// <param name="row">The row to be written</param>
            public void WriteRow(CsvRow row)
            {
                var builder = new StringBuilder();
                bool firstColumn = true;
                foreach (string value in row)
                {
                    // Add separator if this isn't the first value
                    if (!firstColumn)
                        builder.Append(',');
                    // Implement special handling for values that contain comma or quote
                    // Enclose in quotes and double up any double quotes
                    if (value.IndexOfAny(new[] { '"', ',' }) != -1)
                        builder.AppendFormat("\"{0}\"", value.Replace("\"", "\"\""));
                    else
                        builder.Append(value);
                    firstColumn = false;
                }
                row.LineText = builder.ToString();
                
                WriteLine(row.LineText);
            }
        }

        /// <summary>
        ///     Class to store one CSV row
        /// </summary>
        public class CsvRow : List<string>
        {
            public string LineText { get; set; }
        }
    }
}
