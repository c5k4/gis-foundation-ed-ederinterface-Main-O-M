using System;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
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
    /// Summary description for SwapAnnotationPositions
    /// //Note: Translated by Matt Kempf from VBScript provided by Phanirajugupta Palla via email on 3/26/2014
    /// </summary>
    [Guid("562237A5-BF42-414A-8EE6-111B64725484")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("PGE.BatchApplication.DLMTools.SwapAnnotationPosition")]
    [ComponentCategory(ComCategory.ArcMapCommands)]
    public sealed class SwapAnnotationPositions : BaseCommand
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

        public SwapAnnotationPositions()
        {
            //
            // TODO: Define values for the public properties
            //
            m_category = "PGE Conversion Tools"; //localizable text
            m_caption = "Swap Annotation Positions";  //localizable text
            m_message = "Swap Annotation Positions";  //localizable text 
            m_toolTip = "Swap Annotation Positions";  //localizable text 
            m_name = "DLMTools_Swap_Annotation_Positions";   //unique id, non-localizable (e.g. "MyCategory_ArcMapCommand")

            try
            {
                m_bitmap =
                    new Bitmap(
                        Assembly.GetExecutingAssembly()
                            .GetManifestResourceStream("PGE.BatchApplication.DLMTools.Bitmaps.SwapAnnotationPositions.bmp"));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message, "Invalid Bitmap");
            }
        }

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

        public override void OnClick()
        {
            UID editorClassID = new UID();
            editorClassID.Value = "esriEditor.Editor";
            IEditor editor = (IEditor)_application.FindExtensionByCLSID(editorClassID);
            IMxDocument mxDoc = (IMxDocument) _application.Document;

            if (editor.EditState != esriEditState.esriStateEditing)
            {
                MessageBox.Show("Swap Annotation Positions requires an active edit session.", "PG&E Rotate Annotation");
                return;
            }

            IMap map = mxDoc.FocusMap;

            if (map.SelectionCount != 2)
            {
                MessageBox.Show("Swap Annotation Positions requires two annotation features to be selected.", "PG&E Rotate Annotation");
                return;
            }
            else
            {
                IEnumFeature features = (IEnumFeature)map.FeatureSelection;
                IFeature annotation1 = features.Next();
                IFeature annotation2 = features.Next();

                if (annotation1 == null || annotation2 == null || annotation1.FeatureType != esriFeatureType.esriFTAnnotation || annotation2.FeatureType != esriFeatureType.esriFTAnnotation)
                { 
                    MessageBox.Show("Swap Annotation Positions requires two annotation features to be selected.", "PG&E Rotate Annotation");
                    return;
                }

                IPoint point1 = new ESRI.ArcGIS.Geometry.Point();
                IPoint point2 = new ESRI.ArcGIS.Geometry.Point();

                IPointCollection annotation1Points = (IPointCollection)annotation1.Shape;
                IPointCollection annotation2Points = (IPointCollection)annotation2.Shape;

                if (map.ComputeDistance(annotation1Points.Point[0], annotation2Points.Point[0]) < map.ComputeDistance(annotation1Points.Point[3], annotation2Points.Point[3]))
                {
                    point1 = annotation1Points.Point[0];
                    point2 = annotation2Points.Point[0];
                }
                else
                {
                    point1 = annotation1Points.Point[3];
                    point2 = annotation2Points.Point[3];                
                }

                editor.StartOperation();

                ILine line1 = new Line();
                line1.FromPoint = point1;
                line1.ToPoint = point2;

                ILine line2 = new Line();
                line2.FromPoint = point2;
                line2.ToPoint = point1;

                EditFeature(annotation1, line1);
                EditFeature(annotation2, line2);

                editor.StopOperation("Swap Annotation Positions");
                
                mxDoc.ActiveView.Refresh();
                
            }  // else if
        }  // method

        private void EditFeature(IFeature annotationFeature, ILine line)
        {
            IFeatureEdit featureEdit = (IFeatureEdit)annotationFeature;
            ISet temporarySet = new Set();
            temporarySet.Add(annotationFeature);
            featureEdit.MoveSet(temporarySet, line);

            annotationFeature = (IFeature)featureEdit;
            annotationFeature.Store();
        }
    }
}
