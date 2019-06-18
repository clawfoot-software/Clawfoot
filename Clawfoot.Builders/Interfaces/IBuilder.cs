using System;
using System.Collections.Generic;
using System.Text;

namespace Clawfoot.Builders.Interfaces
{
    /// <summary>
    /// Base builder interface
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBuilder<T> where T : new()
    {
        /// <summary>
        /// Builds the model
        /// </summary>
        /// <param name="fillInForeignKeysIds">Marks if foreign key Ids should be filled in from related Objects set on this model</param>
        /// <returns></returns>
        T Build(bool fillInForeignKeysIds = true);

        /// <summary>
        /// Builds the model using the list of actions.
        /// Note: This will override properties on the provided model with queued action changes
        /// </summary>
        /// <param name="built">The existing model you wish to apply property changes to</param>
        /// <param name="fillInForeignKeysIds">Marks if foreign key Ids should be filled in from related Objects set on this model</param>
        /// <returns></returns>
        T Build(T built, bool fillInForeignKeysIds = true);
    }
}
