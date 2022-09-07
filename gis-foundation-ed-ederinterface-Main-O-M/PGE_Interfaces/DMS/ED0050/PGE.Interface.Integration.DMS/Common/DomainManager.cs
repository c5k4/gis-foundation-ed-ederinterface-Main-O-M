// ========================================================================
// Company: Telvent USA Inc., Telvent Utilities Group
// Client : PG&E
// $Revision: 1 $
// ========================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using PGE.Common.Delivery.Framework.Exceptions;

namespace PGE.Common.Delivery.Framework
{
    /// <summary>
    /// Looks up field mappings from GIS to other systems.
    /// </summary>
    public sealed class DomainManager
    {
        static readonly DomainManager _instance = new DomainManager();
        private Dictionary<string, Dictionary<string, Dictionary<string, string>>> _allDomains;
        private Dictionary<string, Dictionary<string, Dictionary<string, int>>> _intDomains;
        private string _default;

        /// <summary>
        /// Default integration to use if one is not specified in other method calls.
        /// </summary>
        public string DefaultIntegration
        {
            get { return _default; }
            set { _default = value; }
        }

        /// <summary>
        /// The single instance of the Domain Manager
        /// </summary>
        public static DomainManager Instance
        {
            get { return DomainManager._instance; }
        }

        private DomainManager()
        {
            _allDomains = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();
            _intDomains = new Dictionary<string, Dictionary<string, Dictionary<string, int>>>();
        }

        /// <summary>
        /// Gets the mapped value
        /// </summary>
        /// <param name="domain">The name of the domain mapping</param>
        /// <param name="value">The input value</param>
        /// <returns>The mapped value</returns>
        public string GetValue(string domain, string value)
        {
            if (_default != null)
            {
                return GetValue(domain, value, _default);
            }
            return null;
        }

        /// <summary>
        /// Check if the domain exists using the default integration
        /// </summary>
        /// <param name="domain">The name of the domain</param>
        /// <returns>True if the domain exists</returns>
        public bool ContainsDomain(string domain)
        {
            return ContainsDomain(domain, _default);
        }
        /// <summary>
        /// Check if the domain exists using the specified integration
        /// </summary>
        /// <param name="domain">The name of the domain</param>
        /// <param name="integration">The name of the integration e.g. DMS</param>
        /// <returns>True if the domain exists</returns>
        public bool ContainsDomain(string domain, string integration)
        {
            return _allDomains[integration].ContainsKey(domain);
        }
        /// <summary>
        /// Gets the mapped value
        /// </summary>
        /// <param name="domain">The name of the domain mapping</param>
        /// <param name="value">The input value</param>
        /// <param name="integration">The name of the integration to use</param>
        /// <returns>The mapped value</returns>
        public string GetValue(string domain, string value, string integration)
        {
            string lookup = value;
            if (lookup == null)
            {
                lookup = "{null}";
            }
            if (_allDomains.ContainsKey(integration))
            {
                if (_allDomains[integration].ContainsKey(domain))
                {
                    if (_allDomains[integration][domain].ContainsKey(lookup))
                    {
                        return _allDomains[integration][domain][lookup];
                    }
                    else//check if there is a default value
                    {
                        if (_allDomains[integration][domain].ContainsKey("{default}"))
                        {
                            return _allDomains[integration][domain]["{default}"];
                        }
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// Get the mapped value for the default integration
        /// </summary>
        /// <param name="domain">The name of the domain mapping</param>
        /// <param name="value">The input value</param>
        /// <returns>The mapped value</returns>
        public int? GetIntValue(string domain, string value)
        {
            return GetIntValue(domain,value,_default);
        }
        /// <summary>
        /// Gets the mapped value
        /// </summary>
        /// <param name="domain">The name of the domain mapping</param>
        /// <param name="value">The input value</param>
        /// <param name="integration">The name of the integration to use</param>
        /// <returns>The mapped value</returns>
        public int? GetIntValue(string domain, string value, string integration)
        {
            string lookup = value;
            if (lookup == null)
            {
                lookup = "{null}";
            }
            if (_intDomains.ContainsKey(integration))
            {
                if (_intDomains[integration].ContainsKey(domain))
                {
                    if (_intDomains[integration][domain].ContainsKey(lookup))
                    {
                        return _intDomains[integration][domain][lookup];
                    }
                    else//check if there is a default value
                    {
                        if (_intDomains[integration][domain].ContainsKey("{default}"))
                        {
                            return _intDomains[integration][domain]["{default}"];
                        }
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// Initialize or reinitialize the domain manager from a file
        /// </summary>
        /// <param name="file">The path and file name</param>
        public void Initialize(string file)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(file);
            //use the root node to initialize
            Initialize(xmlDoc.FirstChild);
        }
        /// <summary>
        /// Initialize or reinitialize the domain manager from an XML node
        /// </summary>
        /// <param name="node">An XML node with domain mappings</param>
        public void Initialize(XmlNode node)
        {
            XmlNodeList list = node.SelectNodes("DomainMapping");
            foreach (XmlNode domainXml in list)
            {
                string name = domainXml.Attributes["name"].Value;
                LoadDomains(domainXml, name);
            }
        }
        /// <summary>
        /// Loads domains from a file. This method will throw IO and XML exceptions. It is up to the caller
        /// to handle any exceptions.
        /// </summary>
        /// <param name="file">The location of the file.</param>
        /// <param name="integration">The name of the integration values to load, ie. SAP.</param>
        public void Initialize(string file, string integration)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(file);
            //use the root node to initialize
            Initialize(xmlDoc.FirstChild, integration);
        }
        /// <summary>
        ///  Loads domains from XML.
        /// </summary>
        /// <param name="node">An XML node</param>
        /// <param name="integration">The name of the integration values to load, ie. SAP.</param>
        public void Initialize(XmlNode node, string integration)
        {
            _default = integration;
            XmlNode domainRoot = node.SelectSingleNode("DomainMapping[@name = \"" + integration + "\"]");
            if (domainRoot == null)
            {
                throw new InvalidConfigurationException("Could not find domain mappings for (" + integration + ").");
            }
            LoadDomains(domainRoot, integration);

        }
        private void LoadDomains(XmlNode node, string integration)
        {
            Dictionary<string, Dictionary<string, string>> domains = null;
            Dictionary<string, Dictionary<string, int>> intdomains = null;
            if (_allDomains.ContainsKey(integration))
            {
                domains = _allDomains[integration];
                domains.Clear();
            }
            else
            {
                domains = new Dictionary<string, Dictionary<string, string>>();
                _allDomains.Add(integration, domains);
            }
            if (_intDomains.ContainsKey(integration))
            {
                intdomains = _intDomains[integration];
                intdomains.Clear();
            }
            else
            {
                intdomains = new Dictionary<string, Dictionary<string, int>>();
                _intDomains.Add(integration, intdomains);
            }
            XmlNodeList list = node.SelectNodes("Domain");
            foreach (XmlNode domainXml in list)
            {
                string name = domainXml.Attributes["name"].Value;
                string datatype = "string";
                if (domainXml.Attributes["datatype"] != null)
                {
                    datatype =  domainXml.Attributes["datatype"].Value;
                }
                switch (datatype)
                {
                    case "string":
                        if (domains.ContainsKey(name))
                        {
                            throw new InvalidConfigurationException("Domain names can only appear once per integration(" + integration + "): " + name);
                        }
                        Dictionary<string, string> mapper = new Dictionary<string, string>();
                        domains.Add(name, mapper);
                        XmlNodeList values = domainXml.SelectNodes("add");
                        foreach (XmlNode value in values)
                        {
                            string fromValue = value.Attributes["fromValue"].Value;
                            string tovalue = value.Attributes["toValue"].Value;
                            if (mapper.ContainsKey(fromValue))
                            {
                                throw new InvalidConfigurationException("Domain values can only appear once per domain(" + name + "): " + fromValue);
                            }
                            mapper.Add(fromValue, tovalue);
                        }
                        break;
                    case "stringint":
                        if (intdomains.ContainsKey(name))
                        {
                            throw new InvalidConfigurationException("Domain names can only appear once per integration(" + integration + "): " + name);
                        }
                        Dictionary<string, int> intmapper = new Dictionary<string, int>();
                        intdomains.Add(name, intmapper);
                        XmlNodeList intvalues = domainXml.SelectNodes("add");
                        foreach (XmlNode value in intvalues)
                        {
                            string fromValue = value.Attributes["fromValue"].Value;
                            string tovalue = value.Attributes["toValue"].Value;
                            if (intmapper.ContainsKey(fromValue))
                            {
                                throw new InvalidConfigurationException("Domain values can only appear once per domain(" + name + "): " + fromValue);
                            }
                            intmapper.Add(fromValue, Convert.ToInt32(tovalue));
                        }
                        break;
                }
            }
        }

    }
}
