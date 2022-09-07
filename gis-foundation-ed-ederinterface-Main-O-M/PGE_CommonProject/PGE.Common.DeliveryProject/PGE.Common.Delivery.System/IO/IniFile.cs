using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace PGE.Common.Delivery.Systems.IO
{
    /// <summary>
    /// A class that handles reading a standard ini configuration file into a dictionary type format organized by sections.
    /// </summary>
    public class IniFile
    {
        #region Fields

        private Dictionary<string, Dictionary<string, string>> _Sections;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="IniFile"/> class.
        /// </summary>
        public IniFile()
        {
            _Sections = new Dictionary<string, Dictionary<string, string>>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IniFile"/> class.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public IniFile(string fileName)
        {
            this.LoadDictionary(fileName);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        public int Count
        {
            get
            {
                return _Sections.Count;
            }
        }

        /// <summary>
        /// Gets or sets the ini settings for the specified section name.
        /// </summary>
        /// <value>A dictionary of the section values.</value>
        public Dictionary<string, string> this[string sectionName]
        {
            get
            {
                return _Sections[sectionName];
            }
            set
            {
                _Sections[sectionName] = value;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds the specified section name.
        /// </summary>
        /// <param name="sectionName">Name of the section.</param>
        /// <param name="settings">The settings.</param>
        public void Add(string sectionName, Dictionary<string, string> settings)
        {
            _Sections.Add(sectionName, settings);
        }

        /// <summary>
        /// Determines whether the file contains the specified section name.
        /// </summary>
        /// <param name="sectionName">Name of the section.</param>
        /// <returns>
        /// 	<c>true</c> if file contains the specified section name; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(string sectionName)
        {
            return _Sections.ContainsKey(sectionName);
        }

        /// <summary>
        /// Saves the changes by writting a new file configuration.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public void Save(string fileName)
        {
            using (StreamWriter sw = new StreamWriter(fileName, false))
            {
                foreach (KeyValuePair<string, Dictionary<string, string>> section in _Sections)
                {
                    // Write the section header.
                    sw.WriteLine("[" + section.Key + "]");

                    // Write it's values.
                    foreach (KeyValuePair<string, string> pair in section.Value)
                        sw.WriteLine(pair.Key + "=" + pair.Value);

                    sw.WriteLine();
                }
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Loads the dictionary.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        private void LoadDictionary(string fileName)
        {
            _Sections = new Dictionary<string, Dictionary<string, string>>();

            string pattern = @"^"                           // Beginning of the line
                            + @"((?:\[)"                    // Section Start
                            + @"(?<Section>[^\]]*)"         // Actual Section text into Section Group
                            + @"(?:\])"                     // Section End then EOL/EOB
                            + @"(?:[\r\n]{0,}|\Z))"         // Match but don't capture the CRLF or EOB
                            + @"("                          // Begin capture groups (Key Value Pairs)
                            + @"(?!\[)"                     // Stop capture groups if a [ is found; new section
                            + @"(?<Key>[^=]*?)"             // Any text before the =, matched few as possible
                            + @"(?:=)"                      // Get the = now
                            + @"(?<Value>[^\r\n]*)"         // Get everything that is not an Line Changes
                            + @"(?:[\r\n]{0,4})"            // MBDC \r\n
                            + @")+";                        // End Capture groups";

            using (StreamReader sr = new StreamReader(fileName))
            {
                string data = sr.ReadToEnd();

                _Sections = (from Match m in Regex.Matches(data, pattern, RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline)
                             select new
                             {
                                 Section = m.Groups["Section"].Value,
                                 kvps = (from cpKey in m.Groups["Key"].Captures.Cast<Capture>().Select((a, i) => new { a.Value, i })
                                         join cpValue in m.Groups["Value"].Captures.Cast<Capture>().Select((b, i) => new { b.Value, i }) on cpKey.i equals cpValue.i
                                         select new KeyValuePair<string, string>(cpKey.Value, cpValue.Value)).ToDictionary(kvp => kvp.Key, kvp => kvp.Value)

                             }).ToDictionary(itm => itm.Section, itm => itm.kvps);
            }
        }

        #endregion
    }
}