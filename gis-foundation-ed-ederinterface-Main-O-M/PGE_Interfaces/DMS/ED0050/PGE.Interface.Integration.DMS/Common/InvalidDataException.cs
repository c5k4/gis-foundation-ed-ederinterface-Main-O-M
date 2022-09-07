using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace PGE.Common.Delivery.Framework.Exceptions
{
    /// <summary>
    /// Exception thrown due to bad configuration
    /// </summary>
    [Serializable]
    public class InvalidDataException : Exception
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public InvalidDataException()
        {
        }
        /// <summary>
        /// Constructor with message
        /// </summary>
        /// <param name="message">The error message</param>
        public InvalidDataException(string message)
            : base(message)
        {
        }
        /// <summary>
        /// Constructor with message and inner exception
        /// </summary>
        /// <param name="message">The error message</param>
        /// <param name="innerException">Exception thrown before this one or NULL</param>
        public InvalidDataException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
        /// <summary>
        /// Needed for proper support of serialization.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected InvalidDataException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

    }
}
