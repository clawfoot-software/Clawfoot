using System;
using System.Collections.Generic;
using System.Text;

namespace Clawfoot.Status
{

    /// <summary>
    /// Attribute used to define error information for enum errors
    /// The message is not required, as the error may be system generated and provides it's own message
    /// </summary>
    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class ErrorAttribute : Attribute
    {
        public ErrorAttribute() { }

        public int Code { get; set; }
        public string GroupName { get; set; }
        public string Message { get; set; }
        public string UserMessage { get; set; }

        public string MemberName { get; set; }

        public string GetFormattedMessage(params string[] values)
        {
            if(values is null || values.Length == 0)
            {
                return Message;
            }

            return String.Format(Message, values);
        }

        public string GetFormattedUserMessage(params string[] values)
        {
            if (values is null || values.Length == 0)
            {
                return UserMessage;
            }

            return String.Format(UserMessage, values);
        }
    }
}
