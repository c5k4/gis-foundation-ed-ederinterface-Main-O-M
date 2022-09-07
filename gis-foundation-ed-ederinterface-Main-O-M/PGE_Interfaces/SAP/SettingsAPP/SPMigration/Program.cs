using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ReadWriteCsv;
namespace FileReader
{
    class Program
    {
        static string commonPath = @"C:\Users\h1c5\Documents\SPMigration_Documents1\Bay\";
        static void Main(string[] args)
        {
            //@"BAY AREA",
            string[] paths = new string[] { @"North Bay", @"Diablo", @"East Bay", @"San Francisco" };
            //@"NORTHERN"
            //string[] paths = new string[] { "Sonoma", "Sierra", "Sacramento", "North Valley", "North Coast" };



            //central coast 
            //string[] paths = new string[] { @"Z:\Central Coast", @"Z:\Los Padres", @"Z:\Peninsula", @"Z:\De Anza", @"Z:\Mission", @"Z:\San Jose" };
            //string[] paths = new string[] {@"Z:\Central Coast"};
            Console.WriteLine("Processing Started!!!");
            using (CsvFileWriter writer = new CsvFileWriter(@"C:\Users\H1C5\Documents\BayAllDevices_9_17.csv"))
            {
                using (CsvFileWriter writer1 = new CsvFileWriter(@"C:\Users\H1C5\Documents\BayAllDevicesNotWorking_9_17.csv"))
                {
                    foreach (string path in paths)
                    {
                        DirectoryInfo di = new DirectoryInfo(string.Format("{0}{1}", commonPath, path));
                        if (di.Exists)
                        {
                            FileInfo[] fileNames = di.GetFiles("*", SearchOption.AllDirectories);
                            Console.WriteLine("Processing Files for " + path);
                            foreach (FileInfo file in fileNames)
                            {
                                CsvRow row = new CsvRow();
                                bool IsGoodData = true;
                                string[] metadatas = CheckMetadata(file.FullName.Replace(commonPath, string.Empty), out IsGoodData); //file.FullName.Split('\\');
                                foreach (var metadata in metadatas)
                                {
                                    row.Add(metadata);
                                }
                                if (!IsGoodData)
                                    row.Add(file.FullName.Replace(commonPath, string.Empty));
                                //row.Add(string.Format("{0:MM/dd/yyyy}", file.LastWriteTimeUtc.ToString()));
                                if (IsGoodData)
                                    writer.WriteRow(row);
                                else
                                    writer1.WriteRow(row);

                            }
                        }
                    }
                }
                Console.WriteLine("File Processing Finished. Press enter to close window");
                Console.ReadKey();
                //ReadSubDirectories(di,writer);
            }

            //Console.WriteLine("Reading File...");

            /*
            using (CsvFileReader reader = new CsvFileReader("C:\\TestC.csv"))
            {
                CsvRow row = new CsvRow();
                while (reader.ReadRow(row))
                {
                    foreach (string s in row)
                    {
                        //Console.Write(s);
                        //Console.Write(" ");
                    }
                    //Console.WriteLine();
                }
            }
            */

        }

        //Unused function
        private static void ReadSubDirectories(DirectoryInfo di, CsvFileWriter writer)
        {
            foreach (DirectoryInfo sDi in di.GetDirectories())
            {
                FileInfo[] fileNames = sDi.GetFiles();

                foreach (FileInfo file in fileNames)
                {
                    CsvRow row = new CsvRow();

                    string[] metadatas = file.FullName.Split('\\');

                    foreach (var metadata in metadatas)
                    {
                        row.Add(metadata);
                    }

                    writer.WriteRow(row);
                }
                ReadSubDirectories(sDi, writer);
            }

        }

        private static string[] CheckMetadata(string filePath, out bool IsGoodData)
        {
            IsGoodData = true;
            string[] deviceTypes = { "switch", "switches", "capacitors", "interrupters", "reclosers", "regulators", "scadamates", "sectionalizers", "capacitor", "interrupter", "recloser", "regulator", "scadamate", "sectionalizer" };

            string[] metadatas = filePath.Split('\\');
            if (metadatas.Count() > 0)
            {
                if (metadatas.Count() > 3)
                {
                    if (metadatas[3].Contains('.') || metadatas[3].Contains(' ') || metadatas[3].Contains('-') || metadatas[3].Contains('_'))
                    {
                        IsGoodData = false;
                    }
                    if (metadatas[1].ToUpper() != "SUBSTATIONS")
                    {
                        if (!deviceTypes.Contains(metadatas[2].ToLower()))
                        {
                            IsGoodData = false;
                        }
                    }
                    //if (metadatas.Contains("old", StringComparer.InvariantCultureIgnoreCase) || metadatas.Contains("historical", StringComparer.InvariantCultureIgnoreCase))
                    //    IsGoodData = false;
                    if (IsGoodData)
                    {
                        var metaList = new List<string>();
                        var extraMetaList = new List<string>();
                        var counter = 0;
                        foreach (var metadata in metadatas)
                        {
                            if (metadata.Contains("."))
                            {
                                metaList.Add(metadata);
                                metaList.Add(filePath);
                            }
                            else
                            {
                                if (counter < 4)
                                {
                                    metaList.Add(metadata);
                                }
                                else
                                {
                                    extraMetaList.Add(metadata);
                                }
                            }
                            counter++;
                        }
                        metaList.AddRange(extraMetaList);
                        metadatas = metaList.ToArray();
                    }
                }
                else
                {
                    IsGoodData = false;
                }
                
            }
            return metadatas;
        }
    }
}
