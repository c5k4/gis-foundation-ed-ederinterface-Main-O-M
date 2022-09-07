using System;
using System.IO.IsolatedStorage;
using System.Windows.Browser;
using NLog.Targets;
using NLog;
using System.IO;

namespace Miner.Silverlight.Logging.Client
{
    [Target("IsolatedStorageTarget")]
    public sealed class IsolatedStorageTarget : FileTarget
    {
        IsolatedStorageFile _storageFile = null;
        string _fileName = "Miner.Silverlight.Logging.Client.log"; // Default. Configurable through the 'filename' attribute in nlog.config

        public IsolatedStorageTarget()
        {
        }

        ~IsolatedStorageTarget()
        {
            if (_storageFile != null)
            {
                _storageFile.Dispose();
                _storageFile = null;
            }
        }

        public string localFileName
        {
            set
            {
                _fileName = value;
            }
            get
            {
                return _fileName;
            }
        }

        protected override void Write(LogEventInfo logEvent)
        {
            try
            {
                WriteToIsolatedStorage(this.Layout.Render(logEvent));
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
            }
        }

        public void ClearArchive()
        {
            long cntr = this.MaxArchiveFiles+1;
            while (cntr > 0)
            {
                if (_storageFile.FileExists(this.localFileName+cntr.ToString()+".log")) {
                    if (cntr >= this.MaxArchiveFiles)
                    {
                        _storageFile.DeleteFile(this.localFileName + cntr.ToString()+".log");
                    }
                    else
                    {
                        IsolatedStreamUtilities.CopyFile(_storageFile, this.localFileName + cntr.ToString() + ".log", this.localFileName + (cntr + 1).ToString() + ".log");
                        _storageFile.DeleteFile(this.localFileName + cntr.ToString() + ".log");
                    }
                }
                cntr--;
            }
        }

        public void RollLogToArchive()
        {
            ClearArchive();
            IsolatedStreamUtilities.CopyFile(_storageFile, this.localFileName + ".log", this.localFileName + "1" + ".log");
            _storageFile.DeleteFile(this.localFileName + ".log");
        }
        
        public void WriteToIsolatedStorage(string msg)
        {
            msg = HttpUtility.HtmlDecode(msg);
            if (_storageFile == null)
                _storageFile = IsolatedStorageFile.GetUserStoreForApplication();

            using (IsolatedStorageFile isolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                // The isolated storage is limited in size. So, when approaching the limit
                // simply purge the log file. (Yeah yeah, the file should be circular, I know...)
                if (_storageFile.AvailableFreeSpace < msg.Length * 100)
                {
                    ClearArchive();
                }

                // Write to isolated storage
                using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(_fileName + ".log", FileMode.Append, FileAccess.Write, isolatedStorage))
                {
                    using (TextWriter writer = new StreamWriter(stream))
                    {
                        writer.WriteLine(msg);
                    }
                }

                //Check to see if we need archiving
                long curFileSize = IsolatedStreamUtilities.GetFilesize(this.localFileName + ".log");
                if (curFileSize > this.ArchiveAboveSize)
                {
                    RollLogToArchive();
                }

            }
        }

    }
}