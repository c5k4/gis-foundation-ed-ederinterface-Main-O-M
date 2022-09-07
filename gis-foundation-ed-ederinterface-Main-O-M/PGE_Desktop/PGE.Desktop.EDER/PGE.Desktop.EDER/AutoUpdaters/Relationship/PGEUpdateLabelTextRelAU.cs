using System;
using System.Reflection;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geodatabase;
using Miner.ComCategories;
using Miner.Geodatabase;
using Miner.Interop;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Framework;
using System.Windows.Forms;
using ESRI.ArcGIS.esriSystem;


namespace PGE.Desktop.EDER.AutoUpdaters.Relationship
{
    /// <summary>
    /// Changes for ENOS to SAP migration, A relationship AU that Updates LabelText feild in Service location of related service point.
    /// </summary>
    [ComVisible(true)]
    [Guid("0C8A85C5-1D46-4E23-B88C-572EDAA5A6C3")]
    [ProgId("PGE.Desktop.EDER.PGEUpdateLabelTextInSL")]
    [ComponentCategory(ComCategory.RelationshipAutoupdateStrategy)]
    public class PGEUpdateLabelTextRelAU : BaseRelationshipAU
    {
        #region private
        private static readonly PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        private static string _projectNameField = "PROJECTNAME";
        private static string _effRtInvField = "EFFRATINGINVKW";
        private static string _effRtMachField = "EFFRATINGMACHKW";
        private static string _genCategoryFldname = "GENCATEGORY";
        private static string _genSymbologyFldname = "GENSYMBOLOGY";
        private static string _genSymbologyvaluePrimary ="Primary";
        private static string _genSymbologyvalueSecondary ="Secondary";
        #endregion private

        #region Constructors
        /// <summary>
        /// constructor for the Populate Conductor Phase Rel AU
        /// </summary>
        public PGEUpdateLabelTextRelAU()
            : base("PGE Update Label Text On Relation Update")
        {
        }

        #endregion Constructors
        #region Relationship AU overrides
        /// <summary>
        /// update the sum of count of objects
        /// </summary>
        /// <param name="relationship">the relationship</param>
        /// <param name="auMode">the mode</param>
        /// <param name="eEvent">the event</param>
        protected override void InternalExecute(IRelationship relationship, mmAutoUpdaterMode auMode, mmEditEvent eEvent)
        {

            //Get parent service point
            IObject origObject = relationship.OriginObject;
            IObject destObject = relationship.DestinationObject;

            //check for not null
            if (origObject == null)
            {
                //log the warning and return the control 
                _logger.Warn("OriginObject is <Null>, exiting.");
                return;
            }

           
            IRow relServiceLoc = null;
            if (GetRelatedServiceLocation(origObject, out relServiceLoc))
            {
                if (eEvent == mmEditEvent.mmEventRelationshipCreated )//|| eEvent == mmEditEvent.mmEventRelationshipDeleted)
                {
                    if (IsPrimaryGeneration(origObject))
                    {
                        //Code change for PrepareLabelText Update
                        string labelText = PrepareLabelText(origObject);
                        //string labelText = PrepareLabelText(destObject);
                        if (string.IsNullOrEmpty(labelText))
                        {
                            labelText = string.Empty;
                        }
                        UpdateLabeltextInSl(relServiceLoc, labelText);
                        _logger.Info("Updated Service Location Label Text as :" + labelText + ", For OID: " + relServiceLoc.OID.ToString());
                    }
                }
                //else if (eEvent == mmEditEvent.mmEventRelationshipDeleted)
                //{
                //    //This case will be handeled
                //}
               
                UpdateGenCategory(origObject);
            }
            else
            {
                _logger.Info("Unable to update Label text in Service location, because more than 2 service locations are related to the service point Oid :" + origObject.OID.ToString());
            }

        }

        private void UpdateGenCategory(IObject pObject)
        {
            int transformerGuidFldId = pObject.Class.Fields.FindField("TRANSFORMERGUID");
            int primaryMeterGuidFldId = pObject.Class.Fields.FindField("PRIMARYMETERGUID");
            int serviceLocGuidFldId = pObject.Class.Fields.FindField("SERVICELOCATIONGUID");
            string transformerGUID = Convert.ToString(pObject.get_Value(transformerGuidFldId));
            string primarymeterGUID = Convert.ToString(pObject.get_Value(primaryMeterGuidFldId));
            string servicelocationGUID = Convert.ToString(pObject.get_Value(serviceLocGuidFldId));
            if(!string.IsNullOrEmpty(transformerGUID) && !string.IsNullOrEmpty(servicelocationGUID))
            {
                UpdateGenCategory(pObject, 2);
            }
            else if (!string.IsNullOrEmpty(primarymeterGUID) && !string.IsNullOrEmpty(servicelocationGUID))
            {
                UpdateGenCategory(pObject, 1);
            }
            else if (string.IsNullOrEmpty(transformerGUID) && string.IsNullOrEmpty(primarymeterGUID) && string.IsNullOrEmpty(servicelocationGUID))
            {
                UpdateGenCategory(pObject, 0);
            }
        }

        /// <summary>
        /// Updates GENCATEGORY field in service location feature class
        /// </summary>
        /// <param name="servicePoint"></param>
        /// <param name="value"></param>
        private void UpdateGenCategory(IObject servicePoint, int value)
        {
            try
            {
                IRelationshipClass iRelationshipClass = ModelNameFacade.RelationshipClassFromModelName(servicePoint.Class, esriRelRole.esriRelRoleAny, SchemaInfo.Electric.ClassModelNames.ServiceLocation);
                if (iRelationshipClass != null)
                {
                    ISet relatedObjects = iRelationshipClass.GetObjectsRelatedToObject(servicePoint as IObject);
                    if (relatedObjects.Count > 0)
                    {
                        IObject relatedObject = null;
                        relatedObjects.Reset();
                        //relatedObject = relatedObjects.Next() as IObject;
                        while ((relatedObject = relatedObjects.Next() as IObject) != null)
                        {
                            relatedObject.set_Value((relatedObject.Fields.FindField(_genCategoryFldname)), value);
                            relatedObject.Store();
                        }
                    }
                }

                //Updating genSymbology value for generationinfo ------------
                string genSymbologyvalue = string.Empty;
                if (value == 1)
                {
                    genSymbologyvalue = _genSymbologyvaluePrimary;
                }
                else if (value == 2)
                {
                    genSymbologyvalue = _genSymbologyvalueSecondary;
                }

                
                //Updating proper values for gensymbology field in generationinfo table
                IRelationshipClass iRelationshipClass_sp_geninfo = ModelNameFacade.RelationshipClassFromModelName(servicePoint.Class, esriRelRole.esriRelRoleAny, SchemaInfo.Electric.ClassModelNames.GenerationInfo);
                if (iRelationshipClass_sp_geninfo != null)
                {
                    ISet relatedObjects = iRelationshipClass_sp_geninfo.GetObjectsRelatedToObject(servicePoint as IObject);
                    if (relatedObjects.Count > 0)
                    {
                        IObject relatedObject = null;
                        relatedObjects.Reset();
                        //relatedObject = relatedObjects.Next() as IObject;
                        while ((relatedObject = relatedObjects.Next() as IObject) != null)
                        {
                            relatedObject.set_Value((relatedObject.Fields.FindField(_genSymbologyFldname)), genSymbologyvalue);
                            relatedObject.Store();
                        }
                    }
                }

            }
            catch (Exception exp)
            {
                _logger.Error(exp.Message + " at " + exp.StackTrace);
            }
        }
        /// <summary>
        /// preparing Complete lable text to update in ServiceLocation
        /// </summary>
        /// <param name="pObject"></param>
        /// <returns></returns>
      

        

        //Original PrepareLabelText(IObject pObject) function
        //private string PrepareLabelText(IObject pObject)
        //{
        //    int projectNameFldIdx = pObject.Fields.FindField(_projectNameField);
        //    int effRtInvFldIdx = pObject.Fields.FindField(_effRtInvField);
        //    int effRtMachFldIdx = pObject.Fields.FindField(_effRtMachField);

        //    string projName = pObject.get_Value(projectNameFldIdx) != null ? pObject.get_Value(projectNameFldIdx).ToString() : string.Empty;

        //    int invkW = pObject.get_Value(effRtInvFldIdx) != null ? (!string.IsNullOrEmpty(pObject.get_Value(effRtInvFldIdx).ToString().Trim()) ? (int)pObject.get_Value(effRtInvFldIdx) : 0) : 0;
        //    int machkW = pObject.get_Value(effRtMachFldIdx) != null ? (!string.IsNullOrEmpty(pObject.get_Value(effRtMachFldIdx).ToString().Trim()) ? (int)pObject.get_Value(effRtMachFldIdx) : 0) : 0; ;
        //    int totalkW = invkW + machkW;

        //    return projName + " " + totalkW.ToString();
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="relatedSPObject"></param>
        /// <param name="serviceLoc"></param>
        /// <returns></returns>
        private bool GetRelatedServiceLocation(IObject relatedSPObject, out IRow serviceLoc)
        {
            IRelationshipClass iRelSptoSL = ModelNameFacade.RelationshipClassFromModelName(relatedSPObject.Class, esriRelRole.esriRelRoleAny, SchemaInfo.Electric.ClassModelNames.ServiceLocation);
            if (iRelSptoSL != null)
            {
                ISet relatedSLObjects = iRelSptoSL.GetObjectsRelatedToObject(relatedSPObject as IObject);
                if (relatedSLObjects.Count == 1)
                {
                    IObject relatedSLObject = null;
                    relatedSLObjects.Reset();
                    while ((relatedSLObject = relatedSLObjects.Next() as IObject) != null)
                    {
                        //Now we need to check how many service points are connected to this service location.
                        //ISet relatedSPObjectsReverse = iRelSptoSL.GetObjectsRelatedToObject(relatedSLObject as IObject);
                        //if (relatedSPObjectsReverse.Count == 1)
                        //{
                            serviceLoc = relatedSLObject;
                            return true;
                        //}
                        //else
                        //{
                        //}
                    }
                }
                else
                {
                }
            }
            else
            {
            }
            serviceLoc = null;
            return false;
        }

        //private bool GetRelatedServiceLocation(IObject relatedSPObject, out IRow serviceLoc)
        //{
        //    IRelationshipClass iRelSptoSL = ModelNameFacade.RelationshipClassFromModelName(relatedSPObject.Class, esriRelRole.esriRelRoleAny, SchemaInfo.Electric.ClassModelNames.ServiceLocation);
        //    if (iRelSptoSL != null)
        //    {
        //        ISet relatedSLObjects = iRelSptoSL.GetObjectsRelatedToObject(relatedSPObject as IObject);
        //        if (relatedSLObjects.Count == 1)
        //        {
        //            IObject relatedSLObject = null;
        //            relatedSLObjects.Reset();
        //            while ((relatedSLObject = relatedSLObjects.Next() as IObject) != null)
        //            {
        //                //Now we need to check how many service points are connected to this service location.
        //                ISet relatedSPObjectsReverse = iRelSptoSL.GetObjectsRelatedToObject(relatedSLObject as IObject);
        //                if (relatedSPObjectsReverse.Count == 1)
        //                {
        //                    serviceLoc = relatedSLObject;
        //                    return true;
        //                }
        //                else
        //                {
        //                }
        //            }
        //        }
        //        else
        //        {
        //        }
        //    }
        //    else
        //    {
        //    }
        //    serviceLoc = null;
        //    return false;
        //}


        private void UpdateLabeltextInSl(IRow relServiceLoc, string labelText)
        {
            int labelTextFldIdx = relServiceLoc.Fields.FindField("LABELTEXT");
            if (labelTextFldIdx != -1)
            {
                relServiceLoc.set_Value(labelTextFldIdx, labelText);
            }
        }


        /// <summary>
        /// The model name "Generation Info" must be on the relationship dest in order to be enabled
        /// </summary>
        /// <param name="relClass">the relationship class</param>
        /// <param name="originClass">the relationship origin</param>
        /// <param name="destClass">the relationship destination</param>
        /// <param name="eEvent">the relationship event</param>
        /// <returns></returns>
        protected override bool InternalEnabled(IRelationshipClass relClass, IObjectClass originClass, IObjectClass destClass, mmEditEvent eEvent)
        {
            bool destClassEnabled = ModelNameFacade.ContainsClassModelName(destClass, new string[] { SchemaInfo.Electric.ClassModelNames.GenerationInfo });
            bool originClassEnabled = ModelNameFacade.ContainsClassModelName(originClass, new string[] { SchemaInfo.Electric.ClassModelNames.ServicePoint });
            _logger.Debug(string.Format("ClassModelName : {0} exist({2}) on ObjectClass{1}", SchemaInfo.Electric.ClassModelNames.GenerationInfo, destClass.AliasName, destClassEnabled));
            _logger.Debug(string.Format("ClassModelName : {0} exist({2}) on ObjectClass{1}", SchemaInfo.Electric.ClassModelNames.ServicePoint, originClass.AliasName, originClassEnabled));
            _logger.Debug(string.Format("Returning Visible : {0}", (destClassEnabled && originClassEnabled)));
            return (destClassEnabled && originClassEnabled && (eEvent == mmEditEvent.mmEventRelationshipCreated ||eEvent == mmEditEvent.mmEventRelationshipUpdated || eEvent == mmEditEvent.mmEventRelationshipDeleted));
        }

        #endregion Relationship AU overrides


        private int GetFieldIdxFromModelName(IObjectClass table, string fieldModelName)
        {
            int fieldIndex = -1;
            try
            {
                IField field = ModelNameManager.Instance.FieldFromModelName(table, fieldModelName);
                if (field != null)
                {
                    fieldIndex = table.FindField(field.Name);
                }
            }
            catch
            {
                fieldIndex = -1;
            }

            return fieldIndex;
        }



        #region PrepareLabelText: Evaluate Lable Text

        /// <summary>
        /// Function will prepare lable text with Project name and Generation Size
        /// </summary>
        /// <param name="pObject">Service Point Object</param>
        /// <returns></returns>
        private string PrepareLabelText(IObject pObject)
        {
            try
            {
                string labelText = string.Empty;
                IObject servicelocation = null;
                double totalkw = 0;
                string genSize = string.Empty;
                string projectName = string.Empty;
                GetServiceLocationFromServicePoint(pObject, out servicelocation);
                if (servicelocation != null)
                {
                    totalkw = PrepareLabelTextFromServiceLocation(servicelocation);
                    genSize = ModifyAccordingToValue(totalkw);
                    projectName = PrepareProjectNameFromServiceLocation(servicelocation);
                }
                labelText = projectName + " " + genSize;
                return labelText;
            }
            catch (Exception e)
            {
                _logger.Error("Error in function PrepareLabelText: " + e.Message);
                return null;
            }

        }

        /// <summary>
        /// To get ServiceLocation from the Service point provided
        /// </summary>
        /// <param name="pObject">Service Point Object</param>
        /// <param name="servicelocation">Output Service Location Object</param>
        /// <returns></returns>
        private bool GetServiceLocationFromServicePoint(IObject pObject, out IObject servicelocation)
        {
            try
            {
            IRelationshipClass iRelSptoSL = ModelNameFacade.RelationshipClassFromModelName(pObject.Class, esriRelRole.esriRelRoleAny, SchemaInfo.Electric.ClassModelNames.ServiceLocation);
            ISet relatedSPObjects = iRelSptoSL.GetObjectsRelatedToObject(pObject as IObject);
                if (relatedSPObjects.Count == 1)
                {
                    
                    IObject relatedSLObject = null;
                    relatedSPObjects.Reset();
                    while ((relatedSLObject = relatedSPObjects.Next() as IObject) != null)
                    {
                        servicelocation=relatedSLObject;
                        return true;
                    }
                }
            servicelocation = null;
            return false;
            
            }
            catch (Exception e)
            {
                _logger.Error("Error in function PrepareLabelText: " + e.Message);
                servicelocation = null;
                return false;
            }
        }

        private bool GetServiceLocationFromGeneration(IObject Generation, out IObject ServiceLocation)
        {
           IRelationshipClass iRelGenToSp = ModelNameFacade.RelationshipClassFromModelName(Generation.Class, esriRelRole.esriRelRoleAny, SchemaInfo.Electric.ClassModelNames.ServicePoint);
            if (iRelGenToSp != null)
            {
                ISet relatedSPObjects = iRelGenToSp.GetObjectsRelatedToObject(Generation as IObject);
                if (relatedSPObjects.Count == 1)
                {
                    IObject relatedSPObject = null;
                    relatedSPObjects.Reset();
                    //relatedObject = relatedObjects.Next() as IObject;
                    while ((relatedSPObject = relatedSPObjects.Next() as IObject) != null)
                    {
                        IRelationshipClass iRelSptoSL = ModelNameFacade.RelationshipClassFromModelName(relatedSPObject.Class, esriRelRole.esriRelRoleAny, SchemaInfo.Electric.ClassModelNames.ServiceLocation);
                        if (iRelSptoSL != null)
                        {
                            ISet relatedSLObjects = iRelSptoSL.GetObjectsRelatedToObject(relatedSPObject as IObject);
                            if (relatedSLObjects.Count == 1)
                            {
                                IObject relatedSLObject = null;
                                relatedSLObjects.Reset();
                                while ((relatedSLObject = relatedSLObjects.Next() as IObject) != null)
                                {
                                    ServiceLocation = relatedSLObject;
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            ServiceLocation = null;
            return false;

        }

        /// <summary>
        /// To Iterate Service Points on one Service Location and return single project name
        /// </summary>
        /// <param name="ServiceLocation">Service Location Object</param>
        /// <returns>Returns Project Name</returns>
        private string PrepareProjectNameFromServiceLocation(IObject ServiceLocation)
        {
            try
            {
            IRelationshipClass iRelSptoSL = ModelNameFacade.RelationshipClassFromModelName(ServiceLocation.Class, esriRelRole.esriRelRoleAny, SchemaInfo.Electric.ClassModelNames.ServicePoint);
            string projectName = string.Empty;
            if (iRelSptoSL != null)
            {
                ISet relatedSPObjectsReverse = iRelSptoSL.GetObjectsRelatedToObject(ServiceLocation as IObject);
                IObject relatedSPObject = null;
                while ((relatedSPObject = relatedSPObjectsReverse.Next() as IObject) != null)
                {
                    projectName = PrepareProjectNameFromServicePoint(relatedSPObject);
                    if (!string.IsNullOrEmpty(projectName))
                        return projectName;
                }
            }
            return projectName;   
            
            }
            catch (Exception e)
            {
                _logger.Error("Error in function PrepareLabelText: " + e.Message);
                
                return null;
            }
        }
        
        /// <summary>
        /// To Iterate Generations on one Service Point and return single project name
        /// </summary>
        /// <param name="ServicePoint">Service Point Object</param>
        /// <returns>Returns Project Name</returns>
        private string PrepareProjectNameFromServicePoint(IObject ServicePoint)
        {
            try
            {
            string projectName = string.Empty;
            IRelationshipClass iRelSPtoGen = ModelNameFacade.RelationshipClassFromModelName(ServicePoint.Class, esriRelRole.esriRelRoleAny, SchemaInfo.Electric.ClassModelNames.GenerationInfo);
           
            if (iRelSPtoGen != null)
            {
                ISet relatedGenISet = iRelSPtoGen.GetObjectsRelatedToObject(ServicePoint as IObject);
                IObject relatedGenObject = null;
                relatedGenISet.Reset();

                while ((relatedGenObject = relatedGenISet.Next() as IObject) != null)
                {
                    int projectNameFldIdx = relatedGenObject.Fields.FindField(_projectNameField);
                    projectName = relatedGenObject.get_Value(projectNameFldIdx) != null ? Convert.ToString(relatedGenObject.get_Value(projectNameFldIdx)) : string.Empty;
                    if (!string.IsNullOrEmpty(projectName))
                        return projectName;
                }
            }
           
            return projectName;
            }
            catch (Exception e)
            {
                _logger.Error("Error in function PrepareLabelText: " + e.Message);
                return null;
            }
        }
        
        /// <summary>
        /// Calculate the Gen Size Value According to the conditions
        /// </summary>
        /// <param name="totalkw">Total KW Value</param>
        /// <returns>Gen Size Value as string</returns>
        private string ModifyAccordingToValue(double totalkw)
        {
            try
            {
                string genSize = string.Empty;
                if (totalkw < 10)
                {
                    totalkw = Math.Round(totalkw, 1);
                    genSize = Convert.ToString(totalkw) + " KW";
                }
                else if (totalkw >= 10 && totalkw < 1000)
                {
                    totalkw = Math.Round(totalkw, 0);
                    genSize = Convert.ToString(totalkw) + " KW";
                }
                else if (totalkw >= 1000)
                {
                    totalkw = Math.Round(totalkw / 1000, 1);
                    genSize = Convert.ToString(totalkw) + " MW";
                }

                return genSize;
            }
            catch (Exception e)
            {
                _logger.Error("Error in function PrepareLabelText: " + e.Message);
                return null;
            }
        }

        /// <summary>
        ///  To Iterate Service Points on one Service Location and returns Generation Size
        /// </summary>
        /// <param name="ServiceLocation">Service Location Object</param>
        /// <returns>Total KW</returns>
        private double PrepareLabelTextFromServiceLocation(IObject ServiceLocation)
        {
            try
            {
                IRelationshipClass iRelSptoSL = ModelNameFacade.RelationshipClassFromModelName(ServiceLocation.Class, esriRelRole.esriRelRoleAny, SchemaInfo.Electric.ClassModelNames.ServicePoint);
                double totalkW = 0;
                if (iRelSptoSL != null)
                {
                    ISet relatedSPObjectsReverse = iRelSptoSL.GetObjectsRelatedToObject(ServiceLocation as IObject);
                    IObject relatedSPObject = null;
                    while ((relatedSPObject = relatedSPObjectsReverse.Next() as IObject) != null)
                    {
                        totalkW += PrepareLabelTextFromServicePoint(relatedSPObject);
                    }
                }
                return totalkW;
            }
            catch (Exception e)
            {
                _logger.Error("Error in function PrepareLabelTextFromServiceLocation: " + e.Message);
                return 0;
            }
            //Iterate all  Servcie point of Service location 
            //Use PrepareLabelTextFromServicePoint(ServicePoint);

            //Return Sum;
        }

        /// <summary>
        /// To Iterate Generations on one Service Point and  returns Generation Size
        /// </summary>
        /// <param name="ServicePoint">Service Point Object</param>
        /// <returns>Total KW</returns>
        private double PrepareLabelTextFromServicePoint(IObject ServicePoint)
        {
            try
            {
            IRelationshipClass iRelSPtoGen = ModelNameFacade.RelationshipClassFromModelName(ServicePoint.Class, esriRelRole.esriRelRoleAny, SchemaInfo.Electric.ClassModelNames.GenerationInfo);
            double totalkW = 0;
            if (iRelSPtoGen != null)
            {
                ISet relatedGenISet = iRelSPtoGen.GetObjectsRelatedToObject(ServicePoint as IObject);
                 IObject relatedGenObject = null;
                    relatedGenISet.Reset();
                    
                    while ((relatedGenObject = relatedGenISet.Next() as IObject) != null)
                    {
                        int effRtInvFldIdx = relatedGenObject.Fields.FindField(_effRtInvField);
                        int effRtMachFldIdx = relatedGenObject.Fields.FindField(_effRtMachField);


                        double invkW = relatedGenObject.get_Value(effRtInvFldIdx) != null ? (!string.IsNullOrEmpty(Convert.ToString(relatedGenObject.get_Value(effRtInvFldIdx)).Trim()) ? (double)relatedGenObject.get_Value(effRtInvFldIdx) : 0) : 0;
                        double machkW = relatedGenObject.get_Value(effRtMachFldIdx) != null ? (!string.IsNullOrEmpty(Convert.ToString(relatedGenObject.get_Value(effRtMachFldIdx)).Trim()) ? (double)relatedGenObject.get_Value(effRtMachFldIdx) : 0) : 0;
                        totalkW += invkW + machkW;
                    }
            }
            return totalkW;
            //Iterate all generations of Servcie point

            //Return Sum;
            }
            catch (Exception e)
            {
                _logger.Error("Error in function PrepareLabelTextFromServicePoint: " + e.Message);
                return 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SPObject"></param>
        /// <returns></returns>
        private bool IsPrimaryGeneration(IObject SPObject)
        {

            int primaryMeterGuidFldId = SPObject.Class.Fields.FindField("PRIMARYMETERGUID");
            if (!string.IsNullOrEmpty(Convert.ToString(SPObject.get_Value(primaryMeterGuidFldId))))
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        #endregion PrepareLabelText: Evaluate Lable Text
    }
}
    
