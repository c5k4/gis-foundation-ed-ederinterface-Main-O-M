using System;
using System.ServiceProcess;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;
using Microsoft.VisualBasic;
using System.Management;
using System.Security.Principal;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices;
using System.Collections;
using System.Net.Sockets;

namespace ValidateSystemConfig
{
    class Program
    {
        /// <summary>
        /// static variables 
        /// </summary>
        #region "static variables"
       
        static string logfilelocation = ConfigurationManager.AppSettings["logfilelocation"];
        static string cfgHostnamekeypath = ConfigurationManager.AppSettings["hostnamekeypath"];
        static string cfgHostnamekey = ConfigurationManager.AppSettings["Hostnamekeyname"];
        static string cfgwinheapsizekeypath = ConfigurationManager.AppSettings["winheapsizekeypath"];
        static string cfgwinheapsizekeyname = ConfigurationManager.AppSettings["winheapsizekeyname"];
        static string cfgvirtualmemkeypath = ConfigurationManager.AppSettings["virtualmemkeypath"];
        static string cfgpagingfileskeyname = ConfigurationManager.AppSettings["pagingfileskeyname"];
        static string cfgdotnetversionkeypath = ConfigurationManager.AppSettings["dotnetversionkeypath"];
        static string cfgdotnetreleasekeyname = ConfigurationManager.AppSettings["dotnetreleasekeyname"];
        static string cfgiiskeypath = ConfigurationManager.AppSettings["iiskeypath"];

        static string cfgtcpipkeypathkeypath = ConfigurationManager.AppSettings["tcpipkeypath"];
        static string cfgmaxUserPortkeyname = ConfigurationManager.AppSettings["maxUserPort"];
        static string cfgtcpTimedWaitDelay = ConfigurationManager.AppSettings["tcpTimedWaitDelay"];

        static string cfguc4agentservicename = ConfigurationManager.AppSettings["uc4agentservicename"];
        static string log_in_database = ConfigurationManager.AppSettings["log_in_database"];
        static string cfg_administrators = ConfigurationManager.AppSettings["Administrators"];
        static string cfg_datasource = ConfigurationManager.AppSettings["datasource"];
        static string cfg_user = ConfigurationManager.AppSettings["user"];
        static string cfg_pwd = ConfigurationManager.AppSettings["pwd"];
        static string cfg_accessgroup = ConfigurationManager.AppSettings["accessgroup"];
        static string cfg_installedsoftware = ConfigurationManager.AppSettings["installedsoftware"];
        static string cfg_check32bitsoftwares = ConfigurationManager.AppSettings["check32bitsoftwares"];
        static string cfg_check64bitsoftwares = ConfigurationManager.AppSettings["check64bitsoftwares"];
        static string cfg_installedfeatures = ConfigurationManager.AppSettings["installedfeatures"];
        static string cfg_host_port_filepath = ConfigurationManager.AppSettings["host_port_filepath"];
        static string cfg_checkport = ConfigurationManager.AppSettings["checkport"];
        static string cfg_msxml6path = ConfigurationManager.AppSettings["msxml6path"];
        

        static Oracle.DataAccess.Client.OracleConnection conn;
        static StreamWriter sw;
        static string hostname;
        #endregion
        static void Main(string[] args)
        {
            try
            {
                string rootoutputpath = logfilelocation;
                string filepath = string.Empty;
               // checkWMIConnection();
                //return ;


                RegistryKey keyTCP = Registry.LocalMachine.OpenSubKey(cfgHostnamekeypath);
                if (keyTCP != null)
                {
                    Object keyValue = keyTCP.GetValue(cfgHostnamekey);
                    if (keyValue != null)
                        hostname = keyValue.ToString();
                    
                    filepath = logfilelocation + "//" + hostname + ".txt";
                }
                else
                {
                    Console.WriteLine("Unable to read host name");
                    Console.ReadKey ();
                    return;
                }
                try
                {
                    if (File.Exists(filepath))
                        File.Delete(filepath);
                    sw = new StreamWriter(filepath);
                }
                catch (Exception ex)
                {

                    Console.WriteLine("Error while creating log file.Please check log file path in config file "+ex.Message);
                }
                
                sw.WriteLine("Datetime,Paramter,value,remarks");
                
                writelog(sw,hostname,"hostname",hostname, "hostname");
               
                RegistryKey winheapsizekey = Registry.LocalMachine.OpenSubKey(cfgwinheapsizekeypath);
                if (winheapsizekey != null)
                {
                    string winheapsizeValue = winheapsizekey.GetValue(cfgwinheapsizekeyname).ToString();
                    if (winheapsizeValue != null)
                    {
                        int i = winheapsizeValue.IndexOf("SharedSection");
                        string strTeamp = winheapsizeValue.Substring(i + 14);
                        i = strTeamp.IndexOf(" ");
                        strTeamp = strTeamp.Substring(0, i);
                        string remarks = strTeamp;
                        i = strTeamp.LastIndexOf(",");
                        strTeamp = strTeamp.Substring(i + 1);

                       
                        // writelog(sw,"WinheapSize:"+ winheapsizeValue); writelog(sw,hostname,"hostname",hostname, cfgHostnamekeypath);
                        writelog(sw, hostname, "WinheapSize", strTeamp, "non-interactive desktop heap:"+ remarks);
                        //Within the Windows string value, there is a SharedSection parameter that, by default, 
                        //should read SharedSection = 1024,20480,768.The last number represents non-interactive desktop heap 
                        //that should be increased, for example from 768 to be 1024.
                        //   Make sure that the entirety of the key value is preserved and only that one part is to be modified.
                    }
                    else
                        writelog(sw, hostname, "WinheapSize", "error", cfgwinheapsizekeypath);
                }
                else
                    writelog(sw, hostname, "WinheapSize", "error", cfgwinheapsizekeypath);
                //
                //checksoftwareinstalledbackup();
                
                getDriveInfo();
                getVirtualMemorySize();
                getNETFrameworkVer();
               // GetProcessorInformation();
                getCPUCores();
                GetIisVersion();
                checkUC4AgentInstallation();
                //IsUserAdministrator();
                checkAdminstrators();
                getTCPLimit();
                checkSoftwareInstalled();
                checkinstalledFeature();
                checkPortStatus();
                checkXML6();
                writelog(sw, hostname, "PROGRAM", "SUCCESS", "Exectued Sucessfully",true);
                
            }

            catch (Exception ex)  
            {
               
                writelog(sw, hostname, "PROGRAM", "FAILED", "Completed with Error"+ex.Message,true);
            }
            sw.Flush();
            sw.Close();
            closeDBConnection();
            Console.WriteLine("Enter key to exit ....");
            Console.ReadKey();

        }
        static void writelog(StreamWriter sw,string hostname,string param, string value, string remarks,bool nodblog=false)
        {
            //"TSEDGISCTXWX001"
            //hostname = "TSEDGISCTXWX001";
            //PRLBGMAPPWG001
            sw.WriteLine(System.DateTime.Now.ToString()+","+ hostname +","+param+","+value+","+remarks);
            Console.WriteLine(System.DateTime.Now.ToString() + "," + hostname + "," + param + "," + value + "," + remarks);
            string system, env ,servertype= string.Empty;
            env = hostname.Substring(0, 2);
            system = hostname.Substring(2, 2);
            servertype = hostname.Substring(6, 3);

            if (bool.Parse(log_in_database) && nodblog==false)
                writeDBLog(env,system,servertype,hostname, param, value, remarks);
        }
        static Oracle.DataAccess.Client.OracleConnection openDBConnection()
        {
            //string strDBUser = "UC4Admin";
            string oradb = "Data Source=" + cfg_datasource + "; User Id="+cfg_user+";Password="+cfg_pwd+";";
            if (conn== null || conn.State != System.Data.ConnectionState.Open)
            {
                conn = new Oracle.DataAccess.Client.OracleConnection(oradb);
                conn.Open();
            }
            return conn;
        }
        static void closeDBConnection()
        {
            if (conn != null && conn.State == System.Data.ConnectionState.Open)
            {
                conn.Close();
                conn.Dispose();
            }
            
        }

        static void writeDBLog(string env,string system,string servertype, string hostname, string param, string value, string remarks)
        {
            //string strDatabase = entry.Key.Substring(0, indx);
            
            //Oracle.DataAccess.Client.OracleConnection conn = new Oracle.DataAccess.Client.OracleConnection(oradb); 
            //conn.Open();
            Oracle.DataAccess.Client.OracleCommand cmd = new Oracle.DataAccess.Client.OracleCommand();
            cmd.Connection = openDBConnection();
            if (remarks.Length > 200)
                remarks = remarks.Substring(0, 200);
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO "+ cfg_user+".SVR_VALIDATION_OUTPUT (ENVIRONMENT,SYSTEM,SERVER_TYPE,   HOST_NAME,PARAM_NAME, PARAM_VALUE, REMARKS, CREATED_DATE) values");
            sb.Append("('"+env+"','"+system+"',");
            sb.Append("'" + servertype + "','" + hostname + "',");
            sb.Append("'" + param + "','" + value + "',");
            sb.Append("'" + remarks + "',");
            sb.Append("sysdate)");


            cmd.CommandText = sb.ToString();
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.ExecuteNonQuery();
        }
        static void getDriveInfo()
        {
            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (DriveInfo drive in drives)
            {
                //There are more attributes you can use.
                //Check the MSDN link for a complete example.
                //Console.WriteLine(drive.Name);
                if (drive.IsReady)
                {
                    int diskSizinGB = (int)(drive.TotalSize / (1024 * 1024 * 1024));
                    
                    writelog(sw, hostname, drive.Name, diskSizinGB.ToString(), "Disk Size(GB)");
                }
               
              //  writelog(sw, hostname, drive.Name, "error", "Error reading value");

            }
        }
        static void getVirtualMemorySize()
        {

            Microsoft.VisualBasic.Devices.ComputerInfo deviceinfo = new Microsoft.VisualBasic.Devices.ComputerInfo();
            double totalVirtualMemory = deviceinfo.TotalVirtualMemory / (1024 * 1024 * 1024);

           // Console.WriteLine("totalVirtualMemory :{0}GB", totalVirtualMemory);
            writelog(sw, hostname, "totalVirtualMemory", totalVirtualMemory.ToString(), "Total Virtual Memory(GB)");
            double totalPhysicalMemory = deviceinfo.TotalPhysicalMemory / (1024 * 1024 * 1024);
            //Console.WriteLine("totalPhysicalMemory :{0}GB", totalPhysicalMemory);
            writelog(sw, hostname, "totalPhysicalMemory", totalPhysicalMemory.ToString(), "Total Physical Memory(GB)");
            RegistryKey keyVM = Registry.LocalMachine.OpenSubKey(cfgvirtualmemkeypath);
            if (keyVM == null)
            {
                writelog(sw, hostname, "Virtual Memory Config", "unable to read", "error");
                return;
            }
            
            string[] keyValue = (string[])keyVM.GetValue(cfgpagingfileskeyname);
            //expectedvalue
            //  C:\pagefile.sys 0 0
            //D:\pagefile.sys 128000 136000
            bool blsuccess = false;
                
            if (keyValue != null)
            {
                foreach (string path in keyValue)
                {
                    //Console.WriteLine(path.ToString());
                    string[] keys = path.ToString().Split(' ');
                    if (keys.Length >= 3)
                    {
                        string strpagefilepath = keys[0].ToString();
                        if (strpagefilepath.ToUpper().Contains("D:"))
                        {
                            int min = int.Parse(keys[1].ToString());
                            //min = min / 1024;
                            int max = int.Parse(keys[2].ToString());
                            blsuccess = true;
                            //max = max / 1024;
                            writelog(sw, hostname, "Virtual Memory Config", strpagefilepath, "init. size:" + min.ToString() + "(MB).Max size:" + max.ToString() + "MB");
                        } 
                    }
                }

            }
            if  (!blsuccess)
                writelog(sw, hostname, "Virtual Memory Config", "not custom", "Please report to windows administrator");




        }

        static void getNETFrameworkVer()
        {
            using (var ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(cfgdotnetversionkeypath))
            {
                if (ndpKey != null && ndpKey.GetValue(cfgdotnetreleasekeyname) != null)
                {
                    //Console.WriteLine($".NET Framework Version: {CheckFor45PlusVersion((int)ndpKey.GetValue(cfgdotnetreleasekeyname))}");
                    writelog(sw, hostname, ".NET Framework Version", CheckFor45PlusVersion((int)ndpKey.GetValue(cfgdotnetreleasekeyname)), ".NET Framework Version");
                }
                else
                {
                   
                    writelog(sw, hostname, ".NET Framework Version", ".NET Framework Version 4.5 or later is not detected.", "Validation failed");
                }
            }
        }
        // Checking the version using >= enables forward compatibility.
        static string CheckFor45PlusVersion(int releaseKey)
        {
            if (releaseKey >= 528040)
                return "4.8 or later";
            if (releaseKey >= 461808)
                return "4.7.2";
            if (releaseKey >= 461308)
                return "4.7.1";
            if (releaseKey >= 460798)
                return "4.7";
            if (releaseKey >= 394802)
                return "4.6.2";
            if (releaseKey >= 394254)
                return "4.6.1";
            if (releaseKey >= 393295)
                return "4.6";
            if (releaseKey >= 379893)
                return "4.5.2";
            if (releaseKey >= 378675)
                return "4.5.1";
            if (releaseKey >= 378389)
                return "4.5";
            // This code should never execute. A non-null release key should mean
            // that 4.5 or later is installed.
            return "No 4.5 or later version detected";
        }

        public static String GetProcessorInformation()
        {
            ManagementClass mc = new ManagementClass("win32_processor");
            ManagementObjectCollection moc = mc.GetInstances();
            String info = String.Empty;
            foreach (ManagementObject mo in moc)
            {
                string name = (string)mo["Name"];
                name = name.Replace("(TM)", "™").Replace("(tm)", "™").Replace("(R)", "®").Replace("(r)", "®").Replace("(C)", "©").Replace("(c)", "©").Replace("    ", " ").Replace("  ", " ");

                info = name + ", " + (string)mo["Caption"] + ", " + (string)mo["SocketDesignation"];
                //mo.Properties["Name"].Value.ToString();
                //break;
            }
            return info;
        }

        static void getCPUCores()
        {
            //phisycal processor
            foreach (var item in new System.Management.ManagementObjectSearcher("Select * from Win32_ComputerSystem").Get())
            {
                //Console.WriteLine("Number Of Physical Processors: {0} ", item["NumberOfProcessors"]);
                writelog(sw, hostname, "Number Of Physical Processors", item["NumberOfProcessors"].ToString(), "number of physical processors");
            }
            //number of core
            int coreCount = 0;
            foreach (var item in new System.Management.ManagementObjectSearcher("Select * from Win32_Processor").Get())
            {
                coreCount += int.Parse(item["NumberOfCores"].ToString());

            }
            //number of logical processors
           // Console.WriteLine("Number Of Cores: {0}", coreCount);
            writelog(sw, hostname, "Number Of Cores", coreCount.ToString(), "number of cores");
            // Console.WriteLine("Number Of Logical Processors: {0}", Environment.ProcessorCount);
            foreach (var item in new System.Management.ManagementObjectSearcher("Select * from Win32_ComputerSystem").Get())
            {
               // Console.WriteLine("Number Of Logical Processors: {0}", item["NumberOfLogicalProcessors"]);
                writelog(sw, hostname, "Number Of Logical Processors", coreCount.ToString(), "number of logical processors");
            }
        }
        public static Version GetIisVersion()
        {
            using (RegistryKey componentsKey = Registry.LocalMachine.OpenSubKey(cfgiiskeypath, false))
            {
                if (componentsKey != null)
                {
                    int majorVersion = (int)componentsKey.GetValue("MajorVersion", -1);
                    int minorVersion = (int)componentsKey.GetValue("MinorVersion", -1);

                    if (majorVersion != -1 && minorVersion != -1)
                    {
                       
                        writelog(sw, hostname, "Microsoft IIS", "IIS"+ majorVersion+ "."+minorVersion,"IIS Installed");
                        return new Version(majorVersion, minorVersion);
                    }
                    else
                        writelog(sw, hostname, "Microsoft IIS", "IIS installed", "Validation failed");
                }else
                    writelog(sw, hostname, "Microsoft IIS", "Missing", "Microsoft IIS not installed");


                return new Version(0, 0);
            }
        }

      static void  checkUC4AgentInstallation()
        {
            ServiceController sc = new ServiceController(cfguc4agentservicename);
           
            try
            {
                string status = sc.Status.ToString();
                writelog(sw, hostname, cfguc4agentservicename, status, "UC4 Agent Windows Service");
            }
            catch (Exception ex)
            {
                writelog(sw, hostname, cfguc4agentservicename, "Not Installed", "UC4 Agent Windows Service");
            }
        }
      static void checkAdminstrators()
        {
            try
            {
                List<string> lstAccounts;
                string [] accounts = cfg_administrators.Split(';');
                lstAccounts= accounts.ToList();
                //string groupNameToSearchFor = "Administrators"; // can be any group,maybe better to use something like builtin.administrators
                               
                //string hostName = "VPEG01B004419";
                //get machine
                using (DirectoryEntry machine = new DirectoryEntry("WinNT://" + hostname))
                {
                    //get local admin group
                    using (DirectoryEntry group = machine.Children.Find(cfg_accessgroup, "Group"))
                    {
                        //get all members of local admin group
                        bool matchFound = false;
                        object members = group.Invoke("Members", null);
                        foreach (string item in lstAccounts)
                        {
                            
                            foreach (object member in (IEnumerable)members)
                            {
                                matchFound = false;
                                //get account name
                                string accountName = new DirectoryEntry(member).Name;
                                if (accountName.ToUpper() == item.ToUpper())
                                {
                                    //object accountFound = lstAccounts[lstAccounts.FindIndex(accountName)];
                                    writelog(sw, hostname, item, cfg_accessgroup, cfg_accessgroup);
                                    matchFound = true;
                                    break;
                                }
                            }
                            if (matchFound==false)
                                writelog(sw, hostname, item, "Missing", "Account is not an administrator");
                        }
                    }
                }
            }
            
            catch (Exception e)
            {
                writelog(sw, hostname, cfguc4agentservicename, "Missing", "Account is not an administrator");
            }
        }

        static void getTCPLimit()
        {

          /*  If the MaxUserPort parameter has been set, ensure that MaxUserPort =
           *  65534 is set in the Windows registry at 
           *  HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\Tcpip\Parameters.a.MaxUserPort = 65534
            Value Name: MaxUserPort
            Value Type: DWORD
            Value data: 65534
            Valid Range: 65534(decimal)
            b.TcpTimedWaitDelay = 30
            Value Name: TcpTimedWaitDelay
            Value Type: DWORD
            Value data: 30
            Valid Range: 30(decimal)*/
            RegistryKey tcpipkeypathkeypath = Registry.LocalMachine.OpenSubKey(cfgtcpipkeypathkeypath);
            if (tcpipkeypathkeypath == null)
            {
                writelog(sw, hostname, "MaxUserPort", "Missing", cfgtcpipkeypathkeypath);
                writelog(sw, hostname, "TcpTimedWaitDelay", "Missing", cfgtcpipkeypathkeypath);
                return;
            }
            object maxUserPortvalue = tcpipkeypathkeypath.GetValue(cfgmaxUserPortkeyname);
            
            if (maxUserPortvalue != null)
                   writelog(sw, hostname, "MaxUserPort", maxUserPortvalue.ToString(), "MaxUserPort");
            else
                writelog(sw, hostname, "MaxUserPort", "Missing", "MaxUserPort configuration is missing");

            object tcpTimedWaitDelay = tcpipkeypathkeypath.GetValue(cfgtcpTimedWaitDelay);
            if (tcpTimedWaitDelay != null)
                writelog(sw, hostname, "tcpTimedWaitDelay", tcpTimedWaitDelay.ToString(), "tcpTimedWaitDelay");
            else
                writelog(sw, hostname, "tcpTimedWaitDelay", "Missing", "tcpTimedWaitDelay configuration is missing");
        }
        static void checksoftwareinstalledbackup()
        {
            
            List<string> lstAccounts;
            string[] softwarelist = cfg_installedsoftware.Split(';');
            lstAccounts = softwarelist.ToList();
            StringBuilder sb = new StringBuilder();
            foreach (object item in lstAccounts)
            {
                sb.Append("(name='" + item.ToString() + "') OR ");
            }
            string finalclause = sb.ToString().Substring(0, sb.Length - 3);
            string query = "SELECT * FROM Win32_Product where "+ finalclause;
            

            List<string> lstFound = new List<string>();
            try
            {
                ManagementObjectSearcher mos = new ManagementObjectSearcher(query);
                
                foreach (ManagementObject mo in mos.Get())
                {
                    Console.WriteLine(mo["Name"]);
                    lstFound.Add(mo["Name"].ToString());
                    //ArcGIS Server 10.8.1
                    //Notepad++
                    // Chef Infra Client v16.8.14;
                    //PGE ED Desktop EDER v10.8.1.0
                    //ROBC Mangement App_v10.8.35
                    //ArcGIS PerfQA Analyzer 10.8
                    //Visual Basic for Applications (R) Core - English
                    //PGE.PerfQA.Setup_v10.8.1
                    //ArcGIS Desktop 10.8.1
                    //ArcGIS VBA Compatibility
                    //ArcFM Solution Desktop 10.8.1
                    //Chrome
                    //Edge
                }
                foreach (string item in lstAccounts)
                {
                    if (lstFound.Contains(item) == true)
                        writelog(sw, hostname, item, "Installed", "Software installed");
                    else
                        writelog(sw, hostname, item, "Missing", "Software not installed");
                }
            }
            catch (Exception ex)
            {

                writelog(sw, hostname, "Software Check", "Error reading", ex.Message);
            }
            
        }

        static void checkinstalledFeature()
        {

            List<string> lstfeaturescfg;
            List<string> lstallfeatures = new List<string>();
            string[] featurelist = cfg_installedfeatures.Split(';');
            lstfeaturescfg = featurelist.ToList();
            //StringBuilder sb = new StringBuilder();
            //foreach (object item in featurelist)
            //{
            //    sb.Append("(name='" + item.ToString() + "') OR ");
            //}
            // string finalclause = sb.ToString().Substring(0, sb.Length - 3);
            string query = "SELECT * FROM Win32_ServerFeature";// where"  + finalclause;
            List<string> lstFound = new List<string>();
            try
            {
                ManagementObjectSearcher mos = new ManagementObjectSearcher(query);

                foreach (ManagementObject mo in mos.Get())
                {
                    //Console.WriteLine(mo["Name"]);
                    lstallfeatures.Add(mo["Name"].ToString());
                }
                foreach (string item in lstfeaturescfg)
                {
                    if (lstallfeatures.Contains(item) == true)
                        writelog(sw, hostname, item, "Installed", "feature installed");
                    else
                        writelog(sw, hostname, item, "Missing", "feature not installed");
                }
            }
            catch (Exception ex)
            {

                writelog(sw, hostname, "Feature Check", "Error reading", ex.Message);
            }
                     
        }

        static void checkSoftwareInstalled()
        {

            List<string> lstAccounts;
            string[] softwarelist = cfg_installedsoftware.Split(';');
            lstAccounts = softwarelist.ToList();
            StringBuilder sb = new StringBuilder();
            foreach (object item in lstAccounts)
            {
                sb.Append("(name='" + item.ToString() + "') OR ");
            }
            List<string> lstFound = new List<string>();
            List<string> lstallsoftware = new List<string>();
            //32 bit softwares
            RegistryKey keys_32bitsoftwares = Registry.LocalMachine.OpenSubKey(cfg_check32bitsoftwares);
           // Console.WriteLine("There are {0} subkeys under {1}.",
            //keys_32bitsoftwares.SubKeyCount.ToString(), keys_32bitsoftwares.Name);
            
            foreach (string subKeyName in keys_32bitsoftwares.GetSubKeyNames())
            {
                using (RegistryKey
                    tempKey = keys_32bitsoftwares.OpenSubKey(subKeyName))
                {
                    //Console.WriteLine("\nThere are {0} values for {1}.",
                      //  tempKey.ValueCount.ToString(), tempKey.Name);
                    foreach (string valueName in tempKey.GetValueNames())
                    {
                        if (tempKey.GetValue(valueName) == null)
                            continue;
                       // Console.WriteLine("{0,-8}: {1}", valueName, tempKey.GetValue(valueName).ToString());
                        if (valueName.ToUpper() == "DisplayName".ToUpper())
                        {
                            lstallsoftware.Add(tempKey.GetValue(valueName).ToString());
                            break;
                        }
                    }
                }
            }
            //64 bit softwares
            RegistryKey keys_64bitsoftwares = Registry.LocalMachine.OpenSubKey(cfg_check64bitsoftwares);
           // Console.WriteLine("There are {0} subkeys under {1}.",
            //keys_64bitsoftwares.SubKeyCount.ToString(), keys_64bitsoftwares.Name);

            foreach (string subKeyName in keys_64bitsoftwares.GetSubKeyNames())
            {
                using (RegistryKey
                    tempKey = keys_64bitsoftwares.OpenSubKey(subKeyName))
                {
                   // Console.WriteLine("\nThere are {0} values for {1}.",
                     //   tempKey.ValueCount.ToString(), tempKey.Name);
                    foreach (string valueName in tempKey.GetValueNames())
                    {
                        if (tempKey.GetValue(valueName) == null)
                            continue;
                       // Console.WriteLine("{0,-8}: {1}", valueName,tempKey.GetValue(valueName).ToString());
                        if (valueName.ToUpper() == "DisplayName".ToUpper())
                        {
                            lstallsoftware.Add(tempKey.GetValue(valueName).ToString());
                            break;
                        }
                    }
                }
            }
           
            string finalclause = sb.ToString().Substring(0, sb.Length - 3);
            string query = "SELECT * FROM Win32_Product where " + finalclause;
            
           
            ManagementObjectSearcher mos = new ManagementObjectSearcher(query);

            foreach (ManagementObject mo in mos.Get())
            {
                //Console.WriteLine(mo["Name"]);
                if (!lstallsoftware.Contains(mo.ToString()))
                    lstallsoftware.Add(mo["Name"].ToString());
            }
            


            foreach (string item in lstAccounts)
            {
                if (lstallsoftware.Contains(item) == true)
                    writelog(sw, hostname, item, "Installed", "Software installed");
                else
                    writelog(sw, hostname, item, "Missing", "Software not installed");
            }

        }
        static void checkXML6()
        {
            if (File.Exists(cfg_msxml6path))
                writelog(sw, hostname, "MSXML6", "Installed", "Software  installed");
            else
                writelog(sw, hostname, "MSXML6", "Missing", "Software not installed");

        }
            static void checkWMIConnection()
        {
            try
            {
                ManagementScope scope = new ManagementScope("\\\\localhost\\root\\cimv2");
                scope.Connect();
                ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_OperatingSystem");
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
            
        }

         static void checkPortStatus()
        {
            try
            {

           
                // path to the csv file
                //string path = "C:/Users/overl/Desktop/people.csv";
                if (cfg_checkport == "false")
                    return;
                if (!File.Exists(cfg_host_port_filepath))
                {
                    writelog(sw, hostname, "Port check", "Port list not found", cfg_host_port_filepath);
                    return;
                }
                string[] lines = System.IO.File.ReadAllLines(cfg_host_port_filepath);
                int linenumber = 0;
                foreach (string line in lines)
                {

                    if (linenumber == 0)
                    {
                        linenumber = +1;
                        continue;
                    }
                    string[] columns = line.Split(',');
                    
                    string[] portcolumn = columns[2].Split(';');
                    if(portcolumn.Length>1)
                    {
                        foreach (string col in portcolumn)
                        {
                            PingHost(columns[1], int.Parse(col));
                        }
                    }
                    else
                        PingHost(columns[1], int.Parse(columns[2]));

                    
                }
            }
            catch (Exception ex)
            {

                writelog(sw, hostname, "Port check", "failed", ex.Message); 
            }

            //PingHost("DVEDGISDBOLX001", 1521);
            //PingHost("DVEDGMDBOLX002", 1521);
            //PingHost("DVLBGISDBOLX001", 1521);
            //PingHost("DVEDGMDBOLX001", 1521);

            //PingHost("TSEDGISDBOLX001", 1521);
            //PingHost("TSEDGMDBOLX002", 1521);
            //PingHost("TSLBGISDBOLX001", 1521);
            //PingHost("TSLBGMDBOLX001", 1521);
            //PingHost("TSEDGMDBOLX001", 1521);

            //PingHost("QAEDGISDBOLG001", 1521);
            //PingHost("QAEDGISDBOLG003", 1521);
            //PingHost("QAEDGMDBOLG005", 1521);

            //PingHost("QAEDGMDBOLG007", 1521);
            //PingHost("QAEDGMDBOLG001", 1521);
            //PingHost("QAEDGMDBOLG003", 1521);

            //PingHost("QALBGISDBOLG001", 1521);
            //PingHost("QALBGMDBOLG001", 1521);
            //PingHost("QALBGMDBOLG003", 1521);

            //PingHost("QAEDGISDBOLX002", 1521);
            //PingHost("QAEDGMDBOLX002", 1521);
            //PingHost("QALBGISDBOLX002", 1521);

            //PingHost("QALBGMAPPWG001", 6080);
            //PingHost("QALBGMAPPWG001", 6443);

            //PingHost("TSEDGMAPPWX001", 6080);
            //PingHost("TSEDGMAPPWX001", 6443);

            //PingHost("TSEDGMAPPWX002", 6080);
            //PingHost("TSEDGMAPPWX002", 6443);
            //PingHost("TSEDGMPTSWX001", 6080);
            //PingHost("TSEDGMPTSWX001", 6443);
            //PingHost("TSLBGMAPPWX001", 6080);
            //PingHost("TSLBGMAPPWX001", 6443);
            //PingHost("TSLBGMAPPWX002", 6080);
            //PingHost("TSLBGMAPPWX002", 6443);
            //PingHost("TSEDGMAPPWX005", 6080);
            //PingHost("TSEDGMAPPWX005", 6443);

        }
        static bool PingHost(string hostUri, int portNumber)
        {




            try
            {
                using (TcpClient client = new TcpClient(hostUri, portNumber)) 
                {
                    //client.Connect(hostUri, portNumber);
                    // NetworkStream ns = client.GetStream();

                    //Console.WriteLine(read(ns));
                    writelog(sw, hostname, hostUri, portNumber.ToString(), "success");
                    //Console.WriteLine("pinging success:'" + hostUri + ":" + portNumber.ToString() + "'");
                    //ns.Close();

                   // client.Close();
                    return true; 
                }
            }
            catch (SocketException ex)
            {
                //Console.WriteLine("Error pinging host:'" + hostUri + ":" + portNumber.ToString() + "'");
                writelog(sw, hostname, hostUri, portNumber.ToString(), "failed:" + ex.Message.ToString() );

                return false;
            }
        }
        static void checkSearch(string destServerIp, int port)

        {

            TcpClient oClient = new TcpClient();

            try

            {

                oClient.Connect(destServerIp, port);

                NetworkStream ns = oClient.GetStream();

                Console.WriteLine(read(ns));

                //write(ns, "telnetUser");

                //txtResults.Text += Environment.NewLine + read(ns);

                //write(ns, "telnetPassword");

                //txtResults.Text += Environment.NewLine + read(ns);

                //write(ns, Environment.NewLine);

                //txtResults.Text += Environment.NewLine + read(ns);

                ns.Close();

                oClient.Close();

            }

            catch (Exception ex)

            {

                Console.WriteLine(Environment.NewLine + ex.Message);

                if (oClient.Connected)

                    oClient.Close();

            }

        }

        static string read(NetworkStream ns)

        {

            StringBuilder sb = new StringBuilder();

            if (ns.CanRead)

            {

                byte[] readBuffer = new byte[1024];

                int numBytesRead = 0;

                do

                {

                    numBytesRead = ns.Read(readBuffer, 0, readBuffer.Length);

                    //sb.Append(readBuffer[0].ToString);

                    sb.AppendFormat("{0}", Encoding.ASCII.GetString(readBuffer, 0, numBytesRead));

                    sb.Replace(Convert.ToChar(24), ' ');

                    sb.Replace(Convert.ToChar(255), ' ');

                    sb.Replace('?', ' ');

                }

                while (ns.DataAvailable);

            }

            return sb.ToString();

        }

        static void write(NetworkStream ns, string message)

        {

            byte[] msg = Encoding.ASCII.GetBytes(message + Environment.NewLine);

            ns.Write(msg, 0, msg.Length);

        }
    }
}
