using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.Geodatabase;

using Miner.ComCategories;
using Miner.Interop;

using PGE.Common.Delivery.Framework;
using PGE.Common.Delivery.Systems;
using PGE.Common.Delivery.Diagnostics;
using System.Reflection;
using Miner.Geodatabase;
using Miner.Geodatabase.Network;
using Miner.Geodatabase.FeederManager;
using PGE.Desktop.EDER.Utility;
using PGE.Common.Delivery.Framework.FeederManager;
using ESRI.ArcGIS.esriSystem;

namespace PGE.Desktop.EDER.DisplayNamers
{
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    [Guid("2366F131-014C-40F2-AE29-D4E3E2791ED3")]
    [ProgId("PGE.Desktop.EDER.PGE_DisplayName")]
    [ComponentCategory(ComCategory.MMDisplayNameObjects)]
    public class PGE_DisplayName : IMMDisplayNamer
    {
        #region Com Reg Methods

        /// <summary>
        /// Registers the specified reg key.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComRegisterFunction]
        private static void Register(string regKey)
        {
            MMDisplayNameObjects.Register(regKey);
        }
        /// <summary>
        /// Uns the register.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComUnregisterFunction]
        private static void UnRegister(string regKey)
        {
            MMDisplayNameObjects.Unregister(regKey);
        }

        #endregion

        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        private static bool EventSetup = false;

        //ME Q4-2019 Release - DA#190404
        private static string tableName_DisplayNameFields = "EDGIS.PGE_DISPLAYNAME_FIELDS";
        public static IWorkspace _loginWorkspace;
        /// <summary>
        /// Creates a string to display for the given row.
        /// Format:
        ///     [OperatingNumber] ([ObjectID]) in [Division] on [CircuitID]
        /// On failure, returns the row object ID.
        /// </summary>
        /// <param name="pRow">the object to operate on</param>
        public string DisplayString(IRow pRow)
        {
            try
            {
                //Feeder manager 2.0 utilizes joins on layers and the object class must be parsed out from a different interface.
                ITable pRowTable = FeederManager2.GetTableFromRow(pRow);
                var obj = (IObject)pRow;

                //check if particular fields need to be set in Display Name or [OperatingNumber] ([ObjectID]) in [Division] on [CircuitID]
                string displayNameFields = GetDisplayField(pRow);
                if (displayNameFields != "")
                {
                    string displayString = string.Empty;
                    string poleHight = string.Empty;
                    List<string> fieldsList = displayNameFields.Split(',').ToList();
                    foreach (string field in fieldsList)
                    {
                        string fieldValue = string.Empty;

                        if (field.ToUpper() == "SUBTYPECD")
                        {
                            ISubtypes subtypes = (ISubtypes)pRowTable;
                            if (subtypes.HasSubtype)
                            {
                                int subtypeCode = Int32.Parse(obj.GetFieldValue(useDomain: false, fieldName: "SubtypeCD").ToString());
                                fieldValue = subtypes.get_SubtypeName(subtypeCode);
                            }
                            else
                                fieldValue = obj.GetFieldValue(field, false).Convert<string>(string.Empty);
                        }
                        else
                        {
                            //For Support Structure : Adding ' after pol
                            if ( ! string.IsNullOrEmpty(poleHight) && field.ToUpper() == "'")
                            {
                                fieldValue = "'";
                                poleHight = string.Empty;
                            }
                            else
                            {
                                fieldValue = obj.GetFieldValue(field, false).Convert<string>(string.Empty);
                                if (field.ToUpper().Equals("HEIGHT"))
                                {
                                    poleHight = fieldValue;
                                }
                            }
                        }
                        if (displayString != string.Empty && fieldValue != string.Empty)
                        {
                            if (field.ToUpper() != "'")
                            {
                                displayString += " ";
                            }
                        }
                        displayString += fieldValue;
                    }

                    //For Jan Breakfix : Additional Requirement of Mike
                    if (obj.Class.AliasName.Equals("Support Structures"))
                    {
                        string height = string.Empty;
                        string poleClass = string.Empty;
                        string subtype = string.Empty;
                        string barccode = string.Empty;
                        string jpSeq = string.Empty;
                        string jpNum = string.Empty;
                        //HEIGHT,CLASS,SUBTYPECD,BARCODE,JPNUMBER,JPSEQUENCE
                        height = obj.GetFieldValue(fieldsList[0], false).Convert<string>(string.Empty);
                        if (!height.IsNullOrEmpty()) { height = height + "'"; }
                        poleClass = obj.GetFieldValue(fieldsList[1], false).Convert<string>(string.Empty);
                        if (!poleClass.IsNullOrEmpty() && !height.IsNullOrEmpty()) { poleClass = "-" + poleClass; }                       
                        ISubtypes subtypes = (ISubtypes)pRowTable;
                        if (subtypes.HasSubtype)
                        {
                            int subtypeCode = Int32.Parse(obj.GetFieldValue(useDomain: false, fieldName: "SubtypeCD").ToString());
                            subtype = subtypes.get_SubtypeName(subtypeCode);
                        }
                        else
                        {
                            subtype = obj.GetFieldValue(fieldsList[2], false).Convert<string>(string.Empty);
                        }
                        barccode = obj.GetFieldValue(fieldsList[3], false).Convert<string>(string.Empty);
                        
                        jpNum = obj.GetFieldValue(fieldsList[4], false).Convert<string>(string.Empty);
                        jpSeq = obj.GetFieldValue(fieldsList[5], false).Convert<string>(string.Empty);

                        //displayString = height + "'"+"-"+poleClass + " " + subtype +","+ "Pole# " + barccode + ","+ jpNum + "-"+jpSeq  ;
                        displayString = height +  poleClass + " " + subtype + ", " + "# " + barccode + ", " + jpNum + "-" + jpSeq;
                        if (jpNum.IsNullOrEmpty())
                        {
                            displayString = height + poleClass + " " + subtype + ", " + "# " + barccode;
                        }
                        
                        if (barccode.IsNullOrEmpty())
                        {
                            string poleSAPId = obj.GetFieldValue("SAPEQUIPID", false).Convert<string>(string.Empty);
                            //barccode = poleSAPId;
                            displayString = height  + poleClass + " " + subtype + ", " + "SAP ID " + poleSAPId + ", " + jpNum + "-" + jpSeq;
                            if (jpNum.IsNullOrEmpty())
                            {
                                displayString = height  + poleClass + " " + subtype + ", " + "SAP ID " + poleSAPId;
                            }
                            if (poleSAPId.IsNullOrEmpty())
                            {
                               // barccode = obj.OID.ToString();
                                displayString = height  + poleClass + " " + subtype + ", " + "OID " + obj.OID.ToString() + ", " + jpNum + "-" + jpSeq;
                                if (jpNum.IsNullOrEmpty())
                                {
                                    displayString = height  + poleClass + " " + subtype + ", " + "OID " + obj.OID.ToString();
                                }
                            }

                        }
                        if (jpSeq.IsNullOrEmpty() && !jpNum.IsNullOrEmpty())
                        {
                            displayString = displayString.Remove(displayString.Length - 1);
                        }
                        
                        return displayString;
                    }

                    if (obj.Class.AliasName.Equals("Street Light"))
                    {
                       // STREETLIGHTNUMBER,CIRCUITID
                        string streetLightNo = string.Empty;
                        string circuitId = string.Empty;
                        string prefixStr = "SL ";
                            streetLightNo = obj.GetFieldValue(fieldsList[0], false).Convert<string>(string.Empty);
                            //circuitId = obj.GetFieldValue(fieldsList[1], false).Convert<string>(string.Empty);
                            circuitId = GetFeederID(pRow);
                            if (streetLightNo.IsNullOrEmpty())
                            {
                                streetLightNo = obj.OID.Convert<string>(string.Empty);
                                prefixStr = "OID ";
                            }
                            if (circuitId.IsNullOrEmpty())
                            {
                                circuitId = "<No Circuit ID>";
                            }
                            displayString = prefixStr + streetLightNo + " on " + circuitId;
                            return displayString;
                        

                    }

                    if (obj.Class.AliasName.Equals("Service Location"))
                    {
                        string loctationDesc = string.Empty;
                        string circuitId = string.Empty;
                        List<IRow> RelatedObject = new List<IRow>();
                        List<string> streetNumList = new List<string>();
                        List<string> streetNameList = new List<string>();
                        string streetNum = string.Empty;
                        string streetName = string.Empty;
                        IEnumRelationshipClass relClassEnum = null;
                        IRelationshipClass relClas = null;

                        //circuitId = obj.GetFieldValue(fieldsList[1], false).Convert<string>(string.Empty);
                        circuitId = GetFeederID(pRow);
                        if (circuitId.IsNullOrEmpty())
                        {
                            circuitId = "<No Circuit ID>";
                        }
                        
                        relClassEnum = obj.Class.get_RelationshipClasses(esriRelRole.esriRelRoleAny);
                        relClassEnum.Reset();
                        while ((relClas = relClassEnum.Next()) != null)
                        {
                            // To Delete the relatiionship check by Class Model Name
                            if(relClas.DestinationClass.AliasName.Equals("Service Point"))                              
                            {
                                //relClass.DeleteRelationship(relClass.DestinationClass as IObject, pObject);
                                ISet relatedFeatures = relClas.GetObjectsRelatedToObject(obj);
                                relatedFeatures.Reset();
                                object pRelatedObj = null;
                                while ((pRelatedObj = relatedFeatures.Next()) != null)
                                {
                                    IRow pRelatedObjectRow = (IRow)pRelatedObj;
                                    RelatedObject.Add(pRelatedObjectRow);
                                    IObject pRelObj = pRelatedObj as IObject;
                                    streetNum = pRelObj.GetFieldValue("STREETNUMBER", false).Convert<string>(string.Empty);
                                    if (!streetNum.IsNullOrEmpty())
                                    {
                                        streetNumList.Add(streetNum);
                                    }
                                    streetName = pRelObj.GetFieldValue("STREETNAME1", false).Convert<string>(string.Empty);
                                    if (!streetName.IsNullOrEmpty())
                                    {
                                        streetNameList.Add(streetName);
                                    }
                                }
                            }
                        }

                        if (streetNumList.Count() > 0 && streetNameList.Count()> 0)
                        {
                            if (streetNumList.Distinct().Count() == 1 && streetNameList.Distinct().Count() == 1)
                            {
                                displayString = streetNumList[0] + " " + streetNameList[0] + " on " + circuitId;
                            }
                            else
                            {
                                displayString = "OID " + obj.OID + " on " + circuitId;
                            }
                        }
                        else if (streetNumList.Count() == 0 && streetNameList.Count() > 0)
                        {
                            if (streetNameList.Distinct().Count() == 1)
                            {
                                displayString =  streetNameList[0] + " on " + circuitId;
                            }
                            else
                            {
                                displayString = "OID " + obj.OID + " on " + circuitId;
                            }
                        }

                        else if (streetNumList.Count() > 0 && streetNameList.Count() == 0)
                        {
                            if (streetNumList.Distinct().Count() == 1)
                            {
                                displayString = streetNumList[0] + " on " + circuitId;
                            }
                            else
                            {
                                displayString = "OID " + obj.OID + " on " + circuitId;
                            }
                        }

                        else if (streetNumList.Count() == 0 && streetNameList.Count() == 0)
                        {
                            displayString = "OID " + obj.OID + " on " + circuitId;
                        }
                                                
                            return displayString;
                        
                    }

                    if (obj.Class.AliasName.Equals("Transformer"))
                    {
                        //CGC12,CIRCUITID
                        string cgc = string.Empty;
                        string circuitId = string.Empty;
                        
                            cgc = obj.GetFieldValue(fieldsList[0], false).Convert<string>(string.Empty);
                            //circuitId = obj.GetFieldValue(fieldsList[1], false).Convert<string>(string.Empty);
                            circuitId = GetFeederID(pRow);
                            string division = obj.GetFieldValue(useDomain: true, modelName: SchemaInfo.Electric.FieldModelNames.Division).Convert("<no division>");
                            if (cgc.IsNullOrEmpty())
                            {
                                cgc = obj.OID.Convert<string>(string.Empty);
                            }
                            if (circuitId.IsNullOrEmpty())
                            {
                                circuitId = "<No Circuit ID>";
                            }
                            displayString = cgc + " in " + division + " on " + circuitId;
                            return displayString;
                        
                    }


                    if (displayString == string.Empty || displayString.Equals("'"))
                    {
                        // ME Q4-19 : After Feedback received for DA#190404
                       // if (ModelNameManager.ContainsClassModelName(obj.Class, SchemaInfo.Electric.ClassModelNames.SupportStructure))
 
                        if (obj.Class.AliasName.Equals("Support Structures"))
                        {
                            string fieldValueSAPId = string.Empty;
                            fieldValueSAPId = obj.GetFieldValue("SAPEQUIPID", false).Convert<string>(string.Empty);
                            if (fieldValueSAPId != string.Empty)
                            {
                                return string.Format("{0}", fieldValueSAPId);
                            }
                        }
                        //For Anchor , displays Support Structor SAP ID
                        if (obj.Class.AliasName.Equals("Anchor"))
                        {
                            IEnumRelationshipClass relClasses = null;
                            IRelationshipClass relClass = null;
                            relClasses = obj.Class.get_RelationshipClasses(esriRelRole.esriRelRoleAny);
                            relClasses.Reset();
                            relClass = relClasses.Next();

                            while (relClass != null)
                            {
                                if(relClass.OriginClass.AliasName.Equals("Support Structures"))
                                {
                                    ISet relatedFeatures =  relClass.GetObjectsRelatedToObject(obj);

                                    relatedFeatures.Reset();

                                    object pRelatedObj = relatedFeatures.Next();
                                    IObject ssObj = pRelatedObj as IObject;
                                    string fieldValueSapId = string.Empty;
                                    fieldValueSapId = ssObj.GetFieldValue("SAPEQUIPID", false).Convert<string>(string.Empty);
                                    if (fieldValueSapId != string.Empty)
                                    {
                                        return string.Format("{0}", fieldValueSapId);
                                    }
                                }
                                relClass = relClasses.Next();
                            }
 
                    }
                        return string.Format("{0}", obj.OID);
                }

                    else
                        return string.Format("{0}", displayString);
                }

                else
                {
                    string[] operatingNumberMN = { SchemaInfo.Electric.FieldModelNames.OperatingNumber };
                    string[] noOperingNumberDisplayMN = { SchemaInfo.Electric.ClassModelNames.NoOperatingNumberDisplay };

                    string firstValue = "";
                    if ((!PGE.Common.Delivery.Framework.ModelNameFacade.ContainsFieldModelName(pRowTable as IObjectClass, operatingNumberMN))
                        || PGE.Common.Delivery.Framework.ModelNameFacade.ContainsClassModelName(pRowTable as IObjectClass, noOperingNumberDisplayMN))
                    {
                        ISubtypes subtypes = (ISubtypes)pRowTable;
                        if (subtypes.HasSubtype)
                        {
                            int subtypeCode = Int32.Parse(obj.GetFieldValue(useDomain: false, fieldName: "SubtypeCD").ToString());
                            firstValue = subtypes.get_SubtypeName(subtypeCode);
                        }
                        else
                        {
                            firstValue = ((IObjectClass)pRowTable).AliasName;
                        }
                    }
                    else
                    {
                        firstValue = obj.GetFieldValue(useDomain: false, modelName: SchemaInfo.Electric.FieldModelNames.OperatingNumber).Convert("<no operating number>");
                    }

                    var division = obj.GetFieldValue(useDomain: true, modelName: SchemaInfo.Electric.FieldModelNames.Division).Convert("<no division>");

                    string circuitID = GetFeederID(pRow);

                    return string.Format("{0} ({1}) in {2} on {3}", firstValue, obj.OID, division, circuitID);
                }
            }
            catch (Exception ex)
            {
                _logger.Warn("Could not create a display string for row" + pRow.OID + ".", ex);
                return pRow.OID.ToString();
            }
        }

        public string GetDisplayField(IRow pRow)
        {
            IWorkspace loginWorkspace = GetWorkspace();

            string returnString = "";
            if (loginWorkspace != null)
            {
                if (((IWorkspace2)loginWorkspace).NameExists[esriDatasetType.esriDTTable, tableName_DisplayNameFields])
                {
                    var obj = (IObject)pRow;

                    IFeatureClass pfc = FeederManager2.GetFeatureClassFromRow(pRow);
                    IDataset pds = (IDataset)pfc;
                    string fcName = pds.Name;

                    //get subtype of selected feature
                    ITable pRowTable = FeederManager2.GetTableFromRow(pRow);
                    ISubtypes subtypes = (ISubtypes)pRowTable;
                    int? subtypeCode = null;
                    if (subtypes.HasSubtype)
                    {
                        if(pfc.FeatureType.ToString().Equals("esriFTAnnotation"))
                        {
                        subtypeCode = Int32.Parse(obj.GetFieldValue(useDomain: false, fieldName: "AnnotationClassID").ToString());
                        }
                        else
                        {
                            subtypeCode = Int32.Parse(obj.GetFieldValue(useDomain: false, fieldName: "SubtypeCD").ToString());
                        }
                    }

                    //query on PGE_DISPLAYNAME_FIELDS table and get the Display Name Fields for selected feature class
                    ITable displayFieldTable = ((IFeatureWorkspace)loginWorkspace).OpenTable(tableName_DisplayNameFields);
                    if (displayFieldTable != null)
                    {
                        IQueryFilter pQf = new QueryFilterClass();
                        pQf.SubFields = "SUBTYPECD,DISPLAY_FIELDS";
                        pQf.WhereClause = "UPPER(FEATURE_CLASS_NAME) = '" + fcName.ToUpper() + "'";

                        ICursor pCur = displayFieldTable.Search(pQf, true);
                        IRow pTableRow = null;
                        while ((pTableRow = pCur.NextRow()) != null)
                        {
                            //get display fields
                            if ((Convert.ToString(pTableRow.get_Value(pTableRow.Fields.FindField("SUBTYPECD"))).ToUpper() == "ALL") || ((Convert.ToString(pTableRow.get_Value(pTableRow.Fields.FindField("SUBTYPECD"))).Length > 0) && (Int32.Parse(Convert.ToString(pTableRow.get_Value(pTableRow.Fields.FindField("SUBTYPECD")))) == subtypeCode)))
                            {
                                int index = pTableRow.Fields.FindField("DISPLAY_FIELDS");
                               

                                   // returnString = Convert.ToString(pTableRow.get_Value(pRow.Fields.FindField("DISPLAY_FIELDS")));
                                returnString = Convert.ToString(pTableRow.get_Value(pTableRow.Fields.FindField("DISPLAY_FIELDS")));
                                //int index = pRow.Fields.FindField("LABELTEXT");
                                break;
                            }
                        }

                        Marshal.ReleaseComObject(pCur);

                    }
                }
            }
            return returnString;
        }

        private static IWorkspace GetWorkspace()
        {
            // Get and return the logged in workspace
            IMMLoginUtils utils = new MMLoginUtils();
            _loginWorkspace = utils.LoginWorkspace;
            return _loginWorkspace;
        }


        public string GetFeederID(IRow pRow)
        {
            string feederIdsValue = string.Empty;

            IRelQueryTable relQueryTable = pRow.Table as IRelQueryTable;
            if (relQueryTable != null)
            {
                // the FM20 Join will be the Destination Table...
                ITable fm20JoinTable = relQueryTable.DestinationTable;
                for (int i = 0; i < fm20JoinTable.Fields.FieldCount; i++)
                {
                    IField field = fm20JoinTable.Fields.get_Field(i);
                    string fieldName = field.Name;
                }
                int feederIDIndex = fm20JoinTable.Fields.FindField("FeederID");
                if (feederIDIndex > 0)
                {
                    IRow pJoinRow = fm20JoinTable.GetRow(pRow.OID);
                    object feederIdValue = pJoinRow.Value[feederIDIndex];
                    if (feederIdValue != null)
                    {
                        feederIdsValue = feederIdValue.ToString();
                    }

                    if (string.IsNullOrEmpty(feederIdsValue)) { feederIdsValue = "<no circuit ID>"; }
                }
            }
            else
            {
                string[] circuitIds = FeederManager2.GetCircuitIDs(pRow);
                if (circuitIds == null || circuitIds.Length < 1) 
                {
                        feederIdsValue = "<no circuit ID>";
                }
                else 
                {
                    feederIdsValue = circuitIds[0]; 
                }
            }
            return feederIdsValue;
        }

        /// <summary>
        /// Gets a string to display for the custom display namer.
        /// </summary>
        public string Name
        {
            get { return "PGE Display Name"; }
        }


        /// <summary>
        /// Gets a value indicating whether this display namer should be available for the provided dataset.
        /// </summary>
        /// <param name="pDataset">A potential target dataset for the display namer</param>
        /// <returns>true if the display namer should be selectable; otherwise false.</returns>
        public bool get_Enabled(IDataset pDataset)
        {
            try
            {
                return ModelNameFacade.ContainsClassModelName((IObjectClass)pDataset, SchemaInfo.General.CustomDisplayField);
            }
            catch (Exception ex)
            {
                _logger.Warn("PGE_DisplayName.Enabled is unknown because an exception was encountered. Assumed false.", ex);
                return false;
            }
        }
    }

}
