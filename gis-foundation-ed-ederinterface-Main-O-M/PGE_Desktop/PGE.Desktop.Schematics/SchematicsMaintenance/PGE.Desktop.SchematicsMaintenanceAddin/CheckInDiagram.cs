// Copyright 2013 ESRI
// 
// All rights reserved under the copyright laws of the United States
// and applicable international laws, treaties, and conventions.
// 
// You may freely redistribute and use this sample code, with or
// without modification, provided you include the original copyright
// notice and use restrictions.
// 
// See the use restrictions at http://resources.arcgis.com/en/help/arcobjects-net/conceptualhelp/index.html#/Copyright_information/00010000009s000000/
// 

using System;
using System.Collections.Generic;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Schematic;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.SchematicUI;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.SchematicControls;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;
using PGE.Desktop.SchematicsMaintenance.Configuration;
using PGE.Desktop.SchematicsMaintenance.Core;
using PGE.Desktop.SchematicsMaintenance.UI.Extensions;

namespace PGE.Desktop.SchematicsMaintenance
{
    public class CheckInDiagram : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        private PGESchematicsMaintenanceExtension m_maintenanceExt;


        private ISchematicTarget m_schematicTarget = null;
        private SchematicExtension m_schematicExtension = null;
        private FrmCheckInProperties m_Form = null;
        private bool m_bFailedUpdate;

        public CheckInDiagram()
        {
        }

        ~CheckInDiagram()
        {
            m_schematicTarget = null;
            m_schematicExtension = null;
            m_Form = null;
        }

        private void ArchiveFeatures(IFeatureClass fromTable, IFeatureClass toTable, string where, bool deleteSource)
        {
            if (fromTable == null || toTable == null)
                return;
            IFeatureCursor cursor = null;

            try
            {
                cursor = fromTable.Search(new QueryFilterClass() { SubFields = "*", WhereClause = where }, false);

                IFeature feature = cursor.NextFeature();
                int featureCount = 0;
                while (feature != null)
                {
                    featureCount++;
                    IFeature archFeature = toTable.CreateFeature();

                    for (int i = 0; i < archFeature.Fields.FieldCount; i++)
                    {
                        string fieldName = archFeature.Fields.Field[i].Name;
                        esriFieldType type = archFeature.Fields.Field[i].Type;
                        if (type != esriFieldType.esriFieldTypeGlobalID &&
                            type != esriFieldType.esriFieldTypeOID &&
                            type != esriFieldType.esriFieldTypeGeometry &&
                            !fieldName.Contains("SHAPE") &&
                            feature.Fields.FindField(fieldName) >= 0)
                        {
                            archFeature.Value[i] = feature.Value[feature.Fields.FindField(fieldName)];
                        }
                    }
                    archFeature.Shape = feature.ShapeCopy;
                    //store archive feature
                    archFeature.Store();

                    //delete source feature
                    if (deleteSource)
                        feature.Delete();

                    feature = cursor.NextFeature();
                }
                Logger.Log.InfoFormat("{0} {1} features archived", featureCount, fromTable.AliasName);
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(cursor);
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
                if (cursor != null)
                {
                    try
                    {
                        System.Runtime.InteropServices.Marshal.FinalReleaseComObject(cursor);
                    }
                    catch
                    {
                    }
                }
                throw;
            }
        }

        private void ArchiveRows(ITable fromTable, ITable toTable, string where, bool deleteSource)
        {
            ICursor cursor = fromTable.Search(new QueryFilterClass() { SubFields = "*", WhereClause = where }, false);

            IRow row = cursor.NextRow();

            while (row != null)
            {
                IRow archRow = toTable.CreateRow();

                for (int i = 0; i < archRow.Fields.FieldCount; i++)
                {
                    string fieldName = archRow.Fields.Field[i].Name;
                    esriFieldType type = archRow.Fields.Field[i].Type;
                    if (type != esriFieldType.esriFieldTypeGlobalID &&
                        type != esriFieldType.esriFieldTypeOID &&
                        row.Fields.FindField(fieldName) >= 0)
                    {
                        archRow.Value[i] = row.Value[row.Fields.FindField(fieldName)];
                    }
                }
                //store archive row
                archRow.Store();

                //delete source row
                if (deleteSource)
                    row.Delete();

                row = cursor.NextRow();
            }

            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(cursor);
        }

        private int DeleteRows(ITable table, string where)
        {
            if (table == null)
                return 0;

            ICursor cursor = null;
            int deleteCount = 0;
            try
            {
                cursor = table.Search(new QueryFilterClass() { SubFields = "*", WhereClause = where }, false);

                IRow row = cursor.NextRow();

                while (row != null)
                {
                    deleteCount++;
                    row.Delete();

                    row = cursor.NextRow();
                }

                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(cursor);
            }
            catch (Exception)
            {
                if (cursor != null)
                {
                    try
                    {
                        System.Runtime.InteropServices.Marshal.FinalReleaseComObject(cursor);
                    }
                    catch
                    {
                    }
                }
                throw;
            }

            return deleteCount;
        }

        private void ArchivePostedSession(string sessionID, ITable postedSessions, ITable postedSessionsArch)
        {
            //Archive Posted Session table
            ICursor cursor = null;
            try
            {
                

                if (postedSessions == null)
                    return;

                //source fields
                int sourceSESSIONID = postedSessions.FindField("SESSIONID");
                int sourceREGION = postedSessions.FindField("REGION");
                int sourceDISTRICT = postedSessions.FindField("DISTRICT");
                int sourceDIVISION = postedSessions.FindField("DIVISION");
                int sourceLOCALOFFICE = postedSessions.FindField("LOCALOFFICE");
                int sourceLANID = postedSessions.FindField("LANID");
                int sourceSESSIONDATE = postedSessions.FindField("SESSIONDATE");
                int sourcePMORDERNUMBER = postedSessions.FindField("PMORDERNUMBER");

                
                if (postedSessionsArch == null)
                    return;

                //archive fields
                int archSESSIONID = postedSessionsArch.FindField("SESSIONID");
                int archREGION = postedSessionsArch.FindField("REGION");
                int archDISTRICT = postedSessionsArch.FindField("DISTRICT");
                int archDIVISION = postedSessionsArch.FindField("DIVISION");
                int archLOCALOFFICE = postedSessionsArch.FindField("LOCALOFFICE");
                int archSESSIONEDITORLANID = postedSessionsArch.FindField("SESSIONEDITORLANID");
                int archSESSIONPOSTDATE = postedSessionsArch.FindField("SESSIONPOSTDATE");
                int archPMORDERNUMBER = postedSessionsArch.FindField("PMORDERNUMBER");
                int archSCHEMATICSCHECKINDATE = postedSessionsArch.FindField("SCHEMATICSCHECKINDATE");
                int archSCHEMATICSEDITORLANID = postedSessionsArch.FindField("SCHEMATICSEDITORLANID");

                cursor =
                    postedSessions.Search(
                        new QueryFilterClass()
                            {
                                SubFields = "*",
                                WhereClause = string.Format("{0} = '{1}'", "SESSIONID", sessionID)
                            }, false);

                IRow row = cursor.NextRow();
                while (row != null)
                {
                    IRow archiveRow = postedSessionsArch.CreateRow();

                    archiveRow.Value[archSESSIONID] = row.Value[sourceSESSIONID];
                    archiveRow.Value[archREGION] = row.Value[sourceREGION];
                    archiveRow.Value[archDISTRICT] = row.Value[sourceDISTRICT];
                    archiveRow.Value[archDIVISION] = row.Value[sourceDIVISION];
                    archiveRow.Value[archLOCALOFFICE] = row.Value[sourceLOCALOFFICE];
                    archiveRow.Value[archSESSIONEDITORLANID] = row.Value[sourceLANID];
                    archiveRow.Value[archSESSIONPOSTDATE] = row.Value[sourceSESSIONDATE];
                    archiveRow.Value[archPMORDERNUMBER] = row.Value[sourcePMORDERNUMBER];
                    archiveRow.Value[archSCHEMATICSCHECKINDATE] = DateTime.UtcNow;
                    archiveRow.Value[archSCHEMATICSEDITORLANID] = Environment.UserName.Length > 8 ? Environment.UserName.Substring(0, 8) : Environment.UserName;

                    archiveRow.Store();

                    row.Delete();
                    row = cursor.NextRow();
                }
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(cursor);
            }
            catch (Exception)
            {
                if (cursor != null)
                {
                    try
                    {
                        System.Runtime.InteropServices.Marshal.FinalReleaseComObject(cursor);
                    }
                    catch
                    {
                    }
                }
                throw;
            }
        }

        private void ArchiveGridChanges(string sessionid, List<string> gridChanges, ITable revisionQueue, ITable gridChangesTable)
        {
            ICursor cursor = null;
            try
            {
                List<string> grids = new List<string>();
                List<string> allGrids = new List<string>();

                grids.AddRange(gridChanges);
                


                //if there is a sessionid, get grids listed in revision  queue table
                if (!string.IsNullOrEmpty(sessionid))
                {

                   
                    //source fields
                    int sourceGRIDCELLID = revisionQueue.FindField("GRIDCELLID");

                    //get grid ids from revision queue table
                    cursor =
                        revisionQueue.Search(
                            new QueryFilterClass()
                                {
                                    SubFields = "*",
                                    WhereClause = string.Format("{0} = '{1}'", "SESSIONID", sessionid)
                                }, false);

                    IRow row = cursor.NextRow();
                    while (row != null)
                    {
                        if (!grids.Contains(Convert.ToString(row.Value[sourceGRIDCELLID])))
                        {
                            grids.Add(Convert.ToString(row.Value[sourceGRIDCELLID]));
                        }
                       
                        row = cursor.NextRow();
                    }
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(cursor);
                }

                //log all modified grids
                

                //archive fields
                int archGRIDID = gridChangesTable.FindField("GRIDID");

              
                //get all grid id's currently in the table
                ICursor gridCursor = gridChangesTable.Search(new QueryFilterClass(), true);
                IRow gridRow = gridCursor.NextRow();
                while (gridRow != null)
                {
                    if (!allGrids.Contains(Convert.ToString(gridRow.Value[archGRIDID])))
                    {
                        allGrids.Add(Convert.ToString(gridRow.Value[archGRIDID]));
                    }

                    gridRow = gridCursor.NextRow();
                }
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(gridCursor);


                //log each grid
                foreach (var grid in grids)
                {
                    if (!allGrids.Contains(grid))
                    {
                        IRow archiveRow = gridChangesTable.CreateRow();

                        archiveRow.Value[archGRIDID] = grid;
                        archiveRow.Store();
                    }
                }



                if (!string.IsNullOrEmpty(sessionid))
                {
                    //delete rows from revisionQueue after we've successfully logged changed grids
                    revisionQueue.DeleteSearchedRows(new QueryFilterClass()
                        {
                            SubFields = "*",
                            WhereClause = string.Format("{0} = '{1}'", "SESSIONID", sessionid)
                        });
                }

                

            }
            catch (Exception)
            {
                if (cursor != null)
                {
                    try
                    {
                        System.Runtime.InteropServices.Marshal.FinalReleaseComObject(cursor);
                    }
                    catch
                    {
                    }
                }

                throw;
            }
        }

        protected override void OnClick()
        {
            if (m_schematicTarget == null)
            {
                OnUpdate();
                if (!Enabled) return;
            }
            string childDiagramName = "";
            ArcMap.Application.CurrentTool = null;
            try
            {
                ISchematicLayer schChildLayer = m_schematicTarget.SchematicTarget;
                ISchematicDiagram schChildDiagram = schChildLayer.SchematicDiagram;
                childDiagramName = schChildDiagram.Name;
        

                bool bUpdate = false; // m_Form.UpdateDiagrams;
                bool bDelete =
                    m_maintenanceExt.Configuration.DeleteDiagramOnCheckIn[schChildDiagram.SchematicDiagramClass.Name];
              
                m_bFailedUpdate = false;

                // Save edits if needed
                if (schChildLayer.IsEditingSchematicDiagram())
                    schChildLayer.SchematicInMemoryDiagram.Save((ILayer)schChildLayer);
                else
                    schChildLayer.StartEditSchematicDiagram(false);

                if (schChildLayer.SchematicInMemoryDiagram == null ||
                    schChildLayer.SchematicInMemoryDiagram.SchematicInMemoryFeatures == null)
                {
                    MessageBox.Show(
                        "The diagram being edited cannot be checked in.  Unable to load In-Memory Features.  Please try again.", "Check In", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string sMasterIds = schChildDiagram.get_Value(schChildDiagram.Fields.FindField("MASTERIDS")).ToString();
                List<string> changedGrids = UpdateMastersDiagram(sMasterIds.Split(','), schChildLayer, bUpdate);

                if (!m_bFailedUpdate) // update failed, keep child diagram in edit mode
                {
                    //close out this edit session...

                    IFeatureClass sipFeatureClass =
                        m_maintenanceExt.Configuration.SchematicConfigWorkspace.OpenFeatureClass(
                            GetQualifiedName((IWorkspace)m_maintenanceExt.Configuration.SchematicConfigWorkspace,
                                             SchematicsMaintenanceConfiguration.EDSCHEM_Owner,
                                             SchematicsMaintenanceConfiguration.EDSCHEM_InProgressPolygon));


                    IFeatureClass sipFeatureClassArch =
                        m_maintenanceExt.Configuration.SchematicConfigWorkspace.OpenFeatureClass(
                            GetQualifiedName((IWorkspace)m_maintenanceExt.Configuration.SchematicConfigWorkspace,
                                             SchematicsMaintenanceConfiguration.EDSCHEM_Owner,
                                             SchematicsMaintenanceConfiguration.EDSCHEM_InProgressPolygonArchive));


                    ITable revisionQueue =
                        m_maintenanceExt.Configuration.SchematicConfigWorkspace.OpenTable(
                            GetQualifiedName((IWorkspace)m_maintenanceExt.Configuration.SchematicConfigWorkspace,
                                        SchematicsMaintenanceConfiguration.EDSCHEM_Owner,
                                        SchematicsMaintenanceConfiguration.EDSCHEM_RevisionQueue));

                    ITable gridChangesTable =
                        m_maintenanceExt.Configuration.SchematicConfigWorkspace.OpenTable(
                            GetQualifiedName((IWorkspace)m_maintenanceExt.Configuration.SchematicConfigWorkspace,
                                      SchematicsMaintenanceConfiguration.EDSCHEM_Owner,
                                      SchematicsMaintenanceConfiguration.EDSCHEM_GridChanges));

                    //Find related SUP and SIP
                    //Pull SessionID from Prefix of diagram name.  If there is no session id, query Schematic In Progress fc by SchematicEditorID where sessionID is null
                    int index = schChildDiagram.Name.IndexOf("_Edit_");
                    string sessionID = null;
                   
                    //ArcMap.Application.StatusBar.Message[0] = "Archiving session information";
                    Application.DoEvents();

                    ArcMap.Application.StatusBar.Message[0] = "Opening the required layers and tables for archiving";
                   
                    //edit.StartEditing(true);
                    //edit.StartEditOperation();
                    if (index > 0)
                    {
                        sessionID = schChildDiagram.Name.Substring(0, index);
                        
                        IFeatureClass supFeatureClass =
                           m_maintenanceExt.Configuration.SchematicConfigWorkspace.OpenFeatureClass(
                               GetQualifiedName(
                                   (IWorkspace)m_maintenanceExt.Configuration.SchematicConfigWorkspace,
                                   SchematicsMaintenanceConfiguration.EDSCHEM_Owner,
                                   SchematicsMaintenanceConfiguration.EDSCHEM_UpdatePolygon));
                        IFeatureClass supFeatureClassArch =
                            m_maintenanceExt.Configuration.SchematicConfigWorkspace.OpenFeatureClass(
                                GetQualifiedName(
                                    (IWorkspace)m_maintenanceExt.Configuration.SchematicConfigWorkspace,
                                    SchematicsMaintenanceConfiguration.EDSCHEM_Owner,
                                    SchematicsMaintenanceConfiguration.EDSCHEM_UpdatePolygonArchive));

                        ITable ederChangeSetLine = m_maintenanceExt.Configuration.SchematicConfigWorkspace.OpenFeatureClass(
                                GetQualifiedName(
                                    (IWorkspace)m_maintenanceExt.Configuration.SchematicConfigWorkspace,
                                    SchematicsMaintenanceConfiguration.EDSCHEM_Owner,
                                    SchematicsMaintenanceConfiguration.EDSCHEM_EDERChangeSetLine)) as ITable;

                        ITable ederChangeSetPoint = m_maintenanceExt.Configuration.SchematicConfigWorkspace.OpenFeatureClass(
                                GetQualifiedName(
                                    (IWorkspace)m_maintenanceExt.Configuration.SchematicConfigWorkspace,
                                    SchematicsMaintenanceConfiguration.EDSCHEM_Owner,
                                    SchematicsMaintenanceConfiguration.EDSCHEM_EDERChangeSetPoint)) as ITable;


                        ITable postedSessions =
                            m_maintenanceExt.Configuration.SchematicConfigWorkspace.OpenTable(
                                GetQualifiedName((IWorkspace)m_maintenanceExt.Configuration.SchematicConfigWorkspace,
                                         SchematicsMaintenanceConfiguration.EDSCHEM_Owner,
                                         SchematicsMaintenanceConfiguration.EDSCHEM_PostedSession));

                        ITable postedSessionsArch =
                            m_maintenanceExt.Configuration.SchematicConfigWorkspace.OpenTable(
                                GetQualifiedName((IWorkspace)m_maintenanceExt.Configuration.SchematicConfigWorkspace,
                                        SchematicsMaintenanceConfiguration.EDSCHEM_Owner,
                                        SchematicsMaintenanceConfiguration.EDSCHEM_PostedSessionArch));
  
                        IWorkspaceEdit edit = m_maintenanceExt.Configuration.SchematicConfigWorkspace as IWorkspaceEdit;



                        try
                        {
                            //April 2019 release - Added MultiuserEditing to resolve database redefined state errors in checkin/checkout when multiple users are editing simultaneously
                            for (int i = 0; i < 5; i++)
                            {
                                if (edit.IsBeingEdited())
                                {
                                    ArcMap.Application.StatusBar.Message[0] = "Archiving Session information - " + i.ToString();
                                    System.Threading.Thread.Sleep(2000);
                                }
                                else
                                {
                                    break;
                                }
                            }
                            ArcMap.Application.StatusBar.Message[0] = "Start archiving Session information";
                            ((IMultiuserWorkspaceEdit)edit).StartMultiuserEditing(esriMultiuserEditSessionMode.esriMESMNonVersioned);


                            //Archive SIP
                            ArcMap.Application.StatusBar.Message[0] = "Archiving SIP";
                            ArchiveFeatures(sipFeatureClass, sipFeatureClassArch,
                                            string.Format("{0} = '{1}'", "SESSIONID", sessionID), true);

                            //ArchiveSUP
                            ArcMap.Application.StatusBar.Message[0] = "Archiving SUP";
                            ArchiveFeatures(supFeatureClass, supFeatureClassArch,
                                            string.Format("{0} = '{1}'", "SESSIONID", sessionID), true);

                            //delete EDER ChangeSet features
                            ArcMap.Application.StatusBar.Message[0] = "Deleting EDER ChangeSets";
                            DeleteRows(ederChangeSetLine,
                                string.Format("{0} = '{1}'", "SESSIONID", sessionID));
                            DeleteRows(ederChangeSetPoint,
                                string.Format("{0} = '{1}'", "SESSIONID", sessionID));

                            //Archive the Posted Session table by sessionID
                            ArcMap.Application.StatusBar.Message[0] = "Archiving posted session";
                            ArchivePostedSession(sessionID, postedSessions, postedSessionsArch);

                            //log grid changes for Schematic Change Detection: write grid ids to gridchanges table
                            ArcMap.Application.StatusBar.Message[0] = "Archiving grid changes";
                            ArchiveGridChanges(sessionID, changedGrids, revisionQueue,gridChangesTable);

                            ArcMap.Application.StatusBar.Message[0] = "Saving Archive details";
                            //edit.StopEditOperation();
                            edit.StopEditing(true);
                        }
                        catch (Exception e)
                        {
                            Logger.Log.Error("Archiving session info failed", e);
                            try
                            {
                                if (edit != null && edit.IsBeingEdited())
                                {
                                    //edit.AbortEditOperation();
                                    edit.StopEditing(false);
                                }
                            }
                            catch (Exception)
                            {
                            }
                            MessageBox.Show(
                                "The master diagram(s) have been updated, however the tool was unable to archive all the required session information.  Please retry the check-in operation." + e.Message.ToString());
                            ArcMap.Application.StatusBar.Message[0] = "Check-in failed";
                            Application.DoEvents();
                            schChildDiagram = null;
                            // Cleanup
                            m_schematicTarget = null;
                            m_schematicExtension = null;
                            GC.Collect();
                            return;
                        }
                    }
                    else
                    {
                        //no session id.  Just archive SIP
                        //query sip table by lan id and session id = ""
                        IWorkspaceEdit edit = m_maintenanceExt.Configuration.SchematicConfigWorkspace as IWorkspaceEdit;

                        try
                        {
                            //April 2019 release - Added MultiuserEditing to resolve database redefined state errors in checkin/checkout when multiple users are editing simultaneously
                            for (int i = 0; i < 5; i++)
                            {
                                if (edit.IsBeingEdited())
                                {
                                    ArcMap.Application.StatusBar.Message[0] = "Archiving Session information - " + i.ToString();
                                    System.Threading.Thread.Sleep(2000);
                                }
                                else
                                {
                                    break;
                                }
                            }
                            ArcMap.Application.StatusBar.Message[0] = "Start archiving Session information";
                            ((IMultiuserWorkspaceEdit)edit).StartMultiuserEditing(esriMultiuserEditSessionMode.esriMESMNonVersioned);


                            //Archive SIP
                            ArcMap.Application.StatusBar.Message[0] = "Archiving SIP";
                            ArchiveFeatures(sipFeatureClass, sipFeatureClassArch,
                                            string.Format("{0} = '{1}' AND {2} is NULL", "SCHEMATICEDITORID",
                                                            Environment.UserName.Length > 8 ? Environment.UserName.Substring(0, 8) : Environment.UserName, "SESSIONID"), true);

                            //log grid changes for Schematic Change Detection
                            ArcMap.Application.StatusBar.Message[0] = "Archiving grid changes";
                            ArchiveGridChanges("", changedGrids, revisionQueue, gridChangesTable);

                            ArcMap.Application.StatusBar.Message[0] = "Saving Archive details";
                            //edit.StopEditOperation();
                            edit.StopEditing(true);
                        }
                        catch (Exception e)
                        {
                            Logger.Log.Error("Archiving session info failed", e);
                            try
                            {
                                if (edit != null && edit.IsBeingEdited())
                                {
                                    //edit.AbortEditOperation();
                                    edit.StopEditing(false);
                                }
                            }
                            catch (Exception)
                            {
                            }
                            MessageBox.Show(
                                "The master diagram(s) have been updated, however the tool was unable to archive the required session information.  Please retry the check-in operation." + e.Message.ToString());
                            ArcMap.Application.StatusBar.Message[0] = "Check-in failed";
                            Application.DoEvents();
                            schChildDiagram = null;
                            // Cleanup
                            m_schematicTarget = null;
                            m_schematicExtension = null;
                            GC.Collect();
                            return;
                        }

                    }

                    ArcMap.Application.StatusBar.Message[0] = "Updating map document";
                    Application.DoEvents();

                    if (schChildLayer.IsEditingSchematicDiagram())
                        schChildLayer.StopEditSchematicDiagram();

                    // Remove child diagram map
                    IMap esrChildMap = GetDiagramMap(schChildDiagram.Name);
                    ArcMap.Document.Maps.Remove(esrChildMap);
                    ArcMap.Document.UpdateContents();

                    if (bDelete) // Delete the child diagram from database, if required
                    {
                        ((IDataset)schChildDiagram).Delete();
                        ArcMap.Application.StatusBar.Message[0] = string.Format("Deleting Child Diagram {0}",
                                                                                childDiagramName);
                        Application.DoEvents();
                    }

                    ArcMap.Application.StatusBar.Message[0] = string.Format("Diagram {0} successfully checked-in",
                                                                            childDiagramName);
                    Application.DoEvents();
                }
                else
                {
                    ArcMap.Application.StatusBar.Message[0] = "Check-in failed";
                    Application.DoEvents();
                }

                IMap ipActiveMap = ArcMap.Document.ActiveView as IMap;

                if (Utils.IsSchematicMap(ipActiveMap, false))
                    ipActiveMap.ClearSelection();

                schChildDiagram = null;
            }
            catch (Exception e)
            {
                MessageBox.Show(
                    "An error occurred during the Check-in.  The current map document may not be configured properly");

                Logger.Log.Error(e);
                ArcMap.Application.StatusBar.Message[0] = "Check-in failed";
                Application.DoEvents();
            }


            // Cleanup
            m_schematicTarget = null;
            m_schematicExtension = null;

            GC.Collect();
        }

        protected override void OnUpdate()
        {
            if (m_schematicExtension == null)
                m_schematicExtension = Utils.GetSchematicExtension();
            if (m_maintenanceExt == null)
                m_maintenanceExt = PGESchematicsMaintenanceExtension.GetExtension();

            if (m_schematicExtension == null)
            {
                Enabled = false;
                return;
            }

            if (m_maintenanceExt == null)
            {
                Enabled = false;
                return;
            }

            // Must be always connected to the good schematic layer
            m_schematicTarget = m_schematicExtension as ISchematicTarget;

            if (m_schematicTarget != null)
            {
                ISchematicLayer schLayer = m_schematicTarget.SchematicTarget;
                if (schLayer != null && schLayer.IsEditingSchematicDiagram())
                {
                    ISchematicDiagram schDiag = schLayer.SchematicDiagram;
                    int iField = schDiag.Fields.FindField("MASTERIDS");

                    if (iField >= 0 && schDiag.get_Value(iField).ToString() != "")
                    {
                        Enabled = true;
                        return;
                    }
                }
            }
            Enabled = false;
        }


        /// <summary>
        /// Updates the Parent diagram(s) with any edits made to the child diagram
        /// Also performs archiving operations on certain work tables.
        /// </summary>
        /// <param name="tabMasterIds"></param>
        /// <param name="schemChildLayer"></param>
        /// <param name="WithUpdate">Forces a full update of the Parent diagram before merging edits from the child diagram.  It should not be required in the PGE workflow.</param>
        private List<string> UpdateMastersDiagram(string[] tabMasterIds, ISchematicLayer schemChildLayer, bool WithUpdate)
        {
            ISchematicDiagram schChildDiagram = schemChildLayer.SchematicDiagram;
            ISchematicDiagramClass schChildDiagramClass = schChildDiagram.SchematicDiagramClass;


            if (m_maintenanceExt.Configuration == null || !m_maintenanceExt.Configuration.IsInitialized)
            {
                MessageBox.Show(
                    "One or more Schematics Maintenance Configuration tables is missing from the current Dataframe.  Unable to Check-in diagram,");
                m_bFailedUpdate = true;
                return new List<string>();
            }
            //Get Parent 


            string sDiagClassName = m_maintenanceExt.Configuration.ParentChildConfig[schChildDiagramClass.Name];

            //strip prefix, if used
            //string sDiagClassName = string.IsNullOrEmpty(Utils.cstPrefix_ChildDiagramTemplate)
            //                            ? schChildDiagramClass.Name
            //                            : schChildDiagramClass.Name.Substring(
            //                                Utils.cstPrefix_ChildDiagramTemplate.Length);
            ////strip suffix, if used
            //sDiagClassName = string.IsNullOrEmpty(Utils.cstSuffix_ChildDiagramTemplate)
            //                     ? sDiagClassName
            //                     : sDiagClassName.Substring(0,
            //                                                sDiagClassName.Length -
            //                                                Utils.cstSuffix_ChildDiagramTemplate.Length);

            ISchematicDiagramClassContainer schDiagClassCont =
                schChildDiagramClass.SchematicDataset as ISchematicDiagramClassContainer;
            ISchematicDiagramClass schDiagramClass = schDiagClassCont.GetSchematicDiagramClass(sDiagClassName);

            ISchematicDiagram schParentDiagram = null;
            ISchematicInMemoryDiagram schImParentDiagram;

            ISchematicBuilder schBuilder = schDiagramClass as ISchematicBuilder;
            List<string> listFailed = new List<string>();
            List<string> circuitChanges = new List<string>();
            List<string> changedGrids = new List<string>();
            bool diagramLocked = false;
            foreach (string s in tabMasterIds)
            {
                try
                {
                    int idDiag = Convert.ToInt32(s);

                    ISchematicLayer schLayer;
                    schParentDiagram = GetParentDiagram(schDiagramClass, idDiag, out schLayer);

                    if (IsUnlocked(schParentDiagram, schParentDiagram.Name))
                    {
                        changedGrids.Add(schParentDiagram.Name);

                        ArcMap.Application.StatusBar.Message[0] =
                            string.Format("Updating parent diagram {0}", schParentDiagram.Name);
                        Application.DoEvents();
                        if (WithUpdate)
                        {
                            // Update the master
                            IPropertySet esrProperty = new PropertySet();
                            esrProperty.SetProperty("UPDATETYPE",
                                                    esriSchematicStandardBuilderUpdateType
                                                        .esriSchematicUpdateFromInitial);
                            esrProperty.SetProperty("UPDATEMODE",
                                                    esriSchematicStandardBuilderUpdateMode.esriSchematicRebuildMode);
                            esrProperty.SetProperty("PERSISTREMOVEDELEMENTS", false);

                            // try to update diagram, failed if it is locked
                            try
                            {
                                schBuilder.UpdateDiagram(schParentDiagram, esrProperty, null, null);
                            }
                            catch
                            {
                                // the diagram is lock, cancel deleting child diagram, and add to the list of locked diagrams
                                m_bFailedUpdate = true;
                                listFailed.Add(schParentDiagram.Name);
                                continue;
                            }
                        }



                        schImParentDiagram = schDiagramClass.LoadSchematicInMemoryDiagram(schParentDiagram.Name);

                        bool updateParentCrossings;
                        // Copy layout to the master
                        circuitChanges.AddRange(CopyLayoutToParent(schemChildLayer.SchematicInMemoryDiagram,
                                                                   schImParentDiagram, out updateParentCrossings));

                        if (updateParentCrossings)
                        {
                            //if we need to update parent crossing, this will require starting an edit session on the diagram.

                            try
                            {

                                schLayer.StartEditSchematicDiagram(true);
                                ArcMap.Application.StatusBar.Message[0] =
                                    string.Format("Updating Crossing Marks in parent diagram {0}", schParentDiagram.Name);
                                Application.DoEvents();
                                IEnumSchematicAlgorithm enumAlgo =
                                    schLayer.SchematicInMemoryDiagram.SchematicDiagramClass.SchematicAlgorithms;


                                ISchematicAlgoMarkCrossings markCrossings = null;

                                enumAlgo.Reset();
                                ISchematicAlgorithm algo = enumAlgo.Next();

                                while (algo != null)
                                {
                                    if (algo is ISchematicAlgoMarkCrossings)
                                    {
                                        markCrossings = algo as ISchematicAlgoMarkCrossings;
                                        break;
                                    }
                                    algo = enumAlgo.Next();
                                }

                                IPropertySet props = markCrossings.PropertySet;


                                markCrossings.Execute(schLayer, null);


                                schLayer.StopEditSchematicDiagram();
                            }
                            catch (Exception e)
                            {

                                Logger.Log.Error(e);
                                MessageBox.Show(
                                    string.Format(
                                        "Unable to update Crossing Marks in diagram {0}, please try the check in again.\nError Message: {1}",
                                        schParentDiagram.Name, e.Message));

                                m_bFailedUpdate = true;
                                listFailed.Add(schParentDiagram.Name);
                                if (schLayer.IsEditingSchematicDiagram())
                                    schLayer.StopEditSchematicDiagram();
                                continue;

                            }
                        }

                        // Update Containers
                        ISchematicRelationController schRelCont = new SchematicRelationController();
                        IEnumSchematicInMemoryFeature enuFeatures = schRelCont.FindParents(schImParentDiagram);
                        enuFeatures.Reset();
                        ISchematicInMemoryFeature schFeature = enuFeatures.Next();
                        while (schFeature != null)
                        {
                            ISchematicRelationManager schRelManager = schFeature.SchematicRelationManager;
                            if (schFeature == null) continue;

                            IGeometry esrGeo = schRelManager.GetParentGeometry(schRelCont, null, (ILayer) schLayer,
                                                                               schFeature);
                            schFeature.Shape = esrGeo;
                            schFeature = enuFeatures.Next();
                        }
                        ArcMap.Application.StatusBar.Message[0] =
                            string.Format("Saving parent diagram:- {0}", schParentDiagram.Name);
                        Application.DoEvents();
                        // Save diagram
                        schImParentDiagram.Save((ILayer) schLayer);
                        schImParentDiagram.Refresh();
                    }
                    else
                    {
                        diagramLocked = true;
                        Logger.Log.Error(string.Format("Diagram {0} could not be updated.  The Diagram is locked, try again later.", schParentDiagram.Name));
                        m_bFailedUpdate = true;
                        listFailed.Add(schParentDiagram.Name);
                    }

                    schImParentDiagram = null;
                }
                catch (Exception e)
                {
                    Logger.Log.Error(string.Format("Diagram {0} could not be updated.  An error occured.\nError Message:  {1}", schParentDiagram.Name, e.Message), e);
                    m_bFailedUpdate = true;
                    listFailed.Add(schParentDiagram.Name);
                }
            }

            //write out circuit changes

            if (circuitChanges.Count > 0)
            {
                IWorkspaceEdit edit = m_maintenanceExt.Configuration.SchematicConfigWorkspace as IWorkspaceEdit;

                // ICursor insertCursor = null;
                try
                {
                    ITable circuitChangesTable =
                       m_maintenanceExt.Configuration.SchematicConfigWorkspace.OpenTable(
                           GetQualifiedName((IWorkspace)m_maintenanceExt.Configuration.SchematicConfigWorkspace,
                                            SchematicsMaintenanceConfiguration.EDSCHEM_Owner,
                                            SchematicsMaintenanceConfiguration.EDSCHEM_CircuitChanges));

                    //April 2019 release - Added MultiuserEditing to resolve database redefined state errors in checkin/checkout when multiple users are editing simultaneously
                    ((IMultiuserWorkspaceEdit)edit).StartMultiuserEditing(esriMultiuserEditSessionMode.esriMESMNonVersioned);
                    //edit.StartEditing(true);
                    //edit.StartEditOperation();

                    //insertCursor = circuitChangesTable.Insert(true);
                    foreach (var circuitChange in circuitChanges)
                    {

                        if (!string.IsNullOrEmpty(circuitChange))
                        {
                            IRow row = circuitChangesTable.CreateRow();
                            row.Value[row.Fields.FindField("CIRCUITID")] = circuitChange;
                            row.Store();
                            //insertCursor.InsertRow(buffer);
                        }
                    }

                    //insertCursor.Flush();
                    //edit.StopEditOperation();
                    edit.StopEditing(true);
                    //System.Runtime.InteropServices.Marshal.FinalReleaseComObject(insertCursor);
                }
                catch (Exception e)
                {
                    //Log error and continue
                    Logger.Log.Error("Error occurred while populating Circuit Changes table", e);
                    try
                    {
                        //edit.AbortEditOperation();
                        edit.StopEditing(false);

                        //if (insertCursor != null)
                        //    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(insertCursor);
                    }
                    catch (Exception)
                    {
                    }

                }
            }


            if (listFailed.Count > 0)
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                foreach (string s in listFailed)
                {
                    sb.Append(String.Format("- {0}\n", s));
                }
                MessageBox.Show(
                    String.Format(
                        "The following diagram(s) have not been saved, {0}.  Please try the Check In again.\n{1}",
                        diagramLocked ? "the diagram(s) are locked by another user" : "an error occurred while saving the diagram",
                        sb.ToString()));
            }

            return changedGrids;
        }


        public bool IsUnlocked(ISchematicDiagram schematicDiagram, string diagramName)
        {
            try
            {
                IRow row = schematicDiagram as IRow;
                int lockStatusFieldIndex = row.Fields.FindField("LOCKSTATUS");
                IField field = row.Fields.get_Field(lockStatusFieldIndex);
                int lockStatus = (int)row.get_Value(lockStatusFieldIndex);
                if (lockStatus == 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                string text = "Error encountered while determining the lock status for diagram '" + diagramName + "'." + ex.Message;
                // this.logger.RecordMessage(text, MessageTypeEnum.Error, true, null);
                return false;
            }
        }


        private List<string> CopyLayoutToParent(ISchematicInMemoryDiagram schemImChildDiagram,
                                                ISchematicInMemoryDiagram schemImParentDiagram,
                                                out bool updateParentCrossings)
        {
            IEnumSchematicInMemoryFeature enuChild = schemImChildDiagram.SchematicInMemoryFeatures;
            ISchematicInMemoryFeature schImChildFeature = enuChild.Next();

            //Compare point features, if x/y has changed; write feature to EDSCHEM_PointFeatureChanges

            //Removing PointChanges table...removed from design
            //ITable pointChanges =
            //    m_maintenanceExt.Configuration.SchematicConfigWorkspace.OpenTable(
            //        SchematicsMaintenanceConfiguration.EDSCHEM_PointFeatureChanges);


            // int changeUGuid = pointChanges.FindField("FEATUREGLOBALID");
            //int changeSGuid = pointChanges.FindField("SCHEMATICFEATUREGLOBALID");
            //int changeUCid = pointChanges.FindField("FEATURECLASSID");
            //int x = pointChanges.FindField("X");
            //int y = pointChanges.FindField("Y");

            List<string> circuitIDs = new List<string>();
            bool updateCrossMarks = false;
            while (schImChildFeature != null)
            {
                ISchematicInMemoryFeature schImParentFeature =
                    schemImParentDiagram.GetSchematicInMemoryFeatureByType(
                        schImChildFeature.SchematicElementClass.SchematicElementType, schImChildFeature.Name);


                if (schImChildFeature.SchematicElementClass.Name == "CrossMark" && schImParentFeature == null)
                {
                    // user created new cross mark, need to run algorithm on parent diagram
                    updateCrossMarks = true;
                }

                if (schImParentFeature != null)
                {
                    int displayedFld = schImChildFeature.Fields.FindField("ISDISPLAYED");

                    if (displayedFld != -1) //field exists, check value
                    {
                        if (Convert.ToInt32(schImChildFeature.Value[displayedFld]) != -1)
                        {
                            //child feature was deleted, ignore this child feature.  It will remain as-is in the parent diagram
                            continue;
                        }
                    }

                    int rotationFld = schImChildFeature.Fields.FindField("ROTATION");
                    //copy the value of the rotation field from the child feature
                    if (rotationFld > -1)
                    {
                        schImParentFeature.Value[rotationFld] = schImChildFeature.Value[rotationFld];
                    }

                    //only write points to secondary table "schematic point feature changes"
                    IGeometry childGeometry = schImChildFeature.ShapeCopy;
                    //IRelationalOperator op = childGeometry as IRelationalOperator;
                    //ITopologicalOperator op2 = childGeometry as ITopologicalOperator;

                    //if (childGeometry.GeometryType == esriGeometryType.esriGeometryPolyline)
                    //{
                    //    IPointCollection childPoint = childGeometry as IPointCollection;
                    //    IPointCollection parentPoint = schImParentFeature.Shape as IPointCollection;

                    //    bool isEqual = op.Equals((IGeometry)schImParentFeature.Shape);



                    //}




                    if (childGeometry != null &&
                        !((IClone)childGeometry).IsEqual(schImParentFeature.Shape as IClone))
                    {
                        //if (childGeometry.GeometryType == esriGeometryType.esriGeometryPoint)
                        //{
                        //     int uguid = schImParentFeature.Fields.FindField("UGUID");
                        //    int sguid = schImParentFeature.Fields.FindField("SCHEMATICID");
                        //    if (sguid == -1)
                        //        sguid = schImParentFeature.Fields.FindField("NAME");
                        //    int ucid = schImParentFeature.Fields.FindField("UCID");

                        //    //write info to point changes table
                        //    IRow newRow = pointChanges.CreateRow();
                        //    if(sguid != -1)
                        //        newRow.Value[changeSGuid] = schImParentFeature.Value[sguid];

                        //    if (uguid != -1)
                        //        newRow.Value[changeUGuid] = schImParentFeature.Value[uguid];
                        //    if (ucid != -1)
                        //        newRow.Value[changeUCid] = schImParentFeature.Value[ucid];
                        //    newRow.Value[x] = ((IPoint) childGeometry).X;
                        //    newRow.Value[y] = ((IPoint) childGeometry).Y;

                        //    newRow.Store();


                        //}

                        //compile distinct list of modified circuit id's for this diagram
                        int circuitIDFld = schImChildFeature.Fields.FindField("CIRCUITID");
                        if (circuitIDFld > -1)
                        {
                            string value = Convert.ToString(schImChildFeature.Value[circuitIDFld]);
                            if (!string.IsNullOrEmpty(value) && !circuitIDs.Contains(value))
                                circuitIDs.Add(value);
                        }


                        //if (childGeometry.GeometryType == esriGeometryType.esriGeometryLine ||
                        //    childGeometry.GeometryType == esriGeometryType.esriGeometryPolyline)
                        //{
                        //    int parentCount = ((ISchematicInMemoryFeatureLinkGeometry) schImParentFeature).VerticesCount;
                        //    int childCount = ((ISchematicInMemoryFeatureLinkGeometry) schImChildFeature).VerticesCount;

                        //}

                        schImParentFeature.Shape = childGeometry;
                    }
                }

                schImChildFeature = enuChild.Next();
            }

            updateParentCrossings = updateCrossMarks;

            return circuitIDs;
        }

        private ISchematicDiagram GetParentDiagram(ISchematicDiagramClass schemDiagramClass, int iDiagram,
                                                   out ISchematicLayer schemLayer)
        {

            ISchematicDiagram parentDiagram = schemDiagramClass.get_SchematicDiagramByID(iDiagram);
            ISchematicLayer schLayer = new SchematicLayer();

            schLayer.SchematicDiagram = parentDiagram;
            schemLayer = schLayer;

            return parentDiagram;
        }

        private IMap GetDiagramMap(string DiagramName)
        {
            IMaps esrMaps = ArcMap.Document.Maps;
            IMap esrMap = null;

            for (int i = 0; i <= esrMaps.Count - 1; i++)
            {
                esrMap = esrMaps.get_Item(i);

                IEnumLayer enuLayers = esrMap.Layers;
                ILayer esrLayer = enuLayers.Next();
                while (esrLayer != null)
                {
                    ISchematicLayer schLayer = esrLayer as ISchematicLayer;
                    if (schLayer != null && schLayer.SchematicDiagram.Name == DiagramName)
                        return esrMap;

                    esrLayer = enuLayers.Next();
                }
            }

            return null;
        }

        private string GetQualifiedName(IWorkspace workspace, string owner, string table)
        {
            if (string.IsNullOrEmpty(owner))
                return table;
            if (workspace == null)
                return table;

            return workspace.WorkspaceFactory.WorkspaceType ==
                   esriWorkspaceType.esriRemoteDatabaseWorkspace
                       ? owner + "." + table
                       : table;
        }
    }
}