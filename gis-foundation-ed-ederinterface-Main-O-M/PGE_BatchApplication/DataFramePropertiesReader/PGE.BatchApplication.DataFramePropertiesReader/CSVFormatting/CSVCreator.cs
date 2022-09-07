using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PGE.BatchApplication.DataFramePropertiesReader
{
    /// <summary>
    /// This class creates a .csv file representing a 2D matrix of values. Populate the data variable before running WriteToCSV. The output file, when opened in an application that supports csv formatting, will look like the following:
    ///     a   b   c
    /// x   -   -   -
    /// y   -   -   -
    /// z   -   -   -
    /// where a,b,c are colHeaders; x,y,z are rowHeaders; and the data is populated from the data variable.
    /// 
    /// This class works in conjunction with the CSVDatum basic wrapper class
    /// </summary>
    /// <param name="colHeaders">Column titles</param>
    /// <param name="rowHeaders">Row titles</param>
    /// <param name="data">Each piece of data you want to populate into the table</param>
    /// <returns></returns>
    class CSVCreator
    {
        private List<string> colHeaders;
        private List<string> rowHeaders;

        private List<CSVDatum> data;
        private string formattedAsCSV;

        public CSVCreator() 
        {
            colHeaders = new List<string>();
            rowHeaders = new List<string>();

            data = new List<CSVDatum>();
            formattedAsCSV = "";
        }

        /// <summary>
        /// Private method to manage the creation of a spreadsheet (csv file). Creates a list of all unique row values and another list of all unique column values
        /// </summary>
        /// <returns></returns>
        private void CreateRowsAndCols()
        {
            HashSet<string> distinctRows = new HashSet<string>(), distinctCols = new HashSet<string>();
            foreach (CSVDatum d in data)
            {
                distinctRows.Add(d.getRow());
                distinctCols.Add(d.getCol());
            }

            foreach (string row in distinctRows)
            {
                rowHeaders.Add(row);
            }

            foreach (string col in distinctCols)
            {
                colHeaders.Add(col);
            }
        }

        public void AddDatum(CSVDatum d)
        {
            data.Add(d);
        }

        /// <summary>
        /// After all data input into the class has been received, this method formats the data as a CSV table
        /// </summary>
        /// <returns>A comma-delimited string containing all data. This can be written to a csv file</returns>
        public string FormatDataToCSV()
        {
            formattedAsCSV = ",";
            CreateRowsAndCols();
            string[,] cells = new string[rowHeaders.Count, colHeaders.Count];

            //This is done to ensure we have the same order for the column/row headers as we do for the data inside the table. Important for the following foreach loop
            colHeaders.Sort();
            rowHeaders.Sort();

            //Creates a 2D array iterates through the data variable, populating the location the data refers to
            foreach (CSVDatum d in data)
            {
                cells[rowHeaders.IndexOf(d.getRow()), 
                    colHeaders.IndexOf(d.getCol())] 
                    = d.getData();
            }

            //Begin formatting the data into csv format

            foreach (string col in colHeaders)
            {
                formattedAsCSV += col + ",";
            }

            formattedAsCSV += "\n";

            for (int i = 0; i < cells.GetLength(0); i++)
            {
                formattedAsCSV += rowHeaders[i] + ",";
                for (int j = 0; j < cells.GetLength(1); j++)
                {
                    formattedAsCSV += cells[i, j] + ",";
                }

                formattedAsCSV += "\n";
            }

            return formattedAsCSV;
        }

        public void WriteToCSV(string path)
        {
            System.IO.File.WriteAllText(path, formattedAsCSV);
        }
    }
}
