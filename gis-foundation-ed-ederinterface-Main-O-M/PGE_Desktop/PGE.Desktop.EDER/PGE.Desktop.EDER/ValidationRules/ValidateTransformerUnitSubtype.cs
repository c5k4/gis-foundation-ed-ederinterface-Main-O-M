using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

using Miner.Interop;
using Miner.Geodatabase;
using Miner.ComCategories;

using PGE.Common.Delivery.Framework;
using PGE.Common.Delivery.ArcFM;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;

namespace PGE.Desktop.EDER.ValidationRules
{
    /// <summary>
    /// Validates a Transformer for TransformerUnits related to it have same subtype.
    /// </summary>
    [ComVisible(true)]
    [Guid("3E371C95-228D-4070-A937-8C399D0699FF")]
    [ProgId("PGE.Desktop.EDER.ValidateTransformerUnitSubtype")]
    [ComponentCategory(ComCategory.MMValidationRules)]
    public class ValidateTransformerUnitSubtype : BaseValidationRule
    {
        /// <summary>
        /// Static variable to hold information about the TransformerUnit Subtype that can be allowed for a specific TransformerSubtype.
        /// </summary>
        private static Dictionary<int, List<int>> SubtypeMap
        {
            get;
            set;
        }

        /// <summary>
        /// Constructor calls the Base constructor with PGE Validate Transformer Unit Subtypes as the name of validation Rule and Transformer as the modelname.
        /// </summary>
        public ValidateTransformerUnitSubtype()
            : base("PGE Validate Transformer Unit Subtypes", SchemaInfo.Electric.ClassModelNames.Transformer)
        {

        }

        protected override ID8List InternalIsValid(IRow row)
        {
            //Change to address INC000003803926 QA/QC freezing on the QAQC subtask 
            //if (!ValidationFilterEngine.Instance.IsQAQCRuleEnabled(_Name, base.Severity))
            //    return _ErrorList; 

            var obj = row as IFeature;
            if (row == null)
            {
                return _ErrorList;
            }
            //Get all related Transformer Unit objects
            var relatedObjects = obj.GetRelatedObjects(null, modelNames: SchemaInfo.Electric.ClassModelNames.TransformerUnit);
            List<int> subtypeToCheck = new List<int>();
            //Continue only if there are any related records.
            if (relatedObjects.Count() > 0)
            {
                //Get distinct subtype Codes of TransformerUnits.
                //If this list count is greater than 1 that means the related transformer Units have different subtypes. Add to error.
                var unitSubtypeCodes = relatedObjects.Select(o => ((IRowSubtypes)o).SubtypeCode).Distinct();
                if (unitSubtypeCodes.Count() > 1)
                {
                    AddError("The transformer has related units with different subtypes");
                }
                //Check if the TransformerUnit subtype is allowed for the given Transformer Subtype. If not add error.
                if (SubtypesMisMatchFound(obj, relatedObjects))
                {
                    AddError("The Transformer is related to a Unit and the Unit's subtype is not allowed for the transformer subtype");
                }
            }
            return _ErrorList;
        }


        /// <summary>
        /// Checks if the subtypes of related TransformerUnit are valid for the Transformer Subtype 
        /// </summary>
        /// <param name="transformerObject"></param>
        /// <param name="relateUnits"></param>
        /// <returns></returns>
        private bool SubtypesMisMatchFound(IObject transformerObject, IEnumerable<IObject> relateUnits)
        {
            bool retVal = false;
            //Populate the Transformer Subtypes and allowed TransformerUnit Subtype for the specific subtype.
            //Happens only once.
            PopulateSubtypeMatchTable(transformerObject.Class, relateUnits.First().Class);
            IRowSubtypes transformerSubtype = (IRowSubtypes)transformerObject;
            List<int> allowedUnitsSubtypes = SubtypeMap[transformerSubtype.SubtypeCode];
            //Get only those Elements where the subtype is available in the allowed units list.
            var allowedUnits = relateUnits.Where(o => allowedUnitsSubtypes.Contains(((IRowSubtypes)o).SubtypeCode));
            //If the allowedUnits count is less than the relateUnits count that means one or more units have a subtype that is not allowed.
            retVal = allowedUnits.Count()<relateUnits.Count();
            return retVal;
        }

        /// <summary>
        /// Populates the transformer subtype and for each transformer subtype allowed TransformerUnit subtypes.
        /// </summary>
        /// <param name="transformerObjectClass">Transformer Object class</param>
        /// <param name="transformerUnitObjectClass">Trasnformer Unit Objectclass</param>
        private void PopulateSubtypeMatchTable(IObjectClass transformerObjectClass, IObjectClass transformerUnitObjectClass)
        {
            if (SubtypeMap== null)
            {
                //Initialize SubtypeMap object
                SubtypeMap = new Dictionary<int, List<int>>();
                //Hardcoded to include all TransformerUnit subtype - temporary
                List<int> transformerUnitSubtype = new List<int>() { 1, 2, 3 };
                //get the Subtypes of Transformer
                ISubtypes transformerSubtype = (ISubtypes)transformerObjectClass;
                IEnumSubtype transformerSubtypes = transformerSubtype.Subtypes;
                transformerSubtypes.Reset();
                int subtype = -1;
                //For each Transformer Subtype populate the allowed Transformer Unit Subtype.
                while (transformerSubtypes.Next(out subtype) != null)
                {
                    List<int> UnitSubtypes = new List<int>();
                    //for now all the subtypes are allowed.
                    switch (subtype)
                    {
                        case 1:
                        case 2:
                        case 3:
                        case 4:
                        case 5:
                        case 6:
                        case 7:
                        case 8:
                        default:
                            SubtypeMap.Add(subtype, transformerUnitSubtype);
                            break;
                    }
                }
            }
        }
    }
}
