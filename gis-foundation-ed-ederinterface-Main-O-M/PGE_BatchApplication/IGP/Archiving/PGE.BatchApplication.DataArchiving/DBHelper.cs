
#region namespaces
using System;
using System.Data;
using Oracle.DataAccess.Client;
using System.Configuration;

#endregion
namespace PGE.BatchApplication.DataArchiving
{
    /// <summary>
    /// This class is written to perform Oracla Database Operation
    /// Execute SQL statement etc.
    /// </summary>
    /// 
    
    public class DBHelper
    {
        OracleConnection conOra = null;        
        private string  _strDBCode = string.Empty;
        public string[] oracleConnectionInfo = new string[0];
        public DBHelper()
        {
        }

        /// <summary>
        /// Opens Oracle Database Connection
        /// </summary>
        public void OpenConnection(string DBConnection)
        {          
            try
            {
                conOra = new OracleConnection();
                // m4jf edgisrearch 919 - Getting connection string using Password manageent tool
                //oracleConnectionInfo = DBConnection.Split(',');
                //string oracleConnectionString = "Data Source=" + oracleConnectionInfo[0] + ";User Id=" + oracleConnectionInfo[1] + ";password=" + oracleConnectionInfo[2];                
                // conOra.ConnectionString = oracleConnectionString;   
                conOra.ConnectionString = DBConnection;
                conOra.Open();
            }
            catch (Exception exp)
            {
                Program._log.Error(exp.Message + " at " + exp.StackTrace);
            }         
        }
        /// <summary>
        /// Closes Oracle Database Connection
        /// </summary>
        public void CloseConnection()
        {
            try
            {
                if (conOra.State == ConnectionState.Open)
                {
                    conOra.Close();
                }
                if (conOra != null)
                    conOra.Dispose();
                conOra = null;
            }
            catch (Exception exp)
            {
                Program._log.Error(exp.Message + " at " + exp.StackTrace);
            }
        }

        public void DeleteData(string From_Table, string Days, string DateField, string DBConnection)
        {
            try 
            {	        
                if (conOra == null)
                    OpenConnection(DBConnection);

                string OracleStatement= " DELETE From " + From_Table + " WHERE  " + DateField + " <= TRUNC(sysdate) -" + Days ;
                OracleCommand cmdzs = new OracleCommand(OracleStatement, conOra);
                int count= cmdzs.ExecuteNonQuery();
                Program._log.Info(count+ " rows deleted from "+ From_Table+" in " + oracleConnectionInfo[0] + " database");
              
            }
            catch (Exception exp)
            {
                Program._log.Error(exp.Message + " at " + exp.StackTrace);
            }  
            finally
            {
                CloseConnection();
            }        
        }        
    }
}
