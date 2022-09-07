using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geodatabase;
using log4net;
using Miner.ComCategories;
using Miner.Interop;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Framework;
using PGE.Common.Delivery.Diagnostics;

namespace PGE.Desktop.EDER.AutoUpdaters.Special.ADMSLabelAU
{
    /// <summary>
    /// Reads XML cofiguration documents from the database and selects a symbol number for a updated object by evaluating the rules defined within.
    /// </summary>
    [Guid("3EE45F65-8960-45EE-ADF5-4D4A7C1415B0")]
    [ProgId("PGE.Desktop.EDER.PGEADMSLabelAU")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    [ComVisible(true)]
    public class PGEADMSLabelAU : BaseSpecialAU
    {
        #region Class Variables
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        private static string _warningMsg = "Field Model name :{0},{1} not found.";
        #endregion
        #region Constructors
        /// <summary>
        /// Constructor, pass in AU name.
        /// </summary>
        public PGEADMSLabelAU()
            : base("PGE ADMS Label AU")
        {
        }

        #endregion

        #region Override
        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectClass"> Object's class. </param>
        /// <param name="eEvent">The edit event.</param>
        /// <returns> <c>true</c> if the AuoUpdater should be enabled; otherwise <c>false</c> </returns>
        protected override bool InternalEnabled(IObjectClass objectClass, mmEditEvent eEvent)
        {
            bool enabled = false;

            if (eEvent == mmEditEvent.mmEventFeatureCreate || eEvent == mmEditEvent.mmEventFeatureUpdate)
            {
                enabled = true; 
                _logger.Debug("Class model name: " + SchemaInfo.Electric.ClassModelNames.DeviceGroup + " Found -" + enabled);

            }

            return enabled;
        }

        /// <summary>
        /// Determines whether actually this AU should be run, based on the AU Mode.
        /// </summary>
        /// <param name="eAUMode"> The auto updater mode. </param>
        /// <returns> <c>true</c> if the AuoUpdater should be executed; otherwise <c>false</c> </returns>
        protected override bool CanExecute(mmAutoUpdaterMode eAUMode)
        {
            //Enable if Application type is ArcMap
            return eAUMode == mmAutoUpdaterMode.mmAUMArcMap;
        }

        /// <summary>
        /// Implementation of AutoUpdater Execute Ex method for derived classes.
        /// </summary>
        /// <param name="obj">The object that triggered the AutoUpdater.</param>
        /// <param name="eAUMode">The auto updater mode.</param>
        /// <param name="eEvent">The edit event.</param>
        protected override void InternalExecute(IObject obj, mmAutoUpdaterMode eAUMode, mmEditEvent eEvent)
        {
            // check if event is create or update
            if (eEvent == mmEditEvent.mmEventFeatureCreate || eEvent == mmEditEvent.mmEventFeatureUpdate)
            {
                UpdateADMSLabel(obj, eEvent);
            }
        }
        #endregion

        /// <summary>
        /// Executes when DeviceGroup is created/updated.
        /// </summary>
        /// <param name="pObject">Object being created/updated.</param>
        /// <param name="eEvent">Eventobject which clarifies that object is created/updated.</param>
        private void UpdateADMSLabel(IObject pObject, Miner.Interop.mmEditEvent eEvent)
        {
            if (eEvent == mmEditEvent.mmEventFeatureCreate || eEvent == mmEditEvent.mmEventFeatureUpdate)
            {
                _logger.Debug("Executing DeviceGroupADMSLabel.");
                DeviceGroupADMSLabel(pObject);
                _logger.Debug("Executed DeviceGroupADMSLabel.");
            }
        }

        /// <summary>
        /// Update DeviceGroup.ADMSLabel field based on facility type and subtype
        /// </summary>
        /// <param name="pObject"></param>
        private void DeviceGroupADMSLabel(IObject pObject)
        {
            if (pObject.HasSubtypeCode(2) || pObject.HasSubtypeCode(3))
            {
                // set subtype var
                int subtype = pObject.HasSubtypeCode(2) ? 2 : 3;

                _logger.Debug("Executing DeviceGroupADMSLabel on SubType: " + pObject.GetSubtypeDescription() + " -- " + subtype);
                int fldidx = pObject.Fields.FindField("DEVICEGROUPTYPE");
                // insure field exists
                if (fldidx > 0)
                {
                    int devicegrouptype;
                    // get the facility type/devicegrouptype number
                    if (int.TryParse(pObject.Value[fldidx].ToString(),out devicegrouptype))
                    {
                        ADMSLabelHelper admsLabelHelper = new ADMSLabelHelper();
                        KeyValuePair<int, string> match = admsLabelHelper.ADMSLabel.GetADMSLabel(subtype, devicegrouptype);
                        if (match.Key == 1)
                        {
                            // not a j-box feature let's populate based on list value (j-boxes have a status of 0 in the xml file)
                            fldidx = pObject.Fields.FindField("ADMSLabel");
                            if (fldidx > 0)
                            {
                                pObject.Value[fldidx] = match.Value;
                                _logger.Debug("Executing DeviceGroupADMSLabel - updated ADMSLabel field to " + match.Value);
                            }
                            else
                            {
                                _logger.Warn("Executing DeviceGroupADMSLabel - missing ADMSLabel field.");
                            }
                        }
                        else if (match.Key == 0)
                        {
                            // ADMSLabel needs to be populated with DeviceGroupname (for J-Box features)
                            fldidx = pObject.Fields.FindField("ADMSLabel");
                            // insure field exists
                            if (fldidx > 0)
                            {
                                int otheridx = pObject.Fields.FindField("DeviceGroupName");
                                // insure field exists
                                if (otheridx > 0)
                                {
                                    string othervalue = pObject.Value[otheridx].ToString();
                                    if (othervalue.Length > 0)
                                    {
                                        if (pObject.Value[fldidx].ToString() != othervalue)
                                        {
                                            pObject.Value[fldidx] = othervalue;
                                        }
                                        else
                                        {
                                            _logger.Debug("Executing DeviceGroupADMSLabel - values match: " + othervalue);
                                        }
                                    }
                                    else
                                    {
                                        _logger.Debug("Executing DeviceGroupADMSLabel - DeviceGroupName empty value.");
                                    }
                                }
                                else
                                {
                                    _logger.Warn("Executing DeviceGroupADMSLabel - Incorrect datatype for DEVICEGROUPTYPE field.");
                                }
                            }
                            else
                            {
                                _logger.Error("Executing DeviceGroupADMSLabel - missing ADMSLabel field.");
                            }

                        }
                        else
                        {
                            // status of something other then 0 or 1
                            _logger.Error("Executing DeviceGroupADMSLabel - ADMSLabelHelper return status > 1: " + match.Key + " : " + match.Value);
                        }
                    }
                    else
                    {
                        _logger.Warn("Executing DeviceGroupADMSLabel - Incorrect datatype for DEVICEGROUPTYPE field.");
                    }

                }
                else
                {
                    _logger.Warn("Executing DeviceGroupADMSLabel - missing DEVICEGROUPTYPE field.");
                }

            
            }
            else
            {
                // this is not the subtype you are looking for (probably subtype 1 which we aren't concerned with)...
                _logger.Debug("Executing DeviceGroupADMSLabel - subtype did not match: " + pObject.GetSubtypeDescription());

            }
        }
    }
}
