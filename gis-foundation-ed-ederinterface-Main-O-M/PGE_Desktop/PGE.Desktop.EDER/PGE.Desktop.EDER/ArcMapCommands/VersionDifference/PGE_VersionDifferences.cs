using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using PGE.Common.Delivery.UI.Commands;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.esriSystem;
using Miner.Interop;
using Miner.ComCategories;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.ArcMapUI;
using PGE.Desktop.EDER.ArcMapCommands.VersionDifference;

namespace PGE.Desktop.EDER.ArcMapCommands
{
    [Guid("e4e57015-26ed-40e7-bbc8-8105cca89a76")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("PGE.Desktop.EDER.ArcMapCommands.PGE_VersionDifferences")]
    [ComponentCategory(ComCategory.ArcMapCommands)]
    public class PGE_VersionDifferences : BaseArcGISCommand
    {
        private IEditor editor = null;

        public PGE_VersionDifferences()
            : base("PGE - Version Differences", "PGE - Version Differences", "PGE Tools", "PGE - Version Differences", "Get version differences between current edit version and parent version")
        {
            try
            {
                if (editor == null)
                {
                    UID uID = new UID();
                    uID.Value = "esriEditor.Editor";
                    editor = PGE.Common.Delivery.ArcFM.ApplicationFacade.Application.FindExtensionByCLSID(uID) as IEditor;
                }
            }
            catch (Exception e)
            {

            }
        }

        public override bool Enabled
        {
            get
            {
                IContentsView view = GetSourceContentsView();
                if (view != null && view.SelectedItem is IWorkspace)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public override void OnClick()
        {
            IContentsView view = GetSourceContentsView();

            if (view != null && view.SelectedItem is IWorkspace)
            {
                IWorkspace childWorkspace = view.SelectedItem as IWorkspace;
                IVersionInfo editVersionInfo = ((IVersion)view.SelectedItem).VersionInfo;
                IVersionInfo parentVersionInfo = editVersionInfo.Parent;
                ID8List verDifferenceList = new D8ListClass();
                IMMVersioningUtils versionUtils = new MMVersioningUtilsClass();
                verDifferenceList = versionUtils.GetAllDifferences((IWorkspace)view.SelectedItem, editVersionInfo.VersionName, parentVersionInfo.VersionName, true, false, "");
                verDifferenceList.Reset();
                VersionChangesForm changesForm = new VersionChangesForm(verDifferenceList, childWorkspace, (IWorkspace)versionUtils.FindVersion(childWorkspace, parentVersionInfo.VersionName));
                changesForm.Show(new ModelessDialog(PGE.Common.Delivery.ArcFM.ApplicationFacade.Application.hWnd));
                changesForm.Refresh();
            }
        }

        /// <summary>
        /// This method will return the contents view from ArcMap for the Source tab.
        /// </summary>
        /// <returns></returns>
        private IContentsView GetSourceContentsView()
        {
            IMxDocument mxDoc = PGE.Common.Delivery.ArcFM.ApplicationFacade.Application.Document as IMxDocument;
            for (int i = 0; i < mxDoc.ContentsViewCount; i++)
            {
                IContentsView view = mxDoc.get_ContentsView(i);
                if (view.Name.ToUpper() == "SOURCE")
                {
                    return view;
                }
            }
            return null;
        }
    }
}
