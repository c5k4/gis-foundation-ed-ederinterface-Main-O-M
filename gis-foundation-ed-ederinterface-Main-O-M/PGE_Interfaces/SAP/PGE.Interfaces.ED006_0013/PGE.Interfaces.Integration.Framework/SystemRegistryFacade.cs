using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;

using Microsoft.Win32;
using Telvent.PGE.Framework;


namespace Telvent.PGE.Framework
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
        internal const string HKEY = @"Software\Miner and Miner\PGE";

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
        /// Gets the directory.
        /// </summary>
        public string Directory
        {
            get
            {
                AssemblyInformation info = new AssemblyInformation(this.GetType());
                string path = this.GetSetting("Directory", info.Directory);
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
    }
}