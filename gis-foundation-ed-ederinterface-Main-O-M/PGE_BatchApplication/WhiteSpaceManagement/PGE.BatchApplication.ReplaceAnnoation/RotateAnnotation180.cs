using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Reflection;
using System.Resources;

using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.CartoUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.SystemUI;

using Miner.ComCategories;

namespace PGE.BatchApplication.ReplaceAnnoation
{

    /// <summary>
    /// Summary description for RotateAnnotation180
    /// </summary>
    [Guid("198946EC-BFCA-4C36-99FF-184B0AA8E5D3")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("PGE.BatchApplication.ReplaceAnnoation.RotateAnnotation180")]
    [ComponentCategory(ComCategory.ArcMapCommands)]
    public sealed class RotateAnnotation180 : ICommand
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

        #region Rotate Annotation 180
        //Note: Translated by Matt Kempf from VBScript provided by Phanirajugupta Palla via email on 3/26/2014

        private void RotateAnnotation()
        {
            UID editorClassID = new UID();
            editorClassID.Value = "esriEditor.Editor";
            IEditor editor = (IEditor)_application.FindExtensionByCLSID(editorClassID);
            
            if (editor.EditState != esriEditState.esriStateEditing)
            {
                MessageBox.Show("Rotate Annotation 180 requires an active edit session.", "PG&E Rotate Annotation");
                return;
            }

            IMap map = _mxDoc.FocusMap;

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

                        _mxDoc.ActiveView.Refresh();
                    }
                    feature = features.Next();
                }  // while 
            }  // if 
        }   // method

        #endregion // Rotate Annotation 180

        #region ICommand Implementation

        //ICommand items...
        private IApplication _application;
        private IMxDocument _mxDoc = null;
        private string _caption = "Rotate Annotation 180 degrees";
        private string _category = "PGE Tools";
        private bool _enabled = false;
        private string _message = "Rotate Annotation 180 degrees";
        private string _name = "Rotate Annotation";
        private string _toolTip = "Rotate Annotation 180 degrees";

        
       
        public int Bitmap
        {
            get
            {
                string bitmapResourceName = GetType().Name + ".bmp";
                return new Bitmap(GetType(), bitmapResourceName).GetHbitmap().ToInt32();
            }
        }

        public string Caption
        {
            get { return _caption; }
        }

        public string Category
        {
            get { return _category; }
        }

        public bool Checked
        {
            get { return false; }
        }

        public bool Enabled
        {
            get { return _enabled; }
        }

        public int HelpContextID
        {
            get { throw new NotImplementedException(); }
        }

        public string HelpFile
        {
            get { throw new NotImplementedException(); }
        }

        public string Message
        {
            get { return _message; }
        }

        public string Name
        {
            get { return _name; }
        }

        public void OnClick()
        {
            RotateAnnotation();
        }

        public void OnCreate(object hook)
        {
            _application = hook as IApplication;

            //Disable if it is not ArcMap
            if (hook is IMxApplication)
            {
                _enabled = true;
                _mxDoc = _application.Document as IMxDocument;
            }
            else
            {
                _enabled = false;
            }
        }

        public string Tooltip
        {
            get { return _toolTip; }
        }

#endregion // ICommand Implementation
    }
}
