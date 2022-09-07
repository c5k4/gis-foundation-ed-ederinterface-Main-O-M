using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;
using System.Collections;
using System.Runtime.InteropServices;
using Miner.Interop;
using ESRI.ArcGIS.Carto;
using Miner.Interop.Process;


namespace PGEReGeoCodingProcess
{
    class ClsMM
    {
        private static mmAutoUpdaterMode objautoupdateroldmode;
        //private static IMMAutoUpdater autoupdater = null;

        
        internal static void stopAutoupdaters(IMMAutoUpdater autoupdater)
        {
            ClsMM.DisableAutoupdaters(ref autoupdater);
            //ClsMM.StopAutoAnnoCreate(false);
        }
        internal static void startAutoupdaters(IMMAutoUpdater autoupdater)
        {
            ClsMM.EnableAutoUpdaters(autoupdater);
            //ClsMM.StopAutoAnnoCreate(true);
        }

        //
        internal static mmAutoUpdaterMode DisableAutoupdaters(ref IMMAutoUpdater autoupdater)
        {
            object objAutoUpdater = null;

            //Create an MMAutoupdater 
            objAutoUpdater = Activator.CreateInstance(Type.GetTypeFromProgID("mmGeodatabase.MMAutoUpdater"));
            autoupdater = objAutoUpdater as IMMAutoUpdater;
            //Save the existing mode
            mmAutoUpdaterMode oldMode = autoupdater.AutoUpdaterMode;
            //Turn off autoupdater events
            autoupdater.AutoUpdaterMode = mmAutoUpdaterMode.mmAUMNoEvents; //mmAutoUpdaterMode.mmAUMNoEvents
            // insert code that needs to execute while autoupdaters 
            return oldMode;
        }

        internal static mmAutoUpdaterMode SetFeederManagerModeAutoupdaters(ref IMMAutoUpdater autoupdater)
        {
            object objAutoUpdater = null;

            //Create an MMAutoupdater 
            objAutoUpdater = Activator.CreateInstance(Type.GetTypeFromProgID("mmGeodatabase.MMAutoUpdater"));
            autoupdater = objAutoUpdater as IMMAutoUpdater;
            //Save the existing mode
            mmAutoUpdaterMode oldMode = autoupdater.AutoUpdaterMode;
            //Turn off autoupdater events
            autoupdater.AutoUpdaterMode = mmAutoUpdaterMode.mmAUMFeederManager; //mmAutoUpdaterMode.mmAUMNoEvents
            // insert code that needs to execute while autoupdaters 
            return oldMode;
        }

        internal static void StopAutoAnnoCreate(Boolean status)
        {
             IEnumDataset DestinationEnumdataset = clsCommonVariables.pWrkSpace.get_Datasets(esriDatasetType.esriDTAny);
            //Loop through all the Destination datasets to append the data 
            IDataset Destinationdataset = DestinationEnumdataset.Next();
            ArrayList objarraylist = null;
            while (Destinationdataset != null)
            {
                Application.DoEvents();
                AutoAnotationcreate(Destinationdataset, status, ref objarraylist);
                Destinationdataset = DestinationEnumdataset.Next();
            }
        }
                /// <summary>
        /// This method is to find the feature class and set the auto anotation create false 
        /// </summary>
        /// <param name="pdestinationdataset"></param>
        private static void AutoAnotationcreate(IDataset pdestinationdataset, Boolean status, ref ArrayList strAnnoclass)
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
                                    objfeatureclass = ClsGLobalFunctions.getFeatureclassByName(clsCommonVariables.pWrkSpace, enudataset.Name);
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
                ClsLogReports.writeLine("Error occured in AutoAnotationcreate method ");
            }
        }
    
        internal static void EnableAutoUpdaters(IMMAutoUpdater autoupdater)
        {
 	        if (autoupdater != null)
            {
                autoupdater.AutoUpdaterMode = objautoupdateroldmode;
            }
        }
    }
}
