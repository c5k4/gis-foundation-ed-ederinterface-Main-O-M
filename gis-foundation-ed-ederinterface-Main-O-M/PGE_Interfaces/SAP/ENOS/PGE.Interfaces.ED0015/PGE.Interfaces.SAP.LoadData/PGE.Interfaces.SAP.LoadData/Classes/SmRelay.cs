using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace PGE.Interfaces.SAP.LoadData.Classes
{
    public class SmRelay
    {
        public SmRelay(Dictionary<string, object> rowToDict)
        {
            ID = rowToDict["ID"];
            PROTECTION_ID = rowToDict["PROTECTION_ID"];
            DATECREATED = rowToDict["DATECREATED"];
            CREATEDBY = ReadConfigurations.Settings_User;//rowToDict["CREATEDBY"];
            DATE_MODIFIED = rowToDict["DATE_MODIFIED"];
            MODIFIEDBY = string.IsNullOrEmpty(Convert.ToString(rowToDict["MODIFIEDBY"])) ? null : ReadConfigurations.Settings_User;
            RELAY_CD = rowToDict["RELAY_CD"];
            RELAY_TYPE = rowToDict["RELAY_TYPE"];
            CURVE_TYPE = rowToDict["CURVE_TYPE"];
            MIN_AMPS_TRIP = rowToDict["MIN_AMPS_TRIP"];
            LEVER_SETTING = rowToDict["LEVER_SETTING"];
            INST_TRIP = rowToDict["INST_TRIP"];
            RESTRAINT_TYPE = rowToDict["RESTRAINT_TYPE"];
            RESTRAINT_PICKUP = rowToDict["RESTRAINT_PICKUP"];
            MIN_VOLT_PICKUP = rowToDict["MIN_VOLT_PICKUP"];
            VOLTAGE_SETPOINT = rowToDict["VOLTAGE_SETPOINT"];
            CURRENT_FUTURE = ReadConfigurations.Value_Current_Future;
            ACTION = rowToDict.ContainsKey("ACTION") ? rowToDict["ACTION"] : string.Empty;
        }
        public object ID {get;set;}
        public object PROTECTION_ID { get; set; }
        public object DATECREATED { get; set; }
        public object CREATEDBY { get; set; }
        public object DATE_MODIFIED { get; set; }
        public object MODIFIEDBY { get; set; }
        public object RELAY_CD { get; set; }
        public object RELAY_TYPE { get; set; }
        public object CURVE_TYPE { get; set; }
        public object MIN_AMPS_TRIP { get; set; }
        public object LEVER_SETTING { get; set; }
        public object INST_TRIP { get; set; }
        public object RESTRAINT_TYPE { get; set; }
        public object RESTRAINT_PICKUP { get; set; }
        public object MIN_VOLT_PICKUP { get; set; }
        public object VOLTAGE_SETPOINT { get; set; }
        public object ACTION { get; set; }
        public object CURRENT_FUTURE { get; set; }

    }
}
