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
using System.Windows.Forms;

namespace PGE.Desktop.SchematicsMaintenance
{
  public partial class FrmCheckInProperties : Form
  {
    public FrmCheckInProperties()
    {
      InitializeComponent();
    }

    private void ClickButtons(object sender, EventArgs e)
    {
      this.Close();
    }

    public bool UpdateDiagrams
    {
      get { return chkUpdate.Checked; }
      set
      {
        chkUpdate.Checked = value;
      }
    }


    public bool DeleteChild { get { return chkDelete.Checked; } }

    public void Clear()
    {
      chkUpdate.Checked = false;
      chkDelete.Checked = false;
    }
  }
}
