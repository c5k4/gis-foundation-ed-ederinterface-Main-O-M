using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;

namespace PGE.Interfaces.AutoCreateWIPPolygon.UtilityClasses
{
    class Elevation
    {
        public double getElevation2(IPoint pPoint,IRasterLayer pRasterLayer)
        {

            try
            { 
                    
                  
                    IIdentify _pIdentify = (IIdentify)pRasterLayer;
                
                return GetHeightAtPoint(pPoint, _pIdentify);

            }
            catch (Exception ex)
            {
                Common._log.Error("Error returning while getting the elevation of the given point"); 
                throw ex; 
            }
        }
        private double GetHeightAtPoint(IPoint pPoint, IIdentify pIdentify)
        {
            try
            {

                double rasterValue = -1;

                //Get RasterIdentifyObject on that point 
                IArray pIDArray = pIdentify.Identify(pPoint);
                if (pIDArray != null)
                {
                    IRasterIdentifyObj pRIDObj = (IRasterIdentifyObj)pIDArray.get_Element(0);
                    rasterValue = Convert.ToDouble(pRIDObj.Name);
                }

                return Math.Round(rasterValue, 1);
            }
            catch (Exception ex)
            {
                //_logger.Error("Error returning elevation at given point", ex);
                throw new Exception("Error returning the height at point");
            }
        }
    }
}
