using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;
using Telvent.Delivery.Diagnostics;
using System.Reflection;
using Miner.Framework;
using Miner;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;

namespace Telvent.PGE.MapProduction.SwizzleLayers
{
    public class Swizzle
    {
        private static Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "\\telvent.mapproduction.log4net.config", "MapProduction");
        /// <summary>
        /// Read the sde connection file.
        /// </summary>
        /// <param name="sdeFilePath">Path of the sde connection file.</param>
        /// <param name="error">Out the error</param>
        /// <returns>Sde connection property set</returns>
        internal protected static IPropertySet ReadConnection(string SdeFilePath, out string Error)
        {
            Error = "";
            //Taking sde workspace and reading connection file.
            IWorkspaceFactory sdeWorkSpaceFactory = new ESRI.ArcGIS.DataSourcesGDB.SdeWorkspaceFactoryClass();
            IPropertySet sdeConnectionProperties = sdeWorkSpaceFactory.ReadConnectionPropertiesFromFile(SdeFilePath);
            //checking the user name and password are available or not.
            //if not availble then storing the message in the out error variable.
            try
            {
                string propertyUserName = sdeConnectionProperties.GetProperty("User").ToString();
                string propertyPassword = sdeConnectionProperties.GetProperty("Password").ToString();
                if (string.IsNullOrEmpty(propertyUserName.Trim()) || string.IsNullOrEmpty(propertyPassword.Trim()))
                {
                    Error = "User Name or Password is not found in the sde file";
                    return null;
                }
            }
            catch
            {
                Error = "User Name or Password is not found in the sde file";
                return null;
            }

            return sdeConnectionProperties;
        }

        /// <summary>
        /// Get the all mxd file path in a folder
        /// </summary>
        /// <param name="mxdFolderPath">Folder path</param>
        /// <returns>All mxd file path.</returns>
        internal protected static IFileNames GetFileNames(string MxdFolderPath)
        {
            IFileNames2 fns = new FileNamesClass();
            fns.LoadFromPath(MxdFolderPath);
            IFileNames fls = fns.GetSubset("mxd|MXD|Mxd");
            return fls;
        }
        /// <summary>
        /// This method will find and replace the source connection of the feature layer and standalone table in the mxd file.
        /// </summary>
        /// <param name="MxdFilePath">Path of the mxd file.</param>
        /// <param name="DestinationConnectionProperties">Source will be set to featurelayer and standalone table</param>
        /// <param name="OriginConnectionProperties">Optional, If given then match the source of feature layer and standalone table andle set the destination connection to the matching feature layer and standalone tab</param>
        internal protected static void FindAndReplaceSourceConnection(string MxdFilePath, IPropertySet DestinationConnectionProperties, IPropertySet OriginConnectionProperties = null)
        {
            MinerRuntimeEnvironment runtime = new ArcGISRuntimeEnvironment();
            if (runtime.RuntimeMode == mmRuntimeMode.mmRuntimeModeArcMap)
            {
                FindAndReplaceDesktop(MxdFilePath, DestinationConnectionProperties, OriginConnectionProperties); 
            }
            else
            {
                FindAndReplaceStandalone(MxdFilePath, DestinationConnectionProperties, OriginConnectionProperties); 
            }
            ////creating new map document
            //ESRI.ArcGIS.Carto.IMapDocument mapDocument = null;
            //mapDocument = new ESRI.ArcGIS.Carto.MapDocumentClass();
            //_logger.Debug("MxdFilePath = " + MxdFilePath);
            ////Opening the mxd file.
            //mapDocument.Open(MxdFilePath);
            //try
            //{
            //    //getting the map
            //    IMap mxdMap = mapDocument.get_Map(0);
            //    //getting the currently assigned pagelayout to set this same pagelayout after swizzle the layer.
            //    IPageLayout mapPageLayOut = mapDocument.PageLayout;

            //    //swizzle the feature layer source.
            //    swizzleFeatureLayer(mxdMap, DestinationConnectionProperties, OriginConnectionProperties);
            //    //swizzle the standalone table source.
            //    //swizzleStandAloneTable(mxdMap, DestinationConnectionProperties, OriginConnectionProperties);


            //    //Replayer the content MAP in the map document (mxd) and saving and closing the map document.
            //    mapDocument.ReplaceContents((IMxdContents)mxdMap);
            //    //replacing the pagelayout.
            //    mapDocument.ReplaceContents((IMxdContents)mapPageLayOut);
            //    mapDocument.Save();
            //}
            //catch { throw; }
            //finally { mapDocument.Close(); }
        }

        /// <summary>
        /// This method will set the source of the feature layer to the destinationconnectionproperties.
        /// </summary>
        /// <param name="mxdMap">Map document map.</param>
        /// <param name="destinationConnectionProperties">Sde connection to set in the feature layer source.</param>
        /// <param name="originConnectionProperties">Optional, If given then match the source of feature layer and set the destination connection to the matching feature layer</param>
        private static void swizzleFeatureLayer(IMap mxdMap, IPropertySet destinationConnectionProperties,IPropertySet originConnectionProperties = null)
        {

            _logger.Debug("Getting feature layer from map");
            //Getting all the feature layer in the mapdocument in an enumeration
            UID uid = new UIDClass();
            uid.Value = "{40A9E885-5533-11d0-98BE-00805F7CED21}";
            IEnumLayer enumLayer = mxdMap.get_Layers(uid, true);
            enumLayer.Reset();
            //looping through each feature layer
            IFeatureLayer featureLayer = enumLayer.Next() as IFeatureLayer;
            while (featureLayer != null)
            {
                //checking if the feature layer datasource type is sde feautre class or not.
                _logger.Debug("DataSourceType = " + featureLayer.DataSourceType);
                if (featureLayer.DataSourceType.ToUpper() == "SDE Feature Class".ToUpper())
                {

                    IDataLayer2 dataLayer = featureLayer as IDataLayer2;
                    //checking whether the data source name is IDatasetName or not.
                    if (dataLayer.DataSourceName is IDatasetName)
                    {
                                               
                            //Getting IworkspaceName2d to change the connectionproperties of the feature layer.
                            IDatasetName pDatasetName = dataLayer.DataSourceName as IDatasetName;
                            IWorkspaceName2 pWorkspaceName = pDatasetName.WorkspaceName as IWorkspaceName2;
                            _logger.Debug("Matching originconnection with featurelayer source");
                            if (matchOriginConnectionWithFeatureLayerAndStandAloneTable(pWorkspaceName.ConnectionProperties, originConnectionProperties))
                            {
                                _logger.Debug("Setting source of feature layer with destinationsdeconnection");
                                //applying the new connection to the feature layer.
                                pWorkspaceName.ConnectionProperties = destinationConnectionProperties;
                            }

                    }
                }
                featureLayer = enumLayer.Next() as IFeatureLayer;
            }
        }

        /// <summary>
        /// Swizzle the stand alone table connection to destination connection.
        /// </summary>
        /// <param name="mxdMap">Map document map</param>
        /// <param name="DestinationConnectionProperties">Destination connection properties</param>
        /// <param name="originConnectionProperties">Optional, If given then match the source of standalone table and set the destination connection to the matching standalone table</param>
        private static void swizzleStandAloneTable(IMap mxdMap, IPropertySet destinationConnectionProperties,IPropertySet originConnectionProperties = null)
        {
            _logger.Debug("Getting collection of standalone table");
            //Changing the standalone table connection.
            IStandaloneTableCollection tableCollection = (IStandaloneTableCollection)mxdMap;

            if (tableCollection != null)
            {
                _logger.Debug("Total" + tableCollection.StandaloneTableCount + " standalone table found");
                //checking if map has the standalone table.
                if (tableCollection.StandaloneTableCount > 0)
                {
                    for (int tblCnt = 0; tblCnt < tableCollection.StandaloneTableCount; tblCnt++)
                    {
                        //getting the stand alone table.
                        IStandaloneTable standAloneTable = tableCollection.get_StandaloneTable(tblCnt);
                        IDataLayer2 standAloneDataLayer = standAloneTable as IDataLayer2;
                        //checking whether the data source name is IDatasetName or not.
                        if (standAloneDataLayer.DataSourceName is IDatasetName)
                        {
                            //Getting IworkspaceName2d to change the connectionproperties of the feature layer.
                            IDatasetName standAloneDatasetName = standAloneDataLayer.DataSourceName as IDatasetName;
                            IWorkspaceName2 standAloneWorkspaceName = standAloneDatasetName.WorkspaceName as IWorkspaceName2;
                            //Matching the connection of the origin connection and sstandalone table source connection.
                            // if origin connection properties is null then this methode will return the true.
                            //If originconnectionproperties is not null then this methode will match the standalone table source connection with originconnection.
                            _logger.Debug("Matching originconnection with standalone table source");
                            if (matchOriginConnectionWithFeatureLayerAndStandAloneTable(standAloneWorkspaceName.ConnectionProperties, originConnectionProperties))
                            {
                                //applying the new connection to the standalone table.
                                standAloneWorkspaceName.ConnectionProperties = destinationConnectionProperties;
                            }

                        }

                    }
                }

            }
        }
        /// <summary>
        /// Matching the connection of the originconnection and objectconnection.
        // if originconnectionproperties is null then this methode will return the true.
        //If originconnectionproperties is not null then this methode will match the objectconnection with originconnection and if matched return true else false.
        /// </summary>
        /// <param name="objectConnectionProperties">Connection properties</param>
        /// <param name="originConnectionProperties">Connection properties</param>
        /// <returns>true= originConnectionProperties matched with objectConnectionProperties or originConnectionProperties is null
        /// false= originConnectionProperties does not matched with the objectConnectionProperties</returns>
        private static Boolean matchOriginConnectionWithFeatureLayerAndStandAloneTable(IPropertySet objectConnectionProperties, IPropertySet originConnectionProperties)
        {
            // Checking if originConnectionProperties is null then it will not match and return the true.
            if (originConnectionProperties == null)
            {
                _logger.Debug("OriginConnectionProperties is null. Returning true");
                return true;

            }
            //Getting the server,instance and database value from originConnectionProperties.
            string originServer = originConnectionProperties.GetProperty("Server").ToString().ToUpper();
            string originInstance = originConnectionProperties.GetProperty("Instance").ToString().ToUpper();
            string originDatabase = originConnectionProperties.GetProperty("Database").ToString().ToUpper();
            _logger.Debug("Origin connection details server = " + originServer + "service =" + originInstance + "Database =" + originDatabase);
            object objectName = null;
            object objectValue = null;
            //This will be rarly happen. checking is objectconneciton properties has the property.
            if (objectConnectionProperties.Count < 1) return false;
            //Getting all the properties
            objectConnectionProperties.GetAllProperties(out objectName, out objectValue);
            //getting the server,instance and database value from objectConnectionProperties.
            string objectServer = "";
            string objectInstance = "";
            string objectDatabase = "";
            string[] strName = (string[]) objectName;
            if (strName.Contains("SERVER"))
            {
                objectServer = objectConnectionProperties.GetProperty("Server").ToString().ToUpper();
            }
            if (strName.Contains("INSTANCE"))
            {
                objectInstance = objectConnectionProperties.GetProperty("Instance").ToString().ToUpper();
            }
            if (strName.Contains("DATABASE"))
            {
                objectDatabase = objectConnectionProperties.GetProperty("Database").ToString().ToUpper();
            }
            _logger.Debug("Featurelayer/Standalone table source connection details server = " + objectServer + "service =" + objectInstance + "Database =" + objectDatabase);
            //Matching both the connection and returning the value.
            return (originServer.Equals(objectServer) && originInstance.Equals(objectInstance) && originDatabase.Equals(objectDatabase));
        }
        
        private static void FindAndReplaceStandalone(string MxdFilePath, IPropertySet DestinationConnectionProperties,IPropertySet OriginConnectionProperties = null)
        {
            //creating new map document
            ESRI.ArcGIS.Carto.IMapDocument mapDocument = null;
            mapDocument = new ESRI.ArcGIS.Carto.MapDocumentClass();
            _logger.Debug("MxdFilePath = " + MxdFilePath);
            //Opening the mxd file.
            mapDocument.Open(MxdFilePath);
            try
            {
                //getting the map
                IMap mxdMap = mapDocument.get_Map(0);
                //getting the currently assigned pagelayout to set this same pagelayout after swizzle the layer.
                IPageLayout mapPageLayOut = mapDocument.PageLayout;

                //swizzle the feature layer source.
                swizzleFeatureLayer(mxdMap, DestinationConnectionProperties,OriginConnectionProperties);
                //swizzle the standalone table source.
                swizzleStandAloneTable(mxdMap, DestinationConnectionProperties, OriginConnectionProperties);


                //Replayer the content MAP in the map document (mxd) and saving and closing the map document.
                mapDocument.ReplaceContents((IMxdContents)mxdMap);
                //replacing the pagelayout.
                mapDocument.ReplaceContents((IMxdContents)mapPageLayOut);
                mapDocument.Save();
            }
            catch { throw; }
            finally { mapDocument.Close(); }
        }

        private static void FindAndReplaceDesktop(string MxdFilePath, IPropertySet DestinationConnectionProperties, IPropertySet OriginConnectionProperties = null)
        {
            Type type = Type.GetTypeFromProgID("esriFramework.AppRef");
            System.Object obj = Activator.CreateInstance(type);
            IApplication app = obj as IApplication;
            //creating new map document
            if (app != null)
            {
                app.OpenDocument(MxdFilePath);
                try
                {
                    //getting the map
                    IMap mxdMap = ((IMxDocument)app.Document).FocusMap;
                    //swizzle the feature layer source.
                    swizzleFeatureLayer(mxdMap, DestinationConnectionProperties, OriginConnectionProperties);
                    //swizzle the standalone table source.
                    swizzleStandAloneTable(mxdMap, DestinationConnectionProperties, OriginConnectionProperties);
                    app.SaveDocument();
                }
                catch { throw; }
            }
        }
    }
}
