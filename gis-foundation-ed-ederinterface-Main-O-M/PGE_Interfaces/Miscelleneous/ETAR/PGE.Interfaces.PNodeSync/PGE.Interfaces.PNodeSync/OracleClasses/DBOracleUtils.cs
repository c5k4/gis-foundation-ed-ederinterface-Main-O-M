using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;


namespace PGE.Interface.PNodeSync.OracleClasses
{
    class DBOracleUtils
    {

        /// <summary>
        /// Create an Oracle connection string.
        /// </summary>
        /// <param name="host">The host name of the database.</param>
        /// <param name="port">Port being used.</param>
        /// <param name="sid">SID of the database.</param>
        /// <param name="user">User name the connection string will use to connect.</param>
        /// <param name="password">Encrypted password.</param>
        public static OracleConnection
                       GetDBConnection(String serviceName, String user, String password)
        {

            //Console.WriteLine("Getting Connection ...");

            // 'Connection string' to connect directly to Oracle.
            //string connString = "Data Source=(DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = "
            //     + host + ")(PORT = " + port + "))(CONNECT_DATA = (SERVER = DEDICATED)(SERVICE_NAME = "
            //     + sid + ")));Password=" + password + ";User ID=" + user;

            // 'Connection string' to connect directly to Oracle.
            //string connString = "Data Source=(DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = "
            //     + host + ")(PORT = " + port + "))(CONNECT_DATA = (SERVER = DEDICATED)(SERVICE_NAME = "
            //     + sid + ")));Password=" + password + ";User ID=" + user;

            string connString = "Data Source=" + serviceName + "; Password=" + password + ";User ID=" + user;
            //string connString = "Data Source=" + serviceName + "; Password=" + password + ";User ID=" + user + ";Min Pool size=10"; 

            OracleConnection conn = new OracleConnection();
            conn.ConnectionString = connString;
            return conn;
        }

    }

}
