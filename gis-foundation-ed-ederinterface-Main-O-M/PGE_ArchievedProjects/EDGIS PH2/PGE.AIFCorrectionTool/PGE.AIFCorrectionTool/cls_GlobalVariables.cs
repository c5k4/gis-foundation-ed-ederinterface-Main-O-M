using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace AIFCorrection
{
    class cls_GlobalVariables
    {
        // Queried FeatureclassesField Name
        public const string strInstallJobNumber="INSTALLJOBNUMBER";
        public const string strGlobalID = "GLOBALID";
        public const string strLocalOfficeID = "LOCALOFFICEID";
        //CSV Column Name
        public static string strCSV_ORDERNUMBER = ConfigurationManager.AppSettings["CSV_COLUMN_ORDER_NO"];
        public static string strCSV_CN24DATE = ConfigurationManager.AppSettings["CSV_COLUMN_CN24_CompDate"];
        public static string strCSV_DESCDATE = ConfigurationManager.AppSettings["CSV_COLUMN_DESC_DATE"];

        //Customer Agreement TableName and  Field Name
        public static string strCustomer_Agreement_TableName = ConfigurationManager.AppSettings["CUSTOMER_AGREEMENT_TABLE"];
        public static string strCUstomer_Agreement_Table_AliasName = "Customer Agreement";
        public const string strCustomer_Agreement_Field_AGREEMENTTYPE = "AGREEMENTTYPE";
        public const string strCustomer_Agreement_Field_AGREEMENTNUM = "AGREEMENTNUM";
        public const string strCustomer_Agreement_Field_AIEXPIRATIONDATE = "AIEXPIRATIONDATE";
        public const string strCustomer_Agreement_Field_ATEXPIRATIONDATE = "ATEXPIRATIONDATE";
        public const string strCustomer_Agreement_Field_AGREEMENTCOMMENTS = "AGREEMENTCOMMENTS";
        public const string strCustomer_Agreement_Field_OFFICECODE = "OFFICECODE";
        public const string strCustomer_Agreement_Field_CREATIONUSER = "CREATIONUSER";
        public const string strCustomer_Agreement_Field_DATECREATED = "DATECREATED";
        public const string strCustomerFieldValue_AgreementTypeValue = "AW";
        //SDE FILE,VERSION NAME,LOGFILE PATH,CSVFILE PATH Decleration
        public static string strAPPCONFIG_SDEFILEPATH = ConfigurationManager.AppSettings["SDE_FILEPATH"].ToString();
        public static string strAPPCONFIG_VERSIONNAME = ConfigurationManager.AppSettings["SESSION_NAME"].ToString();
        public static string strAPPCONFIG_CSVNAME = ConfigurationManager.AppSettings["CSVFILENAME"].ToString();
    }
}
