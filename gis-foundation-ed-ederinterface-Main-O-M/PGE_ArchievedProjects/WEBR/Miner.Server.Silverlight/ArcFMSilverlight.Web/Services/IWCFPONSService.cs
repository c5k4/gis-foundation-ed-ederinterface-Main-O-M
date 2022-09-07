/*Planned Outage */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using ArcFMSilverlight;
using System.Data;

namespace ArcFMSilverlight.Web.Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IWCFPONSService" in both code and config file together.
    [ServiceContract]
    public interface IWCFPONSService
    {
        [OperationContract]
        string GetExportdataFile(string outagedate, string Area, string message, List<TransformercustomerdataComb> customerData, string strFileFormat, string strUserName);
        [OperationContract]
        List<Devicestartcustomerdata> GetDevicedataCustomerSearch(string strDevice1, string strDevice2, int Divisioncode);
        [OperationContract]
        string SendtoPSL(List<StagingdataCust> customerData, int plannedOutageID, string contactID, string division, string work1DTStart, string work1TMStart, string work1TMEnd, string work2DTStart, string work2TMStart, string work2TMEnd, string vicinity, string strUserName);
        [OperationContract]
        string GetAverydata(List<TransformercustomerdataComb> customerData, int skips, string UserName);
        [OperationContract]
        List<TransformercustomerdataComb> GetCustomerdataComb(string strCGSValue, string strCircuitId, string strServicePointID, string strDivisionIn, string strFlagIn);
        //Standard Map Print
        [OperationContract]
        string GetStandardPrint(List<string> Gridlist, string urlpath, string template, string size, string userid);
        //JetJob
        [OperationContract]
        List<JetJoblist> Getjetjoblist(string STATUS, string RESERVEDBY, string DIVISION, string JobNumber, int pagesize);
        [OperationContract]
        string GetDeleteJob(string objectID, string userid);
        [OperationContract]
        string GetEditJob(string objectID, int STATUS, string RESERVEDBY, int DIVISION, string JobNumber, string USERAUDIT, string description, DateTime ENTRYDATE);
        [OperationContract]
        List<jetequipmentlist> GetEquipmentJob(string objectID, string JobNumber);
        [OperationContract]
        List<jetequipmentlist> GetDeleteEquipment(string objectID, string Opstatus);
        [OperationContract]
        List<jetequipmentlist> AddEditJobEquipment(string objectID, int STATUS, int EQUIPTYPEID, string OPERATINGNUMBER, string City, string SketchLoc, string InstallCdName, string Address, Decimal LATITUDE, Decimal LONGITUDE, string CUSTOWNED, string JobNumber, string USERAUDIT, string Cgc12);
        [OperationContract]
        List<jetequipmentlist> EditJobNumberEquipment(string oldJobNum, string newJobNum);

        //WEBR Stability - Replace Feature Service with WCF for JET
        [OperationContract]
        bool[] ValidateAddEditJob(string description, int division, DateTime entryDate, string jobNumber, DateTime lastModifiedDate, string reservedBy, string userAudit, int status, int objectId, string action);

        [OperationContract]
        List<EquipmentIdType> GetEquipmentIDTypes();

        [OperationContract]
        Dictionary<string, string> GetInstallTypeDomain();

        [OperationContract]
        jetequipmentlist GetJetLastEquipLocation(string jobNumber);

        [OperationContract]
        Dictionary<object, string> GetJETDivisions();

        [OperationContract]
        List<JetViewJobEquip> SearchJetJobEquip(string searchInput, string filterBase);        

        //ENOC

        [OperationContract]
        List<GenFeederDetails> GetGenFeederDetails(string FeederId);

        //CIT *start
        [OperationContract]
        bool CIT_Process(int objectID, string globalID, string filledduct, string requested_on, string lanid);

        [OperationContract]
        int CIT_ConduitType(int objectID);

        [OperationContract]
        int CIT_FilledValue(int objectID);

        [OperationContract]
        DateTime? CIT_UpdatedonValue(int objectID);

        //CIT *end

        //Favorites start
        [OperationContract]
        List<FavoritesData> GetFavoritesList(string LANID);

        [OperationContract]
        FavoritesSuccessData DeleteFavorites(int objectId, string lanId);

        [OperationContract]
        FavoritesSuccessData AddEditFavorites(string insertOrUpdate, string user, string name, string storedView, string layerVisibilities);
        //Favorites end
        
          /****************************ENOS2SAP PhaseIII Start****************************/
          [OperationContract]
          List<InverterNameplateDetails> getInverterNameplateFromSettings(string sapEGINotification);
        /****************************ENOS2SAP PhaseIII End****************************/

          /****************************ENOS Tariff Change Start****************************/
          [OperationContract]
          List<DataToExportDetails> getDataToExportFromSettings(string sapEGINotification);
          /****************************ENOS Tariff Change End****************************/

          //Loading Information - //ME Q3 2019 Release - DA# 190501
          [OperationContract]
          List<LoadingInfoCustomerData> GetLoadingInfoCustData(string cgcList);

          //DA# 190506 - WEBR Access Management - ME Q3 2019 Release
          [OperationContract]
          bool StoreUserDetails(string LANID, string machineName);
    }
}

