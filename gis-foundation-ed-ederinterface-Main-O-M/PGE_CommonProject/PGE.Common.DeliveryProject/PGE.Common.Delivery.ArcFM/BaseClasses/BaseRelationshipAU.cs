using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Reflection;

using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;

using Miner.Interop;
using Miner.Interop.Process;

using PGE.Common.Delivery.Diagnostics;
using System.Windows.Forms;

namespace PGE.Common.Delivery.ArcFM
{

    /// <summary>
    /// Base class for Relationship Autoupdaters.
    /// </summary>
    abstract public class BaseRelationshipAU : IMMRelationshipAUStrategy, IMMRelationshipAUStrategyEx
    {
        private static Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "");

        #region Com Reg Methods

        /// <summary>
        /// Registers the specified reg key.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComRegisterFunction]
        static public void Register(string regKey)
        {
            Miner.ComCategories.RelationshipAutoupdateStrategy.Register(regKey);
        }
        /// <summary>
        /// Uns the register.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComUnregisterFunction]
        static public void UnRegister(string regKey)
        {
            Miner.ComCategories.RelationshipAutoupdateStrategy.Unregister(regKey);
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseRelationshipAU"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        protected BaseRelationshipAU(string name)
        {
            _Name = name;
        }

        #endregion

        #region Protected Fields
        /// <summary>
        /// The name of the auto udpater.
        /// </summary>
        protected string _Name;

        #endregion

        #region IMMRelationshipAUStrategy Members

        /// <summary>
        /// Executes the specified relationship AU.
        /// </summary>
        /// <param name="pRelationship">The relationship.</param>
        /// <param name="eAUMode">The AU mode.</param>
        /// <param name="eEvent">The edit event.</param>
        void IMMRelationshipAUStrategy.Execute(IRelationship pRelationship, mmAutoUpdaterMode eAUMode, mmEditEvent eEvent)
        {
            DateTime startTime = DateTime.Now;
            DateTime endTime;

            try
            {
                _logger.Debug(DateTime.Now.ToLongTimeString() + " ==== BEGIN EXECUTE: " + this._Name);

                if (eAUMode == mmAutoUpdaterMode.mmAUMFeederManager) return;

                Logger.WriteLine(TraceEventType.Verbose, "Executing the " + _Name + " Relationship Auto Updater assigned on the "
                    + ((IDataset)pRelationship.RelationshipClass).Name + " relationship class.");

                _logger.Debug("Executing the " + _Name + " Relationship Auto Updater assigned on the "
                    + ((IDataset)pRelationship.RelationshipClass).Name + " relationship class.");

                InternalExecute(pRelationship, eAUMode, eEvent);

            }
            catch (COMException e)
            {
                // If the MM_E_CANCELEDIT error is thrown, let it out.
                if (e.ErrorCode == (int)mmErrorCodes.MM_E_CANCELEDIT)
                {
                    //ArcFM does not show messages when the Edit is performed from the Attribute Editor.
                    //showing a message box explicitly would address the issue of the user not being notified when there is a error.
                    //This would cause the Message box being shown twice when the edit is performed using ArcGIS tools.
                    if (eAUMode == mmAutoUpdaterMode.mmAUMArcMap && !IsCombo)
                    {
                        _logger.Error("Error Executing Autoupdater " + _Name, e);
                        MessageBox.Show(e.Message, "Autoupdater error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    _logger.Error("Error Executing Relationship Autoupdater: " + _Name, e);
                    IsRunningAsRelAU = false;
                    IsRelAUCallingStore = false;

                    throw e;
                }
            }
            catch (Exception e)
            {
                _logger.Error("Error Executing Relationship Autoupdater: " + _Name, e);
                IsRunningAsRelAU = false;
                IsRelAUCallingStore = false;
            }
            finally
            {
                _logger.Debug("FINISHED Executing the " + _Name + " Relationship Auto Updater assigned on the "
                    + ((IDataset)pRelationship.RelationshipClass).Name + " relationship class.");

                endTime = DateTime.Now;
                TimeSpan ts = endTime - startTime;
                _logger.Debug("TOTAL EXECUTION TIME ==== " + ts.TotalSeconds.ToString());

            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        string IMMRelationshipAUStrategy.Name
        {
            get { return _Name; }
        }

        /// <summary>
        /// Gets whether the specified AU is enabled.
        /// </summary>
        /// <param name="pRelClass">The relationship class.</param>
        /// <param name="eEvent">The edit event.</param>
        /// <returns><c>true</c> if this AutoUpdater is enabled; otherwise <c>false</c></returns>
        bool IMMRelationshipAUStrategy.get_Enabled(IRelationshipClass pRelClass, mmEditEvent eEvent)
        {
            try
            {
                return InternalEnabled(pRelClass, pRelClass.OriginClass, pRelClass.DestinationClass, eEvent);
            }
            catch (Exception e)
            {
                _logger.Error("Error Getting Enabled Status For Relationship AU: " + _Name, e);
                return false;
            }
        }

        #endregion

        #region IMMRelationshipAUStrategyEx Members

        /// <summary>
        /// Gets whether the specified AU is enabled.
        /// </summary>
        /// <param name="pRelationshipClass">The relationship class.</param>
        /// <param name="pOriginClass">The origin class.</param>
        /// <param name="pDestClass">The destination class.</param>
        /// <param name="eEvent">The edit event.</param>
        /// <returns><c>true</c> if this AutoUpdater is enabled; otherwise <c>false</c></returns>
        bool IMMRelationshipAUStrategyEx.get_Enabled(IRelationshipClass pRelationshipClass, IObjectClass pOriginClass, IObjectClass pDestClass, mmEditEvent eEvent)
        {
            try
            {
                return InternalEnabled(pRelationshipClass, pOriginClass, pDestClass, eEvent);
            }
            catch (Exception e)
            {
                _logger.Error("Error Getting Enabled Status For Relationship AU: " + _Name, e);
                return false;
            }
        }

        #endregion

        #region Protected Members

        /// <summary>
        /// Internal Execute event for the given relationship
        /// </summary>
        /// <param name="relationship">The relationship.</param>
        /// <param name="auMode">The au mode.</param>
        /// <param name="eEvent">The edit event.</param>
        /// <remarks>This method will be called from IMMRelationshipAUStrategy::Execute
        /// and is wrapped within the exception handling for that method.</remarks>
        virtual protected void InternalExecute(IRelationship relationship, mmAutoUpdaterMode auMode, mmEditEvent eEvent)
        {

        }

        /// <summary>
        /// Internal Enabled event for the given relationship.
        /// </summary>
        /// <param name="relClass">The relelationship class.</param>
        /// <param name="originClass">The origin class.</param>
        /// <param name="destClass">The destination class.</param>
        /// <param name="eEvent">The edit event.</param>
        /// <returns><c>true</c> if this AutoUpdater is enabled; otherwise <c>false</c></returns>
        /// <remarks>This method will be called from IMMRelationshipAUStrategyEx::get_Enabled
        /// and is wrapped within the exception handling for that method.</remarks>
        virtual protected bool InternalEnabled(IRelationshipClass relClass, IObjectClass originClass, IObjectClass destClass, mmEditEvent eEvent)
        {
            return true;
        }

        /// <summary>
        /// Override only for ComboRelationship AUs
        /// Set this to True if the Implementing class is a ComboRelationship AU
        /// </summary>
        virtual protected bool IsCombo
        {
            get
            {
                return false;
            }
        }

        #endregion

        #region Public Static Memebers
        /// <summary>
        /// 
        /// </summary>
        public static bool IsRunningAsRelAU { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public static bool IsRelAUCallingStore { get; set; }
        #endregion
    }
}
