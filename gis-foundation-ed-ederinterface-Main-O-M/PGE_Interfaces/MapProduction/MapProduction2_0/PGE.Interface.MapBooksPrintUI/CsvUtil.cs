using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;

namespace PGE.Interfaces.MapBooksPrintUI
{
    public class CsvUtil
    {
        public List<CsvRow> ReadCsvRows(string filePath)
        {
            List<CsvRow> csvRows = new List<CsvRow>();

            using (CsvReader reader = new CsvReader(filePath))
            {
                CsvRow row = null;
                while (reader.ReadRow(out row))
                {
                    csvRows.Add(row);
                }
            }

            return csvRows;
        }

        public DataTable CsvToDataTable(string filePath)
        {
            DataTable dt = new DataTable();
            List<CsvRow> csvRows = ReadCsvRows(filePath);
            foreach (string columnName in csvRows[0])
            {
                dt.Columns.Add(columnName);
            }

            for (int i = 1; i <= csvRows.Count - 1; i++)
            {
                dt.Rows.Add(csvRows[i].ToArray<string>());
            }

            return dt;
        }
    }

    public class CsvRow : List<string>
    {
        public string RowText { get; set; }
    }

    public class CsvReader : StreamReader
    {
        public CsvReader(Stream stream)
            : base(stream)
        {}

        public CsvReader(string path)
            : base(path)
        {}

        /// <summary>
        /// Reads a row of data from a CSV file
        /// </summary>
        /// <param name="csvRow"></param>
        /// <returns></returns>
        public bool ReadRow(out CsvRow csvRow)
        {
            csvRow = new CsvRow();
            csvRow.RowText = ReadLine();
            if (String.IsNullOrEmpty(csvRow.RowText))
                return false;

            int pos = 0;
            int rows = 0;

            while (pos < csvRow.RowText.Length)
            {
                string value;

                // Special handling for quoted field
                if (csvRow.RowText[pos] == '"')
                {
                    // Skip initial quote
                    pos++;

                    // Parse quoted value
                    int start = pos;
                    while (pos < csvRow.RowText.Length)
                    {
                        // Test for quote character
                        if (csvRow.RowText[pos] == '"')
                        {
                            // Found one
                            pos++;

                            // If two quotes together, keep one. Otherwise, indicates end of value
                            if (pos >= csvRow.RowText.Length || csvRow.RowText[pos] != '"')
                            {
                                pos--;
                                break;
                            }
                        }
                        pos++;
                    }
                    value = csvRow.RowText.Substring(start, pos - start);
                    value = value.Replace("\"\"", "\"");
                }
                else
                {
                    // Parse unquoted value
                    int start = pos;
                    while (pos < csvRow.RowText.Length && csvRow.RowText[pos] != ',')
                        pos++;
                    value = csvRow.RowText.Substring(start, pos - start);
                }

                // Add field to list
                if (rows < csvRow.Count)
                    csvRow[rows] = value;
                else
                    csvRow.Add(value);
                rows++;

                // Eat up to and including next comma
                while (pos < csvRow.RowText.Length && csvRow.RowText[pos] != ',')
                    pos++;
                if (pos < csvRow.RowText.Length)
                    pos++;
            }
            // Delete any unused items
            while (csvRow.Count > rows)
                csvRow.RemoveAt(rows);

            // Return true if any columns read
            return (csvRow.Count > 0);
        }

        //StringBuilder sb = new StringBuilder();

        //string[] columnNames = dt.Columns.Cast<DataColumn>().
        //                                  Select(column => column.ColumnName).
        //                                  ToArray();
        //sb.AppendLine(string.Join(",", columnNames));

        //foreach (DataRow row in dt.Rows)
        //{
        //    string[] fields = row.ItemArray.Select(field => field.ToString()).
        //                                    ToArray();
        //    sb.AppendLine(string.Join(",", fields));
        //}

        //File.WriteAllText(@"c:\Subhankar\CircuitToMapNum.csv", sb.ToString());
    }
}
