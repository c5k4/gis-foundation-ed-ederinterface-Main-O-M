using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Microsoft.Win32;

using PGE.Common.Delivery.Systems.Reflection;

namespace PGE.Common.Delivery.Systems.Configuration
{
    /// <summary>
    /// The local machine registry used to obtain the configuration information specific to the environment.
    /// </summary>
    public class SystemRegistry : IConfigurationSettings
    {
        #region Fields

        /// <summary>
        /// The base root key for all of the products.
        /// </summary>
        internal const string HKEY = @"Software\Miner and Miner";

        /// <summary>
        /// The underlying store key of the configuration settings.
        /// </summary>
        protected RegistryKey _RegistryKey;

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SystemRegistry"/> class.
        /// </summary>
        public SystemRegistry()
        {
            _RegistryKey = Registry.LocalMachine.OpenSubKey(SystemRegistry.HKEY, false);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="SystemRegistry"/> class.
        /// </summary>
        /// <param name="subKey">The sub key.</param>
        public SystemRegistry(string subKey)
        {
            _RegistryKey = Registry.LocalMachine.OpenSubKey(string.Format("{0}\\{1}", SystemRegistry.HKEY, subKey), false);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemRegistry"/> class.
        /// </summary>
        /// <param name="registryKey">The registry key.</param>
        protected SystemRegistry(RegistryKey registryKey)
        {
            _RegistryKey = registryKey;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the ROOT install directory from Rgistry. If the Directory Registry entry is not found will return the assembly location
        /// </summary>
        public string Directory
        {
            get
            {
                AssemblyInfo info = new AssemblyInfo(this.GetType());
                string path = this.GetSetting("Directory", info.Directory);
                return path;
            }
        }

        /// <summary>
        /// Gets the Config path from Registry.
        /// If the Config Registry entry is not found will return the same value as the Directory Property.
        /// </summary>
        public string ConfigPath
        {
            get
            {
                string path = this.GetSetting("Config", Directory);
                return path;
            }
        }
        /// <summary>
        /// Gets the product code.
        /// </summary>
        public string ProductCode
        {
            get
            {
                return this.GetSetting("ProductCode", string.Empty);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is valid.
        /// </summary>
        /// <value><c>true</c> if this instance is valid; otherwise, <c>false</c>.</value>
        public bool Valid
        {
            get
            {
                return (_RegistryKey != null);
            }
        }

        /// <summary>
        /// Gets the version.
        /// </summary>
        public string Version
        {
            get
            {
                return this.GetSetting("Version", string.Empty);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Determines whether the configuration with the specified name exist.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>
        /// 	<c>true</c> if the configuration with the specified name exist; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(string name)
        {
            if (!this.Valid) return false;

            return _RegistryKey.GetValueNames().Contains(name);
        }

        /// <summary>
        /// Gets the setting for the specified name.
        /// </summary>
        /// <typeparam name="T">The type of the return value.</typeparam>
        /// <param name="name">The name.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The option in the type specified.</returns>
        public T GetSetting<T>(string name, T defaultValue)
        {
            if (!this.Valid) return defaultValue;

            return TypeCastFacade.Cast(_RegistryKey.GetValue(name), defaultValue);
        }

        /// <summary>
        /// Gets the sub key names for the specified name.
        /// </summary>
        /// <returns>The array of sub key names.</returns>
        public string[] GetSubKeyNames()
        {
            if (!this.Valid)
                return new string[0];

            return _RegistryKey.GetSubKeyNames();
        }

        /// <summary>
        /// Gets the sub key names for the specified name.
        /// </summary>
        /// <returns>The array of sub key names.</returns>
        public string[] GetValueNames()
        {
            if (!this.Valid)
                return new string[0];

            return _RegistryKey.GetValueNames();
        }

        /// <summary>
        /// Saves the settings.
        /// </summary>
        public void Save()
        {
            if (!this.Valid) return;

            _RegistryKey.Flush();
        }

        /// <summary>
        /// Sets the setting using the specified name and value.
        /// </summary>
        /// <typeparam name="T">The type of the option value</typeparam>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public void SetSetting<T>(string name, T value)
        {
            if (!this.Valid) return;

            _RegistryKey.SetValue(name, value);
        }

        #endregion

        #region Nested Types

        /// <summary>
        /// A struct that represents a registry hive location for each installed product.
        /// </summary>
        public struct Hive
        {
            #region Fields

            /// <summary>
            /// Bin Folder.
            /// </summary>
            public const string Bin = "Bin";

            /// <summary>
            /// Config Folder.
            /// </summary>
            public const string Config = "Config";

            /// <summary>
            /// The desktop auto updaters registry hive key.
            /// </summary>
            public const string AutoUpdaters = Desktop + "\\AutoUpdaters";

            /// <summary>
            /// The desktop registry hive name
            /// </summary>
            public const string Desktop = "Desktop";

            /// <summary>
            /// The SessionManager registry hive name
            /// </summary>
            public const string SessionManager = Desktop + "\\SessionManager";

            /// <summary>
            /// The Designer registry hive name
            /// </summary>
            public const string Designer = Desktop + "\\Designer";

            /// <summary>
            /// The WorkflowManager registry hive name
            /// </summary>
            public const string WorkflowManager = Desktop + "\\WorkflowManager";

            /// <summary>
            /// The mobile registry hive name.
            /// </summary>
            public const string Mobile = "Mobile";

            /// <summary>
            /// The shared registry hive key.
            /// </summary>
            public const string Shared = "Shared";

            #endregion
        }

        #endregion
    }
}