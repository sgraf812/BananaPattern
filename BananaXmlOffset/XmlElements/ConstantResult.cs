using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using BananaPattern;

namespace BananaXmlOffset.XmlElements
{
    internal class ConstantResult : OperationResult
    {
        public virtual string Value
        {
            get
            {
                return Element.Value;
            }
            set
            {
                Element.SetValue(value);
            }
        }

        public ConstantResult()
            : base(new XElement("ConstantResult", ""))
        {
        }

        public ConstantResult(XElement element)
            : base(element)
        {
        }

        public override string Execute(IBotProcessContext context)
        {
            return Value;
        }
    }
}
