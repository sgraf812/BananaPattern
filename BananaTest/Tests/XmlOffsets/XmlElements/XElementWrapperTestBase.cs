using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace BananaTest.Tests.XmlOffsets.XmlElements
{
    public abstract class XElementWrapperTestBase
    {
        protected static XElement CreatePatternXElement(string name, string pattern = "CC", XElement cachedElement = null)
        {
            return new XElement("Pattern",
                cachedElement,
                new XElement("Name", name),
                new XElement("Pattern", pattern));
        }

        protected static XElement CreateRootXElement(params XElement[] content)
        {
            return new XElement("Patterns", content);
        }

        protected static XElement CreateOffsetXElement(string name, XElement operationsElement = null)
        {
            return new XElement("Offset",
                new XElement("Name", name),
                operationsElement);
        }

        protected static XElement CreateOperationsXElementWithConstantResult(int result = 0)
        {
            return new XElement("Operations", 
                CreateConstantResultXElement(result.ToString("X")));
        }

        protected static XElement CreateConstantResultXElement(string value)
        {
            return new XElement("ConstantResult", value);
        }

        protected static XElement CreatePatternResultXElement(string name)
        {
            return new XElement("PatternResult",
                new XAttribute("Name", name));
        }

        protected static XElement CreateOperatorResultXElement(XElement innerOperation = null, string type = "Add", string value = "C")
        {
            return new XElement("OperatorResult",
                innerOperation,
                new XAttribute("Type", type),
                new XAttribute("Value", value));
        }

        protected static XElement CreateBinaryOperatorResultXElement(XElement targetOperation = null, XElement valueOperation = null, string type = "Add", XElement cachedElement = null)
        {
            return new XElement("BinaryOperatorResult",
                cachedElement,
                new XElement("Type", type),
                new XElement("Target", targetOperation),
                new XElement("Value", valueOperation));
        }

        protected static XElement CreateCachedXElement(string build, string value)
        {
            return new XElement("Cached",
                new XAttribute("Build", build),
                new XAttribute("Value", value));
        }
    }
}
