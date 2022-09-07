using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace PGE.Interfaces.SAP.WOSynchronization
{
    /// <summary>
    /// Class to export the data to CSV File
    /// </summary>
    public class CSVFileWriter
    {
        /// <summary>
        /// Logger to log error / debug/ user information
        /// </summary>
        private static log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Exports the data to CSV Files
        /// </summary>
        /// <param name="data">Data to be exported</param>
        /// <param name="csvFileName">CSV target file name</param>
        public static void Export(IList<IRowData2> data, string csvFileName)
        {
            try
            {
                //Validate the data
                if (data == null || data.Count < 1) return;

                //Create StreamWrite to create csv file
                StreamWriter csvWriter = new StreamWriter(csvFileName);

                IDictionary<string, string> fieldValues;
                StringBuilder csvData = new StringBuilder();
                //Invalid characters to handle
                char[] invalidChars = new char[] { '"', ',' };
                //Add headers
                fieldValues = data[0].FieldValues;
                foreach (string fieldName in fieldValues.Keys)
                {
                    //Handle the invalid characters such that the exported CSV file should exactly represent the data
                    if (fieldName.IndexOfAny(invalidChars) != -1)
                    {
                        csvData.AppendFormat("\"{0}\",", fieldName.Replace("\"", "\"\""));
                    }
                    else csvData.AppendFormat("{0},", fieldName);
                }
                //Remove ',' at the end
                csvData.Remove(csvData.Length - 1, 1);
                //Write to File
                csvWriter.WriteLine(csvData.ToString());

                //Loop through each RowData
                for (int loopCount = 0; loopCount < data.Count; loopCount++)
                {

                    //Get the field values
                    fieldValues = data[loopCount].FieldValues;
                    if (fieldValues == null || fieldValues.Count < 1) continue;
                    csvData = new StringBuilder();

                    //Add the Data to StringBuilder
                    string fieldValue;
                    foreach (string fieldName in fieldValues.Keys)
                    {
                        fieldValue = fieldValues[fieldName];
                        if (fieldValue.IndexOfAny(invalidChars) != -1)
                        {
                            csvData.AppendFormat("\"{0}\",", fieldValue.Replace("\"", "\"\""));
                        }
                        else csvData.AppendFormat("{0},", fieldValue);
                    }
                    csvData.Remove(csvData.Length - 1, 1);
                    //Write to File
                    csvWriter.WriteLine(csvData.ToString());
                }
                //Flush Close the Write
                csvWriter.Flush();
                csvWriter.Close();
            }
            catch (Exception ex)
            {
                _logger.Error("Error exporting data.",ex);
            }
        }


    }
}
