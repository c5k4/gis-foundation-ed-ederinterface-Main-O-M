// ========================================================================
// Copyright © 2006 Telvent, Consulting Engineers, Inc.
// <history>
// Status-based Field Required - EDER Component Specification
// Shaikh Rizuan 10/10/2012	Created
// </history>
// All rights reserved.
// ========================================================================
using System;
using System.Reflection;
using ESRI.ArcGIS.Geodatabase;
using log4net;
using Miner.Interop;
using PGE.Common.Delivery.Framework;
using PGE.Common.Delivery.Diagnostics;

namespace PGE.Desktop.EDER.ValidationRules.DateValidator
{
    /// <summary>
    /// Base class for Date validators.
    /// </summary>
   public class DateValidatorRule
    {
       private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        private static IMMConfigTopLevel _configTPL2 = null;
       private const string _invalidDateError ="Invalid date value is found.";
       private const string _invalidModelName = "Verify that the model names are assigned properly on {0}";

       #region Method Properties
       /// <summary>
       ///  set the IMMConfigTopLevel
       /// </summary>
       public static IMMConfigTopLevel ConfigTopLevel
       {
           get
           {
               if (_configTPL2 == null)
               {

                   Type type = Type.GetTypeFromProgID("mmGeodatabase.MMConfigTopLevel");
                   object mmObject = Activator.CreateInstance(type);
                   _configTPL2 = (IMMConfigTopLevel)mmObject;
               }
               return _configTPL2;
           }

       }
       #endregion

       /// <summary>
       /// Check for current year
       /// </summary>
       /// <param name="objToValidate">i.e esri IObject</param>
       /// <param name="maxYear">current year</param>
       /// <param name="modelNames">Field model name</param>
       /// <param name="errorMsg">Out error message if field value greater than current year</param>
        /// <returns>Return "true" field value less than current year or "false" if field value greater than current year </returns>
       public static bool MaximumRangeYear(IObject objToValidate, int maxYear, string modelNames, out string errorMsg)
       {
           bool validationResult = false;
           errorMsg = string.Empty;
           int fldDateValue = -1;
           int fldIndex = ModelNameFacade.FieldIndexFromModelName(objToValidate.Class, modelNames);
           

           //Get primary display field value for object/feature
           //object primaryDispValue = GetPrimaryDisplayFldValue(objToValidate);

           if (fldIndex != -1)//if model name not assigned
           {
               IField pField = ModelNameFacade.FieldFromModelName(objToValidate.Class, modelNames);

               //Check for field type
               esriFieldType fldType = pField.Type;
               object fldValue = objToValidate.get_Value(fldIndex);
               if (fldValue !=DBNull.Value &&!string.IsNullOrEmpty(fldValue.ToString()))
               {
                   if (fldType == esriFieldType.esriFieldTypeDate)
                   {
                       DateTime dateTimeFldValue = Convert.ToDateTime(fldValue);
                       fldDateValue = dateTimeFldValue.Year;
                   }
                   else
                   {
                       if (!int.TryParse(fldValue.ToString(), out fldDateValue))
                       {
                           errorMsg = _invalidDateError;
                           return validationResult;
                       }
                   }
               }
              
               if (fldDateValue != -1)//check for field value
               {
                   if (fldDateValue > maxYear) // if field value greater than current year
                   {

                    
                           errorMsg = pField.AliasName + " of " + fldDateValue.ToString() + " is invalid. It is in the future.";

                   }
                  
                   else //if less than curent year
                   {
                       validationResult = true;
                   }
               }

           }
           else //if model name not assigned
           {
               
               _logger.Debug(string.Format(_invalidModelName,objToValidate.Class.AliasName));
           }
           return validationResult;
       }

       /// <summary>
       /// Check for the minimum year
       /// </summary>
       /// <param name="objToValidate">i.e. esri object</param>
       /// <param name="minYear">Minimum year</param>
       /// <param name="modelNames">Field model name </param>
       /// <param name="errorMsg">Out error message if field value smaller than current year</param>
       /// <param name="validationType">Validation name</param>
       /// <returns>Return "true" field value greater than minimum range year or "false" if field value smaller than minimum range year</returns>
       public static bool MinimumRangeYear(IObject objToValidate, int minYear, string modelNames, out string errorMsg)
       {
            bool validationResult = false;
           errorMsg = string.Empty;
           int fldDateValue=-1;
           int fldIndex = ModelNameFacade.FieldIndexFromModelName(objToValidate.Class, modelNames);
           

           //Get primary display field value
           //object primaryDispValue = GetPrimaryDisplayFldValue(objToValidate);

           if (fldIndex != -1)//check if model name not assigned
           {
               IField pField = ModelNameFacade.FieldFromModelName(objToValidate.Class, modelNames);

               //Check for field type
               esriFieldType fldType = pField.Type;
               object fldValue = objToValidate.get_Value(fldIndex);
               if (fldValue!=DBNull.Value && !string.IsNullOrEmpty(fldValue.ToString())) //if field value is not null
               {
                   if (fldType == esriFieldType.esriFieldTypeDate)
                   {
                       DateTime fldDateTimeValue = Convert.ToDateTime(fldValue);
                       fldDateValue = fldDateTimeValue.Year;
                   }
                   else
                   {
                       if (!int.TryParse(fldValue.ToString(), out fldDateValue))
                       {
                           errorMsg = _invalidDateError;
                           return validationResult;
                       }
                   }
               }
               else
               {
                   _logger.Warn(pField.AliasName + " field value is <Null>.");
                   return validationResult;
               }

               if (fldDateValue < minYear)//if field value less than minimum year
               {
                   
                 
                           errorMsg = pField.AliasName + " of " + fldDateValue.ToString() + " is before " + minYear +".";
           
               }
              
               else // if greater than minimum year
               {
                   validationResult = true;
               }
           }
           else // if model name not assigned
           {
               //errorMsg = "Verify that the model names are assigned properly";
               _logger.Debug(string.Format(_invalidModelName,objToValidate.Class.AliasName));
           }
           return validationResult;
       }


       /// <summary>
       /// Check for the minimum range date
       /// </summary>
       /// <param name="objToValidate">i.e. esri object</param>
       /// <param name="minDate">Minimum Date</param>
       /// <param name="modelNames">Field model name </param>
       /// <param name="errorMsg">Out error message if field value smaller than current year</param>
       /// <param name="validationType">Validation name</param>
       /// <returns>Return "true" field value greater than minimum date or "false" if field value less than minimum date</returns>
       public static bool MinimumRangeDate(IObject objToValidate, DateTime minDate, string modelNames, out string errorMsg)
       {
           
           bool validationResult = false;
           errorMsg = string.Empty;
           int fldIndex = ModelNameFacade.FieldIndexFromModelName(objToValidate.Class, modelNames);

           //get the primary display field value
           //object primaryDispValue = GetPrimaryDisplayFldValue(objToValidate);

           if (fldIndex != -1) // check for model name assigned and field type
           {
               IField pField = ModelNameFacade.FieldFromModelName(objToValidate.Class, modelNames);

               //get the field type
               esriFieldType fldType = pField.Type;
               if (fldType == esriFieldType.esriFieldTypeDate)
               {
                   object fldValue = objToValidate.get_Value(fldIndex);
                   if (fldValue != DBNull.Value && !string.IsNullOrEmpty(fldValue.ToString()))
                   {
                       DateTime fldDateValue = Convert.ToDateTime(fldValue);

                       int result = DateTime.Compare(fldDateValue, minDate);
                       if (result < 0)// if field value less than the minimum date
                       {

                           errorMsg = pField.AliasName + " of " + fldDateValue.ToString() + " is before " + minDate + ".";


                       }

                       else //if greater than minimum date
                       {
                           // no error
                           validationResult = true;
                       }
                   }
               }

           }
           else // if model name or field type is not proper or assigned
           {

               _logger.Warn(string.Format(_invalidModelName, objToValidate.Class.AliasName));
           }
           return validationResult;
           
       }

       /// <summary>
       /// Get primary display field value frim object
       /// </summary>
       /// <param name="obj">esri IObject</param>
       /// <returns>return primary display field value as object</returns>
        public static object GetPrimaryDisplayFldValue(IObject obj)
        {
           
            IMMFeatureClass mmFeatureClass = ConfigTopLevel.GetFeatureClassOnly(obj.Class);
            string primaryDisplayField = mmFeatureClass.PriDisplayField;
            object primaryDisplayFieldValue=null;
            
            if (!string.IsNullOrEmpty(primaryDisplayField))
            {
                int fldIndex = obj.Fields.FindField(primaryDisplayField);
                if (fldIndex != -1)
                {
                    primaryDisplayFieldValue = obj.get_Value(fldIndex);
                }
            }
            else
            {
                _logger.Debug("Feature does not have primary display field value.");
            }
            return primaryDisplayFieldValue;
        }

        
       /// <summary>
       /// Check for maximum range date
       /// </summary>
       /// <param name="objToValidate">esri IObject</param>
       /// <param name="maxDate">Cuuent Date</param>
       /// <param name="modelNames">Field model name</param>
        /// <param name="errorMsg">Out error message if field value greater than current date</param>
       /// <returns>return "true" if less than current date or "false" greater than current date</returns>
        public static bool MaximumRangeDate(IObject objToValidate, DateTime maxDate, string modelNames, out string errorMsg)
       {
           bool validationResult = false;
           errorMsg = string.Empty;
           int fldIndex = ModelNameFacade.FieldIndexFromModelName(objToValidate.Class, modelNames);

           
            //get the primary display field value
           //object primaryDispValue = GetPrimaryDisplayFldValue(objToValidate);

           if (fldIndex != -1)// check for model name and field type
           {
               IField pField = ModelNameFacade.FieldFromModelName(objToValidate.Class, modelNames);
               //get the field type
               esriFieldType fldType = pField.Type;

               if (fldType == esriFieldType.esriFieldTypeDate)
               {
                   object fldValue = objToValidate.get_Value(fldIndex);
                   if (fldValue != DBNull.Value && !string.IsNullOrEmpty(fldValue.ToString()))// check for field value
                   {
                       DateTime fldDateValue = Convert.ToDateTime(fldValue);

                       int result = DateTime.Compare(fldDateValue, maxDate);

                       if (result <= 0) // if field value less than current date
                       {
                           validationResult = true;
                           //no error
                       }

                       else // if grater than current date
                       {
                           // error

                           errorMsg = pField.AliasName + " of " + fldDateValue.ToString() + " is invalid. It is in the future.";

                       }
                   }
               }

           }
           else //if model name or field type are not proper 
           {
               
               
               _logger.Warn(string.Format(_invalidModelName,objToValidate.Class.AliasName));
           }
           return validationResult;
       }

       /// <summary>
       /// compare two dates and find whether compare1 is greater than compare2
       /// </summary>
       /// <param name="compareModelName1">first field model name</param>
       /// <param name="compareModelName2">second field model name to compare</param>
       /// <param name="objToValidate">esri IObject</param>
       /// <param name="errorMsg">out error message if compare1 date is less than comapre2</param>
       /// <param name="validationType">validation name</param>
       /// <returns>return "true" if compare1 date greater than compare2 and "false" if less than compare2</returns>
        public static bool DateGreaterThanCompare(string compareModelName1, string compareModelName2, IObject objToValidate, out string errorMsg)
       {
           bool validationResult = false;
           errorMsg = string.Empty;
           int compare1FldIndex = ModelNameFacade.FieldIndexFromModelName(objToValidate.Class, compareModelName1);
           

           int compare2FldIndex = ModelNameFacade.FieldIndexFromModelName(objToValidate.Class, compareModelName2);
          

            //get the primary display field value
           //object primaryDispValue = GetPrimaryDisplayFldValue(objToValidate);

            //Check for model name and field type
           if (compare1FldIndex != -1 && compare2FldIndex != -1 )
           {
               IField compare1Fld = ModelNameFacade.FieldFromModelName(objToValidate.Class, compareModelName1);

               // get the field type
               esriFieldType compare1FldType = compare1Fld.Type;

               IField compare2Fld = ModelNameFacade.FieldFromModelName(objToValidate.Class, compareModelName2);

               //get the field type
               esriFieldType compare2FldType = compare2Fld.Type;

               if (compare1FldType == esriFieldType.esriFieldTypeDate && compare2FldType == esriFieldType.esriFieldTypeDate)
               {
                   object compare1FldValue = objToValidate.get_Value(compare1FldIndex);
                   object comapre2FldValue = objToValidate.get_Value(compare2FldIndex);

                   //check if field values are null or empty
                   if (compare1FldValue != DBNull.Value && !string.IsNullOrEmpty(compare1FldValue.ToString()) && comapre2FldValue != DBNull.Value && !string.IsNullOrEmpty(comapre2FldValue.ToString()))
                   {
                       DateTime compare1Date = Convert.ToDateTime(compare1FldValue);
                       DateTime compare2Date = Convert.ToDateTime(comapre2FldValue);

                       int result = DateTime.Compare(compare1Date, compare2Date);

                       if (result < 0)//if compare1 date is less than compare2 date
                       {
                           // error


                           errorMsg = compare1Fld.AliasName + " of " + compare1FldValue.ToString()  + " is invalid. It is before the " + compare2Fld.AliasName + " " + comapre2FldValue.ToString() + ".";


                       }

                       else // if greater than comapre2 date
                       {
                           //no error
                           validationResult = true;

                       }
                   }
               }
           }
           else // if model name or field type not proper
           {
               _logger.Warn(string.Format(_invalidModelName, objToValidate.Class.AliasName));

           }
           return validationResult;
       }

       /// <summary>
       /// compare two years and find whether compare1 is greater than compare2
       /// </summary>
       /// <param name="compareModelName1">first field model name</param>
       /// <param name="compareModelName2">second field model name compare to</param>
       /// <param name="objToValidate">esri IObject</param>
       /// <param name="errorMsg">out error message if compare1 year is less than compare2</param>
       /// <param name="validationType">validation name</param>
       /// <returns>return "true" if compare1 year greater than compare2 and "false" if less than compare2</returns>
        public static bool YearGreaterThanComapre(string compareModelName1, string compareModelName2, IObject objToValidate, out string errorMsg)
       {
           bool validationResult = false;
           errorMsg = string.Empty;
           int compare1Year = -1;
           int compare2Year = -1;

           int compare1FldIndex = ModelNameFacade.FieldIndexFromModelName(objToValidate.Class, compareModelName1);
           

           int compare2FldIndex = ModelNameFacade.FieldIndexFromModelName(objToValidate.Class, compareModelName2);
          

            //get primary display field value
           //object primaryDispValue = GetPrimaryDisplayFldValue(objToValidate);

           if (compare1FldIndex != -1 && compare2FldIndex != -1)//check for field model name
           {
               IField compare1Fld = ModelNameFacade.FieldFromModelName(objToValidate.Class, compareModelName1);

               //get the field type
               esriFieldType compare1FldType = compare1Fld.Type;

               IField compare2Fld = ModelNameFacade.FieldFromModelName(objToValidate.Class, compareModelName2);

               //get the field type
               esriFieldType compare2FldType = compare2Fld.Type;

               object compare1FldValue = objToValidate.get_Value(compare1FldIndex);
               object compare2FldValue = objToValidate.get_Value(compare2FldIndex);

               //check for field value is null or empty
               if (compare1FldValue !=DBNull.Value && !string.IsNullOrEmpty(compare1FldValue.ToString()) && compare2FldValue != DBNull.Value && !string.IsNullOrEmpty(compare2FldValue.ToString()))
               {

                   if (compare1FldType == esriFieldType.esriFieldTypeDate)
                   {
                       DateTime compare1DateTime = Convert.ToDateTime(compare1FldValue);
                       compare1Year = compare1DateTime.Year;
                   }
                   else
                   {
                       if (!int.TryParse(compare1FldValue.ToString(), out compare1Year))
                       {
                           errorMsg = _invalidDateError;
                           return validationResult;
                       }
                   }

                   if (compare2FldType == esriFieldType.esriFieldTypeDate)
                   {
                       DateTime compare2DateTime = Convert.ToDateTime(compare2FldValue);
                       compare2Year = compare2DateTime.Year;
                   }
                   else
                   {
                       if (!int.TryParse(compare2FldValue.ToString(), out compare2Year))
                       {
                           errorMsg = _invalidDateError;
                           return validationResult;
                       }
                   }
               }
               if (compare1Year != -1 && compare2Year != -1)
               {
                   if (compare1Year >= compare2Year) // if compare1 is greater than compare2
                   {
                       validationResult = true;
                   }
                 
                   else // if less than compare2
                   {
                      
                               errorMsg = compare1Fld.AliasName + " of " + compare1FldValue.ToString()  + " is invalid. It is before the " + compare2Fld.AliasName + " " + compare2FldValue.ToString() +".";              
                   }
               }
           }
           else // if model names are not assigned
           {
               _logger.Warn(string.Format(_invalidModelName, objToValidate.Class.AliasName));

           }
           return validationResult;
       }

       /// <summary>
       /// Compare two years and find whether compare1 year is smaller than compare2
       /// </summary>
       /// <param name="compareModelName1">first field model name</param>
       /// <param name="compareModelName2">second field model name compare to</param>
       /// <param name="objToValidate">esri IObject</param>
       /// <param name="errorMsg">out error message if compare1 year is greater than compare2</param>
       /// <param name="validationType">validation name</param>
        /// <returns>return "true" if compare1 year less than compare2 and "false" if greater than compare2</returns>
        public static bool YearLessThanComapre(string compareModelName1, string compareModelName2, IObject objToValidate, out string errorMsg)
       {
           bool validationResult = false;
           errorMsg = string.Empty;
           int compare1Year = -1;
           int compare2Year = -1;

           int compare1FldIndex = ModelNameFacade.FieldIndexFromModelName(objToValidate.Class, compareModelName1);
           

           int compare2FldIndex = ModelNameFacade.FieldIndexFromModelName(objToValidate.Class, compareModelName2);
          
             
            //get the primary display field value
           //object primaryDispValue = GetPrimaryDisplayFldValue(objToValidate);

           if (compare1FldIndex != -1 && compare2FldIndex != -1)// check if model name assigned
           {
               IField compare1Fld = ModelNameFacade.FieldFromModelName(objToValidate.Class, compareModelName1);

               //get the field type
               esriFieldType compare1FldType = compare1Fld.Type;

               IField compare2Fld = ModelNameFacade.FieldFromModelName(objToValidate.Class, compareModelName2);

               //get the field type
               esriFieldType compare2FldType = compare2Fld.Type;
               object compare1FldValue = objToValidate.get_Value(compare1FldIndex);
               object comapare2FldValue = objToValidate.get_Value(compare2FldIndex);
               if (compare1FldValue != DBNull.Value && !string.IsNullOrEmpty(compare1FldValue.ToString()) && comapare2FldValue != DBNull.Value && !string.IsNullOrEmpty(comapare2FldValue.ToString()))//check if field value is null or empty
               {
                   if (compare1FldType == esriFieldType.esriFieldTypeDate)
                   {
                       DateTime compare1DateTime = Convert.ToDateTime(compare1FldValue);
                       compare1Year = compare1DateTime.Year;
                   }
                   else
                   {
                       if (!int.TryParse(compare1FldValue.ToString(), out compare1Year))
                       {
                           errorMsg = _invalidDateError;
                           return validationResult;
                       }
                   }


                   if (compare2FldType == esriFieldType.esriFieldTypeDate)
                   {
                       DateTime compare2DateTime = Convert.ToDateTime(comapare2FldValue);
                       compare2Year = compare2DateTime.Year;
                   }
                   else
                   {
                       if (!int.TryParse(comapare2FldValue.ToString(), out compare2Year))
                       {
                           errorMsg = _invalidDateError;
                           return validationResult;
                       }
                   }
               }

               if (compare1Year != -1 && compare2Year != -1) // checkfor integer value
               {
                   if (compare1Year <= compare2Year) //if compare1 field value less than comapre2
                   {
                       validationResult = true;
                   }
                   
                   else // if greater than comapre2
                   {
                     
                               errorMsg = compare1Fld.AliasName + " of " + compare1FldValue.ToString() + " is invalid. It is after the " + compare2Fld.AliasName + " " + comapare2FldValue.ToString()+".";
                        
                   }
               }
           }
           else // if model name are not assigned
           {
               _logger.Warn(string.Format(_invalidModelName, objToValidate.Class.AliasName));


           }
           return validationResult;
       }

       /// <summary>
       /// Compare two dates and find whether compare1 date is smaller than compare2 date
       /// </summary>
        /// <param name="compareModelName1">first field model name</param>
        /// <param name="compareModelName2">second field model name compare to</param>
        /// <param name="objToValidate">esri IObject</param>
        /// <param name="errorMsg">out error message if compare1 date is greater than compare2</param>
        /// <returns>return "true" if compare1 date less than compare2 and "false" if greater than compare2</returns>
        public static bool DateSmallerThanCompare(string compareModelName1, string compareModelName2, IObject objToValidate, out string errorMsg)
       {
           bool validationResult = false;
           errorMsg = string.Empty;
           int compare1FldIndex = ModelNameFacade.FieldIndexFromModelName(objToValidate.Class, compareModelName1);
          

           int compare2FldIndex = ModelNameFacade.FieldIndexFromModelName(objToValidate.Class, compareModelName2);
          

            //get the primary display field value
           //object primaryDispValue = GetPrimaryDisplayFldValue(objToValidate);

            // check for model name and field type
           if (compare1FldIndex != -1 && compare2FldIndex != -1)
           {
               IField compare1Fld = ModelNameFacade.FieldFromModelName(objToValidate.Class, compareModelName1);

               //get the field type
               esriFieldType compare1FldType = compare1Fld.Type;
               IField compare2Fld = ModelNameFacade.FieldFromModelName(objToValidate.Class, compareModelName2);

               // get the field type
               esriFieldType compare2FldType = compare2Fld.Type;
               if (compare1FldType == esriFieldType.esriFieldTypeDate && compare2FldType == esriFieldType.esriFieldTypeDate)
               {
                   object compare1FldValue = objToValidate.get_Value(compare1FldIndex);
                   object comapre2FldValue = objToValidate.get_Value(compare2FldIndex);

                   if (compare1FldValue != DBNull.Value && !string.IsNullOrEmpty(compare1FldValue.ToString()) && comapre2FldValue != DBNull.Value && !string.IsNullOrEmpty(comapre2FldValue.ToString()))// icheck for field value is null or empty
                   {
                       DateTime compare2Date = Convert.ToDateTime(comapre2FldValue);
                       DateTime compare1Date = Convert.ToDateTime(compare1FldValue);

                       int result = DateTime.Compare(compare1Date, compare2Date);

                       if (result <= 0) //if compare1 field value less than compare2
                       {
                           validationResult = true;
                           //no error
                       }

                       else // if greater than comapre2
                       {
                           // error

                           errorMsg = compare1Fld.AliasName + " of " + compare1FldValue.ToString() + " is invalid. It is after the " + compare2Fld.AliasName + comapre2FldValue.ToString() + ".";



                       }
                   }
               }
           }
           else // if model name and field type are not proper
           {
               _logger.Warn(string.Format(_invalidModelName, objToValidate.Class.AliasName));

           }
           return validationResult;
       }
    }
}
