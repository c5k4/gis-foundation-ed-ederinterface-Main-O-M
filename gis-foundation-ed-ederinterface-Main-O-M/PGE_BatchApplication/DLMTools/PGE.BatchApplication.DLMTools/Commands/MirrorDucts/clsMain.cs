using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using Miner.Interop;

namespace PGE.BatchApplication.DLMTools.Commands.MirrorDucts
{
    class clsMain
    {
        private string _logFilename = "";
        private FileInfo _logfileInfo = null;
        
        public clsMain()
        {
            //Initialize the logfile 
            InitializeLogfile(); 
        } 

        /// <summary>
        /// Mirrors the Ducts in the DuctBanks. Can use just ductbanks for 
        /// selected conduitsystems or can just mirror all ductbanks depending 
        /// on repsonse from user 
        /// </summary>
        /// <param name="pApp"></param>
        public void MirrorDucts(IApplication pApp)
        {
            IWorkspaceEdit pWSE = null;
            bool hasEdits = false;
            IMMAutoUpdater autoupdater = null;
            mmAutoUpdaterMode oldMode = mmAutoUpdaterMode.mmAUMArcMap;

            try
            {
                WriteToLogfile("Entering MirrorDucts");

                //Get the workspace from the conduit system layer in the map 
                WriteToLogfile("Get the workspace from the conduit system layer in the map");
                IMap pMap = ((IMxDocument)pApp.Document).FocusMap;
                IFeatureLayer pConduitSysFL = GetFeatureLayerByName(pMap, "Conduit System");
                IFeatureClass pConduitSysFC = null;
                try { pConduitSysFC = pConduitSysFL.FeatureClass; }
                catch { };

                if (pConduitSysFC == null)
                {
                    MessageBox.Show(
                        "Unable to find the Conduit System layer in map!",
                        "Current Stored Display does not contain the Conduit System layer",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                WriteToLogfile("EDGIS.ConduitSystem featureclass found");

                //Open the required featureclasses  
                WriteToLogfile("Opening other required featureclasses...");
                IFeatureWorkspace pFWS = (IFeatureWorkspace)((IDataset)pConduitSysFC).Workspace;
                IFeatureClass pDuctBankFC = pFWS.OpenFeatureClass("edgis.ductbank");
                WriteToLogfile("Opened: edgis.ductbank");
                IFeatureClass pDuctFC = pFWS.OpenFeatureClass("edgis.duct");
                WriteToLogfile("Opened: edgis.duct");
                IFeatureClass pDuctAnnoFC = pFWS.OpenFeatureClass("edgis.ductannotation");
                WriteToLogfile("Opened: edgis.ductannotation");
                IFeatureClass pWallFC = pFWS.OpenFeatureClass("edgis.ufmwall");
                WriteToLogfile("Opened: edgis.ufmwall");
                IFeatureClass pFloorFC = pFWS.OpenFeatureClass("edgis.ufmfloor");
                WriteToLogfile("Opened: edgis.ufmfloor");

                //Check for ArcFM Session 
                WriteToLogfile("Checking for an ArcFM Session - and that the workspace is being edited");
                IVersionedWorkspace pVWS = (IVersionedWorkspace)pFWS;
                IVersion pVersion = (IVersion)pVWS;
                WriteToLogfile("Current Version: " + pVersion.VersionInfo.VersionName);

                pWSE = (IWorkspaceEdit)pFWS;
                if (!pWSE.IsBeingEdited())
                {
                    MessageBox.Show(
                        "You must first create an ArcFM Session, current workspace is not being edited!",
                        "Please Create an ArcFM Session",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                WriteToLogfile("Workspace is being edited");

                bool justSelectedConduits = false;
                DialogResult dr = MessageBox.Show(
                    "Do you want to apply updates to ducts related to just SELECTED conduits?",
                    "Just apply to selected conduits",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);
                if (dr == DialogResult.Cancel)
                    return;
                else if (dr == DialogResult.Yes)
                    justSelectedConduits = true;

                //Get the feature selection
                Hashtable hshSelectedConduits = new Hashtable();
                if (justSelectedConduits)
                {
                    WriteToLogfile("Only run selected conduits");
                    WriteToLogfile("Returning Conduit System selection");
                    IEnumFeature pEnumFeature = (IEnumFeature)pMap.FeatureSelection;
                    IEnumFeatureSetup pEnumFeatureSetup = (IEnumFeatureSetup)pEnumFeature;
                    pEnumFeatureSetup.AllFields = true;
                    IFeature pFeature = pEnumFeature.Next();
                    while (pFeature != null)
                    {
                        IDataset pDS = (IDataset)pFeature.Class;
                        if (pDS.Name.ToLower() == "edgis.conduitsystem")
                        {
                            if (!hshSelectedConduits.ContainsKey(pFeature.OID))
                                hshSelectedConduits.Add(pFeature.OID, 0);
                        }
                        pFeature = pEnumFeature.Next();
                    }
                    WriteToLogfile(hshSelectedConduits.Count.ToString() +
                        " conduit system features selected");
                    if (hshSelectedConduits.Count == 0)
                    {
                        WriteToLogfile("Exiting since no conduitsystem features are selected");
                        return;
                    }
                }

                //Search the ConduitSystem for the ones where direction is east 
                IQueryFilter pQF = new QueryFilterClass();
                IRelationshipClass pDuctBankRC = GetRelClass(pConduitSysFC, "EDGIS.DuctBank");
                int counter = 0;
                pQF.WhereClause = "DIRECTION" + " = " + "'" + "E" + "'";
                int conduitCount = pConduitSysFC.FeatureCount(pQF);
                IFeatureCursor pFCursor = pConduitSysFC.Search(pQF, false);
                IFeature pConduitSysFeature = pFCursor.NextFeature();

                //For each conduitsystem of direction east get the related duct banks 
                Hashtable hshDuctBanks = new Hashtable();
                bool includeConduit = true;

                WriteToLogfile("Finding included ConduitSystem features and related ductbanks...");
                while (pConduitSysFeature != null)
                {
                    //Screen out conduits that are not selected if necessary
                    counter++;
                    if ((counter % 100) == 0)
                        WriteToLogfile("Processing ConduitSystem OId: " +
                            counter.ToString() + " of: " + conduitCount.ToString());

                    includeConduit = true;
                    if (justSelectedConduits)
                    {
                        if (!hshSelectedConduits.ContainsKey(pConduitSysFeature.OID))
                            includeConduit = false;
                    }
                    //WriteToLogfile("Included: " + includeConduit.ToString()); 

                    //Get the related duct banks
                    if (includeConduit)
                    {
                        ISet pSet = pDuctBankRC.GetObjectsRelatedToObject(pConduitSysFeature);
                        IObject pDuctBank = (IObject)pSet.Next();
                        while (pDuctBank != null)
                        {
                            if (!hshDuctBanks.ContainsKey(pDuctBank.OID))
                                hshDuctBanks.Add(pDuctBank.OID, pConduitSysFeature.OID);
                            pDuctBank = (IObject)pSet.Next();
                        }
                        Marshal.FinalReleaseComObject(pSet);
                    }
                    pConduitSysFeature = pFCursor.NextFeature();
                }
                Marshal.FinalReleaseComObject(pFCursor);

                IFeature pWallFeature = null;
                IFeature pFloorFeature = null;
                IFeature pDuctBankFeature = null;
                IPolyline pMirrorPlane = null;

                IRgbColor pColorRed = clsGraphics.GetRGBColour(255, 0, 0);
                IRgbColor pColorGreen = clsGraphics.GetRGBColour(0, 255, 0);
                IRgbColor pColorBlue = clsGraphics.GetRGBColour(0, 0, 255);
                IRgbColor pColorBlack = clsGraphics.GetRGBColour(0, 0, 0);

                //For testing 
                IGraphicsContainer pGC = (IGraphicsContainer)pMap;
                pGC.DeleteAllElements();
                int skippedDuctBankCounter = 0;
                //hshDuctBanks = new Hashtable();
                //hshDuctBanks.Add(258867, 2167754); 

                //For every duct bank:
                counter = 0;
                bool ductsWithinDuctBankPolygon = false;
                string facilityId = string.Empty;
                int facIdFldIdx = pDuctBankFC.Fields.FindField("facilityid");
                Hashtable hshAllDucts = new Hashtable();
                foreach (int OId in hshDuctBanks.Keys)
                {
                    counter++;
                    facilityId = string.Empty;
                    WriteToLogfile(
                        "Processing DuctBank OId: " + OId.ToString() + " " +
                        counter.ToString() + " of: " + hshDuctBanks.Count.ToString());

                    //Get the duct bank feature 
                    pDuctBankFeature = pDuctBankFC.GetFeature(OId);

                    //Get the facilityId 
                    if (pDuctBankFeature.get_Value(facIdFldIdx) != DBNull.Value)
                        facilityId = pDuctBankFeature.get_Value(facIdFldIdx).ToString();
                    WriteToLogfile("facilityId: " + facilityId);

                    //Get the wall it is on 
                    WriteToLogfile("Finding wall...");
                    pWallFeature = GetWallFeature(
                        (IPolygon)pDuctBankFeature.ShapeCopy,
                        pWallFC,
                        facilityId);
                    if (pWallFeature == null)
                    {
                        skippedDuctBankCounter++;
                        WriteToLogfile("Ductbank with OId: " + pDuctBankFeature.OID.ToString() +
                            " was skipped because the WALL for the duct bank was not found!");
                        continue;
                    }

                    //Get the adjacent floor 
                    WriteToLogfile("Finding floor...");
                    pFloorFeature = GetFloorFeature(
                        (IPolygon)pWallFeature.ShapeCopy,
                        pFloorFC,
                        facilityId);
                    if (pFloorFeature == null)
                    {
                        skippedDuctBankCounter++;
                        WriteToLogfile("Ductbank with OId: " + pDuctBankFeature.OID.ToString() +
                            " was skipped because the FLOOR for the duct bank was not found!");
                        continue;
                    }
                    clsGraphics.DrawGeometry(pMap, pFloorFeature.ShapeCopy, pColorBlue, pColorBlue);

                    //Get the mirror plane 
                    WriteToLogfile("Finding Mirror Plane...");
                    pMirrorPlane = GetMirrorPlane2(
                        (IPolygon)pWallFeature.ShapeCopy,
                        (IPolygon)pFloorFeature.ShapeCopy,
                        (IPolygon)pDuctBankFeature.ShapeCopy,
                        pMap);
                    if (pMirrorPlane == null)
                    {
                        skippedDuctBankCounter++;
                        WriteToLogfile("Ductbank with OId: " + pDuctBankFeature.OID.ToString() +
                            " was skipped because the MIRROR PLANE for the duct bank was not found!");
                        continue;
                    }
                    clsGraphics.DrawPolyline(pMap, pMirrorPlane, pColorBlue, "");

                    //Get all the ducts that are on the duct bank
                    WriteToLogfile("Finding Ducts...");
                    Hashtable hshDucts = GetDucts(
                        (IPolygon)pDuctBankFeature.ShapeCopy, pDuctFC, facilityId);
                    if (hshDucts.Count == 0)
                    {
                        skippedDuctBankCounter++;
                        WriteToLogfile("Ductbank with OId: " + pDuctBankFeature.OID.ToString() +
                            " was skipped because no coincident duct features were found!");
                        continue;
                    }

                    //Foreach duct, mirror the shape about the mirror plane and store 
                    //the new duct shape 
                    WriteToLogfile("Mirroring Ducts...");
                    MirrorDuctsAboutMirrorPlane(ref hshDucts, pMirrorPlane,
                        (IPolygon)pDuctBankFeature.ShapeCopy, pMap);

                    WriteToLogfile("Checking mirrored duct polygons are within duct bank polygon...");
                    ductsWithinDuctBankPolygon = CheckDuctsWithinDuctBank(
                        hshDucts, (IPolygon)pDuctBankFeature.ShapeCopy);
                    if (!ductsWithinDuctBankPolygon)
                    {
                        skippedDuctBankCounter++;
                        WriteToLogfile("Ductbank with OId: " + pDuctBankFeature.OID.ToString() +
                            " was skipped because mirrored duct polygons are not all within boundary of duct bank!");
                        continue;
                    }

                    //Create a hashtable of all ducts for updates 
                    foreach (int ductOId in hshDucts.Keys)
                    {
                        if (!hshAllDucts.ContainsKey(ductOId))
                            hshAllDucts.Add(ductOId, hshDucts[ductOId]);
                        else
                            WriteToLogfile("Duct with OId: " + ductOId.ToString() + " appears on multiple ductbanks");
                    }
                }

                //End of loop for duct banks 

                //Perform the edits on the ducts   
                //Start an edit operation
                WriteToLogfile("Skipped ductbank count: " + skippedDuctBankCounter.ToString());


                WriteToLogfile("Saving duct polygons to their mirrored shape...");
                pWSE.StartEditOperation();
                hasEdits = true;

                //Update the shape of all of the ducts, the ductannotation 
                //should move with the duct because of the composite 
                //relationship 

                //Turn off AUs 
                object objAutoUpdater = null;
                objAutoUpdater = Activator.CreateInstance( 
                    Type.GetTypeFromProgID("mmGeodatabase.MMAutoUpdater"));
                autoupdater = (IMMAutoUpdater)objAutoUpdater;
                oldMode = autoupdater.AutoUpdaterMode;
                autoupdater.AutoUpdaterMode = mmAutoUpdaterMode.mmAUMNoEvents;
                WriteToLogfile("Switched AUs off");

                Hashtable hshAnnoOIds = new Hashtable();
                counter = 0;
                //int annoCounter = 0;
                int totalDucts = hshAllDucts.Count;
                foreach (Duct pDuct in hshAllDucts.Values)
                {
                    counter++;
                    //annoCounter = 0;

                    //IClone pClone = (IClone)pDuct.OriginalPolygon;
                    //IPolygon pNewDuctPolygon = (IPolygon)pClone.Clone();
                    //pNewDuctPolygon.SpatialReference = pDuct.OriginalPolygon.SpatialReference;

                    //Transform the duct 
                    //ITransform2D pTransform2D = (ITransform2D)pNewDuctPolygon;
                    //pTransform2D.Move(pDuct.DeltaX, pDuct.DeltaY);

                    //Save the new shape 
                    IFeature pDuctFeature = pDuctFC.GetFeature(pDuct.ObjectId);
                    pDuctFeature.Shape = pDuct.MirroredPolygon;
                    pDuctFeature.Store();

                    //Get the annotation OIds 
                    hshAnnoOIds = GetRelatedAnno(pDuct.ObjectId, pDuctAnnoFC);

                    foreach (int annoOid in hshAnnoOIds.Keys)
                    {
                        autoupdater.AutoUpdaterMode = mmAutoUpdaterMode.mmAUMArcMap;

                        IFeature pDuctAnnoFeature = pDuctAnnoFC.GetFeature(annoOid);
                        pDuctAnnoFeature.Store();

                        autoupdater.AutoUpdaterMode = mmAutoUpdaterMode.mmAUMNoEvents;
                    }

                    //foreach (int annoOId in hshAnnoOIds.Keys)
                    //{
                    //    annoCounter++;
                    //    IFeature pDuctAnnoFeature = pDuctAnnoFC.GetFeature(annoOId);

                    //    //Clone the old shape of the anno 
                    //    pClone = (IClone)pDuctAnnoFeature.ShapeCopy;
                    //    IPolygon pNewDuctAnnoPolygon = (IPolygon)pClone.Clone();
                    //    pNewDuctAnnoPolygon.SpatialReference = pDuctAnnoFeature.ShapeCopy.SpatialReference;

                    //    //Transform the anno 
                    //    pTransform2D = (ITransform2D)pNewDuctAnnoPolygon;
                    //    pTransform2D.Move(pDuct.DeltaX, pDuct.DeltaY);
                    //    pDuctAnnoFeature.Shape = pNewDuctAnnoPolygon;
                    //    pDuctAnnoFeature.Store();
                    //    WriteToLogfile("Saved related anno: " + annoCounter.ToString());
                    //}

                    WriteToLogfile("Saved: " + counter.ToString() + " of: " + totalDucts.ToString() + " ducts");
                }

                //Stop the edit operation                                   
                pWSE.StopEditOperation();
                WriteToLogfile("Leaving MirrorDucts");
            }
            catch (Exception ex)
            {
                if (hasEdits)
                {
                    pWSE.AbortEditOperation();
                    MessageBox.Show(
                        "Errors were detected in MirrorDucts! Edits have been aborted.",
                        "Mirror Ducts Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show(
                        "Errors were detected in MirrorDucts: " + ex.Message,
                        "Mirror Ducts Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            finally
            {
                if (hasEdits)
                {
                    WriteToLogfile("Turning AUs back to original state");
                    if (autoupdater != null)
                        autoupdater.AutoUpdaterMode = oldMode;
                }
            }
        }

        private Hashtable GetRelatedAnno(int ductOId, IFeatureClass pDuctAnnoFC)
        {
            try
            {
                Hashtable hshOIds = new Hashtable();
                IQueryFilter pQF = new QueryFilterClass();
                pQF.WhereClause = "FeatureId = " + ductOId.ToString();
                IFeatureCursor pFCursor = pDuctAnnoFC.Search(pQF, false);
                IFeature pFeature = pFCursor.NextFeature();
                while (pFeature != null)
                {
                    if (!hshOIds.ContainsKey(pFeature.OID))
                        hshOIds.Add(pFeature.OID, 0);
                    pFeature = pFCursor.NextFeature();
                }
                Marshal.FinalReleaseComObject(pFCursor);
                return hshOIds;
            }
            catch (Exception ex)
            {
                WriteToLogfile("Entering error handler for GetRelatedAnno: " + ex.Message);
                throw new Exception("Error returning the related anno for duct");
            }

        }

        /// <summary>
        /// Sanity check to ensure that after the mirror all ducts 
        /// are within the confines on the ductbank 
        /// </summary>
        /// <param name="hshDucts"></param>
        /// <param name="pDuctBankPolygon"></param>
        /// <returns></returns>
        private bool CheckDuctsWithinDuctBank(
            Hashtable hshDucts,
            IPolygon pDuctBankPolygon)
        {
            try
            {
                //WriteToLogfile("Entering CheckDuctsWithinDuctBank");


                //Check all mirrored ducts are within the confines of the ductbankpolygon
                IRelationalOperator pRelOp = (IRelationalOperator)pDuctBankPolygon;
                bool allWithin = true;
                foreach (int oid in hshDucts.Keys)
                {
                    Duct pDuct = (Duct)hshDucts[oid];

                    //IClone pClone = (IClone)pDuct.OriginalPolygon;
                    //IPolygon pNewDuctPolygon = (IPolygon)pClone.Clone();
                    //pNewDuctPolygon.SpatialReference = pDuct.OriginalPolygon.SpatialReference;
                    //ITransform2D pTransform2D = (ITransform2D)pNewDuctPolygon;
                    //pTransform2D.Move(pDuct.DeltaX, pDuct.DeltaY);

                    if (!pRelOp.Contains(pDuct.MirroredPolygon))
                    {
                        allWithin = false;
                        break;
                    }
                }
                if (!allWithin)
                    WriteToLogfile("Data check to see if mirrored ducts are all within ductbank failed!");

                return allWithin;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in MirrorDuctsAboutMirrorPlan: " + ex.Message);
            }
        }

        /// <summary>
        /// Returns a lit of comma separated keys to allow use in a SQL 
        /// IN() clause 
        /// </summary>
        /// <param name="hshKeys"></param>
        /// <param name="batchSize"></param>
        /// <param name="addApostrophe"></param>
        /// <returns></returns>
        private string[] GetArrayOfCommaSeparatedKeys(Hashtable hshKeys, int batchSize, bool addApostrophe)
        {
            try
            {
                Hashtable hshCommaSeparatedKeys = new Hashtable();
                int counter = 0;
                StringBuilder batchLine = new StringBuilder();

                foreach (object key in hshKeys.Keys)
                {
                    if (counter == 0)
                    {
                        if (addApostrophe)
                            batchLine.Append("'" + key.ToString() + "'");
                        else
                            batchLine.Append(key.ToString());
                    }
                    else
                    {
                        if (addApostrophe)
                            batchLine.Append("," + "'" + key.ToString() + "'");
                        else
                            batchLine.Append("," + key.ToString());
                    }

                    counter++;
                    if (counter == batchSize)
                    {
                        hshCommaSeparatedKeys.Add(batchLine.ToString(), 0);
                        batchLine = new StringBuilder();
                        counter = 0;
                    }
                }

                //Add what is left over 
                if (batchLine.ToString().Length != 0)
                    hshCommaSeparatedKeys.Add(batchLine.ToString(), 0);

                //Convert this to an array 
                counter = 0;
                string[] commaSepKeys = new string[hshCommaSeparatedKeys.Count];
                foreach (string line in hshCommaSeparatedKeys.Keys)
                {
                    commaSepKeys[counter] = line;
                    counter++;
                }

                //return array 
                return commaSepKeys;
            }
            catch (Exception ex)
            {
                WriteToLogfile("Entering error handler for " + MethodBase.GetCurrentMethod() + " error: " +
                    ex.Message);
                throw new Exception("Error returning array of comma separated keys");
            }
        }

        public void InitializeLogfile()
        {
            try
            {
                //Create a logfile
                if (_logFilename == string.Empty)
                    _logFilename = "C:\\Temp\\MirrorDucts.txt";                 
                System.IO.FileStream fs = File.Open(_logFilename,
                    FileMode.OpenOrCreate);
                fs.Close();
                if (File.Exists(_logFilename) == true)
                {
                    //Clear out the logfile 
                    File.WriteAllText(_logFilename, "");
                }
                _logfileInfo = new FileInfo(_logFilename);
            }
            catch
            {
                Debug.Print("Error initializing logfile");
            }
        }

        public void WriteToLogfile(string msg)
        {
            try
            {
                //fInfo.Create();
                using (StreamWriter sWriter = _logfileInfo.AppendText())
                {
                    sWriter.WriteLine(DateTime.Now.ToLongTimeString() + " " + msg);
                    sWriter.Close();
                }
            }
            catch
            {
                //Do nothing 
            }
        }

        /// <summary>
        /// Mirrors the shape of each duct about the passed 
        /// mirror plane line 
        /// </summary>
        /// <param name="hshDucts"></param>
        /// <param name="pMirrorPlane"></param>
        /// <param name="pDuctBankPolygon"></param>
        /// <param name="pMap"></param>
        private void MirrorDuctsAboutMirrorPlane( 
            ref Hashtable hshDucts, 
            IPolyline pMirrorPlane, 
            IPolygon pDuctBankPolygon, 
            IMap pMap)
        {
            try
            {
                //WriteToLogfile("Entering MirrorDuctsAboutMirrorPlane");

                //Get some colors for drawing 
                IRgbColor pColorRed = clsGraphics.GetRGBColour(255,0,0); 
                IRgbColor pColorGreen = clsGraphics.GetRGBColour(0,255,0);
                IRgbColor pColorBlue = clsGraphics.GetRGBColour(0,0,255);
                IRgbColor pColorBlack = clsGraphics.GetRGBColour(0, 0, 0);

                //Loop through each duct and mirror each duct about 
                //the mirror plane
                Hashtable hshDuctOIds = new Hashtable();
                foreach (int oid in hshDucts.Keys)
                {
                    hshDuctOIds.Add(oid, 0); 
                }


                foreach (int oid in hshDuctOIds.Keys)
                {
                    //Get the point on the mirror plane closest to the 
                    //centroid of the duct 
                    IPolycurve pPolycurve = (IPolycurve)pMirrorPlane;
                    Duct pDuct = (Duct)hshDucts[oid];
                    IPolygon pOrigDuctPolygon = pDuct.OriginalPolygon;                    
                    IArea pArea = (IArea)pOrigDuctPolygon;
                    IPoint pCentroid = pArea.Centroid; 
                    double distAlongCurve = -1;
                    double distFromCurve = -1; 
                    bool isRightSide = false; 
                    IPoint pMirrorIntPoint = new PointClass();
                    pPolycurve.QueryPointAndDistance(
                        esriSegmentExtension.esriNoExtension,
                        pCentroid,
                        false,
                        pMirrorIntPoint,
                        ref distAlongCurve,
                        ref distFromCurve,
                        ref isRightSide);
                    clsGraphics.DrawPoint(pMap, pMirrorIntPoint, pColorBlack, esriSimpleMarkerStyle.esriSMSX);

                    //Determine the deltaY and deltaX to use for the 
                    //2D Transformation 
                    IClone pClone = (IClone)pOrigDuctPolygon;
                    IPolygon pNewDuctPolygon = (IPolygon)pClone.Clone();
                    pNewDuctPolygon.SpatialReference = pOrigDuctPolygon.SpatialReference;

                    ITransform2D pTransform2D = (ITransform2D)pNewDuctPolygon;
                    double deltaX = (pMirrorIntPoint.X - pCentroid.X) * 2;
                    double deltaY = (pMirrorIntPoint.Y - pCentroid.Y) * 2;

                    //Apply the transformation to the original shape 
                    pTransform2D.Move(deltaX, deltaY);
                    pDuct.MirroredPolygon = pNewDuctPolygon;
                    clsGraphics.DrawGeometry(pMap, pNewDuctPolygon, pColorGreen, pColorGreen);
                    hshDucts[oid] = pDuct; 
                }
                                
                //WriteToLogfile("Leaving MirrorDuctsAboutMirrorPlane"); 
            }
            catch (Exception ex)
            {
                WriteToLogfile("Entering Error Handler for MirrorDuctsAboutMirrorPlane: " + ex.Message);
                throw new Exception("Error in MirrorDuctsAboutMirrorPlan: " + ex.Message);
            } 
        }

        /// <summary>
        /// Return a bool to indicate whether the passed version exists 
        /// </summary>
        /// <param name="pVWS"></param>
        /// <param name="versionName"></param>
        /// <returns></returns>
        private bool VersionExists(IVersionedWorkspace pVWS, string versionName)
        {
            try
            {
                bool versionExists = false;
                IVersion pVersion = pVWS.FindVersion(versionName);
                if (pVersion != null)
                    versionExists = true;
                return versionExists;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Returns a hashtable of Duct objects for the given ductbank 
        /// </summary>
        /// <param name="pDuctBankPolygon"></param>
        /// <param name="pDuctFC"></param>
        /// <param name="facilityId"></param>
        /// <returns></returns>
        private Hashtable GetDucts(IPolygon pDuctBankPolygon, IFeatureClass pDuctFC, string facilityId)
        {
            IFeatureCursor pFCursor = null; 
            try
            {
                Hashtable hshDucts = new Hashtable(); 
                ISpatialFilter pSF = new SpatialFilterClass();
                pSF.Geometry = pDuctBankPolygon;
                pSF.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                pSF.WhereClause = "facilityid" + " = " + "'" + facilityId + "'"; 

                pFCursor = pDuctFC.Search(pSF, false);
                IFeature pDuctFeature = pFCursor.NextFeature();

                while (pDuctFeature != null)
                {
                    //Add the ducts 
                    if (!hshDucts.ContainsKey(pDuctFeature.OID))
                        hshDucts.Add( 
                            pDuctFeature.OID, 
                            new Duct(pDuctFeature.OID, (IPolygon)pDuctFeature.ShapeCopy));
                    pDuctFeature = pFCursor.NextFeature(); 
                }

                return hshDucts;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetDucts: " + ex.Message);
            }
            finally
            {
                if (pFCursor != null)
                    Marshal.FinalReleaseComObject(pFCursor);
            }
        }

        private IFeature GetWallFeature(IPolygon pDuctBankPolygon, IFeatureClass pWallFC, string facilityId)
        {
            IFeatureCursor pFCursor = null; 
            try
            {
                ISpatialFilter pSF = new SpatialFilterClass();
                pSF.Geometry = pDuctBankPolygon;
                pSF.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                pSF.WhereClause = "facilityid" + " = " + "'" + facilityId + "'";  

                //there can be only one 
                int wallCount = pWallFC.FeatureCount(pSF); 
                pFCursor = pWallFC.Search(pSF, false);
                if (wallCount == 1)
                    return pFCursor.NextFeature();
                else
                {
                    WriteToLogfile("Error returning the wall - wallcount: " + 
                        wallCount.ToString()); 
                }                                
                return null; 
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetWallFeature: " + ex.Message);
            }
            finally
            {
                if (pFCursor != null) 
                    Marshal.FinalReleaseComObject(pFCursor);
            }
        }

        private IFeature GetFloorFeature(IPolygon pWallPolygon, IFeatureClass pFloorFC, string facilityId)
        {
            IFeatureCursor pFCursor = null;
            try
            {
                //The wall must intersect the floor 
                IFeature pFloorFeature = null;
                ISpatialFilter pSF = new SpatialFilterClass();
                pSF.Geometry = pWallPolygon;
                pSF.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                pSF.WhereClause = "facilityid" + " = " + "'" + facilityId + "'";  
                //there can be only one 
                pFCursor = pFloorFC.Search(pSF, false);
                pFloorFeature = pFCursor.NextFeature();
                int floorCount = pFloorFC.FeatureCount(pSF);
                if (floorCount == 1)
                {
                    //there is only one so return it as the floor                     
                    return pFloorFeature;
                }
                else
                {
                    //Check for intersection in 1 dimension (shared 
                    //segment) 
                    WriteToLogfile("Floorcount: " + floorCount.ToString());
                    ITopologicalOperator pTopo = (ITopologicalOperator)pWallPolygon;
                    while (pFloorFeature != null)
                    {
                        IGeometry pLineIntersect = pTopo.Intersect( 
                            pFloorFeature.ShapeCopy, 
                            esriGeometryDimension.esriGeometry1Dimension);
                        if (!pLineIntersect.IsEmpty)
                            return pFloorFeature; 
                        pFloorFeature = pFCursor.NextFeature();  
                    }
                }
                return pFloorFeature;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetFloorFeature: " + ex.Message);
            }
            finally
            {
                if (pFCursor != null)
                    Marshal.FinalReleaseComObject(pFCursor); 
            }
        }

        private IPolyline GetMirrorPlane2( 
            IPolygon pWallPolygon, 
            IPolygon pFloorPolygon, 
            IPolygon pDuctBankPolygon, 
            IMap pMap)
        {
            try
            {
                //Get some colors for drawing 
                IRgbColor pColorRed = clsGraphics.GetRGBColour(255,0,0); 
                IRgbColor pColorGreen = clsGraphics.GetRGBColour(0,255,0);
                IRgbColor pColorBlue = clsGraphics.GetRGBColour(0,0,255);
                IRgbColor pColorBlack = clsGraphics.GetRGBColour(0, 0, 0);

                //If the segmentcollection of the duct bank does 
                //not have 4 elements exit returning null mirror 
                //plane 
                ISegmentCollection pSegColl = (ISegmentCollection)pDuctBankPolygon;
                if (pSegColl.SegmentCount != 4)
                    return null;

                //Find the segment of the duct bank polygon 
                //where the midpoint of the segment is closest 
                //to the floor 

                Hashtable hshPointDistances = new Hashtable();                
                //IArea pFloorArea = (IArea)pFloorPolygon;
                //IProximityOperator pProxOp = (IProximityOperator)pFloorArea.Centroid; 
                ArrayList ductBankSegs = new ArrayList(); 
 
                for (int i = 0; i < pSegColl.SegmentCount; i++)
                {
                    //IGraphicsContainer pGC = (IGraphicsContainer)pMap;
                    //pGC.DeleteAllElements();

                    IPoint pSegMidPoint = GetSegmentMidPoint( 
                        pSegColl.get_Segment(i), 
                        pDuctBankPolygon.SpatialReference);
                    clsGraphics.DrawPoint(pMap, pSegMidPoint, 
                        pColorRed, esriSimpleMarkerStyle.esriSMSCross); 

                    IPolycurve pPolycurve = (IPolycurve)pFloorPolygon;
                    double distAlongCurve = -1;
                    double distFromCurve = -1;
                    bool isRightSide = false;
                    IPoint pMirrorIntPoint = new PointClass();
                    pPolycurve.QueryPointAndDistance(
                        esriSegmentExtension.esriNoExtension,
                        pSegMidPoint,
                        false,
                        pMirrorIntPoint,
                        ref distAlongCurve,
                        ref distFromCurve,
                        ref isRightSide);
                    clsGraphics.DrawPoint(pMap, pMirrorIntPoint,
                        pColorGreen, esriSimpleMarkerStyle.esriSMSCross);
                    ductBankSegs.Add(new DuctBankSegment(i, distFromCurve, pSegMidPoint));

                    //((IActiveView)pMap).Refresh(); 
                    //MessageBox.Show("Segment " + i.ToString() + " dist: " + distFromCurve.ToString()); 
                }
                
          
                //Test out the sorting 
                // Write out a header for the output.
                //System.Diagnostics.Debug.Print("Array - Unsorted\n");
                //foreach (DuctBankSegment d in ductBankSegmentArray)
                //    System.Diagnostics.Debug.Print(d.DistanceFromFloor.ToString());
                
                // Demo IComparable by sorting array with "default" sort order.
                ductBankSegs.Sort();
                int idx = 0; 
                //Create the polyine from the 2 points 
                IPolyline pPolyline = new PolylineClass(); 
                IPointCollection pPointColl = (IPointCollection)pPolyline;

                //changed to 0 and 3 
                foreach (DuctBankSegment d in ductBankSegs)
                {
                    if (idx == 0)
                        pPointColl.AddPoint(d.SegmentMidPoint);
                    else if (idx == 3)
                        pPointColl.AddPoint(d.SegmentMidPoint);
 
                    System.Diagnostics.Debug.Print(d.DistanceFromFloor.ToString());
                    idx++; 
                }
                pPolyline.SpatialReference = pDuctBankPolygon.SpatialReference;

                //for (int i = 0; i < ductBankSegs.Count; i++)
                //{
                //    System.Diagnostics.Debug.Print(i + "\t\t" + ductBankSegs[i].DistanceFromFloor);
                //}

                //Simplify the shape  
                ITopologicalOperator2 pTopo2 = (ITopologicalOperator2)pPolyline;
                pTopo2.IsKnownSimple_2 = false;
                pTopo2.Simplify();
                
                return pPolyline;
            }
            catch (Exception ex)
            {
                WriteToLogfile("Entering Error Handler for MirrorDuctsAboutMirrorPlane: " + ex.Message);
                throw new Exception("Error in GetMirrorPlane: " + ex.Message);
            }
        }

        //private IPolyline GetMirrorPlane(
        //    IPolygon pWallPolygon,
        //    IPolygon pFloorPolygon,
        //    IPolygon pDuctBankPolygon)
        //{
        //    try
        //    {

        //        //If the segmentcollection of the duct bank does 
        //        //not have 4 elements exit returning null mirror 
        //        //plane 
        //        ISegmentCollection pSegColl = (ISegmentCollection)pDuctBankPolygon;
        //        if (pSegColl.SegmentCount != 4)
        //            return null;

        //        //Find the segment of the duct bank polygon 
        //        //where the midpoint of the segment is closest 
        //        //to the floor 

        //        Hashtable hshPointDistances = new Hashtable();
        //        IArea pFloorArea = (IArea)pFloorPolygon;
        //        IProximityOperator pProxOp = (IProximityOperator)pFloorArea.Centroid;
        //        DuctBankSegment[] ductBankSegmentArray = new DuctBankSegment[4];

        //        for (int i = 0; i < pSegColl.SegmentCount; i++)
        //        {
        //            IPoint pSegMidPoint = GetSegmentMidPoint(
        //                pSegColl.get_Segment(i),
        //                pDuctBankPolygon.SpatialReference);
        //            double dist = pProxOp.ReturnDistance(pSegMidPoint);
        //            ductBankSegmentArray[i] = new DuctBankSegment(i, dist, pSegMidPoint);
        //        }

        //        //Test out the sorting 
        //        // Write out a header for the output.
        //        //System.Diagnostics.Debug.Print("Array - Unsorted\n");
        //        foreach (DuctBankSegment d in ductBankSegmentArray)
        //            System.Diagnostics.Debug.Print(d.DistanceFromFloor.ToString());

        //        // Demo IComparable by sorting array with "default" sort order.
        //        System.Array.Sort(ductBankSegmentArray);
        //        //foreach (DuctBankSegment d in ductBankSegmentArray)
        //        //    System.Diagnostics.Debug.Print(d.DistanceFromFloor.ToString());

        //        for (int i = 0; i < ductBankSegmentArray.Length; i++)
        //        {
        //            System.Diagnostics.Debug.Print(i + "\t\t" + ductBankSegmentArray[i].DistanceFromFloor);
        //        }

        //        //Create the polyine from the 2 points 
        //        IPolyline pPolyline = new PolylineClass();
        //        IPointCollection pPointColl = (IPointCollection)pPolyline;
        //        pPointColl.AddPoint(ductBankSegmentArray[1].SegmentMidPoint);
        //        pPointColl.AddPoint(ductBankSegmentArray[2].SegmentMidPoint);
        //        pPolyline.SpatialReference = pDuctBankPolygon.SpatialReference;

        //        //Simplify the shape  
        //        ITopologicalOperator2 pTopo2 = (ITopologicalOperator2)pPolyline;
        //        pTopo2.IsKnownSimple_2 = false;
        //        pTopo2.Simplify();

        //        return pPolyline;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Error in GetMirrorPlane: " + ex.Message);
        //    }
        //}

        private IPoint GetSegmentMidPoint(ISegment pSegment, ISpatialReference pSpatRef)
        {
            try
            {
                IPoint pMidPoint = new PointClass();
                pMidPoint.SpatialReference = pSpatRef;
                pMidPoint.PutCoords(
                    ((pSegment.FromPoint.X + pSegment.ToPoint.X) / 2),
                    ((pSegment.FromPoint.Y + pSegment.ToPoint.Y) / 2));
                return pMidPoint; 
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetSegmentMidPoint: " + ex.Message);
            }
        }

        private IRelationshipClass GetRelClass(IFeatureClass pFC, string otherDSName)
        {
            try
            {
                IDataset pDS = null; 
                IEnumRelationshipClass pEnumRel = pFC.get_RelationshipClasses(esriRelRole.esriRelRoleAny);
                IRelationshipClass pRelCls = pEnumRel.Next();
                while (pRelCls != null)
                {
                    pDS = (IDataset)pRelCls.OriginClass;
                    if (pDS.Name.ToLower() == otherDSName.ToLower())
                        return pRelCls; 
                    pDS = (IDataset)pRelCls.DestinationClass;
                    if (pDS.Name.ToLower() == otherDSName.ToLower())
                        return pRelCls; 

                    pRelCls = pEnumRel.Next();
                }

                return null; 
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetRelClass: " + ex.Message);
            }
        }

        /// <summary>
        /// Gets a list of Feature Layers in the given IMap instance that has been assigned the given Modelname
        /// </summary>
        /// <param name="modelName">Modelname to check. This is an object of type string.</param>
        /// <returns>A List of all featurelayers layers in the given map with the specified modelname.</returns>
        /// <remarks>Will return an Empty List if the IMap Instance is null or the Modelname passed is null</remarks>
        public IFeatureLayer GetFeatureLayerByName(IMap pMap, string layerName)
        {
            try
            {
                if (pMap.LayerCount == 0) return null;
                UID pUID = new UID(); 
                pUID.Value = "{40A9E885-5533-11d0-98BE-00805F7CED21}";
                IEnumLayer enumerator = pMap.get_Layers(pUID, true);
                ILayer layer;

                while ((layer = enumerator.Next()) != null)
                {
                    if (layer is IFeatureLayer && layer.Valid)
                    {
                        IFeatureLayer layerFilter = ((IFeatureLayer)layer);
                        if (layer.Name.ToLower() == layerName.ToLower())
                            return layerFilter; 

                    }
                }

                return null; 
            }  
            catch
            {
                throw new Exception("Unable to find the: " + layerName + " layer!"); 
            }
        }

    }
    
    public class DuctBankSegment : IComparable
    {

        private int _segmentIndex;
        private double _distanceFromFloor;
        private IPoint _midPoint;

        public DuctBankSegment(int segmentIndex, double distFromFloor , IPoint midPoint)
        {
            _segmentIndex = segmentIndex;
            _distanceFromFloor = distFromFloor;
            _midPoint = midPoint; 
        }

        public int SegmentIndex
        {
            get { return _segmentIndex; }
            set { _segmentIndex = value; }
        }

        public double DistanceFromFloor
        {
            get { return _distanceFromFloor; }
            set { _distanceFromFloor = value; }
        }

        public IPoint SegmentMidPoint
        {
            get { return _midPoint; }
            set { _midPoint = value; }
        }

        // Implement IComparable CompareTo to provide default sort order.
        int IComparable.CompareTo(object obj)
        {
            if (obj == null) return 1;
            DuctBankSegment otherDBS = obj as DuctBankSegment;
            if (otherDBS != null)
                return this.DistanceFromFloor.CompareTo(otherDBS.DistanceFromFloor);
            else
                throw new ArgumentException("Object is not a DuctBankSegment");
        }
    }

    public class Duct
    {
        private int _objectId;
        //private double _deltaX;
        //private double _deltaY;
        private IPolygon _pOriginalPolygon;
        private IPolygon _pMirroredPolygon;

        public Duct(int objectId, IPolygon pOriginalPolygon)
        {
            _objectId = objectId;
            _pOriginalPolygon = pOriginalPolygon;
        }

        public int ObjectId
        {
            get { return _objectId; }
            set { _objectId = value; }
        }

        public IPolygon OriginalPolygon
        {
            get { return _pOriginalPolygon; }
            set { _pOriginalPolygon = value; }
        }

        //public double DeltaX
        //{
        //    get { return _deltaX; }
        //    set { _deltaX = value; }
        //}

        //public double DeltaY
        //{
        //    get { return _deltaY; }
        //    set { _deltaY = value; }
        //}

        public IPolygon MirroredPolygon
        {
            get { return _pMirroredPolygon; }
            set { _pMirroredPolygon = value; }
        }
    }
}
