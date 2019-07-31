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
        /// Usaed like myInstance.GetMemberName(x => nameof(x.MyMember))
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="nameofMethod"></param>
        /// <returns></returns>
        public static string GetMemberName<T>(this T instance, Func<T, string> nameofMethod) where T : class
        {
            return nameofMethod(instance);
        }
    }
}
