using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesGDB;

namespace PGE.BatchApplication.CircuitProtectionZone.Data
{
    public static class Geodatabase
    {
        public static IWorkspace SubstationWorkspace = null;
        public static IWorkspace LandbaseWorkspace = null;
        public static IWorkspace workspace = null;
        public static Dictionary<string, IObjectClass> ObjectClasses = new Dictionary<string, IObjectClass>();

        public static IWorkspace ExecuteSqlWorkspace = null;
        public static IObjectClass GetObjectClass(string className)
        {
            string ClassName = className.ToUpper();
            try
            {
                if (!ObjectClasses.ContainsKey(ClassName))
                {
                    ITable table = null;

#if DEBUG
                    //Get certain things from different database for development purposes
                    if (ClassName == "EDGIS.PGE_CircuitEnergizedZones".ToUpper() || ClassName == "EDGIS.PGE_CircuitProtectionDetails".ToUpper()
                        || ClassName == "EDGIS.PGE_CircuitEnergizedDetails".ToUpper() || ClassName == "EDGIS.FireIndexAreas_Draft".ToUpper()
                        || ClassName == "EDGIS.PGE_CircuitProtectionZones".ToUpper() || ClassName == "EDGIS.PGE_CircuitProtZones_Process".ToUpper())
                    {
                        table = ((IFeatureWorkspace)ExecuteSqlWorkspace).OpenTable(ClassName);
                    }
#endif

                    if (table == null)
                    {
                        try { table = ((IFeatureWorkspace)workspace).OpenTable(ClassName); }
                        catch
                        {
                            try
                            {
                                //Try to get it from the landbase database if the above failed
                                table = ((IFeatureWorkspace)LandbaseWorkspace).OpenTable(ClassName);
                            }
                            catch
                            {
                                //Finally try to get it from the substation database if the above failed
                                table = ((IFeatureWorkspace)SubstationWorkspace).OpenTable(ClassName);
                            }
                        }
                    }

                    if (table != null)
                    {
                        ObjectClasses.Add(ClassName, table as IObjectClass);
                    }
                    else
                    {
                        throw new Exception("Unable to find object class " + ClassName);
                    }
                }

                if (ObjectClasses.ContainsKey(ClassName)) { return ObjectClasses[ClassName]; }
                else
                {
                    throw new Exception("Unable to find object class " + ClassName);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to find object class " + ClassName);
            }
        }

        public static IFeatureClass GetFeatureClass(string className)
        {
            return GetObjectClass(className) as IFeatureClass;
        }

        public static ITable GetTable(string className)
        {
            return GetObjectClass(className) as ITable;
        }


    }
}
