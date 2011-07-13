using System;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using BananaPattern;
using BananaPattern.Operators;
using BananaXmlOffset.XmlElements;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace BananaTest.Tests.XmlOffsets.XmlElements
{
    [TestClass]
    public class OperatorResultTest : XElementWrapperTestBase
    {
        [TestMethod]
        public void DefaultConstructor_InnerOperation_ReturnsNull()
        {
            OperatorResult result = new OperatorResult();

            Assert.IsNull(result.InnerOperation);
        }

        [TestMethod]
        public void DefaultConstructor_TypeAndValueAttributes_AreEmpty()
        {
            OperatorResult result = new OperatorResult();

            string type = result.Element.Attribute("Type").Value;
            string value = result.Element.Attribute("Value").Value;

            Assert.AreEqual("", type);
            Assert.AreEqual("", value);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetOperator_TypeElementIsEmpty_Throws()
        {
            OperatorResult result = new OperatorResult();

            result.GetOperator(null);
        }

        [TestMethod]
        public void GetOperator_TypeElementIsPresent_ReturnsLateBoundOperator()
        {
            XElement element = CreateOperatorResultXElement(null);

            OperatorResult result = new OperatorResult(element);

            Operator op = result.GetOperator(null);

            Assert.IsNotNull(op);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SetOperator_InvalidOperatorType_Throws()
        {
            OperatorResult result = new OperatorResult();

            result.SetOperator(typeof(int), "");
        }

        [TestMethod]
        public void SetOperator_GenericOverload_SetTypeElement()
        {
            OperatorResult result = new OperatorResult();

            result.SetOperator<AddOperator>("4");

            string actual = result.Element.Attribute("Type").Value;

            Assert.AreEqual("Add", actual);
        }

        [TestMethod]
        public void InnerOperation_Get_ReadsInnerElement()
        {
            string expected = "Hi!";
            XElement innerResult = CreatePatternResultXElement(expected);
            XElement element = CreateOperatorResultXElement(innerResult);
            OperatorResult result = new OperatorResult(element);

            PatternResult innerOperation = result.InnerOperation as PatternResult;

            Assert.IsNotNull(innerOperation);
            Assert.AreEqual(expected, innerOperation.Name);
            Assert.AreEqual(innerResult, innerOperation.Element);
        }

        [TestMethod]
        public void InnerOperation_Set_ReplacesInnerElement()
        {
            XElement innerResult = CreatePatternResultXElement("some name");
            XElement element = CreateOperatorResultXElement(innerResult);
            OperatorResult result = new OperatorResult(element);

            result.InnerOperation = new ConstantResult { Value = "4" };

            ConstantResult innerOperation = result.InnerOperation as ConstantResult;

            Assert.IsNotNull(innerOperation);
            Assert.AreNotEqual(innerResult.Parent, result.Element);
            Assert.AreEqual("4", innerOperation.Value);
        }

        [TestMethod]
        public void Execute_ExecutesOperator()
        {
            string expected = "0";
            TestOperatorResult result = new TestOperatorResult();
            result.InnerOperation = new ConstantResult { Value = "x" };
            Mock<IBotProcessContext> contextMock = new Mock<IBotProcessContext>();
            contextMock.Setup(x => x.TargetProcess).Returns(Process.GetCurrentProcess());
            StubOperator op = new StubOperator();
            StubOperator.RegisterOperator("Stub", typeof(StubOperator));

            result.Operator = op;
            string actual = result.Execute(contextMock.Object);
            StubOperator.UnregisterOperator("Stub");

            Assert.AreEqual(expected, actual);
            Assert.IsTrue(op.IsExecuted);
        }

        [TestMethod]
        public void ToBinaryOperatorResult_Target_IsSame()
        {
            string expected = "4";
            Mock<IBotProcessContext> contextMock = new Mock<IBotProcessContext>();
            OperatorResult result = new OperatorResult
            {
                InnerOperation = new ConstantResult { Value = expected }
            };
            result.SetOperator<AddOperator>("8");

            BinaryOperatorResult binary = result.ToBinaryOperatorResult();

            string actual = binary.TargetOperation.Execute(contextMock.Object);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ToBinaryOperatorResult_Value_IsSame()
        {
            string expected = "8";
            Mock<IBotProcessContext> contextMock = new Mock<IBotProcessContext>();
            OperatorResult result = new OperatorResult
            {
                InnerOperation = new ConstantResult { Value = "4" }
            };
            result.SetOperator<AddOperator>(expected);

            BinaryOperatorResult binary = result.ToBinaryOperatorResult();

            string actual = binary.ValueOperation.Execute(contextMock.Object);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ToBinaryOperatorResult_OperatorType_IsSame()
        {
            Mock<IBotProcessContext> contextMock = new Mock<IBotProcessContext>();
            OperatorResult result = new OperatorResult
            {
                InnerOperation = new ConstantResult { Value = "4" }
            };
            result.SetOperator<AddOperator>("8");

            BinaryOperatorResult binary = result.ToBinaryOperatorResult();

            Operator op = binary.GetOperator(null);

            Assert.IsInstanceOfType(op, typeof(AddOperator));
        }

        [TestMethod]
        public void ToBinaryOperatorResult_ParentElement_ContainsOnlyBinaryResult()
        {
            Mock<IBotProcessContext> contextMock = new Mock<IBotProcessContext>();
            OperatorResult result = new OperatorResult
            {
                InnerOperation = new ConstantResult { Value = "4" }
            };
            result.SetOperator<AddOperator>("8");
            XElement parent = new XElement("Target", result.Element);

            BinaryOperatorResult binary = result.ToBinaryOperatorResult();

            bool containsOperatorResult = parent.Elements("OperatorResult").Any();
            bool containsBinaryOperatorResult = parent.Elements("BinaryOperatorResult").Any();

            Assert.IsFalse(containsOperatorResult);
            Assert.IsTrue(containsBinaryOperatorResult);
        }

        [TestMethod]
        public void ToBinaryOperatorResult_Elements_AreNotEqual()
        {
            Mock<IBotProcessContext> contextMock = new Mock<IBotProcessContext>();
            OperatorResult result = new OperatorResult
            {
                InnerOperation = new ConstantResult { Value = "4" }
            };
            result.SetOperator<AddOperator>("8");
            XElement parent = new XElement("Target", result.Element);

            BinaryOperatorResult binary = result.ToBinaryOperatorResult();

            bool containsOperatorResult = parent.Elements("OperatorResult").Any();
            bool containsBinaryOperatorResult = parent.Elements("BinaryOperatorResult").Any();

            Assert.AreNotEqual(result.Element, binary.Element);
        }

        private class TestOperatorResult : OperatorResult
        {
            public TestOperatorResult()
                : base()
            {
            }

            public Operator Operator { get; set; }

            public override Operator GetOperator(IBotProcessContext context)
            {
                return Operator;
            }
        }
    }
}
