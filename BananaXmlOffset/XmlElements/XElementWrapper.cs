using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace BananaXmlOffset.XmlElements
{
    internal abstract class XElementWrapper
    {
        public virtual XElement Element { get; set; }

        public XElementWrapper(XElement element)
        {
            Element = element;
        }

        protected string GetAttributeValue(string name)
        {
            return Element.Attribute(name).Value;
        }

        protected string GetElementValue(string name)
        {
            return Element.Element(name).Value;
        }

        protected XElement GetElement(string name)
        {
            return Element.Element(name);
        }

        protected IEnumerable<XElement> GetElements(string name = null)
        {
            if (name == null)
            {
                return Element.Elements();
            }
            else
            {
                return Element.Elements(name);
            }
        }

        protected void SetAttribute(string name, object value)
        {
            Element.Attribute(name).SetValue(value);
        }

        protected void SetElement(string name, object value)
        {
            Element.Element(name).SetValue(value);
        }
    }
}
