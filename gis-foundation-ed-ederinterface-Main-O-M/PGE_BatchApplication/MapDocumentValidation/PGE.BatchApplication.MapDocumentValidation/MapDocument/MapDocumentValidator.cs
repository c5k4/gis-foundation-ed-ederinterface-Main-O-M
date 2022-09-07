using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using MapDocumentValidation.Common;
using Miner.Interop;
using Telvent.Delivery.Diagnostics;

namespace MapDocumentValidation.MapDocument
{
    public class MapDocumentValidator
    {
        private static readonly Log4NetLogger Logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "MapDocValidation.log4net.config");

        private static readonly string[] ExcludedStoredDisplays = ConfigurationSettings.AppSettings[
            "ExcludedMapDocuments"].Replace("amp;", "").Split(',');

        private readonly IWorkspace _workspace;

        public MapDocumentValidator(IWorkspace wksp)
        {
            _workspace = wksp;
        }

        /// <summary>
        ///     Wrapper method that uses the 1-on-1 stored display comparison to compare all stored displays to all other stored
        ///     displays in the database, a pair at a time.
        /// </summary>
        /// <returns>
        ///     A list of error messages. Each message includes both stored displays, the symbol, the layer number
        ///     if it is a multilayer symbol, and the type of error
        /// </returns>
        public IList<ErrorMessage> ValidateAllStoredDisplaysForDatabase()
        {
            List<ErrorMessage> errors = new List<ErrorMessage>();
            MapDocuments sdValsDict = ExportStoredDisplays();

            List<string> dictKeysList = sdValsDict.Keys.ToList();
            dictKeysList.Sort();

            //Iterates thorugh every pair of stored displays in the system, running a comparison on each of them
            for (int i = 0; i < dictKeysList.Count; i++)
            {
                for (int j = i + 1; j < dictKeysList.Count; j++)
                {
                    errors.AddRange(CompareMapDocumentsHelper(dictKeysList[i], dictKeysList[j],
                        sdValsDict[dictKeysList[i]], sdValsDict[dictKeysList[j]]));
                    errors.AddRange(CompareMapDocumentsHelper(dictKeysList[j], dictKeysList[i],
                        sdValsDict[dictKeysList[j]], sdValsDict[dictKeysList[i]]));
                }
            }

            return errors;
        }

        public MapDocuments ExportStoredDisplays(string exportSd = "")
        {
            IMMStoredDisplayManager sdManager = new MMStoredDisplayManagerClass { Workspace = _workspace };
            IMMEnumStoredDisplayName sdNameEnum = sdManager.GetStoredDisplayNames(mmStoredDisplayType.mmSDTSystem);
            IMMStoredDisplayName mmStoredDisplayName;
            
            MapDocuments sdValsDict = new MapDocuments();

            //Iterates through the stored displays in the database and retrieves the IMap object for that stored display name.
            //Ignores any stored displays in the ExcludedStoredDisplays appsetting
            while ((mmStoredDisplayName = sdNameEnum.Next()) != null)
            {
                bool excluded = false;
                foreach (string sd in ExcludedStoredDisplays)
                {
                    if (sd.Equals(mmStoredDisplayName.Name)) excluded = true;
                }

                if (excluded) continue;

                if (!exportSd.Equals(""))
                {
                    if (exportSd.Equals(mmStoredDisplayName.Name))
                    {
                        AddDataToDictForSingleStoredDisplay(mmStoredDisplayName.Name, ref sdValsDict);
                        return sdValsDict;
                    }
                }
                else AddDataToDictForSingleStoredDisplay(mmStoredDisplayName.Name, ref sdValsDict);
            }

            return sdValsDict;
        }

        public MapDocuments ExportMxd(string exportMxd)
        {
            MapDocumentDataExtractor extractor = new MapDocumentDataExtractor();
            Layers mxdValsDict = extractor.GetDataForOneMapDocument(GetMapFromMxd(exportMxd));
            MapDocuments doc = new MapDocuments();
            doc.Add(Path.GetFileName(exportMxd), mxdValsDict);

            return doc;
        }

        private void AddDataToDictForSingleStoredDisplay(string sdName, ref MapDocuments sdValsDict)
        {
            MapDocumentDataExtractor extractor = new MapDocumentDataExtractor();
            IMap map = GetMapFromStoredDisplayName(sdName);
            sdValsDict.Add(sdName, extractor.GetDataForOneMapDocument(map));
        }

        public IList<ErrorMessage> CompareAllMxdsBetweenFolders(string controlFolder, string compareFolder, IEnumerable<string> mxdList)
        {
            List<ErrorMessage> errors = new List<ErrorMessage>();

            foreach (string mxd in mxdList)
            {
                if (File.Exists(compareFolder + mxd))
                {
                    errors.AddRange(CompareTwoMxds(controlFolder + mxd, compareFolder + mxd));
                }
                else Logger.Warn(compareFolder+mxd +" does not exist. Skipping.");
            }

            return errors;
        }

        public IList<ErrorMessage> CompareTwoMxds(string mxd1Path, string mxd2Path)
        {
            MapDocumentDataExtractor extractor = new MapDocumentDataExtractor();

            Layers mxdVals1Dict = extractor.GetDataForOneMapDocument(GetMapFromMxd(mxd1Path));
            Layers mxdVals2Dict = extractor.GetDataForOneMapDocument(GetMapFromMxd(mxd2Path));

            string fileName = Path.GetFileName(mxd1Path);
            string fileNameTwo = Path.GetFileName(mxd2Path);

            if (fileName.Equals(fileNameTwo))
            {
                fileNameTwo += " Compare";
            }
            List<ErrorMessage> errors = CompareMapDocumentsHelper(fileName, fileNameTwo, mxdVals1Dict, mxdVals2Dict).ToList();
            errors.AddRange(CompareMapDocumentsHelper(fileNameTwo, fileName, mxdVals2Dict, mxdVals1Dict));
            return errors;
        }

        /// <summary>
        ///     Wrapper method to validate one pair of stored displays. Calls the helper method to do the work
        /// </summary>
        /// <param name="sdName1">Name of the first stored display</param>
        /// <param name="sdName2">Name of the second stored display, the stored display to be compared to</param>
        /// <returns>A list of error messages detailing all errors (parse and validation) encountered</returns>
        public IList<ErrorMessage> CompareTwoStoredDisplays(string sdName1, string sdName2)
        {
            MapDocumentDataExtractor extractor = new MapDocumentDataExtractor();
            Layers sdVals1Dict = extractor.GetDataForOneMapDocument(
                GetMapFromStoredDisplayName(sdName1));

            Layers sdVals2Dict = extractor.GetDataForOneMapDocument(
                GetMapFromStoredDisplayName(sdName2));

            return CompareMapDocumentsHelper(sdName1, sdName2, sdVals1Dict, sdVals2Dict);
        }

        /// <summary>
        ///     Method containing most of the logic to compare and validate the stored displays. Iterates through all properties of
        ///     all dictionaries and compares properties. Reports an error whenever a property does not match between the two
        ///     stored displays
        /// </summary>
        /// <param name="sdName1">Name of the first stored display</param>
        /// <param name="sdName2">Name of the second stored display, the stored display to compare to</param>
        /// <param name="sdVals1Dict">The data to compare for the first stored display</param>
        /// <param name="sdVals2Dict">The data to compare for the second stored display</param>
        /// <returns>A list of error messages detailing all errors encountered while comparing stored displays</returns>
        private IList<ErrorMessage> CompareMapDocumentsHelper(string sdName1, string sdName2, Layers sdVals1Dict,
            Layers sdVals2Dict)
        {
            IList<ErrorMessage> errors = new List<ErrorMessage>();

            foreach (KeyValuePair<string, Symbols> sdEntryKvp in sdVals1Dict)
            {
                foreach (KeyValuePair<string, SymbolLayers> symbolKvp in sdEntryKvp.Value)
                {
                    foreach (KeyValuePair<PropertyId, string> datum in symbolKvp.Value)
                    {
                        Symbols symbol2Dict;
                        SymbolLayers symbolLayers2Dict;
                        string res;

                        //If we cannot retrieve the layer (there is no layer name in sd2 that matches a layer name in sd1)
                        if (!sdVals2Dict.TryGetValue(sdEntryKvp.Key, out symbol2Dict))
                        {
                            errors.Add(new ErrorMessage(sdName1, sdName2, sdEntryKvp.Key,
                                new PropertyId(SymbolDataType.LayerName, -1), "<None>"));
                            continue;
                        }

                        //If we can't find a symbol name match between the two stored displays, then check if the symbol name ends
                        // in '-dup'. This means the symbol was marked as a duplicate while extracting from the stored display.
                        //Create an error for this.
                        if (!symbol2Dict.TryGetValue(symbolKvp.Key, out symbolLayers2Dict))
                        {
                            errors.Add(new ErrorMessage(sdName1, "-", sdEntryKvp.Key, new PropertyId(
                                SymbolDataType.SymbolName, -1), symbolKvp.Key));
                            continue;
                        }

                        //If we are able to find the piece of data we are trying to compare, then compare and report an error
                        //if different. Otherwise, report an error that we cannot find it.
                        if (symbolLayers2Dict.TryGetValue(datum.Key, out res))
                        {
                            if (!datum.Value.Equals(res))
                            {
                                errors.Add(new ErrorMessage(sdName1, sdName2, sdEntryKvp.Key, datum.Key,
                                    symbolKvp.Key));
                            }
                        }
                        else
                            errors.Add(new ErrorMessage(sdName1, sdName2, sdEntryKvp.Key, new PropertyId(
                                SymbolDataType.DataType, -1), symbolKvp.Key));
                    }
                }

                {
                    Symbols symbolList;

                    if (sdVals2Dict.TryGetValue(sdEntryKvp.Key, out symbolList) &&
                        sdEntryKvp.Value.Count != symbolList.Count)
                    {
                        errors.Add(new ErrorMessage(sdName1, sdName2, sdEntryKvp.Key,
                            new PropertyId(SymbolDataType.LayerCount, -1), "<None>"));
                    } 
                }
            }

            return errors;
        }


        private IMap GetMapFromStoredDisplayName(string sdName)
        {
            IMap map = new MapClass();
            Logger.Info("Opening SD: " + sdName);

            return !Common.Common.OpenStoredDisplay(sdName, _workspace, map) ? null : map;
        }

        private static IMap GetMapFromMxd(string mxdPath)
        {
            IMapDocument doc = new MapDocumentClass();
            doc.Open(mxdPath);
            int mapCount = doc.MapCount;
            return mapCount > 0 ? doc.Map[0] : null;
        }

        /// <summary>
        ///     Format the data and write into a CSV file for easy viewing
        /// </summary>
        /// <param name="errors">The errors to iterate through, the main data of the CSV</param>
        public static void WriteErrorsToFile(IEnumerable<ErrorMessage> errors)
        {
            string[] rowIdStrings = { "Map Document 1", "Map Document 2", "Layers Name", "Symbols Name", "SymbolLayerIndex" };
            string[] enumValStrings = Enum.GetNames(typeof(SymbolDataType));
            string[] csvHeaderStrings = new string[rowIdStrings.Length + enumValStrings.Length];

            Array.Copy(rowIdStrings, 0, csvHeaderStrings, 0, rowIdStrings.Length);
            Array.Copy(enumValStrings, 0, csvHeaderStrings, rowIdStrings.Length, enumValStrings.Length);

            CsvManipulator manipulator = new CsvManipulator(csvHeaderStrings, rowIdStrings.Length);

            foreach (ErrorMessage e in errors)
            {
                string rowId = GetRowId(e);
                string reverseRowId = GetReverseRowId(e);
                if (!manipulator.RowExists(rowId) && !manipulator.RowExists(reverseRowId))
                {
                    manipulator.AddRow(rowId, new string[enumValStrings.Length]);
                }

                if (manipulator.RowExists(rowId))
                    manipulator.SetPropertyForRow(rowId, e.Error.DataType.ToString(), "X");
                else manipulator.SetPropertyForRow(reverseRowId, e.Error.DataType.ToString(), "X");
            }

            manipulator.PushAwayEmptyColumns();
            manipulator.WriteToFile(ConfigurationManager.AppSettings["CSVWriteLocation"]);
        }

        private static string GetRowId(ErrorMessage error)
        {
            return error.StoredDisplay1 + "," + error.StoredDisplay2 + ",\"" + error.LayerName + "\",\"" +
                   error.SymbolName + "\"," + error.Error.SymbolLayerSymbolLayerId;
        }

        private static string GetReverseRowId(ErrorMessage error)
        {
            return error.StoredDisplay2 + "," + error.StoredDisplay1 + ",\"" + error.LayerName + "\",\"" +
                   error.SymbolName + "\"," + error.Error.SymbolLayerSymbolLayerId;
        }
    }
}