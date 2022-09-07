// ========================================================================
// Copyright © 2006 Telvent, Consulting Engineers, Inc.
// <history>
// Status-based Field Required - EDER Component Specification
// Shaikh Rizuan 10/10/2012	Created
// </history>
// All rights reserved.
// ========================================================================
using System;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geodatabase;
using Miner.ComCategories;
using Miner.Interop;
using PGE.Common.Delivery.ArcFM;
using log4net;
using System.Reflection;
using PGE.Common.Delivery.Diagnostics;
using PGE.Common.Delivery.Framework;
using System.Collections.Generic;
using ESRI.ArcGIS.esriSystem;

namespace PGE.Desktop.EDER.ValidationRules.DateValidator
{
    /// <summary>
    /// Validate InstallationDate and InstallJobYear field that is present on many feature Classes and objects.
    /// </summary>
    [ComVisible(true)]
    [Guid("930487F2-F956-45FE-86B9-52E0F8CA544A")]
    [ProgId("PGE.Desktop.EDER.ValidateInstallationDate")]
    [ComponentCategory(ComCategory.MMValidationRules)]
    public class ValidateInstallationDate : BaseValidationRule
    {
        #region Class variables

        private const int _minYear = 1900;
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        private static string[] _modelNames = new string[] {SchemaInfo.Electric.FieldModelNames.InstallationDate,SchemaInfo.Electric.ClassModelNames.validateChildField};
        private IRelationshipClass _relClass = null;
        private const string errorForBeforeFloorDte = "{0} (ObjectID: {1}) {2} of {3} is invalid. It is before {4}.";
        private const string errorForAfterDate = "{0} (ObjectID: {1}) {2} of {3} is invalid. It is in the future.";
        private const string errorForComprDate = "{0} of {1} is invalid. It is after the {2} (objectID: {3}) {4} of {5}.";

        #endregion

        #region Constructors
        /// <summary>
        /// Constructor.
        /// </summary>
        public ValidateInstallationDate(): base("PGE Validate Installation Date",_modelNames )
        {
        }
        #endregion

        #region Base Validation Rule Overrides

         /// Validate the row.
        /// </summary>
        /// <param name="row"> The row data being validated with Generator table </param>
        /// <returns> A list of errors or nothing. </returns>
        /// 
        protected override ID8List InternalIsValid(IRow row)
        {

            #region Variable Declarations
            IObject obj = row as IObject;
            string errorMsg = string.Empty;
            string[] modelNames = new string[] {SchemaInfo.Electric.FieldModelNames.InstallationDate,SchemaInfo.Electric.FieldModelNames.InstallationJobYear};
            DateTime currentDate = DateTime.Today; // current date
            int maxYear = currentDate.Year; // current year
            ESRI.ArcGIS.esriSystem.ISet relatedSet = null;
            bool missingInstallDate =false;
            bool missingInstallJob =false;
            #endregion

            #region Check First Condition:Year less than the floor date and Check Second condition :  Year greater than cureent year
            //get field value for installation date
            var fieldInstallationDate = FieldInstance.FromModelName(obj, SchemaInfo.Electric.FieldModelNames.InstallationDate);
            DateTime installationDate = DateTime.MinValue;
            if (fieldInstallationDate != null)//if installation Date is not null
            {
                installationDate = fieldInstallationDate.Value.Convert(DateTime.MinValue);
                _logger.Debug(fieldInstallationDate.Alias + "value: " + installationDate.ToShortDateString());
                //If installation Date is less than 1900
                if (installationDate != DateTime.MinValue && installationDate.Year < _minYear)
                    AddError(fieldInstallationDate.Alias + " of " + installationDate.ToShortDateString() + " is invalid. It is before " + _minYear + ".");
                //if installation Date is greater than current date
                if (installationDate.Year > currentDate.Year)
                    AddError(fieldInstallationDate.Alias + " of " + installationDate.ToShortDateString() + " is invalid. It is in the future.");
                
            }
            else
            {         
                //Get ESRI ISet from relationshipClass
                     relatedSet = GetRelatedSet(obj);
                    if (relatedSet != null && relatedSet.Count > 0) //if count greater than zero
                    {
                        _logger.Debug("Relatd Object Count: " + relatedSet.Count);
                        missingInstallDate =true;//if install date is missing ..make it true
                        IObject relObj = null;
                        //loop through each object
                        while ((relObj = relatedSet.Next() as IObject) != null)
                        {
                            _logger.Debug("Related ObjectID" + relObj.OID);
                            //Check installation date for each related object
                            AddErrorForRelObject(relObj);
                        }
                    }         
            }
            //get field value for installation date
            var fieldInstallationJobYear = FieldInstance.FromModelName(obj, SchemaInfo.Electric.FieldModelNames.InstallationJobYear);
            int installationYear = DateTime.MinValue.Year;
            if (fieldInstallationJobYear != null)
            {
                installationYear = fieldInstallationJobYear.Value.Convert(DateTime.MinValue.Year);
                _logger.Debug(fieldInstallationJobYear.Alias + " value: " + installationYear);
                //Check if install job year less than 1900
                if (installationYear != DateTime.MinValue.Year && installationYear < _minYear)
                    AddError(fieldInstallationJobYear.Alias + " of " + installationYear + " is invalid. It is before " + _minYear + ".");
                //Check if install job year greater than current year
                if (installationYear != DateTime.MinValue.Year && installationYear > currentDate.Year)
                    AddError(fieldInstallationJobYear.Alias + " of " + installationYear + " is invalid. It is in the future.");
            }
            else
            {
                //Get ESRI ISet from relationshipClass
                relatedSet = GetRelatedSet(obj);
                if (relatedSet != null && relatedSet.Count > 0)
                {
                    _logger.Debug("Relatd Object Count: " + relatedSet.Count);
                    missingInstallJob = true;//if install job is missing ..make it true
                    IObject relObj = null;
                    //loop through each object
                    while ((relObj = relatedSet.Next() as IObject) != null)
                    {
                        //Check install job year for each related object
                        AddErrorForRelObjectJobYear(relObj);
                        _logger.Debug("Related ObjectID" + relObj.OID);
                    }
                }      
            }
            #endregion 

            #region Check tird condition comparison between install jab year and Insatllation date if JobYr greater than InstallDate
            //if install date and install job not equals to null
            if (fieldInstallationDate != null && fieldInstallationJobYear != null)
            {
                //if install job > install date
                if (installationYear > installationDate.Year)
                {
                    AddError(fieldInstallationJobYear.Alias + " of " + installationYear + " is invalid. It is after the " + fieldInstallationDate.Alias + " of " + installationDate.ToShortDateString());
                }
            }
            else
            {
                if (relatedSet != null && relatedSet.Count > 0)//if count >0
                {
                    if (missingInstallDate)//if true
                    {
                        AddErrorForRelObjectComparisonYear(relatedSet, installationYear, SchemaInfo.Electric.FieldModelNames.InstallationDate, DateTime.MinValue, fieldInstallationJobYear.Alias);
                    }
                    if (missingInstallJob)//if true
                    {
                        AddErrorForRelObjectComparisonYear(relatedSet,installationDate, SchemaInfo.Electric.FieldModelNames.InstallationJobYear,fieldInstallationDate.Alias);
                    }
                    
                }      
            }
            #endregion

            #region Old Code
            ////////////////////////////////////////////////////
            //if (!(installationDate == DateTime.MinValue || installationYear == DateTime.MinValue.Year))
            //{
            //    const string errorFormat = "{0} of {1} is invalid. It is after the {2}, which is {3}.";
            //    if(installationDate.Year > installationYear)
            //        AddError(string.Format(errorFormat, fieldInstallationDate.Alias, installationDate.ToShortDateString(), fieldInstallationJobYear.Alias, installationYear));
            //    else if (installationDate.Year < installationYear)
            //        AddError(string.Format(errorFormat, fieldInstallationJobYear.Alias, installationYear, fieldInstallationDate.Alias, installationDate.ToShortDateString()));
            //}

            //if (!(installationDate == DateTime.MinValue))
            //{
            //    const string errorformat2 = "{0} of {1} is invalid.  It is after the current year, which is {2}.";
            //    if (installationDate.Year > DateTime.Now.Year)
            //    {
            //        AddError(string.Format(errorformat2, fieldInstallationDate.Alias, installationDate.ToShortDateString(), DateTime.Now.Year.ToString()));
            //    }
            //} 
            #endregion

            return _ErrorList;
        }

        /// <summary>
        /// Check if install job year is greater than installation date
        /// </summary>
        /// <param name="relatedSet">ESRI ISet</param>
        /// <param name="installationJobYear">install job year field vlaue</param>
        /// <param name="modelName">model name</param>
        /// <param name="dateTime">Date time format</param>
        /// <param name="InstallJoYearalisasNme">install job year field name</param>
        private void AddErrorForRelObjectComparisonYear(ISet relatedSet, int installationJobYear, string modelName, DateTime dateTime, string InstallJobYearAliasName)
        {
            if (relatedSet != null && relatedSet.Count > 0)
            {
                IObject relObj = null;
                //Reset the ISet
                relatedSet.Reset();
                //loop through each related object
                while ((relObj = relatedSet.Next() as IObject) != null)
                {
                    //get insatll date field value
                    var fieldInstallationDate = FieldInstance.FromModelName(relObj, modelName);

                     dateTime = DateTime.MinValue;
                    if (fieldInstallationDate != null)//if not null
                    {
                        dateTime = fieldInstallationDate.Value.Convert(DateTime.MinValue);
                        _logger.Debug(fieldInstallationDate.Alias + " value: " + dateTime);
                        //if job year greater than insatll date year
                        if (dateTime != DateTime.MinValue && installationJobYear > dateTime.Year)
                        {
                            AddError(string.Format(errorForComprDate, InstallJobYearAliasName, installationJobYear, relObj.Class.AliasName, relObj.OID, fieldInstallationDate.Alias, dateTime.ToShortDateString()));
                        }
                    }
                }
            } 
        }

        /// <summary>
        /// Check if install job year is greater than installation date
        /// </summary>
        /// <param name="relatedSet">ESRI ISet</param>
        /// <param name="installationDateYear">install Date field vlaue</param>
        /// <param name="modelName">model name</param>
        /// <param name="installDateAliasName">install Date field name</param>
        private void AddErrorForRelObjectComparisonYear(ISet relatedSet, DateTime installationDateYear, string modelName,string installDateAliasName)
        {
            
            if (relatedSet != null && relatedSet.Count > 0)
            { 
                IObject relObj = null;
                //Reset the ISet
                relatedSet.Reset();
                //loop through each related object
                while ((relObj = relatedSet.Next() as IObject) != null)
                {
                    //get field value for job year
                    var fieldInstallationYear = FieldInstance.FromModelName(relObj, modelName);

                    int relInstallationYear = DateTime.MinValue.Year;
                    if (fieldInstallationYear != null)//if not null
                   {
                       relInstallationYear = fieldInstallationYear.Value.Convert(DateTime.MinValue.Year);
                       _logger.Debug(fieldInstallationYear.Alias + " value: " + relInstallationYear);
                       //if job year greater than insatll date year
                       if (relInstallationYear != DateTime.MinValue.Year && relInstallationYear > installationDateYear.Year)
                       {
                           AddError(string.Format(errorForComprDate, fieldInstallationYear.Alias, relInstallationYear, relObj.Class.AliasName, relObj.OID, installDateAliasName, installationDateYear.ToShortDateString()));
                           //AddError(fieldInstallationYear.Alias + " of " + relInstallationYear + " is invalid.  It is after the " + installDateAliasName + " (ObjectID: " + relObj.OID + ") of " + installationDateYear.ToShortDateString() + ".");

                       }
                   }
                }
            } 
        }

        /// <summary>
        /// Check field value for install job year
        /// </summary>
        /// <param name="relObj">ESRI ISet</param>
        private void AddErrorForRelObjectJobYear(IObject relObj)
        {
            //get job year field value
            var fieldInstallationJobYear = FieldInstance.FromModelName(relObj, SchemaInfo.Electric.FieldModelNames.InstallationJobYear);
            int installationYear = DateTime.MinValue.Year;
            if (fieldInstallationJobYear != null)//if not null
            {
                installationYear = fieldInstallationJobYear.Value.Convert(DateTime.MinValue.Year);
                _logger.Debug(fieldInstallationJobYear.Alias + " value: " + installationYear);
                //if job year less than 1900
                if (installationYear != DateTime.MinValue.Year && installationYear < _minYear)
                    AddError(string.Format(errorForBeforeFloorDte,relObj.Class.AliasName,relObj.OID,fieldInstallationJobYear.Alias,installationYear,_minYear));
                //if job year greater than current year
                if (installationYear != DateTime.MinValue.Year && installationYear > DateTime.Now.Year)
                    AddError(string.Format(errorForAfterDate, relObj.Class.AliasName, relObj.OID, fieldInstallationJobYear.Alias, installationYear));
            }
        }

        /// <summary>
        /// Check field value for installation date
        /// </summary>
        /// <param name="relObj">ESRI ISet</param>
        private void AddErrorForRelObject(IObject relObj)
        {
            //get install date field value
            var relFieldInstallationDate = FieldInstance.FromModelName(relObj, SchemaInfo.Electric.FieldModelNames.InstallationDate);
            DateTime installationDate = DateTime.MinValue;
            if (relFieldInstallationDate != null)//if not null
            {
                installationDate = relFieldInstallationDate.Value.Convert(DateTime.MinValue);
                _logger.Debug(relFieldInstallationDate.Alias + " value: " + installationDate);
                //if install date less than 1900
                if (installationDate != DateTime.MinValue && installationDate.Year < _minYear)
                    AddError(string.Format(errorForBeforeFloorDte, relObj.Class.AliasName, relObj.OID, relFieldInstallationDate.Alias, installationDate.ToShortDateString(), _minYear));
                //if install date greater than current year
                if (installationDate.Year > DateTime.Today.Year)
                    AddError(string.Format(errorForAfterDate, relObj.Class.AliasName, relObj.OID, relFieldInstallationDate.Alias, installationDate.ToShortDateString()));

            }
        }

        /// <summary>
        /// Get ESRI ISet
        /// </summary>
        /// <param name="obj">ESRI Object</param>
        /// <returns>return esri ISet</returns>
        private ESRI.ArcGIS.esriSystem.ISet GetRelatedSet(IObject obj)
        {
            ISet relSet = null;
            try
            {
                //get esri relationshipClass
                _relClass = ModelNameFacade.RelationshipClassFromModelName(obj.Class, esriRelRole.esriRelRoleOrigin, SchemaInfo.Electric.ClassModelNames.InstallationChildField);
            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);
            }
            //if relationship class not null and destination object class have the model name
            if (_relClass != null && ModelNameFacade.ContainsClassModelName( _relClass.DestinationClass, SchemaInfo.Electric.ClassModelNames.InstallationChildField))
            {
                _logger.Debug("Destination Class name: " +_relClass.DestinationClass.AliasName);
                //get esri ISet
                relSet = _relClass.GetObjectsRelatedToObject(obj);                
            }
            return relSet;
        }

        #endregion
    }
}
