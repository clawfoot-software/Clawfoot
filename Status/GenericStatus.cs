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

        public bool Success => _errors.Count == 0;

        public bool HasErrors => _errors.Count > 0;

        public string Message
        {
            get => Success
                ? _successMessage
                : $"Failed with {_errors.Count} error" + (_errors.Count == 1 ? "" : "s");
            set => _successMessage = value;
        }

        /// <summary>
        /// Combines all error messages into a single string
        /// </summary>
        /// <param name="seperator"></param>
        /// <returns></returns>
        public string ToString(string seperator = "\n")
        {
            if (_errors.Count > 0)
            {
                return string.Join(seperator, _errors);
            }
            return string.Empty;
        }

        /// <summary>
        /// Combines all user-friendly error messages into a single string
        /// </summary>
        /// <param name="seperator"></param>
        /// <returns></returns>
        public string ToUserFriendyString(string seperator = "\n")
        {
            if (_errors.Count > 0)
            {
                return string.Join(seperator, _errors.Select(x => x.ToUserString()).ToArray());
            }
            return string.Empty;
        }

        public void CombineStatuses(IGenericStatus status)
        {
            _errors.AddRange(status.Errors);

            if (!HasErrors)
            {
                _successMessage = status.Message;
            }
        }

        protected IGenericStatus AddError(string message)
        {
            _errors.Add(new GenericError(message));
            return this;
        }

        protected IGenericStatus AddError(string message, string userMessage)
        {
            _errors.Add(new GenericError(message, userMessage));
            return this;
        }
    }
}
