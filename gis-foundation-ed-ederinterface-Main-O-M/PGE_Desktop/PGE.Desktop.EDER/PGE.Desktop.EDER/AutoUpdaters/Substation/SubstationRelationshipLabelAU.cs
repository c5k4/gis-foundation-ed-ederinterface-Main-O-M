using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using Miner.ComCategories;
using Miner.Interop;
using PGE.Common.Delivery.Diagnostics;
using PGE.Desktop.EDER.AutoUpdaters.Substation.BaseClasses;
using PGE.Desktop.EDER.Utility;

namespace PGE.Desktop.EDER.AutoUpdaters.Substation
{
    [Guid("F37CC8DE-9B47-4A79-AC6A-20E175675ABD")]
    [ProgId("PGE.Substation.SubRelationshipLabelAU")]
    [ComponentCategory(ComCategory.RelationshipAutoupdateStrategy)]
    public class SubstationRelationshipLabelAU : IMMRelationshipAUStrategy
    {
        #region Static Members
        internal static KeyValuePair<int, int>? DeletedRelatedObject = null;
        #endregion

        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        public SubstationRelationshipLabelAU()
        {
            
        }

        public void Execute(IRelationship rel, mmAutoUpdaterMode auMode, mmEditEvent editEvent)
        {
            if (auMode == mmAutoUpdaterMode.mmAUMFeederManager)
                return;

            try
            {
                _logger.Debug("Starting " + Name + ".");

                // If an object is getting deleted, store information about it.
                if (editEvent == mmEditEvent.mmEventRelationshipDeleted)
                {
                    DeletedRelatedObject = new KeyValuePair<int, int>(rel.DestinationObject.Class.ObjectClassID, rel.DestinationObject.OID);
                    _logger.Debug("An object is being unrelated and will be excluded from label checks: " + rel.DestinationObject.Class.AliasName + " OID: " + rel.DestinationObject.OID);
                }
                else if (ModelNames.Manager.ContainsClassModelName(rel.DestinationObject.Class, ModelNames.ANNO) && editEvent == mmEditEvent.mmEventRelationshipCreated)
                {
                    //The anno is being created. Needed to overwrite the horizontal alignment.
                    GetLabelAUForClass(rel.OriginObject.Class).UpdateAnnoAlignment(rel.OriginObject, rel.DestinationObject);
                }
                else
                {
                    // Execute AU on parent object.
                    _logger.Debug("Executing AU on parent object.");
                    ExecuteLabelAU(rel.OriginObject, auMode, editEvent);
                }

                // Clear the value for the DeletedRelatedObject.
                DeletedRelatedObject = null;
            }
            catch (Exception e)
            {
                _logger.Error("Exception in Execute() for " + Name + ":" + e.ToString());
            }
            finally
            {
                _logger.Debug("Finished executing " + Name + ".");
            }
        }

        private void ExecuteLabelAU(IObject obj, mmAutoUpdaterMode auMode, mmEditEvent editEvent)
        {
            GetLabelAUForClass(obj.Class).Execute(obj, auMode, editEvent);
        }

        private SubstationLabelTextSpecialAU GetLabelAUForClass(IObjectClass oc)
        {
            bool fromParent = false;
            if (SubstationLabelTextSpecialAU.FindSubAUParentClass(ModelNames.SUBTRANSFORMERBANK, ref oc, out fromParent))
                return new SubTransformerBankLabelAU(true);
            else if (SubstationLabelTextSpecialAU.FindSubAUParentClass(ModelNames.VOLTAGEREGULATOR, ref oc, out fromParent))
                return new SubVoltageRegulatorLabelAU(true);
            else if (SubstationLabelTextSpecialAU.FindSubAUParentClass(ModelNames.SUBSTATIONTRANSFORMER, ref oc, out fromParent))
                return new SubstationTransformerLabelTextAU(true);

            return null;
        }

        public string Name
        {
            get { return "PGE Substation Relationship LabelText AU"; }
        }

        public bool get_Enabled(ESRI.ArcGIS.Geodatabase.IRelationshipClass relClass, mmEditEvent editEvent)
        {
            try
            {
                _logger.Debug("Checking Enabled for " + Name + " on relationship class " + (relClass as IDataset).Name + ".");

                //Ensure that model names are found by finding the parent.
                IObjectClass oc = relClass.OriginClass;

                if (GetLabelAUForClass(oc) == null) return false;

                _logger.Debug("Everything checks out.");
                return true;
            }
            catch (Exception e)
            {
                _logger.Error("Exception in Enabled() for " + Name + ":" + e.ToString());
                return false;
            }
            finally
            {
                _logger.Debug("Finished checking enabled for " + Name + ".");
            }
        }
    }
}
