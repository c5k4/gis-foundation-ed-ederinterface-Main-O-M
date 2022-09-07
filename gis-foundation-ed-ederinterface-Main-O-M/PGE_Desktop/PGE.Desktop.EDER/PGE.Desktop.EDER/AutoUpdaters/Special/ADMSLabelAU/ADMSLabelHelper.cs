using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using ESRI.ArcGIS.Geodatabase;
using PGE.Common.Delivery.Framework;
using PGE.Common.Delivery.Systems.Configuration;

using System.Xml.Serialization;
using System.Linq;
using log4net;
using PGE.Common.Delivery.Diagnostics;

namespace PGE.Desktop.EDER.AutoUpdaters.Special.ADMSLabelAU
{
    public class ADMSLabelHelper
    {
        private static ADMSLabelConfig.Devicegroup _instance = null;
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        public ADMSLabelHelper()
        {
            if (_instance == null)
            {
                initialize();
            }
        }

        public void initialize()
        {
            //Read the config location from the Registry Entry
            SystemRegistry sysRegistry = new SystemRegistry("PGE");
            //get file path name
            string xmlfilepath = System.IO.Path.Combine(sysRegistry.ConfigPath, "ADMSLabelConfig.xml");

#if DEBUG
            xmlfilepath = System.IO.Path.Combine(@"C:\Source\EDER\ED\Desktop\PGE.Desktop.EDER\AutoUpdaters\Special\ADMSLabelAU\Config", "ADMSLabelConfig.xml");
#endif
            if (System.IO.File.Exists(xmlfilepath))
            {
                _instance = new ADMSLabelConfig.Devicegroup();
                XmlSerializer serial = new XmlSerializer(typeof(ADMSLabelConfig.Devicegroup), new XmlRootAttribute("devicegroup"));
                using (System.IO.StreamReader reader = new System.IO.StreamReader(xmlfilepath))
                {
                    _instance = (ADMSLabelConfig.Devicegroup)serial.Deserialize(reader);
                }
            }
            else
            {
                _logger.Warn("DeviceGroup ADMSLabel Missing ADMSLabelConfig.xml file.");
            }
        }

        internal ADMSLabelConfig.Devicegroup ADMSLabel
        {
            get { return _instance; }
        }
    }

    /// <summary>
    /// Class to deserialize xml config file ADMSLabelConfig.xml
    /// </summary>
    public class ADMSLabelConfig
    {
        [XmlRoot(ElementName = "item")]
        public class Item
        {
            [XmlAttribute(AttributeName = "devicetype")]
            public int Devicetype { get; set; }

            [XmlAttribute(AttributeName = "description")]
            public string Description { get; set; }

            [XmlAttribute(AttributeName = "admslabel")]
            public string ADMSLabel { get; set; }

            [XmlAttribute(AttributeName = "status")]
            public int Status { get; set; }
        }

        [XmlRoot(ElementName = "subtype")]
        public class Subtype
        {
            [XmlElement(ElementName = "item")]
            public List<Item> Item { get; set; }
            [XmlAttribute(AttributeName = "name")]
            public int Name { get; set; }
        }

        [XmlRoot(ElementName = "devicegroup")]
        public class Devicegroup
        {
            [XmlElement(ElementName = "subtype")]
            public List<Subtype> Subtype { get; set; }

            /// <summary>
            /// Get the adms label value from the config.xml file based on subtype and devicetype
            /// </summary>
            /// <param name="subtype"></param>
            /// <param name="devicetype"></param>
            /// <returns></returns>
            public KeyValuePair<int, string> GetADMSLabel(int subtype, int devicetype)
            {
                // send back both status and value
                //  if Status is 1 update with value, if it's 0 we need to update the ADMSLabel with DeviceGroupName value in feature (j-box)
                KeyValuePair<int, string> results = new KeyValuePair<int, string>();

                try
                {
                    // select the subtype list
                    var s = this.Subtype.Where(x => x.Name == subtype).First();
                    // select the devicetype row we're looking for
                    var i = s.Item.Where(x => x.Devicetype == devicetype).First();

                    if (i.Status == 1)
                    {
                        // status other then 1 means we don't mess with the attribute
                        results = new KeyValuePair<int, string>(i.Status, i.ADMSLabel);
                        return results;
                    }
                    else
                    {
                        results = new KeyValuePair<int, string>(i.Status, i.ADMSLabel);
                        return results;
                    }
                }
                catch (System.Exception ex)
                {
                    //  sending back random 33 as key, this indicates an error occurred with this function
                    results = new KeyValuePair<int, string>(33, ex.Message);
                    return results;
                }
            } 
        }
    }
}
