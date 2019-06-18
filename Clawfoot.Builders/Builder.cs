using Clawfoot.Builders.Interfaces;
using Clawfoot.Utilities.Caches;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clawfoot.Builders
{
    public class Builder<TModel> : BuilderBase<TModel>, IGenericBuilder<TModel> where TModel : new()
    {
        public Builder()
            :this(new ModelDefaultValuesCache(), new ForeignKeyPropertyCache()){ }

        public Builder(IModelDefaultValuesCache modelDefaults, IForeignKeyPropertyCache propertyCache)
            :base(modelDefaults, propertyCache)
        { }
    }
}
