using System;
using System.Runtime.Serialization;

namespace PGE.Common.CommandFlags
{
    public class FlagException : Exception
    {
        public FlagException()
        {
        }

        public FlagException(string message)
            : base(message)
        {
        }

        public FlagException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public FlagException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}