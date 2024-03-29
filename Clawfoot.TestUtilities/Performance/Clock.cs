﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace Clawfoot.TestUtilities.Performance
{
    // Derived from https://stackoverflow.com/a/16157458
    public class Clock
    {
        interface IStopwatch
        {
            bool IsRunning { get; }
            TimeSpan Elapsed { get; }

            void Start();
            void Stop();
            void Reset();
        }

        class TimeWatch : IStopwatch
        {
            Stopwatch stopwatch = new Stopwatch();

            public TimeSpan Elapsed => stopwatch.Elapsed;

            public bool IsRunning => stopwatch.IsRunning;



            public TimeWatch()
            {
                if (!Stopwatch.IsHighResolution)
                    throw new NotSupportedException("Your hardware doesn't support high resolution counter");

                //prevent the JIT Compiler from optimizing Fkt calls away
                long seed = Environment.TickCount;

                //use the second Core/Processor for the test
                Process.GetCurrentProcess().ProcessorAffinity = new IntPtr(2);

                //prevent "Normal" Processes from interrupting Threads
                Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;

                //prevent "Normal" Threads from interrupting this thread
                Thread.CurrentThread.Priority = ThreadPriority.Highest;
            }



            public void Start()
            {
                stopwatch.Start();
            }

            public void Stop()
            {
                stopwatch.Stop();
            }

            public void Reset()
            {
                stopwatch.Reset();
            }
        }

        class CpuWatch : IStopwatch
        {
            TimeSpan startTime;
            TimeSpan endTime;
            bool isRunning;

            public TimeSpan Elapsed
            {
                get
                {
                    if (IsRunning)
                        throw new NotImplementedException("Getting elapsed span while watch is running is not implemented");

                    return endTime - startTime;
                }
            }

            public bool IsRunning
            {
                get { return isRunning; }
            }

            public void Start()
            {
                startTime = Process.GetCurrentProcess().TotalProcessorTime;
                isRunning = true;
            }

            public void Stop()
            {
                endTime = Process.GetCurrentProcess().TotalProcessorTime;
                isRunning = false;
            }

            public void Reset()
            {
                startTime = TimeSpan.Zero;
                endTime = TimeSpan.Zero;
            }
        }

        public static PerfTestResult BenchmarkTime(Action action, int iterationsPerChunk = 100, int iterations = 100)
        {
            return Benchmark<TimeWatch>(action, iterationsPerChunk, iterations);
        }

        public static PerfTestResult BenchmarkTime<TState>(Action<TState> action, Func<TState> setup, int iterationsPerChunk = 100, int iterations = 100)
        {
            return Benchmark<TimeWatch, TState>(action, setup, iterationsPerChunk, iterations);
        }

        public static void BenchmarkCpu(Action action, int iterationsPerChunk = 100, int iterations = 100)
        {
            Benchmark<CpuWatch>(action, iterations);
        }

        static PerfTestResult Benchmark<T>(Action action, int iterationsPerChunk = 100, int iterations = 100) 
            where T : IStopwatch, new()
        {
            bool consoleAvailable = Console.LargestWindowWidth != 0;

            //clean Garbage
            GC.Collect();

            //wait for the finalizer queue to empty
            GC.WaitForPendingFinalizers();

            //clean Garbage
            GC.Collect();

            //warm up
            action();

            var stopwatch = new T();
            var results = new PerfTestResult();

            int cursorLeft = consoleAvailable ? Console.CursorLeft : 0;
            int cursorTop = consoleAvailable ? Console.CursorTop : 0;

            for (int i = 0; i < iterations; i++)
            {
                var chunkResults = new PerfTestChunkResult();

                stopwatch.Reset();
                if (i > 0)
                {
                    if (consoleAvailable)
                    {
                        cursorLeft = Console.CursorLeft;
                        cursorTop = Console.CursorTop;
                    }
                }

                for (int j = 0; j < iterationsPerChunk; j++)
                {
                    if (consoleAvailable)
                    {
                        Console.SetCursorPosition(cursorLeft, cursorTop);
                    }
                    
                    stopwatch.Start();

                    action();

                    stopwatch.Stop();
                    chunkResults.Timings.Add(stopwatch.Elapsed.Ticks);
                    
                    // Show per chunk progress
                    // Unused
                    if (i + 1 % 10 == 0 || i + 1 == iterationsPerChunk)
                    {
                        //Console.WriteLine($"{i + 1}/{iterationsPerChunk}");
                    }

                    stopwatch.Reset();
                }

                results.Timings.Add(chunkResults);
                if (consoleAvailable)
                {
                    Console.WriteLine($"{i + 1}/{iterations}: {chunkResults.Milliseconds} ms | {chunkResults.MsPerIteration} ms/iteration ");
                }                
            }

            if (consoleAvailable)
            {
                Console.WriteLine("Normalized Mean: ");
                Console.WriteLine($"    {results.MeanMs}ms/chunk");
                Console.WriteLine($"    {results.MeanMsPerIteration}ms/iteration");
            }

            return results;
        }

        static PerfTestResult Benchmark<TStopwatch, TState>(Action<TState> action, Func<TState> setup, int iterationsPerChunk = 100, int iterations = 100)
            where TStopwatch : IStopwatch, new()
        {
            bool consoleAvailable = Console.LargestWindowWidth != 0;

            //clean Garbage
            GC.Collect();

            //wait for the finalizer queue to empty
            GC.WaitForPendingFinalizers();

            //clean Garbage
            GC.Collect();

            TState warmupState = setup();
            //warm up
            action(warmupState);

            var stopwatch = new TStopwatch();
            var results = new PerfTestResult();

            int cursorLeft = consoleAvailable ? Console.CursorLeft : 0;
            int cursorTop = consoleAvailable ? Console.CursorTop : 0;

            for (int i = 0; i < iterations; i++)
            {
                var chunkResults = new PerfTestChunkResult();

                stopwatch.Reset();
                if (i > 0)
                {
                    if (consoleAvailable)
                    {
                        cursorLeft = Console.CursorLeft;
                        cursorTop = Console.CursorTop;
                    }
                }

                for (int j = 0; j < iterationsPerChunk; j++)
                {
                    if (consoleAvailable)
                    {
                        Console.SetCursorPosition(cursorLeft, cursorTop);
                    }

                    TState state = setup();

                    stopwatch.Start();

                    action(state);

                    stopwatch.Stop();
                    chunkResults.Timings.Add(stopwatch.Elapsed.Ticks);

                    // Show per chunk progress
                    // Unused
                    if (i + 1 % 10 == 0 || i + 1 == iterationsPerChunk)
                    {
                        //Console.WriteLine($"{i + 1}/{iterationsPerChunk}");
                    }

                    stopwatch.Reset();
                }

                results.Timings.Add(chunkResults);
                if (consoleAvailable)
                {
                    Console.WriteLine($"{i + 1}/{iterations}: {chunkResults.Milliseconds} ms | {chunkResults.MsPerIteration} ms/iteration ");
                }
            }

            if (consoleAvailable)
            {
                Console.WriteLine("Normalized Mean: ");
                Console.WriteLine($"    {results.MeanMs}ms/chunk");
                Console.WriteLine($"    {results.MeanMsPerIteration}ms/iteration");
            }

            return results;
        }


    }
}
