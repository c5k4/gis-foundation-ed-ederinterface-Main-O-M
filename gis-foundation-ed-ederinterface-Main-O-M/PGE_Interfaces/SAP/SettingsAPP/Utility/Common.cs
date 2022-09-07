using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Diagnostics;

namespace Utility
{
    class Common
    {
        public static bool TriggerFileExist(string file)
        {
            bool retVal = false;
            if (File.Exists(file))
                retVal = true;
            return retVal;

        }

        public static void CreateTriggerFile(string file)
        {
            FileStream fs = File.Create(file);
            fs.Close();
        }

        public static void WriteLog(string message, string fileName)
        {
            if (!string.IsNullOrEmpty(fileName) && (fileName.ToLower().Contains("loadseer") || fileName.ToLower().Contains("robc")))
            {
                Directory.CreateDirectory(fileName.Substring(0, fileName.LastIndexOf("\\")));
            }
            
            StreamWriter sw = null;
            try
            {
                sw = File.AppendText(fileName);
                sw.WriteLine(string.Concat(message, "      ---- ", DateTime.Now, " ---"));
            }
            catch (Exception ex)
            {
                //throw new Exception("Cannot write to log file", ex);
            }
            finally
            {
                if (sw != null)
                    sw.Close();
            }
        }


        public static void CopyFile(string source, string destination)
        {
            CreateDirectory(destination);
            foreach (string f in Directory.GetFiles(source))
            {
                FileInfo fileInfo = new FileInfo(f);
                File.Copy(f, string.Concat(destination, "\\", fileInfo.Name), true);
            }
        }

        public static void CreateDirectory(string dir)
        {
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }


        public static void UnZipFile(string zipFileName, string outPutFolder)
        {
            FileInfo fi = new FileInfo(zipFileName);
            if (fi.Extension.Equals(".gz", StringComparison.CurrentCultureIgnoreCase))
                unGzFile(zipFileName);
            else if (fi.Extension.Equals(".tar", StringComparison.CurrentCultureIgnoreCase))
            {
                using (FileStream unarchFile = File.OpenRead(zipFileName))
                {
                    TarReader reader = new TarReader(unarchFile);
                    reader.ReadToEnd(outPutFolder);
                }
            }
            else
            {
                throw new ApplicationException("Cannot unzip the file.");
            }
        }

        private static void unGzFile(string filePath)
        {
            FileInfo fileToDecompress = new FileInfo(filePath);
            using (FileStream originalFileStream = fileToDecompress.OpenRead())
            {
                string currentFileName = fileToDecompress.FullName;
                string newFileName = currentFileName.Remove(currentFileName.Length - fileToDecompress.Extension.Length);

                using (FileStream decompressedFileStream = File.Create(newFileName))
                {
                    using (GZipStream decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress))
                    {
                        decompressionStream.CopyTo(decompressedFileStream);
                        Console.WriteLine("Decompressed: {0}", fileToDecompress.Name);
                    }
                }
            }
        }


        private static void UnGzFiles(string zipFileName, string outputFile)
        {
            FileStream fIn = null;
            FileStream fOut = null;
            try
            {
                fIn = File.Open(zipFileName, FileMode.Open, FileAccess.Read);
                fOut = File.Create(outputFile);
                GZipStream G = new GZipStream(fIn, CompressionMode.Decompress);
                for (int i = G.ReadByte(); i != -1; i = G.ReadByte())
                {
                    fOut.WriteByte((byte)i);
                }
            }
            finally
            {
                if (fIn != null)
                {
                    fIn.Close();
                }
                if (fOut != null)
                {
                    fOut.Close();
                }
            }
        }
    }
}
