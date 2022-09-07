using System;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using PGE.BatchApplication.DLMTools.Utility;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using Miner.ComCategories;
using Miner.Interop;

namespace PGE.BatchApplication.DLMTools.Commands
{
    /// <summary>
    /// PGE Annotation Alignment tool intended primarily to align conductor and conduit text
    /// </summary>
    [Guid("25839BD2-C49D-4095-B494-02B2292221C1")]
    [ComponentCategory(ComCategory.ArcMapCommands)]
    [ProgId("PGE.BatchApplication.DLMTools.AlignAnnoCommand")]
    [ComVisible(true)]
    public class AlignAnnoCommand : BaseCommand
    {
        #region COM Registration Function(s)

        [ComRegisterFunction()]
        [ComVisible(false)]
        private static void Register(string regKey)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcMapCommands.Register(regKey);
        }

        [ComUnregisterFunction()]
        [ComVisible(false)]
        private static void UnRegister(string regKey)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcMapCommands.Unregister(regKey);
        }

        #endregion

        #region Private Variables

        /// <summary>
        /// Currently running ArcMap instance
        /// </summary>
        private IApplication _application = null;

        /// <summary>
        /// Logger to log error/debug messages
        /// </summary>
        private static readonly Utility.Logs.Log4NetFileHelper _logger = new Utility.Logs.Log4NetFileHelper(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// To store characteristics about the leading text element
        /// </summary>
        private double _leadingTextAngle;
        private esriTextVerticalAlignment _leadingTextVertAlignment;
        private esriTextHorizontalAlignment _leadingTextHorizontalAlignment;

        #endregion

        #region Constructor

        public AlignAnnoCommand()
        {
            //
            // TODO: Define values for the public properties
            //
            base.m_category = "PGE Conversion Tools"; //localizable text 
            base.m_caption = "Align Annotation";  //localizable text 
            base.m_message = "Align Annotation";  //localizable text
            base.m_toolTip = "Aligns two pieces of annotation next to each other";  //localizable text
            base.m_name = "AlignAnnoCommand";   //unique id, non-localizable (e.g. "MyCategory_ArcMapTool")
            try
            {
                base.m_bitmap = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("PGE.BatchApplication.DLMTools.Bitmaps.AlignAnno.bmp"));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message, "Invalid Bitmap");
            }
        }

        #endregion

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

        public override bool Enabled
        {
            get
            {
                IEditor editor = GetEditor();
                if (editor.EditState == esriEditState.esriStateEditing)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public override void OnClick()
        {
            try
            {
                // We'll need the active view and a map
                IMxDocument mxDocument = _application.Document as IMxDocument;
                IActiveView activeView = mxDocument.FocusMap as IActiveView;
                IMap map = mxDocument.FocusMap as IMap;

                // Also, a workspace
                IWorkspace ws = GetWorkspace();                

                // Get the feature class with the ALIGNANNOFRONT model name - this will be in front of the 'back' text
                IEnumFeatureClass featureClassesFront = ModelNameFacade.ModelNameManager.FeatureClassesFromModelNameWS(ws, SchemaInfo.General.ObjectTools.AlignAnnoFront);                
                if (featureClassesFront != null)
                {
                    // Get the feature class with the ALIGNANNOBACK model name - this will be moved behind the 'front' text
                    IEnumFeatureClass featureClassesBack = ModelNameFacade.ModelNameManager.FeatureClassesFromModelNameWS(ws, SchemaInfo.General.ObjectTools.AlignAnnoBack);
                    if (featureClassesBack != null)
                    {
                        // Get the selected feature for each; will return null if more than one is selected
                        IFeature frontAnno = GetSelectedFeature(map, featureClassesFront);
                        IFeature backAnno = GetSelectedFeature(map, featureClassesBack);

                        // If we got one of each...
                        if (frontAnno != null && backAnno != null)
                        {
                            // Get a bounding box (not envelope) around the text on the front
                            IElement frontAnnoElement = GetAnnoElement(frontAnno);

                            // Save some of the annotation properties about the leading text element
                            StoreAnnoProps(frontAnnoElement);

                            // Grab the bottom right end of the bounding box.. this is our point to move to
                            IPoint pointOfInsertion = GetEndPoint(frontAnno, activeView.ScreenDisplay as IDisplay, frontAnnoElement);

                            // Update the other annotation to be behind the first
                            if (pointOfInsertion != null)
                            {
                                // And update its position
                                UpdateBackAnno(backAnno, pointOfInsertion);
                            }
                            else
                            {
                                _logger.DefaultLogger.Warn(base.m_name + " failed to determine point of insertion");
                            }

                            // Update the screen
                            activeView.Refresh();
                        }
                        else
                        {
                            _logger.DefaultLogger.Warn(base.m_name + " Could not find only one selected feature of each type");
                        }
                    }
                    else
                    {
                        _logger.DefaultLogger.Warn(base.m_name + " Could not find feature class with model name " + SchemaInfo.General.ObjectTools.AlignAnnoBack + " assigned");
                    }
                }
                else
                {
                    _logger.DefaultLogger.Warn(base.m_name + " Could not find feature class with model name " + SchemaInfo.General.ObjectTools.AlignAnnoFront + " assigned");
                }
            }
            catch (Exception ex)
            {
                _logger.DefaultLogger.Error(ex.ToString());
            }

        }

        #endregion Overridden Class Methods

        #region Private

        /// <summary>
        /// Returns a workspace
        /// </summary>
        /// <returns></returns>
        private IWorkspace GetWorkspace()
        {
            // Log entry
            string name = MethodInfo.GetCurrentMethod().Name;
            _logger.DefaultLogger.Debug("Entered " + name);

            // Get and return the logged in workspace
            IMMLoginUtils utils = new MMLoginUtils();
            return utils.LoginWorkspace;
        }

        /// <summary>
        /// Returns an IEditor
        /// </summary>
        /// <returns></returns>
        private IEditor GetEditor()
        {
            // Log entry
            string name = MethodInfo.GetCurrentMethod().Name;
            _logger.DefaultLogger.Debug("Entered " + name);

            // Get an editor
            UID pID = new UIDClass();
            pID.Value = "esriEditor.Editor";
            return _application.FindExtensionByCLSID(pID) as IEditor;
        }

        private IFeature GetSelectedFeature(IMap map, IEnumFeatureClass classes)
        {
            // Log entry
            string name = MethodInfo.GetCurrentMethod().Name;
            _logger.DefaultLogger.Debug("Entered " + name);

            // Assume we get nada
            IFeature feature = null;
            ICursor selectedFeatureCursor = null;
            bool bFound = false;

            try
            {
                IFeatureClass fc = classes.Next();
                while (fc != null)
                {
                    // Grab the layer for the supplied feature class
                    IFeatureLayer layer = GetFeatureLayer(map, fc);

                    // Grab it seleceted features
                    IFeatureSelection selection = layer as IFeatureSelection;
                    ISelectionSet set = selection.SelectionSet;

                    // If there is one and only one...
                    if (set.Count == 1)
                    {
                        if (bFound == false)
                        {
                            // Get the selected feature                    
                            set.Search(null, false, out selectedFeatureCursor);
                            IRow row = selectedFeatureCursor.NextRow();
                            feature = row as IFeature;

                            // Keep track that we found one
                            bFound = true;
                        }
                        else
                        {
                            // We found more than one... bail out!
                            feature = null;
                            break;
                        }
                    }

                    // Get the next feature class
                    fc = classes.Next();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (selectedFeatureCursor != null)
                {
                    Marshal.ReleaseComObject(selectedFeatureCursor);
                }
            }

            // Return the result
            return feature;
        }

        /// <summary>
        /// Returns the single selected feature of the supplied feature class
        /// </summary>
        /// <param name="map">Active map</param>
        /// <param name="fc">Feature class to get selected feature</param>
        /// <returns>selected feature; null if 0 or >1 selected features present</returns>
        private IFeature GetSelectedFeature(IMap map, IFeatureClass fc)
        {
            // Log entry
            string name = MethodInfo.GetCurrentMethod().Name;
            _logger.DefaultLogger.Debug("Entered " + name);

            // Assume we get nada
            IFeature feature = null;
            ICursor selectedFeatureCursor = null;

            try
            {
                // Grab the layer for the supplied feature class
                IFeatureLayer frontLayer = GetFeatureLayer(map, fc);

                // Grab it seleceted features
                IFeatureSelection frontSelection = frontLayer as IFeatureSelection;
                ISelectionSet frontSet = frontSelection.SelectionSet;

                // If there is one and only one...
                if (frontSet.Count == 1)
                {
                    // Get the selected feature                    
                    frontSet.Search(null, false, out selectedFeatureCursor);

                    IRow row = selectedFeatureCursor.NextRow();
                    feature = row as IFeature;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (selectedFeatureCursor != null)
                {
                    Marshal.ReleaseComObject(selectedFeatureCursor);
                }
            }

            // Return the result
            return feature;
        }

        /// <summary>
        /// Returns the layer for the supplied feature class
        /// </summary>
        /// <param name="map"></param>
        /// <param name="featureClass"></param>
        /// <returns></returns>
        private IFeatureLayer GetFeatureLayer(IMap map, IFeatureClass featureClass)
        {
            // Log entry
            string name = MethodInfo.GetCurrentMethod().Name;
            _logger.DefaultLogger.Debug("Entered " + name);

            IFeatureLayer featLayer = null;
            IEnumLayer enumLayer = null;

            try
            {
                // Get all layers in the map
                UID uid = new UID();
                uid.Value = "{40A9E885-5533-11d0-98BE-00805F7CED21}";
                enumLayer = map.get_Layers(uid, true);

                // For each layer
                ILayer layer;
                while ((layer = enumLayer.Next()) != null)
                {
                    // Check to see if the layer is for our feature class
                    featLayer = layer as IFeatureLayer;
                    if (featLayer != null)
                    {
                        if ((featLayer.FeatureClass as IDataset).Name == (featureClass as IDataset).Name)
                        {
                            // If it is, save it and bail out
                            featLayer = layer as IFeatureLayer;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
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

        /// <summary>
        /// Returns a modified annotation element for the supplied feature. The modification
        /// seeks to make the annotation element longer than it really is.
        /// </summary>
        /// <param name="annotation"></param>
        /// <returns>Annotation element for the supplied feature; null if the supplied feature isn't annotation</returns>
        private IElement GetAnnoElement(IFeature annotation)
        {
            // Log entry
            string name = MethodInfo.GetCurrentMethod().Name;
            _logger.DefaultLogger.Debug("Entered " + name);

            // Default to return nada
            IElement annoElement = null;

            try
            {
                // If the feature passed in is indeed annotation
                IAnnotationFeature annoFeature = annotation as IAnnotationFeature;
                if (annoFeature != null)
                {
                    // Grab its annotation element
                    annoElement = annoFeature.Annotation as IElement;

                    // Extend the text with some fake chars to give our bounding box an offset
                    ITextElement frontAnnoTextElement = annoElement as ITextElement;
                    //frontAnnoTextElement.Text = frontAnnoTextElement.Text + "----";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            // Return the result
            return annoElement;
        }

        private void StoreAnnoProps(IElement annoElement)
        {
            ITextSymbol annoSymbol = (annoElement as ITextElement).Symbol;
            _leadingTextAngle = annoSymbol.Angle;
            _leadingTextVertAlignment = annoSymbol.VerticalAlignment;
            _leadingTextHorizontalAlignment = annoSymbol.HorizontalAlignment;
        }

        private IElement SaveAnnoProps(IElement annoElement)
        {
            ITextElement textElement = annoElement as ITextElement;
            ITextSymbol annoSymbol = textElement.Symbol;
            annoSymbol.Angle = _leadingTextAngle;
            annoSymbol.VerticalAlignment = _leadingTextVertAlignment;
            annoSymbol.HorizontalAlignment = _leadingTextHorizontalAlignment;

            textElement.Symbol = annoSymbol;
            return textElement as IElement;
        }

        /// <summary>
        /// Rotates one point around another
        /// </summary>
        /// <param name="pointToRotate">The point to rotate.</param>
        /// <param name="centerPoint">The centre point of rotation.</param>
        /// <param name="angleInDegrees">The rotation angle in degrees.</param>
        /// <returns>Rotated point</returns>
        private IPoint RotatePoint(IPoint pointToRotate, IPoint centerPoint, double angleInDegrees)
        {
            double originX = centerPoint.X;
            double originY = centerPoint.Y;

            double rotateX = pointToRotate.X;
            double rotateY = pointToRotate.Y;

            double angleInRadians = angleInDegrees * (Math.PI / 180);
            double cosTheta = Math.Cos(angleInRadians);
            double sinTheta = Math.Sin(angleInRadians);

            double newX = (cosTheta * (rotateX - originX) - sinTheta * (rotateY - originY) + originX);
            double newY = (sinTheta * (rotateX - originX) + cosTheta * (rotateY - originY) + originY);

            pointToRotate.X = newX;
            pointToRotate.Y = newY;

            return pointToRotate;
        }

        /// <summary>
        /// Returns the end point of the supplied feature. This end point is based on a irregular 
        /// box (ie: not the envelope) around the geometry.
        /// </summary>
        /// <param name="feature"></param>
        /// <param name="display"></param>
        /// <param name="frontAnnoElement"></param>
        /// <returns>IPoint representing the end; null if it could not be calculated</returns>
        private IPoint GetEndPoint(IFeature feature, IDisplay display, IElement frontAnnoElement)
        {
            // Log entry
            string name = MethodInfo.GetCurrentMethod().Name;
            _logger.DefaultLogger.Debug("Entered " + name);

            // Assume we can't generate one
            IPoint pointOfInsertion = null;
            double originalAngle = 0;

            try
            {
                // Update the leading text geometry to not have an angle and be a bit longer (we won't store this)
                ITextElement frontTextElement = frontAnnoElement as ITextElement;
                ITextSymbol frontSymbol = frontTextElement.Symbol;
                originalAngle = frontSymbol.Angle;
                frontSymbol.Angle = 0;
                frontTextElement.Symbol = frontSymbol;
                frontTextElement.Text = frontTextElement.Text + "--";
                //frontAnnoElement = frontTextElement as IElement;

                // Generate a bounding box around the annotation feature (think: blue selection outline)
                IPolygon frontAnnoOutline = new PolygonClass();
                frontAnnoElement.QueryOutline(display, frontAnnoOutline);

                // Get the X from the end of the outline, get the Y from the original geometry
                pointOfInsertion = new PointClass();
                pointOfInsertion.X = frontAnnoOutline.Envelope.XMax;
                pointOfInsertion.Y = (frontAnnoElement.Geometry as IPoint).Y;

                // Rotate it about the origin of the front geometry by the angle of the front geometry
                RotatePoint(pointOfInsertion, frontAnnoElement.Geometry as IPoint, originalAngle);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return pointOfInsertion;
        }

        /// <summary>
        /// Updates the suppied annotation feature to begin at the supplied point of insertion
        /// and saves the result in the database nested in an edit op.
        /// </summary>
        /// <param name="feature"></param>
        /// <param name="pointOfInsertion"></param>
        private void UpdateBackAnno(IFeature feature, IPoint pointOfInsertion)
        {
            // Log entry
            string name = MethodInfo.GetCurrentMethod().Name;
            _logger.DefaultLogger.Debug("Entered " + name);

            // Get an editor to work with
            IEditor editor = GetEditor();

            try
            {
                // Update the geometry of the feature to start at the supplied point
                IAnnotationFeature annoFeature = feature as IAnnotationFeature;
                IElement annoElement = annoFeature.Annotation as IElement;

                // Update the annotation props, such as angle and alignment, to match the front
                annoElement = SaveAnnoProps(annoElement);

                // Update the annotation geometries anchor point to be at the supplied point
                annoElement.Geometry = pointOfInsertion;
                annoFeature.Annotation = annoElement;

                // Save the update
                editor.StartOperation();
                feature.Store();
                editor.StopOperation("Annotation Alignment");
            }
            catch (Exception ex)
            {
                // Abort and raise
                editor.AbortOperation();
                throw ex;
            }
        }

        #endregion
    }
}
