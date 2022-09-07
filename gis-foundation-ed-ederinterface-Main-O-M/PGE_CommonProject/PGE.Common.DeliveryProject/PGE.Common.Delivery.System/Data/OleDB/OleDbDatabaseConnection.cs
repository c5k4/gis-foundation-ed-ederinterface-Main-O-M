using System.Data.OleDb;
using System.Runtime.InteropServices;
using System.Data.Common;
using System.Data;

namespace PGE.Common.Delivery.Systems.Data.OleDb
{
    /// <summary>
    /// A supporting class for handling simple queries against a connection using the <see cref="System.Data.OleDb"/> drivers.
    /// </summary>
    [ComVisible(false)]
    [ClassInterface(ClassInterfaceType.None)]
    public class OleDbDatabaseConnection : DatabaseConnection<OleDbConnection>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OleDbDatabaseConnection"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public OleDbDatabaseConnection(string connectionString)
            : base(new OleDbConnection(connectionString))
        {
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Creates the apdater that is used by the <see cref="System.Data.Common.DbConnection"/>.
        /// </summary>
        /// <returns>The <see cref="System.Data.Common.DbDataAdapter"/> for the specified connection.</returns>
        protected override System.Data.Common.DbDataAdapter CreateApdater()
        {
            return new OleDbDataAdapter();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public override DbParameter CreateParameter(string parameterName)
        {
            DbParameter param = new OleDbParameter(parameterName, OleDbType.IUnknown);
            return param;
        }

        #endregion
    }
}