using AutoMapper;
using Clawfoot.Builders.Interfaces;
using Clawfoot.Utilities.Caches;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clawfoot.Builders
{
    public class ModelEntityBuilder<TModel, TEntity> : Builder<TModel>, IModelEntityBuilder<TModel, TEntity> where TEntity : IEntity, new() where TModel : new()
    {
        private readonly IMapper _mapper;
        public ModelEntityBuilder() : base() { }

        public ModelEntityBuilder(IModelDefaultValuesCache modelDefaults, IForeignKeyPropertyCache propertyCache, IMapper mapper)
            : base(modelDefaults, propertyCache) 
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }


        public IGenericBuilder<TEntity> EntityBuilder => new BuilderBase<TEntity>(_modelDefaults, _propertyCache);

        /// <summary>
        /// Builds the matching Entity from the DTO
        /// Requires automapper configurations for the provided types
        /// </summary>
        /// <param name="fillInForeignKeysIds"></param>
        /// <returns></returns>
        public virtual TEntity BuildAsEntity(bool fillInForeignKeysIds = true)
        {
            return _mapper.Map<TEntity>(Build());
        }

        /// <summary>
        /// Add an action that sets the appropriate model property. Is chainable.
        /// </summary>
        /// <example>
        ///     <code>With(x => x.Name = "Name")</code>
        /// </example>         
        /// <param name="with">The action to set the property</param>
        public new IModelEntityBuilder<TModel, TEntity> With(Action<TModel> with)
        {
            base.With(with);
            return this;
        }

        /// <summary>
        /// Applies the default values for the model configured from <see cref="IModelDefaultValues"/>
        /// This will override any previously set values of the builder
        /// </summary>
        /// <param name="defaults">The defaults to use instead of the built-in defaults</param>
        /// <returns></returns>
        public new IModelEntityBuilder<TModel, TEntity> UseDefaults(IModelDefaultValues<TModel> defaults = null)
        {
            base.UseDefaults(defaults);
            return this;
        }


    }
}
