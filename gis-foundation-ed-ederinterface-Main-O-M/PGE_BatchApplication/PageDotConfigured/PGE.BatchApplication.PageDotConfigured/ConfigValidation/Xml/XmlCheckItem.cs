using System;

namespace PGE.BatchApplication.PageDotConfigured.ConfigValidation.Xml
{
    public class XmlCheckItem : IComparable<XmlCheckItem>
    {
        public int XmlLayerKey { get; private set; }
        public string XmlLayerVal { get; private set; }
        public string Url { get; private set; }
        public Boolean PingOnly { get; private set; }
        public ErrorCodes Error { get; private set; }
        public string ReferenceLine { get; set; }
        public bool isCsv { get; private set; }
        public int LineNumber { get; set; }
        public int OriginatingTableId { get; set; }

        private string ErroneousJsonLayerName { get; set; }

        public XmlCheckItem(string val, int key, string url, string refLine, bool pingOnly = false)
        {
            XmlLayerVal = val;
            XmlLayerKey = key;
            Url = url;
            Error = ErrorCodes.NoError;
            ReferenceLine = refLine;
            ErroneousJsonLayerName = null;
            PingOnly = pingOnly;
            OriginatingTableId = -1;
        }

        public int CompareTo(XmlCheckItem other)
        {
            return XmlLayerKey - other.XmlLayerKey;
        }

        public void SetError(ErrorCodes error, string jsonLayerName)
        {
            Error = error;
            ErroneousJsonLayerName = jsonLayerName;
        }

        public override string ToString()
        {
            return Error + ", Line:" + LineNumber +" XML ID: \"" + XmlLayerKey + "\"\n\"" + XmlLayerVal + "\" (XML), \"" +
                   ErroneousJsonLayerName + "\" (JSON)\nURL: " + Url +
                   "\n" + ReferenceLine;
        }
    }
}