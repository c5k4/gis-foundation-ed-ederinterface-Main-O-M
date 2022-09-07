using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Miner.ComCategories;
using Miner.Interop;
using Miner.Framework;
using ESRI.ArcGIS.Geodatabase;
using PGE.Common.Delivery.Framework;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.EditorExt;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using PGE.Common.Delivery.Diagnostics;
using System.Reflection;
using PGE.Desktop.EDER.ValidationRules;
using PGE.Desktop.EDER.AutoUpdaters.Special;
using PGE.Desktop.EDER.AutoUpdaters;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.GeoDatabaseUI;


namespace PGE.Desktop.EDER.D8TreeTools
{
    [Guid("01E0569E-F137-4625-AAB7-5CE744EBED1D")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComponentCategory(ComCategory.D8SelectionTreeTool)]
    public class PGE_OpenTableShowingSelection : IMMTreeViewTool
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
        private static IMMTreeTool OOTBTableShowingSelection = null;
        IApplication m_application;

        /// <summary>
        /// Variable for error logging.
        /// </summary>
        private static readonly Log4NetLogger _logger = new Log4NetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        #endregion



        #region IMMTreeViewTool Implementation

        /// <summary>
        /// Use an arbitrary number to make your own 'category' in the context menu TreeTools are grouped by category in the context menu
        /// </summary>
        public int Category
        {
            get { return OOTBTableShowingSelection.Category; }
        }

        /// <summary>
        /// Specify the name which will appear in the context menu.
        /// </summary>
        public string Name
        {
            get {
                _logger.Info("PGE-Open Table Showing Selection");

                return "PGE-Open Table Showing Selection"; }
        }

        /// <summary>
        /// Determines the display order of the tool in its category.
        /// </summary>
        public int Priority
        {
            get
            {
                return 2;//OOTBTableShowingSelection.Priority; //
            }
        }

        //MMAbandonTools.MMTVRemove     MMTools.MMArcFMPropPage
        public PGE_OpenTableShowingSelection()
        {
            string MMTools = "MMAbandonTools.MMTVRemove";
            // We get the type using just the ProgID
            Type oType = Type.GetTypeFromProgID(MMTools);
            if (oType != null)
            {
                object obj = Activator.CreateInstance(oType);
                OOTBTableShowingSelection = obj as IMMTreeTool;
            }
        }

         /// <summary>
        /// The selected item and enable or disable the tool in the context menu.
        /// </summary>
        /// <param name="pEnumItems">Current selected tree view item.</param>
        /// <returns></returns>
        public int get_Enabled(IMMTreeViewSelection pSelection)
        {
            int enabled = 0;
            try
            {
                ID8EnumListItem enumListItems = pSelection as ID8EnumListItem;
                enumListItems.Reset();
                ID8ListItem listItem = enumListItems.Next();
                if (listItem != null)
                {
                    if (listItem.ItemType == mmd8ItemType.mmd8itLayer)
                    {
                        enabled = 1;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("ERROR - ", ex);
               // enabled = - 1;
            }

            if (enabled == 1)
            {
                return (int)(mmToolState.mmTSEnabled | mmToolState.mmTSVisible);
            }
            else
                return (int)(mmToolState.mmTSVisible);
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
                    if (listItem.ItemType == mmd8ItemType.mmd8itLayer)
                    {
                        ID8ListItem parentItem = listItem.ContainedBy as ID8ListItem;
                        //Subtype_Field_Editor.ParentSubtype = null;
                        Type t = Type.GetTypeFromCLSID(typeof(AppRefClass).GUID);
                        System.Object obj = Activator.CreateInstance(t);
                        IApplication m_application = obj as IApplication;
                        string layerAliasName = listItem.DisplayName;
                        IFeatureLayer pLayer = ((IMxDocument)m_application.Document).FocusMap.get_Layer(0) as IFeatureLayer;
                        ILayer pFLayer = null;

                        if (FindLayerByName(layerAliasName, out pFLayer))
                        {
                            pLayer = pFLayer as IFeatureLayer;
                            
                            IStandaloneTable pST = null;

                            ITableWindow2 pTableWindow2 = null;

                            ITableWindow pExistingTableWindow = null;
                            pTableWindow2 = new TableWindowClass();

                            pExistingTableWindow = pTableWindow2.FindViaLayer(pLayer);

                            pTableWindow2.Layer = pLayer;

                            pTableWindow2.TableSelectionAction = esriTableSelectionActions.esriSelectFeatures;
                            pTableWindow2.ShowSelected = true;

                            pTableWindow2.ShowAliasNamesInColumnHeadings = true;

                            pTableWindow2.Application = m_application;

                            pTableWindow2.Show(true);
                        }
                    }
                    
                }
            }
            catch (Exception ex)
            {
                _logger.Error("ERROR - ", ex);
            }
        }

        public bool FindLayerByName(String name, out ILayer resLayer)
        {
            try
            {
                Type appRefType = Type.GetTypeFromProgID("esriFramework.AppRef");
                object appRefObj = Activator.CreateInstance(appRefType);
                IApplication arcMapApp = appRefObj as IApplication;
                if (arcMapApp == null)
                {
                    resLayer = null;
                    return false;
                }

                IMxDocument mxDoc = (IMxDocument)arcMapApp.Document;
                IMap map = mxDoc.FocusMap;

                resLayer = FindLayerHelper(map, null, name);
                return resLayer != null;
            }
            catch (Exception ex)
            {
                _logger.Error("PGE Open Table showing Selection, error in function FindLayerByName ", ex);
                resLayer = null;
                return false;
            }
        }

        public ILayer FindLayerHelper(IMap map, ICompositeLayer layers, string lyrName)
        {
            try
            {
                for (int i = 0; i < (map != null ? map.LayerCount : layers.Count); i++)
                {
                    ILayer lyr = map == null ? layers.Layer[i] : map.Layer[i];

                    if (lyr is ICompositeLayer) lyr = FindLayerHelper(null, (ICompositeLayer)lyr, lyrName);

                    if (lyr != null && lyr.Name.ToUpper().Contains(lyrName.ToUpper()))
                    {
                        return lyr;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("PGE Open Table showing Selection, error in function FindLayerHelper ", ex);
            }

            return null;
        }

        #endregion
    }
       

}