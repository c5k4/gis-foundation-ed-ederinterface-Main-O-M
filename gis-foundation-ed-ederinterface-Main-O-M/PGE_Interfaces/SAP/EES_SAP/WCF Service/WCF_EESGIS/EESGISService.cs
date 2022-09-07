using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WCF_EESGIS
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface EESGISService
    {

       

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json,
           UriTemplate = "GetEESGISData?OrderNo={OrderNumber}&OrderType={SystemType}")]
        //[WebInvoke(UriTemplate = "GetEESGISData", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        System.IO.Stream GetEESGISData(string OrderNumber, string SystemType);

        // TODO: Add your service operations here
    }


    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    [DataContract]
    public class EESSAP_PARAM
    {
        [DataMember]
        public string OrderNumber
        {
            get;
            set;
        }

        [DataMember]
        public string SystemType
        {
            get;
            set;
        }
    }

    public class EESGISDataset
    {
        public string status
        {
            get;
            set;
        }
        public string message
        {
            get;
            set;
        }
        public List<GISDatasetAttr> GISDataset
        {
            set;
            get;
        }

    }

    public class GISDatasetAttr
    {
        public string Raptor_Area
        {
            get;
            set;
        }

        public string Loading_District
        {
            get;
            set;
        }

        public string Insulation_District
        {
            get;
            set;
        }

        public double Fire_Area
        {
            get;
            set;
        }

        public string Corrosion_Dist
        {
            get;
            set;
        }


        public string LA_District
        {
            get;
            set;
        }

        public string CDF_SAR
        {
            get;
            set;
        }
        public string[] Primary_OP
        {
            get;
            set;
        }


    }
}
