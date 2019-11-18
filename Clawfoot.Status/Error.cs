using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clawfoot.Status
{
    public class Error : IError
    {
        public Error() { }
        public Error(string message, string userMessage = "", int code = -1, string groupName = "", string memberName = "")
        {
            Message = message;
            GroupName = string.IsNullOrEmpty(groupName) ? String.Empty : groupName;
            UserMessage = string.IsNullOrEmpty(userMessage) ? String.Empty : userMessage;
            MemberName = string.IsNullOrEmpty(memberName) ? String.Empty : memberName;
            Code = code;
        }

        public int Code { get; set; }
        public string GroupName { get; set; }
        public string Message { get; set; }
        public string UserMessage { get; set; }

        public string MemberName { get; set; }

        public override string ToString()
        {
            return Message;
        }

        public string ToUserString()
        {
            return UserMessage;
        }

        /// <summary>
        /// Generates the error details from the enum value
        /// Requires that the enum value has the [Error] Attribute
        /// </summary>
        /// <param name="error"></param>
        /// <param name="errorParams">The parameters to format</param>
        /// <returns></returns>
        public static IError From<TErrorEnum>(TErrorEnum error, params string[] errorParams) where TErrorEnum : Enum
        {
            var attributeType = typeof(ErrorAttribute);

            var enumType = typeof(TErrorEnum);
            var memberInfos = enumType.GetMember(error.ToString());
            var enumValueMemberInfo = memberInfos.FirstOrDefault(x => x.DeclaringType == enumType);

            ErrorAttribute attribute = (ErrorAttribute)Attribute.GetCustomAttribute(enumValueMemberInfo, attributeType, false);

            if (attribute is null)
            {
                throw new InvalidOperationException("Error enum is expected to have an [Error] attribute to be used in Error<TErrorEnum>.From()");
            }

            return new Error()
            {
                Code = attribute.Code,
                GroupName = attribute.GroupName,
                Message = attribute.GetFormattedMessage(errorParams),
                UserMessage = attribute.GetFormattedUserMessage(errorParams),
                MemberName = attribute.MemberName
            };
        }

        /// <summary>
        /// Generates the error details from the enum value, using only the provided message
        /// Requires that the enum value has the [Error] Attribute
        /// </summary>
        /// <param name="error"></param>
        /// <param name="errorParams">The parameters to format</param>
        /// <returns></returns>
        public static IError From<TErrorEnum>(TErrorEnum error, string message, string userMessage = "") where TErrorEnum : Enum
        {
            var attributeType = typeof(ErrorAttribute);

            var enumType = typeof(TErrorEnum);
            var memberInfos = enumType.GetMember(error.ToString());
            var enumValueMemberInfo = memberInfos.FirstOrDefault(x => x.DeclaringType == enumType);

            ErrorAttribute attribute = (ErrorAttribute)Attribute.GetCustomAttribute(enumValueMemberInfo, attributeType, false);

            if (attribute is null)
            {
                throw new InvalidOperationException("Error enum is expected to have an [Error] attribute to be used in Error<TErrorEnum>.From()");
            }

            return new Error()
            {
                Code = attribute.Code,
                GroupName = attribute.GroupName,
                Message = message,
                UserMessage = userMessage,
                MemberName = attribute.MemberName
            };
        }
    }
}
