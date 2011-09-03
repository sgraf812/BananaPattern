using System;
using System.Linq;
using System.Xml.Linq;
using BananaPattern;

namespace BananaXmlOffset.XmlElements
{
    internal class PatternResult : OperationResult
    {
        private XElement RootElement { get { return Element.Document.Root; } }
        protected virtual PatternElement PatternElement
        {
            get
            {
                var patternElements = from ele in RootElement.Elements("Pattern")
                                      select new PatternElement(ele);

                return patternElements.FirstOrDefault(ele => ele.Name == Name);
            }
        }
        public virtual string Name
        {
            get
            {
                return GetAttributeValue("Name").Trim();
            }
            set
            {
                SetAttribute("Name", value);
            }
        }

        public PatternResult()
            : base(new XElement("PatternResult",
                new XAttribute("Name", "")))
        {
        }

        public PatternResult(XElement element)
            : base(element)
        {
        }

        public override string Execute(IBotProcessContext context)
        {
            var element = PatternElement;
            if (element == null)
                throw new PatternException("There was no pattern element named \"" + Name + "\".");

            return element.FindCached(context).ToString("X");
        }
    }
}
