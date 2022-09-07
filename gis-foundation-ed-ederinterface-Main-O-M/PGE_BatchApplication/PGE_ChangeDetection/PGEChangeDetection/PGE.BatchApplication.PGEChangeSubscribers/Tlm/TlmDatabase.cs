using System;
using System.Collections.Generic;
using System.Data;
//using System.Data.OracleClient;
using Oracle.DataAccess.Client;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using PGE.Common.ChangesManagerShared;
using PGE.Common.ChangesManagerShared.Utilities;
using PGE.Common.Delivery.Diagnostics;

namespace PGE.BatchApplication.ChangeSubscribers.Tlm
{
   public class TlmDatabase
    {
       private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "ChangeDetectionDefault.log4net.config");

       private const string FIELD_ERR_NUM = "ERR_NUM";
       private const string FIELD_ERR_MESSAGE = "ERROR_MESSAGE";

       private string _updateStoredProc;
       private string _storedProcStatusView;

       public AdoOracleConnection AdoOracleConnection { private set; get; }
    
       public TlmDatabase(AdoOracleConnection adoOracleConnection, string updateStoredProc, string storedProcStatusView)
       {
           AdoOracleConnection = adoOracleConnection;
           _updateStoredProc = updateStoredProc;
           _storedProcStatusView = storedProcStatusView;
       }

       public void WriteTlmChange(string globalId, string changeXML)
       {
           _logger.Debug(MethodBase.GetCurrentMethod().Name + " globalId [ " + globalId + " ]");

           //create or replace PACKAGE TLM_CD_MGMT AS 
           //procedure ProcessXML(XmlContent varchar2
           //--,ErrorMessage OUT VARCHAR2,IsSuccess OUT CHAR
           //); 
           try
           {
               _logger.Debug("Writing TlmChange [ " + changeXML + " ]");
               changeXML = changeXML.Replace("\r", "");
               changeXML = changeXML.Replace("\n", "");

               OracleCommand command = new OracleCommand(_updateStoredProc, AdoOracleConnection.OracleConnection);
               command.Transaction = AdoOracleConnection.DbTransaction;
               command.CommandType = CommandType.StoredProcedure;

               command.Parameters.Add("XmlContent", OracleDbType.Varchar2).Direction = ParameterDirection.Input;
               command.Parameters["XmlContent"].Value = changeXML;

               //string sql = "EXECUTE " + _updateStoredProc + "('" + changeXML +
               //    "');";
               command.ExecuteNonQuery();

           }
           catch (Exception exception)
           {
               _logger.Error("Error calling TLM Database Proc [ " + _updateStoredProc + " ]", exception);
               throw;
           }
       }


       public void Dispose()
       {
           if (AdoOracleConnection != null) AdoOracleConnection.OracleConnection.Close();
           AdoOracleConnection = null;
           
       }
    }

}
