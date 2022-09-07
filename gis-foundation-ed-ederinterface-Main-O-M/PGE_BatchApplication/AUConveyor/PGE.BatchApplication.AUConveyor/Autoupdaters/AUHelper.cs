using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using Miner.Geodatabase;
using Miner.Interop;
using System;

namespace PGE.BatchApplication.AUConveyor.Autoupdaters
{
    /// <summary>
    /// Helper class used to determine information about autoupdaters relative to the currently-configured process.
    /// </summary>
    internal class AUHelper
    {
        IWorkspace _ws;
        AUWrapper _auWrapper;

        List<KeyValuePair<IObjectClass, int>> _compatibleObjClasses = null;
        internal List<KeyValuePair<IObjectClass, int>> ConfiguredClasses
        {
            get
            {
                if (_compatibleObjClasses == null)
                {
                    _compatibleObjClasses = new List<KeyValuePair<IObjectClass, int>>();
                    if (_auWrapper != null && !string.IsNullOrEmpty(_auWrapper.Name))
                    {
                        IEnumDataset eDS = _ws.get_Datasets(esriDatasetType.esriDTAny);
                        IterateDatasets(eDS);
                    }
                }

                return _compatibleObjClasses;
            }
        }

        internal AUHelper(IWorkspace workspace, AUWrapper auWrapper)
        {
            _ws = workspace;
            _auWrapper = auWrapper;
        }

        internal bool IsClassEligible(IObjectClass oc)
        {
            return ConfiguredClasses.Where(kvp => kvp.Key.ObjectClassID == oc.ObjectClassID).Count() > 0;
        }

        private void IterateDatasets(IEnumDataset pEnumDataset)
        {
            pEnumDataset.Reset();
            for (IDataset ds = pEnumDataset.Next(); ds != null; ds = pEnumDataset.Next())
            {
                if (ds.Type == esriDatasetType.esriDTFeatureClass || ds.Type == esriDatasetType.esriDTTable)
                {
                    ProcessObjectClass(ds as IObjectClass);
                }
                else if (ds.Type == esriDatasetType.esriDTFeatureDataset)
                {
                    IterateDatasets(ds.Subsets);
                }
            }
        }

        private void ProcessObjectClass(IObjectClass oc)
        {
            if (oc != null)
            {
                ISubtypes ocSt = oc as ISubtypes;

                //If the object class doesn't have a subtype, pass in "-1".
                if (!ocSt.HasSubtype)
                    ProcessSubtype(oc, -1);
                else
                {
                    IEnumSubtype enSt = ocSt.Subtypes;

                    int code;
                    for (string desc = enSt.Next(out code); desc != null; desc = enSt.Next(out code))
                    {
                        //Traverse subtypes
                        ProcessSubtype(oc, code);
                    }
                }
            }
        }

        private void ProcessSubtype(IObjectClass oc, int subtypeCode)
        {
            IMMConfigTopLevel ctl = ConfigTopLevel.Instance;

            IMMSubtype mmSubtype = ctl.GetSubtypeForEdit(oc, subtypeCode);
            ID8List list = mmSubtype as ID8List;

            list.Reset();
            for (ID8ListItem listItem = list.Next(false); listItem != null; listItem = list.Next(false))
            {
                if (listItem.ItemType == mmd8ItemType.mmitAutoValue)
                {
                    IMMAutoValue autoValue = listItem as IMMAutoValue;
                    if (autoValue.AutoGenID != null)
                    {
                        if (new Guid(autoValue.AutoGenID.Value.ToString()) == _auWrapper.GUID)
                        {
                            _compatibleObjClasses.Add(new KeyValuePair<IObjectClass, int>(oc, mmSubtype.SubtypeCode));
                        }
                    }
                }
            }
        }
    }
}
