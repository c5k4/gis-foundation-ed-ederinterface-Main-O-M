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
using System.Windows.Forms;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Schematic;
using ESRI.ArcGIS.SchematicUI;
using ESRI.ArcGIS.Carto;
using PGE.Desktop.SchematicsMaintenance.Core;

namespace PGE.Desktop.SchematicsMaintenance
{
  public class PropagateFromGeo : ESRI.ArcGIS.Desktop.AddIns.Button
  {
    SchematicExtension m_schematicExtension = null;

    public PropagateFromGeo()
    {
    }

    ~PropagateFromGeo()
    {
      m_schematicExtension = null;
    }

    protected override void OnClick()
    {

        try
        {
            if (m_schematicExtension == null)
            {
                OnUpdate();
                if (!Enabled) return;
            }

            ArcMap.Application.CurrentTool = null;

            IMap esrMapGeographical = Utils.GetGeographicalMap(true);
            if (esrMapGeographical == null) return;

            IMap esrMapSchematic = Utils.GetSchematicMap();
            if (esrMapSchematic == null) return;

            esrMapSchematic.ClearSelection();

            IEnumFeature enumFeature = (IEnumFeature)esrMapGeographical.FeatureSelection;
            ((IEnumFeatureSetup)enumFeature).AllFields = true;

            enumFeature.Reset();
            IFeature esrFeature = enumFeature.Next();

            // get the list of all feature and associated feature
            string sFilterAssociation = "";
            string sList = "";
            int iFcId = -1;
            while (esrFeature != null)
            {
                int iOid = esrFeature.OID;
                if (iFcId == -1) iFcId = esrFeature.Class.ObjectClassID;

                if (sList == "")
                    sList = iOid.ToString();
                else
                    sList = String.Format("{0}, {1}", sList, iOid);

                esrFeature = enumFeature.Next();

                if (esrFeature == null || (iFcId != esrFeature.Class.ObjectClassID))
                {
                    if (sFilterAssociation == "")
                        sFilterAssociation = String.Format("(UCID = {0} AND UOID IN ({1}))", iFcId, sList);
                    else
                        sFilterAssociation = String.Format("{2} OR (UCID = {0} AND UOID IN ({1}))", iFcId, sList, sFilterAssociation);

                    sList = "";
                    iFcId = -1;
                }
            }

            if (sFilterAssociation == "") return;

            for (int i = 0; i < esrMapSchematic.LayerCount; i++)
            {
                ILayer esrLayer = esrMapSchematic.get_Layer(i);

                ISchematicDiagramClassLayer schDiagClassLayer = esrLayer as ISchematicDiagramClassLayer;
                if (schDiagClassLayer != null)
                {
                    ICompositeLayer esrCompositelayer = schDiagClassLayer as ICompositeLayer;
                    IObjectClass esrObjectClass = schDiagClassLayer.SchematicDiagramClass as IObjectClass;
                    int iDiagClassId = esrObjectClass.ObjectClassID;

                    for (int j = 0; j < esrCompositelayer.Count; j++)
                    {
                        List<int> listIds = new List<int>();

                        IFeatureLayer esrFeatLayer = esrCompositelayer.get_Layer(j) as IFeatureLayer;

                        if (esrFeatLayer == null || esrFeatLayer.FeatureClass == null)
                            continue;

                        iFcId = esrFeatLayer.FeatureClass.ObjectClassID;

                        // Get schematic feature table name
                        string sEltIdTableName = "";
                        IWorkspace esrWorkspace = ((IDataset)esrObjectClass).Workspace;

                        sEltIdTableName = Utils.GetTableName(esrWorkspace, iFcId);

                        IQueryDef esrQuery = ((IFeatureWorkspace)esrWorkspace).CreateQueryDef();
                        esrQuery.Tables = sEltIdTableName;
                        esrQuery.SubFields = "ID";
                        esrQuery.WhereClause = String.Format("DIAGRAMCLASSID = {0} AND ISDISPLAYED = -1 AND ({1})", iDiagClassId, sFilterAssociation);

                        ICursor esrCursor = esrQuery.Evaluate();
                        if (esrCursor != null)
                        {
                            int indexID = esrCursor.FindField("ID");
                            IRow ipRow = esrCursor.NextRow();
                            while (ipRow != null)
                            {
                                int ID = (int)ipRow.get_Value(indexID);
                                if (!listIds.Contains(ID))
                                    listIds.Add(ID);
                                ipRow = esrCursor.NextRow();
                            }
                        }

                        int indexReplace = sEltIdTableName.IndexOf("E_");
                        string sAssTableName = String.Format("{0}A{1}", sEltIdTableName.Substring(0, indexReplace), sEltIdTableName.Substring(indexReplace + 1));

                        string sFcOID = String.Format("{0}.ID", sEltIdTableName);
                        string sAssID = String.Format("{0}.SCHEMATICID", sAssTableName);

                        string sSecFilterAss = sFilterAssociation.Replace("UCID", String.Format("{0}.UCID", sAssTableName));
                        sSecFilterAss = sSecFilterAss.Replace("UOID", String.Format("{0}.UOID", sAssTableName));

                        esrQuery = ((IFeatureWorkspace)esrWorkspace).CreateQueryDef();
                        esrQuery.Tables = String.Format("{0}, {1}", sEltIdTableName, sAssTableName);
                        esrQuery.SubFields = sFcOID;
                        esrQuery.WhereClause = String.Format("DIAGRAMCLASSID = {0} AND ISDISPLAYED = -1 AND {1} = {2} AND ({3})", iDiagClassId, sFcOID, sAssID, sSecFilterAss);

                        esrCursor = esrQuery.Evaluate();
                        if (esrCursor != null)
                        {
                            int indexID = esrCursor.FindField(sFcOID);
                            IRow esrRow = esrCursor.NextRow();

                            while (esrRow != null)
                            {
                                int iID = (int)esrRow.get_Value(indexID);

                                if (!listIds.Contains(iID))
                                    listIds.Add(iID);

                                esrRow = esrCursor.NextRow();
                            }
                        }

                        if (listIds.Count == 0) continue;

                        Utils.SelectFeaturesInMap(esrMapSchematic, esrFeatLayer, listIds);
                    }
                }
            }
            if (ArcMap.Document.ActiveView.FocusMap == esrMapSchematic)
                ArcMap.Document.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection, null, null);
        }
        catch (Exception e)
        {
            MessageBox.Show(
                "And error occurred while selecting features in the schematic layer. The selection may be invalid.",
                "Propagate To Schematic", MessageBoxButtons.OK);
            Logger.Log.Error("Propagate From Geo error", e);
            throw;
        }
      
    }

    protected override void OnUpdate()
    {
      if (m_schematicExtension == null)
        m_schematicExtension = Utils.GetSchematicExtension();

      if (m_schematicExtension == null)
      {
        Enabled = false;
        return;
      }

      Enabled = (Utils.GetGeographicalMap(true) != null);
    }
  }
}
