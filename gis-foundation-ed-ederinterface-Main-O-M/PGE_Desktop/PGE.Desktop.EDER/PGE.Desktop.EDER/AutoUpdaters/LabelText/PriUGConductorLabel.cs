using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Reflection;
using log4net;
using ESRI.ArcGIS.Geodatabase;
using Miner.ComCategories;
using Miner.Interop;
using System.Collections.Generic;
using PGE.Common.Delivery.Diagnostics;
using ESRI.ArcGIS.esriSystem;
using PGE.Desktop.EDER.UFM;

namespace PGE.Desktop.EDER.AutoUpdaters.LabelText
{
    /// <summary>
    /// Builds label text and lableText2 for Primary UG conductors.
    /// </summary>
    [Guid("080AB0E1-E84B-476A-B0DD-60D0BE580FAA")]
    [ProgId("PGE.Desktop.EDER.PriUGConductorLabel")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    public class PriUGConductorLabel : BaseLabelTextAU
    {
        #region Class Variables
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        private const string _phaseDesignation = "PhaseDesignation";
        private const string _conductorSize = "ConductorSize";
        private const string _material = "Material";
        private const string _insulation = "Insulation";
        private const string _rating = "Rating";
        private const string _condudctorCount = "ConductorCount";
        private const string _operatingVoltage = "NominalVoltage";
        private const string _jointTrench = "JointTrenchIdc";
        private const string _serviceIdc = "ServiceIdc";
        private const string _conductorTypeValue = " TPX";
        private const string _conductorValueQpx = " QPX";
        private const string ConductorUse = "ConductorUse";

        //Static int variable to determine whether a conduit is currently be deleted from relationship.  Necessary
        //so that this conduit is not included in the labeltext4 generation.
        public static int ConduitBeingUnrelated = -1;
        #endregion

        private enum Subtype
        {
            None,
            SinglePhasePrimaryUnderground,
            TwoPhasePrimaryUnderground,
            ThreePhasePrimaryUnderground,
        }

        private string labelText1;
        private string labelText2;
        private string labelText3;
        private string labelText4;

        private string sFiberIDC = "N";   //DA Item 52 - ME Release

        /// <summary>
        /// Constructor, pass in name and the number of label texts this AU will build.
        /// </summary>
        public PriUGConductorLabel()
            : base("PGE PriUGConductor LabelText AU", 4)
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
            var subtype = obj.SubtypeCodeAsEnum<Subtype>();
            if (subtype == Subtype.None)
            {
                return null;
            }

            switch (labelIndex)
            {
                case 0:
                    string labelText;
                    BuildLabelTexts(obj, out labelText, out labelText2);
                    labelText1 = labelText;
                    //get sFiberIDC value
                    sFiberIDC = FindRelatedFeature_FiberIdc(obj as IFeature);
                    labelText += sFiberIDC == "Y" && !labelText.EndsWith("w/ FO") ? " w/ FO" : string.Empty;   //DA Item 52 - ME Release
                    _logger.Debug("labelText :" + labelText);
                    return labelText;
                case 1:
                    sFiberIDC = FindRelatedFeature_FiberIdc(obj as IFeature);
                    labelText2 += sFiberIDC == "Y" && !labelText2.EndsWith("w/ FO") ? " w/ FO" : string.Empty;
                    _logger.Debug("labelText2" + labelText2);
                    return labelText2;
                case 2:
                    sFiberIDC = FindRelatedFeature_FiberIdc(obj as IFeature);
                    labelText3 += sFiberIDC == "Y" && !labelText3.EndsWith("w/ FO") ? " w/ FO" : string.Empty;
                    _logger.Debug("labelText3" + labelText3);
                    return labelText3;
                case 3:
                    sFiberIDC = FindRelatedFeature_FiberIdc(obj as IFeature);
                    labelText4 += sFiberIDC == "Y" && !labelText4.EndsWith("w/ FO") ? " w/ FO" : string.Empty;
                    _logger.Debug("labelText4" + labelText4);
                    return labelText4;
            }
            return String.Empty;
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
        /// Actually builds label texts.
        /// </summary>
        /// <param name="obj"> The object that triggered the event. </param>
        /// <param name="labelText"> Label Text. </param>
        /// <param name="labelText2">Label Text 2. </param>
        public void BuildLabelTexts(IObject obj, out string labelText, out string labelText2)
        {
            var label = new StringBuilder();
            //get related object
            var conductorInfoObjs = base.GetRelatedObjects(obj, null, esriRelRole.esriRelRoleOrigin, SchemaInfo.General.ClassModelNames.LabelTextUnit);



            // filter out the conductor infor where PhaseDesignation <= 7 and ConductorUse is not Parallel
            Func<IObject, bool> condition;
            condition = o => o.GetFieldValue(_phaseDesignation, false).Convert<int>() <= 7 && o.GetFieldValue(ConductorUse, false).Convert<int>() != 4;
            Func<IObject, string> key;
            //get field value from the field and add to delegate
            key = delegate(IObject o)
            {
                string keyString = string.Empty;
                //var subtype = obj.SubtypeCodeAsEnum<Subtype>();
                string conductorSize = o.GetFieldValue(_conductorSize, false).Convert<string>(string.Empty);
                string material = o.GetFieldValue(_material, false).Convert<string>(string.Empty);

                if (!conductorSize.ToUpper().Equals("UNK") && !conductorSize.ToUpper().Equals("NONE"))
                {
                    keyString = conductorSize;
                }
                if (!material.ToUpper().Equals("UNK") && !material.ToUpper().Equals("NONE"))
                {
                    keyString += material;
                }

                string insulation = o.GetFieldValue(_insulation, false).Convert<string>(string.Empty);// +" (" + o.GetFieldValue(_rating, true).Convert<string>(string.Empty) + ")";
                if (insulation.ToUpper().Equals("OTH") || insulation.ToUpper().Equals("UNK")) { insulation = ""; }

                string conductoTypeVal = o.GetFieldValue(null, false, SchemaInfo.Electric.FieldModelNames.ConductorType)
                                                       .Convert<string>()
                                                       .Contains("TPX", StringComparison.InvariantCultureIgnoreCase)
                                                    ? _conductorTypeValue
                                                    : string.Empty;
                if (string.IsNullOrEmpty(conductoTypeVal))
                {
                    conductoTypeVal = o.GetFieldValue(null, false, SchemaInfo.Electric.FieldModelNames.ConductorType)
                                                       .Convert<string>()
                                                       .Contains("QPX", StringComparison.InvariantCultureIgnoreCase)
                                                    ? _conductorValueQpx
                                                    : string.Empty;
                }

                if (!string.IsNullOrEmpty(conductoTypeVal))
                {
                    keyString += " " + insulation + conductoTypeVal;
                }
                else if (!string.IsNullOrEmpty(insulation))
                {
                    keyString += " " + insulation;
                }

                return keyString;
            };

            var groups = conductorInfoObjs.Where(condition).GroupBy(key);
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
                //label.Append(count);
                //label.Append("-");
                label.Append(g.Key);
            }

            //get the conductorInfo where Conductor use is Parallel - INC000004050447

            var conductorInfoObjs_PAR = conductorInfoObjs.Where(u => u.GetFieldValue(_phaseDesignation, false).Convert<int>() <= 7 && u.GetFieldValue(ConductorUse, false).Convert<int>() == 4);

            // Calculate each conductor info for Parallel - INC000004050447
            foreach (var o in conductorInfoObjs_PAR)
            {
                var count = o.GetFieldValue(_condudctorCount, false).Convert<int>();
                if (label.Length > 0)
                {
                    label.Append(" & ");
                }
                if (count > 0)
                {
                    label.Append(count);
                    label.Append("-");
                }
                var conductorSize = o.GetFieldValue(_conductorSize, false).Convert<string>(string.Empty);
                if (!conductorSize.ToUpper().Equals("UNK") && !conductorSize.ToUpper().Equals("NONE"))
                {
                    label.Append(conductorSize);
                }
                string material = o.GetFieldValue(_material, false).Convert<string>(string.Empty);
                if (!material.ToUpper().Equals("UNK") && !material.ToUpper().Equals("NONE"))
                {
                    label.Append(material);
                }
                string conductorUse = o.GetFieldValue(ConductorUse, false).Convert<string>(string.Empty);
                if (conductorUse != null)
                {
                    string conductorUseAbbreviation = FilterConductorUseAbbreviation(conductorUse.Convert<int>());
                    if (!String.IsNullOrEmpty(conductorUseAbbreviation))
                    {
                        if (conductorUseAbbreviation.Trim() == "PAR")
                            label.Append(" (" + conductorUseAbbreviation.Trim() + ")");
                    }
                }

                string insulation = o.GetFieldValue(_insulation, false).Convert<string>(string.Empty);// +" (" + o.GetFieldValue(_rating, true).Convert<string>(string.Empty) + ")";
                if (insulation.ToUpper().Equals("OTH") || insulation.ToUpper().Equals("UNK")) { insulation = ""; }

                string conductoTypeVal = o.GetFieldValue(null, false, SchemaInfo.Electric.FieldModelNames.ConductorType)
                                                       .Convert<string>()
                                                       .Contains("TPX", StringComparison.InvariantCultureIgnoreCase)
                                                    ? _conductorTypeValue
                                                    : string.Empty;
                if (string.IsNullOrEmpty(conductoTypeVal))
                {
                    conductoTypeVal = o.GetFieldValue(null, false, SchemaInfo.Electric.FieldModelNames.ConductorType)
                                                       .Convert<string>()
                                                       .Contains("QPX", StringComparison.InvariantCultureIgnoreCase)
                                                    ? _conductorValueQpx
                                                    : string.Empty;
                }

                if (!string.IsNullOrEmpty(conductoTypeVal))
                {
                    label.Append(" " + insulation + conductoTypeVal);
                }
                else if (!string.IsNullOrEmpty(insulation))
                {
                    label.Append(" " + insulation);
                }

            }

            // neutral
            condition = o => o.GetFieldValue(_phaseDesignation, false).Convert<int>() > 7;

            //sanjeev change as per bug 843
            key = delegate(IObject o)
            {
                string keyString = string.Empty;
                string conductorSize = o.GetFieldValue(_conductorSize, false).Convert<string>(string.Empty);
                string material = o.GetFieldValue(_material, false).Convert<string>(string.Empty);

                if (!conductorSize.ToUpper().Equals("UNK") && !conductorSize.ToUpper().Equals("NONE"))
                {
                    keyString = conductorSize;
                }
                if (!material.ToUpper().Equals("UNK") && !material.ToUpper().Equals("NONE"))
                {
                    keyString += material;
                }

                string insulation = o.GetFieldValue(_insulation, false).Convert<string>(string.Empty);
                if (!string.IsNullOrEmpty(insulation) && !insulation.ToUpper().Equals("OTH") && !insulation.ToUpper().Equals("UNK"))
                {
                    keyString += " " + insulation;
                }

                if (o.GetFieldValue(_phaseDesignation, false).Convert<string>(string.Empty) != "12") //UNKNOWN
                {
                    keyString += " (" + o.GetFieldValue(_phaseDesignation, true).Convert<string>(string.Empty) + ")";
                }

                //keyString += " (" + o.GetFieldValue(_rating, true).Convert<string>(string.Empty) + ")"; //Removed per Robert's comments 7/3/2013
                string conductoTypeVal = o.GetFieldValue(null, false, SchemaInfo.Electric.FieldModelNames.ConductorType)
                                                           .Convert<string>()
                                                           .Contains("TPX", StringComparison.InvariantCultureIgnoreCase)
                                                        ? _conductorTypeValue
                                                        : string.Empty;
                if (string.IsNullOrEmpty(conductoTypeVal))
                {
                    conductoTypeVal = o.GetFieldValue(null, false, SchemaInfo.Electric.FieldModelNames.ConductorType)
                                                       .Convert<string>()
                                                       .Contains("QPX", StringComparison.InvariantCultureIgnoreCase)
                                                    ? _conductorValueQpx
                                                    : string.Empty;
                }
                if (!string.IsNullOrEmpty(conductoTypeVal))
                {
                    keyString += conductoTypeVal;
                }

                ////Subhankar
                ////Bug#20033 -- Phase Designation of Neutral conductor should be next to Conductor Type 
                //if (o.GetFieldValue(_phaseDesignation, false).Convert<string>(string.Empty) != "12") //UNKNOWN
                //{
                //    keyString += " (" + o.GetFieldValue(_phaseDesignation, true).Convert<string>(string.Empty) + ")";
                //}

                return keyString;

            };

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
                label.Append(g.Key);
            }
            //get value for operaing voltage
            var operatingVoltage = obj.GetFieldValue(_operatingVoltage, true);
            _logger.Debug(_operatingVoltage + " : " + operatingVoltage);
            label.Append(" ");
            label.Append(operatingVoltage);
            //get value for joint trench
            var jointTrench = obj.GetFieldValue(_jointTrench, false).Convert<string>(string.Empty);
            _logger.Debug(_jointTrench + " : " + jointTrench);
            if (jointTrench.Equals("Y", StringComparison.InvariantCultureIgnoreCase))
            {
                label.Append(" ");
                label.Append("JT");
            }

            //get value for service idc
            var serviceIdc = obj.GetFieldValue(_serviceIdc, false).Convert<string>(string.Empty);
            _logger.Debug(_serviceIdc + " : " + serviceIdc);
            if (serviceIdc.Equals("Y", StringComparison.InvariantCultureIgnoreCase))
            {
                label.Append(" ");
                label.Append("SV");
            }

            labelText = label.ToString();
            labelText1 = labelText;
            // label text 2
            label = new StringBuilder();
            // Start of IBM code change for Schematics Anno - 8/22/2013
            SetLabelText2(label, conductorInfoObjs);
            // End of IBM code change for Schematics Anno - 8/22/2013

            labelText2 = label.ToString();

            if (ModelNameManager.ContainsClassModelName(obj.Class, SchemaInfo.Electric.ClassModelNames.PGEPriUGConductor))
            {
                SetLabelText3(obj);
                SetLabelText4(obj);
            }

        }

        private void SetLabelText3(IObject obj)
        {
            _logger.Debug("Setting conductor cross section anno on " + obj.Class.AliasName + ":" + obj.OID.ToString());
            labelText3 = XSectionConductorHelper.CalculateXSectionConductorText(obj, labelText1);
        }

        private void SetLabelText4(IObject obj)
        {
            //Label text 3 will be the label text for the concatenation of (pri UG label text field) + (related conduit system label text)
            //EDGIS.ConduitSystem_PriUG
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

            //var sFiberIDC = "N";
            ISet relatedConduits = relClass.GetObjectsRelatedToObject(obj);
            relatedConduits.Reset();
            IObject conduitSystemObject = null;
            //IEnumerable<IObject> relatedObjects = GetRelatedObjects(obj, "", esriRelRole.esriRelRoleAny, SchemaInfo.Electric.ClassModelNames.PGEConduitSystem);
            while ((conduitSystemObject = relatedConduits.Next() as IObject) != null)
            {
                if (conduitSystemObject.OID == ConduitBeingUnrelated) { continue; }
                object labelTextValue = conduitSystemObject.GetFieldValue(null, false, "LABELTEXT");
                if (labelTextValue != null) { labelTextValues.Add(labelTextValue.ToString()); }
                //if (sFiberIDC == "N") sFiberIDC = Convert.ToString(conduitSystemObject.get_Value(conduitSystemObject.Fields.FindField("FIBERIDC")));   //DA Item 52 - ME Release
            }

            labelTextValues = labelTextValues.Distinct().ToList();
            labelTextValues.Sort();

            //For PriUGs with multiple related conduits it will only show the first one alphabetically. This ensures it will show the
            //smallest size available that isn't spare.
            string conduitLabels = "";
            if (labelTextValues.Count > 0)
            {
                conduitLabels = labelTextValues[0];
            }

            /*
            for (int i = 0; i < labelTextValues.Count; i++)
            {
                string labelTextConduit = labelTextValues[i];
                if (i == 0) { conduitLabels += labelTextConduit; }
                else { conduitLabels += " " + labelTextConduit; }
            }
            */

            //Subhankar
            //Bug#20032 -- if (!string.IsNullOrEmpty(labelText1) && !string.IsNullOrEmpty(conduitLabels)) { labelText4 = labelText1 + " [" + conduitLabels + "]"; }
            //if (!string.IsNullOrEmpty(conduitLabels)) { conduitLabels += sFiberIDC == "Y" ? " w/ FO" : string.Empty; };
           
            if (!string.IsNullOrEmpty(labelText1) && !string.IsNullOrEmpty(conduitLabels)) { labelText4 = labelText1 + " " + conduitLabels; }
            else if (!string.IsNullOrEmpty(labelText1)) { labelText4 = labelText1; }
            else if (!string.IsNullOrEmpty(conduitLabels)) { labelText4 = conduitLabels; }
            else { labelText4 = ""; }
        }

        // Start of IBM code change for Schematics Anno - 8/22/2013
        // make the changes as per eric's comment(INC000004155382) - 1/27/2016
        private void SetLabelText2(StringBuilder label, IEnumerable<IObject> conductorInfoObjs)
        {

            const string ConductorSize = "ConductorSize";
            const string Material = "Material";
            const string Insulation = "Insulation";
            StringBuilder label2 = new StringBuilder();

            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            IList<string> labelParts = new List<string>();

            foreach (var o in conductorInfoObjs)
            {
                StringBuilder conductorInfoLabel = new StringBuilder();
                var size = o.GetFieldValue(ConductorSize, false).Convert<string>(string.Empty);
                if (!size.ToUpper().Equals("UNK") && !size.ToUpper().Equals("NONE"))
                {
                    conductorInfoLabel.Append(size);
                }
                var material = o.GetFieldValue(Material, false).Convert<string>(string.Empty);
                if (!material.ToUpper().Equals("UNK") && !material.ToUpper().Equals("NONE"))
                {
                    conductorInfoLabel.Append(material);
                }
                var conductorUse = o.GetFieldValue(ConductorUse, false).Convert<string>(string.Empty);
                var insulation = o.GetFieldValue(Insulation, false).Convert<string>(string.Empty);


                conductorInfoLabel.Append(FilterInsulationCodedValue(insulation));
                if (conductorUse != null)
                {
                    string conductorUseAbbreviation = FilterConductorUseAbbreviation(conductorUse.Convert<int>());
                    if (!String.IsNullOrEmpty(conductorUseAbbreviation))
                    {
                        conductorInfoLabel.Append(conductorUseAbbreviation);
                    }
                }

                // Rollup rule -- don't include duplicate ConductorInfos
                if (!labelParts.Contains(conductorInfoLabel.ToString()))
                {
                    if (label2.Length > 0)
                    {
                        label2.Append(" & ");
                    }
                    label2.Append(conductorInfoLabel.ToString());
                }
                labelParts.Add(conductorInfoLabel.ToString());
            }
            label2.Replace("P&L", "P~L");

            // rearrange the label text2 as per INC000004155382 [PHASE CONDUCTOR] & [PAR] & [PN] & [CN] & [RCN].

            // split the original label text 2
            var lst = label2.ToString().Split('&').ToList();

            // find the 'RCN' and add them
            GetStringPart(label, lst, "RCN");

            // find the 'CN' and add them
            GetStringPart(label, lst, "CN");

            // find the 'PN' and add them
            GetStringPart(label, lst, "PN");

            // find the 'PAR' and add them
            GetStringPart(label, lst, "PAR");

            // add the PHASE CONDUCTOR
            for (var i = 0; i < lst.Count; i++)
            {
                if (i == lst.Count - 1)
                    label.Insert(0, lst[i].Trim());
                else
                {
                    label.Insert(0, lst[i].Trim());
                    label.Insert(0, " & ");
                }
            }
            label.Replace("P~L", "P&L");

        }

        private static void GetStringPart(StringBuilder label, List<string> lst, string strPart)
        {
            var Label_Part = lst.FindAll(s => s.Contains(strPart));
            for (var i = 0; i < Label_Part.ToArray().Count(); i++)
            {
                if (i == Label_Part.ToArray().Count())
                {
                    label.Insert(0, Label_Part.ToArray()[0].Trim());
                    if (lst.Count != 1)
                        label.Insert(0, " & ");
                    lst.Remove(Label_Part.ToArray()[0]);
                }
                else
                {
                    label.Insert(0, Label_Part.ToArray()[i].Trim());
                    if (lst.Count != 1)
                        label.Insert(0, " & ");
                    lst.Remove(Label_Part.ToArray()[i]);
                }
            }
        }

        private string FilterInsulationCodedValue(object insulationCodedValue)
        {
            if (insulationCodedValue != null && (insulationCodedValue.ToString() == "PLN" || insulationCodedValue.ToString() == "P&L"))
            {
                return " " + insulationCodedValue;
            }

            return "";
        }

        // Note that this lookup should be somewhere shared but is included here to minimize the impact of the code insertion
        private string FilterConductorUseAbbreviation(int materialCode)
        {
            switch (materialCode)
            {
                case 2:
                    return " PN";
                case 3:
                    return " CN";
                case 4:
                    return " PAR";
                case 9:
                    return "RCN";
                default:
                    return "";
            }
        }

        protected override string GetLabeltextField(IObject obj, string currentLabelField)
        {
            if (currentLabelField == "LABELTEXT") { return currentLabelField; }
            else if (currentLabelField == "LABELTEXT2") { return currentLabelField; }
            else if (currentLabelField == "LABELTEXT3") { return SchemaInfo.UFM.FieldModelNames.UlsText; }
            else if (currentLabelField == "LABELTEXT4") { return "LABELTEXT4"; }
            else { return currentLabelField; }
        }
        // End of IBM code change for Schematics Anno 8/22/2013
    }
}
