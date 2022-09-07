using System;
using System.Reflection;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.Geodatabase;

using Miner.ComCategories;
using Miner.Interop;

using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Framework;
using ESRI.ArcGIS.esriSystem;
using Miner.Geodatabase;

namespace PGE.Desktop.EDER.AutoUpdaters.Special
{
    /// <summary>
    /// Changes for ENOS to SAP migration, A special AU that Updates LabelText feild in Service location of related service point.
    /// </summary>
    [Guid("44AAEA51-5B65-4D67-901E-09B8F8291D7D")]
    [ProgId("PGE.Desktop.EDER.PGEUpdateLabelText")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    [ComVisible(true)]
    public class PGEUpdateLabelText : BaseSpecialAU
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PGEUpdateLabelText"/> class.  
        /// </summary>
        public PGEUpdateLabelText() : base("PGE Update Label Text AU") { }
        #endregion Constructors

        #region Privates

        private static readonly PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        private static string _projectNameField = "PROJECTNAME";
        private static string _effRtInvField = "EFFRATINGINVKW";
        private static string _effRtMachField = "EFFRATINGMACHKW";
        private static string _labeltext = "LABELTEXT"; 
        #endregion Privates

        /// <summary>
        /// Enable the AU when the update object AU category is selected.
        /// </summary>
        /// <param name="pObjectClass">The selected object class.</param>
        /// <param name="eEvent">The AU event type.</param>
        /// <returns></returns>
        protected override bool InternalEnabled(IObjectClass pObjectClass, Miner.Interop.mmEditEvent eEvent)
        {
            string[] modelNames = new string[] { SchemaInfo.Electric.ClassModelNames.GenerationInfo};
            bool enabled = ModelNameFacade.ContainsClassModelName(pObjectClass, modelNames);
            _logger.Debug(string.Format("ClassModelName : {0}, exist:{1}", SchemaInfo.Electric.ClassModelNames.GenerationInfo, enabled));
            if (((eEvent == Miner.Interop.mmEditEvent.mmEventFeatureUpdate) && (enabled)) || ((eEvent == Miner.Interop.mmEditEvent.mmEventFeatureCreate) && (enabled)) || ((eEvent == Miner.Interop.mmEditEvent.mmEventFeatureDelete) && (enabled)))
            {
                _logger.Debug("Returning Visible: true.");
                return true;
            }
            _logger.Debug("Returning Visible: false.");
            return false;
        }

        /// <summary>
        /// Executes the PGEUpdateGenCategory AU.
        /// </summary>
        /// <param name="pObject">The object being updated.</param>
        /// <param name="eAUMode">The AU mode.</param>
        /// <param name="eEvent">The edit event.</param>
        protected override void InternalExecute(IObject pObject, Miner.Interop.mmAutoUpdaterMode eAUMode, Miner.Interop.mmEditEvent eEvent)
        {
            if (ModelNameFacade.ContainsClassModelName(pObject.Class, SchemaInfo.Electric.ClassModelNames.GenerationInfo))
            {
                IRow relServiceLoc = null;
                if (eEvent == mmEditEvent.mmEventFeatureUpdate)
                {
                    if (GetServiceLocationForServicePoint(pObject, out relServiceLoc))
                    {

                        string labelText = PrepareLabelText(pObject);
                        if (!string.IsNullOrEmpty(labelText))
                        {
                            UpdateLabeltextInSl(relServiceLoc, labelText);
                        }
                    }

                }
            }
        }

        private void UpdateLabeltextInSl(IRow relServiceLoc, string labelText)
        {
            try
            {
                int labelTextFldIdx = relServiceLoc.Fields.FindField(_labeltext);
                if (labelTextFldIdx != -1)
                {
                    relServiceLoc.set_Value(labelTextFldIdx, labelText);
                    // Added below line because generationinfo updates were not captured for label text
                    relServiceLoc.Store();
                }
            }
            catch (Exception exp)
            {
                _logger.Error(exp.Message + " at " + exp.StackTrace);
            }
        }

        /*PrepareLabelText Update Changes*/
        //private bool CheckWhetherSLhasOnlyOneSp(IObject pObject, out IRow serviceLoc)
        //{
        //    IRelationshipClass iRelGenToSp = ModelNameFacade.RelationshipClassFromModelName(pObject.Class, esriRelRole.esriRelRoleAny, SchemaInfo.Electric.ClassModelNames.ServicePoint);
        //    if (iRelGenToSp != null)
        //    {
        //        ISet relatedSPObjects = iRelGenToSp.GetObjectsRelatedToObject(pObject as IObject);
        //        if (relatedSPObjects.Count == 1)
        //        {
        //            IObject relatedSPObject = null;
        //            relatedSPObjects.Reset();
        //            //relatedObject = relatedObjects.Next() as IObject;
        //            while ((relatedSPObject = relatedSPObjects.Next() as IObject) != null)
        //            {
        //                IRelationshipClass iRelSptoSL = ModelNameFacade.RelationshipClassFromModelName(relatedSPObject.Class, esriRelRole.esriRelRoleAny, SchemaInfo.Electric.ClassModelNames.ServiceLocation);
        //                if (iRelSptoSL != null)
        //                {
        //                    ISet relatedSLObjects = iRelSptoSL.GetObjectsRelatedToObject(relatedSPObject as IObject);
        //                    if (relatedSLObjects.Count == 1)
        //                    {
        //                        IObject relatedSLObject = null;
        //                        relatedSLObjects.Reset();
        //                        while ((relatedSLObject = relatedSLObjects.Next() as IObject) != null)
        //                        {
        //                            //Now we need to check how many service points are connected to this service location.
        //                            ISet relatedSPObjectsReverse = iRelSptoSL.GetObjectsRelatedToObject(relatedSLObject as IObject);
        //                            if (relatedSPObjectsReverse.Count == 1)
        //                            {
        //                                serviceLoc = relatedSLObject;
        //                                return true;
        //                            }
        //                            else
        //                            {
        //                                serviceLoc = null;
        //                                return false;
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        serviceLoc = null;
        //                        return false;
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            serviceLoc = null;
        //            return false;
        //        }
        //    }
        //    serviceLoc = null;
        //    return false;
        //}

        /* PrepareLabelText Update Changes */
        //private string PrepareLabelText(IObject pObject)
        //{
        //    int projectNameFldIdx = pObject.Fields.FindField(_projectNameField);
        //    int effRtInvFldIdx = pObject.Fields.FindField(_effRtInvField);
        //    int effRtMachFldIdx = pObject.Fields.FindField(_effRtMachField);

        //    string projName = pObject.get_Value(projectNameFldIdx) != null ? pObject.get_Value(projectNameFldIdx).ToString() : string.Empty;

        //    int invkW = pObject.get_Value(effRtInvFldIdx).Convert<int>(0);
        //    int machkW = pObject.get_Value(effRtMachFldIdx).Convert<int>(0);
        //    int totalkW = invkW + machkW;

        //    return projName + " " + totalkW.ToString();
        //}

        #region PrepareLabelText: Evaluate Lable Text

        private bool GetServiceLocationForServicePoint(IObject pObject, out IRow serviceLoc)
        {
            IRelationshipClass iRelGenToSp = ModelNameFacade.RelationshipClassFromModelName(pObject.Class, esriRelRole.esriRelRoleAny, SchemaInfo.Electric.ClassModelNames.ServicePoint);
            if (iRelGenToSp != null)
            {
                ISet relatedSPObjects = iRelGenToSp.GetObjectsRelatedToObject(pObject as IObject);
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
                                   serviceLoc = relatedSLObject;
                                   return true;
                                   
                                }
                            }
                            else
                            {
                                serviceLoc = null;
                                return false;
                            }
                        }
                    }
                }
                else
                {
                    serviceLoc = null;
                    return false;
                }
            }
            serviceLoc = null;
            return false;
        }
        

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
                IObject ServicePoint = null;
                double totalkw = 0;
                string genSize = string.Empty;
                string projectName = string.Empty;
                GetServicePointFromGeneration(pObject, out ServicePoint);
                if (ServicePoint != null)
                {
                    if (IsPrimaryGeneration(ServicePoint))
                    {
                        GetServiceLocationFromGeneration(pObject, out servicelocation);
                        if (servicelocation != null)
                        {
                            totalkw = PrepareLabelTextFromServiceLocation(servicelocation);
                            genSize = ModifyAccordingToValue(totalkw);
                            projectName = PrepareProjectNameFromServiceLocation(servicelocation);
                        }
                    }
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
                        servicelocation = relatedSLObject;
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

        private bool GetServicePointFromGeneration(IObject genObject, out IObject ServicePoint)
        {
            try
            {
                IRelationshipClass iRelGenToSp = ModelNameFacade.RelationshipClassFromModelName(genObject.Class, esriRelRole.esriRelRoleAny, SchemaInfo.Electric.ClassModelNames.ServicePoint);
                 if (iRelGenToSp != null)
                 {
                     ISet relatedSPObjects = iRelGenToSp.GetObjectsRelatedToObject(genObject as IObject);
                     if (relatedSPObjects.Count == 1)
                     {
                         ServicePoint = relatedSPObjects.Next() as IObject;
                         return true;
                     }
                 }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                ServicePoint = null;
                
            }
            ServicePoint = null;
            return false;
        }

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
