using System;
using System.IO;
using System.IO.IsolatedStorage;

#if SILVERLIGHT
namespace Miner.Server.Client
#elif WPF
namespace Miner.Mobile.Client
#endif
{
    internal static class LoggingService
    {
        public static void Write(string message)
        {
            WriteToStorage(message);
        }

        public static void Write(Exception ex)
        {
            WriteToStorage(ex.ToString());
        }

        public static void Write(string message, Exception ex)
        {
            message += Environment.NewLine + "\t" + ex.ToString();
            WriteToStorage(message);
        }

        private static void WriteToStorage(string message)
        {

            if (IsolatedStorageFile.IsEnabled == false) return;

            // Obtain an isolated store for an application.
#if SILVERLIGHT
            using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
#elif WPF
            using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForAssembly())
#endif
            {
                try
                {
                    using (var sw = new StreamWriter(store.OpenFile("AppErrors.log", FileMode.Append, FileAccess.Write)))
                    {
                        sw.WriteLine(DateTime.Now + "\t" + message);
                    }
                }
                catch { }
            }
        }
    }
}
