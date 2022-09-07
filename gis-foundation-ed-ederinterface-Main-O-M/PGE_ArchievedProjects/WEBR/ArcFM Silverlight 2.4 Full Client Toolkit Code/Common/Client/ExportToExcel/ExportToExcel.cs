using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
#if WPF
using System.Web;
#endif

#if SILVERLIGHT
using Miner.Server.Client.Export;
using Miner.Server.Client.Tasks;
#elif WPF
using Miner.Mobile.Client.Export;
using Miner.Mobile.Client.Tasks;
#endif

#if SILVERLIGHT
namespace Miner.Server.Client
#elif WPF
namespace Miner.Mobile.Client
#endif
{
    /// <summary>
    /// This class allows to export Collection of IResultSet to Excel.
    /// </summary>
    public class ExportToExcel
    {
        #region private declarations

        private int _worksheetCount = 0;
        private IEnumerable<IResultSet> _data;
        private IEnumerable<RelationshipInformation> _relationshipInfo;
        private List<Worksheet> _alreadyWrittenWorksheets = new List<Worksheet>();
        private XmlWriter _writer;
        private List<IResultSet> _relatedDataSets = new List<IResultSet>();
        private int _totalRelationshipResults = 0;
        private ICustomizeExportToExcel _customizeExport;
        #endregion private declarations

        #region public methods

        /// <summary>
        /// Writes the data to an Excel compliant xml file.
        /// </summary>
        /// <param name="data">The data to export.</param>
        /// <param name="file">The file to write to.</param>
        /// <param name="relationshipInfo">The relationship information</param>
        /// <param name="customizeExport">an object to customize the export format </param>
        public void ExportToStream(IEnumerable<IResultSet> data, Stream file, IEnumerable<RelationshipInformation> relationshipInfo = null, ICustomizeExportToExcel customizeExport = null)
        {
            if (data == null) return;
            if (file == null) return;

            if (_worksheetCount <= 0)
            {
                OnExportToExcelStarted(EventArgs.Empty);
                _data = data;
                _relationshipInfo = relationshipInfo;
                _customizeExport = customizeExport;
            }
            Dictionary<string, Worksheet> worksheets = new Dictionary<string, Worksheet>();

            IResultSet resultSet = data.ElementAt<IResultSet>(_worksheetCount);

            worksheets.Add(resultSet.Name, new Worksheet { Name = resultSet.Name, ResultSet = resultSet });
            ++_worksheetCount;

            RelationshipRetrievalResetEvent retrieval = new RelationshipRetrievalResetEvent();

            // We can't get relationships on a background thread.
            GetRelationships(worksheets, relationshipInfo, retrieval);

            // Run the export in the background
            BackgroundWorker exportWorker = new BackgroundWorker();
            exportWorker.DoWork += ExportWorker_DoWork;
            exportWorker.RunWorkerCompleted += ExportWorker_RunWorkerCompleted;
            exportWorker.RunWorkerAsync(new ExportObjects { Data = worksheets, File = file, RelationshipRetrieval = retrieval });
        }

        #endregion public methods

        #region events
        /// <summary>
        /// Fires when the export to excel starts .
        /// </summary>
        public event EventHandler ExportToExcelStarted;
        public virtual void OnExportToExcelStarted(EventArgs args)
        {
            EventHandler handler = ExportToExcelStarted;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// Fires when the export to excel ends .
        /// </summary>
        public event EventHandler ExportToExcelFinished;
        public virtual void OnExportToExcelFinished(EventArgs args)
        {
            EventHandler handler = ExportToExcelFinished;
            if (handler != null)
            {
                handler(this, args);
            }
        }
        #endregion events

        #region Event Handlers

        private void ExportWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e == null) return;

            Stream file = e.Result as Stream;
            if (file == null) return;

            file.Flush();
            if (_worksheetCount < _data.Count())
            {
                ExportToStream(_data, file, _relationshipInfo);
            }
            else
            {
                if (_customizeExport != null)
                {
                    file.Position = 0;
                    file = _customizeExport.Convert(file);
                }

                file.Close();

                this.OnExportToExcelFinished(EventArgs.Empty);
            }
        }

        private void ExportWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (e == null) return;

            ExportObjects eo = e.Argument as ExportObjects;
            if (eo == null) return;

            // Wait till we have everything.
            if (eo.RelationshipRetrieval != null)
            {
                eo.RelationshipRetrieval.AutoReset.WaitOne();
            }

            var xmlSettings = new XmlWriterSettings
            {
#if DEBUG
                Indent = true,
                IndentChars = ("\t"),
#endif
                Encoding = Encoding.Unicode
            };

            if (_worksheetCount == 1)
            {
                _writer = XmlWriter.Create(eo.File, xmlSettings);

                _writer.WriteStartDocument();

                _writer.WriteProcessingInstruction("xml-stylesheet", "type='text/xsl'");
                _writer.WriteStartElement("Workbook", "urn:schemas-microsoft-com:office:spreadsheet");
                _writer.WriteAttributeString("xmlns", "o", null, "urn:schemas-microsoft-com:office:office");
                _writer.WriteAttributeString("xmlns", "x", null, "urn:schemas-microsoft-com:office:excel");
                _writer.WriteAttributeString("xmlns", "ss", null, "urn:schemas-microsoft-com:office:spreadsheet");
                _writer.WriteDefaultStyles();
                _writer.Flush();
            }

            foreach (Worksheet worksheet in eo.Data.Values)
            {
                _alreadyWrittenWorksheets.Add(worksheet);
            }

            if (_worksheetCount >= _data.Count())
            {
                //write all worksheets
                List<Worksheet> worksheets = _alreadyWrittenWorksheets.ToList();

                foreach (Worksheet currentWorksheet in _alreadyWrittenWorksheets)
                {
                    Worksheet worksheet = GetNextWorksheetToWrite(worksheets, currentWorksheet);
                    if (worksheet == null) continue;

                    //write worksheet header
                    _writer.WriteWorksheetHeader(worksheet.Name); //Creates a worksheet element
                    _writer.WriteStartElement("Table");

                    int index = -1;
                    while (worksheet != null)
                    {
                        //write worksheet
                        ++index;
                        _writer.WriteResultSet(worksheet.ResultSet, (index == 0));
                        _writer.WriteRelationships(worksheet.Relationships, (index == 0));

                        worksheet = GetNextWorksheetToWrite(worksheets, currentWorksheet);
                    }

                    //write worksheet end
                    _writer.WriteEndElement();   //Table
                    _writer.WriteEndElement();   //Worksheet
                }

                _writer.WriteEndElement();   //Workbook
                _writer.Flush();
                _writer.Close();
            }

            e.Result = eo.File;
        }

        private Worksheet GetNextWorksheetToWrite(List<Worksheet> worksheets, Worksheet worksheetToWrite)
        {
            if ((worksheets == null) || (worksheets.Count <= 0)) return null;
            if (worksheetToWrite == null) return null;

            Worksheet worksheet = null;
            foreach (Worksheet currentWorksheet in worksheets)
            {
                if (currentWorksheet.Name == worksheetToWrite.Name)
                {
                    worksheet = currentWorksheet;
                    worksheets.Remove(currentWorksheet);
                    break;
                }
            }

            return worksheet;
        }

        private void RelationshipServiceCompleted(Dictionary<string, Worksheet> worksheets, IResultSet relatedResultSet, RelationshipEventArgs args, RelationshipRetrievalResetEvent retrieval)
        {
            if ((args == null) || (args.Results == null) || (args.Results.Count == 0))
            {
                retrieval.RemovePendingResult(relatedResultSet);
            }
            else
            {
                foreach (var resultsByObjectId in args.Results)
                {
                    _totalRelationshipResults = _totalRelationshipResults + resultsByObjectId.Value.Count();
                }

                foreach (var resultsByObjectId in args.Results)
                {
                    IEnumerable<IResultSet> results = resultsByObjectId.Value;
                    foreach (IResultSet set in results)
                    {
                        set.Service = relatedResultSet.Service;

                        RelationshipResult relationship = new RelationshipResult { ObjectID = resultsByObjectId.Key, ResultSet = set };

                        FieldInformationService fieldInfoService = new FieldInformationService();
                        retrieval.AddPendingRelationship(relatedResultSet);

                        fieldInfoService.FieldOrderCompleted += (s, e) =>
                            RelationshipFieldOrderComplete(worksheets, e.ResultSet, relatedResultSet, relationship, retrieval);

                        fieldInfoService.GetFieldInformationAsync(set);
                    }
                }
                // Just in case we didn't send any off
                if (retrieval.GetPendingRelationships(relatedResultSet) <= 0)
                {
                    retrieval.RemovePendingResult(relatedResultSet);
                }
            }
        }

        private void RelationshipFieldOrderComplete(
            Dictionary<string, Worksheet> worksheets,
            IResultSet resultSet,
            IResultSet relatedResultSet,
            RelationshipResult relationship,
            RelationshipRetrievalResetEvent retrieval)
        {
            Worksheet worksheet = worksheets.FirstOrDefault(w => w.Key == resultSet.Name).Value;
            if (worksheet == null)
            {
                int index = 0;
                var name = resultSet.Name;
                while (worksheets.ContainsKey(resultSet.Name))
                {
                    resultSet.Name = index + " " + name;
                    index++;
                }

                worksheet = new Worksheet { Name = resultSet.Name };

                worksheets.Add(resultSet.Name, worksheet);
            }

            resultSet.Name = resultSet.Name + " Related to " + relatedResultSet.Name;

            RelationshipResults relationships = worksheet.Relationships.FirstOrDefault(r => r.Name == resultSet.Name);
            if (relationships == null)
            {
                relationships = new RelationshipResults { Name = resultSet.Name };
                worksheet.Relationships.Add(relationships);
            }
            relationships.Results.Add(relationship);

            _relatedDataSets.Add(relatedResultSet);

            --_totalRelationshipResults;
            if (_totalRelationshipResults <= 0)
            {
                foreach (IResultSet set in _relatedDataSets)
                {
                    retrieval.RemovePendingRelationship(set);
                }
                _relatedDataSets.Clear();
            }
        }

        #endregion Event Handlers

        #region Private Methods

        private void GetRelationships(Dictionary<string, Worksheet> worksheets, IEnumerable<RelationshipInformation> relationshipInformation, RelationshipRetrievalResetEvent retrieval)
        {
            if (relationshipInformation == null) return;

            foreach (Worksheet worksheet in worksheets.Values)
            {
                IResultSet resultSet = worksheet.ResultSet;
                if (resultSet == null) continue;

                int id = resultSet.ID;
#if SILVERLIGHT
                string decodedService = System.Windows.Browser.HttpUtility.UrlDecode(resultSet.Service);
#elif WPF
                string decodedService = HttpUtility.UrlDecode(resultSet.Service);
#endif
                IEnumerable<RelationshipInformation> relationshipInfos = from relatedInfo in relationshipInformation
                                                                         where (relatedInfo.LayerId == id) &&
                                                                                decodedService != null &&
                                                                                decodedService.ToLower().Contains(relatedInfo.Service.ToLower())
                                                                         select relatedInfo;
                if (relationshipInfos.Count() <= 0) continue;

                IRelationshipService relationshipService = new RelationshipService();
                if (resultSet.RelationshipService != null)
                {
                    relationshipService = Activator.CreateInstance(resultSet.RelationshipService.GetType()) as IRelationshipService;
                }
                relationshipService.ExecuteCompleted += (s, args) => RelationshipServiceCompleted(worksheets, resultSet, args, retrieval);

                retrieval.AddPendingResult(resultSet);

                var oids = resultSet.Features.Select(feature => Utility.GetObjectIDValue(feature.Attributes)).Where(oid => oid >= 0).ToArray();
                relationshipService.GetRelationshipsAsync(relationshipInfos, oids);
            }
        }

        #endregion Private Methods

        #region ExportObjects Class

        class ExportObjects
        {
            public Dictionary<string, Worksheet> Data { get; set; }
            public Stream File { get; set; }
            public RelationshipRetrievalResetEvent RelationshipRetrieval { get; set; }
        }

        #endregion ExportObjects Class
    }
}