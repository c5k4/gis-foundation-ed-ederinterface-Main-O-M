using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Miner.ComCategories;
using Miner.Interop.Process;
using PGE.Common.Delivery.Diagnostics;

namespace PGE.Common.Delivery.Process.BaseClasses
{
    /// <summary>
    /// The base deleter class used to delete custom data from workflow manager when a WorkRequest, Design, WorkLocation or CU is deleted.
    /// </summary>
    abstract public class BasePxDeleter : IMMPxDeleter, IMMPxDisplayName
    {
        /// <summary>
        /// The process framework application reference
        /// </summary>
        protected IMMPxApplication _PxApp;
        /// <summary>
        /// The name of the deleter.
        /// </summary>
        protected string _DisplayName;

        /// <summary>
        /// Initializes a new instance of the <see cref="BasePxDeleter"/> class.
        /// </summary>
        /// <param name="displayName">The display name </param>
        protected BasePxDeleter(string displayName)
        {
            _DisplayName = displayName;
        }

        #region Component Registration
        /// <summary>
        /// Registers the specified reg key.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComRegisterFunction, ComVisible(false)]
        static void Register(string regKey)
        {
            MMProcessMgrDeleter.Register(regKey);            
        }

        /// <summary>
        /// Unregisters the specified reg key.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComUnregisterFunction, ComVisible(false)]
        static void Unregister(string regKey)
        {
            MMProcessMgrDeleter.Unregister(regKey);
        }
        #endregion

        #region IMMPxDeleter Members

        /// <summary>
        /// Deletes the specified px node from the process framework database table. Any errors the occur will be logged to the event log using <see cref="EventLogger"/>.
        /// </summary>
        /// <param name="pPxNode">The px node.</param>
        /// <param name="sMsg">The message.</param>
        /// <param name="Status">The status.</param>
        public virtual void Delete(IMMPxNode pPxNode, ref string sMsg, ref int Status)
        {
            try
            {
                InternalDelete(pPxNode, ref sMsg, ref Status);
            }
            catch (Exception ex)
            {
                EventLogger.Error(ex);
            }
        }

        /// <summary>
        /// Sets the px application.
        /// </summary>
        /// <value>The px application.</value>
        public IMMPxApplication PxApplication
        {
            set { _PxApp = value; }
        }

        #endregion

        #region IMMPxDisplayName Members

        /// <summary>
        /// Gets the display name.
        /// </summary>
        /// <value>The display name.</value>
        public string DisplayName
        {
            get { return _DisplayName; }
        }

        #endregion

        /// <summary>
        /// Deletes the specified px node from the process framework database table
        /// </summary>
        /// <param name="pxNode">The px node.</param>
        /// <param name="message">The message.</param>
        /// <param name="status">The status.</param>
        protected virtual void InternalDelete(IMMPxNode pxNode, ref string message, ref int status)
        {
            
        }
    }
}
