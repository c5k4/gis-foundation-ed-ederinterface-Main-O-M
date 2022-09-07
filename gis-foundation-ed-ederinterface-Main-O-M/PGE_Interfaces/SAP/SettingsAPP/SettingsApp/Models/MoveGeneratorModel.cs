using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SettingsApp.Common;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
//ENOS2EDGIS
namespace SettingsApp.Models
{
    public class MoveGeneratorModel
    {

        public long GeneratorID { get; set; }
        public List<ProtectionModel> Protections { get; set; }
        public IEnumerable<SelectListItem> ProtectionNamesList { get; set; }
        public int CurrentProtectionIndex { get; set; }
        public string CurrentProtectionId { get; set; }

        public string Dropdown1SelectedProtection { get; set; }
        public string Dropdown2SelectedProtection { get; set; }

        public List<SelectListItem> GeneratorListBoxItems1 { get; set; }
        public string[] SelectedValuesInGeneratorLb1 { get; set; }

        public List<SelectListItem> GeneratorListBoxItems2 { get; set; }
        public string[] SelectedValuesInGeneratorLb2 { get; set; }
    }
}