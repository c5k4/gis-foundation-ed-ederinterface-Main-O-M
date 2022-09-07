using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using ESRI.ArcGIS.Client.Tasks;
using ESRI.ArcGIS.Client;

namespace ArcFMSilverlight.PageTemplates
{
    internal class Utilities
    {
        internal static esriUnits GetMapUnits(Map templateMap)
        {
            if (templateMap == null) return esriUnits.esriUnknownUnits;

            esriUnits mapUnits = esriUnits.esriUnknownUnits;

            try
            {
                foreach (Layer layer in templateMap.Layers)
                {
                    if (layer is ArcGISTiledMapServiceLayer)
                    {
                        mapUnits = (esriUnits)Enum.Parse(typeof(esriUnits), ((ArcGISTiledMapServiceLayer)layer).Units, false);
                        break;
                    }

                    if (!(layer is ArcGISDynamicMapServiceLayer)) continue;
                    mapUnits = (esriUnits)Enum.Parse(typeof(esriUnits), ((ArcGISDynamicMapServiceLayer)layer).Units, false);
                    break;
                }
            }
            catch
            {
                return mapUnits;
            }

            return mapUnits;
        }
    }
}
