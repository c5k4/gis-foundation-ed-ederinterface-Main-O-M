using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

using Miner.Interop;
using Miner.Interop.Process;
using Miner.Geodatabase;
using Miner.ComCategories;
using PGE.Common.Delivery.Diagnostics;
using PGE.Common.Delivery.Framework;

using log4net;
using System.Windows.Forms;

namespace PGE.Common.Delivery.ArcFM
{
    /// <summary>
    /// Abstract base class for Special AutoUpdaters.
    /// </summary>
    [ComVisible(true)]
    public abstract class BaseSpecialAU : IMMSpecialAUStrategyEx
    {

        #region Com Reg Methods

        /// <summary>
        /// Registers the specified reg key.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComRegisterFunction]
        private static void Register(string regKey)
        {
            SpecialAutoUpdateStrategy.Register(regKey);
        }
        /// <summary>
        /// Uns the register.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComUnregisterFunction]
        private static void UnRegister(string regKey)
        {
            SpecialAutoUpdateStrategy.Unregister(regKey);
        }

        #endregion

        private static Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "");

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseSpecialAU"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        protected BaseSpecialAU(string name)
        {
            _Name = name;
        }

        #region Protected Members
        /// <summary>
        /// The name of the auto updater.
        /// </summary>
        protected string _Name;

        #endregion


        #region IMMSpecialAUStrategyEx Members

        /// <summary>
        /// Executes the specified special AU for the ESRI Object.
        /// </summary>
        /// <param name="pObject">The esri object.</param>
        /// <param name="eAUMode">The AU mode.</param>
        /// <param name="eEvent">The edit event.</param>
        public void Execute(IObject pObject, mmAutoUpdaterMode eAUMode, mmEditEvent eEvent)
        {
            DateTime startTime = DateTime.Now;
            DateTime endTime;
            try
            {
                _logger.Debug(DateTime.Now.ToLongTimeString() + " ==== BEGIN EXECUTE: " + this._Name);

                //Changes for ENOS to SAP migration, If the AU is in 'DoNotOperateAutoUpdaters' list and the input object belongs to a object class that was added to DoNotOperateAutoUpdaters,
                //then do not execute this method.
                if (DoNotOperateAutoUpdaters.Instance.ContainsAU(((IObjectClass)pObject.Class).ObjectClassID, this.GetType()))
                    return;
                //Changes for ENOS to SAP migration .. End

                if (CanExecute(eAUMode))
                {
                    Logger.WriteLine(TraceEventType.Verbose, "Executing the " + _Name + " Special AU assigned on the "
                        + ((IDataset)pObject.Class).Name + " object class with ObjectID: " + pObject.OID);

                    string info = "Executing the " + _Name + " Special AU on the " + ((IDataset)pObject.Class).Name + " with OID: " + pObject.OID;
                    _logger.Debug(info);

                    InternalExecute(pObject, eAUMode, eEvent);
                }
            }
            catch (COMException e)
            {
                // If the MM_E_CANCELEDIT error is thrown, let it out.
                if (e.ErrorCode == (int)mmErrorCodes.MM_E_CANCELEDIT)
                {
                    //ArcFM does not show messages when the Edit is performed from the Attribute Editor.
                    //showing a message box explicitly would address the issue of the user not being notified when there is a error.
                    //This would cause the Message box being shown twice when the edit is performed using ArcGIS tools.
                    if (eAUMode == mmAutoUpdaterMode.mmAUMArcMap)
                    {
                        _logger.Error("Error Executing Autoupdater " + _Name, e);
                        MessageBox.Show(e.Message, "Autoupdater error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    throw;
                }

                _logger.Error("Error Executing Autoupdater " + _Name, e);
                IsUnitCallingStore = false;
            }
            catch (Exception e)
            {
                _logger.Error("Error Executing Autoupdater " + _Name, e);
                IsUnitCallingStore = false;
                
            }
            finally
            {
                string info = "FINSHED Executing the " + _Name + " Special AU on the " + ((IDataset)pObject.Class).Name + " with OID: " + pObject.OID;
                _logger.Debug(info);

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

        /// <summary>
        /// Gets whether the auto updater is enabled given the object class and edit event.
        /// </summary>
        /// <param name="pObjectClass">The object class.</param>
        /// <param name="eEvent">The edit event.</param>
        /// <returns><c>true</c> if enabled; otherwise <c>false</c>.</returns>
        public bool get_Enabled(IObjectClass pObjectClass, mmEditEvent eEvent)
        {
            try
            {
                return InternalEnabled(pObjectClass, eEvent);
            }
            catch (Exception e)
            {
                EventLogger.Error(e, "Error Getting Enabled Status For Special AU: " + _Name);
                _logger.Error("Error Getting Enabled Status For Special AU: " + _Name, e);
                return false;
            }
        }

        #endregion

        #region Protected Members
        /// <summary>
        /// Determines whether this instance can execute using the specified AU mode.
        /// </summary>
        /// <param name="eAUMode">The AU mode.</param>
        /// <returns>
        /// 	<c>true</c> if this instance can execute using the specified AU mode; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool CanExecute(mmAutoUpdaterMode eAUMode)
        {
            return (eAUMode != mmAutoUpdaterMode.mmAUMFeederManager);
        }


        /// <summary>
        /// Implementation of Autoupdater Execute Ex method for derived classes.
        /// </summary>
        /// <param name="obj">The object that triggered the Auto Udpater.</param>
        /// <param name="eAUMode">The auto updater mode.</param>
        /// <param name="eEvent">The edit event.</param>
        /// <remarks>This method will be called from IMMSpecialAUStrategy::ExecuteEx
        /// and is wrapped within the exception handling for that method.</remarks>
        protected virtual void InternalExecute(IObject obj, mmAutoUpdaterMode eAUMode, mmEditEvent eEvent)
        {

        }

        /// <summary>
        /// Implementation of Autoupdater Enabled method for derived classes.
        /// </summary>
        /// <param name="objectClass">The object class.</param>
        /// <param name="eEvent">The edit event.</param>
        /// <returns><c>true</c> if the AutoUpdater should be enabled; otherwise <c>false</c></returns>
        /// <remarks>This method will be called from IMMSpecialAUStrategy::get_Enabled
        /// and is wrapped within the exception handling for that method.</remarks>
        protected virtual bool InternalEnabled(IObjectClass objectClass, mmEditEvent eEvent)
        {
            return true;
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
                return Miner.Geodatabase.ModelNameManager.Instance;
            }
        }


        /// <summary>
        /// Gets the relationship class that has the model name on the destination class or origin class.
        /// </summary>
        /// <param name="oclass">The object class.</param>
        /// <param name="modelName">Name of the model.</param>
        /// <returns>
        /// The <see cref="IRelationshipClass"/>; otherwise an exception is thrown.
        /// </returns>
        /// <exception cref="NullReferenceException">If there is not a relationship class found with a destination/origin class assigned the specified model name.</exception>
        protected IRelationshipClass GetRelationshipClass(IObjectClass oclass, params string[] modelName)
        {
            IEnumRelationshipClass enumRelClass = oclass.get_RelationshipClasses(esriRelRole.esriRelRoleAny);
            enumRelClass.Reset();
            IRelationshipClass relClass = enumRelClass.Next();
            while (relClass != null)
            {
                IObjectClass destClass = relClass.DestinationClass;
                if (ModelNameFacade.ContainsAllClassModelNames(destClass, modelName))
                    return relClass;

                IObjectClass origClass = relClass.OriginClass;
                if (ModelNameFacade.ContainsAllClassModelNames(origClass, modelName))
                    return relClass;

                relClass = enumRelClass.Next();
            }

            throw new NullReferenceException("There is no relationship class with a destination/origin class assigned the '" + string.Join(" and ", modelName) + "' class model name.");
        }

        /// <summary>
        /// Gets the <see cref="ISpatialFilter"/> a filter using the geometry of the specified feature.
        /// </summary>
        /// <param name="feature">The feature.</param>
        /// <param name="spatialReference">The spatial reference.</param>
        /// <returns>
        /// 	<see cref="ISpatialFilter"/> a filter using the geometry of the specified feature.
        /// </returns>
        protected ISpatialFilter GetFilter(IFeature feature, esriSpatialRelEnum spatialReference)
        {
            ITopologicalOperator topOp = (ITopologicalOperator)feature.ShapeCopy;
            ISpatialFilter filter = new SpatialFilterClass();
            filter.SpatialRel = spatialReference;
            filter.Geometry = topOp.Buffer(0.1);
            filter.GeometryField = ((IFeatureClass)feature.Class).ShapeFieldName;
            return filter;
        }
        #endregion

        #region Public Static Memebers
        /// <summary>
        /// 
        /// </summary>
        public static bool IsUnitCallingStore { get; set; }
        #endregion

    }
}
