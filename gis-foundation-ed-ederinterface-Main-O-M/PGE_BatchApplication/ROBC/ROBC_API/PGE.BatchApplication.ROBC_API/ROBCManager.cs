using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using Miner.Interop;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.Geodatabase;
using System.Runtime.InteropServices;
using Miner.Interop.Process;
using Miner.Geodatabase;
using PGE.BatchApplication.ROBC_API.DatabaseRecords;
using ESRI.ArcGIS.Geometry;
using System.Diagnostics;
using Oracle.DataAccess.Client;


namespace PGE.BatchApplication.ROBC_API
{
    public class ROBCManager
    {
        #region PrivateVariables
        private static bool _bGDBM = false;
        private static LicenseInitializer m_AOLicenseInitializer = null;
        private OracleConnectionControl _EderSubOracleConnection = null;
        private OracleConnectionControl _EderOracleConnection = null;
        private IWorkspace _EderDatabaseConnection = null;
        private IWorkspace _EderSubDatabaseConnection = null;
        private IGeometricNetwork _EderGeomNetwork = null;
        private IGeometricNetwork _EDERSubGeomNetwork = null;
        //private IWorkspace edpubDatabaseConnection = null;
        private string _EDDatabaseUserName = "";
        private string _EDSubstationDatabaseUserName = "";
      //  private string _strEDERGeomNetwork = "";
        //private string _strSUBGeomNetwork = "";
        private string[] _ListDevicePotentialTables = { "EDGIS.SWITCH", "EDGIS.DYNAMICPROTECTIVEDEVICE" };
        private int _ClassIDTransformer = -1;
        private int _ClassIDPrimaryMeter = -1;
        private int _ClassIDDCRectifier = -1;
        private int _ClassIDEDStitchPoint = -1;

        private string _PCPGUIDModelName = "PGE_PCPGUID";                           // OBJECT MODEL NAME FOR FEATURE CLASSES WHICH HAVE PCP GUID FIELD          
        private int _XFR_RegistrationID = 0;                                        // THE SDE TABLE REGISTRATION FOR THE TRANSFORMER FEATURE CLASS TABLE
        private int _PRIMETER_RegistrationID = 0;                                   // THE SDE TABLE REGISTRATION FOR THE TRANSFORMER FEATURE CLASS TABLE


        #endregion

        #region Public Variables

        public const string _TableNameEDNetwork = "EDGIS.ZZ_MV_N_1_DESC";
        public const string _TableNameTransformerMVView = "EDGIS.ZZ_MV_TRANSFORMER";
        public const string _TableNamePrimaryMeterMVView = "EDGIS.ZZ_MV_PRIMARYMETER";
        public const string _TableNameDCRectifierMVView = "EDGIS.ZZ_MV_DCRECTIFIER";
        public const string _TableNameCIRCUITSOURCEMVView = "EDGIS.ZZ_MV_CIRCUITSOURCE";
        public const string _TableNameGDBITEMS = "SDE.GDB_ITEMS";
        public const string _TableNameSERVICEPOINT = "EDGIS.SERVICEPOINT";

        // public const string _TableNameSERVICEPOINT = "EDGIS.SERVICEPOINT";


        public const string _FC_EDER_SourceName = "EDGIS.ELECTRICSTITCHPOINT";
        public const string _FC_Substation_SourceName = "EDGIS.SUBELECTRICSTITCHPOINT";
        public const string _ROBCEssentialCustomer = "50";
        public const string _ROBCFixedCircuit = "50";
        public const int _ROBCEssentialCustomerCode = 50;
        public const int _ROBCFixedCircuitCode = 50;
        public const string _PhysNameTransformer = "EDGIS.TRANSFORMER";
        public const string _PhysNamePrimaryMeter = "EDGIS.PRIMARYMETER";
        public const string _PhysNameDCRectifier = "EDGIS.DCRECTIFIER";
        public const string _PhysNameROBC = "EDGIS.ROBC";
        public const string _PhysNameSUBINTERUPTINGDEVICE = "EDGIS.SUBINTERRUPTINGDEVICE";
        public const string _PhysNameCircuitSource = "EDGIS.CIRCUITSOURCE";
        public const string _PhysNameServiceLocation = "EDGIS.SERVICELOCATION";
        public const string _FieldName_tbSP_TRANSFORMERGUID = "TRANSFORMERGUID";
        public const string _FieldName_tbSP_PRIMARYMETERGUID = "PRIMARYMETERGUID";
        public const string _FieldName_tbSP_ESSENTIALCUSTOMER = "ESSENTIALCUSTOMERIDC";
        public const string _FieldName_tbSP_DCRECTIFIERGUID = "DCRECTIFIERGUID";
        public const string _FieldName_tbXFMR_WINTERKVA = "WINTERKVA";
        public const string _FieldName_tbXFMR_SUMMERKVA = "SUMMERKVA";
        public const string _FieldName_tbSUBINT_SCADAIDC = "SCADAIDC";
        public const string _FieldName_tbROBC_CircuitSourceGUID = "CIRCUITSOURCEGUID";

        public enum NetworkSourceType
        {
            EDER = 0,
            SUBSTATION = 1,
            EDPUB = 2
        }

        public enum TraceType
        {
            PARENT = 0,
            CHILD = 1
        }

        #endregion

        #region Constructors

        /// <summary>
        /// This constructor will throw an error if it can't check out an ArcGIS and ArcFM license.
        /// <param name="EDERDatabaseTNSName">TNS Name for the EDER Database</param>
        /// <param name="EDERUserName">User name for EDER</param>
        /// <param name="EDERPassword">Password for EDER</param>
        /// <param name="EDSubstationDatabaseTNSName">TNS Name for the Substation Database</param>
        /// <param name="EDSubstationUserName">User name for Substation</param>
        /// <param name="EDSubstationPassword">Password for Substation</param>
        /// <param name="EDERGeomNetwork">Dataset\\NetworkName for EDER</param>
        /// <param name="SUBGeomNetwork">Dataset\\NetworkName for Substation</param>
        /// </summary>
        public ROBCManager(string EDERDatabaseTNSName, string EDERUserName, string EDERPassword,
            string EDSubstationDatabaseTNSName, string EDSubstationUserName, string EDSubstationBPassword,
            string EDERGeomNetwork, string SUBGeomNetwork)
        {
            Initialize(EDERDatabaseTNSName, EDERUserName, EDERPassword,
                EDSubstationDatabaseTNSName, EDSubstationUserName, EDSubstationBPassword,
                EDERGeomNetwork, SUBGeomNetwork,
                esriLicenseProductCode.esriLicenseProductCodeAdvanced,
                mmLicensedProductCode.mmLPArcFM);
        }

        /// <summary>
        /// This constructor is for the ROBC using Change Detection
        /// </summary>
        /// <param name="ederWorkspace"></param>
        /// <param name="edersubWorkspace"></param>
        /// <param name="ederOracleConnection"></param>
        /// <param name="edSubOracleConnection"></param>
        /// <param name="EDERGeometricNetworkName"></param>
        /// <param name="EDERSUBGeometricNetworkName"></param>
        /// <param name="bGDBM"></param>
        public ROBCManager(
            IWorkspace ederWorkspace,
            IWorkspace edersubWorkspace,
            OracleConnection ederOracleConnection,
            OracleConnection edSubOracleConnection,
            string EDERGeometricNetworkName,
            string EDERSUBGeometricNetworkName,
            bool bGDBM)
        {

            _EderDatabaseConnection = ederWorkspace;
            _EderSubDatabaseConnection = edersubWorkspace;

            Initialize(
                ederOracleConnection,
                edSubOracleConnection,
                EDERGeometricNetworkName,
                EDERSUBGeometricNetworkName,
                esriLicenseProductCode.esriLicenseProductCodeAdvanced,
                mmLicensedProductCode.mmLPArcFM,
                bGDBM);
        }

        /// <summary>
        /// This constructor is for the ROBC using Change Detection
        /// </summary>
        /// <param name="ederWorkspace"></param>
        /// <param name="edersubWorkspace"></param>
        /// <param name="ederOracleConnection"></param>
        /// <param name="edSubOracleConnection"></param>
        /// <param name="EDERGeometricNetworkName"></param>
        /// <param name="EDERSUBGeometricNetworkName"></param>
        /// <param name="bGDBM"></param>
        public ROBCManager(
            IWorkspace ederWorkspace,
            IWorkspace edersubWorkspace,
            OracleConnection ederOracleConnection,
            OracleConnection edSubOracleConnection,
            string EDERGeometricNetworkName,
            string EDERSUBGeometricNetworkName,
            bool bGDBM,bool licenseCheckout)
        {

            _EderDatabaseConnection = ederWorkspace;
            _EderSubDatabaseConnection = edersubWorkspace;

            Initialize(
                ederOracleConnection,
                edSubOracleConnection,
                EDERGeometricNetworkName,
                EDERSUBGeometricNetworkName,
                esriLicenseProductCode.esriLicenseProductCodeAdvanced,
                mmLicensedProductCode.mmLPArcFM,
                bGDBM,licenseCheckout);
        }

        /// <summary>
        /// This constructor will throw an error if it can't check out an ArcGIS and ArcFM license. 
        /// <param name="EDERDatabaseTNSName">TNS Name for the EDER Database</param>
        /// <param name="EDERUserName">User name for EDER</param>
        /// <param name="EDERPassword">Password for EDER</param>
        /// <param name="EDSubstationDatabaseTNSName">TNS Name for the Substation Database</param>
        /// <param name="EDSubstationUserName">User name for Substation</param>
        /// <param name="EDSubstationPassword">Password for Substation</param>
        /// <param name="EDERGeomNetwork">Dataset\\NetworkName for EDER</param>
        /// <param name="SUBGeomNetwork">Dataset\\NetworkName for Substation</param>
        /// </summary>
        public ROBCManager(string EDERDatabaseTNSName, string EDERUserName, string EDERPassword,
            string EDSubstationDatabaseTNSName, string EDSubstationUserName, string EDSubstationBPassword,
            string EDERGeomNetwork, string SUBGeomNetwork, bool bGDBM)
        {
            Initialize(EDERDatabaseTNSName, EDERUserName, EDERPassword,
                EDSubstationDatabaseTNSName, EDSubstationUserName, EDSubstationBPassword,
                EDERGeomNetwork, SUBGeomNetwork,
                esriLicenseProductCode.esriLicenseProductCodeAdvanced,
                mmLicensedProductCode.mmLPArcFM, bGDBM);
        }

        /// <summary>
        /// This constructor will throw an error if it can't check out an ArcGIS and ArcFM license.
        /// <param name="EDERDatabaseTNSName">TNS Name for the EDER Database</param>
        /// <param name="EDERUserName">User name for EDER</param>
        /// <param name="EDERPassword">Password for EDER</param>
        /// <param name="EDSubstationDatabaseTNSName">TNS Name for the Substation Database</param>
        /// <param name="EDSubstationUserName">User name for Substation</param>
        /// <param name="EDSubstationPassword">Password for Substation</param>
        /// <param name="EDERGeomNetwork">Dataset\\NetworkName for EDER</param>
        /// <param name="SUBGeomNetwork">Dataset\\NetworkName for Substation</param>
        /// <param name="EsriLicense">Esri License to check out</param>
        /// <param name="ArcFMLicense">ArcFM License to check out</param>
        /// </summary>
        public ROBCManager(string EDERDatabaseTNSName, string EDERUserName, string EDERPassword,
            string EDSubstationDatabaseTNSName, string EDSubstationUserName, string EDSubstationPassword,
            string EDERGeomNetwork, string SUBGeomNetwork,
            esriLicenseProductCode EsriLicense,
            mmLicensedProductCode ArcFMLicense)
        {
            Initialize(EDERDatabaseTNSName, EDERUserName, EDERPassword,
                EDSubstationDatabaseTNSName, EDSubstationUserName, EDSubstationPassword,
                EDERGeomNetwork, SUBGeomNetwork,
                EsriLicense, ArcFMLicense);
        }

        /// <summary>
        /// This constructor will throw an error if it can't check out an ArcGIS and ArcFM license.
        /// <param name="EDERDatabaseTNSName">TNS Name for the EDER Database</param>
        /// <param name="EDERUserName">User name for EDER</param>
        /// <param name="EDERPassword">Password for EDER</param>
        /// <param name="EDSubstationDatabaseTNSName">TNS Name for the Substation Database</param>
        /// <param name="EDSubstationUserName">User name for Substation</param>
        /// <param name="EDSubstationPassword">Password for Substation</param>
        /// <param name="EDERGeomNetwork">Dataset\\NetworkName for EDER</param>
        /// <param name="SUBGeomNetwork">Dataset\\NetworkName for Substation</param>
        /// <param name="EsriLicense">Esri License to check out</param>
        /// <param name="ArcFMLicense">ArcFM License to check out</param>
        /// </summary>
        public ROBCManager(string EDERDatabaseTNSName, string EDERUserName, string EDERPassword,
            string EDSubstationDatabaseTNSName, string EDSubstationUserName, string EDSubstationPassword,
            string EDERGeomNetwork, string SUBGeomNetwork,
            esriLicenseProductCode EsriLicense,
            mmLicensedProductCode ArcFMLicense, bool bGDBM)
        {
            Initialize(EDERDatabaseTNSName, EDERUserName, EDERPassword,
                EDSubstationDatabaseTNSName, EDSubstationUserName, EDSubstationPassword,
                EDERGeomNetwork, SUBGeomNetwork,
                EsriLicense, ArcFMLicense, bGDBM);
        }

        ~ROBCManager()
        {
            if (_bGDBM == false)
            {
                if (_EderGeomNetwork != null)
                {
                    while (Marshal.ReleaseComObject(_EderGeomNetwork) > 0)
                    {
                    }
                    _EderGeomNetwork = null;
                }

                if (_EDERSubGeomNetwork != null)
                {
                    while (Marshal.ReleaseComObject(_EDERSubGeomNetwork) > 0)
                    {
                    }
                    _EDERSubGeomNetwork = null;
                }

                if (_EderDatabaseConnection != null)
                {
                    while (Marshal.ReleaseComObject(_EderDatabaseConnection) > 0)
                    {
                    }
                    _EderDatabaseConnection = null;
                }

                if (_EderSubDatabaseConnection != null)
                {
                    while (Marshal.ReleaseComObject(_EderSubDatabaseConnection) > 0)
                    {
                    }
                    _EderSubDatabaseConnection = null;
                }

                if (m_AOLicenseInitializer != null)
                {
                    m_AOLicenseInitializer.ReleaseArcFMLicense();
                    m_AOLicenseInitializer.ShutdownApplication();
                }
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Initializes database connection to the EDER and EDPUB databases as well as checks out an ArcFM and Esri license
        /// </summary>
        private void Initialize(string EDERDatabaseTNSName, string EDERUserName, string EDERPassword,
            string EDSubstationDatabaseTNSName, string EDSubstationUserName, string EDSubstationBPassword,
            string EDERGeomNetwork, string SUBGeomNetwork,
            esriLicenseProductCode EsriLicense,
            mmLicensedProductCode ArcFMLicense)
        {
            Initialize(EDERDatabaseTNSName, EDERUserName, EDERPassword,
            EDSubstationDatabaseTNSName, EDSubstationUserName, EDSubstationBPassword,
             EDERGeomNetwork, SUBGeomNetwork,
             EsriLicense,
             ArcFMLicense, false);
        }

        /// <summary>
        /// Returns true if the Record needs to be stored for the supplied has essential customer logic.
        /// </summary>
        /// <param name="robcRecord"> ROBCRecord by reference to check and update</param>
        /// <param name="bHasEssentialCustomer">True means there is an essential customer</param>
        /// <returns>True when the record needs to be stored, something changed</returns>
        private bool configureROBCRecordValues(ref ROBCRecord robcRecord, bool bHasEssentialCustomer)
        {
            bool bNeedToUpdate = false;
            if (robcRecord.GetFieldValue("DESIREDROBC") != null)
            {
                if (robcRecord.GetFieldValue("DESIREDROBC").ToString() != _ROBCFixedCircuit)
                {
                    if (bHasEssentialCustomer)
                    {
                        if (robcRecord.GetFieldValue("ESTABLISHEDROBC") == null || robcRecord.GetFieldValue("ESTABLISHEDROBC").ToString() != _ROBCEssentialCustomer)
                        {
                            bNeedToUpdate = true;
                            robcRecord.SetFieldValue("ESTABLISHEDROBC", _ROBCEssentialCustomerCode);
                        }
                        if (robcRecord.GetFieldValue("ESTABLISHEDSUBBLOCK") != null)
                        {
                            if (!string.IsNullOrEmpty(robcRecord.GetFieldValue("ESTABLISHEDSUBBLOCK").ToString()))
                            {
                                bNeedToUpdate = true;
                                robcRecord.SetFieldValue("ESTABLISHEDSUBBLOCK", "");
                            }
                        }

                    }
                    else
                    {
                        if (robcRecord.GetFieldValue("DESIREDROBC") != null)
                        {

                            if (robcRecord.GetFieldValue("ESTABLISHEDROBC") == null || (robcRecord.GetFieldValue("ESTABLISHEDROBC").ToString() != robcRecord.GetFieldValue("DESIREDROBC").ToString()))
                            {
                                bNeedToUpdate = true;
                                robcRecord.SetFieldValue("ESTABLISHEDROBC", robcRecord.GetFieldValue("DESIREDROBC"));
                            }
                        }
                        else
                        {
                            bNeedToUpdate = true;
                            robcRecord.SetFieldValue("ESTABLISHEDROBC", robcRecord.GetFieldValue("DESIREDROBC"));
                        }
                        if (robcRecord.GetFieldValue("DESIREDSUBBLOCK") != null)
                        {
                            if (robcRecord.GetFieldValue("ESTABLISHEDSUBBLOCK") == null || robcRecord.GetFieldValue("ESTABLISHEDSUBBLOCK").ToString() != robcRecord.GetFieldValue("DESIREDSUBBLOCK").ToString())
                            {
                                bNeedToUpdate = true;
                                robcRecord.SetFieldValue("ESTABLISHEDSUBBLOCK", robcRecord.GetFieldValue("DESIREDSUBBLOCK"));
                            }
                        }
                        else
                        {
                            if (robcRecord.GetFieldValue("ESTABLISHEDSUBBLOCK") != null)
                            {
                                bNeedToUpdate = true;
                                robcRecord.SetFieldValue("ESTABLISHEDSUBBLOCK", "");
                            }
                        }
                    }
                }
                else
                {
                    if (robcRecord.GetFieldValue("ESTABLISHEDROBC") != null)
                    {
                        if (robcRecord.GetFieldValue("ESTABLISHEDROBC").ToString() != _ROBCFixedCircuit)
                        {
                            robcRecord.SetFieldValue("ESTABLISHEDROBC", _ROBCFixedCircuit);
                            robcRecord.SetFieldValue("ESTABLISHEDSUBBLOCK", "");
                            bNeedToUpdate = true;
                        }
                    }
                    else
                    {
                        robcRecord.SetFieldValue("ESTABLISHEDROBC", _ROBCFixedCircuit);
                        robcRecord.SetFieldValue("ESTABLISHEDSUBBLOCK", "");
                        bNeedToUpdate = true;
                    }
                }
            }
            return bNeedToUpdate;
        }

        /// <summary>
        /// Initializes database connection to the EDER and EDPUB databases as well as checks out an ArcFM and Esri license
        /// </summary>
        private void Initialize(string EDERDatabaseTNSName, string EDERUserName, string EDERPassword,
            string EDSubstationDatabaseTNSName, string EDSubstationUserName, string EDSubstationBPassword,
            string EDERGeomNetwork, string SUBGeomNetwork,
            esriLicenseProductCode EsriLicense,
            mmLicensedProductCode ArcFMLicense, bool bGDBM)
        {
            _bGDBM = bGDBM;
            _EDDatabaseUserName = EDERUserName;
            _EDSubstationDatabaseUserName = EDSubstationUserName;
            if (_bGDBM == false)
            {
                CheckOutLicense(EsriLicense, ArcFMLicense);
            }
            ConnectToDatabase(ref _EderDatabaseConnection, EDERDatabaseTNSName, EDERUserName, EDERPassword);
            ConnectToDatabase(ref _EderSubDatabaseConnection, EDSubstationDatabaseTNSName, EDSubstationUserName,
                EDSubstationBPassword);
            _EderSubOracleConnection = new OracleConnectionControl(EDERDatabaseTNSName, EDERUserName, EDERPassword,
                EDSubstationDatabaseTNSName, EDSubstationUserName, EDSubstationBPassword);
            getGeometricNetwork(_EderDatabaseConnection, EDERGeomNetwork, ref _EderGeomNetwork);
            getGeometricNetwork(_EderSubDatabaseConnection, SUBGeomNetwork, ref _EDERSubGeomNetwork);

            _EderOracleConnection = new OracleConnectionControl(EDERDatabaseTNSName, EDERUserName,
                EDERPassword, EDSubstationDatabaseTNSName, EDSubstationUserName, EDSubstationBPassword);
            try
            {
                ITable tempTransfomer = GetTable(_PhysNameTransformer, _EderDatabaseConnection);
                IFeatureClass fcTransfomer = (IFeatureClass)tempTransfomer;
                _ClassIDTransformer = fcTransfomer.FeatureClassID;

                ITable tempPrimaryMeter = GetTable(_PhysNamePrimaryMeter, _EderDatabaseConnection);
                IFeatureClass fcPrimaryMeter = (IFeatureClass)tempPrimaryMeter;
                _ClassIDPrimaryMeter = fcPrimaryMeter.FeatureClassID;

                ITable tempDCRectifier = GetTable(_PhysNameDCRectifier, _EderDatabaseConnection);
                IFeatureClass fcDCRectifier = (IFeatureClass)tempDCRectifier;
                _ClassIDDCRectifier = fcDCRectifier.FeatureClassID;

                ITable tempElectStitchPoint = GetTable(_FC_EDER_SourceName, _EderDatabaseConnection);
                IFeatureClass fcElectStitchPoint = (IFeatureClass)tempElectStitchPoint;
                _ClassIDEDStitchPoint = fcElectStitchPoint.FeatureClassID;


                if (tempTransfomer != null) { Marshal.FinalReleaseComObject(tempTransfomer); tempTransfomer = null; }
                if (tempPrimaryMeter != null) { Marshal.FinalReleaseComObject(tempPrimaryMeter); tempPrimaryMeter = null; }
                if (tempDCRectifier != null) { Marshal.FinalReleaseComObject(tempDCRectifier); tempDCRectifier = null; }
                if (tempElectStitchPoint != null) { Marshal.FinalReleaseComObject(tempElectStitchPoint); tempElectStitchPoint = null; }
                if (fcTransfomer != null) { Marshal.FinalReleaseComObject(fcTransfomer); fcTransfomer = null; }
                if (fcPrimaryMeter != null) { Marshal.FinalReleaseComObject(fcPrimaryMeter); fcPrimaryMeter = null; }
                if (fcDCRectifier != null) { Marshal.FinalReleaseComObject(fcDCRectifier); fcDCRectifier = null; }
                if (fcElectStitchPoint != null) { Marshal.FinalReleaseComObject(fcElectStitchPoint); fcElectStitchPoint = null; }
            }
            catch (Exception EX)
            {
            }

        }

        /// <summary>
        /// Initializes database connection to the EDER and EDPUB databases as well as checks out an ArcFM and Esri license
        /// </summary>
        private void Initialize(
            OracleConnection ederOracleConnection,
            OracleConnection ederSubOracleConnection,
            string EDERGeomNetwork,
            string SUBGeomNetwork,
            esriLicenseProductCode EsriLicense,
            mmLicensedProductCode ArcFMLicense,
            bool bGDBM)
        {
            _bGDBM = bGDBM;
            if (_bGDBM == false)
            {
                CheckOutLicense(EsriLicense, ArcFMLicense);
            }
            //*** Why is there 2 separate OracleConnectionControl objects? 
            _EderSubOracleConnection = new OracleConnectionControl(ederOracleConnection, ederSubOracleConnection);
            getGeometricNetwork(_EderDatabaseConnection, EDERGeomNetwork, ref _EderGeomNetwork);
            getGeometricNetwork(_EderSubDatabaseConnection, SUBGeomNetwork, ref _EDERSubGeomNetwork);
            _EderOracleConnection = new OracleConnectionControl(ederOracleConnection, ederSubOracleConnection);

            try
            {
                ITable tempTransfomer = GetTable(_PhysNameTransformer, _EderDatabaseConnection);
                IFeatureClass fcTransfomer = (IFeatureClass)tempTransfomer;
                _ClassIDTransformer = fcTransfomer.FeatureClassID;

                ITable tempPrimaryMeter = GetTable(_PhysNamePrimaryMeter, _EderDatabaseConnection);
                IFeatureClass fcPrimaryMeter = (IFeatureClass)tempPrimaryMeter;
                _ClassIDPrimaryMeter = fcPrimaryMeter.FeatureClassID;

                ITable tempDCRectifier = GetTable(_PhysNameDCRectifier, _EderDatabaseConnection);
                IFeatureClass fcDCRectifier = (IFeatureClass)tempDCRectifier;
                _ClassIDDCRectifier = fcDCRectifier.FeatureClassID;

                ITable tempElectStitchPoint = GetTable(_FC_EDER_SourceName, _EderDatabaseConnection);
                IFeatureClass fcElectStitchPoint = (IFeatureClass)tempElectStitchPoint;
                _ClassIDEDStitchPoint = fcElectStitchPoint.FeatureClassID;


                if (tempTransfomer != null) { Marshal.FinalReleaseComObject(tempTransfomer); tempTransfomer = null; }
                if (tempPrimaryMeter != null) { Marshal.FinalReleaseComObject(tempPrimaryMeter); tempPrimaryMeter = null; }
                if (tempDCRectifier != null) { Marshal.FinalReleaseComObject(tempDCRectifier); tempDCRectifier = null; }
                if (tempElectStitchPoint != null) { Marshal.FinalReleaseComObject(tempElectStitchPoint); tempElectStitchPoint = null; }
                if (fcTransfomer != null) { Marshal.FinalReleaseComObject(fcTransfomer); fcTransfomer = null; }
                if (fcPrimaryMeter != null) { Marshal.FinalReleaseComObject(fcPrimaryMeter); fcPrimaryMeter = null; }
                if (fcDCRectifier != null) { Marshal.FinalReleaseComObject(fcDCRectifier); fcDCRectifier = null; }
                if (fcElectStitchPoint != null) { Marshal.FinalReleaseComObject(fcElectStitchPoint); fcElectStitchPoint = null; }
            }
            catch (Exception EX)
            {
            }

        }

        /// <summary>
        /// Initializes database connection to the EDER and EDPUB databases as well as checks out an ArcFM and Esri license
        /// </summary>
        private void Initialize(
            OracleConnection ederOracleConnection,
            OracleConnection ederSubOracleConnection,
            string EDERGeomNetwork,
            string SUBGeomNetwork,
            esriLicenseProductCode EsriLicense,
            mmLicensedProductCode ArcFMLicense,
            bool bGDBM,bool licenseCheckout)
        {
            _bGDBM = bGDBM;
            if (_bGDBM == false || licenseCheckout == true)
            {
                CheckOutLicense(EsriLicense, ArcFMLicense);
            }
            //*** Why is there 2 separate OracleConnectionControl objects? 
            _EderSubOracleConnection = new OracleConnectionControl(ederOracleConnection, ederSubOracleConnection);
            getGeometricNetwork(_EderDatabaseConnection, EDERGeomNetwork, ref _EderGeomNetwork);
            getGeometricNetwork(_EderSubDatabaseConnection, SUBGeomNetwork, ref _EDERSubGeomNetwork);
            _EderOracleConnection = new OracleConnectionControl(ederOracleConnection, ederSubOracleConnection);

            try
            {
                ITable tempTransfomer = GetTable(_PhysNameTransformer, _EderDatabaseConnection);
                IFeatureClass fcTransfomer = (IFeatureClass)tempTransfomer;
                _ClassIDTransformer = fcTransfomer.FeatureClassID;

                ITable tempPrimaryMeter = GetTable(_PhysNamePrimaryMeter, _EderDatabaseConnection);
                IFeatureClass fcPrimaryMeter = (IFeatureClass)tempPrimaryMeter;
                _ClassIDPrimaryMeter = fcPrimaryMeter.FeatureClassID;

                ITable tempDCRectifier = GetTable(_PhysNameDCRectifier, _EderDatabaseConnection);
                IFeatureClass fcDCRectifier = (IFeatureClass)tempDCRectifier;
                _ClassIDDCRectifier = fcDCRectifier.FeatureClassID;

                ITable tempElectStitchPoint = GetTable(_FC_EDER_SourceName, _EderDatabaseConnection);
                IFeatureClass fcElectStitchPoint = (IFeatureClass)tempElectStitchPoint;
                _ClassIDEDStitchPoint = fcElectStitchPoint.FeatureClassID;


                if (tempTransfomer != null) { Marshal.FinalReleaseComObject(tempTransfomer); tempTransfomer = null; }
                if (tempPrimaryMeter != null) { Marshal.FinalReleaseComObject(tempPrimaryMeter); tempPrimaryMeter = null; }
                if (tempDCRectifier != null) { Marshal.FinalReleaseComObject(tempDCRectifier); tempDCRectifier = null; }
                if (tempElectStitchPoint != null) { Marshal.FinalReleaseComObject(tempElectStitchPoint); tempElectStitchPoint = null; }
                if (fcTransfomer != null) { Marshal.FinalReleaseComObject(fcTransfomer); fcTransfomer = null; }
                if (fcPrimaryMeter != null) { Marshal.FinalReleaseComObject(fcPrimaryMeter); fcPrimaryMeter = null; }
                if (fcDCRectifier != null) { Marshal.FinalReleaseComObject(fcDCRectifier); fcDCRectifier = null; }
                if (fcElectStitchPoint != null) { Marshal.FinalReleaseComObject(fcElectStitchPoint); fcElectStitchPoint = null; }
            }
            catch (Exception EX)
            {
            }

        }

        /// <summary>
        /// Checks out the specified Esri and ArcFM Licenses
        /// </summary>
        /// <param name="EsriLicense">Esri license to check out</param>
        /// <param name="ArcFMLicense">ArcFM license to check out</param>
        private void CheckOutLicense(esriLicenseProductCode EsriLicense, mmLicensedProductCode ArcFMLicense)
        {
            bool licenseCheckoutSuccess = false;
            try
            {

                if (m_AOLicenseInitializer == null)
                {
                    m_AOLicenseInitializer = new LicenseInitializer();
                }
                //ESRI License Initializer generated code.
                if (m_AOLicenseInitializer.InitializedProduct == 0)
                {
                    licenseCheckoutSuccess = m_AOLicenseInitializer.InitializeApplication(
                        new esriLicenseProductCode[] { EsriLicense },
                        new esriLicenseExtensionCode[] { }) && m_AOLicenseInitializer.GetArcFMLicense(ArcFMLicense);
                }
                else
                {
                    licenseCheckoutSuccess = true;
                }
            }
            catch (Exception e)
            {
                throw new Exception("Unable to check out licenses: Message: " + e.Message + ": StackTrace: " +
                                    e.StackTrace);
            }

            if (!licenseCheckoutSuccess)
            {
                throw new Exception("Unable to check out licenses");
            }
        }




        /// <summary>
        /// Attempt to connect to the geodatabase
        /// </summary>
        /// <param name="DatabaseTNSName">Database TNS Name</param>
        /// <param name="DatabaseUserName">Database User Name</param>
        /// <param name="DatabasePassword">Database Password</param>
        /// <returns></returns>
        private void ConnectToDatabase(ref IWorkspace workspace, string DatabaseTNSName, string DatabaseUserName,
            string DatabasePassword)
        {
            SdeWorkspaceFactory sdeWorkspaceFactory = new SdeWorkspaceFactoryClass();
            IPropertySet propertySet = new PropertySetClass();
            propertySet.SetProperty("INSTANCE", "sde:oracle11g:" + DatabaseTNSName);
            propertySet.SetProperty("USER", DatabaseUserName);
            propertySet.SetProperty("PASSWORD", DatabasePassword);
            propertySet.SetProperty("AUTHENTICATION_MODE", "DBMS");
            propertySet.SetProperty("VERSION", "SDE.DEFAULT");
            workspace = sdeWorkspaceFactory.Open(propertySet, 0);
            if (workspace != null)
            {
                return;
            }

            //Failed to connect
            throw new Exception("Failed to connect to the specified geodatabase: " + DatabaseTNSName);
        }

        /// <summary>
        /// Determines if the specified circuit source record exists.
        /// </summary>
        /// <param name="CircuitID">CircuitID to find</param>
        /// <returns>Returns true if a circuit source record was found</returns>
        private CircuitSourceRecord FindCircuitByGUID(string CircuitSourceGlobalID)
        {
            BaseRecord circuitSourceRecord = new CircuitSourceRecord();
            IQueryFilter qf = new QueryFilterClass();
            ITable circuitSourceTable = null;

            try
            {
                circuitSourceTable = GetTable(_EderDatabaseConnection, "CIRCUITSOURCE");

                Dictionary<string, object> CircuitSourceValues = new Dictionary<string, object>();
                GetQueryFilter(ref qf, circuitSourceTable.Fields, "GLOBALID = '" + CircuitSourceGlobalID + "'");
                circuitSourceRecord = ObtainBaseRecord(circuitSourceTable, qf, typeof(CircuitSourceRecord));
            }
            finally
            {
                if (qf != null)
                {
                    while (Marshal.ReleaseComObject(qf) > 0)
                    {
                    }
                }
                if (circuitSourceTable != null)
                {
                    Marshal.ReleaseComObject(circuitSourceTable);
                }
            }

            return circuitSourceRecord as CircuitSourceRecord;
        }

        /// <summary>
        /// Determines if the specified circuit source record exists.
        /// </summary>
        /// <param name="CircuitID">CircuitID to find</param>
        /// <returns>Returns true if a circuit source record was found</returns>
        private CircuitSourceRecord FindCircuitByGUID_noLoadingorScada(string CircuitSourceGlobalID)
        {
            BaseRecord circuitSourceRecord = new CircuitSourceRecord();
            IQueryFilter qf = new QueryFilterClass();
            ITable circuitSourceTable = null;

            try
            {
                circuitSourceTable = GetTable(_EderDatabaseConnection, "CIRCUITSOURCE");

                Dictionary<string, object> CircuitSourceValues = new Dictionary<string, object>();
                GetQueryFilter(ref qf, circuitSourceTable.Fields, "GLOBALID = '" + CircuitSourceGlobalID + "'");
                circuitSourceRecord = ObtainBaseRecord(circuitSourceTable, qf, typeof(CircuitSourceRecord), false, false);
            }
            finally
            {
                if (qf != null)
                {
                    while (Marshal.ReleaseComObject(qf) > 0)
                    {
                    }
                }
                if (circuitSourceTable != null)
                {
                    Marshal.ReleaseComObject(circuitSourceTable);
                }
            }

            return circuitSourceRecord as CircuitSourceRecord;
        }

        /// <summary>
        /// Determines if the specified circuit source record exists.
        /// </summary>
        /// <param name="CircuitID">CircuitID to find</param>
        /// <returns>Returns true if a circuit source record was found</returns>
        private PCPRecord FindPCPByGUID(string PCPGlobalID)
        {
            BaseRecord PCPRecord = new PCPRecord();
            IQueryFilter qf = new QueryFilterClass();
            ITable PCPTable = null;

            try
            {
                PCPTable = GetTable("EDGIS.PARTIALCURTAILPOINT", _EderDatabaseConnection);

                GetQueryFilter(ref qf, PCPTable.Fields, "GLOBALID = '" + PCPGlobalID + "'");
                PCPRecord = ObtainBaseRecord(PCPTable, qf, typeof(PCPRecord));
            }
            finally
            {
                if (qf != null)
                {
                    while (Marshal.ReleaseComObject(qf) > 0)
                    {
                    }
                }
                if (PCPTable != null)
                {
                    Marshal.ReleaseComObject(PCPTable);
                }
            }

            return PCPRecord as PCPRecord;
        }

        private bool IsCircuitROBCValid(string circuitGlobalID, bool checkExistence)
        {
            if (FindCircuitByGUID_noLoadingorScada(circuitGlobalID) == null)
            {
                throw new Exception("Could not find the specified circuit source entry: " + circuitGlobalID);
            }
            if (checkExistence && !string.IsNullOrEmpty(FindCircuitROBC(circuitGlobalID).GlobalID.ToString()))
            {
                throw new Exception("An ROBC record already exists for the circuit source specified: " + circuitGlobalID);
            }

            return true;
        }

        private bool IsPCPROBCValid(string pcpGlobalID, bool checkExistence)
        {
            if (FindPCPByGUID(pcpGlobalID) == null)
            {
                throw new Exception("Could not find the specified partial curtailment point entry: " + pcpGlobalID);
            }
            if (checkExistence && !string.IsNullOrEmpty(FindPCPROBC(pcpGlobalID).GlobalID.ToString()))
            {
                throw new Exception("An ROBC record already exists for the partial curtailment point specified: " +
                                    pcpGlobalID);
            }

            return true;
        }

        /// <summary>
        /// Obtains an update statement to execute for the provided BaseRecord
        /// </summary>
        /// <param name="objectIDFieldName">Object ID field name</param>
        /// <param name="objectIDSequence">ObjectID Sequence number</param>
        /// <param name="tableName">Name of the table being updated</param>
        /// <param name="recordToInsert">BaseRecord with the values to insert</param>
        /// <returns>Returns the sql statement to execute</returns>
        private string GetInsertStatement(string objectIDFieldName, string objectIDSequence, string tableName,
            BaseRecord recordToInsert, ref string newGlobalID)
        {
            string newGuid = "{" + Guid.NewGuid().ToString().ToUpper() + "}";
            newGlobalID = newGuid;
            string sqlInsertStatement = "";
            sqlInsertStatement = "insert into " + tableName;
            string fieldNames = "";
            string fieldValues = "";
            int counter = 0;
            List<string> fieldNamesList = recordToInsert.GetFieldList();
            for (int i = 0; i < fieldNamesList.Count; i++)
            {
                string fieldName = fieldNamesList[i];
                if (fieldName.ToUpper() == "GLOBALID" || fieldName.ToUpper() == objectIDFieldName.ToUpper() ||
                    fieldName.ToUpper() == "CREATIONUSER"
                    || fieldName.ToUpper() == "DATECREATED" || fieldName.ToUpper() == "DATEMODIFIED" ||
                    fieldName.ToUpper() == "LASTUSER")
                {
                    continue;
                }

                if (i == (fieldNamesList.Count - 1))
                {
                    fieldNames += fieldName;
                    fieldValues += "'" + recordToInsert.GetFieldValue(fieldName) + "'";
                }
                else
                {
                    fieldNames += fieldName + ",";
                    fieldValues += "'" + recordToInsert.GetFieldValue(fieldName) + "',";
                }
                counter++;
            }

            if (fieldValues == string.Empty) { fieldNames += "GLOBALID"; } else { fieldNames += ",GLOBALID"; }
            if (fieldValues == string.Empty) { fieldValues += "'" + newGuid + "'"; } else { fieldValues += ",'" + newGuid + "'"; }
            recordToInsert.AddField("GLOBALID", newGuid);

            fieldNames += ",OBJECTID";
            fieldValues += ",EDGIS.r" + objectIDSequence + ".nextval";

            DateTime now = DateTime.Now;
            string dateString = now.Year + "/" + now.Month + "/" + now.Day + " " + now.Hour + ":" + now.Minute + ":" +
                                now.Second;

            fieldNames += ",CREATIONUSER,DATECREATED,DATEMODIFIED,LASTUSER";
            fieldValues += ",'" + _EDDatabaseUserName.ToUpper() + "',TO_DATE('" + dateString +
                           "', 'yyyy/mm/dd hh24:mi:ss')," + "TO_DATE('" + dateString + "', 'yyyy/mm/dd hh24:mi:ss'),'" +
                           _EDSubstationDatabaseUserName.ToUpper() + "'";

            sqlInsertStatement += " (" + fieldNames + ") VALUES(" + fieldValues + ")";
            return sqlInsertStatement;
        }

        /// <summary>
        /// Obtains an update statement to execute for the provided BaseRecord
        /// </summary>
        /// <param name="objectIDFieldName">Object ID field name</param>
        /// <param name="objectIDSequence">ObjectID Sequence number</param>
        /// <param name="tableName">Name of the table being updated</param>
        /// <param name="recordToUpdate">BaseRecord with the values to update</param>
        /// <returns>Returns the sql statement to execute</returns>
        private string GetUpdateStatement(string objectIDFieldName, string objectIDSequence, string tableName,
            BaseRecord recordToUpdate)
        {
            string sqlUpdateStatement = "";
            sqlUpdateStatement = "update " + tableName + " set ";
            List<string> fieldNamesList = recordToUpdate.GetFieldList();
            for (int i = 0; i < fieldNamesList.Count; i++)
            {
                string fieldName = fieldNamesList[i];

                if (fieldName.ToUpper() == "GLOBALID" || fieldName.ToUpper() == objectIDFieldName.ToUpper() ||
                    fieldName.ToUpper() == "CREATIONUSER"
                    || fieldName.ToUpper() == "DATECREATED" || fieldName.ToUpper() == "DATEMODIFIED" ||
                    fieldName.ToUpper() == "LASTUSER")
                {
                    continue;
                }

                sqlUpdateStatement += fieldName + "='" + recordToUpdate.GetFieldValue(fieldName) + "',";
            }
            if (fieldNamesList.Count == 0)
            {
                sqlUpdateStatement += ",";
            }

            DateTime now = DateTime.Now;
            string dateString = now.Year + "/" + now.Month + "/" + now.Day + " " + now.Hour + ":" + now.Minute + ":" +
                                now.Second;
            sqlUpdateStatement += "DATEMODIFIED = " + "TO_DATE('" + dateString + "', 'yyyy/mm/dd hh24:mi:ss')";
            sqlUpdateStatement += ",LASTUSER = '" + _EDSubstationDatabaseUserName.ToUpper() + "'";

            sqlUpdateStatement += " WHERE GLOBALID = '" + recordToUpdate.GlobalID + "'";
            return sqlUpdateStatement;
        }

        /// <summary>
        /// This method will populate create the BaseRecord object of the specified type
        /// </summary>
        /// <param name="tableToQuery">Table to search</param>
        /// <param name="qf">QueryFilter to search on</param>
        /// <param name="recordType">The record type to create.  Must inherit from the BaseRecord interface</param>
        /// <param name="includeScada">Set to True for Scada Information to be returned.</param>
        /// <param name="recordType">Set to True for the LoadingInformation to be returned</param>
        /// <returns>Returns a BaseRecord Objects</returns>
        private BaseRecord ObtainBaseRecord(ITable tableToQuery, IQueryFilter qf, Type recordType, bool includeScada, bool includeLoading)
        {
            BaseRecord newRecord = Activator.CreateInstance(recordType) as BaseRecord;
            ICursor rowCursor = null;
            IRow row = null;
            try
            {
                rowCursor = tableToQuery.Search(qf, false);
                while ((row = rowCursor.NextRow()) != null)
                {
                    for (int i = 0; i < row.Fields.FieldCount; i++)
                    {
                        string fieldName = row.Fields.get_Field(i).Name;
                        newRecord.AddField(fieldName, row.get_Value(i));
                    }
                    while (Marshal.ReleaseComObject(row) > 0)
                    {
                    }
                }
            }
            finally
            {
                if (row != null)
                {
                    while (Marshal.ReleaseComObject(row) > 0)
                    {
                    }
                }
                if (rowCursor != null)
                {
                    while (Marshal.ReleaseComObject(rowCursor) > 0)
                    {
                    }
                }
            }

            if (newRecord is CircuitSourceRecord)
            {
                if (newRecord.GlobalID == null || string.IsNullOrEmpty(newRecord.GlobalID.ToString()))
                {
                    return newRecord;
                }
                if (includeScada)
                {
                    ((CircuitSourceRecord)newRecord).IsScada = IsScada((CircuitSourceRecord)newRecord);

                }
                if (includeLoading)
                {
                    CircuitSourceRecord record = newRecord as CircuitSourceRecord;
                    GetLoadingInformation(ref record, false);
                }
            }

            return newRecord;
        }

        /// <summary>
        /// This method will populate create the BaseRecord object of the specified type
        /// </summary>
        /// <param name="tableToQuery">Table to search</param>
        /// <param name="qf">QueryFilter to search on</param>
        /// <param name="recordType">The record type to create.  Must inherit from the BaseRecord interface</param>
        /// <returns>Returns a BaseRecord Objects</returns>
        private BaseRecord ObtainBaseRecord(ITable tableToQuery, IQueryFilter qf, Type recordType)
        {
            bool includeScada = true;
            bool includeLoading = true;

            return ObtainBaseRecord(tableToQuery, qf, recordType, includeScada, includeLoading);
        }

        /// <summary>
        /// This method will populate create the BaseRecord object of the specified type
        /// </summary>
        /// <param name="tableToQuery">Table to search</param>
        /// <param name="qf">QueryFilter to search on</param>
        /// <param name="recordType">The record type to create.  Must inherit from the BaseRecord interface</param>
        /// <returns>Returns a BaseRecord Objects</returns>
        private List<object> ObtainBaseRecordList(ITable tableToQuery, IQueryFilter qf, Type recordType)
        {
            BaseRecord newRecord = Activator.CreateInstance(recordType) as BaseRecord;
            List<object> newList = new List<object>();
            ICursor rowCursor = null;
            IRow row = null;
            try
            {
                rowCursor = tableToQuery.Search(qf, false);
                while ((row = rowCursor.NextRow()) != null)
                {
                    for (int i = 0; i < row.Fields.FieldCount; i++)
                    {
                        string fieldName = row.Fields.get_Field(i).Name;
                        newRecord.AddField(fieldName, row.get_Value(i));
                    }
                    while (Marshal.ReleaseComObject(row) > 0)
                    {
                    }
                    newList.Add(newRecord);
                    newRecord = Activator.CreateInstance(recordType) as BaseRecord;
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (row != null)
                {
                    while (Marshal.ReleaseComObject(row) > 0)
                    {
                    }
                }
                if (rowCursor != null)
                {
                    while (Marshal.ReleaseComObject(rowCursor) > 0)
                    {
                    }
                }
            }

            return newList;
        }

        /// <summary>
        /// This method will populate create a list of the BaseRecord objects of the specified type
        /// </summary>
        /// <param name="tableToQuery">Table to search</param>
        /// <param name="qf">QueryFilter to search on</param>
        /// <param name="recordType">The record type to create.  Must inherit from the BaseRecord interface</param>
        /// <returns>Returns a list of BaseRecord Objects</returns>
        private List<BaseRecord> ObtainBaseRecords(ITable tableToQuery, IQueryFilter qf, Type recordType)
        {
            List<BaseRecord> newRecords = new List<BaseRecord>();
            ICursor rowCursor = null;
            IRow row = null;
            try
            {
                rowCursor = tableToQuery.Search(qf, false);
                while ((row = rowCursor.NextRow()) != null)
                {
                    BaseRecord newRecord = Activator.CreateInstance(recordType) as BaseRecord;
                    for (int i = 0; i < row.Fields.FieldCount; i++)
                    {
                        string fieldName = row.Fields.get_Field(i).Name;
                        newRecord.AddField(fieldName, row.get_Value(i));
                    }
                    while (Marshal.ReleaseComObject(row) > 0)
                    {
                    }
                    newRecords.Add(newRecord);
                }
            }
            finally
            {
                if (row != null)
                {
                    while (Marshal.ReleaseComObject(row) > 0)
                    {
                    }
                }
                if (rowCursor != null)
                {
                    while (Marshal.ReleaseComObject(rowCursor) > 0)
                    {
                    }
                }
            }

            foreach (BaseRecord newRecord in newRecords)
            {
                if (newRecord is CircuitSourceRecord)
                {
                    ((CircuitSourceRecord)newRecord).IsScada =
                        IsScada((CircuitSourceRecord)newRecord);
                    CircuitSourceRecord record = newRecord as CircuitSourceRecord;
                    GetLoadingInformation(ref record, false);
                }
            }

            return newRecords;
        }

        /// <summary>
        /// Obtains a table based on the workspace and table name supplied
        /// </summary>
        /// <param name="tableName">Name of the table (including Schema name)</param>
        /// <param name="workspace">IWorkspace to search</param>
        /// <returns></returns>
        private ITable GetTable(string tableName, IWorkspace workspace)
        {
            return ((IFeatureWorkspace)workspace).OpenTable(tableName);
        }

        /// <summary>
        /// Obtains a table bases on the workspace and class model supplied
        /// </summary>
        /// <param name="workspace">IWorkspace to search</param>
        /// <param name="modelName">Class Model name to find</param>
        /// <returns></returns>
        private ITable GetTable(IWorkspace workspace, string modelName)
        {
            IMMEnumTable circuitSourceTableEnum = ModelNameManager.Instance.TablesFromModelNameWS(workspace, modelName);
            circuitSourceTableEnum.Reset();
            ITable table = circuitSourceTableEnum.Next() as ITable;
            return table;
        }

        /// <summary>
        /// Obtains a field name based on the supplied objectclass and field model name
        /// </summary>
        /// <param name="objClass">IObjectClass to search</param>
        /// <param name="ModelName">Field Model name</param>
        /// <returns>Returns field name</returns>
        private string GetFieldName(IObjectClass objClass, string ModelName)
        {
            IField feederIDField = ModelNameManager.Instance.FieldFromModelName(objClass, ModelName);
            return feederIDField.Name;
        }

        /// <summary>
        /// Adds the Keys of the supplied list as fields to the QueryFilter and applies the where clause
        /// </summary>
        /// <param name="qf">IQueryFilter to set up</param>
        /// <param name="fieldValues"></param>
        /// <param name="whereClause"></param>
        private void GetQueryFilter(ref IQueryFilter qf, IFields tableFields, string whereClause)
        {
            for (int i = 0; i < tableFields.FieldCount; i++)
            {
                qf.AddField(tableFields.get_Field(i).Name);
            }
            qf.WhereClause = whereClause;
        }

        #endregion

        #region ARCFM TRACING HELPERS


        ///// <summary>
        ///// Test the relationship class between service points and Transformers or Primary Meters to get the related features and determine how many are essential customers.
        ///// </summary>
        ///// <param name="pRel">The relationship class</param>
        ///// <param name="iOID">The ObjectID of the feature being tested.</param>
        ///// <param name="pFClass">The feature class (Transformation or Primary Meter feature class</param>
        ///// <param name="pSPTable">The Service Point Table</param>
        ///// <returns>A count of essential customers.</returns>
        ///// 
        //private int getEssentialCustomerCount(IRelationshipClass pRel, int iOID, IFeatureClass pFClass, ITable pSPTable)
        //{
        //    try
        //    {
        //        int iReturnCount = 0;

        //        IRow pRow = null;
        //        int IfLDidx = pSPTable.Fields.FindField("ESSENTIALCUSTOMERIDC");

        //        IFeature pFeat = pFClass.GetFeature(iOID);

        //        ISet pRelSet = pRel.GetObjectsRelatedToObject(pFeat);

        //        pRelSet.Reset();

        //        IObject pRelObject = (IObject) pRelSet.Next();

        //        pRelSet.Reset();

        //        while (pRelObject != null) // CACHE THE RELATED OBJECTS
        //        {
        //            pRelObject = (ESRI.ArcGIS.Geodatabase.IObject) pRelSet.Next();

        //            if (pRelObject != null)
        //            {
        //                pRow = pSPTable.GetRow(pRelObject.OID);

        //                if (!DBNull.Value.Equals(pRow.get_Value(IfLDidx)))
        //                {
        //                    if (pRow.get_Value(IfLDidx).ToString() == "Y")
        //                    {
        //                        iReturnCount = iReturnCount + 1;
        //                    }
        //                }
        //            }
        //        }

        //        return iReturnCount;
        //    }
        //    catch (Exception EX)
        //    {

        //        throw EX;
        //    }
        //}

        private void FilterJunctions(List<int> iJunctionList, ref List<int> foundXFMRList, ref List<int> foundPriMeterList, ref List<int> foundDCRectifierList)
        {
            List<int> foundStitchPoints = new List<int>();
            FilterJunctions(iJunctionList, ref foundXFMRList, ref foundPriMeterList, ref foundDCRectifierList, ref foundStitchPoints);
        }

        private void FilterJunctions(List<int> iJunctionList, ref List<int> foundXFMRList, ref List<int> foundPriMeterList, ref List<int> foundDCRectifierList, ref List<int> foundStitchPoints)
        {

            int iMaxEIDsSQL = 995;
            int iLoop = -1;
            string whereClauseOIDList = "";
            List<string> qfWhereClauses = new List<string>();
            string sPrefix = " USERCLASSID in ( " + _ClassIDTransformer + "," + _ClassIDPrimaryMeter + "," + _ClassIDDCRectifier + "," + _ClassIDEDStitchPoint + " ) and EID in (";
            string sSuffix = " ) ";
            foreach (int iJ in iJunctionList)
            {
                iLoop++;
                if (iLoop == 0)
                {
                    whereClauseOIDList = iJ.ToString();
                }
                else if (0 < iLoop && iLoop < iMaxEIDsSQL)
                {
                    whereClauseOIDList = whereClauseOIDList + "," + iJ.ToString();
                }
                else if (iLoop == iMaxEIDsSQL)
                {
                    iLoop = -1;
                    whereClauseOIDList = whereClauseOIDList + "," + iJ.ToString();
                    qfWhereClauses.Add(sPrefix + whereClauseOIDList + sSuffix);
                    whereClauseOIDList = "";
                }
            }
            if (whereClauseOIDList != "")
            {
                qfWhereClauses.Add(sPrefix + whereClauseOIDList + sSuffix);
                whereClauseOIDList = "";
            }
            if (qfWhereClauses.Count > 0)
            {
                ITable tabNetTable = null;
                IQueryFilter iQf = null;
                ICursor spCursor = null;
                IRow rowToGet = null;
                try
                {
                    IFeatureWorkspace fwEDER = (IFeatureWorkspace)_EderDatabaseConnection;
                    tabNetTable = fwEDER.OpenTable(_TableNameEDNetwork);
                    int iOIDColumn = tabNetTable.FindField("USERID");
                    int iUserClassIDColumn = tabNetTable.FindField("USERCLASSID");
                    foreach (string searchClause in qfWhereClauses)
                    {
                        iQf = new QueryFilter();
                        iQf.SubFields = "OID,USERID,USERCLASSID";
                        iQf.WhereClause = searchClause;
                        spCursor = tabNetTable.Search(iQf, false);

                        while ((rowToGet = spCursor.NextRow()) != null)
                        {
                            object valField = rowToGet.get_Value(iOIDColumn);
                            int iTemp = Convert.ToInt32(valField.ToString());
                            object valField2 = rowToGet.get_Value(iUserClassIDColumn);
                            int iClassID = Convert.ToInt32(valField2.ToString());
                            if (iClassID == _ClassIDTransformer)
                            {
                                foundXFMRList.Add(iTemp);
                            }
                            else if (iClassID == _ClassIDPrimaryMeter)
                            {
                                foundPriMeterList.Add(iTemp);
                            }
                            else if (iClassID == _ClassIDDCRectifier)
                            {
                                foundDCRectifierList.Add(iTemp);
                            }
                            else if (iClassID == _ClassIDEDStitchPoint)
                            {
                                foundStitchPoints.Add(iTemp);
                            }
                            Marshal.FinalReleaseComObject(rowToGet);
                            rowToGet = null;
                        }
                        Marshal.FinalReleaseComObject(iQf);
                        iQf = null;
                        Marshal.FinalReleaseComObject(spCursor);
                        spCursor = null;
                    }
                }
                catch (Exception EX)
                {

                }
                finally
                {
                    if (tabNetTable != null)
                    {
                        Marshal.FinalReleaseComObject(tabNetTable);
                        tabNetTable = null;
                    }
                    if (iQf != null)
                    {
                        Marshal.FinalReleaseComObject(iQf);
                        iQf = null;
                    }
                    if (spCursor != null)
                    {
                        Marshal.FinalReleaseComObject(spCursor);
                        spCursor = null;
                    }
                }
            }
        }

        private List<int> FilterJunctions(List<int> iList)
        {
            List<int> foundEIDList = new List<int>();
            int iMaxEIDsSQL = 995;
            int iLoop = -1;
            string whereClauseOIDList = "";
            List<string> qfWhereClauses = new List<string>();
            string sPrefix = " USERCLASSID in (select objectid from " + _TableNameGDBITEMS + " where physicalname in ('" +
                             _PhysNameTransformer + "','" + _PhysNamePrimaryMeter + "','" + _PhysNameDCRectifier +
                             "') ) and EID in (";
            string sSuffix = " ) ";
            foreach (int iJ in iList)
            {
                iLoop++;
                if (iLoop == 0)
                {
                    whereClauseOIDList = iJ.ToString();
                }
                else if (0 < iLoop && iLoop < iMaxEIDsSQL)
                {
                    whereClauseOIDList = whereClauseOIDList + "," + iJ.ToString();
                }
                else if (iLoop == iMaxEIDsSQL)
                {
                    iLoop = -1;
                    whereClauseOIDList = whereClauseOIDList + "," + iJ.ToString();
                    qfWhereClauses.Add(sPrefix + whereClauseOIDList + sSuffix);
                    whereClauseOIDList = "";
                }
            }
            if (whereClauseOIDList != "")
            {
                qfWhereClauses.Add(sPrefix + whereClauseOIDList + sSuffix);
                whereClauseOIDList = "";
            }
            if (qfWhereClauses.Count > 0)
            {
                ITable tabNetTable = null;
                IQueryFilter iQf = null;
                ICursor spCursor = null;
                IRow rowToGet = null;
                try
                {
                    IFeatureWorkspace fwEDER = (IFeatureWorkspace)_EderDatabaseConnection;
                    tabNetTable = fwEDER.OpenTable(_TableNameEDNetwork);
                    int iEIDColumn = tabNetTable.FindField("EID");
                    int iUserClassIDColumn = tabNetTable.FindField("USERCLASSID");
                    foreach (string searchClause in qfWhereClauses)
                    {
                        iQf = new QueryFilter();
                        iQf.SubFields = "OID,EID,USERCLASSID";
                        iQf.WhereClause = searchClause;
                        spCursor = tabNetTable.Search(iQf, false);

                        while ((rowToGet = spCursor.NextRow()) != null)
                        {
                            object valField = rowToGet.get_Value(iEIDColumn);
                            int tempInt = Convert.ToInt32(valField.ToString());
                            foundEIDList.Add(tempInt);
                            Marshal.FinalReleaseComObject(rowToGet);
                            rowToGet = null;
                        }
                        Marshal.FinalReleaseComObject(iQf);
                        iQf = null;
                        Marshal.FinalReleaseComObject(spCursor);
                        spCursor = null;
                    }
                }
                catch (Exception EX)
                {

                }
                finally
                {
                    if (tabNetTable != null)
                    {
                        Marshal.FinalReleaseComObject(tabNetTable);
                        tabNetTable = null;
                    }
                    if (iQf != null)
                    {
                        Marshal.FinalReleaseComObject(iQf);
                        iQf = null;
                    }
                    if (spCursor != null)
                    {
                        Marshal.FinalReleaseComObject(spCursor);
                        spCursor = null;
                    }
                }
            }
            return foundEIDList;
        }

        //private int iCountEssentialCustomersFromJunctionsList(List<int> iJunctToCheck)
        //{
        //    int foundEssentialCustomer = 0;
        //    int iMaxEIDsSQL = 995;
        //    int iLoop = -1;
        //    string whereClauseOIDList = "";
        //    List<string> qfWhereClauses = new List<string>();
        //    string sTransformerPrefix = " " + _FieldName_tbSP_TRANSFORMERGUID + " is not null and " +
        //                                _FieldName_tbSP_TRANSFORMERGUID + " in (select GLOBALID from " +
        //                                _TableNameTransformer + " where objectid in (select USERID from " +
        //                                _TableNameEDNetwork + " where USERCLASSID in (select objectid from " +
        //                                _TableNameGDBITEMS + " where physicalname='" + _PhysNameTransformer +
        //                                "')  and EID in (";
        //    string sPrimaryMeterPrefix = " " + _FieldName_tbSP_PRIMARYMETERGUID + " is not null and " +
        //                                 _FieldName_tbSP_PRIMARYMETERGUID + " in (select GLOBALID from " +
        //                                 _TableNamePrimaryMeter + " where objectid in (select USERID from " +
        //                                 _TableNameEDNetwork + " where USERCLASSID in (select objectid from " +
        //                                 _TableNameGDBITEMS + " where physicalname='" + _PhysNamePrimaryMeter +
        //                                 "')  and EID in (";
        //    string sSuffix = " ))) and " + _FieldName_tbSP_ESSENTIALCUSTOMER + "='Y'";
        //    foreach (int iJ in iJunctToCheck)
        //    {
        //        iLoop++;
        //        if (iLoop == 0)
        //        {
        //            whereClauseOIDList = iJ.ToString();
        //        }
        //        else if (0 < iLoop && iLoop < iMaxEIDsSQL)
        //        {
        //            whereClauseOIDList = whereClauseOIDList + "," + iJ.ToString();
        //        }
        //        else if (iLoop == iMaxEIDsSQL)
        //        {
        //            iLoop = -1;
        //            whereClauseOIDList = whereClauseOIDList + "," + iJ.ToString();
        //            qfWhereClauses.Add(sTransformerPrefix + whereClauseOIDList + sSuffix);
        //            qfWhereClauses.Add(sPrimaryMeterPrefix + whereClauseOIDList + sSuffix);
        //            whereClauseOIDList = "";
        //        }
        //    }
        //    if (whereClauseOIDList != "")
        //    {
        //        qfWhereClauses.Add(sTransformerPrefix + whereClauseOIDList + sSuffix);
        //        qfWhereClauses.Add(sPrimaryMeterPrefix + whereClauseOIDList + sSuffix);
        //        whereClauseOIDList = "";
        //    }
        //    if (qfWhereClauses.Count > 0)
        //    {
        //        try
        //        {
        //            IFeatureWorkspace fwEDER = (IFeatureWorkspace) _EderDatabaseConnection;
        //            ITable tabServicePoint = fwEDER.OpenTable(_TableNameSERVICEPOINT);
        //            foreach (string searchClause in qfWhereClauses)
        //            {
        //                IQueryFilter iQf = new QueryFilter();
        //                iQf.SubFields = "OBJECTID";
        //                iQf.WhereClause = searchClause;
        //                ICursor spCursor = tabServicePoint.Search(iQf, false);
        //                IRow rowToGet = null;
        //                while ((rowToGet = spCursor.NextRow()) != null)
        //                {
        //                    Marshal.FinalReleaseComObject(rowToGet);
        //                    foundEssentialCustomer++;
        //                }
        //                Marshal.FinalReleaseComObject(iQf);
        //                Marshal.FinalReleaseComObject(spCursor);
        //            }
        //        }
        //        catch (Exception EX)
        //        {
        //            foundEssentialCustomer = -1;
        //        }
        //    }
        //    return foundEssentialCustomer;
        //}

        private int iCountAllCustomersFromOIDList(List<int> iTransformerList, List<int> iPriMeterList, List<int> iDCRectifierList)
        {
            int foundCustomer = 0;
            List<string> qfWhereClauses = new List<string>();
            qfWhereClauses = GetWhereClausesforOIDs(iTransformerList, iPriMeterList, iDCRectifierList, false);

            if (qfWhereClauses.Count > 0)
            {
                try
                {
                    IFeatureWorkspace fwEDER = (IFeatureWorkspace)_EderDatabaseConnection;
                    ITable tabServicePoint = fwEDER.OpenTable(_TableNameSERVICEPOINT);
                    foreach (string searchClause in qfWhereClauses)
                    {
                        IQueryFilter iQf = new QueryFilter();
                        iQf.SubFields = "OBJECTID";
                        iQf.WhereClause = searchClause;
                        foundCustomer += tabServicePoint.RowCount(iQf);
                        if (iQf != null) { Marshal.FinalReleaseComObject(iQf); iQf = null; }
                    }
                }
                catch (Exception EX)
                {
                    //foundCustomer = -1;
                }
            }
            return foundCustomer;
        }


        private int iCountAllCustomersFromJunctionsList(List<int> iJunctToCheck)
        {
            int foundCustomer = 0;
            int iMaxEIDsSQL = 995;
            int iLoop = -1;
            string whereClauseOIDList = "";
            List<string> qfWhereClauses = new List<string>();
            string sTransformerPrefix = " " + _FieldName_tbSP_TRANSFORMERGUID + " is not null and " +
                                        _FieldName_tbSP_TRANSFORMERGUID + " in (select GLOBALID from " +
                                        _TableNameTransformerMVView + " where objectid in (select USERID from " +
                                        _TableNameEDNetwork + " where USERCLASSID in (select objectid from " +
                                        _TableNameGDBITEMS + " where physicalname='" + _PhysNameTransformer +
                                        "')  and EID in (";
            string sPrimaryMeterPrefix = " " + _FieldName_tbSP_PRIMARYMETERGUID + " is not null and " +
                                         _FieldName_tbSP_PRIMARYMETERGUID + " in (select GLOBALID from " +
                                         _TableNamePrimaryMeterMVView + " where objectid in (select USERID from " +
                                         _TableNameEDNetwork + " where USERCLASSID in (select objectid from " +
                                         _TableNameGDBITEMS + " where physicalname='" + _PhysNamePrimaryMeter +
                                         "')  and EID in (";
            string sSuffix = " ))) ";
            foreach (int iJ in iJunctToCheck)
            {
                iLoop++;
                if (iLoop == 0)
                {
                    whereClauseOIDList = iJ.ToString();
                }
                else if (0 < iLoop && iLoop < iMaxEIDsSQL)
                {
                    whereClauseOIDList = whereClauseOIDList + "," + iJ.ToString();
                }
                else if (iLoop == iMaxEIDsSQL)
                {
                    iLoop = -1;
                    whereClauseOIDList = whereClauseOIDList + "," + iJ.ToString();
                    qfWhereClauses.Add(sTransformerPrefix + whereClauseOIDList + sSuffix);
                    qfWhereClauses.Add(sPrimaryMeterPrefix + whereClauseOIDList + sSuffix);
                    whereClauseOIDList = "";
                }
            }
            if (whereClauseOIDList != "")
            {
                qfWhereClauses.Add(sTransformerPrefix + whereClauseOIDList + sSuffix);
                qfWhereClauses.Add(sPrimaryMeterPrefix + whereClauseOIDList + sSuffix);
                whereClauseOIDList = "";
            }
            if (qfWhereClauses.Count > 0)
            {
                try
                {
                    IFeatureWorkspace fwEDER = (IFeatureWorkspace)_EderDatabaseConnection;
                    ITable tabServicePoint = fwEDER.OpenTable(_TableNameSERVICEPOINT);
                    foreach (string searchClause in qfWhereClauses)
                    {
                        IQueryFilter iQf = new QueryFilter();
                        iQf.SubFields = "OBJECTID";
                        iQf.WhereClause = searchClause;
                        ICursor spCursor = tabServicePoint.Search(iQf, true);
                        IRow rowToGet = null;
                        while ((rowToGet = spCursor.NextRow()) != null)
                        {
                            Marshal.FinalReleaseComObject(rowToGet);
                            foundCustomer++;
                        }
                        Marshal.FinalReleaseComObject(iQf);
                        Marshal.FinalReleaseComObject(spCursor);
                    }
                }
                catch (Exception EX)
                {
                    foundCustomer = -1;
                }
            }
            return foundCustomer;
        }

        private bool bHasEssentialCustomersFromJunctionsList(List<int> iJunctToCheck)
        {
            bool foundEssentialCustomer = false;
            int iMaxEIDsSQL = 995;
            int iLoop = -1;
            string whereClauseOIDList = "";
            List<string> qfWhereClauses = new List<string>();
            string sTransformerPrefix = " " + _FieldName_tbSP_TRANSFORMERGUID + " is not null and " +
                                        _FieldName_tbSP_TRANSFORMERGUID + " in (select GLOBALID from " +
                                        _TableNameTransformerMVView + " where objectid in (select USERID from " +
                                        _TableNameEDNetwork + " where USERCLASSID in (select objectid from " +
                                        _TableNameGDBITEMS + " where physicalname='" + _PhysNameTransformer +
                                        "')  and EID in (";
            string sPrimaryMeterPrefix = " " + _FieldName_tbSP_PRIMARYMETERGUID + " is not null and " +
                                         _FieldName_tbSP_PRIMARYMETERGUID + " in (select GLOBALID from " +
                                         _TableNamePrimaryMeterMVView + " where objectid in (select USERID from " +
                                         _TableNameEDNetwork + " where USERCLASSID in (select objectid from " +
                                         _TableNameGDBITEMS + " where physicalname='" + _PhysNamePrimaryMeter +
                                         "')  and EID in (";
            string sSuffix = " ))) and " + _FieldName_tbSP_ESSENTIALCUSTOMER + "='Y'";
            foreach (int iJ in iJunctToCheck)
            {
                iLoop++;
                if (iLoop == 0)
                {
                    whereClauseOIDList = iJ.ToString();
                }
                else if (0 < iLoop && iLoop < iMaxEIDsSQL)
                {
                    whereClauseOIDList = whereClauseOIDList + "," + iJ.ToString();
                }
                else if (iLoop == iMaxEIDsSQL)
                {
                    iLoop = -1;
                    whereClauseOIDList = whereClauseOIDList + "," + iJ.ToString();
                    qfWhereClauses.Add(sTransformerPrefix + whereClauseOIDList + sSuffix);
                    qfWhereClauses.Add(sPrimaryMeterPrefix + whereClauseOIDList + sSuffix);
                    whereClauseOIDList = "";
                }
            }
            if (whereClauseOIDList != "")
            {
                qfWhereClauses.Add(sTransformerPrefix + whereClauseOIDList + sSuffix);
                qfWhereClauses.Add(sPrimaryMeterPrefix + whereClauseOIDList + sSuffix);
                whereClauseOIDList = "";
            }
            if (qfWhereClauses.Count > 0)
            {
                ITable tabServicePoint = null;
                IQueryFilter iQf = null;
                ICursor spCursor = null;
                IRow rowToGet = null;
                try
                {
                    IFeatureWorkspace fwEDER = (IFeatureWorkspace)_EderDatabaseConnection;
                    tabServicePoint = fwEDER.OpenTable(_TableNameSERVICEPOINT);
                    foreach (string searchClause in qfWhereClauses)
                    {
                        iQf = new QueryFilter();
                        iQf.SubFields = "OBJECTID";
                        iQf.WhereClause = searchClause;
                        spCursor = tabServicePoint.Search(iQf, false);

                        while ((rowToGet = spCursor.NextRow()) != null)
                        {
                            Marshal.FinalReleaseComObject(rowToGet);
                            rowToGet = null;
                            foundEssentialCustomer = true;
                            return foundEssentialCustomer;
                        }
                        Marshal.FinalReleaseComObject(iQf);
                        iQf = null;
                        Marshal.FinalReleaseComObject(spCursor);
                        spCursor = null;
                    }
                }
                catch (Exception EX)
                {
                    foundEssentialCustomer = true;
                }
                finally
                {
                    if (tabServicePoint != null)
                    {
                        Marshal.FinalReleaseComObject(tabServicePoint);
                        tabServicePoint = null;
                    }
                    if (iQf != null)
                    {
                        Marshal.FinalReleaseComObject(iQf);
                        iQf = null;
                    }
                    if (spCursor != null)
                    {
                        Marshal.FinalReleaseComObject(spCursor);
                        spCursor = null;
                    }
                    if (tabServicePoint != null)
                    {
                        Marshal.FinalReleaseComObject(tabServicePoint);
                        tabServicePoint = null;
                    }
                }
            }
            return foundEssentialCustomer;
        }

        private List<string> GetWhereClausesforOIDs(List<int> iTransformerList, List<int> iPriMeterList, List<int> iDCRectifierList, bool bEssentialCustomerOnly)
        {
            int iMaxEIDsSQL = 955;
            int iLoop = -1;
            string whereClauseXFMROIDList = "";
            string whereClausePriMeterList = "";
            string whereClauseDCRectList = "";
            List<string> qfWhereClauses = new List<string>();
            string sTransformerPrefix = //" " + _FieldName_tbSP_TRANSFORMERGUID + " is not null and " +
                                        _FieldName_tbSP_TRANSFORMERGUID + " in (select GLOBALID from " +
                                        _TableNameTransformerMVView + " where objectid in (";

            string sPrimaryMeterPrefix = //" " + _FieldName_tbSP_PRIMARYMETERGUID + " is not null and " +
                                         _FieldName_tbSP_PRIMARYMETERGUID + " in (select GLOBALID from " +
                                         _TableNamePrimaryMeterMVView + " where objectid in (";

            string sDCRectifierPrefix = //" " + _FieldName_tbSP_DCRECTIFIERGUID + " is not null and " +
                                         _FieldName_tbSP_DCRECTIFIERGUID + " in (select GLOBALID from " +
                                         _TableNameDCRectifierMVView + " where objectid in (";

            string sSuffix = ")) ";
            if (bEssentialCustomerOnly == true)
            {
                sSuffix = sSuffix + " and " + _FieldName_tbSP_ESSENTIALCUSTOMER + "='Y'";
            }
            // Where Clause Creation for Transformers
            foreach (int iJ in iTransformerList)
            {
                iLoop++;
                if (iLoop == 0)
                {
                    whereClauseXFMROIDList = iJ.ToString();
                }
                else if (0 < iLoop && iLoop < iMaxEIDsSQL)
                {
                    whereClauseXFMROIDList = whereClauseXFMROIDList + "," + iJ.ToString();
                }
                else if (iLoop == iMaxEIDsSQL)
                {
                    iLoop = -1;
                    whereClauseXFMROIDList = whereClauseXFMROIDList + "," + iJ.ToString();
                    qfWhereClauses.Add(sTransformerPrefix + whereClauseXFMROIDList + sSuffix);
                    //qfWhereClauses.Add(sPrimaryMeterPrefix + whereClauseOIDList + sSuffix);
                    whereClauseXFMROIDList = "";
                }
            }
            if (whereClauseXFMROIDList != "")
            {
                qfWhereClauses.Add(sTransformerPrefix + whereClauseXFMROIDList + sSuffix);
                //qfWhereClauses.Add(sPrimaryMeterPrefix + whereClauseOIDList + sSuffix);
                whereClauseXFMROIDList = "";
            }
            // Where Clause Creation for Primary Meters
            iLoop = -1;
            foreach (int iJ in iPriMeterList)
            {
                iLoop++;
                if (iLoop == 0)
                {
                    whereClausePriMeterList = iJ.ToString();
                }
                else if (0 < iLoop && iLoop < iMaxEIDsSQL)
                {
                    whereClausePriMeterList = whereClausePriMeterList + "," + iJ.ToString();
                }
                else if (iLoop == iMaxEIDsSQL)
                {
                    iLoop = -1;
                    whereClausePriMeterList = whereClausePriMeterList + "," + iJ.ToString();
                    //qfWhereClauses.Add(sTransformerPrefix + whereClausePriMeterList + sSuffix);
                    qfWhereClauses.Add(sPrimaryMeterPrefix + whereClausePriMeterList + sSuffix);
                    whereClausePriMeterList = "";
                }
            }
            if (whereClausePriMeterList != "")
            {
                //qfWhereClauses.Add(sTransformerPrefix + whereClauseXFMROIDList + sSuffix);
                qfWhereClauses.Add(sPrimaryMeterPrefix + whereClausePriMeterList + sSuffix);
                whereClausePriMeterList = "";
            }
            // Where Clause Creation for DC Rectifiers
            iLoop = -1;
            foreach (int iJ in iDCRectifierList)
            {
                iLoop++;
                if (iLoop == 0)
                {
                    whereClauseDCRectList = iJ.ToString();
                }
                else if (0 < iLoop && iLoop < iMaxEIDsSQL)
                {
                    whereClauseDCRectList = whereClauseDCRectList + "," + iJ.ToString();
                }
                else if (iLoop == iMaxEIDsSQL)
                {
                    iLoop = -1;
                    whereClauseDCRectList = whereClauseDCRectList + "," + iJ.ToString();
                    //qfWhereClauses.Add(sTransformerPrefix + whereClausePriMeterList + sSuffix);
                    //qfWhereClauses.Add(sPrimaryMeterPrefix + whereClausePriMeterList + sSuffix);
                    qfWhereClauses.Add(sDCRectifierPrefix + whereClauseDCRectList + sSuffix);
                    whereClauseDCRectList = "";
                }
            }
            if (whereClauseDCRectList != "")
            {
                //qfWhereClauses.Add(sTransformerPrefix + whereClauseXFMROIDList + sSuffix);
                //qfWhereClauses.Add(sPrimaryMeterPrefix + whereClausePriMeterList + sSuffix);
                qfWhereClauses.Add(sDCRectifierPrefix + whereClauseDCRectList + sSuffix);
                whereClauseDCRectList = "";
            }
            return qfWhereClauses;
        }

        private bool bHasEssentialCustomersFromOIDList(List<int> iTransformerList, List<int> iPriMeterList, List<int> iDCRectifierList)
        {
            bool foundEssentialCustomer = false;
            //int iMaxEIDsSQL = 995;
            //int iLoop = -1;

            List<string> qfWhereClauses = new List<string>();
            qfWhereClauses = GetWhereClausesforOIDs(iTransformerList, iPriMeterList, iDCRectifierList, true);
            if (qfWhereClauses.Count > 0)
            {
                ITable tabServicePoint = null;
                IQueryFilter iQf = null;
                ICursor spCursor = null;
                IRow rowToGet = null;
                try
                {
                    IFeatureWorkspace fwEDER = (IFeatureWorkspace)_EderDatabaseConnection;
                    tabServicePoint = fwEDER.OpenTable(_TableNameSERVICEPOINT);
                    foreach (string searchClause in qfWhereClauses)
                    {
                        iQf = new QueryFilter();
                        iQf.SubFields = "OBJECTID";
                        iQf.WhereClause = searchClause;
                        spCursor = tabServicePoint.Search(iQf, false);

                        while ((rowToGet = spCursor.NextRow()) != null)
                        {
                            Marshal.FinalReleaseComObject(rowToGet);
                            rowToGet = null;
                            foundEssentialCustomer = true;
                            return foundEssentialCustomer;
                        }
                        Marshal.FinalReleaseComObject(iQf);
                        iQf = null;
                        Marshal.FinalReleaseComObject(spCursor);
                        spCursor = null;
                    }
                }
                catch (Exception EX)
                {
                    foundEssentialCustomer = true;
                }
                finally
                {
                    if (tabServicePoint != null)
                    {
                        Marshal.FinalReleaseComObject(tabServicePoint);
                        tabServicePoint = null;
                    }
                    if (iQf != null)
                    {
                        Marshal.FinalReleaseComObject(iQf);
                        iQf = null;
                    }
                    if (spCursor != null)
                    {
                        Marshal.FinalReleaseComObject(spCursor);
                        spCursor = null;
                    }
                    if (tabServicePoint != null)
                    {
                        Marshal.FinalReleaseComObject(tabServicePoint);
                        tabServicePoint = null;
                    }
                }
            }
            return foundEssentialCustomer;
        }

        /// <summary>
        /// Return a specific relationship between two provided object classes
        /// </summary>
        /// <param name="pWSpace">The current GIS database workspace.</param>
        /// <param name="sObjName1">The name of the first object class which participates in the relationsip.</param>
        /// <param name="sObjName2">The name of the second object class which participates in the relationsip.</param>
        /// <param name="pRelRole">The relationship type.</param>
        /// <param name="pRelClass">The returned relationship between the two provided object classes.</param>
        /// <returns>A boolean indicating the relationship was determoined.</returns>
        /// 
        private bool bGetRelationshipClass(IWorkspace pWSpace, string sObjName1, string sObjName2, esriRelRole pRelRole,
            ref IRelationshipClass pRelClass)
        {
            IFeatureWorkspace pFWSpace = (IFeatureWorkspace)pWSpace;
            IObjectClass pObjClass = (IObjectClass)pFWSpace.OpenFeatureClass(sObjName1);
            IEnumRelationshipClass eRelationships = pObjClass.get_RelationshipClasses(pRelRole);

            eRelationships.Reset();
            pRelClass = eRelationships.Next();

            while (pRelClass != null) // PROCESS EACH RELATIONSHIP CLASS
            {
                if ((pRelClass.OriginClass.AliasName.ToUpper() == pObjClass.AliasName.ToUpper()) &&
                    (pRelClass.DestinationClass.AliasName.ToUpper() == sObjName2.ToUpper()))
                {
                    return true; // RETURN TRUE IF THE APPORPRIATE RELATIONSHIP IS FOUND
                }
                if ((pRelClass.DestinationClass.AliasName.ToUpper() == pObjClass.AliasName.ToUpper()) &&
                    (pRelClass.OriginClass.AliasName.ToUpper() == sObjName2.ToUpper()))
                {
                    return true; // RETURN TRUE IF THE APPORPRIATE RELATIONSHIP IS FOUND
                }
                pRelClass = eRelationships.Next();
            }

            return false;
        }


        /// <summary>
        /// Get the feature class id for the provided feature class name.
        /// </summary>
        /// <param name="pWSpace">The current GIS database workspace.</param>
        /// <param name="sFeatClassName">The name of the feature class to process.</param>
        /// <param name="pFClass">The returned feature class opened by using the workspace and feature class name.</param>
        /// <returns>The ointeger feature class id.</returns>
        /// 

        private int getFeatureClassID(IWorkspace pWSpace, string sFeatClassName, ref IFeatureClass pFClass)
        {
            IFeatureWorkspace pFWSpace = (IFeatureWorkspace)pWSpace;
            pFClass = pFWSpace.OpenFeatureClass(sFeatClassName);
            IObjectClass pOClass = (IObjectClass)pFClass;

            int iClassID = pOClass.ObjectClassID;

            return iClassID;
        }


        /// <summary>
        /// Get a specifid geometric network from a speciied GIS database workspace.
        /// </summary>
        /// <param name="pWSpace">The current GIS database workspace.</param>
        /// <param name="sGeomNetwork">The name of the geomatric network to locate.</param>
        /// <param name="pGeometricNetwork">The returned geometric network.</param>
        /// 
        private void getGeometricNetwork(IWorkspace pWSpace, string sGeomNetwork,
            ref IGeometricNetwork pGeometricNetwork)
        {
            IFeatureWorkspace fNetw = (IFeatureWorkspace)pWSpace;
            bool bFoundGeomNetwork = false;
            string[] sGN = sGeomNetwork.Split('\\');
            string sDatasetName = null;
            string sGeomNet = null;
            if (sGN != null)
            {
                if (sGN.Count() == 2)
                {
                    sDatasetName = sGN[0];
                    sGeomNet = sGN[1];
                }
            }
            IFeatureDataset fDataset = null;
            INetworkCollection colNetw = null;
            if (sDatasetName != null && sGeomNet != null)
            {
                fDataset = fNetw.OpenFeatureDataset(sDatasetName);
                if (fDataset != null)
                {
                    colNetw = fDataset as INetworkCollection;
                    if (colNetw != null)
                    {
                        pGeometricNetwork = colNetw.get_GeometricNetworkByName(sGeomNet);
                        if (pGeometricNetwork != null)
                        {
                            bFoundGeomNetwork = true;
                        }
                    }
                }
            }
            if (!bFoundGeomNetwork)
            {
                throw new Exception("Could not find the geometric networks in the specified database");
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="pGeometricNetwork">The geometric network to use in the trace.</param>
        /// <param name="CircuitID">The Circuit ID to trace.</param>
        /// <param name="eJunctions">The return collection of Junction EIDs</param>
        /// <param name="eEdges">The returned collection of Edge EIDS.</param>
        ///
        private void ArcFMDwnstreamTrace(IGeometricNetwork pGeometricNetwork, string CircuitID,
            ref IEnumNetEID eReturnJuncs, ref IEnumNetEID eReturnEdges)
        {
            string message = string.Empty;
            IMMFeederExt feederExt = null;
            IMMFeederSpace feederSpace = null;
            IMMFeederSource feederSource = null;
            IMMElectricNetworkTracing downstreamTracer = null;
            IMMNetworkAnalysisExtForFramework networkAnalysisExtForFramework = null;
            IMMElectricTraceSettingsEx electricTraceSettings = null;

            Type type = Type.GetTypeFromProgID("mmFramework.MMFeederExt");
            object obj = Activator.CreateInstance(type);

            try
            {
                //Console.WriteLine("Initializing feeder space");
                //Common.WriteToLog("Initializing feeder space", LoggingLevel.Info);

                feederExt = obj as IMMFeederExt;
                feederSpace = feederExt.get_FeederSpace(pGeometricNetwork, false);
                if (feederSpace == null)
                {
                    throw new Exception("Unable to get feeder space information from geometric network");
                }

                //Console.WriteLine("Finsihed initializing feeder space");
                //Common.WriteToLog("Finished initializing feeder space", LoggingLevel.Info);

                INetwork network = pGeometricNetwork.Network;
                INetSchema networkSchema = network as INetSchema;
                INetWeight electricTraceWeight = networkSchema.get_WeightByName("MMELECTRICTRACEWEIGHT");
                downstreamTracer = new MMFeederTracerClass();

                electricTraceSettings = new MMElectricTraceSettingsClass();
                electricTraceSettings.RespectESRIBarriers = true;
                electricTraceSettings.UseFeederManagerCircuitSources = true;
                electricTraceSettings.UseFeederManagerProtectiveDevices = true;

                networkAnalysisExtForFramework = new MMNetworkAnalysisExtForFrameworkClass();
                networkAnalysisExtForFramework.DrawComplex = true;
                networkAnalysisExtForFramework.SelectionSemantics = mmESRIAnalysisOptions.mmESRIAnalysisOnAllFeatures;

                //Phases to trace.
                mmPhasesToTrace phasesToTrace = mmPhasesToTrace.mmPTT_Any;

                IMMEnumFeederSource feederSources = feederSpace.FeederSources;
                feederSources.Reset();
                double totalFeeders = feederSources.Count;

                try
                {
                    // 
                    feederSource = feederSources.get_FeederSourceByFeederID(CircuitID);
                }
                catch (Exception e)
                {

                }

                if (feederSource != null)
                {
                    DateTime startTime = DateTime.Now;
                    //Common.WriteToLog("Processing Circuit ID: " + feederSource.FeederID, LoggingLevel.Info);
                    //Console.WriteLine("Processing Circuit ID: " + feederSource.FeederID);

                    //start EID
                    int startEID = feederSource.JunctionEID;
                    string feederID = feederSource.FeederID.ToString();

                    IEnumNetEID junctions = null;
                    IEnumNetEID edges = null;

                    //Common.WriteToLog("Tracing circuit: " + feederSource.FeederID, LoggingLevel.Info);

                    //Trace
                    downstreamTracer.TraceDownstream(pGeometricNetwork, networkAnalysisExtForFramework,
                        electricTraceSettings, startEID,
                        esriElementType.esriETJunction, phasesToTrace, out junctions, out edges);

                    eReturnEdges = edges;
                    eReturnJuncs = junctions;

                }
            }
            catch (Exception ex)
            {
                //Common.WriteToLog("Error executing tracing. Message: " + ex.Message + " StackTrace: " + ex.StackTrace, LoggingLevel.Info);
                throw ex;
            }
            finally
            {
                if (feederExt != null)
                {
                    while (Marshal.ReleaseComObject(feederExt) > 0) ;
                }
                if (feederSpace != null)
                {
                    while (Marshal.ReleaseComObject(feederSpace) > 0) ;
                }
                if (feederSource != null)
                {
                    while (Marshal.ReleaseComObject(feederSource) > 0) ;
                }
                if (downstreamTracer != null)
                {
                    while (Marshal.ReleaseComObject(downstreamTracer) > 0) ;
                }
                if (networkAnalysisExtForFramework != null)
                {
                    while (Marshal.ReleaseComObject(networkAnalysisExtForFramework) > 0) ;
                }
                if (electricTraceSettings != null)
                {
                    while (Marshal.ReleaseComObject(electricTraceSettings) > 0) ;
                }
            }
        }

        /// <summary>
        /// Used to get the list of all circuits that supply energy to the circuit in question. Used to build list of upstream circuits.
        /// </summary>
        /// <param name="sCircuitID"></param>
        /// <param name="nTypeOfCircuit"></param>
        /// <param name="inputDictionary"></param>
        /// <param name="ttDirectionOfTrace"></param>
        /// <returns></returns>
        private Dictionary<string, NetworkSourceType> GetParentChildCircuitsForCircuitID(string sCircuitID,
            NetworkSourceType nTypeOfCircuit, ref Dictionary<string, NetworkSourceType> inputDictionary,
            TraceType ttDirectionOfTrace)
        {
            GetParentChildCircuitsForCircuitID(sCircuitID, nTypeOfCircuit, ref inputDictionary, ttDirectionOfTrace, true);
            return inputDictionary;
        }

        /// <summary>
        /// Used to get the list of all circuits that supply energy to the circuit in question. Used to build list of upstream circuits.
        /// </summary>
        /// <param name="sCircuitID"></param>
        /// <param name="nTypeOfCircuit"></param>
        /// <param name="inputDictionary"></param>
        /// <param name="ttDirectionOfTrace"></param>
        /// <param name="bRecursive">Specifies if we are to only ge tthe immediate parent (used to get immediate substations)</param>
        /// <returns></returns>
        private Dictionary<string, NetworkSourceType> GetParentChildCircuitsForCircuitID(string sCircuitID,
            NetworkSourceType nTypeOfCircuit, ref Dictionary<string, NetworkSourceType> inputDictionary,
            TraceType ttDirectionOfTrace, bool bRecursive)
        {
            //Dictionary<string,NetworkSourceType> dParentCircuits = new Dictionary<string,NetworkSourceType>();
            IWorkspace initialWorkspace = null;
            IWorkspace targetWorkspace = null;
            string initialNetworkSource = "";
            string targetNetworkSource = "";
            string initialTRACE_DirWhereClause = "";
            string targetTRACE_DirWhereClause = "";

            IFeatureWorkspace fW = null;
            IFeatureClass fcInitialSource = null;
            IQueryFilter iQf = null;
            IFeatureCursor fcSources = null;
            IFeature fSource = null;
            IGeometryBag bag = null;
            IGeometryCollection gBagCollect = null;
            IGeoDataset geoDataset = null;
            ITopologicalOperator topologicalOperator = null;
            ISpatialIndex spi = null;
            IFeatureWorkspace fWTarget = null;
            IFeatureClass fcTargetSource = null;
            ISpatialFilter iSPF = null;
            IFeature fTarget = null;
            IFeatureCursor fcTargetSources = null;

            NetworkSourceType nstOutput = NetworkSourceType.EDPUB;
            if (nTypeOfCircuit == NetworkSourceType.EDER)
            {
                initialWorkspace = _EderDatabaseConnection;
                initialNetworkSource = _FC_EDER_SourceName;
                targetWorkspace = _EderSubDatabaseConnection;
                targetNetworkSource = _FC_Substation_SourceName;
                nstOutput = NetworkSourceType.SUBSTATION;

            }
            else if (nTypeOfCircuit == NetworkSourceType.SUBSTATION)
            {
                targetWorkspace = _EderDatabaseConnection;
                targetNetworkSource = _FC_EDER_SourceName;
                initialWorkspace = _EderSubDatabaseConnection;
                initialNetworkSource = _FC_Substation_SourceName;
                nstOutput = NetworkSourceType.EDER;
            }
            switch (ttDirectionOfTrace)
            {
                case TraceType.CHILD:
                    initialTRACE_DirWhereClause = " SUBTYPECD=1 ";
                    targetTRACE_DirWhereClause = " SUBTYPECD=2 ";
                    break;
                case TraceType.PARENT:
                    initialTRACE_DirWhereClause = " SUBTYPECD=2 ";
                    targetTRACE_DirWhereClause = " SUBTYPECD=1 ";
                    break;
            }
            try
            {
                fW = (IFeatureWorkspace)initialWorkspace;
                fcInitialSource = fW.OpenFeatureClass(initialNetworkSource);
                fW = null;
                iQf = new QueryFilter();
                iQf.WhereClause = " CIRCUITID='" + sCircuitID + "' and " + initialTRACE_DirWhereClause;
                fcSources = fcInitialSource.Search(iQf, false);
                fSource = null;
                bag = new GeometryBagClass();
                gBagCollect = (IGeometryCollection)bag;
                geoDataset = (IGeoDataset)fcInitialSource;
                bag.SpatialReference = geoDataset.SpatialReference;

                while ((fSource = fcSources.NextFeature()) != null)
                {
                    topologicalOperator = fSource.ShapeCopy as ITopologicalOperator;
                    gBagCollect.AddGeometry(topologicalOperator.Buffer(5));
                    if (topologicalOperator != null) { while (Marshal.ReleaseComObject(topologicalOperator) > 0) ; topologicalOperator = null; }
                    if (fSource != null) { while (Marshal.ReleaseComObject(fSource) > 0) ; fSource = null; }
                }
                if (gBagCollect.GeometryCount > 0)
                {
                    spi = (ISpatialIndex)bag;
                    spi.AllowIndexing = true;
                    spi.Invalidate();
                    fWTarget = (IFeatureWorkspace)targetWorkspace;
                    fcTargetSource = fWTarget.OpenFeatureClass(targetNetworkSource);
                    int iFieldCircuitId = fcTargetSource.FindField("CIRCUITID");
                    int iFieldSUBTYPEId = fcTargetSource.FindField("SUBTYPECD");
                    iSPF = new SpatialFilterClass();
                    iSPF.Geometry = bag;
                    iSPF.GeometryField = fcTargetSource.ShapeFieldName;
                    iSPF.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                    iSPF.SubFields = ("SHAPE,CIRCUITID,SUBTYPECD");
                    iSPF.WhereClause = targetTRACE_DirWhereClause;
                    fcTargetSources = fcTargetSource.Search(iSPF, false);
                    while ((fTarget = fcTargetSources.NextFeature()) != null)
                    {
                        //int newSUBTYPECD = (int) fTarget.get_Value(iFieldSUBTYPEId);
                        string newCircuitID = fTarget.get_Value(iFieldCircuitId).ToString();
                        if (!(inputDictionary.Keys.Contains(newCircuitID)))
                        {

                            inputDictionary.Add(newCircuitID, nstOutput);
                            if (bRecursive)
                            {
                                GetParentChildCircuitsForCircuitID(newCircuitID, nstOutput, ref inputDictionary,
                                    ttDirectionOfTrace);
                            }
                        }
                        if (fTarget != null) { while (Marshal.ReleaseComObject(fTarget) > 0) ; fTarget = null; }
                    }
                    if (iSPF != null) { while (Marshal.ReleaseComObject(iSPF) > 0) ;iSPF = null; }
                    if (fcTargetSources != null) { Marshal.FinalReleaseComObject(fcTargetSources); fcTargetSources = null; }
                    if (fTarget != null) { while (Marshal.ReleaseComObject(fTarget) > 0) ; fTarget = null; }
                    if (fcTargetSource != null) { while (Marshal.ReleaseComObject(fcTargetSource) > 0) ; fcTargetSource = null; }
                    if (iQf != null) { while (Marshal.ReleaseComObject(iQf) > 0) ; iQf = null; }
                    if (spi != null) { while (Marshal.ReleaseComObject(spi) > 0) ; spi = null; }
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (bag != null) { while (Marshal.ReleaseComObject(bag) > 0) ;bag = null; }
                if (fSource != null) { while (Marshal.ReleaseComObject(fSource) > 0) ; fSource = null; }
                if (fcSources != null) { while (Marshal.ReleaseComObject(fcSources) > 0) ; fcSources = null; }
                if (iQf != null) { while (Marshal.ReleaseComObject(iQf) > 0) ; iQf = null; }
                if (gBagCollect != null) { while (Marshal.ReleaseComObject(gBagCollect) > 0) ; gBagCollect = null; }
                if (geoDataset != null) { while (Marshal.ReleaseComObject(geoDataset) > 0) ; geoDataset = null; }
                if (fcTargetSources != null) { Marshal.FinalReleaseComObject(fcTargetSources); fcTargetSources = null; }
                if (fcTargetSource != null) { while (Marshal.ReleaseComObject(fcTargetSource) > 0) ; fcTargetSource = null; }
                if (iSPF != null) { while (Marshal.ReleaseComObject(iSPF) > 0) ;iSPF = null; }
                if (fcTargetSources != null) { while (Marshal.ReleaseComObject(fcTargetSources) > 0) ; fcTargetSources = null; }
                initialWorkspace = null;
                targetWorkspace = null;
                fW = null;
                fWTarget = null;
                if (fcInitialSource != null) { while (Marshal.ReleaseComObject(fcInitialSource) > 0) ; fcInitialSource = null; }
                if (spi != null) { while (Marshal.ReleaseComObject(spi) > 0) ; spi = null; }
                if (fTarget != null) { while (Marshal.ReleaseComObject(fTarget) > 0) ; fTarget = null; }
                if (topologicalOperator != null) { while (Marshal.ReleaseComObject(topologicalOperator) > 0) ; topologicalOperator = null; }
            }
            return inputDictionary;
        }

        private Dictionary<string, NetworkSourceType> GetChildCircuitsForLoadStitchPoint(int iObjectID,
            NetworkSourceType nTypeOfCircuit, ref Dictionary<string, NetworkSourceType> inputDictionary)
        {
            TraceType ttDirectionOfTrace = TraceType.CHILD;
            IWorkspace initialWorkspace = null;
            IWorkspace targetWorkspace = null;
            IFeatureWorkspace fW = null;
            IFeatureWorkspace fWTarget = null;
            IFeatureClass fcInitialSource = null;
            IFeatureClass fcTargetSource = null;
            ISpatialFilter iSPF = null;
            IGeoDataset geoDataset = null;
            IGeometryBag bag = null;
            IGeometryCollection gBagCollect = null;
            IFeature fSource = null;
            IFeature fTarget = null;
            IFeatureCursor fcSources = null;
            IFeatureCursor fcTargetSources = null;
            ITopologicalOperator topologicalOperator = null;
            IQueryFilter iQf = null;
            ISpatialIndex spi = null;
            string initialNetworkSource = "";
            string targetNetworkSource = "";
            string initialTRACE_DirWhereClause = "";
            string targetTRACE_DirWhereClause = "";
            NetworkSourceType nstOutput = NetworkSourceType.EDPUB;
            if (nTypeOfCircuit == NetworkSourceType.EDER)
            {
                initialWorkspace = _EderDatabaseConnection;
                initialNetworkSource = _FC_EDER_SourceName;
                targetWorkspace = _EderSubDatabaseConnection;
                targetNetworkSource = _FC_Substation_SourceName;
                nstOutput = NetworkSourceType.SUBSTATION;

            }
            else if (nTypeOfCircuit == NetworkSourceType.SUBSTATION)
            {
                targetWorkspace = _EderDatabaseConnection;
                targetNetworkSource = _FC_EDER_SourceName;
                initialWorkspace = _EderSubDatabaseConnection;
                initialNetworkSource = _FC_Substation_SourceName;
                nstOutput = NetworkSourceType.EDER;
            }
            switch (ttDirectionOfTrace)
            {
                case TraceType.PARENT:
                    initialTRACE_DirWhereClause = " SUBTYPECD=1 ";
                    targetTRACE_DirWhereClause = " SUBTYPECD=2 ";
                    break;
                case TraceType.CHILD:
                    initialTRACE_DirWhereClause = " SUBTYPECD=2 ";
                    targetTRACE_DirWhereClause = " SUBTYPECD=1 ";
                    break;
            }
            try
            {
                fW = (IFeatureWorkspace)initialWorkspace;
                fcInitialSource = fW.OpenFeatureClass(initialNetworkSource);
                iQf = new QueryFilter();
                iQf.WhereClause = " OBJECTID='" + iObjectID + "' and " + initialTRACE_DirWhereClause;
                fcSources = fcInitialSource.Search(iQf, false);

                bag = new GeometryBagClass();
                gBagCollect = (IGeometryCollection)bag;
                geoDataset = (IGeoDataset)fcInitialSource;
                bag.SpatialReference = geoDataset.SpatialReference;

                while ((fSource = fcSources.NextFeature()) != null)
                {
                    topologicalOperator = fSource.ShapeCopy as ITopologicalOperator;
                    gBagCollect.AddGeometry(topologicalOperator.Buffer(5));
                    Marshal.FinalReleaseComObject(fSource);
                    Marshal.FinalReleaseComObject(topologicalOperator);
                }
                if (gBagCollect.GeometryCount > 0)
                {
                    spi = (ISpatialIndex)bag;
                    spi.AllowIndexing = true;
                    spi.Invalidate();
                    fWTarget = (IFeatureWorkspace)targetWorkspace;
                    fcTargetSource = fWTarget.OpenFeatureClass(targetNetworkSource);
                    fWTarget = null;
                    int iFieldCircuitId = fcTargetSource.FindField("CIRCUITID");
                    int iFieldSUBTYPEId = fcTargetSource.FindField("SUBTYPECD");
                    iSPF = new SpatialFilterClass();
                    iSPF.Geometry = bag;
                    iSPF.GeometryField = fcTargetSource.ShapeFieldName;
                    iSPF.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                    iSPF.SubFields = ("SHAPE,CIRCUITID,SUBTYPECD");
                    iSPF.WhereClause = targetTRACE_DirWhereClause;
                    fcTargetSources = fcTargetSource.Search(iSPF, false);
                    while ((fTarget = fcTargetSources.NextFeature()) != null)
                    {
                        int newSUBTYPECD = (int)fTarget.get_Value(iFieldSUBTYPEId);
                        string newCircuitID = fTarget.get_Value(iFieldCircuitId).ToString();
                        if (!(inputDictionary.Keys.Contains(newCircuitID)))
                        {

                            inputDictionary.Add(newCircuitID, nstOutput);
                            GetParentChildCircuitsForCircuitID(newCircuitID, nstOutput, ref inputDictionary,
                                ttDirectionOfTrace);
                        }
                        Marshal.FinalReleaseComObject(fTarget);
                    }
                    if (iSPF != null) { Marshal.FinalReleaseComObject(iSPF); iSPF = null; }
                    if (fcTargetSources != null) { Marshal.FinalReleaseComObject(fcTargetSources); fcTargetSources = null; }
                    if (fcTargetSource != null) { Marshal.FinalReleaseComObject(fcTargetSource); fcTargetSource = null; }
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (bag != null) { while (Marshal.ReleaseComObject(bag) > 0) ;bag = null; }
                if (fSource != null) { while (Marshal.ReleaseComObject(fSource) > 0) ; fSource = null; }
                if (fcSources != null) { while (Marshal.ReleaseComObject(fcSources) > 0) ; fcSources = null; }
                if (iQf != null) { while (Marshal.ReleaseComObject(iQf) > 0) ; iQf = null; }
                if (gBagCollect != null) { while (Marshal.ReleaseComObject(gBagCollect) > 0) ; gBagCollect = null; }
                if (geoDataset != null) { while (Marshal.ReleaseComObject(geoDataset) > 0) ; geoDataset = null; }
                if (fcTargetSources != null) { Marshal.FinalReleaseComObject(fcTargetSources); fcTargetSources = null; }
                if (fcTargetSource != null) { while (Marshal.ReleaseComObject(fcTargetSource) > 0) ; fcTargetSource = null; }
                if (iSPF != null) { while (Marshal.ReleaseComObject(iSPF) > 0) ;iSPF = null; }
                if (iQf != null) { while (Marshal.ReleaseComObject(iQf) > 0) ; iQf = null; }
                if (fcTargetSources != null) { while (Marshal.ReleaseComObject(fcTargetSources) > 0) ; fcTargetSources = null; }
                initialWorkspace = null;
                targetWorkspace = null;
                fW = null;
                fWTarget = null;
                if (fcInitialSource != null) { while (Marshal.ReleaseComObject(fcInitialSource) > 0) ; fcInitialSource = null; }
                if (spi != null) { while (Marshal.ReleaseComObject(spi) > 0) ; spi = null; }
                if (fTarget != null) { while (Marshal.ReleaseComObject(fTarget) > 0) ; fTarget = null; }
                if (topologicalOperator != null) { while (Marshal.ReleaseComObject(topologicalOperator) > 0) ; topologicalOperator = null; }
            }

            return inputDictionary;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pGeometricNetwork">The geometric network to use in the trace.</param>
        /// <param name="sTableName">Table Name of Feature</param>
        /// <param name="sGlobalid">GlobalID of Feature</param>
        /// <param name="CircuitID">The Circuit ID to trace.</param>
        /// <param name="eJunctions">The return collection of Junction EIDs</param>
        /// <param name="eEdges">The returned collection of Edge EIDS.</param>
        ///
        private void ArcFMDwnstreamTraceFromFeature(IGeometricNetwork pGeometricNetwork,
            string sFeatureClass, string sGlobalID,
            ref IEnumNetEID eReturnJuncs, ref IEnumNetEID eReturnEdges)
        {
            string message = string.Empty;
            IMMFeederExt feederExt = null;
            IMMFeederSpace feederSpace = null;
            IMMFeederSource feederSource = null;
            IMMElectricNetworkTracing downstreamTracer = null;
            IMMNetworkAnalysisExtForFramework networkAnalysisExtForFramework = null;
            IMMElectricTraceSettingsEx electricTraceSettings = null;

            Type type = Type.GetTypeFromProgID("mmFramework.MMFeederExt");
            object obj = Activator.CreateInstance(type);

            try
            {
                //Console.WriteLine("Initializing feeder space");
                //Common.WriteToLog("Initializing feeder space", LoggingLevel.Info);

                feederExt = obj as IMMFeederExt;
                feederSpace = feederExt.get_FeederSpace(pGeometricNetwork, false);
                if (feederSpace == null)
                {
                    throw new Exception("Unable to get feeder space information from geometric network");
                }

                //FeederWorkspaceClass feederWorkspace = (FeederWorkspaceClass)feederSpace;
                IWorkspace pWorkspace = pGeometricNetwork.FeatureDataset.Workspace;
                IFeatureWorkspace pFeatureWorkspace = (IFeatureWorkspace)pWorkspace;
                //Console.WriteLine("Finsihed initializing feeder space");
                //Common.WriteToLog("Finished initializing feeder space", LoggingLevel.Info);

                INetwork network = pGeometricNetwork.Network;
                INetSchema networkSchema = network as INetSchema;
                INetWeight electricTraceWeight = networkSchema.get_WeightByName("MMELECTRICTRACEWEIGHT");
                downstreamTracer = new MMFeederTracerClass();

                electricTraceSettings = new MMElectricTraceSettingsClass();
                electricTraceSettings.RespectESRIBarriers = true;
                electricTraceSettings.UseFeederManagerCircuitSources = true;
                electricTraceSettings.UseFeederManagerProtectiveDevices = true;

                networkAnalysisExtForFramework = new MMNetworkAnalysisExtForFrameworkClass();
                networkAnalysisExtForFramework.DrawComplex = true;
                networkAnalysisExtForFramework.SelectionSemantics = mmESRIAnalysisOptions.mmESRIAnalysisOnAllFeatures;

                IFeatureClass fcToTrace = pFeatureWorkspace.OpenFeatureClass(sFeatureClass);
                //Phases to trace.
                mmPhasesToTrace phasesToTrace = mmPhasesToTrace.mmPTT_Any;

                //IMMEnumFeederSource feederSources = feederSpace.FeederSources;
                //feederSources.Reset();
                //double totalFeeders = feederSources.Count;

                //try
                //{
                //    // 
                //    feederSource = feederSources.get_FeederSourceByFeederID(CircuitID);
                //}
                //catch (Exception e)
                //{

                //}
                IFeature featureToGet = null;
                try
                {
                    int guidField = fcToTrace.FindField("CIRCUITID");
                    IQueryFilter iQf = new QueryFilter();
                    iQf.WhereClause = " GLOBALID='" + sGlobalID + "' ";
                    IFeatureCursor featureCursor = fcToTrace.Search(iQf, false);
                    featureToGet = featureCursor.NextFeature();
                }
                catch (Exception EX)
                {
                }

                if (featureToGet != null)
                {
                    DateTime startTime = DateTime.Now;
                    //Common.WriteToLog("Processing Circuit ID: " + feederSource.FeederID, LoggingLevel.Info);
                    //Console.WriteLine("Processing Circuit ID: " + feederSource.FeederID);

                    //start EID
                    int startEID = ((ISimpleJunctionFeature)featureToGet).EID;


                    IEnumNetEID junctions = null;
                    IEnumNetEID edges = null;

                    //Common.WriteToLog("Tracing circuit: " + feederSource.FeederID, LoggingLevel.Info);

                    //Trace
                    downstreamTracer.TraceDownstream(pGeometricNetwork, networkAnalysisExtForFramework,
                        electricTraceSettings, startEID,
                        esriElementType.esriETJunction, phasesToTrace, out junctions, out edges);

                    eReturnEdges = edges;
                    eReturnJuncs = junctions;

                }
            }
            catch (Exception ex)
            {
                //Common.WriteToLog("Error executing tracing. Message: " + ex.Message + " StackTrace: " + ex.StackTrace, LoggingLevel.Info);
                throw ex;
            }
            finally
            {
                if (feederExt != null)
                {
                    while (Marshal.ReleaseComObject(feederExt) > 0) ;
                }
                if (feederSpace != null)
                {
                    while (Marshal.ReleaseComObject(feederSpace) > 0) ;
                }
                if (feederSource != null)
                {
                    while (Marshal.ReleaseComObject(feederSource) > 0) ;
                }
                if (downstreamTracer != null)
                {
                    while (Marshal.ReleaseComObject(downstreamTracer) > 0) ;
                }
                if (networkAnalysisExtForFramework != null)
                {
                    while (Marshal.ReleaseComObject(networkAnalysisExtForFramework) > 0) ;
                }
                if (electricTraceSettings != null)
                {
                    while (Marshal.ReleaseComObject(electricTraceSettings) > 0) ;
                }
            }
        }




        #endregion

        #region Public Methods


        public CircuitSourceRecord FindCircuit(string CircuitID)
        {
            BaseRecord circuitSourceRecord = new CircuitSourceRecord();
            IQueryFilter qf = new QueryFilterClass();
            ITable circuitSourceTable = null;

            try
            {
                circuitSourceTable = GetTable(_EderDatabaseConnection, "CIRCUITSOURCE");
                string feederIDFieldName = GetFieldName(((IObjectClass)circuitSourceTable), "FEEDERID");
                GetQueryFilter(ref qf, circuitSourceTable.Fields, feederIDFieldName + " = '" + CircuitID + "'");
                circuitSourceRecord = ObtainBaseRecord(circuitSourceTable, qf, typeof(CircuitSourceRecord));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (qf != null)
                {
                    while (Marshal.ReleaseComObject(qf) > 0)
                    {
                    }
                }
                if (circuitSourceTable != null)
                {
                    Marshal.ReleaseComObject(circuitSourceTable);
                }
            }

            return circuitSourceRecord as CircuitSourceRecord;
        }

        /// <summary>
        /// Finds the Circuit Source Record based on the provided CircuitID.
        /// </summary>
        /// <param name="CircuitID">CircuitID to find</param>
        /// <returns>Returns a CircuitSourceRecord</returns>
        public CircuitSourceRecord FindCircuit(string CircuitID, bool scada, bool loadInformation)
        {
            BaseRecord circuitSourceRecord = new CircuitSourceRecord();
            IQueryFilter qf = new QueryFilterClass();
            ITable circuitSourceTable = null;

            try
            {
                circuitSourceTable = GetTable(_EderDatabaseConnection, "CIRCUITSOURCE");
                string feederIDFieldName = GetFieldName(((IObjectClass)circuitSourceTable), "FEEDERID");
                GetQueryFilter(ref qf, circuitSourceTable.Fields, feederIDFieldName + " = '" + CircuitID + "'");
                circuitSourceRecord = ObtainBaseRecord(circuitSourceTable, qf, typeof(CircuitSourceRecord), scada, loadInformation);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (qf != null)
                {
                    while (Marshal.ReleaseComObject(qf) > 0)
                    {
                    }
                }
                if (circuitSourceTable != null)
                {
                    Marshal.ReleaseComObject(circuitSourceTable);
                }
            }

            return circuitSourceRecord as CircuitSourceRecord;
        }

        /// <summary>
        /// Finds the Circuit Source Record based on the provided CircuitID.
        /// </summary>
        /// <param name="CircuitID">CircuitID to find</param>
        /// <returns>Returns a CircuitSourceRecord</returns>
        public CircuitSourceRecord FindCircuit_NoLoadingNoScada(string CircuitID)
        {
            BaseRecord circuitSourceRecord = new CircuitSourceRecord();
            IQueryFilter qf = new QueryFilterClass();
            ITable circuitSourceTable = null;

            try
            {
                circuitSourceTable = GetTable(_EderDatabaseConnection, "CIRCUITSOURCE");
                string feederIDFieldName = GetFieldName(((IObjectClass)circuitSourceTable), "FEEDERID");
                GetQueryFilter(ref qf, circuitSourceTable.Fields, feederIDFieldName + " = '" + CircuitID + "'");
                circuitSourceRecord = ObtainBaseRecord(circuitSourceTable, qf, typeof(CircuitSourceRecord), false, false);
            }
            finally
            {
                if (qf != null)
                {
                    while (Marshal.ReleaseComObject(qf) > 0)
                    {
                    }
                }
                if (circuitSourceTable != null)
                {
                    Marshal.ReleaseComObject(circuitSourceTable);
                }
            }

            return circuitSourceRecord as CircuitSourceRecord;
        }

        /// <summary>
        /// Determines if the specified circuit source record exists.
        /// </summary>
        /// <param name="CircuitID">CircuitID to find</param>
        /// <returns>List of circuit source records found</returns>
        public List<CircuitSourceRecord> FindCircuit(string SubstationName, string CircuitName)
        {
            List<CircuitSourceRecord> circuitSourceRecords = new List<CircuitSourceRecord>();
            IQueryFilter qf = new QueryFilterClass();
            ITable circuitSourceTable = null;

            try
            {
                circuitSourceTable = GetTable(_EderDatabaseConnection, "CIRCUITSOURCE");
                //UPPER for making the search case insensitive
                GetQueryFilter(ref qf, circuitSourceTable.Fields,
                    "UPPER(SUBSTATIONNAME) = '" + SubstationName.ToUpper() + "' AND UPPER(CIRCUITNAME) = '" +
                    CircuitName.ToUpper() + "'");
                List<BaseRecord> baseRecords = ObtainBaseRecords(circuitSourceTable, qf, typeof(CircuitSourceRecord));
                foreach (BaseRecord record in baseRecords)
                {
                    circuitSourceRecords.Add(record as CircuitSourceRecord);
                }
            }
            finally
            {
                if (qf != null)
                {
                    while (Marshal.ReleaseComObject(qf) > 0)
                    {
                    }
                }
                if (circuitSourceTable != null)
                {
                    Marshal.ReleaseComObject(circuitSourceTable);
                }
            }

            return circuitSourceRecords;
        }

        /// <summary>
        /// Searches for a related ROBC record based on the provided Circuit Source GlobalID
        /// </summary>
        /// <param name="circuitSourceRecord">Circuit Source Record to find the related ROBC record for</param>
        /// <returns>ROBC Record found</returns>
        public List<ROBCRecord> FindCircuitROBC(CircuitSourceRecord circuitSourceRecord)
        {
            List<BaseRecord> robcRecords;
            IQueryFilter qf = new QueryFilterClass();
            ITable robcTable = null;

            try
            {
                robcTable = GetTable("EDGIS.ROBC", _EderDatabaseConnection);
                GetQueryFilter(ref qf, robcTable.Fields, "CIRCUITSOURCEGUID = '" + circuitSourceRecord.GlobalID + "'");
                robcRecords = ObtainBaseRecords(robcTable, qf, typeof(ROBCRecord));
            }
            finally
            {
                if (qf != null)
                {
                    Marshal.ReleaseComObject(qf);
                }
                if (robcTable != null)
                {
                    Marshal.ReleaseComObject(robcTable);
                }
            }

            return robcRecords.Cast<ROBCRecord>().ToList();
        }

        /// <summary>
        /// Searches for a related ROBC record based on the provided Circuit Source GlobalID
        /// </summary>
        /// <param name="circuitSourceRecord">Circuit Source Record to find the related ROBC record for</param>
        /// <returns>ROBC Record found</returns>
        public ROBCRecord FindPCPROBC(PCPRecord pcpRecord, bool scada, bool loadingInformation)
        {
            BaseRecord robcRecord;
            IQueryFilter qf = new QueryFilterClass();
            ITable robcTable = null;

            try
            {
                robcTable = GetTable("EDGIS.ROBC", _EderDatabaseConnection);
                GetQueryFilter(ref qf, robcTable.Fields, "PARTCURTAILPOINTGUID = '" + pcpRecord.GlobalID + "'");
                robcRecord = ObtainBaseRecord(robcTable, qf, typeof(ROBCRecord), scada, loadingInformation);
            }
            finally
            {
                if (qf != null)
                {
                    Marshal.ReleaseComObject(qf);
                }
                if (robcTable != null)
                {
                    Marshal.ReleaseComObject(robcTable);
                }
            }

            return robcRecord as ROBCRecord;
        }

        /// <summary>
        /// Searches for a related ROBC record based on the provided Circuit Source GlobalID
        /// </summary>
        /// <param name="circuitSourceRecord">Circuit Source Record to find the related ROBC record for</param>
        /// <returns>ROBC Record found</returns>
        public ROBCRecord FindCircuitROBC(string circuitSourceGUID)
        {
            BaseRecord robcRecord;
            IQueryFilter qf = new QueryFilterClass();
            ITable robcTable = null;

            try
            {
                robcTable = GetTable("EDGIS.ROBC", _EderDatabaseConnection);
                GetQueryFilter(ref qf, robcTable.Fields, "CIRCUITSOURCEGUID = '" + circuitSourceGUID + "'");
                robcRecord = ObtainBaseRecord(robcTable, qf, typeof(ROBCRecord));
            }
            finally
            {
                if (qf != null)
                {
                    Marshal.ReleaseComObject(qf);
                }
                if (robcTable != null)
                {
                    Marshal.ReleaseComObject(robcTable);
                }
            }

            return robcRecord as ROBCRecord;
        }

        /// <summary>
        /// Searches for a related ROBC record based on the provided PCP GlobalID
        /// </summary>
        /// <param name="pcpGUID">PCPGuid to find the related ROBC record for</param>
        /// <returns>ROBC Record found</returns>
        public ROBCRecord FindPCPROBC(string pcpGUID)
        {
            BaseRecord robcRecord;
            IQueryFilter qf = new QueryFilterClass();
            ITable robcTable = null;

            try
            {
                robcTable = GetTable("EDGIS.ROBC", _EderDatabaseConnection);
                GetQueryFilter(ref qf, robcTable.Fields, "PARTCURTAILPOINTGUID = '" + pcpGUID + "'");
                robcRecord = ObtainBaseRecord(robcTable, qf, typeof(ROBCRecord));
            }
            finally
            {
                if (qf != null)
                {
                    Marshal.ReleaseComObject(qf);
                }
                if (robcTable != null)
                {
                    Marshal.ReleaseComObject(robcTable);
                }
            }

            return robcRecord as ROBCRecord;
        }

        /// <summary>
        /// Searches for related ROBC records based on the provided Circuit Source Record
        /// </summary>
        /// <param name="circuitSourceRecords">List of circuit source records to find the associated ROBC records for</param>
        /// <returns>List of ROBC Records</returns>
        public List<ROBCRecord> FindCircuitROBCs(List<CircuitSourceRecord> circuitSourceRecords)
        {
            List<ROBCRecord> robcRecords = new List<ROBCRecord>();

            IQueryFilter qf = new QueryFilterClass();
            ITable robcTable = null;

            try
            {
                foreach (CircuitSourceRecord circuitSourceRecord in circuitSourceRecords)
                {
                    robcTable = GetTable("EDGIS.ROBC", _EderDatabaseConnection);
                    GetQueryFilter(ref qf, robcTable.Fields,
                        "CIRCUITSOURCEGUID = '" + circuitSourceRecord.GlobalID + "'");
                    BaseRecord robcRecord = ObtainBaseRecord(robcTable, qf, typeof(ROBCRecord));
                    robcRecords.Add(robcRecord as ROBCRecord);
                }
            }
            finally
            {
                if (qf != null)
                {
                    Marshal.ReleaseComObject(qf);
                }
                if (robcTable != null)
                {
                    Marshal.ReleaseComObject(robcTable);
                }
            }

            return robcRecords;
        }

        /// <summary>
        /// Searches for an ROBC Record based on the ROBC global ID
        /// </summary>
        /// <param name="ROBCGlobalID">Global ID of the ROBC Record</param>
        /// <returns>ROBC record found</returns>
        public ROBCRecord FindROBC(string ROBCGlobalID)
        {
            BaseRecord robcRecord;
            IQueryFilter qf = new QueryFilterClass();
            ITable robcTable = null;

            try
            {
                robcTable = GetTable("EDGIS.ROBC", _EderDatabaseConnection);
                GetQueryFilter(ref qf, robcTable.Fields, "GLOBALID = '" + ROBCGlobalID + "'");
                robcRecord = ObtainBaseRecord(robcTable, qf, typeof(ROBCRecord));
            }
            finally
            {
                if (qf != null)
                {
                    Marshal.ReleaseComObject(qf);
                }
                if (robcTable != null)
                {
                    Marshal.ReleaseComObject(robcTable);
                }
            }

            return robcRecord as ROBCRecord;
        }

        /// <summary>
        /// Creates a new ROBC record with the passed in parameters. The related CIRCUITSOURCEGUID field must be set correctly
        /// </summary>
        /// <param name="ROBCRecord">ROBC record to set</param>
        /// <returns>Returns new ROBC record object</returns>
        public ROBCRecord CreateROBC(ROBCRecord ROBCRecord)
        {
            BaseRecord updatedRobcRecord = new ROBCRecord();
            ITable ROBCTable = null;
            string sqlInsertStatement = "";
            try
            {
                ROBCTable = GetTable("EDGIS.ROBC", _EderDatabaseConnection);

                object circuitGlobalID = ROBCRecord.GetFieldValue("CIRCUITSOURCEGUID");
                object pcpGlobalID = ROBCRecord.GetFieldValue("PARTCURTAILPOINTGUID");

                //bool isROBCValid = false;
                //if (circuitGlobalID != null && !string.IsNullOrEmpty(circuitGlobalID.ToString()) &&
                //    IsCircuitROBCValid(circuitGlobalID.ToString(), false))
                //{
                //    isROBCValid = true;
                //}
                //else if (circuitGlobalID != null && !string.IsNullOrEmpty(pcpGlobalID.ToString()) &&
                //         IsPCPROBCValid(pcpGlobalID.ToString(), false))
                //{
                //    isROBCValid = true;
                //}
                //else
                //{
                //    throw new Exception(
                //        "Must specify a valid Partial curtailment point or circuit source globalid for the ROBC Record");
                //}

                //if (isROBCValid)
                //{
                string newGuid = "";
                sqlInsertStatement = GetInsertStatement(ROBCTable.OIDFieldName, "77", "EDGIS.ROBC", ROBCRecord,
                    ref newGuid);
                _EderDatabaseConnection.ExecuteSQL(sqlInsertStatement);

                if (circuitGlobalID != null && !string.IsNullOrEmpty(circuitGlobalID.ToString()))
                {
                    updatedRobcRecord = FindCircuitROBC(circuitGlobalID.ToString());
                }
                else if (circuitGlobalID != null && !string.IsNullOrEmpty(pcpGlobalID.ToString()))
                {
                    updatedRobcRecord = FindCircuitROBC(pcpGlobalID.ToString());
                }
                //}
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to create record: Sql: " + sqlInsertStatement + ": Exception: " + ex.Message);
            }
            finally
            {
                if (ROBCTable != null)
                {
                    Marshal.ReleaseComObject(ROBCTable);
                }
            }
            return updatedRobcRecord as ROBCRecord;
        }

        /// <summary>
        /// Updates an existing ROBC record with the passed in parameters. The related CIRCUITSOURCEGUID and GLOBALID fields must be specified in the dictionary.
        /// If the record does not exist currently, it will create a new record
        /// </summary>
        /// <param name="robcRecord">ROBC record to set</param>
        /// <returns>Returns updated ROBC record</returns>
        public ROBCRecord UpdateROBC(ROBCRecord robcRecord)
        {
            BaseRecord updatedROBCRecord = new ROBCRecord();
            ITable ROBCTable = null;
            string sqlInsertStatement = "";
            try
            {
                ROBCTable = GetTable("EDGIS.ROBC", _EderDatabaseConnection);

                object circuitGlobalID = robcRecord.GetFieldValue("CIRCUITSOURCEGUID");
                object pcpGlobalID = robcRecord.GetFieldValue("PARTCURTAILPOINTGUID");
                string ROBCGlobalID = robcRecord.GlobalID.ToString();

                ROBCRecord robc = FindROBC(ROBCGlobalID);
                if (robc == null || string.IsNullOrEmpty(robc.GlobalID.ToString()))
                {
                    updatedROBCRecord = CreateROBC(robcRecord);
                }
                else
                {
                    bool isROBCValid = false;
                    if (circuitGlobalID != null && !string.IsNullOrEmpty(circuitGlobalID.ToString()) &&
                        IsCircuitROBCValid(circuitGlobalID.ToString(), false))
                    {
                        isROBCValid = true;
                    }
                    else if (pcpGlobalID != null && !string.IsNullOrEmpty(pcpGlobalID.ToString()) &&
                             IsPCPROBCValid(pcpGlobalID.ToString(), false))
                    {
                        isROBCValid = true;
                    }
                    else
                    {
                        throw new Exception(
                            "Must specify a valid Partial curtailment point or circuit source globalid for the ROBC Record");
                    }

                    if (isROBCValid)
                    {
                        sqlInsertStatement = GetUpdateStatement(ROBCTable.OIDFieldName, "77", "EDGIS.ROBC", robcRecord);
                        _EderDatabaseConnection.ExecuteSQL(sqlInsertStatement);

                        if (circuitGlobalID != null && !string.IsNullOrEmpty(circuitGlobalID.ToString()))
                        {
                            updatedROBCRecord = FindCircuitROBC(circuitGlobalID.ToString());
                        }
                        else if (pcpGlobalID != null && !string.IsNullOrEmpty(pcpGlobalID.ToString()))
                        {
                            updatedROBCRecord = FindPCPROBC(pcpGlobalID.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to create record: Sql: " + sqlInsertStatement + ": Exception: " + ex.Message);
            }
            finally
            {
                if (ROBCTable != null)
                {
                    Marshal.ReleaseComObject(ROBCTable);
                }
            }

            return updatedROBCRecord as ROBCRecord;
        }

        /// <summary>
        /// Saves a collection of ROBCRecord objects to the database.
        /// </summary>
        /// <param name="robcRecordsToSave">List of ROBC records to save</param>
        public void SaveROBCs(List<ROBCRecord> robcRecordsToSave)
        {
            foreach (ROBCRecord robc in robcRecordsToSave)
            {
                object circuitGlobalID = robc.GetFieldValue("CIRCUITSOURCEGUID");
                object pcpGlobalID = robc.GetFieldValue("PARTCURTAILPOINTGUID");

                if (circuitGlobalID != null && !string.IsNullOrEmpty(circuitGlobalID.ToString()))
                {
                    IsCircuitROBCValid(circuitGlobalID.ToString(), false);
                }
                if (pcpGlobalID != null && !string.IsNullOrEmpty(pcpGlobalID.ToString()))
                {
                    IsCircuitROBCValid(pcpGlobalID.ToString(), false);
                }
            }

            foreach (ROBCRecord robc in robcRecordsToSave)
            {
                if (!string.IsNullOrEmpty(FindROBC(robc.GlobalID.ToString()).GlobalID.ToString()))
                {
                    UpdateROBC(robc);
                }
                else
                {
                    CreateROBC(robc);
                }
            }
        }

        /// <summary>
        /// Returns a dictionary of Code/Description pairs for the specified domain
        /// </summary>
        /// <param name="domainName">Name of the domain</param>
        /// <returns>Code/Description pairs of the specified domain</returns>
        public Dictionary<string, string> GetDomainValues(string domainName)
        {
            return _EderOracleConnection.GetDomainDescriptions(domainName);
        }

        /// <summary>
        /// Returns a list of Circuit Source records for feeders that feed this circuit. The final list will include the provided circuit as well.
        /// </summary>
        /// <param name="circuitSourceRecord">Circuit Source Record to find child feeders of</param>
        /// <returns></returns>
        public List<CircuitSourceRecord> GetFeedingFeeders(CircuitSourceRecord circuitSourceRecord)
        {
            List<CircuitSourceRecord> feedingCircuits = new List<CircuitSourceRecord>();
            try
            {
                object objCircuitID = circuitSourceRecord.GetFieldValue("CIRCUITID");
                string strCircuitID = objCircuitID.ToString();
                Dictionary<string, NetworkSourceType> resultParentCircuits = new Dictionary<string, NetworkSourceType>();
                resultParentCircuits.Add(strCircuitID, NetworkSourceType.EDER);
                GetParentChildCircuitsForCircuitID(strCircuitID, NetworkSourceType.EDER, ref resultParentCircuits, TraceType.PARENT);
                resultParentCircuits.Remove(strCircuitID);
                foreach (string feederID in resultParentCircuits.Keys)
                {
                    if (resultParentCircuits[feederID] == NetworkSourceType.EDER)
                    {
                        CircuitSourceRecord feedingFeeder = FindCircuit_NoLoadingNoScada(feederID);
                        if (!string.IsNullOrEmpty(feedingFeeder.GlobalID.ToString()))
                        {
                            feedingCircuits.Add(feedingFeeder);
                        }
                    }
                }
            }
            catch
            {
            }
            return feedingCircuits;
        }

        /// <summary>
        /// Returns a list of Circuit Source records for feeders that feed this circuit. The final list will include the provided circuit as well.
        /// </summary>
        /// <param name="circuitSourceRecord">Circuit Source Record to find child feeders of</param>
        /// <returns></returns>
        public CircuitSourceRecord GetFeedingFeeder(CircuitSourceRecord circuitSourceRecord)
        {
            CircuitSourceRecord feedingCircuit = null;
            try
            {
                object objCircuitID = circuitSourceRecord.GetFieldValue("CIRCUITID");
                string strCircuitID = objCircuitID.ToString();
                Dictionary<string, NetworkSourceType> resultParentCircuits = new Dictionary<string, NetworkSourceType>();
                resultParentCircuits.Add(strCircuitID, NetworkSourceType.EDER);
                GetParentChildCircuitsForCircuitID(strCircuitID, NetworkSourceType.EDER, ref resultParentCircuits, TraceType.PARENT);
                resultParentCircuits.Remove(strCircuitID);
                foreach (string feederID in resultParentCircuits.Keys)
                {
                    if (resultParentCircuits[feederID] == NetworkSourceType.EDER)
                    {
                        CircuitSourceRecord feedingFeeder = FindCircuit_NoLoadingNoScada(feederID);
                        if (!string.IsNullOrEmpty(feedingFeeder.GlobalID.ToString()))
                        {
                            feedingCircuit = feedingFeeder;
                        }
                    }
                }
            }
            catch
            {
            }
            return feedingCircuit;
        }

        /// <summary>
        /// Returns a list of Circuit Source records for feeders that the provided record feeds. The final list will include the provided circuit as well.
        /// </summary>
        /// <param name="circuitSourceRecord">Circuit Source Record to find child feeders of</param>
        /// <returns></returns>
        public List<CircuitSourceRecord> GetChildFeeders(CircuitSourceRecord circuitSourceRecord)
        {
            List<CircuitSourceRecord> childCircuits = new List<CircuitSourceRecord>();
            try
            {
                object objCircuitID = circuitSourceRecord.GetFieldValue("CIRCUITID");
                string strCircuitID = objCircuitID.ToString();
                Dictionary<string, NetworkSourceType> resultParentCircuits = new Dictionary<string, NetworkSourceType>();
                resultParentCircuits.Add(strCircuitID, NetworkSourceType.EDER);
                GetParentChildCircuitsForCircuitID(strCircuitID, NetworkSourceType.EDER, ref resultParentCircuits, TraceType.CHILD);
                resultParentCircuits.Remove(strCircuitID);
                foreach (string feederID in resultParentCircuits.Keys)
                {
                    if (resultParentCircuits[feederID] == NetworkSourceType.EDER)
                    {
                        CircuitSourceRecord feedingFeeder = FindCircuit_NoLoadingNoScada(feederID);
                        if (!string.IsNullOrEmpty(feedingFeeder.GlobalID.ToString()))
                        {
                            childCircuits.Add(feedingFeeder);
                        }
                    }
                }
            }
            catch
            {
            }
            return childCircuits;
        }

        /// <summary>
        /// This method will query for any circuit sources that do not have an associated ROBC record.
        /// </summary>
        /// <returns>Returns a list of Circuit Source records without an associated ROBC</returns>
        public List<CircuitSourceRecord> GetCircuitsWithoutROBC()
        {
            List<CircuitSourceRecord> circuitRecords = new List<CircuitSourceRecord>();

            IQueryFilter qf = new QueryFilterClass();
            ICursor rowCursor = null;
            ITable circuitSourceTable = null;
            IRow row = null;

            try
            {
                circuitSourceTable = GetTable(_EderDatabaseConnection, "CIRCUITSOURCE");
                string feederIDFieldName = GetFieldName(((IObjectClass)circuitSourceTable), "FEEDERID");
                int feederIDIdx = circuitSourceTable.Fields.FindField(feederIDFieldName);
                GetQueryFilter(ref qf, circuitSourceTable.Fields,
                    "GLOBALID NOT IN (SELECT CIRCUITSOURCEGUID FROM EDGIS.ROBC WHERE CIRCUITSOURCEGUID IS NOT NULL)");
                rowCursor = circuitSourceTable.Search(qf, false);

                while ((row = rowCursor.NextRow()) != null)
                {
                    CircuitSourceRecord newRecord = new CircuitSourceRecord();
                    for (int i = 0; i < row.Fields.FieldCount; i++)
                    {
                        string fieldName = row.Fields.get_Field(i).Name;
                        newRecord.AddField(fieldName, row.get_Value(i));
                    }
                    while (Marshal.ReleaseComObject(row) > 0)
                    {
                    }

                    newRecord.IsScada = IsScada((CircuitSourceRecord)newRecord);
                    CircuitSourceRecord record = newRecord as CircuitSourceRecord;
                    GetLoadingInformation(ref record, true);

                    circuitRecords.Add(newRecord);
                }
            }
            finally
            {
                if (qf != null)
                {
                    while (Marshal.ReleaseComObject(qf) > 0)
                    {
                    }
                }
                if (row != null)
                {
                    while (Marshal.ReleaseComObject(row) > 0)
                    {
                    }
                }
                if (rowCursor != null)
                {
                    while (Marshal.ReleaseComObject(rowCursor) > 0)
                    {
                    }
                }
                if (circuitSourceTable != null)
                {
                    Marshal.ReleaseComObject(circuitSourceTable);
                }
            }

            return circuitRecords;
        }

        /// <summary>
        /// This method will query for any circuit sources that do not have an associated ROBC record. Also it does not include loading information
        /// </summary>
        /// <returns>Returns a list of Circuit Source records without an associated ROBC</returns>
        public List<CircuitSourceRecord> GetCircuitsWithoutROBCWithoutLoadingInformation()
        {
            List<CircuitSourceRecord> circuitRecords = new List<CircuitSourceRecord>();
            IQueryFilter qf = new QueryFilterClass();
            ICursor rowCursor = null;
            ITable circuitSourceTable = null;
            IRow row = null;
            try
            {
                circuitSourceTable = GetTable(_EderDatabaseConnection, "CIRCUITSOURCE");
                string feederIDFieldName = GetFieldName(((IObjectClass)circuitSourceTable), "FEEDERID");
                int feederIDIdx = circuitSourceTable.Fields.FindField(feederIDFieldName);
                GetQueryFilter(ref qf, circuitSourceTable.Fields,
                    " GLOBALID NOT IN (SELECT CIRCUITSOURCEGUID FROM EDGIS.ROBC WHERE CIRCUITSOURCEGUID IS NOT NULL) ");
                rowCursor = circuitSourceTable.Search(qf, false);
                while ((row = rowCursor.NextRow()) != null)
                {
                    CircuitSourceRecord newRecord = new CircuitSourceRecord();
                    for (int i = 0; i < row.Fields.FieldCount; i++)
                    {
                        string fieldName = row.Fields.get_Field(i).Name;
                        newRecord.AddField(fieldName, row.get_Value(i));
                    }
                    while (Marshal.ReleaseComObject(row) > 0)
                    {
                    }
                    //newRecord.IsScada = IsScada((CircuitSourceRecord)newRecord);
                    newRecord.IsScada = ScadaIndicator.Unknown;
                    CircuitSourceRecord record = newRecord as CircuitSourceRecord;
                    circuitRecords.Add(newRecord);
                }
            }
            catch (Exception EX)
            {
            }
            finally
            {
                if (qf != null)
                {
                    while (Marshal.ReleaseComObject(qf) > 0)
                    {
                    }
                }
                if (row != null)
                {
                    while (Marshal.ReleaseComObject(row) > 0)
                    {
                    }
                }
                if (rowCursor != null)
                {
                    while (Marshal.ReleaseComObject(rowCursor) > 0)
                    {
                    }
                }
                if (circuitSourceTable != null)
                {
                    Marshal.ReleaseComObject(circuitSourceTable);
                }
            }

            return circuitRecords;
        }

        /// <summary>
        /// This method will determine all ROBCRecords that exist that do not have an associated circuit
        /// </summary>
        /// <returns></returns>
        public List<ROBCRecord> GetROBCWithoutCircuit()
        {
            List<ROBCRecord> robcRecords = new List<ROBCRecord>();
            IQueryFilter qf = new QueryFilterClass();
            ICursor rowCursor = null;
            ITable robcTable = null;
            IRow row = null;
            try
            {
                robcTable = GetTable("EDGIS.ROBC", _EderDatabaseConnection);
                int circuitSourceGUIDField = robcTable.FindField("CIRCUITSOURCEGUID");
                GetQueryFilter(ref qf, robcTable.Fields, " CIRCUITSOURCEGUID is null or CIRCUITSOURCEGUID not in (select globalid from " + _TableNameCIRCUITSOURCEMVView + ")");
                List<BaseRecord> baseRecords = new List<BaseRecord>();
                baseRecords = ObtainBaseRecords(robcTable, qf, typeof(ROBCRecord));
                foreach (BaseRecord record in baseRecords)
                {
                    robcRecords.Add((ROBCRecord)record);
                }
            }
            finally
            {
                if (qf != null)
                {
                    while (Marshal.ReleaseComObject(qf) > 0)
                    {
                    }
                }
                if (row != null)
                {
                    while (Marshal.ReleaseComObject(row) > 0)
                    {
                    }
                }
                if (rowCursor != null)
                {
                    while (Marshal.ReleaseComObject(rowCursor) > 0)
                    {
                    }
                }
                if (robcTable != null)
                {
                    Marshal.ReleaseComObject(robcTable);
                }
            }

            return robcRecords;
        }

        /// <summary>
        /// Searches for an device record based on the circuit ID and operating number
        /// </summary>
        /// <param name="circuitID">Circuit ID the PCP exists on</param>
        /// <param name="operatingNumber">Operating number to search</param>
        /// <returns>ROBC record found</returns>
        public List<BaseRecord> FindDevices(string circuitID, string operatingNumber)
        {
            IQueryFilter qf = new QueryFilterClass();
            ITable pcpTable = null;
            List<BaseRecord> deviceRecords = new List<BaseRecord>();

            try
            {
                pcpTable = GetTable("EDGIS.PartialCurtailPoint", _EderDatabaseConnection);
                IObjectClass pcpClass = pcpTable as IObjectClass;
                IEnumRelationshipClass pcpRelClasses =
                    pcpClass.get_RelationshipClasses(esriRelRole.esriRelRoleDestination);
                pcpRelClasses.Reset();
                IRelationshipClass relClass = null;
                while ((relClass = pcpRelClasses.Next()) != null)
                {
                    if (relClass.OriginClass.ObjectClassID != pcpClass.ObjectClassID)
                    {
                        IField opNumField = ModelNameManager.Instance.FieldFromModelName(relClass.OriginClass,
                            "PGE_OPERATINGNUMBER");
                        if (opNumField == null)
                        {
                            continue;
                        }
                        IField circuitIDField = ModelNameManager.Instance.FieldFromModelName(relClass.OriginClass,
                            "FEEDERID");
                        if (circuitIDField == null)
                        {
                            continue;
                        }

                        string foreignKey = relClass.OriginForeignKey;
                        int globalIDIdx = relClass.OriginClass.FindField("GLOBALID");
                        IQueryFilter qf2 = new QueryFilterClass();
                        qf2.AddField("GLOBALID");
                        qf2.WhereClause = circuitIDField.Name + " = '" + circuitID + "' AND UPPER(" + opNumField.Name +
                                          ") = '" + operatingNumber + "'";
                        ICursor cursor = ((ITable)relClass.OriginClass).Search(qf2, false);
                        IRow row = null;
                        while ((row = cursor.NextRow()) != null)
                        {
                            BaseRecord newRecord = null;

                            if (((IDataset)relClass.OriginClass).BrowseName.ToUpper() == "EDGIS.SWITCH")
                            {
                                newRecord = ObtainBaseRecord((ITable)relClass.OriginClass, qf2, typeof(SwitchRecord));
                            }

                            if (((IDataset)relClass.OriginClass).BrowseName.ToUpper() == "EDGIS.DYNAMICPROTECTIVEDEVICE")
                            {
                                newRecord = ObtainBaseRecord((ITable)relClass.OriginClass, qf2, typeof(DPDRecord));
                            }

                            if (newRecord != null)
                            {
                                deviceRecords.Add(newRecord);
                            }

                            if (row != null)
                            {
                                while (Marshal.ReleaseComObject(row) > 0)
                                {
                                }
                            }
                        }

                        if (row != null)
                        {
                            while (Marshal.ReleaseComObject(row) > 0)
                            {
                            }
                        }
                        if (cursor != null)
                        {
                            while (Marshal.ReleaseComObject(cursor) > 0)
                            {
                            }
                        }
                        if (qf2 != null)
                        {
                            while (Marshal.ReleaseComObject(qf2) > 0)
                            {
                            }
                        }
                    }
                    if (relClass != null)
                    {
                        while (Marshal.ReleaseComObject(relClass) > 0)
                        {
                        }
                    }
                }
            }
            finally
            {
                if (qf != null)
                {
                    Marshal.ReleaseComObject(qf);
                }
                if (pcpTable != null)
                {
                    Marshal.ReleaseComObject(pcpTable);
                }
            }

            return deviceRecords;
        }

        /// <summary>
        /// Searches for an device record based on the circuit ID and operating number
        /// </summary>
        /// <param name="circuitID">Circuit ID the PCP exists on</param>
        /// <param name="operatingNumber">Operating number to search</param>
        /// <returns>ROBC record found</returns>
        public List<BaseRecord> FindDeviceFromPCP(PCPRecord pcpRecord)
        {
            IQueryFilter qf = new QueryFilterClass();
            ITable pcpTable = null;
            List<BaseRecord> deviceRecords = new List<BaseRecord>();

            try
            {
                pcpTable = GetTable("EDGIS.PartialCurtailPoint", _EderDatabaseConnection);
                IObjectClass pcpClass = pcpTable as IObjectClass;
                IEnumRelationshipClass pcpRelClasses =
                    pcpClass.get_RelationshipClasses(esriRelRole.esriRelRoleDestination);
                pcpRelClasses.Reset();
                IRelationshipClass relClass = null;
                while ((relClass = pcpRelClasses.Next()) != null)
                {
                    if (relClass.OriginClass.ObjectClassID != pcpClass.ObjectClassID)
                    {
                        IField opNumField = ModelNameManager.Instance.FieldFromModelName(relClass.OriginClass,
                            "PGE_OPERATINGNUMBER");
                        if (opNumField == null)
                        {
                            continue;
                        }
                        IField circuitIDField = ModelNameManager.Instance.FieldFromModelName(relClass.OriginClass,
                            "FEEDERID");
                        if (circuitIDField == null)
                        {
                            continue;
                        }

                        string foreignKey = relClass.OriginForeignKey;
                        int globalIDIdx = relClass.OriginClass.FindField("GLOBALID");
                        IQueryFilter qf2 = new QueryFilterClass();
                        //qf2.SubFields= "OBJECTID,GLOBALID";
                        qf2.SubFields = "SUBTYPECD, CIRCUITID, OPERATINGNUMBER";
                        //qf2.WhereClause = " GLOBALID = '"+pcpRecord.GlobalID+"' ";
                        qf2.WhereClause = " GLOBALID = '" + pcpRecord.GetFieldValue("DEVICEGUID") + "' ";
                        ICursor cursor = ((ITable)relClass.OriginClass).Search(qf2, false);
                        IRow row = null;
                        while ((row = cursor.NextRow()) != null)
                        {
                            BaseRecord newRecord = null;

                            if (((IDataset)relClass.OriginClass).BrowseName.ToUpper() == "EDGIS.SWITCH")
                            {
                                newRecord = ObtainBaseRecord((ITable)relClass.OriginClass, qf2, typeof(SwitchRecord));
                            }

                            if (((IDataset)relClass.OriginClass).BrowseName.ToUpper() == "EDGIS.DYNAMICPROTECTIVEDEVICE")
                            {
                                newRecord = ObtainBaseRecord((ITable)relClass.OriginClass, qf2, typeof(DPDRecord));
                            }

                            if (newRecord != null)
                            {
                                deviceRecords.Add(newRecord);
                            }

                            if (row != null)
                            {
                                while (Marshal.ReleaseComObject(row) > 0)
                                {
                                }
                            }
                        }

                        if (row != null)
                        {
                            while (Marshal.ReleaseComObject(row) > 0)
                            {
                            }
                        }
                        if (cursor != null)
                        {
                            while (Marshal.ReleaseComObject(cursor) > 0)
                            {
                            }
                        }
                        if (qf2 != null)
                        {
                            while (Marshal.ReleaseComObject(qf2) > 0)
                            {
                            }
                        }
                    }
                    if (relClass != null)
                    {
                        while (Marshal.ReleaseComObject(relClass) > 0)
                        {
                        }
                    }
                }
            }
            finally
            {
                if (qf != null)
                {
                    Marshal.ReleaseComObject(qf);
                }
                if (pcpTable != null)
                {
                    Marshal.ReleaseComObject(pcpTable);
                }
            }

            return deviceRecords;
        }


        /// <summary>
        /// Searches for an PCP Record based on the PCP global ID
        /// </summary>
        /// <param name="FieldName">FieldName PCP Record</param>
        /// <param name="FieldValue">FieldValue PCP Record</param>
        /// <returns>PCP record found</returns>
        public List<PCPRecord> FindPCPRecords(string FieldName, string FieldValue)
        {
            IQueryFilter qf = new QueryFilterClass();
            ITable pcpTable = null;
            List<PCPRecord> pcpRecords = new List<PCPRecord>();
            try
            {
                pcpTable = GetTable("EDGIS.PartialCurtailPoint", _EderDatabaseConnection);
                GetQueryFilter(ref qf, pcpTable.Fields, FieldName + " = '" + FieldValue + "'");
                List<BaseRecord> foundRecords = ObtainBaseRecords(pcpTable, qf, typeof(PCPRecord));
                foreach (BaseRecord record in foundRecords)
                {
                    pcpRecords.Add((PCPRecord)record);
                }
            }
            finally
            {
                if (qf != null)
                {
                    Marshal.ReleaseComObject(qf);
                }
                if (pcpTable != null)
                {
                    Marshal.ReleaseComObject(pcpTable);
                }
            }

            return pcpRecords;
        }

        /// <summary>
        /// Searches for an PCP Record based on the related devices global ID
        /// </summary>
        /// <param name="DeviceGUID">Global ID of the Device associated with the PCP record</param>
        /// <returns>PCP record found</returns>
        public PCPRecord FindPCP(string DeviceGUID)
        {
            BaseRecord pcpRecord = null;
            IQueryFilter qf = new QueryFilterClass();
            ITable pcpTable = null;

            try
            {
                List<PCPRecord> pcpRecords = FindPCPRecords("DEVICEGUID", DeviceGUID);
                if (pcpRecords.Count > 0)
                {
                    pcpRecord = pcpRecords[0];
                }
            }
            finally
            {
                if (qf != null)
                {
                    Marshal.ReleaseComObject(qf);
                }
                if (pcpTable != null)
                {
                    Marshal.ReleaseComObject(pcpTable);
                }
            }

            return pcpRecord as PCPRecord;
        }

        public PCPRecord FindPCPByGlobalID(string PCPGlobalID)
        {
            BaseRecord pcpRecord;
            IQueryFilter qf = new QueryFilterClass();
            ITable pcpTable = null;

            try
            {
                pcpTable = GetTable("EDGIS.PartialCurtailPoint", _EderDatabaseConnection);
                GetQueryFilter(ref qf, pcpTable.Fields, "GLOBALID = '" + PCPGlobalID + "'");
                pcpRecord = ObtainBaseRecord(pcpTable, qf, typeof(PCPRecord));
            }
            finally
            {
                if (qf != null)
                {
                    Marshal.ReleaseComObject(qf);
                }
                if (pcpTable != null)
                {
                    Marshal.ReleaseComObject(pcpTable);
                }
            }

            return pcpRecord as PCPRecord;
        }

        /// <summary>
        /// Searches for an PCP Record based on the PCP global ID
        /// </summary>
        /// <param name="PCPGlobalID">Global ID of the PCP Record</param>
        /// <returns>PCP record found</returns>
        public PCPRecord FindPCPByGlobalID(string PCPGlobalID, bool scada, bool loadingInfo)
        {
            BaseRecord pcpRecord;
            IQueryFilter qf = new QueryFilterClass();
            ITable pcpTable = null;

            try
            {
                pcpTable = GetTable("EDGIS.PartialCurtailPoint", _EderDatabaseConnection);
                GetQueryFilter(ref qf, pcpTable.Fields, "GLOBALID = '" + PCPGlobalID + "'");
                pcpRecord = ObtainBaseRecord(pcpTable, qf, typeof(PCPRecord), scada, loadingInfo);
            }
            finally
            {
                if (qf != null)
                {
                    Marshal.ReleaseComObject(qf);
                }
                if (pcpTable != null)
                {
                    Marshal.ReleaseComObject(pcpTable);
                }
            }

            return pcpRecord as PCPRecord;
        }

        /// <summary>
        /// Creates or updates a PCP record with the passed in parameters.
        /// </summary>
        /// <param name="pcpRecord">PCP record to create/update</param>
        /// <returns>Returns the PCP record object</returns>
        public PCPRecord CreatePCP(PCPRecord pcpRecord)
        {
            BaseRecord updatedPCPRecord = new ROBCRecord();
            ITable PCPTable = null;
            string sqlInsertStatement = "";
            try
            {
                PCPTable = GetTable("EDGIS.PARTIALCURTAILPOINT", _EderDatabaseConnection);

                string pcpGuid = "";
                if (FindPCPByGUID(pcpRecord.GlobalID.ToString()).GlobalID == "")
                {
                    sqlInsertStatement = GetInsertStatement(PCPTable.OIDFieldName, "78", "EDGIS.PARTIALCURTAILPOINT",
                        pcpRecord, ref pcpGuid);
                    _EderDatabaseConnection.ExecuteSQL(sqlInsertStatement);
                }
                else
                {
                    pcpGuid = pcpRecord.GlobalID.ToString();
                    sqlInsertStatement = GetUpdateStatement(PCPTable.OIDFieldName, "78", "EDGIS.PARTIALCURTAILPOINT",
                        pcpRecord);
                    _EderDatabaseConnection.ExecuteSQL(sqlInsertStatement);
                }

                updatedPCPRecord = FindPCPByGUID(pcpGuid);

            }
            catch (Exception ex)
            {
                throw new Exception("Unable to create record: Sql: " + sqlInsertStatement + ": Exception: " + ex.Message);
            }
            finally
            {
                if (PCPTable != null)
                {
                    Marshal.ReleaseComObject(PCPTable);
                }
            }
            return updatedPCPRecord as PCPRecord;
        }

        /// <summary>
        /// Updates a PCP record with the passed in parameters.
        /// </summary>
        /// <param name="pcpRecord">PCP record to update</param>
        /// <returns>Returns the PCP record object</returns>
        public PCPRecord UpdatePCP(PCPRecord pcpRecord)
        {
            return CreatePCP(pcpRecord);
        }

        /// <summary>
        /// Deletes the specified PCP record matching the passed in GUID.  Returns false if no such record exists.
        /// </summary>
        /// <param name="PCPGuid">GlobalID of the PCP record</param>
        /// <returns></returns>
        public bool DeletePCP(string PCPGuid)
        {
            string deleteStatement = "";
            try
            {
                if (FindPCPByGUID(PCPGuid).GlobalID == "")
                {
                    //record doesn't exist so we can't delete it.
                    return false;
                }
                else
                {
                    deleteStatement = "DELETE from EDGIS.PARTIALCURTAILPOINT WHERE GLOBALID = '" + PCPGuid + "'";
                    _EderDatabaseConnection.ExecuteSQL(deleteStatement);


                    return true;

                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to delete the PCP record: " + PCPGuid + " Ex: " + ex.Message);
            }
        }

        /// <summary>
        /// Deletes the specified ROBC record matching the passed in GUID.  Returns false if no such record exists.
        /// </summary>
        /// <param name="ROBCGuid">GlobalID of the ROBC record</param>
        /// <returns></returns>
        public bool DeleteROBC(string ROBCGuid)
        {
            string deleteStatement = "";
            try
            {
                if (FindROBC(ROBCGuid).GlobalID == "")
                {
                    //record doesn't exist so we can't delete it.
                    return false;
                }
                else
                {
                    deleteStatement = "DELETE from EDGIS.ROBC WHERE GLOBALID = '" + ROBCGuid + "'";
                    _EderDatabaseConnection.ExecuteSQL(deleteStatement);
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to delete the ROBC record: " + ROBCGuid + " Ex: " + ex.Message);
            }
        }

        /// <summary>
        /// Returns a bool if there is an essential customer being provided energy by Circuit.
        /// </summary>
        /// <param name="CircuitID">The Circuit ID to be processed.</param>
        /// <returns>True if it has an Essential Customer</returns>
        /// 
        public bool hasEssentialCustomerForCircuit(string CircuitID)
        {
            bool bReturnEssential = false;

            IEnumNetEID eJunctions = null;
            IEnumNetEID eEdges = null;
            Dictionary<string, NetworkSourceType> ChildCircuitDictionary = new Dictionary<string, NetworkSourceType>();
            ChildCircuitDictionary.Add(CircuitID, NetworkSourceType.EDER);
            GetParentChildCircuitsForCircuitID(CircuitID, NetworkSourceType.EDER, ref ChildCircuitDictionary, TraceType.CHILD);
            List<int> junctionEIDs = new List<int>();
            foreach (string CircuitFound in ChildCircuitDictionary.Keys)
            {
                if (ChildCircuitDictionary[CircuitFound] == NetworkSourceType.EDER)
                {
                    ArcFMDwnstreamTrace(_EderGeomNetwork, CircuitFound, ref eJunctions, ref eEdges);
                    if (eJunctions != null)
                    {
                        eJunctions.Reset();
                        int junctionEID = -1;
                        while ((junctionEID = eJunctions.Next()) > 0)
                        {
                            if (!junctionEIDs.Contains(junctionEID))
                            {
                                junctionEIDs.Add(junctionEID);
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("Unable to trace the supplied Circuit of " + CircuitID);
                    }
                }
            }
            List<int> foundXFMRList = new List<int>();
            List<int> foundPriMeterList = new List<int>();
            List<int> foundDCRectifierList = new List<int>();
            FilterJunctions(junctionEIDs, ref foundXFMRList, ref foundPriMeterList, ref foundDCRectifierList);
            //bReturnEssential = bHasEssentialCustomersFromJunctionsList(junctionEIDs);
            bReturnEssential = bHasEssentialCustomersFromOIDList(foundXFMRList, foundPriMeterList, foundDCRectifierList);
            return bReturnEssential;
        }

        /// <summary>
        ///  Returns a bool if there is an essential customer being provided energy by the PCP
        /// </summary>
        /// <param name="pcpRecord"></param>
        /// <returns>True if it has an Essential Customer</returns>
        public bool hasEssentialCustomerForPcp(PCPRecord pcpRecord)
        {
            bool bReturnEssential = false;
            string strGlobalID = pcpRecord.GlobalID.ToString();
            string strTableName = GetTableNameOfDevice(strGlobalID);
            if (strTableName != "")
            {
                bReturnEssential = hasEssentialCustomerForDevice(strGlobalID, strTableName);
            }
            return bReturnEssential;
        }

        /// <summary>
        ///  Returns a bool if there is an essential customer being provided energy by the Device.
        /// </summary>
        /// <param name="sDeviceGUID">Globalid Of the Device to check</param>
        /// <returns>True if it has an Essential Customer</returns>
        public bool hasEssentialCustomerForDevice(string sDeviceGUID)
        {
            bool bReturnEssential = true;
            string sFCName = GetTableNameOfDevice(sDeviceGUID);
            if (sFCName != "")
            {
                bReturnEssential = hasEssentialCustomerForDevice(sDeviceGUID, sFCName);
            }
            return bReturnEssential;
        }

        /// <summary>
        ///  Returns a bool if there is an essential customer being provided energy by the Device.
        /// </summary>
        /// <param name="sDeviceGUID">GlobalID of the Device to Check</param>
        /// <param name="sFCName">Feature Class Name of the Device to Check</param>
        /// <returns>True if it has an Essential Customer</returns>
        public bool hasEssentialCustomerForDevice(string sDeviceGUID, string sFCName)
        {
            bool bReturnEssential = false;

            IEnumNetEID eJunctions = null;
            IEnumNetEID eEdges = null;
            ArcFMDwnstreamTraceFromFeature(_EderGeomNetwork, sFCName, sDeviceGUID, ref eJunctions, ref eEdges);
            List<int> foundXFMRList = new List<int>();
            List<int> foundPriMeterList = new List<int>();
            List<int> foundDCRectifierList = new List<int>();
            List<int> foundLoadSource = new List<int>();
            List<int> junctionEIDs = new List<int>();
            eJunctions.Reset();
            int junctionEID = -1;
            while ((junctionEID = eJunctions.Next()) > 0)
            {
                if (!junctionEIDs.Contains(junctionEID))
                {
                    junctionEIDs.Add(junctionEID);
                }
            }
            FilterJunctions(junctionEIDs, ref foundXFMRList, ref foundPriMeterList, ref foundDCRectifierList, ref foundLoadSource);
            bReturnEssential = bHasEssentialCustomersFromOIDList(foundXFMRList, foundPriMeterList, foundDCRectifierList);
            if (bReturnEssential == false)
            {
                if (foundLoadSource.Count > 0)
                {
                    Dictionary<string, NetworkSourceType> ChildCircuitDictionary = new Dictionary<string, NetworkSourceType>();
                    foreach (int iObjectID in foundLoadSource)
                    {
                        GetChildCircuitsForLoadStitchPoint(iObjectID, NetworkSourceType.EDER, ref ChildCircuitDictionary);
                    }
                    if (ChildCircuitDictionary.Count > 0)
                    {
                        foreach (string CircuitFound in ChildCircuitDictionary.Keys)
                        {
                            if (bReturnEssential == false)
                            {
                                if (ChildCircuitDictionary[CircuitFound] == NetworkSourceType.EDER)
                                {
                                    bReturnEssential = hasEssentialCustomerForCircuit(CircuitFound);
                                }
                            }
                        }
                    }
                }
            }
            return bReturnEssential;
        }


        /// <summary>
        /// Return the count of total customers being provided energy by the Device.
        /// </summary>
        /// <param name="sDeviceGUID">GlobalID of the Device to Check</param>
        /// <param name="sFCName">Feature Class Name of the Device to Check</param>
        /// <returns>Total count of customers found</returns>
        public int getDeviceCustomerCount(string sDeviceGUID)
        {
            int iReturnCount = -1;
            string strTableName = GetTableNameOfDevice(sDeviceGUID);
            if (strTableName != "")
            {
                iReturnCount = getDeviceCustomerCount(sDeviceGUID, strTableName);
            }
            return iReturnCount;
        }

        /// <summary>
        /// Return the count of total customers being provided energy by the Device.
        /// </summary>
        /// <param name="sDeviceGUID">GlobalID of the Device to Check</param>
        /// <param name="sFCName">Feature Class Name of the Device to Check</param>
        /// <returns>Total count of customers found</returns>
        public int getDeviceCustomerCount(string sDeviceGUID, string sFCName)
        {
            int iReturnCount = 0;
            IEnumNetEID eJunctions = null;
            IEnumNetEID eEdges = null;
            ArcFMDwnstreamTraceFromFeature(_EderGeomNetwork, sFCName, sDeviceGUID, ref eJunctions, ref eEdges);

            List<int> junctionEIDs = new List<int>();
            eJunctions.Reset();
            int junctionEID = -1;

            while ((junctionEID = eJunctions.Next()) > 0)
            {
                junctionEIDs.Add(junctionEID);
            }
            //junctionEIDs = FilterJunctions(junctionEIDs);
            //iReturnCount = iCountAllCustomersFromJunctionsList(junctionEIDs);
            List<int> foundXFMRList = new List<int>();
            List<int> foundPriMeterList = new List<int>();
            List<int> foundDCRectifierList = new List<int>();
            FilterJunctions(junctionEIDs, ref foundXFMRList, ref foundPriMeterList, ref foundDCRectifierList);
            iReturnCount = iCountAllCustomersFromOIDList(foundXFMRList, foundPriMeterList, foundDCRectifierList);


            return iReturnCount;
        }

        /// <summary>
        /// Return the count of total customers being provided energy by the Circuit.
        /// </summary>
        /// <param name="CircuitID">CircuitID of the circuit to Check</param>
        /// <returns>Total count of customers found</returns>
        public int getCircuitCustomerCount(string CircuitID)
        {
            int iReturnCount = 0;

            IEnumNetEID eJunctions = null;
            IEnumNetEID eEdges = null;
            ArcFMDwnstreamTrace(_EderGeomNetwork, CircuitID, ref eJunctions, ref eEdges);

            List<int> junctionEIDs = new List<int>();
            if (eJunctions != null)
            {
                eJunctions.Reset();
                int junctionEID = -1;
                while ((junctionEID = eJunctions.Next()) > 0)
                {

                    junctionEIDs.Add(junctionEID);
                }
            }
            else
            {
                throw new Exception("Unable to trace the supplied circuit:" + CircuitID);
            }
            //junctionEIDs = FilterJunctions(junctionEIDs);
            //iReturnCount = iCountAllCustomersFromJunctionsList(junctionEIDs);
            List<int> foundXFMRList = new List<int>();
            List<int> foundPriMeterList = new List<int>();
            List<int> foundDCRectifierList = new List<int>();
            FilterJunctions(junctionEIDs, ref foundXFMRList, ref foundPriMeterList, ref foundDCRectifierList);
            iReturnCount = iCountAllCustomersFromOIDList(foundXFMRList, foundPriMeterList, foundDCRectifierList);

            return iReturnCount;
        }

        public ROBCRecord getParentCircuitForPcp(PCPRecord pcpRecord)
        {
            ROBCRecord robcRecord;
            robcRecord = getParentCircuitForPcp((pcpRecord.GlobalID).ToString());
            return robcRecord;
        }

        public ROBCRecord getParentCircuitForPcp(string PCPGlobalID)
        {
            BaseRecord pcpRecord;
            BaseRecord robcRecord;
            IQueryFilter qf1 = new QueryFilterClass();
            IQueryFilter qf2 = new QueryFilterClass();
            ITable pcpTable = null;
            ITable robcTable = null;

            try
            {
                pcpTable = GetTable("EDGIS.PartialCurtailPoint", _EderDatabaseConnection);
                GetQueryFilter(ref qf1, pcpTable.Fields, "GLOBALID = '" + PCPGlobalID + "'");
                pcpRecord = ObtainBaseRecord(pcpTable, qf1, typeof(PCPRecord));
                robcTable = GetTable("EDGIS.ROBC", _EderDatabaseConnection);
                GetQueryFilter(ref qf2, robcTable.Fields, " PARTCURTAILPOINTGUID = '" + PCPGlobalID + "'");
                robcRecord = ObtainBaseRecord(robcTable, qf2, typeof(ROBCRecord));
            }
            finally
            {
                if (qf1 != null)
                {
                    Marshal.ReleaseComObject(qf1);
                }
                if (qf2 != null)
                {
                    Marshal.ReleaseComObject(qf2);
                }
                if (pcpTable != null)
                {
                    Marshal.ReleaseComObject(pcpTable);
                }
                if (robcTable != null)
                {
                    Marshal.ReleaseComObject(robcTable);
                }
            }

            return robcRecord as ROBCRecord;
        }

        //public List<PCPRecord> getInvalidPCPs()
        //{
        //    //BaseRecord pcpRecord;
        //    List<PCPRecord> listResults = new List<PCPRecord>();
        //    IQueryFilter qf1 = new QueryFilterClass();
        //    ITable pcpTable = null;
        //    List<object> tempList = new List<object>();
        //    try
        //    {
        //        pcpTable = GetTable("EDGIS.PartialCurtailPoint", _EderDatabaseConnection);
        //        string strTempSQL = "";
        //        for (int i = 0; i < _ListDevicePotentialTables.Count(); i++)
        //        {
        //            if (i == 0)
        //            {
        //                strTempSQL = " select globalid from " + _ListDevicePotentialTables[i] + " ";
        //            }
        //            else
        //            {
        //                strTempSQL = " union all select globalid from " + _ListDevicePotentialTables[i] + " ";
        //            }
        //        }
        //        if (strTempSQL != "")
        //        {
        //            strTempSQL = "DEVICEGUID not in ( " + strTempSQL + " ) OR ";
        //        }
        //        strTempSQL = strTempSQL + " GLOBALID in ( select PARTCURTAILPOINTGUID from EDGIS.ROBC where NVL(ESTABLISHEDROBC,0)<>NVL(DESIREDROBC,-1)) ";
        //        GetQueryFilter(ref qf1, pcpTable.Fields, strTempSQL);
        //        tempList = ObtainBaseRecordList(pcpTable, qf1, typeof(PCPRecord));
        //        foreach (object obj in tempList)
        //        {
        //            listResults.Add(obj as PCPRecord);
        //        }
        //    }
        //    finally
        //    {
        //        if (qf1 != null)
        //        {
        //            Marshal.ReleaseComObject(qf1);
        //        }
        //        if (pcpTable != null)
        //        {
        //            Marshal.ReleaseComObject(pcpTable);
        //        }
        //    }
        //    return listResults;
        //}

        /// <summary>
        /// Function that returns the PCPs that are not valid in the system. 
        /// </summary>
        /// <returns>Dictionary of PCPRecords with a BaseRecord Array that may be null but uses [0] for ROBC, [1] for Circuit Source,[2] for Switch and [3] for DPD </returns>
        public Dictionary<PCPRecord, BaseRecord[]> getInvalidPCPs()
        {
            Dictionary<PCPRecord, BaseRecord[]> dResults = new Dictionary<PCPRecord, BaseRecord[]>();
            IQueryFilter qf1 = new QueryFilterClass();
            ITable pcpTable = null;
            List<object> tempList = new List<object>();
            try
            {
                pcpTable = GetTable("EDGIS.PartialCurtailPoint", _EderDatabaseConnection);
                string strTempSQL = "";
                for (int i = 0; i < _ListDevicePotentialTables.Count(); i++)
                {
                    if (i == 0)
                    {
                        strTempSQL = " select globalid from " + _ListDevicePotentialTables[i] + " ";
                    }
                    else
                    {
                        strTempSQL = strTempSQL + " union all select globalid from " + _ListDevicePotentialTables[i] + " ";
                    }
                }
                if (strTempSQL != "")
                {
                    strTempSQL = "DEVICEGUID not in ( " + strTempSQL + " ) OR ";
                }
                //strTempSQL = strTempSQL + " GLOBALID in ( select PARTCURTAILPOINTGUID from EDGIS.ROBC where NVL(ESTABLISHEDROBC,0)<>NVL(DESIREDROBC,-1)) ";
                //GetQueryFilter(ref qf1, pcpTable.Fields, strTempSQL);
                //tempList = ObtainBaseRecordList(pcpTable, qf1, typeof(PCPRecord));

                strTempSQL = strTempSQL + " GLOBALID in ( select PARTCURTAILPOINTGUID from EDGIS.ROBC where NVL(ESTABLISHEDROBC,0)<>NVL(DESIREDROBC,-1)) or GLOBALID in ( select PARTCURTAILPOINTGUID from EDGIS.ROBC where ESTABLISHEDROBC in (50,60)) ";
                GetQueryFilter(ref qf1, pcpTable.Fields, strTempSQL);
                tempList = ObtainBaseRecordList(pcpTable, qf1, typeof(PCPRecord));

                foreach (object obj in tempList)
                {
                    PCPRecord tempPCP = obj as PCPRecord;
                    ROBCRecord tempRecord = getParentCircuitForPcp(tempPCP);
                    CircuitSourceRecord tempCircuitSource = null;
                    if (tempRecord != null)
                    {
                        tempCircuitSource = FindCircuitByGUID_noLoadingorScada(tempRecord.GetFieldValue(_FieldName_tbROBC_CircuitSourceGUID).ToString());
                    }
                    List<BaseRecord> tempListDevices = FindDeviceFromPCP(tempPCP);
                    //Dictionary<PCPRecord, BaseRecord[4]{ROBCRecord,CircuitSourceRecord,SwitchRecord,DPDRecord}>
                    SwitchRecord tempSwitch = null;
                    DPDRecord tempDPD = null;
                    foreach (BaseRecord br in tempListDevices)
                    {
                        if (br is SwitchRecord) { tempSwitch = br as SwitchRecord; }
                        else if (br is DPDRecord) { tempDPD = br as DPDRecord; }
                    }
                    BaseRecord[] tempArrayBR = new BaseRecord[4];
                    tempArrayBR[0] = tempRecord;
                    tempArrayBR[1] = tempCircuitSource;
                    tempArrayBR[2] = tempSwitch;
                    tempArrayBR[3] = tempDPD;
                    dResults.Add(tempPCP, tempArrayBR);
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (qf1 != null)
                {
                    Marshal.ReleaseComObject(qf1);
                }
                if (pcpTable != null)
                {
                    Marshal.ReleaseComObject(pcpTable);
                }
            }
            return dResults;
        }


        public ROBCRecord setEstablishedRobcForPcp(PCPRecord pcpRecord)
        {
            ROBCRecord robcRecord;
            try
            {
                object objDeviceGlobalid = pcpRecord.GetFieldValue("DEVICEGUID");
                string strDeviceGUID = objDeviceGlobalid.ToString();
                string strDevicetableName = GetTableNameOfDevice(strDeviceGUID);
                //Note: this is not needed anymore because UI has validation on essential customer so following if statement 
                //never get executed but takes a while to execute hasEssentialCustomerForDevice
                bool bHasEssentialCustomer = hasEssentialCustomerForDevice(strDeviceGUID, strDevicetableName);
                robcRecord = setEstablishedRobcForPcp_WithoutValidation(pcpRecord, bHasEssentialCustomer);
            }
            finally
            {

            }

            return robcRecord;
        }

        public ROBCRecord setEstablishedRobcForPcp_WithoutValidation(PCPRecord pcpRecord, bool bHasEssentialCustomer)
        {
            ROBCRecord robcRecord;
            try
            {
                robcRecord = FindPCPROBC(pcpRecord, true, true);
                bool bNeedToUpdate = configureROBCRecordValues(ref robcRecord, bHasEssentialCustomer);
                if (bNeedToUpdate == true)
                {
                    UpdateROBC(robcRecord);
                }
            }
            finally
            {

            }

            return robcRecord;
        }

        public ROBCRecord setEstablishedRobcForCircuit(ROBCRecord robcRecord)
        {
            try
            {
                object objCiruitSourceGuid = robcRecord.GetFieldValue("CIRCUITSOURCEGUID");
                string strCircuitSourceGUID = objCiruitSourceGuid.ToString();
                CircuitSourceRecord csrCircuitSource = FindCircuitByGUID_noLoadingorScada(strCircuitSourceGUID);
                object objCircuitID = csrCircuitSource.GetFieldValue("CIRCUITID");
                string strCircuitID = objCircuitID.ToString();
                bool bHasEssentialCustomer = hasEssentialCustomerForCircuit(strCircuitID);
                robcRecord = setEstablishedRobcForCircuit_WithoutValidation(robcRecord, bHasEssentialCustomer);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            finally
            {
            }
            return robcRecord;
        }

        public ROBCRecord setEstablishedRobcForCircuit_WithoutValidation(ROBCRecord robcRecord, bool bHasEssentialCustomer)
        {
            try
            {
                bool bNeedToUpdate = configureROBCRecordValues(ref robcRecord, bHasEssentialCustomer);
                if (bNeedToUpdate == true)
                {
                    UpdateROBC(robcRecord);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            finally
            {
            }
            return robcRecord;
        }

        public Dictionary<PCPRecord, BaseRecord[]> reCalculateInvalidPCPs()
        {
            Dictionary<PCPRecord, BaseRecord[]> dictPCPRecords = getInvalidPCPs();
            foreach (PCPRecord testPCPRecord in dictPCPRecords.Keys)
            {
                setEstablishedRobcForPcp(testPCPRecord);
            }
            return dictPCPRecords;
        }


        /// <summary>
        /// Used to get the list of all circuits that supply energy to the circuit in question. Used to build list of upstream circuits.
        /// </summary>
        /// <param name="pGeometricNetwork">The geometric network to use in the trace.</param>
        public Dictionary<string, NetworkSourceType> GetParentCircuitsForCircuitID(string sCircuitID)
        {
            Dictionary<string, NetworkSourceType> dCircuits = new Dictionary<string, NetworkSourceType>();
            dCircuits.Add(sCircuitID, NetworkSourceType.EDER);
            GetParentChildCircuitsForCircuitID(sCircuitID, NetworkSourceType.EDER, ref dCircuits, TraceType.PARENT);
            dCircuits.Remove(sCircuitID);
            return dCircuits;
        }

        /// <summary>
        /// Used to get the list of all circuits that supply energy to the circuit in question. Used to build list of upstream circuits.
        /// </summary>
        /// <param name="pGeometricNetwork">The geometric network to use in the trace.</param>
        public Dictionary<string, NetworkSourceType> GetChildCircuitsForCircuitID(string sCircuitID)
        {
            Dictionary<string, NetworkSourceType> dCircuits = new Dictionary<string, NetworkSourceType>();
            dCircuits.Add(sCircuitID, NetworkSourceType.EDER);
            //GetParentChildCircuitsForCircuitID(sCircuitID, NetworkSourceType.EDER, ref dParentCircuits, TraceType.PARENT);
            GetParentChildCircuitsForCircuitID(sCircuitID, NetworkSourceType.EDER, ref dCircuits, TraceType.CHILD);
            dCircuits.Remove(sCircuitID);
            return dCircuits;
        }

        /// <summary>
        /// Used to loop through all candidate feature classes and find the one with this globalid, returns "" when nothing found.
        /// </summary>
        /// <param name="strDeviceGUID">Globalid of the Device to look for</param>
        /// <returns>Will return the Schema.Table_name and defaults to "" when nothing matched.</returns>
        public string GetTableNameOfDevice(string strDeviceGUID)
        {
            string strTableNameFound = "";
            ITable tableToCheck = null;
            IQueryFilter iQF = null;
            List<string> DeviceSupportedTables = new List<string>(_ListDevicePotentialTables);
            try
            {
                foreach (string deviceTableName in DeviceSupportedTables)
                {
                    tableToCheck = GetTable(deviceTableName, _EderDatabaseConnection);
                    if (tableToCheck != null)
                    {
                        iQF = new QueryFilterClass();
                        GetQueryFilter(ref iQF, tableToCheck.Fields, " GLOBALID = '" + strDeviceGUID + "' ");
                        ICursor iCursor = tableToCheck.Search(iQF, false);
                        IRow rowFound = null;
                        while ((rowFound = iCursor.NextRow()) != null)
                        {
                            strTableNameFound = deviceTableName;
                            Marshal.FinalReleaseComObject(rowFound);
                            rowFound = null;
                        }
                        if (iCursor != null)
                        {
                            Marshal.FinalReleaseComObject(iCursor);
                            iCursor = null;
                        }
                        if (iQF != null)
                        {
                            Marshal.FinalReleaseComObject(iQF);
                            iQF = null;
                        }
                    }
                    if (strTableNameFound != "")
                    {
                        return strTableNameFound;
                    }
                }
            }
            catch
            {
            }
            finally
            {
                if (iQF != null)
                {
                    Marshal.FinalReleaseComObject(iQF);
                }
                if (tableToCheck != null)
                {
                    Marshal.FinalReleaseComObject(tableToCheck);
                }
            }
            return strTableNameFound;
        }

        /// <summary>
        ///  Initializes the Loading Information for the passed in Circuit Source Record
        /// </summary>
        /// <param name="circuitSourceRecord"> CircuitSource Record by reference to update</param>
        /// <param name="getAllCircuits"> Must be set to true for the Child Circuits to be returned. </param>
        public void GetLoadingInformation(ref CircuitSourceRecord circuitSourceRecord, bool getAllCircuits)
        {
            string sCircuitID = (string)circuitSourceRecord.GetFieldValue("CIRCUITID");

            Dictionary<string, NetworkSourceType> dicCircuits = new Dictionary<string, NetworkSourceType>();
            dicCircuits.Add(sCircuitID, NetworkSourceType.EDER);
            if (getAllCircuits)
            {
                GetParentChildCircuitsForCircuitID(sCircuitID, NetworkSourceType.EDER, ref dicCircuits, TraceType.CHILD);
            }

            foreach (string sCID in dicCircuits.Keys)
            {
                if (dicCircuits[sCID].ToString().ToUpper() != "SUBSTATION")
                {
                    IEnumNetEID eJunctions = null;
                    IEnumNetEID eEdges = null;
                    ArcFMDwnstreamTrace(_EderGeomNetwork, sCID, ref eJunctions, ref eEdges);

                    List<int> jEIDList = new List<int>();

                    eJunctions.Reset();
                    int jEID = -1;

                    while ((jEID = eJunctions.Next()) > 0)
                    {
                        jEIDList.Add(jEID);
                    }

                    List<int> foundXFMRList = new List<int>();
                    List<int> foundPriMeterList = new List<int>();
                    List<int> foundDCRectifierList = new List<int>();
                    FilterJunctions(jEIDList, ref foundXFMRList, ref foundPriMeterList, ref foundDCRectifierList);

                    int iTotalCount = iCountAllCustomersFromOIDList(foundXFMRList, foundPriMeterList, foundDCRectifierList);
                    circuitSourceRecord.TotalCustomers = iTotalCount;

                    double dWinterkVA = 0.0;
                    double dSummerkVA = 0.0;
                    procXFROIDS(foundXFMRList, ref dWinterkVA, ref dSummerkVA);

                    //182821103
                    //182742102

                    circuitSourceRecord.WinterKVA = dWinterkVA;
                    circuitSourceRecord.SummerKVA = dSummerkVA;


                }
            }
        }


        public ScadaIndicator IsScada(CircuitSourceRecord circuitSourceRecord)
        {
            ITable iSubTable = null;
            ICursor spCursor = null;
            IQueryFilter iQf = null;
            IRow rowToGet = null;
            ScadaIndicator returnScada = ScadaIndicator.Unknown;
            try
            {
                string sCircuitID = (string)circuitSourceRecord.GetFieldValue("CIRCUITID");
                iSubTable = GetTable(_PhysNameSUBINTERUPTINGDEVICE, _EderSubDatabaseConnection);
                int iSCADAIDX = iSubTable.FindField(_FieldName_tbSUBINT_SCADAIDC);

                Dictionary<string, NetworkSourceType> dicCircuits = new Dictionary<string, NetworkSourceType>();
                dicCircuits.Add(sCircuitID, NetworkSourceType.EDER);
                GetParentChildCircuitsForCircuitID(sCircuitID, NetworkSourceType.EDER, ref dicCircuits, TraceType.PARENT, false);
                dicCircuits.Remove(sCircuitID);

                foreach (string sCID in dicCircuits.Keys)
                {
                    if (dicCircuits[sCID] == NetworkSourceType.SUBSTATION)
                    {
                        iQf = new QueryFilter();
                        iQf.WhereClause = "CIRCUITID = '" + sCID + "'";
                        spCursor = iSubTable.Search(iQf, false);

                        while ((rowToGet = spCursor.NextRow()) != null)
                        {
                            object valField = null;
                            valField = rowToGet.get_Value(iSCADAIDX);
                            if (!DBNull.Value.Equals(valField))
                            {
                                switch (valField.ToString())
                                {
                                    case "":
                                        {
                                            returnScada = ScadaIndicator.No;
                                        }
                                        break;
                                    case "Y":
                                        {
                                            returnScada = ScadaIndicator.Yes;
                                        }
                                        break;
                                    case "N":
                                        {
                                            returnScada = ScadaIndicator.No;
                                        }
                                        break;
                                }
                                valField = null;
                            }
                            if (rowToGet != null) { Marshal.FinalReleaseComObject(rowToGet); rowToGet = null; }
                        }
                        if (iQf != null) { Marshal.FinalReleaseComObject(iQf); iQf = null; }
                        if (spCursor != null) { Marshal.FinalReleaseComObject(spCursor); spCursor = null; }
                    }
                }
                if (iSubTable != null) { Marshal.FinalReleaseComObject(iSubTable); iSubTable = null; }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                if (rowToGet != null) { Marshal.FinalReleaseComObject(rowToGet); rowToGet = null; }
                if (iQf != null) { Marshal.FinalReleaseComObject(iQf); iQf = null; }
                if (spCursor != null) { Marshal.FinalReleaseComObject(spCursor); spCursor = null; }
                if (iSubTable != null) { Marshal.FinalReleaseComObject(iSubTable); iSubTable = null; }
            }
            return returnScada;
        }

        #endregion


        #region PCP GUID PROCESSING


        /// <summary>
        /// CLEAR AND RECALCULATE THE PCP GUID VALUES FOR THE CIRCUIT BEING PROCESSED
        /// </summary>
        /// <param name="PCPGUID">THE PCP GUID BEING PROCESSED</param>
        /// 

        public void processPCPbyPCPGUID(string PCPGUID)
        {
            _XFR_RegistrationID = getRegID("TRANSFORMER");
            _PRIMETER_RegistrationID = getRegID("PRIMARYMETER");

            PCPRecord PCPRec = FindPCPByGlobalID(PCPGUID, false, false);
            ROBCRecord ROBCRec = FindPCPROBC(PCPRec, false, false);

            string circuitSourceGUID = (string)ROBCRec.GetFieldValue("CIRCUITSOURCEGUID");
            CircuitSourceRecord circuitSourceRec = FindCircuitByGUID_noLoadingorScada(circuitSourceGUID);
            string circuitID = (string)circuitSourceRec.GetFieldValue("CIRCUITID");                        // GET THE CIRCUITID FROM THE PCP GUID

            clearCircuitPCP(circuitID);                                                                     // CLEAR THE PCP GUID ATTRIBUTES FOR THE CIRCUIT
            calcPCPGUIDforCircuit(circuitID);                                                               // CALCULATE THE UPDATED PCP GUIDS

        }


        /// <summary>
        /// SET ALL THE CIRCUIT'S PCPs GUID TO NULL 
        /// </summary>
        /// <param name="wSpace">THE CURRENT GIS WORKSPACE</param>
        /// <param name="circuitID"> THE CIRCUIT ID TO PROCESS</param>
        /// 

        public void clearCircuitPCP(string circuitID)
        {
            IWorkspace wSpace = _EderDatabaseConnection;

            Dictionary<int, ITable> tableList = new Dictionary<int, ITable>();
            tableList = getTablesforPCPGUID(wSpace);                                        // GET THE LIST OF TABLES CONTAINING THE PCP GUID FIELD

            ITable table;

            try
            {
                foreach (int classID in tableList.Keys)
                {
                    table = tableList[classID];
                    IDataset dSet = (IDataset)table;

                    IEnumBSTR eFieldNames = ModelNameManager.Instance.FieldNamesFromModelName((IObjectClass)table, _PCPGUIDModelName);
                    eFieldNames.Reset();                                                        // GET THE CLASSES PCP GUID FIELD NAME BY FIELD MODEL NAME

                    string fieldName = eFieldNames.Next();

                    string wClause = "UPDATE " + dSet.Name + " SET " + fieldName + " = NULL WHERE CIRCUITID = '" + circuitID + "'";
                    wSpace.ExecuteSQL(wClause);                                                 // SET THE CLASS PCP GUID FIELD TO NULL

                    wClause = "UPDATE EDGIS.a" + classID + " SET " + fieldName + " = NULL  WHERE CIRCUITID = '" + circuitID + "'";
                    wSpace.ExecuteSQL(wClause);                                                 // SET THE CORRESPONDING DELTA (ADD) TABLE  PCP GUID FIELD TO NULL
                }
            }
            catch (Exception EX)
            {
                throw new Exception("Error Clearing Circuit's PCPs... " + circuitID + " Exception: " + EX.Message + " " + EX.StackTrace);
            }
        }


        /// <summary>
        /// GET A LIST OF TABLES BY OBJECT MODEL NAME
        /// </summary>
        /// <param name="wSpace">THE GIS WORKSPACE TO USE FOR PROCESSING</param>
        /// <param name="PCPGUIDModelName">THE OBJECT MODEL NAME</param>
        /// <returns>A LIST OF TABLES CONTAINING THE PROVIDED OBJECT MODEL NAME</returns>
        /// 

        private Dictionary<int, ITable> getTablesforPCPGUID(IWorkspace wSpace)
        {
            Dictionary<int, ITable> tableList = new Dictionary<int, ITable>();
            ITable table = GetTable("EDGIS.TRANSFORMER", wSpace);
            tableList.Add(_XFR_RegistrationID, table);

            table = GetTable("EDGIS.PRIMARYMETER", wSpace);
            tableList.Add(_PRIMETER_RegistrationID, table);

            return tableList;
        }


        private int getRegID(string tablename)
        {
            ICursor cursor = null;
            IQueryFilter qFilter = null;

            try
            {
                IFeatureWorkspace fWSpace = (IFeatureWorkspace)_EderDatabaseConnection;
                ITable regTable = fWSpace.OpenTable("SDE.TABLE_REGISTRY");

                qFilter = new QueryFilterClass();
                qFilter.WhereClause = "TABLE_NAME = '" + tablename + "'";

                cursor = regTable.Search(qFilter, false);

                IRow row = cursor.NextRow();
                int regID = Convert.ToInt16(row.get_Value(row.Fields.FindField("REGISTRATION_ID")));

                return regID;

            }
            finally
            {
                if (cursor != null) { Marshal.ReleaseComObject(cursor); }
                if (qFilter != null) { Marshal.ReleaseComObject(qFilter); }
            }

        }

        /// <summary>
        /// CALCULATE THE PCP GUID VALUES FOR A PROVIDED CIRCUIT
        /// </summary>
        /// <param name="circuitID">THE CIRCUIT ID TO PROCESS</param>
        /// 

        private void calcPCPGUIDforCircuit(string circuitID)
        {
            List<string> PCPList = getPCPForCircuit(circuitID);                                     // GET A LIST OF PCPs ON THE CIRCUIT

            Dictionary<string, List<int>> collectXFRTrace = new Dictionary<string, List<int>>();
            Dictionary<string, List<int>> collectPriMeterTrace = new Dictionary<string, List<int>>();
            traceCircuitPCPs(PCPList, ref collectXFRTrace, ref collectPriMeterTrace);               // TRACE THE PCPs FOR THE CIRCUIT TO GET THEIR DOWN STREAM XFRs AND PRIMARY METERS

            Dictionary<string, List<int>> resultsXFRTrace = new Dictionary<string, List<int>>();
            Dictionary<string, List<int>> resultsPriMeterTrace = new Dictionary<string, List<int>>();
            resultsXFRTrace = procTraceResults(collectXFRTrace);                                    // PROCESS THE TRACE RESULTS - ASSIGN PCP GUIDs TO FEATURES
            resultsPriMeterTrace = procTraceResults(collectPriMeterTrace);

            updatePCPGuids("EDGIS.TRANSFORMER", _XFR_RegistrationID, resultsXFRTrace);                                   // UPDATE THE FEATURE ATTRIBUTES WITH THE PCP GUID VALUE
            updatePCPGuids("EDGIS.PRIMARYMETER", _PRIMETER_RegistrationID, resultsPriMeterTrace);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="assignedPCPs"></param>
        /// 

        private void updatePCPGuids(string tablename, int regID, Dictionary<string, List<int>> assignedPCPs)
        {
            int counter = 0;
            string PCPGUIDOIDS = string.Empty;

            foreach (KeyValuePair<string, List<int>> PCPGUIDItems in assignedPCPs)
            {
                foreach (int PCPGUIDValues in PCPGUIDItems.Value)
                {
                    if (counter == 0)
                    {
                        PCPGUIDOIDS = Convert.ToString(PCPGUIDValues);
                        counter++;
                    }
                    else
                    {
                        PCPGUIDOIDS += ", " + Convert.ToString(PCPGUIDValues);
                        counter++;

                        if (counter == 200)
                        {
                            PCPGUIDOIDS = "(" + PCPGUIDOIDS + ")";

                            procPCPGUIDs(regID, tablename, PCPGUIDItems.Key, PCPGUIDOIDS);                       // PROCESS EACH XFR FOR THE PCP GUID
                            counter = 0;
                        }
                    }
                }

                if (PCPGUIDOIDS.Length > 0)
                {
                    PCPGUIDOIDS = "(" + PCPGUIDOIDS + ")";
                    procPCPGUIDs(regID, tablename, PCPGUIDItems.Key, PCPGUIDOIDS);
                    counter = 0;
                }
            }
        }


        /// <summary>
        /// UPDATE THE XFR/PRIMARY PCPGUID ATTRIBUTE WITH THE CALCULATED PCP GUID VALUE
        /// </summary>
        /// <param name="PCPGUID">THE PCP GUID VALUE</param>
        /// <param name="PCPDeviceOID">THE DEVICE OBJECTID TO BE UPDATED</param>
        /// 

        private void procPCPGUIDs(int regID, string tablename, string PCPGUID, string PCPDeviceOID)
        {
            string sql = "UPDATE " + tablename + " SET PCPGUID = '" + PCPGUID + "' WHERE OBJECTID in " + PCPDeviceOID;
            _EderDatabaseConnection.ExecuteSQL(sql);

            sql = "UPDATE EDGIS.a" + regID + " SET PCPGUID = '" + PCPGUID + "' WHERE OBJECTID in " + PCPDeviceOID;
            _EderDatabaseConnection.ExecuteSQL(sql);
        }





        /// <summary>
        /// GET A LIST OF PCPs ON A PROVIDED CIRCUIT
        /// </summary>
        /// <param name="circuitID">THE CIRCUIT BEING PROCESSED</param>
        /// <returns>A LIST OF PCPs ON THE PROVIDED CIRCUIT</returns>
        /// 

        public List<string> getPCPForCircuit(string circuitID)
        {
            ICursor cursor = null;
            IQueryFilter qFilter = null;

            try
            {
                List<string> PCPForCircuit = new List<string>();
                string PCPGUID = string.Empty;

                CircuitSourceRecord cirSourceRec = FindCircuit(circuitID, false, false);
                if (cirSourceRec == null) { return null; }
                string circuitSourceGUID = (string)cirSourceRec.GlobalID;

                ITable ROBCTable = GetTable("EDGIS.ROBC", _EderDatabaseConnection);
                ITable PCPTable = GetTable("EDGIS.PARTIALCURTAILPOINT", _EderDatabaseConnection);

                qFilter = new QueryFilterClass();
                qFilter.WhereClause = "CIRCUITSOURCEGUID = '" + circuitSourceGUID + "'";                            // GET THE CIRCUITSOURCE GUID

                cursor = ROBCTable.Search(qFilter, false);

                IRow row = cursor.NextRow();
                while (row != null)
                {
                    if (!DBNull.Value.Equals(row.get_Value(row.Fields.FindField("PARTCURTAILPOINTGUID"))))          // GET AND CACHE THE PCP GUID
                    {
                        PCPGUID = (string)row.get_Value(row.Fields.FindField("PARTCURTAILPOINTGUID"));
                        string devicePCPGUID = getPCPDeviceGUID(PCPTable, PCPGUID);

                        if (!DBNull.Value.Equals(devicePCPGUID)) { if (!PCPForCircuit.Contains(devicePCPGUID)) { PCPForCircuit.Add(devicePCPGUID); } }
                    }

                    row = cursor.NextRow();
                }

                return PCPForCircuit;
            }
            finally
            {
                if (cursor != null) { Marshal.ReleaseComObject(cursor); }
                if (qFilter != null) { Marshal.ReleaseComObject(qFilter); }
            }
        }


        private string getPCPDeviceGUID(ITable pcpTable, string PCPGUID)
        {
            ICursor cursor = null;
            IQueryFilter qFilter = null;

            try
            {
                string devicePCPGUID = null;

                qFilter = new QueryFilterClass();
                qFilter.WhereClause = "GLOBALID = '" + PCPGUID + "'";

                cursor = pcpTable.Search(qFilter, false);
                IRow row = cursor.NextRow();

                if (row != null) { devicePCPGUID = (string)row.get_Value(row.Fields.FindField("DEVICEGUID")); }

                return devicePCPGUID;
            }
            finally
            {
                if (cursor != null) { Marshal.ReleaseComObject(cursor); }
                if (qFilter != null) { Marshal.ReleaseComObject(qFilter); }
            }
        }


        /// <summary>
        /// TRACE THE PROVIDED LIST OF PCPs TO GET THEIR DOWNSTREAM XFRs AND PRIMARY METERs
        /// </summary>
        /// <param name="circuitPCPList">THE LIST OF PCPs FOR THIS CIRCUIT</param>
        /// 

        private void traceCircuitPCPs(List<string> circuitPCPList, ref Dictionary<string, List<int>> collectXFRTrace, ref Dictionary<string, List<int>> collectPriMeterTrace)
        {
            for (int i = 0; i < circuitPCPList.Count; i++)                                  // TRACE EACH PCP IN THE LIST
            {
                string PCPDeviceGUID = circuitPCPList[i];
                string PCPDeviceGUIDClassName = GetTableNameOfDevice(PCPDeviceGUID);

                List<int> XFMRList = new List<int>();
                List<int> PriMeterList = new List<int>();
                List<int> DCRectifierList = new List<int>();
                List<int> LoadSource = new List<int>();

                IEnumNetEID eJunctions = null;
                IEnumNetEID eEdges = null;

                List<int> junctionEIDs = new List<int>();

                ArcFMDwnstreamTraceFromFeature(_EderGeomNetwork, PCPDeviceGUIDClassName, PCPDeviceGUID, ref eJunctions, ref eEdges);

                eJunctions.Reset();
                int junctionEID = -1;
                while ((junctionEID = eJunctions.Next()) > 0)
                {
                    if (!junctionEIDs.Contains(junctionEID))
                    {
                        junctionEIDs.Add(junctionEID);
                    }
                }

                FilterJunctions(junctionEIDs, ref XFMRList, ref PriMeterList, ref DCRectifierList, ref LoadSource);

                collectXFRTrace.Add(PCPDeviceGUID, XFMRList);
                collectPriMeterTrace.Add(PCPDeviceGUID, PriMeterList);
            }
        }


        /// <summary>
        /// PROCESS THE PCP TRACE RESULTS TO ASSIGN TRANSFORMERS AND SERVICE POINTS TO THEIR PCPs
        /// </summary>
        /// <param name="PCPDeviceGUID">THE GUID OF THE PCP DEVICE BEING PROCESSED</param>
        /// <param name="XFMRList">THE DOWNSTREAM LIST OF XFRs</param>
        /// <param name="PriMeterList">THE DOWNSTREAM LIST OF PRIMARY METERS</param>
        /// 

        private Dictionary<string, List<int>> procTraceResults(Dictionary<string, List<int>> featList)
        {
            Dictionary<string, int> testValues = new Dictionary<string, int>();

            foreach (string guid in featList.Keys)                                                           // PROCESS THE XFRs TRACED FROM THE CIRCUIT
            {
                testValues.Add(guid, featList[guid].Count);
            }

            Dictionary<string, int> sortedDict = new Dictionary<string, int>();

            foreach (KeyValuePair<string, int> dataSort in testValues.OrderBy(key => key.Value))            // SORT THE TRACE RESULTS BY THE NUMBER OF DOWNSTREAM XFRs
            {
                sortedDict.Add(dataSort.Key, dataSort.Value);
            }

            Dictionary<string, int> tracedPCP = new Dictionary<string, int>();

            List<int> OIDList = new List<int>();
            List<int> testContents = new List<int>();
            List<int> returnList = new List<int>();
            Dictionary<string, List<int>> returnDict = new Dictionary<string, List<int>>();

            foreach (string sortedGUIDs in sortedDict.Keys)                                                 // ASSIGN THE FEATURES TO A PCP GUID AND ENSURE A XFR IS ONLY ASSIGNED TO A SINGLE PCP
            {
                returnList.Clear();

                OIDList = featList[sortedGUIDs];
                for (int i = 0; i < OIDList.Count; i++)
                {
                    if (!testContents.Contains(OIDList[i]))
                    {
                        testContents.Add(OIDList[i]);
                        returnList.Add(OIDList[i]);
                    }
                }

                returnDict.Add(sortedGUIDs, returnList);
            }

            return returnDict;
        }




        #endregion

        #region CircuitSource

        private void getOIDsFromEIDs(List<int> JuncEIDs, string sFeatureClass, ref List<int> JuncOIDs)
        {
            int iMaxEIDsSQL = 20;
            int iLoop = -1;
            string sPrefix = string.Empty;

            string whereClauseOIDList = "";

            List<string> qfWhereClauses = new List<string>();

            if (sFeatureClass.ToUpper() == _PhysNameTransformer.ToUpper())
            {
                sPrefix = "USERCLASSID in (select OBJECTID from SDE.GDB_ITEMS where PHYSICALNAME = '" + _PhysNameTransformer.ToUpper() + "') and EID in (";
            }
            else if (sFeatureClass.ToUpper() == _PhysNameServiceLocation.ToUpper())
            {
                sPrefix = "USERCLASSID in (select OBJECTID from SDE.GDB_ITEMS where PHYSICALNAME = '" + _PhysNameServiceLocation.ToUpper() + "') and EID in (";
            }

            string sSuffix = " )";

            foreach (int i in JuncEIDs)
            {
                iLoop++;
                if (iLoop == 0)
                {
                    whereClauseOIDList = i.ToString();
                }
                else if (0 < iLoop && iLoop < iMaxEIDsSQL && iLoop != (JuncEIDs.Count - 1))
                {
                    whereClauseOIDList = whereClauseOIDList + "," + i.ToString();
                }
                else if ((iLoop == iMaxEIDsSQL) || (iLoop == (JuncEIDs.Count - 1)))
                {
                    iLoop = -1;
                    whereClauseOIDList = whereClauseOIDList + "," + i.ToString();
                    qfWhereClauses.Add(sPrefix + whereClauseOIDList + sSuffix);
                    whereClauseOIDList = "";
                }
            }

            if (whereClauseOIDList != "")
            {
                qfWhereClauses.Add(sPrefix + whereClauseOIDList + sSuffix);
                whereClauseOIDList = "";
            }

            if (qfWhereClauses.Count > 0)
            {
                ITable tabNetTable = null;
                IQueryFilter iQf = null;
                ICursor spCursor = null;
                IRow rowToGet = null;
                try
                {
                    IFeatureWorkspace fwEDER = (IFeatureWorkspace)_EderDatabaseConnection;
                    tabNetTable = fwEDER.OpenTable(_TableNameEDNetwork);
                    int iEIDColumn = tabNetTable.FindField("EID");
                    int iOIDColumn = tabNetTable.FindField("OID");
                    int iUserClassIDColumn = tabNetTable.FindField("USERID");
                    //
                    List<int> JuncEIDsList = new List<int>();
                    string sPrefixWClause = string.Empty;



                    foreach (string searchClause in qfWhereClauses)
                    {
                        iQf = new QueryFilter();
                        // iQf.SubFields = "OID,EID,USERCLASSID";
                        iQf.WhereClause = searchClause;
                        spCursor = tabNetTable.Search(iQf, false);

                        while ((rowToGet = spCursor.NextRow()) != null)
                        {
                            object valField = rowToGet.get_Value(iUserClassIDColumn);
                            // int tempInt = Convert.ToInt32(valField.ToString());
                            int tempInt = Convert.ToInt32(rowToGet.get_Value(iUserClassIDColumn));
                            JuncEIDsList.Add(tempInt);
                            Marshal.FinalReleaseComObject(rowToGet);
                            rowToGet = null;
                        }

                        Marshal.FinalReleaseComObject(iQf);
                        iQf = null;
                        Marshal.FinalReleaseComObject(spCursor);
                        spCursor = null;
                    }

                    JuncOIDs = JuncEIDsList;
                }
                catch (Exception EX)
                {

                }

            }
        }

        private void procXFROIDS(List<int> XFRJuncOIDs, ref double dWinterkVA, ref double dSummerkVA)
        {
            int iMaxEIDsSQL = 995;
            ITable xfrTable = null;
            IQueryFilter iQf = null;
            ICursor spCursor = null;
            IRow rowToGet = null;
            List<string> qfWhereClauses = new List<string>();
            string whereClauseOIDList = "";
            string sPrefix = " OBJECTID IN (";
            string sSuffix = " )";
            try
            {
                int iLoop = -1;
                foreach (int i in XFRJuncOIDs)
                {
                    iLoop++;
                    if (iLoop == 0)
                    {
                        whereClauseOIDList = i.ToString();
                    }
                    else if (0 < iLoop && iLoop < iMaxEIDsSQL && iLoop != (XFRJuncOIDs.Count - 1))
                    {
                        whereClauseOIDList = whereClauseOIDList + "," + i.ToString();
                    }
                    else if ((iLoop == iMaxEIDsSQL) || (iLoop == (XFRJuncOIDs.Count - 1)))
                    {
                        iLoop = -1;
                        whereClauseOIDList = whereClauseOIDList + "," + i.ToString();
                        qfWhereClauses.Add(sPrefix + whereClauseOIDList + sSuffix);
                        whereClauseOIDList = "";
                    }
                }
                if (whereClauseOIDList != "")
                {
                    qfWhereClauses.Add(sPrefix + whereClauseOIDList + sSuffix);
                    whereClauseOIDList = "";
                }

                if (qfWhereClauses.Count > 0)
                {
                    xfrTable = GetTable(_PhysNameTransformer, _EderDatabaseConnection);
                    int iWinterIDX = xfrTable.FindField(_FieldName_tbXFMR_WINTERKVA);
                    int iSummerIDX = xfrTable.FindField(_FieldName_tbXFMR_SUMMERKVA);
                    foreach (string searchClause in qfWhereClauses)
                    {
                        iQf = new QueryFilter();
                        iQf.SubFields = "OBJECTID," + _FieldName_tbXFMR_WINTERKVA + "," + _FieldName_tbXFMR_SUMMERKVA + "";
                        iQf.WhereClause = searchClause;
                        spCursor = xfrTable.Search(iQf, false);

                        while ((rowToGet = spCursor.NextRow()) != null)
                        {
                            if (!DBNull.Value.Equals(rowToGet.get_Value(iWinterIDX))) { dWinterkVA += (double)rowToGet.get_Value(iWinterIDX); }
                            if (!DBNull.Value.Equals(rowToGet.get_Value(iSummerIDX))) { dSummerkVA += (double)rowToGet.get_Value(iSummerIDX); }
                            if (rowToGet != null) { Marshal.FinalReleaseComObject(rowToGet); rowToGet = null; }
                        }
                        if (iQf != null) { Marshal.FinalReleaseComObject(iQf); iQf = null; }
                        if (spCursor != null) { Marshal.FinalReleaseComObject(spCursor); spCursor = null; }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                if (iQf != null) { Marshal.FinalReleaseComObject(iQf); iQf = null; }
                if (spCursor != null) { Marshal.FinalReleaseComObject(spCursor); spCursor = null; }
                if (rowToGet != null) { Marshal.FinalReleaseComObject(rowToGet); rowToGet = null; }
                if (xfrTable != null) { Marshal.FinalReleaseComObject(xfrTable); xfrTable = null; }
            }

        }

        private void procSPOIDs(List<int> SPJuncOIDs, ref int iAGR, ref int iCOM, ref int iDOM, ref int iIND, ref int iOTH)
        {
            int iSPOID = 0;

            IFeatureWorkspace pFWSpace = (IFeatureWorkspace)_EderDatabaseConnection;
            IFeatureClass pSPFClass = pFWSpace.OpenFeatureClass("EDGIS.SERVICELOCATION");
            ITable pSPointTable = pFWSpace.OpenTable("EDGIS.ZZ_MV_SERVICEPOINT");

            int iSLocGUISIDX = pSPFClass.FindField("GLOBALID");
            int iCustType = pSPointTable.FindField("CUSTOMERTYPE");

            string sGUID;
            string sCustType;

            IFeature pFeat;

            IQueryFilter pQFilter = null;
            ICursor pCursor = null;

            IRow pRow = null;

            try
            {
                for (int i = 0; i < SPJuncOIDs.Count; i++)
                {
                    iSPOID = SPJuncOIDs[i];
                    pFeat = pSPFClass.GetFeature(iSPOID);
                    sGUID = (String)pFeat.get_Value(iSLocGUISIDX);


                    if (pFeat != null)
                    {
                        pQFilter = new QueryFilterClass();
                        pQFilter.WhereClause = "SERVICELOCATIONGUID = '" + sGUID + "'";

                        pCursor = pSPointTable.Search(pQFilter, false);

                        pRow = pCursor.NextRow();
                        while (pRow != null)
                        {
                            if (!DBNull.Value.Equals(pRow.get_Value(iCustType)))
                            {
                                sCustType = (string)pRow.get_Value(iCustType);

                                if (sCustType.ToString() == "AGR") { iAGR++; }
                                else if (sCustType.ToUpper() == "COM") { iCOM++; }
                                else if (sCustType.ToUpper() == "DOM") { iDOM++; }
                                else if (sCustType.ToUpper() == "IND") { iIND++; }
                                else if (sCustType.ToUpper() == "OTH") { iOTH++; }
                            }

                            pRow = pCursor.NextRow();
                        }

                        if (pRow != null) { Marshal.ReleaseComObject(pRow); }
                        if (pQFilter != null) { Marshal.ReleaseComObject(pQFilter); }
                        if (pCursor != null) { Marshal.ReleaseComObject(pCursor); }
                    }
                }

            }
            catch (Exception EX)
            {

                throw;
            }
            finally
            {
                if (pRow != null) { Marshal.ReleaseComObject(pRow); }
                if (pQFilter != null) { Marshal.ReleaseComObject(pQFilter); }
                if (pCursor != null) { Marshal.ReleaseComObject(pCursor); }
            }



        }

        private void filterLoadJunctions(List<int> iList, ref List<int> XFRJuncEIDs, ref List<int> SPJuncEIDs)
        {
            int iMaxEIDsSQL = 995;
            int iLoop = -1;

            string whereClauseOIDList = "";

            List<string> qfWhereClauses = new List<string>();

            string sXFRPrefix = " USERCLASSID in (select objectid from " + _TableNameGDBITEMS + " where physicalname = 'EDGIS.TRANSFORMER') and EID in (";

            string sSPPrefix = " USERCLASSID in (select objectid from " + _TableNameGDBITEMS + " where physicalname = 'EDGIS.SERVICELOCATION') and EID in (";

            string sSuffix = " ) ";

            foreach (int i in iList)
            {
                iLoop++;
                if (iLoop == 0)
                {
                    whereClauseOIDList = i.ToString();
                }
                else if (0 < iLoop && iLoop < iMaxEIDsSQL)
                {
                    whereClauseOIDList = whereClauseOIDList + "," + i.ToString();
                }
                else if (iLoop == iMaxEIDsSQL)
                {
                    iLoop = -1;
                    whereClauseOIDList = whereClauseOIDList + "," + i.ToString();
                    qfWhereClauses.Add(whereClauseOIDList + sSuffix);
                    whereClauseOIDList = "";
                }
            }

            if (whereClauseOIDList != "")
            {
                qfWhereClauses.Add(whereClauseOIDList + sSuffix);
                whereClauseOIDList = "";
            }

            if (qfWhereClauses.Count > 0)
            {
                ITable tabNetTable = null;
                IQueryFilter iQf = null;
                ICursor spCursor = null;
                IRow rowToGet = null;
                try
                {
                    IFeatureWorkspace fwEDER = (IFeatureWorkspace)_EderDatabaseConnection;
                    tabNetTable = fwEDER.OpenTable(_TableNameEDNetwork);
                    int iEIDColumn = tabNetTable.FindField("EID");
                    int iOIDColumn = tabNetTable.FindField("OID");
                    int iUserClassIDColumn = tabNetTable.FindField("USERCLASSID");

                    List<int> JuncEIDsList = new List<int>();
                    string sPrefixWClause = string.Empty;

                    for (int i = 0; i < 2; i++)
                    {
                        if (i == 0) { sPrefixWClause = sXFRPrefix; }
                        if (i == 1) { sPrefixWClause = sSPPrefix; }

                        foreach (string searchClause in qfWhereClauses)
                        {
                            iQf = new QueryFilter();
                            iQf.SubFields = "OID,EID,USERCLASSID";
                            iQf.WhereClause = sPrefixWClause + searchClause;
                            spCursor = tabNetTable.Search(iQf, false);

                            while ((rowToGet = spCursor.NextRow()) != null)
                            {
                                object valField = rowToGet.get_Value(iOIDColumn);
                                int tempInt = Convert.ToInt32(valField.ToString());
                                JuncEIDsList.Add(tempInt);
                                Marshal.FinalReleaseComObject(rowToGet);
                                rowToGet = null;
                            }

                            Marshal.FinalReleaseComObject(iQf);
                            iQf = null;
                            Marshal.FinalReleaseComObject(spCursor);
                            spCursor = null;
                        }

                        if (i == 0) { XFRJuncEIDs = JuncEIDsList; }
                        if (i == 1) { SPJuncEIDs = JuncEIDsList; }

                        JuncEIDsList = new List<int>();
                    }
                }
                catch
                    (Exception EX)
                {

                }
                finally
                {
                    if (tabNetTable != null)
                    {
                        Marshal.FinalReleaseComObject(tabNetTable);
                        tabNetTable = null;
                    }

                    if (iQf != null)
                    {
                        Marshal.FinalReleaseComObject(iQf);
                        iQf = null;
                    }
                    if (spCursor != null)
                    {
                        Marshal.FinalReleaseComObject(spCursor);
                        spCursor = null;
                    }
                }
            }
        }
    }
        #endregion





}