// ========================================================================
// Copyright © 2021 PGE.
// <history>
// One Time Utility to Update MAP NO of Existing SUP(s) 
// TCS V3SF 02/23/2021               Created
// </history>
// All rights reserved.
// ========================================================================

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;

namespace PGE.Miscellaneous.WorkQueueMapnoUpdate
{
    class Program
    {
        private static string sPath = default;
        private static StreamWriter pSWriter = default;

        static void Main(string[] args)
        {
            #region Data Members
            IFeatureWorkspace edschmWorkspace = default;
            IFeatureWorkspace ederWorkspace = default;

            IFeatureClass workQueueFC = default;
            IFeatureClass mapGridFC = default;

            int indexMapNOWQ = default;
            int indexMapNoMG = default;

            IFeatureCursor featureCursor = default;
            IFeatureCursor cursor = default;

            IFeature feature = default;
            IFeature mapGridFeat = default;

            ISpatialFilter spatialFilter = default;

            StringBuilder Mapno = default;
            StringBuilder loggingPath = default;

            string EDSCHMSDE = default;
            string EDERSDE = default;

            QueryFilterClass queryFilterClass;
            #endregion

            try
            {
                loggingPath = new StringBuilder();
                loggingPath.Append((String.IsNullOrEmpty(ConfigurationManager.AppSettings["LOGPATH"]) ? System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) : ConfigurationManager.AppSettings["LOGPATH"]) + "\\Logfile_" + DateTime.Now.Ticks.ToString() + ".log");
                sPath = Convert.ToString(loggingPath);
                (pSWriter = File.CreateText(sPath)).Close();
                WriteLine("Initializing License");
                Common.InitializeESRILicense();
                WriteLine("License Acquired Successfully");

                EDSCHMSDE = PGE_DBPasswordManagement.ReadEncryption.GetSDEPath(ConfigurationManager.AppSettings["EDSCHM_DB_SDE"]);
                EDERSDE = PGE_DBPasswordManagement.ReadEncryption.GetSDEPath(ConfigurationManager.AppSettings["EDER_DB_SDE"]);

                queryFilterClass = new QueryFilterClass();
                queryFilterClass.WhereClause = "MAPNO is null";

                //WriteLine("EDSCHM SDE File Path :: " + ConfigurationManager.AppSettings["EDSCHM_DB_SDE"]);
                //WriteLine("EDER SDE File Path :: " + ConfigurationManager.AppSettings["EDER_DB_SDE"]);
                edschmWorkspace = (IFeatureWorkspace)ArcSdeWorkspaceFromFile(EDSCHMSDE);
                ederWorkspace = (IFeatureWorkspace)ArcSdeWorkspaceFromFile(EDERSDE);
                workQueueFC = edschmWorkspace.OpenFeatureClass("EDGIS.EDSCHEM_UpdatePolygon");
                mapGridFC = ederWorkspace.OpenFeatureClass("EDGIS.Schematics_Unified_Grid");

                if (workQueueFC == null || mapGridFC == null)
                {
                    WriteLine("Unable to Acquired Featureclasses");
                    return;
                }

                indexMapNOWQ = workQueueFC.Fields.FindField("MAPNO");
                indexMapNoMG = mapGridFC.FindField("MAPNO");

                Mapno = new StringBuilder();

                if (indexMapNOWQ != -1 && indexMapNoMG != -1)
                {
                    //cursor = workQueueFC.Search(new QueryFilterClass(), false);
                    cursor = workQueueFC.Search(queryFilterClass, false);
                    feature = cursor.NextFeature();

                    while (feature != null)
                    {
                        try
                        {
                            spatialFilter = new SpatialFilterClass();
                            spatialFilter.Geometry = feature.Shape;
                            spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;

                            featureCursor = mapGridFC.Search(spatialFilter, false);
                            mapGridFeat = featureCursor.NextFeature();
                            Mapno.Clear();

                            int i = 0;
                            while (mapGridFeat != null)
                            {
                                try
                                {
                                    if (i == 0)
                                    {
                                        Mapno.Append(Convert.ToString(mapGridFeat.Value[indexMapNoMG]));
                                        i++;
                                    }
                                    else
                                        Mapno.Append("," + Convert.ToString(mapGridFeat.Value[indexMapNoMG]));
                                }
                                catch (Exception ex)
                                {
                                    WriteLine("Error :: " + ex.StackTrace + " " + ex.Message);
                                }
                                mapGridFeat = featureCursor.NextFeature();
                            }

                            if (!string.IsNullOrEmpty(Convert.ToString(Mapno)))
                            {
                                feature.Value[indexMapNOWQ] = Convert.ToString(Mapno);
                                feature.Store();
                            }
                            WriteLine("MAPNO for SUP OID :: " + feature.OID + " : " + Convert.ToString(Mapno));
                        }
                        catch (Exception ex)
                        {
                            WriteLine("Error :: " + ex.StackTrace + " " + ex.Message);
                        }

                        feature = cursor.NextFeature();
                    }
                }
                else
                {
                    WriteLine("MAPNO field not found in Database");
                }

                WriteLine("Process Completed Sucessfully");
            }
            catch (Exception ex)
            {
                WriteLine("Error :: " + ex.StackTrace + " " + ex.Message);
            }
            finally
            {
                if (featureCursor != null) System.Runtime.InteropServices.Marshal.FinalReleaseComObject(featureCursor);
                if (cursor != null) System.Runtime.InteropServices.Marshal.FinalReleaseComObject(cursor);
                if (feature != null) System.Runtime.InteropServices.Marshal.FinalReleaseComObject(feature);
                if (mapGridFeat != null) System.Runtime.InteropServices.Marshal.FinalReleaseComObject(mapGridFeat);

                Common.CloseLicenseObject();
                WriteLine("License Checked-in Sucessfully");
            }
        }

        private static IWorkspace ArcSdeWorkspaceFromFile(String connectionFile)
        {
            return ((IWorkspaceFactory)Activator.CreateInstance(Type.GetTypeFromProgID("esriDataSourcesGDB.SdeWorkspaceFactory"))).
                OpenFromFile(connectionFile, 0);
        }

        private static void WriteLine(string sMsg)
        {
            pSWriter = File.AppendText(sPath);
            pSWriter.WriteLine(DateTime.Now.ToShortTimeString() + " - " + sMsg);
            //DrawProgressBar();
            Console.WriteLine(sMsg);
            pSWriter.Close();
        }
    }
}
