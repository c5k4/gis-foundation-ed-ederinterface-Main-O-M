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

namespace PGE.Common.Delivery.Process.Forms
{
    [DebuggerDisplay("Name = {Name}, ID = {ID}, Owner = {Owner}")]
    public class AssignSession : PxNodeChannel<IMMSession>
    {

        public AssignSession(IMMPxApplication pxApp)
            : base(pxApp, "Session")
        {
        }

        /// <summary>
        /// Constructor used to reference an existing Session by ID.
        /// </summary>
        /// <param name="pxApp">Px Application object reference.</param>
        /// <param name="sessionID">The ID of the Session.</param>
        public AssignSession(IMMPxApplication pxApp, int sessionID)
            : base(pxApp, sessionID, "Session")
        {
        }

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

    }
}

