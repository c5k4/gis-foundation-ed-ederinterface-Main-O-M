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
using PGE.Desktop.SchematicsMaintenance.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PGE.Desktop.SchematicsMaintenance.Enums
{
    /// <summary>
    /// Defines the options for applying a rotation to a schematic.
    /// </summary>
    public enum RotationAngle
    {
        [DisplayString("45")]
        Angle45 = 45,
        [DisplayString("90")]
        Angle90 = 90,
        [DisplayString("135")]
        Angle135 = 135,
        [DisplayString("180")]
        Angle180 = 180,
        [DisplayString("225")]
        Angle225 = 225,
        [DisplayString("270")]
        Angle270 = 270,
        [DisplayString("315")]
        Angle315 = 315//,
        //[DisplayString("Align to Links")]
        //AlignToLinks = 0
    }
}
