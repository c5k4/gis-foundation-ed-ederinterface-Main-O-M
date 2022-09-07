// ========================================================================
// Copyright © 2021 PGE.
// <history>
// PGE DB Password Management (Main Console Application)
// TCS S2NN (EDGISREARC-638) 06/16/2021                             Created
// </history>
// All rights reserved.
// ========================================================================
using System;
using System.Configuration;
using System.IO;
using System.Linq;

namespace PGE_DBPasswordManagement
{
    /// <summary>
    /// Main Class to access Password Management Console
    /// </summary>
    public class Program
    {
        private static Configuration pConfiguration = default(Configuration);
        //V3SF Config Change (11-Jan-2022) [START]
        internal static string ROCONNECTIONPATH = "CONNECTIONPATH";
        internal static string WCONNECTIONPATH = "WCONNECTIONPATH";
        //V3SF Config Change (11-Jan-2022) [END]
        /// <summary>
        /// Constructor to Initialize objects/read App config
        /// </summary>
        public Program()
        {
            Initialize();
        }

        static private void Initialize()
        {
            ExeConfigurationFileMap objExeConfigMap = default;
            try
            {
                objExeConfigMap = new ExeConfigurationFileMap();
                //                objExeConfigMap.ExeConfigFilename = @"PGE_DBPasswordManagement.exe.config";
                if (File.Exists(@"D:\Program Files (x86)\Miner and Miner\PG&E Custom Components\PGE.PasswordMgmt\PGE_DBPasswordManagement.exe.config"))
                    objExeConfigMap.ExeConfigFilename = @"D:\Program Files (x86)\Miner and Miner\PG&E Custom Components\PGE.PasswordMgmt\PGE_DBPasswordManagement.exe.config";
                else if (File.Exists(@"C:\Program Files (x86)\Miner and Miner\PG&E Custom Components\PGE.PasswordMgmt\PGE_DBPasswordManagement.exe.config"))
                    objExeConfigMap.ExeConfigFilename = @"C:\Program Files (x86)\Miner and Miner\PG&E Custom Components\PGE.PasswordMgmt\PGE_DBPasswordManagement.exe.config";

                pConfiguration = ConfigurationManager.OpenMappedExeConfiguration(objExeConfigMap, ConfigurationUserLevel.None);
                if (!pConfiguration.HasFile)
                {
                    throw new Exception("Config File not found. Exiting");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to Initialize PGE_DBPasswordManagement with error :: " + ex.Message + " at (" + ex.StackTrace + ")");
            }
        }

        /// <summary>
        /// Randomizer
        /// </summary>
        static Random pRandom = new Random();

        /// <summary>
        /// Main function for console
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Initialize();

            if (args.Length == 0 || args[0].ToUpper() == "HELP")
            {
                Console.WriteLine("Use this utility to generate password for Database users");
                Help();
                return;
            }
            if (args.Length < 1 || args.Length > 2 && args[0].ToUpper() != "FORMAT")
            {
                Console.WriteLine("Enter valid option and pass correct parameter");
                Help();
                return;
            }
            switch (args[0].ToUpper())
            {
                case "GENPASSWORDF":
                    GENPASSWORDF(args);
                    break;
                case "GENPASSWORD":
                    GENPASSWORD(args);
                    break;
                case "ENCRYPT":
                    //V3SF Config Change
                    Console.WriteLine(EncryptionFacade.Encrypt(args[1], pConfiguration));
                    break;
                case "ENCRYPTF":
                    ENCRYPTF(args);
                    break;
                case "DECRYPT":
                    //V3SF Config Change
                    Console.WriteLine(EncryptionFacade.Decrypt(args[1], pConfiguration));
                    break;
                case "DECRYPTF":
                    DECRYPTF(args);
                    break;
                case "FORMAT":
                    ShowFormat();
                    break;
                case "GENSETDBPASSWORD":
                    GENSETDBPASSWORD(args);
                    break;
                case "GETPASSWORD":
                    GETPASSWORD(args);
                    break;
                case "GETSDEFILE":
                    GETSDEFILE(args);
                    break;
                case "ADMINADD":
                    ADMINADD(args);
                    break;
                case "ADMINADDF":
                    ADMINADDF(args);
                    break;
                case "ADMINREMOVE":
                    ADMINREMOVE(args);
                    break;
                case "ADMINREADALL":
                    ADMINREADCONNETIONF();
                    break;
                default:
                    Help();
                    break;
            }
        }

        /// <summary>
        /// Function to add multiple entry to XML
        /// </summary>
        /// <param name="args">arguments passed from console</param>
        private static void ADMINADDF(string[] args)
        {
            if (!File.Exists(args[1])) { Console.WriteLine("Enter valid file path"); return; }
            string[] sAllText = File.ReadAllLines(args[1]);
            string sEncrypt = string.Empty;

            //V3SF Config Change (11-Jan-2022) [START]
            try
            {
                string test = pConfiguration.AppSettings.Settings[Program.WCONNECTIONPATH].Value;
            }
            catch (NullReferenceException)
            {
                Console.WriteLine("Not a valid system to modify Connections.XML");
                return;
            }
            //V3SF Config Change
            Connection conn = new Connection(pConfiguration.AppSettings.Settings[Program.WCONNECTIONPATH].Value, pConfiguration);
            //Connection conn = new Connection(ConfigurationManager.AppSettings["CONNECTIONPATH"]);
            if (File.Exists(pConfiguration.AppSettings.Settings[Program.WCONNECTIONPATH].Value)) File.Move(pConfiguration.AppSettings.Settings[Program.WCONNECTIONPATH].Value, pConfiguration.AppSettings.Settings[Program.WCONNECTIONPATH].Value.ToUpper().Replace(".XML", "_" + DateTime.Today.Day.ToString() + DateTime.Today.Month.ToString() + DateTime.Today.Year.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + ".XML"));
            //V3SF Config Change (11-Jan-2022) [END]
            foreach (string sText in sAllText)
            {
                if (string.IsNullOrEmpty(sText)) continue;

                //V3SF Crash Fix
                if (sText.Split(',').Length != 3)
                {
                    Console.WriteLine("Invallid arguments passed " + sText + "\n");
                    continue;
                }

                Console.WriteLine(conn.ADDXMLTAG(sText.Split(',')[0].ToUpper(), sText.Split(',')[1], sText.Split(',')[2].ToUpper()));
            }

        }

        /// <summary>
        /// Function to add single entry to XML
        /// </summary>
        /// <param name="args">arguments passed from console</param>
        private static void ADMINADD(string[] args)
        {
            //V3SF Crash Fix
            if (args.Length != 2)
            {
                Console.WriteLine("Invallid arguments passed");
                Help();
                return;
            }
            if(args[1].Split('@').Length !=3)
            {
                Console.WriteLine("Invallid arguments passed");
                Help();
                return;
            }
            //V3SF Config Change (11-Jan-2022) [START]
            try
            {
                string test = pConfiguration.AppSettings.Settings[Program.WCONNECTIONPATH].Value;
            }
            catch(NullReferenceException)
            {
                Console.WriteLine("Not a valid system to modify Connections.XML");
                return;
            }
            //V3SF Config Change
            Connection conn = new Connection(pConfiguration.AppSettings.Settings[Program.WCONNECTIONPATH].Value, pConfiguration);
            if (File.Exists(pConfiguration.AppSettings.Settings[Program.WCONNECTIONPATH].Value)) File.Move(pConfiguration.AppSettings.Settings[Program.WCONNECTIONPATH].Value, pConfiguration.AppSettings.Settings[Program.WCONNECTIONPATH].Value.ToUpper().Replace(".XML", "_" + DateTime.Today.Day.ToString() + DateTime.Today.Month.ToString() + DateTime.Today.Year.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + ".XML"));
            //V3SF Config Change (11-Jan-2022) [END]
            //Connection conn = new Connection(ConfigurationManager.AppSettings["CONNECTIONPATH"]);
            Console.WriteLine((conn.ADDXMLTAG(args[1].Split('@')[0].ToUpper(), args[1].Split('@')[1], args[1].Split('@')[2].ToUpper())).Trim().Replace("\n", "").Replace("\r", ""));
        }

        /// <summary>
        /// Function to remove one entry from XML
        /// </summary>
        /// <param name="args">arguments passed from console</param>
        private static void ADMINREMOVE(string[] args)
        {
            //V3SF Crash Fix
            if (args.Length != 2)
            {
                Console.WriteLine("Invallid arguments passed");
                Help();
                return;
            }
            if (args[1].Split('@').Length != 3)
            {
                Console.WriteLine("Invallid arguments passed");
                Help();
                return;
            }

            //V3SF Config Change (11-Jan-2022) [START]
            try
            {
                string test = pConfiguration.AppSettings.Settings[Program.WCONNECTIONPATH].Value;
            }
            catch (NullReferenceException)
            {
                Console.WriteLine("Not a valid system to modify Connections.XML");
                return;
            }

            //V3SF Config Change
            Connection conn = new Connection(pConfiguration.AppSettings.Settings[Program.WCONNECTIONPATH].Value, pConfiguration);
            //Connection conn = new Connection(ConfigurationManager.AppSettings["CONNECTIONPATH"]);
            if (File.Exists(pConfiguration.AppSettings.Settings[Program.WCONNECTIONPATH].Value)) File.Move(pConfiguration.AppSettings.Settings[Program.WCONNECTIONPATH].Value, pConfiguration.AppSettings.Settings[Program.WCONNECTIONPATH].Value.ToUpper().Replace(".XML", "_" + DateTime.Today.Day.ToString() + DateTime.Today.Month.ToString() + DateTime.Today.Year.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + ".XML"));
            //V3SF Config Change (11-Jan-2022) [END]
            Console.WriteLine(conn.REMOVEXMLTAG(args[1].Split('@')[0].ToUpper(), args[1].Split('@')[1], args[1].Split('@')[2].ToUpper()));
        }

        /// <summary>
        /// Function to get SDE file created
        /// </summary>
        /// <param name="args">arguments passed from console</param>
        private static void GETSDEFILE(string[] args)
        {
            //V3SF Config Change
            //V3SF Config Change (11-Jan-2022)
            Connection conn = new Connection(pConfiguration.AppSettings.Settings[Program.ROCONNECTIONPATH].Value, pConfiguration);
            //Connection conn = new Connection(ConfigurationManager.AppSettings["CONNECTIONPATH"]);
            Console.Write(conn.getSDEFile(args[1].Split('@')[0].ToUpper(), conn.getPassword(args[1].Split('@')[0].ToUpper(), args[1].Split('@')[1].ToUpper()), args[1].Split('@')[1].ToUpper()).Trim().Replace("\n", "").Replace("\r", "").Replace("\\\\","\\"));
        }

        /// <summary>
        /// Function to get SDE file created
        /// </summary>
        /// <param name="args">arguments passed from console</param>
        public string RetGETSDEFILE(string args)
        {
            //V3SF Config Change
            //V3SF Config Change (11-Jan-2022)
            Connection conn = new Connection(pConfiguration.AppSettings.Settings[Program.ROCONNECTIONPATH].Value, pConfiguration);
            //Connection conn = new Connection(ConfigurationManager.AppSettings["CONNECTIONPATH"]);
            return (conn.getSDEFile(args.Split('@')[0].ToUpper(), conn.getPassword(args.Split('@')[0].ToUpper(), args.Split('@')[1].ToUpper()), args.Split('@')[1].ToUpper()).Trim().Replace("\n", "").Replace("\r", ""));
        }

        /// <summary>
        /// Function to get password from xml
        /// </summary>
        /// <param name="args">arguments passed from console</param>
        private static void GETPASSWORD(string[] args)
        {
            //V3SF Config Change
            //V3SF Config Change (11-Jan-2022)
            Connection conn = new Connection(pConfiguration.AppSettings.Settings[Program.ROCONNECTIONPATH].Value, pConfiguration);
            //Connection conn = new Connection(ConfigurationManager.AppSettings["CONNECTIONPATH"]);
            Console.Write(conn.getPassword(args[1].Split('@')[0].ToUpper(), args[1].Split('@')[1].ToUpper()).Trim().Replace("\n", "").Replace("\r", ""));
        }

        /// <summary>
        /// V3SF
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public string RetGETPASSWORD(string args)
        {
            //V3SF Config Change
            //return "Test";
            //return pConfiguration.AppSettings.Settings["CONNECTIONPATH"].Value;
            //V3SF Config Change (11-Jan-2022)
            Connection conn = new Connection(pConfiguration.AppSettings.Settings[Program.ROCONNECTIONPATH].Value, pConfiguration);
            //Connection conn = new Connection(ConfigurationManager.AppSettings["CONNECTIONPATH"]);
            return Convert.ToString(conn.getPassword(args.Split('@')[0].ToUpper(), args.Split('@')[1].ToUpper()).Trim().Replace("\n", "").Replace("\r", ""));
        }

        /// <summary>
        /// Function to generate set password
        /// </summary>
        /// <param name="args">arguments passed from console</param>
        private static void GENSETDBPASSWORD(string[] args)
        {
            if (!File.Exists(args[1])) { Console.WriteLine("Enter valid file path"); return; }
            string[] sAllUsersS = File.ReadAllLines(args[1]);
            string sNewPassFS = string.Empty,
                sPassSetQuery = string.Empty;

            //sPassSetQuery += "sqlplus /nolog" + Environment.NewLine;

            foreach (string sUser in sAllUsersS)
            {
                //V3SF Config Change
                if (string.IsNullOrEmpty(sUser)) continue;
                string sSchemaF = sUser.Split(',')[2].ToLower(),
                    sUserNameF = sUser.Split(',')[3].ToUpper(),
                    sLoginUser = sUser.Split(',')[0].ToUpper(),
                    sLoginPass = sUser.Split(',')[1],
                    sPasswordF = sUserNameF.Substring(0, 3) + RandomChar(1) + RandomString(3) + RandomDigit(2) + RandomChar(1) + sSchemaF.Substring(sSchemaF.Length - 5),
                    sPassEncryptF = EncryptionFacade.Encrypt(sPasswordF, pConfiguration);
                sNewPassFS += sSchemaF + "," + sUserNameF + "," + sPasswordF + "," + sPassEncryptF + Environment.NewLine;
                sPassSetQuery += "conn " + sLoginUser + "/" + sLoginPass + "@" + sSchemaF;
                sPassSetQuery += Environment.NewLine;
                sPassSetQuery += "ALTER USER " + sUserNameF + " IDENTIFIED BY \"" + sPasswordF + "\";";
                sPassSetQuery += Environment.NewLine;
            }
            sPassSetQuery += "exit;";
            sPassSetQuery += Environment.NewLine;

            if (File.Exists((new FileInfo(args[1])).DirectoryName + "\\PGE_DBPasswordManagementSet_" + (new FileInfo(args[1])).Name)) File.Delete((new FileInfo(args[1])).DirectoryName + "\\PGE_DBPasswordManagementSet_" + (new FileInfo(args[1])).Name);
            File.AppendAllText((new FileInfo(args[1])).DirectoryName + "\\PGE_DBPasswordManagementSet_" + (new FileInfo(args[1])).Name, sNewPassFS);
            if (File.Exists(Path.ChangeExtension((new FileInfo(args[1])).DirectoryName + "\\PGE_DBPasswordManagementSetQ_" + (new FileInfo(args[1])).Name, ".sql"))) File.Delete(Path.ChangeExtension((new FileInfo(args[1])).DirectoryName + "\\PGE_DBPasswordManagementSetQ_" + (new FileInfo(args[1])).Name, ".sql"));
            File.AppendAllText(Path.ChangeExtension((new FileInfo(args[1])).DirectoryName + "\\PGE_DBPasswordManagementSetQ_" + (new FileInfo(args[1])).Name, ".sql"), sPassSetQuery);
            //File.Move((new FileInfo(args[1])).DirectoryName + "\\PGE_DBPasswordManagementSetQ_" + (new FileInfo(args[1])).Name, Path.ChangeExtension((new FileInfo(args[1])).DirectoryName + "\\PGE_DBPasswordManagementSetQ_" + (new FileInfo(args[1])).Name, ".sql"));
            // string cmdArgs = "sqlplus /nolog @\"" + Path.ChangeExtension((new FileInfo(args[1])).DirectoryName + "\\PGE_DBPasswordManagementSetQ_" + (new FileInfo(args[1])).Name, ".sql\"");                    
        }

        /// <summary>
        /// 
        /// </summary>
        private static void ADMINREADCONNETIONF()
        {
            //V3SF Config Change (11-Jan-2022)
            Connection conn = new Connection(ConfigurationManager.AppSettings[Program.ROCONNECTIONPATH], pConfiguration);
            foreach (Credential cred in conn.connections.Credential)
            {
                Console.WriteLine(EncryptionFacade.Decrypt(cred.Username, pConfiguration) + "," + EncryptionFacade.Decrypt(cred.Password, pConfiguration) + "," + EncryptionFacade.Decrypt(cred.Instance, pConfiguration));
            }
        }

        /// <summary>
        /// Decypt encrypted text
        /// </summary>
        /// <param name="args">arguments passed from console</param>
        private static void DECRYPTF(string[] args)
        {
            if (!File.Exists(args[1])) { Console.WriteLine("Enter valid file path"); return; }
            string[] sAllText = File.ReadAllLines(args[1]);
            string sDecrypt = string.Empty;
            foreach (string sText in sAllText)
            {
                if (string.IsNullOrEmpty(sText)) continue;
                sDecrypt += EncryptionFacade.Decrypt(sText, pConfiguration) + Environment.NewLine;
            }



            if (File.Exists((new FileInfo(args[1])).DirectoryName + "\\Decrypt_" + (new FileInfo(args[1])).Name)) File.Delete((new FileInfo(args[1])).DirectoryName + "\\Decrypt_" + (new FileInfo(args[1])).Name);
            File.AppendAllText((new FileInfo(args[1])).DirectoryName + "\\Decrypt_" + (new FileInfo(args[1])).Name, sDecrypt);

        }
        //private static void DECRYPTF(string[] args)
        //{
        //    if (!File.Exists(args[1])) { Console.WriteLine("Enter valid file path"); return; }
        //    string[] sAllText = File.ReadAllLines(args[1]);
        //    string sDecrypt = string.Empty;
        //    foreach (string sText in sAllText)
        //    {
        //        if (string.IsNullOrEmpty(sText)) continue;
        //        //V3SF Config Change
        //        sDecrypt += EncryptionFacade.Encrypt(sText, pConfiguration) + Environment.NewLine;
        //    }

        //    if (File.Exists((new FileInfo(args[1])).DirectoryName + "\\Decrypt_" + (new FileInfo(args[1])).Name)) File.Delete((new FileInfo(args[1])).DirectoryName + "\\Decrypt_" + (new FileInfo(args[1])).Name);
        //    File.AppendAllText((new FileInfo(args[1])).DirectoryName + "\\Decrypt_" + (new FileInfo(args[1])).Name, sDecrypt);

        //}

        /// <summary>
        /// Encrypt clear text
        /// </summary>
        /// <param name="args">arguments passed from console</param>
        private static void ENCRYPTF(string[] args)
        {
            if (!File.Exists(args[1])) { Console.WriteLine("Enter valid file path"); return; }
            string[] sAllText = File.ReadAllLines(args[1]);
            string sEncrypt = string.Empty;
            foreach (string sText in sAllText)
            {
                if (string.IsNullOrEmpty(sText)) continue;
                //V3SF Config Change
                sEncrypt += EncryptionFacade.Encrypt(sText, pConfiguration) + Environment.NewLine;
            }

            if (File.Exists((new FileInfo(args[1])).DirectoryName + "\\Encrypt_" + (new FileInfo(args[1])).Name)) File.Delete((new FileInfo(args[1])).DirectoryName + "\\Encrypt_" + (new FileInfo(args[1])).Name);
            File.AppendAllText((new FileInfo(args[1])).DirectoryName + "\\Encrypt_" + (new FileInfo(args[1])).Name, sEncrypt);

        }

        /// <summary>
        /// Generate password text
        /// </summary>
        /// <param name="args">arguments passed from console</param>
        private static void GENPASSWORD(string[] args)
        {
            if (!args[1].Contains('@') || args[1].Split('@').Length != 2) { Console.WriteLine("Enter valid value for <USERNAME>@<DBINSTANCENAME>"); return; }
            string sSchemaS = args[1].Split('@')[1].ToLower(),
                   sUserNameS = args[1].Split('@')[0].ToUpper(),
             sPasswordS = sUserNameS.Substring(0, 3) + RandomChar(1) + RandomString(5) + RandomDigit(2) + RandomChar(1) + (sUserNameS + sSchemaS).Substring((sUserNameS + sSchemaS).Length - 5),
               sPassEncryptS = EncryptionFacade.Encrypt(sPasswordS, pConfiguration);
            if (sUserNameS.Length == 4)
                Console.WriteLine(sPasswordS);
            else
            {
                Console.WriteLine("Password (Text) for " + args[1] + ": " + sPasswordS);
                Console.WriteLine("Password (Encrypted) for " + args[1] + ": " + sPassEncryptS);
            }
            //Console.WriteLine();
            //Console.WriteLine("Press Enter to continue...");
            //Console.ReadLine();
        }
        //private static void GENPASSWORD(string[] args)
        //{

        //    if (!args[1].Contains('@') || args[1].Split('@').Length != 2) { Console.WriteLine("Enter valid value for <USERNAME>@<DBINSTANCENAME>"); return; }
        //    //V3SF Config Change
        //    string sSchemaS = args[1].Split('@')[1].ToLower(),
        //           sUserNameS = args[1].Split('@')[0].ToUpper(),
        //           sPasswordS = sUserNameS.Substring(0, 3) + RandomChar(1) + RandomString(5) + RandomDigit(2) + RandomChar(1) + sSchemaS.Substring(sSchemaS.Length - 5),
        //           sPassEncryptS = EncryptionFacade.Encrypt(sPasswordS, pConfiguration);

        //    if (sUserNameS.Length == 4)
        //        Console.WriteLine(sPasswordS);
        //    else
        //    {
        //        Console.WriteLine("Password (Text) for " + args[1] + ": " + sPasswordS);
        //        Console.WriteLine("Password (Encrypted) for " + args[1] + ": " + sPassEncryptS);
        //    }
        //    //Console.WriteLine();
        //    //Console.WriteLine("Press Enter to continue...");
        //    //Console.ReadLine();
        //}



        /// <summary>
        /// Generate password text
        /// </summary>
        /// <param name="args">arguments passed from console</param>
        public string RetGENPASSWORD(string args)
        {

            if (!args.Contains('@') || args.Split('@').Length != 2) { throw new Exception("Enter valid value for <USERNAME>@<DBINSTANCENAME>"); }
            //V3SF Config Change
            string sSchemaS = args.Split('@')[1].ToLower(),
                   sUserNameS = args.Split('@')[0].ToUpper(),
                   sPasswordS = sUserNameS.Substring(0, 3) + RandomChar(1) + RandomString(5) + RandomDigit(2) + RandomChar(1) + sSchemaS.Substring(sSchemaS.Length - 5),
                   sPassEncryptS = EncryptionFacade.Encrypt(sPasswordS, pConfiguration);

            if (sUserNameS.Length == 4)
                return (sPasswordS);
            else
            {
                return (sPasswordS);
                //return("Password (Encrypted) for " + args[1] + ": " + sPassEncryptS);
            }
            //Console.WriteLine();
            //Console.WriteLine("Press Enter to continue...");
            //Console.ReadLine();
        }

        /// <summary>
        /// Function to generate multiple passwords
        /// </summary>
        /// <param name="args">arguments passed from console</param>
        private static void GENPASSWORDF(string[] args)
        {
            if (!File.Exists(args[1])) { Console.WriteLine("Enter valid file path"); return; }
            string[] sAllUsers = File.ReadAllLines(args[1]);
            string sNewPassF = string.Empty;
            foreach (string sUser in sAllUsers)
            {
                string sUserNameF = sUser.Split(',')[0].ToLower(),
                    sSchemaF = sUser.Split(',')[1].ToUpper(),
                    sPasswordF = sUserNameF.Substring(0, 3) + RandomChar(1) + RandomString(3) + RandomDigit(2) + RandomChar(1) + sSchemaF.Substring(sSchemaF.Length - 5),
                    sPassEncryptF = EncryptionFacade.Encrypt(sPasswordF, pConfiguration);
                sNewPassF += sUserNameF + "," + sSchemaF + "," + sPasswordF + "," + sPassEncryptF + Environment.NewLine;
            }
            if (File.Exists((new FileInfo(args[1])).DirectoryName + "\\PGE_DBPasswordManagement_" + (new FileInfo(args[1])).Name)) File.Delete((new FileInfo(args[1])).DirectoryName + "\\PGE_DBPasswordManagement_" + (new FileInfo(args[1])).Name);
            File.AppendAllText((new FileInfo(args[1])).DirectoryName + "\\PGE_DBPasswordManagement_" + (new FileInfo(args[1])).Name, sNewPassF);
        }

        /// <summary>
        /// Function to show password format
        /// </summary>
        /// <param name="args">arguments passed from console</param>
        private static void ShowFormat()
        {
            // sPasswordF = sUserNameF.Substring(0, 3) + RandomChar(1) + RandomString(3) + RandomDigit(2) + RandomChar(1) + sSchemaF.Substring(sSchemaF.Length - 5),
            Console.WriteLine("Below is the format of password generated");
            Console.WriteLine("First 3 chars of username in UPPERCASE");
            Console.WriteLine("1 random special character");
            Console.WriteLine("3 random characters in uppercase and/or lowercase");
            Console.WriteLine("2 random digits");
            Console.WriteLine("1 random special character");
            Console.WriteLine("Last 5 chars of instance name in lowercase");
            Console.WriteLine("Total password length is 15 characters with upper and lowercase, special characters and numbers");
        }
        static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[pRandom.Next(s.Length)]).ToArray());
        }
        static string RandomDigit(int length)
        {
            const string chars = "0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[pRandom.Next(s.Length)]).ToArray());
        }
        static string RandomChar(int length)
        {
            const string chars = "!_#$%+*";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[pRandom.Next(s.Length)]).ToArray());
        }
        static void Help()
        {
            Console.WriteLine("PGE_DBPasswordManagement HELP");
            Console.WriteLine();

            Console.WriteLine("Below are the options available");
            Console.WriteLine();
            Console.WriteLine("Use this option if Single password need to be generated first value as UserName and second value as DB Instance");
            Console.WriteLine("\t\t PGE_DBPasswordManagement GENPASSWORD <USERNAME>@<DBINSTANCENAME>");
            Console.WriteLine();
            Console.WriteLine("Use this option if multiple passwords need to be generated. Provide path of CSV (without Header) with USERNAME,INSTANCE format and this tool will generate a CSV file in same path with PGE_DBPasswordManagement_ as prefix having USERNAME,INSTACE, Password Text, Password Encryption");
            Console.WriteLine("\t\t PGE_DBPasswordManagement GENPASSWORDF <CSVFileFullPath>");
            //Console.WriteLine();
            //Console.WriteLine("Use this option to generate and set password to database. A file is a must to be provided even for singular case. File must be a CSV without header and first column as LOGIN ADMIN NAME and Second column must LOGIN ADMIN Password. Third Column should be DB Instance and Last column as UserName for which password need to be generated and set to database. This Utility will generate a CSV file in same path with PGE_DBPasswordManagement_ as prefix having password text and encryption ");
            //Console.WriteLine("\t\t PGE_DBPasswordManagement GENSETDBPASSWORD <CSVFileFullPath>");
            Console.WriteLine();
            Console.WriteLine("Use this option to Encrypt any existing Text.  Use everything in CAPSLOCK except passwords");
            Console.WriteLine("\t\t PGE_DBPasswordManagement ENCRYPT <PasswordText>");
            Console.WriteLine();
            Console.WriteLine("Use this option to Encrypt all Text in a file for each in new line. Use everything in CAPSLOCK except passwords");
            Console.WriteLine("\t\t PGE_DBPasswordManagement ENCRYPTF <FilePath>");
            Console.WriteLine();
            Console.WriteLine("Use this option to Decrypt any existing encryption");
            Console.WriteLine("\t\t PGE_DBPasswordManagement DECRYPT <PasswordEncryption>");
            Console.WriteLine();
            Console.WriteLine("Use this option to Decrypt all encrypted text in a file for each in new line");
            Console.WriteLine("\t\t PGE_DBPasswordManagement DECRYPTF <FilePath>");
            Console.WriteLine();
            Console.WriteLine("Use this option for checking on password format");
            Console.WriteLine("\t\t PGE_DBPasswordManagement GETPASSWORD <USERNAME>@<DBINSTANCENAME>");
            Console.WriteLine();
            Console.WriteLine("Use this option to get SDE file created and get path as return value");
            Console.WriteLine("\t\t PGE_DBPasswordManagement GETSDEFILE <USERNAME>@<DBINSTANCENAME>");
            Console.WriteLine();
            Console.WriteLine("Use this option to add single new entry to Connections.xml file");
            Console.WriteLine("\t\t PGE_DBPasswordManagement ADMINADD <USERNAME>@\"<Password>\"@<DBINSTANCENAME>");
            Console.WriteLine();
            Console.WriteLine("Use this option to add multple new entry to Connections.xml file. Pass path of CSV without Header with USERNAME,Password,INSTANCE");
            Console.WriteLine("\t\t PGE_DBPasswordManagement ADMINADDF <FilePath>");
            Console.WriteLine();
            Console.WriteLine("Use this option to remove one entry from Connections.xml file");
            Console.WriteLine("\t\t PGE_DBPasswordManagement ADMINREMOVE <USERNAME>@<Password>@<DBINSTANCENAME>");
            Console.WriteLine();
            Console.WriteLine("Use PGE_DBPasswordManagement FORMAT for checking on password format");
            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
        }
    }
}
