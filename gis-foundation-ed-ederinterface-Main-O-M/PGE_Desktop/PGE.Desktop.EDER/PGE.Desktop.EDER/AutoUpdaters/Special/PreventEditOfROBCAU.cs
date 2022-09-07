// ========================================================================
// Copyright © 2006 Telvent, Consulting Engineers, Inc.
// <history>
// Data Validation-Misc XFR - EDER Component Specification
// Shaikh Rizuan 10/11/2012	Created
// </history>
// All rights reserved.
// ========================================================================
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using log4net;
using Miner.ComCategories;
using Miner.Interop;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Framework;
using ESRI.ArcGIS.esriSystem;
using System.Linq;
using PGE.Common.Delivery.Diagnostics;

namespace PGE.Desktop.EDER.AutoUpdaters.Special
{
    /// <summary>
    ///  used populate the As-Built SourceSide field on the Voltage Regulator featureclass.
    /// </summary>
    [ComVisible(true)]
    [Guid("06ED0E14-156A-498D-9740-A7A8890D3954")]
    [ProgId("PGE.Desktop.EDER.PreventEditOfROBCAU")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    public class PreventEditOfROBCAU : BaseSpecialAU
    {
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        #region
        /// <summary>
        /// Constructor, pass in name.
        /// </summary>
        /// 
        public PreventEditOfROBCAU()
            : base("PGE Prevent Edit of ROBC")
        {

        }

        #endregion

        #region Base Special AU Overrides
        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectClass"> Object's class. </param>
        /// <param name="eEvent">The edit event.</param>
        /// <returns> <c>true</c> if the AuoUpdater should be enabled; otherwise <c>false</c> </returns>
        protected override bool InternalEnabled(IObjectClass objectClass, mmEditEvent eEvent)
        {
            bool enabled = false;

            if (eEvent == mmEditEvent.mmEventFeatureCreate || eEvent == mmEditEvent.mmEventFeatureUpdate || eEvent == mmEditEvent.mmEventFeatureDelete)
            {
                string[] strModelNames = new string[] { SchemaInfo.Electric.ClassModelNames.ROBC, SchemaInfo.Electric.ClassModelNames.PartialCurtailPoint};
                enabled = ModelNameFacade.ContainsClassModelName(objectClass, strModelNames);
                _logger.Debug("Class model name: " + SchemaInfo.Electric.ClassModelNames.ROBC + "," + SchemaInfo.Electric.ClassModelNames.PartialCurtailPoint + "Found -" + enabled);
            }

            return enabled;
        }

        protected override bool CanExecute(mmAutoUpdaterMode eAUMode)
        {

            return true;
        }

        protected override void InternalExecute(IObject obj, mmAutoUpdaterMode eAUMode, mmEditEvent eEvent)
        {
            if (eEvent == mmEditEvent.mmEventFeatureCreate || eEvent == mmEditEvent.mmEventFeatureUpdate || eEvent == mmEditEvent.mmEventFeatureDelete)
            {
                //IObjectClass objClass = obj.Class;
                //IDataset dataset = objClass as IDataset;

                //IWorkspaceEdit wsEdit = (dataset.Workspace) as IWorkspaceEdit;
                //wsEdit.AbortEditOperation();


                throw new COMException(obj.Class.AliasName + " cannot be edited.", (int)mmErrorCodes.MM_E_CANCELEDIT);
            }

        }
        #endregion
    }
}

