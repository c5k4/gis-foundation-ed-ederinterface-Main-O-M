using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.Geodatabase;

using Miner.Interop;
using Miner.ComCategories;

using PGE.Common.Delivery.Diagnostics;
using PGE.Common.Delivery.Framework;
using PGE.Common.Delivery.Systems;

using log4net;
using ESRI.ArcGIS.esriSystem;

namespace PGE.Common.Delivery.ArcFM
{
    /// <summary>
    /// Base class for QA/QC validation rules.
    /// </summary>
    public abstract class BaseValidationRule : IMMValidationRule, IMMExtObject, IDisposable
    {
        #region Com Reg Methods

        /// <summary>
        /// Registers the specified reg key.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComRegisterFunction]
        private static void Register(string regKey)
        {
            MMValidationRules.Register(regKey);
        }
        /// <summary>
        /// Uns the register.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComUnregisterFunction]
        private static void UnRegister(string regKey)
        {
            MMValidationRules.Unregister(regKey);
        }

        #endregion
        
        #region Severity Variables
        private static Dictionary<string,Dictionary<string, int>> _severityMap = new Dictionary<string,Dictionary<string,int>>();
        private int _severity = 8;
        private const string _validationMapClassMN = "VALIDATION_SEVERITY";
        private const string _nameField = "NAME";
        private const string _severityField = "SEVERITY";
        private static Dictionary<string,Dictionary<int, string>> _domainMap = new Dictionary<string,Dictionary<int, string>>();
        private static Dictionary<int, string> _currentDomainMap = new Dictionary<int, string>();
        #endregion

        #region Private variables - Logging
        private static Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "");
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseValidationRule"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        protected BaseValidationRule(string name)
        {
            _Name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseValidationRule"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="modelNames">The model names that must be present on the feature class to be enabled. These can be field or class model names</param>
        protected BaseValidationRule(string name, params string[] modelNames)
        {
            _Name = name;
            _ModelNames = modelNames;
        }
        #endregion

        #region Protected Fields
        /// <summary>
        /// The name of the validation rule. This name will be displayed in ArcCatalog in the ArcFM Properties
        /// </summary>
        protected string _Name;
        /// <summary>
        /// Array of class or field model names used to enable the assignment of the validation rule within ArcCatalog.
        /// </summary>
        protected string[] _ModelNames;
        /// <summary>
        /// D8List of the validation errors. Use the AddError method to add errors to this list.
        /// </summary>
        protected ID8List _ErrorList;

        #endregion

        #region IMMValidationRule Members

        /// <summary>
        /// Determines whether the specified row is valid. 
        /// </summary>
        /// <param name="pRow">The row.</param>
        /// <returns>D8List of IMMValidationError items.</returns>
        public virtual ID8List IsValid(IRow pRow)
        {
            DateTime startTime = DateTime.Now;
            DateTime endTime;
             
            try
            {
                _logger.Debug(DateTime.Now.ToLongTimeString() + " ==== BEGIN EXECUTE: " + this._Name);

                SetSeverity(pRow);
                return InternalIsValid(pRow);
            }
            catch (Exception e)
            {
                //EventLogger.Error(e, "Error Executing Validation Rule " + _Name); // Commented as per Simon's instruction
                _logger.Error("Error Executing Validation Rule " + _Name, e);
                return null;
            }
            finally
            {
                endTime = DateTime.Now;
                TimeSpan ts = endTime - startTime;
                _logger.Debug("TOTAL EXECUTION TIME ==== " + ts.TotalSeconds.ToString());

            }
        }

        #endregion

        #region IMMExtObject Members

        /// <summary>
        /// Gets the bitmap.
        /// </summary>
        /// <value>The bitmap.</value>
        public virtual stdole.IPictureDisp Bitmap
        {
            get { return null; }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return _Name; }
        }

        /// <summary>
        /// Gets if this validation rule is enabled.
        /// </summary>
        /// <param name="pvarValues">The pvar values.</param>
        /// <returns><c>true</c> if the validation rule is enabled or visible within ArcCatalog; otherwise <c>false</c>.</returns>
        public virtual bool get_Enabled(ref object pvarValues)
        {
            try
            {
                return EnableByModelNames(pvarValues);
            }
            catch (Exception e)
            {
                //EventLogger.Error(e);
                _logger.Error(e.Message, e);
                return false;
            }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (_ErrorList != null)
                while (Marshal.ReleaseComObject(_ErrorList) > 0)
                {
                }
        }

        #endregion

        #region Protected Members

        /// <summary>
        /// Internal implementation of the IsValid method. This method is called within internal exception handling to report all errors to the event log and prompt the user.
        /// </summary>
        /// <param name="row">The row being validated.</param>
        /// <returns>D8List of IMMValidationError items.</returns>
        protected virtual ID8List InternalIsValid(IRow row)
        {
            return _ErrorList;
        }

        /// <summary>
        /// Determines if the specified parameter is an object class that has been configured with a class model name identified
        /// in the _ModelNames array.
        /// </summary>
        /// <param name="param">The object class to validate.</param>
        /// <returns>Boolean indicating if the specified object class has any of the appropriate model name(s).</returns>
        protected virtual bool EnableByModelNames(object param)
        {
            if (_ModelNames == null) return true;  // No configured model names.
            if (!(param is IObjectClass)) return true;
            IObjectClass oclass = (IObjectClass)param;

            if (ModelNameFacade.ContainsClassModelName(oclass, _ModelNames)) return true;
            if (ModelNameFacade.ContainsFieldModelName(oclass, _ModelNames)) return true;

            return false;
        }

        /// <summary>
        /// Protected property to return the severity for the QAQC error. This was added 
        /// 1/14/2014 to address INC000003803926 QA/QC freezing on the QAQC subtask  
        /// </summary>
        /// <param name="errorMessage">The error message to be added.</param>
        /// 
        protected int Severity
        {
            get { return _severity; }
        }

        /// <summary>
        /// Adds the error to the interal D8List. Use the _ErrorList property to retreive the list.
        /// </summary>
        /// <param name="errorMessage">The error message to be added.</param>
        protected void AddError(string errorMessage)
        {
            if (_ErrorList == null)
            {
                _ErrorList = new D8ListClass();
            }

            IMMValidationError error = new MMValidationErrorClass();
            //error.Severity = 8;
            error.Severity = _severity;
            error.BitmapID = 0;
            //Get the Domain Description and Append it to Error Message
            string domainDesc = string.Empty;
            if (_currentDomainMap.ContainsKey(_severity))
            {
                domainDesc = _currentDomainMap[_severity];
            }
            error.ErrorMessage = string.IsNullOrEmpty(domainDesc)?errorMessage:domainDesc+" - "+errorMessage;
            _ErrorList.Add((ID8ListItem)error); 
        }

        /// <summary>
        /// Gets the GUID value for the given row.
        /// </summary>
        /// <param name="row">The row </param>
        /// <returns>String</returns>
        protected string GetGUID(IRow row)
        {
            int pos = row.Table.FindField("GLOBALID");
            return (pos != -1) ? TypeCastFacade.Cast(row.get_Value(pos), "?") : "?";
        }
        #endregion 

        #region private methods for setting Severity
        /// <summary>
        /// Sets the Severity for the IMMValidationError based on a Table Configuration that holds the Name of the Validation Rule and te Severity to be assigned.
        /// Gets the Severity from a Table with modelname "VALIDATION_SEVERITY" and caches it. The Severity table is searched for each Workspace that comes thru'
        /// The workspace is not differentiated by version. It is differentited by the Instance and ServerName.
        /// If the table is not found in the database the Severity is defaulted to 8 which is what the product does for all the OOTB validation rules.
        /// </summary>
        /// <param name="row">The Row used to get the Workspace</param>
        private void SetSeverity(IRow row)
        {
            IWorkspace workSpace=((IDataset)row.Table).Workspace;
            string workspaceKey=GetWorkspaceKey(workSpace);
            if (string.IsNullOrEmpty(workspaceKey)) return;
            if (!_severityMap.ContainsKey(workspaceKey))
            {
                Dictionary<string, int> severities = GetSeverities(workSpace,workspaceKey);
                _severityMap.Add(workspaceKey, severities);
            }
            Dictionary<string, int> severityMap = _severityMap[workspaceKey];
            if (severityMap.ContainsKey(this.Name))
            {
                _severity = severityMap[this.Name];
            }
        }

        /// <summary>
        /// Get the Severities from the Workspace
        /// </summary>
        /// <param name="workSpace">IWorkspace</param>
        /// <param name="workspaceKey">A concatenated String of Workspace.Server property,"||" and Workspace.Instance Property</param>
        /// <returns>A dictionary of Validation Rule Name and the configured Validation Severity. If the Severity table is not found then an empty dictionary to make sure the workspace is not queried everytime.</returns>
        private Dictionary<string, int> GetSeverities(IWorkspace workSpace,string workspaceKey)
        {
            Dictionary<string, int> retVal = new Dictionary<string, int>();
            IObjectClass validationMapClass = ModelNameFacade.ObjectClassByModelName(workSpace, _validationMapClassMN);
            ICursor rowCursor = null;
            IRow row = null;
            try
            {
                if (validationMapClass != null)
                {
                    int severityFieldIdx = validationMapClass.Fields.FindField(_severityField);
                    int nameFieldIdx = validationMapClass.Fields.FindField(_nameField);
                    if (severityFieldIdx == -1 || nameFieldIdx == -1)
                    {
                        _logger.Warn("Fields:" + _severityField + " and/or " + _nameField + " is missing in the table:"+((IDataset)validationMapClass).Name);
                        return retVal;
                    }
                    rowCursor = ((ITable)validationMapClass).Search(null, false);
                    string name = string.Empty;
                    int severity = int.MinValue;
                    while ((row = rowCursor.NextRow()) != null)
                    {
                        name = TypeCastFacade.Cast<string>(row.get_Value(nameFieldIdx), string.Empty);
                        severity = TypeCastFacade.Cast<int>(row.get_Value(severityFieldIdx), 8);
                        if (string.IsNullOrEmpty(name))
                        {
                            continue;
                        }
                        retVal.Add(name, severity);
                    }
                    //Set the Domain Mapping to add the String to the Error Message
                    SetDomainMap(validationMapClass, workspaceKey, severityFieldIdx);
                }
                else
                {
                    _logger.Warn("Table with modelname:" + _validationMapClassMN + " is not found. All Validation rule errors will be considered as errors");  
                }
            }
            finally
            {
                if (rowCursor != null)
                {
                    while (Marshal.ReleaseComObject(rowCursor) > 0) { }
                }
                if (row != null)
                {
                    while (Marshal.ReleaseComObject(row) > 0) { }
                }
            }
            return retVal;
        }

        /// <summary>
        /// Gets a concatenated String of Workspace.Server property,"||" and Workspace.Instance Property for the given workspace
        /// This is the key used to identify the Workspace from which a specific Validation Severity Mapping has come from.
        /// </summary>
        /// <param name="workspace">IWorkspace object</param>
        /// <returns></returns>
        private string GetWorkspaceKey(IWorkspace workspace)
        {
            string retVal = string.Empty;
            //IPropertySet wkspcPropSet = workspace.ConnectionProperties;
            //object instance = wkspcPropSet.GetProperty("INSTANCE");
            //object server = wkspcPropSet.GetProperty("SERVER");
            Dictionary<string, object> propertySet = GetWorkspaceProperty(workspace);
            object instance = propertySet.ContainsKey("INSTANCE") ? propertySet["INSTANCE"]:null;
            object server = propertySet.ContainsKey("SERVER") ? propertySet["SERVER"] : string.Empty; 
            if ((instance == null||instance==DBNull.Value) && (server == null ||server==DBNull.Value))
            {
                //For File GDB or PGDB
                //object fileName = wkspcPropSet.GetProperty("DATABASE");
                object fileName = propertySet.ContainsKey("DATABASE") ? propertySet["DATABASE"] : null; 
                if (fileName != null && fileName != DBNull.Value)
                {
                    retVal = fileName.ToString();
                }
            }
            else
            {
                retVal = instance.ToString() + "||" + server.ToString();
            }
            return retVal;
        }

        /// <summary>
        /// Sets Dictionary of Severity code and Description from the given ObjectClass's Severity field.
        /// </summary>
        /// <param name="featClass">Handle to PGE.Common_Validation_Severity Map</param>
        /// <param name="workspaceKey">A concatenated String of Workspace.Server property,"||" and Workspace.Instance Property for the given workspace.</param>
        /// <param name="severityFieldIdx">Severity Field Index</param>
        private void SetDomainMap(IObjectClass featClass, string workspaceKey,int severityFieldIdx)
        {
            if (_domainMap.ContainsKey(workspaceKey))
            {
                _currentDomainMap = _domainMap[workspaceKey];
            }
            else
            {
                Dictionary<int, string> domainMap = new Dictionary<int, string>();
                IField severityField = featClass.Fields.get_Field(severityFieldIdx);
                if (severityField != null)
                {
                    IDomain domain = severityField.Domain;
                    if (domain is ICodedValueDomain)
                    {
                        ICodedValueDomain codedDomain = (ICodedValueDomain)domain;
                        for (int i = 0; i < codedDomain.CodeCount; i++)
                        {
                            domainMap.Add(Convert.ToInt16(codedDomain.get_Value(i)), codedDomain.get_Name(i));
                        }
                        _currentDomainMap = domainMap;
                    }
                }
                _domainMap.Add(workspaceKey, domainMap);
            }
        }

        /// <summary>
        /// Gets all the Workspace property as A Dictionary.
        /// </summary>
        /// <param name="workspace">IWorkspace</param>
        /// <returns>Dictionary of string and object</returns>
        private Dictionary<string, object> GetWorkspaceProperty(IWorkspace workspace)
        {
            Dictionary<string, object> retVal = new Dictionary<string, object>();
            IPropertySet2 propSet = (IPropertySet2)workspace.ConnectionProperties;
            object name = null;
            object value = null;
            object[] nameArray = null;
            object[] valueArray = null;
            propSet.GetAllProperties(out name, out value);
            nameArray = (object[])name;
            valueArray = (object[])value;
            for (int i = 0; i < propSet.Count; i++)
            {
                retVal.Add(nameArray[i].ToString(), valueArray[i]);
            }
            return retVal;
        }
        #endregion
    }
}
