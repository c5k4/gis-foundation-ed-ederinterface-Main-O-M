using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PGE.Interface.Powerbase_To_GIS
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                MainClass.StartProcess();
            }
            catch (Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());                
                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;
            }
        }
    }
}
