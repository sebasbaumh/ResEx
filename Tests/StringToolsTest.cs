using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ResEx.Common;

namespace ResEx.Tests
{
    /// <summary>
    /// This is a test class for StringToolsTest and is intended
    /// to contain all StringToolsTest Unit Tests
    /// </summary>
    [TestClass]
    public class StringToolsTest
    {
        /// <summary>
        /// A test for CountInstances
        /// </summary>
        [TestMethod]
        public void CountInstancesTest()
        {
            const string Text = "Start - Come ye, therefore, let us go down, and there confound their tongue - End";

            var result = StringTools.CountInstances(Text, "the", StringComparison.Ordinal);
            Assert.AreEqual(3, result);

            result = StringTools.CountInstances(Text, "Start", StringComparison.Ordinal);
            Assert.AreEqual(1, result);

            result = StringTools.CountInstances(Text, "End", StringComparison.Ordinal);
            Assert.AreEqual(1, result);

            result = StringTools.CountInstances(Text, "notFound", StringComparison.Ordinal);
            Assert.AreEqual(0, result);

            result = StringTools.CountInstances(string.Empty, "notFound", StringComparison.Ordinal);
            Assert.AreEqual(0, result);

            result = StringTools.CountInstances(Text, ",", StringComparison.Ordinal);
            Assert.AreEqual(3, result);
        }
    }
}
