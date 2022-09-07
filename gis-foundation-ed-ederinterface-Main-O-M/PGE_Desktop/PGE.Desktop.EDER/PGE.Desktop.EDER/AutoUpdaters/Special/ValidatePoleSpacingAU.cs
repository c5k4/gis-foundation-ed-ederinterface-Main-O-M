// ========================================================================
// Copyright © 2006 Telvent, Consulting Engineers, Inc.
// <history>
// Editing - Offset Editing Component Specification v0.3
// Shaikh Rizuan 10/05/2012	Created
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
using System.Linq;
using PGE.Common.Delivery.Diagnostics;

namespace PGE.Desktop.EDER.AutoUpdaters.Special
{
    /// <summary>
    ///  used populate the As-Built SourceSide field on the Voltage Regulator featureclass.
    /// </summary>
    [ComVisible(true)]
    [Guid("A2B3ED49-41C7-4805-B053-B4212A816F2E")]
    [ProgId("PGE.Desktop.EDER.ValidatePoleSpacingAU")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
   public class ValidatePoleSpacingAU: BaseSpecialAU
    {
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
         #region
         /// <summary>
        /// Constructor, pass in name.
        /// </summary>
        /// 
         public ValidatePoleSpacingAU() : base("PGE Validate Pole Spacing AU")
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

             if (eEvent == mmEditEvent.mmEventFeatureCreate || eEvent == mmEditEvent.mmEventFeatureUpdate)
             {
                 enabled = ModelNameFacade.ContainsClassModelName(objectClass, SchemaInfo.Electric.ClassModelNames.SupportStructure);
                 _logger.Debug("Class model name: " + SchemaInfo.Electric.ClassModelNames.SupportStructure + "Found -" + enabled);
             }

             return enabled;
         }

         /// <summary>
         /// Determines whether actually this AU should be run, based on the AU Mode.
         /// </summary>
         /// <param name="eAUMode"> The auto updater mode. </param>
         /// <returns> <c>true</c> if the AuoUpdater should be executed; otherwise <c>false</c> </returns>
         protected override bool CanExecute(mmAutoUpdaterMode eAUMode)
         {
             if (eAUMode == mmAutoUpdaterMode.mmAUMArcMap)
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
             IFeature feat = obj as IFeature;
             IFeatureClass featClass = obj.Class as IFeatureClass;
             IFeatureChanges featChange = feat as IFeatureChanges;
             string objIdArray = string.Empty;
             double initBuffer = 1.0;
             bool fireAU = true;

             if (eEvent == mmEditEvent.mmEventFeatureUpdate)// if event is OnFeatureUpdate
             {
                 fireAU = false;
                 if (featChange.ShapeChanged)// if feature shape changed
                 {
                     fireAU = true;
                 }
             }

             if (fireAU) // if true
             {
                 //Get list of features
                 List<IFeature> featList = GetFeatureListFromBuffer(feat, featClass, initBuffer);
                 List<string> getFeatureOIDList = new List<string>();
                 if (featList.Count > 0)//if count is greater than zero
                 {
                     foreach (IFeature supportStructFeature in featList)//get feature from list
                     {
                         getFeatureOIDList.Add(supportStructFeature.OID.ToString());
                     }
                     objIdArray = string.Join(",", getFeatureOIDList.ToArray());

                     // if support structure feature found within 1 ft range
                     throw new COMException("The SupportStructure (OID: " + objIdArray + ") is within one foot.", (int)mmErrorCodes.MM_E_CANCELEDIT);
                 }
             }
                 
         }

    
/// <summary>
/// Get list of feature in the list which are falling within one feet
/// </summary>
/// <param name="feat">ESRI IFeature</param>
/// <param name="featClass">Feature Class to be searched</param>
/// <param name="initBuffer">distance to search</param>
/// <returns>Return list of features</returns>
         private List<IFeature> GetFeatureListFromBuffer(IFeature feat, IFeatureClass featClass, double initBuffer)
         {
             IFeatureCursor featCursor = null;
             List<IFeature> UpdateFeatLst = new List<IFeature>();
             try
             {
                 //Assign geometry to feature
                 IGeometry geom = feat.ShapeCopy;
                 ITopologicalOperator topoOperator = geom as ITopologicalOperator;

                 //Create a buffer polygon
                 IGeometry buffer = topoOperator.Buffer(initBuffer);
                 _logger.Debug(buffer.GeometryType.ToString());

                 // Create the spatial filter.
                 ISpatialFilter spatialFilter = new SpatialFilterClass();
                 spatialFilter.Geometry = buffer;
                 spatialFilter.GeometryField = featClass.ShapeFieldName;
                 spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                 spatialFilter.SubFields = featClass.OIDFieldName;

                 // Execute the query.
                 featCursor = featClass.Search(spatialFilter, false);
                 IFeature feature;
                 while ((feature = featCursor.NextFeature()) != null) //if feature not = null
                 {
                     if (feature.OID != feat.OID)
                     {
                         UpdateFeatLst.Add(feature);
                     }

                 }
             }
             finally
             {
                 //Release COM object
                 while (Marshal.ReleaseComObject(featCursor) > 0) { }
                 featCursor = null;
             }
             return UpdateFeatLst;

         }
#endregion


    }
}
