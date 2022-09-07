using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using PGE.BatchApplication.PageDotConfigured.ConfigValidation.Validators;
using PGE.BatchApplication.PageDotConfigured.ConfigValidation.Xml;
using PGE.BatchApplication.PageDotConfigured.Repair;
using PGE.Common.Delivery.Diagnostics;

namespace PGE.BatchApplication.PageDotConfigured
{
    internal class Program
    {
        private static readonly Log4NetLogger Logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "PGE.BatchApplication.PageDotConfigured.log4net.config");

        private static string _pageConfigFilePath = "", _templatesConfigFilePath = "", _newPageConfigFilePath = "";

        private static bool _generateClassIdLayerMappings;

        private static void Main(string[] args)
        {
            DateTime exeStart = DateTime.Now;

            try
            {
                _pageConfigFilePath = ConfigurationManager.AppSettings["PageConfigLocation"];
                _templatesConfigFilePath = ConfigurationManager.AppSettings["TemplatesConfigLocation"];
                _newPageConfigFilePath = ConfigurationManager.AppSettings["NewPageConfigLocation"];
                _generateClassIdLayerMappings = Convert.ToBoolean(ConfigurationManager.AppSettings["GenerateClassIdLayerMappings"]);
                Run();
            }
            catch (Exception ex)
            {
                Logger.Fatal("Error [ " + ex + " ]");
            }
            finally
            {
                Logger.Debug(MethodBase.GetCurrentMethod().Name);

                //Report end time.
                DateTime exeEnd = DateTime.Now;
                Logger.Info("");
                Logger.Info("Completed");
                Logger.Info("Process started: " + exeStart.ToLongDateString() + " " + exeStart.ToLongTimeString());
                Logger.Info("Process ended: " + exeEnd.ToLongDateString() + " " + exeEnd.ToLongTimeString());
                Logger.Info("Process length: " + (exeEnd - exeStart));


                Console.WriteLine("Complete. Press any key to exit.");
                Console.ReadKey(true);
            }
        }

        private static void Run()
        {
            List<XmlCheckItem> problems = new List<XmlCheckItem>();
            try
            {
                var templatesConfigValidator = new TemplatesConfigValidator(_templatesConfigFilePath);
                problems = templatesConfigValidator.Run().ToList();
            }
            catch (Exception e)
            {
                Logger.Warn("Could not find templates.config. Skipping.");
            }

            ClassIdLayerMapValidator classIdLayerMapValidator = new ClassIdLayerMapValidator();
            var pageConfigValidator = new PageConfigValidator(_pageConfigFilePath);
            problems.AddRange(pageConfigValidator.Run());

            var errorCounts = new int[Enum.GetNames(typeof (ErrorCodes)).Length];

            //Find out how many of each type of problem there are and list out each problem's details
            foreach (
                XmlCheckItem problem in
                    problems.GroupBy(item => item.Error)
                        .SelectMany(problemGrouping => problemGrouping.OrderBy(p => p.XmlLayerKey))) 
            {
                if (problem.Error != ErrorCodes.NoError) Logger.Warn("\n" + problem + "\n");
                errorCounts[(int)problem.Error]++;
            }

            //List out the number of each type of error, counted in the previous step
            Logger.Info("Count of errors by type:");
            foreach (ErrorCodes e in Enum.GetValues(typeof (ErrorCodes)))
                Logger.Info(e + ": " + errorCounts[(int) e]);

            ConfigRepairer repairer = new ConfigRepairer(_pageConfigFilePath, classIdLayerMapValidator) { GenerateClassIdLayerMappings = _generateClassIdLayerMappings};
            repairer.RepairToFile(problems, _newPageConfigFilePath);
        }
    }
}