
using System.Xml.Linq;
using System.Collections.Generic;

namespace ArcFM.Silverlight.PGE.CustomTools
{
    public static class TraceProtectiveDevice
    {
        private static Dictionary<string, string> _traceVsProDevice = new Dictionary<string, string>();

        public static void ReadProtectiveDeviceSettings(XElement traceElement)
        {
            
            foreach (XElement proDeviceSettings in traceElement.Elements())
            {
                if (proDeviceSettings.Name == "ProtectiveDeviceSettings")
                {
                    _traceVsProDevice = GetTraceTypeInfo(proDeviceSettings);
                }
            }
        }

        public static List<int> GetProtectiveDeviceList(string traceTypeName)
        {
            string protectiveDeviceCSV = string.Empty;
            List<int> protectiveDeviceList = new List<int>();

            if (_traceVsProDevice.ContainsKey(traceTypeName))
            {
                protectiveDeviceCSV = _traceVsProDevice[traceTypeName];
                foreach (string proDeviceLayerIDStr in protectiveDeviceCSV.Split(','))
                {
                    int proDeviceLayerID;
                    if (int.TryParse(proDeviceLayerIDStr, out proDeviceLayerID))
                    {
                        protectiveDeviceList.Add(proDeviceLayerID);
                    }
                }
            }
            return protectiveDeviceList;
           
        }

        private static Dictionary<string, string> GetTraceTypeInfo(XElement element)
        {
            if (element == null) return null;
            string traceName = string.Empty;
            string proDeviceCSV = string.Empty;
            Dictionary<string, string> traceVsProDevice = new Dictionary<string, string>();

            foreach (XElement traceType in element.Elements())
            {
                XAttribute attribute = traceType.Attribute("Name");
                if (attribute != null)
                {
                    traceName = attribute.Value;
                }

                attribute = traceType.Attribute("ProtectiveDevices");
                if (attribute != null)
                {
                    proDeviceCSV = attribute.Value;
                }

                traceVsProDevice.Add(traceName, proDeviceCSV);
            }

            return traceVsProDevice;
        }
    }
}
