using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Desktop.AddIns;
namespace Esri.SchematicsMaintenance
{
  public class SetLinkLengthCombo : ESRI.ArcGIS.Desktop.AddIns.ComboBox
  {
    private static SetLinkLengthCombo s_comboBox;
    //private int m_selAllCookie;

    public SetLinkLengthCombo()
    {      
      //m_selAllCookie = -1;
      s_comboBox = this;
      s_comboBox.Add("25");
      s_comboBox.Add("50");
      s_comboBox.Add("75");
      s_comboBox.Add("100");
      s_comboBox.Add("100");
      s_comboBox.Add("125");
      s_comboBox.Add("150");
      s_comboBox.Add("175"); 
      s_comboBox.Add("200");
      s_comboBox.Value = "75";
    }

    public double Length
    {
        get
        {
            string lengthString = this.Value;
            try
            {
                double length = System.Convert.ToDouble(lengthString);
                return length;
            }
            catch
            {
                return -1.0;
            }
        }
    }
    
    internal static SetLinkLengthCombo GetSetLinkLengthComboBox()
    {
      return s_comboBox;
    }


  }

}
 
