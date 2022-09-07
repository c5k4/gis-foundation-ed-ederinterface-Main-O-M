using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Reflection;

using PGE.Interfaces.SAP.Interfaces;
using PGE.Interfaces.SAP.Data;

namespace PGE.Interfaces.SAP.Batch
{

    /// <summary>
    /// SAPCSVWriter Class helps to capture the csv file location and write a list of 
    /// SAPRow data into a csv file at the specified location
    /// </summary>
    public class SAPCSVWriter : ISAPDataOutput
    {
        private static readonly PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "SAPBatch.log4net.config");
        
        private string csvFileName;

        /// <summary>
        /// CSV file location and name
        /// </summary>
        public string CSVFileName
        {
            get
            {
                return csvFileName;
            }

            set
            {
                csvFileName = value;
            }
        }
        
        /// <summary>
        /// This method takes a list of SAPRowData and writes it into the csv file as
        /// comma separated values
        /// </summary>
        /// <param name="SAPRowDataList">The SAPRowData</param>
        /// /// <param name="sanitize">If True, clean up the data</param>
        public void OutputData(IList<SAPRowData> SAPRowDataList, bool sanitize)
        {
            _logger.Debug("Outputting Data");
            StreamWriter streamWriter = new StreamWriter(CSVFileName, false);
            int i = 0;

            Console.WriteLine("     " + SAPRowDataList.Count + " rows to write for [ " + csvFileName + " ]");
            StringBuilder finalBuilder = new StringBuilder();
            foreach (SAPRowData rowData in SAPRowDataList)
            {
                try
                {
                    StringBuilder builder = new StringBuilder();
                    int cntr=1;
                    foreach(string s in rowData.FieldValues.Values)
                    {
                        builder.Append( sanitize ? Sanitize(s) : s);
//                        builder.Append(Sanitize(s));
                        if (cntr < rowData.FieldValues.Count)
                        {
                            builder.Append(',');
                        }
                    }
                    i++;
                    finalBuilder.AppendLine(builder.ToString());

                    if ((i % 1000) == 0)
                    {
                        try
                        {
                            streamWriter.WriteLine(finalBuilder.ToString().TrimEnd());
                            streamWriter.Flush();
                        }
                        catch (Exception ex)
                        {
                            _logger.Error("Error writing to csv file: " + ex.Message);
                            break;
                        }

                        finalBuilder = new StringBuilder();
                    }
                    _logger.Debug("Row written to csv file. Assetid - " + rowData.AssetID);
                }
                catch (Exception ex)
                { 
                    _logger.Error("Error in constructing csv line from SAProw DB data. AssetID - " + rowData.AssetID, ex);
                }
            }

            try
            {
                streamWriter.Write(finalBuilder.ToString().TrimEnd());
            }
            catch (Exception ex)
            {
                _logger.Error("Error writing to csv file: " + ex.Message);
            }

            _logger.Info("     Wrote [ " + i + " ] of [ " + SAPRowDataList.Count + " ] on [ " + csvFileName + " ]");

            streamWriter.Close();
            streamWriter.Dispose();
            _logger.Debug(CSVFileName + " StreamWriter closed after writing out " + SAPRowDataList.Count + " asset records.");

        }

        /// <summary>
        /// This class sanitizes the data values if they contain '"' or ","
        /// The values are enclosed in double quotes if they contain the above chars.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string Sanitize(string input)
        {
            return PGE.Interfaces.SAP.Csv.Escape(input);

        }
    }
}
