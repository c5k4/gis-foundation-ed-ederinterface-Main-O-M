using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;

namespace PGE.BatchApplication.CWOSL.Classes
{
    public class ServicePoint
    {
        public int ServicePointOID { get; set; }
        public string Address { get; set; }
        //public string AddressWithoutZipCode { get; set; }
        public string Status { get; set; }
        public string ServiceLocationGUIDValue { get; set; }
        public string SecondaryLoadPointGUIDValue { get; set; }
        public string CGC12 { get; set; }
        public Int32 TransformerOID { get; set; }
        public string TransformerType { get; set; }
        public bool AddressMissing { get; set; }
        public bool IsSecondaryLoadPoint { get; set; }
        public string ServicePointID { get; set; }
        public string CircuitID { get; set; }
        public string PhaseDesignation { get; set; }
        public bool DelayProcessing { get; set; } //Due to high number of Sec-UG, delay processing and log to CWOSL with D status.

        public string StreetNumber { get; set; }
        public string StreetName1 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }

        public ServicePoint(IRow servicePoint)
        {
            //ServicePointRow = servicePoint;
            ServicePointOID = servicePoint.OID;
            IsSecondaryLoadPoint = false;
            DelayProcessing = false;
            DetermineAddress(servicePoint);
        }
        public void SetUpObjectInfo(IRow row)
        {
        }

        public void Update_CWOSL_Table(int serviceLocGUIDFldIdx, int secondaryLoadPntGUIDFldIdx)
        {
            //Update service loation guid vlaue or secondary load point value to relate Service Point to it's location.
        }

        private void DetermineAddress(IRow servicePoint)
        {

            AddressMissing = false;
            Status = null;
            //Determine the address for the current service point.
            //STREETNUMBER, STREETNAME1, CITY, STATE, ZIP
            StreetNumber = HelperCls.GetValue(servicePoint, Initialize.StreetNumberFldIdx);
            StreetName1 = HelperCls.GetValue(servicePoint, Initialize.StreetName1FldIdx);
            City = HelperCls.GetValue(servicePoint, Initialize.CityFldIdx);
            State = HelperCls.GetValue(servicePoint, Initialize.StateFldIdx);
            Zip = HelperCls.GetValue(servicePoint, Initialize.ZipFldIdx);
            CGC12 = HelperCls.GetValue(servicePoint, Initialize.CGC12FldIdx);
            ServicePointID = HelperCls.GetValue(servicePoint, Initialize.ServicePointIDFldIdx);

            if (string.IsNullOrEmpty(StreetNumber))
                Status = "Street Number is missing ";
            else
                StreetNumber = StreetNumber.Replace("'", "''");

            if (string.IsNullOrEmpty(StreetName1))
                Status = Status + "Street Name is missing ";
            else
                StreetName1 = StreetName1.Replace("'", "''");

            if (string.IsNullOrEmpty(City))
                Status = Status + "City Name is missing ";
            else
                City = City.Replace("'", "''");

            if (string.IsNullOrEmpty(State))
                Status = Status + "State Name is missing ";

            //if (string.IsNullOrEmpty(Zip))
            //    Status = Status + "Zip code is missing";

            if (!string.IsNullOrEmpty(Status))
                AddressMissing = true;

            //complete address
            Address = StreetNumber + " " +
                      StreetName1 + ", " +
                      City + ", " +
                      State + " " +
                      Zip;
        }
    }
}
