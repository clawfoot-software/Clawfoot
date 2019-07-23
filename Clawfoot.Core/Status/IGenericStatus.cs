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
        /// Creates a <see cref="IGenericStatus{T}"/>, merges this status into it, and sets the result
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// /// <param name="result"></param>
        /// <returns></returns>
        IGenericStatus<T> SetResult<T>(T result);

        /// <summary>
        /// Will combine the errors and exceptions of the provided status with this status. 
        /// If the provided status has a different success message, and no errors, replaces this statuses success message with the provided status.
        /// Returns this status
        /// </summary>
        /// <param name="status">The status to merge into this status</param>
        /// <returns>This status</returns>
        IGenericStatus MergeStatuses(IGenericStatus status);

        /// <summary>
        /// Will combine the errors and exceptions of this status into the provided status.
        /// Returns the provided status
        /// </summary>
        /// <param name="status">The status to merge into</param>
        /// <returns>The provided status</returns>
        IGenericStatus MergeIntoStatus(IGenericStatus status);

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
        /// Returns this status
        /// </summary>
        /// <param name="action"></param>
        /// <param name="keepException"></param>
        /// <returns></returns>
        IGenericStatus Invoke(Action action, bool keepException);

        /// <summary>
        /// Invokes the delegate, and if it throws an exception, records it in the current status and returns null.
        /// If success, return the result of the delegate
        /// </summary>
        /// <typeparam name="TResult">The outut type</typeparam>
        /// <param name="func">The delegate</param>
        /// <param name="keepException">To keep the exception in the stus, or just record the error message</param>
        /// <returns></returns>
        TResult InvokeAndReturnResult<TResult>(Func<TResult> func, bool keepException = false);


    }
}
