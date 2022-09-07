using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Data;

using ESRI.ArcGIS.Geodatabase;
using Oracle.DataAccess.Client;

using PGE.Common.Delivery.Diagnostics;
using PGE.Common.ChangesManagerShared;

namespace PGE.Common.ChangesManagerShared.Streetlights
{
    public class StreetlightCcb
    {
        #region Constants

        private const string CCB_TO_GIS_PROC = "GIS_I_WRITE.CCBTOGIS_STREETLIGHT_UPDATES";
        private const string CCB_TO_GIS_NOSAP_PROC = "GIS_I_WRITE.CCBTOGIS_SLIGHT_UPDATES_NOSAP";

        #endregion

        #region Member vars

        // For logging
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "ChangeDetectionDefault.log4net.config");

        // For database access
        private AdoOracleConnection _adoOracleConnectionSource;

        // For file access
        private string _reportPath = "C:\\temp\\slChangeDetection\\ccb\\";
        //private string _reportFile = "fx32799.itemins.txt";
        //ME Q3 ITEM 2018: DATA FROM GIS WILL BE SENT AS UPDATE ONLY IF DATA EXISTS IN SLCDX DATA
        private string _reportFile = "fx32799.itemchg.txt";
        private FileInfo _ccbInsertsFile = null;

        // For keeping track of the last GIS_ID value used
        private int _lastGisID = 0;

        // For keeping track of lookup table values
        private IDictionary<string, IDictionary<string, string>> _lookupValueDictionary = new Dictionary<string, IDictionary<string, string>>();

        #endregion

        #region Properties

        public string ReportPath
        {
            set
            {
                _reportPath = value;
            }
        }

        public string ReportFile
        {
            set
            {
                _reportFile = value;
            }
        }

        #endregion

        #region Constructors

        public StreetlightCcb(AdoOracleConnection adoOracleConnectionSource)
        {
            _adoOracleConnectionSource = adoOracleConnectionSource;
        }

        #endregion

        #region Public methods

        public IDictionary<string, string> GetLookupTableValues(string tablename)
        {
            IDictionary<string, string> lookupValues = null;

            // Nab it if weve already fetched it...
            if (_lookupValueDictionary.ContainsKey(tablename) == true)
            {
                lookupValues = _lookupValueDictionary[tablename];
            }
            else
            {
                lookupValues = new Dictionary<string, string>();

                // Otherwise fetch it...
                string sql = "select * from PGEDATA." + tablename;
                OracleCommand queryCommand = new OracleCommand(sql, _adoOracleConnectionSource.OracleConnection);
                OracleDataReader reader = queryCommand.ExecuteReader();
                if (reader.HasRows == true)
                {
                    while (reader.Read() == true)
                    {
                        lookupValues.Add(reader.GetValue(1).ToString(), reader.GetString(2));
                    }
                    _lookupValueDictionary.Add(tablename, lookupValues);
                }
            }

            // Return the result
            return lookupValues;
        }

        public string GetNextGisID()
        {
            try
            {
                if (_lastGisID == 0)
                {
                    string sql = "select max(to_number(gis_id)) as LastGISID from PGEDATA.FIELDPTS";
                    OracleCommand queryCommand = new OracleCommand(sql, _adoOracleConnectionSource.OracleConnection);
                    OracleDataReader reader = queryCommand.ExecuteReader();

                    while (reader.Read())
                    {
                        Console.WriteLine(reader[0].ToString());
                        _lastGisID = (int)reader.GetDecimal(0);
                        _lastGisID++;
                        break;
                    }
                    reader.Close();
                }
                else
                {
                    _lastGisID++;
                }

            }
            catch (Exception ex)
            {
                _logger.Error("Error generating next GIS ID: ", ex);
                throw ex;
            }

            return _lastGisID.ToString();
        }

        public void UpdateGisFromCcb(bool sendToSap = true)
        {
            // Log entry
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            try
            {
                // Build and execute a command to run the update proc
                OracleCommand queryCommand = null;

                if (sendToSap == true)
                {
                    _logger.Info("Executing procedure " + CCB_TO_GIS_PROC);
                    queryCommand = new OracleCommand(CCB_TO_GIS_PROC, _adoOracleConnectionSource.OracleConnection);
                }
                else
                {
                    _logger.Info("Executing procedure " + CCB_TO_GIS_NOSAP_PROC);
                    queryCommand = new OracleCommand(CCB_TO_GIS_NOSAP_PROC, _adoOracleConnectionSource.OracleConnection);
                }
                queryCommand.CommandType = CommandType.StoredProcedure;
                queryCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                _logger.Error("Error updating GIS from CCB: " + ex.ToString());
                throw ex;
            }
        }

        public void WriteCCB(IList<string> ccbValues)
        {
            try
            {
                if (ccbValues != null)
                {
                    if (ccbValues.Count > 0)
                    {
                        // Make sure we have a handle on the file
                        if (_ccbInsertsFile == null || _ccbInsertsFile.Exists == false)
                        {
                            _ccbInsertsFile = new FileInfo(Path.Combine(_reportPath, _reportFile));
                        }

                        // Create a writer
                        StreamWriter writer = null;
                        if (_ccbInsertsFile.Exists == false)
                        {
                            writer = _ccbInsertsFile.CreateText();
                        }
                        else
                        {
                            writer = _ccbInsertsFile.AppendText();
                        }

                        // Write and close
                        WriteToFile(ccbValues, writer);
                        writer.Close();
                    }
                    else { _logger.Warn("There is no values to be written in file"); }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to write streetlight to CCB inserts file: " + ex.ToString());
                throw ex;
            }
        }

        #endregion

        #region Private methods

        private static void WriteToFile(IList<string> ccbValues, StreamWriter writer)
        {
            if (ccbValues != null && ccbValues.Count > 0)
            {
                int elementCount = 0;
                foreach (string value in ccbValues)
                {
                    elementCount++;

                    writer.Write(value);

                    if (elementCount < ccbValues.Count)
                    {
                        writer.Write("\t");
                    }
                }
                writer.Write(writer.NewLine);
            }
        }

        #endregion
    }
}
