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
    /// A collection of <see cref="ForeignKeyProperties"/> for a model
    /// </summary>
    public class ModelForeignKeyProperties
    {
        private ConcurrentDictionary<string, ForeignKeyProperty> ModelPropertiesDict { get; set; }

        private List<ForeignKeyProperty> ModelPropertiesList { get; set; }

        public ModelForeignKeyProperties(Type type)
        {
            Type = type;
            ModelPropertiesDict = new ConcurrentDictionary<string, ForeignKeyProperty>();
            ModelPropertiesList = new List<ForeignKeyProperty>();
        }

        public Type Type { get; set; }

        /// <summary>
        /// Will add the property if it does not already exist
        /// </summary>
        /// <param name="property"></param>
        /// <param name="attribute"></param>
        public bool Add(PropertyInfo property, ForeignKeyAttribute attribute, PropertyInfo foreignKeyReferenceProperty)
        {
            return ModelPropertiesDict.TryAdd(property.Name, new ForeignKeyProperty(property, attribute, foreignKeyReferenceProperty));
        }

        public List<ForeignKeyProperty> GetList()
        {
            //Caching list
            //Dictionary cannot have items deleted from it so state will be constant unless Dictionary gets longer
            if (ModelPropertiesList.Count != ModelPropertiesDict.Count)
            {
                ModelPropertiesList = ModelPropertiesDict.Select(x => x.Value).ToList();
            }

            return ModelPropertiesList;
        }
    }
}
