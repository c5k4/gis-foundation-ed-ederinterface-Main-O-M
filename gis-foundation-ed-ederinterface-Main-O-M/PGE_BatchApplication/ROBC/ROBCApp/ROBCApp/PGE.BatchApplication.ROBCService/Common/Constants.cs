using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using PGE_DBPasswordManagement;
namespace PGE.BatchApplication.ROBCService.Common
{
    public static class Constants
    {
        public static readonly string EDERDatabaseTNSName;
        public static readonly string EDERUserName;
        public static readonly string EDERPassword;
        public static readonly string EDERSUBDatabaseTNSName;
        public static readonly string EDERSUBUserName;
        public static readonly string EDERSUBPassword;

        public static readonly string RobcDomainName;
        public static readonly string SubBlockDomainName;
        public static readonly string Division;
        public static readonly string EDERGeometricNetworkName;
        public static readonly string SUBGeometricNetworkName;
        public static readonly string EstablishedROBCCode;
        public static readonly string FixedEssentialROBCCode;

        public static readonly string EepReportFilePath;
        public static readonly string EctpReportFilePath;
        public static string[] EDERUserInst;
        public static string[] EDERSUBUserInst;
        static Constants()
        {
            // m4jf edgisrearch 919 - Get password using Password management tool

            EDERUserInst = ConfigurationManager.AppSettings["EDER_ConnectionStr"].Split('@');
            EDERSUBUserInst = ConfigurationManager.AppSettings["EDERSUB_ConnectionStr"].Split('@');
            // m4jf edgisrearch 919 - commented below line of code - getting password using Password management tool .

            //EDERDatabaseTNSName = ConfigurationManager.AppSettings["EDERDatabaseTNSName"];
            //EDERUserName = ConfigurationManager.AppSettings["EDERUserName"];
            //EDERPassword = ConfigurationManager.AppSettings["EDERPassword"];
            //EDERSUBDatabaseTNSName = ConfigurationManager.AppSettings["EDERSUBDatabaseTNSName"];
            //EDERSUBUserName = ConfigurationManager.AppSettings["EDERSUBUserName"];
            //EDERSUBPassword = ConfigurationManager.AppSettings["EDERSUBPassword"];

            EDERUserName = EDERUserInst[0].ToUpper();
            EDERDatabaseTNSName = EDERUserInst[1].ToUpper();

            EDERPassword = ReadEncryption.GetPassword(EDERUserName+"@"+ EDERDatabaseTNSName);
            EDERSUBUserName = EDERSUBUserInst[0].ToUpper();
            EDERSUBDatabaseTNSName = EDERSUBUserInst[1].ToUpper();

            EDERSUBPassword = ReadEncryption.GetPassword(EDERSUBUserName + "@" + EDERSUBDatabaseTNSName);



            RobcDomainName = ConfigurationManager.AppSettings["RobcDomainName"];
            SubBlockDomainName = ConfigurationManager.AppSettings["SubBlockDomainName"];
            Division = ConfigurationManager.AppSettings["Division"];
            EstablishedROBCCode = ConfigurationManager.AppSettings["EstablishedROBCCode"];
            FixedEssentialROBCCode = ConfigurationManager.AppSettings["FixedEssentialROBCCode"];

            EDERGeometricNetworkName = ConfigurationManager.AppSettings["EDERGeometricNetworkName"];
            SUBGeometricNetworkName = ConfigurationManager.AppSettings["SUBGeometricNetworkName"];

            EepReportFilePath = ConfigurationManager.AppSettings["EEPReportFilePath"];
            EctpReportFilePath = ConfigurationManager.AppSettings["ECTPReportFilePath"];
        }
    }
}
