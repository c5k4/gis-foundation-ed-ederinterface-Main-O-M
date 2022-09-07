using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Configuration;
namespace PGE.BatchApplication.LoadData
{
    class Program
    {
        static void Main(string[] args)        {

            string csv_file_path1 = ConfigurationManager.AppSettings["filePeakLoad"];
            string csv_file_path2 = ConfigurationManager.AppSettings["fileFeederPeak"];
            string csv_file_path3 = ConfigurationManager.AppSettings["fileBankPeak"]; 
            csvread objCsvread = new csvread();          
           bool success = objCsvread.ProcessCsvData(csv_file_path1, csv_file_path2, csv_file_path3);
           if (success)
           {
               Console.Write("Data Successfully inserted in database.");
           }
           else {
               Console.Write("Error Occured.");
           }
        }
    }
}
