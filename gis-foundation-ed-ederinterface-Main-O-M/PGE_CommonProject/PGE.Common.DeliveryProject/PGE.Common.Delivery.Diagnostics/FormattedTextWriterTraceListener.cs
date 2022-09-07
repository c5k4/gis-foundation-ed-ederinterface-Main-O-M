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
using System.Diagnostics;
using System.IO;

using PGE.Common.Delivery.Diagnostics.Formatters;

namespace PGE.Common.Delivery.Diagnostics
{
	/// <summary>
	/// Extends <see cref="TextWriterTraceListener"/> to add formatting capabilities.
	/// </summary>
	public class FormattedTextWriterTraceListener : TextWriterTraceListener
	{
		private ILogFormatter _Formatter;
		
		/// <summary>
		/// Initializes a new instance of <see cref="FormattedTextWriterTraceListener"/>.
		/// </summary>
		public FormattedTextWriterTraceListener()
		{

		}

		/// <summary>
		/// Initializes a new instance of <see cref="FormattedTextWriterTraceListener"/> with a <see cref="ILogFormatter"/>.
		/// </summary>
		/// <param name="formatter">The formatter to format the messages.</param>
		public FormattedTextWriterTraceListener(ILogFormatter formatter)
			: this()
		{
			this.Formatter = formatter;
		}

		/// <summary>
		/// Initializes a new instance of <see cref="FormattedTextWriterTraceListener"/> with a 
		/// <see cref="ILogFormatter"/> and a <see cref="Stream"/>.
		/// </summary>
		/// <param name="stream">The stream to write to.</param>
		/// <param name="formatter">The formatter to format the messages.</param>
		public FormattedTextWriterTraceListener(Stream stream, ILogFormatter formatter)
			: this(stream)
		{
			this.Formatter = formatter;
		}

		/// <summary>
		/// Initializes a new instance of <see cref="FormattedTextWriterTraceListener"/> with a <see cref="Stream"/>.
		/// </summary>
		/// <param name="stream">The stream to write to.</param>
		public FormattedTextWriterTraceListener(Stream stream)
			: base(stream)
		{
			
		}

		/// <summary>
		/// Initializes a new instance of <see cref="FormattedTextWriterTraceListener"/> with a 
		/// <see cref="ILogFormatter"/> and a <see cref="TextWriter"/>.
		/// </summary>
		/// <param name="writer">The writer to write to.</param>
		/// <param name="formatter">The formatter to format the messages.</param>
		public FormattedTextWriterTraceListener(TextWriter writer, ILogFormatter formatter)
			: this(writer)
		{
			this.Formatter = formatter;
		}

		/// <summary>
		/// Initializes a new instance of <see cref="FormattedTextWriterTraceListener"/> with a <see cref="TextWriter"/>.
		/// </summary>
		/// <param name="writer">The writer to write to.</param>
		public FormattedTextWriterTraceListener(TextWriter writer)
			: base(writer)
		{
			
		}

		/// <summary>
		/// Initializes a new instance of <see cref="FormattedTextWriterTraceListener"/> with a 
		/// <see cref="ILogFormatter"/> and a file name.
		/// </summary>
		/// <param name="fileName">The file name to write to.</param>
		/// <param name="formatter">The formatter to format the messages.</param>
		public FormattedTextWriterTraceListener(string fileName, ILogFormatter formatter)
			: this(fileName)
		{
			this.Formatter = formatter;
		}

		/// <summary>
		/// Initializes a new instance of <see cref="FormattedTextWriterTraceListener"/> with a file name.
		/// </summary>
		/// <param name="fileName">The file name to write to.</param>
		public FormattedTextWriterTraceListener(string fileName)
			: base(RootFileNameAndEnsureTargetFolderExists(fileName))
		{
			
		}

		/// <summary>
		/// Initializes a new named instance of <see cref="FormattedTextWriterTraceListener"/> with a 
		/// <see cref="ILogFormatter"/> and a <see cref="Stream"/>.
		/// </summary>
		/// <param name="stream">The stream to write to.</param>
		/// <param name="name">The name.</param>
		/// <param name="formatter">The formatter to format the messages.</param>
		public FormattedTextWriterTraceListener(Stream stream, string name, ILogFormatter formatter)
			: this(stream, name)
		{
			this.Formatter = formatter;
		}

		/// <summary>
		/// Initializes a new named instance of <see cref="FormattedTextWriterTraceListener"/> with a <see cref="Stream"/>.
		/// </summary>
		/// <param name="stream">The stream to write to.</param>
		/// <param name="name">The name.</param>
		public FormattedTextWriterTraceListener(Stream stream, string name)
			: base(stream, name)
		{
			
		}

		/// <summary>
		/// Initializes a new named instance of <see cref="FormattedTextWriterTraceListener"/> with a 
		/// <see cref="ILogFormatter"/> and a <see cref="TextWriter"/>.
		/// </summary>
		/// <param name="writer">The writer to write to.</param>
		/// <param name="name">The name.</param>
		/// <param name="formatter">The formatter to format the messages.</param>
		public FormattedTextWriterTraceListener(TextWriter writer, string name, ILogFormatter formatter)
			: this(writer, name)
		{
			this.Formatter = formatter;
		}

		/// <summary>
		/// Initializes a new named instance of <see cref="FormattedTextWriterTraceListener"/> with a 
		/// <see cref="TextWriter"/>.
		/// </summary>
		/// <param name="writer">The writer to write to.</param>
		/// <param name="name">The name.</param>
		public FormattedTextWriterTraceListener(TextWriter writer, string name)
			: base(writer, name)
		{
			
		}

		/// <summary>
		/// Initializes a new named instance of <see cref="FormattedTextWriterTraceListener"/> with a 
		/// <see cref="ILogFormatter"/> and a file name.
		/// </summary>
		/// <param name="fileName">The file name to write to.</param>
		/// <param name="name">The name.</param>
		/// <param name="formatter">The formatter to format the messages.</param>
		public FormattedTextWriterTraceListener(string fileName, string name, ILogFormatter formatter)
			: this(fileName, name)
		{
            this.Formatter = formatter;
		}

		/// <summary>
		/// Initializes a new named instance of <see cref="FormattedTextWriterTraceListener"/> with a file name.
		/// </summary>
		/// <param name="fileName">The file name to write to.</param>
		/// <param name="name">The name.</param>
		public FormattedTextWriterTraceListener(string fileName, string name)
			: base(RootFileNameAndEnsureTargetFolderExists(fileName), name)
		{
			this.Formatter = _Formatter;			
		}

        /// <summary>
        /// Gets the <see cref="ILogFormatter"/> used to format the trace messages.
        /// </summary>
        public ILogFormatter Formatter
        {
            get
            {
                return this._Formatter;
            }

            set
            {
                this._Formatter = value;
            }
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

        /// <summary>
        /// Roots the file name and ensure target folder exists.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>The file name.</returns>
		private static string RootFileNameAndEnsureTargetFolderExists(string fileName)
		{
			string rootedFileName = fileName;
			if (!Path.IsPathRooted(rootedFileName))
			{
				rootedFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, rootedFileName);
			}

			string directory = Path.GetDirectoryName(rootedFileName);
			if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}

			return rootedFileName;
		}
    }
}
