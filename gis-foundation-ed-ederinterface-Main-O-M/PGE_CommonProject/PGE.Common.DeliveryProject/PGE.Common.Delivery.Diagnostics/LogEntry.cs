using System;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.Management.Instrumentation;
using System.Text;
using System.Security;
using System.Security.Permissions;
using System.Threading;
using System.Xml.Serialization;
using System.Collections.Generic;

using PGE.Common.Delivery.Diagnostics.Formatters;

namespace PGE.Common.Delivery.Diagnostics
{
    /// <summary>
    /// Represents a log message.  Contains the common properties that are required for all log messages.
    /// </summary>
    [XmlRoot("logEntry")]
    [Serializable]
    [InstrumentationClass(InstrumentationType.Event)]
    [ManagedName("LogEntryV20")]
    public class LogEntry : ICloneable
    {
        private static readonly TextFormatter _Formatter = new TextFormatter();

        private string _Message = string.Empty;
        private string _Title = string.Empty;
        private ICollection<string> _Categories = new List<string>(0);
        private int _Priority = -1;
        private int _EventId;
        private Guid _ActivityId;
        private Guid? _RelatedActivityId;

        private TraceEventType _Severity = TraceEventType.Information;

        private string _MachineName = string.Empty;
        private DateTime _TimeStamp = DateTime.MaxValue;

        private StringBuilder _ErrorMessages;
        private IDictionary<string, object> _ExtendedProperties;

        private string _AppDomainName;
        private string _ProcessId;
        private string _ProcessName;
        private string _ThreadName;
        private string _Win32ThreadId;

        /// <summary>
        /// Initialize a new instance of a <see cref="LogEntry"/> class.
        /// </summary>
        public LogEntry()
        {
            CollectIntrinsicProperties();
        }

        /// <summary>
        /// Create a new instance of <see cref="LogEntry"/> with a full set of constructor parameters
        /// </summary>
        /// <param name="message">Message body to log.  Value from ToString() method from message object.</param>
        /// <param name="category">Category name used to route the log entry to a one or more trace listeners.</param>
        /// <param name="priority">Only messages must be above the minimum priority are processed.</param>
        /// <param name="eventId">Event number or identifier.</param>
        /// <param name="severity">Log entry severity as a <see cref="Severity"/> enumeration. (Unspecified, Information, Warning or Error).</param>
        /// <param name="title">Additional description of the log entry message.</param>
        /// <param name="properties">Dictionary of key/value pairs to record.</param>
        public LogEntry(object message, string category, int priority, int eventId,
                        TraceEventType severity, string title, IDictionary<string, object> properties)
            : this(message, BuildCategoriesCollection(category), priority, eventId, severity, title, properties)
        {
        }

        /// <summary>
        /// Create a new instance of <see cref="LogEntry"/> with a full set of constructor parameters
        /// </summary>
        /// <param name="message">Message body to log.  Value from ToString() method from message object.</param>
        /// <param name="categories">Collection of category names used to route the log entry to a one or more sinks.</param>
        /// <param name="priority">Only messages must be above the minimum priority are processed.</param>
        /// <param name="eventId">Event number or identifier.</param>
        /// <param name="severity">Log entry severity as a <see cref="Severity"/> enumeration. (Unspecified, Information, Warning or Error).</param>
        /// <param name="title">Additional description of the log entry message.</param>
        /// <param name="properties">Dictionary of key/value pairs to record.</param>
        public LogEntry(object message, ICollection<string> categories, int priority, int eventId,
                        TraceEventType severity, string title, IDictionary<string, object> properties)
        {
            if (message == null)
                throw new ArgumentNullException("message");
            if (categories == null)
                throw new ArgumentNullException("categories");

            this.Message = message.ToString();
            this.Priority = priority;
            this.Categories = categories;
            this.EventId = eventId;
            this.Severity = severity;
            this.Title = title;
            this.ExtendedProperties = properties;

            CollectIntrinsicProperties();
        }

        /// <summary>
        /// Message body to log.  Value from ToString() method from message object.
        /// </summary>
        public string Message
        {
            get { return this._Message; }
            set { this._Message = value; }
        }

        /// <summary>
        /// Category name used to route the log entry to a one or more trace listeners.
        /// </summary>
        [IgnoreMember]
        public ICollection<string> Categories
        {
            get { return _Categories; }
            set { this._Categories = value; }
        }

        /// <summary>
        /// Importance of the log message.  Only messages whose priority is between the minimum and maximum priorities (inclusive)
        /// will be processed.
        /// </summary>
        public int Priority
        {
            get { return this._Priority; }
            set { this._Priority = value; }
        }

        /// <summary>
        /// Event number or identifier.
        /// </summary>
        public int EventId
        {
            get { return this._EventId; }
            set { this._EventId = value; }
        }

        /// <summary>
        /// Log entry severity as a <see cref="Severity"/> enumeration. (Unspecified, Information, Warning or Error).
        /// </summary>
        [IgnoreMember]
        public TraceEventType Severity
        {
            get { return this._Severity; }
            set { this._Severity = value; }
        }

        /// <summary>
        /// <para>Gets the string representation of the <see cref="Severity"/> enumeration.</para>
        /// </summary>
        /// <value>
        /// <para>The string value of the <see cref="Severity"/> enumeration.</para>
        /// </value>
        public string LoggedSeverity
        {
            get { return _Severity.ToString(); }
        }

        /// <summary>
        /// Additional description of the log entry message.
        /// </summary>
        public string Title
        {
            get { return this._Title; }
            set { this._Title = value; }
        }

        /// <summary>
        /// Date and time of the log entry message.
        /// </summary>
        public DateTime TimeStamp
        {
            get { return this._TimeStamp; }
            set { this._TimeStamp = value; }
        }

        /// <summary>
        /// Name of the computer.
        /// </summary>
        public string MachineName
        {
            get { return this._MachineName; }
            set { this._MachineName = value; }
        }

        /// <summary>
        /// The <see cref="AppDomain"/> in which the program is running
        /// </summary>
        public string AppDomainName
        {
            get { return this._AppDomainName; }
            set { this._AppDomainName = value; }
        }

        /// <summary>
        /// The Win32 process ID for the current running process.
        /// </summary>
        public string ProcessId
        {
            get { return _ProcessId; }
            set { _ProcessId = value; }
        }

        /// <summary>
        /// The name of the current running process.
        /// </summary>
        public string ProcessName
        {
            get { return _ProcessName; }
            set { _ProcessName = value; }
        }

        /// <summary>
        /// The name of the .NET thread.
        /// </summary>
        ///  <seealso cref="Win32ThreadId"/>
        public string ManagedThreadName
        {
            get { return _ThreadName; }
            set { _ThreadName = value; }
        }

        /// <summary>
        /// The Win32 Thread ID for the current thread.
        /// </summary>
        public string Win32ThreadId
        {
            get { return _Win32ThreadId; }
            set { _Win32ThreadId = value; }
        }

        /// <summary>
        /// Dictionary of key/value pairs to record.
        /// </summary>
        [IgnoreMember]
        public IDictionary<string, object> ExtendedProperties
        {
            get
            {
                if (_ExtendedProperties == null)
                {
                    _ExtendedProperties = new Dictionary<string, object>();
                }
                return this._ExtendedProperties;
            }
            set { this._ExtendedProperties = value; }
        }

        /// <summary>
        /// Read-only property that returns the timeStamp formatted using the current culture.
        /// </summary>
        public string TimeStampString
        {
            get { return TimeStamp.ToString(CultureInfo.CurrentCulture); }
        }

        /// <summary>
        /// Tracing activity id
        /// </summary>
        [IgnoreMember]
        public Guid ActivityId
        {
            get { return this._ActivityId; }
            set { this._ActivityId = value; }
        }

        /// <summary>
        /// Related activity id
        /// </summary>
        [IgnoreMember]
        public Guid? RelatedActivityId
        {
            get { return this._RelatedActivityId; }
            set { this._RelatedActivityId = value; }
        }

        /// <summary>
        /// Creates a new <see cref="LogEntry"/> that is a copy of the current instance.
        /// </summary>
        /// <remarks>
        /// If the dictionary contained in <see cref="ExtendedProperties"/> implements <see cref="ICloneable"/>, the resulting
        /// <see cref="LogEntry"/> will have its ExtendedProperties set by calling <c>Clone()</c>. Otherwise the resulting
        /// <see cref="LogEntry"/> will have its ExtendedProperties set to <see langword="null"/>.
        /// </remarks>
        /// <implements>ICloneable.Clone</implements>
        /// <returns>A new <c>LogEntry</c> that is a copy of the current instance.</returns>
        public object Clone()
        {
            LogEntry result = new LogEntry();

            result.Message = this.Message;
            result.EventId = this.EventId;
            result.Title = this.Title;
            result.Severity = this.Severity;
            result.Priority = this.Priority;

            result.TimeStamp = this.TimeStamp;
            result.MachineName = this.MachineName;
            result.AppDomainName = this.AppDomainName;
            result.ProcessId = this.ProcessId;
            result.ProcessName = this.ProcessName;
            result.ManagedThreadName = this.ManagedThreadName;
            result.ActivityId = this.ActivityId;

            // clone categories
            result.Categories = new List<string>(this.Categories);

            // clone extended properties
            if (this._ExtendedProperties != null)
                result.ExtendedProperties = new Dictionary<string, object>(this._ExtendedProperties);

            // clone error messages
            if (this._ErrorMessages != null)
            {
                result._ErrorMessages = new StringBuilder(this._ErrorMessages.ToString());
            }

            return result;
        }

        /// <summary>
        /// Add an error or warning message to the start of the messages string builder.
        /// </summary>
        /// <param name="message">Message to be added to this instance</param>
        public virtual void AddErrorMessage(string message)
        {
            if (_ErrorMessages == null)
            {
                _ErrorMessages = new StringBuilder();
            }
            _ErrorMessages.Insert(0, Environment.NewLine);
            _ErrorMessages.Insert(0, Environment.NewLine);
            _ErrorMessages.Insert(0, message);
        }

        /// <summary>
        /// Gets the error message with the <see cref="LogEntry"></see>
        /// </summary>
        public string ErrorMessages
        {
            get
            {
                if (_ErrorMessages == null)
                    return null;
                return _ErrorMessages.ToString();
            }
        }

        /// <summary>
        /// Returns a <see cref="String"/> that represents the current <see cref="LogEntry"/>, 
        /// using a default formatting template.
        /// </summary>
        /// <returns>A <see cref="String"/> that represents the current <see cref="LogEntry"/>.</returns>
        public override string ToString()
        {
            return _Formatter.Format(this);
        }

        /// <summary>
        /// Set the intrinsic properties such as MachineName and UserIdentity.
        /// </summary>
        private void CollectIntrinsicProperties()
        {
            this.TimeStamp = DateTime.UtcNow;

            try
            {
                this.ActivityId = Trace.CorrelationManager.ActivityId;
            }
            catch (Exception)
            {
                this.ActivityId = Guid.Empty;
            }

            // do not try to avoid the security exception, as it would only duplicate the stack walk
            try
            {
                MachineName = Environment.MachineName;
            }
            catch
            {
                this.MachineName = null;
            }

            try
            {
                _AppDomainName = AppDomain.CurrentDomain.FriendlyName;
            }
            catch
            {
                _AppDomainName = null;
            }

            // check whether the unmanaged code permission is available to avoid three potential stack walks
            bool unmanagedCodePermissionAvailable = false;
            SecurityPermission unmanagedCodePermission = new SecurityPermission(SecurityPermissionFlag.UnmanagedCode);
            // avoid a stack walk by checking for the permission on the current assembly. this is safe because there are no
            // stack walk modifiers before the call.

            // Code Added - Vedant on 12/03/2020 - obselete function SecurityManager.IsGranted
            var permissionSet = new PermissionSet(PermissionState.None);
            permissionSet.AddPermission(unmanagedCodePermission);
            //if (SecurityManager.IsGranted(unmanagedCodePermission))
            if (permissionSet.IsSubsetOf(AppDomain.CurrentDomain.PermissionSet))
            {
                try
                {
                    unmanagedCodePermission.Demand();
                    unmanagedCodePermissionAvailable = true;
                }
                catch (SecurityException)
                { }
            }

            if (unmanagedCodePermissionAvailable)
            {
                try
                {
                    _ProcessId = System.Diagnostics.Process.GetCurrentProcess().Id.ToString();
                }
                catch
                {
                    _ProcessId = "0";
                }

                try
                {
                    _ProcessName = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
                }
                catch
                {
                    _ProcessName = null;
                }

                try
                {
                    _Win32ThreadId = Thread.CurrentThread.ManagedThreadId.ToString();
                }
                catch
                {
                    _Win32ThreadId = "0";
                }
            }

            try
            {
                _ThreadName = Thread.CurrentThread.Name;
            }
            catch
            {
                _ThreadName = null;
            }
        }

        private static ICollection<string> BuildCategoriesCollection(string category)
        {
            if (string.IsNullOrEmpty(category))
                throw new ArgumentNullException("category");

            return new string[] { category };
        }

        /// <summary>
        /// Tracing activity id as a string to support WMI Queries
        /// </summary>
        public string ActivityIdString
        {
            get { return this.ActivityId.ToString(); }
        }

        /// <summary>
        /// Category names used to route the log entry to a one or more trace listeners.
        /// This readonly property is available to support WMI queries
        /// </summary>
        public string[] CategoriesStrings
        {
            get
            {
                string[] categoriesStrings = new string[Categories.Count];
                this.Categories.CopyTo(categoriesStrings, 0);
                return categoriesStrings;
            }
        }
    }
}
