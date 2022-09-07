using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace PGE.Common.Delivery.Systems.IO.Compression
{
    /// <summary>
    /// This implementation is based on the
    /// System.IO.Compression.DeflateStream base class in the .NET Framework
    /// v2.0 base class library.
    /// http://blogs.msdn.com/dotnetinterop/archive/2006/04/05/567402.aspx
    /// </summary>
    public class ZipFile : IEnumerable<ZipEntry>, IDisposable
    {
        private bool _Debug;
        private List<ZipDirEntry> _direntries;
        private bool _disposed;
        private List<ZipEntry> _entries;
        private string _name;
        private Stream _readstream;
        private FileStream _writestream;

        private ZipFile()
        {
        }

        #region For Writing Zip Files

        /// <summary>
        /// Create the file
        /// </summary>
        /// <param name="NewZipFileName"></param>
        /// <param name="Overwrite"></param>
        public ZipFile(string NewZipFileName, bool Overwrite)
        {
            // create a new zipfile
            _name = NewZipFileName;

            if (File.Exists(_name))
                if (Overwrite)
                    File.Delete(_name);
                else
                    throw new Exception("That file already exists.");

            _entries = new List<ZipEntry>();
        }


        ///// <summary>
        ///// Create the file
        ///// </summary>
        ///// <param name="NewZipFileName"></param>
        //public ZipFile(string NewZipFileName) 
        //{
        //    this.ZipFile(NewZipFileName, false); 
        //}

        /// <summary>
        /// Add a file into the zip
        /// </summary>
        /// <param name="FileName"></param>
        public void AddFile(string FileName)
        {
            ZipEntry ze = ZipEntry.Create(FileName);
            _entries.Add(ze);
        }

        /// <summary>
        /// Add a directory and does a recursive for all files
        /// </summary>
        /// <param name="DirectoryName"></param>
        public void AddDirectory(string DirectoryName)
        {
            AddDirectory(DirectoryName, false);
        }

        /// <summary>
        /// Add a directory and does a recursive for all files
        /// </summary>
        /// <param name="DirectoryName"></param>
        /// <param name="WantVerbose"></param>
        public void AddDirectory(string DirectoryName, bool WantVerbose)
        {
            String[] filenames = Directory.GetFiles(DirectoryName);
            foreach (String filename in filenames)
            {
                if (DoNotZipldb && filename.IndexOf(".ldb") > -1)
                {
                    // do not zip files with extension .ldb and the flag is set
                }

                else
                    AddFile(filename);
            }

            String[] dirnames = Directory.GetDirectories(DirectoryName);
            foreach (String dir in dirnames)
            {
                AddDirectory(dir, WantVerbose);
            }
        }

        /// <summary>
        /// Save zip file
        /// </summary>
        public void Save()
        {
            // an entry for each file
            foreach (ZipEntry e in _entries)
            {
                e.Write(WriteStream);
            }

            WriteCentralDirectoryStructure();
            WriteStream.Close();
            _writestream = null;
        }


        private void WriteCentralDirectoryStructure()
        {
            // the central directory structure
            long Start = WriteStream.Length;
            foreach (ZipEntry e in _entries)
            {
                e.WriteCentralDirectoryEntry(WriteStream);
            }
            long Finish = WriteStream.Length;

            // now, the footer
            WriteCentralDirectoryFooter(Start, Finish);
        }


        private void WriteCentralDirectoryFooter(long StartOfCentralDirectory, long EndOfCentralDirectory)
        {
            var bytes = new byte[1024];
            int i = 0;
            // signature
            UInt32 EndOfCentralDirectorySignature = 0x06054b50;
            bytes[i++] = (byte) (EndOfCentralDirectorySignature & 0x000000FF);
            bytes[i++] = (byte) ((EndOfCentralDirectorySignature & 0x0000FF00) >> 8);
            bytes[i++] = (byte) ((EndOfCentralDirectorySignature & 0x00FF0000) >> 16);
            bytes[i++] = (byte) ((EndOfCentralDirectorySignature & 0xFF000000) >> 24);

            // number of this disk
            bytes[i++] = 0;
            bytes[i++] = 0;

            // number of the disk with the start of the central directory
            bytes[i++] = 0;
            bytes[i++] = 0;

            // total number of entries in the central dir on this disk
            bytes[i++] = (byte) (_entries.Count & 0x00FF);
            bytes[i++] = (byte) ((_entries.Count & 0xFF00) >> 8);

            // total number of entries in the central directory
            bytes[i++] = (byte) (_entries.Count & 0x00FF);
            bytes[i++] = (byte) ((_entries.Count & 0xFF00) >> 8);

            // size of the central directory
            var SizeOfCentralDirectory = (Int32) (EndOfCentralDirectory - StartOfCentralDirectory);
            bytes[i++] = (byte) (SizeOfCentralDirectory & 0x000000FF);
            bytes[i++] = (byte) ((SizeOfCentralDirectory & 0x0000FF00) >> 8);
            bytes[i++] = (byte) ((SizeOfCentralDirectory & 0x00FF0000) >> 16);
            bytes[i++] = (byte) ((SizeOfCentralDirectory & 0xFF000000) >> 24);

            // offset of the start of the central directory 
            var StartOffset = (Int32) StartOfCentralDirectory; // cast down from Long
            bytes[i++] = (byte) (StartOffset & 0x000000FF);
            bytes[i++] = (byte) ((StartOffset & 0x0000FF00) >> 8);
            bytes[i++] = (byte) ((StartOffset & 0x00FF0000) >> 16);
            bytes[i++] = (byte) ((StartOffset & 0xFF000000) >> 24);

            // zip comment length
            bytes[i++] = 0;
            bytes[i++] = 0;

            WriteStream.Write(bytes, 0, i);
        }

        #endregion

        #region For Reading Zip Files
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public ZipEntry this[String filename]
        {
            get
            {
                foreach (ZipEntry e in _entries)
                {
                    if (e.FileName == filename) return e;
                }
                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<ZipEntry> GetEnumerator()
        {
            foreach (ZipEntry e in _entries)
                yield return e;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// This will throw if the zipfile does not exist. 
        /// </summary>
        public static ZipFile Read(string zipfilename)
        {
            return Read(zipfilename, false);
        }

        /// <summary>
        /// This will throw if the zipfile does not exist. 
        /// </summary>
        public static ZipFile Read(string zipfilename, bool TurnOnDebug)
        {
            var zf = new ZipFile();
            zf._Debug = TurnOnDebug;
            zf._name = zipfilename;
            zf._entries = new List<ZipEntry>();
            ZipEntry e;
            while ((e = ZipEntry.Read(zf.ReadStream, zf._Debug)) != null)
            {
                zf._entries.Add(e);
            }

            // read the zipfile's central directory structure here.
            zf._direntries = new List<ZipDirEntry>();

            ZipDirEntry de;
            while ((de = ZipDirEntry.Read(zf.ReadStream, zf._Debug)) != null)
                zf._direntries.Add(de);

            return zf;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        public void ExtractAll(string path)
        {
            ExtractAll(path, false);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="WantVerbose"></param>
        public void ExtractAll(string path, bool WantVerbose)
        {
            foreach (ZipEntry e in _entries)
            {
                e.Extract(path);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        public void RemoveFullPath(string path)
        {
            String s = "";
            foreach (ZipEntry e in _entries)
            {
                s = e.FileName;
                if (s.IndexOf(path) >= 0)
                    s = s.Substring(path.Length);
                e.FileName = s;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        public void Extract(string filename)
        {
            this[filename].Extract();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="s"></param>
        public void Extract(string filename, Stream s)
        {
            this[filename].Extract(s);
        }

        #endregion

        /// <summary>
        /// Zip file name
        /// </summary>
        public string Name
        {
            get { return _name; }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool DoNotZipldb { get; set; }

        private Stream ReadStream
        {
            get
            {
                if (_readstream == null)
                {
                    _readstream = File.OpenRead(_name);
                }
                return _readstream;
            }
        }

        private FileStream WriteStream
        {
            get
            {
                if (_writestream == null)
                {
                    _writestream = new FileStream(_name, FileMode.Create);
                }
                return _writestream;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public List<ZipEntry> Entries
        {
            get { return _entries; }
        }

        #region IDisposable Members
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            // dispose of the managed and unmanaged resources
            Dispose(true);

            // tell the GC that the Finalize process no longer needs
            // to be run for this object.
            GC.SuppressFinalize(this);
        }

        #endregion

        /// <summary>
        /// Shall use Name instead, left there as the previous version was using it
        /// </summary>
        /// <returns></returns>
        [Obsolete("Do not call this method. Do not use, will be removed soon. Use property Name")]
        public string getName()
        {
            return (_name);
        }

        /// <summary>
        /// the destructor, needed to cal Dispose there are resources not handled by GC
        /// </summary> 
        ~ZipFile()
        {
            // call Dispose with false.  Since we're in the
            // destructor call, the managed resources will be
            // disposed of anyways.
            Dispose(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposeManagedResources"></param>
        protected virtual void Dispose(bool disposeManagedResources)
        {
            if (!_disposed)
            {
                if (disposeManagedResources)
                {
                    // dispose managed resources
                    if (_readstream != null)
                    {
                        _readstream.Dispose();
                        _readstream = null;
                    }
                    if (_writestream != null)
                    {
                        _writestream.Dispose();
                        _writestream = null;
                    }
                }
                _disposed = true;
            }
        }
    }
}