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
        /// Sets the result of the status
        /// </summary>
        /// <param name="result"></param>
        void SetResult(T result);

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
