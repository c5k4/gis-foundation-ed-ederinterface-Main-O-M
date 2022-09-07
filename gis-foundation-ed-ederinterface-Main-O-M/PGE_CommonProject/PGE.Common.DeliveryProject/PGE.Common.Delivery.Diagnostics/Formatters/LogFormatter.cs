//===============================================================================
// Microsoft patterns & practices Enterprise Library
// Logging Application Block
//===============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

namespace PGE.Common.Delivery.Diagnostics.Formatters
{
	/// <summary>
	/// Abstract implememtation of the <see cref="ILogFormatter"/> interface.
	/// </summary>
	public abstract class LogFormatter : ILogFormatter
	{
		/// <summary>
		/// Formats a log entry and return a string to be outputted.
		/// </summary>
		/// <param name="log">Log entry to format.</param>
		/// <returns>A string representing the log entry.</returns>
		public abstract string Format(LogEntry log);

        /// <summary>
        /// Gets the default template.
        /// </summary>
        /// <value>The default template.</value>
        protected string DefaultTextFormat
        {
            get
            {
                return "Timestamp: {timestamp(local)}{newline}Message: {message}{newline}Severity: {severity}{newline}Machine: {machine}{newline}App Domain: {appDomain}{newline}ProcessId: {processId}{newline}Process Name: {processName}{newline}Thread Name: {threadName}{newline}Win32 ThreadId: {win32ThreadId}";
            }
        }
	}
}
