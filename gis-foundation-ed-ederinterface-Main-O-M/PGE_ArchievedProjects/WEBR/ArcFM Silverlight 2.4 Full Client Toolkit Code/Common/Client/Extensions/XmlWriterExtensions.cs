using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

using ESRI.ArcGIS.Client;

#if SILVERLIGHT
using Miner.Server.Client.Export;
using Miner.Server.Client.Tasks;
#elif WPF
using Miner.Mobile.Client.Export;
using Miner.Mobile.Client.Tasks;
#endif

#if SILVERLIGHT
namespace Miner.Server.Client
#elif WPF
namespace Miner.Mobile.Client
#endif
{
    static class XmlWriterExtensions
    {
        const string StylesheetNamespace = "ss";

        public static void WriteDataCell(this XmlWriter source, string text, string styleID = "StringLiteral", string type = "String", string columnSpan = null)
        {
            if (source == null) return;

            source.WriteStartElement("Cell");
            source.WriteAttributeString(StylesheetNamespace, "StyleID", null, styleID);
            if (!string.IsNullOrEmpty(columnSpan))
            {
                source.WriteAttributeString(StylesheetNamespace, "MergeAcross", null, columnSpan);
            }
            source.WriteStartElement("Data");
            source.WriteAttributeString(StylesheetNamespace, "Type", null, type);
            source.WriteString(text);
            source.WriteEndElement();
            source.WriteEndElement();
        }

        public static void WriteDefaultStyles(this XmlWriter source)
        {
            source.WriteStartElement("Styles");

            source.WriteStartElement("Style");
            source.WriteAttributeString(StylesheetNamespace, "ID", null, "Default");
            source.WriteAttributeString(StylesheetNamespace, "Name", null, "Normal");

            source.WriteStartElement("Alignment");
            source.WriteAttributeString(StylesheetNamespace, "Vertical", null, "Bottom");
            source.WriteAttributeString(StylesheetNamespace, "Horizontal", null, "Center");
            source.WriteEndElement();   //Alignment

            source.WriteStartElement("Borders");

            source.WriteStartElement("Border");
            source.WriteAttributeString(StylesheetNamespace, "Position", null, "Bottom");
            source.WriteAttributeString(StylesheetNamespace, "LineStyle", null, "Continuous");
            source.WriteAttributeString(StylesheetNamespace, "Weight", null, "0.5");
            source.WriteEndElement();   //Border

            source.WriteStartElement("Border");
            source.WriteAttributeString(StylesheetNamespace, "Position", null, "Left");
            source.WriteAttributeString(StylesheetNamespace, "LineStyle", null, "Continuous");
            source.WriteAttributeString(StylesheetNamespace, "Weight", null, "0.5");
            source.WriteEndElement();   //Border

            source.WriteStartElement("Border");
            source.WriteAttributeString(StylesheetNamespace, "Position", null, "Right");
            source.WriteAttributeString(StylesheetNamespace, "LineStyle", null, "Continuous");
            source.WriteAttributeString(StylesheetNamespace, "Weight", null, "0.5");
            source.WriteEndElement();   //Border

            source.WriteStartElement("Border");
            source.WriteAttributeString(StylesheetNamespace, "Position", null, "Top");
            source.WriteAttributeString(StylesheetNamespace, "LineStyle", null, "Continuous");
            source.WriteAttributeString(StylesheetNamespace, "Weight", null, "0.5");
            source.WriteEndElement();   //Border

            source.WriteEndElement();   //Borders
            source.WriteEndElement();   //Style

            source.WriteStartElement("Style");
            source.WriteAttributeString(StylesheetNamespace, "ID", null, "BoldColumn");

            source.WriteStartElement("Alignment");
            source.WriteAttributeString(StylesheetNamespace, "Vertical", null, "Top");
            source.WriteAttributeString(StylesheetNamespace, "Horizontal", null, "Center");
            source.WriteEndElement();   //Alignment

            source.WriteStartElement("Font");
            source.WriteAttributeString("x", "Family", null, "Swiss");
            source.WriteAttributeString(StylesheetNamespace, "Bold", null, "1");
            source.WriteEndElement();   //Font

            source.WriteStartElement("Interior");
            source.WriteAttributeString(StylesheetNamespace, "Color", null, "#D3D3D3");
            source.WriteAttributeString(StylesheetNamespace, "Pattern", null, "Solid");
            source.WriteEndElement();   //Interior
            source.WriteEndElement();   //Style

            source.WriteStartElement("Style");
            source.WriteAttributeString(StylesheetNamespace, "ID", null, "RelatedRowHeader");

            source.WriteStartElement("Alignment");
            source.WriteAttributeString(StylesheetNamespace, "Vertical", null, "Top");
            source.WriteAttributeString(StylesheetNamespace, "Horizontal", null, "Left");
            source.WriteEndElement();   //Alignment

            source.WriteStartElement("Font");
            source.WriteAttributeString("x", "Family", null, "Swiss");
            source.WriteEndElement();   //Font

            source.WriteStartElement("Interior");
            source.WriteAttributeString(StylesheetNamespace, "Color", null, "#F5EEAB");
            source.WriteAttributeString(StylesheetNamespace, "Pattern", null, "Solid");
            source.WriteEndElement();   //Interior
            source.WriteEndElement();   //Style

            source.WriteStartElement("Style");
            source.WriteAttributeString(StylesheetNamespace, "ID", null, "StringLiteral");
            source.WriteStartElement("NumberFormat");
            source.WriteAttributeString(StylesheetNamespace, "Format", null, "@");
            source.WriteEndElement();   //NumberFormat
            source.WriteEndElement();   //Style

            source.WriteEndElement();   //Styles
        }

        public static void WriteWorksheet(this XmlWriter writer, Worksheet worksheet)
        {
            if (worksheet == null) return;

            writer.WriteWorksheetHeader(worksheet.Name); //Creates a worksheet element
            writer.WriteStartElement("Table");

            writer.WriteResultSet(worksheet.ResultSet);
            writer.WriteRelationships(worksheet.Relationships, worksheet.ResultSet == null);

            writer.WriteEndElement();   //Table
            writer.WriteEndElement();   //Worksheet
        }

        public static void WriteResultSet(this XmlWriter writer, IResultSet resultSet, bool writeColumnDefinition = true)
        {
            if (resultSet == null) return;
            if (resultSet.Features == null) return;
            if (resultSet.Features.Count == 0) return;

            List<KeyValuePair<string,string>> fieldAliases = (from field in resultSet.FieldAliases
                                                       where Utility.FieldIsHidden(field.Key) == false
                                                       select field).ToList();

            // Write the OBJECTID field regardless of visibility value                     
            bool hasOID = fieldAliases.Count(f => (String.Compare(f.Key,"OBJECTID",StringComparison.InvariantCultureIgnoreCase) == 0 ) ||
                                                  (String.Compare(f.Key, "OID", StringComparison.InvariantCultureIgnoreCase) == 0))
                                                   > 0;
            if (!hasOID)
            {
                string oidAlias = Utility.GetObjectIDFieldName(resultSet.Features[0].Attributes);
                KeyValuePair<string, string> kvp = new KeyValuePair<string, string>("OBJECTID", oidAlias);
                fieldAliases.Insert(0, kvp);
            }

            // Add one for the "Related To" column
            if (writeColumnDefinition == true)
            {
                writer.WriteColumnDefinitions(fieldAliases.Count() + 1);
            }
            IEnumerable<string> values = fieldAliases.Select(f => f.Value);
            writer.WriteColumns(values);

            foreach (Graphic graphic in resultSet.Features)
            {
                writer.WriteStartElement("Row");

                foreach (KeyValuePair<string, string> field in fieldAliases)
                {
                    string fieldName = Utility.FieldToBind(field.Key, field.Value, graphic.Attributes);
                    string fieldValue = string.Empty;
                    if (graphic.Attributes.ContainsKey(fieldName))
                    {
                        object value = graphic.Attributes[fieldName];
                        if (value != null)
                        {
                            fieldValue = value.ToString();
                        }
                    }

                    writer.WriteDataCell(fieldValue);
                }

                writer.WriteEndElement();
            }
        }

        public static void WriteRelationships(this XmlWriter writer, IList<RelationshipResults> relationships, bool writeColumnDefinitions = false)
        {
            if (relationships == null) return;
            if (relationships.Count == 0) return;

            foreach (RelationshipResults results in relationships)
            {
                if (results.Results == null) continue;
                if (results.Results.Count == 0) continue;

                List<string> fields = (from field in results.Results.First().ResultSet.FieldAliases
                                       where Utility.FieldIsHidden(field.Key) == false
                                       select field.Value).ToList();

                fields.Insert(0,"Related To");

                if (writeColumnDefinitions)
                {
                    writer.WriteColumnDefinitions(fields.Count);
                    writeColumnDefinitions = false;
                }
                else
                {
                    // Add some padding
                    writer.WriteRaw("<Row/>");
                    writer.WriteRaw("<Row/>");
                    writer.WriteRaw("<Row/>");
                }

                writer.WriteStartElement("Row");
                writer.WriteDataCell(results.Name, "RelatedRowHeader", columnSpan: fields.Count.ToString());
                writer.WriteEndElement();

                writer.WriteColumns(fields);
                foreach (RelationshipResult relationship in results.Results)
                {
                    foreach (Graphic graphic in relationship.ResultSet.Features)
                    {
                        writer.WriteStartElement("Row");

                        // Write the "Related To" value at the beginning
                        writer.WriteDataCell(relationship.ObjectID.ToString());

                        foreach (var field in relationship.ResultSet.FieldAliases)
                        {
                            if (Utility.FieldIsHidden(field.Key)) continue;

                            string fieldName = Utility.FieldToBind(field.Key, field.Value, graphic.Attributes);
                            string fieldValue = string.Empty;
                            if (graphic.Attributes.ContainsKey(fieldName))
                            {
                                object value = graphic.Attributes[fieldName];
                                if (value != null)
                                {
                                    fieldValue = value.ToString();
                                }
                            }
                            writer.WriteDataCell(fieldValue);
                        }

                        writer.WriteEndElement();
                    }
                }
            }
        }

        public static void WriteWorksheetHeader(this XmlWriter source, string tabHeader)
        {
            string worksheetName = tabHeader;
            if (worksheetName.Length > 31)
            {
                worksheetName = worksheetName.Substring(0, 28);
                worksheetName = worksheetName + "...";
            }
            if (worksheetName.Contains("History"))
            {
                worksheetName = worksheetName.Replace("History", "Hist."); //DO NOT INT!
            }
            else if (worksheetName.Contains("HISTORY"))
            {
                worksheetName = worksheetName.Replace("HISTORY", "HIST."); //DO NOT INT!
            }

            source.WriteStartElement("Worksheet");
            source.WriteAttributeString(StylesheetNamespace, "Name", null, worksheetName);
        }

        public static void WriteColumnDefinitions(this XmlWriter source, int columnCount)
        {
            for (int i = 0; i < columnCount; i++)
            {
                source.WriteStartElement("Column");
                source.WriteAttributeString(StylesheetNamespace, "Width", null, "180");
                source.WriteEndElement();
            }
        }

        public static void WriteColumns(this XmlWriter source, IEnumerable<string> fieldNames)
        {
            if (fieldNames == null) return;

            source.WriteStartElement("Row");
            source.WriteAttributeString(StylesheetNamespace, "Height", null, "22");

            foreach (string fieldName in fieldNames)
            {
                source.WriteDataCell(fieldName, "BoldColumn");
            }

            source.WriteEndElement();
        }
    }
}
