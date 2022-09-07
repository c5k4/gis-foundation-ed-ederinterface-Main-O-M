using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ESRI.ArcGIS.esriSystem;
using Miner.Interop;

namespace PGE.BatchApplication.PerfQA_ArcFMLogin
{
    [Guid("84bac8ba-1cce-48b4-85de-b3b27c9f883d")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("PGE.BatchApplication.PerfQA_ArcFMLogin.PGE.BatchApplication.PerfQA_ArcFMLogin")]
    public class PerfQA_ArcFMLogin : IMMLoginObject, IMMChangeDefaultVersion
    {
        private MMDefaultLoginObjectClass _defaultLogin;
        public PerfQA_ArcFMLogin()
        {
            try
            {
                _defaultLogin = new MMDefaultLoginObjectClass();

                //this.ShowDialog = false;
            }
            catch (Exception ex)
            {
                string s = ex.Message;
                s = s + "_";
            }
        }

        public string GetFullTableName(string bstrBaseTableName)
        {
            return _defaultLogin.GetFullTableName(bstrBaseTableName);
        }

        public bool IsValidLogin
        {
            get
            { return _defaultLogin.IsValidLogin; }
        }

        bool _initialLogin, _loginChanged;

        bool IMMLoginObject.Login(bool vbInitialLogin)
        {
            try
            {
                _initialLogin = vbInitialLogin;
                //System.Windows.Forms.MessageBox.Show("AD");  

                //ArcGISRuntimeEnvironment rte = new ArcGISRuntimeEnvironment();
                //IntPtr hwnd = new IntPtr(rte.hWnd);
                customLogin();
            }
            catch (Exception ex)
            {
                string s = ex.Message;
                s = s + "_";
            }
            return _loginChanged;

        }

        public ESRI.ArcGIS.Geodatabase.IWorkspace LoginWorkspace
        {
            get { return _defaultLogin.LoginWorkspace; }
        }

        public string UserName
        {
            get { return userName; }
        }

        public void ChangeDefaultVersion(ESRI.ArcGIS.Geodatabase.IVersion pVersion)
        {
            _defaultLogin.ChangeDefaultVersion(pVersion);
        }
        
        private string userName = string.Empty, password = string.Empty;

        private void customLogin()
        {
            try
            {
                // hourglass cursor
                //   Cursor = Cursors.WaitCursor;

                string folderpath = Path.GetDirectoryName(typeof(PerfQA_ArcFMLogin).Assembly.Location) + "\\Users_count";// +ConfigurationManager.AppSettings["USER_FILES_FOLDER"];
                //var autoResetEvent = new AutoResetEvent(false);
                //string path = "D:\\PerfQAnalyzer\\bin\\ARCFMLOGINOBJECT.txt";
                
                int iCurrentNumber = 0;
                
                string[] files = Directory.GetFiles(folderpath);//, "*.log");
                string[] sAllCredentials = File.ReadAllLines(Path.GetDirectoryName(typeof(PerfQA_ArcFMLogin).Assembly.Location) + "\\UserCredentials.txt");
                if (files.Length >= sAllCredentials.Length)
                {
                    foreach (string file in files)
                        File.Delete(file);
                }
                //foreach (string credential in sAllCredentials)  //foreach (string strfile in files)
                string credential = sAllCredentials[files.Length];
                //{
                    //if (files.Length < iCurrentNumber) continue;

                    iCurrentNumber = files.Length;

                    File.CreateText(folderpath + "\\Users_" + iCurrentNumber.ToString()).Close();

                    IPropertySet propertySet = new PropertySetClass();
                    userName = credential.Split(',')[0];
                    password = credential.Split(',')[1];

                    //Set server, instance, database
                    propertySet.SetProperty("SERVER", credential.Split(',')[2]);
                    propertySet.SetProperty("INSTANCE", credential.Split(',')[3]);//"sde:oracle11g:/;local=eder");
                    //Set user and password
                    propertySet.SetProperty("USER", userName);
                    propertySet.SetProperty("PASSWORD", password);


                    _defaultLogin.SetConnectionProperties(propertySet);
                    _defaultLogin.ShowDialog = false;
                    _loginChanged = _defaultLogin.Login(_initialLogin);
                    //checkfile = false;

                    
                //}
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            finally{}
        }
    }
}
