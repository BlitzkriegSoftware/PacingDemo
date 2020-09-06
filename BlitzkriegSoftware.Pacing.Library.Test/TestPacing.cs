using System;
using System.Collections.Generic;
using BlitzkriegSoftware.Pacing.Library.Test.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlitzkriegSoftware.Pacing.Library.Test
{
    [TestClass]
    public class TestPacing
    {

        #region "Boilerplate"

        private static TestContext _testContext;

        [ClassInitialize]
        public static void SetupTests(TestContext testContext)
        {
            _testContext = testContext;
        }

        #endregion

        #region "Constants and Variables"
        public const string KeyPrefix = "test";
        public const string KeySuffix = "Pace1";

        Random dice = new Random();
        #endregion

        #region "Pacer Agent Tests (unit)"

        [TestMethod]
        [TestCategory("Integration")]
        public void TestPacer1()
        {
            var agent = new PacerAgent(new BlitzkriegSoftware.Pacing.Library.Models.RedisConfiguration(), KeyPrefix);

            Assert.IsTrue(agent.IsValid);

            var ts = new TimeSpan(0, 0, 0, 1, 0);

            agent.MarkPacing(KeySuffix, ts);

            var sleepTS = ts.Add(new TimeSpan(0, 0, 0, 5));

            System.Threading.Thread.Sleep(sleepTS);

            var runnable = agent.Runnable(KeySuffix);

            Assert.IsTrue(runnable);
        }

        [TestMethod]
        [TestCategory("Integration")]
        public void TestPacer2()
        {
            var agent = new PacerAgent(new BlitzkriegSoftware.Pacing.Library.Models.RedisConfiguration(), KeyPrefix);

            Assert.IsTrue(agent.IsValid);

            var ts = new TimeSpan(0, 0, 0, 1, 0);

            agent.MarkPacing(KeySuffix, ts);

            var runnable = agent.Runnable(KeySuffix);

            Assert.IsFalse(runnable);
        }

        #endregion

        #region "Pacer Simulator"

        public void PacerSimulator(int jobTimeMin, int jobTimeMax, int maxMessages, int paceMilliseconds)
        {
            var jobs = new Dictionary<int, JobInfo>();

            var agent = new PacerAgent(new BlitzkriegSoftware.Pacing.Library.Models.RedisConfiguration(), KeyPrefix);
            var intervalMS = new TimeSpan(0, 0, 0, 0, paceMilliseconds);

            _testContext.WriteLine($"Pacing at {paceMilliseconds} ms.\n");

            for (int m = 0; m < maxMessages; m++)
            {
                _testContext.WriteLine($"\nLoop: {(m + 1)}");

                #region "Pacing"
                // Pacing (wait until pace interval)
                while (!agent.Runnable(KeySuffix)) ;
                // Reset pacing
                agent.MarkPacing(KeySuffix, intervalMS);
                #endregion

                #region "Unit of Work"

#pragma warning disable SCS0005 // Weak random generator (not a problem here)
                var td = dice.Next(jobTimeMin, jobTimeMax);
#pragma warning restore SCS0005 // Weak random generator

                var job = new JobInfo(m, td);
                jobs.Add(m, job);
                #endregion

                #region "Show Job Status"
                foreach (var j in jobs.Values)
                {
                    if (j.Status == JobStatus.Dead)
                    {
                        jobs.Remove(j.Id);
                    }
                    else
                    {
                        _testContext.WriteLine("\t" + j.Describe());
                    }
                }
                #endregion
            }
        }

        [TestMethod]
        [TestCategory("Demo")]
        public void DemoPacer1()
        {
            const int MinProcessingMilliseconds = 100;
            const int MaxProcessingMilliseconds = 1000;
            const int MaxSimulatedMessages = 50;
            const int IntervalMilliseconds = 200;

            PacerSimulator(MinProcessingMilliseconds, MaxProcessingMilliseconds, MaxSimulatedMessages, IntervalMilliseconds);
        }

        [TestMethod]
        [TestCategory("Demo")]
        public void DemoPacer2()
        {
            const int MinProcessingMilliseconds = 100;
            const int MaxProcessingMilliseconds = 200;
            const int MaxSimulatedMessages = 50;
            const int IntervalMilliseconds = 50;

            PacerSimulator(MinProcessingMilliseconds, MaxProcessingMilliseconds, MaxSimulatedMessages, IntervalMilliseconds);
        }

        [TestMethod]
        [TestCategory("Demo")]
        public void DemoPacer3()
        {
            const int MinProcessingMilliseconds = 50;
            const int MaxProcessingMilliseconds = 100;
            const int MaxSimulatedMessages = 50;
            const int IntervalMilliseconds = 300;

            PacerSimulator(MinProcessingMilliseconds, MaxProcessingMilliseconds, MaxSimulatedMessages, IntervalMilliseconds);
        }

        #endregion

    }
}
