using System;
using System.Reflection;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geodatabase;
using Miner.ComCategories;
using Miner.Geodatabase;
using Miner.Interop;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Framework;
using System.Windows.Forms;

namespace PGE.Desktop.EDER.AutoUpdaters.Relationship
{
    /// <summary>
    /// A relationship AU that counts related objects.
    /// </summary>
    [ComVisible(true)]
    [Guid("3E24379A-CCA0-488A-BF2D-D8C57CD9980B")]
    [ProgId("PGE.Desktop.EDER.PGEAttachedCustomerWarningRelAU")]
    [ComponentCategory(ComCategory.RelationshipAutoupdateStrategy)]
    public class PGEAttachedCustomerWarningRelAU : BaseRelationshipAU
    {
        #region private
        private static readonly PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        public static string _message = "You are about to relate customers to an Equipment or Streetlight transformer.  Do you wish to continue?";
        //Changes for ENOS to SAP migration
        IMMRelationshipAUStrategy _relationGenCategoryAU = new PGEUpdateGenCategoryRelAU() as IMMRelationshipAUStrategy;
        #endregion private

         #region Constructors
        /// <summary>
        /// constructor for the Populate Conductor Phase Rel AU
        /// </summary>
        public PGEAttachedCustomerWarningRelAU() : base("PGE Attached Customer Warning")
        {
        }

        #endregion Constructors
        #region Relationship AU overrides
        /// <summary>
        /// update the sum of count of objects
        /// </summary>
        /// <param name="relationship">the relationship</param>
        /// <param name="auMode">the mode</param>
        /// <param name="eEvent">the event</param>
        protected override void InternalExecute(IRelationship relationship, mmAutoUpdaterMode auMode, mmEditEvent eEvent)
        {
            if (eEvent == mmEditEvent.mmEventRelationshipCreated)
            {
                //Parent object row
                IObject origObject = relationship.OriginObject;
               
                //check for not null
                if (origObject == null)
                {
                    //log the warning and return the control 
                    _logger.Warn("OriginObject is <Null>, exiting.");
                    return;
                }
                if (origObject.HasSubtypeCode(4) || origObject.HasSubtypeCode(7))
                {
                    if (MessageBox.Show(_message, "Warning Message", System.Windows.Forms.MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.Cancel)
                    {
                        throw new COMException("Cancelling the edits.", (int)mmErrorCodes.MM_E_CANCELEDIT);
                    }
                }


                //Workaround for bug#25625
                int parentCGCFieldIdx = GetFieldIxFromModelName(origObject.Class, SchemaInfo.Electric.FieldModelNames.FieldModelCGC12);
                if (parentCGCFieldIdx != -1)
                {

                    string cgc12 = origObject.get_Value(parentCGCFieldIdx).ToString();

                    IObject destObject = relationship.DestinationObject;
                    int fieldIndex = GetFieldIxFromModelName(destObject.Class, SchemaInfo.Electric.FieldModelNames.FieldModelCGC12);
                    if (fieldIndex != -1)
                    {
                        destObject.set_Value(fieldIndex, cgc12);
                    }
                }

                //Changes for ENOS to SAP migration, Execute Update GenCategory Rel AU.
                try
                {
                    BaseRelationshipAU.IsRunningAsRelAU = true;
                    _relationGenCategoryAU.Execute(relationship, auMode, eEvent);

                    _logger.Debug("Executed UpdateGenCategoryAU from PGEAttachedCustomerWarningAU.");
                }
                finally
                {
                    BaseRelationshipAU.IsRunningAsRelAU = false;
                }

            }
        }



        /// <summary>
        /// The model name "CONDUCTORINFO" must be on the relationship dest in order to be enabled
        /// </summary>
        /// <param name="relClass">the relationship class</param>
        /// <param name="originClass">the relationship origin</param>
        /// <param name="destClass">the relationship destination</param>
        /// <param name="eEvent">the relationship event</param>
        /// <returns></returns>
        protected override bool InternalEnabled(IRelationshipClass relClass, IObjectClass originClass, IObjectClass destClass, mmEditEvent eEvent)
        {
            bool destClassEnabled = ModelNameFacade.ContainsClassModelName(destClass, new string[] { SchemaInfo.Electric.ClassModelNames.ServicePoint });
            bool originClassEnabled = ModelNameFacade.ContainsClassModelName(originClass, new string[] { SchemaInfo.Electric.ClassModelNames.PGETransformer });
            _logger.Debug(string.Format("ClassModelName : {0} exist({2}) on ObjectClass{1}", SchemaInfo.Electric.ClassModelNames.ConductorInfo, destClass.AliasName, destClassEnabled));
            _logger.Debug(string.Format("ClassModelName : {0} exist({2}) on ObjectClass{1}", SchemaInfo.Electric.ClassModelNames.Conductor, originClass.AliasName, originClassEnabled));
            _logger.Debug(string.Format("Returning Visible : {0}", (destClassEnabled && originClassEnabled)));
            return (destClassEnabled && originClassEnabled);
        }

        #endregion Relationship AU overrides


        private int GetFieldIxFromModelName(IObjectClass table, string fieldModelName)
        {
            int fieldIndex = -1;
            try
            {
                IField field = ModelNameManager.Instance.FieldFromModelName(table, fieldModelName);
                if (field != null)
                {
                    fieldIndex = table.FindField(field.Name);
                }
            }
            catch
            {
                fieldIndex = -1;
            }

            return fieldIndex;
        }
    }
}
