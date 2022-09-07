using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PGE.BatchApplication.ConductorInTrench
{
    /// An exception which includes error code numbers corresponding to the <see cref="ErrorCode"/> enumeration.
    /// Exceptions are wrapped in this class in order to properly set the application's error code for UC4 purposes.
    /// </summary>
    public class ErrorCodeException : Exception
    {
        #region Constants
        /// <summary>
        /// The message used when a miscellaneous, unwrapped exception is caught.
        /// </summary>
        private const string _ERRORMSG_GENERIC = "A miscellanous exception has been caught within the application.";
        #endregion

        #region Properties
        /// <summary>
        /// The error code of this exception.
        /// </summary>
        public ErrorCode Code { get; set; }

        /// <summary>
        /// The code number corresponding to this error's <see cref="ErrorCode"/>.
        /// </summary>
        public int CodeNumber
        {
            get
            {
                return (int)Code;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorCodeException"/> class.
        /// The exception will be thrown as a general error.
        /// </summary>
        public ErrorCodeException()
            : this(ErrorCode.General, _ERRORMSG_GENERIC) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorCodeException"/> class.
        /// The exception will be thrown as a general error.
        /// </summary>
        /// <param name="innerException">The exception to use as the inner exception.</param>
        public ErrorCodeException(Exception innerException)
            : this(ErrorCode.General, _ERRORMSG_GENERIC, innerException) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorCodeException"/> class with the <see cref="ErrorCode"/> provided.
        /// </summary>
        /// <param name="code">The <see cref="ErrorCode"/> that represents this error.</param>
        /// <param name="message">The error message to associate with this exception.</param>
        public ErrorCodeException(ErrorCode code, string message)
            : base(message)
        {
            Code = code;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorCodeException"/> class with the <see cref="ErrorCode"/> provided.
        /// </summary>
        /// <param name="code">The <see cref="ErrorCode"/> that represents this error.</param>
        /// <param name="message">The error message to associate with this exception.</param>
        /// <param name="innerException">The exception to use as the inner exception.</param>
        public ErrorCodeException(ErrorCode code, string message, Exception innerException)
            : base(message, innerException)
        {
            Code = code;
        }
        #endregion
    }

    #region Enumerations
    /// <summary>
    /// Preset error codes for the application. The integer value of each error is used as the application's
    /// error code if the error is encountered (via use of the <see cref="ErrorCodeException"/>).
    /// </summary>
    public enum ErrorCode
    {
        /// <summary>
        /// Miscellaneous error.
        /// </summary>
        General = 1,
        /// <summary>
        /// Couldn't check out ArcGIS or ArcFM license.
        /// </summary>
        LicenseCheckout = 2,
        /// <summary>
        /// Trouble obtaining workspaces.
        /// </summary>
        Workspace = 3,
        /// <summary>
        /// Settings were not configured properly for the application.
        /// </summary>
        ToolSettings = 4,
        /// <summary>
        /// Couldn't find the versions specified. This may be an owner problem if the version exists.
        /// </summary>
        VersionRetrieval = 5,
        /// <summary>
        /// Couldn't roll versions forward (done at the end of the application run).
        /// </summary>
        VersionReset = 6,
        /// <summary>
        /// Trouble obtaining the grid feature class from the configured name, or one of its required fields.
        /// </summary>
        GridClass = 7,
        /// <summary>
        /// The ChangeManager class could not load differences or encountered an error within its logic.
        /// </summary>
        ChangeManager = 8,
        /// <summary>
        /// Can't find the output table or one of its fields.
        /// </summary>
        OutputTable = 9,
        /// <summary>
        /// Can't find the temporary version used during the processing of the version differences (the future daily change version).
        /// e.g. might have been inadvertently deleted during processing.
        /// </summary>
        TempVersionMissing = 10,
        /// <summary>
        /// Can't find the file gdb (e.g. WIP) used during the processing of the version differences (the future daily change version).
        /// e.g. might have been inadvertently deleted during processing.
        /// </summary>        
        FGDBBaseMissing = 11,
        /// <summary>
        /// File gdb (e.g. WIP) edits exist. They should not exist. If they do, manually rollover (remove _edits)
        /// </summary>        
        FGDBEditsExist = 12,
        /// <summary>
        /// Can't find the mdb (e.g. WIP) used during the processing of the version differences (the future daily change version).
        /// e.g. might have been inadvertently deleted during processing.
        /// </summary>        
        MDBBaseMissing = 13,
        /// <summary>
        /// mdb (e.g. WIP) edits exist. They should not exist. If they do, manually rollover (remove _edits)
        /// </summary>        
        MDBEditsExist = 14,
        /// <summary>
        /// mdb (e.g. WIP) edits exist. They should not exist. If they do, manually rollover (remove _edits)
        /// </summary>        
        MDBBaseFCDoesNotExist = 15,
        /// <summary>
        /// mdb (e.g. WIP) edits exist. They should not exist. If they do, manually rollover (remove _edits)
        /// </summary>        
        SDEFCDoesNotExist = 16
    }

    #endregion
}
