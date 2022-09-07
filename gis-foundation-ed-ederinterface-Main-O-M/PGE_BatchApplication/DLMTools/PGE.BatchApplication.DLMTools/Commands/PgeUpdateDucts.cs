using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;
using Miner.ComCategories;
using PGE.Common.Delivery.Framework;
using ModelNameFacade = PGE.BatchApplication.DLMTools.Utility.ModelNameFacade;

namespace PGE.BatchApplication.DLMTools.Commands
{
    /// <summary>
    /// PGE Custom Print Command that sets print the Grid with buffer hash
    /// </summary>
    [Guid("AAA35FEC-8FBC-499c-AFF7-05988A38BE73")]
    [ComponentCategory(ComCategory.ArcMapCommands)]
    [ProgId("PGE.BatchApplication.DLMTools.MassUpdate")]
    [ComVisible(true)]
    public sealed class PGEUpdateDucts : BaseCommand
    {
        #region Com Reg Methods

        /// <summary>
        /// Registers the specified reg key.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComRegisterFunction]
        private static void Register(string regKey)
        {
            ArcMapCommands.Register(regKey);
        }
        /// <summary>
        /// Uns the register.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComUnregisterFunction]
        private static void UnRegister(string regKey)
        {
            ArcMapCommands.Unregister(regKey);
        }

        #endregion

        #region Private Varibales
        /// <summary>
        /// Currently running ArcMap instance
        /// </summary>
        private IApplication _application;
        IMap _map;

        /// <summary>
        /// Logger to log error/debug messages
        /// </summary>
        private static readonly Utility.Logs.Log4NetFileHelper Logger =
            new Utility.Logs.Log4NetFileHelper(MethodBase.GetCurrentMethod().DeclaringType);
        #endregion Private Varibales

        #region Constructor
        /// <summary>
        /// Creates an instance of <see cref="PGEPrintMapCommand"/>
        /// </summary>
        public PGEUpdateDucts()
        {
            m_category = "PGE Conversion Tools"; //localizable text
            m_caption = "Update Duct Annotation";  //localizable text
            m_message = "Updates rotation on all selected duct annotation";  //localizable text 
            m_toolTip = "Update all selected duct anno";  //localizable text 
            m_name = "PGE_Update_Ducts";   //unique id, non-localizable (e.g. "MyCategory_ArcMapCommand")

            try
            {
                m_bitmap = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("PGE.BatchApplication.DLMTools.Bitmaps.PgeUpdateDucts.bmp"));
            }
            catch (Exception ex)
            {
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
        /// Occurs when this command is clicked
        /// </summary>
        public override void OnClick()
        {
            MouseCursor appCursor = new MouseCursorClass();

            UID editorUID = new UIDClass();
            editorUID.Value = "esriEditor.Editor";
            IEditor3 editor = _application.FindExtensionByCLSID(editorUID) as IEditor3;

            try
            {
                IMxDocument mxDocument = _application.Document as IMxDocument;
                _map = mxDocument.FocusMap;

                appCursor.SetCursor(2);

                // Get the conduit feature class
                CartoFacade cf = new CartoFacade(_map);
                IList<IFeatureLayer> layers = cf.FeatureLayersFromMap();
                IWorkspace ws = ((IDataset) layers[0]).Workspace;
                IFeatureClass ductFc = ModelNameFacade.FeatureClassByModelName(ws, "UFMANNOTATION");
                IFeatureLayer ductLayer = GetFeatureLayer(_map, ductFc);

                // Get the selected conduit
                ISelectionSet selectedDucts = (ductLayer as IFeatureSelection).SelectionSet;

                if (selectedDucts.Count == 0)
                {
                    // Do nothing
                }
                else if (selectedDucts.Count > 0)
                {
                    editor.StartOperation();

                    IEnumIDs ductAnnoIds = selectedDucts.IDs;
                    ductAnnoIds.Reset();
                    int ductAnnoId = ductAnnoIds.Next();

                    while (ductAnnoId != -1)
                    {
                        IFeature ductAnno = ductFc.GetFeature(ductAnnoId);
                        ductAnno.Store();
                        ductAnnoId = ductAnnoIds.Next();
                    }

                    editor.StopOperation("Undo Duct Anno Update");

                    ((IActiveView) _map).Refresh();
                }
            }
            catch (Exception ex)
            {
                Logger.DefaultLogger.Error(ex.Message, ex);
                if ((editor.EditWorkspace as IWorkspaceEdit2).IsInEditOperation == true)
                {
                    editor.AbortOperation();
                }
            }
            finally
            {
                appCursor.SetCursor(0);
            }
        }

        /// <summary>
        /// Returns the layer for the supplied feature class
        /// </summary>
        /// <param name="map"></param>
        /// <param name="featureClass"></param>
        /// <returns></returns>
        public static IFeatureLayer GetFeatureLayer(IMap map, IFeatureClass featureClass)
        {
            // Log entry
            string name = MethodBase.GetCurrentMethod().Name;
            Logger.DefaultLogger.Debug("Entered " + name);

            IFeatureLayer featLayer = null;
            IEnumLayer enumLayer = null;

            try
            {
                // Get all layers in the map
                UID uid = new UID { Value = "{40A9E885-5533-11d0-98BE-00805F7CED21}" };
                enumLayer = map.Layers[uid];

                // For each layer
                ILayer layer;
                while ((layer = enumLayer.Next()) != null)
                {
                    // Check to see if the layer is for our feature class
                    featLayer = layer as IFeatureLayer;
                    if (featLayer != null)
                    {
                        if (((IDataset)featLayer.FeatureClass).Name == ((IDataset)featureClass).Name)
                        {
                            // If it is, save it and bail out
                            featLayer = layer as IFeatureLayer;
                            break;
                        }
                    }
                }
            }
            finally
            {
                // Release object
                if (enumLayer != null)
                {
                    Marshal.ReleaseComObject(enumLayer);
                }
            }

            return featLayer;
        }

        /*
        private void ResetDefQuery(IWorkspace ws, string modelName)
        {
            IFeatureClass fc = ModelNameFacade.FeatureClassByModelName(ws, modelName);
            IFeatureLayer layer = UfmHelper.GetFeatureLayer(_map, fc);
            IFeatureLayerDefinition layerDef = layer as IFeatureLayerDefinition;
            layerDef.DefinitionExpression = "";
        }

        private void UpdateDefQuery(IWorkspace ws, ISet features, string modelName)
        {
            IFeatureClass fc = ModelNameFacade.FeatureClassByModelName(ws, modelName);
            IFeatureLayer layer = UfmHelper.GetFeatureLayer(_map, fc);
            IFeatureLayerDefinition layerDef = layer as IFeatureLayerDefinition;
            string defQuery = "";
            features.Reset();
            object featureObj = features.Next();

            while (featureObj != null)
            {
                IFeature feature = featureObj as IFeature;
                string id = feature.OID.ToString();
                if (defQuery != "")
                {
                    defQuery += " OR ";
                }
                defQuery += "OBJECTID=" + id;
                featureObj = features.Next();
            }

            if (defQuery == string.Empty)
            {
                defQuery = "OBJECTID=0";
            }

            layerDef.DefinitionExpression = defQuery;
        }
        */

        #endregion Overridden Class Methods
    }
}
