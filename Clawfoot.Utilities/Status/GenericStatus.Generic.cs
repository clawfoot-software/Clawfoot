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


        public GenericStatus(){}

        /// <summary>
        /// Creates a <see cref="GenericStatus{T}"/> with the result and sucess message
        /// </summary>
        /// <param name="result"></param>
        /// <param name="successMessage"></param>
        public GenericStatus(T result, string successMessage = null)
            :base(successMessage)
        {
            _result = result;
        }

        /// <summary>
        /// The returned result
        /// </summary>
        public T Result
        {
            get => HasErrors ? default(T) : _result;
            set => _result = value;
        }

        /// <inheritdoc/>
        public IGenericStatus<T> SetResult(T result)
        {
            Result = result;
            return this;
        }

        /// <inheritdoc/>
        public T InvokeAndSetResult(Func<T> func, bool keepException = false)
        {
            try
            {
                T result = func.Invoke();
                SetResult(result);
                return result;
            }
            catch (Exception ex)
            {
                if (!keepException)
                {
                    AddError(ex.Message);
                }
                else
                {
                    AddException(ex);
                }
            }

            return default(T);
        }
    }
}
