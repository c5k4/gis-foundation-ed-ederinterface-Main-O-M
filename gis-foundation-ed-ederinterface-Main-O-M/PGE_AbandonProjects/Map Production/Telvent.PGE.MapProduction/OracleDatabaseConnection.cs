using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telvent.Delivery.Systems.Data;
using System.Data.Common;
using System.Data;
using Oracle.DataAccess.Client;
using System.Linq;

namespace Telvent.PGE.MapProduction
{
    /// <summary>
    /// OracleConnection Implementation of IDatabaseConnection interface.
    /// Extends the DatabaseConnection abstract method by passing in Oracl.DataAccess.Client.OracleConnection
    /// OracleConnection performs better than OleDBDataConnection
    /// </summary>
    public class OracleDatabaseConnection:DatabaseConnection<OracleConnection>
    {
        /// <summary>
        /// Connection string used by OracleConnection
        /// </summary>
        private const string connection = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SID={1})));User Id={2};Password={3};";

        /// <summary>
        /// Constructor. Calls the DatabaseConnection constructor by creating a new OracleConnection object
        /// </summary>
        /// <param name="server"></param>
        /// <param name="sid"></param>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        public OracleDatabaseConnection(string server, string sid, string user, string pass)
            : base(new OracleConnection(string.Format(connection,server,sid,user,pass)))
        {
        }

        /// <summary>
        /// Override to create OracleDataAdapter
        /// OracleDataAdapter performs better than the OleDBDataAdapter
        /// </summary>
        /// <returns></returns>
        protected override DbDataAdapter CreateApdater()
        {
            return new OracleDataAdapter();
        }

        /// <summary>
        /// CreateParameter Override. Creates a OracleParameter object and applies the ParameterName and the SourceColumn properties on the Parameter object to the ParameterName passed. 
        /// </summary>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public override DbParameter CreateParameter(string parameterName)
        {
            DbParameter parameter = new OracleParameter();
            parameter.ParameterName = parameterName;
            parameter.SourceColumn = parameterName;
            return parameter;

        }
    }
}
