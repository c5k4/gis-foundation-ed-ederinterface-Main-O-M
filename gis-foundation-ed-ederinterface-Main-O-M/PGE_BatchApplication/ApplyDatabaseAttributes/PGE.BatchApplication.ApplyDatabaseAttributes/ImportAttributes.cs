using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesGDB;
using Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using System.IO;
using PGE_DBPasswordManagement;

namespace PGE.BatchApplication.ApplyDatabaseAttributes
{
    public class ImportAttributes
    {
        IWorkspace workspace = null;
        string xslLocation = "";

        /// <summary>
        /// This class will import attributes specified in the xsl file into the database specified by the sde connection file.
        /// </summary>
        /// <param name="xslFileLocation">Location of xsl file</param>
        /// <param name="sdeConnectionFileLocation">Location of sde connection file</param>
        public ImportAttributes(string xslFileLocation, string ConnectionFileLocation, string databaseConnectionType)
        {
            string SDE_Path = default;
            xslLocation = xslFileLocation;
            if (databaseConnectionType == "SDE")
            {
                SDE_Path = ReadEncryption.GetSDEPath(ConnectionFileLocation);
                SdeWorkspaceFactory wsFactory = new SdeWorkspaceFactory();
                workspace = wsFactory.OpenFromFile(SDE_Path, 0);
            }
            else if (databaseConnectionType == "PGDB")
            {
                AccessWorkspaceFactory accessWSFactory = new AccessWorkspaceFactoryClass();
                workspace = accessWSFactory.OpenFromFile(ConnectionFileLocation, 0);
            }
        }

        public void Import()
        {
            LoopThroughSheets();
        }

        private void LoopThroughSheets()
        {
            Microsoft.Office.Interop.Excel.Application exApp = new Microsoft.Office.Interop.Excel.ApplicationClass();
            Workbook book = exApp.Workbooks.Open(new FileInfo(xslLocation).FullName);
            int iStartFrom = 0;
            bool foundStart = false;

            //Let's prep the xlsx
            foreach (Worksheet sheet in exApp.ActiveWorkbook.Worksheets)
            {
                //let find out which row we should start from
                string tempValue;
                for (int i = 1; i < 15; i++)
                {
                    if (sheet.Range["A" + i].Value == null) { continue; }
                    tempValue = sheet.Range["A" + i].Value.ToString();
                    if (tempValue.ToUpper() == "NUMBER")
                    {
                        iStartFrom = i + 1;
                        foundStart = true;
                        break;
                    }
                }
                if (foundStart) { break; }
            }

            //Let's prep the xlsx
            foreach (Worksheet sheet in exApp.ActiveWorkbook.Worksheets)
            {
                if (sheet.Range["A" + (iStartFrom - 1)].Value == null || sheet.Range["A" + (iStartFrom - 1)].Value.ToString().ToUpper() != "NUMBER")
                {
                    continue;
                }
                string sA1_name;
                sA1_name = sheet.Range["A1"].Value.ToString();
                sA1_name = sA1_name.Replace(" ", "");

                //Our list to hold the field properties for thie table
                List<FieldProperties> fieldPropsList = new List<FieldProperties>();

                for (int i = iStartFrom; i < 250; i++)
                {
                    if (sheet.Range["B" + i].Value == null) { break; }
                    //TODO: We have our feature name and all the fields that we need to set so we can do this now.
                    FieldProperties props = new FieldProperties();
                    props.FieldName = sheet.Range["B" + i].Value + "";
                    props.Alias = sheet.Range["C" + i].Value + "";
                    props.Description = sheet.Range["D" + i].Value + "";
                    props.ArcFMVisible = sheet.Range["E" + i].Value + "";
                    props.ArcFMEditable = sheet.Range["F" + i].Value + "";
                    props.ArcFMAllowNulls = sheet.Range["G" + i].Value + "";
                    props.ArcFMAllowMassUpdate = sheet.Range["H" + i].Value + "";
                    props.ArcFMAU = sheet.Range["I" + i].Value + "";
                    props.ArcFMValidationRules = sheet.Range["J" + i].Value + "";
                    props.DefaultValue = sheet.Range["K" + i].Value + "";

                    fieldPropsList.Add(props);
                }

                //If there are no fields defined then continue on to the next worksheet in the xsl.
                if (fieldPropsList.Count < 1) { continue; }

                IFeatureWorkspace featWorkspace = workspace as IFeatureWorkspace;
                ITable table = null;

                try
                {
                    table = featWorkspace.OpenTable(sA1_name);
                }
                catch (Exception)
                {
                    //Table could not be found
                    Console.WriteLine("\nERROR: The table \"" + sA1_name + "\" could not be found\n");
                    Console.WriteLine("Press a key to continue.");
                    Console.ReadKey(true);
                }

                if (table != null)
                {
                    Console.WriteLine("\nProcessing Table: \"" + sA1_name + "\"");

                    //Set an exclusive schemalock on the table before modifying field properties
                    ISchemaLock schemaLock = table as ISchemaLock;
                    try
                    {
                        schemaLock.ChangeSchemaLock(esriSchemaLock.esriExclusiveSchemaLock);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("\n     ERROR: An exclusive lock on the table \"" + sA1_name + "\" could not be obtained\n");
                        continue;
                    }

                    //We found this table in the database so we can go ahead and update the field properties
                    UpdateFieldProperties(table, fieldPropsList);

                    schemaLock.ChangeSchemaLock(esriSchemaLock.esriSharedSchemaLock);
                }
                if (table != null) { while (Marshal.ReleaseComObject(table) > 0) { } }
            }

            //Close our book and application
            book.Close(false);
            exApp.Quit();
        }

        private void UpdateFieldProperties(ITable table, List<FieldProperties> propsList)
        {
            for (int i = 0; i < propsList.Count; i++)
            {
                int fieldIndex = table.Fields.FindField(propsList[i].FieldName);
                if (fieldIndex == -1)
                {
                    Console.WriteLine("     ERROR: Unable to find field \"" + propsList[i].FieldName + "\"");
                    Console.WriteLine("     Press a key to continue.");
                    Console.ReadKey(true);
                    continue;
                }

                IField field = table.Fields.get_Field(fieldIndex);

                bool outputField = false;
                
                ModifyFieldAlias(table, field.Name, field.AliasName, propsList[i].Alias, ref outputField);

                //ModifyFieldDefaultValue(table, field.Name, field.DefaultValue.ToString(), propsList[i].DefaultValue);
            }
        }

        /// <summary>
        /// Alters the current field alias with the new alias.  If the current and the old match, no change is made
        /// </summary>
        /// <param name="table">Table that owns the field</param>
        /// <param name="fieldName">Field name to be modified</param>
        /// <param name="currentAlias">Current Alias</param>
        /// <param name="newAlias">New Alias</param>
        /// <param name="outputField">Whether or not the console has written field information to the database.</param>
        private void ModifyFieldAlias(ITable table, string fieldName, string currentAlias, string newAlias, ref bool outputField)
        {
            if (currentAlias == newAlias) { return; }
            
            IClassSchemaEdit schemaEdit = table as IClassSchemaEdit;
            schemaEdit.AlterFieldAliasName(fieldName, newAlias);

            if (!outputField)
            {
                Console.WriteLine("     Processing Field: \"" + fieldName + "\"");
                outputField = true;
            }
            Console.WriteLine("          Updating alias Old: \"" + currentAlias + "\" New: " + newAlias + "\"");
        }

        ///// <summary>
        ///// Alters the current field default value with the new value.  If the current and the old match, no change is made
        ///// </summary>
        ///// <param name="table">Table that owns the field</param>
        ///// <param name="fieldName">Field name to be modified</param>
        ///// <param name="currentAlias">Current Default Value</param>
        ///// <param name="newAlias">New Default Value</param>
        //private void ModifyFieldDefaultValue(ITable table, string fieldName, string currentDefault, string newDefault)
        //{
        //    if (currentDefault == newDefault) { return; }

        //    IClassSchemaEdit schemaEdit = table as IClassSchemaEdit;
        //    try
        //    {
        //        schemaEdit.AlterDefaultValue(fieldName, newDefault);
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine("          ERROR: Unable to update the default value");
        //        return;
        //    }
        //    Console.WriteLine("          Updating default value Old: \"" + currentDefault + "\" New: " + newDefault + "\"");
        //}

        ///// <summary>
        ///// Alters the current field default value with the new value.  If the current and the old match, no change is made
        ///// </summary>
        ///// <param name="table">Table that owns the field</param>
        ///// <param name="fieldName">Field name to be modified</param>
        ///// <param name="currentAlias">Current Default Value</param>
        ///// <param name="newAlias">New Default Value</param>
        //private void ModifyFieldDomain(ITable table, string fieldName, string currentDomain, string newDomain)
        //{
        //    if (currentDomain == newDomain) { return; }

        //    IWorkspaceDomains wsDomains = workspace as IWorkspaceDomains;
        //    IDomain domain =  wsDomains.get_DomainByName(newDomain);

        //    if (domain == null)
        //    {
        //        Console.WriteLine("          ERROR: Unable to find domain \"" + newDomain + "\"");
        //    }

        //    IClassSchemaEdit schemaEdit = table as IClassSchemaEdit;
        //    schemaEdit.AlterDomain(fieldName, domain);
            
        //    Console.WriteLine("          Updating default value Old: \"" + currentDomain + "\" New: \"" + newDomain);
        //}
    }

    struct FieldProperties
    {
        //Esri Field Properties
        public string FieldName;
        public string Alias;
        public string Description;
        public string DefaultValue;

        //ArcFM Field Properties
        public string ArcFMVisible;
        public string ArcFMEditable;
        public string ArcFMAllowNulls;
        public string ArcFMAllowMassUpdate;
        public string ArcFMAU;
        public string ArcFMValidationRules;
    }
}
