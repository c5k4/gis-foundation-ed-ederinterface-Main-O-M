using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.SystemUI;

using Miner.Framework;
using Miner.FrameworkUI;
using Miner.Geodatabase;
using Miner.Interop;

using PGE.Common.Delivery.Diagnostics;
using PGE.Common.Delivery.Framework;
using PGE.Common.Delivery.UI.Tools;

namespace PGE.Desktop.EDER.UFM.Operations
{
    public partial class CreateButterflyAnnoControl : OperationUIControl
    {
        #region Member vars

        /// <summary>
        /// Logger to log error/debug messages
        /// </summary>
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        /// <summary>
        /// Currently running ArcMap instance
        /// </summary>
        private IApplication _application = null;

        /// <summary>
        /// Current map on display
        /// </summary>
        private IMap _map = null;

        /// <summary>
        /// For selecting duct banks
        /// </summary>
        private UfmSelectTool _ufmSelectionTool;
        private GeometryFeatureSnap _ftSnap;

        /// <summary>
        /// Stores the previously selected command state
        /// </summary>
        private ICommandItem _originalCommand;

        #endregion

        #region Constructor

        public CreateButterflyAnnoControl()
        {
            InitializeComponent();
        }

        #endregion

        #region Initialize control

        private void CreateButterflyAnnoControl_Load(object sender, EventArgs e)
        {
            // Setup the font combo box
            cboFontSize.Items.Add("2");
            cboFontSize.Items.Add("3");
            cboFontSize.Items.Add("4");
            cboFontSize.Items.Add("5");
            cboFontSize.Items.Add("6");
            cboFontSize.SelectedIndex = 2;
        }

        #endregion

        #region Control event handlers

        private void butGenerateAnno_Click(object sender, EventArgs e)
        {
            _logger.Debug("Creating butterfly cross section annotation");

            try
            {
                // Store the application
                Type appRefType = Type.GetTypeFromProgID("esriFramework.AppRef");
                object appRefObj = Activator.CreateInstance(appRefType);
                _application = appRefObj as IApplication;

                // And the map
                IMxDocument mxDocument = _application.Document as IMxDocument;
                IActiveView activeView = mxDocument.FocusMap as IActiveView;
                _map = mxDocument.FocusMap as IMap;

                // Set the current tool to be the UFM selection tool
                ICommandBars commandBars = _application.Document.CommandBars;
                UID identifier = new UIDClass
                {
                    Value = "{FDD8A6D9-DB46-495a-8820-08FB994E4618}"
                };
                ICommandItem item = commandBars.Find(identifier, false, false);
                _originalCommand = _application.CurrentTool;
                _application.CurrentTool = item;

                // Setup some events for the tool
                ICommand myCommand = item.Command;
                if (myCommand is UfmSelectTool)
                {
                    _ufmSelectionTool = myCommand as UfmSelectTool;
                }
                _ufmSelectionTool.MouseMove += new EventHandler<MousePositionEventArgs>(CreateButterflyAnno_MouseMove);
                _ufmSelectionTool.MouseDown += new EventHandler<MousePositionEventArgs>(CreateButterflyAnno_MouseDown);
                _ufmSelectionTool.Deactivated += new EventHandler<EventArgs>(CreateButterflyAnno_Deactivated);
                _ufmSelectionTool.RefreshEvent += new EventHandler<EventArgs>(CreateButterflyAnno_RefreshEvent);

                // Inform the user
                Document.StatusBar.Message = "Select Duct Bank";
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to generate anno: " + ex.ToString());
            }
        }

        private void CreateButterflyAnno_MouseMove(object sender, MousePositionEventArgs e)
        {
            // Ensure we have a snap control
            if (_ftSnap == null)
            {
                _ftSnap = new GeometryFeatureSnap(Document.ActiveView, SchemaInfo.UFM.ClassModelNames.UfmDuctBank);
            }

            // Update selection based on mouse position
            _ftSnap.MouseMoveClicked(e.X, e.Y);
        }


        private void CreateButterflyAnno_MouseDown(object sender, MousePositionEventArgs e)
        {
            const string NO_XSECTION = "Cross Section Annotation must be placed for the associated Conduit prior to using this tool";
            const string TITLE = "Create Butterfly Cross Section Text";

            if ((_ftSnap != null) && (_ftSnap.SnappedFeature != null))
            {
                IFeature ductBank = _ftSnap.SnappedFeature;
                _ftSnap.ClearSnapCursor();
                _ftSnap = null;
                _ufmSelectionTool.Deactivate();
                if (_originalCommand != null)
                {
                    _application.CurrentTool = _originalCommand;
                }

                IFeature xsectionAnno = GetCrossSection(ductBank);

                // If there is one, create a new butterfly-sized x-ref anno
                if (xsectionAnno != null)
                {
                    IFeature crossSectionAnno = Create10CrossSection(ductBank, xsectionAnno);

                    if (crossSectionAnno != null)
                    {
                        (_map as IActiveView).Refresh();
                        SelectFeature(_map, crossSectionAnno);
                    }
                }
                else
                {
                    MessageBox.Show(NO_XSECTION, TITLE, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }

            }
        }

        private void CreateButterflyAnno_Deactivated(object sender, EventArgs e)
        {
            Document.StatusBar.ClearMessage();
        }

        private void CreateButterflyAnno_RefreshEvent(object sender, EventArgs e)
        {
            if (_ftSnap != null)
            {
                _ftSnap.ClearSnapCursor();
            }
        }

        #endregion

        #region Private methods

        #region Controls

        private void RevertTool()
        {
            if (_ufmSelectionTool != null)
            {
                if (_originalCommand == null)
                {
                    _ufmSelectionTool.Deactivate();
                }
                else
                {
                    _application.CurrentTool = _originalCommand;
                    _originalCommand = null;
                    _ufmSelectionTool = null;
                }
            }
        }

        #endregion

        #region Get cross section

        private IFeature GetCrossSection(IFeature ductBank)
        {
            IFeature crossSection = null;

            if (ductBank != null)
            {
                // Get its parent conduit feature
                IFeature conduit = UfmHelper.GetParentFeature(ductBank);

                // Get one of the conduits x-section annos
                if (conduit != null)
                {
                    ISet crossSections = UfmHelper.GetRelatedObjects(conduit, SchemaInfo.UFM.ClassModelNames.CrossSection); // GetChildFeatures(conduit, "CROSSSECTION");
                    if (crossSections.Count > 0)
                    {
                        crossSection = crossSections.Next() as IFeature;
                        while (crossSection != null)
                        {
                            // If its not an arrow, were good - bail out
                            if (UfmHelper.IsArrow(crossSection as IObject) == false)
                            {
                                break;
                            }

                            // Grab the next one
                            crossSection = crossSections.Next() as IFeature;
                        }
                    }
                }
            }

            return crossSection;
        }

        #endregion

        #region Create anno feature

        /// <summary>
        /// Creates annotation for the supplied duct bank
        /// </summary>
        /// <param name="ductBank"></param>
        /// <param name="originalCrossSection"></param>
        /// <returns></returns>
        private IFeature Create10CrossSection(IFeature ductBank, IObject originalCrossSection)
        {
            // Get the annotation feature class
            IFeature featCrossSection10Anno = null;
            IFeatureClass fcCrossSection10 = GetCrossSection10AnnoFC(originalCrossSection);
            _logger.Debug("Creating 1:10 x-section annotation in feature class " + fcCrossSection10.AliasName);

            try
            {
                // If we got it...
                if (fcCrossSection10 != null)
                {
                    // Get the existing annotation geometry
                    IElement clonedAnnoElement = UfmHelper.CloneAnnotationElement(originalCrossSection, 120);

                    // Update the geometry to just include text (if there is text)
                    if (UfmHelper.IsArrow(originalCrossSection) == false)
                    {
                        // Apply cosmetic updates
                        clonedAnnoElement = UpdateGroupElement(originalCrossSection, clonedAnnoElement, ductBank);
                    }

                    // Get the OID of the conduit
                    int parentOID = UfmHelper.GetParentConduitOID(originalCrossSection);
                    _logger.Debug("\tfor conduit " + parentOID.ToString());

                    // Build a new annotation feature and save it
                    featCrossSection10Anno = CreateNewAnnotation(fcCrossSection10, clonedAnnoElement, parentOID);
                    if (featCrossSection10Anno != null)
                    {
                        _logger.Debug("Saving");
                        IEditor editor = GetEditor();
                        editor.StartOperation();
                        featCrossSection10Anno.Store();
                        editor.StopOperation("Create Duct Bank Annotation");
                    }
                }
                else
                {
                    _logger.Warn("Could not find CrossSection10Anno feature class");
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to create butterfly annotation: " + ex.ToString());
            }
            
            // Return the result
            return featCrossSection10Anno;
        }

        /// <summary>
        /// Find and return the 1:10 Cross Section feature class
        /// </summary>
        /// <param name="pObject"></param>
        /// <returns></returns>
        private IFeatureClass GetCrossSection10AnnoFC(IObject pObject)
        {
            IFeatureClass fcCrossSectionAnno = null;

            try
            {
                // We'll need a workspace and a model name manager
                IWorkspace ws = (pObject.Class as IDataset).Workspace;
                IMMModelNameManager modelNameManager = ModelNameManager.Instance;

                // Look up the feature class
                string crossSection10Anno = string.Empty;
                IEnumBSTR featureClasses = modelNameManager.ClassNamesFromModelNameWS(ws, SchemaInfo.UFM.ClassModelNames.CrossSection10);
                if (featureClasses != null)
                {
                    featureClasses.Reset();
                    crossSection10Anno = featureClasses.Next();
                }

                // If we found it, open it
                if (crossSection10Anno != string.Empty)
                {
                    IFeatureWorkspace fws = ws as IFeatureWorkspace;
                    fcCrossSectionAnno = fws.OpenFeatureClass(crossSection10Anno);
                }
            }
            catch (Exception ex)
            {
                // Log a warning
                _logger.Warn(ex.Message);
            }

            // Return the result
            return fcCrossSectionAnno;
        }

        private IElement UpdateGroupElement(IObject pObject, IElement element, IFeature ductBank)
        {
            IGroupElement elements = element as IGroupElement;

            // Now lets build a new set of elements based on the existing ones
            IGroupElement newGroup = new GroupElementClass();

            // For each element...
            int count = elements.ElementCount;
            for (int index = 0; index < count; index++)
            {
                // If its a text element
                if (elements.Element[index] is ITextElement)
                {
                    // And has more than two characters (ie: its not duct text)
                    ITextElement text = elements.Element[index] as ITextElement;
                    if (text.Text.Contains(':') == true)
                    {
                        // Get the centerpoint of the duct bank
                        IPoint centroid = (ductBank.Shape as IArea).Centroid;

                        // Update the text size and location
                        IElement currentElement = UpdateElement(elements.Element[index], centroid, 0);

                        // And add it to our new element
                        newGroup.AddElement(currentElement);
                    }
                }
            }

            // Return the result
            return newGroup as IElement;
        }

        private IElement UpdateElement(IElement currentElement, IPoint elementLocation, double angle)
        {
            // If its a point
            if (currentElement.Geometry.GeometryType == esriGeometryType.esriGeometryPoint)
            {
                // Build and set the location for this element
                //IPoint elementLocation = new PointClass();
                //elementLocation.X = currentElement.Geometry.Envelope.UpperLeft.X + xOffset;
                //elementLocation.Y = currentElement.Geometry.Envelope.UpperLeft.Y + yOffset;
                currentElement.Geometry = elementLocation;

                // Get the text for the element, if applicable
                ITextElement text = currentElement as ITextElement;
                if (text != null)
                {
                    IFormattedTextSymbol ts = text.Symbol as IFormattedTextSymbol;
                    //ts.Angle = angle;
                    ts.Size = GetFontSize();
                    ts.Font.Bold = true;
                    text.Symbol = ts;
                }
            }

            return currentElement;
        }
        
        private IFeature CreateNewAnnotation(IFeatureClass fcCrossSection10, IElement clonedAnnoElement, int parentOID)
        {
            IFeature featCrossSection10Anno = null;

            try
            {
                // Create a new row
                featCrossSection10Anno = fcCrossSection10.CreateFeature();

                // Copy the shape info over
                IAnnotationFeature2 featAnnoProps = featCrossSection10Anno as IAnnotationFeature2;
                featAnnoProps.Annotation = clonedAnnoElement;
                featAnnoProps.LinkedFeatureID = parentOID;
                featAnnoProps.AnnotationClassID = 0;
                featAnnoProps.Status = esriAnnotationStatus.esriAnnoStatusPlaced;
            }
            catch (Exception ex)
            {
                // Log a warning
                _logger.Warn(ex.Message);
            }

            // Return the result
            return featCrossSection10Anno;
        }

        private int GetFontSize()
        {

            int fontSize = 4;

            try
            {
                fontSize = int.Parse(cboFontSize.Text);
            }
            catch (Exception ex)
            {
                _logger.Warn("Failed to get font size, using default of 4: " + ex.ToString()); 
            }

            return fontSize;
        }

        /// <summary>
        /// Returns an IEditor
        /// </summary>
        /// <returns></returns>
        private IEditor GetEditor()
        {
            // Log entry
            string name = MethodInfo.GetCurrentMethod().Name;
            _logger.Debug("Entered " + name);

            // Get an editor
            UID pID = new UIDClass();
            pID.Value = "esriEditor.Editor";
            return _application.FindExtensionByCLSID(pID) as IEditor;
        }

        #endregion

        #region Getting related features

        //private IFeature GetParentFeature(IObject pObject)
        //{
        //    IFeature feature = null;
        //    ISet relatedObjects = null;
        //    IRelationshipClass rc = UfmHelper.GetRelationshipByModelName(pObject.Class, SchemaInfo.UFM.ClassModelNames.Conduit);
        //    if (rc != null)
        //    {
        //        relatedObjects = rc.GetObjectsRelatedToObject(pObject);
        //    }

        //    if (relatedObjects != null)
        //    {
        //        relatedObjects.Reset();
        //        feature = relatedObjects.Next() as IFeature;
        //    }

        //    return feature;
        //}

        //private ISet GetChildFeatures(IObject pObject, string modelName)
        //{
        //    ISet relatedObjects = null;
        //    IRelationshipClass rc = GetChildRelationshipByModelName(pObject.Class, modelName);
        //    if (rc != null)
        //    {
        //        relatedObjects = rc.GetObjectsRelatedToObject(pObject);
        //    }

        //    return relatedObjects;
        //}

        //private IRelationshipClass GetChildRelationshipByModelName(IObjectClass pObjectClass, string relatedObjectModelName)
        //{
        //    IRelationshipClass relClass = null;

        //    try
        //    {
        //        IEnumRelationshipClass enumRelClass = pObjectClass.get_RelationshipClasses(esriRelRole.esriRelRoleAny);
        //        enumRelClass.Reset();
        //        IRelationshipClass rc = enumRelClass.Next();
        //        while (rc != null)
        //        {
        //            if (ModelNameManager.Instance.ContainsClassModelName(rc.DestinationClass, relatedObjectModelName))
        //            {
        //                relClass = rc;
        //                break;
        //            }
        //            rc = enumRelClass.Next();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string msg = "Error getting the relationship. Error: " + ex.Message;
        //        throw;
        //    }

        //    return relClass;
        //}

        #endregion

        #region Post processing methods

        private void SelectFeature(IMap map, IFeature crossSectionAnno)
        {
            // Get the layer of the supplied feature
            IFeatureLayer crossSectionAnnoLayer = GetFeatureLayer(map, crossSectionAnno.Class as IFeatureClass);

            // And select it
            if (crossSectionAnnoLayer != null)
            {
                map.ClearSelection();
                map.SelectFeature(crossSectionAnnoLayer, crossSectionAnno);
            }
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
            _logger.Debug("Entered " + name);

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

        #endregion

        #endregion
    }
}
