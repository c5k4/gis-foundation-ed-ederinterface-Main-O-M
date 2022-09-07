using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using SettingsApp.Common;

namespace SettingsApp.Models
{
    public class SpecialLoadModel
    {
        public List<GISAttributes> GISAttributes { get; set; }
        
        [Display(Name = "Transformer CGC# - ")]
        public string CGCId { get; set; }
        public bool IsAdmin { get; set; }
        public string CurrentUserName { get; set; }
        [SettingsValidatorAttribute("SPECIAL_LOAD", "LOAD_DESCRIPTION")]
        [Display(Name = "Description :")]
        public string Description { get; set; }
        [SettingsValidatorAttribute("SPECIAL_LOAD", "S_KW")]
        [Display(Name = "Summer KW :")]
        public int? SummerKW { get; set; }
        [SettingsValidatorAttribute("SPECIAL_LOAD", "S_KVAR")]
        [Display(Name = "Summer KVAR :")]
        public int? SummerKVAR { get; set; }
        [SettingsValidatorAttribute("SPECIAL_LOAD", "W_KW")]
        [Display(Name = "Winter KW :")]
        public int? WinterKW { get; set; }
        [SettingsValidatorAttribute("SPECIAL_LOAD", "W_KVAR")]
        [Display(Name = "Winter KVAR :")]
        public int? WinterKVAR { get; set; }
        [SettingsValidatorAttribute("SPECIAL_LOAD", "CREATE_DTM")]
        [Display(Name = "Created :")]
        public DateTime? CreatedDate { get; set; }
        [SettingsValidatorAttribute("SPECIAL_LOAD", "UPDATE_DTM")]
        [Display(Name = "Last Modified :")]
        public DateTime? ModifiedDate { get; set; }
        [Display(Name = "Created By :")]
        public string CreatedByUser { get; set; }
        [Display(Name = "Last Modified By :")]
        public string ModifiedByUser { get; set; }
        public void PopulateEntityFromModel(string GlobalId,string DeviceType)
        {
            if(this.IsAdmin)
            {
                using (SettingsEntities db = new SettingsEntities())
                {
                    var SpecialLoadQuery = db.SM_SPECIAL_LOAD.Where(l => l.REF_GLOBAL_ID.Trim().ToUpper() == GlobalId.Trim().ToUpper() && l.DEVICE_TYPE.ToUpper() == DeviceType.ToUpper());
                    if (SpecialLoadQuery.Any())
                    {
                        foreach (var LoadItem in SpecialLoadQuery)
                        {
                            PopulateEntityFromModel(LoadItem, GlobalId, DeviceType,false);
                        }
                    }
                    else
                    {
                        var LoadItem = new SM_SPECIAL_LOAD();
                        //var MaxLoadId = db.SM_SPECIAL_LOAD.Select(t => t.ID).Max();
                        //LoadItem.ID = MaxLoadId + 1;
                        PopulateEntityFromModel(LoadItem, GlobalId, DeviceType,true);
                        db.SM_SPECIAL_LOAD.AddObject(LoadItem);
                    }
                    db.SaveChanges();
                }
            }
       }

        private void PopulateEntityFromModel(SM_SPECIAL_LOAD LoadItem,string GlobalId,string DeviceType,bool IsNewItem)
        {
            LoadItem.REF_GLOBAL_ID = GlobalId.ToUpper();
            LoadItem.DEVICE_TYPE = DeviceType.ToUpper();
            LoadItem.LOAD_DESCRIPTION = this.Description;
            LoadItem.S_KVAR = this.SummerKVAR;
            LoadItem.S_KW = this.SummerKW;
            LoadItem.W_KVAR = this.WinterKVAR;
            LoadItem.W_KW = this.WinterKW;
            if (IsNewItem)
            {
               LoadItem.CREATE_USERID = SettingsApp.Common.Security.CurrentUser;
            }
            LoadItem.UPDATE_USERID = SettingsApp.Common.Security.CurrentUser;
        }

        public void PopulateModelFromEntity(string GlobalId,string DeviceType)
        {
            var IsAdmin = false;
            if (SettingsApp.Common.Security.IsInAdminGroup)
                IsAdmin = true;

             if (!string.IsNullOrEmpty(GlobalId) && !string.IsNullOrEmpty(DeviceType))
            {
                using (SettingsEntities db = new SettingsEntities())
                {
                    var SpecialLoadQuery = db.SM_SPECIAL_LOAD.Where(l => l.REF_GLOBAL_ID.Trim() == GlobalId.Trim().ToUpper() && l.DEVICE_TYPE.ToUpper()==DeviceType.ToUpper());
                    if (SpecialLoadQuery.Any())
                    {
                        foreach (var LoadItem in SpecialLoadQuery)
                        {
                            this.Description = LoadItem.LOAD_DESCRIPTION;
                            this.SummerKVAR = LoadItem.S_KVAR;
                            this.SummerKW = LoadItem.S_KW;
                            this.WinterKVAR = LoadItem.W_KVAR;
                            this.WinterKW = LoadItem.W_KW;
                            this.CreatedDate = LoadItem.CREATE_DTM;
                            this.ModifiedDate = LoadItem.UPDATE_DTM;
                            this.CreatedByUser = LoadItem.CREATE_USERID;
                            this.ModifiedByUser = LoadItem.UPDATE_USERID;
                        }
                    }
                }
            }
            this.IsAdmin = IsAdmin;
            
        }
    }
}