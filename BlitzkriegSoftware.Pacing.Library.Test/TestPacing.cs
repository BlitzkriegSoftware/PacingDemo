using System;
using BlitzkriegSoftware.Pacing.Library.Test.Helpers;
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

        public const string KeyPrefix = "test";
        public const string KeySuffix = "Pace1";

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

        [TestMethod]
        [TestCategory("Demo")]
        public void DemoPacer1()
        {
            const int MinProcessingMilliseconds = 100;
            const int MaxProcessingMilliseconds = 1000;
            const int MaxSimulatedMessages = 50;
            const int IntervalMilliseconds = 200;
            
            var dice = new Random();

            var provider = new JobTracker();
            var reporter = new JobReporter("Tester", _testContext);
            reporter.Subscribe(provider);

            var agent = new PacerAgent(new BlitzkriegSoftware.Pacing.Library.Models.RedisConfiguration(), KeyPrefix);
            var intervalMS = new TimeSpan(0, 0, 0, 0, IntervalMilliseconds);

            _testContext.WriteLine($"Pacing at {IntervalMilliseconds} milliseconds.\n");

            for (int m = 0; m < MaxSimulatedMessages; m++)
            {
                // Pacing
                while (!agent.Runnable(KeySuffix)) ;

                #region "Unit of Work"
                var td = dice.Next(MinProcessingMilliseconds, MaxProcessingMilliseconds);
                _testContext.WriteLine($"Starting {m} for {td} milliseconds at {DateTime.UtcNow:mm-ss-ffff}");
                var job = new JobInfo(m, td);
                provider.TrackJob(job);
                #endregion

                // Show all jobs
                foreach (var j in provider.Jobs.Values)
                {
                    if(!j.JobDone)
                    {
                        _testContext.WriteLine($"\t{j.Id} working for {j.JobDuration}, quit: {j.QuitTime:mm-ss-ffff}");
                    }
                }

                // Reset pacing
                agent.MarkPacing(KeySuffix, intervalMS);
            }

            provider.EndObserve();
        }


    }
}
