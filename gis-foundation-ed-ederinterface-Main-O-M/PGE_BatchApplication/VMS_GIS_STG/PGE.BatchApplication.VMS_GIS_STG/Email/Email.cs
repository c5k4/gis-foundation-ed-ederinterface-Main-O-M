using System.Collections.Generic;

namespace PGE.BatchApplication.VMS_GIS_STG
{
    /**
     * This class represents an EMail to be sent
     **/
    public sealed class Email
    {
        public Email()
        {
            IsHTML = true;
            Attachments = new List<string>();
        }

        public string From { get; set; }
        public string FromDisplayName { get; set; }

        public string To { get; set; }
        public string CC { get; set; }
        public string BCC { get; set; }

        public string Subject { get; set; }

        public string BodyText { get; set; }

        public IList<string> Attachments { get; set; }

        public object Parameters { get; set; }

        public bool IsHTML { get; set; }

        public bool IsHighPriority { get; set; }
    }
}



//namespace ZipFileCreator
//{
//    public static class ZipFileCreator
//    {
//        /// <summary>
//        /// Create a ZIP file of the files provided.
//        /// </summary>
//        /// <param name="fileName">The full path and name to store the ZIP file at.</param>
//        /// <param name="files">The list of files to be added.</param>
//        public static void CreateZipFile(string fileName, IEnumerable<string> files)
//        {
//            // Create and open a new ZIP file
//            var zip = ZipFile.Open(fileName, ZipArchiveMode.Create);
//            foreach (var file in files)
//            {
//                // Add the entry for each file
//                zip.CreateEntryFromFile(file, Path.GetFileName(file), CompressionLevel.Optimal);
//            }
//            // Dispose of the object when we are done
//            zip.Dispose();
//        }
//    }
//}