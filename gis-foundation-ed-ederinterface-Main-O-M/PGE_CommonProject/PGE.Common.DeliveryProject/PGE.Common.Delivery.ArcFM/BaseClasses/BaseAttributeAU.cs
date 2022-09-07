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
using Miner.Geodatabase;

using PGE.Common.Delivery.Diagnostics;

namespace PGE.Common.Delivery.ArcFM
{
    /// <summary>
    /// Abstract base class for Attribute AutoUpdaters.
    /// </summary>
    abstract public class BaseAttributeAU : IMMAttrAUStrategy
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
            Miner.ComCategories.AttrAutoUpdateStrategy.Register(regKey);
        }
        /// <summary>
        /// Uns the register.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComUnregisterFunction]
        static public void UnRegister(string regKey)
        {
            Miner.ComCategories.AttrAutoUpdateStrategy.Unregister(regKey);
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseAttributeAU"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="domainName">Name of the domain.</param>
        /// <param name="fieldType">Type of the field.</param>
        public BaseAttributeAU(string name, string domainName, esriFieldType fieldType)
        {
            _Name = name;
            _DomainName = domainName;
            _FieldType = fieldType;
        }

        #endregion

        #region Protected Fields

        /// <summary>
        /// The name of the auto udpater.
        /// </summary>
        protected string _Name;
        /// <summary>
        /// The domain name that must be assigned.
        /// </summary>
        protected string _DomainName;
        /// <summary>
        /// The required field data type.
        /// </summary>
        protected esriFieldType _FieldType;

        #endregion

        #region IMMAttrAUStrategy Members

        /// <summary>
        /// Gets the name of the domain.
        /// </summary>
        /// <value>The name of the domain.</value>
        public string DomainName
        {
            get { return _DomainName; }
        }

        /// <summary>
        /// Gets the type of the field.
        /// </summary>
        /// <value>The type of the field.</value>
        public esriFieldType FieldType
        {
            get { return _FieldType; }
        }

        /// <summary>
        /// Gets the auto value.
        /// </summary>
        /// <param name="pObj">The ESRI object.</param>
        /// <returns>The object that is set on the field.</returns>
        virtual public object GetAutoValue(IObject pObj)
        {
            DateTime startTime = DateTime.Now;
            DateTime endTime;

            try
            {
                _logger.Debug(DateTime.Now.ToLongTimeString() + " ==== BEGIN EXECUTE: " + this._Name);

                // Don't execute if running within FeederManager or PhaseSwap.
                //IMMAutoUpdater au = new MMAutoUpdaterClass();
                // 10.1 workaround:
                Type type = Type.GetTypeFromProgID("mmGeodatabase.MMAutoUpdater");
                object obj = Activator.CreateInstance(type);
                IMMAutoUpdater au = obj as IMMAutoUpdater;

                if (au.AutoUpdaterMode == mmAutoUpdaterMode.mmAUMFeederManager || au.AutoUpdaterMode == mmAutoUpdaterMode.mmAUPhaseSwap)
                    throw new COMException("", (int)mmErrorCodes.MM_S_NOCHANGE);

                Logger.WriteLine(TraceEventType.Verbose, "Executing the " + _Name + " Attribute Auto Updater assigned on the "
                   + ((IDataset)pObj.Class).Name + " object class with ObjectID: " + pObj.OID);

                _logger.Debug("Executing the " + _Name + " Attribute Auto Updater assigned on the "
                   + ((IDataset)pObj.Class).Name + " object class with ObjectID: " + pObj.OID);

                return InternalExecute(pObj);

            }
            catch (COMException e)
            {
                // If the MM_S_NOCHANGE error was thrown, let it out so ArcFM will 
                // know what to do.
                if (e.ErrorCode == (int)mmErrorCodes.MM_S_NOCHANGE)
                    throw;

                _logger.Error("Error Executing Attribute Autoupdater: " + _Name, e);
                return null;
            }
            catch (Exception e)
            {
                _logger.Error("Error Executing Attribute Autoupdater: " + _Name, e);
                return null;
            }
            finally
            {
                endTime = DateTime.Now;
                TimeSpan ts = endTime - startTime;
                _logger.Debug("TOTAL EXECUTION TIME ==== " + ts.TotalSeconds.ToString());
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return _Name; }
        }

        #endregion

        #region Protected Members

        /// <summary>
        /// Returns the value for the field the AU is assigned to.
        /// </summary>
        /// <param name="pObj">The object to be updated.</param>
        /// <returns>A value for the assigned field.</returns>
        /// <remarks>This method will be called from IMMAttrAUStrategy::GetAutoValue
        /// and is wrapped within the exception handling for that method.</remarks>
        virtual protected object InternalExecute(IObject pObj)
        {
            return null;
        }

        /// <summary>
        /// Returns the ESRI Application reference.
        /// </summary>
        protected IApplication Application
        {
            get
            {
                try
                {
                    Type type = Type.GetTypeFromProgID("esriFramework.AppRef");
                    object obj = Activator.CreateInstance(type);
                    IApplication app = (IApplication)obj;
                    return app;
                }
                catch
                {
                    // Couldn't create the AppRef.  Probably not in ArcMap or ArcCatalog.
                    return null;
                }
            }
        }

        /// <summary>
        /// Returns the Px Application object from the Session Manager Integration extension to ArcMap.
        /// </summary>
        protected IMMPxApplication PxApp
        {
            get
            {
                try
                {
                    IApplication app = Application;
                    IMMPxIntegrationCache intcache = (IMMPxIntegrationCache)app.FindExtensionByName("Session Manager Integration Extension");
                    IMMPxApplication pxApp = intcache.Application;
                    return pxApp;
                }
                catch
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Creates an instance of the Model Name Manager component.
        /// </summary>
        /// <returns>Model Name Manager reference.</returns>
        protected IMMModelNameManager ModelNameManager
        {
            get
            {
                Type type = Type.GetTypeFromProgID("mmGeodatabase.MMModelNameManager");
                object o = Activator.CreateInstance(type);
                IMMModelNameManager mnm = (IMMModelNameManager)o;
                return mnm;
            }
        }

        #endregion

    }
}
