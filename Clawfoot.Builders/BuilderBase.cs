using Clawfoot.Builders.Interfaces;
using Clawfoot.Utilities.Caches;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Clawfoot.Builders
{
    public class BuilderBase<TModel> : IGenericBuilder<TModel> where TModel : new()
    {
        internal List<Action<TModel>> actions;

        internal readonly IModelDefaultValuesCache _modelDefaults;
        internal readonly IForeignKeyPropertyCache _propertyCache;

        internal BuilderBase(IModelDefaultValuesCache modelDefaults, IForeignKeyPropertyCache propertyCache)
        {
            _modelDefaults = modelDefaults is null ? throw new ArgumentNullException("modelDefaults is null") : modelDefaults;
            _propertyCache = propertyCache;
            actions = new List<Action<TModel>>();
        }

        /// <summary>
        /// Builds the model
        /// </summary>
        /// <param name="fillInForeignKeysIds">Marks if foreign key Ids should be filled in from related Objects set on this model</param>
        /// <returns></returns>
        public virtual TModel Build(bool fillInForeignKeysIds = true)
        {
            TModel built = new TModel();
            foreach (Action<TModel> action in actions)
            {
                action(built);
            }

            if (fillInForeignKeysIds)
            {
                FillInForeignKeyIds(built);
            }
            return built;
        }

        /// <summary>
        /// Builds the model using the list of actions.
        /// Note: This will override properties on the provided model with queued action changes
        /// </summary>
        /// <param name="built">The existing model you wish to apply property changes to</param>
        /// <param name="fillInForeignKeysIds">Marks if foreign key Ids should be filled in from related Objects set on this model</param>
        /// <returns></returns>
        public TModel Build(TModel built, bool fillInForeignKeysIds = true)
        {
            foreach (Action<TModel> action in actions)
            {
                action(built);
            }

            if (fillInForeignKeysIds)
            {
                FillInForeignKeyIds(built);
            }
            return built;
        }

        /// <summary>
        /// Add an action that sets the appropriate model property. Is chainable.
        /// </summary>
        /// <example>
        ///     <code>With(x => x.Name = "Name")</code>
        /// </example>         
        /// <param name="with">The action to set the property</param>
        public IGenericBuilder<TModel> With(Action<TModel> with)
        {
            actions.Add(with);
            return this;
        }

        /// <summary>
        /// Applies the default values for the model configured from <see cref="IModelDefaultValues"/>
        /// This will override any previously set values of the builder
        /// </summary>
        /// <param name="defaults">The defaults to use instead of the built-in defaults</param>
        /// <returns></returns>
        public virtual IGenericBuilder<TModel> UseDefaults(IModelDefaultValues<TModel> defaults = null)
        {
            if (defaults is null)
            {
                defaults = _modelDefaults.Get<TModel>();
            }
            actions.AddRange(defaults.Actions);
            return this;
        }

        /// <summary>
        /// Will fill in the Foreign key Ids based on the [ForeignKey] attribute from the objects set in the model
        /// </summary>
        /// <param name="model"></param>
        private void FillInForeignKeyIds(TModel model)
        {
            Type modelType = typeof(TModel);

            ModelForeignKeyProperties foreignKeyProperties = _propertyCache.GetOrAdd(modelType);

            foreach (ForeignKeyProperty fkPropertyInfo in foreignKeyProperties.GetList())
            {
                PropertyInfo property = fkPropertyInfo.Property; //Property with attribute
                PropertyInfo referenceProperty = fkPropertyInfo.ReferenceProperty;  //Property referenced in attribute

                object itemValue = property.GetValue(model);

                if (itemValue is null)
                {
                    continue;
                }

                if (typeof(ILinkToEntity).IsAssignableFrom(property.PropertyType))
                {
                    referenceProperty.SetValue(model, ((ILinkToEntity)itemValue).Id);
                }
                else if (typeof(IEntity).IsAssignableFrom(property.PropertyType))
                {
                    referenceProperty.SetValue(model, ((IEntity)itemValue).Id);
                }
                else
                {
                    throw new InvalidOperationException($"Cannot assign Foreign Key to property as the type '{property.PropertyType.Name}' does not implement ILinkToEntity or IEntity");
                }
            }
        }
    }
}
