using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Miner.ComCategories;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Diagnostics;
using System.Reflection;
using ESRI.ArcGIS.Geodatabase;
using Miner.Interop;
using PGE.Common.Delivery.Framework;

namespace PGE.Desktop.EDER.AutoUpdaters.Special
{
    /// <summary>
    /// Inherit operating voltage.
    /// </summary>
    [ComVisible(true)]
    [Guid("AC968D43-B84F-44DA-9871-63B6BE4D0E9C")]
    [ProgId("PGE.Desktop.EDER.InheritOperatingVoltageAU")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    public class PGEInheritOperatingVoltagerAU : BaseSpecialAU
    {

        #region Private Static readonly fields
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        // Q1-2021 ED Scripting updated model name so it could be executed on FaultIndicators. 
        private static readonly string _operatingVoltageModelName = SchemaInfo.Electric.FieldModelNames.OperatingVoltage4PrimaryValidation;
        private static readonly string _phaseDesignationModelName = SchemaInfo.Electric.FieldModelNames.PhaseDesignation;
        private static readonly string _secondaryVoltageModelName = SchemaInfo.Electric.FieldModelNames.SecondaryVoltage;
        private static readonly string[] _doNotUpdateForModelNames = { SchemaInfo.Electric.ClassModelNames.PGETransformer, SchemaInfo.Electric.ClassModelNames.PrimaryUGConductor, SchemaInfo.Electric.ClassModelNames.PrimaryOHConductor };
        private static readonly string[] _primaryConductorModelNames = { SchemaInfo.Electric.ClassModelNames.PrimaryUGConductor, SchemaInfo.Electric.ClassModelNames.PrimaryOHConductor };
        private static readonly string[] _secondaryConductorModelNames = { SchemaInfo.Electric.ClassModelNames.SecondaryOHConductor,SchemaInfo.Electric.ClassModelNames.SecondaryUGConductor };
        private static readonly string _defaultValue = "12.0 kv";

        #endregion

        #region Constructor / Desctructor

        /// <summary>
        /// Constructor, pass in name.
        /// </summary>
        /// 
        public PGEInheritOperatingVoltagerAU()
            : base("PGE Inherit Operating Voltage AU") {}

        #endregion

        #region Base special AU Overrides
        /// <summary>
        /// Determines in which class the AU will be enabled
        /// </summary>
        /// <param name="objectClass"> Object's class. </param>
        /// <param name="eEvent">The edit event.</param>
        /// <returns> <c>true</c> if the AuoUpdater should be enabled; otherwise <c>false</c> </returns>
        protected override bool InternalEnabled(IObjectClass objectClass, mmEditEvent eEvent)
        {
            bool enabled = false;

            if (eEvent == mmEditEvent.mmEventFeatureCreate || eEvent == mmEditEvent.mmEventFeatureUpdate)
            {
                // enabled if object class contains the field model name "PGE_OperatingVoltage")
                enabled = ModelNameFacade.ContainsFieldModelName(objectClass, _operatingVoltageModelName);
                _logger.Debug("field model name :" + _operatingVoltageModelName + " Found-" + enabled);
            }

            return enabled;
        }

        /// <summary>
        /// Determines whether actually this AU should be run, based on the AU Mode.
        /// </summary>
        /// <param name="eAUMode"> The auto updater mode. </param>
        /// <returns> <c>true</c> if the AuoUpdater should be executed; otherwise <c>false</c> </returns>
        protected override bool CanExecute(mmAutoUpdaterMode eAUMode)
        {
            if (eAUMode == mmAutoUpdaterMode.mmAUMArcMap)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Implementation of AutoUpdater Execute Ex method for derived classes.
        /// </summary>
        /// <param name="obj">The object that triggered the AutoUpdater.</param>
        /// <param name="eAUMode">The auto updater mode.</param>
        /// <param name="eEvent">The edit event.</param>
        protected override void InternalExecute(IObject obj, mmAutoUpdaterMode eAUMode, mmEditEvent eEvent)
        {
            // check if event is create or update
            if (eEvent == mmEditEvent.mmEventFeatureCreate || eEvent == mmEditEvent.mmEventFeatureUpdate)
            {
                // if event is update then check whether shape is changed or not.
                // if shape is changed then inherit voltage from connected source line.
                // else do nothing.
                if (eEvent == mmEditEvent.mmEventFeatureUpdate && ModelNameFacade.ContainsClassModelName(obj.Class, _doNotUpdateForModelNames) == false)
                {
                    // Q1-2021 Voltage change for ADMS - GIS Scripting project. Executing when phase designation
                    // changes so that voltage will reflect transitions from Multi-Phase to Single-Phase.
                    IRowChanges rowChanges = obj as IRowChanges;
                    int phaseIndex = ModelNameFacade.FieldIndexFromModelName(obj.Class, _phaseDesignationModelName);
                    bool phaseChanged = rowChanges.get_ValueChanged(phaseIndex);

                    IFeatureChanges objectFeatureChanges = obj as IFeatureChanges;
                    if (objectFeatureChanges.ShapeChanged || phaseChanged)
                    {
                        //inherit operating voltage
                        UpdateVoltage(obj);
                    }
                }
                else if (eEvent == mmEditEvent.mmEventFeatureCreate) // Feature create event
                {
                    // inherit operating voltage
                    UpdateVoltage(obj);
                }
            }
        }
        #endregion

        #region private method
        /// <summary>
        /// Update the default value 
        /// </summary>
        /// <param name="fieldIndex">index of the field</param>
        /// <param name="obj">IObject</param>
        private void UpdateDefaultValue(int fieldIndex, IObject obj)
        {
            // Checking if the default value is matching with the domain assign in the obj voltage field.
            object objValue = MatchDomainDescriptionAndReturnValue(obj,obj.Fields.get_Field(fieldIndex), _defaultValue);

            //if matched then update
            if (objValue != null)
            {
                obj.set_Value(fieldIndex, objValue);
               
            }
            else
            {
                _logger.Warn("The value " + _defaultValue + " not found in the domain assign to the field model name" + _operatingVoltageModelName);
            }
            return;
        }
        /// <summary>
        /// This method will update the operating voltage of a point feature.
        /// </summary>
        /// <param name="obj">Point Object</param>
        private void UpdatePoint(IObject obj)
        {
            // find the connected upstream edge feature.
            IFeature connectedSourceLine = TraceFacade.GetFirstUpstreamEdge(obj as IFeature);
            
            int incommingOVIndex = ModelNameFacade.FieldIndexFromModelName(obj.Class, _operatingVoltageModelName);
            if (incommingOVIndex < 0) { _logger.Warn("Field with model name " + _operatingVoltageModelName + " not found in the class '" + obj.Class.AliasName + "'"); return; }

            /////added by rizuan on 3/1/2013/////////
            //Check wheteher operating volatge is null
            Func<IObject, bool> opVoltageValue = objValue => objValue.GetFieldValue(null, false, _operatingVoltageModelName).Convert<object>(System.DBNull.Value) == System.DBNull.Value ? true : false;

            // Q1-2021 Voltage change for ED-GIS Scripting project. Voltage drop values used when phase transitions from Multi-Phase to Single-Phase.
            string phaseValue = obj.GetFieldValue(null, true, _phaseDesignationModelName).Convert<string>(string.Empty);
            

            //Special case. if no connected source feature then update to default 12 kv.
            if (connectedSourceLine == null)
            {
                _logger.Debug("No any connect source line found.Setting to default 12.0 KV");
                //set the 12 kv. Checking multiple value because of diffrent layer can have diffrent domain assign to the field.
                // update the default value.

                if (opVoltageValue(obj) == true) UpdateDefaultValue(incommingOVIndex, obj);
               
                return;
            }


            // get the operating voltage field index.
            int connectedSourceLineOVIndex = ModelNameFacade.FieldIndexFromModelName(connectedSourceLine.Class, _operatingVoltageModelName);

            // If Operating voltage field not found in the sourceconnected line then update the default value in point (obj) and return;
            if (connectedSourceLineOVIndex < 0)
            {
                _logger.Warn("Field with model name " + _operatingVoltageModelName + " not found in the class '" + connectedSourceLine.Class.AliasName + "'");
                if (opVoltageValue(obj) == true) UpdateDefaultValue(incommingOVIndex, obj);
                return;
            }

            //Match the value in the obj operating voltage field and then update
            object objectValue = connectedSourceLine.get_Value(connectedSourceLineOVIndex);
            // Q1-2021 Voltage change for ED-GIS Scripting project. Voltage drop  values used when phase transitions from Multi-Phase
            // to Single-Phase.
            string upstreamPhaseValue = connectedSourceLine.GetFieldValue(null, true, _phaseDesignationModelName).Convert<string>(string.Empty);
            string upstreamVoltageDescription = connectedSourceLine.GetFieldValue(null, true, _operatingVoltageModelName).Convert<string>(string.Empty);
            if (objectValue.Convert(String.Empty) != string.Empty)
            {
                // Get new value which considers if voltage drop is needed.
                int newVoltageValue = -1;
                string newVoltageDescription = "";
                GetNewVoltageBasedOnPhase(objectValue.Convert<int>(-1), upstreamVoltageDescription, upstreamPhaseValue, phaseValue, out newVoltageValue, out newVoltageDescription);
                int existingVoltageValue = obj.get_Value(incommingOVIndex).Convert<int>(-1);
                // If the existing value is not right then update it.
                if (newVoltageValue != existingVoltageValue)
                {
                    // objectValue = MatchDomainDescriptionAndReturnValue(obj, obj.Fields.get_Field(incommingOVIndex), connectedSourceLine.GetFieldValue(null, true, _operatingVoltageModelName).ToString());
                    objectValue = MatchDomainDescriptionAndReturnValue(obj, obj.Fields.get_Field(incommingOVIndex), newVoltageDescription);
                    if (objectValue != null) { obj.set_Value(incommingOVIndex, objectValue); }
                    else { _logger.Warn("Voltage of " + connectedSourceLine.Class.AliasName + " is not available in the domain assign to the voltage field  to  " + obj.Class.AliasName); }
                }
                else
                {
                    _logger.Debug("The New Voltage: " + newVoltageValue + " already matches this feature: Class: " + obj.Class.AliasName + " OID: " + obj.OID + " Voltage: " + existingVoltageValue + " Phase: " + phaseValue + ". SourceInfo: Class: " + connectedSourceLine.Class.AliasName + " OID: " + connectedSourceLine.OID + " Voltage: " + objectValue.Convert<int>(-1) + " Phase: " + upstreamPhaseValue);
                }
            }
            else
            {
                if (opVoltageValue(obj) == true) UpdateDefaultValue(incommingOVIndex, obj);
                return;
            }

            //If step down/Transformer then Update the down stream line also....
            // IF feature is creating on the line , It will split the line so that the AU for splited line will run before then this object created
            // and at that time no connected source point will be available for the split line and the AU will inherit the operating voltage from the connected upstream Edge.
            // So that updating the split line downstream here.
            if (ModelNameFacade.ContainsClassModelName(obj.Class, SchemaInfo.Electric.ClassModelNames.StepDown)
                || ModelNameFacade.ContainsClassModelName(obj.Class, SchemaInfo.Electric.ClassModelNames.PGETransformer)
                || ModelNameFacade.ContainsClassModelName(obj.Class, SchemaInfo.Electric.ClassModelNames.PrimaryMeter))
            {
                List<IFeature> downStreamEdges = TraceFacade.GetFirstDownstreamEdges(obj as IFeature);
                if (downStreamEdges.Count() > 0)
                {
                    foreach (IFeature connecteddownstreamLine in downStreamEdges)
                    {
                        UpdateLine(connecteddownstreamLine as IObject, true);
                    }
                }
            }
        }

        /// <summary>
        /// This method will update the operating voltage of a line feature
        /// </summary>
        /// <param name="obj">Line object</param>
        private void UpdateLine(IObject obj, bool callStore = false)
        {
            // find the connected upstream point feature.
            IFeature connectedSourcePoint = TraceFacade.GetFirstUpstreamJunction(obj as IFeature);

            int incommingOVIndex = ModelNameFacade.FieldIndexFromModelName(obj.Class, _operatingVoltageModelName);
            if (incommingOVIndex < 0) { _logger.Warn("Field with model name " + _operatingVoltageModelName + " not found in the class '" + obj.Class.AliasName + "'"); return; }

            // get the operating voltage field index.
            object operatingVoltageValue = null;
            object operatingVoltageDescription = null;
            string upstreamPhaseValue = null;
            IFeature connectedUpstreamFeatureLine = TraceFacade.GetFirstUpstreamEdge(obj as IFeature);

            /////added by rizuan on 3/1/2013/////////
            Func<IObject, bool> chkOperatingVoltage = objectValue => objectValue.GetFieldValue(null, false, _operatingVoltageModelName).Convert<object>(System.DBNull.Value) == System.DBNull.Value ? true : false;
          

            //Check the line feature class.
            if (ModelNameFacade.ContainsClassModelName(obj.Class, SchemaInfo.Electric.ClassModelNames.PGEBusBar))
            {
                #region Case for Busbar
                // If the incomming line is busbar and its subtype is primary then it will inherit the operating voltage from upstream line.
                if (obj.HasSubtypeCode(2)) // SecBusBar
                {
                    if (connectedSourcePoint != null)
                    {
                        if (ModelNameFacade.ContainsClassModelName(connectedSourcePoint.Class, SchemaInfo.Electric.ClassModelNames.PGETransformer))
                        {
                            operatingVoltageValue = connectedSourcePoint.GetFieldValue(null, false, _secondaryVoltageModelName);
                            operatingVoltageDescription = connectedSourcePoint.GetFieldValue(null, true, _secondaryVoltageModelName);
                        }
                        else
                        {
                            if (connectedUpstreamFeatureLine == null)
                            {

                                // update the default value.
                                if (chkOperatingVoltage(obj) == true) UpdateDefaultValue(incommingOVIndex, obj);
                                return;

                            }
                            else
                            {
                                operatingVoltageValue = connectedUpstreamFeatureLine.GetFieldValue(null, false, _operatingVoltageModelName);
                                operatingVoltageDescription = connectedUpstreamFeatureLine.GetFieldValue(null, true, _operatingVoltageModelName);
                            }
                        }
                    }
                    else
                    {
                        // update the default value.
                        if (chkOperatingVoltage(obj) == true) UpdateDefaultValue(incommingOVIndex, obj);
                        return;
                    }
                }
                else
                {
                    // Primary BusBar.
                    if (connectedUpstreamFeatureLine == null)
                    {
                        // update the default value.
                        if (chkOperatingVoltage(obj) == true) UpdateDefaultValue(incommingOVIndex, obj);
                        return;
                    }
                    else
                    {
                        // Q1-2021 Voltage change for ED-GIS Scripting project. Voltage drop values used when
                        // phase transitions from Multi-Phase to Single-Phase.
                        upstreamPhaseValue = connectedUpstreamFeatureLine.GetFieldValue(null, true, _phaseDesignationModelName).Convert<string>(string.Empty);
                        operatingVoltageValue = connectedUpstreamFeatureLine.GetFieldValue(null, false, _operatingVoltageModelName);
                        operatingVoltageDescription = connectedUpstreamFeatureLine.GetFieldValue(null, true, _operatingVoltageModelName);
                    }
                }
                #endregion
            }
            else if (ModelNameFacade.ContainsClassModelName(obj.Class, _primaryConductorModelNames))
            {
                #region Case for Primary Conductor
                //If upstream connectedSourcePoint not found then inherit the operating voltage from upstream line.
                // If upstream connectedSourcePoint found and it is not the step down or electricstich point subtype = source then inherit the operatinv voltage from upstream line.
                // If upstream connectedsourcepoint found and it is stepdown or electricstich point subtype = source then inherit from the connectedsource point.
                if (connectedSourcePoint == null && connectedUpstreamFeatureLine == null)
                {
                    // update default value.   
                    if (chkOperatingVoltage(obj) == true) UpdateDefaultValue(incommingOVIndex, obj);
                    return;
                }
                else if(connectedSourcePoint != null)
                {
                    if (ModelNameFacade.ContainsClassModelName(connectedSourcePoint.Class,SchemaInfo.Electric.ClassModelNames.ElectricStitchPoint))
                    {
                        // If subtype is source then inherit the voltage from electricStichPoint else update from line.
                        if (connectedSourcePoint.HasSubtypeCode(2)) // subtyype 2 is Source.
                        {
                            // Q1-2021 Voltage change for ED-GIS Scripting project. Voltage drop values used when phase
                            // transitions from Multi-Phase to Single-Phase.
                            upstreamPhaseValue = connectedSourcePoint.GetFieldValue(null, true, _phaseDesignationModelName).Convert<string>(string.Empty);
                            operatingVoltageValue = connectedSourcePoint.GetFieldValue(null, false, _operatingVoltageModelName);
                            operatingVoltageDescription = connectedSourcePoint.GetFieldValue(null, true, _operatingVoltageModelName);
                        }
                        else // else inherit from upstream line
                        {
                            if (connectedUpstreamFeatureLine != null)
                            {
                                // Q1-2021 Voltage change for ED-GIS Scripting project. Voltage drop values used when phase
                                // transitions from Multi-Phase to Single-Phase.
                                upstreamPhaseValue = connectedUpstreamFeatureLine.GetFieldValue(null, true, _phaseDesignationModelName).Convert<string>(string.Empty);
                                operatingVoltageValue = connectedUpstreamFeatureLine.GetFieldValue(null, false, _operatingVoltageModelName);
                                operatingVoltageDescription = connectedUpstreamFeatureLine.GetFieldValue(null, true, _operatingVoltageModelName);
                            }
                            else // if upstream line is null then update the default value.
                            {
                                //default value will be updated.
                                if (chkOperatingVoltage(obj) == true) UpdateDefaultValue(incommingOVIndex, obj);
                                return;
                            }
                        }
                    }
                    else if (ModelNameFacade.ContainsClassModelName(connectedSourcePoint.Class, SchemaInfo.Electric.ClassModelNames.StepDown))
                    {
                        //Update the primary conductor with stepdown OutPut Voltage voltage
                        // Q1-2021 Voltage change for ED-GIS Scripting project. Voltage drop values used when phase
                        // transitions from Multi-Phase to Single-Phase.
                        upstreamPhaseValue = connectedSourcePoint.GetFieldValue(null, true, _phaseDesignationModelName).Convert<string>(string.Empty);
                        operatingVoltageValue = connectedSourcePoint.GetFieldValue(null, false, _secondaryVoltageModelName);
                        operatingVoltageDescription = connectedSourcePoint.GetFieldValue(null, true, _secondaryVoltageModelName);
                    }
                    else // If source connected upstream point is not stepdown or electricstich point then update from upstream line.
                    {
                        // update from upstream line feature.
                        if (connectedUpstreamFeatureLine != null)
                        {
                            // Q1-2021 Voltage change for ED-GIS Scripting project. Voltage drop values used when phase
                            // transitions from Multi-Phase to Single-Phase.
                            upstreamPhaseValue = connectedUpstreamFeatureLine.GetFieldValue(null, true, _phaseDesignationModelName).Convert<string>(string.Empty);
                            operatingVoltageValue = connectedUpstreamFeatureLine.GetFieldValue(null, false, _operatingVoltageModelName);
                            operatingVoltageDescription = connectedUpstreamFeatureLine.GetFieldValue(null, true, _operatingVoltageModelName);
                        }
                        else // if upstream line is null then update the default value.
                        {
                            //default value will be updated.

                            if (chkOperatingVoltage(obj) == true) UpdateDefaultValue(incommingOVIndex, obj);
                            return;
                        }
                    }
                    
                }
                else 
                {
                    // Q1-2021 Voltage change for ED-GIS Scripting project. Voltage drop values used when phase
                    // transitions from Multi-Phase to Single-Phase.
                    upstreamPhaseValue = connectedUpstreamFeatureLine.GetFieldValue(null, true, _phaseDesignationModelName).Convert<string>(string.Empty);
                    operatingVoltageValue = connectedUpstreamFeatureLine.GetFieldValue(null, false, _operatingVoltageModelName);
                    operatingVoltageDescription = connectedUpstreamFeatureLine.GetFieldValue(null, true, _operatingVoltageModelName);
                }

                #endregion
            }
            else if (ModelNameFacade.ContainsClassModelName(obj.Class, _secondaryConductorModelNames))
            {
                #region Case for Secondary Conductor
                // If Upstream connected point is transformer or primary meter then update from transformer lowsidevoltage and primarymeter secondary voltage
                // If not connected upstream point found or upstream point is not a transformer or primary meter then update from upstream line.
                // if upstream line not found then update the default value
                if (connectedSourcePoint == null && connectedUpstreamFeatureLine == null)
                {
                    // update default value.  
                    if (chkOperatingVoltage(obj) == true) UpdateDefaultValue(incommingOVIndex, obj);
                     return;
                }
                else if (connectedSourcePoint != null)
                {
                    // If Upstream connected point is transformer or primary meter then update from transformer lowsidevoltage and primarymeter secondary voltage
                    if (ModelNameFacade.ContainsClassModelName(connectedSourcePoint.Class, SchemaInfo.Electric.ClassModelNames.PGETransformer)
                        || ModelNameFacade.ContainsClassModelName(connectedSourcePoint.Class, SchemaInfo.Electric.ClassModelNames.PrimaryMeter))
                    {
                        operatingVoltageValue = connectedSourcePoint.GetFieldValue(null, false, _secondaryVoltageModelName);
                        operatingVoltageDescription = connectedSourcePoint.GetFieldValue(null, true, _secondaryVoltageModelName);
                    }
                    else // update from upstream line feature
                    {
                        if (connectedUpstreamFeatureLine != null)
                        {
                            operatingVoltageValue = connectedUpstreamFeatureLine.GetFieldValue(null, false, _operatingVoltageModelName);
                            operatingVoltageDescription = connectedUpstreamFeatureLine.GetFieldValue(null, true, _operatingVoltageModelName);
                        }
                        else
                        {
                            // update default value.
                            if (chkOperatingVoltage(obj) == true) UpdateDefaultValue(incommingOVIndex, obj);
                             return;
                        }
                    }
                }
                else // update from line
                {
                    operatingVoltageValue = connectedUpstreamFeatureLine.GetFieldValue(null, false, _operatingVoltageModelName);
                    operatingVoltageDescription = connectedUpstreamFeatureLine.GetFieldValue(null, true, _operatingVoltageModelName);
                }
                #endregion
            }
            else if (ModelNameFacade.ContainsClassModelName(obj.Class, SchemaInfo.Electric.ClassModelNames.TransformerLead))
            {
                #region Transformer Lead
                if (connectedSourcePoint == null && connectedUpstreamFeatureLine == null)
                {
                    //update the default value.
                    if (chkOperatingVoltage(obj) == true) UpdateDefaultValue(incommingOVIndex, obj);
                     return;
                }
                else if (connectedSourcePoint != null)
                {
                    if (ModelNameFacade.ContainsClassModelName(connectedSourcePoint.Class, SchemaInfo.Electric.ClassModelNames.PGETransformer))
                    {
                        operatingVoltageValue = connectedSourcePoint.GetFieldValue(null, false, _secondaryVoltageModelName);
                        operatingVoltageDescription = connectedSourcePoint.GetFieldValue(null, true, _secondaryVoltageModelName);
                    }
                    else
                    {
                        if (connectedUpstreamFeatureLine == null)
                        {
                            // update the default value.
                            if (chkOperatingVoltage(obj) == true) UpdateDefaultValue(incommingOVIndex, obj);
                             return;
                        }
                        else
                        {
                            operatingVoltageValue = connectedUpstreamFeatureLine.GetFieldValue(null, false, _operatingVoltageModelName);
                            operatingVoltageDescription = connectedUpstreamFeatureLine.GetFieldValue(null, true, _operatingVoltageModelName);
                        }
                    }
                }
                else
                {
                    operatingVoltageValue = connectedUpstreamFeatureLine.GetFieldValue(null, false, _operatingVoltageModelName);
                    operatingVoltageDescription = connectedUpstreamFeatureLine.GetFieldValue(null, true, _operatingVoltageModelName);
                }
                #endregion
            }
            else
            {
                _logger.Warn("PGE Inherit Operating Voltage should not assign to this feature class or Check the class model name has not been assign as per requirement of this autoupdator.");
                return;
            }


            if (operatingVoltageValue == null || operatingVoltageValue.Convert(string.Empty) == string.Empty)
            {
                // Update the default voltage.
                UpdateDefaultValue(incommingOVIndex, obj);
            } 
            else
            {
                // Q1-2021 Voltage change for ED-GIS Scripting project. Voltage drop values used when phase
                // transitions from Multi-Phase to Single-Phase.
                object objFinalValue = null;
                int existingVoltageValue = obj.get_Value(incommingOVIndex).Convert<int>(-1);
                string phaseValue = obj.GetFieldValue(null, true, _phaseDesignationModelName).Convert<string>(string.Empty);
                if (upstreamPhaseValue != null && phaseValue != null)
                {
                    int newVoltageValue = -1;
                    string newVoltageDescription = "";
                    GetNewVoltageBasedOnPhase(operatingVoltageValue.Convert<int>(-1), operatingVoltageDescription.ToString(), upstreamPhaseValue, phaseValue, out newVoltageValue, out newVoltageDescription);
                    // If a new value exists.
                    if (newVoltageValue > -1)
                    {
                        objFinalValue = MatchDomainDescriptionAndReturnValue(obj, obj.Fields.get_Field(incommingOVIndex), newVoltageDescription);
                    }
                    else
                    {
                        // Otherwise lets try the original.
                        objFinalValue = MatchDomainDescriptionAndReturnValue(obj, obj.Fields.get_Field(incommingOVIndex), operatingVoltageDescription.ToString());
                    }
                }
                else
                {
                    // If it's not a primary feature the phase shouldn't be considered.
                    objFinalValue = MatchDomainDescriptionAndReturnValue(obj, obj.Fields.get_Field(incommingOVIndex), operatingVoltageDescription.ToString());
                }
                if (objFinalValue != null) 
                {
                    obj.set_Value(incommingOVIndex, objFinalValue);
                    if (callStore) 
                    { 
                        obj.Store(); 
                    }
                }
                else 
                { 
                    _logger.Warn("Voltage (" + operatingVoltageDescription.ToString() + ") of upstream connected feature is not matching  with the domain assigned to the operating voltage of the line " + obj.Class.AliasName); 
                }
            }
        }

        /// <summary>
        /// This method will update the voltage of a feature object.
        /// </summary>
        /// <param name="obj">IObject (Feature)</param>
        private void UpdateVoltage(IObject obj)
        {
            //Checking whether the obj geometry is point type or line type
            Boolean isPointFeature = ((obj as IFeature).Shape.GeometryType == ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPoint);
            //if obj is Point
            if (isPointFeature)
            {
                UpdatePoint(obj);
            }
            else
            {
                UpdateLine(obj);
            }
            
        }
        /// <summary>
        /// This method will match the "matchingString" name with a field domain and return the value.
        /// </summary>
        /// <param name="voltageField">Operating Voltage field</param>
        /// <param name="matchingString">Name/Description to be matched with</param>
        /// <returns>Value of the matched Name/Description</returns>
        private object MatchDomainDescriptionAndReturnValue(IObject obj, IField voltageField,string matchingString)
        {

            IRowSubtypes rowSubtypes = (IRowSubtypes)obj;
            ISubtypes subtypes = (ISubtypes)obj.Class;
            string fieldName = voltageField.Name;
            IDomain domain = subtypes.get_Domain(rowSubtypes.SubtypeCode, fieldName);
     
            object objValue = null;
            if (domain != null)
            {
                //IDomain voltageDomain = voltageField.Domain;
                if (domain.Type == esriDomainType.esriDTCodedValue)
                {
                    ICodedValueDomain pCodedValueDomain = domain as ICodedValueDomain;

                    //Get coded value name and value and check with given value
                    for (int j = 0; j < pCodedValueDomain.CodeCount; j++)
                    {
                        string codedValueName = pCodedValueDomain.get_Name(j);
                        object codedValueValue = pCodedValueDomain.get_Value(j);

                        if (codedValueName.ToString().ToLower().Equals(matchingString.ToLower()))
                        {
                            objValue = codedValueValue;
                            break;
                        }
                    }
                }
            }
            return objValue;
        }

        /// <summary>
        /// Gets Voltage value based on phase.
        /// </summary>
        /// <param name="upstreamVoltage">Upstream Voltage Code</param>
        /// <param name="upstreamPhase">Upstream Phase Designation Description</param>
        /// <param name="featurePhase">Downstream Phase Designation Description</param>
        /// <returns></returns>
        private void GetNewVoltageBasedOnPhase(int upstreamVoltage, string upstreamVoltageDescription, string upstreamPhase, string featurePhase, out int newVoltageCode, out string newVoltageDescription)
        {


            if (upstreamPhase.Length > 1 && featurePhase.Length == 1)
            {
                VoltageHelper.GetValidPrimaryVoltageDrop(upstreamVoltage, out newVoltageCode, out newVoltageDescription);
            }
            else if (upstreamPhase.Length == 1 && featurePhase.Length > 1)
            {
                VoltageHelper.GetValidPrimaryCircuitVoltage(upstreamVoltage, out newVoltageCode, out newVoltageDescription);

            }
            else
            {
                newVoltageCode = upstreamVoltage;
                newVoltageDescription = upstreamVoltageDescription;
            }

        }
        #endregion
    }
}
