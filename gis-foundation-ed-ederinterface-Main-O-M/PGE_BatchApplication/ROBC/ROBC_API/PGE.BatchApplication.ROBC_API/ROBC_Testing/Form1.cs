using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Geodatabase;
using ROBC_API;
using ROBC_API.DatabaseRecords;

using System.Runtime.InteropServices;


using ESRI.ArcGIS.DataSourcesGDB;
using System.Diagnostics;

namespace ROBC_Testing
{
    public partial class Form1 : Form
    {
        public ROBCManager robcManager = new ROBCManager("LBGISS2Q", "gis_i_write", "gis_i_write", "LBGISG1Q", "gis_i_write", "gis_i_write", "EDGIS.ElectricDataset\\EDGIS.ElectricDistNetwork", "EDGIS.SubstationDataset\\EDGIS.SubGeometricNetwork");
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                try
                {
                    CircuitSourceRecord circuitRecord = robcManager.FindCircuit(tbCircuitIDToTest.Text, true, true);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + " \r\n " + ex.StackTrace);
                }

                PCPRecord pcpRecord = robcManager.FindPCP("{424DE973-7D82-441D-A8DC-537BC4632887}");
                //pcpRecord.SetFieldValue("GLOBALID", "{425DE973-7D82-441D-A8DC-537BC4632887}");
                //robcManager.UpdatePCP(pcpRecord);
                //ROBCRecord robcRecord = robcManager.FindPCPROBC("{CA0126E6-1A8A-4C46-9228-3AF8E07033B0}");
                //PCPRecord pcpRecordByGUID = robcManager.FindPCPByGlobalID("{CA0126E6-1A8A-4C46-9228-3AF8E07033B0}");
                //List<BaseRecord> pcpRecords = robcManager.FindDevices("254072113", "10503");
                //List<BaseRecord> pcpRecords2 = robcManager.FindDevices("254072111", "4022");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " \r\n " + ex.StackTrace);
            }

            /*
            try
            {
                CircuitSourceRecord circuitRecord = robcManager.FindCircuit("013111105");
            }
            catch (Exception ex) 
            { }

            try
            {
                CircuitSourceRecord circuitRecord = robcManager.FindCircuit("013111105");
                ROBCRecord robcRecord = robcManager.FindCircuitROBC(circuitRecord.GlobalID.ToString());
            }
            catch (Exception ex)
            { }

            try
            {
                CircuitSourceRecord circuitRecord = robcManager.FindCircuit("013111105");
                ROBCRecord newROBCRecord = new ROBCRecord();
                newROBCRecord.AddField("CIRCUITSOURCEGUID", circuitRecord.GlobalID);
                newROBCRecord = robcManager.CreateROBC(newROBCRecord);
            }
            catch (Exception ex)
            { }

            try
            {
                CircuitSourceRecord circuitRecord = robcManager.FindCircuit("013111105");

                ROBCRecord robcRecord = robcManager.FindCircuitROBC(circuitRecord.GlobalID.ToString());
                robcRecord.SetFieldValue("SUBBLOCK", "S");

                robcRecord = robcManager.UpdateROBC(robcRecord);
            }
            catch (Exception ex)
            { }

            try
            {
                List<CircuitSourceRecord> circuitRecords = robcManager.FindCircuit("MARYSVILLE", "1103");
            }
            catch (Exception ex)
            { }

            try
            {
                Dictionary<string, string> domain = robcManager.GetDomainValues("Electric Object Class Model Name");
            }
            catch (Exception ex)
            { }

            try
            {
                CircuitSourceRecord circuitRecord = robcManager.FindCircuit("013111105");
                List<CircuitSourceRecord> circuitRecords = robcManager.GetFeedingFeeders(circuitRecord);

                List<CircuitSourceRecord> circuitChildRecords = robcManager.GetChildFeeders(circuitRecord);
            }
            catch (Exception ex)
            { }

            try
            {
                CircuitSourceRecord circuitRecordTwo = robcManager.FindCircuit("258531101");
                CircuitSourceRecord circuitRecordOne = robcManager.FindCircuit("258851101");
                List<CircuitSourceRecord> circuitSourceRecords = new List<CircuitSourceRecord>();
                circuitSourceRecords.Add(circuitRecordOne);
                circuitSourceRecords.Add(circuitRecordTwo);
                List<ROBCRecord> robcRecords = robcManager.FindCircuitROBCs(circuitSourceRecords);

                ROBCRecord newROBCRecordOne = new ROBCRecord();
                ROBCRecord newROBCRecordTwo = new ROBCRecord();
                newROBCRecordOne.AddField("CIRCUITSOURCEGUID", circuitRecordOne.GlobalID);
                //newROBCRecordOne = robcManager.CreateCircuitROBC(newROBCRecordOne);

                newROBCRecordTwo.AddField("CIRCUITSOURCEGUID", circuitRecordTwo.GlobalID);
                //newROBCRecordTwo = robcManager.CreateCircuitROBC(newROBCRecordTwo);

                List<ROBCRecord> newROBCRecords = new List<ROBCRecord>();
                newROBCRecords.Add(newROBCRecordOne);
                newROBCRecords.Add(newROBCRecordTwo);
                robcManager.SaveROBCs(newROBCRecords);
            }
            catch (Exception ex)
            { }

            try
            {
                List<CircuitSourceRecord> circuitsWithoutROBCs = robcManager.GetCircuitsWithoutROBC();
                List<ROBCRecord> robcRecordsWithoutCircuit = robcManager.GetROBCWithoutCircuit();
            }
            catch (Exception ex)
            {

            }
            */
        }

        private void cmdECustCount_Click(object sender, EventArgs e)
        {

            DateTime startTime = DateTime.Now;
            try
            {   // recommend testing 182631107  
                List<CircuitSourceRecord> listCircuitRecord = robcManager.GetCircuitsWithoutROBCWithoutLoadingInformation();
                TimeSpan ts = DateTime.Now - startTime;
                string tempString="";
                foreach (CircuitSourceRecord cr in listCircuitRecord)
                {
                    tempString = tempString+ ",\r\n" + cr.GlobalID.ToString() + ":" + cr.GetFieldValue("CIRCUITID").ToString();
                }
                string timeTaken = ts.Hours.ToString() + ":" + ts.Minutes.ToString() + ":" + ts.Seconds.ToString();
                MessageBox.Show("Found CircuitSource records were " + tempString + " this took " + timeTaken + " seconds and memory is currently: " + System.GC.GetTotalMemory(true).ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " \r\n " + ex.StackTrace);
            }
        }

        private void bPCPGetCount_Click(object sender, EventArgs e)
        {
            DateTime startTime = DateTime.Now;
            try
            {
                // "EDGIS.SWITCH" Circuit= 182631107 , OPERATINGNUMBER=153300, GUID="{092F95E3-9603-4807-8783-CB0CFFF44116}"
                //int zzz = robcManager.getDeviceCustomerCount("{092F95E3-9603-4807-8783-CB0CFFF44116}","EDGIS.SWITCH");
                TimeSpan ts = DateTime.Now-startTime ;
                string timeTaken = ts.Hours.ToString() + ":" + ts.Minutes.ToString() + ":" + ts.Seconds.ToString();
                MessageBox.Show("Button unused " + " this took " + timeTaken + " seconds and memory is currently: " + System.GC.GetTotalMemory(true).ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " \r\n " + ex.StackTrace);
            }
        }

        private void bParentChildIDs_Click(object sender, EventArgs e)
        {
            DateTime startTime = DateTime.Now;
            try
            {
                // 022101107  is fedby  2210BK1 and feeds 2247BK2 022470402
                Dictionary<string, ROBCManager.NetworkSourceType> listChildCircuits = robcManager.GetChildCircuitsForCircuitID(tbCircuitIDToTest.Text);
                Dictionary<string, ROBCManager.NetworkSourceType> listParentCircuits = robcManager.GetParentCircuitsForCircuitID(tbCircuitIDToTest.Text);
                
                TimeSpan ts = DateTime.Now - startTime;
                string timeTaken = ts.Hours.ToString() + ":" + ts.Minutes.ToString() + ":" + ts.Seconds.ToString();
                string listEDERChildCircuits = "";
                string listSubstationChildCircuits = "";
                string listEDERParentCircuits = "";
                string listSubstationParentCircuits = "";
                foreach (string keyValue in listChildCircuits.Keys)
                {
                    if (listChildCircuits[keyValue] == ROBCManager.NetworkSourceType.EDER)
                    {
                        listEDERChildCircuits = listEDERChildCircuits + "," + keyValue;
                    }
                    if (listChildCircuits[keyValue] == ROBCManager.NetworkSourceType.SUBSTATION)
                    {
                        listSubstationChildCircuits = listSubstationChildCircuits + "," + keyValue;
                    }
                }
                foreach (string keyValue in listParentCircuits.Keys)
                {
                    if (listParentCircuits[keyValue] == ROBCManager.NetworkSourceType.EDER)
                    {
                        listEDERParentCircuits = listEDERParentCircuits + "," + keyValue;
                    }
                    if (listParentCircuits[keyValue] == ROBCManager.NetworkSourceType.SUBSTATION)
                    {
                        listSubstationParentCircuits = listSubstationParentCircuits + "," + keyValue;
                    }
                }
                MessageBox.Show("EDER Circuits Found Children of " + listEDERChildCircuits + " and Child Sustation Circuits found: " + listSubstationChildCircuits +
                    " \r\n" + " EDER Circuits Found Parent of " + listEDERParentCircuits + " and Parent Sustation Circuits found: " + listSubstationParentCircuits + 
                    "\r\n this took " + timeTaken + "  and memory is currently: " + System.GC.GetTotalMemory(true).ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " \r\n " + ex.StackTrace);
            }

        }

        private void cmdLoadInfo_Click(object sender, EventArgs e)
        {
            try
            {
                string testCircuit = (tbCircuitIDToTest.Text).Trim();
                DateTime startTime = DateTime.Now;
                CircuitSourceRecord circuitRecord = robcManager.FindCircuit(testCircuit, true, true);
                robcManager.GetLoadingInformation(ref circuitRecord, true);
                TimeSpan ts = DateTime.Now - startTime;
                string timeTaken = ts.Hours.ToString() + ":" + ts.Minutes.ToString() + ":" + ts.Seconds.ToString();
                MessageBox.Show("For Circuit of " + testCircuit + "  Circuits Found of " + circuitRecord.GlobalID.ToString() + " loading information of: " +
                    " \r\n" + " IsScada of " + circuitRecord.IsScada.ToString() +
                    " \r\n" + " TotalCustomers of " + circuitRecord.TotalCustomers.ToString() +
                    " \r\n" + " Summer KVA of " + circuitRecord.SummerKVA.ToString() +
                    " \r\n" + " Winter KVA of " + circuitRecord.WinterKVA.ToString() + 
                    "\r\n this took " + timeTaken + "  and memory is currently: " + System.GC.GetTotalMemory(true).ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " \r\n " + ex.StackTrace);
            }
            
        }

        private void cmdSCADA_Click(object sender, EventArgs e)
        {
            DateTime startTime = DateTime.Now;
            try
            {
                CircuitSourceRecord circuitRecord = robcManager.FindCircuit(tbCircuitIDToTest.Text, true, true);
                robcManager.IsScada(circuitRecord);
                TimeSpan ts = DateTime.Now - startTime;
                string timeTaken = ts.Hours.ToString() + ":" + ts.Minutes.ToString() + ":" + ts.Seconds.ToString();
                MessageBox.Show("Found CircuitSource record  was " + circuitRecord.ToString() + " this took " + timeTaken + " seconds and memory is currently: " + System.GC.GetTotalMemory(true).ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " \r\n " + ex.StackTrace);
            }
        }

        private void bCheckCircuitForEssentialCustomer_Click(object sender, EventArgs e)
        {
             DateTime startTime = DateTime.Now;
            try
            {
                bool returnVal = robcManager.hasEssentialCustomerForCircuit(tbCircuitIDToTest.Text);
                TimeSpan ts = DateTime.Now - startTime;
                string timeTaken = ts.Hours.ToString() + ":" + ts.Minutes.ToString() + ":" + ts.Seconds.ToString();
                MessageBox.Show("Found CircuitSource record  was " + returnVal.ToString() + " this took " + timeTaken + " seconds and memory is currently: " + System.GC.GetTotalMemory(true).ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " \r\n " + ex.StackTrace);
            }
            
        }

        private void bCustomerCountforCircuit_Click(object sender, EventArgs e)
        {
            DateTime startTime = DateTime.Now;
            try
            {
                int returnVal = robcManager.getCircuitCustomerCount(tbCircuitIDToTest.Text);
                //int returnVal = robcManager.getCircuitCustomerCount("022470402");
                TimeSpan ts = DateTime.Now - startTime;
                string timeTaken = ts.Hours.ToString() + ":" + ts.Minutes.ToString() + ":" + ts.Seconds.ToString();
                MessageBox.Show("Found count of customers was " + returnVal.ToString() + " this took " + timeTaken + " seconds and memory is currently: " + System.GC.GetTotalMemory(true).ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " \r\n " + ex.StackTrace);
            }
        }

        private void bHasEssentialCustomerPartialCurtailmentPoint_Click(object sender, EventArgs e)
        {
            string tempDevice = (tbDeviceGuid.Text).Trim();
            DateTime startTime = DateTime.Now;
            try
            {
                // "EDGIS.SWITCH" Circuit= 182631107 , OPERATINGNUMBER=153300, GUID="{092F95E3-9603-4807-8783-CB0CFFF44116}"
                bool zzz = robcManager.hasEssentialCustomerForDevice(tempDevice);
                TimeSpan ts = DateTime.Now - startTime;
                string timeTaken = ts.Hours.ToString() + ":" + ts.Minutes.ToString() + ":" + ts.Seconds.ToString();
                MessageBox.Show("Test of customers was found to be " + zzz.ToString() + " this took " + timeTaken + " seconds and memory is currently: " + System.GC.GetTotalMemory(true).ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message+" \r\n "+ex.StackTrace);
            }
        }

        private void bCustomerCountforPCP_Click(object sender, EventArgs e)
        {
            string tempDevice = (tbDeviceGuid.Text).Trim();
            DateTime startTime = DateTime.Now;
            try
            {
                // "EDGIS.SWITCH" Circuit= 182631107 , OPERATINGNUMBER=153300, GUID="{092F95E3-9603-4807-8783-CB0CFFF44116}"
                //PCPRecord pcpRecord = robcManager.FindPCP(tempDevice);
                int zzz = robcManager.getDeviceCustomerCount(tempDevice);
                TimeSpan ts = DateTime.Now - startTime;
                string timeTaken = ts.Hours.ToString() + ":" + ts.Minutes.ToString() + ":" + ts.Seconds.ToString();
                MessageBox.Show("Count of customers was found to be " + zzz.ToString() + " this took " + timeTaken + " seconds and memory is currently: " + System.GC.GetTotalMemory(true).ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " \r\n " + ex.StackTrace);
            }
        }

        private void bInvalidPCPs_Click(object sender, EventArgs e)
        {
            DateTime startTime = DateTime.Now;
            try
            {
                // "EDGIS.SWITCH" Circuit= 182631107 , OPERATINGNUMBER=153300, GUID="{092F95E3-9603-4807-8783-CB0CFFF44116}"
                Dictionary<PCPRecord,BaseRecord[]> dictResults = robcManager.getInvalidPCPs();
                TimeSpan ts = DateTime.Now - startTime;
                string timeTaken = ts.Hours.ToString() + ":" + ts.Minutes.ToString() + ":" + ts.Seconds.ToString();
                string total_pcp_info = "";
                foreach (PCPRecord pr in dictResults.Keys)
                {
                    ROBCRecord tempROBC = null;
                    CircuitSourceRecord tempCircuitSource = null;
                    SwitchRecord tempSwitch = null;
                    DPDRecord tempDPD = null;

                    BaseRecord[] arrayBRs = dictResults[pr];
                    if (arrayBRs != null)
                    {
                        int iMax = arrayBRs.Rank;
                        if (0 < iMax-1)
                        {
                            tempROBC = arrayBRs[0] as ROBCRecord;
                        }
                        if (1 < iMax - 1)
                        {
                            tempCircuitSource = arrayBRs[1] as CircuitSourceRecord;
                        }
                        if (2 < iMax - 1)
                        {
                            tempSwitch = arrayBRs[2] as SwitchRecord;
                        }
                        if (3 < iMax - 1)
                        {
                            tempDPD = arrayBRs[2] as DPDRecord;
                        }
                    }
                    total_pcp_info = total_pcp_info + "\r\nFound the PCP of:" + pr.GlobalID.ToString();
                }
                MessageBox.Show("The PCPs found were " + total_pcp_info + " this took " + timeTaken + " seconds and memory is currently: " + System.GC.GetTotalMemory(true).ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " \r\n " + ex.StackTrace);
            }
        }

        private void bFindDevice_Click(object sender, EventArgs e)
        {
            DateTime startTime = DateTime.Now;
            try
            {
                // "EDGIS.SWITCH" Circuit= 182631107 , OPERATINGNUMBER=153300, GUID="{092F95E3-9603-4807-8783-CB0CFFF44116}"
                List<BaseRecord> listResults = robcManager.FindDevices((tbCircuitIDToTest.Text).Trim(), (tbOPNumber.Text).Trim());
                TimeSpan ts = DateTime.Now - startTime;
                string timeTaken = ts.Hours.ToString() + ":" + ts.Minutes.ToString() + ":" + ts.Seconds.ToString();
                string total_pcp_info = "";
                foreach (BaseRecord br in listResults)
                {
                    string typeDevice = "Type_Not_Defined";
                    if (br is SwitchRecord)
                    {
                        typeDevice = "Switch";
                    }
                    if (br is DPDRecord)
                    {
                        typeDevice = "DPD";
                    }
                    total_pcp_info = total_pcp_info + "\r\nFound the " + typeDevice + " of:" + br.GlobalID.ToString();
                }
                MessageBox.Show("The PCPs found were " + total_pcp_info + " this took " + timeTaken + " seconds and memory is currently: " + System.GC.GetTotalMemory(true).ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " \r\n " + ex.StackTrace);
            }
        }

        private void cmdCreatePCP_Click(object sender, EventArgs e)
        {
            try
            {
                PCPRecord pcpRecord = new PCPRecord();
                pcpRecord.GlobalID = "{CED1434D-BA92-4525-BFCE-25062D1B42AB}";
                robcManager.CreatePCP(pcpRecord);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " \r\n " + ex.StackTrace);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //182821103
            //182742102
            //083192101

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            //CircuitSourceRecord circuitRecord = robcManager.FindCircuit(tbCircuitIDToTest.Text);
            CircuitSourceRecord circuitRecord = robcManager.FindCircuit_NoLoadingNoScada(tbCircuitIDToTest.Text);
            robcManager.GetLoadingInformation(ref circuitRecord, true);
            stopwatch.Stop();
        }

        private void tbCircuitIDToTest_TextChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            //{B0A9259A-841B-47D0-BC4D-478A2962CAD2}
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            int customerCount = robcManager.getDeviceCustomerCount("{B0A9259A-841B-47D0-BC4D-478A2962CAD2}");
            stopwatch.Stop();
        }
    }
}
