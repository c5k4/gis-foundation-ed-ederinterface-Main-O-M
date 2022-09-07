using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Reflection;
using Diagnostics = PGE.Common.Delivery.Diagnostics;

namespace PGE.Interfaces.SettingsEmailNotification
{
    class Program
    {
        private static readonly Diagnostics.Log4NetLogger _logger = new Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "PGE.Interfaces.SettingsEmailNotification.log4net.config");

        static void Main(string[] args)
        {
            try
            {
                using (SettingsEmailFacade emailFacade = new SettingsEmailFacade())
                {
                    try
                    {
                        _logger.Info("Sending settings email notification at: " + DateTime.Now);
                        Console.WriteLine("Settings email notification...");
                        emailFacade.BeginTransaction();
                        emailFacade.SendNotification();
                        emailFacade.MoveToHistoryTable();
                        emailFacade.Transaction.Commit();
                        Console.WriteLine("done!");
                        _logger.Info("Successfully sent settings email notification");
                        Environment.Exit(0);
                    }
                    catch (Exception ex)
                    {
                        emailFacade.Transaction.Rollback();
                        Console.WriteLine(ex.Message);
                        Environment.Exit(1);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Environment.Exit(1);
            }
        }
    }
}
