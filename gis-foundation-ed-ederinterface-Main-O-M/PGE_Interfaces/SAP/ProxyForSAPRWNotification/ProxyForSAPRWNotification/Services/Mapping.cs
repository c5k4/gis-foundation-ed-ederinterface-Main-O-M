using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace PGE.Interfaces.ProxyForSAPRWNotification.Services
{
    public class Mapping
    {
        private Dictionary<string,DomainMapping> _data;

        public Dictionary<string, DomainMapping> Data
        {
            get { return _data; }
            set { _data = value; }
        }

        public Mapping()
        {
            try
            {
                _data = new Dictionary<string, DomainMapping>();
                XmlDocument xmlDoc = new XmlDocument();
                if(File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\FLMapping.xml"))
                { xmlDoc.Load(AppDomain.CurrentDomain.BaseDirectory + "\\FLMapping.xml"); }
                Initialize(xmlDoc.FirstChild);
                
            }

            catch (Exception)
            {
                // ignored
            }
        }

        public void Initialize(XmlNode node)
        {
            _data = new Dictionary<string, DomainMapping>();

            XmlNodeList values = node.SelectNodes("add");
                
            if (values != null)
            {
                foreach (XmlNode value in values)
                {
                    DomainMapping objDomainMapping = new DomainMapping();
                    if (value.Attributes != null)
                    {
                        string gisDivision = value.Attributes["GISDivision"].Value;
                        objDomainMapping.SapDivision = value.Attributes["SAPDivision"].Value;
                        objDomainMapping.MapOffice = value.Attributes["MapOffice"].Value;
                        objDomainMapping.MainWorkCenter = value.Attributes["MainWorkCenter"].Value;
                        objDomainMapping.District = value.Attributes["District"].Value;
                        objDomainMapping.Floc = value.Attributes["FLOC"].Value;
                        objDomainMapping.GisRegion = value.Attributes["GISRegion"].Value;

                       _data.Add(gisDivision, objDomainMapping);
                    }
                }
            }
        }
    }

    public class DomainMapping
    {
        public string GisRegion {get; set;}
        public string SapDivision { get; set; }
        public string MapOffice { get; set; }
        public string MainWorkCenter { get; set; }
        public string District { get; set; }
        public string Floc { get; set; }

    }

}