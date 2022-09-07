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
using Telvent.Delivery.Diagnostics;

namespace Telvent.PGE.ED.Desktop.AutoUpdaters.LabelText
{
    /// <summary>
    /// Builds label text and lableText2 for Primary UG conductors.
    /// </summary>
    [Guid("080AB0E1-E84B-476A-B0DD-60D0BE580FAA")]
    [ProgId("Telvent.PGE.ED.PriUGConductorLabel")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    public class PriUGConductorLabel : BaseLabelTextAU
    {
        #region Class Variables
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private const string _phaseDesignation = "PhaseDesignation";
        private const string _conductorSize = "ConductorSize";
        private const string _material = "Material";
        private const string _insulation = "Insulation";
        private const string _rating = "Rating";
        private const string _condudctorCount = "ConductorCount";
        private const string _operatingVoltage = "OperatingVoltage";
        private const string _jointTrench = "JointTrenchIdc";
        private const string _serviceIdc = "ServiceIdc";
        #endregion

        private enum Subtype
        {
            None,
            SinglePhasePrimaryUnderground,
            TwoPhasePrimaryUnderground,
            ThreePhasePrimaryUnderground,
        }

        private string labelText2;

        /// <summary>
        /// Constructor, pass in name and the number of label texts this AU will build.
        /// </summary>
        public PriUGConductorLabel()
            : base("PGE PriUGConductor LabelText AU", 2)
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
                    _logger.Debug("labelText2" + labelText2);
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
            //get related object
            var conductorInfoObjs = base.GetRelatedObjects(obj,null, esriRelRole.esriRelRoleOrigin, SchemaInfo.General.ClassModelNames.LabelTextUnit);

            Func<IObject, bool> condition;
            condition = o => o.GetFieldValue(_phaseDesignation, false).Convert<int>() <= 7;
            Func<IObject, string> key;
            //get field value from the field and add to delegate
            key = delegate(IObject o)
            { 
                string keyString = string.Empty;
                //var subtype = obj.SubtypeCodeAsEnum<Subtype>();
                string conductorSize = o.GetFieldValue(_conductorSize,false).Convert<string>(string.Empty);
                string material = o.GetFieldValue(_material,false).Convert<string>(string.Empty);

                    if (!conductorSize.ToUpper().Equals("UNK") && !conductorSize.ToUpper().Equals("NONE"))
                    {
                        keyString = conductorSize + " ";
                    }
                    if (!material.ToUpper().Equals("UNK") && !material.ToUpper().Equals("NONE"))
                    {
                        keyString += material + " ";
                    }

                    keyString += o.GetFieldValue(_insulation, false).Convert<string>(string.Empty);// +" (" + o.GetFieldValue(_rating, true).Convert<string>(string.Empty) + ")";

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
                label.Append(count);
                label.Append("-");
                label.Append(g.Key);
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

                keyString += o.GetFieldValue(_insulation, false).Convert<string>(string.Empty);
                if (o.GetFieldValue(_phaseDesignation, false).Convert<string>(string.Empty) != "12") //UNKNOWN
                {
                    keyString += " (" + o.GetFieldValue(_phaseDesignation, true).Convert<string>(string.Empty) + ")";
                }
                keyString += " (" + o.GetFieldValue(_rating, true).Convert<string>(string.Empty) + ")";
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
                label.Append(count);
                label.Append("-");
                label.Append(g.Key);
            }
            //get value for operaing voltage
            var operatingVoltage = obj.GetFieldValue(_operatingVoltage,true);
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

            // label text 2
            label = new StringBuilder();
            foreach (var o in conductorInfoObjs)
            {
                if (label.Length > 0)
                {
                    label.Append(" & ");
                }
                //get value for conductor size
                //sanjeev change getfieldvalue false to get the description value
                var size = o.GetFieldValue(_conductorSize,false);
                _logger.Debug(_conductorSize + " : " + size);
                label.Append(size);

                //label.Append(" ");
                //get value for material
                //sanjeev change getfieldvalue false to get the description value
                var material = o.GetFieldValue(_material,false);
                _logger.Debug(_material + " : " + material);
                label.Append(material);

                label.Append(" ");
                //get value for insulation
                //sanjeev change getfieldvalue false to get the description value
                var insulation = o.GetFieldValue(_insulation,false);
                _logger.Debug(_insulation + " : " + insulation);
                label.Append(insulation);
            }

            labelText2 = label.ToString();
        }

        #region Pending refactored version of GetLabelText
        /* 
        /// <summary>
        ///   Gets the label text given the specific object.
        /// </summary>
        /// <param name="obj"> The object that triggered the event. </param>
        /// <param name="autoUpdaterMode"> The auto updater mode. </param>
        /// <param name="editEvent"> The edit event. </param>
        /// <param name="labelIndex">The index of the label that should be returned.</param>
        /// <returns> The label text that will be stored on the field assigned the <see
        ///    cref="SchemaInfo.General.FieldModelNames.LabelText" /> field model name, or null if no text should be assigned. </returns>
        protected override string GetLabelText2(IObject obj, mmAutoUpdaterMode autoUpdaterMode, mmEditEvent editEvent, int labelIndex)
        {
            var conductor = new Conductor(obj);
            var stringBuilder = new StringBuilder();
            switch (labelIndex)
            {
                case 0:
                    var nonNeutrals = conductor.Info
                                               .Where(info => !info.IsNeutral)
                                               .GroupBy(info => info.Size
                                                                + ' ' + info.Material
                                                                + '(' + info.Insulation + ')');
                    stringBuilder.Append(nonNeutrals.Select(g => g.Sum(info => info.ConductorCount).ToString() + '-' + g.Key)
                                                    .Concatenate(" & "));
                    var neutrals = conductor.Info
                                            .Where(info => info.IsNeutral)
                                            .GroupBy(info => info.Size
                                                             + info.Material
                                                             + info.Insulation
                                                             + " (" + info.PhaseDesignation + ')'
                                                             + " (" + info.Rating + ')');
                    stringBuilder.Append(neutrals.Select(g => g.Sum(info => info.ConductorCount).ToString() + '-' + g.Key)
                                                 .Concatenate(" & "));

                    //get value for operaing voltage
                    var operatingVoltage = obj.GetFieldValue(_operatingVoltage);
                    Logger.Debug(_operatingVoltage + " : " + operatingVoltage);
                    stringBuilder.Append(' ');
                    stringBuilder.Append(conductor.OperatingVoltage);
                    //get value for joint trench
                    var jointTrench = obj.GetFieldValue(_jointTrench, false).Convert<string>(string.Empty);
                    Logger.Debug(_jointTrench + " : " + jointTrench);
                    if (conductor.IsJointTrench)
                    {
                        stringBuilder.Append(" JT");
                    }
                    return stringBuilder.ToString();
                case 1:
                    stringBuilder.Append(conductor.Info
                                                  .Select(info => info.Size
                                                                  + info.Material
                                                                  + ' ' + info.Insulation)
                                                  .Concatenate(" & "));
                    return stringBuilder.ToString();
            }
            return String.Empty;
        }

        #region Nested Types
        class Conductor
        {
            private const string FieldOperatingVoltage = "OperatingVoltage";
            private const string FieldJointTrenchIndicator = "JointTrenchIdc";

            IObject _obj;
            public Conductor(IObject obj)
            {
                _obj = obj;
            }

            private IEnumerable<Information> _info;
            public IEnumerable<Information> Info
            {
                get
                {
                    return _info
                        ?? (_info =
                            _obj.GetRelatedObjects(relationshipRole: esriRelRole.esriRelRoleOrigin,
                                                   modelNames: SchemaInfo.General.ClassModelNames.LabelTextUnit)
                                .Select(o=>new Information(o)));
                }
            }

            private bool? _isJointTrench;
            public bool IsJointTrench
            {
                get
                {
                    return _isJointTrench
                        ?? (_isJointTrench = _obj.GetFieldValue(FieldJointTrenchIndicator, false)
                                                 .Convert(string.Empty)
                                                 .Contains("y", StringComparison.InvariantCultureIgnoreCase)).Value;
                }
            }

            private string _operatingVoltage;
            public string OperatingVoltage
            {
                get
                {
                    return _operatingVoltage
                        ?? (_operatingVoltage = _obj.GetFieldValue(FieldOperatingVoltage).Convert(string.Empty));
                }
            }

            public class Information
            {
                private const string FieldPhaseDesignation = "PhaseDesignation";
                private const string FieldConductorSize = "ConductorSize";
                private const string FieldMaterial = "Material";
                private const string FieldInsulation = "Insulation";
                private const string FieldRating = "Rating";
                private const string FieldConductorCount = "ConductorCount";

                IObject _obj;
                public Information(IObject obj)
                {
                    _obj = obj;
                }

                public bool IsNeutral
                {
                    get
                    {
                        return PhaseDesignationCode > 7;
                    }
                }

                private int? _phaseDesignationCode;
                public int PhaseDesignationCode
                {
                    get
                    {
                        return _phaseDesignationCode
                            ?? (_phaseDesignationCode =
                                _obj.GetFieldValue(FieldPhaseDesignation, false)
                                    .Convert<int>()).Value;
                    }
                }

                private int? _conductorCount;
                public int ConductorCount
                {
                    get
                    {
                        return _conductorCount
                            ?? (_conductorCount = _obj.GetFieldValue(FieldConductorCount, false)
                                                      .Convert<int>()).Value;
                    }
                }

                private string _size;
                public string Size
                {
                    get
                    {
                        return _size
                            ?? (_size = _obj.GetFieldValue(FieldConductorSize).Convert<string>(string.Empty));
                    }
                }

                private string _material;
                public string Material
                {
                    get
                    {
                        return _material
                            ?? (_material = _obj.GetFieldValue(FieldMaterial).Convert<string>(string.Empty));
                    }
                }

                private string _insulation;
                public string Insulation
                {
                    get
                    {
                        return _insulation
                            ?? (_insulation = _obj.GetFieldValue(FieldInsulation).Convert<string>(string.Empty));
                    }
                }

                private string _phaseDesignation;
                public string PhaseDesignation
                {
                    get
                    {
                        return _phaseDesignation
                            ?? (_phaseDesignation = _obj.GetFieldValue(FieldPhaseDesignation).Convert<string>(string.Empty));
                    }
                }

                private string _rating;
                public string Rating
                {
                    get
                    {
                        return _rating
                            ?? (_rating = _obj.GetFieldValue(FieldRating).Convert<string>(string.Empty));
                    }
                }
            }
        }
        #endregion
        // */
        #endregion
    }
}
