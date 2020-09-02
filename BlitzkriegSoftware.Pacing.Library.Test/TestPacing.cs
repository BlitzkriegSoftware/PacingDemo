using System;
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
            var agent = new PacerAgent(new Models.RedisConfiguration(), KeyPrefix);

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
            var agent = new PacerAgent(new Models.RedisConfiguration(), KeyPrefix);

            Assert.IsTrue(agent.IsValid);

            var ts = new TimeSpan(0, 0, 0, 1, 0);

            agent.MarkPacing(KeySuffix, ts);

            var runnable = agent.Runnable(KeySuffix);

            Assert.IsFalse(runnable);
        }

    }
}
