using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Miner.ComCategories;
using System.Runtime.InteropServices;
using PGE.Common.Delivery.Diagnostics;
using System.Reflection;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using PGE.Common.Delivery.ArcFM;
using Miner.Interop;


namespace PGE.Desktop.EDER.AutoUpdaters.LabelText
{
    /// <summary>
    /// ConduitLabel class updates the LabelText and LabelText2 field in the geodatabase for conduit features.
    /// </summary>
    [Guid("6DE2CC54-542B-4BE7-BE8B-930D273D20DB")]
    [ProgId("PGE.Desktop.EDER.SecUGConductorConduitFiberDcLabelAU")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]

    public class SecUGConductorFiberDcLabelAU : BaseSpecialAU
    {
private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        private StringBuilder _label;
        private string labelText1;
        private string labelText2;
        private string labelText3;
        private string labelText4;
        /// <summary>
        /// Initializes a new instance of the <see cref="ConduitLabel"/> class.
        /// </summary>
        public SecUGConductorFiberDcLabelAU()
            : base("PGE Update FiberDc Label in SecUG Feature")
        {
            _label = new StringBuilder();
        }
        #region Base Attribute AU Override
        /// <summary>
        /// Determines in which class the AU will be enabled
        /// </summary>
        /// <param name="objectClass"> Object's class. </param>
        /// <param name="eEvent">The edit event.</param>
        /// <returns> <c>true</c> if the AuoUpdater should be enabled; otherwise <c>false</c> </returns>
        protected override bool InternalEnabled(IObjectClass objectClass, mmEditEvent eEvent)
        {
            //Enabled if this feature class contains a status field
            if (ModelNameManager.FieldFromModelName(objectClass, SchemaInfo.Electric.FieldModelNames.Status) != null)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Determines whether actually this AU should be run, based on the AU Mode.
        /// </summary>
        /// <param name="eAUMode"> The auto updater mode. </param>
        /// <returns> <c>true</c> if the AuoUpdater should be executed; otherwise <c>false</c> </returns>
        protected override bool CanExecute(mmAutoUpdaterMode eAUMode)
        {
            if (eAUMode != mmAutoUpdaterMode.mmAUMNoEvents)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        protected override void InternalExecute(IObject obj, mmAutoUpdaterMode eAUMode, mmEditEvent eEvent)
        {

            //string sFiberDc_Yes = Convert.ToString(obj.get_Value(obj.Fields.FindField("FIBERIDC")));
            //if (sFiberDc_Yes == "Y")
            //{
                UpdateLabelText(obj);
            //}
        }
        #endregion
        public  void UpdateLabelText(ESRI.ArcGIS.Geodatabase.IObject obj)
        {
           try
            {
                Type auType = Type.GetTypeFromProgID("mmGeodatabase.MMAutoUpdater");
                object auObj = Activator.CreateInstance(auType);
                IMMAutoUpdater immAutoupdater = auObj as IMMAutoUpdater;
                mmAutoUpdaterMode currentAUMode = immAutoupdater.AutoUpdaterMode;
                var subtype = obj.SubtypeCodeAsEnum<Subtype>();
                IFeature SecUGFeature = null;

                if (subtype == Subtype.None)
                {
                    return;
                }
                string labelText = string.Empty;

                //labelText=obj.fi 
                //get sFiberIDC value
                ISet relatedFeatures = FindRelatedFeature_FiberIdc(obj as IFeature);
                
                object pRelatedObj = relatedFeatures.Next();
                if (pRelatedObj == null) { return; }
                while (pRelatedObj != null)
                {
                    SecUGFeature = (IFeature)pRelatedObj;
                    string sFiberDc_Yes = Convert.ToString(obj.get_Value(obj.Fields.FindField("FIBERIDC")));
                    labelText = SecUGFeature.get_Value(SecUGFeature.Fields.FindField("LABELTEXT")).ToString();
                    if (sFiberDc_Yes == "Y")
                    {
                        if ((SecUGFeature != null) && (!labelText.EndsWith("w/ FO")))
                        {

                            labelText += sFiberDc_Yes == "Y" && !labelText.EndsWith("w/ FO") ? " w/ FO" : string.Empty;
                            SecUGFeature.set_Value(SecUGFeature.Fields.FindField("LABELTEXT"), labelText.ToString());

                            _logger.Debug("labelText :" + labelText);
                        }
                    }
                    else
                    {
                        labelText = labelText.Replace("w/ FO", "");
                        SecUGFeature.set_Value(SecUGFeature.Fields.FindField("LABELTEXT"), labelText.ToString());
                    }

                    labelText2 = SecUGFeature.get_Value(SecUGFeature.Fields.FindField("LABELTEXT2")).ToString();
                    if (sFiberDc_Yes == "Y")
                    {
                        if (((SecUGFeature != null) && (!labelText2.EndsWith("w/ FO"))))
                        {
                            labelText2 += sFiberDc_Yes == "Y" && !labelText2.EndsWith("w/ FO") ? " w/ FO" : string.Empty;
                            SecUGFeature.set_Value(SecUGFeature.Fields.FindField("LABELTEXT2"), labelText2.ToString());
                            _logger.Debug("labelText2" + labelText2);
                        }
                    }
                    else
                    {                     
                        labelText2 = labelText2.Replace("w/ FO", "");   
                        SecUGFeature.set_Value(SecUGFeature.Fields.FindField("LABELTEXT2"), labelText2.ToString());
                    }
                    labelText3 = SecUGFeature.get_Value(SecUGFeature.Fields.FindField("LABELTEXT3")).ToString();
                    if (sFiberDc_Yes == "Y")
                    {
                        if (((SecUGFeature != null) && (!labelText3.EndsWith("w/ FO"))))
                        {
                            labelText3 += sFiberDc_Yes == "Y" && !labelText3.EndsWith("w/ FO") ? " w/ FO" : string.Empty;
                            SecUGFeature.set_Value(SecUGFeature.Fields.FindField("LABELTEXT3"), labelText3.ToString());
                            _logger.Debug("labelText3" + labelText3);
                        }
                    }
                    else
                    {
                        labelText3 = labelText3.Replace("w/ FO", "");
                        SecUGFeature.set_Value(SecUGFeature.Fields.FindField("labelText3"), labelText3.ToString());
                    }
                    //labelText4 = SecUGFeature.get_Value(SecUGFeature.Fields.FindField("LABELTEXT4")).ToString();

                    //if (((SecUGFeature != null) && (!labelText4.EndsWith("w/ FO"))))
                    //{
                    //    labelText4 = labelText4 + " w/ FO";
                    //    SecUGFeature.set_Value(SecUGFeature.Fields.FindField("LABELTEXT4"), labelText4.ToString());
                    //    _logger.Debug("labelText4" + labelText4);
                    //}
                    immAutoupdater.AutoUpdaterMode = mmAutoUpdaterMode.mmAUMNoEvents;
                    SecUGFeature.Store();
                    immAutoupdater.AutoUpdaterMode = currentAUMode;
                    pRelatedObj = relatedFeatures.Next();
                }
                //RefreshAnnotation();
                //TCS-YXA6-to handle Crash Scenario1

                //ESRI.ArcGIS.Carto.IMap pMAP = (ESRI.ArcGIS.Carto.IMap)((ESRI.ArcGIS.ArcMapUI.IMxDocument)PGE.Common.Delivery.ArcFM.ApplicationFacade.Application.Document).FocusMap;


                //ESRI.ArcGIS.Carto.IEnumLayer enumLayer = pMAP.get_Layers(null, true);
                //ESRI.ArcGIS.Carto.ILayer layer = enumLayer.Next();
                //while (layer != null)
                //{
                //    if (layer is ESRI.ArcGIS.Carto.IFeatureLayer)
                //    {
                //        ESRI.ArcGIS.Carto.IFeatureLayer fLayer = layer as ESRI.ArcGIS.Carto.IFeatureLayer;
                //        if (fLayer != null)
                //        {
                //            if ((fLayer as IDataset).BrowseName.Trim().ToUpper() == (obj.Class as IDataset).BrowseName.Trim().ToUpper())
                //            {
                //                pMAP.SelectFeature(fLayer, obj as IFeature);
                //                break;
                //            }
                //        }
                //    }
                //    layer = enumLayer.Next();
                //}
                
            }
            catch
            {
            }
            finally
            {
               
            }
             
        }
        public ISet FindRelatedFeature_FiberIdc(IFeature pFeature)
        {
            ISet relatedFeatures = null;
            IObjectClass objectClass = null;
            IEnumRelationshipClass relClasses = null;
            IRelationshipClass relClass = null;
            
            bool priUGFound = false;
           
            try
            {
                objectClass = pFeature.Class;
                relClasses = objectClass.get_RelationshipClasses(esriRelRole.esriRelRoleAny);
                relClasses.Reset();
                relClass = relClasses.Next();
                while (relClass != null)
                {
                    priUGFound = false;
                    if (ModelNameManager.ContainsClassModelName(relClass.OriginClass, SchemaInfo.Electric.ClassModelNames.SecondaryUGConductor ))
                    {
                        priUGFound = true;
                    }
                    else if (ModelNameManager.ContainsClassModelName(relClass.DestinationClass, SchemaInfo.Electric.ClassModelNames.SecondaryUGConductor))
                    {
                        priUGFound = true;
                    }

                    if (priUGFound == true)
                    {
                        //if (ModelNameManager.ContainsClassModelName(relClass.DestinationClass, SchemaInfo.Electric.ClassModelNames.PGEConduitSystem))
                        //{
                        relatedFeatures = relClass.GetObjectsRelatedToObject(pFeature);
                      
                        break;
                        //}
                    }
                    relClass = relClasses.Next();
                }
                return relatedFeatures;
            }
            catch (Exception ex)
            {
                return null;
            }

        }


    }
    }

