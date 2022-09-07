using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PGE.Interface.Integration.DMS.Common
{
    /// <summary>
    /// Not used. Consider deleting
    /// </summary>
    public class RelID : IDictionary<int,int>
    {
        private Dictionary<int, int> _rels;
        private static RelID _value;
        /// <summary>
        /// Not used. Consider deleting
        /// </summary>
        public RelID()
        {
            _rels = new Dictionary<int, int>();
        }
        /// <summary>
        /// Not used. Consider deleting
        /// </summary>
        public static RelID Value
        {
            get
            {
                if (_value == null)
                {
                    _value = new RelID();
                }
                return RelID._value;
            }
        }
        /// <summary>
        /// Not Implemented
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(int key, int value)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        ///  Not used. Consider deleting
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(int key)
        {
            return _rels.ContainsKey(key);
        }
        /// <summary>
        /// Not Implemented
        /// </summary>
        public ICollection<int> Keys
        {
            get { throw new NotImplementedException(); }
        }
        /// <summary>
        /// Not Implemented
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(int key)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Not Implemented
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(int key, out int value)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Not Implemented
        /// </summary>
        public ICollection<int> Values
        {
            get { throw new NotImplementedException(); }
        }
        /// <summary>
        ///  Not used. Consider deleting
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int this[int key]
        {
            get
            {
                return _rels[key];
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        /// <summary>
        /// Not Implemented
        /// </summary>
        /// <param name="item"></param>
        public void Add(KeyValuePair<int, int> item)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Not Implemented
        /// </summary>
        public void Clear()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Not Implemented
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(KeyValuePair<int, int> item)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Not Implemented
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(KeyValuePair<int, int>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Not Implemented
        /// </summary>
        public int Count
        {
            get { throw new NotImplementedException(); }
        }
        /// <summary>
        /// Not Implemented
        /// </summary>
        public bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }
        /// <summary>
        /// Not Implemented
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(KeyValuePair<int, int> item)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Not Implemented
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<int, int>> GetEnumerator()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Not Implemented
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
