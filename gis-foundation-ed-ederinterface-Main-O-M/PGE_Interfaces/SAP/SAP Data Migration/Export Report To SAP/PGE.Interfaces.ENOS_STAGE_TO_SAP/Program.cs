using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using PGE.Interfaces.ENOS_STAGE_TO_SAP;
using System.IO;
using System.Collections;

namespace PGE.Interfaces.ENOS_STAGE_TO_SAP
{
    class Program
    {
       
        static void Main(string[] args)
        {
           //Insert Data into ENOS_TO_SAP_STATUS Table
            InsertInEnosStatus.InsertData();
            //Environment.Exit(0);

            //Export Data after removal of duplicate rows
            ExportToSAP.exportStart();
            

        }
       
    }
}
