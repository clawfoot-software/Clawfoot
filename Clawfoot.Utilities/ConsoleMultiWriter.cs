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

        readonly MultiWriter multiWriter;
        readonly TextWriter originalOutput;

        /// <summary>
        /// The original console writer that outputs to the console.
        /// </summary>
        public TextWriter ConsoleWriter => originalOutput;

        class MultiWriter : TextWriter
        {
            IReadOnlyList<TextWriter> writers;

            public MultiWriter(params TextWriter[] writers)
            {
                if (writers.Length == 0)
                {
                    throw new InvalidOperationException("Must have at least one TextWriter for MultiWriter, none where provided");
                }

                this.writers = new List<TextWriter>(writers);
            }

            public override Encoding Encoding => writers[0].Encoding;

            public override void Flush()
            {
                foreach (TextWriter writer in writers)
                {
                    writer.Flush();
                }
            }

            public override void Write(char value)
            {
                foreach (TextWriter writer in writers)
                {
                    writer.Write(value);
                }
            }
        }

        /// <summary>
        /// Creates a new ConsoleMultiWriter to copy or redirect console output to file(s)
        /// </summary>
        /// <param name="writeToConsole">If you wish to continue writing output to the console</param>
        /// <param name="outputPaths"></param>
        public ConsoleMultiWriter(bool writeToConsole, params string[] outputPaths)
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

                if (writeToConsole)
                {
                    //Create temporary array so originalOutput is included
                    TextWriter[] textWriters = new TextWriter[fileWriters.Count + 1];
                    textWriters[0] = originalOutput;
                    for (int i = 0; i < fileWriters.Count; i++)
                    {
                        textWriters[i + 1] = fileWriters[i];
                    }
                    multiWriter = new MultiWriter(textWriters);
                }
                else
                {
                    multiWriter = new MultiWriter(fileWriters.ToArray());
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
                fileStream.Flush();
                fileStream.Close();
            }
        }

    }
}
