using System;
using System.Collections.Generic;

namespace Telvent.PGE.ED.Desktop.ArcMapCommands.PONS
{
    public class CustomerReport
    {
        private IList<Customer> _CustomersList = new List<Customer>();
        private IList<OutageTimeDetail> _OutageDetail = new List<OutageTimeDetail>();
        private string _Description = string.Empty;
        private string _PSCID = string.Empty;
        private bool _SendmeCopy = true;
        private string _ReportID = string.Empty;
        private string _SubmitterID = string.Empty;
        private string _ContactID = string.Empty;

        public String ReportID
        {
            get
            {
                return _ReportID;
            }
            set
            {
                _ReportID = value.Trim();
            }
        }

        public String SubmitterID
        {
            get
            {
                return _SubmitterID;
            }
            set
            {
                _SubmitterID = value.Trim();
            }
        }

        public IList<Customer> AffectedCustomers
        {
            get
            {
                return _CustomersList;
            }
            set
            {
                _CustomersList = value;
            }
        }

        public IList<OutageTimeDetail> OutageDetails
        {
            get
            {
                return _OutageDetail;
            }
            set
            {
                _OutageDetail = value;
            }
        }

        public String Description
        {
            get
            {
                return _Description;
            }
            set
            {
                _Description = value.Trim();
            }
        }

        public String PSC_ID
        {
            get
            {
                return _PSCID;
            }
            set
            {
                _PSCID = value.Trim();
            }
        }

        public Boolean CC_Me_Mail
        {
            get
            {
                return _SendmeCopy;
            }
            set
            {
                _SendmeCopy = value;
            }
        }

        public String Contact_ID
        {
            get
            {
                return _ContactID;
            }
            set
            {
                _ContactID = value.Trim();
            }
        }
    }
    public class TransformerDetails
    {
        private string _CircuitId = string.Empty;
        private string _GlobalId = string.Empty;
        private Int64 oid = -1;

        public String CIRCUITID
        {
            get
            {
                return _CircuitId;
            }
            set
            {
                _CircuitId = value.Trim();
            }
        }

        public String GLOBALID
        {
            get
            {
                return _GlobalId;
            }
            set
            {
                _GlobalId = value.Trim();
            }
        }
        public Int64 OID
        {
            get
            {
                return oid;
            }
            set
            {
                oid = value;
            }
        }
    }

    public class OutageTimeDetail
    {
        private string _DateofOutage = string.Empty;
        //private string _DateofON = string.Empty;
        private string _TimeOFF = string.Empty;
        private string _TimeON = string.Empty;


        public String Outage_Date_OFF
        {
            get
            {
                return _DateofOutage;
            }
            set
            {
                _DateofOutage = value.Trim();
            }
        }
        
        public String OFF_Time
        {
            get
            {
                return _TimeOFF;
            }
            set
            {
                _TimeOFF = value.Trim();
            }
        }
        
        //public String Outage_Date_ON
        //{
        //    get
        //    {
        //        return _DateofON;
        //    }
        //    set
        //    {
        //        _DateofON = value.Trim();
        //    }
        //}

        public String ON_Time
        {
            get
            {
                return _TimeON;
            }
            set
            {
                _TimeON = value.Trim();
            }
        }
    }

    public class Customer
    {
        private int _SNO = 0;
        private string _CustomerType = string.Empty;
        private Name _CustomerName = new Name();
        private Address _CustomerAddress = new Address();
        private Address _MailAddress = new Address();
        private string _TransformerOperatingNumber = string.Empty;
        private string _CGC12 = string.Empty;
        private string _SSD = string.Empty;
        private Phone _PhoneNum = new Phone();
        private string _ServicePointID = string.Empty;
        private string _MeterNum = string.Empty;
        private string _CustAccNum = string.Empty;
        private string _Division = string.Empty;
        private bool _Excluded = false;

        public int SNO
        {
            get
            {
                return _SNO;
            }
            set
            {
                _SNO = value;
            }
        }

        public Boolean Excluded
        {
            get
            {
                return _Excluded;
            }
            set
            {
                _Excluded = value;
            }
        }

        public Name CustomerName
        {
            get
            {
                return _CustomerName;
            }
            set
            {
                _CustomerName = value;
            }
        }

        public String CustomerType
        {
            get
            {
                return _CustomerType;
            }
            set
            {
                _CustomerType = value.Trim();
            }
        }

        private String TransformerOperatingNumber
        {
            get
            {
                return _TransformerOperatingNumber;
            }
            set
            {
                _TransformerOperatingNumber = value.Trim();
            }
        }

        public String CGC12
        {
            get
            {
                return _CGC12;
            }
            set
            {
                _CGC12 = value.Trim();
            }
        }

        public String SSD
        {
            get
            {
                return _SSD;
            }
            set
            {
                _SSD = value.Trim();
            }
        }

        public Address CustomerAddress
        {
            get
            {
                return _CustomerAddress;
            }
            set
            {
                _CustomerAddress = value;
            }
        }

        public Address MailAddress
        {
            get
            {
                return _MailAddress;
            }
            set
            {
                _MailAddress = value;
            }
        }

        public Phone PhoneNumber
        {
            get
            {
                return _PhoneNum;
            }
            set
            {
                _PhoneNum = value;
            }
        }

        public String ServicePointID
        {
            get
            {
                return _ServicePointID;
            }
            set
            {
                _ServicePointID = value.Trim();
            }
        }

        public String MeterNumber
        {
            get
            {
                return _MeterNum;
            }
            set
            {
                _MeterNum = value.Trim();
            }
        }

        public String CustomerAccountNumber
        {
            get
            {
                return _CustAccNum;
            }
            set
            {
                _CustAccNum = value.Trim();
            }
        }

        public String Division
        {
            get
            {
                return _Division;
            }
            set
            {
                _Division = value.Trim();
            }
        }
    }

    public class Name
    {
        private string _MailName1 = string.Empty;
        private string _MailName2 = string.Empty;

        public String MailName1
        {
            get
            {
                return _MailName1;
            }
            set
            {
                _MailName1 = string.IsNullOrEmpty(value.Trim()) ? "PG&E Customer at" : value.Trim();
            }
        }

        public String MailName2
        {
            get
            {
                return _MailName2;
            }
            set
            {
                _MailName2 = value.Trim();
            }
        }

    }

    public class Address
    {
        private string _StreetNum = string.Empty;
        private string _StreetName1 = string.Empty;
        private string _StreetName2 = string.Empty;
        private string _Zip = string.Empty;
        private string _City = string.Empty;
        private string _State = string.Empty;

        public String StreetNumber
        {
            get
            {
                return _StreetNum;
            }
            set
            {
                _StreetNum = value.Trim();
            }
        }

        public String StreetName1
        {
            get
            {
                return _StreetName1;
            }
            set
            {
                _StreetName1 = value.Trim();
            }
        }

        public String StreetName2
        {
            get
            {
                return _StreetName2;
            }
            set
            {
                _StreetName2 = value.Trim();
            }
        }

        public String State
        {
            get
            {
                return _State;
            }
            set
            {
                _State = value.Trim();
            }
        }

        public String ZIPCode
        {
            get
            {
                return _Zip;
            }
            set
            {
                _Zip = (value.Length > 5) ? value.Insert(5, "-").Trim() : value.Trim();
            }
        }

        public String City
        {
            get
            {
                return _City;
            }
            set
            {
                _City = value.Trim();
            }
        }

    }

    public class Phone
    {
        private string _AreaCode = string.Empty;
        private string _PhoneNum = string.Empty;

        public String AreaCode
        {
            get
            {
                return _AreaCode;
            }
            set
            {
                _AreaCode = value.Trim();
            }
        }

        public String PhoneNumber
        {
            get
            {
                return _PhoneNum;
            }
            set
            {
                _PhoneNum = value.Trim();
            }
        }
    }

    public struct Equipment
    {
        public string ObjectDisplayName;
        public string ObjectClassDisplayName;
        public string CircuitID;
        public string ObjectClassName;
        public Int64 ObjectID;        
    }
}
