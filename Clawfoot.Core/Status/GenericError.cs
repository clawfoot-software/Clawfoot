using System;
using System.Collections.Generic;
using System.Text;

namespace Clawfoot.Core.Status
{
    public struct GenericError : IGenericError
    {
        public GenericError(string message, string userMessage = "")
        {
            Message = message;
            UserMessage = userMessage;
        }

        string Message;

        string UserMessage;

        public override string ToString()
        {
            return Message;
        }

        public string ToUserString()
        {
            return UserMessage;
        }
    }
}
