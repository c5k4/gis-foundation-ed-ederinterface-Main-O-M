using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Miner.Geodatabase.Integration;
using PGE.Interface.Integration.DMS.Common;
using System.Diagnostics;
using PGE.Interface.Integration.DMS.Manager;

namespace PGE.Interface.Integration.DMS.Features
{
    public class SUBTransformer : IFeatureValues
    {
        public void getValues(System.Data.DataRow row, Miner.Geodatabase.Integration.FeatureInfo info, ControlTable controlTable)
        {
            double largestRatingMVARating = 0;
            //check if there are any units
             List<ObjectInfo> units = Utilities.getRelatedObjects(info, FCID.Value[FCID.SUBTransformerUnit]);
             foreach (ObjectInfo unit in units)
             {
                 row["SUMMER_EMERG_KVA"] = Utilities.GetDBFieldValue(unit, "SUMMEREMERGENCYMVA");
                 row["SUMMER_NORM_KVA"] = Utilities.GetDBFieldValue(unit, "SUMMERNORMALMVA");
                 row["WINTER_EMER_KVA"] = Utilities.GetDBFieldValue(unit, "WINTEREMERGENCYMVA");
                 row["WINTER_NORM_KVA"] = Utilities.GetDBFieldValue(unit, "WINTERNORMALMVA");
                 //check if there are any related ratings
                 List<ObjectInfo> ratings = Utilities.getRelatedObjects(unit, FCID.Value[FCID.SUBTransformerRating]);
                 foreach (ObjectInfo rating in ratings)
                 {
                     row["MANUFACTURER"] = Utilities.GetDBFieldValue(rating, "MANUFACTURER");
                     row["SERIAL_NO"] = Utilities.GetDBFieldValue(rating, "SERIALNUMBER");
                     row["RATING_CLASS"] = Utilities.GetDBFieldValue(rating, "RATINGCLASS", "SUB Transformer Cooling Class");
                     /* MVARating code if confirmed to be needed.
                     try
                     {
                         float mvaRating = float.Parse(Utilities.GetDBFieldValue(rating, "MVARATING").ToString());
                         if (mvaRating > largestRatingMVARating)
                         {
                             largestRatingMVARating = mvaRating;
                         }
                     }
                     catch (Exception e){ }
                     */
                 }
             }

            // MVRating code if confirmed to be needed
             //try { row["RATED_KVA"] = largestRatingMVARating; }
             //catch { }
             //try { row["DEVICESIZE"] = largestRatingMVARating; }
             //catch { }

             //check for any loadtap changers
             List<ObjectInfo> ltcs = Utilities.getRelatedObjects(info, FCID.Value[FCID.SUBLoadTapChanger]);
             foreach (ObjectInfo ltc in ltcs)
             {
                 row["NUM_TAPS"] = Utilities.GetDBFieldValue(ltc, "NUMBEROFTAPS");
             }
        }
    }
}
