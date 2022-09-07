using System;
using System.Collections; 
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;

using Miner.Interop;
using Miner.ComCategories;

using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Framework;

using log4net;
using PGE.Common.Delivery.Diagnostics;

namespace PGE.Desktop.EDER.ValidationRules
{
    [Guid("6fabe8b7-4f06-4aaf-8c31-f56f3a9f3fd7")]
    [ProgId("PGE.Desktop.EDER.ValidateDuctDefinition")]
    [ComponentCategory(ComCategory.MMValidationRules)]
    public class ValidateDuctDefinition : BaseValidationRule 
    {

        private static readonly Log4NetLogger _logger = new
            Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        private static string[] _modelNames = new string[] { 
            SchemaInfo.Electric.ClassModelNames.DuctDefinition, 
            SchemaInfo.Electric.ClassModelNames.PGEConduitSystem};
        
        public ValidateDuctDefinition()
            : base("PGE Validate Duct Definition", _modelNames)
        { }
        
        /// <summary>
        /// Validates the PGEDuctDef to verify that the grouping of the duct definition 
        /// meets the business rules - where the material and size are the same they 
        /// should be grouped as a single record with an incremented ductcount field 
        /// value, rather than created multiple PGEDuctDef records with the same material 
        /// and size 
        /// Should be applied to PGEDuctDef - and to the ConduitSystem 
        /// </summary>
        /// <param name="row">Row to be validated</param>
        /// <returns>Returns the error list after validation</returns>
        protected override Miner.Interop.ID8List InternalIsValid(IRow row)
        {

            //Validate input row   
            IObject obj = row as IObject; 
            if (obj == null) return _ErrorList;
            IDataset pDS = (IDataset)row.Table;

            //Validate the format of the user entered structuresize 
            List<string> errorList = new List<string>();                          
            if (ModelNameFacade.ContainsClassModelName((IObjectClass)row.Table,
                SchemaInfo.Electric.ClassModelNames.PGEConduitSystem))
            {
                errorList = ValidateConduitSystem((IFeature)row); 
            }
            else if (ModelNameFacade.ContainsClassModelName((IObjectClass)row.Table,
                SchemaInfo.Electric.ClassModelNames.DuctDefinition))
            {
                errorList = ValidateDuctDef(row); 
            }
            else 
            {
                _logger.Debug("Running " + base._Name + " on incorrect object class: " +
                    pDS.Name); 
            }

            //Add each error encountered 
            for (int i = 0; i < errorList.Count; i++)
            {
                AddError(errorList[i]);
            }

            //Return the error list 
            return _ErrorList; 
        }

        /// <summary>
        /// Validates the PGEDuctDef record and ensure that there are not other 
        /// records related to the same Conduit System with the same MATERIAL and 
        /// SIZE 
        /// </summary>
        /// <param name="structureSize"></param>
        /// <returns></returns>
        private List<string> ValidateDuctDef(IRow pDuctDefRow)
        {
            string errorMsg = "";
            List<string> errorList = new List<string>();

            try
            {
                //Get the relationship class to the ConduitSystem
                IRelationshipClass pRelClass = GetRelationship((IObjectClass)pDuctDefRow.Table,
                    SchemaInfo.Electric.ClassModelNames.PGEConduitSystem);

                //Get the material and size for this PGEDuctDefinition
                string ductSize = ((IObject)pDuctDefRow).GetFieldValue(null, false, 
                    SchemaInfo.Electric.FieldModelNames.DuctSize).Convert<string>(string.Empty);
                string material = ((IObject)pDuctDefRow).GetFieldValue(null, false, 
                    SchemaInfo.Electric.FieldModelNames.DuctMaterial).Convert<string>(string.Empty);
                string curDuctSize = "";
                string curDuctMaterial = "";
                IRow pCurDuctDefRow = null; 
 
                //Get the related Conduit 
                IFeature pConduitSysFeature = null; 
                ISet pSet = pRelClass.GetObjectsRelatedToObject((IObject)pDuctDefRow);
                pSet.Reset();
                object pConduitSystemObj = pSet.Next(); 
                if (pConduitSystemObj != null)
                    pConduitSysFeature = (IFeature)pConduitSystemObj;

                //Loop through all of the related PGEDuctDefinition records for this Conduit
                if (pConduitSysFeature != null)
                {
                    ISet pCurSet = pRelClass.GetObjectsRelatedToObject((IObject)pConduitSysFeature);
                    object relObj = pCurSet.Next();
                    while (relObj != null)
                    {
                        pCurDuctDefRow = (IRow)relObj;

                        curDuctSize = ((IObject)pCurDuctDefRow).GetFieldValue(null, false,
                    SchemaInfo.Electric.FieldModelNames.DuctSize).Convert<string>(string.Empty);
                        curDuctMaterial = ((IObject)pCurDuctDefRow).GetFieldValue(null, false,
                    SchemaInfo.Electric.FieldModelNames.DuctMaterial).Convert<string>(string.Empty);

                        if ((curDuctSize == ductSize) &&
                            (curDuctMaterial == material) &&
                            (pCurDuctDefRow.OID != pDuctDefRow.OID))
                        {
                            errorMsg = "PGEDuctDefinition records related to a Conduit System " + 
                                "with the same MATERIAL and DUCTSIZE MUST be represented as a single record " + 
                                "by incrementing the DUCTCOUNT";
                            if (!errorList.Contains(errorMsg))
                                errorList.Add(errorMsg);
                        } 
                        relObj = pCurSet.Next(); 
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

        /// <summary>
        /// Validates the ConduitSystem record to ensure that there are not more 
        /// than one related PGEDuctDefinition record with the same DUCTSIZE and 
        /// MATERIAL 
        /// </summary>
        /// <param name="structureSize"></param>
        /// <returns></returns>
        private List<string> ValidateConduitSystem(IFeature pConduitSysFeature)
        {
            string errorMsg = "";
            List<string> errorList = new List<string>();

            try
            {
                //Get the relationship class to the ConduitSystem
                IRelationshipClass pRelClass = GetRelationship((IObjectClass)pConduitSysFeature.Class,
                    SchemaInfo.Electric.ClassModelNames.DuctDefinition);

                string curDuctSize = "";
                string curDuctMaterial = "";
                string uniqueKey = ""; 
                IRow pCurDuctDefRow = null;
                Hashtable hshUniqueCombinations = new Hashtable(); 

                //Get the related PGEDuctDefinition records 
                ISet pSet = pRelClass.GetObjectsRelatedToObject((IObject)pConduitSysFeature);
                pSet.Reset();
                object pDuctDefObj = pSet.Next();

                //Loop through all of the related PGEDuctDefinition records for this Conduit
                while (pDuctDefObj != null)
                {
                    pCurDuctDefRow = (IRow)pDuctDefObj;
                    curDuctSize = ((IObject)pCurDuctDefRow).GetFieldValue(null, false, 
                        SchemaInfo.Electric.FieldModelNames.DuctSize).Convert<string>(string.Empty);
                    curDuctMaterial = ((IObject)pCurDuctDefRow).GetFieldValue(null, false,
                        SchemaInfo.Electric.FieldModelNames.DuctMaterial).Convert<string>(string.Empty);

                    uniqueKey = curDuctSize + "*" + curDuctMaterial;
                    if (hshUniqueCombinations.ContainsKey(uniqueKey))
                    {
                        errorMsg = 
                            "Related PGEDuctDefinition records that have the same MATERIAL and " + 
                            "DUCTSIZE MUST be represented as a single record by incrementing " + 
                            "the DUCTCOUNT";
                        if (!errorList.Contains(errorMsg))
                            errorList.Add(errorMsg);
                    }
                    else
                    {
                        hshUniqueCombinations.Add(uniqueKey, 0);
                    }

                    pDuctDefObj = pSet.Next();
                }

                //Return the list of errors 
                return errorList;
            }
            catch (Exception ex)
            {
                throw new Exception("Error validating rule: " + base._Name);
            }
        }

        /// <summary>
        /// Find the ConduitSystem to PGEDuctDefition rel class 
        /// </summary>
        /// <param name="objClass"></param>
        /// <param name="modelName"></param>
        /// <returns></returns>
        protected IRelationshipClass GetRelationship(IObjectClass objClass, string modelName)
        {
            try
            {
                _logger.Debug("Entering GetRelationship");
                _logger.Debug(" modelName: " + modelName);

                IRelationshipClass pTargetRC = null;
                var enumRelClass = objClass.RelationshipClasses[esriRelRole.esriRelRoleAny];
                enumRelClass.Reset();
                IRelationshipClass relClass;
                while ((relClass = enumRelClass.Next()) != null)
                {
                    _logger.Debug("Got Relationship: " + ((IDataset)relClass).Name);
                    var destClass = relClass.DestinationClass;
                    var origClass = relClass.OriginClass;

                    if (ModelNameFacade.ContainsClassModelName(destClass, modelName)
                        || ModelNameFacade.ContainsClassModelName(origClass, modelName))
                    {
                        pTargetRC = relClass;
                        break; 
                    }
                }

                return pTargetRC;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetRelationship: " + ex.Message); 
            }
        }
    }
}
