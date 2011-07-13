using System.Xml.Linq;
using BananaXmlOffset.XmlElements;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BananaTest.Tests.XmlOffsets.XmlElements
{
    [TestClass]
    public class XElementWrapperTest : XElementWrapperTestBase
    {
        private TestableXElementWrapper _wrapper;

        [TestMethod]
        public void Constructor_InitializesElement()
        {
            XElement element = new XElement("Element");
            _wrapper = new TestableXElementWrapper(element);

            Assert.AreEqual(element, _wrapper.Element);
        }

        [TestMethod]
        public void GetAttributeValue_ReturnsAttachedAttributeValue()
        {
            string expected = "Hi!";
            XElement element = new XElement("Element",
                new XAttribute("Attribute", "Hi!"));
            _wrapper = new TestableXElementWrapper(element);

            string actual = _wrapper.GetAttributeValue("Attribute");

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetElementValue_ReturnsNestedElementValue()
        {
            string expected = "Hi!";
            XElement element = new XElement("Element",
                new XElement("Inner", "Hi!"));
            _wrapper = new TestableXElementWrapper(element);

            string actual = _wrapper.GetElementValue("Inner");

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void SetAttribute_SetsAttachedAttributeValue()
        {
            string expected = "Hi!";
            XElement element = new XElement("Element",
                new XAttribute("Attribute", ""));
            _wrapper = new TestableXElementWrapper(element);

            _wrapper.SetAttribute("Attribute", expected);
            string actual = _wrapper.GetAttributeValue("Attribute");

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void SetElement_SetsInnerElementValue()
        {
            string expected = "Hi!";
            XElement element = new XElement("Element",
                new XElement("Inner", ""));
            _wrapper = new TestableXElementWrapper(element);

            _wrapper.SetElement("Inner", expected);
            string actual = _wrapper.GetElementValue("Inner");

            Assert.AreEqual(expected, actual);
        }

        private class TestableXElementWrapper : XElementWrapper
        {
            public TestableXElementWrapper(XElement element)
                : base(element)
            {
            }

            public new string GetAttributeValue(string name)
            {
                return base.GetAttributeValue(name);
            }

            public new string GetElementValue(string name)
            {
                return base.GetElementValue(name);
            }

            public new void SetAttribute(string name, object value)
            {
                base.SetAttribute(name, value);
            }

            public new void SetElement(string name, object value)
            {
                base.SetElement(name, value);
            }
        }
    }
}
