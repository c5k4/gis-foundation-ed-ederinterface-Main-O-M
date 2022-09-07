using System;
using System.Collections;
using System.Collections.Generic;
using System.Json;

#if SILVERLIGHT
namespace Miner.Server.Client.Tasks
#elif WPF
namespace Miner.Mobile.Client.Tasks
#endif
{
    /// <summary>
    ///  An exception specific to tasks. It is raised when the REST endpoint returns an error code. 
    /// </summary>
    public sealed class ServiceException : Exception
    {
        internal ServiceException(int code, string message)
            : base(message)
        {
            this.Details = new List<string>();
            this.Code = code;
        }

        internal static ServiceException CreateFromJson(string json)
        {
            if (json.StartsWith("{\"error", StringComparison.Ordinal))
            {
                JsonObject jsonObject = JsonObject.Parse(json) as JsonObject;
                if (jsonObject != null)
                {
                    JsonArray error = jsonObject["error"] as JsonArray;
                    if (error != null)
                    {
                        int code = 500;
                        string message = "";
                        if (error.ContainsKey("message"))
                        {
                            message = error["message"].ToString();
                        }
                        if (error.ContainsKey("code"))
                        {
                            code = (int)error["code"];
                        }
                        ServiceException exception = new ServiceException(code, message);
                        if (error.ContainsKey("details"))
                        {
                            foreach (object obj2 in error["details"] as IEnumerable)
                            {
                                exception.Details.Add(obj2 as string);
                            }
                        }
                        return exception;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Error code.
        /// </summary>
        public int Code
        {
            get;
            private set;
        }

        /// <summary>
        /// Exception details.
        /// </summary>
        public List<string> Details
        {
            get;
            private set;
        }
    }
}
