using Clawfoot.Builders.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clawfoot.Builders
{
    public abstract class ModelDefaultValues
    {

    }

    public class ModelDefaultValues<TModel> : ModelDefaultValues, IModelDefaultValues<TModel> where TModel : new()
    {
        private List<Action<TModel>> _actions { get; set; }

        public ModelDefaultValues()
        {
            _actions = new List<Action<TModel>>();
        }

        /// <inheritdoc/>
        public IList<Action<TModel>> Actions { get => _actions; }

        /// <inheritdoc/>
        public IModelDefaultValues<TModel> Add(Action<TModel> action)
        {
            _actions.Add(action);
            return this;
        }

        /// <inheritdoc/>
        public IModelDefaultValues<TModel> Add(params Action<TModel>[] actions)
        {
            this._actions.AddRange(actions);
            return this;
        }
    }
}
