using System;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using Miner.ComCategories;
using Miner.Framework.Xml.Serialization;
using Miner.Interop;
using PGE.Common.Delivery.Framework;

namespace PGE.Desktop.EDER.D8TreeTools
{
    [Guid("94EC8903-9856-45E5-A5E6-432EB4C30A5C")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComponentCategory(ComCategory.D8SelectionTreeTool)]
    public class PgePlaceCrossSection : IMMTreeViewTool
    {
        /// <summary>
        /// Registers the specified reg key.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComRegisterFunction]
        private static void Register(string regKey)
        {
            D8SelectionTreeTool.Register(regKey);
        }
        /// <summary>
        /// Uns the register.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComUnregisterFunction]
        private static void UnRegister(string regKey)
        {
            D8SelectionTreeTool.Unregister(regKey);
        }

        public string Name
        {
            get {
                
                return "PGE Place Cross Section"; }
        }

        public int Priority
        {
            get { return _ootbPlaceCrossSection.Priority; }
        }

        public int Category
        {
            get { return _ootbPlaceCrossSection.Category; }
        }

        private IEditor _editor;
        private static IMMTreeViewTool _ootbPlaceCrossSection;

        public PgePlaceCrossSection()
        { 
            if (_ootbPlaceCrossSection == null)
            {
                const string mmCrossSectionProgId = "MMUlsDesktop.MMPlaceCrossSection";
                Type oType = Type.GetTypeFromProgID(mmCrossSectionProgId);
                object obj = Activator.CreateInstance(oType);
                _ootbPlaceCrossSection = obj as IMMTreeViewTool;
            }

            if (_editor == null)
            {
                UID uId = new UID {Value = "esriEditor.Editor"};
                _editor = Common.Delivery.ArcFM.ApplicationFacade.Application.FindExtensionByCLSID(uId) as IEditor;
            }
        }

        public void Execute(IMMTreeViewSelection pSelection)
        {
            _ootbPlaceCrossSection.Execute(pSelection);
        }

        public int get_Enabled(IMMTreeViewSelection pSelection)
        {
            mmToolState enabledState = mmToolState.mmTSNone ;
            try
            {
                pSelection.Reset();
                ID8ListItem item = pSelection.Next;
                if (item != null)
                {
                    if (item.ItemType == mmd8ItemType.mmd8itFeature)
                    {
                        ID8GeoAssoc geoAssoc = item as ID8GeoAssoc;
                        if (geoAssoc.AssociatedGeoRow is IFeature)
                        {
                            IFeature feature = geoAssoc.AssociatedGeoRow as IFeature;
                            if (ModelNameFacade.ContainsClassModelName(feature.Class, new[] { SchemaInfo.UFM.ClassModelNames.Conduit }))
                            {
                                if (_editor.EditState == esriEditState.esriStateEditing)
                                {
                                    enabledState = mmToolState.mmTSVisible | mmToolState.mmTSEnabled;
                                }
                            }
                        }
                    }
                }
            }
            catch 
            {
              
            }
            return (int) enabledState;
        }
    }
}
