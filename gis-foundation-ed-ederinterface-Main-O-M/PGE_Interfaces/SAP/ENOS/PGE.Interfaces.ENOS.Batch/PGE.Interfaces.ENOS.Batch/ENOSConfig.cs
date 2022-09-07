using System;
using System.Collections; 
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Runtime.InteropServices;
using System.Configuration;
using System.Reflection;

namespace PGE.Interface.ENOS.Batch
{
    public class ENOSConfig
    {
        private string _logFileName;
        private int _maxArchiveAge = -1;
        private string _editUser;
        private string _editUserPassword;
        private string _postUser;
        private string _postUserPassword;
        private string _servicePrefix;
        private string _service;
        private string _targetVersion;
        private string _reconcileVersion;
        //private string _sdeConnectionFile;
        //private string _editConnectionFile; 
        private bool _reconcileAquireLock;
        private bool _reconcileAbortIfConflicts;
        private bool _reconcileChildWins;
        private bool _reconcileColumnLevel;
        private bool _postAfterReconcile;
        private bool _deleteENOSStage; 
        //private string _oracle_Connect_String; 
        private ChildProcess[] _childProcesses; 

        public string LogfileName
        {
            get
            {
                return _logFileName;
            }
        }

        public int MaxArchiveAge
        {
            get
            {
                return _maxArchiveAge;
            }
        }

        //public string SDEConnectionFile
        //{
        //    get
        //    {
        //        return _sdeConnectionFile;
        //    }
        //}

        //public string EditConnectionFile
        //{
        //    get
        //    {
        //        return _editConnectionFile;
        //    }
        //}

        public string EditUser
        {
            get
            {
                return _editUser;
            }
        }

        public string EditUserPassword
        {
            get
            {
                return _editUserPassword;
            }
        }

        public string PostUser
        {
            get
            {
                return _postUser;
            }
        }

        public string PostUserPassword
        {
            get
            {
                return _postUserPassword;
            }
        }

        public string ServicePrefix
        {
            get
            {
                return _servicePrefix;
            }
        }

        public string Service
        {
            get
            {
                return _service;
            }
        }

        public string TargetVersion
        {
            get
            {
                return _targetVersion;
            }
        }

        public string ReconcileVersion
        {
            get
            {
                return _reconcileVersion; 
            }
        }

        public bool ReconcileAcquireLock
        {
            get
            {
                return _reconcileAquireLock;
            }
        }

        public bool ReconcileAbortIfConflicts
        {
            get
            {
                return _reconcileAbortIfConflicts;
            }
        }

        public bool ReconcileChildWins
        {
            get
            {
                return _reconcileChildWins;
            }
        }

        public bool ReconcileColumnLevel
        {
            get
            {
                return _reconcileColumnLevel;
            }
        }

        public bool PostAfterReconcile
        {
            get
            {
                return _postAfterReconcile;
            }
        }

        public bool DeleteENOSStage
        {
            get
            {
                return _deleteENOSStage;
            }
        }

        public ChildProcess[] ChildProcesses
        {
            get
            {
                return _childProcesses;
            }
        }

        //public string OracleConnectString
        //{
        //    get
        //    {
        //        return _oracle_Connect_String;
        //    }
        //}

        
        public ENOSConfig()
        {           
            //Load the config document 
            string xPath = string.Empty;
            XmlNode pXMLNode = null;             
            string configFilename = "";
            string installDirectory = new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName;
            configFilename = installDirectory + "\\" + "App.config";
            if (!File.Exists(configFilename))
                throw new Exception("Unable to find config file: " + configFilename);
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(configFilename);
            
            //XPath query for config settings 
            xPath = "//configuration/appSettings/add";
            XmlNodeList pNodeList = xmlDoc.SelectNodes(xPath);
            for (int i = 0; i < pNodeList.Count; i++)
            {
                pXMLNode = pNodeList[i];
                if (pXMLNode != null)
                {
                    switch (pXMLNode.Attributes[0].Value) 
                    {
                        case "LOG_FILENAME":
                            _logFileName = pXMLNode.Attributes[1].Value;
                            break;
                        case "MAX_ARCHIVE_AGE":
                            _maxArchiveAge = Convert.ToInt32(pXMLNode.Attributes[1].Value.Trim());
                            break;
                        case "TARGET_VERSION_NAME":
                            _targetVersion = pXMLNode.Attributes[1].Value.Trim();
                            break;
                        case "RECONCILE_VERSION_NAME":
                            _reconcileVersion = pXMLNode.Attributes[1].Value.Trim();
                            break;
                        case "RECONCILE_ACQUIRE_LOCK":
                            _reconcileAquireLock = Convert.ToBoolean(pXMLNode.Attributes[1].Value.Trim());
                            break;
                        case "RECONCILE_ABORT_IF_CONFLICTS":
                            _reconcileAbortIfConflicts = Convert.ToBoolean(pXMLNode.Attributes[1].Value.Trim());
                            break;
                        case "RECONCILE_CHILD_WINS":
                            _reconcileChildWins = Convert.ToBoolean(pXMLNode.Attributes[1].Value.Trim());
                            break;
                        case "RECONCILE_COLUMN_LEVEL":
                            _reconcileColumnLevel = Convert.ToBoolean(pXMLNode.Attributes[1].Value.Trim());
                            break;
                        case "POST_AFTER_RECONCILE":
                            _postAfterReconcile = Convert.ToBoolean(pXMLNode.Attributes[1].Value.Trim());
                            break;
                        case "DELETE_ENOS_STAGE":
                            _deleteENOSStage = Convert.ToBoolean(pXMLNode.Attributes[1].Value.Trim());
                            break;
                        case "EDIT_USER":
                            _editUser = pXMLNode.Attributes[1].Value.Trim();
                            break;
                        case "EDIT_USER_PASSWORD":
                            _editUserPassword = pXMLNode.Attributes[1].Value.Trim();
                            break;
                        case "POST_USER":
                            _postUser = pXMLNode.Attributes[1].Value.Trim();
                            break;
                        case "POST_USER_PASSWORD":
                            _postUserPassword = pXMLNode.Attributes[1].Value.Trim();
                            break;
                        case "SERVICE_PREFIX":
                            _servicePrefix = pXMLNode.Attributes[1].Value.Trim();
                            break;
                        case "SERVICE":
                            _service = pXMLNode.Attributes[1].Value.Trim();
                            break;
                        default:
                            System.Diagnostics.Debug.Print("Config value unknown!"); 
                            break; 
                    }
                }
            }

            Hashtable hshChildProcesses = new Hashtable(); 
            xPath = "//configuration/appSettings/childprocesses";
            XmlNode pTopNode = xmlDoc.SelectSingleNode(xPath);
            for (int i = 0; i < pTopNode.ChildNodes.Count; i++)
            {
                pXMLNode = pTopNode.ChildNodes.Item(i);
                if (pXMLNode != null)
                {
                    int processIdx = Convert.ToInt32(pXMLNode.InnerText);
                    string lastDigits = pXMLNode.Attributes["last_digits"].Value;
                    if (!hshChildProcesses.ContainsKey(processIdx))
                        hshChildProcesses.Add(processIdx, lastDigits);                    
                }
            }

            //Create the Childprocesses array  
            _childProcesses = new ChildProcess[hshChildProcesses.Count];
            int counter = 0; 
            foreach (int procIndx in hshChildProcesses.Keys)
            {
                ChildProcess pChildProcess = 
                    new ChildProcess(procIndx, (string)hshChildProcesses[procIndx]);  
                _childProcesses[counter] = pChildProcess;
                counter++; 
            }
        }
    }

    public class ChildProcess
    {
        private int _processIndex;
        private string _lastDigits;

        public ChildProcess(int procIdx, string lastDigits)
        {
            _processIndex = procIdx;
            _lastDigits = lastDigits; 
        }

        public string LastDigits
        {
            get
            {
                return _lastDigits;
            }
        }

        public int ProcessIndex
        {
            get
            {
                return _processIndex;
            }
        }
    }

}
