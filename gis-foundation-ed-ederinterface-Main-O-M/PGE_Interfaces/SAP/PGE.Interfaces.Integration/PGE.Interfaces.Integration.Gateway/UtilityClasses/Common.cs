using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using ESRI.ArcGIS.Geodatabase;
using log4net.Config;

namespace PGE.Interfaces.Integration.Gateway
{
    public class Common
    {
        #region Private variable 
        public static Log4NetLogger _log = new Log4NetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "pge.log4net.config");

        #endregion

        #region Public variable 
        #endregion

        #region Private Methods 
        #endregion

        public Common()
        {
            //log4net.GlobalContext.Properties["LogFileName"] = "MyLog";
            //XmlConfigurator.Configure();
        }

        #region Public Methods 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="argStrworkSpaceConnectionstring"></param>
        /// <returns></returns>
        public IWorkspace GetWorkspace(string argStrworkSpaceConnectionstring)
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

        
        #endregion

    }

    public class ProcessFlag
    {
        // Processed flag in all interfaces should be updated properly :(P- GIS Processed/E-GIS Error/T-In Transition/F-Failed/D- Done)
        
        public static string GISProcessed { get; } = "P";
        public static string GISError { get; } = "E";
        public static string InTransition { get; } = "T";
        public static string Failed { get; } = "F";
        public static string Completed { get; } = "D";

    }

    public enum InterfcaeName
    {
         ED07,
         ED11,
         ED13,
         ED13A,
         ED13B,
         ED14,
         ED15INDV,
         ED15SUMM
    }

    /// <summary>
    /// This class is for interface Summary Execution
    /// </summary>
    public class Argument
    {
        /// <summary>
        /// This property to get Sucess status of process
        /// </summary>
        public static string Done { get; } = "D;";
        /// <summary>
        /// This property to get failure status of process
        /// </summary>
        public static string Error { get; } = "F;";
        /// <summary>
        /// This property to get interface name 
        /// </summary>
        public static string Interface { get; } = "ED07;";
        /// <summary>
        /// This property to get name of the system to integration
        /// </summary>
        public static string Integration { get; } = "SAP;";
        /// <summary>
        /// This property to get type of interface
        /// </summary>
        public static string Type { get; } = "Inbound;";
    }
}
