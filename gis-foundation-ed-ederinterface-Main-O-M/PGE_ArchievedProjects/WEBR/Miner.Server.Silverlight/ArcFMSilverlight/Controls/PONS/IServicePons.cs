using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;



namespace ArcFMSilverlight
{
    [ServiceContract(Name = "IWCFPONSService")]
    public interface IServicePons
    {

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetDevicedataCustomerSearch(string strDevice1, string strDevice2, int Divisioncode, AsyncCallback callback, object state);
        List<Devicestartcustomerdata> EndGetDevicedataCustomerSearch(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetExportdataFile(string outagedate, string Area, string message, List<TransformercustomerdataComb> customerData, string strFileFormat, string strUserName, AsyncCallback callback, object state);
        string EndGetExportdataFile(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAverydata(List<TransformercustomerdataComb> customerData, int skips, string UserName, AsyncCallback callback, object state);
        string EndGetAverydata(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSendtoPSL(List<StagingdataCust> customerData, int plannedOutageID, string contactID, string division, string work1DTStart, string work1TMStart, string work1TMEnd, string work2DTStart, string work2TMStart, string work2TMEnd, string vicinity, string strUserName, AsyncCallback callback, object state);
        string EndSendtoPSL(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetCustomerdataComb(string strCGSValue, string strCircuitId, string strServicePointID, string strDivisionIn, string strFlagIn, AsyncCallback callback, object state);
        List<TransformercustomerdataComb> EndGetCustomerdataComb(IAsyncResult result);

        //Map Print
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetStandardPrint(List<string> Gridlist, string urlpath, string template, string size, string userid, AsyncCallback callback, object state);
        string EndGetStandardPrint(IAsyncResult result);
        //Jet
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetjetjoblist(string STATUS, string RESERVEDBY, string DIVISION, string JobNumber, int pagesize, AsyncCallback callback, object state);
        List<JetJoblist> EndGetjetjoblist(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetDeleteJob(string objectID, string userid, AsyncCallback callback, object state);
        string EndGetDeleteJob(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetEditJob(string objectID, int STATUS, string RESERVEDBY, int DIVISION, string JobNumber, string USERAUDIT, string description, DateTime ENTRYDATE, AsyncCallback callback, object state);
        string EndGetEditJob(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetEquipmentJob(string objectID, string JobNumber, AsyncCallback callback, object state);
        List<jetequipmentlist> EndGetEquipmentJob(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetDeleteEquipment(string objectID, string Opstatus, AsyncCallback callback, object state);
        List<jetequipmentlist> EndGetDeleteEquipment(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddEditJobEquipment(string objectID, int STATUS, int EQUIPTYPEID, string OPERATINGNUMBER, string City, string SketchLoc, string InstallCdName, string Address, Decimal LATITUDE, Decimal LONGITUDE, string CUSTOWNED, string JobNumber, string USERAUDIT, string Cgc12, AsyncCallback callback, object state);
        List<jetequipmentlist> EndAddEditJobEquipment(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginEditJobNumberEquipment(string oldJobNum, string newJobNum, AsyncCallback callback, object state);
        List<jetequipmentlist> EndEditJobNumberEquipment(IAsyncResult result);

        //WEBR Stability - Replace Feature Service with WCF for JET
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginValidateAddEditJob(string description, int division, DateTime entryDate, string jobNumber, DateTime lastModifiedDate, string reservedBy, string userAudit, int status, int objectId, string action, AsyncCallback callback, object state);
        bool[] EndValidateAddEditJob(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetEquipmentIDTypes(AsyncCallback callback, object state);
        List<EquipmentIdType> EndGetEquipmentIDTypes(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetInstallTypeDomain(AsyncCallback callback, object state);
        Dictionary<string, string> EndGetInstallTypeDomain(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetJetLastEquipLocation(string jobNumber, AsyncCallback callback, object state);
        jetequipmentlist EndGetJetLastEquipLocation(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetJETDivisions(AsyncCallback callback, object state);
        Dictionary<object, string> EndGetJETDivisions(IAsyncResult result);

        

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSearchJetJobEquip(string searchInput, string filterBase, AsyncCallback callback, object state);
        List<JetViewJobEquip> EndSearchJetJobEquip(IAsyncResult result);

         //Enoc
         [OperationContract(AsyncPattern = true)]
         IAsyncResult BeginGetGenFeederDetails(string FeederId, AsyncCallback callback, object state);
         List<GenFeederDetails> EndGetGenFeederDetails(IAsyncResult result);

         [OperationContract(AsyncPattern = true)]
         IAsyncResult BegingetInverterNameplateFromSettings(string sapEGINotification, AsyncCallback callback, object state);
         List<InverterNameplateDetails> EndgetInverterNameplateFromSettings(IAsyncResult result);

         /**** ENOS Tariff Change - Start ******/
         [OperationContract(AsyncPattern = true)]
         IAsyncResult BegingetDataToExportFromSettings(string sapEGINotification, AsyncCallback callback, object state);
         List<DataToExportDetails> EndgetDataToExportFromSettings(IAsyncResult result);
         /**** ENOS Tariff Change - End ******/
       

        //CIT *start
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginCIT_Process(int objectID, string globalID, string filledduct, string requested_on, string lanid, AsyncCallback callback, object state);
        bool EndCIT_Process(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginCIT_ConduitType(int objectID, AsyncCallback callback, object state);
        int EndCIT_ConduitType(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginCIT_FilledValue(int objectID, AsyncCallback callback, object state);
        int EndCIT_FilledValue(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginCIT_UpdatedonValue(int objectID, AsyncCallback callback, object state);
        DateTime? EndCIT_UpdatedonValue(IAsyncResult result);
        //CIT *end

        //Favorites start - WEBR Stability - Replace Feature Service with WCF for Favorites
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetFavoritesList(string LANID, AsyncCallback callback, object state);
        List<FavoritesData> EndGetFavoritesList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteFavorites(int objectId, string lanId, AsyncCallback callback, object state);
        FavoritesSuccessData EndDeleteFavorites(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddEditFavorites(string insertOrUpdate, string user, string name, string storedView, string layerVisibilities, AsyncCallback callback, object state);
        FavoritesSuccessData EndAddEditFavorites(IAsyncResult result);
        //Favorites end

        //Loading Information - //ME Q3 2019 Release - DA# 190501
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetLoadingInfoCustData(string cgcList, AsyncCallback callback, object state);
        List<LoadingInfoCustomerData> EndGetLoadingInfoCustData(IAsyncResult result);

        //DA# 190506 - WEBR Access Management - ME Q3 2019 Release
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginStoreUserDetails(string LANID, string machineName, AsyncCallback callback, object state);
        bool EndStoreUserDetails(IAsyncResult result);
        
    }
    [DataContract]
    public class JetJoblist
    {
        [DataMember]
        public int OBJECTID { set; get; }
        [DataMember]
        public string JOBNUMBER { set; get; }
        [DataMember]
        public string RESERVEDBY { set; get; }
        [DataMember]
        public string DIVISION { set; get; }
        [DataMember]
        public string DESCRIPTION { set; get; }
        [DataMember]
        public DateTime ENTRYDATE { set; get; }
        [DataMember]
        public DateTime LASTMODIFIEDDATE { set; get; }
        [DataMember]
        public string USERAUDIT { set; get; }
        [DataMember]
        public string STATUS { set; get; }
    }
    [DataContract]
    public class GenFeederDetails
    {
        [DataMember]
        public string SPID { get; set; }
        [DataMember]
        public string Address { get; set; }
        [DataMember]
        public string GenType { get; set; }
        [DataMember]
        public double GenSize { get; set; }
        [DataMember]
        public double Nameplate { get; set; }
        [DataMember]
        public string ProjectReference { get; set; }
        [DataMember]
        public string GENGLOBALID { get; set; }
        [DataMember]
        public string SLGUID { get; set; }
        [DataMember]
        public string TransGUID { get; set; }
        [DataMember]
        public string PMGUID { get; set; }
        [DataMember]
        public string METERNUMBER { get; set; }
        [DataMember]
        public string CGC12 { get; set; }
        [DataMember]
        public string FEEDERNUM { get; set; }

        //ENOS Tariff Change-start
        [DataMember]
        public string Derated { get; set; }
        [DataMember]
        public string LimitedExport { get; set; }
        //ENOS Tariff Change-end
    }
    [DataContract]
    public class jetequipmentlist
    {
        [DataMember]
        public int OBJECTID { set; get; }
        [DataMember]
        public string JOBNUMBER { set; get; }
        [DataMember]
        public int EQUIPTYPEID { set; get; }
        [DataMember]
        public string OperatingNumber { set; get; }
        [DataMember]
        public string Cgc12 { set; get; }

        [DataMember]
        public string SketchLoc { set; get; }
        [DataMember]
        public string InstallCdName { set; get; }
        [DataMember]
        public string Address { set; get; }
        [DataMember]
        public string City { set; get; }
        [DataMember]
        public Decimal LATITUDE { set; get; }
        [DataMember]
        public Decimal LONGITUDE { set; get; }

        [DataMember]
        public DateTime ENTRYDATE { set; get; }
        [DataMember]
        public DateTime LastModifiedDateLocal { set; get; }
        [DataMember]
        public string USERAUDIT { set; get; }
        [DataMember]
        public string CUSTOWNED { set; get; }
        [DataMember]
        public string STATUS { set; get; }
    }
    [DataContract]
    public class StagingdataCust
    {
        [DataMember]
        public string SubmitUser { set; get; }
        [DataMember]
        public string SubmitDT { set; get; }
        [DataMember]
        public string SubmitTM { set; get; }
        [DataMember]
        public string Vicinity { set; get; }
        [DataMember]
        public string AddrLine1 { set; get; }
        [DataMember]
        public string AddrLine2 { set; get; }
        [DataMember]
        public string AddrLine3 { set; get; }
        [DataMember]
        public string AddrLine4 { set; get; }
        [DataMember]
        public string AddrLine5 { set; get; }
        [DataMember]
        public string CGCTNum { set; get; }
        [DataMember]
        public string SSD { set; get; }
        [DataMember]
        public string Phone { set; get; }
        [DataMember]
        public string Work1DT { set; get; }
        [DataMember]
        public string Work1TMStart { set; get; }
        [DataMember]
        public string Work1TMEnd { set; get; }
        [DataMember]
        public string Work2DT { set; get; }
        [DataMember]
        public string Work2TMStart { set; get; }
        [DataMember]
        public string Work2TMEnd { set; get; }
        [DataMember]
        public string AccountID { set; get; }
        [DataMember]
        public int OutageID { set; get; }
        [DataMember]
        public string ContactID { set; get; }
        [DataMember]
        public string SPID { set; get; }
        [DataMember]
        public string SPAddrLine1 { set; get; }
        [DataMember]
        public string SPAddrLine2 { set; get; }
        [DataMember]
        public string SPAddrLine3 { set; get; }
        [DataMember]
        public string CustType { set; get; }
        [DataMember]
        public string meter_number { set; get; }
        [DataMember]
        public int division { set; get; }
    }

    [DataContract]
    public class ExportFileName
    {
        public string ExportedFileName { set; get; }
    }

    [DataContract]
    public class StandardMapPrintGridList
    {
        [DataMember]
        public string Gridlist { set; get; }
    }
    [DataContract]
    public class Devicestartcustomerdata
    {
        [DataMember]
        public string DeviceName { set; get; }
        [DataMember]
        public string OPERATINGNUMBER { set; get; }
        [DataMember]
        public string objectid { set; get; }
        [DataMember]
        public string globalid { set; get; }
        [DataMember]
        public string circuitid { set; get; }
        [DataMember]
        public string circuitid2 { set; get; }
    }
    [DataContract]
    public class TransformercustomerdataComb
    {
        [DataMember]
        public string GridADDCol { set; get; }
        [DataMember]
        public string CustomerName { set; get; }
        [DataMember]
        public string AverySTNameNumber { set; get; }
        [DataMember]
        public string AddressCust { set; get; }
        [DataMember]
        public string CustType { set; get; }
        [DataMember]
        public string MeterNum { set; get; }
        [DataMember]
        public string ServicepointId { set; get; }
        [DataMember]
        public string CustMail1 { set; get; }
        [DataMember]
        public string CustMail2 { set; get; }
        [DataMember]
        public string SerStNumber { set; get; }
        [DataMember]
        public string SerStName1 { set; get; }
        [DataMember]
        public string SerStName2 { set; get; }
        [DataMember]
        public string CustMailStNum { set; get; }
        [DataMember]
        public string CustMailStName1 { set; get; }
        [DataMember]
        public string CustMailStName2 { set; get; }
        [DataMember]
        public string ServiceZip { set; get; }
        [DataMember]
        public string CustMailZipCode { set; get; }
        [DataMember]
        public string SerCity { set; get; }
        [DataMember]
        public string CustMailCity { set; get; }
        [DataMember]
        public string SerState { set; get; }
        [DataMember]
        public string MailState { set; get; }
        [DataMember]
        public string CGC12 { set; get; }
        [DataMember]
        public string TransOperatingNum { set; get; }
        [DataMember]
        public string PMeterOperatingNum { set; get; }
        [DataMember]
        public string TransSSD { set; get; }
        [DataMember]
        public string CustAreaCode { set; get; }
        [DataMember]
        public string CustPhone { set; get; }
        [DataMember]
        public string SerDivision { set; get; }
        [DataMember]
        public string SerActID { set; get; }
        [DataMember]
        public int ORD { set; get; }
        [DataMember]
        public string PREMISEID { set; get; }
        [DataMember]
        public string PREMISETYPE { set; get; }

    }
    [DataContract]
    public class ShutDownID
    {
        [DataMember]
        public string SDownID { set; get; }
    }
    [DataContract]
    public class PSLData
    {
        [DataMember]
        public string FName { set; get; }
        [DataMember]
        public string LName { set; get; }
        [DataMember]
        public string LanID { set; get; }
        [DataMember]
        public string Division { set; get; }
        [DataMember]
        public string ContactID { set; get; }

    }
    [DataContract]
    public class SubStationSearchData
    {
        [DataMember]
        public string SubStationName { set; get; }
        [DataMember]
        public string SubStationID { set; get; }
        [DataMember]
        public string BANK { set; get; }
        [DataMember]
        public string CircuitID { set; get; }
    }

    /*****ENOS2SAP PhaseII Changes*******/
    [DataContract]
    public class InverterNameplateDetails
    {
        [DataMember]
        public string ProjectReference { get; set; }
        [DataMember]
        public double GenSize { get; set; }

    }

    /**** ENOS Tariff Change - Start ******/
    [DataContract]
    public class DataToExportDetails
    {
        [DataMember]
        public string SPID { get; set; }
        [DataMember]
        public string Address { get; set; }


        [DataMember]
        public double GenSize { get; set; }
        [DataMember]
        public double Nameplate { get; set; }
        [DataMember]
        public string ProjectReference { get; set; }

        [DataMember]
        public string METERNUMBER { get; set; }
        [DataMember]
        public string CGC12 { get; set; }
        [DataMember]
        public string FEEDERNUM { get; set; }

        [DataMember]
        public string GenType { get; set; }
        [DataMember]
        public string TechType { get; set; }
        [DataMember]
        public string EquipmentType { get; set; }
        [DataMember]
        public string SAPEquipmentID { set; get; }
        [DataMember]
        public string EquipSAPEquipmentID { set; get; }

        [DataMember]
        public string Derated { get; set; }
        [DataMember]
        public string LimitedExport { get; set; }

        [DataMember]
        public string StandByGen { get; set; }
        [DataMember]
        public string ProgramType { get; set; }
        [DataMember]
        public string ExportToGrid { get; set; }


    }
    /**** ENOS Tariff Change - End ******/
    //WEBR Stability - Replace Feature Service with WCF for JET
    [DataContract]
    public class EquipmentIdType
    {
        [DataMember]
        public string ObjectId { set; get; }
        [DataMember]
        public Int16 EquipTypeId { set; get; }
        [DataMember]
        public string EquipTypeDesc { set; get; }
        [DataMember]
        public string HasOperatingNum { set; get; }
        [DataMember]
        public string HasCGC12 { set; get; }
    }

    [DataContract]
    public class JetViewJobEquip
    {
        [DataMember]
        public int ObjectId { get; set; }
        [DataMember]
        public string CGC12 { get; set; }
        [DataMember]
        public string JobNumber { get; set; }
        [DataMember]
        public string Address { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string ReservedBy { get; set; }
        [DataMember]
        public int Division { get; set; }
        [DataMember]
        public DateTime LastModifiedDate { get; set; }
        [DataMember]
        public int Status { get; set; }
        [DataMember]
        public string UserAudit { get; set; }
        [DataMember]
        public DateTime EntryDate { get; set; }
    }

    //Favorites start - WEBR Stability - Replace Feature Service with WCF for Favorites
    [DataContract]
    public class FavoritesData
    {
        [DataMember]
        public string ObjectId { set; get; }
        [DataMember]
        public string LayerVisibilities { set; get; }
        [DataMember]
        public string StoredView { set; get; }
        [DataMember]
        public string Name { set; get; }
        [DataMember]
        public string DefaultYN { set; get; }
        [DataMember]
        public string UserName { set; get; }
    }

    [DataContract]
    public class FavoritesSuccessData
    {
        [DataMember]
        public bool isSuccess { set; get; }
        [DataMember]
        public List<FavoritesData> FavoritesList { set; get; }
    }
    //Favorites end

    //ME Q3 2019 - DA# 190501
    [DataContract]
    public class LoadingInfoCustomerData
    {
        [DataMember]
        public string GLOBALID { set; get; }
        [DataMember]
        public string CUSTOMERTYPE { set; get; }
    }
}
