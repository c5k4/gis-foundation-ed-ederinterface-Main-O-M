using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Json;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Xml.Linq;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Symbols;
using ESRI.ArcGIS.Client.Tasks;
using Miner.Server.Client.Events;
using Miner.Server.Client.Toolkit;
using Miner.Server.Client.Toolkit.Events;
using System.Xml;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Json;
using System.Collections;

namespace ArcFMSilverlight
{
    public class Ponsservicelayercall
    {

        private string _GetSendDataToPSLurl;
        private string _GetAveryDataURL;
        private string _GetSendemailURL;
        private string _GetExportURL;
        private string _GetgeneratePSCDataUrl;
        private string _GetDevicedataCustomerSearchUrl;
        private string _GetDevicestartcusdataURL;
        private string _GetDevicecusdataURL;
        private string _GetgenerateCustomerUrl;
        private string _GetSequenceURL;
        private string _GetCustomerServiceURL;
        private string _GetretClassInstanceURL;
        private string _GetsubstationBankCircuitURL;
        private string _GetbankURL;
        private string _GetCircuitURL;
        private string _GetStandardPrintSendemailURL;
        /// <summary>
        /// Method is for getting the prefix URL of Web Service.
        /// </summary>
        /// <returns></returns>
        public string GetPrefixUrl()
        {
            string prefixUrl;
            var pageUri = "";
            if (System.Windows.Browser.HtmlPage.Document.DocumentUri.Query != null && System.Windows.Browser.HtmlPage.Document.DocumentUri.Query != "")
                pageUri = System.Windows.Browser.HtmlPage.Document.DocumentUri.ToString().Replace(System.Windows.Browser.HtmlPage.Document.DocumentUri.Query.ToString(), "");
            else
                pageUri = System.Windows.Browser.HtmlPage.Document.DocumentUri.ToString();
            if (pageUri.Contains("Default.aspx"))
            {
                prefixUrl = pageUri.Substring(0, pageUri.IndexOf("Default.aspx"));
            }
            else
            {
                prefixUrl = pageUri;
            }
            return prefixUrl;
        }
        //Read data from Page.config
        /// <summary>
        /// 
        /// </summary>
        public void LoadLayerConfiguration()
        {
            try
            {
                if (Application.Current.Host.InitParams.ContainsKey("Config"))
                {
                    string config = Application.Current.Host.InitParams["Config"];
                    var attribute = "";
                    config = System.Windows.Browser.HttpUtility.HtmlDecode(config).Replace("***", ",");
                    XElement elements = XElement.Parse(config);
                    foreach (XElement element in elements.Elements())
                    {
                        if (element.Name.LocalName == "PONSInformation")
                        {
                            if (element.HasElements)
                            {
                                foreach (XElement childelement in element.Elements())
                                {

                                    if (childelement.Name.LocalName == "PONSService")
                                    {
                                        attribute = childelement.Attribute("generateCustomerUrl").Value;
                                        if (attribute != null)
                                        {
                                            _GetCustomerServiceURL = attribute;
                                        }
                                        attribute = childelement.Attribute("getSendtoPSLURL").Value;
                                        if (attribute != null)
                                        {
                                            _GetSendDataToPSLurl = attribute;
                                        }
                                        attribute = childelement.Attribute("getAveryDataURL").Value;
                                        if (attribute != null)
                                        {
                                            _GetAveryDataURL = attribute;
                                        }
                                        attribute = childelement.Attribute("getSendemailURL").Value;
                                        if (attribute != null)
                                        {
                                            _GetSendemailURL = attribute;
                                        }
                                        attribute = childelement.Attribute("getStandardPrintSendemailURL").Value;
                                        if (attribute != null)
                                        {
                                            _GetStandardPrintSendemailURL = attribute;
                                        }

                                        attribute = childelement.Attribute("getExportURL").Value;
                                        if (attribute != null)
                                        {
                                            _GetExportURL = attribute;
                                        }
                                        attribute = childelement.Attribute("generatePSCDataUrl").Value;//
                                        if (attribute != null)
                                        {
                                            _GetgeneratePSCDataUrl = attribute;
                                        }
                                        attribute = childelement.Attribute("getDevicedataCustomerSearchUrl").Value;
                                        if (attribute != null)
                                        {
                                            _GetDevicedataCustomerSearchUrl = attribute;
                                        }
                                        attribute = childelement.Attribute("getDevicestartcusdataUrl").Value;
                                        if (attribute != null)
                                        {
                                            _GetDevicestartcusdataURL = attribute;
                                        }
                                        attribute = childelement.Attribute("getDevicecusdataUrl").Value;
                                        if (attribute != null)
                                        {
                                            _GetDevicecusdataURL = attribute;
                                        }
                                        attribute = childelement.Attribute("generateCustomerUrl").Value;
                                        if (attribute != null)
                                        {
                                            _GetgenerateCustomerUrl = attribute;
                                        }
                                        attribute = childelement.Attribute("getSequenceURL").Value;
                                        if (attribute != null)
                                        {
                                            _GetSequenceURL = attribute;
                                        }
                                        attribute = childelement.Attribute("retClassInstanceURL").Value;
                                        if (attribute != null)
                                        {
                                            _GetretClassInstanceURL = attribute;
                                        }
                                        attribute = childelement.Attribute("getSubstationBankCiercuitID").Value;
                                        if (attribute != null)
                                        {
                                            _GetsubstationBankCircuitURL = attribute;
                                        }
                                        attribute = childelement.Attribute("getCircuitURL").Value;
                                        if (attribute != null)
                                        {
                                            _GetCircuitURL = attribute;
                                        }
                                        attribute = childelement.Attribute("getBankURL").Value;
                                        if (attribute != null)
                                        {
                                            _GetbankURL = attribute;
                                        }

                                    }

                                }

                            }

                        }
                    }
                }

            }
            catch
            {
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strDevice1"></param>
        /// <param name="strDevice2"></param>
        /// <param name="divisioncode"></param>
        /// <returns></returns>
        public string GetDevicedataCustomerSearchURL(string strDevice1, string strDevice2, int divisioncode)
        {
            string strURLSDevice = "";
            LoadLayerConfiguration();
            string prefixUrl = "";
            string queryStringSuffix = "?strDevice1=" + strDevice1 + "&strDevice2=" + strDevice2 +
                                       "&divisioncode=" + divisioncode + "";
            prefixUrl = GetPrefixUrl();
            strURLSDevice = prefixUrl + _GetDevicedataCustomerSearchUrl + queryStringSuffix;
            return strURLSDevice;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="devisionnumber"></param>
        /// <returns></returns>
        public string GetDevicestartcusdataURL(int devisionnumber)
        {
            string strURLSDevice = "";
            LoadLayerConfiguration();
            string prefixUrl = "";
            string queryStringSuffix = "?devisionnumber=" + devisionnumber + "";
            prefixUrl = GetPrefixUrl();
            strURLSDevice = prefixUrl + _GetDevicestartcusdataURL + queryStringSuffix;
            return strURLSDevice;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="operationnumber"></param>
        /// <param name="devisionnumber"></param>
        /// <returns></returns>
        public string GetDevicecusdata(string operationnumber, int devisionnumber)
        {
            string strURLSDevice = "";
            LoadLayerConfiguration();
            string prefixUrl = "";
            string queryStringSuffix = "?operationnumber=" + operationnumber + "&devisionnumber=" + devisionnumber + "";
            prefixUrl = GetPrefixUrl();
            strURLSDevice = prefixUrl + _GetDevicecusdataURL + queryStringSuffix;
            return strURLSDevice;
        }

        /// <summary>
        /// To call the method for calling Customer Information
        /// </summary>
        /// <param name="strCGSValue"></param>
        /// <param name="strCircuitId"></param>
        /// <param name="strServicePointID"></param>
        /// <param name="strDivisionIn"></param>
        /// <param name="strFlagIn"></param>
        /// <returns></returns>
        public string GetTransformercusdataCombAddress(string strCGSValue, string strCircuitId, string strServicePointID, string strDivisionIn, string strFlagIn)
        {
            string strResult = "";
            try
            {
                WebClient client = new WebClient();
                LoadLayerConfiguration();
                string prefixUrl = "";
                string queryStringSuffix = "?strCGSValue=" + strCGSValue + "&strCircuitId=" + strCircuitId +
                                           "&strServicePointID=" + strServicePointID + "&strDivisionIn=" + strDivisionIn + "&strFlagIn=" + strFlagIn + "";
                prefixUrl = GetPrefixUrl();
                strResult = prefixUrl + _GetCustomerServiceURL + queryStringSuffix;
                return strResult;
            }
            catch
            {
                return strResult;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetPSLDataAddress()
        {
            string strResult = "";
            try
            {
                LoadLayerConfiguration();
                string prefixUrl = "";
                prefixUrl = GetPrefixUrl();
                strResult = prefixUrl + _GetgeneratePSCDataUrl;
                return strResult;
            }
            catch
            {
                return strResult;
            }
        }
        public string GetSequenceAddress()
        {
            string strResult = "";
            try
            {
                LoadLayerConfiguration();
                string prefixUrl = "";
                prefixUrl = GetPrefixUrl();
                strResult = prefixUrl + _GetSequenceURL;
                return strResult;
            }
            catch
            {
                return strResult;
            }
        }
        public string GetSendEmailAddress(string include, string emailTo, string strShutdownID, string strUserName, string strMsgBody, string strDateTime, string strDescription)
        {
            string strResult = "";
            try
            {
                WebClient client = new WebClient();
                LoadLayerConfiguration();
                string prefixUrl = "";
                string queryString = "";
                queryString = "?include=" + include + "&emailTo=" + emailTo + "&strShutdownID=" + strShutdownID + "&strUserName=" + strUserName + "&strMsgBody=" + strMsgBody + "&strDateTime=" + strDateTime + "&strDescription=" + strDescription + "";
                prefixUrl = GetPrefixUrl();
                strResult = prefixUrl + _GetSendemailURL + queryString;
                return strResult;
            }
            catch
            {
                return strResult;
            }
        }
        public string GetStandardPrintSendEmailAddress(string include, string emailTo, string strUserName, string strMsgBody, string strDateTime, string strDescription)
        {
            string strResult = "";
            try
            {
                WebClient client = new WebClient();
                LoadLayerConfiguration();
                string prefixUrl = "";
                string queryString = "";
                queryString = "?include=" + include + "&emailTo=" + emailTo + "&strUserName=" + strUserName + "&strMsgBody=" + strMsgBody + "&strDateTime=" + strDateTime + "&strDescription=" + strDescription + "";
                prefixUrl = GetPrefixUrl();
                strResult = prefixUrl + _GetStandardPrintSendemailURL + queryString;
                return strResult;
            }
            catch
            {
                return strResult;
            }
        }
        public string GetCircuitIDBankSubstation(string strDivisionCode)
        {
            string strResult = "";
            try
            {
                WebClient client = new WebClient();
                LoadLayerConfiguration();
                string prefixUrl = "";
                string queryString = "";
                queryString = "?strdivCode=" + strDivisionCode;
                prefixUrl = GetPrefixUrl();
                strResult = prefixUrl + _GetsubstationBankCircuitURL + queryString;
                return strResult;
            }
            catch
            {
                return strResult;
            }
        }

        public string GetBank(string strSubName)
        {
            string strResult = "";
            try
            {
                WebClient client = new WebClient();
                LoadLayerConfiguration();
                string prefixUrl = "";
                string queryString = "";
                queryString = "?strSubstationName=" + strSubName;
                prefixUrl = GetPrefixUrl();
                strResult = prefixUrl + _GetbankURL + queryString;
                return strResult;
            }
            catch
            {
                return strResult;
            }
        }

        public string GetCircuit(string strbank)
        {
            string strResult = "";
            try
            {
                WebClient client = new WebClient();
                LoadLayerConfiguration();
                string prefixUrl = "";
                string queryString = "";
                queryString = "?strBankName=" + strbank;
                prefixUrl = GetPrefixUrl();
                strResult = prefixUrl + _GetCircuitURL + queryString;
                return strResult;
            }
            catch
            {
                return strResult;
            }
        }


    }


    public class JsonHelper
    {
        public static T Deserialize<T>(string json)
        {
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));

                return (T)serializer.ReadObject(ms);
            }
        }
    }
    public class JsonHelperSerialiser
    {

        public static string JsonSerialize(object obj)
        {
            using (MemoryStream mstream = new MemoryStream())
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
                serializer.WriteObject(mstream, obj); mstream.Position = 0;
                using (StreamReader reader = new StreamReader(mstream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}



