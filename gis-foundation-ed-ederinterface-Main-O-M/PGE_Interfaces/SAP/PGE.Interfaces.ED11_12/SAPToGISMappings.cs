using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PGE.Common.Delivery.Systems.Configuration;
using System.Reflection;
using System.IO;
using System.Xml;
using PGE.Common.Delivery.Diagnostics;

namespace PGE.Interfaces.ED11_12
{
    public class SAPToGISMappings
    {
        /// <summary>
        /// Logs error/ custom information
        /// </summary>
        private static Log4NetLogger _log = new Log4NetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "PGE.Interfaces.ED11_12.log4net.config");

        public static SAPToGISMappings Instance = new SAPToGISMappings();
        private SAPToGISMappings() 
        {
            Initialize();
        }

        /// <summary>
        /// Field mapping from SAP to GIS
        /// </summary>
        public Dictionary<string, string> SAPToGISFieldMap = new Dictionary<string, string>();

        /// <summary>
        /// Domain mapping from SAP to GIS
        /// </summary>
        public Dictionary<string, Dictionary<string, string>> SAPToGISDomainMap = new Dictionary<string, Dictionary<string, string>>();

        /// <summary>
        /// Initialize the SAP to GIS mappings
        /// </summary>
        private void Initialize()
        {
            try
            {
                InitializeDomainMapping();
                InitializeFieldMapping();
            }
            catch (Exception ex)
            {
                _log.Error("Failed to initialize domain and field mappings: " + ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// Initialize the SAP to GIS domain value mapping
        /// </summary>
        private void InitializeDomainMapping()
        {
            string _xmlFilePath = "";

            try
            {
                //Read the config location from the Registry Entry
                SystemRegistry sysRegistry = new SystemRegistry("PGE");
                _xmlFilePath = sysRegistry.ConfigPath;
            }
            catch { }

            //prepare the xmlfile if config path exist
            string fieldMappingFilePath = System.IO.Path.Combine(_xmlFilePath, "Domains.xml");

            if (!File.Exists(fieldMappingFilePath))
            {
                _xmlFilePath = Assembly.GetExecutingAssembly().Location;
                _xmlFilePath = _xmlFilePath.Substring(0, _xmlFilePath.LastIndexOf("\\"));
                fieldMappingFilePath = System.IO.Path.Combine(_xmlFilePath, "Domains.xml");
            }

            if (!File.Exists(fieldMappingFilePath)) { throw new Exception("Unable to find Domains.xml"); }

            try
            {
                //Initialize our field mapping
                XmlDocument doc = new XmlDocument();
                doc.Load(fieldMappingFilePath);

                for (int i = 0; i < doc.DocumentElement.ChildNodes.Count; i++)
                {
                    //Process the domain node
                    XmlNode childNode = doc.DocumentElement.ChildNodes.Item(i);
                    if (childNode is XmlComment) { continue; }

                    string domainName = childNode.Attributes[0].Value;

                    if (!SAPToGISDomainMap.ContainsKey(domainName))
                    {
                        SAPToGISDomainMap.Add(domainName, new Dictionary<string, string>());

                        for (int j = 0; j < childNode.ChildNodes.Count; j++)
                        {
                            XmlNode domainValueMap = childNode.ChildNodes[j];
                            if (domainValueMap is XmlComment) { continue; }

                            //Get this domain mapping
                            XmlAttribute SAPDomainValueAttribute = domainValueMap.Attributes[1];
                            XmlAttribute GISDomainValueAttribute = domainValueMap.Attributes[0];

                            if (!SAPToGISDomainMap[domainName].ContainsKey(SAPDomainValueAttribute.Value))
                            {
                                SAPToGISDomainMap[domainName].Add(SAPDomainValueAttribute.Value, GISDomainValueAttribute.Value);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed processing Domains.xml: " + ex.Message);
            }
        }

        /// <summary>
        /// Initialize the SAP to GIS field mapping
        /// </summary>
        private void InitializeFieldMapping()
        {
            string _xmlFilePath = "";

            try
            {
                //Read the config location from the Registry Entry
                SystemRegistry sysRegistry = new SystemRegistry("PGE");
                _xmlFilePath = sysRegistry.ConfigPath;
            }
            catch { }

            //prepare the xmlfile if config path exist
            string fieldMappingFilePath = System.IO.Path.Combine(_xmlFilePath, "SupportStructureFieldMap.xml");

            if (!File.Exists(fieldMappingFilePath))
            {
                _xmlFilePath = Assembly.GetExecutingAssembly().Location;
                _xmlFilePath = _xmlFilePath.Substring(0, _xmlFilePath.LastIndexOf("\\"));
                fieldMappingFilePath = System.IO.Path.Combine(_xmlFilePath, "SupportStructureFieldMap.xml");
            }

            if (!File.Exists(fieldMappingFilePath)) { throw new Exception("Unable to find SupportStructureFieldMap.xml"); }

            try
            {
                //Initialize our field mapping
                XmlDocument doc = new XmlDocument();
                doc.Load(fieldMappingFilePath);

                for (int i = 0; i < doc.DocumentElement.ChildNodes.Count; i++)
                {
                    XmlNode childNode = doc.DocumentElement.ChildNodes.Item(i);

                    //Get this field name mapping
                    XmlAttribute SAPFieldNameAttribute = childNode.Attributes[0];
                    XmlAttribute GISFieldNameAttribute = childNode.Attributes[1];

                    if (!SAPToGISFieldMap.ContainsKey(SAPFieldNameAttribute.Value.ToUpper()))
                    {
                        SAPToGISFieldMap.Add(SAPFieldNameAttribute.Value.ToUpper(), GISFieldNameAttribute.Value.ToUpper());
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed processing SupportStructureFieldMap.xml: " + ex.Message);
            }
        }

        /// <summary>
        /// Retrieves the GIS field name associated with the SAP field name from our mapping
        /// </summary>
        /// <param name="SAPFieldName"></param>
        /// <returns></returns>
        public string GetGISMappedField(string SAPFieldName)
        {
            if (SAPToGISFieldMap.ContainsKey(SAPFieldName.ToUpper())) { return SAPToGISFieldMap[SAPFieldName.ToUpper()]; }
            else
            {
                _log.Error("Missing field mapping for SAP field " + SAPFieldName);
                throw new Exception("Missing field mapping for SAP field " + SAPFieldName);
            }
        }

        /// <summary>
        /// Returns the appropriate GIS domain value from the specified mapping
        /// </summary>
        /// <param name="domainName"></param>
        /// <param name="SAPDomainValue"></param>
        /// <param name="ErrorIfNotMatched">If false, returns SAPDomainValue if a match isn't found</param>
        /// <returns></returns>
        public string GetGISDomainValue(string domainName, string SAPDomainValue, bool ErrorIfNotMatched)
        {
            if (SAPToGISDomainMap.ContainsKey(domainName))
            {
                if (SAPToGISDomainMap[domainName].ContainsKey(SAPDomainValue))
                {
                    return SAPToGISDomainMap[domainName][SAPDomainValue];
                }
                else if (string.IsNullOrEmpty(SAPDomainValue))
                {
                    //An empty string, let's return an empty string
                    return "";
                }
                {
                    if (ErrorIfNotMatched)
                    {
                        _log.Error(string.Format("No Value {0} found for domain {1}", SAPDomainValue, domainName));
                        throw new Exception(string.Format("No Value {0} found for domain {1}", SAPDomainValue, domainName));
                    }
                    else
                    {
                        //Simply return the SAP Domain Value
                        return SAPDomainValue;
                    }
                }
            }
            else
            {
                _log.Error("Missing domain mapping for domain " + domainName);
                throw new Exception("Missing domain mapping for domain " + domainName);
            }
        }
    }
}
