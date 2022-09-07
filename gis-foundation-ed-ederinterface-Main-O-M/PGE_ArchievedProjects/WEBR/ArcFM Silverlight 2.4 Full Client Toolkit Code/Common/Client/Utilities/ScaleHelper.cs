using System;

using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Tasks;

#if SILVERLIGHT
namespace Miner.Server.Client
#elif WPF
namespace Miner.Mobile.Client
#endif
{
    static internal class ScaleHelper
    {
        internal static double GetApproximateScale(Map map)
        {
            double scale = double.NaN;

            // Map scale = 1 : (ScreenRes pixels/inch * 39.37 inches/meter * (Map.Resolution * conversionFactorToMeters))
            esriUnits units = GetMapUnits(map);
            if (units == esriUnits.esriUnknownUnits) return double.NaN;

            double mapResInMeters = ConvertUnitstoMeters(map.Resolution, units);
            if (mapResInMeters == double.NaN) return double.NaN;

            const double dotsPerMeter = 96 * 39.37;
            scale = dotsPerMeter * mapResInMeters;

            return scale;
        }

        internal static esriUnits GetMapUnits(Map map)
        {
            esriUnits mapUnits = esriUnits.esriUnknownUnits;

            try
            {
                foreach (Layer layer in map.Layers)
                {
                    if (layer is ArcGISTiledMapServiceLayer)
                    {
                        Enum.TryParse(((ArcGISTiledMapServiceLayer)layer).Units, out mapUnits);
                        break;
                    }

                    if (layer is ArcGISDynamicMapServiceLayer)
                    {
                        Enum.TryParse(((ArcGISDynamicMapServiceLayer)layer).Units, out mapUnits);
                        break;
                    }
                }
            }
            catch
            {
                mapUnits = esriUnits.esriUnknownUnits;
            }

            return mapUnits;
        }

        private static double ConvertUnitstoMeters(double item, esriUnits units)
        {
            const double MilesToMeters = 1609.344;
            const double FeetToMeters = 0.3048;
            const double DDtoMeters = 111319.9; // at the equator, so only approximate

            if (double.IsNaN(item)) return double.NaN;
            double convertedItem = double.NaN;

            switch (units)
            {
                case esriUnits.esriMiles:
                    convertedItem = item * MilesToMeters;
                    break;
                case esriUnits.esriKilometers:
                    convertedItem = item * 1000;
                    break;
                case esriUnits.esriFeet:
                    convertedItem = item * FeetToMeters;
                    break;
                case esriUnits.esriMeters:
                    convertedItem = item;
                    break;
                case esriUnits.esriDecimalDegrees:
                    convertedItem = item * DDtoMeters;
                    break;
                default:
                    break;
            }

            return convertedItem;
        }
    }
}
