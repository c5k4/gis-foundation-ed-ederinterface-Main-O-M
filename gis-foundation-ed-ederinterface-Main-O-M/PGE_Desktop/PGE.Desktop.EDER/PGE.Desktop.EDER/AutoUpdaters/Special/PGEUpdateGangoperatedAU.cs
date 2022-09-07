/// This Autoupdaters is meet the requirement of EGIS-812
/// Function: Autoupdater will update the value of the GANGOPERATED attribute to “N” when DeviceType IN (‘FS’, ‘TS’), and “Y” for all other DeviceTypes in Recloser DPD
/// Developed By: Arvind Sinha
/// Date: 12 Feb 2021
/// JIRA : EGIS-812
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
    [Guid("562F67B3-9691-4F70-8ED0-FDD3FBB21149")]
    [ProgId("PGE.Desktop.EDER.AutoUpdaters.Special.PGEUpdateGangoperatedAU")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    [ComVisible(true)]
    public class PGEUpdateGangoperatedAU : BaseSpecialAU
    {
        private IFeatureClass m_DPDFeatureClass = null;
        private const string _GangoperatedFieldName = "GANGOPERATED";
        private const string _DeviceTypeFieldName = "DEVICETYPE";
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PGEUpdateGangoperatedAU"/> class.  
        /// </summary>
        public PGEUpdateGangoperatedAU() : base("PGE Update Gangoperated AU") { }
        #endregion Constructors

        #region ClassVariables

        private static readonly PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        #endregion ClassVariables

        #region Special AU Overrides
        /// <summary>
        /// Enable Autoupdater in case of onCreate/onUpdate event and DynamicProtectiveDevice Feature Class
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
            int m_DeviceTypeFldIndex = -1;
            int m_GangopratedFldIndex = -1;            
            try
            {
                //AU will run only on FeatureUpdate/ on FeatureDelete events
                if (eEvent == mmEditEvent.mmEventFeatureUpdate || eEvent == mmEditEvent.mmEventFeatureCreate)
                {
                    //Dynamic Protective Device feature class
                    if (ModelNameFacade.ContainsClassModelName(pObject.Class, SchemaInfo.Electric.ClassModelNames.DynamicProtectiveDevice))
                    {
                        m_DPDFeatureClass = pObject.Class as IFeatureClass;
                        m_GangopratedFldIndex = m_DPDFeatureClass.FindField(_GangoperatedFieldName);
                        m_DeviceTypeFldIndex = m_DPDFeatureClass.FindField(_DeviceTypeFieldName);
                        string subtypecd = pObject.get_Value(pObject.Fields.FindField("SUBTYPECD")).ToString();
                        if (subtypecd.Equals("3"))//For Recloser Subtype
                        {
                            string deviceType = pObject.get_Value(m_DeviceTypeFldIndex).ToString();
                            if (deviceType.Equals("FS") || deviceType.Equals("TS")) // if Device Type is FuseSaver (FS) and TripSaver (TS)
                            {
                                pObject.set_Value(m_GangopratedFldIndex, "N");
                            }
                            else //For All other Device Type
                            {
                                pObject.set_Value(m_GangopratedFldIndex, "Y");
                            }
                        }
                        else //For Sectionalizer and Interrupter
                        {
                            pObject.set_Value(m_GangopratedFldIndex, "Y");
                        }
                    }
                }
            }
            catch(Exception ex)
            { _logger.Warn(ex.Message); 
            }
        }

        #endregion
    }
}
