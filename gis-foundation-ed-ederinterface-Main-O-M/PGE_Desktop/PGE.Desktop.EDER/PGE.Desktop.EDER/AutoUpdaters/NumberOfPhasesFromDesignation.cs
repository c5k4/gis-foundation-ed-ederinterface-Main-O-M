using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

using Miner.ComCategories;
using Miner.Interop;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Framework;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace PGE.Desktop.EDER.AutoUpdaters
{
    [Guid("3113E3E2-EBA3-4973-B6FA-3011B1D4DC7F")]
    [ProgId("PGE.Desktop.EDER.NumberOfPhasesFromDesignation")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    public class NumberOfPhasesFromDesignation : BaseSpecialAU
    {
        public NumberOfPhasesFromDesignation()
            : base("PGE Number of Phases from Designation")
        { }

        protected override bool InternalEnabled(IObjectClass objectClass, mmEditEvent eEvent)
        {
            return ModelNameFacade.ContainsAllFieldModelNames(objectClass, SchemaInfo.Electric.FieldModelNames.PhaseDesignation,
                                                                           SchemaInfo.Electric.FieldModelNames.NumberOfPhases);
        }
        
        protected override void InternalExecute(IObject obj, mmAutoUpdaterMode eAUMode, mmEditEvent eEvent)
        {
            
            foreach (var phaseCountField in obj.GetFields(SchemaInfo.Electric.FieldModelNames.NumberOfPhases))
            {
                // Each phase is one letter, at the moment, so the length of the string is the phase count.
                object pPhaseDesig = obj.GetFieldValue(null, true, SchemaInfo.Electric.FieldModelNames.PhaseDesignation);
                //Fix for INC3820166 should not update phase count if phasedesignation
                //is null 
                if (pPhaseDesig != null)
                    phaseCountField.Value = pPhaseDesig.ToString().Length;
            }
        }
    }
}
