using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using log4net;
using ESRI.ArcGIS.Geodatabase;
using System.Reflection;
using Miner.ComCategories;
using Miner.Interop;
using PGE.Common.Delivery.Diagnostics;
using PGE.Desktop.EDER.UFM;
using ESRI.ArcGIS.esriSystem;

namespace PGE.Desktop.EDER.AutoUpdaters.LabelText
{
    [Guid("1a34cc89-3956-4aa7-8dda-9cb3331ba385")]    
    [ProgId("PGE.Desktop.EDER.00AutoUpdaters.LabelText.NeutralConductorLabel")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    public class NeutralConductorLabel : BaseLabelTextAU
    {
        #region Class Variables
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        //private const string _phaseDesig = "PhaseDesignation";
        private const string _conductorSize = "ConductorSize";
        private const string _material = "Material";
        //private const string _condudctorType = "ConductorType";
        private const string _condudctorCount = "ConductorCount";
        //private const string _operatingVoltage = "OperatingVoltage";
        //private const string _seriesSLIdc = "SeriesSLIdc";
        private const string _conductorUse = "ConductorUse";

        private string labelText1;
        private string labelText2;

        #endregion
        private enum Subtype
        {
            None = 0,
            Overhead = 1,
            Underground = 2
        }

        /// <summary>
        /// Constructor, pass in name of this AU.
        /// </summary>
        public NeutralConductorLabel()
            : base("PGE NeutralConductor LabelText AU", 2)
        {
        }

        /// <summary>
        ///   Gets the label text given the specific object.
        /// </summary>
        /// <param name="obj"> The object that triggered the event. </param>
        /// <param name="autoUpdaterMode"> The auto updater mode. </param>
        /// <param name="editEvent"> The edit event. </param>
        /// <param name="labelIndex">The index of the label that should be returned.</param>
        /// <returns> The label text that will be stored on the field assigned the <see
        ///    cref="SchemaInfo.General.FieldModelNames.LabelText" /> field model name, or null if no text should be assigned. </returns>
        protected override string GetLabelText(IObject obj, mmAutoUpdaterMode autoUpdaterMode, mmEditEvent editEvent, int labelIndex)
        {
            string label = string.Empty;

            var subtype = obj.SubtypeCodeAsEnum<Subtype>();
            if (subtype == Subtype.None)
            {
                return null;
            }

            //DA Item - 52

            string returnText = "";
            string sFiberIDC = FindRelatedFeature_FiberIdc(obj as IFeature);

            if (labelIndex == 0)
            {
                BuildLabelText(obj);
                if (sFiberIDC == "Y")
                {

                    labelText1 = labelText1 + " w/ FO";
                }
                else
                {
                    labelText1 = labelText1.Replace("w/ FO", "");
                }
                label = labelText1;
            }
            else
            {
                if (sFiberIDC == "Y")
                {

                    labelText2 = labelText2 + " w/ FO";
                }
                else
                {
                    labelText2 = labelText2.Replace("w/ FO", "");
                }

                label = labelText2;
            }

            return label;
        }

        /// <summary>
        /// Actually builds the label text.
        /// </summary>
        /// <param name="obj"> The object that triggered the event. </param>
        /// <returns> Label Text. </returns>
        private void BuildLabelText(IObject obj)
        {
            var label = new StringBuilder();
            
            var conductorInfoObjs = obj;

            // if conductorsize is unk or none then do not include in annotation.
            string conductorSize = obj.GetFieldValue(_conductorSize, false).Convert<string>(string.Empty);
            if (!conductorSize.ToUpper().Equals("UNK") && !conductorSize.ToUpper().Equals("NONE"))
            {
                label.Append(conductorSize);
            }
            _logger.Debug(_conductorSize + " : " + conductorSize);
            // if material is unk or none then do not include in annotation.
            string material = obj.GetFieldValue(_material, false).Convert<string>(string.Empty);
            if (!material.ToUpper().Equals("UNK") && !material.ToUpper().Equals("NONE"))
            {
                label.Append(material);
            }
            _logger.Debug(_material + " : " + material);
            // Get conductor Use
            var conductorUse = obj.GetFieldValue(_conductorUse, false).Convert<int>();
            string conductorUseAbbreviation = LookupConductorUseAbbreviation(conductorUse.Convert<int>());
            {
                label.Append(conductorUseAbbreviation);
            }
            _logger.Debug(_conductorUse + " : " + conductorUse);
            // checking If there is any combination of conductor size, material and conductor use
            if(label.ToString().Trim() != "")
            {
                int CounductorCount = obj.GetFieldValue(_condudctorCount, false).Convert<int>(0);
                if (CounductorCount != 0)
                {
                    label.Insert(0, CounductorCount + "-"); 
                }
            }
           _logger.Debug("Attribute : " + label.ToString());
            labelText1 = label.ToString();
            SetLabelText2(obj);
        }

        private string LookupConductorUseAbbreviation(int CounductorUseCode)
        {
            switch (CounductorUseCode)
            {
                case 2:
                    return "(PN)";
                case 3:
                    return "(CN)";
                case 9:
                    return "(RCN)";
                default:
                    return "";
            }
        }

        private void SetLabelText2(IObject obj)
        {
            _logger.Debug("Setting conductor cross section anno on " + obj.Class.AliasName + ":" + obj.OID.ToString());
            labelText2 = XSectionConductorHelper.CalculateXSectionConductorText(obj, labelText1);
        }

        //DA Item 52

        public string FindRelatedFeature_FiberIdc(IFeature pFeature)
        {
            IObjectClass objectClass = null;
            IEnumRelationshipClass relClasses = null;
            IRelationshipClass relClass = null;
            IFeature pFeat = null;
            string sFiberIDC = null;
            bool blConduitFound = false;
            try
            {
                objectClass = pFeature.Class;
                relClasses = objectClass.get_RelationshipClasses(esriRelRole.esriRelRoleAny);
                relClasses.Reset();
                relClass = relClasses.Next();
                while (relClass != null)
                {
                    blConduitFound = false;
                    if (ModelNameManager.ContainsClassModelName(relClass.OriginClass, SchemaInfo.Electric.ClassModelNames.PGEConduitSystem))
                    {
                        blConduitFound = true;
                    }
                    else if (ModelNameManager.ContainsClassModelName(relClass.DestinationClass, SchemaInfo.Electric.ClassModelNames.PGEConduitSystem))
                    {
                        blConduitFound = true;
                    }

                    if (blConduitFound == true)
                    {
                        //if (ModelNameManager.ContainsClassModelName(relClass.DestinationClass, SchemaInfo.Electric.ClassModelNames.PGEConduitSystem))
                        //{
                        ISet relatedFeatures = relClass.GetObjectsRelatedToObject(pFeature);
                        object pRelatedObj = relatedFeatures.Next();
                        while (pRelatedObj != null)
                        {
                            pFeat = null;
                            sFiberIDC = null;
                            try
                            {
                                pFeat = (IFeature)pRelatedObj;

                                if (pFeat.Fields.FindField("FIBERIDC") != -1)
                                {
                                    sFiberIDC = pFeat.get_Value(pFeat.Fields.FindField("FIBERIDC")).ToString();

                                    if (sFiberIDC == "Y")
                                    {
                                        return sFiberIDC;

                                    }
                                }
                            }
                            catch { }
                            pRelatedObj = relatedFeatures.Next();
                        }
                        //break;
                        //}
                    }

                    relClass = relClasses.Next();
                }
                return "N";
            }
            catch (Exception ex)
            {
                return "N";
            }

        }
    }
}