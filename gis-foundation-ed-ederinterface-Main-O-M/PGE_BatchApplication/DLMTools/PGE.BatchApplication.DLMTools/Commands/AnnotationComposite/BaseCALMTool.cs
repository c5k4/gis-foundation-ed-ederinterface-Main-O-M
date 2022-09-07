using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using PGE.BatchApplication.DLMTools.Commands.AnnotationComposite.UI;
using PGE.BatchApplication.DLMTools.Utility.Annotation;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;

namespace PGE.BatchApplication.DLMTools.Commands.AnnotationComposite
{
    /// <summary>
    /// Part of the CALM Toolbar - a base tool for processing annotation.
    /// </summary>
    [ComVisible(true)]
    public abstract class BaseCALMTool : BaseTool
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

        #region Private members
        private const int MinFontSize = 6;
        #endregion

        #region Protected Members
        protected List<IFeatureLayer> _annoFeatLayers = new List<IFeatureLayer>();
        protected IApplication _application;
        protected IMxDocument _mxDoc = null;
        protected IEditor _pEditor = null;

        protected double _angle = 0, _xOffset = 0, _yOffset = 0;
        protected int _size = 0;
        protected int _alignment = 0;
        
        protected int _bold = 0;
        protected string _deleteAnnoMatch = null;
        protected bool _inlineGroupAnno = false;
        #endregion

        #region Public Constants

        /// <summary>
        /// If the settings indicate to delete Job Number anno, this indicates what the anno class name will be
        /// compared to when determining whether or not it should be deleted. This could be changed to be managed
        /// directly by the settings to allow for more versatility.
        /// </summary>
        public const string ANNOCLASS_NAMEMATCH_DELETE = "Job";

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public BaseCALMTool()
        {
            base.m_category = "PGE Tools"; //localizable text 
        }

        #endregion

        #region Overridden Class Methods

        /// <summary>
        /// Occurs when this tool is created
        /// </summary>
        /// <param name="hook">Instance of the application</param>
        public override void OnCreate(object hook)
        {
            _application = hook as IApplication;

            //Disable if it is not ArcMap
            if (hook is IMxApplication)
            {
                base.m_enabled = true;
                _mxDoc = _application.Document as IMxDocument;
            }
            else
                base.m_enabled = false;
        }

        public override void Refresh(int hDC)
        {
            base.Refresh(hDC);

            //Don't refresh them here. Just clear them. The next time the tool is run, we'll take that burden.
            _annoFeatLayers.Clear();
        }

        #endregion

        #region Helper Methods

        protected void GetEligibleLayers()
        {
            _annoFeatLayers.Clear();

            //Get all of the IFeatureLayers
            ESRI.ArcGIS.esriSystem.UID uid = null;
            try
            {
                uid = new ESRI.ArcGIS.esriSystem.UIDClass();
                uid.Value = "{40A9E885-5533-11D0-98BE-00805F7CED21}";
                IEnumLayer featLayers = _mxDoc.FocusMap.get_Layers(uid);
                IFeatureLayer featLayer = featLayers.Next() as IFeatureLayer;
                while (featLayer != null)
                {
                    if (featLayer.FeatureClass.FeatureType == esriFeatureType.esriFTAnnotation)
                    {
                        _annoFeatLayers.Add(featLayer);
                    }
                    featLayer = featLayers.Next() as IFeatureLayer;
                }
            }
            finally
            {
                if (uid != null)
                    Marshal.FinalReleaseComObject(uid);
            }
        }

        /// <summary>
        /// The annotation movement settings are determined using a form. This method reads the settings
        /// from the form, ensures that they are correct, and puts them in class-level variables.
        /// </summary>
        /// <returns>A boolean value indicating whether or not there was an error in the settings.</returns>
        protected bool ReadSettings()
        {
            //Get values from toolbar form.
            try
            {
                //Validate angle
                if (CALMSettingsUC.Rotation != null &&
                                  Double.TryParse(CALMSettingsUC.Rotation, out _angle))
                {
                    if (_angle >= -360 && _angle < 0)
                    {
                        _angle = 360 + _angle;
                    }
                    else if (Math.Abs(_angle) > 360)
                    {
                        MessageBox.Show("Please enter a rotation value between -360 and 360 degrees.", "CALM",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
                //Sentinel value used to communicate to other parts of this toolbar that we should not perform a
                //rotation. Bug 14307
                else _angle = AnnotationFacade.DoNotRotate;


                //Offset
                if (CALMSettingsUC.XOffset == null || !Double.TryParse(CALMSettingsUC.XOffset, out _xOffset))
                {
                    MessageBox.Show("Please enter a numeric X offset value.", "CALM", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                if (CALMSettingsUC.YOffset == null || !Double.TryParse(CALMSettingsUC.YOffset, out _yOffset))
                {
                    MessageBox.Show("Please enter a numeric Y offset value.", "CALM", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                _size = CALMSettingsUC.FontSize;

                //Whether to change the bold setting
                _bold = CALMSettingsUC.Bold;

                //Alignment
                _alignment = CALMSettingsUC.Alignment;

                //Delete Anno?
                _deleteAnnoMatch = CALMSettingsUC.Delete ? ANNOCLASS_NAMEMATCH_DELETE : null;

                //Inline Anno?
                _inlineGroupAnno = CALMSettingsUC.Inline;
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Starts processing the anno using the class-level settings.
        /// </summary>
        /// <param name="annosToTransform">The annotation features to transform.</param>
        protected void TransformAnno(List<IFeature> annosToTransform)
        {
            try
            {
                List<CALMAnnotationParentGroup> parentsToProcess = new List<CALMAnnotationParentGroup>();

                //Loop through and find all parent objects to process.
                foreach (IFeature anno in annosToTransform)
                {
                    //Get anno class information
                    int annoClassID = 0;
                    AnnotationFacade.FindAnnoClass(anno, out annoClassID);

                    IEnumRelationshipClass relClasses = anno.Class.get_RelationshipClasses(esriRelRole.esriRelRoleAny);
                    IRelationshipClass rel = relClasses.Next();
                    if (rel != null)
                    {
                        IObject parentObject = AnnotationFacade.FindAnnoParent(anno, rel);
                        if (parentObject == null)
                            throw new Exception("Could not find parent feature for this object.");

                        CALMAnnotationParentGroup parentGroup = new CALMAnnotationParentGroup(parentObject, rel, annoClassID);

                        //Add parent group to process.
                        if (!parentsToProcess.Contains(parentGroup))
                            parentsToProcess.Add(parentGroup);
                    }
                }

                //Loop through and find all parent objects to process.
                foreach (CALMAnnotationParentGroup parentGroup in parentsToProcess)
                {
                    //Delete any features if necessary, and determine which features were deleted.
                    AnnotationFacade.DeleteAnno(parentGroup.ParentObject, parentGroup.RelationshipClass, false, _deleteAnnoMatch);

                    //Refresh cached relationship class object so the deleted object is accounted for.
                    parentGroup.RefreshRelationshipClass();

                    //Apply settings
                    if (parentGroup.RelationshipClass != null)
                        AnnotationFacade.ApplySettingsToNewAnno(parentGroup.RelationshipClass, parentGroup.ParentObject,
                            _alignment, _angle, parentGroup.AnnotationClassID, _inlineGroupAnno, _xOffset, _yOffset, _size, _bold, MinFontSize);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error during execution: " + ex.Message, "CALM", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }

        /// <summary>
        /// Gets the Display Transformation details for use in translating device coordinates to map coordinates.
        /// </summary>
        /// <param name="mxApp">The current GIS application.</param>
        /// <returns>The display transformation object for this application's active screen.</returns>
        protected IDisplayTransformation GetDisplayTransformation(IMxApplication mxApp)
        {
            IAppDisplay appDisplay = mxApp.Display;
            if (appDisplay == null)
                return null;

            IDisplay display = (IDisplay)appDisplay.FocusScreen;

            if (display == null)
                return null;

            return display.DisplayTransformation;
        }

        #endregion

        #region Helper Classes

        protected class CALMAnnotationParentGroup : IEquatable<CALMAnnotationParentGroup>
        {
            public IObject ParentObject;
            public IRelationshipClass RelationshipClass;
            public int AnnotationClassID;

            private string _relParentID;

            public CALMAnnotationParentGroup(IObject parentObj, IRelationshipClass relClass, int annoClassID)
            {
                ParentObject = parentObj;
                RelationshipClass = relClass;
                AnnotationClassID = annoClassID;

                _relParentID = relClass.RelationshipClassID + "." + parentObj.OID;
            }

            public void RefreshRelationshipClass()
            {
                if (RelationshipClass == null) return;

                int relClassID = RelationshipClass.RelationshipClassID;
                RelationshipClass = null;

                IEnumRelationshipClass relClasses = ParentObject.Class.get_RelationshipClasses(esriRelRole.esriRelRoleAny);
                for (IRelationshipClass rel = relClasses.Next(); rel != null; rel = relClasses.Next())
                {
                    if (rel.RelationshipClassID == relClassID)
                        RelationshipClass = rel;
                }
            }

            public override string ToString()
            {
                return _relParentID;
            }

            public bool Equals(CALMAnnotationParentGroup other)
            {
                return this.ToString() == other.ToString();
            }
        }

        #endregion
    }
}
