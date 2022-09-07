using System;
using System.Linq;
using System.Windows.Forms;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace PGE.BatchApplication.DLMTools.Commands.AnnoStackingValidation
{
    /// <summary>
    /// Class to pass info from form.
    /// </summary>
    public static class Form_Status
    {
        public static bool Proceed_Pressed = false;
        public static bool Cancel_Pressed = false;
        public static bool Delete_Pressed = false;
    }
    //
    public class AnnoLeaderGraphics
    {
        #region Local Variables
        //TextWriterTraceListener m_Logger = null;
        System.IO.StreamWriter m_Logger = null;
        IApplication m_app;
        IMxDocument m_pMxDoc;
        IEditor3 m_editor = null;
        IWorkspace m_workspace = null;
        IFeatureWorkspace m_featureworkspace = null;
        IMap m_map = null;
        int m_coloridx = 0;
        string[] m_arr_colors;
        string[] m_arr_layers;
        frmAnnoLeaderGraphics m_dialog;
        int m_argRun = 0;
        #endregion

        public AnnoLeaderGraphics(frmAnnoLeaderGraphics dialog, int argRun, IApplication app)
        {
            m_app = app;
            m_pMxDoc = (IMxDocument) app.Document;
            //
            // TODO: Set the pathing for the logger or use the listener. See region Local Variables also.
            //m_Logger = new TextWriterTraceListener("CreateConduitAnno.log", "CreateConduitAnno");
            //m_Logger = new System.IO.StreamWriter("c:\\projects\\pge\\reports\\CreateConduitAnno.txt");
            //m_Logger = new System.IO.StreamWriter("c:\\temp\\AnnoLeaderGraphics.txt");
            // 
            m_editor = m_app.FindExtensionByName("ESRI Object Editor") as IEditor3;
            m_workspace = m_editor.EditWorkspace;
            m_featureworkspace = (IFeatureWorkspace)m_workspace;
            m_map = m_pMxDoc.Maps.get_Item(0);
            //
            m_dialog = dialog;
            m_argRun = argRun;
        }
        /// <summary>
        /// Rum method
        /// </summary>
        public bool Run()
        {
            // This block of code is for making the form function like
            // a persistent toolbar.
            WriteLogEntry("Start");
            if (m_argRun == 0)
            {
                m_dialog.Show();
            }
            if (m_argRun == 1)
            {
                SetUpColorsAndLayers();
                ParseSelectedFeatures();
            }
            if (m_argRun == 2)
            {
                DeleteAllGraphics();
            }
            CloseFiles();


            //WriteLogEntry("Start");
            //m_dialog.ShowDialog();
            //if (Form_Status.Proceed_Pressed == true)
            //{
            //    SetUpColorsAndLayers();
            //    ParseSelectedFeatures();
            //}
            //if (Form_Status.Delete_Pressed == true)
            //{
            //    DeleteAllGraphics();
            //}
            //
            //CloseFiles();
            return true;
        }

        /// <summary>
        /// This is the main method for marking annotations.
        /// </summary>
        public bool ParseSelectedFeatures()
        {
            if (m_map.SelectionCount == 0)
            {
                MessageBox.Show("There are no conductors selected.");
                return false;
            }
            //
            #region Graphics container and graphic symbols
            IGraphicsContainer graphicsContainer;
		    graphicsContainer = (IGraphicsContainer)m_map;
            // Symbol for annos
            ISimpleFillSymbol polygonSymbol =new SimpleFillSymbol();
            if (m_dialog.rdoFill.Checked == true)
            {
                //polygonSymbol.Style = esriSimpleFillStyle.esriSFSDiagonalCross;
                polygonSymbol.Style = esriSimpleFillStyle.esriSFSSolid;
            }
            if (m_dialog.rdoHollow.Checked == true)
            {
                polygonSymbol.Style = esriSimpleFillStyle.esriSFSNull;
            }
            // Symbol to use for conductors
            ISimpleLineSymbol lineSymbol = new SimpleLineSymbolClass();
            lineSymbol.Style = esriSimpleLineStyle.esriSLSSolid;
            lineSymbol.Width = 1.0;
            #endregion
            //
            IEnumFeature enumFeature = (IEnumFeature)m_map.FeatureSelection;
            IEnumFeatureSetup enumSetup = (IEnumFeatureSetup)enumFeature;
            enumSetup.AllFields = true;
            enumFeature.Reset();
            IFeature feature = enumFeature.Next();
            while (feature != null)
            {
                IDataset dataset = (IDataset)feature.Class;
                IFeatureClass fc = (IFeatureClass)feature.Class;
                string src_layer = dataset.Name.ToUpper();
                WriteLogEntry("HANDLING " + src_layer.ToUpper());
                if(m_arr_layers.Contains(src_layer))
                {
                    try
                    {
                        // Get a set of annotations
                        ISet anno_set = GetRelatedAnnotations(fc, feature);
                        bool has_annos = false;
                        if (anno_set.Count != 0)
                        {
                            int ct = 0;
                            has_annos = true;
                            // Get a new color for the line and polygon symbols
                            lineSymbol.Color = GetNextColor();
                            polygonSymbol.Outline = lineSymbol;
                            polygonSymbol.Color = lineSymbol.Color;

                            // Loop through anno features
                            anno_set.Reset();
                            IFeature anno_fea = (IFeature)anno_set.Next();
                            while (anno_fea != null)
                            {
                                ++ct;
                                WriteLogEntry("   Graphic being added for anno #" + ct.ToString());
                                IFillShapeElement polygonElement = new PolygonElementClass();
                                polygonElement.Symbol = polygonSymbol;
                                IElement ele = (IElement)polygonElement;
                                IGeometry geometry = anno_fea.ShapeCopy;
                                if (geometry != null)
                                {
                                    ele.Geometry = geometry;
                                    graphicsContainer.AddElement(ele, 0);
                                }
                                else
                                {
                                    WriteLogEntry("   Anno has null geometry!");
                                }
                                //
                                anno_fea = (IFeature)anno_set.Next();
                            }
                        }

                        // If there are annos add the line graphic
                        if (has_annos)
                        {
                            LineElementClass line_element = new LineElementClass();
                            line_element.Symbol = lineSymbol;
                            IGeometry geometry = feature.ShapeCopy;
                            line_element.Geometry = geometry;
                            graphicsContainer.AddElement((IElement)line_element, 0);
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteLogEntry("\r\nEXCEPTION OCCURRED! \r\n" + ex.Message + "\r\n" + ex.StackTrace);
                    }
                }
                //
                feature = enumFeature.Next();
            }
            //
            m_map.ClearSelection();
            IActiveView activeview = (IActiveView)m_map;
            activeview.Refresh();

            return true;
        }

        /// <summary>
        /// Delete all graphics from the current map
        /// </summary>
        public void DeleteAllGraphics()
        {
            IGraphicsContainer graphicsContainer;
            graphicsContainer = (IGraphicsContainer)m_map;
            graphicsContainer.DeleteAllElements();
            IActiveView activeview = (IActiveView)m_map;
            activeview.Refresh();
        }
        /// <summary>
        /// Returns a set of related annos for fea. Uses the presence of "ANNO" in the destination
        /// dataset name to determine if the related class is annotation.
        /// </summary>
        public ISet GetRelatedAnnotations(IFeatureClass fc, IFeature fea)
        {
            ISet anno_set = new Set();
            IEnumRelationshipClass enum_rel = fc.get_RelationshipClasses(esriRelRole.esriRelRoleOrigin);
            enum_rel.Reset();
            IRelationshipClass rel = enum_rel.Next();
            WriteLogEntry("Getting annos for " + fc.AliasName);
            while (rel != null)
            {
                IDataset ds = (IDataset)rel;
                //WriteLogEntry("  Dataset is " + ds.Name);
                //WriteLogEntry("  Destination class is  " + rel.DestinationClass.AliasName);
                //WriteLogEntry("  Dataset type is  " + ds.Type.ToString());
                if (rel.DestinationClass.AliasName.ToUpper().Contains("ANNO"))
                {
                    WriteLogEntry(" Anno name is  " + ds.Name);
                    ISet set = rel.GetObjectsRelatedToObject(fea);
                    WriteLogEntry("    Returned " + set.Count.ToString() + " annos");
                    if (set.Count != 0)
                    {
                        set.Reset();
                        IFeature anno_fea = (IFeature)set.Next();
                        while (anno_fea != null)
                        {
                            anno_set.Add(anno_fea);
                            anno_fea = (IFeature)set.Next();
                        }
                    }
                }
                //
                rel = enum_rel.Next();
            }
            //
            WriteLogEntry("Anno set count is " + anno_set.Count.ToString());
            return anno_set;

        }
        /// <summary>
        /// This sets up the color "wheel". And the list of conductor feature classes.
        /// When adding new colors set the RGB attributes in GetNextColor
        /// </summary>
        public void SetUpColorsAndLayers()
        {
            if (m_dialog.rdoColorConv.Checked == true)
            {
                string colors = "BLACK,RED,BLUE,LIME,YELLOW,CYAN,MAGENTA,GRAY,PURPLE,NAVY,GREEN";
                m_arr_colors = colors.Split(',');
            }
            if (m_dialog.rdoColorEnhanced.Checked == true)
            {
                string colors = "BLACK,ORANGE,SKY BLUE,BLUE GREEN,YELLOW,BLUE,VERMILLION,RED PURPLE";
                m_arr_colors = colors.Split(',');
            }
            m_coloridx = 1;
            //
            string layers = "EDGIS.PRIUGCONDUCTOR,EDGIS.PRIOHCONDUCTOR,EDGIS.SECUGCONDUCTOR,EDGIS.SECOHCONDUCTOR";
            layers = layers + "EDGIS.DCCONDUCTOR,EDGIS.CONDUITSYSTEM,EDGIS.DEACTIVATEDELECTRICLINESEGMENT";
            m_arr_layers = layers.Split(',');
        }
        /// <summary>
        /// Returns the next color in the "wheel".
        /// cref = http://www.rapidtables.com/web/color/RGB_Color.htm
        /// cref = http://chronicle.com/blogs/profhacker/color-blind-accessible-figures/59189?cid=wc&utm_source=wc&utm_medium=en
        /// </summary>
        public IRgbColor GetNextColor()
        {
            IRgbColor rgbColor = new RgbColor();
            rgbColor.Red = 0;
            rgbColor.Blue = 0;
            rgbColor.Green = 0;

            ++m_coloridx;
            if (m_coloridx > m_arr_colors.Length)
                m_coloridx = 1;

            #region Conventional Colors
            if (m_dialog.rdoColorConv.Checked == true)
            {
                switch (m_coloridx)
                {
                    case 1:
                        // Black
                        rgbColor.Red = 0;
                        rgbColor.Green = 0;
                        rgbColor.Blue = 0;
                        break;
                    case 2:
                        // Red
                        rgbColor.Red = 255;
                        rgbColor.Green = 0;
                        rgbColor.Blue = 0;
                        break;
                    case 3:
                        // Blue
                        rgbColor.Red = 0;
                        rgbColor.Green = 0;
                        rgbColor.Blue = 255;
                        break;
                    case 4:
                        // Lime
                        rgbColor.Red = 0;
                        rgbColor.Green = 255;
                        rgbColor.Blue = 0;
                        break;
                    case 5:
                        // Yellow
                        rgbColor.Red = 255;
                        rgbColor.Green = 255;
                        rgbColor.Blue = 0;
                        break;
                    case 6:
                        // Cyan
                        rgbColor.Red = 0;
                        rgbColor.Green = 255;
                        rgbColor.Blue = 255;
                        break;
                    case 7:
                        // Magenta
                        rgbColor.Red = 255;
                        rgbColor.Green = 0;
                        rgbColor.Blue = 255;
                        break;
                    case 8:
                        // Gray
                        rgbColor.Red = 128;
                        rgbColor.Green = 128;
                        rgbColor.Blue = 128;
                        break;
                    case 9:
                        // Purple
                        rgbColor.Red = 128;
                        rgbColor.Green = 0;
                        rgbColor.Blue = 128;
                        break;
                    case 10:
                        // Navy
                        rgbColor.Red = 0;
                        rgbColor.Green = 0;
                        rgbColor.Blue = 128;
                        break;
                    case 11:
                        // Green
                        rgbColor.Red = 0;
                        rgbColor.Green = 128;
                        rgbColor.Blue = 0;
                        break;
                }
            }
            #endregion
            
            #region Enhanced Colors
            if (m_dialog.rdoColorEnhanced.Checked == true)
            {
                switch (m_coloridx)
                {
                    case 1:
                        // Black
                        rgbColor.Red = 0;
                        rgbColor.Green = 0;
                        rgbColor.Blue = 0;
                        break;
                    case 2:
                        // ORANGE
                        rgbColor.Red = 230;
                        rgbColor.Green = 159;
                        rgbColor.Blue = 0;
                        break;
                    case 3:
                        // SKY BLUE
                        rgbColor.Red = 86;
                        rgbColor.Green = 180;
                        rgbColor.Blue = 233;
                        break;
                    case 4:
                        // BLUE GREEN
                        rgbColor.Red = 0;
                        rgbColor.Green = 158;
                        rgbColor.Blue = 115;
                        break;
                    case 5:
                        // YELLOW
                        rgbColor.Red = 240;
                        rgbColor.Green = 228;
                        rgbColor.Blue = 66;
                        break;
                    case 6:
                        // BLUE
                        rgbColor.Red = 0;
                        rgbColor.Green = 114;
                        rgbColor.Blue = 178;
                        break;
                    case 7:
                        // VERMILLION
                        rgbColor.Red = 213;
                        rgbColor.Green = 94;
                        rgbColor.Blue = 0;
                        break;
                    case 8:
                        // RED PURPLE
                        rgbColor.Red = 80;
                        rgbColor.Green = 60;
                        rgbColor.Blue = 70;
                        break;
                }
            }
            #endregion

            return rgbColor;

        }

        #region Utility methods - log file
        ///-------------------------------
        /// <summary>
        /// 
        /// </summary>
        /// ------------------------------
        public void WriteLogEntry(string entry)
        {
            switch (entry)
            {
                case "Start":
                    //m_Logger.WriteLine("Start time " + DateTime.Now);
                    //m_Logger.WriteLine("-----------------------------");
                    //m_Logger.Flush();
                    break;

                case "End":
                    //m_Logger.WriteLine("-----------------------------");
                    //m_Logger.WriteLine("End time " + DateTime.Now);
                    //m_Logger.Flush();
                    break;

                default:
                    //m_Logger.WriteLine(entry);
                    //m_Logger.Flush();
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool CloseFiles()
        {
            try
            {
                //m_Logger.Flush();
                //m_Logger.Close();
            }
            catch
            { }
            return true;
        }

        #endregion


    }//
}//
