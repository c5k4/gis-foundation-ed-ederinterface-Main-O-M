using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using Miner.Interop.Process;
using PGE.Common.Delivery.Process;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace PGE.Common.Delivery.SessionManager
{
    /// <summary>
    /// A channel used to handle communication between the process framework for a session.
    /// </summary>
    [DebuggerDisplay("Name = {Name}, ID = {ID}, Owner = {Owner}")]
    public class Session : PxNodeChannel<IMMSession>, ICloneable
    {
        #region Constructors
        /// <summary>
        /// Constructor used to create a new Session.
        /// </summary>
        /// <param name="pxApp">Px Application object reference.</param>
        public Session(IMMPxApplication pxApp)
            : base(pxApp, "Session")
        {
        }

        /// <summary>
        /// Constructor used to reference an existing Session by ID.
        /// </summary>
        /// <param name="pxApp">Px Application object reference.</param>
        /// <param name="sessionID">The ID of the Session.</param>
        public Session(IMMPxApplication pxApp, int sessionID)
            : base(pxApp, sessionID, "Session")
        {
        }
        #endregion

        #region Public Members
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Session"/> is redlining.
        /// </summary>
        /// <value><c>true</c> if redlining; otherwise, <c>false</c>.</value>
        public bool Redlining
        {
            get
            {
                if (_Channel == null) return false;

                return ((IMMSession4)_Channel).get_Redlining();
            }
            set
            {
                if (_Channel == null)
                    return;

                ((IMMSession4)_Channel).set_Redlining(ref value);
            }
        }

        /// <summary>
        /// Gets or sets the create user.
        /// </summary>
        /// <value>The create user.</value>
        /// <exception cref="ArgumentOutOfRangeException">The create user cannot be larger then 32 characters.</exception>
        public string CreateUser
        {
            get
            {
                if (_Channel == null) return "";
                return _Channel.get_CreateUser();
            }
            set
            {
                if (_Channel == null) return;

                if (value.Length > 32)
                {
                    throw new ArgumentOutOfRangeException("The create user cannot be larger then 32 characters.");
                }
                else
                {
                    _Channel.set_CreateUser(ref value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the create date.
        /// </summary>
        /// <value>The create date.</value>
        public DateTime CreateDate
        {
            get
            {
                return _Channel.get_CreateDate();
            }
            set
            {
                if (_Channel == null) return;

                _Channel.set_CreateDate(ref value);
            }
        }

        /// <summary>
        /// Gets the date that the session was last saved.
        /// </summary>
        public DateTime DateModified
        {
            get
            {
                DateTime dateModified = DateTime.Now;
                if (this.History == null) return dateModified;

                // Iterate through all of the history records to find the one the indicates when this session was last saved.
                this.History.Reset();
                IMMPxHistory history;

                // The enumeration is organized by primary keys so the last one we find is the latest modified date.
                while ((history = this.History.Next()) != null)
                {
                    // Look for descriptions of either the session being created or the Save Session task being executed.                     
                    if (history.Description.Contains(PxDb.Tasks.SessionManager.SaveSession) ||
                        history.Description.Contains("Session created"))
                    {
                        dateModified = history.Date;
                    }
                }

                return dateModified;
            }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        /// <exception cref="ArgumentOutOfRangeException">The name cannot be larger then 64 characters.</exception>
        public string Name
        {
            get
            {
                if (_Channel == null) return "";
                return _Channel.get_Name();
            }
            set
            {
                if (_Channel == null) return;

                if (value.Length > 64)
                {
                    throw new ArgumentOutOfRangeException("The name cannot be larger then 64 characters.");
                }
                else
                {
                    _Channel.set_Name(ref value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        /// <exception cref="ArgumentOutOfRangeException">The description cannot be larger then 255 characters.</exception>
        public string Description
        {
            get
            {
                if (_Channel == null) return "";
                return _Channel.get_Description();
            }
            set
            {
                if (_Channel == null) return;

                if (value.Length > 255)
                {
                    throw new ArgumentOutOfRangeException("The description cannot be larger then 255 characters.");
                }
                else
                {
                    _Channel.set_Description(ref value);
                }
            }
        }

        /// <summary>
        /// Gets/sets the Session Type ID value.
        /// </summary>
        public int SessionType
        {
            get
            {
                if (_Channel == null) return -1;

                int type = ((IMMSession3)_Channel).get_SessionTypeId();
                return type;
            }
            set
            {
                if (_Channel == null) return;
                ((IMMSession3)_Channel).set_SessionTypeId(ref value);
            }
        }

        /// <summary>
        /// Gets or sets the owner.
        /// </summary>
        /// <value>The owner.</value>
        /// <exception cref="ArgumentOutOfRangeException">The owner cannot be larger then 32 characters.</exception>
        public string Owner
        {
            get
            {
                if (_Channel == null) return "";

                return _Channel.get_Owner();
            }
            set
            {
                if (_Channel == null) return;

                if (value.Length > 32)
                {
                    throw new ArgumentOutOfRangeException("The owner cannot be larger then 32 characters.");
                }
                else
                {
                    _Channel.set_Owner(ref value);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is open.
        /// </summary>
        /// <value><c>true</c> if this instance is open; otherwise, <c>false</c>.</value>
        public override bool IsOpen
        {
            get
            {
                IMMSessionManager2 sm = (IMMSessionManager2)_PxApp.FindPxExtensionByName("MMSessionManager");
                IMMSession session = sm.CurrentOpenSession;
                if (session == null) return false;
                bool isOpen = (session.get_ID() == _ID);
                return isOpen;
            }
        }

        /// <summary>
        /// Updates the session by flushing the <see cref="IMMPxNode"/> to the database, 
        /// clearing the <see cref="IMMSessionManager"/> cache and reinitializing the <see cref="IMMPxNode"/>.
        /// </summary>
        public override void Update()
        {
            // Flush the updates to the database.
            base.Update();

            // Clear the cache by calling get session with -1.
            int sessionID = -1;
            bool ro = false;
            IMMSessionManager sm = (IMMSessionManager)_PxApp.FindPxExtensionByName("MMSessionManager");
            sm.GetSession(ref sessionID, ref ro);
        }
        #endregion

        #region Protected Members
        /// <summary>
        /// Creates a new session.
        /// </summary>
        protected override void Create()
        {
            // Create the product Session object.
            IMMSessionManager sm = (IMMSessionManager)_PxApp.FindPxExtensionByName("MMSessionManager");

            _Channel = sm.CreateSession();
            _ID = _Channel.get_ID();
        }

        /// <summary>
        /// Initializes an existing session.
        /// </summary>
        protected override void Initialize()
        {
            // Reference the TM&M Session object.
            IMMSessionManager sm = (IMMSessionManager)_PxApp.FindPxExtensionByName("MMSessionManager");
            bool ro = false;

            _Channel = sm.GetSession(ref _ID, ref ro);
        }
        #endregion

        #region ICloneable Members

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <param name="deepCopy">if set to <c>true</c> deep copy will be created; otherwise a shallow copy will be created.</param>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        /// <remarks>If performing a deep copy the packet information will also be cloned for the new session.</remarks>
        public virtual object Clone(bool deepCopy)
        {
            // Create a clone and copy over the logical properties.
            Session clone = new Session(_PxApp)
            {
                Description = this.Description,
                SessionType = this.SessionType,
                Redlining = this.Redlining
            };

            // Make sure the states are updated to be the same.
            ((IMMPxApplicationEx5)_PxApp).SetNodeState(clone.PxNode, this.PxNode.State);

            // Copy the history.
            base.CopyHistory(clone);

            // When it's a deep copy clone the packet. 
            if (deepCopy)
            {
                // Copy the packet data.
                this.CopyPacket(clone);
            }

            // Update the clone.
            clone.Update();

            // Return the cloned session.
            return clone;
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// The packet information will also be cloned for the new session.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public object Clone()
        {
            return Clone(true);
        }
        #endregion

        #region Private Members
        /// <summary>
        /// Copies the packet from this session to the specified clone.
        /// </summary>
        /// <param name="clone">The clone.</param>
        private void CopyPacket(Session clone)
        {
            // When the session is not a mobile session we can stop here.
            IMMPxMobileHelper helper = (IMMPxMobileHelper)_PxApp;
            if (!helper.IsMobileSide(false)) return;

            // When the original packet doesn't exist exit out.
            string fileName = helper.GetPacketPath(this.PxNode, true);
            if (!File.Exists(fileName)) return;

            // Create a new packet for the clone.
            string packetID = string.Format("MMSM_{0}", Regex.Replace(Guid.NewGuid().ToString(), "{|}", ""));
            string clonePacket = helper.CreatePacket(clone.PxNode, true, ref packetID);

            // Load the original packet into the cloned packet.
            XmlDocument cloneDoc = new XmlDocument();
            cloneDoc.Load(fileName);

            // Update the node data.
            XmlNode node = cloneDoc.SelectSingleNode("/XML_PACKET/PACKET_ADAPTER/NODE_DATA");
            if (node != null) node.Attributes["NODE_ID"].Value = clone.PxNode.Id.ToString();

            // Save the modified packet.
            cloneDoc.Save(clonePacket);
        }
        #endregion
     }    
}
