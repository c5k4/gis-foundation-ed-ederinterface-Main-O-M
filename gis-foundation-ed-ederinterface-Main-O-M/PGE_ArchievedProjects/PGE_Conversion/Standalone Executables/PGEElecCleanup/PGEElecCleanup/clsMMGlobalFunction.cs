using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Miner.Interop;
using ESRI.ArcGIS.Geodatabase;
using System.Collections;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;

namespace PGEElecCleanup
{
    class clsMMGlobalFunction
    {
        public static clsMMGlobalFunction _globalFunctions = new clsMMGlobalFunction();
        private IMMAutoUpdater autoupdater = null;

        public void StopAutoCreationofAnnotations(IWorkspace pWorkspace)
        {
             #region "Stop Auto Creation of Annotations in Destination database"
                    IEnumDataset DestinationEnumdataset = pWorkspace.get_Datasets(esriDatasetType.esriDTAny);
                    //Loop through all the Destination datasets to append the data 
                    IDataset Destinationdataset = DestinationEnumdataset.Next();
                    ArrayList objarraylist = null;
                    while (Destinationdataset != null)
                    {
                        Application.DoEvents();
                        AutoAnotationcreate(Destinationdataset, false, ref objarraylist);
                        Destinationdataset = DestinationEnumdataset.Next();
                    }
            # endregion
        }
        //public void StartAutoCreationofAnnotations(IEnumDataset DestinationEnumdataset,IDataset Destinationdataset)
        //{
        //    #region "Start Auto Creation of Annotations in Destination database"

        //    // Sbmessage.Panels["stsBarPanelToolStatus"].Text = "Start Auto Creation of Annotations in Destination database......";
        //    DestinationEnumdataset.Reset();
        //    Destinationdataset = DestinationEnumdataset.Next();
        //    while (Destinationdataset != null)
        //    {
        //        Application.DoEvents();
        //        AutoAnotationcreate(Destinationdataset, true, ref objarraylist);
        //        Destinationdataset = DestinationEnumdataset.Next();
        //    }
        //    #endregion
        //    #region start AU
        //    if (autoupdater != null)
        //    {
        //        autoupdater.AutoUpdaterMode = objautoupdateroldmode;
        //    }
        //    #endregion
        //}
        
        /// <summary>
        /// Disable Autoupdaters
        /// </summary>
        /// <returns></returns>
        public mmAutoUpdaterMode DisableAutoupdaters()
        {
            object objAutoUpdater = null;

            //Create an MMAutoupdater 
            objAutoUpdater = Activator.CreateInstance(Type.GetTypeFromProgID("mmGeodatabase.MMAutoUpdater"));
            autoupdater = objAutoUpdater as IMMAutoUpdater;
            //Save the existing mode
            mmAutoUpdaterMode oldMode = autoupdater.AutoUpdaterMode;
            //Turn off autoupdater events
            autoupdater.AutoUpdaterMode = mmAutoUpdaterMode.mmAUMNoEvents;
            // insert code that needs to execute while autoupdaters 
            return oldMode;
        }
        /// <summary>
        /// This method is to find the feature class and set the auto anotation create false 
        /// </summary>
        /// <param name="pdestinationdataset"></param>
        private void AutoAnotationcreate(IDataset pdestinationdataset, Boolean status, ref ArrayList strAnnoclass)
        {
            IFeatureClass objfeatureclass = null;
            IAnnoClassAdmin2 objannotation = null;
            IEnumDataset Enumdataset = null;
            IDataset enudataset = null;

            try
            {
                switch (pdestinationdataset.Type)
                {
                    case esriDatasetType.esriDTFeatureDataset:
                        {

                            Enumdataset = pdestinationdataset.Subsets;
                            enudataset = Enumdataset.Next();
                            while (enudataset != null)
                            {
                                if (enudataset.Type == esriDatasetType.esriDTFeatureClass)
                                {
                                    // objfeatureclass = (IFeatureClass)enudataset;
                                    objfeatureclass =clsGlobalFunctions._globalFunctions.getFeatureclassByName(clsTestWorkSpace.Workspace, enudataset.Name);
                                    if (objfeatureclass.FeatureType == esriFeatureType.esriFTAnnotation)
                                    {

                                        objannotation = (IAnnoClassAdmin2)objfeatureclass.Extension;
                                        objannotation.AutoCreate = status;

                                        objannotation.UpdateProperties();

                                    }

                                }
                                enudataset = Enumdataset.Next();
                            }
                            break;
                        }
                    case esriDatasetType.esriDTFeatureClass:
                        {
                            objfeatureclass = (IFeatureClass)pdestinationdataset;
                            if (objfeatureclass.FeatureType == esriFeatureType.esriFTAnnotation)
                            {
                                objannotation = (IAnnoClassAdmin2)objfeatureclass.Extension;
                                objannotation.AutoCreate = status;
                                objannotation.UpdateProperties();
                            }
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
            catch
            {
                Logs.writeLine("Error occured in AutoAnotationcreate method ");
            }
        }
    }

}
