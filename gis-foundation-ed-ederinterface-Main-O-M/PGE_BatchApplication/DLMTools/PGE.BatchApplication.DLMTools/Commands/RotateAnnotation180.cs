using System;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.SystemUI;
using Miner.ComCategories;

namespace PGE.BatchApplication.DLMTools.Commands
{

    /// <summary>
    /// Summary description for RotateAnnotation180
    /// //Note: Translated by Matt Kempf from VBScript provided by Phanirajugupta Palla via email on 3/26/2014
    /// </summary>
    [Guid("198946EC-BFCA-4C36-99FF-184B0AA8E5D3")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("PGE.BatchApplication.DLMTools.RotationAnnotation180")]
    [ComponentCategory(ComCategory.ArcMapCommands)]
    public sealed class RotateAnnotation180 : BaseCommand
    {
        #region COM Registration Function(s)
        [ComRegisterFunction()]
        [ComVisible(false)]
        static void RegisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryRegistration(registerType);

            //
            // TODO: Add any COM registration code here
            //
        }

        [ComUnregisterFunction()]
        [ComVisible(false)]
        static void UnregisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryUnregistration(registerType);

            //
            // TODO: Add any COM unregistration code here
            //
        }

        #region ArcGIS Component Category Registrar generated code
        /// <summary>
        /// Required method for ArcGIS Component Category registration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryRegistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            MxCommands.Register(regKey);

        }
        /// <summary>
        /// Required method for ArcGIS Component Category unregistration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryUnregistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            MxCommands.Unregister(regKey);

        }

        #endregion
        #endregion

        private IApplication _application;

        public RotateAnnotation180()
        {
            //
            // TODO: Define values for the public properties
            //
            m_category = "PGE Conversion Tools"; //localizable text
            m_caption = "Rotate Annotation 180 degrees";  //localizable text
            m_message = "Rotate Annotation 180 degrees";  //localizable text 
            m_toolTip = "Rotate Annotation 180 degrees";  //localizable text 
            m_name = "DLMTools_Rotate_Annotation_180";   //unique id, non-localizable (e.g. "MyCategory_ArcMapCommand")

            try
            {
                m_bitmap =
                    new Bitmap(
                        Assembly.GetExecutingAssembly()
                            .GetManifestResourceStream("PGE.BatchApplication.DLMTools.Bitmaps.RotateAnnotation180.bmp"));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message, "Invalid Bitmap");
            }
        }

        public override void OnClick()
        {
            UID editorClassID = new UID();
            editorClassID.Value = "esriEditor.Editor";
            IEditor editor = (IEditor)_application.FindExtensionByCLSID(editorClassID);
            IMxDocument mxDoc = (IMxDocument) _application.Document;

            if (editor.EditState != esriEditState.esriStateEditing)
            {
                MessageBox.Show("Rotate Annotation 180 requires an active edit session.", "PG&E Rotate Annotation");
                return;
            }

            IMap map = mxDoc.FocusMap;

            if (map.SelectionCount > 0)
            {
                IEnumFeature features = (IEnumFeature)map.FeatureSelection;
                features.Reset();
                IFeature feature = features.Next();

                while (feature != null)
                {
                    if (feature.FeatureType == esriFeatureType.esriFTAnnotation)
                    {
                        IGeometry geometry = feature.Shape;
                        IEnvelope envelope = geometry.Envelope;
                        IPoint originalPoint = envelope.UpperLeft;

                        editor.StartOperation();

                        IAnnotationFeature annotationFeature = (IAnnotationFeature)feature;
                        ITextElement textElement = (ITextElement)annotationFeature.Annotation;
                        ITextSymbol textSymbol = textElement.Symbol;

                        double originalAngle = textSymbol.Angle;
                        double newAngle = 0;

                        if (originalAngle >= 180)
                        {
                            newAngle = originalAngle - 180;
                        }
                        else
                        {
                            newAngle = originalAngle + 180;
                        }

                        textSymbol.Angle = newAngle;
                        textElement.Symbol = textSymbol;
                        annotationFeature.Annotation = (IElement)textElement;
                        feature = (IFeature)annotationFeature;
                        feature.Store();

                        IPoint newPoint = feature.Extent.UpperLeft;
                        IFeatureEdit featureEdit = (IFeatureEdit)feature;
                        ILine line = new Line();
                        line.FromPoint = newPoint;
                        line.ToPoint = originalPoint;

                        ISet temporarySet = new Set();
                        temporarySet.Add(feature);
                        featureEdit.MoveSet(temporarySet, line);
                        feature = (IFeature)featureEdit;
                        feature.Store();

                        editor.StopOperation("Rotate Annotation");

                        mxDoc.ActiveView.Refresh();
                    }
                    feature = features.Next();
                }  // while 
            }  // if 
        }   // method

        public override void OnCreate(object hook)
        {
            _application = hook as IApplication;

            //Disable if it is not ArcMap
            if (hook is IMxApplication)
            {
                m_enabled = true;
            }
            else
            {
                m_enabled = false;
            }
        }
    }
}
