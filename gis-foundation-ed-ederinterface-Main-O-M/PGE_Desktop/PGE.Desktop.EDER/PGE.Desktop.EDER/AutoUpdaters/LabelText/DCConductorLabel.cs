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
using PGE.Desktop.EDER.UFM;

namespace PGE.Desktop.EDER.AutoUpdaters.LabelText
{
    /// <summary>
    /// Builds label text for DC conductors.
    /// </summary>
    [Guid("4B8E6512-57A4-4878-811F-5B680B2A1F4E")]
    [ProgId("PGE.Desktop.EDER.DCConductorLabel")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    public class DCConductorLabel : BaseLabelTextAU
    {
        #region Class Variables
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        private const string _phaseDesig = "PhaseDesignation";
        private const string _conductorSize = "ConductorSize";
        private const string _material = "Material";
        private const string _conductorType = "ConductorType";
        private const string _conductorCount = "ConductorCount";
        private const string _operatingVoltage = "OperatingVoltage";
        private const string _insulation = "Insulation";

        private string _labelText1;
        private string _labelText2;
        #endregion

        /// <summary>
        /// Constructor, pass in name of this AU.
        /// </summary>
        public DCConductorLabel()
            : base("PGE DCConductor LabelText AU", 2)
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
            string labelText = string.Empty;

            switch (labelIndex)
            {
                case 0:
                    BuildLabelTexts(obj);
                    labelText = _labelText1;
                    _logger.Debug("labelText :" + labelText);
                    break;
                    
                case 1:
                    _logger.Debug("labelText2" + _labelText2);
                    labelText = _labelText2;
                    break;
            }

            return labelText;
        }

        /// <summary>
        /// Actually builds the label text.
        /// </summary>
        /// <param name="obj"> The object that triggered the event. </param>
        /// <returns> Label Text. </returns>
        private void BuildLabelTexts(IObject obj)
        {
            var label = new StringBuilder();
            //get related records 
            //var conductorInfoObjs = obj.GetRelatedObjects(null, esriRelRole.esriRelRoleOrigin, SchemaInfo.General.ClassModelNames.LabelTextUnit);
            var conductorInfoObjs = base.GetRelatedObjects(obj,null, esriRelRole.esriRelRoleOrigin, SchemaInfo.General.ClassModelNames.LabelTextUnit);
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
                if (!insulation.ToUpper().Equals("UNK") && !insulation.ToUpper().Equals("NONE") && !insulation.ToUpper().Equals("OTH"))
                {
                    keyString += " " + insulation;
                }

                string conductorType = o.GetFieldValue(_conductorType, false).Convert<string>(string.Empty);
                if (!conductorType.ToUpper().Equals("SW"))
                {
                    keyString += " " + conductorType;
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

                int count = g.Sum<IObject>(o => o.GetFieldValue(_conductorCount, false).Convert<int>());
                //********INC000003832405 *******
                if (count > 0 && g.Key.ToString().Trim() != "")
                {
                    label.Append(count);
                    label.Append("-");
                }

                if(g.Key.ToString().Trim() != "")
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

                int count = g.Sum<IObject>(o => o.GetFieldValue(_conductorCount, false).Convert<int>());
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
                    label.Remove(label.ToString().Length-3,3);
                }
            }

            if (label.Length > 0)
            {
                label.Append(" DC");
            }

            _logger.Debug("Attribute : " + label.ToString());

            _labelText1 = label.ToString();

            SetLabelText2(obj);
        }

        private void SetLabelText2(IObject obj)
        {
            _logger.Debug("Setting conductor cross section anno on " + obj.Class.AliasName + ":" + obj.OID.ToString());
            _labelText2 = XSectionConductorHelper.CalculateXSectionConductorText(obj, _labelText1);
        }


        protected override string GetLabeltextField(IObject obj, string currentLabelField)
        {
            if (currentLabelField == "LABELTEXT") { return currentLabelField; }
            else if (currentLabelField == "LABELTEXT2") { return SchemaInfo.UFM.FieldModelNames.UlsText; }
            else { return currentLabelField; }
        }
    }
}
