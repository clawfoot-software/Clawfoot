using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace Clawfoot.Utilities.AspNetCore
{
    /// <summary>
    /// https://dotnetcoretutorials.com/2017/03/12/uploading-files-asp-net-core/
    /// </summary>
    public static class MultipartRequestHelper
    {
        // Content-Type: multipart/form-data; boundary="----WebKitFormBoundarymx2fSWqWSd0OxQqq"
        // The spec says 70 characters is a reasonable limit.
        public static string GetBoundary(MediaTypeHeaderValue contentType, int lengthLimit)
        {
            string boundary = HeaderUtilities.RemoveQuotes(contentType.Boundary).Value; //.NET Core 2.0
            if (string.IsNullOrWhiteSpace(boundary))
            {
                throw new InvalidDataException("Missing content-type boundary.");
            }

            if (boundary.Length > lengthLimit)
            {
                throw new InvalidDataException(
                    $"Multipart boundary length limit {lengthLimit} exceeded.");
            }

            return boundary;
        }

        /// <summary>
        /// Gets the content-type boundary using the requests ContentType
        /// and FormOptions.DefaultMultipartBoundaryLengthLimit
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string GetBoundary(HttpRequest request)
        {
            var contentType = MediaTypeHeaderValue.Parse(request.ContentType);
            int lengthLimit = FormOptions.DefaultMultipartBoundaryLengthLimit;

            return GetBoundary(contentType, lengthLimit);
        }

        public static bool IsMultipartContentType(string contentType)
        {
            return !string.IsNullOrEmpty(contentType)
                    && contentType.IndexOf("multipart/", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public static bool HasFormDataContentDisposition(ContentDispositionHeaderValue contentDisposition)
        {
            // Content-Disposition: form-data; name="key";
            return contentDisposition != null
                    && contentDisposition.DispositionType.Equals("form-data")
                    && string.IsNullOrEmpty(contentDisposition.FileName.Value) // For .NET Core <2.0 remove ".Value"
                    && string.IsNullOrEmpty(contentDisposition.FileNameStar.Value); // For .NET Core <2.0 remove ".Value"
        }

        public static bool HasFileContentDisposition(ContentDispositionHeaderValue contentDisposition)
        {
            // Content-Disposition: form-data; name="myfile1"; filename="Misc 002.jpg"
            return contentDisposition != null
                    && contentDisposition.DispositionType.Equals("form-data")
                    && (!string.IsNullOrEmpty(contentDisposition.FileName.Value) // For .NET Core <2.0 remove ".Value"
                        || !string.IsNullOrEmpty(contentDisposition.FileNameStar.Value)); // For .NET Core <2.0 remove ".Value"
        }
    }
}
