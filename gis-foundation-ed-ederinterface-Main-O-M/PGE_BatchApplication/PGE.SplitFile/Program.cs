using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
//using Oracle.DataAccess.Client;
//using Oracle.DataAccess;
//using Oracle.DataAccess.Types;

namespace PGE.BatchApplication.SplitFile
{
    class Program
    {
        public static void TestSplit(string inputFile, int maxLen = 9000, bool ed06 = false)
        {
            Console.WriteLine("Splitting file [ " + inputFile + " ] with max length [ " + maxLen + " ]");

            var list = new List<string>();
            var fileSuffix = 0;

            DateTime now = DateTime.Now;
            using (var file = File.OpenRead(inputFile))
            using (var reader = new StreamReader(file))
            {
                while (!reader.EndOfStream)
                {
                    list.Add(reader.ReadLine());

                    if (list.Count >= maxLen)
                    {
                        WriteAllLines(inputFile, ++fileSuffix, list, ed06, ref now);

                        list = new List<string>();
                    }
                }
            }

            WriteAllLines(inputFile, ++fileSuffix, list, ed06, ref now);
        }

        static void WriteAllLines(string inputFile, int fileSuffix, List<string> list, bool ed06, ref DateTime now)
        {
            if (ed06)
            {
                string directory = Directory.GetParent(inputFile).FullName;
                now = now.AddSeconds(1.0);
                List<string> emptyStringList = new List<string>();

                string EUFileName = directory + "\\GIS_Data_Export_EU_" + now.ToString("yyyyMMdd_hhmmss");
                string FLFileName = directory + "\\GIS_Data_Export_FL_" + now.ToString("yyyyMMdd_hhmmss");
                string SCFileName = directory + "\\GIS_Data_Export_SC_" + now.ToString("yyyyMMdd_hhmmss");
                if (inputFile.ToUpper().Contains("EU"))
                {
                    File.WriteAllLines(EUFileName + ".csv", list);
                    File.WriteAllLines(FLFileName + ".csv", emptyStringList);
                    File.WriteAllLines(SCFileName + ".csv", emptyStringList);
                }
                else if (inputFile.ToUpper().Contains("FL"))
                {
                    File.WriteAllLines(FLFileName + ".csv", list);
                    File.WriteAllLines(EUFileName + ".csv", emptyStringList);
                    File.WriteAllLines(SCFileName + ".csv", emptyStringList);
                }
                else if (inputFile.ToUpper().Contains("SC"))
                {
                    File.WriteAllLines(SCFileName + ".csv", list);
                    File.WriteAllLines(FLFileName + ".csv", emptyStringList);
                    File.WriteAllLines(EUFileName + ".csv", emptyStringList);
                }
            }
            else
            {
                File.WriteAllLines(inputFile + "_" + (++fileSuffix) + ".csv", list);
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine(DateTime.Now);
            if (args.Length == 0)
            {
                Console.WriteLine("Split <file> (<max_split_file_length>) (-ed06)");
                Console.WriteLine("Example: Split badgers.csv 9000");
                Console.WriteLine("Example: Split GIS_Data_Export_EU_20180201_154614.csv 9000 -ed06");
                return;
            }

            if (args.Length > 2)
            {
                TestSplit(args[0], Convert.ToInt32(args[1]), true);
            }
            else if (args.Length > 1)
            {
                TestSplit(args[0], Convert.ToInt32(args[1]));
            }
            else
            {
                TestSplit(args[0]);
            }

            Console.WriteLine("Done");
            //            string trouble = "120°18";
        }
    }
}
