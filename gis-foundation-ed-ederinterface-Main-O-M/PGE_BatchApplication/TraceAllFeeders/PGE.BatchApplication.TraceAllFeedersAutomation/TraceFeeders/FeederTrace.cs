using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using System.Runtime.InteropServices;
using System.IO;
using System.Text.RegularExpressions;
using Miner.Interop;
using System.Threading;
using PGE.BatchApplication.TraceAllFeeders.Common;
using System.Diagnostics;

namespace PGE.BatchApplication.TraceAllFeeders.TraceFeeders
{
    public class FeederTrace
    {
        IWorkspace workspace = null;
        string VersionName = "";
        string InputFile = "";
        string NetworkName = "";
        string FeederID = null;
        StreamWriter writer = null;

        public FeederTrace(string directConnectString, string user, string pass, string networkName, string inputFileName, string versionName)
        {
            workspace = Common.Common.OpenWorkspace(directConnectString, user, pass);
            this.InputFile = inputFileName;
            this.NetworkName = networkName;

            if (!Directory.Exists(Common.Common.LogFileDirectory))
            {
                Directory.CreateDirectory(Common.Common.LogFileDirectory);
            }
            writer = new StreamWriter(Common.Common.LogFileDirectory + "\\" + versionName + ".txt",true);
            Log("Process ID: " + Process.GetCurrentProcess().Id);

            if (workspace == null)
            {
                Console.WriteLine("Unable to connect to the specified database.");
                return;
            }
        }

        public void Log(string log)
        {
            writer.WriteLine(DateTime.Now.ToString() + ": " + log);
            writer.Flush();
        }

        private IVersion CreateVersion(string VersionName)
        {
            Log("Creating version " + VersionName);
            this.VersionName = VersionName;
            IVersionedWorkspace versionedWorkspace = workspace as IVersionedWorkspace;
            IVersion baseVersion = versionedWorkspace.FindVersion(Common.Common.BaseVersion);
            IVersion newVersion = null;

            try
            {
                newVersion = versionedWorkspace.FindVersion(VersionName);
            }
            catch (Exception e) { }

            if (newVersion != null)
            {
                newVersion.Delete();
                while (Marshal.ReleaseComObject(newVersion) > 0) { }
            }

            //Create our version for this thread
            newVersion = baseVersion.CreateVersion(VersionName);
            while (Marshal.ReleaseComObject(baseVersion) > 0) { }

            Log("Version created");
            return newVersion;
        }

        /// <summary>
        /// Traces all the feeders that are specified in the input file
        /// </summary>
        public void TraceFeeders()
        {
            //Read our feederIDs
            FeederID = ReadFeederIDs();

            //Create our version to process
            IVersion newVersion = CreateVersion(Common.Common.BaseChildVersion + "_" + FeederID);
            string versionName = newVersion.VersionName;

            //Trace this feeder
            TraceFeeder(newVersion);

            //Save our edits
            SaveEdits(newVersion);

            //Release our resources
            while (Marshal.ReleaseComObject(newVersion) > 0) { }

            writer.Close();

            //Send this version for rec and post
            SendForPosting(versionName);
        }

        /// <summary>
        /// Sends this version to the rec and post mail slot to be place in queue
        /// </summary>
        /// <param name="versionName"></param>
        private void SendForPosting(string versionName)
        {
            MailSlot.WriteToMailSlot(Common.Common.ReconcileAndPostMailSlot, versionName);
        }

        /// <summary>
        /// Saves the edits and stops editing
        /// </summary>
        /// <param name="version"></param>
        private void SaveEdits(IVersion version)
        {
            Log("Saving Edits");
            IWorkspaceEdit wsEdit = version as IWorkspaceEdit;
            wsEdit.StopEditing(true);
        }

        /// <summary>
        /// Saves the edits and restarts editing
        /// </summary>
        /// <param name="version"></param>
        /// <param name="saveEdits"></param>
        private void SaveAndReStartEditing(IVersion version, bool saveEdits)
        {
            Log("Saving Edits");
            IWorkspaceEdit wsEdit = version as IWorkspaceEdit;
            wsEdit.StopEditing(saveEdits);
            wsEdit.StartEditing(false);
        }

        /// <summary>
        /// Reconciles the specified version in favor of the parent and returns true if conflicts found
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        private bool Reconcile(IVersion version)
        {
            try
            {
                Log("Reconciling version...");
                IWorkspaceEdit wsEdit = version as IWorkspaceEdit;
                IVersionEdit4 versionEdit4 = version as IVersionEdit4;
                bool TryAgain = true;
                bool conflictsFound = true;
                while (TryAgain)
                {
                    try
                    {
                        TryAgain = false;
                        conflictsFound = versionEdit4.Reconcile4(Common.Common.BaseVersion, false, false, false, false);
                    }
                    catch (COMException e)
                    {
                        if (e.ErrorCode == -2147217137)
                        {
                            TryAgain = true;
                        }
                        System.Threading.Thread.Sleep(10000);
                    }
                }

                if (conflictsFound) { Log("Conflicts found. Resolved in favor of parent. These feeders must be traced again."); }
                return conflictsFound;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// Posts the specified version to the parent and returns false if contains conflicts
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        private bool Post(IVersion version)
        {
            bool TryAgain = true;
            while (TryAgain)
            {
                try
                {
                    IVersionEdit4 versionEdit = version as IVersionEdit4;
                    if (!Reconcile(version))
                    {
                        Log("Attempting to post version...");
                        TryAgain = false;
                        if (versionEdit.CanPost())
                        {
                            versionEdit.Post(Common.Common.BaseVersion);
                            Log("Version posted");
                            return true;
                        }
                    }
                    else
                    {
                        SaveAndReStartEditing(version, true);
                        TryAgain = false;
                    }
                }
                catch (Exception e)
                {
                    //If something fails then something changed in default when we tried to post.  Need to reconcile and try again.
                    Log("Unable to post: " + e.Message);
                    SaveAndReStartEditing(version, true);
                    TryAgain = true;
                }
            }

            Log("Version could not be posted. Parent version must have changed. These feeders must be traced again.");
            return false;
        }

        /// <summary>
        /// This method will read in the input file which contains the feeder ids that will be processed.
        /// </summary>
        /// <returns>String array of the numbers to process</returns>
        private string ReadFeederIDs()
        {
            StreamReader reader = new StreamReader(InputFile);
            string feederID = reader.ReadLine();
            reader.Close();
            return feederID;
        }

        /// <summary>
        /// This will trace all feeders specified in the input file against the version created for this execution
        /// </summary>
        /// <param name="version"></param>
        private void TraceFeeder(IVersion version)
        {
            Log("Tracing specified feeders");
            IWorkspace versionWorkspace = version as IWorkspace;
            IGeometricNetwork geomNetwork = Common.Common.GetNetwork(versionWorkspace, NetworkName) as IGeometricNetwork;
            IWorkspaceEdit wsEdit = version as IWorkspaceEdit;
            if (wsEdit.IsBeingEdited()) { wsEdit.StartEditing(false); }

            Type type = Type.GetTypeFromProgID("mmFramework.MMFeederExt");
            object obj = Activator.CreateInstance(type);
            IMMFeederExt feederExt = obj as IMMFeederExt;
            IMMFeederSpace feederSpace = feederExt.get_FeederSpace(geomNetwork, false);
            IMMEnumFeederSource feederSources = feederSpace.FeederSources;
            IMMFeederTracer feederTracer = new MMFeederTracer();
            feederSources.Reset();
            int feederCount = 1;
            try
            {
                IMMFeederSource feederSource = feederSources.Next();
                while (feederSource != null)
                {
                    if (FeederID == feederSource.FeederID.ToString())
                    {
                        Log("Tracing --- FeederID: " + feederSource.FeederID + " FeatureClassID: " + feederSource.FeatureClassID);
                        //Log("FeederID: " + feederSource.FeederID + " FeatureClassID: " + feederSource.FeatureClassID);
                        feederCount++;

                        //Now we can trace this feeder
                        feederTracer.TraceFeeder(geomNetwork, feederSource);
                        Log("Finished tracing feeder " + feederSource.FeederID);
                        //SaveAndReStartEditing(version, true);
                    }
                    if (feederSource != null) { while (Marshal.ReleaseComObject(feederSource) > 0) { } }
                    feederSource = feederSources.Next();
                }
            }
            finally
            {
                if (feederSpace != null) { while (Marshal.ReleaseComObject(feederSpace) > 0);}
                if (feederSources != null) { while (Marshal.ReleaseComObject(feederSources) > 0);}
                if (feederTracer != null) { while (Marshal.ReleaseComObject(feederTracer) > 0);}
            }
        }
    }
}
