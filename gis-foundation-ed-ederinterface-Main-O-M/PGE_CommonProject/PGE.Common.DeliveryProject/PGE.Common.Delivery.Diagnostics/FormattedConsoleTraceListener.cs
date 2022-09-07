using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

using PGE.Common.Delivery.Diagnostics.Formatters;

namespace PGE.Common.Delivery.Diagnostics
{
    /// <summary>
    /// Represents a FormattedConsoleTraceListener
    /// </summary>
    public class FormattedConsoleTraceListener : FormatterTraceListener
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FormattedConsoleTraceListener"/> class.
        /// </summary>
        public FormattedConsoleTraceListener()
            : this(null)
        {

        }

        /// <summary>
		/// Initializes a new instance of <see cref="FormattedConsoleTraceListener"/> with a <see cref="ILogFormatter"/>.
		/// </summary>
		/// <param name="formatter">The formatter.</param>
        public FormattedConsoleTraceListener(ILogFormatter formatter)
        {
            this.Formatter = formatter;            
		}


        /// <summary>
        /// Writes the value of the object's <see cref="M:System.Object.ToString"></see> method to the listener you create when you implement the <see cref="T:System.Diagnostics.TraceListener"></see> class.
        /// </summary>
        /// <param name="o">An <see cref="T:System.Object"></see> whose fully qualified class name you want to write.</param>
        public override void Write(object o)
        {
            if (o is LogEntry)
            {
                SetForeground(((LogEntry)o).Severity);

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
        /// Writes a category name and the value of the object's <see cref="M:System.Object.ToString"></see> method to the listener you create when you implement the <see cref="T:System.Diagnostics.TraceListener"></see> class.
        /// </summary>
        /// <param name="o">An <see cref="T:System.Object"></see> whose fully qualified class name you want to write.</param>
        /// <param name="category">A category name used to organize the output.</param>
        public override void Write(object o, string category)
        {
            if (o is LogEntry)
            {
                SetForeground(((LogEntry)o).Severity);

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
        /// Writes the value of the object's <see cref="M:System.Object.ToString"></see> method to the listener you create when you implement the <see cref="T:System.Diagnostics.TraceListener"></see> class, followed by a line terminator.
        /// </summary>
        /// <param name="o">An <see cref="T:System.Object"></see> whose fully qualified class name you want to write.</param>
        public override void WriteLine(object o)
        {
            if (o is LogEntry)
            {
                SetForeground(((LogEntry)o).Severity);

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
                SetForeground(((LogEntry)o).Severity);

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
        /// When overridden in a derived class, writes the specified message to the listener you create in the derived class.
        /// </summary>
        /// <param name="message">A message to write.</param>
        public override void Write(string message)
        {
            Console.Out.Write(message);
        }        
        
        /// <summary>
        /// When overridden in a derived class, writes a message to the listener you create in the derived class, followed by a line terminator.
        /// </summary>
        /// <param name="message">A message to write.</param>
        public override void WriteLine(string message)
        {            
            Console.Out.WriteLine(message);
        }

        /// <summary>
        /// Sets the foreground.
        /// </summary>
        /// <param name="severity">The severity.</param>
        protected void SetForeground(TraceEventType severity)
        {
            Console.ResetColor();

            switch (severity)
            {
                case TraceEventType.Critical:
                case TraceEventType.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case TraceEventType.Information:                
                case TraceEventType.Verbose:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
                case TraceEventType.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case TraceEventType.Resume:
                case TraceEventType.Start:
                case TraceEventType.Stop:
                case TraceEventType.Suspend:
                case TraceEventType.Transfer:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                default:
                    break;
            }
        }
    }
}
