using System;

namespace PGnE.Printing
{
    /// <summary>
    /// The event arguments for JobId coming from the print task upon being subitted. 
    /// </summary>
    public class PrintJobSubmittedEventArgs : EventArgs
    {

        public PrintJobSubmittedEventArgs(string jobId)
        {
            JobId = jobId;
        }

        /// <summary>
        /// Property containing the results, used by the attribute viewer. 
        /// </summary>
        public string JobId { get; set; }
    }
}
