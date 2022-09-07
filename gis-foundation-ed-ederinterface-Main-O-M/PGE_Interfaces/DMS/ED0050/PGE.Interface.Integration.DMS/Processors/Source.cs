using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Miner.Geodatabase.Integration;
using PGE.Interface.Integration.DMS.Features;
using Miner.Geodatabase.Integration.Electric;
using PGE.Interface.Integration.DMS.Manager;

namespace PGE.Interface.Integration.DMS.Processors
{
    public class Source : BaseProcessor
    {

        public Source(DataSet data)
            : base(data, "DMSSTAGING.SOURCE")
        {

        }
        public void AddJunction(JunctionFeatureInfo junct, ControlTable controlTable)
        {
            DataRow row = _table.NewRow();
            ElectricJunction ejunct = (ElectricJunction)junct.Junction;
            double id = Utilities.getID(junct);
            string uid = id.ToString();
            //row["ABB_INT_ID"] = id;
            row["NO_KEY"] = uid;
            row["SOFPOS"] = uid;
            //row["ANNOPOS_1"] = 3;
            //row["ANNOPOS_2"] = 6;
            //row["DISTRICT"] = Utilities.GetDistrict(junct);
            row["STATE"] = Utilities.GetState(junct);
            FeatureValues.FCID[junct.ObjectClassID].getValues(row, junct, controlTable);
            GetElectricJunctionValues(ejunct, row);
            GetMappedValues(junct, row);

            /*Changes for ENOS to SAP migration - DMS ..Start..*/
            //Calling function here to set related attributes for service location and then will add the row...    
            
            // Changes for sending all generations to DMS when service location : service point : generation is 1:n:n -- Start 
            if (junct.ObjectClassID == PGE.Interface.Integration.DMS.Common.FCID.Value[PGE.Interface.Integration.DMS.Common.FCID.ServiceLocation])
            {  
                Dictionary<ObjectInfo, ObjectInfo> dictAllServPtGenInfo = null;
                try
                {
                    _log4.Info("Start Finding related records for service location : " + junct.ObjectID);
                    //Iterate each servicepoint and generationinfo -- take only where service point not null , geninfo not null and geninfo.symbology='Primary'
                    dictAllServPtGenInfo = GetAllServicePointRelatedtoPrimGenInfo(junct);
                    if (dictAllServPtGenInfo.Count == 0)
                    {
                        _table.Rows.Add(row);
                    }
                    else
                    {
                        foreach (KeyValuePair<ObjectInfo, ObjectInfo> obj in dictAllServPtGenInfo)
                        {
                            DataRow row_copy = _table.NewRow();
                            row_copy.ItemArray = row.ItemArray;

                            GetRelatedValuesForServiceLocationForGivenServicePointAndGenInfo(junct, row_copy, controlTable, obj.Key, obj.Value);

                            _table.Rows.Add(row_copy);
                        }
                    }

                    _log4.Info("End Finding related records for service location : " + junct.ObjectID);
                }
                catch (Exception exp)
                {
                    _log4.Error(exp.Message+ " at "+exp.StackTrace);
                }
                finally
                {
                    if (dictAllServPtGenInfo != null)
                    {
                        dictAllServPtGenInfo.Clear();
                        dictAllServPtGenInfo = null;
                    }
                }
            }
            else
            {
                _table.Rows.Add(row);
            }

            // Changes for sending all generations to DMS when service location : service point : generation is 1:n:n -- End 

            /*Changes for ENOS to SAP migration - DMS ..End..*/          
        }
    }
}
