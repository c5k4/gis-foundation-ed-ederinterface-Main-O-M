using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Miner.ComCategories;
using System.Runtime.InteropServices;
using Miner.Framework.Autoupdaters;
using PGE.Common.Delivery.ArcFM;
using Miner.Interop;
using ESRI.ArcGIS.Geodatabase;
using Miner.Geodatabase;
using PGE.Common.Delivery.Framework;
using ESRI.ArcGIS.esriSystem;

namespace PGE.Desktop.EDER.AutoUpdaters.LabelText
{
    [Guid("455F05F1-4BF2-4864-B0A2-415222C895CA")]
    [ProgId("PGE.Desktop.EDER.PriUGConduitLabelTextAU")]
    [ComponentCategory(ComCategory.RelationshipAutoupdateStrategy)]
    public class RelLabelTextSymbolNumCombo : BaseRelationshipAU
    {
        IMMRelationshipAUStrategy symbolNumberAU = new SymbolNumberUpdateParentObject();
        IMMRelationshipAUStrategy relationshipLabelAU = new RelationshipLabel();

        public static IList<string> nestedCallTracker = new List<string>(); 

        public RelLabelTextSymbolNumCombo()
            : base("PGE LabelText Symbol Number Combo")
        {

        }

        protected override bool InternalEnabled(IRelationshipClass relClass, IObjectClass originClass, IObjectClass destClass, mmEditEvent eEvent)
        {
           bool relationshipAUEnabled = ((ModelNameManager.Instance.ContainsClassModelName(relClass.OriginClass, SchemaInfo.Electric.ClassModelNames.PGEPriUGConductor))
                || (ModelNameManager.Instance.ContainsClassModelName(relClass.DestinationClass, SchemaInfo.Electric.ClassModelNames.PGEPriUGConductor)) ||
                (ModelNameManager.Instance.ContainsClassModelName(relClass.OriginClass, SchemaInfo.Electric.ClassModelNames.PGEConduitSystem)));

            bool symbolNumAUEnabled = symbolNumberAU.get_Enabled(relClass, eEvent);
            return (symbolNumAUEnabled || relationshipAUEnabled);
        }

        protected override void InternalExecute(IRelationship relationship, mmAutoUpdaterMode auMode, mmEditEvent eEvent)
        {
            //Execute relationship AU
            const string SUBTYPE_DUCTBANK = "1";

            string trackerItem = relationship.OriginObject.OID.ToString() + "-" + relationship.DestinationObject.OID.ToString();

            if (nestedCallTracker.Contains(trackerItem) == false)
            {
                try
                {
                    nestedCallTracker.Add(trackerItem);

                    if ((ModelNameManager.Instance.ContainsClassModelName(relationship.RelationshipClass.OriginClass, SchemaInfo.Electric.ClassModelNames.PGEPriUGConductor))
                        || (ModelNameManager.Instance.ContainsClassModelName(relationship.RelationshipClass.DestinationClass, SchemaInfo.Electric.ClassModelNames.PGEPriUGConductor))
                        || (ModelNameManager.Instance.ContainsClassModelName(relationship.RelationshipClass.DestinationClass, SchemaInfo.Electric.ClassModelNames.SecondaryUGConductor))
                        || (ModelNameManager.Instance.ContainsClassModelName(relationship.RelationshipClass.DestinationClass, SchemaInfo.Electric.ClassModelNames.SecondaryUGConductor)))
                    {
                        try
                        {
                            int conduitOIDBeingUnrelated = -1;
                            if (eEvent == mmEditEvent.mmEventRelationshipDeleted)
                            {
                                ConduitLabel.UGRelClassBeingUnrelated = relationship.RelationshipClass;
                                if ((ModelNameManager.Instance.ContainsClassModelName(relationship.RelationshipClass.OriginClass, SchemaInfo.Electric.ClassModelNames.PGEConduitSystem)))
                                {
                                    conduitOIDBeingUnrelated = relationship.OriginObject.OID;
                                    ConduitLabel.UGObjectBeingUnrelated = relationship.DestinationObject.OID;
                                    ConduitLabel.UGObjectFCIDBeingUnrelated = relationship.DestinationObject.Class.ObjectClassID;
                                }
                                else
                                {
                                    conduitOIDBeingUnrelated = relationship.DestinationObject.OID;
                                    ConduitLabel.UGObjectBeingUnrelated = relationship.OriginObject.OID;
                                    ConduitLabel.UGObjectFCIDBeingUnrelated = relationship.OriginObject.Class.ObjectClassID;
                                }
                            }

                            PriUGConductorLabel.ConduitBeingUnrelated = conduitOIDBeingUnrelated;
                            SecUGConductorLabel.ConduitBeingUnrelated = conduitOIDBeingUnrelated;
                            relationshipLabelAU.Execute(relationship, auMode, eEvent);
                            //TODO: modify BaseLabelTextAU to do .store()?
                            if (eEvent == mmEditEvent.mmEventRelationshipCreated)
                            {
                                // Get the subtype
                                IFeature conduit = relationship.OriginObject as IFeature;
                                string subtype = conduit.get_Value(conduit.Fields.FindField("SUBTYPECD")).ToString();

                                if (subtype != SUBTYPE_DUCTBANK)
                                {
                                    try
                                    {
                                        BaseRelationshipAU.IsRelAUCallingStore = true;

                                        // If we found it, get the related objects
                                        ISet relatedObjects = relationship.RelationshipClass.GetObjectsRelatedToObject(relationship.OriginObject);
                                        if (relatedObjects.Count == 1)
                                        {
                                            relationship.DestinationObject.Store();
                                            relationship.OriginObject.Store();
                                        }
                                    }
                                    finally
                                    {
                                        BaseRelationshipAU.IsRelAUCallingStore = false;
                                    }
                                }
                            }
                        }
                        finally
                        {
                            PriUGConductorLabel.ConduitBeingUnrelated = -1;
                            SecUGConductorLabel.ConduitBeingUnrelated = -1;
                            ConduitLabel.UGObjectBeingUnrelated = -1;
                            ConduitLabel.UGObjectFCIDBeingUnrelated = -1;
                            ConduitLabel.UGRelClassBeingUnrelated = null;
                        }

                        //Execute symbol number AU
                        if (symbolNumberAU.get_Enabled(relationship.RelationshipClass, eEvent))
                        {
                            symbolNumberAU.Execute(relationship, auMode, eEvent);
                        }
                    }

                    // Call the UFM AUs
                    if (auMode != mmAutoUpdaterMode.mmAUMFeederManager &&
                        ModelNameManager.Instance.ContainsClassModelName(relationship.RelationshipClass.OriginClass, SchemaInfo.Electric.ClassModelNames.PGEConduitSystem))
                    {
                        try
                        {
                            if (eEvent == mmEditEvent.mmEventRelationshipDeleted)
                            {
                                ConduitLabel.UGObjectBeingUnrelated = relationship.DestinationObject.OID;
                                ConduitLabel.UGObjectFCIDBeingUnrelated = relationship.DestinationObject.Class.ObjectClassID;
                                ConduitLabel.UGRelClassBeingUnrelated = relationship.RelationshipClass;
                            }

                            CompositeConduitToConductor compositeConduitToConductor = new CompositeConduitToConductor();
                            compositeConduitToConductor.Execute(relationship, auMode, eEvent);

                            if (eEvent != mmEditEvent.mmEventRelationshipDeleted)
                            {
                                IMMSpecialAUStrategy csAnnoAU = Activator.CreateInstance(Type.GetTypeFromProgID("mmUlsAUs.MMUpdateCrossSection")) as IMMSpecialAUStrategy;
                                csAnnoAU.Execute(relationship.OriginObject);
                            }
                        }
                        finally
                        {
                            ConduitLabel.UGObjectBeingUnrelated = -1;
                            ConduitLabel.UGObjectFCIDBeingUnrelated = -1;
                            ConduitLabel.UGRelClassBeingUnrelated = null;
                        }
                    }
                }
                finally
                {
                    nestedCallTracker.Remove(trackerItem);
                }
            }
        }

        protected override bool IsCombo
        {
            get
            {
                return true;
            }
        }

    }
}
