using Clawfoot.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Clawfoot.Utilities.MultiMaps
{
    /// <summary>
    /// Base class for KeyPairMap to avoid CS0695
    /// </summary>
    /// <typeparam name="TKey1"></typeparam>
    /// <typeparam name="TKey2"></typeparam>
    public class KeyPairMapBase<TKey1, TKey2> : IEnumerable<TKey1>
    {
        private protected HashSet<KeyPair<TKey1, TKey2>> _keysHash = new HashSet<KeyPair<TKey1, TKey2>>();

        private protected Dictionary<TKey1, TKey2> _key1AssociationDictionary = new Dictionary<TKey1, TKey2>();
        private protected Dictionary<TKey2, TKey1> _key2AssociationDictionary = new Dictionary<TKey2, TKey1>();

        private protected void AddNewPair(KeyPair<TKey1, TKey2> pair)
        {
            _keysHash.Add(pair);
            _key1AssociationDictionary.Add(pair.Key1, pair.Key2);
            _key2AssociationDictionary.Add(pair.Key2, pair.Key1);
        }

        public IEnumerator<TKey1> GetEnumerator()
        {
            foreach (var item in _key1AssociationDictionary)
            {
                yield return item.Key;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class KeyPairMap<TKey1, TKey2> : KeyPairMapBase<TKey1, TKey2>, IEnumerable<TKey2>
    {
        /// <summary>
        /// The count of key pairs held within this map
        /// </summary>
        public int Count
        {
            get
            {
                return _keysHash.Count;
            }
        }

        /// <summary>
        /// Adds a new set of keys
        /// Throws an ArgumentException if the keys already exist
        /// </summary>
        /// <param name="key1"></param>
        /// <param name="key2"></param>
        public void Add(TKey1 key1, TKey2 key2)
        {
            var pair = new KeyPair<TKey1, TKey2>(key1, key2);

            if (_keysHash.Contains(pair))
            {
                throw new ArgumentException("This set of keys already exists in the map");
            }

            AddNewPair(pair);
        }

        /// <summary>
        /// Adds a new set of keys
        /// Throws an ArgumentException if the keys already exist
        /// </summary>
        /// <param name="pair"></param>
        public void Add(KeyPair<TKey1, TKey2> pair)
        {
            if (_keysHash.Contains(pair))
            {
                throw new ArgumentException("This set of keys already exists in the map");
            }

            AddNewPair(pair);
        }

        public KeyPair<TKey1, TKey2> this[TKey1 key1]
        {
            get
            {
                TKey2 key2;
                if (_key1AssociationDictionary.TryGetValue(key1, out key2))
                {
                    return new KeyPair<TKey1, TKey2>(key1, key2);
                }
                throw new KeyNotFoundException($"The Key {key1} was not found in this map");
            }
        }

        public KeyPair<TKey1, TKey2> this[TKey2 key2]
        {
            get
            {
                TKey1 key1;
                if (_key2AssociationDictionary.TryGetValue(key2, out key1))
                {
                    return new KeyPair<TKey1, TKey2>(key1, key2);
                }
                throw new KeyNotFoundException($"The Key {key2} was not found in this map");
            }
        }

        public bool TryGetValue(TKey1 key1, out KeyPair<TKey1, TKey2> value)
        {
            if (Contains(key1))
            {
                value = this[key1];
                return true;
            }

            value = default;
            return false; 
        }

        public bool TryGetValue(TKey2 key2, out KeyPair<TKey1, TKey2> value)
        {
            if (Contains(key2))
            {
                value = this[key2];
                return true;
            }

            value = default;
            return false;
        }

        /// <summary>
        /// If this map contains the provided key
        /// </summary>
        /// <param name="key1"></param>
        /// <returns></returns>
        public bool Contains(TKey1 key1)
        {
            return _key1AssociationDictionary.ContainsKey(key1);
        }

        /// <summary>
        /// If this map contains the provided key
        /// </summary>
        /// <param name="key2"></param>
        /// <returns></returns>
        public bool Contains(TKey2 key2)
        {
            return _key2AssociationDictionary.ContainsKey(key2);
        }

        /// <summary>
        /// If this map contains the provided key pair.
        /// THis is the same as indivdually calling the single key Contains methods
        /// </summary>
        /// <param name="pair"></param>
        /// <returns></returns>
        public bool Contains(KeyPair<TKey1, TKey2> pair)
        {
            return _keysHash.Contains(pair);
        }

        IEnumerator<TKey2> IEnumerable<TKey2>.GetEnumerator()
        {
            foreach (var item in _key2AssociationDictionary)
            {
                yield return item.Key;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
