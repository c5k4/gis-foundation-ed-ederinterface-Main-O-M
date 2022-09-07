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

namespace PGE.Desktop.SchematicsMaintenance
{
  public partial class FrmSelectMap : Form
  {
    public FrmSelectMap()
    {
      InitializeComponent();
    }

    private void BtnClick(object sender, EventArgs e)
    {
      this.Close();
    }

    public void InitList(Dictionary<string, int> myList)
    {
      cboMap.Items.Clear();

      foreach (KeyValuePair<string, int> myValue in myList)
      {
        cboMap.Items.Add(myValue.Key);
      }
    }

    public string GetMapName()
    {
      return cboMap.Text;
    }
  }
}
