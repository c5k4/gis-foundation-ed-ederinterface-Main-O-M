using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.GeoDatabaseDistributed;
using PGE.Common.ChangesManager.Utilities;

namespace PGE.Common.ChangesManager
{
    public class ChangeXmlConverter
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeXmlConverter"/> class.
        /// </summary>
        /// <param name="targetVersion">The target version to detect changes against.</param>
        /// <param name="sourceVersion">The source version (the version containing the changes) to detect changes against.</param>
        /// <param name="loadVersion">The version to which differences will be loaded.</param>
        /// <param name="xmlFileLocation">The file to which the export will be written.</param>
        public ChangeXmlConverter(IVersion targetVersion, IVersion sourceVersion, IVersion loadVersion, string xmlFileLocation)
        {
            this.targetVersion = targetVersion;
            this.sourceVersion = sourceVersion;
            this.loadVersion = loadVersion;
            this.xmlFileLocation = xmlFileLocation;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The file name of the file to be saved (and reloaded).
        /// </summary>
        public string xmlFileLocation
        {
            get;
            private set;
        }

        /// <summary>
        /// The source version used for the difference evaluation.
        /// </summary>
        public IVersion sourceVersion
        {
            get;
            private set;
        }

        /// <summary>
        /// The target version used for the difference evaluation.
        /// </summary>
        public IVersion targetVersion
        {
            get;
            private set;
        }

        /// <summary>
        /// The version to which the differences will be reloaded.
        /// </summary>
        public IVersion loadVersion
        {
            get;
            private set;
        }

        #endregion

        #region Public Methods

        public void ExportDifferences()
        {
            IVersionDataChangesInit vdci = null;
            IExportDataChanges exporter = null;
            try
            {
                // loop through each of the modified classes in this version to get differences.
                vdci = new ESRI.ArcGIS.GeoDatabaseDistributed.VersionDataChangesClass();
                IWorkspaceName wsNameSource = (IWorkspaceName)((IDataset)this.sourceVersion).FullName;
                IWorkspaceName wsNameTarget = (IWorkspaceName)((IDataset)this.targetVersion).FullName;
                vdci.Init(wsNameSource, wsNameTarget);
                VersionDataChanges vdc = (VersionDataChanges)vdci;
                IDataChanges dataChanges = (IDataChanges)vdci;
                IDataChangesInfo dci = (IDataChangesInfo)vdc;

                exporter = new DataChangesExporterClass();
                exporter.ExportDataChanges(this.xmlFileLocation, esriExportDataChangesOption.esriExportToXML, dataChanges as IDataChanges, true);
            }
            finally
            {
                if (exporter != null)
                    while (Marshal.ReleaseComObject(exporter) > 0) { }
                if (vdci != null)
                    while (Marshal.ReleaseComObject(vdci) > 0) { }
            }
        }

        public void ImportDifferences()
        {
            IWorkspace loadWorkspace = (IWorkspace)this.loadVersion;

            IDeltaDataChangesInit2 deltaLoader = null;
            IImportDataChanges deltaImporter = null;

            try
            {
                //Disable AUs.
                AUDisabler auControl = new AUDisabler();
                auControl.AUsEnabled = false;

                //Import rows.
                deltaLoader = new DeltaDataChangesClass() as IDeltaDataChangesInit2;
                deltaImporter = new DataChangesImporterClass() as IImportDataChanges;

                deltaLoader.Init2(this.xmlFileLocation, esriExportDataChangesOption.esriExportToXML, true);
                deltaImporter.ImportDataChanges((loadWorkspace as IDataset).FullName as IWorkspaceName, deltaLoader as IDeltaDataChanges, false, true);

                //Re-enable AUs.
                auControl.AUsEnabled = true;
            }
            finally
            {
                if (deltaLoader != null)
                    while (Marshal.ReleaseComObject(deltaLoader) > 0) { }
                if (deltaImporter != null)
                    while (Marshal.ReleaseComObject(deltaImporter) > 0) { }
            }
        }

        #endregion
    }
}
