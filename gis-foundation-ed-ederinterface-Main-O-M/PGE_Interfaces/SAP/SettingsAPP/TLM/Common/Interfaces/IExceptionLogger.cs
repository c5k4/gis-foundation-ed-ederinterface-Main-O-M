using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TLM.Common.Interfaces
{
    interface IExceptionLogger
    {
        void LogExceptionInFile(ExceptionLog ExcLog);  
    }
}
