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
    public class BinaryOperatorResultTest : XElementWrapperTestBase
    {
        [TestMethod]
        public void DefaultConstructor_Operations_AreNull()
        {
            BinaryOperatorResult result = new BinaryOperatorResult();

            Assert.IsNull(result.TargetOperation);
            Assert.IsNull(result.ValueOperation);
        }

        [TestMethod]
        public void DefaultConstructor_TypeElement_IsEmpty()
        {
            BinaryOperatorResult result = new BinaryOperatorResult();

            string type = result.Element.Element("Type").Value;

            Assert.AreEqual("", type);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetOperator_TypeElementIsEmpty_Throws()
        {
            BinaryOperatorResult result = new BinaryOperatorResult();

            result.GetOperator(null);
        }

        [TestMethod]
        public void GetOperator_TypeElementIsPresent_ReturnsLateBoundOperator()
        {
            XElement element = CreateBinaryOperatorResultXElement();

            BinaryOperatorResult result = new BinaryOperatorResult(element);

            Operator op = result.GetOperator(null);

            Assert.IsNotNull(op);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SetOperatorType_InvalidOperatorType_Throws()
        {
            BinaryOperatorResult result = new BinaryOperatorResult();

            result.SetOperatorType(typeof(int));
        }

        [TestMethod]
        public void SetOperatorType_GenericOverload_SetTypeElement()
        {
            BinaryOperatorResult result = new BinaryOperatorResult();

            result.SetOperatorType<AddOperator>();

            string actual = result.Element.Element("Type").Value;

            Assert.AreEqual("Add", actual);
        }

        [TestMethod]
        public void SetOperatorType_OperatorOverload_SetTypeElement()
        {
            BinaryOperatorResult result = new BinaryOperatorResult();

            result.SetOperatorType(new AddOperator());

            string actual = result.Element.Element("Type").Value;

            Assert.AreEqual("Add", actual);
        }

        [TestMethod]
        public void TargetOperation_Get_ReadsTargetElement()
        {
            string expected = "some name";
            XElement innerResult = CreatePatternResultXElement(expected);
            XElement element = CreateBinaryOperatorResultXElement(innerResult, innerResult);

            BinaryOperatorResult result = new BinaryOperatorResult(element);
            PatternResult operation = result.TargetOperation as PatternResult;

            Assert.IsNotNull(operation);
            Assert.AreEqual(expected, operation.Name);
            Assert.AreEqual(innerResult.ToString(), operation.Element.ToString());
        }

        [TestMethod]
        public void ValueOperation_Get_ReadsTargetElement()
        {
            string expected = "some name";
            XElement innerResult = CreatePatternResultXElement(expected);
            XElement element = CreateBinaryOperatorResultXElement(innerResult, innerResult);

            BinaryOperatorResult result = new BinaryOperatorResult(element);
            PatternResult operation = result.ValueOperation as PatternResult;

            Assert.IsNotNull(operation);
            Assert.AreEqual(expected, operation.Name);
            Assert.AreEqual(innerResult.ToString(), operation.Element.ToString());
        }

        [TestMethod]
        public void TargetOperation_Set_ReplacesTargetElementContent()
        {
            string expected = "some name";
            XElement innerResult = CreatePatternResultXElement(expected);
            XElement element = CreateBinaryOperatorResultXElement(innerResult, innerResult);

            BinaryOperatorResult result = new BinaryOperatorResult(element);
            result.TargetOperation = new ConstantResult { Value = "4" };

            XElement targetElement = element.Element("Target");

            Assert.IsFalse(targetElement.Elements("PatternResult").Any());
            Assert.AreEqual("4", targetElement.Element("ConstantResult").Value);
        }

        [TestMethod]
        public void ValueOperation_Set_ReplacesValueElementContent()
        {
            string expected = "some name";
            XElement innerResult = CreatePatternResultXElement(expected);
            XElement element = CreateBinaryOperatorResultXElement(innerResult, innerResult);

            BinaryOperatorResult result = new BinaryOperatorResult(element);
            result.ValueOperation = new ConstantResult { Value = "4" };

            XElement valueElement = element.Element("Value");

            Assert.IsFalse(valueElement.Elements("PatternResult").Any());
            Assert.AreEqual("4", valueElement.Element("ConstantResult").Value);
        }

        [TestMethod]
        public void Execute_ExecutesOperator()
        {
            string expected = "0";
            TestBinaryOperatorResult result = new TestBinaryOperatorResult();
            result.TargetOperation = new ConstantResult { Value = "x" };
            result.ValueOperation = new ConstantResult { Value = "x" };
            Mock<IBotProcessContext> contextMock = new Mock<IBotProcessContext>();
            contextMock.Setup(x => x.TargetProcess).Returns(Process.GetCurrentProcess());
            StubOperator op = new StubOperator();

            result.Operator = op;
            string actual = result.Execute(contextMock.Object);

            Assert.AreEqual(expected, actual);
            Assert.IsTrue(op.IsExecuted);
        }

        private class TestBinaryOperatorResult : BinaryOperatorResult
        {
            public Operator Operator { get; set; }

            public override Operator GetOperator(IBotProcessContext context)
            {
                return Operator;
            }
        }
    }
}
