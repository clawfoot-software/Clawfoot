using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Clawfoot.Utilities.AspNetCore
{
    /// <summary>
    /// Helper class that assists with handling File Streaming
    /// </summary>
    public static class FileStreamHelper
    {
        /// <summary>
        /// Parses the HttpRequest into it's form data and files
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<FileStreamHelperModel> ParseAsync(HttpRequest request, bool fileOnly = false)
        {
            (var formSection, var fileSections) = await ExtractSections(request, fileOnly);

            FormValueProvider formValues = await ParseFormBody(formSection);
            return new FileStreamHelperModel(formValues, fileSections);
        }

        /// <summary>
        /// Copies the file Stream from the request to the target stream
        /// Returns the formvalues if there are any
        /// </summary>
        /// <param name="request"></param>
        /// <param name="targetStream"></param>
        /// <returns></returns>
        public static async Task<FormValueProvider> StreamFileAsync(this HttpRequest request, Stream targetStream)
        {
            FileStreamHelperModel model = await ParseAsync(request);
            await model.FileStream.CopyToAsync(targetStream);

            return model.FormValues;
        }

        /// <summary>
        /// Parses the HttpRequest and extracts the file sections and the form section
        /// Sets the _formSection and _fileSections fields.
        /// </summary>
        /// <returns></returns>
        private static async Task<(FormMultipartSection formSection, List<FileMultipartSection> fileSections)> ExtractSections(HttpRequest request, bool fileOnly = false)
        {
            if (!MultipartRequestHelper.IsMultipartContentType(request.ContentType))
            {
                throw new Exception($"Expected a multipart request, but got {request.ContentType}");
            }

            FormMultipartSection formSection = null;
            List<FileMultipartSection> fileSections = new List<FileMultipartSection>();

            string boundary = MultipartRequestHelper.GetBoundary(request);
            var reader = new MultipartReader(boundary, request.Body);

            MultipartSection section = await reader.ReadNextSectionAsync();

            while (section != null)
            {
                ContentDispositionHeaderValue contentDisposition;
                var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out contentDisposition);

                if (hasContentDispositionHeader)
                {
                    if (MultipartRequestHelper.HasFileContentDisposition(contentDisposition))
                    {
                        fileSections.Add(section.AsFileSection());
                    }
                    else if (MultipartRequestHelper.HasFormDataContentDisposition(contentDisposition))
                    {
                        formSection = section.AsFormDataSection();
                    }
                }

                if(fileOnly && fileSections.Count > 0)
                {
                    break;
                }

                // Drains any remaining section body that has not been consumed and
                // reads the headers for the next section.
                section = await reader.ReadNextSectionAsync();
            }

            return (formSection, fileSections);
        }

        /// <summary>
        /// Parses the form body into a FormValueProvider
        /// </summary>
        /// <param name="formSection"></param>
        /// <returns></returns>
        private static async Task<FormValueProvider> ParseFormBody(FormMultipartSection formSection)
        {
            // Used to accumulate all the form url encoded key value pairs in the request.
            var formAccumulator = new KeyValueAccumulator();

            if (!(formSection is null))
            {
                MultipartSection section = formSection.Section;
                ContentDispositionHeaderValue contentDisposition = section.GetContentDispositionHeader();

                // Do not limit the key name length here because the 
                // multipart headers length limit is already in effect.
                StringSegment key = HeaderUtilities.RemoveQuotes(contentDisposition.Name);
                Encoding encoding = GetEncoding(section);

                using (var streamReader = new StreamReader(
                    section.Body,
                    encoding,
                    detectEncodingFromByteOrderMarks: true,
                    bufferSize: 1024,
                    leaveOpen: true))
                {
                    // The value length limit is enforced by MultipartBodyLengthLimit
                    string value = await streamReader.ReadToEndAsync();

                    if (String.Equals(value, "undefined", StringComparison.OrdinalIgnoreCase))
                    {
                        value = String.Empty;
                    }

                    formAccumulator.Append(key.Value, value);
                    if (formAccumulator.ValueCount > FormReader.DefaultValueCountLimit)
                    {
                        throw new InvalidDataException($"Form key count limit {FormReader.DefaultValueCountLimit} exceeded.");
                    }
                }
            }

            // Bind form data to a model
            return new FormValueProvider(
                BindingSource.Form,
                new FormCollection(formAccumulator.GetResults()),
                CultureInfo.CurrentCulture);
        }

        private static Encoding GetEncoding(MultipartSection section)
        {
            MediaTypeHeaderValue mediaType;
            var hasMediaTypeHeader = MediaTypeHeaderValue.TryParse(section.ContentType, out mediaType);
            // UTF-7 is insecure and should not be honored. UTF-8 will succeed in 
            // most cases.
            if (!hasMediaTypeHeader || Encoding.UTF7.Equals(mediaType.Encoding))
            {
                return Encoding.UTF8;
            }
            return mediaType.Encoding;
        }
    }
}
