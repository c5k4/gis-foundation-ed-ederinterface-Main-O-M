using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.Win32;
using System.IO;
using System.Reflection;


namespace PGE.BatchApplication.ReplaceAnnoation
{
    [RunInstaller(true)]
    public partial class Installer : System.Configuration.Install.Installer
    {
        public Installer()
        {
            InitializeComponent();
        }

        #region Install/Un-Install
        public override void Install(System.Collections.IDictionary stateSaver)
        {
            base.Install(stateSaver);
            RegistrationServices regSrv = new RegistrationServices();
            regSrv.RegisterAssembly(base.GetType().Assembly,
              AssemblyRegistrationFlags.SetCodeBase);

            //Get the assembly of the newly installed dll so we can determine it's location
            Assembly assem = base.GetType().Assembly;
            string location = assem.Location;

            //Get the miner and miner bin directory from the registry
            string minerBinDirectory = ReadBinDir();

            //Create our regx.exe path
            string regx = minerBinDirectory + "regx.exe";

            //Set up our cmd line arguments to run regx on this new dll
            string cmdLineArguments = "\"" + regx + "\"" + " /r /c " + "\"" + location + "\"";

            string fileName = location.Substring(0, location.LastIndexOf("\\")) + "\\ReplaceAnnoTemp.bat";
            FileStream filestream = File.Create(fileName);
            filestream.Close();
            TextWriter tw = new StreamWriter(fileName);
            tw.WriteLine(cmdLineArguments);
            tw.Flush();
            tw.Close();


            // Start the child process.
            Process p = new Process();
            // Redirect the output stream of the child process.
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = fileName;
            p.Start();
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            File.Delete(fileName);
        }

        public override void Uninstall(System.Collections.IDictionary savedState)
        {
            base.Uninstall(savedState);
            RegistrationServices regSrv = new RegistrationServices();
            regSrv.UnregisterAssembly(base.GetType().Assembly);

            //Get the assembly of the newly installed dll so we can determine it's location
            Assembly assem = base.GetType().Assembly;
            string location = assem.Location;

            //Get the miner and miner bin directory from the registry
            string minerBinDirectory = ReadBinDir();

            //Create our regx.exe path
            string regx = minerBinDirectory + "regx.exe";

            //Set up our cmd line arguments to run regx on this new dll
            string cmdLineArguments = "\"" + regx + "\"" + " /u " + "\"" + location + "\"";

            string fileName = location.Substring(0, location.LastIndexOf("\\")) + "\\ReplaceAnnotationTemp.bat";
            FileStream filestream = File.Create(fileName);
            filestream.Close();
            TextWriter tw = new StreamWriter(fileName);
            tw.WriteLine(cmdLineArguments);
            tw.Flush();
            tw.Close();

            // Start the child process.
            Process p = new Process();
            // Redirect the output stream of the child process.
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = fileName;
            p.Start();
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            File.Delete(fileName);

        }
        #endregion

        public string ReadBinDir()
        {
            Platform platform = GetPlatform();
            // Opening the registry key

            RegistryKey rk = Registry.LocalMachine;
            // Open a subKey as read-only

            RegistryKey sk1 = null;

            if (platform == Platform.IA64 || platform == Platform.X64)
            {
                sk1 = rk.OpenSubKey(@"Software\Wow6432Node\Miner and Miner\ArcFM8");
            }
            else
            {
                sk1 = rk.OpenSubKey(@"Software\Miner and Miner\ArcFM8");
            }
            // If the RegistrySubKey doesn't exist -> (null)

            if (sk1 == null)
            {
                if (platform == Platform.IA64 || platform == Platform.X64)
                {
                    MessageBox.Show(@"Unable to find Regx.exe.  Make sure that HKLM\Software\Wow6432Node\Miner and Miner\ArcFM8\MMBinDir exists and that the location specified exists.");
                }
                else
                {
                    MessageBox.Show(@"Unable to find Regx.exe.  Make sure that HKLM\Software\Miner and Miner\ArcFM8\MMBinDir exists and that the location specified exists.");
                }
                return null;
            }
            else
            {
                try
                {
                    // If the RegistryKey exists I get its value

                    // or null is returned.

                    return (string)sk1.GetValue("MMBinDir");
                }
                catch (Exception e)
                {
                    // AAAAAAAAAAARGH, an error!
                    if (platform == Platform.IA64 || platform == Platform.X64)
                    {
                        MessageBox.Show(@"Unable to find Regx.exe.  Make sure that HKLM\Software\Wow6432Node\Miner and Miner\ArcFM8\MMBinDir exists and that the location specified exists.");
                    }
                    else
                    {
                        MessageBox.Show(@"Unable to find Regx.exe.  Make sure that HKLM\Software\Miner and Miner\ArcFM8\MMBinDir exists and that the location specified exists.");
                    }
                    return null;
                }
            }
        }

        public enum Platform
        {
            X86,
            X64,
            IA64,
            Unknown
        }

        internal const ushort PROCESSOR_ARCHITECTURE_INTEL = 0;
        internal const ushort PROCESSOR_ARCHITECTURE_IA64 = 6;
        internal const ushort PROCESSOR_ARCHITECTURE_AMD64 = 9;
        internal const ushort PROCESSOR_ARCHITECTURE_UNKNOWN = 0xFFFF;

        [StructLayout(LayoutKind.Sequential)]
        internal struct SYSTEM_INFO
        {
            public ushort wProcessorArchitecture;
            public ushort wReserved;
            public uint dwPageSize;
            public IntPtr lpMinimumApplicationAddress;
            public IntPtr lpMaximumApplicationAddress;
            public UIntPtr dwActiveProcessorMask;
            public uint dwNumberOfProcessors;
            public uint dwProcessorType;
            public uint dwAllocationGranularity;
            public ushort wProcessorLevel;
            public ushort wProcessorRevision;
        };

        [DllImport("kernel32.dll")]
        internal static extern void GetNativeSystemInfo(ref SYSTEM_INFO lpSystemInfo);

        [DllImport("kernel32.dll")]
        internal static extern void GetSystemInfo(ref SYSTEM_INFO lpSystemInfo);

        public static Platform GetPlatform()
        {
            SYSTEM_INFO sysInfo = new SYSTEM_INFO();

            if (System.Environment.OSVersion.Version.Major > 5 ||
          (System.Environment.OSVersion.Version.Major == 5 && System.Environment.OSVersion.Version.Minor >= 1))
            {
                GetNativeSystemInfo(ref sysInfo);
            }
            else
            {
                GetSystemInfo(ref sysInfo);
            }

            switch (sysInfo.wProcessorArchitecture)
            {
                case PROCESSOR_ARCHITECTURE_IA64:
                    return Platform.IA64;

                case PROCESSOR_ARCHITECTURE_AMD64:
                    return Platform.X64;

                case PROCESSOR_ARCHITECTURE_INTEL:
                    return Platform.X86;

                default:
                    return Platform.Unknown;
            }
        }
    }
}
