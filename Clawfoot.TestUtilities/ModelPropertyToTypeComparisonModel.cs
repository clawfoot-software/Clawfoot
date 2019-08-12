using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Clawfoot.TestUtilities
{
    /// <summary>
    /// This is intended to hold the data necessary to check a single model
    /// and one of it's properties against a class that should have matching properties
    /// </summary>
    public class ModelPropertyToTypeComparisonModel : XUnitSerializable
    {
        public ModelPropertyToTypeComparisonModel() { }

        public ModelPropertyToTypeComparisonModel(Type model, PropertyInfo property, Type comparate)
        {
            Model = model;
            Property = property;
            Comparate = comparate;
        }

        /// <summary>
        /// The Model that is being verified
        /// </summary>
        public Type Model { get; private set; }

        /// <summary>
        /// The Property of that model that is being checked
        /// </summary>
        public PropertyInfo Property { get; private set; }

        /// <summary>
        /// The comparate for this model, what the model property is being compared to
        /// </summary>
        public Type Comparate { get; private set; }

        public string ModelName => Model.Name; 
        public string PropertyName => Property.Name;

        public override string ToString()
        {
            return $"{ModelName}: {PropertyName}";
        }
    }
}
