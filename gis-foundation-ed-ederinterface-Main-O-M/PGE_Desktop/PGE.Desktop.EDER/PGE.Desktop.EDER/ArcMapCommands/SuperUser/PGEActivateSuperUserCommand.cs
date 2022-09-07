using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PGE.Common.Delivery.UI.Commands;
using System.Drawing;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.ADF;

using Miner.ComCategories;
using Miner.Interop;

using PGE.Common.Delivery.Framework;
using PGE.Common.Delivery.Diagnostics;
using PGE.Common.Delivery.ArcFM;
using System.Reflection;
using System.Windows.Forms;
using System.Collections;
using System.Security.Principal;

namespace PGE.Desktop.EDER.ArcMapCommands.SuperUser
{
    /// <summary>
    /// Changes for ENOS to SAP migration, PGE Activate Super User Command activates the admin roles for a user to edit generation table data
    /// </summary>
    [Guid("D7F836A1-A99B-4517-AFCF-2944B62809AA")]
    [ComponentCategory(ComCategory.ArcMapCommands)]
    [ProgId("PGE.Desktop.EDER.ArcMapCommands.SuperUser.PGEActivateSuperUserCommand")]
    [ComVisible(true)]
    public class PGEActivateSuperUserCommand : BaseArcGISCommand
    {
        #region Com Reg Methods

        /// <summary>
        /// Registers the specified reg key.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComRegisterFunction]
        private static void Register(string regKey)
        {
            Miner.ComCategories.ArcMapCommands.Register(regKey);
        }
        /// <summary>
        /// Uns the register.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComUnregisterFunction]
        private static void UnRegister(string regKey)
        {
            Miner.ComCategories.ArcMapCommands.Unregister(regKey);
        }

        #endregion

        #region Private Varibales
        /// <summary>
        /// Currently running ArcMap instance
        /// </summary>
        private IApplication _application = null;
        /// <summary>
        /// Logger to log error/debug messages
        /// </summary>
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");


        #endregion Private Varibales

        private static PGEActivateSuperUser frmSuperUser;
        public static IWorkspace _loginWorkspace;
        public static ITable _superuserTable;
        private string _currentUser = string.Empty;
        #region Constructor
        /// <summary>
        /// Creates an instance of <see cref="PGEActivateSuperUserCommand"/>
        /// </summary>
        public PGEActivateSuperUserCommand() :
            base("PGETools_PGEActivateSuperUserCommand", "PGE Activate Super User", "PGE Admin Tools", "PGE Activate Super User", "PGE Activate Super User")
        {
            try
            {
                //
                // TODO: change bitmap name if necessary
                //
                Bitmap bmp = null;
                //Get path for bitmap image 
                string path = GetType().Assembly.GetName().Name + ".Bitmaps." + GetType().Name + ".bmp";
                _logger.Debug("Bitmap image path" + path);
                //Get bitmap image
                bmp = new Bitmap(GetType().Assembly.GetManifestResourceStream(path));
                //Assign bitmap image
                UpdateBitmap(bmp, 0, 0);
            }
            catch (Exception ex)
            {
                _logger.Warn("Invalid Bitmap" + ex.Message);
                System.Diagnostics.Trace.WriteLine(ex.Message, "Invalid Bitmap");
            }
        }



        #endregion Constructor

        #region Overridden Class Methods

        /// <summary>
        /// Occurs when this command is created
        /// </summary>
        /// <param name="hook">Instance of the application</param>
        public override void OnCreate(object hook)
        {
            if (hook == null) return;
            _application = hook as IApplication;
        }

        /// <summary>
        /// this command will be enabled only for those users who are in super user group
        /// </summary>
        /// <returns></returns>
        public bool Enabled()
        {
            return Security.IsInAdminGroup;
        }

        /// <summary>
        /// Occurs when this command is clicked
        /// </summary>
        protected override void InternalClick()
        {
            try
            {
                //Get the current extend of the map.
                Type type = Type.GetTypeFromProgID("esriFramework.AppRef");
                object obj = Activator.CreateInstance(type);
                IApplication app = (IApplication)obj;
                _currentUser = ((IMMLogin2)app.FindExtensionByName("MMPROPERTIESEXT")).LoginObject.UserName;

                IMxDocument mxDocument = _application.Document as IMxDocument;
                IActiveView activeView = mxDocument.FocusMap as IActiveView;
                IEnvelope envelope = activeView.Extent;
                IWorkspace loginWorkspace = GetWorkspace();
                var groupNames = new List<string>();
                var comboSource = new List<string>();
                Dictionary<string,List<string>> listOfAdGroups = new Dictionary<string,List<string>>();
                comboSource.Add("SELECT");               

                if (loginWorkspace != null)
                {
                    if (((IWorkspace2)loginWorkspace).NameExists[esriDatasetType.esriDTTable, "PGEDATA.SUPERUSERS_GROUPS"])
                    {
                        _superuserTable = ((IFeatureWorkspace)loginWorkspace).OpenTable("PGEDATA.SUPERUSERS_GROUPS");
                        if (_superuserTable != null)
                        {
                            IQueryFilter pQf = new QueryFilterClass();
                            pQf.SubFields = "ADGROUPNAME,TABLENAME";

                            ICursor pCur = _superuserTable.Search(pQf, true);                           
                            IRow pRow = null;
                            while ((pRow = pCur.NextRow()) != null)
                            {
                                string adGrpName = pRow.get_Value(pRow.Fields.FindField("ADGROUPNAME")).ToString().ToUpper();
                                if (!string.IsNullOrEmpty(adGrpName))
                                {
                                    string tableName = pRow.get_Value(pRow.Fields.FindField("TABLENAME")).ToString().ToUpper();

                                    var listOfTableNames = new List<string>();

                                    if (listOfAdGroups.ContainsKey(adGrpName))
                                        listOfTableNames = listOfAdGroups[adGrpName];
                                    else
                                        listOfAdGroups[adGrpName] = listOfTableNames;

                                    listOfTableNames.Add(tableName);
                                }
                            }
                          
                            Marshal.ReleaseComObject(pCur);
                            bool eligibleToOpen = IsInGroup(_currentUser, listOfAdGroups, ref comboSource);
                            if (eligibleToOpen && comboSource.Count > 1)
                            {
                                bool IsFormAlreadyOpen = IsAlreadyOpen(typeof(PGEActivateSuperUser));
                                if (!IsFormAlreadyOpen)
                                {
                                    frmSuperUser = new PGEActivateSuperUser(comboSource);
                                    frmSuperUser.StartPosition = FormStartPosition.CenterParent;
                                    frmSuperUser.Show(System.Windows.Forms.Control.FromHandle((IntPtr)_application.hWnd));                                    
                                }
                            }
                            else
                            {
                                MessageBox.Show(_currentUser + " do not have sufficient privileges to run this tool", "In sufficient privileges", System.Windows.Forms.MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            }  
                        }
                    }
                    else
                    {
                        MessageBox.Show("PGEDATA.SUPERUSERS_GROUPS table was not found in the logged in workspace", "Table Not Found", System.Windows.Forms.MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        #endregion Overridden Class Methods

        private bool IsInGroup(string user, Dictionary<string, List<string>> listOfAdGroups, ref List<string> comboSource)
        {
            bool isPartOfADGrp = false;
            try
            {
                using (var identity = new WindowsIdentity(user))
                {
                    var principal = new WindowsPrincipal(identity);
                    foreach (var adGrp in listOfAdGroups)
                    {
                        if (principal.IsInRole(adGrp.Key))
                        {
                            isPartOfADGrp = true;
                            comboSource.AddRange(adGrp.Value);
                        }
                    }
                }
                return isPartOfADGrp;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in verifying the user, Exception : " + ex.Message, "Unable to verify user", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return false;
            }
        }


        public static bool IsAlreadyOpen(Type formType)
        {
            try
            {
                foreach (Form f in Application.OpenForms)
                {
                    if (f.GetType() == formType)
                    {
                        f.BringToFront();
                        f.WindowState = FormWindowState.Normal;
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private static IWorkspace GetWorkspace()
        {
            // Get and return the logged in workspace
            IMMLoginUtils utils = new MMLoginUtils();
            _loginWorkspace = utils.LoginWorkspace;
            return _loginWorkspace;
        }

        public IWorkspace LoginWorkspace
        {
            get
            {
                _loginWorkspace = GetWorkspace();
                return _loginWorkspace;
            }
            set
            {
                _loginWorkspace = value;
            }
        }
   
    }

    public class Security
    {
        public static bool IsInAdminGroup
        {
            get { return isInAdminGroup(); }

        }

        public static bool IsInSuperUserGroup
        {
            get { return isInSuperUserGroup(); }

        }

        private static bool isInAdminGroup()
        {

            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole("EDGIS_PLENG_OPENG_NP");
            
        }

        private static bool isInSuperUserGroup()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole("EDGIS_PLENG_OPENG_NP");
        }
    }



    //public class TableNames
    //{
    //    public string Name { get; set; }
    //    public string Value { get; set; }
    //}
}
