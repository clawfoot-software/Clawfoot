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
    /// <typeparam name="V"></typeparam>
    public class MultiMap<V> : IEnumerable<KeyValuePair<string, List<V>>>
    {
        Dictionary<string, List<V>> _dictionary =
            new Dictionary<string, List<V>>();

        public int Count
        {
            get
            {
                return _dictionary.Count;
            }
        }
        public void Add(string key, V value)
        {
            List<V> list;
            if (this._dictionary.TryGetValue(key, out list))
            {
                list.Add(value);
            }
            else
            {
                list = new List<V>();
                list.Add(value);
                this._dictionary[key] = list;
            }
        }

        public IEnumerator<KeyValuePair<string, List<V>>> GetEnumerator()
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

        public IEnumerable<string> Keys
        {
            get
            {
                return this._dictionary.Keys;
            }
        }

        public List<V> this[string key]
        {
            get
            {
                List<V> list;
                if (!this._dictionary.TryGetValue(key, out list))
                {
                    list = new List<V>();
                    this._dictionary[key] = list;
                }
                return list;
            }
        }
    }
}
