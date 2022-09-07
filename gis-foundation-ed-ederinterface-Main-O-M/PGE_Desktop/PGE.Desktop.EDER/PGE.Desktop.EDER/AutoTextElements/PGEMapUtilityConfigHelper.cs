using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using PGE.Common.Delivery.Systems.Configuration;

namespace PGE.Desktop.EDER
{
    #region AutoTextElement XML File Structure Classes

    /// <summary>
    /// Holds the settings for AutoTextElements
    /// </summary>
    [XmlRoot("ATE")]
    public class ATEConfig
    {
        /// <summary>
        /// Holds the collection of settings
        /// </summary>
        [XmlElement("Add")]
        public List<Setting> Settings = new List<Setting>();
    }

    /// <summary>
    /// Holds the Name and Value of each settings
    /// </summary>
    public class Setting
    {
        /// <summary>
        /// Name of the setting
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("Name")]
        public string Name { get; set; }
        /// <summary>
        /// Value of the setting
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("Value")]
        public string Value { get; set; }
    }
    #endregion

    /// <summary>
    /// PGEMap Utility ConfigHelper the reads the config file of AutoTexElements
    /// </summary>
    public class PGEMapUtilityConfigHelper
    {
        /// <summary>
        /// Buffer Diameter mentoned in the config file
        /// </summary>
        public double BufferDiameter;
        /// <summary>
        /// Buffer Transparancy mentoned in the config file
        /// </summary>
        public byte BufferTransperancy;
        /// <summary>
        /// Buffer border color mentioned in the config file
        /// </summary>
        public string BufferBorderColorRGB;
        /// <summary>
        /// Buffer fill color mentioned in the config file
        /// </summary>
        public string BufferFillColorRGB;
        /// <summary>
        /// Buffer border width mentioned in the config file
        /// </summary>
        public double BufferBorderWidth;
        /// <summary>
        /// Buffer fill style mentioned in the config file
        /// </summary>
        public int BufferFillStyle;

        /// <summary>
        /// Holds the collection of all settings in the ATE config file
        /// </summary>
        public List<Setting> Settings;

        public bool Valid { get; set; }

        /// <summary>
        /// Creates an instacne of <see cref="PGEMapUtilityConfigHelper"/>
        /// </summary>
        public PGEMapUtilityConfigHelper()
        {
            Valid = false;
            try
            {
                //Read the config location from the Registry Entry
                SystemRegistry sysRegistry = new SystemRegistry("PGE");
                string _xmlFilePath = sysRegistry.ConfigPath;
                //prepare the xmlfile if config path exist
                string ateXmlFilePath = System.IO.Path.Combine(_xmlFilePath, "PGEMapUtiltiesConfig.xml");

                if (!System.IO.File.Exists(ateXmlFilePath)) { Valid = false; return; }
                ///Deserialize the config file
                XmlSerializer deserializer = new XmlSerializer(typeof(ATEConfig));
                TextReader reader = new StreamReader(ateXmlFilePath);
                ATEConfig config = (ATEConfig)deserializer.Deserialize(reader);
                reader.Close();

                //Get all settings and values
                this.Settings = config.Settings;
                //Lopping through the objects and setting the value of the Buffer

                foreach (Setting setting in config.Settings)
                {
                    //if (setting.Name == "BufferDiameter") BufferDiameter = Convert.ToDouble(setting.Value);
                    if (setting.Name == "BufferTransperancy") BufferTransperancy = Convert.ToByte(setting.Value);
                    if (setting.Name == "BufferBorderColorRGB") BufferBorderColorRGB = setting.Value;
                    if (setting.Name == "BufferFillColorRGB") BufferFillColorRGB = setting.Value;
                    if (setting.Name == "BufferBorderWidth") BufferBorderWidth = Convert.ToDouble(setting.Value);
                    if (setting.Name == "BufferFillStyle") BufferFillStyle = Convert.ToInt32(setting.Value);
                }
                Valid = true;
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                Valid = false;
            }
        }
    }

}
