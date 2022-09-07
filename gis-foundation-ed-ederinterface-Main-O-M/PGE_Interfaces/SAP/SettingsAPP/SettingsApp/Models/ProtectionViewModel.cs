using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
//ENOS2EDGIS
namespace SettingsApp.Models
{
    [Serializable]
    public class ProtectionViewModel 
    {
        private int _activeProtectionIndex = -1;
        private int _activeGeneratorIndex = -1;
        private int _activeEquipmentIndex = -1;

        [HiddenInput]
        public ProtectionModel ActiveProtection { get; set; }
        [HiddenInput]
        public RelayModel ActivePrimaryPhaseRelay { get; set; }
        [HiddenInput]
        public RelayModel ActivePrimaryGroundRelay { get; set; }
        [HiddenInput]
        public RelayModel ActiveBackupPhaseRelay { get; set; }
        [HiddenInput]
        public RelayModel ActiveBackupGroundRelay { get; set; }
        [HiddenInput]
        public GeneratorModel ActiveGenerator { get; set; }
        [HiddenInput]
        public GenEquipmentModel ActiveEquipment { get; set; }

        [HiddenInput]
        public int ActiveProtectionIndex 
        {
            get { return _activeProtectionIndex; }
            set { _activeProtectionIndex = value; }
        }
        [HiddenInput]
        public int ActiveGeneratorIndex 
        {
            get { return _activeGeneratorIndex; }
            set { _activeGeneratorIndex = value; }
        }
        [HiddenInput]
        public int ActiveEquipmentIndex 
        {
            get { return _activeEquipmentIndex; }
            set { _activeEquipmentIndex = value; }
        }

        [HiddenInput]
        public List<ProtectionModel> Protections { get; set; }
        [HiddenInput]
        public List<RelayModel> Relays { get; set; }
        [HiddenInput]
        public List<GeneratorModel> Generators { get; set; }
        [HiddenInput]
        public List<GenEquipmentModel> Equipments { get; set; }


        //public SelectList ProtectionTypeList { get; set; }

        //public SelectList RelayCodeList { get; set; }
        //public SelectList RelayTypeList { get; set; }
        //public SelectList CurveTypeList { get; set; }
        //public SelectList RestraintList { get; set; }

        //public SelectList ConnectionTypeList { get; set; }
        //public SelectList StatusList { get; set; }
        //public SelectList GenTechCodeList { get; set; }
        //public SelectList ModeOfInverterList { get; set; }
        //public SelectList ControlList { get; set; }
        //public SelectList ProgramTypeList { get; set; }
        //public SelectList TechnologyTypeList { get; set; }

        //public SelectList GenTechCodeList { get; set; }
    }
}