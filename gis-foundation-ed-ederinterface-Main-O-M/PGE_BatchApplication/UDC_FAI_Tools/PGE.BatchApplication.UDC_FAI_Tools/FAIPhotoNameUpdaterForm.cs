using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PGE.Common.Delivery.Geodatabase.ChangeDetection;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.GeoDatabaseDistributed;
using PGE.Common.Delivery.Framework.FeederManager;
using System.Runtime.InteropServices;
using Miner.Interop;
using ESRI.ArcGIS.NetworkAnalysis;
using PGE.Common.Delivery.Framework;
using ESRI.ArcGIS.esriSystem;
using System.IO;

namespace PGE.BatchApplication.UDC_FAI_Tools
{
    public partial class FAIPhotoNameUpdaterForm : Form
    {
        public FAIPhotoNameUpdaterForm()
        {
            InitializeComponent();
        }

        private void UpdateProgress(string message, int currentPosition, int maxValue, bool reportInMessage)
        {
            double progress = (((double)currentPosition) / ((double)maxValue)) * 100.0;

            if (reportInMessage) { lblProgress.Text = message + ": " + currentPosition + " of " + maxValue; }
            else { lblProgress.Text = message; }
            prgProgressBar.Value = Int32.Parse(Math.Floor(progress).ToString());
            Application.DoEvents();
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            //Specified directory does not exist
            if (!Directory.Exists(txtJPGPath.Text))
            {
                MessageBox.Show("Specified Directory does not exist");
                return;
            }

            //Get the ArcFM Login Workspace
            IMMWorkspaceManager wsManager = new MMWorkspaceManagerClass();
            IWorkspace ws = wsManager.GetWorkspace(null, (int)(mmWorkspaceOptions.mmWSOIncludeLogin));

            if (ws == null) { MessageBox.Show("Could not determine ArcFM Login Workspace"); }

            IFeatureWorkspace featWorkspace = ws as IFeatureWorkspace;
            ITable JPGMapping = null;
            try { JPGMapping = featWorkspace.OpenTable("EDGIS.PGE_FAI_JPGMAPPING"); }
            catch (Exception ex)
            {
                if (JPGMapping == null) { MessageBox.Show("Could not open table EDGIS.PGE_FAI_JPGMAPPING: " + ex.Message); }
                return;
            }

            if (JPGMapping == null)
            {
                MessageBox.Show("Could not find table EDGIS.PGE_FAI_JPGMAPPING");
                return;
            }

            int globalIDIdx = JPGMapping.Fields.FindField("GLOBALID");
            int jpgNameIdx = JPGMapping.Fields.FindField("JPGNAME");

            SortedDictionary<string, List<string>> GUIDtoJPGMap = new SortedDictionary<string, List<string>>();
            IQueryFilter qf = new QueryFilterClass();

            ICursor rowCursor = JPGMapping.Search(qf, false);
            IRow row = null;
            while ((row = rowCursor.NextRow()) != null)
            {
                if (row.get_Value(globalIDIdx) != null && row.get_Value(jpgNameIdx) != null)
                {
                    string globalID = row.get_Value(globalIDIdx).ToString();
                    string jpgName = row.get_Value(jpgNameIdx).ToString();

                    //Add this guid to JPG map
                    if (!GUIDtoJPGMap.ContainsKey(globalID))
                    {
                        GUIDtoJPGMap.Add(globalID, new List<string>());
                    }

                    GUIDtoJPGMap[globalID].Add(jpgName);
                }
            }

            //Now rename files
            List<string> files = new List<string>();
            GetFileNames(txtJPGPath.Text, ref files);

            List<string> FailedFiles = new List<string>();
            List<string> SuccessfulFiles = new List<string>();
            foreach (KeyValuePair<string, List<string>> GuidToFileName in GUIDtoJPGMap)
            {
                foreach (string file in files)
                {
                    int splitIndex = file.LastIndexOf("\\");
                    string path = file.Substring(0, splitIndex + 1);
                    string fileName = file.Substring(splitIndex + 1);
                    string fileExtension = fileName.Substring(fileName.IndexOf("."));
                    
                    for (int i = 0; i < GuidToFileName.Value.Count; i++)
                    {
                        string testFileName = GuidToFileName.Value[i]; 
                        if (testFileName == fileName)
                        {
                            string newFileName = path + GuidToFileName.Key + "_" + i + fileExtension;

                            try
                            {
                                File.Move(file, newFileName);
                                SuccessfulFiles.Add(fileName);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Unable to rename file: " + file + "\r\n" + ex.Message);
                                FailedFiles.Add(fileName);
                            }
                        }
                    }
                }
            }
            
            MessageBox.Show(string.Format("{0} files successfully renamed\r\n{1} files failed to rename", SuccessfulFiles.Count, FailedFiles.Count));
        }

        private void GetFileNames(string directory, ref List<string> files)
        {
            string[] directoryFiles = Directory.GetFiles(directory);
            foreach (string fileName in directoryFiles)
            {
                if (fileName.ToUpper().EndsWith(".JPG"))
                {
                    files.Add(fileName);
                    /*
                    string fileNameShort = fileName.Substring(fileName.LastIndexOf("\\"));
                    fileNameShort = fileNameShort.Substring(0, fileName.LastIndexOf("."));
                    try 
                    {
                        //If the file name is already a GUID then it has already been renamed
                        Guid guid = new Guid(fileNameShort); 
                    }
                    catch 
                    {
                        //File has not already been renamed so we can add this one.
                        files.Add(fileName);
                    }
                     * */
                }
            }

            foreach (string subDirectory in Directory.GetDirectories(directory))
            {
                GetFileNames(subDirectory, ref files);
            }
        }
    }
}
