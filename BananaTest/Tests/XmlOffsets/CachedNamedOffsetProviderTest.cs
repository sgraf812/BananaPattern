using System;
using BananaXmlOffset;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BananaTest.Tests.XmlOffsets
{
    [TestClass]
    public class CachedNamedOffsetProviderTest
    {
        [TestMethod]
        public void GetAddress_NotCached_CalculateAddressIsCalled()
        {
            IntPtr expected = new IntPtr(4);
            TestCachedNamedOffsetProvider provider = new TestCachedNamedOffsetProvider();
            provider.ReturnValue = expected;

            IntPtr actual = provider.GetAddress("not cached");

            Assert.AreEqual(expected, actual);
            Assert.IsTrue(provider.CalculateAddressWasCalled);
        }

        [TestMethod]
        public void GetAddress_AddResultToCacheFalse_CalculateAddressIsCalled()
        {
            IntPtr expected = new IntPtr(4);
            TestCachedNamedOffsetProvider provider = new TestCachedNamedOffsetProvider();
            provider.ReturnValue = expected;
            provider.AddResultToCache = false;
            provider.GetAddress("not cached");
            provider.CalculateAddressWasCalled = false;

            IntPtr actual = provider.GetAddress("not cached");

            Assert.AreEqual(expected, actual);
            Assert.IsTrue(provider.CalculateAddressWasCalled);
        }

        [TestMethod]
        public void GetAddress_AddResultToCacheTrue_CalculateAddressIsNotCalled()
        {
            IntPtr expected = new IntPtr(4);
            TestCachedNamedOffsetProvider provider = new TestCachedNamedOffsetProvider();
            provider.ReturnValue = expected;
            provider.AddResultToCache = true;
            provider.GetAddress("cached");
            provider.CalculateAddressWasCalled = false;

            IntPtr actual = provider.GetAddress("cached");

            Assert.AreEqual(expected, actual);
            Assert.IsFalse(provider.CalculateAddressWasCalled);
        }

        [TestMethod]
        public void CanResolve_NotCached_ReturnsFalse()
        {
            TestCachedNamedOffsetProvider provider = new TestCachedNamedOffsetProvider();

            Assert.IsFalse(provider.CanResolve("not cached"));
        }

        [TestMethod]
        public void CanResolve_AddResultToCacheFalse_ReturnsFalse()
        {
            TestCachedNamedOffsetProvider provider = new TestCachedNamedOffsetProvider();
            provider.AddResultToCache = false;
            provider.GetAddress("not cached");
            provider.CalculateAddressWasCalled = false;

            Assert.IsFalse(provider.CanResolve("not cached"));
        }

        [TestMethod]
        public void CanResolve_AddResultToCacheTrue_ReturnsTrue()
        {
            TestCachedNamedOffsetProvider provider = new TestCachedNamedOffsetProvider();
            provider.AddResultToCache = true;
            provider.GetAddress("cached");
            provider.CalculateAddressWasCalled = false;

            Assert.IsTrue(provider.CanResolve("cached"));
        }

        private class TestCachedNamedOffsetProvider : CachedNamedOffsetProvider
        {
            public bool AddResultToCache { get; set; }
            public IntPtr ReturnValue { get; set; }

            public string LastName { get; set; }
            public bool CalculateAddressWasCalled { get; set; }

            protected override IntPtr CalculateAddress(string name, out bool addResultToCache)
            {
                addResultToCache = AddResultToCache;
                CalculateAddressWasCalled = true;
                return ReturnValue;
            }
        }
    }
}
