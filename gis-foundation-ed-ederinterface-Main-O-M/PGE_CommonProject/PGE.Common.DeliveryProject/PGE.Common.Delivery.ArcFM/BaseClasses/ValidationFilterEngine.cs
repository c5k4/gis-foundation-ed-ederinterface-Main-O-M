using System;
using System.Collections; 
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Framework;
using PGE.Common.Delivery.Diagnostics;
using PGE.Common.Delivery.Framework;
using PGE.Common.Delivery.Systems;
using Miner.Interop;

namespace PGE.Common.Delivery.ArcFM
{
    /// <summary>
    /// Singleton class for evaluating whether validation rules should fire. We require a means to 
    /// allow the application to run validation selectively. We need to  rules of severity Error to 
    /// be run in isolation. 
    /// </summary>
    public class ValidationFilterEngine
    {
        #region Private
        /// <summary>
        /// Logger to log all the information as Debug/Warning/Error.
        /// </summary>
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "");

        /// <summary>
        /// Hashtable to store the ValidationRules and their severity from the 
        /// PGE.Common_VALIDATION_SEVERITY_MAP table 
        /// </summary>
        //private static Hashtable _hshValidationSeverity;

        /// <summary>
        /// Hashtable to store the filtered list of validation errors 
        /// </summary>
        private Hashtable _hshFilteredErrors;

        /// <summary>
        /// Form for displaying the validation Msg  
        /// </summary>
        private DisplayValidationMessageForm _frmValidationMsg; 

        /// <summary>
        /// Private instance member.
        /// </summary>
        private static readonly ValidationFilterEngine _instance = new ValidationFilterEngine();

        /// <summary>
        /// Private instance member.
        /// </summary>
        private static  ValidationHelper.ValidationFilterType _valFilterType;


        /// <summary>
        /// Gets the workspace containing the PGE.Common_VALIDATION_SEVERITY_MAP table.
        /// </summary>
        /// <returns></returns>
        //private static IWorkspace getWorkspace()
        //{
        //    Type type = Type.GetTypeFromProgID("esriFramework.AppRef");
        //    object obj = Activator.CreateInstance(type);
        //    IApplication app = obj as IApplication;

        //    // get the workspace based on the ArcFM login object
        //    IMMLogin2 log2 = (IMMLogin2)app.FindExtensionByName("MMPROPERTIESEXT");
        //    return log2.LoginObject.LoginWorkspace;
        //}

        #endregion Private               

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationFilterEngine"/> class.
        /// </summary>
        private ValidationFilterEngine()
        {
            //By default QAQC must validate everything 
            _valFilterType = ValidationHelper.ValidationFilterType.valFilterTypeAll;
            _hshFilteredErrors = new Hashtable();
        }
        #endregion Constructor
                
        #region Public
        #region Property
        /// <summary>
        /// Gets the instance.
        /// </summary>
        public static ValidationFilterEngine Instance
        {
            get
            {
                _logger.Debug("Returning the singleton.");
                return ValidationFilterEngine._instance;
            }
        }

        /// <summary>
        /// Sets what kind of validation filter type is to be applied to QAQC 
        /// </summary>
        public ValidationHelper.ValidationFilterType FilterType
        {
            get { return _valFilterType; }
            set 
            { 
                _valFilterType = value;
                ClearErrors();  
            }
        }

        #endregion Property

        /// <summary>
        /// Returns the error count 
        /// </summary>
        /// <param name="name">The name.</param>
        public long GetErrorCount()
        {
            try
            {
                return _hshFilteredErrors.Count; 
            }
            catch (Exception ex)
            {
                _logger.Debug("entering error handler for GetErrorCount: " + ex.Message);
                //assume the error should be run (to be safe) 
                throw new Exception("Error returning the QAQC error count"); 
            }
        }

        /// <summary>
        /// Allows the user to set the list of errors 
        /// </summary>
        /// <param name="name">The name.</param>
        //public ID8List QAQCErrors
        //{
        //    get { return _qaqcErrors; }
        //    set { _qaqcErrors = value; }
        //}

        /// <summary>
        /// Returns the error count 
        /// </summary>
        /// <param name="name">The name.</param>
        public void DisplayValidationMsg()
        {
            try
            {
                //Get a hashtable of errors from the ID8List 
                PGEError[] errorArray = new PGEError[_hshFilteredErrors.Count];
                int counter = 0; 
                foreach (PGEError pError in _hshFilteredErrors.Values)
                {
                    errorArray[counter] = pError;
                    counter++; 
                }

                ////Close if already open 
                //if (_frmValidationMsg != null)
                //{
                //    if (_frmValidationMsg.Visible) 
                //        _frmValidationMsg.Close();
                //    _frmValidationMsg.Dispose();
                //    _frmValidationMsg = null; 
                //}

                ////Show the form modelessly 
                //_frmValidationMsg = new DisplayValidationMessageForm(errorArray); 
                //_frmValidationMsg.Show();
                //if (_frmValidationMsg.Visible)
                //    System.Diagnostics.Debug.Print("DisplayValidationMessageForm is now visible");

                //Show the form modally 
                _frmValidationMsg = new DisplayValidationMessageForm(errorArray);
                _frmValidationMsg.ShowDialog();

            }
            catch (Exception ex)
            {
                _logger.Debug("entering error handler for DisplayValidationMsg: " + ex.Message);
                //assume the error should be run (to be safe) 
                throw new Exception("Error returning the QAQC error count");
            }
        }

        /// <summary>
        /// Pass the function the results from the QAQC and the 
        /// the function will populate a hashtable containing the 
        /// errors only (excluding warnings) 
        /// </summary>
        /// <param name="name">The name.</param>
        public void PopulateErrors(ID8List validationResults)
        {
            try
            {
                //Print out the deletes 
                string objectIdentifier = "";
                bool reachedValLevel = false;
                validationResults.Reset();
                ID8ListItem pListItem = validationResults.Next();

                //If the parent of the item is a attribute rel item 
                //or a iD8mmFeature then we have come down far enough 
                ID8ListItem pParListItem = null;
                if (pListItem != null)
                {
                    if (pListItem.ContainedBy != null)
                    {
                        pParListItem = (ID8ListItem)pListItem.ContainedBy;
                        if ((pParListItem.ItemType == mmd8ItemType.mmd8itFeature) ||
                            (pParListItem.ItemType == mmd8ItemType.mmitAttrRelationshipInstance))
                            reachedValLevel = true;
                        else
                            reachedValLevel = false;
                    }
                }

                while (pListItem != null)
                {
                    System.Diagnostics.Debug.Print("DisplayName:" + pListItem.DisplayName);

                    if (pListItem.ContainedBy != null)
                    {
                        pParListItem = (ID8ListItem)pListItem.ContainedBy;
                        if ((pParListItem.ItemType == mmd8ItemType.mmd8itFeature) ||
                            (pParListItem.ItemType == mmd8ItemType.mmitAttrRelationshipInstance))
                            reachedValLevel = true;
                        else
                            reachedValLevel = false;
                    }

                    if (pListItem is IMMValidationError)
                    {
                        ID8GeoAssoc geo = (ID8GeoAssoc)pParListItem;
                        if (geo.AssociatedGeoRow is ESRI.ArcGIS.Geodatabase.IObject)
                        {
                            //Assume errors start with "Error " 
                            if (pListItem.DisplayName.StartsWith("Error "))
                            {
                                ESRI.ArcGIS.Geodatabase.IObject obj = (ESRI.ArcGIS.Geodatabase.IObject)geo.AssociatedGeoRow;
                                ESRI.ArcGIS.Geodatabase.IDataset pDS = (ESRI.ArcGIS.Geodatabase.IDataset)obj.Class;
                                objectIdentifier = pDS.Name + ":" + obj.OID.ToString();
                                PGEError pError = new PGEError(
                                    pDS.Name,
                                    obj.OID,
                                    ValidationFilterEngine.Instance.GetFCImageIndex((IObjectClass)pDS),
                                    pListItem.DisplayName);
                                _hshFilteredErrors.Add(_hshFilteredErrors.Count, pError);
                            }
                        }
                    }

                    if (pListItem is ID8List)
                    {
                        if (!reachedValLevel)
                        {
                            //System.Diagnostics.Debug.Print(pListItem.DisplayName); 
                            ID8List pChildD8List = (ID8List)pListItem;
                            //Recursive call here 
                            PopulateErrors(pChildD8List);
                        }
                    }
                    pListItem = validationResults.Next();
                }
            }
            catch
            {
                throw new Exception("Error populating the session errors");
            }
        }

        /// <summary>
        /// Clear the validation errors 
        /// </summary>
        /// <param name="name">The name.</param>
        public void ClearErrors()
        {
            try
            {
                _hshFilteredErrors.Clear();                 
            }
            catch
            {
                throw new Exception("Error clearing the session errors");
            }
        }


        /// <summary>
        /// Pass the function what kind of filter is being applied to the validation and 
        /// the name of the validation rule to be run and the function will return a boolean
        /// to indicate whether the rule should be enabled 
        /// </summary>
        /// <param name="name">The name.</param>
        public bool IsQAQCRuleEnabled(string ruleName, int severity)
        {
            try
            {
                bool IsEnabled = false; 
                _logger.Debug("Entering IsQAQCRuleEnabled");

                //_logger.Debug("Rule count for hshValidationSeverity is: " + _hshValidationSeverity.Count.ToString());                
                //if (_hshValidationSeverity.Count == 0)
                //    _hshValidationSeverity = LoadRuleSeverities();
                //_logger.Debug("After loading rules hshValidationSeverity count is: " + _hshValidationSeverity.Count.ToString());
                //int severity = -1; 

                //Domain used with SEVERITY field 
                //0 - error 
                //1 - warning 
                //7 - product not in use - esri rules 
                //8 - product not in use - PGE.Common rules 

                //Determine if the passed rule is a warning or an error 
                //if (_hshValidationSeverity.ContainsKey(ruleName.ToLower()))
                //    severity = (int)_hshValidationSeverity[ruleName.ToLower()]; 
                //else 
                //    _logger.Debug("Unable to determine severity for rule: " + ruleName);
                
                //Determine if the rule should be enabled 
                switch (_valFilterType) 
                {
                    case ValidationHelper.ValidationFilterType.valFilterTypeAll:
                        //enable provided the rule is in use 
                        if ((severity != 7) && (severity != 8))
                            IsEnabled = true; 
                        break; 
                    case ValidationHelper.ValidationFilterType.valFilterTypeErrorOnly:
                        //enable provided the rule is error 
                        if (severity == 0)
                            IsEnabled = true;
                        break; 
                    case ValidationHelper.ValidationFilterType.valFilterTypeWarningOnly:
                        if (severity == 1)
                            IsEnabled = true;
                        break; 
                    default:  
                        _logger.Debug("Error unknown validation filter"); 
                        throw new Exception("Invalid validation filter"); 
                }

                if (IsEnabled)
                    System.Diagnostics.Debug.Print("this is one"); 
                _logger.Debug("IsQAQCRuleEnabled Returning: " + IsEnabled.ToString() + " for: " + ruleName);
                return IsEnabled; 
            }
            catch (Exception ex)
            {
                _logger.Debug("entering error handler for IsQAQCRuleEnabled: " + ex.Message);
                //assume the error should be run (to be safe) 
                return true; 
            }
        }

        /// <summary>
        /// Where QAQC is run on a session and where there are a huge amount 
        /// of errors returned - looping through the ID8List is taking a large 
        /// amount of time. This routine will add just the filtered custom  QA 
        /// rules to a list so that finding the QAQC failures of the filtered 
        /// severity will be extremely fast 
        /// </summary>
        //public void AddError(string errorMsg, int severity, IObject pObject)
        //{
        //    try
        //    {
                
        //        switch (_valFilterType) 
        //        {
        //            case ValidationHelper.ValidationFilterType.valFilterTypeAll:
        //                //Do nothing, no need to capture because no filter 
        //                //is being applied 
        //                break; 
        //            case ValidationHelper.ValidationFilterType.valFilterTypeErrorOnly:
        //                if (severity == 0)
        //                {
        //                    IDataset pDS = (IDataset)pObject.Class; 
        //                    string datasetName = pDS.BrowseName;
        //                    PGEError pError = new PGEError(GetShortDatasetName(datasetName), pObject.OID,
        //                        GetFCImageIndex(pObject.Class), errorMsg);
        //                    _hshFilteredErrors.Add(_hshFilteredErrors.Count, pError); 
        //                }
        //                break; 
        //            case ValidationHelper.ValidationFilterType.valFilterTypeWarningOnly:
        //                if (severity == 1)
        //                {
        //                    IDataset pDS = (IDataset)pObject.Class;
        //                    string datasetName = pDS.BrowseName;
        //                    PGEError pError = new PGEError(GetShortDatasetName(datasetName), pObject.OID,
        //                        GetFCImageIndex(pObject.Class), errorMsg);
        //                    _hshFilteredErrors.Add(_hshFilteredErrors.Count, pError);
        //                }
        //                break; 
        //            default:  
        //                _logger.Debug("Error unknown validation filter"); 
        //                throw new Exception("Invalid validation filter"); 
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Debug("entering error handler for AddError: " + ex.Message);
        //        throw new Exception("Error adding error in ValidationFilterEngine"); 
        //    }
        //}

        private string GetShortDatasetName(string datasetName)
        {
            try
            {
                string shortDatasetName = "";
                int posOfLastPeriod = -1;

                posOfLastPeriod = datasetName.LastIndexOf(".");
                if (posOfLastPeriod != -1)
                {
                    shortDatasetName = datasetName.Substring( 
                        posOfLastPeriod + 1, (datasetName.Length - posOfLastPeriod) - 1);  
                }
                else
                {
                    shortDatasetName = datasetName; 
                }

                return shortDatasetName;
            }
            catch (Exception ex)
            {
                throw new Exception("Error returning the shortened dataset name" + ex.Message.ToString());
            }
        }

        public int GetFCImageIndex(IObjectClass pOC)
        {
            int defaultImageIdx = 0; 

            try
            {
                int imageIdx = defaultImageIdx; 
                IDataset pDS = (IDataset)pOC;
                esriDatasetType pType = pDS.Type;
                if (pType != esriDatasetType.esriDTFeatureClass)
                {
                    //Assume a table 
                    imageIdx = 3;                                                     
                }
                else 
                {                               
                    IFeatureClass pFC = (IFeatureClass)pDS;
                    if (pFC.FeatureType == esriFeatureType.esriFTAnnotation)
                    {
                        imageIdx = 4; 
                    }
                    else
                    {
                        if (pFC.ShapeType == ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPoint)
                        {
                            imageIdx = 0; 
                        }
                        else if (pFC.ShapeType == ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolyline)
                        {
                            imageIdx = 1; 
                        }
                        else if (pFC.ShapeType == ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolygon)
                        {
                            imageIdx = 2; 
                        }
                    }
                }
                return imageIdx; 
            }
            catch
            {
                return defaultImageIdx; 
            }
        }

        #endregion Public
    }

    public static class ValidationHelper
    {
        public enum ValidationFilterType
        {
            valFilterTypeAll = 0,
            valFilterTypeErrorOnly = 1,
            valFilterTypeWarningOnly = 2
        }
    }

    public class PGEError : IComparable
    {
        //Property storage variables 
        private string m_DatasetAlias;
        private int m_ObjectId;
        private int m_Type; 
        private string m_errorMsg; 

        public PGEError(
            string datasetAlias,
            int objectId, 
            int type, 
            string errorMsg)
        {
            m_DatasetAlias = datasetAlias;
            m_ObjectId = objectId;
            m_Type = type; 
            m_errorMsg = errorMsg;           
        }

        //IComparable implementation (for sort order) 
        //Sort Dataset and then OId  
        int stringCompare = -1;
        int IComparable.CompareTo(object obj)
        {
            PGEError temp =
                (PGEError)obj;
            if (this.Dataset != temp.Dataset)
                stringCompare = string.Compare(
                this.Dataset, temp.Dataset);
            else
            {
                if (this.ObjectId > temp.ObjectId)
                    stringCompare = 1;
                else
                    stringCompare = 0;
            }
            return stringCompare;
        }

        public string Dataset
        {
            get { return m_DatasetAlias; }
        }
        public int ObjectId
        {
            get { return m_ObjectId; }
        }
        public string ErrorMsg
        {
            get { return m_errorMsg; }
        }
        public int Type
        {
            get { return m_Type; }
        }

    }
}
