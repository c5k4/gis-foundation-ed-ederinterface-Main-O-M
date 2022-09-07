using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace PGE.Interface.Integration.DMS.Common
{
    /// <summary>
    /// This class reads all of the subtypes for all of the features in the database.
    /// It is used to quickly convert the subtype code to the subtype description without having to
    /// look it up in the database every time. It also loads all of the field domain assignments for
    /// determining what domains are assigned to a field
    /// </summary>
    public class Subtypes
    {
        
        private Dictionary<int, Dictionary<int, string>> _subtypeMap;
        private Dictionary<string, int> _fcidMap;
        private Dictionary<int, Dictionary<string, Dictionary<int, string>>> _domains;

        /// <summary>
        /// Provides access to domain field assignment. Dictionary[FCID, Dictionary[Field Name, Dictionary[Subtype Code, Domain Name]]]
        /// </summary>
        public Dictionary<int, Dictionary<string, Dictionary<int, string>>> Domains
        {
            get { return _domains; }
        }

        /// <summary>
        /// Get the name of the domain for a field on a specific feature
        /// </summary>
        /// <param name="FCID">The feature class ID</param>
        /// <param name="field">The name of the field</param>
        /// <param name="subtype">The subtype code of the feature</param>
        /// <returns>The name of the domain</returns>
        public string GetDomain(int FCID, string field, int subtype)
        {
            if (_domains.ContainsKey(FCID))
            {
                if (_domains[FCID].ContainsKey(field))
                {
                    if (_domains[FCID][field].ContainsKey(subtype))
                    {
                        return _domains[FCID][field][subtype];
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// A map of feature class name to its ID
        /// </summary>
        public Dictionary<string, int> FCIDMap
        {
            get { return _fcidMap; }
            set { _fcidMap = value; }
        }
        /// <summary>
        /// Get a visual printout of feature classes and their IDs. Only used for debugging
        /// </summary>
        /// <returns>A string with all of the feature class-ID pairs</returns>
        public string GetFCIDMap()
        {
            string output = "";
            if (_fcidMap != null)
            {
                foreach (string key in _fcidMap.Keys)
                {
                    output += key + " - " + _fcidMap[key].ToString() + ",";
                }
            }
            return output;
        }
        /// <summary>
        /// Dictionary for looking up Subtype descriptions. Dictionary[FCID, Dictionary[Subtype Code, Subtype Value]]
        /// </summary>
        public Dictionary<int, Dictionary<int, string>> SubtypeMap
        {
            get { return _subtypeMap; }
            set { _subtypeMap = value; }
        }
        /// <summary>
        /// Initialize the Subtypes
        /// </summary>
        /// <param name="workspace">The workspace to read all the subtypes and domains from</param>
        public Subtypes(IWorkspace workspace, bool readFromCache, bool writeToCache,
            string subtypeMappingFileName, string fcidMappingFileName, string domainsMappingFileName)
        {
            _subtypeMap = new Dictionary<int, Dictionary<int, string>>();
            _fcidMap = new Dictionary<string, int>();
            _domains = new Dictionary<int, Dictionary<string, Dictionary<int, string>>>();

            if (readFromCache && File.Exists(subtypeMappingFileName) && File.Exists(fcidMappingFileName) && File.Exists(domainsMappingFileName))
            {
                BinaryFormatter formatter = new BinaryFormatter();

                FileStream file = File.OpenRead(subtypeMappingFileName);
                _subtypeMap = formatter.Deserialize(file) as Dictionary<int, Dictionary<int, string>>;
                file.Close();

                file = File.OpenRead(fcidMappingFileName);
                _fcidMap = formatter.Deserialize(file) as Dictionary<string, int>;
                file.Close();

                file = File.OpenRead(domainsMappingFileName);
                _domains = formatter.Deserialize(file) as Dictionary<int, Dictionary<string, Dictionary<int, string>>>;
                file.Close();
            }
            else
            {
                IEnumDataset dsenum = workspace.get_Datasets(esriDatasetType.esriDTAny);
                if (dsenum != null)
                {
                    dsenum.Reset();
                    IDataset ds = dsenum.Next();
                    while (ds != null)
                    {
                        processDataset(ds);
                        ds = dsenum.Next();
                    }
                }
            }

            if (writeToCache)
            {
                if (File.Exists(subtypeMappingFileName)) { File.Delete(subtypeMappingFileName); }
                if (File.Exists(fcidMappingFileName)) { File.Delete(fcidMappingFileName); }
                if (File.Exists(domainsMappingFileName)) { File.Delete(domainsMappingFileName); }

                BinaryFormatter formatter = new BinaryFormatter();

                FileStream file = File.Create(subtypeMappingFileName);
                formatter.Serialize(file, _subtypeMap);
                file.Flush();
                file.Close();

                file = File.Create(fcidMappingFileName);
                formatter.Serialize(file, _fcidMap);
                file.Flush();
                file.Close();

                file = File.Create(domainsMappingFileName);
                formatter.Serialize(file, _domains);
                file.Flush();
                file.Close();
            }

        }

        private void processDataset(IDataset ds)
        {
            //Console.WriteLine(ds.Name);
            if (ds.Type == esriDatasetType.esriDTFeatureClass || ds.Type == esriDatasetType.esriDTTable)
            {
                int fcid = ((IObjectClass)ds).ObjectClassID;
                if (!_fcidMap.ContainsKey(ds.Name))
                {
                    _fcidMap.Add(ds.Name, fcid);
                }
                LoadDomains((IObjectClass)ds);
                if (ds is ISubtypes)
                {
                    Dictionary<int, string> map = new Dictionary<int, string>();
                    if (!_subtypeMap.ContainsKey(fcid))
                    {
                        _subtypeMap.Add(fcid, map);
                    }
                    else
                    {
                        map = _subtypeMap[fcid];
                    }
                    ISubtypes subtypes = (ISubtypes)ds;
                    if (subtypes.HasSubtype)
                    {
                        IEnumSubtype enumsub = subtypes.Subtypes;
                        enumsub.Reset();
                        int code = 0;
                        string value = enumsub.Next(out code);
                        while (value != null)
                        {
                            if (!map.ContainsKey(code))
                            {
                                map.Add(code, value);
                            }
                            value = enumsub.Next(out code);
                        }

                    }
                }
            }
            IEnumDataset dsenum = null;
            try
            {
                dsenum = ds.Subsets;
            }
            catch (NotImplementedException) { }
            if (dsenum != null)
            {
                dsenum.Reset();
                IDataset subds = dsenum.Next();
                while (subds != null)
                {
                    processDataset(subds);
                    subds = dsenum.Next();
                }
            }
        }
        /// <summary>
        /// Write all the domains loaded out to a text file. For debugging only.
        /// </summary>
        public void PrintDomains()
        {
            StringBuilder sb = new StringBuilder();
            foreach (int table in _domains.Keys)
            {
                foreach (string field in _domains[table].Keys)
                {
                    foreach (int subtype in _domains[table][field].Keys)
                    {
                        sb.AppendLine(table + "," + field + "," + subtype + "," + _domains[table][field][subtype]);
                    }
                }
            }
            System.IO.File.WriteAllText("domainlist.csv", sb.ToString());
        }
        private void AddDomain(int table, string fieldc, int subtype, string domain)
        {
            string field = fieldc.ToUpper();
            Dictionary<string, Dictionary<int, string>> fields = null;
            if (_domains.ContainsKey(table))
            {
                fields = _domains[table];
            }
            else
            {
                fields = new Dictionary<string, Dictionary<int, string>>();
                _domains.Add(table, fields);
            }
            Dictionary<int, string> subtypes = null;
            if (fields.ContainsKey(field))
            {
                subtypes = fields[field];
            }
            else
            {
                subtypes = new Dictionary<int, string>();
                fields.Add(field, subtypes);
            }
            if (!subtypes.ContainsKey(subtype))
            {
                subtypes.Add(subtype, domain);
            }
        }
        private void LoadDomains(IObjectClass oc)
        {
            if (oc is ISubtypes && ((ISubtypes)oc).HasSubtype)
                LoadSubtypeDomains(oc);
            else
            {
                for (int i = 0; i < oc.Fields.FieldCount; i++)
                {
                    var fld = oc.Fields.get_Field(i);
                    if (fld.Domain == null)
                        continue;
                    AddDomain(oc.ObjectClassID, fld.Name, -1, fld.Domain.Name);
                }
            }
        }
        private void LoadSubtypeDomains(IObjectClass oc)
        {
            ISubtypes subTypes = oc as ISubtypes;
            var enumSubtypes = subTypes.Subtypes;
            enumSubtypes.Reset();
            int code;
            string stName;
            while ((stName = enumSubtypes.Next(out code)) != null)
            {
                for (int i = 0; i < oc.Fields.FieldCount; i++)
                {
                    string fldName = oc.Fields.get_Field(i).Name;
                    var domain = subTypes.get_Domain(code, fldName);
                    if (domain != null)
                    {
                        AddDomain(oc.ObjectClassID, fldName, code, domain.Name);
                    }
                }
            }
        }
    }
}
