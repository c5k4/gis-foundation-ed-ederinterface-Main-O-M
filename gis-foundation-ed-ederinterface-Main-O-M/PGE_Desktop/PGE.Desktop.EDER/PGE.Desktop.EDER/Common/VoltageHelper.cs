using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using PGE.Common.Delivery.Framework;
using PGE.Common.Delivery.Diagnostics;
using PGE.Common.Delivery.Systems.Configuration;

namespace PGE.Desktop.EDER
{
    /// <summary>
    /// Helper Class for Operating Voltage related methods and Voltage drop.
    /// </summary>
    public static class VoltageHelper
    {
        
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        
        #region Class Configuration Properties
        private static VoltageConfig _voltageDropConfigurations = null;
        private static VoltageDrop _primaryVoltageDropConfiguration = null;
      

        /// <summary>
        /// Gets VoltageDrop configuration.
        /// </summary>
        public static VoltageConfig VoltageDropConfiguration
        {
            get
            {
                if (_voltageDropConfigurations == null)
                {
                    LoadConfiguration();
                }

                return _voltageDropConfigurations;
            }
        }
        /// <summary>
        /// Gets PrimaryVoltageDrop configuration.
        /// </summary>
        public static VoltageDrop PrimaryVoltageDropConfiguration
        {
            get
            {
                if (_primaryVoltageDropConfiguration == null)
                {
                    LoadConfiguration();
                }

                return _primaryVoltageDropConfiguration;
            }
        }
        #endregion Class Properties
        
        #region Public Methods

        /// <summary>
        /// Gets the Voltage Drop Code(single-phase) given the circuit voltage code (multi-phase).
        /// </summary>
        /// <param name="voltage">Circuit Voltage Code (Nominal Voltage)</param>
        /// <returns>int-Voltage Drop Code</returns>
        public static int GetValidPrimaryVoltageDropCode(int voltage)
        {
            int returnVal = -1;

            var matchingCircuitVoltage = PrimaryVoltageDropConfiguration.Voltages.Where(cv => cv.CircuitVoltageCode == voltage).FirstOrDefault();
            if (matchingCircuitVoltage != null)
            {
                returnVal = matchingCircuitVoltage.VoltageDropCode;
            }
            return returnVal;
        }

        /// <summary>
        ///  Gets the Voltage Drop Values(single-phase) given the circuit voltage code (multi-phase).
        /// </summary>
        /// <param name="voltage">Circuit Voltage Code (Nominal Voltage)</param>
        /// <param name="voltageDrop">out Voltage Drop Code</param>
        /// <param name="voltageDropDescription">out Voltage Drop Description</param>
        public static void GetValidPrimaryVoltageDrop(int voltage, out int voltageDrop, out string voltageDropDescription)
        {


            var matchingCircuitVoltage = PrimaryVoltageDropConfiguration.Voltages.Where(primaryVoltageConfig => primaryVoltageConfig.CircuitVoltageCode == voltage).FirstOrDefault();
            if (matchingCircuitVoltage != null)
            {
                voltageDrop = matchingCircuitVoltage.VoltageDropCode;
                voltageDropDescription = matchingCircuitVoltage.VoltageDropDescription;
            }
            else
            {
                voltageDrop = -1;
                voltageDropDescription = "";
            }

        }

        /// <summary>
        ///  Gets the circuit voltage values (multi-phase) given the voltage drop code (single-phase).
        /// </summary>
        /// <param name="voltage">Voltage Drop Code (SinglePhase Voltage)</param>
        /// <param name="circuitVoltage">out Circuit Voltage Code</param>
        /// <param name="circuitVoltageDescription">out Circuit Voltage Description</param>
        public static void GetValidPrimaryCircuitVoltage(int voltage, out int circuitVoltage, out string circuitVoltageDescription)
        {


            var matchingCircuitVoltage = PrimaryVoltageDropConfiguration.Voltages.Where(primaryVoltageConfig => primaryVoltageConfig.VoltageDropCode == voltage).FirstOrDefault();
            if (matchingCircuitVoltage != null)
            {
                circuitVoltage = matchingCircuitVoltage.CircuitVoltageCode;
                circuitVoltageDescription = matchingCircuitVoltage.CircuitVoltageDescription;
            }
            else
            {
                circuitVoltage = -1;
                circuitVoltageDescription = "";
            }

        }
        /// <summary>
        /// Gets the circuit voltage code (multi-phase) given the voltage drop code (single-phase).
        /// </summary>
        /// <param name="voltage">Voltage Drop Code (SinglePhase Voltage)</param>
        /// <returns>int-Circuit PrimaryVoltage Code value</returns>
        public static int GetValidPrimaryCircuitVoltageCode(int voltage)
        {
            int returnVal = -1;
            var matchingCircuitVoltage = PrimaryVoltageDropConfiguration.Voltages.Where(primaryVoltageConfig => primaryVoltageConfig.VoltageDropCode == voltage).FirstOrDefault();
            if (matchingCircuitVoltage != null)
            {
                returnVal = matchingCircuitVoltage.CircuitVoltageCode;
            }
            return returnVal;
        }

        /// <summary>
        /// Determines if voltage is available for multi-phase.
        /// </summary>
        /// <param name="voltage"></param>
        /// <returns>true or false</returns>
        public static bool IsMultiPhaseVoltageValid(int voltage)
        {
            bool returnVal = false;

            var matchingCircuitVoltage = PrimaryVoltageDropConfiguration.Voltages.Where(cv => cv.CircuitVoltageCode == voltage).FirstOrDefault();
            if (matchingCircuitVoltage != null)
            {
                returnVal = true;
            }
            return returnVal;
        }

        /// <summary>
        /// Determines if voltage is available for single-phase.
        /// </summary>
        /// <param name="voltage"></param>
        /// <returns>true or false</returns>
        public static bool IsSinglePhaseVoltageValid(int voltage)
        {
            bool returnVal = false;

            var matchingCircuitVoltage = PrimaryVoltageDropConfiguration.Voltages.Where(primaryVoltageConfig => primaryVoltageConfig.VoltageDropCode == voltage).FirstOrDefault();
            if (matchingCircuitVoltage != null)
            {
                returnVal = true;
            }
            return returnVal;
        }
#endregion Public Methods

        #region Private Methods
        /// <summary>
        /// Loads VoltageConfig.xml file.
        /// </summary>
        private static void LoadConfiguration()
        {
            _logger.Debug("VoltageHelper starting LoadConfigration.");
            // Read the config location from the Registry Entry.
            SystemRegistry sysRegistry = new SystemRegistry("PGE");
            string voltageConfigPath = sysRegistry.ConfigPath;
            _logger.Debug("PGE Registry Config path is: " + voltageConfigPath);
            // Get file path name.
            string xmlfilepath = System.IO.Path.Combine(voltageConfigPath, "VoltageConfig.xml");

            if (!System.IO.File.Exists(xmlfilepath))
            {
                _logger.Warn("Xml File path doesn't exist: " + xmlfilepath);
                string assemblyLoc = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                xmlfilepath = System.IO.Path.Combine(assemblyLoc, "VoltageConfig.xml");
            }

            if (System.IO.File.Exists(xmlfilepath))
            {

                XmlSerializer serial = new XmlSerializer(typeof(VoltageConfig), new XmlRootAttribute("voltageconfig"));
                using (System.IO.StreamReader reader = new System.IO.StreamReader(xmlfilepath))
                {
                    _voltageDropConfigurations = (VoltageConfig)serial.Deserialize(reader);

                }
                _primaryVoltageDropConfiguration = _voltageDropConfigurations.VoltageDropConfigs.Where(voltagedropConfigName => voltagedropConfigName.Name == "Primary Voltage").FirstOrDefault();
            }
            else
            {
                _logger.Error("VoltageHelper Missing VoltageConfig.xml file: " + xmlfilepath);
            }

        }
        #endregion Private Methods
    }

    #region Configuration Classes
    /// <summary>
    /// Xml VoltageConfig object.
    /// </summary>
    [XmlRoot(ElementName = "voltageconfig")]
    public class VoltageConfig
    {
        [XmlElement(ElementName = "voltagedrop")]
        public List<VoltageDrop> VoltageDropConfigs { get; set; }
    }

    /// <summary>
    /// Xml VoltageDrop object.
    /// </summary>
    [XmlRoot(ElementName = "voltagedrop")]
    public class VoltageDrop
    {
        [XmlElement(ElementName = "voltage")]
        public List<Voltage> Voltages { get; set; }

        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
    }
    /// <summary>
    /// Xml Voltage object.
    /// </summary>
    [XmlRoot(ElementName = "voltage")]
    public class Voltage
    {
        [XmlAttribute(AttributeName = "circuitvoltagecode")]
        public int CircuitVoltageCode { get; set; }

        [XmlAttribute(AttributeName = "circuitvoltagedescription")]
        public string CircuitVoltageDescription { get; set; }

        [XmlAttribute(AttributeName = "voltagedropcode")]
        public int VoltageDropCode { get; set; }

        [XmlAttribute(AttributeName = "voltagedropdescription")]
        public string VoltageDropDescription { get; set; }


    }
    #endregion Configuration Objects
}
