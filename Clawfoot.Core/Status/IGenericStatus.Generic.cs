using System;
using System.Collections.Generic;
using System.Text;

namespace Clawfoot.Core.Status
{
    public interface IGenericStatus<T> : IGenericStatus
    {
        /// <summary>
        /// The return result, or if there are errors it will return default(T)
        /// </summary>
        T Result { get; set; }

        /// <summary>
        /// If this status has a result.
        /// Returns false if there are errors, even if a result has been set
        /// </summary>
        bool HasResult { get; }

        /// <summary>
        /// Sets the result of the status
        /// </summary>
        /// <param name="result"></param>
        IGenericStatus<T> SetResult(T result);

        /// <summary>
        /// Will combine the result, errors, and exceptions of the provided status with this status. 
        /// If the provided status has a different success message, and no errors, replaces this statuses success message with the provided status.
        /// Will prioritize keeping the status result that exists. If this status doesn't have a result, and the provided status doe, will keep the provided result.
        /// If both statuses have a result, this will prioritize the current statuses result over the provided status.
        /// Returns this status
        /// </summary>
        /// <param name="status">The status to merge into this status</param>
        /// <returns>This status</returns>
        IGenericStatus<T> MergeStatuses(IGenericStatus<T> status);


        /// <summary>
        /// Will combine the errors, exceptions, and result of this status into the provided status using <see cref="MergeStatuses(IGenericStatus{T})"/>.
        /// Returns the provided status
        /// </summary>
        /// <param name="status">The status to merge into</param>
        /// <returns>The provided status</returns>
        IGenericStatus<T> MergeIntoStatus(IGenericStatus<T> status);

        /// <summary>
        /// Will combine the errors and exceptions of this status into the provided status using <see cref="IGenericStatus.MergeStatuses(IGenericStatus)"/>.
        /// Returns the result of this status
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        T MergeIntoStatusWithResult(IGenericStatus status);

        /// <summary>
        /// Invokes the delegate, and if it throws an exception, records it in the current status and returns null.
        /// If success, sets the status result, and returns the result of the delegate
        /// </summary>
        /// <param name="func"></param>
        /// <param name="keepException"></param>
        /// <returns></returns>
        T InvokeAndSetResult(Func<T> func, bool keepException = false);
    }
}
