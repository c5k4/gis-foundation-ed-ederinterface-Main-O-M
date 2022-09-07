using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Oracle.ManagedDataAccess; 
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;
using System.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Configuration;
using System.Data;


namespace PLDBBatchProcess
{
    class PLDInfoProcessor
    {
        private SqlConnection _pConn = null;

        public void SynchronizePLD_Info()
        {
            try
            {
                //Load up the workspaces 
                Shared.InitializeLogfile();
                Shared.InitializeSQLfile(0); 
                Shared.WriteToLogfile("Entering " + MethodBase.GetCurrentMethod());
                Shared.WriteToLogfile("Connecting to workspaces");
                Shared.LoadWorkspaces();
               
                Shared.WriteToLogfile("======================================================");
                PopulatePLD_Info_Temp();
                Shared.WriteToLogfile("Copy the data from temp table to main table");
                InsertIntoPLD_Info();
                Shared.WriteToLogfile("Process Completed Successfully");
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw ex;
            }
            finally
            {
                if (_pConn != null)
                {
                    if (_pConn.State == System.Data.ConnectionState.Open)
                        _pConn.Close();
                }
            }
        }

        //private void PopulatePLC_Info()
        //{
        //    SqlDataReader reader = null;
        //    IFeatureClassLoad featureClassLoad = null;
        //    int recCount = 0;

        //    try
        //    {
        //        //Step 1. Populate a list of SAPEQUIPIDs already loaded 
        //        //into the featureclass 
        //        IFeatureWorkspace pFWS = (IFeatureWorkspace)Shared.GetWorkspaceByName("wip");
        //        //IFeatureWorkspace pFWS = (IFeatureWorkspace)Shared.GetWorkspace("C:\\Simon\\Data\\LocalFileGDB\\Scratch.gdb"); 
        //        IWorkspaceEdit pWSE = (IWorkspaceEdit)pFWS;

        //        //First delete all existing rows if necessary 
        //        IFeatureClass pDestFC = pFWS.OpenFeatureClass(PLDBBatchConstants.PLD_INFO_FC_NAME);
        //        //IFeatureClass pDestFC = pFWS.OpenFeatureClass("PLD_INFO");
                
        //        //Get the input and output spatial references 
        //        int geoType = (int)ESRI.ArcGIS.Geometry.esriSRGeoCSType.esriSRGeoCS_WGS1984;
        //        ISpatialReferenceFactory pSRF = new SpatialReferenceEnvironmentClass();
        //        ISpatialReference pWGS84_SR = pSRF.CreateGeographicCoordinateSystem(geoType);
        //        ISpatialReference pUTM10_SR = ((IGeoDataset)pDestFC).SpatialReference;

        //        //Get a list of statuses and names from PLDB 
        //        Hashtable hshStatuses = GetPoleStatuses();

        //        //2. Query the PLDB for the Poles 
        //        reader = GetPoleAnalysisQuery();                                

        //        //Step 3. Loop through the records in the PoleAnalysis query 
        //        //and where they have not been already added populate the 
        //        //PLC_Info featureclass 

        //        //Get the field indexes
        //        int PLDBIDfldIdx = pDestFC.Fields.FindField("PLDBID");
        //        int sapEquipIdFldIdx = pDestFC.Fields.FindField("SAPEQUIPID");
        //        int pldbStatusFldIdx = pDestFC.Fields.FindField("PLD_STATUS");
        //        int latFldIdx = pDestFC.Fields.FindField("LAT");
        //        int longFldIdx = pDestFC.Fields.FindField("LONGITUDE");
        //        int elevationFldIdx = pDestFC.Fields.FindField("ELEVATION");
        //        int horizSFFldIdx = pDestFC.Fields.FindField("HORIZONTAL_SF");
        //        int overallSFFldIdx = pDestFC.Fields.FindField("OVERALL_SF");
        //        int bendingSFFldIdx = pDestFC.Fields.FindField("BENDING_SF");
        //        int verticalSFFldIdx = pDestFC.Fields.FindField("VERTICAL_SF");
        //        int classFldIdx = pDestFC.Fields.FindField("CLASS");
        //        int lengthInInchesFldIdx = pDestFC.Fields.FindField("LENGTHININCHES");
        //        int speciesFldIdx = pDestFC.Fields.FindField("SPECIES");
        //        int globalIdFldIdx = pDestFC.Fields.FindField("GLOBALID_PLD");
        //        int lanIdFldIdx = pDestFC.Fields.FindField("LANID");
        //        int snowLoadFldIdx = pDestFC.Fields.FindField("SNOW_LOAD_DIST");
        //        int oderDescriptionFldIdx = pDestFC.Fields.FindField("ORDER_DESCRIPTION");

        //        int newFeatureCount = 0;
        //        int editCount = 0;
        //        int sapEquipId = 0;
        //        //string sapEquipIdString = string.Empty;
        //        IPoint pPoint = null;
        //        double latitude = 0;
        //        double longitude = 0;
        //        double elevation = 0; 
        //        string classString;
        //        int statusIdx = -1;
        //        string statusName = string.Empty; 
        //        object o = null;
        //        long pldbid = 0;

        //        Shared.WriteToLogfile("calling DeleteSearchedRows");
        //        ((ITable)pDestFC).DeleteSearchedRows(null);
        //        Shared.WriteToLogfile("completed");

        //        pWSE.StartEditing(false);
        //        pWSE.StartEditOperation();
                                
        //        //If this is not an existing sapEquipId then add the 
        //        //feature to the featureclass 

        //        // Cast the feature class to the IFeatureClassLoad interface.
        //        featureClassLoad = (IFeatureClassLoad)pDestFC;

        //        // Enable load-only mode on the feature class (much faster) 
        //        featureClassLoad.LoadOnlyMode = true;

        //        //Create an insert featurecusor                     
        //        IFeatureCursor pInsertCursor = pDestFC.Insert(true);
        //        IFeatureBuffer pInsertFB = pDestFC.CreateFeatureBuffer();

        //        int latitudeOrdinal = reader.GetOrdinal("latitude");
        //        int longitudeOrdinal = reader.GetOrdinal("longitude");
        //        int sapEquipIdOrdinal = reader.GetOrdinal("pge_sapequipid");
        //        int pldbidOrdinal = reader.GetOrdinal("pldbid");
        //        int classOrdinal = reader.GetOrdinal("class");
        //        int speciesOrdinal = reader.GetOrdinal("species");
        //        int pgeGlobalidOrdinal = reader.GetOrdinal("pge_globalid");
        //        int elevationOrdinal = reader.GetOrdinal("elevation");
        //        int statusIdxOrdinal = reader.GetOrdinal("statusidx");
        //        int snowLoadOrdinal = reader.GetOrdinal("pge_snowLoaddistrict");
        //        int lengthOrdinal = reader.GetOrdinal("LenghtInInches");
        //        int verticalFSOrdinal = reader.GetOrdinal("VerticalFactorOfSafety");
        //        int bendingFSOrdinal = reader.GetOrdinal("bendingfactorofsafety");
        //        int overallFSOrdinal = reader.GetOrdinal("polefactorofsafety");
        //        int horizontalFSOrdinal = reader.GetOrdinal("polestrengthfactor");

        //        while (reader.Read())
        //        {
        //            //Loop through the features to clip   
        //            recCount++;

        //            //Check we do not already have this feature 

        //            //Set all the attributes 

        //            //LATITUDE 
        //            o = reader.GetDouble(latitudeOrdinal);
        //            if (o != DBNull.Value)
        //            {
        //                latitude = Convert.ToDouble(o); 
        //                pInsertFB.set_Value(latFldIdx, o);
        //            }

        //            //LONGITUDE 
        //            o = reader.GetDouble(longitudeOrdinal);
        //            if (o != DBNull.Value)
        //            {
        //                longitude = Convert.ToDouble(o);
        //                pInsertFB.set_Value(longFldIdx, o);
        //            }
                    
        //            //SAPEQUIPID  
        //            o = reader.GetValue(sapEquipIdOrdinal);
        //            if (o != DBNull.Value)
        //            {
        //                pInsertFB.set_Value(sapEquipIdFldIdx, o.ToString());
        //            }

        //            //PLDBID 
        //            pldbid = 0;
        //            o = reader.GetValue(pldbidOrdinal);
        //            if (o != DBNull.Value)
        //            {
        //                pldbid = Convert.ToInt64(o);
        //                pInsertFB.set_Value(PLDBIDfldIdx, pldbid); 
        //            }
                    
        //            //CLASS 
        //            classString = string.Empty; 
        //            o = reader.GetValue(classOrdinal);
        //            if (o != DBNull.Value)
        //            {
        //                classString = o.ToString();
        //                pInsertFB.set_Value(classFldIdx, classString); 
        //            }

        //            //SPECIES 
        //            o = reader.GetValue(speciesOrdinal);
        //            if (o != DBNull.Value)
        //            {
        //                pInsertFB.set_Value(speciesFldIdx, o.ToString());
        //            }

        //            //PGE_GLOBALID 
        //            o = reader.GetValue(pgeGlobalidOrdinal);
        //            if (o != DBNull.Value)
        //            {
        //                pInsertFB.set_Value(globalIdFldIdx, o.ToString());
        //            }                    

        //            //ELEVATION 
        //            elevation = 0;
        //            o = reader.GetValue(elevationOrdinal);
        //            if (o != DBNull.Value)
        //            {
        //                elevation = Convert.ToDouble(o);
        //                pInsertFB.set_Value(elevationFldIdx, elevation);
        //            }

        //            //PLDB_STATUS
        //            statusName = string.Empty; 
        //            o = reader.GetValue(statusIdxOrdinal);
        //            if (o != DBNull.Value)
        //            {
        //                statusIdx = Convert.ToInt32(o);
        //                statusName = hshStatuses[statusIdx].ToString();
        //                pInsertFB.set_Value(pldbStatusFldIdx, statusName); 
        //            }

        //            //SNOW_LOAD_DIST 
        //            o = reader.GetValue(snowLoadOrdinal);
        //            if (o != DBNull.Value)
        //            {
        //                pInsertFB.set_Value(snowLoadFldIdx, o.ToString());
        //            }

        //            //LENGTHININCHES 
        //            o = reader.GetValue(lengthOrdinal);
        //            if (o != DBNull.Value)
        //            {
        //                pInsertFB.set_Value(lengthInInchesFldIdx, o);
        //            }

        //            //LANID 
        //            pInsertFB.set_Value(lanIdFldIdx, "Unset");
                                        
        //            //HORIZONTAL_SF
        //            o = reader.GetValue(horizontalFSOrdinal);
        //            if (o != DBNull.Value)
        //            {
        //                pInsertFB.set_Value(horizSFFldIdx, o);
        //            }

        //            //OVERALL_SF
        //            o = reader.GetValue(overallFSOrdinal);
        //            if (o != DBNull.Value)
        //            {
        //                pInsertFB.set_Value(overallSFFldIdx, o);
        //            }

        //            //BENDING_SF
        //            o = reader.GetValue(bendingFSOrdinal);
        //            if (o != DBNull.Value)
        //            {
        //                pInsertFB.set_Value(bendingSFFldIdx, o);
        //            }

        //            //VERTICALFACTOROFSAFETY  
        //            o = reader.GetValue(verticalFSOrdinal);
        //            if (o != DBNull.Value)
        //            {
        //                pInsertFB.set_Value(bendingSFFldIdx, o);
        //            }

        //            //ORDER_DESCRIPTION 
        //            o = reader.GetValue(sapEquipIdOrdinal);
        //            if (o != DBNull.Value)
        //            {
        //                pInsertFB.set_Value(oderDescriptionFldIdx, o.ToString());
        //            }

        //            //Set the shape 
        //            pPoint = new PointClass();
        //            pPoint.PutCoords(longitude, latitude);
        //            pPoint.SpatialReference = pWGS84_SR;
        //            pPoint.Project(pUTM10_SR);

        //            pInsertFB.Shape = pPoint;

        //            //Insert the new feature 
        //            newFeatureCount++;
        //            editCount++;
        //            try
        //            {
        //                pInsertCursor.InsertFeature(pInsertFB);
        //            }
        //            catch (Exception ex)
        //            {
        //                Shared.WriteToLogfile("Failed to insert featurebuffer on: " +
        //                    " pldbid: " + pldbid.ToString() +
        //                    " error: " + ex.Message);
        //                throw new Exception("Unable to insert feature");
        //            }

        //            //Flush every record 
        //            if (recCount % 5000 == 0)
        //            {
        //                Shared.WriteToLogfile("Written: " + editCount.ToString() + " Poles");
        //                try
        //                { pInsertCursor.Flush(); }
        //                catch
        //                { throw new Exception("Unable to flush feature buffer"); }
        //                newFeatureCount = 0;
        //            }
        //            //if (recCount % 25000 == 0)
        //            //{
        //            //    Shared.WriteToLogfile("Rolling over the edit operation");
        //            //    pWSE.StopEditOperation();
        //            //    pWSE.StartEditOperation(); 
        //            //}
        //        }

        //        if (reader != null)
        //            reader.Close();

        //        //Flush if necessary 
        //        if (newFeatureCount != 0)
        //            pInsertCursor.Flush();

        //        //Switch this back off again  
        //        featureClassLoad.LoadOnlyMode = false;

        //        //Stop edit operation 
        //        pWSE.StopEditOperation();
        //        pWSE.StopEditing(true);

        //        //Release the cursor and buffer 
        //        Marshal.FinalReleaseComObject(pInsertCursor);
        //        Marshal.FinalReleaseComObject(pInsertFB);

        //    }
        //    catch (Exception ex)
        //    {
        //        if (reader != null)
        //            reader.Close();
        //        Shared.WriteToLogfile("Error in PopulatePLD_Info: " + ex.Message); 
        //        throw new Exception("Error populating PLD_Info featureclass"); 
        //    }
        //}

        //private void PopulatePLC_Info2()
        //{
        //    SqlDataReader reader = null;
        //    //IFeatureClassLoad featureClassLoad = null;
        //    int recCount = 0;

        //    try
        //    {
        //        //Open the PLD_Info featureclass 
        //        IFeatureWorkspace pFWS = (IFeatureWorkspace)Shared.GetWorkspaceByName("wip");
        //        IWorkspaceEdit pWSE = (IWorkspaceEdit)pFWS; 
        //        IFeatureClass pDestFC = pFWS.OpenFeatureClass(PLDBBatchConstants.PLD_INFO_FC_NAME);

        //        //Get the input and output spatial references 
        //        int geoType = (int)ESRI.ArcGIS.Geometry.esriSRGeoCSType.esriSRGeoCS_WGS1984;
        //        ISpatialReferenceFactory pSRF = new SpatialReferenceEnvironmentClass();
        //        ISpatialReference pWGS84_SR = pSRF.CreateGeographicCoordinateSystem(geoType);
        //        ISpatialReference pUTM10_SR = ((IGeoDataset)pDestFC).SpatialReference;

        //        //Get a list of statuses and names from PLDB 
        //        Hashtable hshStatuses = GetPoleStatuses();

        //        //2. Query the PLDB for the Poles 
        //        reader = GetPoleAnalysisQuery();

        //        //Step 3. Loop through the records in the PoleAnalysis query 
        //        //and where they have not been already added populate the 
        //        //PLC_Info featureclass 

        //        //Get the field indexes
        //        //int PLDBIDfldIdx = pDestFC.Fields.FindField("PLDBID");
        //        //int sapEquipIdFldIdx = pDestFC.Fields.FindField("SAPEQUIPID");
        //        //int pldbStatusFldIdx = pDestFC.Fields.FindField("PLD_STATUS");
        //        //int latFldIdx = pDestFC.Fields.FindField("LAT");
        //        //int longFldIdx = pDestFC.Fields.FindField("LONGITUDE");
        //        //int elevationFldIdx = pDestFC.Fields.FindField("ELEVATION");
        //        //int horizSFFldIdx = pDestFC.Fields.FindField("HORIZONTAL_SF");
        //        //int overallSFFldIdx = pDestFC.Fields.FindField("OVERALL_SF");
        //        //int bendingSFFldIdx = pDestFC.Fields.FindField("BENDING_SF");
        //        //int verticalSFFldIdx = pDestFC.Fields.FindField("VERTICAL_SF");
        //        //int classFldIdx = pDestFC.Fields.FindField("CLASS");
        //        //int lengthInInchesFldIdx = pDestFC.Fields.FindField("LENGTHININCHES");
        //        //int speciesFldIdx = pDestFC.Fields.FindField("SPECIES");
        //        //int globalIdFldIdx = pDestFC.Fields.FindField("GLOBALID_PLD");
        //        //int lanIdFldIdx = pDestFC.Fields.FindField("LANID");
        //        //int snowLoadFldIdx = pDestFC.Fields.FindField("SNOW_LOAD_DIST");
        //        //int oderDescriptionFldIdx = pDestFC.Fields.FindField("ORDER_DESCRIPTION");

        //        int editCount = 0;
        //        string sapEquipId = "NULL";
        //        IPoint pPoint = null;
        //        string latitude = "NULL";
        //        string longitude = "NULL";
        //        string elevation = "NULL";
        //        string snowLoadDistrict = "NULL";
        //        string lengthInInches = "NULL";
        //        string horizontalSF = "NULL";
        //        string overallSF = "NULL";
        //        string bendingSF = "NULL";
        //        string verticalSF = "NULL";
        //        string classString = "NULL";
        //        string species = "NULL";
        //        string globalId = "NULL"; 
        //        string statusName = "NULL";
        //        string pldbid = "NULL";
        //        int statusIdx = -1;
        //        object o = null;                 

        //        Shared.WriteToLogfile("calling DeleteSearchedRows");
        //        ((ITable)pDestFC).DeleteSearchedRows(null);
        //        Shared.WriteToLogfile("completed");


        //        int latitudeOrdinal = reader.GetOrdinal("latitude");
        //        int longitudeOrdinal = reader.GetOrdinal("longitude");
        //        int sapEquipIdOrdinal = reader.GetOrdinal("pge_sapequipid");
        //        int pldbidOrdinal = reader.GetOrdinal("pldbid");
        //        int classOrdinal = reader.GetOrdinal("class");
        //        int speciesOrdinal = reader.GetOrdinal("species");
        //        int pgeGlobalidOrdinal = reader.GetOrdinal("pge_globalid");
        //        int elevationOrdinal = reader.GetOrdinal("elevation");
        //        int statusIdxOrdinal = reader.GetOrdinal("statusidx");
        //        int snowLoadOrdinal = reader.GetOrdinal("pge_snowLoaddistrict");
        //        int lengthOrdinal = reader.GetOrdinal("LenghtInInches");
        //        int verticalFSOrdinal = reader.GetOrdinal("VerticalFactorOfSafety");
        //        int bendingFSOrdinal = reader.GetOrdinal("bendingfactorofsafety");
        //        int overallFSOrdinal = reader.GetOrdinal("polefactorofsafety");
        //        int horizontalFSOrdinal = reader.GetOrdinal("polestrengthfactor");

        //        string fieldsList = "LAT, LONGITUDE, SAPEQUIPID, PLDBID, CLASS, SPECIES, GLOBALID_PLD, ELEVATION, PLD_STATUS, SNOW_LOAD_DIST, LENGTHININCHES, LANID, HORIZONTAL_SF, OVERALL_SF, BENDING_SF, VERTICAL_SF, ORDER_DESCRIPTION"; 

        //        string sqlInsertStatement = string.Empty;
        //        StringBuilder sqlStatement = new StringBuilder();
        //        double latDouble = 0;
        //        double longDouble = 0; 


        //        while (reader.Read())
        //        {
        //            //Loop through the features to clip   
        //            recCount++;
        //            latDouble = 0;
        //            longDouble = 0;

        //            //Check we do not already have this feature 

        //            //Set all the attributes 

        //            //LATITUDE 
        //            latitude = "NULL"; 
        //            o = reader.GetValue(latitudeOrdinal);
        //            if (o != DBNull.Value)
        //            {
        //                latitude = o.ToString();
        //                double.TryParse(latitude, out latDouble); 
        //            }

        //            //LONGITUDE 
        //            longitude = "NULL";
        //            o = reader.GetDouble(longitudeOrdinal);
        //            if (o != DBNull.Value)
        //            {
        //                longitude = o.ToString();
        //                double.TryParse(longitude, out longDouble); 
        //            }

        //            //SAPEQUIPID  
        //            sapEquipId = "NULL"; 
        //            o = reader.GetValue(sapEquipIdOrdinal);
        //            if (o != DBNull.Value)
        //            {
        //                sapEquipId = o.ToString(); 
        //            }

        //            //PLDBID 
        //            pldbid = "NULL";
        //            o = reader.GetValue(pldbidOrdinal);
        //            if (o != DBNull.Value)
        //            {
        //                pldbid = o.ToString();
        //            }

        //            //CLASS 
        //            classString = "NULL";
        //            o = reader.GetValue(classOrdinal);
        //            if (o != DBNull.Value)
        //            {
        //                classString = o.ToString();
        //            }

        //            //SPECIES 
        //            species = "NULL"; 
        //            o = reader.GetValue(speciesOrdinal);
        //            if (o != DBNull.Value)
        //            {
        //                species = "'" + o.ToString() + "'";
        //            }

        //            //PGE_GLOBALID 
        //            globalId = "NULL"; 
        //            o = reader.GetValue(pgeGlobalidOrdinal);
        //            if (o != DBNull.Value)
        //            {
        //                globalId = o.ToString(); 
        //            }

        //            //ELEVATION 
        //            elevation = "NULL";
        //            o = reader.GetValue(elevationOrdinal);
        //            if (o != DBNull.Value)
        //            {
        //                elevation = o.ToString(); 
        //            }

        //            //PLDB_STATUS
        //            statusName = "NULL";
        //            o = reader.GetValue(statusIdxOrdinal);
        //            if (o != DBNull.Value)
        //            {
        //                statusIdx = Convert.ToInt32(o);
        //                statusName = hshStatuses[statusIdx].ToString();
        //            }

        //            //SNOW_LOAD_DIST
        //            snowLoadDistrict = "NULL"; 
        //            o = reader.GetValue(snowLoadOrdinal);
        //            if (o != DBNull.Value)
        //            {
        //                snowLoadDistrict = o.ToString(); 
        //            }

        //            //LENGTHININCHES
        //            lengthInInches = "NULL"; 
        //            o = reader.GetValue(lengthOrdinal);
        //            if (o != DBNull.Value)
        //            {
        //                lengthInInches = o.ToString(); 
        //            }

        //            //LANID 
        //            //pInsertFB.set_Value(lanIdFldIdx, "Unset");

        //            //HORIZONTAL_SF
        //            horizontalSF = "NULL"; 
        //            o = reader.GetValue(horizontalFSOrdinal);
        //            if (o != DBNull.Value)
        //            {
        //                horizontalSF = o.ToString(); 
        //            }

        //            //OVERALL_SF
        //            overallSF = "NULL";
        //            o = reader.GetValue(overallFSOrdinal);
        //            if (o != DBNull.Value)
        //            {
        //                overallSF = o.ToString(); 
        //            }

        //            //BENDING_SF
        //            bendingSF = "NULL"; 
        //            o = reader.GetValue(bendingFSOrdinal);
        //            if (o != DBNull.Value)
        //            {
        //                bendingSF = o.ToString(); 
        //            }

        //            //VERTICALFACTOROFSAFETY  
        //            o = reader.GetValue(verticalFSOrdinal);
        //            if (o != DBNull.Value)
        //            {
        //                //pInsertFB.set_Value(bendingSFFldIdx, o);
        //            }

        //            //ORDER_DESCRIPTION 
        //            o = reader.GetValue(sapEquipIdOrdinal);
        //            if (o != DBNull.Value)
        //            {
        //                //pInsertFB.set_Value(oderDescriptionFldIdx, o.ToString());
        //            }

        //            if ((latDouble != 0) && (longDouble != 0))
        //            {

        //                //Set the shape 
        //                pPoint = new PointClass();
        //                pPoint.PutCoords(longDouble, latDouble);
        //                pPoint.SpatialReference = pWGS84_SR;
        //                pPoint.Project(pUTM10_SR);

                    
        //                sqlStatement.Clear();
        //                sqlStatement.Append("INSERT INTO " + "WEBR.PLD_INFO" + " ");
        //                sqlStatement.Append(" (");
        //                sqlStatement.Append("objectid, species, shape");
        //                sqlStatement.Append(" ) ");
        //                sqlStatement.Append("VALUES");
        //                sqlStatement.Append(" (");
        //                sqlStatement.Append("R634318.nextval" + ", ");
        //                sqlStatement.Append(species + ", ");
        //                sqlStatement.Append(String.Format("sde.st_geometry('point ({0} {1})', 300001)", pPoint.X.ToString(), pPoint.Y.ToString()));
        //                //sqlStatement.Append(latitude.ToString() + ",");
        //                //sqlStatement.Append(longitude.ToString() + ",");
        //                //sqlStatement.Append(pldbid + ",");
        //                //sqlStatement.Append("'" + sapEquipId + "'" + ",");
        //                //sqlStatement.Append("'" + classString + "'" + ",");
        //                //sqlStatement.Append("'" + species + "'" + ",");
        //                //sqlStatement.Append("'" + pge_glo + "'" + ",");
        //                //sqlStatement.Append("'" + pSpan.ConductorClass + "'" + ",");
        //                //sqlStatement.Append("'" + pSpan.ConductorSubtype + "'" + ",");
        //                //sqlStatement.Append("'" + pSpan.SpanAngle + "'");
        //                sqlStatement.Append(" );");

        //                Shared.WriteToSQLFile(sqlStatement.ToString()); 
        //            }

        //            //Report progress periodically  
        //            if (recCount % 5000 == 0)
        //            {
        //                Shared.WriteToLogfile("Written: " + recCount.ToString() + " Poles");
        //            }
        //        }

        //        if (reader != null)
        //            reader.Close();
        //    }
        //    catch (Exception ex)
        //    {
        //        if (reader != null)
        //            reader.Close();
        //        Shared.WriteToLogfile("Error in PopulatePLD_Info: " + ex.Message);
        //        throw new Exception("Error populating PLD_Info featureclass");
        //    }
        //}
        /// <summary>
        /// This will use the SqlBulkLoad to load into a temporary 
        /// table first, it will then execute a stored proc which 
        /// will load the data into PLD_INFO featureclass 
        /// </summary>
        private void PopulatePLD_Info_Temp()
        {
            SqlDataReader reader = null;
            int recCount = 0;

            try
            {

                //Get the SQL to create the table 
                string sql = Shared.GetSQLFromSQLFile(
                    PLDBBatchConstants.CREATE_PLD_INFO_TEMP_TABLE_SQL);

                //First have to delete and rfecreate the table 
                DropTable(PLDBBatchConstants.PLD_INFO_TEMP_TABLE);
                CreateTable(sql);

                //Open the PLD_Info featureclass 
                IFeatureWorkspace pFWS = (IFeatureWorkspace)Shared.GetWorkspaceByName("wip");
                IWorkspaceEdit pWSE = (IWorkspaceEdit)pFWS;
                IFeatureClass pDestFC = pFWS.OpenFeatureClass(PLDBBatchConstants.PLD_INFO_FC_NAME);

                //Get the input and output spatial references 
                int geoType = (int)ESRI.ArcGIS.Geometry.esriSRGeoCSType.esriSRGeoCS_WGS1984;

                ISpatialReferenceFactory pSRF = new SpatialReferenceEnvironmentClass();
                ISpatialReference pWGS84_SR = pSRF.CreateGeographicCoordinateSystem(geoType);
                ISpatialReference pUTM10_SR = ((IGeoDataset)pDestFC).SpatialReference;

                //Get a list of statuses and names from PLDB 
                Hashtable hshStatuses = GetPoleStatuses();

                //2. Query the PLDB for the Poles 
                reader = GetPoleAnalysisQuery();

                long sapEquipId = 0;
                IPoint pPoint = null;
                double latitude = 0;
                double longitude = 0;
                double elevation = 0;
                string snowLoadDistrict = null;
                double lengthInInches = 0;
                double horizontalSF = 0;
                double overallSF = 0;
                double bendingSF = 0;
                double verticalSF = 0;
                string classString = null;
                string species = null;
                string globalId = null;
                string statusName = null;
                long pldbid = 0;
                int statusIdx = -1;
                object o = null;

                int latitudeOrdinal = reader.GetOrdinal("latitude");
                int longitudeOrdinal = reader.GetOrdinal("longitude");
                int sapEquipIdOrdinal = reader.GetOrdinal("pge_sapequipid");
                int pldbidOrdinal = reader.GetOrdinal("pldbid");
                int classOrdinal = reader.GetOrdinal("class");
                int speciesOrdinal = reader.GetOrdinal("species");
                int pgeGlobalidOrdinal = reader.GetOrdinal("pge_globalid");
                int elevationOrdinal = reader.GetOrdinal("elevation");
                int statusIdxOrdinal = reader.GetOrdinal("statusidx");
                int snowLoadOrdinal = reader.GetOrdinal("pge_snowLoaddistrict");
                int lengthOrdinal = reader.GetOrdinal("LenghtInInches");
                int verticalFSOrdinal = reader.GetOrdinal("VerticalFactorOfSafety");
                int bendingFSOrdinal = reader.GetOrdinal("bendingfactorofsafety");
                int overallFSOrdinal = reader.GetOrdinal("polefactorofsafety");
                int horizontalFSOrdinal = reader.GetOrdinal("polestrengthfactor");
                double xVal = 0;
                double yVal = 0;
                List<Pole> pEdgisPoles = new List<Pole>();

                while (reader.Read())
                {                    

                    //Set all the attributes 

                    //LATITUDE 
                    latitude = 0;
                    o = reader.GetValue(latitudeOrdinal);
                    if (o != DBNull.Value)
                    {
                        double.TryParse(o.ToString(), out latitude);
                    }

                    //LONGITUDE 
                    longitude = 0;
                    o = reader.GetDouble(longitudeOrdinal);
                    if (o != DBNull.Value)
                    {
                        double.TryParse(o.ToString(), out longitude);
                    }

                    //SAPEQUIPID  
                    sapEquipId = 0;
                    o = reader.GetValue(sapEquipIdOrdinal);
                    if (o != DBNull.Value)
                    {
                        Int64.TryParse(o.ToString(), out sapEquipId);
                    }

                    //PLDBID 
                    pldbid = 0;
                    o = reader.GetValue(pldbidOrdinal);
                    if (o != DBNull.Value)
                    {
                        Int64.TryParse(o.ToString(), out pldbid);
                    }

                    //CLASS 
                    classString = null;
                    o = reader.GetValue(classOrdinal);
                    if (o != DBNull.Value)
                    {
                        classString = o.ToString();
                    }

                    //SPECIES 
                    species = null;
                    o = reader.GetValue(speciesOrdinal);
                    if (o != DBNull.Value)
                    {
                        species = o.ToString();
                    }

                    //PGE_GLOBALID 
                    globalId = null;
                    o = reader.GetValue(pgeGlobalidOrdinal);
                    if (o != DBNull.Value)
                    {
                        globalId = o.ToString();
                    }

                    //ELEVATION 
                    elevation = 0;
                    o = reader.GetValue(elevationOrdinal);
                    if (o != DBNull.Value)
                    {
                         double.TryParse(o.ToString(), out elevation);
                    }

                    //PLDB_STATUS
                    statusName = null;
                    o = reader.GetValue(statusIdxOrdinal);
                    if (o != DBNull.Value)
                    {
                        statusIdx = Convert.ToInt32(o);
                        statusName = hshStatuses[statusIdx].ToString();
                    }

                    //SNOW_LOAD_DIST
                    snowLoadDistrict = null;
                    o = reader.GetValue(snowLoadOrdinal);
                    if (o != DBNull.Value)
                    {
                        snowLoadDistrict = o.ToString();
                    }

                    //LENGTHININCHES
                    lengthInInches = 0;
                    o = reader.GetValue(lengthOrdinal);
                    if (o != DBNull.Value)
                    {
                        double.TryParse(o.ToString(), out lengthInInches);
                    }

                    //LANID 
                    //pInsertFB.set_Value(lanIdFldIdx, "Unset");

                    //HORIZONTAL_SF
                    horizontalSF = 0;
                    o = reader.GetValue(horizontalFSOrdinal);
                    if (o != DBNull.Value)
                    {
                        double.TryParse(o.ToString(), out horizontalSF);
                    }

                    //OVERALL_SF
                    overallSF = 0;
                    o = reader.GetValue(overallFSOrdinal);
                    if (o != DBNull.Value)
                    {
                        double.TryParse(o.ToString(), out overallSF);
                    }

                    //BENDING_SF
                    bendingSF = 0;
                    o = reader.GetValue(bendingFSOrdinal);
                    if (o != DBNull.Value)
                    {
                        double.TryParse(o.ToString(), out bendingSF);
                    }

                    //VERTICALFACTOROFSAFETY
                    verticalSF = 0;   
                    o = reader.GetValue(verticalFSOrdinal);
                    if (o != DBNull.Value)
                    {
                        double.TryParse(o.ToString(), out verticalSF);
                    }

                    //ORDER_DESCRIPTION 
                    o = reader.GetValue(sapEquipIdOrdinal);
                    if (o != DBNull.Value)
                    {
                        //pInsertFB.set_Value(oderDescriptionFldIdx, o.ToString());
                    }

                    if ((latitude != 0) && (longitude != 0))
                    {
                        //increment the record count    
                        recCount++;

                        ////Set the shape 
                        pPoint = new PointClass();
                        pPoint.PutCoords(longitude, latitude);
                       // pPoint.SpatialReference = ((IGeoDataset)pDestFC).SpatialReference;
                        pPoint.SpatialReference = pWGS84_SR;
                        pPoint.Project(pUTM10_SR);

                        //Get the x and y in UTM spatial reference coords 
                        xVal = pPoint.X;
                        yVal = pPoint.Y;
                        //xVal = 0;
                        //yVal = 0;
                        Pole thePole = new Pole(
                            pldbid,
                            statusName,
                            latitude,
                            longitude,
                            elevation,
                            horizontalSF,
                            overallSF,
                            bendingSF,
                            verticalSF,
                            classString,
                            lengthInInches,
                            species,
                            sapEquipId,
                            globalId,
                            snowLoadDistrict,
                            xVal,
                            yVal);
                        pEdgisPoles.Add(thePole);

                        if (recCount % PLDBBatchConstants.INSERTS_PER_FILE == 0)
                        {
                            BulkUploadToOracle myData = BulkUploadToOracle.Load(
                                pEdgisPoles,
                                PLDBBatchConstants.PLD_INFO_TEMP_TABLE);
                            myData.Flush();
                            pEdgisPoles.Clear();
                        }
                    }

                    //Report progress periodically  
                    if (recCount % 5000 == 0)
                    {
                        Shared.WriteToLogfile("Written: " + recCount.ToString() + " Poles");
                    }
                }

                if (reader != null)
                    reader.Close();

                //Make sure to update any residual 
                if (pEdgisPoles.Count != 0)
                {
                    //There must be a residual - so send the SQL for 
                    //the remainder 
                    Shared.WriteToLogfile("Remainder exists");
                    BulkUploadToOracle myData = BulkUploadToOracle.Load(
                                pEdgisPoles,
                                PLDBBatchConstants.PLD_INFO_TEMP_TABLE);
                    myData.Flush();
                    pEdgisPoles.Clear();
                }                 
            }
            catch (Exception ex)
            {
                if (reader != null)
                    reader.Close();
                Shared.WriteToLogfile("Error in PopulatePLD_Info: " + ex.Message);
                throw new Exception("Error populating PLD_Info featureclass");
            }
        }

        /// <summary>
        /// So now the data show be in the PLD_INFO_TEMP
        /// table. We now run an 'INSERT INTO' query to 
        /// push the data into the PLD_INFO featureclass
        /// </summary>
        /// <param name="sqlCreateStatement"></param>
        private void InsertIntoPLD_Info()
        {
            OracleCommand pCmd = null;

            try
            {
                //There are 2 important config settings here 
                //make sure you get the right sequence for the 
                //generation of ObjectIds in PLD_INO and also 
                //make sure the SRID is correct for the PLD_INFO 
                //featureclass 

                string sqlDeleteIntoStatement ="truncate table  " + PLDBBatchConstants.PLD_INFO_FC_NAME ;

                string sequence = ConfigurationManager.AppSettings.Get(
                PLDBBatchConstants.CONFIG_PLD_INFO_SEQUENCE);
                string srid = ConfigurationManager.AppSettings.Get(
                PLDBBatchConstants.CONFIG_PLD_INFO_SRID);                
                
                string sqlInsertIntoStatement = string.Empty;
                sqlInsertIntoStatement = 
                "INSERT INTO " + PLDBBatchConstants.PLD_INFO_FC_NAME + " " +
                "(" +
                    "OBJECTID, PLDBID, PLD_STATUS, LAT, LONGITUDE, " +
                    "ELEVATION, HORIZONTAL_SF, OVERALL_SF, BENDING_SF, " +
                    "VERTICAL_SF, CLASS, LENGTHININCHES, SPECIES, " +
                    "SAPEQUIPID, GLOBALID_PLD, SNOW_LOAD_DIST, shape" +
                ")" +
                "(" +
                    "SELECT" + " " + 
                    sequence + ".nextval, PLDBID, PLD_STATUS, LAT, LONGITUDE, " +
                    "ELEVATION, HORIZONTAL_SF, OVERALL_SF, BENDING_SF, " +
                    "VERTICAL_SF, CLASS, LENGTHININCHES, SPECIES, " +
                    "SAPEQUIPID, GLOBALID_PLD, SNOW_LOAD_DIST, " +
                    "sde.st_point('point ('|| X_VALUE ||' '|| Y_VALUE ||')', " + srid + 
                    ") FROM " + PLDBBatchConstants.PLD_INFO_TEMP_TABLE +
                ")";

                using (OracleConnection connection = new OracleConnection(
                ConfigurationManager.AppSettings.Get(
                PLDBBatchConstants.CONFIG_WIP_CONNECTION)))
                {
                    connection.Open();
                    pCmd = connection.CreateCommand();
                    OracleTransaction transaction =
                       connection.BeginTransaction(
                           IsolationLevel.ReadCommitted);
                    pCmd.Transaction = transaction;

                    try
                    {
                        pCmd.CommandText = sqlDeleteIntoStatement;
                        pCmd.ExecuteNonQuery();
                        pCmd.CommandText = sqlInsertIntoStatement;
                        pCmd.ExecuteNonQuery();
                        transaction.Commit();
                    }
                    catch (Exception e)
                    {
                        Shared.WriteToLogfile( 
                            "Error running insert to PLD_INFO" + e.Message);
                        transaction.Rollback();
                    }
                }
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw ex;
            }
            finally
            {
                //release database 
                pCmd = null;
            }
        }

        private void CreateTable(string sqlCreateStatement)
        {
            OracleCommand pCmd = null;

            try
            {
                using (OracleConnection connection = new OracleConnection(
                ConfigurationManager.AppSettings.Get(
                PLDBBatchConstants.CONFIG_WIP_CONNECTION)))
                {
                    connection.Open(); 

                    //Execute the command against the database 
                    if (sqlCreateStatement != string.Empty)
                    {
                        pCmd = connection.CreateCommand(); 
                        pCmd.CommandText = sqlCreateStatement;
                        object o = pCmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw ex;
            }

            finally
            {
                //release database 
                pCmd = null;
            }
        }

        private void DropTable(string tableName)
        {
            OracleCommand pCmd = null;

            try
            {
                using (OracleConnection connection = new OracleConnection(ConfigurationManager.AppSettings.Get(
                PLDBBatchConstants.CONFIG_WIP_CONNECTION)))
                {
                    //Populate string to hold the SQL statement
                    connection.Open(); 
                    int recordCount = 0;
                    string sqlStatement =
                        "DROP TABLE " + tableName;

                    //Execute the command against the database 
                    if (sqlStatement != string.Empty)
                    {
                        pCmd = connection.CreateCommand(); 
                        pCmd.CommandText = sqlStatement;
                        object o = pCmd.ExecuteNonQuery();
                        recordCount = Convert.ToInt32(o);
                    }
                }            

            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
            }

            finally
            {
                //release database 
                pCmd = null;
            }
        }

        /// <summary>
        /// Has an IPoint, a new Geographic Coordinate System and a new Spatial Reference passed in 
        /// and returns an IPoint reprojected to the new Geographic Coordinate system.
        /// The esriTransformDirection should be set to esriTransformReverse if the PointToConvert is in a
        /// Projected Coordinate System i.e. X and Y values.  (Currently used by the Get Coordinates)
        /// The esriTransformDirection should be set to esriTransformForward if the PointToConvert is in a
        /// Geographic Coordinate System i.e. Decimal Degree values. (Currently used by the Goto Coordinates)
        /// NOTE:  This method does not work for Projected Coordinate Systems.
        /// </summary>
        /// <returns>
        /// The reprojected IPoint
        /// </returns>
        //public static IPoint Project(IPoint pointToConvert, esriSRGeoCSType newGeoCoordSystem, ISpatialReference newSpatialReference, esriTransformDirection transformDirection)
        //{
        //    //Create a projected coordinate system using the selected projected coordinate systems
        //    ISpatialReferenceFactory spatialReferenceFactory = new SpatialReferenceEnvironmentClass();

        //    //Create a Geographic Coordinate System based on the parameter passed in
        //    IGeographicCoordinateSystem geographicCoordinateSystem = spatialReferenceFactory.CreateGeographicCoordinateSystem((int)newGeoCoordSystem);
        //    //Create the Geographic transformation
        //    IGeoTransformation pGeoTransformation = (IGeoTransformation)spatialReferenceFactory.CreateGeoTransformation((int)esriSRGeoTransformation3Type.esriSRGeoTransformation_NAD_1983_harn_To_WGS_1984);
        //    //QI from the Geographic Coordinate System to the ISpatialReference
        //    ISpatialReference spatialReferenceGCS = geographicCoordinateSystem;
        //    //Create a new point
        //    IPoint newPoint = new Point();
        //    //If transforming from X/Y to Decimal Degrees
        //    if (transformDirection == esriTransformDirection.esriTransformReverse)
        //    {
        //        //Set the GCS Spatial Reference False Origin and Units
        //        spatialReferenceGCS.SetFalseOriginAndUnits(-180, -90, 1000000);
        //        //Set the coordinate system of the point to the passed in Spatial Reference
        //        newPoint.SpatialReference = newSpatialReference;
        //    }
        //    else
        //    {
        //        //Set the coordinate system of the point
        //        newPoint.SpatialReference = spatialReferenceGCS;
        //    }
        //    //Set in the coordinates of the new point
        //    newPoint.PutCoords(pointToConvert.X, pointToConvert.Y);
        //    //Get the geometry of this point.
        //    IGeometry2 pointGeom2 = (IGeometry2)newPoint;
        //    //Now project to new Coordinate system
        //    //If transforming from X/Y to Decimal Degrees
        //    if (transformDirection == esriTransformDirection.esriTransformReverse)
        //    {
        //        pointGeom2.ProjectEx(geographicCoordinateSystem, transformDirection, pGeoTransformation, false, 0, 0);
        //    }
        //    else
        //    {
        //        pointGeom2.ProjectEx(newSpatialReference, transformDirection, pGeoTransformation, false, 0, 0);
        //    }

        //    IGeoTransformation geoTransformation = spatialReferenceFactory.CreateGeoTransformation((int)esriSRGeoTransformationType.esriSRGeoTransformation_);


        //    return newPoint;
        //}

        private SqlDataReader GetPoleAnalysisQuery()
        {
            try
            {
                //Horizontal_SF is mapped to PoleStrengthFactor
                //overall_SF is mapped to PoleFactorOfSafety
                //VERTICAL_SF is mapped to VerticalFactorOfSafety 
                if (_pConn == null)
                {
                    //Get output DB connection 
                    _pConn = new SqlConnection(
                        ConfigurationManager.
                        ConnectionStrings[PLDBBatchConstants.
                        CONFIG_PLDB_CONNECTION].ConnectionString);
                }
                if (_pConn.State != System.Data.ConnectionState.Open)
                    _pConn.Open();

                //Setup the command 
                SqlCommand pCmd = _pConn.CreateCommand();

                //Return the SQL from the file in the install 
                //directory 
                string sql = Shared.GetSQLFromSQLFile(
                    PLDBBatchConstants.PLDB_INFO_QUERY_SQL);  
                
                //Run the SQL select statement 
                pCmd.CommandText = sql;
                string sapIdString = string.Empty;
                SqlDataReader reader = pCmd.ExecuteReader();  
                return reader;
            }
            catch (Exception ex)
            {
                throw new Exception("Error returning Pole Analysis query");
            }
        }
        
        private Hashtable GetPoleStatuses()
        {
            SqlConnection pConn = null; 
            SqlCommand pCmd = null;

            try
            {
                Hashtable hshStatuses = new Hashtable();
                pConn = new SqlConnection(
                        ConfigurationManager.
                        ConnectionStrings[PLDBBatchConstants. 
                        CONFIG_PLDB_CONNECTION].ConnectionString);
                pConn.Open();

                //Setup the command 
                pCmd = pConn.CreateCommand();

                //Run an INSERT INTO SELECT statement for the error 
                string sql = "SELECT * FROM dbo.STATUS_CODE;";

                pCmd.CommandText = sql;
                string sapIdString = string.Empty;
                int statusIdx = 0;
                object o = DBNull.Value;
                string statusName = string.Empty;
                SqlDataReader reader = pCmd.ExecuteReader();

                while (reader.Read())
                {
                    o = reader.GetValue(reader.GetOrdinal("statusidx"));
                    if (o != DBNull.Value)
                        statusIdx = Convert.ToInt32(o);

                    o = reader.GetValue(reader.GetOrdinal("name"));
                    if (o != DBNull.Value)
                        statusName = o.ToString();


                    if (!hshStatuses.ContainsKey(statusIdx))
                        hshStatuses.Add(statusIdx, statusName);

                }

                reader.Close(); 
                return hshStatuses;
            }
            catch (Exception ex)
            {
                throw new Exception("Error returning Pole Analysis query");
            }
            finally
            {
                if (pConn != null)
                {
                    if (pConn.State == System.Data.ConnectionState.Open)
                        pConn.Close();
                    pConn = null;
                    pCmd = null; 
                }
            }
        }
    }

    public class BulkUploadToOracle
    {

        private List<Pole> internalStore;
        //protected string tableName;
        //protected DataTable dataTable = new DataTable();
        protected int recordCount;
        //protected int commitBatchSize;

        private BulkUploadToOracle()
        {
            internalStore = new List<Pole>();
            this.recordCount = 0;
        }

        public static BulkUploadToOracle Load(List<Pole> thePoles, string tableName)
        {
            // create a new object to return
            BulkUploadToOracle o = new BulkUploadToOracle();
            foreach (Pole thePole in thePoles)
            {
                o.internalStore.Add(thePole);
            }
            return o;
        }

        public void Flush()

        {
            // transfer data to the datatable 
            this.WriteToDatabase();
        }

        private void WriteToDatabase()
        {
            using (OracleConnection connection = new OracleConnection(
                ConfigurationManager.AppSettings.Get(
                PLDBBatchConstants.CONFIG_WIP_CONNECTION)))
            {

                connection.Open();

                double[] horizSFs = new double[this.internalStore.Count];
                double[] overallSFs = new double[this.internalStore.Count];
                double[] bendingSFs = new double[this.internalStore.Count];
                double[] verticalSFs = new double[this.internalStore.Count];
                string[] classs = new string[this.internalStore.Count];
                double[] lengths = new double[this.internalStore.Count];
                string[] species = new string[this.internalStore.Count];
                string[] sapEquips = new string[this.internalStore.Count];
                string[] globalilds = new string[this.internalStore.Count];
                string[] snowLoads = new string[this.internalStore.Count];
                double[] xvals = new double[this.internalStore.Count];
                double[] yvals = new double[this.internalStore.Count];

                int j = 0; 
                foreach (Pole thePole in this.internalStore)
                {
                    horizSFs[j] = thePole.HorizontalSF;
                    overallSFs[j] = thePole.OverallSF;
                    bendingSFs[j] = thePole.BendingSF;
                    verticalSFs[j] = thePole.VerticalSF;
                    classs[j] = thePole.ClassString;
                    lengths[j] = thePole.LengthInInches;
                    species[j] = thePole.Species;
                    sapEquips[j] = thePole.SapEquipID.ToString();
                    globalilds[j] = thePole.GUID;
                    snowLoads[j] = thePole.SnowLoadingDistrict;
                    xvals[j] = thePole.XValue;
                    yvals[j] = thePole.YValue;
                    j++;
                }

                // create command and set properties  
                OracleCommand cmd = connection.CreateCommand();
                cmd.CommandText = "INSERT INTO " + PLDBBatchConstants.PLD_INFO_TEMP_TABLE +
                    "(" +
                        "PLDBID, PLD_STATUS, LAT, LONGITUDE, ELEVATION, HORIZONTAL_SF, " + 
                        "OVERALL_SF, BENDING_SF, VERTICAL_SF, CLASS, LENGTHININCHES, SPECIES, " + 
                        "SAPEQUIPID, GLOBALID_PLD, SNOW_LOAD_DIST, X_VALUE, Y_VALUE" + 
                    ")" +
                    " VALUES " + 
                    "(" + 
                    ":pldbid, :status, :latitude, :longitude, :elevation, :horizontalSF, :overallSF, " + 
                    ":bendingSF, :verticalSF, :classParam, :lengthParam, :speciesParam, " + 
                    ":sapEquipIdParam, :globalIdParam, :snowLoadParam, :xValParam, :yValParam" + 
                    ")";

                cmd.ArrayBindCount = this.internalStore.Count;
                cmd.Parameters.Add(":pldbid", OracleDbType.Long, this.internalStore.Select(c => c.PLDBID).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add(":status", OracleDbType.Varchar2, this.internalStore.Select(c => c.Status).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add(":latitude", OracleDbType.Double, this.internalStore.Select(c => c.Latitude).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add(":longitude", OracleDbType.Double, this.internalStore.Select(c => c.Longitude).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add(":elevation", OracleDbType.Double, this.internalStore.Select(c => c.Elevation).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add(":horizontalSF", OracleDbType.Double, horizSFs, ParameterDirection.Input);
                cmd.Parameters.Add(":overallSF", OracleDbType.Double, overallSFs, ParameterDirection.Input);
                cmd.Parameters.Add(":bendingSF", OracleDbType.Double, bendingSFs, ParameterDirection.Input);
                cmd.Parameters.Add(":verticalSF", OracleDbType.Double, verticalSFs, ParameterDirection.Input);
                cmd.Parameters.Add(":classParam", OracleDbType.Varchar2, classs, ParameterDirection.Input);
                cmd.Parameters.Add(":lengthParam", OracleDbType.Double, lengths, ParameterDirection.Input);
                cmd.Parameters.Add(":speciesParam", OracleDbType.Varchar2, species, ParameterDirection.Input);
                cmd.Parameters.Add(":sapEquipIdParam", OracleDbType.Varchar2, sapEquips, ParameterDirection.Input);
                cmd.Parameters.Add(":globalIdParam", OracleDbType.Varchar2, globalilds, ParameterDirection.Input);
                cmd.Parameters.Add(":snowLoadParam", OracleDbType.Varchar2, snowLoads, ParameterDirection.Input);
                cmd.Parameters.Add(":xValParam", OracleDbType.Double, xvals, ParameterDirection.Input);
                cmd.Parameters.Add(":yValParam", OracleDbType.Double, yvals, ParameterDirection.Input);

                cmd.ExecuteNonQuery();
            }

            // reset
            this.recordCount = 0;
        }


        //private void PopulateDataTable(Pole thePole)
        //{
        //    DataRow row = this.dataTable.NewRow();

        //    // populate the datarow with a pole
        //    //row[0] = thePole.PLDBID;
        //    //row[1] = thePole.Status;
        //    //row[2] = thePole.Latitude;
        //    //row[3] = thePole.Longitude;
        //    //row[4] = thePole.Elevation;
        //    //row[5] = thePole.YValue;
        //    //row[4] = thePole.Elevation;
        //    //row[5] = thePole.HorizontalSF;
        //    //row[6] = thePole.OverallSF;
        //    //row[7] = thePole.BendingSF;
        //    //row[8] = thePole.VerticalSF;
        //    //row[9] = thePole.ClassString;
        //    //row[10] = thePole.LengthInInches;
        //    //row[11] = thePole.Species;
        //    //row[12] = thePole.SapEquipID;
        //    //row[13] = thePole.GUID;
        //    //row[14] = thePole.SnowLoadingDistrict;
        //    //row[15] = thePole.XValue;
        //    //row[16] = thePole.YValue;

        //    // add it to the base for final addition to the DB
        //    this.dataTable.Rows.Add(row);
        //    this.recordCount++;
        //}
    }
}
