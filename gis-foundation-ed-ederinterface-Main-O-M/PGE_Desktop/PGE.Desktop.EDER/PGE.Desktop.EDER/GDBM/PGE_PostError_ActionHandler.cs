using System;
using System.Runtime.InteropServices;
using Miner.Process.GeodatabaseManager.ActionHandlers;
using Miner.Process.GeodatabaseManager;
using Miner.Geodatabase.GeodatabaseManager;
using Miner.Geodatabase.GeodatabaseManager.Serialization;
using ESRI.ArcGIS.Geodatabase;
using System.IO;
using Miner.Interop.Process;


namespace PGE.Desktop.EDER.GDBM
{
    [Guid("ea219426-74f6-4021-8860-4128d046a9a4")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("PGE.Desktop.EDER.GDBM.PGE_PostError_ActionHandler")]
    public class PGE_PostError_ActionHandler : PxActionHandler
    {
        #region StaticMembers
        public static string strHost;
        private static StreamWriter pSWriter = default(StreamWriter);
        IMMPxApplication _pxApplication;
        IMMPxLogin pxLogin;
        const string strPostErrorLookupTableName = "EDGIS.PGE_POSTERROR_AH_LOOKUP";
        const string strSessionTable = "PROCESS.MM_SESSION";
        #endregion
        public PGE_PostError_ActionHandler()
        {
            this.Name = "PGE POST ERROR EMAIL NOTIFICATION";
            this.Description = "PGE POST ERROR EMAIL NOTIFICATION";
        }
        protected override bool PxSubExecute(PxActionData actionData, GdbmParameter[] parameters)
        {
             
            bool success = true;
            try
            {
                 
               sendMail(actionData);
              
            }
            catch
            {
              
                 success = false;
            }
            return success;
           
        }

        private void sendMail(PxActionData actionData)
        {
            ICursor pSearchCursor=null;
            try
            {
                String strVersionName = actionData.VersionName.ToString();
                IFeatureWorkspace featureWorkspace = (IFeatureWorkspace)((IWorkspace)actionData.Version);
                
                ITable AHLookupTable = null;
                QueryFilter queryFilter = null;
                IRow codeRow = null;
                String strToList = String.Empty;
                String strFromList = String.Empty;
                String strSubject = String.Empty;
                String strMessage = String.Empty;
               
                AHLookupTable = featureWorkspace.OpenTable(strPostErrorLookupTableName);

                if (AHLookupTable == null) { return; }

                string SessionName = FindSessionNameFromVersionName(featureWorkspace, strVersionName);
                if (string.IsNullOrEmpty(SessionName))
                {                  
                    return;
                }

                //if (!SessionName.Contains('_')) return;

                queryFilter = new QueryFilterClass();
                //queryFilter.WhereClause = string.Format("VERSIONNAME like '%" + SessionName + "%'");
                queryFilter.WhereClause = string.Format(SessionName.ToUpper() + "LIKE '%'||UPPER(VERSIONNAME)||'%'");

                
                pSearchCursor = AHLookupTable.Search(queryFilter, false);
                codeRow = pSearchCursor.NextRow();

                if (codeRow == null)
                {
                   
                    return;
                }
                

                strFromList = (codeRow.get_Value(codeRow.Fields.FindField("FROMLIST"))).ToString();
                
                strToList = (codeRow.get_Value(codeRow.Fields.FindField("TOLIST"))).ToString();
                
                strMessage = (codeRow.get_Value(codeRow.Fields.FindField("MESSAGE"))).ToString();
                
                strSubject = (codeRow.get_Value(codeRow.Fields.FindField("SUBJECT"))).ToString();
                
                strHost = (codeRow.get_Value(codeRow.Fields.FindField("HOST"))).ToString();
              
                string[] LanIDsList = strToList.Split(';');
                //send email
             
                for (int i = 0; i < LanIDsList.Length; i++)
                {
                    string lanID = LanIDsList[i];
                    EmailService.Send(mail =>
                    {
                        mail.From = strFromList;
                        mail.FromDisplayName = strFromList;
                        mail.To = lanID + "@pge.com;";
                        mail.Subject = strSubject + " Session: " + SessionName;
                        mail.BodyText = strMessage;

                    });
                }

            }
            catch (Exception ex)
            {
                
                throw ex;
            }
            finally
            {
                if (pSearchCursor != null)
                {
                    Marshal.ReleaseComObject(pSearchCursor);
                }
            }
        }

        private string FindSessionNameFromVersionName(IFeatureWorkspace featureWorkspace, string strVersionName)
        {
            string sessionname=string.Empty ;
            try
            {

                string SessionID = GetSessionID(strVersionName);
                
                ITable PSessionTable = featureWorkspace.OpenTable(strSessionTable);
             
                IQueryFilter pqf = new QueryFilterClass();
                pqf.WhereClause = "SESSION_ID = " + SessionID ;
                ICursor pcursor = PSessionTable.Search(pqf, false);
                IRow prow = pcursor.NextRow();
                if (prow != null)
                {
                    sessionname = prow.get_Value(prow.Fields.FindField("SESSION_NAME")).ToString();
                }
                Marshal.ReleaseComObject(pcursor);

            }
            catch { }


            return sessionname;
        }
       public string GetSessionID( string strVersionName  ) 
       {
          string strSessionID = String.Empty;
        try
        {
             string[] words  = strVersionName.Split('_');
             if (words.Length > 1)
             {
                 strSessionID = words[1];
             }
             else
             {
                 strSessionID = words[0];
             }
            
        }
        catch 
           {}
       
        return strSessionID;

       }
        public override bool Enabled(ActionType actionType, Actions gdbmAction, PostVersionType versionType)
        {
            bool enabled = false;
           
           
            if ((gdbmAction == Actions.PostError ))
            {
                
                enabled = true;
            }

            return enabled;

        }
       
      
    }
}
