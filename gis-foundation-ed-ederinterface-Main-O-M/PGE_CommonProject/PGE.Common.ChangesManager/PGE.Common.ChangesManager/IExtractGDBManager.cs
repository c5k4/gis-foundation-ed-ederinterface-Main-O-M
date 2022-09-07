using System;
namespace PGE.Common.ChangesManager
{
    public interface IExtractGDBManager
    {
        string BaseLocation { get; }
        void CreateGDB(string location);
        string EditsLocation { get; }
        void ExportToBaseGDB();
        void ExportToEditsGDB();
        void KillConnections();
        ESRI.ArcGIS.Geodatabase.IFeatureClass OpenBaseFeatureClass();
        ESRI.ArcGIS.Geodatabase.IFeatureClass OpenEditsFeatureClass();
        void RolloverGDBs();
        void Dispose();
    }
}
