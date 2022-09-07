using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Miner.ComCategories;
using System.Runtime.InteropServices;
using PGE.Common.Delivery.ArcFM;
using ESRI.ArcGIS.Geodatabase;
using Miner.Interop;
using Miner.Geodatabase.AutoUpdaters;
using PGE.Common.Delivery.Diagnostics;
using System.Reflection;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.esriSystem;
using Miner;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Carto;
using System.Threading;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;

namespace PGE.Desktop.EDER.AutoUpdaters.Special
{
    [Guid("38A1717F-A928-47E4-96BD-2815D80B270C")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("PGE.Desktop.EDER.AutoUpdaters.Special.PGESegmentSplitAU")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    public class PGESegmentSplitAU : BaseSpecialAU
    {
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        public static bool DisableAU = false;

        #region Com Reg Methods

        /// <summary>
        /// Registers the specified reg key.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComRegisterFunction]
        private static void Register(string regKey)
        {
            Miner.ComCategories.SpecialAutoUpdateStrategy.Register(regKey);
        }
        /// <summary>
        /// Uns the register.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComUnregisterFunction]
        private static void UnRegister(string regKey)
        {
            Miner.ComCategories.SpecialAutoUpdateStrategy.Unregister(regKey);
        }

        #endregion

        public PGESegmentSplitAU()
            : base("PGE Segment Split")
        {

        }

        protected override bool CanExecute(mmAutoUpdaterMode eAUMode)
        {
            return (eAUMode != Miner.Interop.mmAutoUpdaterMode.mmAUMSplit);
        }

        protected override bool InternalEnabled(IObjectClass objectClass, mmEditEvent eEvent)
        {
            bool enabled = true;
            IFeatureClass featClass = objectClass as IFeatureClass;

            if (eEvent != mmEditEvent.mmEventFeatureCreate) { enabled = false; }
            else if (featClass == null) { enabled = false; }
            else if (featClass.ShapeType != ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPoint) { enabled = false; }

            return enabled;
        }

        /// <summary>
        /// This AU will not fire on split events.  When it executes it simply calls the out of the box ArcFM Segment 
        /// Split autoupdater.  This is to work around a bug with the code that fails in the out of the box AU when it
        /// executes in split mode.
        /// </summary>
        protected override void InternalExecute(IObject obj, mmAutoUpdaterMode eAUMode, mmEditEvent eEvent)
        {
            try
            {
                //If the static variable to disable this AU is set, then don't execute this AU
                if (DisableAU) { return; }

                if (eEvent == mmEditEvent.mmEventFeatureCreate && eAUMode != mmAutoUpdaterMode.mmAUMSplit)
                {
                    IFeature feature = obj as IFeature;
                    double num = 0.01;
                    IPoint pPoint = feature.Shape as IPoint;
                    if (pPoint == null) { return; }
                    IFeatureClass class2 = feature.Class as IFeatureClass;
                    if (class2 == null) { return; }

                    IWorkspace pWorkspace = ((IDataset)class2).Workspace;
                    IMMFeatureSplittingUtils utils = new MMFeatureSplitUtilsClass();
                    ISpatialReferenceTolerance tolerance = pPoint.SpatialReference as ISpatialReferenceTolerance;
                    if (tolerance.XYTolerance != null)
                    {
                        num = tolerance.XYTolerance;
                    }

                    ISet setOfLines = utils.SplitFeatureByClassModelNameAndDistance(pWorkspace, pPoint, "SplitTarget", class2.ObjectClassID, num);
                    if (setOfLines != null)
                    {
                        //No real need to flash the new lines.
                        //FlashNewPolyLines(setOfLines);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error executing PGE Segment Split AU: " + ex.Message);
                throw ex;
            }
        }

        private void FlashNewPolyLines(ISet setOfLines)
        {
            if (setOfLines == null)
            {
                return;
            }
            IMxDocument mxDoc = Application.Document as IMxDocument;
            if (mxDoc == null) { return; }

            IActiveView activeView = mxDoc.ActiveView;
            if (activeView == null) { return; }

            IScreenDisplay screenDisplay = activeView.ScreenDisplay;
            if (screenDisplay == null) { return; }

            RgbColor color2 = new RgbColorClass();
            color2.Red = 255;
            IRgbColor color = color2;
            SimpleLineSymbol symbol2 = new SimpleLineSymbolClass();
            symbol2.Color = color;
            symbol2.Style = 0;
            symbol2.Width = 2.0;
            ISimpleLineSymbol simpleLineSymbol = symbol2;
            setOfLines.Reset();
            while (true)
            {
                IFeature feature = (IFeature)setOfLines.Next();
                if (feature == null)
                {
                    return;
                }
                DrawPolyLine(feature, simpleLineSymbol, screenDisplay);
            }
        }

        private static void DrawPolyLine(IFeature feature, ISimpleLineSymbol simpleLineSymbol, IScreenDisplay screenDisplay)
        {
            int num = screenDisplay.ActiveCache;
            screenDisplay.ActiveCache = -1;
            screenDisplay.StartDrawing(0, -1);
            ISymbol symbol = simpleLineSymbol as ISymbol;
            symbol.ROP2 = esriRasterOpCode.esriROPNotXOrPen;
            screenDisplay.SetSymbol(symbol);
            screenDisplay.DrawPolyline(feature.Shape);
            Thread.Sleep(150);
            screenDisplay.DrawPolyline(feature.Shape);
            screenDisplay.FinishDrawing();
            screenDisplay.ActiveCache = ((short)num);
        }
    }
}
