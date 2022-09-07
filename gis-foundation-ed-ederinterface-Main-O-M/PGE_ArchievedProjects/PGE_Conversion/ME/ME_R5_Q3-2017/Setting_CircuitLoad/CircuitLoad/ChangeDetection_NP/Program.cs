using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Reflection;
using System.Configuration;
using Oracle.DataAccess.Client;
using System.Data;
namespace ChangeDetection_NP
{
    class Program
    {
        static void Main(string[] args)
        {

            string csv_file_path1 = ConfigurationManager.AppSettings["filePeakLoad"];
            string csv_file_path2 = ConfigurationManager.AppSettings["fileFeederPeak"];
            string csv_file_path3 = ConfigurationManager.AppSettings["fileEDPIFeeder"];
            csvread objCsvread = new csvread();
            bool success = objCsvread.ProcessCsvData(csv_file_path1, csv_file_path2, csv_file_path3);
            if (success)
            {

                Console.Write("Process Completed.");
                Console.Write("Press Enter to exit");
                Console.ReadLine();
            }
            else
            {
                Console.Write("Error Occured.");
            }
        }
    }
       

   
}
