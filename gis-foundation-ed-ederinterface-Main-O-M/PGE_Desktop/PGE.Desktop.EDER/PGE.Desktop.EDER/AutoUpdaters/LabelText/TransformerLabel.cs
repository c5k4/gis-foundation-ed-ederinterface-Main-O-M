using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using Miner.ComCategories;
using Miner.Interop;
using PGE.Common.Delivery.Framework;
using System.Reflection;
using log4net;
using PGE.Common.Delivery.Diagnostics;

namespace PGE.Desktop.EDER.AutoUpdaters.LabelText
{
    /// <summary>
    /// Build label text and set UnitsRatedKVAs value.
    /// </summary>
    [Guid("8768C73D-8A4A-4F44-8470-11A115B6A261")]
    [ProgId("PGE.Desktop.EDER.TransformerLabel")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    public class TransformerLabel : BaseLabelTextAU
    {
        #region Class Variables
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        private const string _phaseDesig = "PhaseDesignation";
        private const string _ratedKVA = "RatedKVA";
        private const string _lowsideVoltage = "LowsideVoltage";
        private const string _numberOfPhases = "NumberOfPhases";
        private const string _operatingVoltage = "OperatingVoltage";
        private const string _stabilizingBankIDC = "StabilizingBankIDC";
        private const string _groundedIDC = "GroundedIDC";
        private const string _regulatedOutputIDC = "RegulatedOutputIDC";
        //EGIS-695 Change
        private const string _transformerType = "TRANSFORMERTYPE";

        private string _labelText2;
        #endregion
        enum Subtype
        {
            None,
            DistributionOverhead,
            DistributionSubsurface,
            DistributionPadmount,
            Equipment,
            NetworkSubsurface,
            NetworkPadmount,
            StreetLight,
            Secondary
        }

        /// <summary>
        /// Constructor, pass in name and the number of label texts this AU will build.
        /// </summary>
        public TransformerLabel()
            : base("PGE Transformer LabelText AU", 2)
        {

        }

        /// <summary>
        ///   Implementation of AutoUpdater Enabled method for derived classes.
        /// </summary>
        /// <param name="objectClass"> The object class. </param>
        /// <param name="editEvent"> The edit event. </param>
        /// <returns> <c>true</c> if the AuoUpdater should be enabled; otherwise <c>false</c> </returns>
        /// <remarks>
        ///   This method will be called from IMMSpecialAUStrategy::get_Enabled and is wrapped within the exception handling for that method.
        /// </remarks>
        protected override bool InternalEnabled(IObjectClass objectClass, mmEditEvent editEvent)
        {
            bool enabled = false;
            bool isTransformerBankClass = ModelNameFacade.ContainsAllClassModelNames(objectClass,
                    new string[] { SchemaInfo.Electric.ClassModelNames.Transformer, SchemaInfo.General.ClassModelNames.LabelTextBank }
                    );

            if (editEvent == mmEditEvent.mmEventFeatureCreate)
            {
                enabled = isTransformerBankClass;
            }
            else if (editEvent == mmEditEvent.mmEventFeatureUpdate)
            {
                bool isTransformerUnitClass = ModelNameFacade.ContainsAllClassModelNames(objectClass,
                    new string[] { SchemaInfo.Electric.ClassModelNames.TransformerUnit, SchemaInfo.General.ClassModelNames.LabelTextUnit }
                    );

                enabled = isTransformerBankClass || isTransformerUnitClass;
            }

            return enabled;
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
            switch (labelIndex)
            {
                case 0:
                    string labelText;
                    BuildLabelTexts(obj, out labelText, out _labelText2);
                    _logger.Debug("labelText :" + labelText);
                    return labelText;
                case 1:
                    _logger.Debug("labelText2 :" + _labelText2);
                    return _labelText2;
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
            var subtype = obj.SubtypeCodeAsEnum<Subtype>();

            var unitsRatedKVAs = string.Empty;

            var relatedUnits = base.GetRelatedObjects(obj, null, esriRelRole.esriRelRoleOrigin, SchemaInfo.General.ClassModelNames.LabelTextUnit);
            unitsRatedKVAs = relatedUnits.Select(o => o.GetFieldValue(_ratedKVA, false).Convert(string.Empty)).Concatenate("/");

            //As per bug 843 if lowsidevoltage is 20,22,23,99 then do not show.
            var lowSideVoltageNotShow = new int[] { 20, 22, 23, 99 };
            //get value for lowsideVoltage
            var lowsideVoltage = obj.GetFieldValue(_lowsideVoltage, true).Convert(string.Empty);
            _logger.Debug(_lowsideVoltage + " : " + lowsideVoltage);

            //get value for numberOfPhases

            //#Remedy Item:  INC000004255623 Schematics: Single phase and 2 phase should not be labeled on the maps.  Finding the Number of phases of transformer unit instead of the transformer.       
            //var numberOfPhases = relatedUnits.Select(o => o.GetFieldValue(_numberOfPhases, false)).Convert(0);
            var numberOfPhases = obj.GetFieldValue(_numberOfPhases, false).Convert(0);
            _logger.Debug(_numberOfPhases + " : " + numberOfPhases);
            //var operatingNumber = obj.GetFieldValue("OperatingNumber").Convert(string.Empty); // Moved to separate class

            //get value for operating voltage
            var operatingVoltage = obj.GetFieldValue(_operatingVoltage, true).Convert(string.Empty);
            _logger.Debug(_operatingVoltage + " : " + operatingVoltage);
            StringBuilder builder = new StringBuilder();

            switch (subtype)
            {
                case Subtype.DistributionOverhead:
                    {
                        if (relatedUnits.Count<IObject>() == 3)
                        {
                            //Changing per PG&E request TFS ID 7927
                            //builder.Append(unitsRatedKVAs + "\r\n");
                            builder.Append(unitsRatedKVAs);
                            if (numberOfPhases > 2)
                            {
                                if (!string.IsNullOrEmpty(unitsRatedKVAs)) builder.Append(" ");
                                //builder.Append("3\u00D8");
                                //Changing per PG&E request TFS ID 7927
                                builder.Append("3ɸ");
                            }
                            builder.Append("\r\n");
                            if (!lowSideVoltageNotShow.Contains(obj.GetFieldValue(_lowsideVoltage, false).Convert<int>(0)))// lowsideVoltage.Contains("120/240") == false)
                            {
                                builder.Append(lowsideVoltage);
                            }
                            builder.Append("\r\n");
                            //get value for stabilizingBankIdx
                            var stabilizingBankIdx = obj.GetFieldValue(_stabilizingBankIDC, false)
                                                        .Convert("N")
                                                        .ToUpperInvariant() == "Y";
                            _logger.Debug(_stabilizingBankIDC + " : " + stabilizingBankIdx);

                            //get value for groundingIdc
                            var groundingIdc = obj.GetFieldValue(_groundedIDC, false)
                                                  .Convert("N")
                                                  .ToUpperInvariant() == "Y";
                            _logger.Debug(_groundedIDC + " : " + groundingIdc);
                            if (stabilizingBankIdx) builder.Append("STB");
                            if (builder.ToString().Contains("STB"))
                            {
                                if (groundingIdc) builder.Append(" GRD");
                            }
                            else
                            {
                                if (groundingIdc) builder.Append("GRD");
                            }
                        }
                        else
                        {
                            //builder.Append(unitsRatedKVAs + "\r\n");
                            //Changing per PG&E request TFS ID 7927
                            builder.Append(unitsRatedKVAs);
                            if (numberOfPhases > 2)
                            {
                                if (!string.IsNullOrEmpty(unitsRatedKVAs)) builder.Append(" ");
                                //builder.Append(numberOfPhases);
                                builder.Append(numberOfPhases + "ɸ");
                            }
                            builder.Append("\r\n");
                            if (!lowSideVoltageNotShow.Contains(obj.GetFieldValue(_lowsideVoltage, false).Convert<int>(0)))
                            {
                                builder.Append(lowsideVoltage + "\r\n");
                            }
                        }

                        break;
                    }
                case Subtype.DistributionSubsurface:
                    {
                        //EGIS-695 : Change for Duplex Transformer of subtype Subsurface or Padmount subtype forshowing splitted unit rated kva
                        if (relatedUnits.Count<IObject>() == 1)
                        {
                            var transUnitType = string.Empty;
                            transUnitType = relatedUnits.Select(o => o.GetFieldValue(_transformerType, false).Convert(string.Empty)).Concatenate("/");
                            if (transUnitType != null)
                            {
                                if (transUnitType.ToString() == "21" || transUnitType.ToString() == "32")
                                {
                                    unitsRatedKVAs = GetSplittedRatedUnitKVA(unitsRatedKVAs);
                                }
                            }
                        }
                        builder.Append(unitsRatedKVAs);

                        //Change 10/16/2013 add number of phases and low side voltage 
                        //TFS 9761/INC000003696233 
                        if (numberOfPhases > 2)
                        {
                            if (!string.IsNullOrEmpty(unitsRatedKVAs)) builder.Append(" ");
                            builder.Append("3ɸ");
                        }                            
                        builder.Append("\r\n");
                        
                        if (!lowSideVoltageNotShow.Contains(obj.GetFieldValue(_lowsideVoltage, false).Convert<int>(0)))
                        {
                            builder.Append(lowsideVoltage);
                        }

                        break;
                    }
                case Subtype.DistributionPadmount:
                    {
                        //EGIS-695 : Change for Duplex Transformer of subtype Subsurface or Padmount subtype forshowing splitted unit rated kva
                        if (relatedUnits.Count<IObject>() == 1)
                        {
                            var transUnitType = relatedUnits.Select(o => o.GetFieldValue(_transformerType, false).Convert(string.Empty)).Concatenate("/");
                            if (transUnitType != null)
                            {
                                if (transUnitType.ToString() == "21" || transUnitType.ToString() == "32")
                                {
                                    unitsRatedKVAs = GetSplittedRatedUnitKVA(unitsRatedKVAs);
                                }
                            }
                        }
                        builder.Append(unitsRatedKVAs);

                        //Change 10/16/2013 add number of phases and low side voltage 
                        //TFS 9761/INC000003696233  
                        if (numberOfPhases > 2)
                        {
                            if (!string.IsNullOrEmpty(unitsRatedKVAs)) builder.Append(" ");
                            builder.Append("3ɸ");
                        }
                        builder.Append("\r\n"); 
                        if (!lowSideVoltageNotShow.Contains(obj.GetFieldValue(_lowsideVoltage, false).Convert<int>(0)))
                        {
                            builder.Append(lowsideVoltage);
                        }
                        break;
                    }
                case Subtype.Equipment:
                    {
                        builder.Append(unitsRatedKVAs);

                        //Change 10/16/2013 add number of phases and low side voltage 
                        //TFS 9761/INC000003696233  
                        if (numberOfPhases > 2)
                        {
                            if (!string.IsNullOrEmpty(unitsRatedKVAs)) builder.Append(" ");
                            builder.Append("3ɸ");
                        }
                        builder.Append("\r\n");
                        if (!lowSideVoltageNotShow.Contains(obj.GetFieldValue(_lowsideVoltage, false).Convert<int>(0)))
                        {
                            builder.Append(lowsideVoltage);
                        }
                        break;
                    }
                case Subtype.NetworkSubsurface:
                    {
                        builder.Append(unitsRatedKVAs);

                        //Change 10/16/2013 add number of phases and low side voltage 
                        //TFS 9761/INC000003696233  
                        if (numberOfPhases > 2)
                        {
                            if (!string.IsNullOrEmpty(unitsRatedKVAs)) builder.Append(" ");
                            builder.Append("3ɸ");
                        }
                        builder.Append("\r\n");
                        if (!lowSideVoltageNotShow.Contains(obj.GetFieldValue(_lowsideVoltage, false).Convert<int>(0)))
                        {
                            builder.Append(lowsideVoltage);
                        }
                        break;
                    }
                case Subtype.NetworkPadmount:
                    {
                        builder.Append(unitsRatedKVAs);

                        //Change 10/16/2013 add number of phases and low side voltage 
                        //TFS 9761/INC000003696233  
                        if (numberOfPhases > 2)
                        {
                            if (!string.IsNullOrEmpty(unitsRatedKVAs)) builder.Append(" ");
                            builder.Append("3ɸ");
                        }
                        builder.Append("\r\n");
                        if (!lowSideVoltageNotShow.Contains(obj.GetFieldValue(_lowsideVoltage, false).Convert<int>(0)))
                        {
                            builder.Append(lowsideVoltage);
                        }
                        break;
                    }
                case Subtype.StreetLight:
                    {
                        builder.Append(unitsRatedKVAs);

                        //Change 10/16/2013 add number of phases and low side voltage 
                        //TFS 9761/INC000003696233  
                        if (numberOfPhases > 2)
                        {
                            if (!string.IsNullOrEmpty(unitsRatedKVAs)) builder.Append(" ");
                            builder.Append("3ɸ");
                        }
                        builder.Append("\r\n");
                        if (!lowSideVoltageNotShow.Contains(obj.GetFieldValue(_lowsideVoltage, false).Convert<int>(0)))
                        {
                            builder.Append(lowsideVoltage);
                        }

                        //get value for regulatedOutputIdc
                        var regulatedOutputIdc = obj.GetFieldValue(_regulatedOutputIDC, false)
                                                    .Convert("N")
                                                    .ToUpperInvariant() == "Y";
                        _logger.Debug(_regulatedOutputIDC + " : " + regulatedOutputIdc);
                        if (regulatedOutputIdc) builder.Append(" R.O.");

                        break;
                    }
                case Subtype.Secondary:
                    {
                        builder.Append(unitsRatedKVAs);

                        //Change 10/16/2013 add number of phases TFS 9761/INC000003696233 
                        if (numberOfPhases > 2)
                        {
                            if (!string.IsNullOrEmpty(unitsRatedKVAs)) builder.Append(" ");
                            builder.Append("3ɸ");
                        }
                        builder.Append("\r\n");
                        if (!lowSideVoltageNotShow.Contains(obj.GetFieldValue(_lowsideVoltage, false).Convert<int>(0)))
                        {
                            builder.Append(lowsideVoltage);
                        }
                        break;
                    }
            }
            labelText = builder.ToString();
            _logger.Debug("Attribute Value: " + labelText);

            //#Remedy Item:  INC000004255623 Schematics: Single phase and 2 phase should not be labeled on the maps.  Finding the Number of phases of transformer unit instead of the transformer.       
            // Start of IBM code change for Schematics Anno - 8/22/2013
            labelText2 = unitsRatedKVAs + " " + (numberOfPhases > 2 ? 3 : 1).ToString() + "Ø";
            // End of IBM code change for Schematics Anno - 8/22/2013

        }
        //EGIS-695 : This method is created based on DA attached in EGIS-695 for getting splitted unit kva for duplex transformer of subtype subsurface or padmount
        private string GetSplittedRatedUnitKVA(string ratedUnitKVA)
        {
            string splittedRateUnitKVA = ratedUnitKVA;
            if (ratedUnitKVA.Equals("35")) { splittedRateUnitKVA = "25/10"; }
            else if (ratedUnitKVA.Equals("60")) { splittedRateUnitKVA = "50/10"; }
            else if (ratedUnitKVA.Equals("90")) { splittedRateUnitKVA = "75/15"; }
            else if (ratedUnitKVA.Equals("125")) { splittedRateUnitKVA = "100/25"; }
            else if (ratedUnitKVA.Equals("150")) { splittedRateUnitKVA = "100/50"; }
            return splittedRateUnitKVA;

        }
    }
}