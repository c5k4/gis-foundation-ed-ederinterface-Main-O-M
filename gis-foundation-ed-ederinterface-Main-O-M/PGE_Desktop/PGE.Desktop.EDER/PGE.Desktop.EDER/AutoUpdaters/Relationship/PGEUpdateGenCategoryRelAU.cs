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
    /// Changes for ENOS to SAP migration, A relationship AU that Updates Gen Category feild in Service location of related service point.
    /// </summary>
    [ComVisible(true)]
    [Guid("27938717-5CF6-4634-A14B-E1D3C865A717")]
    [ProgId("PGE.Desktop.EDER.PGEUpdateGenCategoryRelAU")]
    [ComponentCategory(ComCategory.RelationshipAutoupdateStrategy)]
    public class PGEUpdateGenCategoryRelAU : BaseRelationshipAU
    {
        #region private
        private static readonly PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        private static string _projectNameField = "PROJECTNAME";
        private static string _effRtInvField = "EFFRATINGINVKW";
        private static string _effRtMachField = "EFFRATINGMACHKW";
        private static string _genCategoryFldname = "GENCATEGORY";
        #endregion private

        #region Constructors
        /// <summary>
        /// constructor for the Populate Conductor Phase Rel AU
        /// </summary>
        public PGEUpdateGenCategoryRelAU()
            : base("PGE Update Gen Category On Relation Update")
        {
        }

        #endregion Constructors

        #region Relationship AU overrides
        /// <summary>
        /// Update the Gen Category field value if any one creates a new realted service location / Primary meter / Transformer with any existing service point. 
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
            if (origObject == null || destObject == null)
            {
                //log the warning and return the control 
                _logger.Warn("Either OriginObject or DestinationObject is <Null>, exiting.");
                return;
            }
            IObject genInfoRecord = null;
            if (ModelNameFacade.ContainsClassModelName(relationship.OriginObject.Class, new string[] { SchemaInfo.Electric.ClassModelNames.PrimaryMeter }) &&
                ModelNameFacade.ContainsClassModelName(relationship.DestinationObject.Class, new string[] { SchemaInfo.Electric.ClassModelNames.ServicePoint }))
            {
                if (eEvent == mmEditEvent.mmEventRelationshipCreated || eEvent == mmEditEvent.mmEventRelationshipDeleted)
                {
                    //If a new relatenship between Primary meter and Service point is created
                    if (CheckGenerationInfoExist(destObject, ref genInfoRecord))
                    {
                        IObject serviceLocObj = UpdateGenCategory(destObject, 1);
                        UpdateLabelTextInSl(serviceLocObj, genInfoRecord);
                    }
                }

            }
            else if (ModelNameFacade.ContainsClassModelName(relationship.OriginObject.Class, new string[] { SchemaInfo.Electric.ClassModelNames.PGETransformer }) &&
                ModelNameFacade.ContainsClassModelName(relationship.DestinationObject.Class, new string[] { SchemaInfo.Electric.ClassModelNames.ServicePoint }))
            {
                //If a new relatenship between Transformer and Service point is created
                if (CheckGenerationInfoExist(destObject, ref genInfoRecord))
                {
                    UpdateGenCategory(destObject, 2);
                }
            }
            else if (ModelNameFacade.ContainsClassModelName(relationship.OriginObject.Class, new string[] { SchemaInfo.Electric.ClassModelNames.ServiceLocation }) &&
                ModelNameFacade.ContainsClassModelName(relationship.DestinationObject.Class, new string[] { SchemaInfo.Electric.ClassModelNames.ServicePoint }))
            {
                if (eEvent == mmEditEvent.mmEventRelationshipCreated)
                {
                    //If a new relatenship between Service Location and Service point is created
                    //ENOS CHANGE- Check whether Service Point is related to PM  if yes then update GenCatarogy 2 and if it related to transformer then update 1.
                    //If Service point already has a relationship with primary meter then UpdateGenCategory(1)
                    //Code Change - Prod Issue Start

                    //Check if Service Point has Related Generationinfo
                    IRelationshipClass iRelSPtoGen = ModelNameFacade.RelationshipClassFromModelName(destObject.Class, esriRelRole.esriRelRoleAny, SchemaInfo.Electric.ClassModelNames.GenerationInfo);

                    if (iRelSPtoGen != null)
                    {
                        ISet relatedGenObjects = iRelSPtoGen.GetObjectsRelatedToObject(destObject as IObject);
                        if (relatedGenObjects.Count == 1)
                        {
                            if (CheckRelationshipExitfromSPtoPrimaryMeter(destObject) == true)
                            {  
                                double totalkw = PrepareLabelTextFromServiceLocation((IObject)origObject);
                                string genSize = ModifyAccordingToValue(totalkw);
                                string projectName = PrepareProjectNameFromServiceLocation((IObject)origObject);
                                string labelText = projectName + " " + genSize;

                                if (origObject.Fields.FindField(_genCategoryFldname) > 0)
                                {
                                    origObject.set_Value((origObject.Fields.FindField(_genCategoryFldname)), 1);
                                    origObject.set_Value((origObject.Fields.FindField("LABELTEXT")), labelText);
                                    origObject.Store();
                                }
                            }
                            //If service point already has a relationship with transformer then updagt(2)
                            else if (CheckRelationshipExitfromSPtoTransformer(destObject) == true)
                            {
                                UpdateGenCategory(destObject, 2);
                            }
                        }
                    }
                    //Code Change - Prod Issue End
                }
                else if (eEvent == mmEditEvent.mmEventRelationshipDeleted)
                {
                    try
                    {
                        IRelationshipClass iRelSPtoGen = ModelNameFacade.RelationshipClassFromModelName(destObject.Class, esriRelRole.esriRelRoleAny, SchemaInfo.Electric.ClassModelNames.GenerationInfo);

                        if (iRelSPtoGen != null)
                        {
                            ISet relatedGenObjects = iRelSPtoGen.GetObjectsRelatedToObject(destObject as IObject);
                            if (relatedGenObjects.Count == 1)
                            {
                                // Find all generation related to all service points for service location. If multiple generation found then don't update Gencategory
                                if (ServiceLocationHasSingleGeneration(origObject))
                                {
                                    if (origObject.Fields.FindField(_genCategoryFldname) > 0)
                                    {
                                        origObject.set_Value((origObject.Fields.FindField(_genCategoryFldname)), 0);
                                        origObject.set_Value((origObject.Fields.FindField("LABELTEXT")), string.Empty);
                                        origObject.Store();
                                    }
                                }
                                else
                                { 
                                    // If its a primary generation then update the label text only
                                    if (origObject.Fields.FindField(_genCategoryFldname) > 0)
                                    {
                                        int genCategory = 0;
                                        if (int.TryParse(Convert.ToString(origObject.get_Value((origObject.Fields.FindField(_genCategoryFldname)))),out genCategory))
                                        {
                                            if (genCategory == 1)
                                            {
                                                double totalkw = PrepareLabelTextFromServiceLocation((IObject)origObject);
                                                string genSize = ModifyAccordingToValue(totalkw);
                                                string projectName = PrepareProjectNameFromServiceLocation((IObject)origObject);
                                                string labelText = projectName + " " + genSize;

                                                origObject.set_Value((origObject.Fields.FindField("LABELTEXT")), labelText);
                                                origObject.Store();
                                            }
                                        }                                       
                                    }                                
                                }
                            }
                        }
                    }
                    catch (Exception exp)
                    {
                        _logger.Error(exp.Message + " at " + exp.StackTrace);
                    }
                }                    
            }

            //Refresh the Map here
            try
            {
                Type t = Type.GetTypeFromCLSID(typeof(ESRI.ArcGIS.Framework.AppRefClass).GUID);
                System.Object obj = Activator.CreateInstance(t);
                ESRI.ArcGIS.Framework.IApplication app = obj as ESRI.ArcGIS.Framework.IApplication;
                ESRI.ArcGIS.ArcMapUI.IMxDocument mxDoc = app.Document as ESRI.ArcGIS.ArcMapUI.IMxDocument;
                mxDoc.ActiveView.PartialRefresh(ESRI.ArcGIS.Carto.esriViewDrawPhase.esriViewAll, null, null);
            }
            catch (Exception exp)
            {
            }   
        }

        private bool CheckRelationshipExitfromSPtoTransformer(IObject destObject)
        {
            bool RelExist = false;
            IRelationshipClass iRelSptoTranformer = ModelNameFacade.RelationshipClassFromModelName(destObject.Class, esriRelRole.esriRelRoleAny, SchemaInfo.Electric.ClassModelNames.PGETransformer);
           if (iRelSptoTranformer != null)
           {
               ISet relatedSLObjects = iRelSptoTranformer.GetObjectsRelatedToObject(destObject as IObject);    
               if (relatedSLObjects.Count == 1)
               {
                   RelExist= true;
               }
               
           }
           return RelExist;
        }

        private bool CheckRelationshipExitfromSPtoPrimaryMeter(IObject destObject)
        {
            bool RelExist = false;
            IRelationshipClass iRelSptoPM = ModelNameFacade.RelationshipClassFromModelName(destObject.Class, esriRelRole.esriRelRoleAny, SchemaInfo.Electric.ClassModelNames.PrimaryMeter);
            if (iRelSptoPM != null)
            {
                ISet relatedSLObjects = iRelSptoPM.GetObjectsRelatedToObject(destObject as IObject);
                if (relatedSLObjects.Count == 1)
                {
                    RelExist = true;
                }

            }
            return RelExist;
        }

        private void UpdateLabelTextInSl(IObject relServiceLoc, IObject genInfoRecord)
        {
            if (relServiceLoc != null)
            {
                string labelText = PrepareLabelText(genInfoRecord);
                if (!string.IsNullOrEmpty(labelText))
                {
                    UpdateLabeltextInSl(relServiceLoc, labelText);
                }
            }
        }

        private void UpdateLabeltextInSl(IRow relServiceLoc, string labelText)
        {
            int labelTextFldIdx = relServiceLoc.Fields.FindField("LABELTEXT");
            if (labelTextFldIdx != -1)
            {
                relServiceLoc.set_Value(labelTextFldIdx, labelText);
            }
        }

        private string PrepareLabelText(IObject pObject)
        {
            int projectNameFldIdx = pObject.Fields.FindField(_projectNameField);
            int effRtInvFldIdx = pObject.Fields.FindField(_effRtInvField);
            int effRtMachFldIdx = pObject.Fields.FindField(_effRtMachField);

            string projName = pObject.get_Value(projectNameFldIdx) != null ? pObject.get_Value(projectNameFldIdx).ToString() : string.Empty;

            int invkW = pObject.get_Value(effRtInvFldIdx).Convert<int>(0);
            int machkW = pObject.get_Value(effRtMachFldIdx).Convert<int>(0);
            int totalkW = invkW + machkW;

            return projName + " " + totalkW.ToString();
        }

        private void UpdateGenCategory(IObject pObject)
        {
            int transformerGuidFldId = pObject.Class.Fields.FindField("TRANSFORMERGUID");
            int primaryMeterGuidFldId = pObject.Class.Fields.FindField("PRIMARYMETERGUID");
            int serviceLocGuidFldId = pObject.Class.Fields.FindField("SERVICELOCATIONGUID");
            string transformerGUID = pObject.get_Value(transformerGuidFldId).ToString();
            string primarymeterGUID = pObject.get_Value(primaryMeterGuidFldId).ToString();
            string servicelocationGUID = pObject.get_Value(serviceLocGuidFldId).ToString();
            
            if (!string.IsNullOrEmpty(transformerGUID) && !string.IsNullOrEmpty(servicelocationGUID))
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

        private bool CheckGenerationInfoExist(IObject servicepointObj, ref IObject genInfoObject)
        {
            IRelationshipClass iRelationshipClass = ModelNameFacade.RelationshipClassFromModelName(servicepointObj.Class, esriRelRole.esriRelRoleAny, SchemaInfo.Electric.ClassModelNames.GenerationInfo);
            if (iRelationshipClass != null)
            {
                ISet relatedObjects = iRelationshipClass.GetObjectsRelatedToObject(servicepointObj as IObject);
                if (relatedObjects.Count == 1)
                {
                    relatedObjects.Reset();
                    genInfoObject = relatedObjects.Next() as IObject;
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }

        /// <summary>
        /// Updates GENCATEGORY field in service location feature class
        /// </summary>
        /// <param name="servicePoint"></param>
        /// <param name="value"></param>
        private IObject UpdateGenCategory(IObject servicePoint, int value)
        {
            IObject serviceLocObj = null;
            IRelationshipClass iRelationshipClass = ModelNameFacade.RelationshipClassFromModelName(servicePoint.Class, esriRelRole.esriRelRoleAny, SchemaInfo.Electric.ClassModelNames.ServiceLocation);
            if (iRelationshipClass != null)
            {
                ISet relatedObjects = iRelationshipClass.GetObjectsRelatedToObject(servicePoint as IObject);

                IEnumRelationship rell = iRelationshipClass.GetRelationshipsForObject(servicePoint as IObject);

                 IRelationship relShip = rell.Next();

                 while (relShip != null)
                 {
                     relShip = rell.Next();

                 }
                

                if (relatedObjects.Count == 1)
                {
                    IObject relatedObject = null;
                    relatedObjects.Reset();
                    //relatedObject = relatedObjects.Next() as IObject;
                    while ((relatedObject = relatedObjects.Next() as IObject) != null)
                    {
                        relatedObject.set_Value((relatedObject.Fields.FindField(_genCategoryFldname)), value);
                        relatedObject.Store();
                        return serviceLocObj;
                    }
                }
                else
                {
                    return null;
                    //If more than two service locations found, no need to update GEN CATEGORY
                }
            }
            return serviceLocObj;
        }
        
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
                        ISet relatedSPObjectsReverse = iRelSptoSL.GetObjectsRelatedToObject(relatedSLObject as IObject);
                        if (relatedSPObjectsReverse.Count == 1)
                        {
                            serviceLoc = relatedSLObject;
                            return true;
                        }
                        else
                        {
                        }
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
        /// <summary>
        /// The model name "Service Point" must be on the relationship dest in order to be enabled
        /// </summary>
        /// <param name="relClass">the relationship class</param>
        /// <param name="originClass">the relationship origin</param>
        /// <param name="destClass">the relationship destination</param>
        /// <param name="eEvent">the relationship event</param>
        /// <returns></returns>
        protected override bool InternalEnabled(IRelationshipClass relClass, IObjectClass originClass, IObjectClass destClass, mmEditEvent eEvent)
        {
            bool originClassEnabled = ModelNameFacade.ContainsClassModelName(originClass, new string[] { SchemaInfo.Electric.ClassModelNames.PrimaryMeter, SchemaInfo.Electric.ClassModelNames.PGETransformer, SchemaInfo.Electric.ClassModelNames.ServiceLocation });
            bool destClassEnabled = ModelNameFacade.ContainsClassModelName(destClass, new string[] { SchemaInfo.Electric.ClassModelNames.ServicePoint });
            _logger.Debug(string.Format("ClassModelName : {0} exist({2}) on ObjectClass{1}", SchemaInfo.Electric.ClassModelNames.ServicePoint, destClass.AliasName, destClassEnabled));
            _logger.Debug(string.Format("Returning Visible : {0}", (destClassEnabled && originClassEnabled)));
            return (destClassEnabled && originClassEnabled && (eEvent == mmEditEvent.mmEventRelationshipCreated || eEvent == mmEditEvent.mmEventRelationshipDeleted));
        }

        /// <summary>
        /// This function return boolean based on the fact that service location having multiple generation or not
        /// </summary>
        /// <param name="servicelocObject"></param>
        /// <returns></returns>
        private bool ServiceLocationHasSingleGeneration(IObject servicelocObject)
        {
            bool bServiceLocationHasSingleGeneration = true; 
            try
            {
                IRelationshipClass iRelSLtoSP = ModelNameFacade.RelationshipClassFromModelName(servicelocObject.Class, esriRelRole.esriRelRoleAny, SchemaInfo.Electric.ClassModelNames.ServicePoint);
                if (iRelSLtoSP != null)
                {
                    ISet relatedSPointObjects = iRelSLtoSP.GetObjectsRelatedToObject(servicelocObject as IObject);
                    if (relatedSPointObjects.Count > 0)
                    {
                        IObject relatedSPointObject = null;
                        relatedSPointObjects.Reset();
                        while ((relatedSPointObject = relatedSPointObjects.Next() as IObject) != null)
                        {
                            //Now we need to check service point having generation or not.
                            IRelationshipClass iRelSPtoGen = ModelNameFacade.RelationshipClassFromModelName(relatedSPointObject.Class, esriRelRole.esriRelRoleAny, SchemaInfo.Electric.ClassModelNames.GenerationInfo);

                            if (iRelSPtoGen != null)
                            {
                                ISet relatedGenObjects = iRelSPtoGen.GetObjectsRelatedToObject(relatedSPointObject as IObject);
                                if (relatedGenObjects.Count >0)
                                {
                                    bServiceLocationHasSingleGeneration = false;                                    
                                }                               
                            }

                            if (!bServiceLocationHasSingleGeneration)
                                break;
                        }
                    }                    
                }                  
            }
            catch (Exception exp)
            {
                _logger.Error(exp.Message+" at "+exp.StackTrace);
            }
            return bServiceLocationHasSingleGeneration;           
        }

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
                        //PREPARELABELTEXTPG-Check the type of Generation Here, Pass the case only for Primary Generation
                        int primaryMeterGuidFldId = relatedSPObject.Class.Fields.FindField("PRIMARYMETERGUID");
                        if (relatedSPObject.get_Value(primaryMeterGuidFldId) != null)
                        {
                            //Call the Function to find out generationinfo on SP and evaluate LabelText
                            totalkW += PrepareLabelTextFromServicePoint(relatedSPObject);
                        }

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
                        //PREPARELABELTEXTPG-Check the type of Generation Here, Pass the case only for Primary Generation
                        int primaryMeterGuidFldId = relatedSPObject.Class.Fields.FindField("PRIMARYMETERGUID");
                        if (relatedSPObject.get_Value(primaryMeterGuidFldId) != null)
                        {
                            //Call the Function to find out generationinfo on SP and evaluate LabelText
                            projectName = PrepareProjectNameFromServicePoint(relatedSPObject);
                            if (!string.IsNullOrEmpty(projectName))
                                return projectName;
                        }
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

        #endregion Relationship AU overrides
    }
}
