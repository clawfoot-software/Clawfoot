using Clawfoot.Builders.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clawfoot.Builders
{
    /// <inheritdoc/>
    public class ModelDefaultValuesCache : IModelDefaultValuesCache
    {
        private Dictionary<Type, IModelDefaultValues> Defaults { get; set; }

        public ModelDefaultValuesCache()
        {
            Defaults = new Dictionary<Type, IModelDefaultValues>();
        }

        /// <inheritdoc/>
        public void Add<T>(IModelDefaultValues<T> defaultValues) where T : new()
        {
            Defaults.Add(typeof(T), defaultValues);
        }

        /// <inheritdoc/>
        public IModelDefaultValues<T> Get<T>() where T : new()
        {
            Type type = typeof(T);
            if (Defaults.ContainsKey(type))
            {
                return (IModelDefaultValues<T>)Defaults[type];
            }
            throw new InvalidOperationException($"No defaults exist for type {type.Name}");
        }
    }
}
