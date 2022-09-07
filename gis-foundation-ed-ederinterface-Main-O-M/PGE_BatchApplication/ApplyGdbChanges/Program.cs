using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using ESRI.ArcGIS.Geodatabase;
using PGE.Common.CommandFlags;
using PGE.Common.Delivery.Diagnostics;
using PGE_DBPasswordManagement;

namespace PGE.BatchApplication.ApplyGdbChanges
{
    class Program
    {
        private static LicenseHandler _licenseHandler = new LicenseHandler();
        private static Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "GDBChanges.log4net.config");

        private static string _sdeConnection;

        private IWorkspace _workspace = null;

        private IWorkspace Workspace
        {
            get
            {
                if (_workspace == null)
                {
                    _licenseHandler.Bind();
                    _licenseHandler.CheckOut();
                    _workspace = ArcSDEWorkspaceFromPath(_sdeConnection);
                }
                return _workspace;
            }

        }
        public static ESRI.ArcGIS.Geodatabase.IWorkspace ArcSDEWorkspaceFromPath(string sFile)
        {
            IWorkspace workspace = null;
            string SDEPath = ReadEncryption.GetSDEPath(sFile);
            IWorkspaceFactory workspaceFactory = new ESRI.ArcGIS.DataSourcesGDB.SdeWorkspaceFactory();
            workspace = workspaceFactory.OpenFromFile(SDEPath, 0);

            return workspace;

        }

        static void Main(string[] args)
        {
            DateTime exeStart = DateTime.Now;

            try
            {
                //Report start time.
                _logger.Info("Process started: " + exeStart.ToLongDateString() + " " + exeStart.ToLongTimeString());

                ProcessArgs(args);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
            }
            finally
            {
                _logger.Info("Process finished: " + DateTime.Now.ToLongDateString());
                //                Console.ReadKey();
            }

        }

        public static void ProcessArgs(string[] args)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            string mode = "";
            string configFile = "";
            string filter = "";
            string input = "";
            // Parse Flags
            var flags = new FlagParser()
                {
                    { "o", "operation", true, false, "The operation to perform", f => mode = f },
                    { "d", "database", true, false, "The password-saved sde file", f => _sdeConnection = f },
                    { "i", "input", true, false, "File input", f => input = f },
                    { "f", "filter", true, false, "Filter", f => filter = f },
                };
            flags.Parse(args);

            if (String.IsNullOrEmpty(mode))
            {
                ShowHelp();
            }
            else
            {
                //LoadConfig(configFile);
                Program program = new Program();
                switch (mode.ToUpper())
                {
                    case "APPLYALIASNAMES":
                        program.ApplyAliasNames(input, filter);
                        break;
                    default:
                        ShowHelp();
                        break;
                }

            }
        }

        protected static void ShowHelp()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            Console.WriteLine("PGE.BatchApplication.ApplyGdbChanges Usage:");
            Console.WriteLine("");
            Console.WriteLine("PGEChangeDetection -o applyAliasNames -i <inputFile> -f <filter> -d <sdeConnectionFile>");
            Console.WriteLine("");

            //-o applyAliasNames -i c:\temp\fields.csv -f ZPGEVW_\w+_CSOURCE -d C:\Users\p1pcadmin\AppData\Roaming\ESRI\Desktop10.1\ArcCatalog\edgis@edgisp2d.sde
        }

        public void ApplyAliasNames(string inputFile, string filter)
        {
            IEnumDataset enumDataset = this.Workspace.get_Datasets(esriDatasetType.esriDTFeatureClass);
            
            string[] lines = File.ReadAllLines(inputFile);
            Dictionary<string,string> fieldAliases = new Dictionary<string, string>();
            foreach (string line in lines)
            {
                fieldAliases.Add(line.Split(',')[0], line.Split(',')[1]);
            }

            IDataset dataset;
            while ((dataset = enumDataset.Next()) != null)
            {
                Regex regex = new Regex(filter);

                if (dataset is IFeatureClass && regex.IsMatch(dataset.Name))
                {
                    IFeatureClass featureClass = dataset as IFeatureClass;
                    _logger.Info(dataset.Name);
                    for (int i = 0; i < featureClass.Fields.FieldCount; i++)
                    {
                        IField field = featureClass.Fields.get_Field(i);
                        if (field.Type == esriFieldType.esriFieldTypeOID && field.Name == "ID")
                        {
                            IFieldEdit fieldEdit = field as IFieldEdit;
                            _logger.Debug("Changing Alias from [ " + field.AliasName + " ] to [ Object ID ]");
                            IClassSchemaEdit classSchemaEdit = featureClass as IClassSchemaEdit;
                            classSchemaEdit.AlterFieldAliasName(field.Name, "Object ID");
                        }
                        else if (fieldAliases.ContainsKey(field.Name) &&
                            field.AliasName != fieldAliases[field.Name])
                        {
                            IFieldEdit fieldEdit = field as IFieldEdit;
                            _logger.Debug("Changing Alias from [ " + field.AliasName + " ] to [ " + fieldAliases[field.Name] + " ]");
                            IClassSchemaEdit classSchemaEdit = featureClass as IClassSchemaEdit;
                            classSchemaEdit.AlterFieldAliasName(field.Name, fieldAliases[field.Name]);
                            //                            fieldEdit.AliasName_2 = fieldAliases[field.Name];
                        }
                    }

                }
            }
        }
    }
}
