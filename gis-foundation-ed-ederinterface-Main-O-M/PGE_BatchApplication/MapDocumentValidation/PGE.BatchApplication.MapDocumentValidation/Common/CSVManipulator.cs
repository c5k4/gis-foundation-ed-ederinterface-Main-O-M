using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Telvent.Delivery.Diagnostics;

namespace MapDocumentValidation.Common
{
    public class CsvManipulator
    {
        private static readonly Log4NetLogger Logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "MapDocValidation.log4net.config");

        //Dictionary to store all data to write to csv/read from csv

        //Dictionary mapping of header strings to locations in the _headers class member. Used for quick access to
        //the headers since this is accessed in every iteration of the main loop
        private readonly IDictionary<string, int> _headerIndices;

        //An array of strings representing all column titles, the string array value in the _items class member is ordered
        //in the same way as this array. However, this array has an extra entry in the 0th index of the array,
        //representing the key used to index the _items array (the key for the _items class member).
        private readonly string[] _headers;
        private readonly IDictionary<string, string[]> _items;

        /*
         *********************************************************************************************
         * The below methods are used to read from an existing Excel file
         *********************************************************************************************
         */

        /// <summary>
        ///     Loads a file from Excel into the IDictionary class attribute. This constructor is typically used to write to ArcFM
        ///     Field Properties
        /// </summary>
        /// <param name="fileName">Name of file to open</param>
        /// <param name="numColsToConcat">Number of columns that need to be concatenated to represent a unique ID across the data</param>
        public CsvManipulator(string fileName, int numColsToConcat)
        {
            _headerIndices = new Dictionary<string, int>();
            _items = new Dictionary<string, string[]>();

            // Read the file and display it line by line.
            StreamReader file = new StreamReader(@fileName);

            //Header line, combine numColsToConcat columns to create a unique ID for the data
            string line = file.ReadLine();
            int nthIndex = nthIndexOf(line, Constants.Delimiter, numColsToConcat);

            if (line != null)
            {
                _headers = new string[line.Substring(nthIndex + 1).Split(Constants.Delimiter).Length + 1];
                _headers[0] = line.Substring(0, nthIndex);
                _headerIndices.Add(line.Substring(0, nthIndex), 0);

                string[] tempVals = line.Substring(nthIndex + 1).Split(Constants.Delimiter);

                for (int i = 0; i < tempVals.Length; i++)
                {
                    _headers[i + 1] = tempVals[i];
                    _headerIndices.Add(_headers[i + 1], i + 1);
                }

                while ((line = file.ReadLine()) != null)
                {
                    nthIndex = nthIndexOf(line, Constants.Delimiter, 3);
                    string[] values = line.Substring(nthIndex + 1).Split(Constants.Delimiter);

                    _items.Add(line.Substring(0, nthIndex), values);
                }
            }

            file.Close();
        }

        /// <summary>
        ///     Initializes a blank IDictionary attribute to be populated and eventually written to file. Also tracks header
        ///     columns
        /// </summary>
        /// <param name="header">Array of header columns, each string is the name of the column</param>
        /// <param name="numColsToConcat">Number of columns that need to be concatenated to represent a unique ID across the data</param>
        public CsvManipulator(string[] header, int numColsToConcat)
        {
            _headerIndices = new Dictionary<string, int>();
            _items = new Dictionary<string, string[]>();
            _headers = new string[header.Length - (numColsToConcat - 1)];

            string line = "";
            for (int i = 0; i < numColsToConcat; i++)
            {
                 line += header[i] + Constants.Delimiter;
            }
            _headerIndices.Add(line.Substring(0, line.Length - 1), 0);
            _headers[0] = line.Substring(0, line.Length - 1);

            for (int i = numColsToConcat; i < header.Length; i++)
            {
                _headerIndices.Add(header[i], i - (numColsToConcat - 1));
                _headers[i - (numColsToConcat - 1)] = header[i];
            }
        }

        /// <summary>
        ///     Private helper method to retrieve the nth index of a particular character in a string
        /// </summary>
        /// <param name="source">The string to search</param>
        /// <param name="search">The character to search for</param>
        /// <param name="n">
        ///     Specifies which occurrence of the character to search for. Ex: n=2 means the second
        ///     occurrence of the 'search' char will be found and its location returned
        /// </param>
        /// <returns>The index of the 'n'th occurence of the 'search' char in the 'source' string</returns>
        private int nthIndexOf(string source, char search, int n)
        {
            int subStart = -1;

            for (int i = 0; i < n; i++)
            {
                subStart = source.IndexOf(search, subStart + 1);
                if (subStart == -1) return -1;
            }

            return subStart;
        }

        public bool HeaderExists(string property)
        {
            int idx;
            return _headerIndices.TryGetValue(property, out idx);
        }

        /// <summary>
        ///     Gets the value for a property in a specific row
        /// </summary>
        /// <param name="rowId">The unique identifier for the row being searched</param>
        /// <param name="property">
        ///     The property being searched. This string must be equal to one of the strings
        ///     passed into the header array of the constructor
        /// </param>
        /// <returns>The string value of the queried property</returns>
        public string GetPropertyForRow(string rowId, string property)
        {
            try
            {
                string[] vals = _items[rowId];
                return vals[_headerIndices[property] - 1];
            }
            catch (Exception e)
            {
                Logger.Warn("Could not find row/property with rowID \"" + rowId + "\", property  \"" + property + "\"");
                Logger.Warn(e.Message);
                Logger.Warn(e.StackTrace);
            }

            return null;
        }

        /*
         *******************************************************************************************
         * The below methods are used to write a new Excel file
         *******************************************************************************************
         */

        //

        public int GetHeaderLength()
        {
            return _headers.Length;
        }

        /// <summary>
        ///     Adds a row to the CSV
        /// </summary>
        /// <param name="rowId">The unique identifier for the row</param>
        /// <param name="rowProperties">
        ///     All columns associtaed with that row. The order of these properties
        ///     must align with the header array passed into the constructor to correctly export this CSV
        /// </param>
        public void AddRow(string rowId, string[] rowProperties)
        {
            _items.Add(rowId, rowProperties);
        }

        /// <summary>
        ///     Whether the row has been created yet
        /// </summary>
        /// <param name="rowId">The unique identifier for the row we are checking</param>
        /// <returns>True if the row exists, false if it does not</returns>
        public bool RowExists(string rowId)
        {
            string[] vals;
            return _items.TryGetValue(rowId, out vals);
        }

        /// <summary>
        ///     Sets a single property for a row.
        /// </summary>
        /// <param name="rowId">The unique ID of the row to be changed</param>
        /// <param name="property">
        ///     The property to change. This must be the same string as one of the strings
        ///     in the header array passed into the constructor
        /// </param>
        /// <param name="value">The value to assign to the property</param>
        public void SetPropertyForRow(string rowId, string property, string value)
        {
            string[] vals;

            if (_items.TryGetValue(rowId, out vals))
            {
                vals[_headerIndices[property] - 1] = value;
            }
            else
            {
                Logger.Warn("Could not find row with rowID: \"" + rowId + "\". Did not modify row with value: \"" +
                            value + "\".");
            }
        }

        /// <summary>
        ///     Call this method when all data is ready to be written to file. This method will write the data to disk
        /// </summary>
        /// <param name="fileName">The file path and name to write the csv to. Ex: "C:\temp\test.csv"</param>
        public void WriteToFile(string @fileName)
        {
            StringBuilder line = new StringBuilder();

            StreamWriter file = new StreamWriter(@fileName);

            foreach (string t in _headers)
            {
                line.Append(t + Constants.Delimiter);
            }

            //Remove trailing comma
            file.WriteLine(line.ToString().Substring(0, line.Length - 1));

            foreach (string rowId in _items.Keys)
            {
                line = new StringBuilder();
                string[] rowProperties = _items[rowId];
                line.Append(rowId + Constants.Delimiter);

                foreach (string t in rowProperties)
                {
                    line.Append("\"" + t + "\"" + Constants.Delimiter);
                }

                //Remove trailing comma
                file.WriteLine(line.ToString().Substring(0, line.Length - 1));
            }

            file.Close();
        }

        public void SwapColumn(int oldIdx, int newIdx)
        {
            if (oldIdx >= _items.Values.First().Length || newIdx >= _items.Values.First().Length)
                throw new ArgumentOutOfRangeException("Must be within the total number of columns in the csv.");

            string temp;

            temp = _headers[newIdx + 1];
            _headers[newIdx + 1] = _headers[oldIdx + 1];
            _headers[oldIdx + 1] = temp;

            foreach (string key in _items.Keys.ToArray())
            {
                string[] row = _items[key];

                temp = row[newIdx];
                row[newIdx] = row[oldIdx];
                row[oldIdx] = temp;

                _items[key] = row;
            }
        }

        public void PushAwayEmptyColumns()
        {
            bool[] isEmpty = GetEmptyStatus();

            if (isEmpty.Length == 1 && isEmpty[0]) return;

            for (int i = 0; i < _items.Values.First().Length; i++)
            {
                if (!isEmpty[i]) continue;

                bool emptyFound = false;
                for (int j = i + 1; !emptyFound && j < _items.Values.First().Length; j++)
                {
                    if (isEmpty[j]) continue;
                    bool emptyTemp;

                    emptyFound = true;
                    SwapColumn(i, j);

                    emptyTemp = isEmpty[j];
                    isEmpty[j] = isEmpty[i];
                    isEmpty[i] = emptyTemp;
                }
            }
        }

        private bool[] GetEmptyStatus()
        {
            if (_items.Values.Count == 0) return new[] {true};

            bool[] isEmpty = new bool[_items.Values.First().Length];
            for (int i = 0; i < _items.Values.First().Length; i++)
                isEmpty[i] = _items.Values.All(row => String.IsNullOrEmpty(row[i]));

            return isEmpty;
        }
    }

    public static class Constants
    {
        public const char Delimiter = ',';
    }
}