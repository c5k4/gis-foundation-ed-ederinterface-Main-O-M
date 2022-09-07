using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using PGE.Common.Delivery.Framework;
using Miner.Interop;
//using PGE.Common.Delivery.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using Miner.ComCategories;
using Miner.Interop.Process;
using ESRI.ArcGIS.Framework;

namespace PGE.Desktop.EDER.Subtasks
{
    [ComVisible(true)]
    [Guid("F23DD1C5-69FB-4813-821E-32F3713CBA13")]
    [ProgId("PGE.Desktop.EDER.Subtasks.CheckCurrentOwnerSubtask")]
    [ComponentCategory(ComCategory.MMPxSubtasks)]
    public class CheckCurrentOwnerSubtask : IMMPxSubtask
    {
        protected static PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "");
        private static IApplication _app;
        private static IMMPxApplication _PxApp;       

        private static IWorkspace _loginWorkspace;
        private static string sessionCurOwnerTableName = "PGEDATA.SESSIONCURRENTOWNER";



        #region Component Registration

        /// <summary>
        /// Registers the specified reg key.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComRegisterFunction, ComVisible(false)]
        static void Register(string regKey)
        {
            Miner.ComCategories.MMPxSubtasks.Register(regKey);
        }

        /// <summary>
        /// Unregisters the specified reg key.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComUnregisterFunction, ComVisible(false)]
        static void Unregister(string regKey)
        {
            Miner.ComCategories.MMPxSubtasks.Unregister(regKey);
        }
        #endregion

        public bool Enabled(IMMPxNode pPxNode)
        {
            return true;
        }


        public bool Execute(IMMPxNode pPxNode)
        {
            string sessionId = string.Empty;
            string currentOwner = string.Empty;
            string recInsertDT = string.Empty;
            try
            {
                 _loginWorkspace = GetWorkspace();
                 if (_loginWorkspace != null)
                 {
                     ITable mmSessionTbl = ((IFeatureWorkspace)_loginWorkspace).OpenTable("PROCESS.MM_SESSION");
                     if (mmSessionTbl != null)
                     {
                         IQueryFilter sQf = new QueryFilterClass();
                         sQf.WhereClause = "SESSION_ID ='" + pPxNode.Id + "'";
                         ICursor sCur = mmSessionTbl.Search(sQf, false);
                         IRow sRw = sCur.NextRow();
                         if (sRw != null)
                         {
                             sessionId = pPxNode.Id.ToString();

                             currentOwner = sRw.get_Value(sRw.Fields.FindField("CURRENT_OWNER")).ToString();
                         }
                         //release cursor
                         if (sCur != null)
                         {
                             System.Runtime.InteropServices.Marshal.ReleaseComObject(sCur);
                             System.Runtime.InteropServices.Marshal.FinalReleaseComObject(sCur);
                             sCur = null;
                         }
                         recInsertDT = DateTime.Now.ToString("dd-MMM-yy hh:mm:ss");
                         string sqlInsert = "INSERT INTO " + sessionCurOwnerTableName + " VALUES ('" + sessionId + "','" + currentOwner + "', TO_DATE('" + recInsertDT + "', 'dd-mon-yy hh:mi:ss'))";
                         _loginWorkspace.ExecuteSQL(sqlInsert);
                     }
                 }

            }
            catch (Exception ex)
            {
                
            }

            return true;
        }


        //get workspace
        private static IWorkspace GetWorkspace()
        {
            _loginWorkspace = ((IMMPxApplicationEx2)_PxApp).Workspace;

            return _loginWorkspace;
        }

        public bool Initialize(IMMPxApplication pPxApp)
        {
            _PxApp = pPxApp;
            if (_PxApp != null)
            {
                return true;
            }
            else return false;
        }

        public string Name
        {           
            get { return "PGE Check CurrentOwner SubTask"; }
        }

        public bool Rollback(IMMPxNode pPxNode)
        {
           throw new NotImplementedException();
        }
    }
}
