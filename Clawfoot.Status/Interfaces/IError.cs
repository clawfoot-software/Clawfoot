using System;
using System.Collections.Generic;
using System.Text;

namespace Clawfoot.Status
{
    public interface IError
    {
        int Code { get; }
        string Message { get; }
        string UserMessage { get; }

        string ToString();
        string ToUserString();
    }
}
