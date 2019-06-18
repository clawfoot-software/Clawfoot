using System;
using System.Collections.Generic;
using System.Text;

namespace Clawfoot.Builders.Interfaces
{
    /// <summary>
    /// Marker interface used for upcasting from a generic method
    /// </summary>
    public interface IModelDefaultValues
    {

    }

    /// <summary>
    /// A set of default values for a Model type.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public interface IModelDefaultValues<TModel> : IModelDefaultValues
    {
        /// <summary>
        /// The array of default value setting actions
        /// </summary>
        IList<Action<TModel>> Actions { get; }

        /// <summary>
        /// Adds a default value setter. Can be chained.
        /// </summary>
        /// <example>
        ///     <code>Add(x => x.Name = "Name")</code>
        /// </example> 
        IModelDefaultValues<TModel> Add(Action<TModel> action);

        /// <summary>
        /// Adds an array of default value setters
        /// </summary>
        /// <param name="actions"></param>
        IModelDefaultValues<TModel> Add(params Action<TModel>[] actions);
    }
}
