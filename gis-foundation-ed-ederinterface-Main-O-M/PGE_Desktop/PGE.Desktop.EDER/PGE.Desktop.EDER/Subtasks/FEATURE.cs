using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace PGE.Desktop.EDER.Subtasks
{
    public class FEATURE
    {
        [XmlAttribute("GLOBALID")]
        public string featureGlobalid { get; set; }

        [XmlElement("EDITTYPE")]
        public string featureEditType { get; set; }

        [XmlElement("FEATURECLASSNAME")]
        public string featureClsNm { get; set; }

        [XmlElement("OBJECTID")]
        public string featureOID { get; set; }

        [XmlElement("SUBTYPE")]
        public string featureSubType { get; set; }

        [XmlElement("ATTRIBUTE")]
        public List<ATTRIBUTE> fieldValue { get; set; }
    }
}
