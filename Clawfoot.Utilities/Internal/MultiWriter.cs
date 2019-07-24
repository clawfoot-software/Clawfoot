using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Clawfoot.Utilities.Internal
{
    internal class MultiWriter : TextWriter
    {
        TextWriter consoleWriter = null; // The console-specific writer
        MultiWriter multiWriter = null; // Nested multiwriter, used for cases where much more complex behavior is needed
        IReadOnlyList<StreamWriter> fileWriters; // The file specific writers       

        bool insertTimestamps;

        public TextWriter ConsoleWriter => consoleWriter;
        public IEnumerable<StreamWriter> FileWriters => fileWriters;

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
            this.fileWriters = new List<StreamWriter>(fileWriters);
        }

        public MultiWriter(bool insertTimestamps, TextWriter consoleWriter, params StreamWriter[] fileWriters)
            : this(insertTimestamps, fileWriters)
        {
            this.consoleWriter = consoleWriter;
        }

        public MultiWriter(bool insertTimestamps, TextWriter consoleWriter, MultiWriter multiWriter, params StreamWriter[] fileWriters)
            : this(insertTimestamps, consoleWriter, fileWriters)
        {
            this.multiWriter = multiWriter;
        }

        public override Encoding Encoding => consoleWriter?.Encoding ?? fileWriters[0].Encoding;

        public override void Flush()
        {
            consoleWriter?.Flush();
            multiWriter?.Flush();
            foreach (TextWriter writer in fileWriters)
            {
                writer.Flush();
            }
        }

        public override void Write(char value)
        {
            consoleWriter?.Write(value);
            multiWriter?.Write(value);
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
                multiWriter?.WriteLine(value); //Write multiwriter like normal

                string newValue = DateTime.Now.ToString() + ": " + value;
                foreach (TextWriter writer in fileWriters)
                {
                    writer.WriteLine(newValue);
                }
            }
            else
            {
                base.WriteLine(value); //Write all like normal, triggers overriden Write() function
            }
        }
    }
}
