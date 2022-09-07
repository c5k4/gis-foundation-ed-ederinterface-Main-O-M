using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SettingsApp;
namespace SettingsApp.Common
{
    public class LoadInfo
    {
        private void GetTransformerBank(string GlobalId)
        {
            int Id = 1;
            using(SettingsEntities db = new SettingsEntities())
            {
                var TrfBankLoad = db.SM_SUBTRF_BANK_LOAD.SingleOrDefault(t => t.REF_DEVICE_ID == Id);
                if (TrfBankLoad == null)
                    throw new Exception("No load information is available in the system!");


            }
        }
    }
}
