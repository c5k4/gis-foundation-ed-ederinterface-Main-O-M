using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Miner.Interop;
using PGE.Common.Delivery.Diagnostics;
using ESRI.ArcGIS.Geodatabase;
using System.Reflection;
using Miner.Geodatabase;
using System.Windows.Forms;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using System.Linq;
using Miner.ComCategories;
using PGE.Common.Delivery.Framework;
//using PGE.Common.Delivery.Framework;

namespace PGE.Desktop.EDER.D8TreeTools
{
    [Guid("9146426F-8604-4FC0-9D32-0D62A5D3D6DF")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComponentCategory(ComCategory.D8SelectionTreeTool)]
    public class PGE_Fill_PII_Info : IMMTreeViewTool
    {
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        private static IMMTreeTool OOTBFillPII_Info = null;
        private PGE_Fill_PII_Info_Form form = null;
        IApplication m_application;
        //IObject pObj = null;
        #region Com Reg Methods

        /// <summary>
        /// Registers the specified reg key.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComRegisterFunction]
        private static void Register(string regKey)
        {
            Miner.ComCategories.D8SelectionTreeTool.Register(regKey);
        }
        /// <summary>
        /// Uns the register.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComUnregisterFunction]
        private static void UnRegister(string regKey)
        {
            Miner.ComCategories.D8SelectionTreeTool.Unregister(regKey);
        }

        #endregion

        public PGE_Fill_PII_Info()
        {
            string MMTools = "MMAbandonTools.MMTVRemove";
            // We get the type using just the ProgID
            Type oType = Type.GetTypeFromProgID(MMTools);
            if (oType != null)
            {
                object obj = Activator.CreateInstance(oType);
                OOTBFillPII_Info = obj as IMMTreeTool;
            }
        }

        public int Category
        {
            get { return OOTBFillPII_Info.Category; }
        }

        public void Execute(IMMTreeViewSelection pSelection)
        {
            
          ID8EnumListItem enumListItems = pSelection as ID8EnumListItem;
                enumListItems.Reset();
                ID8ListItem listItem = enumListItems.Next();
                if (listItem != null)
                {
                    if (listItem.ItemType == mmd8ItemType.mmd8itFeature)
                    {
                        ID8GeoAssoc geoAssocFeature = listItem as ID8GeoAssoc;
                        IObject pObject = geoAssocFeature.AssociatedGeoRow as IObject;
                        if (form != null)
                        {
                            form.Close();
                            form = new PGE_Fill_PII_Info_Form(pObject);
                            DialogResult result = form.ShowDialog();
                            if (result == DialogResult.OK)
                            {
                                form.Close();
                            }
                        }
                        else
                        {
                            form = new PGE_Fill_PII_Info_Form(pObject);
                            DialogResult result = form.ShowDialog();
                            if (result == DialogResult.OK)
                            {
                                form.Close();
                            }
                        }
                    }

                }
        }

        public string Name
        {
            //After UAT 
            get {
                _logger.Info("PGE POL Information");
                return "PGE POL Information"; }
        }

        public int Priority
        {
            get { return 1; }
        }

        public int get_Enabled(IMMTreeViewSelection pSelection)
        {
            //string ADGroupUserList = "V1RR,AKMB,M2V9,S1YH";
            //string [] userLANIdList = ADGroupUserList.Split(',');

            //System.Security.Principal.WindowsIdentity wi = System.Security.Principal.WindowsIdentity.GetCurrent();

            //string userLanID = wi.Name.Substring(wi.Name.LastIndexOf("\\") + 1);
            //userLanID = userLanID.ToUpper();
            
            bool editEnabled = Miner.Geodatabase.Edit.Editor.EditState == Miner.Geodatabase.Edit.EditState.StateEditing;
            int enabled = 0;
            try
            {
                if (editEnabled)
                {
                    ID8EnumListItem enumListItems = pSelection as ID8EnumListItem;
                    enumListItems.Reset();
                    ID8ListItem listItem = enumListItems.Next();
                    if (listItem != null)
                    {
                        ID8GeoAssoc geoAssoc = listItem as ID8GeoAssoc;
                        if (geoAssoc != null)
                        {
                            IObject pObj = geoAssoc.AssociatedGeoRow as IObject;
                            if (pObj != null)
                            {
                                if (listItem.ItemType == mmd8ItemType.mmd8itFeature && ModelNameFacade.ContainsClassModelName(pObj.Class, SchemaInfo.Electric.ClassModelNames.CustomerAgreementObject))
                                {
                                    int subtypeIndex = pObj.Class.Fields.FindField("SUBTYPECD");
                                    if (subtypeIndex != -1)
                                    {
                                        //For Subtype - Privately Owned Line 
                                        object subtypeVal = pObj.get_Value(subtypeIndex);
                                        if (subtypeVal.ToString().Equals("2"))
                                        {
                                            enabled = 1;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("ERROR - ", ex);
                //enabled = -1;
            }

            if (enabled == 1)
            {
                return (int)(mmToolState.mmTSEnabled | mmToolState.mmTSVisible);
            }
            else
                return (int)mmToolState.mmTSNone;
        }
    }
}
