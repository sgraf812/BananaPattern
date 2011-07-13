using System.Xml.Linq;
using BananaXmlOffset;
using BananaXmlOffset.XmlElements;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BananaTest.Tests.XmlOffsets.XmlElements
{
    [TestClass]
    public class OperationResultTest
    {
        [TestMethod]
        [ExpectedException(typeof(OffsetException))]
        public void Create_InvalidTypeString_Throws()
        {
            OperationResult.Create(new XElement("invalid"));
        }

        [TestMethod]
        public void Create_ConstantResult_ReturnsConstantResult()
        {
            XElement element = new XElement("ConstantResult");

            var operation = OperationResult.Create(element) as ConstantResult;

            Assert.IsNotNull(operation);
        }

        [TestMethod]
        public void Create_PatternResult_ReturnsPatternResult()
        {
            XElement element = new XElement("PatternResult");

            var operation = OperationResult.Create(element) as PatternResult;

            Assert.IsNotNull(operation);
        }

        [TestMethod]
        public void Create_OperatorResult_ReturnsOperatorResult()
        {
            XElement element = new XElement("OperatorResult");

            var operation = OperationResult.Create(element) as OperatorResult;

            Assert.IsNotNull(operation);
        }

        [TestMethod]
        public void Create_BinaryOperatorResult_ReturnsBinaryOperatorResult()
        {
            XElement element = new XElement("BinaryOperatorResult");

            var operation = OperationResult.Create(element) as BinaryOperatorResult;

            Assert.IsNotNull(operation);
        }
    }
}
