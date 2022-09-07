// ========================================================================
// Company: PGE USA Inc., PGE Utilities Group
// Client : PG&E
// $Revision: 1 $
// ========================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using PGE.Interfaces.Integration.Framework.Exceptions;

namespace PGE.Interfaces.Integration.Framework.Utilities
{
    /// <summary>
    /// Looks up field mappings from GIS to other systems.
    /// </summary>
    public sealed class DomainManager
    {
        static readonly DomainManager _instance = new DomainManager();
        private Dictionary<string,Dictionary<string, Dictionary<string, string>>> _allDomains;
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
        /// Gets the mapped value
        /// </summary>
        /// <param name="domain">The name of the domain mapping</param>
        /// <param name="value">The input value</param>
        /// <param name="integration">The name of the integration to use</param>
        /// <returns>The mapped value</returns>
        public string GetValue(string domain, string value, string integration)
        {
            string lookup = value;
            string toValue = string.Empty;

            if (string.IsNullOrEmpty(lookup) == false)
            {
                if (_allDomains.ContainsKey(integration))
                {
                    if (_allDomains[integration].ContainsKey(domain))
                    {
                        if (_allDomains[integration][domain].ContainsKey(lookup))
                        {
                            toValue = _allDomains[integration][domain][lookup];
                        }
                        else//check if there is a default value
                        {
                            if (_allDomains[integration][domain].ContainsKey("{default}"))
                            {
                                toValue = _allDomains[integration][domain]["{default}"];
                            }
                            else
                            {
                                Exception ex = new Exception(string.Format("Domain {0} doesn't have value {1}.", domain, lookup));
                                throw ex;
                            }
                        }
                    }
                }
            }

            return toValue;
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
            XmlNodeList list = node.SelectNodes("Domain");
            foreach (XmlNode domainXml in list)
            {
                string name = domainXml.Attributes["name"].Value;
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
            }
        }

    }
}
