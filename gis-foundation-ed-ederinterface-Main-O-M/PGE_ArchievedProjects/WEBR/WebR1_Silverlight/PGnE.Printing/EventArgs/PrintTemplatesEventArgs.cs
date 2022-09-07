using System;
using System.Collections.Generic;

namespace PGnE.Printing
{
    /// <summary>
    /// The event arguments for JobId coming from the print task upon being subitted. 
    /// </summary>
    public class PrintTemplatesEventArgs : EventArgs
    {

        public PrintTemplatesEventArgs(IEnumerable<string> printTemplates)
        {
            PrintTemplates = printTemplates;
        }

        /// <summary>
        /// Property containing the results, used by the attribute viewer. 
        /// </summary>
        public IEnumerable<string> PrintTemplates { get; set; }
    }
}
