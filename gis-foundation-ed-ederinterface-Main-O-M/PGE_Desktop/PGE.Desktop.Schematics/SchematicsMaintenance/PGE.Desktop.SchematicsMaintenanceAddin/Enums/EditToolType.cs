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
    /// Defines the tab index lookup values for edit tools in the dockable tab control.
    /// </summary>
    public enum EditToolType
    {
        RotateSymbols = 0,
        RotateFeatures = 1,
        SetLinkLength = 2,
        SetLinkPerpendicularLength = 3,
        ConvergeBusBars = 4,
        CreateBypass = 5,
        OffsetLinks = 6
    }
}
