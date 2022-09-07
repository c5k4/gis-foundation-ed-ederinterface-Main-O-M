using System;
using System.Collections.Generic;
using System.Text;
using Miner.Interop.Process;
using System.Runtime.InteropServices;

namespace PGE.Common.Delivery.Process
{
    /// <summary>
    /// A wrapper around the <see cref="IDictionary"/> interface that uses reference types instead of value types.
    /// </summary>
    [ComVisible(false), ClassInterface(ClassInterfaceType.None)]
    public class ReferenceDictionary : Dictionary<string, object>, IDictionary
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceDictionary"/> class.
        /// </summary>
        public ReferenceDictionary()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceDictionary"/> class.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        public ReferenceDictionary(IDictionary dictionary)
        {
            object[] keys = (object[])dictionary.Keys();
            foreach (object k in keys)
            {
                object key = k;
                this.Add((string)key, dictionary.get_Item(ref key));
            }
        }

        #region IDictionary Members

        /// <summary>
        /// Adds the specified key.
        /// </summary>
        /// <param name="Key">The key.</param>
        /// <param name="Item">The item.</param>
        void IDictionary.Add(ref object Key, ref object Item)
        {
            base.Add(Key.ToString(), Item);
        }

        /// <summary>
        /// Gets or sets the compare mode.
        /// </summary>
        /// <value>The compare mode.</value>
        CompareMethod IDictionary.CompareMode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the number of key/value pairs contained in the <see cref="T:System.Collections.Generic.Dictionary`2"/>.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The number of key/value pairs contained in the <see cref="T:System.Collections.Generic.Dictionary`2"/>.
        /// </returns>
        int IDictionary.Count
        {
            get
            {
                return base.Count;
            }
        }

        /// <summary>
        /// Existses the specified key.
        /// </summary>
        /// <param name="Key">The key.</param>
        /// <returns></returns>
        bool IDictionary.Exists(ref object Key)
        {
            return base.ContainsKey(Key.ToString());
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        System.Collections.IEnumerator IDictionary.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        object IDictionary.Items()
        {
            return base.Values;
        }

        object IDictionary.Keys()
        {
            return base.Keys;
        }

        void IDictionary.Remove(ref object Key)
        {
            if (base.ContainsKey(Key.ToString()))
                base.Remove(Key.ToString());
        }

        void IDictionary.RemoveAll()
        {
            base.Clear();
        }

        object IDictionary.get_HashVal(ref object Key)
        {
            return null;
        }

        object IDictionary.get_Item(ref object Key)
        {
            return base[Key.ToString()];
        }

        void IDictionary.let_Item(ref object Key, ref object Item)
        {
            base[Key.ToString()] = Item;
        }

        void IDictionary.set_Item(ref object Key, ref object Item)
        {
            base[Key.ToString()] = Item;
        }

        void IDictionary.set_Key(ref object Key, ref object Item)
        {
            base[Key.ToString()] = Item;
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((System.Collections.IEnumerable)this).GetEnumerator();
        }

        #endregion

        /// <summary>
        /// Creates a new instance of the <see cref="ReferenceDictionary"/> using the specified <see cref="IDictionary"/> object.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <returns>A new instance of the <see cref="ReferenceDictionary"/> containing the information from the specified dictionary.</returns>
        public static ReferenceDictionary Create(IDictionary dictionary)
        {
            ReferenceDictionary reference = new ReferenceDictionary();
            object[] keys = (object[])dictionary.Keys();
            foreach (object k in keys)
            {
                object key = k;
                reference.Add((string)key, dictionary.get_Item(ref key));
            }

            return reference;
        }
    }
}
