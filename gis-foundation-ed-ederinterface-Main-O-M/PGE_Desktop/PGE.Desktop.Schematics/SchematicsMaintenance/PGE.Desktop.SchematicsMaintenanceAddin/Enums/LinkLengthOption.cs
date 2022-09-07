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
    /// Defines the options for applying a length to a selected link.
    /// </summary>
    public enum LinkLengthOption
    {
        [DisplayString("25")]
        LinkLength25 = 25,
        [DisplayString("50")]
        LinkLength50 = 50,
        [DisplayString("75")]
        LinkLength75 = 75,
        [DisplayString("100")]
        LinkLength100 = 100,
        [DisplayString("125")]
        LinkLength125 = 125,
        [DisplayString("150")]
        LinkLength150 = 150,
        [DisplayString("175")]
        LinkLength175 = 175,
        [DisplayString("200")]
        LinkLength200 = 200

    }
}


