using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace PGE.BatchApplication.AUConveyor.Utilities
{
    /// <summary>
    /// Manages writing to various loggers and writers within the application.
    /// </summary>
    internal static class LogManager
    {
        internal static CustomTextWriterTraceListener FileLogger = new CustomTextWriterTraceListener("Output.log", "AUConveyorLog");
        internal static CustomConsoleTraceListener ConsoleLogger = new CustomConsoleTraceListener();

        internal static List<TraceListener> Listeners = new List<TraceListener>();

        /// <summary>
        /// Adds the default console logger to the manager.
        /// </summary>
        internal static void AddConsoleLogger()
        {
            Listeners.Add(ConsoleLogger);
        }

        /// <summary>
        /// Adds the default file logger to the manager
        /// </summary>
        /// <param name="outputFilePrefix">The string to be prepended to the file name of the log. Usually used for child processes.</param>
        internal static void AddFileLogger(string outputFilePrefix)
        {
            FileLogger = new CustomTextWriterTraceListener((!string.IsNullOrEmpty(outputFilePrefix) ? outputFilePrefix + "." : "") + "Output.log", "AUConveyorLog");
            FileLogger.TraceOutputOptions |= TraceOptions.Timestamp;
            Listeners.Add(FileLogger);
        }

        /// <summary>
        /// Writes a message to all listeners.
        /// </summary>
        /// <param name="message">The message to write.</param>
        internal static void Write(string message)
        {
            foreach(TraceListener tl in Listeners)
                tl.Write(message);
        }

        /// <summary>
        /// Writes a message to all listeners. The console colors will be displayed as specified; all
        /// other listeners will still write sans color options.
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="textColor">The console foreground color, or <c>null</c> to use the default color.</param>
        /// <param name="backColor">The console background color, or <c>null</c> to use the default color.</param>
        internal static void Write(string message, ConsoleColor? textColor, ConsoleColor? backColor)
        {
            if (textColor.HasValue)
                Console.ForegroundColor = textColor.Value;
            if (backColor.HasValue)
                Console.BackgroundColor = backColor.Value;

            Write(message);

            Console.ResetColor();
        }

        /// <summary>
        /// Writes a message and a line terminator to all listeners.
        /// </summary>
        /// <param name="message">The message to write.</param>
        internal static void WriteLine()
        {
            WriteLine(string.Empty);
        }

        /// <summary>
        /// Writes a line terminator to all listeners.
        /// </summary>
        internal static void WriteLine(string message)
        {
            foreach (TraceListener tl in Listeners)
                tl.WriteLine(message);
        }

        /// <summary>
        /// Writes a message and a line terminator to all listeners. The console colors will be 
        /// displayed as specified; all other listeners will still write sans color options.
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="textColor">The console foreground color, or <c>null</c> to use the default color.</param>
        /// <param name="backColor">The console background color, or <c>null</c> to use the default color.</param>
        internal static void WriteLine(string message, ConsoleColor? textColor, ConsoleColor? backColor)
        {
            if (textColor.HasValue)
                Console.ForegroundColor = textColor.Value;
            if (backColor.HasValue)
                Console.BackgroundColor = backColor.Value;

            WriteLine(message);

            Console.ResetColor();
        }
    }

    #region Custom Trace Listeners
    /// <summary>
    /// Writes output to the text logs.
    /// </summary>
    internal class CustomTextWriterTraceListener : TextWriterTraceListener
    {
        public CustomTextWriterTraceListener(string fileName, string name) : base(fileName, name)
        {
        }

        private void MessageAppend(ref string message)
        {
            if (((int)TraceOutputOptions | (int)TraceOptions.Timestamp) == (int)TraceOutputOptions && !string.IsNullOrEmpty(message))
                message = "[" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "] " + message;
        }

        public override void Write(object o)
        {
            Write(o.ToString());
        }

        public override void Write(string message)
        {
            MessageAppend(ref message);

            base.Write(message);
            this.Flush();
        }

        public override void Write(object o, string category)
        {
            Write(o.ToString(), category);
        }

        public override void Write(string message, string category)
        {
            MessageAppend(ref message);

            base.Write(message, category);
            this.Flush();
        }

        public override void WriteLine(object o)
        {
            WriteLine(o.ToString());
        }

        public override void WriteLine(string message)
        {
            MessageAppend(ref message);

            base.WriteLine(message);
            this.Flush();
        }

        public override void WriteLine(object o, string category)
        {
            WriteLine(o.ToString(), category);
        }

        public override void WriteLine(string message, string category)
        {
            MessageAppend(ref message);

            base.WriteLine(message, category);
            this.Flush();
        }
    }

    /// <summary>
    /// Writes output to the console.
    /// </summary>
    internal class CustomConsoleTraceListener : ConsoleTraceListener
    {
        public CustomConsoleTraceListener() : base(false)
        {
        }

        private void MessageAppend(ref string message)
        {
            if (((int)TraceOutputOptions | (int)TraceOptions.Timestamp) == (int)TraceOutputOptions && !string.IsNullOrEmpty(message))
                message = "[" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "] " + message;
        }

        public override void Write(object o)
        {
            Write(o.ToString());
        }

        public override void Write(string message)
        {
            MessageAppend(ref message);

            base.Write(message);
            this.Flush();
        }

        public override void Write(object o, string category)
        {
            Write(o.ToString(), category);
        }

        public override void Write(string message, string category)
        {
            MessageAppend(ref message);

            base.Write(message, category);
            this.Flush();
        }

        public override void WriteLine(object o)
        {
            WriteLine(o.ToString());
        }

        public override void WriteLine(string message)
        {
            MessageAppend(ref message);

            base.WriteLine(message);
            this.Flush();
        }

        public override void WriteLine(object o, string category)
        {
            WriteLine(o.ToString(), category);
        }

        public override void WriteLine(string message, string category)
        {
            MessageAppend(ref message);

            base.WriteLine(message, category);
            this.Flush();
        }
    }
    #endregion
}
