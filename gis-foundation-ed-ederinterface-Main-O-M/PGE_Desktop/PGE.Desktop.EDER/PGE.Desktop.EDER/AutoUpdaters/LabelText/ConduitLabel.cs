using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ESRI.ArcGIS.Geodatabase;
using Miner.Interop;
using Miner.ComCategories;
using System.Runtime.InteropServices;
using PGE.Common.Delivery.Framework;
using ESRI.ArcGIS.esriSystem;
using System.Reflection;
using log4net;
using PGE.Common.Delivery.Diagnostics;
using System.Collections;

namespace PGE.Desktop.EDER.AutoUpdaters.LabelText
{

    /// <summary>
    /// ConduitLabel class updates the LabelText and LabelText2 field in the geodatabase for conduit features.
    /// </summary>
    [Guid("66B2F0F8-207D-4806-AF76-A7FBB9DD1BD5")]
    [ProgId("PGE.Desktop.EDER.ConduitLabel")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    public class ConduitLabel : BaseLabelTextAU
    {
        #region Class Variables
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        private const string FieldNameCapitalize = "CAPITALIZEDIDC";
        private const string Pvc = " PVC";
        private const string CapitalizedText = " NCC";
        private const string FieldNameDuctsVacant = "DUCTSVACANT";
        private const string _fieldNameAvailable = "AVAILABLE";
        private const string _fieldNameDuctSize = "DUCTSIZE";
        private const string _fieldNameMaterial = "MATERIAL";
        private const string _fieldNameDuctCount = "DUCTCOUNT"; // make the change as per INC000003929209
        private string[] _unknownFldValue = { "UNK", "0", "Unknown" };
        private const string _multipleDucts = "DUCTS";
        private const string _singleDuct = "DUCT";
        public static IRelationshipClass UGRelClassBeingUnrelated = null;
        public static int UGObjectBeingUnrelated = -1;
        public static int UGObjectFCIDBeingUnrelated = -1;

        private StringBuilder _label;

        private Dictionary<string, int> DuctCount =
            new Dictionary<string, int>();

        private static readonly Dictionary<Subtype, string[]> LabelFormat = new Dictionary<Subtype, string[]>
                {
                    //{ Subtype, { Format label 1,
                    //             Format label 2,
                    //             ...             } }
                    {Subtype.DuctBank, new [] {"{0}{1}{2}{3} {4}{5}","{0}{1}{2}{3} Spare"}},
                    {Subtype.Conduit, new [] {"{0}{1}{2}{3} {4}{5}","{0}{1}{2}{3} Spare"}},
                    {Subtype.CIC,     new [] {"{1} {2}{3}"}}
                };
        #endregion
        /// <summary>
        /// ConduitSystem subtypes
        /// </summary>
        enum Subtype
        {
            //Enums are 0 based and the Subtype starts with 1
            None,
            DuctBank,
            Conduit,
            CIC
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="ConduitLabel"/> class.
        /// </summary>
        public ConduitLabel()
            : base("PGE Conduit Label Text AU", 2)
        {
            _label = new StringBuilder();
        }


        /// <summary>
        /// Gets the label text given the specific object.
        /// </summary>
        /// <param name="obj">The object that triggered the event.</param>
        /// <param name="autoUpdaterMode">The auto updater mode.</param>
        /// <param name="editEvent">The edit event.</param>
        /// <param name="labelIndex"></param>
        /// <returns>
        /// The label text that will be stored on the field assigned the <see cref="SchemaInfo.General.FieldModelNames.LabelText"/> field model name, or null if no text should be assigned.
        /// </returns>
        protected override string GetLabelText(IObject obj, mmAutoUpdaterMode autoUpdaterMode, mmEditEvent editEvent, int labelIndex)
        {
            // Ideally this class will be refactored ASAP -- this was a late-breaking fix for 9.3
            string returnString = "";
            var sFiberIDC = Convert.ToString(obj.get_Value(obj.Fields.FindField("FIBERIDC")));

            try
            {
                var subtype = obj.SubtypeCodeAsEnum<Subtype>();
                if (subtype != Subtype.DuctBank)
                {
                    // Summary of the change is that LabelText1 and LabelText2 are mutually exclusive,
                    // based on the existence of a relationship to conductors
                    // This solution has cannibalized the existing LabelText1/2 code, which has
                    // LabelText1 providing the "non-spare" DuctDef and LabelText2 providing the "spare" +
                    // any suffix information.
                    // 
                    string labelText = GetLabelTextInternal(obj, autoUpdaterMode, editEvent, 0);
                    string labelText2 = GetLabelTextInternal(obj, autoUpdaterMode, editEvent, 1);

                    if (!String.IsNullOrEmpty(labelText) && !String.IsNullOrEmpty(labelText2) && labelText != labelText2)
                    {
                        returnString = labelText + " & " + labelText2;
                        if (Regex.Matches(returnString, CapitalizedText).Count > 1)
                        {
                            returnString = returnString.Remove(returnString.IndexOf(CapitalizedText), CapitalizedText.Length);
                        }
                    }
                    else if (!String.IsNullOrEmpty(labelText))
                    {
                        returnString = labelText;
                    }
                    else
                    {
                        returnString = labelText2;
                    }

                    if ((labelIndex == 0 && ConduitSystemHasRelatedConductor(obj)) ||
                        (labelIndex == 1 && !ConduitSystemHasRelatedConductor(obj)))
                    {
                        //DA Item 52 - ME Release
                        returnString = returnString.Replace("  ", " ").Trim();
                        returnString += sFiberIDC == "Y" && !returnString.EndsWith("w/ FO") ? " w/ FO" : string.Empty;
                        return returnString;
                    }
                }
                // otherwise it will be blank
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Cannot create label text " + (labelIndex + 1) + ":\r\n"
                                                     + ex.Message
                                                     );
            }
          //  returnString = sFiberIDC == "Y" ? "w/ FO" : string.Empty;  //DA Item 52 - ME Release
            return string.Empty; // returnString;
        }

        private string GetLabelTextInternal(IObject obj, mmAutoUpdaterMode autoUpdaterMode, mmEditEvent editEvent,
            int labelIndex)
        {
            string returnString = "";
            try
            {
                var feature = (IFeature)obj;

                // Prepend a deactivated flag if the conduit has been abandoned
                object deact = obj.GetFieldValue(null, false, SchemaInfo.General.FieldModelNames.DeactivateIndicator);
                string deactivated = string.Empty;
                if (deact != null)
                {
                    if (deact.ToString() == "Y")
                    {
                        deactivated = "DEACT ";
                    }
                }

                var ducts = new DuctBankInfo(feature)
                            .GetDucts();
                var subtype = obj.SubtypeCodeAsEnum<Subtype>();
                if (ducts.FirstOrDefault() == null)
                {
                    if (subtype == Subtype.CIC)
                    {
                        var ductLabelText = deactivated + "CIC";
                        returnString = ductLabelText;
                        return ductLabelText;
                    }

                    var relatedUnits = base.GetRelatedObjects(obj, null, esriRelRole.esriRelRoleOrigin, SchemaInfo.Electric.ClassModelNames.DuctDefinition);
                    if (relatedUnits.Count<IObject>() > 0)
                    {
                        var ductLabelText = deactivated + GetLabelTextFromDuctDefinition(relatedUnits, labelIndex, subtype, obj);
                        // if field capitalizedIdc is No
                        string capitalizedIdc = obj.GetFieldValue(FieldNameCapitalize, true, null)
                                                                   .Convert<string>()
                                                                   .Contains("n", StringComparison.InvariantCultureIgnoreCase)
                                                                ? CapitalizedText
                                                                : string.Empty;
                        if (!String.IsNullOrEmpty(ductLabelText))
                        {
                            returnString = ductLabelText + capitalizedIdc;
                        }
                        return returnString;
                    }
                    else
                    {
                        _logger.Debug("No ducts found on conduit or model name " + SchemaInfo.Electric.ClassModelNames.DuctDefinition + " not present for objectid " + feature.OID);
                        returnString = string.Empty;
                        return string.Empty;
                    }
                }
                else
                {
                    _logger.Debug("Found " + ducts.Count() + " ducts on Conduit with OID " + feature.OID);
                }

                switch (labelIndex)
                {
                    case 0:
                        {
                            string labelText;
                            // Label text 2 is blank for CIC subtype
                            if (subtype == Subtype.CIC)
                            {
                                labelText = "";
                                returnString = labelText;
                                return labelText;
                            }
                            string capitalizedIdc = obj.GetFieldValue(FieldNameCapitalize, true, null)
                                                       .Convert<string>()
                                                       .Contains("n", StringComparison.InvariantCultureIgnoreCase)
                                                    ? CapitalizedText
                                                    : string.Empty;
                            Func<string, string> readMaterial = (string material) => material;


                            if (subtype == Subtype.DuctBank)
                                readMaterial = (string material) => Pvc.Contains(material, StringComparison.InvariantCultureIgnoreCase)
                                                                    ? string.Empty
                                                                    : material;
                            labelText = deactivated + ducts
                               .GroupBy(d => new
                               {
                                   Diameter = d.Diameter,
                                   Material = _unknownFldValue.Contains(d.Material) ? string.Empty : readMaterial(d.Material),
                               })
                               .Select(g => new
                               {
                                   Count = g.Count(),
                                   Diameter = g.Key.Diameter,
                                   Material = g.Key.Material
                               })
                               .OrderByDescending(g => g.Diameter)
                               .ThenByDescending(g => g.Count)
                              .Select(g => string.Format(LabelFormat[subtype][labelIndex],
                                                          g.Count > 0 ? g.Count.ToString() + '-' : String.Empty,
                                                          (g.Diameter.ToString().Length < 1 || _unknownFldValue.Contains(g.Diameter.ToString())) && g.Count > 1 && (g.Material.Length < 1 || _unknownFldValue.Contains(g.Material.ToString())) ? _multipleDucts : string.Empty,
                                                          (g.Diameter.ToString().Length < 1 || _unknownFldValue.Contains(g.Diameter.ToString())) && g.Count == 1 && (g.Material.Length < 1 || _unknownFldValue.Contains(g.Material.ToString())) ? _singleDuct : string.Empty,
                                                          g.Diameter.ToString().Length > 0 && !_unknownFldValue.Contains(g.Diameter.ToString()) ? g.Diameter.ToString() + '"' : string.Empty,
                                                          g.Material.Length > 0 && !_unknownFldValue.Contains(g.Material.ToString()) ? g.Material : string.Empty,
                                                          capitalizedIdc))
                               .Concatenate(" & ");
                            _logger.Debug("Created label text: " + labelText);
                            returnString = labelText;
                            return labelText;
                        }
                    case 1: // DuctBanks and Conduits label e.g. 2-10" Spare
                        {
                            // Label text 2 is blank for CIC subtype
                            if (subtype == Subtype.CIC) break;

                            var labelText =
                                ducts
                                .Where(d => d.Available)
                                .GroupBy(d => new { Diameter = d.Diameter })
                                .Select(g => new
                                {
                                    Count = g.Count(),
                                    Diameter = g.Key.Diameter
                                })
                                .OrderByDescending(g => g.Diameter)
                                .ThenByDescending(g => g.Count)
                                .Select(g => string.Format(LabelFormat[subtype][labelIndex],
                                                           g.Count > 0 ? g.Count.ToString() + '-' : String.Empty,
                                                           (g.Diameter.ToString().Length < 1 || _unknownFldValue.Contains(g.Diameter.ToString())) && g.Count > 1 ? _multipleDucts : string.Empty,
                                                           (g.Diameter.ToString().Length < 1 || _unknownFldValue.Contains(g.Diameter.ToString())) && g.Count == 1 ? _singleDuct : string.Empty,
                                                           g.Diameter.ToString().Length > 0 && !_unknownFldValue.Contains(g.Diameter.ToString()) ? g.Diameter.ToString() + '"' : string.Empty))

                                .Concatenate(" & ");
                            labelText = labelText + deactivated;
                            _logger.Debug("Created label text: " + labelText);
                            returnString = labelText;
                            return labelText;
                        }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Cannot create label text " + (labelIndex + 1) + ":\r\n"
                                                     + ex.Message);
            }
            finally
            {

            }
            returnString = string.Empty;
            return string.Empty;

        }

        public static void ExecuteOnRelatedObjects(IObject obj, IMMSpecialAUStrategyEx AUToExecute, mmAutoUpdaterMode autoUpdaterMode, string modelNameOfRelated, IMMModelNameManager ModelNameManager)
        {
            IEnumRelationshipClass relClasses = obj.Class.get_RelationshipClasses(esriRelRole.esriRelRoleAny);
            relClasses.Reset();
            IRelationshipClass relClass = null;
            while ((relClass = relClasses.Next()) != null)
            {
                if (relClass.DestinationClass == obj.Class)
                {
                    if (ModelNameManager.ContainsClassModelName(relClass.OriginClass, modelNameOfRelated))
                    {
                        break;
                    }
                }
                else
                {
                    if (ModelNameManager.ContainsClassModelName(relClass.DestinationClass, modelNameOfRelated))
                    {
                        break;
                    }
                }
            }
            if (relClass != null)
            {
                ISet relatedUGs = relClass.GetObjectsRelatedToObject(obj);
                relatedUGs.Reset();
                IObject UGSystemObject = null;
                //IEnumerable<IObject> relatedObjects = GetRelatedObjects(obj, "", esriRelRole.esriRelRoleAny, SchemaInfo.Electric.ClassModelNames.PGEConduitSystem);
                while ((UGSystemObject = relatedUGs.Next() as IObject) != null)
                {
                    AUToExecute.Execute(UGSystemObject, autoUpdaterMode, mmEditEvent.mmEventFeatureUpdate);
                }
            }
        }

        /// <summary>
        /// Get Label text from duct definition
        /// </summary>
        /// <param name="relatedUnits">Related Objects</param>
        /// <param name="labelIndex">Integer</param>
        /// <param name="conduitSubtype">Subtype</param>
        /// <param name="obj">Esri Object</param>
        /// <returns></returns>
        private string GetLabelTextFromDuctDefinition(IEnumerable<IObject> relatedUnits, int labelIndex, Subtype conduitSubtype, IObject obj)
        {
            string labelText = string.Empty;

            switch (labelIndex)
            {
                case 0:
                    {
                        // If subtype is CIC,retun "CIC"
                        if (conduitSubtype == Subtype.CIC)
                        {
                            //labelText = "CIC";
                            break;
                        }


                        // Get label text for LABELTEXT1
                        labelText = GetLabelTextOneFromDuctDefinition(obj, relatedUnits, conduitSubtype);
                        break;
                    }
                case 1:
                    {
                        //// If subtype is CIC,retun null
                        //if (conduitSubtype == Subtype.CIC) //break;
                        // If subtype is CIC,retun "CIC"
                        if (conduitSubtype == Subtype.CIC)
                        {
                            //labelText = "CIC";
                            break;
                        }

                        // Get label text for LABELTEXT2
                        labelText = GetLabelTextTwoFromDuctDefinition(relatedUnits);
                        break;
                    }
            }


            return labelText;
        }

        /// <summary>
        /// Get LABELTEXT2 from duct definition
        /// </summary>
        /// <param name="relatedUnits">Esri related objects</param>
        /// <returns>string</returns>
        private string GetLabelTextTwoFromDuctDefinition(IEnumerable<IObject> relatedUnits)
        {
            string keyString = string.Empty;
            _label = new StringBuilder();

            try
            {
                //Check availbilty -- availability = 1 means spare
                Func<IObject, bool> condition;
                condition = o => o.GetFieldValue(_fieldNameAvailable, false).Convert<int>() == 1;
                Func<IGrouping<string, IObject>, double> descendOrder = delegate(IGrouping<string, IObject> s)
                {
                    double i = -1;
                    if (s.Key.Contains('"'))
                    {
                        var keyValue = s.Key.Split(new char[] { '"' }.First());
                        if (double.TryParse(keyValue[0], out i))
                        {
                            return i;

                        }
                    }
                    return i;

                };

                #region LabelText2
                Func<IObject, string> keyLabelText2 = delegate(IObject o)
                {

                    //Get Duct size
                    string ductSize = o.GetFieldValue(_fieldNameDuctSize, true, SchemaInfo.Electric.FieldModelNames.DuctSize).Convert<string>(string.Empty);
                    //Get material type
                    string material = o.GetFieldValue(_fieldNameMaterial, false).Convert<string>(string.Empty);
                    int ductCount = o.GetFieldValue(_fieldNameDuctCount, false).Convert<Int32>(1);//Make the change as per INC000003929209

                    if ((ductSize.Length < 1 || _unknownFldValue.Contains(ductSize)) && (material.Length < 1 || _unknownFldValue.Contains(material)))
                    {
                        keyString = "0";
                    }
                    else
                    {
                        if (ductSize.Length > 0 && !_unknownFldValue.Contains(ductSize))
                        {
                            keyString = ductSize;
                        }

                        if (!material.ToUpper().Equals("PVC") && !_unknownFldValue.Contains(material))
                        {
                            _logger.Debug("Material Type: " + material);
                            keyString = keyString + " " + material;
                        }
                    }

                    // Start - Make the change as per INC000003929209
                    if (!DuctCount.ContainsKey(keyString))
                    {
                        DuctCount.Add(keyString, ductCount);
                    }

                    else if (DuctCount.ContainsKey(keyString))
                    {
                        int prevDuctCount = 0;
                        if (DuctCount.TryGetValue(keyString, out prevDuctCount))
                        {
                            ductCount = ductCount + prevDuctCount;
                        }
                        DuctCount[keyString] = ductCount;
                    }
                    // End - make the change as per INC000003929209
                    /*
                    if ((ductSize.Length > 0 && !_unknownFldValue.Contains(ductSize)) || material.Length > 0 && !_unknownFldValue.Contains(material))
                    {
                        if (!_unknownFldValue.Contains(ductSize))
                        {
                            keyString = ductSize;
                        }
                        
                        if (!_unknownFldValue.Contains(material))
                        {
                            _logger.Debug("Material Type: " + material);
                            keyString = keyString + " " + material;
                        }
                    }
                    else
                    {
                        keyString = "0";
                    }
                    
                    */

                    _logger.Debug("Conduit key: " + keyString);
                    return keyString;
                };
                #endregion

                //Get groups by key
                ///var groups = relatedUnits.Where(condition).GroupBy(keyLabelText2).OrderByDescending(a =>Convert.ToInt32(a.Key.Trim(new char[]{'"'}))).ThenByDescending(a => a.Count());
                var groups = relatedUnits.Where(condition).GroupBy(keyLabelText2).OrderByDescending(descendOrder).ThenByDescending(a => a.Count());

                //loop through each groups
                foreach (var g in groups)
                {
                    if (_label.Length > 0)
                    {
                        _label.Append(" & ");
                    }

                    //Start -Make the change as per INC000003929209
                    //int count = g.Count();

                    int count = 0;
                    if (DuctCount.TryGetValue(g.Key, out count))
                    { }
                    //End - Make the change as per INC000003929209
                    if (count > 1)
                    {
                        _label.Append(count);
                        _label.Append("-");
                    }
                    else if (g.Key == "0")
                    {
                        if (count != 0)
                        {
                            _label.Append(count);
                            _label.Append("-");
                        }
                    }

                    if (count > 1 && g.Key == "0")
                    {
                        _label.Append(_multipleDucts);
                    }
                    else if (g.Key == "0")
                    {
                        _label.Append(_singleDuct);
                    }
                    else
                    {
                        _label.Append(g.Key);
                    }
                    _label.Append(" Spare");

                }
                _logger.Debug("LABELTEXT2 value: " + _label.ToString());
                DuctCount.Clear();
                return _label.ToString();
            }
            catch (Exception ex)
            {
                _logger.Warn(ex.Message);
                DuctCount.Clear();
                return "";
            }
        }

        /// <summary>
        /// Get LABELTEXT1 from ductdefinition
        /// <param name="obj">ESRI object</param>
        /// <param name="relatedUnits">ESRI related objects</param>
        /// <returns>string</returns>
        private string GetLabelTextOneFromDuctDefinition(IObject obj, IEnumerable<IObject> relatedUnits, Subtype conduitSubtype)
        {
            string keyString = string.Empty;
            _label = new StringBuilder();

            try
            {
                //Check availbilty -- availability = 0 means not spare
                Func<IObject, bool> condition;
                condition = o => o.GetFieldValue(_fieldNameAvailable, false).Convert<int>() == 0;

                Func<IGrouping<string, IObject>, double> descendOrder = delegate(IGrouping<string, IObject> s)
                {
                    double i = -1;
                    if (s.Key.Contains('"'))
                    {
                        var keyValue = s.Key.Split(new char[] { '"' }.First());
                        if (double.TryParse(keyValue[0], out i))
                        {
                            return i;

                        }
                    }
                    return i;
                };


                #region Group all the fields
                Func<IObject, string> key = delegate(IObject o)
                {
                    //Get ductSize
                    string ductSize = o.GetFieldValue(_fieldNameDuctSize, true, SchemaInfo.Electric.FieldModelNames.DuctSize).Convert<string>(string.Empty);
                    //Get material type
                    string material = o.GetFieldValue(_fieldNameMaterial, false).Convert<string>(string.Empty);
                    int ductCount = o.GetFieldValue(_fieldNameDuctCount, false).Convert<Int32>(1);//Make the change as per INC000003929209


                    if ((ductSize.Length < 1 || _unknownFldValue.Contains(ductSize)) && (material.Length < 1 || _unknownFldValue.Contains(material)))
                    {
                        keyString = _singleDuct;
                    }

                    if (ductSize.Length > 0 && !_unknownFldValue.Contains(ductSize))
                    {
                        //append to string
                        keyString = ductSize + " ";
                    }

                    if (ductSize.Length < 1 && material.Length > 0)
                    {

                        if (!_unknownFldValue.Contains(material))
                        {
                            keyString = material;
                        }

                        /*
                        // If subtype is conduit
                        if (conduitSubtype == Subtype.DuctBank)
                        {

                            //Check matrial type is not PVC
                            if (!material.ToUpper().Equals("PVC") && !_unknownFldValue.Contains(material))
                            {
                                _logger.Debug("Material Type: " + material);
                                keyString = material;
                            }
                        }
                        else
                        {
                            if (!_unknownFldValue.Contains(material))
                            {
                                keyString = material;
                            }
                        }
                        */
                    }

                    if (ductSize.Length > 0 && !_unknownFldValue.Contains(ductSize) && material.Length > 0)
                    {
                        if (!material.ToUpper().Equals("PVC") && !_unknownFldValue.Contains(material))
                        {
                            keyString += material;
                        }


                        /*
                        // If subtype is conduit
                        if (conduitSubtype == Subtype.DuctBank)
                        {
                            //Check matrial type is not PVC
                            if (!material.ToUpper().Equals("PVC") && !_unknownFldValue.Contains(material))
                            {
                                _logger.Debug("Material Type: " + material);
                                keyString += material;
                            }
                        }
                        else
                        {
                            if (!_unknownFldValue.Contains(material))
                            {
                                keyString += material;
                            }
                        }

                        */
                    }
                    //Start -Make the change as per INC000003929209
                    if (!DuctCount.ContainsKey(keyString))
                    {
                        DuctCount.Add(keyString, ductCount);
                    }

                    else if (DuctCount.ContainsKey(keyString))
                    {
                        int prevDuctCount = 0;
                        if (DuctCount.TryGetValue(keyString, out prevDuctCount))
                        {
                            ductCount = ductCount + prevDuctCount;
                        }
                        DuctCount[keyString] = ductCount;
                    }
                    //End -Make the change as per INC000003929209
                    _logger.Debug("Conduit key: " + keyString);
                    return keyString;
                };
                #endregion

                #region Get groups by key

                var groups = relatedUnits.Where(condition).GroupBy(key).OrderByDescending(descendOrder).ThenByDescending(a => a.Count());//.OrderByDescending(a => Convert.ToInt32(a.Key.Trim(new char[] { '"' }))).ThenBy(a => a.Key.Count());

                //loop through each groups
                foreach (var g in groups)
                {
                    if (_label.Length > 0)
                    {
                        _label.Append(" & ");
                    }

                    //Start -Make the change as per INC000003929209
                    //int count = g.Count();
                    int count = 0;
                    if (DuctCount.TryGetValue(g.Key, out count))
                    { }
                    //Start -Make the change as per INC000003929209
                    if (count > 1)
                    {
                        _label.Append(count);
                        _label.Append("-");
                    }
                    else if (g.Key == _singleDuct)
                    {
                        if (count != 0)
                        {
                            _label.Append(count);
                            _label.Append("-");
                        }
                    }

                    if (count > 1 && g.Key == _singleDuct)
                    {
                        _label.Append(g.Key + "S");
                    }
                    else
                    {
                        _label.Append(g.Key);
                    }
                }
                #endregion
                _logger.Debug("LABELTEXT1 value: " + _label.ToString());
                DuctCount.Clear();
                return _label.ToString();
            }
            catch (Exception ex)
            {
                _logger.Warn(ex.Message);
                DuctCount.Clear();
                return "";
            }
        }

        private bool ConduitSystemHasRelatedConductor(IObject obj)
        {
            bool conduitSystemIsRelated = false;

            string[] relatedConductorClasses = new string[]
            {
                SchemaInfo.Electric.ClassModelNames.PGEDeactivatedElectricLineSegment,
                SchemaInfo.UFM.ClassModelNames.DcConductor,
                SchemaInfo.Electric.ClassModelNames.PrimaryUGConductor,
                SchemaInfo.Electric.ClassModelNames.SecondaryUnderGround
            };
            var relatedUnits = base.GetRelatedObjects(obj, null, esriRelRole.esriRelRoleOrigin, relatedConductorClasses);
            if (relatedUnits.Count<IObject>() > 0)
            {
                if (relatedUnits.Count<IObject>() == 1)
                {
                    foreach (IObject relatedObj in relatedUnits)
                    {
                        if (relatedObj.Class.ObjectClassID != UGObjectFCIDBeingUnrelated || relatedObj.OID != UGObjectBeingUnrelated)
                        {
                            conduitSystemIsRelated = true;
                        }
                        else
                        {
                            ITable UGRelationshipTable = UGRelClassBeingUnrelated as ITable;
                            if (UGRelationshipTable != null)
                            {
                                IQueryFilter qf = new QueryFilterClass();
                                qf.AddField(relatedObj.Class.OIDFieldName);
                                qf.WhereClause = UGRelClassBeingUnrelated.DestinationForeignKey + " = " + relatedObj.OID;
                                ICursor relObjectCursor = UGRelationshipTable.Search(qf, false);
                                int counter = 0;
                                IRow row = null;
                                while ((row = relObjectCursor.NextRow()) != null)
                                {
                                    counter++;
                                    Marshal.ReleaseComObject(row);
                                }

                                if (counter > 1) { conduitSystemIsRelated = true; }
                            }
                        }
                    }
                }
                else
                {
                    conduitSystemIsRelated = true;
                }
            }

            return conduitSystemIsRelated;
        }
    }
}
