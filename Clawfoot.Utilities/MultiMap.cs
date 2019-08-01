using Clawfoot.Core;
using Clawfoot.Utilities.MultiMaps;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Clawfoot.Utilities
{
    /// <summary>
    /// Backwards compatible MultiMap version for string-only keys
    /// </summary>
    /// <typeparam name="TValue">The type of the value held in this multimap</typeparam>
    public class MultiMap<TValue> : MultiMap<string, TValue> {}

    //https://www.dotnetperls.com/multimap
    /// <summary>
    /// A dictionary where multiple values can be returned for a single key
    /// </summary>
    /// <typeparam name="TKey">The type of the key for this multimap</typeparam>
    /// <typeparam name="TValue">The type of the value held in this multimap</typeparam>
    public class MultiMap<TKey, TValue> : IEnumerable<KeyValuePair<TKey, List<TValue>>>
    {
        Dictionary<TKey, List<TValue>> _dictionary = new Dictionary<TKey, List<TValue>>();

        /// <summary>
        /// The count of key value pairs cotained in the dictionary
        /// </summary>
        public int Count
        {
            get
            {
                return _dictionary.Count;
            }
        }

        /// <summary>
        /// Add a new value with the provided key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(TKey key, TValue value)
        {
            List<TValue> list;
            if (_dictionary.TryGetValue(key, out list))
            {
                list.Add(value);
            }
            else
            {
                list = new List<TValue>()
                {
                    value
                };

                _dictionary[key] = list;
            }
        }

        /// <summary>
        /// Eumerate the KeyValuePairs contained within the dictionary
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<TKey, List<TValue>>> GetEnumerator()
        {
            foreach (var item in _dictionary)
            {
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// The collection of keys within the dictionary
        /// </summary>
        public IEnumerable<TKey> Keys
        {
            get
            {
                return _dictionary.Keys;
            }
        }

        /// <summary>
        /// Get the collection of values associated with this key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<TValue> this[TKey key]
        {
            get
            {
                List<TValue> list;
                if (!_dictionary.TryGetValue(key, out list))
                {
                    list = new List<TValue>();
                    _dictionary[key] = list;
                }
                return list;
            }
        }
    }

    public class MultiKeyMapBase<TKey1, TKey2, TValue> : IEnumerable<KeyValuePair<TKey1, List<TValue>>>
    {
        private protected Dictionary<KeyPair<TKey1, TKey2>, List<TValue>> _valuesDictionary = new Dictionary<KeyPair<TKey1, TKey2>, List<TValue>>();
        private protected KeyPairMap<TKey1, TKey2> _keysMap = new KeyPairMap<TKey1, TKey2>();

        /// <summary>
        /// The count of key value pairs contained in the dictionary
        /// </summary>
        public int Count
        {
            get
            {
                return _valuesDictionary.Count;
            }
        }

        public IEnumerator<KeyValuePair<TKey1, List<TValue>>> GetEnumerator()
        {
            foreach(var item in _valuesDictionary)
            {
                yield return new KeyValuePair<TKey1, List<TValue>>(item.Key.Key1, item.Value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

    public class MultiKeyMap<TKey1, TKey2, TValue> : MultiKeyMapBase<TKey1, TKey2, TValue>, IEnumerable<KeyValuePair<TKey2, List<TValue>>>
    {
        public void Add(TKey1 key1, TKey2 key2, TValue value)
        {
            List<TValue> valueList;
            var keyPair = new KeyPair<TKey1, TKey2>(key1, key2);

            if (_valuesDictionary.TryGetValue(keyPair, out valueList))
            {
                valueList.Add(value);
            }
            else
            {
                valueList = new List<TValue>() { value };
                _keysMap.Add(keyPair);
                _valuesDictionary.Add(keyPair, valueList);
            }
        }

        /// <summary>
        /// Get the collection of values associated with this key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<TValue> this[TKey1 key]
        {
            get
            {
                List<TValue> list;
                KeyPair<TKey1, TKey2> keyPair;

                if(!_keysMap.TryGetValue(key, out keyPair))
                {
                    list = new List<TValue>();
                }
                else
                {
                    list = _valuesDictionary[keyPair];
                }

                return list;
            }
        }

        /// <summary>
        /// Get the collection of values associated with this key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<TValue> this[TKey2 key]
        {
            get
            {
                List<TValue> list;
                KeyPair<TKey1, TKey2> keyPair;

                if (!_keysMap.TryGetValue(key, out keyPair))
                {
                    list = new List<TValue>();
                }
                else
                {
                    list = _valuesDictionary[keyPair];
                }

                return list;
            }
        }

        public IEnumerator<KeyValuePair<TKey2, List<TValue>>> GetEnumerator()
        {
            foreach (var item in _valuesDictionary)
            {
                yield return new KeyValuePair<TKey2, List<TValue>>(item.Key.Key2, item.Value);
            }
        }
    }
}
