using System;
using System.Collections; 
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PGE.Interface.ENOS.Batch
{
    public class ServicePoint
    {
        private string _spId;
        private Int64 _enosRefId;
        private List<GenEquipmentItem> _equipmentList;
        private string _servicePointGUID;
        private string _transformerGUID;
        private string _localOffice; 

        public ServicePoint(string spid, Int64 enosRefId)
        {
            _spId = spid;
            _enosRefId = enosRefId;             
            _equipmentList = new List<GenEquipmentItem>();
            _servicePointGUID = string.Empty;
            _transformerGUID = string.Empty;
            _localOffice = string.Empty;
        }

        public bool EquipmentItemHasInverter(int equipId)
        {
            try
            {
                Hashtable hshInverters = new Hashtable(); 
                bool hasInverter = false;
                GenEquipmentItem pTargetEquipItem = null; 

                foreach (GenEquipmentItem pEquipItem in this.EquipmentList)
                {
                    if (pEquipItem.EquipmentType == EquipmentType.equipTypeInverter)
                    {
                        if (!hshInverters.ContainsKey(pEquipItem.EquipmentId))
                            hshInverters.Add(pEquipItem.EquipmentId, 0);
                    }
                    if (pEquipItem.EquipmentId == equipId) 
                        pTargetEquipItem = pEquipItem; 

                }

                //Check for the inverter 
                if (hshInverters.ContainsKey(pTargetEquipItem.InverterId))
                    hasInverter = true;
                return hasInverter; 
            }
            catch
            {
                throw new Exception("Error determining if equipment item has an inverter");
            }
        }

        public bool HasEquipmentItem(Int64 equipId)
        {
            bool hasEquipItem = false;
            foreach (GenEquipmentItem pEquipItem in this.EquipmentList)
            {
                if (pEquipItem.EquipmentId == equipId)
                {
                    hasEquipItem = true;
                    break; 
                } 
            }
            return hasEquipItem; 
        }
        
        public string SPID
        {
            get
            {
                return _spId;
            }
            set
            {
                _spId = value;
            }
        }

        public string ServicePointGUID
        {
            get
            {
                return _servicePointGUID;
            }
            set
            {
                _servicePointGUID = value;
            }
        }

        public string TransformerGUID
        {
            get
            {
                return _transformerGUID;
            }
            set
            {
                _transformerGUID = value;
            }
        }

        public string LocalOffice
        {
            get
            {
                return _localOffice;
            }
            set
            {
                _localOffice = value;
            }
        }

        public Int64 ENOSRefId
        {
            get
            {
                return _enosRefId;
            }
            set
            {
                _enosRefId = value;
            }
        }        

        public List<GenEquipmentItem> EquipmentList
        {
            get
            {
                return _equipmentList;
            }
        }
    }

    public class GenEquipmentItem
    {
        private Int64 _equipmentId;
        private Int64 _inverterId; 
        private EquipmentType _equipmentType;
        private SourceType _sourceType;
        private string _fuelType; 
        private string _manufacturer;
        private string _model;
        private double _rating;
        private int _quantity; 

        public GenEquipmentItem( 
            Int64 equipId, Int64 inverterId, EquipmentType equipType, 
            SourceType sourceType, string manufacturer, string fuelType, 
            string model, double rating, int quantity)
        {
            _equipmentId = equipId;
            _inverterId = inverterId;
            _equipmentType = equipType;
            _sourceType = sourceType;
            _fuelType = fuelType;             
            _manufacturer = manufacturer;
            _model = model;
            _rating = rating;
            _quantity = quantity; 
        }

        public object CalculatekWOut(bool isInverter)
        {
            object kWOut = 0;
            if (!isInverter)
            {
                kWOut = (_rating * _quantity) / 1000;
            }
            else
                kWOut = DBNull.Value; 

            return kWOut; 
        }

        public object CalculateDCRating()
        {
            object dcRating = 0;
            if ((_equipmentType == Batch.EquipmentType.equipTypePV) ||
                (_equipmentType == Batch.EquipmentType.equipTypeWindTurbine) || 
                (_equipmentType == Batch.EquipmentType.equipTypePVWithInverter) || 
                (_equipmentType == Batch.EquipmentType.equipTypeWindTurbineWithInverter))
            {
                dcRating = _rating;
            }
            else
                dcRating = DBNull.Value;

            return dcRating;
        }

        public object CalculateNP_KVA(bool isInverter)
        {
            object np_kva = 0;
            if (isInverter)
            {
                np_kva = (_rating * _quantity) / 1000;
            }
            else
                np_kva = DBNull.Value; 

            return np_kva;
        }
        
        public Int64 EquipmentId
        {
            get
            {
                return _equipmentId;
            }
            set
            {
                _equipmentId = value;
            }
        }

        public Int64 InverterId
        {
            get
            {
                return _inverterId;
            }
        }

        public EquipmentType EquipmentType
        {
            get
            {
                return _equipmentType;
            }
        }

        public SourceType SourceType
        {
            get
            {
                return _sourceType;
            }
        }

        public string FuelType
        {
            get
            {
                return _fuelType;
            }
        }

        public string Manufacturer
        {
            get
            {
                return _manufacturer;
            }
        }

        public string Model
        {
            get
            {
                return _model;
            }
        }

        public double Rating
        {
            get
            {
                return _rating;
            }
        }

        public int Quantity
        {
            get
            {
                return _quantity;
            }
        }
    }
}
