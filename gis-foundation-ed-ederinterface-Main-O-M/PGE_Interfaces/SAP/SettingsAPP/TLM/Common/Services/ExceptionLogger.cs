using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Xml.Serialization;
using TLM.EntityFramework;

namespace TLM.Common.Services
{
    public class ExceptionLogger:TLM.Common.Interfaces.IExceptionLogger
    {
        string Path = string.Empty;
        string CurrentDate = System.DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_ff");
        string FileName = string.Empty;
        string FullFileName = string.Empty;
        bool AppendTo = true;
        public void LogExceptionInFile(ExceptionLog ExcLog)
        {
            Path = ExcLog.Path;
            FileName = CurrentDate + ".xml";
            FullFileName = string.Format("{0}\\{1}", Path, FileName);
            if (!File.Exists(FullFileName))
            {
                using (var createStream = File.Create(FullFileName)) { }
                AppendTo = false;
            }
            XmlSerializer writer = new XmlSerializer(typeof(ExceptionLog));
            using (StreamWriter file = new StreamWriter(FullFileName, AppendTo))
            {
                writer.Serialize(file, ExcLog);
            }  
        }

        public void LogException(string GlobalId,Exception Ex,string LogPath,string ClassName,string MethodName)
        {
            ExceptionLog logData = new ExceptionLog();
            logData.ClassName=ClassName;
            logData.GlobalId = GlobalId;
            logData.MethodName=MethodName;
            logData.ErrorMessage = Ex.Message;
            logData.StackTrace = Ex.StackTrace;
            if(Ex.InnerException!=null)
            logData.InnerException = Ex.InnerException.StackTrace;
            logData.ErrorTime = System.DateTime.Now.ToString();
            logData.Path = LogPath;

            try
            {
                //LogExceptionInDB(logData);
                //LogExceptionInFile(logData);
            }
            catch (Exception ex)
            {
                //LogExceptionInFile(logData);
            }
            
        }

        //public void LogExceptionInDB(ExceptionLog ExcLog)
        //{
        //    try
        //    {
        //        using (var context = new TLMEntities())
        //        {
        //            decimal MaxId = 1;
        //            var Data = context.ExecuteStoreQuery<decimal>("select max(ID) from TLM_APP_ERRORS", null);
        //            if (Data != null)
        //            {
        //                MaxId = Data.First() + 1;
        //            }
        //            TLM_APP_ERRORS TLMErrors = new TLM_APP_ERRORS();
        //            TLMErrors.ID = MaxId;
        //            TLMErrors.GLOBAL_ID = ExcLog.GlobalId;
        //            TLMErrors.CLASS = ExcLog.ClassName;
        //            TLMErrors.METHOD = ExcLog.MethodName;
        //            TLMErrors.ERROR_MSG = ExcLog.ErrorMessage;
        //            TLMErrors.ERROR_STACK = ExcLog.StackTrace;
        //            TLMErrors.ERROR_INNER = ExcLog.InnerException;
        //            TLMErrors.CREATE_DTM = DateTime.Now;
        //            context.AddObject("TLM_APP_ERRORS", TLMErrors);
        //            context.SaveChanges();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message + ex.StackTrace);
        //    }
        //}
    }
}