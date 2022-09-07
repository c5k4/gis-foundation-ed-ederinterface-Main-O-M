using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Miner.Interop;
using Miner.ComCategories;
using PGE.Common.Delivery.Diagnostics;
using System.Reflection;
using ESRI.ArcGIS.Geodatabase;
using Miner.Geodatabase;
using System.Windows.Forms;
using PGE.Desktop.EDER.PLC;
using ESRI.ArcGIS.Geometry;
using PGE.Desktop.EDER.ArcMapCommands;

namespace PGE.Desktop.EDER.D8TreeTools
{
    [Guid("2acef1b3-c09a-4298-a5a5-abf7d85ed8a5")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("PGE.Desktop.EDER.D8TreeTools.PTTPromoteStructure")]
    [ComponentCategory(ComCategory.D8SelectionTreeTool)]
    public class PTTPromoteStructure : IMMTreeViewTool
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

        public PTTPromoteStructure()
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

            IFeatureClass PTTSupportStructure = null;
            List<int> OIDs = new List<int>();
            
            if (item.ItemType == mmd8ItemType.mmd8itLayer)
            {
                //This is a feature layer with potentially multiple selected features
                ID8Layer layer = item as ID8Layer;
                ID8List d8List = item as ID8List;
                d8List.Reset();
                ID8ListItem listItem = null;
                while ((listItem = d8List.Next()) != null)
                {
                    ID8GeoAssoc geoAssociation = listItem as ID8GeoAssoc;
                    OIDs.Add(geoAssociation.AssociatedGeoRow.OID);
                    PTTSupportStructure = geoAssociation.AssociatedGeoRow.Table as IFeatureClass;
                }
            }
            else if (item.ItemType == mmd8ItemType.mmd8itFeature)
            {
                ID8GeoAssoc geoAssociation = item as ID8GeoAssoc;
                OIDs.Add(geoAssociation.AssociatedGeoRow.OID);
                PTTSupportStructure = geoAssociation.AssociatedGeoRow.Table as IFeatureClass;
            }

            string message = "Are you sure you want to promote this pole?";
            if (OIDs.Count > 1) { message = "Are you sure you want to promote the " + OIDs.Count + " selected poles?"; }
            DialogResult result = MessageBox.Show(message, "Confirm Promote", MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                //Find the Support Structure feature class
                IMMEnumObjectClass supportStructureClasses = ModelNameManager.Instance.ObjectClassesFromModelNameWS(((IDataset)PTTSupportStructure).Workspace, SchemaInfo.General.ClassModelNames.SupportStructure);
                supportStructureClasses.Reset();
                IFeatureClass overheadStructure = supportStructureClasses.Next() as IFeatureClass;
                if (overheadStructure == null) { MessageBox.Show("Unable to find feature class with the " + SchemaInfo.General.ClassModelNames.SupportStructure + " class model name"); }
                else
                {
                    //Promote the list of poles
                    PromotePoles(PTTSupportStructure, overheadStructure, OIDs);
                }
            }
        }

        public string Name
        {
            get {
                _logger.Info("PTT Promote Structure"); 
                return "PTT Promote Structure"; }
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
                    if (item.ItemType == mmd8ItemType.mmd8itLayer)
                    {
                        ID8Layer2 layer2 = item as ID8Layer2;
                        IFeatureClass featClass = layer2.FeatureLayer.FeatureClass;
                        if (ModelNameManager.Instance.ContainsClassModelName(featClass, SchemaInfo.General.ClassModelNames.PTTSupportStructure))
                        {
                            visible = true;
                        }
                    }
                    else if (item.ItemType == mmd8ItemType.mmd8itFeature)
                    {
                        ID8GeoAssoc geoAssociation = item as ID8GeoAssoc;
                        if (ModelNameManager.Instance.ContainsClassModelName(geoAssociation.AssociatedGeoRow.Table as IObjectClass, SchemaInfo.General.ClassModelNames.PTTSupportStructure))
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
        private void PromotePoles(IFeatureClass PTTSupportStructure, IFeatureClass SupportStructure, List<int> OIDs)
        {
            try
            {
                //Start a new edit operation
                Miner.Geodatabase.Edit.Editor.StartOperation();

                int promotedStructures = 0;
                List<string> whereInClauses = GetWhereInClauses(OIDs);
                foreach (string whereInClause in whereInClauses)
                {
                    IQueryFilter qf = new QueryFilterClass();
                    //Should only be able to promote structures that are in our list of OIDs and that are 1 series SAPEQUIPID values
                    qf.WhereClause = PTTSupportStructure.OIDFieldName + " in (" + whereInClause + ") AND SAPEQUIPID LIKE '1%'";

                    int sapEquipIDIdx = PTTSupportStructure.FindField("SAPEQUIPID");
                    IFeatureCursor featCursor = PTTSupportStructure.Search(qf, false);
                    IFeature poleToCopy = null;
                    while ((poleToCopy = featCursor.NextFeature()) != null)
                    {
                        //Copy all of the attributes
                        IFeature newPole = SupportStructure.CreateFeature();
                        newPole.Shape = poleToCopy.ShapeCopy;

                        for (int i = 0; i < poleToCopy.Fields.FieldCount; i++)
                        {
                            IField field = poleToCopy.Fields.get_Field(i);
                            if (field.Editable)
                            {
                                try
                                {
                                    //This field is editable, so let's set it
                                    int associatedFieldIdx = newPole.Fields.FindField(field.Name);
                                    if (associatedFieldIdx > 0)
                                    {
                                        newPole.set_Value(associatedFieldIdx, poleToCopy.get_Value(i));
                                    }
                                    else
                                    {

                                    }
                                }
                                catch (Exception ex)
                                {

                                }
                            }
                        }

                        //Need to set default values if they weren't already set
                        SetDefaultValues(newPole);

                        //Get the PLDBID if it doesn't already exist
                        //Calling Start Anaysis to get PLDBID
                        try
                        {
                            StartAnalysisCall sacObj = new StartAnalysisCall();
                            IPoint point = newPole.Shape as IPoint;
                            string jobNumber = PopulateLastJobNumberUC.jobNumber;
                            //if (string.IsNullOrEmpty(jobNumber)) { jobNumber = "FIF"; }
                            double pldbid = sacObj.Createpldbid(point.X, point.Y, newPole, jobNumber);
                            newPole.set_Value(newPole.Fields.FindField(sacObj._pldbidColName), pldbid);
                        }
                        catch (Exception ex)
                        {
                            _logger.Error("Unable to populate PLDBID: " + ex.Message);
                        }
                        newPole.Store();

                        //Now we need to set the sap equipment id's to -1 for the staging pole
                        //This will notify SAP that this is promoting a staging pole and to update the GUID
                        poleToCopy.set_Value(sapEquipIDIdx, "-1");
                        poleToCopy.Store();
                        promotedStructures++;
                        while (Marshal.ReleaseComObject(poleToCopy) > 0) { }
                    }

                    if (poleToCopy != null) { while (Marshal.ReleaseComObject(poleToCopy) > 0) { } }
                    if (qf != null) { while (Marshal.ReleaseComObject(qf) > 0) { } }
                    if (featCursor != null) { while (Marshal.ReleaseComObject(featCursor) > 0) { } }
                }

                //Stop our edit operation
                Miner.Geodatabase.Edit.Editor.StopOperation("Promote Staging Poles");

                if (promotedStructures == OIDs.Count)
                {
                    if (promotedStructures == 1) { MessageBox.Show(promotedStructures + " pole was promoted", "PTT Promote Poles"); }
                    else { MessageBox.Show(promotedStructures + " poles were promoted", "PTT Promote Poles"); }
                }
                else if (promotedStructures != OIDs.Count)
                {
                    MessageBox.Show(string.Format("Only {0} poles were promoted out of {1} selected poles. Some poles may have already been promoted or some poles may"
                    + " be invalid candidates for promote. Only 1-series SAP Equipment ID poles can be promoted", promotedStructures, OIDs.Count), "PTT Promote Poles"); 
                }
            }
            catch (Exception ex)
            {
                if (Miner.Geodatabase.Edit.Editor.IsOperationInProgress()) { Miner.Geodatabase.Edit.Editor.AbortOperation(); }
                MessageBox.Show("Failed to promote staging poles: " + ex.Message);
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

            int installJobNumberIdx = newPole.Fields.FindField("INSTALLJOBNUMBER");
            int customerOwnedIdx = newPole.Fields.FindField("CUSTOMEROWNED");
            int materialIdx = newPole.Fields.FindField("MATERIAL");
            int jointCountIdx = newPole.Fields.FindField("JOINTCOUNT");
            int poleUseIdx = newPole.Fields.FindField("POLEUSE");
            int SymbolRotationIdx = newPole.Fields.FindField("SYMBOLROTATION");

            if (installJobNumberIdx >= 0) { if (newPole.get_Value(installJobNumberIdx) == DBNull.Value) { newPole.set_Value(installJobNumberIdx, "FIF"); } }
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
    }
}
