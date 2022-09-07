using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace PGE.BatchApplication.LogToCSV
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 1 && Directory.Exists(args[0]))
            {
                ConsolidateEachLogFile(args[0]);
                ConsolidateAllLogFiles(args[0]);

                string[] sDirectories = Directory.GetDirectories(args[0], "*", SearchOption.AllDirectories);
                foreach (string sDirectory in sDirectories)
                {
                    ConsolidateEachLogFile(sDirectory);
                    ConsolidateAllLogFiles(sDirectory);
                }
            }
            else
            {
                ConsolidateEachLogFile(ConfigurationManager.AppSettings["LOGFOLDER_PATH"]);
                ConsolidateAllLogFiles(ConfigurationManager.AppSettings["LOGFOLDER_PATH"]);
            }
        }
        private static int iUserNumber = 100;
        private static void ConsolidateEachLogFile(string sDirectory)
        {
            // PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.pAList_Log.Add(Convert.ToString(++PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.iCount_Log) + ":" + PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sNFR_Log + ":" + PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sCommand_Log + ":" + Convert.ToString(this.SW1.ElapsedMilliseconds));//For Summary Log- Shashwat Nigam
            
            //Read Config File
            if (!Directory.Exists(sDirectory)) return;
            string[] sFiles = Directory.GetFiles(sDirectory, "*.log");
            
            foreach (string sFile in sFiles)
            {
                var pDic_NFRTotalTime = new Dictionary<string, double>();
                bool isValid = false;
                StreamReader sr = new StreamReader(new System.IO.FileStream(sFile, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite));
               
                List<String> sLogAllLines = new List<string>();
                
                while (!sr.EndOfStream)
                    sLogAllLines.Add(sr.ReadLine());
                try { sr.Close(); }
                catch { }
                //MessageBox.Show(sLogAllLines.Count.ToString());
                //string[] sLogAllLines = File.ReadAllLines(this.Logger.LogPath);
                foreach (string sLogLine in sLogAllLines)
                {
                    if (sLogLine.StartsWith("LOG_FOR_USER"))
                    { isValid = true; break; }
                }
                if (isValid) continue;

                double dTimeMilliSecond = 0.0;
                string sNFR = string.Empty;
                pDic_NFRTotalTime.Clear();
                foreach (string sLogLine in sLogAllLines)
                {
                    //NFR to NFR is one block and not consider NFR without number
                    if (sLogLine.StartsWith("NFR"))// && !sLogLine.Split(':')[0].Trim().EndsWith("NFR"))
                        if (!pDic_NFRTotalTime.ContainsKey(sLogLine.Split(':')[0].Trim()))
                        { pDic_NFRTotalTime.Add(sNFR = sLogLine.Split(':')[0].Trim(), 0.0); continue; }
                    if (sLogLine.Split(':')[0].Trim().EndsWith("NFR")) { sNFR = string.Empty; continue; }
                    if (pDic_NFRTotalTime.Count > 0 && !string.IsNullOrEmpty(sNFR))
                    {
                        if (sLogLine.Split(':').Length == 2 &&
                            isAllUpper(sLogLine.Split(':')[0].Trim()) &&
                            !sLogLine.Split(':')[0].Trim().StartsWith("EXECUTESQL") &&
                            double.TryParse(sLogLine.Split(':')[1].Trim(), out dTimeMilliSecond))
                        {
                            pDic_NFRTotalTime[sNFR] = Convert.ToDouble(pDic_NFRTotalTime[sNFR]) + dTimeMilliSecond;
                            // MessageBox.Show(Convert.ToString(pDic_NFRTotalTime[sNFR]));
                        }
                        else if (sLogLine.Split(',')[0] == "All Layers")
                        {
                            //  MessageBox.Show("All Layers");
                            pDic_NFRTotalTime[sNFR] = Convert.ToDouble(pDic_NFRTotalTime[sNFR]) + Convert.ToDouble(sLogLine.Split(',')[sLogLine.Split(',').Length - 1]) * Convert.ToDouble(ConfigurationManager.AppSettings["MILLISECONDFACTOR"]); //converting to mill
                        }
                    }
                    //Assumption Command Name must be in Block Letter and EXECUTESQL not to be considered
                }
                //IEnumerator pEnumKeys = pDic_NFRTotalTime.Keys.GetEnumerator();
                List<string> pEnumKeys = pDic_NFRTotalTime.Keys.ToList<string>();
                pEnumKeys.Sort();
                //while (pEnumKeys.MoveNext())

                string sUserName = "PTUser" + Convert.ToString(++iUserNumber);

                StreamWriter sw = new StreamWriter(new FileStream(sFile, FileMode.Append, FileAccess.Write, FileShare.ReadWrite));
                sw.WriteLine();
                sw.WriteLine("LOG_FOR_USER: " + sUserName);
                foreach (string sKey in pEnumKeys)
                {
                    //if (pEnumKeys.Current.ToString() == "NFR") continue;
                    if (sKey == "NFR") continue;
                    //this.Logger.WriteLine(pEnumKeys.Current.ToString() + ": " + Convert.ToString(pDic_NFRTotalTime[Convert.ToString(pEnumKeys.Current)]), false, string.Empty);
                    sw.WriteLine(sKey + ": " + Convert.ToString(pDic_NFRTotalTime[sKey]), false, string.Empty);
                }
                sw.WriteLine("---------------------------");
                sw.Close();
            }
        }

        static bool isAllUpper(string input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                if (!Char.IsLetter(input[i])) continue;
                if (!Char.IsUpper(input[i]))
                    return false;
            }

            return true;
        }

        private static void ConsolidateAllLogFiles(string sDirectory)
        {
            //Read Config File
            if (!Directory.Exists(sDirectory)) return;
            string[] sFiles = Directory.GetFiles(sDirectory, "*.log");

            string[] sNFRIDs = ConfigurationManager.AppSettings["NFR_ID"].Split(',');
            //Sorting NFR ID's
          //  Array.Sort(sNFRIDs, new AlphanumComparatorFast());

            //Create NFR Rows in Ascending Order
            DataTable pDTable_Log = new DataTable("LOG_SUMMARY");
            pDTable_Log.Columns.Add("NFR_ID", typeof(string));

            foreach (string sNFRID in sNFRIDs)
            {
                DataRow pRow = pDTable_Log.NewRow();
                pRow["NFR_ID"] = sNFRID;
                pDTable_Log.Rows.Add(pRow);
            }

            //Reading all files
            for (int iCount_File = 0; iCount_File < sFiles.Length; ++iCount_File)
            {
                try
                {
                    Console.WriteLine("Reading File " + sFiles[iCount_File]);
                    string[] sAllLines = File.ReadAllLines(sFiles[iCount_File]);
                    string sUserName = string.Empty;
                    bool isUser = false;

                    ArrayList pAList = new ArrayList();
                    int iHyphen = 0;

                    foreach (string sLine in sAllLines)
                    {
                        try
                        {
                            if (sLine.StartsWith("LOG_FOR_USER"))
                            {
                                sUserName = sLine.Split(':')[sLine.Split(':').Length - 1].Trim();
                                pDTable_Log.Columns.Add(sUserName, typeof(double)).SetOrdinal(pDTable_Log.Columns.Count - 1);
                                isUser = true;
                                continue;
                            }

                            if (isUser && sLine.StartsWith("-"))
                            {
                                isUser = false; continue;
                            }

                            if (isUser && sLine.Length != 0&& sLine.StartsWith("NFR"))
                            {
                                if (sLine.Split(':')[0].Contains('-')) // FOr picking higher value
                                {
                                    ++iHyphen;
                                    pAList.Add(Convert.ToDouble(sLine.Split(':')[1].Trim()));
                                    if (iHyphen == 2)
                                    {
                                        pAList.Sort();
                                        DataRow[] pDRow_NFRID = pDTable_Log.Select("NFR_ID = '" + sLine.Split(':')[0].Split('-')[0] + "'");
                                        pDRow_NFRID[0][sUserName] = Convert.ToDouble(pAList[1]) / 1000; //change to seconds
                                        iHyphen = 0; pAList.Clear();
                                    }
                                }
                                else
                                {
                                    try
                                    {
                                    FINDROW:
                                        DataRow[] pDRow_NFRID = pDTable_Log.Select("NFR_ID = '" + sLine.Split(':')[0] + "'");
                                        if (pDRow_NFRID.Count() == 0)
                                        {
                                            DataRow pRow = pDTable_Log.NewRow();
                                            pRow["NFR_ID"] = sLine.Split(':')[0];
                                            pDTable_Log.Rows.Add(pRow);
                                            goto FINDROW;
                                        }
                                        pDRow_NFRID[0][sUserName] = Convert.ToDouble(sLine.Split(':')[1].Trim()) / 1000.00;  //Change to seconds
                                    }
                                    catch
                                    {
                                        DataRow pRow = pDTable_Log.NewRow();
                                        pRow["NFR_ID"] = sLine.Split(':')[0];
                                        pDTable_Log.Rows.Add(pRow);
                                        DataRow[] pDRow_NFRID = pDTable_Log.Select("NFR_ID = '" + sLine.Split(':')[0] + "'");
                                        pDRow_NFRID[0][sUserName] = Convert.ToDouble(sLine.Split(':')[1].Trim()) / 1000.00;  //Change to seconds
                                    }
                                }
                            }
                        }
                        catch (Exception ex1) { Console.WriteLine(ex1.Message); }
                    }
                }
                catch (Exception ex) { Console.WriteLine(ex.Message); }
            }
            try
            {
                StringBuilder sBuilder = new StringBuilder();
                string[] columnNames = pDTable_Log.Columns.Cast<DataColumn>().Select(column => column.ColumnName).ToArray();
                sBuilder.AppendLine(string.Join(",", columnNames));
                foreach (DataRow row in pDTable_Log.Rows)
                {
                    string[] fields = row.ItemArray.Select(field => field.ToString()).ToArray();
                    sBuilder.AppendLine(string.Join(",", fields));
                }
                Console.WriteLine("Writing CSV File " + sDirectory + "\\PerfQ_Logs_Summary.csv");
                if (File.Exists(sDirectory + "\\PerfQ_Logs_Summary.csv"))
                    File.Delete(sDirectory + "\\PerfQ_Logs_Summary.csv");
                File.WriteAllText(sDirectory + "\\PerfQ_Logs_Summary.csv", sBuilder.ToString());
            }
            catch (Exception ex2) { Console.WriteLine(ex2.Message); }
        }

        public class AlphanumComparatorFast : IComparer
        {
            public int Compare(object x, object y)
            {
                string s1 = x as string;
                if (s1 == null)
                {
                    return 0;
                }
                string s2 = y as string;
                if (s2 == null)
                {
                    return 0;
                }

                int len1 = s1.Length;
                int len2 = s2.Length;
                int marker1 = 0;
                int marker2 = 0;

                // Walk through two the strings with two markers.
                while (marker1 < len1 && marker2 < len2)
                {
                    char ch1 = s1[marker1];
                    char ch2 = s2[marker2];

                    // Some buffers we can build up characters in for each chunk.
                    char[] space1 = new char[len1];
                    int loc1 = 0;
                    char[] space2 = new char[len2];
                    int loc2 = 0;

                    // Walk through all following characters that are digits or
                    // characters in BOTH strings starting at the appropriate marker.
                    // Collect char arrays.
                    do
                    {
                        space1[loc1++] = ch1;
                        marker1++;

                        if (marker1 < len1)
                        {
                            ch1 = s1[marker1];
                        }
                        else
                        {
                            break;
                        }
                    } while (char.IsDigit(ch1) == char.IsDigit(space1[0]));

                    do
                    {
                        space2[loc2++] = ch2;
                        marker2++;

                        if (marker2 < len2)
                        {
                            ch2 = s2[marker2];
                        }
                        else
                        {
                            break;
                        }
                    } while (char.IsDigit(ch2) == char.IsDigit(space2[0]));

                    // If we have collected numbers, compare them numerically.
                    // Otherwise, if we have strings, compare them alphabetically.
                    string str1 = new string(space1);
                    string str2 = new string(space2);

                    int result;

                    if (char.IsDigit(space1[0]) && char.IsDigit(space2[0]))
                    {
                        int thisNumericChunk = int.Parse(str1);
                        int thatNumericChunk = int.Parse(str2);
                        result = thisNumericChunk.CompareTo(thatNumericChunk);
                    }
                    else
                    {
                        result = str1.CompareTo(str2);
                    }

                    if (result != 0)
                    {
                        return result;
                    }
                }
                return len1 - len2;
            }
        }
    }
}
