using System.Xml.Linq;
using BananaXmlOffset.XmlElements;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BananaTest.Tests.XmlOffsets.XmlElements
{
    [TestClass]
    public class ConstantResultTest : XElementWrapperTestBase
    {
        [TestMethod]
        public void DefaultConstructor_Value_IsEmpty()
        {
            ConstantResult result = new ConstantResult();

            Assert.AreEqual("", result.Value);
        }

        [TestMethod]
        public void Value_Get_ReadsValueElement()
        {
            string expected = "Hi!";
            XElement element = CreateConstantResultXElement(expected);
            ConstantResult result = new ConstantResult(element);

            Assert.AreEqual(expected, result.Value);
        }

        [TestMethod]
        public void Value_Set_SetsValueElement()
        {
            string expected = "Hi!";
            ConstantResult result = new ConstantResult
            {
                Value = expected
            };

            string actual = result.Element.Value;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Execute_RegardlessOfContext_ReturnsValue()
        {
            string expected = "Hi!";
            ConstantResult result = new ConstantResult
            {
                Value = expected
            };

            Assert.AreEqual(expected, result.Execute(null));
        }
    }
}
