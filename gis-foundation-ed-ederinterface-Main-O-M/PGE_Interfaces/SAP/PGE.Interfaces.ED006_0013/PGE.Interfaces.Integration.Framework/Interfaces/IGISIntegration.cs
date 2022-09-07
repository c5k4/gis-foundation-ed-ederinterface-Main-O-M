using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PGE.Interfaces.Integration.Framework
{
    /// <summary>
    /// Interface to manage the Integration
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IGISIntegration<T>
    {
        /// <summary>
        /// Event fired when the Initialize method is called
        /// </summary>
        event EventHandler OnInitialize;
        /// <summary>
        /// Run any code needed to initialize the class
        /// </summary>
        /// <returns>True if the class successfully initialized otherwise false</returns>
        bool Initialize();
        /// <summary>
        /// Primary method used to begin the data extraction and transformation
        /// </summary>
        /// <returns>The transformed data</returns>
        bool Process(); 
        //Better if C# allowed generics in Property
        //Since this will be just a Read-Only Property this is not bad
        /// <summary>
        /// Store any non fatal errors that occurred
        /// </summary>
        /// <returns>Any Errors that occurred during processing</returns>
        List<Exception> ErrorMessages();
    }
}
