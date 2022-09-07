using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ESRI.ArcGIS.ArcMap;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.ADF;
using System.Runtime.InteropServices;
using System.Linq;
using Miner.Framework;
using Miner.Geodatabase;
using Miner.Interop;


namespace PGE.BatchApplication.ReGeneratePriUGAnnotation
{
    public class ReGeneratePriUGAnnotation : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public ReGeneratePriUGAnnotation()
        {

        }

        protected override void OnClick()
        {
            string folderPath = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string path = folderPath + "\\regeneration_of_PriUG_labeltext.log";
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            TextWriter tw = new StreamWriter(path);
            StreamWriter sw = tw as StreamWriter;
            sw.AutoFlush = true;
            tw.WriteLine("Log Start");

            IMxDocument mxDoc = ArcMap.Application.Document as IMxDocument;
            IMap map = mxDoc.FocusMap;

            RegeneratePriUGLabelText(map, tw);
            tw.Close();
        }

        protected override void OnUpdate()
        {

        }

        static public void RegeneratePriUGLabelText(IMap map, TextWriter tw)
        {
            tw.WriteLine("SavingAUMode");
            Type auType = Type.GetTypeFromProgID("mmGeodatabase.MMAutoUpdater");
            object auObj = Activator.CreateInstance(auType);
            IMMAutoUpdater immAutoupdater = auObj as Miner.Interop.IMMAutoUpdater;
            mmAutoUpdaterMode currentAUMode = immAutoupdater.AutoUpdaterMode;

            tw.WriteLine("Getting Layer");
            IFeatureLayer layer = map.get_Layer(0) as IFeatureLayer;

            tw.WriteLine("Getting FeatureClass");
            IFeatureClass featureClass = layer.FeatureClass;

            //Now update features

            IDataset dataset = featureClass as IDataset;
            IWorkspaceEdit wsEdit = dataset.Workspace as IWorkspaceEdit;

            tw.WriteLine("Starting the Edit Operation");
            
            wsEdit.StartEditing(true);
            wsEdit.StartEditOperation();

            tw.WriteLine("Initializing the AU to call");

            
            try
            {

                using (ComReleaser comReleaser = new ComReleaser())
                {
                    string clsID = "Telvent.PGE.ED.PriUGConductorLabel";
                    Miner.Interop.IMMUIDTools auInstance = null;
                    auInstance = new MMUIDToolsClass();
                    object priUGLabelTextAUinit = auInstance.CreateFromClsidString(clsID);

                    //Find the AU, ensure it exists, instantiate it.

                    immAutoupdater.AutoUpdaterMode = mmAutoUpdaterMode.mmAUMNoEvents;
                    IMMSpecialAUStrategyEx priUGLabelTextAU = priUGLabelTextAUinit as IMMSpecialAUStrategyEx;

                    //Create the filter on PriUG
                    IQueryFilter queryFilter = new ESRI.ArcGIS.Geodatabase.QueryFilter();
                    tw.WriteLine("Initializing the definition query:");
                    IFeatureLayerDefinition defQuery = layer as IFeatureLayerDefinition;
                    tw.WriteLine(defQuery.DefinitionExpression);
                    queryFilter.WhereClause = defQuery.DefinitionExpression; //Oracle
                    tw.WriteLine("Opening Cursor");
                    IFeatureCursor updateCursor = featureClass.Search(queryFilter, false);
                    IFeature featureToUpdate = null;
                    while ((featureToUpdate = updateCursor.NextFeature() as IFeature) != null)
                    {
                        tw.WriteLine(DateTime.Now.ToLongTimeString() + " - Updating Feature:" + featureToUpdate.OID);
                        priUGLabelTextAU.Execute(featureToUpdate, mmAutoUpdaterMode.mmAUMStandAlone, mmEditEvent.mmEventFeatureUpdate);
                        featureToUpdate.Store();
                    }
                    immAutoupdater.AutoUpdaterMode = currentAUMode;
                }
            }
            catch (COMException comExc)
            {
                //if we fail, stop the edit session
                wsEdit.StopEditOperation();
                wsEdit.StopEditing(false);
                immAutoupdater.AutoUpdaterMode = currentAUMode;
                tw.WriteLine("Failed to update");
                tw.WriteLine(comExc.Message);
                tw.WriteLine(comExc.StackTrace);
                tw.WriteLine(comExc.TargetSite);
            }

            wsEdit.StopEditOperation();
            wsEdit.StopEditing(true);
            tw.WriteLine("Edits Saved");
            immAutoupdater.AutoUpdaterMode = currentAUMode;

        }

        public static IEnumerable<int> FindIndexes(string text, string query)
        {
            return Enumerable.Range(0, text.Length - query.Length)
                .Where(i => query.Equals(text.Substring(i, query.Length)));
        }

        static private string CharValueToString(char charValue)
        {
            char theChar = charValue; //UTF16 value for char
            return Char.ToString(charValue);
        }



    }
}