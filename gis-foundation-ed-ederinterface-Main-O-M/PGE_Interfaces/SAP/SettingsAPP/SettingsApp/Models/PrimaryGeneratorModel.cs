using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using SettingsApp.Common;
using System.Web.Mvc;

namespace SettingsApp.Models
{
    public class PrimaryGeneratorModel
    {
        [SettingsValidatorAttribute("PRIMARY_GEN_DTL", "DEVICE_ID")]
        [Display(Name = "Device Id:")]
        public decimal DeviceId { get; set; }
        [SettingsValidatorAttribute("PRIMARY_GEN_DTL", "GENERATOR_TYPE")]
        [Display(Name = "Choose Generator Type:")]
        public string GeneratorType { get; set; }
        [SettingsValidatorAttribute("PRIMARY_GEN_DTL", "RATED_POWER_KVA")]
        [Display(Name = "Rated Power (Kva):")]
        public decimal? RatedPowerKva { get; set; }
        [SettingsValidatorAttribute("PRIMARY_GEN_DTL", "RATED_VOLT_KVLL")]
        [Display(Name = "Rated Voltage (Kvll):")]
        public decimal? RatedVoltKvll { get; set; }
        [SettingsValidatorAttribute("PRIMARY_GEN_DTL", "ACTIVE_GENERATION_KW")]
        [Display(Name = "Active Generation (Kw):")]
        public decimal? ActiveGenerationKw { get; set; }
        [SettingsValidatorAttribute("PRIMARY_GEN_DTL", "POWER_FACTOR_PERC")]
        [Display(Name = "Power Factor (%):")]
        public decimal? PowerFactorPerc { get; set; }
        [SettingsValidatorAttribute("PRIMARY_GEN_DTL", "FAULT_CONTRIBUTION_PERC")]
        [Display(Name = "Fault Contribution (% Of Rated Current):")]
        public decimal? FaultContributionPerc { get; set; }
        [SettingsValidatorAttribute("PRIMARY_GEN_DTL", "RATED_SPEED_RPM")]
        [Display(Name = "Rated Speed (Rpm):")]
        public decimal? RatedSpeedRpm { get; set; }
        [SettingsValidatorAttribute("PRIMARY_GEN_DTL", "ANSI_MOTOR_GROUP")]
        [Display(Name = "Ansi Motor Group :")]
        public string AnsiMotorGroup { get; set; }
        [SettingsValidatorAttribute("PRIMARY_GEN_DTL", "SUBTRANSIENT_Z_R_OHMS")]
        [Display(Name = "Subtransient Z\" R (Ohms):")]
        public decimal? SubtransientZROhms { get; set; }
        [SettingsValidatorAttribute("PRIMARY_GEN_DTL", "SUBTRANSIENT_Z_X_OHMS")]
        [Display(Name = "Subtransient Z\" X (Ohms):")]
        public decimal? SubtransientZXOhms { get; set; }
        [SettingsValidatorAttribute("PRIMARY_GEN_DTL", "ROTOR_TYPE")]
        [Display(Name = "Rotor Type :")]
        public string RotorType { get; set; }
        [SettingsValidatorAttribute("PRIMARY_GEN_DTL", "ESTIMATION_METHOD")]
        [Display(Name = "Estimation Method :")]
        public string EstimationMethod { get; set; }
        [SettingsValidatorAttribute("PRIMARY_GEN_DTL", "STATOR_RS_OHMS")]
        [Display(Name = "Stator Rs (Ohms):")]
        public decimal? StatorRsOhms { get; set; }
        [SettingsValidatorAttribute("PRIMARY_GEN_DTL", "STATOR_XS_OHMS")]
        [Display(Name = "Stator Xs (Ohms):")]
        public decimal? StatorXsOhms { get; set; }
        [SettingsValidatorAttribute("PRIMARY_GEN_DTL", "MAGNETISING_RM_OHMS")]
        [Display(Name = "Magnetising Rm (Ohms):")]
        public decimal? MagnetisingRmOhms { get; set; }
        [SettingsValidatorAttribute("PRIMARY_GEN_DTL", "MAGNETISING_XM_OHMS")]
        [Display(Name = "Magnetising Xm (Ohms):")]
        public decimal? MagnetisingXmOhms { get; set; }
        [SettingsValidatorAttribute("PRIMARY_GEN_DTL", "ROTOR1_RR_OHMS")]
        [Display(Name = "Rotor1 Rr (Ohms):")]
        public decimal? Rotor1RrOhms { get; set; }
        [SettingsValidatorAttribute("PRIMARY_GEN_DTL", "ROTOR1_XR_OHMS")]
        [Display(Name = "Rotor1 Xr (Ohms):")]
        public decimal? Rotor1XrOhms { get; set; }
        [SettingsValidatorAttribute("PRIMARY_GEN_DTL", "ROTOR2_RR_OHMS")]
        [Display(Name = "Rotor2 Rr (Ohms):")]
        public decimal? Rotor2RrOhms { get; set; }
        [SettingsValidatorAttribute("PRIMARY_GEN_DTL", "ROTOR2_XR_OHMS")]
        [Display(Name = "Rotor2 Xr (Ohms):")]
        public decimal? Rotor2XrOhms { get; set; }
        [SettingsValidatorAttribute("PRIMARY_GEN_DTL", "CAGE_FACTOR_CFR")]
        [Display(Name = "Cage Factor Cfr:")]
        public decimal? CageFactorCfr { get; set; }
        [SettingsValidatorAttribute("PRIMARY_GEN_DTL", "CAGE_FACTOR_CFX")]
        [Display(Name = "Cage Factor Cfx:")]
        public decimal? CageFactorCfx { get; set; }
        [SettingsValidatorAttribute("PRIMARY_GEN_DTL", "INERTIAL_ROTATING_MASSH")]
        [Display(Name = "Inertial Of Rotating Mass H (Mw*S/Mva):")]
        public decimal? InertialRotatingMassh { get; set; }
        [SettingsValidatorAttribute("PRIMARY_GEN_DTL", "INERTIAL_ROTATING_MASSJ")]
        [Display(Name = "Inertial Of Rotating Mass J (Lb*Ft*Ft):")]
        public decimal? InertialRotatingMassj { get; set; }
        [SettingsValidatorAttribute("PRIMARY_GEN_DTL", "NUMBER_OF_POLES")]
        [Display(Name = "Number Of Poles:")]
        public decimal? NumberOfPoles { get; set; }
        [SettingsValidatorAttribute("PRIMARY_GEN_DTL", "MAX_REACTIVE_PW_KVAR")]
        [Display(Name = "Max Reactive Power(Kvar):")]
        public decimal? MaxReactivePwKvar { get; set; }
        [SettingsValidatorAttribute("PRIMARY_GEN_DTL", "MIN_REACTIVE_PW_KVAR")]
        [Display(Name = "Min Reactive Power (Kvar):")]
        public decimal? MinReactivePwKvar { get; set; }
        [SettingsValidatorAttribute("PRIMARY_GEN_DTL", "CONFIGURATION")]
        [Display(Name = "Configuration :")]
        public string Configuration { get; set; }
        [SettingsValidatorAttribute("PRIMARY_GEN_DTL", "REACTIVE_POWER_CAPABILITY")]
        [Display(Name = "Reactive Power Capability :")]
        public string ReactivePowerCapability { get; set; }
        [SettingsValidatorAttribute("PRIMARY_GEN_DTL", "STEADY_STATE_Z1_0_R_OHMS")]
        [Display(Name = "Steady State Z1 R (Ohms):")]
        public decimal? SteadyStateZ10ROhms { get; set; }
        [SettingsValidatorAttribute("PRIMARY_GEN_DTL", "STEADY_STATE_Z1_0_X_OHMS")]
        [Display(Name = "Steady State Z1 X (Ohms):")]
        public decimal? SteadyStateZ10XOhms { get; set; }
        [SettingsValidatorAttribute("PRIMARY_GEN_DTL", "STEADY_STATE_Z1_1_R_OHMS")]
        [Display(Name = "Steady State Z1' R (Ohms):")]
        public decimal? SteadyStateZ11ROhms { get; set; }
        [SettingsValidatorAttribute("PRIMARY_GEN_DTL", "STEADY_STATE_Z1_1_X_OHMS")]
        [Display(Name = "Steady State Z1' X (Ohms):")]
        public decimal? SteadyStateZ11XOhms { get; set; }
        [SettingsValidatorAttribute("PRIMARY_GEN_DTL", "STEADY_STATE_Z1_2_R_OHMS")]
        [Display(Name = "Steady State Z1\" R (Ohms):")]
        public decimal? SteadyStateZ12ROhms { get; set; }
        [SettingsValidatorAttribute("PRIMARY_GEN_DTL", "STEADY_STATE_Z1_2_X_OHMS")]
        [Display(Name = "Steady State Z1\" X (Ohms):")]
        public decimal? SteadyStateZ12XOhms { get; set; }
        [SettingsValidatorAttribute("PRIMARY_GEN_DTL", "ZERO_SEQ_Z0_R_OHMS")]
        [Display(Name = "Zero Sequence Z0 R (Ohms):")]
        public decimal? ZeroSeqZ0ROhms { get; set; }
        [SettingsValidatorAttribute("PRIMARY_GEN_DTL", "ZERO_SEQ_Z0_X_OHMS")]
        [Display(Name = "Zero Sequence Z0 X (Ohms):")]
        public decimal? ZeroSeqZ0XOhms { get; set; }
        [SettingsValidatorAttribute("PRIMARY_GEN_DTL", "NEG_SEQ_Z2_R_OHMS")]
        [Display(Name = "Negative Sequence Z2 R (Ohms):")]
        public decimal? NegSeqZ2ROhms { get; set; }
        [SettingsValidatorAttribute("PRIMARY_GEN_DTL", "NEG_SEQ_Z2_X_OHMS")]
        [Display(Name = "Negative Sequence Z2 X (Ohms):")]
        public decimal? NegSeqZ2XOhms { get; set; }
        [SettingsValidatorAttribute("PRIMARY_GEN_DTL", "GROUNDING_ZG_R_OHMS")]
        [Display(Name = "Grounding Zg R (Ohms):")]
        public decimal? GroundingZgROhms { get; set; }
        [SettingsValidatorAttribute("PRIMARY_GEN_DTL", "GROUNDING_ZG_X_OHMS")]
        [Display(Name = "Grounding Zg X (Ohms):")]
        public decimal? GroundingZgXOhms { get; set; }
        [SettingsValidatorAttribute("PRIMARY_GEN_DTL", "SYNC_GEN_MODEL")]
        [Display(Name = "Sync Gen Model :")]
        public string SyncGenModel { get; set; }
        [SettingsValidatorAttribute("PRIMARY_GEN_DTL", "SYNCH_REACTANCE_XD_PU")]
        [Display(Name = "Synchronous Reactance Xd (Pu):")]
        public decimal? SynchReactanceXdPu { get; set; }
        [SettingsValidatorAttribute("PRIMARY_GEN_DTL", "SYNCH_REACTANCE_XQ_PU")]
        [Display(Name = "Synchronous Reactance Xq (Pu):")]
        public decimal? SynchReactanceXqPu { get; set; }
        [SettingsValidatorAttribute("PRIMARY_GEN_DTL", "SYNCH_REACTANCE_XI_PU")]
        [Display(Name = "Synchronous Reactance Xl (Pu):")]
        public decimal? SynchReactanceXiPu { get; set; }
        [SettingsValidatorAttribute("PRIMARY_GEN_DTL", "TRANSIENT_DATA_XD_PU")]
        [Display(Name = "Transient Data X\"D (Pu):")]
        public decimal? TransientDataXdPu { get; set; }
        [SettingsValidatorAttribute("PRIMARY_GEN_DTL", "TRANSIENT_DATA_TDO_PU")]
        [Display(Name = "Transient Data T\"Do (Pu):")]
        public decimal? TransientDataTdoPu { get; set; }
        [SettingsValidatorAttribute("PRIMARY_GEN_DTL", "TRANSIENT_DATA_XQ_PU")]
        [Display(Name = "Transient Data X\"Q (Pu):")]
        public decimal? TransientDataXqPu { get; set; }
        [SettingsValidatorAttribute("PRIMARY_GEN_DTL", "TRANSIENT_DATA_TQO_PU")]
        [Display(Name = "Transient Data T\"Qo (Pu):")]
        public decimal? TransientDataTqoPu { get; set; }
        [SettingsValidatorAttribute("PRIMARY_GEN_DTL", "SUBTRANSIENT_DATA_XD_PU")]
        [Display(Name = "Subtransient Data X\"D (Pu):")]
        public decimal? SubtransientDataXdPu { get; set; }
        [SettingsValidatorAttribute("PRIMARY_GEN_DTL", "SUBTRANSIENT_DATA_TDO_PU")]
        [Display(Name = "Subtransient Data T\"Do (Pu):")]
        public decimal? SubtransientDataTdoPu { get; set; }
        [SettingsValidatorAttribute("PRIMARY_GEN_DTL", "SUBTRANSIENT_DATA_XQ_PU")]
        [Display(Name = "Subtransient Data X\"Q (Pu):")]
        public decimal? SubtransientDataXqPu { get; set; }
        [SettingsValidatorAttribute("PRIMARY_GEN_DTL", "SUBTRANSIENT_DATA_TQO_PU")]
        [Display(Name = "Subtransient Data T\"Qo (Pu):")]
        public decimal? SubtransientDataTqoPu { get; set; }
        [SettingsValidatorAttribute("PRIMARY_GEN_DTL", "SATURATION_DATA_SGU_PU")]
        [Display(Name = "Saturation Data Sgu (Pu):")]
        public decimal? SaturationDataSguPu { get; set; }
        [SettingsValidatorAttribute("PRIMARY_GEN_DTL", "SATURATION_DATA_SGL_PU")]
        [Display(Name = "Saturation Data Sgl (Pu):")]
        public decimal? SaturationDataSglPu { get; set; }
        [SettingsValidatorAttribute("PRIMARY_GEN_DTL", "SATURATION_DATA_EU_PU")]
        [Display(Name = "Saturation Data Eu (Pu):")]
        public decimal? SaturationDataEuPu { get; set; }
        [SettingsValidatorAttribute("PRIMARY_GEN_DTL", "SATURATION_DATA_EL_PU")]
        [Display(Name = "Saturation Data El (Pu):")]
        public decimal? SaturationDataElPu { get; set; }
        [SettingsValidatorAttribute("PRIMARY_GEN_DTL", "DAMPING_CONSTANT_KD_PU")]
        [Display(Name = "Damping Constant Kd (Pu):")]
        public decimal? DampingConstantKdPu { get; set; }

        public HashSet<string> FieldsToDisplay { get; set; }
        public SelectList GeneratorTypeList { get; set; }
        public SelectList AnsiMotorGroupList { get; set; }
        public SelectList RotorTypeList { get; set; }
        public SelectList EstimatedMethodsList { get; set; }
        public SelectList ConfigurationList { get; set; }
        public SelectList ReactivePowerCapabilityList { get; set; }
        public SelectList SyncGenModelList { get; set; }
        public string DropDownPostbackScriptGeneratorType { get; set; }


        public void PopulateEntityFromModel(SM_PRIMARY_GEN_DTL e)
        {
            e.PRIMARY_GEN_ID = this.DeviceId;
            e.GENERATOR_TYPE = string.IsNullOrWhiteSpace(this.GeneratorType) ? " " : this.GeneratorType;
            e.RATED_POWER_KVA = this.RatedPowerKva??0;
            e.RATED_VOLT_KVLL = this.RatedVoltKvll??0;
            e.ACTIVE_GENERATION_KW = this.ActiveGenerationKw??0;
            e.POWER_FACTOR_PERC = this.PowerFactorPerc??0;
            e.FAULT_CONTRIBUTION_PERC = this.FaultContributionPerc;
            e.RATED_SPEED_RPM = this.RatedSpeedRpm;
            e.ANSI_MOTOR_GROUP = this.AnsiMotorGroup;
            e.SUBTRANSIENT_Z_R_OHMS = this.SubtransientZROhms;
            e.SUBTRANSIENT_Z_X_OHMS = this.SubtransientZXOhms;
            e.ROTOR_TYPE = this.RotorType;
            e.ESTIMATION_METHOD = this.EstimationMethod;
            e.STATOR_RS_OHMS = this.StatorRsOhms;
            e.STATOR_XS_OHMS = this.StatorXsOhms;
            e.MAGNETISING_RM_OHMS = this.MagnetisingRmOhms;
            e.MAGNETISING_XM_OHMS = this.MagnetisingXmOhms;
            e.ROTOR1_RR_OHMS = this.Rotor1RrOhms;
            e.ROTOR1_XR_OHMS = this.Rotor1XrOhms;
            e.ROTOR2_RR_OHMS = this.Rotor2RrOhms;
            e.ROTOR2_XR_OHMS = this.Rotor2XrOhms;
            e.CAGE_FACTOR_CFR = this.CageFactorCfr;
            e.CAGE_FACTOR_CFX = this.CageFactorCfx;
            e.INERTIAL_ROTATING_MASSH = this.InertialRotatingMassh;
            e.INERTIAL_ROTATING_MASSJ = this.InertialRotatingMassj;
            e.NUMBER_OF_POLES = this.NumberOfPoles;
            e.MAX_REACTIVE_PW_KVAR = this.MaxReactivePwKvar;
            e.MIN_REACTIVE_PW_KVAR = this.MinReactivePwKvar;
            e.CONFIGURATION = this.Configuration;
            e.REACTIVE_POWER_CAPABILITY = this.ReactivePowerCapability;
            e.STEADY_STATE_Z1_0_R_OHMS = this.SteadyStateZ10ROhms;
            e.STEADY_STATE_Z1_0_X_OHMS = this.SteadyStateZ10XOhms;
            e.STEADY_STATE_Z1_1_R_OHMS = this.SteadyStateZ11ROhms;
            e.STEADY_STATE_Z1_1_X_OHMS = this.SteadyStateZ11XOhms;
            e.STEADY_STATE_Z1_2_R_OHMS = this.SteadyStateZ12ROhms;
            e.STEADY_STATE_Z1_2_X_OHMS = this.SteadyStateZ12XOhms;
            e.ZERO_SEQ_Z0_R_OHMS = this.ZeroSeqZ0ROhms;
            e.ZERO_SEQ_Z0_X_OHMS = this.ZeroSeqZ0XOhms;
            e.NEG_SEQ_Z2_R_OHMS = this.NegSeqZ2ROhms;
            e.NEG_SEQ_Z2_X_OHMS = this.NegSeqZ2XOhms;
            e.GROUNDING_ZG_R_OHMS = this.GroundingZgROhms;
            e.GROUNDING_ZG_X_OHMS = this.GroundingZgXOhms;
            e.SYNC_GEN_MODEL = this.SyncGenModel;
            e.SYNCH_REACTANCE_XD_PU = this.SynchReactanceXdPu;
            e.SYNCH_REACTANCE_XQ_PU = this.SynchReactanceXqPu;
            e.SYNCH_REACTANCE_XI_PU = this.SynchReactanceXiPu;
            e.TRANSIENT_DATA_XD_PU = this.TransientDataXdPu;
            e.TRANSIENT_DATA_TDO_PU = this.TransientDataTdoPu;
            e.TRANSIENT_DATA_XQ_PU = this.TransientDataXqPu;
            e.TRANSIENT_DATA_TQO_PU = this.TransientDataTqoPu;
            e.SUBTRANSIENT_DATA_XD_PU = this.SubtransientDataXdPu;
            e.SUBTRANSIENT_DATA_TDO_PU = this.SubtransientDataTdoPu;
            e.SUBTRANSIENT_DATA_XQ_PU = this.SubtransientDataXqPu;
            e.SUBTRANSIENT_DATA_TQO_PU = this.SubtransientDataTqoPu;
            e.SATURATION_DATA_SGU_PU = this.SaturationDataSguPu;
            e.SATURATION_DATA_SGL_PU = this.SaturationDataSglPu;
            e.SATURATION_DATA_EU_PU = this.SaturationDataEuPu;
            e.SATURATION_DATA_EL_PU = this.SaturationDataElPu;
            e.DAMPING_CONSTANT_KD_PU = this.DampingConstantKdPu;



        }

        public void PopulateModelFromEntity(SM_PRIMARY_GEN_DTL e)
        {
            this.DeviceId = e.PRIMARY_GEN_ID;
            this.GeneratorType = string.IsNullOrWhiteSpace(e.GENERATOR_TYPE) ? string.Empty : e.GENERATOR_TYPE;
            this.RatedPowerKva = e.RATED_POWER_KVA;
            this.RatedVoltKvll = e.RATED_VOLT_KVLL;
            this.ActiveGenerationKw = e.ACTIVE_GENERATION_KW;
            this.PowerFactorPerc = e.POWER_FACTOR_PERC;
            this.FaultContributionPerc = e.FAULT_CONTRIBUTION_PERC;
            this.RatedSpeedRpm = e.RATED_SPEED_RPM;
            this.AnsiMotorGroup = e.ANSI_MOTOR_GROUP;
            this.SubtransientZROhms = e.SUBTRANSIENT_Z_R_OHMS;
            this.SubtransientZXOhms = e.SUBTRANSIENT_Z_X_OHMS;
            this.RotorType = e.ROTOR_TYPE;
            this.EstimationMethod = e.ESTIMATION_METHOD;
            this.StatorRsOhms = e.STATOR_RS_OHMS;
            this.StatorXsOhms = e.STATOR_XS_OHMS;
            this.MagnetisingRmOhms = e.MAGNETISING_RM_OHMS;
            this.MagnetisingXmOhms = e.MAGNETISING_XM_OHMS;
            this.Rotor1RrOhms = e.ROTOR1_RR_OHMS;
            this.Rotor1XrOhms = e.ROTOR1_XR_OHMS;
            this.Rotor2RrOhms = e.ROTOR2_RR_OHMS;
            this.Rotor2XrOhms = e.ROTOR2_XR_OHMS;
            this.CageFactorCfr = e.CAGE_FACTOR_CFR;
            this.CageFactorCfx = e.CAGE_FACTOR_CFX;
            this.InertialRotatingMassh = e.INERTIAL_ROTATING_MASSH;
            this.InertialRotatingMassj = e.INERTIAL_ROTATING_MASSJ;
            this.NumberOfPoles = e.NUMBER_OF_POLES;
            this.MaxReactivePwKvar = e.MAX_REACTIVE_PW_KVAR;
            this.MinReactivePwKvar = e.MIN_REACTIVE_PW_KVAR;
            this.Configuration = e.CONFIGURATION;
            this.ReactivePowerCapability = e.REACTIVE_POWER_CAPABILITY;
            this.SteadyStateZ10ROhms = e.STEADY_STATE_Z1_0_R_OHMS;
            this.SteadyStateZ10XOhms = e.STEADY_STATE_Z1_0_X_OHMS;
            this.SteadyStateZ11ROhms = e.STEADY_STATE_Z1_1_R_OHMS;
            this.SteadyStateZ11XOhms = e.STEADY_STATE_Z1_1_X_OHMS;
            this.SteadyStateZ12ROhms = e.STEADY_STATE_Z1_2_R_OHMS;
            this.SteadyStateZ12XOhms = e.STEADY_STATE_Z1_2_X_OHMS;
            this.ZeroSeqZ0ROhms = e.ZERO_SEQ_Z0_R_OHMS;
            this.ZeroSeqZ0XOhms = e.ZERO_SEQ_Z0_X_OHMS;
            this.NegSeqZ2ROhms = e.NEG_SEQ_Z2_R_OHMS;
            this.NegSeqZ2XOhms = e.NEG_SEQ_Z2_X_OHMS;
            this.GroundingZgROhms = e.GROUNDING_ZG_R_OHMS;
            this.GroundingZgXOhms = e.GROUNDING_ZG_X_OHMS;
            this.SyncGenModel = e.SYNC_GEN_MODEL;
            this.SynchReactanceXdPu = e.SYNCH_REACTANCE_XD_PU;
            this.SynchReactanceXqPu = e.SYNCH_REACTANCE_XQ_PU;
            this.SynchReactanceXiPu = e.SYNCH_REACTANCE_XI_PU;
            this.TransientDataXdPu = e.TRANSIENT_DATA_XD_PU;
            this.TransientDataTdoPu = e.TRANSIENT_DATA_TDO_PU;
            this.TransientDataXqPu = e.TRANSIENT_DATA_XQ_PU;
            this.TransientDataTqoPu = e.TRANSIENT_DATA_TQO_PU;
            this.SubtransientDataXdPu = e.SUBTRANSIENT_DATA_XD_PU;
            this.SubtransientDataTdoPu = e.SUBTRANSIENT_DATA_TDO_PU;
            this.SubtransientDataXqPu = e.SUBTRANSIENT_DATA_XQ_PU;
            this.SubtransientDataTqoPu = e.SUBTRANSIENT_DATA_TQO_PU;
            this.SaturationDataSguPu = e.SATURATION_DATA_SGU_PU;
            this.SaturationDataSglPu = e.SATURATION_DATA_SGL_PU;
            this.SaturationDataEuPu = e.SATURATION_DATA_EU_PU;
            this.SaturationDataElPu = e.SATURATION_DATA_EL_PU;
            this.DampingConstantKdPu = e.DAMPING_CONSTANT_KD_PU;

        }

        public void PopulateGeneratorTypebasedModels(string GeneratorType, PrimaryGeneratorModel source, PrimaryGeneratorModel dest)
        {
            dest.DeviceId = source.DeviceId;
            dest.GeneratorType = string.IsNullOrWhiteSpace(source.GeneratorType) ? " " : source.GeneratorType;
            dest.RatedPowerKva = source.RatedPowerKva;
            dest.RatedVoltKvll = source.RatedVoltKvll;
            dest.ActiveGenerationKw = source.ActiveGenerationKw;
            dest.PowerFactorPerc = source.PowerFactorPerc;
            switch (GeneratorType)
            {
                case "EC":
                    dest.FaultContributionPerc = source.FaultContributionPerc;
                    break;
                case "IN":
                    dest.RatedSpeedRpm = source.RatedSpeedRpm;
                    dest.AnsiMotorGroup = source.AnsiMotorGroup;
                    dest.SubtransientZROhms = source.SubtransientZROhms;
                    dest.SubtransientZXOhms = source.SubtransientZXOhms;
                    dest.RotorType = source.RotorType;
                    dest.EstimationMethod = source.EstimationMethod;
                    dest.StatorRsOhms = source.StatorRsOhms;
                    dest.StatorXsOhms = source.StatorXsOhms;
                    dest.MagnetisingRmOhms = source.MagnetisingRmOhms;
                    dest.MagnetisingXmOhms = source.MagnetisingXmOhms;
                    dest.Rotor1RrOhms = source.Rotor1RrOhms;
                    dest.Rotor1XrOhms = source.Rotor1XrOhms;
                    dest.Rotor2RrOhms = source.Rotor2RrOhms;
                    dest.Rotor2XrOhms = source.Rotor2XrOhms;
                    dest.CageFactorCfr = source.CageFactorCfr;
                    dest.CageFactorCfx = source.CageFactorCfx;
                    dest.InertialRotatingMassh = source.InertialRotatingMassh;
                    dest.InertialRotatingMassj = source.InertialRotatingMassj;
                    break;
                case "SY":
                    dest.InertialRotatingMassh = source.InertialRotatingMassh;
                    dest.InertialRotatingMassj = source.InertialRotatingMassj;
                    dest.NumberOfPoles = source.NumberOfPoles;
                    dest.MaxReactivePwKvar = source.MaxReactivePwKvar;
                    dest.MinReactivePwKvar = source.MinReactivePwKvar;
                    dest.Configuration = source.Configuration;
                    dest.ReactivePowerCapability = source.ReactivePowerCapability;
                    dest.SteadyStateZ10ROhms = source.SteadyStateZ10ROhms;
                    dest.SteadyStateZ10XOhms = source.SteadyStateZ10XOhms;
                    dest.SteadyStateZ11ROhms = source.SteadyStateZ11ROhms;
                    dest.SteadyStateZ11XOhms = source.SteadyStateZ11XOhms;
                    dest.SteadyStateZ12ROhms = source.SteadyStateZ12ROhms;
                    dest.SteadyStateZ12XOhms = source.SteadyStateZ12XOhms;
                    dest.ZeroSeqZ0ROhms = source.ZeroSeqZ0ROhms;
                    dest.ZeroSeqZ0XOhms = source.ZeroSeqZ0XOhms;
                    dest.NegSeqZ2ROhms = source.NegSeqZ2ROhms;
                    dest.NegSeqZ2XOhms = source.NegSeqZ2XOhms;
                    dest.GroundingZgROhms = source.GroundingZgROhms;
                    dest.GroundingZgXOhms = source.GroundingZgXOhms;
                    dest.SyncGenModel = source.SyncGenModel;
                    dest.SynchReactanceXdPu = source.SynchReactanceXdPu;
                    dest.SynchReactanceXqPu = source.SynchReactanceXqPu;
                    dest.SynchReactanceXiPu = source.SynchReactanceXiPu;
                    dest.TransientDataXdPu = source.TransientDataXdPu;
                    dest.TransientDataTdoPu = source.TransientDataTdoPu;
                    dest.TransientDataXqPu = source.TransientDataXqPu;
                    dest.TransientDataTqoPu = source.TransientDataTqoPu;
                    dest.SubtransientDataXdPu = source.SubtransientDataXdPu;
                    dest.SubtransientDataTdoPu = source.SubtransientDataTdoPu;
                    dest.SubtransientDataXqPu = source.SubtransientDataXqPu;
                    dest.SubtransientDataTqoPu = source.SubtransientDataTqoPu;
                    dest.SaturationDataSguPu = source.SaturationDataSguPu;
                    dest.SaturationDataSglPu = source.SaturationDataSglPu;
                    dest.SaturationDataEuPu = source.SaturationDataEuPu;
                    dest.SaturationDataElPu = source.SaturationDataElPu;
                    dest.DampingConstantKdPu = source.DampingConstantKdPu;
                    break;
            }
        }
        
    }
}