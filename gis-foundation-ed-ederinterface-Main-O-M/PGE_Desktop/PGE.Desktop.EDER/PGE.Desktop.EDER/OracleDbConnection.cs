using System;
using System.Data;
using Oracle.DataAccess.Client;

namespace PGE.Desktop.EDER
{
    public class OracleDbConnection
    {
        [Obsolete]
        private readonly OracleConnection _conn;

        [Obsolete]
        public OracleDbConnection(String username, String password, string dataSource)
        {
            _conn = new OracleConnection("Data Source=" + dataSource + ";User ID=" +
                                                         username + ";Password=" + password + ";");
            _conn.Open();
        }

        [Obsolete]
        public OracleDataReader ExecuteCommand(String command)
        {
            OracleCommand cmd = new OracleCommand
            {
                Connection = _conn,
                CommandText = command,
                CommandType = CommandType.Text
            };

            return cmd.ExecuteReader();
        }

        [Obsolete]
        public object ExecuteScalar(String command)
        {
            OracleCommand cmd = new OracleCommand
            {
                Connection = _conn,
                CommandText = command,
                CommandType = CommandType.Text
            };

            return cmd.ExecuteScalar();
        }

        public void Close()
        {
            if (_conn != null && _conn.State != ConnectionState.Closed) _conn.Close();
        }
    }
}
