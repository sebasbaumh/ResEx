using System;
using System.IO;
using System.Text;
using System.Xml;
using ResEx.Core;
using ResEx.StandardAdapters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ResEx.StandardAdapters.Common;

namespace ResEx.Tests.Adapters.Xml
{
    /// <summary>
    /// This is a test class for XmlAdapterTest and is intended
    /// to contain all XmlAdapterTest Unit Tests
    /// </summary>
    [TestClass]
    [DeploymentItem("ResEx.StandardAdapters.dll")]
    [DeploymentItem(@"Adapters\Xml\CrossLoopStrings.en.xml")]
    [DeploymentItem(@"Adapters\Xml\CrossLoopStrings.it.xml")]
    [DeploymentItem(@"Adapters\Xml\CrossLoopStrings.ja.xml")]
    [DeploymentItem(@"Adapters\Xml\ResxConvertTo.xslt")]
    [DeploymentItem(@"Adapters\Xml\ResxConvertFrom.xslt")]
    public class XmlAdapterTest
    {
        /// <summary>
        /// A test for Xml2ResourceSet
        /// </summary>
        [TestMethod]
        public void Xml2ResourceSetTest()
        {
            var adapter = new ResXResourceBundleAdapter(new FileSystem());
            var bundle = adapter.Load("CrossLoopStrings.en.xml");

            Assert.IsTrue(bundle.ContainsCulture("en"));
            Assert.IsTrue(bundle.ContainsCulture("it"));
            Assert.IsTrue(bundle.ContainsCulture("ja"));

            var resourceSet = bundle["en"];
            Assert.AreEqual(173, resourceSet.Count);
            Assert.AreEqual("Connected to help", resourceSet["s3"].Value);
        }

        /// <summary>
        /// A test for Xml2ResourceSet
        /// </summary>
        [TestMethod]
        public void AddRemoveResourceSetWhenBaseNameContainsCultureInfoTest()
        {
            var adapter = new ResXResourceBundleAdapter(new FileSystem());
            const string fileName = "CrossLoopStrings.en.xml";
            var bundle = adapter.Load(fileName);

            var newResourceSet = bundle.Add("el");
            newResourceSet.Add("s1", new ResourceStringItem {Name = "s1", Value = "test"});

            adapter.Save(fileName, bundle);

            Assert.IsTrue(File.Exists("CrossLoopStrings.el.xml"));
        }

        /// <summary>
        /// A test for Xml2ResourceSet
        /// </summary>
        [TestMethod]
        public void ResourceSet2XmlTest()
        {
            // create a sample resource set
            var resourceSet = new ResourceSet("el-GR")
                                  {
                                      { "key1", new ResourceItem { Name = "key1", Value = "value1" } },
                                      { "key2", new ResourceItem { Name = "key2", Value = "value2" } },
                                      { "key3", new ResourceItem { Name = "key3", Value = "value3" } },
                                      { "key4", new ResourceItem { Name = "key4", Value = "value4" } },
                                      { "key5", new ResourceItem { Name = "key5", Value = "value5" } }
                                  };

            // do the actual job (convertion from resource set to xml memory stream)
            var stream = new MemoryStream();
            ResXResourceBundleAdapter_Accessor.ResourceSet2Resx(resourceSet, stream, "ResxConvertFrom.xslt");
            stream.Position = 0;    // move position of stream to beginning to be ready for reading later

            // write to temporary file for debugging
            Console.WriteLine(Encoding.Default.GetString(stream.ToArray()));

            // convert the xml memory stream into xml document to help assertion of the result
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(stream);

            // assert the result
            var rootNode = xmlDoc.SelectSingleNode("//VIPTunnel/strings");
            Assert.IsNotNull(rootNode);
            Assert.AreEqual(5, rootNode.ChildNodes.Count);
            Assert.IsNotNull(rootNode.SelectSingleNode("key1"));
            Assert.IsNotNull(rootNode.SelectSingleNode("key1").Attributes.GetNamedItem("text"));
            Assert.AreEqual("value1", rootNode.SelectSingleNode("key1").Attributes.GetNamedItem("text").Value);
        }
    }
}