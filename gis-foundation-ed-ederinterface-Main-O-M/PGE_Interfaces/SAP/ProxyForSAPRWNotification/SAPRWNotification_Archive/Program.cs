using System;
using System.Configuration;
using System.IO;

namespace PGE.Interfaces.SAPRWNotification_Archive
{
    class Program
    {
        private static string _dataCenter = "";
        public static readonly DataHelper ObjDataHelper = new DataHelper();
        static void Main(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].ToLower() == "-d")
                {
                    _dataCenter = args[i + 1];
                }
            }

            ArchieveDirectories(_dataCenter);
        }

        private static void ArchieveDirectories(string dataCenter)
        {
            Console.WriteLine("Start Deleting Directories");
            string serverPathDc1 = ConfigurationManager.AppSettings["DC1DestFileNamePath"];
            string serverPathDc2 = ConfigurationManager.AppSettings["DC2DestFileNamePath"].ToString();
            if (dataCenter.ToUpper() == "DC1")
            {
                Console.WriteLine("Deleting Directories for" + serverPathDc1);
                DeleteDirectories(serverPathDc1);
                Console.WriteLine("Deleted Directories for" + serverPathDc1);
                Console.WriteLine("Deleted Directories");
                Console.WriteLine("Removing records from database");
                RemoveRecords(dataCenter);
                Console.WriteLine("Records has removed from database");

            }
            else if (dataCenter.ToUpper() == "DC2")
            {
                Console.WriteLine("Deleting Directories for" + serverPathDc2);
                DeleteDirectories(serverPathDc2);
                Console.WriteLine("Deleted Directories for" + serverPathDc2);
                Console.WriteLine("Deleted Directories");
                Console.WriteLine("Removing records from database");
                RemoveRecords(dataCenter);
                Console.WriteLine("Records has removed from database");
            }
                        
        }

        internal static void DeleteDirectories(string path)
        {
            
            try
            {
                
                DirectoryInfo dirInfo = new DirectoryInfo(path);
                DirectoryInfo[] subdirinfo = dirInfo.GetDirectories();
                foreach (DirectoryInfo dir in subdirinfo)
                {

                    if ((DateTime.Now - dir.LastWriteTime).TotalDays >= Convert.ToInt32(ConfigurationManager.AppSettings["NoofDays"]))
                    {
                        
                        Directory.Delete(dir.FullName, true);
                        Console.WriteLine(dir.Name + " has deleted");
                    }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
            
        }

        internal static void RemoveRecords(string DataCenter)
        {
            DateTime date = System.DateTime.Now.AddDays(-Convert.ToInt32(ConfigurationManager.AppSettings["NoofDays"]));
            string sqlselectNotificatioIDs = ConfigurationManager.AppSettings["Select_NotificationDetail"].ToString();
            sqlselectNotificatioIDs = sqlselectNotificatioIDs + date.ToShortDateString() + "', 'MM/DD/YYYY') and DATACENTER = '"+DataCenter+"'";

            string sqlDeleteAttachments = ConfigurationManager.AppSettings["Delete_Attachment"].ToString();
            sqlDeleteAttachments = sqlDeleteAttachments + sqlselectNotificatioIDs + ")";

            string sqlDeleteLocations = ConfigurationManager.AppSettings["Delete_NotificationLocation"].ToString();
            sqlDeleteLocations = sqlDeleteLocations + sqlselectNotificatioIDs + ")";

            string sqlDeleteHeaderDetail = ConfigurationManager.AppSettings["Delete_NotificationDetail"].ToString();
            sqlDeleteHeaderDetail = sqlDeleteHeaderDetail + date.ToShortDateString() + "', 'MM/DD/YYYY')";

            Console.WriteLine("Removing records for Attachments");
            ObjDataHelper.DeleteData(sqlDeleteAttachments);
            Console.WriteLine("Records for Attachments has deleted");

            Console.WriteLine("Removing records for Locations");
            ObjDataHelper.DeleteData(sqlDeleteLocations);
            Console.WriteLine("Records for Locations has deleted");

            Console.WriteLine("Removing records for Header");
            ObjDataHelper.DeleteData(sqlDeleteHeaderDetail);
            Console.WriteLine("Records for Header has deleted");

        }
    }
}
