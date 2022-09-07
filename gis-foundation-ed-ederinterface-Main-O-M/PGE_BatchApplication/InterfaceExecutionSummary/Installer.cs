using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace PGE.BatchApplication.IntExecutionSummary
{
    [RunInstaller(true)]
    public partial class Installer : System.Configuration.Install.Installer
    {
        /// <summary>
        /// Installer Class Constructor
        /// </summary>
        public Installer()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Add GAC Entry
        /// </summary>
        /// <param name="stateSaver"></param>
        public override void Install(IDictionary stateSaver)
        {
            //Add PGE_DBPasswordManagement path in Path System Environment Variable
            var name = "PATH";
            var scope = EnvironmentVariableTarget.Machine; // or User
            var oldValue = Environment.GetEnvironmentVariable(name, scope);
            var newValue = oldValue + @";" + System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            Environment.SetEnvironmentVariable(name, newValue, scope);

            base.Install(stateSaver);
            System.EnterpriseServices.Internal.Publish pub = new System.EnterpriseServices.Internal.Publish();
            pub.GacInstall(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\PGE.BatchApplication.IntExecutionSummary.exe");
        }

        /// <summary>
        /// Remove GAC Entry
        /// </summary>
        /// <param name="savedState"></param>
        public override void Uninstall(IDictionary savedState)
        {
            //Removes PGE_DBPasswordManagement path in Path System Environment Variable
            var name = "PATH";
            var scope = EnvironmentVariableTarget.Machine; // or User
            var oldValue = Environment.GetEnvironmentVariable(name, scope);
            var newValue = oldValue.Replace(@";C:\Program Files (x86)\Miner and Miner\PG&E Custom Components\PGE.IntExecutionSummaryTool", "");
            newValue = newValue.Replace(@";D:\Program Files (x86)\Miner and Miner\PG&E Custom Components\PGE.IntExecutionSummaryTool", "");
            Environment.SetEnvironmentVariable(name, newValue, scope);

            base.Uninstall(savedState);
            System.EnterpriseServices.Internal.Publish pub = new System.EnterpriseServices.Internal.Publish();
            pub.GacRemove(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\PGE.BatchApplication.IntExecutionSummary.exe");   
        }

    }
}
