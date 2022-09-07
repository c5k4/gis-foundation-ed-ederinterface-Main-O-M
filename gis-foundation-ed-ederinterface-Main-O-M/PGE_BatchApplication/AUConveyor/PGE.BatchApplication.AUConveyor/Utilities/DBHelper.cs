using System;
using System.IO;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.Geodatabase;
using PGE_DBPasswordManagement;

namespace PGE.BatchApplication.AUConveyor.Utilities
{
    /// <summary>
    /// Contains methods for managing GIS database connections.
    /// </summary>
    internal static class DBHelper
    {
        /// <summary>
        /// Connects to the specified SDE connection.
        /// </summary>
        /// <param name="dbLocation">The location of the SDE file used to connect to the geodatabase.</param>
        /// <returns></returns>
        internal static IWorkspace Connect(string dbLocation)
        {
            IWorkspace ws = null;
            try
            {
                string SDEPath = ReadEncryption.GetSDEPath(dbLocation);
                //FileInfo fi = new FileInfo(dbLocation);
                //if (fi.Extension.ToUpper() == ".SDE")
                if(!string.IsNullOrWhiteSpace(SDEPath))
                {
                    Type t = Type.GetTypeFromProgID("esriDataSourcesGDB.SdeWorkspaceFactory");
                    IWorkspaceFactory wsFactory = Activator.CreateInstance(t) as IWorkspaceFactory;

                    //SdeWorkspaceFactory wsFactory = new SdeWorkspaceFactory();
                    ws = wsFactory.OpenFromFile(SDEPath, 0);
                }
                else
                {
                    AccessWorkspaceFactory accessWSFactory = new AccessWorkspaceFactoryClass();
                    ws = accessWSFactory.OpenFromFile(dbLocation, 0);
                }

                return ws;
            }
            catch (Exception e)
            {
                Program.WriteError("Could not connect to database and table.", false);
                //Only write detailed error message to the file listener.
                LogManager.FileLogger.IndentLevel++;
                LogManager.FileLogger.WriteLine(e.ToString());
                LogManager.FileLogger.IndentLevel--;
                return null;
            }
        }
    }
}
