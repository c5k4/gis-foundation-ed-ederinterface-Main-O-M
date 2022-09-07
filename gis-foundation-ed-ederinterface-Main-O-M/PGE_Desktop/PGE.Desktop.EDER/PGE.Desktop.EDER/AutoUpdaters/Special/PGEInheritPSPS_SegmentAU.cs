using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Miner.ComCategories;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Diagnostics;
using System.Reflection;
using ESRI.ArcGIS.Geodatabase;
using Miner.Interop;
using PGE.Common.Delivery.Framework;

namespace PGE.Desktop.EDER.AutoUpdaters.Special
{
    [ComVisible(true)]
    [Guid("8082F002-61B0-4D45-8B0C-EE8867EEA96A")]
    [ProgId("PGE.Desktop.EDER.PGEInheritPSPS_SegmentAU")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    public class PGEInheritPSPS_SegmentAU : BaseSpecialAU
    {
        
        #region Private Static readonly fields
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        #endregion

        #region Constructor
 
        public PGEInheritPSPS_SegmentAU()
            : base("PGE Inherit PSPS Segment AU") {}

        #endregion

        #region Base special AU Overrides
        
        protected override bool InternalEnabled(IObjectClass objectClass, mmEditEvent eEvent)
        {
            bool priOHenabled = false;
            bool priUGenabled = false;
            bool distBusBar = false;
            bool AUenable = false;
            if (eEvent == mmEditEvent.mmEventFeatureCreate)
            {
                
                priOHenabled = ModelNameFacade.ContainsClassModelName(objectClass, SchemaInfo.Electric.ClassModelNames.PrimaryOHConductor);
                priUGenabled = ModelNameFacade.ContainsClassModelName(objectClass, SchemaInfo.Electric.ClassModelNames.PGEPriUGConductor);
                //After UAT
                distBusBar = ModelNameFacade.ContainsClassModelName(objectClass, SchemaInfo.Electric.ClassModelNames.PGEBusBar);
                _logger.Debug("field model name :" + SchemaInfo.Electric.ClassModelNames.PrimaryOHConductor + " Found-" + priOHenabled);
                if (priOHenabled || priUGenabled || distBusBar)
                {
                    AUenable = true;
                }
            }
            return AUenable;
        }

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
            // check if event is create
            if (eEvent == mmEditEvent.mmEventFeatureCreate)
            {
                // if event is update then check whether shape is changed or not.
                // if shape is changed then inherit PSPS from connected source line.
                // else do nothing.
                if (eEvent == mmEditEvent.mmEventFeatureUpdate)
                {
                    IFeatureChanges objectFeatureChanges = obj as IFeatureChanges;
                    if (objectFeatureChanges.ShapeChanged)
                    {
                        //inherit PSPS_Segment value
                        UpdatePSPS_Segment(obj);
                    }
                }
                else if (eEvent == mmEditEvent.mmEventFeatureCreate) // Feature create event
                {
                    // inherit PSPS_Segment value
                    UpdatePSPS_Segment(obj);
                }
            }
        }
        #endregion

        private void UpdatePSPS_Segment(IObject obj)
        {
            //Checking whether the obj geometry is point type or line type
            Boolean isPointFeature = ((obj as IFeature).Shape.GeometryType == ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPoint);
            //if obj is Point
            if (isPointFeature)
            {
                //UpdatePoint(obj);
            }
            else
            {
                UpdateLine(obj);
            }
        }

        private void UpdateLine(IObject obj)
        {

            IFeature connectedUpstreamFeatureLine = TraceFacade.GetFirstUpstreamEdge(obj as IFeature);
            IField PSPS_SegmentField = ModelNameFacade.FieldFromModelName(obj.Class, SchemaInfo.Electric.FieldModelNames.PGE_PSPS_SEGMENT);
            //Default Value as per Requirement
            object PSPS_Segment = "N/A";
            if (connectedUpstreamFeatureLine != null)
            {

                PSPS_Segment = connectedUpstreamFeatureLine.get_Value(connectedUpstreamFeatureLine.Fields.FindField(PSPS_SegmentField.Name));

            }
           //Updating the PSPS Segment Map value
            obj.set_Value(obj.Fields.FindField(PSPS_SegmentField.Name), PSPS_Segment);

        }
    }
}
