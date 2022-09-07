using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Objects;
using System.Reflection;
using System.Linq.Expressions;
using System.Data.Objects.DataClasses;

namespace SettingsApp.Common
{
    public class SiteCache
    {
        # region GIS Methods

        public static Tuple<int, string> GetLayerID(string layerName)
        {
            string key = "GISLayers";
            if (HttpContext.Current.Application[key] == null)
                HttpContext.Current.Application[key] = new GISService().GetLayers();

            Dictionary<string, Tuple<int, string>> layers = HttpContext.Current.Application[key] as Dictionary<string, Tuple<int, string>>;
            return layers[layerName.ToUpper()];
        }

        public static List<GISAttributes> GetLayerAttributes(int layerID, string serviceURL)
        {
            string key = string.Concat("GISLayer_", layerID, "_", serviceURL);
            List<GISAttributes> fields;
            Dictionary<int, List<GISAttributes>> subTypes;
            if (HttpContext.Current.Application[key] == null || HttpContext.Current.Application[string.Concat(key, "_SubTypes")] == null)
            {
                new GISService().GetPropertiesOrder(layerID, out fields, out subTypes, serviceURL);
                HttpContext.Current.Application[key] = fields;
                HttpContext.Current.Application[string.Concat(key, "_SubTypes")] = subTypes;
            }
            return HttpContext.Current.Application[key] as List<GISAttributes>;
        }

        public static List<GISAttributes> GetLayerSubTypes(int layerID, int subTypeID, string serviceURL)
        {
            string key = string.Concat("GISLayer_", layerID, "_", serviceURL, "_SubTypes");
            if (HttpContext.Current.Application[key] == null)
                GetLayerAttributes(layerID, serviceURL);

            Dictionary<int, List<GISAttributes>> subTypes = HttpContext.Current.Application[key] as Dictionary<int, List<GISAttributes>>;
            return subTypes[subTypeID];
        }

        public static string GetObjectIdFromGIS(string layerName, string globalID, string KEY)
        {
            var key = string.Concat(globalID, "-objectid");
            var ObjectId = string.Empty;
            if (HttpContext.Current.Application[key] == null)
            {
                var attributeValues = getGISAttributes(layerName, globalID);
                ObjectId = getValueFromAttributes(attributeValues, KEY);
                HttpContext.Current.Application[key] = ObjectId;
            }
            ObjectId = HttpContext.Current.Application[key] as string;
            return ObjectId;
        }
        private static Dictionary<string, string> getGISAttributes(string layerName, string globalID)
        {
            Tuple<int, string> layer = SiteCache.GetLayerID(layerName);
            Dictionary<string, string> attributeValues = new Dictionary<string, string>();
            attributeValues = (new GISService()).GetProperties(globalID, layer.Item1, layer.Item2);
            return attributeValues;
        }
        public static string getValueFromAttributes(Dictionary<string, string> attributeValues, string KEY)
        {
            var ObjectId = string.Empty;
            if (attributeValues.ContainsKey(KEY))
            {
                if (attributeValues[KEY] != null) ObjectId = Convert.ToString(attributeValues[KEY]);
            }

            return ObjectId;
        }
        # endregion

        # region Site Lookups

        private static Dictionary<string, string> yesNo;

        public static Dictionary<string, string> GetYesNoLookUp
        {
            get
            {
                if (yesNo == null)
                {
                    yesNo = new Dictionary<string, string>();
                    yesNo.Add("", "");
                    yesNo.Add("Y", "Yes");
                    yesNo.Add("N", "No");
                }
                return yesNo;
            }
        }
        public static Dictionary<string, string> GetIntegerLookUp(int firstNum, int lastNum, int increment)
        {
            Dictionary<string, string> output = new Dictionary<string, string>();
            output.Add("", "");
            int num = firstNum;
            while (num <= lastNum)
            {
                output.Add(num.ToString(), num.ToString());
                num = num + increment;
            };
            return output;
        }
        public static Dictionary<string, string> GetLookUpValues(string deviceName, string fieldName, string controlType)
        {
            string key = string.Concat("LookUp_", deviceName).ToUpper();
            string fieldKey = string.Concat(fieldName, "_", controlType).ToUpper();
            if (HttpContext.Current.Application[key] == null)
                HttpContext.Current.Application[key] = getLookupValues(deviceName);

            Dictionary<string, Dictionary<string, string>> lookUp = HttpContext.Current.Application[key] as Dictionary<string, Dictionary<string, string>>;
            if (lookUp.ContainsKey(fieldKey))
                return lookUp[fieldKey];
            else
                return new Dictionary<string, string>();
        }

        public static Dictionary<string, string> GetLookUpValues(string deviceName, string fieldName)
        {
            return GetLookUpValues(deviceName, fieldName, "All");
        }

        private static Dictionary<string, Dictionary<string, string>> getLookupValues(string deviceName)
        {
            SettingsEntities context = new SettingsEntities();
            Dictionary<string, Dictionary<string, string>> lookupValues = new Dictionary<string, Dictionary<string, string>>();

            var lookups = context.SM_TABLE_LOOKUP.Where(l => l.DEVICE_NAME.Equals(deviceName, StringComparison.CurrentCultureIgnoreCase)).OrderBy(l => l.SORT_NUM);
            string key = string.Empty;

            foreach (SM_TABLE_LOOKUP l in lookups)
            {
                key = string.Concat(l.FIELD_NAME.Trim(), "_", l.CONTROL_TYPE.Trim()).ToUpper();
                if (lookupValues.ContainsKey(key))
                {
                    lookupValues[key].Add((l.CODE == null ? "" : l.CODE.Trim()), (l.DESCRIPTION == null ? "" : l.DESCRIPTION.Trim()));
                }
                else
                {
                    lookupValues.Add(key, new Dictionary<string, string>());
                    lookupValues[key].Add((l.CODE == null ? "" : l.CODE.Trim()), (l.DESCRIPTION == null ? "" : l.DESCRIPTION.Trim()));
                }
            }
            return lookupValues;
        }

        #endregion

        # region Validation Methods

        public static HashSet<string> GetRequiredFields(string deviceName)
        {
            string key = string.Concat("RequiredFields").ToUpper();
            if (HttpContext.Current.Application[key] == null)
                HttpContext.Current.Application[key] = getRequiredFields();

            Dictionary<string, HashSet<string>> required = HttpContext.Current.Application[key] as Dictionary<string, HashSet<string>>;
            key = string.Concat(deviceName).ToUpper();
            if (!required.ContainsKey(key))
                return new HashSet<string>();
            return required[key];
        }

        public static HashSet<string> GetRequiredFields(string deviceName, string controlType)
        {
            string key = string.Concat("RequiredFields").ToUpper();
            if (HttpContext.Current.Application[key] == null)
                HttpContext.Current.Application[key] = getRequiredFields();

            Dictionary<string, HashSet<string>> required = HttpContext.Current.Application[key] as Dictionary<string, HashSet<string>>;
            key = string.Concat(deviceName.Trim(), "_", controlType.Trim()).ToUpper();
            if (required.ContainsKey(key))
                return required[key];
            else
                return new HashSet<string>();
        }

        public static HashSet<string> GetRequiredFields(string deviceName, string switchType, string ats)
        {
            string key = string.Concat("RequiredFields").ToUpper();
            if (HttpContext.Current.Application[key] == null)
                HttpContext.Current.Application[key] = getRequiredFields();

            Dictionary<string, HashSet<string>> required = HttpContext.Current.Application[key] as Dictionary<string, HashSet<string>>;
            key = string.Concat(deviceName.Trim(), "_", switchType.Trim(), "_", ats.Trim()).ToUpper();
            if (required.ContainsKey(key))
                return required[key];
            else
                return new HashSet<string>();
        }

        private static Dictionary<string, HashSet<string>> getRequiredFields()
        {
            SettingsEntities context = new SettingsEntities();
            Dictionary<string, HashSet<string>> retVal = new Dictionary<string, HashSet<string>>();
            string key = string.Empty;
            foreach (SM_SWITCH_REQUIRED req in context.SM_SWITCH_REQUIRED)
            {
                key = string.Concat("SWITCH_", req.SWITCH_TYPE, "_", req.ATS).ToUpper();
                if (req.REQUIRED != null && req.REQUIRED.Equals("Y", StringComparison.CurrentCultureIgnoreCase))
                    addRequiredFields(key, req.FIELD_NAME, retVal);
            }

            key = string.Empty;
            foreach (SM_SECTIONALIZER_REQUIRED req in context.SM_SECTIONALIZER_REQUIRED)
            {
                key = string.Concat("SECTIONALIZER").ToUpper();
                if (req.REQUIRED.Equals("Y", StringComparison.CurrentCultureIgnoreCase))
                    addRequiredFields(key, req.FIELD_NAME, retVal);
            }

            key = string.Empty;
            foreach (SM_INTERRUPTER_REQUIRED req in context.SM_INTERRUPTER_REQUIRED)
            {
                key = string.Concat("INTERRUPTER").ToUpper();
                if (req.REQUIRED.Equals("Y", StringComparison.CurrentCultureIgnoreCase))
                    addRequiredFields(key, req.FIELD_NAME, retVal);
            }

            key = string.Empty;
            foreach (SM_NETWORK_PROTECTOR_REQUIRED req in context.SM_NETWORK_PROTECTOR_REQUIRED)
            {
                key = string.Concat("NETWORK_PROTECTOR").ToUpper();
                if (req.REQUIRED.Equals("Y", StringComparison.CurrentCultureIgnoreCase))
                    addRequiredFields(key, req.FIELD_NAME, retVal);
            }

            key = string.Empty;
            foreach (SM_RECLOSER_REQUIRED req in context.SM_RECLOSER_REQUIRED)
            {
                // key = string.Concat("RECLOSER_", req.CONTROL_TYPE).ToUpper();
                key = string.Concat("RECLOSER_", req.CONTROL_TYPE, "_", req.ACTIVE_PROFILE).ToUpper();   //DA# 200101 - ME Q1 2020
                if (req.REQUIRED != null && req.REQUIRED.Equals("Y", StringComparison.CurrentCultureIgnoreCase))
                    addRequiredFields(key, req.FIELD_NAME, retVal);
            }

            key = string.Empty;
            foreach (SM_REGULATOR_REQUIRED req in context.SM_REGULATOR_REQUIRED)
            {
                key = string.Concat("REGULATOR").ToUpper();
                if (req.REQUIRED.Equals("Y", StringComparison.CurrentCultureIgnoreCase))
                    addRequiredFields(key, req.FIELD_NAME, retVal);
            }

            key = string.Empty;
            foreach (SM_CIRCUIT_BREAKER_REQUIRED req in context.SM_CIRCUIT_BREAKER_REQUIRED)
            {
                key = string.Concat("CIRCUIT_BREAKER").ToUpper();
                if (req.REQUIRED.Equals("Y", StringComparison.CurrentCultureIgnoreCase))
                    addRequiredFields(key, req.FIELD_NAME, retVal);
            }

            key = string.Empty;
            foreach (SM_CAPACITOR_REQUIRED req in context.SM_CAPACITOR_REQUIRED)
            {
                key = string.Concat("CAPACITOR_", req.CONTROL_TYPE).ToUpper();
                if (req.REQUIRED != null && req.REQUIRED.Equals("Y", StringComparison.CurrentCultureIgnoreCase))
                    addRequiredFields(key, req.FIELD_NAME, retVal);
            }

            key = string.Empty;
            foreach (SM_PRIMARY_METER_REQUIRED req in context.SM_PRIMARY_METER_REQUIRED)
            {
                key = string.Concat("PRIMARY_METER").ToUpper();
                if (req.REQUIRED != null && req.REQUIRED.Equals("Y", StringComparison.CurrentCultureIgnoreCase))
                    addRequiredFields(key, req.FIELD_NAME, retVal);
            }

            key = string.Empty;
            //******************ENOS2EDGIS Start*******************
            /*foreach (SM_PRIMARY_GEN_REQUIRED req in context.SM_PRIMARY_GEN_REQUIRED)
            {
                key = string.Concat("PRIMARY_GEN").ToUpper();
                if (req.REQUIRED != null && req.REQUIRED.Equals("Y", StringComparison.CurrentCultureIgnoreCase))
                    addRequiredFields(key, req.FIELD_NAME, retVal);
            }
            key = string.Empty;
            foreach (SM_PRIMARY_GEN_DTL_REQUIRED req in context.SM_PRIMARY_GEN_DTL_REQUIRED)
            {
                key = string.Concat("PRIMARY_GEN_DTL").ToUpper();
                if (req.REQUIRED != null && req.REQUIRED.Equals("Y", StringComparison.CurrentCultureIgnoreCase))
                    addRequiredFields(key, req.FIELD_NAME, retVal);
            }
            key = string.Empty;*/
            //********************ENOS2EDGIS End*********************

            foreach (SM_RECLOSER_TS_REQUIRED req in context.SM_RECLOSER_TS_REQUIRED)
            {
                key = string.Concat("TRIPSAVER").ToUpper();
                if (req.REQUIRED != null && req.REQUIRED.Equals("Y", StringComparison.CurrentCultureIgnoreCase))
                    addRequiredFields(key, req.FIELD_NAME, retVal);
            }

            //******************ENOS2EDGIS Start*******************
            foreach (SM_GENERATION_REQUIRED req in context.SM_GENERATION_REQUIRED)
            {
                key = string.Concat("GENERATION").ToUpper();
                if (req.REQUIRED != null && req.REQUIRED.Equals("Y", StringComparison.CurrentCultureIgnoreCase))
                    addRequiredFields(key, req.FIELD_NAME, retVal);
            }
            key = string.Empty;
            foreach (SM_PROTECTION_REQUIRED req in context.SM_PROTECTION_REQUIRED)
            {
                key = string.Concat("PROTECTION").ToUpper();
                if (req.REQUIRED != null && req.REQUIRED.Equals("Y", StringComparison.CurrentCultureIgnoreCase))
                    addRequiredFields(key, req.FIELD_NAME, retVal);
            }
            key = string.Empty;
            foreach (SM_RELAY_REQUIRED req in context.SM_RELAY_REQUIRED)
            {
                key = string.Concat("RELAY").ToUpper();
                if (req.REQUIRED != null && req.REQUIRED.Equals("Y", StringComparison.CurrentCultureIgnoreCase))
                    addRequiredFields(key, req.FIELD_NAME, retVal);
            }
            key = string.Empty;
            foreach (SM_GENERATOR_REQUIRED req in context.SM_GENERATOR_REQUIRED)
            {
                key = string.Concat("GENERATOR").ToUpper();
                if (req.REQUIRED != null && req.REQUIRED.Equals("Y", StringComparison.CurrentCultureIgnoreCase))
                    addRequiredFields(key, req.FIELD_NAME, retVal);
            }
            key = string.Empty;
            foreach (SM_GEN_EQUIPMENT_REQUIRED req in context.SM_GEN_EQUIPMENT_REQUIRED)
            {
                key = string.Concat("GENEQUIPMENT").ToUpper();
                if (req.REQUIRED != null && req.REQUIRED.Equals("Y", StringComparison.CurrentCultureIgnoreCase))
                    addRequiredFields(key, req.FIELD_NAME, retVal);
            }
            //********************ENOS2EDGIS End*********************
            foreach (SM_RECLOSER_FS_REQUIRED req in context.SM_RECLOSER_FS_REQUIRED)
            {
                key = string.Concat("FUSESAVER").ToUpper();
                if (req.REQUIRED != null && req.REQUIRED.Equals("Y", StringComparison.CurrentCultureIgnoreCase))
                    addRequiredFields(key, req.FIELD_NAME, retVal);
            }

            //ME Q4 2019 Release - DA#1
            foreach (SM_SWITCH_MSO_REQUIRED req in context.SM_SWITCH_MSO_REQUIRED)
            {
                key = string.Concat("MSOSWITCH_", req.SWITCH_TYPE, "_", req.ATS).ToUpper();
                if (req.REQUIRED != null && req.REQUIRED.Equals("Y", StringComparison.CurrentCultureIgnoreCase))
                    addRequiredFields(key, req.FIELD_NAME, retVal);
            }
            return retVal;
        }

        private static void addRequiredFields(string key, string fieldName, Dictionary<string, HashSet<string>> requiredFields)
        {
            key = key.Trim();
            fieldName = fieldName.Trim().ToUpper();
            if (requiredFields.ContainsKey(key))
            {
                requiredFields[key].Add(fieldName);
            }
            else
            {
                requiredFields.Add(key, new HashSet<string>());
                requiredFields[key].Add(fieldName);
            }
        }

        public static Tuple<decimal, decimal> GetRangeFields(string deviceName, string fieldName)
        {
            string key = string.Concat("Range_", deviceName).ToUpper();
            if (HttpContext.Current.Application[key] == null)
                HttpContext.Current.Application[key] = getRangeFields(deviceName);

            Dictionary<string, Tuple<decimal, decimal>> range = HttpContext.Current.Application[key] as Dictionary<string, Tuple<decimal, decimal>>;
            if (range.ContainsKey(fieldName.ToUpper()))
                return range[fieldName.ToUpper()];
            else
                return null;
        }

        private static Dictionary<string, Tuple<decimal, decimal>> getRangeFields(string deviceName)
        {
            SettingsEntities context = new SettingsEntities();
            Dictionary<string, Tuple<decimal, decimal>> retVal = new Dictionary<string, Tuple<decimal, decimal>>();
            string key = string.Empty;

            if (deviceName == "TRIPSAVER")
                deviceName = "RECLOSER_TS";
            else if (deviceName == "FUSESAVER")
                deviceName = "RECLOSER_FS";
            else if (deviceName == "MSOSWITCH")    //ME Q4 2019 - DA #1
                deviceName = "SWITCH_MSO";
            deviceName = "SM_" + deviceName;

            foreach (SM_TABLE_RANGE_VALUE range in context.SM_TABLE_RANGE_VALUE.Where(r => r.DEVICE_NAME.ToUpper() == deviceName.ToUpper()))
            {
                key = range.FIELD_NAME.Trim().ToUpper();
                if (retVal.ContainsKey(key))
                {
                    retVal[key] = new Tuple<decimal, decimal>(range.MIN_VALUE.GetValueOrDefault(0), range.MAX_VALUE.GetValueOrDefault(0));
                }
                else
                {
                    retVal.Add(key, new Tuple<decimal, decimal>(range.MIN_VALUE.GetValueOrDefault(0), range.MAX_VALUE.GetValueOrDefault(0)));
                }
            }
            return retVal;
        }

        #endregion

        # region Display Methods

        public static HashSet<string> GetFieldsToDisplayForSwitch(string switchType, string ats)
        {
            string key = string.Concat("DisplayFields").ToUpper();
            if (HttpContext.Current.Application[key] == null)
                HttpContext.Current.Application[key] = getDisplayFields();

            Dictionary<string, HashSet<string>> required = HttpContext.Current.Application[key] as Dictionary<string, HashSet<string>>;
            key = string.Concat("SWITCH_DISPLAY_", switchType, "_", ats).ToUpper();
            if (required.ContainsKey(key))
                return required[key];
            else
                return new HashSet<string>();
        }

        //ME Q4 2019 MSO REQUIREMENT
        public static HashSet<string> GetFieldsToDisplayForMSOSwitch(string switchType, string ats)
        {
            string key = string.Concat("DisplayFields").ToUpper();
            if (HttpContext.Current.Application[key] == null)
                HttpContext.Current.Application[key] = getDisplayFields();

            Dictionary<string, HashSet<string>> required = HttpContext.Current.Application[key] as Dictionary<string, HashSet<string>>;
            key = string.Concat("SWITCH_MSO_DISPLAY_", switchType, "_", ats).ToUpper();
            if (required.ContainsKey(key))
                return required[key];
            else
                return new HashSet<string>();
        }

        public static HashSet<string> GetFieldsToDisplayForCapacitor(string controllerUnitType)
        {
            string key = string.Concat("DisplayFields").ToUpper();
            if (HttpContext.Current.Application[key] == null)
                HttpContext.Current.Application[key] = getDisplayFields();

            Dictionary<string, HashSet<string>> required = HttpContext.Current.Application[key] as Dictionary<string, HashSet<string>>;
            key = string.Concat("CAPACITOR_DISPLAY_", controllerUnitType).ToUpper();
            if (required.ContainsKey(key))
                return required[key];
            else
                return new HashSet<string>();
        }

        //DA# 200101 - ME Q1 2020
        public static HashSet<string> GetFieldsToDisplayForRecloser(string controllerUnitType, string activeProfile)
        {
            string key = string.Concat("DisplayFields").ToUpper();
            if (HttpContext.Current.Application[key] == null)
                HttpContext.Current.Application[key] = getDisplayFields();

            Dictionary<string, HashSet<string>> required = HttpContext.Current.Application[key] as Dictionary<string, HashSet<string>>;
            //key = string.Concat("RECLOSER_DISPLAY_", controllerUnitType).ToUpper();
            key = string.Concat("RECLOSER_DISPLAY_", controllerUnitType, "_", activeProfile).ToUpper();
            if (required.ContainsKey(key))
                return required[key];
            else
                return new HashSet<string>();
        }

        public static HashSet<string> GetFieldsToDisplayForFuseSaver()
        {
            string key = string.Concat("DisplayFields").ToUpper();
            if (HttpContext.Current.Application[key] == null)
                HttpContext.Current.Application[key] = getDisplayFields();

            Dictionary<string, HashSet<string>> required = HttpContext.Current.Application[key] as Dictionary<string, HashSet<string>>;
            key = "FUSESAVER_DISPLAY";
            if (required.ContainsKey(key))
                return required[key];
            else
                return new HashSet<string>();
        }

        private static Dictionary<string, HashSet<string>> getDisplayFields()
        {
            SettingsEntities context = new SettingsEntities();
            Dictionary<string, HashSet<string>> retVal = new Dictionary<string, HashSet<string>>();
            string key = string.Empty;
            foreach (SM_SWITCH_REQUIRED req in context.SM_SWITCH_REQUIRED)
            {
                key = string.Concat("SWITCH_DISPLAY_", req.SWITCH_TYPE.Trim(), "_", req.ATS.Trim()).ToUpper();
                addRequiredFields(key, req.FIELD_NAME.ToUpper().Trim(), retVal);
            }
            foreach (SM_RECLOSER_REQUIRED req in context.SM_RECLOSER_REQUIRED)
            {
                if (req.CONTROL_TYPE != null)
                {
                    //key = string.Concat("RECLOSER_DISPLAY_", req.CONTROL_TYPE.Trim()).ToUpper();
                    key = string.Concat("RECLOSER_DISPLAY_", req.CONTROL_TYPE.Trim(), "_", req.ACTIVE_PROFILE.Trim()).ToUpper();    //DA# 200101 - ME Q1 2020
                    addRequiredFields(key, req.FIELD_NAME.ToUpper().Trim(), retVal);
                }
            }
            foreach (SM_CAPACITOR_REQUIRED req in context.SM_CAPACITOR_REQUIRED)
            {
                key = string.Concat("CAPACITOR_DISPLAY_", req.CONTROL_TYPE.Trim()).ToUpper();
                addRequiredFields(key, req.FIELD_NAME.ToUpper().Trim(), retVal);
            }
            foreach (SM_RECLOSER_FS_REQUIRED req in context.SM_RECLOSER_FS_REQUIRED)
            {
                key = "FUSESAVER_DISPLAY";
                addRequiredFields(key, req.FIELD_NAME.ToUpper().Trim(), retVal);
            }

            //ME Q4 2019 Release - DA#1
            foreach (SM_SWITCH_MSO_REQUIRED req in context.SM_SWITCH_MSO_REQUIRED)
            {
                key = string.Concat("SWITCH_MSO_DISPLAY_", req.SWITCH_TYPE.Trim(), "_", req.ATS.Trim()).ToUpper();
                addRequiredFields(key, req.FIELD_NAME.ToUpper().Trim(), retVal);
            }
            return retVal;
        }
        #endregion

        public static Dictionary<string, string> LayerSubTypeTableMapping()
        {
            string key = "LayerSubTypeTableMapping";
            Dictionary<string, string> retVal = new Dictionary<string, string>();
            if (HttpContext.Current.Application[key] == null)
            {
                using (SettingsEntities db = new SettingsEntities())
                {
                    foreach (SM_FC_LAYER_MAPPING map in db.SM_FC_LAYER_MAPPING)
                    {
                        if (map.SUBTYPE != null)
                            retVal[string.Concat(map.LAYER_NAME, "_", map.SUBTYPE).ToUpper()] = map.SM_TABLE;
                    }
                    HttpContext.Current.Application[key] = retVal;
                }
            }
            retVal = HttpContext.Current.Application[key] as Dictionary<string, string>;
            return retVal;
        }

        public static Dictionary<string, string> LayerTableMapping()
        {
            string key = "LayerTableMapping";
            Dictionary<string, string> retVal = new Dictionary<string, string>();
            if (HttpContext.Current.Application[key] == null)
            {
                using (SettingsEntities db = new SettingsEntities())
                {
                    foreach (SM_FC_LAYER_MAPPING map in db.SM_FC_LAYER_MAPPING)
                    {
                        //ENOS2EDGIS, SUBTYPE == -1 condition added
                        if (map.SUBTYPE == null || map.SUBTYPE == -1)
                            retVal[map.LAYER_NAME.ToUpper()] = map.SM_TABLE;
                    }
                    HttpContext.Current.Application[key] = retVal;
                }

            }
            retVal = HttpContext.Current.Application[key] as Dictionary<string, string>;
            return retVal;
        }

        public static List<string> DistinctLayers()
        {
            string key = "DistinctLayerTableMapping";
            Dictionary<string, string> retVal = new Dictionary<string, string>();
            if (HttpContext.Current.Application[key] == null)
            {
                using (SettingsEntities db = new SettingsEntities())
                {
                    foreach (SM_FC_LAYER_MAPPING map in db.SM_FC_LAYER_MAPPING)
                    {
                        if (!retVal.ContainsKey(map.LAYER_NAME.ToUpper()))
                            retVal[map.LAYER_NAME.ToUpper()] = map.SM_TABLE;
                    }
                    HttpContext.Current.Application[key] = retVal;
                }
            }
            retVal = HttpContext.Current.Application[key] as Dictionary<string, string>;
            return retVal.Keys.ToList();
        }

        public static string OperatingNumber(string globalID, string device)
        {
            string retVal = string.Empty;
            if (device == "Capacitor")
                retVal = BaseModel<SM_CAPACITOR>.Select(s => s.GLOBAL_ID == globalID && s.CURRENT_FUTURE == "C").FirstOrDefault().OPERATING_NUM;

            else if (device == "CircuitBreaker")
                retVal = BaseModel<SM_CIRCUIT_BREAKER>.Select(s => s.GLOBAL_ID == globalID && s.CURRENT_FUTURE == "C").FirstOrDefault().OPERATING_NUM;

            else if (device == "Interrupter")
                retVal = BaseModel<SM_INTERRUPTER>.Select(s => s.GLOBAL_ID == globalID && s.CURRENT_FUTURE == "C").FirstOrDefault().OPERATING_NUM;

            else if (device == "NetworkProtector")
                retVal = BaseModel<SM_NETWORK_PROTECTOR>.Select(s => s.GLOBAL_ID == globalID && s.CURRENT_FUTURE == "C").FirstOrDefault().OPERATING_NUM;

            else if (device == "Recloser")
                retVal = BaseModel<SM_RECLOSER>.Select(s => s.GLOBAL_ID == globalID && s.CURRENT_FUTURE == "C").FirstOrDefault().OPERATING_NUM;

            else if (device == "Regulator")
                retVal = BaseModel<SM_REGULATOR>.Select(s => s.GLOBAL_ID == globalID && s.CURRENT_FUTURE == "C").FirstOrDefault().OPERATING_NUM;

            else if (device == "Sectionalizer")
                retVal = BaseModel<SM_SECTIONALIZER>.Select(s => s.GLOBAL_ID == globalID && s.CURRENT_FUTURE == "C").FirstOrDefault().OPERATING_NUM;

            else if (device == "Switch")
                retVal = BaseModel<SM_SWITCH>.Select(s => s.GLOBAL_ID == globalID && s.CURRENT_FUTURE == "C").FirstOrDefault().OPERATING_NUM;

            else if (device == "TripSaver")
                retVal = BaseModel<SM_RECLOSER_TS>.Select(s => s.GLOBAL_ID == globalID && s.CURRENT_FUTURE == "C").FirstOrDefault().OPERATING_NUM;

            else if (device == "FuseSaver")
                retVal = BaseModel<SM_RECLOSER_FS>.Select(s => s.GLOBAL_ID == globalID && s.CURRENT_FUTURE == "C").FirstOrDefault().OPERATING_NUM;

                //ME Q4 2019 Release - DA#1
            else if (device == "MSOSwitch")
                retVal = BaseModel<SM_SWITCH_MSO>.Select(s => s.GLOBAL_ID == globalID && s.CURRENT_FUTURE == "C").FirstOrDefault().OPERATING_NUM;

            return retVal;
        }
    }

    public abstract class BaseModel<TEntity> where TEntity : EntityObject
    {
        public static string EntityName = typeof(TEntity).Name;

        public static ObjectQuery<TEntity> CreateQuery()
        {
            SettingsEntities context = new SettingsEntities();
            ObjectQuery<TEntity> query = context.CreateQuery<TEntity>(EntityName);
            query.MergeOption = MergeOption.NoTracking;
            return query;
        }

        public static List<TEntity> Select(Expression<Func<TEntity, bool>> predicate)
        {
            ObjectQuery<TEntity> query = CreateQuery();
            using (ObjectContext context = query.Context)
            {
                List<TEntity> result = query.Where(predicate).ToList();
                return result;
            }
        }
    }
}
