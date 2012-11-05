using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using ResEx.Win;

namespace ResEx.Tests
{
    [TestClass]
    public class UpdateCheckerTest
    {
        [TestMethod]
        [DeploymentItem(@"UpdateCheckerFeed.xml")]
        public void CheckForUpdatesTest()
        {
            var fullyUpdated = new DateTime(2009, 12, 1);
            var hasLastReleaseButNotTheBeta = new DateTime(2009, 11, 1);
            var hasOldRelease = new DateTime(2009, 10, 1);

            Assert.AreEqual(UpdateChecker.UpdateCheckResult.Nothing, UpdateChecker.Check(@"UpdateCheckerFeed.xml", false, fullyUpdated));

            Assert.AreEqual(UpdateChecker.UpdateCheckResult.Nothing, UpdateChecker.Check(@"UpdateCheckerFeed.xml", true, fullyUpdated));

            Assert.AreEqual(UpdateChecker.UpdateCheckResult.Nothing, UpdateChecker.Check(@"UpdateCheckerFeed.xml", false, hasLastReleaseButNotTheBeta));

            Assert.AreEqual(UpdateChecker.UpdateCheckResult.NewBeta, UpdateChecker.Check(@"UpdateCheckerFeed.xml", true, hasLastReleaseButNotTheBeta));

            Assert.AreEqual(UpdateChecker.UpdateCheckResult.NewRelease, UpdateChecker.Check(@"UpdateCheckerFeed.xml", false, hasOldRelease));

            Assert.AreEqual(UpdateChecker.UpdateCheckResult.NewBeta, UpdateChecker.Check(@"UpdateCheckerFeed.xml", true, hasOldRelease));
        }
    }
}
