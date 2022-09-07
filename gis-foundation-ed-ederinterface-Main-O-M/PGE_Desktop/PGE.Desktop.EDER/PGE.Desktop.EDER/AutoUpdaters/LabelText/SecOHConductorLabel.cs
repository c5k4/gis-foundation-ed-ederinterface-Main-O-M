using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using log4net;
using ESRI.ArcGIS.Geodatabase;
using System.Reflection;
using Miner.ComCategories;
using Miner.Interop;
using PGE.Common.Delivery.Diagnostics;

namespace PGE.Desktop.EDER.AutoUpdaters.LabelText
{
    /// <summary>
    /// Builds label text for Secondary OH conductors.
    /// </summary>
    [Guid("6C745EC9-C3F7-4246-9E40-54CE5F572EEF")]
    [ProgId("PGE.Desktop.EDER.SecOHConductorLabel")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    public class SecOHConductorLabel : BaseLabelTextAU
    {
        #region Class Variables
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        private const string _phaseDesig = "PhaseDesignation";
        private const string _conductorSize = "ConductorSize";
        private const string _material = "Material";
        private const string _condudctorType = "ConductorType";
        private const string _condudctorCount ="ConductorCount";
        private const string _operatingVoltage = "OperatingVoltage";
        private const string _seriesSLIdc = "SeriesSLIdc";
        private const string _conductorUse = "ConductorUse";

        #endregion
        private enum Subtype
        {
            None = 0,
            SinglePhaseSecondaryOverhead = 1,
            ThreePhaseSecondaryOverhead = 2,
            ServiceSecondaryOverhead = 3,
            StreetlightSecondaryOverhead = 4,
            PseudoService = 5
        }

        /// <summary>
        /// Constructor, pass in name of this AU.
        /// </summary>
        public SecOHConductorLabel()
            : base("PGE SecOHConductor LabelText AU")
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

            return BuildLabelText(obj);
        }

        /// <summary>
        /// Actually builds the label text.
        /// </summary>
        /// <param name="obj"> The object that triggered the event. </param>
        /// <returns> Label Text. </returns>
        private string BuildLabelText(IObject obj)
        {
            var label = new StringBuilder();
            //get related records
            var conductorInfoObjs = base.GetRelatedObjects(obj,null, esriRelRole.esriRelRoleOrigin, SchemaInfo.General.ClassModelNames.LabelTextUnit);
            //get field value from the field and add to delegate
            Func<IObject, bool> condition;
            condition = o => o.GetFieldValue(_phaseDesig, false).Convert<int>() <= 7;
            //get field value from the field and add to delegate
            //sanjeev changed as per bug 843.
            Func<IObject, string> key = delegate(IObject o)
            {
                string keyString = string.Empty;
                // if conductorsize is unk or none then do not include in annotation.
                string conductorSize = o.GetFieldValue(_conductorSize, false).Convert<string>(string.Empty);
                if (!conductorSize.ToUpper().Equals("UNK") && !conductorSize.ToUpper().Equals("NONE"))
                {
                    keyString = conductorSize;
                }
                // if material is unk or none then do not include in annotation.
                string material = o.GetFieldValue(_material, false).Convert<string>(string.Empty);
                if (!material.ToUpper().Equals("UNK") && !material.ToUpper().Equals("NONE"))
                {
                    // as per INC000004154627 Sec Conductor OH Info domain value AAC should annotate AW (not AAC)
                    if (material.ToUpper().Equals("AAC"))
                    {
                        keyString += "AW";
                    }
                    else
                        keyString += material;
                }

                //if conductor is CN || RCN then append to LabelText
                //INC000003855983
                string conductoruse = o.GetFieldValue(_conductorUse, false).Convert<string>(string.Empty);

                if (conductoruse == "3" || conductoruse == "9")
                {
                    if (conductoruse == "3")
                    {
                        keyString += "(CN)";
                    }
                    else if (conductoruse == "9")
                    {
                        keyString += "(RCN)";
                    }
                }
   

                // if material is unk or none then do not include in annotation.
                string conductorType = o.GetFieldValue(_condudctorType, false).Convert<string>(string.Empty);
                if (!conductorType.ToUpper().Equals("OPN"))
                {
                    keyString += " " + conductorType;
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

            //********INC000003832405 *******
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
                case Subtype.SinglePhaseSecondaryOverhead:
                case Subtype.ThreePhaseSecondaryOverhead:
                case Subtype.StreetlightSecondaryOverhead:
                    {
                        //get value for operating voltage
                        //As per bug 843 if operatingVoltage is 20,22,23,99 then do not show.
                        //var operatingVoltageNotShow = new int[] { 20, 22, 23, 99 };
                        //var operatingVoltage = obj.GetFieldValue(_operatingVoltage,true);
                        //_logger.Debug(_operatingVoltage + " : " + operatingVoltage);
                        //if (!operatingVoltageNotShow.Contains(obj.GetFieldValue(_operatingVoltage, false).Convert<int>(0)))
                        //{
                        //    label.Append(" ");
                        //    label.Append(operatingVoltage);
                        //}

                        if (subtype == Subtype.StreetlightSecondaryOverhead)
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

                            //get value for seriesSlIdc
                            var seriesSL = obj.GetFieldValue(_seriesSLIdc, false).Convert<string>(string.Empty);
                            _logger.Debug(_seriesSLIdc + " : " + seriesSL);
                            if (seriesSL.Equals("Y", StringComparison.InvariantCultureIgnoreCase))
                            {
                                //******INC000003832405 - SSL is being annotated on streetlight conductor******

                                if (label.ToString().Trim() != "") 
                                    label.Append(" SSL");
                            }
                        }

                        break;
                    }
                //case Subtype.ServiceSecondaryOverhead:
                //    label.Append(" SV");
                //    break;
                default:
                    break;
            }
            _logger.Debug ("Attribute : " + label.ToString());
            return label.ToString();
        }
    }
}
