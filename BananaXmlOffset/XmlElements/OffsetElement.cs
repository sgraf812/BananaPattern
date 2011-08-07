using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace BananaXmlOffset.XmlElements
{
    internal class OffsetElement : XElementWrapper
    {
        public virtual string Name
        {
            get
            {
                return GetElementValue("Name").Trim();
            }
            set
            {
                SetElement("Name", value);
            }
        }
        public virtual OperationResult RootOperation
        {
            get
            {
                XElement operationsElement = Element.Elements("Operations").FirstOrDefault();

                if (operationsElement == null)
                {
                    return CreateImplicitPatternResult();
                }

                XElement operationElement = operationsElement.Elements().First();
                return OperationResult.Create(operationElement);
            }
            set
            {
                XElement operationsElement = Element.Elements("Operations").FirstOrDefault();

                if (operationsElement == null)
                {
                    operationsElement = new XElement("Operations", value.Element);
                    Element.Element("Name").AddAfterSelf(operationsElement);
                }
                else
                {
                    if (value == null)
                    {
                        operationsElement.Remove();
                    }
                    else
                    {
                        operationsElement.ReplaceNodes(value.Element);
                    }
                }
            }
        }

        private PatternResult CreateImplicitPatternResult()
        {
            var patternEle = Element.Parent.Elements("Pattern")
                .Select(ele => new PatternElement(ele))
                .FirstOrDefault(ele => ele.Name == Name);

            if (patternEle == null) return null;

            var result = new ImplicitPatternResult { Name = Name };
            result.SetPatternElement(patternEle);
            return result;
        }

        public OffsetElement()
            : base(new XElement("Offset",
                new XElement("Name", "")))
        {
        }

        public OffsetElement(XElement element)
            : base(element)
        {
        }

        private class ImplicitPatternResult : PatternResult
        {
            public override string Name { get; set; }
            private PatternElement _patternElement;
            protected override PatternElement PatternElement { get { return _patternElement; } }

            public ImplicitPatternResult()
                : base(null)
            {
            }

            public void SetPatternElement(PatternElement patternElement)
            {
                _patternElement = patternElement;
            }
        }
    }
}
