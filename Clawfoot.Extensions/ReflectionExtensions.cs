using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Clawfoot.Extensions
{
    public static class ReflectionExtensions
    {
        /// <summary>
        /// Filters this collection by an attribute
        /// </summary>
        /// <param name="items"></param>
        /// <param name="attribute">The attribute to filter by</param>
        /// <returns></returns>
        public static IEnumerable<PropertyInfo> ExcludeAttribute(this IEnumerable<PropertyInfo> items, Type attribute)
        {
            return items.Where(x => x.GetCustomAttribute(attribute) is null).ToList();
        }
    }
}
