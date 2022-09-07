using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using Oracle.DataAccess.Client;


namespace Utility
{   
    class Program
    {
   
        static int Main(string[] args)
        {
            
            if (args.Length == 0)
                return 0;
            if (args[0].Equals("test", StringComparison.CurrentCultureIgnoreCase))
            {
                testTMLConnection();
                return 0;
            }
            if (args[0].Equals("LOADSEER", StringComparison.CurrentCultureIgnoreCase))
            {
                int retVal = 0;
                if (args.Length == 2)
                {
                    switch (args[1].ToUpper())
                    {
                        case "FEEDER":
                            retVal = LoadSEER.LoadFeederStagingTables();
                            break;
                        case "TRANSFORMER":
                            retVal =LoadSEER.LoadTransformerStagingTables();
                            break;
                    
                    }

                }
                return retVal;
            }
            if (args[0].Equals("ROBC", StringComparison.CurrentCultureIgnoreCase))
            {
                int retVal = 0;
                if (args.Length == 2)
                {
                    switch (args[1].ToUpper())
                    {
                        case "EEP":
                            retVal = Robc.LoadEepStagingTables();
                            break;
                        case "ECTP":
                            retVal = Robc.LoadEctpStagingTables();
                            break;

                    }

                }
                return retVal;
            }
            
            if (args[0].Equals("Settings", StringComparison.CurrentCultureIgnoreCase))
            {
                int retVal = 0;
                if (args.Length == 2)
                {
                    switch (args[1].ToUpper())
                    {
                        case "OMSDMS":
                            retVal = OMSDMS.Process();
                            break;
                    }
                }
                return retVal;
            }

            if (args[0].Equals("TLM", StringComparison.CurrentCultureIgnoreCase))
            {
                int retVal = 0;
                if (args.Length == 2)
                {
                    switch (args[1].ToUpper())
                    {
                        case "CDW":
                            retVal = TLM.ProcessCDWFiles(true, true);
                            break;
                        case "CCNB":
                            retVal = TLM.ProcessCCNBFiles(true, Constants.TLMCCNBZipped);
                            break;
                        case "GIS":
                            retVal = TLM.ProcessGISChanges();
                            break;
                        case "MIGRATETOSTAGING":
                            retVal = TLM.MigrateExtToStaging();
                            break;
                        case "MONTHLYLOAD":
                            retVal = TLM.RunMonthyLoad();
                            break;
                        case "REPORT":
                            retVal = TLM.SendMonthyLoadLog();
                            break;
                        case "CYMEMONTHLYLOAD":
                            retVal = TLM.RunCYMEMonthyLoad();
                            break;
                        //added  on  12/04/2019 for TLM ENHANCEMENT
                        case "GCD":
                            retVal = TLM.ProcessGenerateCD();
                            break;
                        case "MTB":
                            retVal = TLM.ProcessMaintainTb();
                            break;
                        case "PCCB":
                            retVal = TLM.ProcessCcb();
                            break;
                        case "PCDW":
                            retVal = TLM.populateCDW();
                            break; 
                     }
                }
                //added  on  12/04/2019 for TLM ENHANCEMENT
                else if (args.Length == 4)
                {
                    if (args[1].ToUpper() == "MTR")
                    {
                        string arg3 = args[2].ToUpper();
                        string arg4 = args[3].ToUpper();
                        retVal = TLM.ProcessMonthlyTr(arg3, arg4);

                    }                    
                }
                return retVal;
            }
            
            else
                return 0;
        }

        private static void testTMLConnection()
        {
            using (Oracle.DataAccess.Client.OracleConnection connection = new Oracle.DataAccess.Client.OracleConnection(Constants.TLMConnectionString))
            {
                connection.Open();

                connection.Close();
            }
        }
    }
}
