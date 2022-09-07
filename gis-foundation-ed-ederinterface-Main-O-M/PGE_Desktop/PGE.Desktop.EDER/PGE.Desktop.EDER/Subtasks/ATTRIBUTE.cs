using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace PGE.Desktop.EDER.Subtasks
{
    public class ATTRIBUTE
    {
        [XmlAttribute("NAME")]
        public string FieldName { get; set; }

        [XmlAttribute("OLDVALUE")]
        public string OldValue { get; set; }

        [XmlAttribute("NEWVALUE")]
        public string NewValue { get; set; }

    }
}
