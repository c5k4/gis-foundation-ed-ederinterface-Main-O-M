using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using ESRI.ArcGIS.Framework;
using System.Runtime.InteropServices;
using Miner.ComCategories;
using ESRI.ArcGIS.ADF.BaseClasses;
using Miner.Interop.Process;
using PGE.Common.Delivery.Process.BaseClasses;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.ADF.CATIDs;
using System.Drawing;
using PGE.Common.Delivery.Process;
using ADODB;
using ESRI.ArcGIS.Geodatabase;
using Miner.Interop;
using System.Reflection;
using PGE.Common.Delivery.Diagnostics;

namespace PGE.Desktop.EDER.ArcMapCommands
{
    [Guid("1A0520CC-73AF-4E7C-9891-C542A7664DAB")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("PGE.Desktop.EDER.ArcMapCommands.PGE_UpdateSesionDescription")]
    [ComponentCategory(ComCategory.ArcMapCommands)]

    public class PGEUpdateSessionDescription : BaseCommand
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
        private IMMPxApplication _pxApp;
        private PxDb _pxDb;
        private IMMPxNode _pxNode;
        public static IWorkspace _loginWorkspace;
        IMMSessionManager2 _mmSessionMangerExt;
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

         public PGEUpdateSessionDescription()
        {
            //
            // TODO: Define values for the public properties
            //
            base.m_category = "PGE Tools"; //localizable text
            base.m_caption = "PGE Update Session Description";  //localizable text
            base.m_message = "PGE Update Session Description";  //localizable text 
            base.m_toolTip = "Update session description within open edit session mode";  //localizable text 
            base.m_name = "PGE Update Session Description";   //unique id, non-localizable (e.g. "MyCategory_ArcMapCommand")

            try
            {
                //
                // TODO: change bitmap name if necessary
                //
                base.m_bitmap = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("PGE.Desktop.EDER.Bitmaps.SessionDescriptionIconNew.bmp"));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message, "Invalid Bitmap");
            }
        }

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
             {
                 base.m_enabled = false;
             }
         }
           
         //to enable this button 
         public override bool Enabled
         {
             get
             {
                //Should only be enabled if the user is currently in an edit session
                if (Miner.Geodatabase.Edit.Editor.EditState == Miner.Geodatabase.Edit.EditState.StateEditing) { return true; }
                else { return false; }
                
             }
         }

        public override void OnClick()
        {
            try
            {
               IMMPxIntegrationCache intcache = (IMMPxIntegrationCache)m_application.FindExtensionByName("Session Manager Integration Extension");
               if (intcache != null)
               {
                   _pxApp = intcache.Application;
                   if (_pxApp != null)
                   {
                       _mmSessionMangerExt = _pxApp.FindPxExtensionByName("MMSessionManager") as IMMSessionManager2;
                   }
               }

                DataTable dt = new DataTable();
                string oldDescription = string.Empty;
                string newDescription = string.Empty;

                _pxDb = new PxDb(_pxApp);
                string sessionid = _mmSessionMangerExt.CurrentOpenSession.get_ID().ToString();
                //sessionid = sessionid.Substring(8, 6);
                //intcache.Application.
                string sqlDes = "select  DESCRIPTION from process.mm_session where session_id ='" + sessionid + "'";
                dt = _pxDb.ExecuteQuery(sqlDes);
                foreach (DataRow r in dt.Rows)
                {
                    oldDescription = r["DESCRIPTION"].ToString(); ;
                }
                PGEUpdateSessionDesForm myForm = new PGEUpdateSessionDesForm(sessionid,oldDescription);
                DialogResult result = myForm.ShowDialog();
                if (result == DialogResult.OK)
                {
                    newDescription = myForm.newDescription;
                    string sqlQuery = "update process.mm_session set description ='" + newDescription + "' where session_id ='" + sessionid + "'";
                    _pxDb.ExecuteNonQuery(sqlQuery);
                    MessageBox.Show("Session description updated");
                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Occured. Please contact system administrator");
                _logger.Debug("Error Occured : " + ex.ToString());
            }

        }    
    }
}
