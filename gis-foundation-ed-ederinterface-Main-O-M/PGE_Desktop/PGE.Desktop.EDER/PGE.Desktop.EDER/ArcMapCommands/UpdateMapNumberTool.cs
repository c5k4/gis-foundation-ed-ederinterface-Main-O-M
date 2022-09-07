using System;
using System.Drawing;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Controls;
using System.Windows.Forms;
using Telvent.PGE.ED.Desktop.AutoUpdaters.Special;
using Miner.Interop;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.esriSystem;
using System.Reflection;
using Telvent.Delivery.Framework;

namespace Telvent.PGE.ED.Desktop
{
    /// <summary>
    /// Command that works in ArcMap/Map/PageLayout
    /// </summary>
    [Guid("34314930-1a94-4119-bf4f-e24a1d376931")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("Telvent.PGE.ED.Desktop.UpdateMapNumberTool")]
    public sealed class UpdateMapNumberTool : BaseCommand
    {
        private IApplication m_application;
        private IEditor3 m_editor;
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
            ControlsCommands.Register(regKey);
        }
        /// <summary>
        /// Required method for ArcGIS Component Category unregistration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryUnregistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            MxCommands.Unregister(regKey);
            ControlsCommands.Unregister(regKey);
        }

        #endregion
        #endregion

        private IHookHelper m_hookHelper = null;
        public UpdateMapNumberTool()
        {
            //
            // TODO: Define values for the public properties
            //
            base.m_category = "PGE Tools"; //localizable text
            base.m_caption = "Update Map Number";  //localizable text 
            base.m_message = "";  //localizable text
            base.m_toolTip = "Update Map Number";  //localizable text
            base.m_name = "Update Map Number";   //unique id, non-localizable (e.g. "MyCategory_MyCommand")

            try
            {

                // TODO: change bitmap name if necessary
                base.m_bitmap = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Telvent.PGE.ED.Desktop.Bitmaps.UpdateMapNumberTool.bmp"));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message, "Invalid Bitmap");
            }
        }

        #region Overridden Class Methods

        /// <summary>
        /// Occurs when this command is created
        /// </summary>
        /// <param name="hook">Instance of the application</param>
        public override void OnCreate(object hook)
        {
            if (hook == null)
                return;

            try
            {
                m_application = hook as IApplication;
                UID editorUid = new UID();
                editorUid.Value = "esriEditor.Editor";
                m_editor = m_application.FindExtensionByCLSID(editorUid) as IEditor3;
                m_hookHelper = new HookHelperClass();
                m_hookHelper.Hook = hook;
                if (m_hookHelper.ActiveView == null)
                    m_hookHelper = null;
            }
            catch
            {
                m_hookHelper = null;
            }

            if (m_hookHelper == null)
                base.m_enabled = false;
            else
                base.m_enabled = true;

            // TODO:  Add other initialization code
        }

        /// <summary>
        /// Occurs when this command is clicked
        /// </summary>       
        /// 
        //        Private Property Get IMMSpecialAUStrategy_Name()String
        //  IMMSpecialAUStrategy_Name = "Simple Special AU Example"
        //End Property
        IMMSpecialAUStrategyEx objLocalOfficeAU, objDistMapNoAU;
        public override void OnClick()
        {
            // TODO: Add UpdateMapNumberTool.OnClick implementation
            try
            {
                Type localOfficeAUType = Type.GetTypeFromProgID("Telvent.PGE.ED.PopulateLocalOfficeAU");
                Type distMapNoAUType = Type.GetTypeFromProgID("Telvent.PGE.ED.PopulateDistMapNoAU");

                if (localOfficeAUType != null)
                    objLocalOfficeAU = Activator.CreateInstance(localOfficeAUType) as IMMSpecialAUStrategyEx;
                if (distMapNoAUType != null)
                    objDistMapNoAU = Activator.CreateInstance(distMapNoAUType) as IMMSpecialAUStrategyEx;

                IMxDocument pmxDocument = (IMxDocument)m_application.Document;
                IMap map = pmxDocument.FocusMap;
                for (int i = 0; i < map.LayerCount; i++)
                {
                    if (map.get_Layer(i) is IFeatureLayer)
                    {
                        IFeatureLayer pLayer = (IFeatureLayer)pmxDocument.FocusMap.get_Layer(i);
                        IFeatureClass pFeatureClass = pLayer.FeatureClass;
                        ISelectionSet selectedFeatures = ((IFeatureSelection)pLayer).SelectionSet;
                        int mapNoFldIx = ModelNameFacade.FieldIndexFromModelName(pFeatureClass, SchemaInfo.Electric.FieldModelNames.LocalOfficeID);
                        bool distmapAUflag = ModelNameFacade.ContainsClassModelName(pFeatureClass, SchemaInfo.Electric.ClassModelNames.DistMapUpdate);
                        if (selectedFeatures.Count > 0)
                        {
                            IEnumFeature enumFeature = map.FeatureSelection as IEnumFeature;
                            for (IFeature feature = enumFeature.Next(); feature != null; feature = enumFeature.Next())
                            {
                                IObject pobject = (IObject)feature;
                                if (mapNoFldIx!=-1 && objLocalOfficeAU != null)
                                {
                                    objLocalOfficeAU.Execute(pobject, mmAutoUpdaterMode.mmAUMArcMap, mmEditEvent.mmEventFeatureCreate);
                                    
                                }
                                if (distmapAUflag && distMapNoAUType != null)
                                {
                                    objDistMapNoAU.Execute(pobject, mmAutoUpdaterMode.mmAUMArcMap, mmEditEvent.mmEventFeatureCreate);
                                }
                            }
                        }
                    }

                }

            }// try block close
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }

        }
        public override bool Enabled
        {
            //Enable the command when map are in edit mode.
            get
            {
                return m_editor.EditState == esriEditState.esriStateEditing;
            }
        }

        #endregion
    }
}
