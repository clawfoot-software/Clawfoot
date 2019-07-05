using System;
using System.Collections.Generic;
using System.Text;

namespace Clawfoot.Utilities.Status
{
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
    }
}
