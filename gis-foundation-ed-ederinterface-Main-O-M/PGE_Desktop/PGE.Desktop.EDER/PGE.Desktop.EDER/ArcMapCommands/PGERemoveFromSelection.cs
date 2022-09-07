using System;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;
using Miner.Interop;
using Miner.ComCategories;
using System.Windows.Forms;
using PGE.Common.Delivery.Framework;
using ESRI.ArcGIS.ADF.CATIDs;
using System.Collections.Generic ;
using PGE.Common.Delivery.Diagnostics;
using System.Reflection;
using Microsoft.Win32;

namespace PGE.Desktop.EDER.ArcMapCommands
{
	//The following line may be used to register in the correct component category (rather than
	//using the Component Category Registration section below. If you choose to do it this way,
	//you must execute RegX.exe (in ArcFM Solution\Bin) by dragging the .dll and dropping it on the .exe.

	[ComponentCategory(ComCategory.D8SelectionTreeTool)]
	[ComVisible(true)]
    [Guid("f2413fc9-60de-498d-9bbb-26cd7f3ec335"), ProgId("PGE.Desktop.EDER.ArcMapCommands.PGERemoveFromSelection")]

	public class PGERemoveFromSelection : TreeToolBase
	{

		IScreenDisplay _display;
		IApplication _app;
		IMxApplication _mxapp;
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        const string REMOVE_FROM_SELECTION_KEY = @"SOFTWARE\Wow6432Node\Classes\CLSID\{B420F742-68A6-11D4-A7AF-0001031AE99A}\InprocServer32";
        const string APPEND_CHAR = "_";
        const int INSTALL_MODE = 1;
        const int UNINSTALL_MODE = 0;

        
        #region Com Reg Methods

        /// <summary>
        /// Registers the specified reg key.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComRegisterFunction]
        private static void Register(string regKey)
        {
            Miner.ComCategories.D8SelectionTreeTool.Register(regKey);
            HideShowArcFMMenuItem(1); 
        }
        /// <summary>
        /// Uns the register.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComUnregisterFunction]
        private static void UnRegister(string regKey)
        {
            Miner.ComCategories.D8SelectionTreeTool.Unregister(regKey);
            HideShowArcFMMenuItem(0); 
        }

        #endregion


		public PGERemoveFromSelection() : base("Remove From Selection", 2000, 1, true, mmShortCutKey.mmCtrlR  )
		{
			// set the tool's bitmap. See base class for handling of bitmap to avoid memory leaks..Namespace
			//InternalBitmap = new System.Drawing.Bitmap(GetType().Assembly.GetManifestResourceStream(GetType().Namespace + ".Resources.ZoomHighlight.bmp"));

			// Get the display
			// AppRef is usually not our friend, but there in no other option here.
			_app = Activator.CreateInstance(Type.GetTypeFromProgID("esriFramework.AppRef")) as IApplication;
			_mxapp = _app as IMxApplication;
			if(_mxapp != null)
			{
				_display = _mxapp.Display as IScreenDisplay;
			}
		}

		public override void Execute(Miner.Interop.ID8EnumListItem pEnumItems, int lItemCount)
		{          
            //Type type = Type.GetTypeFromCLSID(D8SelectionTreeTool.CatID.GetType().GUID  );
            //object obj = Activator.CreateInstance(type);
            //ICommandBar treeViewTool = (ICommandBar)obj;   

            IMxDocument pMxDoc = _app.Document as IMxDocument;
            IMap pMap = pMxDoc.FocusMap as IMap;
            ISelectionEvents pSelectionEvents;
            pSelectionEvents = pMap as ISelectionEvents;            

            IActiveView pAV = pMxDoc.ActiveView;

            //Get the visible extent of the display
            IEnvelope bounds = pAV.ScreenDisplay.DisplayTransformation.FittedBounds;            
           
           
			pEnumItems.Reset();
            System.Collections.Generic.List<int> constructOIDAddList = new System.Collections.Generic.List<int>();
            IFeatureLayer pFeatLayer = null ;  

            IFeatureSelection pFeatSelection = null    ;
            ISelectionSet pSelectionSet = null  ;
            int[] oidAddList = null ;

            try
            {
                if (lItemCount > 0)
                {

                    //add a list of features (OID's) to the selection set

                    ID8ListItem li = pEnumItems.Next();
                    pFeatLayer = null;

                    if (li.ItemType == mmd8ItemType.mmd8itLayer)
                    {
                        while (li != null)
                        {
                            pFeatLayer = GetFeatureLayer(pMap, li.DisplayName);

                            if (pFeatLayer != null)
                            {
                                pFeatSelection = pFeatLayer as IFeatureSelection;
                                pFeatSelection.Clear();
                            }

                            li = pEnumItems.Next();
                        }
                    }
                    else
                    {
                        ID8GeoAssoc geo = li as ID8GeoAssoc;
                        IFeature feature = geo.AssociatedGeoRow as IFeature;
                        List<IFeatureLayer> featlist = null;
                        int iCount = 0;
                        featlist = GetFeatureLayerByClassName(pMap, feature.Class.AliasName);

                        while (li != null)
                        {
                            geo = li as ID8GeoAssoc;
                            feature = geo.AssociatedGeoRow as IFeature;
                            constructOIDAddList.Add(feature.OID);
                            li = pEnumItems.Next();
                        }

                        oidAddList = constructOIDAddList.ToArray();

                        for (iCount = 0; iCount < featlist.Count; iCount++)
                        {
                            pFeatSelection = featlist[iCount] as IFeatureSelection;
                            pSelectionSet = pFeatSelection.SelectionSet;
                            pSelectionSet.RemoveList(constructOIDAddList.Count, ref oidAddList[0]);
                            pFeatSelection.SelectionSet = pSelectionSet;
                        }
                    }                   

                    pSelectionEvents.SelectionChanged();
                    pAV.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection, null, bounds);

                }
            }
            catch (Exception ex)
            {
                
                _logger.Error("PGE Remove From Selection failed.", ex);
                MessageBox.Show("Failed to clear the selected feature(s).");
            }

           

		}

        private static void HideShowArcFMMenuItem(int installMode)
        {
            try
            {
                //Look for the existing registry setting for RemoveFromSelection 
                //and RemoveAllFromSelection 
                string regKey = "";
                string keyValue = "";
                RegistryKey pRegKey = null;
                regKey = REMOVE_FROM_SELECTION_KEY;
                pRegKey = Registry.LocalMachine.OpenSubKey(regKey, true);

                if (pRegKey == null)
                    throw new Exception("Unable to find registry key");
                keyValue = pRegKey.GetValue("").ToString();

                if (installMode == 1)
                {
                    //They are installing 
                    if (!keyValue.EndsWith(APPEND_CHAR))
                        keyValue = keyValue + APPEND_CHAR;
                }
                else
                {
                    //They are uninstalling 
                    if (keyValue.EndsWith(APPEND_CHAR))
                        keyValue = keyValue.Substring(0, keyValue.Length - 1);
                }

                //Set the value 
                pRegKey.SetValue("", keyValue);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private List<IFeatureLayer> GetFeatureLayerByClassName(IMap map, string layerName)
        {
            if (map == null) return null;
            //get all layers in Map
            IEnumLayer enumLayer = map.get_Layers(CartoFacade.UIDFacade.FeatureLayers, true);
            IFeatureLayer featLayer = null;
            ILayer layer;
            IFeatureClass pFeatClass = null;

            List<IFeatureLayer> featlist = new List<IFeatureLayer>();

            while ((layer = enumLayer.Next()) != null)//loop through each layer
            {
                //Validate the layer and check layer name
                
                if (layer.Valid) //check layer name matching
                {
                    featLayer = layer as IFeatureLayer;
                    pFeatClass = featLayer.FeatureClass;

                    if (pFeatClass.AliasName.ToLower() == layerName.ToLower())
                    {
                        //_logger.Debug(layerName + " layer found.");
                        featLayer = layer as IFeatureLayer;
                        featlist.Add(featLayer);
                    }
                }
            }

            //Release object
            if (enumLayer != null) Marshal.ReleaseComObject(enumLayer);

            return featlist;
        }

        private IFeatureLayer  GetFeatureLayer(IMap map, string layerName)
        {
            if (map == null) return null;
            //get all layers in Map
            IEnumLayer enumLayer = map.get_Layers(CartoFacade.UIDFacade.FeatureLayers, true);
            IFeatureLayer featLayer = null;
            ILayer layer;
            
            while ((layer = enumLayer.Next()) != null)//loop through each layer
            {
                //Validate the layer and check layer name
                if (layer.Valid && layer.Name.ToLower() == layerName.ToLower()) //check layer name matching
                {
                    //_logger.Debug(layerName + " layer found.");
                    featLayer = layer as IFeatureLayer;                    
                    break;
                }
            }

            //Release object
            if (enumLayer != null) Marshal.ReleaseComObject(enumLayer);

            return featLayer;
        }

		public override int get_Enabled(Miner.Interop.ID8EnumListItem pEnumItems, int lItemCount)
		{
			pEnumItems.Reset();

			if(lItemCount > 0)  // just check again to be safe
			{
				
				// this type of casting will raise an exception
				ID8ListItem li = pEnumItems.Next();

                if (li.ItemType == mmd8ItemType.mmd8itLayer || li.ItemType == mmd8ItemType.mmd8itFeature)
                {
                    if (li.ItemType == mmd8ItemType.mmd8itFeature)
                    {
                        ID8GeoAssoc geo = (ID8GeoAssoc)li;
                        if (geo.AssociatedGeoRow is IRow)
                        {
                            IFeature feat = geo.AssociatedGeoRow as IFeature;
                            if ( feat == null)
                                return (int)mmToolState.mmTSNone;
                            else 
                                return (int)(mmToolState.mmTSEnabled | mmToolState.mmTSVisible);
                        }
                        else
                            return (int)(mmToolState.mmTSEnabled | mmToolState.mmTSVisible);
                    }
                    else
                        return (int)(mmToolState.mmTSEnabled | mmToolState.mmTSVisible);
                }
                else
                    return (int)mmToolState.mmTSNone;
               
                  
			}
			else // no items
			{
				return (int)mmToolState.mmTSNone;
			}

            return (int)mmToolState.mmTSNone;
			// if we got here, they are all features.
			//return (int)(mmToolState.mmTSEnabled | mmToolState.mmTSVisible);
		}
	}
}
