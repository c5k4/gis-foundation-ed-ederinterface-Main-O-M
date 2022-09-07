using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PGE.BatchApplication.GISViewUpdatePLC
{
    public class AppConfiguration
    {
        private static Dictionary<string, int> _fcids;
        private static Dictionary<string, int> _subfcids;
        private static Dictionary<string, int> _nonfcids;
        private static string _cadopsConnection;
        private static List<String> _tables;
        private static string _path;
        private static string _archivepath;
        private static string _triggerfile;
        private static int _buffer;

        public static string TriggerFile
        {
            get
            {
                if (_triggerfile == null)
                {
                    _triggerfile = getStringSetting("TriggerFile", "trigger.txt");
                }

                return _triggerfile;
            }

        }
        public static string ArchivePath
        {
            get
            {
                if (_archivepath == null)
                {
                    _archivepath = getStringSetting("ArchivePath", "");
                }

                return _archivepath;
            }
            set
            {
                _archivepath = value;
            }
        }
        public static string Path
        {
            get
            {
                if (_path == null)
                {
                    _path = getStringSetting("ExportPath", "");
                }

                return _path;
            }
            set
            {
                _path = value;
            }
        }



        public static string getSetting(string key)
        {
            string setting = null;
            try
            {
                setting = System.Configuration.ConfigurationManager.AppSettings[key];
            }
            catch { }
            return setting;
        }
        public static string getStringSetting(string Key, string Default)
        {
            string output = Default;
            string value = getSetting(Key);
            if (value != null)
            {
                output = value;
            }
            return output;
        }
        public static bool getBoolSetting(string Key, bool Default)
        {
            bool output = Default;
            string value = getSetting(Key);
            if (value != null)
            {
                try
                {
                    output = Convert.ToBoolean(value);
                }
                catch { }
            }
            return output;
        }
        public static int getIntSetting(string Key, int Default)
        {
            int output = Default;
            string value = getSetting(Key);
            if (value != null)
            {
                try
                {
                    output = Convert.ToInt32(value);
                }
                catch { }
            }
            return output;
        }
        public static double getDoubleSetting(string Key, double Default)
        {
            double output = Default;
            string value = getSetting(Key);
            if (value != null)
            {
                try
                {
                    output = Convert.ToDouble(value);
                }
                catch { }
            }
            return output;
        }
        public static string[] getCommaSeparatedList(string Key, string[] Default)
        {
            string[] output = Default;
            string value = getSetting(Key);
            if (value != null)
            {
                try
                {
                    string[] temp = value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    output = new string[temp.Length];
                    for (int i = 0; i < temp.Length; i++)
                    {
                        output[i] = temp[i].Trim();
                    }

                }
                catch { }
            }
            return output;
        }
        public static Dictionary<string, bool> getCommaSeparatedList(string Key, Dictionary<string, bool> Default)
        {
            Dictionary<string, bool> output = Default;
            string[] list = getCommaSeparatedList(Key, new string[0]);
            if (list.Length > 0)
            {
                output = new Dictionary<string, bool>();
                foreach (string s in list)
                {
                    if (!output.ContainsKey(s))
                    {
                        output.Add(s, true);
                    }
                }
            }
            return output;
        }
        public static List<String> getCommaSeparatedList(string Key, List<String> Default)
        {
            List<String> output = Default;
            string[] list = getCommaSeparatedList(Key, new string[0]);
            if (list.Length > 0)
            {
                output = new List<String>();
                foreach (string s in list)
                {
                    output.Add(s);
                }
            }
            return output;
        }
        public static string[] getSemmiSeparatedList(string Key, string[] Default)
        {
            string[] output = Default;
            string value = getSetting(Key);
            if (value != null)
            {
                try
                {
                    string[] temp = value.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    output = new string[temp.Length];
                    for (int i = 0; i < temp.Length; i++)
                    {
                        output[i] = temp[i].Trim();
                    }

                }
                catch { }
            }
            return output;
        }
        public static Dictionary<string, bool> getSemmiSeparatedList(string Key, Dictionary<string, bool> Default)
        {
            Dictionary<string, bool> output = Default;
            string[] list = getSemmiSeparatedList(Key, new string[0]);
            if (list.Length > 0)
            {
                output = new Dictionary<string, bool>();
                foreach (string s in list)
                {
                    if (!output.ContainsKey(s))
                    {
                        output.Add(s, true);
                    }
                }
            }
            return output;
        }
        public static Dictionary<string, string> getStringMap(string Key, Dictionary<string, string> Default)
        {
            Dictionary<string, string> output = Default;
            string[] list = getSemmiSeparatedList(Key, new string[0]);
            if (list.Length > 0)
            {
                output = new Dictionary<string, string>();
                foreach (string s in list)
                {
                    string[] map = s.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (map.Length == 2)
                    {
                        if (!output.ContainsKey(map[0]))
                        {
                            if (!String.IsNullOrEmpty(map[1]))
                            {
                                output.Add(map[0], map[1]);
                            }
                        }
                    }
                }
            }
            return output;
        }
        public static Dictionary<string, string> getStringMapValue(string Value, Dictionary<string, string> Default)
        {
            Dictionary<string, string> output = Default;
            string[] list = getSemmiSeparatedListValue(Value, new string[0]);
            if (list.Length > 0)
            {
                output = new Dictionary<string, string>();
                foreach (string s in list)
                {
                    string[] map = s.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (map.Length == 2)
                    {
                        if (!output.ContainsKey(map[0]))
                        {
                            if (!String.IsNullOrEmpty(map[1]))
                            {
                                output.Add(map[0], map[1]);
                            }
                        }
                    }
                }
            }
            return output;
        }
        public static string[] getSemmiSeparatedListValue(string value, string[] Default)
        {
            string[] output = Default;
            if (value != null)
            {
                try
                {
                    string[] temp = value.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    output = temp;
                }
                catch { }
            }
            return output;
        }
        public static Dictionary<string, int> getIntMap(string Key, Dictionary<string, int> Default)
        {
            Dictionary<string, int> output = Default;
            string[] list = getSemmiSeparatedList(Key, new string[0]);
            if (list.Length > 0)
            {
                output = new Dictionary<string, int>();
                foreach (string s in list)
                {
                    string[] map = s.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (map.Length == 2)
                    {
                        if (!output.ContainsKey(map[0]))
                        {
                            int value = 0;
                            if (int.TryParse(map[1], out value))
                            {
                                output.Add(map[0], value);
                            }
                        }
                    }
                }
            }
            return output;
        }
        public static Dictionary<int, int> getIntIntMap(string Key, Dictionary<int, int> Default)
        {
            Dictionary<int, int> output = Default;
            string[] list = getSemmiSeparatedList(Key, new string[0]);
            if (list.Length > 0)
            {
                output = new Dictionary<int, int>();
                foreach (string s in list)
                {
                    string[] map = s.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (map.Length == 2)
                    {
                        int key = 0;
                        if (int.TryParse(map[0], out key))
                        {
                            if (!output.ContainsKey(key))
                            {
                                int value = 0;
                                if (int.TryParse(map[1], out value))
                                {
                                    output.Add(key, value);
                                }
                            }
                        }
                    }
                }
            }
            return output;
        }
        public static Dictionary<int, List<int>> getIntIntListMap(string Key, Dictionary<int, List<int>> Default)
        {
            Dictionary<int, List<int>> output = Default;
            string[] list = getSemmiSeparatedList(Key, new string[0]);
            if (list.Length > 0)
            {
                output = new Dictionary<int, List<int>>();
                foreach (string s in list)
                {
                    string[] map = s.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (map.Length == 2)
                    {
                        int key = 0;
                        if (int.TryParse(map[0], out key))
                        {
                            if (!output.ContainsKey(key))
                            {
                                int value = 0;
                                if (int.TryParse(map[1], out value))
                                {
                                    List<int> ilist = new List<int>();
                                    ilist.Add(value);
                                    output.Add(key, ilist);
                                }
                            }
                            else
                            {
                                int value = 0;
                                if (int.TryParse(map[1], out value))
                                {
                                    output[key].Add(value);
                                }
                            }
                        }
                    }
                }
            }
            return output;
        }
        public static Dictionary<int, Dictionary<int, string>> getIntIntStringMap(string Key, Dictionary<int, Dictionary<int, string>> Default)
        {
            Dictionary<int, Dictionary<int, string>> output = Default;
            string[] list = getSemmiSeparatedList(Key, new string[0]);
            if (list.Length > 0)
            {
                output = new Dictionary<int, Dictionary<int, string>>();
                foreach (string s in list)
                {
                    string[] map = s.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (map.Length == 3)
                    {
                        int key = 0;
                        if (int.TryParse(map[0], out key))
                        {
                            if (!output.ContainsKey(key))
                            {
                                output.Add(key, new Dictionary<int, string>());
                            }
                            Dictionary<int, string> output2 = output[key];
                            int value = 0;
                            if (int.TryParse(map[1], out value))
                            {
                                if (!output2.ContainsKey(value))
                                {
                                    output2.Add(value, map[2]);
                                }
                            }
                        }
                    }
                }
            }
            return output;
        }
    }
}
