using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Diagnostics;
using System.IO;

namespace InvokeESRIRegAsm
{
    [RunInstaller(true)]
    public partial class Installer1 : Installer
    {
        public Installer1()
        {
            InitializeComponent();
        }
        public override void Install(System.Collections.IDictionary stateSaver)
        {
            base.Install(stateSaver);
            // Register the custom component.
            // -----------------------------
            // The default location of the ESRIRegAsm utility.
            // Note how the whole string is embedded in quotes because of the spaces in the path.
            string[] strProgramFile = Environment.GetEnvironmentVariable("AGSDESKTOPJAVA").ToString().Split('\\');
            string fnlStrProgramFile = (strProgramFile[0] + ("\\" + strProgramFile[1]));
            string cmd1 = "\"" + "C:\\Program Files (x86)\\Common Files\\ArcGIS\\bin\\ESRIRegAsm.exe" + "\"";
            //string cmd1 = "\"" + Environment.GetFolderPath
            //     (Environment.SpecialFolder.CommonProgramFiles) + "\\ArcGIS\\bin\\ESRIRegAsm.exe" + "\"";
            int intParameters;
            string part;
            string strArg;
            string part2 = " /p:Desktop /s";
            string cmd2;
            int exitCode;
            // ************* Installation of Miner & Miner product.
            //string cmd3 = "\"" + Environment.GetFolderPath
            //     (Environment.SpecialFolder.ProgramFiles) + "\\Miner and Miner\\ArcFM Solution\\Bin\\RegX.exe" + "\"";
            string cmd3 = "\"" + fnlStrProgramFile  + "\\Miner and Miner\\ArcFM Solution\\Bin\\RegX.exe" + "\"";
            string part3 = " /d ";
            string cmd4;
            // ************* Installation of Miner & Miner product/.
            string[] Parameters= new string[2];
            Parameters[0] = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\" + "PGE.Interfaces.SAP.dll";
            Parameters[1] = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\" + "PGE.Interfaces.SAP.ED06.Batch.dll";

            //Parameters[2] = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\" + "ArcFMSubTask.dll";           
            for (intParameters = 0; intParameters < Parameters.Length; intParameters++)
            {
               // strArg = ("arg" + intParameters.ToString());
                part = Parameters[intParameters];//this.Context.Parameters[strArg];
              //  File.WriteAllText("C:\\yasha\\yasha.txt",part);
                cmd2 = ("\""
                            + (part + ("\"" + part2)));
                exitCode = ExecuteCommand(cmd1, cmd2, 10000);
                // ************* Installation of Miner & Miner product.
                cmd4 = ("\""
                            + (part3 + ("\"" + part)));
                exitCode = ExecuteCommand(cmd3, cmd4, 10000);
                // ************* Installation of Miner & Miner product/.
            }

        }

        public override void Uninstall(System.Collections.IDictionary savedState)
        {
            base.Uninstall(savedState);
            // Unregister the custom component.
            // ---------------------------------
            // The default location of the ESRIRegAsm utility.
            // Note how the whole string is embedded in quotes because of the spaces in the path.
            string[] strProgramFile = Environment.GetEnvironmentVariable("AGSDESKTOPJAVA").ToString().Split('\\');
            string fnlStrProgramFile = (strProgramFile[0] + ("\\" + strProgramFile[1]));
            string cmd1 = "\"" + "C:\\Program Files (x86)\\Common Files\\ArcGIS\\bin\\ESRIRegAsm.exe" + "\"";
            int intParameters;
            string part;
            string strArg;
            // Add the appropriate command line switches when invoking the ESRIRegAsm utility.
            // In this case: /p:Desktop = means the ArcGIS Desktop product, /u = means unregister the Custom Component, /s = means a silent install.
            string part2 = " /p:Desktop /u /s";
            string cmd2;
            int exitCode;
            // ************* Installation of Miner & Miner product.
            string cmd3 = "\"" + fnlStrProgramFile + "\\Miner and Miner\\ArcFM Solution\\Bin\\RegX.exe" + "\"";
            string part3 = " /u ";
            string cmd4;
            // ************* Installation of Miner & Miner product/.
            string[] Parameters = new string[2];
            Parameters[0] = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\" + "PGE.Interfaces.SAP.dll";
            Parameters[1] = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\" + "PGE.Interfaces.SAP.ED06.Batch.dll";
            //Parameters[2] = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\" + "ArcFMSubTask.dll";           
           
            for (intParameters = 0; intParameters < Parameters.Length; intParameters++)
            {
                // strArg = ("arg" + intParameters.ToString());
                part = Parameters[intParameters];//this.Context.Parameters[strArg];
               // part = this.Context.Parameters[strArg];
                cmd2 = ("\""
                            + (part + ("\"" + part2)));
                exitCode = ExecuteCommand(cmd1, cmd2, 10000);
                // ************* Installation of Miner & Miner product.
                cmd4 = ("\""
                            + (part3 + ("\"" + part)));
                exitCode = ExecuteCommand(cmd3, cmd4, 10000);
                // ************* Installation of Miner & Miner product/.
            }

        }

        //public override void Install(System.Collections.IDictionary stateSaver)
        //{
        //    base.Install(stateSaver);

        //    //Register the custom component.
        //    //-----------------------------
        //    //The default location of the ESRIRegAsm utility.
        //    //Note how the whole string is embedded in quotes because of the spaces in the path.
        //    //string cmd1 = "C:\\Program Files (x86)\\Common Files\\ArcGIS\\bin\\ESRIRegAsm.exe" + "\"";
        //    string cmd1 = "\"" + Environment.GetFolderPath
        //        (Environment.SpecialFolder.CommonProgramFiles) +
        //        "\\ArcGIS\\bin\\ESRIRegAsm.exe" + "\"";
        //    //Obtain the input argument (via the CustomActionData Property) in the setup project.
        //    //An example CustomActionData property that is passed through might be something like:
        //    // /arg1="[ProgramFilesFolder]\[ProductName]\bin\ArcMapClassLibrary_Implements.dll",
        //    //which translates to the following on a default install:
        //    //C:\Program Files\MyGISApp\bin\ArcMapClassLibrary_Implements.dll.
        //    string part1 = "C:\\Program Files (x86)\\Pacific Gas and Electric Co\\PGE_Desktop_Installer\\PGE.PGE.ET.Autoupdators_ValidationRule.dll";
          
        //    //string part1 = this.Context.Parameters["arg1"];

        //    //Add the appropriate command line switches when invoking the ESRIRegAsm utility.
        //    //In this case: /p:Desktop = means the ArcGIS Desktop product, /s = means a silent install.
        //    string part2 = " /p:Desktop /s";

        //    //It is important to embed the part1 in quotes in case there are any spaces in the path.
        //    string cmd2 = "\"" + part1 + "\"" + part2;

        //    //Call the routing that will execute the ESRIRegAsm utility.
        //    int exitCode = ExecuteCommand(cmd1, cmd2, 30000);
        //}

        //public override void Uninstall(System.Collections.IDictionary savedState)
        //{
        //    base.Uninstall(savedState);

        //    //Unregister the custom component.
        //    //-----------------------------
        //    //The default location of the ESRIRegAsm utility.
        //    //Note how the whole string is embedded in quotes because of the spaces in the path.
        //   // string cmd1 = "" + "C:\\Program Files (x86)\\Common Files\\ArcGIS\\bin\\ESRIRegAsm.exe" + "\"";
        //    string cmd1 = "\"" + Environment.GetFolderPath
        //        (Environment.SpecialFolder.CommonProgramFiles) +
        //        "\\ArcGIS\\bin\\ESRIRegAsm.exe" + "\"";
        //    //Obtain the input argument (via the CustomActionData Property) in the setup project.
        //    //An example CustomActionData property that is passed through might be something like:
        //    // /arg1="[ProgramFilesFolder]\[ProductName]\PGE.PGE.ET.Autoupdators_ValidationRule.dll",
        //    //which translate to the following on a default install:
        //    //C:\Program Files\MyGISApp\bin\ArcMapClassLibrary_Implements.dll.
        //  string part1 = "C:\\Program Files (x86)\\Pacific Gas and Electric Co\\PGE_Desktop_Installer\\PGE.PGE.ET.Autoupdators_ValidationRule.dll";
         
        //    //string part1 = this.Context.Parameters["arg1"];//"C: \\Program Files (x86)\\Pacific Gas and Electric Co\\PGE_Desktop_Installer\\PGE.PGE.ET.Autoupdators_ValidationRule.dll";
        //        //this.Context.Parameters["arg1"];

        //    //Add the appropriate command line switches when invoking the ESRIRegAsm utility.
        //    //In this case: /p:Desktop = means the ArcGIS Desktop product, /u = means unregister the Custom Component, /s = means a silent install.
        //    string part2 = " /p:Desktop /u /s";

        //    //It is important to embed the part1 in quotes in case there are any spaces in the path.
        //    string cmd2 = "\"" + part1 + "\"" + part2;

        //    //Call the routing that will execute the ESRIRegAsm utility.
        //    int exitCode = ExecuteCommand(cmd1, cmd2, 30000);
        //}

        public static int ExecuteCommand(string Command1, string Command2, int
            Timeout)
        {
            //Set up a ProcessStartInfo using your path to the executable (Command1) and the command line arguments (Command2).
            ProcessStartInfo ProcessInfo = new ProcessStartInfo(Command1, Command2);
            ProcessInfo.CreateNoWindow = true;
            ProcessInfo.UseShellExecute = false;

            //Invoke the process.
            Process Process = Process.Start(ProcessInfo);
            Process.WaitForExit(Timeout);

            //Finish.
            int ExitCode = Process.ExitCode;
            Process.Close();
            return ExitCode;
        }
    }
}