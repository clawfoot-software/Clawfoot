using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Text;

namespace Clawfoot.Utilities.Caches
{
    /// <summary>
    /// Used in the <see cref="ForeignKeyPropertyCache"/> to cache <see cref="ForeignKeyAttribute"/> information to avoid constant use of reflection
    /// </summary>
    public class ForeignKeyProperty
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="property">The property that has the [ForeignKey()] attribute</param>
        /// <param name="attribute">The attribute</param>
        /// <param name="referenceProperty">The property referenced in the ForeignKey attributes name</param>
        public ForeignKeyProperty(PropertyInfo property, ForeignKeyAttribute attribute, PropertyInfo referenceProperty)
        {
            Property = property;
            Attribute = attribute;
            ReferenceProperty = referenceProperty;
        }

        /// <summary>
        /// The property that has the [ForeignKey()] attribute
        /// </summary>
        public PropertyInfo Property { get; }

        /// <summary>
        /// The <see cref="ForeignKeyAttribute"/> attached to this property
        /// </summary>
        public ForeignKeyAttribute Attribute { get; }

        /// <summary>
        /// The property referenced in the ForeignKey attributes name
        /// </summary>
        public PropertyInfo ReferenceProperty { get; }
    }
}
