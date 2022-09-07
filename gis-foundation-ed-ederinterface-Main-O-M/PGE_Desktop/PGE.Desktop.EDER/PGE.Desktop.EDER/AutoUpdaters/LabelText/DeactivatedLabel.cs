using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using Miner.ComCategories;
using Miner.Interop;
using PGE.Common.Delivery.Diagnostics;
using PGE.Desktop.EDER.UFM;

namespace PGE.Desktop.EDER.AutoUpdaters.LabelText
{
    [Guid("6934916B-189D-4010-A583-DACF3E8D92B2")]
    [ProgId("PGE.Desktop.EDER.DeactivatedLabel")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    public class DeactivatedLabel : BaseLabelTextAU
    {
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        public DeactivatedLabel() : base("PGE Deactivated LabelText AU")
        {
        }

        protected override string GetLabelText(IObject obj, mmAutoUpdaterMode autoUpdaterMode, mmEditEvent editEvent, int labelIndex)
        {
            _logger.Info("Executing Deactivated LabelText AU");

            return XSectionConductorHelper.CalculateXSectionConductorText(obj);
        }
    }
}
