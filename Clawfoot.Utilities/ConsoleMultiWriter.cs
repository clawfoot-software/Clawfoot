using Clawfoot.Utilities.Internal;
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
        readonly IReadOnlyList<StreamWriter> fileWriterStreams;
        
        readonly MultiWriter multiWriter; // Combined multiwriter. What becomes the default output for Console

        readonly TextWriter originalOutput; // Original Console output
        readonly MultiWriter fileWriter; // File-only writers, subset of fileWriters

        /// <summary>
        /// The original console writer that outputs to the console.
        /// </summary>
        public TextWriter ConsoleWriter => originalOutput;
        /// <summary>
        /// The TextWriter that will write only to files
        /// </summary>
        public TextWriter FileWriter => fileWriter;

        /// <summary>
        /// Creates a new ConsoleMultiWriter to copy or redirect console output to file(s)
        /// </summary>
        /// <param name="writeToConsole">If you wish to continue writing output to the console</param>
        /// <param name="timestamps">If you wish for DateTime stamps to be inserted on new file lines</param>
        /// <param name="replaceFiles">If the output files should be replaced if they exist</param>
        /// <param name="outputPaths"></param>
        public ConsoleMultiWriter(bool writeToConsole, bool timestamps = false, bool replaceFiles = false, params string[] outputPaths)
        {
            if (outputPaths.Length == 0)
            {
                throw new InvalidOperationException("Must have at least one outputPath for ConsoleMultiWriter. None where provided.");
            }

            // Maintain easy reference to original output
            originalOutput = Console.Out;

            try
            {
                (fileStreams, fileWriterStreams) = CreateFileWriters(replaceFiles, outputPaths);
                fileWriter = new MultiWriter(timestamps, fileWriterStreams.ToArray());

                // If we want to write to the console
                // include the original TextWriter in the multiwriter
                if (writeToConsole)
                {
                    multiWriter = new MultiWriter(timestamps, originalOutput, fileWriterStreams.ToArray());
                }
                else
                {
                    multiWriter = new MultiWriter(timestamps, fileWriterStreams.ToArray());
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

        private (List<FileStream> streams, List<StreamWriter> writers) CreateFileWriters(bool replaceFiles, params string[] outputPaths)
        {
            List<FileStream> streams = new List<FileStream>();
            List<StreamWriter> writers = new List<StreamWriter>();

            foreach (string outputPath in outputPaths)
            {
                FileStream fileStream;
                if (replaceFiles)
                {
                    fileStream = File.Open(outputPath, FileMode.Create, FileAccess.Write, FileShare.Read);
                }
                else
                {
                    fileStream = File.Open(outputPath, FileMode.Append, FileAccess.Write, FileShare.Read);
                }
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

            foreach(var fileWriter in fileWriterStreams)
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
