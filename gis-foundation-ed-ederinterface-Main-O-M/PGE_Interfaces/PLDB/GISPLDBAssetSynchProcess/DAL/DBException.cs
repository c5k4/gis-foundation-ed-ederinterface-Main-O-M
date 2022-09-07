using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GISPLDBAssetSynchProcess.DAL
{
    public class DBException : SystemException
    {
        public string ErrorCode = "UnknownDBErrorCode";
        public DBException(string message): base(message)
        {
        }

        public DBException(string message, string errorCode)
            : base(message)
        {
            this.ErrorCode = errorCode;
        }
        public DBException(string message, string errorCode, Exception innerException)
            : base(message, innerException)
        {
            this.ErrorCode = errorCode;
        }
    }
}
