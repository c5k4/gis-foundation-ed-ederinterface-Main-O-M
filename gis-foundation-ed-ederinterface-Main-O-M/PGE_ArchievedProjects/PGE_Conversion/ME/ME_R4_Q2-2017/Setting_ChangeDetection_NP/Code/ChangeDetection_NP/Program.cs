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
            Console.WriteLine("Start.");                   
            ProcessData objProcessData = new ProcessData();
            Console.WriteLine("GetDataFrmDB called");
            bool isDataInserted = objProcessData.GetDataFrmDB();
            // bool isDataInserted = true;
            if (isDataInserted)
            {
                Console.WriteLine("Record Inserted.");
            }
            else
            {
                Console.WriteLine("Error in Insertion.");
            }
            Console.WriteLine("press key to exit");
            Console.ReadLine();
        }
    }
       

   
}
