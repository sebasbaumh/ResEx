using System.IO;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ResEx.Core;
using ResEx.StandardAdapters;
using ResEx.StandardAdapters.Common;

namespace ResEx.Tests.Adapters
{
    /// <summary>
    /// this is a test class for ResXResourceBundleAdapterTest and is intended
    /// to contain all ResXResourceBundleAdapterTest Unit Tests
    /// </summary>
    [TestClass]
    [DeploymentItem(@"Adapters\Resx\ResXResourceBundleAdapterTest.Sample.resx")]
    [DeploymentItem(@"Adapters\Resx\ResXResourceBundleAdapterTest.Sample.el-GR.resx")]
    public class ResXResourceBundleAdapterTest
    {
        [TestMethod]
        public void ResXFile2ResourceSetTest()
        {
            var actual = new ResourceSet(ResourceSet.NeutralCulture);
            ResXResourceBundleAdapter_Accessor.Resx2ResourceSet(actual, "ResXResourceBundleAdapterTest.Sample.resx", null);

            Assert.AreEqual(15, actual.Values.Count);
            Assert.IsTrue(actual.ContainsKey("Ampersands"));
            Assert.IsTrue(actual.ContainsKey("MaxLengthString"));
            Assert.IsTrue(actual.ContainsKey("ReadOnlyString"));
            Assert.IsTrue(actual.ContainsKey("Set1_Placeholder1"));
            Assert.IsTrue(actual.ContainsKey("Set1_Placeholder2"));
            Assert.IsTrue(actual.ContainsKey("Set2"));
            Assert.IsTrue(actual.ContainsKey("Set2_SubString1"));
            Assert.IsTrue(actual.ContainsKey("Set2_SubString2"));
            Assert.IsTrue(actual.ContainsKey("String2Multiline"));
            Assert.IsTrue(actual.ContainsKey(">>FrameworkLocked"));
            Assert.IsTrue(actual.ContainsKey("Exclude1"));
            Assert.IsTrue(actual.ContainsKey("StringWithExclusions"));
            Assert.IsTrue(actual.ContainsKey("ItemWithoutValue"));

            Assert.AreEqual(4, actual.CountLocked());
            Assert.AreEqual(13, actual.CountStringItems(false));
            Assert.AreEqual(14, actual.CountStringItems(true));
            Assert.AreEqual(52, actual.CountWords());

            Assert.IsInstanceOfType(actual["MaxLengthString"], typeof(ResourceStringItem));
            var item = (ResourceStringItem)actual["MaxLengthString"];
            Assert.AreEqual(10, item.MaxLength);

            Assert.IsTrue(actual["ReadOnlyString"].Locked);
            Assert.AreEqual(LockedReason.DeveloperLock, actual["ReadOnlyString"].LockedReason);

            Assert.IsTrue(actual[">>FrameworkLocked"].Locked);
            Assert.AreEqual(LockedReason.FrameworkLock, actual[">>FrameworkLocked"].LockedReason);

            Assert.IsTrue(actual["Exclude1"].Locked);
            Assert.AreEqual(LockedReason.ResexMetadata, actual["Exclude1"].LockedReason);

            // exclusions
            var exclusions = actual.Exclusions;
            Assert.AreEqual(3, exclusions.Count);
            Assert.AreEqual(@"\{.*}", exclusions[1].Pattern);
            Assert.AreEqual(@"{Text}", exclusions[1].Sample);
            Assert.AreEqual(@"Windows", exclusions[2].Pattern);
        }

        [TestMethod]
        public void AddRemoveResourceSetWhenBaseNameDoesNotContainCultureInfoTest()
        {
            var adapter = new ResXResourceBundleAdapter(new FileSystem());
            const string fileName = "ResXResourceBundleAdapterTest.Sample.resx";
            var bundle = adapter.Load(fileName);

            var newResourceSet = bundle.Add("hi");
            newResourceSet.Add("s1", new ResourceStringItem { Name = "s1", Value = "test" });

            adapter.Save(fileName, bundle);

            Assert.IsTrue(File.Exists("ResXResourceBundleAdapterTest.Sample.hi.resx"));
        }

        [TestMethod]
        public void LoadAndSaveTest()
        {
            var adapter = new ResXResourceBundleAdapter(new FileSystem());
            var bundle = adapter.Load("ResXResourceBundleAdapterTest.Sample.resx");

            Assert.AreEqual(2, bundle.Count);
            Assert.IsTrue(bundle.ContainsCulture("neutral"));
            Assert.IsTrue(bundle.ContainsCulture("el-GR"));

            var baseResourceSet = bundle.NeutralOrFirst();
            var resourceSet = bundle["el-GR"];

            Assert.AreEqual(2, resourceSet.CountTranslatedItems(baseResourceSet));
            Assert.AreEqual(1, resourceSet.CountMarkedForReviewing());
            Assert.AreEqual(ResourceSetStatus.Unaffected, resourceSet.Status);

            // simulate that contents have been updated and save
            bundle["neutral"].Status = ResourceSetStatus.Updated;
            bundle["el-GR"].Status = ResourceSetStatus.Updated;
            adapter.Save("SaveTest.resx", bundle);

            // compare original and new files
            Assert.AreEqual(XmlReader.Create("ResXResourceBundleAdapterTest.Sample.resx").ReadOuterXml(), XmlReader.Create("SaveTest.resx").ReadOuterXml());
            Assert.AreEqual(XmlReader.Create("ResXResourceBundleAdapterTest.Sample.el-GR.resx").ReadOuterXml(), XmlReader.Create("SaveTest.el-GR.resx").ReadOuterXml());
        }

        /// <summary>
        ///A test for GetCultureInfoFromFileName
        ///</summary>
        [TestMethod]
        public void GetCultureInfoFromFileNameTest()
        {
            var extractor = new ResxExtractCultureFromFileStrategy();

            // typical case (name + culture + extension)
            Assert.AreEqual("el", extractor.GetCulture("Part1.el.xml"));
            Assert.AreEqual("el-GR", extractor.GetCulture("Part1.el-GR.xml"));

            // typical case (name + culture + extension)
            Assert.AreEqual("el", extractor.GetCulture("Part1.el.resx"));
            Assert.AreEqual("el-GR", extractor.GetCulture("Part1.el-GR.resx"));

            // typical case (nameWithDots + culture + extension)
            Assert.AreEqual("el", extractor.GetCulture("Part1.Part2.el.resx"));
            Assert.AreEqual("el-GR", extractor.GetCulture("Part1.Part2.el-GR.resx"));

            // case with four letters in country name (zh-Hans)
            Assert.AreEqual("zh-Hans", extractor.GetCulture("Part1.Part2.zh-Hans.resx"));

            // case with 2 dashes in culture (ha-Latn-NG)
            ////Assert.AreEqual("ha-Latn-NG", extractor.GetCulture("Part1.Part2.ha-Latn-NG.resx"));

            // file with no culture information
            Assert.AreEqual(ResourceSet.NeutralCulture, extractor.GetCulture("Part1.resx"));
            Assert.AreEqual(ResourceSet.NeutralCulture, extractor.GetCulture("Part1.Part2.resx"));
        }
    }
}