using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clawfoot.TestUtilities.Performance
{
    public class PerfTest
    {
        public PerfTest(Action action, int testCount = 500, int iterationsPerTest = 100)
        {
            Action = action;
            TestCount = TestCount;
            IterationsPerTest = iterationsPerTest;
        }

        /// <summary>
        /// The Action that will be timed
        /// </summary>
        public Action Action { get; }

        /// <summary>
        /// The number of runs that will be timed and analyzed
        /// Each test is composed of multiple test iterations who's results are grouped together
        /// </summary>
        public int TestCount { get; }

        /// <summary>
        /// The number of iterations per test chunk
        /// </summary>
        public int IterationsPerTest { get; }
    }

    /// <summary>
    /// The timing results of chunk of a performance test
    /// </summary>
    public class PerfTestChunkResult
    {
        /// <summary>
        /// The timings in ticks for all runs of this Performance Test
        /// </summary>
        public virtual List<double> Timings { get; } = new List<double>();
        
        /// <summary>
        /// The total time in Ticks for this Performance Test
        /// </summary>
        public double Ticks => Timings.Sum();

        /// <summary>
        /// The total time in milliseconds for this Performance Test, rounded to the nearest thousandth place
        /// </summary>
        public double Milliseconds => Math.Truncate(Ticks / 10000 * 1000) / 1000;

        /// <summary>
        /// The average time in milliseconds for each iteration
        /// </summary>
        public double MsPerIteration => Math.Truncate(Ticks / 10000 / Timings.Count * 1000) / 1000;

    }

    /// <summary>
    /// The total timings result of a performance test
    /// </summary>
    public class PerfTestResult
    {
        /// <summary>
        /// The timings for all runs of this Performance Test
        /// </summary>
        public virtual List<PerfTestChunkResult> Timings { get; } = new List<PerfTestChunkResult>();

        /// <summary>
        /// The total time in Ticks for this Performance Test
        /// </summary>
        public double Ticks => Timings.Sum(x => x.Ticks);

        /// <summary>
        /// The total time in milliseconds for this Performance Test, rounded to the nearest thousandth place
        /// </summary>
        public double Milliseconds => Math.Truncate(Ticks / 10000 * 1000) / 1000;

        /// <summary>
        /// The normalized mean for each chunk of the test in Ticks
        /// </summary>
        public double MeanTicks => Timings.Select(x => x.Ticks).ToList().NormalizedMean();

        /// <summary>
        /// The normalized mean in milliseconds for each chunk
        /// </summary>
        public double MeanMs =>  Math.Truncate(MeanTicks / 10000 * 1000) / 1000;

        /// <summary>
        /// The normalized mean for each iteration of the test in Ticks
        /// </summary>
        public double MeanTicksPerIteration => Math.Truncate(Timings.SelectMany(x => x.Timings).ToList().NormalizedMean() * 1000) / 1000;

        /// <summary>
        /// The normalized mean in milliseconds for each iteration
        /// </summary>
        /// 

        public double MeanMsPerIteration
        {
            get
            {
                double mean = Timings.SelectMany(
                    x => x.Timings.Select(n => n / 10000).ToList()
                )
                .ToList()
                .NormalizedMean();

                return Math.Truncate(mean * 1000) / 1000;

            }
        }
        //public double MeanMsPerIteration => Math.Truncate(Ticks / Timings.SelectMany(x => x.Timings).Count() / 10000 * 1000) / 1000;
    }
}
