using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PGE.Desktop.SchematicsMaintenance.Common;

namespace PGE.Desktop.SchematicsMaintenance.Enums
{
    /// <summary>
    /// Defines the options for applying the angle at which a bus bar intersects another.
    /// </summary>
    public enum ConvergeAngle
    {
        [DisplayString("30")]
        Angle30 = 30,
        [DisplayString("45")]
        Angle45 = 45,
        [DisplayString("90")]
        Angle90 = 90,
        [DisplayString("120")]
        Angle120 = 120,
        [DisplayString("135")]
        Angle135 = 135,
       
    }
}
