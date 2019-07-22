﻿using Clawfoot.Core.Status;
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
        private readonly List<IGenericError> _errors = new List<IGenericError>();
        private readonly List<Exception> _exceptions = new List<Exception>();
        private string _successMessage = DefaultSuccessMessage;

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
        public static IGenericStatus CreateWithException(Exception ex)
        {
            GenericStatus status = new GenericStatus();
            status.AddException(ex);
            return status;
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
                return CreateAsSuccess<TResult>(result);
            }
            catch (Exception ex)
            {
                if (!keepException)
                {
                    return CreateWithError<TResult>(ex.Message);
                }

                GenericStatus<TResult> status = new GenericStatus<TResult>();
                status.AddException(ex);
                return status;
            }
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
        public string ToUserFriendyString(string seperator = "\n")
        {
            if (_errors.Count > 0)
            {
                return string.Join(seperator, _errors.Select(x => x.ToUserString()).ToArray());
            }
            return string.Empty;
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
