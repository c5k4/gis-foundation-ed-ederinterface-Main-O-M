using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using PGE.BatchApplication.AUConveyor.Autoupdaters;
using PGE.BatchApplication.AUConveyor.Processing;
using PGE.BatchApplication.AUConveyor.Utilities;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using Miner.Interop;

namespace PGE.BatchApplication.AUConveyor
{
    /// <summary>
    /// Main execution point for the conveyor functionality. Starts processing, calls all necessary methods
    /// to run the conveyor and child processes if necessary.
    /// </summary>
    public class Conveyor
    {
        IWorkspace _ws = null;
        List<QueuedObjectClass> _classesToRun = null;
        AUHelper _auHelper;
        AUWrapper _auWrapper;
        bool _aborted = false;

        /// <summary>
        /// Constructor
        /// </summary>
        public Conveyor()
        {
            LogManager.WriteLine("Initializing conveyor against DB: " + ToolSettings.Instance.DatabaseLocation);
            LogManager.WriteLine("  within version: " + ToolSettings.Instance.VersionName);
            LogManager.WriteLine("  using AU ProgID: " + ToolSettings.Instance.AuProgId);
            if (ToolSettings.Instance.BypassAuStoreWhenFieldsEqual)
                LogManager.WriteLine("    only storing AU changes when a non-shape field has changed");
            if (ToolSettings.Instance.TableName != null)
                LogManager.WriteLine("  against: " + ToolSettings.Instance.TableName);
            else
                LogManager.WriteLine("  against all configured tables");
            LogManager.WriteLine("Feature save policy:");
            if (ToolSettings.Instance.UpdateRelatedIfAuNotFound)
            {
                if (ToolSettings.Instance.ForceType.HasValue)
                {
                    LogManager.WriteLine("  Update related objects of type '" + ToolSettings.Instance.ForceType.Value.ToString() + "' when AU is ineligible");
                }
                else if (ToolSettings.Instance.ForceClassNames.Count > 0)
                {
                    LogManager.WriteLine("  Update Objects from specific related classes when AU is ineligible:");
                    ToolSettings.Instance.ForceClassNames.ForEach(name => LogManager.WriteLine("    " + name));
                }
                else
                {
                    LogManager.WriteLine("  Update all related objects when AU is ineligible");
                }
            }
            else
            {
                LogManager.WriteLine("  Only attempt to fire AUs");
            }
            if (ToolSettings.Instance.RecreateRelated)
            {
                LogManager.WriteLine("  Deleting and recreating eligible relationships " + (ToolSettings.Instance.ForceType.HasValue ? "(" + ToolSettings.Instance.ForceType.Value.ToString() + ") " : ""));
                if (ToolSettings.Instance.RecreateOnlyWhenExists)
                    LogManager.WriteLine("    (only if at least one related feature was placed previously)");
            }
            if (ToolSettings.Instance.SaveRowInterval > 0)
            {
                LogManager.WriteLine("Saving every " + ToolSettings.Instance.SaveRowInterval + " rows.");
            }
            if (!string.IsNullOrEmpty(ToolSettings.Instance.CustomWhereClause))
            {
                LogManager.WriteLine("Using custom WHERE clause:");
                LogManager.WriteLine("  `" + ToolSettings.Instance.CustomWhereClause + "`");
            }
            if (!string.IsNullOrEmpty(ToolSettings.Instance.InputFile))
            {
                LogManager.WriteLine("Using input file:");
                LogManager.WriteLine("  `" + ToolSettings.Instance.InputFile + "`");
            }
            if (ToolSettings.Instance.NumProcesses > 1)
            {
                LogManager.WriteLine("Using parallel processing with " + ToolSettings.Instance.NumProcesses + " processes.");
                if (ToolSettings.Instance.GenerateInputFilesOnly)
                    LogManager.WriteLine("  No updates - input files only.", ConsoleColor.Magenta, null);
            }
            LogManager.WriteLine("");

            _ws = DBHelper.Connect(ToolSettings.Instance.DatabaseLocation);
            if (_ws != null)
            {
                //Connect to the version.
                IVersionedWorkspace vWks = _ws as IVersionedWorkspace;
                if (vWks == null)
                {
                    ConsoleKey enteredChar = ConsoleKey.NoName;
                    while (enteredChar != ConsoleKey.Y && enteredChar != ConsoleKey.N)
                    {
                        Console.Write("  This data is not versioned. Would you still like to continue (Y/N)? ");
                        enteredChar = Console.ReadKey(false).Key;
                        Console.WriteLine();
                        if (enteredChar == ConsoleKey.N)
                        {
                            _aborted = true;
                            return;
                        }
                    }
                }
                else
                    _ws = (vWks.FindVersion(ToolSettings.Instance.VersionName) as IVersionedWorkspace) as IWorkspace;

                //Find the AU, ensure it exists.
                if (!string.IsNullOrEmpty(ToolSettings.Instance.AuProgId))
                    _auWrapper = new AUWrapper(ToolSettings.Instance.AuProgId);

                //Get all feature classes supported by the AU.
                if (!string.IsNullOrEmpty(ToolSettings.Instance.AuProgId))
                {
                    LogManager.WriteLine("    Compiling list of supported object classes.");
                }

                _auHelper = new AUHelper(_ws, _auWrapper);

                if (!string.IsNullOrEmpty(ToolSettings.Instance.AuProgId))
                {
                    int configuredCnt = _auHelper.ConfiguredClasses.Count;
                    LogManager.WriteLine("      " + configuredCnt + " eligible class/subtype pairing" + (configuredCnt != 1 ? "s" : "") + " found.");
                }
           }
            else
                _aborted = true;
        }

        /// <summary>
        /// Determines whether or not the conveyor is enabled given the specified settings.
        /// </summary>
        /// <returns><c>true</c> if the conveyor is enabled, otherwise <c>false</c></returns>
        internal bool Enabled()
        {
            if (_aborted) return false;

            //If a feature class is specified distinctly, ensure that it is supported by the AU.
            //However, if updateRelatedIfAuNotFound is true, go ahead and process it anyway - just don't run the AU.
            _classesToRun = new List<QueuedObjectClass>();
            if (ToolSettings.Instance.TableName != null)
            {
                IObjectClass oc = (_ws as IFeatureWorkspace).OpenTable(ToolSettings.Instance.TableName) as IObjectClass;

                bool auIsEligibleForOC = _auHelper.IsClassEligible(oc);
                if (!auIsEligibleForOC && !ToolSettings.Instance.UpdateRelatedIfAuNotFound)
                {
                    Program.WriteError("    The specified object class (" + ToolSettings.Instance.TableName + ") is not configured for this AU.", false);
                    return false;
                }

                _classesToRun.Add(new QueuedObjectClass(oc, auIsEligibleForOC));
            }
            else
                _auHelper.ConfiguredClasses.Select(kvp => kvp.Key).ToList().ForEach(k => _classesToRun.Add(new QueuedObjectClass(k, true)));

            if (!string.IsNullOrEmpty(ToolSettings.Instance.AuProgId))
            {
                int specifiedCnt = _classesToRun.Count;
                LogManager.WriteLine("      " + specifiedCnt + " class" + (specifiedCnt != 1 ? "es" : "") + " specified to run.");
            }

            if (_classesToRun.Count == 0)
                return false;

            return true;
        }

        /// <summary>
        /// Performs the editing of features.
        /// </summary>
        internal void Convey()
        {
            if ((_classesToRun == null || _classesToRun.Count == 0) && !Enabled())
                return;

            GISEditor gisEditor = new GISEditor((IWorkspaceEdit)_ws);

            try
            {
                //Start editing.
                LogManager.WriteLine("    Starting an edit session...");
                gisEditor.StartEditing();

                //Turn off AUs.
                AUDisabler AUD = new AUDisabler();
                AUD.AUsEnabled = false;

                //Loop through object classes configured to be run.
                foreach (QueuedObjectClass classGroup in _classesToRun)
                {
                    List<string> whereClauseList = new List<string>(); //to accommodate the 1000-OID limit on "IN" statements.

                    if (!string.IsNullOrEmpty(ToolSettings.Instance.InputFile))
                    {
                        List<StringBuilder> OIDList = new List<StringBuilder>();
                        List<ProcessOIDInfo> featureLoad = CombineFeatureLoad();
                        int featureLoadCnt = 0;
                        foreach (ProcessOIDInfo f in featureLoad)
                        {
                            if (f.ClassID != classGroup.ObjectClass.ObjectClassID)
                                continue;

                            if (featureLoadCnt % 999 == 0)
                                OIDList.Add(new StringBuilder("-1"));

                            OIDList.Last().Append("," + f.OID);
                            featureLoadCnt++;
                        }

                        //Ensure that empty input files don't get processed (in an input file situation)
                        if (OIDList.Count == 0)
                            continue;

                        //Convert the input file readings into a SQL where clause.
                        OIDList.ForEach(s => whereClauseList.Add("OBJECTID IN (" + s.ToString() + ")"));
                    }
                    else if (!string.IsNullOrEmpty(ToolSettings.Instance.CustomWhereClause))
                        whereClauseList.Add(ToolSettings.Instance.CustomWhereClause);

                    classGroup.ProcessClass(ref gisEditor, _auWrapper, _auHelper, whereClauseList);

                    if (!string.IsNullOrEmpty(ToolSettings.Instance.InputFile))
                        ToolSettings.Instance.CustomWhereClause = null;
                }

                //Rec and Post if enabled.
                bool deleteVersion = false;
                if (ToolSettings.Instance.PostVersion)
                {
                    bool posted = false;
                    int attempts = 0;
                    //For now just wait a bit if something is already happening and try again.
                    string attemptText = "Attempting to reconcile & post version " + ToolSettings.Instance.VersionName;
                    LogManager.WriteLine(attemptText);
                    while (!posted && attempts < ToolSettings.Instance.MaxReconcileAttempts)
                    {
                        attempts++;
                        if (attempts > 1)
                        {
                            //This keeps the console writing to the same line for every attempt.
                            Console.Write("\r".PadRight(attemptText.Length + attempts.ToString().Length + 3));
                            Console.Write("\r");
                        }
                        LogManager.Write(attemptText + attempts + " ");
                        gisEditor.RestartEditing(true);
                        if (gisEditor.Reconcile())
                        {
                            if (gisEditor.Post())
                            {
                                gisEditor.StopEditing(true);
                                LogManager.Write("✓", ConsoleColor.Green, null);
                                posted = true;
                            }
                            else
                            {
                                gisEditor.AbortEditing();
                                LogManager.Write("X", ConsoleColor.Red, null);
                            }
                        }

                        if (!posted)
                        {
                            if (attempts == ToolSettings.Instance.MaxReconcileAttempts)
                            {
                                LogManager.WriteLine();
                                Program.WriteError("The version " + ToolSettings.Instance.VersionName + " could not be posted. Please do so manually.", true);
                                break;
                            }

                            System.Threading.Thread.Sleep(5000);
                        }
                    }
                    LogManager.WriteLine("");

                    if (posted)
                        deleteVersion = true;
                }
                else
                {
                    LogManager.WriteLine("Not going to Rec & Post...");
                }

                ProcessBitgate.ThisExitBitgate.DeleteVersion = deleteVersion;

                //Turn AUs back on.
                AUD.AUsEnabled = true;

                //Stop editing.
                gisEditor.StopEditing(true);
            }
            catch
            {
                //Abort editing.
                gisEditor.AbortEditing();

                throw;
            }
        }

        /// <summary>
        /// Splits the objects to process into multiple input files for parallel processing.
        /// </summary>
        /// <returns>
        ///     A list of class and object ID information which will indicate the objects that
        ///     will be written to each input file.
        /// </returns>
        internal List<ProcessOIDInfo> SplitFeatureLoad()
        {
            List<ProcessOIDInfo> featureLoad = new List<ProcessOIDInfo>();
            //Loop through object classes configured to be run.
            foreach (QueuedObjectClass classGroup in _classesToRun)
            {
                classGroup.SplitLoad(ref featureLoad, _auWrapper, _auHelper);
            }

            return featureLoad;
        }

        /// <summary>
        /// Takes an input file and recreates the structure used to determine the input files
        /// for parallel processing.
        /// </summary>
        /// <returns>
        ///     A list of class and object ID information which will indicate the objects that
        ///     will be written to each input file.
        /// </returns>
        internal List<ProcessOIDInfo> CombineFeatureLoad()
        {
            List<ProcessOIDInfo> featureLoad = new List<ProcessOIDInfo>();

            if (!string.IsNullOrEmpty(ToolSettings.Instance.InputFile))
            {
                StreamReader reader = new StreamReader(ToolSettings.Instance.InputFile);
                string toEnd = reader.ReadToEnd();
                string[] featureArray = Regex.Split(toEnd, "\r\n");
                reader.Close();
                foreach (string s in featureArray)
                {
                    string[] load = s.Split('.');
                    if (load.Length >= 2)
                        featureLoad.Add(new ProcessOIDInfo(Convert.ToInt32(load[1]), Convert.ToInt32(load[0])));
                }
            }

            return featureLoad;
        }

        /// <summary>
        /// Deletes a specific child version based on the base version name..
        /// </summary>
        /// <param name="baseVersionName">The name of the base version.</param>
        /// <param name="counter">The number appended to the end of the base version to indicate that it is a child.</param>
        internal void DeleteChildVersion(string baseVersionName, int counter)
        {
            string childVersionName = GetAppendedVersionName(baseVersionName, counter, true);
            try
            {
                IVersionedWorkspace versionedWorkspace = _ws as IVersionedWorkspace;
                IVersion childVersion = null;

                childVersion = versionedWorkspace.FindVersion(childVersionName);
                childVersion.Delete();
            }
            catch (Exception)
            {
                Program.WriteWarning("The auto-generated version " + childVersionName + " encountered an error when deleting.");
            }
        }

        /// <summary>
        /// Deletes all relevant numerically-appended-named versions from the base.
        /// </summary>
        /// <param name="baseVersionName">The name of the base version.</param>
        /// <param name="numVersions">The amount of child versions that need to be created.</param>
        internal void DeleteChildVersions(string baseVersionName, int numVersions)
        {
            IVersionedWorkspace versionedWorkspace = _ws as IVersionedWorkspace;
            IVersion baseVersion = null;
            try
            {
                baseVersion = versionedWorkspace.FindVersion(baseVersionName);
            }
            catch (Exception) { }

            if (baseVersion != null)
            {

                //Try to delete the children.
                for (int counter = 0; counter < numVersions; counter++)
                {
                    //Format the child version name.
                    string childVersionName = GetAppendedVersionName(baseVersionName, counter, true);

                    try
                    {
                        IVersion childVersion = versionedWorkspace.FindVersion(childVersionName);

                        //Make sure we're deleting old children and not something else.
                        if (!childVersion.HasParent())
                        {
                            throw new Exception("When attempting to delete existing process child versions, a numerically-appended version name was encountered that was NOT a child of the parent version. Delete the '" + childVersionName + "' version or use a new base version name.");
                        }
                        string parentVersionNameUnqualified = childVersion.VersionInfo.Parent.VersionName.Substring(childVersion.VersionInfo.Parent.VersionName.LastIndexOf(".") + 1);
                        string baseVersionNameUnqualified = childVersion.VersionInfo.Parent.VersionName.Substring(childVersion.VersionInfo.Parent.VersionName.LastIndexOf(".") + 1);
                        if (parentVersionNameUnqualified.ToUpper() != baseVersionNameUnqualified.ToUpper())
                            throw new Exception("When attempting to delete existing process child versions, a numerically-appended version name was encountered that was NOT a child of the parent version. Delete the '" + childVersionName + "' version or use a new base version name.");

                        childVersion.Delete();
                    }
                    catch (COMException)
                    {
                        //Check to see if the version exists under a different owner.
                        IEnumVersionInfo versions = versionedWorkspace.Versions;
                        for (IVersionInfo vi = versions.Next(); vi != null; vi = versions.Next())
                        {
                            //Get rid of the prefix, if it exists.
                            string versionName = vi.VersionName;

                            int idxOwner = versionName.LastIndexOf('.');
                            if (idxOwner > 0)
                                versionName = versionName.Substring(idxOwner + 1);

                            //Compare this to the passed-in version.
                            if (versionName == childVersionName)
                                throw new Exception("When attempting to delete existing process child versions, a version was found that was not owned by the current user. Delete the '" + childVersionName + "' version or use a new base version name.");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Creates a child version based on the current version.
        /// </summary>
        /// <param name="baseVersionName">The current, base version to use as a parent.</param>
        /// <param name="versionAppend">The number to append to the base version name when creating the child.</param>
        internal void CreateChildVersion(string baseVersionName, int versionAppend)
        {
            IVersionedWorkspace versionedWorkspace = _ws as IVersionedWorkspace;
            IVersion baseVersion = null;
            try
            {
                baseVersion = versionedWorkspace.FindVersion(baseVersionName);
            }
            catch (Exception) { }

            if (baseVersion != null)
            {
                //Format the child version name.
                string childVersionName = GetAppendedVersionName(baseVersionName, versionAppend, true);

                IVersion newVersion = baseVersion.CreateVersion(childVersionName);
            }
        }

        /// <summary>
        /// Formats a version base name for use in creating or finding a child version.
        /// </summary>
        /// <param name="baseVersionName">The name of the base version - may or may not be prefixed with the version owner.</param>
        /// <param name="versionAppend">The number to append to the end of the version for the child.</param>
        /// <param name="removePrefix">Whether or not to strip out the owner of the version if it exists.</param>
        /// <returns></returns>
        internal static string GetAppendedVersionName(string baseVersionName, int versionAppend, bool removePrefix)
        {
            string newVersionName = baseVersionName + "_" + versionAppend;

            if (removePrefix)
            {
                //Get rid of the prefix, if it exists.
                int idxOwner = newVersionName.LastIndexOf('.');
                if (idxOwner > 0)
                    newVersionName = newVersionName.Substring(idxOwner + 1);
            }

            return newVersionName;
        }
    }
}
