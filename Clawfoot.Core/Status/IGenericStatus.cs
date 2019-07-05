using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Clawfoot.Core.Status
{
    public interface IGenericStatus
    {
        IImmutableList<IGenericError> Errors { get; }

        bool Success { get; }

        bool HasErrors { get; }

        string Message { get; }

        void CombineStatuses(IGenericStatus status);

        string ToString(string seperator = "\n");

        string ToUserFriendyString(string seperator = "\n");
    }
}
