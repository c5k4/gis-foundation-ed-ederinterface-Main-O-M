using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

using Miner.Interop;
using Miner.Geodatabase;
using Miner.ComCategories;

using PGE.Common.Delivery.ArcFM;
using ESRI.ArcGIS.Geodatabase;
using PGE.Common.Delivery.Framework;
using PGE.Common.Delivery.Diagnostics;
using log4net;
using System.Reflection;

namespace PGE.Desktop.EDER.ValidationRules
{
    /// <summary>
    /// Validates DeviceGroup relationship with Padmounted/Subsurface Structure
    /// </summary>
    [ComVisible(true)]
    [Guid("5E33D340-B252-4775-B1DD-E3E6D10DE2FF")]
    [ProgId("PGE.Desktop.EDER.DeviceGroupRelToStructure")]
    [ComponentCategory(ComCategory.MMValidationRules)]
    public class DeviceGroupRelToStructure : BaseValidationRule
    {
        #region Constructor

        /// <summary>
        /// Initializes the new instance of <see cref="DeviceGroupRelToStructure"/> class
        /// </summary>
        public DeviceGroupRelToStructure()
            : base("PGE Device Group Rel To Structure", SchemaInfo.Electric.ClassModelNames.DeviceGroup)
        {
        }

        #endregion Constructor

        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        #region Overridden BaseValidationRule Methods
              

        /// <summary>
        /// Execute Validation for DeviceGroup Related To Structure
        /// </summary>
        /// <param name="pRow">The Object Row to Validate.</param>
        /// <returns>Returns ID8List of IMMValidationError items.</returns>
        protected override ID8List InternalIsValid(ESRI.ArcGIS.Geodatabase.IRow pRow)
        {
            try
            {

                if (!ModelNameFacade.ContainsClassModelName(pRow.Table as IObjectClass, SchemaInfo.Electric.ClassModelNames.DeviceGroup))
                {
                    return _ErrorList;
                }
                string stringError = string.Empty;

                // Array of Parent ModelName to check relation
                string[] modelsNames = { SchemaInfo.Electric.ClassModelNames.PadMountStructure, 
                                       SchemaInfo.Electric.ClassModelNames.SubSurfaceStructure  };

                //Checks Relation with DeviceGroup
                if (!CheckParentRel(pRow, modelsNames))
                {
                    stringError = "The Device Group (OID: " + pRow.OID.ToString() + ") needs to be related to either a Subsurface Structure or PadMounted Structure.";
                    AddError(stringError);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error Executing Validation Rule "+_Name , ex);
                throw;
            }
            return _ErrorList;
        }

        #endregion Overridden BaseValidationRule Methods
        
        #region Private Method
        /// <summary>
        /// Validates the object class for a relationship with the class assigned with the required model names
        /// </summary>
        /// <param name="oclass">The object class.</param>
        /// <param name="modelName">Name of the model.</param>
        /// <returns>If Relationship exist returns True, else False</returns>
        private bool CheckParentRel(IRow pRow, params string[] modelName)
        {
            IObjectClass pclass = pRow.Table as IObjectClass;
            //Gets all destination Relations availble on ObjectClass
            IEnumRelationshipClass enumRelClass = pclass.get_RelationshipClasses(esriRelRole.esriRelRoleDestination);
            enumRelClass.Reset();
            IRelationshipClass relClass = enumRelClass.Next();
            IEnumRelationship enumRelObj = null;
            IRelationship relObj = null;
            //Loop through related objectClass
            while (relClass != null)
            {                
                //Checks Origin Class for ModelName. If Exists return True else False
                if (ModelNameFacade.ContainsClassModelName(relClass.OriginClass, modelName))
                {
                    enumRelObj = relClass.GetRelationshipsForObject(pRow as IObject);
                    enumRelObj.Reset();
                    relObj = enumRelObj.Next();

                    //Checks Origin Object for relation. If Exists return True else False
                    if (relObj.OriginObject != null)
                    {
                        return true;
                    }
                }
                else
                {
                    _logger.Error("Error modelName not available on Origin Class");
                }
                               
                relClass = enumRelClass.Next();
            }
            return false;
        }
        #endregion Private Method






    }
}
