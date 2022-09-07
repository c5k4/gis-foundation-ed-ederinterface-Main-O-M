#region Organized and sorted using
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geodatabase;
using Miner.ComCategories;
using Miner.Interop;
using PGE.Common.Delivery.Diagnostics;
#endregion

namespace PGE.Desktop.EDER.D8TreeTools.CreateRelatedObject
{
    /// <summary>
    /// Class responsible for executing custom field editor from target tab.
    /// </summary>
    [Guid("4C04A74F-5163-49BE-A47C-18D112649740")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComponentCategory(ComCategory.D8SelectedCuTreeTool)]
    public class PGE_Create_Related_Object_TargetTab : IMMTreeViewTool
    {
        #region Com Reg Methods

        /// <summary>
        /// Registers the specified reg key.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComRegisterFunction]
        private static void Register(string regKey)
        {
            Miner.ComCategories.D8SelectedCuTreeTool.Register(regKey); //Bug#13131 and 13135
        }
        /// <summary>
        /// Uns the register.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComUnregisterFunction]
        private static void UnRegister(string regKey)
        {
            Miner.ComCategories.D8SelectedCuTreeTool.Unregister(regKey); //Bug#13131 and 13135
        }

        #endregion

        #region Private variable

        /// <summary>
        ///  Create PGE Create Related Object custom tools for Target tab.
        /// </summary>
        private static IMMTreeViewTool OOTBCreateRelatedObjectForTarget = null;

        /// <summary>
        /// Variable for error logging.
        /// </summary>
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        #endregion

        #region Constructor

        /// <summary>
        /// Register current tool for <code>MMRelTools.MMAddRelatedFavorite</code>.
        /// <remarks>This registration make "PGE - Create Related Object" option available for target tab.</remarks>
        /// </summary>
        public PGE_Create_Related_Object_TargetTab()
        {
            if (OOTBCreateRelatedObjectForTarget == null)
            {
                _logger.Debug("PGE - Create Related Object for target tab Constructor called.");
                string MMCreateRelatedProgID = "MMRelTools.MMAddRelatedFavorite";
                // We get the type using just the ProgID
                Type oType = Type.GetTypeFromProgID(MMCreateRelatedProgID);
                if (oType != null)
                {
                    _logger.Debug("PGE - Create Related Object for target tab Constructor: Created type of MMRelTools.MMAddRelatedFavorite.");
                    object obj = Activator.CreateInstance(oType);
                    OOTBCreateRelatedObjectForTarget = obj as IMMTreeViewTool;
                    if (OOTBCreateRelatedObjectForTarget != null)
                        _logger.Debug("PGE - Create Related Object for target tab Constructor: IMMTreeViewTool instanciated.");
                }
            }
        }

        #endregion

        #region IMMTreeViewTool Implementation

        /// <summary>
        /// Use an arbitrary number to make your own 'category' in the context menu TreeTools are grouped by category in the context menu
        /// </summary>
        public int Category
        {
            get
            {
                return 1025;
            }
        }

        /// <summary>
        /// Custom code to perform the tool's task.
        /// </summary>
        /// <param name="pEnumItems"></param>
        public void Execute(IMMTreeViewSelection pEnumItems)
        {
            try
            {
                ID8EnumListItem enumListItems = pEnumItems as ID8EnumListItem;               
                enumListItems.Reset();
                ID8ListItem listItem = enumListItems.Next();

                // We are getting "IMMFieldAdapter" as our ID8ListItem is not ID8GeoAssoc type.
                IMMProposedObject pPO = (IMMProposedObject)listItem.ContainedBy;
                IMMFieldManager pFM = pPO.FieldManager;
                if (listItem != null)
                {
                    if (listItem.ItemType == mmd8ItemType.mmitRelationship)
                    {
                        Subtype_Field_Editor.ParentFeatureClass = (pFM.Table as IDataset).BrowseName;
                        Subtype_Field_Editor.ParentSubtype = pFM.SubtypeCode.ToString();
                    }
                }
                // Executing the Custom Field Editor from target tab.
                OOTBCreateRelatedObjectForTarget.Execute(pEnumItems);
            }
            catch (Exception ex)
            {
                _logger.Error("ERROR - ", ex);
            }
        }

        /// <summary>
        /// Specify the name which will appear in the context menu.
        /// </summary>
        public string Name
        {
            get
            {
                _logger.Info("PGE - Create Related Object-> Loaded");
                return "PGE - Create Related Object"; }
        }

        /// <summary>
        /// Determines the display order of the tool in its category.
        /// </summary>
        public int Priority
        {
            get { return 6482; }
        }

        /// <summary>
        /// The selected item and enable or disable the tool in the context menu.
        /// </summary>
        /// <param name="pEnumItems"></param>
        /// <returns></returns>
        public int get_Enabled(IMMTreeViewSelection pEnumItems)
        {
            int enabled = 0;
            try
            {
                //Bug#13131 and 13135
                ID8EnumListItem enumListItems = pEnumItems as ID8EnumListItem;
                enumListItems.Reset();
                ID8ListItem listItem = enumListItems.Next();
                if (listItem != null)
                {
                    if (listItem.ItemType == mmd8ItemType.mmitRelationship)
                    {
                        enabled = OOTBCreateRelatedObjectForTarget.get_Enabled(pEnumItems);  // (int)(mmToolState.mmTSEnabled | mmToolState.mmTSVisible);
                        _logger.Debug("PGE - Create Related Object for target tab: OOTBCreateRelated.Enabled = " + enabled);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("ERROR - ", ex);
               // enabled = - 1;
            }

            return enabled;
        }

        #endregion
    }
}
