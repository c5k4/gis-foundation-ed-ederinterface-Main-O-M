using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PGE.Interfaces.ED11_12
{
    [Serializable]
    public class PTTTransaction
    {
        public PTTTransaction()
        {

        }

        private Guid TransactionGuid = Guid.NewGuid();

        public int OrderNumber { get; set; }
        public TransactionType Type { get; set; }
        public Pole TransactionPole { get; set; }

        public override string ToString()
        {
            return Type.ToString() + " Transaction";
        }

        public override bool Equals(object obj)
        {
            if (obj is PTTTransaction && ((PTTTransaction)obj).TransactionGuid == TransactionGuid)
            {
                return true;
            }
            return false;
        }
    }

    [Serializable]
    public abstract class Pole
    {
        public TransactionStatus Status { get; set; }
        public TransactionErrorType ErrorType { get; set; }
        public string MessageDetail { get; set; }
        public string DeletedGhostPole { get; set; }

        public bool MissingAttributesEncountered
        {
            get
            {
                if (string.IsNullOrEmpty(_missingAttributes)) { return false; }
                else { return true; }
            }
        }

        private string _missingAttributes = "";
        public string MissingAttributes
        {
            get
            {
                return _missingAttributes;
            }
            set
            {
                if (string.IsNullOrEmpty(_missingAttributes)) { _missingAttributes = value; }
                else { _missingAttributes += "," + value; }
            }
        }

        private bool _poleMalformedXml = false;
        public bool PoleMalformedXml
        {
            get
            {
                return _poleMalformedXml;
            }
            set
            {
                _poleMalformedXml = value;
            }
        }

        private string _poleMalformedMessage = "";
        public string PoleMalformedMessage
        {
            get { return _poleMalformedMessage; }
            set
            {
                if (string.IsNullOrEmpty(_poleMalformedMessage))
                {
                    _poleMalformedMessage = value;
                }
                else
                {
                    _poleMalformedMessage += " --- " + value;
                }
            }
        }

        private string _SAPEquipID = "";
        public string SAPEquipID
        {
            get
            {
                if (this is PoleReplace)
                {
                    return ((PoleReplace)this).PoleToInsert.SAPEquipID;
                }
                else
                {
                    return _SAPEquipID;
                }
            }
            set
            {
                _SAPEquipID = value;
            }
        }

        private Guid _guid = new Guid("00000000000000000000000000000000");
        public Guid Guid
        {
            get
            {
                if (this is PoleReplace)
                {
                    return ((PoleReplace)this).PoleToInsert.Guid;
                }
                else
                {
                    return _guid;
                }
            }
            private set
            {
                _guid = value;
            }
        }

        public void SetGuid(string guidValue)
        {
            //Verify that this is a valid GUID. If not, throw an error
            try
            {
                Guid guid = new System.Guid(guidValue);
                Guid = guid;
            }
            catch (Exception ex)
            {
                PoleMalformedXml = true;
                PoleMalformedMessage = "Malformed Error: Specified Global ID is invalid (" + guidValue + ")";
            }
        }

        /// <summary>
        /// m4jf edgisrearch 415 
        ///  set RecordId of each transaction
        /// </summary>
        
        private string RecordIDValue = default;
        public string RecordID
        {
            get
            {
                return RecordIDValue;
            }
            set
            {
                RecordIDValue = value;
            }  
                       
        }
        private string PoleClassValue = default;

        public string PoleClass
        {
            get
            {
                return PoleClassValue;
            }
            set
            {
                PoleClassValue = value;
            }

        }
        private string HeightValue = default;
        public string Height
        {
            get
            {
                return HeightValue;
            }
            set
            {
                HeightValue = value;
            }

        }

        private string MaterialValue = default;
        public string Material
        {
            get
            {
                return MaterialValue;
            }
            set
            {
                MaterialValue = value;
            }

        }
        private string PlantSectionValue = default;
        public string PlantSection
        {
            get
            {
                return PlantSectionValue;
            }
            set
            {
                PlantSectionValue = value;
            }

        }
        private string StartupDateValue =default;
             public string StartupDate
        {
            get
            {
                return StartupDateValue;
            }
            set
            {
                StartupDateValue = value;
            }

        }

        private string _projectID = "";
        public string ProjectID
        {
            get
            {
                if (this is PoleReplace)
                {
                    return ((PoleReplace)this).PoleToInsert.ProjectID;
                }
                else
                {
                    return _projectID;
                }
            }
            set
            {
                _projectID = value;
            }
        }

        // m4jf edgisrearch 415

        private string _InstallationDate = "";

        public string InstallationDate
        {
            get
            {
                if (this is PoleReplace)
                {
                    return ((PoleReplace)this).PoleToInsert.InstallationDate;
                }
                else
                {
                    return _InstallationDate;
                }
            }
            set
            {
                _InstallationDate = value;
            }
        }
        private string _originalTransaction = "";
        public string OriginalTransaction
        {
            get { return _originalTransaction; }
            set { _originalTransaction = value; }
        }

        public OriginalTransaction _orginalTransction { get; set; }
        public override string ToString()
        {
            if (!(Guid == Guid.Empty)) { return Guid.ToString("B").ToUpper(); }
            else { return SAPEquipID; }
        }
    }


    [Serializable]
    public class OriginalTransaction
    {
        public string attributename;
        public string AttributeName
        {
            get { return attributename; }
            set { attributename = value; }

        }
    }

        [Serializable]
        public class PoleInsert : Pole
        {
            private double _latitude = 0.0;
            public double Latitude
            {
                get
                {
                    return _latitude;
                }
            }

            private double _longitude = 0.0;
            public double Longitude
            {
                get
                {
                    return _longitude;
                }
            }

            public void SetLongitude(string longitude)
            {
                double longitudeValue = 0.0;

                if (double.TryParse(longitude, out longitudeValue))
                {
                    //Check if number has 6 decimal place values
                    int indexOfDecimal = longitude.IndexOf(".");
                    int decimalPlaces = longitude.Length - indexOfDecimal - 1;

#if DEBUG
                    decimalPlaces += 1;
#endif
                    if (indexOfDecimal < 0 || decimalPlaces < 6)
                    {
                        //Malformed Xml
                        PoleMalformedXml = true;
                        PoleMalformedMessage = "Malformed Error: Longitude value must have 6 decimal places (" + longitude + ")";
                    }
                }
                else
                {
                    //Malformed Xml
                    PoleMalformedXml = true;
                    PoleMalformedMessage = "Malformed Error: Longitude value is invalid (" + longitude + ")";
                }

                _longitude = longitudeValue;
            }

            public void SetLatitude(string latitude)
            {
                double latitudeValue = 0.0;

                if (double.TryParse(latitude, out latitudeValue))
                {
                    //Check if number has 6 decimal place values
                    int indexOfDecimal = latitude.IndexOf(".");
                    int decimalPlaces = latitude.Length - indexOfDecimal - 1;

#if DEBUG
                    decimalPlaces += 1;
#endif

                    if (indexOfDecimal < 0 || decimalPlaces < 6)
                    {
                        //Malformed Xml
                        PoleMalformedXml = true;
                        PoleMalformedMessage = "Malformed Error: Latitude value must have 6 decimal places (" + latitude + ")";
                    }
                }
                else
                {
                    //Malformed Xml
                    PoleMalformedXml = true;
                    PoleMalformedMessage = "Malformed Error: Latitude value is invalid (" + latitude + ")";
                }

                _latitude = latitudeValue;
            }

            public Dictionary<string, string> FieldValues { get; set; }
        }

        [Serializable]
        public class PoleDelete : Pole
        {
            //Nothing necessary for a pole delete except what is in base class
        }

        [Serializable]
        public class PoleReplace : Pole
        {
            public PoleDelete PoleToDelete { get; set; }
            public PoleInsert PoleToInsert { get; set; }


        }


        [Serializable]
        public class PoleUpdate : Pole
        {
            public string FieldName { get; set; }
            public string OldFieldValue { get; set; }
            public string NewFieldValue { get; set; }
        }


        [Serializable]
        public enum TransactionType
        {
            Insert,
            Update,
            Delete,
            Replace
        }

        /// <summary>
        /// Error types for PTT transaction failures
        /// </summary>
        [Serializable]
        public enum TransactionStatus
        {
            Success,
            Failure,
            InProgress
        }

        /// <summary>
        /// Error types for PTT transaction failures
        /// </summary>
        [Serializable]
        public enum TransactionErrorType
        {
            Success = 0,
            DomainValueMismatch = 1,
            MalformedXml = 2,
            MissingMandatoryFieldValues = 3,
            DuplicateInsertFeature = 4,
            UpdateMissingFeature = 5,
            DeleteMissingFeature = 6,
            DuplicateReplace = 7,
            UpdateConflict = 8,
            DeletedAlternativeGhostPole = 9,
            InProgress = 10,
            UnknownError = 11
        }


   
}
