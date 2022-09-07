using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;

using Miner.Interop;

using PGE.Common.Delivery.Framework;
using PGE.Desktop.EDER.UFM;

namespace PGE.Desktop.EDER.AutoUpdaters.LabelText
{
    internal class LabelTextHelper
    {
        internal static void ExecuteAUOnParent(IObject parentObject, mmAutoUpdaterMode eAUMode, mmEditEvent eEvent)
        {

            IMMSpecialAUStrategyEx executeAU = null;
            IObjectClass parentObjectClass = parentObject.Class;

            if (ModelNameFacade.ContainsClassModelName(parentObjectClass, SchemaInfo.Electric.ClassModelNames.Transformer))
            {
                executeAU = new TransformerLabel();                
            }
            else if (ModelNameFacade.ContainsClassModelName(parentObjectClass, SchemaInfo.Electric.ClassModelNames.PrimaryOHConductor))
            {
                executeAU = new PriOHConductorLabel();                
            }
            else if (ModelNameFacade.ContainsClassModelName(parentObjectClass, SchemaInfo.Electric.ClassModelNames.PrimaryUnderGround))
            {
                executeAU = new PriUGConductorLabel();                
            }
            else if (ModelNameFacade.ContainsClassModelName(parentObjectClass, SchemaInfo.Electric.ClassModelNames.SecondaryOHConductor))
            {
                executeAU = new SecOHConductorLabel();                
            }
            else if (ModelNameFacade.ContainsClassModelName(parentObjectClass, SchemaInfo.Electric.ClassModelNames.SecondaryUGConductor))
            {
                executeAU = new SecUGConductorLabel();                
            }
            else if (ModelNameFacade.ContainsClassModelName(parentObjectClass, SchemaInfo.Electric.ClassModelNames.PGEConduitSystem))
            {
                executeAU = new ConduitLabel();
            }
            else if (ModelNameFacade.ContainsClassModelName(parentObjectClass, SchemaInfo.Electric.ClassModelNames.SupportStructure))
            {
                executeAU = new SupportStructureLabel();
            }
            else if (ModelNameFacade.ContainsClassModelName(parentObjectClass, SchemaInfo.UFM.ClassModelNames.DcConductor))
            {
                executeAU = new DCConductorLabel();
            }

            else if (ModelNameFacade.ContainsClassModelName(parentObjectClass, SchemaInfo.Electric.ClassModelNames.PGEDeactivatedElectricLineSegment))
            {
                executeAU = new DeactivatedLabel();

            }

            executeAU.Execute(parentObject, eAUMode, eEvent);
        }
    }
}
