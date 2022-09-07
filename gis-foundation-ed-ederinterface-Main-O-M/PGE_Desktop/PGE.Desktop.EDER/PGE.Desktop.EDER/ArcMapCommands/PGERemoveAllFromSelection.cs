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
using ESRI.ArcGIS.ADF.CATIDs;
using Microsoft.Win32;

namespace PGE.Desktop.EDER.ArcMapCommands
{
    /// <summary>
    /// Summary description for Tool1.
    /// </summary>
    /// 
    [ComponentCategory(ComCategory.D8SelectionTreeTool)]
    [ComVisible(true)]
    [Guid("2d5699b5-ed05-41e7-955d-5b474ac468fd"), ProgId("PGE.Desktop.EDER.ArcMapCommands.PGERemoveAllFromSelection")]

      
    public sealed class PGERemoveAllFromSelection : TreeToolBase 
    {       

        IScreenDisplay _display;
		IApplication _app;

		IMxApplication _mxapp;
        const string REMOVE_ALL_FROM_SELECTION_KEY = @"SOFTWARE\Wow6432Node\Classes\CLSID\{CFF956A2-EC34-4D06-92D6-E867D2DE9921}\InprocServer32";
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

		public PGERemoveAllFromSelection() : base("Remove All From Selection", 2000, 2, true, mmShortCutKey.mmCtrlA)
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
            IMxDocument pMxDoc = _app.Document as IMxDocument;
            IMap pMap = pMxDoc.FocusMap as IMap;
            
            IActiveView pAV = pMxDoc.ActiveView;

            //Get the visible extent of the display
            IEnvelope bounds = pAV.ScreenDisplay.DisplayTransformation.FittedBounds;   

			// reset the enum
			pEnumItems.Reset();

            if (lItemCount > 0)
            {
                pMap.ClearSelection();
                pAV.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection, null, bounds);
                
            }
          
		}

		public override int get_Enabled(Miner.Interop.ID8EnumListItem pEnumItems, int lItemCount)
		{
			pEnumItems.Reset();

			if(lItemCount > 0)  // just check again to be safe
			{
               
                return (int)(mmToolState.mmTSEnabled | mmToolState.mmTSVisible);
			}
			else // no items
			{
				return (int)mmToolState.mmTSNone;
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
                regKey = REMOVE_ALL_FROM_SELECTION_KEY;
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
        
    }
}
