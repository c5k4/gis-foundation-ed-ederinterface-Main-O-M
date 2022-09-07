using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PGE.Common.Delivery.Systems.Configuration
{
    /// <summary>
    /// An interface used to handle user/machine level configuration settings.
    /// </summary>
    public interface IConfigurationSettings
    {
        #region Properties

        /// <summary>
        /// Gets a value indicating whether this instance is valid.
        /// </summary>
        /// <value><c>true</c> if this instance is valid; otherwise, <c>false</c>.</value>
        bool Valid
        {
            get;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Determines whether the configuration with the specified name exist.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>
        /// 	<c>true</c> if the configuration with the specified name exist; otherwise, <c>false</c>.
        /// </returns>
        bool Contains(string name);

        /// <summary>
        /// Gets the setting for the specified name.
        /// </summary>
        /// <typeparam name="T">The type of the return value.</typeparam>
        /// <param name="name">The name.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The option in the type specified.</returns>
        T GetSetting<T>(string name, T defaultValue);

        /// <summary>
        /// Gets the sub key names for the specified name.
        /// </summary>
        /// <returns>The array of sub key names.</returns>
        string[] GetSubKeyNames();

        /// <summary>
        /// Gets the sub key names for the specified name.
        /// </summary>
        /// <returns>The array of sub key names.</returns>
        string[] GetValueNames();

        /// <summary>
        /// Saves the settings.
        /// </summary>
        void Save();

        /// <summary>
        /// Sets the setting using the specified name and value.
        /// </summary>
        /// <typeparam name="T">The type of the option value</typeparam>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        void SetSetting<T>(string name, T value);

        #endregion
    }
}