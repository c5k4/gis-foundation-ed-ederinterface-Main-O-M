using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Xml.Serialization;

namespace PGE.Interfaces.Integration.Framework
{
    /// <summary>
    /// Class with generic utility methods
    /// </summary>
    public static class StaticUtilities
    {
        /// <summary>
        /// Load appsettings from xml. This will throw exceptions if the config is invalid.
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Dictionary<string, string> LoadAppSettings(XmlNode config)
        {
            Dictionary<string, string> output = new Dictionary<string, string>();
            if (config != null)
            {
                XmlNode appsettings = config.SelectSingleNode("appsettings");
                if (appsettings != null)
                {
                    XmlNodeList settings = appsettings.SelectNodes("add");
                    foreach (XmlNode setting in settings)
                    {
                        string key = setting.Attributes["key"].Value;
                        string value = setting.Attributes["value"].Value;
                        output.Add(key, value);
                    }
                }

            }
            return output;
        }
        /// <summary>
        /// Convert a string into an XmlNode object. This method will throw an exception if the xml is not valid.
        /// </summary>
        /// <param name="input">XML string</param>
        /// <returns>XmlNode object</returns>
        public static XmlNode String2Node(string input)
        {
            string xmlContent = input;
 
            XmlDocument doc = new XmlDocument(); 
            doc.LoadXml(xmlContent); 
            XmlNode newNode = doc.DocumentElement;
            
            //XmlTextReader textReader = new XmlTextReader(new StringReader(xmlContent)); 
            //XmlDocument myXmlDocument = new XmlDocument(); 
            //XmlNode newNode = myXmlDocument.ReadNode(textReader);

            return newNode;
        }

    }
}
