using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using PGE.Common.Delivery.Diagnostics.Formatters;

namespace PGE.Common.Delivery.Diagnostics
{
    /// <summary>
    /// A trace listener that is used to redirect the input to a text box.
    /// </summary>
    public class FormattedTextBoxTraceListener : FormatterTraceListener
    {
        private RichTextBox _TextBox;

        /// <summary>
        /// The delegate used for the adding the log entry to the control.
        /// </summary>
        /// <param name="entry">The log entry.</param>
        private delegate void AppendEntryEventHandler(LogEntry entry);

        /// <summary>
        /// Initializes a new instance of <see cref="FormattedTextBoxTraceListener"/> with a <see cref="ILogFormatter"/>.
        /// </summary>
        /// <param name="formatter">The formatter.</param>
        /// <param name="textbox">The textbox.</param>
        public FormattedTextBoxTraceListener(ILogFormatter formatter, RichTextBox textbox)
        {
            this.Formatter = formatter;

            _TextBox = textbox;
        }

        /// <summary>
        /// Writes the value of the object's <see cref="M:System.Object.ToString"></see> method to the listener you create when you implement the <see cref="T:System.Diagnostics.TraceListener"></see> class.
        /// </summary>
        /// <param name="o">An <see cref="T:System.Object"></see> whose fully qualified class name you want to write.</param>
        public override void Write(object o)
        {
            Write(o, null);
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
                Invoke((LogEntry)o);
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
            WriteLine(o, null);
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
                Invoke((LogEntry)o);                
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
            WriteLine(message);
        }

        /// <summary>
        /// When overridden in a derived class, writes a message to the listener you create in the derived class, followed by a line terminator.
        /// </summary>
        /// <param name="message">A message to write.</param>
        public override void WriteLine(string message)
        {
            LogEntry entry = new LogEntry();
            entry.Message = message;
            entry.Severity = TraceEventType.Verbose;

            Invoke(entry);
        }

        /// <summary>
        /// Invokes the specified entry.
        /// </summary>
        /// <param name="entry">The entry.</param>
        private void Invoke(LogEntry entry)
        {
            try
            {
                if (_TextBox.InvokeRequired)
                    _TextBox.Invoke(new AppendEntryEventHandler(AppendEntry), entry);
                else
                    AppendEntry(entry);
            }
            catch
            {
                
            }            
        }

        /// <summary>
        /// Appends the entry.
        /// </summary>
        /// <param name="entry">The entry.</param>
        private void AppendEntry(LogEntry entry)
        {
            Color color = GetForeColor(entry.Severity);
            string message = (this.Formatter != null) ? this.Formatter.Format(entry) : null;

            _TextBox.AppendText(message);
            _TextBox.AppendText(Environment.NewLine);

            string[] msgs = message.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string msg in msgs)
            {
                int startIndex = _TextBox.Text.LastIndexOf(msg);
                if (startIndex == -1) continue;

                // The metacharacters that must be escaped.
                string[] meta = new string[] { "\\", "^", "[", "$", "{", "*", "(", "+", ")", "|", "?", "<", ">" };

                string input = msg;
                foreach (string s in meta)
                    input = input.Replace(s, "\\" + s);

                string line = _TextBox.Text.Substring(startIndex, msg.Length);
                Regex regex = new Regex(input, RegexOptions.Compiled);
                for (Match match = regex.Match(line); match.Success; match = match.NextMatch())
                {
                    _TextBox.SelectionStart = startIndex + match.Index;
                    _TextBox.SelectionLength = match.Length;
                    _TextBox.SelectionColor = color;
                }
            }

            ScrollToEnd(_TextBox.Handle);
        }
        
        /// <summary>
        /// Gets the color of the fore.
        /// </summary>
        /// <param name="severity">The severity.</param>
        /// <returns></returns>
        private Color GetForeColor(TraceEventType severity)
        {
            switch (severity)
            {
                case TraceEventType.Critical:
                case TraceEventType.Error:
                    return Color.Red;
                case TraceEventType.Information:
                case TraceEventType.Verbose:
                    return Color.Gray;
                case TraceEventType.Warning:
                    return Color.Yellow;
                default:
                    return Color.White;
            }
        }

        /// <summary>
        /// Scrolls the textbox to the end
        /// </summary>
        /// <param name="handle">The handle of the textbox</param>
        private void ScrollToEnd(IntPtr handle)
        {
            const int WM_VSCROLL = 277;
            const int SB_BOTTOM = 7;

            IntPtr ptrWparam = new IntPtr(SB_BOTTOM);
            IntPtr ptrLparam = new IntPtr(0);
            SendMessage(handle, WM_VSCROLL, ptrWparam, ptrLparam);
        }

        /// <summary>
        /// Sends the message.
        /// </summary>
        /// <param name="hWnd">The h WND.</param>
        /// <param name="Msg">The MSG.</param>
        /// <param name="wParam">The w param.</param>
        /// <param name="lParam">The l param.</param>
        /// <returns></returns>
        [System.Runtime.InteropServices.DllImport("User32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, EntryPoint = "SendMessage")]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);    
    }
}
