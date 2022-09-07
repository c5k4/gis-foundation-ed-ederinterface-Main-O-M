using System.Linq;
using System.Xml.Linq;
using System.IO;
using System.ComponentModel.Composition;

namespace Miner.Server.Client
{
    // Need to export the ICustomizeExportToExcel interface for Managed Extensibility Framework (MEF)
    [Export(typeof(ICustomizeExportToExcel))]
    public class Test : ICustomizeExportToExcel
    {
        Stream ICustomizeExportToExcel.Convert(Stream inStream)
        {
            XDocument xdoc = XDocument.Load(inStream);
            try
            {
                string ns = @"urn:schemas-microsoft-com:office:spreadsheet";
                XName qualifiedName = XName.Get("Interior", ns);
                foreach (XElement node in xdoc.Descendants(qualifiedName))
                {
                    XAttribute att = (from a in node.Attributes()
                                      where a.Name == XName.Get("Color", ns)
                                      select a).First<XAttribute>();
                    att.Value = "Yellow";
                }
            }
            finally
            {
                // reset stream to beginning, then save modified xml doc
                inStream.SetLength(0);
                xdoc.Save(inStream);
            }

            // Return the modified stream back to the caller
            return inStream;
        }

        private string _name = "Change Header Color";
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

    }
}
