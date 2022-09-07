using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Miner.Interop;
using PGE.Common.Delivery.Framework;
using ESRI.ArcGIS.Geodatabase;
using System.Reflection;
using System.Runtime.InteropServices;
using Miner.Geodatabase;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.Framework;
using System.Security.Principal;
using PGE.Desktop.EDER.Login;
using Miner.Interop.Process;
using PGE.Common.Delivery.ArcFM;
using PGE.Desktop.EDER.AutoUpdaters.Special;

//Changes for ENOS to SAP migration
namespace PGE.Desktop.EDER.ArcMapCommands.SuperUser
{
    public partial class PGEActivateSuperUser : Form
    {
        protected static string _ComboBoxSelected = String.Empty;
        protected static mmAutoUpdaterMode _oldMode;
        protected static IMMAutoUpdater _autoupdater = null;
        protected static IObjectClass _genInfoTable = null;
        protected static bool _alreadyAssigned = false;
        private static IObjectClass _ObjectClassInAction = null;
        public static Dictionary<string, bool> _IsSuperUserRoleActive = null;
        public static Dictionary<string, IObjectClass> _LayerObjects = null;

        public PGEActivateSuperUser()
        {
            InitializeComponent();
        }

        public PGEActivateSuperUser(List<string> dataSource)
        {
            InitializeComponent();
            this.comboBox1.DataSource = dataSource;
            this.comboBox1.SelectedIndex = 0;
            _IsSuperUserRoleActive = new Dictionary<string, bool>();
            _LayerObjects = new Dictionary<string, IObjectClass>();
        }

        private void btnActivate_Click(object sender, EventArgs e)
        {
            if (_ObjectClassInAction != null)
            {
                try
                {
                    if (_ComboBoxSelected.ToString().ToUpper() != "SELECT")
                    {
                        DoNotOperateAutoUpdaters.Instance.AddAU(_ObjectClassInAction.ObjectClassID, typeof(PGEAllowableEdits));
                        toolStripStatusLabel1.Text = string.Format("Activated...");
                        if(!_IsSuperUserRoleActive.ContainsKey(_ComboBoxSelected.ToString().ToUpper()))
                            _IsSuperUserRoleActive.Add(_ComboBoxSelected.ToString().ToUpper(), true);
                        statusStrip1.Refresh();
                        btnActivate.Enabled = false;
                        btnDeactivate.Enabled = true;
                        MessageBox.Show("Super user role successfully activated on : " + _ComboBoxSelected, "Activated", System.Windows.Forms.MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Please Select Any table / feature class name from the dropdown ", "Select Any Table", System.Windows.Forms.MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        btnActivate.Enabled = true;
                    }

                }
                catch (Exception ex)
                {
                    btnActivate.Enabled = true;
                    MessageBox.Show("Unable to activate Super User Role, Exception : " + ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name, System.Windows.Forms.MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
            else
            {
                btnActivate.Enabled = true;
                MessageBox.Show("Unable to find : " + _ComboBoxSelected + " Table/Feature class in logged in database", "Table / Feature Class not found", System.Windows.Forms.MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        
        private void btnDeactivate_Click(object sender, EventArgs e)
        {
            if (_ObjectClassInAction != null)
            {
                try
                {
                    //If the selected layer / table is already active, then deactivate
                    if (_IsSuperUserRoleActive.ContainsKey(_ComboBoxSelected.ToString().ToUpper()) && _IsSuperUserRoleActive[_ComboBoxSelected.ToString().ToUpper()])
                    {
                        DoNotOperateAutoUpdaters.Instance.RemoveAU(_ObjectClassInAction.ObjectClassID, typeof(PGEAllowableEdits));
                        if (_IsSuperUserRoleActive.ContainsKey(_ComboBoxSelected.ToString().ToUpper()))
                            _IsSuperUserRoleActive.Remove(_ComboBoxSelected.ToString().ToUpper());
                        btnDeactivate.Enabled = false;
                        btnActivate.Enabled = true;
                        toolStripStatusLabel1.Text = string.Format("Dectivated");
                        statusStrip1.Refresh();
                        MessageBox.Show("Super user role successfully Deactivated on :" + _ComboBoxSelected, "Deactivated", System.Windows.Forms.MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to De-activate Super User Role, Exception :" + ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name, System.Windows.Forms.MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }

        }

        private void PGEActivateSuperUser_Load(object sender, EventArgs e)
        {

            btnActivate.Enabled = false;
            btnDeactivate.Enabled = false;
            toolStripStatusLabel1.Text = "";
            statusStrip1.Refresh();
        }


        public bool AssignSpecialAU(IObjectClass pObjectClass, int subtype, bool assign)
        {
            int itemIndex = -1;
            int lastAUIndex = -1;
            Guid pClsID = new Guid("B67756A8-DB97-4061-8504-3419FFFDDCDC");
            //CLSIDFromProgID(AutoUpdaterProgID, out pClsID);
            if (string.Compare(pClsID.ToString(), "00000000-0000-0000-0000-000000000000", true) != 0)
            {
                string sUID = "{" + pClsID.ToString() + "}";  // convert System.Guid to ESRI.ArcGis.esriSystem.UID
                try
                {
                    IMMConfigTopLevel pConfigTopLevel = ConfigTopLevel.Instance;
                    IMMSubtype pSubType = pConfigTopLevel.GetSubtypeByID(pObjectClass, subtype, false);
                    if (pSubType != null)
                    {
                        ID8List pList = (ID8List)pSubType;
                        pList.Reset();
                        ID8ListItem pListItem = null;
                        while ((pListItem = pList.Next(true)) != null)
                        {
                            itemIndex++;
                            if (pListItem.ItemType == mmd8ItemType.mmitAutoValue)
                            {
                                lastAUIndex = itemIndex;
                                IMMAutoValue pAutoValue = (IMMAutoValue)pListItem;
                                if (pAutoValue.AutoGenID != null)
                                {
                                    if (string.Compare(pAutoValue.AutoGenID.Value.ToString(), sUID, true) == 0) _alreadyAssigned = true;
                                }
                                else
                                    _alreadyAssigned = false;

                                switch (pAutoValue.EditEvent)
                                {
                                    case mmEditEvent.mmEventFeatureCreate:
                                    case mmEditEvent.mmEventFeatureUpdate:
                                    case mmEditEvent.mmEventFeatureDelete:
                                        ApplyAuToObject(pAutoValue, assign, sUID, pAutoValue.EditEvent, pList, lastAUIndex);
                                        break;
                                }
                            }
                        }
                        pConfigTopLevel.SaveFeatureClassToDB(pObjectClass);
                        return true;
                    }
                    return false;
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name, System.Windows.Forms.MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;
                }

            }
            else
            {
                System.Windows.Forms.MessageBox.Show("ProgID: PGE.Desktop.EDER.PGEAllowableEdits not found", System.Reflection.MethodBase.GetCurrentMethod().Name, System.Windows.Forms.MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return false;
            }
            return false;
        }

        private void ApplyAuToObject(IMMAutoValue pAutoValue, bool assign, string sUID, mmEditEvent pEditEvent, ID8List pList, int lastAUIndex)
        {
            if (pAutoValue.AutoGenID != null)
            {
                if (String.Compare(pAutoValue.AutoGenID.Value.ToString(),
                                     sUID, true) == 0)
                {
                    if (assign)
                    {
                        if (!_alreadyAssigned)
                        {
                            IMMAutoValue pNewAutoValue = new MMAutoValueClass();
                            pNewAutoValue.EditEvent = pEditEvent;
                            UID pUID = new UIDClass();
                            pUID.Value = sUID;
                            pNewAutoValue.AutoGenID = pUID;
                            //pList.Clear();
                            pList.Insert((ID8ListItem)pNewAutoValue, lastAUIndex);  //insert the new AU ahead of the last OnDelete AU

                        }
                    }
                    else
                    {
                        pAutoValue.AutoGenID = null;
                        pAutoValue = null;
                        pList.Remove((ID8ListItem)pAutoValue);
                        //pList.Clear();
                    }

                }
            }
            else
            {
                if (assign)
                {
                    IMMAutoValue pNewAutoValue = new MMAutoValueClass();
                    pNewAutoValue.EditEvent = pEditEvent;
                    UID pUID = new UIDClass();
                    pUID.Value = sUID;
                    pNewAutoValue.AutoGenID = pUID;
                    //pList.Clear();
                    pList.Insert((ID8ListItem)pNewAutoValue, lastAUIndex);  //insert the new AU ahead of the last OnDelete AU
                }
                else
                {
                    pList.Remove((ID8ListItem)pAutoValue);
                    //pList.Clear();
                }
            }
        }

        private bool UserHasRole()
        {
            try
            {
                bool userHasRole = false;
                IMMPxIntegrationCache intcache = (IMMPxIntegrationCache)ApplicationFacade.Application.FindExtensionByName("Session Manager Integration Extension");
                IMMPxApplication pxApp = intcache.Application;
                IMMPxUser pPxUser = pxApp.User;

                //Loop through the roles for the user 
                IMMEnumPxRole pRoles = pPxUser.Roles;
                string role = pRoles.Next();
                while (!(role == null))
                {
                    if (role == "SUPER_USER_ADMIN")
                    {
                        userHasRole = true;
                        break;
                    }
                    role = pRoles.Next();
                }

                return userHasRole;
            }
            catch (Exception ex)
            {
                throw new Exception("Error checking is user has Super User Role");
            }
        }


        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                _ComboBoxSelected = comboBox1.SelectedValue.ToString();
                if (_ComboBoxSelected.ToString().ToUpper() != "SELECT" && !_ComboBoxSelected.ToString().IsNullOrEmpty())
                {
                    string objectName = _ComboBoxSelected.ToString().Contains("EDGIS.") ? _ComboBoxSelected.ToString() : "EDGIS." + _ComboBoxSelected.ToString();
                    GetObjectClassToDisableAU(objectName);
                    if (_ObjectClassInAction != null)
                    {
                        if (_IsSuperUserRoleActive.ContainsKey(_ComboBoxSelected.ToString().ToUpper()))
                        {
                            //it means, super user role is already active for this layer
                            if (_IsSuperUserRoleActive[_ComboBoxSelected.ToString().ToUpper()])
                            {
                                btnActivate.Enabled = false;
                                btnDeactivate.Enabled = true;
                                toolStripStatusLabel1.Text = string.Format("Activated");
                            }
                            else
                            {
                                btnActivate.Enabled = true;
                                btnDeactivate.Enabled = false;
                                toolStripStatusLabel1.Text = string.Format("Deactivated");
                            }
                        }
                        else
                        {
                            btnActivate.Enabled = true;
                            btnDeactivate.Enabled = false;
                            toolStripStatusLabel1.Text = string.Empty;
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                // throw;
            }
        }

        /// <summary>
        /// Checks whether a object class exist in login workspace, if yes, assigns it to '_ObjectClassInAction' parameter
        /// </summary>
        /// <param name="objClassName"></param>
        private void GetObjectClassToDisableAU(string objClassName)
        {
            if (_LayerObjects.Count > 0 && _LayerObjects.ContainsKey(objClassName))
            {
                _ObjectClassInAction = _LayerObjects[objClassName];
            }
            else
            {
                if (PGEActivateSuperUserCommand._loginWorkspace != null)
                {
                    if (((IWorkspace2)PGEActivateSuperUserCommand._loginWorkspace).NameExists[esriDatasetType.esriDTFeatureClass, objClassName])
                    {
                        _ObjectClassInAction = ((IFeatureWorkspace)PGEActivateSuperUserCommand._loginWorkspace).OpenFeatureClass(objClassName) as IObjectClass;
                    }
                    else if (((IWorkspace2)PGEActivateSuperUserCommand._loginWorkspace).NameExists[esriDatasetType.esriDTTable, objClassName])
                    {
                        _ObjectClassInAction = ((IFeatureWorkspace)PGEActivateSuperUserCommand._loginWorkspace).OpenTable(objClassName) as IObjectClass;
                    }
                    if (_ObjectClassInAction != null)
                    {
                        if (!_LayerObjects.ContainsKey(objClassName))
                            _LayerObjects.Add(objClassName, _ObjectClassInAction);
                    }
                }
            }
        }

        /// <summary>
        /// Form closing event, if user closes form, then enable AU back in action
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PGEActivateSuperUser_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (_IsSuperUserRoleActive.Count > 0 && _IsSuperUserRoleActive.Any(x=>x.Value == true))
                {
                    DialogResult dialogresult = MessageBox.Show("Super User role is still active for few layers, closing this window will de-activate super user role for all layers, Do you want to proceed", "Warning", MessageBoxButtons.YesNo);
                    if (dialogresult == DialogResult.Yes)
                    {
                        foreach (var x in _IsSuperUserRoleActive)
                        {
                            if (x.Value)
                            {
                                var objectClass = _LayerObjects.Count > 0 && _LayerObjects.ContainsKey(x.Key) ? _LayerObjects[x.Key] as IObjectClass : null;
                                if (objectClass != null)
                                {
                                    DoNotOperateAutoUpdaters.Instance.RemoveAU(objectClass.ObjectClassID, typeof(PGEAllowableEdits));
                                }
                            }
                        }
                        _LayerObjects = null;
                        _IsSuperUserRoleActive = null;
                    }
                    else
                    {
                        e.Cancel = true;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {

            }
        }
    }
}
