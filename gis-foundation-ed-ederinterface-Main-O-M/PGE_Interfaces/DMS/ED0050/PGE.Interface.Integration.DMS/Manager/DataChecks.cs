using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PGE.Interface.Integration.DMS.Common;
using System.Xml;
using PGE.Common.Delivery.Framework.Exceptions;

namespace PGE.Interface.Integration.DMS.Manager
{
    /// <summary>
    /// Class for performing SQL data checks on the data in the staging schema
    /// </summary>
    public class DataChecks
    {
        private static List<SQLCheck> _checks;

        /// <summary>
        /// The list of SQL checks to perform.
        /// </summary>
        public static List<SQLCheck> Checks
        {
            get
            {
                if (_checks == null)
                {
                    Initialize();
                }
                return DataChecks._checks;
            }
            set { DataChecks._checks = value; }
        }

        /// <summary>
        /// Execute all of the SQL Checks using the connection and return the errors
        /// </summary>
        /// <param name="connection">The connection string for the staging schema</param>
        /// <returns>Each error encountered concatenated together</returns>
        public static string CheckData(string connection)
        {
            string errors = "";
            foreach (SQLCheck check in Checks)
            {
                
                object result = Common.Oracle.GetSingleValue(check.SQL, connection);
                if (result != null)
                {
                    int output = -1;
                    int.TryParse(result.ToString(), out output);
                    if (!(output == 0))
                    {
                        errors += check.Error + "\r\n";
                    }
                }
                else
                {
                    errors += "Database error on: " + check.Error + "\r\n";
                }
            }

            return errors;
        }

        private static void Initialize(string file)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(file);
            //use the root node to initialize
            Initialize(xmlDoc.FirstChild);
        }

        private static void Initialize()
        {
            Initialize("DataChecks.xml");
        }

        private static void Initialize(XmlNode node)
        {
            _checks = new List<SQLCheck>();

            try
            {
                XmlNodeList values = node.SelectNodes("add");
                if (values != null)
                {
                    foreach (XmlNode value in values)
                    {
                        string sql = value.Attributes["sql"].Value;
                        string error = value.Attributes["error"].Value;
                        _checks.Add(new SQLCheck(sql,error));

                    }
                }
            }
            catch (Exception ex)
            {
                //if there were any errors/exceptions the configuration file is bad
                throw new InvalidConfigurationException("There was an error loading the Sites.xml file.", ex);
            }
        }
    }

    /// <summary>
    /// Data class for storing SQL checks read in from the XML config file
    /// </summary>
    public class SQLCheck
    {
        string _SQL;

        /// <summary>
        /// A SQL select statement. The SQL statement should return a number where 0 means there was no error.
        /// </summary>
        public string SQL
        {
            get { return _SQL; }
            set { _SQL = value; }
        }
        string _error;
        /// <summary>
        /// The error to return if the SQL statement does not return 0
        /// </summary>
        public string Error
        {
            get { return _error; }
            set { _error = value; }
        }
        /// <summary>
        /// Create a new SQLCheck
        /// </summary>
        /// <param name="sql">The SQL select statement</param>
        /// <param name="error">The error to return</param>
        public SQLCheck(string sql, string error)
        {
            _SQL = sql;
            _error = error;
        }
    }
}
