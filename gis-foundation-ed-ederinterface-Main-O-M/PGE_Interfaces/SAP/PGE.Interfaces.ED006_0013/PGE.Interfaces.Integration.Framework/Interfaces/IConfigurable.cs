using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace PGE.Interfaces.Integration.Framework
{
    /// <summary>
    /// Defines an object as configurable.
    /// </summary>
    public interface IConfigurable
    {
        /// <summary>
        /// Initialize the object with XML
        /// </summary>
        /// <param name="config">XML Configuration</param>
        /// <returns>true if there were no errors, otherwise false</returns>
        bool Initialize(XmlNode config);
    }
}
