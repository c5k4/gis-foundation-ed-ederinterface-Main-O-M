using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.XPath;
using PGE.BatchApplication.PageDotConfigured.ConfigValidation.Json;
using PGE.BatchApplication.PageDotConfigured.ConfigValidation.Validators;
using PGE.BatchApplication.PageDotConfigured.ConfigValidation.Xml;
using PGE.Common.Delivery.Diagnostics;

namespace PGE.BatchApplication.PageDotConfigured.Repair
{
    class ConfigRepairer
    {
        private static Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "PgeDotConfigured.log4net.config");

        private readonly string _xmlFile;
        private StringBuilder _newFile;

        private const string LayerIdPrefix = "LayerId=\"";
        private const string RelationshipIdPrefix = "RelationshipIds=\"";
        private const string ProposedLayerIdPrefix = " ProposedLayerID=\"";
        private const string NonProposedLayerIdPrefix = "NonProposedLayerID=\"";
        private const string LinkedSubtypeFCPrefix = "LinkedSubtypeFC=\"";

        private ClassIdLayerMapValidator _classIdLayerMapValidator;

        public bool GenerateClassIdLayerMappings { get; set; }

        private IEnumerable<XmlCheckItem> _items;

        public ConfigRepairer(string pageConfigFilePath, ClassIdLayerMapValidator classIdLayerMapValidator)
        {
            _xmlFile = File.ReadAllText(pageConfigFilePath);
            _newFile = new StringBuilder(_xmlFile);
            _classIdLayerMapValidator = classIdLayerMapValidator;
        }

        public void RepairToFile(IEnumerable<XmlCheckItem> items, string filePath)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            ISet<string> urls = new HashSet<string>();
            JsonDataDictionaryCache jsonDataCache = JsonDataDictionaryCache.Instance;
            _items = items;
            
            foreach (XmlCheckItem i in items)
            {
                urls.Add(i.Url);
            }

            foreach (string url in urls)
            {
                IDictionary<int, MapServiceLayer> jsonService;
                try
                {
                    jsonService = jsonDataCache.GetUriData(url);
                }
                catch
                {
                    _logger.Error("Failed to get url [ " + url + " ]");
                    continue;
                }

                RepairService(jsonService, url);
            }
            
            ReplaceLayerIdsCsv(jsonDataCache);
            ReplaceRelationshipIdsCsv(jsonDataCache);
            if (GenerateClassIdLayerMappings)
            {
                ReplaceClassIdLayerMaps();
            }

            File.WriteAllText(@filePath, _newFile.ToString());
        }

        private void RepairService(IDictionary<int, MapServiceLayer> jsonService, string url)
        {
            _logger.Debug("RepairService on [ " + url + " ]");

            foreach (KeyValuePair<int, MapServiceLayer> layerKvp in jsonService)
            {
                foreach (XmlCheckItem item in _items.Where(i => i.Error != ErrorCodes.NoError && i.Url.Equals(url)))
                {
                    // This check is precluding OH Transformers from being fixed
                    if (!layerKvp.Value.Name.Equals(item.XmlLayerVal) || item.XmlLayerKey == layerKvp.Key) continue;

                    string searchStr = item.ReferenceLine;
                    while (_xmlFile.IndexOf(searchStr) == -1)
                    {
                        searchStr = searchStr.Substring(0, searchStr.Length - 1);
                    }

                    if (item.ReferenceLine.Contains(RelationshipIdPrefix + item.XmlLayerKey) &&
                        layerKvp.Value.IsLayerUrl)
                    {
                        _newFile = _newFile.Replace(searchStr,
                            searchStr.Replace(RelationshipIdPrefix + item.XmlLayerKey,
                                RelationshipIdPrefix + layerKvp.Key));
                    }
                    else if (item.ReferenceLine.Contains(LayerIdPrefix + item.XmlLayerKey))
                    {
                        _newFile = _newFile.Replace(searchStr,
                            searchStr.Replace(LayerIdPrefix + item.XmlLayerKey,
                                LayerIdPrefix + layerKvp.Key));
                    }
                    else if (item.ReferenceLine.Contains(ProposedLayerIdPrefix + item.XmlLayerKey) &&
                             layerKvp.Value.Name.Contains("Proposed"))
                    {
                        _newFile = _newFile.Replace(searchStr,
                            searchStr.Replace(ProposedLayerIdPrefix + item.XmlLayerKey,
                                ProposedLayerIdPrefix + layerKvp.Key));
                    }
                    else if (item.ReferenceLine.Contains(NonProposedLayerIdPrefix + item.XmlLayerKey) &&
                             !layerKvp.Value.Name.Contains("Proposed"))
                    {
                        _newFile = _newFile.Replace(searchStr,
                            searchStr.Replace(NonProposedLayerIdPrefix + item.XmlLayerKey,
                                NonProposedLayerIdPrefix + layerKvp.Key));
                    }
                    else if (item.ReferenceLine.Contains(LinkedSubtypeFCPrefix) &&
                             item.XmlLayerKey != layerKvp.Value.Id)
                    {
                        _newFile = _newFile.Replace(searchStr,
                            searchStr.Replace(item.Url + "/" + item.XmlLayerKey,
                                item.Url + "/" + layerKvp.Key));
                    }
                }
            }
        }

        private void ReplaceRelationshipIdsCsv(JsonDataDictionaryCache jsonDataCache)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            try
            {
                XDocument xmlDocument = XDocument.Load(new StringReader(_newFile.ToString()));

                var relationshipElements = xmlDocument.XPathSelectElements("//RelatedData/Path[contains(@RelationshipIds,',')]");

                foreach (var relationshipElement in relationshipElements)
                {
                    string url = relationshipElement.Attribute("Url").Value;
                    string relationshipIdsCsv = relationshipElement.Attribute("RelationshipIds").Value;
                    string relationshipAliasesCsv = relationshipElement.Attribute("RelationshipAlias").Value;
                    int layerId = Convert.ToInt32(relationshipElement.Attribute("LayerId").Value);

                    string ids = "";
                    string relatedUrl = url + "/" + layerId;
                    IDictionary<int, MapServiceLayer> jsonService = jsonDataCache.GetUriData(relatedUrl);

                    var relationshipAliases = relationshipAliasesCsv.Split(',');

                    try
                    {
                        // Write out the first relationship
                        string relationshipAlias = relationshipAliases[0];
                        int id = jsonService.Where(l => l.Value.Name.ToUpper() == relationshipAlias.ToUpper()).First().Value.Id;
                        ids += id + ",";
                        // Now get the second relationship by following the first
                        IDictionary<int, MapServiceLayer> jsonServiceRelated = jsonDataCache.GetUriData(url + "/" + jsonService[id].RelatedTableId);
                        id = jsonServiceRelated.Where(l => l.Value.Name.ToUpper() == relationshipAliases[1].ToUpper()).First().Value.Id;
                        ids += id;
                    }
                    catch (Exception)
                    {
                        
                    }

                    relationshipElement.SetAttributeValue("RelationshipIds", ids);
                }

                _newFile.Clear();
                _newFile.Append(xmlDocument.ToString());

            }
            catch (Exception exception)
            {
                _logger.Fatal("Cannot write RelationshipIds [ " + exception + " ]");
                throw;
            }
            
        }

        private void ReplaceLayerIdsCsv(JsonDataDictionaryCache jsonDataCache)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            try
            {
                XDocument xmlDocument = XDocument.Load(new StringReader(_newFile.ToString()));

                var searchableElements = xmlDocument.XPathSelectElements(
                    "//searchable | //SchematicsFilter | //ExcludeFromProposedAnnoList | " +
                    "//VoltageFilterByCircuitID | //VoltageFilterAnnoByCircuitID | " +
                    "//LayersOff | //LayersOn | //DefinitionQuery | //ButterflyDiagram | " +
                    "//ExcludeFromVoltageFilterLayerIDsCSV");

                foreach (var searchableElement in searchableElements)
                {
                    string url = searchableElement.Attribute("url").Value;
                    string layerNames = searchableElement.Attribute("LayerName").Value;
                    if (layerNames.Contains("ignore"))  continue;
                
                    string ids = "";

                    IDictionary<int, MapServiceLayer> jsonService = jsonDataCache.GetUriData(url);

                    foreach (var layerName in layerNames.Split(','))
                    {
                        try
                        {
                            int id = jsonService.Where(l => l.Value.Name.ToUpper() == layerName.ToUpper()).First().Value.Id;
                            ids += id + ",";
                        }
                        catch (Exception)
                        {
                            _logger.Error("Layer may not exist [ " + layerName + " ]");
                            throw;
                        }
                    }
                    ids = ids.Remove(ids.LastIndexOf(","));
                    searchableElement.SetAttributeValue("ids", ids);
                }

                _newFile.Clear();
                _newFile.Append(xmlDocument.ToString());

            }
            catch (Exception exception)
            {
                _logger.Fatal("Cannot write LayerIds ");
                throw;
            }
        }

        private void ReplaceClassIdLayerMaps()
        {
            XDocument xmlDocument = XDocument.Load(new StringReader(_newFile.ToString()));

            var urlElements = xmlDocument.XPathSelectElements("//ClassIDToLayerIDMap/Url");

            foreach (var urlElement in urlElements)
            {
                string url = urlElement.Attribute("value").Value;

                _classIdLayerMapValidator.Generate(url);
            }

            var classIdToLayerMapElement = xmlDocument.XPathSelectElement("//ClassIDToLayerIDMap");
            classIdToLayerMapElement.ReplaceWith(_classIdLayerMapValidator.ClassIdToLayerMapElement);

            _newFile.Clear();
            _newFile.Append(xmlDocument.ToString());
        }
    }
}
