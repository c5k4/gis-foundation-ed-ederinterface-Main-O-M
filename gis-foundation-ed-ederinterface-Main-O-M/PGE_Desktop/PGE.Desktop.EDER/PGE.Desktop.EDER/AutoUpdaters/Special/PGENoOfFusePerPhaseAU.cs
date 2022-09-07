using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using Miner.Interop;
using Miner.Geodatabase;
using Miner.ComCategories;
using PGE.Common.Delivery.Diagnostics;
using PGE.Common.Delivery.Framework;
using PGE.Common.Delivery.ArcFM;
using log4net;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;

namespace PGE.Desktop.EDER.AutoUpdaters.Special
{
    /// <summary>
    /// INC000003862231 - EDER Issue (Config): Annotation for Fuses needs to be updated to include a prefix.
    /// Class - Annotation for Fuses needs to be updated to include a prefix if a number has been entered for "Number of fuses per phase" attribute.  The number entered must preceed the Fuse/Type Anno (e.g. 1-25CL or 2-25CL). 
    /// </summary>
    [Guid("c303aaef-9a25-4514-97a9-c637f831204d")]
    //[ClassInterface(ClassInterfaceType.None)]
    [ProgId("PGE.Desktop.EDER.AutoUpdaters.Special.PGENoOfFusePerPhaseAU")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    public class PGENoOfFusePerPhaseAU : BaseSpecialAU
    {
        /// <summary>
        /// Initializes the new instance of <see cref="PGENoOfFusePerPhaseAU"/>
        /// </summary>
        /// 
        //const string strFUSETableName = "EDGIS.FUSE";
        public PGENoOfFusePerPhaseAU() : base("PGE Annotation No Of Fuse Per Phase AU") { }        

        #region Private Variables
        /// <summary>
        /// Logs the debug/error information
        /// </summary>
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        #endregion Private Variables

        #region Override Methods

        protected override void InternalExecute(IObject pObject, mmAutoUpdaterMode eAUMode, mmEditEvent eEvent)
        {
            try
            {
                IFeature feat = pObject as IFeature;  
                //if(feat.Class.AliasName ==
                int iSubTypeCd = 0;
                int iLinkRating = 0;
                String iNumOfPhases = String.Empty;
                String strLinkType = String.Empty;
                String strLinkRating = String.Empty;
                
                iSubTypeCd = Convert.ToInt32(pObject.GetFieldValue("SUBTYPECD", false, ""));
                iNumOfPhases = pObject.GetFieldValue("NUMBERFUSESPERPHASE", false, "").ToString();
                int fieldIx = feat.Fields.FindField("LINKTYPE");
                //Get the Domain description
                object objLinkType = GetValueFromDomain(feat, feat.Fields.get_Field(fieldIx));

                if (objLinkType != null)
                {
                    try
                    {
                        strLinkType = objLinkType.ToString();
                    }
                    catch { strLinkType = ""; }
                }
              
                int fieldIxLr = feat.Fields.FindField("LINKRATING");
                //Get the Domain description
                object objLinkTypeLr = GetValueFromDomain(feat, feat.Fields.get_Field(fieldIxLr));

                if (objLinkTypeLr != null)
                {
                    strLinkRating = objLinkTypeLr.ToString();
                    strLinkRating = strLinkRating.Substring(0, strLinkRating.Length - 2);
                    iLinkRating = Convert.ToInt32(strLinkRating);
                }

                String strIsOnDeviceGroup = "NO";
                IEnumRelationshipClass relClasses = feat.Class.get_RelationshipClasses(esriRelRole.esriRelRoleAny);
                for (IRelationshipClass rel = relClasses.Next(); rel != null; rel = relClasses.Next())
                {
                    IFeatureClass originClass = rel.OriginClass as IFeatureClass;

                    if (originClass.AliasName.ToUpper() == "DEVICE GROUP")
                    {                        
                        ISet relatedObjects = rel.GetObjectsRelatedToObject((IObject)feat);
                        for (IObject relObj = relatedObjects.Next() as IObject; relObj != null; relObj = relatedObjects.Next() as IObject)
                        {
                            try
                            {
                                if (relObj != null)
                                {
                                    if (Convert.ToInt32(relObj.GetFieldValue("SUBTYPECD", true, null)) == 2)
                                    {
                                        if (Convert.ToString(relObj.GetFieldValue("DEVICEGROUPTYPE", true, null)).ToUpper() == "Subsurface Fused Switch Dual Well".ToUpper())
                                        {
                                            strIsOnDeviceGroup = "YES";
                                        }
                                    }
                                }
                                break;
                            }
                            catch (Exception)
                            {

                            }
                        }                        
                    }
                    if (strIsOnDeviceGroup == "YES")
                    { break; }
                }

                if (iSubTypeCd == 2 && strIsOnDeviceGroup == "YES")
                {
                    if (iSubTypeCd == 2 && (iNumOfPhases == "1" || iNumOfPhases == "2") && strIsOnDeviceGroup == "YES")
                    {
                        // Not include phase=1 in the labeltext
                        if (iNumOfPhases == "1")
                        {
                            if (String.IsNullOrEmpty(strLinkType))
                                UpdateAnnoFeature(feat, iLinkRating.ToString(), relClasses);
                            else
                                UpdateAnnoFeature(feat, iLinkRating.ToString() + strLinkType, relClasses);
                        }
                        else
                        {
                            if (String.IsNullOrEmpty(strLinkType))
                                UpdateAnnoFeature(feat, iNumOfPhases + "-" + iLinkRating.ToString(), relClasses);
                            else
                                UpdateAnnoFeature(feat, iNumOfPhases + "-" + iLinkRating.ToString() + strLinkType, relClasses);
                        }
                    }
                    else if (iSubTypeCd == 2 && String.IsNullOrEmpty(iNumOfPhases) && strIsOnDeviceGroup == "YES")
                    {
                        if (String.IsNullOrEmpty(strLinkType))
                            UpdateAnnoFeature(feat, iLinkRating.ToString(), relClasses);
                        else
                            UpdateAnnoFeature(feat, iLinkRating.ToString() + strLinkType, relClasses);
                    }
                }
                else 
                {
                    if (String.IsNullOrEmpty(strLinkType))
                        UpdateAnnoFeature(feat, iLinkRating.ToString(), relClasses);
                    else
                        UpdateAnnoFeature(feat, iLinkRating.ToString() + strLinkType, relClasses);
                }
                //return iNumOfPhases;
            }
            catch (Exception ex)
            {
                _logger.Error("Error executing PGE Annotation No Of Fuse Per Phase AU. Message: " + ex.Message);
                //return null;
            }

        }
        #endregion Override Methods

        #region Private Methods

        /// <summary>
        /// Updates Annotations properties
        /// </summary>
        /// <param name="annoFeature">AnnotationFeature to be updated</param>
        private void UpdateAnnoFeature(IFeature feature, String sAnnoText, IEnumRelationshipClass relClass)
        {
            try
            {
                //relClass.Reset();
                //for (IRelationshipClass rel = relClass.Next(); rel != null; rel = relClass.Next())
                //{
                //    IFeatureClass destinationClass = rel.DestinationClass as IFeatureClass;

                //    if (destinationClass.AliasName.ToUpper() == "EDGIS.FUSEANNO" || destinationClass.AliasName.ToUpper() == "EDGIS.FUSE50ANNO")
                //    {
                //        object relatedObject = null;
                //        ISet relatedObjects = rel.GetObjectsRelatedToObject(feature);
                //        relatedObject = relatedObjects.Next();
                //        for (relatedObject = relatedObjects.Next(); relatedObject != null; relatedObject = relatedObjects.Next())
                //         {
                //            IAnnotationFeature2 annoFeature = relatedObject as IAnnotationFeature2;
                //            if (annoFeature == null) return;
                //            IElement element = annoFeature.Annotation;
                //            ITextElement tElement = element as ITextElement;
                //            //cast IElement object to ISymbolCollectionElement
                //            ISymbolCollectionElement sce = element as ISymbolCollectionElement;
                //            if (sce != null && annoFeature.AnnotationClassID == 1)
                //            {
                //                UpdateAnno(annoFeature as IFeature, sAnnoText);
                //                break;
                //            }
                //        }
                feature.set_Value(feature.Fields.FindField("LABELTEXT"), sAnnoText);
                //feature.Store();
                feature.RefreshAnnotation();
                //    }
                //}
            }
            catch (Exception ex)
            { }

        }

        private void UpdateAnno(IFeature feature, String sAnnoText)
        {
            //set annotation property and its alignment and store it
            IAnnotationFeature annoFeature = feature as IAnnotationFeature;
            //get AnnotationFeature.Annotation as IElement
            IElement element = annoFeature.Annotation;

            ITextElement tElement = element as ITextElement;
            //cast IElement object to ISymbolCollectionElement
            ISymbolCollectionElement sce = element as ISymbolCollectionElement;
            if (sce != null)
            {
                sce.Text = sAnnoText;
                annoFeature.Annotation = sce as IElement;
                //cast AnnotationFeature to IFeature
                feature = annoFeature as IFeature;
                feature.set_Value(feature.Fields.FindField("TEXTSTRING"), sAnnoText);
                feature.Store();
            }
        }

        private int IField(IFields iFields)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the domain description from the field value of the object
        /// </summary>
        /// <param name="iObj">Reference to the IObject</param>
        /// <param name="field">Reference to Field to retrieve the domain description for the field value</param>
        /// <returns>Returns the domain descriptions for the field value</returns>
        private string GetValueFromDomain(IObject iObj, IField field)
        {
            string valueFromDomain = null;

            try
            {
                if (iObj == null || field == null)
                {
                    return valueFromDomain;
                }

                //Get the field value
                valueFromDomain = iObj.get_Value(iObj.Fields.FindField(field.Name)).ToString();

                //Get the domain attached to the field
                IDomain domain = field.Domain;
                if (domain == null)
                {
                    return valueFromDomain;
                }

                ICodedValueDomain codeValueDomain = domain as ICodedValueDomain;
                if (codeValueDomain == null)
                {
                    return valueFromDomain;
                }

                int codeCount = codeValueDomain.CodeCount;
                if (codeCount < 1)
                {
                    return valueFromDomain;
                }

                //Get domain description matching domain code
                for (int i = 0; i < codeCount; i++)
                {
                    if (codeValueDomain.get_Value(i).ToString() == valueFromDomain)
                    {
                        valueFromDomain = codeValueDomain.get_Name(i);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to retrieve Domain Descritpion from Value.", ex);
            }

            return valueFromDomain;
        }
               

        #endregion Private Methods
    }
}
