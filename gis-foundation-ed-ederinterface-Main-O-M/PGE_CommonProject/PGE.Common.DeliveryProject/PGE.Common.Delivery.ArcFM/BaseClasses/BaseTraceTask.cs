using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Reflection;

using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.EditorExt;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;

using Miner.Interop;

using PGE.Common.Delivery.Diagnostics;

namespace PGE.Common.Delivery.ArcFM
{
    /// <summary>
    /// An abstract class that is used to create custom trace tasks in ArcMap.
    /// </summary>
    public abstract class BaseTraceTask : ITraceTask, ITraceTaskResults
    {

        private static Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "");

        /// <summary>
        /// The name of the trace task.
        /// </summary>
        protected string _Name;
        /// <summary>
        /// The network analysis extension.
        /// </summary>
        protected INetworkAnalysisExt _NetworkAnalysis;        
        /// <summary>
        /// The network flag extension.
        /// </summary>
        protected INetworkAnalysisExtFlags _Flags;
        /// <summary>
        /// The network barrier extension.
        /// </summary>
        protected INetworkAnalysisExtBarriers _Barriers;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseTraceTask"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        protected BaseTraceTask(string name)
        {
            _Name = name;
        }

        #region Com Reg Methods

        /// <summary>
        /// Registers the specified reg key.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComRegisterFunction]
        static public void Register(string regKey)
        {
            Miner.ComCategories.ArcMapTraceTasks.Register(regKey);
        }

        /// <summary>
        /// Unregisters the specified reg key.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComUnregisterFunction]
        static public void Unregister(string regKey)
        {
            Miner.ComCategories.ArcMapTraceTasks.Unregister(regKey);
        }

        #endregion

        #region ITraceTask Members

        /// <summary>
        /// Gets a value indicating if the trace task is ready to be executed.
        /// </summary>
        /// <value><c>true</c> if the trace task is ready to be executed; otherwise, <c>false</c>.</value>
        public virtual bool EnableSolve
        {
            get { return true; }
        }

        /// <summary>
        /// Gets the name of the trace task.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return _Name; }
        }

        /// <summary>
        /// Initializes the trace task.
        /// </summary>
        /// <param name="utilityNetworkAnalysis">The utility network analysis.</param>
        public virtual void OnCreate(IUtilityNetworkAnalysisExt utilityNetworkAnalysis)
        {
            _NetworkAnalysis = (INetworkAnalysisExt)utilityNetworkAnalysis;
            _Flags = (INetworkAnalysisExtFlags)utilityNetworkAnalysis;                      
            _Barriers = (INetworkAnalysisExtBarriers)utilityNetworkAnalysis;
        }

        /// <summary>
        /// Executes the trace task.
        /// </summary>
        public virtual void OnTraceExecution()
        {
            try
            {
                this.ResultEdges = new EnumNetEIDArrayClass();
                this.ResultJunctions = new EnumNetEIDArrayClass();

                InternalExecute();
            }
            catch (Exception ex)
            {
                _logger.Error(Name, ex);
            }
        }

        #endregion

        #region ITraceTaskResults Members

        /// <summary>
        /// Gets the edges in the trace task result.
        /// </summary>
        /// <value>The result edges.</value>
        public abstract IEnumNetEID ResultEdges
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets the junctions in the trace task result.
        /// </summary>
        /// <value>The result junctions.</value>
        public abstract IEnumNetEID ResultJunctions
        {
            get;
            protected set;
        }
      
        #endregion

        #region Protected Members
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
        /// Handles executing the trace for the trace task.
        /// </summary>
        /// <remarks>This method will be called from <see cref="M:ITraskTask.OnTraceExecution"/>
        /// and is wrapped within the exception handling for that method.</remarks>
        protected virtual void InternalExecute()
        {
            
        }

        /// <summary>
        /// Adds the flag.
        /// </summary>
        /// <param name="flagSymbol">The flag symbol.</param>
        /// <param name="elementType">Type of the element.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        protected void AddFlag(ISymbol flagSymbol, esriElementType elementType, int x, int y)
        {
            IMMTraceUIUtilitiesEx traceUtilities = new MMTraceUIUtilitiesClass();
            traceUtilities.AddFlag(_NetworkAnalysis, elementType, flagSymbol, x, y);
        }       
        #endregion
    }
}
