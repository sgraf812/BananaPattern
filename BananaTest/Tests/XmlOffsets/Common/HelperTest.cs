using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using BananaXmlOffset.Common;

namespace BananaTest.Tests.XmlOffsets.Common
{
    [TestClass]
    public class HelperTest
    {
        [TestMethod]
        public void GetAssemblyRootedPath_CalledWithArbitraryFileName_RootsToExecutingAssemblyDirectory()
        {
            string relativePath = "someFile.data";
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            string expectedRoot = Path.GetDirectoryName(executingAssembly.Location);

            string expected = Path.Combine(expectedRoot, relativePath);

            string actual = Helper.GetAssemblyRootedPath(relativePath);

            Assert.AreEqual(expected, actual);
        }
    }
}
