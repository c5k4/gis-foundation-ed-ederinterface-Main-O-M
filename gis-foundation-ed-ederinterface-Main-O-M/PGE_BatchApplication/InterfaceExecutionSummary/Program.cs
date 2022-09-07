// ========================================================================
// Copyright © 2021 PGE.
// <history>
// Updates Interface Batch status on INTDATAARCH.IntExecutionSummary Table
// TCS V3SF (EDGISREARC-389) 04/28/2021               Created
// </history>
// All rights reserved.
// ========================================================================
using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PGE.BatchApplication.IntExecutionSummary
{
    /// <summary>
    /// Updates Interface Batch status
    /// </summary>
    class Program
    {
        /// <summary>
        /// Program accepting arguments for Interface Execution Summary Update
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            try
            {
                //Main Process
                MainProcess mainProcess = new MainProcess();
                mainProcess.RunIntExecutionSummary(args);
            }
            catch(Exception ex)
            {
                Common.ErrorMessage(ex, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            }
        }
    }
}
