using System;
namespace PGE.Desktop.EDER
{

    /// <summary>
    /// The schema info struct that holds constant information.     
    /// </summary>
    public struct SchemaInfo
    {
        /// <summary>
        /// General schema information
        /// </summary>
        public struct General
        {
            public const string CustomDisplayField = "PGE_CUSTOMDISPLAYFIELD";

            public const string PhaseDesignationDomainName = "Phase Designation";

            public const string GeometricNetworkName = "EDGIS.ElectricDistNetwork";
            
            public const string ESRI_RULES = "ESRI RULES";
            public static string pAHInfoTableName = "PGEDATA.PGE_GDBM_AH_Info";
            public static string pEDERCONFIGTABLE = "PGEDATA.PGE_EDERCONFIG";
            public const string PrimaryVoltageDomainName = "Primary Voltage";

            /// <summary>
            /// The field model names that are not schema specific.
            /// </summary>
            public struct FieldModelNames
            {
                /// <summary>
                /// Used to identify the LabelText field.
                /// </summary>
                public const string LabelText = "LABELTEXT";

                /// <summary>
                /// Used to identify the LabelText2 field.
                /// </summary>
                public const string LabelText2 = "LABELTEXT2";

                /// <summary>
                /// Used to identify the Measured Length field.
                /// </summary>
                public const string MeasuredLength = "MEASUREDLENGTH";

                /// <summary>
                /// Used to identify the LabelText field.
                /// </summary>
                public const string PGEAnnoExpField = "PGE_ANNO_EXP_FIELD";
                /// <summary>
                /// Used to identify the DetectionScheme field.
                /// </summary>
                public const string DetectionScheme = "PGE_DetectionScheme";

                /// <summary>
                /// Identifies the Mail Name field.
                /// </summary>
                public const string MailName = "PGE_MAILNAME";

                /// <summary>
                /// Identifies the latitude field.
                /// </summary>
                public const string Latitude = "PGE_LATITUDE";

                /// <summary>
                /// Identifies the longitude field.
                /// </summary>
                public const string Longitude = "PGE_LONGITUDE";

                /// <summary>
                /// Identifies a field with a value that should be upper case.
                /// (unused)
                /// </summary>
                public const string UpperCase = "PGE_UPPERCASE";
                /// <summary>
                /// Map Number field model name in MaintenancePlat polygon
                /// </summary>
                public const string MapNumberMN = "PGE_DISTRIBUTIONMAPNUMBER";
                /// <summary>
                /// Map Office field model name in MaintenancePlat polygon
                /// </summary>
                public const string MapOfficeMN = "PGE_MAPOFFICE";
                /// <summary>
                /// Map Scale field model name in MaintenancePlat polygon
                /// </summary>
                public const string MapScaleeMN = "PGE_MAPSCALE";
                /// <summary>
                /// HasData field model name in Plat_Unified polygon
                /// </summary>
                public const string HasDataMN = "PGE_HASDATA";

                /// <summary>
                /// Field model name for Asset replacement functionality field mapping
                /// </summary>
                public const string AssetCopyFMN = "PGE_ASSETCOPY";
               
                // code for subtypecd and symbolnumber should be added seperately (this model name should not be applied on subtypecd and symbolnumber
                public const string FuseSBAssetCopyFMN = "PGE_FUSE_SB_ASSETCOPY";
                
                /// <summary>
                /// Field model name for Asset replacement functionality to store old features GUID to new features REPLACEGUID
                /// </summary>
                public const string ReplaceGUID = "PGE_REPLACEGUID";

                /// <summary>
                /// Field model name for Deactivate functionality to indicate that the feature is deactivated
                /// </summary>
                public const string DeactivateIndicator = "PGE_DEACTIVATEINDICATOR";

                /// <summary>
                /// Field model name used for linking between subsurface structures and other features
                /// </summary>
                public const string SubStructGUID = "PGE_SUBSTRUCTGUID";

                /// <summary>
                /// Field model name for indicate enabled field in the feature class
                /// </summary>
                public const string Enabled = "PGE_ENABLED";

                /// <summary>
                /// Field model name for indicating if a dead phase is present
                /// </summary>
                public const string DeadPhase = "PGE_DEADPHASEINDICATOR";

                /// <summary>
                /// Field model name for Schematics grid scale
                /// </summary>
                public const string Scale = "PGE_SCALE";

                /// <summary>
                /// Field model name for Font Size
                /// </summary>
                public const string FontSize = "PGE_FONTSIZE";

            }
            public struct FIELDS
            {
                public const string GLOBALID = "GLOBALID";
                public const string CIRCUITID = "CIRCUITID";
                public const string FEATURECLASSNAME = "FEATURECLASSNAME";
                public const string VERSIONNAME = "VERSIONNAME";
                public const string STATUS = "STATUS";
                public const string ACTION = "ACTION";
                public const string PARENT_VERSIONNAME = "PARENT_VERSIONNAME";
                public const string FEATOID = "FEATOID";
            }
            public struct ClassModelNames
            {
                /// <summary>
                /// Used to identify the SupportStructure feature class used for pole test and treat staging
                /// </summary>
                public const string PTTSupportStructure = "PGE_PTTSUPPORTSTRUCTURE";

                /// <summary>
                /// Used to identify the SupportStructure feature class used for pole test and treat staging
                /// </summary>
                public const string SupportStructure = "OVERHEADSTRUCTURE";

                /// <summary>
                /// Used to identify the label text unit.
                /// </summary>
                public const string LabelTextUnit = "PGE_LABELTEXTUNIT"; //"PGE_LabelTextUnit";

                /// <summary>
                /// Used to identify the label text bank.
                /// </summary>
                public const string LabelTextBank = "PGE_LABELTEXTBANK";//"PGE_LabelTextBank";

                /// <summary>
                /// Used to identify the symbol number rules table in the geodatabase.
                /// </summary>
                public const string SymbolNumberRules = "PGE_SYMBOL_NUMBER_RULES";

                /// <summary>
                /// Used to identify the symbol number rules table in the geodatabase.
                /// </summary>
                public const string OperatingNumberRules = "PGE_OPERATING_NUMBER_RULES";

                /// <summary>
                /// Used to identify the District Feature Class in the geodatabase.
                /// </summary>
                public const string District = "PGE_DISTRICT";

                /// <summary>
                /// Used to identify the Local Office Feature Class in the geodatabase.
                /// </summary>
                public const string LOCALOFFICE = "PGE_LOCALOFFICE";

                /// <summary>
                /// Used to identify the Local Office Feature Class in the geodatabase.
                /// </summary>
                public const string LOPC = "PGE_LOCALOFFICE";

                /// <summary>
                /// Identifies an class with latitude and longitude fields.
                /// </summary>
                public const string LatitudeLongitude = "PGE_LATLON";

                /// <summary>
                /// Identifies an annotation class.
                /// </summary>
                public const string Annotation = "PGE_ANNOTATION";
                /// <summary>
                /// Class model name for MaintenancePlat polygon
                /// </summary>
                public const string MapGridMN = "PGE_MAPGRID";

                /// <summary>
                /// Class model name for Asset replacement functionality to be enabled
                /// </summary>
                public const string AssetReplacementMN = "PGE_ASSETREPLACEMENT";
               
                // This has to be applied to Fuse Feature class
                public const string ReplaceFuseWithSBMN = "PGE_REPLACE_FUSE_WITH_SB";

                //This has to be applied to SB feature class
                public const string ReplaceSBWithFuseMN = "PGE_REPLACE_SB_WITH_FUSE";
                
                /// <summary>
                /// Class model name for Asset replacement functionality to be enabled
                /// </summary>
                public const string AssetReplacementAbandonedMN = "PGE_ASSETABANDONED";

                /// <summary>
                /// Class model name for Deactivation tool to be enabled
                /// </summary>
                public const string Deactivate = "PGE_DEACTIVATE";

                /// <summary>
                /// The below two model names are used in Schematics change detection
                /// </summary>
                public const string VERSIONDELETESPOINT = "PGE_EDSCHEM_VERSIONDELETEPOINT";
                public const string VERSIONDELETESLINE = "PGE_EDSCHEM_VERSIONDELETELINE";

                /// <summary>
                /// The model name is used for Schematic Unified Grid
                /// </summary>
                public const string SchematicsGrid = "PGE_SCHMGRID";

                /// <summary>
                /// The model name is used in 500 Schematics Anno
                /// </summary>
                public const string Schematic500Anno = "PGE_SCHM500ANNO";

                /// <summary>
                /// The model name is used to filter out classes for Feeder Manager 2.0 QA/QC
                /// </summary>
                public const string QAQC = "PGE_QAQC";

                /// <summary>

                /// The model name is used to filter install year field of feature and related class

                /// </summary>

                public const string INSTALL_DATE = "PGE_UPDATE_INSTALL_DT";

                /// <summary>

                /// The model name is used to filter install year field of feature's related class

                /// </summary>

                public const string REL_INST_DATE = "PGE_UPDATE_REL_INS_DT";
                public const string PGE_CONDUIT = "PGE_CONDUITSYSTEM";

                //egis-903: GUID LOCATOR SEARCH
                public const string PGE_LOCATOR_GUID_RESULT = "PGE_LOCATOR_GUID_RESULT";
            }


            public struct Domains
            {
                public struct YesNoText
                {
                    public const string Yes = "Y";
                    public const string No = "N";
                }

                public struct YesNoInteger
                {
                    public const int Yes = 1;
                    public const int No = 0;
                }
                public struct EnabledDomain

                {
                    public const bool True = true;
                    public const bool False = false;
                }
            }

            public struct ObjectTools
            {
                public const string AlignAnnoFront = "PGE_ALIGNANNOFRONT";
                public const string AlignAnnoBack = "PGE_ALIGNANNOBACK";
            }

            public struct Fields
            {
                public const string NumberOfUnits = "NUMBEROFUNITS";
                public const string PhaseDesignation = "PHASEDESIGNATION";
                public const string RatedKVA = "RATEDKVA";
            }
        }

        public struct Substation
        {
            public struct Fields
            {
                //                public const string NumberOfUnits = "NUMBEROFUNITS";
            }
        }


        public struct Wip
        {
            public static readonly String JETEquipmentTable = "WEBR.JET_EQUIPMENT";
            public static readonly String JETEquipmentTypeSelectionTable = "WEBR.EQUIPMENT_TYPE_SELECTION";
            public static readonly String JETJobsTable = "WEBR.JET_JOBS";
        }


        /// <summary>
        /// The schema information directly related to the electric data model either Transmission or Distribution.
        /// </summary>
        public struct Electric
        {
            #region Fields

            /// <summary>
            /// Used to identify the electric trace weight.
            /// </summary>
            public const string TraceWeight = "MMElectricTraceWeight";

            /// <summary>
            /// Identifies the GlobalID field.
            /// </summary>
            public const string GlobalID = "GLOBALID";

            /// <summary>
            /// Used to identify the Capacitor Bank Field.
            /// </summary>
            public const string UnitKvar = "UNITKVAR";

            /// <summary>
            /// Identifies the Unit Count field.
            /// </summary>
            public const string UnitCount = "UNITCOUNT";

            /// <summary>
            /// Identifies the Total Kvar field.
            /// </summary>
            public const string TotalKvar = "TOTALKVAR";

            /// <summary>
            /// Identifies the fields used for Schematics Change Detection
            /// </summary>
            public const string FeatureClassID = "FEATURECLASSID";
            public const string FeatureGUID = "FEATUREGUID";
            public const string VersionName = "VERSIONNAME";
            public const string DateDeleted = "DATEDELETED";
            public const string CircuitID = "CIRCUITID";
            public const string Shape = "SHAPE";
            public const string Status = "STATUS";
            public const string InstallJobNumber = "INSTALLJOBNUMBER";
            public const string PartialCurtailmentGUID = "PARTCURTAILPOINTGUID";
            #endregion

            /// <summary>
            /// Added By: Arvind Sinha, EGIS-849, Date: 3rd Feb 2021
            /// </summary>
            public const string Attachementtype = "ATTACHMENTTYPE";
            public const string Ccrating = "CCRATING";
            public const string InterruptingMedium = "INTERRUPTINGMEDIUM";
            public const string MaxInterruptingCurrent = "MAXIINTERRUPTINGCURRENT";
            public const string ComplexDeviceIDC = "COMPLEXDEVICEIDC";

            #region Phase Designation Enum
            /// <summary>
            /// Required for bitwise operation. Used for validating phase designation.
            /// </summary>
            public enum PhaseDesignationEnum
            {
                A = 1,
                B = 2,
                C = 4
            }

            #endregion

            #region Nested Types

            public struct Subtypes
            {
                public struct DynamicProtectiveDevice
                {
                    /// <summary>
                    /// Subtype code for Circuit Breakers.
                    /// </summary>
                    public const int CircuitBreaker = 1;
                    /// <summary>
                    /// Subtype code for Interrupters.
                    /// </summary>
                    public const int Interrupter = 2;
                    /// <summary>
                    /// Subtype code for Reclosers.
                    /// </summary>
                    public const int Recloser = 3;
                    /// <summary>
                    /// Subtype code for Sectionalizers.
                    /// </summary>
                    public const int Sectionalizer = 8;
                }
                public struct Switch
                {
                    /// <summary>
                    /// Subtype code for Overhead Disconnects.
                    /// </summary>
                    public const int OverheadDisconnect = 1;
                    /// <summary>
                    /// Subtype code for Overhead Switches.
                    /// </summary>
                    public const int OverheadSwitch = 2;
                    /// <summary>
                    /// Subtype code for Underground Disconnects.
                    /// </summary>
                    public const int UndergroundDisconnect = 3;
                    /// <summary>
                    /// Subtype code for Padmounted Switches.
                    /// </summary>
                    public const int PadmountSwitch = 4;
                    /// <summary>
                    /// Subtype code for Subsurface Switches.
                    /// </summary>
                    public const int SubsurfaceSwitch = 5;
                    /// <summary>
                    /// Subtype code for SCADAmate Switches.
                    /// </summary>
                    public const int ScadaMateSwitch = 6;
                }
                public struct NormalPosition
                {
                    /// <summary>
                    /// Value indicating the normal position for a device is 'open'.
                    /// </summary>
                    public const string StatusOpen = "open";
                    /// <summary>
                    /// Value indicating the normal position for a device is 'closed'.
                    /// </summary>
                    public const string StatusClosed = "closed";
                }
                public struct Status
                {
                    public const int ProposedInstall = 0;
                    public const int ProposedChange = 1;
                    public const int ProposedRemove = 2;
                    public const int ProposedDeactivated = 3;
                }
                public struct Division
                {
                    /// <summary>
                    /// Unused.
                    /// </summary>
                    public const string DivisionName = "Division Name";
                }
            }

            /// <summary>
            /// The class model names will be held within a Coded Value Domain named Sempra Electric Object Class Model Name.            
            /// The model names should be prefixed with either T_ (Transmission), D_(Distribution), TC_ (Telcom) or S_(Substation) if they are network specific.
            /// </summary>
            public struct ClassModelNames
            {
                #region Classes

                public const string SessionConfig = "PGE_SESSIONCONFIG";

                public const string SessionQAQC = "PGE_Session_QAQC";

                public const string AnnotationStatusSync = "PGE_ANNOSTATUSSYNC";

                /// <summary>
                /// Used to identify the EDGIS.Substation feature class
                /// </summary>
                public const string Substation = "PGE_SUBSTATION";
                /// <summary>
                /// Used to identify the switch feature class.
                /// </summary>
                public const string Switch = "SWITCH";

                //Added FUSE
                public const string Fuse = "FUSE";

                //model names for replace with SB and replace with Fuse tool
                public const string PGEReplacedSwitch = "PGE_REPLACED_SB_FROM_FUSE";
                public const string PGEReplacedFuse = "PGE_REPLACED_FUSE_FROM_SB";
                
                /// <summary>
                /// Used to identify feature classes that store a circuit color in a field
                /// </summary>
                public const string PGECircuitColor = "PGE_CIRCUITCOLOR";

                /// <summary>
                /// Used to identify object classes which require the orphan row cleanup 
                /// </summary>
                public const string OrphanCleanup = "PGE_ORPHANCLEANUP";

                /// <summary>
                /// Used to identify orphan rows table 
                /// </summary>
                public const string OrphanRows = "PGE_ORPHANROWS";

                /// <summary>
                /// Used to identify objects classes that require relationship restored on split  
                /// </summary>
                public const string RestoreRels = "PGE_RESTORERELS";

                /// <summary>
                /// Used to identify feature classes that contain a FEEDERTYPE field, storing one of the four types of potential feeders
                /// </summary>
                public const string PGEFeederType = "PGE_FEEDERTYPE";

                /// <summary>
                /// Used to identify switch feature class (without OpenPoint)
                /// </summary>
                public const string PGESwitch = "PGE_SWITCH";

                /// <summary>
                /// Used to identify the dynamic protective devices in the geodatabase.
                /// </summary>
                public const string DynamicProtectiveDevice = "PGE_DYNAMICPROTECTIVEDEVICE";

                /// <summary>
                /// Used to identify the capacitor bank devices in the geodatabase.
                /// </summary>
                public const string CapacitorBank = "PGE_CAPACITORBANK";

                /// <summary>
                /// Used to identify the voltage regulator devices in the geodatabase.
                /// </summary>
                public const string VoltageRegulator = "PGE_VOLTAGEREGULATOR";

                /// <summary>
                /// Used to identify the primary conductors in the geodatabase.
                /// </summary>
                public const string PrimaryConductor = "PRIMARYCONDUCTOR";

                /// <summary>
                /// Used to identify the secondary feeder in the geodatabase.
                /// </summary>
                public const string SecondaryFeeder = "FEEDERSECONDARY";

                /// <summary>
                /// Used to identify the Conductor Info in the geodatabase.
                /// </summary>
                public const string ConductorInfo = "CONDUCTORINFO";

                /// <summary>
                /// Used to identify the Conductor in the geodatabase.
                /// </summary>
                public const string Conductor = "CONDUCTOR";

                /// <summary>
                /// Used to identify the Conductor in the geodatabase.
                /// </summary>
                public const string ValidateDataAndGeometry = "PGE_VALIDATE_DATA_AND_GEOMETRY";

                /// <summary>
                /// Used to identify overhead conductors in the geodatabase.
                /// </summary>
                public const string OverheadConductor = "OVERHEADCONDUCTOR";

                /// <summary>
                /// Used to identify the distribution transformer devices in the geodatabase.
                /// </summary>
                public const string DistributionTransformer = "DISTRIBUTIONTRANSFORMER";

                /// <summary>
                /// Used to identify the stepdown devices in the geodatabase.
                /// </summary>
                public const string StepDown = "PGE_STEPDOWN";

                /// <summary>
                /// Used to identify the DuctBankConfiguration object class in the geodatabase.
                /// </summary>
                public const string DuctBankConfiguration = "DUCTBANKCONFIGURATION";

                /// <summary>
                /// Used to identify the DuctDefinition object class in the geodatabase.
                /// </summary>
                public const string DuctDefinition = "PGE_DUCTDEFINITION";

                /// <summary>
                /// Used to identify the duct object class in the geodatabase.
                /// </summary>
                public const string Duct = "DUCT";

                /// <summary>
                /// Used to identify the Transformer feature class in the geodatabase.
                /// </summary>
                public const string Transformer = "TRANSFORMER";

                /// <summary>
                /// Used to identify the Transformer feature class in the geodatabase.
                /// </summary>
                public const string PGETransformer = "PGE_TRANSFORMER";

                /// <summary>
                /// Used to identify the TransformerUnit object class in the geodatabase.
                /// </summary>
                public const string TransformerUnit = "TRANSFORMERUNIT";

                /// <summary>
                /// Used to identify the Generator object class in the geodatabase.
                /// </summary>
                public const string Generator = "PGE_GENERATOR";

                /// <summary>
                /// Used to identify the Generation object class in the geodatabase.
                /// </summary>
                public const string Generation = "PGE_GENERATION";

                // Changes for ENOS to SAP migration - Adding generationinfo model name identifier
                /// <summary>
                /// Used to identify the Generation object class in the geodatabase.
                /// </summary>
                public const string GenerationInfo = "PGE_GENERATIONINFO";

                /// <summary>
                /// Used to identify the ServicePoint object class in the geodatabase.
                /// </summary>
                public const string ServicePoint = "PGE_SERVICEPOINT";

                /// <summary>
                /// Used to identify the ServicePoint object class in the geodatabase.
                /// </summary>
                public const string ElectricStitchPoint = "PGE_ELECTRICSTITCHPOINT";

                /// <summary>
                /// Used to identify the ProtectiveDevice object class in the geodatabase.
                /// </summary>
                public const string ProtectiveDevice = "PGE_PROTECTIVEDEVICE";

                /// <summary>
                /// Used to identify the PrimaryOHConductor feature class in the geodatabase.
                /// </summary>
                public const string TransformerLead = "PGE_TRANSFORMERLEAD";

                /// <summary>
                /// Used to identify the PrimaryOHConductor feature class in the geodatabase.
                /// </summary>
                public const string PrimaryOHConductor = "PGE_PRIOHCONDUCTOR";

                /// <summary>
                /// Used to identify the PrimaryUGConductor feature class in the geodatabase.
                /// </summary>
                public const string PrimaryUGConductor = "PGE_PRIMARYUGCONDUCTOR";

                /// <summary>
                /// Used to identify the SecondaryOHConductor feature class in the geodatabase.
                /// </summary>
                public const string SecondaryOHConductor = "PGE_SECONDARYOVERHEAD";

                /// <summary>
                /// Used to identify the SecondaryUGConductor feature class in the geodatabase.
                /// </summary>
                public const string SecondaryUGConductor = "PGE_SECONDARYUNDERGROUND";

                /// <summary>
                /// Used to identify the PrimaryOHConductorInfo Object class in the geodatabase.
                /// </summary>
                public const string PrimaryOHConductorInfo = "PGE_PRIOHCONDUCTORINFO";

                /// <summary>
                /// Used to identify the PrimaryUGConductorInfo Object class in the geodatabase.
                /// </summary>
                public const string PrimaryUGConductorInfo = "PGE_PRIUGCONDUCTORINFO";

                /// <summary>
                /// Used to identify the SecondaryOHConductorInfo Object class in the geodatabase.
                /// </summary>
                public const string SecondaryOHConductorInfo = "PGE_SECOHCONDUCTORINFO";

                /// <summary>
                /// Used to identify the SecondaryOHConductorInfo Object class in the geodatabase.
                /// </summary>
                public const string SecondaryUGConductorInfo = "PGE_SECUGCONDUCTORINFO";

                /// <summary>
                /// Used to identify the ValidationRulesForOperatingNumber Object class in the geodatabase.
                /// </summary> 
                public const string ValidationRulesForOperatingNumber = "PGE_VALIDATIONRULESFOROPERATINGNUMBER";

                #region Class model names for Electric protective Device
                /// <summary>
                /// Used to identify the DynamicProtectiveDevice,ELectricStitchPoint,Switch,Fuse,OpenPoint Object class in the geodatabase.
                /// </summary>
                public const string FdrmgProrectiveDevice = "FDRMGRPROTECTIVE";
                /// <summary>
                /// Identifies the object class as a protective device.
                /// </summary>
                public const string Protective = "PROTECTIVE";

                #endregion

                /// <summary>
                /// Used to identify the RelateSource Model Name in the geodatabase.
                /// </summary>
                public const string RelateSource = "PGE_RELATESOURCE";

                /// <summary>
                /// Used to identify the RelateDestination Model Name in the geodatabase.
                /// </summary>
                public const string RelateDestination = "PGE_RELATEDESTINATION";


                /// <summary>
                /// Used to identify the DeviceGroup Model Name in the geodatabase.
                /// </summary>
                public const string DeviceGroup = "PGE_DEVICEGROUP";

                /// <summary>
                /// Used to identify the CompledDevice Model Name in the geodatabase.
                /// </summary>
                public const string ComplexDevice = "PGE_COMPLEXDEVICE";

                /// <summary>
                /// Used to identify the DeviceGroupChild Model Name in the geodatabase.
                /// </summary>
                public const string DeviceGroupChild = "PGE_DEVICEGROUPCHILD";

                /// <summary>
                /// Used to identify the Voltage Regulator Model Name in the geodatabase.
                /// </summary>
                public const string StoreOriginal = "PGE_STOREORIGINALSSL";

                /// <summary>
                /// Identifies the object class as switchable device.
                /// </summary>
                public const string Switchable = "PGE_SWITCHABLE";

                /// <summary>
                /// Identifies the object class as a primary overhead conductor record.
                /// </summary>
                public const string PGEPriOHConductor = "PGE_PRIOHCONDUCTOR";

                /// <summary>
                /// Identifies the object class as a primary underground conductor record.
                /// </summary>
                public const string PGEPriUGConductor = "PGE_PRIUGCONDUCTOR";

                /// <summary>
                /// Identifies the object class as a bus bar.
                /// </summary>
                public const string PGEBusBar = "PGE_BUSBAR";

                /// <summary>
                /// Used to identify the CircuitSource Model Name in the geodatabase.
                /// </summary>
                public const string CircuitSource = "CIRCUITSOURCE";

                /// <summary>
                /// Used to identify the SubSurfaceStructure Model Name in the geodatabase.
                /// </summary>
                public const string SubSurfaceStructure = "PGE_SUBSURFACESTRUCTURE";

                /// <summary>
                /// Used to identify the PadMountStructure Model Name in the geodatabase.
                /// </summary>
                public const string PadMountStructure = "PGE_PADMOUNTSTRUCTURE";

                /// <summary>
                /// Used to identify the DistributionMap Model Name in the geodatabase.
                /// </summary>
                public const string DistributionMap = "PGE_DISTRIBUTIONMAP";

                /// <summary>
                /// Used to identify the DistMapUpdate Model Name in the geodatabase.
                /// </summary>
                public const string DistMapUpdate = "PGE_DISTMAPUPDATE";

                /// <summary>
                /// Used to identify the PGE_ValidateRegionalAttributes Model name in the geodatabase.
                /// </summary>
                public const string ValidateRegionalAttributes = "PGE_ValidateRegionalAttributes";
                /// <summary>
                /// Used to identify the Transformer & Primary Meter in the geodatabase.
                /// </summary>
                public const string CGC12ClassMN = "PGE_CGC12";
                /// <summary>
                /// Conductor model name assigned to conductor classes
                /// </summary>
                public const string PGEConductor = "PGE_CONDUCTOR";
                /// <summary>
                /// Conduit system model name
                /// </summary>
                public const string PGEConduitSystem = "PGE_CONDUITSYSTEM";
                /// <summary>
                /// In Service Child model name
                /// </summary>
                public const string PGEInServiceChild = "PGE_INSERVICE_CHILD";
                /// <summary>
                /// Validate Child model name
                /// </summary>
                public const string PGEValidateChildField = "PGE_VALIDATE_CHILD_FIELD";
                /// <summary>
                /// Installation Date Child model name
                /// </summary>
                public const string PGEInstallationDateChild = "PGE_INSTALLATIONDATE_CHILD";

                /// <summary>
                /// Used to identify the PGE_VALIDATEMAPGRID Model name in the geodatabase.
                /// </summary>
                public const string ValidateMapGrid = "PGE_VALIDATEMAPGRID";

                //ME Q2 2018 DA Item# 21-Prevent Duplicate SAP Equipment ID’s & PLDBID#’s
                /// <summary>
                /// Used to identify the SAP Equipment Id Model name in the geodatabase.
                /// </summary>
                public const string SapEquipIdClassMn = "PGE_SAPEQUIPID_CLASSMN";

                //ME Q2 2018 DA Item# 21-Prevent Duplicate SAP Equipment ID’s & PLDBID#’s
                /// <summary>
                /// Used to identify the PLDBID Model name in the geodatabase.
                /// </summary>
                public const string PldbIdClassMn = "PGE_PLDBID_CLASSMN";

                // ED Scripting Q2 2021 - Added to identify Classes for Validate Related Record Status.
                /// <summary>
                ///  Used to identify records that should be validated as a parent feature where the status must match children
                /// </summary>
                public const string ValidateStatusRelatedOrigin = "PGE_VALIDATE_STATUS_ORIGIN";

                // ED Scripting Q2 2021 - Added to identify Classes for Validate Related Record Status.
                /// <summary>
                ///  Used to identify records that should be validated as a child record where the status must match the parent
                /// </summary>
                public const string ValidateStatusRelatedDestination = "PGE_VALIDATE_STATUS_DEST";
                // ED Scripting Q2 2021 - Added to identify Classes for Validate Related Record Status.
                /// <summary>
                ///  Used to identify records that should be validated as a parent feature where the status must be Inservice if any children are inservice,
                ///  for example a DeviceGroup must be Inservice if a related device is Inservice, but there can be related proposed features
                /// </summary>
                public const string ValidateInserviceRelatedOrigin = "PGE_VALIDATE_INSERVICE_ORIGIN";
                // ED Scripting Q2 2021 - Added to identify Classes for Validate Related Record Status.
                /// <summary>
                ///  Used to identify records that should be validated as a child feature when the status is Inservice the parent must also be InService,
                ///  for example a DeviceGroup must be Inservice if a related child device is Inservice, but the parent could have proposed children devices
                /// </summary>
                public const string ValidateInserviceRelatedDestination = "PGE_VALIDATE_INSERVICE_DEST";

                
                /// <summary>
                ///  Used to validate Source/Target for which Attribute Copy is Applicable
                /// </summary>
                public const string AttributeCopySourceClass = "PGE_ACSource";

                /// <summary>
                ///  Used to validate Related Class for which Attribute Copy is Applicable
                /// </summary>
                public const string AttributeCopyRelatedClass = "PGE_ACRelated";


                #endregion

                public struct Scada
                {
                    #region SCADA Fields
                    /// <summary>
                    /// Used to identify a SCADA record
                    /// </summary>
                    public const string ScadaInfo = "PGE_SCADA";

                    /// <summary>
                    /// Used to identify a SCADA Controller object class
                    /// </summary>
                    public const string ScadaController = "PGE_SCADACONTROLLER";

                    /// <summary>
                    /// Used to identify a SCADA 'AssociatedDevice' object class
                    /// </summary>
                    public const string ScadaOperateableDevice = "PGE_SCADAOPERATEABLEDEVICE";

                    /// <summary>
                    /// Used to identify a device which can be assigned SCADA units.
                    /// </summary>
                    public const string OperateableDevice = "PGE_SCADAOPERATEABLEDEVICE";
                    #endregion
                }

                /// <summary>
                /// used to identify the class model name
                /// </summary>
                public const string GetFieldFromChild = "PGE_GETFIELD_FROM_CHILD";
                /// <summary>
                /// Used to identify a SupportStructure feature Class
                /// </summary>
                public const string SupportStructure = "PGE_SUPPORTSTRUCTURE";

                /// <summary>
                /// Used to identify a JointOwner feature Class
                /// </summary>
                public const string JointOwner = "PGE_JOINTOWNER";

                /// <summary>
                /// Used to identify Transformer feature Class
                /// </summary>
                public const string InServiceClsModelName = "PGE_INSERVICE";

                /// <summary>
                /// Used to identify ServiceLocation feature Class
                /// </summary>
                public const string ServiceLocation = "PGE_SERVICELOCATION";

                /// <summary>
                /// Used to identify Primary meter feature Class
                /// </summary>
                public const string PrimaryMeter = "PGE_PRIMARYMETER";

                /// <summary>
                /// Used to identify Line feature Class
                /// </summary>
                public const string LineStartPoint = "PGE_START_POINT";
                /// <summary>
                /// Used to identify Line feature Class
                /// </summary>
                public const string LineEndPoint = "PGE_END_POINT";
                /// <summary>
                /// Used to identify Line feature Class
                /// </summary>
                public const string LineMidPoint = "PGE_MID_POINT";

                /// <summary>
                /// Used to identify point feature Class
                /// </summary>
                public const string PointEnforceRegion = "PGE_ENFORCE_REGION";


                /// <summary>
                /// Used to identify feature Class
                /// </summary>
                public const string AssetInformation = "PGE_ASSETINFORMATION";

                /// <summary>
                /// Used to identify Proposed Change feature class
                /// </summary>
                public const string PropsedChange = "PGE_PROPOSEDCHANGE";

                /// <summary>
                /// Used to identify PGE COnfig Object class
                /// </summary>
                public const string PGEConfig = "PGE_CONFIG";


                /// <summary>
                /// Used to identify PGE_Appsettings Class
                /// </summary>
                public const string PgeAppsettings = "PGE_APPSETTING";

                /// <summary>
                /// Used to identify PGE_SESSION_DELETES_LOG Class
                /// </summary>
                public const string PgeSessionDeletesLog = "PGE_SESSION_DELETES_LOG";

                public const string SourceSideDevice = "PGE_SOURCESIDEDEVICE";

                /// <summary>
                /// Used to identify PGE_PRIMARYOVERHEAD Class model name
                /// </summary>
                public const string PrimaryOverHead = "PGE_PRIMARYOVERHEAD";

                /// <summary>
                /// Used to identify PGE_PRIMARYUNDERGROUND Class model name
                /// </summary>
                public const string PrimaryUnderGround = "PGE_PRIMARYUNDERGROUND";

                /// <summary>
                /// Used to identify PGE_SECONDARYOVERHEAD Class model name
                /// </summary>
                public const string SecondaryOverHead = "PGE_SECONDARYOVERHEAD";

                /// <summary>
                /// Used to identify PGE_SECONDARYUNDERGROUND Class model name
                /// </summary>
                public const string SecondaryUnderGround = "PGE_SECONDARYUNDERGROUND";
                /// <summary>
                /// Used to identify DonotUpdateModelName
                /// </summary>
                public const string DonotUpdateModelName = "PGE_DONOTUPDATE";
                /// <summary>
                /// Used to identify DonotCreateModelName
                /// </summary>
                public const string DonotCreateModelName = "PGE_DONOTCREATE";
                /// <summary>
                /// Used to identify DonotDeleteModelName
                /// </summary>
                public const string DonotDeleteModelName = "PGE_DONOTDELETE";
                /// <summary>
                /// Recongnizes the features assigned with this model names barrier while tracing electric network upstream/downstream
                /// </summary>
                public const string EdgeBarrierModelName = "PGE_TRACEEDGEBARRIER";

                /// <summary>
                /// Used to identify Installation date
                /// </summary>
                public const string validateChildField = "PGE_VALIDATE_CHILD_FIELD";
                /// <summary>
                /// Used to identify Child object Installation date
                /// </summary>
                public const string InstallationChildField = "PGE_INSTALLATIONDATE_CHILD";

                /// <summary>
                /// Class model name used for Schematics Change Detection. Use together with the
                /// version name field model name to detect if a feature class was 
                /// inserted into/updated/deleted from
                /// </summary>
                public const string SchematicsChangeDetection = "PGE_EDSCHEM_CHANGEDETECTION";

                /// <summary>
                /// Used to identify the CustomerAgreement table.
                /// </summary>
                public const string LocatableObject = "LOCATABLEOBJECT";

                /// <summary>
                /// Used for identifying following feature classes that can be relate to 
                /// //customeragreements of agreementtype = MLX 
                /// [DeliveryPoint, CustomerAgreementNumber, PadmountStructure, PriOHConductor, 
                /// PriUGConductor, SecOHConductor, SecUGConductor, ServiceLocation, 
                /// SubsurfaceStructure, SupportStructure and Transformer].
                /// </summary>
                public const string MLXRelateField = "PGE_MLX_RELATE";

                /// <summary>
                /// Used for identifying following feature classes that can be relate to 
                /// //customeragreements of agreementtype = AW: Applicant Warranty  
                /// [CustomerAgreementNumber, PadmountStructure, PriUGConductor, 
                /// SecUGConductor, SubsurfaceStructure].
                /// </summary>
                public const string AWRelateField = "PGE_AW_RELATE";

                /// <summary>
                /// Used for identifying following feature classes that can be relate to 
                /// //customeragreements of agreementtype = TF: Temporary Facility   
                /// [DeliveryPoint, CustomerAgreementNumber, PadmountStructure, PriOHConductor, 
                /// PriUGConductor, SecOHConductor, SecUGConductor, ServiceLocation, 
                /// SubsurfaceStructure, SupportStructure and Transformer].
                /// </summary>
                public const string TFRelateField = "PGE_TF_RELATE";

                /// <summary>
                /// Used for identifying following feature classes that can be relate to 
                /// //customeragreements of agreementtype = SF: Special Facility   
                /// [DeliveryPoint, CustomerAgreementNumber, PadmountStructure, PriOHConductor, 
                /// PriUGConductor, SecOHConductor, SecUGConductor, ServiceLocation, 
                /// SubsurfaceStructure, SupportStructure and Transformer].
                /// </summary>
                public const string SFRelateField = "PGE_SF_RELATE";

                /// <summary>
                /// Used for identifying following feature classes that can be relate to 
                /// //customeragreements of agreementtype = RPA: Riser Pole Agreement    
                /// [SupportStructure].
                /// </summary>
                public const string RPARelateField = "PGE_RPA_RELATE";

                /// <summary>
                /// Class model name used for locate Customer Agreement table.
                /// </summary>
                public const string CustomerAgreementObject = "PGE_CUST_AGREEMENT";

                /// <summary>
                /// Used to identify the OperatingNumber NOT to display in the Display Name.
                /// </summary>
                public const string NoOperatingNumberDisplay = "PGE_NOOPERATINGNUMBERDISPLAY";

                /// <summary>
                /// Used to identify the SecondaryLoadPoint feature class.
                /// </summary>
                public const string PGESecondaryLoadPoint = "PGE_SECONDARYLOADPOINT";

                /// <summary>
                /// Used to identify whether the Unit table has to be validated for Phase Designation.
                /// </summary>
                public const string PGEPhaseValidationForUnitTable = "PGE_PHASEVALIDATIONFORUNIT";

                /// <summary>
                /// Used to identify whether the Unit table has to be validated for Phase Designation.
                /// </summary>
                public const string PGEUnitTable = "PGE_UNIT";

                /// <summary>
                /// Used to identify the "EDGIS.DeactivatedElectricLineSegment" feature class.
                /// </summary>
                public const string PGEDeactivatedElectricLineSegment = "PGE_DEACTIVATEDELECTRICLINESEGMENT";

                public const string PGENeutralConductor = "PGE_NEUTRALCONDUCTOR";


                public const string ROBC = "PGE_ROBC";
                public const string PartialCurtailPoint = "PGE_PERTIALCURTAILPOINT";
                public const string OperatingNumber = "PGE_OPERATINGNUMBER";
                public const string CGC12 = "PGE_CGC12";

                /// <summary>
                /// Used to identify the feature classes where ESRI RULES need to be displayed
                /// as errors during PGE-Custom QA/QC Validation process
                /// </summary>
                public const string ESRI_Rules_Severity_Error = "ESRI_RULES_SEVERITY_ERROR";

                // ME Q1 2020 : For LiDAR Corrections
                public const string LiDARCORRECTIONS = "PGE_LiDARCORRECTIONS";

                // ME Q1 2020 : AmpYear

                /// <summary>
                /// Used to identify the Transformer feature class in the geodatabase.
                /// </summary>
                public const string PGENetworkProtector = "PGE_NETWORKPROTECTOR";


                /// <summary>
                /// Used to identify the Transformer feature class in the geodatabase.
                /// </summary>
                public const string PGEOpenPoint = "PGE_OPENPOINT";

                /// <summary>
                /// Used to identify the Transformer feature class in the geodatabase.
                /// </summary>
                public const string PGEPrimaryRiser = "PGE_PRIMARYRISER";

                /// <summary>
                /// Used to identify the Transformer feature class in the geodatabase.
                /// </summary>
                public const string PGEVaultPoly = "PGE_VAULTPOLY";

            }

            /// <summary>
            /// The field model names will be held within a Coded Value Domain named Electric Field Model Name.
            /// </summary>
            public struct FieldModelNames
            {
               
                    /// <summary>
                /// Used to identify the Duct Position 
                /// </summary>
                public const string BankCode = "PGE_BANKCODE";

                /// <summary>
                /// Used to identify the Duct Position 
                /// </summary>
                public const string DuctPosition = "DUCTPOSITION";

                /// <summary>
                /// Use to identify the feeder manager non traceable field
                /// </summary>
                public const string FeederManagerNonTraceable = "FDRMGRNONTRACEABLE";

                /// <summary>
                /// Used to identify the trace weight of a feature.
                /// </summary>
                public const string TraceWeight = "MMELECTRICTRACEWEIGHT";

                /// <summary>
                /// Used to identify feature classes that store a circuit color in a field
                /// </summary>
                public const string PGECircuitColor = "PGE_CIRCUITCOLOR";

                /// <summary>
                /// Used to identify feature classes that store a circuit name in a field
                /// </summary>
                public const string PGECircuitName = "PGE_CIRCUITNAME"; //S2NN: BugFix for CIRCUITNAME AU

                /// <summary>
                /// Used to identify the FEEDERTYPE field
                /// </summary>
                public const string PGEFeederType = "PGE_FEEDERTYPE";

                /// <summary>
                /// Used to identify the JointOwnerName field.
                /// </summary>
                public const string JOName = "PGE_JOINTOWNERNAME";

                /// <summary>
                /// Used to identify the CustomerOwned field.
                /// </summary>
                public const string CustomerOwned = "PGE_CUSTOMEROWNED";

                public const string SSDGUID = "PGE_SSDGUID";
                public const string SourceSideDeviceId = "PGE_SOURCESIDEDEVICEID";
                public const string ProtectiveSSD = "PGE_PROTECTIVESSD";
                public const string AutoProtectiveSSD = "PGE_AUTOPROTECTIVESSD";
                /// <summary>
                /// Used to identify the Job Number field.
                /// </summary>
                public const string JobNumber = "PGE_JOBNUMBER";
                /// <summary>
                /// Used to identify the ZGBPCTIMP
                /// </summary>
                public const string Zgbpctimp = "PGE_ZGBPCTIMP";
                /// <summary>
                /// Used to identify the ZHLPCTIMP
                /// </summary>
                public const string Zhlpctimp = "PGE_ZHLPCTIMP";
                /// <summary>
                /// Used to identify the ZHPCTIMP
                /// </summary>
                public const string Zhpctimp = "PGE_ZHPCTIMP";
                /// <summary>
                /// Used to identify the ZTPCTIMP
                /// </summary>
                public const string Ztpctimp = "PGE_ZTPCTIMP";
                /// <summary>
                /// Used to identify the ZLPCTIMP
                /// </summary>
                public const string Zlpctimp = "PGE_ZLPCTIMP";

                /// <summary>
                /// Used to identify the ConnectionCode field.
                /// </summary>
                public const string ConnectionCode = "PGE_CONNECTIONCODE";
                /// <summary>
                /// Used to identify the ConstructionType field.
                /// </summary>
                public const string ConstructionType = "PGE_CONSTRUCTIONTYPE";
                /// <summary>
                /// Used to identify the PrimaryVoltage field.
                /// </summary>
                public const string PrimaryVoltage = "PGE_PRIMARYVOLTAGE";

                /// <summary>
                /// Used to identify the OperatingVoltage field.
                /// </summary>
                public const string OperatingVoltage = "PGE_OPERATINGVOLTAGE";

                /// <summary>
                /// Used to identify the OperatingVoltage field.
                /// </summary>
                public const string OperatingVoltage4PrimaryValidation = "OPERATINGVOLTAGE";
                
                /// <summary>
                /// Used to identify the OperatingVoltage field.
                /// </summary>
                public const string OperatingVoltage4TransformerValidation = "OPERATINGVOLTAGE";
                /// <summary>
                /// Used to identify the CondutorUse field model name.
                /// </summary>
                public const string CondutorUse = "PGE_CONDUCTORUSE";

                /// <summary>
                /// Used to identify the CondutorCount field model name.
                /// </summary>
                public const string CondutorCount = "PGE_CONDUCTORCOUNT";

                /// <summary>
                /// Identifies the conductor code.
                /// </summary>
                public const string ConductorCode = "PGE_CONDUCTORCODE";

                /// <summary>
                /// Identifies the conductor material.
                /// </summary>
                public const string ConductorMaterial = "PGE_CONDUCTORMATERIAL";

                /// <summary>
                /// Identifies the conductor size.
                /// </summary>
                public const string ConductorSize = "PGE_CONDUCTORSIZE";

                /// <summary>
                /// Identifies the conductor type.
                /// </summary>
                public const string ConductorType = "PGE_CONDUCTORTYPE";

                /// <summary>
                /// Identifies the conductor insulation.
                /// </summary>
                public const string ConductorInsulation = "PGE_CONDUCTORINSULATION";

                /// <summary>
                /// Identifies the conductor rating.
                /// </summary>
                public const string ConductorRating = "PGE_CONDUCTORRATING";

                /// <summary>
                /// Used to identify the NumberOfPhases field model name.
                /// </summary>
                public const string NumberOfPhases = "PGE_NUMBEROFPHASES";

                /// <summary>
                /// Used to identify the OutputVoltage field.
                /// </summary>
                public const string OutputVoltage = "PGE_OUTPUTVOLTAGE";

                /// <summary>
                /// Added for Q2 - ED Scripting Project 
                /// Used to identify the LowSideVoltage field.
                /// </summary>
                public const string LowSideVoltage = "LOWSIDEVOLTAGE";

                /// <summary>
                /// Used to identify the SymbolNumberXML field.
                /// </summary>
                public const string SymbolNumberXML = "PGE_SYMBOL_NUMBER_XML";

                /// <summary>
                /// Used to identify the SymbolNumberFCName field.
                /// </summary>
                public const string SymbolNumberFCName = "PGE_SYMBOL_NUMBER_FCNAME";

                /// <summary>
                /// Used to identify the SecondaryVoltage field.
                /// </summary>
                public const string SecondaryVoltage = "PGE_SECONDARYVOLTAGE";

                /// <summary>
                /// Used to identify the SecondaryIDC field.
                /// </summary>
                public const string SecondaryIDC = "PGE_SECONDARYIDC";

                /// <summary>
                /// Used to identify the PhaseDesignation field.
                /// </summary>
                public const string PhaseDesignation = "PHASEDESIGNATION";

                /// <summary>
                /// Used to identify the OperatingNumber in the geodatabase.
                /// </summary>
                public const string OperatingNumber = "PGE_OPERATINGNUMBER";

                /// <summary>
                /// Used to identify the OperatingNumber in the geodatabase.
                /// </summary>
                public const string LocatorOperatingNumber = "PGE_LOCATOR_OPNUM";

                /// <summary>
                /// Used to identify the Division in the geodatabase.
                /// </summary>
                public const string LocatorDivision = "PGE_LOCATOR_DIVISION";

                /// <summary>
                /// Used to identify FeederID field.
                /// </summary>
                public const string FeederID = "FeederID";

                /// <summary>
                /// Used to identify FeederID field.
                /// </summary>
                public const string FeederID2 = "FeederID2";

                /// <summary>
                /// Used to identify FeederInfo field.
                /// </summary>
                public const string FeederInfo = "FeederInfo";

                /// <summary>
                /// Used to identify PGE_SUBSTATIONID field.
                /// </summary>
                public const string SubstationID = "PGE_SUBSTATIONID";

                /// <summary>
                /// Used to identify PGE_SUBSTATIONNAME field.
                /// </summary>
                public const string SubstationName = "PGE_SUBSTATIONNAME";

                /// <summary>
                /// Used to identify PGE_CIRCUITID field.
                /// </summary>
                public const string CircuitID = "PGE_CIRCUITID";

                /// <summary>
                /// Used to identify PGE_CIRCUITID2 field.
                /// </summary>
                public const string CircuitID2 = "PGE_CIRCUITID2";

                /// <summary>
                /// Used to identify FeederID field.
                /// </summary>
                public const string Division = "PGE_DIVISION";

                /// <summary>
                /// Used to identify LastFedFeeder field.
                /// </summary>
                public const string RetainedFeederIdModelName = "PGE_LastFedFeeder";

                /// <summary>
                /// Identifies the generator name field.
                /// </summary>
                public const string GeneratorName = "PGE_GENERATORNAME";

                /// <summary>
                /// Identifies the protection type field.
                /// </summary>
                public const string ProtectionType = "PGE_PROTECTIONTYPE";

                /// <summary>
                /// Used to identify the Capacitor Bank TotalKVAR field.
                /// </summary>
                public const string CapBankTotalKVAR = "PGE_TOTALKVAR";

                /// <summary>
                /// Used to identify the RelateCount Model Name in the geodatabase.
                /// </summary>
                public const string RelateCount = "PGE_RELATECOUNT";

                /// <summary>
                /// Used to identify the Normal position field.
                /// </summary>
                public const string NormalPosition = "PGE_COMBONORMALPOSTION";

                /// <summary>
                /// Used to identify the NormalpositionA field.
                /// </summary>
                public const string NormalpositionA = "NORMALPOSITION_A";

                /// <summary>
                /// Used to identify the NormalpositionB field.
                /// </summary>
                public const string NormalpositionB = "NORMALPOSITION_B";

                /// <summary>
                /// Used to identify the NormalpositionC field.
                /// </summary>
                public const string NormalPositionC = "NORMALPOSITION_C";

                /// <summary>
                /// Used to identify the LocalOfficeID field.
                /// </summary>
                public const string LocalOffice = "LOCALOFFICE";

                /// <summary>
                /// Used to identify the Voltage Regulator field.
                /// </summary>
                public const string OriginalSourceSide = "PGE_ORIGINALSSL";

                /// <summary>
                /// Used to identify the Sattus field.
                /// </summary>
                public const string Status = "PGE_STATUS";

                /// <summary>
                /// Used to identify the MapNumber field.
                /// </summary>
                public const string MapNumber = "PGE_DISTRIBUTIONMAPNUMBER";

                /// <summary>
                /// Used to identify the MapType field.
                /// </summary>
                public const string MapType = "PGE_MAPTYPE";

                /// <summary>
                /// Used to identify the GEMSDISTMAPNUM or MapNumber or DISTMAP field.
                /// </summary>
                public const string ElecMapNumber = "PGE_DISTRIBUTIONMAPNO";

                /// <summary>
                /// Used to identify the GEMSDISTMAPNUM or MapNumber field.
                /// </summary>
                public const string LocalOfficeID = "PGE_LOCALOFFICE";

                /// <summary>
                /// Used to identify the HFTD field.
                /// </summary>
                public const string HFTD = "PGE_HFTD";

                /// <summary>
                /// Used to identify the GEMSDISTMAPNUM or MapNumber field.
                /// </summary>
                public const string LocalOPOffice = "PGE_LOCALOPOFFICE";
                /// <summary>
                /// Field model name for MeasuredLatitude
                /// </summary>
                public const string MeasuredLatitude = "PGE_MEASUREDLATITUDE";

                /// <summary>
                /// Field model name for MeasuredLonitude
                /// </summary>
                public const string MeasuredLongitude = "PGE_MEASUREDLONGITUDE";

                /// <summary>
                /// Field model name for BearingAngle
                /// </summary>
                public const string BearingAngle = "PGE_BEARINGANGLE";

                /// <summary>
                /// Field model name for RangeToShape
                /// </summary>
                public const string RangetoShape = "PGE_RANGETOSHAPE_FT";

                /// <summary>
                /// Field model name for SourceAccuracy
                /// </summary>
                public const string SourceAccuracy = "PGE_SOURCEACCURACY";


                #region SCADA Fields
                /// <summary>
                /// Used to identify the ScadaType field on the SCADA object class
                /// </summary>
                public const string ScadaType = "PGE_SCADATYPE";

                /// <summary>
                /// Used to identify the ScadaComm field on the SCADA object class
                /// </summary>
                public const string ScadaCommunication = "PGE_SCADACOMMUNICATION";

                /// <summary>
                /// Used to identify the RTUType field on the SCADA object class
                /// </summary>
                public const string ScadaRtuType = "PGE_RTUTYPE";

                /// <summary>
                /// Used to identify the SupervisoryControl field on AssociatedDevice
                /// </summary>
                public const string ScadaSupervisoryControl = "PGE_SUPERVISORYCONTROL";

                /// <summary>
                /// Used to identify the AutoBooster field on VoltageRegulator
                /// </summary>
                public const string AutoBooster = "PGE_AUTOBOOSTER";

                /// <summary>
                /// Used to identify the StructureSize field on VoltageRegulator
                /// </summary>
                public const string StructureSize = "PGE_STRUCTURESIZE";

                /// <summary>
                /// Used to identify the radio manufacturer on a SCADA device.
                /// </summary>
                public const string ScadaRadioManufacturer = "PGE_RADIOMANUFACTURER";
                #endregion

                /// <summary>
                /// Used to identify the InstallationJobYear field.
                /// </summary>
                public const string InstallationJobYear = "PGE_INSTALLJOBYEAR";

                /// <summary>
                /// Used to identify the InstallationDate field.
                /// </summary>
                public const string InstallationDate = "PGE_INSTALLATIONDATE";

                /// <summary>
                /// Used to identify the year Manufactured field.
                /// </summary>
                public const string ManufacturedYear = "PGE_YEARMANUFACTURED";

                /// <summary>
                /// Used to identify the test date field.
                /// </summary>
                public const string TestDate = "PGE_CABLETESTDATE";

                /// <summary>
                /// Used to identify the windspeedrerateyear field.
                /// </summary>
                public const string WindSpeedRerateYr = "PGE_WINDSPEEDRERATEYEAR";

                /// <summary>
                /// Used to identify the windspeedcode field.
                /// </summary>
                public const string WindSpeedCode = "PGE_WINDSPEEDCODE";

                /// <summary>
                /// Used to identify the Manufacturer field.
                /// </summary>
                public const string Manufacturer = "PGE_MANUFACTURER";

                /// <summary>
                /// Used to identify the serial number field.
                /// </summary>
                public const string SerialNumber = "PGE_SERIALNUMBER";

                /// <summary>
                /// Used to identify the Dist Map field.
                /// </summary>
                public const string DistMap = "PGE_MAPNUMBER";

                /// <summary>
                /// Used to identify city field.
                /// </summary>
                public const string City = "PGE_CITY";

                /// <summary>
                /// Used to identify county field.
                /// </summary>
                public const string County = "PGE_COUNTY";

                /// <summary>
                /// Used to identify Zip field.
                /// </summary>
                public const string Zip = "PGE_ZIP";

                ///// <summary>
                ///// Used to identify LocalOfficeID field.
                ///// </summary>
                //public const string LocalOfficeID = "PGE_LOCALOFFICE";

                /// <summary>
                /// Used to identify District field.
                /// </summary>
                public const string District = "PGE_DISTRICT";

                /// <summary>
                /// Used to identify region field.
                /// </summary>
                public const string Region = "PGE_REGION";

                /// <summary>
                /// Used to identify SAP PARENT ID field.
                /// </summary>
                public const string SAPParentID = "PGE_SAPPARENTID";

                /// <summary>
                /// Identifies the installation type 
                /// </summary>
                public const string InstallType = "PGE_INSTALLATIONTYPE";

                /// <summary>
                /// Used to identify the CGC12 field in the geodatabase.
                /// </summary>
                public const string FieldModelCGC12 = "PGE_CGC12";

                //Inherit Regional Attributes Model names - Start
                /// <summary>
                /// Used to identify Inherit County field.
                /// </summary>
                public const string InheritCounty = "PGE_INHERITCOUNTY";

                /// <summary>
                /// Used to identify Inherit District field.
                /// </summary>
                public const string InheritDistrict = "PGE_INHERITDISTRICT";

                /// <summary>
                /// Used to identify Inherit Zip field.
                /// </summary>
                public const string InheritZip = "PGE_INHERITZIP";

                /// <summary>
                /// Used to identify Inherit Division field.
                /// </summary>
                public const string InheritDivision = "PGE_INHERITDIVISION";

                /// <summary>
                /// Used to identify Inherit Region field.
                /// </summary>
                public const string InheritRegion = "PGE_INHERITREGION";

                /// <summary>
                /// Used to identify Inherit City field.
                /// </summary>
                public const string InheritCity = "PGE_INHERITCITY";

                /// <summary>
                /// Used to identify Inherit LocalOffice field.
                /// </summary>
                public const string InheritLocalOffice = "PGE_INHERITLOCALOFFICE";

                //Inherit Regional Attributes Model names - End

                /// <summary>
                /// Used to identify M&S code field.
                /// </summary>
                public const string UpperCase = "PGE_UPPERCASE";

                /// <summary>
                /// Identifies the 'year installed' field on a fault indicator.
                /// </summary>
                public const string FaultIndicatorYearInstalled = "PGE_FAULTINDICATORYEARINSTALLED";
                /// <summary>
                /// Used to identify AllowFieldedit
                /// </summary>
                public const string AllowFieldedit = "PGE_ALLOWEDIT";
                /// <summary>
                /// Used to identify DuctSize field
                /// </summary>
                public const string DuctSize = "DUCTSIZE";
                /// <summary>
                /// Used to identify DuctMaterial field
                /// </summary>
                public const string DuctMaterial = "DUCTMATERIAL";

                /// <summary>
                /// Field model name for Schematics Change Detection.
                /// Use together with the schematics change detection class model name 
                /// to write the version into the specified field in which a feature class 
                /// was inserted/deleted/updated
                /// </summary>
                public const string VersionName = "PGE_VERSIONNAME";

                /// <summary>
                /// Used for Used to identify the AgreementNum.
                /// </summary>
                public const string AgreementNumber = "PGE_AGREEMENTNUM";
                //Changes After GO-Live
                /// <summary>
                /// Used for Used to identify the Elevation.
                /// </summary>
                public const string Elevation = "PGE_ELEVATION";

                /// <summary>
                /// Used for Used to identify the Elevation.
                /// </summary>
                public const string PLDBID = "PGE_PLDBID";

                //ME Q2 2018 DA Item# 21-Prevent Duplicate SAP Equipment ID’s & PLDBID#’s
                /// <summary>
                /// Used to identify Sap Equipment Id
                /// </summary>
                public const string UNIQUE_SAPEQUIPID = "PGE_UNIQUE_SAPEQUIPID";

                //ME Q2 2018 DA Item# 21-Prevent Duplicate SAP Equipment ID’s & PLDBID#’s
                /// <summary>
                /// Used to identify Sap Equipment Id
                /// </summary>
                public const string UNIQUE_PLDBID = "PGE_UNIQUE_PLDBID";

                //ME Q1 2020 : DA#200102
                public const string PGE_PSPS_SEGMENT = "PGE_PSPS_SEGMENT";

                public const string PGE_OWNERNAME = "PGE_OWNERNAME";
                public const string PGE_OWNERSTREETADDRESS = "PGE_OWNERSTREETADDRESS";
                public const string PGE_OWNERSTREETADDRESS2 = "PGE_OWNERSTREETADDRESS2";
                public const string PGE_OWNERSTATE = "PGE_OWNERSTATE";
                public const string PGE_OWNERPHONE = "PGE_OWNERPHONE";
                public const string PGE_POLNUMBER = "PGE_POLNUMBER";
                public const string PGE_AGREEMENTDATE = "PGE_AGREEMENTDATE";
                public const string PGE_PREMISEID = "PGE_PREMISEID";
                public const string PGE_OWNERCITY = "PGE_OWNERCITY";
                public const string PGE_OWNERZIP = "PGE_OWNERZIP";

                //ME Q2 2020: AmpYear
                public const string AMPYear = "PGE_AMPYEAR";

                /// <summary>
                /// ED Scripting Q2 2021 Used to identify the HighSideConfgiuration field.
                /// </summary>
                public const string HighSideConfiguration = "HIGHSIDECONFIGURATION";

                /// <summary>
                /// ED Scripting Q2 2021 Used to identify the RatedAmps field.
                /// </summary>
                public const string RatedAMPS = "PGE_RATEDAMPS";

                /// <summary>
                /// ED Scripting Q2 2021 Used to identify the LowSideConfgiuration field.
                /// </summary>
                public const string LowSideConfiguration = "LOWSIDECONFIGURATION";

                /// <summary>
                /// ED Scripting Q2 2021 Used to identify the ADMSLabel field.
                /// </summary>
                public const string ADMSLabel = "PGE_ADMSLABEL";

                //ME Q2 2021: EGIS-913
                public const string ClimateZone = "PGE_CLIMATEZONE";

                /// <summary>
                /// Attribute Copy Field for Validation Check
                /// </summary>
                public const string AttributeCopyValidateField = "PGE_ACValidateField";

                /// <summary>
                /// Attribute Copy Field for Attribute Copy to new or existing Related Record
                /// </summary>
                public const string AttributeCopyCopyField = "PGE_ACCopyField";

                //ME PSPS 2022: EGIS-1119
                public const string Barcode = "PGE_BARCODE";

            }

            /// <summary>
            /// The geometric networks that reside within the electric data model.
            /// </summary>
            public struct Networks
            {
                #region Fields

                /// <summary>
                /// The distribtion network name
                /// </summary>
                public const string Distribution = "ElectricDistNetwork";

                /// <summary>
                /// The substation network name
                /// </summary>
                public const string Substation = "SubstationNetwork";

                /// <summary>
                /// The telcommunication network name.
                /// </summary>
                public const string Telcom = "TelcomNetwork";

                /// <summary>
                /// The transmission network name.
                /// </summary>
                public const string Transmission = "ElectricTransNetwork";

                #endregion
            }

            /// <summary>
            /// The names used to identify relationship fields
            /// </summary>
            public struct RelationshipFields
            {
                #region Fields

                /// <summary>
                /// Used to identify the SchematicsDisplayIdc relationship attribute
                /// </summary>
                public const string SchematicsDisplayIdc = "SchematicsDisplayIdc";

                #endregion
            }

            /// <summary>
            /// The names used to identify relationships
            /// </summary>
            public struct RelationshipNames
            {

                public struct Scada
                {
                    #region Fields
                    /// <summary>
                    /// Identifies the VoltageRegulator to SCADA relationship.
                    /// </summary>
                    public const string VoltageRegulator = "VoltageRegulator_SCADA";
                    /// <summary>
                    /// Identifies the Switch to SCADA relationship.
                    /// </summary>
                    public const string Switch = "Switch_SCADA";
                    /// <summary>
                    /// Identifies the DynamicProtectiveDevice to SCADA relationship.
                    /// </summary>
                    public const string DynProtDev = "DynProtDev_SCADA";
                    /// <summary>
                    /// Identifies the CapacitorBank to SCADA relationship.
                    /// </summary>
                    public const string CapacitorBank = "CapacitorBank_SCADA";
                    #endregion
                }

                public struct ServicePoint
                {
                    #region Relationship Names
                    /// <summary>
                    /// Postfix identifying a relationship with ServicePoint.
                    /// </summary>
                    public const string FeatureRelatnship = "_ServicePoint";

                    #endregion
                }

                #region Fields

                /// <summary>
                /// Used to identify the DeviceContain_DynProtDev relationship
                /// </summary>
                public const string DeviceContain_DynProtDev = "DeviceContain_DynProtDev";

                /// <summary>
                /// Used to identify the TransformerGroundingBank relationship
                /// </summary>
                public const string TransformerGroundingBank = "Transformer_GroundingBank";

                /// <summary>
                /// Used to identify the PrimaryGeneration_ProtectiveDevice relationship
                /// </summary>
                public const string PrimaryGeneration_ProtectiveDevice = "PriGeneration_ProtectiveDevice";

                /// <summary>
                /// Used to identify the SecondaryGeneration_ProtectiveDevice relationship
                /// </summary>
                public const string SecondaryGeneration_ProtectiveDevice = "SecGeneration_ProtectiveDevice";

                /// <summary>
                /// Used to identify the ProtectiveDevice_Generator relationship
                /// </summary>
                public const string ProtectiveDevice_Generator = "ProtectiveDevice_Generator";

                /// <summary>
                /// Used to identify the Primary Overhead Conductor Conductor Info relationship
                /// </summary>
                public const string PrimaryOHConductor_CondictorInfo = "PriOHConductor_PriOHConductorInfo";

                /// <summary>
                /// Used to identify the Primary Underground Conductor Conductor Info relationship
                /// </summary>
                public const string PrimaryUGConductor_CondictorInfo = "PriUGConductor_PriUGConductorInfo";

                /// <summary>
                /// Used to identify the Secondary Overhead Conductor Conductor Info relationship
                /// </summary>
                public const string SecondaryOHConductor_CondictorInfo = "SecOHConductor_SecOHConductorInfo";

                /// <summary>
                /// Used to identify the SecondaryUnderground Conductor Conductor Info relationship
                /// </summary>
                public const string SecondaryUGConductor_CondictorInfo = "SecUGConductor_SecUGConductorInfo";

                ///// <summary>
                ///// Used to identify the  Transformer and ServicePoint relationship
                ///// </summary>
                //public const string Transformer_ServicePoint = "Transformer_ServicePoint";

                #endregion
            }

            #endregion

            #region PGEPreserveRelationship

            public const string UlsObjectId = "ULSOBJECTID";
            public const string UlsPosition = "ULS_POSITION";
            public const string Phasedesignation = "PHASEDESIGNATION";
            public const string Rel_BackwardPathLabel = "Conduit System";
            public const string UGObjectId = "UGOBJECTID";
            #endregion

        }

        #region UFM Model Names

        public struct UFM
        {
            public struct ClassModelNames
            {
                public const string Conduit = "ULS";
                public const string CrossSection = "CROSSSECTION";
                public const string CrossSection10 = "CROSSSECTION10";
                public const string CrossSection50 = "CROSSSECTION50";
                public const string DcConductor = "PGE_DCCONDUCTOR";
                public const string PgeCircuitColor = "PGE_CIRCUITCOLOR";
                public const string PrimaryConductor = "PGE_PRIMARYUGCONDUCTOR";
                public const string SecondaryConductor = "PGE_SECONDARYUGCONDUCTOR";
                public const string UfmDuct = "UFMDUCT";
                public const string UfmDuctBank = "UFMDUCTBANK";
                public const string UfmWall = "UFMWALL";
                public const string UfmFloor = "UFMFLOOR";
                public const string UlsMember = "ULSMEMBER";
                public const string VaultPoly = "PGE_VAULTPOLY";
                // QAQC- INC000004371045 - EDGIS QA/QC Rule to allow Loops
                public const string SEC_GRID_SPOT_NTWRK = "SEC_GRID_SPOT_NTWRK";

            }

            public struct FieldModelNames
            {
                public const string CircuitId = "FEEDERID";
                public const string CircuitName = "CIRCUITNAME";
                public const string CircuitName2 = "PGE_CIRCUITNAME";
                public const string ConversionCircuitID = "PGE_XSECTIONANNOCIRCUITID";
                public const string DirectBuriedIdc = "PGE_DIRECTBURYIDC";
                public const string Direction = "DIRECTION";
                public const string DuctBankConfig = "DUCTBANKCONFIGURATION";
                public const string DuctId = "DUCTID";
                public const string DuctPosition = "DUCTPOSITION";
                public const string DuctSync = "PGE_DUCTSYNCATTR";
                public const string FacilityId = "FACILITYID";
                public const string FeatureId = "FEATUREID";
                public const string LabelText = "LABELTEXT";
                public const string Occupied = "DUCTOCCUPIED";
                public const string StructureNumber = "PGE_STRUCTURENUMBER";
                public const string UlsText = "ULSTEXTFIELD";
                public const string FeederType = "PGE_FEEDER_TYPE";
            }
        }

        #endregion
    }
}
