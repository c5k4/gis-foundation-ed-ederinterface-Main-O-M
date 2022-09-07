using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

//using System.Data.OleDb;
using System.Configuration;
using System.Data;
using log4net;
using PGE.Interfaces.ENOS_SAP_TO_PRIMARY_CONSOLE;
using Oracle.DataAccess.Client;
using System.Data.OleDb;
//using Telvent.Delivery.Diagnostics;
//using Oracle.DataAccess.Client;

namespace PGE.Interfaces.ENOS_SAP_TO_PRIMARY_CONSOLE
{
    class Program
    {
       //Progarm To Load Data to Primary Stage Tables
        //private static readonly ILog log = LogManager.GetLogger(typeof(Program));
        static void Main(string[] args)
        {
            //Confuguring Log4Net
            //log4net.Config.XmlConfigurator.Configure();

           
           //Start Reading Text files from SAP
            LoadData.readTextFiles();


            //Further Code for Data Migration

           
         
        }
        
      
    }
}
