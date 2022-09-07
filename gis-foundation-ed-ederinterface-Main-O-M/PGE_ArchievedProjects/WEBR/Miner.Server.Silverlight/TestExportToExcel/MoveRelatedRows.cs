using System.Linq;
using System.Xml.Linq;
using System.IO;
using System.ComponentModel.Composition;
using System.Collections;
using System.Collections.Generic;

namespace Miner.Server.Client
{
    // Need to export the ICustomizeExportToExcel interface for Managed Extensibility Framework (MEF)
    [Export(typeof(ICustomizeExportToExcel))]
    public class MoveRelatedRows : ICustomizeExportToExcel
    {
        Stream ICustomizeExportToExcel.Convert(Stream inStream)
        {
            XDocument xdoc = XDocument.Load(inStream);
            try
            {
                string ns = @"urn:schemas-microsoft-com:office:spreadsheet";
                const string relToFlag = "Related to ";

                XName qualifiedName = XName.Get("Worksheet", ns);
                foreach (XElement node in xdoc.Descendants(qualifiedName))
                {
                    // get name for this worksheet
                    XAttribute att = (from a in node.Attributes()
                                      where a.Name == XName.Get("Name", ns)
                                      select a).First<XAttribute>();
                    string name = att.Value;

                    // now find the section of other worksheets with related data
                    IEnumerable<XElement> relatedHeaders = (from rel in xdoc.Descendants(XName.Get("Data", ns))
                                                            where rel.Value.Contains(relToFlag + name)
                                                            select rel);

                    if (relatedHeaders.Count<XElement>() == 0) continue;

                    // Grab the parent row for the related header
                    XElement relatedHeaderRow = relatedHeaders.First().Parent.Parent;
                        
                    IList<XElement> rowsToMove = new List<XElement>();
                    rowsToMove.Add(relatedHeaderRow);

                    int headerCount = 0;
                    foreach (XElement row in relatedHeaderRow.ElementsAfterSelf())
                    {
                        // looking for the next header row in this table
                        XAttribute height = row.Attribute(XName.Get("Height", ns));
                        bool isHeader = (height != null) && (height.Value == "22");
                        if (isHeader) headerCount++;
                        if (headerCount >= 2) break;

                        rowsToMove.Add(row);
                    }

                    // get the table of the current worksheet
                    XElement table = node.Descendants(XName.Get("Table", ns)).First<XElement>();
                    foreach (XElement row in rowsToMove)
                    {
                        row.Remove();
                        table.Add(new XElement(row));
                    }
                    
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

        public string Name
        {
            get { return "Move Related Rows"; }
        }
    }
}
