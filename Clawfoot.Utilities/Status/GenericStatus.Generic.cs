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
        public bool HasResult
        {
            get
            {
                if (HasErrors) return false;
                if(EqualityComparer<T>.Default.Equals(_result, default(T))) return false;

                return true;
            }
        }

        /// <inheritdoc/>
        public IGenericStatus<T> SetResult(T result)
        {
            Result = result;
            return this;
        }

        /// <inheritdoc/>
        public IGenericStatus<T> MergeStatuses(IGenericStatus<T> status)
        {
            _errors.AddRange(status.Errors);
            _exceptions.AddRange(status.Exceptions);

            if (!this.HasErrors)
            {
                _successMessage = status.Message;
            }

            // If this doesn't have a result, and status does, keep the provided result
            // THis also implicitly means that an existing result on this status is maintained either way
            if(!this.HasResult && status.HasResult)
            {
                this.SetResult(status.Result);
            }
            
            return this;
        }

        /// <inheritdoc/>
        public IGenericStatus<T> MergeIntoStatus(IGenericStatus<T> status)
        {
            return status.MergeStatuses(this);
        }

        /// <inheritdoc/>
        public T MergeIntoStatusAndReturnResult(IGenericStatus status)
        {
            status.MergeStatuses(this);
            return this.Result;
        }

        /// <inheritdoc/>
        public T MergeIntoStatusAndReturnResult(IGenericStatus<T> status)
        {
            status.MergeStatuses(this);
            return this.Result;
        }

        /// <inheritdoc/>
        public IGenericStatus<TResult> ConvertTo<TResult>(TResult result)
        {
            IGenericStatus<TResult> status = new GenericStatus<TResult>();
            status.SetResult(result);

            status.MergeStatuses(this);
            return status;

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
