using ESRI.ArcGIS.Geodatabase;
using Miner.Geodatabase.Utilities;
using System;

namespace Miner.Geodatabase.FeederManager
{
    internal class FeederManagerFieldModelNames : FieldIndexHelper
    {
        public const string feederIDModelName = "FeederID";

        public const string feederID2ModelName = "FeederID2";

        public const string feederInfoModelName = "FeederInfo";

        public const string circuitSourceIDModelName = "CircuitSourceID";

        public const string parentCircuitSourceIDModelName = "ParentCircuitSourceID";

        public const string subSourceModelName = "SubSource";

        public const string feederLevelModelName = "FeederLevel";

        public const string parentCircuitSourceGUIDModelName = "ParentCircuitSourceGUID";

        public const string circuitSourceGUIDModelName = "CircuitSourceGUID";

        public const string feederNameModelName = "FeederName";

        public const string substationIDModelName = "SubstationID";

        public const string feederSourceInfoModelName = "FeederSourceInfo";

        private int? _circuitSourceIDFieldIndex;

        public static string weightModelName
        {
            get
            {
                return "MMElectricTraceWeight";
            }
        }

        public int WeightFieldIndex
        {
            get
            {
                return base.FieldIndexFromFieldModelName(FeederManagerFieldModelNames.weightModelName);
            }
        }

        public int FeederIDFieldIndex
        {
            get
            {
                return base.FieldIndexFromFieldModelName("FeederID");
            }
        }

        public int FeederID2FieldIndex
        {
            get
            {
                return base.FieldIndexFromFieldModelName("FeederID2");
            }
        }

        public int FeederInfoFieldIndex
        {
            get
            {
                return base.FieldIndexFromFieldModelName("FeederInfo");
            }
        }

        public int ParentCircuitSourceIDFieldIndex
        {
            get
            {
                int num = base.FieldIndexFromFieldModelName("ParentCircuitSourceGUID");
                if (num < 0)
                {
                    num = base.FieldIndexFromFieldModelName("ParentCircuitSourceID");
                }
                return num;
            }
        }

        public int CircuitSourceIDFieldIndex
        {
            get
            {
                if (!this._circuitSourceIDFieldIndex.HasValue)
                {
                    int num = base.FieldIndexFromFieldModelName("CircuitSourceGUID");
                    if (num < 0)
                    {
                        num = base.FieldIndexFromFieldModelName("CircuitSourceID");
                    }
                    if (num < 0)
                    {
                        IDataset dataset = this._table as IDataset;
                        if (dataset != null)
                        {
                            IDataset dataset2 = FeederManagerClassModelNames.GetSubsourceTable(dataset.Workspace) as IDataset;
                            if (dataset2 != null && dataset.Name == dataset2.Name)
                            {
                                num = this._table.FindField(this._table.OIDFieldName);
                            }
                        }
                    }
                    this._circuitSourceIDFieldIndex = new int?(num);
                }
                return this._circuitSourceIDFieldIndex.Value;
            }
        }

        public int SubSourceFieldIndex
        {
            get
            {
                return base.FieldIndexFromFieldModelName("SubSource");
            }
        }

        public int NormalPositionAFieldIndex
        {
            get
            {
                return base.FieldIndexFromFieldModelName("NormalPosition_A");
            }
        }

        public int NormalPositionBFieldIndex
        {
            get
            {
                return base.FieldIndexFromFieldModelName("NormalPosition_B");
            }
        }

        public int NormalPositionCFieldIndex
        {
            get
            {
                return base.FieldIndexFromFieldModelName("NormalPosition_C");
            }
        }

        public int PhaseDesignationFieldIndex
        {
            get
            {
                return base.FieldIndexFromFieldModelName("PHASEDESIGNATION");
            }
        }

        public int OperatingVoltageFieldIndex
        {
            get
            {
                return base.FieldIndexFromFieldModelName("OperatingVoltage");
            }
        }

        public int FeederNameFieldIndex
        {
            get
            {
                return base.FieldIndexFromFieldModelName("FeederName");
            }
        }

        public int SubstationIDFieldIndex
        {
            get
            {
                return base.FieldIndexFromFieldModelName("SubstationID");
            }
        }

        public int FeederSourceInfoFieldIndex
        {
            get
            {
                return base.FieldIndexFromFieldModelName("FeederSourceInfo");
            }
        }

        public int FeederLevelFieldIndex
        {
            get
            {
                return base.FieldIndexFromFieldModelName("FeederLevel");
            }
        }

        public FeederManagerFieldModelNames()
        {
        }

        public FeederManagerFieldModelNames(IFeatureClass featureClass)
            : base(featureClass)
        {
        }

        public FeederManagerFieldModelNames(ITable table)
            : base(table)
        {
        }
    }
}
