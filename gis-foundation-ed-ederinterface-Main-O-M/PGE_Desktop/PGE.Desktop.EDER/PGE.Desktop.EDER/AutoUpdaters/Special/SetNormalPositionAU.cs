// ========================================================================
// Copyright © 2006 Telvent, Consulting Engineers, Inc.
// <history>
// Editing-General - EDER Component Specification
// Shaikh Rizuan 10/02/2012	Created
// </history>
// All rights reserved.
// ========================================================================
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geodatabase;
using log4net;
using Miner.ComCategories;
using Miner.Interop;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Framework;
using System.Collections.Generic;
using PGE.Common.Delivery.Diagnostics;
using PGE.Desktop.EDER;

namespace PGE.Desktop.EDER.AutoUpdaters.Special
{

    /// <summary>
    /// Update Normal Position A B C for Dynamic Protective Device,Electric Stitch Point,Fuse,Open point,Switch.
    /// </summary>
    [ComVisible(true)]
    [Guid("E850A6AC-D6D4-44FC-B07D-25778150D66B")]
    [ProgId("PGE.Desktop.EDER.SetNormalPositionAU")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    public class SetNormalPositionAU:BaseSpecialAU
    {
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
         /// <summary>
        /// Constructor, pass in name.
        /// </summary>
        /// 

        public SetNormalPositionAU() : base("PGE Set Normal Position AU")
        {
        }

        #region Base Special AU Overrides
        /// <summary>
        /// Determines in which class the AU will be enabled
        /// </summary>
        /// <param name="objectClass"> Object's class. </param>
        /// <param name="eEvent">The edit event.</param>
        /// <returns> <c>true</c> if the AuoUpdater should be enabled; otherwise <c>false</c> </returns>
        protected override bool InternalEnabled(IObjectClass objectClass, mmEditEvent eEvent)
        {
            
            string[] pModelNames = new string[] {SchemaInfo.Electric.ClassModelNames.Protective,SchemaInfo.Electric.ClassModelNames.Switch };
            bool enabled = false;
            
            if (eEvent == mmEditEvent.mmEventFeatureCreate || eEvent == mmEditEvent.mmEventFeatureUpdate)
            {
                enabled = ModelNameFacade.ContainsClassModelName(objectClass, pModelNames);
                _logger.Debug("Class model name: " + SchemaInfo.Electric.ClassModelNames.Protective + "," + SchemaInfo.Electric.ClassModelNames.Switch + "Found: " + enabled); 
            }

            return enabled;
        }

         /// <summary>
        /// Determines whether actually this AU should be run, based on the AU Mode.
        /// </summary>
        /// <param name="eAUMode"> The auto updater mode. </param>
        /// <returns> <c>true</c> if the AuoUpdater should be executed; otherwise <c>false</c> </returns>
        protected override bool CanExecute(mmAutoUpdaterMode eAUMode)
        {
            if (eAUMode == mmAutoUpdaterMode.mmAUMArcMap)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Implementation of AutoUpdater Execute Ex method for derived classes.
        /// </summary>
        /// <param name="obj">The object that triggered the AutoUpdater.</param>
        /// <param name="eAUMode">The auto updater mode.</param>
        /// <param name="eEvent">The edit event.</param>
        protected override void InternalExecute(IObject obj, mmAutoUpdaterMode eAUMode, mmEditEvent eEvent)
        {
            // variable Declarations
           // var resolver = new ModelNameResolver();
            bool fldModelNames = false;
            bool fireAU = false;
            string[] strArryFldModelNames = new string[] { SchemaInfo.Electric.FieldModelNames.NormalPosition, SchemaInfo.Electric.FieldModelNames.NormalpositionA, SchemaInfo.Electric.FieldModelNames.NormalpositionB, SchemaInfo.Electric.FieldModelNames.NormalPositionC };
            IObjectClass objClass = obj.Class;
            int normalPositionIndx;
            IField normalPositionAField=null;
            IField normalPositionBField=null;
            IField normalPositionCField=null;
            object normalPositionPhaseValue;// = string.Empty;
            string normalPositionBValue = string.Empty;
            string normalPositionCValue = string.Empty;
            object normalPositionValue;// = string.Empty;
            string erroMsg=string.Empty;
            object assignValue=null;
            //int[] numberOfPhases = null;
            List<IField> numberOfPhases = null;
            // Check for field model names
            fldModelNames = ModelNameFacade.ContainsFieldModelName(objClass, strArryFldModelNames);
            if (fldModelNames)
            {
                
               
                // Get the Field Index by passsing object class and model name
                normalPositionIndx = ModelNameFacade.FieldIndexFromModelName(objClass, SchemaInfo.Electric.FieldModelNames.NormalPosition); //obj.Fields.FindField(detectionNormalPositionFld.Name);
                normalPositionAField = ModelNameFacade.FieldFromModelName(objClass, SchemaInfo.Electric.FieldModelNames.NormalpositionA);//obj.Fields.FindField(detectionNormalPositionAFld.Name);
                normalPositionBField = ModelNameFacade.FieldFromModelName(objClass, SchemaInfo.Electric.FieldModelNames.NormalpositionB);//obj.Fields.FindField(detectionNormalPositionBFld.Name);
                normalPositionCField = ModelNameFacade.FieldFromModelName(objClass, SchemaInfo.Electric.FieldModelNames.NormalPositionC);//obj.Fields.FindField(detectionNormalPositionCFld.Name);
                if (normalPositionIndx != -1 && normalPositionAField != null && normalPositionBField != null && normalPositionCField != null)
                {
                    numberOfPhases =new List<IField> { normalPositionAField, normalPositionBField, normalPositionCField };
                    normalPositionValue = obj.GetFieldValue(null, true, SchemaInfo.Electric.FieldModelNames.NormalPosition).Convert<object>(System.DBNull.Value); //obj.GetDomainDescription(normalPositionIndx, out erroMsg);
                    _logger.Debug(SchemaInfo.Electric.FieldModelNames.NormalPosition + ": " + normalPositionValue.ToString());
                    if (normalPositionValue != System.DBNull.Value && !string.IsNullOrEmpty(normalPositionValue.ToString()))
                    {
                        // Check For type of event
                        if (eEvent == mmEditEvent.mmEventFeatureUpdate)
                        {
                            IRowChanges rowChange = obj as IRowChanges;
                            // check for row change
                            if (rowChange.get_ValueChanged(normalPositionIndx))
                            {
                                fireAU = true;
                            }
                        }
                        else if (eEvent == mmEditEvent.mmEventFeatureCreate)
                        {
                            //check for normal position field value 
                            if (normalPositionValue != System.DBNull.Value)
                            {
                                fireAU = true;
                            }
                        }
                    }
                    if (fireAU) /// If true
                    {
                        //check for type of normal position value
                        if (normalPositionValue.ToString().ToLower() == SchemaInfo.Electric.Subtypes.NormalPosition.StatusOpen)
                        {
                            for (int i = 0; i < numberOfPhases.Count; i++)
                            {
                                //normalPositionPhaseValue = string.Empty;
                                normalPositionPhaseValue = System.DBNull.Value;
                                //get field value
                                normalPositionPhaseValue = obj.GetFieldValue(numberOfPhases[i].Name, true, null).Convert<object>(System.DBNull.Value);
                                if (normalPositionPhaseValue==System.DBNull.Value)//if null
                                {
                                    // Return Error erroMsg
                                    _logger.Warn(numberOfPhases[i].Name + ": " + normalPositionPhaseValue.ToString());
                                }
                                else if (normalPositionPhaseValue.ToString().ToLower() == SchemaInfo.Electric.Subtypes.NormalPosition.StatusClosed)
                                {
                                    assignValue = GetDomainValueFromObject(obj, (obj.Fields.FindField(numberOfPhases[i].Name)), SchemaInfo.Electric.Subtypes.NormalPosition.StatusOpen, out erroMsg);
                                    obj.set_Value((obj.Fields.FindField(numberOfPhases[i].Name)), assignValue);
                                }


                            }

                        }
                        else if (normalPositionValue.ToString().ToLower() == SchemaInfo.Electric.Subtypes.NormalPosition.StatusClosed)
                        {
                            for (int i = 0; i < numberOfPhases.Count; i++)
                            {
                                normalPositionPhaseValue = System.DBNull.Value;
                                //get field value
                                normalPositionPhaseValue = obj.GetFieldValue(numberOfPhases[i].Name, true, null).Convert<object>(System.DBNull.Value);
                                if (normalPositionPhaseValue == System.DBNull.Value)
                                {
                                    // Return Error erroMsg
                                    _logger.Warn(numberOfPhases[i].Name + ": " + normalPositionPhaseValue.ToString());
                                }
                                else if (normalPositionPhaseValue.ToString().ToLower() == SchemaInfo.Electric.Subtypes.NormalPosition.StatusOpen)//if open then closed
                                {
                                    //get field value
                                    assignValue = GetDomainValueFromObject(obj, (obj.Fields.FindField(numberOfPhases[i].Name)), SchemaInfo.Electric.Subtypes.NormalPosition.StatusClosed, out erroMsg);
                                    obj.set_Value((obj.Fields.FindField(numberOfPhases[i].Name)), assignValue);
                                }


                            }
                        }
                    }
                }
                else
                {
                    _logger.Error("Field is not present in feature class: Normal Position,NormalPositionA,NoramlPositionB,NormalPositionC");
                }
               

            }
            else
            {
                _logger.Warn("Verify that field model names are assigned to the following fields: Normal Position,NormalPositionA,NoramlPositionB,NormalPositionC.");
               // throw new COMException("Verify that field model names are assigned to the following fields: Normal Position,NormalPositionA,NoramlPositionB,NormalPositionC.", (int)mmErrorCodes.MM_E_CANCELEDIT);
            }
        }
        #endregion

        #region Custom Methods
        /// <summary>
        /// Get the Code value for coded value domain
        /// </summary>
        /// <param name="pObj"> pass object</param>
        /// <param name="pFieldIndex">pass the field Index for the field</param>
        /// <param name="description"> pass the description for the subtype</param>
        /// <param name="pErrorMessage">out error message if domain is null</param>
        /// <returns>Returns Coded value for the subtype description</returns>
        private object GetDomainValueFromObject(IObject pObj, int pFieldIndex, string description, out string pErrorMessage)
        {
            //Variable Declarations
            object obj=null;
            pErrorMessage = string.Empty;
            ICodedValueDomain pCodedValueDomain = null;


            IField pField = pObj.Fields.get_Field(pFieldIndex);

            // if domain is null return message
            if (pField.Domain == null)
            {
                pErrorMessage = "Domain is not assign for the field '" + pField.Name + "'.";
                return string.Empty;
            }
            IDomain pDomain = pField.Domain;

            //Check for domain type
            if (pDomain.Type == esriDomainType.esriDTCodedValue)
            {
                pCodedValueDomain = pDomain as ICodedValueDomain;

                //Get coded value name and value and check with given value
                for (int j = 0; j < pCodedValueDomain.CodeCount; j++)
                {
                    string codedValueName = pCodedValueDomain.get_Name(j);
                    object codedValueValue = pCodedValueDomain.get_Value(j);

                    if (codedValueName.ToString().ToLower().Equals(description))
                    {
                        obj = codedValueValue;
                        break;
                    }
                }
            }
            else
            {
                pErrorMessage = "Domain is not codedvalue.";
                return string.Empty;
            }
            return obj;
        }
        #endregion
    }
}
