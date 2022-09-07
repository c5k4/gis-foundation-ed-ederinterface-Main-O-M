using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Miner.ComCategories;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Diagnostics;
using ESRI.ArcGIS.Geodatabase;
using System.Reflection;

namespace PGE.Desktop.EDER.AutoUpdaters.Special
{
    [ComVisible(true)]
    [Guid("d22673c8-aeb6-4c29-9d22-889d0f1829bf")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    public class PGE_Asset_Replacement_AU : BaseSpecialAU
    {
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        public static IObjectClass abandonedTable = null;
        public static IObject newAbandonedRow = null;

        public PGE_Asset_Replacement_AU()
            : base("PG&E - Asset Replacement")
        {

        }

        /// <summary>
        /// Only allow execution if the feature class and replacement feature have
        /// been specified from the PGE Asset Replacement D8 tree tool
        /// </summary>
        /// <param name="eAUMode"></param>
        /// <returns></returns>
        protected override bool CanExecute(Miner.Interop.mmAutoUpdaterMode eAUMode)
        {
            return true;
        }

        /// <summary>
        /// Return enabled if it is on feature create and it contains the asset replacement abandoned model name
        /// </summary>
        /// <param name="objectClass"></param>
        /// <param name="eEvent"></param>
        /// <returns></returns>
        protected override bool InternalEnabled(IObjectClass objectClass, Miner.Interop.mmEditEvent eEvent)
        {
            if (eEvent == Miner.Interop.mmEditEvent.mmEventFeatureCreate)
            {
                if (PGE.Common.Delivery.Framework.ModelNameFacade.ModelNameManager.ContainsClassModelName(objectClass, SchemaInfo.General.ClassModelNames.AssetReplacementAbandonedMN))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// This AU is soley to provide the necessary information to the PGE_Asset_Replacement D8 tree tool to be able to
        /// related the two features to each other.  No actual editing is done with this AU.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="eAUMode"></param>
        /// <param name="eEvent"></param>
        protected override void InternalExecute(IObject obj, Miner.Interop.mmAutoUpdaterMode eAUMode, Miner.Interop.mmEditEvent eEvent)
        {
            abandonedTable = obj.Class;
            newAbandonedRow = obj;
        }
    }
}
