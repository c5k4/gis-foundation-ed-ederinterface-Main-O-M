using System;
using System.Collections.Generic;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using System.IO;
using PGE.BatchApplication.TraceAllFeeders.TraceFeeders;
using PGE.BatchApplication.TraceAllFeeders.Common;

namespace PGE.BatchApplication.TraceAllFeeders
{
    class Program
    {
        private static LicenseInitializer m_AOLicenseInitializer = new PGE.BatchApplication.TraceAllFeeders.LicenseInitializer();

        [STAThread()]
        static void Main(string[] args)
        {
            if (args[0] == "/?")
            {
                //Print out the arguments to the user
                Console.WriteLine("Valid Arguments:");
                Console.WriteLine("-i {instance name}");
                Console.WriteLine("-u {user name}");
                Console.WriteLine("-p {password}");
                Console.WriteLine("-e {Number of trace all feeders processes to run}");
                Console.WriteLine("-n {Fully qualified feeder manager geometric network name}");
                //Console.WriteLine("-I {Input file location}");
                //Console.WriteLine("-v {Child version name}");
                Console.WriteLine("-su {sde user name}");
                Console.WriteLine("-sp {sde password}");
                Console.WriteLine("");
                Console.WriteLine("Valid argument combinations");
                Console.WriteLine("-i,-u,-p,-e,-n,-su,-sp");
                //Console.WriteLine("-i,-u,-p,-e,-n,-s");
                //Console.WriteLine("-i,-u,-p,-I,-v,-n");
                Console.WriteLine("");
                Console.WriteLine("Notes:");
                Console.WriteLine("-n: Specifying larger values here will decrease execution time, but also increases the CPU and memory requirements");
                //Console.WriteLine("-v: This specifies the child version that will be created off of the " + Common.Common.BaseVersion + " version but all edits still get posted to the " + Common.Common.BaseVersion + " version");
                Console.WriteLine("-su, -sp: If these are specified the process will compress the database during processing. If compress does not occur it can cause performance issues");
            }
            else
            {
                Console.WriteLine("************************************************");
                Console.WriteLine("       PG&E Trace All Feeders");
                Console.WriteLine("************************************************");

                //ESRI License Initializer generated code.
                Console.WriteLine("Checking out Esri license...");
                bool licenseCheckoutSuccess = m_AOLicenseInitializer.InitializeApplication(new esriLicenseProductCode[] { esriLicenseProductCode.esriLicenseProductCodeAdvanced },
                new esriLicenseExtensionCode[] { });

                if (licenseCheckoutSuccess)
                {
                    try
                    {
                        Console.WriteLine("Checking out ArcFM license...");
                        m_AOLicenseInitializer.GetArcFMLicense(Miner.Interop.mmLicensedProductCode.mmLPArcFM);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Unable to checkout ArcFM license");
                        Console.WriteLine(e.Message);
                        licenseCheckoutSuccess = false;
                    }
                }

                try
                {
                    if (licenseCheckoutSuccess)
                    {
                        //Arg: Direct connect string
                        string DirectConnectString = "";
                        string User = "";
                        string Pass = "";
                        string NumProcesses = "";
                        string networkName = "";
                        string inputFile = "";
                        string VersionName = "";
                        bool recAndPost = false;
                        string sdeUser = "";
                        string sdePass = "";

                        //Parse our arguments first
                        for (int i = 0; i < args.Length; i++)
                        {
                            if (args[i] == "-i")
                            {
                                DirectConnectString = args[i + 1];
                            }
                            else if (args[i] == "-u")
                            {
                                User = args[i + 1];
                            }
                            else if (args[i] == "-p")
                            {
                                Pass = args[i + 1];
                            }
                            else if (args[i] == "-e")
                            {
                                NumProcesses = args[i + 1];
                            }
                            else if (args[i] == "-n")
                            {
                                networkName = args[i + 1];
                            }
                            else if (args[i] == "-I")
                            {
                                inputFile = args[i + 1];
                            }
                            else if (args[i] == "-v")
                            {
                                VersionName = args[i + 1];
                            }
                            else if (args[i] == "-r")
                            {
                                recAndPost = true;
                            }
                            else if (args[i] == "-su")
                            {
                                sdeUser = args[i + 1];
                            }
                            else if (args[i] == "-sp")
                            {
                                sdePass = args[i + 1];
                            }
                        }

                        if (recAndPost && !string.IsNullOrEmpty(DirectConnectString) && !string.IsNullOrEmpty(User) && !string.IsNullOrEmpty(Pass))
                        {
                            ReconcileAndPost reconcileAndPost = new ReconcileAndPost(DirectConnectString, User, Pass, sdeUser, sdePass);
                            try
                            {
                                //This is a spawn for the rec and post process
                                reconcileAndPost.BeginReconcileAndPost();
                            }
                            catch (Exception e)
                            {
                                reconcileAndPost.Log("Error during reconcile and post: Message: " + e.Message + " StackTrace: " + e.StackTrace);
                            }
                        }
                        else if (!string.IsNullOrEmpty(DirectConnectString) && !string.IsNullOrEmpty(User) && !string.IsNullOrEmpty(Pass)
                            && !string.IsNullOrEmpty(NumProcesses) && !string.IsNullOrEmpty(networkName) && !string.IsNullOrEmpty(NumProcesses))
                        {
                            if (!String.IsNullOrEmpty(VersionName)) { Common.Common.BaseChildVersion = VersionName; }
                            TraceAllFeedersSpawn spawnTraces = new TraceAllFeedersSpawn(DirectConnectString, User, Pass, networkName, NumProcesses, sdeUser, sdePass);
                            try
                            {
                                spawnTraces.SpawnThreads();
                            }
                            catch (Exception e)
                            {
                                spawnTraces.Log("Error processing: Message: " + e.Message + " StackTrace: " + e.StackTrace);
                            }
                        }
                        else if (!string.IsNullOrEmpty(DirectConnectString) && !string.IsNullOrEmpty(User) && !string.IsNullOrEmpty(Pass)
                          && !string.IsNullOrEmpty(VersionName) && !string.IsNullOrEmpty(networkName) && !string.IsNullOrEmpty(inputFile))
                        {
                            FeederTrace feederTrace = new FeederTrace(DirectConnectString, User, Pass, networkName, inputFile, VersionName);
                            try
                            {
                                feederTrace.TraceFeeders();
                            }
                            catch (Exception e) { feederTrace.Log("Failed tracing feeders Error: " + e.Message + " StackTrace: " + e.StackTrace); }
                        }
                        else
                        {
                            Console.WriteLine("Invalid Arguments specified. Please refer to documentation for valid arguments");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Could not check out an ArcEditor license");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error in execution\n" + e.Message + "\n" + e.StackTrace);
                }

                try
                {
                    m_AOLicenseInitializer.ReleaseArcFMLicense();
                }
                catch { }

                try
                {
                    //Do not make any call to ArcObjects after ShutDownApplication()
                    m_AOLicenseInitializer.ShutdownApplication();
                }
                catch { }

#if DEBUG
                //Console.WriteLine("Press Enter to exit");
                //Console.ReadLine();
#endif
            }
        }
    }
}
