using System;

namespace PGE.Common.Delivery.Systems.IO.Compression
{
    /// <summary>
    /// This class represents the gzip data format, 
    /// which uses an industry standard algorithm for 
    /// lossless file compression and decompression. 
    /// The format includes a cyclic redundancy check 
    /// value for detecting data corruption. The gzip data 
    /// format uses the same algorithm as the DeflateStream class, 
    /// but can be extended to use other compression formats. 
    /// The format can be readily implemented in a manner not 
    /// covered by patents. The format for gzip is available 
    /// from the RFC 1952, "GZIP ." This class cannot be used 
    /// to compress files larger than 4 GB.
    /// </summary>
    public static class ZipUtils
    {
        private static string _Name = String.Empty;

        /// <summary>
        /// Name of the zip file
        /// </summary>
        public static string Name
        {
            get { return _Name; }
        }


        /// <summary>
        /// Creates an empty zip file
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static ZipFile CreateEmptyZipFile(string fileName)
        {
            _Name = fileName;
            return new ZipFile(fileName, true);
        }

        /// <summary>
        /// Updates existing file
        /// </summary>
        /// <param name="file"></param>
        /// <param name="newFiles"></param>
        /// <returns></returns>
        public static ZipFile UpdateZipFile(ZipFile file, string[] newFiles)
        {
            foreach (string oneFile in newFiles)
            {
                file.AddFile(oneFile);
            }

            return (file);
        }

        /// <summary>
        /// Extract zip file
        /// </summary>
        /// <param name="FileName">The ZipFile Name</param>
        /// <param name="DestinationPath">The Destination Path</param>
        public static void ExtractZipFile(string FileName, string DestinationPath)
        {
            ExtractZipFile(FileName, DestinationPath, string.Empty);
        }


        /// <summary>
        /// Extract zip file
        /// </summary>
        /// <param name="FileName">The ZipFile Name</param>
        /// <param name="DestinationPath">The Destination Path</param>
        /// <param name="BasePathToRemove">Pat to be removed from the files and directories in the zip file</param>
        /// <returns>Zip file info of the specified filename.</returns>
        public static ZipFile ExtractZipFile(string FileName, string DestinationPath, string BasePathToRemove)
        {
            ZipFile zip = ZipFile.Read(FileName);
            if (BasePathToRemove.Length > 0)
            {
                zip.RemoveFullPath(BasePathToRemove);
            }
            zip.ExtractAll(DestinationPath);

            zip.Dispose();
            return zip;
        }
    }
}