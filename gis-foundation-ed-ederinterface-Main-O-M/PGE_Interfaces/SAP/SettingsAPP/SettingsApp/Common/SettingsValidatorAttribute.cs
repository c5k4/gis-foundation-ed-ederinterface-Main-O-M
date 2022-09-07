using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SettingsApp.Common
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class SettingsValidatorAttribute : ValidationAttribute
    {
        public string PropertyName { get; set; }
        public string DeviceName { get; set; }
        private int CommentsMaxLength = 1000;
        private int ByPassPlanMaxLendth = 500;
        private int FLISRCommentsMaxLength = 250;
        private int CircuitLoadSWLimitDescMaxLength = 100;

        public SettingsValidatorAttribute(string deviceName, string propertyName)
        {
            this.PropertyName = propertyName;
            this.DeviceName = deviceName.ToUpper();
        }

        public override string FormatErrorMessage(string errorMessage)
        {
            ErrorMessage = errorMessage;
            return base.FormatErrorMessage("");
        }



        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var fieldDisplayName = validationContext.DisplayName.Trim().ToLower();

            if (fieldDisplayName == "engineering comments :" || fieldDisplayName == "special conditions :")
            {
                return IsMaxSizeExceed(value, CommentsMaxLength);
            }
            else if (fieldDisplayName == "flisr comments :")
            {
                return IsMaxSizeExceed(value, FLISRCommentsMaxLength);
            }
            else if (fieldDisplayName == "bypass plan :")
            {
                return IsMaxSizeExceed(value, ByPassPlanMaxLendth);
            }


            ValidationResult retVal = ValidationResult.Success;
            HashSet<string> requiredFields;
            object controlType = null;
            var LoadDevices = new string[] { "SUBTRF_BANK_LOAD", "SUBTRF_BANK_LOAD_HIST", "CIRCUIT_LOAD", "CIRCUIT_LOAD_HIST" };
            if (LoadDevices.Contains(DeviceName))
            {
                return ValidateLoadInfoData(value);
            }

            if (DeviceName == "SWITCH")
            {
                object switchType = validationContext.ObjectType.GetProperty("SwitchType").GetValue(validationContext.ObjectInstance, null);
                object ats = validationContext.ObjectType.GetProperty("ATSCapable").GetValue(validationContext.ObjectInstance, null);
                requiredFields = SiteCache.GetRequiredFields(this.DeviceName, (switchType == null ? "" : switchType.ToString()), (ats == null ? "" : ats.ToString()));
            }
            else if (DeviceName == "RECLOSER")
            {
                controlType = validationContext.ObjectType.GetProperty("ControllerUnitType").GetValue(validationContext.ObjectInstance, null);
                requiredFields = SiteCache.GetRequiredFields(this.DeviceName, (controlType == null ? "" : controlType.ToString()));
            }
            else if (DeviceName == "CAPACITOR")
            {
                controlType = validationContext.ObjectType.GetProperty("ControlType").GetValue(validationContext.ObjectInstance, null);
                requiredFields = SiteCache.GetRequiredFields(this.DeviceName, (controlType == null ? "" : controlType.ToString()));
            }
            //**********************ENOS2EDGIS Start*************************
            else if (DeviceName == "GENERATOR" || DeviceName == "GENEQUIPMENT" || DeviceName == "GENERATION")
            {
                if (!Security.IsSuperUserActive)
                {
                    return ValidationResult.Success;
                }
                else
                {
                    requiredFields = SiteCache.GetRequiredFields(this.DeviceName);
                }
            }
            //**************************ENOS2EDGIS End*************************
            //ME Q4 2019 - DA #1
            else if (DeviceName == "MSOSWITCH")
            {
                object switchType = validationContext.ObjectType.GetProperty("SwitchType").GetValue(validationContext.ObjectInstance, null);
                object ats = validationContext.ObjectType.GetProperty("ATSCapable").GetValue(validationContext.ObjectInstance, null);
                requiredFields = SiteCache.GetRequiredFields(this.DeviceName, (switchType == null ? "" : switchType.ToString()), (ats == null ? "" : ats.ToString()));
            }
            else
                requiredFields = SiteCache.GetRequiredFields(this.DeviceName);

            retVal = IsRequired(value, validationContext, requiredFields);
            if (retVal == ValidationResult.Success)
                retVal = IsInRange(value, validationContext, (controlType == null ? "" : controlType.ToString()));

            return retVal;
        }

        private ValidationResult ValidateLoadInfoData(object value)
        {
            if (this.PropertyName.ToUpper() == "PEAKDATE" || this.PropertyName.ToUpper() == "PEAKTIME")
            {
                return ValidateDateTime(this.PropertyName, value);
            }
            else if (this.PropertyName.ToUpper() == "SUMMER_LIMIT_DESC" || this.PropertyName.ToUpper() == "WINTER_LIMIT_DESC")
            {
                return IsMaxSizeExceed(value, CircuitLoadSWLimitDescMaxLength);
            }
            else
            {
                var range = SiteCache.GetRangeFields(this.DeviceName, this.PropertyName);
                if (range != null)
                {
                    if (value == null || string.IsNullOrEmpty(value.ToString()))
                        return ValidationResult.Success;
                    else
                    {
                        decimal objectValue;
                        if (decimal.TryParse(value.ToString(), out objectValue))
                        {
                            if (objectValue < range.Item1 || objectValue > range.Item2)
                                return new ValidationResult(this.FormatErrorMessage(string.Format("Value must be between {0} - {1}", range.Item1, range.Item2)));
                            else
                                return ValidationResult.Success;
                        }
                        else
                            return new ValidationResult(this.FormatErrorMessage(string.Format("Value must be between {0} - {1}", range.Item1, range.Item2)));
                    }
                }
                else
                    return ValidationResult.Success;
            }
        }


        private ValidationResult IsRequired(object value, ValidationContext validationContext, HashSet<string> requiredFields)
        {
            if (requiredFields != null && requiredFields.Count > 0)
            {

                if (requiredFields.Contains(PropertyName))
                {
                    if (value == null || string.IsNullOrEmpty(value.ToString()))
                        return new ValidationResult(this.FormatErrorMessage("Field is required."));
                    else
                        return ValidationResult.Success;
                }
                else
                    return ValidationResult.Success;
            }
            else
            {
                return ValidationResult.Success;
            }
        }

        private ValidationResult IsInRange(object value, ValidationContext validationContext, string controlType)
        {
            Tuple<decimal, decimal> range = null;

            // This is special code added for dynamic range. This request came me when system was in production
            // Correct way to handle this is to change the underlying table and framework to allow flexibility.
            // Because of time constraing we did this.
            if (this.DeviceName == "RECLOSER" &&
                (this.PropertyName == "SGF_MIN_TRIP_PERCENT" || this.PropertyName == "ALT_SGF_MIN_TRIP_PERCENT"
                || this.PropertyName == "ALT3_SGF_MIN_TRIP_PERCENT"))
            {
                if (controlType.ToString() == "6")
                    range = new Tuple<decimal, decimal>(0.5m, 100);
                else if (controlType == "4C")
                    range = new Tuple<decimal, decimal>(10, 100);
                else
                    range = SiteCache.GetRangeFields(this.DeviceName, PropertyName);
            }
            else
                range = SiteCache.GetRangeFields(this.DeviceName, PropertyName);

            decimal objectValue = 0;
            if (range != null)
            {
                if (value == null || string.IsNullOrEmpty(value.ToString()))
                    return ValidationResult.Success;
                else
                {
                    if (decimal.TryParse(value.ToString(), out objectValue))
                    {
                        if (objectValue < range.Item1 || objectValue > range.Item2)
                            return new ValidationResult(this.FormatErrorMessage(string.Format("Value must be between {0} - {1}", range.Item1, range.Item2)));
                        else
                            return ValidationResult.Success;
                    }
                    else
                        return new ValidationResult(this.FormatErrorMessage(string.Format("Value must be between {0} - {1}", range.Item1, range.Item2)));
                }
            }
            else
                return ValidationResult.Success;
        }

        private ValidationResult IsMaxSizeExceed(object value, int MaxLength)
        {
            if (value != null)
            {
                var count = value.ToString().Length;
                if (count > MaxLength)
                {
                    return new ValidationResult(this.FormatErrorMessage(string.Format("Number of characters can't be more than {0}.", MaxLength)));
                }
            }
            return ValidationResult.Success;
        }

        private ValidationResult ValidateDateTime(string propertyName, object value)
        {

            if (propertyName == "PEAKDATE")
            {
                if (value == null)
                    return new ValidationResult("Peak Date can't be null. Please select a valid Peak Date");
                else
                {
                    DateTime NewValue = new DateTime();
                    if (DateTime.TryParse(value.ToString(), out NewValue))
                    {
                        return ValidationResult.Success;
                    }
                    else
                    {
                        return new ValidationResult("Invalid Peak Date.");
                    }
                }
            }
            else if (propertyName == "PEAKTIME")
            {
                if (value == null)
                    return ValidationResult.Success;
                else
                {
                    var StrValue = value.ToString();
                    if (StrValue.Length != 4)
                    {
                        return new ValidationResult("Invalid Peak Time format. It must be in the 24 hours format - HHMM e.g. 2359 for 11:59pm.");
                    }
                    else
                    {
                        int NewIntValue;
                        if (int.TryParse(StrValue, out NewIntValue))
                        {
                            int hour = int.Parse(StrValue.Substring(0, 2));
                            int min = int.Parse(StrValue.Substring(2, 2));
                            if (hour >= 0 && hour <= 23 && min >= 0 && min <= 59)
                            {
                                return ValidationResult.Success;
                            }
                            else
                            {
                                return new ValidationResult("Invalid Peak Time. Hours must be between 00 and 23 and minutes must be between 00 and 59");
                            }
                        }
                        else
                        {
                            return new ValidationResult("Invalid Peak Time format. It must be in the 24 hours format - HHMM e.g. 2359 for 11:59pm.");
                        }
                    }
                }
            }
            else
            {
                return ValidationResult.Success;
            }

        }
    }

}