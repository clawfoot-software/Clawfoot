using Clawfoot.Core.Status;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace Clawfoot.Utilities.Status
{
    public class GenericStatus : IGenericStatus
    {
        internal const string DefaultSuccessMessage = "Success";
        private protected readonly List<IGenericError> _errors = new List<IGenericError>();
        private protected readonly List<Exception> _exceptions = new List<Exception>();
        private protected string _successMessage = DefaultSuccessMessage;

        /// <summary>
        /// Create a generic status
        /// </summary>
        public GenericStatus() { }

        /// <summary>
        /// Create a generic status
        /// </summary>
        /// <param name="successMessage">The default success message</param>
        public GenericStatus(string successMessage)
        {
            if (!String.IsNullOrWhiteSpace(successMessage))
            {
                _successMessage = successMessage;
            }
        }

        /// <summary>
        /// Helper method that creates a <see cref="GenericStatus"/> with no errors
        /// </summary>
        /// <param name="successMessage">The default success message</param>
        /// <returns></returns>
        public static IGenericStatus AsSuccess(string successMessage = null)
        {
            return new GenericStatus(successMessage);
        }

        /// <summary>
        /// Helper method that creates a <see cref="GenericStatus{T}"/> with a sucess message and a result
        /// </summary>
        /// <param name="result">The result of this generic</param>
        /// <param name="successMessage">The default success message</param>
        /// <returns></returns>
        public static IGenericStatus<TResult> AsSuccess<TResult>(TResult result, string successMessage = null)
        {
            return new GenericStatus<TResult>(result, successMessage);
        }

        /// <summary>
        /// Helper method that creates a <see cref="GenericStatus"/> with an error message
        /// </summary>
        /// <param name="message">The error message</param>
        /// <param name="userMessage">The user friendly error message</param>
        /// <returns></returns>
        public static IGenericStatus AsError(string message, string userMessage = "")
        {
            GenericStatus status = new GenericStatus();
            status.AddError(message, userMessage);
            return status;
        }

        /// <summary>
        /// Helper method that creates a <see cref="GenericStatus{T}"/> with an error message
        /// </summary>
        /// <param name="message">The error message</param>
        /// <param name="userMessage">The user friendly error message</param>
        /// <returns></returns>
        public static IGenericStatus<TResult> AsError<TResult>(string message, string userMessage = "")
        {
            GenericStatus<TResult> status = new GenericStatus<TResult>();
            status.AddError(message, userMessage);
            return status;
        }

        /// <summary>
        /// Helper method that creates a <see cref="GenericStatus"/> with the provided exception
        /// </summary>
        /// <param name="ex">The exception</param>
        /// <returns></returns>
        public static IGenericStatus AsErrorWithException(Exception ex)
        {
            GenericStatus status = new GenericStatus();
            status.AddException(ex);
            return status;
        }

        /// <summary>
        /// Helper method that creates a <see cref="GenericStatus"/> with no errors
        /// </summary>
        /// <param name="successMessage">The default success message</param>
        /// <returns></returns>
        [Obsolete("Use AsSuccess instead")]
        public static IGenericStatus CreateAsSuccess(string successMessage = null)
        {
            return new GenericStatus(successMessage);
        }

        /// <summary>
        /// Helper method that creates a <see cref="GenericStatus{T}"/> with a sucess message and a result
        /// </summary>
        /// <param name="result">The result of this generic</param>
        /// <param name="successMessage">The default success message</param>
        /// <returns></returns>
        [Obsolete("Use AsSuccess instead")]
        public static IGenericStatus<TResult> CreateAsSuccess<TResult>(TResult result, string successMessage = null)
        {
            return new GenericStatus<TResult>(result, successMessage);
        }

        /// <summary>
        /// Helper method that creates a <see cref="GenericStatus"/> with an error message
        /// </summary>
        /// <param name="message">The error message</param>
        /// <param name="userMessage">The user friendly error message</param>
        /// <returns></returns>
        [Obsolete("Use AsError instead")]
        public static IGenericStatus CreateWithError(string message, string userMessage = "")
        {
            GenericStatus status = new GenericStatus();
            status.AddError(message, userMessage);
            return status;
        }

        /// <summary>
        /// Helper method that creates a <see cref="GenericStatus{T}"/> with an error message
        /// </summary>
        /// <param name="message">The error message</param>
        /// <param name="userMessage">The user friendly error message</param>
        /// <returns></returns>
        [Obsolete("Use AsError instead")]
        public static IGenericStatus<TResult> CreateWithError<TResult>(string message, string userMessage = "")
        {
            GenericStatus<TResult> status = new GenericStatus<TResult>();
            status.AddError(message, userMessage);
            return status;
        }

        /// <summary>
        /// Helper method that creates a <see cref="GenericStatus"/> with the provided exception
        /// </summary>
        /// <param name="ex">The exception</param>
        /// <returns></returns>
        [Obsolete("Use AsErrorWithException instead")]
        public static IGenericStatus CreateWithException(Exception ex)
        {
            GenericStatus status = new GenericStatus();
            status.AddException(ex);
            return status;
        }

        /// <summary>
        /// Helper method that invokes the delegate, and if it throws an exception, records it in a returned status
        /// Returns a new status
        /// </summary>
        /// <param name="action"></param>
        /// <param name="keepException"></param>
        /// <returns></returns>
        public static IGenericStatus InvokeAndReturnStatus(Action action, bool keepException = false)
        {
            IGenericStatus status = new GenericStatus();

            return status.Invoke(action, keepException);

        }

        /// <summary>
        /// Helper method that invokes the delegate, and if it throws an exception, records it in a returned status
        /// Returns a new status
        /// </summary>
        /// <param name="action"></param>
        /// <param name="keepException"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static IGenericStatus InvokeAndReturnStatus<TParam>(Action<TParam> action, TParam obj, bool keepException = false)
        {
            IGenericStatus status = new GenericStatus();

            return status.Invoke(action, obj, keepException);

        }

        /// <summary>
        /// Invokes the delegate, and if it throws an exception, records it in a new GenericStatus.
        /// If success, return the result of the delegate as a new GenericStatus
        /// </summary>
        /// <param name="func">The delegate</param>
        /// <param name="keepException">To keep the exception in the stus, or just record the error message</param>
        /// <returns></returns>
        public static IGenericStatus<TResult> InvokeAndReturnStatusResult<TResult>(Func<TResult> func, bool keepException = false)
        {
            try
            {
                TResult result = func.Invoke();
                return AsSuccess<TResult>(result);
            }
            catch (Exception ex)
            {
                if (!keepException)
                {
                    return GenericStatus.AsError<TResult>(ex.Message);
                }

                GenericStatus<TResult> status = new GenericStatus<TResult>();
                status.AddException(ex);
                return status;
            }
        }

        /// <summary>
        /// Invokes the delegate, and returns a merges status based on the return or the failure of the delegate
        /// </summary>
        /// <param name="func"></param>
        /// <param name="keepException"></param>
        /// <returns></returns>
        public static IGenericStatus InvokeAndReturnMergedStatus(Func<IGenericStatus> func, bool keepException = false)
        {
            IGenericStatus status = new GenericStatus();
            status.InvokeAndMergeStatus(func, keepException);

            return status;
        }

        /// <inheritdoc/>
        public IEnumerable<IGenericError> Errors => _errors.AsEnumerable();

        /// <inheritdoc/>
        public IEnumerable<Exception> Exceptions => _exceptions.AsEnumerable();

        /// <inheritdoc/>
        public bool Success => _errors.Count == 0;

        /// <inheritdoc/>
        public bool HasErrors => _errors.Count > 0;

        /// <inheritdoc/>
        public bool HasExceptions => _exceptions.Count > 0;

        /// <inheritdoc/>
        public string Message
        {
            get => Success
                ? _successMessage
                : $"Failed with {_errors.Count} error(s)"; 
            set => _successMessage = value;
        }

        /// <inheritdoc/>
        public string ToString(string seperator = "\n")
        {
            if (_errors.Count > 0)
            {
                return string.Join(seperator, _errors);
            }
            return string.Empty;
        }

        /// <inheritdoc/>
        public string ToUserFriendlyString(string seperator = "\n")
        {
            if (_errors.Count > 0)
            {
                return string.Join(seperator, _errors.Select(x => x.ToUserString()).ToArray());
            }
            return string.Empty;
        }

        /// <inheritdoc/>
        public IGenericStatus<T> AsGeneric<T>()
        {
            IGenericStatus<T> status = new GenericStatus<T>();
            status.MergeStatuses(this);
            return status;
        }

        /// <inheritdoc/>
        public IGenericStatus<T> SetResult<T>(T result)
        {
            IGenericStatus<T> status = new GenericStatus<T>();
            status.MergeStatuses(this);
            status.SetResult(result);
            return status;
        }

        /// <inheritdoc/>
        public IGenericStatus MergeStatuses(IGenericStatus status)
        {
            _errors.AddRange(status.Errors);
            _exceptions.AddRange(status.Exceptions);

            if (!HasErrors)
            {
                _successMessage = status.Message;
            }

            return this;
        }

        /// <inheritdoc/>
        public IGenericStatus MergeIntoStatus(IGenericStatus status)
        {
            return status.MergeStatuses(this);
        }

        /// <inheritdoc/>
        public IGenericStatus AddException(Exception ex)
        {
            _exceptions.Add(ex);
            AddError(ex.Message);
            return this;
        }

        /// <inheritdoc/>
        public IGenericStatus AddError(string message, string userMessage = "")
        {
            _errors.Add(new GenericError(message, userMessage));
            return this;
        }

        /// <inheritdoc/>
        public IGenericStatus AddErrorIfNull<T>(T value, string message, string userMessage = "") where T : class
        {
            if (value is null)
            {
                return AddError(message, userMessage);
            }
            return this;
        }

        /// <inheritdoc/>
        public IGenericStatus AddErrorIfNull<T>(T? value, string message, string userMessage = "") where T : struct
        {
            if (value is null)
            {
                return AddError(message, userMessage);
            }
            return this;
        }

        /// <inheritdoc/>
        public IGenericStatus AddErrorIfNullOrDefault<T>(T value, string message, string userMessage = "") where T : class
        {
            if (value is null || value == default(T))
            {
                return AddError(message, userMessage);
            }
            return this;
        }

        /// <inheritdoc/>
        public IGenericStatus AddErrorIfNullOrDefault<T>(T? value, string message, string userMessage = "") where T : struct
        {
            if (value is null || value.Value.Equals(default(T)))
            {
                return AddError(message, userMessage);
            }
            return this;
        }

        /// <inheritdoc/>
        public IGenericStatus Invoke(Action action, bool keepException = false)
        {
            try
            {
                action.Invoke();
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

            return this;
        }

        /// <inheritdoc/>
        public IGenericStatus Invoke<TParam>(Action<TParam> action, TParam obj, bool keepException = false)
        {
            try
            {
                action.Invoke(obj);
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

            return this;
        }

        /// <inheritdoc/>
        public IGenericStatus InvokeAndMergeStatus(Func<IGenericStatus> func, bool keepException = false)
        {
            try
            {
                IGenericStatus resultStatus = func.Invoke();
                resultStatus.MergeIntoStatus(this);
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

            return this;
        }

        /// <inheritdoc/>
        public TResult InvokeMergeStatusAndReturnResult<TResult>(Func<IGenericStatus<TResult>> func, bool keepException = false)
        {
            try
            {
                IGenericStatus<TResult> resultStatus = func.Invoke();
                resultStatus.MergeIntoStatus(this); //Merge errors into this
                return resultStatus.Result;
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

            return default(TResult);
        }

        /// <inheritdoc/>
        public TResult InvokeAndReturnResult<TResult>(Func<TResult> func, bool keepException = false)
        {
            try
            {
                TResult result = func.Invoke();
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

            return default(TResult);
        }
    }
}
