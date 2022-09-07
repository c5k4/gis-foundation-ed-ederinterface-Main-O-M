using System;
using System.Data;
using System.Data.OracleClient;

namespace Telvent.PGE.ED.Desktop
{
    public class OracleDbConnection
    {
        private readonly OracleConnection _conn;

        public OracleDbConnection(String username, String password, string dataSource)
        {
            _conn = new OracleConnection("Data Source=" + dataSource + ";User ID=" +
                                                         username + ";Password=" + password + ";");
            _conn.Open();
        }

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
