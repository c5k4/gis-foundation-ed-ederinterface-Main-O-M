using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Miner.Interop;

namespace Telvent.PGE.Test.Data
{
    public class TestModelName : IMMModelNameManager
    {
        #region IMMModelNameManager Members

        public void AddClassModelName(ESRI.ArcGIS.Geodatabase.IObjectClass pClass, string bstrName)
        {
            throw new NotImplementedException();
        }

        public void AddClassModelNameSet(ESRI.ArcGIS.Geodatabase.IObjectClass pClass, ESRI.ArcGIS.esriSystem.IEnumBSTR peNames)
        {
            throw new NotImplementedException();
        }

        public void AddFieldModelName(ESRI.ArcGIS.Geodatabase.IObjectClass pClass, ESRI.ArcGIS.Geodatabase.IField pField, string bstrName)
        {
            throw new NotImplementedException();
        }

        public bool CanReadModelNames(ESRI.ArcGIS.Geodatabase.IObjectClass pClass)
        {
            throw new NotImplementedException();
        }

        public bool CanWriteModelNames(ESRI.ArcGIS.Geodatabase.IObjectClass pClass)
        {
            throw new NotImplementedException();
        }

        public ESRI.ArcGIS.esriSystem.IEnumBSTR ClassModelNames(ESRI.ArcGIS.Geodatabase.IObjectClass Class)
        {
            throw new NotImplementedException();
        }

        public ESRI.ArcGIS.esriSystem.IEnumBSTR ClassNamesFromModelNameDS(ESRI.ArcGIS.Geodatabase.IDataset pDataset, string bstrModelName)
        {
            throw new NotImplementedException();
        }

        public ESRI.ArcGIS.esriSystem.IEnumBSTR ClassNamesFromModelNameWS(ESRI.ArcGIS.Geodatabase.IWorkspace pWorkspace, string bstrModelName)
        {
            throw new NotImplementedException();
        }

        public void ClearClassModelNames(ESRI.ArcGIS.Geodatabase.IObjectClass pClass)
        {
            throw new NotImplementedException();
        }

        public void ClearFieldModelNames(ESRI.ArcGIS.Geodatabase.IObjectClass pClass, ESRI.ArcGIS.Geodatabase.IField pField)
        {
            throw new NotImplementedException();
        }

        public bool ContainsClassModelName(ESRI.ArcGIS.Geodatabase.IObjectClass Class, string Name)
        {
            throw new NotImplementedException();
        }

        public bool ContainsFieldModelName(ESRI.ArcGIS.Geodatabase.IObjectClass pClass, ESRI.ArcGIS.Geodatabase.IField pField, string bstrName)
        {
            throw new NotImplementedException();
        }

        public ESRI.ArcGIS.Geodatabase.IEnumFeatureClass FeatureClassesFromModelName(ESRI.ArcGIS.Geodatabase.IEnumFeatureClass EnumFeatureClass, string bstrModelName)
        {
            throw new NotImplementedException();
        }

        public ESRI.ArcGIS.Geodatabase.IEnumFeatureClass FeatureClassesFromModelNameDS(ESRI.ArcGIS.Geodatabase.IDataset pDataset, string bstrModelName)
        {
            throw new NotImplementedException();
        }

        public ESRI.ArcGIS.Geodatabase.IEnumFeatureClass FeatureClassesFromModelNameWS(ESRI.ArcGIS.Geodatabase.IWorkspace pWorkspace, string bstrModelName)
        {
            throw new NotImplementedException();
        }

        public ESRI.ArcGIS.Geodatabase.IField FieldFromModelName(ESRI.ArcGIS.Geodatabase.IObjectClass ObjectClass, string ModelName)
        {
            throw new NotImplementedException();
        }

        public ESRI.ArcGIS.esriSystem.IEnumBSTR FieldModelNames(ESRI.ArcGIS.Geodatabase.IObjectClass pClass, ESRI.ArcGIS.Geodatabase.IField pField)
        {
            throw new NotImplementedException();
        }

        public ESRI.ArcGIS.esriSystem.IEnumBSTR FieldNamesFromModelName(ESRI.ArcGIS.Geodatabase.IObjectClass pClass, string bstrModelName)
        {
            throw new NotImplementedException();
        }

        public IMMEnumField FieldsFromModelName(ESRI.ArcGIS.Geodatabase.IObjectClass pClass, string bstrModelName)
        {
            throw new NotImplementedException();
        }

        public IMMEnumObjectClass ObjectClassesFromModelNameWS(ESRI.ArcGIS.Geodatabase.IWorkspace pWorkspace, string bstrModelName)
        {
            throw new NotImplementedException();
        }

        public void RemoveClassModelName(ESRI.ArcGIS.Geodatabase.IObjectClass pClass, string bstrName)
        {
            throw new NotImplementedException();
        }

        public void RemoveFieldModelName(ESRI.ArcGIS.Geodatabase.IObjectClass pClass, ESRI.ArcGIS.Geodatabase.IField pField, string bstrName)
        {
            throw new NotImplementedException();
        }

        public IMMEnumTable TablesFromModelNameWS(ESRI.ArcGIS.Geodatabase.IWorkspace pWorkspace, string bstrModelName)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
