using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using PGE.Common.Delivery.Framework.Exceptions;
using PGE.Interface.Integration.DMS.Common;

namespace PGE.Interface.Integration.DMS.Processors
{
    /// <summary>
    /// Class used to map a GIS column (field) to a DMS column using a configuration file
    /// </summary>
    public sealed class ColumnMapper
    {
        static readonly ColumnMapper _instance = new ColumnMapper();
        private Dictionary<string, Dictionary<int, List<ColumnMap>>> _data;
        /// <summary>
        /// Column Mapping data. Dictionary[Staging Schema Table Name,Dictionary[FCID,List[ColumnMap]]]
        /// </summary>
        public Dictionary<string, Dictionary<int, List<ColumnMap>>> Data
        {
            get { return _data; }
            set { _data = value; }
        }
        private static Dictionary<string, int> _fcidMap;
        /// <summary>
        /// Since there is only one config file we only need one instance of the ColumnMapper
        /// </summary>
        public static ColumnMapper Instance
        {
            get { return ColumnMapper._instance; }
        }
        /// <summary>
        /// Initialize the ColumnMapper. Does not load config
        /// </summary>
        private ColumnMapper()
        {
            _data = new Dictionary<string, Dictionary<int, List<ColumnMap>>>();
            _fcidMap = new Dictionary<string, int>();
        }
        /// <summary>
        /// Load the configuration from a file
        /// </summary>
        /// <param name="file">The fully qualified path to the file or just a filename if it is in the same directory</param>
        public void Initialize(string file)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(file);
            //use the root node to initialize
            Initialize(xmlDoc.FirstChild);
        }
        /// <summary>
        /// Load the configuration from the default configuration file
        /// </summary>
        public void Initialize()
        {
            Initialize("Columns.xml");
        }
        /// <summary>
        /// Load the configuration from an XML node
        /// </summary>
        /// <param name="node">The XML node that contains the configuration</param>
        public void Initialize(XmlNode node)
        {
            _data = new Dictionary<string, Dictionary<int, List<ColumnMap>>>();
            XmlNodeList DMSlist = node.SelectNodes("DMSTable");
            if (DMSlist == null)
            {
                throw new InvalidConfigurationException("Configuration Error: There must be at least one DMSTable node.");
            }
            foreach (XmlNode DMSTable in DMSlist)
            {
                string name = DMSTable.Attributes["name"].Value;
                
                if (_data.ContainsKey(name))
                {
                    throw new InvalidConfigurationException("DMS Table names can only appear once: " + name);
                }
                Dictionary<int, List<ColumnMap>> tablemap = new Dictionary<int, List<ColumnMap>>();
                _data.Add(name, tablemap);

                XmlNodeList GISlist = DMSTable.SelectNodes("GISTable");
                if (GISlist == null)
                {
                    throw new InvalidConfigurationException("Configuration Error: There must be at least one GISTable node for " + name);
                }
                foreach (XmlNode GISTable in GISlist)
                {
                    string id = GISTable.Attributes["id"].Value;
                    int fcid = getFCID(id);
                    if (fcid == 0)
                    {
                        throw new InvalidConfigurationException("Invalid GIS ID: " + id);
                    }
                    else if (fcid == -1)
                    {
                        //This will occurr if this is substation instead of ED or vice versa. Continue processing next device
                        continue;
                    }
                    List<ColumnMap> mappings = new List<ColumnMap>();
                    tablemap.Add(fcid, mappings);
                    XmlNodeList values = GISTable.SelectNodes("add");
                    foreach (XmlNode value in values)
                    {
                        string GISfield = value.Attributes["GISfield"].Value;
                        string DMSfield = value.Attributes["DMSfield"].Value;
                        if (string.IsNullOrEmpty(GISfield) || string.IsNullOrEmpty(DMSfield))
                        {
                            throw new InvalidConfigurationException("Empty field names not allowed.");
                        }
                        string domain = null;
                        if (value.Attributes["Domain"] != null)
                        {
                            domain = value.Attributes["Domain"].Value;
                        }
                        string dateformat = null;
                        if (value.Attributes["DateFormat"] != null)
                        {
                            dateformat = value.Attributes["DateFormat"].Value;
                        }
                        ColumnMap map = new ColumnMap(GISfield, DMSfield, domain, dateformat);
                        if (value.Attributes["Replace"] != null)
                        {
                            map.SetReplace(value.Attributes["Replace"].Value);
                        }
                        mappings.Add(map);

                    }
                }
            }
        }
        /// <summary>
        /// Look up the FCID for the given configuration name
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private static int getFCID(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return 0;
            }
            switch (id)
            {
                case "Junction":
                    return FCID.Value[FCID.Junction];
                case "PrimaryOH":
                    return FCID.Value[FCID.PrimaryOH];
                case "PrimaryUG":
                    return FCID.Value[FCID.PrimaryUG];
                case "DistBusBar":
                    return FCID.Value[FCID.DistBusBar];
                case "StitchPoint":
                    return FCID.Value[FCID.StitchPoint];
                case "DeviceGroup":
                    return FCID.Value[FCID.DeviceGroup];
                case "Transformer":
                    return FCID.Value[FCID.Transformer];
                case "Capacitor":
                    return FCID.Value[FCID.Capacitor];
                case "Switch":
                    return FCID.Value[FCID.Switch];
                case "StepDown":
                    return FCID.Value[FCID.StepDown];
                case "Fuse":
                    return FCID.Value[FCID.Fuse];
                case "DCRectifier":
                    return FCID.Value[FCID.DCRectifier];
                case "FaultInd":
                    return FCID.Value[FCID.FaultInd];
                case "PrimaryMeter":
                    return FCID.Value[FCID.PrimaryMeter];
                case "DynamicProtectiveDevice":
                    return FCID.Value[FCID.DynamicProtectiveDevice];
                case "VoltageRegulator":
                    return FCID.Value[FCID.VoltageRegulator];
                case "OpenPoint":
                    return FCID.Value[FCID.OpenPoint];
                case "Tie":
                    return FCID.Value[FCID.Tie];
                /*Changes for ENOS to SAP migration - DMS  Changes .. Start */
                //case "PrimaryGen":
                //    return FCID.Value[FCID.PrimaryGen];
                /*Changes for ENOS to SAP migration - DMS  Changes .. End */
                case "FaultIndicator":
                    return FCID.Value[FCID.FaultInd];
                case "SmartMeterNetworkDevice":
                    return FCID.Value[FCID.SmartMeterNetworkDevice];
                case "SUBPrimaryOH":
                    return FCID.Value[FCID.SUBPrimaryOH];
                case "SUBPrimaryUG":
                    return FCID.Value[FCID.SUBPrimaryUG];
                case "SUBBusBar":
                    return FCID.Value[FCID.SUBBusBar];
                case "SUBStitchPoint":
                    return FCID.Value[FCID.SUBStitchPoint];
                case "SUBJunction":
                    return FCID.Value[FCID.SUBJunction];
                case "SUBRiser":
                    return FCID.Value[FCID.SUBRiser];
                case "SUBCurrentTransformer":
                    return FCID.Value[FCID.SUBCurrentTransformer];
                case "SUBPotentialTransformer":
                    return FCID.Value[FCID.SUBPotentialTransformer];
                case "SUBStationTransformer":
                    return FCID.Value[FCID.SUBStationTransformer];
                case "SUBTransformer":
                    return FCID.Value[FCID.SUBTransformer];
                case "SUBSwitch":
                    return FCID.Value[FCID.SUBSwitch];
                case "SUBFuse":
                    return FCID.Value[FCID.SUBFuse];
                case "SUBCapacitor":
                    return FCID.Value[FCID.SUBCapacitor];
                case "SUBCell":
                    return FCID.Value[FCID.SUBCell];
                case "SUBReactor":
                    return FCID.Value[FCID.SUBReactor];
                case "SUBTie":
                    return FCID.Value[FCID.SUBTie];
                case "SUBVoltageRegulator":
                    return FCID.Value[FCID.SUBVoltageRegulator];
                case "SUBInterruptingDevice":
                    return FCID.Value[FCID.SUBInterruptingDevice];
                case "SUBMTU":
                    return FCID.Value[FCID.SUBMTU];
                case "SUBLink":
                    return FCID.Value[FCID.SUBLink];
                case "SUBExtraWinding":
                    return FCID.Value[FCID.SUBExtraWinding];
                case "SUBLightningArrestor":
                    return FCID.Value[FCID.SUBLightningArrestor];
                /*Changes for ENOS to SAP migration - DMS  Changes .. Start */
                case "ServiceLocation":
                    return FCID.Value[FCID.ServiceLocation];
                case "ServicePoint":
                    return FCID.Value[FCID.ServicePoint];
                case "GenerationInfo":
                    return FCID.Value[FCID.GenerationInfo];
                /*Changes for ENOS to SAP migration - DMS  Changes .. End */

            }
            if (_fcidMap.ContainsKey(id))
            {
                return _fcidMap[id];
            }
            return 0;
        }
    }
    /// <summary>
    /// Data class for storing column map settings in the XML file
    /// </summary>
    public class ColumnMap
    {
        private string _GISField;
        private string _DMSField;
        private string _domain;
        private string _dateFormat;
        private Dictionary<string, string> _replace;

        /// <summary>
        /// Initialize the ColumnMap
        /// </summary>
        public ColumnMap()
        {

        }
        /// <summary>
        /// Initialize the ColumnMap
        /// </summary>
        /// <param name="gisfield"></param>
        /// <param name="dmsfield"></param>
        /// <param name="domain"></param>
        /// <param name="dateformat"></param>
        /// <param name="replace"></param>
        public ColumnMap(string gisfield, string dmsfield, string domain, string dateformat, string replace)
        {
            _GISField = gisfield;
            _DMSField = dmsfield;
            _domain = domain;
            _dateFormat = dateformat;
            SetReplace(replace);
        }
        /// <summary>
        /// Initialize the ColumnMap
        /// </summary>
        /// <param name="gisfield"></param>
        /// <param name="dmsfield"></param>
        /// <param name="domain"></param>
        /// <param name="dateformat"></param>
        public ColumnMap(string gisfield, string dmsfield, string domain, string dateformat)
        {
            _GISField = gisfield;
            _DMSField = dmsfield;
            _domain = domain;
            _dateFormat = dateformat;
        }
        /// <summary>
        /// Initialize the ColumnMap
        /// </summary>
        /// <param name="gisfield"></param>
        /// <param name="dmsfield"></param>
        /// <param name="domain"></param>
        public ColumnMap(string gisfield, string dmsfield, string domain)
        {
            _GISField = gisfield;
            _DMSField = dmsfield;
            _domain = domain;
        }
        /// <summary>
        /// Add a replace values for this column. The value \r\n is replaced with a real newline
        /// </summary>
        /// <param name="replace">A comma and semicolon separated list old,new;old,new</param>
        public void SetReplace(string replace)
        {
            _replace = Configuration.getStringMapValue(replace, null);
            string newline = "\\r\\n";
            bool found = false;
            foreach (string key in _replace.Keys)
            {
                if (key.Equals(newline))
                {
                    _replace.Add("\r\n", _replace[newline]);
                    found = true;
                    break;
                }
            }
            if (found)
            {
                _replace.Remove(newline);
            }
        }
        /// <summary>
        /// Dictionary with replacement values old,new, i.e. replace old value with new.
        /// This will be null if there are no values
        /// </summary>
        public Dictionary<string, string> Replace
        {
            get { return _replace; }
            set { _replace = value; }
        }
        /// <summary>
        /// The string format to convert a Date field to.
        /// This will be null if the field is not a Date field
        /// </summary>
        public string DateFormat
        {
            get { return _dateFormat; }
            set { _dateFormat = value; }
        }
        /// <summary>
        /// The name of the coded domain for the GIS field or null if none
        /// </summary>
        public string Domain
        {
            get { return _domain; }
            set { _domain = value; }
        }
        /// <summary>
        /// The name of the field in the DMS staging schema
        /// </summary>
        public string DMSField
        {
            get { return _DMSField; }
            set { _DMSField = value; }
        }
        /// <summary>
        /// The name of the GIS field
        /// </summary>
        public string GISField
        {
            get { return _GISField; }
            set { _GISField = value; }
        }
    }
}
