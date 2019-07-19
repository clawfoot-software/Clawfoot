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
        /// Helper method that creates a <see cref="GenericStatus{T}"/> with a sucess message and a result
        /// </summary>
        /// <param name="result">The result of this generic</param>
        /// <param name="successMessage">The default success message</param>
        /// <returns></returns>
        public static IGenericStatus<T> CreateAsSuccess(T result, string successMessage = null)
        {
            return new GenericStatus<T>(result, successMessage);
        }

        /// <summary>
        /// Helper method that creates a <see cref="GenericStatus{T}"/> with an error message
        /// </summary>
        /// <param name="message">The error message</param>
        /// <param name="userMessage">The user friendly error message</param>
        /// <returns></returns>
        new public static IGenericStatus<T> CreateWithError(string message, string userMessage = "")
        {
            GenericStatus<T> status = new GenericStatus<T>();
            status.AddError(message, userMessage);
            return status;
        }

        /// <summary>
        /// Invokes the delegate, and if it throws an exception, records it in a new GenericStatus.
        /// If success, return the result of the delegate as a new GenericStatus
        /// </summary>
        /// <param name="func">The delegate</param>
        /// <param name="keepException">To keep the exception in the stus, or just record the error message</param>
        /// <returns></returns>
        public static IGenericStatus<T> InvokeAndReturnStatusResult<T>(Func<T> func, bool keepException = false)
        {
            try
            {
                T result = func.Invoke();
                return GenericStatus<T>.CreateAsSuccess(result);
            }
            catch (Exception ex)
            {
                if (!keepException)
                {
                    return GenericStatus<T>.CreateWithError(ex.Message);
                }

                GenericStatus<T> status = new GenericStatus<T>();
                status.AddException(ex);
                return status;
            }
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
        public void SetResult(T result)
        {
            Result = result;
        }
    }
}
