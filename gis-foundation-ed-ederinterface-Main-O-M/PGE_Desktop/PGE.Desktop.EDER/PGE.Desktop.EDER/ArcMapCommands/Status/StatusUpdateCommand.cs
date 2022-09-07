using System;
using System.Drawing;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;
using Miner.Interop;
using ESRI.ArcGIS.Geodatabase;
using System.Windows.Forms;
using Miner.ComCategories;
using ESRI.ArcGIS.esriSystem;
using System.Collections.Generic;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using Miner.Framework;
using ESRI.ArcGIS.Editor;

namespace PGE.Desktop.EDER.ArcMapCommands.Status
{
    /// <summary>
    /// Summary description for StatusUpdateCommand.
    /// </summary>
    [Guid("b2a765be-0aa7-4724-8143-d5140ca49b89")]
    //[Guid("0034473D-0AEB-409F-9A01-E7EDD8DF75CD")]
    //[ClassInterface(ClassInterfaceType.None)]
    [ComponentCategory(ComCategory.ArcMapCommands)]

    [ProgId("PGE.Desktop.EDER.ArcMapCommands.Status.StatusUpdateCommand")]
    public sealed class StatusUpdateCommand : BaseCommand
    {
        // private StatusUpdate StatusUpdateForm = null;
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

        public static IWorkspace _loginWorkspace;
        private IApplication m_application;

        public StatusUpdateCommand()
        {
            //
            // TODO: Define values for the public properties
            //
            base.m_category = "PGE Tools"; //localizable text
            base.m_caption = "Change Status Tool";  //localizable text
            base.m_message = "Change Status Tool";  //localizable text 
            base.m_toolTip = "Change Status Tool";  //localizable text 
            base.m_name = "PGESTATUSCHANGETOOL";   //unique id, non-localizable (e.g. "MyCategory_ArcMapCommand")

            try
            {
                //
                // TODO: change bitmap name if necessary
                //
                string bitmapResourceName = GetType().Name + ".bmp";
                base.m_bitmap = new Bitmap(GetType(), bitmapResourceName);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message, "Invalid Bitmap");
            }
        }

        #region Overridden Class Methods

        private static IWorkspace GetWorkspace()
        {
            // Get and return the logged in workspace
            IMMLoginUtils utils = new MMLoginUtils();
            _loginWorkspace = utils.LoginWorkspace;
            return _loginWorkspace;
        }

        public override bool Enabled
        {

            get
            {

                //Should only be enabled if the user is currently in an edit session

                if (Miner.Geodatabase.Edit.Editor.EditState == Miner.Geodatabase.Edit.EditState.StateEditing) { return true; }

                else { return false; }

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
                base.m_enabled = false;

            // TODO:  Add other initialization code
        }

        /// <summary>
        /// Occurs when this command is clicked
        /// </summary>
        public override void OnClick()
        {
            // TODO: Add StatusUpdateCommand.OnClick implementation
            System.Windows.Forms.Cursor.Current = Cursors.WaitCursor;
            UID pID = new UIDClass();
            pID.Value = "esriEditor.Editor";
            //  IWorkspace loginWorkspace = GetWorkspace();
            IEditor editor = m_application.FindExtensionByCLSID(pID) as IEditor;
            IWorkspaceEdit _wksp = editor.EditWorkspace as IWorkspaceEdit;
            //_wksp.StartEditing(true);
            //  IWorkspaceEdit _wksp = (IWorkspaceEdit)loginWorkspace;
            Type auType = Type.GetTypeFromProgID("mmGeodatabase.MMAutoUpdater");
            object auObj = Activator.CreateInstance(auType);
            IMMAutoUpdater immAutoupdater = auObj as IMMAutoUpdater;
            mmAutoUpdaterMode currentAUMode = immAutoupdater.AutoUpdaterMode;
            immAutoupdater.AutoUpdaterMode = mmAutoUpdaterMode.mmAUMNoEvents;
            try
            {
                IMxDocument MXDOC = m_application.Document as IMxDocument;
                IMap pMap = MXDOC.FocusMap;

                IEnumFeature enumFeatSelected = pMap.FeatureSelection as IEnumFeature;

                _wksp.StartEditOperation();

                AllSelectedFeatures(m_application);
                _wksp.StopEditOperation();
                //_wksp.StopEditing(true);
                //ME Q1-2020 : BUG FIX
                MXDOC.ActivatedView.Refresh();
            }
            catch (Exception ex)
            {
                //System.Windows.Forms.MessageBox.Show("Failed to open Change status Update tool: " + ex.Message);
                MessageBox.Show(ex.Message);
                _wksp.AbortEditOperation();
                System.Windows.Forms.Cursor.Current = Cursors.Default;
            }

            finally
            {
                Marshal.ReleaseComObject(_wksp);
                immAutoupdater.AutoUpdaterMode = currentAUMode;
                _wksp.StopEditOperation();
                //_wksp.StopEditing(true);
                System.Windows.Forms.Cursor.Current = Cursors.Default;
                //this.Dispose();
            }
        }

        public IDisplayTransformation GetDisplayTransformation(IMxApplication mxApp)
        {
            try
            {
                IAppDisplay appDisplay = mxApp.Display;
                if (appDisplay == null)
                    return null;

                IDisplay display = (IDisplay)appDisplay.FocusScreen;

                if (display == null)
                    return null;
                return display.DisplayTransformation;
            }
            catch (Exception ex)
            {

            }

            return null;
        }

        void ReleaseAllTheThings(params object[] comObjects)
        {
            try
            {
                foreach (var item in comObjects)
                {
                    if (item is IDisposable)
                        ((IDisposable)item).Dispose();
                    else if (item != null && Marshal.IsComObject(item))
                        while (Marshal.ReleaseComObject(item) > 0) ;
                }
            }
            catch (Exception ex)
            {

            }

        }

        public IMap map { get; set; }

        public void AllSelectedFeatures(IApplication m_application)
        {
            System.Windows.Forms.Cursor.Current = Cursors.WaitCursor;
            IFeature feat;
            List<IRow> RelatedObject = new List<IRow>();
            IObjectClass objectClass = null;
            IRow pRow = null;
            IDataset pDataset = null;
            IMxDocument AMXDOC = m_application.Document as IMxDocument;
            IMap pmap = AMXDOC.FocusMap;
            int iRelatedObjectCount = 0;
            IEnumFeature enumFeat = pmap.FeatureSelection as IEnumFeature;
            IEnumFeature enumFeatCopy = enumFeat;
            enumFeat.Reset();
            enumFeatCopy.Reset();

            int recUpdateCount = 0;
            int recRelUpdateCount = 0;

            //bool isAllSelFeat = true;

            //Get the ArcFm LogIn User Name
            Miner.Interop.Process.IMMPxIntegrationCache intcache = (Miner.Interop.Process.IMMPxIntegrationCache)m_application.FindExtensionByName("Session Manager Integration Extension");
            Miner.Interop.Process.IMMPxApplication _pxApp = intcache.Application;
            string lastUser = _pxApp.User.Name.ToString();


            try
            {
                bool recUpdateStatus = false;
                if (enumFeatCopy != null && enumFeatCopy.Next() != null)
                {
                    enumFeat.Reset();
                    enumFeatCopy.Reset();


                    while ((feat = enumFeat.Next()) != null)
                    {
                        if (feat.FeatureType != esriFeatureType.esriFTAnnotation)
                        {
                            objectClass = feat.Class;
                            int index1 = objectClass.Fields.FindField("STATUS");

                            int indexLastUser = objectClass.Fields.FindField("LASTUSER");
                            int indexDateModified = objectClass.Fields.FindField("DATEMODIFIED");

                            try
                            {
                                if (index1 != -1)
                                {
                                    // var value = Convert.ToInt32(feat.get_Value(index1));
                                    if (feat.get_Value(feat.Fields.FindField("STATUS")).ToString() == "0")
                                    {

                                        feat.set_Value(index1, 5);
                                        try
                                        {
                                            feat.set_Value(indexLastUser, lastUser);
                                            feat.set_Value(indexDateModified, DateTime.Now.Date);
                                        }
                                        catch (Exception ex) { }

                                        feat.Store();
                                        recUpdateCount++;
                                        recUpdateStatus = true;
                                    }


                                }





                                RelatedObject = findMultipleRelatedFeaturewithClassName(feat);
                                iRelatedObjectCount = RelatedObject.Count;


                                for (int i = 0; i < iRelatedObjectCount; i++)
                                {
                                    IObject relatedObject = RelatedObject[i] as IObject;
                                    // pRow = null;
                                    // pRow = RelatedObject[i];
                                    // if (relatedObject.FeatureType != esriFeatureType.esriFTAnnotation)
                                    //  { 

                                    int rindex1 = relatedObject.Fields.FindField("STATUS");

                                    int indexLastUserRel = relatedObject.Fields.FindField("LASTUSER");
                                    int indexDateModifiedRel = relatedObject.Fields.FindField("DATEMODIFIED");


                                    if (rindex1 != -1)
                                    {
                                        if (relatedObject.get_Value(relatedObject.Fields.FindField("STATUS")).ToString() == "0")
                                        {


                                            relatedObject.set_Value(rindex1, 5);
                                            try
                                            {
                                                relatedObject.set_Value(indexLastUserRel, lastUser);
                                                relatedObject.set_Value(indexDateModifiedRel, DateTime.Now.Date);
                                            }
                                            catch(Exception ex) {}

                                            relatedObject.Store();
                                            recRelUpdateCount++;
                                            recUpdateStatus = true;
                                        }


                                    }



                                }


                                // feat.Store();

                            }
                            catch (Exception ex)
                            {
                                // MessageBox.Show("Failed to Update: "+ feat.OID.ToString());
                                //return false;
                            }


                        }
                    }
                    if (recUpdateStatus)
                    {
                        // MessageBox
                        DisplayStatusBarMessage(recUpdateCount + " feature(s) and " + recRelUpdateCount + " related" + " record(s) updated successfully.");



                    }
                    else
                    {
                        DisplayStatusBarMessage("Records not updated successfully.Please select Feature(s) having status Proposed");
                    }

                }
                else
                {
                    DisplayStatusBarMessage("Please select layers to update.");
                    enumFeat.Reset();
                    enumFeatCopy.Reset();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Unhandled exception encountered while finding related features," + ex.Message.ToString() + " at " + ex.StackTrace);
                DisplayStatusBarMessage(ex.Message);
                System.Windows.Forms.Cursor.Current = Cursors.Default;
            }

            finally
            {
                Marshal.ReleaseComObject(enumFeat);
                System.Windows.Forms.Cursor.Current = Cursors.Default;

            }
            // return feat;
        }


        public List<IRow> findMultipleRelatedFeaturewithClassName(IFeature pFeature)
        {
            List<IRow> RelatedObject = new List<IRow>();
            IObjectClass objectClass = null;
            IObjectClass objectTable = null;
            IEnumRelationshipClass relClasses = null;
            IRelationshipClass relClass = null;
            ITable pTable_Rel = null;
            IFeature pfeat;
            IDataset pDataset = null;
            string pTableName_Rel = null;

            try
            {
                objectClass = pFeature.Class;
                relClasses = objectClass.get_RelationshipClasses(esriRelRole.esriRelRoleAny);
                relClasses.Reset();
                relClass = relClasses.Next();

                while (relClass != null)
                {
                    pTable_Rel = null;
                    pTableName_Rel = null;

                    if (relClass.DestinationClass is ITable)
                    {
                        objectTable = relClass.DestinationClass;


                        var destClass = relClass.DestinationClass as IFeatureClass;

                        if (destClass == null || destClass.FeatureType != esriFeatureType.esriFTAnnotation)
                        {
                            // pDataset = (IDataset)pTable_Rel;
                            //pTableName_Rel = pDataset.Name;
                            // if  (relClasses.Feature)
                            //    if (pTable_Rel.FeatureType != esriFeatureType.esriFTAnnotation)
                            //  {
                            //  if ((ModelNameFacade.ContainsAllClassModelNames(objectTable, SchemaInfo.Electric.FieldModelNames.Status)))
                            //{
                            // IRelationshipClass relClass2 = ModelNameFacade.RelationshipClassFromModelName(objectTable, esriRelRole.esriRelRoleOrigin, SchemaInfo.General.ClassModelNames.INSTALL_DATE);
                            ISet relatedFeatures = relClass.GetObjectsRelatedToObject(pFeature);

                            relatedFeatures.Reset();
                            object pRelatedObj = relatedFeatures.Next();

                            while (pRelatedObj != null)
                            {

                                IRow pRelatedObjectRow = (IRow)pRelatedObj;

                                //int FieldIndex = pRelatedObjectRow.Fields.FindField("INSTALLATIONDATE");
                                // pRelatedObjectRow.set_Value(FieldIndex, prdate);
                                //pRelatedObjectRow.Store();
                                RelatedObject.Add(pRelatedObjectRow);


                                pRelatedObj = relatedFeatures.Next();

                            }
                        }

                    }

                    relClass = relClasses.Next();
                    //return RelatedObject;
                }


            }
            catch (Exception ex)
            {
                RelatedObject = null;
                Console.WriteLine("Unhandled exception encountered while finding related features," + ex.Message.ToString() + " at " + ex.StackTrace);

            }
            return RelatedObject;
        }
        /// <param name="statusBarMessage"></param>
        private static void DisplayStatusBarMessage(string statusBarMessage)
        {
            IMMArcGISRuntimeEnvironment environment = new ArcGISRuntimeEnvironment();
            if (environment != null)
            {
                environment.SetStatusBarMessage(statusBarMessage);
            }
        }
        #endregion
    }
}
