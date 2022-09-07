using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using log4net;
using ESRI.ArcGIS.Geodatabase;
using System.Reflection;
using Miner.ComCategories;
using Miner.Interop;
using PGE.Common.Delivery.Diagnostics;
using System.Collections.Generic;
using ESRI.ArcGIS.esriSystem;
using PGE.Desktop.EDER.UFM;

namespace PGE.Desktop.EDER.AutoUpdaters.LabelText
{
    /// <summary>
    /// Builds label text for Secondary UG conductors.
    /// </summary>
    [Guid("4A22F22B-D703-4329-8509-4594C24F704C")]
    [ProgId("PGE.Desktop.EDER.SecUGConductorLabel")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    public class SecUGConductorLabel : BaseLabelTextAU
    {
        #region Class Variables
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        private const string _phaseDesig = "PhaseDesignation";
        private const string _conductorSize = "ConductorSize";
        private const string _material = "Material";
        private const string _condudctorType = "ConductorType";
        private const string _condudctorCount = "ConductorCount";
        private const string _operatingVoltage = "OperatingVoltage";
        private const string _jointTrenchIdc = "JointTrenchIdc";
        private const string _insulation = "Insulation";
        public static int ConduitBeingUnrelated = -1;
        #endregion
        private enum Subtype
        {
            None,
            SinglePhaseSecondaryUnderground,
            ThreePhaseSecondaryUnderground,
            ServiceSecondaryUnderground,
            StreetlightSecondaryUnderground,
            SecondaryTie,
            PseudoService,
            BusDuctService
        }

        /// <summary>
        /// Constructor, pass in name of this AU.
        /// </summary>
        public SecUGConductorLabel()
            : base("PGE SecUGConductor LabelText AU", 3)
        {
        }

        private string LabelText1 = "";
        private string LabelText2 = "";
        private string LabelText3 = "";

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
            var subtype = obj.SubtypeCodeAsEnum<Subtype>();
            if (subtype == Subtype.None)
            {
                return null;
            }

            string returnText = "";
            string sFiberIDC = FindRelatedFeature_FiberIdc(obj as IFeature);
            if (labelIndex == 0)
            {
                LabelText1 = BuildLabelText(obj);

                if (sFiberIDC == "Y")
                {

                    LabelText1 += sFiberIDC == "Y" && !LabelText1.EndsWith("w/ FO") ? " w/ FO" : string.Empty;
                }
                returnText = LabelText1;
                ////DA Item 52 - ME Release
                //IEnumRelationshipClass relClasses = obj.Class.get_RelationshipClasses(esriRelRole.esriRelRoleAny);
                //relClasses.Reset();
                //IRelationshipClass relClass = null;
                //while ((relClass = relClasses.Next()) != null)
                //{
                //    if (relClass.DestinationClass == obj.Class)
                //    {
                //        if (ModelNameManager.ContainsClassModelName(relClass.OriginClass, SchemaInfo.Electric.ClassModelNames.PGEConduitSystem))
                //        {
                //            break;
                //        }
                //    }
                //    else
                //    {
                //        if (ModelNameManager.ContainsClassModelName(relClass.DestinationClass, SchemaInfo.Electric.ClassModelNames.PGEConduitSystem))
                //        {
                //            break;
                //        }
                //    }
                //}
              
                //ISet relatedConduits = relClass.GetObjectsRelatedToObject(obj);
                //relatedConduits.Reset();
                //IObject conduitSystemObject = null;
                ////IEnumerable<IObject> relatedObjects = GetRelatedObjects(obj, "", esriRelRole.esriRelRoleAny, SchemaInfo.Electric.ClassModelNames.PGEConduitSystem);
                //while ((conduitSystemObject = relatedConduits.Next() as IObject) != null)
                //{
                //    if (conduitSystemObject.OID == ConduitBeingUnrelated) { continue; }
                //    // sFiberIDC = Convert.ToString(conduitSystemObject.get_Value(conduitSystemObject.Fields.FindField("FIBERIDC")));
                //    // returnText += sFiberIDC == "Y" ? " w/ FO" : string.Empty;
                //}

            }
            else if (labelIndex == 1)
            {
                GetLabelText2(obj);
                  
                  if (sFiberIDC == "Y")
                  {
                      LabelText2 = sFiberIDC == "Y" && !LabelText2.EndsWith("w/ FO") ? " w/ FO" : string.Empty;
                  }
                returnText = LabelText2;
            }
            else if (labelIndex == 2)
            {
                GetLabelText3(obj);
                LabelText3 = LabelText3.Replace("w/ FO", "");
                if (sFiberIDC == "Y")  
                {
                    LabelText3 += sFiberIDC == "Y" && !LabelText3.EndsWith("w/ FO") ? "w/ FO" : string.Empty;
                }
                
                returnText = LabelText3;
            }

            return returnText;
        }
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
                                if (pRelatedObj is IFeature)
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


        /// <summary>
        /// Actually builds the label text.
        /// </summary>
        /// <param name="obj"> The object that triggered the event. </param>
        /// <returns> Label Text. </returns>
        public string BuildLabelText(IObject obj)
        {
            var label = new StringBuilder();
            //get related records 
            //var conductorInfoObjs = obj.GetRelatedObjects(null, esriRelRole.esriRelRoleOrigin, SchemaInfo.General.ClassModelNames.LabelTextUnit);
            var conductorInfoObjs = base.GetRelatedObjects(obj, null, esriRelRole.esriRelRoleOrigin, SchemaInfo.General.ClassModelNames.LabelTextUnit);
            //get field value from the field and add to delegate
            Func<IObject, bool> condition;
            condition = o => o.GetFieldValue(_phaseDesig, false).Convert<int>() <= 7;
            //get field value from the field and add to delegate
            Func<IObject, string> key = delegate(IObject o)
            {
                string keyString = string.Empty;

                string conductorSize = o.GetFieldValue(_conductorSize, false).Convert<string>(string.Empty);
                if (!conductorSize.ToUpper().Equals("UNK") && !conductorSize.ToUpper().Equals("NONE"))
                {
                    keyString = conductorSize;
                }

                string material = o.GetFieldValue(_material, false).Convert<string>(string.Empty);
                if (!material.ToUpper().Equals("UNK") && !material.ToUpper().Equals("NONE") && !material.ToUpper().Equals("BUSDUCT"))
                {
                    keyString += material;
                }

                string insulation = o.GetFieldValue(_insulation, false).Convert<string>(string.Empty);
                if (!insulation.ToUpper().Equals("UNK") && !insulation.ToUpper().Equals("NONE") && !insulation.ToUpper().Equals("OTH") && !insulation.Equals(string.Empty))
                {
                    keyString += " " + insulation;
                }

                string conductorType = o.GetFieldValue(_condudctorType, false).Convert<string>(string.Empty);
                if (!conductorType.ToUpper().Equals("SW") && !conductorType.Equals(string.Empty))
                {
                    keyString += " " + conductorType;
                }

                // If the conductor has its dead phase flag set, append phi
                string deadPhase = o.GetFieldValue(null, false, SchemaInfo.General.FieldModelNames.DeadPhase).Convert<string>(string.Empty);
                if (deadPhase.ToUpper().Equals("Y") == true)
                {
                    // Appened Dead ɸ
                    keyString += " DEAD " + Convert.ToChar(632);
                }

                return keyString;
            };
            //key = o => o.GetFieldValue(_conductorSize,false).Convert<string>(string.Empty) +
            //    o.GetFieldValue(_material,false).Convert<string>(string.Empty) +
            //    " " + o.GetFieldValue(_insulation,false).Convert<string>(string.Empty) +
            //    " " + o.GetFieldValue(_condudctorType,false).Convert<string>(string.Empty);

            var groups = conductorInfoObjs.Where(condition).GroupBy(key);
            foreach (var g in groups)
            {
                if (label.Length > 0)
                {
                    label.Append(" & ");
                }

                int count = g.Sum<IObject>(o => o.GetFieldValue(_condudctorCount, false).Convert<int>());
                //********INC000003832405 *******
                if (count > 0 && g.Key.ToString().Trim() != "")
                {
                    label.Append(count);
                    label.Append("-");
                }

                if (g.Key.ToString().Trim() != "")
                    label.Append(g.Key);
            }

            // neutral
            //get value for phase desination
            condition = o => o.GetFieldValue(_phaseDesig, false).Convert<int>() > 7;
            _logger.Debug(_phaseDesig + " : " + condition);
            groups = conductorInfoObjs.Where(condition).GroupBy(key);
            foreach (var g in groups)
            {
                if (label.Length > 0)
                {
                    label.Append(" & ");
                }

                int count = g.Sum<IObject>(o => o.GetFieldValue(_condudctorCount, false).Convert<int>());
                if (count > 0 && g.Key.ToString().Trim() != "")
                {
                    label.Append(count);
                    label.Append("-");
                }

                if (g.Key.ToString().Trim() != "")
                    label.Append(g.Key);
            }

            //To remove the last char "& "

            if (label.ToString().Trim() != "")
            {
                if (label.ToString().Trim().EndsWith("&"))
                {
                    label.Remove(label.ToString().Length - 3, 3);
                }
            }

            var subtype = obj.SubtypeCodeAsEnum<Subtype>();
            switch (subtype)
            {
                //case Subtype.SinglePhaseSecondaryUnderground:
                //case Subtype.ThreePhaseSecondaryUnderground:
                case Subtype.StreetlightSecondaryUnderground:
                    {
                        //******INC000003832405 - SSL is being annotated on streetlight conductor******
                        if (subtype == Subtype.StreetlightSecondaryUnderground)
                        {
                            // Add voltage if its at a primary level
                            string voltage = obj.GetFieldValue(null, true, SchemaInfo.Electric.FieldModelNames.OperatingVoltage).Convert<string>(string.Empty);
                            if (voltage.Equals("4000") == true)
                            {
                                label.Append(" 4kV");
                            }
                            else if (voltage.Equals("5000") == true)
                            {
                                label.Append(" 5kV");
                            }
                            //else if (voltage.Equals(string.Empty) == false)
                            //{
                            //    label.Append(" " + voltage);
                            //}

                            // Add SSL if needed
                            var seriesSL = obj.GetFieldValue("SeriesSLIdc", false).Convert<string>(string.Empty);
                            if (seriesSL.Equals("Y", StringComparison.InvariantCultureIgnoreCase))
                            {

                                if (label.ToString().Trim() != "")
                                    label.Append(" SSL");
                            }
                        }

                        break;
                    }
                //sanjeev as per bug 843
                //case Subtype.ServiceSecondaryUnderground:
                //    label.Append(" SV");
                //    break;
                default:
                    break;
            }
            //get value for jointTrench
            var jointTrench = obj.GetFieldValue(_jointTrenchIdc, false).Convert<string>(string.Empty);
            _logger.Debug(_jointTrenchIdc + " : " + jointTrench);
            if (jointTrench.Equals("Y", StringComparison.InvariantCultureIgnoreCase))
            {
                label.Append(" ");
                label.Append("JT");
            }

            _logger.Debug("Attribute : " + label.ToString());
            return label.ToString();
        }

        private void GetLabelText2(IObject obj)
        {
            _logger.Debug("Setting conductor cross section anno on " + obj.Class.AliasName + ":" + obj.OID.ToString());
            LabelText2 = XSectionConductorHelper.CalculateXSectionConductorText(obj, LabelText1);
        }

        private void GetLabelText3(IObject obj)
        {
            //Label text 2 will be the label text for the concatenation of (sec UG label text field) + (related conduit system label text)
            //EDGIS.ConduitSystem_SecUG
            List<string> labelTextValues = new List<string>();
            IEnumRelationshipClass relClasses = obj.Class.get_RelationshipClasses(esriRelRole.esriRelRoleAny);
            relClasses.Reset();
            IRelationshipClass relClass = null;
            while ((relClass = relClasses.Next()) != null)
            {
                if (relClass.DestinationClass == obj.Class)
                {
                    if (ModelNameManager.ContainsClassModelName(relClass.OriginClass, SchemaInfo.Electric.ClassModelNames.PGEConduitSystem))
                    {
                        break;
                    }
                }
                else
                {
                    if (ModelNameManager.ContainsClassModelName(relClass.DestinationClass, SchemaInfo.Electric.ClassModelNames.PGEConduitSystem))
                    {
                        break;
                    }
                }
            }
            ISet relatedConduits = relClass.GetObjectsRelatedToObject(obj);
            relatedConduits.Reset();
            IObject conduitSystemObject = null;
            //IEnumerable<IObject> relatedObjects = GetRelatedObjects(obj, "", esriRelRole.esriRelRoleAny, SchemaInfo.Electric.ClassModelNames.PGEConduitSystem);
            while ((conduitSystemObject = relatedConduits.Next() as IObject) != null)
            {
                if (conduitSystemObject.OID == ConduitBeingUnrelated) { continue; }
                object labelTextValue = conduitSystemObject.GetFieldValue(null, false, "LABELTEXT");
                if (labelTextValue != null && !string.IsNullOrEmpty(labelTextValue.ToString())) { labelTextValues.Add(labelTextValue.ToString()); }
            }

            labelTextValues = labelTextValues.Distinct().ToList();
            labelTextValues.Sort();
            string conduitLabels = "";
            for (int i = 0; i < labelTextValues.Count; i++)
            {
                string labelTextConduit = labelTextValues[i];
                if (i == 0) { conduitLabels += labelTextConduit; }
                else { conduitLabels += " & " + labelTextConduit; }
            }
            if (!string.IsNullOrEmpty(LabelText1) && !string.IsNullOrEmpty(conduitLabels)) { LabelText3 = LabelText1 + " " + conduitLabels; }
            else if (!string.IsNullOrEmpty(LabelText1)) { LabelText3 = LabelText1; }
            else if (!string.IsNullOrEmpty(conduitLabels)) { LabelText3 = conduitLabels; }
            else { LabelText3 = ""; }
        }

        protected override string GetLabeltextField(IObject obj, string currentLabelField)
        {
            if (currentLabelField == "LABELTEXT") { return currentLabelField; }
            else if (currentLabelField == "LABELTEXT2") { return SchemaInfo.UFM.FieldModelNames.UlsText; ; }
            else if (currentLabelField == "LABELTEXT3") { return "LABELTEXT3"; }
            else { return currentLabelField; }
        }
    }
}
