using ESRI.ArcGIS.Geodatabase;

namespace PGE.Common.Delivery.ArcFM
{
    /// <summary>
    /// 
    /// </summary>
    public interface IFacilityIDGenerator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        string GetFacilityID(IObject obj);
    }
}
