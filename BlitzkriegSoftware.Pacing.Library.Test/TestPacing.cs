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


        [TestMethod]
        [TestCategory("Integration")]
        public void TestPacer1()
        {
        }
    }
}
