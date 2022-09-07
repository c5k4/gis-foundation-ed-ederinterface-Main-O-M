using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Win32;

namespace PGE.Common.Delivery.Systems.Configuration
{
    /// <summary>
    /// The user registry used to obtain the configuration information specific to the environment.
    /// </summary>
    public class UserRegistry : SystemRegistry
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UserRegistry"/> class.
        /// </summary>
        /// <param name="subKey">The sub key.</param>
        public UserRegistry(string subKey)
            : base(Registry.CurrentUser.CreateSubKey(SystemRegistry.HKEY).CreateSubKey(subKey))
        {
        }

        #endregion
    }
}