using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using PGE.Desktop.EDER.AutoUpdaters.Special;
using Miner.Interop;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;

namespace PGE.Desktop.EDER.ArcMapCommands
{
    public partial class PGE_UpdateMapNumberForm : Form
    {
        public PGE_UpdateMapNumberForm()
        {
            InitializeComponent();

            lblProgress.Text = "";
        }

        public IMap map { get; set; }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            ExecuteUpdates();
        }

        private void ExecuteUpdates()
        {
            int totalProcessed = 0;

            try
            {
                PopulateDistMapNoAU distMapNoAU = new PopulateDistMapNoAU();
                PopulateLocalOfficeAU localOfficeAU = new PopulateLocalOfficeAU();
                PopulateLocalOfficeAU.LookupLocalOfficeByModelName = false;

                Type auType = Type.GetTypeFromProgID("mmGeodatabase.MMAutoUpdater");
                object auObj = Activator.CreateInstance(auType);
                IMMAutoUpdater immAutoupdater = auObj as IMMAutoUpdater;
                mmAutoUpdaterMode currentAUMode = immAutoupdater.AutoUpdaterMode;

                UID uid = new UIDClass();
                uid.Value = "{E156D7E5-22AF-11D3-9F99-00C04F6BC78E}";
                IEnumLayer featureLayers = map.get_Layers(uid, true);

                IFeatureLayer featLayer = null;
                int totalCountToProcess = 0;

                //Determine our total counts first
                while ((featLayer = featureLayers.Next() as IFeatureLayer) != null)
                {
                    if (featLayer.FeatureClass == null) { continue; }

                    if (PGE.Common.Delivery.Framework.ModelNameFacade.ContainsFieldModelName(featLayer.FeatureClass, SchemaInfo.Electric.FieldModelNames.LocalOfficeID)
                        && PGE.Common.Delivery.Framework.ModelNameFacade.ContainsClassModelName(featLayer.FeatureClass, SchemaInfo.Electric.ClassModelNames.DistMapUpdate))
                    {
                        //Both of the AUs are enabled for this feature class so let's go ahead and execute an update on all of the selected features
                        IFeatureSelection featSelection = featLayer as IFeatureSelection;
                        ISelectionSet selectedFeatures = featSelection.SelectionSet;
                        IEnumIDs selectedObjectIDs = selectedFeatures.IDs;
                        selectedObjectIDs.Reset();
                        int OID = -1;
                        while ((OID = selectedObjectIDs.Next()) > 0)
                        {
                            totalCountToProcess++;
                        }
                    }
                }

                Miner.Geodatabase.Edit.Editor.StartOperation();
                featureLayers.Reset();
                UpdateProgress(totalCountToProcess, totalProcessed);

                //Now process through all of the selected features and execute our autoupdaters on them
                while ((featLayer = featureLayers.Next() as IFeatureLayer) != null)
                {
                    if (CancelUpdates) { throw new Exception("Updates cancelled"); }

                    if (featLayer.FeatureClass == null) { continue; }

                    if (PGE.Common.Delivery.Framework.ModelNameFacade.ContainsFieldModelName(featLayer.FeatureClass, SchemaInfo.Electric.FieldModelNames.LocalOfficeID)
                        && PGE.Common.Delivery.Framework.ModelNameFacade.ContainsClassModelName(featLayer.FeatureClass, SchemaInfo.Electric.ClassModelNames.DistMapUpdate))
                    {
                        //Both of the AUs are enabled for this feature class so let's go ahead and execute an update on all of the selected features
                        IFeatureSelection featSelection = featLayer as IFeatureSelection;
                        ISelectionSet selectedFeatures = featSelection.SelectionSet;
                        IEnumIDs selectedObjectIDs = selectedFeatures.IDs;
                        selectedObjectIDs.Reset();
                        int OID = -1;
                        while ((OID = selectedObjectIDs.Next()) > 0)
                        {
                            if (CancelUpdates) { throw new Exception("Updates cancelled"); }

                            try
                            {
                                IFeature feature = featLayer.FeatureClass.GetFeature(OID);

                                PopulateLocalOfficeAU.LookupLocalOfficeByModelName = false;
                                localOfficeAU.Execute(feature, currentAUMode, mmEditEvent.mmEventFeatureUpdate);
                                distMapNoAU.Execute(feature, currentAUMode, mmEditEvent.mmEventFeatureUpdate);
                                PopulateLocalOfficeAU.LookupLocalOfficeByModelName = true;
                                feature.Store();
                                totalProcessed++;

                                //if ((totalProcessed % 10) == 0)
                                //{
                                    UpdateProgress(totalCountToProcess, totalProcessed);
                                    Application.DoEvents();
                                //}
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }
                    Application.DoEvents();
                }
            }
            catch (Exception ex)
            {
                if (Miner.Geodatabase.Edit.Editor.IsOperationInProgress())
                {
                    Miner.Geodatabase.Edit.Editor.AbortOperation();
                }

                if (CancelUpdates)
                {
                    MessageBox.Show("User cancelled the operation");
                }
                else
                {
                    throw ex;
                }
            }
            finally
            {
                PopulateLocalOfficeAU.LookupLocalOfficeByModelName = true;
            }

            if (!CancelUpdates)
            {
                //Save our updates
                if (Miner.Geodatabase.Edit.Editor.IsOperationInProgress()) { Miner.Geodatabase.Edit.Editor.StopOperation("PGE Update Map Numbers"); }
                MessageBox.Show(string.Format("{0} features were processed", totalProcessed));
            }
            this.Close();
        }


        private void UpdateProgress(int totalToProcess, int totalProcessed)
        {
            double progress = Math.Round((((double)totalProcessed) / ((double)totalToProcess)) * 100.0, 2);
            int currentProgress = Int32.Parse(Math.Floor(progress).ToString());
            lblProgress.Text = string.Format("{2}% complete: {0} out of {1} processed", totalProcessed, totalToProcess, currentProgress);
            prgProgressBar.Value = currentProgress;
        }

        private bool CancelUpdates = false;
        private void btnCancelUpdates_Click(object sender, EventArgs e)
        {
            CancelUpdates = true;
        }
    }
}
