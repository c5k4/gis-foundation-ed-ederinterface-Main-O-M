using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TLM.Common
{
    public class ExceptionLog
    {
        public string Path { get; set; }
        public string ClassName { get; set; }
        public string MethodName { get; set; }
        public string ProjectName { get; set; }
        public string SolutionName { get; set; }
        public string UserName { get; set; }
        public string ErrorMessage { get; set; }
        public string StackTrace { get; set; }
        public string InnerException { get; set; }
        public string ErrorTime { get; set; }
        public string GlobalId { get; set; }
    }
}