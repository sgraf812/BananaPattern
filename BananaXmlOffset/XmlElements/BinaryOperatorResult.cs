using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using BananaPattern;
using BananaPattern.Operators;

namespace BananaXmlOffset.XmlElements
{
    internal class BinaryOperatorResult : OperationResult
    {
        public virtual OperationResult TargetOperation
        {
            get
            {
                XElement targetOperation = Element.Element("Target").Elements().FirstOrDefault();
                if (targetOperation == null) return null;
                return OperationResult.Create(targetOperation);
            }
            set
            {
                Element.Element("Target").ReplaceAll(value.Element);
            }
        }
        public virtual OperationResult ValueOperation
        {
            get
            {
                XElement valueOperation = Element.Element("Value").Elements().FirstOrDefault();
                if (valueOperation == null) return null;
                return OperationResult.Create(valueOperation);
            }
            set
            {
                Element.Element("Value").ReplaceAll(value.Element);
            }
        }

        public BinaryOperatorResult()
            : base(new XElement("BinaryOperatorResult",
                new XElement("Type", ""),
                new XElement("Value", ""),
                new XElement("Target", "")))
        {
        }

        public BinaryOperatorResult(XElement element)
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
            string identifier = GetElementValue("Type");
            if (string.IsNullOrEmpty(identifier))
            {
                throw new InvalidOperationException("BinaryOperatorResult wasn't proper initialized. " +
                    "The operator type identifier was empty.");
            }

            Func<string> targetFactory = () => TargetOperation.Execute(context);
            Func<string> valueFactory = () => ValueOperation.Execute(context);
            return Operator.Create(identifier, valueFactory, targetFactory);
        }

        public virtual void SetOperatorType(Type typeOfOperator)
        {
            string identifier = Operator.GetRegisteredIdentifier(typeOfOperator);
            if (string.IsNullOrEmpty(identifier))
            {
                throw new InvalidOperationException("The type identifier of " + typeOfOperator.FullName + 
                    " was not present in the Operator type map.");
            }

            SetElement("Type", identifier);
        }

        public virtual void SetOperatorType(Operator op)
        {
            SetOperatorType(op.GetType());
        }

        public virtual void SetOperatorType<T>() where T : Operator
        {
            SetOperatorType(typeof(T));
        }
    }
}
