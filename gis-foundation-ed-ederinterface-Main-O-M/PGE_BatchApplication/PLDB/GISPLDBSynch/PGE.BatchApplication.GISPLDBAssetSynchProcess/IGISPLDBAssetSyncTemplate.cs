using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Web;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Collections;


namespace PGE.BatchApplication.GISPLDBAssetSynchProcess
{
    interface IGISPLDBAssetSyncTemplate
    {
        string postDataToPLDB(Dictionary<string, string> dictSend, string ActionType);
    }
    
    public class TLDBDictTemplate 
    {
        public TLDBDictTemplate() { }

        [DataMember]
        public List<Dictionary<string, string>> ASSET = new List<Dictionary<string, string>>();
        
        [DataMember]
        public List<Dictionary<string, string>> InsertASSET = new List<Dictionary<string, string>>();

        [DataMember]
        public List<Dictionary<string, string>> UpdateASSET = new List<Dictionary<string, string>>();

        [DataMember]
        public List<Dictionary<string, string>> DeleteASSET = new List<Dictionary<string, string>>();

        
       
    }
}
