using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Miner.Interop;
using PGE.Common.Delivery.Diagnostics;
using ESRI.ArcGIS.Geodatabase;
using System.Reflection;
using Miner.Geodatabase;
using System.Windows.Forms;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using System.Linq;
using Miner.ComCategories;

namespace PGE.Desktop.EDER.D8TreeTools
{
    [Guid("7d1e074d-40ff-4931-9fa8-3c070c9259f9")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("PGE.Desktop.EDER.D8TreeTools.PTTCombineStructure")]
    [ComponentCategory(ComCategory.D8SelectionTreeTool)]
    public class PTTCombineStructure : IMMTreeViewTool
    {
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        #region Com Reg Methods

        /// <summary>
        /// Registers the specified reg key.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComRegisterFunction]
        private static void Register(string regKey)
        {
            Miner.ComCategories.D8SelectionTreeTool.Register(regKey);
        }
        /// <summary>
        /// Uns the register.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComUnregisterFunction]
        private static void UnRegister(string regKey)
        {
            Miner.ComCategories.D8SelectionTreeTool.Unregister(regKey);
        }

        #endregion

        public PTTCombineStructure()
        {

        }

        public int Category
        {
            get { return 16; }
        }

        public void Execute(IMMTreeViewSelection pSelection)
        {
            pSelection.Reset();
            ID8ListItem item = pSelection.Next;

            IFeatureClass SupportStructureFeatureClass = null;
            IFeatureClass PTTSupportStructureFeatureClass = null;
            IFeature combineFromFeature = null;
            List<int> OIDs = new List<int>();
            if (item.ItemType == mmd8ItemType.mmd8itFeature)
            {
                ID8GeoAssoc geoAssociation = item as ID8GeoAssoc;
                combineFromFeature = geoAssociation.AssociatedGeoRow as IFeature;
            }

            int sapEquipIDIdx = combineFromFeature.Table.FindField("SAPEQUIPID");


            List<int> featuresToSelect = new List<int>();
            //Determine the support structure and PTT staging table feature classess
            if (ModelNameManager.Instance.ContainsClassModelName(combineFromFeature.Table as IObjectClass, SchemaInfo.General.ClassModelNames.SupportStructure))
            {
                //Support structure feature class
                SupportStructureFeatureClass = combineFromFeature.Table as IFeatureClass;
                PTTSupportStructureFeatureClass = GetFeatureClass(((IDataset)combineFromFeature.Table).Workspace, SchemaInfo.General.ClassModelNames.PTTSupportStructure);
                featuresToSelect = GetSelectedFeatures(SchemaInfo.General.ClassModelNames.PTTSupportStructure);

                PTTCombineForm combinePoleWindow = new PTTCombineForm("Select Staging Pole Object ID to combine to", featuresToSelect);
                combinePoleWindow.ShowDialog(new ArcMapWindow(Application));

                if (combinePoleWindow.DialogResult == DialogResult.OK)
                {
                    //User selected a valid pole to combine to
                    int combineToOID = combinePoleWindow.ObjectIDSelected;
                    CombinePole(PTTSupportStructureFeatureClass.GetFeature(combineToOID), combineFromFeature, false);
                }
            }
            else
            {
                //Support structure feature class
                PTTSupportStructureFeatureClass = combineFromFeature.Table as IFeatureClass;
                SupportStructureFeatureClass = GetFeatureClass(((IDataset)combineFromFeature.Table).Workspace, SchemaInfo.General.ClassModelNames.SupportStructure);
                featuresToSelect = GetSelectedFeatures(SchemaInfo.General.ClassModelNames.SupportStructure);

                PTTCombineForm combinePoleWindow = new PTTCombineForm("Select Pole Object ID to combine to", featuresToSelect);
                combinePoleWindow.ShowDialog(new ArcMapWindow(Application));

                if (combinePoleWindow.DialogResult == DialogResult.OK)
                {
                    //User selected a valid pole to combine to
                    int combineToOID = combinePoleWindow.ObjectIDSelected;
                    CombinePole(combineFromFeature, SupportStructureFeatureClass.GetFeature(combineToOID), true);
                }
            }
        }

        public string Name
        {
            get {
                _logger.Info("PTT Combine Structure");
                return "PTT Combine Structure"; }
        }

        public int Priority
        {
            get { return 0; }
        }

        /// <summary>
        /// Tool is enabled and visible if the mapper is editing and the feature class is the PTT Support Structure feature class
        /// </summary>
        /// <param name="pSelection"></param>
        /// <returns></returns>
        public int get_Enabled(IMMTreeViewSelection pSelection)
        {
            try
            {
                pSelection.Reset();
                ID8ListItem item = pSelection.Next;

                bool enabled = Miner.Geodatabase.Edit.Editor.EditState == Miner.Geodatabase.Edit.EditState.StateEditing;
                bool visible = false;
                if (item != null)
                {
                    if (item.ItemType == mmd8ItemType.mmd8itFeature)
                    {
                        ID8GeoAssoc geoAssociation = item as ID8GeoAssoc;
                        if (ModelNameManager.Instance.ContainsClassModelName(geoAssociation.AssociatedGeoRow.Table as IObjectClass, SchemaInfo.General.ClassModelNames.PTTSupportStructure)
                            || ModelNameManager.Instance.ContainsClassModelName(geoAssociation.AssociatedGeoRow.Table as IObjectClass, SchemaInfo.General.ClassModelNames.SupportStructure))
                        {
                            visible = true;
                        }
                    }
                }
                if (enabled && visible) { return (int)(mmToolState.mmTSEnabled | mmToolState.mmTSVisible); }
                else if (visible) { return (int)mmToolState.mmTSVisible; }
                else { return (int)mmToolState.mmTSNone; }
            }
            catch (Exception Ex)
            {
                //ignore the error.
            }
            return (int)mmToolState.mmTSNone;
        }

        /// <summary>
        /// Promotes the list of Object IDs for staging poles to the support structure table
        /// </summary>
        /// <param name="PTTSupportStructure"></param>
        /// <param name="SupportStructure"></param>
        /// <param name="OIDs"></param>
        private void CombinePole(IFeature PTTSupportStructure, IFeature SupportStructure, bool takeStagingPoleShape)
        {
            try
            {
                //Check the material for our support structure
                int materialIdx = SupportStructure.Fields.FindField("MATERIAL");
                object poleMaterialObj = SupportStructure.get_Value(materialIdx);

                /* We will not do any sort of validation of subtype and material.  This will be handled by mappers Defect ID 154
                IRowSubtypes rowSubtypes = SupportStructure as IRowSubtypes;
                if (rowSubtypes.SubtypeCode != 1 || poleMaterialObj == null || poleMaterialObj.ToString().ToUpper() != "WOOD")
                {
                    //This is an invalid support structure.  Only Wood distribution poles are valid
                    MessageBox.Show("Support structure " + SupportStructure.OID + " is not a valid candidate for combine. Support structure must have Subtype of 'Pole' and Material of 'WOOD'", "PTT Combine Pole");
                    return;
                }
                */

                //Check to make sure this SAPEQUIPID is a 1 series.  If it's not, then it is not a valid combine candidate
                int sapEquipIDIdx = PTTSupportStructure.Table.FindField("SAPEQUIPID");
                object sapEquipIDObj = PTTSupportStructure.get_Value(sapEquipIDIdx);
                if (sapEquipIDObj == null || !sapEquipIDObj.ToString().StartsWith("1"))
                {
                    MessageBox.Show("Staging pole " + PTTSupportStructure.OID + " is not a valid candidate for combine. Staging pole must have a 1-series SAP Equipment id", "PTT Combine Pole");
                    return;
                }

                //Start a new edit operation
                Miner.Geodatabase.Edit.Editor.StartOperation();

                try
                {
                    //Get the retire GUID field.
                    int retireGuidIdx = SupportStructure.Fields.FindField("REPLACEGUID");
                    int globalIDIdx = PTTSupportStructure.Fields.FindField("GLOBALID");
                    object retireGuid = PTTSupportStructure.get_Value(globalIDIdx);

                    //Combine the attributes onto the Support Structure feature
                    if (takeStagingPoleShape) { SupportStructure.Shape = PTTSupportStructure.ShapeCopy; }

                    //Update retire guid to the global ID of the PTT Support Structure
                    SupportStructure.set_Value(retireGuidIdx, retireGuid);
                    SupportStructure.Store();

                    //Delete the staging structure
                    PTTSupportStructure.Delete();
                }
                catch (Exception ex) { throw new Exception("Failed to combine to existing pole: " + ex.Message); }

                //Stop our edit operation
                Miner.Geodatabase.Edit.Editor.StopOperation("Combine Pole");
            }
            catch (Exception ex)
            {
                if (Miner.Geodatabase.Edit.Editor.IsOperationInProgress()) { Miner.Geodatabase.Edit.Editor.AbortOperation(); }
                MessageBox.Show("Failed to combine pole: " + ex.Message);
            }
        }


        private void SetDefaultValues(IFeature newPole)
        {
            /*  The following default values are defined for promoting poles.  If the ED11 interface set up different values for these then we won't assign them.  
             * But if there are null values, then we can default these values.
             *    - Job# => "FIF"
b.	    - Angle/Offset => 0 (Non-Existent Field. Supposed to be SymbolRotation)
c.	    - CustomerOwned => N
d.	    - Subtype => Pole (Already set by the PTT interface
e.	    - Material => Wood 
f.	    - JointCount => 1 (If PoleUsage attribute is set to Communication then set this value to 2)
g.	    - PoleUse => Distribution

             */

            int customerOwnedIdx = newPole.Fields.FindField("CUSTOMEROWNED");
            int materialIdx = newPole.Fields.FindField("MATERIAL");
            int jointCountIdx = newPole.Fields.FindField("JOINTCOUNT");
            int poleUseIdx = newPole.Fields.FindField("POLEUSE");
            int SymbolRotationIdx = newPole.Fields.FindField("SYMBOLROTATION");

            if (customerOwnedIdx >= 0) { if (newPole.get_Value(customerOwnedIdx) == DBNull.Value) { newPole.set_Value(customerOwnedIdx, "N"); } }
            if (materialIdx >= 0) { if (newPole.get_Value(materialIdx) == DBNull.Value) { newPole.set_Value(materialIdx, "WOOD"); } }
            if (jointCountIdx >= 0) { if (newPole.get_Value(jointCountIdx) == DBNull.Value) { newPole.set_Value(jointCountIdx, "1"); } }
            if (poleUseIdx >= 0) { if (newPole.get_Value(poleUseIdx) == DBNull.Value) { newPole.set_Value(poleUseIdx, 4); } }
            if (SymbolRotationIdx >= 0) { if (newPole.get_Value(SymbolRotationIdx) == DBNull.Value) { newPole.set_Value(SymbolRotationIdx, 0); } }
        }

        /// <summary>
        /// Obtains a list of where in clauses for a list of guids
        /// </summary>
        /// <param name="guids"></param>
        /// <returns></returns>
        private static List<string> GetWhereInClauses(List<int> guids)
        {
            try
            {
                List<string> whereInClauses = new List<string>();
                StringBuilder builder = new StringBuilder();

                for (int i = 0; i < guids.Count; i++)
                {
                    if ((((i % 1000) == 0) && i != 0) || (guids.Count == i + 1))
                    {
                        builder.Append(guids[i]);
                        whereInClauses.Add(builder.ToString());
                        builder = new StringBuilder();
                    }
                    else { builder.Append(guids[i] + ","); }
                }
                return whereInClauses;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to create guid where in clauses. Error: " + ex.Message);
            }
        }

        /// <summary>
        /// Obtains a list of where in clauses for a list of guids
        /// </summary>
        /// <param name="guids"></param>
        /// <returns></returns>
        private List<string> GetWhereInClauses(List<string> guids)
        {
            try
            {
                List<string> whereInClauses = new List<string>();
                StringBuilder builder = new StringBuilder();

                int counter = 0;
                foreach (string guid in guids)
                {
                    if (counter == 999)
                    {
                        builder.Append("'" + guid + "'");
                        whereInClauses.Add(builder.ToString());
                        builder = new StringBuilder();
                        counter = 0;
                    }
                    else
                    {
                        builder.Append("'" + guid + "',");
                        counter++;
                    }
                }

                if (builder.Length > 0)
                {
                    string whereInClause = builder.ToString();
                    whereInClause = whereInClause.Substring(0, whereInClause.LastIndexOf(","));
                    whereInClauses.Add(whereInClause);
                }
                return whereInClauses;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to create guid where in clauses. Error: " + ex.Message);
            }
        }

        private List<int> GetSelectedFeatures(string poleModelName)
        {
            List<int> selectedObjectIDs = new List<int>();
            IMxDocument mxDoc = Application.Document as IMxDocument;
            IMap focusMap = mxDoc.FocusMap;
            UID uid = new UIDClass();
            uid.Value = "{40A9E885-5533-11d0-98BE-00805F7CED21}";
            IEnumLayer featureLayers = focusMap.get_Layers(uid, true);
            IFeatureLayer featureLayer = null;
            while ((featureLayer = featureLayers.Next() as IFeatureLayer) != null)
            {
                //Make sure that the feature class is not null
                if (featureLayer.FeatureClass == null) { continue; }

                //If this contains our model name then we need to add these selected features as well to our list
                if (ModelNameManager.Instance.ContainsClassModelName(featureLayer.FeatureClass as IObjectClass, poleModelName))
                {
                    IFeatureSelection featureSelection = featureLayer as IFeatureSelection;
                    ISelectionSet selectedFeatures = featureSelection.SelectionSet;
                    IEnumIDs selectedIDs = selectedFeatures.IDs;
                    selectedIDs.Reset();
                    int selectedID = -1;
                    while ((selectedID = selectedIDs.Next()) > 0)
                    {
                        selectedObjectIDs.Add(selectedID);
                    }
                }
            }

            //Return a distinct list of object IDs
            selectedObjectIDs.Sort();
            return selectedObjectIDs.Distinct().ToList();
        }

        private IFeatureClass GetFeatureClass(IWorkspace workspace, string modelName)
        {
            //Find the staging pole feature class
            IMMEnumObjectClass featureClasses = ModelNameManager.Instance.ObjectClassesFromModelNameWS(workspace, modelName);
            featureClasses.Reset();
            IFeatureClass featureClass = featureClasses.Next() as IFeatureClass;
            if (featureClass == null) { MessageBox.Show("Unable to find feature class with the " + modelName + " class model name"); }

            return featureClass;
        }

        private static IApplication _application = null;
        /// <summary>
        /// Returns the ESRI Application reference.
        /// </summary>
        private static IApplication Application
        {
            get
            {
                try
                {
                    if (_application == null)
                    {
                        Type type = Type.GetTypeFromProgID("esriFramework.AppRef");
                        object obj = Activator.CreateInstance(type);
                        _application = (IApplication)obj;
                    }
                    return _application;
                }
                catch
                {
                    // Couldn't create the AppRef.  Probably not in ArcMap or ArcCatalog.
                    return null;
                }
            }
        }
    }
}
