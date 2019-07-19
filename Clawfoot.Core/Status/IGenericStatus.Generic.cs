using System;
using System.Collections.Generic;
using System.Text;

namespace Clawfoot.Core.Status
{
    public interface IGenericStatus<T> : IGenericStatus
    {
        /// <summary>
        /// The return result, or if there are errors it will return default(T)
        /// </summary>
        T Result { get; set; }

        /// <summary>
        /// Sets the result of the status
        /// </summary>
        /// <param name="result"></param>
        void SetResult(T result);
    }
}
