using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Text;
using System.Threading;
using PGE.CircuitProtectionZone.Trace;
using PGE.Common;

namespace PGE.CircuitProtectionZone
{
    class Program
    {
        [STAThread()]
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += UhandledExceptionHandler;

            try
            {
                Args clArgs = Args.Create(args);

                clArgs.Validate();

                Logger.Initialize(clArgs.LogDir,
                                       clArgs.LogName,
                                       500,
                                       clArgs.LogLevel);
                Logger.LogToConsole = true;

                using (new EsriLicense())
                {
                    try
                    {
                        Console.WriteLine("Connecting to {0}", clArgs.ConnectionString);
                        Logger.Info(string.Format("Connecting to {0}", clArgs.ConnectionString));

                        SessionManager.Instance.GeodatabaseManager.Connect(clArgs.ConnectionString, clArgs.DbOwner);

                        //Read application settings
                        Common.ReadAppSettings();

                        TraceQueue traceQueue = new TraceQueue();
                        bool traceHasCompleted = false;

                        traceQueue.ItemCompletedEvent += trace =>
                        {
                            Console.WriteLine();
                            Console.WriteLine();

                            try
                            {
                                traceHasCompleted = true;
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                            }
                        };

                        //Reads the database to determine what we need to trace
                        TraceQueueData traceQueueData = new TraceQueueData();

                        Console.WriteLine("Tracing: ");

                        QueuedTrace queuedTrace = new QueuedTrace(traceQueueData);
                        queuedTrace.UpdateEvent += TraceQueueDataUpdateEventHandler;

                        //Add the traces to the queue
                        traceQueue.Add(queuedTrace);

                        while (traceHasCompleted == false)
                        {
                            Thread.Sleep(1000);
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Error("cpz failed", e);
                    }
                }
            }
            catch (ArgsException ex)
            {
                ShowUsage(ex);
            }
            catch (EsriLicenseException ex)
            {
                Logger.Error(ex);
                Console.WriteLine();
                Console.WriteLine("Failed to initialize ESRI License.  Check log for details.");
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                Console.WriteLine();
                Console.WriteLine("Exception occured during run. Check log for details. ");
                Console.WriteLine("Exception Message: " + ex.Message);
            }
        }

        private static void TraceQueueDataUpdateEventHandler(Trace<TraceQueueData> trace)
        {
            QueuedTrace queueTrace = (QueuedTrace)trace;

            if (queueTrace.State == TraceState.Tracing)
            {
                string message =
                   string.Format("..{0}", queueTrace.TraceNumberExecuting + 1);
                Console.Write(message);
            }
        }

        private static void ShowUsage(ArgsException ex)
        {
            Console.WriteLine();
            Console.WriteLine(ex.Message);
            Console.WriteLine();
            Console.WriteLine("PGE.CircuitProtectionZoneCL -c <connection file> -loglevel [Error|Info|Debug] -logdir <logdir> -logname <logname>");
            Console.WriteLine("\t-c : Required - Location of the connection file.");
            Console.WriteLine("\t-loglevel : Optional - Either Error, Info, or Debug (Info is default)");
            Console.WriteLine("\t-logname : Optional - Log file name (SentryCommandLine.log is default)");
            Console.WriteLine("\t-logdir : Optional - Log file location (current dir of exe is default)");
            Console.ReadLine();
        }

        private static void UhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.IsTerminating == true)
            {
                Console.WriteLine("Unhandled Fatal Exception: " + e.ExceptionObject);
            }
            else
            {
                Console.WriteLine("Unhandled Exception: " + e.ExceptionObject);
            }
        }
    }
}
