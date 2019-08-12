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
            EntityName = entityName;
            Items = items;
        }

        public EntityAndItemHashSetModel(string entityName, IEnumerable<T> items)
            :this(entityName, new HashSet<T>(items)) { }

        /// <summary>
        /// The name of type representing the entity
        /// </summary>
        public string EntityName { get; private set; }

        /// <summary>
        /// The items related to the entity
        /// </summary>
        public HashSet<T> Items { get; private set; }

        public override string ToString()
        {
            return EntityName;
        }
    }
}
