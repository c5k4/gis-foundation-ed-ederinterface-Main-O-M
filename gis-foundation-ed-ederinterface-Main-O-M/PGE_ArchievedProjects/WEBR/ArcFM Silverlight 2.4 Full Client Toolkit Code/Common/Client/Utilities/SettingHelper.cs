using System;
using System.IO.IsolatedStorage;
using System.Text;
#if WPF
using System.Collections.Generic;
using System.IO;
#endif

#if SILVERLIGHT
namespace Miner.Server.Client
#elif WPF
namespace Miner.Mobile.Client
#endif
{
    public static class SettingHelper
    {
#if WPF
        private const string AppSettingsPath = "AppSettings.log";

        private static Dictionary<string, string> _appSettings;
#endif

        /// <summary>
        /// Write a setting to the isolated storage.
        /// </summary>
        /// <param name="section">Section (or tool) name</param>
        /// <param name="item">Item (or property) name</param>
        /// <param name="attribute">Attribute name</param>
        /// <param name="value">Setting value</param>
        public static void WriteSetting(string section, string item, string attribute, object value)
        {
#if SILVERLIGHT
            WriteSettingToStorage(Concat(new string[] { section, item, attribute }), value);
#elif WPF
            WriteSettingToStorage(Concat(new string[] { section, item, attribute }), value.ToString());
#endif
        }

        /// <summary>
        /// Read a setting from the isolated storage.
        /// </summary>
        /// <param name="section">Section (or tool) name</param>
        /// <param name="item">Item (or property) name</param>
        /// <param name="attribute">Attribute name</param>
        /// <param name="dataType">Data type of the setting for the validation purpose</param>
        /// <returns>Setting value</returns>
#if SILVERLIGHT
        public static object ReadSetting(string section, string item, string attribute, string dataType = null)
#elif WPF
        public static object ReadSetting(string section, string item, string attribute, string dataType)
#endif
        {
            var settingKey = Concat(new string[] { section, item, attribute });
            var setting = ReadSettingFromStorage(settingKey);
            if (setting == null) return null;

            if (!string.IsNullOrEmpty(dataType))
            {
                var type = Type.GetType(dataType);
#if SILVERLIGHT
                if (type != null && setting.GetType() != type)
                {
                    RemoveSettingFromStorage(settingKey);

                    return null;
                }
            }

            return setting;
#elif WPF
                if (type != null)
                {
                    try
                    {
                        if (type.IsPrimitive)
                        {
                            if (type == typeof(bool))
                            {
                                return bool.Parse(setting);
                            }
                            else if (type == typeof(byte))
                            {
                                return byte.Parse(setting);
                            }
                            else if (type == typeof(sbyte))
                            {
                                return sbyte.Parse(setting);
                            }
                            else if (type == typeof(short))
                            {
                                return short.Parse(setting);
                            }
                            else if (type == typeof(ushort))
                            {
                                return ushort.Parse(setting);
                            }
                            else if (type == typeof(int))
                            {
                                return int.Parse(setting);
                            }
                            else if (type == typeof(uint))
                            {
                                return uint.Parse(setting);
                            }
                            else if (type == typeof(long))
                            {
                                return long.Parse(setting);
                            }
                            else if (type == typeof(ulong))
                            {
                                return ulong.Parse(setting);
                            }
                            else if (type == typeof(char))
                            {
                                return char.Parse(setting);
                            }
                            else if (type == typeof(double))
                            {
                                return double.Parse(setting);
                            }
                            else if (type == typeof(float))
                            {
                                return float.Parse(setting);
                            }
                            else if (type == typeof(decimal))
                            {
                                return decimal.Parse(setting);
                            }
                        }
                        else if (type.IsEnum)
                        {
                            return Enum.Parse(type, setting);
                        }
                        else
                        {
                            if (type == typeof(string))
                            {
                                return setting;
                            }
                            else if (type == typeof(DateTime))
                            {
                                return DateTime.Parse(setting);
                            }
                            else
                            {
                                RemoveSettingFromStorage(settingKey);
                            }
                        }
                    }
                    catch
                    {
                        RemoveSettingFromStorage(settingKey);
                    }
                }
            }

            return null;
#endif
        }

#if SILVERLIGHT
        private static void WriteSettingToStorage(string settingKey, object settingValue)
        {
            var settings = IsolatedStorageSettings.ApplicationSettings;
            if (settings.Contains(settingKey))
            {
                settings[settingKey] = settingValue;
            }
            else
            {
                settings.Add(settingKey, settingValue);
            }
        }

        private static object ReadSettingFromStorage(string settingKey)
        {
            var settings = IsolatedStorageSettings.ApplicationSettings;

            return settings.Contains(settingKey) ? settings[settingKey] : null;
        }
#elif WPF
        private static void WriteSettingToStorage(string settingKey, string settingValue)
        {
            if (!InitializeSettings()) return;

            if (_appSettings.ContainsKey(settingKey))
            {
                _appSettings[settingKey] = settingValue;
                OverwriteSettings();
            }
            else
            {
                _appSettings.Add(settingKey, settingValue);
                AppendSetting(settingKey, settingValue);
            }
        }

        private static string ReadSettingFromStorage(string settingKey)
        {
            if (!InitializeSettings()) return null;

            return _appSettings.ContainsKey(settingKey) ? _appSettings[settingKey] : null;
        }

        private static bool InitializeSettings()
        {
            if (!IsolatedStorageFile.IsEnabled) return false;

            using (var store = GetUserStore())
            {
                if (_appSettings == null)
                {
                    _appSettings = new Dictionary<string, string>();
                    using (var reader = new StreamReader(store.OpenFile(AppSettingsPath, FileMode.OpenOrCreate, FileAccess.Read)))
                    {
                        while (!reader.EndOfStream)
                        {
                            var key = reader.ReadLine();
                            if (reader.EndOfStream) break;

                            var value = reader.ReadLine();
                            _appSettings.Add(key, value);
                        }
                    }
                }
            }

            return true;
        }

        private static bool OverwriteSettings()
        {
            if (!IsolatedStorageFile.IsEnabled) return false;

            using (var store = GetUserStore())
            {
                using (var writer = new StreamWriter(store.OpenFile(AppSettingsPath, FileMode.Truncate, FileAccess.Write)))
                {
                    foreach (var key in _appSettings.Keys)
                    {
                        writer.WriteLine(key);
                        writer.WriteLine(_appSettings[key]);
                    }
                }
            }

            return true;
        }

        private static bool AppendSetting(string settingKey, string settingValue)
        {
            if (!IsolatedStorageFile.IsEnabled) return false;

            using (var store = GetUserStore())
            {
                using (var writer = new StreamWriter(store.OpenFile(AppSettingsPath, FileMode.Append, FileAccess.Write)))
                {
                    writer.WriteLine(settingKey);
                    writer.WriteLine(settingValue);
                }
            }

            return true;
        }

        private static IsolatedStorageFile GetUserStore()
        {
            return AppDomain.CurrentDomain.ActivationContext != null ? IsolatedStorageFile.GetUserStoreForApplication() : IsolatedStorageFile.GetUserStoreForAssembly();
        }
#endif

        private static bool RemoveSettingFromStorage(string settingKey)
        {
#if SILVERLIGHT
            var settings = IsolatedStorageSettings.ApplicationSettings;

            return settings.Contains(settingKey) ? settings.Remove(settingKey) : false;
#elif WPF
            if (!InitializeSettings()) return false;
            if (!_appSettings.ContainsKey(settingKey)) return false;

            _appSettings.Remove(settingKey);
            OverwriteSettings();

            return true;
#endif
        }

        private static string Concat(string[] strings)
        {
            var builder = new StringBuilder();
            foreach(var element in strings)
            {
                if (!string.IsNullOrEmpty(element))
                {
                    builder.Append(element.Replace("%", "%25").Replace("|", "%7C"));
                    builder.Append('|');
                }
            }
            var elements = builder.ToString();

            return elements.Substring(0, elements.Length - 1);
        }
    }
}
