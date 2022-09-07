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

using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Collections.Generic;
using PGE.Common.Delivery.Diagnostics.Formatters;

namespace PGE.Common.Delivery.Diagnostics
{
	/// <summary>
	/// Base class for custom trace listeners that support formatters.
	/// </summary>
	public abstract class FormatterTraceListener : TraceListener
	{
		private ILogFormatter _Formatter;

	    /// <summary>
		/// Gets or sets the log entry formatter.
		/// </summary>
		public ILogFormatter Formatter
		{
			get { return this._Formatter; }
			set { this._Formatter = value; }
		}

        /// <summary>
        /// Intercepts the tracing request to format the object to trace.
        /// </summary>
        /// <remarks>
        /// Formatting is only performed if the object to trace is a <see cref="LogEntry"/> and the formatter is set.
        /// </remarks>
        /// <param name="eventCache">The context information.</param>
        /// <param name="source">The trace source.</param>
        /// <param name="eventType">The severity.</param>
        /// <param name="id">The event id.</param>
        /// <param name="data">The object to trace.</param>
        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
        {
            if (data is LogEntry)
            {
                if (this.Formatter != null)
                {
                    WriteLine(this.Formatter.Format(data as LogEntry));
                }
                else
                {
                    base.TraceData(eventCache, source, eventType, id, data);
                }
            }
            else
            {
                base.TraceData(eventCache, source, eventType, id, data);
            }
        }
        
        /// <summary>
        /// Writes a category name and the value of the object's <see cref="M:System.Object.ToString"></see> method to the listener you create when you implement the <see cref="T:System.Diagnostics.TraceListener"></see> class.
        /// </summary>
        /// <param name="o">An <see cref="T:System.Object"></see> whose fully qualified class name you want to write.</param>
        /// <param name="category">A category name used to organize the output.</param>
        public override void Write(object o, string category)
        {
            if (o is LogEntry)
            {
                if (this.Formatter != null)
                {
                    Write(this.Formatter.Format(o as LogEntry));
                }
                else
                {
                    base.Write(o, category);
                }
            }
            else
            {
                base.Write(o, category);
            }
        }

        /// <summary>
        /// Writes the value of the object's <see cref="M:System.Object.ToString"></see> method to the listener you create when you implement the <see cref="T:System.Diagnostics.TraceListener"></see> class.
        /// </summary>
        /// <param name="o">An <see cref="T:System.Object"></see> whose fully qualified class name you want to write.</param>
        public override void Write(object o)
        {
            if (o is LogEntry)
            {
                if (this.Formatter != null)
                {
                    Write(this.Formatter.Format(o as LogEntry));
                }
                else
                {
                    base.Write(o);
                }
            }
            else
            {
                base.Write(o);
            }
        }        

        /// <summary>
        /// Writes the value of the object's <see cref="M:System.Object.ToString"></see> method to the listener you create when you implement the <see cref="T:System.Diagnostics.TraceListener"></see> class, followed by a line terminator.
        /// </summary>
        /// <param name="o">An <see cref="T:System.Object"></see> whose fully qualified class name you want to write.</param>
        public override void WriteLine(object o)
        {
            if (o is LogEntry)
            {
                if (this.Formatter != null)
                {
                    WriteLine(this.Formatter.Format(o as LogEntry));
                }
                else
                {
                    base.WriteLine(o);
                }
            }
            else
            {
                base.WriteLine(o);
            }
        }

        /// <summary>
        /// Writes a category name and the value of the object's <see cref="M:System.Object.ToString"></see> method to the listener you create when you implement the <see cref="T:System.Diagnostics.TraceListener"></see> class, followed by a line terminator.
        /// </summary>
        /// <param name="o">An <see cref="T:System.Object"></see> whose fully qualified class name you want to write.</param>
        /// <param name="category">A category name used to organize the output.</param>
        public override void WriteLine(object o, string category)
        {
            if (o is LogEntry)
            {
                if (this.Formatter != null)
                {
                    WriteLine(this.Formatter.Format(o as LogEntry));
                }
                else
                {
                    base.WriteLine(o, category);
                }
            }
            else
            {
                base.WriteLine(o, category);
            }
        }

        /// <summary>
        /// Declares "formatter" as a supported attribute name.
        /// </summary>
        /// <returns>
        /// A string array naming the custom attributes supported by the trace listener, or null if there are no custom attributes.
        /// </returns>
        protected override string[] GetSupportedAttributes()
        {
            return new string[1] { "formatter" };
        }
	}
}
