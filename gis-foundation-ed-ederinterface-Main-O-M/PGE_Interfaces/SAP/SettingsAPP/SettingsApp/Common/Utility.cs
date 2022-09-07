using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using System.Diagnostics;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Web.Hosting;

namespace SettingsApp.Common
{
    public class Utility
    {
        private static readonly object _syncObject = new object();

        public static string CleanPathString(string val)
        {
            return val.Replace(':','!');
        }

        public static string UnCleanPathString(string val)
        {
            return val.Replace('!', ':');
        }

        public static void BuildMapping<T, T1>(T model, T1 entity, bool modelToEntity)
        {
            foreach (PropertyInfo pi in model.GetType().GetProperties())
            {
                //Debug.WriteLine(pi.Name);
                var x = pi.GetCustomAttributes(typeof(SettingsValidatorAttribute), true);
                if (x.Length > 0)
                {
                    SettingsValidatorAttribute si = x[0] as SettingsValidatorAttribute;
                    if (modelToEntity)
                        Debug.WriteLine(string.Format("e.{0} = this.{1};", si.PropertyName, pi.Name));
                    else
                        Debug.WriteLine(string.Format("this.{0} = e.{1};", pi.Name, si.PropertyName));
                }
            }

        }

        public static void BuildMapping<T, T1>(T entityHistory, T1 entity)
        {
            foreach (PropertyInfo pi in entity.GetType().GetProperties())
            {
                Debug.WriteLine(string.Format("entityHistory.{0} = e.{1};", pi.Name, pi.Name));
            }

        }

        public static void BuildDataType<T, T1>(T model, T1 entity, bool findMissing)
        {
            foreach (PropertyInfo pi in model.GetType().GetProperties())
            {
                //Debug.WriteLine(pi.Name);
                var x = pi.GetCustomAttributes(typeof(SettingsValidatorAttribute), true);
                if (x.Length > 0)
                {
                    SettingsValidatorAttribute si = x[0] as SettingsValidatorAttribute;
                    PropertyInfo epi = entity.GetType().GetProperty(si.PropertyName);
                    if (epi == null)
                    {
                        if (findMissing)
                            Debug.WriteLine(string.Format("Property:{0} not found in entity {1}", pi.Name, si.PropertyName.ToUpper()));
                    }
                    else
                    {
                        if (!findMissing)
                        {
                            var displayAtt = pi.GetCustomAttributes(typeof(DisplayAttribute), true);

                            Type t = epi.PropertyType;


                            Debug.WriteLine(string.Format("[SettingsValidatorAttribute(\"{0}\", \"{1}\")]", si.DeviceName, si.PropertyName));
                            if (displayAtt.Length > 0)
                            {
                                DisplayAttribute di = displayAtt[0] as DisplayAttribute;
                                Debug.WriteLine(string.Format("[Display(Name =\"{0}:\")]", di.Name));
                            }

                            string type = t.ToString();
                            if (type.StartsWith("System.Nullable"))
                            {
                                type = type + "?";
                                type = type.Replace("System.Nullable`1", "");
                                type = type.Replace("[", "");
                                type = type.Replace("]", "");
                            }
                            Debug.WriteLine(string.Format("public {0} {1} {{ get; set; }}", type, pi.Name));
                            Debug.WriteLine("");
                        }
                    }

                }
            }

        }

        public static void GenerateView<T>(T model)
        {
            foreach (PropertyInfo pi in model.GetType().GetProperties())
            {
                Debug.WriteLine("<tr>");
                Debug.WriteLine("<td>");
                Debug.WriteLine("@Html.LabelForRequired(model => model." + pi.Name + ")");
                Debug.WriteLine("</td>");
                Debug.WriteLine("<td>");
                Debug.WriteLine("@Html.TextBoxFor(model => model." + pi.Name + ", ViewBag.IsDisabled ? (object)new { disabled = \"disabled\" } : new { })");
                Debug.WriteLine("@Html.ValidationMessageFor(model => model." + pi.Name + ")");
                Debug.WriteLine("</td>");
                Debug.WriteLine("</tr>");
            }
        }

        public static DateTime? ParseDateTime(string date)
        {
            DateTime? dateResult = null;
            DateTime dateOut;
            if (DateTime.TryParse(date, out dateOut))
                dateResult = dateOut;
            return dateResult;
        }

        public static string ParseDateTimeNoYear(DateTime? date)
        {
            if(date.HasValue)
                return date.Value.ToString("dd MMM");
            else
                return string.Empty;
        }
        public static void WriteLog(string message)
        {
            //return;
            if (Constants.LoggingEnabled)
            {
                lock (_syncObject)
                {
                    using (HostingEnvironment.Impersonate())
                    {
                        string path = HttpContext.Current.Server.MapPath("~/data/logs/");
                        string fileName = string.Concat(path, DateTime.Now.ToString("MM-dd-yyyy"), ".log");

                        StreamWriter sw = null;
                        try
                        {
                            sw = File.AppendText(fileName);
                            sw.WriteLine("--  " + DateTime.Now + "  ----");
                            sw.WriteLine(message);
                        }
                        catch (Exception ex)
                        {
                            //throw new Exception("Cannot write to log file", ex);
                        }
                        finally
                        {
                            if (sw != null)
                                sw.Close();
                        }
                    }
                }
            }
        }
    }
}