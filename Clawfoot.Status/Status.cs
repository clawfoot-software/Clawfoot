using Clawfoot.Status.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Clawfoot.Status
{
    public class Status : IStatus
    {
        internal const string DefaultSuccessMessage = "Success";
        private protected readonly List<IError> _errors = new List<IError>();
        private protected readonly List<Exception> _exceptions = new List<Exception>();
        private protected string _successMessage = DefaultSuccessMessage;

        /// <summary>
        /// Create a generic status
        /// </summary>
        public Status() { }

        /// <summary>
        /// Create a generic status
        /// </summary>
        /// <param name="successMessage">The default success message</param>
        public Status(string successMessage)
        {
            if (!String.IsNullOrWhiteSpace(successMessage))
            {
                _successMessage = successMessage;
            }
        }

        /// <summary>
        /// Helper method that creates a <see cref="Status"/> with no errors
        /// </summary>
        /// <param name="successMessage">The default success message</param>
        /// <returns></returns>
        public static IStatus AsSuccess(string successMessage = null)
        {
            return new Status(successMessage);
        }

        /// <summary>
        /// Helper method that creates a <see cref="Status{T}"/> with a sucess message and a result
        /// </summary>
        /// <param name="result">The result of this generic</param>
        /// <param name="successMessage">The default success message</param>
        /// <returns></returns>
        public static IStatus<TResult> AsSuccess<TResult>(TResult result, string successMessage = null)
        {
            return new Status<TResult>(result, successMessage);
        }

        /// <summary>
        /// Helper method that creates a <see cref="Status"/> with an error message
        /// </summary>
        /// <param name="message">The error message</param>
        /// <param name="userMessage">The user friendly error message</param>
        /// <returns></returns>
        public static IStatus AsError(string message, string userMessage = "")
        {
            Status status = new Status();
            status.AddError(message, userMessage);
            return status;
        }

        /// <summary>
        /// Helper method that creates a <see cref="Status{T}"/> with an error message
        /// </summary>
        /// <param name="message">The error message</param>
        /// <param name="userMessage">The user friendly error message</param>
        /// <returns></returns>
        public static IStatus<TResult> AsError<TResult>(string message, string userMessage = "")
        {
            Status<TResult> status = new Status<TResult>();
            status.AddError(message, userMessage);
            return status;
        }

        /// <summary>
        /// Helper method that creates a <see cref="Status"/> with the provided exception
        /// </summary>
        /// <param name="ex">The exception</param>
        /// <returns></returns>
        public static IStatus AsErrorWithException(Exception ex)
        {
            Status status = new Status();
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
        public static IStatus InvokeAndReturnStatus(Action action, bool keepException = false)
        {
            IStatus status = new Status();

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
        public static IStatus InvokeAndReturnStatus<TParam>(Action<TParam> action, TParam obj, bool keepException = false)
        {
            IStatus status = new Status();

            return status.Invoke(action, obj, keepException);

        }

        /// <summary>
        /// Invokes the delegate, and if it throws an exception, records it in a new Status.
        /// If success, return the result of the delegate as a new Status
        /// </summary>
        /// <param name="func">The delegate</param>
        /// <param name="keepException">To keep the exception in the stus, or just record the error message</param>
        /// <returns></returns>
        public static IStatus<TResult> InvokeAndReturnStatusResult<TResult>(Func<TResult> func, bool keepException = false)
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
                    return Status.AsError<TResult>(ex.Message);
                }

                Status<TResult> status = new Status<TResult>();
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
        public static IStatus InvokeAndReturnMergedStatus(Func<IStatus> func, bool keepException = false)
        {
            IStatus status = new Status();
            status.InvokeAndMergeStatus(func, keepException);

            return status;
        }

        /// <inheritdoc/>
        public IEnumerable<IError> Errors => _errors.AsEnumerable();

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
        public IStatus<T> AsGeneric<T>()
        {
            IStatus<T> status = new Status<T>();
            status.MergeStatuses(this);
            return status;
        }

        /// <inheritdoc/>
        public IStatus<T> SetResult<T>(T result)
        {
            IStatus<T> status = new Status<T>();
            status.MergeStatuses(this);
            status.SetResult(result);
            return status;
        }

        /// <inheritdoc/>
        public IStatus MergeStatuses(IStatus status)
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
        public IStatus MergeIntoStatus(IStatus status)
        {
            return status.MergeStatuses(this);
        }

        /// <inheritdoc/>
        public IStatus AddException(Exception ex)
        {
            _exceptions.Add(ex);
            AddError(ex.Message);
            return this;
        }

        /// <inheritdoc/>
        public IStatus AddError(string message, string userMessage = "")
        {
            _errors.Add(new Error(message, userMessage));
            return this;
        }

        /// <inheritdoc/>
        public IStatus AddError(IError error)
        {
            _errors.Add(error);
            return this;
        }

        /// <inheritdoc/>
        public IStatus AddErrorIfNull<T>(T value, string message, string userMessage = "") where T : class
        {
            if (value is null)
            {
                return AddError(message, userMessage);
            }
            return this;
        }

        /// <inheritdoc/>
        public IStatus AddErrorIfNull<T>(T? value, string message, string userMessage = "") where T : struct
        {
            if (value is null)
            {
                return AddError(message, userMessage);
            }
            return this;
        }

        /// <inheritdoc/>
        public IStatus AddErrorIfNullOrDefault<T>(T value, string message, string userMessage = "") where T : class
        {
            if (value is null || value == default(T))
            {
                return AddError(message, userMessage);
            }
            return this;
        }

        /// <inheritdoc/>
        public IStatus AddErrorIfNullOrDefault<T>(T? value, string message, string userMessage = "") where T : struct
        {
            if (value is null || value.Value.Equals(default(T)))
            {
                return AddError(message, userMessage);
            }
            return this;
        }

        /// <inheritdoc/>
        public IStatus Invoke(Action action, bool keepException = false)
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
        public IStatus Invoke<TParam>(Action<TParam> action, TParam obj, bool keepException = false)
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
        public IStatus InvokeAndMergeStatus(Func<IStatus> func, bool keepException = false)
        {
            try
            {
                IStatus resultStatus = func.Invoke();
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
        public TResult InvokeMergeStatusAndReturnResult<TResult>(Func<IStatus<TResult>> func, bool keepException = false)
        {
            try
            {
                IStatus<TResult> resultStatus = func.Invoke();
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
