using Clawfoot.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clawfoot.Extensions
{
    public static class GenericExtensions
    {
        //https://stackoverflow.com/a/56116478
        /// <summary>
        /// Gets the name of the method of type T
        /// Used like myInstance.GetMemberName(x => nameof(x.MyMember))
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="nameofMethod"></param>
        /// <returns></returns>
        public static string GetMemberName<T>(this T instance, Func<T, string> nameofMethod) where T : class
        {
            return nameofMethod(instance);
        }

        public static KeyValuePair<TKey1, TValue> Parse<TKey1, TKey2, TValue>(this KeyValuePair<KeyPair<TKey1, TKey2>, TValue> instance)
        {
            return new KeyValuePair<TKey1, TValue>(instance.Key.Key1, instance.Value);
        } 
    }
}
