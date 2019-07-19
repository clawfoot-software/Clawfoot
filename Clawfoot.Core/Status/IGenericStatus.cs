using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Clawfoot.Core.Status
{
    public interface IGenericStatus
    {
        /// <summary>
        /// The list of errors of this status
        /// </summary>
        IEnumerable<IGenericError> Errors { get; }

        /// <summary>
        /// The list of exceptions contained in this status
        /// </summary>
        IEnumerable<Exception> Exceptions { get; }

        /// <summary>
        /// If there are no errors this is true
        /// </summary>
        bool Success { get; }

        /// <summary>
        /// If there are errors this is true
        /// </summary>
        bool HasErrors { get; }

        /// <summary>
        /// If the status contains exceptions
        /// </summary>
        bool HasExceptions { get; }

        /// <summary>
        /// The message of this status, does not combine error messages. Use ToString() instead
        /// </summary>
        string Message { get; }

        /// <summary>
        /// Will combine the error messages of the provided status with this status. 
        /// If the provided status has a different success message, and no errors, replaces this statuses success message with the provided status
        /// </summary>
        /// <param name="status"></param>
        /// <returns>This status</returns>
        IGenericStatus MergeStatuses(IGenericStatus status);

        /// <summary>
        /// Combines all error messages into a single string
        /// </summary>
        /// <param name="seperator"></param>
        /// <returns></returns>
        string ToString(string seperator = "\n");

        /// <summary>
        /// Combines all user-friendly error messages into a single string
        /// </summary>
        /// <param name="seperator"></param>
        /// <returns></returns>
        string ToUserFriendyString(string seperator = "\n");

        /// <summary>
        /// Adds the provided exception to the status.
        /// This also adds the exception message as <see langword="abstract"/>new error
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        IGenericStatus AddException(Exception ex);

        /// <summary>
        /// Adds a new error to the status
        /// </summary>
        /// <param name="message">The error message</param>
        /// <param name="userMessage">The user friendly error message</param>
        /// <returns></returns>
        IGenericStatus AddError(string message, string userMessage = "");

        /// <summary>
        /// Adds a new error to the status if the item is null
        /// </summary>
        /// <remarks>This only accepts reference types</remarks>
        /// <param name="value">>The value that is checked</param>
        /// <param name="message">The error message</param>
        /// <param name="userMessage">The user friendly error message</param>
        /// <returns></returns>
        IGenericStatus AddErrorIfNull<T>(T value, string message, string userMessage = "") where T : class;

        /// <summary>
        /// Adds a new error to the status if the item is null
        /// </summary>
        /// <remarks>This only accepts structs that impliment <see cref="Nullable{T}"/></remarks>
        /// <param name="value">>The nullable value that is checked</param>
        /// <param name="message">The error message</param>
        /// <param name="userMessage">The user friendly error message</param>
        /// <returns></returns>
        IGenericStatus AddErrorIfNull<T>(T? value, string message, string userMessage = "") where T : struct;

        /// <summary>
        /// Adds a new error to the status if the item is null or is default(T)
        /// </summary>
        /// <remarks>This only accepts reference types</remarks>
        /// <param name="value">>The value that is checked</param>
        /// <param name="message">The error message</param>
        /// <param name="userMessage">The user friendly error message</param>
        /// <returns></returns>
        IGenericStatus AddErrorIfNullOrDefault<T>(T value, string message, string userMessage = "") where T : class;


        /// <summary>
        /// Adds a new error to the status if the item is null or is default(T)
        /// </summary>
        /// <remarks>This only accepts structs that impliment <see cref="Nullable{T}"/></remarks>
        /// <param name="value">>The nullable value that is checked</param>
        /// <param name="message">The error message</param>
        /// <param name="userMessage">The user friendly error message</param>
        /// <returns></returns>
        IGenericStatus AddErrorIfNullOrDefault<T>(T? value, string message, string userMessage = "") where T : struct;

        /// <summary>
        /// Invokes the delegate, and if it throws an exception, records it in the current status.
        /// If success, return the result of the delegate
        /// </summary>
        /// <typeparam name="TOut">The outut type</typeparam>
        /// <param name="func">The delegate</param>
        /// <param name="keepException">To keep the exception in the stus, or just record the error message</param>
        /// <returns></returns>
        TOut InvokeAndReturnResult<TOut>(Func<TOut> func, bool keepException = false);
    }
}
