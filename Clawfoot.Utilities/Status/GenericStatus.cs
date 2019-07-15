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
        private readonly List<IGenericError> _errors = new List<IGenericError>();
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
        /// Helper method that creates a <see cref="GenericStatus"/> with an error message
        /// </summary>
        /// <param name="message">The error message</param>
        /// <returns></returns>
        public static IGenericStatus CreateWithError(string message)
        {
            GenericStatus status = new GenericStatus();
            status.AddError(message);
            return status;
        }

        /// <summary>
        /// Helper method that creates a <see cref="GenericStatus"/> with an error message
        /// </summary>
        /// <param name="message">The error message</param>
        /// <param name="userMessage">The user friendly error message</param>
        /// <returns></returns>
        public static IGenericStatus CreateWithError(string message, string userMessage)
        {
            GenericStatus status = new GenericStatus();
            status.AddError(message, userMessage);
            return status;
        }

        /// <inheritdoc/>
        public IImmutableList<IGenericError> Errors => _errors.ToImmutableList();

        /// <inheritdoc/>
        public bool Success => _errors.Count == 0;

        /// <inheritdoc/>
        public bool HasErrors => _errors.Count > 0;

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
        public void MergeStatuses(IGenericStatus status)
        {
            _errors.AddRange(status.Errors);

            if (!HasErrors)
            {
                _successMessage = status.Message;
            }
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
    }
}
