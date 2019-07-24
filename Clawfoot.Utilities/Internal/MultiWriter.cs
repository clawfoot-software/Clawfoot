using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Clawfoot.Utilities.Internal
{
    internal class MultiWriter : TextWriter
    {
        TextWriter consoleWriter; // The console-specific writer
        IReadOnlyList<TextWriter> fileWriters; // The file specific writers         
        bool insertTimestamps;

        public MultiWriter(bool insertTimestamps, params StreamWriter[] fileWriters)
        {
            if (fileWriters.Length == 0)
            {
                throw new InvalidOperationException("Must have at least one TextWriter for MultiWriter, none where provided");
            }

            if (!fileWriters.Any(x => x.BaseStream is FileStream))
            {
                throw new InvalidOperationException("MultiWriter expects all StreamWriters in the `fileWriters` parameter to be FileStreams");
            }

            this.insertTimestamps = insertTimestamps;
            this.fileWriters = new List<TextWriter>(fileWriters);
        }

        public MultiWriter(bool insertTimestamps, TextWriter consoleWriter, params StreamWriter[] fileWriters)
            : this(insertTimestamps, fileWriters)
        {
            this.consoleWriter = consoleWriter;
        }

        public override Encoding Encoding => consoleWriter?.Encoding ?? fileWriters[0].Encoding;

        public override void Flush()
        {
            consoleWriter?.Flush();
            foreach (TextWriter writer in fileWriters)
            {
                writer.Flush();
            }
        }

        public override void Write(char value)
        {
            consoleWriter?.Write(value);
            foreach (TextWriter writer in fileWriters)
            {
                writer.Write(value);
            }
        }

        // Overriden to attempt to insert timestamps
        public override void WriteLine(String value)
        {
            if (insertTimestamps)
            {
                consoleWriter?.WriteLine(value); //Write console like normal

                string newValue = DateTime.Now.ToString() + ": " + value;
                foreach (TextWriter writer in fileWriters)
                {
                    writer.WriteLine(newValue);
                }
            }
            else
            {
                base.WriteLine(value); //Write all like normal
            }
        }
    }
}
