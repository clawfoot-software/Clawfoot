using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clawfoot.Extensions.Newtonsoft.Json
{
    public static class Newtonsoft
    {
        public static bool IsJObjectStringEmpty(this JObject jObject, string key)
        {
            if (jObject[key] == null)
            {
                return true;
            }
            string value = (string)jObject[key];
            return String.IsNullOrWhiteSpace(value);
        }

        public static bool IsJTokenStringEmpty(this JToken jToken, string key)
        {
            if (jToken[key] == null)
            {
                return true;
            }
            string value = (string)jToken[key];
            return String.IsNullOrWhiteSpace(value);
        }

        public static bool IsJTokenBoolEmpty(this JToken jToken, string key)
        {
            if (jToken[key] == null)
            {
                return true;
            }
            try
            {
                bool value = (bool)jToken[key];
                return false;
            }
            catch
            {
                return true;
            }
        }

        public static bool IsJObjectDictionaryEmpty(this JObject jObject, string key)
        {
            if (jObject[key] == null)
            {
                return true;
            }
            JObject value = (JObject)jObject[key];
            return value.Count == 0;
        }
    }
}
