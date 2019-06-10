using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Clawfoot.Utilities.Caches
{
    /// <summary>
    /// The cache of <see cref="ModelForeignKeyProperties"/>
    /// </summary>
    public class ForeignKeyPropertyCache
    {
        private ConcurrentDictionary<Type, ModelForeignKeyProperties> Cache { get; set; }

        internal ForeignKeyPropertyCache()
        {
            Cache = new ConcurrentDictionary<Type, ModelForeignKeyProperties>();
        }

        /// <summary>
        /// Gets the ModelForeignKeyProperties for the Type.
        /// If it does not exist, it is created, added to the cache, and returned
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal ModelForeignKeyProperties GetOrAdd(Type type)
        {
            if (!Contains(type))
            {
                ModelForeignKeyProperties modelFkProperties = new ModelForeignKeyProperties(type);

                Dictionary<string, PropertyInfo> properties = type.GetProperties().ToDictionary(x => x.Name);

                foreach (KeyValuePair<string, PropertyInfo> propertyElement in properties)
                {
                    PropertyInfo property = propertyElement.Value;
                    ForeignKeyAttribute attribute = (ForeignKeyAttribute)property.GetCustomAttribute(typeof(ForeignKeyAttribute));
                    if (attribute is null)
                    {
                        continue;
                    }

                    string attributeValue = attribute.Name;

                    if (!properties.ContainsKey(attribute.Name))
                    {
                        throw new Exception($"ForeignKey Attribute value ({attribute.Name}) on Property ({property.Name}) in Model ({type.Name}) is invalid");
                    }

                    PropertyInfo fKReferenceProperty = type.GetProperty(attributeValue);

                    modelFkProperties.Add(property, attribute, fKReferenceProperty);
                }
                Cache.TryAdd(type, modelFkProperties);
            }

            return Cache[type];
        }

        /// <summary>
        /// If the cache contains a ModelForeignKeyProperties for the specified Type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal bool Contains(Type type)
        {
            return Cache.ContainsKey(type);
        }
    }
}
