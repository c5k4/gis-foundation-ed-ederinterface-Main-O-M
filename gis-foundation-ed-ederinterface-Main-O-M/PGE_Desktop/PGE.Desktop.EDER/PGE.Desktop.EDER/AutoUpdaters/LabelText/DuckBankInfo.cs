using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Miner.Interop;
using PGE.Common.Delivery.Framework;
using ESRI.ArcGIS.Geodatabase;
using PGE.Common.Delivery.Diagnostics;
using ESRI.ArcGIS.esriSystem;
using System.Reflection;

namespace PGE.Desktop.EDER.AutoUpdaters.LabelText
{
    /// <summary>
    /// DuctBankInfo deserializes duct info and returns enumeration suitable for link queries.
    /// </summary>
    public class DuctBankInfo
    {
        /// <summary>
        /// A list of duct definition objects.
        /// </summary>
        private List<IMMDuctDefinition> _ductDefinitions = new List<IMMDuctDefinition>();

        private static readonly PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        /// <summary>
        /// Initializes a new instance of the <see cref="DuctBankInfo"/> class related to the provided conduit feature.
        /// </summary>
        /// <param name="conduitFeature">The conduit feature.</param>
        public DuctBankInfo(IFeature conduitFeature)
        {
            try
            {
                //var ductBankConfiguration = new MMDuctBankConfigClass();
                //var configField = ModelNameFacade.FieldFromModelName(conduitFeature.Class, SchemaInfo.Electric.ClassModelNames.DuctBankConfiguration);
                //object configValue = conduitFeature.GetFieldValue(configField.Name);
                IMMDuctBankConfig ductBankConfiguration = PGE.Desktop.EDER.UFM.UfmHelper.GetDuctBankConfig(conduitFeature);
                if (ductBankConfiguration != null)
                {
                    //ductBankConfiguration.LoadFromField(conduitFeature, conduitFeature.Fields.FindField(configField.Name));
                    var ductBankList = (ID8List)ductBankConfiguration;
                    ductBankList.Reset();
                    InitializeDuctInfo((ID8List)ductBankList.Next(false));
                }
                else
                {
                    _logger.Debug("Duct Configuration BLOB not present.");
                    return;
                }
            }
            catch (Exception ex)
            {
                _logger.Warn("Failed to deserialize duct configuration blob.", ex);
                return;
            }
        }

        /// <summary>
        /// Initialize the DuctInfo list.
        /// </summary>
        /// <param name="ductViewList">The list.</param>
        private void InitializeDuctInfo(ID8List ductViewList)
        {
            // A null list indicates that the Duct Definition has not been edited.
            if (ductViewList == null) return;

            var queue = new Queue<ID8List>(16);
            var ductDefinitionCount = 0;
            var listItems = 0;
            queue.Enqueue(ductViewList);
            while (queue.Count > 0)
            {
                var currentItem = (ductViewList = queue.Dequeue()) as ID8ListItem;
                if (currentItem == null)
                    continue;
                listItems++;
                if (currentItem is IMMDuctDefinition)
                {
                    _ductDefinitions.Add((IMMDuctDefinition)currentItem);
                    ductDefinitionCount++;
                }
                ductViewList.Reset();
                while ((currentItem = ductViewList.Next(false)) != null)
                {
                    if (currentItem is ID8List)
                        queue.Enqueue((ID8List)currentItem);
                }
            }
        }

        /// <summary>
        /// Returns an enumeration of the ducts present in the conduit.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DuctInfo> GetDucts()
        {
            foreach (IMMDuctDefinition def in _ductDefinitions)
            {
                yield return new DuctInfo(def);
            }
        }
    }
}
