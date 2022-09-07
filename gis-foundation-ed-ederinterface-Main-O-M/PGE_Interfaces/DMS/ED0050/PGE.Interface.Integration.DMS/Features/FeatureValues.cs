using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PGE.Interface.Integration.DMS.Features
{
    /// <summary>
    /// Singleton Dictionary class containing custom featurevalue processors. New ones must be added here
    /// </summary>
    public class FeatureValues : IDictionary<int,IFeatureValues>
    {
        private Dictionary<int, IFeatureValues> _features;
        private static FeatureValues _FCID;

        /// <summary>
        /// Initialize all the custom featurevalue processors. Add new ones here
        /// </summary>
        public FeatureValues()
        {
            _features = new Dictionary<int, IFeatureValues>();
            _features.Add(Common.FCID.Value[Common.FCID.PrimaryOH], new PrimaryOH());
            _features.Add(Common.FCID.Value[Common.FCID.PrimaryUG], new PrimaryUG());
            _features.Add(Common.FCID.Value[Common.FCID.DistBusBar], new DistrBusBar());
            _features.Add(Common.FCID.Value[Common.FCID.StitchPoint], new StitchPoint());
            //*Changes for ENOS to SAP migration - DMS .Start .. Adding here for Service location */
            //_features.Add(Common.FCID.Value[Common.FCID.PrimaryGen], new PrimaryGen());
            _features.Add(Common.FCID.Value[Common.FCID.ServiceLocation], new ServiceLocation());
            //*Changes for ENOS to SAP migration - DMS .End ..*/
            _features.Add(Common.FCID.Value[Common.FCID.Transformer], new Transformer());
            _features.Add(Common.FCID.Value[Common.FCID.PrimaryMeter], new PrimaryMeter());
            _features.Add(Common.FCID.Value[Common.FCID.DynamicProtectiveDevice], new DPD());
            _features.Add(Common.FCID.Value[Common.FCID.Capacitor], new Capacitor());
            _features.Add(Common.FCID.Value[Common.FCID.Switch], new Switch());
            _features.Add(Common.FCID.Value[Common.FCID.VoltageRegulator], new VoltageRegulator());

            _features.Add(Common.FCID.Value[Common.FCID.SUBBusBar], new SubBusBar());
            _features.Add(Common.FCID.Value[Common.FCID.SUBPrimaryUG], new SubUG());
            _features.Add(Common.FCID.Value[Common.FCID.SUBPrimaryOH], new SubOH());
            _features.Add(Common.FCID.Value[Common.FCID.SUBStitchPoint], new SubStitchPoint());
            _features.Add(Common.FCID.Value[Common.FCID.SUBTransformer], new SUBTransformer());
            _features.Add(Common.FCID.Value[Common.FCID.SUBVoltageRegulator], new SUBVoltageRegulator());
        }
        /// <summary>
        /// The singleton FeatureValues dictionary. Enter an objects FCID to get the featurevalue processor for that object. 
        /// Make sure the FCID is present first with the ContainsKey method
        /// </summary>
        public static FeatureValues FCID
        {
            get
            {
                if (_FCID == null)
                {
                    _FCID = new FeatureValues();
                }
                return FeatureValues._FCID;
            }
        }
        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(int key, IFeatureValues value)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Check if there is a processor for the feature class
        /// </summary>
        /// <param name="key">The FCID of the feature class</param>
        /// <returns>True if a processor exists</returns>
        public bool ContainsKey(int key)
        {
            return _features.ContainsKey(key);
        }
        /// <summary>
        ///  Not implemented
        /// </summary>
        public ICollection<int> Keys
        {
            get { throw new NotImplementedException(); }
        }
        /// <summary>
        ///  Not implemented
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(int key)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        ///  Not implemented
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(int key, out IFeatureValues value)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        ///  Not implemented
        /// </summary>
        public ICollection<IFeatureValues> Values
        {
            get { throw new NotImplementedException(); }
        }
        /// <summary>
        /// Get the FeatureValue processor. Set  not implemented
        /// </summary>
        /// <param name="key">The FCID of the feature class</param>
        /// <returns>The FeatureValue processor</returns>
        public IFeatureValues this[int key]
        {
            get
            {
                return _features[key];
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="item"></param>
        public void Add(KeyValuePair<int, IFeatureValues> item)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Not implemented
        /// </summary>
        public void Clear()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(KeyValuePair<int, IFeatureValues> item)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(KeyValuePair<int, IFeatureValues>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Not implemented
        /// </summary>
        public int Count
        {
            get { throw new NotImplementedException(); }
        }
        /// <summary>
        /// Not implemented
        /// </summary>
        public bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }
        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(KeyValuePair<int, IFeatureValues> item)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Not implemented
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<int, IFeatureValues>> GetEnumerator()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Not implemented
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
