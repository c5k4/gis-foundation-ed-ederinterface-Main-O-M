using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;

#if SILVERLIGHT
namespace Miner.Server.Client
#elif WPF
namespace Miner.Mobile.Client
#endif
{
    /// <summary>
    /// A class to facilitate autocomplete functionality.  Holds lists of values by key and stores
    /// the lists in local storage.
    /// </summary>
    public class AutoCompleteHelper
    {
        #region Constants

        private const string SubDir = "ArcFM";
        private const char Delimiter = ';';

        #endregion

        #region Properties

        /// <summary>
        /// Key for the storage location.
        /// </summary>
        public string ToolKey { get; set; }

        /// <summary>
        /// Maximum number of values to store.
        /// </summary>
        public int MaxValueCount { get; set; }

        /// <summary>
        /// Cached list of values.
        /// </summary>
        public Dictionary<string,List<string>> ListCache { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor to create an instance with an associated key path.
        /// </summary>
        /// <param name="toolKey">path to use in creating the storage</param>
        public AutoCompleteHelper(string toolKey)
        {
            ToolKey = Path.Combine(SubDir, toolKey);

            InitializeStorage();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Stores a set of key value items to the silverlight local storage and updates the cached list.
        /// The items are stored one set per line semi-colon delimited.
        /// </summary>
        /// <param name="list">key value pairs to store</param>
        public void StoreKeyValuePairList(Dictionary<string, List<string>> list)
        {
            if (IsolatedStorageFile.IsEnabled == false) return;
            
            // Obtain an isolated store for an application.
#if SILVERLIGHT
            using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
#elif WPF
            using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForAssembly())
#endif
            {
                using (var sw = new StreamWriter(store.OpenFile(ToolKey, FileMode.Open, FileAccess.Write)))
                {
                    // Clear the cache list.
                    ListCache.Clear();

                    foreach (var key in list.Keys)
                    {
                        if (!ListCache.ContainsKey(key)) ListCache.Add(key, new List<string>());

                        int count = 0;

                        foreach (var item in list[key])
                        {
                            // Limit the number of values stored to MaxValueCount
                            if (count == MaxValueCount) break;

                            ListCache[key].Add(item);

                            string itemLine = key + Delimiter + item;
                            sw.WriteLine(itemLine);

                            count++;
                        }
                    }
                    sw.Close();
                }
            }
        }

        /// <summary>
        /// Updates the key value items in the local storage from the cached list.
        /// The items are stored one set per line semi-colon delimited.
        /// </summary>
        public void UpdateKeyValuePairList()
        {
            if (IsolatedStorageFile.IsEnabled == false) return;
            
            // Obtain an isolated store for an application.
#if SILVERLIGHT
            using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
#elif WPF
            using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForAssembly())
#endif
            {
                using (var sw = new StreamWriter(store.OpenFile(ToolKey, FileMode.Open, FileAccess.Write)))
                {
                    foreach (var key in ListCache.Keys)
                    {
                        int count = 0;

                        foreach (var item in ListCache[key])
                        {
                            // Limit the number of values stored to MaxValueCount
                            if (count == MaxValueCount) break;

                            string itemLine = key + Delimiter + item;
                            sw.WriteLine(itemLine);

                            count++;
                        }
                    }
                    sw.Close();
                }
            }
        }
        
        /// <summary>
        /// Returns a list of values for a given key that match a given prefix.
        /// </summary>
        /// <param name="key">the key of the list to compare against</param>
        /// <param name="prefix">the prefix to look for</param>
        /// <returns>a list of values from the given key starting with the prefix</returns>
        public List<string> GetValueSuggestionList(string key, string prefix)
        {
            List<string> list = null;
            if (string.IsNullOrEmpty(key)) return list;
            if (string.IsNullOrEmpty(prefix)) return list;

            if (!ListCache.ContainsKey(key)) return list;

            list = (from query in ListCache[key] where query.ToLower().StartsWith(prefix.ToLower()) select query).ToList();

            list.Sort();

            return list.Count > 0 ? list : null;
        }

        /// <summary>
        /// Adds a unique value to a key.  If the key doesn't exist, it is created. Updates the cache list and, if
        /// the updateStorage flag is set, updates local storage.
        /// </summary>
        /// <param name="key">the key for the list to update</param>
        /// <param name="value">the value to add to the list</param>
        /// <param name="updateStorage">whether to update storage</param>
        public void AddUniqueValue(string key, string value, bool updateStorage)
        {
            if (string.IsNullOrEmpty(key)) return;
            if (string.IsNullOrEmpty(value)) return;

            if (ListCache.ContainsKey(key))
            {
                if (ListCache[key].Contains(value)) return;

                // If we are already at capacity, remove the first (which should be oldest) item from the list.
                if (ListCache[key].Count >= MaxValueCount) ListCache[key].RemoveAt(0);

                ListCache[key].Add(value);
            }
            else
            {
                ListCache.Add(key, new List<string> { value });
            }

            if (updateStorage) UpdateKeyValuePairList();
        }

        /// <summary>
        /// Retrieves a set of key value items from the silverlight local storage.
        /// Does not update the cached list.
        /// The items were initially stored one set per line semi-colon delimited.
        /// </summary>
        /// <returns>key value pairs</returns>
        public Dictionary<string, List<string>> RetrieveKeyValuePairList()
        {
            var list = new Dictionary<string, List<string>>();

            if (IsolatedStorageFile.IsEnabled == false) return list;
            
            // Obtain an isolated store for an application.
#if SILVERLIGHT
            using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
#elif WPF
            using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForAssembly())
#endif
            {
                using (var reader = new StreamReader(store.OpenFile(ToolKey, FileMode.Open, FileAccess.Read)))
                {
                    while (!reader.EndOfStream)
                    {
                        string itemLine = reader.ReadLine();

                        string[] item = itemLine.Split(Delimiter);

                        if (!list.ContainsKey(item[0])) list.Add(item[0], new List<string>());
                        if (!list[item[0]].Contains(item[1])) list[item[0]].Add(item[1]);

                        // Don't get more values than the configured maximum.
                        if (list[item[0]].Count >= MaxValueCount) break;

                    }

                    reader.Close();
                }
            }

            return list;
        }

        /// <summary>
        /// Clear the stored data.
        /// </summary>
        public void ClearStore()
        {
            if (IsolatedStorageFile.IsEnabled == false) return;

#if SILVERLIGHT
            using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
#elif WPF
            using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForAssembly())
#endif
            {
                // Remove the store file.
                store.DeleteFile(ToolKey);

                // Recreate it.
                IsolatedStorageFileStream settings = store.CreateFile(ToolKey);
                settings.Close();
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Initializes the storage ensuring the storage exists.
        /// </summary>
        private void InitializeStorage()
        {
            ListCache = new Dictionary<string, List<string>>();

            // Set the default MaxValueCount to 15
            MaxValueCount = 15;

            if (IsolatedStorageFile.IsEnabled == false) return;

            // Obtain an isolated store for an application.
#if SILVERLIGHT
            using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
#elif WPF
            using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForAssembly())
#endif
            {
                // Determine if the settings directory exists. Create it if not.
                if (!store.DirectoryExists(SubDir)) store.CreateDirectory(SubDir);

                // Determine if the settings file exists. If not create, if so open.
                StreamReader reader = store.FileExists(ToolKey) ? new StreamReader(store.OpenFile(ToolKey, FileMode.Open, FileAccess.Read)) : new StreamReader(store.CreateFile(ToolKey));

                while (!reader.EndOfStream)
                {
                    string itemLine = reader.ReadLine();

                    string[] item = itemLine.Split(Delimiter);

                    if (!ListCache.ContainsKey(item[0])) ListCache.Add(item[0], new List<string>());
                    if (!ListCache[item[0]].Contains(item[1])) ListCache[item[0]].Add(item[1]);

                    // Don't get more values than the configured maximum.
                    if (ListCache[item[0]].Count >= MaxValueCount) break;
                }

                reader.Close();
            }
        }

        #endregion
    }
}