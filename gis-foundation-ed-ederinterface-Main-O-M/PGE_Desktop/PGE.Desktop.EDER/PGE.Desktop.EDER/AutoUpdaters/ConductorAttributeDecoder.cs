using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Reflection;

using Miner.ComCategories;
using Miner.Interop;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Framework;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using log4net;
using System.Windows.Forms;
using PGE.Common.Delivery.Diagnostics;

namespace PGE.Desktop.EDER.AutoUpdaters
{
    [Guid("CFEC3137-251A-4941-9A8B-F4E87D409031")]
    [ProgId("PGE.Desktop.EDER.ConductorAttributeDecoder")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    public class ConductorAttributeDecoder : BaseSpecialAU
    {
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        const string ConductorCodeMapTableName = "PGE_ConductorCodeMap";

        public ConductorAttributeDecoder()
            : base("PGE Conductor Attributes from Code AU")
        { }


        protected override bool InternalEnabled(IObjectClass objectClass, mmEditEvent eEvent)
        {
            
            return (ModelNameFacade.ContainsClassModelName(objectClass, SchemaInfo.Electric.ClassModelNames.PrimaryOHConductorInfo)
                    && ModelNameFacade.ContainsAllFieldModelNames(objectClass, SchemaInfo.Electric.FieldModelNames.ConductorCode,
                                                                               SchemaInfo.Electric.FieldModelNames.ConductorMaterial,
                                                                               SchemaInfo.Electric.FieldModelNames.ConductorSize,
                                                                               SchemaInfo.Electric.FieldModelNames.ConductorType))
                || (ModelNameFacade.ContainsClassModelName(objectClass, SchemaInfo.Electric.ClassModelNames.PrimaryUGConductorInfo)
                    && ModelNameFacade.ContainsAllFieldModelNames(objectClass, SchemaInfo.Electric.FieldModelNames.ConductorCode,
                                                                               SchemaInfo.Electric.FieldModelNames.ConductorMaterial,
                                                                               SchemaInfo.Electric.FieldModelNames.ConductorSize,
                                                                               SchemaInfo.Electric.FieldModelNames.ConductorType,
                                                                               SchemaInfo.Electric.FieldModelNames.ConductorInsulation,
                                                                               SchemaInfo.Electric.FieldModelNames.ConductorRating));
        }

        protected override void InternalExecute(IObject obj, mmAutoUpdaterMode eAUMode, mmEditEvent eEvent)
        {
            var objectClass = obj.Class;
            var conductorCode = obj.GetFieldValue(null, false, SchemaInfo.Electric.FieldModelNames.ConductorCode).Convert(-1);
            var featureWorkspace = (IFeatureWorkspace)((IDataset)((IRow)obj).Table).Workspace; // I feel like I'm writing C++ again. ArcObjects: they're interface-arific!
            ITable conductorCodeMap = null;
            IRow codeRow = null;
            QueryFilter queryFilter = null;
            try
            {
                conductorCodeMap = new MMTableUtilsClass().OpenTable(ConductorCodeMapTableName, featureWorkspace);
                if (conductorCodeMap == null)
                    throw new Exception("Failed to load table " + ConductorCodeMapTableName);
                Action<IObject, IRow> decodeFields = (o, r) => { ; }; // This is a NOOP. See below.

                if (ModelNameFacade.ContainsClassModelName(objectClass, SchemaInfo.Electric.ClassModelNames.PrimaryOHConductorInfo))
                {
                    queryFilter = new QueryFilter { WhereClause = string.Format("CONDUCTORCODE={0} AND SUBTYPECD={1}", conductorCode, 1) };
                    decodeFields = DecodeOverheadFields;
                }
                else if (ModelNameFacade.ContainsClassModelName(objectClass, SchemaInfo.Electric.ClassModelNames.PrimaryUGConductorInfo))
                {
                    queryFilter = new QueryFilter { WhereClause = string.Format("CONDUCTORCODE={0} AND SUBTYPECD={1}", conductorCode, 2) };
                    decodeFields = DecodeUndergroundFields;
                }

                codeRow = new Extensions.CursorEnumerator(() => conductorCodeMap.Search(queryFilter, false)).FirstOrDefault();
                if (codeRow == null)
                {
                    throw new Exception("Conductor code is not configured in " + ConductorCodeMapTableName);
                }
                decodeFields(obj, codeRow); // Either decode the fields or, if someone changed the escape logic above, do nothing.
            }
            catch (Exception ex)
            {
                _logger.Error("PGE Conductor Attributes from Code AU failed.", ex);
            }
            finally
            {
                ReleaseAllTheThings(codeRow);
            }
        }

        void ReleaseAllTheThings(params object[] comObjects)
        {
            foreach (var item in comObjects)
            {
                if (item is IDisposable)
                    ((IDisposable)item).Dispose();
                else if (item != null && Marshal.IsComObject(item))
                    while (Marshal.ReleaseComObject(item) > 0) ;
            }
        }
        
        private static void DecodeUndergroundFields(IObject obj, IRow codeRow)
        {
            // Collect

            var objectConductorMaterial     = FieldInstance.FromModelName(obj, SchemaInfo.Electric.FieldModelNames.ConductorMaterial    );
            var objectConductorSize         = FieldInstance.FromModelName(obj, SchemaInfo.Electric.FieldModelNames.ConductorSize        );
            var objectConductorType         = FieldInstance.FromModelName(obj, SchemaInfo.Electric.FieldModelNames.ConductorType        );
            var objectConductorInsulation   = FieldInstance.FromModelName(obj, SchemaInfo.Electric.FieldModelNames.ConductorInsulation  );
            var objectConductorRating       = FieldInstance.FromModelName(obj, SchemaInfo.Electric.FieldModelNames.ConductorRating      );
                        
            var rowConductorMaterial        = FieldInstance.FromFieldName(codeRow, "Material"       );
            var rowConductorSize            = FieldInstance.FromFieldName(codeRow, "ConductorSize"  );
            var rowConductorType            = FieldInstance.FromFieldName(codeRow, "Type"  );
            var rowConductorInsulation      = FieldInstance.FromFieldName(codeRow, "Insulation"     );
            var rowConductorRating          = FieldInstance.FromFieldName(codeRow, "Rating"         );

            // Verify
            var errorFormat = "Object is missing field with model name {0}";
            if (objectConductorMaterial     == null) throw new Exception(string.Format(errorFormat, SchemaInfo.Electric.FieldModelNames.ConductorMaterial   ));
            if (objectConductorSize         == null) throw new Exception(string.Format(errorFormat, SchemaInfo.Electric.FieldModelNames.ConductorSize       ));
            if (objectConductorType         == null) throw new Exception(string.Format(errorFormat, SchemaInfo.Electric.FieldModelNames.ConductorType       ));
            if (objectConductorInsulation   == null) throw new Exception(string.Format(errorFormat, SchemaInfo.Electric.FieldModelNames.ConductorInsulation ));
            if (objectConductorRating       == null) throw new Exception(string.Format(errorFormat, SchemaInfo.Electric.FieldModelNames.ConductorRating     ));
            
            errorFormat = "Missing field '{0}' on table {1}";
            if (rowConductorMaterial        == null) throw new Exception(string.Format(errorFormat, "Material"      , ConductorCodeMapTableName));
            if (rowConductorSize            == null) throw new Exception(string.Format(errorFormat, "ConductorSize" , ConductorCodeMapTableName));
            if (rowConductorType            == null) throw new Exception(string.Format(errorFormat, "Type"          , ConductorCodeMapTableName));
            if (rowConductorInsulation      == null) throw new Exception(string.Format(errorFormat, "Insulation"    , ConductorCodeMapTableName));
            if (rowConductorRating          == null) throw new Exception(string.Format(errorFormat, "Rating"        , ConductorCodeMapTableName));

            // Apply

            objectConductorMaterial.Value   = rowConductorMaterial.Value;
            objectConductorSize.Value       = rowConductorSize.Value;
            objectConductorType.Value       = rowConductorType.Value;
            objectConductorInsulation.Value = rowConductorInsulation.Value;
            objectConductorRating.Value     = rowConductorRating.Value;
        }

        private static void DecodeOverheadFields(IObject obj, IRow codeRow)
        {
            // Collect
            try
            {
                var objectConductorMaterial = FieldInstance.FromModelName(obj, SchemaInfo.Electric.FieldModelNames.ConductorMaterial);
                var objectConductorSize = FieldInstance.FromModelName(obj, SchemaInfo.Electric.FieldModelNames.ConductorSize);
                var objectConductorType = FieldInstance.FromModelName(obj, SchemaInfo.Electric.FieldModelNames.ConductorType);
                var objectConductorInsulation = FieldInstance.FromModelName(obj, SchemaInfo.Electric.FieldModelNames.ConductorInsulation);

                var rowConductorMaterial = FieldInstance.FromFieldName(codeRow, "Material");
                var rowConductorSize = FieldInstance.FromFieldName(codeRow, "ConductorSize");
                var rowConductorType = FieldInstance.FromFieldName(codeRow, "Type");
                var rowConductorInsulation = FieldInstance.FromFieldName(codeRow, "Insulation");
                // Verify

                var errorFormat = "Object is missing field with model name {0}";
                if (objectConductorMaterial == null) throw new Exception(string.Format(errorFormat, SchemaInfo.Electric.FieldModelNames.ConductorMaterial));
                if (objectConductorSize == null) throw new Exception(string.Format(errorFormat, SchemaInfo.Electric.FieldModelNames.ConductorSize));
                if (objectConductorType == null) throw new Exception(string.Format(errorFormat, SchemaInfo.Electric.FieldModelNames.ConductorType));
                if (objectConductorInsulation == null) throw new Exception(string.Format(errorFormat, SchemaInfo.Electric.FieldModelNames.ConductorInsulation));
                


                errorFormat = "Missing field '{0}' on table {1}";
                if (rowConductorMaterial == null) throw new Exception(string.Format(errorFormat, "Material", ConductorCodeMapTableName));
                if (rowConductorSize == null) throw new Exception(string.Format(errorFormat, "ConductorSize", ConductorCodeMapTableName));
                if (rowConductorType == null) throw new Exception(string.Format(errorFormat, "Type", ConductorCodeMapTableName));
                if (rowConductorInsulation == null) throw new Exception(string.Format(errorFormat, "Insulation", ConductorCodeMapTableName));
                
                // Apply

                objectConductorMaterial.Value = rowConductorMaterial.Value;
                objectConductorSize.Value = rowConductorSize.Value;
                objectConductorType.Value = rowConductorType.Value;
                objectConductorInsulation.Value = rowConductorInsulation.Value;
            }
            catch (Exception ee)
            {
            }
        }
    }
}
