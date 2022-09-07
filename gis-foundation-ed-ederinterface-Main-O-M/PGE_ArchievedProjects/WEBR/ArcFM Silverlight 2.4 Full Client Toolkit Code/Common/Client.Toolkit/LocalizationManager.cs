using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
#if WPF
using Xceed.Wpf.Toolkit;
#endif

#if SILVERLIGHT
namespace Miner.Server.Client.Toolkit
#elif WPF
namespace Miner.Mobile.Client.Toolkit
#endif
{
    /// <summary>
    /// Utility class to pull resources out of XML and displays them. 
    /// </summary>
    ///<exclude/>   
    public class LocalizationManager
    {
        private static readonly Dictionary<string, ResourceManager> _resourceManagers =
            new Dictionary<string, ResourceManager>(1);

        private static volatile object _padlock = new object();

        #region Dependency Properties

        public static readonly DependencyProperty ResourceKeyProperty = DependencyProperty.RegisterAttached(
            "ResourceKey",
            typeof(string),
            typeof(LocalizationManager),
            new PropertyMetadata(null, new PropertyChangedCallback(LocalizationManager.OnResourceKeyChanged)));

        #endregion Dependency Properties

        #region Public Static Properties

        /// <summary>
        /// Static property to provide a default CultureInfo
        /// </summary>
        public static CultureInfo CultureInfo { get; set; }

        /// <summary>
        /// Utility static property to return an instance of myself.
        /// </summary>
        public static LocalizationManager Manager { get; set; }

        /// <summary>
        /// Utility static property to return an instance of the ResourceManager
        /// </summary>
        public static ResourceManager ResourceManager { get; set; }

        #endregion Public Static Properties

        #region Public Static Methods

        /// <summary>
        /// Returns a key based on a DependencyObject
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static string GetResourceKey(DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return (element.GetValue(ResourceKeyProperty) as string);
        }

        /// <summary>
        /// Adds a ResourceKey
        /// </summary>
        /// <param name="element"></param>
        /// <param name="value"></param>
        public static void SetResourceKey(DependencyObject element, string value)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            element.SetValue(ResourceKeyProperty, value);
        }

        /// <summary>
        /// Returns a resource based on the key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetString(string key)
        {
            LocalizationManager localizationManager = Manager;
            if (localizationManager != null)
            {
                return localizationManager.GetStringOverride(key);
            }
            return GetString(key, CultureInfo, ResourceManager ?? GetResourceManager(Assembly.GetCallingAssembly()));
        }

        #endregion Public Static Methods

        #region Public Methods

        public virtual string GetStringOverride(string key)
        {
            return GetString(key, CultureInfo, ResourceManager ?? GetResourceManager(Assembly.GetCallingAssembly()));
        }

        #endregion Public Methods

        #region Property Changed Callbacks

        [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        private static void OnResourceKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            string resourceKey = e.NewValue as string;
            if (resourceKey != null)
            {
                string resourceData = GetString(resourceKey);
                if (string.IsNullOrEmpty(resourceData) == false)
                {
                    resourceData = Regex.Replace(resourceData, "_(?!_)", string.Empty);
                    if (d is TextBox)
                    {
                        (d as TextBox).Text = resourceData;
                    }
                    else if (d is TextBlock)
                    {
                        (d as TextBlock).Text = resourceData;
                    }
                    else if (d is BusyIndicator)
                    {
                        (d as BusyIndicator).BusyContent = resourceData;
                    }
                    else if (d is ContentControl)
                    {
                        (d as ContentControl).Content = resourceData;
                    }
                    else if (d is ContentPresenter)
                    {
                        (d as ContentPresenter).Content = resourceData;
                    }
                    else if (d is HeaderedContentControl)
                    {
                        (d as HeaderedContentControl).Header = resourceData;
                    }
                    else if (d is HeaderedItemsControl)
                    {
                        (d as HeaderedItemsControl).Header = resourceData;
                    }
                    else if (d is DiscreteObjectKeyFrame)
                    {
                        (d as DiscreteObjectKeyFrame).Value = resourceData;
                    }
#if SILVERLIGHT
                    else if (d is Setter)
                    {
                        (d as Setter).Value = resourceData;
                    }
#endif
                }
            }
        }

        #endregion Property Changed Callbacks

        #region Private Static Methods

        public static ResourceManager GetResourceManager(Assembly assembly)
        {
            ResourceManager manager = null;
            string key = "Properties.Resources";
            string fullKey = assembly.FullName.Split(',')[0] + '.' + key;
            if (!_resourceManagers.TryGetValue(fullKey, out manager))
            {
                lock (_padlock)
                {
                    if (!_resourceManagers.TryGetValue(fullKey, out manager))
                    {
                        manager = new ResourceManager(fullKey, assembly);
                        _resourceManagers[fullKey] = manager;
                    }
                }
            }
            return manager;
        }

        private static string GetString(string key, CultureInfo culture, ResourceManager resourceManager)
        {
            string data = null;
            if (resourceManager != null)
            {
                try
                {
                    data = (culture == null) ? resourceManager.GetString(key) : resourceManager.GetString(key, culture);
                }
                catch (InvalidOperationException)
                {
                }
                catch (MissingManifestResourceException)
                {
                }
            }
            return data;
        }

        #endregion Private Static Methods
    }
}
