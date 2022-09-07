using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using System.Reflection;
using System.IO;
using PGE.BatchApplication.CWOSLAssignmentTool.classes;
using System.Text.RegularExpressions;
using System.Threading;
using PGE_DBPasswordManagement;

namespace PGE.BatchApplication.CWOSLAssignmentTool
{
    public partial class CWOSLForm : Form
    {
        // M4JF EDGISREARC 919
        public static string EDERConnectionString = ReadEncryption.GetConnectionStr(ConfigurationManager.ConnectionStrings["EDER_ConnectionStr"].ConnectionString.ToUpper());
        private static string logPath = (String.IsNullOrEmpty(ConfigurationManager.AppSettings["LOGPATH"]) ? System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) : ConfigurationManager.AppSettings["LOGPATH"]) + "\\Logfile_" + DateTime.Now.Ticks.ToString() + ".log";
        private static int LogRefreshIntervalSec = Convert.ToInt16(Convert.ToString(ConfigurationManager.AppSettings["LogRefreshIntervalSec"]));
        private static StreamWriter pSWriter = default(StreamWriter);
        private IDictionary<string, string> transSubTypeList = new Dictionary<string, string>();
        private IDictionary<string, string> hftdCodeValue = new Dictionary<string, string>();
        private IDictionary<string, string> divisionCodeValue = new Dictionary<string, string>();
        private IDictionary<string, string> countyCodeValue = new Dictionary<string, string>();
        private IDictionary<string, string> localOfcIdCodeValue = new Dictionary<string, string>();
        private IDictionary<string, string> localOfcIdCodeValueUnknUnsp = new Dictionary<string, string>();
        private string noOfFeaturesPrevValue;
        private string programRunTimePrevValue;
        private System.Windows.Forms.Timer logTimer = new System.Windows.Forms.Timer();

        public CWOSLForm()
        {
            InitializeComponent();
            (pSWriter = File.CreateText(logPath)).Close();
            WriteLine(DateTime.Now.ToLongTimeString() + "PGE EDGIS CWOSL Assignment Tool Window opened.");
        }

        private static void WriteLine(string sMsg)
        {
            pSWriter = File.AppendText(logPath);
            pSWriter.WriteLine(sMsg);
            Console.WriteLine(sMsg);
            pSWriter.Close();
        }

        private void CWOSLForm_Load(object sender, EventArgs e)
        {
            try
            {
                PopulateDomianValues();
                PopulateTransformerSubType();
                PopulateDefaultValues();
                WriteLine(DateTime.Now.ToLongTimeString() + "Dropdown values populated.");
                CheckCWOSLSession();
            }
            catch (Exception ex)
            {
                WriteLine(DateTime.Now.ToLongTimeString() + "Error in loading dropdown values. " + ex.Message);
                MessageBox.Show("Error in loading dropdown values");
            }
        }

        private void PopulateDomianValues()
        {
            string sqlString = "select * from " + Convert.ToString(ConfigurationManager.AppSettings["EDER_DOMAIN_TABLE"])
                + " where domain_name in ('" + Convert.ToString(ConfigurationManager.AppSettings["HFTDDomainName"]) + "', '"
                + Convert.ToString(ConfigurationManager.AppSettings["DivisionDomainName"]) + "', '"
                + Convert.ToString(ConfigurationManager.AppSettings["CountyDomainName"]) + "', '"
                + Convert.ToString(ConfigurationManager.AppSettings["LocalOfcIdDomainName"]) + "')";

            DataTable domainDataTable = DBhelper.GetDataTableByQuery(sqlString);

            foreach (DataRow dr in domainDataTable.Rows)
            {
                //populate HFTD dictionary
                if (Convert.ToString(dr["DOMAIN_NAME"]) == Convert.ToString(ConfigurationManager.AppSettings["HFTDDomainName"]))
                {
                    if (!hftdCodeValue.Keys.Contains(Convert.ToString(dr["CODE"])))
                    {
                        hftdCodeValue.Add(Convert.ToString(dr["CODE"]), Convert.ToString(dr["DESCRIPTION"]));
                    }
                }

                //populate Division dictionary
                if (Convert.ToString(dr["DOMAIN_NAME"]) == Convert.ToString(ConfigurationManager.AppSettings["DivisionDomainName"]))
                {
                    if (!divisionCodeValue.Keys.Contains(Convert.ToString(dr["CODE"])))
                    {
                        divisionCodeValue.Add(Convert.ToString(dr["CODE"]), Convert.ToString(dr["DESCRIPTION"]));
                    }
                }

                //populate County dictionary
                if (Convert.ToString(dr["DOMAIN_NAME"]) == Convert.ToString(ConfigurationManager.AppSettings["CountyDomainName"]))
                {
                    if (!countyCodeValue.Keys.Contains(Convert.ToString(dr["CODE"])))
                    {
                        countyCodeValue.Add(Convert.ToString(dr["CODE"]), Convert.ToString(dr["DESCRIPTION"]));
                    }
                }

                //populate Local Office ID dictionary
                if (Convert.ToString(dr["DOMAIN_NAME"]) == Convert.ToString(ConfigurationManager.AppSettings["LocalOfcIdDomainName"]))
                {
                    if (Convert.ToString(dr["DESCRIPTION"]).ToUpper().StartsWith("UNKNOWN") || Convert.ToString(dr["DESCRIPTION"]).ToUpper().StartsWith("UNSPECIFIED"))
                    {
                        if (!localOfcIdCodeValueUnknUnsp.Keys.Contains(Convert.ToString(dr["CODE"])))
                        {
                            localOfcIdCodeValueUnknUnsp.Add(Convert.ToString(dr["CODE"]), Convert.ToString(dr["DESCRIPTION"]));
                        }
                    }
                    else
                    {
                        if (!localOfcIdCodeValue.Keys.Contains(Convert.ToString(dr["CODE"])))
                        {
                            localOfcIdCodeValue.Add(Convert.ToString(dr["CODE"]), Convert.ToString(dr["DESCRIPTION"]));
                        }
                    }
                }
            }

            //sorting of domain descriptions
            var hftdSortedValues = from name in hftdCodeValue.Values.ToArray()
                                   orderby name
                                   select name;
            var divisionSortedValues = from name in divisionCodeValue.Values.ToArray()
                                       orderby name
                                       select name;
            var countySortedValues = from name in countyCodeValue.Values.ToArray()
                                     orderby name
                                     select name;
            var localOfcIdSortedValues = from name in localOfcIdCodeValue.Values.ToArray()
                                         orderby name
                                         select name;
            var localOfcIdUnknUnspSortedValues = from name in localOfcIdCodeValueUnknUnsp.Values.ToArray()
                                                 orderby name
                                                 select name;

            var combinedLocalOfcIdArr = localOfcIdSortedValues.ToArray().Union(localOfcIdUnknUnspSortedValues.ToArray());
            localOfcIdCodeValue.Union(localOfcIdCodeValueUnknUnsp);

            //add blank values in dropdown
            HFTDCombo.Items.Insert(0, "");
            DivisionCombo.Items.Insert(0, "");
            CountyCombo.Items.Insert(0, "");
            LocalOfficeIdCombo.Items.Insert(0, "");

            //add domain values in dropdown
            HFTDCombo.Items.AddRange(hftdSortedValues.ToArray());
            DivisionCombo.Items.AddRange(divisionSortedValues.ToArray());
            CountyCombo.Items.AddRange(countySortedValues.ToArray());
            LocalOfficeIdCombo.Items.AddRange(combinedLocalOfcIdArr.ToArray());

            //set default selection to blank value in dropdwons
            HFTDCombo.SelectedIndex = 0;
            DivisionCombo.SelectedIndex = 0;
            CountyCombo.SelectedIndex = 0;
            LocalOfficeIdCombo.SelectedIndex = 0;
            SecLoadPointCombo.SelectedIndex = 0;
            SketchPseudoCombo.SelectedIndex = 0;
        }

        private void PopulateTransformerSubType()
        {
            string sqlString = Convert.ToString(ConfigurationManager.AppSettings["TransformerSubTypeQuery"]);
            DataTable tranSubTypeDataTable = DBhelper.GetDataTableByQuery(sqlString);
            foreach (DataRow dr in tranSubTypeDataTable.Rows)
            {
                if (!transSubTypeList.Keys.Contains(Convert.ToString(dr["SUBTYPECD"])))
                {
                    transSubTypeList.Add(Convert.ToString(dr["SUBTYPECD"]), Convert.ToString(dr["DESCRIPTION"]));
                }
            }
            var transSubtypeValues = from name in transSubTypeList.Values.ToArray()
                                     orderby name
                                     select name;

            TransformerSubTypeCombo.Items.Insert(0,"");
            TransformerSubTypeCombo.Items.AddRange(transSubtypeValues.ToArray());
            TransformerSubTypeCombo.SelectedIndex = 0;
        }

        private void PopulateDefaultValues()
        {
            try
            {
                //fetch default values from CWOSL.config
                string[] defaultValues = PGE.BatchApplication.CWOSL.Program.GetNoFeat_RunTime();
                NoOfFeaturesText.Text = Convert.ToString(defaultValues[0]);
                ProgramRunTimeText.Text = Convert.ToString(defaultValues[1]);
            }
            catch (Exception ex)
            {
                WriteLine(DateTime.Now.ToLongTimeString() + "Error in fetching No. of features and Program run time from CWOSL.config. " + ex.Message);
            }
        }

        private void CheckCWOSLSession()
        {
            try
            {
                WriteLine(DateTime.Now.ToLongTimeString() + "Checking if CWOSL Session is avaialable in database in PROCESS.MM_SESSION table.");
                string sqlString = Convert.ToString(ConfigurationManager.AppSettings["SessionNameQuery"]);
                DataTable sessionNameDataTable = DBhelper.GetDataTableByQuery(sqlString);
                 if (sessionNameDataTable != null && sessionNameDataTable.Rows.Count > 0)
                {
                    string sessionName = Convert.ToString(sessionNameDataTable.Rows[0][Convert.ToString(ConfigurationManager.AppSettings["SessionNameField"])]);
                    WriteLine(DateTime.Now.ToLongTimeString() + "Session found - " + sessionName);
                    SessionNameLabel.Text = "Session - " + sessionName;

                    //enable-disable fields / buttons
                    PostBtn.Enabled = true;
                    AcceptButton = PostBtn;
                    SubmitBtn.Enabled = false;
                    EnableDisableNonMandatoryFields(false);
                    EnableDisableMandatoryFields(false);
                    FullRunChkBox.Enabled = false;
                }
                else
                {
                    WriteLine(DateTime.Now.ToLongTimeString() + "No CWOSL Session found.");
                    SessionNameLabel.Text = "";

                    //enable-disable fields / buttons
                    PostBtn.Enabled = false;
                    SubmitBtn.Enabled = true;
                    AcceptButton = SubmitBtn;
                    EnableDisableMandatoryFields(true);
                    FullRunChkBox.Enabled = true;

                    if (FullRunChkBox.Checked == true)
                    {
                        EnableDisableNonMandatoryFields(false);
                    }
                    else
                    {
                        EnableDisableNonMandatoryFields(true);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLine(DateTime.Now.ToLongTimeString() + "Error in fetching CWOSL session. " + ex.Message);
                MessageBox.Show("Error in fetching CWOSL session");
            }
        }

        private void SubmitBtn_Click(object sender, EventArgs e)
        {
            try
            {
                WriteLine(DateTime.Now.ToLongTimeString() + "Submit button clicked.");
                LogsText.Text = "";

                if (NoOfFeaturesText.Text.Trim().Length == 0 || ProgramRunTimeText.Text.Trim().Length == 0)
                {
                    MessageBox.Show("Please enter No. of Features and Program Run Time (min.) to proceed.");
                    WriteLine(DateTime.Now.ToLongTimeString() + "Either/Both No. of Featues and Program Run Time are null. Prompted user to enter the values.");
                    return;
                }

                //valiate cicuit ID
                if (CircuitIdText.Enabled == true && CircuitIdText.Text.Trim().Length > 0)
                {
                    if (isInvalidCircuitID())
                    {
                        WriteLine(DateTime.Now.ToLongTimeString() + "Circuit ID enetered is incorrect.");
                        MessageBox.Show("Please enter valid Circuit ID. Circuit ID can be alpha-numeric value with 9 characters.");
                        return;
                    }
                }

                StatusLabel.Text = "Processing...";
                SubmitBtn.Enabled = false;
                EnableDisableMandatoryFields(false);
                EnableDisableNonMandatoryFields(false);
                FullRunChkBox.Enabled = false;

                string selectedHftd = "null";
                string selectedDivision = "null";
                string selectedCounty = "null";
                string selectedLocalOffice = "null";
                string selectedTransSubType = "null";
                string selectedSecLoadPt = "null";
                string selectedSketchPseudo = "null";
                string numOfFeaturesVal = "null";
                string programRunTimeVal = "null";
                string circuitIdVal = "null";
                string fullRun = Convert.ToString(FullRunChkBox.Checked);

                if (Convert.ToString(HFTDCombo.SelectedItem) != "")
                {
                    selectedHftd = hftdCodeValue.FirstOrDefault(x => x.Value == Convert.ToString(HFTDCombo.SelectedItem)).Key;
                }
                if (Convert.ToString(DivisionCombo.SelectedItem) != "")
                {
                    selectedDivision = divisionCodeValue.FirstOrDefault(x => x.Value == Convert.ToString(DivisionCombo.SelectedItem)).Key;
                }
                if (Convert.ToString(CountyCombo.SelectedItem) != "")
                {
                    selectedCounty = countyCodeValue.FirstOrDefault(x => x.Value == Convert.ToString(CountyCombo.SelectedItem)).Key;
                }
                if (Convert.ToString(LocalOfficeIdCombo.SelectedItem) != "")
                {
                    selectedLocalOffice = localOfcIdCodeValue.FirstOrDefault(x => x.Value == Convert.ToString(LocalOfficeIdCombo.SelectedItem)).Key;
                }
                if (Convert.ToString(TransformerSubTypeCombo.SelectedItem) != "")
                {
                    selectedTransSubType = transSubTypeList.FirstOrDefault(x => x.Value == Convert.ToString(TransformerSubTypeCombo.SelectedItem)).Key;
                }
                if (Convert.ToString(SecLoadPointCombo.SelectedItem) == "Include")
                {
                    selectedSecLoadPt = "INC";
                }
                else if (Convert.ToString(SecLoadPointCombo.SelectedItem) == "Exclude")
                {
                    selectedSecLoadPt = "EXC";
                }
                if (Convert.ToString(SketchPseudoCombo.SelectedItem) == "Include")
                {
                    selectedSketchPseudo = "INC";
                }
                else if (Convert.ToString(SketchPseudoCombo.SelectedItem) == "Exclude")
                {
                    selectedSketchPseudo = "EXC";
                }
                if(Convert.ToString(NoOfFeaturesText.Text) != "")
                {
                    numOfFeaturesVal = Convert.ToString(NoOfFeaturesText.Text);
                }
                if (Convert.ToString(ProgramRunTimeText.Text) != "")
                {
                    programRunTimeVal = Convert.ToString(ProgramRunTimeText.Text);
                }
                if (Convert.ToString(CircuitIdText.Text) != "")
                {
                    circuitIdVal = Convert.ToString(CircuitIdText.Text);
                }

                string[] cwoslParameters = new string[] { selectedHftd, selectedDivision, selectedCounty, selectedLocalOffice ,
                selectedSecLoadPt, selectedTransSubType, selectedSketchPseudo, numOfFeaturesVal.Trim(), programRunTimeVal.Trim(), fullRun, circuitIdVal};
                WriteLine(DateTime.Now.ToLongTimeString() + "Parameters to be passed to PGE.CWOSL console (HFTD, Division, County, Local Office ID, Secondary Load Point, Transformer SubType, Sketch Pseudoservice Features, Number of Features, Program Run Time (min.), Full Run, Citcuit ID): " + String.Join(", ", cwoslParameters));
                WriteLine(DateTime.Now.ToLongTimeString() + "Call PGE.CWOSL console.");


                Action onCompleted = () =>
                {
                    System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
                    timer.Interval = (LogRefreshIntervalSec * 1000);
                    timer.Tick += (s, ev) =>
                    {
                        timer.Stop();
                        StatusLabel.Text = "Completed. Please check LOGS.";
                        logTimer.Stop();
                        CheckCWOSLSession();
                    };
                    timer.Start();
                };

                var thread = new Thread(
                  () =>
                  {
                      try
                      {
                          CallCWOSLMainMethod(cwoslParameters);
                      }
                      finally
                      {
                          this.Invoke(onCompleted);
                      }
                  });
                thread.IsBackground = true;
                thread.Start();

                DisplayLogs();
                WriteLine(DateTime.Now.ToLongTimeString() + "Process completed. Exit PGE.CWOSL console.");
            }
            catch(Exception ex){
                WriteLine(DateTime.Now.ToLongTimeString() + "Error in submitting CWOSL. " + ex.Message);
                StatusLabel.Text = "Error in submitting.";
                SubmitBtn.Enabled = true;
                EnableDisableMandatoryFields(true);
                FullRunChkBox.Enabled = true;
                if (FullRunChkBox.Checked == true)
                {
                    EnableDisableNonMandatoryFields(false);
                }
                else
                {
                    EnableDisableNonMandatoryFields(true);
                }
                CheckCWOSLSession();
            }
        }

        private bool isInvalidCircuitID()
        {
            Regex re = new Regex("[A-Z,a-z,0-9]");

            if (CircuitIdText.Text.Trim().Length != 9 || (!re.IsMatch(CircuitIdText.Text.Trim())))
            {

                return true;

            }
            return false;
        }

        private void CloseBtn_Click(object sender, EventArgs e)
        {
            WriteLine(DateTime.Now.ToLongTimeString() + "Close button clicked. Exit window.");
            this.Close();
        }

        private void PostBtn_Click(object sender, EventArgs e)
        {
            try
            {
                WriteLine(DateTime.Now.ToLongTimeString() + "Post button clicked.");
                LogsText.Text = "";
                StatusLabel.Text = "Processing...";
                PostBtn.Enabled = false;
                string [] cwoslParameters = new string[] { "POST" };
                WriteLine(DateTime.Now.ToLongTimeString() + "Parameters to be passed to PGE.CWOSL console = 'POST'");
                WriteLine(DateTime.Now.ToLongTimeString() + "Call PGE.CWOSL console.");

                Action onCompleted = () =>
                {
                    System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
                    timer.Interval = (LogRefreshIntervalSec * 1000);
                    timer.Tick += (s, ev) =>
                    {
                        timer.Stop();
                        StatusLabel.Text = "Completed. Please check LOGS.";
                        logTimer.Stop();
                        CheckCWOSLSession();
                    };
                    timer.Start();
                };

                var thread = new Thread(
                  () =>
                  {
                      try
                      {
                          CallCWOSLMainMethod(cwoslParameters);
                      }
                      finally
                      {
                          this.Invoke(onCompleted);
                      }
                  });
                thread.IsBackground = true;
                thread.Start();

                DisplayLogs();

                WriteLine(DateTime.Now.ToLongTimeString() + "Process completed. Exit PGE.CWOSL console.");
            }
            catch (Exception ex)
            {
                WriteLine(DateTime.Now.ToLongTimeString() + "Error in posting from CWOSL Assignment Tool. " + ex.Message);
                StatusLabel.Text = "Error in posting.";
                CheckCWOSLSession();
            }
        }

        private void CallCWOSLMainMethod(string[] cwoslParameters)
        {
            PGE.BatchApplication.CWOSL.Program.Main(cwoslParameters);
        }

        private void DisplayLogs()
        {
            try
            {
                logTimer.Interval = (LogRefreshIntervalSec * 1000);
                logTimer.Tick += new EventHandler(logTimer_Tick);
                logTimer.Start();
            }
            catch (Exception ex)
            {
                WriteLine(DateTime.Now.ToLongTimeString() + "Error in fetching logs. " + ex.Message);
                MessageBox.Show("Error in displaying logs");
                if (logTimer != null)
                    logTimer.Stop();
            }
        }

        private void logTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                string path = PGE.BatchApplication.CWOSL.Program.GetLogFilePath();
                if (File.Exists(path))
                {
                    string readText; // = File.ReadAllText(path);
                    var fileStream = new FileStream(@path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
                    {
                        readText = streamReader.ReadToEnd();
                    }
                    LogsText.Text = readText;
                    LogsText.SelectionStart = LogsText.TextLength;
                    LogsText.ScrollToCaret();
                }
                else
                {
                    WriteLine(DateTime.Now.ToLongTimeString() + "CWOSL console log file not found.");
                    MessageBox.Show("Eror in displaying logs");
                    logTimer.Stop();
                }
            }
            catch (Exception ex)
            {
                WriteLine(DateTime.Now.ToLongTimeString() + "Error in fetching logs. " + ex.Message);
                MessageBox.Show("Eror in displaying logs");
                logTimer.Stop();
            }
        }

        private void NoOfFeaturesText_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (System.Text.RegularExpressions.Regex.IsMatch(NoOfFeaturesText.Text, "[^0-9]"))
                {
                    MessageBox.Show("Please enter only numbers.");
                    NoOfFeaturesText.Text = noOfFeaturesPrevValue;
                }
                noOfFeaturesPrevValue = NoOfFeaturesText.Text;
            }
            catch (Exception ex)
            {
                WriteLine(DateTime.Now.ToLongTimeString() + "Error in validating No. of features. " + ex.Message);
            }
        }

        private void ProgramRunTimeText_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (System.Text.RegularExpressions.Regex.IsMatch(ProgramRunTimeText.Text, "[^0-9]"))
                {
                    MessageBox.Show("Please enter only numbers.");
                    ProgramRunTimeText.Text = programRunTimePrevValue;
                }
                programRunTimePrevValue = ProgramRunTimeText.Text;
            }
            catch (Exception ex)
            {
                WriteLine(DateTime.Now.ToLongTimeString() + "Error in validating Program Run Time " + ex.Message);
            }
        }

        private void FullRunChkBox_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (FullRunChkBox.Checked == true)
                {
                    EnableDisableNonMandatoryFields(false);
                }
                else
                {
                    EnableDisableNonMandatoryFields(true);
                }
            }
            catch (Exception ex)
            {
                WriteLine(DateTime.Now.ToLongTimeString() + "Error: " + ex.Message);
            }
        }

        private void EnableDisableMandatoryFields(bool enable)
        {
            NoOfFeaturesText.Enabled = enable;
            ProgramRunTimeText.Enabled = enable;
        }

        private void EnableDisableNonMandatoryFields(bool enable)
        {
                HFTDCombo.Enabled = enable;
                DivisionCombo.Enabled = enable;
                CountyCombo.Enabled = enable;
                LocalOfficeIdCombo.Enabled = enable;
                SecLoadPointCombo.Enabled = enable;
                TransformerSubTypeCombo.Enabled = enable;
                SketchPseudoCombo.Enabled = enable;
                CircuitIdText.Enabled = enable;
        }
    }
}
