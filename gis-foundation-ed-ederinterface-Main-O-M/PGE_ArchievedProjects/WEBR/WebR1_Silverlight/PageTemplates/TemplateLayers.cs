using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using ESRI.ArcGIS.Client;

namespace PageTemplates
{
    public class TemplateLayers
    {
        public static void TemplateMap_Loaded(Map TemplateMap, string Name)
        {
            List<string> endpoints = TemplateLayers.GetTemplateConfiguration(Name);
            if (endpoints != null)
            {
                InitializeTemplateLayers(TemplateMap, endpoints);
            }
        }
        public static void InitializeTemplateLayers(Map templateMap, List<string> endpoints)
        {
            ClearMapLayers(templateMap);
            AddMapLayer(templateMap, endpoints);
        }

        /// <summary>
        /// Clears all layers in the template map
        /// </summary>
        /// <param name="map">The template map that will have its layers cleared</param>
        private static void ClearMapLayers(Map map)
        {
            map.Layers.Clear();
        }

        /// <summary>
        /// Adds a new ArcGISDynamicMapServiceLayer to the templatemap with
        /// specific layers set to visible
        /// </summary>
        /// <param name="map">The map to apply the layer(s)</param>
        /// <param name="layers">Map service endpoint string and a string of layers
        /// to me made visible</param>
        private static void AddMapLayer(Map map, List<string> layers)
        {
            int count = layers.Count;

            for (int i = 0; i < count; i++)
            {
                ArcGISDynamicMapServiceLayer newLayer = new ArcGISDynamicMapServiceLayer();
                newLayer.Url = layers[i];
                map.Layers.Add(newLayer);
            }
        }

        /// <summary>
        /// This method reads in configuration settings from the Templates.config
        /// file and gets a map service endpoint string as well as a comma
        /// separated string of layer ids to make visible within the template.
        /// </summary>
        /// <returns>A Tuple containing the map service string and the string
        /// of layer ids.</returns>
        public static List<string> GetTemplateConfiguration(string Name)
        {
            try
            {
                XElement xe = XElement.Load("Templates.config");
                if (!xe.HasElements) return null;
                else
                {
                    List<string> layerServices = new List<string>();

                    var PageTemplateNode = from pt in xe.Descendants("PageTemplate")
                                           where pt.Attribute("Name").Value == Name
                                           select pt;

                    foreach (XElement layer in PageTemplateNode.Elements())
                    {
                        layerServices.Add(layer.Attribute("MapService").Value);
                    }
                    return layerServices;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                return null;
            }
        }
    }
}
