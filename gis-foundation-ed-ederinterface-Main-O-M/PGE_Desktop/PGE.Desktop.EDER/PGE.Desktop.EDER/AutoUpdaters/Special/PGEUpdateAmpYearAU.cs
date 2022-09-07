using System;
using System.Reflection;
using System.Runtime.InteropServices;
using Miner.ComCategories;
using Miner.Interop;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Framework;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;

namespace PGE.Desktop.EDER.AutoUpdaters.Special
{
    [Guid("F81CC740-F8B7-41D1-8E0D-B6F135F443D1")]
    [ProgId("PGE.Desktop.EDER.AutoUpdaters.Special.PGEUpdateAmpYearAU")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    [ComVisible(true)]
    public class PGEUpdateAmpYearAU : BaseSpecialAU
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PGEUpdateDeleteLiDARCorrectionAU"/> class.  
        /// </summary>
        public PGEUpdateAmpYearAU() : base("PGE Update AmpYear AU") { }
        #endregion Constructors

        #region ClassVariables

        private static readonly PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        #endregion ClassVariables

        #region Special AU Overrides
        /// <summary>
        /// Enable Autoupdater in case of onDelete/onUpdate event and class contans SupportStucture model name
        /// </summary>
        /// <param name="pObjectClass">The selected object class.</param>
        /// <param name="eEvent">The AU event type.</param>
        /// <returns></returns>
        protected override bool InternalEnabled(IObjectClass pObjectClass, Miner.Interop.mmEditEvent eEvent)
        {
            if ((eEvent == Miner.Interop.mmEditEvent.mmEventFeatureCreate) || (eEvent == Miner.Interop.mmEditEvent.mmEventFeatureUpdate))
            {
                return true;
            }

            return false;
        }

        protected override void InternalExecute(IObject pObject, Miner.Interop.mmAutoUpdaterMode eAUMode, Miner.Interop.mmEditEvent eEvent)
        {
            //AU will run only on FeatureUpdate/ on FeatureDelete events
            if (eEvent == mmEditEvent.mmEventFeatureUpdate || eEvent == mmEditEvent.mmEventFeatureCreate)
            {

                //Transformer
                if (ModelNameFacade.ContainsClassModelName(pObject.Class, SchemaInfo.Electric.ClassModelNames.Transformer))
                {
                    string subtypecd = pObject.get_Value(pObject.Fields.FindField("SUBTYPECD")).ToString();
                    //Subtype - Network
                    if (subtypecd.Equals("5"))
                    {
                        setAmpYearToFeature(pObject);

                        //Getting related transformer Unit to update
                        //IEnumRelationshipClass relClasses = null;
                        //IRelationshipClass relClass = null;
                        //relClasses = pObject.Class.get_RelationshipClasses(esriRelRole.esriRelRoleAny);
                        //relClasses.Reset();
                        //while ((relClass = relClasses.Next()) != null)
                        //{
                        //    if (relClass.DestinationClass is ITable)
                        //    {
                        //        //Transformer Unit
                        //        if ((ModelNameFacade.ContainsAllClassModelNames(relClass.DestinationClass, SchemaInfo.Electric.ClassModelNames.TransformerUnit)))
                        //        {
                        //            ISet relatedFeatures = relClass.GetObjectsRelatedToObject(pObject);
                        //            relatedFeatures.Reset();
                        //            object pRelatedObj = null;
                        //            while ((pRelatedObj = relatedFeatures.Next()) != null)
                        //            {
                        //                IRow pRelatedObjectRow = (IRow)pRelatedObj;
                        //                string subtypeCD = pRelatedObjectRow.get_Value(pRelatedObjectRow.Fields.FindField("SUBTYPECD")).ToString();
                        //                //For SubType Surface Unit
                        //                if (subtypeCD.Equals("3"))
                        //                {
                        //                    setAmpYearToFeature(pRelatedObjectRow as IObject);
                        //                }
                        //            }

                        //        }
                        //    }
                        //}

                    }


                }
                //DistBusBar
                if (ModelNameFacade.ContainsClassModelName(pObject.Class, SchemaInfo.Electric.ClassModelNames.PGEBusBar))
                {
                    setAmpYearToFeature(pObject);

                }
                //Network Protector
                if (ModelNameFacade.ContainsClassModelName(pObject.Class, SchemaInfo.Electric.ClassModelNames.PGENetworkProtector))
                {
                    setAmpYearToFeature(pObject);
                }
                //Neutral Conductor
                if (ModelNameFacade.ContainsClassModelName(pObject.Class, SchemaInfo.Electric.ClassModelNames.PGENeutralConductor))
                {
                    setAmpYearToFeature(pObject);
                }
                //Open Point
                if (ModelNameFacade.ContainsClassModelName(pObject.Class, SchemaInfo.Electric.ClassModelNames.PGEOpenPoint))
                {
                    string subtypecd = pObject.get_Value(pObject.Fields.FindField("SUBTYPECD")).ToString();
                    //Subtype - 
                    if (subtypecd.Equals("4") || subtypecd.Equals("11"))
                    {
                        //Do Nothing
                    }
                    else
                    {
                        setAmpYearToFeature(pObject);
                    }
                }
                //Primary Riser
                if (ModelNameFacade.ContainsClassModelName(pObject.Class, SchemaInfo.Electric.ClassModelNames.PGEPrimaryRiser))
                {
                    setAmpYearToFeature(pObject);
                }
                //PriOH Conductor
                if (ModelNameFacade.ContainsClassModelName(pObject.Class, SchemaInfo.Electric.ClassModelNames.PrimaryOHConductor))
                {
                    setAmpYearToFeature(pObject);

                    //Getting related transformer Unit to update
                    //IEnumRelationshipClass relClasses = null;
                    //IRelationshipClass relClass = null;
                    //relClasses = pObject.Class.get_RelationshipClasses(esriRelRole.esriRelRoleAny);
                    //relClasses.Reset();
                    //while ((relClass = relClasses.Next()) != null)
                    //{
                    //    if (relClass.DestinationClass is ITable)
                    //    {
                    //        //Transformer Unit
                    //        if ((ModelNameFacade.ContainsAllClassModelNames(relClass.DestinationClass, SchemaInfo.Electric.ClassModelNames.PrimaryOHConductorInfo)))
                    //        {
                    //            ISet relatedFeatures = relClass.GetObjectsRelatedToObject(pObject);
                    //            relatedFeatures.Reset();
                    //            object pRelatedObj = null;
                    //            while ((pRelatedObj = relatedFeatures.Next()) != null)
                    //            {
                    //                IRow pRelatedObjectRow = (IRow)pRelatedObj;
                    //                //string subtypeCD = pRelatedObjectRow.get_Value(pRelatedObjectRow.Fields.FindField("SUBTYPECD")).ToString();                                    
                    //                setAmpYearToFeature(pRelatedObjectRow as IObject);

                    //            }

                    //        }
                    //    }
                    //}
                }
                //PriUG Conductor
                if (ModelNameFacade.ContainsClassModelName(pObject.Class, SchemaInfo.Electric.ClassModelNames.PrimaryUGConductor))
                {
                    setAmpYearToFeature(pObject);

                    //Getting related transformer Unit to update
                    //IEnumRelationshipClass relClasses = null;
                    //IRelationshipClass relClass = null;
                    //relClasses = pObject.Class.get_RelationshipClasses(esriRelRole.esriRelRoleAny);
                    //relClasses.Reset();
                    //while ((relClass = relClasses.Next()) != null)
                    //{
                    //    if (relClass.DestinationClass is ITable)
                    //    {
                    //        //Transformer Unit
                    //        if ((ModelNameFacade.ContainsAllClassModelNames(relClass.DestinationClass, SchemaInfo.Electric.ClassModelNames.PrimaryUGConductorInfo)))
                    //        {
                    //            ISet relatedFeatures = relClass.GetObjectsRelatedToObject(pObject);
                    //            relatedFeatures.Reset();
                    //            object pRelatedObj = null;
                    //            while ((pRelatedObj = relatedFeatures.Next()) != null)
                    //            {
                    //                IRow pRelatedObjectRow = (IRow)pRelatedObj;
                    //                //string subtypeCD = pRelatedObjectRow.get_Value(pRelatedObjectRow.Fields.FindField("SUBTYPECD")).ToString();                                    
                    //                setAmpYearToFeature(pRelatedObjectRow as IObject);

                    //            }

                    //        }
                    //    }
                    //}
                }
                //Switch
                if (ModelNameFacade.ContainsClassModelName(pObject.Class, SchemaInfo.Electric.ClassModelNames.Switch))
                {
                    string subtypecd = pObject.get_Value(pObject.Fields.FindField("SUBTYPECD")).ToString();
                    //Subtype - 
                    if (subtypecd.Equals("3") || subtypecd.Equals("5") || subtypecd.Equals("7"))
                    {
                        setAmpYearToFeature(pObject);
                    }
                }
                //Vault Poly
                if (ModelNameFacade.ContainsClassModelName(pObject.Class, SchemaInfo.Electric.ClassModelNames.PGEVaultPoly))
                {
                    setAmpYearToFeature(pObject);
                }
                //TransformerUnit
                if (ModelNameFacade.ContainsClassModelName(pObject.Class, SchemaInfo.Electric.ClassModelNames.TransformerUnit))
                {
                    string subtypecd = pObject.get_Value(pObject.Fields.FindField("SUBTYPECD")).ToString();
                    //For SubType Surface Unit
                    if (subtypecd.Equals("3"))
                    {
                        setAmpYearToFeature(pObject);
                    }
                }
                //PriOHConductorInfo
                if (ModelNameFacade.ContainsClassModelName(pObject.Class, SchemaInfo.Electric.ClassModelNames.PrimaryOHConductorInfo))
                {
                    setAmpYearToFeature(pObject);
                }
                //PriUGConductorInfo
                if (ModelNameFacade.ContainsClassModelName(pObject.Class, SchemaInfo.Electric.ClassModelNames.PrimaryUGConductorInfo))
                {
                    setAmpYearToFeature(pObject);
                }
            }

        }

        #endregion

        #region private methods
        private void setAmpYearToFeature(IObject pObject)
        {
            int indYearManufactured = -1;
            int indInstallationDate = -1;
            int indInstalljobYear = -1;
            int indAmpYear = -1;
            try
            {
                // indInstallationDate = pObject.Fields.FindField(ModelNameFacade.FieldFromModelName((pObject.Class), SchemaInfo.Electric.FieldModelNames.InstallationDate).Name);

                IField fieldInstallationDate = ModelNameFacade.FieldFromModelName((pObject.Class), SchemaInfo.Electric.FieldModelNames.InstallationDate);
                if (fieldInstallationDate != null)
                {
                    indInstallationDate = pObject.Fields.FindField(fieldInstallationDate.Name);
                }
                IField fieldInstallJobYear = ModelNameFacade.FieldFromModelName((pObject.Class), SchemaInfo.Electric.FieldModelNames.InstallationJobYear);
                if (fieldInstallJobYear != null)
                {
                    indInstalljobYear = pObject.Fields.FindField(fieldInstallJobYear.Name);
                }
                
                IField fieldManufacturedYear = ModelNameFacade.FieldFromModelName((pObject.Class), SchemaInfo.Electric.FieldModelNames.ManufacturedYear);
                if (fieldManufacturedYear != null)
                {
                    indYearManufactured = pObject.Fields.FindField(fieldManufacturedYear.Name);
                }

                
                IField fieldIndAmpYear = ModelNameFacade.FieldFromModelName((pObject.Class), SchemaInfo.Electric.FieldModelNames.AMPYear);
                if (fieldIndAmpYear != null)
                {
                    indAmpYear = pObject.Fields.FindField(fieldIndAmpYear.Name);
                }
                object installtionDate = null;
                string installJobYear = string.Empty;
                string yearManufactured = string.Empty;

                int yearFromInstallationDate;
                int yearfromInstallJobYear;
                int yaerFromYearManufactured;

                if (indInstallationDate != -1)
                {
                    installtionDate = pObject.get_Value(indInstallationDate);

                }
                if (indInstalljobYear != -1)
                {
                    installJobYear = pObject.get_Value(indInstalljobYear).ToString();

                }
                if (indYearManufactured != -1)
                {
                    yearManufactured = pObject.get_Value(indYearManufactured).ToString();

                }

                if ((installtionDate != null) && (!string.IsNullOrEmpty(installtionDate.ToString())))
                {
                    //if (!string.IsNullOrEmpty(installtionDate.ToString()))
                    //{
                    //get the year from Installation date
                    yearFromInstallationDate = ((System.DateTime)(installtionDate)).Year;
                    pObject.set_Value(indAmpYear, yearFromInstallationDate);
                    //}
                }
                else
                {
                    if (!string.IsNullOrEmpty(installJobYear))
                    {
                        yearfromInstallJobYear = Convert.ToInt16(installJobYear);
                        pObject.set_Value(indAmpYear, yearfromInstallJobYear);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(yearManufactured))
                        {
                            yaerFromYearManufactured = Convert.ToInt16(yearManufactured);
                            pObject.set_Value(indAmpYear, yaerFromYearManufactured);
                        }
                    }
                }
            }
            catch (Exception ex) { }


        }
       

        #endregion
    }
}