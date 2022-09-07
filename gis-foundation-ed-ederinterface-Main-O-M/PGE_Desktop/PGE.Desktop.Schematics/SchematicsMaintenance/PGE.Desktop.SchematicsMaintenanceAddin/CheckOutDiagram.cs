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
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Schematic;
using ESRI.ArcGIS.SchematicControls;
using ESRI.ArcGIS.SchematicUI;
using ESRI.ArcGIS.esriSystem;
using PGE.Desktop.SchematicsMaintenance.Configuration;
using PGE.Desktop.SchematicsMaintenance.Core;
using PGE.Desktop.SchematicsMaintenance.UI.Extensions;
using Button = ESRI.ArcGIS.Desktop.AddIns.Button;

namespace PGE.Desktop.SchematicsMaintenance
{
    public class CheckOutDiagram : Button
    {
        private SchematicExtension m_schematicExtension;
        private PGESchematicsMaintenanceExtension m_maintenanceExt;

        ~CheckOutDiagram()
        {
            m_schematicExtension = null;
        }

        protected override void OnClick()
        {
            
            //create geometry from graphics, use it to select features
            //don't update SIP and SUP until checkout is successfull.
            IMap map = ArcMap.Document.FocusMap;
            var referenceScale = map.ReferenceScale;

            var defaultGraphicsLayer = map.ActiveGraphicsLayer as IGraphicsContainer;

            IMap esrMapGeographical = null;
            ISchematicDataset schDataset = null;

            IFeatureClass sipFeatureClass = Utils.FindFeatureClass("EDSCHEM_InProgressPolygon");
            IFeatureClass supFeatureClass = Utils.FindFeatureClass("EDSCHEM_UpdatePolygon");

             int iFieldMasterId = -1;
             List<int> listMasterIds = new List<int>();

            ISchematicDiagramClass schChildTemplate = null;
            IPolygon sipGeometry = null;
            try
            {

            
        
                //create polgyon geometry from graphics
                
                if (sipFeatureClass == null)
                {
                    MessageBox.Show("The Check Out Diagrams function is looking for a Feature Layer containing the Schematic In Progress Polygon features. Such a layer is not found.");
                    return;
                }
                if (supFeatureClass == null)
                {
                    MessageBox.Show("The Check Out Diagrams function is looking for a Feature Layer containing the Schematic Update Polygon features. Such a layer is not found.");
                    return;
                }



                sipGeometry = Utils.CreateSIPPolygonFromGraphics(defaultGraphicsLayer, sipFeatureClass, supFeatureClass);

                if (sipGeometry == null)
                {
                    MessageBox.Show(string.Format("Please define the area of interest."));
                    return;
                }

                ArcMap.Application.StatusBar.Message[0] = "Selecting schematic features";
                Application.DoEvents();

                Utils.SelectSchematicFeaturesByPolygon(map, sipGeometry);

                if (m_schematicExtension == null)
                {
                    OnUpdate();
                    if (!Enabled)
                        return;
                }

                ArcMap.Application.CurrentTool = null;

                esrMapGeographical = Utils.GetGeographicalMap();
                if (esrMapGeographical == null)
                {
                    MessageBox.Show(
                        "The Check Out Diagrams function is looking for a data frame containing the GIS features associated with the currently selected schematic features. Such a data frame is not found.");
                    ArcMap.Application.StatusBar.Message[0] = "";
                    return;
                }

             
                ISchematicDiagramClass schDiagramClass = null;

                
                Utils.SelectFromSchematic(ArcMap.Document.ActiveView.FocusMap, esrMapGeographical, true, ref listMasterIds,
                                          ref schDiagramClass);
              

                if (listMasterIds.Count == 0)
                {
                    MessageBox.Show("Current selection is not valid");
                    ArcMap.Application.StatusBar.Message[0] = "";
                    Application.DoEvents();
                    return;
                }

                schDataset = schDiagramClass.SchematicDataset;
                if (!m_maintenanceExt.Configuration.ParentChildConfig.ContainsKey(schDiagramClass.Name))
                {
                    MessageBox.Show(string.Format("Unable to determine Child Diagram template for diagram class {0}.  The check out tool is not configured for this diagram type.",schDiagramClass.Name));
                    ArcMap.Application.StatusBar.Message[0] = "";
                    Application.DoEvents();
                    return;
                }

                string sChildDiagramTemplateName = m_maintenanceExt.Configuration.ParentChildConfig[schDiagramClass.Name];

                var schDiagClassCont = schDataset as ISchematicDiagramClassContainer;
                schChildTemplate =
                    schDiagClassCont.GetSchematicDiagramClass(sChildDiagramTemplateName);

                if (schChildTemplate == null)
                {
                    MessageBox.Show(
                        String.Format(
                            "The diagram template, '{0}', used to generate child diagram does not exists in this schematic dataset.",
                            sChildDiagramTemplateName));
                    ArcMap.Application.StatusBar.Message[0] = "";
                    Application.DoEvents();
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occured.  The current map document may not be configured properly.");
                ArcMap.Application.StatusBar.Message[0] = "";
                Application.DoEvents();
                Logger.Log.Error(ex);
                return;
            }

            try
            {
                ArcMap.Application.StatusBar.Message[0] = "Initializing schematic builder";
                Application.DoEvents();
                // Initialize the builder context from the selected features
                ISchematicBuilderContext schBuilderContext = InitBuilderContextFromMap(esrMapGeographical);

                if (schBuilderContext == null)
                {
                    MessageBox.Show(
                        "The Check Out Diagrams function is looking for a data frame containing the GIS features associated with the currently selected schematic features. Such a data frame is not found.");
                    ArcMap.Application.StatusBar.Message[0] = "";
                    Application.DoEvents();
                    return;
                }

                ISchematicFolder schJobFolder =
                    (schDataset as ISchematicFolderContainer).get_SchematicFolderByName(Utils.cstJobFolderName);
                if (schJobFolder == null)
                    schJobFolder = schDataset.CreateSchematicFolder(Utils.cstJobFolderName);

                //get session id
                string sessionID;
                string region;
                bool cancelled = Utils.GetSessionID(supFeatureClass, sipGeometry, out sessionID, out region);

                if (cancelled)
                {
                    ArcMap.Application.StatusBar.Message[0] = "Checkout Cancelled";
                    Application.DoEvents();
                    return;
                }

                string sMasterIDs = Utils.BuildString(listMasterIds);

                //Child Diagram naming format 'Session ID'+'_Edit'+'_'+'LAN ID'
                string pChildName = GetDiagramName(schChildTemplate, sessionID + "_Edit", System.Environment.UserName);

                ArcMap.Application.StatusBar.Message[0] = string.Format("Generating edit diagram {0}", pChildName);
                Application.DoEvents();
                // Generate the new 'child' diagram
                var schBuilder = schChildTemplate as ISchematicBuilder;
                var schDiagramCont = schJobFolder as ISchematicDiagramContainer;

                ISchematicDiagram schChildDiagram = schBuilder.GenerateDiagram(pChildName, schDiagramCont, null,
                                                                               schBuilderContext, null);

                iFieldMasterId = schChildDiagram.Fields.FindField("MASTERIDS");
                if (iFieldMasterId < 0)
                {
                    MessageBox.Show("The field MASTERIDS does not exist for this diagram template");
                    ((IDataset) schChildDiagram).Delete();
                    ArcMap.Application.StatusBar.Message[0] = "";
                    Application.DoEvents();
                    return;
                }

                // Set the field so we know this is a child and what the masters are
                try
                {
                    schChildDiagram.set_Value(schChildDiagram.Fields.FindField("MASTERIDS"), sMasterIDs);
                    schChildDiagram.Store();
                }
                catch
                {
                    MessageBox.Show("The field MASTERIDS is too short to store the list of Master Diagram Id");
                    ((IDataset) schChildDiagram).Delete();
                    ArcMap.Application.StatusBar.Message[0] = "";
                    Application.DoEvents();
                    return;
                }

                ArcMap.Application.StatusBar.Message[0] = string.Format("Adding diagram {0} to the current document",
                                                                        pChildName);
                Application.DoEvents();
                // Open the new diagram in the map and start an edit session
                
                ISchematicLayer schChildLayer =
                    ((ISchematicDisplayDiagramHelper) m_schematicExtension).DisplayDiagram(schChildDiagram, null);
                schChildLayer.StartEditSchematicDiagram(true);

                //disable drawing until the we're done making changes to the map
                ArcMap.Document.FocusMap.DelayDrawing(true);
                CopyLayersToChildDataFrame(map, ArcMap.Document.FocusMap, schChildTemplate.Name);


                ArcMap.Application.StatusBar.Message[0] = string.Format("Copying layout from master diagram(s)");
                Application.DoEvents();
                // Copy layout from masters to child diagram
                CopyLayoutFromMasterToChildDiagram(sMasterIDs, schChildLayer);


                Utils.CommitSessionMetadata(sipFeatureClass, supFeatureClass, sipGeometry, sessionID, region);

                // Attempt to set the spatial reference of the data frame
                if (ArcMap.Document.FocusMap.SpatialReference is IUnknownCoordinateSystem &&
                    !ArcMap.Document.FocusMap.SpatialReferenceLocked)
                {
                    var geoDataset = sipFeatureClass as IGeoDataset;
                    if (geoDataset != null &&
                        geoDataset.SpatialReference != null &&
                        !(geoDataset.SpatialReference is IUnknownCoordinateSystem))
                    {
                        ArcMap.Document.FocusMap.SpatialReference = geoDataset.SpatialReference;
                    }
                }



                defaultGraphicsLayer.DeleteAllElements();


                //Utils.ClearGraphics(map);
                ArcMap.Application.StatusBar.Message[0] = "Diagram Check-Out completed successfully";
                Application.DoEvents();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    String.Format("Exception message:\n{0}\nException stack trace:\n{1}", ex.Message, ex.StackTrace),
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                ArcMap.Application.StatusBar.Message[0] = "";
                Application.DoEvents();
                Logger.Log.Error(ex);
            }
           
            finally
            {

                //set Edit frame reference scale
                ArcMap.Document.FocusMap.ReferenceScale = referenceScale;
                ArcMap.Document.FocusMap.DelayDrawing(false);
                //refresh the map, since we just updated the schematic layout
                ArcMap.Document.ActiveView.Refresh();
            }
        }

        private void CopyLayersToChildDataFrame(IMap map, IMap focusMap, string templateName)
        {
            ITable checkOutLayersTable = null;
            var checkOutLayers = new Dictionary<string, bool>();
            try
            {
                checkOutLayersTable =
                    m_maintenanceExt.Configuration.SchematicConfigWorkspace.OpenTable(
                        GetQualifiedName((IWorkspace) m_maintenanceExt.Configuration.SchematicConfigWorkspace,
                                         SchematicsMaintenanceConfiguration.EDSCHEM_Owner,
                                         SchematicsMaintenanceConfiguration.EDSCHEM_CheckOutLayers));
            }
            catch (Exception e)
            {
                Logger.Log.Error(
                    string.Format("{0} table does not exist.  No layers will be copied to Check Out Dataframe",
                                  SchematicsMaintenanceConfiguration.EDSCHEM_CheckOutLayers), e);
                return;
            }
            try
            {

                var cursor = checkOutLayersTable.Search(
                    new QueryFilterClass()
                        {
                            SubFields = "*",
                            WhereClause = string.Format("{0} = '{1}'", "Template", templateName)
                        }, true);
                for (var row = cursor.NextRow(); row != null; row = cursor.NextRow())
                {
                    checkOutLayers.Add(Convert.ToString(row.Value[row.Fields.FindField("Layer")]), false);
                }

                if (checkOutLayers.Count == 0)
                {
                    Logger.Log.Error(
                    string.Format("There are no Check Out Layers configured for the {0} template.  No layers will be copied to Check Out Dataframe",
                                  templateName));
                    return;
                }

                var targetMap = focusMap as IMapLayers2;
                IObjectCopy objectCopy = new ObjectCopyClass();
                int insertAt = 0;
                for (int i = 0; i < map.LayerCount; i++)
                {

                    //copy layers, only the template layer and configuration layers are excluded
                    if (checkOutLayers.ContainsKey(map.Layer[i].Name))
                    {
                        checkOutLayers[map.Layer[i].Name] = true;
                        ArcMap.Application.StatusBar.Message[0] = string.Format("Copying layer {0} to {1}", map.Layer[i].Name, focusMap.Name);
                        Application.DoEvents();
                        object clonedLayer = objectCopy.Copy(map.Layer[i]);

                        targetMap.InsertLayer(clonedLayer as ILayer, false, insertAt);
                        insertAt++;
                    }

                    //if the schematic template layer was skipped, advance the counter.
                    if (map.Layer[i] is ISchematicDiagramClassLayer)
                        insertAt++;

                }
                //warn the user if any layers weren't found in the source dataframe.

                foreach (var checkOutLayer in checkOutLayers)
                {
                    if (!checkOutLayer.Value)
                    {
                        MessageBox.Show(
                            string.Format("{0} layer does not exist in the source dataframe.", checkOutLayer.Key),
                            "Layer not found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }

               
            }
            catch (Exception ex)
            {
                Logger.Log.Error("",ex);
                ArcMap.Application.StatusBar.Message[0] = "";
            }
            
        }

        private bool CanCopyLayer(ILayer layer, IList<string> excludedLayers )
        {
          
            if (layer is ISchematicDiagramClassLayer)
            {
                return false;
            }
            if (layer is IFeatureLayer)
            {
                var flayer = layer as IFeatureLayer;
                var dataset = flayer.FeatureClass as IDataset;


                return dataset != null && !excludedLayers.Contains(dataset.BrowseName.ToUpper());
            }
            if (layer is ICompositeLayer)
            {
                var glayer = layer as ICompositeLayer;

                for (int j = 0; j < glayer.Count; j++)
                {
                    if(!CanCopyLayer(glayer.Layer[j], excludedLayers))
                        return false;
                }

            }
            return true;

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

            Enabled = (Utils.IsSchematicMap(ArcMap.Document.ActiveView.FocusMap, false));
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
        private ISchematicBuilderContext InitBuilderContextFromMap(IMap mapGeo)
        {
            var myResult = new MyEnumObject((IEnumFeature) mapGeo.FeatureSelection);

            IObject myObject = myResult.Next();
            if (myObject == null) return null;

            myResult.Reset();

            Type obType = Type.GetTypeFromProgID("esriSchematic.SchematicStandardBuilderContext");
            var myContext = (ISchematicStandardBuilderContext) Activator.CreateInstance(obType);

            myContext.InitialObjects = myResult;
            return myContext as ISchematicBuilderContext;
        }

        private void CopyLayoutFromMasterToChildDiagram(string sMasterIds, ISchematicLayer schemChildLayer)
        {
            IRow esrRow;
            ITable esrTable;
            ICursor esrCursor;
            IQueryFilter esrQueryFilter;

            ISchematicInMemoryDiagram schImDiagram = schemChildLayer.SchematicInMemoryDiagram;
            IEnumSchematicElementClass enuSchEltClass =
                schImDiagram.SchematicDiagramClass.AssociatedSchematicElementClasses;
            enuSchEltClass.Reset();

            IList<string> listIDs;
            int iOid = schemChildLayer.SchematicDiagram.OID;

            ISchematicElementClass schEltClass = enuSchEltClass.Next();
            while (schEltClass != null)
            {
                listIDs = GetSchematicIDs(schEltClass, iOid);

                // get the list of Schematic objects affected
                if (listIDs.Count > 0)
                {
                    esrTable = schEltClass as ITable;
                    esrQueryFilter = new QueryFilter();
                    esrQueryFilter.SubFields = "*";
                    esrQueryFilter.WhereClause =
                        String.Format("DIAGRAMOBJECTID IN ({0}) AND {1} AND ISDISPLAYED = -1",
                                      sMasterIds, Utils.BuildInClause(listIDs, "SCHEMATICTID", "'"));
                    esrCursor = esrTable.Search(esrQueryFilter, true);

                    int indexSchematicTID = esrTable.Fields.FindField("SCHEMATICTID");
                    int indexShape = esrTable.Fields.FindField("SHAPE");

                    esrRow = esrCursor.NextRow();
                    while (esrRow != null)
                    {
                        var sSchematicTID = esrRow.get_Value(indexSchematicTID) as string;
                        var esrGeo = esrRow.get_Value(indexShape) as IGeometry;

                        ISchematicInMemoryFeature schChildImFeature =
                            schImDiagram.GetSchematicInMemoryFeatureByType(schEltClass.SchematicElementType,
                                                                           sSchematicTID);
                        
                        //copy the value of the rotation field to the child feature

                        if (schChildImFeature != null)
                        {
                            int childRotationFld = schChildImFeature.Fields.FindField("ROTATION");
                            int parentRotationFld = esrRow.Fields.FindField("ROTATION");
                            
                            if (childRotationFld > -1 && parentRotationFld > -1)
                            {
                                if (esrRow.Value[parentRotationFld] != null)
                                {
                                    schChildImFeature.Value[childRotationFld] = esrRow.Value[parentRotationFld];
                                    schChildImFeature.Store();
                                }
                            }
                            schChildImFeature.Shape = esrGeo;
                        }

                        esrRow = esrCursor.NextRow();
                    }
                }
                schEltClass = enuSchEltClass.Next();
            }
        }

        private string GetDiagramName(ISchematicDiagramClass schemDiagramClass, string DiagramName, string Suffix)
        {
            int i = -1;
            const int csiMaxLengthName = 220;
            if (DiagramName.Length > csiMaxLengthName) DiagramName = DiagramName.Substring(0, csiMaxLengthName);

            if (schemDiagramClass.get_SchematicDiagramByName(String.Format("{0}_{1}", DiagramName, Suffix)) != null)
            {
                int iDiagramID = 0;
                while (true)
                {
                    if (
                        schemDiagramClass.get_SchematicDiagramByName(String.Format("{0}_{1}_{2}", DiagramName, Suffix,
                                                                                   iDiagramID)) == null)
                    {
                        i = iDiagramID;
                        break;
                    }
                    iDiagramID++;
                }
            }

            if (i == -1)
                return String.Format("{0}_{1}", DiagramName, Suffix);
            else
                return String.Format("{0}_{1}_{2}", DiagramName, Suffix, i);
        }

        private IList<string> GetSchematicIDs(ISchematicElementClass schemEltClass, int iDiagramOID)
        {
            string sWhereClauseChild = String.Format("DIAGRAMOBJECTID = {0}", iDiagramOID.ToString());

            IQueryFilter esrQueryFilter = new QueryFilter();
            esrQueryFilter.WhereClause = sWhereClauseChild;

            IList<string> listIDs = new List<string>();

            var esrTable = schemEltClass as ITable;
            ICursor esrCursor = esrTable.Search(esrQueryFilter, true);
            int indexID = esrCursor.FindField("SCHEMATICTID");

            IRow esrRow = esrCursor.NextRow();

            while (esrRow != null)
            {
                listIDs.Add(esrRow.get_Value(indexID) as string);
                esrRow = esrCursor.NextRow();
            }

            return listIDs;
        }
    }
}