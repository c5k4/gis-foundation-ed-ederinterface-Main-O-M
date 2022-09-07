// ========================================================================
// Copyright © 2021 PGE.
// <history> 
// Get workspace .(EDGISREARCH - 378)
// TCS M4JF 04/07/2021 Created
// </history>
// All rights reserved.
// ========================================================================
using System;
using ESRI.ArcGIS.Geodatabase;

namespace PGE.BatchApplication.OldSessionNotification
{
    public  class Common
    {
        
        public static Log4NetLogger _log = new Log4NetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, ReadConfigurations.LOGCONFIG);
       
        public  IWorkspace GetWorkspace(string argStrworkSpaceConnectionstring)
        {
            Type t = null;
            IWorkspaceFactory workspaceFactory = null;
            IWorkspace workspace = null;
            try
            {

                t = Type.GetTypeFromProgID("esriDataSourcesGDB.SdeWorkspaceFactory");
                workspaceFactory = (IWorkspaceFactory)Activator.CreateInstance(t);
                workspace = workspaceFactory.OpenFromFile(argStrworkSpaceConnectionstring, 0);
            }
            catch (Exception exp)
            {
                Common._log.Error("Error in getting SDE Workspace, function GetWorkspace: " + exp.Message);
                Common._log.Info(exp.Message + "   " + exp.StackTrace);
                throw exp;
            }
            return workspace;
        }

       
       
    }
}
