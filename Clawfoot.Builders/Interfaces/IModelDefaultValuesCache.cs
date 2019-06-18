using System;
using System.Collections.Generic;
using System.Text;

namespace Clawfoot.Builders.Interfaces
{
    /// <summary>
    /// The cache of default values for each model
    /// </summary>
    public interface IModelDefaultValuesCache
    {
        /// <summary>
        /// Adds the default values for a model to the cache
        /// </summary>
        /// <typeparam name="T">The type of the model</typeparam>
        /// <param name="defaultValues"></param>
        void Add<T>(IModelDefaultValues<T> defaultValues) where T : new();

        /// <summary>
        /// Gets the defaults for the model from the cache
        /// </summary>
        /// <typeparam name="T">The type of the model</typeparam>
        /// <exception cref="InvalidOperationException"></exception>
        IModelDefaultValues<T> Get<T>() where T : new();
    }
}
