using Microsoft.VisualStudio.TestTools.UnitTesting;
using ResEx.Core;

namespace ResEx.Tests
{
    [TestClass]
    public class ExclusionManagerTest
    {
        [TestMethod]
        public void ExcludeRestoreTest()
        {
            // set up
            const string originalText = "Windows is an operating system created by Microsoft, which was founded by Bill Gates";
            
            var exclusionManager = new ExclusionManager();
            exclusionManager.ExclusionPatterns.Add(new Exclusion("Windows"));    // We want these words not to be translated
            exclusionManager.ExclusionPatterns.Add(new Exclusion("Gates"));    // We want these words not to be translated

            // run 1
            var textWithExclusions = exclusionManager.Exclude(originalText);

            // assert 1
            Assert.AreEqual("[### 0 ###] is an operating system created by Microsoft, which was founded by Bill [### 1 ###]", textWithExclusions);

            // run 2
            var restoredText = exclusionManager.Restore(textWithExclusions);

            // assert 2
            Assert.AreEqual(originalText, restoredText);
        }

        [TestMethod]
        public void ValidateTest()
        {
            // set up
            const string originalText = "Windows is an operating system created by Microsoft, which was founded by Bill Gates";
            const string correctTranslation = "Τα Windows είναι ένα λειτουργικό σύστημα που δημιουργήθηκε από την Microsoft, η οποία ιδρύθηκε από τον Bill Gates";
            const string incorrectTranslation = "Τα Παράθυρα είναι ένα λειτουργικό σύστημα που δημιουργήθηκε από την Microsoft, η οποία ιδρύθηκε από τον Bill Πύλες";

            var exclusionManager = new ExclusionManager();
            exclusionManager.ExclusionPatterns.Add(new Exclusion("Windows"));    // We want these words not to be translated
            exclusionManager.ExclusionPatterns.Add(new Exclusion("Gates"));    // We want these words not to be translated

            // correct case
            var actual = exclusionManager.Validate(correctTranslation, originalText);
            Assert.AreEqual(0, actual.Length);
            
            // incorrect case
            actual = exclusionManager.Validate(incorrectTranslation, originalText);
            Assert.AreEqual(2, actual.Length);
            Assert.AreEqual("Windows", actual[0]);
            Assert.AreEqual("Gates", actual[1]);
        }
    }
}