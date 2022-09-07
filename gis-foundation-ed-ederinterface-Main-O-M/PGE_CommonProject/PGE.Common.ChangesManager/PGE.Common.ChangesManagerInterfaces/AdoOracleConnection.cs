using System.Data;
using Oracle.DataAccess.Client;
using System.Reflection;
using System.Text.RegularExpressions;
using PGE.Common.Delivery.Framework;
using PGE.Common.Delivery.Diagnostics;
using PGE_DBPasswordManagement;

namespace PGE.Common.ChangesManagerShared
{
    public class AdoOracleConnection
    {
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "");

        private string _connectionStringEncrypted;

        public string ConnectionStringEncrypted
        {
            get
            {
                return _connectionStringEncrypted;
            }
        }
        private string _connectionStringDecrypted;
        private OracleConnection _oracleConnection = null;
        private OracleTransaction _transaction = null;

        public OracleConnection OracleConnection
        {
            get
            {
                if (_oracleConnection == null)
                {
                    CreateOpenOracleConnection();
                }
                return _oracleConnection;
            }
        }

        public OracleTransaction DbTransaction
        {
            get
            {
                if (_transaction == null)
                {
                    _transaction = OracleConnection.BeginTransaction(IsolationLevel.ReadCommitted);
                }
                return _transaction;
            }
        }

        public AdoOracleConnection(string connectionString)
        {
            // M4JD EDGISREARCH 919
            // _connectionStringEncrypted = connectionString;
            _connectionStringEncrypted = ReadEncryption.GetConnectionStr(connectionString);
        }

        public OracleTransaction BeginOracleTransaction()
        {
            return DbTransaction;
        }

        private void CreateOpenOracleConnection()
        {
            // m4jf edgisrearch 919 - commented below line of code remove logging of password and encryption of password as nor connection string will be obtained using Password management tool
           // _logger.Debug(MethodBase.GetCurrentMethod().Name +  " [ " + _connectionStringEncrypted + " ]");

          //  DecryptConnectionStringPassword();

           // _oracleConnection = new OracleConnection(_connectionStringDecrypted);
            _oracleConnection = new OracleConnection(_connectionStringEncrypted);
            _oracleConnection.Open();
        }

        private void DecryptConnectionStringPassword()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            string REGEX_PATTERN = @"assword\s*=[^;]+";

            Match match = Regex.Match(_connectionStringEncrypted, REGEX_PATTERN, RegexOptions.IgnoreCase);

            string encryptedPassword = match.Value.Substring(match.Value.IndexOf("=") + 1);
            encryptedPassword = encryptedPassword.TrimStart(' ');
            string decryptedPassword = EncryptionFacade.Decrypt(encryptedPassword);

            _connectionStringDecrypted = _connectionStringEncrypted.Replace(encryptedPassword, decryptedPassword);
        }

        public void Commit()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            if (_transaction != null)
            {
                _transaction.Commit();
                _transaction = null;
            }
        }

        public void Rollback()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            if (_transaction != null)
            {
                _transaction.Rollback();
            }
        }
    }
}
