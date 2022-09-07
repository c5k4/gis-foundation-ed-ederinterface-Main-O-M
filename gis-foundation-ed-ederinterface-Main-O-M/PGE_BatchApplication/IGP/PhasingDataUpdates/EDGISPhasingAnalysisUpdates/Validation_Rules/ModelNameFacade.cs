using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Carto;
using Miner.Interop;
using PGE.BatchApplication.IGPPhaseUpdate;

namespace Telvent.Delivery.Framework
{
    #region Model Name Framework
    /// <summary>
    /// Class to Manage ArcFM Model Name Framework
    /// </summary>
    public class ModelNameFacade
    {
        Common CommonFuntions = new Common();
        /// <summary>
        /// Static instance of ModelName Manager
        /// </summary>
        private  IMMModelNameManager _mnManager = Miner.Geodatabase.ModelNameManager.Instance;

        /// <summary>
        /// Instance of the ModelNameManager
        /// </summary>
        public  IMMModelNameManager ModelNameManager
        {
            get { return _mnManager; }
            set { _mnManager = value; }
        }

    
        /// <summary>
        /// Given a ObjectClass and a modelname gets the first field that has the modelname assigned.
        /// </summary>
        /// <param name="pOC">An object of type IObjectClass</param>
        /// <param name="sModelName">Modelname to check</param>
        /// <returns>Returns the first field that has the given modelname assigned</returns>
        public  IField FieldFromModelName(IObjectClass pOC, string sModelName)
        {
            try
            {
                return ModelNameManager.FieldFromModelName(pOC, sModelName);
            }
            catch (Exception ex)
            {
                CommonFuntions.WriteLine_Error(ex.Message + "   " + ex.StackTrace); 
                return null;
            }
        }

        /// <summary>
        /// Given a ObjectClass and a modelname gets the first field index that has the modelname assigned.
        /// </summary>
        /// <param name="objectClass">An object of type IObjectClass</param>
        /// <param name="modelName">Modelname to check</param>
        /// <returns>Returns the first field index that has the given modelname assigned</returns>
        public  int FieldIndexFromModelName(IObjectClass objectClass, string modelName)
        {
            int iRetVal = -1;
            try
            {
                IField field = FieldFromModelName(objectClass, modelName);
                if (field != null)
                    return objectClass.Fields.FindField(field.Name);
            }
            catch (Exception ex)
            {
                CommonFuntions.WriteLine_Error(ex.Message + "   " + ex.StackTrace); 
                return -1;
            }
            return iRetVal;
        }

      
        /// <summary>
        /// Given a modelname and objectclass gets all the field names that has the given modelname assigned.
        /// </summary>
        /// <param name="objectClass">An object of type IObjectClass</param>
        /// <param name="modelName">Modelname to check</param>
        /// <returns>Returns a list of field names that has the given modelname assigned. If the there are no fields with the modelname an empty list is returned.</returns>
        public  List<string> FieldNamesFromModelName(IObjectClass objectClass,string modelName)
        {
            List<string> retVal = new List<string>();
            try
            {
                IMMEnumField enumField = ModelNameManager.FieldsFromModelName(objectClass, modelName);
                enumField.Reset();
                IField fld = enumField.Next();
                while (fld != null)
                {
                    retVal.Add(fld.Name);
                    fld = enumField.Next();
                }
                return retVal;
            }
            catch (Exception ex)
            {
                CommonFuntions.WriteLine_Error(ex.Message + "   " + ex.StackTrace); 
                return null;
            }
        }

        /// <summary>
        /// Given a modelname and objectclass gets all the field names that has the given modelname assigned.
        /// </summary>
        /// <param name="objectClass">An object of type IObjectClass</param>
        /// <param name="modelName">Modelname to check</param>
        /// <returns>Returns a list of field names that has the given modelname assigned. If the there are no fields with the modelname an empty list is returned.</returns>
        public  List<int> FieldIndicesFromModelName(IObjectClass objectClass, string modelName)
        {
            List<int> retVal = new List<int>();
            try
            {
                IMMEnumField enumField = ModelNameManager.FieldsFromModelName(objectClass, modelName);
                enumField.Reset();
                IField fld = enumField.Next();
                while (fld != null)
                {
                    retVal.Add(objectClass.FindField(fld.Name));
                    fld = enumField.Next();
                }
                return retVal;
            }
            catch (Exception ex)
            {
                CommonFuntions.WriteLine_Error(ex.Message + "   " + ex.StackTrace); 
                return null;
            }
           
        }

      
        /// <summary>
        /// Determine object class contains any of the class model name
        /// </summary>
        /// <param name="classIn">IObjectClass to check for model names</param>
        /// <param name="modelNames">The model names.</param>
        /// <returns><c>true</c> if object class contains any of the class model name; otherwise, <c>false</c>.</returns>
        public  bool ContainsClassModelName(IObjectClass classIn, params string[] modelNames)
        {
            try
            {
                if (modelNames == null) return false;

                foreach (string name in modelNames)
                {
                    if (ModelNameManager.ContainsClassModelName(classIn, name))
                        return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                CommonFuntions.WriteLine_Error(ex.Message + "   " + ex.StackTrace); 
                return false;
            }
           
        }

     
        public  IDomain FindByName(IWorkspace workspace, string domainName)
        {
            IWorkspaceDomains wd = (IWorkspaceDomains)workspace;
            IEnumDomain enumDomains = wd.Domains;
            IDomain domain;

            try
            {
                while ((domain = enumDomains.Next()) != null)
                {
                    if (domain.Name.Equals(domainName, StringComparison.InvariantCultureIgnoreCase))
                        return domain;
                }
                return null;
            }
            catch (Exception ex)
            {
                CommonFuntions.WriteLine_Error(ex.Message + "   " + ex.StackTrace); 
                return null;
            }
           
        }


        /// <summary>
        /// Gets the Domain Code given the Domain Description or the Domain Description given Domain Code
        /// </summary>
        /// <param name="domainToCheck">Domain that should be used to decode the value</param>
        /// <param name="codeDescription">Domain Code or Description that should be used</param>
        /// <param name="isCode">Set this to true if the passed in codeDescription value is of Domain Code</param>
        /// <returns>Returns a string. The returned value is domain description if the isCode value passed in is true otherwise the returned value is the domain code.</returns>
        public  string GetDomainDescriptionOrCode(ICodedValueDomain domainToCheck, string codeDescription, bool isCode)
        {
            try
            {
                for (int i = 0; i < domainToCheck.CodeCount; i++)
                {
                    if (isCode)
                    {
                        if (domainToCheck.get_Value(i).ToString().ToUpper() == codeDescription.ToUpper())
                            return domainToCheck.get_Name(i);
                    }
                    else
                    {
                        if (domainToCheck.get_Name(i).ToString().ToUpper() == codeDescription.ToUpper())
                            return domainToCheck.get_Value(i).ToString();
                    }
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                CommonFuntions.WriteLine_Error(ex.Message + "   " + ex.StackTrace); 
                return null;
            }
        }


        public  void AddError(string sClassName, int iOid,string sGuid, string sError, string sCircuitID)
        {
            try
            {
                Common pCommonClass = new Common();
                pCommonClass.InsertRecordInDatatable_QAQC(sClassName, iOid,sGuid, sError, sCircuitID);
            }
            catch (Exception ex)
            {
                CommonFuntions.WriteLine_Error(ex.Message + "   " + ex.StackTrace); 
                return;
            }
        }

    }
    #endregion

}
