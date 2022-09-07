using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections.Generic;
using log4net;
using System.Reflection;
using ESRI.ArcGIS.Geodatabase;
using Miner.ComCategories;
using Miner.Interop;
using PGE.Common.Delivery.Diagnostics;

namespace PGE.Desktop.EDER.AutoUpdaters.LabelText
{
    /// <summary>
    /// Builds label text and lableText2 for Primary OH conductors.
    /// </summary>
    [Guid("BFE7CEE9-3AC0-4345-98D7-89C1CAFB3ACB")]
    [ProgId("PGE.Desktop.EDER.PriOHConductorLabel")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    public class PriOHConductorLabel : BaseLabelTextAU
    {
        #region Class Variables
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        private const string PhaseDesignation = "PhaseDesignation";
        private const string ConductorSize = "ConductorSize";
        private const string Material = "Material";
        private const string ConductorCount = "ConductorCount";
        private const string OperatingVoltage = "NominalVoltage";
        private const string WindSpeedCode = "WindSpeedCode";
        private const string _serviceIdc = "ServiceIdc";
        private const string ConductorUse = "ConductorUse";
        #endregion

        private enum Subtype
        {
            None,
            SinglePhasePrimaryOverhead,
            TwoPhasePrimaryOverhead,
            ThreePhasePrimaryOverhead,
        }

        private string labelText2;

        /// <summary>
        /// Constructor, pass in name and the number of label texts this AU will build.
        /// </summary>
        public PriOHConductorLabel()
            : base("PGE PriOHConductor LabelText AU", 2)
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
                    _logger.Debug("labelText :" + labelText);
                    return labelText;
                case 1:
                    _logger.Debug("labelText2 :" + labelText2);
                    return labelText2;
            }
            return String.Empty;
        }

        /// <summary>
        /// Actually builds label texts.
        /// </summary>
        /// <param name="obj"> The object that triggered the event. </param>
        /// <param name="labelText"> Label Text. </param>
        /// <param name="labelText2">Label Text 2. </param>
        private void BuildLabelTexts(IObject obj, out string labelText, out string labelText2)
        {
            var label = new StringBuilder();
            //get related objects
            var conductorInfoObjs = base.GetRelatedObjects(obj, null, esriRelRole.esriRelRoleOrigin, SchemaInfo.General.ClassModelNames.LabelTextUnit);
            _logger.Debug("Related object count for " + SchemaInfo.General.ClassModelNames.LabelTextUnit + " : " + conductorInfoObjs.Count());
            var conductor_code = string.Empty;
            conductor_code = conductorInfoObjs.Select(o => o.GetFieldValue("PGE_CONDUCTORCODE", false).Convert(string.Empty)).Concatenate("/");
            
            
            // filter out the conductor infor where PhaseDesignation <= 7 and ConductorUse is not Parallel
            Func<IObject, bool> condition;
            condition = o => o.GetFieldValue(PhaseDesignation, false).Convert<int>() <= 7 && o.GetFieldValue(ConductorUse, false).Convert<int>() != 4;

            Func<IObject, string> key;
            key = delegate(IObject o)
            {
                string keyString = string.Empty;
                //get conductor size code value
                string conductorSize = o.GetFieldValue(ConductorSize, false).Convert<string>(string.Empty);
                if (!conductorSize.ToUpper().Equals("UNK") && !conductorSize.ToUpper().Equals("NONE"))
                {
                    keyString = conductorSize;
                }
                //get material code value
                string material = o.GetFieldValue(Material, false).Convert<string>(string.Empty);
                if (!material.ToUpper().Equals("UNK") && !material.ToUpper().Equals("NONE"))
                {
                    keyString += material;
                }
                return keyString;

            };
            //Query the contorInfoObjs.
            var groups = conductorInfoObjs.Where(condition).GroupBy(key);
            foreach (var g in groups)
            {
                if (label.Length > 0)
                {
                    label.Append(" & ");
                }
                //Total number of conductor count.
                int count = g.Sum<IObject>(o => o.GetFieldValue(ConductorCount, false).Convert<int>());
                if (count > 0 && g.Key.ToString().Trim() != "")
                {
                    label.Append(count);
                    label.Append("-");
                }
                label.Append(g.Key);
            }

            //get the conductorInfo where Conductor use is  Parallel - INC000004050447

            var conductorInfoObjs_PAR = conductorInfoObjs.Where(u => u.GetFieldValue(PhaseDesignation, false).Convert<int>() <= 7 && u.GetFieldValue(ConductorUse, false).Convert<int>() == 4);

            // Calculate each conductor info for Parallel - INC000004050447
            foreach (var o in conductorInfoObjs_PAR)
            {
                var count = o.GetFieldValue(ConductorCount, false).Convert<int>();
                if (label.Length > 0)
                {
                    label.Append(" & ");
                }
                if (count > 0)
                {
                    label.Append(count);
                    label.Append("-");
                }
                var conductorSize = o.GetFieldValue(ConductorSize, false).Convert<string>(string.Empty);
                if (!conductorSize.ToUpper().Equals("UNK") && !conductorSize.ToUpper().Equals("NONE"))
                {
                    label.Append(conductorSize);
                }
                string material = o.GetFieldValue(Material, false).Convert<string>(string.Empty);
                if (!material.ToUpper().Equals("UNK") && !material.ToUpper().Equals("NONE"))
                {
                    label.Append(material);
                }
                string conductorUse = o.GetFieldValue(ConductorUse, false).Convert<string>(string.Empty);
                if (conductorUse != null)
                {
                    string conductorUseAbbreviation = LookupConductorUseAbbreviation(conductorUse.Convert<int>());
                    if (!String.IsNullOrEmpty(conductorUseAbbreviation))
                    {
                        if (conductorUseAbbreviation == "PAR")
                            label.Append(" (" + conductorUseAbbreviation + ")");
                    }
                }
            }


            // neutral
            condition = o => o.GetFieldValue(PhaseDesignation, false).Convert<int>() > 7;

            key = delegate(IObject o)
            {
                string keyString = string.Empty;
                //get conductor size code value
                string conductorSize = o.GetFieldValue(ConductorSize, false).Convert<string>(string.Empty);
                if (!conductorSize.ToUpper().Equals("UNK") && !conductorSize.ToUpper().Equals("NONE"))
                {
                    keyString = conductorSize;
                }
                //get material code value
                string material = o.GetFieldValue(Material, false).Convert<string>(string.Empty);
                if (!material.ToUpper().Equals("UNK") && !material.ToUpper().Equals("NONE"))
                {
                    keyString += material;
                }
                //If phasedesignation is not UNKNOWN then show.
                if (o.GetFieldValue(PhaseDesignation, false).Convert<string>(string.Empty) != "12")
                {
                    keyString += "(" + o.GetFieldValue(PhaseDesignation, true).Convert<string>(string.Empty) + ")";
                }
                return keyString;

            };


            groups = conductorInfoObjs.Where(condition).GroupBy(key);
            foreach (var g in groups)
            {
                if (label.Length > 0)
                {
                    label.Append(" & ");
                }

                //Total number of conductor count.
                int count = g.Sum<IObject>(o => o.GetFieldValue(ConductorCount, false).Convert<int>());
                if (count > 0 && g.Key.ToString().Trim() != "")
                {
                    label.Append(count);
                    label.Append("-");
                }
                //label.Append(count);
                //label.Append("-");
                label.Append(g.Key);
            }
            
            //Get operatingvoltage description value.
            var operatingVoltage = obj.GetFieldValue(OperatingVoltage, true);
            _logger.Debug(OperatingVoltage + " : " + operatingVoltage);

            label.Append(" ");
            label.Append(operatingVoltage);

            //get value for service idc
            var serviceIdc = obj.GetFieldValue(_serviceIdc, false).Convert<string>(string.Empty);
            _logger.Debug(_serviceIdc + " : " + serviceIdc);
            if (serviceIdc.Equals("Y", StringComparison.InvariantCultureIgnoreCase))
            {
                label.Append(" ");
                label.Append("SV");
            }

            //Issue-INC000004382554 (Only for labeltext)-Append TW 
            //DA 50
            if (conductor_code.Contains("81") || 
                conductor_code.Contains("82") || 
                conductor_code.Contains("83") || 
                conductor_code.Contains("84") || 
                conductor_code.Contains("85") || 
                conductor_code.Contains("86") || 
                conductor_code.Contains("87") || 
                conductor_code.Contains("88") ||
                conductor_code.Contains("89") ||
                conductor_code.Contains("90") ||
                conductor_code.Contains("91") ||
                conductor_code.Contains("92")
 
                )
            {
                var label_labeltext = new StringBuilder();
                label_labeltext = label;
                label_labeltext.Append(" ");
                label_labeltext.Append("TW");
                labelText = label_labeltext.ToString();
            }
            else
            {
                labelText = label.ToString();
            }

            // label text 2
            label.Remove(0, label.Length);
            // Start of IBM code change for Schematics Anno - 8/22/2013
            SetLabelText2(label, conductorInfoObjs);
            // End of IBM code change for Schematics Anno - 8/22/2013

            labelText2 = label.ToString();
        }

        // Start of IBM code change for Schematics Anno - 8/22/2013
        // make the changes as per eric's comment(INC000004155382) - 1/27/2016
        private void SetLabelText2(StringBuilder label, IEnumerable<IObject> conductorInfoObjs)
        {

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
                var conductorUse = o.GetFieldValue(ConductorUse, false);               
                if (conductorUse != null)
                {
                    string conductorUseAbbreviation = LookupConductorUseAbbreviation(conductorUse.Convert<int>());
                    if (!String.IsNullOrEmpty(conductorUseAbbreviation))
                    {
                        conductorInfoLabel.Append(" ");
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
                if (i == lst.Count -1)
                    label.Insert(0, lst[i].Trim());
                else
                {
                    label.Insert(0, lst[i].Trim());
                    label.Insert(0, " & ");
                }
            }

        }

        private static void GetStringPart(StringBuilder label, List<string> lst,string strPart)
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

        // Note that this lookup should be somewhere shared but is included here to minimize the impact of the code insertion
        private string LookupConductorUseAbbreviation(int materialCode)
        {
            switch (materialCode)
            {
                case 2:
                    return "PN";
                case 3:
                    return "CN";
                case 4:
                    return "PAR";
                case 9:
                    return "RCN";
                default:
                    return "";
            }
        }
        // End of IBM code change for Schematics Anno - 8/22/2013

    }
}