using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;

using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;

using Miner.Interop;
using PGE.Common.Delivery.Diagnostics;

namespace PGE.Common.Delivery.ArcFM
{
    /// <summary>
    /// Abstract base class for Abandoned AutoUpdaters.
    /// </summary>
    abstract public class BaseAbandonAU : IMMAbandonAUStrategy
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
            Miner.ComCategories.MMAbandonStrategy.Register(regKey);
        }
        /// <summary>
        /// Uns the register.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComUnregisterFunction]
        static public void UnRegister(string regKey)
        {
            Miner.ComCategories.MMAbandonStrategy.Unregister(regKey);
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseAbandonAU"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public BaseAbandonAU(string name)
        {
            _Name = name;
        }

        #endregion
        /// <summary>
        /// The name of the auto udpater.
        /// </summary>
        protected string _Name;

        #region IMMAbandonAUStrategy Members

        /// <summary>
        /// Executes the specified abandon auto updater.
        /// </summary>
        /// <param name="pObj">The p obj.</param>
        /// <param name="pNewObj">The p new obj.</param>
        virtual public void Execute(IObject pObj, IObject pNewObj)
        {
            try
            {
                Logger.WriteLine(TraceEventType.Verbose, "Executing the " + _Name + " Abandon Autoupdater.");
                _logger.Debug("Executing the " + _Name + " Abandon Autoupdater.");

                InternalExecute(pObj, pNewObj);
            }
            catch (Exception e)
            {
                _logger.Error("Error Executing Abandon AU: " + _Name, e);
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
        /// Internal execution method for Abandon AUs.
        /// </summary>
        /// <param name="origObj">The original (pre-abandoned) object.</param>
        /// <param name="newObj">The new (abandoned) object.</param>
        virtual protected void InternalExecute(IObject origObj, IObject newObj)
        {


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

        #endregion

    }
}
