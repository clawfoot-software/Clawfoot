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
        public void CombineStatuses(IGenericStatus status)
        {
            _errors.AddRange(status.Errors);

            if (!HasErrors)
            {
                _successMessage = status.Message;
            }
        }

        /// <inheritdoc/>
        public IGenericStatus AddError(string message)
        {
            _errors.Add(new GenericError(message));
            return this;
        }

        /// <inheritdoc/>
        public IGenericStatus AddError(string message, string userMessage)
        {
            _errors.Add(new GenericError(message, userMessage));
            return this;
        }
    }
}
