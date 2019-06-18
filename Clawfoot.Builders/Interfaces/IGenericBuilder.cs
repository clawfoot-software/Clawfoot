using System;
using System.Collections.Generic;
using System.Text;

namespace Clawfoot.Builders.Interfaces
{
    /// <summary>
    /// A generic builder used to build any model type
    /// </summary>
    /// <typeparam name="TModel">The model type</typeparam>
    public interface IGenericBuilder<TModel> : IBuilder<TModel> where TModel : new()
    {
        /// <summary>
        /// Add an action that sets the appropriate model property. Is chainable.
        /// </summary>
        /// <example>
        ///     <code>With(x => x.Name = "Name")</code>
        /// </example>         
        /// <param name="with">The action to set the property</param>
        IGenericBuilder<TModel> With(Action<TModel> with);

        /// <summary>
        /// Applies the default values for the model configured from <see cref="IModelDefaultValues"/>
        /// This will override any previously set values of the builder
        /// </summary>
        /// <param name="defaults">The defaults to use instead of the built-in defaults</param>
        /// <returns></returns>
        IGenericBuilder<TModel> UseDefaults(IModelDefaultValues<TModel> defaults = null);
    }
}
