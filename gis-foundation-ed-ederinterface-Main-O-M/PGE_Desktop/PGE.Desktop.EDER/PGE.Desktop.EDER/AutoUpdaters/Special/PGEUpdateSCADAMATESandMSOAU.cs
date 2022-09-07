/// This Autoupdaters is meet the requirement of EGIS-849
/// Function: Autoupdater will update the value in ATTACHMENTTYPE, CCRATING, INTERRUPTINGMEDIUM, MAXIINTERRUPTINGCURRENT for SCADAMATES switch type
/// Developed By: Arvind Sinha
/// Date: 12 Feb 2021
/// JIRA : EGIS-849
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using Miner.ComCategories;
using Miner.Interop;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Framework;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
namespace PGE.Desktop.EDER.AutoUpdaters.Special
{
    [Guid("ED2D8884-3AC0-4B04-8B9B-2D53BE9CE86E")]
    [ProgId("PGE.Desktop.EDER.AutoUpdaters.Special.PGEUpdateSCADAMATESandMSOAU")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    [ComVisible(true)]
    public class PGEUpdateSCADAMATESandMSOAU : BaseSpecialAU
    {
        private IFeatureClass m_SwitchFeatureClass = null;
        private int m_SwitchTypeFldIndex = -1;
        private const string _switchTpeFieldName = "SwitchType";
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PGEUpdateSCADAMATESandMSOAU"/> class.  
        /// </summary>
        public PGEUpdateSCADAMATESandMSOAU() : base("PGE Update SCADAMATE and MSO AU") { }
        #endregion Constructors

        #region ClassVariables

        private static readonly PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        #endregion ClassVariables

        #region Special AU Overrides
        /// <summary>
        /// Enable Autoupdater in case of onCreate/onUpdate event for Switch feature class.
        /// </summary>
        /// <param name="pObjectClass">The selected object class.</param>
        /// <param name="eEvent">The AU event type.</param>
        /// <returns></returns>
        protected override bool InternalEnabled(IObjectClass pObjectClass, Miner.Interop.mmEditEvent eEvent)
        {
            if ((eEvent == Miner.Interop.mmEditEvent.mmEventFeatureCreate) || (eEvent == Miner.Interop.mmEditEvent.mmEventFeatureUpdate))
            {
                return true;
            }

            return false;
        }

        protected override void InternalExecute(IObject pObject, Miner.Interop.mmAutoUpdaterMode eAUMode, Miner.Interop.mmEditEvent eEvent)
        {
            int indAttachmentType = -1;
            int indCCRating = -1;
            int indInterruptingMedium = -1;
            int indMaxInterruptingCurrent = -1;
            int indComplexDeviceIDC = -1;
            try
            {
                //AU will run only on FeatureUpdate/ on FeatureDelete events
                if (eEvent == mmEditEvent.mmEventFeatureUpdate || eEvent == mmEditEvent.mmEventFeatureCreate)
                {
                    //Switch feature class
                    if (ModelNameFacade.ContainsClassModelName(pObject.Class, SchemaInfo.Electric.ClassModelNames.Switch))
                    {
                        m_SwitchFeatureClass = pObject.Class as IFeatureClass;
                        m_SwitchTypeFldIndex = m_SwitchFeatureClass.FindField(_switchTpeFieldName);
                        indAttachmentType = m_SwitchFeatureClass.FindField(SchemaInfo.Electric.Attachementtype);
                        indCCRating = m_SwitchFeatureClass.FindField(SchemaInfo.Electric.Ccrating);
                        indInterruptingMedium = m_SwitchFeatureClass.FindField(SchemaInfo.Electric.InterruptingMedium);
                        indMaxInterruptingCurrent = m_SwitchFeatureClass.FindField(SchemaInfo.Electric.MaxInterruptingCurrent);
                        indComplexDeviceIDC = m_SwitchFeatureClass.FindField(SchemaInfo.Electric.ComplexDeviceIDC);
                        string switchtype = pObject.get_Value(m_SwitchTypeFldIndex).ToString();
                        string subtypecd = pObject.get_Value(pObject.Fields.FindField("SUBTYPECD")).ToString();
                        if (indAttachmentType != -1 && indCCRating != -1 && indMaxInterruptingCurrent != -1 && indInterruptingMedium != -1 && indComplexDeviceIDC != -1)
                        {
                            if (!string.IsNullOrEmpty(subtypecd) && subtypecd.Equals("2")) // Overhead Switch
                            {

                                if (!string.IsNullOrEmpty(switchtype) && switchtype.Equals("52"))//domain coded value 52: SF6 SCADAMATE
                                {
                                        pObject.set_Value(indAttachmentType, 7);
                                        pObject.set_Value(indCCRating, 900);
                                        pObject.set_Value(indMaxInterruptingCurrent, 900);
                                        pObject.set_Value(indInterruptingMedium, "SF6");
                                        pObject.set_Value(indComplexDeviceIDC, "Y");
                                    
                                }
                                else if (!string.IsNullOrEmpty(switchtype) && switchtype.Equals("53"))//domain coded value 53: SD SCADAMATE
                                {
                                    pObject.set_Value(indAttachmentType, 7);
                                    pObject.set_Value(indCCRating, 900);
                                    pObject.set_Value(indMaxInterruptingCurrent, 900);
                                    pObject.set_Value(indInterruptingMedium, "VAC");
                                    pObject.set_Value(indComplexDeviceIDC, "Y");

                                }
                                else if (!string.IsNullOrEmpty(switchtype) && switchtype.Equals("51"))//domain coded value 51: Motorized Switch Operator
                                {                                    
                                        pObject.set_Value(indAttachmentType, 10);
                                        pObject.set_Value(indCCRating, 900);
                                        pObject.set_Value(indMaxInterruptingCurrent, 1500);
                                        pObject.set_Value(indInterruptingMedium, "NA");                                    
                                }
                                /*EGIS-1091 : Removed AC5 as requested by user
                                 else
                                {         
                                    // null for other overhead switch type other than scadamate and mso                          
                                    pObject.set_Value(indAttachmentType, DBNull.Value);
                                    pObject.set_Value(indCCRating, DBNull.Value);
                                    pObject.set_Value(indMaxInterruptingCurrent, DBNull.Value);
                                    pObject.set_Value(indInterruptingMedium, DBNull.Value);                                    
                                }*/
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Warn(ex.Message);
            }
        }

        #endregion
    }
}
