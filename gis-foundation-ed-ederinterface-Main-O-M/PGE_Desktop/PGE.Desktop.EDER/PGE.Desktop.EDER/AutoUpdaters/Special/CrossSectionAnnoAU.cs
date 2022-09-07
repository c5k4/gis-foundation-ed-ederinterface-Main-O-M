using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using Miner.ComCategories;
using Miner.Interop;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Diagnostics;
using PGE.Common.Delivery.Framework;
using PGE.Desktop.EDER.UFM;

namespace PGE.Desktop.EDER.AutoUpdaters.Special
{
    [ComVisible(true)]
    [Guid("F3D9154C-13BE-4A47-BD57-F28B8C7B27FC")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    public class CrossSectionAnnoAU : BaseSpecialAU
    {
        #region Constants

        private const string AuName = "PGE Cross Section Anno AU";
        private const int AnchorElement = 0;

        #endregion

        #region Member vars

        private IDictionary<string, string> FieldToDisplayMap = new Dictionary<string, string>
        {
            {"FIBEROPTICIDC", "Fiber"},
            {"VENTIDC", "Vent"},
            {"TELCOIDC", "Telco"},
            {"ELECTRICTRANSMISSIONIDC", "ET"},
            {"GASLINEIDC", "Gas Fac"},
            {"SCADAIDC", "SCADA"},
            {"SFDTIDC", "SFDT"},
            {"MUNICABLEIDC", "Muni Cable"},
            {"TEMPERATUREIDC", "Temp/PW"},
            {"MISCIDC1", "Parallel Cond"}
        };

        private static readonly Log4NetLogger Logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        // For keeping track of neutral lines to be added
        private IDictionary<int, string> _neutrals = null;

        private IDictionary<string, IList<int>> _unsplits = null;

        private IList<string> _sizes = null;
        private IList<string> _materials = null;

        #endregion

        #region Constructor

        public CrossSectionAnnoAU() : base(AuName) {}

        #endregion

        #region Overrides

        protected override bool InternalEnabled(IObjectClass objectClass, mmEditEvent eEvent)
        {
            bool enabled = false;

            try
            {
                // Enable if the OC passed in is the Cross Section Anno feature
                enabled = ModelNameFacade.ContainsClassModelName(objectClass,
                    SchemaInfo.UFM.ClassModelNames.CrossSection);
            }
            catch (Exception ex)
            {
                Logger.Warn("Failed enabled test for AU " + AuName + ": " + ex);
            }

            return enabled;
        }

        protected override bool CanExecute(mmAutoUpdaterMode eAUMode)
        {
            //Enable if Application type is ArcMap
            return eAUMode == mmAutoUpdaterMode.mmAUMArcMap;
        }

        protected override void InternalExecute(IObject pObject, mmAutoUpdaterMode eAUMode, mmEditEvent eEvent)
        {
            // Log entry
            Logger.Debug(AuName + " launched");

            mmAutoUpdaterMode currentAuMode = eAUMode;
            Type auType = Type.GetTypeFromProgID("mmGeodatabase.MMAutoUpdater");
            object auObj = Activator.CreateInstance(auType);
            IMMAutoUpdater immAutoupdater = auObj as IMMAutoUpdater;

            try
            {
                // Update the cross section with custom changes
                Update100CrossSection(pObject, eEvent);

                // Clone to the 50 and 10 scales
                immAutoupdater.AutoUpdaterMode = mmAutoUpdaterMode.mmAUMNoEvents;
                Update50CrossSection(pObject, eEvent);
                Update10CrossSection(pObject, eEvent);

                // Update the screen
                if (eAUMode == mmAutoUpdaterMode.mmAUMArcMap)
                {
                    UfmHelper.RefreshView();
                }
            }
            catch (Exception ex)
            {
                Logger.Warn("Failed to execute PGE Cross Section Annotation AU: " + ex);
            }
            finally
            {
                // Always restore the AU mode
                immAutoupdater.AutoUpdaterMode = currentAuMode;
            }
        }

        #endregion

        #region Private methods

        #region Top level methods

        private void Update100CrossSection(IObject pObject, mmEditEvent editEvent)
        {
            // If were a graphic...
            if (UfmHelper.IsArrow(pObject) == false)
            {
                // Get the related conduit feature
                IFeature conduit = UfmHelper.GetParentFeature(pObject);                

                // Handle conductor counts and eliminate phasing
                IDictionary<string, IList<int>> splitDucts = UfmHelper.GetSplitDucts(conduit);
                StoreUnsplitDucts(conduit, splitDucts);
                UpdateForSplits(conduit, pObject, splitDucts);
                RemovePhasingAndInsulation(pObject, splitDucts);

                // Update text formatting and add direction
                string direction = GetDirection(conduit);
                IElement updatedAnno = UpdateText(pObject, ((IAnnotationFeature2)pObject).Annotation, direction, false, true, true);
                ((IAnnotationFeature2)pObject).Annotation = updatedAnno;

                // Add indicators and right justify duct numbers
                AddMiscIdc100ScaleAnno(pObject);
                UpdateDuctNumbers(pObject);

                // Store it
                updatedAnno = ((IAnnotationFeature2)pObject).Annotation;
                IAnnotationFeature2 featAnnoProps = (IAnnotationFeature2)pObject;
                featAnnoProps.Annotation = updatedAnno;
            }
        }

        private void Update10CrossSection(IObject pObject, mmEditEvent eEvent)
        {
            if (eEvent != mmEditEvent.mmEventFeatureUpdate) return;

            // Get the existing annotation geometry
            IElement clonedAnnoElement = UfmHelper.CloneAnnotationElement(pObject, 120);
            IFeature conduit = UfmHelper.GetParentFeature(pObject);
            ISet scale10AnnoSet = UfmHelper.GetRelatedObjects(conduit, SchemaInfo.UFM.ClassModelNames.CrossSection10);
            IFeature scale10Anno;

            Logger.Debug("Existing 1:10 x-section anno found, updating...");

            for (scale10AnnoSet.Reset(), scale10Anno = (IFeature) scale10AnnoSet.Next();
                scale10Anno != null;
                scale10Anno = (IFeature) scale10AnnoSet.Next()) 
            {
                IElement resultElement = FormatAndFilter10ScaleGroupElement(scale10Anno, clonedAnnoElement);
                ((IGroupElement)resultElement).Element[0].Geometry = ((IGroupElement)((IAnnotationFeature2)scale10Anno).Annotation).Element[0].Geometry;

                ((IAnnotationFeature2)scale10Anno).Annotation = resultElement;
                scale10Anno.Store();
            }
        }

        private void Update50CrossSection(IObject pObject, mmEditEvent eEvent)
        {
            // Get the annotation feature class
            IFeatureClass fcCrossSection50 = ModelNameFacade.FeatureClassByModelName((pObject.Class as IDataset).Workspace, SchemaInfo.UFM.ClassModelNames.CrossSection50); 
            Logger.Debug("Creating 1:50 x-section annotation in feature class " + fcCrossSection50.AliasName);

            // If we got it...
            if (fcCrossSection50 != null)
            {
                IAnnotationFeature2 originalAnnoFeat = (IAnnotationFeature2) pObject;
                IGroupElement orig100ScaleGroup = (IGroupElement)originalAnnoFeat.Annotation;
                double currentAngle = ((ITextElement) (orig100ScaleGroup).Element[0]).Symbol.Angle;
                IPoint anchorPoint = orig100ScaleGroup.Element[AnchorElement].Geometry.Envelope.UpperLeft;

                ApplyAngleRotation50CrossSection(pObject, -1 * currentAngle, anchorPoint);

                // Get the existing annotation geometry
                IElement clonedAnnoElement = UfmHelper.CloneAnnotationElement(pObject, 600);

                ApplyAngleRotation50CrossSection(pObject, currentAngle, anchorPoint);

                // Are we creating or updating?
                if (eEvent == mmEditEvent.mmEventFeatureCreate)
                {
                    if (UfmHelper.IsArrow(pObject) == false)
                    {
                        // Apply cosmetic updates
                        UpdateText(pObject, clonedAnnoElement, String.Empty, true, false, false);
                    }

                    // Get the OID of the conduit
                    int parentOid = GetParentConduitOid(pObject);
                    Logger.Debug("\tfor conduit " + parentOid);

                    // Build a new annotation feature
                    IFeature featCrossSection50Anno = CreateNewAnnotation(fcCrossSection50, clonedAnnoElement, parentOid);

                    // Persist the changes
                    if (featCrossSection50Anno != null)
                    {
                        Logger.Debug("Saving");
                        featCrossSection50Anno.Store();
                    }
                }
                else
                {
                    if (UfmHelper.IsArrow(pObject) == false)
                    {
                        // Update the existing anno
                        Logger.Debug("Existing 1:50 x-section anno found, updating...");
                        UpdateExistingCrossSectionAnno(pObject, clonedAnnoElement,
                            SchemaInfo.UFM.ClassModelNames.CrossSection50);
                    }
                }
            }
        }

        #endregion

        #region Creation and Cloning methods

        private IFeature CreateNewAnnotation(IFeatureClass fcCrossSectionLowScale, IElement clonedAnnoElement,
            int parentOid)
        {
            IFeature featCrossSectionLowScaleAnno = null;

            try
            {
                // Create a new row
                featCrossSectionLowScaleAnno = fcCrossSectionLowScale.CreateFeature();

                // Copy the shape info over
                IAnnotationFeature2 featAnnoProps = (IAnnotationFeature2)featCrossSectionLowScaleAnno;
                featAnnoProps.Annotation = clonedAnnoElement;
                featAnnoProps.LinkedFeatureID = parentOid;
                featAnnoProps.AnnotationClassID = 0;
                featAnnoProps.Status = esriAnnotationStatus.esriAnnoStatusPlaced;
            }
            catch (Exception ex)
            {
                // Log a warning
                Logger.Warn(ex.Message);
            }

            // Return the result
            return featCrossSectionLowScaleAnno;
        }

        private void UpdateExistingCrossSectionAnno(IObject pObject, IElement clonedAnnoElement, string annoModelName)
        {
            // Get a list of x-section 50 annos
            IFeature conduit = UfmHelper.GetParentFeature(pObject);
            ISet crossSectionLowScaleAnnos = UfmHelper.GetRelatedObjects(conduit, annoModelName);

            // Get the first anno
            IFeature crossSectionLowScaleAnnoFt;

            // While there are x-section annos to update...
            while ((crossSectionLowScaleAnnoFt = crossSectionLowScaleAnnos.Next() as IFeature) != null)
            {
                // Get the x and y of the cloned 1:100 anchor element
                IGroupElement hundredScaleElements = (IGroupElement)clonedAnnoElement;
                double hundredX = hundredScaleElements.Element[AnchorElement].Geometry.Envelope.UpperLeft.X;
                double hundredY = hundredScaleElements.Element[AnchorElement].Geometry.Envelope.UpperLeft.Y;

                // Get the x and y of the 1:50 anchor element
                IAnnotationFeature2 lowScaleAnno = (IAnnotationFeature2)crossSectionLowScaleAnnoFt;
                IGroupElement lowScaleElements = (IGroupElement)lowScaleAnno.Annotation;
                double lowScaleX = lowScaleElements.Element[AnchorElement].Geometry.Envelope.UpperLeft.X;
                double lowScaleY = lowScaleElements.Element[AnchorElement].Geometry.Envelope.UpperLeft.Y;

                // Determine offset between original and current feature based on anchor element
                double xOffset = lowScaleX - hundredX;
                double yOffset = lowScaleY - hundredY;

                double beforeAngle = ((ITextElement)(lowScaleElements).Element[AnchorElement]).Symbol.Angle;

                // Now lets build a new set of elements based on the existing ones
                IGroupElement newGroup = new GroupElementClass();

                // For each element...
                int count = hundredScaleElements.ElementCount;
                for (int index = 0; index < count; index++)
                {
                    IElement hundredScaleElement = hundredScaleElements.Element[index];
                    string newText = String.Empty;

                    // Update it with an offset and some other cosmetic changes
                    IElement currentElement = Update50or100ScaleElement(hundredScaleElement, xOffset, yOffset, newText, true, false);

                    // Add it to our new group
                    newGroup.AddElement(currentElement);
                }

                // Update the geometry and save
                lowScaleAnno.Annotation = newGroup as IElement;

                IPoint anchorPoint = lowScaleElements.Element[AnchorElement].Geometry.Envelope.UpperLeft;
                ApplyAngleRotation50CrossSection(crossSectionLowScaleAnnoFt, beforeAngle, anchorPoint);
            }
        }

        private void ApplyAngleRotation50CrossSection(IObject newAnno, double angle, IPoint aboutPoint)
        {
            IAnnotationFeature2 annoFeat2 = newAnno as IAnnotationFeature2;
            IFeature feat = annoFeat2 as IFeature;

            if (feat.Shape == null) return;

            IGroupElement annoGroup = (IGroupElement)annoFeat2.Annotation;

            for (int i = 0; i < annoGroup.ElementCount; i++)
            {
                IElement annoElement = annoGroup.Element[i];
                IGeometry annoGeom = annoElement.Geometry;

                ITransform2D transform2D = annoGeom as ITransform2D;
                double radianAngle = (Math.PI / 180.0) * angle;
                transform2D.Rotate(aboutPoint, radianAngle);

                annoElement.Geometry = transform2D as IGeometry;
                annoFeat2.Annotation = annoElement;

                if (annoElement is ITextElement)
                {
                    IFormattedTextSymbol ts = (IFormattedTextSymbol)((ITextElement)annoElement).Symbol;
                    ts.Angle = angle;
                    ((ITextElement)annoElement).Symbol = ts;
                }
            }

            annoFeat2.Annotation = (IElement)annoGroup;
            feat.Store();
        }

        #endregion

        #region Updating conductor text methods

        private void StoreUnsplitDucts(IFeature conduit, IDictionary<string, IList<int>> splitDucts)
        {
            IDictionary<int, string> conductors = new Dictionary<int, string>();
            _unsplits = new Dictionary<string, IList<int>>();
            _sizes = new List<String>();
            _materials = new List<String>();
            IMMDuctBankConfig dbc = UfmHelper.GetDuctBankConfig(conduit);

            _unsplits = new Dictionary<string, IList<int>>();
            ID8List dbcList = (ID8List)dbc;
            dbcList.Reset();
            ID8List ductDefObjs = (ID8List)dbcList.Next(false);
            ID8List ductDefinitionObj;

            IObjectClass ductDefOc = ModelNameFacade.ObjectClassByModelName((conduit.Class as IDataset).Workspace, "DUCT");
            int sizeIndex = ModelNameFacade.FieldIndexFromModelName(ductDefOc, "ACTUALDUCTSIZE");

            for (ductDefObjs.Reset(), ductDefinitionObj = (ID8List)ductDefObjs.Next(); ductDefinitionObj != null;
                ductDefinitionObj = (ID8List)ductDefObjs.Next())
            {
                if (ductDefinitionObj is IMMDuctDefinition)
                {
                    IMMDuctDefinition ductDef = (IMMDuctDefinition)ductDefinitionObj;
                    
                    // Store the material in case there are multiple
                    if (ductDef.material != null)
                    {
                        if (ductDef.material != string.Empty)
                        {
                            if (_materials.Contains(ductDef.material) == false)
                            {
                                _materials.Add(ductDef.material);
                            }
                        }
                    }
                    
                    ID8ListItem ductDefId8Item;
                    
                    // Get cables
                    for (ductDefinitionObj.Reset(), ductDefId8Item = ductDefinitionObj.Next(); ductDefId8Item != null;
                        ductDefId8Item = ductDefinitionObj.Next())
                    {
                        if (ductDefId8Item is IMMAttribute)
                        {
                            // If its the size, store it
                            IMMAttribute attr = ductDefId8Item as IMMAttribute;
                            if (attr.Name == "ACTUALSIZE")
                            {
                                object val = attr.Value;
                                if (val != null)
                                {
                                    if (val.ToString() != string.Empty)
                                    {
                                        if (_sizes.Contains(val.ToString()) == false)
                                        {
                                            _sizes.Add(val.ToString());
                                        }
                                    }
                                }
                            }
                        }

                        if (ductDefId8Item is IMMDuctPhase)
                        {
                            IMMDuctPhase ductPhase = (IMMDuctPhase)ductDefId8Item;

                            bool add = true;
                            if (splitDucts.ContainsKey(ductDef.ductID) == true)
                            {
                                IList<int> splitOids = splitDucts[ductDef.ductID];
                                if (splitOids.Contains(ductPhase.cableID) == true)
                                {
                                    add = false;
                                }
                            }

                            if (add == true)
                            {
                                if (_unsplits.ContainsKey(ductDef.ductID) == false)
                                {
                                    IList<int> ids = new List<int>();
                                    ids.Add(ductPhase.cableID);
                                    _unsplits.Add(ductDef.ductID, ids);
                                }
                                else
                                {
                                    _unsplits[ductDef.ductID].Add(ductPhase.cableID);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void UpdateForSplits(IFeature conduit, IObject anno, IDictionary<string, IList<int>> splitDucts)
        {
            // If this is even an issue...
            if (splitDucts.Count > 0 || _unsplits.Count > 0)
            {
                // Get the annotation group element and the conductor text element within it
                IGroupElement groupElement = (IGroupElement)((IAnnotationFeature2)anno).Annotation;
                int textElemIdx = GetConductorTextElementIndex(groupElement);

                // If we found it...
                if (textElemIdx != 0)
                {
                    // Get a list of the current annotation broken out and keep track of new annotation lines
                    IList<string> currentAnnoLines = GetConductorAnnoLines(groupElement.Element[textElemIdx] as ITextElement);
                    IList<string> newAnnoLines = new List<string>();

                    // Keep track of added neutral lines
                    _neutrals = new Dictionary<int, string>();

                    // for each annotation line
                    foreach (string line in currentAnnoLines)
                    {
                        // Assume we'll go with the current line
                        string newLine = line;

                        // Find the duct number
                        int index = line.IndexOf(':');
                        if (index > 0)
                        {
                            string ductNo = line.Substring(0, index).Trim();
                            if (ductNo.Length > 0)
                            {
                                // If this duct has a split in it
                                if (splitDucts.ContainsKey(ductNo) == true)
                                {
                                    newLine = UpdateForSplit(conduit, line, ductNo, splitDucts[ductNo]);
                                }

                                newLine = CheckUnsplits(conduit, ductNo, newLine);
                            }
                        }

                        // Add back whatever we found
                        if (newLine != string.Empty)
                        {
                            newAnnoLines.Add(newLine);
                        }
                    }

                    // Add in any neutrals we stored
                    if (_neutrals.Count > 0)
                    {
                        // For each conductor stored
                        foreach (int conductorOid in _neutrals.Keys)
                        {
                            // Get the neutral text
                            string neutralLine = _neutrals[conductorOid];

                            if (neutralLine != string.Empty)
                            {
                                // Find the correct location for it and insert
                                if ((groupElement.Element[textElemIdx] as ITextElement).Text.IndexOf(neutralLine) != -1) continue;
                                int i = 0;
                                while (i < newAnnoLines.Count &&
                                       Convert.ToInt32(newAnnoLines[i].Substring(0, newAnnoLines[i].IndexOf(':'))) <=
                                       Convert.ToInt32(neutralLine.Substring(0, neutralLine.IndexOf(':'))))
                                {
                                    i++;
                                }
                                newAnnoLines.Insert(i, neutralLine);
                            }
                        }
                    }

                    // Re-save the anno
                    ((ITextElement)groupElement.Element[textElemIdx]).Text = String.Join("\n", newAnnoLines.ToArray());
                    ((IAnnotationFeature2)anno).Annotation = groupElement as IElement;
                }
            }
        }

        private string CheckUnsplits(IFeature conduit, string ductNo, string line)
        {
            // This code looks for neutral lines on duct positions that aren't split
            // Current format doesn't have neutral lines split correctly

            // Note: Lots of inefficiencies in this code.. it does slow things that are already being done but
            // this falls outside of the structure
            string newLine = line;

            if (_unsplits.ContainsKey(ductNo) == true)
            {
                foreach (int id in _unsplits[ductNo])
                {
                    IFeature conductor = GetConductor(conduit, id);
                    if (conductor != null)
                    {
                        string infoMn = GetInfoModelName(conductor);
                        StoreNeutral(conductor, infoMn, line, string.Empty);
                        if (_neutrals.ContainsKey(id) == true)
                        {
                            string featureMn = GetClassModelName(conductor);
                            object phase = UfmHelper.GetFieldValue(conductor as IRow, SchemaInfo.Electric.FieldModelNames.PhaseDesignation);
                            if (phase != null)
                            {
                                string phasing = phase.ToString();
                                ISet infoRecs = UfmHelper.GetRelatedObjects(conductor as IRow, infoMn);
                                IObject info = GetMatchingInfoRecord(infoRecs, phasing, featureMn);
                                if (info != null)
                                {
                                    IList<string> pos = new List<string>();
                                    pos.Add(ductNo);
                                    IList<string> phasingList = new List<string>();
                                    phasingList.Add(phasing);
                                    newLine = MergeLines(pos, phasingList, line, infoRecs, ductNo, featureMn);
                                }
                                else
                                {
                                    // strip phase text
                                    IObject neutralInfo = GetMatchingInfoRecord(infoRecs, "8", featureMn);
                                    if (neutralInfo != null)
                                    {
                                        string neutralText = UpdateSplitText(newLine, neutralInfo);
                                        if (neutralText != string.Empty)
                                        {
                                            if (newLine.Contains(neutralText) == true)
                                            {
                                                string matchString = " & " + neutralText.Trim();
                                                newLine = newLine.Replace(matchString, string.Empty);
                                                newLine = newLine.Replace(neutralText, string.Empty);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                foreach (int id in _neutrals.Keys)
                {
                    if (_unsplits[ductNo].Contains(id) == true)
                    {
                        _unsplits[ductNo].Remove(id);
                    }
                }

            }

            if (_unsplits.ContainsKey(ductNo) == true)
            {
                if (_unsplits[ductNo].Count == 0)
                {
                    _unsplits.Remove(ductNo);
                }
            }

            return newLine;
        }

        private string UpdateForSplit(IFeature conduit, string line, string duct, IList<int> conductors)
        {
            string newLine = line;

            try
            {
                // Get the conductor and its corresponding model names
                IFeature conductor = GetConductor(conduit, conductors[0]);
                string featureMn = GetClassModelName(conductor);
                string infoMn = GetInfoModelName(conductor);

                // Get the attributed relationships between the conduit and this conductor - 
                // Note that each conductor should appear more than once since its split (but we'll see other conductors as well)
                IRelationshipClass rc = ModelNameFacade.RelationshipClassFromModelName(((IObject)conduit).Class, esriRelRole.esriRelRoleAny, featureMn);
                IEnumRelationship eRel = rc.GetRelationshipsForObject(conduit);
                eRel.Reset();
                IRelationship rel = eRel.Next();

                // We don't know what to do until weve looked at all relationships between the current conductor and conduit
                // So loop through them all to build a list of actions, then loop through them again to carry out those actions
                IList<string> positions = new List<string>();
                IList<string> phases = new List<string>();
                IList<string> actions = new List<string>();

                // For each related conductor
                while (rel != null)
                {
                    // If its the conductor were updating
                    IFeature relatedConductor = rel.DestinationObject as IFeature;
                    if (relatedConductor == conductor)
                    {
                        IRow relationObj = rel as IRow;
                        string conductorDuctPos = GetAttributedValue(relationObj, "ULS_POSITION");
                        string conductorPhase = GetAttributedValue(relationObj, "PHASEDESIGNATION");

                        // If the same conductor appears in the same duct more than once, we need to merge the first and delete the rest
                        if (positions.Contains(conductorDuctPos) == true)
                        {
                            int index = positions.IndexOf(conductorDuctPos);

                            IList<string> tempPos = new List<string>();
                            string updatePos = string.Empty;
                            foreach (string position in positions)
                            {
                                if (tempPos.Contains(position) == false)
                                {
                                    tempPos.Add(position);
                                    if (position != conductorDuctPos)
                                    {
                                        updatePos = position;
                                    }
                                }
                            }

                            if (tempPos.Count > 1)
                            {
                                actions[index] = "MergeSplitCount";
                                if (updatePos != string.Empty)
                                {
                                    if (actions[positions.IndexOf(updatePos)] == "n/a")
                                    {
                                        ISet infos = UfmHelper.GetRelatedObjects(conductor as IRow, infoMn);
                                        if (infos.Count == 1)
                                        {
                                            actions[positions.IndexOf(updatePos)] = "SplitCount";
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (actions[index] != "Merge")
                                {
                                    actions[index] = "Merge";
                                }
                            }
                            actions.Add("Delete");
                        }
                        else
                        {
                            actions.Add("n/a");
                            if (positions.Count > 1)
                            {
                                if (positions[0] != conductorDuctPos && positions[0] == positions[1])
                                {
                                    if (actions[0] == "Merge")
                                    {
                                        actions[0] = "MergeSplitCount";
                                        actions[actions.Count - 1] = "SplitCount";
                                    }
                                }
                            }
                        }

                        positions.Add(conductorDuctPos);
                        phases.Add(conductorPhase);
                    }

                    rel = eRel.Next();
                }

                // If everything is n/a, then we actually need to split but we couldn't know this until we looked at them all
                bool splitCount = true;
                foreach (string action in actions)
                {
                    if (action != "n/a")
                    {
                        splitCount = false;
                    }
                }
                if (splitCount == true)
                {
                    for (int index=0; index < actions.Count; index++)
                    {
                        actions[index] = "SplitCount";
                    }
                }

                // Process the actions
                newLine = ProcessActions(eRel, conductor, duct, line, featureMn, infoMn, positions, phases, actions);

                // Check for neutral
                newLine = StoreNeutral(conductor, infoMn, line, newLine);
            }
            catch (Exception ex)
            {
                Logger.Warn("Failed to handle split: " + ex.ToString());
            }

            return newLine;
        }

        private string ProcessActions(IEnumRelationship eRel,
                                        IFeature conductor,
                                        string duct,
                                        string line,
                                        string featureMn,
                                        string infoMn,
                                        IList<string> positions,
                                        IList<string> phases,
                                        IList<string> actions)
        {
            ISet infoRecords = null;

            string newLine = line;

            // Get the phase were after
            string linePhasing = GetPhaseFromLine(line);

            // For each related conductor
            eRel.Reset();
            IRelationship rel = eRel.Next();
            while (rel != null)
            {
                // If its the conductor were updating
                IFeature relatedConductor = rel.DestinationObject as IFeature;
                if (relatedConductor == conductor)
                {
                    // and in the duct were updating
                    IRow relationObj = rel as IRow;
                    string conductorDuctPos = GetAttributedValue(relationObj, "ULS_POSITION");
                    if (conductorDuctPos == duct)
                    {
                        // Get the phase in the relationship
                        string conductorPhase = GetAttributedValue(relationObj, "PHASEDESIGNATION");
                        //if (conductorPhase != string.Empty && conductorPhase == linePhasing)
                        if (conductorPhase == linePhasing)
                        {
                            int conductorCount = 0;

                            // Get the conductor info records and the total conductor count
                            if (infoRecords == null)
                            {
                                infoRecords = UfmHelper.GetRelatedObjects(conductor, infoMn);
                                conductorCount = GetConductorCount(infoRecords);
                            }

                            // Get the matching info record
                            IObject infoRecord = GetMatchingInfoRecord(infoRecords, conductorPhase, featureMn);
                            if (infoRecord != null)
                            {
                                // Process the action
                                int actionsIndex = phases.IndexOf(conductorPhase);
                                if (actions[actionsIndex] == "Merge")
                                {
                                    newLine = MergeLines(positions, phases, newLine, infoRecords, duct, featureMn);
                                }
                                else if (actions[actionsIndex] == "MergeSplitCount" || actions[actionsIndex] == "n/a")
                                {
                                    newLine = MergeLines(positions, phases, newLine, infoRecords, duct, featureMn);
                                    newLine = GetConductorCountText(newLine, conductor, conductorCount, conductorPhase, positions.Count, infoRecord);
                                }
                                else if (actions[actionsIndex] == "Delete")
                                {
                                    newLine = string.Empty;
                                }
                                else if (actions[actionsIndex] == "SplitCount")
                                {
                                    string tempNewLine = MergeLines(positions, phases, line, infoRecords, duct, featureMn);
                                    if (newLine.EndsWith("\r") == false)
                                    {
                                        tempNewLine = tempNewLine.Replace("\r", string.Empty);
                                    }
                                    newLine = newLine.Replace("ABC: ", string.Empty);

                                    if (newLine == tempNewLine)
                                    {
                                        newLine = GetConductorCountText(newLine, conductor, conductorCount, conductorPhase, positions.Count, infoRecord);
                                    }
                                    else
                                    {
                                        bool bUpdated = false;
                                        IObject neutralInfoRecord = null;
                                        int neutralPhase = 8;
                                        while (neutralInfoRecord == null && neutralPhase < 12)
                                        {
                                            neutralInfoRecord = GetMatchingInfoRecord(infoRecords, neutralPhase.ToString(), featureMn);
                                            neutralPhase++;
                                        }
                                        if (neutralInfoRecord != null)
                                        {
                                            string neutralText = UpdateSplitText(line, neutralInfoRecord);
                                            if (neutralText != string.Empty)
                                            {
                                                neutralText = neutralText.Replace("\r", string.Empty);
                                                string testLine = line.Replace(" &" + neutralText, string.Empty);
                                                if (testLine == tempNewLine)
                                                {
                                                    newLine = GetConductorCountText(testLine, conductor, conductorCount, conductorPhase, positions.Count, infoRecord);
                                                    bUpdated = true;
                                                }
                                            }
                                        }

                                        if (bUpdated == false)
                                        {
                                            newLine = tempNewLine;
                                        }
                                    }

                                    //string tempOldLine = string.Empty;
                                    //if (newLine.Length > tempNewLine.Length)
                                    //{
                                    //    tempOldLine = newLine.Substring(0, tempNewLine.Length).Trim() + "\r";
                                    //}
                                    //else
                                    //{
                                    //    tempOldLine = newLine;
                                    //}

                                    //if (tempOldLine == tempNewLine)
                                    //{
                                    //    newLine = GetConductorCountText(newLine, conductor, conductorPhase, positions.Count, infoRecord);
                                    //}
                                    //else
                                    //{
                                    //    newLine = tempNewLine;
                                    //    //IObject neutralInfoRecord = GetMatchingInfoRecord(infoRecords, "8", featureMn);
                                    //    //if (neutralInfoRecord != null)
                                    //    //{
                                    //    //    string neutralText = UpdateSplitText(line, neutralInfoRecord);
                                    //    //    tempNewLine = tempNewLine + " & " + neutralText;
                                    //    //}
                                    //    //if (newLine == tempNewLine)
                                    //    //{
                                    //    //    newLine = GetConductorCountText(newLine, conductor, conductorPhase, positions.Count, infoRecord);
                                    //    //}
                                    //    //{
                                    //    //    newLine = tempNewLine;
                                    //    //}
                                    //}
                                }
                            }

                            break;
                        }
                    }
                }

                rel = eRel.Next();
            }

            if (line.EndsWith("IDLE\r") || line.EndsWith("IDLE"))
            {
                if (newLine.EndsWith("IDLE\r") == false && newLine.EndsWith("IDLE") == false && newLine != string.Empty)
                {
                    newLine = newLine.Replace("IDLE", string.Empty);
                    if (newLine.EndsWith("\r") == true)
                    {
                        newLine = newLine.Substring(0, newLine.Length - 1) + " IDLE\r";
                    }
                    else
                    {
                        newLine += " IDLE";
                    }
                }
            }

            return newLine;
        }

        private int GetConductorCount(ISet infoRecords)
        {
            int count = 0;

            if (infoRecords != null)
            {
                if (infoRecords.Count > 0)
                {
                    infoRecords.Reset();
                    IObject infoRecord = infoRecords.Next() as IObject;
                    while (infoRecord != null)
                    {
                        object ductPhaseObj = UfmHelper.GetFieldValue(infoRecord, SchemaInfo.Electric.FieldModelNames.PhaseDesignation);
                        if (infoRecord.Class.AliasName == "DC Conductor Info")
                        {
                            ductPhaseObj = "7";
                        }
                        if (ductPhaseObj != null)
                        {
                            string ductPhase = ductPhaseObj.ToString();
                            if (ductPhase == "1" || ductPhase == "2" || ductPhase == "3" || ductPhase == "4" ||
                                ductPhase == "5" || ductPhase == "6" || ductPhase == "7")
                            {
                                object ductConductorCount = UfmHelper.GetFieldValue(infoRecord, SchemaInfo.Electric.FieldModelNames.CondutorCount);
                                if (ductConductorCount != null)
                                {
                                    try
                                    {
                                        int conCount;
                                        int.TryParse(ductConductorCount.ToString(), out conCount);
                                        count = count + conCount;
                                    }
                                    catch (Exception)
                                    {
                                        // do nothing
                                    }
                                }
                            }
                        }

                        infoRecord = infoRecords.Next() as IObject;
                    }
                }

                infoRecords.Reset();
            }

            return count;
        }

        private string MergeLines(IList<string> positions, IList<string> phases, string line, ISet infoRecords, string duct, string featureMn)
        {
            string newLine = line;

            string phasing = string.Empty;
            string conductorText = string.Empty;
            int ductIndex = 0;

            // For each position
            foreach (string ductPos in positions)
            {
                // If its the duct we care about
                if (ductPos == duct)
                {
                    // Build up the phase
                    if (phases[ductIndex] == "4")
                    {
                        phasing = "A" + phasing;
                    }
                    else if (phases[ductIndex] == "2")
                    {
                        if (phasing == "A")
                        {
                            phasing = phasing + "B";
                        }
                        else if (phasing == "C")
                        {
                            phasing = "B" + phasing;
                        }
                        else if (phasing == "AC")
                        {
                            phasing = "ABC";
                        }
                        else
                        {
                            phasing = "B";
                        }
                    }
                    else if (phases[ductIndex] == "1")
                    {
                        phasing = phasing + "C";
                    }

                    // And the conductor text
                    IObject infoRecord2 = GetMatchingInfoRecord(infoRecords, phases[ductIndex].ToString(), featureMn);
                    if (conductorText == string.Empty)
                    {                        
                        conductorText = UpdateSplitText(newLine, infoRecord2).Trim();
                    }
                    else
                    {
                        string conductorPart = UpdateSplitText(newLine, infoRecord2).Trim();
                        if (conductorPart != conductorText)
                        {
                            conductorText += " & " + conductorPart;
                        }
                    }
                }
                ductIndex++;
            }

            // Ensure the conductor type is present
            string conductorType = string.Empty;
            string firstPart = string.Empty;
            string secondPart = string.Empty;
            if (newLine.Contains(":") == true)
            {
                int index = line.LastIndexOf(":");
                conductorType = newLine.Substring(index + 2, 1);
                firstPart = line.Substring(0, index + 1);
                secondPart = line.Substring(index + 1);
            }

            if (conductorText != string.Empty)
            {
                if (conductorText.StartsWith(conductorType) == false)
                {
                    conductorText = conductorType + " " + conductorText;
                }
            }

            // Merge it all together
            if (phasing == string.Empty)
            {
                newLine = duct + ": " + conductorText + "\r";
            }
            else
            {
                newLine = duct + ": " + phasing + ": " + conductorText + "\r";
            }

            // Return the result
            return newLine;
        }

        private string GetConductorCountText(string line, IFeature conductor, int conductorCount, string conductorPhase, int conductorSplitCount, IObject infoRecord)
        {
            string newLine = line;

            // Get the full phasing from the conductor
            string linePhase = GetPhaseFromLine(line);
            object conPhaseObj = null;
            if (linePhase != "0")
            {
                conductorPhase = linePhase;
            }
            conPhaseObj = UfmHelper.GetFieldValue(conductor, SchemaInfo.Electric.FieldModelNames.PhaseDesignation);
            if (conPhaseObj != null && !(conPhaseObj is DBNull))
            {
                // Calculate the multiplier
                double mult;
                if (conductorPhase != string.Empty && infoRecord.Class.AliasName != "DC Conductor Info")
                {
                    mult = GetMultiplier(conductorPhase, conPhaseObj.ToString());
                }
                else
                {
                    mult = 1.0 / conductorSplitCount;
                }

                // Get the conductor count
                //ISet infos = UfmHelper.GetRelatedObjects(conductor, 
                object ductConductorCount = UfmHelper.GetFieldValue(infoRecord, SchemaInfo.Electric.FieldModelNames.CondutorCount);
                if (ductConductorCount != null)
                {
                    // Calculate the new count
                    int conCount = int.Parse(ductConductorCount.ToString());
                    conCount = conductorCount;
                    int newConCount = Convert.ToInt32(Math.Round(conCount * mult));
                    if (line.IndexOf(conCount.ToString() + "-") > 0)
                    {
                        if (newConCount == 0 || newConCount == 1)
                        {
                            newLine = line.Replace(conCount.ToString() + "-", string.Empty);
                        }
                        else
                        {
                            newLine = line.Replace(conCount.ToString() + "-", newConCount + "-");
                        }
                    }
                }
            }

            // Return the result
            return newLine;
        }

        private string StoreNeutral(IFeature conductor, string infoMn, string line, string newLine)
        {
            const int NEUTRAL_PHASE = 8;

            string updatedLine = newLine;
            string neutralLine = "";

            string conductorType = XSectionConductorHelper.GetConductorType(conductor);

            if (line.Contains(conductorType) == true)
            {
                // Get its info records
                ISet infos = UfmHelper.GetRelatedObjects(conductor, infoMn);

                // If it has a neutral line
                object info = infos.Next();
                while (info != null)
                {
                    IRow infoRecord = info as IRow;
                    object phase = UfmHelper.GetFieldValue(infoRecord, SchemaInfo.Electric.FieldModelNames.PhaseDesignation);
                    if (phase != null)
                    {
                        int phasing = (int) phase;
                        if ((phasing & NEUTRAL_PHASE) == NEUTRAL_PHASE)
                        {
                            // Get the neutrals duct position
                            string ductPos = string.Empty;
                            object ductPosObj = UfmHelper.GetFieldValue(infoRecord, "DUCTPOSITION");
                            if (ductPosObj != null)
                            {
                                ductPos = ductPosObj.ToString();

                                // Get its text and save it off
                                string conductorPart = UpdateSplitText(line, infoRecord as IObject).Trim();
                                if (updatedLine != string.Empty && line.Length > 0 && ductPos == line.Substring(0, ductPos.Length))
                                {
                                    if (updatedLine.EndsWith("\r") == true)
                                    {
                                        if (conductorPart.Contains("IDLE") && updatedLine.Contains("IDLE"))
                                        {
                                            updatedLine = updatedLine.Replace(" IDLE", string.Empty);
                                        }
                                        updatedLine = updatedLine.Substring(0, updatedLine.Length - 1) + " & " + conductorPart + "\r";
                                    }
                                    else
                                    {
                                        updatedLine += " & " + conductorPart;
                                    }
                                    if (_neutrals.ContainsKey(conductor.OID) == false)
                                    {
                                        _neutrals.Add(conductor.OID, neutralLine = string.Empty);
                                    }
                                    else
                                    {
                                        _neutrals[conductor.OID] = string.Empty;
                                    }
                                }
                                else
                                {
                                    if (_neutrals.ContainsKey(conductor.OID) == false)
                                    {
                                        _neutrals.Add(conductor.OID, neutralLine = ductPos + ": N: " + conductorPart + "\r");
                                    }
                                }
                            }
                        }
                    }

                    info = infos.Next();
                }
            }

            return updatedLine;
        }

        private string GetPhaseFromLine(string line)
        {
            string phase = string.Empty;

            // Get the indices of the colons
            int prePhaseIndex = line.IndexOf(':') + 2;
            int phaseIndex = line.LastIndexOf(':');
                        
            if (prePhaseIndex == phaseIndex)
            {
                phase = line.Substring(phaseIndex - 1, 1);
            }
            else
            {
                if (prePhaseIndex <= phaseIndex)
                {
                    phase = line.Substring(prePhaseIndex, phaseIndex - prePhaseIndex);
                }
            }

            // Convert from string to number
            if (phase == "A") phase = "4";
            if (phase == "B") phase = "2";
            if (phase == "C") phase = "1";
            if (phase == "AB") phase = "6";
            if (phase == "AC") phase = "5";
            if (phase == "BC") phase = "3";
            if (phase == "ABC") phase = "7";

            return phase;
        }

        //private string UpdatePhasingForNeutral(string line, IObject infoRecord)
        //{
        //    string newLine = line;

        //    // Get its phasing
        //    object infoPhasingObj = UfmHelper.GetFieldValue(infoRecord, SchemaInfo.Electric.FieldModelNames.PhaseDesignation);
        //    if (infoPhasingObj != null)
        //    {
        //        // If its 8 (Neutral)
        //        int infoPhasing = int.Parse(infoPhasingObj.ToString());
        //        if (infoPhasing == 8)
        //        {
        //            // Replace the phase designation in the text with N
        //            newLine = newLine.Replace(": P", ": N:");
        //            newLine = newLine.Replace(": S", ": N:");
        //        }
        //    }

        //    return newLine;
        //}

        private IObject GetMatchingInfoRecord(ISet infoRecords, string conductorPhase, string featureMn)
        {
            IObject infoRecord = null;

            if (infoRecords != null)
            {
                // Get the phase were looking for
                int ductPhase = 0;
                if (featureMn != SchemaInfo.UFM.ClassModelNames.DcConductor && conductorPhase != string.Empty)
                {
                    ductPhase = int.Parse(conductorPhase);
                }

                // Loop over each info record until we find the one we want
                infoRecords.Reset();
                infoRecord = infoRecords.Next() as IObject;
                while (infoRecord != null)
                {
                    // If its a DC, just go with the first one
                    if (featureMn == SchemaInfo.UFM.ClassModelNames.DcConductor)
                    {
                        break;
                    }

                    // Get the info records phasing
                    object infoPhasingObj = UfmHelper.GetFieldValue(infoRecord, SchemaInfo.Electric.FieldModelNames.PhaseDesignation);
                    if (infoPhasingObj != null)
                    {
                        int infoPhasing = int.Parse(infoPhasingObj.ToString());

                        // If they AND together, go with it
                        if (infoPhasing < 8)
                        {
                            if ((infoPhasing & ductPhase) == ductPhase)
                            {
                                break;
                            }
                        }
                        else
                        {
                            if (infoPhasing == ductPhase)
                            {
                                break;
                            }
                        }
                    }
                    infoRecord = infoRecords.Next() as IObject;
                }

                //if (infoRecord == null)
                //{
                //    infoRecords.Reset();
                //    infoRecord = infoRecords.Next() as IObject;
                //}
            }

            return infoRecord;
        }

        private string UpdateSplitText(string line, IObject infoRecord)
        {
            string firstPart = string.Empty;
            string secondPart = string.Empty;
            string partMatch = string.Empty;
            string newLine = line;
            
            // Break the line up into its constituent parts
            if (line.Contains(":") == true)
            {
                int index = line.LastIndexOf(":");
                firstPart = line.Substring(0, index + 1);
                secondPart = line.Substring(index + 1);
            }

            // If the last part has splits in it...
            if (secondPart.Contains(" &") == true)
            {
                string[] parts = secondPart.Split(new string[] { " &" }, StringSplitOptions.None);

                // Get the size and material
                string size = string.Empty;
                object sizeObj = UfmHelper.GetFieldValue(infoRecord as IRow, SchemaInfo.Electric.FieldModelNames.ConductorSize);
                if (sizeObj != null && !(sizeObj is DBNull))
                {
                    size = sizeObj.ToString();
                }
                string material = string.Empty;
                object materialObj = UfmHelper.GetFieldValue(infoRecord as IRow, SchemaInfo.Electric.FieldModelNames.ConductorMaterial);
                if (materialObj != null && !(materialObj is DBNull))
                {
                    material = materialObj.ToString();
                    if (material.ToUpper() == "UNK" || material.ToUpper() == "NONE" || material.ToUpper() == "BUSDUCT")
                    {
                        material = string.Empty;
                    }
                }

                string matchString = size + material;

                if (matchString != string.Empty)
                {
                    // Search in the string for the bit we want
                    int matchCount = 0;
                    //string partMatch = string.Empty;
                    foreach (string part in parts)
                    {
                        if (part.Contains(matchString) == true)
                        {
                            matchCount++;
                            partMatch = part;
                        }
                    }

                    // Return everything with the other parts excluded
                    if (matchCount == 1)
                    {
                        newLine = firstPart + partMatch;
                    }
                    else if (matchCount > 1)
                    {
                        string count = string.Empty;
                        object countObj = UfmHelper.GetFieldValue(infoRecord as IRow, SchemaInfo.Electric.FieldModelNames.CondutorCount);
                        if (countObj != null)
                        {
                            count = countObj.ToString();
                        }
                        if (count != "1")
                        {
                            matchString = count + "-" + matchString;

                            // Search in the string for the bit we want
                            matchCount = 0;
                            //string partMatch = string.Empty;
                            foreach (string part in parts)
                            {
                                if (part.Contains(matchString) == true)
                                {
                                    matchCount++;
                                    partMatch = part;
                                }
                            }

                            // Return everything with the other parts excluded
                            if (matchCount == 1)
                            {
                                newLine = firstPart + partMatch;
                            }
                        }
                    }
                }
            }
            else
            {
                partMatch = secondPart;
            }

            return partMatch;
        }

        private void RemovePhasingAndInsulation(IObject anno, IDictionary<string, IList<int>> splitDucts)
        {
            try
            {
                // Get the annotation group element and the conductor text element within it
                IGroupElement groupElement = (IGroupElement)((IAnnotationFeature2)anno).Annotation;
                int textElemIdx = GetConductorTextElementIndex(groupElement);

                if (textElemIdx > 0)
                {
                    // Get a list of the current annotation broken out and keep track of new annotation lines
                    IList<string> currentAnnoLines = GetConductorAnnoLines(groupElement.Element[textElemIdx] as ITextElement);
                    IList<string> newAnnoLines = new List<string>();

                    // For each line of annotation
                    foreach (string annoLine in currentAnnoLines)
                    {
                        string newAnnoLine = annoLine;

                        int index = annoLine.IndexOf(':');
                        if (index > 0)
                        {
                            // Eliminate phasing unless split
                            string ductId = annoLine.Substring(0, index).Trim();
                            if (splitDucts.ContainsKey(ductId) == false && _unsplits.ContainsKey(ductId) == true)
                            {
                                newAnnoLine = UfmHelper.EliminatePhase(newAnnoLine);                                
                            }

                            // Eliminate any conductor insulation
                            newAnnoLine = UfmHelper.EliminateConductorInsulation(anno, newAnnoLine);
                        }
                        newAnnoLines.Add(newAnnoLine);
                    }

                    // Re-save the anno
                    ((ITextElement)groupElement.Element[textElemIdx]).Text = String.Join("\n", newAnnoLines.ToArray());
                    ((IAnnotationFeature2)anno).Annotation = groupElement as IElement;
                }
                else
                {
                    Logger.Warn("Could not find an ITextElement in the cross section group element. Exiting.");
                    return;
                }

            }
            catch (Exception ex)
            {
                Logger.Warn("Failed to remove phasing: " + ex.ToString());
            }
        }

        private void AddMiscIdc100ScaleAnno(IObject anno)
        {
            IGroupElement groupElement = (IGroupElement)((IAnnotationFeature2)anno).Annotation;
            List<string> currentAnnoLines = null;
            List<string> newAnnoLines = GetCustomDuctLabels(anno);

            if (newAnnoLines.Count > 0)
            {
                int textElemIdx = groupElement.ElementCount;
                string ductText = string.Empty;

                for (int i = 0; i < groupElement.ElementCount && textElemIdx > groupElement.ElementCount - 1; i++)
                {
                    if ((groupElement.Element[i] is ITextElement) &&
                        ((ITextElement)groupElement.Element[i]).Text.Contains(":"))
                    {
                        textElemIdx = i;
                    }
                }


                ITextElement elementToClone = null;
                if (textElemIdx < groupElement.ElementCount)
                {
                    ITextElement data = (ITextElement)groupElement.Element[textElemIdx];
                    ductText = data.Text;
                    ductText = ductText + "\r\n";
                    currentAnnoLines = new List<string>(data.Text.Substring(0, ductText.LastIndexOf('\r')).Split('\n'));
                    ductText = data.Text;
                }
                else
                {
                    // No existing annotation, need to add a new element to host it
                    Logger.Warn("Could not find an ITextElement in the cross section group element. Adding...");

                    // The last element should be the header text
                    if (groupElement.Element[groupElement.ElementCount - 1] is ITextElement)
                    {
                        // Clone that element
                        elementToClone = groupElement.Element[groupElement.ElementCount - 1] as ITextElement;
                        IClone clonedElement = (elementToClone as IClone).Clone();
                        ITextElement newElement = clonedElement as ITextElement;

                        // Calculate and assign a new x, y location for the new element to be at
                        IPoint startPoint = groupElement.Element[0].Geometry as IPoint;
                        IPoint location = GetConductorTextLocation(startPoint, groupElement, newElement.Symbol.Angle);
                        (newElement as IElement).Geometry = location as IGeometry;

                        // Add the element to the group
                        newElement.Text = string.Empty;
                        groupElement.AddElement(newElement as IElement);

                        // Rest of the algortihm requires the element index and existing text
                        textElemIdx = groupElement.ElementCount - 1;
                        currentAnnoLines = new List<string>();
                        ductText = "";
                    }
                }

                if (currentAnnoLines != null)
                {
                    int i = 0;
                    while (i <= currentAnnoLines.Count && newAnnoLines.Count > 0)
                    {
                        string newDuctInfo = newAnnoLines[0];
                        newAnnoLines.RemoveAt(0);

                        if (ductText.IndexOf(newDuctInfo) != -1) continue;

                        while (i < currentAnnoLines.Count &&
                            Convert.ToInt32(currentAnnoLines[i].Substring(0, currentAnnoLines[i].IndexOf(':'))) <=
                               Convert.ToInt32(newDuctInfo.Substring(0, newDuctInfo.IndexOf(':'))))
                        {
                            i++;
                        }

                        currentAnnoLines.Insert(i, newDuctInfo);
                    }
                }

                ((ITextElement)groupElement.Element[textElemIdx]).Text = String.Join("\n", currentAnnoLines.ToArray());
                ((IAnnotationFeature2)anno).Annotation = groupElement as IElement;
            }
        }

        private List<string> GetCustomDuctLabels(IObject anno)
        {
            List<string> newLabels = new List<string>();
            ISet conduitSet = PgeModelNameFacade.GetRelatedObjects(anno, SchemaInfo.UFM.ClassModelNames.Conduit);
            IMMDuctBankConfig dbc = UfmHelper.GetDuctBankConfig((IFeature)conduitSet.Next());

            ID8List dbcList = (ID8List)dbc;
            dbcList.Reset();
            ID8List ductDefObjs = (ID8List)dbcList.Next(false);
            ID8List ductDefinitionObj;

            for (ductDefObjs.Reset(), ductDefinitionObj = (ID8List)ductDefObjs.Next();
                ductDefinitionObj != null;
                ductDefinitionObj = (ID8List)ductDefObjs.Next())
            {
                if (!(ductDefinitionObj is IMMDuctDefinition)) continue;
                IMMDuctDefinition ductDef = (IMMDuctDefinition)ductDefinitionObj;

                ID8ListItem ductDefId8Item;

                for (ductDefinitionObj.Reset(), ductDefId8Item = ductDefinitionObj.Next();
                    ductDefId8Item != null;
                    ductDefId8Item = ductDefinitionObj.Next())
                {
                    if (!(ductDefId8Item is IMMAttribute)) continue;

                    IMMAttribute ductDefAttr = (IMMAttribute)ductDefId8Item;

                    if (ductDefAttr.Value != null && FieldToDisplayMap.ContainsKey(ductDefAttr.Name) &&
                        Convert.ToInt32(ductDefAttr.Value) == 1)
                    {
                        newLabels.Add(ductDef.ductNumber + ": " + FieldToDisplayMap[ductDefAttr.Name]);
                    }
                }
            }

            return newLabels;
        }

        /// <summary>
        /// Calculates the location (IPoint) where conductor text should be placed if there isn't any to edit
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="groupElement"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        private IPoint GetConductorTextLocation(IPoint startPoint, IGroupElement groupElement, double angle)
        {
            // Start with the original point
            IPoint newPoint = ((startPoint as IClone).Clone()) as IPoint;

            try
            {
                // Apply a fake rotation to the entire x-section geometry (that we won't save) so that its horizontal
                IGeometry origGeom = (groupElement as IElement).Geometry;
                IGroupElement cloneGroup = ((groupElement as IClone).Clone()) as IGroupElement;

                // Rotation has to be done element by element :(
                for (int i = 0; i < cloneGroup.ElementCount; i++)
                {
                    ITransform2D transform2D = cloneGroup.Element[i].Geometry as ITransform2D;
                    transform2D.Rotate(startPoint, angle * -1);
                    cloneGroup.Element[i].Geometry = transform2D as IGeometry;
                }

                // We don't know how many ducts are in this ductbank or how high and wide they are, so...
                // Find the rightmost point of the rotated geometry as well as the lowest Y of the highest duct
                double rightMostX = 0;
                double highestY = 0;
                for (int i = 0; i < cloneGroup.ElementCount; i++)
                {
                    // If its a polygon (aka: a duct and not text)
                    if (cloneGroup.Element[i].Geometry is IPolygon)
                    {
                        // Get the rightmost point of the Element
                        IElement ele = cloneGroup.Element[i];
                        if (ele.Geometry.Envelope.XMax > rightMostX)
                        {
                            rightMostX = ele.Geometry.Envelope.XMax;
                        }

                        // The first duct will always be the highest
                        if (highestY == 0)
                        {
                            highestY = ele.Geometry.Envelope.YMin;
                        }
                    }
                }

                // These values, with a offset applied, should put us in the right location for the text
                newPoint.X = rightMostX + 5;
                newPoint.Y = highestY + 0.5;

                // Now rotate the point around the original start point
                ITransform2D transformBack2D = newPoint as ITransform2D;
                transformBack2D.Rotate(startPoint, angle);

                // And voila!
                newPoint = transformBack2D as IPoint;
            }
            catch (Exception ex)
            {
                // Not the end of the world, just log a warning
                Logger.Warn("Failed to calculate location for conductor text: " + ex.ToString());
            }

            // Return the final point
            return newPoint;
        }

        /// <summary>
        /// Calculates the location (IPoint) where conductor text should be placed if there isn't any to edit
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="groupElement"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        private IPoint GetHeaderTextLocation(IPoint startPoint, IGroupElement groupElement, double angle, string newText)
        {
            // Start with the original point
            IPoint newPoint = ((startPoint as IClone).Clone()) as IPoint;

            try
            {
                // Apply a fake rotation to the entire x-section geometry (that we won't save) so that its horizontal
                IGeometry origGeom = (groupElement as IElement).Geometry;
                IGroupElement cloneGroup = ((groupElement as IClone).Clone()) as IGroupElement;

                // Rotation has to be done element by element :(
                for (int i = 0; i < cloneGroup.ElementCount; i++)
                {
                    ITransform2D transform2D = cloneGroup.Element[i].Geometry as ITransform2D;
                    transform2D.Rotate(startPoint, angle * -1);
                    cloneGroup.Element[i].Geometry = transform2D as IGeometry;
                }

                // We don't know how many ducts are in this ductbank or how high and wide they are, so...
                // Find the rightmost point of the rotated geometry as well as the lowest Y of the highest duct
                double leftMostX = 0;
                double highestY = 0;
                for (int i = 0; i < cloneGroup.ElementCount; i++)
                {
                    // If its a polygon (aka: a duct and not text)
                    if (cloneGroup.Element[i].Geometry is IPolygon)
                    {
                        // Get the rightmost point of the Element
                        IElement ele = cloneGroup.Element[i];
                        if (ele.Geometry.Envelope.XMin < leftMostX || leftMostX == 0)
                        {
                            leftMostX = ele.Geometry.Envelope.XMin;
                        }

                        // The first duct will always be the highest
                        if (highestY == 0)
                        {
                            highestY = ele.Geometry.Envelope.YMin;
                        }
                    }
                }

                // These values, with a offset applied, should put us in the right location for the text
                newPoint.X = leftMostX;
                newPoint.Y = highestY + 10;

                // Now rotate the point around the original start point
                ITransform2D transformBack2D = newPoint as ITransform2D;
                transformBack2D.Rotate(startPoint, angle);

                // And voila!
                newPoint = transformBack2D as IPoint;
            }
            catch (Exception ex)
            {
                // Not the end of the world, just log a warning
                Logger.Warn("Failed to calculate location for conductor text: " + ex.ToString());
            }

            // Return the final point
            return newPoint;
        }

        private void UpdateDuctNumbers(IObject anno)
        {
            try
            {
                IGroupElement groupElement = (IGroupElement)((IAnnotationFeature2)anno).Annotation;
                List<string> currentAnnoLines;
                int textElemIdx = groupElement.ElementCount;

                for (int i = 0; i < groupElement.ElementCount && textElemIdx > groupElement.ElementCount - 1; i++)
                {
                    if ((groupElement.Element[i] is ITextElement) &&
                        ((ITextElement)groupElement.Element[i]).Text.Contains(":"))
                    {
                        textElemIdx = i;
                    }
                }

                if (textElemIdx < groupElement.ElementCount)
                {
                    // Get the text element
                    ITextElement data = (ITextElement)groupElement.Element[textElemIdx];

                    // This only applies if there is more than one line
                    if (data.Text.Contains('\r') == true)
                    {
                        // Split the lines
                        currentAnnoLines = new List<string>(data.Text.Substring(0, data.Text.LastIndexOf('\r')).Split('\n'));
                        if (data.Text.EndsWith("\r\n") == false)
                        {
                            currentAnnoLines[currentAnnoLines.Count - 1] = currentAnnoLines[currentAnnoLines.Count - 1] + "\r";
                            currentAnnoLines.Add(data.Text.Substring(data.Text.LastIndexOf('\r') + 2));
                        }

                        // If any of them have a double digit duct number then we need to update
                        bool updateRequired = false;
                        foreach (string annoLine in currentAnnoLines)
                        {
                            if (annoLine.Substring(2, 1) == ":")
                            {
                                updateRequired = true;
                                break;
                            }
                        }

                        // Then add a space to each line where the duct number is less than 10 so it aligns with those above 10
                        if (updateRequired == true)
                        {
                            IList<string> newAnnoLines = new List<string>();
                            foreach (string annoLine in currentAnnoLines)
                            {
                                string newAnnoLine = annoLine;
                                if (annoLine.Substring(1, 1) == ":")
                                {
                                    newAnnoLine = "  " + newAnnoLine;
                                }
                                newAnnoLines.Add(newAnnoLine);
                            }

                            // Re-save the anno
                            ((ITextElement)groupElement.Element[textElemIdx]).Text = String.Join("\n", newAnnoLines.ToArray());
                            ((IAnnotationFeature2)anno).Annotation = groupElement as IElement;
                        }
                    }
                }
                else
                {
                    Logger.Warn("Could not find x-section conductor annotation in the cross section");
                }
            }
            catch (Exception ex)
            {
                Logger.Warn("Failed to update duct numbers: " + ex.ToString());
            }
        }

        #endregion

        #region Updating formatting and location methods

        private IElement UpdateText(IObject pObject, IElement element, string direction, bool updateTextSize, bool isHundredScale, bool updateHeaderText)
        {
            IGroupElement elements = (IGroupElement)element;

            // Now lets build a new set of elements based on the existing ones
            IGroupElement newGroup = new GroupElementClass();

            // For each element...
            int count = elements.ElementCount;
            for (int index = 0; index < count; index++)
            {
                string newText = String.Empty;

                if (updateHeaderText == true)
                {
                    if (index == count - 1)
                    {
                        newText = direction;
                        if (_sizes.Count > 1 || _materials.Count > 1)
                        {
                            string sizeText = string.Empty;
                            if (_sizes.Count > 1)
                            {
                                foreach (string size in _sizes)
                                {
                                    sizeText += size + "\" & ";
                                }
                                newText += " " + sizeText.Substring(0, sizeText.Length - 3);
                            }
                            string materialText = string.Empty;
                            if (_materials.Count > 1)
                            {
                                IDictionary<string, string> domainVals = new Dictionary<string, string>();
                                IWorkspaceDomains wsDomain = (pObject.Class as IDataset).Workspace as IWorkspaceDomains;
                                IDomain domain = wsDomain.DomainByName["ULS Material - Duct Bank"];
                                if (domain is ICodedValueDomain)
                                {
                                    ICodedValueDomain codedDomain = (ICodedValueDomain)domain;
                                    for (int i = 0; i < codedDomain.CodeCount; i++)
                                    {
                                        domainVals.Add(codedDomain.get_Value(i).ToString(), codedDomain.get_Name(i).ToString());
                                    }
                                }

                                foreach (string material in _materials)
                                {
                                    if (domainVals.ContainsKey(material) == true)
                                    {
                                        materialText += " " + domainVals[material] + " &";
                                    }
                                }
                                newText += materialText.Substring(0, materialText.Length - 2);
                            }
                            

                            ITextElement currText = elements.Element[index] as ITextElement;
                            if (currText.Text.Contains(":") == true)
                            {
                                //newText = newText.Substring(0, 10);
                                IElement headerElement = AddHeader(newGroup, newText);
                                headerElement = Update50or100ScaleElement(headerElement, 0, 0, newText, updateTextSize, isHundredScale);
                                newGroup.AddElement(headerElement);
                                newText = string.Empty;
                            }
                        }
                    }
                }

                //Update it with an offset and some other cosmetic changes
                IElement currentElement = Update50or100ScaleElement(elements.Element[index], 0, 0, newText,
                    updateTextSize, isHundredScale);

                // Add it to our new group
                newGroup.AddElement(currentElement);
            }

            return newGroup as IElement;
        }

        private IElement AddHeader(IGroupElement groupElement, string newText)
        {
            ITextElement elementToClone = null;
            ITextElement newElement = null;

            // No existing annotation, need to add a new element to host it
            Logger.Warn("Could not find an ITextElement in the cross section group element. Adding...");

            // The last element should be the header text
            if (groupElement.Element[groupElement.ElementCount - 1] is ITextElement)
            {
                // Clone that element
                elementToClone = groupElement.Element[groupElement.ElementCount - 1] as ITextElement;
                IClone clonedElement = (elementToClone as IClone).Clone();
                newElement = clonedElement as ITextElement;

                // Calculate and assign a new x, y location for the new element to be at
                IPoint startPoint = groupElement.Element[0].Geometry as IPoint;
                IPoint location = GetHeaderTextLocation(startPoint, groupElement, newElement.Symbol.Angle, newText);
                (newElement as IElement).Geometry = location as IGeometry;

                // Apply some cosmetics
                ITextSymbol symbol = newElement.Symbol;
                symbol.HorizontalAlignment = esriTextHorizontalAlignment.esriTHALeft;
                symbol.Font.Bold = false;
                newElement.Symbol = symbol;
                newElement.Text = string.Empty;
            }

            // Return the new element
            return newElement as IElement;
        }

        private IElement Update50or100ScaleElement(IElement currentElement, double xOffset, double yOffset, string newText,
            bool updateTextSize, bool isHundredScale)
        {
            // If its a point
            if (currentElement.Geometry.GeometryType == esriGeometryType.esriGeometryPoint)
            {

                // Build and set the location for this element
                IPoint elementLocation = new PointClass();
                elementLocation.X = currentElement.Geometry.Envelope.UpperLeft.X + xOffset;
                elementLocation.Y = currentElement.Geometry.Envelope.UpperLeft.Y + yOffset;
                currentElement.Geometry = elementLocation;

                // Get the text for the element, if applicable
                ITextElement text = currentElement as ITextElement;
                if (text != null)
                {
                    // And double its size since we shrank it when it got cloned
                    if (updateTextSize)
                    {
                        IFormattedTextSymbol ts = (IFormattedTextSymbol)text.Symbol;
                        if (ts.Size < 4)
                        {
                            ts.Size = ts.Size * 2;
                            ts.Font.Bold = true;
                            text.Symbol = ts;
                        }
                    }

                    // Prepend any new text
                    if (newText != String.Empty)
                    {
                        if (_materials.Count > 1 && _sizes.Count < 2)
                        {
                            if (newText.Length > text.Text.Length || text.Text.Substring(0, newText.Length) != newText)
                            {
                                int sep = newText.IndexOf(' ');
                                if (sep > 0)
                                {
                                    string direction = newText.Substring(0, sep);
                                    string materials = newText.Substring(sep + 1);
                                    text.Text = direction + " " + text.Text + materials;
                                }
                                else
                                {
                                    text.Text = newText + " " + text.Text;
                                }
                            }
                        }
                        else
                        {
                            if (newText.Length > text.Text.Length || text.Text.Substring(0, newText.Length) != newText)
                            {                                
                                text.Text = newText + " " + text.Text.Trim();
                            }
                        }
                    }

                    if (text.Text.ToUpper().Contains("UNKNOWN"))
                    {
                        text.Text = text.Text.Replace(" Unknown", string.Empty);
                    }
                }

            }
            // If its a polygon...
            else if (currentElement.Geometry.GeometryType == esriGeometryType.esriGeometryPolygon)
            {

                // Get the arc
                IPolygon elementPolygon = (IPolygon)currentElement.Geometry;
                ISegmentCollection elementSegments = (ISegmentCollection)elementPolygon;
                ICircularArc arc = (ICircularArc)elementSegments.Segment[0];

                if (arc != null)
                {

                    // Determine the new location
                    IPoint newPoint = new PointClass();
                    newPoint.X = elementPolygon.Envelope.UpperLeft.X + xOffset + arc.Radius;
                    newPoint.Y = elementPolygon.Envelope.UpperLeft.Y + yOffset - arc.Radius;

                    // Get a point on the other side of the arc
                    IPoint pRadiusPoint = new PointClass();
                    pRadiusPoint.X = newPoint.X + arc.Radius;
                    pRadiusPoint.Y = newPoint.Y;

                    if (!isHundredScale)
                    {
                        // Update and replace the segment in the segment collection
                        arc.PutCoords(newPoint, pRadiusPoint, pRadiusPoint, esriArcOrientation.esriArcClockwise);
                        elementSegments.RemoveSegments(0, 1, true);
                        elementSegments.AddSegment(arc as ISegment);
                    }

                    elementPolygon = elementSegments as IPolygon;
                    currentElement.Geometry = elementPolygon;

                }
                else
                {
                    Logger.Warn("Square encountered?");
                }
            }

            return currentElement;
        }

        private static IElement FormatAndFilter10ScaleGroupElement(IFeature annoFt, IElement element)
        {
            IGroupElement elements = (IGroupElement)element;

            // Now lets build a new set of elements based on the existing ones
            IGroupElement newGroup = new GroupElementClass();

            // For each element...
            int count = elements.ElementCount;
            for (int index = 0; index < count; index++)
            {
                // If its a text element
                if (!(elements.Element[index] is ITextElement)) continue;

                // And has more than two characters (ie: its not duct text)
                ITextElement text = (ITextElement)elements.Element[index];
                if (!text.Text.Contains(':')) continue;

                if (text != null)
                {
                    IFormattedTextSymbol ts = (IFormattedTextSymbol)text.Symbol;
                    IFormattedTextSymbol oldSymbol = (IFormattedTextSymbol)((ITextElement)((IGroupElement)((IAnnotationFeature2)annoFt).Annotation).Element[0]).Symbol;
                    ts.Angle = oldSymbol.Angle;
                    ts.Size = oldSymbol.Size;
                    ts.Font.Bold = oldSymbol.Font.Bold;
                    text.Symbol = ts;
                }

                // And add it to our new element
                newGroup.AddElement(elements.Element[index]);
            }

            // Return the result
            return newGroup as IElement;
        }

        #endregion

        #region Support methods for updating the text

        private string GetDirection(IRow feature)
        {
            string dir = String.Empty;

            // Get the parent features direction value
            if (feature != null)
            {
                object direction = UfmHelper.GetFieldValue(feature, SchemaInfo.UFM.FieldModelNames.Direction);
                if (direction != null)
                {
                    dir = direction.ToString();
                }
            }

            return dir;
        }

        private int GetParentConduitOid(IObject pObject)
        {
            int oid = 0;

            try
            {
                // Get the value of the feature ID field which is the parent conduit
                IField conduitFeatureIdField = ModelNameManager.FieldFromModelName(pObject.Class,
                    SchemaInfo.UFM.FieldModelNames.FeatureId);
                object idValue = pObject.Value[pObject.Fields.FindFieldByAliasName(conduitFeatureIdField.AliasName)];
                if (idValue != null)
                {
                    oid = (int)idValue;
                }
            }
            catch (Exception ex)
            {
                Logger.Warn("Failed to get parent conduit OID: " + ex);
            }

            return oid;
        }

        private int GetConductorTextElementIndex(IGroupElement groupElement)
        {
            int textElemIdx = 0;

            for (int i = 0; i < groupElement.ElementCount; i++)
            {
                if (groupElement.Element[i] is ITextElement) 
                {
                    if (((ITextElement)groupElement.Element[i]).Text.Contains(":") == true)
                    {
                        textElemIdx = i;
                        break;
                    }
                }
            }

            return textElemIdx;
        }

        private IList<string> GetConductorAnnoLines(ITextElement conductorAnnoElement)
        {
            IList<string> conductorAnnoLines = null;

            string conductorText = conductorAnnoElement.Text;
            if (conductorText.EndsWith("\r\n") == false)
            {
                conductorText += "\r\n";
            }
            conductorAnnoLines = new List<string>(conductorAnnoElement.Text.Substring(0, conductorText.LastIndexOf('\r')).Split('\n'));
            
            return conductorAnnoLines;
        }

        private double GetMultiplier(string ductPhase, string allPhases)
        {
            double multiplier = 1;

            // Surely some sort of bitwise op would do this easier but out of time and brain hurting
            if (ductPhase == "1" || ductPhase == "2" || ductPhase == "4")
            {
                if (allPhases == "7") multiplier = 0.33;
                if (allPhases == "3" || allPhases == "5" || allPhases == "6") multiplier = 0.5;
            }
            else if (ductPhase == "3" || ductPhase == "5" || ductPhase == "6")
            {
                if (allPhases == "7") multiplier = 0.66;
            }

            return multiplier;
        }

        private IFeature GetConductor(IFeature conduit, int oid)
        {
            IFeature conductor = null;

            // Check for these guys
            IList<string> conductorMn = new List<string>();
            conductorMn.Add(SchemaInfo.Electric.ClassModelNames.PrimaryUGConductor);
            conductorMn.Add(SchemaInfo.Electric.ClassModelNames.SecondaryUGConductor);
            conductorMn.Add(SchemaInfo.UFM.ClassModelNames.DcConductor);

            // Look for each
            foreach (string modelName in conductorMn)
            {
                try
                {
                    IFeatureClass conductorFc = ModelNameFacade.FeatureClassByModelName((conduit.Class as IDataset).Workspace, modelName);
                    conductor = conductorFc.GetFeature(oid);
                    break;
                }
                catch (Exception)
                {
                    Logger.Debug("Conductor does not have contain model name: " + modelName);
                }
            }

            // And return when we find one
            return conductor;
        }

        private string GetClassModelName(IFeature conductor)
        {
            string modelName = string.Empty;

            if (ModelNameFacade.ContainsClassModelName(conductor.Class as IObjectClass, SchemaInfo.Electric.ClassModelNames.PrimaryUGConductor))
            {
                modelName = SchemaInfo.Electric.ClassModelNames.PrimaryUGConductor;
            }
            else if (ModelNameFacade.ContainsClassModelName(conductor.Class as IObjectClass, SchemaInfo.Electric.ClassModelNames.SecondaryUGConductor))
            {
                modelName = SchemaInfo.Electric.ClassModelNames.SecondaryUGConductor;
            }
            else if (ModelNameFacade.ContainsClassModelName(conductor.Class, SchemaInfo.UFM.ClassModelNames.DcConductor))
            {
                modelName = SchemaInfo.UFM.ClassModelNames.DcConductor;
            }

            return modelName;
        }

        private string GetInfoModelName(IFeature conductor)
        {
            string modelName = string.Empty;

            if (ModelNameFacade.ContainsClassModelName(conductor.Class as IObjectClass, SchemaInfo.Electric.ClassModelNames.PrimaryUGConductor))
            {
                modelName = SchemaInfo.Electric.ClassModelNames.PrimaryUGConductorInfo;
            }
            else if (ModelNameFacade.ContainsClassModelName(conductor.Class as IObjectClass, SchemaInfo.Electric.ClassModelNames.SecondaryUGConductor))
            {
                modelName = SchemaInfo.Electric.ClassModelNames.SecondaryUGConductorInfo;
            }
            else if (ModelNameFacade.ContainsClassModelName(conductor.Class, SchemaInfo.UFM.ClassModelNames.DcConductor))
            {
                modelName = "PGE_DCCONDUCTORINFO";
            }

            return modelName;
        }

        private string GetAttributedValue(IRow relationObj, string field)
        {
            string value = string.Empty;

            // Relationship classes don't support model names so we use good ol' field names
            object valueObj = relationObj.get_Value(relationObj.Fields.FindField(field));
            if (valueObj != null && !(valueObj is DBNull))
            {
                value = valueObj.ToString();
            }

            // Return the result
            return value;
        }

        #endregion

        #endregion
    }
}