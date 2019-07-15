using Clawfoot.Core.Status;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clawfoot.Utilities.Status
{
    /// <summary>
    /// A generic version of a generic status
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GenericStatus<T> : GenericStatus, IGenericStatus<T>
    {
        private T _result;

        /// <summary>
        /// The returned result
        /// </summary>
        public T Result
        {
            get => HasErrors ? default(T) : _result;
            set => _result = value;
        }

        /// <summary>
        /// Sets the result of the status
        /// </summary>
        /// <param name="result"></param>
        public void SetResult(T result)
        {
            Result = result;
        }
    }
}
