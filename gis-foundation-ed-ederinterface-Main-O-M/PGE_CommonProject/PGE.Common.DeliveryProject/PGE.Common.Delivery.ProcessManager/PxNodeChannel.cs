using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Miner.Interop.Process;
using System.Runtime.InteropServices;
using System.IO;
using System.Collections;

namespace PGE.Common.Delivery.Process
{
    /// <summary>
    /// An abstract class used to handle communication between the process framework database and client information.
    /// </summary>
    public abstract class PxNodeChannel<TChannel> : DictionaryBase, IDisposable
    {
        /// <summary>
        /// The Px Application object reference.
        /// </summary>
        protected IMMPxApplication _PxApp;
        /// <summary>
        /// The channel ID.
        /// </summary>
        protected int _ID;
        /// <summary>
        /// The name of the node type.
        /// </summary>
        protected string _NodeTypeName;
        /// <summary>
        /// The process framework helper.
        /// </summary>
        protected PxDb _PxDb;
        /// <summary>
        /// The underlying channel type.
        /// </summary>
        protected TChannel _Channel;

        /// <summary>
        /// The corresponding process framework node.
        /// </summary>
        private IMMPxNode _PxNode;
        /// <summary>
        /// The corresponding process framework history.
        /// </summary>
        private IMMPxNodeHistory _History;

        /// <summary>
        /// Initializes a new instance of the PxNodeChannel class.
        /// </summary>
        /// <param name="pxApp">The process framework application reference.</param>
        /// <param name="nodeTypeName">Name of the node type.</param>
        protected PxNodeChannel(IMMPxApplication pxApp, string nodeTypeName)
        {
            _PxApp = pxApp;
            _NodeTypeName = nodeTypeName;
            _PxDb = new PxDb(pxApp);
            this.Create();
        }

        /// <summary>
        /// Initializes a new instance of the PxNodeChannel class.
        /// </summary>
        /// <param name="pxApp">The process framework application reference.</param>
        /// <param name="channelID">The channel ID.</param>
        /// <param name="nodeTypeName">Name of the node type.</param>
        protected PxNodeChannel(IMMPxApplication pxApp, int channelID, string nodeTypeName)
        {
            _PxApp = pxApp;
            _ID = channelID;
            _NodeTypeName = nodeTypeName;
            _PxDb = new PxDb(pxApp);

            this.Initialize();
        }

        #region Abstract Members
        /// <summary>
        /// Gets a value indicating whether this instance is open.
        /// </summary>
        /// <value><c>true</c> if this instance is open; otherwise, <c>false</c>.</value>
        public abstract bool IsOpen { get; }

        /// <summary>
        /// Creates a new channel.
        /// </summary>
        protected abstract void Create();

        /// <summary>
        /// Initializes an existing channel.
        /// </summary>
        protected abstract void Initialize();

        #endregion

        #region Public Members
        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> with the specified key.
        /// </summary>
        /// <value></value>
        public object this[object key]
        {
            get
            {
                return this.Dictionary[key];
            }
            set
            {
                this.Dictionary[key] = value;
            }
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.IDictionary"/> object contains an element with the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="T:System.Collections.IDictionary"/> object.</param>
        /// <returns>
        /// true if the <see cref="T:System.Collections.IDictionary"/> contains an element with the key; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="key"/> is null.
        /// </exception>
        public bool Contains(object key)
        {
            return this.Dictionary.Contains(key);
        }

        /// <summary>
        /// Gets the history associated with the <see cref="P:Node"/>.
        /// </summary>
        /// <value>The history.</value>
        public IMMPxNodeHistory History
        {
            get
            {
                if (_History == null)
                {
                    _History = new PxNodeHistoryClass();
                    _History.Init(_PxApp.Connection, _PxApp.Login.SchemaName, this.PxNode.Id, this.PxNode.NodeType, null);
                }

                return _History;
            }
        }

        /// <summary>
        /// Gets the associated <see cref="IMMPxNode"/> object.
        /// </summary>
        /// <value>The node.</value>
        public IMMPxNode PxNode
        {
            get
            {
                if (_PxNode == null)
                {
                    // Initialize a new Session node.
                    int nodeType = _PxApp.Helper.GetNodeTypeID(_NodeTypeName);
                    IMMPxNodeEdit nodeEdit = new MMPxNodeListClass();
                    nodeEdit.Initialize(nodeType, _NodeTypeName, _ID);

                    _PxNode = (IMMPxNode)nodeEdit;
                    ((IMMPxApplicationEx)_PxApp).HydrateNodeFromDB(_PxNode);
                }

                return _PxNode;
            }
        }

        /// <summary>
        /// Gets the ID.
        /// </summary>
        /// <value>The ID.</value>
        public int ID
        {
            get
            {
                return _ID;
            }
        }

        /// <summary>
        /// Gets the version name.
        /// </summary>
        /// <value>The name of the version.</value>
        public string VersionName
        {
            get
            {
                int nodeType = _PxApp.Helper.GetNodeTypeID(_NodeTypeName);
                IMMPxSDEVersionNamer versionNamer = ((IMMPxApplicationEx2)_PxApp).GetVersionNamer(nodeType);
                if (versionNamer == null) return string.Empty;

                return versionNamer.GetVersionName(_ID);
            }
        }
        /// <summary>
        /// Gets the Associated WFM/SM Node
        /// </summary>
        public TChannel Node
        {
            get
            {
                return _Channel;
            }
        }
        /// <summary>
        /// Adds the history record for this session.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <param name="extraData">The extra data.</param>
        public void AddHistory(string description, string extraData)
        {
            if (this.History == null) return;

            IMMPxHistory history = new PxHistoryClass();
            history.CurrentUser = _PxApp.User.Id;
            history.CurrentUserName = _PxApp.User.Name;
            history.Date = DateTime.Now;
            history.Description = description;
            history.NodeId = this.PxNode.Id;
            history.nodeTypeId = this.PxNode.NodeType;
            history.ExtraData = extraData;

            ADODB.Property property = _PxApp.Connection.Properties["Data Source Name"];
            string dataSource = (!Convert.IsDBNull(property.Value)) ? Convert.ToString(property.Value) : string.Empty;

            if (File.Exists(dataSource))
            {
                FileSystemInfo fsi = new FileInfo(dataSource);
                history.Server = fsi.Name;
            }
            else if (Directory.Exists(dataSource))
            {
                FileSystemInfo fsi = new DirectoryInfo(dataSource);
                history.Server = fsi.Name;
            }
            else
            {
                history.Server = dataSource;
            }

            this.History.Add(history);
        }

        /// <summary>
        /// Updates the channel by flushing the <see cref="IMMPxNode"/> to the database and reinitializing the <see cref="IMMPxNode"/>.
        /// </summary>
        public virtual void Update()
        {
            // Flush the updates to the database.
            ((IMMPxApplicationEx)_PxApp).UpdateNodeToDB(this.PxNode);

            // Null out the node/history because it will be reinitialized once accessed again.
            _PxNode = null;
            _History = null;
        }
        #endregion

        #region Protected Members
        /// <summary>
        /// Copies the history from this channel to the specified channel.
        /// </summary>
        /// <param name="channel">The channel.</param>
        protected void CopyHistory(PxNodeChannel<TChannel> channel)
        {
            if (this.History == null) return;

            // Iterate through all of the history copying over the new history.
            this.History.Reset();
            IMMPxHistory history;
            while ((history = this.History.Next()) != null)
            {
                channel.History.Add(new PxHistoryClass()
                {
                    CurrentUser = history.CurrentUser,
                    CurrentUserName = history.CurrentUserName,
                    Date = history.Date,
                    Description = history.Description,
                    ExtraData = history.ExtraData,
                    Server = history.Server,
                    NodeId = channel.PxNode.Id,
                    nodeTypeId = channel.PxNode.NodeType
                });
            }
        }
        #endregion

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
            if (_PxNode != null)
                while (Marshal.ReleaseComObject(_PxNode) > 0) { }

            if (_History != null)
                while (Marshal.ReleaseComObject(_History) > 0) { }

            if (_Channel != null)
                while (Marshal.ReleaseComObject(_Channel) > 0) { }
        }

        #endregion
    }

}
