using System.Runtime.InteropServices;

namespace PGE.Common.Delivery.Systems.Data.OleDb
{
    /// <summary>
    /// A supporting class for handling simple queries against an Oracle connection using the <see cref="System.Data.OleDb"/> drivers.
    /// </summary>
    [ComVisible(false)]
    [ClassInterface(ClassInterfaceType.None)]
    public class OracleDatabaseConnection : OleDbDatabaseConnection
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OracleDatabaseConnection"/> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        public OracleDatabaseConnection(string dataSource, string userName, string password)
            : base(string.Format("Provider=OraOLEDB.Oracle;Data Source={0};User Id={1};Password={2};OLEDB.NET=True;", dataSource, userName, password))
        {
        }

        #endregion
    }
}