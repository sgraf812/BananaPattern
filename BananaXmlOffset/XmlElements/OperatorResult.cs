using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using BananaPattern.Operators;
using BananaPattern;

namespace BananaXmlOffset.XmlElements
{
    internal class OperatorResult : OperationResult
    {
        public virtual OperationResult InnerOperation
        {
            get
            {
                XElement innerOperation = Element.Elements().FirstOrDefault();
                if (innerOperation == null) return null;
                return OperationResult.Create(innerOperation);
            }
            set
            {
                Element.ReplaceNodes(value.Element);
            }
        }

        public OperatorResult()
            : base(new XElement("OperatorResult",
                new XAttribute("Type", ""),
                new XAttribute("Value", "")))
        {
        }

        public OperatorResult(XElement element)
            : base(element)
        {
        }

        public override string Execute(IBotProcessContext context)
        {
            Operator op = GetOperator(context);
            return op.Execute(context.Memory).ToString("X");
        }

        public virtual Operator GetOperator(IBotProcessContext context)
        {
            string identifier = GetAttributeValue("Type"); 
            if (string.IsNullOrEmpty(identifier))
            {
                throw new InvalidOperationException("BinaryOperatorResult wasn't proper initialized. " +
                    "The operator type identifier was empty.");
            }
            Func<string> targetFactory = () => InnerOperation.Execute(context);
            Func<string> valueFactory = () => GetAttributeValue("Value");
            return Operator.Create(identifier, valueFactory, targetFactory);
        }

        public virtual void SetOperator(Type typeOfOperator, string value)
        {
            string identifier = Operator.GetRegisteredIdentifier(typeOfOperator);
            if (string.IsNullOrEmpty(identifier))
            {
                throw new InvalidOperationException("The type identifier of " + typeOfOperator.FullName +
                    " was not present in the Operator type map.");
            }

            SetAttribute("Type", identifier);
            SetAttribute("Value", value);
        }

        public virtual void SetOperator<T>(string value) where T : Operator
        {
            SetOperator(typeof(T), value);
        }

        public virtual BinaryOperatorResult ToBinaryOperatorResult()
        {
            BinaryOperatorResult binary = new BinaryOperatorResult();

            binary.TargetOperation = InnerOperation;
            binary.ValueOperation = new ConstantResult { Value = GetAttributeValue("Value") };
            binary.SetOperatorType(GetOperator(null));

            if (Element.Parent != null) Element.Parent.ReplaceNodes(binary.Element);

            return binary;
        }
    }
}
