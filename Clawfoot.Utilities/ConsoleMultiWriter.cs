using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Clawfoot.Utilities
{
    //Inspired by https://stackoverflow.com/a/6927051

    /// <summary>
    /// A disposable class that enables copying or redirecting console output to file(s)
    /// </summary>
    public class ConsoleMultiWriter : IDisposable
    {
        readonly IReadOnlyList<FileStream> fileStreams;
        readonly IReadOnlyList<StreamWriter> fileWriters;
        
        readonly MultiWriter multiWriter; // Combined multiwriter. What becomes the defualt output for Console
        readonly TextWriter originalOutput; // Original Console output
        readonly MultiWriter fileWriter; // File-only writers

        /// <summary>
        /// The original console writer that outputs to the console.
        /// </summary>
        public TextWriter ConsoleWriter => originalOutput;

        /// <summary>
        /// The TextWriter that will write only to files
        /// </summary>
        public TextWriter FileWriter => fileWriter;

        class MultiWriter : TextWriter
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
                :this(insertTimestamps, fileWriters)
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
                    string newValue = DateTime.Now.ToString() + ": " + value;
                    foreach (TextWriter writer in fileWriters)
                    {
                        writer.WriteLine(newValue);
                    }
                }
                else
                {
                    base.WriteLine(value);
                }
            }
        }

        /// <summary>
        /// Creates a new ConsoleMultiWriter to copy or redirect console output to file(s)
        /// </summary>
        /// <param name="writeToConsole">If you wish to continue writing output to the console</param>
        /// <param name="timestamps">If you wish for DateTime stamps to be inserted on new file lines</param>
        /// <param name="outputPaths"></param>
        public ConsoleMultiWriter(bool writeToConsole, bool timestamps, params string[] outputPaths)
        {
            if (outputPaths.Length == 0)
            {
                throw new InvalidOperationException("Must have at least one outputPath for ConsoleMultiWriter. None where provided.");
            }

            // Maintain easy reference to original output
            originalOutput = Console.Out;

            try
            {
                (fileStreams, fileWriters) = CreateFileWriters(outputPaths);

                fileWriter = new MultiWriter(timestamps, fileWriters.ToArray());

                // If we want to write to the console
                // include the original TextWriter in the multiwriter
                if (writeToConsole)
                {
                    multiWriter = new MultiWriter(timestamps, originalOutput, fileWriters.ToArray());
                }
                else
                {
                    multiWriter = new MultiWriter(timestamps, fileWriters.ToArray());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot open file for writing");
                Console.WriteLine(e.Message);
                return;
            }

            Console.SetOut(multiWriter);
        }

        private (List<FileStream> streams, List<StreamWriter> writers) CreateFileWriters(params string[] outputPaths)
        {
            List<FileStream> streams = new List<FileStream>();
            List<StreamWriter> writers = new List<StreamWriter>();

            foreach (string outputPath in outputPaths)
            {
                FileStream fileStream = File.Open(outputPath, FileMode.Append, FileAccess.Write, FileShare.Read);
                var fileWriter = new StreamWriter(fileStream);
                fileWriter.AutoFlush = true;

                streams.Add(fileStream);
                writers.Add(fileWriter);
            }
            return (streams, writers);
        }


        public void Dispose()
        {
            Console.SetOut(originalOutput);

            foreach(var fileWriter in fileWriters)
            {
                fileWriter.Flush();
                fileWriter.Close();
            }

            foreach (var fileStream in fileStreams)
            {
                try
                {
                    fileStream.Flush();
                    fileStream.Close();
                }
                catch
                {
                    continue;
                }
            }

            fileWriter.Dispose();
            multiWriter.Dispose();
        }

    }
}
