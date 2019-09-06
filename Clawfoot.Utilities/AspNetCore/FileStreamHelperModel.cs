using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Clawfoot.Utilities.AspNetCore
{
    /// <summary>
    /// The model outputted by <see cref="FileStreamHelper"/>.
    /// Contains Form and File data for the request
    /// </summary>
    public class FileStreamHelperModel
    {
        public FileStreamHelperModel(IEnumerable<FileMultipartSection> fileSections)
        {
            FileSections = fileSections ?? throw new ArgumentNullException(nameof(fileSections));
            FileCount = FileSections.Count();
        }

        public FileStreamHelperModel(FormValueProvider formValues, IEnumerable<FileMultipartSection> fileSections)
            : this(fileSections)
        {
            FormValues = formValues;
        }

        public bool HasFormValues => !(FormValues is null);
        public int FileCount { get; }

        public FormValueProvider FormValues { get; }
        public IEnumerable<FileMultipartSection> FileSections { get; }

        public IEnumerable<Stream> FileStreams
        {
            get
            {
                foreach (var section in FileSections)
                {
                    yield return section.FileStream;
                }
            }
        }

        public FileMultipartSection FileSection => FileCount == 1
                ? FileSections.First()
                : throw new InvalidOperationException("Cannot use FileSection when multiple FileSections exist");

        public Stream FileStream => FileCount == 1
                ? FileSection.FileStream
                : throw new InvalidOperationException("Cannot get a single file stream when there are multiple files");

    }
}
