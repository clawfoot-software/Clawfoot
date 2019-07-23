using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clawfoot.Extensions.Newtonsoft.Json
{
    public static class Newtonsoft
    {
        /// <summary>
        /// Determines if an int value located by the provided key exists, is invalid, or is default(int)
        /// </summary>
        /// <param name="jToken"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool IsIntEmpty(this JToken jToken, string key)
        {
            if (jToken[key] == null)
            {
                return true;
            }

            int value;
            if(!int.TryParse((string)jToken[key], out value))
            {
                return true;
            }

            return value == default(int);
        }

        /// <summary>
        /// Determines if an int value located by the provided key exists, is invalid, or is default(int)
        /// </summary>
        /// <param name="jObject"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool IsIntEmpty(this JObject jObject, string key)
        {
            if (jObject[key] == null)
            {
                return true;
            }

            int value;
            if (!int.TryParse((string)jObject[key], out value))
            {
                return true;
            }

            return value == default(int);
        }

        /// <summary>
        /// Determines if a string value located by the provided key exists, or is empty
        /// </summary>
        /// <param name="jObject"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool IsStringEmpty(this JObject jObject, string key)
        {
            if (jObject[key] == null)
            {
                return true;
            }
            string value = (string)jObject[key];
            return String.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// Determines if a string value located by the provided key exists, or is empty
        /// </summary>
        /// <param name="jToken"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool IsStringEmpty(this JToken jToken, string key)
        {
            if (jToken[key] == null)
            {
                return true;
            }
            string value = (string)jToken[key];
            return String.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// Determines if a bool value located by the provided key exists, or is empty
        /// </summary>
        /// <param name="jToken"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool IsBoolEmpty(this JToken jToken, string key)
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

        /// <summary>
        /// Determines if a JObject by the provided key exists, or is empty
        /// </summary>
        /// <param name="jObject"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool IsDictionaryEmpty(this JObject jObject, string key)
        {
            if (jObject[key] == null)
            {
                return true;
            }
            JObject value = (JObject)jObject[key];
            return value.Count == 0;
        }

        /// <summary>
        /// Determines if a JArray by the provided key exists, or is empty
        /// </summary>
        /// <param name="jObject"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool IsArrayEmpty(this JObject jObject, string key)
        {
            if (jObject[key] == null)
            {
                return true;
            }

            JArray value = (JArray)jObject[key];
            return value.Count == 0;
        }

        /// <summary>
        /// Determines if a JArray by the provided key exists, or is empty
        /// </summary>
        /// <param name="JToken"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool IsArrayEmpty(this JToken jToken, string key)
        {
            if (jToken[key] == null)
            {
                return true;
            }

            JArray value = (JArray)jToken[key];
            return value.Count == 0;
        }
    }
}
