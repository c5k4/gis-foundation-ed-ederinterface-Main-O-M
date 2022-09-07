using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.Geodatabase;

using Miner.Interop;
using Miner.ComCategories;

using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Framework;

using log4net;
using PGE.Common.Delivery.Diagnostics;

namespace PGE.Desktop.EDER.ValidationRules
{
    [Guid("520c0ae3-42dd-49e4-935c-f8d06909466b")]
    [ProgId("PGE.Desktop.EDER.ValidateVaultDimensions")]
    [ComponentCategory(ComCategory.MMValidationRules)] 
    public class ValidateVaultDimensions:BaseValidationRule 
    {
        private static readonly Log4NetLogger _logger = new
            Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        private const string WarnMissingModelNames = "Missing object class or field model names:\r\n{0}";
        private const int MAX_DECIMAL = 2;
        private const string SPACE = " ";
        private const string APOSTROPHE = "'";
        private const string DOUBLE_QUOTE = "\"";  
        private const string DECIMAL_POINT = ".";
        private const string SLASH = @"/";
        private const string CAPITAL_X = "X";

        public ValidateVaultDimensions()
            : base("PGE Validate Vault Dimensions", SchemaInfo.Electric.ClassModelNames.SubSurfaceStructure)
        { }

        /// <summary>
        /// Validates the format of the Vault Sizing it should be 
        /// in the following format: 
        /// L'XW'XH' - so length (feet/inches) X width (feet/inches) X hight (feet/inches) 
        /// e.g. 6'X8'X10' 
        /// - Disallow fractions eg 6 1/2' by looking for "/" 
        /// - Should enter values in feet - disallow inches " 
        /// </summary>
        /// <param name="row">Row to be validated</param>
        /// <returns>Returns the error list after validation</returns>
        protected override Miner.Interop.ID8List InternalIsValid(IRow row)
        {

            //return null; 

            //Validate input row
            IObject obj = row as IObject;
            if (obj == null) return _ErrorList;

            //Get the value entered in the structSize field 
            object structSize = (row as IObject).GetFieldValue(
                null, false, SchemaInfo.Electric.FieldModelNames.StructureSize);

            if (!(structSize is System.DBNull))
            {
                //Validate the format of the user entered structuresize 
                List<string> errorList = ValidateVaultDimensionFormat(structSize.ToString());
                if (errorList.Count > 0)
                {
                    for (int i = 0; i < errorList.Count; i++)
                    {
                        AddError(errorList[i]);
                    }
                }
            }

            return _ErrorList;
        }

        /// <summary>
        /// Validates the string format of the STRUCTURESIZE field for the 
        /// SubsurfaceStructure according to business rules 
        /// -no fractions 
        /// -no decimals 
        /// -no spaces 
        /// </summary>
        /// <param name="structureSize"></param>
        /// <returns></returns>
        private List<string> ValidateVaultDimensionFormat(string structureSize)
        {
            string errorMsg = "";
            List<string> errorList = new List<string>();

            try
            {
                if (!structureSize.Contains(CAPITAL_X))
                {
                    errorMsg = "StructureSize field dimensions (length, width etc) MUST be separated by a large caps '" +
                        CAPITAL_X + "'. No '" + CAPITAL_X + "' was entered! e.g. "  + @"10'6" + ((char)34).ToString() + "X8’X10’";
                    if (!errorList.Contains(errorMsg))
                        errorList.Add(errorMsg);
                }
                else
                {
                    //Split the string using the x and validate each dimension 
                    //individually 
                    string[] dimensions = structureSize.Split(CAPITAL_X.ToCharArray());

                    for (int i = 0; i < dimensions.Length; i++)
                    {
                        string dimension = dimensions[i];

                        //Dimension cannot have spaces 
                        if (dimension.Contains(SPACE))
                        {
                            errorMsg = "StructureSize field dimensions (length, width etc) MUST be specified in feet/inches without spaces. e.g. " + @"10'6" + ((char)34).ToString() + "X8'X10'";
                            if (!errorList.Contains(errorMsg))
                                errorList.Add(errorMsg);
                        }

                        //Dimension cannot contain fractions 
                        if (dimension.Contains(SLASH))
                        {
                            errorMsg = "StructureSize field dimensions (length, width etc) MUST be numeric values in feet/inches without fractions e.g. " + @"10'6" + ((char)34).ToString() + "X8'X10'";
                            if (!errorList.Contains(errorMsg))
                                errorList.Add(errorMsg);
                        }

                        //Dimension cannot contain decimals 
                        if (dimension.Contains(DECIMAL_POINT))
                        {
                            errorMsg = "StructureSize field dimensions (length, width etc) MUST be numeric values in feet/inches without decimals e.g. " + @"10'6" + ((char)34).ToString() + "X8'X10'";
                            if (!errorList.Contains(errorMsg))
                                errorList.Add(errorMsg);
                        }

                        //Dimension must be terminated with apostrophe or double quote   
                        if ((!dimension.EndsWith(APOSTROPHE)) && (!dimension.EndsWith(DOUBLE_QUOTE)))
                        {
                            errorMsg = "StructureSize field dimensions (length, width etc) MUST end with an apostrophe (for feet) or a double quote (for inches). e.g. " + @"10'6" + ((char)34).ToString() + "X8'X10'";
                            if (!errorList.Contains(errorMsg))
                                errorList.Add(errorMsg);
                        }
                        else 
                        {
                        
                            //Extract the feet and inches from the dimension
                            int posOfFeet = dimension.IndexOf(APOSTROPHE);
                            int posOfInch = dimension.IndexOf(DOUBLE_QUOTE); 
                            string feetString = "";
                            string inchString = "";
                            string dimensionString = ""; 

                            if (posOfFeet != -1)
                                feetString = dimension.Substring(0, posOfFeet);
                            if (posOfInch != -1)
                            {
                                if (posOfFeet != -1)
                                    inchString = dimension.Substring(posOfFeet + 1, posOfInch - (posOfFeet + 1));
                                else
                                    inchString = dimension.Substring(0, posOfInch - 1); 
                            }                                                        

                            //feet / inches must be castable to a double if they are numeric 
                            for (int j = 1; j < 3; j++)
                            {
                                if (j == 1)
                                    dimensionString = feetString;
                                else
                                    dimensionString = inchString;
                                double doubleVal = 0;
                                if (dimensionString != string.Empty)
                                {
                                    if (!double.TryParse(dimensionString, out doubleVal))
                                    {
                                        errorMsg = "StructureSize field dimensions (length, width etc) MUST be numeric values in feet/inches (no fractions). e.g. " + @"10'6" + ((char)34).ToString() + "X8'X10'";
                                        if (!errorList.Contains(errorMsg))
                                            errorList.Add(errorMsg);
                                    }

                                    if (j == 2)
                                    {
                                        if (doubleVal == 0)
                                        {
                                            errorMsg = "StructureSize field dimensions (length, width etc) MUST omit the inches entry where the number of inches is zero. e.g. " + @"10'6" + ((char)34).ToString() + "X8'X10'";
                                            if (!errorList.Contains(errorMsg))
                                                errorList.Add(errorMsg);
                                        }
                                        if (doubleVal >= 12)
                                        {
                                            errorMsg = "StructureSize field dimensions (length, width etc) MUST be numeric values in feet/inches where the number of inches < 12. e.g. " + @"10'6" + ((char)34).ToString() + "X8'X10'";
                                            if (!errorList.Contains(errorMsg))
                                                errorList.Add(errorMsg);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                //Return the list of errors 
                return errorList;
            }
            catch (Exception ex)
            {
                throw new Exception("Error validating rule: " + base._Name); 
            }
        }
    }
}
