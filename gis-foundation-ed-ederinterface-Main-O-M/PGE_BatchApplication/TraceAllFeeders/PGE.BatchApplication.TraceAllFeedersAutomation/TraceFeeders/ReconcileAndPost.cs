using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using PGE.BatchApplication.TraceAllFeeders.Common;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections;
using System.Text.RegularExpressions;
using System.Threading;
using System.Diagnostics;

namespace PGE.BatchApplication.TraceAllFeeders.TraceFeeders
{
    public class ReconcileAndPost
    {
        IWorkspace workspace = null;
        IWorkspace sdeWorkspace = null;
        StreamWriter writer = null;
        Queue<string> versionsToRecAndPost = new Queue<string>();
        MailSlot RecAndPostMailSlot = new MailSlot(Common.Common.ReconcileAndPostMailSlot);

        public ReconcileAndPost(string directConnectString, string user, string pass, string sdeUser, string sdePass)
        {
            workspace = Common.Common.OpenWorkspace(directConnectString, user, pass);
            sdeWorkspace = Common.Common.OpenWorkspace(directConnectString, sdeUser, sdePass);

            writer = new StreamWriter(Common.Common.LogFileDirectory + "\\" + "ReconcileAndPostLog" + ".txt", true);
            Log("Process ID: " + Process.GetCurrentProcess().Id);
        }

        public void BeginReconcileAndPost()
        {
            int counter = 0;

            //We will continuously run this until the process that spawned it, kills it
            while (true)
            {
                while (versionsToRecAndPost.Count > 0)
                {
                    Log("Versions to Reconcile: " + versionsToRecAndPost.Count);
                    string versionName = versionsToRecAndPost.Dequeue();
                    string[] temp = Regex.Split(versionName, "_");
                    string feederID = temp[1];//.Replace("\0","");
                    bool postSuccess = false;
                    bool conflictsFound = false;

                    IVersion versionToPost = GetVersion(versionName);
                    IWorkspaceEdit wsEdit = versionToPost as IWorkspaceEdit;
                    if (!wsEdit.IsBeingEdited()) { wsEdit.StartEditing(false); }

                    //Reconcile and post regardless of conflicts.  We will re-submit it to trace again if there were conflicts
                    conflictsFound = Reconcile(versionToPost);
                    postSuccess = Post(versionToPost);

                    //We will actually post regardless of conflicts and the feeder will be sent back to the main process
                    //to run the trace on this feeder again to get any necessary updates
                    if (conflictsFound || !postSuccess)
                    {
                        //Conflicts were encountered so we need to place this version back into the queue to trace again.
                        MailSlot.WriteToMailSlot(Common.Common.TraceAllReTraceListMailSlot, feederID);
                    }
                    else
                    {
                        //No conflicts encountered and we were able to post
                        MailSlot.WriteToMailSlot(Common.Common.TraceAllFinishedMailSlot, feederID);
                    }

                    wsEdit.StopEditing(false);

                    //Now we can delete this version and release our resources
                    versionToPost.Delete();
                    while (Marshal.ReleaseComObject(versionToPost) > 0) { }

                    counter++;

                    //Compress every 10 reconcile and posts
                    if ((counter % 10) == 0)
                    {
                        Compress();
                    }

                    //Update our queue
                    UpdateQueue();  
                }

                Thread.Sleep(10000);   
            }
        }

        private void Compress()
        {
            Log("Compressing database...");
            IVersionedWorkspace versionedWorkspace = sdeWorkspace as IVersionedWorkspace;
            versionedWorkspace.Compress();
            Log("Finished Compressing");
        }

        private IVersion GetVersion(string versionName)
        {
            IVersionedWorkspace versionedWorkspace = workspace as IVersionedWorkspace;
            return versionedWorkspace.FindVersion(versionName);
        }

        private void UpdateQueue()
        {
            List<string> newRecAndPostVersions = RecAndPostMailSlot.ReadMailslotMessages();
            foreach(string versionName in newRecAndPostVersions)
            {
                versionsToRecAndPost.Enqueue(versionName);
            }
        }

        public void Log(string log)
        {
            writer.WriteLine(DateTime.Now.ToString() + ": " + log);
            writer.Flush();
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
                Log("Reconciling version " + version.VersionName + "...");
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
                        //This error code tells us someone else is reconciling with a lock at the same time. 
                        //So we will simply try again every 10 secs.
                        if (e.ErrorCode == -2147217137)
                        {
                            TryAgain = true;
                        }
                        System.Threading.Thread.Sleep(10000);
                    }
                }

                if (conflictsFound) { Log("Conflicts found. Resolved in favor of parent. This feeder will need to be traced again."); }

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
            try
            {
                IVersionEdit4 versionEdit = version as IVersionEdit4;
                Log("Attempting to post version...");
                if (versionEdit.CanPost())
                {
                    versionEdit.Post(Common.Common.BaseVersion);
                    Log("Version posted");
                    return true;
                }
            }
            catch (Exception e)
            {
                //If something fails then something changed in default when we tried to post.  Need to reconcile and try again.
                Log("Unable to post: " + e.Message);
            }

            Log("Version could not be posted. Parent version must have changed. These feeders must be traced again.");
            return false;
        }
    }
}
