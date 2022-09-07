using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Miner.ComCategories;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Diagnostics;
using System.Diagnostics;
using System.Reflection;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using Miner.Interop;
using PGE.Common.Delivery.Framework;

namespace PGE.Desktop.EDER.AutoUpdaters
{
    [Guid("8D3461BD-C57A-41C2-9BB5-3AB669ED54D8")]
    [ProgId("PGE.Desktop.EDER.AutoUpdaters.SecOHConductorInfoOnDeleteAU")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    public class SecOHConductorInfoOnDeleteAU : BaseSpecialAU
    {
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        public SecOHConductorInfoOnDeleteAU()
            : base("PGE SecConductor FieldUpdate AU")
        {
        }

        #region Base special AU Overrides
        /// <summary>
        /// Determines when the AU should be enabled 
        /// </summary>
        /// <param name="objectClass"> Object's class. </param>
        /// <param name="eEvent">The edit event.</param>
        /// <returns> <c>true</c> if the AuoUpdater should be enabled; otherwise <c>false</c> </returns>
        protected override bool InternalEnabled(IObjectClass objectClass, mmEditEvent eEvent)
        {
            bool isEnabled = false;
            if (eEvent == mmEditEvent.mmEventFeatureDelete)
                isEnabled = true;
            return isEnabled;
        }

        /// <summary>
        /// Determines whether actually this AU should be run, based on the AU Mode.
        /// </summary>
        /// <param name="eAUMode"> The auto updater mode. </param>
        /// <returns> <c>true</c> if the AuoUpdater should be executed; otherwise <c>false</c> </returns>
        //protected override bool CanExecute(mmAutoUpdaterMode eAUMode)
        //{
        //    if (eAUMode == mmAutoUpdaterMode.mmAUMArcMap)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        /// <summary>
        /// Implementation of AutoUpdater Execute Ex method for derived classes.
        /// </summary>
        /// <param name="obj">The object that triggered the AutoUpdater.</param>
        /// <param name="eAUMode">The auto updater mode.</param>
        /// <param name="eEvent">The edit event.</param>
        protected override void InternalExecute(IObject obj, mmAutoUpdaterMode eAUMode, mmEditEvent eEvent)
        {
            if (eEvent != mmEditEvent.mmEventFeatureDelete)
            return;
            string msg = "Deleting " + obj.Class.AliasName + " with oid: " +
            obj.OID.ToString() + " in " + base._Name;
            _logger.Debug(msg);

            try
            {
                var field = obj.Fields.FindField("CONDUCTORSIZE");
                try
                {
                    obj.set_Value(field, "$U" + obj.get_Value(obj.Fields.FindField("CONDUCTORSIZE")));
                }
                catch { }
                obj.Store();
            }
            catch (Exception ex)
            {
                _logger.Error("PG&E Update DPD Recloser CCRating , Max Interrupting Current & Interruptinc medium autoupdator ", ex);
            }
            finally
            {

            }
        }
        #endregion

     
    }
}
