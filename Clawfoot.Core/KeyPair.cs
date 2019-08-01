using System;
using System.Collections.Generic;
using System.Text;

namespace Clawfoot.Core
{
    /// <summary>
    /// Represents a pair of keys that are related to each other and are not necessarily a key/value pair.
    /// Used within the IndexedKeyMap
    /// </summary>
    /// <typeparam name="TKey1"></typeparam>
    /// <typeparam name="TKey2"></typeparam>
    public struct KeyPair<TKey1, TKey2> : IEquatable<KeyPair<TKey1, TKey2>>
    {
        public TKey1 Key1 { get; }
        public TKey2 Key2 { get; }

        public KeyPair(TKey1 key1, TKey2 key2)
        {
            Key1 = key1;
            Key2 = key2;
        }

        public static implicit operator KeyValuePair<TKey1, TKey2>(KeyPair<TKey1, TKey2> pair)
        {
            return new KeyValuePair<TKey1, TKey2>(pair.Key1, pair.Key2);
        }

        //---------------------------------------------------------
        //===== Equality =====

        public bool Equals(KeyPair<TKey1, TKey2> other)
        {
            if (!EqualityComparer<TKey1>.Default.Equals(other.Key1, Key1)) return false;
            if (!EqualityComparer<TKey2>.Default.Equals(other.Key2, Key2)) return false;

            return true;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is KeyPair<TKey1, TKey2>)) return false;

            return Equals((KeyPair<TKey1, TKey2>)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 13;
                hash = hash * 23 + (Key1.GetHashCode());
                hash = hash * 23 + (Key2.GetHashCode());
                return hash;
            }
        }
    }
}
