using System;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Framework;

namespace PGE.BatchApplication.DLMTools.Commands.AnnoStackingValidation
{
    [Guid("F0EEBCB8-5E86-4D3B-B4E1-2357209C10EF")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("PGE.BatchApplication.DLMTools.AnnoStackingValidation")]
    public class AsvTool : BaseCommand
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

        private IApplication _app;

        public AsvTool()
        {
            //
            // TODO: Define values for the public properties
            //
            m_category = "PGE Conversion Tools"; //localizable text
            m_caption = "Anno Stacking Validation";  //localizable text
            m_message = "Validates stacked anno";  //localizable text 
            m_toolTip = "Anno Stacking Validation";  //localizable text 
            m_name = "DLMTools_Anno_Stacking_Validation";   //unique id, non-localizable (e.g. "MyCategory_ArcMapCommand")

            try
            {
                m_bitmap =
                    new Bitmap(
                        Assembly.GetExecutingAssembly()
                            .GetManifestResourceStream("PGE.BatchApplication.DLMTools.Bitmaps.AnnoStackingValidation.png"));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message, "Invalid Bitmap");
            }
        }

        public override void OnClick()
        {
            //
            //  TODO: Sample code showing how to access button host
            //
            frmAnnoLeaderGraphics dialog = new frmAnnoLeaderGraphics(_app);
            AnnoLeaderGraphics this_tool = new AnnoLeaderGraphics(dialog, 0, _app);
            this_tool.Run();
        }

        public override void OnCreate(object hook)
        {
            _app = hook as IApplication;
        }
    }

}
