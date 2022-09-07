using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;
using System.Diagnostics;


namespace Telvent.PGE.ED.Desktop
{
    [RunInstaller(true)]
    public partial class ProjectInstallActions : Installer
    {
        public ProjectInstallActions()
        {
            InitializeComponent();
        }

        #region Install/Un-Install
        public override void Install(System.Collections.IDictionary stateSaver)
        {
            base.Install(stateSaver);
            var regSrv = new RegistrationServices();
            //Get the assembly of the newly installed dll so we can determine it's location
            var assembly = GetType().Assembly;
            regSrv.RegisterAssembly(assembly, AssemblyRegistrationFlags.SetCodeBase);

            //Create our regx.exe path
            var regx = GetMinerBinDirectory() + "regx.exe";

            //Set up our cmd line arguments to run regx on this new dll
            var cmdLineArguments = "/r /c " + "\"" + assembly.Location + "\"";

            var process = new System.Diagnostics.Process
                          {
                              StartInfo =
                              {
                                  FileName = regx,
                                  Arguments = cmdLineArguments,
                                  UseShellExecute = false,
                                  RedirectStandardOutput = false
                              }
                          };

            process.Start();
            process.WaitForExit();
        }

        public override void Uninstall(System.Collections.IDictionary savedState)
        {
            var regSrv = new RegistrationServices();
            //Get the assembly of the newly installed dll so we can determine it's location
            var assembly = GetType().Assembly;
            regSrv.UnregisterAssembly(assembly);

            //Get the miner and miner bin directory from the registry
            var minerBinDirectory = GetMinerBinDirectory();

            //Create our regx.exe path
            var regx = minerBinDirectory + "regx.exe";

            //Set up our cmd line arguments to run regx on this new dll
            var cmdLineArguments = "/u \"" + assembly.Location + "\"";

            var process = new System.Diagnostics.Process
                          {
                              StartInfo =
                              {
                                  CreateNoWindow = true,
                                  FileName = regx,
                                  Arguments = cmdLineArguments,
                                  UseShellExecute = false,
                                  RedirectStandardOutput = false
                              }
                          };

            process.Start();
            process.WaitForExit();

            base.Uninstall(savedState);
        }
        #endregion

        /// <summary>
        /// Gets the location of the rebel Miner bin base.
        /// </summary>
        /// <returns>The location of the rebel Miner bin base.</returns>
        public string GetMinerBinDirectory()
        {
            var platform = SafeNativeMethods.GetPlatform();
            // Opening the registry key

            var hklm = Microsoft.Win32.Registry.LocalMachine;
            // Open a subKey as read-only

            Microsoft.Win32.RegistryKey subkey;

            subkey = platform == SafeNativeMethods.Platform.IA64 || platform == SafeNativeMethods.Platform.X64
                ? hklm.OpenSubKey(@"Software\Wow6432Node\Miner and Miner\ArcFM8")
                : hklm.OpenSubKey(@"Software\Miner and Miner\ArcFM8");

            try
            {
                if (subkey == null)
                    throw new System.Exception();
                return (string)subkey.GetValue("MMBinDir");
            }
            catch (System.Exception)
            {
                string message;
                message = platform == SafeNativeMethods.Platform.IA64 || platform == SafeNativeMethods.Platform.X64
                    ? @"Unable to find Regx.exe.  Make sure that HKLM\Software\Wow6432Node\Miner and Miner\ArcFM8\MMBinDir exists and that the location specified exists."
                    : @"Unable to find Regx.exe.  Make sure that HKLM\Software\Miner and Miner\ArcFM8\MMBinDir exists and that the location specified exists.";
                System.Windows.Forms.MessageBox.Show(message, "Error");
                return null;
            }
        }

        /// <summary>
        ///     Starts (or reuses) a process with the specified executable path and arguments.
        /// </summary>
        /// <param name="executablePath"></param>
        /// <param name="arguments"></param>
        /// <returns>
        ///    true if a process resource is started; false if no new process resource is
        ///    started (for example, if an existing process is reused).
        /// </returns>
        private static bool Run(string executablePath, string arguments, int timeout = 30000)
        {
            // Start the child process.
            var process = new Process
            {
                StartInfo =
                {
                    UseShellExecute = false,
                    FileName = executablePath,
                    Arguments = arguments,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };
            var result = process.Start();

            if (timeout < 0)
                process.WaitForExit();
            else
                process.WaitForExit(timeout);
            return result;
        }
    }
}
