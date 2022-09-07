using PGE.Common.Delivery.Diagnostics;
using PGE.Common.Delivery.Framework;
using System;
using System.Data;
using System.Data.OleDb;
using System.Reflection;
using System.Text.RegularExpressions;
using PGE_DBPasswordManagement;


namespace PGE.Common.ChangesManagerShared
{
    public class AdoOleDbConnection
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

        private string _decryptedPassword;

        public string DecryptedPassword
        {
            get
            {
                return _decryptedPassword;
            }
        }
        private string _connectionStringDecrypted;
        public string ConnectionStringDecrypted
        {
            get
            {
                if (String.IsNullOrEmpty(_connectionStringDecrypted))
                {
                    // m4jf edgisrearch 919
                    // DecryptConnectionStringPassword();
                    _connectionStringDecrypted = "Provider=OraOLEDB.Oracle.1;"+ReadEncryption.GetConnectionStr(_connectionStringEncrypted);

                }
                return _connectionStringDecrypted;
            }
        }

        public string UserId
        {
            get
            {
                return GetUserId();
            }
        }

        private OleDbConnection _oleDbConnection = null;
        private OleDbTransaction _transaction = null;

        public OleDbConnection OleDbConnection
        {
            get
            {
                if (_oleDbConnection == null)
                {
                    CreateOpenOleDbConnection();
                }
                return _oleDbConnection;
            }
        }

        public OleDbTransaction DbTransaction
        {
            get
            {
                if (_transaction == null)
                {
                    _transaction = OleDbConnection.BeginTransaction(IsolationLevel.ReadCommitted);
                }
                return _transaction;
            }
        }

        public AdoOleDbConnection(string connectionString)
        {
            _connectionStringEncrypted = connectionString;
        }

        public OleDbTransaction BeginOleDbTransaction()
        {
            return DbTransaction;
        }

        private void CreateOpenOleDbConnection()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            DecryptConnectionStringPassword();
            _oleDbConnection = new OleDbConnection(_connectionStringDecrypted);
            _oleDbConnection.Open();
        }

        private string GetUserId()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            string REGEX_PATTERN = @"user\s*id=[^;]+";

            Match match = Regex.Match(_connectionStringEncrypted, REGEX_PATTERN, RegexOptions.IgnoreCase);
            string userId = match.Value.Substring(match.Value.IndexOf("=") + 1);

            return userId.Replace(" ", "");
        }

        private void DecryptConnectionStringPassword()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            string REGEX_PATTERN = @"assword\s*=[^;]+";

            Match match = Regex.Match(_connectionStringEncrypted, REGEX_PATTERN, RegexOptions.IgnoreCase);

            string encryptedPassword = match.Value.Substring(match.Value.IndexOf("=") + 1);
            encryptedPassword = encryptedPassword.TrimStart(' ');
            _decryptedPassword = EncryptionFacade.Decrypt(encryptedPassword);

            _connectionStringDecrypted = _connectionStringEncrypted.Replace(encryptedPassword, _decryptedPassword);
        }

        public void Commit()
        {
            _transaction.Commit();
        }

        public void Rollback()
        {
            _transaction.Rollback();
        }
    }
}
