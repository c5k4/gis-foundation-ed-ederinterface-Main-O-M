using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PGE.BatchApplication.DataFramePropertiesReader
{
    /// <summary>
    /// Stores a basic piece of data for inclusion into a 2D matrix of a spreadsheet. Works in conjunction with the CSVCreator class
    /// </summary>
    /// <param name="row">Specifies one of the dimensions of the 2D matrix</param>
    /// <param name="workspace">Specifies the other of the two dimensions of the 2D matrix</param>
    /// <param name="data">Specifies the value of the cell at the intersection of both dimensions of the 2D matrix</param>
    /// <returns></returns>
    class CSVDatum
    {
        private string row;
        private string col;
        private string data;

        public CSVDatum(string row, string col, string data)
        {
            this.row = row;
            this.col = col;
            this.data = data;
        }

        public string getRow() { return row; }
        public string getCol() { return col; }
        public string getData() { return data; }
    }
}
