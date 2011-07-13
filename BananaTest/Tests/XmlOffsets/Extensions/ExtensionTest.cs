using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.InteropServices;
using BananaXmlOffset.Extensions;

namespace BananaTest.Tests.XmlOffsets.Extensions
{
    [TestClass]
    public class ExtensionTest
    {
        [TestMethod]
        public void GetInt32OffsetFrom_Scenario_ExpectedBehaviour()
        {
            IntPtr from = new IntPtr(0x1000);
            IntPtr to = new IntPtr(0x2000);

            int expected = 0x1000;
            int actual = to.GetInt32OffsetFrom(from);

            Assert.AreEqual(expected, actual);
        }
    }
}
