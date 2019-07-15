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
        IImmutableList<IGenericError> Errors { get; }

        /// <summary>
        /// If there are no errors this is true
        /// </summary>
        bool Success { get; }

        /// <summary>
        /// If there are errors this is true
        /// </summary>
        bool HasErrors { get; }

        /// <summary>
        /// The message of this status, does not combine error messages. Use ToString() instead
        /// </summary>
        string Message { get; }

        /// <summary>
        /// Will combine the error messages of the provided status with this status. 
        /// If the provided status has a different success message, and no errors, replaces this statuses success message with the provided status
        /// </summary>
        /// <param name="status"></param>
        void MergeStatuses(IGenericStatus status);

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
    }
}
