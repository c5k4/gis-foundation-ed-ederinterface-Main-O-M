using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace ArcFMSilverlight
{
    public class ApplicationCacheManager
    {
        private const string VERSION_NUMBER = "versionNumber";
        private const string FILE_APPLICATION_VERSION = "ApplicationVersion.txt";

        private const string FILE_APPLICATION_GENERAL = "ApplicationGeneral.txt";

        private IsolatedStorageSettings appSettings = IsolatedStorageSettings.ApplicationSettings;
        private IsolatedStorageFile _store;
        private string _versionNumber;

        private IsolatedStorageFile Store
        {
            get
            {
                if (_store == null)
                {
                    _store = IsolatedStorageFile.GetUserStoreForApplication();
                }
                return _store;
            }
        }

        public void Initialize(XElement root)
        {
            _versionNumber = root.Attribute(VERSION_NUMBER).Value;
        }

        public void CheckApplicationStorageVersion()
        {
            if (!Store.FileExists(FILE_APPLICATION_VERSION) || !VersionIsCurrent())
            {
                DeleteApplicationStorageMain();
                StoreApplicationVersionNumber();
            }

        }

        private bool VersionIsCurrent()
        {
            try
            {
                using (StreamReader reader =
                    new StreamReader(Store.OpenFile(FILE_APPLICATION_VERSION,
                        FileMode.Open, FileAccess.Read)))
                {
                    string contents = reader.ReadToEnd();
                    string currentVersionNumber = contents.Substring(contents.IndexOf("=") + 1);
                    if (currentVersionNumber == _versionNumber)
                    {
                        return true;
                    }
                }
            }
            catch (Exception)
            {
            }
            return false;
        }

        void StoreApplicationVersionNumber()
        {
            IsolatedStorageFileStream stream = Store.CreateFile(FILE_APPLICATION_VERSION);
            stream.Close();

            try
            {
                using (StreamWriter sw =
                    new StreamWriter(Store.OpenFile(FILE_APPLICATION_VERSION,
                        FileMode.Open, FileAccess.Write)))
                {
                    sw.Write(VERSION_NUMBER + "=" + _versionNumber);
                }
            }
            catch (Exception)
            {
            }
        }

        public void StoreLocalValue(string attributeName, string attributeValue)
        {
            IsolatedStorageFileStream isfs = GetGeneralIsfs();

            //TODO: enable CSV
            using (StreamWriter sw =
                new StreamWriter(isfs))
            {
                sw.Write(attributeName + "=" + attributeValue);
            }
        }

        private IsolatedStorageFileStream GetGeneralIsfs()
        {
            IsolatedStorageFileStream isfs = null;

            try
            {
                isfs = Store.OpenFile(FILE_APPLICATION_GENERAL,
                    FileMode.Open, FileAccess.ReadWrite);
            }
            catch
            {
            }

            if (isfs == null)
            {
                isfs = Store.CreateFile(FILE_APPLICATION_GENERAL);
            }

            return isfs;
        }

        public string ReadLocallValue(string attributeName)
        {
            //TODO: enable CSV
            string retval = "";

            try
            {
                using (StreamReader reader =
                    new StreamReader(GetGeneralIsfs()))
                {
                    string contents = reader.ReadToEnd();
                    retval = contents.Substring(contents.IndexOf("=") + 1);
                }
            }
            catch (Exception)
            {
            }

            return retval;
        }

        public void DeleteApplicationStorageMain()
        {
            try
            {
                // If you need to recurse... http://msdn.microsoft.com/en-us/library/zd5e2z84(v=vs.110).aspx
                appSettings.Clear();

                _store = IsolatedStorageFile.GetUserStoreForApplication();
                foreach (string fileName in _store.GetFileNames())
                {
                    _store.DeleteFile(fileName);
                }
                // Getting errors from DeleteDirectory. Bizarrely the MSDN sample for DeleteDirectory uses Remove.hmmmm
                // https://msdn.microsoft.com/en-us/library/system.io.isolatedstorage.isolatedstoragefile.deletedirectory(v=vs.110).aspx
                //_store.Remove();
            }
            catch { }

        }


    }
}
