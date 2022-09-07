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
    [Guid("8AB49234-E707-4B4A-B3C0-FA0AE30AE744")]
    [ProgId("PGE.Desktop.EDER.PreventDeleteofDeviceswithCustomersAU")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    public class PreventDeleteofDeviceswithCustomersAU : BaseSpecialAU
    {
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

         #region
         /// <summary>
        /// Constructor, pass in name.
        /// </summary>
        /// 
         public PreventDeleteofDeviceswithCustomersAU() : base("PGE Prevent Delete of Devices with Customers")
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

             if (eEvent == mmEditEvent.mmEventFeatureDelete)
             {
                 string[] strModelNames = new string[]{SchemaInfo.Electric.ClassModelNames.Transformer,SchemaInfo.Electric.ClassModelNames.ServiceLocation,SchemaInfo.Electric.ClassModelNames.PrimaryMeter};
                 enabled = ModelNameFacade.ContainsClassModelName(objectClass,strModelNames );
                 _logger.Debug("Class model name: " + SchemaInfo.Electric.ClassModelNames.Transformer + "," + SchemaInfo.Electric.ClassModelNames.ServiceLocation + "," + SchemaInfo.Electric.ClassModelNames.PrimaryMeter + "Found -" + enabled);
             }

             return enabled;
         }

          protected override bool CanExecute(mmAutoUpdaterMode eAUMode)
         {
             if ( eAUMode == mmAutoUpdaterMode.mmAUMArcMap)
             {
                 return true;
             }
             else
             {
                 return false;
             }
         }

          protected override void InternalExecute(IObject obj, mmAutoUpdaterMode eAUMode, mmEditEvent eEvent)
          {
              if (eEvent == mmEditEvent.mmEventFeatureDelete)
              {
                  //ITable tbl = obj.Table;
                 //Get relationship Records from object
                  IEnumerable<IObject> relRecords = obj.GetRelatedObjects(null, esriRelRole.esriRelRoleAny, SchemaInfo.Electric.ClassModelNames.ServicePoint);
                  if (relRecords.Count() > 0)
                  {

                          throw new COMException(obj.Class.AliasName + " with related Service Points cannot be deleted.", (int)mmErrorCodes.MM_E_CANCELEDIT);

                  }
              }
                
          }
    
# endregion
}
}
