// ========================================================================
// Copyright © 2021 PGE.
// <history>
// PGE DB Password Management (Connections Classes and functions)
// TCS S2NN (EDGISREARC-638) 06/16/2021                             Created
// </history>
// All rights reserved.
// ========================================================================
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace PGE_DBPasswordManagement
{
    /// <summary>
    /// Class object to Connections.xml file
    /// </summary>
    [XmlRoot(ElementName = "Credential")]
    public class Credential
    {
        /// <summary>
        /// Instance of the DB Connection
        /// </summary>
        [XmlElement(ElementName = "Instance")]
        public string Instance { get; set; }

        /// <summary>
        /// Username of the DB Connection
        /// </summary>
        [XmlElement(ElementName = "Username")]
        public string Username { get; set; }

        /// <summary>
        /// Password of the DB Connection
        /// </summary>
        [XmlElement(ElementName = "Password")]
        public string Password { get; set; }
    }

    /// <summary>
    /// Class to define all Connections
    /// </summary>
    [XmlRoot(ElementName = "Connections")]
    public class Connections
    {
        /// <summary>
        /// Collection of Connections
        /// </summary>
        [XmlElement(ElementName = "Credential")]
        public List<Credential> Credential { get; set; }
    }

    /// <summary>
    /// Class to define all Connections related functions
    /// </summary>
    public class Connection
    {
        /// <summary>
        /// Object for Connections
        /// </summary>
        public Connections connections;

        /// <summary>
        /// (V3SF) Configration Reference
        /// </summary>
        private Configuration pConfiguration = default(Configuration);

        /// <summary>
        /// Constructor for initializing Connection and reading Connections.xml
        /// </summary>
        /// <param name="sConnectionsPath"></param>
        /// <param name="pConfiguration"></param>
        public Connection(string sConnectionsPath, Configuration pConfiguration)
        {
            this.pConfiguration = pConfiguration;
            StreamReader file = new StreamReader(sConnectionsPath);
            connections = (Connections)(new XmlSerializer(typeof(Connections))).Deserialize(file);
            file.Close();
        }

        /// <summary>
        /// Function return clear text Password
        /// </summary>
        /// <param name="Username">Username for which password is required</param>
        /// <param name="Instance">Instance for which password is required</param>
        /// <returns>Clear Password string</returns>
        public string getPassword(string Username, string Instance)
        {
            try
            {
                //V3SF Config Change
                Credential credential = connections.Credential.First(cred => cred.Username == EncryptionFacade.Encrypt(Username, pConfiguration) && cred.Instance == EncryptionFacade.Encrypt(Instance, pConfiguration));
                return EncryptionFacade.Decrypt(credential.Password, pConfiguration);
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Create SDE files
        /// </summary>
        /// <param name="username">Username to create SDE file</param>
        /// <param name="password">Password to create SDE file</param>
        /// <param name="instance">Instance to create SDE file</param>
        /// <returns>Path for the newly created SDE path</returns>
        public string getSDEFile(string username, string password, string instance)
        {
            string sdepath = Path.GetTempPath() + username + "_" + instance + "_" + DateTime.Today.Month + "_" + DateTime.Today.Day + "_" + DateTime.Today.Year + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second + "_" + DateTime.Now.Millisecond + ".sde";
            sdepath = sdepath.Replace("\\","\\\\");
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = pConfiguration.AppSettings.Settings["ARCPYPATH"].Value;
            //start.FileName = ConfigurationManager.AppSettings["ARCPYPATH"];
            start.Arguments = string.Format("\"{0}\" {1}", pConfiguration.AppSettings.Settings["SDEPYTHONPATH"].Value, "\"" + instance + "\"" + " " + "\"" + username + "\"" + " " + "\"" + password + "\"" + " " + "\"" + sdepath + "\"");
            //start.Arguments = string.Format("{0} {1}", ConfigurationManager.AppSettings["SDEPYTHONPATH"], "\"" + instance + "\"" + " " + "\"" + username + "\"" + " " + "\"" + password + "\"" + " " + "\"" + sdepath + "\"");
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            start.CreateNoWindow = true;
            using (Process process = Process.Start(start))
            {
                process.WaitForExit();
                using (StreamReader reader = process.StandardOutput)
                {
                    if (File.Exists(sdepath))
                        return sdepath;
                    else
                        return string.Empty;
                }
            }
        }

        /// <summary>
        /// Remove a XML entry from Connection.xml
        /// </summary>
        /// <param name="username">Username for tag to be removed</param>
        /// <param name="password">Password for tag to be removed in clear text</param>
        /// <param name="instance">Instance for tag to be removed</param>
        /// <returns>Remove the provided tag information from XML</returns>
        public string REMOVEXMLTAG(string username, string password, string instance)
        {
            //V3SF - Fix Crash Issue
            //int i = connections.Credential.RemoveAll(cred => cred.Username == EncryptionFacade.Encrypt(username,pConfiguration) && cred.Password == EncryptionFacade.Encrypt(password,pConfiguration) && cred.Instance == EncryptionFacade.Encrypt(instance,pConfiguration));
            int i = 0;

            try
            {
                Credential credential = new Credential() { Username = EncryptionFacade.Encrypt(username, pConfiguration), Instance = EncryptionFacade.Encrypt(instance, pConfiguration), Password = EncryptionFacade.Encrypt(password, pConfiguration) };
                if (!string.IsNullOrWhiteSpace(credential.Username) && !string.IsNullOrWhiteSpace(credential.Password) && !string.IsNullOrWhiteSpace(credential.Instance))
                {
                    i = connections.Credential.RemoveAll(cred => cred.Username == credential.Username && cred.Password == credential.Password && cred.Instance == credential.Instance);
                }
            }
            catch { }

            using (var stringwriter = new StringWriter())
            {
                (new XmlSerializer(typeof(Connections))).Serialize(stringwriter, connections);

                //V3SF Config Change
                //if (File.Exists(pConfiguration.AppSettings.Settings["CONNECTIONPATH"].Value)) File.Move(pConfiguration.AppSettings.Settings["CONNECTIONPATH"].Value, pConfiguration.AppSettings.Settings["CONNECTIONPATH"].Value.ToUpper().Replace(".XML", "_" + DateTime.Today.Day.ToString() + DateTime.Today.Month.ToString() + DateTime.Today.Year.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + ".XML"));
                //V3SF Config Change (11-Jan-2022)
                File.WriteAllText(pConfiguration.AppSettings.Settings[Program.WCONNECTIONPATH].Value, stringwriter.ToString());

                //if (File.Exists(ConfigurationManager.AppSettings["CONNECTIONPATH"])) File.Move(ConfigurationManager.AppSettings["CONNECTIONPATH"], ConfigurationManager.AppSettings["CONNECTIONPATH"].ToUpper().Replace(".XML", "_" + DateTime.Today.Day.ToString() + DateTime.Today.Month.ToString() + DateTime.Today.Year.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + ".XML"));
                //File.WriteAllText(ConfigurationManager.AppSettings["CONNECTIONPATH"], stringwriter.ToString());
            }
            return "Removed " + i.ToString();
        }

        /// <summary>
        /// Add a XML entry to Connection.xml
        /// </summary>
        /// <param name="username">Username for tag to be added</param>
        /// <param name="password">Password for tag to be added in clear text</param>
        /// <param name="instance">Instance for tag to be added</param>
        /// <returns>Added the provided tag information to XML in encypted format</returns>
        public string ADDXMLTAG(string username, string password, string instance)
        {
            //V3SF Config Change
            string returnString = string.Empty;
            try
            {
                Credential credential = new Credential() { Username = EncryptionFacade.Encrypt(username, pConfiguration), Instance = EncryptionFacade.Encrypt(instance, pConfiguration), Password = EncryptionFacade.Encrypt(password, pConfiguration) };
                if (!string.IsNullOrWhiteSpace(credential.Username) && !string.IsNullOrWhiteSpace(credential.Password) && !string.IsNullOrWhiteSpace(credential.Instance))
                {
                    connections.Credential.Add(credential);
                    returnString = "ADDED";
                }
                //connections.Credential.Add(new Credential() { Username = EncryptionFacade.Encrypt(username, pConfiguration), Instance = EncryptionFacade.Encrypt(instance, pConfiguration), Password = EncryptionFacade.Encrypt(password, pConfiguration) });
            }
            catch { }

            using (var stringwriter = new StringWriter())
            {
                (new XmlSerializer(typeof(Connections))).Serialize(stringwriter, connections);

                //V3SF Config Change
                //if (File.Exists(pConfiguration.AppSettings.Settings["CONNECTIONPATH"].Value)) File.Move(pConfiguration.AppSettings.Settings["CONNECTIONPATH"].Value, pConfiguration.AppSettings.Settings["CONNECTIONPATH"].Value.ToUpper().Replace(".XML", "_" + DateTime.Today.Day.ToString() + DateTime.Today.Month.ToString() + DateTime.Today.Year.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + ".XML"));
                //V3SF Config Change (11-Jan-2022)
                File.WriteAllText(pConfiguration.AppSettings.Settings[Program.WCONNECTIONPATH].Value, stringwriter.ToString());

                //if (File.Exists(ConfigurationManager.AppSettings["CONNECTIONPATH"])) File.Move(ConfigurationManager.AppSettings["CONNECTIONPATH"], ConfigurationManager.AppSettings["CONNECTIONPATH"].ToUpper().Replace(".XML", "_" + DateTime.Today.Day.ToString() + DateTime.Today.Month.ToString() + DateTime.Today.Year.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + ".XML"));
                //File.WriteAllText(ConfigurationManager.AppSettings["CONNECTIONPATH"], stringwriter.ToString());
            }
            return returnString;
            //return "ADDED";
        }
    }
}