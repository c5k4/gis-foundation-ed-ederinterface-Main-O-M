using System;
using System.IO;
using System.IO.Compression;

namespace PGE.Common.Delivery.Systems.IO.Compression
{
    /// <summary>
    /// This implementation is based on the
    /// System.IO.Compression.DeflateStream base class in the .NET Framework
    /// v2.0 base class library.
    /// http://blogs.msdn.com/dotnetinterop/archive/2006/04/05/567402.aspx
    /// </summary>
    public class ZipEntry
    {
        private const int ZipEntrySignature = 0x04034b50;
        private byte[] __filedata;
        private Int16 _BitField;
        private Int32 _CompressedSize;
        private DeflateStream _CompressedStream;
        private Int16 _CompressionMethod;
        private Int32 _Crc32;
        private byte[] _Extra;
        private string _FileName;
        private byte[] _header;
        private Int32 _LastModDateTime;

        private DateTime _LastModified;
        private int _RelativeOffsetOfHeader;
        private Int32 _UncompressedSize;
        private MemoryStream _UnderlyingMemoryStream;
        private Int16 _VersionNeeded;
        /// <summary>
        /// 
        /// </summary>
        public DateTime LastModified
        {
            get { return _LastModified; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string FileName
        {
            get { return _FileName; }
            set { _FileName = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public Int16 VersionNeeded
        {
            get { return _VersionNeeded; }
        }
        /// <summary>
        /// 
        /// </summary>
        public Int16 BitField
        {
            get { return _BitField; }
        }
        /// <summary>
        /// 
        /// </summary>
        public Int16 CompressionMethod
        {
            get { return _CompressionMethod; }
        }
        /// <summary>
        /// 
        /// </summary>
        public Int32 CompressedSize
        {
            get { return _CompressedSize; }
        }
        /// <summary>
        /// 
        /// </summary>
        public Int32 UncompressedSize
        {
            get { return _UncompressedSize; }
        }
        /// <summary>
        /// 
        /// </summary>
        public Double CompressionRatio
        {
            get { return 100*(1.0 - (1.0*CompressedSize)/(1.0*UncompressedSize)); }
        }

        private byte[] _FileData
        {
            get { return __filedata; }
        }
        /// <summary>
        /// 
        /// </summary>
        private DeflateStream CompressedStream
        {
            get
            {
                if (_CompressedStream == null)
                {
                    _UnderlyingMemoryStream = new MemoryStream();
                    bool LeaveUnderlyingStreamOpen = true;
                    _CompressedStream = new DeflateStream(_UnderlyingMemoryStream,
                                                          CompressionMode.Compress,
                                                          LeaveUnderlyingStreamOpen);
                }
                return _CompressedStream;
            }
        }

        internal byte[] Header
        {
            get { return _header; }
        }


        private static bool ReadHeader(Stream s, ZipEntry ze)
        {
            int signature = ZipShared.ReadSignature(s);

            // return null if this is not a local file header signature
            if (SignatureIsNotValid(signature))
            {
                s.Seek(-4, SeekOrigin.Current);
                return false;
            }

            var block = new byte[26];
            int n = s.Read(block, 0, block.Length);
            if (n != block.Length) return false;

            int i = 0;
            ze._VersionNeeded = (short) (block[i++] + block[i++]*256);
            ze._BitField = (short) (block[i++] + block[i++]*256);
            ze._CompressionMethod = (short) (block[i++] + block[i++]*256);
            ze._LastModDateTime = block[i++] + block[i++]*256 + block[i++]*256*256 + block[i++]*256*256*256;
            ze._Crc32 = block[i++] + block[i++]*256 + block[i++]*256*256 + block[i++]*256*256*256;
            ze._CompressedSize = block[i++] + block[i++]*256 + block[i++]*256*256 + block[i++]*256*256*256;
            ze._UncompressedSize = block[i++] + block[i++]*256 + block[i++]*256*256 + block[i++]*256*256*256;

            var filenameLength = (short) (block[i++] + block[i++]*256);
            var extraFieldLength = (short) (block[i++] + block[i++]*256);

            ze._LastModified = ZipShared.PackedToDateTime(ze._LastModDateTime);

            block = new byte[filenameLength];
            n = s.Read(block, 0, block.Length);
            ze._FileName = ZipShared.StringFromBuffer(block, 0, block.Length);

            ze._Extra = new byte[extraFieldLength];
            n = s.Read(ze._Extra, 0, ze._Extra.Length);

            return true;
        }


        private static bool SignatureIsNotValid(int signature)
        {
            return (signature != ZipEntrySignature);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static ZipEntry Read(Stream s)
        {
            return Read(s, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="TurnOnDebug"></param>
        /// <returns></returns>
        public static ZipEntry Read(Stream s, bool TurnOnDebug)
        {
            var entry = new ZipEntry();

            if (!ReadHeader(s, entry)) return null;

            entry.__filedata = new byte[entry.CompressedSize];
            int n = s.Read(entry._FileData, 0, entry._FileData.Length);
            if (n != entry._FileData.Length)
            {
                throw new Exception("badly formatted zip file.");
            }

            return entry;
        }


        internal static ZipEntry Create(String filename)
        {
            var entry = new ZipEntry();
            entry._FileName = filename;

            entry._LastModified = File.GetLastWriteTime(filename);
            if (entry._LastModified.IsDaylightSavingTime())
            {
                DateTime AdjustedTime = entry._LastModified - new TimeSpan(1, 0, 0);
                entry._LastModDateTime = ZipShared.DateTimeToPacked(AdjustedTime);
            }
            else
                entry._LastModDateTime = ZipShared.DateTimeToPacked(entry._LastModified);

            // we don't actually slurp in the file until the caller invokes Write on this entry.

            return entry;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Extract()
        {
            Extract(".");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        public void Extract(Stream s)
        {
            Extract(null, s);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="basedir"></param>
        public void Extract(string basedir)
        {
            Extract(basedir, null);
        }


        // pass in either basdir or s, but not both. 
        private void Extract(string basedir, Stream s)
        {
            string TargetFile = null;
            if (basedir != null)
            {
                TargetFile = Path.Combine(basedir, FileName);

                // check if a directory
                if (FileName.EndsWith("/"))
                {
                    if (!Directory.Exists(TargetFile))
                        Directory.CreateDirectory(TargetFile);
                    return;
                }
            }
            else if (s != null)
            {
                if (FileName.EndsWith("/"))
                    // extract a directory to streamwriter?  nothing to do!
                    return;
            }
            else throw new Exception("Invalid input.");


            using (var memstream = new MemoryStream(_FileData))
            {
                Stream input = null;
                try
                {
                    if (CompressedSize == UncompressedSize)
                    {
                        // the System.IO.Compression.DeflateStream class does not handle uncompressed data.
                        // so if an entry is not compressed, then we just translate the bytes directly.
                        input = memstream;
                    }
                    else
                    {
                        input = new DeflateStream(memstream, CompressionMode.Decompress);
                    }

                    if (TargetFile != null)
                    {
                        // ensure the target path exists
                        if (!Directory.Exists(Path.GetDirectoryName(TargetFile)))
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(TargetFile));
                        }
                    }

                    Stream output = null;
                    try
                    {
                        if (TargetFile != null)
                            output = new FileStream(TargetFile, FileMode.CreateNew);
                        else
                            output = s;


                        var bytes = new byte[4096];
                        int n;


                        n = 1; // anything non-zero
                        while (n != 0)
                        {
                            n = input.Read(bytes, 0, bytes.Length);

                            if (n > 0)
                            {
                                output.Write(bytes, 0, n);
                            }
                        }
                    }
                    finally
                    {
                        // we only close the output stream if we opened it. 
                        if ((output != null) && (TargetFile != null))
                        {
                            output.Close();
                            output.Dispose();
                        }
                    }

                    if (TargetFile != null)
                    {
                        // We may have to adjust the last modified time to compensate
                        // for differences in how the .NET Base Class Library deals
                        // with daylight saving time (DST) versus how the Windows
                        // filesystem deals with daylight saving time.
                        if (LastModified.IsDaylightSavingTime())
                        {
                            DateTime AdjustedLastModified = LastModified + new TimeSpan(1, 0, 0);
                            File.SetLastWriteTime(TargetFile, AdjustedLastModified);
                        }
                        else
                            File.SetLastWriteTime(TargetFile, LastModified);
                    }
                }
                finally
                {
                    // we cannot use using() here because in some cases we do not want to Dispose the stream
                    if ((input != null) && (input != memstream))
                    {
                        input.Close();
                        input.Dispose();
                    }
                }
            }
        }


        internal void WriteCentralDirectoryEntry(Stream s)
        {
            var bytes = new byte[4096];
            int i = 0;
            // signature
            bytes[i++] = (ZipDirEntry.ZipDirEntrySignature & 0x000000FF);
            bytes[i++] = ((ZipDirEntry.ZipDirEntrySignature & 0x0000FF00) >> 8);
            bytes[i++] = ((ZipDirEntry.ZipDirEntrySignature & 0x00FF0000) >> 16);
            bytes[i++] = (byte) ((ZipDirEntry.ZipDirEntrySignature & 0xFF000000) >> 24);

            // Version Made By
            bytes[i++] = Header[4];
            bytes[i++] = Header[5];

            // Version Needed, Bitfield, compression method, lastmod,
            // crc, sizes, filename length and extra field length -
            // are all the same as the local file header. So just copy them
            int j = 0;
            for (j = 0; j < 26; j++)
                bytes[i + j] = Header[4 + j];

            i += j; // positioned at next available byte

            // File Comment Length
            bytes[i++] = 0;
            bytes[i++] = 0;

            // Disk number start
            bytes[i++] = 0;
            bytes[i++] = 0;

            // internal file attrs
            // TODO: figure out what is required here. 
            bytes[i++] = 1;
            bytes[i++] = 0;

            // external file attrs
            // TODO: figure out what is required here. 
            bytes[i++] = 0x20;
            bytes[i++] = 0;
            bytes[i++] = 0xb6;
            bytes[i++] = 0x81;

            // relative offset of local header (I think this can be zero)
            bytes[i++] = (byte) (_RelativeOffsetOfHeader & 0x000000FF);
            bytes[i++] = (byte) ((_RelativeOffsetOfHeader & 0x0000FF00) >> 8);
            bytes[i++] = (byte) ((_RelativeOffsetOfHeader & 0x00FF0000) >> 16);
            bytes[i++] = (byte) ((_RelativeOffsetOfHeader & 0xFF000000) >> 24);


            // actual filename (starts at offset 34 in header) 
            for (j = 0; j < Header.Length - 30; j++)
            {
                bytes[i + j] = Header[30 + j];
            }

            i += j;

            s.Write(bytes, 0, i);
        }


        private void WriteHeader(Stream s, byte[] bytes)
        {
            // write the header info

            int i = 0;
            // signature
            bytes[i++] = (ZipEntrySignature & 0x000000FF);
            bytes[i++] = ((ZipEntrySignature & 0x0000FF00) >> 8);
            bytes[i++] = ((ZipEntrySignature & 0x00FF0000) >> 16);
            bytes[i++] = (byte) ((ZipEntrySignature & 0xFF000000) >> 24);

            // version needed
            Int16 FixedVersionNeeded = 0x14; // from examining existing zip files
            bytes[i++] = (byte) (FixedVersionNeeded & 0x00FF);
            bytes[i++] = (byte) ((FixedVersionNeeded & 0xFF00) >> 8);

            // bitfield
            Int16 BitField = 0x00; // from examining existing zip files
            bytes[i++] = (byte) (BitField & 0x00FF);
            bytes[i++] = (byte) ((BitField & 0xFF00) >> 8);

            // compression method
            Int16 CompressionMethod = 0x08; // 0x08 = Deflate
            bytes[i++] = (byte) (CompressionMethod & 0x00FF);
            bytes[i++] = (byte) ((CompressionMethod & 0xFF00) >> 8);

            // LastMod
            bytes[i++] = (byte) (_LastModDateTime & 0x000000FF);
            bytes[i++] = (byte) ((_LastModDateTime & 0x0000FF00) >> 8);
            bytes[i++] = (byte) ((_LastModDateTime & 0x00FF0000) >> 16);
            bytes[i++] = (byte) ((_LastModDateTime & 0xFF000000) >> 24);

            // CRC32 (Int32)
            var crc32 = new CRC32();
            UInt32 crc = 0;
            try
            {
                Stream input = File.OpenRead(FileName);
                using (input)
                {
                    crc = crc32.GetCrc32AndCopy(input, CompressedStream);
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                CompressedStream.Close(); // to get the footer bytes written to the underlying stream
            }
            bytes[i++] = (byte) (crc & 0x000000FF);
            bytes[i++] = (byte) ((crc & 0x0000FF00) >> 8);
            bytes[i++] = (byte) ((crc & 0x00FF0000) >> 16);
            bytes[i++] = (byte) ((crc & 0xFF000000) >> 24);

            // CompressedSize (Int32)
            var isz = (Int32) _UnderlyingMemoryStream.Length;
            var sz = (UInt32) isz;
            bytes[i++] = (byte) (sz & 0x000000FF);
            bytes[i++] = (byte) ((sz & 0x0000FF00) >> 8);
            bytes[i++] = (byte) ((sz & 0x00FF0000) >> 16);
            bytes[i++] = (byte) ((sz & 0xFF000000) >> 24);

            // UncompressedSize (Int32)

            bytes[i++] = (byte) (crc32.TotalBytesRead & 0x000000FF);
            bytes[i++] = (byte) ((crc32.TotalBytesRead & 0x0000FF00) >> 8);
            bytes[i++] = (byte) ((crc32.TotalBytesRead & 0x00FF0000) >> 16);
            bytes[i++] = (byte) ((crc32.TotalBytesRead & 0xFF000000) >> 24);

            // filename length (Int16)
            var length = (Int16) FileName.Length;
            bytes[i++] = (byte) (length & 0x00FF);
            bytes[i++] = (byte) ((length & 0xFF00) >> 8);

            // extra field length (short)
            Int16 ExtraFieldLength = 0x00;
            bytes[i++] = (byte) (ExtraFieldLength & 0x00FF);
            bytes[i++] = (byte) ((ExtraFieldLength & 0xFF00) >> 8);

            // actual filename
            char[] c = FileName.ToCharArray();
            int j = 0;


            for (j = 0; (j < c.Length) && (i + j < bytes.Length); j++)
            {
                bytes[i + j] = BitConverter.GetBytes(c[j])[0];
            }

            i += j;

            // extra field (we always write null in this implementation)
            // ;;

            // remember the file offset of this header
            _RelativeOffsetOfHeader = (int) s.Length;

            // finally, write the header to the stream
            s.Write(bytes, 0, i);

            // preserve this header data for use with the central directory structure.
            _header = new byte[i];

            for (j = 0; j < i; j++)
                _header[j] = bytes[j];
        }


        internal void Write(Stream s)
        {
            var bytes = new byte[4096];
            int n;

            // write the header:
            WriteHeader(s, bytes);

            // write the actual file data: 
            _UnderlyingMemoryStream.Position = 0;


            while ((n = _UnderlyingMemoryStream.Read(bytes, 0, bytes.Length)) != 0)
            {
                s.Write(bytes, 0, n);
            }

            _UnderlyingMemoryStream.Close();
            _UnderlyingMemoryStream = null;
        }
    }
}