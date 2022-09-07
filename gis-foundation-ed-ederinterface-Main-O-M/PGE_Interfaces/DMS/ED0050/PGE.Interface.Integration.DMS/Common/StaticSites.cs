using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using PGE.Common.Delivery.Framework.Exceptions;
using PGE.Interface.Integration.DMS.Common;

namespace PGE.Interface.Integration.DMS.Common
{
    /// <summary>
    /// Reads in and caches a config file that has the static height and width of a Site based on the device group's
    /// subtype and devicegrouptype
    /// </summary>
    public sealed class StaticSites
    {
        static readonly StaticSites _instance = new StaticSites();
        private Dictionary<int, Dictionary<int, SiteSize>> _data;
        /// <summary>
        /// The lookup dictionary for static sites. Dictionary[subtype code, Dictionary[devicegroup type, Site Size]]
        /// </summary>
        public Dictionary<int, Dictionary<int, SiteSize>> Data
        {
            get { return _data; }
            set { _data = value; }
        }
        /// <summary>
        /// Since there is only one config file we only need one instance of this
        /// </summary>
        public static StaticSites Instance
        {
            get { return StaticSites._instance; }
        }
        /// <summary>
        /// Create a new instance of StaticSites
        /// </summary>
        private StaticSites()
        {
            _data = new Dictionary<int, Dictionary<int, SiteSize>>();
        }
        /// <summary>
        /// Load the configuration from a file
        /// </summary>
        /// <param name="file">The fully qualified path to the file or just the file name if it is in the same directory</param>
        public void Initialize(string file)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(file);
            //use the root node to initialize
            Initialize(xmlDoc.FirstChild);
        }
        /// <summary>
        /// Load the default config file Sites.xml
        /// </summary>
        public void Initialize()
        {
            Initialize("Sites.xml");
        }
        /// <summary>
        /// Load the configuration from an XML node
        /// </summary>
        /// <param name="node">The root XML node that contains the config</param>
        public void Initialize(XmlNode node)
        {
            _data = new Dictionary<int, Dictionary<int, SiteSize>>();

            try
            {
                XmlNodeList values = node.SelectNodes("add");
                if (values != null)
                {
                    foreach (XmlNode value in values)
                    {
                        int subtype = Convert.ToInt32(value.Attributes["subtype"].Value);
                        int devtype = Convert.ToInt32(value.Attributes["devicetype"].Value);
                        double height = Convert.ToDouble(value.Attributes["height"].Value);
                        double width = Convert.ToDouble(value.Attributes["width"].Value);
                        if (!_data.ContainsKey(subtype))
                        {
                            _data.Add(subtype, new Dictionary<int, SiteSize>());
                        }
                        _data[subtype].Add(devtype, new SiteSize(height, width));

                    }
                }
            }
            catch (Exception ex)
            {
                //if there were any errors/exceptions the configuration file is bad
                throw new InvalidConfigurationException("There was an error loading the Sites.xml file.",ex);
            }
        }
        /// <summary>
        /// Look up the Site's static size based on its subtype and devicegrouptype
        /// </summary>
        /// <param name="subtype">The device group's subtype</param>
        /// <param name="devicegroupType">The device group's devicegrouptype</param>
        /// <returns>The static size of that device group/Site</returns>
        public SiteSize GetSize(int subtype, int devicegroupType)
        {
            if (_data.ContainsKey(subtype))
            {
                if (_data[subtype].ContainsKey(devicegroupType))
                {
                    return _data[subtype][devicegroupType];
                }
            }
            return null;
        }
        
    }
    /// <summary>
    /// Data class for storing a Site's size
    /// </summary>
    public class SiteSize
    {
        private double _height;
        private double _width;

        /// <summary>
        /// The height of the Site
        /// </summary>
        public double Height
        {
            get { return _height; }
            set { _height = value; }
        }
        /// <summary>
        /// The width of the Site
        /// </summary>
        public double Width
        {
            get { return _width; }
            set { _width = value; }
        }
        /// <summary>
        /// Create a new blank SiteSize
        /// </summary>
        public SiteSize()
        {

        }
        /// <summary>
        /// Initialize a SiteSize with height and width
        /// </summary>
        /// <param name="height">The height of the Site</param>
        /// <param name="width">The width of the Site</param>
        public SiteSize(double height, double width)
        {
            _height = height;
            _width = width;
        }

    }
        
}
