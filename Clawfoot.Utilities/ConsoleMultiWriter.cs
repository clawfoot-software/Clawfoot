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
        private List<FileStream> fileStreams = new List<FileStream>();
        private List<StreamWriter> fileWriterStreams = new List<StreamWriter>();

        private TextWriter originalOutput; // Original Console output
        private TextWriter originalErrorOutput; // Original Console Error output

        private MultiWriter fileWriter; // File-only writers, subset of fileWriterStreams
        private MultiWriter fileErrorWriter; // File-only error writers, subset of fileWriterStreams

        private MultiWriter multiWriter; // Combined multiwriter. What becomes the default output for Console
        private MultiWriter multiErrorWriter; // Combined multiwriter. What becomes the default output for Console.Error



        /// <summary>
        /// The original console writer that outputs to the console.
        /// </summary>
        public TextWriter ConsoleWriter => originalOutput;
        /// <summary>
        /// The original console error writer that outputs to the console.
        /// </summary>
        public TextWriter ConsoleErrorWriter => originalErrorOutput;
        /// <summary>
        /// The TextWriter that will write only to files
        /// </summary>
        public TextWriter FileWriter => fileWriter;
        /// <summary>
        /// The TextWriter that will write only to files
        /// </summary>
        public TextWriter FileErrorWriter => fileErrorWriter;

        /// <summary>
        /// Creates a new ConsoleMultiWriter to copy or redirect console output to file(s)
        /// </summary>
        /// <param name="writeToConsole">If you wish to continue writing output to the console</param>
        /// <param name="outputPaths"></param>
        /// <param name="timestamps">If you wish for DateTime stamps to be inserted on new file lines</param>
        /// <param name="replaceFiles">If the output files should be replaced, if they exist</param>
        /// <param name="combineErrorOutput">Combine the output of Console and Console.Error into the Console files(s)</param>
        /*public ConsoleMultiWriter(bool writeToConsole, string[] outputPaths, bool timestamps = false, bool replaceFiles = false, bool combineErrorOutput = true)
        {
            if (outputPaths.Length == 0)
            {
                throw new InvalidOperationException("Must have at least one outputPath for ConsoleMultiWriter. None where provided.");
            }

            // Maintain easy reference to original output
            originalOutput = Console.Out;
            originalErrorOutput = Console.Error;

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

            if (combineErrorOutput)
            {
                Console.SetError(multiWriter);
            }
        }*/

        /// <summary>
        /// Creates a new ConsoleMultiWriter to copy or redirect console output to file(s)
        /// </summary>
        /// <param name="writeToConsole">If you wish to continue writing output to the console</param>
        /// <param name="errorOutputPath">The output path for just errors if Console.Error is to be redirected</param>
        /// <param name="timestamps">If you wish for DateTime stamps to be inserted on new file lines</param>
        /// <param name="replaceFiles">If the output files should be replaced, if they exist</param>
        /// <param name="combineErrorOutput">Combine the output of Console and Console.Error into the normal Console files(s)</param>
        /// <param name="outputPaths"></param>
        public ConsoleMultiWriter(bool writeToConsole, string errorOutputPath, string[] outputPaths, bool timestamps = false, bool replaceFiles = false, bool combineErrorOutput = true)
        {
            if(outputPaths is null)
            {
                throw new ArgumentNullException("outputPaths cannot be null");
            }

            // Maintain easy reference to original output
            originalOutput = Console.Out;
            originalErrorOutput = Console.Error;

            // Writer that will write messages to normal console and outputs
            MultiWriter multiWriter = CreateMultiWriter(replaceFiles, timestamps, outputPaths, originalOutput);
            fileWriter = CreateMultiWriter(replaceFiles, timestamps, multiWriter.FileWriters); //Writes Console only to files

            // Writer that will write Console.Error's to error file, console, and to the normal multiWriter
            MultiWriter errorMultiWriter = CreateMultiWriter(replaceFiles, timestamps, new[] { errorOutputPath }, multiWriter);
            fileErrorWriter = CreateMultiWriter(replaceFiles, timestamps, errorMultiWriter.FileWriters, multiWriter); //Writes Console.Error only to files


            Console.SetOut(multiWriter);
            Console.SetError(errorMultiWriter);
        }

        /// <summary>
        /// Creates a new ConsoleMultiWriter to copy or redirect console error output to file(s)
        /// </summary>
        /// <param name="writeToConsole">If you wish to continue writing output to the console</param>
        /// <param name="errorOutputPath">The output path for errors if Console.Error is to be redirected</param>
        /// <param name="timestamps">If you wish for DateTime stamps to be inserted on new file lines</param>
        /// <param name="replaceFiles">If the output files should be replaced, if they exist</param>
        /*public ConsoleMultiWriter(bool writeToConsole, string errorOutputPath, bool timestamps = false, bool replaceFiles = false)
        {
            originalErrorOutput = Console.Error;

            if (!(errorOutputPath is null))
            {
                (FileStream fileStream, StreamWriter fileWriter) = CreateFileWriter(replaceFiles, errorOutputPath);
                fileStreams.Add(fileStream);
                fileWriterStreams.Add(fileWriter);

                fileErrorWriter = new MultiWriter(timestamps, fileWriter);

                if (writeToConsole)
                {
                    multiErrorWriter = new MultiWriter(timestamps, originalErrorOutput, fileWriter);
                }
                else
                {
                    multiErrorWriter = new MultiWriter(timestamps, fileWriter);
                }
                Console.SetError(multiErrorWriter);
            }
        }*/

        private MultiWriter CreateMultiWriter(bool replaceFiles, bool timestamps, IEnumerable<StreamWriter> fileWriters, TextWriter consoleOutput = null)
        {
            MultiWriter fileWriter;
            if (consoleOutput is null)
            {
                fileWriter = new MultiWriter(timestamps, fileWriters.ToArray());
            }
            else
            {
                fileWriter = new MultiWriter(timestamps, consoleOutput, fileWriters.ToArray());
            }

            return fileWriter;
        }

        private MultiWriter CreateMultiWriter(bool replaceFiles, bool timestamps, IEnumerable<StreamWriter> fileWriters, MultiWriter nestedWriter, TextWriter consoleOutput = null)
        {
            MultiWriter fileWriter;
            if (consoleOutput is null)
            {
                fileWriter = new MultiWriter(timestamps, nestedWriter, fileWriters.ToArray());
            }
            else
            {
                fileWriter = new MultiWriter(timestamps, consoleOutput, nestedWriter, fileWriters.ToArray());
            }

            return fileWriter;
        }

        private MultiWriter CreateMultiWriter(bool replaceFiles, bool timestamps, string[] paths, TextWriter consoleOutput = null)
        {
            (List<FileStream> streams, List<StreamWriter> writers) = CreateFileWriters(replaceFiles, paths);

            MultiWriter fileWriter = CreateMultiWriter(replaceFiles, timestamps, writers, consoleOutput);

            fileStreams.AddRange(streams);
            fileWriterStreams.AddRange(writers);

            return fileWriter;
        }

        private MultiWriter CreateMultiWriter(bool replaceFiles, bool timestamps, string[] paths, MultiWriter nestedWriter, TextWriter consoleOutput = null)
        {
            (List<FileStream> streams, List<StreamWriter> writers) = CreateFileWriters(replaceFiles, paths);
            MultiWriter fileWriter;

            if (consoleOutput is null)
            {
                fileWriter = new MultiWriter(timestamps, nestedWriter, writers.ToArray());
            }
            else
            {
                fileWriter = new MultiWriter(timestamps, consoleOutput, nestedWriter, writers.ToArray());
            }

            fileStreams.AddRange(streams);
            fileWriterStreams.AddRange(writers);

            return fileWriter;
        }

        private (List<FileStream> streams, List<StreamWriter> writers) CreateFileWriters(bool replaceFiles, params string[] outputPaths)
        {
            List<FileStream> streams = new List<FileStream>();
            List<StreamWriter> writers = new List<StreamWriter>();

            foreach (string outputPath in outputPaths)
            {
                (FileStream fileStream, StreamWriter fileWriter) = CreateFileWriter(replaceFiles, outputPath);

                streams.Add(fileStream);
                writers.Add(fileWriter);
            }
            return (streams, writers);
        }

        private (FileStream stream, StreamWriter writer) CreateFileWriter(bool replaceFiles, string outputPath)
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

            return (fileStream, fileWriter);
        }


        public void Dispose()
        {
            Console.SetOut(originalOutput);
            Console.SetError(originalErrorOutput);

            foreach (var fileWriter in fileWriterStreams)
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

            fileWriter?.Dispose();
            multiWriter?.Dispose();
        }

    }
}
