using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.SystemUI;

using Miner.ComCategories;
using Miner.Interop;

using PGE.Common.Delivery.UI.Commands;
using PGE.Desktop.EDER.ArcMapCommands;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Editor;
using Miner.Geodatabase;
using PGE.Desktop.EDER.UFM;
using PGE.Common.Delivery.Framework;
using System.Windows.Forms;
using System.IO;

namespace PGE.Desktop.EDER.ArcMapCommands.UFMFilter
{
    /// <summary>
    /// ToolControl implementation
    /// </summary>
    [Guid("895D6C7F-DD15-4ad7-A13D-49AB2D0C0FD4")]
    [ComponentCategory(ComCategory.ArcMapCommands)]
    [ProgId("PGE.Desktop.EDER.UpdateDuctAvail")]
    [ComVisible(true)]
    public class UpdateDuctAvail : BaseArcGISCommand
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

        public UpdateDuctAvail()
            : base(
                "PGE_UpdateDuctAvail", "PGE Update Duct Availability", "PGE Conversion Tools",
                "Updates Duct Availability", "Updates Duct Availability") 
        {
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

            _application = hook as IApplication;

            //Disable if it is not ArcMap
            if (hook is IMxApplication)
                m_enabled = true;
            else
                m_enabled = false;
        }

        /// <summary>
        /// Occurs when this command is clicked
        /// </summary>
        public override void OnClick()
        {
            bool inEditOperation = false;

            // Get edit workspace
            UID pID = new UIDClass();
            pID.Value = "esriEditor.Editor";
            IEditor editor = _application.FindExtensionByCLSID(pID) as IEditor;
            IWorkspaceEdit wkspcEdit = editor.EditWorkspace as IWorkspaceEdit;

            Type auType = Type.GetTypeFromProgID("mmGeodatabase.MMAutoUpdater");
            object auObj = Activator.CreateInstance(auType);
            IMMAutoUpdater immAutoupdater = auObj as IMMAutoUpdater;
            mmAutoUpdaterMode currentAUMode = immAutoupdater.AutoUpdaterMode;
            immAutoupdater.AutoUpdaterMode = mmAutoUpdaterMode.mmAUMNoEvents;

            try
            {
                if (wkspcEdit.IsBeingEdited())
                {
                    // Start edit op
                    wkspcEdit.StartEditOperation();
                    inEditOperation = true;

                    // Get Duct Banks with blobs
                    IFeatureCursor configuredDuctBanks = GetConfiguredDuctBanks(wkspcEdit as IWorkspace);

                    File.AppendAllText("c:\\temp\\ConduitSystemAvailUpdates.txt", "**Updating conduit system availability fields**" + DateTime.Now + "**" + Environment.NewLine);

                    // Check each one for availability
                    int iCount = 0;
                    int iInterimCount = 0;
                    int iUpdateCount = 0;
                    IFeature ductBank = configuredDuctBanks.NextFeature();
                    while (ductBank != null)
                    {
                        try
                        {
                            bool updated = ProcessDuctBank(ductBank);
                            if (updated == true)
                            {
                                // Apply AU's - Updating the Cross Section will require other AU's to fire
                                immAutoupdater.AutoUpdaterMode = currentAUMode;
                                IMMSpecialAUStrategy csAnnoAU = Activator.CreateInstance(Type.GetTypeFromProgID("mmUlsAUs.MMUpdateCrossSection")) as IMMSpecialAUStrategy;
                                csAnnoAU.Execute(ductBank as IObject);
                                immAutoupdater.AutoUpdaterMode = mmAutoUpdaterMode.mmAUMNoEvents;

                                // Update the duct banks does not
                                IMMSpecialAUStrategyEx sdAnnoAU = Activator.CreateInstance(Type.GetTypeFromProgID("mmFramework.DuctBankConduitCrossAU")) as IMMSpecialAUStrategyEx;
                                sdAnnoAU.Execute(ductBank as IObject, mmAutoUpdaterMode.mmAUMNoEvents, mmEditEvent.mmEventFeatureUpdate);

                                // Keep track of counts
                                iUpdateCount++;
                                iInterimCount++;
                                File.AppendAllText("c:\\temp\\ConduitSystemAvailUpdates.txt", iUpdateCount.ToString() + "-" + ductBank.OID.ToString() + " (" + iCount.ToString() + ")" + Environment.NewLine);
                            }
                        }
                        catch (Exception ex)
                        {
                            string msg = ex.Message;
                            File.AppendAllText("c:\\temp\\ConduitSystemAvailUpdates.txt", "WARNING: ConduitSystem record not fully updated correctly. Moving onto next one. (" + msg + ")" + Environment.NewLine);
                        }
                        ductBank = configuredDuctBanks.NextFeature();
                        
                        // Update the user occasioanlly
                        iCount++;
                        if (iCount % 20 == 0)
                        {
                            string test = iCount.ToString();
                            _application.StatusBar.set_Message(0, "Processed " + iCount.ToString() + " records (" + iUpdateCount.ToString() + " records updated)");
                            Application.DoEvents();
                        }
                        if (iInterimCount == 50)
                        {
                            // Save
                            File.AppendAllText("c:\\temp\\ConduitSystemAvailUpdates.txt", "Saving...");
                            wkspcEdit.StopEditOperation();
                            inEditOperation = false;
                            wkspcEdit.StopEditing(true);
                            wkspcEdit.StartEditing(true);
                            wkspcEdit.StartEditOperation();
                            inEditOperation = true;
                            File.AppendAllText("c:\\temp\\ConduitSystemAvailUpdates.txt", "Saved" + Environment.NewLine);
                            iInterimCount = 0;
                        }
                    }

                    File.AppendAllText("c:\\temp\\ConduitSystemAvailUpdates.txt", "**Updates complete, list is by OID**" + DateTime.Now + "**" + Environment.NewLine);

                    // Stop edit
                    wkspcEdit.StopEditOperation();
                }
            }
            catch (Exception ex)
            {
                // Cancel the edit
                string wha = ex.ToString();
                File.AppendAllText("c:\\temp\\ConduitSystemAvailUpdates.txt", wha + Environment.NewLine);
                            
                if (inEditOperation == true)
                {
                    wkspcEdit.AbortEditOperation();                    
                }
            }
            finally
            {
                inEditOperation = false;
                immAutoupdater.AutoUpdaterMode = currentAUMode;
            }
        }

        #endregion

        #region Private methods

        private IFeatureCursor GetConfiguredDuctBanks(IWorkspace ws)
        {
            IFeatureCursor ductBanks = null;
            IQueryFilter qf = new QueryFilterClass();
            qf.WhereClause = "SUBTYPECD=1 AND CONFIGURATION IS NOT NULL"; //  AND OBJECTID=23714432
            IFeatureClass conduitFc = ModelNameFacade.FeatureClassByModelName(ws, SchemaInfo.UFM.ClassModelNames.Conduit);
            ductBanks = conduitFc.Search(qf, false);
            return ductBanks;
        }

        private bool ProcessDuctBank(IFeature conduit)
        {
            bool updated = false;

            try
            {
                // Open up its blob
                IMMDuctBankConfig dbc = UfmHelper.GetDuctBankConfig(conduit);

                // Loop over each duct def
                ID8List dbcList = (ID8List)dbc;
                dbcList.Reset();
                ID8List ductDefObjs = (ID8List)dbcList.Next(false);
                ID8List ductDefinitionObj;

                for (ductDefObjs.Reset(), ductDefinitionObj = (ID8List)ductDefObjs.Next(); ductDefinitionObj != null;
                    ductDefinitionObj = (ID8List)ductDefObjs.Next())
                {
                    if (ductDefinitionObj is IMMDuctDefinition)
                    {
                        // If the duct def is marked unavailable, make it available
                        IMMDuctDefinition ductDef = (IMMDuctDefinition)ductDefinitionObj;
                        if (ductDef.availability != true)
                        {
                            ductDef.availability = true;
                            updated = true;
                        }
                    }
                }

                // Save if updated - each one of these will fire AU's (to update duct DC and x-section) so will take some time
                if (updated == true)
                {
                    int dbcFieldIndex = ModelNameFacade.FieldIndexFromModelName(conduit.Class, SchemaInfo.UFM.FieldModelNames.DuctBankConfig);
                    IMMPersistentListItem persist = (IMMPersistentListItem)dbc;
                    persist.SaveToField(conduit as IRow, dbcFieldIndex);
                    conduit.Store();
                }
            }
            catch (Exception ex)
            {
                string wha = ex.ToString();
            }

            return updated;
        }

        #endregion
    }
}
