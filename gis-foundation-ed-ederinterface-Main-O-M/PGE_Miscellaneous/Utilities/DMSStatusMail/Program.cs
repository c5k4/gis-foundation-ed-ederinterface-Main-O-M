using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data;
using DMSStatusMail.Email;
using DMSStatusMail.DBHelper;
using System.IO;
using System.Reflection;

namespace DMSStatusMail.Email
{
     class Program
    {
       // public static Log4NetLogger _log = new Log4NetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "pge.log4net.config");
        static void Main(string[] args)
        {
            createLogFile();
            WriteLine("START");            
           GetData( );

        }
        public static void GetData( )
        {

            string Strmailbody = "";
            string html = "";
            string strexporttype = "";
            try
            {


               
                
                string strtype = "";
                 strtype = cls_DBHelper.ExecuteScalerQuery(" select  max(EXTRACTTYPE) from dmsstarttime").ToString();
                if (strtype.ToUpper() == "CHANGE")
                {
                    strexporttype = "Incremental";
                }
                else
                {
                    strexporttype = "Full";
                }

                string partialName = "log";
                string Exportpath = ConfigurationManager.AppSettings["ExportPath"];
                DirectoryInfo hdDirectoryInWhichToSearch = new DirectoryInfo(@Exportpath);
                FileInfo[] filesInDir = hdDirectoryInWhichToSearch.GetFiles("*" + partialName + ".txt");
                string fullName = "";
                foreach (FileInfo foundFile in filesInDir)
                {
                    fullName = foundFile.FullName;
                    
                }

                //
                string log = "";
                string[] lines = System.IO.File.ReadAllLines(fullName);

                // Display the file contents by using a foreach loop.
                
                foreach (string line in lines)
                {
                    // Use a tab to indent each line of the file.
                    if (log == "") {
                      log =   log  + line;
                    }
                    else
                    {
                        log = log + "</BR>" + line;
                    }
                   // Console.WriteLine("\t" + line);
                }


                //

                System.Data.DataTable DTStatusReport = cls_DBHelper.GetDataTableByQuery("select CIRCUITSTATUS,SUBSTATIONORCIRCUIT,COUNT(*) TOTAL from dmsstaging.PGE_DMS_TO_PROCESS group by circuitstatus,SUBSTATIONORCIRCUIT");//control.GetCircuitStatusReport();
                System.Data.DataTable DTServerStatusReport = cls_DBHelper.GetDataTableByQuery("select SERVERNAME, SUBSTATIONORCIRCUIT, COUNT(*) TOTAL from dmsstaging.PGE_DMS_TO_PROCESS group by SERVERNAME, SUBSTATIONORCIRCUIT ");//control.GetServerStatusReport();
                System.Data.DataTable DTCircuitStatus = cls_DBHelper.GetDataTableByQuery("select PROCESSORID,CIRCUITIDS,SUBSTATIONORCIRCUIT,SERVERNAME,ERROR from dmsstaging.PGE_DMS_TO_PROCESS where circuitstatus = 3");
                
                String ErrorCircuitCount = cls_DBHelper.ExecuteScalerQuery("select count(*) total from dmsstaging.pge_dms_to_process where circuitstatus = 3").ToString();//control.CheckCircuitError();
               string  sErrorCircuitCount= ErrorCircuitCount == "0" ? "error" : "errors";
                html += "<p font-size: 12pt >Hello Everyone,</p>";
                html += "<p font-size: 16pt > ED50 " + strexporttype + " extract completed today with " + ErrorCircuitCount + " circuit " + sErrorCircuitCount + " and successfully generated files.The next sftp scheduled batch will transfer the files to the DMS server.</p>";
                html += "<p font-size: 16pt > Please find the status below:</p>";
                html += "<p font-size: 16pt ><B>SUMMARY:</B></p>";
                html += "<table style='border: 1px solid #ccc;font-size: 9pt;font-family:arial'>";

                //Adding HeaderRow.
                string border_style = "style='width:120px;border: 1px solid #ccc'";
                html += "<tr>";
                html += "<th bgcolor='#add8e6' " + border_style + ">CIRCUITSTATUS	</th>";
                html += "<th bgcolor='#add8e6' " + border_style + ">SUBSTATIONORCIRCUIT</th>";
                html += "<th  bgcolor='#add8e6' " + border_style + ">Total</th>";

                html += "</tr>";

                foreach (System.Data.DataRow row in DTStatusReport.Rows)
                {
                    try
                    {
                        html += "<tr>";
                        html += "<td " + border_style + ">" + row["CIRCUITSTATUS"].ToString() + "</td>";
                        html += "<td " + border_style + ">" + row["SUBSTATIONORCIRCUIT"].ToString() + "</td>";
                        html += "<td " + border_style + ">" + row["Total"].ToString() + "</td>";
                        html += "</tr>";
                    }

                    catch (Exception ex)
                    {
                       // _log.Error("Error", ex);
                    }
                }

                html += "</table>";
                html += "<p font-size: 16pt ><B>Server Status:</B></p>";
                //Server Status
                html += "<table style='border: 1px solid #ccc;font-size: 9pt;font-family:arial'>";
                html += "<tr>";
                html += "<th bgcolor='#add8e6' " + border_style + ">SERVERNAME</th>";
                html += "<th bgcolor='#add8e6' " + border_style + ">SUBSTATIONORCIRCUIT</th>";
                html += "<th  bgcolor='#add8e6' " + border_style + ">Total</th>";
                html += "</tr>";
                foreach (System.Data.DataRow row in DTServerStatusReport.Rows)
                {
                    try
                    {
                        html += "<tr>";
                        html += "<td " + border_style + ">" + row["SERVERNAME"].ToString() + "</td>";
                        html += "<td " + border_style + ">" + row["SUBSTATIONORCIRCUIT"].ToString() + "</td>";
                        html += "<td " + border_style + ">" + row["TOTAL"].ToString() + "</td>";
                        html += "</tr>";
                    }

                    catch (Exception ex)
                    {
                       // _log.Error("Error", ex);
                    }
                }

                html += "</table>";

                if ((DTCircuitStatus !=null) && (DTCircuitStatus.Rows.Count >0 ))
                    {
               

                    html += "<p font-size: 16pt ><B>Circuit Error:</B></p>";
                    //Server Status
                    html += "<table style='border: 1px solid #ccc;font-size: 9pt;font-family:arial'>";
                    html += "<tr>";
                    html += "<th bgcolor='#add8e6' " + border_style + ">PROCESSORID</th>";
                    html += "<th bgcolor='#add8e6' " + border_style + ">CIRCUITIDS</th>";
                    html += "<th  bgcolor='#add8e6' " + border_style + ">SUBSTATIONORCIRCUIT</th>";
                    html += "<th  bgcolor='#add8e6' " + border_style + ">SERVERNAME</th>";
                    html += "<th  bgcolor='#add8e6' " + border_style + ">ERROR</th>";
                    html += "</tr>";
                    foreach (System.Data.DataRow row in DTCircuitStatus.Rows)
                    {
                        try
                        {
                            html += "<tr>";
                            html += "<td " + border_style + ">" + row["PROCESSORID"].ToString() + "</td>";
                            html += "<td " + border_style + ">" + row["CIRCUITIDS"].ToString() + "</td>";
                            html += "<td " + border_style + ">" + row["SUBSTATIONORCIRCUIT"].ToString() + "</td>";
                            html += "<td " + border_style + ">" + row["SERVERNAME"].ToString() + "</td>";
                            html += "<td " + border_style + ">" + row["ERROR"].ToString() + "</td>";
                            html += "</tr>";
                        }

                        catch (Exception ex)
                        {
                            // _log.Error("Error", ex);
                        }
                    }

                    html += "</table>";
                }

                html += "<p font-size: 16pt ><B>Output Log:</B></p>";
                html += "<p font-size: 16pt >"+ log +"</p>"; ;
                html += "<p font-size: 16pt >Thanks & Regards,</BR><b>Electric Support – GIS Operations(Electric)</b></p>";               
                SendMail(html, strexporttype);
            }
            catch (Exception ex)
            {
                WriteLine("Error" + ex.Message);
            }


        }



        public static void SendMail(string strbodyText, string mail_subject)
        {
            try
            {                
                string lanID = string.Empty;
                string lanIDCC = string.Empty;
                string fromDisplayName = ConfigurationManager.AppSettings["MAIL_FROM_DISPLAY_NAME"];
                string fromAddress = ConfigurationManager.AppSettings["MAIL_FROM_ADDRESS"];
                string subject = "ED50 " + mail_subject + " Extract Completed - " + DateTime.Now.ToString("MM-dd-yyyy");
                string bodyText = strbodyText;
                // string lanIDs = ConfigurationManager.AppSettings["LANIDS"];                          
               
                lanID=ConfigurationManager.AppSettings["LANID"].ToString();
                lanIDCC = ConfigurationManager.AppSettings["LANIDCC"].ToString();

                EmailService.Send(mail =>
                                {
                                    mail.From = fromAddress;
                                    mail.FromDisplayName = fromDisplayName;
                                    mail.To = lanID;
                                    mail.CC = lanIDCC;
                                    mail.Subject = subject;
                                    mail.BodyText = bodyText;

                                });
                
                
                WriteLine("Email Sent");

            }



            catch (Exception ex)
            {
                WriteLine("Error in Sending Email: " + ex.Message);
                //_log.Error("Error in Sending Mail: ", ex);
            }
        }

        private static string sPath = (String.IsNullOrEmpty(ConfigurationManager.AppSettings["LOG_PATH"]) ? System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) : ConfigurationManager.AppSettings["LOGPATH"]) + "\\Logfile_" + DateTime.Now.Ticks.ToString() + ".log";
        private static StreamWriter pSWriter = default(StreamWriter);
        /// <summary>
        /// create log file 
        /// </summary>
        public static void createLogFile()
        {
            (pSWriter = File.CreateText(sPath)).Close();
        }
        /// <summary>
        /// Write on console and log file
        /// </summary>
        /// <param name="sMsg"></param>
        public static void WriteLine(string sMsg)
        {
            sMsg = !String.IsNullOrEmpty(sMsg) ? DateTime.Now.ToLongTimeString() + " -- " + sMsg : sMsg;
            pSWriter = File.AppendText(sPath);
            pSWriter.WriteLine(sMsg);
            Console.WriteLine(sMsg);
            pSWriter.Close();
        }

    }
}
        
    
