using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Diagnostics;
using BananaPattern;

namespace BananaXmlOffset.XmlElements
{
    internal abstract class OperationResult : XElementWrapper
    {
        private enum OperationType
        {
            BinaryOperatorResult,
            OperatorResult,
            ConstantResult,
            PatternResult,
        }

        public OperationResult(XElement element)
            : base(element)
        {
        }

        public abstract string Execute(IBotProcessContext context);

        public static OperationResult Create(XElement element)
        {
            OperationType type = ParseTypeString(element.Name.LocalName);
            switch (type)
            {
                case OperationType.BinaryOperatorResult:
                    return new BinaryOperatorResult(element);

                case OperationType.OperatorResult:
                    return new OperatorResult(element);

                case OperationType.ConstantResult:
                    return new ConstantResult(element);

                case OperationType.PatternResult:
                    return new PatternResult(element);

                default:
                    throw new OffsetException("Invalid Operation element type " + type + ".");
            }
        }

        private static OperationType ParseTypeString(string typeString)
        {
            OperationType type;
            if (Enum.TryParse(typeString, out type))
            {
                return type;
            }
            else
            {
                throw new OffsetException("Operation element type " + typeString + " could not be parsed.");
            }
        }
    }
}
