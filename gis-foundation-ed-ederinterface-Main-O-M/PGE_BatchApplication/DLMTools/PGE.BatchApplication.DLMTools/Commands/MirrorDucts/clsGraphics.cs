//using ESRI.ArcGIS.NetworkAnalysis;
//using System.Linq;
//using SysForms = System.Windows.Forms;
//using System.Windows.Forms;
using System;
using System.Diagnostics;
using System.Threading;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;

namespace PGE.BatchApplication.DLMTools.Commands.MirrorDucts
{
    public static class clsGraphics
    {

        public static IRgbColor GetRGBColour(Int32 pRed, Int32 pGreen, Int32 pBlue)
        {
            try
            {
                IRgbColor pColor = new RgbColorClass();
                pColor.Red = pRed;
                pColor.Green = pGreen;
                pColor.Blue = pBlue;
                return pColor;
            }
            catch
            {
                throw new Exception("Error returning colour");
            }
        }

        //public static void DrawEdge(
        //    int EID,
        //    IGeometricNetwork pGN,
        //    IRgbColor pColor,
        //    string tag,
        //    DEEnums.DEGraphicType pGraphicType)
        //{
        //    try
        //    {
        //        //Draw the edge  
        //        IGeometry pGeometry = pGN.get_GeometryForEdgeEID(EID);
        //        IPolyline pPolyline = (IPolyline)pGeometry;
        //        DrawPolyline((IMap)DEData.g_ActiveView,
        //            pPolyline, pColor, tag,
        //            pGraphicType);

        //    }
        //    catch
        //    {
        //        throw new Exception("Error drawing edge");
        //    }
        //}

        //public static void DrawTextElement(
        //    IPoint pPoint,
        //    IRgbColor pRGBcolor,
        //    string strText,
        //    string elementTag,
        //    string graphicFont,
        //    double graphicSize,
        //    bool graphicBold,
        //    bool graphicUnderline)
        //{
        //    try
        //    {
        //        //Pointers needed to make text element                  
        //        ITextElement pTextElement;
        //        IElementProperties pElementProps;
        //        ITextSymbol pTextSymbol;
        //        IElement pElement;

        //        //Next, cocreate a new TextElement
        //        pTextElement = new TextElementClass();
        //        pElement = (IElement)pTextElement;
        //        pElement.Geometry = pPoint;

        //        //Next, setup a font
        //        stdole.IFontDisp pFontDisp = (stdole.IFontDisp)new stdole.StdFont();
        //        pFontDisp.Name = graphicFont;
        //        pFontDisp.Bold = graphicBold;
        //        pFontDisp.Underline = graphicUnderline;

        //        //Next, setup a TextSymbol that the TextElement will draw with
        //        pTextSymbol = new TextSymbolClass();
        //        pTextSymbol.Font = pFontDisp;
        //        pTextSymbol.Color = pRGBcolor;
        //        //set the size of the text symbol here, rather than on the font 
        //        pTextSymbol.Size = graphicSize;

        //        //Next, Give the TextSymbol and text string to the TextElement
        //        pTextElement.Symbol = pTextSymbol;
        //        pTextElement.Text = strText;
        //        if (elementTag != "")
        //        {
        //            pElementProps = (IElementProperties)pTextElement;
        //            pElementProps.Name = elementTag;
        //        }

        //        IGraphicsContainer graphicsContainer = (IGraphicsContainer)DEData.g_ActiveView;
        //        graphicsContainer.AddElement(pElement, 0);

        //    }
        //    catch
        //    {
        //        throw new Exception("Error creating text symbol");
        //    }
        //}

        //public static void DrawJunction(
        //    int EID,
        //    IGeometricNetwork pGN,
        //    esriSimpleMarkerStyle pMarkerStyle,
        //    IRgbColor pColor,
        //    string tag)
        //{
        //    try
        //    {
        //        //find it in the traced juntions list 
        //        IGeometry pGeometry = pGN.get_GeometryForJunctionEID(EID);
        //        IPoint pPoint = (IPoint)pGeometry;
        //        DrawPoint(
        //            (IMap)DEData.g_ActiveView,
        //            pPoint,
        //            pColor,
        //            pMarkerStyle,
        //            DEEnums.DEGraphicType.GraphicTypeNode);

        //    }
        //    catch
        //    {
        //        throw new Exception("Error drawing edge");
        //    }
        //}

        public static void DrawGeometry(
            IMap map,
            IGeometry geometry,
            IRgbColor rgbColor,
            IRgbColor outlineRgbColor)
        {
            IGraphicsContainer graphicsContainer = (IGraphicsContainer)map; // Explicit Cast
            IElement element = null;
            IColor pColor = (IColor)rgbColor;

            if ((geometry.GeometryType) == esriGeometryType.esriGeometryPoint)
            {
                // Marker symbols
                ISimpleMarkerSymbol simpleMarkerSymbol = new SimpleMarkerSymbolClass();
                simpleMarkerSymbol.Color = rgbColor;
                //simpleMarkerSymbol.Outline = true;
                //simpleMarkerSymbol.OutlineColor = outlineRgbColor;
                simpleMarkerSymbol.Size = 15;
                simpleMarkerSymbol.Style = esriSimpleMarkerStyle.esriSMSCross;

                IMarkerElement markerElement = new MarkerElementClass();
                markerElement.Symbol = simpleMarkerSymbol;
                element = (IElement)markerElement; // Explicit Cast
            }
            else if ((geometry.GeometryType) == esriGeometryType.esriGeometryPolyline)
            {
                //  Line elements
                ISimpleLineSymbol simpleLineSymbol = new SimpleLineSymbolClass();
                simpleLineSymbol.Color = rgbColor;
                simpleLineSymbol.Style = esriSimpleLineStyle.esriSLSSolid;
                simpleLineSymbol.Width = 5;

                ILineElement lineElement = new LineElementClass();
                lineElement.Symbol = simpleLineSymbol;
                element = (IElement)lineElement; // Explicit Cast
            }
            else if ((geometry.GeometryType) == esriGeometryType.esriGeometryPolygon)
            {
                ISimpleLineSymbol simpleLineSymbol = new SimpleLineSymbolClass();
                simpleLineSymbol.Color = rgbColor;
                simpleLineSymbol.Style = esriSimpleLineStyle.esriSLSSolid;
                simpleLineSymbol.Width = 0.0004;

                // Polygon elements
                ISimpleFillSymbol simpleFillSymbol = new SimpleFillSymbolClass();
                simpleFillSymbol.Color = rgbColor;
                simpleFillSymbol.Style = esriSimpleFillStyle.esriSFSSolid;
                simpleFillSymbol.Outline = simpleLineSymbol; 
                IFillShapeElement fillShapeElement = new PolygonElementClass();
                fillShapeElement.Symbol = simpleFillSymbol;
                element = (IElement)fillShapeElement; // Explicit Cast
            }
            if (!(element == null))
            {
                element.Geometry = geometry;
                graphicsContainer.AddElement(element, 0);
            }
        }

        public static void DrawPoint(
            IMap pMap,
            IPoint pPoint,
            IRgbColor pColor,
            esriSimpleMarkerStyle pMarkerStyle)
        {
            IGraphicsContainer graphicsContainer = (IGraphicsContainer)pMap; // Explicit Cast
            IElement element = null;
            IElementProperties pElementProps;
            //string elementTag = pGraphicType.ToString();

            // Marker symbols
            ISimpleMarkerSymbol simpleMarkerSymbol = new SimpleMarkerSymbolClass();
            simpleMarkerSymbol.Color = pColor;
            simpleMarkerSymbol.Size = 0.2;
            simpleMarkerSymbol.Style = pMarkerStyle;

            IMarkerElement markerElement = new MarkerElementClass();
            markerElement.Symbol = simpleMarkerSymbol;
            element = (IElement)markerElement; // Explicit Cast

            if (!(element == null))
            {
                pElementProps = (IElementProperties)element;
                //pElementProps.Name = elementTag;
                element.Geometry = pPoint;
                graphicsContainer.AddElement(element, 0);
            }
        }

        public static void DrawPolyline(
            IMap pMap,
            IPolyline pPolyline,
            IRgbColor pColor,
            string displayText)
        {

            IGraphicsContainer graphicsContainer = (IGraphicsContainer)pMap; // Explicit Cast
            IElement element = null;
            //string elementTag = pGraphicType.ToString();

            //  Line elements
            ISimpleLineSymbol simpleLineSymbol = new SimpleLineSymbolClass();
            simpleLineSymbol.Color = pColor;
            simpleLineSymbol.Style = esriSimpleLineStyle.esriSLSSolid;
            simpleLineSymbol.Width = 0.1;

            ILineElement lineElement = new LineElementClass();
            lineElement.Symbol = simpleLineSymbol;
            element = (IElement)lineElement; // Explicit Cast

            //Create a text element 
            IElement pTextEl = null;
            IElementProperties pElementProps = null;

            if (displayText != "")
            {
                IEnvelope pEnv = pPolyline.Envelope;
                IPoint pCenter = new PointClass();
                pCenter.SpatialReference = pPolyline.SpatialReference;
                pCenter.PutCoords(((pEnv.XMin + pEnv.XMax) / 2), ((pEnv.YMin + pEnv.YMax) / 2));
                ISimpleTextSymbol pTextSym = new TextSymbolClass();
                ITextElement pTextElement = new TextElementClass();
                pTextElement.Text = displayText;
                pTextElement.Symbol = pTextSym;
                pTextEl = (IElement)pTextElement;
                pTextEl.Geometry = pCenter;
            }

            if (!(element == null))
            {
                element.Geometry = pPolyline;
                //if (elementTag != "")
                //{
                //    pElementProps = (IElementProperties)element;
                //    //pElementProps.Name = elementTag;
                //}

                graphicsContainer.AddElement(element, 0);
            }
            if (!(pTextEl == null))
            {
                graphicsContainer.AddElement(pTextEl, 0);
            }
        }

        public static bool FlashGeometry(IActiveView pActiveView, IGeometry pGeo, IColor pColor, int pSize, int pInterval)
        {

            try
            {
                if (pActiveView == null)
                    return false;
                IScreenDisplay pScreenDisplay = (IScreenDisplay)pActiveView.ScreenDisplay;
                short pScrCache;
                ILineSymbol pSimpleLineSymbol;
                ISimpleFillSymbol pSimpleFillSymbol;
                ISimpleMarkerSymbol pSimpleMarkersymbol;
                ISymbol pSymbol;
                pScrCache = (short)esriScreenCache.esriNoScreenCache;
                pScreenDisplay.StartDrawing(0, pScrCache);

                switch (pGeo.GeometryType)
                {

                    case esriGeometryType.esriGeometryPolyline:
                        pSimpleLineSymbol = new SimpleLineSymbolClass();
                        pSymbol = (ISymbol)pSimpleLineSymbol;
                        pSymbol.ROP2 = esriRasterOpCode.esriROPNotXOrPen;
                        pSimpleLineSymbol.Width = pSize;
                        pSimpleLineSymbol.Color = pColor;
                        pScreenDisplay.SetSymbol(pSymbol);
                        pScreenDisplay.DrawPolyline(pGeo);
                        //Win32API.Sleep(pInterval);
                        Thread.Sleep(pInterval);
                        pScreenDisplay.DrawPolyline(pGeo);
                        break;

                    case esriGeometryType.esriGeometryPolygon:
                        pSimpleFillSymbol = new SimpleFillSymbolClass();
                        pSymbol = (ISymbol)pSimpleFillSymbol;
                        pSymbol.ROP2 = esriRasterOpCode.esriROPNotXOrPen;
                        pSimpleFillSymbol.Color = pColor;
                        pScreenDisplay.SetSymbol(pSymbol);
                        pScreenDisplay.DrawPolygon(pGeo);
                        //Win32API.Sleep(pInterval);
                        Thread.Sleep(pInterval);
                        pScreenDisplay.DrawPolygon(pGeo);
                        break;

                    case esriGeometryType.esriGeometryPoint:
                        pSimpleMarkersymbol = new SimpleMarkerSymbolClass();
                        pSymbol = (ISymbol)pSimpleMarkersymbol;
                        pSymbol.ROP2 = esriRasterOpCode.esriROPNotXOrPen;
                        pSimpleMarkersymbol.Color = pColor;
                        pSimpleMarkersymbol.Size = pSize;
                        pScreenDisplay.SetSymbol(pSymbol);
                        pScreenDisplay.DrawPoint(pGeo);
                        //Win32API.Sleep(pInterval);
                        Thread.Sleep(pInterval);
                        pScreenDisplay.DrawPoint(pGeo);
                        break;

                    case esriGeometryType.esriGeometryMultipoint:
                        IPointCollection pPointCollection = (IPointCollection)pGeo;
                        pSimpleMarkersymbol = new SimpleMarkerSymbolClass();
                        pSymbol = (ISymbol)pSimpleMarkersymbol;
                        pSymbol.ROP2 = esriRasterOpCode.esriROPNotXOrPen;
                        pSimpleMarkersymbol.Color = pColor;
                        pSimpleMarkersymbol.Size = pSize;
                        pScreenDisplay.SetSymbol(pSymbol);
                        for (int i = 0; i < pPointCollection.PointCount; i++)
                        {
                            pScreenDisplay.DrawPoint(pPointCollection.get_Point(i));
                        }

                        //Win32API.Sleep(pInterval);
                        Thread.Sleep(pInterval);
                        for (int i = 0; i < pPointCollection.PointCount; i++)
                        {
                            pScreenDisplay.DrawPoint(pPointCollection.get_Point(i));
                        }
                        break;
                    default:
                        break;
                }

                pScreenDisplay.FinishDrawing();
                return true;

            }

            catch (Exception ex)
            {
                Debug.Print("Error occurred in FlashGeometry function. " + ex.Message);
                return false;
            }

        }

    }
}

