using System;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geodatabase;
using Miner.ComCategories;
using Miner.Interop;
using PGE.Desktop.EDER.D8TreeTools.CreateRelatedObject;
using PGE.Common.Delivery.Diagnostics;

namespace PGE.Desktop.EDER.D8TreeTools
{
    /// <summary>
    /// Class responsible for executing custom field editor from selection tab.
    /// </summary>
    [Guid("a3f18aef-936a-4753-b849-d23caf106095")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComponentCategory(ComCategory.D8SelectionTreeTool)]
    public class PGE_Create_Related_Object : IMMTreeViewTool
    {
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

        #region Private variable

        /// <summary>
        ///  Create custom tools for a PGE Create Related Object TargetTa.
        /// </summary>
        private static IMMTreeViewTool OOTBCreateRelated = null;

        /// <summary>
        /// Variable for error logging.
        /// </summary>
        private static readonly Log4NetLogger _logger = new Log4NetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        #endregion

        #region Constructor
        /// <summary>
        /// Register current tool for <code>MMRelTools.MMCreateRelatedObject</code>.
        /// <remarks>This registration make "PGE - Create Related Object" option available for selection tab.</remarks>
        /// </summary>
        public PGE_Create_Related_Object()
        {
            string MMCreateRelatedProgID = "MMRelTools.MMCreateRelatedObject";
            // We get the type using just the ProgID
            Type oType = Type.GetTypeFromProgID(MMCreateRelatedProgID);
            if (oType != null)
            {
                object obj = Activator.CreateInstance(oType);
                OOTBCreateRelated = obj as IMMTreeViewTool;
            }
        }

        #endregion

        #region IMMTreeViewTool Implementation

        /// <summary>
        /// Use an arbitrary number to make your own 'category' in the context menu TreeTools are grouped by category in the context menu
        /// </summary>
        public int Category
        {
            get { return OOTBCreateRelated.Category; }
        }

        /// <summary>
        /// Specify the name which will appear in the context menu.
        /// </summary>
        public string Name
        {
            get 
            {
                _logger.Info("PGE - Create Related Object-> Loaded");
                return "PGE - Create Related Object"; 
            }
        }

        /// <summary>
        /// Determines the display order of the tool in its category.
        /// </summary>
        public int Priority
        {
            get
            {
                return 1;//OOTBCreateRelated.Priority; //
            }
        }

        /// <summary>
        /// Custom code to perform the tool's task.
        /// </summary>
        /// <param name="pEnumItems"></param>
        public void Execute(IMMTreeViewSelection pSelection)
        {
            try
            {
                ID8EnumListItem enumListItems = pSelection as ID8EnumListItem;
                enumListItems.Reset();
                ID8ListItem listItem = enumListItems.Next();
                if (listItem != null)
                {
                    if (listItem.ItemType == mmd8ItemType.mmitRelationship)
                    {
                        ID8ListItem parentItem = listItem.ContainedBy as ID8ListItem;
                        Subtype_Field_Editor.ParentSubtype = null;
                        if (parentItem != null && parentItem is ID8GeoAssoc)
                        {
                            ID8GeoAssoc geoAssoc = parentItem as ID8GeoAssoc;
                            Subtype_Field_Editor.ParentSubtype = geoAssoc.AssociatedGeoRow.get_Value(geoAssoc.AssociatedGeoRow.Fields.FindField("SUBTYPECD")).ToString();
                            //Subtype_Field_Editor.InstallationTypeCode = geoAssoc.AssociatedGeoRow.get_Value(geoAssoc.AssociatedGeoRow.Fields.FindField("INSTALLATIONTYPE")).ToString();

                            IDataset objectDataset = geoAssoc.AssociatedGeoRow.Table as IDataset;
                            Subtype_Field_Editor.ParentFeatureClass = objectDataset.BrowseName;
                        }
                    }
                    //If there is not a specific configuration for this then just launch the OOTB tree tool
                    //Create an instance of the out of the box ArcFM Create Related object tree tool so we can enable the same way
                    OOTBCreateRelated.Execute(pSelection);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("ERROR - ", ex);
            }
        }

        /// <summary>
        /// The selected item and enable or disable the tool in the context menu.
        /// </summary>
        /// <param name="pEnumItems">Current selected tree view item.</param>
        /// <returns></returns>
        public int get_Enabled(IMMTreeViewSelection pSelection)
        {
            int enabled =  0; 
            try
            {
                ID8EnumListItem enumListItems = pSelection as ID8EnumListItem;
                enumListItems.Reset();
                ID8ListItem listItem = enumListItems.Next();
                if (listItem != null)
                {
                    if (listItem.ItemType == mmd8ItemType.mmitRelationship)
                    {
                        enabled = OOTBCreateRelated.get_Enabled(pSelection);
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
