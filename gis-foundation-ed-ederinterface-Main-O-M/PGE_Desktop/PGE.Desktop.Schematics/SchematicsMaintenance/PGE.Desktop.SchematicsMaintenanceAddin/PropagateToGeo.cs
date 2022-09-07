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

using System.Collections.Generic;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.SchematicUI;
using System.Windows.Forms;
using ESRI.ArcGIS.Schematic;

namespace PGE.Desktop.SchematicsMaintenance
{
  public class PropagateToGeo : ESRI.ArcGIS.Desktop.AddIns.Button
  {
    SchematicExtension m_schematicExtension = null;

    public PropagateToGeo()
    {
    }

    ~PropagateToGeo()
    {
      m_schematicExtension = null;
    }

    protected override void OnClick()
    {
      if (m_schematicExtension == null)
      {
        OnUpdate();
        if (!Enabled)
          return;
      }

      ArcMap.Application.CurrentTool = null;

      IMap esrMapGeographical = Utils.GetGeographicalMap();

      if (esrMapGeographical == null)
      {
        MessageBox.Show("The Check Out Diagrams function is looking for a data frame containing the GIS features associated with the currently selected schematic features. Such a data frame is not found.");
        return;
      }

      IMap esrMapSchematic = Utils.GetSchematicMap(true);
      if (esrMapSchematic == null) return;

      List<int> listMasterIds = new System.Collections.Generic.List<int>();
      ISchematicDiagramClass schDiag = null;
      Utils.SelectFromSchematic(esrMapSchematic, esrMapGeographical, false, ref listMasterIds, ref schDiag);

      if (ArcMap.Document.ActiveView.FocusMap == esrMapGeographical)
        ArcMap.Document.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection, null, null);
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

      Enabled = (Utils.GetSchematicMap(true) != null);
    }
  }
}
