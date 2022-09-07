using System;
using NLog;
using Newtonsoft.Json;

namespace Helper
{
    public static class LogHelper
    {
        private static readonly Logger logger = LogManager.GetLogger("Logger");

        public static bool Log(string errorCommentOrType, string detail)
        {
            bool isLogged = true;
            string errorMessage = String.Empty;
            try
            {
                detail = "Detail: " + detail;

                errorMessage = errorCommentOrType + " - " + detail;
                switch (errorCommentOrType)
                {
                    case "Fatal":
                        logger.Fatal(errorMessage);
                        break;
                    case "Error":
                        logger.Error(errorMessage);
                        break;
                    case "Warn":
                        logger.Warn(errorMessage);
                        break;
                    case "Info":
                        logger.Info(errorMessage);
                        break;
                    case "Debug":
                        logger.Debug(errorMessage);
                        break;
                    case "Trace":
                        logger.Trace(errorMessage);
                        break;
                    default:
                        logger.Error(errorMessage);
                        break;
                }
            }
            catch (Exception ex)
            {
                isLogged = false;
                //throw ex;
                //logger.Error("Logging Error: " + ex);
            }
            finally
            {

            }
            return isLogged;
        }

        public static bool Log(string errorCommentOrType, object detail)
        {
            bool isLogged = false;
            string errorMessage = String.Empty;
            if (detail != null)
            {
                try
                {
                    errorMessage = errorCommentOrType + " - " + "Detail: " + JsonConvert.SerializeObject(detail);
                    switch (errorCommentOrType)
                    {
                        case "Fatal":
                            logger.Fatal(errorMessage);
                            break;
                        case "Error":
                            logger.Error(errorMessage);
                            break;
                        case "Warn":
                            logger.Warn(errorMessage);
                            break;
                        case "Info":
                            logger.Info(errorMessage);
                            break;
                        case "Debug":
                            logger.Debug(errorMessage);
                            break;
                        case "Trace":
                            logger.Trace(errorMessage);
                            break;
                        default:
                            logger.Error(errorMessage);
                            break;
                    }
                    isLogged = true;
                }
                catch (Exception ex)
                {
                    isLogged = false;
                    //throw ex;
                    //logger.Error("Logging Error: " + ex);
                }
                finally
                {

                }
            }
            return isLogged;
        }

        public static bool Log(string errorCommentOrType, string message, object data)
        {
            bool isLogged = false;
            string errorMessage = String.Empty;

            try
            {
                errorMessage = errorCommentOrType + " - " + message + " Data: " + JsonConvert.SerializeObject(data);
                switch (errorCommentOrType)
                {
                    case "Fatal":
                        logger.Fatal(errorMessage);
                        break;
                    case "Error":
                        logger.Error(errorMessage);
                        break;
                    case "Warn":
                        logger.Warn(errorMessage);
                        break;
                    case "Info":
                        logger.Info(errorMessage);
                        break;
                    case "Debug":
                        logger.Debug(errorMessage);
                        break;
                    case "Trace":
                        logger.Trace(errorMessage);
                        break;
                    default:
                        logger.Error(errorMessage);
                        break;
                }
                isLogged = true;
            }
            catch (Exception ex)
            {
                isLogged = false;
                //throw ex;
                //logger.Error("Logging Error: " + ex);
            }
            finally
            {

            }

            return isLogged;
        }
    }
}