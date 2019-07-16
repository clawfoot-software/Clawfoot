using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Clawfoot.Utilities
{
    //https://www.dotnetperls.com/multimap
    /// <summary>
    /// A dictionary where multiple values can be returned for a single key
    /// </summary>
    /// <typeparam name="T">The type of the value held in this multimap</typeparam>
    public class MultiMap<T> : IEnumerable<KeyValuePair<string, List<T>>>
    {
        Dictionary<string, List<T>> _dictionary = new Dictionary<string, List<T>>();

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
        public void Add(string key, T value)
        {
            List<T> list;
            if (_dictionary.TryGetValue(key, out list))
            {
                list.Add(value);
            }
            else
            {
                list = new List<T>()
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
        public IEnumerator<KeyValuePair<string, List<T>>> GetEnumerator()
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
        public IEnumerable<string> Keys
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
        public List<T> this[string key]
        {
            get
            {
                List<T> list;
                if (!_dictionary.TryGetValue(key, out list))
                {
                    list = new List<T>();
                    _dictionary[key] = list;
                }
                return list;
            }
        }
    }
}
