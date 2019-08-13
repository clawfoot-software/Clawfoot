using System;
using System.Collections.Generic;
using System.Text;
using Xunit.Abstractions;

namespace Clawfoot.TestUtilities
{
    /// <summary>
    /// An XUnitSerializeable class used to provide
    /// the name of an Entity type, and a HashSet of related items
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EntityAndItemHashSetModel<T> : XUnitSerializable
    {
        public EntityAndItemHashSetModel() { }

        public EntityAndItemHashSetModel(string entityName, HashSet<T> items)
        {
            EntityName = entityName ?? throw new ArgumentNullException(nameof(entityName), "entityName cannot be null");
            Items = items ?? throw new ArgumentNullException(nameof(items),"Items hashset cannot be null");          
        } 

        /// <summary>
        /// The name of type representing the entity
        /// </summary>
        public string EntityName { get; set; }

        /// <summary>
        /// The items related to the entity
        /// </summary>
        public HashSet<T> Items { get; set; }

        public override string ToString()
        {
            return EntityName;
        }
    }
}
