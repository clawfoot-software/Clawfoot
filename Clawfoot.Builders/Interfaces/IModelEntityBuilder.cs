using System;
using System.Collections.Generic;
using System.Text;

namespace Clawfoot.Builders.Interfaces
{
    /// <summary>
    /// Allows for the building of an entity using the normal builder for a model
    /// </summary>
    /// <typeparam name="TModel">The model this builder represents</typeparam>
    /// <typeparam name="TEntity">The entity this builder can convert the model to</typeparam>
    public interface IModelEntityBuilder<TModel, TEntity> : IGenericBuilder<TModel> where TEntity : IEntity, new() where TModel : new()
    {
        /// <summary>
        /// Creates a builder specifically for the entity type
        /// This is handy when you need to use existing entities as properties for the builder instead of DTOs
        /// </summary>
        IGenericBuilder<TEntity> EntityBuilder { get; }

        /// <summary>
        /// Builds the matching Entity from the DTO
        /// Requires automapper configurations for the provided types
        /// </summary>
        /// <param name="fillInForeignKeysIds"></param>
        /// <returns></returns>
        TEntity BuildAsEntity(bool fillInForeignKeysIds = true);

        new IModelEntityBuilder<TModel, TEntity> With(Action<TModel> with);
        new IModelEntityBuilder<TModel, TEntity> UseDefaults(IModelDefaultValues<TModel> defaults = null);
    }
}
