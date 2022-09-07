using System;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Framework;

namespace PGE.BatchApplication.DLMTools.Commands.MirrorDucts
{
    /// <summary>
    /// Summary description for cmdExecuteScript.
    /// </summary>
    [Guid("bc33dec1-6567-450f-8655-7691fae9c77a")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("PGE.BatchApplication.DLMTools.MirrorDucts.cmdExecuteScript")]
    public sealed class cmdExecuteScript : BaseCommand
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

        private IApplication m_application;
        public cmdExecuteScript()
        {
            //
            // TODO: Define values for the public properties
            //
            m_category = "PGE Conversion Tools"; //localizable text
            m_caption = "Mirror Ducts";  //localizable text
            m_message = "Flips the two Duct Bank configs connected to the selected Conduit";  //localizable text 
            m_toolTip = "Mirror Ducts";  //localizable text 
            m_name = "DLMTools_Mirror_Ducts";   //unique id, non-localizable (e.g. "MyCategory_ArcMapCommand")

            try
            {
                m_bitmap =
                    new Bitmap(
                        Assembly.GetExecutingAssembly()
                            .GetManifestResourceStream("PGE.BatchApplication.DLMTools.Bitmaps.cmdExecuteScript.bmp"));
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

            m_application = hook as IApplication;

            //Disable if it is not ArcMap
            if (hook is IMxApplication)
                base.m_enabled = true;
            else
                base.m_enabled = false;

            // TODO:  Add other initialization code
        }

        /// <summary>
        /// Occurs when this command is clicked
        /// </summary>
        public override void OnClick()
        {
            try
            {
                // TODO: Add cmdExecuteScript.OnClick implementation
                clsMain pMain = new clsMain();
                pMain.MirrorDucts(m_application);
                MessageBox.Show("Duct mirroring completed");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in Main: " + ex.Message); 
            }
        }

        #endregion
    }
}
