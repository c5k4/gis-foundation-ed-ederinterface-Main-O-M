using System;
using System.IO;

namespace PGE.Common.Delivery.Systems.IO.Compression
{
    /// <summary>
    /// 
    /// </summary>
    public class ZipDirEntry
    {
        internal const int ZipDirEntrySignature = 0x02014b50;
        private Int16 _BitField;
        private string _Comment;
        private Int32 _CompressedSize;
        private Int16 _CompressionMethod;
        private Int32 _Crc32;
        private byte[] _Extra;
        private string _FileName;
        private Int32 _LastModDateTime;

        private DateTime _LastModified;
        private Int32 _UncompressedSize;
        private Int16 _VersionMadeBy;
        private Int16 _VersionNeeded;
        /// <summary>
        /// 
        /// </summary>
        private ZipDirEntry()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ze"></param>
        internal ZipDirEntry(ZipEntry ze)
        {
        }
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
        }
        /// <summary>
        /// 
        /// </summary>
        public string Comment
        {
            get { return _Comment; }
        }
        /// <summary>
        /// 
        /// </summary>
        public Int16 VersionMadeBy
        {
            get { return _VersionMadeBy; }
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static ZipDirEntry Read(Stream s)
        {
            return Read(s, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="TurnOnDebug"></param>
        /// <returns></returns>
        public static ZipDirEntry Read(Stream s, bool TurnOnDebug)
        {
            int signature = ZipShared.ReadSignature(s);
            // return null if this is not a local file header signature
            if (SignatureIsNotValid(signature))
            {
                s.Seek(-4, SeekOrigin.Current);
                return null;
            }

            var block = new byte[42];
            int n = s.Read(block, 0, block.Length);
            if (n != block.Length) return null;

            int i = 0;
            var zde = new ZipDirEntry();

            zde._VersionMadeBy = (short) (block[i++] + block[i++]*256);
            zde._VersionNeeded = (short) (block[i++] + block[i++]*256);
            zde._BitField = (short) (block[i++] + block[i++]*256);
            zde._CompressionMethod = (short) (block[i++] + block[i++]*256);
            zde._LastModDateTime = block[i++] + block[i++]*256 + block[i++]*256*256 + block[i++]*256*256*256;
            zde._Crc32 = block[i++] + block[i++]*256 + block[i++]*256*256 + block[i++]*256*256*256;
            zde._CompressedSize = block[i++] + block[i++]*256 + block[i++]*256*256 + block[i++]*256*256*256;
            zde._UncompressedSize = block[i++] + block[i++]*256 + block[i++]*256*256 + block[i++]*256*256*256;

            zde._LastModified = ZipShared.PackedToDateTime(zde._LastModDateTime);

            var filenameLength = (short) (block[i++] + block[i++]*256);
            var extraFieldLength = (short) (block[i++] + block[i++]*256);
            var commentLength = (short) (block[i++] + block[i++]*256);
            var diskNumber = (short) (block[i++] + block[i++]*256);
            var internalFileAttrs = (short) (block[i++] + block[i++]*256);
            Int32 externalFileAttrs = block[i++] + block[i++]*256 + block[i++]*256*256 + block[i++]*256*256*256;
            Int32 Offset = block[i++] + block[i++]*256 + block[i++]*256*256 + block[i++]*256*256*256;

            block = new byte[filenameLength];
            n = s.Read(block, 0, block.Length);
            zde._FileName = ZipShared.StringFromBuffer(block, 0, block.Length);

            zde._Extra = new byte[extraFieldLength];
            n = s.Read(zde._Extra, 0, zde._Extra.Length);

            block = new byte[commentLength];
            n = s.Read(block, 0, block.Length);
            zde._Comment = ZipShared.StringFromBuffer(block, 0, block.Length);

            return zde;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="signature"></param>
        /// <returns></returns>
        private static bool SignatureIsNotValid(int signature)
        {
            return (signature != ZipDirEntrySignature);
        }
    }
}